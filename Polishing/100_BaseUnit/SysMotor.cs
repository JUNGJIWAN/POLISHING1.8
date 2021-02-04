using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using WaferPolishingSystem.Define;
using WaferPolishingSystem.BaseUnit;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain;
using ACS.SPiiPlusNET;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.ManualId;
using System.Windows.Markup;

namespace WaferPolishingSystem.BaseUnit
{

    /***************************************************************************/
    /* Class: TSysMotor                                                        */
    /* Create:                                                                 */
    /* Developer:                                                              */
    /* Note:                                                                   */
    /***************************************************************************/
    public class SysMotor
    {
        /* Constants                                                          */
        /**********************************************************************/

        private Api ACSAPI;

        private int[]  m_nHomeEnd = new int[MAX_MOTOR];
        private object objVar;
        private Array  arHomeEnd;


        //Timer
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        TOnDelayTimer m_tServoOff = new TOnDelayTimer();
        TOnDelayTimer m_tDrvReset = new TOnDelayTimer();
        
        //Vars.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        int          m_iNumOfMotr;
        int          m_iNumOfDstb;
        int          m_iAddDstb  ;
        int          m_iPartCnt  ;
        bool         m_bConnect  ;
        //bool         m_bACSERROR ; //Open Fail Check
        int          m_nACSMode  ;

        TAxisUnit[]  AXIS        ;
		SMCControl[] SMC;

        int[]        m_iRef      ;
        int[]        m_iStepRst = new int[2];

        int[]        m_nProcessDir ;

        public bool m_bNeedReboot  ;
        public bool m_bInitAxis    ;
        public int  m_iFHomeErr    ;
        public int  m_iSpedMode    ;
        public bool m_bContiMove   ;
        public bool m_bSkipChkCrash;

        public DSTB_POSN[] DstbPosn;

        bool m_bInit;


        //Indexer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public TAxisUnit this[int Axe]
        {
            get 
            { 
                if (Axe<0 | Axe>=m_iNumOfMotr) return null;
                return AXIS[Axe]; 
            }
            set 
            {
                if (Axe<0 | Axe>=m_iNumOfMotr) return; 
                AXIS[Axe]= value; 
            }

        }

        //Property.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int _iNumOfMotr { get { return m_iNumOfMotr; } }
        public int _iNumOfDstb { get { return m_iNumOfDstb; } }
        public int _iFHomeErr  { get { return m_iFHomeErr ; } set { m_iFHomeErr = value; }}

        public bool _bConnect  { get { return m_bConnect; } }

        //Position Data
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public TSetPart[] Dat = new TSetPart[MAX_SEQ_PART];


        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public SysMotor()
        {
            ACSAPI   = new Api();

            m_iNumOfMotr    = (int)EN_MOTR_ID.EndOfId;
            m_iNumOfDstb    = (int)EN_DSTB_ID.EndOfId;
            m_iAddDstb      = 0;
            m_bSkipChkCrash = false;
            m_bConnect      = false;
            //m_bACSERROR     = false; 

            AXIS          = new TAxisUnit [m_iNumOfMotr];
            m_iRef        = new int       [m_iNumOfMotr];
            m_nProcessDir = new int       [m_iNumOfMotr];

            DstbPosn      = new DSTB_POSN [m_iNumOfDstb];

            //SMC
            SMC = new SMCControl[3];

            int motor = (int)EN_MOTR_ID.miSPD_Z1;
            for (int n = 0; n < 3; n++)
            {
                SMC[n] = new SMCControl(motor++);
            }
            for (int iAxe=0; iAxe<m_iNumOfMotr; iAxe++) {
                AXIS         [iAxe] = new TAxisUnit ();
                m_iRef       [iAxe] = new int();
                m_nProcessDir[iAxe] = new int();
            }
            
            for (int i = 0; i < m_iNumOfDstb; i++)
            {
                DstbPosn[i] = new DSTB_POSN(0.0);
            }

            //
            for (int i = 0; i < MAX_SEQ_PART; i++)
            {
                Dat[i] = new TSetPart();
            }

            for (int i = 0; i < MAX_SEQ_PART; i++)
            {
                Dat[i].m_sName   = "";
                Dat[i].m_iItemCnt = 0;

                for (int j = 0; j < (int)EN_MOTR_ID.EndOfId; j++)
                {
                    Dat[i].m_iPosnCnt[j] = -1;
                }
            }

            //
            for (int iAxe=0; iAxe<m_iNumOfMotr; iAxe++) {
                m_nProcessDir[iAxe] = 1;
            }

            //
            for (int n = 0; n < MAX_MOTOR; n++)
            {
                m_nHomeEnd[n] = new int();
            }
            
            objVar    = null;
            arHomeEnd = null;

            m_bInit   = false;
        }
        //---------------------------------------------------------------------------
        public void fn_InitMotor(int AcsMode)
        {
            //ACS Mode
            m_nACSMode = AcsMode;

            //UserSet - 모터 메이커의 모터 수량을 입력 
            //ex) MOTR.Init   (EN_MOTR_MAKER.COMI,0, EN_MOTR_MAKER.AJECAT, 4);
            Init                 (EN_MOTR_MAKER.ACS,  0);
            InitMotrName         ();
            InitPosName          ();
            InitDstb             ();
            //InitMinMaxData       ();
            InitMinMaxData       (ref FM.m_stMasterOpt.dMinPos, ref FM.m_stMasterOpt.dMaxPos, ref FM.m_stMasterOpt.dMinVel, ref FM.m_stMasterOpt.dMaxVel, ref FM.m_stMasterOpt.dMinAcc, ref FM.m_stMasterOpt.dMaxAcc);
            InitProcessDir       ();
            
            Load                 (true , FM._sCurrJob  );
            //LoadMotrDisturb      (true                 );
            SetAxis              ();
        }   

        //---------------------------------------------------------------------------
        private void InitMotrName         (          )
        {
            //UserSet - 화면에 표시될 Motor 이름 및 Motor Error 설정 
            int iFHomeMan   = (int)EN_MAN_LIST.MAN_0005; //5  ; //First Part Home Manual No
            int iFManNo     = (int)EN_MAN_LIST.MAN_0025; //25 ; //First Motor Manual Start No
            int iFErrNo     = (int)EN_ERR_LIST.ERR_0200;        //First Motor Error  Start No 

            MOTR._iFHomeErr = (int)EN_ERR_LIST.ERR_0015;        //First Motor Homing Error No

            //UserSet - Set Motor Description                     //Error  ,Manual   , Home Manual No
            fn_SetMotor(EN_MOTR_ID.miSPD_X , "Spindle"  , "X " , iFErrNo, iFManNo, iFHomeMan);  //00//200 miSPD_X
            fn_SetMotor(EN_MOTR_ID.miPOL_Y , "Polishing", "Y " , iFErrNo, iFManNo, iFHomeMan);  //01//    miPOL_Y
            fn_SetMotor(EN_MOTR_ID.miSPD_Z , "Spindle"  , "Z " , iFErrNo, iFManNo, iFHomeMan);  //02//    miSPD_Z
            fn_SetMotor(EN_MOTR_ID.miCLN_R , "Cleaning" , "R " , iFErrNo, iFManNo, iFHomeMan);  //03//    miCLN_R
            fn_SetMotor(EN_MOTR_ID.miPOL_TH, "Polishing", "TH" , iFErrNo, iFManNo, iFHomeMan);  //04//    miPOL_TH
            fn_SetMotor(EN_MOTR_ID.miPOL_TI, "Polishing", "TI" , iFErrNo, iFManNo, iFHomeMan);  //05//    miPOL_TI
            fn_SetMotor(EN_MOTR_ID.miSTR_Y , "Storage"  , "Y " , iFErrNo, iFManNo, iFHomeMan);  //06//    miSTR_Y
            fn_SetMotor(EN_MOTR_ID.miCLN_Y , "Cleaning" , "Y " , iFErrNo, iFManNo, iFHomeMan);  //07//    miCLN_Y
            fn_SetMotor(EN_MOTR_ID.miSPD_Z1, "Spindle"  , "Z1" , iFErrNo, iFManNo, iFHomeMan);  //08//    miSPD_Z1
            fn_SetMotor(EN_MOTR_ID.miTRF_T , "Transfer" , "T " , iFErrNo, iFManNo, iFHomeMan);  //09//    miTRF_T
            fn_SetMotor(EN_MOTR_ID.miTRF_Z , "Transfer" , "Z " , iFErrNo, iFManNo, iFHomeMan);  //10//    miTRF_Z

        }                                  

        //---------------------------------------------------------------------------
        public void fn_SetMotor(EN_MOTR_ID iMotrId, string sName, string sAxis, 
                                                    int iErrNo = -1, int iManNo = -1, int iPartHomeNo = -1)
        {
            int iMotr = (int)iMotrId;

            if(iMotr<0 || iMotr>=MOTR._iNumOfMotr) iMotr = 0;

            MOTR[iMotr].m_sName       = sName   ;
            MOTR[iMotr].m_sNameAxis   = sAxis   + " AXIS";

            //Define Manual No
            MOTR[iMotr].m_iManStop     = iManNo  + (25 * iMotr)   ; //25 //50 //75 // 100 //125
            MOTR[iMotr].m_iManJog      = iManNo  + (25 * iMotr)+ 1; //26
            MOTR[iMotr].m_iManPitch    = iManNo  + (25 * iMotr)+ 2; //27
            MOTR[iMotr].m_iManServo    = iManNo  + (25 * iMotr)+ 3; //28
            MOTR[iMotr].m_iManAlarm    = iManNo  + (25 * iMotr)+ 4; //29
            MOTR[iMotr].m_iManDirect   = iManNo  + (25 * iMotr)+ 5; //30
            MOTR[iMotr].m_iManHome     = iManNo  + (25 * iMotr)+ 6; //31  //56
            MOTR[iMotr].m_iManPartHome = iPartHomeNo;

            //Define Error  No
            MOTR[iMotr].m_iErrAlarm    = iErrNo  + (10 * iMotr);
            MOTR[iMotr].m_iErrCW       = iErrNo  + (10 * iMotr)+ 1;
            MOTR[iMotr].m_iErrCCW      = iErrNo  + (10 * iMotr)+ 2;
            MOTR[iMotr].m_iErrHome     = iErrNo  + (10 * iMotr)+ 3;
            MOTR[iMotr].m_iErrControl  = iErrNo  + (10 * iMotr)+ 4;
            MOTR[iMotr].m_iErrHold     = iErrNo  + (10 * iMotr)+ 5;
            MOTR[iMotr].m_iErrPos      = iErrNo  + (10 * iMotr)+ 6;
            MOTR[iMotr].m_iErrVel      = iErrNo  + (10 * iMotr)+ 7;
            MOTR[iMotr].m_iErrAcc      = iErrNo  + (10 * iMotr)+ 8;


            //m_iLManNo = iManNo  + (20 * iMotr) + 20 + 100;
            //m_iLErrNo = iErrNo  + (10 * iMotr) + 10 + 100;
        }

        //---------------------------------------------------------------------------
        private void InitPosName()
        {
            //UserSet - Pos 화면에 설정할 이름 및 모터 설정 
             string sPart = string.Empty;


            //Part no, Part Name, Pos Item Name ,단위    ,Pos Index, 소수점 자리, 모터 No, Move Manual No
            sPart = "SPINDLE";                                                                
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Wait Pos."                  , "mm", EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Pos."             , "mm", EN_POSN_ID.User1  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cleaning Pos"               , "mm", EN_POSN_ID.User2  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Tool Pick Pos"    , "mm", EN_POSN_ID.FSP1_1 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cleaning Tool Pick Pos"     , "mm", EN_POSN_ID.FSP2_1 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Tool Discard Pos."          , "mm", EN_POSN_ID.User4  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Force Check Pos."           , "mm", EN_POSN_ID.User5  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Plate Polishing P/P Pos."   , "mm", EN_POSN_ID.User6  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Plate Cleaning P/P Pos."    , "mm", EN_POSN_ID.User7  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Plate Load P/P Pos."        , "mm", EN_POSN_ID.User8  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Util Check Pos."  , "mm", EN_POSN_ID.User9  , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cleaning Util Check Pos."   , "mm", EN_POSN_ID.User10 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Loading Vision Pos.(PRE)"   , "mm", EN_POSN_ID.User11 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Vision Pos."      , "mm", EN_POSN_ID.User12 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Storage Vision Align Pos.01", "mm", EN_POSN_ID.User13 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Storage Vision Align Pos.02", "mm", EN_POSN_ID.User14 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cup Storage P/P Pos."       , "mm", EN_POSN_ID.User15 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Cup P/P Pos."     , "mm", EN_POSN_ID.User16 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Auto Teaching Pos - Left"   , "mm", EN_POSN_ID.User20 , 3, EN_MOTR_ID.miSPD_X);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Auto Teaching Pos - Right"  , "mm", EN_POSN_ID.User21 , 3, EN_MOTR_ID.miSPD_X);

            //
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Wait Pos."                  , "mm", EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miSPD_Z);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Start Pos."       , "mm", EN_POSN_ID.User1  , 3, EN_MOTR_ID.miSPD_Z);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cleaning Start Pos."        , "mm", EN_POSN_ID.User2  , 3, EN_MOTR_ID.miSPD_Z);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Tool Pick Pos."   , "mm", EN_POSN_ID.User3  , 3, EN_MOTR_ID.miSPD_Z);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cleaning Tool Pick Pos."    , "mm", EN_POSN_ID.User6  , 3, EN_MOTR_ID.miSPD_Z);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Tool Dispose Pos."          , "mm", EN_POSN_ID.User4  , 3, EN_MOTR_ID.miSPD_Z);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Force Check Pos."           , "mm", EN_POSN_ID.User5  , 3, EN_MOTR_ID.miSPD_Z);

            //                                                                 
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Wait Pos."                  , "mm", EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Load Zone Pick Pos."        , "mm", EN_POSN_ID.User1  , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Pick Pos."        , "mm", EN_POSN_ID.User3  , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cleaning Pick Pos."         , "mm", EN_POSN_ID.User4  , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Load Zone Place Pos."       , "mm", EN_POSN_ID.User11 , 3, EN_MOTR_ID.miSPD_Z1); //JUNG/200528/Position 분리
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Place Pos."       , "mm", EN_POSN_ID.User13 , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cleaning Place Pos."        , "mm", EN_POSN_ID.User14 , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Cup Storage P/P Pos."       , "mm", EN_POSN_ID.User5  , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Cup Pick Pos."    , "mm", EN_POSN_ID.User6  , 3, EN_MOTR_ID.miSPD_Z1);
            fn_PosSet(EN_SEQ_ID.SPINDLE, sPart, "Polishing Cup Place Pos."   , "mm", EN_POSN_ID.User7  , 3, EN_MOTR_ID.miSPD_Z1);


            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            sPart  = "POLISHING";
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Plate P/P Pos.(Wait Pos.)" , "mm" , EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miPOL_Y ); //
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Work Pos."                 , "mm" , EN_POSN_ID.User1  , 3, EN_MOTR_ID.miPOL_Y ); //Polishing Position
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Vision Inspect Pos."       , "mm" , EN_POSN_ID.User2  , 3, EN_MOTR_ID.miPOL_Y ); //Vision Position
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Cup Storage In/Out Pos."   , "mm" , EN_POSN_ID.User4  , 3, EN_MOTR_ID.miPOL_Y ); //Storage Cup In/Out Position
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Polishing Cup In/Out Pos." , "mm" , EN_POSN_ID.User3  , 3, EN_MOTR_ID.miPOL_Y ); //Cup In/Out Position
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Utility Check Pos."        , "mm" , EN_POSN_ID.User5  , 3, EN_MOTR_ID.miPOL_Y ); //Utility Exist Check Position
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Transfer turn safety Pos." , "mm" , EN_POSN_ID.Wait2  , 3, EN_MOTR_ID.miPOL_Y, EN_POS_KIND.VIEW); //avoid position of Transfer Turn 
                                                                             
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Wait Pos."                 , "Deg", EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miPOL_TH);
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Cup In/Out Pos."           , "Deg", EN_POSN_ID.User1  , 3, EN_MOTR_ID.miPOL_TH);
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Polishing Cal. Pos."       , "Deg", EN_POSN_ID.CalPos , 3, EN_MOTR_ID.miPOL_TH);
                                                                             
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Wait Pos."                 , "Deg" , EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miPOL_TI);
            fn_PosSet(EN_SEQ_ID.POLISH  , sPart, "Work Pos."                 , "Deg" , EN_POSN_ID.User1  , 3, EN_MOTR_ID.miPOL_TI);
            
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            sPart = "CLEANING";
            fn_PosSet(EN_SEQ_ID.CLEAN  , sPart, "Plate P/P Pos.(Wait Pos.)"  , "mm", EN_POSN_ID.Wait1   , 3, EN_MOTR_ID.miCLN_Y ); //Plate P/P Position
            fn_PosSet(EN_SEQ_ID.CLEAN  , sPart, "Work Pos."                  , "mm", EN_POSN_ID.User1   , 3, EN_MOTR_ID.miCLN_Y );
            fn_PosSet(EN_SEQ_ID.CLEAN  , sPart, "Transfer turn safety Pos."  , "mm", EN_POSN_ID.Wait2   , 3, EN_MOTR_ID.miCLN_Y, EN_POS_KIND.VIEW); //avoid position of Transfer Turn 

            fn_PosSet(EN_SEQ_ID.CLEAN  , sPart, "Wait Pos."                  , "Deg", EN_POSN_ID.Wait1   , 3, EN_MOTR_ID.miCLN_R);
            fn_PosSet(EN_SEQ_ID.CLEAN  , sPart, "Rotation_Clean"             , "Deg", EN_POSN_ID.User1   , 3, EN_MOTR_ID.miCLN_R);
            fn_PosSet(EN_SEQ_ID.CLEAN  , sPart, "Rotation_DEHYDE"            , "Deg", EN_POSN_ID.User2   , 3, EN_MOTR_ID.miCLN_R);
            fn_PosSet(EN_SEQ_ID.CLEAN  , sPart, "Rotation(with time)"        , "sec", EN_POSN_ID.User3   , 3, EN_MOTR_ID.miCLN_R);
                                                                                              
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     
            sPart = "STORAGE";                                                             
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Wait Pos."                  , "mm", EN_POSN_ID.Wait1   , 3, EN_MOTR_ID.miSTR_Y);
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Polishing Tool Pick Pos."   , "mm", EN_POSN_ID.FSP1_1  , 3, EN_MOTR_ID.miSTR_Y);
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Cleaning Tool Pick Pos."    , "mm", EN_POSN_ID.FSP2_1  , 3, EN_MOTR_ID.miSTR_Y);
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Tool Discard Pos."          , "mm", EN_POSN_ID.User3   , 3, EN_MOTR_ID.miSTR_Y);
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Storage Out Pos."           , "mm", EN_POSN_ID.User4   , 3, EN_MOTR_ID.miSTR_Y);
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Tool Storage Check Pos."    , "mm", EN_POSN_ID.Wait2   , 3, EN_MOTR_ID.miSTR_Y);
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Vision Align Pos - Bottom"  , "mm", EN_POSN_ID.User20  , 3, EN_MOTR_ID.miSTR_Y);
            fn_PosSet(EN_SEQ_ID.STORAGE, sPart, "Vision Align Pos - Top"     , "mm", EN_POSN_ID.User21  , 3, EN_MOTR_ID.miSTR_Y);
           

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     
            sPart = "TRANSFER";
            fn_PosSet(EN_SEQ_ID.TRANSFER, sPart, "Wait Pos.(Load/Unload)"    , "mm" , EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miTRF_T );
            fn_PosSet(EN_SEQ_ID.TRANSFER, sPart, "Pre-Align Pos."            , "mm" , EN_POSN_ID.User1  , 3, EN_MOTR_ID.miTRF_T );
            fn_PosSet(EN_SEQ_ID.TRANSFER, sPart, "Align Calibration Pos."    , "mm" , EN_POSN_ID.CalPos , 3, EN_MOTR_ID.miTRF_T, EN_POS_KIND.VIEW);

            fn_PosSet(EN_SEQ_ID.TRANSFER, sPart, "Wait Pos."                 , "mm" , EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miTRF_Z );
            fn_PosSet(EN_SEQ_ID.TRANSFER, sPart, "Work Start Pos."           , "mm" , EN_POSN_ID.FSP1_1 , 3, EN_MOTR_ID.miTRF_Z );
                                                                             
        }
        //---------------------------------------------------------------------------
        public void fn_PosSet(EN_SEQ_ID iPart, string sPart, string sName, string sUnit,
                              EN_POSN_ID PosnId, int sDigit , EN_MOTR_ID iMotorId , EN_POS_KIND iPosnKind = EN_POS_KIND.NORM,
                              int iManNo = -1, bool bHomeOffset = false)
        {
            int iSeqPart =(int)iPart   ;
            int iMotor   =(int)iMotorId;
            int cRow     = Dat[iSeqPart].m_iItemCnt;
            
            string sDesc;

            if(iSeqPart<0 || iSeqPart>=MAX_SEQ_PART) iSeqPart = 0;
            if(cRow    <0 || cRow    >=MAX_ITEM    ) cRow     = 0;
            
            Dat [iSeqPart].m_sName = sPart;
            m_iPartCnt = iSeqPart;

            if(sName == "" ) return;

            Dat[iSeqPart].Set[cRow].m_sName        = sName         ;
            Dat[iSeqPart].Set[cRow].m_sUnit        = sUnit         ;
            Dat[iSeqPart].Set[cRow].m_iDigit       = sDigit        ;
            Dat[iSeqPart].Set[cRow].m_iMotor       = iMotor        ;
            Dat[iSeqPart].Set[cRow].m_iPosnId      = (int)PosnId   ;
            Dat[iSeqPart].Set[cRow].m_bHomeOffset  = bHomeOffset   ;
            Dat[iSeqPart].Set[cRow].m_bDefUserMan  = false         ;
            Dat[iSeqPart].Set[cRow].m_iPosnKind    = (int)iPosnKind;

            //
            if(Dat[iSeqPart].Set[cRow]._Val < Dat[iSeqPart].Set[cRow].m_dMin) Dat[iSeqPart].Set[cRow]._Val = Dat[iSeqPart].Set[cRow].m_dMin;
            if(Dat[iSeqPart].Set[cRow]._Val > Dat[iSeqPart].Set[cRow].m_dMax) Dat[iSeqPart].Set[cRow]._Val = Dat[iSeqPart].Set[cRow].m_dMax;

            Dat[iSeqPart].m_iItemCnt ++;

            //
            if(iMotor<0                ) return;
            if(iMotor>=MOTR._iNumOfMotr) return;

            Dat [iSeqPart].m_iPosnCnt[iMotor] +=1;

            Dat [iSeqPart].Set[cRow].m_iManNo  = MOTR[iMotor].m_iManHome + Dat[iSeqPart].m_iPosnCnt[iMotor] + 1; //32

            MOTR[iMotor].SetPart(iSeqPart);
            Dat [iSeqPart].m_iMotorCnt ++;


            sDesc = string.Format("{0}[{1}]{2}_{3}", MOTR[iMotor].m_sName      ,
                                                     MOTR[iMotor].m_sNameAxis  ,
                                                     fn_GetPartName(iSeqPart)  ,
                                                     Dat[iSeqPart].Set[cRow].m_sName  );
            //MOTR[iMotor].MP.sPosn_Desc[(int)PosnId] = sDesc;
            MOTR[iMotor].MP.sPosn_Desc[(int)PosnId] = sDesc; 

        }
        //---------------------------------------------------------------------------
        public void InitMinMaxData()
        {

            //Polishing Tilt - HomeOffset -2 : (-6 ~+10) 눈금기준으로...

                                                 /* MinPos, MaxPos, MinVel, MaxVel, MaxAcc, MinAcc, */
            //MOTR[(int)EN_MOTR_ID.miSPD_X ].SetMaxMin(   0,    640, 0.5, 1000, 0.5, 300);
            //MOTR[(int)EN_MOTR_ID.miPOL_Y ].SetMaxMin(   0,    105, 0.5, 1000, 0.5, 300);
            //MOTR[(int)EN_MOTR_ID.miSPD_Z ].SetMaxMin(   0,     70, 0.5, 1000, 0.5, 300);
            //MOTR[(int)EN_MOTR_ID.miCLN_R ].SetMaxMin(   0, 999999, 0.5, 1000, 0.5, 300);
            //MOTR[(int)EN_MOTR_ID.miPOL_TH].SetMaxMin(  -6,      5, 0.5,    2,   5,   1);
            //MOTR[(int)EN_MOTR_ID.miPOL_TI].SetMaxMin(  -7,     10, 0.5,    2,   5,   1);
            //                                          
            //MOTR[(int)EN_MOTR_ID.miSTR_Y ].SetMaxMin( -15,    300, 0.5, 1000, 0.5, 300);
            //MOTR[(int)EN_MOTR_ID.miCLN_Y ].SetMaxMin(   0,    110, 0.5, 1000, 0.5, 300);
            //                                                
            //MOTR[(int)EN_MOTR_ID.miSPD_Z1].SetMaxMin(   0,     90, 5 ,   500, 0.5,  30);
            //MOTR[(int)EN_MOTR_ID.miTRF_Z ].SetMaxMin(   0,    280, 5 ,   125, 0.5,  30);
            //MOTR[(int)EN_MOTR_ID.miTRF_T ].SetMaxMin(   0,     90, 20,   200, 0.5,  30);
                                                        
        }
        //---------------------------------------------------------------------------
        public void InitMinMaxData(ref double[] minpos, ref double[] maxpos, ref double[] minvel, ref double[] maxvel, ref double[] maxacc, ref double[] minacc)
        {

            //Polishing Tilt - HomeOffset -2 : (-6 ~+10) 눈금기준으로...

                                                 /* MinPos, MaxPos, MinVel, MaxVel, MaxAcc, MinAcc, */
            MOTR[(int)EN_MOTR_ID.miSPD_X ].SetMaxMin( minpos[0] , maxpos[0] , minvel[0] , maxvel[0] , maxacc[0] , minacc[0] );
            MOTR[(int)EN_MOTR_ID.miPOL_Y ].SetMaxMin( minpos[1] , maxpos[1] , minvel[1] , maxvel[1] , maxacc[1] , minacc[1] );
            MOTR[(int)EN_MOTR_ID.miSPD_Z ].SetMaxMin( minpos[2] , maxpos[2] , minvel[2] , maxvel[2] , maxacc[2] , minacc[2] );
            MOTR[(int)EN_MOTR_ID.miCLN_R ].SetMaxMin( minpos[3] , maxpos[3] , minvel[3] , maxvel[3] , maxacc[3] , minacc[3] );
            MOTR[(int)EN_MOTR_ID.miPOL_TH].SetMaxMin( minpos[4] , maxpos[4] , minvel[4] , maxvel[4] , maxacc[4] , minacc[4] );
            MOTR[(int)EN_MOTR_ID.miPOL_TI].SetMaxMin( minpos[5] , maxpos[5] , minvel[5] , maxvel[5] , maxacc[5] , minacc[5] );
            MOTR[(int)EN_MOTR_ID.miSTR_Y ].SetMaxMin( minpos[6] , maxpos[6] , minvel[6] , maxvel[6] , maxacc[6] , minacc[6] );
            MOTR[(int)EN_MOTR_ID.miCLN_Y ].SetMaxMin( minpos[7] , maxpos[7] , minvel[7] , maxvel[7] , maxacc[7] , minacc[7] );
            MOTR[(int)EN_MOTR_ID.miSPD_Z1].SetMaxMin( minpos[8] , maxpos[8] , minvel[8] , maxvel[8] , maxacc[8] , minacc[8] );
            MOTR[(int)EN_MOTR_ID.miTRF_Z ].SetMaxMin( minpos[9] , maxpos[9] , minvel[9] , maxvel[9] , maxacc[9] , minacc[9] );
            MOTR[(int)EN_MOTR_ID.miTRF_T ].SetMaxMin( minpos[10], maxpos[10], minvel[10], maxvel[10], maxacc[10], minacc[10]);
                                                        
        }
        //---------------------------------------------------------------------------
        private void InitProcessDir()
        {
            //
            MOTR[(int)EN_MOTR_ID.miSPD_X].SetProcessDir(0);


        }
        //---------------------------------------------------------------------------
        public void SetHomeOffset()
        {
            //
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_00_SPD_X_Home_Offset ] = FM.m_stMasterOpt.dHomeOffset[0];
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_01_POL_Y_Home_Offset ] = FM.m_stMasterOpt.dHomeOffset[1];
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_02_SPD_Z_Home_Offset ] = FM.m_stMasterOpt.dHomeOffset[2];

            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_03_CLN_R_Home_Offset ] = FM.m_stMasterOpt.dHomeOffset[3];
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_04_POL_TH_Home_Offset] = FM.m_stMasterOpt.dHomeOffset[4];
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_05_POL_TI_Home_Offset] = FM.m_stMasterOpt.dHomeOffset[5];

            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_06_STR_Y_Home_Offset ] = FM.m_stMasterOpt.dHomeOffset[6];
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_07_CLN_Y_Home_Offset ] = FM.m_stMasterOpt.dHomeOffset[7];

        }
        //---------------------------------------------------------------------------
        public void InitHomeOffset()
        {
            double dVal = 0.0;

            FM.m_stMasterOpt.dHomeOffset[0] = dVal;
            FM.m_stMasterOpt.dHomeOffset[1] = dVal;
            FM.m_stMasterOpt.dHomeOffset[2] = dVal;
            FM.m_stMasterOpt.dHomeOffset[3] = dVal;
            FM.m_stMasterOpt.dHomeOffset[4] = dVal;
            FM.m_stMasterOpt.dHomeOffset[5] = dVal;
            FM.m_stMasterOpt.dHomeOffset[6] = dVal;
            FM.m_stMasterOpt.dHomeOffset[7] = dVal;

            //
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_00_SPD_X_Home_Offset ] = dVal;
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_01_POL_Y_Home_Offset ] = dVal;
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_02_SPD_Z_Home_Offset ] = dVal;

            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_03_CLN_R_Home_Offset ] = dVal;
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_04_POL_TH_Home_Offset] = dVal;
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_05_POL_TI_Home_Offset] = dVal;

            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_06_STR_Y_Home_Offset ] = dVal;
            IO.DATA_EQ_TO_ACS[(int)EN_EQ_TO_ACS.ETA_07_CLN_Y_Home_Offset ] = dVal;
        }
        //---------------------------------------------------------------------------
        public string fn_GetPartName(int iPart, bool IsNor = true)
        {
            String pName;
            if(iPart == GetPartCnt()         ) return "SYSTEM";
            if(iPart<0 || iPart>=GetPartCnt()) return "ALL"   ;
            string Item = Dat[iPart].m_sName;

            if(!IsNor) return Item;

            int iPos = Item.IndexOf("\n");
            if(iPos > 0) 
            {
                string Item1 = Item.Substring(1, iPos - 1);
                string Item2 = Item .Substring(iPos+2) ;
                pName = Item1 + " " + Item2;
            }
            else pName = Item;

            return pName;
        }
        //---------------------------------------------------------------------------
        public int GetPartCnt()
        {
            return m_iPartCnt + 1;
        }
        //--------------------------------------------------------------------------
        public bool GetMotorPart(ref int iPart, ref int iIndex, int iMotorNo)
        {
            for(int i=0; i<MAX_SEQ_PART; i++) {
                for(int j=0; j<Dat[i].m_iItemCnt; j++) {
                    if(j>=MAX_ITEM) continue;
                    if(iMotorNo == Dat[i].Set[j].m_iMotor) {
                        iPart  = i;
                        iIndex = j;
                        return true;
                    }
                }
            }
            return false;
        }

        //---------------------------------------------------------------------------
        private void  InitDstb()                                                                               
        {

            fn_SetDstb(EN_DSTB_ID.DP_SPDLZ_POS_MOVE_MAINX     , 5.0  , "DP_SPDLZ_POS_MOVE_MAINX "   , "Spindle Z-Axis Position for move Spindle X-Axis"  );
            fn_SetDstb(EN_DSTB_ID.DP_SPDLZ1_POS_MOVE_MAINX    , 5.0  , "DP_SPDLZ1_POS_MOVE_MAINX"   , "Spindle Z1-Axis Position for move Spindle X-Axis" );
            fn_SetDstb(EN_DSTB_ID.DP_SPDLX_POS_DSTB_POLIZ     , 100.0, "DP_SPDLX_POS_MOVE_MAINX "   , "Spindle X-Axis Position for move Polishing cap Z-Axis");
            fn_SetDstb(EN_DSTB_ID.DP_SPDLZ_POS_MOVE_BTMY      , 10.0 , "DP_SPDLZ_POS_MOVE_BTMY "    , "Spindle Z-Axis Max Position for move Bottom Y-Axis");
            fn_SetDstb(EN_DSTB_ID.DP_SPDLZ1_POS_MOVE_BTMY     , 10.0 , "DP_SPDLZ1_POS_MOVE_BTMY"    , "Spindle Z-Axis Max Position for move Bottom Y-Axis");

            fn_SetDstb(EN_DSTB_ID.DP_CLEANY_POS_MOVE_LOADTURN ,100.0 , "DP_CLEANY_POS_MOVE_LOADTURN", "Clean Y-Axis Max Position for transfer turn");


        }                      
        //---------------------------------------------------------------------------
        private void fn_SetDstb(EN_DSTB_ID id, double pos, string name, string desc, string unit = "mm" )
        {
            int nId = (int)id;

            if (nId > m_iNumOfDstb) return;

            MOTR.DstbPosn[nId].dPos  = pos ;
            MOTR.DstbPosn[nId].sName = name;
            MOTR.DstbPosn[nId].sDesc = desc;
            MOTR.DstbPosn[nId].sUnit = unit;
        }                       

        //Init.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void Init(params object[] args)
        {
            //Init - Motor Init는 MAX_MOTR:64 까지 진행

            //string sParamFile = UserFile.fn_GetExePath() + "System\\Motion_para_AJIN.mot";
            //string sCmePath   = UserFile.fn_GetExePath() + "Motion_para_AJIN.mot";
            
            /*
            <ACS>
            0.Main X(IN(0).0 HOME)
            1.Polishing Y(IN(0).1 HOME)
            2.Main Z(IN(1).0 HOME IN(1).1 ???)
            
            <Yaskawa>
            3.Cleaning T(Node 26, IN(2).2 - HOME)
            4.Polishing TH(node 27, IN(3)
            5.Polishing TI(node28, IN(4))
            6.Loading Y(node29, IN(5).0 LL IN(5).1 RL IN(5).2 HOME)
            7.Storage Y(node30, IN(6)
            8.Cleaning Y(node31, IN(7).0 LL IN(7).1 RL IN(7).2 HOME)
            
            <FASTECH>
            9.Polishing Z(node 35, IN(8)
            
            <SMC> - Program
            10.Main Z1
            11.Loading T1
            12.Loading T2
            */
            bool   bTryOnTime = false; 
            int    iObjNo = 0 ;
            int    iMaker = 0 ;
            int    iAxisNo  = (int)args[1];

            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {
                iObjNo = Axe;
                iMaker = (int)args[0];

                //for(int iM=args.Count()-1;iM>=2;iM-=2)
                //{
                //    if (Axe >= (int)args[iM]) { //3번 Maker Axis
                //        iMaker = (int)args[iM-1];
                //        iObjNo =  Axe - (int)args[iM];
                //    }
                //}

                if (Axe >= (int)EN_MOTR_ID.miSPD_Z1) iMaker = (int)EN_MOTR_MAKER.SMC;

                this[(int)Axe].Init     (Axe, iObjNo, iMaker, ACSAPI); //mmMC1x  , mmCOMI,  AJIN, mmEMTN
                this[(int)Axe].LoadOptn (true);

                if (!m_bConnect && !bTryOnTime)
                {
                    m_bConnect = this[(int)Axe].Connect(m_nACSMode);
                    bTryOnTime = !m_bConnect; 
                }
                
                //this[(int)Axe].SetParamPath(sParamFile , sCmePath );
                
            }

            //Open.
            m_bInitAxis = true;
            EN_MOTR_MAKER[] iOpenMaker = new EN_MOTR_MAKER[args.Count()];
            EN_MOTR_MAKER   iCrntMaker = EN_MOTR_MAKER.NONE;
            int             iOpenQty   = 0;
            bool            isSame     = false;

            for(int iM=1;iM<args.Count();iM+=2)
            {
                iAxisNo = (int)args[iM];
                if(iAxisNo>=m_iNumOfMotr) {
                    MessageBox.Show("Motor Open Error[Check InitMotor() Function]");
                    m_bInitAxis = false;
                    break;        
                }
                iCrntMaker = (EN_MOTR_MAKER)args[iM-1];

                isSame = false;
                for (int iO = 0; iO < iOpenQty; iO++)
                {
                    if (iCrntMaker == iOpenMaker[iO]) { isSame = true; break; }
                    if (iCrntMaker == EN_MOTR_MAKER.AJIN || iCrntMaker == EN_MOTR_MAKER.AJECAT)
                    {
                        if (iOpenMaker[iO] == EN_MOTR_MAKER.AJIN || iOpenMaker[iO] == EN_MOTR_MAKER.AJECAT) { isSame = true; break; }
                    }
                }
                if (isSame) continue;

                if (!OpenMotor(iAxisNo)) m_bInitAxis = false; 
                //else                     m_bConnect  = true ;
                
                iOpenMaker[iOpenQty] = iCrntMaker;
                iOpenQty ++;

            } 
            
            if(m_bInitAxis) InitServoOff();
     
            //Set Motor Parameter
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {
                this[Axe]._bInitAxis = m_bInitAxis;

                if (Axe >= (int)EN_MOTR_ID.miSPD_Z1) continue;
                this[(int)Axe].SetParameter (Axe);
            }

            
            //Reset Motor. (Alarm , Flag, Step)
            Reset();
            
        }
        //---------------------------------------------------------------------------
        public void SetDstb(String sName, String sDesc, String sUnit = "mm")
        {
            if(m_iAddDstb>=m_iNumOfDstb) return;
            DstbPosn[m_iAddDstb].sName = sName; 
		    DstbPosn[m_iAddDstb].sDesc = sDesc; 
		    DstbPosn[m_iAddDstb].sUnit = sUnit;
            m_iAddDstb ++;
        }
        //---------------------------------------------------------------------------
        public void Reset(EN_MOTR_ID Axe)
        { 
            if(WrongAXE(Axe)) return; this[(int)Axe].Reset(); 
        }
        public void Reset()
        { 
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) Reset((EN_MOTR_ID)Axe); 
        }
        //---------------------------------------------------------------------------
        public void InitServoOff()
        {
            if(AXIS[0]._iMaker != (int)EN_MOTR_MAKER.AJIN) return;

            SetServo(false);
            m_tServoOff.Clear();
            bool isOneServoOn = false;
            do {
                isOneServoOn = false;
                m_tServoOff.OnDelay(true, 30000);
                for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) {
                    if(AXIS[Axe].GetServo()) { isOneServoOn = true; break; }
                }
                if(!isOneServoOn) break;
            }while(!m_tServoOff.Out);
        }
        //---------------------------------------------------------------------------
        //Check Min/Max                        
        public bool WrongAXE (EN_MOTR_ID Axe)
        { 
            return ((int)Axe<0 | (int)Axe>=m_iNumOfMotr); 
        }

        //Init. Motor.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool OpenMotor(int iAxisNo) 
        {
            return AXIS[iAxisNo].Open  ();
        }
        //---------------------------------------------------------------------------
        public void CloseMotor(EN_MOTR_MAKER enAxis = EN_MOTR_MAKER.AJIN)         
        {//프로그램 종료시에 한번 호출.
            for (int iAxe = 0 ; iAxe < m_iNumOfMotr ; iAxe++) {
                AXIS[iAxe].SetPairAxe(AXIS[iAxe]._iPairAxisNo, 0);
            }

            //AXIS[0].Close ();
        }
        //---------------------------------------------------------------------------
        public void SetAxis()   
        {//프로그램 로딩시에 한번 호출.
            
            //JUNG/200703/Set Jerk/Kill Dec.
            if(m_bConnect)
            {
                AXIS[(int)EN_MOTR_ID.miSPD_X ].SetJerk(10000);
                AXIS[(int)EN_MOTR_ID.miPOL_Y ].SetJerk(8000 );
                AXIS[(int)EN_MOTR_ID.miSPD_Z ].SetJerk(10000);
                AXIS[(int)EN_MOTR_ID.miCLN_R ].SetJerk(10000);
                AXIS[(int)EN_MOTR_ID.miPOL_TH].SetJerk(100  );
                AXIS[(int)EN_MOTR_ID.miPOL_TI].SetJerk(100  );
                AXIS[(int)EN_MOTR_ID.miSTR_Y ].SetJerk(100  );
                AXIS[(int)EN_MOTR_ID.miCLN_Y ].SetJerk(8000 );
            }
        }
        
        //---------------------------------------------------------------------------
        public void SetAxis_AsDevice()    
        {//JOB FILE 로딩시마다 호출.
            //Set InPos & S-Curve.
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) {
                this[(int)Axe].SetInPos    (AXIS[Axe].MP.dPosn[(int)EN_POSN_ID.InPos]);
                this[(int)Axe].SetAppScurve(false                         );
                this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.UserPitch] = 0.1;  //User Pitch는 모두 0.1mm로 한다.
            }
        }

        //Parameters.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public MOTN_PARA  GetMP            (EN_MOTR_ID Axe ) {if(WrongAXE(Axe)) Axe = 0; return this[(int)Axe].MP ; }
        public MOTN_PARA  GetCMP           (EN_MOTR_ID Axe ) {if(WrongAXE(Axe)) Axe = 0; return this[(int)Axe].CMP; }
        public TAxisUnit  GetAXIS          (EN_MOTR_ID Axe ) {if(WrongAXE(Axe)) Axe = 0; return this[(int)Axe]    ; }
        public double     GetDSTB(int no)  { if (no < 0 || no >= m_iNumOfDstb) no=0;     return DstbPosn[no].dPos ; }


        //Device Informations.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public double  GetPitch(EN_MOTR_ID Axe, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            if(WrongAXE(Axe)) return 0.0;
            return this[(int)Axe].GetPitch(FStep);
        }        
        public double  GetCenPitch (EN_MOTR_ID Axe, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            if(WrongAXE(Axe)) return 0.0;
            return this[(int)Axe].GetCenPitch(FStep);
        }
        public double  GetCenPitch (EN_MOTR_ID Axe , int iSlotNo, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            if(WrongAXE(Axe)) return 0.0;
            return this[(int)Axe].GetCenPitch(iSlotNo, FStep);
        }
        public int GetMaxSlot(EN_MOTR_ID Axe, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1)
        {
            if(WrongAXE(Axe)) return 1;
            return this[(int)Axe].GetMaxSlot(FStep);
        }
        //---------------------------------------------------------------------------
        public bool SetStragePitch(double rowpitch, double colpitch)
        {
            this[(int)EN_MOTR_ID.miSTR_Y].SetPitch(rowpitch);
            this[(int)EN_MOTR_ID.miSPD_X].SetPitch(colpitch);

            return true; 
        }
        //---------------------------------------------------------------------------
        public bool SetMagazinePitch(double rowpitch, double colpitch)
        {
            this[(int)EN_MOTR_ID.miTRF_Z].SetPitch(rowpitch);
            //this[(int)EN_MOTR_ID.miSPD_X].SetPitch(colpitch, 2, 0, EN_FSTEP_INDEX.Step1); //

            return true;
        }
        //---------------------------------------------------------------------------
        public bool SetTransferPPOffset(double pick, double place)
        {
            this[(int)EN_MOTR_ID.miTRF_Z].SetPPOffset(pick, place);

            return true;
        }

        //---------------------------------------------------------------------------
        public int GetCrntStep(EN_MOTR_ID Axe, EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.NONE, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE)
        {
            //
            double dPitch  = GetPitch (Axe, FStep);
            double dCmd    = GetCmdPos(Axe);
            double dOrigin = 0.0;
            double dLow , dHigh, dCenPitch;

            //Check None Index.
            if (FStep  == EN_FSTEP_INDEX.NONE) FStep  = EN_FSTEP_INDEX.Step1 ;
            if (FIndex == EN_FPOSN_INDEX.NONE) FIndex = EN_FPOSN_INDEX.Index1;
        
            //Check Marking Step.
            switch (FStep) 
            {
                case EN_FSTEP_INDEX.Step1: 
                    if (((int)EN_POSN_ID.FSP1_1 + (int)FIndex) >= MAX_POSN) FIndex = EN_FPOSN_INDEX.Index1;
                    dOrigin = this[(int)Axe].MP.dPosn [(int)EN_POSN_ID.FSP1_1 + (int)FIndex]; 
                    break;
                
                case EN_FSTEP_INDEX.Step2:
                    if (((int)EN_POSN_ID.FSP2_1 + (int)FIndex) >= MAX_POSN) FIndex = EN_FPOSN_INDEX.Index1;
                    dOrigin = this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.FSP2_1 + (int)FIndex];
                    break;

                case EN_FSTEP_INDEX.Step3:
                    if (((int)EN_POSN_ID.FSP3_1 + (int)FIndex) >= MAX_POSN) FIndex = EN_FPOSN_INDEX.Index1;
                    dOrigin = this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.FSP3_1 + (int)FIndex];
                    break;
                
                case EN_FSTEP_INDEX.Step4: 
                    if (((int)EN_POSN_ID.FSP4_1 + (int)FIndex) >= MAX_POSN) FIndex = EN_FPOSN_INDEX.Index1;
                    dOrigin = this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.FSP4_1 + (int)FIndex];
                    break;
            }
        
            //Cal. Step.
            for (int i = 0  ; i < GetMaxSlot(Axe) ; i++) 
            {
                dCenPitch = GetCenPitch (Axe , i, FStep);
                if (m_nProcessDir[i] == 1) {
                    dLow  = dOrigin + (double)i * dPitch + dCenPitch - 0.5;
                    dHigh = dOrigin + (double)i * dPitch + dCenPitch + 0.5;
                    }
                else {
                    dLow  = dOrigin - ((double)i * dPitch) - dCenPitch - 0.5;
                    dHigh = dOrigin - ((double)i * dPitch) - dCenPitch + 0.5;
                    }
        
                if ((dCmd >= dLow) && (dCmd <= dHigh)) 
                {
                    return  i;
                }
            }
        
            //Return Normal Step.
            return -1;
    



        }
        //---------------------------------------------------------------------------
        public bool SetEachPitchSlot(EN_MOTR_ID Axe, double Pitch = 1.0, int Slot = 1)
        {
            double[] dPitch    = new double [4];
            int   [] iSlot     = new int    [4];
 
            EN_FSTEP_INDEX FStep2 = EN_FSTEP_INDEX.NONE ;
            EN_FSTEP_INDEX FStep3 = EN_FSTEP_INDEX.NONE ;
            EN_FSTEP_INDEX FStep4 = EN_FSTEP_INDEX.NONE ;

            if(WrongAXE(Axe)) return false;
            dPitch[0] = Pitch;
            iSlot [0] = Slot ;

            if (dPitch[0] < 0.1) dPitch[0] = 1.0;
            if (iSlot [0] < 0.1) iSlot [0] = 1;
            this[(int)Axe].SetPitch(dPitch[0] , iSlot[0]);  //FSP1_x, FSP2_x, FSP3_x, FSP4_x;
            if(FStep2 != EN_FSTEP_INDEX.NONE) this[(int)Axe].SetPitch(dPitch[1] , iSlot[1], 0, EN_FSTEP_INDEX.Step2); //FSP2_x
            if(FStep3 != EN_FSTEP_INDEX.NONE) this[(int)Axe].SetPitch(dPitch[2] , iSlot[2], 0, EN_FSTEP_INDEX.Step3); //FSP3_x
            if(FStep4 != EN_FSTEP_INDEX.NONE) this[(int)Axe].SetPitch(dPitch[3] , iSlot[3], 0, EN_FSTEP_INDEX.Step4); //FSP4_x
            return true;
        }
        //---------------------------------------------------------------------------
        public void fn_SetDisableSMC()
        {
            SetServo(EN_MOTR_ID.miSPD_Z1, false) ;
            SetServo(EN_MOTR_ID.miTRF_Z , false) ;
            SetServo(EN_MOTR_ID.miTRF_T , false) ;
        }
        //---------------------------------------------------------------------------
        public void fn_SetEnableSMC()
        {
            SetServo(EN_MOTR_ID.miSPD_Z1, true) ;
            SetServo(EN_MOTR_ID.miTRF_Z , true) ;
            SetServo(EN_MOTR_ID.miTRF_T , true) ;
        }

        //Status.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetServo(EN_MOTR_ID Axe , bool On         )  //Each Servo On/Off.
        {
            if(WrongAXE(Axe)) return;          
            this[(int)Axe].SetServo(On);       

            if(Axe == EN_MOTR_ID.miSPD_Z1)
            {
                IO.fn_Z2Unlock(On);
            }
        } 
        //---------------------------------------------------------------------------
        public void SetServo(bool On, EN_MOTR_MAKER enAxis = EN_MOTR_MAKER.NONE) 
        {
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) {
                if (enAxis == EN_MOTR_MAKER.NONE || this[(int)Axe]._iMaker == (int)enAxis)  SetServo((EN_MOTR_ID)Axe , On);
            }
        }
        //---------------------------------------------------------------------------
        public void SetAlarm(EN_MOTR_ID Axe , bool On ) //Clear each alarm.
        {
            if(WrongAXE(Axe)) return;          
            this[(int)Axe].SetAlarm(On);        
        }
        public void SetAlarm (bool On ) 
        {//Clear all  alarm.
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
                SetAlarm((EN_MOTR_ID)Axe , On); 
        }
        public void ClearPos(EN_MOTR_ID Axe , double Pos = 0.0) 
        {//Clear each axis position.
            if(WrongAXE(Axe)) return;          
            this[(int)Axe].ClearPos(Pos);      
        } 
        public void ClearPos(double Pos = 0.0) 
        {//Clear all  axis position.
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
                ClearPos((EN_MOTR_ID)Axe , Pos);
        } 
        public bool SetPos (EN_MOTR_ID Axe , double Pos = 0.0) 
        {//Set Any Position.
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].SetPos        (Pos);
        } 
        public void SetPosEncToCmd (EN_MOTR_ID Axe) 
        {
            if(WrongAXE(Axe)) return;          
            this[(int)Axe].SetPosEncToCmd();   
        } 
        public void ClearHomeEnd (EN_MOTR_ID Axe) 
        {//Kill the each home end flag.
            if(WrongAXE(Axe)) return;          
            this[(int)Axe].ClearHomeEnd  ();   
        } 
        public void ClearHomeEnd() 
        {//Kill the all  home end flag. 
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
                ClearHomeEnd((EN_MOTR_ID)Axe);   
        }
        public void SetHomeEnd  (EN_MOTR_ID Axe , bool On) 
        { //Kill the each home end flag.
            if(WrongAXE(Axe)) return;          
            this[(int)Axe].SetHomeEnd  (On);   
        }
                                                                                                                                                
        public bool IsAllAlarm() 
        {//Is all alarm    ? 
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {       
                 if (this[(int)Axe]._iNoUseMotr==1) continue;    
                 if(!this[(int)Axe].GetAlarm  ()) return false; 
            }
            return true; 
        } 
        public bool IsAllServoOff() 
        {//Is all servo off 
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {
                if (this[(int)Axe]._iNoUseMotr==1) continue;    
                if ( this[(int)Axe].GetServo  ()) return false; 
            }
            return true; 
        }
        public bool IsAllServoOn() 
        {
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {
                if (this[(int)Axe]._iNoUseMotr==1) continue;    
                if (!this[(int)Axe].GetServo  ()) return false; 
            }
            return true; 
        }
        public bool IsAllHomeEnd () 
        {//Is all home  end 
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {
                if ( this[(int)Axe]._iNoUseMotr==1) continue;    
                if (!this[(int)Axe].GetHomeEnd()  ) return false; 
            }
            return true; 
        }

        public bool MotnIn (EN_MOTR_ID Axe , double S   , double E , bool ChkTrg = false) 
        {//설정 범위내에 Motor가 위치 해 있는지  확인.
            //Local Var.
            double dCmd = GetCmdPos(Axe);
            double dEnc = GetEncPos(Axe);
            double dTrg = GetTrgPos(Axe);

            //Return.
            if (ChkTrg) { return ( ((dCmd >= S) &&  (dCmd <= E)) || ((dEnc >= S) &&  (dEnc <= E)) || ((dTrg >= S) &&  (dTrg <= E))); }
            else        { return ( ((dCmd >= S) &&  (dCmd <= E)) || ((dEnc >= S) &&  (dEnc <= E))                                 ); }
        }
        public bool MotnIn (EN_MOTR_ID Axe , int iPosnId, double dArea = 0.5, bool ChkTrg = false)
        {
            //Local Var.
            double dCmd = GetCmdPos(Axe);
            double dEnc = GetEncPos(Axe);
            double dTrg = GetTrgPos(Axe);

            double dStrt  = this[(int)Axe].MP.dPosn[iPosnId] - dArea;
            double dEnd   = this[(int)Axe].MP.dPosn[iPosnId] + dArea;

            //Return.
            if (ChkTrg) { return ( ((dCmd >= dStrt) &&  (dCmd <= dEnd)) || ((dEnc >= dStrt) &&  (dEnc <= dEnd)) || ((dTrg >= dStrt) &&  (dTrg <= dEnd))); }
            else        { return ( ((dCmd >= dStrt) &&  (dCmd <= dEnd)) || ((dEnc >= dStrt) &&  (dEnc <= dEnd))                                        ); }
        }
        //Motor Position.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public double  GetCmdPos (EN_MOTR_ID Axe) 
        {//Set Any Position.
            if(WrongAXE(Axe)) return 0.0; 
            return this[(int)Axe].GetCmdPos   ();
        } 
        public double  GetTrgPos (EN_MOTR_ID Axe) 
        {//Set Any Position.
            if(WrongAXE(Axe)) return 0.0; 
            return this[(int)Axe].GetTrgPos   ();
        } 
        public double  GetEncPos(EN_MOTR_ID Axe) 
        { //Set Any Position.
            if(WrongAXE(Axe)) return 0.0; 
            return this[(int)Axe].GetEncPos   ();
        }
        public double  GetAbsEncPos(EN_MOTR_ID Axe ) 
        {//Set Any Position.
            if(WrongAXE(Axe)) return 0.0; 
            return this[(int)Axe].GetAbsEncPos();
        }
        //Inspection Motor.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Tool Impact.
        public bool InspectCrash(EN_SEQ_STATE SeqStat)
        {
            return false;
        }
        public bool CheckCrash (EN_MOTR_ID  Axe   , EN_COMD_ID Cmd = EN_COMD_ID.NoneCmd, 
                                   int Step = UserConst.NONE_STEP , EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE , double DirPosn = 0.0)
        {
            //             int iPart = 0;
            //             int iItem = 0;
            //             if (!cDEF.POSN.GetMotorPart(ref iPart, ref iItem, (int)Axe)) return false;
            //             if(m_bSkipChkCrash) return true;
            //             bool isOk = cDEF.SEQ.CheckDstb((EN_SEQ_ID)iPart, Axe, Cmd, Step, FIndex, DirPosn);
            bool isOk = true; 
            return isOk;
        }
        public bool    CheckTorqueLimit     (EN_MOTR_ID Axe, double dTorque, double dchkRange)
        {
            if( dTorque <= 0                                ) return false;
            if( this[(int)Axe].GetStop()                    ) return false;
            if(!this[(int)Axe].m_bUseTorque                 ) return false;
            if( this[(int)Axe].GetEncPos() < (dchkRange+0.5)) return false;

            if(Math.Abs(this[(int)Axe].GetTorque())>=dTorque) {
                Stop(Axe);
                return false;
                }
            return true;
        }

        //Min-Max
        public bool InspectMinMax(bool ChkOnly = false)
        {
            for (int i = 0 ; i < m_iNumOfMotr ; i++) {
                //Position.
                for (int p = 0 ; p < MAX_POSN ; p++) 
                {
                    if (!CheckMinMaxP((EN_MOTR_ID)i  , p))
                    {
                        EPU.fn_SetErr(ErrNoPos((EN_MOTR_ID)i) , !ChkOnly);
                        return false;
                    }
                }
                /*
                //Vel.
                for (int v = 0 ; v < (int)EN_MOTR_VEL.LJog  ; v++) {
                    if (!CheckMinMaxV((EN_MOTR_ID)i  , v)) {
                        EPU.fn_SetErr(ErrNoVel((EN_MOTR_ID)i) , !ChkOnly);
                        return false;
                        }
                    }
                //Acc
                for (int v = 0 ; v <= (int)EN_MOTR_VEL.LJog ; v++) {
                    if (!CheckMinMaxA((EN_MOTR_ID)i  , v)) {
                        EPU.fn_SetErr(ErrNoAcc((EN_MOTR_ID)i) , !ChkOnly); return false;
                        }
                    }
                */
            }

            //Ok.
            return true;
        }
        public bool CheckMinMax (EN_MOTR_ID Axe , double P , double V , double A) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMax (P ,V ,A);
        }
        public bool CheckMinMaxP(EN_MOTR_ID Axe , double P) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMaxP(P      );
        }
        public bool CheckMinMaxV(EN_MOTR_ID Axe , double V) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMaxV(V      );
        }
        public bool CheckMinMaxA (EN_MOTR_ID Axe , double A) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMaxA(A      );
        }
        public bool CheckMinMax(EN_MOTR_ID Axe , int P , int V) 
        {//mpPara Checking.
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMax (P ,V   );
        }
        public bool CheckMinMaxP(EN_MOTR_ID Axe , int    P) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMaxP(P      );
        }
        public bool CheckMinMaxV(EN_MOTR_ID Axe , int    V) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMaxV(V      );
        }
        public bool CheckMinMaxA (EN_MOTR_ID Axe , int    V) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckMinMaxA(V      );
        }
        //Compare position.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool CmprPos (EN_MOTR_ID Axe , double Pos) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CmprPos     (Pos);
        }
        public bool CmprPos (EN_MOTR_ID Axe , double Pos, double InPos ) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CmprPos(Pos,InPos);
        }
        public int CmprPosStat (EN_MOTR_ID Axe , double Pos , bool ChkTrg   = false) 
        {
            if(WrongAXE(Axe)) return 0; 
            return this[(int)Axe].CmprPosStat (Pos ,ChkTrg  );
        }
        public int CmprPosStat (EN_MOTR_ID Axe , double Pos    , double Offset , bool ChkTrg   = false) 
        {
            if(WrongAXE(Axe)) return 0; 
            return this[(int)Axe].CmprPosStat (Pos,Offset ,ChkTrg  );
        }
        public int CmprPosStat(EN_MOTR_ID Axe , int PosID , bool ChkTrg= false) 
        {
            if(WrongAXE(Axe)) return 0; 
            return this[(int)Axe].CmprPosStat (PosID ,ChkTrg);
        }
        public bool CmprStep (EN_MOTR_ID Axe , int Step , 
                              EN_FSTEP_INDEX FStep = EN_FSTEP_INDEX.Step1 , EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.Index1) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CmprStep    (Step,FStep,FIndex);
        }
        public bool CmprArea(EN_MOTR_ID Axe , double Pos, double Area  , bool NoChkEnc = false ) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CmprArea(Pos, Area,NoChkEnc);
        }
        public bool CmprPosByCmd(EN_MOTR_ID Axe  , EN_COMD_ID Comd , double InPos = 0, int iStep=0, int iIndex=0) 
        {
            if(WrongAXE(Axe)) return false; 

            return this[(int)Axe].CmprPosByCmd(Comd ,InPos  ,iStep ,iIndex);
        }
                                               
        public int WhreTarget(EN_MOTR_ID Axe) 
        {//Motor Target 확인.
            if(WrongAXE(Axe)) return MOTR_TARG_STP; 
            return this[(int)Axe].WhreTarget();
        }
        public int WhreTarget(EN_MOTR_ID Axe  , EN_COMD_ID Cmd  , int Step , EN_FPOSN_INDEX FIndex , double DirPosn = 0.0  )
        {
            if(WrongAXE(Axe)) return MOTR_TARG_STP;
            double dPos = 0.0;

            if      (Cmd == EN_COMD_ID.Direct) dPos = DirPosn                                 ;
            else                                         dPos = GetNextCmdTrg(Axe , Cmd , Step , FIndex);

            if      (this[(int)Axe].GetTrgPos() < (dPos + this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.InPos])) return MOTR_TARG_POS;
            else if (this[(int)Axe].GetTrgPos() > (dPos - this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.InPos])) return MOTR_TARG_NEG;

            return MOTR_TARG_STP;
        }
        public int WhreTarget(EN_MOTR_ID Axe  , double dPos)
        {
            if(WrongAXE(Axe)) return MOTR_TARG_STP;

            if      (this[(int)Axe].GetTrgPos() < (dPos + this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.InPos])) return MOTR_TARG_POS;
            else if (this[(int)Axe].GetTrgPos() > (dPos - this[(int)Axe].MP.dPosn[(int)EN_POSN_ID.InPos])) return MOTR_TARG_NEG;

            return MOTR_TARG_STP;
        }
        //---------------------------------------------------------------------------
        //Motor Position Check
        public EN_WHERE_SPD fn_WhrerSPD(bool plate = false, bool ChkTrg = false)
        {

            double dStartPos = 0.0;
            double dEndPos   = 0.0;
            double dCmd      = 0.0;
            int    nSPD_X    = (int)EN_MOTR_ID.miSPD_X;
            double dInpos    = AXIS[nSPD_X].m_dInPos;

            double dXOffset  = 12;

            /*
            
            "Wait Pos."                  , "mm", EN_POSN_ID.Wait1  , 3, EN_MOTR_ID.miSPD_X);
            "Polishing Pos."             , "mm", EN_POSN_ID.User1  , 3, EN_MOTR_ID.miSPD_X);
            "Cleaning Pos"               , "mm", EN_POSN_ID.User2  , 3, EN_MOTR_ID.miSPD_X);
            "Polishing Tool Pick Pos"    , "mm", EN_POSN_ID.FSP1_1 , 3, EN_MOTR_ID.miSPD_X);
            "Cleaning Tool Pick Pos"     , "mm", EN_POSN_ID.FSP2_1 , 3, EN_MOTR_ID.miSPD_X);
            "Tool Discard Pos."          , "mm", EN_POSN_ID.User4  , 3, EN_MOTR_ID.miSPD_X);
            "Force Check Pos."           , "mm", EN_POSN_ID.User5  , 3, EN_MOTR_ID.miSPD_X);

            "Plate Polishing P/P Pos."   , "mm", EN_POSN_ID.User6  , 3, EN_MOTR_ID.miSPD_X);
            "Plate Cleaning P/P Pos."    , "mm", EN_POSN_ID.User7  , 3, EN_MOTR_ID.miSPD_X);
            "Plate Load P/P Pos."        , "mm", EN_POSN_ID.User8  , 3, EN_MOTR_ID.miSPD_X);
            "Cup Storage P/P Pos."       , "mm", EN_POSN_ID.User15 , 3, EN_MOTR_ID.miSPD_X);
            "Polishing Cup P/P Pos."     , "mm", EN_POSN_ID.User16 , 3, EN_MOTR_ID.miSPD_X);

            "Polishing Util Check Pos."  , "mm", EN_POSN_ID.User9  , 3, EN_MOTR_ID.miSPD_X);
            "Cleaning Util Check Pos."   , "mm", EN_POSN_ID.User10 , 3, EN_MOTR_ID.miSPD_X);
            "Loading Vision Pos.(PRE)"   , "mm", EN_POSN_ID.User11 , 3, EN_MOTR_ID.miSPD_X);
            "Polishing Vision Pos."      , "mm", EN_POSN_ID.User12 , 3, EN_MOTR_ID.miSPD_X);
            "Storage Vision Align Pos.01", "mm", EN_POSN_ID.User13 , 3, EN_MOTR_ID.miSPD_X);
            "Storage Vision Align Pos.02", "mm", EN_POSN_ID.User14 , 3, EN_MOTR_ID.miSPD_X);

             */

            dCmd = ChkTrg ? MOTR[nSPD_X].GetTrgPos() : MOTR[nSPD_X].GetEncPos();

            if(!plate)
            {
                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.Wait1] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.Wait1] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpWAIT;

                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User1] - dInpos - dXOffset;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User1] + dInpos + dXOffset;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpPOLI;

                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User2] - dInpos - 6;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User2] + dInpos + 6;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpCLEN;

                //JUNG/201223/Check Direct Position 
                if (FM.m_stMasterOpt.nUseDirPos == 1)
                {
                    dCmd = MOTR[nSPD_X].GetEncPos();

                    ST_PIN_POS Pos1 = new ST_PIN_POS(0);
                    ST_PIN_POS Pos2 = new ST_PIN_POS(0);

                    // 
                    Pos1 = DM.STOR[(int)EN_STOR_ID.POLISH].GetPinPos(0, 0);
                    Pos2 = DM.STOR[(int)EN_STOR_ID.POLISH].GetPinPos(DM.STOR[siPolish]._nMaxRow-1, 0);
                    dEndPos = Pos1.dXPos > Pos2.dXPos ? Pos1.dXPos : Pos2.dXPos;

                    Pos1 = DM.STOR[(int)EN_STOR_ID.CLEAN].GetPinPos(0, DM.STOR[siClean]._nMaxCol - 1); //JUNG/210112
                    Pos2 = DM.STOR[(int)EN_STOR_ID.CLEAN].GetPinPos(DM.STOR[siClean]._nMaxRow - 1, DM.STOR[siClean]._nMaxCol-1);
                    dStartPos  = Pos1.dXPos < Pos2.dXPos ? Pos1.dXPos : Pos2.dXPos;

                    if ((dCmd >= (dStartPos - dInpos)) && (dCmd <= (dEndPos + dInpos))) return EN_WHERE_SPD.wpPTOOLPICK;

                }
                else
                { 
                    if (MOTR[(int)EN_MOTR_ID.miSPD_X].m_iProcessDir == 1)
                    {
                        double dStrWidth = FM.m_stProjectBase.nStorage_Col * GetPitch(EN_MOTR_ID.miSPD_X);
                        dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP1_1] - dInpos;
                        dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP1_1] + dInpos + dStrWidth;
                        if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpPTOOLPICK;

                        dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP2_1] - dInpos;
                        dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP2_1] + dInpos + dStrWidth;
                        if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpPTOOLPICK;
                    }
                    else
                    {
                        double dStrWidth = FM.m_stProjectBase.nStorage_Col * GetPitch(EN_MOTR_ID.miSPD_X);
                        dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP1_1] + dInpos;
                        dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP1_1] - dInpos - dStrWidth;
                        if ((dCmd <= dStartPos) && (dCmd >= dEndPos)) return EN_WHERE_SPD.wpPTOOLPICK;

                        dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP2_1] + dInpos;
                        dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP2_1] - dInpos - dStrWidth;
                        if ((dCmd <= dStartPos) && (dCmd >= dEndPos)) return EN_WHERE_SPD.wpPTOOLPICK;

                    }
                }

                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User4] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User4] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpTOOLOUT;

                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User5] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User5] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpFORCECHK;


            }
            else
            {
                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User6] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User6] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpPLATEPOLI;

                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User7] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User7] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpPLATECLEN;

                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User8] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User8] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpPLATELOAD1;


                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User15] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User15] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpCUPSTORAGE;

                dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User16] - dInpos;
                dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User16] + dInpos;
                if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpCUPPOLISH;


            }

            dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User9] - dInpos;
            dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User9] + dInpos;
            if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpUTILCHECK_P;

            dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User10] - dInpos;
            dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User10] + dInpos;
            if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpUTILCHECK_C;

            dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User11] - dInpos;
            dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User11] + dInpos;
            if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpVISNLOAD;

            dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User12] - dInpos;
            dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User12] + dInpos;
            if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpVISNPOLISH;

            dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User13] - dInpos;
            dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User13] + dInpos;
            if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpALIGNSTRG1;

            dStartPos = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User14] - dInpos;
            dEndPos   = MOTR[nSPD_X].MP.dPosn[(int)EN_POSN_ID.User14] + dInpos;
            if ((dCmd >= dStartPos) && (dCmd <= dEndPos)) return EN_WHERE_SPD.wpALIGNSTRG2;

            
            return EN_WHERE_SPD.wpUnkonw;
        }
        //Converter
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public double ConvVelToRPM(double dVel, double dDist = 1.0) 
        { // mm/s -> RPM ,  dDist :  //회전당 이동 거리
            double dRPM  = 0.0  ;

            dRPM = (dVel*60)/dDist;

            return dRPM;
        } 
        public double ConvRpmToVel(double dRPM, double dDist = 1.0) 
        { // RPM  -> mm/s ,  dDist :  //회전당 이동 거리 
            double dVel = 0.0;

            dRPM = (dDist*60)/dRPM;

            return dVel;
        } 

        //Motion.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool    EmrgStop  (EN_MOTR_ID Axe) 
        { //지정 비상 정지.
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].EmrgStop();                    
        } 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void    EmrgStop ( ) 
        {//전제 비상 정지.
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
                EmrgStop((EN_MOTR_ID)Axe); 
        } 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool    Stop  (EN_MOTR_ID Axe , double Dec = 0.1) 
        {//지정 모터 감속 정지.
            if(WrongAXE(Axe)) 
            {
                for (int iAxis = 0 ; iAxis < m_iNumOfMotr ; iAxis++) this[(int)iAxis].Stop(true, Dec);     
                return true;
            }    

            return this[(int)Axe].Stop(true, Dec);               
        } 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void    Stop  (   ) 
        {  //전제 모터 감속 정지.
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
                Stop((EN_MOTR_ID)Axe);     
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MoveHome (EN_MOTR_ID Axe ) 
        { //지정 모터 Home. (모터 자체의 Home 작업이며, 각 Part별 Home 작업과 구별)
            if(WrongAXE(Axe)) return false; 

			if(!MOTR.CheckCrash(Axe, EN_COMD_ID.Home)) return false;

            return this[(int)Axe].MoveHome();                    
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MoveMotrR(EN_MOTR_ID Axe, double P, double V) //상대 위치 이동.
        {
            //if ((int)Axe < 0 || (int)Axe >= m_iNumOfMotr) return false;
            if ((int)Axe < 0 || Axe > EN_MOTR_ID.miCLN_Y) return false; //JUNG/200523/

            //
            if (!CheckCrash(Axe, EN_COMD_ID.Direct, NONE_STEP, EN_FPOSN_INDEX.NONE, P)) return false;


            //Check MinMax
            //if (!CheckMinMax(Axe, P, V, A)) { InspectMinMax(); return false; }

            //Go Move.
            return this[(int)Axe].MoveR(P, V);
            
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MoveMotr (EN_MOTR_ID Axe , double P, double V, double A , bool   R      ) //상대 위치 이동.
        {
            if((int)Axe<0 || (int)Axe>=m_iNumOfMotr) return false;

            //
            if (!CheckCrash(Axe, EN_COMD_ID.Direct , NONE_STEP , EN_FPOSN_INDEX.NONE , P)) return false;


            //Check MinMax
            if (!CheckMinMax(Axe , P , V , A)) { InspectMinMax(); return false; }

            //Go Move.
            if (R) return this[(int)Axe].MoveR(P , V , A);
            else   return this[(int)Axe].MoveA(P , V , A);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MoveMotr(EN_MOTR_ID Axe , double P, double V, double A, double D = 0.0) //절대 위치 이동.
        {
            if((int)Axe<0 || (int)Axe>=m_iNumOfMotr) return false;

            //
            if (!CheckCrash(Axe, EN_COMD_ID.Direct , NONE_STEP , EN_FPOSN_INDEX.NONE , P)) return false;


            //Check MinMax
            if (!CheckMinMax(Axe , P , V , A)) { InspectMinMax(); return false; }
            if (!CheckMinMax(Axe , P , V , D)) { InspectMinMax(); return false; }

            //Go Move.
            return this[(int)Axe].MoveA(P , V , A , D);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MoveMotr (EN_MOTR_ID Axe , double P, EN_MOTR_VEL iSPD = EN_MOTR_VEL.Normal)
        {
            if((int)Axe<0 || (int)Axe>=m_iNumOfMotr) return false;

            double V, A, D;

            if(iSPD == EN_MOTR_VEL.Normal) 
            {
                switch ((EN_SPED_MODE)m_iSpedMode) 
                {
                    default               : V = this[(int)Axe].MP.dVel[(int)EN_MOTR_VEL.Dry ]; break;
                    case EN_SPED_MODE.Dry : V = this[(int)Axe].MP.dVel[(int)EN_MOTR_VEL.Dry ]; break;
                    case EN_SPED_MODE.Work: V = this[(int)Axe].MP.dVel[(int)EN_MOTR_VEL.Work]; break;
                }
                A = this[(int)Axe].MP.dAcc[(int)EN_MOTR_VEL.Work];
                D = this[(int)Axe].MP.dDec[(int)EN_MOTR_VEL.Work];
            }
            else 
            {
                if ((int)iSPD<0 || (int)iSPD>=MAX_SPED) return false;
                
                V = this[(int)Axe].MP.dVel[(int)iSPD];
                A = this[(int)Axe].MP.dAcc[(int)iSPD];
                D = this[(int)Axe].MP.dDec[(int)iSPD];

            }

            //Check MinMax
            if (!CheckMinMax(Axe , P , V , A)) { InspectMinMax(); return false; }
            if (!CheckMinMax(Axe , P , V , D)) { InspectMinMax(); return false; }

            //
            if (!CheckCrash(Axe, EN_COMD_ID.Direct , NONE_STEP , EN_FPOSN_INDEX.NONE , P)) return false;

            //Go Move.
            return this[(int)Axe].MoveA(P , V , A , D);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MoveJog (EN_MOTR_ID Axe , bool Dir, EN_MOTR_VEL iSPD = EN_MOTR_VEL.Normal) 
        {//JOG 이동.
            if(WrongAXE(Axe)) return false; 

            //
            if (!CheckCrash(Axe)) return false;

            return this[(int)Axe].MoveJog (Dir   , iSPD ); 
        } 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MotnDone(EN_MOTR_ID Axe , bool ChkEnc , double Inp ) 
        {//사용자 임의 지정 InPos의 Motion Done 확인.
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].MotnDone(ChkEnc, Inp);   
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool MotnDone(EN_MOTR_ID Axe ) 
        {//설정 InPos의 Motion Done 확인.
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].MotnDone();              
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetVGain (EN_MOTR_ID Axe , double V) 
        {
            if(WrongAXE(Axe)) return; 
            this[(int)Axe].SetVGain(V);           
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetAGain(EN_MOTR_ID Axe , double A) 
        {
            if(WrongAXE(Axe)) return; 
            this[(int)Axe].SetAGain(A);           
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetGain (EN_MOTR_ID Axe , double V    , double A) 
        {
            if(WrongAXE(Axe)) return; 
            this[(int)Axe].SetGain (V,A);         
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetSpeedMode(bool Run)
        {
           m_iSpedMode = Run ? (int)EN_SPED_MODE.Work : (int)EN_SPED_MODE.Dry;            
        }
 
        //---------------------------------------------------------------------------
        public bool MoveAsComd  (EN_MOTR_ID Axe , EN_COMD_ID Comd , EN_MOTR_VEL iSPD , int Step  = 0, 
                                 EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE, double DirPosn = 0.0   ) //Command ID에 따른 위치 이동.
        {
            //To remove incorrect Axe.
            if((int)Axe < 0 || (int)Axe >= m_iNumOfMotr) return false;

            //Command
            double dPosn;
            switch (Comd) 
            {
                case EN_COMD_ID.Stop      : return this[(int)Axe].Stop    (true              );
                case EN_COMD_ID.EStop     : return this[(int)Axe].EmrgStop(                  );
                case EN_COMD_ID.Home      : return MoveHome               (Axe               );
                case EN_COMD_ID.JogP      : return MoveJog                (Axe  , true , iSPD);
                case EN_COMD_ID.JogN      : return MoveJog                (Axe  , false, iSPD);
            }

            dPosn = this[(int)Axe].GetNextCmdTrg(Comd ,Step ,FIndex ,DirPosn);

            //Check Crash.
            /*Move Motor로 이동
            if(Comd !=  EN_COMD_ID.Direct) {
                if (!CheckCrash(Axe, Comd , Step, FIndex)) return false;
                }
            else {
                if (!CheckCrash(Axe, Comd , vDEF.NONE_STEP , EN_FPOSN_INDEX.NONE , DirPosn)) return false;
            }
            */

            return MoveMotr(Axe , dPosn, iSPD);
        }
        //---------------------------------------------------------------------------
        public double  GetNextCmdTrg(EN_MOTR_ID Axe , EN_COMD_ID Comd , int Step  = 0, 
                                     EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE, double DirPosn = 0.0   ) //다음 모터의 Command Target.
        {
            double dPosn = this[(int)Axe].GetNextCmdTrg(Comd ,Step ,FIndex ,DirPosn);
            return dPosn;
        }
        //---------------------------------------------------------------------------
        public double  GetLinear (double dXi, double dX1, double dX2, double dY1, double dY2)
        {
            double dA      = 0.0;
            double dB      = 0.0;
            double dLinear = 0.0;

            if(dX2 - dX1 == 0) return 0.0;

            dA      = (dY2 -  dY1) / (dX2 - dX1);
            dB      =  dY1 -         (dA  * dX1);
            dLinear = (dA  *  dXi) +  dB        ;

            return dLinear;
        }
        public double  GetCalcStrtPlateX     (int iTopId, int iBtmId, int iWorkRow)
        {
            
            //double dXOff_LeftTop  ;
            //double dXOff_RightTop ;
            //double dXOff_LeftBtm  ;
            //double dXOff_RightBtm ;
             
            double dCalcPosnX     = 0.0;

 
            return dCalcPosnX;
        }
        public double  CalExpansion         (EN_MOTR_ID Motr, double ColPitch , double Min , double Max)
        {
            //double dRealSize;
            //double dMotrSize;
            double dPosn     = 0.0;

            return dPosn;
        }       
        //Check Part Motr.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool CheckPartMotr(EN_MOTR_ID Axe , int Part) 
        {
            if(WrongAXE(Axe)) return false; 
            return this[(int)Axe].CheckPartMotr(Part); 
        }
 
        //Display Motor Set Form
        public void    DispParamFrm         (EN_MOTR_ID Axe           ) 
        {
            if(WrongAXE(Axe)) return      ; 
            this[(int)Axe].DispParamFrm(); 
        }

        //Update.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void fn_Update (bool Run)
        {
            try
            {
                m_iSpedMode = Run ? (int)EN_SPED_MODE.Work : (int)EN_SPED_MODE.Dry;

                if (!m_bConnect    ) return;
                if (IO._bDrngReboot) return; //JUNG/200915

                UpdateAxe1();
                UpdateAxe2();
            }
            catch (Exception e)
            {
                LOG.ExceptionTrace("Motor Update", e);
                return;
            }
        }
        //---------------------------------------------------------------------------
        public void  UpdateAxe1()
        {
            try
            {
                //Set Speed Mode.
                for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
                {
                    this[(int)Axe].Update();
                }
            }
            catch (Exception e)
            {
                LOG.ExceptionTrace("Motor UpdateAxe1", e);
                return;
            }

        }
        //---------------------------------------------------------------------------
        public void UpdateAxe2()
        {
            try
            {
                fn_CheckZAxisBreak();
                fn_ReadHomeEnd();

                //for (int n = 0; n < MAX_MOTOR_SMC; n++)
                //{
                //    SMC[n].fn_Update();
                //}
                SMC[0].fn_Update();
                SMC[1].fn_Update();
                SMC[2].fn_Update();

            }
            catch (Exception e)
            {
                LOG.ExceptionTrace("Motor UpdateAxe2", e);
                Console.WriteLine("Motor UpdateAxe2" + e.Message);
                return;
            }

        }
        //---------------------------------------------------------------------------
        private void fn_CheckZAxisBreak()
        {
            //Check Z2 Axis Break
            IO.fn_Z2Unlock (AXIS[(int)EN_MOTR_ID.miSPD_Z1].GetServo());
            IO.fn_TRZUnlock(AXIS[(int)EN_MOTR_ID.miTRF_Z ].GetServo());
        }
        //---------------------------------------------------------------------------
        private void fn_ReadHomeEnd()
        {
            //Read Data
            objVar    = ACSAPI.ReadVariable("HomeFlag");
            arHomeEnd = objVar as Array;
            Array.Copy(arHomeEnd, m_nHomeEnd, m_nHomeEnd.Length);

            for (int n = 0; n<(int)EN_MOTR_ID.miSPD_Z1; n++) //Before SMC 
            {
                //this[n].SetHomeEnd(m_nHomeEnd[n]==1 ? true : false);
                //this[n].SetHomeEnd((m_nHomeEnd[n] == 1 && this[n].GetServo()) ? true : false); //JUNG/200610
                this[n].SetHomeEnd((m_nHomeEnd[n] == 1 && this[n].GetServo() && this[n].GetHomeEndDone()) ? true : false); //JUNG/200616
            }

            //
            if (!m_bInit)
            {
                m_bInit = true;
                
                for (int n = 0; n < (int)EN_MOTR_ID.miSPD_Z1; n++) //Before SMC 
                {
                    this[n].SetHomeEndDone(m_nHomeEnd[n] == 1? true : false);
                }
            }



        }
        //Read/Write Para.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Load(bool IsLoad, string Device, EN_SEQ_ID SelPart = EN_SEQ_ID.ALL)
        {
            //Check Device.
            if (Device == "") return;

            //Local Var.
            string sPath = string.Empty;

            //Make Dir.
            sPath = UserFile.fn_GetExePath() + "Project";
            UserFile.fn_CheckDir(sPath);
            sPath = sPath + "\\" + Device;
            UserFile.fn_CheckDir(sPath);

            //Load Motor.
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {
                 //Check Part Motr.
                if (SelPart != EN_SEQ_ID.ALL)
                {
                     if (!CheckPartMotr((EN_MOTR_ID)Axe, (int)SelPart)) continue;
                }
                //Load.
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (IsLoad)
                {
                    this[(int)Axe].Load(IsLoad , sPath, this[(int)Axe].MP);
                    //
                }

                //DefSetPos();

                //Save.
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (!IsLoad) 
                {
                    //
                    this[(int)Axe].Load(IsLoad , sPath, this[(int)Axe].MP);
                }
            }

            //
            SetAxis_AsDevice();

        }
        //Read/Write Para.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Load(ref MOTN_PARA mPara, int nAxis, string Device, EN_SEQ_ID SelPart = EN_SEQ_ID.ALL)
        {
            //Check Device.
            if (Device == "") return;

            //Local Var.
            string sPath = string.Empty;

            //Make Dir.
            sPath = UserFile.fn_GetExePath() + "Project";
            UserFile.fn_CheckDir(sPath);
            sPath = sPath + "\\" + Device;
            UserFile.fn_CheckDir(sPath);

            if ((int)SelPart < 0) return;

            this[(int)nAxis].Load(true, sPath, this[(int)nAxis].MP);
            mPara.dPosn = this[(int)nAxis].MP.dPosn;
            mPara.sPosn_Desc = this[(int)nAxis].MP.sPosn_Desc;

            ////Load Motor.
            //for (int Axe = 0; Axe < m_iNumOfMotr; Axe++)
            //{
            //    //Check Part Motr.
            //    if (SelPart != EN_SEQ_ID.ALL)
            //    {
            //        if (!CheckPartMotr((EN_MOTR_ID)Axe, (int)SelPart)) continue;
            //    }
            //    //Load.
            //    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


            //    //DefSetPos();

            //    //Save.
            //    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //    if (!IsLoad)
            //    {
            //        //
            //        this[(int)Axe].Load(IsLoad, sPath, this[(int)Axe].MP);
            //    }
            //}

            //
            SetAxis_AsDevice();

        }

        //Read/Write Para.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void LoadCommSpd(bool IsLoad, EN_SEQ_ID SelPart = EN_SEQ_ID.ALL)
        {
            //Load Motor Speed.
            for (int Axe = 0 ; Axe < m_iNumOfMotr ; Axe++) 
            {
                 //Check Part Motr.
                if (SelPart != EN_SEQ_ID.ALL)
                     if (!CheckPartMotr((EN_MOTR_ID)Axe, (int)SelPart)) continue;
                
                this[(int)Axe].LoadCommSpd(IsLoad);
            }

        }
        //---------------------------------------------------------------------------
        public void LoadAxe(bool IsLoad , string Device , int Axe)
        {
            //Check Device.
            if (Device == "") return;

            //Local Var.
            string sPath;

            //Make Dir.
            sPath = UserFile.fn_GetExePath() + "Project";
            UserFile.fn_CheckDir(sPath);
            
            sPath = sPath  + "\\" + Device;
            UserFile.fn_CheckDir(sPath);

            //Load Motor.```
            if (IsLoad) this[(int)Axe].Load(IsLoad , sPath, this[(int)Axe].MP);

            //
            DefSetPos();

            //Save.
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (!IsLoad) this[(int)Axe].Load(IsLoad , sPath, this[(int)Axe].MP);
        }
        //---------------------------------------------------------------------------
        public void fn_LoadMotrDstb(bool IsLoad)                                      
        {
            //Local Var.
            string sName = string.Empty;
            

            //Make Dir.
            string sPath = UserFile.fn_GetExePath()+ "System";
            UserFile.fn_CheckDir(sPath);
            sPath += "\\MotorDstb.INI";

            //Load Master Options.
            if (IsLoad) 
            {
                //Normal Load.
                for(int i=0;i<m_iNumOfDstb;i++) 
                {
                    sName = string.Format($"DSTB{i}");
                    
                    DstbPosn[i].dPos = UserINI.fn_Load("MASTER_DSTB", sName, DstbPosn[i].dPos, sPath);
                }
            }
            else 
            {
                for(int i=0;i<m_iNumOfDstb;i++) 
                {
                    sName = string.Format($"DSTB{i}");
                    UserINI.fn_Save("MASTER_DSTB", sName, DstbPosn[i].dPos, sPath);
                }
            }
        }                                             
        //---------------------------------------------------------------------------
        //Get Manual Define                    
        public int     ManNoStop    (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iManStop    ;}
        public int     ManNoJog     (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iManJog     ;}
        public int     ManNoPitch   (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iManPitch   ;}
        public int     ManNoServo   (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iManServo   ;}
        public int     ManNoAlarm   (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iManAlarm   ;}
        public int     ManNoDirect  (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iManDirect  ;}
        public int     ManNoHome    (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iManHome    ;}
        
        //--------------------------------------------------------------------------
        public bool GetPosnByManNo(int iMotr, int iManNo, out double dPosn)
        {
            int iPosnId;
            dPosn = 0.0;
            for(int i=0; i< MAX_SEQ_PART; i++) {
                for(int j=0; j<Dat[i].m_iItemCnt; j++) {
                    if(j >= MAX_ITEM) continue;
                    if(iMotr == Dat[i].Set[j].m_iMotor && iManNo == Dat[i].Set[j].m_iManNo) {

                        if(Dat [i].Set[j].m_bDefUserMan) return false; 

                        iPosnId = Dat [i].Set[j].m_iPosnId;

                        if(iPosnId<0 || iPosnId>=MAX_POSN) continue;
                        
                        dPosn = MOTR[iMotr].MP.dPosn[iPosnId];
                        
                        return true;
                    }
                }
            }

            return false;
        }

        //---------------------------------------------------------------------------                               
        //Get Error Define             
        public int     ErrNoAlarm   (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrAlarm   ;}
        public int     ErrNoCW      (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrCW      ;}
        public int     ErrNoCCW     (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrCCW     ;}
        public int     ErrNoHome    (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrHome    ;}
        public int     ErrNoControl (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrControl ;}
        public int     ErrNoHold    (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrHold    ;}
        public int     ErrNoPos     (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrPos     ;}
        public int     ErrNoVel     (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrVel     ;}
        public int     ErrNoAcc     (EN_MOTR_ID Axe) {if(WrongAXE(Axe)) return -1; return this[(int)Axe].m_iErrAcc     ;}
        //---------------------------------------------------------------------------                               
        //Default Pos Setting          
        public void    DefSetPos    ()
        {
            //UserSet - Motor Default Position 설정 
	        //AXIS[mBNF_Y1].MP.dPosn[mpos_Stck] = AXIS[mBNF_Y1].MP.dPosn[mpos_Sply] - 1.0;

        }
        //---------------------------------------------------------------------------
        public string  GetPosnName      (int iPartId, int Axe, double dSrhPosn)
        {
             string sPosnName = "";
//             int    iPosnId;
//             double dPosn  ;
// 
//             for (int i = 0; i < cDEF.POSN.Dat[iPartId].m_iItemCnt; i++)
//             {
//                 if(i>=vDEF.MAXITEM) continue;
//                 if (cDEF.POSN.Dat[iPartId].Set[i].m_iMotor != Axe) continue;
//                 
//                 iPosnId = cDEF.POSN.Dat[iPartId].Set[i].m_iPosnId;
//                 
//                 if (cDEF.POSN.Dat[iPartId].Set[i].m_bHomeOffset) dPosn = this[(int)Axe].MP.dPosn[iPosnId] + this[(int)Axe].m_dHomeOff;
//                 else                                             dPosn = this[(int)Axe].MP.dPosn[iPosnId];
//                 
//                 dPosn = this[(int)Axe].MP.dPosn[iPosnId];
//                 if(dSrhPosn == dPosn) 
//                 {
//                     sPosnName = cDEF.POSN.Dat[iPartId].Set[i].m_sName;
//                     break;
//                 }
//             }
             return sPosnName;
        }
        
        //---------------------------------------------------------------------------
        public int fn_GetPart(int motor)
        {
            if (motor <= (int)EN_MOTR_ID.miNone ) return (int)EN_SEQ_ID.SPINDLE;
            if (motor >= (int)EN_MOTR_ID.EndOfId) return (int)EN_SEQ_ID.SPINDLE;

            EN_MOTR_ID SelMotr = (EN_MOTR_ID)motor;

            switch (SelMotr)
            {
                case EN_MOTR_ID.miSPD_X:
                case EN_MOTR_ID.miSPD_Z:
                case EN_MOTR_ID.miSPD_Z1:
                    return (int)EN_SEQ_ID.SPINDLE;

                case EN_MOTR_ID.miPOL_Y:
                case EN_MOTR_ID.miPOL_TH:
                case EN_MOTR_ID.miPOL_TI:

                    return (int)EN_SEQ_ID.POLISH;
                    
                case EN_MOTR_ID.miCLN_Y:
                case EN_MOTR_ID.miCLN_R:
                    return (int)EN_SEQ_ID.CLEAN;

                case EN_MOTR_ID.miSTR_Y:
                    return (int)EN_SEQ_ID.STORAGE;

                case EN_MOTR_ID.miTRF_T:
                case EN_MOTR_ID.miTRF_Z:
                    return (int)EN_SEQ_ID.TRANSFER;
                
                default:
                    return (int)EN_SEQ_ID.SPINDLE;
            }
        }
        //---------------------------------------------------------------------------
        public bool fn_ACSDisConnect()
        {
            ACSAPI.CloseComm();

            UserFunction.fn_WriteLog("ACS Motor Close");
            return true;
        }
        //---------------------------------------------------------------------------
        public Api fn_GetAPI()
        {
            return ACSAPI; 
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_IsSMCMotor(EN_MOTR_ID motr)
        {
            if (motr == EN_MOTR_ID.miSPD_Z1) return true;
            if (motr == EN_MOTR_ID.miTRF_T ) return true;
            if (motr == EN_MOTR_ID.miTRF_Z ) return true;

            return false;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_Reset()
        {
            SetAlarm(true);
            fn_ResetSMC();
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_ResetSMC()
        {
            SMC[0].Reset();
            SMC[1].Reset();
            SMC[2].Reset();
        }

    }


    //---------------------------------------------------------------------------
    /************************************************************************/
    /* Set Part Class                                                       */
    /************************************************************************/
    public class TSetPart
    {
        public TSetItem[]  Set = new TSetItem[MAX_ITEM];
        public int         m_iItemCnt ;
        public int         m_iMotorCnt;
        public int[]       m_iPosnCnt = new int[(int)EN_MOTR_ID.EndOfId];
        public string      m_sName    ;    
        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public TSetPart()
        {
            for (int i=0; i<MAX_ITEM; i++) {
                Set[i] = new TSetItem (); 
            }
        }
    }

    //---------------------------------------------------------------------------
    public class TSetItem
    {
        //Vars.
        public string m_sName      ;
        public string m_sUnit      ;
        public double m_dVal       ;   
        public int    m_iDigit     ;
        public int    m_iMotor     ;
        public double m_dMin       ;
        public double m_dMax       ;
        public int    m_iManNo     ;
        public int    m_iPosnKind  ;
        public int    m_iPosnId    ;
        public bool   m_bHomeOffset;
        public bool   m_bDefUserMan;

        //Property.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int _int
        {
            get { return (int)m_dVal; }
            set { m_dVal = (double)value; }
        }
        public double _Pos
        {
            get { return m_dVal*1000; }
            set { m_dVal = value/1000; }
        }
        public string _gVal
        {
            get { return tFormat(m_dVal); }
            set { m_dVal = Convert.ToDouble(value); }
        }
        public double _Val
        {
            get { return m_dVal; }
            set { m_dVal = value; }
        }

        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public TSetItem()
        {
            m_sName         = string.Empty;
            m_sUnit         = string.Empty;
            m_dVal          = 0.0;
            m_iDigit        = 0;
            m_iMotor        = 0;
            m_dMin          = 0.0;
            m_dMax          = 0.0;
            m_iManNo        = 0;
            m_iPosnKind     = 0;
            m_iPosnId       = 0;
            m_bHomeOffset   = false;
            m_bDefUserMan   = false;
        }
        //--------------------------------------------------------------------------
        public string tFormat (double dVal)
        {
            string sTemp = string.Empty;
            if(m_iDigit == 0)          sTemp = string.Format("{0:F4}", m_dVal);
            else
            {
                     if(m_iDigit == 1) sTemp = string.Format($"{m_dVal:F1}");
                else if(m_iDigit == 2) sTemp = string.Format($"{m_dVal:F2}");
                else if(m_iDigit == 3) sTemp = string.Format($"{m_dVal:F3}");
                else                   sTemp = string.Format($"{m_dVal:F4}");
            }
            return sTemp;
        }
        //--------------------------------------------------------------------------
        public string Add(string sStr, bool bMode)
        {
            double dVal = Convert.ToDouble(sStr); 
            double dAdd = 1;
            for(int i=0; i<m_iDigit-2; i++) dAdd = dAdd * 10;
            if(dAdd == 0) dAdd = 1.0;
            else          dAdd = 1.0/ dAdd;

            if(bMode) { dVal += dAdd; if(dVal>m_dMax) dVal = m_dMax; }
            else      { dVal -= dAdd; if(dVal<m_dMin) dVal = m_dMin; }
            return tFormat(dVal);
        }

        //-------------------------------------------------------------------------------------------------


    }



}
