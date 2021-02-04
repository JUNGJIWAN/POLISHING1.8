using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WaferPolishingSystem;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserConstVision;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.Define.UserINI;
using UserInterface;
using System.Data;
using System.IO;

namespace WaferPolishingSystem.Form
{
    //public delegate void del_ViewPolishing(int Index, string strName);
    //public delegate void del_ViewLoading(int Index, string strName);
    //public delegate void del_ViewRecipeMain();
    /// <summary>
    /// PageRecipe.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageRecipe : Page
    {

        //Var
        UserInterface.UserButton m_bnPrev;
        PageRecipe_Polishing mc_Polishing = new PageRecipe_Polishing();
        PageRecipe_Cleaning mc_Cleaning = new PageRecipe_Cleaning();
        
        PageRecipe_InspectionVision mc_InspectionVision = new PageRecipe_InspectionVision();

        DataTable _dtRecipe = new DataTable();

        private int _nSelRecipe = -1;

        private string _prevName = "";

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        //---------------------------------------------------------------------------
        public PageRecipe()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;
            this.GridSub.Background = UserConst.G_COLOR_SUBMENU;
            fn_SetPage(bn_Polishing);
            //---------------------------------------------------------------------------
            // Recipe Loading전에는 항목 수정 불가하게 Disable 처리.
            //frame.IsEnabled = false;
            //---------------------------------------------------------------------------
            mc_Polishing._frame_Main = frame_Modify;
            mc_Polishing.del_Edit += fn_ModelEdit;


            _dtRecipe.Columns.Add("HEADER");
            _dtRecipe.Columns.Add("NAME");
            _dtRecipe.Columns.Add("DATEVIEW");
            _dtRecipe.Columns.Add("DATE");

            dg_Recipe.ItemsSource = _dtRecipe.DefaultView;
        }

        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            menuConfirm.IsEnabled = (SEQ._bRun ? false : true) && frame.IsEnabled; //JUNG/201007



            //
            m_UpdateTimer.Start();
        }

        //---------------------------------------------------------------------------
        //Timer On/Off
        public void fn_SetTimer(bool on)
        {
            if (on)
            {
                m_UpdateTimer.IsEnabled = true;
                m_UpdateTimer.Start();
            }
            else
                m_UpdateTimer.Stop();

        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void MenuChange(object sender, RoutedEventArgs e)
        @brief	메뉴 누를 때 버튼 이벤트.
        @return	void
        @param	object          sender
        @param  RoutedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:27
        */
        private void MenuChange(object sender, RoutedEventArgs e)
        {
            UserButton menubn = (sender as UserButton);
            fn_SetPage(menubn);
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void fn_SetPage(UserButton bn)
        @brief	페이지 변경 함수.
        @return	void
        @param	UserButton bn
        @remark	
         - 이전 버튼 배경을 Normal로 변경.
         - 현재 버튼의 String을 Check해서 Frame Content에 Page Class Set.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:30
        */
        private void fn_SetPage(UserButton bn)
        {
            if (m_bnPrev != null) m_bnPrev.Background = G_COLOR_BTNNORMAL;
            bn.Background = G_COLOR_BTNCLICKED;
            
            m_bnPrev = bn;
            
            string strName = bn.Content as string;

             if (strName == "Polishing")
            {
                frame.Content = mc_Polishing;
            }
            else if (strName == "Cleaning")
            {
                frame.Content = mc_Cleaning;
            }
            else if (strName == "Inspection Vision")
            {
                //mc_Vision.fn_LoadRecipe(lb_SelRecipe.Text as string);
                frame.Content = mc_InspectionVision;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void btCopy_Click(object sender, RoutedEventArgs e)
        @brief	Copy Button Click
        @return	void
        @param	object          sender
        @param	RoutedEventArgs e     
        @remark	
         - Copy 버튼을 눌렀을 때, 현재 선택된 Recipe Name에 _Copy를 붙여서 생성.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  11:14
        */
        private void btCopy_Click(object sender, RoutedEventArgs e)
        {
            //
            _nSelRecipe = dg_Recipe.SelectedIndex;
            if (_nSelRecipe >= 0)
            {
                //string strSelRecipe = _dtRecipe.Rows[_nSelRecipe]["NAME"].ToString();
                string strSelRecipe = (dg_Recipe.Items[_nSelRecipe] as DataRowView).Row[1].ToString();
                string strCopyRecipe = strSelRecipe + $"_COPY{DateTime.Now.ToString("HHmmss")}";

                
                fn_CopyRecipe(strSelRecipe, strCopyRecipe);
                fn_GetRecipeList();

            }
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		private void btDel_Click(object sender, RoutedEventArgs e)
        @brief	Model ListBox에서 Model 삭제.
        @return	void
        @param	object          sender
        @param	RoutedEventArgs e
        @remark	
         - Todo : 실제 파일에 수정 결과 반영 해야함.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  11:16
        */
        private void btDel_Click(object sender, RoutedEventArgs e)
        {
            //
            int nSel = dg_Recipe.SelectedIndex;

            if (nSel > -1)
            {
                //string strName = _dtRecipe.Rows[nSel]["NAME"] as string;
                string strName = (dg_Recipe.Items[nSel] as DataRowView).Row[1].ToString();
                if (MessageBox.Show(strName + "을 삭제 하시겠습니까?", "Recipe 삭제", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // del File
                    fn_DeleteRecipe(strName);
                    fn_GetRecipeList();
                    MessageBox.Show(strName + " 삭제 되었습니다.", "Recipe 삭제", MessageBoxButton.OK, MessageBoxImage.Information);
                    fn_WriteLog(this.Title + " : Delete Recipe." + strName, UserEnum.EN_LOG_TYPE.ltTrace);
                }
                else
                {
                    MessageBox.Show(strName + "을 삭제가 취소 되었습니다.", "Recipe 삭제", MessageBoxButton.OK, MessageBoxImage.Information);
                    fn_WriteLog(this.Title + " : Delete Cancel." + strName, UserEnum.EN_LOG_TYPE.ltTrace);
                }
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void btRcpLoad_Click(object sender, RoutedEvetArgs e)
        @brief	ListBox에서 선택된 Model Load.
        @return	void
        @param	object          sender
        @param	RoutedEventArgs e
        @remark	
         - PageRecipe_Polishing에 파일 로드함.
         - PageRecipe_Cleaning에 파일 로드함.(Todo!)
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:16
        */
        private void btRcpLoad_Click(object sender, RoutedEventArgs e)
        {
            _nSelRecipe = dg_Recipe.SelectedIndex;
            if (_nSelRecipe >= 0)
            {
                frame.IsEnabled = true;
                //menuConfirm.IsEnabled = true;
                //tb_SelectedRecipe.Text = _dtRecipe.Rows[_nSelRecipe]["NAME"].ToString();
                tb_SelectedRecipe.Text = (dg_Recipe.Items[_nSelRecipe] as DataRowView).Row[1].ToString();
                mc_Polishing.fn_LoadRecipe(tb_SelectedRecipe.Text);
                mc_Cleaning.fn_LoadRecipe(tb_SelectedRecipe.Text);
            }
        }
        //---------------------------------------------------------------------------
        private void menuConfirm_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"{tb_SelectedRecipe.Text}을 Setting 하시겠습니까?", "Recipe Setting", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                lb_CurrentRecipe.Text = tb_SelectedRecipe.Text;
                FM._sRecipeName = tb_SelectedRecipe.Text;
                g_VisionManager._RecipeModel.CopyTo(g_VisionManager.CurrentRecipe);
                g_VisionManager._RecipeVision.CurrentRecipeName = g_VisionManager.CurrentRecipe.strRecipeName;
                g_VisionManager._RecipeManager.fn_WriteVisionRecipe();

                LOT.fn_LotOpen(FM._sRecipeName); //JUNG/200814

                fn_WriteLog("[Manual]" + this.Title + " : Confirm Recipe. " + tb_SelectedRecipe.Text, UserEnum.EN_LOG_TYPE.ltTrace);
                MessageBox.Show($"{tb_SelectedRecipe.Text} Recipe Setting 성공.");
            }
            else
            {
                MessageBox.Show("취소되었습니다.");
            }
        }
        //---------------------------------------------------------------------------
        private void fn_UnLoadData()
        {

        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Get Recipe List
            tb_RecipeName.Text = "";
            btEdit.Content = "EDIT";
            frame_Modify.Content = null;
            fn_ModelEdit(true);
            fn_GetRecipeList();
            lb_CurrentRecipe.Text = FM._sRecipeName;
        }
        //---------------------------------------------------------------------------
        private void fn_GetRecipeList()
        {
            if (Directory.Exists(STRRECIPEPATH))
            {
                string[] strRecipeList = Directory.GetDirectories(STRRECIPEPATH);
                string strSort = _dtRecipe.DefaultView.Sort;
                
                _dtRecipe.Rows.Clear();

                for (int i = 0; i < strRecipeList.Length; i++)
                {
                    DirectoryInfo di = new DirectoryInfo(strRecipeList[i]);
                    FileInfo fi = new FileInfo(di.FullName + "\\" + di.Name + ".ini");

                    _dtRecipe.Rows.Add(di.Name.Substring(0, 3), di.Name, fi.LastWriteTime.ToString("yy.MM.dd HH:mm:ss"), fi.LastWriteTime);
                }
                _dtRecipe.DefaultView.Sort = strSort;
                _dtRecipe = _dtRecipe.DefaultView.ToTable();

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_dtRecipe.DefaultView);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("HEADER");
                view.GroupDescriptions.Add(groupDescription);

                dg_Recipe.ItemsSource = _dtRecipe.DefaultView;

                // Recipe UnLoad
                frame.IsEnabled = false;
                tb_SelectedRecipe.Text = "-";
            }
        }
        //---------------------------------------------------------------------------
        private bool fn_CopyRecipe(string strSrc, string strDst)
        {
            bool bRet = false;
            try
            {
                string strSrcDir = STRRECIPEPATH + strSrc + "\\";
                string strDstDir = STRRECIPEPATH + strDst + "\\";

                Directory.CreateDirectory(strDstDir);

                string[] strSrcFiles = Directory.GetFiles(strSrcDir);

                string strFileName = "";
                for (int i = 0;i < strSrcFiles.Length; i++)
                {
                    strFileName = strSrcFiles[i].Substring(strSrcFiles[i].LastIndexOf('\\')+1);
                    if(strFileName.Substring(strFileName.LastIndexOf('.') +1) == "ini")
                        File.Copy(strSrcFiles[i], strDstDir + strDst + ".ini");
                    else
                        File.Copy(strSrcFiles[i], strDstDir + strFileName); 
                }
                fn_UpdateImagePath(strDst);
                bRet = true;
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltTrace);
            }
            return bRet;
        }
        //---------------------------------------------------------------------------
        private void fn_UpdateImagePath(string strDst)
        {
            for (int i = 0; i < 10; i++)
            {
                fn_Save($"Model{i + 1}", "LoadingImagePath", STRRECIPEPATH + strDst + $"\\Loading{i + 1}.bmp", STRRECIPEPATH + strDst + "\\" + strDst + ".ini");
                fn_Save($"Model{i + 1}", "ModelImagePath", STRRECIPEPATH + strDst + $"\\Model{i + 1}.bmp", STRRECIPEPATH + strDst + "\\" + strDst + ".ini");
            }
        }
        //---------------------------------------------------------------------------
        private bool fn_DeleteRecipe(string strName)
        {
            bool bRet = false;
            try
            {
                Directory.Delete(STRRECIPEPATH + strName, true);
                bRet = true;
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltTrace);
            }
            return bRet;
        }
        //---------------------------------------------------------------------------
        private void bnListAlign_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                _dtRecipe.DefaultView.Sort = ub.CommandParameter as string;
                _dtRecipe = _dtRecipe.DefaultView.ToTable();
                dg_Recipe.ItemsSource = _dtRecipe.DefaultView;
            }
        }
        //---------------------------------------------------------------------------
        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                string Mode = ub.Content as string;
                int nSel = dg_Recipe.SelectedIndex;
                if (nSel > -1)
                {
                    if (Mode == "EDIT")
                    {
                        ub.Content = "DONE";
                        _prevName = (dg_Recipe.Items[nSel] as DataRowView).Row[1] as string;
                        tb_RecipeName.Text = _prevName;
                        tb_RecipeName.IsEnabled = true;
                        tb_RecipeName.Focus();
                        tb_RecipeName.SelectAll();

                        // Disable DataGrid
                        fn_EditModeControlEnable(false);
                        frame.IsEnabled = false;
                    }
                    else
                    {
                        // Rename Recipe Folder
                        try
                        {
                            if (_prevName != tb_RecipeName.Text)
                            {
                                // Check Same Name Folder
                                bool bSameExist = Directory.Exists(STRRECIPEPATH + tb_RecipeName.Text + "\\");
                                if (bSameExist)
                                {
                                    if (MessageBox.Show("동일한 이름의 Recipe가 확인되었습니다. 덮어쓰시겠습니까?", "Rename Message", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                    {
                                        throw new Exception("Rename Canceled.");
                                    }
                                    
                                    Directory.Delete(STRRECIPEPATH + tb_RecipeName.Text + "\\", true);
                                }
                                File.Move(STRRECIPEPATH + _prevName + "\\" + _prevName + ".ini", STRRECIPEPATH + _prevName + "\\" + tb_RecipeName.Text + ".ini");
                                Directory.Move(STRRECIPEPATH + _prevName, STRRECIPEPATH + tb_RecipeName.Text);

                                fn_UpdateImagePath(tb_RecipeName.Text);
                            }
                            (dg_Recipe.Items[nSel] as DataRowView).Row[1] = tb_RecipeName.Text;
                            
                        }
                        catch (Exception ex)
                        {
                            tb_RecipeName.Text = _prevName;
                            fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltTrace);
                            //return;
                        }
                        finally
                        {
                            tb_RecipeName.Text = string.Empty;
                            dg_Recipe.Focus();

                            fn_EditModeControlEnable(true);
                            frame.IsEnabled = true;
                            ub.Content = "EDIT";
                            _prevName = "";
                            fn_GetRecipeList();
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        private void fn_EditModeControlEnable(bool bEnable)
        {
            dg_Recipe.IsEnabled = bEnable;
            gb_ListAlign.IsEnabled = bEnable;
            btCopy.IsEnabled = bEnable;
            btDel.IsEnabled = bEnable;
            //btNew.IsEnabled = bEnable;
            menuConfirm.IsEnabled = bEnable;
            btRcpLoad.IsEnabled = bEnable;
        }
        //---------------------------------------------------------------------------
        private void dg_Recipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nSel = dg_Recipe.SelectedIndex;
            if (nSel > -1)
            {
                //tb_RecipeName.Text = _dtRecipe.Rows[nSel]["NAME"] as string;
                //tb_RecipeName.Text = dg_Recipe.Items[nSel] as string;
                //tb_RecipeName.Text = (dg_Recipe.Items[nSel] as DataRowView).Row[1].ToString();
            }
        }
        //---------------------------------------------------------------------------
        private void fn_ModelEdit(bool bEnable)
        {
            btEdit.IsEnabled = bEnable;
            fn_EditModeControlEnable(bEnable);
        }
        //---------------------------------------------------------------------------
        private void btNew_Click(object sender, RoutedEventArgs e)
        {
            // New Recipe
            string strSelRecipe = (dg_Recipe.Items[_nSelRecipe] as DataRowView).Row[1].ToString();
            string strCopyRecipe = strSelRecipe + "_Copy";

            for (int i = 0; i < _dtRecipe.Rows.Count; i++)
            {
                if (strSelRecipe == strCopyRecipe)
                {
                    strCopyRecipe += "_Copy";
                }
            }

            fn_CopyRecipe(strSelRecipe, strCopyRecipe);

            fn_GetRecipeList();
        }
        //---------------------------------------------------------------------------
        private void tb_RecipeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(btEdit.Content as string == "EDIT")
            {
                // 검색
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    if (tb.Text == "")
                        _dtRecipe.DefaultView.RowFilter = string.Empty;
                    else
                        _dtRecipe.DefaultView.RowFilter = $"NAME like '%{tb.Text}%'";
                }
            }
        }

        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
                e.Cancel = true;
        }
    }
}
