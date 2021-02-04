using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using UserInterface;
using static WaferPolishingSystem.Define.UserClass;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageOperation_MainVision.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageOperation_MainVision : Page
    {
        public ObservableCollection<string> ListResult = new ObservableCollection<string>();
        public PageOperation_MainVision()
        {
            InitializeComponent();
            //g_VisionManager._CamManager.fn_AddEvent(ac_Main.OnImageGrabed);
            //ac_Main.ZoomScale(0.13);
            ac_Main.SetMove(AlignControl.EnRoiMode.ModeMove);
            ac_Main.fn_EnableDragButton(false);
            g_VisionManager._CamManager.update += ac_Main.SetImage;
            g_VisionManager._AlginMainCtrl = ac_Main;
            g_VisionManager.lbMain = lb_MainImageName;
            lb_Result.ItemsSource = ListResult;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
//             if(g_VisionManager._CamManager.CamBuffer != null)
//             {
//                 ac_Main.SetImage(g_VisionManager._CamManager.CamBuffer,
//                     (int)g_VisionManager._CamManager._Width,
//                     (int)g_VisionManager._CamManager._Height);
//             }
            ac_Main.SetFitScale();
            g_VisionManager.delUpdateMainresult = fn_SetData;
            g_VisionManager.delUpdateMainresultClear = fn_ClearData;
        }

        private void lb_MainImageName_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Process.Start(g_VisionManager._strImageLogPath);
                string strPath;
                strPath = @"D:\Image\" + DateTime.Now.ToString("yyyyMMdd") + @"\";
                if (!System.IO.Directory.Exists(strPath))
                {
                    strPath = @"D:\Image\";
                }
                Process.Start(strPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void bn_ManualAlign_Click(object sender, RoutedEventArgs e)
        {
            FormMain.FormMnlAlign.ShowDialog();
            //fn_SetData();
        }

        private void fn_SetData(string strResult)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ListResult.Add(strResult);
            }));
        }

        private void fn_ClearData()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ListResult.Clear();
            }));
        }
    }
}
