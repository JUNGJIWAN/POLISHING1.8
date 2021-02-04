using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserFunctionVision;
using static WaferPolishingSystem.Define.UserConstVision;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserEnumVision;
using System.Windows;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace WaferPolishingSystem.Vision
{
    /**
    @class	ToolStorageAlign
    @brief	ToolStorage Align을 위한 클래스.
    @remark	
     - 최종 리턴 좌표계는 Work 좌표계 기준으로.
     - Sequence LT->RT->RB->LB->Cal
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/12/9  11:11
    */
    public class ToolStorageAlign
    {
        string Title = "ToolStorageAlign";
        DateTime _DateTime;
        public struct stPos 
        {
            public int   Count;
            public Point[] GrabPos  ;
            public Point[] SearchPos;
            public char[]  PosH;
            public char[]  PosV;
            public stPos(bool bInit = false)
            {
                Count     = 0;
                PosH      = new char [(int)EN_TOOLSTORAGE_POS.MemberCount];
                PosV      = new char [(int)EN_TOOLSTORAGE_POS.MemberCount];
                GrabPos   = new Point[(int)EN_TOOLSTORAGE_POS.MemberCount];
                SearchPos = new Point[(int)EN_TOOLSTORAGE_POS.MemberCount];
                for(int i = 0; i < GrabPos.Length; i++)
                {
                    GrabPos  [i] = new Point();
                    SearchPos[i] = new Point();
                }
            }
        }

        stPos _Pos = new stPos(false);
        ST_ALIGN_RESULT _stResult = new ST_ALIGN_RESULT();
        public bool fn_PinSearch(double posx, double posy, int nPos ,WriteableBitmap img = null)
        {
            bool bRtn = false;
            
            _Pos.Count = nPos;

            if (_Pos.Count == 0)
                _DateTime = DateTime.Now;

            // Position 처리.
            _Pos.GrabPos[_Pos.Count].X = posx;
            _Pos.GrabPos[_Pos.Count].Y = posy;
            
            string strPath = $"D:\\Image\\{_DateTime:yyyyMMdd}\\ToolStorage\\";
            // Image 처리.
            if (img == null)
                img = UserClass.g_VisionManager._CamManager.fn_GetFrame();
            if (img == null)
                return bRtn;
            // Image Save.

            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            try
            {
                using (FileStream stream = new FileStream(strPath + $"{_DateTime:yyyyMMdd_HHmm}_Pin{_Pos.Count + 1}_{posx:F3}_{posy:F3}.bmp", FileMode.Create))
                {
                    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(img));
                    encoder.Save(stream);
                    fn_WriteLog(this.Title + $"Image Saved. {stream.Name}", UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                Console.WriteLine(ex.Message);
            }

            libSetImage(img.BackBuffer, img.PixelWidth, img.PixelHeight, img.Format.BitsPerPixel / 8);
            // 원 지름 : 210, 스무딩 : 99%
            libPinSearch(UserClass.g_VisionManager._RecipeVision.PinRadiusPixel, UserClass.g_VisionManager._RecipeVision.PinSmooth);
            IntPtr pResult = Marshal.AllocHGlobal(Marshal.SizeOf(_stResult));
            Marshal.StructureToPtr(_stResult, pResult, true);
            libGetResult(pResult);
            fn_WriteLog(this.Title + " : Get Result.", UserEnum.EN_LOG_TYPE.ltVision);
            _stResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(pResult, typeof(ST_ALIGN_RESULT));

            int nMaxScorePos = -1;
            double dMaxValue = 0.0;
            if(_stResult.NumOfFound > 0)
            {
                for(int resultcnt = 0; resultcnt < _stResult.NumOfFound; resultcnt++)
                {
                    if (_stResult.stResult[resultcnt].dScore > dMaxValue)
                    {
                        dMaxValue = _stResult.stResult[resultcnt].dScore;
                        nMaxScorePos = resultcnt;
                    }
                }
                if (nMaxScorePos > -1)
                {
                    _Pos.SearchPos[_Pos.Count].X = _stResult.stResult[nMaxScorePos].dX;
                    _Pos.SearchPos[_Pos.Count].Y = _stResult.stResult[nMaxScorePos].dY;
                    fn_WriteLog(this.Title + $"Index : {_Pos.Count} Search X : {_Pos.SearchPos[_Pos.Count].X}, Search Y : {_Pos.SearchPos[_Pos.Count].Y}", UserEnum.EN_LOG_TYPE.ltVision);
                    _Pos.Count++;
                    bRtn = true;
                }
            }
            fn_WriteLog(this.Title + $"Searched Num : {_stResult.NumOfFound}, MAX Score Pos : {nMaxScorePos}", UserEnum.EN_LOG_TYPE.ltVision);

            return bRtn;
        }

        public bool fn_CheckSearchResultInPosition(out Point pntMove)
        {
            pntMove = new Point();
            Point pnt = new Point();
            double dCamWidth2 = UserClass.g_VisionManager._RecipeVision.CamWidth / 2.0;
            double dCamHeight2 = UserClass.g_VisionManager._RecipeVision.CamHeight / 2.0;
            bool bRet = false;
            if (_Pos.Count > 0)
            {
                pnt.X = dCamWidth2 - _Pos.SearchPos[_Pos.Count - 1].X;  // 이동량 변환.
                pnt.Y = dCamHeight2 - _Pos.SearchPos[_Pos.Count - 1].Y;
                fn_WriteLog(this.Title + $"Point Move(px) : ( {pnt.X}, {pnt.Y}", UserEnum.EN_LOG_TYPE.ltVision);
                pntMove.X = pnt.X * UserClass.g_VisionManager._RecipeVision.ResolutionX; // pixel -> um Scale.
                pntMove.Y = pnt.Y * UserClass.g_VisionManager._RecipeVision.ResolutionY;
                pntMove.X /= 1000.0; // mm Scale.
                pntMove.Y /= 1000.0;

                // Search 결과 좌표의 절대치 비교.
                fn_WriteLog(this.Title + $"Current Pos (mm) : {_Pos.GrabPos[_Pos.Count-1].X}, {_Pos.GrabPos[_Pos.Count-1].Y}", UserEnum.EN_LOG_TYPE.ltVision);
                fn_WriteLog(this.Title + $"Point Move(mm) : ( {pntMove.X}, {pntMove.Y}", UserEnum.EN_LOG_TYPE.ltVision);
                if (Math.Abs(pnt.X) < UserClass.g_VisionManager._RecipeVision.InpositionPixelX && 
                    Math.Abs(pnt.Y) < UserClass.g_VisionManager._RecipeVision.InpositionPixelY)
                    bRet = true;
            }
            
            return bRet;
        }

        public bool fn_ProcessOffset(out Point ColOffset, out Point RowOffset, out Point RefToolPos, int RefIdx = 0)
        {
            bool bRet = false;
            bool bFourPoint = false;
            
            ColOffset  = new Point();
            RowOffset  = new Point();
            RefToolPos = new Point();

            if (_Pos.Count > 0 && _Pos.Count == 4)
            {
                int idx = 0;
                //---------------------------------------------------------------------------
                // Position 판단. (L,T,R,B)
                double dPosAvgX = 0;
                double dPosAvgY = 0;
                for (int i = 0; i < _Pos.Count; i++)
                {
                    dPosAvgX += _Pos.GrabPos[i].X;
                    dPosAvgY += _Pos.GrabPos[i].Y;
                }

                dPosAvgX /= _Pos.Count;
                dPosAvgY /= _Pos.Count;

                for (int i = 0; i < _Pos.Count; i++)
                {
                    _Pos.PosH[i] = dPosAvgX > _Pos.GrabPos[i].X ? 'R' : 'L';
                    _Pos.PosV[i] = dPosAvgY > _Pos.GrabPos[i].Y ? 'B' : 'T';
                }
                //---------------------------------------------------------------------------
                // Cal Angle
                double dHalfWidth = UserClass.g_VisionManager._RecipeVision.CamWidth / 2.0;
                double dHalfHeight = UserClass.g_VisionManager._RecipeVision.CamHeight / 2.0;
                Point PinLB = new Point();
                Point PinRB = new Point();
                Point PinLT = new Point();
                Point PinRT = new Point();

                // 실제 Pin Position 계산.
                // X축 : GrabPos - SearchPos * ResolutionX + SpindleOffsetX
                // Y축 : GrabPos + SearchPos * ResolutionY - SpindleOffsetY
                // ※ SearchPos는 1사분면 좌표계로 변환된 값임.
                // ※ 위 식은 Polishing Bath Seq에서 참조함.
                // Pin Pos After Align.
                idx = fn_GetIndexPosition(EN_TOOLSTORAGE_POS.LeftBottom);
                PinLB.X = _Pos.GrabPos[idx].X - (_Pos.SearchPos[idx].X - dHalfWidth)  * UserClass.g_VisionManager._RecipeVision.ResolutionX / 1000.0 + UserClass.g_VisionManager._RecipeVision.SpindleOffsetX;
                PinLB.Y = _Pos.GrabPos[idx].Y + (dHalfHeight - _Pos.SearchPos[idx].Y) * UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000.0 - UserClass.g_VisionManager._RecipeVision.SpindleOffsetY;

                idx = fn_GetIndexPosition(EN_TOOLSTORAGE_POS.LeftTop);
                PinLT.X = _Pos.GrabPos[idx].X - (_Pos.SearchPos[idx].X - dHalfWidth)  * UserClass.g_VisionManager._RecipeVision.ResolutionX / 1000.0 + UserClass.g_VisionManager._RecipeVision.SpindleOffsetX;
                PinLT.Y = _Pos.GrabPos[idx].Y + (dHalfHeight - _Pos.SearchPos[idx].Y) * UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000.0 - UserClass.g_VisionManager._RecipeVision.SpindleOffsetY;

                idx = fn_GetIndexPosition(EN_TOOLSTORAGE_POS.RightTop);
                PinRT.X = _Pos.GrabPos[idx].X - (_Pos.SearchPos[idx].X - dHalfWidth)  * UserClass.g_VisionManager._RecipeVision.ResolutionX / 1000.0 + UserClass.g_VisionManager._RecipeVision.SpindleOffsetX;
                PinRT.Y = _Pos.GrabPos[idx].Y + (dHalfHeight - _Pos.SearchPos[idx].Y) * UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000.0 - UserClass.g_VisionManager._RecipeVision.SpindleOffsetY;

                idx = fn_GetIndexPosition(EN_TOOLSTORAGE_POS.RightBottom);
                PinRB.X = _Pos.GrabPos[idx].X - (_Pos.SearchPos[idx].X - dHalfWidth)  * UserClass.g_VisionManager._RecipeVision.ResolutionX / 1000.0 + UserClass.g_VisionManager._RecipeVision.SpindleOffsetX;
                PinRB.Y = _Pos.GrabPos[idx].Y + (dHalfHeight - _Pos.SearchPos[idx].Y) * UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000.0 - UserClass.g_VisionManager._RecipeVision.SpindleOffsetY;

                double dAngle = 90.0;
                if (PinLB.X != PinLT.X)
                {
                    dAngle = Math.Atan2(PinLT.Y - PinLB.Y, PinLT.X - PinLB.X);
                }

                // 4Point Angle Cal
                if (bFourPoint)
                {
                    dAngle += Math.Atan2(PinRT.Y - PinRB.Y, PinRT.X - PinRB.X);
                    dAngle /= 2.0;
                }
                //---------------------------------------------------------------------------
                // Pin - Ref Tool 간 관계

                Point LBTool = PinLB;
                Point RBTool = PinRB;
                Point RTTool = PinRT;
                Point LTTool = PinLT;

                
                //---------------------------------------------------------------------------
                ColOffset.X = (PinRB.X - PinLB.X) / 2.0;
                ColOffset.Y = (PinRB.Y - PinLB.Y) / 2.0;

                RowOffset.X = (PinLT.X - PinLB.X) / 5.0;
                RowOffset.Y = (PinLT.Y - PinLB.Y) / 5.0;

                double dRow = -2.5;
                double dCol = 1.5;
                RefToolPos.X = PinLB.X + (RowOffset.X * dRow) - (ColOffset.X * dCol);
                RefToolPos.Y = PinLB.Y + (RowOffset.Y * dRow) - (ColOffset.Y * dCol);
                UserClass.g_VisionManager._RecipeVision.RefToolX = RefToolPos.X;
                UserClass.g_VisionManager._RecipeVision.RefToolY = RefToolPos.Y;

                fn_WriteLog($"LB : ({LBTool.X}, {LBTool.Y}) ", UserEnum.EN_LOG_TYPE.ltVision);
                fn_WriteLog($"ColOffset : ({ColOffset}), RowOffset : ({RowOffset}) ", UserEnum.EN_LOG_TYPE.ltVision);
                
                //---------------------------------------------------------------------------

                // 각도 Degree -> Radian
                // Radian -> Degree
                /*
                fn_GetRotateTransfer(ref LBTool, ref RBTool, ref RTTool, ref LTTool, dAngle - Math.PI / 2.0);
                fn_WriteLog($"LB : ({LBTool.X}, {LBTool.Y}) RB : ({RBTool.X}, {RBTool.Y}) RT : ({RTTool.X}, {RTTool.Y}) LT : ({LTTool.X}, {LTTool.Y})", UserEnum.EN_LOG_TYPE.ltVision);
                //---------------------------------------------------------------------------
                // Out Offset Data.
                ColOffset.X  = (RBTool.X - LBTool.X) / ((double)UserClass.g_VisionManager._RecipeVision.ColToolCount - 1);
                ColOffset.Y  = (RBTool.Y - LBTool.Y) / ((double)UserClass.g_VisionManager._RecipeVision.ColToolCount - 1);
                ColOffset.X += (RTTool.X - LTTool.X) / ((double)UserClass.g_VisionManager._RecipeVision.ColToolCount - 1);
                ColOffset.Y += (RTTool.Y - LTTool.Y) / ((double)UserClass.g_VisionManager._RecipeVision.ColToolCount - 1);
                ColOffset.X /= 2.0;
                ColOffset.Y /= 2.0;

                RowOffset.X  = (LTTool.X - LBTool.X) / ((double)UserClass.g_VisionManager._RecipeVision.RowToolCount - 1);
                RowOffset.Y  = (LTTool.Y - LBTool.Y) / ((double)UserClass.g_VisionManager._RecipeVision.RowToolCount - 1);
                RowOffset.X += (RTTool.X - RBTool.X) / ((double)UserClass.g_VisionManager._RecipeVision.RowToolCount - 1);
                RowOffset.Y += (RTTool.Y - RBTool.Y) / ((double)UserClass.g_VisionManager._RecipeVision.RowToolCount - 1);
                RowOffset.X /= 2.0;
                RowOffset.Y /= 2.0;

                // RowOffset 방향 반대

                UserClass.g_VisionManager._RecipeVision.RefToolX = LBTool.X;
                UserClass.g_VisionManager._RecipeVision.RefToolY = LBTool.Y;
                switch (RefIdx)
                {
                    case 0: RefToolPos = LBTool; break;
                    case 1: RefToolPos = LTTool; break;
                    case 2: RefToolPos = RTTool; break;
                    case 3: RefToolPos = RBTool; break;
                }
                */
                // 10 도 이상 틀어지면 False 반환.
                if (ColOffset.X <= UserClass.g_VisionManager._RecipeVision.TooltoToolDistanceX * Math.Cos(UserClass.g_VisionManager._RecipeVision.AllowAngle / 180.0 * Math.PI) && 
                    RowOffset.Y <= UserClass.g_VisionManager._RecipeVision.TooltoToolDistanceY * Math.Cos(UserClass.g_VisionManager._RecipeVision.AllowAngle / 180.0 * Math.PI))
                {
                    bRet = false;
                }
                else
                    bRet = true;
            }
            else
                bRet = false;
            return bRet;
        }

        public void fn_GetDir(out char[] chV, out char[] chH)
        {
            chV = new char[4];
            chH = new char[4];
            _Pos.PosV.CopyTo(chV, 0);
            _Pos.PosH.CopyTo(chH, 0);
        }

        public void fn_GetSearchPos(out Point[] posSearch)
        {
            posSearch = new Point[4];
            for(int i = 0; i < posSearch.Length; i++)
            {
                posSearch[i] = _Pos.SearchPos[i];
            }
        }

        private void fn_GetRotateTransfer(ref Point LB, ref Point RB, ref Point RT, ref Point LT, double Angle)
        {
            double dPTRelationX = UserClass.g_VisionManager._RecipeVision.CentertoToolRelationX;
            double dPTRelationY = UserClass.g_VisionManager._RecipeVision.CentertoToolRelationY;
            double dTTDisX      = UserClass.g_VisionManager._RecipeVision.TooltoToolDistanceX;
            double dTTDisY      = UserClass.g_VisionManager._RecipeVision.TooltoToolDistanceY;
            double dColCount    = UserClass.g_VisionManager._RecipeVision.ColToolCount - 1;
            double dRowCount    = UserClass.g_VisionManager._RecipeVision.RowToolCount - 1;

            // x'=xcosth-ysinth
            // y'=xsinth+ycosth
            Point posResult = new Point();
            Point posRef = new Point();

            posRef.X = (LB.X + RB.X + RT.X + LT.X) / 4.0;
            posRef.Y = (LB.Y + RB.Y + RT.Y + LT.Y) / 4.0;
            fn_WriteLog($"Center Position_Mean : ( {posRef} )", UserEnum.EN_LOG_TYPE.ltVision);
            double a1 = 0.0, a2 = 0.0;
            double b1 = 0.0, b2 = 0.0;
            double x1 = 0.0, x2 = 0.0;
            fn_GetLineParam(LB, RT, out a1, out b1, out x1);
            fn_GetLineParam(LT, RB, out a2, out b2, out x2);
            posRef = fn_GetLineIntersectionPoint(a1, b1, x1, a2, b2, x2);
            fn_WriteLog($"Center Position_Calc : ( {posRef} )",UserEnum.EN_LOG_TYPE.ltVision);
            
            // LB Tool 회전 변환
            posResult.X = dPTRelationX * dTTDisX * Math.Cos(Angle) - dPTRelationY * dTTDisY * Math.Sin(Angle);
            posResult.Y = dPTRelationX * dTTDisX * Math.Sin(Angle) + dPTRelationY * dTTDisY * Math.Cos(Angle);

            LB.X = posRef.X + posResult.X;
            LB.Y = posRef.Y + posResult.Y;

            // RB Tool 회전변환
            posResult.X = (dPTRelationX - dColCount) * dTTDisX * Math.Cos(Angle) - (dPTRelationY) * dTTDisY * Math.Sin(Angle);
            posResult.Y = (dPTRelationX - dColCount) * dTTDisX * Math.Sin(Angle) + (dPTRelationY) * dTTDisY * Math.Cos(Angle);

            RB.X = posRef.X + posResult.X;
            RB.Y = posRef.Y + posResult.Y;

            // RT Tool 회전변환
            posResult.X = (dPTRelationX - dColCount) * dTTDisX * Math.Cos(Angle) - (dPTRelationY + dRowCount) * dTTDisY * Math.Sin(Angle);
            posResult.Y = (dPTRelationX - dColCount) * dTTDisX * Math.Sin(Angle) + (dPTRelationY + dRowCount) * dTTDisY * Math.Cos(Angle);

            RT.X = posRef.X + posResult.X;
            RT.Y = posRef.Y + posResult.Y;

            // LT Tool 회전 변환
            posResult.X = (dPTRelationX) * dTTDisX * Math.Cos(Angle) - (dPTRelationY + dRowCount) * dTTDisY * Math.Sin(Angle);
            posResult.Y = (dPTRelationX) * dTTDisX * Math.Sin(Angle) + (dPTRelationY + dRowCount) * dTTDisY * Math.Cos(Angle);

            LT.X = posRef.X + posResult.X;
            LT.Y = posRef.Y + posResult.Y;
        }

        private int fn_GetIndexPosition(EN_TOOLSTORAGE_POS enPos)
        {
            int index = -1;
            if(_Pos.Count > 0)
            {
                for(int i = 0; i < _Pos.Count; i++)
                {
                    switch (enPos)
                    {
                        case UserEnumVision.EN_TOOLSTORAGE_POS.LeftTop:
                            if (_Pos.PosH[i] == 'L' && _Pos.PosV[i] == 'T')
                                index = i;
                            break;
                        case UserEnumVision.EN_TOOLSTORAGE_POS.RightTop:
                            if (_Pos.PosH[i] == 'R' && _Pos.PosV[i] == 'T')
                                index = i;
                            break;
                        case UserEnumVision.EN_TOOLSTORAGE_POS.RightBottom:
                            if (_Pos.PosH[i] == 'R' && _Pos.PosV[i] == 'B')
                                index = i;
                            break;
                        case UserEnumVision.EN_TOOLSTORAGE_POS.LeftBottom:
                            if (_Pos.PosH[i] == 'L' && _Pos.PosV[i] == 'B')
                                index = i;
                            break;
                    }
                }
            }

            return index;
        }

        private void fn_GetLineParam(Point input1, Point input2, out double a, out double b, out double x)
        {
            if(Math.Abs(input1.X - input2.X) == 0)
            {
                a = 0;
                b = 0;
                x = input1.X;
            }
            else
            {
                a = (input2.Y - input1.Y) / (input2.X - input1.X);
                b = input1.Y - a * input1.X;
                x = 0;
            }
        }

        private Point fn_GetLineIntersectionPoint(double a1, double b1, double x1, double a2, double b2, double x2)
        {
            Point pntRtn = new Point();
            if(x1 == 0)
            {
                if (x2 == 0)
                    pntRtn.X = (b2 - b1) / (a1 - a2);
                else
                    pntRtn.X = x2;
            }
            else
            {
                pntRtn.X = x1;
            }
            pntRtn.Y = a1 * pntRtn.X + b1;

            return pntRtn;
        }
    }
}
