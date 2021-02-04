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
using UserInterface;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserConst;


namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Manual : Page
    {
        
        //Var
        UserButton _PrevBn;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //
        private PageSetting_Manual_Spindle  mc_Man01 = new PageSetting_Manual_Spindle ();
        private PageSetting_Manual_Loadcell mc_Man02 = new PageSetting_Manual_Loadcell();
        private PageSetting_Manual_Slurry   mc_Man03 = new PageSetting_Manual_Slurry  ();
        private PageSetting_Manual_WHM      mc_Man04 = new PageSetting_Manual_WHM     ();
        private PageSetting_Manual_PMC      mc_Man05 = new PageSetting_Manual_PMC     ();
        private PageSetting_Manual_Valve    mc_Man06 = new PageSetting_Manual_Valve   ();
        private PageSetting_Manual_RF       mc_Man07 = new PageSetting_Manual_RF      ();
        private PageSetting_Manual_REST     mc_Man08 = new PageSetting_Manual_REST    ();


        public PageSetting_Manual()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = true;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            fn_SetPageName();

            fn_SetPage(bnManu01);
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
            if (on) m_UpdateTimer.Start();
            else    m_UpdateTimer.Stop();

        }
        //---------------------------------------------------------------------------
        private void fn_SetPageName()
        {
            //
            bnManu01.Content = "SPINDLE"     ; bnManu01.Visibility = Visibility.Visible;
            bnManu02.Content = "LOADCELL"    ; bnManu02.Visibility = Visibility.Visible;
            bnManu03.Content = "AUTO SUPPLY" ; bnManu03.Visibility = Visibility.Visible;
            bnManu04.Content = "VALVE"       ; bnManu04.Visibility = Visibility.Visible;
            
            bnManu05.Content = "PMC"         ; bnManu05.Visibility = Visibility.Hidden;

            bnManu06.Content = "RF READER"   ; bnManu06.Visibility = Visibility.Visible;
            bnManu07.Content = "REST API"    ; bnManu07.Visibility = Visibility.Visible;

            bnManu08.Content = "LEVEL SENSOR"; bnManu08.Visibility = Visibility.Hidden;
            bnManu09.Content = "AUTO SUPPLY" ; bnManu09.Visibility = Visibility.Hidden;
            bnManu10.Content = "SPINDLE"     ; bnManu10.Visibility = Visibility.Hidden;

        }
        //---------------------------------------------------------------------------
        private void fn_PageSwitch(object sender, RoutedEventArgs e)
        {
            fn_SetPage(sender as UserButton);
        }
        //---------------------------------------------------------------------------
        private void fn_SetPage(UserButton bn)
        {
            string strName = bn.Content as string;

                 if (strName == "SPINDLE"    )  frame.Content = mc_Man01;
            else if (strName == "LOADCELL"   )  frame.Content = mc_Man02;
            else if (strName == "AUTO SUPPLY")  frame.Content = mc_Man03;
            else if (strName == "WHM"        )  frame.Content = mc_Man04;
            else if (strName == "PMC"        )  frame.Content = mc_Man05;
            else if (strName == "VALVE"      )  frame.Content = mc_Man06;
            else if (strName == "RF READER"  )  frame.Content = mc_Man07;
            else if (strName == "REST API"   )  frame.Content = mc_Man08;


            //
            if (_PrevBn != null) _PrevBn.Background = G_COLOR_BTNNORMAL;
            bn.Background = G_COLOR_BTNCLICKED;

            _PrevBn = bn;
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //fn_SetPageName();
        }
        //---------------------------------------------------------------------------
        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
                e.Cancel = true;
        }
    }
}
