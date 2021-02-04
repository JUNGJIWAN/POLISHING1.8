﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferPolishingSystem;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;

using static WaferPolishingSystem.FormMain;
using ACS.SPiiPlusNET;

namespace WaferPolishingSystem.BaseUnit
{
    /***************************************************************************/
    /* Constants                                                               */
    /***************************************************************************/

    //Motor Type.
    //===========================================================================
    public enum EN_MOTR_TYPE  : int
    {
        Rotary      ,
        Linear      ,
        Step        ,
        StepOriental,
        Induct      ,
        ACS         ,
        Etc
    }


    //Motor Kind.
    //===========================================================================
    enum EN_MOTR_KIND  : int
    {
        INC,
        ABS
    }


    //Motor Position ID.
    //===========================================================================
        //Common.
    public enum  EN_POSN_ID  : int 
    {
        None      = -1   ,
        User1     = 0    , //User defined position as Motor Characteristics.
        User2     = 1    ,
        User3     = 2    ,
        User4     = 3    ,
        User5     = 4    ,
        User6     = 5    ,
        User7     = 6    ,
        User8     = 7    ,
        User9     = 8    ,
        User10    = 9    ,
        User11    = 10   ,
        User12    = 11   ,
        User13    = 12   ,
        User14    = 13   ,
        User15    = 14   ,
        User16    = 15   ,
        User17    = 16   ,
        User18    = 17   ,
        User19    = 18   ,
        User20    = 19   ,
        User21    = 20   ,
        User22    = 21   ,
        User23    = 22   ,
        User24    = 23   ,
        User25    = 24   ,
        User26    = 25   ,
        User27    = 26   ,
        User28    = 27   ,
        User29    = 28   ,
        User30    = 29   ,
        Wait1     = 30   , //Basis Position.
        Wait2     = 31   ,
        IntpPos   = 32   , //
        FSP1_1    = 33   , //Step1 - Index1
        FSP1_2    = 34   , //        - fpIndex2
        FSP1_3    = 35   , //        - fpIndex3
        FSP1_4    = 36   , //        - fpIndex4
        FSP1_5    = 37   , //        - fpIndex5
        FSP1_6    = 38   , //        - fpIndex6
        FSP1_7    = 39   , //        - fpIndex7
        FSP1_8    = 40   , //        - fpIndex8
        FSP1_9    = 41   , //        - fpIndex9
        FSP2_1    = 42   , //Step2 - Index1
        FSP2_2    = 43   , //        - fpIndex2
        FSP2_3    = 44   , //        - fpIndex3
        FSP2_4    = 45   , //        - fpIndex4
        FSP2_5    = 46   , //        - fpIndex5
        FSP2_6    = 47   , //        - fpIndex6
        FSP2_7    = 48   , //        - fpIndex7
        FSP2_8    = 49   , //        - fpIndex8
        FSP2_9    = 50   , //        - fpIndex9
        FSP3_1    = 51   , //Step3 - Index1
        FSP3_2    = 52   , //        - fpIndex2
        FSP3_3    = 53   , //        - fpIndex3
        FSP3_4    = 54   , //        - fpIndex4
        FSP3_5    = 55   , //        - fpIndex5
        FSP3_6    = 56   , //        - fpIndex6
        FSP3_7    = 57   , //        - fpIndex7
        FSP3_8    = 58   , //        - fpIndex8
        FSP3_9    = 59   , //        - fpIndex9
        FSP4_1    = 60   , //Step4 - Index1
        FSP4_2    = 61   , //        - fpIndex2
        FSP4_3    = 62   , //        - fpIndex3
        FSP4_4    = 63   , //        - fpIndex4
        FSP4_5    = 64   , //        - fpIndex5
        FSP4_6    = 65   , //        - fpIndex6
        FSP4_7    = 66   , //        - fpIndex7
        FSP4_8    = 67   , //        - fpIndex8
        FSP4_9    = 68   , //        - fpIndex9
        Sply      = 69   ,
        Stck      = 70   ,
        CalPos    = 71   , //Calculated Position.
        UserPitch = 72   , //User Setting PITCH
        InPos     = 73   , //InPosition.
        Pick1     = 74   , //Tool Z-Axis.
        Pick2     = 75   ,
        Plce1     = 76   ,
        Plce2     = 77   ,
        PickInc   = 88   ,  //Pick-Up Auto Increasing Command.
        PlceInc   = 89   ,
        FindOff1  = 80   ,
        FindOff2  = 81   ,
        FindInc1  = 82   ,
        FindInc2  = 83
    }


    //Auto Tunning Response.
    //===========================================================================
    public enum  EN_AUT_RESP  : int 
    {
        Normal ,
        High   ,
        Low
    }

    //Limit Usage.
    //===========================================================================
    public enum  EN_LIMIT_USAGE  : int 
    {
        Use   ,
        NoUse ,
        UseP  ,
        UseN
    }

    //Linear Pack Type.
    //===========================================================================
    public enum  EN_PACK_TYPE  : int 
    {
        Normal    ,
        RJ004_12M ,
        RJ004_24M ,
        RJ004_48M
    }

    //Home Type.
    //===========================================================================
    public enum  EN_HOME_TYPE:int 
    {
        Normal        = 0  ,
        DataSet       = 1  ,

        elRLS_F_INDEX = 11 , //RLS Falling + Index             1 + 10
        elFLS_F_INDEX = 12 , //FLS Falling + Index             2
        elRLS_F       = 27 , //RLS Falling                    17
        elFLS_F       = 28 , //FLS Falling                    18
        elNEG_HOME_F  = 31 , //Home Neg Side, Falling Edgy    21
        elNEG_HOME_R  = 32 , //Home Neg Side, Rising Edgy     22
        elDATA_SET    = 45   //Current Position Home(DataSet) 35
    }

    //Home Option.
    //===========================================================================
    public enum  EN_HOME_OPT  : int 
    {
        optUse    ,
        optNoUse
    }

    //Home Option.
    //===========================================================================
    public enum  EN_ENCPULSE  : int 
    {
        pulse4000    ,
        pulse65535
    }
// 
//     enum EN_MOTR_DELAY  : int
//     {
//         TimeOut, //0                                                                        
//         Stop       //1  (6 ~ 9 UserDefined)
//     }

    /***************************************************************************/
    /* delegate                                                                */
    /***************************************************************************/

	delegate 	void   dgfInit          (int nAxis);
    delegate    bool   dgfDevReset      ();
    delegate    bool   dgfClose         ();
    delegate    void   dgfReset         ();
    delegate    void   dgfUpdateAxe     (int iAxis);
    delegate    bool   dgfOpen          (int nAxis);
    delegate    void   dgfClearHomeEnd  ();
    delegate    void   dgfSetServo      (int iAxis, int iOn                        );
    delegate    void   dgfSetAlarm      (int iAxis, int iOn                        );
    delegate    bool   dgfEmrgStop      (int iAxis                                 );
    delegate    bool   dgfStop          (int iAxis, bool   DecStop , double DecTime);
    delegate    bool   dgfMoveJogP      (int iAxis, double Vel, double Acc, double Dec = 0.0);
    delegate    bool   dgfMoveJogN      (int iAxis, double Vel, double Acc, double Dec = 0.0);
    delegate    bool   dgfMove          (int iAxis, double Pos , double Vel = 20.0, 
                                                    double Acc = 0.3, double Dec = 0.0, double SndPos = 0.0, int iSpdRatio = 0);
    delegate    bool   dgfMoveOverride  (int iAxis, double Pos , double Vel = 20.0, 
                                                    double Acc = 0.3, double Dec = 0.0, double SndPos = 0.0); //abs move with mm.
    delegate    bool   dgfMoveHome      (int iAxis, double Vel , double Acc, double Dec = 0.0, double OffsetPulse = 0.0 , double OffSetPos = 0.0);
    delegate    double dgfGetCmdPos     (int iAxis                                 );
    delegate    double dgfGetEncPos     (int iAxis                                 );
    delegate    bool   dgfSetPos        (int iAxis, double Pos                     );
    delegate    bool   dgfSetPosEncToCmd(int iAxis                                 );
                       
    delegate    void   dgfClearPos      (int iAxis, double Pos = 0.0);
    delegate    bool   dgfGetStop       (int iAxis, bool ChkEnc, double InPos);  
    delegate    bool   dgfMotionDone    (int iAxis                                 );
    delegate    void   dgfSetParamPath  (String Path, String CmePath = "");
    delegate    void   dgfSetType       (int iAxis, int    iType = 0 ,int iKind = 0 , int iNotUse = 0);
    delegate    void   dgfSetPulseOut   (int iAxis, int    Data   = 1);
    delegate    void   dgfSetEncInput   (int iAxis, int    Data   );
    delegate    void   dgfSetSONLevel   (int iAxis, int    Data   );
	delegate	void   dgfSetInpLevel   (int iAxis, int    Data   );
	delegate	void   dgfSetAlarmLevel (int iAxis, int    Data   );
	delegate	void   dgfSetLimitLevel (int iAxis, int    PosData, int NegData);
    delegate    void   dgfSetHomeLevel  (int iAxis, int    Data   );
    delegate    void   dgfSetDirection  (int iAxis, int    Data   );
    delegate    void   dgfSetAutoResp   (int iAxis, int    Data   );
    delegate    void   dgfSetPackType   (int iAxis, int    Data   );
    delegate    void   dgfSetHomeType   (int iAxis, int    Data   );
    delegate    void   dgfSetHomeOptn   (int iAxis, int    Data   , int Data2);
    delegate    void   dgfSetEncPulse   (int iAxis, int    Data   );
    delegate    void   dgfSetServoType  (int iAxis, int    Data   );
    delegate    void   dgfSetAppScurve  (int iAxis, bool   Data   );
    delegate    void   dgfSetIntpAxe    (int iAxis, int    Data   );
    delegate    void   dgfSetPairAxe    (int lMasterAxeNo, int lSlaveAxeNo=0, int Data2 = 0);
    delegate    void   dgfSetCoefficient(double Data              );
    delegate    void   dgfSetRingCounter(int iAxis, bool bEnable, double dMaxCntr);

	delegate	void   dgfSetABS        (int iAxis, int    Data1,int Data2);

	delegate	void   dgfSetPreTrgPos  (double dPos);
	delegate	void   dgfSetTrgPos     (double dPos);
    delegate    void   dgfSetHomeEnd    (bool bSet);
    delegate    void   dgfSetHomeEndDone(bool bSet);

    delegate	bool   dgfGServo        ();
	delegate	bool   dgfGHome         ();
    delegate	bool   dgfGHomeDone     ();
    delegate	bool   dgfGStop         ();
	delegate	bool   dgfGReady        ();
	delegate	bool   dgfGBusy         ();
    delegate	bool   dgfGHomeEnd      ();
	delegate	bool   dgfGHomeEndDone  ();
	delegate	bool   dgfGPackInPosn   ();
	delegate	bool   dgfGAlarm        ();
	delegate	bool   dgfGCW           ();
	delegate	bool   dgfGCCW          ();
	delegate	bool   dgfGZP           ();
	delegate	bool   dgfGLtBusy       ();
	delegate	bool   dgfGRing         ();
	delegate	double dgfGTorque       ();
	delegate	double dgfGPreTrgPos    ();
	delegate	double dgfGTrgPos       ();
	delegate	int	   dgfGHomeStep     ();

    delegate	void   dgfSetComPort    (string sPort);
    
    delegate    int    dgfGetErrCode1   ();
    delegate    int    dgfGetErrCode2   ();

    delegate	bool   dgfGSRL          ();
    delegate	bool   dgfGSLL          ();

    delegate    bool   dgfSetJerk   (int iAxis, double Data);
    delegate    bool   dgfSetKillDec(int iAxis, double Data);



    /***************************************************************************/
    /* Class: TAxisUnit                                                        */
    /* Create:                                                                 */
    /* Developer:                                                              */
    /* Note:                                                                   */
    /***************************************************************************/
    public class TAxisUnit
    {
        const int MAX_PITCH = 5;

        //Member Class 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ACSControl ACS;

        //Timer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        TOnDelayTimer m_StopOnTimer  = new TOnDelayTimer();
        TOnDelayTimer m_MoveTimer    = new TOnDelayTimer();
        TOnDelayTimer m_MoveInpTimer = new TOnDelayTimer();
        TOnDelayTimer m_tAbsOrigin   = new TOnDelayTimer();
        TOnDelayTimer m_tHomeEnd     = new TOnDelayTimer();
        TOnDelayTimer m_tJogStopDely = new TOnDelayTimer();

        //Vars.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //private:   /* Member Var.             */

        int      m_iAxis       ; //
        double   dScanTime     ;
        double   dStrtTime     ;
                
        double   m_dCoef       ; //Position.
        double   m_dCmdPos     ;
        double   m_dEncPos     ;
        bool     m_bJogSpd     ;
        bool     m_bStepMove   ;
        bool     m_bJogN       ; //Jog Status.
        bool     m_bJogP       ;
        bool     m_bLtJogN     ;
        bool     m_bLtJogP     ;
        bool     m_bTorqueN    ; //Torque Status.
        bool     m_bTorqueP    ;
        bool     m_bLtTorqueN  ;
        bool     m_bLtTorqueP  ;
        bool     m_bPrivServoOn;
        bool     m_bPrivAlarm  ;
        double[] m_dPitch    = new double[MAX_PITCH];
        double[] m_dCenPitch = new double[MAX_PITCH];
        int[]    m_NoSlot    = new int   [MAX_PITCH];
        double   m_dSpedGain   ; //One Time Motor Gain & Speed Mode.
        double   m_dAccGain    ;
        int      m_iSpedRatio  ;
        int      m_iObjNo      ;
        int      m_iAbsStep    ;
        int      m_iMaker      ;    
        bool     m_bInitAxis   ;
        //bool     m_bACSSKIP    ;


        //Var Motor Option
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int    m_iMotrType      ;
        public int    m_iMotrKind      ;
        public double m_dGR_Pulse      ;
        public double m_dGR_Disp       ;
        public double m_dMinPosn       ;
        public double m_dMaxPosn       ;
        public double m_dMaxVel        ;
        public double m_dMinAcc        ;
        public double m_dMinDec        ;
        public double m_dHomeOff       ;
        public double m_dMaxPulse      ;
        public double m_dInPos         ;
        public double m_dPartialPos    ;
        public double m_dAbsOriginPos  ;
        public double m_dPickOffset    ;
        public double m_dPlaceOffset   ;
        public int    m_iSONLevel      ;
        public int    m_iInpLevel      ;
        public int    m_iAlarmLevel    ;
        public int    m_iEncInputLevel ;
        public int    m_iEndLimitEnable;
        public int    m_iHomeLevel     ;
        public int    m_iProcessDir    ; //
        public int    m_iPulseOut      ;
        public int    m_iUseCam        ;
        public int    m_iNoUseEnc      ;
        public int    m_iNoUseMotr     ;
        public int    m_iNoUseHomeSen  ;
        public int    m_iMotionPart    ;
        public double m_dRingMax       ;
        public int    m_iPackType      ;
        public int    m_iIntpAxisNo    ;
        public int    m_iPairAxisNo    ;
        public int    m_iMoveDirection ;
        public int    m_iAutoResp      ;
        public int    m_iHomeType      ;
        public int    m_iHomeOptUse    ;
        public int    m_iEncPulse      ;
        public int    m_iServoType     ;
        public bool   m_bSetScurve     ;
        public int    m_iComPort       ; //EN_PORT_NAME
        public int    m_iCOMIStartIO   ;
        public int    m_iHomeSignal    ;
        public int    m_iHomeZPhase    ;

        public int    m_iPosLimitLevel ;
        public int    m_iNegLimitLevel ;
        public bool   m_bUseRingCounter;
        public bool   m_bUseOverride   ;
        public bool   m_bUseTorque     ;
        public bool   m_bRising        ;
        public int    m_iHomeDir       ;
        public string m_sComPort       ;
        public bool   m_bConect        ;


        public string m_sToqWAddr      ;
        public string m_sToqRAddr      ;


        //Define Motor                   
        public string m_sName          ;
        public string m_sNameAxis      ;
        //public string m_sImgP          ;
        //public string m_sImgN          ;

        //Define Manual No               
        public int    m_iManStop       ;
        public int    m_iManJog        ;
        public int    m_iManPitch      ;
        public int    m_iManServo      ;
        public int    m_iManAlarm      ;
        public int    m_iManDirect     ;
        public int    m_iManHome       ;
        public int    m_iManPartHome   ;

        //Define Error  No               
        public int    m_iErrAlarm      ;
        public int    m_iErrCW         ;
        public int    m_iErrCCW        ;
        public int    m_iErrHome       ;
        public int    m_iErrControl    ;
        public int    m_iErrHold       ;
        public int    m_iErrPos        ;
        public int    m_iErrVel        ;
        public int    m_iErrAcc        ;

        //Buffer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public MOTN_PARA MP     = new MOTN_PARA(0.0); //Motion parameters. (INI  )
        public MOTN_PARA CMP    = new MOTN_PARA(0.0);
        public MOTN_PARA TempMP = new MOTN_PARA(0.0);
                                


        //delegate 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        dgfInit            _fInit              ;
        dgfDevReset        _fDevReset          ;
        dgfClose           _fClose             ;
        dgfReset           _fReset             ;
        dgfUpdateAxe       _fUpdateAxe         ;
        dgfOpen            _fOpen              ;
        dgfClearHomeEnd    _fClearHomeEnd      ;
        dgfSetServo        _fSetServo          ;
        dgfSetAlarm        _fSetAlarm          ;
        dgfEmrgStop        _fEmrgStop          ;
        dgfStop            _fStop              ;
        dgfMoveJogP        _fMoveJogP          ;
        dgfMoveJogN        _fMoveJogN          ;
        dgfMove            _fMove              ;
        dgfMoveOverride    _fMoveOverride      ;
                          
        dgfMoveHome        _fMoveHome          ;
        dgfGetCmdPos       _fGetCmdPos         ;
        dgfGetEncPos       _fGetEncPos         ;
        dgfSetPos          _fSetPos            ;
        dgfSetPosEncToCmd  _fSetPosEncToCmd    ;
                                   
        dgfClearPos        _fClearPos          ;
        dgfGetStop         _fGetStop           ;
        dgfMotionDone      _fMotionDone        ;
        dgfSetParamPath    _fSetParamPath      ;
        dgfSetType         _fSetType           ;
        dgfSetPulseOut     _fSetPulseOut       ;
        dgfSetEncInput     _fSetEncInput       ;
        dgfSetSONLevel     _fSetSONLevel       ;
        dgfSetInpLevel     _fSetInpLevel       ;
        dgfSetAlarmLevel   _fSetAlarmLevel     ;
        dgfSetLimitLevel   _fSetLimitLevel     ;
        dgfSetHomeLevel    _fSetHomeLevel      ;
        dgfSetDirection    _fSetDirection      ;
        dgfSetAutoResp     _fSetAutoResp       ;
        dgfSetPackType     _fSetPackType       ;
        dgfSetHomeType     _fSetHomeType       ;
        dgfSetHomeOptn     _fSetHomeOptn       ;
        dgfSetEncPulse     _fSetEncPulse       ;
        dgfSetServoType    _fSetServoType      ;
        dgfSetAppScurve    _fSetAppScurve      ;
        dgfSetIntpAxe      _fSetIntpAxe        ;
        dgfSetPairAxe      _fSetPairAxe        ;
        dgfSetCoefficient  _fSetCoefficient    ;
        dgfSetRingCounter  _fSetRingCounter    ;                                  
        dgfSetABS          _fSetABS            ;                               
        dgfSetPreTrgPos    _fSetPreTrgPos      ;
        dgfSetTrgPos       _fSetTrgPos         ;
        dgfSetHomeEnd      _fSetHomeEnd        ;
        dgfSetHomeEndDone  _fSetHomeEndDone    ;
        dgfGServo          _fGServo            ;
        dgfGHome           _fGHome             ;
        dgfGStop           _fGStop             ;
        dgfGReady          _fGReady            ;
        dgfGBusy           _fGBusy             ;
        dgfGHomeEnd        _fGHomeEnd          ;
        dgfGHomeEndDone    _fGHomeEndDone      ;
        dgfGPackInPosn     _fGPackInPosn       ;
        dgfGAlarm          _fGAlarm            ;
        dgfGCW             _fGCW               ;
        dgfGCCW            _fGCCW              ;
        dgfGLtBusy         _fGLtBusy           ;
        dgfGRing           _fGRing             ;
        dgfGTorque         _fGTorque           ;
        dgfGPreTrgPos      _fGPreTrgPos        ;
        dgfGTrgPos         _fGTrgPos           ;
        dgfGHomeStep       _fGHomeStep         ;
        dgfSetComPort      _fSetComport        ;
        dgfGetErrCode1     _fGErrCode1         ;
        dgfGetErrCode2     _fGErrCode2         ;

        dgfGSRL            _fGSRL              ;
        dgfGSLL            _fGSLL              ;

        dgfSetJerk         _fSetJerk           ;
        dgfSetKillDec      _fSetKillDec        ;

        //Property.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int _iNoUseMotr
        {
            get { return m_iNoUseMotr; }
        }
        public int _iMaker
        {
            get { return m_iMaker; }
        }
        public int _iPairAxisNo
        {
            get { return m_iPairAxisNo; }
        }
        public bool _bInitAxis
        {
            get { return m_bInitAxis; } set { m_bInitAxis = value; }
        } 
        //Member Class 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public TAxisUnit()
        {

            //Init. Var.
            m_iAxis           = -1;
            m_iObjNo          = -1;
            m_iMaker          = (int)EN_MOTR_MAKER.ACS;
                              
            m_iMotrType       = (int)EN_MOTR_TYPE.Rotary;
            m_iMotrKind       = (int)EN_MOTR_KIND.INC;
            m_dGR_Pulse       = 8192.0;
            m_dGR_Disp        = 10.0;

            m_dMinPosn        = 0.0;
            m_dMaxPosn        = 100.0;
            m_dMaxVel         = 100.0;
            m_dMinAcc         = 0.1;
            m_dMinDec         = 0.1;

            m_dHomeOff        = 0.0;
            m_dMaxPulse       = 8192;
            m_dInPos          = 1.0;
            m_dPartialPos     = 1.0;
            m_dPickOffset     = 0.0;
            m_dPlaceOffset    = 0.0;

            m_iSONLevel       = 1;
            m_iInpLevel       = 1;
            m_iAlarmLevel     = 1;
            m_iEncInputLevel  = 3;
            m_iEndLimitEnable = 1;
            m_iHomeLevel      = 1;
            m_iProcessDir     = 1;
            m_iPulseOut       = 8;
            m_iUseCam         = 0;
            m_iNoUseEnc       = 0;
            m_iNoUseHomeSen   = 0;
            m_iMotionPart     = 0;
            m_iHomeDir        = 0;
            m_dRingMax        = 360.0;
            m_iPackType       = (int)EN_PACK_TYPE.RJ004_12M;
            m_iIntpAxisNo     = -1;
            m_iPairAxisNo     = -1;
            m_iSpedRatio      = (int)EN_SPED_RATIO.sr100;
            m_bUseOverride    = false;
            m_sComPort        = "COM1";

            //Define Manual No
            m_iManStop        = -1;
            m_iManJog         = -1;
            m_iManPitch       = -1;
            m_iManServo       = -1;
            m_iManAlarm       = -1;
            m_iManDirect      = -1;

            //Define Error  No
            m_iErrAlarm       = -1;
            m_iErrCW          = -1;
            m_iErrCCW         = -1;
            m_iErrHome        = -1;
            m_iErrControl     = -1;
            m_iErrHold        = -1;
            m_iErrPos         = -1;
            m_iErrVel         = -1;
            m_iErrAcc         = -1;
            m_bRising         = false;
            m_bSetScurve      = true;
            m_iHomeSignal     = 2;
            m_iHomeZPhase     = 0;
            m_iPosLimitLevel  = 1;
            m_iNegLimitLevel  = 1;
            m_iAbsStep        = 0;

            m_bConect         = false;
            //m_bACSSKIP        = false;  

            for (int i = 0; i < MAX_PITCH; i++)
            {
                m_dPitch   [i] = 0.0;
                m_dCenPitch[i] = 0.0;
                m_NoSlot   [i] = 0;
            }
        }
        
        //Base Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Init(int iAxe, int iObjNo, int iMaker, Api acs)
        {
            //Init. Var.
            m_iAxis  = iAxe  ;
            m_iObjNo = iObjNo;
            m_iMaker = iMaker;

            ACS = new ACSControl(acs);

            //Function Set.
            if (IsMakerACS())
            {
                _fInit           = ACS.Init;
                _fClose          = ACS.fn_CloseACS;
                _fDevReset       = ACS.fn_DisableAllAsync;
                _fReset          = ACS.fn_Reset;
                _fUpdateAxe      = ACS.fn_Update;
                _fOpen           = ACS.fn_Open;
                _fClearHomeEnd   = ACS.fn_ClearHomeEnd;
                _fSetServo       = ACS.fn_SetServo;
                _fSetAlarm       = ACS.fn_SetAlarm;
                _fEmrgStop       = ACS.fn_StopEMG;
                _fStop           = ACS.fn_Stop;
                _fMoveJogP       = ACS.fn_MoveJogP;
                _fMoveJogN       = ACS.fn_MoveJogN;
                _fMove           = ACS.fn_Move;
                _fMoveHome       = ACS.fn_MoveHome;
                _fGetCmdPos      = ACS.fn_GetCmdPos;
                _fGetEncPos      = ACS.fn_GetEncPos;
                _fSetPos         = ACS.fn_SetPos;
                _fSetPosEncToCmd = ACS.fn_SetPosEncToCmd;
                _fClearPos       = ACS.fn_ClearPos;
                _fGetStop        = ACS.fn_GetStop;
                _fMotionDone     = ACS.fn_MotionDone;
                _fSetType        = ACS.fn_SetType;
                _fSetHomeType    = ACS.fn_SetHomeType;
                _fSetHomeOptn    = ACS.fn_SetHomeOptn;
                _fSetCoefficient = ACS.fn_SetCoefficient;
                _fSetABS         = ACS.fn_SetABS;
                
                _fSetJerk        = ACS.fn_SetParameterJerk;
                _fSetKillDec     = ACS.fn_SetParameterKillDeceleration;

                //
                _fSetPreTrgPos   = ACS.sPreTrgPos;
                _fSetTrgPos      = ACS.sTrgPos;
                _fSetHomeEnd     = ACS.sHomeEnd;
                _fSetHomeEndDone = ACS.sHomeEndDone;
                _fGServo         = ACS.gServo;
                _fGHome          = ACS.gHome;
                _fGStop          = ACS.gStop;
                _fGReady         = ACS.gReady;
                _fGBusy          = ACS.gBusy;
                _fGHomeEnd       = ACS.gHomeEnd;
                _fGHomeEndDone   = ACS.gHomeEndDone;
                _fGPackInPosn    = ACS.gPackInPosn;
                _fGAlarm         = ACS.gAlarm ;
                _fGCW            = ACS.gCW    ;
                _fGCCW           = ACS.gCCW   ;
                _fGLtBusy        = ACS.gLtBusy;
                _fGRing          = ACS.gRing  ;
                _fGTorque        = ACS.gTorque;
                _fGPreTrgPos     = ACS.gPreTrgPos;
                _fGTrgPos        = ACS.gTrgPos;
                _fGHomeStep      = ACS.gHomeStep;
                _fGSRL           = ACS.gSRL    ;
                _fGSLL           = ACS.gSLL    ;


                _fMoveOverride   = null; //ACS.MoveOverride  ;
                _fSetParamPath   = null; //ACS.SetParamPath  ;
                _fSetPulseOut    = null; //ACS.SetPulseOut   ;
                _fSetEncInput    = null; //ACS.SetEncInput   ;
                _fSetSONLevel    = null; //ACS.SetSONLevel   ;
                _fSetInpLevel    = null; //ACS.SetInpLevel   ;
                _fSetAlarmLevel  = null; //ACS.SetAlarmLevel ;
                _fSetLimitLevel  = null; //ACS.SetLimitLevel ;
                _fSetHomeLevel   = null; //ACS.SetHomeLevel  ;
                _fSetDirection   = null; //ACS.SetDirection  ;
                _fSetAutoResp    = null; //ACS.SetAutoResp   ;
                _fSetPackType    = null; //ACS.SetPackType   ;
                _fSetEncPulse    = null; //ACS.SetEncPulse   ;
                _fSetServoType   = null; //ACS.SetServoType  ;
                _fSetAppScurve   = null; //ACS.SetAppScurve  ;
                _fSetIntpAxe     = null; //ACS.SetIntpAxe    ;
                _fSetPairAxe     = null; //ACS.SetPairAxe    ;
                _fSetRingCounter = null; //ACS.SetRingCounter;

                _fSetComport     = null; //ACS.SetComPort;
                _fGErrCode1      = null; //ACS.gErrCode1;
                _fGErrCode2      = null; //ACS.gErrCode2;

            }
            else if (IsMakerSMC())
            {
                _fInit           = ACS.Init;
                _fClose          = ACS.fn_CloseACS;
                _fDevReset       = ACS.fn_DisableAllAsync;
                _fReset          = ACS.fn_Reset;
                _fUpdateAxe      = ACS.fn_UpdateSMC;
                _fOpen           = ACS.fn_OpenSMC;
                _fClearHomeEnd   = ACS.fn_ClearHomeEndSMC;
                _fSetServo       = ACS.fn_SetServoSMC;
                _fSetAlarm       = ACS.fn_SetAlarmSMC;
                _fEmrgStop       = ACS.fn_StopEMGSMC;
                _fStop           = ACS.fn_StopSMC;
                _fMoveJogP       = ACS.fn_MoveJogPSMC;
                _fMoveJogN       = ACS.fn_MoveJogNSMC;
                _fMove           = ACS.fn_MoveSMC;
                _fMoveHome       = ACS.fn_MoveHomeSMC;
                _fGetCmdPos      = ACS.fn_GetCmdPos;
                _fGetEncPos      = ACS.fn_GetEncPos;
                _fSetPos         = null; //ACS.fn_SetPos;
                _fSetPosEncToCmd = ACS.fn_SetPosEncToCmdSMC;
                _fClearPos       = ACS.fn_ClearPosSMC;
                _fGetStop        = ACS.fn_GetStop;
                _fMotionDone     = ACS.fn_MotionDone;
                _fSetType        = null;//ACS.fn_SetType;
                _fSetHomeType    = null;//ACS.fn_SetHomeType;
                _fSetHomeOptn    = null;//ACS.fn_SetHomeOptn;
                _fSetCoefficient = ACS.fn_SetCoefficient;
                _fSetABS         = null; //ACS.fn_SetABS;

                //
                _fSetPreTrgPos   = ACS.sPreTrgPos;
                _fSetTrgPos      = ACS.sTrgPos;
                _fSetHomeEnd     = ACS.sHomeEnd;
                _fSetHomeEndDone = ACS.sHomeEndDone;
                _fGServo         = ACS.gServo;
                _fGHome          = ACS.gHome;
                _fGStop          = ACS.gStop;
                _fGReady         = ACS.gReady;
                _fGBusy          = ACS.gBusy;
                _fGHomeEnd       = ACS.gHomeEnd;
                _fGHomeEndDone   = ACS.gHomeEndDone;
                _fGPackInPosn    = ACS.gPackInPosn;
                _fGAlarm         = ACS.gAlarm ;
                _fGCW            = ACS.gCW    ;
                _fGCCW           = ACS.gCCW   ;
                _fGLtBusy        = ACS.gLtBusy;
                _fGRing          = ACS.gRing  ;
                _fGTorque        = ACS.gTorque;
                _fGPreTrgPos     = ACS.gPreTrgPos;
                _fGTrgPos        = ACS.gTrgPos;
                _fGHomeStep      = ACS.gHomeStep;

                _fMoveOverride   = null; //ACS.MoveOverride  ;
                _fSetParamPath   = null; //ACS.SetParamPath  ;
                _fSetPulseOut    = null; //ACS.SetPulseOut   ;
                _fSetEncInput    = null; //ACS.SetEncInput   ;
                _fSetSONLevel    = null; //ACS.SetSONLevel   ;
                _fSetInpLevel    = null; //ACS.SetInpLevel   ;
                _fSetAlarmLevel  = null; //ACS.SetAlarmLevel ;
                _fSetLimitLevel  = null; //ACS.SetLimitLevel ;
                _fSetHomeLevel   = null; //ACS.SetHomeLevel  ;
                _fSetDirection   = null; //ACS.SetDirection  ;
                _fSetAutoResp    = null; //ACS.SetAutoResp   ;
                _fSetPackType    = null; //ACS.SetPackType   ;
                _fSetEncPulse    = null; //ACS.SetEncPulse   ;
                _fSetServoType   = null; //ACS.SetServoType  ;
                _fSetAppScurve   = null; //ACS.SetAppScurve  ;
                _fSetIntpAxe     = null; //ACS.SetIntpAxe    ;
                _fSetPairAxe     = null; //ACS.SetPairAxe    ;
                _fSetRingCounter = null; //ACS.SetRingCounter;

                _fSetComport     = null; //ACS.SetComPort;
                _fGErrCode1      = null; //ACS.gErrCode1;
                _fGErrCode2      = null; //ACS.gErrCode2;

                _fGSRL           = ACS.gSRL;
                _fGSLL           = ACS.gSLL;

            }

            _fInit(m_iAxis);
            
        }   
        //---------------------------------------------------------------------------
        public bool Connect(int mode)
        {
            m_bConect = ACS.fn_Connect(mode);

            return m_bConect; 
        }
        //--------------------------------------------------------------------------
        public bool Open()
        {
            bool bOpen = false;

            if (m_bConect)
            {
                //Open Motor
                bOpen = _fOpen(m_iAxis);
            }
            return bOpen;
        }
        //--------------------------------------------------------------------------
        public void Close(    )
        {
            _fClose();
        }
        //--------------------------------------------------------------------------
        public void Reset(    )
        {
            if (m_iObjNo < 0 || m_iObjNo >= FormMain.MOTR._iNumOfMotr) return;
            if(!m_bInitAxis) return;

            //Reset homing flag.
            if (GetStop ()) { _fReset(); }
            if (GetAlarm()) { _fClearPos(m_iObjNo); }
            _fSetTrgPos(m_dCmdPos);

            m_dSpedGain = 1.0;
            m_dAccGain  = 1.0;


            //Reset Timer.
            m_StopOnTimer .Clear();
            m_MoveTimer   .Clear();
            m_MoveInpTimer.Clear();
        }
        //--------------------------------------------------------------------------
        public void  DevReset         (    )
        {
            _fDevReset();
        }
        //Home Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void ClearHomeEnd()
        {
            //Check Motor Status.
            if (m_iNoUseMotr==1) return;
            if(!m_bInitAxis    ) return;

            _fClearHomeEnd();
            SetHomeEndDone(false);

            System.Console.WriteLine("--------------------> ClearHomeEnd() <--------------------");

            int iPairAxe = GetPairAxe();
            if (iPairAxe >= 0)
            {
                _fClearHomeEnd();
            }
        }
        //--------------------------------------------------------------------------
        public bool HomeProc(double Vel = 50.0)
        {
            return false;
        }
        //--------------------------------------------------------------------------
        //Motion Done
        public bool MotnDone (bool ChkEnc , double Inp)
        {
            return GetStop(ChkEnc, Inp);
        }
        //--------------------------------------------------------------------------
        public bool        MotnDone         ()
        {
            return GetStop(true, MP.dPosn[(int)EN_POSN_ID.InPos]);
        }
        //--------------------------------------------------------------------------

        public bool isMakerAjin         ()
        {
            return (m_iMaker == (int)EN_MOTR_MAKER.AJIN || m_iMaker == (int)EN_MOTR_MAKER.AJECAT); 

        }
        public bool IsMakerACS()
        {
            return (m_iMaker == (int)EN_MOTR_MAKER.ACS);

        }
        public bool IsMakerSMC()
        {
            return (m_iMaker == (int)EN_MOTR_MAKER.SMC);

        }

        //--------------------------------------------------------------------------
        //Set Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public void SetParameter(int Axis = -1)
        {
            SetAxis          (Axis);
            SetType          (m_iMotrType, m_iMotrKind, m_iNoUseMotr);

            //SetSONLevel      (m_iSONLevel);
            //SetInpLevel      (m_iInpLevel);
            //SetAlarmLevel    (m_iAlarmLevel);
            //SetHomeLevel     (m_iHomeLevel);
            //SetPulseOut      (m_iPulseOut);
            //SetEncInput      (m_iEncInputLevel);
            //SetAppScurve     (m_bSetScurve);
            //SetPackType      (m_iPackType);
            //SetDirection     (m_iMoveDirection);
            //SetIntpAxe       (m_iIntpAxisNo); //보간
            //SetPairAxe       (m_iPairAxisNo); //동기화
            //SetAutoResp      (m_iAutoResp);
            //SetHomeType      (m_iHomeType);
            //SetHomeOptn      (m_iHomeOptUse, m_iHomeDir);
            //SetEncPulse      (m_iEncPulse);
            SetServoType     (m_iServoType);
            //SetMoveHomeSen   (m_iHomeSignal, m_iHomeZPhase);
            //SetEndLimitEnable(m_iEndLimitEnable);
            //SetLimitLevel    (m_iPosLimitLevel, m_iNegLimitLevel);
            //SetABS           (m_iComPort, m_iCOMIStartIO);

            SetMaxMin        (m_dMaxPosn, m_dMaxVel, 5.0, m_dMinPosn, 0.05, m_dMinAcc);
            SetMaxSped       ();
            SetInPos         (m_dInPos);
            //SetComPort       (m_sComPort);

            //Set Co.eff.
            if (m_dGR_Disp <= 0) { 
                SetCoefficient(1); //SetCoefficient(8192.0 / 10.0); 
                SetMaxPulse   (8192); 
            }
            else {
                SetCoefficient(1); //SetCoefficient(m_dGR_Pulse / m_dGR_Disp); 
                SetMaxPulse   (m_dGR_Pulse); 
            }

            //SetAbsOrigin  (m_dAbsOriginPos);
            //SetRingCounter(m_bUseRingCounter, m_dGR_Disp);
            //SetUseTorque  (m_bUseTorque     , m_sToqWAddr, m_sToqRAddr);

            
        }
        //--------------------------------------------------------------------------
        public void SetAxis(int Axis = -1)
        {
            m_iAxis = Axis;
        }
        //--------------------------------------------------------------------------
        public void SetType(int  iType     , int iKind, int iNotUse = 0 )
        {
            if(!m_bInitAxis    ) return;
            m_iMotrType = iType;
            m_iMotrKind = iKind;
            _fSetType(m_iObjNo, iType, iKind, iNotUse);            
        }
        //--------------------------------------------------------------------------
        public void        SetABS           (int        ComPort  , int SrartIO                 )
        {
            if (!    m_bInitAxis                                     ) return;
            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return;
            if (m_iMotrKind  != (int)EN_MOTR_KIND.ABS                ) return;
            if (m_iMaker == (int)EN_MOTR_MAKER.COMI && m_iMotrKind == (int)EN_MOTR_KIND.ABS) _fSetABS(m_iObjNo, ComPort, SrartIO);
            else
            {
                _fSetABS(m_iObjNo, m_iMotrKind , 0);
            }
        }
        //--------------------------------------------------------------------------
        public void SetPart(int iPart= 0)
        {
            m_iMotionPart = iPart;
        }
        //--------------------------------------------------------------------------
        public void SetProcessDir(int iDir)
        {
            m_iProcessDir = iDir; 
        }
        //--------------------------------------------------------------------------
        public void SetPPOffset(double pick, double place)
        {
            m_dPickOffset  = pick ;
            m_dPlaceOffset = place; 
        }
        //--------------------------------------------------------------------------

        public void SetParamPath (string Path , string CemPath)
        {
            //if(!m_bInitAxis    ) return;
            _fSetParamPath(Path, CemPath);  
        }
        //--------------------------------------------------------------------------
        public void  SetCoefficient(double Data  = 819)
        {

            m_dCoef = Data; 
            _fSetCoefficient(Data);
        }
        //--------------------------------------------------------------------------
        public void SetPulseOut(int Data = 1)
        {
            if(!m_bInitAxis    ) return;
            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; _fSetPulseOut(m_iObjNo, Data); 
        }
        //--------------------------------------------------------------------------
        public void SetEncInput(int Data = 2) //2:Sqr4Mode
        {
            if (!m_bInitAxis    ) return;
            if ( m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; _fSetEncInput(m_iObjNo, Data); 
        }
        //--------------------------------------------------------------------------
        public void SetSONLevel(int Data = 0 ) //1:Positive Level. 0:Negative Level.
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            m_iSONLevel = Data; 
            _fSetSONLevel(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void SetInpLevel(int Data = 0)
        {
            if(!m_bInitAxis                                 ) return;
            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetInpLevel(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetAlarmLevel    (int        Data     = 0                           )
        {
            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetAlarmLevel(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetHomeLevel     (int        Data     = 0                           )
        {
            if(!m_bInitAxis                                      ) return;
            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetHomeLevel(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetLimitLevel    (int        PosData  = 0,int        NegData  = 0   )
        {
            if(!m_bInitAxis                                     ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetLimitLevel(m_iObjNo, PosData, NegData);
        }

        //--------------------------------------------------------------------------
        public void        SetComPort    (string sComPort)
        {
            if(!m_bInitAxis                                     ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetComport(sComPort);
        }

        //--------------------------------------------------------------------------
        public void        SetInPos         (double     Data                                   )
        {
            m_dInPos = Data;

            //Check Min/Max.
            if (m_dInPos < m_dGR_Disp / m_dGR_Pulse) m_dInPos = m_dGR_Disp / m_dGR_Pulse;
            if (m_dInPos > m_dGR_Disp / 10) m_dInPos = m_dGR_Disp / 10;
        }
        //--------------------------------------------------------------------------
        public void        SetHomeOff       (double     Off      = 0                           )
        {
            m_dHomeOff = Off;
        }
        //--------------------------------------------------------------------------
        public void        SetAbsOrigin     (double     dPos     = 0                           )
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            double dAbsOff = dPos + m_dHomeOff;
            if(m_iMotrKind != (int)EN_MOTR_KIND.ABS) return;
            m_dAbsOriginPos = dPos;
            //if(isMakerAjin()) AJIN.SetAbsOffset(CalPosToPulse(dAbsOff));
        }
        //--------------------------------------------------------------------------
        public void        SetAppScurve     (bool       bApply   = true                        )
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            m_bSetScurve = bApply; 
            //_fSetAppScurve(m_iObjNo, bApply);
        }
        //--------------------------------------------------------------------------
        public void        SetNoUseEnc      (bool       NoUse                                  )
        {
            m_iNoUseEnc = NoUse ? 1 : 0;  
        }
        //--------------------------------------------------------------------------
        public void  SetNoUseHomeSen  (bool       NoUse                                  )
        {
            m_iNoUseHomeSen = NoUse ? 1 : 0;
        }
        //--------------------------------------------------------------------------
        public void  SetCamUse        (int        iUseCam           , double dPartialPos )
        {
            m_iUseCam = iUseCam;
            m_dPartialPos = dPartialPos;
        }
        //--------------------------------------------------------------------------
        public void  SetMaxSped  ()
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

//             if(isMakerAjin())
//             {
//                      if (m_iMotrType == (int)EN_MOTR_TYPE.Rotary) AJIN.SetMaxSped(m_iObjNo, 3000000);
//                 else if (m_iMotrType == (int)EN_MOTR_TYPE.Linear) AJIN.SetMaxSped(m_iObjNo, 5000000);
//                 //
//                 //else                                              AJIN.SetMaxSped(m_iObjNo,  100000);
//                 else                                              AJIN.SetMaxSped(m_iObjNo, 3000000);
//             }
        }
        //--------------------------------------------------------------------------
        public void        SetMaxPulse      (double        Data)
        {
            m_dMaxPulse = Data; 
        }
        //--------------------------------------------------------------------------
        public void        SetDirection     (int        Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetDirection(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetAutoResp      (int        Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetAutoResp(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetPackType      (int        Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetPackType(m_iObjNo, Data); 
        }
        //--------------------------------------------------------------------------
        public void        SetHomeType      (int        Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetHomeType(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetHomeOptn      (int        Data, int Data2)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetHomeOptn(m_iObjNo, Data, Data2);
        }
        //--------------------------------------------------------------------------
        public void        SetEncPulse      (int        Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetEncPulse(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetServoType     (int        Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            //_fSetServoType(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetIntpAxe       (int        Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetIntpAxe(m_iObjNo, Data); 
        }
        //--------------------------------------------------------------------------
        public void        SetPairAxe       (int iSlaveNo=0, int Data2=1) 
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            if(iSlaveNo < 0 || iSlaveNo >= MOTR._iNumOfMotr) return;

            int iSlaveAxe = MOTR[iSlaveNo].m_iObjNo;

            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; _fSetPairAxe(m_iObjNo, iSlaveAxe, Data2);
        }
        //--------------------------------------------------------------------------
        public void        SetVGain         (double     V                                      )
        {
            m_dSpedGain = V;
            int iPairAxe = GetPairAxe();
            if (iPairAxe >= 0)
            {
                MOTR[iPairAxe].SetVGain(V);
            }
        }
        //--------------------------------------------------------------------------
        public void        SetAGain         (double     A                                      )
        {
            m_dAccGain = A;
            int iPairAxe = GetPairAxe();
            if (iPairAxe >= 0)
            {
                MOTR[iPairAxe].SetAGain(A);
            }
        }
        //--------------------------------------------------------------------------
        public void        SetGain          (double     V         , double A                   )
        {
            m_dSpedGain = V;
            m_dAccGain  = A;
        }
        //--------------------------------------------------------------------------
        public void SetJogSpd(bool Data)
        {
            m_bJogSpd = Data;
        }
        //--------------------------------------------------------------------------
        public void SetHomeEnd(bool Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            if (Data)
            {
                if (m_iHomeType == (int)EN_HOME_TYPE.DataSet) _fSetPos(m_iObjNo, 0.0);
                if (GetAbsAxe() && _fGHomeEnd() == false) _fMove(m_iObjNo, _fGetEncPos(m_iObjNo), MP.MinVel, MP.MaxAcc);
            }
            _fSetHomeEnd(Data);

            int iPairAxe = GetPairAxe();
            if (iPairAxe >= 0)
            {
                MOTR[iPairAxe].SetHomeEnd(Data);
            }
        }
        //-------------------------------------------------------------------------------------------------
        public void SetHomeEndDone(bool set)
        {
            _fSetHomeEndDone(set);
        }
        //--------------------------------------------------------------------------
        public void SetStepMove(bool Data)
        {
            m_bStepMove = Data;
        }
        //--------------------------------------------------------------------------
        public void SetMoveHomeSen (int Data1, int Data2 )
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            //if(isMakerAjin()) AJIN.SetMoveHomeSensor(m_iObjNo, Data1, Data2);
        }
        //--------------------------------------------------------------------------
        public void        SetEndLimitEnable(int       Data)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            //if(isMakerAjin()) AJIN.SetEndLimitEnable(m_iObjNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetServoParam    (int        iParamNo, int Data                     )
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            //if(isMakerAjin()) AJIN.SetServoParam(m_iObjNo, iParamNo, Data);
        }
        //--------------------------------------------------------------------------
        public void        SetUseTorque     (bool       Data, string sWAddr, string sRAddr)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            //if(isMakerAjin()) AJIN.SetUseTorque(m_iObjNo, Data, sWAddr, sRAddr);
        }

        //Device Informations.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetPitch(double dPitch, int iSlot = 1, double dCenPitch = 0.0, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.StepA)
        {
            if (FStep == EN_FSTEP_INDEX.StepA)
            {
                m_dPitch[0] = dPitch; m_dCenPitch[0] = dCenPitch; m_NoSlot[0] = iSlot;
                m_dPitch[1] = dPitch; m_dCenPitch[1] = dCenPitch; m_NoSlot[1] = iSlot;
                m_dPitch[2] = dPitch; m_dCenPitch[2] = dCenPitch; m_NoSlot[2] = iSlot;
                m_dPitch[3] = dPitch; m_dCenPitch[3] = dCenPitch; m_NoSlot[3] = iSlot;
            }

            if (FStep < 0 || (int)FStep >= MAX_PITCH) return;
            m_dPitch   [(int)FStep] = dPitch;
            m_NoSlot   [(int)FStep] = iSlot ;
            m_dCenPitch[(int)FStep] = dCenPitch;
        }
        //--------------------------------------------------------------------------
        public double GetPitch(EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            if (FStep < 0 || (int)FStep >= MAX_PITCH) return 1.0;

            double dRet = m_dPitch[(int)FStep];
            if (dRet < 0.1) dRet = 0.1;
            return dRet;
        }
        //--------------------------------------------------------------------------
        public double GetCenPitch(EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            if (FStep < 0 || (int)FStep >= MAX_PITCH) return 0.0;

            double dRet = m_dCenPitch[(int)FStep];
            if (dRet <= 0) dRet = 0.0;
            return dRet;
        }
        //--------------------------------------------------------------------------
        public double GetCenPitch(int iSlotNo, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            int  iMaxSlot  = GetMaxSlot(FStep);
            int  iMidSlot  = iMaxSlot / 2;
            bool IsLowSlot = iSlotNo < iMidSlot;
            if (FStep < 0 || (int)FStep >= MAX_PITCH) return 0.0;
            if (IsLowSlot) return 0.0;

            double dRet = m_dCenPitch[(int)FStep];
            if (dRet <= 0) dRet = 0.0;
            return dRet;
        }
        //--------------------------------------------------------------------------
        public int GetMaxSlot(EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            if (FStep < 0 || (int)FStep >= MAX_PITCH) return 1;

            int dRet = m_NoSlot[(int)FStep];
            if (dRet < 0) dRet = 1;
            return dRet;
        }
        //--------------------------------------------------------------------------
        //In/Out Sensors.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool        GetServo         (                                               ) //Servo Motor Enable Signal.
        {
            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false;
            
            return _fGServo();
        }
        //--------------------------------------------------------------------------
        public bool        GetHome          (                                               ) //Home Sensor.
        {
            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 
            return _fGHome();
        }
        //--------------------------------------------------------------------------
        public bool GetStop(bool ChkEnc = false , double InPos = 0.2) //Motion Done.
        {
            //Local Var.
            double dTrg = 0.0;
            double dCmd = 0.0;
            double dEnc = 0.0;

            //Get Motion Done.
            if(!m_bInitAxis                                ) return true;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return true; 
            if (m_iNoUseMotr==1                            ) return true; 

            //Check Stop.
            if (!ChkEnc) return _fGStop();
            else
            {
                if (!GetHomeEnd()) return _fGStop();
                dTrg = GetTrgPos();
                dCmd = GetCmdPos();
                dEnc = GetEncPos();
                if (_fGStop())
                {
                    if (_fGRing())
                    {
                        bool r1 = Math.Abs((m_dGR_Disp - dEnc) - dTrg) < InPos;
                        bool r2 = Math.Abs(dEnc - dTrg) < InPos;
                        if (r1 || r2) dEnc = dTrg;
                    }
                    if (Math.Abs(dCmd - dEnc) > InPos) return false;
                    if (Math.Abs(dTrg - dCmd) > InPos) return false;
                    if (Math.Abs(dTrg - dEnc) > InPos) return false;

                    //Set Pre Trg Pos.
                    _fSetPreTrgPos(dTrg);

                    if (m_bRising)
                    {
                        m_bRising = false;
                    }

                    //return
                    return true;

                    //
                    //if (m_MoveInpTimer.Ondelay(true , 10)) return true ;
                    //else                                   return false;
                }
            }
            return false;
        }
        //--------------------------------------------------------------------------
        public bool GetBusy() //Check in motion.
        {
            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 
            return _fGBusy();
        }
        //--------------------------------------------------------------------------
        public bool GetCntrErr()//Check Control Error. (Command Position >< Actual Position)
        {
            m_StopOnTimer.OnDelay(GetStop(), 100);
            if (m_StopOnTimer.Out) { if (GetErrPos() > m_dInPos) return true; }
            else { if (CalPosToPulse(GetErrPos()) > m_dGR_Pulse) return true; }
            return false;
        }
        //--------------------------------------------------------------------------
        public bool GetHomeEnd() //Check Home Done. (Logical)
        {
            return _fGHomeEnd(); 
        }
        //-------------------------------------------------------------------------------------------------
        public bool GetHomeEndDone() //Home Done for SMC Motor and Homing
        {
            return _fGHomeEndDone();
        }
        //--------------------------------------------------------------------------
        public bool GetTimeOut() //Check Moving TimeOut.
        {
            return m_MoveTimer.OnDelay(!GetStop(true, m_dInPos), (int)MP.dTime[(int)EN_MOTR_DELAY.TimeOut]);
        }
        //--------------------------------------------------------------------------
        public bool GetPackInPosn()//Check InPosition Signal. (Servo Pack Signal)
        {
            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 

            return _fGPackInPosn();
        }
        //--------------------------------------------------------------------------
        public bool GetAlarm () //Check Motor Alarm Signal.
        {
            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 
            if (m_iNoUseMotr==1                            ) return false;
            return _fGAlarm(); 
        }
        //--------------------------------------------------------------------------
        public bool GetCW() //Check CW Limit.
        {
            if ( m_iNoUseMotr==1                             ) return false;
            if (!m_bInitAxis                                 ) return false;
            if ( m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false;

            if      (m_iPosLimitLevel == 0) { return !_fGCW();  }//LOW
            else if (m_iPosLimitLevel == 1) { return  _fGCW();  }//high
            else return false;
            
            //return _fGCW();
        }
        //--------------------------------------------------------------------------
        public bool GetCCW() //Check CCW Limit.
        {
            if ( m_iNoUseMotr==1                             ) return false;
            if (!m_bInitAxis                                 ) return false;
            if ( m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false;

            if      (m_iNegLimitLevel == 0) { return !_fGCCW(); }//LOW
            else if (m_iNegLimitLevel == 1) { return  _fGCCW(); }//high
            else return false;

            //return _fGCCW();
        }
        //--------------------------------------------------------------------------
        public bool GetSRL() //Check SRL
        {
            if ( m_iNoUseMotr==1                             ) return false;
            if (!m_bInitAxis                                 ) return false;
            if ( m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false;
          
            return _fGSRL();

        }
        //--------------------------------------------------------------------------
        public bool GetSLL() //Check SLL
        {
            if ( m_iNoUseMotr==1                             ) return false;
            if (!m_bInitAxis                                 ) return false;
            if ( m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false;
          
            return _fGSLL();

        }

        //--------------------------------------------------------------------------
        public bool GetOk() //Check No Limit & Servo Enable.
        {
            if (!m_bInitAxis                                 ) return false;
            if ( m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 
            if ( m_iNoUseMotr==1                             ) return true ; 
            if ( GetCCW        ()) return false;
            if ( GetCW         ()) return false;
            if ( GetAlarm      ()) return false;
            if (!GetHomeEnd    ()) return false;
            if (!GetServo      ()) return false;
            return true;
        }
        //--------------------------------------------------------------------------
        public bool GetReady() //Check ready to move.
        {
            if (m_iNoUseMotr==1 ) return true; 

            //if (GetCCW        ()) return false;
            //if (GetCW         ()) return false;
            if (GetAlarm      ()) return false;
            //if (!GetServo     ()) return false;
            //if (!GetPackInPosn()) return false;
            //if (!GetStop      ()) return false;
            return true;
        }
        //--------------------------------------------------------------------------
        public int         GetHomeStep      (                                               )
        {
            return _fGHomeStep();
        }
        //--------------------------------------------------------------------------
        public bool        GetJogSpd        (                                               )
        {
            return m_bJogSpd;
        }
        //--------------------------------------------------------------------------
        public bool         GetStepMove      (                                               )
        {
            return m_bStepMove;
        }
        //--------------------------------------------------------------------------
        public double      GetTorque        (                                               )
        {
            if(!m_bInitAxis                                ) return 0;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return 0; 
            return _fGTorque();
        }
        //--------------------------------------------------------------------------
        public int         GetRunCode       (                                               )
        {
            return 0;
        }
        //--------------------------------------------------------------------------
        public int         GetAlarmCode     (int Type = 0                                   )
        {
            switch (Type)
            {
                default: return 0;
                case 0 : return  _fGErrCode1();
                case 1 : return  _fGErrCode2();
            }

            //return 0;
        }
        //--------------------------------------------------------------------------
        public int         GetDetailCode    (                                               )
        {
            return 0;
        }
        //--------------------------------------------------------------------------
        public string  GetName          (bool IsNor                                         )
        {
            return "";
        }  
        //--------------------------------------------------------------------------      
        public double      GetFindStep      (int Step , EN_FSTEP_INDEX FStep , EN_FPOSN_INDEX FIndex)
        {
            //Local Var.
            double dPitch;
            double dCenPitch;
            double dMovePosn;

            //Check None Step, Index.
            if (Step   == UserConst.NONE_STEP) Step   = 0;
            if (FStep  == EN_FSTEP_INDEX.NONE) FStep  = EN_FSTEP_INDEX.Step1;
            if (FIndex == EN_FPOSN_INDEX.NONE) FIndex = EN_FPOSN_INDEX.Index1;

            if(!m_bInitAxis                                ) return 0;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return 0; 
            if(m_iNoUseMotr==1                             ) return 0;

            //Get Pitch.
            dPitch = GetPitch(FStep);
            dCenPitch = GetCenPitch(Step, FStep);
            int iPosnNo = 0;
            //Set position to move.
            switch (FStep)
            {
                case EN_FSTEP_INDEX.Step1: iPosnNo = (int)EN_POSN_ID.FSP1_1; break;
                case EN_FSTEP_INDEX.Step2: iPosnNo = (int)EN_POSN_ID.FSP2_1; break;
                case EN_FSTEP_INDEX.Step3: iPosnNo = (int)EN_POSN_ID.FSP3_1; break;
                case EN_FSTEP_INDEX.Step4: iPosnNo = (int)EN_POSN_ID.FSP4_1; break;
            }
            if ((iPosnNo + FIndex) < 0 || (iPosnNo + (int)FIndex) >= MAX_POSN) FIndex = EN_FPOSN_INDEX.Index1;
            if (m_iProcessDir==1)
            {
                
                dMovePosn = MP.dPosn[iPosnNo + (int)FIndex] + (double)Step * dPitch + dCenPitch;

            }
            else
            {
                dMovePosn = MP.dPosn[iPosnNo + (int)FIndex] - (double)Step * dPitch - dCenPitch;
            }

            return dMovePosn;
        }
        //--------------------------------------------------------------------------
        public double GetNextCmdTrg(EN_COMD_ID Comd , int Step , EN_FPOSN_INDEX FIndex, double DirPosn)
        {
            //Local Var.
            int            iMaxSlot;
            int            iPosnId ;
            double         dCenPitch, dCenPitchP, dCenPitchN;
            double         dPitch, dPitchP, dPitchN;
            EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1;
            double         dPickOffset  = m_dPickOffset  ; //5;
            double         dPlaceOffset = m_dPlaceOffset ; //5;

            //Check None Step, Index.
            if (Step == NONE_STEP            ) Step = 0;
            if (FIndex == EN_FPOSN_INDEX.NONE) FIndex = EN_FPOSN_INDEX.Index1;
            if (m_iNoUseMotr == 1            ) return GetTrgPos();

            if (Comd == EN_COMD_ID.FindStep1  ||
                Comd == EN_COMD_ID.FindStepF1 ||
                Comd == EN_COMD_ID.FindStepB1 ||
                Comd == EN_COMD_ID.OneStepF1  ||
                Comd == EN_COMD_ID.OneStepB1) FStep = EN_FSTEP_INDEX.Step1;
            if (Comd == EN_COMD_ID.FindStep2 ||
                Comd == EN_COMD_ID.FindStepF2 ||
                Comd == EN_COMD_ID.FindStepB2 ||
                Comd == EN_COMD_ID.OneStepF2 ||
                Comd == EN_COMD_ID.OneStepB2) FStep = EN_FSTEP_INDEX.Step2;
            if (Comd == EN_COMD_ID.FindStep3 ||
                Comd == EN_COMD_ID.FindStepF3 ||
                Comd == EN_COMD_ID.FindStepB3 ||
                Comd == EN_COMD_ID.OneStepF3 ||
                Comd == EN_COMD_ID.OneStepB3) FStep = EN_FSTEP_INDEX.Step3;
            if (Comd == EN_COMD_ID.FindStep4 ||
                Comd == EN_COMD_ID.FindStepF4 ||
                Comd == EN_COMD_ID.FindStepB4 ||
                Comd == EN_COMD_ID.OneStepF4 ||
                Comd == EN_COMD_ID.OneStepB4) FStep = EN_FSTEP_INDEX.Step4;

            //
            iMaxSlot = GetMaxSlot  (FStep);    
            
            //Get Pitch.
            dPitch    = GetPitch   (FStep);
            dCenPitch = GetCenPitch(Step, FStep);

            dPitchP    = dPitch    * ((m_iProcessDir==1) ? 1 : -1);
            dPitchN    = dPitch    * ((m_iProcessDir==1) ? 1 : -1);
            dCenPitchP = dCenPitch * ((m_iProcessDir==1) ? 1 : -1);
            dCenPitchN = dCenPitch * ((m_iProcessDir==1) ? 1 : -1);

            //Command
            switch (Comd)
            {
                case EN_COMD_ID.JogP: return GetCmdPos();
                case EN_COMD_ID.JogN: return GetCmdPos();

                case EN_COMD_ID.FindStep1: 
                    iPosnId = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP1_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP;

                case EN_COMD_ID.FindStepF1: 
                    iPosnId = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP1_1;
                    return MP.dPosn[iPosnId] + (Step * dPitch) + dCenPitch + dPitchP;

                case EN_COMD_ID.FindStepB1: 
                    iPosnId = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP1_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitch - dPitchP;

                case EN_COMD_ID.OneStepF1: return GetEncPos() + dPitchP + dCenPitchP;
                case EN_COMD_ID.OneStepB1: return GetEncPos() - dPitchN - dCenPitchN;

                case EN_COMD_ID.FindStep2: 
                    iPosnId = (int)EN_POSN_ID.FSP2_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP2_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP;

                case EN_COMD_ID.FindStepF2: 
                    iPosnId = (int)EN_POSN_ID.FSP2_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP2_1;
                    return MP.dPosn[iPosnId] + (Step * dPitch) + dCenPitch;
                case EN_COMD_ID.FindStepB2:
                    iPosnId = (int)EN_POSN_ID.FSP2_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP2_1;
                    return MP.dPosn[iPosnId] - (Step * dPitch) - dCenPitch;
                case EN_COMD_ID.OneStepF2: return GetTrgPos() + dPitchP + dCenPitchP;
                case EN_COMD_ID.OneStepB2: return GetTrgPos() - dPitchN - dCenPitchN;
                case EN_COMD_ID.FindStep3:
                    iPosnId = (int)EN_POSN_ID.FSP3_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP3_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP;
                case EN_COMD_ID.FindStepF3: 
                    iPosnId = (int)EN_POSN_ID.FSP3_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP3_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP;
                case EN_COMD_ID.FindStepB3: 
                    iPosnId = (int)EN_POSN_ID.FSP3_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP3_1;
                    return MP.dPosn[iPosnId] - (Step * dPitchP) + dCenPitchP;
                case EN_COMD_ID.OneStepF3: return GetTrgPos() + dPitchP + dCenPitchP;
                case EN_COMD_ID.OneStepB3: return GetTrgPos() - dPitchN - dCenPitchN;
                case EN_COMD_ID.FindStep4:
                    iPosnId = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP4_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP;
                case EN_COMD_ID.FindStepF4: 
                    iPosnId = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP4_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP;

                case EN_COMD_ID.FindStepB4: 
                    iPosnId = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP4_1;
                    return MP.dPosn[iPosnId] - (Step * dPitchP) - dCenPitchP;
                case EN_COMD_ID.OneStepF4: return GetTrgPos() + dPitchP + dCenPitchP;
                case EN_COMD_ID.OneStepB4: return GetTrgPos() - dPitchN - dCenPitchN;

                case EN_COMD_ID.UserPitchP: return GetCmdPos() + (MP.dPosn[(int)EN_POSN_ID.UserPitch] *  1.0);
                case EN_COMD_ID.UserPitchN: return GetCmdPos() + (MP.dPosn[(int)EN_POSN_ID.UserPitch] * -1.0);


                case EN_COMD_ID.PickInc:
                    iPosnId = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP1_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP + dPickOffset;
                    
                case EN_COMD_ID.PlceInc:
                    iPosnId = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
                    if (iPosnId < 0 || iPosnId >= MAX_POSN) iPosnId = (int)EN_POSN_ID.FSP1_1;
                    return MP.dPosn[iPosnId] + (Step * dPitchP) + dCenPitchP - dPlaceOffset;

                case EN_COMD_ID.Wait1 : return MP.dPosn[(int)EN_POSN_ID.Wait1 ];
                case EN_COMD_ID.Wait2 : return MP.dPosn[(int)EN_POSN_ID.Wait2 ];
                case EN_COMD_ID.FSP1_1: return MP.dPosn[(int)EN_POSN_ID.FSP1_1];
                case EN_COMD_ID.FSP1_2: return MP.dPosn[(int)EN_POSN_ID.FSP1_2];
                case EN_COMD_ID.FSP1_3: return MP.dPosn[(int)EN_POSN_ID.FSP1_3];
                case EN_COMD_ID.FSP1_4: return MP.dPosn[(int)EN_POSN_ID.FSP1_4];
                case EN_COMD_ID.FSP1_5: return MP.dPosn[(int)EN_POSN_ID.FSP1_5];
                case EN_COMD_ID.FSP1_6: return MP.dPosn[(int)EN_POSN_ID.FSP1_6];
                case EN_COMD_ID.FSP1_7: return MP.dPosn[(int)EN_POSN_ID.FSP1_7];
                case EN_COMD_ID.FSP1_8: return MP.dPosn[(int)EN_POSN_ID.FSP1_8];
                case EN_COMD_ID.FSP1_9: return MP.dPosn[(int)EN_POSN_ID.FSP1_9];

                case EN_COMD_ID.FSP2_1: return MP.dPosn[(int)EN_POSN_ID.FSP2_1];
                case EN_COMD_ID.FSP2_2: return MP.dPosn[(int)EN_POSN_ID.FSP2_2];
                case EN_COMD_ID.FSP2_3: return MP.dPosn[(int)EN_POSN_ID.FSP2_3];
                case EN_COMD_ID.FSP2_4: return MP.dPosn[(int)EN_POSN_ID.FSP2_4];
                case EN_COMD_ID.FSP2_5: return MP.dPosn[(int)EN_POSN_ID.FSP2_5];
                case EN_COMD_ID.FSP2_6: return MP.dPosn[(int)EN_POSN_ID.FSP2_6];
                case EN_COMD_ID.FSP2_7: return MP.dPosn[(int)EN_POSN_ID.FSP2_7];
                case EN_COMD_ID.FSP2_8: return MP.dPosn[(int)EN_POSN_ID.FSP2_8];
                case EN_COMD_ID.FSP2_9: return MP.dPosn[(int)EN_POSN_ID.FSP2_9];

                case EN_COMD_ID.FSP3_1: return MP.dPosn[(int)EN_POSN_ID.FSP3_1];
                case EN_COMD_ID.FSP3_2: return MP.dPosn[(int)EN_POSN_ID.FSP3_2];
                case EN_COMD_ID.FSP3_3: return MP.dPosn[(int)EN_POSN_ID.FSP3_3];
                case EN_COMD_ID.FSP3_4: return MP.dPosn[(int)EN_POSN_ID.FSP3_4];
                case EN_COMD_ID.FSP3_5: return MP.dPosn[(int)EN_POSN_ID.FSP3_5];
                case EN_COMD_ID.FSP3_6: return MP.dPosn[(int)EN_POSN_ID.FSP3_6];
                case EN_COMD_ID.FSP3_7: return MP.dPosn[(int)EN_POSN_ID.FSP3_7];
                case EN_COMD_ID.FSP3_8: return MP.dPosn[(int)EN_POSN_ID.FSP3_8];
                case EN_COMD_ID.FSP3_9: return MP.dPosn[(int)EN_POSN_ID.FSP3_9];

                case EN_COMD_ID.FSP4_1: return MP.dPosn[(int)EN_POSN_ID.FSP4_1];
                case EN_COMD_ID.FSP4_2: return MP.dPosn[(int)EN_POSN_ID.FSP4_2];
                case EN_COMD_ID.FSP4_3: return MP.dPosn[(int)EN_POSN_ID.FSP4_3];
                case EN_COMD_ID.FSP4_4: return MP.dPosn[(int)EN_POSN_ID.FSP4_4];
                case EN_COMD_ID.FSP4_5: return MP.dPosn[(int)EN_POSN_ID.FSP4_5];
                case EN_COMD_ID.FSP4_6: return MP.dPosn[(int)EN_POSN_ID.FSP4_6];
                case EN_COMD_ID.FSP4_7: return MP.dPosn[(int)EN_POSN_ID.FSP4_7];
                case EN_COMD_ID.FSP4_8: return MP.dPosn[(int)EN_POSN_ID.FSP4_8];
                case EN_COMD_ID.FSP4_9: return MP.dPosn[(int)EN_POSN_ID.FSP4_9];

                case EN_COMD_ID.LSP1_1: return MP.dPosn[(int)EN_POSN_ID.FSP1_1] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_2: return MP.dPosn[(int)EN_POSN_ID.FSP1_2] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_3: return MP.dPosn[(int)EN_POSN_ID.FSP1_3] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_4: return MP.dPosn[(int)EN_POSN_ID.FSP1_4] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_5: return MP.dPosn[(int)EN_POSN_ID.FSP1_5] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_6: return MP.dPosn[(int)EN_POSN_ID.FSP1_6] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_7: return MP.dPosn[(int)EN_POSN_ID.FSP1_7] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_8: return MP.dPosn[(int)EN_POSN_ID.FSP1_8] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP1_9: return MP.dPosn[(int)EN_POSN_ID.FSP1_9] + (iMaxSlot - 1) * dPitchP + dCenPitchP;

                case EN_COMD_ID.LSP2_1: return MP.dPosn[(int)EN_POSN_ID.FSP2_1] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_2: return MP.dPosn[(int)EN_POSN_ID.FSP2_2] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_3: return MP.dPosn[(int)EN_POSN_ID.FSP2_3] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_4: return MP.dPosn[(int)EN_POSN_ID.FSP2_4] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_5: return MP.dPosn[(int)EN_POSN_ID.FSP2_5] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_6: return MP.dPosn[(int)EN_POSN_ID.FSP2_6] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_7: return MP.dPosn[(int)EN_POSN_ID.FSP2_7] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_8: return MP.dPosn[(int)EN_POSN_ID.FSP2_8] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP2_9: return MP.dPosn[(int)EN_POSN_ID.FSP2_9] + (iMaxSlot - 1) * dPitchP + dCenPitchP;

                case EN_COMD_ID.LSP3_1: return MP.dPosn[(int)EN_POSN_ID.FSP3_1] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_2: return MP.dPosn[(int)EN_POSN_ID.FSP3_2] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_3: return MP.dPosn[(int)EN_POSN_ID.FSP3_3] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_4: return MP.dPosn[(int)EN_POSN_ID.FSP3_4] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_5: return MP.dPosn[(int)EN_POSN_ID.FSP3_5] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_6: return MP.dPosn[(int)EN_POSN_ID.FSP3_6] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_7: return MP.dPosn[(int)EN_POSN_ID.FSP3_7] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_8: return MP.dPosn[(int)EN_POSN_ID.FSP3_8] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP3_9: return MP.dPosn[(int)EN_POSN_ID.FSP3_9] + (iMaxSlot - 1) * dPitchP + dCenPitchP;

                case EN_COMD_ID.LSP4_1: return MP.dPosn[(int)EN_POSN_ID.FSP4_1] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_2: return MP.dPosn[(int)EN_POSN_ID.FSP4_2] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_3: return MP.dPosn[(int)EN_POSN_ID.FSP4_3] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_4: return MP.dPosn[(int)EN_POSN_ID.FSP4_4] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_5: return MP.dPosn[(int)EN_POSN_ID.FSP4_5] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_6: return MP.dPosn[(int)EN_POSN_ID.FSP4_6] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_7: return MP.dPosn[(int)EN_POSN_ID.FSP4_7] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_8: return MP.dPosn[(int)EN_POSN_ID.FSP4_8] + (iMaxSlot - 1) * dPitchP + dCenPitchP;
                case EN_COMD_ID.LSP4_9: return MP.dPosn[(int)EN_POSN_ID.FSP4_9] + (iMaxSlot - 1) * dPitchP + dCenPitchP;

                case EN_COMD_ID.Zero:    return 0.0;
                case EN_COMD_ID.Direct:  return DirPosn;
                case EN_COMD_ID.Pick1:   return MP.dPosn[(int)EN_POSN_ID.Pick1];
                case EN_COMD_ID.Pick2:   return MP.dPosn[(int)EN_POSN_ID.Pick2];
                case EN_COMD_ID.Plce1:   return MP.dPosn[(int)EN_POSN_ID.Plce1];
                case EN_COMD_ID.Plce2:   return MP.dPosn[(int)EN_POSN_ID.Plce2];
                case EN_COMD_ID.Sply:    return MP.dPosn[(int)EN_POSN_ID.Sply ];
                case EN_COMD_ID.Stck:    return MP.dPosn[(int)EN_POSN_ID.Stck ];

                case EN_COMD_ID.CalPos:  return MP.dPosn[(int)EN_POSN_ID.CalPos];

                case EN_COMD_ID.User1:   return MP.dPosn[(int)EN_POSN_ID.User1];
                case EN_COMD_ID.User2:   return MP.dPosn[(int)EN_POSN_ID.User2];
                case EN_COMD_ID.User3:   return MP.dPosn[(int)EN_POSN_ID.User3];
                case EN_COMD_ID.User4:   return MP.dPosn[(int)EN_POSN_ID.User4];
                case EN_COMD_ID.User5:   return MP.dPosn[(int)EN_POSN_ID.User5];
                case EN_COMD_ID.User6:   return MP.dPosn[(int)EN_POSN_ID.User6];
                case EN_COMD_ID.User7:   return MP.dPosn[(int)EN_POSN_ID.User7];
                case EN_COMD_ID.User8:   return MP.dPosn[(int)EN_POSN_ID.User8];
                case EN_COMD_ID.User9:   return MP.dPosn[(int)EN_POSN_ID.User9];
                case EN_COMD_ID.User10:  return MP.dPosn[(int)EN_POSN_ID.User10];
                case EN_COMD_ID.User11:  return MP.dPosn[(int)EN_POSN_ID.User11];
                case EN_COMD_ID.User12:  return MP.dPosn[(int)EN_POSN_ID.User12];
                case EN_COMD_ID.User13:  return MP.dPosn[(int)EN_POSN_ID.User13];
                case EN_COMD_ID.User14:  return MP.dPosn[(int)EN_POSN_ID.User14];
                case EN_COMD_ID.User15:  return MP.dPosn[(int)EN_POSN_ID.User15];
                case EN_COMD_ID.User16:  return MP.dPosn[(int)EN_POSN_ID.User16];
                case EN_COMD_ID.User17:  return MP.dPosn[(int)EN_POSN_ID.User17];
                case EN_COMD_ID.User18:  return MP.dPosn[(int)EN_POSN_ID.User18];
                case EN_COMD_ID.User19:  return MP.dPosn[(int)EN_POSN_ID.User19];
                case EN_COMD_ID.User20:  return MP.dPosn[(int)EN_POSN_ID.User20];
                case EN_COMD_ID.User21:  return MP.dPosn[(int)EN_POSN_ID.User21];
                case EN_COMD_ID.User22:  return MP.dPosn[(int)EN_POSN_ID.User22];
                case EN_COMD_ID.User23:  return MP.dPosn[(int)EN_POSN_ID.User23];
                case EN_COMD_ID.User24:  return MP.dPosn[(int)EN_POSN_ID.User24];
                case EN_COMD_ID.User25:  return MP.dPosn[(int)EN_POSN_ID.User25];
                case EN_COMD_ID.User26:  return MP.dPosn[(int)EN_POSN_ID.User26];
                case EN_COMD_ID.User27:  return MP.dPosn[(int)EN_POSN_ID.User27];
                case EN_COMD_ID.User28:  return MP.dPosn[(int)EN_POSN_ID.User28];
                case EN_COMD_ID.User29:  return MP.dPosn[(int)EN_POSN_ID.User29];
                case EN_COMD_ID.User30:  return MP.dPosn[(int)EN_POSN_ID.User30];

                default: return GetTrgPos();
            }
        }
        //--------------------------------------------------------------------------
        public int         GetPairMstrAxe   ()
        {
            return -1;
        }
        //--------------------------------------------------------------------------
        public int         GetPairAxe       ()
        {
            int iSubAxis;
            iSubAxis = MOTR[m_iObjNo].m_iPairAxisNo;
            return iSubAxis;
        }
        //--------------------------------------------------------------------------
        public bool        GetAbsAxe        ()
        {
            return false;
        }
        //Position Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public double      GetPreTrgPos     (             )                           //Get Target  Position.
        {
            if(!m_bInitAxis                                     ) return 0;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return 0; 
            return _fGPreTrgPos();
        }
        //--------------------------------------------------------------------------
        public double      GetTrgPos        (             )                           //Get Target  Position.
        {
            if(!m_bInitAxis                                ) return 0;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return 0; 
            return _fGTrgPos();
        }
        //--------------------------------------------------------------------------
        public double      GetCmdPos        (             )                           //Get Command Position.
        {                      
            if(!m_bInitAxis                                ) return 0;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return 0; 
            
            //Local Var.
            double cmdPos = 0.0;

            //Get.
            cmdPos = _fGetCmdPos(m_iObjNo);
            if(!IsMakerACS() && !IsMakerSMC()) cmdPos = CalPulseToPos(cmdPos); //JUNG/

            //Return.
            return Math.Round(cmdPos,4);
        }
        //--------------------------------------------------------------------------
        public double      GetEncPos        (             )                           //Get Encoder Position.
        {
            //Local Var.
            double encPos;

            if(!m_bInitAxis                                ) return 0;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return 0; 

            bool IsServo = m_iMotrType == (int)EN_MOTR_TYPE.Rotary || m_iMotrType == (int)EN_MOTR_TYPE.Linear;
            if (IsServo && m_iNoUseEnc == 0) encPos = _fGetEncPos(m_iObjNo);
            else 
            {
                encPos = _fGetCmdPos(m_iObjNo);
            }
            if(!IsMakerSMC()) encPos = CalPulseToPos(encPos);

            //Return.
            return Math.Round(encPos, 4);
        }
        //--------------------------------------------------------------------------
        public double      GetAbsEncPos     (             )                           //Get Encoder Position with Offset.
        {
            return (GetEncPos() + m_dHomeOff); 
        }
        //--------------------------------------------------------------------------
        public double      GetErrPos        (             )                           //Get Control Error Position.
        {
            return Math.Abs(GetCmdPos() - GetEncPos());
        }
        //--------------------------------------------------------------------------
        public void        ClearPos         (double Pos  = 0.0)                           //Clear Position.
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            double dPulse = (Pos * m_dCoef);
            _fClearPos(m_iObjNo, dPulse);
        }
        //--------------------------------------------------------------------------
        public bool        SetPos           (double Pos  = 0.0)                           //Set Any Position.
        {
            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 

            _fSetTrgPos   (Pos);
            _fSetPreTrgPos(Pos);

            double dPulse = (Pos * m_dCoef);
            return _fSetPos(m_iObjNo, dPulse);
        }
        //--------------------------------------------------------------------------
        public void        SetPosEncToCmd   (             )
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetPosEncToCmd(m_iObjNo);
        }
        //--------------------------------------------------------------------------
        public void        SetPosEncToTrg   ()
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            double encPos = _fGetEncPos(m_iObjNo);
            _fSetPreTrgPos(GetTrgPos());
            //_fSetTrgPos   (CalPulseToPos(encPos));
            _fSetTrgPos(encPos);
        }
        //--------------------------------------------------------------------------
        public void        SetPosCmdToTrg   (             )
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            double cmdPos = _fGetCmdPos(m_iObjNo);
            _fSetPreTrgPos(GetTrgPos());
            _fSetTrgPos(CalPulseToPos(cmdPos));
        }
        //--------------------------------------------------------------------------
        public void        SetPosEncToAll   (             )
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            double encPos = _fGetEncPos(m_iObjNo);

            //Set Command.
            _fSetPos(m_iObjNo, encPos);
        }
        //---------------------------------------------------------------------------
        public void SetJerk(double value)
        {
            _fSetJerk(m_iObjNo, value);
        }
        //--------------------------------------------------------------------------
        public double      GetOrgEncPos     ()
        {
            return 0.0;
            //double dOrgPos ;
            //if (m_iMaker != (int)EN_MOTR_MAKER.AJIN && m_iMaker != (int)EN_MOTR_MAKER.AJECAT) return 0.0;
            //dOrgPos = AJIN.m_dOrgEncPos;
            //return CalPulseToPos(dOrgPos);
        }     
        //--------------------------------------------------------------------------   
        //Limit Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //Get.
        public double GetMaxPos()            { return MP.MaxPos; }
        public double GetMaxVel()            { return MP.MaxVel; }
        public double GetMaxAcc()            { return MP.MaxAcc; }
        public double GetMinPos()            { return MP.MinPos; }
        public double GetMinVel()            { return MP.MinVel; }
        public double GetMinAcc()            { return MP.MinAcc; }
            //Set.                             
        public void   SetMaxPos(double Data) { MP.MaxPos = Data; }
        public void   SetMaxVel(double Data) { MP.MaxVel = Data; }
        public void   SetMaxAcc(double Data) { MP.MaxAcc = Data; }
        public void   SetMinPos(double Data) { MP.MinPos = Data; }
        public void   SetMinVel(double Data) { MP.MinVel = Data; }
        public void   SetMinAcc(double Data) { MP.MinAcc = Data; }
        public void   SetMaxMin(double MinPos , double MaxPos, double MinVel, double MaxVel, double MinAcc, double MaxAcc)
        {
            MP.MaxPos = MaxPos;
            MP.MaxVel = MaxVel;
            MP.MaxAcc = MaxAcc;
            MP.MinPos = MinPos;
            MP.MinVel = MinVel;
            MP.MinAcc = MinAcc;
        }
        //--------------------------------------------------------------------------
        //Move Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetServo(bool On)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            if (m_iNoUseMotr==1) SetHomeEnd(true);

            if (MOTR[m_iObjNo].IsPairSlave()) return;

            Reset();

            //Servo On/Off.
            if (m_iMotrType == (int)EN_MOTR_TYPE.StepOriental)
            {
                if (m_iSONLevel==1) { _fSetServo(m_iObjNo, (!On) ? 1:0); }
                else                { _fSetServo(m_iObjNo, ( On) ? 1:0); }
            }
            else
            {
                if (m_iSONLevel==1) { _fSetServo(m_iObjNo,  On ? 1 : 0); }
                else                { _fSetServo(m_iObjNo,  On ? 0 : 1); }
            }

            if (m_iMaker != (int)EN_MOTR_MAKER.AJIN && m_iMaker != (int)EN_MOTR_MAKER.AJECAT) Stop();
        }
        //--------------------------------------------------------------------------
        public void SetAlarm(bool on)
        {
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 
            _fSetAlarm(m_iObjNo, (on)? 1: 0);
        }   
        //--------------------------------------------------------------------------   
        public bool        EmrgStop         (  )
        {
            bool isOk = false;

            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 

            //Reset Jog Status.
            m_bJogN    = false;
            m_bJogP    = false;
            m_bTorqueN = false;
            m_bTorqueP = false;

            //E-Stop.
            if(_fEmrgStop != null) isOk = _fEmrgStop(m_iObjNo);

            //if (isOk) {
            //    SetPosEncToAll();
            //    }
            return isOk;
        } 
        //--------------------------------------------------------------------------       
        public bool Stop(bool   DecStop = false , double DecTime = 0.1)
        {
            bool isOk = false;

            if(!m_bInitAxis                                ) return true;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return true; 

            //Reset Jog Status.
            m_bJogN = false;
            m_bJogP = false;


            if (m_bUseTorque)
            {
                if (m_bLtTorqueN || m_bLtTorqueP)
                {
                    MoveTorqueStop();
                    return true;
                }
            }
            //Stop.
            isOk = _fStop(m_iObjNo, DecStop, DecTime);

            //if (isOk) {
            //    SetPosEncToAll();
            //    }

            return isOk;
        }
        //--------------------------------------------------------------------------
        public bool MoveJog(bool Dir, EN_MOTR_VEL iSPD = EN_MOTR_VEL.Normal)
        {
            //Move JOG.

            if (iSPD == EN_MOTR_VEL.Normal)
            {
                if (m_bJogSpd)
                {
                    if (Dir) { return MoveJogP(MP.dVel[(int)EN_MOTR_VEL.HJog]); }
                    else     { return MoveJogN(MP.dVel[(int)EN_MOTR_VEL.HJog]); }
                }
                else
                {
                    if (Dir) { return MoveJogP(MP.dVel[(int)EN_MOTR_VEL.LJog]); }
                    else     { return MoveJogN(MP.dVel[(int)EN_MOTR_VEL.LJog]); }
                }
            }
            else
            {
                if (iSPD < 0 || (int)iSPD >= MAX_SPED) return false;
                if (Dir) { return MoveJogP(MP.dVel[(int)iSPD]); }
                else     { return MoveJogN(MP.dVel[(int)iSPD]); }
            }
        }
        //-------------------------------------------------------------------------------------------------
        public bool MoveJog(bool Dir, double dVel)
        {
            //Move JOG.
            if (Dir) { return MoveJogP(dVel); }
            else     { return MoveJogN(dVel); }
        }
        //--------------------------------------------------------------------------
        public bool        MoveHome         (                                         )
        {
            //Local Val
            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 

            //Check use.

            if (m_iHomeType == (int)EN_HOME_TYPE.DataSet) _fSetPos(m_iObjNo, 0.0);
            if (m_iNoUseMotr==1 || m_iNoUseHomeSen == 1)
            {
                _fSetPos(m_iObjNo, CalPosToPulse(m_dHomeOff));
                _fSetHomeEnd(true);
                return true;
            }
            //Reset Jog Status.
            m_bJogN = false;
            m_bJogP = false;

            if (GetHomeEnd()) return true;

            //Move Home Start
            _fMoveHome(m_iObjNo, MP.dVel[(int)EN_MOTR_VEL.Home], MP.dAcc[(int)EN_MOTR_VEL.Work], MP.dDec[(int)EN_MOTR_VEL.Work], 
                        CalPosToPulse(m_dHomeOff), m_dHomeOff);

            if (m_iMotrKind == (int)EN_MOTR_KIND.ABS || m_iHomeType == (int)EN_HOME_TYPE.DataSet)
            {
                _fSetPreTrgPos(GetEncPos());
                _fSetTrgPos(GetEncPos());
                
            }
            //Check Home End
            return (GetHomeStep() == 0);
        }
        //--------------------------------------------------------------------------
        public bool        MoveHomeForce    ()
        {
            return true; 

//             //Local Val
//             if(!m_bInitAxis                                ) return false;
//             if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 
// 
//             if(m_iMaker != (int)EN_MOTR_MAKER.AJIN && m_iMaker != (int)EN_MOTR_MAKER.AJECAT) return false;
//             
//             //Check use.
//             if (m_iNoUseMotr==1 || m_iNoUseHomeSen == 1)
//             {
//                 _fSetHomeEnd(true);
//                 return true;
//                 }
// 
//             //Reset Jog Status.
//             m_bJogN = false;
//             m_bJogP = false;
// 
//             if(GetHomeEnd()) return true;

//             //Move Home Start
//             AJIN.MoveHomeForce(m_iObjNo, MP.dVel[(int)EN_MOTR_VEL.Home], MP.dAcc[(int)EN_MOTR_VEL.Work]);
// 
//             if (m_iMotrKind == (int)EN_MOTR_KIND.ABS) {
//                 _fSetPreTrgPos(GetEncPos());
//                 _fSetTrgPos(GetEncPos());
// 
//             }
//             //Check Home End
//             return (GetHomeStep() == 0);
        }
        //--------------------------------------------------------------------------
        public bool MoveOverrideVel  (double Pos , double Vel , double Acc , double Dec, double dOverridePos, double dOverrideVelocity)
        {
            return true; 

//             //Local Var.
//             bool   Receive   ;
//             double tPos ,tOverridePos;
// 
// 
//             //Apply Coef.
//             tPos         = CalPosToPulse(Pos         );
//             tOverridePos = CalPosToPulse(dOverridePos);
// 
//             //Set Gain.
//                 //Speed.
//             if (m_dSpedGain < 0.01) m_dSpedGain = 0.01;
//             if (m_dSpedGain > 2.0 ) m_dSpedGain = 2.0 ;
//             Vel *= m_dSpedGain;
//             m_dSpedGain = 1.0;
//                 //Acc.
//             if (m_dAccGain  < 0.01 ) m_dAccGain  = 0.01;
//             if (m_dAccGain  > 5.0  ) m_dAccGain  = 5.0 ;
//             Acc *= m_dAccGain;
//             Dec *= m_dAccGain;
//             m_dAccGain = 1.0;
// 
// 
//             //Set Speed Ratio.
//             switch ((EN_SPED_RATIO)m_iSpedRatio) {
//                 case EN_SPED_RATIO.sr100: Vel *= 1.0; break;
//                 case EN_SPED_RATIO.sr90 : Vel *= 0.9; break;
//                 case EN_SPED_RATIO.sr80 : Vel *= 0.8; break;
//                 case EN_SPED_RATIO.sr70 : Vel *= 0.7; break;
//                 case EN_SPED_RATIO.sr60 : Vel *= 0.6; break;
//                 case EN_SPED_RATIO.sr50 : Vel *= 0.5; break;
//                 case EN_SPED_RATIO.sr40 : Vel *= 0.4; break;
//                 case EN_SPED_RATIO.sr30 : Vel *= 0.3; break;
//                 case EN_SPED_RATIO.sr20 : Vel *= 0.2; break;
//                 case EN_SPED_RATIO.sr10 : Vel *= 0.1; break;
//                 default                 : Vel *= 1.0; break;
//                 }
// 
//             //Check Motor Limit.
//             if (Dec <= 0       ) { Dec = Acc ;                 }
//             if (!GetHomeEnd()  ) { Stop(true); return false;   }
//             if (Pos > MP.MaxPos) { Stop(true); return false;   }
//             if (Pos < MP.MinPos) { Stop(true); return false;   }
//             if (Vel > MP.MaxVel) { Vel = MP.MaxVel;            }
//             if (Vel < MP.MinVel) { Vel = MP.MinVel;            }
//             if (Acc > MP.MaxAcc) { Acc = MP.MaxAcc;            }
//             if (Acc < MP.MinAcc) { Acc = MP.MinAcc;            }
//             if (Dec > MP.MaxAcc) { Dec = MP.MaxAcc;            }
//             if (Dec < MP.MinAcc) { Dec = MP.MinAcc;            }
//             if (dOverrideVelocity > MP.MaxVel) { dOverrideVelocity = MP.MaxVel; }
//             if (dOverrideVelocity < MP.MinVel) { dOverrideVelocity = MP.MinVel; }
// 
//             if(!m_bInitAxis                                ) return true;
//             if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return true; 
//             if(m_iNoUseMotr==1                             ) return true;
//             if(m_iMaker != (int)EN_MOTR_MAKER.AJIN && m_iMaker != (int)EN_MOTR_MAKER.AJECAT) return true;
// 
//             //Reset Jog Status.
//             m_bJogN    = false;
//             m_bJogP    = false;
//             m_bTorqueN = false;
//             m_bTorqueP = false;
// 
//             //Move Motor.
//             if (!GetOk()) return false;
//             else {
//                 Receive = false;
//                 if (_fGStop()) {
//                     //dwStrtTime = GetTickTime_us();
//                     if(m_iMaker == (int)EN_MOTR_MAKER.AJIN) {
//                         Receive = AJIN.MoveOverrideVel(m_iObjNo, tPos, Vel, Acc, Dec, tOverridePos, dOverrideVelocity);
//                         }
//                     //dwScanTime = GetTickTime_us() - dwStrtTime;
//                     if (!m_bRising) {
//                         //dwMoveStrtTime = GetTickTime();
//                         m_bRising = true;
//                         }
//                     }
//                 else {
//                     if(m_bUseOverride) {
//                         //Receive = MoveOverride(m_iObjNo, tPos, Vel, Acc, Dec);
//                         }
//                     }
// 
//                 if (Receive) { 
//                     _fSetPreTrgPos(GetTrgPos());
//                     _fSetTrgPos(Pos);
//                     }
//                 }
// 
//             //Timer Clear.
//             m_MoveInpTimer.Clear();
// 
//             //Check InPos.
//             return GetStop(true , m_dInPos);
        }
        //--------------------------------------------------------------------------
        public bool MoveJogP(double Vel)
        {
            //Check Status.
            if (Vel     <= 0) return false;
            if (GetAlarm  ()) return false;
            if (!GetServo ()) return false;

            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 

            //Set Jog Status.
            m_bJogP    = true ;
            m_bTorqueN = false;
            m_bTorqueP = false;

            //
            return _fMoveJogP(m_iObjNo, Vel, MP.dAcc[(int)EN_MOTR_VEL.Work], MP.dDec[(int)EN_MOTR_VEL.Work]);
        }
        //--------------------------------------------------------------------------
        public bool MoveJogN         (double Vel)
        {
            //Check Status.
            if (m_iNoUseMotr==1) return false;
            if (Vel <= 0       ) return false;
            if (GetAlarm ()    ) return false;
            if (!GetServo()    ) return false;


            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 

            //Set Jog Status.
            m_bJogN    = true ;
            m_bTorqueN = false;
            m_bTorqueP = false;

            return _fMoveJogN(m_iObjNo, Vel, MP.dAcc[(int)EN_MOTR_VEL.Work], MP.dDec[(int)EN_MOTR_VEL.Work]);
        }
        //--------------------------------------------------------------------------
        public bool MoveA (double Pos , double Vel = 20.0, double Acc = 0.3 , double Dec = 0.0)  //abs move with mm.
        {
            //Local Var.
            bool   Receive   ;
            double tPos      ;

            //Apply Coef.
            //tPos = CalPosToPulse(Pos);
            tPos = Pos;

            //Set Gain.
            //Speed.
            if (m_dSpedGain < 0.01) m_dSpedGain = 0.01;
            if (m_dSpedGain > 2.0 ) m_dSpedGain = 2.0 ;
            Vel *= m_dSpedGain;
            m_dSpedGain = 1.0;
                //Acc.
            if (m_dAccGain  < 0.01 ) m_dAccGain  = 0.01;
            if (m_dAccGain  > 5.0  ) m_dAccGain  = 5.0 ;
            Acc *= m_dAccGain;
            Dec *= m_dAccGain;
            m_dAccGain = 1.0;


            //Set Speed Ratio.
            switch ((EN_SPED_RATIO)m_iSpedRatio)
            {
                case EN_SPED_RATIO.sr100: Vel *= 1.0; break;
                case EN_SPED_RATIO.sr90 : Vel *= 0.9; break;
                case EN_SPED_RATIO.sr80 : Vel *= 0.8; break;
                case EN_SPED_RATIO.sr70 : Vel *= 0.7; break;
                case EN_SPED_RATIO.sr60 : Vel *= 0.6; break;
                case EN_SPED_RATIO.sr50 : Vel *= 0.5; break;
                case EN_SPED_RATIO.sr40 : Vel *= 0.4; break;
                case EN_SPED_RATIO.sr30 : Vel *= 0.3; break;
                case EN_SPED_RATIO.sr20 : Vel *= 0.2; break;
                case EN_SPED_RATIO.sr10 : Vel *= 0.1; break;
                default: Vel *= 1.0; break;
            }


            //Check Motor Limit.
            if (Dec <= 0       ) { Dec = Acc; }
            if (!GetHomeEnd()  ) { Stop(true); return false; }
            if (Pos > MP.MaxPos) { Stop(true); return false; }
            if (Pos < MP.MinPos) { Stop(true); return false; }
            if (Vel > MP.MaxVel) { Vel = MP.MaxVel; }
            if (Vel < MP.MinVel) { Vel = MP.MinVel; }
            if (Acc > MP.MaxAcc) { Acc = MP.MaxAcc; }
            if (Acc < MP.MinAcc) { Acc = MP.MinAcc; }
            if (Dec > MP.MaxAcc) { Dec = MP.MaxAcc; }
            if (Dec < MP.MinAcc) { Dec = MP.MinAcc; }

            if(!m_bInitAxis                                ) return true;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return true; 
            if(m_iNoUseMotr==1                             ) return true;

            //Reset Jog Status.
            m_bJogN    = false;
            m_bJogP    = false;
            m_bTorqueN = false;
            m_bTorqueP = false;

            //bool bStop = *m_pbStop || fabs(GetEncPos() - Pos) < m_dInPos;

            //Move Motor.
            if (!GetOk()) return false;
            Receive = false;
            if (_fGStop()) 
            {
                //dwStrtTime = GetTickTime_us();
                Receive = _fMove(m_iObjNo, tPos, Vel, Acc, Dec, m_iSpedRatio);
                //dwScanTime = GetTickTime_us() - dwStrtTime;
                if (!m_bRising) m_bRising = true;
            }
            else if(m_bUseOverride) 
            {
                //if(isMakerAjin()) Receive = AJIN.MoveOverride(m_iObjNo, tPos, Vel, Acc, Dec);                
            }

            if (Receive) 
            {  
                _fSetPreTrgPos(GetTrgPos());
                _fSetTrgPos(Pos);
            }
            //Timer Clear.
            m_MoveInpTimer.Clear();
            //Check InPos.
            return GetStop(true , m_dInPos);
        }    
        //--------------------------------------------------------------------------   
        public bool MoveP (double Pos , double Vel = 20.0, double Acc = 0.3)  //Position over ride motion.
        {
            //Local Var.
            double cPos = GetCmdPos();

            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 
            if(m_iNoUseMotr==1                             ) return true;

            //Reset Jog Status.
            m_bJogN    = false;
            m_bJogP    = false;
            m_bTorqueN = false;
            m_bTorqueP = false;

            //Move Relative.
            return MoveP(Pos + cPos, Vel, Acc);
        }
        //--------------------------------------------------------------------------
        public bool MoveR (double Pos , double Vel = 20.0, double Acc = 0.3                   )  
        {
            //Local Var.
            double cPos = GetCmdPos();

            if(!m_bInitAxis                                ) return false;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return false; 
            if(m_iNoUseMotr==1                             ) return true ;

            //Reset Jog Status.
            m_bJogN    = false;
            m_bJogP    = false;
            m_bTorqueN = false;
            m_bTorqueP = false;

            //Move Relative.
            return ACS.fn_MoveRelativePosition(m_iObjNo, Pos, true, Vel, Vel, Vel);
        }
        //--------------------------------------------------------------------------
        //Ring Counter Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetRingCounter  (bool Enable , double Max)
        {
            //Local Var.
            double MaxCntr;

            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            //Set Var.
            m_dRingMax = Max;
            MaxCntr = Max * m_dCoef;

            _fSetRingCounter(m_iObjNo, Enable, MaxCntr);
        }
        //--------------------------------------------------------------------------
        //Min-Max
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool CheckMinMax (double P , double V , double A) //Value Checking.
        {
            //Check.
            if (m_iNoUseMotr==1 ) return true;
            if (!CheckMinMaxP(P)) return false;
            //if (!CheckMinMaxV(V)) return false;
            //if (!CheckMinMaxA(A)) return false;

            //Ok.
            return true;
        }
        //--------------------------------------------------------------------------
        public bool  CheckMinMaxP(double P)
        {
            //Check.
            if (m_iNoUseMotr == 1                     ) return true;
            if ((GetMaxPos() < P) || (GetMinPos() > P)) return false;

            //Ok.
            return true;
        }
        //--------------------------------------------------------------------------
        public bool CheckMinMaxV (double V)
        {
            //Check.
            if (m_iNoUseMotr==1                       ) return true;
            if ((GetMaxVel() < V) || (GetMinVel() > V)) return false;
            //Ok.
            return true;
        }
        //--------------------------------------------------------------------------
        public bool        CheckMinMaxA     (double A                      )
        {
            //Check.
            if (m_iNoUseMotr==1                       ) return true;
            if ((GetMaxAcc() < A) || (GetMinAcc() > A)) return false;

            //Ok.
            return true;
        }
        //--------------------------------------------------------------------------
        public bool        CheckMinMax      (int    P , int    V           ) //mpPara Checking.
        {
            //Check Spare ID.
            if (m_iNoUseMotr==1 ) return true;

            //Check.
            if (!CheckMinMaxP(P)) return false;
            if (!CheckMinMaxV(V)) return false;
            if (!CheckMinMaxA(V)) return false;

            //Ok.
            return true;
        }
        //--------------------------------------------------------------------------
        public bool        CheckMinMaxP     (int    P                      )
        {
            if (P < 0 || P >= MAX_POSN) return false;
            if (m_iNoUseMotr==1 ) return true;
            return CheckMinMaxP(MP.dPosn[P]);
        }
        //--------------------------------------------------------------------------
        public bool        CheckMinMaxV     (int    V                      )
        {
            //Check Spare ID.
            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return true;
            if (m_iNoUseMotr==1) return true;
            if (V >= MAX_SPED  ) return true;

            //Check.
            return CheckMinMaxV(MP.dVel[V]);
        }
        //--------------------------------------------------------------------------
        public bool        CheckMinMaxA     (int    V                      )
        {
            //Check Motor Axe.
            if (m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return true;

            if (V >= MAX_SPED   ) return true;
            if (m_iNoUseMotr==1 ) return true;
            //Check.       
            return CheckMinMaxA(MP.dAcc[V]);
        }
        //--------------------------------------------------------------------------
        public bool CheckPartMotr (int Part)
        {
            return m_iMotionPart == Part;
        }
        //Compare position.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool  CmprPos (double Pos)
        {
            bool isOk;
            double dCortPos = 0.0;

            if (m_iNoUseMotr==1) return true;

            Pos -= dCortPos;

            isOk = (Math.Abs(GetEncPos() - Pos) < MP.dPosn[(int)EN_POSN_ID.InPos]) &&
                   (Math.Abs(GetCmdPos() - Pos) < MP.dPosn[(int)EN_POSN_ID.InPos]);

            if (isOk)
            {
                SetGain(1.0, 1.0);
            }
            return isOk;
        }
        //--------------------------------------------------------------------------
        public bool CmprPos(double Pos, double InPos)
        {
            bool isOk;

            double dCortPos = 0.0;
            Pos -= dCortPos;

            if (m_iNoUseMotr==1) return true;

            isOk = (Math.Abs(GetEncPos() - Pos) < InPos) &&
                   (Math.Abs(GetCmdPos() - Pos) < InPos);

            if (isOk)
            {
                SetGain(1.0, 1.0);
            }

            return isOk;
        }
        //--------------------------------------------------------------------------
        public int CmprPosStat(double Pos  , bool ChkTrg = false)
        {
            double dGetPos = ChkTrg ? GetTrgPos() : GetEncPos();

            if (m_iNoUseMotr==1) return CMPR_SAME;

            if (dGetPos < Pos  ) return CMPR_SMAL;
            if (dGetPos == Pos ) return CMPR_SAME;
            if (dGetPos > Pos  ) return CMPR_LARG;

            return CMPR_SAME;
        }
        //--------------------------------------------------------------------------
        public int CmprPosStat (double Pos, double Offset, bool ChkTrg= false)
        {
            double dGetPos = ChkTrg ? GetTrgPos() : GetEncPos();

            if (m_iNoUseMotr==1           ) return CMPR_SAME;

            if (dGetPos < (Pos - Offset)  ) return CMPR_SMAL;
            if (dGetPos > (Pos + Offset)  ) return CMPR_LARG;
            if ((dGetPos >= (Pos - Offset)) && (dGetPos <= (Pos + Offset))) return CMPR_SAME;

            return CMPR_SAME;
        }
        //--------------------------------------------------------------------------
        public int CmprPosStat(int    PosID, bool ChkTrg = false)
        {
            double dGetPos = ChkTrg ? GetTrgPos() : GetEncPos();

            if (PosID < 0 || PosID >= MAX_POSN) return 0;
            if (m_iNoUseMotr==1                    ) return 0;

            if ( dGetPos <  (MP.dPosn[PosID] - MP.dPosn[(int)EN_POSN_ID.InPos])) return CMPR_SMAL;
            if ( dGetPos >  (MP.dPosn[PosID] + MP.dPosn[(int)EN_POSN_ID.InPos])) return CMPR_LARG;
            if ((dGetPos >= (MP.dPosn[PosID] - MP.dPosn[(int)EN_POSN_ID.InPos])) && (dGetPos <= (MP.dPosn[PosID] + MP.dPosn[(int)EN_POSN_ID.InPos]))) return CMPR_SAME;

            return CMPR_SAME;
        }
        //--------------------------------------------------------------------------
        public bool CmprStep(int Step, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.Index1)
        {
            int iPosnID;

            if (m_iNoUseMotr==1) return true;

                 if (FStep == EN_FSTEP_INDEX.Step1) iPosnID = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step2) iPosnID = (int)EN_POSN_ID.FSP2_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step3) iPosnID = (int)EN_POSN_ID.FSP3_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step4) iPosnID = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;
            else                                    iPosnID = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;

            if ((iPosnID + FIndex) < 0 || (iPosnID + (int)FIndex) >= MAX_POSN) return false;

            if (m_iProcessDir==1) return CmprPos((MP.dPosn[iPosnID] + (double)Step * GetPitch(FStep) + GetCenPitch(Step, FStep)));
            else                  return CmprPos((MP.dPosn[iPosnID] - (double)Step * GetPitch(FStep) - GetCenPitch(Step, FStep)));
        }
        //--------------------------------------------------------------------------
        public bool CmprStepF(int Step, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.Index1)
        {
            int iPosnID;

            if (m_iNoUseMotr==1) return true;

                 if (FStep == EN_FSTEP_INDEX.Step1) iPosnID = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step2) iPosnID = (int)EN_POSN_ID.FSP2_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step3) iPosnID = (int)EN_POSN_ID.FSP3_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step4) iPosnID = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;
            else                                    iPosnID = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;

            if ((iPosnID + FIndex) < 0 || (iPosnID + (int)FIndex) >= MAX_POSN) return false;


            return CmprPos((MP.dPosn[iPosnID] + (double)Step * GetPitch(FStep) + GetCenPitch(Step, FStep)));
        }
        //--------------------------------------------------------------------------
        public bool CmprStepB(int Step, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.Index1)
        {
            int iPosnID;

            if (m_iNoUseMotr==1) return true;

                 if (FStep == EN_FSTEP_INDEX.Step1) iPosnID = (int)EN_POSN_ID.FSP1_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step2) iPosnID = (int)EN_POSN_ID.FSP2_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step3) iPosnID = (int)EN_POSN_ID.FSP3_1 + (int)FIndex;
            else if (FStep == EN_FSTEP_INDEX.Step4) iPosnID = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;
            else                                    iPosnID = (int)EN_POSN_ID.FSP4_1 + (int)FIndex;

            if ((iPosnID + FIndex) < 0 || (iPosnID + (int)FIndex) >= MAX_POSN) return false;

            return CmprPos((MP.dPosn[iPosnID] + (double)Step * GetPitch(FStep) + GetCenPitch(Step, FStep)) - GetPitch(FStep));
        }
        //--------------------------------------------------------------------------
        public bool CmprArea (double Pos    , double Area , bool NoChkEnc = false)
        {
            bool isOk;

            double dCortPos = 0.0;
            Pos -= dCortPos;


            if (m_iNoUseMotr==1) return true;

            if (NoChkEnc) isOk = (Math.Abs(GetTrgPos() - Pos) < Area) && Math.Abs(GetCmdPos() - Pos) < Area;
            else          isOk = (Math.Abs(GetEncPos() - Pos) < Area) && Math.Abs(GetCmdPos() - Pos) < Area;

            return isOk;
        }
        //--------------------------------------------------------------------------
        public int WhreTarget() 
        {
            if (m_iNoUseMotr==1) return MOTR_TARG_STP;

                 if (GetTrgPos() > (GetEncPos() + MP.dPosn[(int)EN_POSN_ID.InPos])) return MOTR_TARG_POS;
            else if (GetTrgPos() < (GetEncPos() - MP.dPosn[(int)EN_POSN_ID.InPos])) return MOTR_TARG_NEG;
            else if (GetTrgPos() == GetCmdPos())                                    return MOTR_TARG_STP;

            return MOTR_TARG_STP;
        }
        //--------------------------------------------------------------------------
        public EN_POSN_ID GetCmdToPosnId(EN_COMD_ID Comd)
        {
            EN_POSN_ID iPosnId = EN_POSN_ID.None;

            //
            //if (m_iNoUseMotr==1) return iPosnId;

            switch (Comd)
            {
                default:                 iPosnId = EN_POSN_ID.None ;   break;
                case EN_COMD_ID.User1:   iPosnId = EN_POSN_ID.User1;   break;
                case EN_COMD_ID.User2:   iPosnId = EN_POSN_ID.User2;   break;
                case EN_COMD_ID.User3:   iPosnId = EN_POSN_ID.User3;   break;
                case EN_COMD_ID.User4:   iPosnId = EN_POSN_ID.User4;   break;
                case EN_COMD_ID.User5:   iPosnId = EN_POSN_ID.User5;   break;
                case EN_COMD_ID.User6:   iPosnId = EN_POSN_ID.User6;   break;
                case EN_COMD_ID.User7:   iPosnId = EN_POSN_ID.User7;   break;
                case EN_COMD_ID.User8:   iPosnId = EN_POSN_ID.User8;   break;
                case EN_COMD_ID.User9:   iPosnId = EN_POSN_ID.User9;   break;
                case EN_COMD_ID.User10:  iPosnId = EN_POSN_ID.User10;  break;
                case EN_COMD_ID.User11:  iPosnId = EN_POSN_ID.User11;  break;
                case EN_COMD_ID.User12:  iPosnId = EN_POSN_ID.User12;  break;
                case EN_COMD_ID.User13:  iPosnId = EN_POSN_ID.User13;  break;
                case EN_COMD_ID.User14:  iPosnId = EN_POSN_ID.User14;  break;
                case EN_COMD_ID.User15:  iPosnId = EN_POSN_ID.User15;  break;
                case EN_COMD_ID.User16:  iPosnId = EN_POSN_ID.User16;  break;
                case EN_COMD_ID.User17:  iPosnId = EN_POSN_ID.User17;  break;
                case EN_COMD_ID.User18:  iPosnId = EN_POSN_ID.User18;  break;
                case EN_COMD_ID.User19:  iPosnId = EN_POSN_ID.User19;  break;
                case EN_COMD_ID.User20:  iPosnId = EN_POSN_ID.User20;  break;
                case EN_COMD_ID.User21:  iPosnId = EN_POSN_ID.User21;  break;
                case EN_COMD_ID.User22:  iPosnId = EN_POSN_ID.User22;  break;
                case EN_COMD_ID.User23:  iPosnId = EN_POSN_ID.User23;  break;
                case EN_COMD_ID.User24:  iPosnId = EN_POSN_ID.User24;  break;
                case EN_COMD_ID.User25:  iPosnId = EN_POSN_ID.User25;  break;
                case EN_COMD_ID.User26:  iPosnId = EN_POSN_ID.User26;  break;
                case EN_COMD_ID.User27:  iPosnId = EN_POSN_ID.User27;  break;
                case EN_COMD_ID.User28:  iPosnId = EN_POSN_ID.User28;  break;
                case EN_COMD_ID.User29:  iPosnId = EN_POSN_ID.User29;  break;
                case EN_COMD_ID.User30:  iPosnId = EN_POSN_ID.User30;  break;
                case EN_COMD_ID.Wait1 :  iPosnId = EN_POSN_ID.Wait1;   break;
                case EN_COMD_ID.Wait2 :  iPosnId = EN_POSN_ID.Wait2;   break;
                case EN_COMD_ID.FSP1_1:  iPosnId = EN_POSN_ID.FSP1_1;  break;
                case EN_COMD_ID.FSP1_2:  iPosnId = EN_POSN_ID.FSP1_2;  break;
                case EN_COMD_ID.FSP1_3:  iPosnId = EN_POSN_ID.FSP1_3;  break;
                case EN_COMD_ID.FSP1_4:  iPosnId = EN_POSN_ID.FSP1_4;  break;
                case EN_COMD_ID.FSP1_5:  iPosnId = EN_POSN_ID.FSP1_5;  break;
                case EN_COMD_ID.FSP1_6:  iPosnId = EN_POSN_ID.FSP1_6;  break;
                case EN_COMD_ID.FSP1_7:  iPosnId = EN_POSN_ID.FSP1_7;  break;
                case EN_COMD_ID.FSP1_8:  iPosnId = EN_POSN_ID.FSP1_8;  break;
                case EN_COMD_ID.FSP1_9:  iPosnId = EN_POSN_ID.FSP1_9;  break;
                                                                       
                case EN_COMD_ID.FSP2_1:  iPosnId = EN_POSN_ID.FSP2_1;  break;
                case EN_COMD_ID.FSP2_2:  iPosnId = EN_POSN_ID.FSP2_2;  break;
                case EN_COMD_ID.FSP2_3:  iPosnId = EN_POSN_ID.FSP2_3;  break;
                case EN_COMD_ID.FSP2_4:  iPosnId = EN_POSN_ID.FSP2_4;  break;
                case EN_COMD_ID.FSP2_5:  iPosnId = EN_POSN_ID.FSP2_5;  break;
                case EN_COMD_ID.FSP2_6:  iPosnId = EN_POSN_ID.FSP2_6;  break;
                case EN_COMD_ID.FSP2_7:  iPosnId = EN_POSN_ID.FSP2_7;  break;
                case EN_COMD_ID.FSP2_8:  iPosnId = EN_POSN_ID.FSP2_8;  break;
                case EN_COMD_ID.FSP2_9:  iPosnId = EN_POSN_ID.FSP2_9;  break;
                                                                       
                case EN_COMD_ID.FSP3_1:  iPosnId = EN_POSN_ID.FSP3_1;  break;
                case EN_COMD_ID.FSP3_2:  iPosnId = EN_POSN_ID.FSP3_2;  break;
                case EN_COMD_ID.FSP3_3:  iPosnId = EN_POSN_ID.FSP3_3;  break;
                case EN_COMD_ID.FSP3_4:  iPosnId = EN_POSN_ID.FSP3_4;  break;
                case EN_COMD_ID.FSP3_5:  iPosnId = EN_POSN_ID.FSP3_5;  break;
                case EN_COMD_ID.FSP3_6:  iPosnId = EN_POSN_ID.FSP3_6;  break;
                case EN_COMD_ID.FSP3_7:  iPosnId = EN_POSN_ID.FSP3_7;  break;
                case EN_COMD_ID.FSP3_8:  iPosnId = EN_POSN_ID.FSP3_8;  break;
                case EN_COMD_ID.FSP3_9:  iPosnId = EN_POSN_ID.FSP3_9;  break;
                                                                       
                case EN_COMD_ID.FSP4_1:  iPosnId = EN_POSN_ID.FSP4_1;  break;
                case EN_COMD_ID.FSP4_2:  iPosnId = EN_POSN_ID.FSP4_2;  break;
                case EN_COMD_ID.FSP4_3:  iPosnId = EN_POSN_ID.FSP4_3;  break;
                case EN_COMD_ID.FSP4_4:  iPosnId = EN_POSN_ID.FSP4_4;  break;
                case EN_COMD_ID.FSP4_5:  iPosnId = EN_POSN_ID.FSP4_5;  break;
                case EN_COMD_ID.FSP4_6:  iPosnId = EN_POSN_ID.FSP4_6;  break;
                case EN_COMD_ID.FSP4_7:  iPosnId = EN_POSN_ID.FSP4_7;  break;
                case EN_COMD_ID.FSP4_8:  iPosnId = EN_POSN_ID.FSP4_8;  break;
                case EN_COMD_ID.FSP4_9:  iPosnId = EN_POSN_ID.FSP4_9;  break;

                case EN_COMD_ID.Pick1:   iPosnId = EN_POSN_ID.Pick1;   break;
                case EN_COMD_ID.Pick2:   iPosnId = EN_POSN_ID.Pick2;   break;
                case EN_COMD_ID.Plce1:   iPosnId = EN_POSN_ID.Plce1;   break;
                case EN_COMD_ID.Plce2:   iPosnId = EN_POSN_ID.Plce2;   break;
                case EN_COMD_ID.PickInc: iPosnId = EN_POSN_ID.PickInc; break;
                case EN_COMD_ID.PlceInc: iPosnId = EN_POSN_ID.PlceInc; break;
                case EN_COMD_ID.Sply:    iPosnId = EN_POSN_ID.Sply;    break;
                case EN_COMD_ID.Stck:    iPosnId = EN_POSN_ID.Stck;    break;
                case EN_COMD_ID.CalPos:  iPosnId = EN_POSN_ID.CalPos;  break;
            }
            return iPosnId;
        }
        //--------------------------------------------------------------------------
        public double GetPosToCmdId(EN_COMD_ID Comd)
        {
            EN_POSN_ID iPosnID = GetCmdToPosnId(Comd);
            
            if ((int)iPosnID < 0) return 0.0; 

            double dPosn = MP.dPosn[(int)iPosnID];
            return dPosn;
        }
        //--------------------------------------------------------------------------

        public bool CmprPosByCmd(EN_COMD_ID Comd, double InPos = 0, int iStep = 0, int iIndex = 0)
        {
            //Local Val.
            bool isOk;
            double dPos;
            EN_POSN_ID iPosnId;


            if (m_iNoUseMotr==1) return true;

            //Comp Step
            if (Comd == EN_COMD_ID.FindStep1 ) return CmprStep (iStep, EN_FSTEP_INDEX.Step1, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStep2 ) return CmprStep (iStep, EN_FSTEP_INDEX.Step2, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStep3 ) return CmprStep (iStep, EN_FSTEP_INDEX.Step3, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStep4 ) return CmprStep (iStep, EN_FSTEP_INDEX.Step4, (EN_FPOSN_INDEX)iIndex);

            if (Comd == EN_COMD_ID.FindStepF1) return CmprStepF(iStep, EN_FSTEP_INDEX.Step1, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStepF2) return CmprStepF(iStep, EN_FSTEP_INDEX.Step2, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStepF3) return CmprStepF(iStep, EN_FSTEP_INDEX.Step3, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStepF4) return CmprStepF(iStep, EN_FSTEP_INDEX.Step4, (EN_FPOSN_INDEX)iIndex);

            if (Comd == EN_COMD_ID.FindStepB1) return CmprStepB(iStep, EN_FSTEP_INDEX.Step1, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStepB2) return CmprStepB(iStep, EN_FSTEP_INDEX.Step2, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStepB3) return CmprStepB(iStep, EN_FSTEP_INDEX.Step3, (EN_FPOSN_INDEX)iIndex);
            if (Comd == EN_COMD_ID.FindStepB4) return CmprStepB(iStep, EN_FSTEP_INDEX.Step4, (EN_FPOSN_INDEX)iIndex);

            //Comd Move
            iPosnId = GetCmdToPosnId(Comd);
            if (iPosnId == EN_POSN_ID.None) return false;
            if (InPos <= 0) InPos = MP.dPosn[(int)EN_POSN_ID.InPos];
            dPos = MP.dPosn[(int)iPosnId];

            double dCortPos = 0.0;
            dPos -= dCortPos;

            isOk = (Math.Abs(GetEncPos() - dPos) < InPos) &&
                   (Math.Abs(GetCmdPos() - dPos) < InPos);

            if (isOk)
            {
                SetGain(1.0, 1.0);
            }
            return isOk;
        }
        //--------------------------------------------------------------------------
        //Status Check
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool        IsPairSlave      ()
        {
            for (int i = 0; i < MOTR._iNumOfMotr; i++)
            {
                if (m_iPairAxisNo == m_iAxis) return true;
            }

            return false;
        }
        //--------------------------------------------------------------------------
        //Torque
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool       MoveTorqueP       (double dTorque, double dVel)
        {
             bool bRet = false; 
// 
//             if(!m_bInitAxis                                     ) return false;
//             if(m_iObjNo < 0 || m_iObjNo >= cDEF.MOTR._iNumOfMotr) return false; 
// 
//             m_bJogN    = false;
//             m_bJogP    = false;
//             m_bTorqueP = true ;
// 
//             if(isMakerAjin()) bRet = AJIN.MoveTorqueP(m_iObjNo, dTorque, dVel);
            return bRet;
        }
        //--------------------------------------------------------------------------
        public bool       MoveTorqueN       (double dTorque, double dVel)
        {
            bool bRet = false; 
//             if(!m_bInitAxis                                     ) return false;
//             if(m_iObjNo < 0 || m_iObjNo >= cDEF.MOTR._iNumOfMotr) return false; 
// 
//             m_bJogN    = false;
//             m_bJogP    = false;
//             m_bTorqueN = true ;
// 
//             if(isMakerAjin()) bRet = AJIN.MoveTorqueN(m_iObjNo, dTorque, dVel);
            return bRet;
        }
        //--------------------------------------------------------------------------
        public bool       MoveTorqueStop    ()
        {
            bool bRet = false; 
//             if(!m_bInitAxis                                     ) return false;
//             if(m_iObjNo < 0 || m_iObjNo >= cDEF.MOTR._iNumOfMotr) return false; 
// 
//             m_bJogN    = false;
//             m_bJogP    = false;
//             m_bTorqueN = false;
//             m_bTorqueP = false;
// 
//             if(isMakerAjin()) bRet = AJIN.MoveTorqueStop(m_iObjNo);
            return bRet;
        }
        //--------------------------------------------------------------------------
        public bool       SetParamTorque    ()
        {
            bool bRet = false; 
//             if(!m_bInitAxis                                     ) return false;
//             if(m_iObjNo < 0 || m_iObjNo >= cDEF.MOTR._iNumOfMotr) return false; 
// 
//             if(isMakerAjin()) bRet = AJIN.SetParamTorque(m_iObjNo);
            return bRet;
        }
        //--------------------------------------------------------------------------
        public bool       MoveTorque        (double dTorque, double Pos , double Vel = 20.0, double Acc = 0.3 , double Dec = 0.0)
        {
            bool bRet = false; 
//             if(!m_bInitAxis                                     ) return false;
//             if(m_iObjNo < 0 || m_iObjNo >= cDEF.MOTR._iNumOfMotr) return false; 
// 
//             m_bJogN    = false;
//             m_bJogP    = false;
//             m_bTorqueN = false;
//             m_bTorqueP = false;
// 
//             if(isMakerAjin()) bRet = AJIN.MoveTorque(m_iObjNo, dTorque, Pos, Vel, Acc, Dec);
            return bRet;
        }
        //--------------------------------------------------------------------------
        public bool       SetTorqueLimit    (double dTorque)
        {
            bool bRet = false; 
//             if(!m_bInitAxis                                     ) return false;
//             if(m_iObjNo < 0 || m_iObjNo >= cDEF.MOTR._iNumOfMotr) return false; 
// 
//             if(isMakerAjin()) bRet = AJIN.SetTorqueLimit(m_iObjNo, dTorque);
            return bRet;
        }
        //--------------------------------------------------------------------------
        //Update Motor Status.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Update()
        {
            int iPairMstrAxe = -1;
            int iPairSubAxe  = -1;
            //
            if(!m_bInitAxis                                ) return;
            if(m_iObjNo < 0 || m_iObjNo >= MOTR._iNumOfMotr) return; 

            try 
            { 
                dStrtTime = (double)Environment.TickCount;

                //Update
                _fUpdateAxe(m_iObjNo);

                iPairMstrAxe = GetPairMstrAxe();
                iPairSubAxe  = GetPairAxe    ();
                m_dCmdPos    = GetCmdPos     ();
                m_dEncPos    = GetEncPos     ();
                

                //If Jog On , set target to current command.
                if (m_bJogN  ||  m_bJogP               ) { SetPosCmdToTrg(); }
                if (m_bJogN  && !m_bLtJogN             )   m_bLtJogN = true;
                if (!m_bJogN &&  m_bLtJogN && _fGStop()) { m_bLtJogN = false; SetPosCmdToTrg(); }
                if (m_bJogP  && !m_bLtJogP             )   m_bLtJogP = true;
                if (!m_bJogP &&  m_bLtJogP && _fGStop()) { m_bLtJogP = false; SetPosCmdToTrg(); }


                if (m_bTorqueN  ||  m_bTorqueP               ) { SetPosCmdToTrg(); }
                if (m_bTorqueN  && !m_bLtTorqueN             )   m_bLtTorqueN = true;
                if (!m_bTorqueN &&  m_bLtTorqueN && _fGStop()) { m_bLtTorqueN = false; SetPosCmdToTrg(); }
                if (m_bTorqueP  && !m_bLtTorqueP             )   m_bLtTorqueP = true;
                if (!m_bTorqueP &&  m_bLtTorqueP && _fGStop()) { m_bLtTorqueP = false; SetPosCmdToTrg(); }
                

                //If Servo is off, Set target position to current position.
                if (!GetOk() && (GetHomeStep() == 0))
                {
                    SetPosEncToTrg();
                    //If Servo is off, Set target position to current position.
                    if (GetStop() && GetHomeEnd() && iPairMstrAxe >= 0)
                    {
                        if (Math.Abs(m_dCmdPos - m_dEncPos) < m_dInPos) 
                        { 
                            SetPosCmdToTrg(); 
                        }
                    }
                }

                //If Servo is off, Set target position to current position.
                if (GetStop() && GetHomeEnd())
                {
                    if (Math.Abs(m_dCmdPos - m_dEncPos) < m_dInPos) 
                    { 
                        SetPosCmdToTrg();
                    }
                }                

                //Check Ring Counter.
                if (_fGRing() && _fGStop())
                {
                    if (!GetHomeEnd()) SetPosCmdToTrg();
                    else
                    {
                        if (_fMotionDone(m_iObjNo)) SetPosCmdToTrg();
                    }
                }

                //Jog Force Stop
                if (m_bJogP || m_bJogN)
                {
                    if (m_tJogStopDely.OnDelay(true, 2000))
                    {
                        //EmrgStop(); //JUNG/0807/삭제
                    }
                }
                else m_tJogStopDely.Clear();
                
                //Check Error.
                if (m_iNoUseMotr==0 && !m_bPrivAlarm   && GetAlarm ()) { Stop();           _fClearHomeEnd(); }
                if (m_iNoUseMotr==0 &&  m_bPrivServoOn && !GetServo()) { SetPosEncToCmd(); _fClearHomeEnd(); }

                m_bPrivAlarm    = GetAlarm();
                m_bPrivServoOn  = GetServo();

                //Check Limit.
                if (m_iNoUseMotr==0 && GetServo() && GetHomeEnd() && !GetStop() && (m_dEncPos > MP.MaxPos)) Stop(true);
                if (m_iNoUseMotr==0 && GetServo() && GetHomeEnd() && !GetStop() && (m_dEncPos < MP.MinPos)) Stop(true);
                

                if(isMakerAjin()) SetAbsOriginCycle();

                if (iPairSubAxe >= 0)
                {//m_iObjNo
                   MOTR[iPairSubAxe].SetHomeEnd(GetHomeEnd());
                }

                //JUNG/200901/Alarm
                if(m_bPrivAlarm)
                {
                    ACS.fn_SetHomeFlagOff(m_iAxis);
                }

                dScanTime = (double)Environment.TickCount - dStrtTime; 

            }
            catch (Exception ex)
            {
                LOG.ExceptionTrace("TAxis. Update " , ex);
                return;
            }

        }
        //--------------------------------------------------------------------------
        //Read/Write Para.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Load(bool IsLoad , string Path, MOTN_PARA TMP)
        {
            //Local Var.
            string AxisPath = string.Empty ;
    
            string sFile1   = string.Format($"[{m_iAxis:D2}]AXIS");
            string sSection = string.Empty;
            string sName    = string.Empty , sDesc = string.Empty;

            if (m_iAxis < 0 || m_iAxis >= MOTR._iNumOfMotr) return;


            //Make Dir.
            UserFile.fn_CheckDir(Path);
            AxisPath = string.Format($"{Path}\\{sFile1}.ini"); 


            //Position.
            for (int i = 0 ; i < MAX_POSN ; i++ ) 
            {
                sSection = string.Format($"{m_sNameAxis}_{m_sName}_Position" );
                sName    = sSection + "_Posn #" + (i + 1);
                //sDesc    = sSection + "_Desc #" + (i + 1);
                if (IsLoad) {

                    TMP.dPosn[i] = UserINI.fn_Load(sSection, sName, TMP.dPosn[i], AxisPath);
                    if(m_iAxis == (int)EN_MOTR_ID.miCLN_Y || m_iAxis == (int)EN_MOTR_ID.miPOL_Y)
                    {
                        TMP.dPosn[(int)EN_POSN_ID.Wait2] = 50; //JUNG/200506
                    }
                }
                else
                {
                    UserINI.fn_Save(sSection, sName, TMP.dPosn[i], AxisPath);
                }
            }

            //Velocity.
            for (int i = 0 ; i < MAX_SPED ; i++ ) 
            {
                sSection = string.Format($"{m_sNameAxis}_{m_sName}_Velocity");
                sName    = sSection + "_Vel #" + (i + 1);
                //sDesc    = sSection + "_Desc #" + (i + 1);
                if (IsLoad) 
                {
                    TMP.dVel[i] = UserINI.fn_Load(sSection, sName, TMP.dVel[i], AxisPath);
                }
                else        
                {
                    UserINI.fn_Save(sSection, sName, TMP.dVel[i], AxisPath);
                }
            }

            //Acceleration.
            for (int i = 0 ; i < MAX_SPED ; i++ ) 
            {   
                sSection = string.Format($"{m_sNameAxis}_{m_sName}_Acceleration");
                sName    = sSection + "_Acc #" + (i + 1);
                //sDesc    = sSection + "_Desc #" + (i + 1);
                if (IsLoad) 
                {
                    TMP.dAcc[i] = UserINI.fn_Load(sSection, sName, TMP.dAcc[i], AxisPath);
                }
                else        
                {
                    UserINI.fn_Save(sSection, sName, TMP.dAcc[i], AxisPath);
                }
            }


            //Deceleration.
            for (int i = 0 ; i < MAX_SPED ; i++ ) {
                sSection = string.Format($"{m_sNameAxis}_{m_sName}_Deceleration"); 
                sName    = sSection + "_Dec #"  + (i + 1);
                //sDesc    = sSection + "_Desc #" + (i + 1);
                if (IsLoad) 
                {
                    
                    TMP.dDec[i] = UserINI.fn_Load(sSection, sName, TMP.dDec[i], AxisPath);
                }
                else        
                {
                    UserINI.fn_Save(sSection, sName, TMP.dDec[i], AxisPath);
                }
            }

            //TimeSet.
            for (int i = 0 ; i < MAX_DLAY ; i++ ) {                
                sSection = string.Format($"{m_sNameAxis}_{m_sName}_TimeSet");
                sName    = sSection + "_Time #" + (i + 1);
                //sDesc    = sSection + "_Desc #" + (i + 1);
                if (IsLoad) 
                {
                    TMP.dTime[i] = UserINI.fn_Load(sSection, sName, TMP.dTime[i], AxisPath);
                }
                else        
                {
                    UserINI.fn_Save(sSection, sName, TMP.dTime[i], AxisPath);
                }

            }

            //Default Setting
            if(IsLoad)
            {
                for (int i = 0 ; i < MAX_SPED ; i++ ) 
                {
                    if(TMP.dAcc[i]  <= 0) TMP .dAcc[i] = 0.5;
                    if(TMP.dDec[i]  <= 0) TMP .dDec[i] = 0.5;
                    if(TMP.dVel[i]  <= 0) TMP .dVel[i] = 30 ;
                }
                if(TMP.dPosn[(int)EN_POSN_ID.InPos]  <= 0) TMP .dPosn[(int)EN_POSN_ID.InPos] = 0.1 ;
            }

        }
        //--------------------------------------------------------------------------
        public void        LoadCommSpd      (bool IsLoad    )
        {//현재 속도 정보를 DEFAULT로 저장할때 사용한다. 
            //Local Var.
            string AxisPath;

            //JSM 160316
            string sFile1  = string.Format($"[{m_iAxis:D2}]AXIS");
            string sSection ;
            string sName    ;

            if (m_iAxis < 0 || m_iAxis >= MOTR._iNumOfMotr) return;

            //Make Dir.
            string Path = UserFile.fn_GetExePath() + "Project";

            //Make Dir.
            UserFile.fn_CheckDir(Path);

            Path += "\\Common";

            UserFile.fn_CheckDir(Path);
            AxisPath = Path + "\\"+ sFile1 + ".ini";

            //Velocity.
            for (int i = 0 ; i < MAX_SPED ; i++ ) 
            {
                sSection = string.Format("{0}_{1}_Velocity" , m_sNameAxis, m_sName);    // sAxisName[m_iAxis] -> m_sName 변경
                sName    = sSection + "_Vel #" + (i + 1);
                if (IsLoad) 
                {
                    
                    CMP.dVel[i] = UserINI.fn_Load(sSection, sName, CMP.dVel[i], AxisPath);

                    MP.dVel[i] = CMP.dVel[i]; 
                }
                else        
                {
                    CMP.dVel[i] = MP.dVel[i];
                    UserINI.fn_Save(sSection, sName, CMP.dVel[i], AxisPath);
                }
            }

            //Acceleration.
            for (int i = 0 ; i < MAX_SPED ; i++ ) 
            {   
                sSection = string.Format("{0}_{1}_Acceleration" , m_sNameAxis, m_sName);    // sAxisName[m_iAxis] -> m_sName 변경
                sName    = sSection + "_Acc #" + (i + 1);
                if (IsLoad) {
                    
                    CMP.dAcc[i] = UserINI.fn_Load(sSection, sName, CMP.dAcc[i], AxisPath);
                    MP.dAcc[i] = CMP.dAcc[i]; 

                    }
                else  {
                    CMP.dAcc[i] = MP.dAcc[i];
                    UserINI.fn_Save(sSection, sName, CMP.dAcc[i], AxisPath);
                }
            }


            //Deceleration.
            for (int i = 0 ; i < MAX_SPED ; i++ ) {
                sSection = string.Format("{0}_{1}_Deceleration" , m_sNameAxis, m_sName);    // sAxisName[m_iAxis] -> m_sName 변경
                sName    = sSection + "_Dec #" + (i + 1);
                if (IsLoad) {

                    CMP.dDec[i] = UserINI.fn_Load(sSection, sName, CMP.dDec[i], AxisPath);
                    MP.dDec[i] = CMP.dDec[i];  
                }
                else        {
                    CMP.dDec[i] = MP.dDec[i];
                    UserINI.fn_Save(sSection, sName, CMP.dDec[i], AxisPath);
                    
                }
            }

            //TimeSet.
            for (int i = 0 ; i < MAX_DLAY ; i++ ) {                
                sSection = string.Format("{0}_{1}_TimeSet" , m_sNameAxis, m_sName);    // sAxisName[m_iAxis] -> m_sName 변경
                sName    = sSection + "_Time #" + (i + 1);
                if (IsLoad) {
                     CMP.dTime[i] = UserINI.fn_Load(sSection, sName, CMP.dTime[i], AxisPath);
                     MP.dTime[i] = CMP.dTime[i];  
                }
                else        {
                    CMP.dTime[i] = MP.dTime[i]; 
                    UserINI.fn_Save(sSection, sName, CMP.dTime[i], AxisPath);
                }

            }
        }
        //--------------------------------------------------------------------------
        public void LoadOptn(bool IsLoad)
        {
            //Local Var.
            string sName   = string.Empty;

            //Make Dir.
            string Path = UserFile.fn_GetExePath() + "System";

            //Make Dir.
            UserFile.fn_CheckDir(Path);
            Path += "\\MasterMotor.ini";

            //Check File Exist.
            //MasterMotor 파일이 없을 경우 Default Master Option 설정.
            if (!UserFile.fn_CheckFileExist(Path))
            {
                IsLoad = false;
                UserFunction.fn_UserMsg("Can not find the Master Motor configuration file.\nPlease contact SMEC Co., Ltd.");
            }

            sName = string.Format($"{m_iAxis:D2}");

            //Load Master Options.
            if (IsLoad) 
            {
                m_iMotrType       = UserINI.fn_Load("MotrType"        , sName, m_iMotrType       , Path);
                m_iMotrKind       = UserINI.fn_Load("MotrKind"        , sName, m_iMotrKind       , Path);
                m_dGR_Pulse       = UserINI.fn_Load("GR_Pulse"        , sName, m_dGR_Pulse       , Path);
                m_dGR_Disp        = UserINI.fn_Load("GR_Disp"         , sName, m_dGR_Disp        , Path);
                m_dMinPosn        = UserINI.fn_Load("MinPosn"         , sName, m_dMinPosn        , Path);
                m_dMaxPosn        = UserINI.fn_Load("MaxPosn"         , sName, m_dMaxPosn        , Path);
                m_dMaxVel         = UserINI.fn_Load("MaxVel"          , sName, m_dMaxVel         , Path);
                m_dMinAcc         = UserINI.fn_Load("MinAcc"          , sName, m_dMinAcc         , Path);
                m_dMinDec         = UserINI.fn_Load("MinDec"          , sName, m_dMinDec         , Path);
                m_dHomeOff        = UserINI.fn_Load("HomeOff"         , sName, m_dHomeOff        , Path);
                m_dMaxPulse       = UserINI.fn_Load("MaxPulse"        , sName, m_dMaxPulse       , Path);
                m_dInPos          = UserINI.fn_Load("InPos"           , sName, m_dInPos          , Path);
                m_dPartialPos     = UserINI.fn_Load("PartialPos"      , sName, m_dPartialPos     , Path);
                m_dAbsOriginPos   = UserINI.fn_Load("AbsOriginPos"    , sName, m_dAbsOriginPos   , Path);
                m_iSONLevel       = UserINI.fn_Load("SONLevel"        , sName, m_iSONLevel       , Path);
                m_iInpLevel       = UserINI.fn_Load("InpLevel"        , sName, m_iInpLevel       , Path);
                m_iAlarmLevel     = UserINI.fn_Load("AlarmLevel"      , sName, m_iAlarmLevel     , Path);
                m_iEncInputLevel  = UserINI.fn_Load("EncInputLevel"   , sName, m_iEncInputLevel  , Path);
                m_iEndLimitEnable = UserINI.fn_Load("EndLimitEnable"  , sName, m_iEndLimitEnable , Path);
                m_iHomeLevel      = UserINI.fn_Load("HomeLevel"       , sName, m_iHomeLevel      , Path);
                m_iProcessDir     = UserINI.fn_Load("ProcessDir"      , sName, m_iProcessDir     , Path);
                m_iPulseOut       = UserINI.fn_Load("PulseOut"        , sName, m_iPulseOut       , Path);
                m_iUseCam         = UserINI.fn_Load("UseCam"          , sName, m_iUseCam         , Path);
                m_iNoUseEnc       = UserINI.fn_Load("NoUseEnc"        , sName, m_iNoUseEnc       , Path);
                m_iNoUseMotr      = UserINI.fn_Load("NoUseMotr"       , sName, m_iNoUseMotr      , Path);
                m_iNoUseHomeSen   = UserINI.fn_Load("NoUseHomeSen"    , sName, m_iNoUseHomeSen   , Path);
                m_dRingMax        = UserINI.fn_Load("dRingMax"        , sName, m_dRingMax        , Path);
                m_iMoveDirection  = UserINI.fn_Load("MoveDirection"   , sName, m_iMoveDirection  , Path);
                m_iPackType       = UserINI.fn_Load("PackType"        , sName, m_iPackType       , Path);
                m_iIntpAxisNo     = UserINI.fn_Load("IntpAxisNo"      , sName, m_iIntpAxisNo     , Path);
                m_iPairAxisNo     = UserINI.fn_Load("PairAxisNo"      , sName, m_iPairAxisNo     , Path);
                m_iAutoResp       = UserINI.fn_Load("AutoResp"        , sName, m_iAutoResp       , Path);
                m_iHomeType       = UserINI.fn_Load("HomeType"        , sName, m_iHomeType       , Path);
                m_iHomeOptUse     = UserINI.fn_Load("HomeOptUse"      , sName, m_iHomeOptUse     , Path);
                m_iEncPulse       = UserINI.fn_Load("EncPulse"        , sName, m_iEncPulse       , Path);
                m_iServoType      = UserINI.fn_Load("ServoType"       , sName, m_iServoType      , Path);
                m_bSetScurve      = UserINI.fn_Load("SetScurve"       , sName, m_bSetScurve      , Path);
                m_iCOMIStartIO    = UserINI.fn_Load("COMIStartIO"     , sName, m_iCOMIStartIO    , Path);
                m_bUseRingCounter = UserINI.fn_Load("RingCounter"     , sName, m_bUseRingCounter , Path);
                m_bUseOverride    = UserINI.fn_Load("UseOverride"     , sName, m_bUseOverride    , Path);
                m_iPosLimitLevel  = UserINI.fn_Load("PosLimitLevel"   , sName, m_iPosLimitLevel  , Path);
                m_iNegLimitLevel  = UserINI.fn_Load("NegLimitLevel"   , sName, m_iNegLimitLevel  , Path);
                m_iHomeSignal     = UserINI.fn_Load("HomeSignal"      , sName, m_iHomeSignal     , Path);
                m_iHomeZPhase     = UserINI.fn_Load("HomeZPhase"      , sName, m_iHomeZPhase     , Path);
                m_bUseTorque      = UserINI.fn_Load("UseTorque"       , sName, m_bUseTorque      , Path);
                m_iHomeDir        = UserINI.fn_Load("HomeDir"         , sName, m_iHomeDir        , Path);
                m_sToqWAddr       = UserINI.fn_Load("ToqWAddr"        , sName, m_sToqWAddr       , Path);
                m_sToqRAddr       = UserINI.fn_Load("ToqRAddr"        , sName, m_sToqRAddr       , Path);
                m_sComPort        = UserINI.fn_Load("ComPort"         , sName, m_sComPort        , Path);
            }
            else 
            {
                UserINI.fn_Save("MotrType"        , sName, m_iMotrType       , Path);
                UserINI.fn_Save("MotrKind"        , sName, m_iMotrKind       , Path);
                UserINI.fn_Save("GR_Pulse"        , sName, m_dGR_Pulse       , Path);
                UserINI.fn_Save("GR_Disp"         , sName, m_dGR_Disp        , Path);
                UserINI.fn_Save("MinPosn"         , sName, m_dMinPosn        , Path);
                UserINI.fn_Save("MaxPosn"         , sName, m_dMaxPosn        , Path);
                UserINI.fn_Save("MaxVel"          , sName, m_dMaxVel         , Path);
                UserINI.fn_Save("MinAcc"          , sName, m_dMinAcc         , Path);
                UserINI.fn_Save("MinDec"          , sName, m_dMinDec         , Path);
                UserINI.fn_Save("HomeOff"         , sName, m_dHomeOff        , Path);
                UserINI.fn_Save("MaxPulse"        , sName, m_dMaxPulse       , Path);
                UserINI.fn_Save("InPos"           , sName, m_dInPos          , Path);
                UserINI.fn_Save("PartialPos"      , sName, m_dPartialPos     , Path);
                UserINI.fn_Save("AbsOriginPos"    , sName, m_dAbsOriginPos   , Path);
                UserINI.fn_Save("SONLevel"        , sName, m_iSONLevel       , Path);
                UserINI.fn_Save("InpLevel"        , sName, m_iInpLevel       , Path);
                UserINI.fn_Save("AlarmLevel"      , sName, m_iAlarmLevel     , Path);
                UserINI.fn_Save("EncInputLevel"   , sName, m_iEncInputLevel  , Path);
                UserINI.fn_Save("EndLimitEnable"  , sName, m_iEndLimitEnable , Path);
                UserINI.fn_Save("HomeLevel"       , sName, m_iHomeLevel      , Path);
                UserINI.fn_Save("ProcessDir"      , sName, m_iProcessDir     , Path);
                UserINI.fn_Save("PulseOut"        , sName, m_iPulseOut       , Path);
                UserINI.fn_Save("UseCam"          , sName, m_iUseCam         , Path);
                UserINI.fn_Save("NoUseEnc"        , sName, m_iNoUseEnc       , Path);
                UserINI.fn_Save("NoUseMotr"       , sName, m_iNoUseMotr      , Path);
                UserINI.fn_Save("NoUseHomeSen"    , sName, m_iNoUseHomeSen   , Path);
                UserINI.fn_Save("dRingMax"        , sName, m_dRingMax        , Path);
                UserINI.fn_Save("MoveDirection"   , sName, m_iMoveDirection  , Path);
                UserINI.fn_Save("PackType"        , sName, m_iPackType       , Path);
                UserINI.fn_Save("IntpAxisNo"      , sName, m_iIntpAxisNo     , Path);
                UserINI.fn_Save("PairAxisNo"      , sName, m_iPairAxisNo     , Path);
                UserINI.fn_Save("AutoResp"        , sName, m_iAutoResp       , Path);
                UserINI.fn_Save("HomeType"        , sName, m_iHomeType       , Path);
                UserINI.fn_Save("HomeOptUse"      , sName, m_iHomeOptUse     , Path);
                UserINI.fn_Save("EncPulse"        , sName, m_iEncPulse       , Path);
                UserINI.fn_Save("ServoType"       , sName, m_iServoType      , Path);
                UserINI.fn_Save("SetScurve"       , sName, m_bSetScurve      , Path);
                UserINI.fn_Save("COMIStartIO"     , sName, m_iCOMIStartIO    , Path);
                UserINI.fn_Save("RingCounter"     , sName, m_bUseRingCounter , Path);
                UserINI.fn_Save("UseOverride"     , sName, m_bUseOverride    , Path);
                UserINI.fn_Save("PosLimitLevel"   , sName, m_iPosLimitLevel  , Path);
                UserINI.fn_Save("NegLimitLevel"   , sName, m_iNegLimitLevel  , Path);
                UserINI.fn_Save("HomeSignal"      , sName, m_iHomeSignal     , Path);
                UserINI.fn_Save("HomeZPhase"      , sName, m_iHomeZPhase     , Path);
                UserINI.fn_Save("UseTorque"       , sName, m_bUseTorque      , Path);
                UserINI.fn_Save("HomeDir"         , sName, m_iHomeDir        , Path);
                UserINI.fn_Save("ToqWAddr"        , sName, m_sToqWAddr       , Path);
                UserINI.fn_Save("ToqRAddr"        , sName, m_sToqRAddr       , Path);
                UserINI.fn_Save("ComPort"         , sName, m_sComPort        , Path);
            }

        }
        //Conversion.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public double      CalPulseToPos    (double    Pulse)
        {
            //Local Var.
            double dPos;

            //Check Coef.
            if (m_dCoef <= 0) return 0;

            //
            dPos = ((double)Pulse / m_dCoef);

            //Check Cam.
            if (m_iUseCam == 0) return dPos;

            //Cal.
            dPos = CalCamDegToPos(dPos);

            //Return.
            return dPos;
        }
        //--------------------------------------------------------------------------
        public double         CalPosToPulse    (double Pos  )
        {
            //Local Var.
            double dPulse;

            //Check Coef.
            if (m_dCoef <= 0) return 0;

            //
            dPulse = (Pos * m_dCoef);

            //Check Cam.
            if ((m_dGR_Pulse == 1) && (m_dGR_Disp == 1)) return dPulse;
            else
            {
                if (m_iUseCam == 0) return (int)dPulse; //
            }

            //Cal.
            dPulse = CalCamPosToDeg(Pos) * m_dCoef;

            //Return.
            return dPulse;
        }
        //--------------------------------------------------------------------------
        public double CalCamDegToPos(double Deg)
        {
            //Local Var.
            double h = m_dPartialPos;
            double b = 360.0;
            double T = Deg;
            double s;

            //Check Max.
            if (Math.Abs(T) > b) T = b;

            //Cal.
            s = (h) * (Math.Sin(Math.PI * (T / b)));
            //Ok.
            return s;
        }
        //--------------------------------------------------------------------------
        public double      CalCamPosToDeg   (double Pos  )
        {
            //Local Var.
            double h = m_dPartialPos;
            double b = 360.0;
            double s = Pos;
            double T;

            //Check Max.
            //s = fabs(s);
            if (Math.Abs(s) > h) s = h;

            //Cal.
            T = (Math.Asin((s) / h) * b) / Math.PI;

            //Ok.
            return T;
        }
        //--------------------------------------------------------------------------
        //Display.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void        DispParamFrm     ()
        {
//             FrmSetMotr frmSetMotr = new FrmSetMotr();
//             frmSetMotr.m_iSelMotr = m_iAxis;
//             frmSetMotr.ShowDialog();
//             frmSetMotr = null;
        }
        //--------------------------------------------------------------------------
        //SET ABS POSITION
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void       ReqAbsOrigin          ()
        {
            if (m_iMaker != (int)EN_MOTR_MAKER.AJIN && m_iMaker != (int)EN_MOTR_MAKER.AJECAT) {
                m_iAbsStep = 0;
                return;
            }
            m_iAbsStep = 10;
        }
        //--------------------------------------------------------------------------
        public bool       SetAbsOriginCycle     ()
        {
            switch (m_iAbsStep)
            {
                default: m_iAbsStep = 0; break;

                case 10: SetServo(false);
                    m_iAbsStep++;
                    break;

                case 11: if (GetServo())
                    {
                        SetServo(false);
                        break;
                    }
                    SetHomeLevel(m_iHomeLevel);
                    m_tAbsOrigin.Clear();
                    m_iAbsStep++;
                    break;

                case 12: if (!m_tAbsOrigin.OnDelay(true, 5000)) return false;
                    SetAbsOrigin(GetOrgEncPos());
                    LoadOptn(false);
                    m_iAbsStep++;
                    break;

                case 13: SetServo(true);
                    m_iAbsStep = 0;
                    return true;
            }
            return false;
        }
    }
}

