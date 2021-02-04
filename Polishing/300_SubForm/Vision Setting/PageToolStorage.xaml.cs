using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFunctionVision;
using static WaferPolishingSystem.Define.UserEnumVision;
using System.Text;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using static WaferPolishingSystem.BaseUnit.ManualId;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageToolStorage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageToolStorage : Page
    {
        DataTable dtData = new DataTable();
        string strFileName = "";
        public PageToolStorage()
        {
            InitializeComponent();

            g_VisionManager._CamManager.update += ac_Align.SetImage;

            dtData.Columns.Add("Dir");
            dtData.Columns.Add("ToolPosX");
            dtData.Columns.Add("ToolPosY");
            dtData.Columns.Add("SearchPosX");
            dtData.Columns.Add("SearchPosY");
            dtData.Columns.Add("ImagePath");

            /*
            201215_D: Image Info
            LB X :93.6
            LB Y :168.8172

            RB X :34.76
            RB Y :168.8172

            RT X :34.76
            RT Y :313.7072

            LT X : 92.29
            LT Y : 313.7072
             */
            dtData.Rows.Add("-", "93.6" , "168.8172", "-", "-", @"D:\LB.bmp");
            dtData.Rows.Add("-", "34.76", "168.8172", "-", "-", @"D:\RB.bmp");
            dtData.Rows.Add("-", "34.76", "313.7072", "-", "-", @"D:\RT.bmp");
            dtData.Rows.Add("-", "92.29", "313.7072", "-", "-", @"D:\LT.bmp");

            dg_Data.ItemsSource = dtData.DefaultView;
#if DEBUG
            gb_Control  .IsEnabled = true;
            gb_Image    .IsEnabled = true;
            gb_Message  .IsEnabled = true;
#endif
        }

        private void bn_LotNameTest_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder(tb_LotName.Text);
            libSetLotName(sb, sb.Length);
            fn_WriteMessage($"Lot Test : {tb_LotName.Text}");
        }

        private void fn_WriteMessage(string strMsg)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate ()
            {
                listMsg.Items.Add($"[{DateTime.Now.ToString("hh:MM:ss:fff")}] {strMsg}");
                listMsg.ScrollIntoView(listMsg.Items[listMsg.Items.Count - 1]);
            }));
        }

        private void bnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Filter = "Image File (*.bmp)|*.bmp";
            if (fdlg.ShowDialog() == true)
            {
                ac_Align.OpenImage(fdlg.FileName);
                strFileName = fdlg.FileName;
            }
        }

        private void bnSearch_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap wb = ac_Align.fn_GetImageStream();
            if(wb != null)
            {

            }
        }

        private WriteableBitmap fn_OpenImage(string strPath)
        {
            WriteableBitmap wbm = null;
            if (File.Exists(strPath))
            {
                BitmapImage bmpImg = new BitmapImage();
                FileStream source = File.OpenRead(strPath);

                bmpImg.BeginInit();
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = source;
                bmpImg.EndInit();
                wbm = new WriteableBitmap(bmpImg);
            }
            return wbm;
        }

        private void bnImagePath_Click(object sender, RoutedEventArgs e)
        {
            int nSel = dg_Data.SelectedIndex;
            if (nSel > -1)
            {
                dtData.Rows[nSel]["ImagePath"] = strFileName;
            }
        }

        private void bn_FunctionTest_Click(object sender, RoutedEventArgs e)
        {
            fn_RunSearch();
        }

        private void fn_RunSearch()
        {
            Thread thread = new Thread(new ThreadStart(thread_Search));
            thread.Start();
        }

        private void thread_Search()
        {
            int step = 0;
            bool bWork = true;
            string strName = "";
            double dPosX = 0.0;
            double dPosY = 0.0;
            fn_WriteMessage("Thread Start");
            while(bWork)
            {
                Thread.Sleep(10);
                switch(step)
                {
                    case 0:
                        double.TryParse(dtData.Rows[step]["ToolPosX"] as string, out dPosX);
                        double.TryParse(dtData.Rows[step]["ToolPosY"] as string, out dPosY);
                        strName = dtData.Rows[step]["ImagePath"] as string;
                        if (UserClass.g_VisionManager._ToolAlign.fn_PinSearch(dPosX, dPosY, step, fn_OpenImage(strName)))
                            fn_WriteMessage($"Step : {step++}, PinSearched.");
                        break;
                    case 1:
                        double.TryParse(dtData.Rows[step]["ToolPosX"] as string, out dPosX);
                        double.TryParse(dtData.Rows[step]["ToolPosY"] as string, out dPosY);
                        strName = dtData.Rows[step]["ImagePath"] as string;
                        if (UserClass.g_VisionManager._ToolAlign.fn_PinSearch(dPosX, dPosY, step, fn_OpenImage(strName)))
                            fn_WriteMessage($"Step : {step++}, PinSearched.");
                        break;
                    case 2:
                        double.TryParse(dtData.Rows[step]["ToolPosX"] as string, out dPosX);
                        double.TryParse(dtData.Rows[step]["ToolPosY"] as string, out dPosY);
                        strName = dtData.Rows[step]["ImagePath"] as string;
                        if (UserClass.g_VisionManager._ToolAlign.fn_PinSearch(dPosX, dPosY, step, fn_OpenImage(strName)))
                            fn_WriteMessage($"Step : {step++}, PinSearched.");
                        break;
                    case 3:
                        double.TryParse(dtData.Rows[step]["ToolPosX"] as string, out dPosX);
                        double.TryParse(dtData.Rows[step]["ToolPosY"] as string, out dPosY);
                        strName = dtData.Rows[step]["ImagePath"] as string;
                        if (UserClass.g_VisionManager._ToolAlign.fn_PinSearch(dPosX, dPosY, step, fn_OpenImage(strName)))
                            fn_WriteMessage($"Step : {step++}, PinSearched.");
                        break;
                    case 4:
                        UserClass.g_VisionManager._ToolAlign.fn_ProcessOffset(out Point coloffset, out Point rowoffset, out Point refpos);
                        fn_WriteMessage($"Process Offset : Col Offset ({coloffset.X:F3}, {coloffset.Y:F3}), Row Offset ({rowoffset.X:F3}, {rowoffset.Y:F3})");
                        
                        fn_WriteMessage($"Process Offset : RefToolPos ({refpos.X:F3}, {refpos.Y:F3})");
                        //fn_WriteMessage($"Process Offset : LBToolPos  ({UserClass.g_VisionManager._ToolAlign.pntPositions[0].X:F4}, {UserClass.g_VisionManager._ToolAlign.pntPositions[0].Y:F4})");
                        //fn_WriteMessage($"Process Offset : LTToolPos  ({UserClass.g_VisionManager._ToolAlign.pntPositions[1].X:F4}, {UserClass.g_VisionManager._ToolAlign.pntPositions[1].Y:F4})");
                        //fn_WriteMessage($"Process Offset : RTToolPos  ({UserClass.g_VisionManager._ToolAlign.pntPositions[2].X:F4}, {UserClass.g_VisionManager._ToolAlign.pntPositions[2].Y:F4})");
                        //fn_WriteMessage($"Process Offset : RBToolPos  ({UserClass.g_VisionManager._ToolAlign.pntPositions[3].X:F4}, {UserClass.g_VisionManager._ToolAlign.pntPositions[3].Y:F4})");

                        UserClass.g_VisionManager._ToolAlign.fn_GetDir(out char[] chDirV, out char[] chDirH);
                        UserClass.g_VisionManager._ToolAlign.fn_GetSearchPos(out Point[] pntSearched);

                        for (int i = 0; i < pntSearched.Length; i++)
                        {
                            dtData.Rows[i]["Dir"] = chDirH[i].ToString() + chDirV[i].ToString();
                            dtData.Rows[i]["SearchPosX"] = pntSearched[i].X.ToString("0.000");
                            dtData.Rows[i]["SearchPosY"] = pntSearched[i].Y.ToString("0.000");
                        }
                        step++;
                        break;
                    case 5:
                        fn_WriteMessage("End Step");
                        bWork = false;
                        step = 99;
                        break;
                    case 100:
                        g_VisionManager.fn_EPDResult(new Point(0, 0));
                        bWork = false;
                        break;
                }
            }
        }

        private void bn_AutoToolAlign_Click(object sender, RoutedEventArgs e)
        {
            //Auto Tool Calibration 
            int cnt = 1; 
            int.TryParse(tbCalCnt.Text, out cnt);
            fn_UpdateData();
            SEQ_SPIND._nAlignTestCount = cnt;
            MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0425, true, false);

            fn_WriteLog("[MASTER] Auto Tool Storage Calibration.");
        }

		private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            up_HWOffsetX.UPValue = UserClass.g_VisionManager._RecipeVision.HardwareOffsetX.ToString("0.000");
            up_HWOffsetY.UPValue = UserClass.g_VisionManager._RecipeVision.HardwareOffsetY.ToString("0.000");
        }

        private void fn_UpdateData()
        {
            try
            {
                UserClass.g_VisionManager._RecipeVision.HardwareOffsetX = Convert.ToDouble(up_HWOffsetX.UPValue);
                UserClass.g_VisionManager._RecipeVision.HardwareOffsetY = Convert.ToDouble(up_HWOffsetY.UPValue);
                UserClass.g_VisionManager._RecipeManager.fn_SaveHardwareOffset();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
		
        private void bn_Test1_Click(object sender, RoutedEventArgs e)
        {
            g_VisionManager.delUpdateMainresult?.Invoke("- Model_No : #1 , Score : 90, Shift XY : 0.02, 0.03, Angle : -0.5");
            g_VisionManager.delUpdateMainresult?.Invoke("- Model_No : #1 , Score : 90 (Set Score : 50)");
            g_VisionManager.delUpdateMainresult?.Invoke("- Model_No : #2 , Score : 90 (Set Score : 45)");
            g_VisionManager.delUpdateMainresult?.Invoke("- Model_No : #1 , Score : 85, Shift XY : 0.02, 0.03, Angle : -1.1 (Search_Shift : 1.0 mm, Search_Angle : -1.0 mm");
        }

        private void bn_Test2_Click(object sender, RoutedEventArgs e)
        {
            g_VisionManager.delUpdateMainresultClear?.Invoke();
            
        }
    }
}
