using ACS.SPiiPlusNET;
using System;
using System.Collections.Generic;
using System.Data;
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
using WaferPolishingSystem.BaseUnit;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserEnum;
using WaferPolishingSystem.Define;

namespace WaferPolishingSystem.Form
{
    enum EN_MOTOR_STATUS : int
    {
        NONE          =0,
        POSITION_ERR    ,
        POSITION_R      ,
        POSITION_F      ,
        JERK            ,
        VELOCITY_F      ,
        ENABLED         ,
        ALARM           ,
        MOVING          ,
        HOME            ,
        LIMIT_LEFT      ,
        LIMIT_RIGHT     ,
        ACC             ,
        DEC             ,
        
    }

    enum EN_MOTOR_PARAMETER : int
    {
        NONE            =  1,
        VEL                 ,
        JERK                ,
        ACC                 ,
        DEC                 ,
        KDEC                ,
        SWPOT               ,
        SWNOT               ,
        SWPOTVAL            ,
        SWNOTVAL            ,
    }

    

    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageMaster_ACS : Page
    {
        //Var

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        private ACSControl      mc_Acs;
        private Api             AcsApi;

        private int  m_nComType;
        private int  m_nAxis;
        private bool m_bConnect = false;
        private const int maxlistcount = 11;

        private double m_dCmd;
        private double m_dEnc;
        private double m_dJerk;
        private double m_dVel;
        private double m_dAcc;
        private double m_dDec;
        //List<List<Ellipse>> listmotstatus = new List<List<Ellipse>>();
        //List<List<TextBox>> textmotstatus = new List<List<TextBox>>();

        DataTable motStatusdataTable = new DataTable();
        DataTable motParameterdataTable = new DataTable();

        public PageMaster_ACS()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            fn_Init();

        }
        //-------------------------------------------------------------------------------------------------
        private void fn_Init()
        {
            m_nAxis = 0;

            m_bConnect = false;

            m_dCmd  = 0;
            m_dEnc  = 0;
            m_dJerk = 0;
            m_dVel  = 0;
            m_dAcc  = 0;
            m_dDec  = 0;

            fn_InitMotStatusDataTable();

            fn_InitMotParameterDataTable();
            
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_InitMotStatusDataTable()
        {
            motStatusdataTable.Columns.Add("No");
            motStatusdataTable.Columns.Add("Name");
            motStatusdataTable.Columns.Add("Cmd");
            motStatusdataTable.Columns.Add("Enc");
            motStatusdataTable.Columns.Add("Jerk");
            motStatusdataTable.Columns.Add("Vel");
            motStatusdataTable.Columns.Add("Servo");
            motStatusdataTable.Columns.Add("Alarm");
            motStatusdataTable.Columns.Add("Moving");
            motStatusdataTable.Columns.Add("Home");
            motStatusdataTable.Columns.Add("POS");
            motStatusdataTable.Columns.Add("NEG");
            motStatusdataTable.Columns.Add("ACC");
            motStatusdataTable.Columns.Add("DEC");
            motStatusdataTable.Columns.Add("SBACK");
            motStatusdataTable.Columns.Add("ABACK");
            motStatusdataTable.Columns.Add("MBACK");
            motStatusdataTable.Columns.Add("HBACK");
            motStatusdataTable.Columns.Add("PBACK");
            motStatusdataTable.Columns.Add("NBACK");

            // Wet Cleaner

            //motStatusdataTable.Rows.Add("1", "Main_X", "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            //motStatusdataTable.Rows.Add("2", "Griper_Y", "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            //motStatusdataTable.Rows.Add("3", "Bath_1_R", "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            //motStatusdataTable.Rows.Add("4", "Bath_2_R", "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            //motStatusdataTable.Rows.Add("5", "Tool_R", "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            //motStatusdataTable.Rows.Add("6", "Tool_Z", "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            //motStatusdataTable.Rows.Add("7", "Griper_Z", "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");

            // Polishing 2.0

            motStatusdataTable.Rows.Add("1" , STRING_MOTOR_ID[ 0], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("2" , STRING_MOTOR_ID[ 1], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("3" , STRING_MOTOR_ID[ 2], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("4" , STRING_MOTOR_ID[ 3], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("5" , STRING_MOTOR_ID[ 4], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("6" , STRING_MOTOR_ID[ 5], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("7" , STRING_MOTOR_ID[ 6], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("8" , STRING_MOTOR_ID[ 7], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("9" , STRING_MOTOR_ID[ 8], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("10", STRING_MOTOR_ID[ 9], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");
            motStatusdataTable.Rows.Add("11", STRING_MOTOR_ID[10], "0", "0", "0", "0", "OFF", "OFF", "OFF", "OFF", "OFF", "OFF", "0", "0", "White");

            DataGrid_MotorStatus.ItemsSource = motStatusdataTable.DefaultView;
        }
        //---------------------------------------------------------------------------
        private void fn_InitMotParameterDataTable()
        {
            motParameterdataTable.Columns.Add("No");
            motParameterdataTable.Columns.Add("Name");
            motParameterdataTable.Columns.Add("Vel");
            motParameterdataTable.Columns.Add("Jerk");
            motParameterdataTable.Columns.Add("Acc");
            motParameterdataTable.Columns.Add("Dec");
            motParameterdataTable.Columns.Add("KDec");
            motParameterdataTable.Columns.Add("SWPOT");
            motParameterdataTable.Columns.Add("SWNOT");
            motParameterdataTable.Columns.Add("SWPOTValue");
            motParameterdataTable.Columns.Add("SWNOTValue");
            motParameterdataTable.Columns.Add("POTBACK");
            motParameterdataTable.Columns.Add("NOTBACK");


            // Wet Cleaner

            //motParameterdataTable.Rows.Add("1", "Main_X",       "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            //motParameterdataTable.Rows.Add("2", "Griper_Y",     "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            //motParameterdataTable.Rows.Add("3", "Bath_1_R",     "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            //motParameterdataTable.Rows.Add("4", "Bath_2_R",     "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            //motParameterdataTable.Rows.Add("5", "Tool_R",       "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            //motParameterdataTable.Rows.Add("6", "Tool_Z",       "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            //motParameterdataTable.Rows.Add("7", "Griper_Z",     "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");

            // Polishing 2.0
            
            motParameterdataTable.Rows.Add("1" , STRING_MOTOR_ID[ 0], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("2" , STRING_MOTOR_ID[ 1], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("3" , STRING_MOTOR_ID[ 2], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("4" , STRING_MOTOR_ID[ 3], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("5" , STRING_MOTOR_ID[ 4], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("6" , STRING_MOTOR_ID[ 5], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("7" , STRING_MOTOR_ID[ 6], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("8" , STRING_MOTOR_ID[ 7], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("9" , STRING_MOTOR_ID[ 8], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("10", STRING_MOTOR_ID[ 9], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");
            motParameterdataTable.Rows.Add("11", STRING_MOTOR_ID[10], "0", "0", "0", "0", "0", "OFF", "OFF", "0", "0", "White");


            DataGrid_Parameter.ItemsSource = motParameterdataTable.DefaultView;
        }
        private void fn_Destroy()
        {
            m_UpdateTimer.Stop();
        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            if (m_bConnect)
            {
                Rectangle_Status.Fill = Brushes.Green;
                fn_UpdateMotorStatus();
                fn_UpdateMotorParameter();
            }
            else
            {
                Rectangle_Status.Fill = Brushes.Red;
            }
                
            //
            m_UpdateTimer.Start();
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_UpdateMotorStatus()
        {
            
            for (int j = 0; j < motStatusdataTable.Rows.Count; j++)
            {
                //
                if (MOTR.fn_IsSMCMotor((EN_MOTR_ID)j))
                {

                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.ENABLED] = MOTR[j].GetServo() ? "ON" : "OFF";
                    motStatusdataTable.Rows[j]["SBACK"] = MOTR[j].GetServo() ? "Green" : "LightGray";

                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.HOME] = MOTR[j].GetHomeEnd() ? "ON" : "OFF";
                    motStatusdataTable.Rows[j]["HBACK"] = MOTR[j].GetHomeEnd() ? "Green" : "LightGray";

                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.ALARM] = MOTR[j].GetAlarm() ? "ON" : "OFF";
                    motStatusdataTable.Rows[j]["ABACK"] = MOTR[j].GetAlarm() ? "Red" : "LightGray";

                    continue;
                }

                if (mc_Acs.fn_GetStateEnabled(j))
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.ENABLED] = "ON";
                    motStatusdataTable.Rows[j]["SBACK"] = "Green";
                }
                else
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.ENABLED] = "OFF";
                    motStatusdataTable.Rows[j]["SBACK"] = "LightGray";
                }

                if (mc_Acs.fn_GetStateAlarmAll(j))
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.ALARM] = "ON";
                    motStatusdataTable.Rows[j]["ABACK"] = "Red";
                }
                else
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.ALARM] = "OFF";
                    motStatusdataTable.Rows[j]["ABACK"] = "LightGray";
                }

                if (mc_Acs.fn_GetStateMove(j))
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.MOVING] = "ON";
                    motStatusdataTable.Rows[j]["MBACK"] = "Green";
                }
                else
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.MOVING] = "OFF";
                    motStatusdataTable.Rows[j]["MBACK"] = "LightGray";
                }

                //
                motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.HOME] = MOTR[j].GetHomeEnd() ? "ON" : "OFF";
                motStatusdataTable.Rows[j]["HBACK"] = MOTR[j].GetHomeEnd()? "Green" : "LightGray";

                if (mc_Acs.fn_GetStateHardwareLeftLimit(j))
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.LIMIT_LEFT] = "ON";
                    motStatusdataTable.Rows[j]["PBACK"] = "Red";
                }
                else
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.LIMIT_LEFT] = "OFF";
                    motStatusdataTable.Rows[j]["PBACK"] = "LightGray";
                }

                if (mc_Acs.fn_GetStateHardwareRightLimit(j))
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.LIMIT_RIGHT] = "ON";
                    motStatusdataTable.Rows[j]["NBACK"] = "Red";
                }
                else
                {
                    motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.LIMIT_RIGHT] = "OFF";
                    motStatusdataTable.Rows[j]["NBACK"] = "LightGray";
                }

                motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.POSITION_R] = Math.Round(mc_Acs.fn_GetTargetPosition       (j), 3);
                motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.POSITION_F] = Math.Round(mc_Acs.fn_GetEncoderPosition      (j), 3);
                motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.JERK      ] = Math.Round(mc_Acs.fn_GetParameterJerk        (j), 3);
                motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.VELOCITY_F] = Math.Round(mc_Acs.fn_GetParameterVelocity    (j), 3);
                motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.ACC       ] = Math.Round(mc_Acs.fn_GetParameterAcceleration(j), 3);
                motStatusdataTable.Rows[j][(int)EN_MOTOR_STATUS.DEC       ] = Math.Round(mc_Acs.fn_GetParameterAcceleration(j), 3);
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_UpdateMotorParameter()
        {
            for (int i = 0; i < motParameterdataTable.Rows.Count; i++)
            {
                //
                if (MOTR.fn_IsSMCMotor((EN_MOTR_ID)i)) continue;

                motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.VEL ] = Math.Round(mc_Acs.fn_GetParameterVelocity        (i), 3);
                motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.JERK] = Math.Round(mc_Acs.fn_GetParameterJerk            (i), 3);
                motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.ACC ] = Math.Round(mc_Acs.fn_GetParameterAcceleration    (i), 3);
                motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.DEC ] = Math.Round(mc_Acs.fn_GetParameterDeceleration    (i), 3);
                motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.KDEC] = Math.Round(mc_Acs.fn_GetParameterKillDeceleration(i), 3);

                if (mc_Acs.fn_GetStateSoftwareRightLimit(i))
                {
                    motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.SWPOT] = "ON";
                    motParameterdataTable.Rows[i]["POTBACK"] = "Red";
                }
                else
                {
                    motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.SWPOT] = "OFF";
                    motParameterdataTable.Rows[i]["POTBACK"] = "LightGray";
                }

                if (mc_Acs.fn_GetStateSoftwareLeftLimit(i))
                {
                    motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.SWNOT] = "ON";
                    motParameterdataTable.Rows[i]["NOTBACK"] = "Red";
                }
                else
                {
                    motParameterdataTable.Rows[i][(int)EN_MOTOR_PARAMETER.SWNOT] = "OFF";
                    motParameterdataTable.Rows[i]["NOTBACK"] = "LightGray";
                }


            }
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
        //-------------------------------------------------------------------------------------------------
        private void Radio_TCP_Click(object sender, RoutedEventArgs e)
        {
            if (Radio_TCP.IsChecked.Value)
                m_nComType = 0;
            else if (Radio_SIM.IsChecked.Value)
                m_nComType = 1;
            else
                m_nComType = -1;
        }
        //-------------------------------------------------------------------------------------------------
        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            string strIp   = Text_IP  .Text.ToString();
            string strPort = Text_PORT.Text.ToString();
            int    nPort   = 0;

            if (m_bConnect    ) return;

            //AcsApi = new Api();
            if(AcsApi == null)
            {
                AcsApi = new Api();
                mc_Acs = new ACSControl(AcsApi);
            }

            m_bConnect = false;

            if (mc_Acs != null) mc_Acs.fn_CloseACS(); //Close

            if (m_nComType == 0)
            {
                if (strIp   == null || strIp == ""  ) return;
                if (strPort == null || strPort == "") return;
                nPort = Convert.ToInt32(strPort);
                m_bConnect = mc_Acs.fn_OnConnectTCP(strIp, nPort);
            }
            else if (m_nComType == 1)
            {
                m_bConnect = mc_Acs.fn_OnConnectSimulator();
            }
            else
            {
                m_bConnect = false;
            }
            
            //
            if (!m_bConnect)
            {
                MessageBox.Show("ACS MOTOR에 연결 할 수 없습니다.!!!!", "[ERROR]");
            }
                
        }
        //-------------------------------------------------------------------------------------------------
        
        private void Button_DisConnect_Click(object sender, RoutedEventArgs e)
        {
            if (m_nComType == 0)
            {
                m_bConnect = false;
                mc_Acs.fn_CloseACS();
            }
            else
            {
                m_bConnect = false;
                mc_Acs.fn_CloseSimulatorACS();
            }
                
        }

        private void Button_Enable_Click(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_SetEnabled(m_nAxis);
        }

        private void Button_Disable_Click(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_SetDisabled(m_nAxis);
        }

        private void Button_DisableAll_Click(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_SetDisableAll();
        }
        //-------------------------------------------------------------------------------------------------
        private void Button_Jog_Positive_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_MoveJog(m_nAxis, true, 100);
        }

        private void Button_Jog_Positive_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_StopJog(m_nAxis);
        }

        private void Button_Jog_Negative_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_MoveJog(m_nAxis, false, 100);
        }

        private void Button_Jog_Negative_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_StopJog(m_nAxis);
        }
        //---------------------------------------------------------------------------
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;
            
            if (MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nAxis))
            {
                MOTR.SetServo((EN_MOTR_ID)m_nAxis, true);
            }
            else
            {
                mc_Acs.fn_SetEnabled(m_nAxis);
            }
        }
        //---------------------------------------------------------------------------
        private void UserButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            if (MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nAxis))
            {
                MOTR.SetServo((EN_MOTR_ID)m_nAxis, false);
            }
            else
            {
                mc_Acs.fn_SetDisabled(m_nAxis);
            }
            
        }
        //---------------------------------------------------------------------------
        private void DataGrid_MotorStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            m_nAxis = DataGrid_MotorStatus.SelectedIndex;

            //
            if (MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nAxis)) return; 

            m_dCmd = Math.Round(mc_Acs.fn_GetTargetPosition       (m_nAxis), 3);
            m_dEnc = Math.Round(mc_Acs.fn_GetEncoderPosition      (m_nAxis), 3);
            m_dJerk= Math.Round(mc_Acs.fn_GetParameterJerk        (m_nAxis), 3);
            m_dVel = Math.Round(mc_Acs.fn_GetParameterVelocity    (m_nAxis), 3);
            m_dAcc = Math.Round(mc_Acs.fn_GetParameterAcceleration(m_nAxis), 3);
            m_dDec = Math.Round(mc_Acs.fn_GetParameterAcceleration(m_nAxis), 3);

            TextBox_Cmd.Text    = Convert.ToString(m_dCmd   );
            TextBox_Enc.Text    = Convert.ToString(m_dEnc   );
            TextBox_Jerk.Text   = Convert.ToString(m_dJerk  );
            TextBox_Vel.Text    = Convert.ToString(m_dVel   );
            TextBox_Acc.Text    = Convert.ToString(m_dAcc   );
            TextBox_Dec.Text    = Convert.ToString(m_dDec   );
        }
        //---------------------------------------------------------------------------
        private void UserButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            m_dVel = Convert.ToDouble(TextBox_Vel.Text);

            mc_Acs.fn_MoveJog(m_nAxis, true, m_dVel);
        }

        private void UserButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_StopNor(m_nAxis);
        }

        private void UserButton_Click_2(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_StopNor(m_nAxis);
        }

        private void UserButton_Click_3(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_StopEMG(m_nAxis);
        }

        private void UserButton_Click_4(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            for (int i = 0; i < motStatusdataTable.Rows.Count; i++)
            {
                if (MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nAxis)) continue;

                mc_Acs.fn_SetEnabled(i);
            }

            MOTR.fn_SetEnableSMC();
        }
        //---------------------------------------------------------------------------
        private void UserButton_Click_5(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            //All Servo Off
            mc_Acs.fn_SetDisableAll();

            //
            MOTR.fn_SetDisableSMC();

        }
        //---------------------------------------------------------------------------
        private void UserButton_Click_6(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            //mc_Acs.fn_SetBuffer(m_nAxis, true); //HOME

            int nHomeNo = MOTR.ManNoHome((EN_MOTR_ID)m_nAxis);

            if (!fn_UserMsg($"{STRING_MOTOR_ID[m_nAxis]} Motor Home?", EN_MSG_TYPE.Check)) return;

            SEQ.fn_Reset();

            if(nHomeNo == 32) //Z-Axis
            {
                if(SEQ_SPIND.fn_IsExistTool() && !FM.fn_IsLvlMaster())
                {
                    fn_UserMsg("Please remove tool");
                    return ;
                }
            }
            MAN.fn_ManProcOn(nHomeNo, true, false);

        }
        //---------------------------------------------------------------------------
        private void UserButton_Click_7(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_SetBuffer(m_nAxis, false); //HOME STOP
        }
        //---------------------------------------------------------------------------
        private void UserButton_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            m_dVel = Convert.ToDouble(TextBox_Vel.Text);

            mc_Acs.fn_MoveJog(m_nAxis, false, m_dVel);
        }

        private void UserButton_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_StopNor(m_nAxis);
        }

        private void UserButton_Click_8(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0 || mc_Acs.fn_GetStateMove(m_nAxis))
                return;

            m_dCmd = Convert.ToDouble(TextBox_Cmd.Text);
            m_dVel = Convert.ToDouble(TextBox_Vel.Text);
            m_dAcc = Convert.ToDouble(TextBox_Acc.Text);
            m_dDec = Convert.ToDouble(TextBox_Dec.Text);

            mc_Acs.fn_MoveAbsolutePosition(m_nAxis, m_dCmd, m_dVel, m_dAcc, m_dDec);
        }

        private void UserButton_Click_9(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_SetFPos(m_nAxis, 0);
        }
        private void UserButton_Click_10(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0) return;

            mc_Acs.fn_SetFaultClear (m_nAxis);
        }

        private void UserButton_Click_11(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0 || mc_Acs.fn_GetStateMove(m_nAxis))
                return;

            m_dCmd = Convert.ToDouble(TextBox_Cmd.Text);
            m_dVel = Convert.ToDouble(TextBox_Vel.Text);
            m_dAcc = Convert.ToDouble(TextBox_Acc.Text);
            m_dDec = Convert.ToDouble(TextBox_Dec.Text);

            mc_Acs.fn_MoveRelativePosition(m_nAxis, m_dCmd, false, m_dVel, m_dAcc, m_dDec);
        }

        private void UserButton_Click_12(object sender, RoutedEventArgs e)
        {
            if (!m_bConnect || m_nAxis < 0 || mc_Acs.fn_GetStateMove(m_nAxis))
                return;

            m_dCmd = Convert.ToDouble(TextBox_Cmd.Text);
            m_dVel = Convert.ToDouble(TextBox_Vel.Text);
            m_dAcc = Convert.ToDouble(TextBox_Acc.Text);
            m_dDec = Convert.ToDouble(TextBox_Dec.Text);

            mc_Acs.fn_MoveRelativePosition(m_nAxis, m_dCmd, true, m_dVel, m_dAcc, m_dDec);
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.IsEnabled = true;
            m_UpdateTimer.Start();

            m_bConnect = MOTR._bConnect;

            //
            if (AcsApi == null)
            {
                AcsApi = MOTR.fn_GetAPI();
                mc_Acs = new ACSControl(AcsApi);
            }
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.Stop();
        }
        //---------------------------------------------------------------------------
        private void TextBox_Cmd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = fn_IsNumeric(e.Text);
        }
    }
}
