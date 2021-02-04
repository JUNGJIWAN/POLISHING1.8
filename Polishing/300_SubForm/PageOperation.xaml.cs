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

//User Using
using WaferPolishingSystem.Define;
using WaferPolishingSystem.BaseUnit;
//
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.FormMain;
using UserInterface;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.Define.UserConst;


namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageOperation.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageOperation : Page
    {

        //Var
        

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageMapDisplay pageMap  = new PageMapDisplay();
        public PageOperMain   pageOper = new PageOperMain  ();

        bool m_bMap = false;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public PageOperation()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false; 
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            frame.Content = pageOper;

        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            //m_UpdateTimer.Stop();
            m_UpdateTimer.IsEnabled = false; 
            

            //Error Check
            EPU.fn_ErrorDisplay();

            //Run Status
            fn_SetLabelColor(ref lbRun, SEQ._bRun, Brushes.Green , Brushes.DarkGray               );
            fn_SetLabelText (ref lbRun, SEQ._bRun, "RUN", "STOP", Brushes.Black, Brushes.Chocolate);

            //
            bool a =  SEQ._bRun && SEQ_CLEAN._bDrngCleaning && SEQ_CLEAN._bDrngRotateHome;
            bool b = (MOTR.IsAllHomeEnd() && MOTR.IsAllServoOn()) || a;
            //fn_SetLabelColor(ref btAllHome, MOTOR.IsAllHome(), Brushes.Lime, Brushes.LightGray);
            fn_SetLabel(ref lbAllHome,  b? "ALL HOME" : "NEED INIT...", b ? Brushes.White : Brushes.Brown, b ? Brushes.Black : SEQ._bFlick1? Brushes.Aqua : Brushes.Yellow);

            fn_SetLabelColor(ref lbAuto, SEQ._bAuto, Brushes.Green, Brushes.DarkBlue);
            lbAuto.Content    = SEQ._bAuto ? "AUTO" : "MANUAL";
            lbAuto.Foreground = SEQ._bAuto ? Brushes.Black : Brushes.LightYellow;

            //
            btDoor_O .IsEnabled = !SEQ._bRun; 
            btDoor_C .IsEnabled = !SEQ._bRun; 
            lbAllHome.IsEnabled = !SEQ._bRun;
            lbAuto   .IsEnabled = !SEQ._bRun;
            //btSDoor_O.IsEnabled = !SEQ._bRun;
            //btSDoor_C.IsEnabled = !SEQ._bRun;


            //btReset.Background  = EPU.fn_GetHasErr()? Brushes.Green : SEQ._bFlick1 ? Brushes.Gray : Brushes.Yellow;

            //btDoor_O.Background = (!IO.XV[(int)EN_INPUT_ID.xSYS_DR_Right_KeyIn] || !IO.XV[(int)EN_INPUT_ID.xSYS_DR_Left_KeyIn]) ? Brushes.MintCream : Brushes.MintCream;
            //btDoor_C.Background = ( IO.XV[(int)EN_INPUT_ID.xSYS_DR_Right_KeyIn ] &&  IO.XV[(int)EN_INPUT_ID.xSYS_DR_Left_KeyIn]) ? Brushes.MintCream : Brushes.MintCream;
            
            bool bDoorOutput =  IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontLeft] || IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontRight] || IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_Side        ];
            bool bDoorKeyIn  =  IO.XV[(int)EN_INPUT_ID .xSYS_DR_Right_KeyIn] && IO.XV[(int)EN_INPUT_ID .xSYS_DR_Left_KeyIn  ] && IO.XV[(int)EN_INPUT_ID .xSYS_DR_SideDoor_KeyIn];
            bool bDoorActrOk =  IO.XV[(int)EN_INPUT_ID .xSYS_DR_MainClose_R] && IO.XV[(int)EN_INPUT_ID .xSYS_DR_MainClose_L ] && IO.XV[(int)EN_INPUT_ID .xSYS_DR_SideDoorClose ];
           
            btDoor_O.Background = (!bDoorOutput && bDoorKeyIn && bDoorActrOk) ? G_COLOR_BTNNORMAL : Brushes.Lime;



            //JUNG/200525/삭제
            //bool bDoorY = IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontLeft] == true || IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontRight] == true ||
            //              IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_Side     ] == true;
            //btDoor_O .Background = bDoorY ? Brushes.Gray : Brushes.WhiteSmoke;


            //btSDoor_O.Background = ( IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_Side     ] == true) ? Brushes.Gray : Brushes.WhiteSmoke;


            //
            btLight.Background = IO.YV[(int)EN_OUTPUT_ID.ySYS_Light]? Brushes.Lime : new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF7, 0xF7)); 

            //
            m_UpdateTimer.IsEnabled = true;
        }

        //---------------------------------------------------------------------------
        //Timer On/Off
        public void fn_SetTimer(bool on)
        {
            if (on) m_UpdateTimer.Start();
            else    m_UpdateTimer.Stop();

        }

        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //
            
        }
        //---------------------------------------------------------------------------
        //For TEST
        private void Button_Click(object sender, RoutedEventArgs e)
        {
   


        }
        //---------------------------------------------------------------------------
        private void btLevel_Click(object sender, RoutedEventArgs e)
        {
            MAIN.fn_ShowPassword();
        }
        //---------------------------------------------------------------------------
        private void btMap_Click(object sender, RoutedEventArgs e)
        {
            //
            m_bMap = !m_bMap; 

            frame.Content    = m_bMap ? (Page)pageMap : (Page)pageOper;
            btMap.Background = m_bMap ?  G_COLOR_BTNCLICKED : G_COLOR_BTNNORMAL;


        }
        //-------------------------------------------------------------------------------------------------
        public void fn_MapHide()
        {
            m_bMap = false; 

            frame.Content    = (Page)pageOper;
            btMap.Background = G_COLOR_BTNNORMAL;
        }
        //---------------------------------------------------------------------------
        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            UserButton selbtn = sender as UserButton;

            string sBtn = selbtn.Content as string;

            switch (sBtn)
            {
                case "RUN":
                    if(!SEQ._bAuto && FM.m_stMasterOpt.nRunMode == EN_RUN_MODE.AUTO_MODE) //JUNG/200824
                    {
                        fn_UserMsg("CHECK AUTO SWITCH.");
                        return;
                    }
                    
                    ////LEE/200929 [Add]
                    //if (!SEQ_STORG.fn_IsLockPos() && SEQ_STORG._nManStep == 0)
                    //{
                    //    MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0445, true, false);  //SEQ_STORG.fn_ReqMoveToolChkPos();
                    //}
                    //if (SEQ_STORG._nManStep > 0)
                    //{
                    //    return;
                    //}
                    //----------------------------------------------------------------------------------------------------

                    SEQ._bBtnWinStart = true; 
                    break;

                case "STOP":
                    SEQ._bBtnWinStop = true;
                    break;

                case "RESET":
                    //SEQ.fn_Reset();
                    SEQ._bBtnWinReset = true; 
                    break;

                default:
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void lbAuto_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (SEQ._bRun) return; 
            //
            ////Auto -> Manual 변경 시 
            //if (SEQ._bAuto)
            //{
            //    if(fn_UserMsg("Change Manual Mode??", EN_MSG_TYPE.Check, "MODE CHANGE..."))
            //    {
            //        IO.YV[(int)EN_OUTPUT_ID.ySW_AutoUnlock] = true; 
            //    }
            //}
        }
        //---------------------------------------------------------------------------
        private void btDoor_M_Click(object sender, RoutedEventArgs e)
        {
            //Door Open Button Click
            UserButton selBtn = sender as UserButton;

            switch (selBtn.Name)
            {
                case "btDoor_O": //Main Door Open
                    
                    if (SEQ._bAuto) return; //JUNG/200814

                    SEQ.fn_SetDoorLock     (ccOpen);
                    SEQ.fn_SetDoorLock_Side(ccOpen);
                    break;

                case "btDoor_C": //Main Door Close
                    SEQ.fn_SetDoorLock     (ccClose);
                    SEQ.fn_SetDoorLock_Side(ccClose);
                    break;

                case "btSDoor_O": //Side door open
                    SEQ.fn_SetDoorLock_Side(ccOpen);
                    break;

                case "btSDoor_C": //Side Door Close
                    SEQ.fn_SetDoorLock_Side(ccClose);
                    break;

                default:
                    break;
            }
        }
        //---------------------------------------------------------------------------
        private void btLight_Click(object sender, RoutedEventArgs e)
        {
            //LIGHT
            //SEQ._bLightOn = !SEQ._bLightOn; 
            SEQ.fn_SetLight(!IO.YV[(int)EN_OUTPUT_ID.ySYS_Light]);

        }
        //---------------------------------------------------------------------------
        private void lbAllHome_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //
            if (fn_UserMsg("Do you want All Home?", EN_MSG_TYPE.Check))
            {
                //Home Seq.
                MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0001, true, false);

                //Log
                fn_WriteLog("Click All Home");
            }

        }
        //---------------------------------------------------------------------------
        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Forward)
                e.Cancel = true;
        }
    }
}
