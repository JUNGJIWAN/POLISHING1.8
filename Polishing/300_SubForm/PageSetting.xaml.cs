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
using WaferPolishingSystem.Define;
using UserInterface;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserEnum;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting : Page
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Var
        UserButton _prevButton;
        int        m_nNowPage; 

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        private PageSetting_IO       mc_Io     = new PageSetting_IO      ();
        private PageSetting_Option   mc_Option = new PageSetting_Option  ();
        private PageSetting_Vision   mc_Vision = new PageSetting_Vision  ();
        private PageSetting_Error    mc_Error  = new PageSetting_Error   ();
        private PageSetting_Motor    mc_Motor  = new PageSetting_Motor   ();
        private PageSetting_LampBuzz mc_Lamp   = new PageSetting_LampBuzz();
        private PageSetting_Manual   mc_Man    = new PageSetting_Manual  ();
        private PageSetting_Actr     mc_Actr   = new PageSetting_Actr    ();

        private enum EN_PAGE
        {
            IO, 
            OPTION, 
            VISION,
            ERROR,
            MOTOR,
            LAMP, 
            MANUAL, 
            ACTUATOR,
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public PageSetting()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = true;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            System.Windows.Interop.ComponentDispatcher.ThreadIdle += MenuInit;

            //Back Color Set
            this.Background         = UserConst.G_COLOR_PAGEBACK;
            this.GridSub.Background = UserConst.G_COLOR_SUBMENU ;

            m_nNowPage = 0;
// #if DEBUG
//     menuVision.Visibility = Visibility.Visible;
// #endif
        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();


            menuSave.IsEnabled = (SEQ._bAuto || SEQ._bRun) ? false : true;

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

        //---------------------------------------------------------------------------
        private void m_fn_SetPage(UserInterface.UserButton bn)
        {
            string strName = bn.Content as string;

            if (_prevButton != null) _prevButton.Background = UserConst.G_COLOR_BTNNORMAL;
            
            bn.Background = UserConst.G_COLOR_BTNCLICKED;

            if      (strName == "IO"       ) frame.Content = mc_Io    ;
            else if (strName == "OPTION"   ) frame.Content = mc_Option;
            else if (strName == "VISION"   ) frame.Content = mc_Vision;
            else if (strName == "ERROR"    ) frame.Content = mc_Error ;
            else if (strName == "MOTOR"    ) frame.Content = mc_Motor ;
            else if (strName == "LAMP/BUZZ") frame.Content = mc_Lamp  ;
            else if (strName == "MANUAL"   ) frame.Content = mc_Man   ;
            else if (strName == "ACTUATOR" ) frame.Content = mc_Actr  ;

            //
            _prevButton = bn;
            m_nNowPage = Convert.ToInt32(bn.Tag);
        }
        //---------------------------------------------------------------------------
        private void MenuInit(object sender, EventArgs e)
        {
            System.Windows.Interop.ComponentDispatcher.ThreadIdle -= MenuInit;
            m_fn_SetPage(menuIO);
        }
        //---------------------------------------------------------------------------
        private void MenuChange(object sender, RoutedEventArgs e)
        {
            UserInterface.UserButton menubn = (sender as UserInterface.UserButton);

            m_fn_SetPage(menubn);
        }
        //---------------------------------------------------------------------------
        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            //Run Check
            if (SEQ._bAuto || SEQ._bRun) return; 
            
            //Save
            switch ((EN_PAGE) m_nNowPage)
            {
                case EN_PAGE.IO:
                    IO.fn_LoadIO(false);
                    fn_WriteLog("[Setting Save] IO");
                    break;
                case EN_PAGE.OPTION:
                    mc_Option.fn_DisplayOptionData(false);
                    fn_WriteLog("[Setting Save] OPTION");
                    break;

                case EN_PAGE.VISION:

                    fn_WriteLog("[Setting Save] VISION");
                    break;

                case EN_PAGE.ERROR:
                    EPU.fn_LoadErrorData(false);

                    fn_WriteLog("[Setting Save] ERROR");
                    break;

                case EN_PAGE.MOTOR:
                    mc_Motor.fn_GridToBuff();
                    MOTR.Load(false, FM._sCurrJob);
                    FM.fn_LoadMastOptn(false); //JUNG/200723
                    fn_WriteLog("[Setting Save] MOTOR");
                    break;
                
                case EN_PAGE.LAMP:
                    mc_Lamp.fn_GridToBuff();
                    LAMP.fn_Load(false);

                    fn_WriteLog("[Setting Save] LAMP");
                    break;

                case EN_PAGE.ACTUATOR:
                    mc_Actr.fn_GridToBuff();
                    ACTR.fn_Load(false);
                    fn_WriteLog("[Setting Save] ACTUATOR");
                    break;

                default:
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // MasterLevel에서만 Vision Setting Menu 동작
//             if (FM._nCrntLevel == (int)EN_USER_LEVEL.lvMaster)
//                 menuVision.Visibility = Visibility.Visible;
//             else
//                 menuVision.Visibility = Visibility.Hidden;
        }
        //---------------------------------------------------------------------------
        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
                e.Cancel = true;
        }
    }
}
