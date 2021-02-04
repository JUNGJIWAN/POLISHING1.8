using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WaferPolishingSystem.Define;
using UserInterface;
using System.Windows.Input;
using WaferPolishingSystem.Vision;
using static UserInterface.AlignControl;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunctionVision;
using System.Windows.Media;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageRecipe_Vision.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageRecipe_LoadingVision : Page
    {
        public del_EndModify delEndModfy = null;
        DataTable _dtROI = new DataTable();
        //string m_strRecipePath = STRRECIPEPATH;
        public int _SelectedModelIndex = -1;
        public string _SelectedModelName = "";
        public string _SelectedRecipeName = "";
        public Frame _frame_Main;

        public ModelList _ModelRecipe = new ModelList();

        public int[] GlobalLightIR;
        public int[] GlobalLightIRFilter;
        public int[] GlobalLightWhite;
        public int[] GlobalCameraExposure;
        public int[] GlobalCameraGain;

        Recipe_SelectROI _SelROI = new Recipe_SelectROI();

        public PageRecipe_LoadingVision()
        {
            InitializeComponent();
            ComponentDispatcher.ThreadIdle += UIInit;
            g_VisionManager._AlginPreCtrl = ac_Align;
            ac_Mark.fn_EnableDragButton(false);
            ac_Mark.SetMove(AlignControl.EnRoiMode.ModeMove);
        }

        private void UIInit(object sender, EventArgs args)
        {
            ComponentDispatcher.ThreadIdle -= UIInit;
            this.Background = G_COLOR_PAGEBACK;
            ac_Align.delAngle += del_Theta;
            ac_Align.delUpdateRect += fn_UpdateMilling;
            _dtROI.Columns.Add("NAME");
            _dtROI.Columns.Add("X");
            _dtROI.Columns.Add("Y");
            _dtROI.Columns.Add("WIDTH");
            _dtROI.Columns.Add("HEIGHT");

            _dtROI.Rows.Add("Model ROI", "0", "0", "0", "0");
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

            dg_ROI.SelectedIndex = 0;
        }

        private void del_Theta(double dTheta)
        {
            up_Theta.UPValue = dTheta.ToString("0.00");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lb_Recipe.Text = _SelectedRecipeName;
            bn_Live.Content = "Live";
            if (_SelectedModelIndex > -1)
                lb_Title.Text = _SelectedModelName;
            else
                lb_Title.Text = "-";

            if (g_VisionManager._CamManager._paramExposureRaw != null)
                uis_Exposuretime.Parameter = g_VisionManager._CamManager._paramExposureRaw;
            if (g_VisionManager._CamManager._paramGainRaw != null)
                uis_Gain.Parameter = g_VisionManager._CamManager._paramGainRaw;

            fn_LoadData();

            ac_Align.fn_SetResolution(g_VisionManager._RecipeVision.ResolutionX, g_VisionManager._RecipeVision.ResolutionY);
            if (!SEQ._bAuto)
                bn_OneShot_Click(bn_OneShot, null);

            fn_SetButton(EnRoiMode.ModeMove);
            fn_SetRuler(false);

            ac_Mark.SetFitScale();
            dg_ROI.ItemsSource = _dtROI.DefaultView;
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

        private void fn_LoadData()
        {
            try
            {
                double dValue = 0.0;
                Rect[] rect = new Rect[2];
                for (int i = 0; i < 2; i++)
                {
                    rect[i] = new Rect();
                }
// 
                rect[0] = new Rect(0, 0, _ModelRecipe.LoadingMarkWidth, _ModelRecipe.LoadingMarkHeight);
                _dtROI.Rows[0]["X"]         = rect[0].X     .ToString("0");
                _dtROI.Rows[0]["Y"]         = rect[0].Y     .ToString("0");
                _dtROI.Rows[0]["Width"]     = rect[0].Width .ToString("0");
                _dtROI.Rows[0]["Height"]    = rect[0].Height.ToString("0");

                rect[1].X                   = _ModelRecipe.LoadingParam.SearchOffsetX;
                rect[1].Y                   = _ModelRecipe.LoadingParam.SearchOffsetY;
                rect[1].Width               = _ModelRecipe.LoadingParam.SearchSizeX;
                rect[1].Height              = _ModelRecipe.LoadingParam.SearchSizeY;
                _dtROI.Rows[1]["X"]         = rect[1].X     .ToString("0");
                _dtROI.Rows[1]["Y"]         = rect[1].Y     .ToString("0");
                _dtROI.Rows[1]["Width"]     = rect[1].Width .ToString("0");
                _dtROI.Rows[1]["Height"]    = rect[1].Height.ToString("0");

                up_Theta.UPValue            = _ModelRecipe.LoadingTheta.ToString("0.00");
                up_Acceptance.UPValue       = _ModelRecipe.LoadingParam.Acceptance.ToString("0.00");
                up_Certainty.UPValue        = _ModelRecipe.LoadingParam.Certainty.ToString("0.00");
                cb_Detail.SelectedIndex     = _ModelRecipe.LoadingParam.DetailLevel;
                up_Smoothness.UPValue       = _ModelRecipe.LoadingParam.Smoothness.ToString("0.00");

                dValue = ((1.0 - _ModelRecipe.LoadingParam.ScaleMinFactor) + (_ModelRecipe.LoadingParam.ScaleMaxFactor - 1.0)) / 2.0;
                up_ScaleMargin.UPValue = (dValue * 100.0).ToString();
                dValue = (_ModelRecipe.LoadingParam.AngleDeltaPos + _ModelRecipe.LoadingParam.AngleDeltaNeg) / 2.0;
                up_AngleMargin.UPValue = dValue.ToString();

                //chk_AngleEnable.IsChecked   = _ModelRecipe.LoadingParam.SearchAngleRange == 1 ? true : false;
                //up_NegAngle.UPValue         = _ModelRecipe.LoadingParam.AngleDeltaNeg.ToString("0.00");
                //up_PosAngle.UPValue         = _ModelRecipe.LoadingParam.AngleDeltaPos.ToString("0.00");
                //
                //chk_ScaleEnable.IsChecked   = _ModelRecipe.LoadingParam.SearchScaleRange == 1 ? true : false;
                //up_MinScale.UPValue         = _ModelRecipe.LoadingParam.ScaleMinFactor.ToString("0.00");
                //up_MaxScale.UPValue         = _ModelRecipe.LoadingParam.ScaleMaxFactor.ToString("0.00");
                
                tg_ThetaEnable.Button.IsChecked = Convert.ToBoolean(_ModelRecipe.LoadingThetaEnable);

                if(_ModelRecipe.UseGlobalLoading == 1)
                {
                    gb_CamParam.Background = Brushes.Transparent;
                    gb_Light   .Background = Brushes.Transparent;
                    gb_CamParam.Header     = "Camera Parameter (Global)";
                    gb_Light   .Header     = "Light (Global)";
                    us_LightIR.USValue            = GlobalLightIR        [(int)EN_VISION_MODE.Loading];
                    us_LightW .USValue            = GlobalLightWhite     [(int)EN_VISION_MODE.Loading];
                    tg_shutter.Button.IsChecked   = GlobalLightIRFilter  [(int)EN_VISION_MODE.Loading] == 1 ? true : false ;
                    uis_Exposuretime.slider.Value = GlobalCameraExposure [(int)EN_VISION_MODE.Loading];
                    uis_Gain        .slider.Value = GlobalCameraGain     [(int)EN_VISION_MODE.Loading];
                }
                else
                {
                    gb_CamParam.Background = G_COLOR_SUBMENU;
                    gb_Light   .Background = G_COLOR_SUBMENU;
                    gb_CamParam.Header     = "Camera Parameter (Private)";
                    gb_Light   .Header     = "Light (Private)";
                    us_LightIR.USValue            = _ModelRecipe.LoadingLightIR;
                    us_LightW .USValue            = _ModelRecipe.LoadingLightWhite;
                    tg_shutter.Button.IsChecked   = _ModelRecipe.LoadingLightIRFilter == 1 ? true : false;
                    uis_Exposuretime.slider.Value = _ModelRecipe.LoadingCameraExposureTime;
                    uis_Gain        .slider.Value = _ModelRecipe.LoadingCameraGain;
                }

                ac_Align.SetRectangle(rect);
                ac_Mark.OpenImage(_ModelRecipe.strLoadingPath);
                ac_Mark.SetFitScale();
            }
            catch (Exception ex)
            {
                //fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        private void fn_InitData()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 1; j < _dtROI.Columns.Count; j++)
                {
                    _dtROI.Rows[i][j] = "0";
                }
            }
        }

        private void fn_EnableButton(bool bEnable)
        {
            bn_Open.IsEnabled = bEnable;
            bn_OneShot.IsEnabled = bEnable;
            bn_Search.IsEnabled = bEnable;
            bn_SetROI.IsEnabled = bEnable;
            bn_SaveMarkImage.IsEnabled = bEnable;
        }

        private void bn_Zoom_Click(object sender, RoutedEventArgs e)
        {
            double dTemp = ac_Align.GetZoomScale();

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
            if (bn_Theta.Background == G_COLOR_BTNCLICKED)
            {
                fn_SetButton(EnRoiMode.ModeDraw);
                ac_Align.ViewContent((int)EnObjectSelect.SelAngle    , false);
                ac_Align.ViewContent((int)EnObjectSelect.SelAngle + 1, false);
                ac_Align.ViewContent((int)EnObjectSelect.SelAngle + 2, false);
            }
            else
            {
                fn_SetButton(EnRoiMode.ModeTheta);
                ac_Align.ViewContent((int)EnObjectSelect.SelAngle    , true);
                ac_Align.ViewContent((int)EnObjectSelect.SelAngle + 1, true);
                ac_Align.ViewContent((int)EnObjectSelect.SelAngle + 2, true);
            }
        }

        private void fn_SetButton(EnRoiMode mode)
        {
            bn_Move.Background = G_COLOR_BTNNORMAL;
            bn_Draw.Background = G_COLOR_BTNNORMAL;
            bn_Theta.Background = G_COLOR_BTNNORMAL;

            ac_Align.SetMove(mode);
            switch (mode)
            {
                case EnRoiMode.ModeMove:
                    bn_Move.Background = G_COLOR_BTNCLICKED;
                    break;
                case EnRoiMode.ModeDraw:
                    bn_Draw.Background = G_COLOR_BTNCLICKED;
                    break;
                case EnRoiMode.ModeTheta:
                    bn_Theta.Background = G_COLOR_BTNCLICKED;
                    break;
                default:
                    break;
            }
        }

        private void cb_Detail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Detail.SelectedIndex != -1)
                _ModelRecipe.LoadingParam.DetailLevel = cb_Detail.SelectedIndex + 1;
        }

        private void bn_OneShot_Click(object sender, RoutedEventArgs e)
        {
            fn_SetOpticCondition();
            // Grab Image.
            g_VisionManager._CamManager.fn_GrabStart(0);
        }

        private void bn_Search_Click(object sender, RoutedEventArgs e)
        {
            ac_Align.fn_ClearResult();
            fn_Update();
            ST_ALIGN_RESULT stResult;
            try
            {
                WriteableBitmap wbmark = ac_Mark.fn_GetImageStream();
                WriteableBitmap wbimg = ac_Align.fn_GetImageStream();
                if(wbmark == null || wbimg== null)
                {
                    fn_WriteResult($"Search Fail : Invalid Image.");
                    return;
                }
                g_VisionManager._AlignManager.fn_SetMarkStream(wbmark);
                g_VisionManager._AlignManager.fn_SetImageStream(wbimg);
                g_VisionManager._AlignManager.fn_SetParameter(_ModelRecipe, true);
                g_VisionManager._AlignManager.fn_RunAlignment();
                stResult = g_VisionManager._AlignManager.fn_GetSearchResult();
                ac_Align.fn_SetResult(stResult);

                libDestroyModel();
            }
            catch (Exception ex)
            {
                fn_WriteResult($"Search Exception : {ex.Message}");
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
                fn_WriteResult($"X : {pntModelSpan.X:F3} mm, Y : {pntModelSpan.Y:F3} mm, Score : {Result.dScore:F3}, Angle : {Result.dAngle:F2}");

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
                fn_WriteResult($"Search Fail : {stResult.NumOfFound}");
        }
        private void fn_WriteResult(string strResult)
        {
            if (listbox_ResultTest.Items.Count > 200)
                listbox_ResultTest.Items.RemoveAt(0);
            listbox_ResultTest.Items.Add(DateTime.Now.ToString("[hh:mm:ss] ") + strResult);
            listbox_ResultTest.SelectedIndex = listbox_ResultTest.Items.Count - 1;
            listbox_ResultTest.ScrollIntoView(listbox_ResultTest.SelectedItem);
        }

        private void dg_ROI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                int nIdx = dg.SelectedIndex;
                if (nIdx > -1)
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

        private void bn_SetROI_Click(object sender, RoutedEventArgs e)
        {
            CroppedBitmap cb = ac_Align.GetModelImage();
            if (cb != null)
            {
                ac_Mark.SetImage(new WriteableBitmap(cb));
                ac_Mark.SetFitScale();
            }
        }
        private void fn_UpdateMilling(int idx, Rect rect)
        {
            try
            {
                _SelROI.X = rect.X;
                _SelROI.Y = rect.Y;
                _SelROI.W = rect.Width;
                _SelROI.H = rect.Height;

                if (idx < 2)
                {
                    _dtROI.Rows[idx]["X"]      = lb_SelROI_X.Content = rect.X.ToString("0.00");
                    _dtROI.Rows[idx]["Y"]      = lb_SelROI_Y.Content = rect.Y.ToString("0.00");
                    _dtROI.Rows[idx]["Width"]  = lb_SelROI_W.Content = rect.Width.ToString("0.00");
                    _dtROI.Rows[idx]["Height"] = lb_SelROI_H.Content = rect.Height.ToString("0.00");

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

                double.TryParse((string)_dtROI.Rows[0]["Width"], out dValue);
                _ModelRecipe.LoadingMarkWidth = (int)dValue;
                double.TryParse((string)_dtROI.Rows[0]["Height"], out dValue);
                _ModelRecipe.LoadingMarkHeight = (int)dValue;

                double.TryParse((string)_dtROI.Rows[1]["X"], out dValue);
                _ModelRecipe.LoadingParam.SearchOffsetX = (int)dValue;
                double.TryParse((string)_dtROI.Rows[1]["Y"], out dValue);
                _ModelRecipe.LoadingParam.SearchOffsetY = (int)dValue;
                double.TryParse((string)_dtROI.Rows[1]["Width"], out dValue);
                _ModelRecipe.LoadingParam.SearchSizeX   = (int)dValue;
                double.TryParse((string)_dtROI.Rows[1]["Height"], out dValue);
                _ModelRecipe.LoadingParam.SearchSizeY   = (int)dValue;

                _ModelRecipe.LoadingThetaEnable       = tg_ThetaEnable.Button.IsChecked == true ? 1 : 0;

                double.TryParse(up_Acceptance.UPValue, out dValue);
                _ModelRecipe.LoadingParam.Acceptance  = dValue;
                double.TryParse(up_Certainty.UPValue, out dValue);
                _ModelRecipe.LoadingParam.Certainty   = dValue;
                double.TryParse(up_Smoothness.UPValue, out dValue);
                _ModelRecipe.LoadingParam.Smoothness  = dValue;
                _ModelRecipe.LoadingParam.DetailLevel = cb_Detail.SelectedIndex;

                //_ModelRecipe.LoadingParam.SearchAngleRange = chk_AngleEnable.IsChecked == true ? 1 : 0;
                //double.TryParse(up_NegAngle.UPValue, out dValue);
                //_ModelRecipe.LoadingParam.AngleDeltaNeg = dValue;
                //double.TryParse(up_PosAngle.UPValue, out dValue);
                //_ModelRecipe.LoadingParam.AngleDeltaPos = dValue;
                //_ModelRecipe.LoadingParam.SearchScaleRange = chk_ScaleEnable.IsChecked == true ? 1 : 0;
                //double.TryParse(up_MinScale.UPValue, out dValue);
                //_ModelRecipe.LoadingParam.ScaleMinFactor = dValue;
                //double.TryParse(up_MaxScale.UPValue, out dValue);
                //_ModelRecipe.LoadingParam.ScaleMaxFactor = dValue;

                // Scale, Angle 강제 적용 및 파라메터 단일화 - 20210105
                _ModelRecipe.LoadingParam.SearchScaleRange = 1;
                double.TryParse(up_ScaleMargin.UPValue, out dValue);
                if (dValue == 0)
                {
                    dValue = UserConstVision.DEFAULT_SCALEMARGIN;
                    up_ScaleMargin.UPValue = dValue.ToString();
                }
                _ModelRecipe.LoadingParam.ScaleMinFactor = (100 - dValue) / 100.0;
                _ModelRecipe.LoadingParam.ScaleMaxFactor = (100 + dValue) / 100.0;

                _ModelRecipe.LoadingParam.SearchAngleRange = 1;
                double.TryParse(up_AngleMargin.UPValue, out dValue);
                if (dValue == 0)
                {
                    dValue = UserConstVision.DEFAULT_ANGLEMARGIN;
                    up_AngleMargin.UPValue = dValue.ToString();
                }
                _ModelRecipe.LoadingParam.AngleDeltaNeg = dValue;
                _ModelRecipe.LoadingParam.AngleDeltaPos = dValue;

                if (_ModelRecipe.UseGlobalLoading == 1)
                {
                    GlobalCameraExposure [(int)EN_VISION_MODE.Loading] = (int)uis_Exposuretime.slider.Value;
                    GlobalCameraGain     [(int)EN_VISION_MODE.Loading] = (int)uis_Gain        .slider.Value;
                    GlobalLightIRFilter  [(int)EN_VISION_MODE.Loading] = tg_shutter.Button.IsChecked == true ? 1 : 0;
                    GlobalLightIR        [(int)EN_VISION_MODE.Loading] = (int)us_LightIR.USValue;
                    GlobalLightWhite     [(int)EN_VISION_MODE.Loading] = (int)us_LightW .USValue;
                }
                else
                {
                    _ModelRecipe.LoadingCameraExposureTime = (int)uis_Exposuretime.slider.Value;
                    _ModelRecipe.LoadingCameraGain         = (int)uis_Gain        .slider.Value;
                    _ModelRecipe.LoadingLightIRFilter      = tg_shutter.Button.IsChecked == true ? 1 : 0;
                    _ModelRecipe.LoadingLightIR            = (int)us_LightIR.USValue;
                    _ModelRecipe.LoadingLightWhite         = (int)us_LightW .USValue;
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
                if(g_VisionManager._CamManager.fn_GetGrabbingState())
                {
                    // Grab Stop
                    g_VisionManager._CamManager.fn_GrabStop();

                    ub.Content = "Live";
                    fn_EnableButton(true);

                    // Light off.
                    g_VisionManager.fn_SetLightValue(0);
                }
                else
                {
                    fn_SetOpticCondition();
                    // Grab Live.
                    g_VisionManager._CamManager.fn_GrabStart(1);
                    ub.Content = "Stop";
                    fn_EnableButton(false);
                }
            }
        }

        private void up_Theta_UPValueChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                string strTheta = tb.Text;
                double dTheta = 0.0;
                double.TryParse(strTheta, out dTheta);
                ac_Align.SetTheta(dTheta);
                _ModelRecipe.LoadingTheta = Convert.ToDouble(up_Theta.UPValue);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Grab Stop
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
        //---------------------------------------------------------------------------
        private void bn_MoveVisn_Click(object sender, RoutedEventArgs e)
        {
            //Move Pre-Align Position
            SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X, EN_COMD_ID.User11);
            SEQ_TRANS.fn_ReqMoveVisnPos();


            //
            SEQ_SPIND.fn_MoveCylLensCvr(ccFwd);
            SEQ_SPIND.fn_MoveCylIR     (tg_shutter.Button.IsChecked == true ? ccBwd : ccFwd);

        }

        private void bn_Test_Click(object sender, RoutedEventArgs e)
        {
            g_VisionManager._CamManager.fn_SetSimulFrame(ac_Align.fn_GetImageStream());
            ST_VISION_RESULT vResult = new ST_VISION_RESULT();
            g_VisionManager.fn_PolishingAlign(ref vResult, true, true);
            fn_WriteResult($"X : {vResult.pntModel.X:F3} mm, Y : {vResult.pntModel.Y:F3} mm, Score : {vResult.dScore:F3}, Angle : {vResult.dTheta:F2}");
        }

        private void bn_Test2_Click(object sender, RoutedEventArgs e)
        {
            g_VisionManager._CamManager.fn_SetSimulFrame(ac_Align.fn_GetImageStream());
            ST_VISION_RESULT vResult = new ST_VISION_RESULT();
            g_VisionManager.fn_PreAlign(ref vResult, true);
            fn_WriteResult($"X : {vResult.pntModel.X:F3} mm, Y : {vResult.pntModel.Y:F3} mm, Score : {vResult.dScore:F3}, Angle : {vResult.dTheta:F2}");
        }

        private void ac_Mark_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            fn_Update();
            g_VisionManager._ImgProc.ViewEdge(ac_Mark, _ModelRecipe.LoadingParam);
        }

        private void ac_Mark_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ac_Mark.fn_OriginalImage(true);
        }

        private void bn_SaveMark_Click(object sender, RoutedEventArgs e)
        {
            if (ac_Mark.fn_GetImagePtr() != null)
            {
                if (MessageBox.Show($"Mark 이미지를 저장 하시겠습니까?", "Mark Save", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _ModelRecipe.strLoadingPath = UserConstVision.STRRECIPEPATH + _SelectedRecipeName + "\\Loading" + (_SelectedModelIndex + 1).ToString() + ".bmp";

                    ac_Mark.fn_SaveImage(_ModelRecipe.strLoadingPath, true);
                    MessageBox.Show("저장 되었습니다.", "Image Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show("Mark Image가 설정 되지 않았습니다. Mark Image를 설정하고 다시 시도해 주세요.", "Mark Save", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void bn_Reset_Click(object sender, RoutedEventArgs e)
        {
            int nIndex = dg_ROI.SelectedIndex;
            if (nIndex > -1)
            {
                fn_UpdateMilling(nIndex, new Rect(0, 0, 0, 0));
                ac_Align.fn_SetModelRect(new Rect(0, 0, 0, 0));
            }
        }

        private void SelectedROIResize_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
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

        private void bn_ClearResult_Click(object sender, RoutedEventArgs e)
        {
            listbox_ResultTest.Items.Clear();
        }

        private void listbox_ResultTest_Copy_Click(object sender, RoutedEventArgs e)
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
