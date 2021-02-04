using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserConstVision;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFunctionVision;
using static UserInterface.AlignControl;
using UserInterface;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Controls.DataVisualization.Charting;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using WaferPolishingSystem.Vision;
using System.IO;

namespace WaferPolishingSystem.Form
{
    public class HistogramData : IPropertyChanged
    {
        private double x = 0.0;
        public double X 
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }

        private double y = 0.0;
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }
    }
    /// <summary>
    /// PageRecipe_Vision.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageRecipe_InspectionVision : Page
    {
        public DelegateParamDouble delThreshold = null;
        public del_EndModify delEndModfy = null;
        DataTable _dtROI = new DataTable();
        
        //string m_strRecipePath = STRRECIPEPATH;
        public int _SelectedModelIndex = -1;
        public string _SelectedModelName = "";
        public string _SelectedRecipe = "";
        public Frame _frame_Main;
        public ModelList _ModelRecipe = new ModelList();

        public int[] GlobalLightIR;
        public int[] GlobalLightIRFilter;
        public int[] GlobalLightWhite;
        public int[] GlobalCameraExposure;
        public int[] GlobalCameraGain;

        ObservableCollection<HistogramData> obData = new ObservableCollection<HistogramData>();
        public PageRecipe_InspectionVision()
        {
            InitializeComponent();
            ComponentDispatcher.ThreadIdle += UIInit;
        }
        private void UIInit(object sender, EventArgs args)
        {
            ComponentDispatcher.ThreadIdle -= UIInit;
            this.Background = G_COLOR_PAGEBACK;
            ac_Align.delUpdateRect += fn_UpdateMilling;

            _dtROI.Columns.Add("NAME");
            _dtROI.Columns.Add("X");
            _dtROI.Columns.Add("Y");
            _dtROI.Columns.Add("WIDTH");
            _dtROI.Columns.Add("HEIGHT");

            _dtROI.Rows.Add("Search ROI", "0", "0", "0", "0");
            //g_VisionManager._CamManager.fn_AddEvent(ac_Align.OnImageGrabed);
            g_VisionManager._CamManager.update += ac_Align.SetImage;
            uis_Exposuretime.lb_ValueName.Content = "Exposure Time :";
            uis_Exposuretime.slider.Minimum = 1000;
            uis_Exposuretime.slider.Maximum = 20000;
            uis_Exposuretime.slider.TickFrequency = 100;
            uis_Gain.lb_ValueName.Content = "Gain :";
            uis_Gain.slider.Minimum = 1;
            uis_Gain.slider.TickFrequency = 100;
            uis_Gain.slider.Maximum = 10000;

            ug_ROI.DataContext = _dtROI.DefaultView;

            ((LineSeries)Histo.Series[0]).ItemsSource = obData;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ac_Align.SearchLock = false;
                lb_Title.Text = _SelectedModelName;
                lb_Recipe.Text = _SelectedRecipe;
                if (g_VisionManager._CamManager._paramExposureRaw != null)
                    uis_Exposuretime.Parameter = g_VisionManager._CamManager._paramExposureRaw;
                if (g_VisionManager._CamManager._paramGainRaw != null)
                    uis_Gain.Parameter = g_VisionManager._CamManager._paramGainRaw;
                fn_LoadData();
                ac_Align.fn_SetResolution(g_VisionManager._RecipeVision.ResolutionX, g_VisionManager._RecipeVision.ResolutionY);
                // Run 일 때 막을것.
                if(!SEQ._bAuto)
                    bn_OneShot_Click(bn_OneShot, null);

                BitmapImage bmpImg = new BitmapImage();

                FileStream source = File.OpenRead(_ModelRecipe.strImgPath);

                bmpImg.BeginInit();
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = source;
                bmpImg.EndInit();

                WriteableBitmap wbm = new WriteableBitmap(bmpImg);
                CroppedBitmap cb = new CroppedBitmap(
                    wbm,
                    new Int32Rect((int)_ModelRecipe.Model.X, (int)_ModelRecipe.Model.Y,
                    (int)_ModelRecipe.Model.Width, (int)_ModelRecipe.Model.Height));

                WriteableBitmap wb = new WriteableBitmap(cb);
                ac_RefImage.SetImage(wb);
                ac_RefImage.SetFitScale();

                fn_SetButton(EnRoiMode.ModeMove);
                fn_SetRuler(false);

                bn_OneShot.IsEnabled = true;
                bn_Process.IsEnabled = true;
                bn_Inspection.IsEnabled = true;

                if (SEQ._bRun)
                {
                    bn_OneShot.IsEnabled = false;
                    bn_Live.IsEnabled = false;
                }
                else
                {
                    bn_OneShot.IsEnabled = true;
                    bn_Live.IsEnabled = true;
                }
            }
            catch(Exception ex)
            {
                ac_RefImage.ClearCanvas();
                Console.WriteLine(ex.Message);
            }
        }

        private void fn_LoadData()
        {
            try
            {
                Rect[] rect = new Rect[2];
                for (int i = 0; i < 2; i++)
                {
                    rect[i] = new Rect();
                }
                rect[0] = _ModelRecipe.Model;
                rect[1] = new Rect(_ModelRecipe.Inspection.ROIX, _ModelRecipe.Inspection.ROIY, _ModelRecipe.Inspection.ROIW, _ModelRecipe.Inspection.ROIH);

                _dtROI.Rows[0]["X"] = rect[1].X.ToString("0.00");
                _dtROI.Rows[0]["Y"] = rect[1].Y.ToString("0.00");
                _dtROI.Rows[0]["Width"] = rect[1].Width.ToString("0.00");
                _dtROI.Rows[0]["Height"] = rect[1].Height.ToString("0.00");

                if (_ModelRecipe.UseGlobalInspection == 1)
                {
                    gb_CamParam.Background = Brushes.Transparent;
                    gb_Light   .Background = Brushes.Transparent;
                    gb_CamParam.Header     = "Camera Parameter (Global)";
                    gb_Light   .Header     = "Light (Global)";
                    us_LightIR.USValue            = GlobalLightIR       [(int)EN_VISION_MODE.Inspection];
                    us_LightW.USValue             = GlobalLightWhite    [(int)EN_VISION_MODE.Inspection];
                    tg_shutter.Button.IsChecked   = GlobalLightIRFilter [(int)EN_VISION_MODE.Inspection] == 1 ? true : false;
                    uis_Exposuretime.slider.Value = GlobalCameraExposure[(int)EN_VISION_MODE.Inspection];
                    uis_Gain.slider.Value         = GlobalCameraGain    [(int)EN_VISION_MODE.Inspection];
                }
                else
                {
                    gb_CamParam.Background = G_COLOR_SUBMENU;
                    gb_Light   .Background = G_COLOR_SUBMENU;
                    gb_CamParam.Header     = "Camera Parameter (Private)";
                    gb_Light   .Header     = "Light (Private)";
                    us_LightIR.USValue            = _ModelRecipe.InspectionLightIR;
                    us_LightW.USValue             = _ModelRecipe.InspectionLightWhite;
                    tg_shutter.Button.IsChecked   = _ModelRecipe.InspectionLightIRFilter == 1 ? true : false;
                    uis_Exposuretime.slider.Value = _ModelRecipe.InspectionCameraExposureTime;
                    uis_Gain.slider.Value         = _ModelRecipe.InspectionCameraGain;
                }

                us_Smooth    .USValue = _ModelRecipe.Inspection.Sigma;
                //us_Threshold .USValue = _ModelRecipe.Inspection.Threshold;
                us_LowThresh .USValue = _ModelRecipe.Inspection.LowThreshold;
                us_HighThresh.USValue = _ModelRecipe.Inspection.HighThreshold;
                up_RefDis1   .UPValue = _ModelRecipe.Inspection.RefDistance.ToString();
                up_RefDis2   .UPValue = _ModelRecipe.Inspection.RefDistance2.ToString();

                cb_Condition .SelectedIndex = _ModelRecipe.Inspection.Condition;
                cb_Section1  .SelectedIndex = _ModelRecipe.Inspection.Section1;
                cb_Section2  .SelectedIndex = _ModelRecipe.Inspection.Section2;

                ac_Align.SetRectangle(rect);

                ac_Align.SelectObject(EnObjectSelect.SelModel + 1);
                ac_Align.ViewContent((int)(EnObjectSelect.SelModel + 1), true);
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        private void bn_Ruler_Click(object sender, RoutedEventArgs e)
        {

            if (bn_Ruler.Background == G_COLOR_BTNNORMAL)
                fn_SetRuler(true);
            else
                fn_SetRuler(false);
        }

        private void fn_SetRuler(bool bRuler)
        {
            if (bRuler)
                bn_Ruler.Background = G_COLOR_BTNCLICKED;
            else
                bn_Ruler.Background = G_COLOR_BTNNORMAL;
            ac_Align.RulerEnable(bRuler);
        }

        private void bn_Draw_Click(object sender, RoutedEventArgs e)
        {
            fn_SetButton(EnRoiMode.ModeDraw);
        }

        private void bn_Move_Click(object sender, RoutedEventArgs e)
        {
            fn_SetButton(EnRoiMode.ModeMove);
        }

        private void bn_Theta_Click(object sender, RoutedEventArgs e)
        {
            fn_SetButton(EnRoiMode.ModeTheta);
        }

        private void fn_SetButton(EnRoiMode mode)
        {
            bn_Move.Background = G_COLOR_BTNNORMAL;
            bn_Draw.Background = G_COLOR_BTNNORMAL;

            ac_Align.SetMove(mode);
            switch (mode)
            {
                case EnRoiMode.ModeMove:
                    bn_Move.Background = G_COLOR_BTNCLICKED;
                    break;
                case EnRoiMode.ModeDraw:
                    bn_Draw.Background = G_COLOR_BTNCLICKED;
                    break;
                default:
                    break;
            }
        }

        private void bn_OneShot_Click(object sender, RoutedEventArgs e)
        {
            fn_SetOpticCondition();
            // Grab Image.
            g_VisionManager._CamManager.fn_GrabStart(0);
        }

        private void fn_UpdateMilling(int idx, Rect rect)
        {
            try
            {
                if (idx < 2)
                {
                    _dtROI.Rows[0]["X"] = rect.X.ToString("0.00");
                    _dtROI.Rows[0]["Y"] = rect.Y.ToString("0.00");
                    _dtROI.Rows[0]["Width"] = rect.Width.ToString("0.00");
                    _dtROI.Rows[0]["Height"] = rect.Height.ToString("0.00");
                }
            }
            catch (Exception ex)
            {
                //fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        public void fn_Update()
        {
            try
            {
                double dValue;

                double.TryParse((string)_dtROI.Rows[0]["X"], out dValue);
                _ModelRecipe.Inspection.ROIX = dValue;
                double.TryParse((string)_dtROI.Rows[0]["Y"], out dValue);
                _ModelRecipe.Inspection.ROIY = dValue;
                double.TryParse((string)_dtROI.Rows[0]["Width"], out dValue);
                _ModelRecipe.Inspection.ROIW = dValue;
                double.TryParse((string)_dtROI.Rows[0]["Height"], out dValue);
                _ModelRecipe.Inspection.ROIH = dValue;

                //g_VisionManager._RecipeModel.CameraExposureTime[(int)EN_VISION_MODE.Inspection] = (int)uis_Exposuretime.slider.Value;
                //g_VisionManager._RecipeModel.CameraGain[(int)EN_VISION_MODE.Inspection] = (int)uis_Gain.slider.Value;

                _ModelRecipe.Inspection.Sigma           = us_Smooth.USValue;
                //_ModelRecipe.Inspection.Threshold       = us_Threshold.USValue;
                _ModelRecipe.Inspection.LowThreshold    = us_LowThresh.USValue;
                _ModelRecipe.Inspection.HighThreshold   = us_HighThresh.USValue;
                double.TryParse((string)up_RefDis1.UPValue, out dValue);
                _ModelRecipe.Inspection.RefDistance     = dValue;
                double.TryParse((string)up_RefDis2.UPValue, out dValue);
                _ModelRecipe.Inspection.RefDistance2    = dValue;

                _ModelRecipe.Inspection.Condition = cb_Condition.SelectedIndex;
                _ModelRecipe.Inspection.Section1  = cb_Section1 .SelectedIndex;
                _ModelRecipe.Inspection.Section2  = cb_Section2.SelectedIndex;

                if(_ModelRecipe.UseGlobalInspection == 1)
                {
                    GlobalCameraExposure [(int)EN_VISION_MODE.Inspection] = (int)uis_Exposuretime.slider.Value;
                    GlobalCameraGain     [(int)EN_VISION_MODE.Inspection] = (int)uis_Gain        .slider.Value;
                    GlobalLightIRFilter  [(int)EN_VISION_MODE.Inspection] = tg_shutter.Button.IsChecked == true ? 1 : 0;
                    GlobalLightIR        [(int)EN_VISION_MODE.Inspection] = (int)us_LightIR.USValue;
                    GlobalLightWhite     [(int)EN_VISION_MODE.Inspection] = (int)us_LightW .USValue;
                }
                else
                {
                    _ModelRecipe.InspectionCameraExposureTime = (int)uis_Exposuretime.slider.Value;
                    _ModelRecipe.InspectionCameraGain         = (int)uis_Gain        .slider.Value;
                    _ModelRecipe.InspectionLightIRFilter      = tg_shutter.Button.IsChecked == true ? 1 : 0;
                    _ModelRecipe.InspectionLightIR            = (int)us_LightIR.USValue;
                    _ModelRecipe.InspectionLightWhite         = (int)us_LightW .USValue;
                }
            }
            catch (Exception ex)
            {
                //fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        private void bn_Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fdlg = new Microsoft.Win32.OpenFileDialog();
            if (fdlg.ShowDialog() == true)
            {
                ac_Align.OpenImage(fdlg.FileName);
                fn_SetButton(EnRoiMode.ModeDraw);
                ac_Align.SetFitScale();
            }
        }

        private void bn_EndModify_Click(object sender, RoutedEventArgs e)
        {
            if (_frame_Main != null)
                _frame_Main.Content = null;

            fn_Update();
            delEndModfy?.Invoke();
        }

        private void bn_Live_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;

            if (ub != null)
            {
                if (g_VisionManager._CamManager.fn_GetGrabbingState())
                {
                    // Grab Stop
                    g_VisionManager._CamManager.fn_GrabStop();

                    ub.Content = "Live";
                    bn_OneShot.IsEnabled = true;
                    bn_Process.IsEnabled = true;
                    bn_Inspection.IsEnabled = true;

                    // Light Off
                    g_VisionManager.fn_SetLightValue(0);
                }
                else
                {
                    fn_SetOpticCondition();

                    // Grab Image.
                    g_VisionManager._CamManager.fn_GrabStart(1);
                    ub.Content = "Stop";
                    bn_OneShot.IsEnabled = false;
                    bn_Process.IsEnabled = false;
                    bn_Inspection.IsEnabled = false;
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Grab stop
            g_VisionManager._CamManager.fn_GrabStop();

            // Light off.
            g_VisionManager.fn_SetLightValue(0);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void us_LisghtIR_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        @brief	IR 마우스 UP 이벤트.
        @return	void
        @param	object               sender
        @param  MouseButtonEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:06
        */
        private void us_LightIR_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            g_VisionManager._LightManger.Channel = 2;
            g_VisionManager._LightManger.Bright = (int)us_LightIR.USValue;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void us_LightIR_PreviewKeyDown(object sender, KeyEventArgs e)
        @brief	IR Key Down 이벤트.
        @return	void
        @param	object               sender
        @param  KeyEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:06
        */
        private void us_LightIR_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                g_VisionManager._LightManger.Channel = 2;
                g_VisionManager._LightManger.Bright = (int)us_LightIR.USValue;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void us_LightW_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        @brief	White 마우스 UP 이벤트.
        @return	void
        @param	object               sender
        @param  MouseButtonEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:06
        */
        private void us_LightW_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            g_VisionManager._LightManger.Channel = 1;
            g_VisionManager._LightManger.Bright = (int)us_LightW.USValue;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void us_LightW_PreviewKeyDown(object sender, KeyEventArgs e)
        @brief	White Key Down 이벤트.
        @return	void
        @param	object               sender
        @param  KeyEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:06
        */
        private void us_LightW_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                g_VisionManager._LightManger.Channel = 1;
                g_VisionManager._LightManger.Bright = (int)us_LightW.USValue;
            }
        }

        private void bn_MotionControl_Click(object sender, RoutedEventArgs e)
        {
            fn_UserJog();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (sender as RadioButton);
            if (rb != null)
            {
                try
                {
                    if (_ModelRecipe != null)
                    {
                        int num = Convert.ToInt32(rb.CommandParameter);
                        _ModelRecipe.Inspection.Algorithm = num;
                    }
                }
                catch (Exception ex)
                {
                    //fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
        }

        private void bn_Reset_Click(object sender, RoutedEventArgs e)
        {
            fn_UpdateMilling(0, new Rect(0, 0, 0, 0));
            ac_Align.fn_SetModelRect(new Rect(0, 0, 0, 0));
        }

        private void bn_Inspection_Click(object sender, RoutedEventArgs e)
        {
            fn_Update();
            WriteableBitmap wb = ac_Align.fn_GetImageStream(true);
            if (wb != null)
            {
                

                ST_ALIGN_RESULT result = new ST_ALIGN_RESULT();
                result.stResult = new ST_RESULT[5];
                
                //g_VisionManager._ImgProc.EPDResult(wb, _ModelRecipe.Inspection, ref StartY, ref EndY, ref MaxIdx);
                bool bRtn = g_VisionManager._ImgProc.EPDResult(wb, _ModelRecipe.Inspection, ref result);

                //ac_Align.fn_SetResult(result);
                //_ModelRecipe.Inspection.RefDistance
                // dScore = cy
                double dResult1 = result.stResult[1].dScore - result.stResult[0].dScore;
                double dResult2 = result.stResult[2].dScore - result.stResult[1].dScore;
                dResult1 *= UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000; // mm scale
                dResult2 *= UserClass.g_VisionManager._RecipeVision.ResolutionY / 1000; // mm scale

                bool bResult1 = false;
                bool bResult2 = false;

                bool bResult = false;

                if (_ModelRecipe.Inspection.Section1 == 0)//result <= target
                    bResult1 = dResult1 <= _ModelRecipe.Inspection.RefDistance;
                else if (_ModelRecipe.Inspection.Section1 == 1)//result >= target
                    bResult1 = dResult1 >= _ModelRecipe.Inspection.RefDistance;

                if (_ModelRecipe.Inspection.Section2 == 0)//result <= target
                    bResult2 = dResult2 <= _ModelRecipe.Inspection.RefDistance2;
                else if (_ModelRecipe.Inspection.Section2 == 1)//result >= target
                    bResult2 = dResult2 >= _ModelRecipe.Inspection.RefDistance2;

                switch (_ModelRecipe.Inspection.Condition)
                {
                    case 0://Only 1
                        bResult = bResult1;
                        break;
                    case 1://Only 2
                        bResult = bResult2;
                        break;
                    case 2:// Or
                        bResult = bResult1 && bResult2;
                        break;
                    case 3:// And
                        bResult = bResult1 || bResult2;
                        break;
                }

                up_Result1.UPValue = $"{dResult1:F3}";
                up_Result2.UPValue = $"{dResult2:F3}";

                if (bResult1)
                    up_Result1.UPValueBackground = Brushes.Green;
                else
                    up_Result1.UPValueBackground = Brushes.Red;

                if (bResult2)
                    up_Result2.UPValueBackground = Brushes.Green;
                else
                    up_Result2.UPValueBackground = Brushes.Red;

                if (bResult)
                {
                    ug_Section1.Background = Brushes.Green;
                    ug_Section2.Background = Brushes.Green;
                }
                else
                {
                    ug_Section1.Background = Brushes.Red;
                    ug_Section2.Background = Brushes.Red;
                }
            }
        }

        private void bn_Process_Click(object sender, RoutedEventArgs e)
        {
            fn_Update();
            WriteableBitmap wb = ac_Align.fn_GetImageStream(true);
            if (wb != null)
            {
                WriteableBitmap wbRtn = null;
//                  switch (_ModelRecipe.Inspection.Algorithm)
//                  {
//                      case 0:
//                          wbRtn = g_VisionManager._ImgProc.Threshold(wb, _ModelRecipe.Inspection);
//                          break;
//                      case 1:
//                          wbRtn = g_VisionManager._ImgProc.CannyEdge(wb, _ModelRecipe.Inspection);
//                          break;
//                  }
                wbRtn = g_VisionManager._ImgProc.CannyEdge(wb, _ModelRecipe.Inspection);
                try
                {
                    double[] fdata = new double[(int)Math.Round(_ModelRecipe.Inspection.ROIH + 0.5)];
                    IntPtr ptr = Marshal.AllocHGlobal(fdata.Length * sizeof(double));
                    libGetLineProfile(ptr, fdata.Length);
                    Marshal.Copy(ptr, fdata, 0, fdata.Length);
                    Marshal.FreeHGlobal(ptr);

                    new Thread(new ThreadStart(() => fn_SetHisto(fdata))).Start();
                    
                }
                catch(Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
                if (wbRtn != null)
                    ac_Align.SetImage(wbRtn, Stretch.None, false);
            }
        }
        
        private void fn_SetHisto(double[] data)
        {
            Histo.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate 
            {
                if (data != null)
                {
                    obData.Clear();
                    ((LineSeries)Histo.Series[0]).Visibility = Visibility.Visible;
                    ((LineSeries)Histo.Series[0]).Background = Brushes.DarkGray;
                    for (int i = 0; i < data.Length; i++)
                    {
                        obData.Add(new HistogramData() { X = i, Y = (float)data[i] });
                    }
                }
                else
                    ((LineSeries)Histo.Series[0]).Visibility = Visibility.Hidden;
            }));
        }

        private void bn_MoveVisn_Click(object sender, RoutedEventArgs e)
        {
            //Move Polishing Vision Position
            SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X, EN_COMD_ID.User12);
            SEQ_POLIS.fn_ReqBathImagePos();
            //
            SEQ_SPIND.fn_MoveCylLensCvr(ccFwd);
            SEQ_SPIND.fn_MoveCylIR(tg_shutter.Button.IsChecked == true ? ccBwd : ccFwd);
        }

        private void fn_SetOpticCondition()
        {
            SEQ_SPIND.fn_MoveCylLensCvr(ccFwd);
            SEQ_SPIND.fn_MoveCylIR(tg_shutter.Button.IsChecked == true ? ccBwd : ccFwd);

            g_VisionManager._CamManager.fn_SetExposure(uis_Exposuretime.slider.Value);
            g_VisionManager._CamManager.fn_SetGain(uis_Gain.slider.Value);
            g_VisionManager._LightManger.Channel = 1;
            g_VisionManager._LightManger.Bright = (int)us_LightW.USValue;
            g_VisionManager._LightManger.Channel = 2;
            g_VisionManager._LightManger.Bright = (int)us_LightIR.USValue;
        }
    }
}
