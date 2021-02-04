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

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageMaster.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageMaster : Page
    {
        //Var
        UserButton _prevButton ; 

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();


        //Sub Page
        private PageMaster_ACS     mc_Motor   = new PageMaster_ACS    ();
        private PageMaster_Flag    mc_Flag    = new PageMaster_Flag   ();
        private PageMaster_OneShot mc_Shot    = new PageMaster_OneShot();
        private PageMaster_Option  mc_Option  = new PageMaster_Option ();
        private PageMaster_ACSData mc_ACSData = new PageMaster_ACSData();
        private PageSetting_Vision mc_Vision  = new PageSetting_Vision();
        //
        private int m_nNowPage;

        private enum EN_PAGE
        {
            MOTOR ,
            ONECYCLE,
            FLAG,
            OPTION,
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public PageMaster()
        {
            InitializeComponent();

            m_nNowPage = 0;

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background         = UserConst.G_COLOR_PAGEBACK;
            this.GridSub.Background = UserConst.G_COLOR_SUBMENU ;

            _prevButton = menuOption;


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
            {
                m_UpdateTimer.IsEnabled = true;
                m_UpdateTimer.Start();
            }
            else
                m_UpdateTimer.Stop();

        }
        //---------------------------------------------------------------------------
        private void menuMotor_Click(object sender, RoutedEventArgs e)
        {
            UserInterface.UserButton menubn = (sender as UserInterface.UserButton);
            m_fn_SetPage(menubn);

        }
        //---------------------------------------------------------------------------
        private void m_fn_SetPage(UserInterface.UserButton bn)
        {
            string strName = bn.Content as string;

            if(_prevButton != null) _prevButton.Background = UserConst.G_COLOR_BTNNORMAL;

            bn.Background = UserConst.G_COLOR_BTNCLICKED;

            if      (strName == "MOTOR"      ) frame.Content = mc_Motor  ;
            else if (strName == "FLAG"       ) frame.Content = mc_Flag   ;
            else if (strName == "ONE CYCLE"  ) frame.Content = mc_Shot   ;
            else if (strName == "OPTION"     ) frame.Content = mc_Option ;
            else if (strName == "ACS DATA"   ) frame.Content = mc_ACSData;
            else if (strName == "TOOL ALIGN" ) frame.Content = mc_Vision ;
            else                               frame.Content = mc_Option ;

            _prevButton = bn;
            m_nNowPage = Convert.ToInt32(bn.Tag);
        }
        //---------------------------------------------------------------------------
        public void fn_SetPage()
        {
            if(m_nNowPage < 0) frame.Content = mc_Motor;
            else
            {
                if(m_nNowPage == 0)
                {
                    frame.Content = mc_Motor;
                }
                else if (m_nNowPage == 1)
                {
                    //frame.Content = mc_Motor;
                }

            }
        }
        //---------------------------------------------------------------------------
        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            //Save Button
            switch ((EN_PAGE) m_nNowPage)
            {
                case EN_PAGE.MOTOR:
                    break;
                case EN_PAGE.ONECYCLE:
                    break;
                case EN_PAGE.FLAG:
                    break;
                case EN_PAGE.OPTION:
                    mc_Option.fn_SaveToBuff();
                   
                    break;
                default:
                    break;
            }

        }
        //-------------------------------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            m_fn_SetPage(_prevButton);
        }
        //---------------------------------------------------------------------------
        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
                e.Cancel = true;
        }
        //---------------------------------------------------------------------------
        public void fn_DisplayOffset()
        {
            mc_Option.fn_DisplayOffset();
        }
    }
}
