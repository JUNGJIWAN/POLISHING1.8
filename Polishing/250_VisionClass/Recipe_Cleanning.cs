using System;
using System.Windows;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserConstVision;

namespace WaferPolishingSystem.Vision
{
    /**
    @class	class Recipe_Cleaning : IPropertyChanged
    @brief	클리닝 레시피 Binding 클래스.
    @remark	
     - 
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/10/6  12:12
    */
    class Recipe_Cleaning : IPropertyChanged
    {
        const int nInterlockX = 16;
        const int nInterlockY = 16;
        private double dXOffset              = 0.0;
        public double XOffset
        {
            get { return dXOffset; }
            set
            {
                dXOffset = value;
                Offset = new Thickness(-1 * dXOffset, -1 * dYOffset, 0, 0);
                OnPropertyChanged("XOffset");
            }
        }

        private double dYOffset              = 0.0;
        public double YOffset
        {
            get { return dYOffset; }
            set
            {
                dYOffset = value;
                Offset = new Thickness(-1 * dXOffset, -1 * dYOffset, 0, 0);
                OnPropertyChanged("YOffset");
            }
        }

        private double dXSpeed               = 0.0;
        public double XSpeed
        {
            get { return dXSpeed; }
            set
            {
                dXSpeed = value;
                OnPropertyChanged("XSpeed");
            }
        }

        private double dYSpeed               = 0.0;
        public double YSpeed
        {
            get { return dYSpeed; }
            set
            {
                dYSpeed = value;
                OnPropertyChanged("YSpeed");
            }
        }

        private double dXDistance            = 0.0;
        public double XDistance
        {
            get { return dXDistance; }
            set
            {
                dXDistance = value;
                if (dXDistance > nInterlockX)
                    dXDistance = nInterlockX;
                ClrWidthTool = dXDistance;
                OnPropertyChanged("XDistance");
            }
        }

        private double dYPitch               = 0.0;
        public double YPitch
        {
            get { return dYPitch; }
            set
            {
                dYPitch = value;
                if (nPathCount * dYPitch > 16)
                    dYPitch = nInterlockY / (double)nPathCount;

                YDistance = nPathCount * dYPitch;
                OnPropertyChanged("YPitch");
            }
        }

        private int    nPathCount            = 0;
        public int PathCount
        {
            get { return nPathCount; }
            set
            {
                nPathCount = value;
                if (nPathCount * dYPitch > 16)
                    nPathCount = (int)(nInterlockY / dYPitch);

                YDistance = nPathCount * dYPitch;
                OnPropertyChanged("PathCount");
            }
        }

       

        private double dSpindleRPM           = 0.0;
        public double SpindleRPM
        {
            get { return dSpindleRPM; }
            set
            {
                dSpindleRPM = value;
                OnPropertyChanged("SpindleRPM");
            }
        }

        private double dPreWashingRPM        = 0.0;
        public double PreWashingRPM
        {
            get { return dPreWashingRPM; }
            set
            {
                dPreWashingRPM = value;
                OnPropertyChanged("PreWashingRPM");
            }
        }

        private double dDehydrationRPM = 0.0;
        public double DehydrationRPM
        {
            get { return dDehydrationRPM; }
            set
            {
                dDehydrationRPM = value;
                OnPropertyChanged("DehydrationRPM");
            }
        }

        private double dPreWashingTime = 0.0;
        public double PreWashingTime
        {
            get { return dPreWashingTime; }
            set
            {
                dPreWashingTime = value;
                OnPropertyChanged("PreWashingTime");
            }
        }

        private double dDehydrationTime = 0.0;
        public double DehydrationTime
        {
            get { return dDehydrationTime; }
            set
            {
                dDehydrationTime = value;
                OnPropertyChanged("DehydrationTime");
            }
        }

        private double dForce                = 0.0;
        public double Force
        {
            get { return dForce; }
            set
            {
                dForce = value;
                OnPropertyChanged("Force");
            }
        }

        //---------------------------------------------------------------------------
        // Sample  Drawing 
        //---------------------------------------------------------------------------
        private double dSampleWidth = 0.0;
        public double SampleWidth
        {
            get { return dSampleWidth; }
            set
            {
                dSampleWidth = value;
                OnPropertyChanged("SampleWidth");
            }
        }
        private double dSampleHeight = 0.0;
        public double SampleHeight
        {
            get { return dSampleHeight; }
            set
            {
                dSampleHeight = value;
                OnPropertyChanged("SampleHeight");
            }
        }

        private Thickness dOffset = new Thickness();
        public Thickness Offset
        {
            get { return dOffset; }
            set
            {
                dOffset = value;
                if (dXDistance < 16)
                {
                    if ((dOffset.Left + dXDistance / 2.0) > nInterlockX / 2.0)
                    {
                        dOffset.Left = nInterlockX / 2.0 - dXDistance / 2.0;
                        XOffset = dOffset.Left * -1;
                    }

                    if (dOffset.Left + dXDistance / 2.0 - dXDistance < -1 * nInterlockX / 2.0)
                    {
                        dOffset.Left = dXDistance - nInterlockX / 2.0 - dXDistance / 2.0;
                        XOffset = dOffset.Left * -1;
                    }
                }
                if(dYDistance <= 16)
                {
                    if ((dOffset.Top + dYDistance / 2.0) > nInterlockY / 2.0)
                    {
                        dOffset.Top = nInterlockY / 2.0 - dYDistance / 2.0;
                        YOffset = dOffset.Top * -1;
                    }
                    if (dOffset.Top + dYDistance / 2.0 - dYDistance < -1 * nInterlockY / 2.0)
                    {
                        dOffset.Top = dYDistance - nInterlockY / 2.0 - dYDistance / 2.0;
                        YOffset = dOffset.Top * -1;
                    }
                }
                
                OnPropertyChanged("Offset");
            }
        }

        private double dYDistance = 0;
        public double YDistance
        {
            get { return dYDistance; }
            set
            {
                dYDistance = value;
                ClrHeightTool = dYDistance;
                OnPropertyChanged("YDistance");
            }
        }

        private double dClrWidth = 0;
        public double ClrWidth
        {
            get { return dClrWidth; }
            set
            {
                dClrWidth = value;
                
                OnPropertyChanged("ClrWidth");
            }
        }

        private double dClrHeight = 0;
        public double ClrHeight
        {
            get { return dClrHeight; }
            set
            {
                dClrHeight = value;

                
                OnPropertyChanged("ClrHeight");
            }
        }

        private double dClrWidthTool = 0;
        public double ClrWidthTool
        {
            get { return dClrWidthTool; }
            set
            {
                dClrWidthTool = value;

                if (dClrWidthTool <= nInterlockX)
                {
                    if ((dOffset.Left + dClrWidthTool / 2.0) > nInterlockX / 2.0)
                    {
                        dClrWidthTool = -1 * (dOffset.Left - nInterlockX / 2.0) * 2;
                        XDistance = dClrWidthTool;
                    }

                    if (dOffset.Left - dClrWidthTool / 2.0 < -1 * nInterlockX / 2.0)
                    {
                        dClrWidthTool = (dOffset.Left + nInterlockX / 2.0) * 2;
                        XDistance = dClrWidthTool;
                    }
                }

                ClrWidth = dClrWidthTool + 12;
                OnPropertyChanged("ClrWidthTool");
            }
        }

        private double dClrHeightTool = 0;
        public double ClrHeightTool
        {
            get { return dClrHeightTool; }
            set
            {
                dClrHeightTool = value;

                if (dClrHeightTool <= nInterlockY)
                {
                    if ((dOffset.Top + dClrHeightTool / 2.0) > nInterlockY / 2.0)
                    {
                        dClrHeightTool = -1 * (dOffset.Top - nInterlockY / 2.0) * 2;
                        YDistance = dClrHeightTool;
                        PathCount = (int)(dClrHeightTool / dYPitch);
                    }

                    if (dOffset.Top - dClrHeightTool / 2.0 < -1 * nInterlockY / 2.0)
                    {
                        dClrHeightTool = (dOffset.Top + nInterlockY / 2.0) * 2;
                        YDistance = dClrHeightTool;
                        PathCount = (int)(dClrHeightTool / dYPitch);
                    }
                }

                ClrHeight = dClrHeightTool + 12;
                OnPropertyChanged("ClrHeightTool");
            }
        }
        //---------------------------------------------------------------------------

        public void GetDataFromRecipe(ST_Cleaning Cleaning)
        {
            XOffset         = Cleaning.XOffset        ;
            YOffset         = Cleaning.YOffset        ;
            XSpeed          = Cleaning.XSpeed         ;
            YSpeed          = Cleaning.YSpeed         ;
            XDistance       = Cleaning.XDistance      ;
            YPitch          = Cleaning.YPitch         ;
            PathCount       = Cleaning.PathCount      ;
            SpindleRPM      = Cleaning.SpindleRPM     ;
            PreWashingRPM   = Cleaning.PreWashingRPM  ;
            DehydrationRPM  = Cleaning.DehydrationRPM ;
            PreWashingTime  = Cleaning.PreWashingTime;
            DehydrationTime = Cleaning.DehydrationTime;
            Force           = Math.Round(Cleaning.Force * ONEGRAM_TO_NEWTON, 3);
        }

        public void SetDataToRecipe(ref ST_Cleaning Cleaning)
        {
            Cleaning.XOffset         = XOffset        ;
            Cleaning.YOffset         = YOffset        ;
            Cleaning.XSpeed          = XSpeed         ;
            Cleaning.YSpeed          = YSpeed         ;
            Cleaning.XDistance       = XDistance      ;
            Cleaning.YPitch          = YPitch         ;
            Cleaning.PathCount       = PathCount      ;
            Cleaning.SpindleRPM      = SpindleRPM     ;
            Cleaning.PreWashingRPM   = PreWashingRPM  ;
            Cleaning.DehydrationRPM  = DehydrationRPM ;
            Cleaning.PreWashingTime  = PreWashingTime;
            Cleaning.DehydrationTime = DehydrationTime;
            Cleaning.Force           = Force / ONEGRAM_TO_NEWTON;
        }
    }
}
