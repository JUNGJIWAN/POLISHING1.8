using System.Windows;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserINI;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserConstVision;
using static WaferPolishingSystem.FormMain;

namespace WaferPolishingSystem.Vision
{
    /**
    @class	RecipeManager
    @brief	Recipe File 입출력 관리.
    @remark	
     - 
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/10/6  12:20
    */
    public class RecipeManager
    {
        /**	
        @fn     public void fn_ReadVisionRecipe()
        @brief	Vision Recipe 읽기
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/26  21:07
        */
        public void fn_ReadVisionRecipe()
        {
            string strVisionPath = STRVISIONEPATH + "Vision.ini";
            string strApp;
            strApp = "Setting";
            g_VisionManager._RecipeVision.CurrentRecipeIndex                = fn_Load(strApp, "CurrentRecipeIndex"          , 0          , strVisionPath);
            g_VisionManager._RecipeVision.CurrentRecipeName                 = fn_Load(strApp, "CurrentRecipeName"           , ""         , strVisionPath);
            g_VisionManager._RecipeVision.ResolutionX                       = fn_Load(strApp, "ResolutionX"                 , 0.0        , strVisionPath);
            g_VisionManager._RecipeVision.ResolutionY                       = fn_Load(strApp, "ResolutionY"                 , 0.0        , strVisionPath);
            g_VisionManager._RecipeVision.TiltRotateCenterFromSample        = fn_Load(strApp, "TiltRotateCenterFromSample"  , 0.0        , strVisionPath);
            g_VisionManager._RecipeVision.CamWidth                          = fn_Load(strApp, "CamWidth"                    , 0.0        , strVisionPath);
            g_VisionManager._RecipeVision.CamHeight                         = fn_Load(strApp, "CamHeight"                   , 0.0        , strVisionPath);
            g_VisionManager._RecipeVision.LightPort                         = fn_Load(strApp, "LightPort"                   , ""         , strVisionPath);
            g_VisionManager._RecipeVision.LightBaudRate                     = fn_Load(strApp, "LightBaudRate"               , 0          , strVisionPath);
            g_VisionManager._strImageLogPath                                = fn_Load(strApp, "ImagePath"                   , @"D:\Image\", strVisionPath);

            SEQ_SPIND.fn_SetVisnCamOffset();
            
            strApp = "ToolStorage";
            g_VisionManager._RecipeVision.CentertoToolRelationX     = fn_Load(strApp, "CentertoToolRelationX" ,   2.5 , strVisionPath);
            g_VisionManager._RecipeVision.CentertoToolRelationY     = fn_Load(strApp, "CentertoToolRelationY" ,  -5.0 , strVisionPath);
            g_VisionManager._RecipeVision.TooltoToolDistanceX       = fn_Load(strApp, "TooltoToolDistanceX",  29.0 , strVisionPath);
            g_VisionManager._RecipeVision.TooltoToolDistanceY       = fn_Load(strApp, "TooltoToolDistanceY",  29.0 , strVisionPath);
            g_VisionManager._RecipeVision.PinRadiusPixel            = fn_Load(strApp, "PinRadiusPixel"     , 200.0 , strVisionPath);
            g_VisionManager._RecipeVision.PinSmooth                 = fn_Load(strApp, "PinSmooth"          ,  99.0 , strVisionPath);
            g_VisionManager._RecipeVision.ColToolCount              = fn_Load(strApp, "ColToolCount"       ,     6 , strVisionPath);
            g_VisionManager._RecipeVision.RowToolCount              = fn_Load(strApp, "RowToolCount"       ,    11 , strVisionPath);
            g_VisionManager._RecipeVision.ColOffsetX                = fn_Load(strApp, "ColOffsetX"         ,     0 , strVisionPath);
            g_VisionManager._RecipeVision.ColOffsetY                = fn_Load(strApp, "ColOffsetY"         ,     0 , strVisionPath);
            g_VisionManager._RecipeVision.RowOffsetX                = fn_Load(strApp, "RowOffsetX"         ,     0 , strVisionPath);
            g_VisionManager._RecipeVision.RowOffsetY                = fn_Load(strApp, "RowOffsetY"         ,     0 , strVisionPath);
            g_VisionManager._RecipeVision.ToolLightIR               = fn_Load(strApp, "ToolLightIR"        ,     0 , strVisionPath);
            g_VisionManager._RecipeVision.ToolLightWhite            = fn_Load(strApp, "ToolLightWhite"     ,   150 , strVisionPath);
            g_VisionManager._RecipeVision.AllowAngle                = fn_Load(strApp, "AllowAngle"         ,    10 , strVisionPath);
            g_VisionManager._RecipeVision.InpositionPixelX          = fn_Load(strApp, "InpositionPixelX"   ,    10 , strVisionPath);
            g_VisionManager._RecipeVision.InpositionPixelY          = fn_Load(strApp, "InpositionPixelY"   ,    10 , strVisionPath);
            g_VisionManager._RecipeVision.HardwareOffsetX           = fn_Load(strApp, "HardwareOffsetX"    ,    0.0, strVisionPath);
            g_VisionManager._RecipeVision.HardwareOffsetY           = fn_Load(strApp, "HardwareOffsetY"    ,    0.0, strVisionPath);
        }

        /**	
        @fn     public void fn_WriteVisionRecipe()
        @brief	Vision Recipe 쓰기
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/26  21:08
        */
        public void fn_WriteVisionRecipe()
        {
            string strVisionPath = STRVISIONEPATH + "Vision.ini";
            string strApp;
            strApp = "Setting";
            fn_Save(strApp, "CurrentRecipeIndex"            , g_VisionManager._RecipeVision.CurrentRecipeIndex               , strVisionPath);
            fn_Save(strApp, "CurrentRecipeName"             , g_VisionManager._RecipeVision.CurrentRecipeName                , strVisionPath);
            fn_Save(strApp, "ResolutionX"                   , g_VisionManager._RecipeVision.ResolutionX                      , strVisionPath);
            fn_Save(strApp, "ResolutionY"                   , g_VisionManager._RecipeVision.ResolutionY                      , strVisionPath);
            fn_Save(strApp, "SpindleOffsetX"                , g_VisionManager._RecipeVision.SpindleOffsetX                   , strVisionPath);
            fn_Save(strApp, "SpindleOffsetY"                , g_VisionManager._RecipeVision.SpindleOffsetY                   , strVisionPath);
            fn_Save(strApp, "TiltRotateCenterFromSample"    , g_VisionManager._RecipeVision.TiltRotateCenterFromSample       , strVisionPath);
            fn_Save(strApp, "CamWidth"                      , g_VisionManager._RecipeVision.CamWidth                         , strVisionPath);
            fn_Save(strApp, "CamHeight"                     , g_VisionManager._RecipeVision.CamHeight                        , strVisionPath);
            fn_Save(strApp, "LightPort"                     , g_VisionManager._RecipeVision.LightPort                        , strVisionPath);
            fn_Save(strApp, "LightBaudRate"                 , g_VisionManager._RecipeVision.LightBaudRate                    , strVisionPath);
            //if (g_VisionManager._strImageLogPath == string.Empty) g_VisionManager._strImageLogPath = @"D:\Image\";
            //fn_Save(strApp, "ImagePath"                     , g_VisionManager._strImageLogPath                               , strVisionPath);

            strApp = "ToolStorage";
            fn_Save(strApp, "RefToolX"              , g_VisionManager._RecipeVision.RefToolX               , strVisionPath);
            fn_Save(strApp, "RefToolY"              , g_VisionManager._RecipeVision.RefToolY               , strVisionPath);
            fn_Save(strApp, "CentertoToolRelationX" , g_VisionManager._RecipeVision.CentertoToolRelationX  , strVisionPath);
            fn_Save(strApp, "CentertoToolRelationY" , g_VisionManager._RecipeVision.CentertoToolRelationY  , strVisionPath);
            fn_Save(strApp, "TooltoToolDistanceX"   , g_VisionManager._RecipeVision.TooltoToolDistanceX    , strVisionPath);
            fn_Save(strApp, "TooltoToolDistanceY"   , g_VisionManager._RecipeVision.TooltoToolDistanceY    , strVisionPath);
            fn_Save(strApp, "PinRadiusPixel"        , g_VisionManager._RecipeVision.PinRadiusPixel         , strVisionPath);
            fn_Save(strApp, "PinSmooth"             , g_VisionManager._RecipeVision.PinSmooth              , strVisionPath);
            fn_Save(strApp, "ColToolCount"          , g_VisionManager._RecipeVision.ColToolCount           , strVisionPath);
            fn_Save(strApp, "RowToolCount"          , g_VisionManager._RecipeVision.RowToolCount           , strVisionPath);
            fn_Save(strApp, "ColOffsetX"            , g_VisionManager._RecipeVision.ColOffsetX             , strVisionPath);
            fn_Save(strApp, "ColOffsetY"            , g_VisionManager._RecipeVision.ColOffsetY             , strVisionPath);
            fn_Save(strApp, "RowOffsetX"            , g_VisionManager._RecipeVision.RowOffsetX             , strVisionPath);
            fn_Save(strApp, "RowOffsetY"            , g_VisionManager._RecipeVision.RowOffsetY             , strVisionPath);
            fn_Save(strApp, "ToolLightIR"           , g_VisionManager._RecipeVision.ToolLightIR            , strVisionPath);
            fn_Save(strApp, "ToolLightWhite"        , g_VisionManager._RecipeVision.ToolLightWhite         , strVisionPath);
            fn_Save(strApp, "AllowAngle"            , g_VisionManager._RecipeVision.AllowAngle             , strVisionPath);
            fn_Save(strApp, "InpositionPixelX"      , g_VisionManager._RecipeVision.InpositionPixelX       , strVisionPath);
            fn_Save(strApp, "InpositionPixelY"      , g_VisionManager._RecipeVision.InpositionPixelY       , strVisionPath);

        }
        public void fn_SaveHardwareOffset()
        {
            string strVisionPath = STRVISIONEPATH + "Vision.ini";
            string strApp = "ToolStorage";
            fn_Save(strApp, "HardwareOffsetX", g_VisionManager._RecipeVision.HardwareOffsetX, strVisionPath);
            fn_Save(strApp, "HardwareOffsetY", g_VisionManager._RecipeVision.HardwareOffsetY, strVisionPath);
        }

        /**	
        @fn     public void fn_ReadRecipe(string strFilePath, string strFileName)
        @brief	Model Recipe 읽기
        @return	void
        @param	string strFilePath : 저장된 폴더 위치
        @param	string strFileName : 파일 이름
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/26  21:09
        */
        public void fn_ReadRecipe(string strFilePath, string strFileName)
        {
            if (strFilePath == "")
                strFilePath = STRRECIPEPATH;

            string strFullPath = strFilePath + strFileName + "\\" + strFileName + ".ini";

            g_VisionManager._RecipeModel.ModelCount = fn_Load("Setting", "ModelCount", 0, strFullPath);
            g_VisionManager._RecipeModel.strPath = strFilePath + strFileName + "\\";
            g_VisionManager._RecipeModel.strRecipeName = strFileName;

            //---------------------------------------------------------------------------
            // Loading
            g_VisionManager._RecipeModel.CameraExposureTime[(int)EN_VISION_MODE.Loading]    = fn_Load("Setting", "LoadingCameraExposureTime"    , 0, strFullPath);
            g_VisionManager._RecipeModel.CameraGain        [(int)EN_VISION_MODE.Loading]    = fn_Load("Setting", "LoadingCameraGain"            , 0, strFullPath);
            g_VisionManager._RecipeModel.CameraExposureTime[(int)EN_VISION_MODE.Polishing]  = fn_Load("Setting", "PolishingCameraExposureTime"  , 0, strFullPath);
            g_VisionManager._RecipeModel.CameraGain        [(int)EN_VISION_MODE.Polishing]  = fn_Load("Setting", "PolishingCameraGain"          , 0, strFullPath);
            g_VisionManager._RecipeModel.CameraExposureTime[(int)EN_VISION_MODE.Inspection] = fn_Load("Setting", "InspectionCameraExposureTime" , 0, strFullPath);
            g_VisionManager._RecipeModel.CameraGain        [(int)EN_VISION_MODE.Inspection] = fn_Load("Setting", "InspectionCameraGain"         , 0, strFullPath);

            //---------------------------------------------------------------------------
            // Light
            g_VisionManager._RecipeModel.LightIR           [(int)EN_VISION_MODE.Loading]    = fn_Load("Setting", "LoadingLightIR"      , 0, strFullPath);
            g_VisionManager._RecipeModel.LightWhite        [(int)EN_VISION_MODE.Loading]    = fn_Load("Setting", "LoadingLightWhite"   , 0, strFullPath);
            g_VisionManager._RecipeModel.LightIR           [(int)EN_VISION_MODE.Polishing]  = fn_Load("Setting", "PolishingLightIR"    , 0, strFullPath);
            g_VisionManager._RecipeModel.LightWhite        [(int)EN_VISION_MODE.Polishing]  = fn_Load("Setting", "PolishingLightWhite" , 0, strFullPath);
            g_VisionManager._RecipeModel.LightIR           [(int)EN_VISION_MODE.Inspection] = fn_Load("Setting", "InspectionLightIR"   , 0, strFullPath);
            g_VisionManager._RecipeModel.LightWhite        [(int)EN_VISION_MODE.Inspection] = fn_Load("Setting", "InspectionLightWhite", 0, strFullPath);

            //----------------------------------------------------------------------------
            // IR Filter
            g_VisionManager._RecipeModel.LightIRFilter[(int)EN_VISION_MODE.Loading]    = fn_Load("Setting", "LoadingLightIRFilter"   , 0, strFullPath);
            g_VisionManager._RecipeModel.LightIRFilter[(int)EN_VISION_MODE.Polishing]  = fn_Load("Setting", "PolishingLightIRFilter"       , 0, strFullPath);
            g_VisionManager._RecipeModel.LightIRFilter[(int)EN_VISION_MODE.Inspection] = fn_Load("Setting", "InspectionLightIRFilter", 0, strFullPath);

            for (int nStep1 = 0; nStep1 < 10; nStep1++)
            {
                fn_ReadRecipeModel(strFileName, nStep1);
            }
        }

        /**
        @fn     public void fn_ReadRecipeCleaning(string strFileName)
        @brief	클리닝 레시피 데이터 읽기
        @return	void
        @param	string strFileName : Recipe 파일명
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:19
        */
        public void fn_ReadRecipeCleaning(string strFileName)
        {
            string strFullPath = STRRECIPEPATH + strFileName + "\\" + strFileName + ".ini";
            //---------------------------------------------------------------------------
            // Cleaning
            g_VisionManager._RecipeModel.CleaningCount = fn_Load("Cleaning", "CleaningCount", 0, strFullPath);
            g_VisionManager._RecipeModel.SampleWidth   = fn_Load("Cleaning", "SampleWidth"  , 0, strFullPath);
            g_VisionManager._RecipeModel.SampleHeight  = fn_Load("Cleaning", "SampleHeight" , 0, strFullPath);
            for ( int i = 0; i < 10; i++)
            {
                g_VisionManager._RecipeModel.Cleaning[i].XOffset          = fn_Load("Cleaning", $"XOffset{i + 1}"        , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].YOffset          = fn_Load("Cleaning", $"YOffset{i + 1}"        , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].XSpeed           = fn_Load("Cleaning", $"XSpeed{i + 1}"         , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].YSpeed           = fn_Load("Cleaning", $"YSpeed{i + 1}"         , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].XDistance        = fn_Load("Cleaning", $"XDistance{i + 1}"      , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].YPitch           = fn_Load("Cleaning", $"YPitch{i + 1}"         , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].PathCount        = fn_Load("Cleaning", $"PathCount{i + 1}"      , 0     , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].SpindleRPM       = fn_Load("Cleaning", $"SpindleRPM{i + 1}"     , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].PreWashingRPM    = fn_Load("Cleaning", $"PreWashingRPM{i + 1}"  , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].DehydrationRPM   = fn_Load("Cleaning", $"DehydrationRPM{i + 1}" , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].PreWashingTime   = fn_Load("Cleaning", $"PreWashingTime{i + 1}" , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].DehydrationTime  = fn_Load("Cleaning", $"DehydrationTime{i + 1}", 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Cleaning[i].Force            = fn_Load("Cleaning", $"Force{i + 1}"          , 0.0   , strFullPath);
            }
            //---------------------------------------------------------------------------
        }

        /**	
        @fn		public void fn_ReadRecipeModel(string strRecipeName, int idx)
        @brief	Model Sub 데이터 읽기.
        @return	void
        @param	string strRecipeName : Recipe 파일명
        @param	int    idx           : Read Index
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  19:18
        */
        public void fn_ReadRecipeModel(string strRecipeName, int idx)
        {
            string strFullPath = g_VisionManager._RecipeModel.strPath + strRecipeName + ".ini";
            string strApp = $"Model{idx + 1}";
            string strModelPath = g_VisionManager._RecipeModel.strPath + strApp + ".bmp";
            string strLoadingPath = g_VisionManager._RecipeModel.strPath + $"Loading{idx + 1}.bmp";
            try
            {
                g_VisionManager._RecipeModel.Model[idx] = new ModelList();
                g_VisionManager._RecipeModel.Model[idx].Model = new Rect();
                g_VisionManager._RecipeModel.Model[idx].Algorithm =         (EN_ALGORITHM)fn_Load(strApp, "Algorithm"               , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Enable                          = fn_Load(strApp, "ModelEnable"             , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].strName                         = fn_Load(strApp, "ModelName"               , ""    , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].strImgPath                      = fn_Load(strApp, "ModelImagePath"          , strModelPath, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].SearchIndex                     = fn_Load(strApp, "SearchIndex"             , 0     , strFullPath);

                g_VisionManager._RecipeModel.Model[idx].LoadingTheta                    = fn_Load(strApp, "LoadingTheta"            , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingThetaEnable              = fn_Load(strApp, "LoadingThetaEnable"      , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingMarkWidth                = fn_Load(strApp, "LoadingMarkWidth"        , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingMarkHeight               = fn_Load(strApp, "LoadingMarkHeight"       , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].strLoadingPath                  = fn_Load(strApp, "LoadingImagePath"        , strLoadingPath, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.DetailLevel        = fn_Load(strApp, "L_DetailLevel"           , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.Acceptance         = fn_Load(strApp, "L_Acceptance"            , 70.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.Certainty          = fn_Load(strApp, "L_Certainty"             , 90.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.Smoothness         = fn_Load(strApp, "L_Smoothness"            , 50.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.Angle              = fn_Load(strApp, "L_Angle"                 , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaNeg      = fn_Load(strApp, "L_AngleDeltaNeg"         , 45.0  , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaPos      = fn_Load(strApp, "L_AngleDeltaPos"         , 45.0  , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchAngleRange   = fn_Load(strApp, "L_SearchAngleRange"      , 1     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.Scale              = fn_Load(strApp, "L_Scale"                 , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMinFactor     = fn_Load(strApp, "L_ScaleMinFactor"        , 0.9   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMaxFactor     = fn_Load(strApp, "L_ScaleMaxFactor"        , 1.1   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchScaleRange   = fn_Load(strApp, "L_SearchScaleRange"      , 1     , strFullPath);
                // 20210105 Recipe Loading시 Angle, Scale 옵션 강제 활성화.
                if (g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchAngleRange == 0) g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchAngleRange = 1;
                if (g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaNeg    == 0) g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaNeg    = UserConstVision.DEFAULT_ANGLEMARGIN;
                if (g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaPos    == 0) g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaPos    = UserConstVision.DEFAULT_ANGLEMARGIN;
                if (g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchScaleRange == 0) g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchScaleRange = 1;
                if (g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMinFactor   == 0) g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMinFactor   = (100 - UserConstVision.DEFAULT_SCALEMARGIN) / 100.0;
                if (g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMaxFactor   == 0) g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMaxFactor   = (100 + UserConstVision.DEFAULT_SCALEMARGIN) / 100.0;

                g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchOffsetX      = fn_Load(strApp, "L_SearchOffsetX"         , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchOffsetY      = fn_Load(strApp, "L_SearchOffsetY"         , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchSizeX        = fn_Load(strApp, "L_SearchSizeX"           , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchSizeY        = fn_Load(strApp, "L_SearchSizeY"           , 0     , strFullPath);

                g_VisionManager._RecipeModel.Model[idx].Inspection.ROIX                 = fn_Load(strApp, "InspectionROIInfoX"      , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.ROIY                 = fn_Load(strApp, "InspectionROIInfoY"      , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.ROIW                 = fn_Load(strApp, "InspectionROIInfoW"      , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.ROIH                 = fn_Load(strApp, "InspectionROIInfoH"      , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.Algorithm            = fn_Load(strApp, "InspectionAlgorithm"     , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.RefDistance          = fn_Load(strApp, "InspectionRefDistance"   , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.RefDistance2         = fn_Load(strApp, "InspectionRefDistance2"  , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.Sigma                = fn_Load(strApp, "InspectionSigma"         , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.Threshold            = fn_Load(strApp, "InspectionThreshold"     , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.LowThreshold         = fn_Load(strApp, "InspectionLowThreshold"  , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.HighThreshold        = fn_Load(strApp, "InspectionHighThreshold" , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.Condition            = fn_Load(strApp, "InspectionCondition"     , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.Section1             = fn_Load(strApp, "InsepctionSection1"      , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.Section2             = fn_Load(strApp, "InsepctionSection2"      , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.DLLVHRate            = fn_Load(strApp, "DLLVHRate"               , 2.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.DLLLenRate           = fn_Load(strApp, "DLLLenRate"              , 0.8   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.DLLDiliteCnt         = fn_Load(strApp, "DLLDiliteCnt"            , 1.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.DLLReserve1          = fn_Load(strApp, "DLLLenRate"              , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Inspection.DLLReserve2          = fn_Load(strApp, "DLLLenRate"              , 0.8   , strFullPath);

                g_VisionManager._RecipeModel.Model[idx].Model.X                         = fn_Load(strApp, "ModelX"                  , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Model.Y                         = fn_Load(strApp, "ModelY"                  , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Model.Width                     = fn_Load(strApp, "ModelWidth"              , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].Model.Height                    = fn_Load(strApp, "ModelHeight"             , 0.0   , strFullPath);

                g_VisionManager._RecipeModel.Model[idx].ParamModel.DetailLevel          = fn_Load(strApp, "M_DetailLevel"           , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.Smoothness           = fn_Load(strApp, "M_Smoothness"            , 50     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.Acceptance           = fn_Load(strApp, "M_Acceptance"            , 70     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.AcceptanceTarget     = fn_Load(strApp, "M_AcceptanceTarget"      , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.Angle                = fn_Load(strApp, "M_Angle"                 , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaNeg        = fn_Load(strApp, "M_AngleDeltaNeg"         , 45.0  , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaPos        = fn_Load(strApp, "M_AngleDeltaPos"         , 45.0  , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchAngleRange     = fn_Load(strApp, "M_SearchAngleRange"      , 1     , strFullPath);     
                g_VisionManager._RecipeModel.Model[idx].ParamModel.Certainty            = fn_Load(strApp, "M_Certainty"             , 90.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.CertaintyTarget      = fn_Load(strApp, "M_CertaintyTarget"       , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.Scale                = fn_Load(strApp, "M_Scale"                 , 0.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMinFactor       = fn_Load(strApp, "M_ScaleMinFactor"        , 0.9   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMaxFactor       = fn_Load(strApp, "M_ScaleMaxFactor"        , 1.1   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchScaleRange     = fn_Load(strApp, "M_SearchScaleRange"      , 1     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.Speed                = fn_Load(strApp, "M_Speed"                 , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchOffsetX        = fn_Load(strApp, "M_SearchOffsetX"         , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchOffsetY        = fn_Load(strApp, "M_SearchOffsetY"         , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchSizeX          = fn_Load(strApp, "M_SearchSizeX"           , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchSizeY          = fn_Load(strApp, "M_SearchSizeY"           , 0     , strFullPath);

                g_VisionManager._RecipeModel.Model[idx].ParamPattern.Acceptance         = fn_Load(strApp, "P_Acceptance"            , 70.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.Certainty          = fn_Load(strApp, "P_Certainty"             , 90.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.AngleMode          = fn_Load(strApp, "P_AngleMode"             , 1     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.NegativeDelta      = fn_Load(strApp, "P_NegativeDelta"         , 45.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.PositiveDelta      = fn_Load(strApp, "P_PositiveDelta"         , 45.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.Angle              = fn_Load(strApp, "P_Angle"                 , 1.0   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.Tolerance          = fn_Load(strApp, "P_Tolerance"             , 0.5   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.Accuracy           = fn_Load(strApp, "P_Accuracy"              , 0.1   , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.InterpolationMode  = fn_Load(strApp, "P_InterpolationMode"     , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchOffsetX      = fn_Load(strApp, "P_SearchOffsetX"         , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchOffsetY      = fn_Load(strApp, "P_SearchOffsetY"         , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchSizeX        = fn_Load(strApp, "P_SearchSizeX"           , 0     , strFullPath);
                g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchSizeY        = fn_Load(strApp, "P_SearchSizeY"           , 0     , strFullPath);

                // 20210105 Recipe Loading시 Angle, Scale 옵션 강제 활성화.
                if (g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchAngleRange == 0) g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchAngleRange = 1;
                if (g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaNeg    == 0) g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaNeg    = UserConstVision.DEFAULT_ANGLEMARGIN;
                if (g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaPos    == 0) g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaPos    = UserConstVision.DEFAULT_ANGLEMARGIN;
                if (g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchScaleRange == 0) g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchScaleRange = 1;
                if (g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMinFactor   == 0) g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMinFactor   = (100 - UserConstVision.DEFAULT_SCALEMARGIN) / 100.0;
                if (g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMaxFactor   == 0) g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMaxFactor   = (100 + UserConstVision.DEFAULT_SCALEMARGIN) / 100.0;

                if (g_VisionManager._RecipeModel.Model[idx].ParamPattern.AngleMode      == 0) g_VisionManager._RecipeModel.Model[idx].ParamPattern.AngleMode = 1;
                if (g_VisionManager._RecipeModel.Model[idx].ParamPattern.NegativeDelta  == 0) g_VisionManager._RecipeModel.Model[idx].ParamPattern.NegativeDelta = UserConstVision.DEFAULT_ANGLEMARGIN;
                if (g_VisionManager._RecipeModel.Model[idx].ParamPattern.PositiveDelta  == 0) g_VisionManager._RecipeModel.Model[idx].ParamPattern.PositiveDelta = UserConstVision.DEFAULT_ANGLEMARGIN;

                //---------------------------------------------------------------------------
                g_VisionManager._RecipeModel.Model[idx].UseGlobalLoading              = fn_Load(strApp, "UseGlobalLoading"              , 1, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].UseGlobalPolishing            = fn_Load(strApp, "UseGlobalPolishing"            , 1, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].UseGlobalInspection           = fn_Load(strApp, "UseGlobalInspection"           , 1, strFullPath);

                g_VisionManager._RecipeModel.Model[idx].LoadingCameraExposureTime     = fn_Load(strApp, "LoadingCameraExposureTime"     , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingCameraGain             = fn_Load(strApp, "LoadingCameraGain"             , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingLightIR                = fn_Load(strApp, "LoadingLightIR"                , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingLightWhite             = fn_Load(strApp, "LoadingLightWhite"             , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].LoadingLightIRFilter          = fn_Load(strApp, "LoadingLightIRFilter"          , 0, strFullPath);

                g_VisionManager._RecipeModel.Model[idx].PolishingCameraExposureTime   = fn_Load(strApp, "PolishingCameraExposureTime"   , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].PolishingCameraGain           = fn_Load(strApp, "PolishingCameraGain"           , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].PolishingLightIR              = fn_Load(strApp, "PolishingLightIR"              , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].PolishingLightWhite           = fn_Load(strApp, "PolishingLightWhite"           , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].PolishingLightIRFilter        = fn_Load(strApp, "PolishingLightIRFilter"        , 0, strFullPath);

                g_VisionManager._RecipeModel.Model[idx].InspectionCameraExposureTime  = fn_Load(strApp, "InspectionCameraExposureTime"  , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].InspectionCameraGain          = fn_Load(strApp, "InspectionCameraGain"          , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].InspectionLightIR             = fn_Load(strApp, "InspectionLightIR"             , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].InspectionLightWhite          = fn_Load(strApp, "InspectionLightWhite"          , 0, strFullPath);
                g_VisionManager._RecipeModel.Model[idx].InspectionLightIRFilter       = fn_Load(strApp, "InspectionLightIRFilter"       , 0, strFullPath);
                //---------------------------------------------------------------------------

                g_VisionManager._RecipeModel.Model[idx].MillingCount                    = fn_Load(strApp, "MillingCount"            , 0     , strFullPath);

                for (int nStep2 = 0; nStep2 < 10; nStep2++)
                {
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2] = new MillingList();
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect = new Rect();
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Enable          = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Enable")           , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.X       = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}X")                , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.Y       = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Y")                , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.Width   = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Width")            , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.Height  = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Height")           , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Pitch           = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Pitch")            , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Cycle           = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Cycle")            , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].StartPos        = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}StartPos")         , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Force           = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Force")            , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Tilt            = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Tilt")             , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].RPM             = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}RPM")              , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Speed           = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}Speed")            , 0.0   , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].PathCount       = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}PathCount")        , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].ToolChange      = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}ToolChange")       , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].UtilFill        = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}UtilFill")         , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].UtilDrain       = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}UtilDrain")        , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].UtilType        = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}UtilType")         , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MillingImage    = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}MillingImage")     , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].EPD             = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}EPD")              , 0     , strFullPath);
                    g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].ToolType        = fn_Load(strApp, string.Format($"Milling{nStep2 + 1}ToolType")         , 0     , strFullPath);
                }
            }
            catch
            {

            }
        }

        /**	
        @fn     public void fn_WriteRecipe()
        @brief	Model Recipe 쓰기.
        @return	void.
        @param	void.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/26  21:11
        */
        public void fn_WriteRecipe()
        {
            string strFullPath = g_VisionManager._RecipeModel.strPath + g_VisionManager._RecipeModel.strRecipeName + ".ini";
            // Loading
            fn_Save("Setting", "LoadingCameraExposureTime"    , g_VisionManager._RecipeModel.CameraExposureTime[(int)EN_VISION_MODE.Loading]   , strFullPath);
            fn_Save("Setting", "LoadingCameraGain"            , g_VisionManager._RecipeModel.CameraGain        [(int)EN_VISION_MODE.Loading]   , strFullPath);
            fn_Save("Setting", "PolishingCameraExposureTime"  , g_VisionManager._RecipeModel.CameraExposureTime[(int)EN_VISION_MODE.Polishing] , strFullPath);
            fn_Save("Setting", "PolishingCameraGain"          , g_VisionManager._RecipeModel.CameraGain        [(int)EN_VISION_MODE.Polishing] , strFullPath);
            fn_Save("Setting", "InspectionCameraExposureTime" , g_VisionManager._RecipeModel.CameraExposureTime[(int)EN_VISION_MODE.Inspection], strFullPath);
            fn_Save("Setting", "InspectionCameraGain"         , g_VisionManager._RecipeModel.CameraGain        [(int)EN_VISION_MODE.Inspection], strFullPath);

            fn_Save("Setting", "LoadingLightIR"               , g_VisionManager._RecipeModel.LightIR           [(int)EN_VISION_MODE.Loading]   , strFullPath);
            fn_Save("Setting", "LoadingLightWhite"            , g_VisionManager._RecipeModel.LightWhite        [(int)EN_VISION_MODE.Loading]   , strFullPath);
            fn_Save("Setting", "PolishingLightIR"             , g_VisionManager._RecipeModel.LightIR           [(int)EN_VISION_MODE.Polishing] , strFullPath);
            fn_Save("Setting", "PolishingLightWhite"          , g_VisionManager._RecipeModel.LightWhite        [(int)EN_VISION_MODE.Polishing] , strFullPath);
            fn_Save("Setting", "InspectionLightIR"            , g_VisionManager._RecipeModel.LightIR           [(int)EN_VISION_MODE.Inspection], strFullPath);
            fn_Save("Setting", "InspectionLightWhite"         , g_VisionManager._RecipeModel.LightWhite        [(int)EN_VISION_MODE.Inspection], strFullPath);

            fn_Save("Setting", "LoadingLightIRFilter"         , g_VisionManager._RecipeModel.LightIRFilter     [(int)EN_VISION_MODE.Loading]   , strFullPath);
            fn_Save("Setting", "PolishingLightIRFilter"       , g_VisionManager._RecipeModel.LightIRFilter     [(int)EN_VISION_MODE.Polishing] , strFullPath);
            fn_Save("Setting", "InspectionLightIRFilter"      , g_VisionManager._RecipeModel.LightIRFilter     [(int)EN_VISION_MODE.Inspection], strFullPath);

            for (int nStep1 = 0; nStep1 < (int)10; nStep1++)
            {
                fn_WriteRecipeModel(nStep1);
            }
        }

        public void fn_WriteRecipeCleaning()
        {
            string strFullPath = g_VisionManager._RecipeModel.strPath + g_VisionManager._RecipeModel.strRecipeName + ".ini";
            //---------------------------------------------------------------------------
            // Cleaning
            fn_Save("Cleaning", "CleaningCount", g_VisionManager._RecipeModel.CleaningCount, strFullPath);
            fn_Save("Cleaning", "SampleWidth"  , g_VisionManager._RecipeModel.SampleWidth  , strFullPath);
            fn_Save("Cleaning", "SampleHeight" , g_VisionManager._RecipeModel.SampleHeight , strFullPath);
            for (int i = 0; i < 10; i ++)
            {
                fn_Save("Cleaning", $"XOffset{i + 1}"        , g_VisionManager._RecipeModel.Cleaning[i].XOffset          , strFullPath);
                fn_Save("Cleaning", $"YOffset{i + 1}"        , g_VisionManager._RecipeModel.Cleaning[i].YOffset          , strFullPath);
                fn_Save("Cleaning", $"XSpeed{i + 1}"         , g_VisionManager._RecipeModel.Cleaning[i].XSpeed           , strFullPath);
                fn_Save("Cleaning", $"YSpeed{i + 1}"         , g_VisionManager._RecipeModel.Cleaning[i].YSpeed           , strFullPath);
                fn_Save("Cleaning", $"XDistance{i + 1}"      , g_VisionManager._RecipeModel.Cleaning[i].XDistance        , strFullPath);
                fn_Save("Cleaning", $"YPitch{i + 1}"         , g_VisionManager._RecipeModel.Cleaning[i].YPitch           , strFullPath);
                fn_Save("Cleaning", $"PathCount{i + 1}"      , g_VisionManager._RecipeModel.Cleaning[i].PathCount        , strFullPath);
                fn_Save("Cleaning", $"SpindleRPM{i + 1}"     , g_VisionManager._RecipeModel.Cleaning[i].SpindleRPM       , strFullPath);
                fn_Save("Cleaning", $"PreWashingRPM{i + 1}"  , g_VisionManager._RecipeModel.Cleaning[i].PreWashingRPM    , strFullPath);
                fn_Save("Cleaning", $"DehydrationRPM{i + 1}" , g_VisionManager._RecipeModel.Cleaning[i].DehydrationRPM   , strFullPath);
                fn_Save("Cleaning", $"PreWashingTime{i + 1}" , g_VisionManager._RecipeModel.Cleaning[i].PreWashingTime   , strFullPath);
                fn_Save("Cleaning", $"DehydrationTime{i + 1}", g_VisionManager._RecipeModel.Cleaning[i].DehydrationTime  , strFullPath);
                fn_Save("Cleaning", $"Force{i + 1}"          , g_VisionManager._RecipeModel.Cleaning[i].Force            , strFullPath);
            }
        }

        /**	
        @fn		public void fn_WriteRecipeModel(int idx)
        @brief	Milling Recipe 쓰기.
        @return	void
        @param	int idx : 저장할 Recipe index
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  19:04
        */
        public void fn_WriteRecipeModel(int idx)
        {
            string strFullPath = g_VisionManager._RecipeModel.strPath + g_VisionManager._RecipeModel.strRecipeName + ".ini";
            string strApp;
            //fnGetINIInt
            strApp = string.Format($"Model{idx + 1}");
            fn_Save(strApp, "Algorithm"                 , (int)g_VisionManager._RecipeModel.Model[idx].Algorithm                   , strFullPath);
            fn_Save(strApp, "ModelEnable"               , g_VisionManager._RecipeModel.Model[idx].Enable                           , strFullPath);
            fn_Save(strApp, "ModelName"                 , g_VisionManager._RecipeModel.Model[idx].strName                          , strFullPath);
            fn_Save(strApp, "ModelImagePath"            , g_VisionManager._RecipeModel.Model[idx].strImgPath                       , strFullPath);
            fn_Save(strApp, "SearchIndex"               , g_VisionManager._RecipeModel.Model[idx].SearchIndex                      , strFullPath);

            fn_Save(strApp, "LoadingTheta"              , g_VisionManager._RecipeModel.Model[idx].LoadingTheta                     , strFullPath);
            fn_Save(strApp, "LoadingThetaEnable"        , g_VisionManager._RecipeModel.Model[idx].LoadingThetaEnable               , strFullPath);
            fn_Save(strApp, "LoadingMarkWidth"          , g_VisionManager._RecipeModel.Model[idx].LoadingMarkWidth                 , strFullPath);
            fn_Save(strApp, "LoadingMarkHeight"         , g_VisionManager._RecipeModel.Model[idx].LoadingMarkHeight                , strFullPath);
            fn_Save(strApp, "LoadingImagePath"          , g_VisionManager._RecipeModel.Model[idx].strLoadingPath                   , strFullPath);
            fn_Save(strApp, "L_DetailLevel"             , g_VisionManager._RecipeModel.Model[idx].LoadingParam.DetailLevel         , strFullPath);
            fn_Save(strApp, "L_Acceptance"              , g_VisionManager._RecipeModel.Model[idx].LoadingParam.Acceptance          , strFullPath);
            fn_Save(strApp, "L_Certainty"               , g_VisionManager._RecipeModel.Model[idx].LoadingParam.Certainty           , strFullPath);
            fn_Save(strApp, "L_Smoothness"              , g_VisionManager._RecipeModel.Model[idx].LoadingParam.Smoothness          , strFullPath);
            fn_Save(strApp, "L_Angle"                   , g_VisionManager._RecipeModel.Model[idx].LoadingParam.Angle               , strFullPath);
            fn_Save(strApp, "L_AngleDeltaNeg"           , g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaNeg       , strFullPath);
            fn_Save(strApp, "L_AngleDeltaPos"           , g_VisionManager._RecipeModel.Model[idx].LoadingParam.AngleDeltaPos       , strFullPath);
            fn_Save(strApp, "L_SearchAngleRange"        , g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchAngleRange    , strFullPath);
            fn_Save(strApp, "L_Scale"                   , g_VisionManager._RecipeModel.Model[idx].LoadingParam.Scale               , strFullPath);
            fn_Save(strApp, "L_ScaleMinFactor"          , g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMinFactor      , strFullPath);
            fn_Save(strApp, "L_ScaleMaxFactor"          , g_VisionManager._RecipeModel.Model[idx].LoadingParam.ScaleMaxFactor      , strFullPath);
            fn_Save(strApp, "L_SearchScaleRange"        , g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchScaleRange    , strFullPath);

            fn_Save(strApp, "L_SearchOffsetX"           , g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchOffsetX       , strFullPath);
            fn_Save(strApp, "L_SearchOffsetY"           , g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchOffsetY       , strFullPath);
            fn_Save(strApp, "L_SearchSizeX"             , g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchSizeX         , strFullPath);
            fn_Save(strApp, "L_SearchSizeY"             , g_VisionManager._RecipeModel.Model[idx].LoadingParam.SearchSizeY         , strFullPath);

            fn_Save(strApp, "InspectionROIInfoX"        , g_VisionManager._RecipeModel.Model[idx].Inspection.ROIX.ToString("0.000"), strFullPath);
            fn_Save(strApp, "InspectionROIInfoY"        , g_VisionManager._RecipeModel.Model[idx].Inspection.ROIY.ToString("0.000"), strFullPath);
            fn_Save(strApp, "InspectionROIInfoW"        , g_VisionManager._RecipeModel.Model[idx].Inspection.ROIW.ToString("0.000"), strFullPath);
            fn_Save(strApp, "InspectionROIInfoH"        , g_VisionManager._RecipeModel.Model[idx].Inspection.ROIH.ToString("0.000"), strFullPath);
            fn_Save(strApp, "InspectionAlgorithm"       , g_VisionManager._RecipeModel.Model[idx].Inspection.Algorithm             , strFullPath);
            fn_Save(strApp, "InspectionRefDistance"     , g_VisionManager._RecipeModel.Model[idx].Inspection.RefDistance           , strFullPath);
            fn_Save(strApp, "InspectionRefDistance2"    , g_VisionManager._RecipeModel.Model[idx].Inspection.RefDistance2          , strFullPath);
            fn_Save(strApp, "InspectionSigma"           , g_VisionManager._RecipeModel.Model[idx].Inspection.Sigma                 , strFullPath);
            fn_Save(strApp, "InspectionThreshold"       , g_VisionManager._RecipeModel.Model[idx].Inspection.Threshold             , strFullPath);
            fn_Save(strApp, "InspectionLowThreshold"    , g_VisionManager._RecipeModel.Model[idx].Inspection.LowThreshold          , strFullPath);
            fn_Save(strApp, "InspectionHighThreshold"   , g_VisionManager._RecipeModel.Model[idx].Inspection.HighThreshold         , strFullPath);
            fn_Save(strApp, "InspectionCondition"       , g_VisionManager._RecipeModel.Model[idx].Inspection.Condition             , strFullPath);
            fn_Save(strApp, "InsepctionSection1"        , g_VisionManager._RecipeModel.Model[idx].Inspection.Section1              , strFullPath);
            fn_Save(strApp, "InsepctionSection2"        , g_VisionManager._RecipeModel.Model[idx].Inspection.Section2              , strFullPath);
            fn_Save(strApp, "DLLVHRate"                 , g_VisionManager._RecipeModel.Model[idx].Inspection.DLLVHRate             , strFullPath);
            fn_Save(strApp, "DLLLenRate"                , g_VisionManager._RecipeModel.Model[idx].Inspection.DLLLenRate            , strFullPath);
            fn_Save(strApp, "DLLDiliteCnt"              , g_VisionManager._RecipeModel.Model[idx].Inspection.DLLDiliteCnt          , strFullPath);
            fn_Save(strApp, "DLLLenRate"                , g_VisionManager._RecipeModel.Model[idx].Inspection.DLLReserve1           , strFullPath);
            fn_Save(strApp, "DLLLenRate"                , g_VisionManager._RecipeModel.Model[idx].Inspection.DLLReserve2           , strFullPath);

            fn_Save(strApp, "ModelX"                    , g_VisionManager._RecipeModel.Model[idx].Model.X     .ToString("0.000")   , strFullPath);
            fn_Save(strApp, "ModelY"                    , g_VisionManager._RecipeModel.Model[idx].Model.Y     .ToString("0.000")   , strFullPath);
            fn_Save(strApp, "ModelWidth"                , g_VisionManager._RecipeModel.Model[idx].Model.Width .ToString("0.000")   , strFullPath);
            fn_Save(strApp, "ModelHeight"               , g_VisionManager._RecipeModel.Model[idx].Model.Height.ToString("0.000")   , strFullPath);

            fn_Save(strApp, "M_DetailLevel"             , g_VisionManager._RecipeModel.Model[idx].ParamModel.DetailLevel           , strFullPath);
            fn_Save(strApp, "M_Smoothness"              , g_VisionManager._RecipeModel.Model[idx].ParamModel.Smoothness            , strFullPath);
            fn_Save(strApp, "M_Acceptance"              , g_VisionManager._RecipeModel.Model[idx].ParamModel.Acceptance            , strFullPath);
            fn_Save(strApp, "M_AcceptanceTarget"        , g_VisionManager._RecipeModel.Model[idx].ParamModel.AcceptanceTarget      , strFullPath);
            fn_Save(strApp, "M_Angle"                   , g_VisionManager._RecipeModel.Model[idx].ParamModel.Angle                 , strFullPath);
            fn_Save(strApp, "M_AngleDeltaNeg"           , g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaNeg         , strFullPath);
            fn_Save(strApp, "M_AngleDeltaPos"           , g_VisionManager._RecipeModel.Model[idx].ParamModel.AngleDeltaPos         , strFullPath);
            fn_Save(strApp, "M_SearchAngleRange"        , g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchAngleRange      , strFullPath);
            fn_Save(strApp, "M_Certainty"               , g_VisionManager._RecipeModel.Model[idx].ParamModel.Certainty             , strFullPath);
            fn_Save(strApp, "M_CertaintyTarget"         , g_VisionManager._RecipeModel.Model[idx].ParamModel.CertaintyTarget       , strFullPath);
            fn_Save(strApp, "M_Scale"                   , g_VisionManager._RecipeModel.Model[idx].ParamModel.Scale                 , strFullPath);
            fn_Save(strApp, "M_ScaleMaxFactor"          , g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMaxFactor        , strFullPath);
            fn_Save(strApp, "M_ScaleMinFactor"          , g_VisionManager._RecipeModel.Model[idx].ParamModel.ScaleMinFactor        , strFullPath);
            fn_Save(strApp, "M_SearchScaleRange"        , g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchScaleRange      , strFullPath);
            fn_Save(strApp, "M_Speed"                   , g_VisionManager._RecipeModel.Model[idx].ParamModel.Speed                 , strFullPath);
            fn_Save(strApp, "M_SearchOffsetX"           , g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchOffsetX         , strFullPath);
            fn_Save(strApp, "M_SearchOffsetY"           , g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchOffsetY         , strFullPath);
            fn_Save(strApp, "M_SearchSizeX"             , g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchSizeX           , strFullPath);
            fn_Save(strApp, "M_SearchSizeY"             , g_VisionManager._RecipeModel.Model[idx].ParamModel.SearchSizeY           , strFullPath);

            fn_Save(strApp, "P_Acceptance"              , g_VisionManager._RecipeModel.Model[idx].ParamPattern.Acceptance          , strFullPath);
            fn_Save(strApp, "P_Certainty"               , g_VisionManager._RecipeModel.Model[idx].ParamPattern.Certainty           , strFullPath);
            fn_Save(strApp, "P_AngleMode"               , g_VisionManager._RecipeModel.Model[idx].ParamPattern.AngleMode           , strFullPath);
            fn_Save(strApp, "P_NegativeDelta"           , g_VisionManager._RecipeModel.Model[idx].ParamPattern.NegativeDelta       , strFullPath);
            fn_Save(strApp, "P_PositiveDelta"           , g_VisionManager._RecipeModel.Model[idx].ParamPattern.PositiveDelta       , strFullPath);
            fn_Save(strApp, "P_Angle"                   , g_VisionManager._RecipeModel.Model[idx].ParamPattern.Angle               , strFullPath);
            fn_Save(strApp, "P_Tolerance"               , g_VisionManager._RecipeModel.Model[idx].ParamPattern.Tolerance           , strFullPath);
            fn_Save(strApp, "P_Accuracy"                , g_VisionManager._RecipeModel.Model[idx].ParamPattern.Accuracy            , strFullPath);
            fn_Save(strApp, "P_InterpolationMode"       , g_VisionManager._RecipeModel.Model[idx].ParamPattern.InterpolationMode   , strFullPath);
            fn_Save(strApp, "P_SearchOffsetX"           , g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchOffsetX       , strFullPath);
            fn_Save(strApp, "P_SearchOffsetY"           , g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchOffsetY       , strFullPath);
            fn_Save(strApp, "P_SearchSizeX"             , g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchSizeX         , strFullPath);
            fn_Save(strApp, "P_SearchSizeY"             , g_VisionManager._RecipeModel.Model[idx].ParamPattern.SearchSizeY         , strFullPath);

            //---------------------------------------------------------------------------
            fn_Save(strApp, "UseGlobalLoading"              , g_VisionManager._RecipeModel.Model[idx].UseGlobalLoading              , strFullPath);
            fn_Save(strApp, "UseGlobalPolishing"            , g_VisionManager._RecipeModel.Model[idx].UseGlobalPolishing            , strFullPath);
            fn_Save(strApp, "UseGlobalInspection"           , g_VisionManager._RecipeModel.Model[idx].UseGlobalInspection           , strFullPath);

            fn_Save(strApp, "LoadingCameraExposureTime"     , g_VisionManager._RecipeModel.Model[idx].LoadingCameraExposureTime     , strFullPath);
            fn_Save(strApp, "LoadingCameraGain"             , g_VisionManager._RecipeModel.Model[idx].LoadingCameraGain             , strFullPath);
            fn_Save(strApp, "LoadingLightIR"                , g_VisionManager._RecipeModel.Model[idx].LoadingLightIR                , strFullPath);
            fn_Save(strApp, "LoadingLightWhite"             , g_VisionManager._RecipeModel.Model[idx].LoadingLightWhite             , strFullPath);
            fn_Save(strApp, "LoadingLightIRFilter"          , g_VisionManager._RecipeModel.Model[idx].LoadingLightIRFilter          , strFullPath);

            fn_Save(strApp, "PolishingCameraExposureTime"   , g_VisionManager._RecipeModel.Model[idx].PolishingCameraExposureTime   , strFullPath);
            fn_Save(strApp, "PolishingCameraGain"           , g_VisionManager._RecipeModel.Model[idx].PolishingCameraGain           , strFullPath);
            fn_Save(strApp, "PolishingLightIR"              , g_VisionManager._RecipeModel.Model[idx].PolishingLightIR              , strFullPath);
            fn_Save(strApp, "PolishingLightWhite"           , g_VisionManager._RecipeModel.Model[idx].PolishingLightWhite           , strFullPath);
            fn_Save(strApp, "PolishingLightIRFilter"        , g_VisionManager._RecipeModel.Model[idx].PolishingLightIRFilter        , strFullPath);

            fn_Save(strApp, "InspectionCameraExposureTime"  , g_VisionManager._RecipeModel.Model[idx].InspectionCameraExposureTime  , strFullPath);
            fn_Save(strApp, "InspectionCameraGain"          , g_VisionManager._RecipeModel.Model[idx].InspectionCameraGain          , strFullPath);
            fn_Save(strApp, "InspectionLightIR"             , g_VisionManager._RecipeModel.Model[idx].InspectionLightIR             , strFullPath);
            fn_Save(strApp, "InspectionLightWhite"          , g_VisionManager._RecipeModel.Model[idx].InspectionLightWhite          , strFullPath);
            fn_Save(strApp, "InspectionLightIRFilter"       , g_VisionManager._RecipeModel.Model[idx].InspectionLightIRFilter       , strFullPath);
            //---------------------------------------------------------------------------

            fn_Save(strApp, "MillingCount"              , g_VisionManager._RecipeModel.Model[idx].MillingCount                     , strFullPath);

            for (int nStep2 = 0; nStep2 < 10; nStep2++)
            {
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Enable")           , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Enable                          , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}X")                , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.X     .ToString("0.000"), strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Y")                , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.Y     .ToString("0.000"), strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Width")            , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.Width .ToString("0.000"), strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Height")           , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MilRect.Height.ToString("0.000"), strFullPath);

                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Pitch")            , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Pitch                           , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Cycle")            , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Cycle                           , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}StartPos")         , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].StartPos                        , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Force")            , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Force                           , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Tilt")             , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Tilt                            , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}RPM")              , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].RPM                             , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}Speed")            , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].Speed                           , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}PathCount")        , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].PathCount                       , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}ToolChange")       , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].ToolChange                      , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}UtilFill")         , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].UtilFill                        , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}UtilDrain")        , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].UtilDrain                       , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}UtilType")         , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].UtilType                        , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}MillingImage")     , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].MillingImage                    , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}EPD")              , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].EPD                             , strFullPath);
                fn_Save(strApp, string.Format($"Milling{nStep2 + 1}ToolType")         , g_VisionManager._RecipeModel.Model[idx].Milling[nStep2].ToolType                        , strFullPath);
            }
        }
    }
}
