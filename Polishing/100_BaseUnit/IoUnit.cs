using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACS.SPiiPlusNET;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserFile;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.Define.UserEnum;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Diagnostics;

namespace WaferPolishingSystem.BaseUnit
{
    public class IoUnit
    {
        //public ACSControl AcsCon;

        //Var.
        private int    m_iNumOfX;
        private int    m_iNumOfY;
        private bool   m_bReqReboot;
        private double dStart, dScan;
        private double m_dAvgValue;
        private double m_dTopLoadCelValue, m_dTopLoadCelValueOrg;
        bool           m_bSetTopLDCOffset ;
        bool           m_bDrngReboot      ;
        bool           m_bACSNetErr ;
        int            m_nRebootStep;

        //
        Object objVar = null;
        Array arInput = null;

        private int m_nNumOfIn  ;
        private int m_nNumOfOut ;

        int[] m_nIn  ;
        int[] m_nOut ;


        public bool   [] XV    = new bool   [MAX_INPUT_COUNT_D ];   //X Val.
        public bool   [] YV    = new bool   [MAX_OUTPUT_COUNT_D];   //Y Val.
                               
        public bool   [] YR    = new bool   [MAX_OUTPUT_COUNT_D];   //Y Return.
                               
        public int    [] XInv  = new int    [MAX_INPUT_COUNT_D ];   //입력 반전 Flag.
        public int    [] YInv  = new int    [MAX_OUTPUT_COUNT_D];   //출력 반전 Flag.
                               
        public int    [] XPart = new int    [MAX_INPUT_COUNT_D ];   //입력 Part.
        public int    [] YPart = new int    [MAX_OUTPUT_COUNT_D];   //출력 Part.
                               
        public string [] XName = new string [MAX_INPUT_COUNT_D ];
        public string [] YName = new string [MAX_OUTPUT_COUNT_D];
                               
        public string [] XA    = new string [MAX_INPUT_COUNT_D ];   //Address
        public string [] YA    = new string [MAX_OUTPUT_COUNT_D];
                      
        public int    [] XAdd  = new int    [MAX_INPUT_COUNT_D ];   //Address
        public int    [] YAdd  = new int    [MAX_OUTPUT_COUNT_D];
                      
        public int    [] AI    = new int    [MAX_INPUT_COUNT_A ];   //Analog Input
        public int    [] AO    = new int    [MAX_OUTPUT_COUNT_A];   //Analog Output

        public string [] AIName = new string[MAX_INPUT_COUNT_A ];
        public string [] AOName = new string[MAX_OUTPUT_COUNT_A];


        public double [] DATA_EQ_TO_ACS = new double[50];   //Write Var. to ACS
        public double [] DATA_ACS_TO_EQ = new double[50];   //Read Var. from ACS

        public int    [] DATA_EQ_TO_SMC = new int[90];   //Write EQ to SMC
        public int    [] DATA_SMC_TO_EQ = new int[90];   //Write SMC to ACS
        public int    [] REQ_EQ_TO_SMC  = new int[90];   //Write EQ to SMC
                      
        public int    [] EC_PLCIN       = new int[20];   //Safety PLC In Value
        public int    [] EC_PLCOUT      = new int[20];   //Safety PLC Out Value
                      
        public int    [] Inport         = new int[16];
        public int    [] Outport        = new int[16];
        public int    [] Inport_Ext     = new int[16];
        public int    [] Outport_Ext    = new int[16];
                      
        private bool m_bConnect     ;
        private bool m_bConnectAsSim;
        private bool m_bConnectErr  ;
        private bool m_bDrngLoad    ;
        private bool m_bInitOk      ;

        private int  m_nBuffCount   ;
        private int  m_nSelAxis     ; //Select Axis for Home 

        //
        private int  m_nACS_STATE   ;

        private TOnDelayTimer[] m_tDelayTimer = new TOnDelayTimer[15];

        //int      m_nCnt = 0;
        double[] m_dSum = new double[50];
        Queue<double> m_queSumLoadcell = new Queue<double>();
        Queue<double> m_queSumUTLevel  = new Queue<double>();
        Queue<double> m_QueDataTopLDC  = new Queue<double>();

        const int _LOADCELLMAXAVERAGECOUNT = 2;
        const int _UTLEVELMAXAVERAGECOUNT  = 30;


        //---------------------------------------------------------------------------
        private double m_dTopLoadCellOffSet;

        //---------------------------------------------------------------------------
        //Property
        public int _iNumOfX
        {
            get { return m_iNumOfX; }
        }
        public int _iNumOfY
        {
            get { return m_iNumOfY; }
        }
        
        public bool _bInitOk       { get { return m_bInitOk      ; } }
        public bool _bConnect      { get { return m_bConnect     ; } }
        public bool _bConnectAsSim { get { return m_bConnectAsSim; } }
        public bool _bConnectErr   { get { return m_bConnectErr  ; } }
        public bool _bDrngLoad     { get { return m_bDrngLoad    ; } }
        public bool _bACSNetErr    { get { return m_bACSNetErr   ; } }
        public bool _bDrngReboot   { get { return m_bDrngReboot  ; } }
        
        public int _nBuffCount     { get { return m_nBuffCount   ; } }
        public int _nSelAxis       { get { return m_nSelAxis     ; } set { m_nSelAxis = value; } }

        public double _dTopLoadCellOffSet { get { return m_dTopLoadCellOffSet; } }
        public double _dTopLoadCelValue   { get { return m_dTopLoadCelValue  ; } }
        
        public double _dUpdateScanTime   { get { return dScan;  } }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //ACS Api
        private Api ACS_IO;


        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public IoUnit()
        {

        	//
        	m_bConnect            = false;
            m_bDrngLoad           = false; 
        	m_bConnectErr         = true ;
            m_bConnectAsSim       = false;
            m_bInitOk             = false;
            m_bReqReboot          = false;

            m_nBuffCount          = -1   ;
            m_nSelAxis            = -1   ;

            m_dTopLoadCellOffSet  = 0.0  ;
            m_dAvgValue           = 0.0  ;
            m_dTopLoadCelValue    = 0.0  ;
            m_dTopLoadCelValueOrg = 0.0  ;


            m_iNumOfX = MAX_INPUT_COUNT_D ; 
            m_iNumOfY = MAX_OUTPUT_COUNT_D;

            m_bSetTopLDCOffset    = false;
            m_bACSNetErr          = false;

            //
            fn_Init();

            ACS_IO = new Api();

            //IO Read Buffer
            m_nNumOfIn  = MAX_INPUT_COUNT_D  / 8;
            m_nNumOfOut = MAX_OUTPUT_COUNT_D / 8;

            m_nIn  = new int[m_nNumOfIn ];
            m_nOut = new int[m_nNumOfOut];

            for (int n = 0; n < m_nNumOfIn; n++)
            {
                m_nIn[n] = new int();
            }

            for (int n = 0; n < m_nNumOfOut; n++)
            {
                m_nOut[n] = new int();
            }

            //
            for (int n = 0; n < 15; n++)
            {
                m_tDelayTimer[n] = new TOnDelayTimer();
                m_tDelayTimer[n].Clear();
            }

            //m_nCnt = 0;

            for (int n = 0; n < m_dSum.Length; n++)
            {
                m_dSum[n] = new double();
                m_dSum[n] = 0.0;
            }

            //
            for (int n = 0; n < DATA_EQ_TO_ACS.Length; n++)
            {
                DATA_EQ_TO_ACS[n] = new double();
                DATA_EQ_TO_ACS[n] = 0;  

                DATA_ACS_TO_EQ[n] = new double();
                DATA_ACS_TO_EQ[n] = 0;  

                DATA_EQ_TO_SMC[n] = new int();
                DATA_EQ_TO_SMC[n] = 0;

                DATA_SMC_TO_EQ[n] = new int();
                DATA_SMC_TO_EQ[n] = 0;

                REQ_EQ_TO_SMC [n] = new int();
                REQ_EQ_TO_SMC [n] = 0;
                

            }

            for (int n = 0; n < EC_PLCIN.Length; n++)
            {
                EC_PLCIN [n] = new int();
                EC_PLCIN [n] = 0;

                EC_PLCOUT[n] = new int();
                EC_PLCOUT[n] = 0;
            }

            m_bDrngReboot = false;
            m_nRebootStep = 0;
        }
        //---------------------------------------------------------------------------
        //초기화
        private void fn_Init()
        {

        	//Init IO Data.
        	for (int i = 0 ; i < MAX_INPUT_COUNT_D; i++)
        	{
        		XV   [i] = false;
        		XInv [i] = 0; 
                XA   [i] = string.Format($"X{i + 1:D4}");
        		XName[i] = string.Format($"X{i:D4}");
                XAdd [i] = 0;
            }

        	for (int i = 0; i < MAX_OUTPUT_COUNT_D; i++)
        	{
        		YV   [i] = false;
        		YInv [i] = 0;
                YA   [i] = string.Format($"Y{i + 1:D4}");
                YName[i] = string.Format($"Y{i:D4}");
                YAdd [i] = 0;
            }

            //Analog
            for (int i = 0; i < MAX_INPUT_COUNT_A; i++)
            {
                AIName[i] = string.Format($"AI{i:D4}");
            }

            for (int i = 0; i < MAX_OUTPUT_COUNT_A; i++)
            {
                AOName[i] = string.Format($"AO{i:D4}");

            }



            //JUNG/200416/Always On
            YV[(int)EN_OUTPUT_ID.yMC_PowerOn] = true;

            for (int i = 0; i < MAX_INPUT_COUNT_A; i++)
            {
                AI[i] = 0;
            }

        }
        //---------------------------------------------------------------------------
        public Api fn_GetAPI()
        {
            return ACS_IO;
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     ACS Connection
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/12/16  9:55
        */
        public bool fn_ACSConnect(int type = 0)
        {
            //Local Var
            m_bConnect      = false;
            m_bConnectAsSim = false;

            string sIp = "10.0.0.100";
            int    port = (int)EthernetCommOption.ACSC_SOCKET_STREAM_PORT;

            try
            {
                //
                if (type == ACS_CON_NOR)
                {
                    ACS_IO.OpenCommEthernetTCP(sIp, port);
                }
                else
                {
                    ACS_IO.OpenCommSimulator();
                    m_bConnectAsSim = true;
                }

                //
                m_bConnect = true;

                // Get Total Buffer Count
                //string sTemp = ACS_IO.Transaction("?SYSINFO(10)");
                //int nTotalBuffer = Convert.ToInt32(sTemp.Trim());
                m_nBuffCount = (int)ACS_IO.GetBuffersCount();


            }
            catch (System.Exception ex)
            {
                fn_WriteLog("[ACS] IO Open Fail!!! --> " + ex.Message);
                MessageBox.Show("ACS IO OPEN FAIL!!! \n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            if (m_bConnect)
            {
                if (m_bConnectAsSim) fn_WriteLog("[ACS] IO Open as Simulator");
                else                 fn_WriteLog("[ACS] IO Open");
            }

            return m_bConnect;

        }
        //---------------------------------------------------------------------------
        /**    
        @brief     ACS Disconnection
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/12/18  9:18
        */
        public bool fn_ACSDisConnect()
        {
            if (m_bConnectAsSim) ACS_IO.CloseSimulator();
            else                 ACS_IO.CloseComm     ();

            fn_WriteLog("ACS IO Close");
            return true;
        }
        //---------------------------------------------------------------------------
        void fn_SetInvX(int n, int Data)
        {
            XInv[n] = Data;
        }
        //---------------------------------------------------------------------------
        void fn_SetInvY(int n, int Data)
        {
            YInv[n] = Data;
        }
        //---------------------------------------------------------------------------
        public bool fn_IsDrngLoad() { return m_bDrngLoad; }
        //---------------------------------------------------------------------------
        /**    
        @brief     Update(void)
        @return    
        @param     
        @remark    IO Data 갱신
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/4  19:10
        */

        public void fn_Update()
        {
            try
            {
                dStart = TICK._GetTickTime();

                if (m_bDrngReboot) return; //JUNG/200915

                //
                if (m_bConnect)
                {
                    //
                    fn_UpdateIO      (); // IO From ACS
                    fn_UpdateData    (); // Read, Write Data with ACS
                    //fn_CheckACSReboot();

                    //Manual Sequence
                    SEQ_POLIS.fn_SeqDrain   ();
                    SEQ_POLIS.fn_SeqSuckBack();
                    SEQ_CLEAN.fn_SeqDrain   ();

                    fn_GetUTLevelValue();
                }

                fn_UpdateInput ();
                //fn_UpdateOutput();

                //Check Off Time
                fn_CheckOutputTime();

                dScan = TICK._GetTickTime() - dStart;

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[IO.fn_Update()]" + ex.Message);
            }

        }
        //---------------------------------------------------------------------------
        private void fn_UpdateIO()
        {

            if (!m_bConnect) return;

            try
            {
                //Digital Read
                objVar  = ACS_IO.ReadVariable("EC_IN");
                arInput = objVar as Array;
                Array.Copy(arInput, m_nIn, m_nIn.Length);

                //Analog Read
                objVar  = ACS_IO.ReadVariable("EC_AIN");
                arInput = objVar as Array;
                Array.Copy(arInput, AI, AI.Length);

                //Digital Write
                int MaskBit = 0x01;
                int nValue  = 0;
                Array.Clear(m_nOut, 0, m_nOut.Length);

                YV[(int)EN_OUTPUT_ID.yMC_PowerOn] = true; //JUNG/Y000 - On

                for (int n = 0; n < m_nOut.Length; n++)
                {
                    MaskBit = 0x01;
                    for (int x = 0; x < 8; x++)
                    {
                        //if (n == 0 && x == 0) nValue = MaskBit; //JUNG/Y000 - On
                        //else                  nValue = YV[(n * 8) + x] ? MaskBit : 0;
                        nValue = YV[(n * 8) + x] ? MaskBit : 0;

                        m_nOut[n] += nValue;

                        MaskBit = (0x01) << (x + 1);

                        //Return 
                        YR[(n * 8) + x] = YV[(n * 8) + x];
                        //Inverse Check
                        if (YInv[(n * 8) + x] == 1) YR[(n * 8) + x] = !YR[(n * 8) + x];

                    }
                }

                ACS_IO.WriteVariable(m_nOut, "EC_OUT");


                //Analog Write
                ACS_IO.WriteVariable(AO, "EC_AOUT");


                //ACS Network
                fn_GetECError();

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Digital Input Data Update
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/5  10:11
        */
        private void fn_UpdateInput()
        {
            //
            int MaskBit = 0x01;
            for (int n = 0; n < m_nIn.Length; n++)
            {
                MaskBit = 0x01;
                for (int x = 0; x < 8; x++)
                {
                    XV[(n * 8) + x] = (m_nIn[n] & MaskBit) != 0 ? true : false;

                    //Inverse Check
                    if (XInv[(n * 8) + x] == 1) XV[(n * 8) + x] = !XV[(n * 8) + x];

                    MaskBit = (0x01) << (x + 1);
                }
            }
        }
        //---------------------------------------------------------------------------
        public void fn_DataEqToAcs()
        {
            //Write Data
            ACS_IO.WriteVariable(DATA_EQ_TO_ACS, "DATA_EQ_TO_ACS");
        }
        //---------------------------------------------------------------------------
        private void fn_UpdateData()
        {
            //Local Var
            object objVar = null;

            //Write Data
            ACS_IO.WriteVariable(DATA_EQ_TO_ACS, "DATA_EQ_TO_ACS");

            //Read Data
            objVar = ACS_IO.ReadVariable("DATA_ACS_TO_EQ");
            Array arInput = objVar as Array;
            Array.Copy(arInput, DATA_ACS_TO_EQ, DATA_ACS_TO_EQ.Length);


            //SMC Data
            ACS_IO.WriteVariable(DATA_EQ_TO_SMC, "DATA_EQ_TO_SMC");

            objVar = ACS_IO.ReadVariable("DATA_SMC_TO_EQ");
            arInput = objVar as Array;
            Array.Copy(arInput, DATA_SMC_TO_EQ, DATA_SMC_TO_EQ.Length);


            //PLC Data
            objVar = ACS_IO.ReadVariable("EC_PLCIN");
            arInput = objVar as Array;
            Array.Copy(arInput, EC_PLCIN, EC_PLCIN.Length);

            //JUNG/200713/미사용으로 삭제
            //objVar = ACS_IO.ReadVariable("EC_PLCOUT");
            //arInput = objVar as Array;
            //Array.Copy(arInput, EC_PLCOUT, EC_PLCOUT.Length);

        }
        //---------------------------------------------------------------------------

        private void fn_CheckACSReboot()
        {
            //The method is used to retrieve the last EtherCAT error code.
            m_nACS_STATE = ACS_IO.GetEtherCATError();

            /*
            6000 General EtherCAT Error
            6001 EtherCAT cable not connected
            6002 EtherCAT master is in incorrect state
            6003 Not all EtherCAT frames can be processed
            6004 EtherCAT Slave Error
            6005 EtherCAT initialization failure
            6007 EtherCAT work count error
            6008 Not all EtherCAT slaves are in the OP State
            6009 EtherCAT protocol timeout
            6010 Slave initialization failed
            6011 Bus configuration mismatch
            6012 CoE Emergency
            6013 EtherCAT Slave won't enter INIT state
            6014 EtherCAT ring topology requires network reconfiguration
            6015 One or more EtherCAT cables are not connected
            6018 EtherCAT Master won't enter PREOP state
            6019 EtherCAT Master won't enter SAFEOP state
            6020 EtherCAT Master won't enter OP state
            */

            //상태에 따라 Reboot
            switch (m_nACS_STATE)
            {
                case 6004: //EtherCAT Slave Error
                case 6007: //
                    m_bReqReboot = true;
                    break;

                default:
                    break;
            }

        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	The method retrieves the explanation of an error code.
        </summary>
        <param name="ErrCode"> Error Code </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/04/07 11:47
        */
        public string fn_GetErrorString(int ErrCode)
        {
            return ACS_IO.GetErrorString(ErrCode);
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Digital Output Data Update
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/5  10:11
        */
        private void fn_UpdateOutput()
        {
            //Port 번호에 따라 변경 필요....

            for (int i = 0; i < MAX_OUTPUT_COUNT_D; i++)
            {
                YR[i] = YV[i];

                //Inverse Check
                if (YInv[i] == 1) YR[i] = !YR[i];

            }

        }
        //---------------------------------------------------------------------------
        public bool fn_Z2Unlock(bool on)
        {
            YV[(int)EN_OUTPUT_ID.yMOTR_Z2_Unlock] = on;
            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_TRZUnlock(bool on)
        {
            YV[(int)EN_OUTPUT_ID.yMOTR_TRZ_Unlock] = on;
            return true;
        }

        //---------------------------------------------------------------------------
        public void fn_CheckOutputTime()
        {
            //Spindle Reset
            m_tDelayTimer[0].OnDelay(IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_Reset], 3000);
            if (m_tDelayTimer[0].Out) IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_Reset] = false;

            //SMC Reset Clear
            m_tDelayTimer[1].OnDelay(DATA_EQ_TO_SMC[     (int)EN_SMC_WRITE.FAULT_RESET] == 1, 1000); if (m_tDelayTimer[1].Out) DATA_EQ_TO_SMC[     (int)EN_SMC_WRITE.FAULT_RESET] = 0;
            m_tDelayTimer[2].OnDelay(DATA_EQ_TO_SMC[30 + (int)EN_SMC_WRITE.FAULT_RESET] == 1, 1000); if (m_tDelayTimer[2].Out) DATA_EQ_TO_SMC[30 + (int)EN_SMC_WRITE.FAULT_RESET] = 0;
            m_tDelayTimer[3].OnDelay(DATA_EQ_TO_SMC[60 + (int)EN_SMC_WRITE.FAULT_RESET] == 1, 1000); if (m_tDelayTimer[3].Out) DATA_EQ_TO_SMC[60 + (int)EN_SMC_WRITE.FAULT_RESET] = 0;

            if (!SEQ._bRun)
            {
                //JUNG/200602/Valve On Time Check
                m_tDelayTimer[5 ].OnDelay(YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ], 7000); if (m_tDelayTimer[5 ].Out) SEQ_POLIS.fn_StopUtil(EN_UTIL_KIND.Silica01); //YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] = false;
                m_tDelayTimer[6 ].OnDelay(YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ], 7000); if (m_tDelayTimer[6 ].Out) SEQ_POLIS.fn_StopUtil(EN_UTIL_KIND.Silica02); //YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ] = false;
                m_tDelayTimer[7 ].OnDelay(YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ], 7000); if (m_tDelayTimer[7 ].Out) SEQ_POLIS.fn_StopUtil(EN_UTIL_KIND.Silica03); //YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ] = false;
                m_tDelayTimer[8 ].OnDelay(YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater], 3000); if (m_tDelayTimer[8 ].Out) SEQ_POLIS.fn_SetDIWaterValve(false);          //YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = false;
                m_tDelayTimer[9 ].OnDelay(YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater], 3000); if (m_tDelayTimer[9 ].Out) SEQ_CLEAN.fn_SetDIWaterValve(false);          //YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater] = false;
                m_tDelayTimer[10].OnDelay(YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap   ], 7000); if (m_tDelayTimer[10].Out) SEQ_POLIS.fn_StopUtil(EN_UTIL_KIND.Soap);
            }
            else
            {
                m_tDelayTimer[5].Clear();
                m_tDelayTimer[6].Clear();
                m_tDelayTimer[7].Clear();
                m_tDelayTimer[8].Clear();
                m_tDelayTimer[9].Clear();
                m_tDelayTimer[10].Clear();
            }

        }
        //---------------------------------------------------------------------------
        /**
        @brief     IO Data Load
        @return
        @param
        @remark
        -
        @author    정지완(JUNGJIWAN)
        @date      2019/9/6  9:17
        */
        public void fn_LoadIO(bool bLoad)
        {
            //Local Var.
            string sIniPath, sTemp;

            //Input
            try
            {
                sIniPath = fn_GetExePath() + "SYSTEM\\IO_Input.ini";

                if (!fn_CheckFileExist(sIniPath) && bLoad) return;

                m_bDrngLoad = true;

                if (bLoad)
                {
                    //Digital
                    for (int i = 0; i < MAX_INPUT_COUNT_D; i++)
                    {
                        sTemp      = string.Format($"INPUT({i + 1:D4})");
                        XName  [i] = UserINI.fn_Load("INPUT NAME", sTemp, XName[i], sIniPath); 
        	    		XInv   [i] = UserINI.fn_Load("INPUT INVS", sTemp, 0       , sIniPath); 
        	    		XA     [i] = UserINI.fn_Load("INPUT ADDR", sTemp, XA[i]   , sIniPath);
                        
                        if(XA[i].Length > 1) XAdd[i] = Convert.ToInt32(XA[i].Substring(1, XA[i].Length-1));
        	    	}
                    
                    //Analog
                    for (int i = 0; i < MAX_INPUT_COUNT_A; i++)
                    {
                        sTemp     = string.Format($"INPUT_A({i + 1:D4})");
                        AIName[i] = UserINI.fn_Load("INPUT NAME ANALOG", sTemp, AIName[i], sIniPath); 
                    }
                }
                else
                {
                    //Digital
                    for (int i = 0; i < MAX_INPUT_COUNT_D; i++)
                    {
                        sTemp = string.Format($"INPUT({i + 1:D4})");
                        UserINI.fn_Save("INPUT NAME", sTemp, XName  [i], sIniPath); 
        	    		UserINI.fn_Save("INPUT INVS", sTemp, XInv   [i], sIniPath); 
        	    		UserINI.fn_Save("INPUT ADDR", sTemp, XA     [i], sIniPath);

                        if (XA[i].Length > 1)
                        {
                            XAdd[i] = Convert.ToInt32(XA[i].Substring(1, XA[i].Length - 1));
                        }
                    }

                    //Analog
                    for (int i = 0; i < MAX_INPUT_COUNT_A; i++)
                    {
                        sTemp = string.Format($"INPUT_A({i + 1:D4})");
                        UserINI.fn_Save("INPUT NAME ANALOG", sTemp, AIName[i], sIniPath);
                    }
                }

                m_bDrngLoad = false;

                //Output
                sIniPath = fn_GetExePath() + "SYSTEM\\IO_Output.ini";

                if (!fn_CheckFileExist(sIniPath) && bLoad) return;

                m_bDrngLoad = true;

                if (bLoad)
                {
                    for (int i = 0; i < MAX_OUTPUT_COUNT_D; i++)
                    {
                        sTemp = string.Format($"OUTPUT({i + 1:D4})");
                        YName  [i] = UserINI.fn_Load("OUTPUT NAME", sTemp, YName[i], sIniPath); 
        	    		YInv   [i] = UserINI.fn_Load("OUTPUT INVS", sTemp, 0       , sIniPath); 
        	    		YA     [i] = UserINI.fn_Load("OUTPUT ADDR", sTemp, YA[i]   , sIniPath);
                        if (YA[i].Length > 1)
                        {
                            YAdd[i] = Convert.ToInt32(YA[i].Substring(1, YA[i].Length - 1));
                        }
                    }

                    //Analog
                    for (int i = 0; i < MAX_OUTPUT_COUNT_A; i++)
                    {
                        sTemp = string.Format($"OUTPUT_A({i + 1:D4})");
                        AOName[i] = UserINI.fn_Load("OUTPUT NAME ANALOG", sTemp, AOName[i], sIniPath);
                    }

                }
                else
                {
                    for (int i = 0; i < MAX_OUTPUT_COUNT_D; i++)
                    {
                        sTemp = string.Format($"OUTPUT({i + 1:D4})");
                        UserINI.fn_Save("OUTPUT NAME", sTemp, YName  [i], sIniPath); 
        	    		UserINI.fn_Save("OUTPUT INVS", sTemp, YInv   [i], sIniPath); 
        	    		UserINI.fn_Save("OUTPUT ADDR", sTemp, YA     [i], sIniPath);
                        if (YA[i].Length > 1)
                        {
                            YAdd[i] = Convert.ToInt32(YA[i].Substring(1, YA[i].Length - 1));
                        }
                    }

                    //Analog
                    for (int i = 0; i < MAX_OUTPUT_COUNT_A; i++)
                    {
                        sTemp = string.Format($"OUTPUT_A({i + 1:D4})");
                        UserINI.fn_Save("OUTPUT NAME ANALOG", sTemp, AOName[i], sIniPath);
                    }

                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            m_bDrngLoad = false;
        }
        //---------------------------------------------------------------------------

        public int fn_GetUTLevelValue()
        {

            //double MAX_OUT   = 1024.0; //AD 0~20V 출력 0~4095 // 250ohm 저항 장착시 1024가 최대 출력값 현재 235ohm 장착
            //double LEVEL_REF = 235.0 ; //연결된 저항값과 같음
            //double rtnvalue  = 0.0   ;
            //
            //double dInValue = AI[(int)EN_AINPUT_ID.aiPOL_UtilLevel] / MAX_OUT;


            //풀 스케일(0에서 검출범위 상한까지)에 대해서 4~20mA를 출력합니다 --> 1~5V로 변경
            //FW-H02 : 50~200mm
            //Output: 1~5 V
            double dRtn = 0.0;
            double dCal = 0.0;

            int    nInput    = AI[(int)EN_AINPUT_ID.aiPOL_UtilLevel];
            int    nMaxValue = MAX_VALUE_5V      ;
            int    nMinValue = MAX_VALUE_10V / 10;
            double dOffset   = FM.m_stMasterOpt.dUtilOffset ; //
            double dMinValue = 0  ;
            double dMaxValue = 200;


            if (nInput < nMinValue) return (int)dRtn;
            if (nInput > nMaxValue) return (int)dMaxValue;

            dCal = ((nInput - nMinValue) / (double)(nMaxValue - nMinValue)) * (dMaxValue - dMinValue) + dMinValue;
            dCal = Math.Round(dCal, 1) + dOffset;

            //Average
            //if (m_nCnt >= m_dSum.Length)
            //{
            //    dAvgValue = Enumerable.Average(m_dSum);
            //    m_nCnt = 0;
            //}
            //if (0 < dCal && dCal < 300) m_dSum[m_nCnt++] = dCal; //LJH/200422 : Hunting Data 처리

            m_queSumUTLevel.Enqueue(dCal);
            if (m_queSumUTLevel.Count >= _UTLEVELMAXAVERAGECOUNT)
            {
                m_dAvgValue = Enumerable.Average(m_queSumUTLevel);
                m_queSumUTLevel.Dequeue();
            }
            else m_dAvgValue = dMaxValue;

            return (int)m_dAvgValue;
        }
        //-------------------------------------------------------------------------------------------------
        public double fn_GetUTAvgValue()
        {
            return Math.Round(m_dAvgValue, 1);
        }
        //---------------------------------------------------------------------------
        public bool fn_IsUTLevelDone()
        {
            double dUserSet = FM.m_stSystemOpt.nWaterLvlPol; //145
            double dOffset  = 1;
            double dValue   =  fn_GetUTAvgValue(); //fn_GetUTLevelValue();

            //Console.WriteLine("fn_GetUTLevelValue : " + dValue);

            if (dValue < dUserSet + dOffset) return true;

            return false;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_ClearUTQue()
        {
            m_queSumUTLevel.Clear();
        }
        public int fn_GetUTQueQty()
        {
            return m_queSumUTLevel.Count;
        }


        /************************************************************************/
        /* FLOW METER                                                           */
        /************************************************************************/
        public double fn_GetFlowMeter(EN_AINPUT_ID idx)
        {
            //순간 유량 : 0.4 ~4.0 L / Min
            //Output: 1~5 V

            double dRtn = 0.0;

            int    nIdx      = (int)idx;
            int    nInput    = AI[nIdx];
            int    nMaxValue = MAX_VALUE_5V;
            int    nMinValue = MAX_VALUE_10V / 10;
            double dMinValue = 0.4;
            double dMaxValue = 4.0;


            if (nInput < nMinValue) return dRtn;
            if (nInput > nMaxValue) return dMaxValue;


            dRtn = ((nInput - nMinValue) / (double)(nMaxValue - nMinValue)) * (dMaxValue - dMinValue) + dMinValue;

            return dRtn;
        }

        /************************************************************************/
        /* Top Load Cell                                                        */
        /************************************************************************/
        //---------------------------------------------------------------------------
        public void fn_SetTopOffset()
        {
            //m_dTopLoadCellOffSet = m_dTopLoadCelValueOrg; // fn_GetTopLoadCell(); 
            m_bSetTopLDCOffset = true;
            //fn_WriteLog(string.Format($"[LOADCELL] Top Load cell Offset Save : {m_dTopLoadCellOffSet}"));

            //
            Console.WriteLine(string.Format($"[LOADCELL] Top Load cell Offset Save : {m_dTopLoadCellOffSet}"));
        }
        //---------------------------------------------------------------------------
        public double fn_GetTopLoadCell(bool bNT = false)
        {
            //Default = g / bNT = N
            double ONEGRAM_TO_NEWTON = 0.00980665;

            double dRtn = bNT ? Math.Round(m_dTopLoadCelValue * ONEGRAM_TO_NEWTON, 2) : Math.Round(m_dTopLoadCelValue, 4); //Original Value

            return dRtn;
        }
        //---------------------------------------------------------------------------
        public double fn_GetTopLoadCellAsBTM(bool bNT = false)
        {
            //Default = g / bNT = N
            double ONEGRAM_TO_NEWTON = 0.00980665;
            double dIntercept        = FM.m_stMasterOpt.dYInterceptBT;
            double dYSlope           = FM.m_stMasterOpt.dYSlopeBT;
            double dValue            = (m_dTopLoadCelValue * dYSlope) + dIntercept;
            
            if (dValue < 0) dValue = 0;

            //Cal. Value
            double dRtn = bNT ? Math.Round(dValue * ONEGRAM_TO_NEWTON, 2) : Math.Round(dValue, 4);

            return dRtn;
        }
        //---------------------------------------------------------------------------
        public double fn_GetTopLoadCellOrg()
        {
            return m_dTopLoadCelValueOrg;
        }
        //---------------------------------------------------------------------------
        public double fn_GetTopLoadCellValue()
        {
            //Model : LSB201 25lb
            //Output: 0~10 V
            //0 lb ~ 25 lb : 0g ~ 11,339.8093g
            //1 lb = 453.59237g; 
            double dLb  = 0.0;
            double dRtn = 0.0;

            int    nIdx      = (int)EN_AINPUT_ID.aiSPD_LoadCell;
            int    nInput    = AI[nIdx];
            int    nMaxValue = MAX_VALUE_10V;
            int    nMinValue = 0 ;
            double dMinValue = 0 ;
            double dMaxValue = 25;


            if (nInput < nMinValue ) return dRtn     ;
            if (nInput > nMaxValue ) return dMaxValue;

            dLb = ((nInput - nMinValue) / (double)(nMaxValue - nMinValue)) * (dMaxValue - dMinValue) + dMinValue;

            dRtn = (dLb * 453.59237);

            //Return -> g
            return Math.Round(dRtn, 4);

        }

        //---------------------------------------------------------------------------

        public double fn_UpdateTopLDC()
        {
            //20ms 마다 Data Average
            double dTopValue = fn_GetTopLoadCellValue();
            int    nAvgCount = 50;

            m_QueDataTopLDC.Enqueue(dTopValue);
            if (m_QueDataTopLDC.Count >= nAvgCount)
            {
                int count    = 0;
                var DataList = new List<double>();
                DataList.AddRange(m_QueDataTopLDC);
                DataList.Sort();

                dTopValue = 0;
                for (int i = 0; i < DataList.Count; i++)
                {
                    if (i < 3                  ) continue;
                    if (i >= DataList.Count - 3) continue;

                    dTopValue += DataList[i];
                    count++;
                }

                dTopValue = dTopValue / (double)count;

                if (m_bSetTopLDCOffset) //JUNG/200901
                {
                    m_bSetTopLDCOffset = false;
                    m_dTopLoadCellOffSet = dTopValue;

                    fn_SetTopLoadCellOffset(m_dTopLoadCellOffSet);

                    fn_WriteLog(string.Format($"[LOADCELL] Top Load cell Offset Save : {m_dTopLoadCellOffSet}"));

                }

                m_dTopLoadCelValueOrg = Math.Round(dTopValue, 4);
                m_dTopLoadCelValue = m_dTopLoadCellOffSet - Math.Round(dTopValue, 4); //JUNG/200819
                if (m_dTopLoadCelValue < 0) m_dTopLoadCelValue = 0.0;

                m_QueDataTopLDC.Clear();
            }

            return dTopValue;


        }
        //---------------------------------------------------------------------------
        public bool fn_Reset()
        {
            //
            YV[(int)EN_OUTPUT_ID.yMC_PowerOn] = true;

            //SMC Home Clear
            fn_SMCHomeClear();

            //SMC JOG Clear
            fn_SMCJOGClear();

            //fn_ACSReboot();

            return true;

        }
        //---------------------------------------------------------------------------
        public void fn_ACSReboot(bool bDo = false)
        {
            try
            {
                if (m_bReqReboot || bDo)
                {
                    m_nACS_STATE = 0;
                    m_bReqReboot = false;

                    fn_WriteLog("[ACS] Controller Reboot...");

                    //This method reboots controller and waits for process completion.
                    ACS_IO.ControllerReboot(30000);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[IO Unit] Reboot error : " + ex.ToString());
            }

        }
        //---------------------------------------------------------------------------
        public void fn_SetACSReboot()
        {
            m_bReqReboot = true;
        }
        //---------------------------------------------------------------------------
        public void fn_UpdateACSReboot()
        {
            //
            if (m_bReqReboot)
            {
                m_tDelayTimer[14].Clear();

                m_bDrngReboot = true; 
                m_bReqReboot  = false;
                
                m_nRebootStep = 10;
                return;
            }

            //
            try
            {
                if (m_nRebootStep < 0) m_nRebootStep = 0;

                switch (m_nRebootStep)
                {
                    case 10:

                        m_bDrngReboot = true;

                        fn_WriteLog("[ACS] Controller Reboot...");

                        m_nRebootStep++;
                        return;

                    case 11:

                        //This method reboots controller and waits for process completion.
                        ACS_IO.ControllerReboot(30000);

                        m_tDelayTimer[14].Clear();
                        m_nRebootStep++;
                        return;

                    case 12:
                        if (!m_tDelayTimer[14].OnDelay(true, 3000)) return;

                        fn_WriteLog("[ACS] Controller Reboot OK");

                        m_bDrngReboot = false;
                        m_nRebootStep = 0;
                        return;

                    default:
                        m_bReqReboot = false;
                        m_nRebootStep = 0;
                        m_bDrngReboot = false;
                        m_tDelayTimer[14].Clear();
                        break;
                }
            }
            catch (System.Exception ex)
            {
                m_bReqReboot  = false;
                m_nRebootStep = 0;
                m_bDrngReboot = false;

                Console.WriteLine("[IO Unit] ACS Reboot error : " + ex.ToString());
                fn_WriteLog("[IO Unit] ACS Reboot error : " + ex.ToString());
            }

        }

        /************************************************************************/
        /* ACS Buffer                                                           */
        /************************************************************************/
        //---------------------------------------------------------------------------
        public bool fn_IsBuffRun(int buff)
        {
            int m_nProgState = (int)ACS_IO.GetProgramState((ProgramBuffer)buff);

            bool rtn = (m_nProgState == (int)ProgramStates.ACSC_PST_RUN) || m_nProgState == 3;

            return rtn;
        }
        //---------------------------------------------------------------------------
        private bool fn_SetSoftwareLimit(int nAxis, bool bDir, bool bUse, double dValue)
        {
            try
            {
                if (bUse)
                {
                    if (bDir)
                    {
                        ACS_IO.WriteVariable(dValue, "SRLIMIT", ProgramBuffer.ACSC_NONE, nAxis, nAxis, -1, -1);
                        ACS_IO.EnableFault((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SRL);
                    }
                    else
                    {
                        ACS_IO.WriteVariable(dValue, "SLLIMIT", ProgramBuffer.ACSC_NONE, nAxis, nAxis, -1, -1);
                        ACS_IO.EnableFault((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SLL);
                    }
                }
                else
                {
                    if (bDir) ACS_IO.DisableFault((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SRL);
                    else      ACS_IO.DisableFault((Axis)nAxis, SafetyControlMasks.ACSC_SAFETY_SLL);
                }


                return true;
            }
            catch (ACSException ex)
            {
                Console.WriteLine("fn_SetSoftwareLimit : " + ex.Message);
                return false;
            }
        }

        //---------------------------------------------------------------------------
        public bool fn_IsForceBuffRun(bool test = false)
        {
            bool rtn       = false;
            bool IsTestRun = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE) || test;
            
            if(IsTestRun)
            {
                rtn = fn_IsBuffRun(BFNo_12_FORCE_TEST);
            }
            else
            {
                rtn = fn_IsBuffRun(BFNo_14_FORCECHECK);
            }


            return rtn;
        }

        //---------------------------------------------------------------------------
        public bool fn_SMCHome(EN_MOTR_ID id, bool bon = false)
        {
            int nIndex = ((int)id - (int)EN_MOTR_ID.miSPD_Z1) * 30;

            if (bon)
            {
                //REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.SERVO_ON   ] = 1; //Servo ON //JUNG/200715/삭제
                REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.FAULT_RESET] = 1; //Fault Reset
                REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME       ] = 1; //Home
                REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.TARGET_VEL ] = (int)MOTR[(int)id].MP.dVel[(int)EN_MOTR_VEL.Home]; //Vel. 
            }
            else
            {
                //DATA_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME] = 0; //Home
                REQ_EQ_TO_SMC[nIndex + (int)EN_SMC_WRITE.HOME] = 0; //Home
            }

            return true;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SMCHomeClear()
        {
            for (EN_MOTR_ID m = EN_MOTR_ID.miSPD_Z1; m <= EN_MOTR_ID.miTRF_Z; m++)
            {
                fn_SMCHome(m, false);
            }

        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SMCJOGClear()
        {
            for (EN_MOTR_ID m = EN_MOTR_ID.miSPD_Z1; m <= EN_MOTR_ID.miTRF_Z; m++)
            {
                fn_SetSMCData(m, EN_SMC_WRITE.JOG_RIGHT, 0);
                fn_SetSMCData(m, EN_SMC_WRITE.JOG_LEFT , 0);
            }
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SMCAlarmClear()
        {
            for (EN_MOTR_ID m = EN_MOTR_ID.miSPD_Z1; m <= EN_MOTR_ID.miTRF_Z; m++)
            {
                fn_SetSMCData(m, EN_SMC_WRITE.FAULT_RESET, 1);
            }
        }

        //---------------------------------------------------------------------------
        public bool fn_RunBuffer(int nBuffNo, bool bOn = false)
        {
            if (!m_bConnect) return false;

            try
            {
                if (bOn)
                    ACS_IO.RunBuffer((ProgramBuffer)nBuffNo, null);
                else
                    ACS_IO.StopBuffer((ProgramBuffer)nBuffNo);

                return true;

            }
            catch (ACSException)
            {
                return false;
            }
        }
        //---------------------------------------------------------------------------
        public bool fn_StopAllBuffer()
        {
            for (int i = 0; i <= MAX_ACSBUFF; i++)
            {
                if (i == BFNo_09_XSEG_COUNT) continue;
                fn_RunBuffer(i, false);
            }

            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_StopAllHomeBuffer()
        {
            for (int i = 0; i <= MAX_HOMEBUFF; i++)
            {
                if (fn_IsBuffRun(i)) fn_RunBuffer(i, false);
            }

            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_StopBuffer(int buff)
        {
            if (!m_bConnect) return false;

            return fn_RunBuffer(buff, false);
        }
        //---------------------------------------------------------------------------
        public bool fn_SetSoftLimit(int Axis, bool set = false, bool blog = true)
        {
            double dLimitPos = 0.0;
            double dOffset   = FM.m_stSystemOpt.dSoftLimitOffset;
            double dEncPos   = MOTR[Axis].GetEncPos();

            if(Axis == (int)EN_MOTR_ID.miSPD_Z) dLimitPos = dEncPos + dOffset; 
            else                                dLimitPos = dEncPos          ;

            if (blog)
            {
                string sOn  = set ? "ON" : "OFF";
                string slog = string.Format($"[SOFT LIMIT][{STR_MOTOR_ID[Axis]}] Set - {sOn} / Limit Pos - {dLimitPos} / Enc Pos - {dEncPos} / Offset - {dOffset}");
                fn_WriteLog(slog);
                fn_WriteLog(slog, EN_LOG_TYPE.ltLot);
                Console.WriteLine(slog);
            }

            return fn_SetSoftwareLimit(Axis, drPOS, set, dLimitPos);
        }

        //---------------------------------------------------------------------------
        public bool fn_SetSoftLimit_Z(bool set, double pos = 1)
        {
            int    Axis      = (int)EN_MOTR_ID.miSPD_Z;
            double dLimitPos = pos;
            //double dEncPos   = MOTR[Axis].GetEncPos();
            //double dOffset   = pos;

            //if(set)
            //{
            //    dLimitPos = dEncPos + dOffset;
            //}

            return fn_SetSoftwareLimit(Axis, drPOS, set, dLimitPos);
        }
        //---------------------------------------------------------------------------
        public bool fn_SetForceBuffData(double landpos) //Force Buffer Start
        {
            double dSetRatio = SEQ_SPIND._dForceRatio       ;
            bool   IsTestRun = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);
            int    nBuffNo   = IsTestRun? BFNo_12_FORCE_TEST : BFNo_14_FORCECHECK;

            fn_WriteLog(string.Format($"[ForceBufferRun] Set Ratio_{dSetRatio:F2}"), EN_LOG_TYPE.ltEvent, EN_SEQ_ID.SPINDLE);
            fn_WriteLog(string.Format($"[ForceBufferRun] SOFT_START_POS : {landpos:F2}"), EN_LOG_TYPE.ltEvent, EN_SEQ_ID.SPINDLE);

            ACS_IO.WriteVariable(dSetRatio, "DynamicCurrentRatio"); //DCOM Value
            ACS_IO.WriteVariable(landpos  , "SOFT_START_POS"     );
            ACS_IO.WriteVariable(1        , "Force_Flag"         );
            ACS_IO.WriteVariable(0        , "Force_END"          );

            //
            Console.WriteLine("SEQ_SPIND._dForceRatio : " + SEQ_SPIND._dForceRatio);

            fn_WriteLog(string.Format($"[BUFF RUN] SET DCOM : {dSetRatio:F2}"), EN_LOG_TYPE.ltLot);

            return true;
        }


        //---------------------------------------------------------------------------
        public bool fn_ForceBufferRun(double landpos) //Force Buffer Start
        {
            double dSetRatio = SEQ_SPIND._dForceRatio ;
            bool   IsTestRun = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);
            int    nBuffNo   = IsTestRun? BFNo_12_FORCE_TEST : BFNo_14_FORCECHECK;

            ACS_IO.WriteVariable(dSetRatio, "DynamicCurrentRatio"); //DCOM Value
            ACS_IO.WriteVariable(landpos  , "SOFT_START_POS"     );
            ACS_IO.WriteVariable(1        , "Force_Flag"         );
            ACS_IO.WriteVariable(0        , "Force_END"          );

            //ACS Buffer Run
            if (!fn_RunBuffer(nBuffNo, true))
            {
                //fn_UserMsg(string.Format($"ACS Buffer[{nBuffNo:D2}] Run Error!!!!"));

                return false;
            }
            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_ForceBufferRunforCal(double landpos, double DCOM)
        {
            double dSetRatio = DCOM;
            int    nBuffNo   = BFNo_14_FORCECHECK;

            ACS_IO.WriteVariable(dSetRatio, "DynamicCurrentRatio"); //DCOM Value
            ACS_IO.WriteVariable(landpos  , "SOFT_START_POS"     );
            ACS_IO.WriteVariable(1        , "Force_Flag"         );
            ACS_IO.WriteVariable(0        , "Force_END"          );

            //ACS Buffer Run
            if (!fn_RunBuffer(nBuffNo, true))
            {
                //fn_UserMsg(string.Format($"ACS Buffer[{nBuffNo:D2}] Run Error!!!!"));

                return false;
            }
            return true;
        }

        //---------------------------------------------------------------------------
        public void fn_ResetDCOMValue()
        {
            double dSetRatio = (SEQ_SPIND._dForceRatio * FM.m_stMasterOpt.dDCOMRatio) / 100.0; // 3.0;

            string sCmd = string.Format($"DCOM(2)= {dSetRatio}");

            try
            {
                ACS_IO.Command(sCmd);

                fn_WriteLog(string.Format($"[END] SET DCOM : {dSetRatio:F2} [{FM.m_stMasterOpt.dDCOMRatio}%] / CYCLE : {SEQ_SPIND.fn_GetCurrMillCnt()}"), EN_LOG_TYPE.ltLot);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
        //---------------------------------------------------------------------------
        public void fn_SetDCOM(double set)
        {
            string sCmd = string.Format($"DCOM(2)= {set}");

            try
            {
                ACS_IO.Command(sCmd);

                fn_WriteLog(string.Format($"SET DCOM : {set:F2}"));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        //---------------------------------------------------------------------------
        public bool fn_ForceBuffTest(double Target)
        {
            double dSetRatio  = 0.0;
            double dOffset    = 0.36;

            dSetRatio = (Target + 139) / 123.8235 + dOffset;

            try
            {
                ACS_IO.WriteVariable(dSetRatio, "TARGET_FORCE");

                //ACS Buffer Run
                fn_RunBuffer(11, true);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return true;
        }

        //-------------------------------------------------------------------------------------------------
        public void fn_ForceBufferStop() //Force Buffer Start
        {
            //bool IsTestRun = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);
            //int nBuffNo = IsTestRun ? BFNo_12_FORCE_TEST : BFNo_14_FORCECHECK;

            //fn_RunBuffer(nBuffNo, false);
            try
            {
                ACS_IO.WriteVariable(0, "Force_Flag");
                ACS_IO.WriteVariable(1, "Force_END"); //JUNG/200602
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_ClearDuringFlag() //During Flag Off
        {
            try
            {
                ACS_IO.WriteVariable(0, "Drng_Flag");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        //---------------------------------------------------------------------------
        public bool fn_SetCurrentRatio(double Forcetratio)
        {
            //Set Force Ratio
            SEQ_SPIND._dForceRatio = Forcetratio;

            return true;
        }


        //---------------------------------------------------------------------------
        public void fn_UpdatePLCINData(ref Grid grd)
        {
            if (grd == null) return;

            Label[,] Items = new Label[20, 2];
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1, 0] = new Label();
                Items[n1, 1] = new Label();

                Items[n1, 0].BorderThickness = new Thickness(1);
                Items[n1, 0].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 0].FontSize = 11;

                Items[n1, 1].BorderThickness = new Thickness(1);
                Items[n1, 1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 1].FontSize = 11;
            }

            //
            string sTemp = string.Empty;
            EN_PLC_IN enPIn;
            for (int nRow = 0; nRow < EC_PLCIN.Length; nRow++)
            {
                enPIn = (EN_PLC_IN)nRow;
                sTemp = string.Format($"[{nRow:D2}] {enPIn.ToString()}");
                Items[nRow, 0].Content = sTemp;
                Items[nRow, 1].Content = EC_PLCIN[nRow].ToString();
            }

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < EC_PLCIN.Length; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Star);//columnDefinition.Width = new GridLength(1, GridUnitType.Auto);

            for (int c = 0; c < 2; c++)
            {
                if (c == 0) grd.ColumnDefinitions.Add(columnDefinition);
                else grd.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int r = 0; r < EC_PLCIN.Length; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r, c]);
					Grid.SetRow     (Items[r, c], r);
					Grid.SetColumn  (Items[r, c], c);
				}
            }
        }

        //---------------------------------------------------------------------------
        public void fn_UpdatePLCOutData(ref Grid grd)
        {
            if (grd == null) return;

            Label[,] Items = new Label[20, 2];
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1, 0] = new Label();
                Items[n1, 1] = new Label();

                Items[n1, 0].BorderThickness = new Thickness(1);
                Items[n1, 0].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 0].FontSize = 11;

                Items[n1, 1].BorderThickness = new Thickness(1);
                Items[n1, 1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 1].FontSize = 11;
            }

            //
            string sTemp = string.Empty;
            EN_PLC_OUT enPOut;
            for (int nRow = 0; nRow < EC_PLCOUT.Length; nRow++)
            {
                enPOut = (EN_PLC_OUT)nRow;
                sTemp = string.Format($"[{nRow:D2}] {enPOut.ToString()}");
                Items[nRow, 0].Content = sTemp;
                Items[nRow, 1].Content = EC_PLCOUT[nRow].ToString();
            }

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < EC_PLCOUT.Length; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Star);//columnDefinition.Width = new GridLength(1, GridUnitType.Auto);

            for (int c = 0; c < 2; c++)
            {
                if (c == 0) grd.ColumnDefinitions.Add(columnDefinition);
                else grd.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int r = 0; r < EC_PLCOUT.Length; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r,c]);
					Grid.SetRow     (Items[r, c], r);
					Grid.SetColumn  (Items[r, c], c);
				}
            }
        }
        //---------------------------------------------------------------------------
        public void fn_UpdateSMCtoEQData(ref Grid grd)
        {
            if (grd == null) return;

            Label[,] Items = new Label[DATA_SMC_TO_EQ.Length, 2];
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1, 0] = new Label();
                Items[n1, 1] = new Label();

                Items[n1, 0].BorderThickness = new Thickness(1);
                Items[n1, 0].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 0].FontSize = 11;

                Items[n1, 1].BorderThickness = new Thickness(1);
                Items[n1, 1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 1].FontSize = 11;
            }

            //
            string sTemp = string.Empty;
            string sMotr = string.Empty;
            EN_SMC_READ readadd;
            for (int nRow = 0; nRow < DATA_SMC_TO_EQ.Length; nRow++)
            {
                if      (nRow / 30 < 1) sMotr = "[SP-Z1]";
                else if (nRow / 30 < 2) sMotr = "[TR- Z]";
                else if (nRow / 30 > 1) sMotr = "[LD- T]";

                readadd = (EN_SMC_READ)(nRow % 30);
                sTemp = string.Format($"[{nRow:D2}] {sMotr} {readadd.ToString()}");
                Items[nRow, 0].Content = sTemp;
                Items[nRow, 1].Content = DATA_SMC_TO_EQ[nRow].ToString();
            }

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < DATA_SMC_TO_EQ.Length; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Star);//columnDefinition.Width = new GridLength(1, GridUnitType.Auto);

            for (int c = 0; c < 2; c++)
            {
                if(c == 0)grd.ColumnDefinitions.Add(columnDefinition);
                else      grd.ColumnDefinitions.Add(new ColumnDefinition());
            }

			for (int r = 0; r < DATA_SMC_TO_EQ.Length; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r,c]);
					Grid.SetRow     (Items[r, c], r);
					Grid.SetColumn  (Items[r, c], c);
				}
            }
        }
        //---------------------------------------------------------------------------
        public void fn_UpdateEQtoSMCData(ref Grid grd)
        {
            if (grd == null) return;

            Label[,] Items = new Label[DATA_EQ_TO_SMC.Length, 2];
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1, 0] = new Label();
                Items[n1, 1] = new Label();

                Items[n1, 0].BorderThickness = new Thickness(1);
                Items[n1, 0].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 0].FontSize = 11;

                Items[n1, 1].BorderThickness = new Thickness(1);
                Items[n1, 1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 1].FontSize = 11;
            }

            //
            string sTemp = string.Empty;
            string sMotr = string.Empty;

            EN_SMC_WRITE writeadd;
            for (int nRow = 0; nRow < DATA_EQ_TO_SMC.Length; nRow++)
            {
                if      (nRow / 30 < 1) sMotr = "[SP-Z1]";
                else if (nRow / 30 < 2) sMotr = "[TR- Z]";
                else if (nRow / 30 > 1) sMotr = "[LD- T]";

                writeadd = (EN_SMC_WRITE)(nRow % 30);
                sTemp = string.Format($"[{nRow:D2}] {sMotr} {writeadd.ToString()}");
                Items[nRow, 0].Content = sTemp;
                Items[nRow, 1].Content = DATA_EQ_TO_SMC[nRow].ToString();
            }

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < DATA_EQ_TO_SMC.Length; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Star);//columnDefinition.Width = new GridLength(1, GridUnitType.Auto);

            for (int c = 0; c < 2; c++)
            {
                if (c == 0) grd.ColumnDefinitions.Add(columnDefinition);
                else grd.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int r = 0; r < DATA_EQ_TO_SMC.Length; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r,c]);
					Grid.SetRow   (Items[r, c], r);
					Grid.SetColumn(Items[r, c], c);
				}
            }
        }

        //---------------------------------------------------------------------------
        public void fn_UpdateEQtoACSData(ref Grid grd)
        {
            if (grd == null) return;

            Label[,] Items = new Label[DATA_EQ_TO_ACS.Length, 2];
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1, 0] = new Label();
                Items[n1, 1] = new Label();

                Items[n1, 0].BorderThickness = new Thickness(1);
                Items[n1, 0].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 0].FontSize = 11;

                Items[n1, 1].BorderThickness = new Thickness(1);
                Items[n1, 1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 1].FontSize = 11;
            }

            //
            string sTemp = string.Empty;
            EN_EQ_TO_ACS enEtA;
            for (int nRow = 0; nRow < DATA_EQ_TO_ACS.Length; nRow++)
            {
                enEtA = (EN_EQ_TO_ACS)nRow;
                sTemp = string.Format($"[{nRow:D2}] {enEtA.ToString()}");
                Items[nRow, 0].Content = sTemp;
                Items[nRow, 1].Content = DATA_EQ_TO_ACS[nRow].ToString();
            }

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < DATA_EQ_TO_ACS.Length; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Star);//columnDefinition.Width = new GridLength(1, GridUnitType.Auto);

            for (int c = 0; c < 2; c++)
            {
                if (c == 0) grd.ColumnDefinitions.Add(columnDefinition);
                else grd.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int r = 0; r < DATA_EQ_TO_ACS.Length; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r,c]);
					Grid.SetRow   (Items[r, c], r);
					Grid.SetColumn(Items[r, c], c);
				}
            }
        }

        //---------------------------------------------------------------------------
        public void fn_UpdateACStoEQData(ref Grid grd)
        {
            if (grd == null) return;

            Label[,] Items = new Label[DATA_ACS_TO_EQ.Length, 2];
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1, 0] = new Label();
                Items[n1, 1] = new Label();

                Items[n1, 0].BorderThickness = new Thickness(1);
                Items[n1, 0].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 0].FontSize = 11;

                Items[n1, 1].BorderThickness = new Thickness(1);
                Items[n1, 1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1, 1].FontSize = 11;
            }

            //
            string sTemp = string.Empty;
            EN_ACS_TO_EQ enAtE;
            for (int nRow = 0; nRow < DATA_ACS_TO_EQ.Length; nRow++)
            {
                enAtE = (EN_ACS_TO_EQ)nRow;
                sTemp = string.Format($"[{nRow:D2}] {enAtE.ToString()}");
                Items[nRow, 0].Content = sTemp;
                Items[nRow, 1].Content = DATA_ACS_TO_EQ[nRow].ToString();
            }

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < DATA_ACS_TO_EQ.Length; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Star);//columnDefinition.Width = new GridLength(1, GridUnitType.Auto);

            for (int c = 0; c < 2; c++)
            {
                if (c == 0) grd.ColumnDefinitions.Add(columnDefinition);
                else grd.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int r = 0; r < DATA_ACS_TO_EQ.Length; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r,c]);
					Grid.SetRow   (Items[r, c], r);
					Grid.SetColumn(Items[r, c], c);
				}
            }

        }
        //-------------------------------------------------------------------------------------------------
        public int fn_GetIndexNo(EN_MOTR_ID motor)
        {
            int nRtn = -1;
            int nMotor = (int)EN_MOTR_ID.miSPD_Z1;

            if (motor < EN_MOTR_ID.miSPD_Z1) return 0;

            nRtn = ((int)motor - (int)nMotor) * 30;

            return nRtn;
        }
        //-------------------------------------------------------------------------------------------------
        public int fn_GetSMCData(EN_MOTR_ID motr, EN_SMC_READ add)
        {
            if (motr < EN_MOTR_ID.miSPD_Z1) return -1;
            
            int nAdd = fn_GetIndexNo(motr) + (int)add; 
            int rtn  = DATA_SMC_TO_EQ[nAdd]; 
            
            return rtn;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetSMCData(EN_MOTR_ID motr, EN_SMC_WRITE add, int value)
        {
            if (motr < EN_MOTR_ID.miSPD_Z1) return;

            int nAdd = fn_GetIndexNo(motr) + (int)add;
            //DATA_EQ_TO_SMC[nAdd] = value;

            if (fn_IsNeedDirect(add, value))
            {
                DATA_EQ_TO_SMC[nAdd] = value;
            }
            else
            {
                REQ_EQ_TO_SMC[nAdd] = value;
            }


        }
        //-------------------------------------------------------------------------------------------------
        private bool fn_IsNeedDirect(EN_SMC_WRITE add, int value)
        {
            if (add == EN_SMC_WRITE.JOG_RIGHT             ) return true; 
            if (add == EN_SMC_WRITE.JOG_LEFT              ) return true; 
            if (add == EN_SMC_WRITE.HOLD                  ) return true; 
            if (add == EN_SMC_WRITE.SERVO_ON && value != 1) return true; 

            return false;

        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_SetOpenLoopOff(EN_MOTR_ID motr = EN_MOTR_ID.miSPD_Z)
        {
            int    nMotr = (int)motr;
            string sCmd  = string.Format($"MFLAGS({nMotr}).#OPEN=0");

            try
            {
                //ACS_IO.Command("MFLAGS(2).#OPEN=0");
                ACS_IO.Command(sCmd);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            //fn_WriteLog("Close Loop Done : " + motr.ToString());
            return true;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_ClearPos(EN_MOTR_ID motr)
        {
            //ACS_IO.SetFPosition((Axis)EN_MOTR_ID.miCLN_R, 0);
            ACS_IO.SetFPosition((Axis)motr, 0);
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_MovePTP_R(int motr, double move)
        {
            if (motr < (int)EN_MOTR_ID.miSPD_Z1)
            {
                int nMotr = (int)motr;
                string sCmd = string.Format($"PTP/R {nMotr}, {move}");

                try
                {
                    ACS_IO.Command(sCmd);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                fn_WriteLog(string.Format($"Move PTT/R {motr}_{move}"));
                return true;
            }
            else
            {
                return false;
            }

        }
        //---------------------------------------------------------------------------
        public bool fn_GroupDisable(bool Clean_Y = false)
        {
            //SAFETYGROUP(0,1)
            int nAxis = Clean_Y ? (int)EN_MOTR_ID.miCLN_Y : (int)EN_MOTR_ID.miPOL_Y;
            string sCmd = string.Format($"SAFETYGROUP(0,{nAxis})");

            return fn_ACSCommand(sCmd);

        }
        //---------------------------------------------------------------------------

        public bool fn_ACSCommand(string cmd)
        {
            try
            {
                ACS_IO.Command(cmd);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            //fn_WriteLog("ACS Command = " + cmd);
            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_SetTEST()
        {
            try
            {
                ACS_IO.Command("VEL(3) = 9000");
                ACS_IO.Command("ACC(3) = 9000");
                ACS_IO.Command("DEC(3) = 9000");
                ACS_IO.Command("PTP/R 3, 144000");

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            fn_WriteLog("Close Loop Done");
            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_GetPLCIN(int index)
        {
            return EC_PLCIN[index] == 1;
        }
        //---------------------------------------------------------------------------
        public bool fn_SetAutoKey(bool force = false)
        {
            bool bAllHome   = MOTR.IsAllHomeEnd();
            bool bDoorOpen  = SEQ.fn_IsAnyDoorOpen();
            bool yOutOff    = !YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontLeft ] && !YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontRight] &&
                              !YV[(int)EN_OUTPUT_ID.ySYS_Door_Side]    ;
            bool bSICKErr   = fn_GetDoorSignalErr();

            if (force)
            {
                YV[(int)EN_OUTPUT_ID.ySW_AutoUse] = false; //PGM End
            }
            else
            {
                YV[(int)EN_OUTPUT_ID.ySW_AutoUse] = !bDoorOpen && bAllHome && yOutOff && !bSICKErr;
            }

            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_GetDoorSignalErr()
        {
            //SICK Door Signal 
            bool bErr = IO.EC_PLCIN[(int)EN_PLC_IN.SICK_DOOR_1] == 0 || IO.EC_PLCIN[(int)EN_PLC_IN.SICK_DOOR_2] == 0;

            return bErr;
        }
        //---------------------------------------------------------------------------
        public void fn_CloseCLNValve()
        {
            YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater] = false;
        }
        //---------------------------------------------------------------------------
        public void fn_SetTestDCOMValue(double value)
        {
            //
            DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_31_Mill_Force_Var] = value;
        }
        //-------------------------------------------------------------------------------------------------
        public double fn_GetDCOMValue()
        {
            double dValue = 0.0;
            object objVar = null;

            if (m_bConnect)
            {
                try
                {
                    objVar = ACS_IO.ReadVariable("DCOM", ProgramBuffer.ACSC_NONE, 0, 0, 2, 2);

                    if (objVar != null) dValue = Convert.ToDouble(objVar);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }

            return Math.Round(dValue, 2);
        }
        //---------------------------------------------------------------------------
        public bool fn_GetECError()
        {
            double dValue = -1;
            object objVar = null;

            if (m_bConnect)
            {
                try
                {
                    objVar = ACS_IO.ReadVariable("ECERR");

                    if (objVar != null) dValue = Convert.ToDouble(objVar);

                    m_bACSNetErr = (dValue != 0);

                }
                catch (System.Exception ex)
                {
                    m_bACSNetErr = true;

                    Console.WriteLine(ex);

                }


            }

            return m_bACSNetErr;
        }

        //---------------------------------------------------------------------------
        public double fn_GetVoltage(bool AI, int add)
        {
            if (add < 0) return 0.0;

            if (AI)
            {
                if (add >= MAX_INPUT_COUNT_A) return 0.0;

                return Math.Round((IO.AI[add] / 32767.0) * 10.0, 2);
            }
            else
            {
                if (add >= MAX_OUTPUT_COUNT_A) return 0.0;

                return Math.Round((IO.AO[add] / 32767.0) * 10.0, 2);
            }

        }
        //---------------------------------------------------------------------------
        public void fn_SetTopLoadCellOffset(double set)
        {
            m_dTopLoadCellOffSet = set;
            FM.m_stMasterOpt.dTopLDCellOffset = set;
        }

    }
}


