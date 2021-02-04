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
using System.Windows.Shapes;
using System.Windows.Threading;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserClass;

namespace WaferPolishingSystem
{
    /// <summary>
    /// FormLoad.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormLoad : Window
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Var
        int    m_nStep;
        //string m_sPath, m_sJobName;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        private TOnDelayTimer   m_tDelayTimer = new TOnDelayTimer  ();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public FormLoad()
        {
            InitializeComponent();

            this.Background = UserConst.G_COLOR_FORMBACK;

            m_nStep = 0;

            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(300);
            m_UpdateTimer.Tick    += new EventHandler(fn_tmUpdate);

            m_tDelayTimer.Clear();

                     

        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            
            //m_UpdateTimer.Stop();

            //
            switch (m_nStep)
            {
                case 0:
                    
                    tbLoadMsg.Text = " Now Loading System Data...";

                    if (!m_tDelayTimer.OnDelay(true, 300)) return; 
                    m_tDelayTimer.Clear();
                    m_nStep++;
                    break;

                case 1:
                    //Load Default file Info.
                    tbLoadMsg.Text = " Loading the Information..."      ; FM.fn_LoadLastInfo   (true                  );
                    tbLoadMsg.Text = " Loading the Project Info..."     ; FM.fn_LoadProject    (true, FM._sCurrJob    );
                    tbLoadMsg.Text = " Loading the System Option..."    ; FM.fn_LoadSysOptn    (true                  );
                    tbLoadMsg.Text = " Loading the Master Option..."    ; FM.fn_LoadMastOptn   (true                  );
                    tbLoadMsg.Text = " Loading the Recipe Data..."      ; FM.fn_LoadRecipeInfo (true, FM._sRecipeName );
                    tbLoadMsg.Text = " Loading the Password..."         ; FM.fn_LoadPassWord   (true                  );
                                                                        
                    tbLoadMsg.Text = " Loading SPC Data..."             ; SPC.Load             (true);
                    
                    m_tDelayTimer.Clear();
                    
                    m_nStep++;
                    break;

                case 2:
                    if (!m_tDelayTimer.OnDelay(true, 100)) return;
                    tbLoadMsg.Text = " Loading the IO List..."          ; IO  .fn_LoadIO(true);
                    tbLoadMsg.Text = " Loading the Lamp/Buzzer Data..." ; LAMP.fn_Load  (true);
                    tbLoadMsg.Text = " Loading the Actuator Data..."    ; ACTR.fn_Load  (true);

                    //tbLoadMsg.Text = " ACS IO Connection...(SIM)"        ; IO.fn_ACSConnect(UserConst.ACS_CON_SIM);
                    tbLoadMsg.Text = " ACS IO Connection..."             ; IO.fn_ACSConnect(UserConst.ACS_CON_NOR);

                    m_tDelayTimer.Clear();

                    m_nStep++;
                    break;

                case 3:
                    //if (!m_tDelayTimer.OnDelay(true, 100)) return;

                    tbLoadMsg.Text = " Loading the Error List..."       ; EPU.fn_LoadErrorData(true);
                    tbLoadMsg.Text = " Loading the Lamp/Buzzer Data..." ; EPU.fn_LoadLampData (true);

                    m_tDelayTimer.Clear();

                    m_nStep++;
                    break; 


                case 4:
                    if (!m_tDelayTimer.OnDelay(true, 500)) return;
                    //tbLoadMsg.Text = " Loading the Motor List...(SIM)"; MOTR.fn_InitMotor(UserConst.ACS_CON_SIM);
                    tbLoadMsg.Text = " Loading the Motor List..."       ; MOTR.fn_InitMotor(UserConst.ACS_CON_NOR);

                    tbLoadMsg.Text = " Loading the Vision Recipe..."    ; g_VisionManager.fn_LoadVision(FM._sRecipeName);


                    //tbLoadMsg.Text = " Loading the Motor Data..."       ; FM.fn_LoadMastOptn(true);
                    //tbLoadMsg.Text = " Loading the Motor DSTB..."       ; MOTR.fn_LoadMotrDisturb(true);

                    m_tDelayTimer.Clear();

                    m_nStep++;
                    break;


                case 5:
                    //if (!m_tDelayTimer.OnDelay(true, 500)) return;
                    tbLoadMsg.Text = " Loading the Data Manager Map Data..." ; DM .fn_LoadMap     (true);
                    tbLoadMsg.Text = " Loading the Sequence Data..."         ; SEQ.fn_LoadWorkInfo(true);

                    m_tDelayTimer.Clear();

                    m_nStep++;
                    break;

                case 6:

                    //Row, Col Setting...
                    tbLoadMsg.Text = " Apply Project..."                     ; FM.ApplyProject(FM._sCurrJob);

                    m_nStep++;
                    break;

                case 7:
                    //LoadCell
                    tbLoadMsg.Text = " Initial Load Cell ..."                ; 
                    LDCBTM.fn_SetParam();
                    LDCBTM.fn_Open    (LDCBTM._sSerialNo); //LDCBTM.fn_Open    (FM.m_stMasterOpt.sLoadCellSN);
                    
                    LDCBTM.fn_Close   (); //JUNG/201014/Retry
                    LDCBTM.fn_Open    (LDCBTM._sSerialNo);

                    //PMC, Slurry Init
                    //tbLoadMsg.Text = " Initial PMC IP,Port ..."              ; PMC.fn_SetIpAddress(FM.m_stSystemOpt.sPMCIp, FM.m_stSystemOpt.nPMCPort);
                    tbLoadMsg.Text = " Initial REST API ..."                  ; REST.fn_SetURL(FM.m_stSystemOpt.sRestApiUrl);

                    tbLoadMsg.Text = " Initial RFID Reader ...";
                    m_nStep++;
                    break;

                case 8:
                    if (IO._bConnect)
                    {
                        //RFID
                        tbLoadMsg.Text = " Initial RFID Reader ..."; RFID.fn_Connect();
                    }
                    
                    m_nStep++;
                    break;

                case 9:
                    //if (!m_tDelayTimer.OnDelay(true, 500)) return;

                    //Main Load
                    fn_MainFormLoad();

                    m_UpdateTimer.Stop();

                    m_nStep++;
                    break;


                default:
                    break;
            }

            //Step Inc.
            pbStep.Value = m_nStep * 10;

            //m_UpdateTimer.Start();


        }
        //---------------------------------------------------------------------------
        private void fn_MainFormLoad()
        {
//             FormMain mainform = new FormMain();
//             mainform.Show();

            this.Close();

        }
        //---------------------------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
            tbVersion.Text = FormMain.FM._sVersion;

            m_UpdateTimer.Start();
            m_tDelayTimer.Clear();

        }
        //---------------------------------------------------------------------------
        private void Window_Closed(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

        }
    }
}
