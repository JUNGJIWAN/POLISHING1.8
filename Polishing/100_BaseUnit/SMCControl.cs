
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;


namespace WaferPolishingSystem.BaseUnit
{


    public class SMCControl
    {
        //Const 
        const int MAX_READ   = 30;
        const int MAX_WRITE  = 30;
        const int DELAY_TIME = 1000; //

        //Var
        private int   m_nMotorNo, nIndex;
        
        private int   m_nStep       ;
        private int   m_nSeqStep    ;
        
        private bool  m_bDrngSMC    ;
        private bool  m_bDrngHome   ;
        private bool  m_bDrngServoOn;
        private bool  m_bDrngMove   ;
        private bool  m_bDrngReset  ;

        private bool  m_bReqReset   ;

        TOnDelayTimer m_tMainCycle = new TOnDelayTimer();
        TOnDelayTimer m_tDelayTime = new TOnDelayTimer();
        TOnDelayTimer m_tErrDelay  = new TOnDelayTimer();

        string m_sLog;

        //---------------------------------------------------------------------------
        //Property
        public bool _bDrngSMC { get { return m_bDrngSMC; } }



        //---------------------------------------------------------------------------
        public SMCControl(int motr)
        {
            m_nMotorNo = motr;
            
            int nStartMotr = (int)EN_MOTR_ID.miSPD_Z1;
            nIndex = (m_nMotorNo - nStartMotr) * MAX_READ;

            Init();

            m_nSeqStep     = 0;

            m_bDrngSMC     = false;
            m_bDrngHome    = false; 
            m_bDrngServoOn = false; 
            m_bDrngMove    = false; 
            m_bDrngReset   = false;
            m_bReqReset    = false;

            m_sLog         = string.Empty; 

        }
        //---------------------------------------------------------------------------
        public void Init()
        {
            m_nStep = 0; 

            m_tMainCycle.Clear();
            m_tDelayTime.Clear();
            m_tErrDelay .Clear();

        }
        //---------------------------------------------------------------------------
        public bool fn_Update()
        {
            //
            m_bDrngSMC = m_nSeqStep != 0;

            m_tMainCycle.OnDelay(m_bDrngSMC, 15 * 1000); //15sec
            if(m_tMainCycle.Out)
            {

                m_sLog = string.Format($"SMC Update Time Out [STEP] : {m_nSeqStep}");
                Console.WriteLine(m_sLog);
                WriteLog(m_sLog);

                m_nSeqStep = 0;
                fn_Clear();

                m_tErrDelay.Clear();

                return true;
            }

            if (!m_tErrDelay.OnDelay(true, 1000)) return true; //Error 발생 후 1sec Delay

            //
            if (m_nSeqStep == 0)
            {
                int nTarPos      = IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS ];
                int nReqTar      = IO.REQ_EQ_TO_SMC [nIndex + (int)EN_SMC_WRITE.TARGET_POS ];

                //SMC Data
                bool xServoOn    = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.SERVO_ON     ] == 1;
                bool xHomeEnd    = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.HOME_COMPLETE] == 1;
                bool xAlarm      = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.ALRAM        ] == 1;
                bool xReady      = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.READY        ] == 1;
                              
                //Request
                bool bReqServoOn = IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SERVO_ON   ] == 1;
                bool bReqHome    = IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME       ] == 1;
                bool bReqMove    = IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG ] == 1;
                bool bReqReset   = IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.FAULT_RESET] == 1;

                bool isConServoOn  =  bReqServoOn && !xServoOn;
                bool isConServoOff = !bReqServoOn &&  xServoOn;
                bool isConHome     =  bReqHome                ; //&& bReqServoOn
                bool isConMove     =  bReqMove                ; //&& xHomeEnd && xServoOn   && xAlarm;


                bool isConReset = (xAlarm || !xReady) && (bReqReset || m_bReqReset);

                m_tDelayTime.Clear();

                m_bDrngHome    = false;
                m_bDrngServoOn = false;
                m_bDrngMove    = false;
                m_bDrngReset   = false;

                if (isConHome   ) { m_bDrngHome    = true; m_nSeqStep = 100; m_nStep = 10; goto __GOTO_CYCLE__; }
                if (isConServoOn) { m_bDrngServoOn = true; m_nSeqStep = 200; m_nStep = 10; goto __GOTO_CYCLE__; }
                if (isConMove   ) { m_bDrngMove    = true; m_nSeqStep = 300; m_nStep = 10; goto __GOTO_CYCLE__; }
                if (isConReset  ) { m_bDrngReset   = true; m_nSeqStep = 400; m_nStep = 10; goto __GOTO_CYCLE__; }
                //if (isConServoOff) { m_bDrngServoOn = true; m_nSeqStep = 200; m_nStep = 30; goto __GOTO_CYCLE__; }
            }


            //Cycle Start
            __GOTO_CYCLE__:

            switch (m_nSeqStep)
            {
                case 100:
                    if (fn_Home()) m_nSeqStep = 0;
                    return false;

                case 200:
                    if (fn_SetServo()) m_nSeqStep = 0;
                    return false;
                
                case 300:
                    if (fn_SetMove()) m_nSeqStep = 0;
                    return false;

                case 400:
                    if (fn_Reset()) m_nSeqStep = 0;
                    return false;

                default:
                    m_bDrngHome    = false;
                    m_bDrngServoOn = false;
                    m_bDrngMove    = false;
                    m_bDrngReset   = false;
                    m_nSeqStep     = 0;
                    return true;
            }


        }
        //---------------------------------------------------------------------------
        private bool fn_SetMove()
        {
            bool r1;

            bool xServoOn  = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.SERVO_ON     ] == 1;
            bool xHomeEnd  = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.HOME_COMPLETE] == 1;
            bool xAlarm    = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.ALRAM        ] == 1;
            bool xReady    = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.READY        ] == 1;

            int  nTarPos   = IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS  ];
            int  nReqTar   = IO.REQ_EQ_TO_SMC [nIndex + (int)EN_SMC_WRITE.TARGET_POS  ];
            int  nCurrPos  = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.CURRENT_POS  ];


            if (m_nStep < 0) m_nStep = 0;

            switch (m_nStep)
            {
                case 10:
                    m_bDrngMove = true; 

                    //
                    IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG] = 0;

                    if(!xServoOn || !xHomeEnd || xAlarm || !xReady)
                    {
                        m_bDrngMove = false;

                        IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG] = 0;
                        m_nStep = 0;
                        return true; 
                    }

                    if (nReqTar == nCurrPos)
                    {
                        m_bDrngMove = false;

                        IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG] = 0;
                        m_nStep = 0;
                        return true;
                    }

                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_LEFT  ] = 0;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_RIGHT ] = 0;

                    //
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.POSITION_ON] = 1;  //SMC_CONTROL_FLAG(0).5 = 1
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SPEED_ON   ] = 1;  //SMC_CONTROL_FLAG(0).6 = 1
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS ] = IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS] ;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL ] = IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL] ;

                    Console.WriteLine("TARGET_POS :" + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS]);
                    Console.WriteLine("TARGET_VEL :" + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL]);
                    WriteLog("[MOVE] TARGET_POS :" + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS] + "/" + "TARGET_VEL :" + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL]);

                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 11:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG] = 1;

                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 12:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    r1 = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.INPOSITION] == 1; 
                    if (!r1) return false;

                    Console.WriteLine("[SMC] MOVE DONE   : " + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS]);
                    Console.WriteLine("      CURRENT_POS : " + IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.CURRENT_POS]);
                    WriteLog("[SMC] MOVE DONE   : " + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_POS]);

                    fn_Clear(); //JUNG/200523

                    m_bDrngMove = false;

                    m_nStep = 0;
                    return true; 

            }

            return true;
        }
        
        //---------------------------------------------------------------------------
        private bool fn_Home()
        {
            bool r1, r2; 
            int nCurrPos = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.CURRENT_POS];
            
            if (m_nStep < 0) m_nStep = 0;
            switch (m_nStep)
            {
                case 10:

                    m_bDrngHome = true;

                    //
                    IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME ] = 0;
                    
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME] = 0;

                    m_sLog = string.Format($"[HOME] Start - {m_nMotorNo}");
                    WriteLog(m_sLog);

                    //Check Servo On
                    if (IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.SERVO_ON] !=1)
                    {
                        m_nStep = 20;
                        return false;
                    }
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 11:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false; 

                    //Home
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL ] = IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL];  //(int)MOTR[m_nMotorNo].MP.dVel[(int)EN_MOTR_VEL.Home]; //Velocity
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME       ] = 1;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_LEFT   ] = 0;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_RIGHT  ] = 0;
                    
                    //Error Clear
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.FAULT_RESET] = 0;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOLD       ] = 0;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG ] = 0;

                    Console.WriteLine("[HOME] TARGET_VEL : " + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL]);
                    WriteLog("[HOME] TARGET_VEL : " + IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL]);

                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 12:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 13:
                    if (!m_tDelayTime.OnDelay(true, 100)) return false;

                    if(IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.ALRAM] == 1) //Error
                    {
                        fn_WriteLog("SMC - Home Fail");
                        WriteLog("SMC - Home Fail");
                        m_bDrngHome = false ;
                        m_nStep++;
                        return false;
                    }

                    r1 = IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.HOME_COMPLETE] == 1;
                    r2 = nCurrPos < 10;
                    if (!r1 || !r2) return false;
                    
                    MOTR[m_nMotorNo].SetHomeEndDone(true);

                    m_nStep++;
                    return false;

                case 14:
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME] = 0;

                    fn_Clear(); //JUNG/200523
                    
                    Console.WriteLine("[SMC] HOME DONE");
                    WriteLog("[SMC] HOME DONE");

                    m_bDrngHome = false; 
                    m_nStep =0;
                    return true ;

                //-------------------------------------------------------------------------------------------------
                case 20:
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SERVO_ON   ] = 1;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.FAULT_RESET] = 0;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG ] = 0;
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 21:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;
                    
                    if (IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.SERVO_ON] != 1) return false;

                    WriteLog("[SMC] HOME - SERVO ON");

                    m_nStep = 11;
                    return false;

                default:
                    m_bDrngHome = false;
                    m_nStep = 0;
                    return true;
            }

        }
        //---------------------------------------------------------------------------
        private bool fn_Reset()
        {
            if (m_nStep < 0) m_nStep = 0;
            switch (m_nStep)
            {
                case 10:

                    //
                    IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.FAULT_RESET ] = 0;
                    m_bReqReset = false ;

                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.FAULT_RESET] = 1;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOLD       ] = 0;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_LEFT   ] = 0;
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_RIGHT  ] = 0;

                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;
                
                case 11:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;
                
                case 12:
                    m_tDelayTime.OnDelay(true, 2000);
                    if(m_tDelayTime.Out)
                    {
                        fn_WriteLog("SMC - Reset Fail");
                        WriteLog("SMC - Reset Fail");
                        m_nStep++;
                        return false;
                    }

                    if (IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.ALRAM] != 0) return false;
                    
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 13:
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.FAULT_RESET] = 0;
                    
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;
                    
                    Console.WriteLine("[SMC] RESET DONE");
                    WriteLog("[SMC] RESET DONE");

                    fn_Clear(); //JUNG/200523

                    m_nStep = 0;
                    return true;

                default:
                    m_nStep = 0;
                    return true; 
            }


        }
        //---------------------------------------------------------------------------
        private bool fn_SetServo()
        {
            if (m_nStep < 0) m_nStep = 0;

            switch (m_nStep)
            {
                //Servo On
                case 10:

                    m_bDrngServoOn = true;

                    //
                    IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SERVO_ON] = 0;

                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SERVO_ON] = 1;
                    
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false ;
                
                case 11:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 12:
                    m_tDelayTime.OnDelay(true, 3 * 1000); //3sec
                    if(m_tDelayTime.Out)
                    {
                        fn_WriteLog("SMC - Servo On Fail");
                        WriteLog("SMC - Servo On Fail");
                        m_bDrngServoOn = false;
                        m_nStep = 30; //Reset
                        return false;
                    }

                    if (IO.DATA_SMC_TO_EQ[nIndex + (int)EN_SMC_READ.SERVO_ON] != 1) return false;

                    //
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.POSITION_ON] = 1;  //SMC_CONTROL_FLAG(0).5 = 1
                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SPEED_ON   ] = 1;  //SMC_CONTROL_FLAG(0).6 = 1
                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 13:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;

                    Console.WriteLine("[SMC] SERVO ON");
                    WriteLog("[SMC] SERVO ON");

                    fn_Clear(); //JUNG/200523

                    m_bDrngServoOn = false;

                    m_tDelayTime.Clear();
                    m_nStep=0;
                    return true;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Reset - Servo Off
                case 30:
                    IO.REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SERVO_ON] = 0;

                    IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SERVO_ON] = 0;

                    m_tDelayTime.Clear();
                    m_nStep++;
                    return false;

                case 31:
                    if (!m_tDelayTime.OnDelay(true, DELAY_TIME)) return false;

                    Console.WriteLine("[SMC] SERVO OFF");
                    WriteLog("[SMC] SERVO OFF");

                    m_nStep = 0;
                    return true;

                default:
                    m_nStep = 0;
                    return true;

            }

        }
        //-------------------------------------------------------------------------------------------------
        public void Reset()
        {
            m_bReqReset = true; 
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_Clear()
        {
            IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME     ] = 0;
            IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_LEFT ] = 0;
            IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.JOG_RIGHT] = 0;
            IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOLD     ] = 0;
            
            IO.DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.START_FLAG] = 0;//JUNG/200429

            m_bDrngHome    = false; 
            m_bDrngServoOn = false; 
            m_bDrngMove    = false; 
            m_bDrngReset   = false;
            m_bReqReset    = false;
        }
        //---------------------------------------------------------------------------
        private void WriteLog(string log)
        {
            string sTemp;

            sTemp = string.Format($"[MOTR:{m_nMotorNo}] ");
            fn_WriteLog(sTemp + log, EN_LOG_TYPE.ltSMC);
        }



    }
}
