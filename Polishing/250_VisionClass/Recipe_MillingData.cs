using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserConstVision;

namespace WaferPolishingSystem.Vision
{
    /**
    @class	IPropertyChanged : INotifyPropertyChanged
    @brief	데이터 Binding을 위한 클래스.
    @remark	
     - 
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/10/6  12:13
    */
    public class IPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /**
    @class	Recipe_MillingData : IPropertyChanged
    @brief	밀링 레시피 Binding 클래스.
    @remark	
     - 
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/10/6  12:13
    */
    class Recipe_MillingData : IPropertyChanged
    {
        private bool bEnable = false;
        public bool Enable
        {
            get { return bEnable; }
            set
            {
                bEnable = value;
                OnPropertyChanged("Enable");
            }
        }

        private string strName = "";
        public string Name
        {
            get { return strName; }
            set
            {
                strName = value;
                OnPropertyChanged("Name");
            }
        }

        private double dX = 0.0;
        public double X
        {
            get { return dX; }
            set
            {
                dX = value;
                OnPropertyChanged("X");
            }
        }

        private double dY = 0.0;
        public double Y
        {
            get { return dY; }
            set
            {
                dY = value;
                OnPropertyChanged("Y");
            }
        }

        private double dWidth = 0.0;
        public double Width
        {
            get { return dWidth; }
            set
            {
                dWidth = value;
                OnPropertyChanged("Width");
            }
        }

        private double dHeight = 0.0;
        public double Height
        {
            get { return dHeight; }
            set
            {
                dHeight = value;
                OnPropertyChanged("Height");
            }
        }

        private double dPitch = 0.0;
        public double Pitch
        {
            get { return dPitch; }
            set
            {
                dPitch = value;
                OnPropertyChanged("Pitch");
            }
        }

        private int nCycle = 1;
        public int Cycle
        {
            get { return nCycle; }
            set
            {
                nCycle = value;
                if (nCycle < 1)
                    nCycle = 1;
                OnPropertyChanged("Cycle");
            }
        }

        private int nStartPos = 0;
        public int StartPos
        {
            get { return nStartPos; }
            set
            {
                nStartPos = value;
                OnPropertyChanged("StartPos");
            }
        }

        private double dForce = 0.0;
        public double Force
        {
            get { return dForce; }
            set
            {
                dForce = value;
                OnPropertyChanged("Force");
            }
        }

        private double dTilt = 0.0;
        public double Tilt
        {
            get { return dTilt; }
            set
            {
                dTilt = value;
                OnPropertyChanged("Tilt");
            }
        }

        private double dRpm = 0.0;
        public double Rpm
        {
            get { return dRpm; }
            set
            {
                dRpm = value;
                OnPropertyChanged("Rpm");
            }
        }

        private double dSpeed = 0.0;
        public double Speed
        {
            get { return dSpeed; }
            set
            {
                dSpeed = value;
                OnPropertyChanged("Speed");
            }
        }

        private bool bToolChange = false;
        public bool ToolChange
        {
            get { return bToolChange; }
            set
            {
                bToolChange = value;
                OnPropertyChanged("ToolChange");
            }
        }

        private bool bUtilFill = false;
        public bool UtilFill
        {
            get { return bUtilFill; }
            set
            {
                bUtilFill = value;
                OnPropertyChanged("UtilFill");
            }
        }

        private bool bUtilDrain = false;
        public bool UtilDrain
        {
            get { return bUtilDrain; }
            set
            {
                bUtilDrain = value;
                OnPropertyChanged("UtilDrain");
            }
        }

        private int nUtilType = 0;
        public int UtilType
        {
            get { return nUtilType; }
            set
            {
                nUtilType = value;
                OnPropertyChanged("UtilType");
            }
        }

        private bool bMillingImage = false;
        public bool MillingImage
        {
            get { return bMillingImage; }
            set
            {
                bMillingImage = value;
                OnPropertyChanged("MillingImage");
            }
        }

        private bool bEPD = false;
        public bool EPD
        {
            get { return bEPD; }
            set
            {
                bEPD = value;
                OnPropertyChanged("EPD");
            }
        }

        private int nPathCount = 0;
        public int PathCount
        {
            get { return nPathCount; }
            set
            {
                nPathCount = value;
                OnPropertyChanged("PathCount");
            }
        }

        private int nToolType = 0;
        public int ToolType
        {
            get { return nToolType; }
            set
            {
                nToolType = value;
                OnPropertyChanged("ToolType");
            }
        }

        public void GetDataFromRecipe(MillingList milList)
        {
            try
            {
                Enable          = Convert.ToBoolean(milList.Enable);
                X               = milList.MilRect.X * (g_VisionManager._RecipeVision.ResolutionX / 1000.0);
                Y               = milList.MilRect.Y * (g_VisionManager._RecipeVision.ResolutionY / 1000.0);
                Width           = milList.MilRect.Width * (g_VisionManager._RecipeVision.ResolutionX / 1000.0);
                Height          = milList.MilRect.Height * (g_VisionManager._RecipeVision.ResolutionY / 1000.0);
                PathCount       = milList.PathCount;
                Pitch           = milList.Pitch;
                Cycle           = milList.Cycle;
                Force           = Math.Round(milList.Force * ONEGRAM_TO_NEWTON, 3);
                StartPos        = milList.StartPos;
                Tilt            = milList.Tilt;
                Rpm             = milList.RPM;
                Speed           = milList.Speed;
                ToolChange      = Convert.ToBoolean(milList.ToolChange);
                UtilFill        = Convert.ToBoolean(milList.UtilFill);
                UtilDrain       = Convert.ToBoolean(milList.UtilDrain);
                UtilType        = milList.UtilType;
                MillingImage    = Convert.ToBoolean(milList.MillingImage);
                EPD             = Convert.ToBoolean(milList.EPD);
                ToolType        = milList.ToolType;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool Compare(MillingList milList, List<string> changelist = null)
        {
            bool bRtn = false;
            bRtn |= (Enable         != Convert.ToBoolean(milList.Enable));                                                           if (Enable != Convert.ToBoolean(milList.Enable))                        changelist?.Add($"     Milling Enable : {Convert.ToBoolean(milList.Enable)} -> {Enable}");
            // 소수 2째 자리 까지 비교 할 것. (연산 Scale 차이 고려) - 소수점 차이 날 수 있음.
            bRtn |= Math.Round(X      / (g_VisionManager._RecipeVision.ResolutionX / 1000.0), 3)      != (milList.MilRect.X       ); if (Math.Round(X      / (g_VisionManager._RecipeVision.ResolutionX / 1000.0), 3) != (milList.MilRect.X       )) changelist?.Add($"     MilRect.X : {milList.MilRect.X       } -> {Math.Round(X      / (g_VisionManager._RecipeVision.ResolutionX / 1000.0), 3)}");
            bRtn |= Math.Round(Width  / (g_VisionManager._RecipeVision.ResolutionX / 1000.0), 3)      != (milList.MilRect.Width   ); if (Math.Round(Width  / (g_VisionManager._RecipeVision.ResolutionX / 1000.0), 3) != (milList.MilRect.Width   )) changelist?.Add($"     MilRect.W : {milList.MilRect.Width   } -> {Math.Round(Width  / (g_VisionManager._RecipeVision.ResolutionX / 1000.0), 3)}");
            bRtn |= Math.Round(Y      / (g_VisionManager._RecipeVision.ResolutionY / 1000.0), 3)      != (milList.MilRect.Y       ); if (Math.Round(Y      / (g_VisionManager._RecipeVision.ResolutionY / 1000.0), 3) != (milList.MilRect.Y       )) changelist?.Add($"     MilRect.Y : {milList.MilRect.Y       } -> {Math.Round(Y      / (g_VisionManager._RecipeVision.ResolutionY / 1000.0), 3)}");
            bRtn |= Math.Round(Height / (g_VisionManager._RecipeVision.ResolutionY / 1000.0), 3)      != (milList.MilRect.Height  ); if (Math.Round(Height / (g_VisionManager._RecipeVision.ResolutionY / 1000.0), 3) != (milList.MilRect.Height  )) changelist?.Add($"     MilRect.H : {milList.MilRect.Height  } -> {Math.Round(Height / (g_VisionManager._RecipeVision.ResolutionY / 1000.0), 3)}");
            bRtn |= (PathCount      != milList.PathCount)                               ;                                            if (PathCount      != milList.PathCount)                                changelist?.Add($"      PathCount : {      milList.PathCount                               } -> {PathCount      }");
            bRtn |= (Pitch          != milList.Pitch)                                   ;                                            if (Pitch          != milList.Pitch)                                    changelist?.Add($"      Pitch : {          milList.Pitch                                   } -> {Pitch          }");
            bRtn |= (Cycle          != milList.Cycle)                                   ;                                            if (Cycle          != milList.Cycle)                                    changelist?.Add($"      Cycle : {          milList.Cycle                                   } -> {Cycle          }");
            bRtn |= (Force          != Math.Round(milList.Force * ONEGRAM_TO_NEWTON, 3));                                            if (Force          != Math.Round(milList.Force * ONEGRAM_TO_NEWTON, 3)) changelist?.Add($"      Force : {          Math.Round(milList.Force * ONEGRAM_TO_NEWTON, 3)} -> {Force          }");
            bRtn |= (StartPos       != milList.StartPos)                                ;
            if (StartPos != milList.StartPos)
            {
                if(StartPos > -1 && StartPos < (int)UserEnumVision.EN_STARTPOS.MemberCount && milList.StartPos > -1 && milList.StartPos < (int)UserEnumVision.EN_STARTPOS.MemberCount)
                    changelist?.Add($"      StartPos : {(UserEnumVision.EN_STARTPOS)milList.StartPos} -> {(UserEnumVision.EN_STARTPOS)StartPos}");
                else
                    changelist?.Add($"      StartPos : {milList.StartPos} -> {StartPos}");
            }
            bRtn |= (Tilt           != milList.Tilt)                                    ;                                            if (Tilt           != milList.Tilt)                                     changelist?.Add($"      Tilt : {           milList.Tilt                                    } -> {Tilt           }");
            bRtn |= (Rpm            != milList.RPM)                                     ;                                            if (Rpm            != milList.RPM)                                      changelist?.Add($"      Rpm : {            milList.RPM                                     } -> {Rpm            }");
            bRtn |= (Speed          != milList.Speed)                                   ;                                            if (Speed          != milList.Speed)                                    changelist?.Add($"      Speed : {          milList.Speed                                   } -> {Speed          }");
            bRtn |= (ToolChange     != Convert.ToBoolean(milList.ToolChange))           ;                                            if (ToolChange     != Convert.ToBoolean(milList.ToolChange))            changelist?.Add($"      ToolChange : {     Convert.ToBoolean(milList.ToolChange)           } -> {ToolChange     }");
            bRtn |= (UtilFill       != Convert.ToBoolean(milList.UtilFill))             ;                                            if (UtilFill       != Convert.ToBoolean(milList.UtilFill))              changelist?.Add($"      UtilFill : {       Convert.ToBoolean(milList.UtilFill)             } -> {UtilFill       }");
            bRtn |= (UtilDrain      != Convert.ToBoolean(milList.UtilDrain))            ;                                            if (UtilDrain      != Convert.ToBoolean(milList.UtilDrain))             changelist?.Add($"      UtilDrain : {      Convert.ToBoolean(milList.UtilDrain)            } -> {UtilDrain      }");
            bRtn |= (UtilType       != milList.UtilType)                                ;
            if (UtilType != milList.UtilType)
            {
                if(UtilType > -1 && UtilType < (int)UserEnumVision.EN_UTILTYPE.MemberCount && milList.UtilType > -1 && milList.UtilType < (int)UserEnumVision.EN_UTILTYPE.MemberCount)
                    changelist?.Add($"      UtilType : {(UserEnumVision.EN_UTILTYPE)milList.UtilType} -> {(UserEnumVision.EN_UTILTYPE)UtilType}");
                else
                    changelist?.Add($"      UtilType : {milList.UtilType} -> {UtilType}");
            }
            bRtn |= (MillingImage   != Convert.ToBoolean(milList.MillingImage))         ;                                            if (MillingImage   != Convert.ToBoolean(milList.MillingImage))          changelist?.Add($"      MillingImage : {   Convert.ToBoolean(milList.MillingImage)         } -> {MillingImage   }");
            bRtn |= (EPD            != Convert.ToBoolean(milList.EPD))                  ;                                            if (EPD            != Convert.ToBoolean(milList.EPD))                   changelist?.Add($"      EPD : {            Convert.ToBoolean(milList.EPD)                  } -> {EPD            }");
            bRtn |= (ToolType       != milList.ToolType)                                ;
            if (ToolType != milList.ToolType)
            {
                if(ToolType > -1 && ToolType < FormMain.FM.m_stSystemOpt.sToolType.Length && milList.ToolType > -1 && milList.ToolType < FormMain.FM.m_stSystemOpt.sToolType.Length)
                    changelist?.Add($"      ToolType : {FormMain.FM.m_stSystemOpt.sToolType[milList.ToolType]} -> {FormMain.FM.m_stSystemOpt.sToolType[ToolType]}");
                else
                    changelist?.Add($"      ToolType : {milList.ToolType} -> {ToolType}");
            }
            return bRtn;
        }
    }
}
