using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using UserInterface;
using WaferPolishingSystem.Define;
using WaferPolishingSystem.Vision;
using static WaferPolishingSystem.Define.UserClass;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageRecipe_Cleaning.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageRecipe_Cleaning : Page
    {
        public string _CurrentRecipeName = string.Empty;
        List<Recipe_Cleaning> _listCleaning = new List<Recipe_Cleaning>();
        public PageRecipe_Cleaning()
        {
            InitializeComponent();
            //this.DataContext = Cleaning;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            grid_PlateDrawing.Visibility = Visibility.Visible;
            fn_LoadData();
        }

        private void fn_LoadData()
        {
            _listCleaning.Clear();
            fn_ClearStackpannel();
            
            for (int i = 0; i < g_VisionManager._RecipeModel.CleaningCount; i++)
            {
                bn_CleaningAdd_Click(null, null);
                _listCleaning[i].GetDataFromRecipe(g_VisionManager._RecipeModel.Cleaning[i]);
                _listCleaning[i].SampleWidth  = g_VisionManager._RecipeModel.SampleWidth;
                _listCleaning[i].SampleHeight = g_VisionManager._RecipeModel.SampleHeight;
                // Add Tab
            }
            if (g_VisionManager._RecipeModel.CleaningCount == 0)
                bn_CleaningAdd_Click(null, null);
            bn_SelectionMillingTab(sp_CleaningList.Children[0], null);
        }

        private void fn_SaveData()
        {
            g_VisionManager._RecipeModel.CleaningCount = _listCleaning.Count;
            for (int i = 0; i < _listCleaning.Count; i++)
            {
                if (_listCleaning[i] == null)
                {
                    //Error
                    Console.WriteLine($"{this.Title}_Save Data {i} Recipe is null ");
                    return;
                }
                _listCleaning[i].SetDataToRecipe(ref g_VisionManager._RecipeModel.Cleaning[i]);
                if(_listCleaning[i].SampleWidth > 0)
                    g_VisionManager._RecipeModel.SampleWidth  = _listCleaning[i].SampleWidth;
                if (_listCleaning[i].SampleHeight > 0)
                    g_VisionManager._RecipeModel.SampleHeight = _listCleaning[i].SampleHeight;
            }
            g_VisionManager._RecipeManager.fn_WriteRecipeCleaning();
        }

        private void bn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Cleaning을 저장 하시겠습니까?", "Cleaning Save", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                FormMain.MAIN.frame.IsEnabled = false;
                FormMain.MAIN.grid_button.IsEnabled = false;
                FormMain.MAIN.Cursor = Cursors.Wait;
                new Thread(new ThreadStart(delegate ()
                {
                    fn_SaveData();
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        FormMain.MAIN.Cursor = Cursors.Arrow;
                        FormMain.MAIN.frame.IsEnabled = true;
                        FormMain.MAIN.grid_button.IsEnabled = true;
                        MessageBox.Show($"Cleaning 저장 성공.", "Cleaning Save");
                    }));
                })).Start();
            }
            else
            {
                MessageBox.Show("취소되었습니다.", "Cleaning Save");
            }

        }

        public void fn_LoadRecipe(string strRecipe)
        {
            g_VisionManager._RecipeManager.fn_ReadRecipeCleaning(strRecipe);
            fn_LoadData();
        }

        private void bn_CleaningAdd_Click(object sender, RoutedEventArgs e)
        {
            int nspCount = sp_CleaningList.Children.Count;
            if (nspCount < 11)
            {
                UserButton ubClose = new UserButton();
                ubClose.Content = "X";
                ubClose.Background = Brushes.Transparent;
                // ubClose.Background = Brushes.Maroon;
                // ubClose.Foreground = Brushes.White;
                ubClose.FontWeight = FontWeights.Bold;
                ubClose.MouseOverBackgroundBrush = Brushes.Magenta;
                ubClose.BorderThickness = new Thickness(0);
                ubClose.CornerRadius = new CornerRadius(5);
                ubClose.HorizontalAlignment = HorizontalAlignment.Right;
                ubClose.FontSize = 12;
                ubClose.Width = 20;
                ubClose.Height = 20;
                ubClose.Click += bn_DeleteMillingTab;
                ubClose.CommandParameter = nspCount - 1;

                Label tb = new Label();
                tb.Content = "Clean " + nspCount.ToString("00");
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

                sp_CleaningList.Children.Insert(sp_CleaningList.Children.Count - 1, ubTab);

                _listCleaning.Add(new Recipe_Cleaning());
                _listCleaning[_listCleaning.Count - 1].SampleWidth  = g_VisionManager._RecipeModel.SampleWidth;
                _listCleaning[_listCleaning.Count - 1].SampleHeight = g_VisionManager._RecipeModel.SampleHeight;
                fn_RefreshStackPannel();
                bn_SelectionMillingTab(ubTab, e);
                //sv_MillingTab.ScrollToRightEnd();
            }
        }
        private void bn_DeleteMillingTab(object sender, EventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                int nIdx = Convert.ToInt32(ub.CommandParameter);
                sp_CleaningList.Children.RemoveAt(nIdx);
                _listCleaning.RemoveAt(nIdx);

                fn_RefreshStackPannel();
                Console.WriteLine($"Delete : {ub.CommandParameter}");
            }
        }
        private void bn_SelectionMillingTab(object sender, EventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                if (sp_CleaningList.Children.Count > 1)
                {
                    try
                    {
                        fn_RefreshStackPannel();
                        int nIdx = Convert.ToInt32(ub.CommandParameter);
                        ub.Background = Brushes.White;
                        ub.BorderThickness = new Thickness(1, 1, 1, 0);
                        ub.FontWeight = FontWeights.Bold;
                        ((ub.Content as Grid).Children[0] as Label).Foreground = Brushes.Black;

                        _listCleaning[nIdx].SampleWidth  = g_VisionManager._RecipeModel.SampleWidth;
                        _listCleaning[nIdx].SampleHeight = g_VisionManager._RecipeModel.SampleHeight;

                        bd_Milling.DataContext = _listCleaning[nIdx];
                        grid_Plate.DataContext = _listCleaning[nIdx];
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

        private void fn_RefreshStackPannel()
        {
            if (sp_CleaningList.Children.Count > 1)
            {
                UserButton ubTab;
                Grid grd;
                UserButton ubClose;
                Label lb;
                for (int i = 0; i < sp_CleaningList.Children.Count; i++)
                {
                    ubTab = sp_CleaningList.Children[i] as UserButton;
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
                                lb.Content = "Clean " + (i + 1).ToString("00");
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
        private void fn_ClearStackpannel()
        {
            int Count = sp_CleaningList.Children.Count - 1;
            for (int i = 0; i < Count; i++)
            {
                sp_CleaningList.Children.RemoveAt(0);
            }
        }

    }
}
