using mcOMRON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain;
using System.Windows;

namespace WaferPolishingSystem.BaseUnit
{
    public class AutoSupply
    {
        //IP, Port
        string SUPPLY_IP            ;
        int    SUPPLY_PORT          ;
        int    SUPPLY_START_ADD     ;
        string SUPPLY_EQP           ;

        const int    MAX_ADD  = 170;
        const int    MAX_SEND = 4;

        //Local Var
        private OmronPLC plc          ;
        private int      m_nType      ; 
        private int      m_nStep      ;
        private int      m_nCommStep  ;
        private int      m_nSlurryType;  //Supply slurry type

        private bool     m_bSend      ;
        private bool     m_bReqCon    ;
        private bool     m_bDrngCon   ;


        //1 : POU SLURRY REQUEST
        //2 : POU CLEANING REQUEST(DI)
        //3 : POU COMMUNICATION SIGNAL
        //4 : CMP DRAIN VALVE
        private bool m_bSlurryReq;
        private bool m_bCleanReq;
        private bool m_bCommSignal;
        private bool m_bDrainvValve;

        //Read data
        private bool m_bSlurryReqState;
        private bool m_bCleaningReqState;
        private bool m_bCommunicationReqState;
        private bool m_bDrainReqState;
        private string  m_strPouEquipId;
        private string  m_strSupplySlurryId;

        // Alarm
        private bool m_bIsAlarmSupplyRequest;
        private bool m_bIsAlarmUnitLeakDown;
        private bool m_bIsAlarmPropellerRotation;
        private bool m_bIsAlarmSupplyFluxHigh;
        private bool m_bIsAlarmSupplyFluxLow;
        private bool m_bIsAlarmReturnFluxHigh;
        private bool m_bIsAlarmReturnFluxLow;
        private bool m_bIsAlarmReturnPressureHigh;
        private bool m_bIsAlarmReturnPressureLow;
        private bool m_bIsAlarmSlurryTankLevelHigh;
        private bool m_bIsAlarmSlurryTankLevelLow;
        private bool m_bIsAlarmSlurryTankLevelEmpty;
        private bool m_bIsAlarmUnitLeakRoomBottom;
        private bool m_bIsAlarmUnitLeakTankRoom;
        private bool m_bIsAlarmUnitLeakValveBox;
        private bool m_bIsAlarmUnitLeakLeakBox;

        // Status
        private bool m_bStatusTankLevelLowLow;
        private bool m_bStatusTankLevelLow;
        private bool m_bStatusTankLevelMedium;
        private bool m_bStatusTankLevelHigh;
        private bool m_bStatusTankLevelHighHigh;
        private bool m_bStatusTankLevelHighHighHigh;
        private bool m_bStatusSlurryTankPropellerRotation;
        private double m_dReturnPressure;
        private double m_dSlurryFlux;
        private double m_dReturnFlux;
        private double m_dTankKg;
        private double m_dTankLiter;

        //Send data
        private byte[] m_temp;

        Label[] ItemA = new Label[MAX_ADD ]; //164 + 5
        Label[] ItemB = new Label[MAX_SEND]; //5

        TOnDelayTimer m_tCommDelayTimer = new TOnDelayTimer();
        //------------------------------------------------------------------------------------------------
        //Property

        public bool _bConnect { get { return plc.Connected; } }
        public byte[] _ReadData { get { return plc._bReadE0Data; } }
        public byte[] _SendData { get { return plc._bReadE2Data; } }

        //------------------------------------------------------------------------------------------------
        //Write Data
        public bool _bSlurryReq
        {
            get { return m_bSlurryReq; }
            set { m_bSlurryReq = value; }
        }
        public bool _bCleanReq
        {
            get { return m_bCleanReq; }
            set { m_bCleanReq = value; }
        }
        public bool _bDrainvValve
        {
            get { return m_bDrainvValve; }
            set { m_bDrainvValve = value; }
        }
        public bool _bDrngCon
        {
            get { return m_bDrngCon; }
        }

        //------------------------------------------------------------------------------------------------
        //Read Data
        public bool _bSlurryReqState        { get { return m_bSlurryReqState; } }
        public bool _bCleaningReqState      { get { return m_bCleaningReqState; } }
        public bool _bCommunicationReqState { get { return m_bCommunicationReqState; } }
        public bool _bDrainReqState         { get { return m_bDrainReqState; } }
        public string _strPouEquipId        { get { return m_strPouEquipId; } }
        public string _strSupplySlurryId    { get {return m_strSupplySlurryId; } }

        //------------------------------------------------------------------------------------------------
        // Alarm Data
        public bool _bIsAlarmPropellerRotation { get { return m_bIsAlarmPropellerRotation; } }
        public bool _bIsAlarmSupplyRequest { get { return m_bIsAlarmSupplyRequest; } }
        public bool _bIsAlarmUnitLeakDown { get { return m_bIsAlarmUnitLeakDown; } }
        public bool _bIsAlarmSupplyFluxHigh { get { return m_bIsAlarmSupplyFluxHigh; } }
        public bool _bIsAlarmSupplyFluxLow { get { return m_bIsAlarmSupplyFluxLow; } }
        public bool _bIsAlarmReturnFluxHigh { get { return m_bIsAlarmReturnFluxHigh; } }
        public bool _bIsAlarmReturnFluxLow { get { return m_bIsAlarmReturnFluxLow; } }
        public bool _bIsAlarmReturnPressureHigh { get { return m_bIsAlarmReturnPressureHigh; } }
        public bool _bIsAlarmReturnPressureLow { get { return m_bIsAlarmReturnPressureLow; } }
        public bool _bIsAlarmSlurryTankLevelHigh { get { return m_bIsAlarmSlurryTankLevelHigh; } }
        public bool _bIsAlarmSlurryTankLevelLow { get { return m_bIsAlarmSlurryTankLevelLow; } }
        public bool _bIsAlarmSlurryTankLevelEmpty { get { return m_bIsAlarmSlurryTankLevelEmpty; } }
        public bool _bIsAlarmUnitLeakRoomBottom { get { return m_bIsAlarmUnitLeakRoomBottom; } }
        public bool _bIsAlarmUnitLeakTankRoom { get { return m_bIsAlarmUnitLeakTankRoom; } }
        public bool _bIsAlarmUnitLeakValveBox { get { return m_bIsAlarmUnitLeakValveBox; } }
        public bool _bIsAlarmUnitLeakLeakBox { get { return m_bIsAlarmUnitLeakLeakBox; } }
        //------------------------------------------------------------------------------------------------- 
        // Status Data
        public bool _bStatusTankLevelLowLow { get { return m_bStatusTankLevelLowLow; } }
        public bool _bStatusTankLevelLow { get { return m_bStatusTankLevelLow; } }
        public bool _bStatusTankLevelMedium { get { return m_bStatusTankLevelMedium; } }
        public bool _bStatusTankLevelHigh { get { return m_bStatusTankLevelHigh; } }
        public bool _bStatusTankLevelHighHigh { get { return m_bStatusTankLevelHighHigh; } }
        public bool _bStatusTankLevelHighHighHigh { get { return m_bStatusTankLevelHighHighHigh; } }
        public bool _bStatusSlurryTankPropellerRotationStatus { get { return m_bStatusSlurryTankPropellerRotation; } }
        public double _dReturnPressure { get { return m_dReturnPressure; } }
        public double _dSlurryFlux { get { return m_dSlurryFlux; } }
        public double _dReturnFlux { get { return m_dReturnFlux; } }
        public double _dTankKg { get { return m_dTankKg; } }
        public double _dTankLiter { get { return m_dTankLiter; } }

        public int    _nSlurryType { get { return m_nSlurryType; } set { m_nSlurryType = value; } }
        //-------------------------------------------------------------------------------------------------
        public AutoSupply(int type)
        {
            this.plc = new OmronPLC();

            m_nType = type; 

            fn_InitVar();
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_InitVar()
        {
            SUPPLY_IP          = string.Empty;
            SUPPLY_PORT        =  0;
            SUPPLY_START_ADD   = -1;
            SUPPLY_EQP         = string.Empty;

            m_nStep                              = 0;
            m_bSend                              = false;

            m_bSlurryReq                         = false;
            m_bCleanReq                          = false;
            m_bCommSignal                        = false;
            m_bDrainvValve                       = false;
            m_nCommStep                          = 0;

            m_bSlurryReqState                    = false;
            m_bCleaningReqState                  = false;
            m_bCommunicationReqState             = false;
            m_bDrainReqState                     = false;
            m_strPouEquipId                      = string.Empty;
            m_strSupplySlurryId                  = string.Empty;

            m_bIsAlarmPropellerRotation          = false;
            m_bIsAlarmSupplyRequest              = false;
            m_bIsAlarmUnitLeakDown               = false;

            m_bIsAlarmSupplyFluxHigh             = false;
            m_bIsAlarmSupplyFluxLow              = false;
            m_bIsAlarmReturnFluxHigh             = false;
            m_bIsAlarmReturnFluxLow              = false;
            m_bIsAlarmReturnPressureHigh         = false;
            m_bIsAlarmReturnPressureLow          = false;
            m_bIsAlarmSlurryTankLevelHigh        = false;
            m_bIsAlarmSlurryTankLevelLow         = false;
            m_bIsAlarmSlurryTankLevelEmpty       = false;
            m_bIsAlarmUnitLeakRoomBottom         = false;
            m_bIsAlarmUnitLeakTankRoom           = false;
            m_bIsAlarmUnitLeakValveBox           = false;
            m_bIsAlarmUnitLeakLeakBox            = false;
                                                 
            m_bStatusTankLevelLowLow             = false;
            m_bStatusTankLevelLow                = false;
            m_bStatusTankLevelMedium             = false;
            m_bStatusTankLevelHigh               = false;
            m_bStatusTankLevelHighHigh           = false;
            m_bStatusTankLevelHighHighHigh       = false;
            m_bStatusSlurryTankPropellerRotation = false;

            m_dReturnPressure = 0.0;
            m_dSlurryFlux     = 0.0;
            m_dReturnFlux     = 0.0;
            m_dTankKg         = 0.0;
            m_dTankLiter      = 0.0;

            m_temp  = new byte[MAX_SEND];

            for (int n=0; n < MAX_ADD; n++)
            {
                ItemA[n] = new Label();
            }

            for (int n = 0; n < MAX_SEND; n++)
            {
                ItemB[n] = new Label();
            }

            m_nSlurryType = -1;

        }
        //---------------------------------------------------------------------------
        public void fn_SetAddress(string Ip, int Port, int Add, string EqpId)
        {
            SUPPLY_IP        = Ip   ;
            SUPPLY_PORT      = Port ;
            SUPPLY_START_ADD = Add  ;
            SUPPLY_EQP       = EqpId;
        }
        //---------------------------------------------------------------------------
        public void fn_Reset()
        {
            m_bDrngCon = false; 
            m_bReqCon  = true ; 
        }
        //-------------------------------------------------------------------------------------------------
        private bool fn_ResetCon()
        {
            string m_sIP    = string.Empty;
            int    m_nPort  = 0;

            //if(m_nType == SLURRY_POLISH) //Polishing
            //{             
            //    m_sIP   = SUPPLY_IP  ; //FM.m_stSystemOpt.sSupplyIp;
            //    m_nPort = SUPPLY_PORT; //FM.m_stSystemOpt.nSupplyPort;
            //}
            
            m_sIP   = SUPPLY_IP  ;
            m_nPort = SUPPLY_PORT;
            if (m_sIP == string.Empty || m_nPort == 0)
            {
                return false;
            }

            return fn_Connect(m_sIP, m_nPort);
        }
        //---------------------------------------------------------------------------
        public bool fn_Connect(string strIp, int nPort)
        {
            try
            {
                if (_bConnect) return false;

                bool rtn = false; 

                m_bDrngCon = true; 
                
                // Set Param
                plc.fn_SetParam(strIp, nPort);

                // connection
                rtn = plc.Connect();

                m_bDrngCon = false;

                return rtn;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                m_bDrngCon = false;
                return false;
            }
        }
        //------------------------------------------------------------------------------------------------
        public void fn_PlcUpdate()
        {
            if(m_bReqCon)
            {
                m_bReqCon = false;
                fn_ResetCon();
                return; 
            }
            
            fn_CheckAliveSignal();

            if (!_bConnect) return;

            fn_Update();

        }
        //------------------------------------------------------------------------------------------------
        private void fn_CheckAliveSignal()
        {
            switch (m_nCommStep)
            {
                case 0:
                    m_tCommDelayTimer.Clear();
                    m_nCommStep++;
                    return;

                case 1:
                    if (!m_tCommDelayTimer.OnDelay(true, 2000)) return;
                    
                    m_bCommSignal = !m_bCommSignal;
                    
                    m_nCommStep = 0;
                    return;

                default:
                    m_nCommStep = 0;
                    return;
            }
        }

        //-------------------------------------------------------------------------------------------------
        private void fn_Update()
        {
            switch (m_nStep)
            {
                case 0:
                    
                    if (m_bSend)
                    {
                        m_bSend = false; 
                        m_nStep = 5;
                        return; 
                    }
                    
                    m_nStep++;
                    return;

                //-------------------------------------------------------------------------------------------------
                // Read Seq
                case 1:
                    //if (!m_tSendDelayTimer.fnTimeOut(200)) return;
                    
                    //Read
                    plc.fn_ReadArea(MemoryArea.EM, (ushort)SUPPLY_START_ADD, 0, 512);

                    // Request State Check
                    fn_IsRequest();

                    // Get Status Data 
                    fn_GetStatusData();
                    
                    // Alarm Check
                    //fn_GetAlarm(); //-->> 차후 적용 필요

                    m_bSend = true;
                    m_nStep = 0;
                    return;

                //-------------------------------------------------------------------------------------------------
                // Send Seq
                case 5:
                    
                    if(!fn_CheckEquipID())
                    {
                        //Alarm
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0452 + m_nType); //JUNG/200811
                        
                        m_nStep = 0;
                        return;
                    }

                    //Write
                    fn_ConvertBooltoByte();
                    plc.fn_SendArea((ushort)SUPPLY_START_ADD, m_temp); //All Write

                    m_nStep = 0; 
                    return;

                default:
                    m_nStep = 0;
                    return;
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_ConvertBooltoByte()
        {
            m_temp[0] = Convert.ToByte(m_bSlurryReq  );
            m_temp[1] = Convert.ToByte(m_bCleanReq   );
            m_temp[2] = Convert.ToByte(m_bCommSignal );
            m_temp[3] = Convert.ToByte(m_bDrainvValve);
            
            for (int i = 0; i < MAX_SEND; i++)
            {
                if (m_temp[i] != 0 && m_temp[i] != 1) m_temp[i] = 0;
            }
        }
        //---------------------------------------------------------------------------
        public void fn_Disconnect()
        {
            try
            {
                plc.Close();

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_IsRequest()
        {
            m_bSlurryReqState                    = _ReadData[(int)EN_ATSUPPLY_READ_AREA._000_POU_SLURRY_REQUEST      ] == 1 ? true : false;
            m_bCleaningReqState                  = _ReadData[(int)EN_ATSUPPLY_READ_AREA._001_POU_CLEANING_REQUEST    ] == 1 ? true : false;
            m_bCommunicationReqState             = _ReadData[(int)EN_ATSUPPLY_READ_AREA._002_POU_COMMUNICATION_SIGNAL] == 1 ? true : false;
          //m_bDrainReqState                     = _ReadData[(int)EN_ATSUPPLY_READ_AREA._012_CMP_DRAIN_VALVE          ] == 1 ? true : false;
            m_strPouEquipId                      = fn_ConvertByteToStrValue(_ReadData, 16);
            m_strSupplySlurryId                  = fn_ConvertByteToStrValue(_ReadData, 32);
        }
        //-------------------------------------------------------------------------------------------------
        private bool fn_CheckEquipID()
        {
            //Init Value
            if (SUPPLY_EQP      == "0000"    ) return false ; 
            if (m_strPouEquipId != SUPPLY_EQP) return false ;
            
            return true;
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_GetStatusData()
        {
            m_bStatusTankLevelLowLow             = _ReadData[(int)EN_ATSUPPLY_READ_AREA._137_SUP_TK_A_LEVEL_LL     ] == 1 ? true : false;
            m_bStatusTankLevelLow                = _ReadData[(int)EN_ATSUPPLY_READ_AREA._138_SUP_TK_A_LEVEL_L      ] == 1 ? true : false;
            m_bStatusTankLevelMedium             = _ReadData[(int)EN_ATSUPPLY_READ_AREA._139_SUP_TK_A_LEVEL_M      ] == 1 ? true : false;
            m_bStatusTankLevelHigh               = _ReadData[(int)EN_ATSUPPLY_READ_AREA._140_SUP_TK_A_LEVEL_H      ] == 1 ? true : false;
            m_bStatusTankLevelHighHigh           = _ReadData[(int)EN_ATSUPPLY_READ_AREA._141_SUP_TK_A_LEVEL_HH     ] == 1 ? true : false;
            m_bStatusTankLevelHighHighHigh       = _ReadData[(int)EN_ATSUPPLY_READ_AREA._142_SUP_TK_A_LEVEL_HHH    ] == 1 ? true : false;
            m_bStatusSlurryTankPropellerRotation = _ReadData[(int)EN_ATSUPPLY_READ_AREA._162_AGITATOR_RUN__SUP_TK_A] == 1 ? true : false;

            m_dReturnPressure = fn_ConvertByteToDoubleValue(_ReadData, 208); //BTool.BytesToInt32(_ReadData[208], _ReadData[209]);
            m_dSlurryFlux     = fn_ConvertByteToDoubleValue(_ReadData, 288);
            m_dReturnFlux     = fn_ConvertByteToDoubleValue(_ReadData, 320);
            m_dTankKg         = fn_ConvertByteToDoubleValue(_ReadData, 464);
            m_dTankLiter      = fn_ConvertByteToDoubleValue(_ReadData, 496);

            return;

            //temp[0] = 0;
            //temp[1] = 1;
            //temp[2] = 0;
            //temp[3] = 1;

            //temp[4] = 1;
            //temp[5] = 0;
            //temp[6] = 0;
            //temp[7] = 1;

            //temp[8] = 0;
            //temp[9] = 0;
            //temp[10] = 0;
            //temp[11] = 0;

            //temp[12] = 0;
            //temp[13] = 0;
            //temp[14] = 0;
            //temp[15] = 0; // Hex : 009A  => 15.4

            //_ReadData[288] = 1;
            //_ReadData[289] = 0;
            //_ReadData[290] = 0;
            //_ReadData[291] = 1;

            //_ReadData[292] = 1;
            //_ReadData[293] = 1;
            //_ReadData[294] = 0;
            //_ReadData[295] = 0;

            //_ReadData[296] = 1;
            //_ReadData[297] = 0;
            //_ReadData[298] = 0;
            //_ReadData[299] = 0;

            //_ReadData[300] = 0;
            //_ReadData[301] = 0;
            //_ReadData[302] = 0;
            //_ReadData[303] = 0; // Hex : 0139  => 31.3
        }
        //---------------------------------------------------------------------------
        private double fn_ConvertByteToDoubleValue(byte[] bytes, int nIndex)
        {
            int nData = 0;
            int nLastIndex = nIndex + 16;

            for (int i = nIndex; i < nLastIndex; i++)
            {
                nData |= bytes[i] << i + nIndex;
            }
            return Convert.ToDouble(nData * 0.1);
        }
        //---------------------------------------------------------------------------
        private string fn_ConvertByteToStrValue(byte[] bytes, int nIndex)
        {
            int nData = 0;
            int nLastIndex = nIndex + 16;
            string str = string.Empty;

            for (int i = nIndex; i < nLastIndex; i++)
            {
                nData |= bytes[i] << i+ nIndex;
            }

            str = nData.ToString("X");

            return str;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_GetAlarm()
        {
            m_bIsAlarmUnitLeakDown         = _ReadData[(int)EN_ATSUPPLY_READ_AREA._049_UNIT_LEAK_DOWN          ] == 1 ? true : false;
            m_bIsAlarmUnitLeakRoomBottom   = _ReadData[(int)EN_ATSUPPLY_READ_AREA._056_UNIT_LEAK__ROOM_BOTTOM  ] == 1 ? true : false;
            m_bIsAlarmUnitLeakTankRoom     = _ReadData[(int)EN_ATSUPPLY_READ_AREA._057_UNIT_LEAK__TANK_ROOM    ] == 1 ? true : false;
            m_bIsAlarmUnitLeakLeakBox      = _ReadData[(int)EN_ATSUPPLY_READ_AREA._058_UNIT_LEAK__VALVE_BOX    ] == 1 ? true : false;
            m_bIsAlarmUnitLeakValveBox     = _ReadData[(int)EN_ATSUPPLY_READ_AREA._059_UNIT_LEAK__LEAK_BOX_    ] == 1 ? true : false;
            m_bIsAlarmPropellerRotation    = _ReadData[(int)EN_ATSUPPLY_READ_AREA._066_SUP_TK_A_ERROR          ] == 1 ? true : false;
            m_bIsAlarmSlurryTankLevelHigh  = _ReadData[(int)EN_ATSUPPLY_READ_AREA._068_SUP_TK_A_LEVEL_HH       ] == 1 ? true : false;
            m_bIsAlarmSlurryTankLevelLow   = _ReadData[(int)EN_ATSUPPLY_READ_AREA._069_SUP_TK_A_LEVEL_L_OFF    ] == 1 ? true : false;
            m_bIsAlarmSlurryTankLevelEmpty = _ReadData[(int)EN_ATSUPPLY_READ_AREA._070_SUP_TK_A_LEVEL_EMPTY    ] == 1 ? true : false;
            m_bIsAlarmSupplyFluxHigh       = _ReadData[(int)EN_ATSUPPLY_READ_AREA._071_SUPPLY__FLUX_HIGH       ] == 1 ? true : false;
            m_bIsAlarmSupplyFluxLow        = _ReadData[(int)EN_ATSUPPLY_READ_AREA._072_SUPPLY__FLUX_LOW        ] == 1 ? true : false;
            m_bIsAlarmReturnFluxHigh       = _ReadData[(int)EN_ATSUPPLY_READ_AREA._073_RETURN__FLUX_HIGH       ] == 1 ? true : false;
            m_bIsAlarmReturnFluxLow        = _ReadData[(int)EN_ATSUPPLY_READ_AREA._074_RETURN__FLUX_LOW        ] == 1 ? true : false;
            m_bIsAlarmReturnPressureHigh   = _ReadData[(int)EN_ATSUPPLY_READ_AREA._075_RETURN__PRESSURE_HIGH   ] == 1 ? true : false;
            m_bIsAlarmReturnPressureLow    = _ReadData[(int)EN_ATSUPPLY_READ_AREA._076_RETURN__PRESSURE_LOW    ] == 1 ? true : false;
            m_bIsAlarmSupplyRequest        = _ReadData[(int)EN_ATSUPPLY_READ_AREA._112_SUP_TK_A_CHARGE_REQUEST ] == 1 ? true : false;

            //Alarm Set
            if(m_nType == SPLY_SLURRY)
            {             
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0400, m_bIsAlarmUnitLeakDown        );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0401, m_bIsAlarmUnitLeakRoomBottom  );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0402, m_bIsAlarmUnitLeakTankRoom    );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0403, m_bIsAlarmUnitLeakLeakBox     );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0404, m_bIsAlarmUnitLeakValveBox    );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0405, m_bIsAlarmPropellerRotation   );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0406, m_bIsAlarmSlurryTankLevelHigh );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0407, m_bIsAlarmSlurryTankLevelLow  );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0408, m_bIsAlarmSlurryTankLevelEmpty);
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0409, m_bIsAlarmSupplyFluxHigh      );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0410, m_bIsAlarmSupplyFluxLow       );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0411, m_bIsAlarmReturnFluxHigh      );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0412, m_bIsAlarmReturnFluxLow       );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0413, m_bIsAlarmReturnPressureHigh  );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0414, m_bIsAlarmReturnPressureLow   );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0415, m_bIsAlarmSupplyRequest       ); 
            }
            else
            { 
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0420, m_bIsAlarmUnitLeakDown        );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0421, m_bIsAlarmUnitLeakRoomBottom  );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0422, m_bIsAlarmUnitLeakTankRoom    );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0423, m_bIsAlarmUnitLeakLeakBox     );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0424, m_bIsAlarmUnitLeakValveBox    );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0425, m_bIsAlarmPropellerRotation   );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0426, m_bIsAlarmSlurryTankLevelHigh );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0427, m_bIsAlarmSlurryTankLevelLow  );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0428, m_bIsAlarmSlurryTankLevelEmpty);
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0429, m_bIsAlarmSupplyFluxHigh      );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0430, m_bIsAlarmSupplyFluxLow       );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0431, m_bIsAlarmReturnFluxHigh      );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0432, m_bIsAlarmReturnFluxLow       );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0433, m_bIsAlarmReturnPressureHigh  );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0434, m_bIsAlarmReturnPressureLow   );
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0435, m_bIsAlarmSupplyRequest       ); 
            }

        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetReadGrid(ref Grid grd)
        {
            int nRow = MAX_ADD; 

            Label[] Items = new Label[nRow]; //164 + 5
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1] = new Label();

                Items[n1].BorderThickness = new Thickness(1);
                Items[n1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1].FontSize = 11;

                ItemA[n1].BorderThickness = new Thickness(1);
                ItemA[n1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                ItemA[n1].FontSize = 11;
            }

            if (grd == null) return;

            
            EN_ATSUPPLY_READ_AREA add = EN_ATSUPPLY_READ_AREA._000_POU_SLURRY_REQUEST;

            //Read Data : 164
            for (int r = 0; r < nRow-5; r++)
            {
                Items[r].Content = add++.ToString();
            }

            Items[165].Content = "Return Pressure"; 
            Items[166].Content = "Slurry Flux    "; 
            Items[167].Content = "Return Flux    "; 
            Items[168].Content = "Tank(Kg)       "; 
            Items[169].Content = "Tank(Liter)    "; 

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < nRow; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }
            
            for (int c = 0; c < 2; c++)
            {
                grd.ColumnDefinitions.Add(new ColumnDefinition());
            }
            grd.ColumnDefinitions[0].Width = new System.Windows.GridLength(3, GridUnitType.Star);
            grd.ColumnDefinitions[1].Width = new System.Windows.GridLength(1, GridUnitType.Star);


            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    if (c == 0)
                    {
                        grd.Children.Add(Items[r]);

                        Grid.SetRow   (Items[r], r);
                        Grid.SetColumn(Items[r], c);
                    }
                    if (c == 1)
                    {
                        grd.Children.Add(ItemA[r]);

                        Grid.SetRow   (ItemA[r], r);
                        Grid.SetColumn(ItemA[r], c);
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        public void fn_SetSendGrid(ref Grid grd)
        {
            int nRow = MAX_SEND;

            Label[] Items = new Label[nRow]; //4
            for (int n1 = 0; n1 <= Items.GetUpperBound(0); n1++)
            {
                Items[n1] = new Label();

                Items[n1].BorderThickness = new Thickness(1);
                Items[n1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                Items[n1].FontSize = 11;

                ItemB[n1].BorderThickness = new Thickness(1);
                ItemB[n1].BorderBrush = System.Windows.Media.Brushes.LightGray;
                ItemB[n1].FontSize = 11;
            }

            if (grd == null) return;

            EN_ATSUPPLY_SEND_AREA add = EN_ATSUPPLY_SEND_AREA._000_POU_SLURRY_REQUEST;

            //Read Data : 164
            for (int r = 0; r < nRow; r++)
            {
                Items[r].Content = add++.ToString();
            }

            grd.Children.Clear();
            grd.Background = System.Windows.Media.Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            for (int R = 0; R < nRow; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }

            for (int c = 0; c < 2; c++)
            {
                grd.ColumnDefinitions.Add(new ColumnDefinition());
            }
            grd.ColumnDefinitions[0].Width = new System.Windows.GridLength(3, GridUnitType.Star);
            grd.ColumnDefinitions[1].Width = new System.Windows.GridLength(1, GridUnitType.Star);


            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    if (c == 0)
                    {
                        grd.Children.Add(Items[r]);

                        Grid.SetRow(Items[r], r);
                        Grid.SetColumn(Items[r], c);
                    }
                    if (c == 1)
                    {
                        grd.Children.Add(ItemB[r]);

                        Grid.SetRow(ItemB[r], r);
                        Grid.SetColumn(ItemB[r], c);
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        public void fn_UpdateState()
        {
            //State Update
            for (int r = 0; r < MAX_ADD - 5; r++) //Read Data : 164
            {
                ItemA[r].Content = _ReadData[r] == 1 ? "1" : "0";
            }

            ItemA[3].Content    = m_strPouEquipId;
            ItemA[4].Content    = m_strSupplySlurryId;

            ItemA[165].Content = m_dReturnPressure.ToString();
            ItemA[166].Content = m_dSlurryFlux    .ToString();
            ItemA[167].Content = m_dReturnFlux    .ToString();
            ItemA[168].Content = m_dTankKg        .ToString();
            ItemA[169].Content = m_dTankLiter     .ToString();

            for (int r = 0; r < MAX_SEND; r++)
            {
                ItemB[r].Content = _SendData[r] == 1 ? "1" : "0";
            }

        }
        //---------------------------------------------------------------------------



    }
}
