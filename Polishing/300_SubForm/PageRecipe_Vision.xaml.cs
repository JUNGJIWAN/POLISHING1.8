using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaferPolishingSystem.Vision;
using static WaferPolishingSystem.Define.UserEnumVision;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserConstVision;
using static WaferPolishingSystem.Define.UserClass;
using WaferPolishingSystem.Define;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Basler.Pylon;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageRecipe_Vision.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageRecipe_Vision : Page
    {
        PageRecipe_Vision_Parameter pageParameter = new PageRecipe_Vision_Parameter();
        RecipeList _recipe;

        string m_strRecipeName;
        string m_strRecipePath = STRRECIPEPATH;
        int m_nSelModels = -1;
        int m_nSelMillings = -1;


        public PageRecipe_Vision()
        {
            InitializeComponent();
            ComponentDispatcher.ThreadIdle += UIInit;
        }
        private void UIInit(object sender, EventArgs args)
        {
            ComponentDispatcher.ThreadIdle -= UIInit;
            this.Background = G_COLOR_PAGEBACK;
            frame.Content = pageParameter;

            Fn_SelectAlgorithm(EN_RECIPE_MODE.Models);
            //FileLoad

            frame.IsEnabled = false;
        }

        private void bn_ModelsAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(lv_Models.Items.Count < 10)
                    lv_Models.Items.Add(new MyListItem(EN_RECIPE_MODE.Models, "Models" + lv_Models.Items.Count.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void bn_ModelsDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lv_Models.Items.Remove(lv_Models.SelectedItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void lv_Models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int nIndex = lv_Models.SelectedIndex;
                if (nIndex >= 0 && nIndex < RECIPE_MAX_MODEL_COUNT && g_VisionManager._RecipeManager != null)
                {
                    lb_ModelsSel.Content = nIndex + 1;
                    lv_Millings.Items.Clear();
                    if (_recipe != null && _recipe.Model[nIndex] != null)
                    {
                        tb_Model_Left.Text =    _recipe.Model[nIndex].Model.Left.ToString();
                        tb_Model_Top.Text =     _recipe.Model[nIndex].Model.Top.ToString();
                        tb_Model_Right.Text =   _recipe.Model[nIndex].Model.Right.ToString();
                        tb_Model_Bottom.Text =  _recipe.Model[nIndex].Model.Bottom.ToString();
                        for (int i = 0; i < _recipe.Model[nIndex].MillingCount; i++)
                        {
                            lv_Millings.Items.Add(new MyListItem(EN_RECIPE_MODE.Milling, "Milling" + (i + 1).ToString()));
                        }
                    }
                    frame.IsEnabled = true;
                }
                else
                {
                    lb_ModelsSel.Content = "-";
                    frame.IsEnabled = false;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void lv_Millings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int nIndex = lv_Models.SelectedIndex;
                int nIndex2 = lv_Millings.SelectedIndex;
                if (nIndex2 >= 0 && g_VisionManager._RecipeManager != null)
                {
                    lb_MilSel.Content = nIndex2 + 1;
                    if (_recipe != null && _recipe.Model[nIndex] != null && _recipe.Model[nIndex].Milling[nIndex2] != null)
                    {
                        tb_Milling_Left.Text =      _recipe.Model[nIndex].Milling[nIndex2].Left.ToString();
                        tb_Milling_Top.Text =       _recipe.Model[nIndex].Milling[nIndex2].Top.ToString();
                        tb_Milling_Right.Text =     _recipe.Model[nIndex].Milling[nIndex2].Right.ToString();
                        tb_Milling_Bottom.Text =    _recipe.Model[nIndex].Milling[nIndex2].Bottom.ToString();
                    }
                   
                }
                else
                {
                    lb_MilSel.Content = "-";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //lb_MilSel.Content = lv_Millings.SelectedIndex;
        }

        private void bn_MillingsAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(lv_Millings.Items.Count < 10)
                    lv_Millings.Items.Add(new MyListItem(EN_RECIPE_MODE.Milling, "Milling" + lv_Millings.Items.Count.ToString()));
            }
            catch(Exception ex)
            {

            }
        }

        private void bn_MillingsDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bn_Models_Click(object sender, RoutedEventArgs e)
        {
            Fn_SelectAlgorithm(EN_RECIPE_MODE.Models);
        }

        private void bn_Pattern_Click(object sender, RoutedEventArgs e)
        {
            Fn_SelectAlgorithm(EN_RECIPE_MODE.Pattern);
        }

        private void Fn_SelectAlgorithm(EN_RECIPE_MODE mode)
        {
            bn_Models.Background = G_COLOR_BTNNORMAL;
            bn_Pattern.Background = G_COLOR_BTNNORMAL;
            //pageParameter.FnSetView(mode);
            switch (mode)
            {
                case EN_RECIPE_MODE.Models:
                    bn_Models.Background = G_COLOR_BTNCLICKED;
                    break;
                case EN_RECIPE_MODE.Pattern:
                    bn_Pattern.Background = G_COLOR_BTNCLICKED;
                    break;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(m_strRecipePath != null)
            {
                fn_LoadRecipe(m_strRecipeName);
                fn_UpdateUI();
            }
        }

        public void fn_LoadRecipe(string strName)
        {
            if (strName != null)
            {
                m_strRecipeName = strName;
                g_VisionManager._RecipeManager.fn_ReadRecipe(m_strRecipePath, m_strRecipeName);
                
                lv_Models.Items.Clear();
                lv_Millings.Items.Clear();
                for (int i = 0; i < g_VisionManager._RecipeModel.ModelCount; i++)
                {
                    lv_Models.Items.Add(new MyListItem((EN_RECIPE_MODE)g_VisionManager._RecipeModel.Model[i].Algorithm, "Models" + (lv_Models.Items.Count+1).ToString()));
                }
            }
        }

        private void fn_UpdateUI()
        {
            g_VisionManager._RecipeManager.fn_ReadVisionRecipe();
            uc_VisionResolutionX.UPValue            = g_VisionManager._RecipeVision.ResolutionX.ToString("0.0");
            uc_VisionResolutionY.UPValue            = g_VisionManager._RecipeVision.ResolutionY.ToString("0.0");
            cb_ThetaCalibration.SelectedIndex       = g_VisionManager._RecipeVision.ThetaCalibration;
            uc_ThetaValue.UPValue                   = g_VisionManager._RecipeVision.ThetaValue.ToString("0.0");
            uc_SpindleOffsetX.UPValue               = g_VisionManager._RecipeVision.SpindleOffsetX.ToString("0.0");
            uc_SpindleOffsetY.UPValue               = g_VisionManager._RecipeVision.SpindleOffsetY.ToString("0.0");

            uc_LTX.UPValue                          = g_VisionManager._RecipeVision.LTX.ToString("0.0");
            uc_LTY.UPValue                          = g_VisionManager._RecipeVision.LTY.ToString("0.0");
            uc_RTX.UPValue                          = g_VisionManager._RecipeVision.RTX.ToString("0.0");
            uc_RTY.UPValue                          = g_VisionManager._RecipeVision.RTY.ToString("0.0");
            uc_RBX.UPValue                          = g_VisionManager._RecipeVision.RBX.ToString("0.0");
            uc_RBY.UPValue                          = g_VisionManager._RecipeVision.RBY.ToString("0.0");
            uc_LBX.UPValue                          = g_VisionManager._RecipeVision.LBX.ToString("0.0");
            uc_LBY.UPValue                          = g_VisionManager._RecipeVision.LBY.ToString("0.0");

            uc_AngleAllow.UPValue                   = g_VisionManager._RecipeVision.AngleOffset.ToString("0.0");
        }

        public void fn_UpdateData()
        {
            //double.TryParse(uc_VisionResolution.UPValue, out g_VisionManager._RecipeVision.Resolution);
            g_VisionManager._RecipeVision.ThetaCalibration = cb_ThetaCalibration.SelectedIndex;
            double.TryParse(uc_ThetaValue.UPValue, out g_VisionManager._RecipeVision.ThetaValue);
            double.TryParse(uc_SpindleOffsetX.UPValue, out g_VisionManager._RecipeVision.SpindleOffsetX);
            double.TryParse(uc_SpindleOffsetY.UPValue, out g_VisionManager._RecipeVision.SpindleOffsetY);
            double.TryParse(uc_LTX.UPValue, out g_VisionManager._RecipeVision.LTX);
            double.TryParse(uc_LTY.UPValue, out g_VisionManager._RecipeVision.LTY);
            double.TryParse(uc_RTX.UPValue, out g_VisionManager._RecipeVision.RTX);
            double.TryParse(uc_RTY.UPValue, out g_VisionManager._RecipeVision.RTY);
            double.TryParse(uc_RBX.UPValue, out g_VisionManager._RecipeVision.RBX);
            double.TryParse(uc_RBY.UPValue, out g_VisionManager._RecipeVision.RBY);
            double.TryParse(uc_LBX.UPValue, out g_VisionManager._RecipeVision.LBX);
            double.TryParse(uc_LBY.UPValue, out g_VisionManager._RecipeVision.LBY);
            double.TryParse(uc_AngleAllow.UPValue, out g_VisionManager._RecipeVision.AngleOffset);
        }
    }
}
