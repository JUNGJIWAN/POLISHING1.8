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
using System.Threading;
using System.Diagnostics;
using WaferPolishingSystem.Define  ;
using WaferPolishingSystem.BaseUnit;
using WaferPolishingSystem.Form;
using WaferPolishingSystem.Unit;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserClass;
using System.Windows.Controls.Primitives;
using System.Security.Authentication.ExtendedProtection.Configuration;

namespace WaferPolishingSystem
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormMain : Window
    {
        //---------------------------------------------------------------------------
        //Structure
        #region [Structure Define]

        enum EN_PAGE_NO
        {
            Oper = 0,
            Motion  ,
            Setting ,
            Recipe  ,
            Log     ,
            Master  ,
            AllPage
        }

        #endregion

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Variable 
        #region [Variable Define]

        int m_nPageSel;

        bool m_bReqOperLevel; 

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //Delay Time
        private TOnDelayTimer m_tDelayTimer = new TOnDelayTimer();

        #endregion


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Page 선언
        #region [Page/Form Init]

        static public PageOperation   PgOper       = new PageOperation  ();
        static public PageMotion      PgMotion     = new PageMotion     ();
        static public PageSetting     PgSettting   = new PageSetting    ();
        static public PageRecipe      PgRecipe     = new PageRecipe     ();
        static public PageLog         PgLog        = new PageLog        ();
        static public PageMaster      PgMaster     = new PageMaster     ();
                                                                        
        static public FormMessage     UserMsg      = new FormMessage    ();
        static public FormPassword    FormPass     = new FormPassword   ();
        static public FormAlarmDlg    FormAlarm    = new FormAlarmDlg   ();
        static public FormUpdateList  FormInfo     = new FormUpdateList ();
        static public FormStorageMap  FormMapStrg  = new FormStorageMap ();
        static public FormMagazineMap FormMapMAGZ  = new FormMagazineMap();
        static public FormManualAlign FormMnlAlign = new FormManualAlign();
        #endregion

        //---------------------------------------------------------------------------
        //Base Unit                            
        #region [Base Unit]          

        static public LogUnit        LOG         = new LogUnit     ();
        static public FileManager    FM          = new FileManager ();
        static public DataManager    DM          = new DataManager ();
        static public ErrorProcess   EPU         = new ErrorProcess();
        static public LampBuzz       LAMP        = new LampBuzz    ();
        static public IoUnit         IO          = new IoUnit      ();
        static public TSysActuator   ACTR        = new TSysActuator();
        static public SysMotor       MOTR        = new SysMotor    ();
        static public SequenceUnit   SEQ         = new SequenceUnit();
        static public ManProcess     MAN         = new ManProcess  ();
        static public LotUnit        LOT         = new LotUnit     ();
        static public TickTime       TICK        = new TickTime    ();
        static public SpcManager     SPC         = new SpcManager  ();

        //Unit                                   
        static public SeqSpindle     SEQ_SPIND   = new SeqSpindle  ();
        static public SeqPolishing   SEQ_POLIS   = new SeqPolishing(); 
        static public SeqCleaning    SEQ_CLEAN   = new SeqCleaning (); 
        static public SeqStorage     SEQ_STORG   = new SeqStorage  (); 
        static public SeqTransfer    SEQ_TRANS   = new SeqTransfer (); 
                                                 
        //Thread                                 
        private       ThreadUnit     THREAD      = new ThreadUnit  ();
        

        //                                       
        static public PMCUnit        PMC         = new PMCUnit      ();
        static public FutekLoadcell  LDCBTM      = new FutekLoadcell();
        static public AutoSupply[]   SUPPLY      = new AutoSupply [2] ;
        static public RFIDUnit       RFID        = new RFIDUnit     ();
        static public RESTUnit       REST        = new RESTUnit     ();

        static public FormMain       MAIN        = null;


        //Property
        public bool _bReqOperLevel
        {
            get { return m_bReqOperLevel ; }
            set { m_bReqOperLevel = value; }
        }

        #endregion

        //---------------------------------------------------------------------------
        public FormMain()
        {

            FormLoad formload = new FormLoad();
            formload.ShowDialog();
            
            InitializeComponent();

            //Init
            fn_Init();

            //Sub Page 생성
            fn_CreatePage();


            SUPPLY[SPLY_SLURRY] = new AutoSupply(SPLY_SLURRY); //Polishing
            SUPPLY[SPLY_SOAP  ] = new AutoSupply(SPLY_SOAP  ); //Cleaning

            SUPPLY[SPLY_SLURRY].fn_SetAddress(FM.m_stSystemOpt.sSupplyIp , FM.m_stSystemOpt.nSupplyPort , FM.m_stSystemOpt.nSupplyAddress, FM.m_stSystemOpt.sSupplyEqpId );
            SUPPLY[SPLY_SOAP  ].fn_SetAddress(FM.m_stSystemOpt.sSupplyIp1, FM.m_stSystemOpt.nSupplyPort1, FM.m_stSystemOpt.nSupplyAddress, FM.m_stSystemOpt.sSupplyEqpId1);


            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);
            
            //
            MAIN = this;
           
        }
        //---------------------------------------------------------------------------
        private void fn_Init()
        {
            m_nPageSel = -1 ;

            lbMsg .Visibility = Visibility.Hidden;
            lbWarm.Visibility = Visibility.Hidden;

            m_tDelayTimer.Clear();

        }
        //---------------------------------------------------------------------------
        private void fn_CreatePage()
        {
            //Background Color Set
            this.Background = UserConst.G_COLOR_FORMBACK; 

            //Tag Setting
            menuOperation.Tag = 0;
            menuMotion   .Tag = 1;
            menuSetting  .Tag = 2;
            menuRecipe   .Tag = 3;
            menuLog      .Tag = 4; 
            menuMaster   .Tag = 5;

            //

        }
        //---------------------------------------------------------------------------
        //Timer On/off
        public void fn_TimerSet(bool on)
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
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            //Current Time
            tb05.Text = DateTime.Now.ToString("yyyy-MM-dd");
            tb06.Text = DateTime.Now.ToString("HH:mm:ss");

            
            //Manual No
            lbStbar01 .Content = string.Format($"[ MAN : {MAN._nManNo:D4} ]");

            //Scan Time
            //lbStbar021.Content = string.Format($"[TH0 : {THREAD.m_nScanTime[0]:D3} ms] ");
            lbStbar021.Content = string.Format($"[TH0 : {THREAD.fn_GetScanTime(0):00.00} ms] ");
            lbStbar022.Content = string.Format($"[TH1 : {THREAD.m_nScanTime[1]:D3} ms] ");
            //lbStbar022.Content = string.Format($"[TH1 : {THREAD.fn_GetScanTime(1):F1} ms] ");
            lbStbar023.Content = string.Format($"[TH2 : {THREAD.m_nScanTime[2]:D3} ms] ");
            lbStbar024.Content = string.Format($"[TH3 : {THREAD.m_nScanTime[3]:D3} ms] ");
            lbStbar025.Content = string.Format($"[TH4 : {THREAD.m_nScanTime[4]:D3} ms] ");
            //lbStbar026.Content = string.Format($"[TH5 : {THREAD.m_nScanTime[5]:D3} ms] "); //TCP/IP
            lbStbar026.Content = string.Format($"[TH6 : {THREAD.m_nScanTime[6]:D3} ms] "); //Slurry
            lbStbar027.Content = string.Format($"[TH7 : {THREAD.m_nScanTime[7]:D3} ms] "); //REST, RFID



            lbStbar029.Content = string.Format($"[TH8 : {THREAD.m_nScanTime[8]:D3} ms] ");
            lbStbar030.Content = string.Format($"[TH9 : {THREAD.m_nScanTime[9]:D3} ms] ");
            //lbStbar031.Content = string.Format($"[TH10: {THREAD.m_nScanTime[10]:D3} ms] ");


            lbStbar04 .Content = MAN._bDrngWarm? string.Format($"[During Warming...({MAN._nWarmStep}/23)]") : "";
            lbStbar05 .Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //Time

            //Run Status
            fn_SetLabelColor(ref lbRun, SEQ._bRun, Brushes.Green, Brushes.DarkGray                );
            fn_SetLabelText (ref lbRun, SEQ._bRun, "RUN", "STOP", Brushes.Black, Brushes.Chocolate);

            //check Request Change Operator Level
            if (m_bReqOperLevel)
            {
                m_bReqOperLevel = false;
                fn_ChangeLevel(EN_USER_LEVEL.lvOperator);
            }

            //Connection Display
            fn_SetLabelColor(ref lbIO   , (IO  ._bConnect && !IO._bDrngReboot), Brushes.Lime, Brushes.Red, true, Brushes.Gray);
            fn_SetLabelColor(ref lbMotor, (MOTR._bConnect && !IO._bDrngReboot), Brushes.Lime, Brushes.Red, true, Brushes.Gray);
            fn_SetLabelColor(ref lbRFID , RFID._bConnect, Brushes.Lime, Brushes.Red, true, Brushes.Gray);

            fn_SetLabelColor(ref lbREST , REST._bConnect, Brushes.Lime, Brushes.Red, true, Brushes.Gray); 

            fn_SetLabelColor(ref lbCam  , g_VisionManager._CamManager._bConnect , Brushes.Lime, Brushes.Red, true, Brushes.Gray);
            fn_SetLabelColor(ref lbLight, g_VisionManager._LightManger._bConnect, Brushes.Lime, Brushes.Red, true, Brushes.Gray);
            
            fn_SetLabelColor(ref lbLDC  , LDCBTM._bConect, Brushes.Lime, Brushes.Red, true, Brushes.Gray);

            fn_SetLabelColor(ref lbUTIL1, SUPPLY[SPLY_SLURRY]._bConnect, Brushes.Lime, Brushes.Red, true, Brushes.Gray);
            fn_SetLabelColor(ref lbUTIL2, SUPPLY[SPLY_SOAP  ]._bConnect, Brushes.Lime, Brushes.Red, true, Brushes.Gray);

            if(FM.m_stSystemOpt.nUseAutoSlurry == 1)
            { 
                lbUTIL1.Content = SUPPLY[SPLY_SLURRY]._bDrngCon? "Connect..." : "SLURRY";
                lbUTIL2.Content = SUPPLY[SPLY_SOAP  ]._bDrngCon? "Connect..." : "SOAP";
            }
            else
            {
                lbUTIL1.Content = "Not Used"; //JUNG/201014
                lbUTIL2.Content = "Not Used";
            }

            bool isOperPage = m_nPageSel == (int)EN_PAGE_NO.Oper;
            lbWarm.Visibility = isOperPage && (MAN._bStartWarm || MAN._bDrngWarm) && SEQ._bFlick1 ? Visibility.Visible : Visibility.Hidden;
            lbWarm.Content    =  MAN._bDrngWarm ? "Warming up now..." : "Wait Warming up";
            lbWarm.Background =  MAN._bDrngWarm ? Brushes.Aquamarine : Brushes.Gray;



            if (SEQ._bRun)
            {
                lbMsg.Visibility = (SEQ_SPIND._bDrngVisnInspA || SEQ_SPIND._bDrngVisnInspL) ? Visibility.Visible : Visibility.Hidden;
                lbMsg.Content    = string.Format($"VISION INSPECTION...(STEP : {SEQ_SPIND._nVisnInspStep} / Retry:{SEQ_SPIND._nVisnRetry})");
                lbMsg.Background = Brushes.Gray;
            }
            else
            {
                if (MAN._bStartWarm || MAN._bDrngWarm)
                {
                    lbMsg.Visibility = SEQ._bFlick1 ? Visibility.Visible : Visibility.Hidden;
                    lbMsg.Content = "(RESET 시 중지)";
                    lbMsg.Background = Brushes.Gray; 
                }
                else if (MAN._bHoming)
                {
                    lbMsg.Visibility = SEQ._bFlick1 ? Visibility.Visible : Visibility.Hidden;
                    lbMsg.Content    = "Homing...";
                    lbMsg.Background = Brushes.Brown;
                }
                else
                {
                    lbMsg.Visibility = Visibility.Hidden;
                }


            }

            //Mode Display
            //if (FM.m_stSystemOpt.nRunMode == (int)EN_RUN_MODE.AUTO_MODE)
            //{
            //    lbMode.Content = "AUTO";
            //    lbMode.Background = Brushes.Aqua; 
            //} 
            //else if (FM.m_stSystemOpt.nRunMode == (int)EN_RUN_MODE.MAN_MODE)
            //{
            //    lbMode.Content = "MANUAL";
            //    lbMode.Background = Brushes.Brown;
            //}
            //else
            //{
            //    lbMode.Content = "OFFLINE";
            //    lbMode.Background = Brushes.Gray;
            //}

            if (SEQ._bAuto) //JUNG/200825
            {
                lbMode.Content = "AUTO";
                lbMode.Background = Brushes.Green;
            }
            else
            {
                lbMode.Content = "MANUAL";
                lbMode.Background = Brushes.Gray;
            }

            //
            switch (SEQ._iSeqStat)
            {
                case (int)EN_SEQ_STATE.INIT:
                    fn_SetLabel(ref lbState, "INIT", SEQ._bFlick1? Brushes.Brown : Brushes.Yellow);
                    break;
                case (int)EN_SEQ_STATE.ERROR:
                    
                    if(SEQ._bFlick1)
                    {
                        fn_SetLabel(ref lbState, "ERROR", Brushes.Red, Brushes.Yellow);
                    }
                    else
                    {
                        fn_SetLabel(ref lbState, "ERROR", Brushes.Yellow, Brushes.Red);
                    }
                    //fn_SetLabel(ref lbState, "ERROR", SEQ._bFlick1 ? Brushes.Red : Brushes.Yellow);
                    
                    break;
                case (int)EN_SEQ_STATE.RUNNING:
                    fn_SetLabel(ref lbState, "RUNNING", Brushes.Lime);
                    break;
                case (int)EN_SEQ_STATE.STOP:
                    fn_SetLabel(ref lbState, "STOP", Brushes.Gray);
                    break;
                case (int)EN_SEQ_STATE.WARNING:
                    fn_SetLabel(ref lbState, "WARNING", Brushes.Yellow);
                    break;
                case (int)EN_SEQ_STATE.RUNWARN:
                    fn_SetLabel(ref lbState, "RUN_WARN", SEQ._bFlick2 ? Brushes.Lime : Brushes.Yellow);
                    break;

                default:
                    break;
            }

            //Form Message Display







            //
            m_UpdateTimer.Start();
            
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Menu Button Click
        </summary>
        <param name=""></param>
        @author    정지완(JUNGJIWAN)
        @date      2020/01/29 20:25
        */
        private void menuOperation_Click(object sender, RoutedEventArgs e)
        {
            UserInterface.UserButton btMenu = (sender as UserInterface.UserButton);
            
            int nPageNo = Convert.ToInt32(btMenu.Tag);

            PgOper.fn_MapHide();

            fn_PageShow(nPageNo);
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Page Hide Function
        </summary>
        @author    정지완(JUNGJIWAN)
        @date      2020/01/30 15:54
        */
        private void fn_PageHide()
        {
            //
            switch ((EN_PAGE_NO)m_nPageSel)
            {
                case EN_PAGE_NO.Oper   :
                    PgOper.fn_SetTimer(false);
                    menuOperation.Background = UserConst.G_COLOR_BTNNORMAL;

                    break;          

                case EN_PAGE_NO.Motion :
                    PgMotion.fn_SetTimer(false);
                    menuMotion.Background = UserConst.G_COLOR_BTNNORMAL;
                    break;

                case EN_PAGE_NO.Setting:
                    PgSettting.fn_SetTimer(false);
                    menuSetting.Background = UserConst.G_COLOR_BTNNORMAL;
                    break;

                case EN_PAGE_NO.Recipe :
                    PgRecipe.fn_SetTimer(false);
                    menuRecipe.Background = UserConst.G_COLOR_BTNNORMAL;
                    break;

                case EN_PAGE_NO.Log    :

                    menuLog.Background = UserConst.G_COLOR_BTNNORMAL;
                    break;

                case EN_PAGE_NO.Master :
                    PgMaster.fn_SetTimer(false);
                    menuMaster.Background = UserConst.G_COLOR_BTNNORMAL;
                    
                    break;

                
                default:
                    break;
            }
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Page Display
        </summary>
        <param name="nPage"> 선택한 Page No </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/01/30 15:53
        */
        public void fn_PageShow(int nPage)
        {
            if (m_nPageSel == nPage) return;

            fn_PageHide();

            switch ((EN_PAGE_NO)nPage)
            {
                case EN_PAGE_NO.Oper:
                    fn_WriteLog("Page_Operation Click");
                    PgOper.fn_SetTimer(true);
                    if(FM._nCrntLevel < (int)EN_USER_LEVEL.lvMaster) fn_ChangeLevel(EN_USER_LEVEL.lvOperator);
                    frame.Content = PgOper;
                    menuOperation.Background = G_COLOR_BTNCLICKED;

                    break;

                case EN_PAGE_NO.Motion:
                    fn_WriteLog("Page_Motion Click");
                    PgMotion.fn_SetTimer(true);
                    frame.Content = PgMotion;
                    menuMotion.Background = G_COLOR_BTNCLICKED;

                    //mc_MotionDlg->m_fn_SetPage(pnMotor1);
                    //mc_MotionDlg->m_fn_SetTimer(true);
                    //mc_MotionDlg->ShowWindow(SW_SHOW);

                    break;

                case EN_PAGE_NO.Setting:
                    fn_WriteLog("Page_Setting Click");
                    PgSettting.fn_SetTimer(true);
                    frame.Content = PgSettting;
                    menuSetting.Background = UserConst.G_COLOR_BTNCLICKED;

                    //                     LOG.m_fn_SetLog(_T("FORM_SETTING"));
                    //                     mc_SettingDlg->ShowWindow(SW_SHOW);
                    //                     mc_SettingDlg->PageShow(pnSetIO);

                    break;

                case EN_PAGE_NO.Recipe:
                    fn_WriteLog("Page_Recipe Click");
                    PgRecipe.fn_SetTimer(true);
                    frame.Content = PgRecipe;
                    menuRecipe.Background = UserConst.G_COLOR_BTNCLICKED;

                    //                     LOG.m_fn_SetLog(_T("FORM_RECIPE"));
                    //                     mc_RecipeDlg->m_fn_SetTimer(true);
                    //                     mc_RecipeDlg->m_fn_SetRecipeList(); //List Init
                    //                     mc_RecipeDlg->m_fn_LoadRecipe(mc_FileManager->m_fn_GetCurrRecipe()); //Display Recipe
                    //                     mc_RecipeDlg->ShowWindow(SW_SHOW);
                    break;

                case EN_PAGE_NO.Log:
                    fn_WriteLog("Page_Log Click");
                    frame.Content = PgLog;
                    menuLog.Background = UserConst.G_COLOR_BTNCLICKED;

                    break;

                case EN_PAGE_NO.Master:
                    fn_WriteLog("Page_Master Click");
                    //PgMaster.fn_SetTimer(true);
                    frame.Content = PgMaster;
                    PgMaster.fn_SetPage();
                    menuMaster.Background = UserConst.G_COLOR_BTNCLICKED;

                    break;

                default:
                    return;
            }

            //
            m_nPageSel = nPage;

        }
        //---------------------------------------------------------------------------
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            if(SEQ._bAuto || SEQ._bRun)
            {
                UserMsgBox.Warning("Can't exit while the machine is running or AUTO mode");
                return;
            }
            
            if (!UserMsgBox.Confirm("System Shut Down?")) return;

            
            //
            LOG.fn_Trace("----- [MAIN] PROGRAM END   -----  ");


            //JUNG/200417
            SEQ_SPIND.fn_MoveToolClamp(ccFwd);

            //Door Open Set
            SEQ.fn_SetDoorLock     (ccOpen);
            SEQ.fn_SetDoorLock_Side(ccOpen);
            
            //
            IO.fn_SetAutoKey(true);

            //
            fn_MainClose();

            //
            this.Close();

        }
        //---------------------------------------------------------------------------
        //Main Close 동작 
        private void fn_MainClose()
        {
            //각 Unit 별 Timer Off
            this      .fn_TimerSet(false);
            PgOper    .fn_SetTimer(false);
            PgMotion  .fn_SetTimer(false);
            PgSettting.fn_SetTimer(false);
            PgRecipe  .fn_SetTimer(false);
            PgMaster  .fn_SetTimer(false);

            Thread.Sleep(50);

            //End Thread
            THREAD.fn_StopThread();
            Thread.Sleep(100); //Delay

            //
            PMC.fn_Close();
            PMC.fn_StopThread();
            Thread.Sleep(100); //Delay

            //ACS Disconnection
            IO  .fn_ACSDisConnect();
            MOTR.fn_ACSDisConnect();

            Thread.Sleep(100);

            //Data Save
            DM .fn_LoadMap     (flSave);
            SEQ.fn_LoadWorkInfo(flSave);

            FM .fn_LoadLastInfo        (flSave);
            SEQ_SPIND.fn_LoadVisnResult(flSave);

            if(EPU._bNeedSave) EPU.fn_LoadErrorData(false); //JUNG/200910

            //SPC
            SPC.Load(flSave);
            
            //Load Cell
            LDCBTM.fn_Close();

            //Slurry
            SUPPLY[0].fn_Disconnect();
            SUPPLY[1].fn_Disconnect();


            //Form Close
            fn_SubFormClose();

            //Vision 
            g_VisionManager.fn_UnLoadVision();

        }
        //---------------------------------------------------------------------------
        private void FormMain_Loaded(object sender, RoutedEventArgs e)
        {
            //Thread Start
            THREAD.fn_StartThread();
            Thread.Sleep(100);

            fn_TimerSet(true);

            fn_PageShow((int)EN_PAGE_NO.Oper);

            tbVer.Text = FM._sVersion;
            PMC.fn_SetVersion(FM._sVersion);

            //fn_ChangeLevel(EN_USER_LEVEL.lvOperator);
#if DEBUG
            fn_ChangeLevel(EN_USER_LEVEL.lvMaster);
#else
            fn_ChangeLevel(EN_USER_LEVEL.lvOperator);
#endif

            //LOG
            LOG.fn_WriteLog($"---------- [MAIN] PROGRAM START ---------- [{FM._sVersion}]"); //JUNG/210126

            SEQ.fn_Reset();

            //All Servo On
            MOTR.SetServo(true);

            //Clear All Buffer
            IO.fn_StopAllBuffer();
        }
        //---------------------------------------------------------------------------
        public void fn_MainManuDisable()
        {
            //fn_ChangeLevel((EN_USER_LEVEL)FM._nCrntLevel);

            menuMotion .IsEnabled = false; menuMotion .Foreground = Brushes.LightGray;
            menuSetting.IsEnabled = false; menuSetting.Foreground = Brushes.LightGray;
            menuRecipe .IsEnabled = false; menuRecipe .Foreground = Brushes.LightGray;
            menuLog    .IsEnabled = false; menuLog    .Foreground = Brushes.LightGray;
            menuMaster .IsEnabled = false; menuMaster .Foreground = Brushes.LightGray;
            menuExit   .IsEnabled = false; menuExit   .Foreground = Brushes.LightGray;

        }
        //---------------------------------------------------------------------------
        public void fn_MainManuForeground()
        {
            //fn_ChangeLevel((EN_USER_LEVEL)FM._nCrntLevel);

            if (menuMotion .IsEnabled ) menuMotion .Foreground = Brushes.Black;
            if (menuSetting.IsEnabled ) menuSetting.Foreground = Brushes.Black;
            if (menuRecipe .IsEnabled ) menuRecipe .Foreground = Brushes.Black;
            if (menuLog    .IsEnabled ) menuLog    .Foreground = Brushes.Black;
            if (menuMaster .IsEnabled ) menuMaster .Foreground = Brushes.Black;
            if (menuExit   .IsEnabled ) menuExit   .Foreground = Brushes.Black;

        }
        //---------------------------------------------------------------------------
        public void fn_ChangeLevel(EN_USER_LEVEL level)
        {
            fn_MainManuDisable();

            if (level > EN_USER_LEVEL.lvEngineer)
            {
                menuMotion .IsEnabled = true; 
                menuSetting.IsEnabled = true;
                menuRecipe .IsEnabled = true;
                menuLog    .IsEnabled = true;
                menuMaster .IsEnabled = true;
                menuExit   .IsEnabled = true;
                
                PgOper.btLevel.Content = "USER\n[MASTER]";
                PgOper.btLevel.Background = Brushes.Yellow;


            }
            else if(level == EN_USER_LEVEL.lvEngineer)
            {
                menuMotion .IsEnabled = FM.m_stSystemOpt.stUserSet[1].bMotion ;
                menuSetting.IsEnabled = FM.m_stSystemOpt.stUserSet[1].bSetting;
                menuRecipe .IsEnabled = FM.m_stSystemOpt.stUserSet[1].bRecipe ;
                menuLog    .IsEnabled = FM.m_stSystemOpt.stUserSet[1].bLog    ;
                menuMaster .IsEnabled = false;                                
                menuExit   .IsEnabled = FM.m_stSystemOpt.stUserSet[1].bExit   ;

                PgOper.btLevel.Content = "USER\n[ENGINEER]";
                PgOper.btLevel.Background = Brushes.Tomato;

            }
            else if(level == EN_USER_LEVEL.lvOperator)
            {
                menuMotion .IsEnabled = FM.m_stSystemOpt.stUserSet[0].bMotion ;
                menuSetting.IsEnabled = FM.m_stSystemOpt.stUserSet[0].bSetting;
                menuRecipe .IsEnabled = FM.m_stSystemOpt.stUserSet[0].bRecipe ;
                menuLog    .IsEnabled = FM.m_stSystemOpt.stUserSet[0].bLog    ;
                menuMaster .IsEnabled = false;                                
                menuExit   .IsEnabled = FM.m_stSystemOpt.stUserSet[0].bExit   ;

                PgOper.btLevel.Content = "USER\n[OPERATOR]";
                PgOper.btLevel.Background = Brushes.White;

                FM.fn_DefaultSystemOpt();

            }

            fn_MainManuForeground();

            //Change Level
            FM._nCrntLevel = (int)level;

            tbCrntLevel.Text = FM.fn_GetLevelText();


        }
        //---------------------------------------------------------------------------
        public void fn_ShowPassword()
        {
            //JUNG/200617
            FormPass.pbPassWord.Password = "";
            FormPass.fn_SetBackground(FM._nCrntLevel);

            FormPass.ShowDialog();
        }
        //---------------------------------------------------------------------------
        private void fn_SubFormClose()
        {
            if (UserMsg   != null) UserMsg .Close();

            if (FormPass  != null) FormPass.Close();
            
            if (FormAlarm != null)
            {
                FormAlarm._bClose = true;
                FormAlarm.Close();
            }

            if (FormInfo     != null) FormInfo    .Close();
            if (FormMapStrg  != null) FormMapStrg .Close();
            if (FormMapMAGZ  != null) FormMapMAGZ .Close();
            if (FormMnlAlign != null) FormMnlAlign.Close();
            if (UserJog      != null) UserJog     .Close();  //JUNG/200602
        }
        //---------------------------------------------------------------------------
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //
#if DEBUG
            this.DragMove();
#endif
        }
        //---------------------------------------------------------------------------
        private void tbVer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //
            FormInfo.fn_ShowInfo();

        }
        //-------------------------------------------------------------------------------------------------
        public void fn_CameraReset()
        { 
            g_VisionManager._CamManager.CameraRetryCount = 0;
        }

        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
                e.Cancel = true;
        }

        private void tb_Mute_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tg = sender as ToggleButton;
            if (tg != null) LAMP.fn_Mute(tg.IsChecked == true ? true : false);
        }
    }
}
