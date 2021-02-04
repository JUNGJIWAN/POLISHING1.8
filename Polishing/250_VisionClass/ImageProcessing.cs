using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UserInterface;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFunctionVision;

namespace WaferPolishingSystem.Vision
{
    public class ImageProcessing
    {
        string Title = "ImageProcessing";

        //---------------------------------------------------------------------------
        /**
        @fn     public void Threshold(AlignControl control, double thresh)
        @brief	AlignControl객체의 임계값 이진화.
        @return	WriteableBitmap : 결과 이미지
        @param	WriteableBitmap wImg    : 입력 이미지
        @param  ST_INSPECTION   stparam : 입력 파라메터
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:29
        */
        public WriteableBitmap Threshold(WriteableBitmap wImg, ST_INSPECTION stparam)
        {
            try
            {
                IntPtr? ptr = wImg.BackBuffer;
                if (ptr != null)
                {
                    int width   = (int)Math.Round(wImg.Width + 0.5);
                    int height  = (int)Math.Round(wImg.Height + 0.5);
                    int channel = 1;
                    int Stride = (width * channel + 3) & ~3;

                    SetRoiRectangle(new Rect(stparam.ROIX, stparam.ROIY, stparam.ROIW, stparam.ROIH));

                    IntPtr rtnptr = libThreshold((IntPtr)ptr, width, height, channel, stparam.Threshold, stparam.Sigma);

                    WriteableBitmap wb = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Gray8, null);
                    Int32Rect rect = new Int32Rect(0, 0, (int)width, (int)height);
                    wb.Lock();
                    wb.WritePixels(rect, rtnptr, (int)(Stride * height * channel), (int)width);
                    wb.AddDirtyRect(rect);
                    wb.Unlock();
                    return wb;
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return null;
        }
        /**
        @fn     public WriteableBitmap CannyEdge(WriteableBitmap wImg, ST_INSPECTION stparam)
        @brief	Canny Edge Processing
        @return	WriteableBitmap : 결과 이미지
        @param	WriteableBitmap wImg    : 입력 이미지
        @param	ST_INSPECTION   stparam : 입력 파라메터
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  9:56
        */
        public WriteableBitmap CannyEdge(WriteableBitmap wImg, ST_INSPECTION stparam)
        {
            try
            {
                IntPtr? ptr = wImg.BackBuffer;
                if (ptr != null)
                {
                    int width   = (int)Math.Round(wImg.Width + 0.5);
                    int height  = (int)Math.Round(wImg.Height + 0.5);
                    int channel = 1;
                    int Stride = (width * channel + 3) & ~3;

                    SetRoiRectangle(new Rect(stparam.ROIX, stparam.ROIY, stparam.ROIW, stparam.ROIH));

                    IntPtr rtnptr = libCanny((IntPtr)ptr, width, height, channel, stparam.Sigma, stparam.LowThreshold, stparam.HighThreshold);

                    WriteableBitmap wb = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Gray8, null);
                    Int32Rect rect = new Int32Rect(0, 0, (int)width, (int)height);
                    wb.Lock();
                    wb.WritePixels(rect, rtnptr, (int)(Stride * height * channel), (int)width);
                    wb.AddDirtyRect(rect);
                    wb.Unlock();
                    return wb;
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return null;
        }

        /**
        @fn     public bool EPDResult(WriteableBitmap wImg, ST_INSPECTION stparam, re ST_ALIGN_RESULT result)
        @brief	EPD Processing (Edge Point Detect)
        @return	bool : EPD 결과.
        @param	WriteableBitmap     wImg    : 입력 이미지
        @param	ST_INSPECTION       stparam : EPD Parameter
        @param	ref ST_ALIGN_RESULT result  : 결과 구조체
        @remark	
         - EPD는 이미지 상단부터 구간을 정의함.
         - Y값이 작은값부터 순서대로 index 부여.
         - index 0 - 1 1구간.
         - index 1 - 2 2구간.
         - 상단부터 2구간의 조건만 확인함.
         - DLL에서 Image Log 저장.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  10:28
        */
        public bool EPDResult(WriteableBitmap wImg, ST_INSPECTION stparam, ref ST_ALIGN_RESULT result, bool bAuto = true)
        {
            bool bRtn = false;
            try
            {
                int StartY = 0;
                int EndY   = 0;
                int CY     = 0;

                int nCount = 0;
                CannyEdge(wImg, stparam);

                IntPtr ptrParam = IntPtr.Zero;
                ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(stparam));
                Marshal.StructureToPtr(stparam, ptrParam, false);

                int nSecCount = libProcMeasureLen(ptrParam, bAuto);
                if(nSecCount > 0)
                {
                    fn_WriteLog(this.Title + $" : Detect Section ( {nSecCount} ).", UserEnum.EN_LOG_TYPE.ltVision);
                    nCount = nSecCount < result.stResult.Length ? nSecCount : result.stResult.Length;
                    for (int i = 0; i < nCount; i ++)
                    {
                        libGetMeasureLen(ref StartY, ref EndY, ref CY, i);
                        if (result.stResult != null)
                        {
                            result.stResult[i] = new ST_RESULT();
                            result.stResult[i].dWidth = 200;
                            result.stResult[i].dHeight = EndY - StartY;
                            result.stResult[i].dX = stparam.ROIX;
                            result.stResult[i].dY = stparam.ROIY + StartY + result.stResult[i].dHeight / 2.0;
                            // dScore = cy
                            result.stResult[i].dScore = stparam.ROIY + CY;
                        }
                        if (StartY == -2 && EndY == -2)
                        {
                            fn_WriteLog(this.Title + " : Index Out of Range", UserEnum.EN_LOG_TYPE.ltVision);
                            break;
                        }
                    }

                    double dResult1 = result.stResult[1].dScore - result.stResult[0].dScore;
                    double dResult2 = result.stResult[2].dScore - result.stResult[1].dScore;
                    dResult1 *= UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000; // mm scale
                    dResult2 *= UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000; // mm scale

                    bool bResult1 = false;
                    bool bResult2 = false;

                    if (stparam.Section1 == 0)//result <= target
                        bResult1 = dResult1 <= stparam.RefDistance;
                    else if (stparam.Section1 == 1)//result >= target
                        bResult1 = dResult1 >= stparam.RefDistance;

                    if (stparam.Section2 == 0)//result <= target
                        bResult2 = dResult2 <= stparam.RefDistance2;
                    else if (stparam.Section2 == 1)//result >= target
                        bResult2 = dResult2 >= stparam.RefDistance2;

                    switch (stparam.Condition)
                    {
                        case 0://Only 1
                            bRtn = bResult1;
                            break;
                        case 1://Only 2
                            bRtn = bResult2;
                            break;
                        case 2:// Or
                            bRtn = bResult1 && bResult2;
                            break;
                        case 3:// And
                            bRtn = bResult1 || bResult2;
                            break;
                    }

                }
                else if(nSecCount == -1)
                    fn_WriteLog(this.Title + " : Please Set ROI Image.", UserEnum.EN_LOG_TYPE.ltVision);
                else
                    fn_WriteLog(this.Title + " : Can't Detect Section.", UserEnum.EN_LOG_TYPE.ltVision);

            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return bRtn;
        }

        /**
        @fn     public void SetRoiRectagle(Rect rect)
        @brief	DLL에 ROI Setting
        @return	void
        @param	Rect rect : ROI 영역
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  10:10
        */
        public void SetRoiRectangle(Rect rect)
        {
            int x      = (int)Math.Round(rect.X      + 0.5);
            int y      = (int)Math.Round(rect.Y      + 0.5);
            int width  = (int)Math.Round(rect.Width  + 0.5);
            int height = (int)Math.Round(rect.Height + 0.5);
            libSetROI(x, y, width, height);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     public void OriginalImage(AlignControl control)
        @brief	Align Control의 이미지를 원본 으로 되돌림.
        @return	void
        @param	AlignControl control : 대상 Control
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:30
        */
        public void OriginalImage(AlignControl control)
        {
            try
            {
                control.fn_OriginalImage();
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        /**
        @fn     public void ViewEdge(AlignControl control, ST_PARAM_MODEL param)
        @brief	MIL Model Finder의 Edge 확인.
        @return	void
        @param	AlignControl   control : 대상 Control
        @param	ST_PARAM_MODEL param   : Model Finder Parameter
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  10:01
        */
        public void ViewEdge(AlignControl control, ST_PARAM_MODEL param)
        {
            try
            {
                IntPtr? ptr = control.fn_GetImgOrgPtr();
                if (ptr != null)
                {
                    int width = control.nWidth;
                    int height = control.nHeight;
                    int channel = control.nChannel;
                    int Stride = (width * channel + 3) & ~3;

                    IntPtr ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(param));
                    Marshal.StructureToPtr(param, ptrParam, false);

                    IntPtr rtnptr = libGetModelEdge((IntPtr)ptr, width, height, ptrParam);

                    WriteableBitmap wb = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Bgr24, null);
                    Int32Rect rect = new Int32Rect(0, 0, (int)width, (int)height);
                    wb.Lock();
                    wb.WritePixels(rect, rtnptr, (int)(Stride * height * channel * 3), (int)width * 3);
                    wb.AddDirtyRect(rect);
                    wb.Unlock();

                    control.SetImage(wb, Stretch.None, false);
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

    }
}
