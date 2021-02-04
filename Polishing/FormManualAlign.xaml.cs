using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UserInterface;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFunctionVision;
using static WaferPolishingSystem.Define.UserEnum;

namespace WaferPolishingSystem
{
    /// <summary>
    /// FormMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormManualAlign : Window
    {
        //bool m_bClose = false;
        //bool m_bResult = false;

        public int m_nKind;
        public string m_sTitle;
        public string m_sMsg;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //---------------------------------------------------------------------------
        public FormManualAlign()
        {
            InitializeComponent();

            //m_bResult = false;
            
        }
        //---------------------------------------------------------------------------
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            btSave.IsEnabled = SEQ._bRun ? false : true;
        }
        //---------------------------------------------------------------------------
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            Rect rect = ac_Manual.GetRectModel();
            //fn_ManualAlign(rect);
        }
        //---------------------------------------------------------------------------
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.Stop();
            this.Hide();
        }
        //---------------------------------------------------------------------------
        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //---------------------------------------------------------------------------
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            if (fdlg.ShowDialog() == true)
            {
                ac_Manual.OpenImage(fdlg.FileName);
            }
        }

        private void bn_Click_ROI(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                Rect rect = ac_Manual.GetRectModel();
                switch (ub.CommandParameter)
                {
                    case "LL":
                        rect.X--;
                        rect.Width++;
                        break;
                    case "LR":
                        rect.X++;
                        rect.Width--;
                        break;
                    case "RR":
                        rect.Width++;
                        break;
                    case "RL":
                        rect.Width--;
                        break;
                    case "TT":
                        rect.Y--;
                        rect.Height++;
                        break;
                    case "TB":
                        rect.Y++;
                        rect.Height--;
                        break;
                    case "BT":
                        rect.Height--;
                        break;
                    case "BB":
                        rect.Height++;
                        break;
                }
                Rect[] rectroi = new Rect[2];
                rectroi[0] = rect;
                ac_Manual.SetRectangle(rectroi);
                ac_Manual.DrawRectGrid();
            }
        }
        //---------------------------------------------------------------------------
        
        void fn_ManualAlign(Rect rect)
        {
            // Manual align이라고 Log 표시 할 것. (Lot Log)
            // - Map 바꿔줘야함 => Align을 Polishing으로.
            // Inposition Check해야함.
            int step_result = 0;
            //bool bErr = false;
            double dTiltInterpolation = 0.0;
            Point pntStart = new Point();
            Point pntEnd = new Point();

            Point pntMillingStart = new Point();
            Point pntMillingEnd = new Point();

            Point pntModelSpan = new Point();

            //ST_RESULT Result = new ST_RESULT();
            ST_VISION_RESULT stvisionResult = new ST_VISION_RESULT(0);

            double dPlateCenterX = MOTR[(int)EN_MOTR_ID.miSPD_X].GetPosToCmdId(EN_COMD_ID.User1);
            double dPlateCenterY = MOTR[(int)EN_MOTR_ID.miPOL_Y].GetPosToCmdId(EN_COMD_ID.User1);

            double dPlateInterpolationPosX = dPlateCenterX;
            double dPlateInterpolationPosY = 0.0;

            double dXOffset = 7;
            double dYOffset = 7;

            for (int step = 0; step < g_VisionManager.CurrentRecipe.Model[0].MillingCount; step++)
            {
                dTiltInterpolation = g_VisionManager.fn_GetTiltInterpolation(g_VisionManager.CurrentRecipe.Model[0].Milling[step].Tilt);
                fn_WriteLog(this.Title + $"Tilt Interpolation : {dTiltInterpolation.ToString("0.000")}", UserEnum.EN_LOG_TYPE.ltVision);
            
                pntStart.X = 0;
                pntStart.Y = 0;
                pntEnd.X = 0;
                pntEnd.Y = 0;
            
                SEQ_SPIND.vresult.stRecipeList[step_result].nUseMilling   = g_VisionManager.CurrentRecipe.Model[0].Milling[step].Enable;
                SEQ_SPIND.vresult.stRecipeList[step_result].dTilt         = g_VisionManager.CurrentRecipe.Model[0].Milling[step].Tilt;
                SEQ_SPIND.vresult.stRecipeList[step_result].dRPM          = g_VisionManager.CurrentRecipe.Model[0].Milling[step].RPM;
                SEQ_SPIND.vresult.stRecipeList[step_result].dForce        = g_VisionManager.CurrentRecipe.Model[0].Milling[step].Force;
                SEQ_SPIND.vresult.stRecipeList[step_result].dSpeed        = g_VisionManager.CurrentRecipe.Model[0].Milling[step].Speed;
                SEQ_SPIND.vresult.stRecipeList[step_result].dPitch        = g_VisionManager.CurrentRecipe.Model[0].Milling[step].Pitch;
                SEQ_SPIND.vresult.stRecipeList[step_result].nPathCnt      = g_VisionManager.CurrentRecipe.Model[0].Milling[step].PathCount;
                SEQ_SPIND.vresult.stRecipeList[step_result].nCycle        = g_VisionManager.CurrentRecipe.Model[0].Milling[step].Cycle;
                SEQ_SPIND.vresult.stRecipeList[step_result].nUtilType     = g_VisionManager.CurrentRecipe.Model[0].Milling[step].UtilType;
                SEQ_SPIND.vresult.stRecipeList[step_result].nUseUtilFill  = g_VisionManager.CurrentRecipe.Model[0].Milling[step].UtilFill;
                SEQ_SPIND.vresult.stRecipeList[step_result].nUseUtilDrain = g_VisionManager.CurrentRecipe.Model[0].Milling[step].UtilDrain;
                SEQ_SPIND.vresult.stRecipeList[step_result].nUseImage     = g_VisionManager.CurrentRecipe.Model[0].Milling[step].MillingImage;
                SEQ_SPIND.vresult.stRecipeList[step_result].nUseEPD       = g_VisionManager.CurrentRecipe.Model[0].Milling[step].EPD;
                SEQ_SPIND.vresult.stRecipeList[step_result].nUseToolChg   = g_VisionManager.CurrentRecipe.Model[0].Milling[step].ToolChange;
                SEQ_SPIND.vresult.stRecipeList[step_result].nToolType     = g_VisionManager.CurrentRecipe.Model[0].Milling[step].ToolType;

                SEQ_SPIND.vresult.stRecipeList[step_result].dTiltOffset = dTiltInterpolation;
                //vs.stRecipeList[step_result].nUtility = CurrentRecipe.Model[nModelNum].Milling[step].Util;
                // Milling Rect의 좌상단 좌표 1사분면으로 변환. (Start End Position 계산 때문에 Rect 중심계산 생략)
                pntMillingStart = fn_GetPositionFromImageCenter(
                    new Point(
                        g_VisionManager.CurrentRecipe.Model[0].Milling[step].MilRect.Left,
                        g_VisionManager.CurrentRecipe.Model[0].Milling[step].MilRect.Top)
                    , g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);
            
                pntMillingEnd = fn_GetPositionFromImageCenter(
                    new Point(
                        g_VisionManager.CurrentRecipe.Model[0].Milling[step].MilRect.Right,
                        g_VisionManager.CurrentRecipe.Model[0].Milling[step].MilRect.Bottom)
                    , g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);
                fn_WriteLog(this.Title + $"Manual Align Step : {step}, MilPx X: {g_VisionManager.CurrentRecipe.Model[0].Milling[step].MilRect.X.ToString("0.000")}, MilPx Y: {g_VisionManager.CurrentRecipe.Model[0].Milling[step].MilRect.Y.ToString("0.000")}, pntMilling : S({pntMillingStart.X.ToString("0.000")}, {pntMillingStart.Y.ToString("0.000")}), E({pntMillingEnd.X.ToString("0.000")}, {pntMillingEnd.Y.ToString("0.000")})", UserEnum.EN_LOG_TYPE.ltVision);
                //---------------------------------------------------------------------------
                // Tilt 보상 적용 할 것.
                //---------------------------------------------------------------------------
                // Position 계산.
                // Left
                pntStart.X = pntMillingStart.X + pntModelSpan.X;
                pntStart.X *= (g_VisionManager._RecipeVision.ResolutionX / 1000.0); // um -> mm
            
                // Top
                pntStart.Y = pntMillingStart.Y + pntModelSpan.Y;
                pntStart.Y *= (g_VisionManager._RecipeVision.ResolutionY / 1000.0); // um -> mm
                pntStart.Y += dTiltInterpolation; // (mm Scale)
            
                // Right
                pntEnd.X = pntMillingEnd.X + pntModelSpan.X;
                pntEnd.X *= (g_VisionManager._RecipeVision.ResolutionX / 1000.0); // um -> mm
            
                // Bottom
                pntEnd.Y = pntMillingEnd.Y + pntModelSpan.Y;
                pntEnd.Y *= (g_VisionManager._RecipeVision.ResolutionY / 1000.0); // um -> mm
                pntEnd.Y += dTiltInterpolation; // (mm Scale)
            
                fn_WriteLog(this.Title + $"pntStart : ({pntStart.X.ToString("0.000")}, {pntStart.Y.ToString("0.000")}), pntEnd : ({pntEnd.X.ToString("0.000")}, {pntEnd.Y.ToString("0.000")})", UserEnum.EN_LOG_TYPE.ltVision);
            
                // Start Position Direction Calculation.
                switch (g_VisionManager.CurrentRecipe.Model[0].Milling[step].StartPos)
                {
                    // Left-Bottom
                    case 0:
                        SEQ_SPIND.vresult.stRecipeList[step_result].pStartPos.X = pntStart.X;
                        SEQ_SPIND.vresult.stRecipeList[step_result].pStartPos.Y = pntEnd.Y;
                        SEQ_SPIND.vresult.stRecipeList[step_result].pEndPos.X = pntEnd.X;
                        SEQ_SPIND.vresult.stRecipeList[step_result].pEndPos.Y = pntStart.Y;
                        break;
                    // Right-Bottom
                    case 1:
                        SEQ_SPIND.vresult.stRecipeList[step_result].pStartPos.X = pntEnd.X;
                        SEQ_SPIND.vresult.stRecipeList[step_result].pStartPos.Y = pntEnd.Y;
                        SEQ_SPIND.vresult.stRecipeList[step_result].pEndPos.X = pntStart.X;
                        SEQ_SPIND.vresult.stRecipeList[step_result].pEndPos.Y = pntStart.Y;
                        break;
                }
                fn_WriteLog(this.Title + $"End Direction : ({SEQ_SPIND.vresult.stRecipeList[step_result].pStartPos.X}, {SEQ_SPIND.vresult.stRecipeList[step_result].pStartPos.Y}), pntEnd : ({SEQ_SPIND.vresult.stRecipeList[step_result].pEndPos.X}, {SEQ_SPIND.vresult.stRecipeList[step_result].pEndPos.Y})", UserEnum.EN_LOG_TYPE.ltVision);
                //---------------------------------------------------------------------------
                // Theta
                //---------------------------------------------------------------------------
                SEQ_SPIND.vresult.stRecipeList[step_result].dTheta = 0;
                //---------------------------------------------------------------------------
                step_result++;
            }
            //---------------------------------------------------------------------------
            // 좌표계 변환. => Cam 기준 상대좌표. (3사분면 좌표계)
            stvisionResult.pntModel.X *= g_VisionManager._RecipeVision.ResolutionX / 1000.0;
            stvisionResult.pntModel.Y *= g_VisionManager._RecipeVision.ResolutionY / 1000.0;
            //---------------------------------------------------------------------------
            int nTotal = stvisionResult.nTotalStep;
            double dXpos  = MOTR.GetEncPos(EN_MOTR_ID.miSPD_X);
            double dYpos  = MOTR.GetEncPos(EN_MOTR_ID.miPOL_Y);
            double dTHpos = MOTR.GetEncPos(EN_MOTR_ID.miPOL_TH);

            //Save X,Y Value
            for (int n = 0; n < nTotal; n++)
            {
                //		+ (Spindle)
                //	□ (Cam)
                // Spindle X Pos Value > Cam X Pos Value
                // Spindle이 더가야하므로 Offset + 부호
            
                //	+ (Spindle)
                //		□ (Cam)
                // Spindle X Pos Value < Cam X Pos Value
                // Spindle이 덜가야하므로 Offset - 부호
            
                stvisionResult.stRecipeList[n].dStartX = dXpos - stvisionResult.stRecipeList[n].pStartPos.X + g_VisionManager._RecipeVision.SpindleOffsetX;
                stvisionResult.stRecipeList[n].dEndX   = dXpos - stvisionResult.stRecipeList[n].pEndPos.X   + g_VisionManager._RecipeVision.SpindleOffsetX;
            
                stvisionResult.stRecipeList[n].dStartY = dYpos + stvisionResult.stRecipeList[n].pStartPos.Y - g_VisionManager._RecipeVision.SpindleOffsetY;
                stvisionResult.stRecipeList[n].dEndY   = dYpos + stvisionResult.stRecipeList[n].pEndPos.Y   - g_VisionManager._RecipeVision.SpindleOffsetY;
            
                fn_WriteLog($"-----------------------------Result Process------------------------", EN_LOG_TYPE.ltVision);
                fn_WriteLog($"{n} - Start Pos : {stvisionResult.stRecipeList[n].dStartX}, {stvisionResult.stRecipeList[n].dStartY}", EN_LOG_TYPE.ltVision);
                fn_WriteLog($"{n} - End Pos   : {stvisionResult.stRecipeList[n].dEndX}, {stvisionResult.stRecipeList[n].dEndY}", EN_LOG_TYPE.ltVision);
                stvisionResult.stRecipeList[n].dPosTH = dTHpos;
            
                //---------------------------------------------------------------------------
                // System Offset
                //---------------------------------------------------------------------------
                stvisionResult.stRecipeList[n].dStartX += FM.m_stProjectBase.dPolishOffset_X;
                stvisionResult.stRecipeList[n].dEndX   += FM.m_stProjectBase.dPolishOffset_X;
                stvisionResult.stRecipeList[n].dStartY += FM.m_stProjectBase.dPolishOffset_Y;
                stvisionResult.stRecipeList[n].dEndX   += FM.m_stProjectBase.dPolishOffset_Y;
                stvisionResult.stRecipeList[n].dTilt   += FM.m_stProjectBase.dPolishOffset_TI;

                //---------------------------------------------------------------------------
                // Theta Offset 은 추가 스탭 작업 요함.
                // => Theta 가공은 Align 이후 특정 각도를 돌려서 가공을 하기위함 이므로.
                //stvisionResult.stRecipeList[n].dPosTH += FM.m_stProjectBase.dPolishOffset_TH;
                //---------------------------------------------------------------------------
            
                //---------------------------------------------------------------------------
                //	Vision Align InPosition Error
                //---------------------------------------------------------------------------
                // Interpolation Tilt.
                dPlateInterpolationPosY = dPlateCenterY + g_VisionManager.fn_GetTiltInterpolation(stvisionResult.stRecipeList[n].dTilt);
            
                //vresult.stRecipeList[n].dTiltOffset = g_VisionManager.fn_GetTiltInterpolation(stvisionResult.stRecipeList[n].dTilt);
                //Console.WriteLine($"TiltOffset- Offset - {n}:{stvisionResult.stRecipeList[n].dTiltOffset}, Tilt : {stvisionResult.stRecipeList[n].dTilt}");
                //fn_WriteLog($"TiltOffset- Offset - {n}:{stvisionResult.stRecipeList[n].dTiltOffset}, Tilt : {stvisionResult.stRecipeList[n].dTilt}");
                
                if (stvisionResult.stRecipeList[n].dStartX < dPlateInterpolationPosX - dXOffset || stvisionResult.stRecipeList[n].dStartX > dPlateInterpolationPosX + dXOffset)
                {
                    fn_WriteLog($"Start X InPosition Error.{n}", EN_LOG_TYPE.ltVision);
                    //bErr = true;
                }
                if (stvisionResult.stRecipeList[n].dEndX < dPlateInterpolationPosX - dXOffset || stvisionResult.stRecipeList[n].dEndX > dPlateInterpolationPosX + dXOffset)
                { 
                    //bErr = true; 
                }
                if (stvisionResult.stRecipeList[n].dStartY < dPlateInterpolationPosY - dYOffset || stvisionResult.stRecipeList[n].dStartY > dPlateInterpolationPosY + dYOffset)
                {
                    //bErr = true;
                }
                if (stvisionResult.stRecipeList[n].dEndY < dPlateInterpolationPosY - dYOffset || stvisionResult.stRecipeList[n].dEndY > dPlateInterpolationPosY + dYOffset)
                {
                    //bErr = true;
                }
            }


            //File Save
            SEQ_SPIND.fn_LoadVisnResult(false);
            DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolish);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ac_Manual.SetMove(AlignControl.EnRoiMode.ModeDraw);
        }
    }
}