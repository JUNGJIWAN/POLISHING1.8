using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using UserInterface;
using WaferPolishingSystem.Define;
using WaferPolishingSystem.Vision;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserConstVision;
using System.Threading;
using System.Windows.Threading;

namespace WaferPolishingSystem.Form
{

    // Convert Test Code
    public class BooleanConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as string == "False")
                value = "True";
            else
                value = "False";
            return (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
    public delegate void del_EditModelName(bool bEnable);
    public delegate void del_EndModify();
    /// <summary>
    /// PageRecipe_Polishing.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageRecipe_Polishing : Page
    {
        public del_EditModelName del_Edit = null;
        PageRecipe_LoadingVision    mc_LoadingVision    = new PageRecipe_LoadingVision();
        PageRecipe_PolishingVision  mc_PolishingVision  = new PageRecipe_PolishingVision();
        PageRecipe_InspectionVision mc_InspectionVision = new PageRecipe_InspectionVision();

        DataTable _dtModels = new DataTable();
        List<Recipe_MillingData> _listMillingData = new List<Recipe_MillingData>();

        ST_INSPECTION _insepction = new ST_INSPECTION(); // EPD Compare용 변수.

        int UseGlobalLoading    = 0;
        int UseGlobalPolishing  = 0;
        int UseGlobalInspection = 0;

        int[] LightIR           = new int[(int)EN_VISION_MODE.MemberCount];
        int[] LightIRFilter     = new int[(int)EN_VISION_MODE.MemberCount];
        int[] LightWhite        = new int[(int)EN_VISION_MODE.MemberCount];
        int[] CameraExposure    = new int[(int)EN_VISION_MODE.MemberCount];
        int[] CameraGain        = new int[(int)EN_VISION_MODE.MemberCount];
        
        ModelList _ModelRecipe = new ModelList();
        ModelList _CopyRecipe  = new ModelList();
        string m_strRecipeName = "";
        public Frame _frame_Main;
        public string _CurrentModelName = "";
        public int _SelectedIndex = -1;
        public int _SelMillingTab = 0;

        private bool bChanged = false;
        private bool bLoadRecipe = false;
        private bool bRollback = false;
        private List<string> listChange = new List<string>();

        //---------------------------------------------------------------------------
        /**
        @fn		public PageRecipe_Polishing()
        @brief  클래스 생성자.
        @return	
        @param	
        @remark
         -
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  19:42
        */
        public PageRecipe_Polishing()
        {
            InitializeComponent();

            mc_LoadingVision.delEndModfy += fn_UpdateLoadingData;
            mc_PolishingVision.delEndModfy += fn_UpdateMilData;
            mc_InspectionVision.delEndModfy += fn_UpdateEPDData;

            _dtModels.Columns.Add("INDEX");
            _dtModels.Columns.Add("ENABLE");
            _dtModels.Columns.Add("MODEL");
            _dtModels.Columns.Add("SEARCH");

            for(int i = 0; i < 10; i ++)
            {
                _dtModels.Rows.Add("Model " + (i+1), false, "Model" + (i+1).ToString(), i.ToString());
            }

            lv_Model.ItemsSource = _dtModels.DefaultView;

            for (int i = 0; i < 10; i++)
            {
                _ModelRecipe.Milling[i] = new MillingList();
            }
            
            grid_Setting.IsEnabled = false;
        }



        private void bnEPDEdit_Click(object sender, RoutedEventArgs e)
        {
            mc_InspectionVision._frame_Main          = _frame_Main;
            mc_InspectionVision._SelectedModelIndex  = _SelectedIndex;
            mc_InspectionVision._SelectedModelName   = _CurrentModelName;
            mc_InspectionVision._SelectedRecipe      = m_strRecipeName;
            mc_InspectionVision._ModelRecipe         = _ModelRecipe;

            mc_InspectionVision.GlobalLightIR        = LightIR;
            mc_InspectionVision.GlobalLightIRFilter  = LightIRFilter;
            mc_InspectionVision.GlobalLightWhite     = LightWhite;
            mc_InspectionVision.GlobalCameraExposure = CameraExposure;
            mc_InspectionVision.GlobalCameraGain     = CameraGain;

            _frame_Main.Content = mc_InspectionVision;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void bnLoadingModify_Click(object sender, RoutedEventArgs e)
        @brief	Loading Modify 클릭
        @return	void
        @param	object sender
        @param	RoutedEventArgs e
        @remark	
         - Loading Modify 클릭시 PageRecipe에 있는 fn_ViewLoading을 delegate로 호출.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  11:01
        */
        private void bnLoadingModify_Click(object sender, RoutedEventArgs e)
        {
            mc_LoadingVision._frame_Main            = _frame_Main;
            mc_LoadingVision._SelectedModelIndex    = _SelectedIndex;
            mc_LoadingVision._SelectedModelName     = _CurrentModelName;
            mc_LoadingVision._ModelRecipe           = _ModelRecipe;
            mc_LoadingVision._SelectedRecipeName    = m_strRecipeName;
            
            mc_LoadingVision.GlobalLightIR          = LightIR       ;
            mc_LoadingVision.GlobalLightIRFilter    = LightIRFilter ;
            mc_LoadingVision.GlobalLightWhite       = LightWhite    ;
            mc_LoadingVision.GlobalCameraExposure   = CameraExposure;
            mc_LoadingVision.GlobalCameraGain       = CameraGain    ;

            _frame_Main.Content                     = mc_LoadingVision;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void bnMillingModify_Click(object sender, RoutedEventArgs e)
        @brief	Milling Modify 클릭
        @return	void
        @param	object sender
        @param	RoutedEventArgs e
        @remark	
         - Milling Modify 클릭시 PageRecipe에 있는 fn_ViewPolishing을 delegate로 호출.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  11:01
        */
        private void bnMillingModify_Click(object sender, RoutedEventArgs e)
        {
            mc_PolishingVision._frame_Main          = _frame_Main;
            mc_PolishingVision._SelectedModelIndex  = _SelectedIndex;
            mc_PolishingVision._SelectedModelName   = _CurrentModelName;
            mc_PolishingVision._ModelRecipe         = _ModelRecipe;
            mc_PolishingVision._TabCount            = _listMillingData.Count;
            mc_PolishingVision._SelectedRecipeName  = m_strRecipeName;

            mc_PolishingVision.GlobalLightIR        = LightIR       ;
            mc_PolishingVision.GlobalLightIRFilter  = LightIRFilter ;
            mc_PolishingVision.GlobalLightWhite     = LightWhite    ;
            mc_PolishingVision.GlobalCameraExposure = CameraExposure;
            mc_PolishingVision.GlobalCameraGain     = CameraGain    ;

            _frame_Main.Content                     = mc_PolishingVision;
        }

        //---------------------------------------------------------------------------
        /**
        @fn		private void fn_UpdateModelData()
        @brief  Model 정보 Update
        @return	void
        @param	void
        @remark
         - Model UI 갱신.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  19:42
        */
        private void fn_UpdateModelData()
        {
            //---------------------------------------------------------------------------
            // Optic Global Condition
            for (int i = 0; i < (int)EN_VISION_MODE.MemberCount; i++)
            {
                LightIR       [i] = UserClass.g_VisionManager._RecipeModel.LightIR[i];
                LightIRFilter [i] = UserClass.g_VisionManager._RecipeModel.LightIRFilter[i];
                LightWhite    [i] = UserClass.g_VisionManager._RecipeModel.LightWhite[i];
                CameraExposure[i] = UserClass.g_VisionManager._RecipeModel.CameraExposureTime[i];
                CameraGain    [i] = UserClass.g_VisionManager._RecipeModel.CameraGain[i];
            }
            //---------------------------------------------------------------------------

            //---------------------------------------------------------------------------
            // Update Use Global Check Button
            UseGlobalLoading    = _ModelRecipe.UseGlobalLoading   ;
            UseGlobalPolishing  = _ModelRecipe.UseGlobalPolishing ;
            UseGlobalInspection = _ModelRecipe.UseGlobalInspection;

            cb_OpticGlobalLoading   .IsChecked = UseGlobalLoading    == 1 ? true : false;
            cb_OpticGlobalPolishing .IsChecked = UseGlobalPolishing  == 1 ? true : false;
            cb_OpticGlobalInspection.IsChecked = UseGlobalInspection == 1 ? true : false;
            //---------------------------------------------------------------------------

            up_ModelX.UPValue = _ModelRecipe.Model.X     .ToString("0.000");
            up_ModelY     .UPValue = _ModelRecipe.Model.Y     .ToString("0.000");
            up_ModelWidth .UPValue = _ModelRecipe.Model.Width .ToString("0.000");
            up_ModelHeight.UPValue = _ModelRecipe.Model.Height.ToString("0.000");

            switch(_ModelRecipe.Algorithm)
            {
                case EN_ALGORITHM.algModel:
                    up_SearchX     .UPValue = _ModelRecipe.ParamModel  .SearchOffsetX.ToString("0.000");
                    up_SearchY     .UPValue = _ModelRecipe.ParamModel  .SearchOffsetY.ToString("0.000");
                    up_SearchWidth .UPValue = _ModelRecipe.ParamModel  .SearchSizeX  .ToString("0.000");
                    up_SearchHeight.UPValue = _ModelRecipe.ParamModel  .SearchSizeY  .ToString("0.000");
                    break;
                case EN_ALGORITHM.algPattern:
                    up_SearchX     .UPValue = _ModelRecipe.ParamPattern.SearchOffsetX.ToString("0.000");
                    up_SearchY     .UPValue = _ModelRecipe.ParamPattern.SearchOffsetY.ToString("0.000");
                    up_SearchWidth .UPValue = _ModelRecipe.ParamPattern.SearchSizeX  .ToString("0.000");
                    up_SearchHeight.UPValue = _ModelRecipe.ParamPattern.SearchSizeY  .ToString("0.000");
                    break;
            }
            
            up_LoadingWidth .UPValue = _ModelRecipe.LoadingParam.SearchSizeX.ToString("0.000");
            up_LoadingHeight.UPValue = _ModelRecipe.LoadingParam.SearchSizeY.ToString("0.000");
            up_LoadingTheta .UPValue = _ModelRecipe.LoadingTheta.ToString("0.00");
            up_ThetaEnable.UPValue = Convert.ToBoolean(_ModelRecipe.LoadingThetaEnable).ToString();

            gb_Milling.IsEnabled = true;
            _listMillingData.Clear();
            // Update Data
            for (int j = 0; j < _ModelRecipe.MillingCount; j++)
            {
                _listMillingData.Add(new Recipe_MillingData());
                _listMillingData[j].GetDataFromRecipe(_ModelRecipe.Milling[j]);
            }
            // load inspection data
            _insepction = _ModelRecipe.Inspection;
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_LoadRecipe(string strName)
        @brief	Recipe 불러오기
        @return	void
        @param	string strName : Recipe Name
        @remark	
         - PageRecipe에서 Load시 Model DataTable에 데이터 Set
         - PageRecipe에서 Load시 Milling DataTable 0으로 Set
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  10:51
        */
        public void fn_LoadRecipe(string strName)
        {
            if (strName != null)
            {
                bRollback = false;
                bLoadRecipe = true;
                lv_Model.SelectedIndex = -1;
                //dg_Milling.SelectedIndex = -1;
                m_strRecipeName = strName;
                g_VisionManager._RecipeManager.fn_ReadRecipe("", m_strRecipeName);

                for (int i = 0; i < 10; i++)
                {
                    _dtModels.Rows[i]["Enable"] = Convert.ToBoolean(g_VisionManager._RecipeModel.Model[i].Enable);
                    _dtModels.Rows[i]["MODEL"]  = g_VisionManager._RecipeModel.Model[i].strName;
                    _dtModels.Rows[i]["SEARCH"] = g_VisionManager._RecipeModel.Model[i].SearchIndex;
                }

                bLoadRecipe = true;
                lv_Model.SelectedIndex = 0;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void SaveRecipe()
        @brief	Model 저장.
        @return	void
        @param	void
        @remark	
         - PageRecipe에서 저장 할 때 Model의 이름을 데이터에 업데이트.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  10:49
        */
        public void SaveRecipe(int nIdx)
        {
            try
            {
                for (int i = 0; i < (int)EN_VISION_MODE.MemberCount; i++)
                {
                    UserClass.g_VisionManager._RecipeModel.LightIR           [i] = LightIR       [i];
                    UserClass.g_VisionManager._RecipeModel.LightIRFilter     [i] = LightIRFilter [i];
                    UserClass.g_VisionManager._RecipeModel.LightWhite        [i] = LightWhite    [i];
                    UserClass.g_VisionManager._RecipeModel.CameraExposureTime[i] = CameraExposure[i];
                    UserClass.g_VisionManager._RecipeModel.CameraGain        [i] = CameraGain    [i];
                }
                if (nIdx > -1)
                {
                    _ModelRecipe.Enable = Convert.ToInt32(Convert.ToBoolean(_dtModels.Rows[nIdx]["ENABLE"]));
                    _ModelRecipe.strName = _dtModels.Rows[nIdx]["MODEL"].ToString();
                    _ModelRecipe.SearchIndex = Convert.ToInt32(_dtModels.Rows[nIdx]["SEARCH"]);

                    //_ModelRecipe.strImgPath = "";
                    //_ModelRecipe.strLoadingPath = "";
                    //---------------------------------------------------------------------------
                    _ModelRecipe.UseGlobalLoading    = UseGlobalLoading;
                    _ModelRecipe.UseGlobalPolishing  = UseGlobalPolishing;
                    _ModelRecipe.UseGlobalInspection = UseGlobalInspection;
                    //---------------------------------------------------------------------------
                    // Milling Data Save
                    _ModelRecipe.MillingCount = _listMillingData.Count;

                    for (int i = 0; i < 10; i++)
                    {
                        if( i < _listMillingData.Count)
                        { 
                            try
                            {
                                _ModelRecipe.Milling[i].Enable = Convert.ToInt32(_listMillingData[i].Enable);
                            } catch (Exception ex) { Console.WriteLine(ex.Message); }
                            _ModelRecipe.Milling[i].Pitch       = _listMillingData[i].Pitch;
                            _ModelRecipe.Milling[i].Cycle       = _listMillingData[i].Cycle;
                            _ModelRecipe.Milling[i].StartPos    = _listMillingData[i].StartPos;
                            _ModelRecipe.Milling[i].Tilt        = _listMillingData[i].Tilt;
                            _ModelRecipe.Milling[i].RPM         = _listMillingData[i].Rpm;
                            _ModelRecipe.Milling[i].Speed       = _listMillingData[i].Speed;
                            _ModelRecipe.Milling[i].Force       = _listMillingData[i].Force / ONEGRAM_TO_NEWTON;
                            _ModelRecipe.Milling[i].PathCount   = _listMillingData[i].PathCount;
                            _ModelRecipe.Milling[i].UtilType    = _listMillingData[i].UtilType;
                            _ModelRecipe.Milling[i].ToolType    = _listMillingData[i].ToolType;
                            try
                            {
                                _ModelRecipe.Milling[i].UtilFill = Convert.ToInt32(_listMillingData[i].UtilFill);
                            } catch (Exception ex) { Console.WriteLine(ex.Message); }
                            try
                            {
                                _ModelRecipe.Milling[i].UtilDrain = Convert.ToInt32(_listMillingData[i].UtilDrain);
                            } catch (Exception ex) { Console.WriteLine(ex.Message); }

                            try
                            {
                                _ModelRecipe.Milling[i].ToolChange = Convert.ToInt32(_listMillingData[i].ToolChange);
                            } catch (Exception ex) { Console.WriteLine(ex.Message); }
                            try
                            {
                                _ModelRecipe.Milling[i].MillingImage = Convert.ToInt32(_listMillingData[i].MillingImage);
                            } catch (Exception ex) { Console.WriteLine(ex.Message); }
                            try
                            {
                                _ModelRecipe.Milling[i].EPD = Convert.ToInt32(_listMillingData[i].EPD);
                            } catch (Exception ex) { Console.WriteLine(ex.Message); }
                        }
                        else
                        {
                            _ModelRecipe.Milling[i].Enable          = 0;
                            _ModelRecipe.Milling[i].Pitch           = 0;
                            _ModelRecipe.Milling[i].Cycle           = 0;
                            _ModelRecipe.Milling[i].StartPos        = 0;
                            _ModelRecipe.Milling[i].Tilt            = 0;
                            _ModelRecipe.Milling[i].RPM             = 0;
                            _ModelRecipe.Milling[i].Speed           = 0;
                            _ModelRecipe.Milling[i].Force           = 0;
                            _ModelRecipe.Milling[i].PathCount       = 0;
                            _ModelRecipe.Milling[i].UtilType        = 0;
                            _ModelRecipe.Milling[i].UtilFill        = 0;
                            _ModelRecipe.Milling[i].UtilDrain       = 0;
                            _ModelRecipe.Milling[i].ToolType        = 0;
                            _ModelRecipe.Milling[i].ToolChange      = 0;
                            _ModelRecipe.Milling[i].MillingImage    = 0;
                            _ModelRecipe.Milling[i].EPD             = 0;
                        }
                    }
                    _ModelRecipe.CopyTo(g_VisionManager._RecipeModel.Model[nIdx]);
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }

        }

        //---------------------------------------------------------------------------
        /**	
        @fn		void fn_UpdateMilData(int nIdx)
        @brief	EndModify시 호출해서 Milling Data Update
        @return	void
        @param	int nIdx : Recipe Index
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  10:38
        */
        public void fn_UpdateMilData()
        {
            if (_ModelRecipe != null)
            {
                //mc_PolishingVision._ModelRecipe.CopyTo(_ModelRecipe);
                up_ModelX.UPValue       = _ModelRecipe.Model .X     .ToString("0.000");
                up_ModelY.UPValue       = _ModelRecipe.Model .Y     .ToString("0.000");
                up_ModelWidth.UPValue   = _ModelRecipe.Model .Width .ToString("0.000");
                up_ModelHeight.UPValue  = _ModelRecipe.Model .Height.ToString("0.000");
                
                switch(_ModelRecipe.Algorithm)
                {
                    case EN_ALGORITHM.algModel:
                        up_SearchX     .UPValue = _ModelRecipe.ParamModel  .SearchOffsetX.ToString("0.00");
                        up_SearchY     .UPValue = _ModelRecipe.ParamModel  .SearchOffsetY.ToString("0.00");
                        up_SearchWidth .UPValue = _ModelRecipe.ParamModel  .SearchSizeX  .ToString("0.00");
                        up_SearchHeight.UPValue = _ModelRecipe.ParamModel  .SearchSizeY  .ToString("0.00");
                        break;
                    case EN_ALGORITHM.algPattern:
                        up_SearchX     .UPValue = _ModelRecipe.ParamPattern.SearchOffsetX.ToString("0.00");
                        up_SearchY     .UPValue = _ModelRecipe.ParamPattern.SearchOffsetY.ToString("0.00");
                        up_SearchWidth .UPValue = _ModelRecipe.ParamPattern.SearchSizeX  .ToString("0.00");
                        up_SearchHeight.UPValue = _ModelRecipe.ParamPattern.SearchSizeY  .ToString("0.00");
                        break;
                }

                for (int j = 0; j < _listMillingData.Count; j++)
                {
                    _listMillingData[j].GetDataFromRecipe(_ModelRecipe.Milling[j]);
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_UpdateLoadingData()
        @brief	Delegate For Loading Plate Information Update.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  19:42
        */
        private void fn_UpdateLoadingData()
        {
            try
            {
                //mc_LoadingVision._ModelRecipe.CopyTo(_ModelRecipe);
                up_LoadingWidth.UPValue  = _ModelRecipe.LoadingMarkWidth    .ToString("0.00");
                up_LoadingHeight.UPValue = _ModelRecipe.LoadingMarkHeight   .ToString("0.00");
                up_LoadingTheta.UPValue  = _ModelRecipe.LoadingTheta        .ToString("0.00");
                up_ThetaEnable.UPValue   = Convert.ToBoolean(_ModelRecipe.LoadingThetaEnable).ToString();
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        private void fn_UpdateEPDData()
        {
            try
            {
                //mc_InspectionVision._ModelRecipe.CopyTo(_ModelRecipe);
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }


        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Save_Click(obejct sender, RoutedEventArgs e)
        @brief	Model Save.
        @return	void
        @param	object          sender  :
        @param	RoutedEventArgs e       :
        @remark	
         - 모델 단위 저장.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:07
        */
        private void bn_Save_Click(object sender, RoutedEventArgs e)
        {
            
            if (MessageBox.Show($"모델을 저장 하시겠습니까?", "Model Save", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                bRollback = false;
                if (!fn_CheckOpticCondition())
                {
                    if (!FormMessage_Change.ShowChange($"다음 영상 조건으로 인해 이미지가 어둡게 나올 수 있습니다. 계속 진행 하시겠습니까?", "영상 획득 조건 경고", listChange, EN_MESSAGETYPE.WARNNING) == true)
                        bRollback = true;
                }
                if (!bRollback)
                {
                    FormMain.MAIN.frame.IsEnabled = false;
                    FormMain.MAIN.grid_button.IsEnabled = false;
                    FormMain.MAIN.Cursor = Cursors.Wait;

                    new Thread(new ThreadStart(delegate ()
                    {
                    //if(bSaveImg)
                    //{
                    //mc_PolishingVision.ac_Align.fn_SaveImage(_ModelRecipe.strImgPath);
                    //mc_LoadingVision.ac_Mark.fn_SaveImage(_ModelRecipe.strLoadingPath, true);
                    //}
                    SaveRecipe(_SelectedIndex);
                        g_VisionManager._RecipeManager.fn_WriteRecipe();
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                        {
                            FormMain.MAIN.Cursor = Cursors.Arrow;
                            FormMain.MAIN.frame.IsEnabled = true;
                            FormMain.MAIN.grid_button.IsEnabled = true;
                            MessageBox.Show($"모델 저장 성공.", "Model Save", MessageBoxButton.OK, MessageBoxImage.Information);
                        }));
                    })).Start();
                }
                else
                {
                    MessageBox.Show("취소되었습니다.", "Model Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("취소되었습니다.", "Model Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Copy_Click(object sender, RoutedEventArgs e)
        @brief	Model Copy.
        @return	void
        @param	object          sender  :
        @param	RoutedEventArgs e       :
        @remark	
         - 모델 복사.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:09
        */
        private void bn_Copy_Click(object sender, RoutedEventArgs e)
        {    
            int nIndex = lv_Model.SelectedIndex;
            if (nIndex > -1)
            {
                bn_Paste.IsEnabled = true;
                _ModelRecipe.CopyTo(_CopyRecipe);
                lb_Copyfrom.Content = _dtModels.Rows[nIndex]["MODEL"];
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Paste_Click(object sender, RoutedEventArgs e)
        @brief	Model Paste
        @return	void
        @param	object          sender  :
        @param	RoutedEventArgs e       :
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:10
        */
        private void bn_Paste_Click(object sender, RoutedEventArgs e)
        {
            int nIndex = lv_Model.SelectedIndex;
            if (nIndex > -1)
            {
                string strName = _ModelRecipe.strName;
                //bChanged = _ModelRecipe.Compare(_CopyRecipe);
                _CopyRecipe.CopyTo(_ModelRecipe);
                _ModelRecipe.strName = strName;
                _dtModels.Rows[_SelectedIndex]["ENABLE"] = _ModelRecipe.Enable == 1 ? "True" : "False";
                _dtModels.Rows[_SelectedIndex]["MODEL"] = _ModelRecipe.strName;
                fn_ClearStackpannel();
                for (int i = 0; i < _ModelRecipe.MillingCount; i++)
                {
                    bn_MillingListAdd_Click(null, null);
                }
                fn_UpdateModelData();
                fn_RefreshStackPannel();
                bn_SelectionMillingTab(sp_MillingList.Children[0], null);
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Del_Click(object sender, RoutedEventArgs e)
        @brief	Model Delete
        @return	void
        @param	object          sender  :
        @param	RoutedEventArgs e       :
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:11
        */
        private void bn_Del_Click(object sender, RoutedEventArgs e)
        {
            int nIndex = lv_Model.SelectedIndex;
            if (nIndex > -1)
            {
                string strName = _ModelRecipe.strName;
                ModelList temp = new ModelList();
                for(int i = 0;i < 10; i ++)
                {
                    temp.Milling[i] = new MillingList();
                }
                temp.CopyTo(_ModelRecipe);
                _ModelRecipe.strName = strName;
                fn_ClearStackpannel();
                fn_UpdateModelData();
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_Edit_Click(object sender, RoutedEventArgs e)
        @brief	Model Name Edit.
        @return	void
        @param	object          sender  :
        @param	RoutedEventArgs e       :
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:13
        */
        private void bn_Edit_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                string Mode = ub.Content as string;
                int nSel = lv_Model.SelectedIndex;
                if (nSel > -1)
                {
                    if (Mode == "Edit")
                    {
                        tb_SelModel.IsEnabled = true;
                        //cb_Search.IsEnabled = true;
                        tb_SelModel.Focus();
                        tb_SelModel.SelectAll();

                        // Disable DataGrid
                        fn_EditModeControlEnable(false);
                        ub.Content = "Done";
                    }
                    else
                    {
                        tb_SelModel.IsEnabled = false;
                        //cb_Search.IsEnabled = false;
                        lv_Model.Focus();

                        _dtModels.Rows[nSel]["MODEL"] = tb_SelModel.Text;

                        //_dtModels.Rows[nSel]["SEARCH"] = cb_Search.SelectedIndex + 1;
                        //---------------------------------------------------------------------------
                        // 무슨코드? 생각해 볼 것. => Search Index Code
                        //---------------------------------------------------------------------------                                                                       
                        for(int i = 0; i < _dtModels.Rows.Count; i++)
                        {
                            if (_dtModels.Rows[i]["ENABLE"] as string == "True")
                            {

                            }
                            else
                            {
                                //_dtModels.Rows[i]["SEARCH"] = 0;
                            }
                        }

                        fn_EditModeControlEnable(true);
                        ub.Content = "Edit";
                    }
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_EditModeControlEnable(bool bEnable)
        @brief	Model Name Edit Mode 일 때, Control Enable 제어.
        @return	void
        @param	bool bEnable : Enable 여부.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:19
        */
        private void fn_EditModeControlEnable(bool bEnable)
        {
            lv_Model.IsEnabled = bEnable;
            bn_Save.IsEnabled = bEnable;
            bn_Copy.IsEnabled = bEnable;
            //bn_Paste.IsEnabled = bEnable;
            bn_Del.IsEnabled = bEnable;
            gridModelInfo.IsEnabled = bEnable;
            gridMilling.IsEnabled = bEnable;
            del_Edit?.Invoke(bEnable);
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_SearchUp_Click(object sender, RoutedEventArgs e)
        @brief	Reserved Code for Search Index.
        @return	void
        @param	object          sender  :
        @param	RoutedEventArgs e       :
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:22
        */
        private void bn_SearchUp_Click(object sender, RoutedEventArgs e)
        {
            int idx = lv_Model.SelectedIndex;
            if(idx > 0)
            {
                string[] strPrev = new string[_dtModels.Columns.Count];
                string[] strCurr = new string[_dtModels.Columns.Count];
                for(int i = 0; i < strPrev.Length; i++)
                {
                    strPrev[i] = _dtModels.Rows[idx - 1][i] as string;
                }

                for(int i = 0; i < strCurr.Length; i++)
                {
                    strCurr[i] = _dtModels.Rows[idx][i] as string;
                }

                for (int i = 0; i < _dtModels.Columns.Count; i++)
                {
                    _dtModels.Rows[idx][i] = strPrev[i];
                    _dtModels.Rows[idx - 1][i] = strCurr[i];
                }
                lv_Model.SelectedIndex = idx - 1;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_SearchDown_Click(object sender, RoutedEventArgs e)
        @brief	Reserved Code for SearchIndex.
        @return	void
        @param	object          sender  :
        @param	RoutedEventArgs e       :
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:23
        */
        private void bn_SearchDown_Click(object sender, RoutedEventArgs e)
        {
            int idx = lv_Model.SelectedIndex;
            if (idx > -1 && idx < 9)
            {
                string[] strPrev = new string[_dtModels.Columns.Count];
                string[] strCurr = new string[_dtModels.Columns.Count];
                for (int i = 0; i < strPrev.Length; i++)
                {
                    strPrev[i] = _dtModels.Rows[idx + 1][i] as string;
                }

                for (int i = 0; i < strCurr.Length; i++)
                {
                    strCurr[i] = _dtModels.Rows[idx][i] as string;
                }

                for (int i = 0; i < _dtModels.Columns.Count; i++)
                {
                    _dtModels.Rows[idx][i] = strPrev[i];
                    _dtModels.Rows[idx + 1][i] = strCurr[i];
                }
                lv_Model.SelectedIndex = idx + 1;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void lv_Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        @brief	ListView Model Item 선택 변경
        @return	void
        @param	object                      sender
        @param	SelectionChangedEventArgs   e
        @remark	
         - Data Grid의 Model 선택 Index가 바뀔 때 데이터 Set.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/4  10:59
        */
        private void lv_Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = (sender as ListView);
            if (lv != null)
            {
                // Check Recipe
                if (_SelectedIndex != lv.SelectedIndex && _SelectedIndex > -1 && !bLoadRecipe)
                {
                    bRollback = false;
                    bChanged |= fn_CompareRecipe();
                    if (bChanged)
                    {
                        if (FormMessage_Change.ShowChange("항목에 변경 사항이 있습니다. 변경 항목을 저장 하시겠습니까?", "Recipe 항목 변경 안내", listChange))
                        {
                            if (!fn_CheckOpticCondition())
                            {
                                if (!FormMessage_Change.ShowChange($"다음 영상 조건으로 인해 이미지가 어둡게 나올 수 있습니다. 계속 진행 하시겠습니까?", "영상 획득 조건 경고", listChange, EN_MESSAGETYPE.WARNNING) == true)
                                {
                                    // 데이터 원본 
                                    bRollback = true;
                                    lv.SelectedIndex = _SelectedIndex;
                                    MessageBox.Show("모델 이동이 취소 되었습니다.", "ModelSave", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }

                            if (!bRollback)
                            {
                                FormMain.MAIN.frame.IsEnabled = false;
                                FormMain.MAIN.grid_button.IsEnabled = false;
                                FormMain.MAIN.Cursor = Cursors.Wait;

                                fn_WriteLog(this.Title + $"_Recipe 변경 : {g_VisionManager._RecipeModel.strRecipeName} - {_ModelRecipe.strName} (idx : {_SelectedIndex})", UserEnum.EN_LOG_TYPE.ltTrace);
                                for (int i = 0; i < listChange?.Count; i++)
                                {
                                    fn_WriteLog(this.Title + $" {listChange[i]}", UserEnum.EN_LOG_TYPE.ltTrace);
                                }
                                new Thread(new ParameterizedThreadStart(delegate (object obj)
                                {
                                    int[] idx = (int[])obj;
                                    SaveRecipe(idx[0]);
                                    g_VisionManager._RecipeManager.fn_WriteRecipe();
                                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                                    {
                                        FormMain.MAIN.Cursor = Cursors.Arrow;
                                        FormMain.MAIN.frame.IsEnabled = true;
                                        FormMain.MAIN.grid_button.IsEnabled = true;
                                        lv_Model.SelectedIndex = idx[1];
                                        lv_Model_SelectionChanged(lv_Model, null);
                                        MessageBox.Show($"모델 저장 성공.", "Model Save", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }));
                                })).Start(new int[] { _SelectedIndex, lv.SelectedIndex });
                            }
                        }
                        else
                        {
                            _dtModels.Rows[_SelectedIndex]["ENABLE"] = UserClass.g_VisionManager._RecipeModel.Model[_SelectedIndex].Enable == 1 ? "True" : "False";
                            _dtModels.Rows[_SelectedIndex]["MODEL"] = UserClass.g_VisionManager._RecipeModel.Model[_SelectedIndex].strName;
                        }
                        bChanged = false;
                    }
                }
                if (!bRollback)
                {
                    _SelectedIndex = lv.SelectedIndex;
                    if (_SelectedIndex > -1)
                    {
                        grid_Setting.IsEnabled = true;
                        

                        g_VisionManager._RecipeModel.Model[_SelectedIndex].CopyTo(_ModelRecipe);
                        _CurrentModelName = _dtModels.Rows[_SelectedIndex]["MODEL"].ToString();

                        tb_SelModel.Text = _CurrentModelName;

                        //
                        fn_ClearStackpannel();
                        if (_ModelRecipe.MillingCount == 0)
                        {
                            _ModelRecipe.MillingCount = 1;
                            _ModelRecipe.Milling[0].ToolChange = 1;
                            _ModelRecipe.Milling[0].Cycle = 1;
                        }
                        for (int i = 0; i < _ModelRecipe.MillingCount; i++)
                        {
                            bn_MillingListAdd_Click(null, null);
                        }
                        fn_UpdateModelData();


                        // addData;
                        mc_LoadingVision.ac_Mark.ClearCanvas();
                        mc_LoadingVision.ac_Align.ClearCanvas();
                        mc_PolishingVision.ac_Mark.ClearCanvas();
                        mc_PolishingVision.ac_Align.ClearCanvas();
                        mc_InspectionVision.ac_Align.ClearCanvas();
                        mc_InspectionVision.ac_RefImage.ClearCanvas();

                        //Sel Mil1 Tab
                        bn_SelectionMillingTab(sp_MillingList.Children[0], null);
                        bLoadRecipe = false;
                    }
                    else
                    {
                        grid_Setting.IsEnabled = false;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private bool fn_CompareRecipe()
        @brief	Recipe 수정 여부 확인.
        @return	bool : 다르면 True, 같으면 False
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/6  20:24
        */
        private bool fn_CompareRecipe()
        {
            bool bRtn = false;
            listChange.Clear();

            
            if (_listMillingData.Count == _ModelRecipe.MillingCount)
            {
                try
                {
                    for (int i = 0; i < _listMillingData.Count; i++)
                    {

                        listChange?.Add($"Milling Step : {i + 1}");
                        bool bCmp = _listMillingData[i].Compare(g_VisionManager._RecipeModel.Model[_SelectedIndex].Milling[i], listChange);
                        if (!bCmp) listChange?.RemoveAt(listChange.Count - 1);
                        bRtn |= bCmp;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                if (_ModelRecipe.MillingCount == 0)
                    bRtn = false;
                else
                {
                    listChange?.Add($"Step Chagend : {_ModelRecipe.MillingCount} -> {_listMillingData.Count}");
                    bRtn = true;
                }
            }

            bRtn |= _ModelRecipe.UseGlobalLoading             != g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalLoading            ; if(_ModelRecipe.UseGlobalLoading             != g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalLoading            ) listChange?.Add($"UseGlobalLoading : {            Convert.ToBoolean(g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalLoading    )        } -> {Convert.ToBoolean(_ModelRecipe.UseGlobalLoading   )         }");
            bRtn |= _ModelRecipe.UseGlobalPolishing           != g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalPolishing          ; if(_ModelRecipe.UseGlobalPolishing           != g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalPolishing          ) listChange?.Add($"UseGlobalPolishing : {          Convert.ToBoolean(g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalPolishing  )        } -> {Convert.ToBoolean(_ModelRecipe.UseGlobalPolishing )         }");
            bRtn |= _ModelRecipe.UseGlobalInspection          != g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalInspection         ; if(_ModelRecipe.UseGlobalInspection          != g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalInspection         ) listChange?.Add($"UseGlobalInspection : {         Convert.ToBoolean(g_VisionManager._RecipeModel.Model[_SelectedIndex].UseGlobalInspection )        } -> {Convert.ToBoolean(_ModelRecipe.UseGlobalInspection)         }");
            
            for (int i = 0; i < (int)EN_VISION_MODE.MemberCount; i++)
            {
                bRtn |= g_VisionManager._RecipeModel.CameraExposureTime [i] != CameraExposure[i]; if (g_VisionManager._RecipeModel.CameraExposureTime [i] != CameraExposure[i]) listChange?.Add($"GlobalOpticCondition{(EN_VISION_MODE)i} CameraExposure :{g_VisionManager._RecipeModel.CameraExposureTime [i]} -> {CameraExposure[i]}");
                bRtn |= g_VisionManager._RecipeModel.CameraGain         [i] != CameraGain    [i]; if (g_VisionManager._RecipeModel.CameraGain         [i] != CameraGain    [i]) listChange?.Add($"GlobalOpticCondition{(EN_VISION_MODE)i} CameraGain :{    g_VisionManager._RecipeModel.CameraGain         [i]} -> {CameraGain    [i]}");
                bRtn |= g_VisionManager._RecipeModel.LightIR            [i] != LightIR       [i]; if (g_VisionManager._RecipeModel.LightIR            [i] != LightIR       [i]) listChange?.Add($"GlobalOpticCondition{(EN_VISION_MODE)i} LightIR :{       g_VisionManager._RecipeModel.LightIR            [i]} -> {LightIR       [i]}");
                bRtn |= g_VisionManager._RecipeModel.LightIRFilter      [i] != LightIRFilter [i]; if (g_VisionManager._RecipeModel.LightIRFilter      [i] != LightIRFilter [i]) listChange?.Add($"GlobalOpticCondition{(EN_VISION_MODE)i} LightIRFilter :{ g_VisionManager._RecipeModel.LightIRFilter      [i]} -> {LightIRFilter [i]}");
                bRtn |= g_VisionManager._RecipeModel.LightWhite         [i] != LightWhite    [i]; if (g_VisionManager._RecipeModel.LightWhite         [i] != LightWhite    [i]) listChange?.Add($"GlobalOpticCondition{(EN_VISION_MODE)i} LightWhite :{    g_VisionManager._RecipeModel.LightWhite         [i]} -> {LightWhite    [i]}");
            }

            //---------------------------------------------------------------------------
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
            bRtn |= _ModelRecipe.LoadingLightIR               != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightIR              ; if(_ModelRecipe.LoadingLightIR               != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightIR              ) listChange?.Add($"LoadingLightIR : {              g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightIR              } -> {_ModelRecipe.LoadingLightIR              }");
            bRtn |= _ModelRecipe.LoadingLightWhite            != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightWhite           ; if(_ModelRecipe.LoadingLightWhite            != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightWhite           ) listChange?.Add($"LoadingLightWhite : {           g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightWhite           } -> {_ModelRecipe.LoadingLightWhite           }");
            bRtn |= _ModelRecipe.LoadingLightIRFilter         != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightIRFilter        ; if(_ModelRecipe.LoadingLightIRFilter         != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightIRFilter        ) listChange?.Add($"LoadingLightIRFilter : {        g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingLightIRFilter        } -> {_ModelRecipe.LoadingLightIRFilter        }");
            bRtn |= _ModelRecipe.LoadingCameraExposureTime    != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingCameraExposureTime   ; if(_ModelRecipe.LoadingCameraExposureTime    != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingCameraExposureTime   ) listChange?.Add($"LoadingCameraExposureTime : {   g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingCameraExposureTime   } -> {_ModelRecipe.LoadingCameraExposureTime   }");
            bRtn |= _ModelRecipe.LoadingCameraGain            != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingCameraGain           ; if(_ModelRecipe.LoadingCameraGain            != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingCameraGain           ) listChange?.Add($"LoadingCameraGain : {           g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingCameraGain           } -> {_ModelRecipe.LoadingCameraGain           }");
                                                                                                                                                                                                                                                                                                                                     
            bRtn |= _ModelRecipe.PolishingLightIR             != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightIR            ; if(_ModelRecipe.PolishingLightIR             != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightIR            ) listChange?.Add($"PolishingLightIR : {            g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightIR            } -> {_ModelRecipe.PolishingLightIR            }");
            bRtn |= _ModelRecipe.PolishingLightWhite          != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightWhite         ; if(_ModelRecipe.PolishingLightWhite          != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightWhite         ) listChange?.Add($"PolishingLightWhite : {         g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightWhite         } -> {_ModelRecipe.PolishingLightWhite         }");
            bRtn |= _ModelRecipe.PolishingLightIRFilter       != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightIRFilter      ; if(_ModelRecipe.PolishingLightIRFilter       != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightIRFilter      ) listChange?.Add($"PolishingLightIRFilter : {      g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingLightIRFilter      } -> {_ModelRecipe.PolishingLightIRFilter      }");
            bRtn |= _ModelRecipe.PolishingCameraExposureTime  != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingCameraExposureTime ; if(_ModelRecipe.PolishingCameraExposureTime  != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingCameraExposureTime ) listChange?.Add($"PolishingCameraExposureTime : { g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingCameraExposureTime } -> {_ModelRecipe.PolishingCameraExposureTime }");
            bRtn |= _ModelRecipe.PolishingCameraGain          != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingCameraGain         ; if(_ModelRecipe.PolishingCameraGain          != g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingCameraGain         ) listChange?.Add($"PolishingCameraGain : {         g_VisionManager._RecipeModel.Model[_SelectedIndex].PolishingCameraGain         } -> {_ModelRecipe.PolishingCameraGain         }");
                                                                                                                                                                                                                                                                                   
            bRtn |= _ModelRecipe.InspectionLightIR            != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightIR           ; if(_ModelRecipe.InspectionLightIR            != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightIR           ) listChange?.Add($"InspectionLightIR : {           g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightIR           } -> {_ModelRecipe.InspectionLightIR           }");
            bRtn |= _ModelRecipe.InspectionLightWhite         != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightWhite        ; if(_ModelRecipe.InspectionLightWhite         != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightWhite        ) listChange?.Add($"InspectionLightWhite : {        g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightWhite        } -> {_ModelRecipe.InspectionLightWhite        }");
            bRtn |= _ModelRecipe.InspectionLightIRFilter      != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightIRFilter     ; if(_ModelRecipe.InspectionLightIRFilter      != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightIRFilter     ) listChange?.Add($"InspectionLightIRFilter : {     g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionLightIRFilter     } -> {_ModelRecipe.InspectionLightIRFilter     }");
            bRtn |= _ModelRecipe.InspectionCameraExposureTime != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionCameraExposureTime; if(_ModelRecipe.InspectionCameraExposureTime != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionCameraExposureTime) listChange?.Add($"InspectionCameraExposureTime : {g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionCameraExposureTime} -> {_ModelRecipe.InspectionCameraExposureTime}");
            bRtn |= _ModelRecipe.InspectionCameraGain         != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionCameraGain        ; if(_ModelRecipe.InspectionCameraGain         != g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionCameraGain        ) listChange?.Add($"InspectionCameraGain : {        g_VisionManager._RecipeModel.Model[_SelectedIndex].InspectionCameraGain        } -> {_ModelRecipe.InspectionCameraGain        }");

            //---------------------------------------------------------------------------
            bRtn |= Math.Round(_ModelRecipe.Model.X     , 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.X     , 3); if (Math.Round(_ModelRecipe.Model.X     , 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.X     , 3)) listChange?.Add($"Model ROI X : {Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.X     , 3)} -> {Math.Round(_ModelRecipe.Model.X     , 3)}");
            bRtn |= Math.Round(_ModelRecipe.Model.Y     , 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Y     , 3); if (Math.Round(_ModelRecipe.Model.Y     , 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Y     , 3)) listChange?.Add($"Model ROI Y : {Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Y     , 3)} -> {Math.Round(_ModelRecipe.Model.Y     , 3)}");
            bRtn |= Math.Round(_ModelRecipe.Model.Width , 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Width , 3); if (Math.Round(_ModelRecipe.Model.Width , 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Width , 3)) listChange?.Add($"Model ROI W : {Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Width , 3)} -> {Math.Round(_ModelRecipe.Model.Width , 3)}");
            bRtn |= Math.Round(_ModelRecipe.Model.Height, 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Height, 3); if (Math.Round(_ModelRecipe.Model.Height, 3)   != Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Height, 3)) listChange?.Add($"Model ROI H : {Math.Round(g_VisionManager._RecipeModel.Model[_SelectedIndex].Model.Height, 3)} -> {Math.Round(_ModelRecipe.Model.Height, 3)}");

            bRtn |= _ModelRecipe.Algorithm                  != g_VisionManager._RecipeModel.Model[_SelectedIndex].Algorithm; if (_ModelRecipe.Algorithm != g_VisionManager._RecipeModel.Model[_SelectedIndex].Algorithm) listChange?.Add($"Algorithm : {g_VisionManager._RecipeModel.Model[_SelectedIndex].Algorithm} -> {_ModelRecipe.Algorithm}");

            bRtn |= _ModelRecipe.ParamModel  .Compare(g_VisionManager._RecipeModel.Model[_SelectedIndex].ParamModel  , listChange);
            bRtn |= _ModelRecipe.ParamPattern.Compare(g_VisionManager._RecipeModel.Model[_SelectedIndex].ParamPattern, listChange);

            //---------------------------------------------------------------------------
            // Pre Align Compare
            bRtn |= _ModelRecipe.LoadingMarkWidth   != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingMarkWidth   ; if(_ModelRecipe.LoadingMarkWidth   != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingMarkWidth  ) listChange?.Add($"LoadingMarkWidth : {  g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingMarkWidth  } -> {_ModelRecipe.LoadingMarkWidth  }");
            bRtn |= _ModelRecipe.LoadingMarkHeight  != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingMarkHeight  ; if(_ModelRecipe.LoadingMarkHeight  != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingMarkHeight ) listChange?.Add($"LoadingMarkHeight : { g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingMarkHeight } -> {_ModelRecipe.LoadingMarkHeight }");
            bRtn |= _ModelRecipe.LoadingTheta       != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingTheta       ; if(_ModelRecipe.LoadingTheta       != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingTheta      ) listChange?.Add($"LoadingTheta : {      g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingTheta      } -> {_ModelRecipe.LoadingTheta      }");
            bRtn |= _ModelRecipe.LoadingThetaEnable != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingThetaEnable ; if(_ModelRecipe.LoadingThetaEnable != g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingThetaEnable) listChange?.Add($"LoadingThetaEnable : {g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingThetaEnable} -> {_ModelRecipe.LoadingThetaEnable}");
            bRtn |= _ModelRecipe.LoadingParam.Compare(g_VisionManager._RecipeModel.Model[_SelectedIndex].LoadingParam, listChange);

            //---------------------------------------------------------------------------
            // EPD Compare
            bRtn |= _ModelRecipe.Inspection.Compare(g_VisionManager._RecipeModel.Model[_SelectedIndex].Inspection, listChange);

            bRtn |= g_VisionManager._RecipeModel.Model[_SelectedIndex].Enable != Convert.ToInt32(Convert.ToBoolean(_dtModels.Rows[_SelectedIndex]["ENABLE"]));
            if (g_VisionManager._RecipeModel.Model[_SelectedIndex].Enable != Convert.ToInt32(Convert.ToBoolean(_dtModels.Rows[_SelectedIndex]["ENABLE"])))
                listChange?.Add($"Model Enable : {Convert.ToBoolean(g_VisionManager._RecipeModel.Model[_SelectedIndex].Enable)} -> {Convert.ToBoolean(_dtModels.Rows[_SelectedIndex]["ENABLE"])}");
            bRtn |= g_VisionManager._RecipeModel.Model[_SelectedIndex].strName != _dtModels.Rows[_SelectedIndex]["MODEL"].ToString();
            if (g_VisionManager._RecipeModel.Model[_SelectedIndex].strName != _dtModels.Rows[_SelectedIndex]["MODEL"].ToString())
                listChange?.Add($"Model Name : {g_VisionManager._RecipeModel.Model[_SelectedIndex].strName} -> {_dtModels.Rows[_SelectedIndex]["MODEL"]}");

            return bRtn;
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_MillingListAdd_Click(object sender, RoutedEventArgs e)
        @brief	Milling Tab 추가.
        @return	void
        @param	object                      sender
        @param	SelectionChangedEventArgs   e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  8:37
        */
        private void bn_MillingListAdd_Click(object sender, RoutedEventArgs e)
        {
            int nspCount = sp_MillingList.Children.Count;
            if(nspCount < 11)
            {
                UserButton ubClose = new UserButton();
                ubClose.Content = "X";
                ubClose.Background = Brushes.Transparent;
//                 ubClose.Background = Brushes.Maroon;
//                 ubClose.Foreground = Brushes.White;
                ubClose.FontWeight = FontWeights.Bold;
                ubClose.MouseOverBackgroundBrush = Brushes.Magenta;
                ubClose.BorderThickness = new Thickness(0);
                ubClose.CornerRadius = new CornerRadius(5);
                ubClose.HorizontalAlignment = HorizontalAlignment.Right;
                ubClose.FontSize = 12;
                ubClose.Width  = 20;
                ubClose.Height = 20;
                ubClose.Click += bn_DeleteMillingTab;
                ubClose.CommandParameter = nspCount - 1;

                Label tb = new Label();
                tb.Content = "Mil " + nspCount.ToString("00");
                tb.Foreground = Brushes.Gray;
                tb.FontWeight = FontWeights.Normal;
                tb.FontSize = 14;

                Grid grd = new Grid();
                grd.Children.Add(tb);
                grd.Children.Add(ubClose);
                
                UserButton ubTab = new UserButton();
                ubTab.Content = grd;
                ubTab.Width = 95;
                ubTab.CornerRadius = new CornerRadius(5, 5, 0, 0);
                ubTab.Margin = new Thickness(0, 0, 2, -1);
                ubTab.CommandParameter = nspCount - 1;
                ubTab.BorderBrush = Brushes.DarkGray;
                ubTab.Click += bn_SelectionMillingTab;

                grd.Width = ubTab.Width - 5;

                sp_MillingList.Children.Insert(sp_MillingList.Children.Count - 1, ubTab);
                                
                _listMillingData.Add(new Recipe_MillingData());

                bn_SelectionMillingTab(ubTab, e);
                sv_MillingTab.ScrollToRightEnd();
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_SelectionMillingTab(object sender, EventArgs e)
        @brief	
        @return	void
        @param	object                      sender
        @param	SelectionChangedEventArgs   e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  8:39
        */
        private void bn_SelectionMillingTab(object sender, EventArgs e)
        {
            UserButton ub = sender as UserButton;
            if(ub != null)
            {
                if (sp_MillingList.Children.Count > 1)
                {
                    try
                    {
                        fn_RefreshStackPannel();
                        _SelMillingTab = Convert.ToInt32(ub.CommandParameter);
                        ub.Background = Brushes.White;
                        ub.BorderThickness = new Thickness(1, 1, 1, 0);
                        ub.FontWeight = FontWeights.Bold;
                        ((ub.Content as Grid).Children[0] as Label).Foreground = Brushes.Black;
                        // Check - Milling 삭제할 때 버그 있음. 확인할 것
                        bd_Milling.DataContext = _listMillingData[_SelMillingTab];

                        if (_SelMillingTab == 0)
                        {
                            _listMillingData[_SelMillingTab].ToolChange = true;
                        }
                        Console.WriteLine($"Selected : {ub.CommandParameter}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    bd_Milling.DataContext = new Recipe_MillingData();
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void bn_DeleteMillingTab(object sender, EventArgs e)
        @brief	Milling 탭 삭제.
        @return	void
        @param	object                      sender
        @param	SelectionChangedEventArgs   e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  13:48
        */
        private void bn_DeleteMillingTab(object sender, EventArgs e)
        {
            UserButton ub = sender as UserButton;
            if(ub != null)
            {
                int nIdx = Convert.ToInt32(ub.CommandParameter);
                sp_MillingList.Children.RemoveAt(nIdx);


                _listMillingData.RemoveAt(nIdx);

                fn_RefreshStackPannel();
                Console.WriteLine($"Delete : {ub.CommandParameter}");
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_RefreshStackPannel()
        @brief	Milling Tab Update
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  13:49
        */
        private void fn_RefreshStackPannel()
        {
            if(sp_MillingList.Children.Count > 1)
            {
                UserButton ubTab;
                Grid grd;
                UserButton ubClose;
                Label lb;
                for (int i = 0; i < sp_MillingList.Children.Count; i++)
                {
                    ubTab = sp_MillingList.Children[i] as UserButton;
                    if (ubTab != null)
                    {
                        ubTab.Background = Brushes.LightGray;
                        ubTab.BorderThickness = new Thickness(1);
                        ubTab.CommandParameter = i;
                        grd = (ubTab.Content as Grid);
                        if (grd != null)
                        {
                            lb = grd.Children[0] as Label;
                            if (lb != null)
                            {
                                lb.Content = "Mil " + (i + 1).ToString("00");
                                lb.Foreground = Brushes.Gray;
                            }
                            ubClose = grd.Children[1] as UserButton;
                            if (ubClose != null)
                                ubClose.CommandParameter = i;
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_ClearStackpannel()
        @brief	스택패널 초기화
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  13:50
        */
        private void fn_ClearStackpannel()
        {
            int Count = sp_MillingList.Children.Count - 1;
            for (int i = 0; i < Count; i ++)
            {
                sp_MillingList.Children.RemoveAt(0);
            }
        }
        //---------------------------------------------------------------------------
        private void bn_Scroll_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                if((string)ub.CommandParameter == "Left")
                {
                    double offset = sv_MillingTab.HorizontalOffset;
                    sv_MillingTab.ScrollToHorizontalOffset(offset - 100);
                }
                else if((string)ub.CommandParameter == "Right")
                {
                    double offset = sv_MillingTab.HorizontalOffset;
                    sv_MillingTab.ScrollToHorizontalOffset(offset + 100);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //_ModelRecipe.MillingCount = _listMillingData.Count;
            cb_ToolType.Items.Clear();
            for (int i = 0; i < UserConst.MAX_TOOLTYPE; i++)
            {
                cb_ToolType.Items.Add(FormMain.FM.m_stSystemOpt.sToolType[i]);
            }

            gb_MOC.Visibility = FormMain.FM.m_stMasterOpt.nUseMOC == 1 ? Visibility.Visible : Visibility.Hidden;
        }

        private void bd_Milling_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Left)
            {
                Console.WriteLine("Left");
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Right)
            {
                Console.WriteLine("Right");
            }
        }

        private void cbOpticCondition_Click(object sender, RoutedEventArgs e)
        {
            //UserClass.g_VisionManager._RecipeModel.CommonOpticCondition = cb_OpticCondition.IsChecked == true ? 1 : 0;

            _ModelRecipe.UseGlobalLoading    = cb_OpticGlobalLoading   .IsChecked == true ? 1 : 0;
            _ModelRecipe.UseGlobalPolishing  = cb_OpticGlobalPolishing .IsChecked == true ? 1 : 0;
            _ModelRecipe.UseGlobalInspection = cb_OpticGlobalInspection.IsChecked == true ? 1 : 0;

            UseGlobalLoading    = _ModelRecipe.UseGlobalLoading   ;
            UseGlobalPolishing  = _ModelRecipe.UseGlobalPolishing ;
            UseGlobalInspection = _ModelRecipe.UseGlobalInspection;
        } 

        /**
        @fn     private bool fn_CheckOpticCondition()
        @brief	영상 Grab 광학 조건 확인.
        @return	Grab 가능시 true, 불가시 false
        @param	-
        @remark	
         - Exposure Time이 0이 아니고, Gain이 0이 아니어야한다.
         - Gain이 0이 아니어야한다.
         - IR Filter가 켜져있을 때 IR이 0이 아니어야한다.
         - IR Filter가 꺼져있을 때 IR, White중 하나는 0이 아니어야한다.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2021/1/5  11:14
        */
        private bool fn_CheckOpticCondition()
        {
            bool bRet = false;
            bool bCmp = false;
            listChange?.Clear();
            if (UseGlobalLoading == 1)
            {
                listChange?.Add("Global - Loading");
                bCmp = fn_GetOpticCondition(
                    CameraExposure[(int)EN_VISION_MODE.Loading], 
                    CameraGain    [(int)EN_VISION_MODE.Loading], 
                    LightIR       [(int)EN_VISION_MODE.Loading],
                    LightWhite    [(int)EN_VISION_MODE.Loading], 
                    LightIRFilter [(int)EN_VISION_MODE.Loading]);
            }
            else
            {
                listChange?.Add($"Private - Loading");
                bCmp = fn_GetOpticCondition(
                    _ModelRecipe.LoadingCameraExposureTime, 
                    _ModelRecipe.LoadingCameraGain        , 
                    _ModelRecipe.LoadingLightIR           ,
                    _ModelRecipe.LoadingLightWhite        , 
                    _ModelRecipe.LoadingLightIRFilter     );
            }
            if (!bCmp)
                listChange?.RemoveAt(listChange.Count - 1);

            bRet |= bCmp;

            if (UseGlobalPolishing == 1)
            {
                listChange?.Add($"Global-Polishing");
                bCmp = fn_GetOpticCondition(
                    CameraExposure[(int)EN_VISION_MODE.Polishing], 
                    CameraGain    [(int)EN_VISION_MODE.Polishing], 
                    LightIR       [(int)EN_VISION_MODE.Polishing],
                    LightWhite    [(int)EN_VISION_MODE.Polishing], 
                    LightIRFilter [(int)EN_VISION_MODE.Polishing]);
            }
            else
            {
                listChange?.Add($"Private-Polishing");
                bCmp = fn_GetOpticCondition(
                    _ModelRecipe.PolishingCameraExposureTime, 
                    _ModelRecipe.PolishingCameraGain        , 
                    _ModelRecipe.PolishingLightIR           ,
                    _ModelRecipe.PolishingLightWhite        , 
                    _ModelRecipe.PolishingLightIRFilter     );
            }
            if (!bCmp)
                listChange?.RemoveAt(listChange.Count - 1);

            bRet |= bCmp;

            if (UseGlobalInspection == 1)
            {
                listChange?.Add($"Global - EPD");
                bCmp = fn_GetOpticCondition(
                    CameraExposure[(int)EN_VISION_MODE.Inspection],
                    CameraGain[(int)EN_VISION_MODE.Inspection],
                    LightIR[(int)EN_VISION_MODE.Inspection],
                    LightWhite[(int)EN_VISION_MODE.Inspection],
                    LightIRFilter[(int)EN_VISION_MODE.Inspection]);
            }
            else
            {
                listChange?.Add($"Private - EPD");
                bCmp = fn_GetOpticCondition(
                    _ModelRecipe.InspectionCameraExposureTime,
                    _ModelRecipe.InspectionCameraGain,
                    _ModelRecipe.InspectionLightIR,
                    _ModelRecipe.InspectionLightWhite,
                    _ModelRecipe.InspectionLightIRFilter);
            }

            if (!bCmp)
                listChange?.RemoveAt(listChange.Count - 1);

            bRet |= bCmp;

            return !bRet;
        }

        private bool fn_GetOpticCondition(int exposure, int gain, int lightIR, int lightWhite, int IRfilter)
        {
            bool bRet = false;
            if (!(exposure >= 1000 && gain >= 0))
            {
                listChange?.Add($"   ExposureTime : {exposure}  -  Gain : {gain}");
                bRet |= true;
            }

            if (IRfilter == 1)
            {
                if (lightIR == 0)
                {
                    listChange?.Add($"   IRFilter : ON      Light IR : {lightIR}        Light White : {lightWhite}");
                    bRet |= true;
                }
            }
            else
            {
                if (!(lightWhite > 0 || lightIR > 0))
                {
                    listChange?.Add($"   IRFilter : OFF     Light IR : {lightIR}        Light White : {lightWhite}");
                    bRet |= true;
                }
            }
            return bRet;
        }
    }
}
