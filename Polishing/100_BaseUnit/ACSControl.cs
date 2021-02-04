using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACS.SPiiPlusNET;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;


namespace WaferPolishingSystem.BaseUnit
{
    public class ACSControl
    {

        private Api _ACS; // = new Api();

        #region Variable

        // 연결 상태 flag
        private bool               m_bConnect = false;
        
        private MotorStates        m_nMotorState; //Motor State Variable
        private SafetyControlMasks m_nMotorAlarm; //Safety Control Variable
        private ProgramStates      m_nProgState ; //Program State Variable
        
        // return value
        private bool   m_bRet = false;
        private string m_sErrMsg;

        private bool m_bSynch;

        private Axis[] m_arrAxisList = null;

        //private int m_nMotor;

        //Object   objVar     = null;
        //Array    InputArray = null;

        //Property
        public bool   _bConnect  { get { return m_bConnect; } }
        public string _sErrMsg   { get { return m_sErrMsg ; } }

        
        
        //Timer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        TOnDelayTimer m_tWaitTimer   = new TOnDelayTimer();
        TOnDelayTimer m_tServoWait   = new TOnDelayTimer();
        TOnDelayTimer m_tRingCounter = new TOnDelayTimer();
        TOnDelayTimer m_tAlarmReset  = new TOnDelayTimer();
        TOnDelayTimer m_tHomeEnd     = new TOnDelayTimer();
        
        //Vars.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //private:   /* Member Var.             */
        int  m_iHomeType      ;
        //int  m_iHomeSignal    ;
        //int  m_iHomeZPhase    ;
        //int  m_iGroupAxeNo    ;
        //uint m_uModuleID      ;
        bool m_bReqServoOn    ;
        bool m_bReqResetAlarm ;
        //bool m_bSetUseTorque  ;
        int  m_lTotalAxis     ; 
        //int  m_iHomeDly       ;
        int  m_nAxis          ;
        bool m_bConnectAsSim  ;


        public string m_sParamPath     ;
        public int    m_iMotorType     ;
        public int    m_iMotorKind     ;
        public double m_dCoef          ;
        public int    m_iSONLevel      ;
        public int    m_iMotrKind      ;
        public string m_sToqWAddr      ;
        public string m_sToqRAddr      ;

        //Vars. - Home
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int    m_iSetHomeLevel ;
        public bool   m_bKeepHomeProc ;
        public bool   m_bForceHome    ;
        public double m_dHomeVel      ;
        public double m_dHomeAcc      ;
        public double m_dHomeDec      ;
        public double m_dHomeOffset   ;
        public double m_dHomeOffsetPos;


        //Var. - Update
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool   m_bServo         ;
        public bool   m_bHome          ;
        public bool   m_bStop          ;
        public bool   m_bReady         ;
        public bool   m_bBusy          ;
        public bool   m_bHomeEnd       ;
        public bool   m_bPackInPosn    ;
        public bool   m_bAlarm         ;
        public bool   m_bCW            ;
        public bool   m_bCCW           ;
        public bool   m_bSRL           ; //Right Soft Limit
        public bool   m_bSLL           ; //Right Soft Limit

        public bool   m_bHomeEndDone   ;
        
        public bool   m_bLtBusy        ;
        public bool   m_bRing          ;
        public bool   m_bLtHomeSen     ;
        public bool   m_bReqRingCounter;
        public double m_dTorque        ;
        public double m_dPreTrgPos     ;
        public double m_dTrgPos        ;
        public double m_dAbsOffset     ;
        public double m_dReqRingMaxPos ;
        public int    m_iStepHome      ;
        public bool   m_bApplyScurve   ;
        public int    m_iHomeDir       ;

        public double m_dCmdPos        ;
        public double m_dEncPos        ;

        public double m_dOrgCmdPos     ;
        public double m_dOrgEncPos     ;


        //-------------------------------------------------------------------
        //get/Set Functions.
		public bool  	  gServo              () {return m_bServo;      }
		public bool  	  gHome               () {return m_bHome;       }
		public bool  	  gStop               () {return m_bStop;       }
		public bool  	  gReady              () {return m_bReady;      }
		public bool  	  gBusy               () {return m_bBusy;       }
		public bool  	  gHomeEnd            () {return m_bHomeEnd;    }
        public bool  	  gHomeEndDone        () {return m_bHomeEndDone;}
        
        public bool  	  gPackInPosn         () {return m_bPackInPosn; }
		public bool  	  gAlarm              () {return m_bAlarm;      }
		public bool  	  gCW                 () {return m_bCW;         }
		public bool  	  gCCW                () {return m_bCCW;        }
		public bool  	  gLtBusy             () {return m_bLtBusy;     }
		public bool  	  gRing               () {return m_bRing;       }
		public double	  gTorque             () {return m_dTorque;     }
		public double	  gPreTrgPos          () {return m_dPreTrgPos;  }
		public double	  gTrgPos             () {return m_dTrgPos;     }
		public int	      gHomeStep           () {return m_iStepHome;   }

        public void       sPreTrgPos          (double bSet) { m_dPreTrgPos = bSet; }
        public void       sTrgPos             (double bSet) { m_dTrgPos    = bSet; }
        public void       sHomeEnd            (bool   bSet) { m_bHomeEnd   = bSet; }
        public void       sHomeEndDone        (bool   bSet) { m_bHomeEndDone = bSet; }
		public bool  	  gSRL                 () {return m_bSRL;         }
		public bool  	  gSLL                 () {return m_bSLL;         }
        


        #endregion

        public ACSControl(Api acsapi)
        {
            //
            m_bSynch = true; //Synchronous
            //m_nMotor = 0;

            _ACS = acsapi;


        }
        //---------------------------------------------------------------------------
        public void Init (int nAxis)
        {
            m_iStepHome     = 0;
            m_bKeepHomeProc = false;
            m_dHomeVel      = 0.0;
            m_dHomeAcc      = 0.0;
            m_dHomeDec      = 0.0;
            m_dHomeOffset   = 0.0;
            m_dAbsOffset    = 0.0;
            m_bReqServoOn   = false;
            m_lTotalAxis    = 0;
            m_bConnectAsSim = false;

            m_nAxis         = nAxis; 

        }
        //---------------------------------------------------------------------------
        public bool fn_Connect(int type)
        {
            bool brtn = false; 
            
            string sIp  = "10.0.0.100";
            int    port = (int)EthernetCommOption.ACSC_SOCKET_STREAM_PORT;


            //Connection
            if (type == UserConst.ACS_CON_NOR)
            {
                brtn = fn_OnConnectEthernet(sIp, port);
            }
            else
            {
                brtn = fn_OnConnectSimulator();
                m_bConnectAsSim = brtn;
            }


            return brtn; 

        }

        //---------------------------------------------------------------------------

        #region Communication - Open / Close
        /**    
        <summary>
            Open Ethernet
        </summary>
        <param name="strIp">IP 주소</param>
        <param name="nPort">Port 넘버</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 14:31
        */
        public bool fn_OnConnectEthernet(string strIp, int nPort)
        {
            m_bConnect = false;

            try
            {
                _ACS.OpenCommEthernet(strIp, nPort);
                
                m_bConnect = true;
                return m_bConnect;
            }
            catch (Exception ex)
            {
                m_sErrMsg = "[ACS] MOTOR Open Fail!!! --> " + ex.Message;

                MessageBox.Show("ACS MOTOR OPEN FAIL!!! \n" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                m_bConnect = false;
                return m_bConnect;

            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Open TCP/IP
        </summary>
        <param name="strIp">IP 주소</param>
        <param name="nPort">Port 넘버</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:02
        */
        public bool fn_OnConnectTCP(string strIp, int nPort)
        {
            try
            {
                _ACS.OpenCommEthernetTCP(strIp, nPort);
                m_bConnect = true;
                return m_bConnect;
            }
            catch (Exception ex)
            {
                Console.WriteLine("fn_OnConnectTCP :" + ex.Message);
                m_bConnect = false;
                return m_bConnect;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Open UDP
        </summary>
        <param name="strIp">IP 주소</param>
        <param name="nPort">Port 넘버</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:03
        */
        public bool fn_OnConnectUDP(string strIp, int nPort)
        {
            try
            {
                _ACS.OpenCommEthernetUDP(strIp, nPort);
                m_bConnect = true;
                return m_bConnect;
            }
            catch (Exception ex)
            {
                Console.WriteLine("fn_OnConnectUDP :" + ex.Message);
                m_bConnect = false;
                return m_bConnect;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Open Serial
        </summary>
        <param name="nChannel">COM Number</param>
        <param name="nBaudrate">Baud Rate</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:04
        */
        public bool fn_OnConnectSerial(int nChannel, int nBaudrate)
        {
            try
            {
                _ACS.OpenCommSerial(nChannel, nBaudrate);
                m_bConnect = true;
                return m_bConnect;
            }
            catch (Exception ex)
            {
                Console.WriteLine("fn_OnConnectSerial :" + ex.Message);

                m_bConnect = false;
                return m_bConnect;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Open Simulator
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:07
        */
        public bool fn_OnConnectSimulator()
        {
            m_bConnect = false;

            try
            {
                _ACS.OpenCommSimulator();
                m_bConnect = true;
                return m_bConnect;
            }
            catch (Exception ex)
            {
                MessageBox.Show("[SIM] ACS MOTOR OPEN FAIL!!! \n" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                m_bConnect = false;
                return m_bConnect;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Close Controller
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 16:44
        */
        public bool fn_CloseACS()
        {
            if (m_bConnect)
            {
                _ACS.CloseComm();
                m_bConnect = false;
                return true;
            }
            else
            {
                return true;
            }
        }
        public bool fn_DisableAllAsync()
        {
            if (m_bConnect)
            {
                //Shutdown all motors
                ACSC_WAITBLOCK wait = _ACS.DisableAllAsync();

                _ACS.CancelOperation(wait);
                
                return true;
            }
            else
            {
                return true;
            }
        }
        //-------------------------------------------------------------------------------------------------
        public void  fn_Reset ()
        {
            //Reset homing flag.
            if (m_bKeepHomeProc)
            {
                m_bKeepHomeProc = false;
                m_iStepHome     = 0;
            }

        }

        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Close Simulator
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 16:44
        */
        public bool fn_CloseSimulatorACS()
        {
            if (m_bConnect)
            {
                _ACS.CloseSimulator();
                m_bConnect = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Connect Status
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 10:30
        */
        public bool fn_IsConnect()
        {
            return m_bConnect;
        }
        //---------------------------------------------------------------------------
        public bool fn_Open(int nAxis)
        {

            if (m_lTotalAxis > 0) return true;

            //Initialize ACS

            // Get Total number of axes
            // Using Transaction function : return string text from controller, we need to convert to integer value
            //string sTemp      = _ACS.Transaction("?SYSINFO(13)");
            //int    nTotalAxis = Convert.ToInt32(sTemp.Trim());
            int nTotalAxis = (int)_ACS.GetAxesCount();

            //return fn_SetEnabled(nAxis);
            return true; 



// 
//             if (nTotalAxis > 0)
//             {
//                 m_arrAxisList = new Axis[nTotalAxis + 1]; //Total Axis List
//                 m_arrAxisList[nTotalAxis] = Axis.ACSC_NONE;
// 
//                 // If you want to enable several axes, 
//                 // Ex) Enable three axes (0, 1, 6)
//                 // int[] AxisList = new int[] { 0, 1, 6, -1 };      !!!! Important !! Must insert '-1' at the last
//                 // _ACS.EnableM(AxisList);
// 
//                 //All Motor Enable
//                 if (m_arrAxisList != null) _ACS.EnableM(m_arrAxisList);
// 
//                 m_lTotalAxis = nTotalAxis;
//             }
// 
//             //
//             return (m_lTotalAxis > 0);

        }
        //---------------------------------------------------------------------------
        public bool fn_OpenSMC(int nAxis)
        {
            if (nAxis < (int)EN_MOTR_ID.miSPD_Z1) return false;

            return fn_SetEnabledSMC(nAxis);

        }
        //---------------------------------------------------------------------------
        public void fn_ClearHomeEnd()
        {
            m_bKeepHomeProc = false;
            m_iStepHome     = 0;
            m_bHomeEnd      = false; //

            //
            m_bHomeEndDone  = false;

        }
        //-------------------------------------------------------------------------------------------------
        public void fn_ClearHomeEndSMC()
        {
            m_bKeepHomeProc = false;
            m_iStepHome     = 0;
            m_bHomeEnd      = false;
            
            //
            m_bHomeEndDone = false;

        }

        //-------------------------------------------------------------------------------------------------
        public void fn_SetServo(int nAxis, int iOn)
        {
            //Servo On/Off.
            if (iOn != 1)
            {
                if (!m_bServo) return;
                
                fn_StopEMG    (nAxis);
                fn_SetDisabled(nAxis);
                return;
            }

            if (m_bServo) return;
            
            m_bReqServoOn = true;
            m_tServoWait.Clear();
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetServoSMC(int nAxis, int iOn)
        {
            //Servo On/Off.
            if (iOn != 1)
            {
                if (!m_bServo) return;
                //
                fn_SetDisabledSMC(nAxis);
                return;
            }

            if (m_bServo) return;
            
            m_bReqServoOn = true;
            m_tServoWait.Clear();
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetAlarm(int nAxis, int On)
        {
            //Alarm Reset
            fn_SetFaultClear(nAxis);

            m_bReqResetAlarm = true;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetAlarmSMC(int nAxis, int On)
        {
            //Alarm Reset
            fn_SetFaultClearSMC(nAxis);

            m_bReqResetAlarm = true;
        }

        //-------------------------------------------------------------------------------------------------
        #endregion

        #region Motion Enabled / Disabled

        public bool fn_SetServo(int nAxis, bool on)
        {
            m_bRet = false;

            try
            {
                if (on) fn_SetEnabled (nAxis);
                else    fn_SetDisabled(nAxis);

                return true;
            }
            catch (ACSException ex)
            {
                //System.Console.WriteLine(ex.Message);
                Console.WriteLine("fn_SetServo : " + ex.Message);
                return false;
            }
        }

        /**    
        <summary>
            Set Enabled
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:12
        */
        public bool fn_SetEnabled(int nAxis)
        {
            m_bRet = false;

            try
            {
                _ACS.Enable ((Axis)nAxis);
                
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                System.Console.WriteLine("fn_SetEnabled : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_SetEnabledSMC(int nAxis)
        {
            if(nAxis < (int)EN_MOTR_ID.miSPD_Z1) return false;

            //int nindex = fn_GetIndexNo(nAxis);//((nAxis - (int)EN_MOTR_ID.miSPD_Z1)) * 10;
            //IO.DATA_EQ_TO_SMC[nindex + (int)EN_SMC_WRITE.SERVO_ON] = 1;
            
            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.SERVO_ON, 1);

            return true;
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_SetDisabledSMC(int nAxis)
        {
            if (nAxis < (int)EN_MOTR_ID.miSPD_Z1) return false;

            //int nindex = fn_GetIndexNo(nAxis);//((nAxis - (int)EN_MOTR_ID.miSPD_Z1)) * 10;
            //IO.DATA_EQ_TO_SMC[nindex + (int)EN_SMC_WRITE.SERVO_ON] = 1;

            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.SERVO_ON, 0);

            return true;
        }

        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set Disabled
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:13
        */
        public bool fn_SetDisabled(int nAxis)
        {
            try
            {
                _ACS.Disable((Axis)nAxis);

                //Home End Off
                //int VAR = 0; 
                //_ACS.WriteVariable(VAR, "HomeFlag", ProgramBuffer.ACSC_NONE, nAxis, nAxis);
                fn_SetHomeFlagOff(nAxis);

                return true;
            }
            catch (ACSException ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;

            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set Disable All
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:14
        */
        public bool fn_SetDisableAll()
        {
            m_bRet = false;
            try
            {
                _ACS.DisableAll();
                
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetDisableAll : " + ex.Message);
                return m_bRet;
            }
        }
        //---------------------------------------------------------------------------
        public bool fn_SetHomeFlagOff(int nAxis)
        {
            try
            {
                //Home End Off
                _ACS.WriteVariable(0, "HomeFlag", ProgramBuffer.ACSC_NONE, nAxis, nAxis);

                return true;
            }
            catch (ACSException ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;

            }
        }


        //-------------------------------------------------------------------------------------------------
        #endregion

        #region Motion Parameter

        /**    
        <summary>
            Get Velocity Parameter
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:34
        */
        public double fn_GetParameterVelocity(int nAxis)
        {
            return Convert.ToDouble(_ACS.GetVelocity((Axis)nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Acceleration Parameter
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:42
        */
        public double fn_GetParameterAcceleration(int nAxis)
        {
            return Convert.ToDouble(_ACS.GetAcceleration((Axis)nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Deceleration Parameter
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:43
        */
        public double fn_GetParameterDeceleration(int nAxis)
        {
            return Convert.ToDouble(_ACS.GetDeceleration((Axis)nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Kill Deceleration Parameter
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:44
        */
        public double fn_GetParameterKillDeceleration(int nAxis)
        {
            return Convert.ToDouble(_ACS.GetKillDeceleration((Axis)nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Jerk Parameter
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:45
        */
        public double fn_GetParameterJerk(int nAxis)
        {
            return Convert.ToDouble(_ACS.GetJerk((Axis)nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set Velocity Parameter
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dVel">Velocity</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:41
        */
        public bool fn_SetParameterVelocity(int nAxis, double dVel)
        {
            m_bRet = false;
            try
            {
                _ACS.SetVelocity((Axis)nAxis, dVel);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetParameterVelocity : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set Acceleration Parameter
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dAcc">Acceleration</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:41
        */
        public bool fn_SetParameterAcceleration(int nAxis, double dAcc)
        {
            m_bRet = false;
            try
            {
                _ACS.SetAcceleration((Axis)nAxis, dAcc);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetParameterAcceleration : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set Deceleration Parameter
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dDec">Deceleration</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:41
        */
        public bool fn_SetParameterDeceleration(int nAxis, double dDec)
        {
            m_bRet = false;
            try
            {
                _ACS.SetDeceleration((Axis)nAxis, dDec);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetParameterDeceleration : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set KillDeceleration Parameter
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dKillDec">KillDeceleration</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:41
        */
        public bool fn_SetParameterKillDeceleration(int nAxis, double dKillDec)
        {
            m_bRet = false;
            try
            {
                _ACS.SetKillDeceleration((Axis)nAxis, dKillDec);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetParameterKillDeceleration : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set Jerk Parameter
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dJerk">Jerk</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:41
        */
        public bool fn_SetParameterJerk(int nAxis, double dJerk)
        {
            m_bRet = false;
            try
            {
                _ACS.SetJerk((Axis)nAxis, dJerk);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetParameterJerk : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Set VAD Parameter (Vel, Acc, Dec)
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dVel">Velocity</param>
        <param name="dAcc">Acc</param>
        <param name="dDec">Dec</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:41
        */
        public bool fn_SetParameterVAD(int nAxis, double dVel, double dAcc, double dDec)
        {
            m_bRet = false;
            try
            {
                _ACS.SetVelocity        ((Axis)nAxis, dVel     );
                _ACS.SetAcceleration    ((Axis)nAxis, dAcc     );
                _ACS.SetDeceleration    ((Axis)nAxis, dDec     );

                _ACS.SetKillDeceleration((Axis)nAxis, dAcc * 10); //JUNG/200911
                _ACS.SetJerk            ((Axis)nAxis, dAcc * 10); //JUNG/200911

                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetParameterVAD : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        #endregion

        #region Motion Update Variable
        /**    
        <summary>
            Get Motor State Enabled
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:29
        */
        public bool fn_GetStateEnabled(int nAxis)
        {
            m_nMotorState = _ACS.GetMotorState((Axis)nAxis);
            if ((m_nMotorState & MotorStates.ACSC_MST_ENABLE) != 0)
                return true;
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Motor State In position
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:35
        */
        public bool fn_GetStateInposition(int nAxis)
        {
            m_nMotorState = _ACS.GetMotorState((Axis)nAxis);
            if ((m_nMotorState & MotorStates.ACSC_MST_INPOS) != 0)
                return true;
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Motor State Acceleration
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:35
        */
        public bool fn_GetStateAcceleration(int nAxis)
        {
            m_nMotorState = _ACS.GetMotorState((Axis)nAxis);
            if ((m_nMotorState & MotorStates.ACSC_MST_ACC) != 0)
                return true;
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Motor State Move
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:35
        */
        public bool fn_GetStateMove(int nAxis)
        {
            m_nMotorState = _ACS.GetMotorState((Axis)nAxis);
            if ((m_nMotorState & MotorStates.ACSC_MST_MOVE) != 0)
                return true;
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Motor State
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 10:56
        */
        public MotorStates fn_GetStateMotor(int nAxis)
        {
            return m_nMotorState = _ACS.GetMotorState((Axis)nAxis);
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Safety State Alarm
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 10:45
        */
        public SafetyControlMasks fn_GetStateAlarm(int nAxis)
        {
            return m_nMotorAlarm = _ACS.GetFault((Axis)nAxis);
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_SetFaultClear(int nAxis)
        {
            m_bRet = false;
            try
            {
                if(m_bSynch) _ACS.FaultClear     ((Axis)nAxis);
                else         _ACS.FaultClearAsync((Axis)nAxis);
                m_bRet = true;
                return m_bRet;
            }
            catch (System.Exception ex)
            {
                //System.Diagnostics.Trace.WriteLine(ex.Message);
                Console.WriteLine("fn_SetFaultClear : " + ex.Message);

                return m_bRet;
            }
            
        }
        //---------------------------------------------------------------------------
        public bool fn_SetFaultClearSMC(int nAxis)
        {
            //int nIndex = fn_GetIndexNo(nAxis);
            //IO.DATA_EQ_TO_SMC[nIndex+(int)EN_SMC_WRITE.FAULT_RESET] = 1;

            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.FAULT_RESET, 1);

            return true; 
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Motor Alarm Status
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/11 09:54
        */
        public bool fn_GetStateAlarmAll(int nAxis)
        {
            m_nMotorAlarm = _ACS.GetFault((Axis)nAxis);
            if ((m_nMotorAlarm & SafetyControlMasks.ACSC_ALL) != 0)
                return true;
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Hardware Left Limit
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/11 09:47
        */
        public bool fn_GetStateHardwareLeftLimit(int nAxis)
        {
            m_nMotorAlarm = _ACS.GetFault((Axis)nAxis);
            if ((m_nMotorAlarm & SafetyControlMasks.ACSC_SAFETY_LL) != 0)
                return true;
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Hardware Right Limit
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/11 09:47
        */
        public bool fn_GetStateHardwareRightLimit(int nAxis)
        {
            m_nMotorAlarm = _ACS.GetFault((Axis)nAxis);
            if ((m_nMotorAlarm & SafetyControlMasks.ACSC_SAFETY_RL) != 0)
                return true;
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Software Left Limit
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/12 15:04
        */
        public bool fn_GetStateSoftwareLeftLimit(int nAxis)
        {
            m_nMotorAlarm = _ACS.GetFault((Axis)nAxis);
            if ((m_nMotorAlarm & SafetyControlMasks.ACSC_SAFETY_SLL) != 0)
                return true;
            else
                return false;
        }
        //------------------------------------------------------------------------------------------------- 
        /**    
        <summary>
            Get Software Right Limit
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/12 15:04
        */
        public bool fn_GetStateSoftwareRightLimit(int nAxis)
        {
            m_nMotorAlarm = _ACS.GetFault((Axis)nAxis);
            if ((m_nMotorAlarm & SafetyControlMasks.ACSC_SAFETY_SRL) != 0)
                return true;
            else
                return false;
        }
        //------------------------------------------------------------------------------------------------- 
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Encoder Position
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 15:59
        */
        public double fn_GetEncoderPosition(int nAxis)
        {
            return Convert.ToDouble(_ACS.GetFPosition((Axis)nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Target Position
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 16:02
        */
        public double fn_GetTargetPosition(int nAxis)
        {
            return Convert.ToDouble(_ACS.GetRPosition((Axis)nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Position Error
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 16:07
        */
        public double fn_GetPositionError(int nAxis)
        {
            return Convert.ToDouble(_ACS.ReadVariable("PE", ProgramBuffer.ACSC_NONE, nAxis, nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Velocity Profile
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 16:10
        */
        public double fn_GetVelocity(int nAxis)
        {
            return Convert.ToDouble(_ACS.ReadVariable("FVEL", ProgramBuffer.ACSC_NONE, nAxis, nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Acceleration Profile
        </summary>
        <param name="nAxis"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 16:32
        */
        public double fn_GetAcceleration(int nAxis)
        {
            return Convert.ToDouble(_ACS.ReadVariable("FACC", ProgramBuffer.ACSC_NONE, nAxis, nAxis));
        }
        //-------------------------------------------------------------------------------------------------
        #endregion

        #region Move Absolute Position
        /**    
        <summary>
            Move Absolute Position (Position)
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dPos">Position Value</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:27
        */
        public bool fn_MoveAbsPosition(int nAxis, double dPos)
        {
            //if (fn_GetStateMove(nAxis)) return false;

            m_bRet = false;
            try
            {
                _ACS.ToPoint(MotionFlags.ACSC_NONE, (Axis)nAxis, dPos);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_MoveAbsPosition : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_MoveAbsPositionSMC(int nAxis, double dPos)
        {
            
            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.START_FLAG, 1);
            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.TARGET_POS, (int)dPos);
           

            //bool bInpos = IO.fn_GetSMCData((EN_MOTR_ID)nAxis, EN_SMC_READ.INPOSITION)==1;
            //bool bBusy  = IO.fn_GetSMCData((EN_MOTR_ID)nAxis, EN_SMC_READ.BUSY      )==1;
            //int    nCurrPos = IO.fn_GetSMCData((EN_MOTR_ID)nAxis, EN_SMC_READ.CURRENT_POS) * 100;
            //int    nTargPos = (int)dPos;
            //int    nCapPos  = Math.Abs(nCurrPos - nTargPos);
            //double dInpos   = MOTR[nAxis].MP.dPosn[(int)EN_POSN_ID.InPos];
            //bool   bMoveDone = nCapPos < dInpos;
            //return (bInpos && !bBusy && bMoveDone); 

            return true; 

        }

        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Move Absolute Position (Vel,Acc,Dec,Pos)
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dPos">Position</param>
        <param name="dVel">Velocity</param>
        <param name="dAcc">Acceleration</param>
        <param name="dDec">Deceleration</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 19:38
        */
        public bool fn_MoveAbsolutePosition(int nAxis, double dPos, double dVel, double dAcc, double dDec)
        {
            if (fn_GetStateMove(nAxis))
                return false;
            if (!fn_SetParameterVAD(nAxis, dVel, dAcc, dDec))
                return false;
            
            m_bRet = false;
            try
            {
                _ACS.ToPoint(MotionFlags.ACSC_NONE, (Axis)nAxis, dPos);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                //System.Diagnostics.Trace.WriteLine(ex.Message);
                Console.WriteLine("fn_MoveAbsolutePosition : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        #endregion

        #region Move Relative Position
        /**    
        <summary>
            Move Relative Position (Position, Direction)
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dPos">Position</param>
        <param name="bDir">true:Positive, false:Negative</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 09:03
        */
        public bool fn_MoveRelativePosition(int nAxis, double dPos, bool bDir)
        {
            if (fn_GetStateMove(nAxis))
                return false;

            m_bRet = false;

            double dTargetPos;

            if (bDir)
                dTargetPos = dPos;
            else
                dTargetPos = dPos * (-1);

            try
            {
                _ACS.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, (Axis)nAxis, dTargetPos);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_MoveRelativePosition : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Move Relative Position (Position, Direction, VAD)
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="dPos">Position</param>
        <param name="bDir">true:Positive, false:Negative</param>
        <param name="dVel">Velocity</param>
        <param name="dAcc">Acceleration</param>
        <param name="dDec">Deceleration</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 09:03
        */
        public bool fn_MoveRelativePosition(int nAxis, double dPos, bool bDir, double dVel, double dAcc, double dDec)
        {
            if (fn_GetStateMove(nAxis))
                return false;
            if (!fn_SetParameterVAD(nAxis, dVel, dAcc, dDec))
                return false;

            m_bRet = false;

            double dTargetPos;

            if (bDir)
                dTargetPos = dPos;
            else
                dTargetPos = dPos * (-1);

            try
            {
                _ACS.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, (Axis)nAxis, dTargetPos);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_MoveRelativePosition : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        #endregion

        #region Move Jog
        /**    
        <summary>
            Move Jog (Velocity)
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="bDir">true:Positive, false:Negative</param>
        <param name="dVel">Velocity</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 09:39
        */
        public bool fn_MoveJog(int nAxis, bool bDir, double dVel)
        {
            m_bRet = false;

            double dTargetVel = 0 ;

            if (bDir) dTargetVel = dVel;
            else      dTargetVel = dVel * (-1);

            try
            {
                _ACS.Jog(MotionFlags.ACSC_AMF_VELOCITY, (Axis)nAxis, dTargetVel);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_MoveJog : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_MoveJogSMC(int nAxis, bool bDir, double dVel)
        {
            //
            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.TARGET_POS, (int)dVel*100);

            if (bDir) IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.JOG_RIGHT, 1);
            else      IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.JOG_LEFT , 1);

            return true; 
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Jog Stop
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 09:47
        */
        public bool fn_StopJog(int nAxis)
        {
            m_bRet = false;

            try
            {
                _ACS.Halt((Axis)nAxis);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_StopJog : " + ex.Message);
                return m_bRet;
            }
        }
        #endregion

        #region Motion Set Position
        /**    
        <summary>
            Set Position
        </summary>
        <param name="nAxis">Axis No</param>
        <param name="dPos">Position Value</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/03 17:18
        */
        public bool fn_SetFPos(int nAxis, double dPos)
        {
            bool bRet = false;
            try
            {
                _ACS.SetFPosition((Axis)nAxis, dPos);
                bRet = true;
                return bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetFPos : " + ex.Message);
                return bRet;
            }
        }
        public bool fn_SetRPos(int nAxis, double dPos)
        {
            bool bRet = false;
            try
            {
                _ACS.SetRPosition((Axis)nAxis, dPos);
                bRet = true;
                return bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetRPos : " + ex.Message);
                return bRet;
            }
        }

        #endregion

        #region Stop Method
        /**    
        <summary>
            Normal Stop
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 09:53
        */
        public bool fn_StopNor(int nAxis)
        {
            m_bRet = false;

            try
            {
                _ACS.Halt((Axis)nAxis);

                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_StopNor : " + ex.Message);
                return m_bRet;
            }
        }
        public bool fn_StopAll()
        {
            m_bRet = false;

            try
            {
                if (m_arrAxisList != null) _ACS.HaltM(m_arrAxisList);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.ErrorCode);
                Console.WriteLine("fn_StopNor : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Emergency Stop
        </summary>
        <param name="nAxis">Axis No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 09:57
        */
        public bool fn_StopEMG(int nAxis)
        {
            m_bRet = false;

            try
            {
                _ACS.Kill((Axis)nAxis);
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_StopEMG : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_StopEMGSMC(int nAxis)
        {
            //IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.SERVO_ON  , 0);
            //IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.HOLD      , 1);

            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.JOG_RIGHT , 0);
            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.JOG_LEFT  , 0);
            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.START_FLAG, 0);

            return true; 
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_Stop(int nAxis, bool DecStop = false, double DecTime = 0.1)
        {
            //Local Var.
            bool uReturn = false;
            if (!m_bServo) return false;

            if (m_bKeepHomeProc)
            {
                m_bKeepHomeProc = false;
                m_iStepHome = 0;
            }

            //Stop.
            if (!DecStop || (DecTime <= 0)) uReturn = fn_StopEMG(nAxis);
            else
            {
                uReturn = fn_StopNor(nAxis);
            }

            return !m_bBusy ;
        }
        //---------------------------------------------------------------------------
        public bool fn_StopSMC(int nAxis, bool DecStop = false, double DecTime = 0.1)
        {
            //Local Var.
            bool uReturn = false;

            if (!m_bServo) return false;

            //Stop.
            uReturn = fn_StopEMGSMC(nAxis);

            return !m_bBusy;
        }

        //--------------------------------------------------------------------------
        //_fMoveJogP(m_iObjNo, Vel, MP.dAcc[(int)EN_MOTR_VEL.Work], MP.dDec[(int)EN_MOTR_VEL.Work]);
        public bool fn_MoveJogP(int nAxis, double Vel = 40.0, double Acc = 0.01, double Dec = 0.0)
        {
            bool uReturn = false;

            //Check Status.
            if (m_bAlarm ) return false;
            if (!m_bServo) return false;

            if (Vel <= 0) return false;
            if (Acc <= 0) return false;
            if (Dec <= 0) Acc = Dec;
            if (Vel < 0.1) Vel = 0.1;

            double lVel = ConvVel(Vel     );
            double lAcc = ConvAcc(Vel, Acc);
            double lDec = ConvAcc(Vel, Dec);

            uReturn = fn_MoveJog(nAxis, true, lVel); //_ACS.Jog(MotionFlags.ACSC_AMF_VELOCITY, (Axis)nAxis, lVel);

            return uReturn;
        }
        //--------------------------------------------------------------------------
        public bool fn_MoveJogPSMC(int nAxis, double Vel = 40.0, double Acc = 0.01, double Dec = 0.0)
        {
            bool uReturn = false;

            //Check Status.
            if (m_bAlarm ) return false;
            if (!m_bServo) return false;

            if (Vel <= 0) return false;
            if (Acc <= 0) return false;
            if (Dec <= 0) Acc = Dec;
            if (Vel < 0.1) Vel = 0.1;

            double lVel = ConvVel(Vel     );
            double lAcc = ConvAcc(Vel, Acc);
            double lDec = ConvAcc(Vel, Dec);

            uReturn = fn_MoveJogSMC(nAxis, true, lVel); //_ACS.Jog(MotionFlags.ACSC_AMF_VELOCITY, (Axis)nAxis, lVel);

            return uReturn;
        }

        //--------------------------------------------------------------------------
        public bool fn_MoveJogN(int nAxis, double Vel = 40.0, double Acc = 0.01, double Dec = 0.0)
        {
            bool uReturn = false;

            //Check Status.
            if ( m_bAlarm) return false;
            if (!m_bServo) return false;

            if (Vel <= 0) return false;
            if (Acc <= 0) return false;
            if (Dec <= 0) Acc = Dec;
            if (Vel < 0.1) Vel = 0.1;

            double lVel = ConvVel(Vel);
            double lAcc = ConvAcc(Vel, Acc);
            double lDec = ConvAcc(Vel, Dec);

            uReturn = fn_MoveJog(nAxis, false, lVel); //_ACS.Jog(MotionFlags.ACSC_AMF_VELOCITY, (Axis)nAxis, lVel);

            return uReturn;
        }
        //--------------------------------------------------------------------------
        public bool fn_MoveJogNSMC(int nAxis, double Vel = 40.0, double Acc = 0.01, double Dec = 0.0)
        {
            bool uReturn = false;

            //Check Status.
            if ( m_bAlarm) return false;
            if (!m_bServo) return false;

            if (Vel <= 0) return false;
            if (Acc <= 0) return false;
            if (Dec <= 0) Acc = Dec;
            if (Vel < 0.1) Vel = 0.1;

            double lVel = ConvVel(Vel    );
            double lAcc = ConvAcc(Vel, Acc);
            double lDec = ConvAcc(Vel, Dec);

            uReturn = fn_MoveJogSMC(nAxis, false, lVel); //_ACS.Jog(MotionFlags.ACSC_AMF_VELOCITY, (Axis)nAxis, lVel);

            return uReturn;
        }

        //--------------------------------------------------------------------------
        public bool fn_Move(int nAxis, double Pos      , double Vel = 20.0,
                                       double Acc = 0.3, double Dec =  0.0, double SndPos = 0.0, int iSpdRatio = 0)
        {
            //Local Var.
            bool bFuncRet = false ;

            if ( m_bAlarm) return false;
            if (!m_bServo) return false;
            if (Dec <= 0) Acc = Dec;

            if (m_iMotrKind == (int)EN_MOTR_KIND.ABS) Pos = Pos + m_dAbsOffset;

            double lVel = ConvVel(Vel     );
            double lAcc = Vel * 10; //ConvAcc(Vel, Acc);
            double lDec = Vel * 10; //ConvAcc(Vel, Dec);

            bFuncRet = fn_SetParameterVAD(nAxis, lVel, lAcc, lDec);
            if (!bFuncRet) return false;

            bFuncRet = fn_MoveAbsPosition(nAxis, Pos);

            return bFuncRet;
        }
        //--------------------------------------------------------------------------
        public bool fn_MoveSMC(int nAxis, double Pos, double Vel = 20.0,
                                          double Acc = 0.3, double Dec = 0.0, double SndPos = 0.0, int iSpdRatio = 0)
        {
            //Local Var.
            bool bFuncRet = false ;

            if ( m_bAlarm) return false;
            if (!m_bServo) return false;
            if (Dec <= 0) Acc = Dec;

            if (m_iMotrKind == (int)EN_MOTR_KIND.ABS) Pos = Pos + m_dAbsOffset;

            double lVel = Vel; //ConvVel(Vel     );
            double lAcc = Acc; //ConvAcc(Vel, Acc);
            double lDec = Dec; //ConvAcc(Vel, Dec);

            
            IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.TARGET_VEL, (int)lVel);

            bFuncRet = fn_MoveAbsPositionSMC(nAxis, Pos * 100);

            return bFuncRet;
        }

        //--------------------------------------------------------------------------
        public bool fn_MoveHome(int iAxis, double Vel, double Acc, double Dec = 0.0, double OffsetPulse = 0.0, double OffSetPos = 0.0)
        {
            if ( m_bAlarm      ) return false;
            if (!m_bServo      ) return false;
            if (Vel <= 0       ) return false;
            if (m_bKeepHomeProc) return false;
            if (Dec <= 0       ) Acc = Dec;

            if (m_iMotorKind == (int)EN_MOTR_KIND.ABS || m_iHomeType == (int)EN_HOME_TYPE.DataSet)
            {
                //
                m_bHomeEnd = true;
            }
            else
            {
                m_dHomeVel       = Vel; 
                m_dHomeAcc       = Acc;
                m_dHomeDec       = Dec;
                m_dHomeOffset    = OffsetPulse;
                m_dHomeOffsetPos = OffSetPos;
                m_iStepHome      = 10;
                m_bKeepHomeProc  = true;
            }

            return true;
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_MoveHomeSMC(int nAxis, double Vel, double Acc, double Dec = 0.0, double OffsetPulse = 0.0, double OffSetPos = 0.0)
        {
            if ( m_bAlarm      ) return false;
            if (!m_bServo      ) return false;
            if (Vel <= 0       ) return false;
            if (m_bKeepHomeProc) return false;
            if (Dec <= 0       ) Acc = Dec;

            if (m_iMotorKind == (int)EN_MOTR_KIND.ABS || m_iHomeType == (int)EN_HOME_TYPE.DataSet)
            {
                //
                m_bHomeEnd     = true;
                m_bHomeEndDone = true; 
            }
            else
            {
                IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.FAULT_RESET, 1);
                IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.SERVO_ON   , 1);
                IO.fn_SetSMCData((EN_MOTR_ID)nAxis, EN_SMC_WRITE.HOME       , 1);
            }

            return true;
        }

        //--------------------------------------------------------------------------        
        //Position Functions.
        public double fn_GetCmdPos(int nAxis)
        {
//            double cmdPos = 0.0;
//             if (m_bRing)
//             {
//                 CAXM.AxmStatusGetCmdPos(iAxis, ref cmdPos);
//                 m_dCmdPos = cmdPos;
//             }
            return m_dCmdPos;
        }
        //--------------------------------------------------------------------------
        public double fn_GetEncPos(int nAxis)
        {//Get Encoder Position.

            bool isStepMotr = (m_iMotorType == (int)EN_MOTR_TYPE.StepOriental) ||
                              (m_iMotorType == (int)EN_MOTR_TYPE.Step);
            return (isStepMotr) ? m_dCmdPos : m_dEncPos;
        }
        //---------------------------------------------------------------------------
        public bool fn_SetPos(int nAxis, double Pos)
        {
            if (m_iMotorKind == (int)EN_MOTR_KIND.ABS) return false;

            bool bRtn = true;

            if (!fn_SetFPos(nAxis, Pos)) bRtn = false; 
            if (!fn_SetRPos(nAxis, Pos)) bRtn = false;

            return bRtn;
        }
        //--------------------------------------------------------------------------
        public bool fn_SetPosEncToCmd(int nAxis)
        {
            //Local Var.
            double encPos = 0.0;
            bool   bRtn = true;

            encPos = fn_GetEncoderPosition(nAxis);
            if (!fn_SetRPos(nAxis, encPos)) bRtn = false;

            return bRtn;
        }
        //--------------------------------------------------------------------------
        public bool fn_SetPosEncToCmdSMC(int nAxis)
        {
            //Local Var.
            double encPos = 0.0;
            bool   bRtn   = true;
            
            //int    nIndex = fn_GetIndexNo(nAxis);
            //encPos = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.CURRENT_POS];
            encPos = IO.fn_GetSMCData((EN_MOTR_ID)nAxis, EN_SMC_READ.CURRENT_POS);

            //if (!fn_SetRPos(nAxis, encPos)) bRtn = false;

            return bRtn;
        }

        //---------------------------------------------------------------------------
        public void fn_ClearPos(int nAxis, double Pos = 0.0)
        {
            //m_bHomeEnd = false;
            if (m_bKeepHomeProc)
            {
                m_bKeepHomeProc = false;
                m_iStepHome = 0;
                fn_Stop(nAxis);
            }
            fn_SetPos(nAxis, Pos);
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_ClearPosSMC(int nAxis, double Pos = 0.0)
        {

        }

        //---------------------------------------------------------------------------
        //Get Function
        public bool fn_GetStop(int nAxis, bool ChkEnc = false, double InPos = 0.1)
        {//Motion Done.
            return false;
        }
        //--------------------------------------------------------------------------
        public bool fn_MotionDone(int nAxis)
        {
            return m_bStop;
        }
        //--------------------------------------------------------------------------
        public void fn_SetType(int nAxis, int iType = 0, int iKind = 0, int iNotUse = 0)
        {
            m_iMotorType = iType;
            m_iMotorKind = iKind;
        }

        //--------------------------------------------------------------------------
        public void fn_SetHomeType(int iAxis, int Data)
        {
            m_iHomeType = Data;
        }
        //--------------------------------------------------------------------------
        public void fn_SetHomeOptn(int iAxis, int Data, int Data2)
        {
            m_iHomeDir = Data2;
        }

        public void fn_SetCoefficient(double Data = 819)
        {
            m_dCoef = Data;
        }
        //--------------------------------------------------------------------------
        public void fn_SetABS(int iAxis, int Data1 = 0, int Data2 = 0)
        {
            m_iMotrKind = Data1;
        }



        //--------------------------------------------------------------------------
        private double ConvVel(double Vel)
        {
            double lVel = m_dCoef * Vel;

            return lVel;
        }
        //--------------------------------------------------------------------------
        private double ConvAcc(double Vel, double Acc)
        {
            double lAcc = m_dCoef * Vel * (1.0 / Acc);

            return lAcc;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
            Emergency Stop All
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 10:01
        */
        public bool fn_StopEMGAll()
        {
            m_bRet = false;

            try
            {
                _ACS.KillAll();
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_StopEMGAll : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Register Emergency Stop Enable
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 10:34
        */
        public bool fn_StopEMGRegisterEnabled()
        {
            m_bRet = false;

            try
            {
                _ACS.RegisterEmergencyStop();
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException)
            {
                return m_bRet;
            }
            
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Register Emergency Stop Disable
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 10:34
        */
        public bool fn_StopEMGRegisterDisabled()
        {
            m_bRet = false;

            try
            {
                _ACS.UnregisterEmergencyStop();
                m_bRet = true;
                return m_bRet;
            }
            catch (ACSException)
            {
                return m_bRet;
            }

        }
        //-------------------------------------------------------------------------------------------------
        #endregion

        #region Safety Method
        /**    
        <summary>
            Set Software Limit (Enabled/Disabled)
        </summary>
        <param name="nAxis">Axis No.</param>
        <param name="bDir">true:Positive, false:Negative</param>
        <param name="bUse">true:Use, false:Not Use</param>
        <param name="dValue">Limit Position</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 13:41
        */
        public bool fn_SetSoftwareLimit(int nAxis, bool bDir, bool bUse, double dValue)
        {
            try
            {
                if (bUse)
                {
                    if (bDir)
                    {
                        _ACS.WriteVariable(dValue, "SRLIMIT", ProgramBuffer.ACSC_NONE, nAxis, nAxis, -1, -1);
                        _ACS.EnableFault  ((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SRL);
                    }
                    else
                    {
                        _ACS.WriteVariable(dValue, "SLLIMIT", ProgramBuffer.ACSC_NONE, nAxis, nAxis, -1, -1);
                        _ACS.EnableFault((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SLL);
                    }
                }
                else
                {
                    if (bDir) _ACS.DisableFault((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SRL);
                    else      _ACS.DisableFault((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SLL);
                }

                
                return true;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetSoftwareLimit : " + ex.Message);
                return false;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Get Software Limit Value
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/12 15:15
        */
        //public double fn_GetSoftwareLimitValue(int nAxis, bool bDir)
        //{
        //    m_bRet = false;
        //}
        #endregion

        #region ACSPL+ Buffer
        /**    
        <summary>
            ACSPL+ Set Buffer P/R (Run/Stop)
        </summary>
        <param name="nBuffNo">Buffer No.</param>
        <param name="bOn">true: Run, false: Stop</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 14:56
        */
        public bool fn_SetBuffer(int nBuffNo, bool bOn)
        {
            m_bRet = false;

            try
            {
                if (bOn)
                    _ACS.RunBuffer((ProgramBuffer)nBuffNo, null);
                else
                    _ACS.StopBuffer((ProgramBuffer)nBuffNo);

                m_bRet = true;
                return m_bRet;

            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetBuffer : " + ex.Message);
                return m_bRet;
            }
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            ACSPL+ Get Buffer P/R Status
        </summary>
        <param name="nBuffNo">Buffer No.</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/04 15:01
        */
        public ProgramStates fn_GetBuffer(int nBuffNo)
        {
            m_nProgState = _ACS.GetProgramState((ProgramBuffer)nBuffNo);
            
            return m_nProgState;
        }
        public bool fn_IsBuffRun(int nBuffNo)
        {
            int m_nProgState = (int)_ACS.GetProgramState((ProgramBuffer)nBuffNo);

            bool rtn = (m_nProgState == (int)ProgramStates.ACSC_PST_RUN) || m_nProgState == 3;

            return rtn;
        }

        //-------------------------------------------------------------------------------------------------
        #endregion


        //---------------------------------------------------------------------------
        
        public void fn_Update(int nAxis)
        {
            try
            {
                //Check Axis
                if ((nAxis < 0) || (nAxis >= MOTR._iNumOfMotr)) return;
                
                //Local Val
                MotorStates        MotionInfo = new MotorStates       ();
                SafetyControlMasks fault      = new SafetyControlMasks();
                
                double      dEncPos     ;
                int         nScanTime   ;
                int         nStrtTime   ; 
                double      fPosition = 0.0;
                double      rPosition = 0.0;

                //
                nStrtTime = Environment.TickCount;

                if (m_bSynch)
                {
                    // Synchronous get motor state
                    MotionInfo = _ACS.GetMotorState((Axis)nAxis);

                    // Synchronous get axis state
                    //AxisStates state = _ACS.GetAxisState((Axis)nAxis);

                    fault = _ACS.GetFault((Axis)nAxis);

                    // Synchronous get motor feedback position
                    fPosition = _ACS.GetFPosition((Axis)nAxis);

                    // Synchronous get instant value of motor reference position
                    rPosition = _ACS.GetRPosition((Axis)nAxis);

                }
                else
                {
                    // Asynchronous get motor state
                    ACSC_WAITBLOCK wb = _ACS.GetMotorStateAsync((Axis)nAxis);
                    MotionInfo = (MotorStates)_ACS.GetResult(wb, 300);

                    ACSC_WAITBLOCK wb1 = _ACS.GetFaultAsync((Axis)nAxis);
                    fault = (SafetyControlMasks)_ACS.GetResult(wb1, 300);

                    ACSC_WAITBLOCK wb3 = _ACS.GetFPositionAsync((Axis)nAxis);
                    fPosition = (double)_ACS.GetResult(wb3, 100);

                    ACSC_WAITBLOCK wb4 = _ACS.GetRPositionAsync((Axis)nAxis);
                    rPosition = (double)_ACS.GetResult(wb4, 100);
                }


                //m_dTorque = GetTorque(iAxis);

                dEncPos       = fPosition;
                m_dCmdPos     = rPosition;
                m_dEncPos     = dEncPos  ;
                
                //Motion Status.
                m_bServo      = (MotionInfo & MotorStates       .ACSC_MST_ENABLE ) != 0; //Servo On
                m_bBusy       = (MotionInfo & MotorStates       .ACSC_MST_MOVE   ) != 0 ||
                                (MotionInfo & MotorStates       .ACSC_MST_ACC    ) != 0; //Busy.
	            m_bPackInPosn = (MotionInfo & MotorStates       .ACSC_MST_INPOS  ) != 0; //In Position. 
                
                m_bAlarm      =  fault      > SafetyControlMasks.ACSC_SAFETY_LL  && 
                                 fault     != SafetyControlMasks.ACSC_SAFETY_CPE && 
                                 fault     != SafetyControlMasks.ACSC_SAFETY_SRL && 
                                 fault     != SafetyControlMasks.ACSC_SAFETY_SLL &&
                                 fault     != SafetyControlMasks.ACSC_SAFETY_PE     ; //Alarm.

                m_bCCW        = (fault      & SafetyControlMasks.ACSC_SAFETY_LL  ) != 0; //CW.
                m_bCW         = (fault      & SafetyControlMasks.ACSC_SAFETY_RL  ) != 0; //CCW.

                m_bSRL        = (fault      & SafetyControlMasks.ACSC_SAFETY_SRL ) != 0; //Right Soft Limit
                m_bSLL        = (fault      & SafetyControlMasks.ACSC_SAFETY_SLL ) != 0; //Left Soft Limit



                //m_bHome       =  ((uMechaSig >> 7  ) & 0x01 ) == 0x01; //Home Sensor???

                if (m_iMotorType == 2) m_bPackInPosn = true;
	            
                m_bStop =  !m_bBusy && m_bPackInPosn ; //Stop.

                m_dOrgCmdPos  = rPosition;
                m_dOrgEncPos  =  dEncPos ;

                if (m_iMotrKind == (int)EN_MOTR_KIND.ABS)
                {
                    m_dCmdPos = m_dCmdPos - m_dAbsOffset;
                    m_dEncPos = m_dEncPos - m_dAbsOffset;
                }

	            m_bReady =  !m_bCW && !m_bCCW && !m_bAlarm && m_bHomeEnd && m_bServo && m_bPackInPosn && m_bStop; //Ready.
                
                //HomeProc(iAxis);

	            if (m_bBusy && m_iStepHome != 0) m_bLtBusy = true;

                //Servo On
                m_tServoWait.OnDelay(m_bReqServoOn, 1000);
                if(m_tServoWait.Out)
                {
                    m_bReqServoOn = false;
                    fn_SetEnabled(nAxis);
                    Thread.Sleep(50);
                    fn_StopNor(nAxis);
                    m_tServoWait.Clear();
                }

//                 m_tRingCounter.OnDelay(m_bReqRingCounter, 1000);
//                 if(m_tRingCounter.Out)
//                 {
//                     CAXM.AxmStatusSetPosType(iAxis, 1, m_dReqRingMaxPos, 0.0); // 0 : Default, 1: Ring Counter
//                     m_bReqRingCounter = false;
//                 }

                if(m_iMotorKind == (int)EN_MOTR_KIND.ABS || m_iHomeType == (int)EN_HOME_TYPE.DataSet) 
                {
                    m_tHomeEnd.OnDelay(m_bServo && !m_bHomeEnd , 500);
                    if(m_tHomeEnd.Out) m_bHomeEnd = true;
                }

                //Alarm Reset
                m_tAlarmReset.OnDelay(m_bReqResetAlarm, 1000);
                if(m_tAlarmReset.Out)
                {
                    m_bReqResetAlarm = false;
                    fn_SetFaultClear(nAxis);
                }

                //
                nScanTime = Environment.TickCount - nStrtTime;
            }
            catch (Exception ex)
            {
                Console.WriteLine("fAxis ACS Update : " + ex.Message);
                LOG.ExceptionTrace("Axis ACS Update", ex);
            }
        }
        //---------------------------------------------------------------------------
        public void fn_UpdateSMC(int nAxis)
        {
            try
            {
                //Check Axis
                if ((nAxis < (int)EN_MOTR_ID.miSPD_Z1) || (nAxis > (int)EN_MOTR_ID.miTRF_T)) return;
                
                //Local Val
                double      dEncPos     ;
                int         nScanTime   ;
                int         nStrtTime   ; 
                //double      fPosition = 0.0;
                double      rPosition = 0.0;

                //
                nStrtTime = Environment.TickCount;

                //                 int nindex = ((nAxis - (int)EN_MOTR_ID.miSPD_Z1)) * 10;
                // 
                //                 //Data Read
                //                 objVar = _ACS.ReadVariable("DATA_SMC_TO_EQ", ProgramBuffer.ACSC_NONE, nindex, nindex + 9);
                //                 InputArray = objVar as Array;
                //                 Array.Copy(InputArray, DATA_SMC_TO_EQ, DATA_SMC_TO_EQ.Length);


                //SMC Data 
                int nindex    = ((nAxis - (int)EN_MOTR_ID.miSPD_Z1)) * 30;
                
                dEncPos       = IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.CURRENT_POS];
                dEncPos       = dEncPos / 100.0;
                rPosition     = IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.TARGET_POS ];
                m_dCmdPos     = rPosition / 100.0;
                m_dEncPos     = dEncPos  ;
                
                //Motion Status.
                m_bServo      = IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.SERVO_ON     ] == 1; //Servo On
                m_bBusy       = IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.BUSY         ] == 1; //Busy.
                m_bPackInPosn = IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.INPOSITION   ] == 1; //In Position. 
                m_bAlarm      = IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.ALRAM        ] == 1 ||
                                IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.ESTOP        ] == 1; //Alarm.

	            m_bHomeEnd    = IO.DATA_SMC_TO_EQ[nindex + (int)EN_SMC_READ.HOME_COMPLETE] == 1; //Home End
                
                
                m_bSRL        = false;
                m_bSLL        = false;

                m_bCW         = false; //CW.
                m_bCCW        = false; //CCW.
                m_bHome       = false; //Home Sensor
                
                if (m_iMotorType == 2) m_bPackInPosn = true;

                m_bStop = !m_bBusy; //&& m_bPackInPosn ; //Stop.

                m_dOrgCmdPos  = rPosition;
                m_dOrgEncPos  =  dEncPos ;

                if (m_iMotrKind == (int)EN_MOTR_KIND.ABS)
                {
                    m_dCmdPos = m_dCmdPos - m_dAbsOffset;
                    m_dEncPos = m_dEncPos - m_dAbsOffset;
                }

	            m_bReady =  !m_bCW && !m_bCCW && !m_bAlarm && m_bHomeEnd && m_bServo && m_bPackInPosn && m_bStop; //Ready.
                
                //HomeProc(iAxis);

	            if (m_bBusy && m_iStepHome != 0) m_bLtBusy = true;

                //Servo On
                m_tServoWait.OnDelay(m_bReqServoOn, 1000);
                if(m_tServoWait.Out)
                {
                    m_bReqServoOn = false;
                    fn_SetEnabledSMC(nAxis);
                    Thread.Sleep(50);
                    //fn_StopNor(nAxis);
                    m_tServoWait.Clear();
                }

                if(m_iMotorKind == (int)EN_MOTR_KIND.ABS || m_iHomeType == (int)EN_HOME_TYPE.DataSet) 
                {
                    m_tHomeEnd.OnDelay(m_bServo && !m_bHomeEnd , 500);
                    if(m_tHomeEnd.Out) m_bHomeEnd = true;
                }

                //Alarm Reset
                m_tAlarmReset.OnDelay(m_bReqResetAlarm, 1000);
                if(m_tAlarmReset.Out)
                {
                    m_bReqResetAlarm = false;
                    fn_SetFaultClearSMC(nAxis);
                }

                //
                nScanTime = Environment.TickCount - nStrtTime;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Axis ACS Update(SMC) : " + ex.Message);
                LOG.ExceptionTrace("Axis ACS Update(SMC)", ex);
            }
        }
        //---------------------------------------------------------------------------
        public bool fn_GetSafetyRightLimit(int nAxis)
        {
            // The method reads Emergency Stop system safety input
            // Reads Right Limit Safety input of axis 0
            SafetyControlMasks sinput = _ACS.GetSafetyInput((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_RL);

            return (int)sinput > 0; 
        }
        public bool fn_GetSafetyLeftLimit(int nAxis)
        {
            // The method reads Emergency Stop system safety input
            // Reads Right Limit Safety input of axis 0
            SafetyControlMasks sinput = _ACS.GetSafetyInput((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_LL);

            return (int)sinput > 0; 
        }
        //---------------------------------------------------------------------------

        public void fn_SetFaultMask(int nAxis, SafetyControlMasks mask)
        {
            //The method sets the mask that enables or disables the examination and processing of the
            //controller faults.

            _ACS.SetFaultMask((Axis)nAxis, mask);

        }
        //---------------------------------------------------------------------------
        public void fn_SetHomeSensor(bool set)
        {
            m_bHome = set; //Home Sensor
        }
        //---------------------------------------------------------------------------

    }
}
