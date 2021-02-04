using System;
using System.Windows.Media.Imaging;
using WaferPolishingSystem.Vision;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserConstVision;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Collections.Generic;

namespace WaferPolishingSystem.Define
{
    //---------------------------------------------------------------------------
    // delegate
    //---------------------------------------------------------------------------
    
    // LoadingTray
    public delegate void DelegateEnableTheta(bool bTheta);
    public delegate void DelUpdateMainResult(string strResult);
    public delegate void DelClearMainResult();
    //---------------------------------------------------------------------------
    /**
    @class	ModelList
    @brief	다중 모델 자료구조
    @remark	
     - 다중 모델 선택을 위한 자료구조.
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:16
    */
    public class ModelList
    {
        //---------------------------------------------------------------------------
        // Models
        //---------------------------------------------------------------------------
        public int              Enable;
        public int              SearchIndex;
        public string           strName;
        public string           strImgPath;
        public int              MillingCount;
        public string           strLoadingPath;
        public double           LoadingTheta;
        public int              LoadingThetaEnable;
        public int              LoadingMarkWidth;
        public int              LoadingMarkHeight;
        public ST_PARAM_MODEL   LoadingParam;
        public Rect             Model = new Rect();
        public MillingList[]    Milling = new MillingList[10];
        public ST_PARAM_MODEL   ParamModel;
        public ST_PARAM_PATTERN ParamPattern;
        public EN_ALGORITHM     Algorithm;

        public int UseGlobalLoading             ;
        public int UseGlobalPolishing           ;
        public int UseGlobalInspection          ;

        public int LoadingLightIR               ;
        public int LoadingLightWhite            ;
        public int LoadingLightIRFilter         ;
        public int LoadingCameraExposureTime    ;
        public int LoadingCameraGain            ;
        public int PolishingLightIR             ;
        public int PolishingLightWhite          ;
        public int PolishingLightIRFilter       ;
        public int PolishingCameraExposureTime  ;
        public int PolishingCameraGain          ;
        public int InspectionLightIR            ;
        public int InspectionLightWhite         ;
        public int InspectionLightIRFilter      ;
        public int InspectionCameraExposureTime ;
        public int InspectionCameraGain         ;

        //---------------------------------------------------------------------------
        // Inspection
        public ST_INSPECTION Inspection = new ST_INSPECTION();
        public void CopyTo(ModelList Dst)
        {
            if(Dst != null)
            {
                Dst.Enable                       = this.Enable;
                Dst.SearchIndex                  = this.SearchIndex;
                Dst.strName                      = this.strName;
                Dst.Model                        = this.Model;
                Dst.ParamModel                   = this.ParamModel;
                Dst.ParamPattern                 = this.ParamPattern;
                Dst.strImgPath                   = this.strImgPath;
                Dst.MillingCount                 = this.MillingCount;
                Dst.Algorithm                    = this.Algorithm;

                Dst.strLoadingPath               = this.strLoadingPath;
                Dst.LoadingTheta                 = this.LoadingTheta;
                Dst.LoadingThetaEnable           = this.LoadingThetaEnable;
                Dst.LoadingMarkWidth             = this.LoadingMarkWidth;
                Dst.LoadingMarkHeight            = this.LoadingMarkHeight;
                Dst.LoadingParam                 = this.LoadingParam;

                Dst.UseGlobalLoading             = this.UseGlobalLoading   ;
                Dst.UseGlobalPolishing           = this.UseGlobalPolishing ;
                Dst.UseGlobalInspection          = this.UseGlobalInspection;

                Dst.LoadingLightIR               = this.LoadingLightIR              ;
                Dst.LoadingLightWhite            = this.LoadingLightWhite           ;
                Dst.LoadingLightIRFilter         = this.LoadingLightIRFilter        ;
                Dst.LoadingCameraExposureTime    = this.LoadingCameraExposureTime   ;
                Dst.LoadingCameraGain            = this.LoadingCameraGain           ;
                Dst.PolishingLightIR             = this.PolishingLightIR            ;
                Dst.PolishingLightWhite          = this.PolishingLightWhite         ;
                Dst.PolishingLightIRFilter       = this.PolishingLightIRFilter      ;
                Dst.PolishingCameraExposureTime  = this.PolishingCameraExposureTime ;
                Dst.PolishingCameraGain          = this.PolishingCameraGain         ;
                Dst.InspectionLightIR            = this.InspectionLightIR           ;
                Dst.InspectionLightWhite         = this.InspectionLightWhite        ;
                Dst.InspectionLightIRFilter      = this.InspectionLightIRFilter     ;
                Dst.InspectionCameraExposureTime = this.InspectionCameraExposureTime;
                Dst.InspectionCameraGain         = this.InspectionCameraGain        ;

                Dst.Inspection           = this.Inspection;

                for (int i = 0; i < this.Milling.Length; i++)
                {
                    if (Dst.Milling[i] == null)
                        Dst.Milling[i] = new MillingList();
                    this.Milling[i].CopyTo(Dst.Milling[i]);
                }
            }
        }
        public bool Compare(ModelList list)
        {
            bool bRtn = false;

            bRtn |= Enable                           != list.Enable                           ;
            bRtn |= SearchIndex                      != list.SearchIndex                      ;
            bRtn |= strName                          != list.strName                          ;
            bRtn |= Model.X                          != list.Model.X                          ;
            bRtn |= Model.Y                          != list.Model.Y                          ;
            bRtn |= Model.Width                      != list.Model.Width                      ;
            bRtn |= Model.Height                     != list.Model.Height                     ;
                                                                                              
            bRtn |= strLoadingPath                   != list.strLoadingPath                   ;
            bRtn |= LoadingTheta                     != list.LoadingTheta                     ;
            bRtn |= LoadingParam.DetailLevel         != list.LoadingParam.DetailLevel         ;
            bRtn |= LoadingParam.Smoothness          != list.LoadingParam.Smoothness          ;
            bRtn |= LoadingParam.Acceptance          != list.LoadingParam.Acceptance          ;
            bRtn |= LoadingParam.AcceptanceTarget    != list.LoadingParam.AcceptanceTarget    ;
            bRtn |= LoadingParam.Angle               != list.LoadingParam.Angle               ;
            bRtn |= LoadingParam.AngleDeltaNeg       != list.LoadingParam.AngleDeltaNeg       ;
            bRtn |= LoadingParam.AngleDeltaPos       != list.LoadingParam.AngleDeltaPos       ;
            bRtn |= LoadingParam.Certainty           != list.LoadingParam.Certainty           ;
            bRtn |= LoadingParam.CertaintyTarget     != list.LoadingParam.CertaintyTarget     ;
            bRtn |= LoadingParam.Scale               != list.LoadingParam.Scale               ;
            bRtn |= LoadingParam.ScaleMaxFactor      != list.LoadingParam.ScaleMaxFactor      ;
            bRtn |= LoadingParam.ScaleMinFactor      != list.LoadingParam.ScaleMinFactor      ;
            bRtn |= LoadingParam.SearchScaleRange    != list.LoadingParam.SearchScaleRange    ;
            bRtn |= LoadingParam.Speed               != list.LoadingParam.Speed               ;
            bRtn |= LoadingParam.SearchOffsetX       != list.LoadingParam.SearchOffsetX       ;
            bRtn |= LoadingParam.SearchOffsetY       != list.LoadingParam.SearchOffsetY       ;
            bRtn |= LoadingParam.SearchSizeX         != list.LoadingParam.SearchSizeX         ;
            bRtn |= LoadingParam.SearchSizeY         != list.LoadingParam.SearchSizeY         ;
            bRtn |= LoadingMarkWidth                 != list.LoadingMarkWidth                 ;
            bRtn |= LoadingMarkHeight                != list.LoadingMarkHeight                ;

            bRtn |= Inspection.ROIX                  != list.Inspection.ROIX            ;
            bRtn |= Inspection.ROIY                  != list.Inspection.ROIY            ;
            bRtn |= Inspection.ROIW                  != list.Inspection.ROIW            ;
            bRtn |= Inspection.ROIH                  != list.Inspection.ROIH            ;
            bRtn |= Inspection.Algorithm             != list.Inspection.Algorithm       ;
            bRtn |= Inspection.RefDistance           != list.Inspection.RefDistance     ;
            bRtn |= Inspection.Sigma                 != list.Inspection.Sigma           ;
            bRtn |= Inspection.Threshold             != list.Inspection.Threshold       ;
            bRtn |= Inspection.LowThreshold          != list.Inspection.LowThreshold    ;
            bRtn |= Inspection.HighThreshold         != list.Inspection.HighThreshold   ;

            bRtn |= LoadingLightIR                   != list.LoadingLightIR              ;
            bRtn |= LoadingLightWhite                != list.LoadingLightWhite           ;
            bRtn |= LoadingLightIRFilter             != list.LoadingLightIRFilter        ;
            bRtn |= LoadingCameraExposureTime        != list.LoadingCameraExposureTime   ;
            bRtn |= LoadingCameraGain                != list.LoadingCameraGain           ;
            bRtn |= PolishingLightIR                 != list.PolishingLightIR            ;
            bRtn |= PolishingLightWhite              != list.PolishingLightWhite         ;
            bRtn |= PolishingLightIRFilter           != list.PolishingLightIRFilter      ;
            bRtn |= PolishingCameraExposureTime      != list.PolishingCameraExposureTime ;
            bRtn |= PolishingCameraGain              != list.PolishingCameraGain         ;
            bRtn |= InspectionLightIR                != list.InspectionLightIR           ;
            bRtn |= InspectionLightWhite             != list.InspectionLightWhite        ;
            bRtn |= InspectionLightIRFilter          != list.InspectionLightIRFilter     ;
            bRtn |= InspectionCameraExposureTime     != list.InspectionCameraExposureTime;
            bRtn |= InspectionCameraGain             != list.InspectionCameraGain        ;

            for (int i = 0;i < 10; i ++)
            {
                bRtn |= Milling[i].Enable           != list.Milling[i].Enable        ;
                bRtn |= Milling[i].MilRect.X        != list.Milling[i].MilRect.X     ;
                bRtn |= Milling[i].MilRect.Y        != list.Milling[i].MilRect.Y     ;
                bRtn |= Milling[i].MilRect.Width    != list.Milling[i].MilRect.Width ;
                bRtn |= Milling[i].MilRect.Height   != list.Milling[i].MilRect.Height;
                bRtn |= Milling[i].Pitch            != list.Milling[i].Pitch         ;
                bRtn |= Milling[i].Cycle            != list.Milling[i].Cycle         ;
                bRtn |= Milling[i].StartPos         != list.Milling[i].StartPos      ;
                bRtn |= Milling[i].Force            != list.Milling[i].Force         ;
                bRtn |= Milling[i].Tilt             != list.Milling[i].Tilt          ;
                bRtn |= Milling[i].RPM              != list.Milling[i].RPM           ;
                bRtn |= Milling[i].Speed            != list.Milling[i].Speed         ;
                bRtn |= Milling[i].PathCount        != list.Milling[i].PathCount     ;
                bRtn |= Milling[i].ToolChange       != list.Milling[i].ToolChange    ;
                bRtn |= Milling[i].UtilFill         != list.Milling[i].UtilFill      ;
                bRtn |= Milling[i].UtilDrain        != list.Milling[i].UtilDrain     ;
                bRtn |= Milling[i].UtilType         != list.Milling[i].UtilType      ;
                bRtn |= Milling[i].MillingImage     != list.Milling[i].MillingImage  ;
                bRtn |= Milling[i].EPD              != list.Milling[i].EPD           ;
                bRtn |= Milling[i].ToolType         != list.Milling[i].ToolType      ;
            }
            bRtn |= ParamModel.DetailLevel         != list.ParamModel.DetailLevel         ;
            bRtn |= ParamModel.Smoothness          != list.ParamModel.Smoothness          ;
            bRtn |= ParamModel.Acceptance          != list.ParamModel.Acceptance          ;
            bRtn |= ParamModel.AcceptanceTarget    != list.ParamModel.AcceptanceTarget    ;
            bRtn |= ParamModel.Angle               != list.ParamModel.Angle               ;
            bRtn |= ParamModel.AngleDeltaNeg       != list.ParamModel.AngleDeltaNeg       ;
            bRtn |= ParamModel.AngleDeltaPos       != list.ParamModel.AngleDeltaPos       ;
            bRtn |= ParamModel.Certainty           != list.ParamModel.Certainty           ;
            bRtn |= ParamModel.CertaintyTarget     != list.ParamModel.CertaintyTarget     ;
            bRtn |= ParamModel.Scale               != list.ParamModel.Scale               ;
            bRtn |= ParamModel.ScaleMaxFactor      != list.ParamModel.ScaleMaxFactor      ;
            bRtn |= ParamModel.ScaleMinFactor      != list.ParamModel.ScaleMinFactor      ;
            bRtn |= ParamModel.SearchScaleRange    != list.ParamModel.SearchScaleRange    ;
            bRtn |= ParamModel.Speed               != list.ParamModel.Speed               ;
            bRtn |= ParamModel.SearchOffsetX       != list.ParamModel.SearchOffsetX       ;
            bRtn |= ParamModel.SearchOffsetY       != list.ParamModel.SearchOffsetY       ;
            bRtn |= ParamModel.SearchSizeX         != list.ParamModel.SearchSizeX         ;
            bRtn |= ParamModel.SearchSizeY         != list.ParamModel.SearchSizeY         ;

            bRtn |= ParamPattern.Acceptance        != list.ParamPattern.Acceptance        ;
            bRtn |= ParamPattern.Certainty         != list.ParamPattern.Certainty         ;
            bRtn |= ParamPattern.AngleMode         != list.ParamPattern.AngleMode         ;
            bRtn |= ParamPattern.NegativeDelta     != list.ParamPattern.NegativeDelta     ;
            bRtn |= ParamPattern.PositiveDelta     != list.ParamPattern.PositiveDelta     ;
            bRtn |= ParamPattern.Angle             != list.ParamPattern.Angle             ;
            bRtn |= ParamPattern.Tolerance         != list.ParamPattern.Tolerance         ;
            bRtn |= ParamPattern.Accuracy          != list.ParamPattern.Accuracy          ;
            bRtn |= ParamPattern.InterpolationMode != list.ParamPattern.InterpolationMode ;
            bRtn |= ParamPattern.SearchOffsetX     != list.ParamPattern.SearchOffsetX     ;
            bRtn |= ParamPattern.SearchOffsetY     != list.ParamPattern.SearchOffsetY     ;
            bRtn |= ParamPattern.SearchSizeX       != list.ParamPattern.SearchSizeX       ;
            bRtn |= ParamPattern.SearchSizeY       != list.ParamPattern.SearchSizeY       ;

            bRtn |= strImgPath                     != list.strImgPath   ;
            bRtn |= MillingCount                   != list.MillingCount ;
            bRtn |= Algorithm                      != list.Algorithm    ;
            return bRtn;
        }
    };

    public class MillingList
    {
        public int      Enable;
        public Rect     MilRect = new Rect();
        public double   Pitch;
        public int      Cycle;
        public int      StartPos;
        public double   Force;
        public double   Tilt;
        public double   RPM;
        public double   Speed;
        public int      PathCount;
        public int      ToolChange;
        public int      UtilFill;
        public int      UtilDrain;
        public int      UtilType;
        public int      MillingImage;
        public int      EPD;
        public int      ToolType;
        public void CopyTo(MillingList Dst)
        {
            if(Dst != null)
            {
                
                Dst.Enable          = this.Enable;
                Dst.MilRect         = this.MilRect;
                Dst.Pitch           = this.Pitch;
                Dst.Cycle           = this.Cycle;
                Dst.StartPos        = this.StartPos;
                Dst.Force           = this.Force;
                Dst.Tilt            = this.Tilt;
                Dst.RPM             = this.RPM;
                Dst.Speed           = this.Speed;
                Dst.PathCount       = this.PathCount;
                Dst.ToolChange      = this.ToolChange;
                Dst.UtilFill        = this.UtilFill;
                Dst.UtilDrain       = this.UtilDrain;
                Dst.UtilType        = this.UtilType;
                Dst.MillingImage    = this.MillingImage;
                Dst.EPD             = this.EPD;
                Dst.ToolType        = this.ToolType;
            }
        }
        public bool Compare(MillingList param, List<string> changelist = null)
        {
            bool bRtn = false;

            bRtn |= this.Enable         != param.Enable         ; if (this.Enable         != param.Enable      ) changelist?.Add($"     Enable : {          param.Enable      } -> {Enable      }");
            bRtn |= this.MilRect        != param.MilRect        ; if (this.MilRect        != param.MilRect     ) changelist?.Add($"     MilRect(X,Y,W,H) : {param.MilRect     } -> {MilRect     }");
            bRtn |= this.Pitch          != param.Pitch          ; if (this.Pitch          != param.Pitch       ) changelist?.Add($"     Pitch : {           param.Pitch       } -> {Pitch       }");
            bRtn |= this.Cycle          != param.Cycle          ; if (this.Cycle          != param.Cycle       ) changelist?.Add($"     Cycle : {           param.Cycle       } -> {Cycle       }");
            bRtn |= this.StartPos       != param.StartPos       ; if (this.StartPos       != param.StartPos    ) changelist?.Add($"     StartPos : {        param.StartPos    } -> {StartPos    }");
            bRtn |= this.Force          != param.Force          ; if (this.Force          != param.Force       ) changelist?.Add($"     Force : {           param.Force       } -> {Force       }");
            bRtn |= this.Tilt           != param.Tilt           ; if (this.Tilt           != param.Tilt        ) changelist?.Add($"     Tilt : {            param.Tilt        } -> {Tilt        }");
            bRtn |= this.RPM            != param.RPM            ; if (this.RPM            != param.RPM         ) changelist?.Add($"     RPM : {             param.RPM         } -> {RPM         }");
            bRtn |= this.Speed          != param.Speed          ; if (this.Speed          != param.Speed       ) changelist?.Add($"     Speed : {           param.Speed       } -> {Speed       }");
            bRtn |= this.PathCount      != param.PathCount      ; if (this.PathCount      != param.PathCount   ) changelist?.Add($"     PathCount : {       param.PathCount   } -> {PathCount   }");
            bRtn |= this.ToolChange     != param.ToolChange     ; if (this.ToolChange     != param.ToolChange  ) changelist?.Add($"     ToolChange : {      param.ToolChange  } -> {ToolChange  }");
            bRtn |= this.UtilFill       != param.UtilFill       ; if (this.UtilFill       != param.UtilFill    ) changelist?.Add($"     UtilFill : {        param.UtilFill    } -> {UtilFill    }");
            bRtn |= this.UtilDrain      != param.UtilDrain      ; if (this.UtilDrain      != param.UtilDrain   ) changelist?.Add($"     UtilDrain : {       param.UtilDrain   } -> {UtilDrain   }");
            bRtn |= this.UtilType       != param.UtilType       ; if (this.UtilType       != param.UtilType    ) changelist?.Add($"     UtilType : {        param.UtilType    } -> {UtilType    }");
            bRtn |= this.MillingImage   != param.MillingImage   ; if (this.MillingImage   != param.MillingImage) changelist?.Add($"     MillingImage : {    param.MillingImage} -> {MillingImage}");
            bRtn |= this.EPD            != param.EPD            ; if (this.EPD            != param.EPD         ) changelist?.Add($"     EPD : {             param.EPD         } -> {EPD         }");
            bRtn |= this.ToolType       != param.ToolType       ; if (this.ToolType       != param.ToolType    ) changelist?.Add($"     ToolType : {        param.ToolType    } -> {ToolType    }");

            return bRtn;
        }
    }
    //---------------------------------------------------------------------------
    /**
    @class	RecipeList
    @brief	레시피 설정을 위한 자료구조
    @remark	
     - Recipe Read시 사용할 자료구조.
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:16
    */
    public class RecipeList
    {

        public string strPath;
        public string strRecipeName;
        //---------------------------------------------------------------------------
        // Setting
        public int   ModelCount;
        public int[] LightIR             = new int[(int)EN_VISION_MODE.MemberCount];
        public int[] LightWhite          = new int[(int)EN_VISION_MODE.MemberCount];
        public int[] LightIRFilter       = new int[(int)EN_VISION_MODE.MemberCount];
        public int[] CameraExposureTime  = new int[(int)EN_VISION_MODE.MemberCount];
        public int[] CameraGain          = new int[(int)EN_VISION_MODE.MemberCount];
        //---------------------------------------------------------------------------
        // Cleaning
        public int    CleaningCount;
        public double SampleWidth;
        public double SampleHeight;
        public ST_Cleaning[]     Cleaning = new ST_Cleaning[10];
        
        //---------------------------------------------------------------------------
        // Model
        public ModelList[]      Model = new ModelList[10];

        public void CopyTo(RecipeList Dst)
        {
            if(Dst != null)
            { 
                Dst.strPath              = this.strPath;
                Dst.strRecipeName        = this.strRecipeName;
                Dst.ModelCount           = this.ModelCount;

                for (int i = 0; i < (int)EN_VISION_MODE.MemberCount; i++)
                {
                    Dst.LightIR             [i] = this.LightIR             [i];
                    Dst.LightWhite          [i] = this.LightWhite          [i];
                    Dst.LightIRFilter       [i] = this.LightIRFilter       [i];
                    Dst.CameraExposureTime  [i] = this.CameraExposureTime  [i];
                    Dst.CameraGain          [i] = this.CameraGain          [i];
                }
                Dst.CleaningCount = this.CleaningCount;
                Dst.SampleWidth   = this.SampleWidth;
                Dst.SampleHeight  = this.SampleHeight;
                for (int i = 0; i < this.Cleaning.Length; i++)
                {  
                    Dst.Cleaning[i] = this.Cleaning[i];
                }

                for (int i = 0; i < this.Model.Length; i ++)
                {
                    if (Dst.Model[i] == null) Dst.Model[i] = new ModelList();
                    this.Model[i].CopyTo(Dst.Model[i]);
                }
            }
        }
    };

    public struct ST_Cleaning
    {
        public double XOffset;
        public double YOffset;
        public double XSpeed;
        public double YSpeed;
        public double XDistance;
        public double YPitch;
        public int    PathCount;
        public double SpindleRPM;
        public double PreWashingRPM;
        public double DehydrationRPM;
        public double PreWashingTime;
        public double DehydrationTime;
        public double Force;
    }

    public struct ST_INSPECTION
    {
        public double ROIX          ;
        public double ROIY          ;
        public double ROIW          ;
        public double ROIH          ;
        public int    Algorithm     ;
        public double RefDistance   ;
        public double RefDistance2  ;
        public double Sigma         ;
        public double Threshold     ;
        public double LowThreshold  ;
        public double HighThreshold ;
        public int    Condition     ;
        public int    Section1      ;
        public int    Section2      ;
        public double DLLVHRate     ;
        public double DLLLenRate    ;
        public double DLLDiliteCnt  ;
        public double DLLReserve1   ;
        public double DLLReserve2   ;

        public bool Compare(ST_INSPECTION param, List<string> changelist = null)
        {
            bool bRtn = false;

            bRtn |= this.ROIX           != param.ROIX          ; if(this.ROIX           != param.ROIX         ) changelist?.Add($"ROIX : {          param.ROIX         } -> {ROIX         }");
            bRtn |= this.ROIY           != param.ROIY          ; if(this.ROIY           != param.ROIY         ) changelist?.Add($"ROIY : {          param.ROIY         } -> {ROIY         }");
            bRtn |= this.ROIW           != param.ROIW          ; if(this.ROIW           != param.ROIW         ) changelist?.Add($"ROIW : {          param.ROIW         } -> {ROIW         }");
            bRtn |= this.ROIH           != param.ROIH          ; if(this.ROIH           != param.ROIH         ) changelist?.Add($"ROIH : {          param.ROIH         } -> {ROIH         }");
            bRtn |= this.Algorithm      != param.Algorithm     ; if(this.Algorithm      != param.Algorithm    ) changelist?.Add($"Algorithm : {     param.Algorithm    } -> {Algorithm    }");
            bRtn |= this.RefDistance    != param.RefDistance   ; if(this.RefDistance    != param.RefDistance  ) changelist?.Add($"RefDistance : {   param.RefDistance  } -> {RefDistance  }");
            bRtn |= this.RefDistance2   != param.RefDistance2  ; if(this.RefDistance2   != param.RefDistance2 ) changelist?.Add($"RefDistance2 : {  param.RefDistance2 } -> {RefDistance2 }");
            bRtn |= this.Sigma          != param.Sigma         ; if(this.Sigma          != param.Sigma        ) changelist?.Add($"Sigma : {         param.Sigma        } -> {Sigma        }");
            bRtn |= this.Threshold      != param.Threshold     ; if(this.Threshold      != param.Threshold    ) changelist?.Add($"Threshold : {     param.Threshold    } -> {Threshold    }");
            bRtn |= this.LowThreshold   != param.LowThreshold  ; if(this.LowThreshold   != param.LowThreshold ) changelist?.Add($"LowThreshold : {  param.LowThreshold } -> {LowThreshold }");
            bRtn |= this.HighThreshold  != param.HighThreshold ; if(this.HighThreshold  != param.HighThreshold) changelist?.Add($"HighThreshold : { param.HighThreshold} -> {HighThreshold}");
            bRtn |= this.Condition      != param.Condition     ; if(this.Condition      != param.Condition    ) changelist?.Add($"Condition : {     param.Condition    } -> {Condition    }");
            bRtn |= this.Section1       != param.Section1      ; if(this.Section1       != param.Section1     ) changelist?.Add($"Section1 : {      param.Section1     } -> {Section1     }");
            bRtn |= this.Section2       != param.Section2      ; if(this.Section2       != param.Section2     ) changelist?.Add($"Section2 : {      param.Section2     } -> {Section2     }");
            bRtn |= this.DLLVHRate      != param.DLLVHRate     ; if(this.DLLVHRate      != param.DLLVHRate    ) changelist?.Add($"DLLVHRate : {     param.DLLVHRate    } -> {DLLVHRate    }");
            bRtn |= this.DLLLenRate     != param.DLLLenRate    ; if(this.DLLLenRate     != param.DLLLenRate   ) changelist?.Add($"DLLLenRate : {    param.DLLLenRate   } -> {DLLLenRate   }");
            bRtn |= this.DLLDiliteCnt   != param.DLLDiliteCnt  ; if(this.DLLDiliteCnt   != param.DLLDiliteCnt ) changelist?.Add($"DLLDiliteCnt : {  param.DLLDiliteCnt } -> {DLLDiliteCnt }");
            bRtn |= this.DLLReserve1    != param.DLLReserve1   ; if(this.DLLReserve1    != param.DLLReserve1  ) changelist?.Add($"DLLReserve1 : {   param.DLLReserve1  } -> {DLLReserve1  }");
            bRtn |= this.DLLReserve2    != param.DLLReserve2   ; if(this.DLLReserve2    != param.DLLReserve2  ) changelist?.Add($"DLLReserve2 : {   param.DLLReserve2  } -> {DLLReserve2  }");

            return bRtn;
        }
    }

    //---------------------------------------------------------------------------
    /**
    @struct	ST_PARAM_MODEL
    @brief	ModelFinder 알고리즘용 파라메터 자료구조
    @remark	
     - Mil에서 사용할 ModelFinder용 파라메터.
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:19
    */
    public struct ST_PARAM_MODEL
    {
        public int      DetailLevel     ; // 엣지 추출 감도 M_HIGH, M_MEDIUM(*), M_VERY_HIGH
        public double   Smoothness      ; // 엣지 노이즈 처리 감도 50.0(*)(0~100.0) // 후처리?
        public double   Acceptance      ; // 모델의 1차 검색 스코어.(허용수준)
        public double   AcceptanceTarget; // 모델에 정의되지 않은 엣지의 허용 수준
        public double   Angle           ; // 기본 발생으로 반환될 수 있는 각도를 제한한다.
        public double   AngleDeltaNeg   ; // 기본 발생으로 반환될 수 있는 각도를 제한한다.
        public double   AngleDeltaPos   ; // 기본 발생으로 반환될 수 있는 각도를 제한한다.
        public int      SearchAngleRange; // 각도 범위 탐색 M_DISABLE(*) - 201103
        public double   Certainty       ; // 모델 결정을 위한 스코어.(확실성)
        public double   CertaintyTarget ; // 특정 모델 target 스코어 확실성
        public double   Scale           ; // 축척 범위 검색
        public double   ScaleMaxFactor  ; // 축척 범위 검색
        public double   ScaleMinFactor  ; // 축척 범위 검색
        public int      SearchScaleRange; // 축척 범위 탐색 M_DISABLE(*)
        public int      Speed           ; // 검색 속도
        public int      SearchOffsetX   ; // 검색 ROI
        public int      SearchOffsetY   ; // 검색 ROI
        public int      SearchSizeX     ; // 검색 ROI
        public int      SearchSizeY     ; // 검색 ROI
        public byte     dummy1          ;
        public byte     dummy2          ;
        public byte     dummy3          ;
        public byte     dummy4          ;

        public bool Compare(ST_PARAM_MODEL param, List<string> changelist = null)
        {
            bool bRtn = false;
            bRtn |= this.DetailLevel      != param.DetailLevel     ;
            if (this.DetailLevel != param.DetailLevel)
            {
                if (this.DetailLevel > -1 && this.DetailLevel < (int)EN_DETAILLEVEL.MemberCount && param.DetailLevel > -1 && param.DetailLevel < (int)EN_DETAILLEVEL.MemberCount)
                    changelist?.Add($"ModelFinder DetailLevel : {(EN_DETAILLEVEL)param.DetailLevel} -> {(EN_DETAILLEVEL)DetailLevel}");
                else
                    changelist?.Add($"ModelFinder DetailLevel : {param.DetailLevel} -> {DetailLevel}");
            }
            bRtn |= this.Smoothness       != param.Smoothness      ; if(this.Smoothness       != param.Smoothness      ) changelist?.Add($"ModelFinder Smoothness : {      param.Smoothness      } -> {this.Smoothness       }");
            bRtn |= this.Acceptance       != param.Acceptance      ; if(this.Acceptance       != param.Acceptance      ) changelist?.Add($"ModelFinder Acceptance : {      param.Acceptance      } -> {this.Acceptance       }");
            bRtn |= this.AcceptanceTarget != param.AcceptanceTarget; if(this.AcceptanceTarget != param.AcceptanceTarget) changelist?.Add($"ModelFinder AcceptanceTarget : {param.AcceptanceTarget} -> {this.AcceptanceTarget }");
            bRtn |= this.Angle            != param.Angle           ; if(this.Angle            != param.Angle           ) changelist?.Add($"ModelFinder Angle : {           param.Angle           } -> {this.Angle            }");
            bRtn |= this.AngleDeltaNeg    != param.AngleDeltaNeg   ; if(this.AngleDeltaNeg    != param.AngleDeltaNeg   ) changelist?.Add($"ModelFinder AngleDeltaNeg : {   param.AngleDeltaNeg   } -> {this.AngleDeltaNeg    }");
            bRtn |= this.AngleDeltaPos    != param.AngleDeltaPos   ; if(this.AngleDeltaPos    != param.AngleDeltaPos   ) changelist?.Add($"ModelFinder AngleDeltaPos : {   param.AngleDeltaPos   } -> {this.AngleDeltaPos    }");
            bRtn |= this.SearchAngleRange != param.SearchAngleRange; if(this.SearchAngleRange != param.SearchAngleRange) changelist?.Add($"ModelFinder SearchAngleRange : {param.SearchAngleRange} -> {this.SearchAngleRange }");
            bRtn |= this.Certainty        != param.Certainty       ; if(this.Certainty        != param.Certainty       ) changelist?.Add($"ModelFinder Certainty : {       param.Certainty       } -> {this.Certainty        }");
            bRtn |= this.CertaintyTarget  != param.CertaintyTarget ; if(this.CertaintyTarget  != param.CertaintyTarget ) changelist?.Add($"ModelFinder CertaintyTarget : { param.CertaintyTarget } -> {this.CertaintyTarget  }");
            bRtn |= this.Scale            != param.Scale           ; if(this.Scale            != param.Scale           ) changelist?.Add($"ModelFinder Scale : {           param.Scale           } -> {this.Scale            }");
            bRtn |= this.ScaleMaxFactor   != param.ScaleMaxFactor  ; if(this.ScaleMaxFactor   != param.ScaleMaxFactor  ) changelist?.Add($"ModelFinder ScaleMaxFactor : {  param.ScaleMaxFactor  } -> {this.ScaleMaxFactor   }");
            bRtn |= this.ScaleMinFactor   != param.ScaleMinFactor  ; if(this.ScaleMinFactor   != param.ScaleMinFactor  ) changelist?.Add($"ModelFinder ScaleMinFactor : {  param.ScaleMinFactor  } -> {this.ScaleMinFactor   }");
            bRtn |= this.SearchScaleRange != param.SearchScaleRange; if(this.SearchScaleRange != param.SearchScaleRange) changelist?.Add($"ModelFinder SearchScaleRange : {param.SearchScaleRange} -> {this.SearchScaleRange }");
            bRtn |= this.Speed            != param.Speed           ; if(this.Speed            != param.Speed           ) changelist?.Add($"ModelFinder Speed : {           param.Speed           } -> {this.Speed            }");
            bRtn |= this.SearchOffsetX    != param.SearchOffsetX   ; if(this.SearchOffsetX    != param.SearchOffsetX   ) changelist?.Add($"ModelFinder SearchOffsetX : {   param.SearchOffsetX   } -> {this.SearchOffsetX    }");
            bRtn |= this.SearchOffsetY    != param.SearchOffsetY   ; if(this.SearchOffsetY    != param.SearchOffsetY   ) changelist?.Add($"ModelFinder SearchOffsetY : {   param.SearchOffsetY   } -> {this.SearchOffsetY    }");
            bRtn |= this.SearchSizeX      != param.SearchSizeX     ; if(this.SearchSizeX      != param.SearchSizeX     ) changelist?.Add($"ModelFinder SearchSizeX : {     param.SearchSizeX     } -> {this.SearchSizeX      }");
            bRtn |= this.SearchSizeY      != param.SearchSizeY     ; if(this.SearchSizeY      != param.SearchSizeY     ) changelist?.Add($"ModelFinder SearchSizeY : {     param.SearchSizeY     } -> {this.SearchSizeY      }");
            return bRtn;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    /**
    @struct	ST_PARAM_PATTERN
    @brief	PatternMatching 알고리즘용 파라메터 자료구조
    @remark	
     - Mil에서 사용할 PatternMatching 파라메터.
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:19
    */
    public struct ST_PARAM_PATTERN
    {
        public double   Acceptance          ; // 모델 스코어 허용 수준
        public double   Certainty           ; // 모델 스코어 모델의 확실성
        public int      AngleMode           ; // 각도 검색 허용 여부
        public double   NegativeDelta       ; // 회전 공차를 기준으로 하한선
        public double   PositiveDelta       ; // 회전 공자를 기준으로 상한선
        public double   Angle               ; // 기본 회전 각도
        public double   Tolerance           ; // 회전공차
        public double   Accuracy            ; // 각도 정확도, 검색을 미세조정, 회전공차보다 작아야함.
        public int      InterpolationMode   ; // Mark 회전시 보간 모드
        public int      SearchOffsetX       ; // 검색 ROI
        public int      SearchOffsetY       ; // 검색 ROI
        public int      SearchSizeX         ; // 검색 ROI
        public int      SearchSizeY         ; // 검색 ROI

        public bool Compare(ST_PARAM_PATTERN param, List<string> changelist = null)
        {
            bool bRtn = false;
            bRtn |= this.Acceptance          != param.Acceptance          ; if (this.Acceptance          != param.Acceptance       ) changelist?.Add($"Pattern Matching Acceptance : {       param.Acceptance       } -> {this.Acceptance       }");
            bRtn |= this.Certainty           != param.Certainty           ; if (this.Certainty           != param.Certainty        ) changelist?.Add($"Pattern Matching Certainty : {        param.Certainty        } -> {this.Certainty        }");
            bRtn |= this.AngleMode           != param.AngleMode           ; if (this.AngleMode           != param.AngleMode        ) changelist?.Add($"Pattern Matching AngleMode : {        param.AngleMode        } -> {this.AngleMode        }");
            bRtn |= this.NegativeDelta       != param.NegativeDelta       ; if (this.NegativeDelta       != param.NegativeDelta    ) changelist?.Add($"Pattern Matching NegativeDelta : {    param.NegativeDelta    } -> {this.NegativeDelta    }");
            bRtn |= this.PositiveDelta       != param.PositiveDelta       ; if (this.PositiveDelta       != param.PositiveDelta    ) changelist?.Add($"Pattern Matching PositiveDelta : {    param.PositiveDelta    } -> {this.PositiveDelta    }");
            bRtn |= this.Angle               != param.Angle               ; if (this.Angle               != param.Angle            ) changelist?.Add($"Pattern Matching Angle : {            param.Angle            } -> {this.Angle            }");
            bRtn |= this.Tolerance           != param.Tolerance           ; if (this.Tolerance           != param.Tolerance        ) changelist?.Add($"Pattern Matching Tolerance : {        param.Tolerance        } -> {this.Tolerance        }");
            bRtn |= this.Accuracy            != param.Accuracy            ; if (this.Accuracy            != param.Accuracy         ) changelist?.Add($"Pattern Matching Accuracy : {         param.Accuracy         } -> {this.Accuracy         }");
            bRtn |= this.InterpolationMode   != param.InterpolationMode   ;
            if (this.InterpolationMode != param.InterpolationMode)
            {
                if(this.InterpolationMode > -1 && this.InterpolationMode < (int)EN_INTERPOLATION.MemberCount && param.InterpolationMode > -1 && param.InterpolationMode < (int)EN_INTERPOLATION.MemberCount)
                    changelist?.Add($"Pattern Matching InterpolationMode : {(EN_INTERPOLATION)param.InterpolationMode} -> {(EN_INTERPOLATION)this.InterpolationMode}");
                else
                    changelist?.Add($"Pattern Matching InterpolationMode : {param.InterpolationMode} -> {this.InterpolationMode}");
            }
            bRtn |= this.SearchOffsetX       != param.SearchOffsetX       ; if (this.SearchOffsetX       != param.SearchOffsetX    ) changelist?.Add($"Pattern Matching SearchOffsetX : {    param.SearchOffsetX    } -> {this.SearchOffsetX    }");
            bRtn |= this.SearchOffsetY       != param.SearchOffsetY       ; if (this.SearchOffsetY       != param.SearchOffsetY    ) changelist?.Add($"Pattern Matching SearchOffsetY : {    param.SearchOffsetY    } -> {this.SearchOffsetY    }");
            bRtn |= this.SearchSizeX         != param.SearchSizeX         ; if (this.SearchSizeX         != param.SearchSizeX      ) changelist?.Add($"Pattern Matching SearchSizeX : {      param.SearchSizeX      } -> {this.SearchSizeX      }");
            bRtn |= this.SearchSizeY         != param.SearchSizeY         ; if (this.SearchSizeY         != param.SearchSizeY      ) changelist?.Add($"Pattern Matching SearchSizeY : {      param.SearchSizeY      } -> {this.SearchSizeY      }");
            return bRtn;
        }
    }
    //---------------------------------------------------------------------------
    public struct SystemRecipeVision
    {
        public string         CurrentRecipeName          ;
        public int            CurrentRecipeIndex         ;
        public string         ImageSavePath              ;
        // [Setting]                                     
        public double         ResolutionX                ;
        public double         ResolutionY                ;
        public double         SpindleOffsetX             ;
        public double         SpindleOffsetY             ;
        public double         TiltRotateCenterFromSample ;
        public double         CamWidth                   ;
        public double         CamHeight                  ;
        public string         LightPort                  ;
        public int            LightBaudRate              ;
        //---------------------------------------------------------------------------
        // Tool Align
        public double         RefToolX                   ;
        public double         RefToolY                   ;
        public double         CentertoToolRelationX      ; // Pin기준 X방향 거리 계수.
        public double         CentertoToolRelationY      ;
        public double         TooltoToolDistanceX        ; // 툴간 거리 설계치 (mm단위).
        public double         TooltoToolDistanceY        ;
        public double         PinRadiusPixel             ;
        public double         PinSmooth                  ;
        public int            ColToolCount               ;
        public int            RowToolCount               ;
        public double         ColOffsetX                 ;
        public double         ColOffsetY                 ;
        public double         RowOffsetX                 ;
        public double         RowOffsetY                 ;
        public int            ToolLightIR                ;
        public int            ToolLightWhite             ;
        public double         AllowAngle                 ;
        public double         InpositionPixelX           ;
        public double         InpositionPixelY           ;
        public double         HardwareOffsetX            ;
        public double         HardwareOffsetY            ;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    /**
    @struct	ST_ALIGN_RESULT
    @brief	결과 구조체
    @remark	
     - DLL에서 받을 결과 데이터.
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:19
    */
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ST_ALIGN_RESULT
    {
        public Int64 NumOfFound;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MODELS_MAX_OCCURRENCES)]
        public ST_RESULT[] stResult;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct ST_RESULT
    {
        public double dX;
        public double dY;
        public double dWidth;
        public double dHeight;
        public double dScore;
        public double dAngle;
    };

    [Serializable]
    public struct PointD
    {
        public double x;
        public double y;
        public PointD(double dx = 0.0, double dy = 0.0)
        {
            x = dx;
            y = dy;
        }

        public static PointD operator +(PointD pnt1, PointD pnt2)
        {
            PointD ret = new PointD();
            ret.x = pnt1.x + pnt2.x;
            ret.y = pnt1.y + pnt2.y;

            return ret;
        }
    };
    
    /************************************************************************/
    /* USER ENUM                                                            */
    /************************************************************************/
    public class UserEnumVision
    {
        //---------------------------------------------------------------------------
        //Enum
        //---------------------------------------------------------------------------
        #region [ENUM DEFINE]
        public enum EN_ALGORITHM
        {
            algModel = 0,
            algPattern
        };

        public enum EN_RECIPE_MODE 
        { 
            Models = 0, 
            Pattern, 
            Milling 
        }

        public enum EN_VISION_MODE
        {
            Polishing = 0,
            Loading,
            ToolStorage,
            Inspection,
            Reserve1,
            Reserve2,
            Reserve3,
            Reserve4,
            MemberCount
        }

        public enum EN_TOOLSTORAGE_POS
        {
            LeftBottom = 0,
            RightBottom,
            RightTop,
            LeftTop,
            MemberCount
        }

        public enum EN_UTILTYPE
        {
            Slurry1 = 0,
            Slurry2,
            Slurry3,
            Soap,
            DIWater,
            MemberCount
        }

        public enum EN_STARTPOS
        {
            LeftBottom  = 0,
            RightBottom,
            LeftTop    ,
            RightTop,
            MemberCount
        }

        public enum EN_DETAILLEVEL
        {
            MEDIUM = 0,
            HIGH,
            VERYHIGH,
            MemberCount
        }

        public enum EN_INTERPOLATION
        {
            BICUBIC = 0,
            BILINEAR,
            NEAREST_NEIGHBOR,
            MemberCount
        }

        public enum EN_MESSAGETYPE
        {
            MESSAGE = 0,
            WARNNING,
            ERROR
        }

        public enum EN_ALIGNSTEP
        {
            ThetaAlign = 0,
            XYAlign
        }
        #endregion
    }

    //---------------------------------------------------------------------------
    public class UserConstVision
    {
        // Recipe Path
        public const string STRRECIPEPATH = "RECIPE\\";
        public const string STRVISIONEPATH = "System\\";
        // Matrox Imaging Library Max Models Number;
        public const int MODELS_MAX_OCCURRENCES = 16;

        public const int RECIPE_MAX_MODEL_COUNT = 10;

        public const double ONEGRAM_TO_NEWTON = 0.00980665;

        public const double DEFAULT_ANGLEMARGIN = 45.0; // Degree
        public const double DEFAULT_SCALEMARGIN = 10.0; // Percent

        public const double DEFAULT_EXPOSURE = 10000;
        public const double DEFAULT_GAIN     = 1;

        public const double DEFAULT_IR       = 120;
        public const double DEFAULT_WHITE    = 0;
    }

    //---------------------------------------------------------------------------
    public class UserClass
    {
        public static VisionManager g_VisionManager = new VisionManager();
    }
}
