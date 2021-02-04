using System;
using System.IO;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using UserInterface;
using Microsoft.Win32;
using WaferPolishingSystem.Define;
using WaferPolishingSystem.Vision;
using static UserInterface.AlignControl;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserFunctionVision;

namespace WaferPolishingSystem.Form
{

    /// <summary>
    /// PageRecipe_Visieshot.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageRecipe_PolishingVision : Page
    {
        public del_EndModify delEndModfy = null;
        Recipe_SelectROI _SelROI = new Recipe_SelectROI();
        DataTable _dtROI = new DataTable();
        DataTable _dtMil = new DataTable();

        Rect rectCopy = new Rect();
        public int _SelectedModelIndex = -1;
        public string _SelectedModelName = "";
        public string _SelectedRecipeName = "";
        public Frame _frame_Main;

        public ModelList _ModelRecipe = new ModelList();

        public int[] GlobalLightIR           ;
        public int[] GlobalLightIRFilter     ;
        public int[] GlobalLightWhite        ;
        public int[] GlobalCameraExposure    ;
        public int[] GlobalCameraGain        ;

        public int _TabCount = 0;

        private bool bLinkModelMil = false;

        DispatcherTimer m_OneCycleTimer = new DispatcherTimer();
        
        //---------------------------------------------------------------------------
        /**
        @fn     public PageRecipe_PolishingVision()
        @brief	Recipe 초기화
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  13:55
        */
        public PageRecipe_PolishingVision()
        {
            InitializeComponent();
            ComponentDispatcher.ThreadIdle += UIInit;
            ac_Align.delUpdateRect += fn_UpdateMilling;
            ac_Align.delMilling += fn_UpdateMillingList;
            ac_Mark.fn_EnableDragButton(false);
            ac_Mark.SetMove(AlignControl.EnRoiMode.ModeMove);
            g_VisionManager._AlginPolCtrl = ac_Align;
            //tg_shutter.Button.Click += tg_shutter_UTClick;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void UIInit(object sender, EventArgs args)
        @brief	UI Init Callback
        @return	void
        @param	object    sender
        @param  EventArgs args
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:12
        */
        private void UIInit(object sender, EventArgs args)
        {
            ComponentDispatcher.ThreadIdle -= UIInit;
            this.Background = G_COLOR_PAGEBACK;

            bn_Paste.IsEnabled = false;

            _dtROI.Columns.Add("NAME");
            
            _dtROI.Columns.Add("X");
            _dtROI.Columns.Add("Y");
            _dtROI.Columns.Add("WIDTH");
            _dtROI.Columns.Add("HEIGHT");

            _dtMil.Columns.Add("NAME");
            _dtMil.Columns.Add("ENABLE");
            _dtMil.Columns.Add("Xp");
            _dtMil.Columns.Add("Yp");
            _dtMil.Columns.Add("Wp");
            _dtMil.Columns.Add("Hp");
            _dtMil.Columns.Add("Xm");
            _dtMil.Columns.Add("Ym");
            _dtMil.Columns.Add("Wm");
            _dtMil.Columns.Add("Hm");
            _dtMil.Columns.Add("Pitch");
            _dtMil.Columns.Add("PathCount");

            _dtROI.Rows.Add("Model ROI", "0", "0", "0", "0");
            _dtROI.Rows.Add("Search ROI", "0", "0", "0", "0");

            g_VisionManager._CamManager.update += ac_Align.SetImage;
            uis_Exposuretime.lb_ValueName.Content = "Exposure Time :";
            uis_Exposuretime.slider.Minimum = 1000;
            uis_Exposuretime.slider.Maximum = 20000;
            uis_Exposuretime.slider.TickFrequency = 100;
            uis_Gain.lb_ValueName.Content = "Gain :";
            uis_Gain.slider.Minimum = 1;
            uis_Gain.slider.TickFrequency = 100;
            uis_Gain.slider.Maximum = 10000;

            dg_ROI.SelectedIndex = 0;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void Page_Loaded(object sender, RoutedEventArgs e)
        @brief	Page Load Event
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:18
        */
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fn_LoadUI();
            bn_Live.Content = "Live";
            lb_Recipe.Text = _SelectedRecipeName;
            if (_SelectedModelIndex > -1)
            {
                lb_Title.Text = _SelectedModelName;
                fn_LoadData();
            }
            else
            {
                lb_Title.Text = "-";
                fn_InitData();
            }

            fn_SetButton(EnRoiMode.ModeMove);
            fn_SetRuler(false);
            ac_Align.SetFitScale();
            
            dg_ROI.ItemsSource = _dtROI.DefaultView;
            dg_Milling.ItemsSource = _dtMil.DefaultView;
            if (UserClass.g_VisionManager._CamManager._paramExposureRaw != null)
                uis_Exposuretime.Parameter = UserClass.g_VisionManager._CamManager._paramExposureRaw;
            if (UserClass.g_VisionManager._CamManager._paramGainRaw != null)
                uis_Gain.Parameter = UserClass.g_VisionManager._CamManager._paramGainRaw;

            //uis_Exposuretime.slider.Value = UserClass.g_VisionManager._RecipeModel.CameraExposureTime [(int)visionmode];
            //uis_Gain        .slider.Value = UserClass.g_VisionManager._RecipeModel.CameraGain         [(int)visionmode];

            fn_EnableButton(true);

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

        //---------------------------------------------------------------------------
        /**
        @fn     public void fn_LoadUI()
        @brief	UI Visibility 설정.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:20
        */
        public void fn_LoadUI()
        {
            if (FM._nCrntLevel >= (int)EN_USER_LEVEL.lvEngineer)
            {
                //bn_SaveImg.Visibility = Visibility.Visible;
                //bn_Open.Visibility = Visibility.Visible;
                //bn_Live.Visibility = Visibility.Visible;
                //listbox_ResultTest.Visibility = Visibility.Visible;
            }
            else
            {
                //bn_SaveImg.Visibility = Visibility.Hidden;
                //bn_Open.Visibility = Visibility.Hidden;
                //bn_Live.Visibility = Visibility.Hidden;
                //listbox_ResultTest.Visibility = Visibility.Hidden;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     public void fn_InitData()
        @brief	DataTable Data Init
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:20
        */
        public void fn_InitData()
        {
            for(int i = 0; i < 2; i++)
            {
                for(int j = 1; j<_dtROI.Columns.Count; j++)
                {
                    _dtROI.Rows[i][j] = "0";
                }
            }

            for(int i = 0; i < 10; i++)
            {
                for (int j = 1; j < _dtMil.Columns.Count; j++)
                {
                    _dtMil.Rows[i][j] = "0";
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     public void fn_LoadData()
        @brief	Page Load 시 Data Load
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:21
        */
        public void fn_LoadData()
        {
            //_SelectedModelIndex
            // Load Image
            try
            {
                if (_ModelRecipe != null)
                {
                    Point pntModelRelative = new Point();
                    Point pntModel = new Point();
                    Point pntMil = new Point();
                    Point pntMilRelative = new Point();
                    Rect[] rect = new Rect[12];
                    
                    for (int i = 0; i < 12; i++)
                    {
                        rect[i] = new Rect();
                    }

                    ac_Align.OpenImage(_ModelRecipe.strImgPath);
                    
                    ac_Align.fn_SetResolution(UserClass.g_VisionManager._RecipeVision.ResolutionX, UserClass.g_VisionManager._RecipeVision.ResolutionY);

                    fn_SetAlgorithm(_ModelRecipe.Algorithm);

                    if (_ModelRecipe.UseGlobalPolishing == 1)
                    {
                        gb_CamParam.Background = Brushes.Transparent;
                        gb_Light   .Background = Brushes.Transparent;
                        gb_CamParam.Header     = "Camera Parameter (Global)";
                        gb_Light   .Header     = "Light (Global)";
                        us_LightIR.USValue            = GlobalLightIR       [(int)EN_VISION_MODE.Polishing];
                        us_LightW.USValue             = GlobalLightWhite    [(int)EN_VISION_MODE.Polishing];
                        tg_shutter.Button.IsChecked   = GlobalLightIRFilter [(int)EN_VISION_MODE.Polishing] == 1 ? true : false;
                        uis_Exposuretime.slider.Value = GlobalCameraExposure[(int)EN_VISION_MODE.Polishing];
                        uis_Gain.slider.Value         = GlobalCameraGain    [(int)EN_VISION_MODE.Polishing];
                    }
                    else
                    {
                        gb_CamParam.Background = G_COLOR_SUBMENU;
                        gb_Light   .Background = G_COLOR_SUBMENU;
                        gb_CamParam.Header     = "Camera Parameter (Private)";
                        gb_Light   .Header     = "Light (Private)";
                        us_LightIR.USValue            = _ModelRecipe.PolishingLightIR;
                        us_LightW.USValue             = _ModelRecipe.PolishingLightWhite;
                        tg_shutter.Button.IsChecked   = _ModelRecipe.PolishingLightIRFilter == 1 ? true : false;
                        uis_Exposuretime.slider.Value = _ModelRecipe.PolishingCameraExposureTime;
                        uis_Gain.slider.Value         = _ModelRecipe.PolishingCameraGain;
                    }

                    switch (_ModelRecipe.Algorithm)
                    {
                        case EN_ALGORITHM.algModel:
                            rb_Model.IsChecked = true;
                            break;
                        case EN_ALGORITHM.algPattern:
                            rb_Pattern.IsChecked = true;
                            break;
                    }

                    pntModel.X = _ModelRecipe.Model.X;
                    pntModel.Y = _ModelRecipe.Model.Y;
                    pntModel.X += (_ModelRecipe.Model.Width / 2.0);
                    pntModel.Y += (_ModelRecipe.Model.Height / 2.0);

                    pntModelRelative = fn_GetPositionFromImageCenter(pntModel, UserClass.g_VisionManager._RecipeVision.CamWidth, UserClass.g_VisionManager._RecipeVision.CamHeight);

                    rect[0] = _ModelRecipe.Model;
                    _dtROI.Rows[0]["X"]      = rect[0].X     .ToString("0.000");
                    _dtROI.Rows[0]["Y"]      = rect[0].Y     .ToString("0.000");
                    _dtROI.Rows[0]["Width"]  = rect[0].Width .ToString("0.000");
                    _dtROI.Rows[0]["Height"] = rect[0].Height.ToString("0.000");

                    switch (_ModelRecipe.Algorithm)
                    {
                        case EN_ALGORITHM.algModel:
                            rect[1].X       = _ModelRecipe.ParamModel.SearchOffsetX;
                            rect[1].Y       = _ModelRecipe.ParamModel.SearchOffsetY;
                            rect[1].Width   = _ModelRecipe.ParamModel.SearchSizeX;
                            rect[1].Height  = _ModelRecipe.ParamModel.SearchSizeY;
                            break;
                        case EN_ALGORITHM.algPattern:
                            rect[1].X       = _ModelRecipe.ParamPattern.SearchOffsetX;
                            rect[1].Y       = _ModelRecipe.ParamPattern.SearchOffsetY;
                            rect[1].Width   = _ModelRecipe.ParamPattern.SearchSizeX;
                            rect[1].Height  = _ModelRecipe.ParamPattern.SearchSizeY;
                            break;
                    }


                    _dtROI.Rows[1]["X"]      = rect[1].X     .ToString("0.000");
                    _dtROI.Rows[1]["Y"]      = rect[1].Y     .ToString("0.000");
                    _dtROI.Rows[1]["Width"]  = rect[1].Width .ToString("0.000");
                    _dtROI.Rows[1]["Height"] = rect[1].Height.ToString("0.000");

                    _dtMil.Rows.Clear();
                    for (int i = 0; i < _TabCount; i++)
                    {
                        _dtMil.Rows.Add("Milling" + (i + 1), false, "0", "0", "0", "0", "0", "0", "0", "0", "0.005", "0");
                        _dtMil.Rows[i]["ENABLE"] = Convert.ToBoolean(_ModelRecipe.Milling[i].Enable);
                        _dtMil.Rows[i]["Xp"] = _ModelRecipe.Milling[i].MilRect.X.ToString("0.000");
                        _dtMil.Rows[i]["Yp"] = _ModelRecipe.Milling[i].MilRect.Y.ToString("0.000");
                        _dtMil.Rows[i]["Wp"] = _ModelRecipe.Milling[i].MilRect.Width.ToString("0.000");
                        _dtMil.Rows[i]["Hp"] = _ModelRecipe.Milling[i].MilRect.Height.ToString("0.000");

                        rect[i + 2] = _ModelRecipe.Milling[i].MilRect;
                        //---------------------------------------------------------------------------
                        // Meter Scale Transform Relative Location
                        //---------------------------------------------------------------------------
                        pntMil.X = _ModelRecipe.Milling[i].MilRect.X;
                        pntMil.Y = _ModelRecipe.Milling[i].MilRect.Y;
                        pntMil.X += (_ModelRecipe.Milling[i].MilRect.Width / 2.0);
                        pntMil.Y += (_ModelRecipe.Milling[i].MilRect.Height / 2.0);
                        pntMilRelative = fn_GetPositionFromImageCenter(pntMil, g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);


                        _dtMil.Rows[i]["Xm"] = ((pntMilRelative.X - pntModelRelative.X) * (g_VisionManager._RecipeVision.ResolutionX / 1000.0)).ToString("0.00");
                        _dtMil.Rows[i]["Ym"] = ((pntMilRelative.Y - pntModelRelative.Y) * (g_VisionManager._RecipeVision.ResolutionY / 1000.0)).ToString("0.00");
                        _dtMil.Rows[i]["Wm"] = (_ModelRecipe.Milling[i].MilRect.Width * (g_VisionManager._RecipeVision.ResolutionX / 1000.0)).ToString("0.00");
                        _dtMil.Rows[i]["Hm"] = (_ModelRecipe.Milling[i].MilRect.Height * (g_VisionManager._RecipeVision.ResolutionY / 1000.0)).ToString("0.00");

                        if (_ModelRecipe.Milling[i].Pitch == 0)
                            _ModelRecipe.Milling[i].Pitch = 0.1;
                        _dtMil.Rows[i]["Pitch"] = _ModelRecipe.Milling[i].Pitch;
                        _dtMil.Rows[i]["PathCount"] = _ModelRecipe.Milling[i].PathCount;
                    }

                    ac_Align.SetRectangle(rect);
                    ac_Align.SelectObject(EnObjectSelect.SelShowAll);
                    ac_Align.SetFitScale();
                    CroppedBitmap bmp = ac_Align.GetModelImage();
                    if (bmp != null)
                    {
                        ac_Mark.SetImage(new WriteableBitmap(bmp));
                        ac_Mark.SetFitScale();
                    }
                    else
                    {
                        ac_Mark.ClearCanvas();
                    }
                }
            }
            catch (Exception ex)
            {
                //fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void dg_ROI_GetFocus(object sender, RoutedEventArgs e)
        @brief	DataGrid - ROI가 포커스를 얻었을 때
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:23
        */
        private void dg_ROI_GotFocus(object sender, RoutedEventArgs e)
        {
            if (dg_Milling.SelectedIndex > -1)
                dg_Milling.SelectedIndex = -1;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void dg_Milling_GotFocus(object sender, RoutedEventArgs e)
        @brief	DataGrid - Milling이 포커스를 얻었을 때
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:23
        */
        private void dg_Milling_GotFocus(object sender, RoutedEventArgs e)
        {
            if (dg_ROI.SelectedIndex > -1)
                dg_ROI.SelectedIndex = -1;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void cb_Detail_SelectionChanged(object sender, SelectionChagedEventArgs e)
        @brief	Detail ComboBox 이벤트.
        @return	void
        @param	object                   sender
        @param  SelectionChagedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:25
        */
        private void cb_Detail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if(cb_Detail.SelectedIndex != -1)
                _ModelRecipe.ParamModel.DetailLevel = cb_Detail.SelectedIndex;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void RadioButton_Checked(object sender, RoutedEventArgs e)
        @brief	알고리즘 선택 Radio 버튼 이벤트.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:27
        */
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (sender as RadioButton);
            if(rb != null)
            {
                try
                {
                    if (_ModelRecipe != null)
                    {
                        int num = Convert.ToInt32(rb.CommandParameter);
                        _ModelRecipe.Algorithm = (EN_ALGORITHM)num;
                        fn_SetAlgorithm(_ModelRecipe.Algorithm);
                    }
                }
                catch (Exception ex)
                {
                    //fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_SEtAlgorithm(EN_ALGORITHM alg)
        @brief	알고리즘 선택 시 데이터 로드
        @return	void
        @param	EN_ALGORITHM alg : 알고리즘 (Pattern Matching or Model Finder)
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:28
        */
        private void fn_SetAlgorithm(EN_ALGORITHM alg)
        {
            double dValue = 0.0;
            switch (alg)
            {
                case EN_ALGORITHM.algModel:
                    up_Acceptance   .UPValue        = _ModelRecipe.ParamModel.Acceptance.ToString();
                    up_Certainty    .UPValue        = _ModelRecipe.ParamModel.Certainty.ToString();
                    up_Smoothness   .UPValue        = _ModelRecipe.ParamModel.Smoothness.ToString();
                    cb_Detail       .SelectedIndex  = _ModelRecipe.ParamModel.DetailLevel;
                    sp_Detail       .IsEnabled      = true;
                    up_Smoothness   .IsEnabled      = true;
                    ug_Pattern      .IsEnabled      = false;
                    up_ScaleMargin  .IsEnabled      = true;
                    //ug_Scale        .IsEnabled      = true;
                    //chk_Scale       .IsChecked      = _ModelRecipe.ParamModel.SearchScaleRange == 1 ? true : false;
                    //up_ScaleMinimum .UPValue        = _ModelRecipe.ParamModel.ScaleMinFactor.ToString();
                    //up_ScaleMaximum .UPValue        = _ModelRecipe.ParamModel.ScaleMaxFactor.ToString();
                    //chk_AngleEnable.IsChecked       = _ModelRecipe.ParamModel.SearchAngleRange == 1 ? true : false ;
                    //up_NegAngle     .UPValue        = _ModelRecipe.ParamModel.AngleDeltaNeg  .ToString();
                    //up_PosAngle     .UPValue        = _ModelRecipe.ParamModel.AngleDeltaPos  .ToString();
                    dValue = ((1.0 - _ModelRecipe.ParamModel.ScaleMinFactor) + (_ModelRecipe.ParamModel.ScaleMaxFactor - 1.0)) / 2.0;
                    up_ScaleMargin.UPValue = (dValue * 100.0).ToString();
                    dValue = (_ModelRecipe.ParamModel.AngleDeltaPos + _ModelRecipe.ParamModel.AngleDeltaNeg) / 2.0;
                    up_AngleMargin.UPValue = dValue.ToString();
                    //up_Angle        .UPValue        = _ModelRecipe.ParamModel.Angle          .ToString();
                    break;
                case EN_ALGORITHM.algPattern:
                    up_Acceptance   .UPValue        = _ModelRecipe.ParamPattern.Acceptance     .ToString();
                    up_Certainty    .UPValue        = _ModelRecipe.ParamPattern.Certainty      .ToString();
                    up_Angle        .UPValue        = _ModelRecipe.ParamPattern.Angle          .ToString();
                    //up_NegAngle     .UPValue        = _ModelRecipe.ParamPattern.NegativeDelta  .ToString();
                    //up_PosAngle     .UPValue        = _ModelRecipe.ParamPattern.PositiveDelta  .ToString();
                    //chk_AngleEnable.IsChecked       = _ModelRecipe.ParamPattern.AngleMode == 1 ? true : false ;
                    //ug_Scale        .IsEnabled      = false;
                    dValue = (_ModelRecipe.ParamPattern.NegativeDelta + _ModelRecipe.ParamPattern.PositiveDelta) / 2.0;
                    up_AngleMargin.UPValue = dValue.ToString("0.000");
                    up_Tolerance    .UPValue        = _ModelRecipe.ParamPattern.Tolerance      .ToString();
                    up_Accuracy     .UPValue        = _ModelRecipe.ParamPattern.Accuracy       .ToString();
                    cb_Detail       .SelectedIndex  = -1;
                    sp_Detail       .IsEnabled      = false;
                    up_Smoothness   .IsEnabled      = false;
                    ug_Pattern      .IsEnabled      = true;
                    up_ScaleMargin  .IsEnabled      = false;
                    break;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void dg_ROI_SelectionChaged(object sender, SelectionChagedEventArgs e)
        @brief	DataGrid-ROI 선택 변화에 따른 동작.
        @return	void
        @param	object                   sender
        @param  SelectionChagedEvnetArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:29
        */
        private void dg_ROI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if( dg != null )
            {
                int nIdx = dg.SelectedIndex;
                if(nIdx > -1)
                {
                    _SelROI.SelMode = 1;
                    _SelROI.SelIdx = nIdx;
                    _SelROI.Name = _dtROI.Rows[nIdx]["Name"].ToString();
                    _SelROI.X = Convert.ToDouble(_dtROI.Rows[nIdx]["X"].ToString());
                    _SelROI.Y = Convert.ToDouble(_dtROI.Rows[nIdx]["Y"].ToString());
                    _SelROI.W = Convert.ToDouble(_dtROI.Rows[nIdx]["Width"].ToString());
                    _SelROI.H = Convert.ToDouble(_dtROI.Rows[nIdx]["Height"].ToString());
                    grid_SelROI.DataContext = _SelROI;
                    ac_Align.SelectObject(EnObjectSelect.SelModel + nIdx);
                    //ac_Align.ViewContent((int)(EnObjectSelect.SelModel + nIdx), true);
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void dg_Milling_SelectionChanged(object sender, SelectionChagedEventArgs e)
        @brief	DataGrid-Milling 선택 변화에 따른 동작.
        @return	void
        @param	object                   sender
        @param  SelectionChagedEvnetArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:29
        */
        private void dg_Milling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if(dg != null)
            {
                int nIdx = dg.SelectedIndex;
                if( nIdx > -1)
                {
                    _SelROI.SelMode = 2;
                    _SelROI.SelIdx = nIdx;
                    _SelROI.Name = _dtMil.Rows[nIdx]["Name"].ToString();
                    _SelROI.X = Convert.ToDouble(_dtMil.Rows[nIdx]["Xp"].ToString());
                    _SelROI.Y = Convert.ToDouble(_dtMil.Rows[nIdx]["Yp"].ToString());
                    _SelROI.W = Convert.ToDouble(_dtMil.Rows[nIdx]["Wp"].ToString());
                    _SelROI.H = Convert.ToDouble(_dtMil.Rows[nIdx]["Hp"].ToString());
                    grid_SelROI.DataContext = _SelROI;
                    ac_Align.SelectObject(EnObjectSelect.SelMil1 + nIdx);
                    //ac_Align.ViewContent((int)(EnObjectSelect.SelMil1 + nIdx), true);
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Zoom_Click(object sender, RoutedEventArgs e)
        @brief	줌 클릭 동작.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:31
        */
        private void bn_Zoom_Click(object sender, RoutedEventArgs e)
        {
            double dTemp = 1.0;

            //bn_Zoom_p1.Background = G_COLOR_BTNNORMAL;
            bn_Zoom_p2.Background = G_COLOR_BTNNORMAL;
            bn_Zoom_p4.Background = G_COLOR_BTNNORMAL;
            bn_Zoom_p8.Background = G_COLOR_BTNNORMAL;
            bn_Zoom_1.Background = G_COLOR_BTNNORMAL;
            bn_Zoom_1p5.Background = G_COLOR_BTNNORMAL;
            bn_Zoom_Fit.Background = G_COLOR_BTNNORMAL;
            UserButton bn = (sender as UserButton);
            string strScale = bn.Content as string;
            switch (strScale)
            {
                case "x 0.1":
                    dTemp = 0.1;
                    break;
                case "x 0.2":
                    dTemp = 0.2;
                    break;
                case "x 0.4":
                    dTemp = 0.4;
                    break;
                case "x 0.8":
                    dTemp = 0.8;
                    break;
                case "x 1.0":
                    dTemp = 1.0;
                    break;
                case "x 1.5":
                    dTemp = 1.5;
                    break;
                case "Fit":
                        dTemp = ac_Align.GetFitScale();
                    break;
            }
            bn.Background = G_COLOR_BTNCLICKED;
            ac_Align.SetScale(dTemp);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Draw_Click(object sender, RoutedEventArgs e)
        @brief	Draw 버튼 클릭 동작
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:32
        */
        private void bn_Draw_Click(object sender, RoutedEventArgs e)
        {
            fn_SetButton(EnRoiMode.ModeDraw);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Move_Click(object sender, RoutedEventArgs e)
        @brief	Move 버튼 클릭 동작
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:32
        */
        private void bn_Move_Click(object sender, RoutedEventArgs e)
        {
            fn_SetButton(EnRoiMode.ModeMove);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Draw_Click(object sender, RoutedEventArgs e)
        @brief	Draw, Move 버튼 클릭 동작
        @return	void
        @param	EnRoiMode mode : 동작 Mode
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:32
        */
        private void fn_SetButton(EnRoiMode mode)
        {
//             bn_Move.Background = G_COLOR_BTNNORMAL;
//             bn_Draw.Background = G_COLOR_BTNNORMAL;
            
            ac_Align.SetMove(mode);
//             switch (mode)
//             {
//                 case EnRoiMode.ModeMove:
//                     bn_Move.Background = G_COLOR_BTNCLICKED;
//                     break;
//                 case EnRoiMode.ModeDraw:
//                     bn_Draw.Background = G_COLOR_BTNCLICKED;
//                     break;
//                 default:
//                     break;
//             }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Ruler_Click(object sender, RoutedEventArgs e)
        @brief	룰러 클릭 동작
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:34
        */
        private void bn_Ruler_Click(object sender, RoutedEventArgs e)
        {

            if (bn_Ruler.Background == G_COLOR_BTNNORMAL)
                fn_SetRuler(true);
            else
                fn_SetRuler(false);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_SetRuler(bool bRuler)
        @brief	룰러 동작
        @return void
        @param	bool bRuler : Ruler 동작 여부.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:35
        */
        private void fn_SetRuler(bool bRuler)
        {
            if (bRuler)
                bn_Ruler.Background = G_COLOR_BTNCLICKED;
            else
                bn_Ruler.Background = G_COLOR_BTNNORMAL;
            ac_Align.RulerEnable(bRuler);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_UpdateMilling(int idx, Rect rect)
        @brief	Milling ROI Update Delegate
        @return	void
        @param	int  idx  : Update된 ROI Index
        @param  Rect rect : ROI의 Rectangle 정보
        @remark	
         - Align Control에 ROI 그릴때 발생되는 이벤트.
         - DataGrid 및 Recipe 내용 Update.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:36
        */
        private void fn_UpdateMilling(int idx, Rect rect)
        {
            try
            {
                Point pntMil = new Point();
                Point pntMilRelative = new Point();
                Point pntModel = new Point();
                Point pntModelRelative = new Point();

                _SelROI.X = rect.X;
                _SelROI.Y = rect.Y;
                _SelROI.W = rect.Width;
                _SelROI.H = rect.Height;

                // Milling DataGrid 데이터 셋팅.
                if (idx >= 2)
                {
                    _dtMil.Rows[idx - 2]["Xp"] = lb_SelROI_X.Content = rect.X.ToString("0.000");
                    _dtMil.Rows[idx - 2]["Yp"] = lb_SelROI_Y.Content = rect.Y.ToString("0.000");
                    _dtMil.Rows[idx - 2]["Wp"] = lb_SelROI_W.Content = rect.Width.ToString("0.000");
                    _dtMil.Rows[idx - 2]["Hp"] = lb_SelROI_H.Content = rect.Height.ToString("0.000");

                    // Model 기준 상대좌표 변환할 것.
                    pntModel.X = Convert.ToDouble(_dtROI.Rows[0]["X"] as string);
                    pntModel.Y = Convert.ToDouble(_dtROI.Rows[0]["Y"] as string);
                    // Milling-Model Center to Center
                    pntModel.X += (_ModelRecipe.Model.Width / 2.0);
                    pntModel.Y += (_ModelRecipe.Model.Height / 2.0);
                    pntModelRelative = fn_GetPositionFromImageCenter(pntModel, g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);

                    pntMil.X = rect.X;
                    pntMil.Y = rect.Y;
                    // Milling-Model Center to Center
                    pntMil.X += (rect.Width / 2.0);
                    pntMil.Y += (rect.Height / 2.0);
                    pntMilRelative = fn_GetPositionFromImageCenter(pntMil, g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);
                    
                    _dtMil.Rows[idx - 2]["Xm"] = ((pntMilRelative.X - pntModelRelative.X) * (g_VisionManager._RecipeVision.ResolutionX / 1000.0)).ToString("0.000");
                    _dtMil.Rows[idx - 2]["Ym"] = ((pntMilRelative.Y - pntModelRelative.Y) * (g_VisionManager._RecipeVision.ResolutionY / 1000.0)).ToString("0.000");
                    _dtMil.Rows[idx - 2]["Wm"] = (rect.Width  * g_VisionManager._RecipeVision.ResolutionX / 1000.0).ToString("0.000");
                    _dtMil.Rows[idx - 2]["Hm"] = (rect.Height * g_VisionManager._RecipeVision.ResolutionY / 1000.0).ToString("0.000");
                    double dpitch, dHm;
                    double.TryParse(_dtMil.Rows[idx - 2]["Pitch"].ToString(), out dpitch);
                    double.TryParse(_dtMil.Rows[idx - 2]["Hm"]   .ToString(), out dHm);
                    _dtMil.Rows[idx - 2]["PathCount"] = (dHm / (dpitch * 2)).ToString("0");
                }
                //ModelROI
                else
                {
                    //g_VisionManager._RecipeModel.Model[_SelectedModelIndex].Model = rect;
                    _ModelRecipe.Model = rect;

                    _dtROI.Rows[idx]["X"]      = lb_SelROI_X.Content = rect.X.ToString("0.000");
                    _dtROI.Rows[idx]["Y"]      = lb_SelROI_Y.Content = rect.Y.ToString("0.000");
                    _dtROI.Rows[idx]["Width"]  = lb_SelROI_W.Content = rect.Width.ToString("0.000");
                    _dtROI.Rows[idx]["Height"] = lb_SelROI_H.Content = rect.Height.ToString("0.000");
                    

                    if (idx == 0)
                    {
                        pntModel.X = rect.X;
                        pntModel.Y = rect.Y;
                        pntModel.X += (rect.Width  / 2.0);
                        pntModel.Y += (rect.Height / 2.0);
                        pntModelRelative = fn_GetPositionFromImageCenter(pntModel, g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);
                        

                        for (int i = 0; i < _TabCount; i++)
                        {
                            pntMil.X = Convert.ToDouble(_dtMil.Rows[i]["Xp"] as string);
                            pntMil.Y = Convert.ToDouble(_dtMil.Rows[i]["Yp"] as string);
                            // Milling-Model Center to Center
                            pntMil.X += (Convert.ToDouble(_dtMil.Rows[i]["Wp"] as string) / 2.0);
                            pntMil.Y += (Convert.ToDouble(_dtMil.Rows[i]["Hp"] as string) / 2.0);
                            
                            pntMilRelative = fn_GetPositionFromImageCenter(pntMil, g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);
                            
                            _dtMil.Rows[i]["Xm"] = ((pntMilRelative.X - pntModelRelative.X) * (g_VisionManager._RecipeVision.ResolutionX / 1000.0)).ToString("0.000");
                            _dtMil.Rows[i]["Ym"] = ((pntMilRelative.Y - pntModelRelative.Y) * (g_VisionManager._RecipeVision.ResolutionY / 1000.0)).ToString("0.000");
                        }
                    }
                    else if (idx == 1)
                    {
                        // Param 처리.
                        _ModelRecipe.ParamModel  .SearchOffsetX   = (int)rect.X;
                        _ModelRecipe.ParamModel  .SearchOffsetY   = (int)rect.Y;
                        _ModelRecipe.ParamModel  .SearchSizeX     = (int)rect.Width;
                        _ModelRecipe.ParamModel  .SearchSizeY     = (int)rect.Height;

                        _ModelRecipe.ParamPattern.SearchOffsetX   = (int)rect.X;
                        _ModelRecipe.ParamPattern.SearchOffsetY   = (int)rect.Y;
                        _ModelRecipe.ParamPattern.SearchSizeX     = (int)rect.Width;
                        _ModelRecipe.ParamPattern.SearchSizeY     = (int)rect.Height;
                    }

                }
            }
            catch (Exception ex)
            {
                //fn_WriteLog(this.Title + " : " + ex.Message, EN_LOG_TYPE.ltVision);
            }
        }

        private void fn_UpdateMillingList(Rect[] rect)
        {
            for(int idx =0; idx < _dtMil.Rows.Count; idx++)
            {
                _dtMil.Rows[idx]["Xp"] = rect[idx].X.ToString("0.000");
                _dtMil.Rows[idx]["Yp"] = rect[idx].Y.ToString("0.000");
                _dtMil.Rows[idx]["Wp"] = rect[idx].Width.ToString("0.000");
                _dtMil.Rows[idx]["Hp"] = rect[idx].Height.ToString("0.000");
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     public fn_Update()
        @brief	EndModify시 데이터 업데이트.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:40
        */
        public void fn_Update()
        {
            try
            {
                bool bValue;
                double dValue;
                Rect rectSearch;
                
                // Optic Condition 분리 코드.
                if(_ModelRecipe.UseGlobalPolishing == 1)
                {
                    GlobalCameraExposure [(int)EN_VISION_MODE.Polishing] = (int)uis_Exposuretime.slider.Value;
                    GlobalCameraGain     [(int)EN_VISION_MODE.Polishing] = (int)uis_Gain        .slider.Value;
                    GlobalLightIRFilter  [(int)EN_VISION_MODE.Polishing] = tg_shutter.Button.IsChecked == true ? 1 : 0;
                    GlobalLightIR        [(int)EN_VISION_MODE.Polishing] = (int)us_LightIR.USValue;
                    GlobalLightWhite     [(int)EN_VISION_MODE.Polishing] = (int)us_LightW .USValue;
                }
                else
                {
                    _ModelRecipe.PolishingCameraExposureTime = (int)uis_Exposuretime.slider.Value;
                    _ModelRecipe.PolishingCameraGain         = (int)uis_Gain        .slider.Value;
                    _ModelRecipe.PolishingLightIRFilter      = tg_shutter.Button.IsChecked == true ? 1 : 0;
                    _ModelRecipe.PolishingLightIR            = (int)us_LightIR.USValue;
                    _ModelRecipe.PolishingLightWhite         = (int)us_LightW .USValue;
                }
                // 201106 - Model ROI 변경되는 증상 Test Code

                _ModelRecipe.Model = ac_Align.GetRectModel();
                //double.TryParse((string)_dtROI.Rows[0]["X"], out dValue);
                //_ModelRecipe.Model.X        = dValue;
                //double.TryParse((string)_dtROI.Rows[0]["Y"], out dValue);
                //_ModelRecipe.Model.Y        = dValue;
                //double.TryParse((string)_dtROI.Rows[0]["Width"], out dValue);
                //_ModelRecipe.Model.Width    = dValue;
                //double.TryParse((string)_dtROI.Rows[0]["Height"], out dValue);
                //_ModelRecipe.Model.Height   = dValue;

                rectSearch = ac_Align.GetRectSearch();
                //double dSearchOffsetX, dSearchOffsetY, dSearchSizeX, dSearchSizeY;
                //
                //double.TryParse((string)_dtROI.Rows[1]["X"], out dSearchOffsetX);
                //double.TryParse((string)_dtROI.Rows[1]["Y"], out dSearchOffsetY);
                //double.TryParse((string)_dtROI.Rows[1]["Width"], out dSearchSizeX);
                //double.TryParse((string)_dtROI.Rows[1]["Height"], out dSearchSizeY);


                
                if (_ModelRecipe.Algorithm == EN_ALGORITHM.algModel)
                {
                    _ModelRecipe.ParamModel.SearchOffsetX = (int)rectSearch.X;
                    _ModelRecipe.ParamModel.SearchOffsetY = (int)rectSearch.Y;
                    _ModelRecipe.ParamModel.SearchSizeX = (int)rectSearch.Width;
                    _ModelRecipe.ParamModel.SearchSizeY = (int)rectSearch.Height;
                    double.TryParse(up_Acceptance.UPValue, out dValue);
                    _ModelRecipe.ParamModel.Acceptance = dValue;
                    double.TryParse(up_Certainty.UPValue, out dValue);
                    _ModelRecipe.ParamModel.Certainty = dValue;
                    double.TryParse(up_Smoothness.UPValue, out dValue);
                    _ModelRecipe.ParamModel.Smoothness = dValue;
                    _ModelRecipe.ParamModel.DetailLevel = cb_Detail.SelectedIndex;

                    // Scale 탐색 추가 20201103 -> 항목 수정 20210105
                    //_ModelRecipe.ParamModel.SearchScaleRange = chk_Scale.IsChecked == true ? 1 : 0;
                    //double.TryParse(up_ScaleMinimum.UPValue, out dValue);
                    //_ModelRecipe.ParamModel.ScaleMinFactor = dValue;
                    //double.TryParse(up_ScaleMaximum.UPValue, out dValue);
                    //_ModelRecipe.ParamModel.ScaleMaxFactor = dValue;
                    //
                    //_ModelRecipe.ParamModel.SearchAngleRange = chk_AngleEnable.IsChecked == true ? 1 : 0;
                    //double.TryParse(up_NegAngle.UPValue, out dValue);
                    //_ModelRecipe.ParamModel.AngleDeltaNeg = dValue;
                    //double.TryParse(up_PosAngle.UPValue, out dValue);
                    //_ModelRecipe.ParamModel.AngleDeltaPos = dValue;

                    // Scale, Angle 강제 적용 및 파라메터 단일화 - 20210105
                    _ModelRecipe.ParamModel.SearchScaleRange = 1;
                    double.TryParse(up_ScaleMargin.UPValue, out dValue);
                    if (dValue == 0)
                    {
                        dValue = UserConstVision.DEFAULT_SCALEMARGIN;
                        up_ScaleMargin.UPValue = dValue.ToString();
                    }
                    _ModelRecipe.ParamModel.ScaleMinFactor = (100 - dValue) / 100.0;
                    _ModelRecipe.ParamModel.ScaleMaxFactor = (100 + dValue) / 100.0;

                    _ModelRecipe.ParamModel.SearchAngleRange = 1;
                    double.TryParse(up_AngleMargin.UPValue, out dValue);
                    if (dValue == 0)
                    {
                        dValue = UserConstVision.DEFAULT_ANGLEMARGIN;
                        up_AngleMargin.UPValue = dValue.ToString();
                    }
                    _ModelRecipe.ParamModel.AngleDeltaNeg = dValue;
                    _ModelRecipe.ParamModel.AngleDeltaPos = dValue;
                }
                else
                {
                    _ModelRecipe.ParamPattern.SearchOffsetX = (int)rectSearch.X;
                    _ModelRecipe.ParamPattern.SearchOffsetY = (int)rectSearch.Y;
                    _ModelRecipe.ParamPattern.SearchSizeX = (int)rectSearch.Width;
                    _ModelRecipe.ParamPattern.SearchSizeY = (int)rectSearch.Height;
                    double.TryParse(up_Acceptance.UPValue, out dValue);
                    _ModelRecipe.ParamPattern.Acceptance = dValue;
                    double.TryParse(up_Certainty.UPValue, out dValue);
                    _ModelRecipe.ParamPattern.Certainty = dValue;

                    double.TryParse(up_Angle.UPValue    , out dValue);
                    _ModelRecipe.ParamPattern.Angle = dValue;

                    // Angle 단일화 - 20210105
                    double.TryParse(up_AngleMargin.UPValue, out dValue);
                    if (dValue == 0)
                    {
                        dValue = UserConstVision.DEFAULT_ANGLEMARGIN;
                        up_AngleMargin.UPValue = dValue.ToString();
                    }
                    _ModelRecipe.ParamPattern.NegativeDelta = dValue;
                    _ModelRecipe.ParamPattern.PositiveDelta = dValue;

                    // Angle Mode Force True - 20210105
                    _ModelRecipe.ParamPattern.AngleMode = 1;
                    //_ModelRecipe.ParamPattern.AngleMode = chk_AngleEnable.IsChecked == true ? 1 : 0;

                    //double.TryParse(up_NegAngle.UPValue , out dValue);
                    //_ModelRecipe.ParamPattern.NegativeDelta = dValue;
                    //double.TryParse(up_PosAngle.UPValue , out dValue);
                    //_ModelRecipe.ParamPattern.PositiveDelta = dValue;

                    double.TryParse(up_Tolerance.UPValue, out dValue);
                    _ModelRecipe.ParamPattern.Tolerance = dValue;
                    double.TryParse(up_Accuracy.UPValue , out dValue);
                    _ModelRecipe.ParamPattern.Accuracy = dValue;
                    _ModelRecipe.ParamPattern.InterpolationMode = cb_Interpolation.SelectedIndex;
                }

                for (int i = 0; i < 10; i++)
                {
                    bool.TryParse((string)_dtMil.Rows[i]["ENABLE"], out bValue);

                    _ModelRecipe.Milling[i].Enable           = bValue ? 1 : 0;
                    double.TryParse((string)_dtMil.Rows[i]["Xp"], out dValue);
                    _ModelRecipe.Milling[i].MilRect.X        = dValue;
                    double.TryParse((string)_dtMil.Rows[i]["Yp"], out dValue);
                    _ModelRecipe.Milling[i].MilRect.Y        = dValue;
                    double.TryParse((string)_dtMil.Rows[i]["Wp"], out dValue);
                    _ModelRecipe.Milling[i].MilRect.Width    = dValue;
                    double.TryParse((string)_dtMil.Rows[i]["Hp"], out dValue);
                    _ModelRecipe.Milling[i].MilRect.Height   = dValue;

                    double.TryParse(_dtMil.Rows[i]["Pitch"].ToString(), out dValue);
                    _ModelRecipe.Milling[i].Pitch = dValue;
                    double.TryParse(_dtMil.Rows[i]["PathCount"].ToString(), out dValue);
                    _ModelRecipe.Milling[i].PathCount = (int)dValue;
                }
            }
            catch(Exception ex)
            {
                //fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_OneShot_Click(object sender, RoutedEventArgs e)
        @brief	카메라 원샷 클릭.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:42
        */
        private void bn_OneShot_Click(object sender, RoutedEventArgs e)
        {
            fn_SetOpticCondition();
            // Grab Image.
            g_VisionManager._CamManager.fn_GrabStart();
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Search_Click(object sender, RoutedEventArgs e)
        @brief	Algorithm Test
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  14:43
        */
        private void bn_Search_Click(object sender, RoutedEventArgs e)
        {
            ac_Align.fn_ClearResult();
            fn_Update();
            ST_ALIGN_RESULT stResult;
            try
            {
                WriteableBitmap wbmark = ac_Mark.fn_GetImageStream(true);
                WriteableBitmap wbimg = ac_Align.fn_GetImageStream();
                if (wbmark == null || wbimg == null)
                {
                    fn_WriteResult($"Search Fail : Invalid Image.");
                    return;
                }
                g_VisionManager._AlignManager.fn_SetMarkStream(wbmark);
                g_VisionManager._AlignManager.fn_SetImageStream(wbimg);
                g_VisionManager._AlignManager.fn_SetParameter(_ModelRecipe);
                g_VisionManager._AlignManager.fn_RunAlignment(_ModelRecipe.Algorithm == EN_ALGORITHM.algModel ? true : false);
                stResult = g_VisionManager._AlignManager.fn_GetSearchResult();
                                
                libDestroyModel();
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
                return;
            }
            // 최대 스코어 결과 처리.
            ST_RESULT Result = new ST_RESULT();
            int nIdx = 0;
            double dMaxScore = 0.00;
            if (stResult.NumOfFound > 0)
            {
                for (int i = 0; i < stResult.stResult.Length; i++)
                {
                    if (stResult.stResult[i].dScore > dMaxScore)
                    {
                        nIdx = i;
                        dMaxScore = stResult.stResult[i].dScore;
                    }
                }
                Result.dX = stResult.stResult[nIdx].dX;
                Result.dY = stResult.stResult[nIdx].dY;
                Result.dWidth = stResult.stResult[nIdx].dWidth;
                Result.dHeight = stResult.stResult[nIdx].dHeight;
                Result.dAngle = stResult.stResult[nIdx].dAngle;
                Result.dScore = stResult.stResult[nIdx].dScore;


                Point pntTemp = new Point();
                // 현재 레시피 모델 좌표 중심으로 변환.
                pntTemp.X = _ModelRecipe.Model.X + (_ModelRecipe.Model.Width / 2.0);
                pntTemp.Y = _ModelRecipe.Model.Y + (_ModelRecipe.Model.Height / 2.0);
                // 현재 레시피 모델 중심 좌표 1사분면 좌표계로 변환.
                Point pntRecipeModel = fn_GetPositionFromImageCenter(pntTemp, g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);

                // ModelFinder 결과는 찾은 모델의 중심을 반환하므로 별도 과정 없이 1사분면 좌표계로 변환.
                Point pntSearchedModel = fn_GetPositionFromImageCenter(new Point(Result.dX, Result.dY), g_VisionManager._RecipeVision.CamWidth, g_VisionManager._RecipeVision.CamHeight);

                // Recipe의 Model과 찾은 Model간의 거리차 계산.
                Point pntModelSpan = new Point();
                pntModelSpan.X = pntSearchedModel.X - pntRecipeModel.X;
                pntModelSpan.Y = pntSearchedModel.Y - pntRecipeModel.Y;

                pntModelSpan.X *= g_VisionManager._RecipeVision.ResolutionX / 1000.0;
                pntModelSpan.Y *= g_VisionManager._RecipeVision.ResolutionY / 1000.0; // um -> mm
                // 좌표계 변환 작업 할 것.
                //fn_WriteResult($"X : {pntModelSpan.X:F3} mm, Y : {pntModelSpan.Y:F3} mm, Score : {Result.dScore:F3}, Angle : {Result.dAngle:F2}");

                if (Result.dAngle > 180)
                    Result.dAngle -= 360;
                if (Result.dAngle > 90)
                    Result.dAngle -= 180;
                if (Result.dAngle < -90)
                    Result.dAngle += 180;


                fn_WriteResult($"X : {Result.dX:F1} px, Y : {Result.dY:F1} px, Score : {Result.dScore:F3}, Angle : {Result.dAngle:F2}");

                fn_WriteLog($"Pol_SearchTest - X : {Result.dX:F1} px, Y : {Result.dY:F1} px, Score : {Result.dScore:F3}, Angle : {Result.dAngle:F2}", EN_LOG_TYPE.ltVision);

                for (int i = 0; i < stResult.stResult.Length; i++)
                {
                    if (i != nIdx)
                    {
                        stResult.stResult[i].dX = 0;
                        stResult.stResult[i].dY = 0;
                        stResult.stResult[i].dWidth = 0;
                        stResult.stResult[i].dHeight = 0;
                    }
                }
                ac_Align.fn_SetResult(stResult);
            }
            else
            {
                fn_WriteResult($"Search Fail : {stResult.NumOfFound}");
                fn_WriteLog($"Pol_SearchTest - Search Fail {stResult.NumOfFound}", EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_AllView_Click(object sender, RoutedEventArgs e)
        @brief	AllView Click 이벤트.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  15:01
        */
        private void bn_AllView_Click(object sender, RoutedEventArgs e)
        {
            ac_Align.SelectObject(EnObjectSelect.SelShowAll);
            bool bEnable = false;

            for(int i = 0; i < _dtROI.Rows.Count; i++)
            {
                ac_Align.ViewContent((int)(EnObjectSelect.SelModel + i), false);
            }

            for(int i = 0; i < _dtMil.Rows.Count; i ++)
            {

                bEnable = Convert.ToBoolean(_dtMil.Rows[i]["ENABLE"]);

                ac_Align.ViewContent((int)(EnObjectSelect.SelMil1 + i), bEnable);
            }
            
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private bn_Copy_Click(object sender, RoutedEventArgs e)
        @brief	DataGrid Milling Copy
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  15:44
        */
        private void bn_Copy_Click(object sender, RoutedEventArgs e)
        {
            int nIndex = dg_Milling.SelectedIndex;
            if(nIndex > -1)
            {
                rectCopy = ac_Align.GetRectMilling(nIndex);
                lb_CopyFrom.Content = _dtMil.Rows[nIndex]["NAME"];
                bn_Paste.IsEnabled = true;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private bn_Paste_Click(object sender, RoutedEventArgs e)
        @brief	DataGrid Milling Paste
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  15:44
        */
        private void bn_Paste_Click(object sender, RoutedEventArgs e)
        {
            int nIndex = dg_Milling.SelectedIndex;
            if (nIndex > -1)
            {
                fn_UpdateMilling(nIndex + 2, rectCopy);
                ac_Align.fn_SetModelRect(rectCopy);
            }
        }
        //---------------------------------------------------------------------------
        /**
        @fn     private bn_Reset_Click(object sender, RoutedEventArgs e)
        @brief	DataGrid Milling Reset
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  15:44
        */
        private void bn_Reset_Click(object sender, RoutedEventArgs e)
        {
            int nIndex = dg_ROI.SelectedIndex;
            if (nIndex > -1)
            {
                fn_UpdateMilling(nIndex, new Rect(0, 0, 0, 0));
                ac_Align.fn_SetModelRect(new Rect(0, 0, 0, 0));
            }

            nIndex = dg_Milling.SelectedIndex;
            if (nIndex > -1)
            {
                fn_UpdateMilling(nIndex + 2, new Rect(0, 0, 0, 0));
                ac_Align.fn_SetModelRect(new Rect(0, 0, 0, 0));
            }
            
        }
        //---------------------------------------------------------------------------
        /**
        @fn     private bn_EndModify_Click(object sender, RoutedEventArgs e)
        @brief	End Modify Click.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  15:44
        */
        private void bn_EndModify_Click(object sender, RoutedEventArgs e)
        {
            if (!ac_Align.fn_CheckROIModelIntoSearch())
            {
                if(MessageBox.Show("Model ROI가 Search ROI에서 벗어나있습니다. 계속 진행 하시겠습니까?", "Warnning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            if (_frame_Main != null)
            {
                _frame_Main.Content = null;
                dg_ROI.SelectedIndex = -1;
                dg_Milling.SelectedIndex = -1;
                _SelectedModelIndex = -1;
            }
            fn_Update();
            delEndModfy?.Invoke();
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

        private void bn_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Filter = "Image File (*.bmp)|*.bmp";
            //g_VisionManager.fn_SetLightValue(swOn, EN_VISION_MODE.Polishing); //Light On
            if (fdlg.ShowDialog() == true)
            {
                ac_Align.OpenImage(fdlg.FileName);
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Live_Click(object sender, RoutedEventArgs e)
        @brief	Camera Live Run.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:16
        */
        private void bn_Live_Click(object sender, RoutedEventArgs e)
        {
            UserButton bn = sender as UserButton;
            if (bn != null)
            {
                if (g_VisionManager._CamManager.fn_GetGrabbingState())
                {
                    // Grab Stop
                    g_VisionManager._CamManager.fn_GrabStop();

                    bn.Content = "Live";
                    fn_EnableButton(true);

                    // Light Off.
                    g_VisionManager.fn_SetLightValue(0);
                }
                else
                {
                    fn_SetOpticCondition();
                    // Grab Image.
                    g_VisionManager._CamManager.fn_GrabStart(1);
                    bn.Content = "Stop";
                    fn_EnableButton(false);
                }
            }
        }

        private void fn_EnableButton(bool bEnable)
        {
            bn_Open.IsEnabled = bEnable;
            bn_OneShot.IsEnabled = bEnable;
            bn_Search.IsEnabled = bEnable;
            bn_SetMark.IsEnabled = bEnable;
            bn_SaveMarkImage.IsEnabled = bEnable;
            bn_SaveImg.IsEnabled = bEnable;
        }
        //---------------------------------------------------------------------------
        /**
        @fn     private void Page_Unloaded(object sender, RoutedEventArgs e)
        @brief	Page Unload 이벤트.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:16
        */
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Grab Stop
            g_VisionManager._CamManager.fn_GrabStop();
            // Light off.
            g_VisionManager.fn_SetLightValue(0);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_SaveImg_Click(object sender, RoutedEventArgs e)
        @brief	이미지 저장.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:18
        */
        private void bn_SaveImg_Click(object sender, RoutedEventArgs e)
        {
            // Image Path 변경 적용 할 것.
            // 아래는 참고용 코드.
            if (ac_Align.fn_GetImagePtr() != null)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Filter = "Image File (*.bmp)|*.bmp";
                dlg.AddExtension = true;
                dlg.Title = "Image Save";
                dlg.OverwritePrompt = true;
                if (dlg.ShowDialog() == true)
                {
                    ac_Align.fn_SaveImage(dlg.FileName);
                    MessageBox.Show($"{dlg.FileName}에 이미지가 저장 되었습니다.", "Image Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show($"취소 되었습니다.", "Image Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show($"이미지가 비었습니다.", "Image Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_WriteResult(string strResult)
        @brief	Search 결과 Listbox에 출력.
        @return	void
        @param	string strResult : 출력 내용.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:19
        */
        private void fn_WriteResult(string strResult)
        {
            if (listbox_ResultTest.Items.Count > 200)
                listbox_ResultTest.Items.RemoveAt(0);
            listbox_ResultTest.Items.Add(DateTime.Now.ToString("[hh:mm:ss] ") + strResult);
            listbox_ResultTest.SelectedIndex = listbox_ResultTest.Items.Count - 1;
            listbox_ResultTest.ScrollIntoView(listbox_ResultTest.SelectedItem);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void TextBox_WidthChange(object sender, TextChagedEventArgs e)
        @brief	Milling DataGrid Width 수정 이벤트.
        @return	void
        @param	object              sender
        @param  TextChagedEventArgs e
        @remark	
         - 수정시 Align Control(Image Control)에 수정된 위치 업데이트.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:24
        */
        private void TextBox_WidthChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int nSelIdx = dg_Milling.SelectedIndex;
            double dValue = 0.0;
            if (tb != null)
            {
                try
                {
                    dValue = Convert.ToDouble(tb.Text);

                    dValue = dValue * 1000 / g_VisionManager._RecipeVision.ResolutionX;
                    double dX = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Xp"]);
                    double dY = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Yp"]);
                    double dW = dValue;
                    double dH = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Hp"]);

                    ac_Align.fn_SetModelRect(new Rect(dX, dY, dW, dH));
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void TextBox_HeightChanged(object sender, TextChagedEventArgs e)
        @brief	Milling DataGrid Height 수정 이벤트.
        @return	void
        @param	object              sender
        @param  TextChagedEventArgs e
        @remark	
         - 수정시 Align Control(Image Control)에 수정된 위치 업데이트.
         - Pitch, Path Count로 Height 결정 함.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:24
        */
        private void TextBox_HeightChanged(object sender, TextChangedEventArgs e)
        {
//             TextBox tb = sender as TextBox;
//             int nSelIdx = dg_Milling.SelectedIndex;
//             double dValue = 0.0;
//             if (tb != null)
//             {
//                 try
//                 {
//                     dValue = Convert.ToDouble(tb.Text);
// 
//                     dValue = dValue * 1000 / g_VisionManager._RecipeVision.ResolutionY;
// 
//                     double dX = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Xp"]);
//                     double dY = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Yp"]);
//                     double dW = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Wp"]); 
//                     double dH = dValue;
// 
//                     ac_Align.fn_SetModelRect(new Rect(dX, dY, dW, dH));
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.WriteLine(ex.Message);
//                 }
//             }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void TextBox_PathCountChanged(object sender, TextChagedEventArgs e)
        @brief	Milling DataGrid PathCount 수정 이벤트.
        @return	void
        @param	object              sender
        @param  TextChagedEventArgs e
        @remark	
         - 수정시 Align Control(Image Control)에 수정된 위치 업데이트.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:24
        */
        private void TextBox_PathCountChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int nSelIdx = dg_Milling.SelectedIndex;
            double dValue = 0.0;
            if (tb != null)
            {
                try
                {
                    dValue = Convert.ToDouble(tb.Text);
                    if (dValue > 0)
                    {
                        double pitch = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Pitch"]);
                        pitch = pitch * 1000 / g_VisionManager._RecipeVision.ResolutionY;
                        dValue = pitch * (dValue * 2);

                        double dSX = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Xp"]);
                        double dSY = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Yp"]);
                        double dW = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Wp"]);
                        double dEY = dSY + Convert.ToDouble(_dtMil.Rows[nSelIdx]["Hp"]);

                        double dX = dSX;
                        double dY = dEY - dValue;

                        ac_Align.fn_SetModelRect(new Rect(dX, dY, dW, dValue));
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void TextBox_PitchChanged(object sender, TextChagedEventArgs e)
        @brief	Milling DataGrid Pitch 수정 이벤트.
        @return	void
        @param	object              sender
        @param  TextChagedEventArgs e
        @remark	
         - 수정시 Align Control(Image Control)에 수정된 위치 업데이트.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:24
        */
        private void TextBox_PitchChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int nSelIdx = dg_Milling.SelectedIndex;
            double dValue = 0.0;
            if (tb != null)
            {
                try
                {
                    dValue = Convert.ToDouble(tb.Text);
                    if (dValue > 0)
                    {
                        dValue = dValue * 1000 / g_VisionManager._RecipeVision.ResolutionY;

                        double pathcnt = Convert.ToDouble(_dtMil.Rows[nSelIdx]["PathCount"]);

                        dValue = pathcnt * (dValue * 2);

                        double dSX = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Xp"]);
                        double dSY = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Yp"]);
                        double dW = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Wp"]);
                        double dEY = dSY + Convert.ToDouble(_dtMil.Rows[nSelIdx]["Hp"]);

                        double dX = dSX;
                        double dY = dEY - dValue;

                        ac_Align.fn_SetModelRect(new Rect(dX, dY, dW, dValue));
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            }
        }

        private void TextBox_XChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int nSelIdx = dg_Milling.SelectedIndex;
            double dValue = 0.0;
            if (tb != null)
            {
                try
                {
                    double dModelCTX = Convert.ToDouble(_dtROI.Rows[0]["X"]) + (Convert.ToDouble(_dtROI.Rows[0]["Width"]) / 2.0);
                    double dMilCTX = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Xp"]) + (Convert.ToDouble(_dtMil.Rows[nSelIdx]["Wp"]) / 2.0);
                    double dGapX = dMilCTX - dModelCTX;
                    dValue = Convert.ToDouble(tb.Text);
                    // mm->px
                    dValue = dValue * 1000 / g_VisionManager._RecipeVision.ResolutionX;
                    // (model + mill) relative -> mill Relative pos
                    dValue = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Xp"]) + (dValue - dGapX);

                    double dX = dValue;
                    double dY = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Yp"]);
                    double dW = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Wp"]);
                    double dH = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Hp"]);

                    ac_Align.fn_SetModelRect(new Rect(dX, dY, dW, dH));
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            }
        }

        private void TextBox_YChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int nSelIdx = dg_Milling.SelectedIndex;
            double dValue = 0.0;
            if (tb != null)
            {
                try
                {
                    double dModelCTY = Convert.ToDouble(_dtROI.Rows[0]["Y"]) + (Convert.ToDouble(_dtROI.Rows[0]["Height"]) / 2.0);
                    double dMilCTY = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Yp"]) + (Convert.ToDouble(_dtMil.Rows[nSelIdx]["Hp"]) / 2.0);
                    double dGapY = dMilCTY - dModelCTY;
                    dValue = Convert.ToDouble(tb.Text);
                    dValue *= -1;
                    // mm->px
                    dValue = dValue * 1000 / UserClass.g_VisionManager._RecipeVision.ResolutionY;
                    // (model + mill) realtive -> mill Relative pos
                    dValue = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Yp"]) + (dValue - dGapY);
            
                    double dX = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Xp"]);
                    double dY = dValue;
                    double dW = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Wp"]);
                    double dH = Convert.ToDouble(_dtMil.Rows[nSelIdx]["Hp"]);
            
                    ac_Align.fn_SetModelRect(new Rect(dX, dY, dW, dH));
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            }
        }

        private void bn_MotionControl_Click(object sender, RoutedEventArgs e)
        {
            fn_UserJog();
        }

        private void bn_ViewEdge_Click(object sender, RoutedEventArgs e)
        {
            g_VisionManager._ImgProc.ViewEdge(ac_Mark, _ModelRecipe.ParamModel);
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

        private void bn_SetMark_Click(object sender, RoutedEventArgs e)
        {
            CroppedBitmap cb = ac_Align.GetModelImage();
            if (cb != null)
            {
                ac_Mark.SetImage(new WriteableBitmap(cb));
                ac_Mark.SetFitScale();
            }
        }

        private void ac_Mark_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            fn_Update();
            g_VisionManager._ImgProc.ViewEdge(ac_Mark, _ModelRecipe.ParamModel);
        }

        private void ac_Mark_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ac_Mark.fn_OriginalImage(true);
        }

        private void bn_ClearResult_Click(object sender, RoutedEventArgs e)
        {
            listbox_ResultTest.Items.Clear();
        }

        private void bn_SaveMark_Click(object sender, RoutedEventArgs e)
        {
            if (ac_Align.fn_GetImagePtr() != null)
            {
                if (MessageBox.Show($"Mark 이미지를 저장 하시겠습니까?", "Image Save", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _ModelRecipe.strImgPath = UserConstVision.STRRECIPEPATH + _SelectedRecipeName + "\\Model" + (_SelectedModelIndex + 1).ToString() + ".bmp";

                    ac_Align.fn_SaveImage(_ModelRecipe.strImgPath);
                    MessageBox.Show("저장 되었습니다.", "Image Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show("Image가 비어있습니다. Image를 확인해 주세요.", "Image Save", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SelectedROIResize_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if(ub != null)
            {
                int tag = Convert.ToInt32(ub.Tag);
                switch (tag)
                {
                    case 11:
                        _SelROI.X += _SelROI.Offset;
                        if (_SelROI.X > g_VisionManager._RecipeVision.CamWidth - _SelROI.W) _SelROI.X = g_VisionManager._RecipeVision.CamWidth - _SelROI.W;

                        break;
                    case 12:
                        _SelROI.X -= _SelROI.Offset;
                        if (_SelROI.X < 0) _SelROI.X = 0.0;
                        break;
                    case 21:
                        _SelROI.Y += _SelROI.Offset;
                        if (_SelROI.Y > g_VisionManager._RecipeVision.CamHeight - _SelROI.H) _SelROI.Y = g_VisionManager._RecipeVision.CamHeight - _SelROI.H;
                        break;
                    case 22:
                        _SelROI.Y -= _SelROI.Offset;
                        if (_SelROI.Y < 0) _SelROI.Y = 0.0;
                        break;
                    case 31:
                        _SelROI.W += _SelROI.Offset;
                        if (_SelROI.X + _SelROI.W > g_VisionManager._RecipeVision.CamWidth) _SelROI.W = g_VisionManager._RecipeVision.CamWidth - _SelROI.X;
                        break;
                    case 32:
                        _SelROI.W -= _SelROI.Offset;
                        if (_SelROI.W < 1) _SelROI.W = 1;
                        break;
                    case 41:
                        _SelROI.H += _SelROI.Offset;
                        if (_SelROI.Y + _SelROI.H > g_VisionManager._RecipeVision.CamHeight) _SelROI.H = g_VisionManager._RecipeVision.CamHeight - _SelROI.Y;
                        break;
                    case 42:
                        _SelROI.H -= _SelROI.Offset;
                        if (_SelROI.H < 1) _SelROI.H = 1;
                        break;
                }
                ac_Align.fn_SetModelRect(new Rect(_SelROI.X, _SelROI.Y, _SelROI.W, _SelROI.H));
            }
        }

        private void tb_Link_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;

            if (tb != null)
            {
                bLinkModelMil = tb.IsChecked == true ? true : false;
                ac_Align.fn_SetLink(bLinkModelMil);
            }
        }

        private void listbox_ResultText_Copy_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList list = listbox_ResultTest.SelectedItems;
            string strTemp, strClip = string.Empty;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    strTemp = list[i] as string;
                    if (strTemp != null)
                    {
                        strClip += strTemp;
                        strClip += "\n";
                    }
                }
                Clipboard.SetText(strClip);
            }
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