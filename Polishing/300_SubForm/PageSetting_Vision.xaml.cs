using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Diagnostics;
using UserInterface;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserEnumVision;
using System.Windows.Navigation;

namespace WaferPolishingSystem.Form
{
    public delegate void DelegateParamDouble(double dValue);
    //public delegate void DelegateSaveMark(string strPath);
    public delegate void DelegateParamBool(bool bSet);
    public delegate void DelegateParamInt(int index);
    public delegate void DelegateDefault();
    public delegate Point DelegateRetPoint();
    public delegate void DelegateParamRect(Rect rect);

    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Vision : Page
    {

        //Var
        PageToolStorage mc_PageTool = new PageToolStorage();

        Stopwatch stopWatch = new Stopwatch();

        UserButton _PrevBn;
        //int _nObjectIdx = (int)AlignControl.EnObjectSelect.SelModel;
        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        public PageSetting_Vision()
        {
            InitializeComponent();

            //---------------------------------------------------------------------------
            //Timer 생성
            m_UpdateTimer.IsEnabled = true;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;
            fn_SetPage(bn_Tool);
            
            
        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            //
            m_UpdateTimer.Start();
        }
        //---------------------------------------------------------------------------
        //Timer On/Off
        public void fn_SetTimer(bool on)
        {
            if (on)
                m_UpdateTimer.Start();
            else
                m_UpdateTimer.Stop();
        }

        private void fn_PageSwitch(object sender, RoutedEventArgs e)
        {
            fn_SetPage(sender as UserButton);
        }

        private void fn_SetPage(UserButton bn)
        {
            string strName = bn.Content as string;

            if (strName == "Tool Storage")
            {
                frame.Content = mc_PageTool;
            }

            if (_PrevBn != null)
                _PrevBn.Background = G_COLOR_BTNNORMAL;
            bn.Background = G_COLOR_BTNCLICKED;

            _PrevBn = bn;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            g_VisionManager._CamManager.fn_GrabStop();
        }

        private void frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
                e.Cancel = true;
        }
    }
}
