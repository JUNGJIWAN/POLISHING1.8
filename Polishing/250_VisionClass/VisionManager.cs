using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using WaferPolishingSystem.Define;
using UserInterface;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFunctionVision;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserConstVision;
using System.Text;

namespace WaferPolishingSystem.Vision
{
    public class VisionManager
    {
        string Title = "VisionManager_ ";
        public Label lbMain = null;
        //Class
        public RecipeManager            _RecipeManager  = new RecipeManager         ();
        public CameraManager            _CamManager     = new CameraManager         ();
        public AlignManager             _AlignManager   = new AlignManager          ();
        public ToolStorageAlign         _ToolAlign      = new ToolStorageAlign      ();
        public LightControllerManager   _LightManger    = new LightControllerManager();

        public SystemRecipeVision       _RecipeVision   = new SystemRecipeVision    ();
        public RecipeList               _RecipeModel    = new RecipeList            ();
        public RecipeList               CurrentRecipe   = new RecipeList            ();
        public ImageProcessing          _ImgProc        = new ImageProcessing       ();
        public AlignControl             _AlginMainCtrl  = null;
        public AlignControl             _AlginPreCtrl   = null;
        public AlignControl             _AlginPolCtrl   = null;

        public DelUpdateMainResult delUpdateMainresult      = null;
        public DelClearMainResult delUpdateMainresultClear = null;

        int _LastLightIRValue = 0;
        int _LastLightWValue = 0;

        double dAlignedTheta = 0.0;

        public string _strImageLogPath = @"D:\Image\";

        const int UNDERSCORESEARCHTIME = 3;

        int _nDetectedIdx = -1;
        public int SearchedIndex
        {
            get { return _nDetectedIdx; }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_LoadVision(string strRecipe)
        @brief	Vision Loading.
        @return	void
        @param	string strRecipe : Recipe Name
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:11
        */
        public void fn_LoadVision(string strRecipe)
        {
            fn_InitDLL();

            _RecipeManager.fn_ReadVisionRecipe();

            fn_LoadRecipe(strRecipe);

            _LightManger.Serial_Conn(_RecipeVision.LightPort, _RecipeVision.LightBaudRate);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     public bool fn_LoadRecipe(string strRecipe)
        @brief	Recipe Load.
        @return	bool : Recipe Load 성공 여부.
        @param	string strRecipe : Recipe 이름
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  14:40
        */
        public bool fn_LoadRecipe(string strRecipe)
        {
            if (File.Exists(STRRECIPEPATH + strRecipe + "\\" + strRecipe + ".ini"))
            {
                _RecipeManager.fn_ReadRecipe("", strRecipe);
                _RecipeManager.fn_ReadRecipeCleaning(strRecipe);
                _RecipeModel.CopyTo(CurrentRecipe);

                fn_WriteLog($"Recipe Open : {strRecipe}", UserEnum.EN_LOG_TYPE.ltVision);
                
                return true; 
            }
            else
            {
                // Error
                strRecipe = "";
                _RecipeManager.fn_WriteVisionRecipe();

                return false;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_SaveVision(string strRecipe)
        @brief	Vision Param 저장.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:10
        */
        public void fn_SaveVision(string strRecipe)
        {
            _RecipeManager.fn_WriteVisionRecipe();
            _RecipeManager.fn_WriteRecipe();
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_UnLoadVision()
        @brief	Vision 객체 해제.
        @return	void
        @param	void
        @remark	
         - Vision 객체 해제.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:09
        */
        public void fn_UnLoadVision()
        {
            fn_DeinitDLL();
            _RecipeManager.fn_WriteVisionRecipe();
            _CamManager.fn_GrabStop();
            _CamManager.fn_CloseCam();
            _LightManger.Serial_DisConn();
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void fn_InitDLL()
        @brief	C++ DLL Lib 초기화.
        @return	void    
        @param	void
        @remark	
         - C++ DLL 초기화 호출.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:09
        */
        private void fn_InitDLL()
        {
            try
            {
                libInit();
                if(libCheckLicense() == 0)
                    fn_WriteLog(this.Title + " : Can't Detected Matrox Imaging Library License Dongle.", UserEnum.EN_LOG_TYPE.ltVision);
                else
                    fn_WriteLog(this.Title + " : Checked Matrox Imaging Library License Dongle."       , UserEnum.EN_LOG_TYPE.ltVision);
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void fn_DeinitDLL()
        @brief	C++ DLL Lib 해제.
        @return	void
        @param	void
        @remark	
         - C++ DLL 해제 호출.
         - ModelFinder 객체 파괴.
         - PatternMatching 객체 파괴.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:08
        */
        private void fn_DeinitDLL()
        {
            try
            {
                libDestroyModel();
                libDestroyPattern();
                libDestroy();
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            
        }

        /**
        @fn     public bool fn_PreAlign(ref ST_VISION_RESULT vresult, bool bSimul = false)
        @brief	시퀀스상 PreAlign호출 함수.
        @return	bool : PreAlign 성공 여부
        @param	ref ST_VISION_RESULT vresult : 결과 구조체 
        @param	    bool             bSimul  : Simul Mode 여부
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:22
        */
        public bool fn_PreAlign(ref ST_VISION_RESULT vresult, bool bSimul = false)
        {
            bool bOk = false;
            int nDetectedIdx = -1;
            try
            {
                //Local Var.
                ST_VISION_RESULT vs = new ST_VISION_RESULT(0);
                ST_ALIGN_RESULT AlignResult = new ST_ALIGN_RESULT();
                AlignResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                {
                    AlignResult.stResult[i] = new ST_RESULT();
                }

                WriteableBitmap wbImg;
                // Get Frame
                if (bSimul)
                {
                    BitmapImage bmpImg = new BitmapImage();
                    FileStream source = File.OpenRead(CurrentRecipe.Model[0].strImgPath);
                    bmpImg.BeginInit();
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.StreamSource = source;
                    bmpImg.EndInit();
                    wbImg = new WriteableBitmap(bmpImg);
                }
                else
                    wbImg = _CamManager.fn_GetFrame();

                try
                {
                    string LogPath = _strImageLogPath + DateTime.Now.ToString("yyyyMMdd") + @"\";
                    if (!Directory.Exists(LogPath))
                        Directory.CreateDirectory(LogPath);
                    string strName = "PreAlign_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".bmp";
                    fn_SaveImage(wbImg, LogPath + strName);
                    lbMain.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        lbMain.Content = strName;
                    }));
                }
                catch(Exception ex)
                {
                    fn_WriteLog(this.Title + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }

                if (wbImg == null)
                {
                    fn_WriteLog(this.Title + "Cam Grab Error.", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                if (CurrentRecipe == null)
                {
                    fn_WriteLog(this.Title + "Can't Not Loaded Recipe. (CurrentRecipe is null.)", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                libSetImage(wbImg.BackBuffer, wbImg.PixelWidth, wbImg.PixelHeight, 1);

                for (int nModelNum = 0; nModelNum < CurrentRecipe.Model.Length; nModelNum++)
                {
                    if (CurrentRecipe.Model[nModelNum].Enable == 0)
                        continue;

                    if (!File.Exists(CurrentRecipe.Model[nModelNum].strLoadingPath))
                    {
                        fn_WriteLog(this.Title + $"{CurrentRecipe.Model[nModelNum].strLoadingPath} Exists Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                        //return false;
                        continue;
                    }
                    BitmapImage bmpImg = new BitmapImage();
                    FileStream source = File.OpenRead(CurrentRecipe.Model[nModelNum].strLoadingPath);
                    bmpImg.BeginInit();
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.StreamSource = source;
                    bmpImg.EndInit();

                    WriteableBitmap wbMark = new WriteableBitmap(bmpImg);
                    if (wbMark == null)
                    {
                        fn_WriteLog(this.Title + "Mark Read Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                        return false;
                    }

                    libSetMark(wbMark.BackBuffer, wbMark.PixelWidth, wbMark.PixelHeight, 1);

                    IntPtr ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(CurrentRecipe.Model[nModelNum].LoadingParam));
                    Marshal.StructureToPtr(CurrentRecipe.Model[nModelNum].LoadingParam, ptrParam, false);

                    if (ptrParam != IntPtr.Zero)
                    {
                        libSetParam(ptrParam, (IntPtr)null);
                        Marshal.FreeHGlobal(ptrParam);

                        libRunProcModel();
                        ST_ALIGN_RESULT stResult = new ST_ALIGN_RESULT();
                        stResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                        for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                        {
                            stResult.stResult[i] = new ST_RESULT();
                        }
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));

                        Marshal.StructureToPtr(stResult, ptr, true);
                        libGetResult(ptr);

                        AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                        Marshal.FreeHGlobal(ptr);

                        libDestroyModel();

                        bOk = true;
                        if (AlignResult.NumOfFound > 0)
                        {
                            nDetectedIdx = nModelNum;
                            fn_WriteLog(this.Title + $" : Pre-Align Model Detected Index : {nDetectedIdx + 1}, Name : {CurrentRecipe.Model[nDetectedIdx].strName}", UserEnum.EN_LOG_TYPE.ltVision);
                            fn_WriteLog(this.Title + $" : Pre-Align Model Detected Index : {nDetectedIdx + 1}, Name : {CurrentRecipe.Model[nDetectedIdx].strName}", UserEnum.EN_LOG_TYPE.ltLot   );
                            break;
                        }
                    }
                    else
                    {
                        fn_WriteLog(this.Title + " : Invalid Pointer", UserEnum.EN_LOG_TYPE.ltVision);
                        bOk = false;
                    }
                }
                // Model Num 필요함.

                // 최대 스코어 결과 처리.
                ST_RESULT Result = new ST_RESULT();
                int nIdx = 0;
                double dMaxScore = 0.00;
                if (AlignResult.NumOfFound > 0)
                {
                    fn_WriteLog(this.Title + $" Pre-Align Result From Lib : Total     - {AlignResult.NumOfFound} ", UserEnum.EN_LOG_TYPE.ltVision);
                    for (int i = 0; i < AlignResult.NumOfFound; i++)
                    {
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : X     - {AlignResult.stResult[i].dX} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Y     - {AlignResult.stResult[i].dY} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : W     - {AlignResult.stResult[i].dWidth} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : H     - {AlignResult.stResult[i].dHeight} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Score - {AlignResult.stResult[i].dScore} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Angle - {AlignResult.stResult[i].dAngle} ", UserEnum.EN_LOG_TYPE.ltVision);
                        if (AlignResult.stResult[i].dScore > dMaxScore)
                        {
                            dMaxScore = AlignResult.stResult[i].dScore;
                            nIdx = i;
                        }
                    }
                    
                    Result.dX = AlignResult.stResult[nIdx].dX;
                    Result.dY = AlignResult.stResult[nIdx].dY;
                    Result.dWidth = AlignResult.stResult[nIdx].dWidth;
                    Result.dHeight = AlignResult.stResult[nIdx].dHeight;
                    Result.dAngle = AlignResult.stResult[nIdx].dAngle;
                    Result.dScore = AlignResult.stResult[nIdx].dScore;
                }
                else
                {
                    fn_WriteLog(this.Title + $" : Find Result Count : {AlignResult.NumOfFound}", UserEnum.EN_LOG_TYPE.ltVision);
                    bOk = false;
                }

                // Theta 처리 할 것.
                if (bOk)
                {
                    //---------------------------------------------------------------------------
                    // Result 처리
                    // - 검색된 모델 기준 상대 좌표 처리.
                    // - 검색 완료 모델 Score 처리.

                    vs.dScore = Result.dScore;
                    vs.dTheta = Result.dAngle;

                    // Theta 처리 (반시계 +, 시계 -)
                    // Stage (반시계 - , 시계 +)
                    fn_WriteLog(this.Title + $"Pre Align Theta_Before : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);

                    // 0~360 => ±180
                    if (vs.dTheta > 180)
                        vs.dTheta -= 360;

                    // ±180 => ±90
                    if (vs.dTheta > 90)
                        vs.dTheta = vs.dTheta - 180;
                    if (vs.dTheta < -90)
                        vs.dTheta = vs.dTheta + 180;

                    // Theta Enable시 Theta Offset 적용.
                    if(CurrentRecipe.Model[nDetectedIdx].LoadingThetaEnable > 0)
                    {
                        vs.dTheta += CurrentRecipe.Model[nDetectedIdx].LoadingTheta;
                    }

                    fn_WriteLog(this.Title + $"Pre Align Theta_After : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);
                
                    vs.bResult = bOk;
                    vresult = vs;
                    fn_WriteLog(this.Title + " : Pre Align Complete.", UserEnum.EN_LOG_TYPE.ltVision);
                    if (_AlginMainCtrl != null)
                        _AlginMainCtrl.fn_SetResult(AlignResult, true);

                    delUpdateMainresult?.Invoke("");
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return bOk;
        }

        public bool fn_PreAlign(int index, ref ST_VISION_RESULT vresult, bool bSimul = false)
        {
            if (CurrentRecipe.Model[index].Enable == 0)
            {
                fn_WriteLog(this.Title + $"{CurrentRecipe.Model[index].strName} Disabled.", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }

            if (!File.Exists(CurrentRecipe.Model[index].strLoadingPath))
            {
                fn_WriteLog(this.Title + $"{CurrentRecipe.Model[index].strLoadingPath} Exists Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }
            bool bOk = false;
            double dNowAcceptance = 0.0;
            try
            {
                //Local Var.
                ST_VISION_RESULT vs = new ST_VISION_RESULT(0);
                ST_ALIGN_RESULT AlignResult = new ST_ALIGN_RESULT();
                AlignResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                {
                    AlignResult.stResult[i] = new ST_RESULT();
                }

                BitmapImage bmpImg = null;
                FileStream  source = null;
                WriteableBitmap wbImg = null;
                // Get Frame
                if (bSimul)
                {
                    bmpImg = new BitmapImage();
                    source = File.OpenRead(CurrentRecipe.Model[0].strImgPath);
                    bmpImg.BeginInit();
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.StreamSource = source;
                    bmpImg.EndInit();
                    wbImg = new WriteableBitmap(bmpImg);
                }
                else
                    wbImg = _CamManager.fn_GetFrame();

                try
                {
                    string LogPath = _strImageLogPath + DateTime.Now.ToString("yyyyMMdd") + @"\";
                    if (!Directory.Exists(LogPath))
                        Directory.CreateDirectory(LogPath);
                    string strName = "PreAlign_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".bmp";
                    fn_SaveImage(wbImg, LogPath + strName);
                    lbMain.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        lbMain.Content = strName;
                    }));
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }

                if (wbImg == null)
                {
                    fn_WriteLog(this.Title + "Cam Grab Error.", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                if (CurrentRecipe == null)
                {
                    fn_WriteLog(this.Title + "Can't Not Loaded Recipe. (CurrentRecipe is null.)", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                libSetImage(wbImg.BackBuffer, wbImg.PixelWidth, wbImg.PixelHeight, 1);
                
                bmpImg = new BitmapImage();
                source = File.OpenRead(CurrentRecipe.Model[index].strLoadingPath);
                bmpImg.BeginInit();
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = source;
                bmpImg.EndInit();

                WriteableBitmap wbMark = new WriteableBitmap(bmpImg);
                if (wbMark == null)
                {
                    fn_WriteLog(this.Title + "Mark Read Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                libSetMark(wbMark.BackBuffer, wbMark.PixelWidth, wbMark.PixelHeight, 1);

                IntPtr ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(CurrentRecipe.Model[index].LoadingParam));
                Marshal.StructureToPtr(CurrentRecipe.Model[index].LoadingParam, ptrParam, false);

                if (ptrParam != IntPtr.Zero)
                {
                    libSetParam(ptrParam, (IntPtr)null);
                    Marshal.FreeHGlobal(ptrParam);


                    libRunProcModel();
                    ST_ALIGN_RESULT stResult = new ST_ALIGN_RESULT();
                    stResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                    for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                    {
                        stResult.stResult[i] = new ST_RESULT();
                    }
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));

                    Marshal.StructureToPtr(stResult, ptr, true);
                    libGetResult(ptr);

                    AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                    Marshal.FreeHGlobal(ptr);

                    // 못찾으면 스코어 낮춰서 검색 시작.
                    if (AlignResult.NumOfFound == 0)
                    {
                        
                        for (int j = 0; j < UNDERSCORESEARCHTIME; j++) // Model Under Score
                        {
                            //Save Search Try Count;
                            dNowAcceptance = libRunProcModelNext(false);

                            ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));
                            Marshal.StructureToPtr(stResult, ptr, true);
                            libGetResult(ptr);

                            AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                            Marshal.FreeHGlobal(ptr);

                            // 결과를 찾았으면 중지.
                            if(AlignResult.NumOfFound > 0)
                            {
                                break;
                            }
                        }
                    }

                    libDestroyModel();

                    bOk = true;
                }
                else
                {
                    fn_WriteLog(this.Title + " : Invalid Pointer", UserEnum.EN_LOG_TYPE.ltVision);
                    bOk = false;
                }
                
                // 최대 스코어 결과 처리.
                ST_RESULT Result = new ST_RESULT();
                int nIdx = 0;
                double dMaxScore = 0.00;
                if (AlignResult.NumOfFound > 0)
                {
                    _nDetectedIdx = index;
                    fn_WriteLog(this.Title + $" : Pre-Align Model Detected Index : {index + 1}, Name : {CurrentRecipe.Model[index].strName}", UserEnum.EN_LOG_TYPE.ltVision);
                    fn_WriteLog(this.Title + $" : Pre-Align Model Detected Index : {index + 1}, Name : {CurrentRecipe.Model[index].strName}", UserEnum.EN_LOG_TYPE.ltLot);
                    
                    fn_WriteLog(this.Title + $" : Pre-Align Result From Lib : Total     - {AlignResult.NumOfFound} ", UserEnum.EN_LOG_TYPE.ltVision);
                    for (int i = 0; i < AlignResult.NumOfFound; i++)
                    {
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : X     - {AlignResult.stResult[i].dX} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Y     - {AlignResult.stResult[i].dY} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : W     - {AlignResult.stResult[i].dWidth} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : H     - {AlignResult.stResult[i].dHeight} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Score - {AlignResult.stResult[i].dScore} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Angle - {AlignResult.stResult[i].dAngle} ", UserEnum.EN_LOG_TYPE.ltVision);
                        if (AlignResult.stResult[i].dScore > dMaxScore)
                        {
                            dMaxScore = AlignResult.stResult[i].dScore;
                            nIdx = i;
                        }
                    }

                    Result.dX = AlignResult.stResult[nIdx].dX;
                    Result.dY = AlignResult.stResult[nIdx].dY;
                    Result.dWidth = AlignResult.stResult[nIdx].dWidth;
                    Result.dHeight = AlignResult.stResult[nIdx].dHeight;
                    Result.dAngle = AlignResult.stResult[nIdx].dAngle;
                    Result.dScore = AlignResult.stResult[nIdx].dScore;
                }
                else
                {
                    fn_WriteLog(this.Title + $" : Find Result Count : {AlignResult.NumOfFound}", UserEnum.EN_LOG_TYPE.ltVision);
                    bOk = false;
                }

                if (dNowAcceptance > 0) // dNowAcceptance가 0이 아니면 검색 실패한 것.
                {
                    bOk = false;
                    if (AlignResult.NumOfFound > 0)
                    {
                        delUpdateMainresult?.Invoke($"PreAlign - Model_No : #{index + 1} , Score : {Result.dScore:F2} (Set Score : {CurrentRecipe.Model[index].LoadingParam.Acceptance:F2})");
                        fn_WriteLog(this.Title + $"PreAlign - Model_No : #{index + 1} , Score : {Result.dScore:F2} (Set Score : {CurrentRecipe.Model[index].LoadingParam.Acceptance:F2})", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                    else
                    {
                        delUpdateMainresult?.Invoke($"PreAlign - Model_No : #{index + 1} , Score : {dNowAcceptance:F2} Under (Set Score : {CurrentRecipe.Model[index].LoadingParam.Acceptance:F2})");
                        fn_WriteLog(this.Title + $"PreAlign - Model_No : #{index + 1} , Score : {dNowAcceptance:F2} Under (Set Score : {CurrentRecipe.Model[index].LoadingParam.Acceptance:F2})", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                }


                // Theta 처리 할 것.
                if (bOk)
                {
                    //---------------------------------------------------------------------------
                    // Result 처리
                    // - 검색된 모델 기준 상대 좌표 처리.
                    // - 검색 완료 모델 Score 처리.

                    vs.dScore = Result.dScore;
                    vs.dTheta = Result.dAngle;

                    // Theta 처리 (반시계 +, 시계 -)
                    // Stage (반시계 - , 시계 +)
                    fn_WriteLog(this.Title + $"Pre Align Theta_Before : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);

                    // 0~360 => ±180
                    if (vs.dTheta > 180)
                        vs.dTheta -= 360;

                    // ±180 => ±90
                    if (vs.dTheta > 90)
                        vs.dTheta = vs.dTheta - 180;
                    if (vs.dTheta < -90)
                        vs.dTheta = vs.dTheta + 180;

                    // Theta Enable시 Theta Offset 적용.
                    if (CurrentRecipe.Model[index].LoadingThetaEnable > 0)
                    {
                        vs.dTheta += CurrentRecipe.Model[index].LoadingTheta;
                    }

                    fn_WriteLog(this.Title + $"Pre Align Theta_After : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);

                    vs.bResult = bOk;
                    vresult = vs;
                    fn_WriteLog(this.Title + " : Pre Align Complete.", UserEnum.EN_LOG_TYPE.ltVision);
                    delUpdateMainresult?.Invoke($"PreAlign - Model_No : #{index + 1} , Score : {Result.dScore:F2}, Angle : {vs.dTheta:F2}");

                    if (_AlginMainCtrl != null)
                        _AlginMainCtrl.fn_SetResult(AlignResult, true);

                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }

            return bOk;
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public bool fn_Polishingalign(ref ST_VISION_RESULT vresult, int nModelNum)
        @brief	폴리싱 Align
        @return	bool : algin 성공 여부
        @param	ref ST_VISION_RESULT vresult : Align 결과.
        @param	    int              nModel  : Align할 Model Number.
        @remark	
         - 내부 계산은 1사분면으로 계산함.
         - 결과값은 mm단위로 계산함.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/1  20:06
        */
        public bool fn_PolishingAlign(ref ST_VISION_RESULT vresult, bool bSimul = false, bool bSeqTest = false)
        {
            bool bOk = false;
            _nDetectedIdx = -1;
            try
            {
                //Local Var.
                ST_VISION_RESULT vs = new ST_VISION_RESULT(0);
                ST_ALIGN_RESULT AlignResult = new ST_ALIGN_RESULT();

                AlignResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                {
                    AlignResult.stResult[i] = new ST_RESULT();
                }

                WriteableBitmap wbImg;
                // Get Frame
                if (bSimul)
                {
                    BitmapImage bmpImg = new BitmapImage();
                    FileStream source = File.OpenRead(CurrentRecipe.Model[0].strImgPath);
                    bmpImg.BeginInit();
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.StreamSource = source;
                    bmpImg.EndInit();
                    wbImg = new WriteableBitmap(bmpImg);
                    //_CamManager.fn_SetSimulFrame();
                    //wbImg = _CamManager.fn_GetSimulFrame();
                }
                if(bSeqTest)
                    wbImg = _CamManager.fn_GetSimulFrame();
                else
                    wbImg = _CamManager.fn_GetFrame();

                // Save Frame
                try
                {
                    //if (_strImageLogPath == string.Empty)
                    string LogPath = _strImageLogPath + DateTime.Now.ToString("yyyyMMdd") + @"\";
                    if (!Directory.Exists(LogPath))
                        Directory.CreateDirectory(LogPath);
                    string strName = "Pol_Bath_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".bmp";
                    fn_SaveImage(wbImg, LogPath + strName);
                    lbMain.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        lbMain.Content = strName;
                    }));
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }

                if (wbImg == null)
                {
                    fn_WriteLog(this.Title + "Cam Grab Error.", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                if(CurrentRecipe == null)
                {
                    fn_WriteLog(this.Title + "Can't Not Loaded Recipe. ( CurrentRecipe is null.)", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                libSetImage(wbImg.BackBuffer, wbImg.PixelWidth, wbImg.PixelHeight, 1);

                // Set Mark
                for (int nModelNum = 0; nModelNum < CurrentRecipe.Model.Length; nModelNum++)
                {
                    if (CurrentRecipe.Model[nModelNum].Enable == 0)
                        continue;

                    if (!File.Exists(CurrentRecipe.Model[nModelNum].strImgPath))
                    {
                        fn_WriteLog(this.Title + $"{CurrentRecipe.Model[nModelNum].strImgPath} Exists Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                        //return false;
                        continue;
                    }
                    BitmapImage bmpImg = new BitmapImage();
                    FileStream source = File.OpenRead(CurrentRecipe.Model[nModelNum].strImgPath);
                    bmpImg.BeginInit();
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.StreamSource = source;
                    bmpImg.EndInit();
                    WriteableBitmap wbm = new WriteableBitmap(bmpImg);
                    if (wbm == null)
                    {
                        fn_WriteLog(this.Title + "Mark Read Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                        //return false;
                        continue;
                    }
                    CroppedBitmap cb = new CroppedBitmap(wbm,
                        new Int32Rect(
                            (int)CurrentRecipe.Model[nModelNum].Model.X, (int)CurrentRecipe.Model[nModelNum].Model.Y,
                            (int)CurrentRecipe.Model[nModelNum].Model.Width, (int)CurrentRecipe.Model[nModelNum].Model.Height)); //select region rect

                    WriteableBitmap wbMark = new WriteableBitmap(cb);
                    libSetMark(wbMark.BackBuffer, wbMark.PixelWidth, wbMark.PixelHeight, 1);

                    IntPtr ptrParam = IntPtr.Zero;
                    if (CurrentRecipe.Model[nModelNum].Algorithm == EN_ALGORITHM.algModel)
                    {
                        ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(CurrentRecipe.Model[nModelNum].ParamModel));
                        Marshal.StructureToPtr(CurrentRecipe.Model[nModelNum].ParamModel, ptrParam, false);
                        if (ptrParam != IntPtr.Zero)
                        {
                            libSetParam(ptrParam, (IntPtr)null);
                            Marshal.FreeHGlobal(ptrParam);

                            libRunProcModel();
                            ST_ALIGN_RESULT stResult = new ST_ALIGN_RESULT();
                            stResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                            for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                            {
                                stResult.stResult[i] = new ST_RESULT();
                            }
                            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));

                            Marshal.StructureToPtr(stResult, ptr, true);
                            libGetResult(ptr);

                            AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                            Marshal.FreeHGlobal(ptr);

                            libDestroyModel();


                            // Detected!
                            if (AlignResult.NumOfFound > 0)
                            {
                                bOk = true;
                                _nDetectedIdx = nModelNum;
                                fn_WriteLog(this.Title + $" : Pol-Align Model Detected Index : {_nDetectedIdx + 1}, Name : {CurrentRecipe.Model[_nDetectedIdx].strName}", UserEnum.EN_LOG_TYPE.ltVision);
                                fn_WriteLog(this.Title + $" : Pol-Align Model Detected Index : {_nDetectedIdx + 1}, Name : {CurrentRecipe.Model[_nDetectedIdx].strName}", UserEnum.EN_LOG_TYPE.ltLot   );
                                break;
                            }
                        }
                        else
                        {
                            fn_WriteLog(this.Title + " : Invalid Parameter", UserEnum.EN_LOG_TYPE.ltVision);
                            bOk = false;
                        }
                    }
                    else if (CurrentRecipe.Model[nModelNum].Algorithm == EN_ALGORITHM.algPattern)
                    {
                        ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(CurrentRecipe.Model[nModelNum].ParamPattern));
                        Marshal.StructureToPtr(CurrentRecipe.Model[nModelNum].ParamPattern, ptrParam, false);

                        if (ptrParam != IntPtr.Zero)
                        {
                            libSetParam(IntPtr.Zero, ptrParam);
                            Marshal.FreeHGlobal(ptrParam);

                            libRunProcPattern();
                            ST_ALIGN_RESULT stResult = new ST_ALIGN_RESULT();
                            stResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                            for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                            {
                                stResult.stResult[i] = new ST_RESULT();
                            }
                            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));

                            Marshal.StructureToPtr(stResult, ptr, true);
                            libGetResult(ptr);

                            AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                            Marshal.FreeHGlobal(ptr);

                            libDestroyPattern();


                            // Detected!
                            if (AlignResult.NumOfFound > 0)
                            {
                                bOk = true;
                                _nDetectedIdx = nModelNum;
                                fn_WriteLog(this.Title + $" : Pol-Align Pattern Detected Index : {_nDetectedIdx + 1}, Name : {CurrentRecipe.Model[_nDetectedIdx].strName}", UserEnum.EN_LOG_TYPE.ltVision);
                                break;
                            }
                        }
                        else
                        {
                            fn_WriteLog(this.Title + " : Invalid Pointer", UserEnum.EN_LOG_TYPE.ltVision);
                            bOk = false;
                        }
                    }
                }

                if (_nDetectedIdx == -1)
                {
                    fn_WriteLog(this.Title + $" : Pol-Align Model Invalid Index", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }
                // 최대 스코어 결과 처리.
                ST_RESULT Result = new ST_RESULT();
                int nIdx = 0;
                double dMaxScore = 0.00;
                if (AlignResult.NumOfFound > 0)
                {
                    fn_WriteLog(this.Title + $" : Pol-Align Result From Lib : Total     - {AlignResult.NumOfFound} ", UserEnum.EN_LOG_TYPE.ltVision);
                    for (int i = 0; i < AlignResult.NumOfFound; i++)
                    {
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : X     - {AlignResult.stResult[i].dX} "     , UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Y     - {AlignResult.stResult[i].dY} "     , UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : W     - {AlignResult.stResult[i].dWidth} " , UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : H     - {AlignResult.stResult[i].dHeight} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Score - {AlignResult.stResult[i].dScore} " , UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Angle - {AlignResult.stResult[i].dAngle} " , UserEnum.EN_LOG_TYPE.ltVision);

                        if (AlignResult.stResult[i].dScore > dMaxScore)
                        {
                            dMaxScore = AlignResult.stResult[i].dScore;
                            nIdx = i;
                        }
                    }
                    Result.dX      = AlignResult.stResult[nIdx].dX;
                    Result.dY      = AlignResult.stResult[nIdx].dY;
                    Result.dWidth  = AlignResult.stResult[nIdx].dWidth;
                    Result.dHeight = AlignResult.stResult[nIdx].dHeight;
                    Result.dAngle  = AlignResult.stResult[nIdx].dAngle;
                    Result.dScore  = AlignResult.stResult[nIdx].dScore;
                }
                else
                {
                    fn_WriteLog(this.Title + $" : Find Result Count : {AlignResult.NumOfFound}", UserEnum.EN_LOG_TYPE.ltVision);
                    bOk = false;
                }

                if (bOk)
                {
                    //---------------------------------------------------------------------------
                    // Result 처리
                    // - 검색된 모델 기준 상대 좌표 처리.
                    // - 검색 완료 모델 Score 처리.
                    Point pntTemp = new Point();
                    // 현재 레시피 모델 좌표 중심으로 변환.
                    pntTemp.X = CurrentRecipe.Model[_nDetectedIdx].Model.X + (CurrentRecipe.Model[_nDetectedIdx].Model.Width / 2.0);
                    pntTemp.Y = CurrentRecipe.Model[_nDetectedIdx].Model.Y + (CurrentRecipe.Model[_nDetectedIdx].Model.Height / 2.0);
                    fn_WriteLog(this.Title + $"Result : ({Result.dX}, {Result.dY}), Model : ({pntTemp.X}, {pntTemp.Y} )", UserEnum.EN_LOG_TYPE.ltVision);
                    // 현재 레시피 모델 중심 좌표 1사분면 좌표계로 변환.
                    Point pntRecipeModel = fn_GetPositionFromImageCenter(pntTemp, _RecipeVision.CamWidth, _RecipeVision.CamHeight);

                    // ModelFinder 결과는 찾은 모델의 중심을 반환하므로 별도 과정 없이 1사분면 좌표계로 변환.
                    Point pntSearchedModel = fn_GetPositionFromImageCenter(new Point(Result.dX, Result.dY), _RecipeVision.CamWidth, _RecipeVision.CamHeight);

                    // Recipe의 Model과 찾은 Model간의 거리차 계산.
                    Point pntModelSpan = new Point();
                    pntModelSpan.X = pntSearchedModel.X - pntRecipeModel.X;
                    pntModelSpan.Y = pntSearchedModel.Y - pntRecipeModel.Y;

                    fn_WriteLog(this.Title + $"Model : ({pntRecipeModel.X}, {pntRecipeModel.Y} ), Search : {pntSearchedModel.X}, {pntSearchedModel.Y}, Span : ({pntModelSpan.X}, {pntModelSpan.Y})", UserEnum.EN_LOG_TYPE.ltVision);

                    // Mark Search 상대 픽셀 값.
                    vs.pntModel = pntModelSpan;
                    vs.dScore = Result.dScore;
                    vs.dTheta = Result.dAngle;

                    // Theta 처리 (반시계 +, 시계 -)
                    // Stage (반시계 - , 시계 +)
                    fn_WriteLog(this.Title + $"Theta_Before : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);


                    // 0~360 => ±180
                    if(vs.dTheta > 180)
                        vs.dTheta -= 360;
                    //
                    // ±180 => ±90
                    if (vs.dTheta > 90)
                        vs.dTheta = vs.dTheta - 180;
                    if (vs.dTheta < -90)
                        vs.dTheta = vs.dTheta + 180;
                    
                    if (vs.dTheta > 45)
                        vs.dTheta = vs.dTheta - 90;
                    if (vs.dTheta < -45)
                        vs.dTheta = vs.dTheta + 90;

                    fn_WriteLog(this.Title + $"Theta_After : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);
                    fn_WriteLog(this.Title + $"Offset X : {g_VisionManager._RecipeVision.SpindleOffsetX}, Offset Y : {g_VisionManager._RecipeVision.SpindleOffsetY}, ResX : {g_VisionManager._RecipeVision.ResolutionX}, ResY : {g_VisionManager._RecipeVision.ResolutionY}", UserEnum.EN_LOG_TYPE.ltVision);

                    Point pntStart = new Point();
                    Point pntEnd = new Point();
                    Point pntMillingStart, pntMillingEnd;
                    int step_result = 0;
                    double dTiltInterpolation = 0.0;
                    for (int step = 0; step < CurrentRecipe.Model[_nDetectedIdx].MillingCount; step++)
                    {
                        dTiltInterpolation = fn_GetTiltInterpolation(CurrentRecipe.Model[_nDetectedIdx].Milling[step].Tilt);
                        fn_WriteLog(this.Title + $"Tilt Interpolation : {dTiltInterpolation.ToString("0.000")}", UserEnum.EN_LOG_TYPE.ltVision);
                        
                        pntStart.X = 0;
                        pntStart.Y = 0;
                        pntEnd.X = 0;
                        pntEnd.Y = 0;

                        vs.stRecipeList[step_result].nUseMilling   = CurrentRecipe.Model[_nDetectedIdx].Milling[step].Enable         ;
                        vs.stRecipeList[step_result].dTilt         = CurrentRecipe.Model[_nDetectedIdx].Milling[step].Tilt           ;
                        vs.stRecipeList[step_result].dRPM          = CurrentRecipe.Model[_nDetectedIdx].Milling[step].RPM            ;
                        vs.stRecipeList[step_result].dForce        = CurrentRecipe.Model[_nDetectedIdx].Milling[step].Force          ;
                        vs.stRecipeList[step_result].dSpeed        = CurrentRecipe.Model[_nDetectedIdx].Milling[step].Speed          ;
                        vs.stRecipeList[step_result].dPitch        = CurrentRecipe.Model[_nDetectedIdx].Milling[step].Pitch          ;
                        vs.stRecipeList[step_result].nPathCnt      = CurrentRecipe.Model[_nDetectedIdx].Milling[step].PathCount      ;
                        vs.stRecipeList[step_result].nCycle        = CurrentRecipe.Model[_nDetectedIdx].Milling[step].Cycle          ;
                        vs.stRecipeList[step_result].nUtilType     = CurrentRecipe.Model[_nDetectedIdx].Milling[step].UtilType       ;
                        vs.stRecipeList[step_result].nUseUtilFill  = CurrentRecipe.Model[_nDetectedIdx].Milling[step].UtilFill       ;
                        vs.stRecipeList[step_result].nUseUtilDrain = CurrentRecipe.Model[_nDetectedIdx].Milling[step].UtilDrain      ;
                        vs.stRecipeList[step_result].nUseImage     = CurrentRecipe.Model[_nDetectedIdx].Milling[step].MillingImage   ;
                        vs.stRecipeList[step_result].nUseEPD       = CurrentRecipe.Model[_nDetectedIdx].Milling[step].EPD            ;
                        vs.stRecipeList[step_result].nUseToolChg   = CurrentRecipe.Model[_nDetectedIdx].Milling[step].ToolChange     ;
                        vs.stRecipeList[step_result].nToolType     = CurrentRecipe.Model[_nDetectedIdx].Milling[step].ToolType       ;

                        vs.stRecipeList[step_result].dTiltOffset = dTiltInterpolation;
                        //vs.stRecipeList[step_result].nUtility = CurrentRecipe.Model[nModelNum].Milling[step].Util;
                        // Milling Rect의 좌상단 좌표 1사분면으로 변환. (Start End Position 계산 때문에 Rect 중심계산 생략)
                        pntMillingStart = fn_GetPositionFromImageCenter(
                            new Point(
                                CurrentRecipe.Model[_nDetectedIdx].Milling[step].MilRect.Left, 
                                CurrentRecipe.Model[_nDetectedIdx].Milling[step].MilRect.Top)
                            , _RecipeVision.CamWidth, _RecipeVision.CamHeight);

                        pntMillingEnd = fn_GetPositionFromImageCenter(
                            new Point(
                                CurrentRecipe.Model[_nDetectedIdx].Milling[step].MilRect.Right,
                                CurrentRecipe.Model[_nDetectedIdx].Milling[step].MilRect.Bottom)
                            , _RecipeVision.CamWidth, _RecipeVision.CamHeight);
                        fn_WriteLog(this.Title + $"Step : {step}, MilPx X: {CurrentRecipe.Model[_nDetectedIdx].Milling[step].MilRect.X.ToString("0.000")}, MilPx Y: {CurrentRecipe.Model[_nDetectedIdx].Milling[step].MilRect.Y.ToString("0.000")}, pntMilling : S({pntMillingStart.X.ToString("0.000")}, {pntMillingStart.Y.ToString("0.000")}), E({pntMillingEnd.X.ToString("0.000")}, {pntMillingEnd.Y.ToString("0.000")})", UserEnum.EN_LOG_TYPE.ltVision);
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
                        switch (CurrentRecipe.Model[_nDetectedIdx].Milling[step].StartPos)
                        {
                            // Left-Bottom
                            case 0:
                                vs.stRecipeList[step_result].pStartPos.X = pntStart.X;
                                vs.stRecipeList[step_result].pStartPos.Y = pntEnd  .Y;
                                vs.stRecipeList[step_result].pEndPos  .X = pntEnd  .X;
                                vs.stRecipeList[step_result].pEndPos  .Y = pntStart.Y;
                                break;
                            // Right-Bottom
                            case 1:
                                vs.stRecipeList[step_result].pStartPos.X = pntEnd  .X;
                                vs.stRecipeList[step_result].pStartPos.Y = pntEnd  .Y;
                                vs.stRecipeList[step_result].pEndPos  .X = pntStart.X;
                                vs.stRecipeList[step_result].pEndPos  .Y = pntStart.Y;
                                break;
                        }
                        fn_WriteLog(this.Title + $"End Direction : ({vs.stRecipeList[step_result].pStartPos.X}, {vs.stRecipeList[step_result].pStartPos.Y}), pntEnd : ({vs.stRecipeList[step_result].pEndPos.X}, {vs.stRecipeList[step_result].pEndPos.Y})", UserEnum.EN_LOG_TYPE.ltVision);
                        vs.stRecipeList[step_result].dTheta = Result.dAngle;
                        step_result++;
                    }
                    vs.nTotalStep = CurrentRecipe.Model[_nDetectedIdx].MillingCount;
                    vs.bResult = bOk;
                    vresult = vs;
                    fn_WriteLog(this.Title + " : Polishing Align Complete.", UserEnum.EN_LOG_TYPE.ltVision);

                    
                    if(_AlginMainCtrl != null)
                        _AlginMainCtrl.fn_SetResult(AlignResult, true);
                    if(_AlginPreCtrl != null)
                        _AlginPreCtrl.fn_SetResult(AlignResult, true);

                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return bOk;
        }

        
        public bool fn_PolishingAlign(int index, ref ST_VISION_RESULT vresult, EN_ALIGNSTEP enAlignStep, bool bSimul = false, bool bSeqTest = false)
        {
            // Seq에서 체크해야함.
            if (CurrentRecipe.Model[index].Enable == 0)
            {
                fn_WriteLog(this.Title + $"{CurrentRecipe.Model[index].strName} Disabled.", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }

            if (!File.Exists(CurrentRecipe.Model[index].strImgPath))
            {
                fn_WriteLog(this.Title + $"{CurrentRecipe.Model[index].strImgPath} Exists Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }

            bool bOk = false;
            double dNowAcceptance = 0;
            double dParamAcceptance = 0.0;
            try
            {
                //Local Var.
                ST_VISION_RESULT vs = new ST_VISION_RESULT(0);
                ST_ALIGN_RESULT AlignResult = new ST_ALIGN_RESULT();

                AlignResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                {
                    AlignResult.stResult[i] = new ST_RESULT();
                }
                BitmapImage bmpImg;
                FileStream source;
                WriteableBitmap wbImg;

                

                // Get Frame
                if (bSimul)
                {
                    bmpImg = new BitmapImage();
                    source = File.OpenRead(CurrentRecipe.Model[0].strImgPath);
                    bmpImg.BeginInit();
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.StreamSource = source;
                    bmpImg.EndInit();
                    wbImg = new WriteableBitmap(bmpImg);
                    //_CamManager.fn_SetSimulFrame();
                    //wbImg = _CamManager.fn_GetSimulFrame();
                }
                if (bSeqTest)
                    wbImg = _CamManager.fn_GetSimulFrame();
                else
                    wbImg = _CamManager.fn_GetFrame();

                // Save Frame
                try
                {
                    //if (_strImageLogPath == string.Empty)
                    string LogPath = _strImageLogPath + DateTime.Now.ToString("yyyyMMdd") + @"\";
                    if (!Directory.Exists(LogPath))
                        Directory.CreateDirectory(LogPath);
                    string strName = "Pol_Bath_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".bmp";
                    fn_SaveImage(wbImg, LogPath + strName);
                    lbMain.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        lbMain.Content = strName;
                    }));
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }

                if (wbImg == null)
                {
                    fn_WriteLog(this.Title + "Cam Grab Error.", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                if (CurrentRecipe == null)
                {
                    fn_WriteLog(this.Title + "Can't Not Loaded Recipe. ( CurrentRecipe is null.)", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }

                libSetImage(wbImg.BackBuffer, wbImg.PixelWidth, wbImg.PixelHeight, 1);

                // Set Mark

                
                bmpImg = new BitmapImage();
                source = File.OpenRead(CurrentRecipe.Model[index].strImgPath);
                bmpImg.BeginInit();
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = source;
                bmpImg.EndInit();
                WriteableBitmap wbm = new WriteableBitmap(bmpImg);
                if (wbm == null)
                {
                    fn_WriteLog(this.Title + "Mark Read Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                    return false;
                }
                CroppedBitmap cb = new CroppedBitmap(wbm,
                    new Int32Rect(
                        (int)CurrentRecipe.Model[index].Model.X, (int)CurrentRecipe.Model[index].Model.Y,
                        (int)CurrentRecipe.Model[index].Model.Width, (int)CurrentRecipe.Model[index].Model.Height)); //select region rect

                WriteableBitmap wbMark = new WriteableBitmap(cb);
                libSetMark(wbMark.BackBuffer, wbMark.PixelWidth, wbMark.PixelHeight, 1);

                IntPtr ptrParam = IntPtr.Zero;
                if (CurrentRecipe.Model[index].Algorithm == EN_ALGORITHM.algModel)
                {
                    dParamAcceptance = CurrentRecipe.Model[index].ParamModel.Acceptance;
                    ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(CurrentRecipe.Model[index].ParamModel));
                    Marshal.StructureToPtr(CurrentRecipe.Model[index].ParamModel, ptrParam, false);
                    if (ptrParam != IntPtr.Zero)
                    {
                        libSetParam(ptrParam, (IntPtr)null);
                        Marshal.FreeHGlobal(ptrParam);

                        libRunProcModel();
                        ST_ALIGN_RESULT stResult = new ST_ALIGN_RESULT();
                        stResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                        for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                        {
                            stResult.stResult[i] = new ST_RESULT();
                        }
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));

                        Marshal.StructureToPtr(stResult, ptr, true);
                        libGetResult(ptr);

                        AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                        Marshal.FreeHGlobal(ptr);

                        // 못찾으면 스코어 낮춰서 검색 시작.
                        // UNDERSCORESEARCHTIME : 3
                        if (AlignResult.NumOfFound == 0)
                        {
                            for (int j = 0; j < UNDERSCORESEARCHTIME; j++) // Model Under Score
                            {
                                //Save Search Try Score;
                                dNowAcceptance = libRunProcModelNext(false);

                                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));
                                Marshal.StructureToPtr(stResult, ptr, true);

                                libGetResult(ptr);

                                AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                                Marshal.FreeHGlobal(ptr);

                                // 결과를 찾았으면 중지.
                                if (AlignResult.NumOfFound > 0)
                                    break;
                            }
                        }

                        libDestroyModel();


                        // Detected!
                        if (AlignResult.NumOfFound > 0)
                        {
                            bOk = true;
                            fn_WriteLog(this.Title + $" : Pol-Align Model Detected Index : {index + 1}, Name : {CurrentRecipe.Model[index].strName}", UserEnum.EN_LOG_TYPE.ltVision);
                            fn_WriteLog(this.Title + $" : Pol-Align Model Detected Index : {index + 1}, Name : {CurrentRecipe.Model[index].strName}", UserEnum.EN_LOG_TYPE.ltLot);
                        }
                    }
                    else
                    {
                        fn_WriteLog(this.Title + " : Invalid Parameter", UserEnum.EN_LOG_TYPE.ltVision);
                        bOk = false;
                    }
                }
                else if (CurrentRecipe.Model[index].Algorithm == EN_ALGORITHM.algPattern)
                {
                    dParamAcceptance = CurrentRecipe.Model[index].ParamPattern.Acceptance;
                    ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(CurrentRecipe.Model[index].ParamPattern));
                    Marshal.StructureToPtr(CurrentRecipe.Model[index].ParamPattern, ptrParam, false);

                    if (ptrParam != IntPtr.Zero)
                    {
                        libSetParam(IntPtr.Zero, ptrParam);
                        Marshal.FreeHGlobal(ptrParam);

                        libRunProcPattern();
                        ST_ALIGN_RESULT stResult = new ST_ALIGN_RESULT();
                        stResult.stResult = new ST_RESULT[MODELS_MAX_OCCURRENCES];
                        for (int i = 0; i < MODELS_MAX_OCCURRENCES; i++)
                        {
                            stResult.stResult[i] = new ST_RESULT();
                        }
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));

                        Marshal.StructureToPtr(stResult, ptr, true);
                        libGetResult(ptr);

                        AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                        Marshal.FreeHGlobal(ptr);

                        // 못찾으면 스코어 낮춰서 검색 시작.
                        if (AlignResult.NumOfFound == 0)
                        {

                            for (int j = 0; j < UNDERSCORESEARCHTIME; j++) // Model Under Score
                            {

                                // Save Search Try Score;
                                dNowAcceptance = libRunProcPatternNext(false);
                                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(stResult));
                                Marshal.StructureToPtr(stResult, ptr, true);
                                libGetResult(ptr);

                                AlignResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(ptr, typeof(ST_ALIGN_RESULT));
                                Marshal.FreeHGlobal(ptr);

                                // 결과를 찾았으면 중지.
                                if (AlignResult.NumOfFound > 0)
                                    break;
                            }
                        }

                        libDestroyPattern();

                        // Detected!
                        if (AlignResult.NumOfFound > 0)
                        {
                            bOk = true;
                            fn_WriteLog(this.Title + $" : Pol-Align Pattern Detected Index : {index + 1}, Name : {CurrentRecipe.Model[index].strName}", UserEnum.EN_LOG_TYPE.ltVision);
                        }
                    }
                    else
                    {
                        fn_WriteLog(this.Title + " : Invalid Pointer", UserEnum.EN_LOG_TYPE.ltVision);
                        bOk = false;
                    }
                }
                
                // 최대 스코어 결과 처리.
                ST_RESULT Result = new ST_RESULT();
                int nIdx = 0;
                double dMaxScore = 0.00;
                if (AlignResult.NumOfFound > 0)
                {
                    fn_WriteLog(this.Title + $" : Pol-Align Result From Lib : Total     - {AlignResult.NumOfFound} ", UserEnum.EN_LOG_TYPE.ltVision);
                    for (int i = 0; i < AlignResult.NumOfFound; i++)
                    {
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : X     - {AlignResult.stResult[i].dX} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Y     - {AlignResult.stResult[i].dY} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : W     - {AlignResult.stResult[i].dWidth} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : H     - {AlignResult.stResult[i].dHeight} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Score - {AlignResult.stResult[i].dScore} ", UserEnum.EN_LOG_TYPE.ltVision);
                        fn_WriteLog(this.Title + $" :   {i + 1} From Lib : Angle - {AlignResult.stResult[i].dAngle} ", UserEnum.EN_LOG_TYPE.ltVision);

                        if (AlignResult.stResult[i].dScore > dMaxScore)
                        {
                            dMaxScore = AlignResult.stResult[i].dScore;
                            nIdx = i;
                        }
                    }
                    Result.dX = AlignResult.stResult[nIdx].dX;
                    Result.dY = AlignResult.stResult[nIdx].dY;
                    Result.dWidth = AlignResult.stResult[nIdx].dWidth;
                    Result.dHeight = AlignResult.stResult[nIdx].dHeight;
                    Result.dAngle = AlignResult.stResult[nIdx].dAngle;
                    Result.dScore = AlignResult.stResult[nIdx].dScore;

                }
                else
                {
                    fn_WriteLog(this.Title + $" : Find Result Count : {AlignResult.NumOfFound}", UserEnum.EN_LOG_TYPE.ltVision);
                    bOk = false;
                }

                if (dNowAcceptance > 0) // dNowAcceptance가 0이 아니면 검색 실패한 것.
                {
                    bOk = false;
                    // 재검색시에도 실패시 numoffound가 0, 그러므로 표기도 less Score로 표기 할 것.
                    if (AlignResult.NumOfFound > 0)
                    {
                        delUpdateMainresult?.Invoke($"PolAlign - Model_No : #{index + 1} , Score : {Result.dScore:F2} (Set Score : {dParamAcceptance:F2})");
                        fn_WriteLog(this.Title + $" : PolAlign - Model_No : #{index + 1} , Score : {Result.dScore:F2} (Set Score : {dParamAcceptance:F2})", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                    else
                    {
                        delUpdateMainresult?.Invoke($"PolAlign - Model_No : #{index + 1} , Score : less {dNowAcceptance:F2} (Set Score : {dParamAcceptance:F2})");
                        fn_WriteLog(this.Title + $" : PolAlign - Model_No : #{index + 1} , Score : less {dNowAcceptance:F2} Under (Set Score : {dParamAcceptance:F2})", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                }


                if (bOk)
                {
                    //---------------------------------------------------------------------------
                    // Result 처리
                    // - 검색된 모델 기준 상대 좌표 처리.
                    // - 검색 완료 모델 Score 처리.
                    Point pntTemp = new Point();
                    // 현재 레시피 모델 좌표 중심으로 변환.
                    pntTemp.X = CurrentRecipe.Model[index].Model.X + (CurrentRecipe.Model[index].Model.Width / 2.0);
                    pntTemp.Y = CurrentRecipe.Model[index].Model.Y + (CurrentRecipe.Model[index].Model.Height / 2.0);
                    fn_WriteLog(this.Title + $"Result : ({Result.dX}, {Result.dY}), Model : ({pntTemp.X}, {pntTemp.Y} )", UserEnum.EN_LOG_TYPE.ltVision);
                    // 현재 레시피 모델 중심 좌표 1사분면 좌표계로 변환.
                    Point pntRecipeModel = fn_GetPositionFromImageCenter(pntTemp, _RecipeVision.CamWidth, _RecipeVision.CamHeight);

                    // ModelFinder 결과는 찾은 모델의 중심을 반환하므로 별도 과정 없이 1사분면 좌표계로 변환.
                    Point pntSearchedModel = fn_GetPositionFromImageCenter(new Point(Result.dX, Result.dY), _RecipeVision.CamWidth, _RecipeVision.CamHeight);

                    // Recipe의 Model과 찾은 Model간의 거리차 계산.
                    Point pntModelSpan = new Point();
                    pntModelSpan.X = pntSearchedModel.X - pntRecipeModel.X;
                    pntModelSpan.Y = pntSearchedModel.Y - pntRecipeModel.Y;

                    fn_WriteLog(this.Title + $"Model : ({pntRecipeModel.X}, {pntRecipeModel.Y} ), Search : {pntSearchedModel.X}, {pntSearchedModel.Y}, Span : ({pntModelSpan.X}, {pntModelSpan.Y})", UserEnum.EN_LOG_TYPE.ltVision);

                    // Mark Search 상대 픽셀 값.
                    vs.pntModel = pntModelSpan;
                    vs.dScore = Result.dScore;
                    vs.dTheta = Result.dAngle;

                    // Theta 처리 (반시계 +, 시계 -)
                    // Stage (반시계 - , 시계 +)
                    fn_WriteLog(this.Title + $"Theta_Before : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);


                    // 0~360 => ±180
                    if (vs.dTheta > 180)
                        vs.dTheta -= 360;
                    //
                    // ±180 => ±90
                    if (vs.dTheta > 90)
                        vs.dTheta = vs.dTheta - 180;
                    if (vs.dTheta < -90)
                        vs.dTheta = vs.dTheta + 180;

                    if (vs.dTheta > 45)
                        vs.dTheta = vs.dTheta - 90;
                    if (vs.dTheta < -45)
                        vs.dTheta = vs.dTheta + 90;

                    fn_WriteLog(this.Title + $"Theta_After : {vs.dTheta}", UserEnum.EN_LOG_TYPE.ltVision);
                    fn_WriteLog(this.Title + $"Offset X : {g_VisionManager._RecipeVision.SpindleOffsetX}, Offset Y : {g_VisionManager._RecipeVision.SpindleOffsetY}, ResX : {g_VisionManager._RecipeVision.ResolutionX}, ResY : {g_VisionManager._RecipeVision.ResolutionY}", UserEnum.EN_LOG_TYPE.ltVision);

                    Point pntStart = new Point();
                    Point pntEnd = new Point();
                    Point pntMillingStart, pntMillingEnd;
                    int step_result = 0;
                    double dTiltInterpolation = 0.0;
                    for (int step = 0; step < CurrentRecipe.Model[index].MillingCount; step++)
                    {
                        dTiltInterpolation = fn_GetTiltInterpolation(CurrentRecipe.Model[index].Milling[step].Tilt);
                        fn_WriteLog(this.Title + $"Tilt Interpolation : {dTiltInterpolation.ToString("0.000")}", UserEnum.EN_LOG_TYPE.ltVision);

                        pntStart.X = 0;
                        pntStart.Y = 0;
                        pntEnd.X = 0;
                        pntEnd.Y = 0;

                        vs.stRecipeList[step_result].nUseMilling   = CurrentRecipe.Model[index].Milling[step].Enable;
                        vs.stRecipeList[step_result].dTilt         = CurrentRecipe.Model[index].Milling[step].Tilt;
                        vs.stRecipeList[step_result].dRPM          = CurrentRecipe.Model[index].Milling[step].RPM;
                        vs.stRecipeList[step_result].dForce        = CurrentRecipe.Model[index].Milling[step].Force;
                        vs.stRecipeList[step_result].dSpeed        = CurrentRecipe.Model[index].Milling[step].Speed;
                        vs.stRecipeList[step_result].dPitch        = CurrentRecipe.Model[index].Milling[step].Pitch;
                        vs.stRecipeList[step_result].nPathCnt      = CurrentRecipe.Model[index].Milling[step].PathCount;
                        vs.stRecipeList[step_result].nCycle        = CurrentRecipe.Model[index].Milling[step].Cycle;
                        vs.stRecipeList[step_result].nUtilType     = CurrentRecipe.Model[index].Milling[step].UtilType;
                        vs.stRecipeList[step_result].nUseUtilFill  = CurrentRecipe.Model[index].Milling[step].UtilFill;
                        vs.stRecipeList[step_result].nUseUtilDrain = CurrentRecipe.Model[index].Milling[step].UtilDrain;
                        vs.stRecipeList[step_result].nUseImage     = CurrentRecipe.Model[index].Milling[step].MillingImage;
                        vs.stRecipeList[step_result].nUseEPD       = CurrentRecipe.Model[index].Milling[step].EPD;
                        vs.stRecipeList[step_result].nUseToolChg   = CurrentRecipe.Model[index].Milling[step].ToolChange;
                        vs.stRecipeList[step_result].nToolType     = CurrentRecipe.Model[index].Milling[step].ToolType;

                        vs.stRecipeList[step_result].dTiltOffset = dTiltInterpolation;
                        //vs.stRecipeList[step_result].nUtility = CurrentRecipe.Model[nModelNum].Milling[step].Util;
                        // Milling Rect의 좌상단 좌표 1사분면으로 변환. (Start End Position 계산 때문에 Rect 중심계산 생략)
                        pntMillingStart = fn_GetPositionFromImageCenter(
                            new Point(
                                CurrentRecipe.Model[index].Milling[step].MilRect.Left,
                                CurrentRecipe.Model[index].Milling[step].MilRect.Top)
                            , _RecipeVision.CamWidth, _RecipeVision.CamHeight);

                        pntMillingEnd = fn_GetPositionFromImageCenter(
                            new Point(
                                CurrentRecipe.Model[index].Milling[step].MilRect.Right,
                                CurrentRecipe.Model[index].Milling[step].MilRect.Bottom)
                            , _RecipeVision.CamWidth, _RecipeVision.CamHeight);
                        fn_WriteLog(this.Title + $"Step : {step}, MilPx X: {CurrentRecipe.Model[index].Milling[step].MilRect.X.ToString("0.000")}, MilPx Y: {CurrentRecipe.Model[index].Milling[step].MilRect.Y.ToString("0.000")}, pntMilling : S({pntMillingStart.X.ToString("0.000")}, {pntMillingStart.Y.ToString("0.000")}), E({pntMillingEnd.X.ToString("0.000")}, {pntMillingEnd.Y.ToString("0.000")})", UserEnum.EN_LOG_TYPE.ltVision);
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
                        switch (CurrentRecipe.Model[index].Milling[step].StartPos)
                        {
                            // Left-Bottom
                            case 0:
                                vs.stRecipeList[step_result].pStartPos.X = pntStart.X;
                                vs.stRecipeList[step_result].pStartPos.Y = pntEnd.Y;
                                vs.stRecipeList[step_result].pEndPos.X = pntEnd.X;
                                vs.stRecipeList[step_result].pEndPos.Y = pntStart.Y;
                                break;
                            // Right-Bottom
                            case 1:
                                vs.stRecipeList[step_result].pStartPos.X = pntEnd.X;
                                vs.stRecipeList[step_result].pStartPos.Y = pntEnd.Y;
                                vs.stRecipeList[step_result].pEndPos.X = pntStart.X;
                                vs.stRecipeList[step_result].pEndPos.Y = pntStart.Y;
                                break;
                        }
                        fn_WriteLog(this.Title + $"End Direction : ({vs.stRecipeList[step_result].pStartPos.X}, {vs.stRecipeList[step_result].pStartPos.Y}), pntEnd : ({vs.stRecipeList[step_result].pEndPos.X}, {vs.stRecipeList[step_result].pEndPos.Y})", UserEnum.EN_LOG_TYPE.ltVision);
                        vs.stRecipeList[step_result].dTheta = Result.dAngle;
                        step_result++;
                    }
                    vs.nTotalStep = CurrentRecipe.Model[index].MillingCount;
                    vs.bResult = bOk;
                    vresult = vs;
                    fn_WriteLog(this.Title + " : Polishing Align Complete.", UserEnum.EN_LOG_TYPE.ltVision);


                    if (_AlginMainCtrl != null)
                        _AlginMainCtrl.fn_SetResult(AlignResult, true);
                    if (_AlginPreCtrl != null)
                        _AlginPreCtrl.fn_SetResult(AlignResult, true);

                    // Main Vision Result Update
                    // Angle, Shift X,Y Update
                    switch (enAlignStep)
                    {
                        case UserEnumVision.EN_ALIGNSTEP.ThetaAlign:
                            dAlignedTheta = vs.dTheta;
                            break;
                        case UserEnumVision.EN_ALIGNSTEP.XYAlign:
                            delUpdateMainresult?.Invoke($"PolAlign - Model_No : #{index + 1} , Score : {Result.dScore:F3}, Shift XY : {pntModelSpan.X * g_VisionManager._RecipeVision.ResolutionX / 1000.0:F3}, {pntModelSpan.Y * _RecipeVision.ResolutionY / 1000.0:F3}, Angle : {dAlignedTheta:F2}");
                            fn_WriteLog(this.Title + $" : PolAlign - Model_No : #{index + 1} , Score : {Result.dScore:F3}, Shift XY : {pntModelSpan.X * _RecipeVision.ResolutionX / 1000.0:F3}, {pntModelSpan.Y * _RecipeVision.ResolutionY / 1000.0:F3}, Angle : {dAlignedTheta:F2}", UserEnum.EN_LOG_TYPE.ltVision);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return bOk;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     public bool fn_EPDResult(bool bOnlyMeasure = false)
        @brief	시퀀스상 EPD 호출 함수.
        @return	bool : EPD 성공 여부.
        @param	bool bOnlyMeasure : EPD를 단순 측정만 할지, 공정에 영향을 줄지 옵션.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:25
        */
        public bool fn_EPDResult(Point SearchResultOffset, bool bOnlyMeasure = false)
        {
            bool bRtn = true;
            const bool bNewFrame = false;
            WriteableBitmap wbImg = null;
            // Get Frame
            // Save Frame이랑 다르다? -> 파라메터에서 grab을 새로할 지 결정.
            wbImg = _CamManager.fn_GetFrame(bNewFrame);

            if (wbImg != null)
            {
                ST_ALIGN_RESULT result = new ST_ALIGN_RESULT();
                result.stResult = new ST_RESULT[5];

                if (_nDetectedIdx >= 0)
                {
                    // EPD Result
                    ST_INSPECTION stInspection = CurrentRecipe.Model[_nDetectedIdx].Inspection;
                    stInspection.ROIX += SearchResultOffset.X;
                    stInspection.ROIY += SearchResultOffset.Y;

                    bRtn = g_VisionManager._ImgProc.EPDResult(wbImg, stInspection, ref result, true);
                }
                else
                {
                    fn_WriteLog(this.Title + $"_EPD : Invaild Index {_nDetectedIdx}.", UserEnum.EN_LOG_TYPE.ltVision);
                    bRtn = false;
                }
                if(bOnlyMeasure) // EPD결과가 공정에 영향을 미치지 않는 옵션.
                    bRtn = false;
            }
            else
            {
                bRtn = false;
                fn_WriteLog(this.Title + $"_EPD : Grab Fail.", UserEnum.EN_LOG_TYPE.ltVision);
            }
            return bRtn;
        }

        public bool fn_EPDResult(int index, Point SearchResultOffset, bool bOnlyMeasure = false)
        {
            bool bRtn = true;
            const bool bNewFrame = false;
            WriteableBitmap wbImg = null;
            // Get Frame
            // Save Frame이랑 다르다? -> 파라메터에서 grab을 새로할 지 결정.
            wbImg = _CamManager.fn_GetFrame(bNewFrame);

            if (wbImg != null)
            {
                ST_ALIGN_RESULT result = new ST_ALIGN_RESULT();
                result.stResult = new ST_RESULT[5];

                if (index >= 0)
                {
                    // EPD Result
                    ST_INSPECTION stInspection = CurrentRecipe.Model[index].Inspection;
                    stInspection.ROIX += SearchResultOffset.X;
                    stInspection.ROIY += SearchResultOffset.Y;

                    bRtn = g_VisionManager._ImgProc.EPDResult(wbImg, stInspection, ref result, true);
                }
                else
                {
                    fn_WriteLog(this.Title + $"_EPD : Invaild Index {_nDetectedIdx}.", UserEnum.EN_LOG_TYPE.ltVision);
                    bRtn = false;
                }
                if (bOnlyMeasure) // EPD결과가 공정에 영향을 미치지 않는 옵션.
                    bRtn = false;
            }
            else
            {
                bRtn = false;
                fn_WriteLog(this.Title + $"_EPD : Grab Fail.", UserEnum.EN_LOG_TYPE.ltVision);
            }
            return bRtn;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public bool fn_SetLightValue(EN_LIGHTON_MODE nMode = 0)
        @brief  조명 컨트롤에 Mode에 따른 조명 값 셋.
        @return	성공 여부.
        @param	EN_LIGHTON_MODE nMode : 조명 모드.
            Polishing = 0,
            Loading,
            ToolStorage,
            Insepction,
            Reserve1,
            Reserve2,
            Reserve3,
            Reserve4
        @remark	
         - EN_LIGHTON_MODE에 따라 _RecipeModel에 있는 Light값을 가져다 씀
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/27  10:36
        */
        public bool fn_SetLightValue(int on , int lightWhite, int lightIR)
        {
            bool bRet = true;

            if (on == 1)
            {
                _LightManger.Channel = 1;
                _LightManger.Bright  = lightWhite;
                _LastLightWValue     = lightWhite;

                fn_WriteLog(this.Title + " : Set White Light_OK. " + lightWhite, UserEnum.EN_LOG_TYPE.ltVision);

                _LightManger.Channel = 2;
                _LightManger.Bright  = lightIR;
                _LastLightIRValue    = lightIR;
                fn_WriteLog(this.Title + " : Set IR Light_OK. " + lightIR, UserEnum.EN_LOG_TYPE.ltVision);
            }
            else
            {
                _LightManger.SetOff(0);
                _LightManger.SetOff(1);
                Console.WriteLine($"Light Off");
            }

            return bRet;
        }

        public bool fn_SetLightValue(int on, EN_VISION_MODE nMode = 0)
        {
            bool bRet = true;

            if (on == 1)
            {
                int nLw = CurrentRecipe.LightWhite[(int)nMode];

                _LightManger.Channel = 1;
                _LightManger.Bright = nLw;
                _LastLightWValue = nLw;
                Console.WriteLine($"White {_LightManger.Bright}");

                nLw = CurrentRecipe.LightIR[(int)nMode];
                _LightManger.Channel = 2;
                _LightManger.Bright = nLw;
                _LastLightIRValue = nLw;
                Console.WriteLine($"IR {_LightManger.Bright}");
            }
            else
            {
                _LightManger.SetOff(0);
                _LightManger.SetOff(1);
                Console.WriteLine($"Light Off");
            }

            return bRet;
        }

        public bool fn_SetLight_Tool(int on)
        {
            bool bRet = true;

            if (on == 1)
            {
                int nLw = _RecipeVision.ToolLightWhite;

                _LightManger.Channel = 1;
                _LightManger.Bright = nLw;
                _LastLightWValue = nLw;
                Console.WriteLine($"White {_LightManger.Bright}");

                nLw = _RecipeVision.ToolLightIR;
                _LightManger.Channel = 2;
                _LightManger.Bright = nLw;
                _LastLightIRValue = nLw;
                Console.WriteLine($"IR {_LightManger.Bright}");
            }
            else
            {
                _LightManger.SetOff(0);
                _LightManger.SetOff(1);
                Console.WriteLine($"Light Off");
            }

            return bRet;
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public Point fn_TiltInterpolation(Point pnt, double tilt)
        @brief	Tilt 각도 보상
        @return	각도 보상 포인트
        @param	Point   pnt  : 보상할 포인트.
        @param	double  tilt : Degree.
        @remark	
         - d
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/6  15:17
        */
        public double fn_GetTiltInterpolation(double tilt)
        {
            return Math.Sin(tilt / 180.0 * Math.PI) * _RecipeVision.TiltRotateCenterFromSample;
        }

        public void fn_SaveImage(WriteableBitmap wb, string strpath)
        {
            if (wb != null)
            {
                try
                {
                    using (FileStream stream = new FileStream(strpath, FileMode.Create))
                    {
                        BmpBitmapEncoder encoder = new BmpBitmapEncoder();

                        encoder.Frames.Add(BitmapFrame.Create(wb));
                        encoder.Save(stream);
                        fn_WriteLog(this.Title + $"Save Image {strpath}");
                    }
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
            else
            {
                Console.WriteLine("SaveImage_WriteableBitmap is null.");
            }
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
            Vision Grab & Image Save Func //LEE/200513 Vision Grab & Save
        </summary>
        <param name="strFileName">Image File Name (Recipe Name) </param>
        @author    이준호(LEEJOONHO)
        @date      2020/05/13 11:41
        */
        public bool fn_GrabImageSave(string strFileName = "LOTNo")
        {
            try
            {
                try
                {
                    if (_AlginMainCtrl != null)
                        _AlginMainCtrl.fn_ClearResult();
                }
                catch{}
                WriteableBitmap wbImg = _CamManager.fn_GetFrame();

                if (wbImg != null)
                {
                    //if (_strImageLogPath == "")
                    string LogPath = _strImageLogPath + DateTime.Now.ToString("yyyyMMdd") + @"\";
                    if (!Directory.Exists(LogPath))
                        Directory.CreateDirectory(LogPath);
                    string strName = $"{strFileName}_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".bmp";
                    fn_SaveImage(wbImg, LogPath + strName);
                    lbMain.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        lbMain.Content = strName;
                    }));
                    
                    Console.WriteLine("fn_GrabImageSave- OK");
                    return true;
                }
                else
                {
                    fn_WriteLog($"{this.Title} : fn_GrabImageSave-Fail");
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                Console.WriteLine(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return false;
        }

        public void fn_SetLotName(string strLotName)
        {
            // 이미지 저장용 LOT 폴더
            StringBuilder sb = new StringBuilder(strLotName);
            libSetLotName(sb, sb.Length);
        }
    }
}
