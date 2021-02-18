using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using System.Windows;

namespace WaferPolishingSystem.Define
{
    /************************************************************************/
    /* CONSTANT                                                             */
    /************************************************************************/
    public class UserConst
    {
        
        //Brush Color Define
        static public SolidColorBrush colormenudefaltbk  = new SolidColorBrush(Colors.LightGray);
        static public SolidColorBrush G_COLOR_FORMBACK   = new SolidColorBrush(Color.FromArgb(255, 0, 81, 165));
                                                         
        static public SolidColorBrush G_COLOR_PAGEBACK   = new SolidColorBrush(Color.FromArgb(255, 247, 247, 247));
        static public SolidColorBrush G_COLOR_SUBMENU    = new SolidColorBrush(Color.FromArgb(255, 222, 227, 247));
        static public SolidColorBrush G_COLOR_BTNCLICKED = new SolidColorBrush(Color.FromArgb(255,  66, 162, 231));
        static public SolidColorBrush G_COLOR_BTNNORMAL  = new SolidColorBrush(Color.FromArgb(255, 247, 247, 247));
       

        //Color Define
        static public Color G_COLOR_BACKGROUND = Colors.White;


        //
        public const int _MCTYPE_20 = 20;
        public const int _MCTYPE_18 = 18;
        //
        public const int _MCTYPE    = _MCTYPE_18;

        //
        public const int siPolish = 0; //Polishing Storage ID
        public const int siClean  = 1;

        //
        public const int SPLY_SLURRY = 0; //Slurry 
        public const int SPLY_SOAP   = 1; //Soap


        //Storage
        public const int MAX_STORAGE      =  2;
        public const int MAX_STORAGE_R    = 20;
        public const int MAX_STORAGE_C    = 10;
                                          
        public const int STORAGE_PITCH_R  = 5 ; //Pitch R
        public const int STORAGE_PITCH_C  = 5 ; //Pitch C

        //Tool
        public const int MAX_TOOL_COUNT   = 1;
        public const int MAX_TOOL_PIN     = 1;
        public const int MAX_TOOL_PLATE   = 1;
                                          
        //PIN                             
        public const int MAX_PIN_STAT     = 5;
                                          
        //Magazine                        
        public const int MAX_MAGAZINE_R   = 20;
        public const int MAX_MAGAZINE_C   = 2;

        public const int MAGAZINE_PITCH_R = 5;
        public const int MAGAZINE_PITCH_C = 5;

        //
        public const int MAX_SEQ_PART = 5 ;
        public const int MAX_ITEM     = 50;


        //
        public const int MAX_MOTOR     = 11  ;
        public const int MAX_PITCH     = 5   ;
        public const int MAX_POSN      = 100 ;
        public const int MAX_SPED      = 10  ;
        public const int MAX_DLAY      = 10  ;
        public const int MAX_MOTOR_SMC = 3   ;

        //
        public const int MAX_TOOLTYPE  = 2   ;
        public const int MAX_TOOLCOLOR = 5   ;


        //
        public const int MAX_POLI = 10;
        public const int MAX_CLEN = 10;
        public const int MAX_VISN = 10;

        //
        public const int MAX_ERROR       = 600 ;
        public const int MAX_ERROR_START = 100 ;
        public const int MAX_LAMP_KIND   = 10  ;

        //
        public const int MAX_MANUAL = 500;

        //
        public const int ACS_CON_NOR = 0;
        public const int ACS_CON_SIM = 1;

        //

        //Motor.
        //===========================================================================
        public const int    MAX_COMMON_POS       = 80 ;
                                                 
        //Current target position of motor.      
        public const int    MOTR_TARG_NEG        = -1;
        public const int    MOTR_TARG_STP        =  0;
        public const int    MOTR_TARG_POS        =  1;
                                                 
        //Result of position compare       
        public const int    CMPR_SMAL            = -1;
        public const int    CMPR_SAME            =  0;
        public const int    CMPR_LARG            =  1;

        //NONE STEP ID.                          
        public const int    NONE_STEP            = -100;
        public const int    NONE_INDX            = -100;
        public const int    UNKNOWN_AREA         =  -1 ;
                            
        //Speed             
        public const int    SPD_LOW              =  0;
        public const int    SPD_HIGH             =  1;

        //
        public const int    UPDATE_INTERVAL      = 50 ;
        public const int    MAX_INPUT_COUNT_D    = 192;
        public const int    MAX_OUTPUT_COUNT_D   = 96 ;
        public const int    MAX_INPUT_COUNT_A    = 12 ;
        public const int    MAX_OUTPUT_COUNT_A   = 4  ;

        //
        public const bool   flLoad    = true ;
        public const bool   flSave    = false;
                                      
        public const bool   drPOS     = true;
        public const bool   drNEG     = false;
                            
        public const bool   vvClose   = false;
        public const bool   vvOpen    = true ;

        public const int    swOff     = 0;
        public const int    swOn      = 1;
        public const int    ccBwd     = 0;
        public const int    ccFwd     = 1;
        public const int    ccDown    = 0;
        public const int    ccUp      = 1;
        public const int    ccClose   = 0;
        public const int    ccOpen    = 1;
        public const int    ccClamp   = 0;
        public const int    ccUnclamp = 1;
        public const int    ccDeg0    = 0;
        public const int    ccDeg180  = 1;
        public const int    ccLeft    = 0;
        public const int    ccRight   = 1;
                            
        //Max Voltage
        public const int    MAX_VALUE_5V  = 16383;
        public const int    MAX_VALUE_10V = 32767;


        //ACS Buffer No
        public const int BFNo_00_HOME_SPD_X  = 0;   //0. Main X Home Buffer
        public const int BFNo_01_HOME_POL_Y  = 1;   //1. Polishing Y Home Buffer
        public const int BFNo_02_HOME_SPD_Z  = 2;   //2. Main Z Home Buffer
        public const int BFNo_03_HOME_CLN_R  = 3;   //3. Cleaning Theta Home Buffer
        public const int BFNo_04_HOME_POL_TH = 4;   //4. Polishing Theta Home Buffer
        public const int BFNo_05_HOME_POL_TI = 5;   //5. Polishing Tilt Home Buffer
        public const int BFNo_06_HOME_STR_Y  = 6;   //6. Storage Y Home Buffer
        public const int BFNo_07_HOME_CLN_Y  = 7;   //7. Cleaning Y Home Buffer

        public const int BFNo_08_            = 8;   //8. 
        public const int BFNo_09_XSEG_COUNT  = 9;   //9. 
        public const int BFNo_10_            = 10;  //10. 
        public const int BFNo_11_            = 11;  //11. 
        public const int BFNo_12_FORCE_TEST  = 12;  //12. 

        public const int BFNo_13_MILLING     = 13;  //13. Milling Buffer
        public const int BFNo_14_FORCECHECK  = 14;  //14. Force Control Buffer
        public const int BFNo_15_ADD_MAP     = 15;  //15. Ethercat Address Mapping, SMC Interface Buffer

        public const int MAX_ACSBUFF  = 14;  //
        public const int MAX_HOMEBUFF = 7 ;  //


        //---------------------------------------------------------------------------
        public static string[] STR_PLATE_STAT =
        {
            "EMPTY"         , //00//Plate가 없는 상태
            "LOADED"        , //01//Plate Loading 상태 > Pre-Align 진행필요
            "PRE-ALIGN"     , //02//
            "READY"         , //03//작업 준비 상태
            "ALIGN"         , //04//
            "POLISHING"     , //05//Polishing 완료 상태
            "CLEANING"      , //06//Cleaning 완료 상태
            "FINISH"        , //07//배출 상태
            "DEHYDRATE"     , //08//DeHydrate

            "Skip"          , //09//
            "T.W"           , //10//

        };

        public static string[] STR_PIN_STAT =
        {
            "EMPTY"       , //Pin이 없는 상태
            "READY-P"     , //작업 준비 상태
            "USED"        , //사용완료 상태
            "READY-C"     , //작업 준비 상태
            "USED-P"      ,
            "USED-C"      ,
            "MASK"      
           
            //Empty
            //NewPol
            //Used
            //NewCln
            //UsedPol
            //UsedCln
        };
        
        public static string[] STR_SEL_PIN_STAT =
        {
            "EMPTY"       , //Pin이 없는 상태
            "READY"       , //작업 준비 상태
            "USED"        , //사용완료 상태
        };

        public static string[] STR_MAGAZINE_STAT =
        {
            "EMPTY"         , //Magazine 이 없는 상태
            "READY"         , //작업 준비 상태
            "FINISH"        , //사용완료 상태
        };

        public static string[] STR_MAGAZINE_NAME =
        {
            "POLISHING"  , //
            "CLEANING"   , //
            "LOAD"       , //
            "TRANSFER"   , //
            "MAGA01"     , //
            "MAGA02"     , //
        };

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static string[] STR_STORAGE_ID = 
        {
            "POLISH STORAGE",
            "CLEAN STORAGE"
        };
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static string[] STR_MOTOR_ID =
        {
            "SPD_X" ,//SPD_X 
            "POL_Y" ,//POL_Y 
            "SPD_Z" ,//SPD_Z 
            "CLN_R" ,//CLN_R 
            "POL_TH",//POL_TH
            "POL_TI",//POL_TI
            "STR_Y" ,//STR_Y 
            "CLN_Y" ,//CLN_Y 
            "SPD_Z1",//SPD_Z1
            "TRF_Z" ,//TRF_Z 
            "TRF_T" ,//TRF_T 
        };

        public static string[] STRING_MOTOR_ID =
        {
            "Spindle X"     ,
            "Polishing Y"   ,
            "Spindle Z"     ,
            "Cleaning R"    ,
            "Polishing TH"  ,
            "Polishing TI"  ,
            "Storage Y"     ,
            "Cleaning Y"    ,
            "Spindle Z1"    ,
            "Transfer Z"    ,
            "Transfer T"    ,
        };
    }

    /************************************************************************/
    /* USER ENUM                                                            */
    /************************************************************************/
    public class UserEnum
    {

        //---------------------------------------------------------------------------
        //Enum
        //---------------------------------------------------------------------------
        #region [ENUM DEFINE]
        
        public enum EN_PLC_IN
        {
            Leak_Polishing     = 0 ,//00//Leak-Polishing
            Leak_CleanBottom       ,//01//Leak-Cleaning Bottom
            Leak_CleanTop          ,//02//Leak-Cleaning Top
            Leak_LocalBtmPlate     ,//03//Leak-Local Bottom Plate
            Leak_BtmSolBox         ,//04//Leak-Bottom Sol Box
            Leak_Settling          ,//05//Leak-Settling
            Leak_UtilInlet         ,//06//Leak-Utility Inlet
            Leak_LocalFloor        ,//07//Leak-Local Floor
            Accura_Gas             ,//08//Accura Gas
            Accura_Temp            ,//09//Accura Temp
            SICK_DOOR_1            ,//10//SICK Door Signal-01
            SICK_DOOR_2            ,//11//SICK Door Signal-02
        }

        public enum EN_PLC_OUT
        {
            PLC_OUT_00 = 0,//00
            PLC_OUT_01    ,//01
            PLC_OUT_02    ,//02
            PLC_OUT_03    ,//03
            PLC_OUT_04    ,//04
            PLC_OUT_05    ,//05
            PLC_OUT_06    ,//06
            PLC_OUT_07    ,//07
            PLC_OUT_08    ,//08
            PLC_OUT_09    ,//09
        }

        public enum EN_EQ_TO_ACS
        {
            ETA_00_SPD_X_Home_Offset = 0,//00
            ETA_01_POL_Y_Home_Offset    ,//01
            ETA_02_SPD_Z_Home_Offset    ,//02
            ETA_03_CLN_R_Home_Offset    ,//03
            ETA_04_POL_TH_Home_Offset   ,//04
            ETA_05_POL_TI_Home_Offset   ,//05
            ETA_06_STR_Y_Home_Offset    ,//06
            ETA_07_CLN_Y_Home_Offset    ,//07

            DATA_EQ_TO_ACS_08           ,//08
            DATA_EQ_TO_ACS_09           ,//09
            DATA_EQ_TO_ACS_10           ,//10
            DATA_EQ_TO_ACS_11           ,//11
            DATA_EQ_TO_ACS_12           ,//12
            DATA_EQ_TO_ACS_13           ,//13
            DATA_EQ_TO_ACS_14           ,//14
            DATA_EQ_TO_ACS_15           ,//15
            DATA_EQ_TO_ACS_16           ,//16
            DATA_EQ_TO_ACS_17           ,//17
            DATA_EQ_TO_ACS_18           ,//18
            DATA_EQ_TO_ACS_19           ,//19
                 
            ETA_20_Mill_X_start         ,//20//Milling X Start Position
            ETA_21_Mill_X_end           ,//21
            ETA_22_Mill_Y_start         ,//22
            ETA_23_Mill_Y_end           ,//23
            ETA_24_Mill_Y_Pitch         ,//24//Milling Y Pitch
            ETA_25_Mill_Direction       ,//25//0 - Horizontal , 1 - Vertical

            DATA_EQ_TO_ACS_26           ,//26
            DATA_EQ_TO_ACS_27           ,//27
            DATA_EQ_TO_ACS_28           ,//28
            DATA_EQ_TO_ACS_29           ,//29
            DATA_EQ_TO_ACS_30           ,//30

            ETA_31_Mill_Force_Var       ,//31
            DATA_EQ_TO_ACS_32           ,//32
            DATA_EQ_TO_ACS_33           ,//33
            DATA_EQ_TO_ACS_34           ,//34
            DATA_EQ_TO_ACS_35           ,//35
            DATA_EQ_TO_ACS_36           ,//36
            DATA_EQ_TO_ACS_37           ,//37
            DATA_EQ_TO_ACS_38           ,//38
            DATA_EQ_TO_ACS_39           ,//39
            DATA_EQ_TO_ACS_40           ,//40
            DATA_EQ_TO_ACS_41           ,//41
            DATA_EQ_TO_ACS_42           ,//42
            DATA_EQ_TO_ACS_43           ,//43
            DATA_EQ_TO_ACS_44           ,//44
            DATA_EQ_TO_ACS_45           ,//45
            DATA_EQ_TO_ACS_46           ,//46
            DATA_EQ_TO_ACS_47           ,//47
            DATA_EQ_TO_ACS_48           ,//48
            DATA_EQ_TO_ACS_49           ,//49
        }

        public enum EN_ACS_TO_EQ
        {

            ATE_00_SPD_X_Home_Error = 0 ,//00
            ATE_01_POL_Y_Home_Error     ,//01
            ATE_02_SPD_Z_Home_Error     ,//02
            ATE_03_CLN_R_Home_Error     ,//03
            ATE_05_POL_TH_Home_Error    ,//05
            ATE_04_POL_TI_Home_Error    ,//04
            ATE_06_STR_Y_Home_Error     ,//06
            ATE_07_CLN_Y_Home_Error     ,//07
            DATA_ACS_TO_EQ_08           ,//08
            DATA_ACS_TO_EQ_09           ,//09

            ATE_10_SPD_X_Home           ,//10
            ATE_11_POL_Y_Home           ,//11
            ATE_12_SPD_Z_Home           ,//12
            ATE_13_CLN_R_Home           ,//13
            ATE_14_POL_TH_Home          ,//14
            ATE_15_POL_TI_Home          ,//15
            ATE_16_STR_Y_Home           ,//16
            ATE_17_CLN_Y_Home           ,//17
            DATA_ACS_TO_EQ_18           ,//18
            DATA_ACS_TO_EQ_19           ,//19

            ATE_POLISH_TotalCnt         ,//20
            ATE_POLISH_XCnt             ,//21
            ATE_POLISH_YCnt             ,//22
            ATE_Milling_Cnt             ,//23
            DATA_ACS_TO_EQ_24           ,//24
            DATA_ACS_TO_EQ_25           ,//25
            DATA_ACS_TO_EQ_26           ,//26
            DATA_ACS_TO_EQ_27           ,//27
            DATA_ACS_TO_EQ_28           ,//28
            DATA_ACS_TO_EQ_29           ,//29
            ATE_30_SPD_X_Home_Offset    ,//30
            ATE_31_POL_Y_Home_Offset    ,//31
            ATE_32_SPD_Z_Home_Offset    ,//32
            ATE_33_CLN_R_Home_Offset    ,//33
            ATE_34_POL_TH_Home_Offset   ,//34
            ATE_35_POL_TI_Home_Offset   ,//35
            ATE_36_TRF_Y_Home_Offset    ,//36
            ATE_37_STR_Y_Home_Offset    ,//37
            ATE_38_CLN_Y_Home_Offset    ,//38
            ATE_39_POL_Z_Home_Offset    ,//39

            DATA_ACS_TO_EQ_40           ,//40
            DATA_ACS_TO_EQ_41           ,//41
            DATA_ACS_TO_EQ_42           ,//42
            DATA_ACS_TO_EQ_43           ,//43
            DATA_ACS_TO_EQ_44           ,//44
            ATE_45_CheckForceBuffer     ,//45//Check Force Buffer State
            DATA_ACS_TO_EQ_46           ,//46
            DATA_ACS_TO_EQ_47           ,//47
            DATA_ACS_TO_EQ_48           ,//48

            ATE_49_ACSValveState        ,//49//Valve Control State
        }

        public enum EN_SMC_READ
        {
            CURRENT_POS  , //0_6020h(0-31)
            CURRENT_VEL  , //1_6021h(0-31)
            ALRAM        , //2_6010h(15)
            ALRAM_CODE   , //3_6030h(0-7)_Latest alarm code
            HOME_COMPLETE, //4_6010h(10)
            INPOSITION   , //5_6010h(11)
            BUSY         , //6_6010h(8)
            SERVO_ON     , //7_6010h(9)
            AREA         , //8_6010h(12)
            ESTOP        , //9_6010h(14)
            PUSH_FORCE   , //10_6022h
            TARGET_POS   , //11_6023h
            READY        , //12_6011h(4)_READY turns ON when Servo is ON and no alarm is generated.
                            

        }
        public enum EN_SMC_WRITE
        {
            START_FLAG   , //0_7012h(0)
            FAULT_RESET  , //1
            SERVO_ON     , //2
            JOG_LEFT     , //3
            JOG_RIGHT    , //4
            STEP_MOVE    , //5
            HOME         , //6_SETUP
            DEFAULT_PARA , //7
            TARGET_VEL   , //8
            TARGET_POS   , //9_7022h(0-31)_Target position
            
            HOLD         , //10
            SPEED_ON     , //11_7011h(5)_Default Set On
            POSITION_ON  , //12_7011h(6)_Default Set On
            ACC_ON       , //13_7011h(7)_Acceleration On
            DEC_ON       , //14_7011h(8)_Deceleration On

            MOVEMODE     , //15_7020h(0-1)_Movement mode >> 1: ABS (Absolute) 2: INC (Relative)
            ACC_VEL      , //16_7023h(0-15)
            DEC_VEL      , //17_7024h(0-15)

            PUSH_FORCE   , //18_7025h(0-15)
            TRIGGER_LV   , //19_7026h(0-15)
            PUSH_SPEED   , //20_7027h(0-15)
            MOVE_FORCE   , //21_7028h(0-15)
            IN_POSISIONT , //22_702Bh(0-31)_0.01mm




        }

        public enum EN_SHOW_MODE : int
        {
            Normal = 0,
            Modal   ,
        }


        //---------------------------------------------------------------------------
        //UserSet - 간섭 조건 ID 정의 
        public enum EN_DSTB_ID : int
        {
            DP_SPDLZ_POS_MOVE_MAINX   = 0,
            DP_SPDLZ1_POS_MOVE_MAINX     ,
            DP_SPDLX_POS_DSTB_POLIZ      , 
            DP_SPDLZ_POS_MOVE_BTMY       , 
            DP_SPDLZ1_POS_MOVE_BTMY      , 
            DP_CLEANY_POS_MOVE_LOADTURN  ,

            EndOfId
        }


        //---------------------------------------------------------------------------
        public enum EN_MC_TYPE
        {
            MCTYPE_20 = 0, 
            MCTYPE_18        
        }
        
        //---------------------------------------------------------------------------
        public enum EN_SLURRY_TYPE
        {
            slPolish = 0,
            slClean
        }



        //---------------------------------------------------------------------------
        //Motion Part ID
        public enum EN_PART_ID
        {
            piSPDL = 0,
            piPOLI    , //1
            piCLEN    , //2
            piSTRG    , //3
            piLOAD    , //4
            piSYS       //5 

        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public enum EN_MSG_TYPE
        {
            Info    = 0,  
            Check      ,
            Warning    , 
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public enum EN_RUN_MODE
        {
            AUTO_MODE = 0, 
            MAN_MODE     , 
            TEST_MODE      //ONLY MAKER 사용
        }
        public enum EN_MC_MODE
        {
            REMOTE = 0, //Remote Mode
            LOCAL     , //Local Mode
            OFFLINE     //Offline 
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Display Direction.
		public enum EN_MAP_DIR  : int
		{
		    None = -1  ,
		    Deg0       ,  //0
		    Deg90      ,  //90
		    Deg180     ,  //180
		    Deg270     ,  //270
		    Deg270_VMir,  //270 + Vert. Mirror.
		    Deg0_HMir  ,  //Horz. Mirrror.
		    Deg180_VMir   //180 + Vert. Mirror.
		};

        public enum EN_STOR_ID
        {
            None      = -1,
            POLISH    =  0, //Polishing
            CLEAN           //Cleaning
        }

        public enum EN_STOR_TYPE
        {
            None      = -1,
            stPOLISH  = 0 , //Polishing
            stCLEAN         //Cleaning 
        }
        public enum EN_STOR_STAT
        {
            ssNone = -1, //미사용
            ssExist = 0, //사용 가능
            ssEmpty       //사용 가능한데 없는 경우

        }

        //---------------------------------------------------------------------------
        public enum EN_PIN_STAT
        {
            psNone    = -1,
            psEmpty   = 0, //0//
            psNewPol     , //1////Polishing New Tool
            psUsed       , //2//
            psNewCln     , //3////Cleaning New Tool
            psUsedPol    , //4//
            psUsedCln    , //5//
            psMask         //6////Storage 이상 시 Mask 처리

            //psNeedChk    ,

        }
        //PIN Type
        public enum EN_PIN_TYPE
        {
            ptNone  = -1,
            ptPolis = 0,
            ptClean = 1,

        }
        
        //PIN Kind
        public enum EN_PIN_KIND
        {
            pkNone = -1,
            pkUser1 = 0,
            pkUser2 = 1,
            pkUser3 = 2,
            pkUser4 = 3,
            pkUser5 = 4,

        }

        //PIN Find Mode.
        public enum EN_PIN_FIND_MODE
        {
            fmNone    = -1,
            fmExit    = 0,
            fmEmpty   = 1,
            fmNewPol  = 2,
            fmNewCln  = 3,
            fmUsed    = 4,
            fmUsedPol    ,
            fmUsedCln    ,
            fmNeedChk    ,

        }
        //---------------------------------------------------------------------------
        //Plate
        public enum EN_PLATE_STAT
        {
            ptsNone       = -1,
            ptsEmpty      =  0, //00
            ptsLoad           , //01
            ptsPreAlign       , //02
            ptsReady          , //03//
            ptsAlign          , //04//Align 한 생태(at Plate Rotation)
            ptsPolish         , //05//Polishing 완료 한 상태
            ptsClean          , //06//Cleaning 완료 한 상태
            ptsFinish         , //07//작업 완료한 상태 (Move to Magazine)
            ptsDeHydrate      , //08//탈수 작업
                                   
            ptsSkip           , //09
            ptsPolishWait     , //10//Tool Change Wait 


        }
        public enum EN_PLATE_ID
        {
            ptiNone   = -1,
            ptiPolish =  0,
            ptiClean      ,
            ptiLoad       ,  //
            ptiMaga1      , 
            ptiMaga2      , 
            ptiSpindl
        }

        public enum EN_PLATE_INFO
        {
            ifNone    = 0,
            ifVisnErr = 1,

        }

        //Plate Find Mode.
        public enum EN_PLATE_FIND_MODE
        {
            fmpNone = -1,
            fmpReady = 0,
            fmpEmpty = 1, //1
            fmpFinish, //2
            fmpAlign, //3
            fmpPolish, //4
            fmpWashing, //5
            fmpSkip          //6  

        }


        //---------------------------------------------------------------------------
        //Log
        public enum EN_LOG_TYPE
        {
            ltTrace = 0,
            ltEvent,
            ltJam  ,
            ltRS232,
            ltPMC  ,
            ltLot  , 
            ltMill ,
            ltSMC   ,
            ltIO   ,
            ltError,
            ltVision,
            ltTest,
            ltREST
        }
        public enum EN_TEST_LOG_NAME
        {            
            ToolAutoCal = 0,    
            
            
            
        }


        //---------------------------------------------------------------------------
        //Tool
        public enum EN_TOOL_ID
        {
            tiNone = -1,
            tiMainX = 0,  //
        }

        //---------------------------------------------------------------------------
        public enum EN_MAGA_ID
        {
            miNone  = -1,
            POLISH  = 0 , //0
            CLEAN       , //1
            LOAD        , //2
            TRANS       , //3
            MAGA01      , //4
            MAGA02      , //5
                        
            SPINDLE     , //번외....
        }

        public enum EN_MAGA_STAT
        {
            mzsNone = -1,  // 미사용
            mzsEmpty = 0,  // 사용 가능한데 없는 경우
            mzsReady    ,  // 준비된 상태
            mzsFinish   ,  // 전부다 사용한 상태
            mzsWork     ,  // 현재 작업 중
                           //	mzsExist       ,  // 사용 가능
        }
        //---------------------------------------------------------------------------
        public enum EN_DIS_DIR
        {
            ddNone = -1,
            ddTopLeft  , //Normal
            ddTopRight ,
            ddBtmLeft  ,
            ddBtmRight , //Reverse
            ddNormal
        };
        //---------------------------------------------------------------------------
        //Level
        public enum EN_USER_LEVEL
        {
            lvOperator = 0,
            lvEngineer,
            //lvAdmin   , <<- Master
            //lvTechnis,
            lvMaster,
            lvMaker
        };
        //---------------------------------------------------------------------------
        public enum EN_FILE
        {
            fiSave = 0,
            fiLoad = 1,
        }
        //---------------------------------------------------------------------------
        
        public enum EN_ERR_GRADE 
        { //The grade of error.
            egInit = -1,
        	egDisplay,
        	egWarning,
        	egError
        }
        
        
        public enum EN_ERR_KIND 
        {
        	ekMachine ,
        	ekMaterial,
        	ekHuman   ,
        	ekMethod
        }
        //Sequence State
        public enum EN_SEQ_STATE 
        {
        	STOP   ,
        	RUNNING, 
        	ERROR  , 
        	WARNING, 
        	INIT   , 
        	RUNWARN,
            IDLE   , 

            EndofId
        }
        //Lamp Operations.
        
        public enum EN_LAMP_STATE 
        {
        	lsLampOff  ,
        	lsLampOn   ,
        	lsLampFlick
        }
        
        //Buzzer Operations.
        public enum EN_BUZZ_STATE 
        {
        	bsBuzzOff,
        	bsBuzz1  ,
        	bsBuzz2  ,
        	bsBuzz3
        }


        //Motor Index Id
        public enum EN_MOTR_ID
        {
        	miNone    = -1,
        	miSPD_X   =  0,  //00//Main Spindle X Axis
            miPOL_Y       ,  //01//Polishing Y Axis
            miSPD_Z       ,  //02//Main Spindle Z Axis
            miCLN_R       ,  //03//Cleaning R Axis
        	miPOL_TH      ,  //04//Polishing Theta Axis
        	miPOL_TI      ,  //05//Polishing Tilt Axis
            miSTR_Y       ,  //06//Storage Y Axis
            miCLN_Y       ,  //07//Cleaning Y Axis
            miSPD_Z1      ,  //08//Main Plate Z Axis (Pick/Place)
            miTRF_Z       ,  //09//Magazine Z Axis
            miTRF_T       ,  //10//Transfer Pre-Align TH1 Axis
            

            EndOfId
        }
        public enum EN_POS_KIND : int
        {
            NONE,
            NORM,
            COMM,
            VIEW

        }


        //Main-X
        public enum EN_WHERE_SPD
        {
        	wpUnkonw  = -1,
            wpPOLI   = 0  ,  //Spindle - Polishing Position
        	wpCLEN        ,  //Spindle - Cleaning Position
        	wpSTOR        ,  //Spindle - Storage Position
        	wpPTOOLPICK   ,  //Spindle - Polishing Tool pick/Place Position
        	wpCTOOLPICK   ,  //Spindle - Cleaning Tool pick/Place Position
        	wpTOOLOUT     ,  //Spindle - Tool Discard Position
        	wpFORCECHK    ,  //Spindle - Force Check Position
        	wpPLATEPOLI   ,  //Spindle - Polishing Plate Pick, Place Position
        	wpPLATECLEN   ,  //Spindle - Cleaning Plate Pick, Place Position
        	wpPLATEALGN   ,  //Spindle - Plate In/Out Position
            wpPLATELOAD1  ,  //Spindle - Plate Load1 Position
            wpPLATELOAD2  ,  //Spindle - Plate Load2 Position
            wpUTILCHECK_P ,  //Spindle - Utility Check Position - Polishing
            wpUTILCHECK_C ,  //Spindle - Utility Check Position - Cleaning
            wpVISNLOAD    ,  //Spindle - Polishing Vision Position
            wpVISNPOLISH  ,  //Spindle - Polishing Vision Position
            wpALIGNSTRG1  ,  //Spindle - Storage Align Position1
            wpALIGNSTRG2  ,  //Spindle - Storage Align Position2
            wpCUPSTORAGE  ,  //Spindle - Cup Storage Position
            wpCUPPOLISH   ,  //Spindle - Cup Polishing Position
            wpTOOLCHECK   ,  //Spindle - Tool Exist Check Position
        	wpWAIT           //Spindle - Wait Position
            

        }

        //Polishing-Y
        public enum EN_WHERE_POL
        {
        	wpPOL_VISN = 0,  //Polishing - Vision Inspection Position
        	wpPOL_POLI    ,  //Polishing - Polishing Position
        	wpPOL_WAIT       //Polishing - Wait Position
        }

        //Cleaning-Y
        public enum EN_WHERE_CLN
        {
        	wpCLN_CLEN = 0,  //Cleaning - Cleaning Position
        	wpCLN_WAIT       //Cleaning - Wait Position
        }
        
        //Storage-Y
        public enum EN_WHERE_STR
        {
        	wpSTR_WORK = 0,  //Storage - Work Position
        	wpSTR_WAIT       //Storage - Wait Position
        }
        //Loading/Unloading
        public enum EN_WHERE_LOAD
        {
        	wpLUD_PICK   = 0,  //Load/Unload - Load : Plate Pick Position
            wpLUD_PLACE     ,  //Load/Unload - Unload : Plate Place Position
            wpLUD_LOADPOS   ,  //Load/Unload - Load Position
            wpLUD_UNLOADPOS    //Load/Unload - Unload Position
        }


        //---------------------------------------------------------------------------
        //Speed Ration
        public enum EN_SPEED_RATIO
        {
        	sr100 = 0,
        	sr90 = 1,
        	sr80,
        	sr70,
        	sr60,
        	sr50,
        	sr40,
        	sr30,
        	sr20,
        	sr10,
        
        }
        //Motor Type.
        //===========================================================================
        public enum EN_MOTOR_TYPE
        {
            mtServo = 0,
            mtRotary,
            mtLinear,
            mtStep,
            mtInduct,
            mtACS,
            mtEtc
        }
        //Motor Maker
        //===========================================================================
        public enum EN_MOTR_MAKER  : int
        {
            NONE  ,
            ACS   , 
            SMC   , 
            COMI  ,
            AJIN  ,
            AJECAT,
            EZSTEP,
            HABER ,
            ELMO
        }
    
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
        public enum EN_MOTR_KIND  : int
        {
            INC,
            ABS
        }
    
    
        //Motion Command ID.
        //===========================================================================
            //Common.
        public enum EN_COMD_ID  : int 
        {
            NoneCmd    = -1   ,
            User1      = 0    , //User defined position as Motor Characteristics.
            User2      = 1    ,
            User3      = 2    ,
            User4      = 3    ,
            User5      = 4    ,
            User6      = 5    ,
            User7      = 6    ,
            User8      = 7    ,
            User9      = 8    ,
            User10     = 9    ,
            User11     = 10   ,
            User12     = 11   ,
            User13     = 12   ,
            User14     = 13   ,
            User15     = 14   ,
            User16     = 15   ,
            User17     = 16   ,
            User18     = 17   ,
            User19     = 18   ,
            User20     = 19   ,
            User21     = 20   ,
            User22     = 21   ,
            User23     = 22   ,
            User24     = 23   ,
            User25     = 24   ,
            User26     = 25   ,
            User27     = 26   ,
            User28     = 27   ,
            User29     = 28   ,
            User30     = 29   ,
            Stop       = 30   , //Basis command.
            EStop      = 31   ,
            Home       = 32   ,
            JogP       = 33   ,
            JogN       = 34   ,
            Wait1      = 35   ,
            Wait2      = 36   ,
            FindStep1  = 37   , //Step Command.
            FindStepF1 = 38   ,
            FindStepB1 = 39   ,
            OneStepF1  = 40   ,
            OneStepB1  = 41   ,
            FindStep2  = 42   ,
            FindStepF2 = 43   ,
            FindStepB2 = 44   ,
            OneStepF2  = 45   ,
            OneStepB2  = 46   ,
            FindStep3  = 47   ,
            FindStepF3 = 48   ,
            FindStepB3 = 49   ,
            OneStepF3  = 50   ,
            OneStepB3  = 51   ,
            FindStep4  = 52   ,
            FindStepF4 = 53   ,
            FindStepB4 = 54   ,
            OneStepF4  = 55   ,
            OneStepB4  = 56   ,
            UserPitchP = 57   ,
            UserPitchN = 58   ,
            FSP1_1     = 59   , //Step1 - Index1
            FSP1_2     = 60   , //        - fpIndex2
            FSP1_3     = 61   , //        - fpIndex3
            FSP1_4     = 62   , //        - fpIndex4
            FSP1_5     = 63   , //        - fpIndex5
            FSP1_6     = 64   , //        - fpIndex6
            FSP1_7     = 65   , //        - fpIndex7
            FSP1_8     = 66   , //        - fpIndex8
            FSP1_9     = 67   , //        - fpIndex9
            FSP2_1     = 68   , //Step2 - Index1
            FSP2_2     = 69   , //        - fpIndex2
            FSP2_3     = 70   , //        - fpIndex3
            FSP2_4     = 71   , //        - fpIndex4
            FSP2_5     = 72   , //        - fpIndex5
            FSP2_6     = 73   , //        - fpIndex6
            FSP2_7     = 74   , //        - fpIndex7
            FSP2_8     = 75   , //        - fpIndex8
            FSP2_9     = 76   , //        - fpIndex9
            FSP3_1     = 77   , //Step3 - Index1
            FSP3_2     = 78   , //        - fpIndex2
            FSP3_3     = 79   , //        - fpIndex3
            FSP3_4     = 80   , //        - fpIndex4
            FSP3_5     = 81   , //        - fpIndex5
            FSP3_6     = 82   , //        - fpIndex6
            FSP3_7     = 83   , //        - fpIndex7
            FSP3_8     = 84   , //        - fpIndex8
            FSP3_9     = 85   , //        - fpIndex9
            FSP4_1     = 86   , //Step4 - Index1
            FSP4_2     = 87   , //        - fpIndex2
            FSP4_3     = 88   , //        - fpIndex3
            FSP4_4     = 89   , //        - fpIndex4
            FSP4_5     = 90   , //        - fpIndex5
            FSP4_6     = 91   , //        - fpIndex6
            FSP4_7     = 92   , //        - fpIndex7
            FSP4_8     = 93   , //        - fpIndex8
            FSP4_9     = 94   , //        - fpIndex9
            LSP1_1     = 95   ,
            LSP1_2     = 96   ,
            LSP1_3     = 97   ,
            LSP1_4     = 98   ,
            LSP1_5     = 99   ,
            LSP1_6     = 100  ,
            LSP1_7     = 101  ,
            LSP1_8     = 102  ,
            LSP1_9     = 103  ,
    
            LSP2_1     = 104  ,
            LSP2_2     = 105  ,
            LSP2_3     = 106  ,
            LSP2_4     = 107  ,
            LSP2_5     = 108  ,
            LSP2_6     = 109  ,
            LSP2_7     = 110  ,
            LSP2_8     = 111  ,
            LSP2_9     = 112  ,
    
            LSP3_1     = 113  ,
            LSP3_2     = 114  ,
            LSP3_3     = 115  ,
            LSP3_4     = 116  ,
            LSP3_5     = 117  ,
            LSP3_6     = 118  ,
            LSP3_7     = 119  ,
            LSP3_8     = 120  ,
            LSP3_9     = 121  ,
    
            LSP4_1     = 122  ,
            LSP4_2     = 123  ,
            LSP4_3     = 124  ,
            LSP4_4     = 125  ,
            LSP4_5     = 126  ,
            LSP4_6     = 127  ,
            LSP4_7     = 128  ,
            LSP4_8     = 129  ,
            LSP4_9     = 130  ,
    
            Zero       = 131  , //To Zero.
            Direct     = 132  , //To Direct.
            Pick1      = 133  , //Tool Z-Axis.
            Pick2      = 134  ,
            Plce1      = 135  ,
            Plce2      = 136  ,
            PickInc    = 137  ,
            PlceInc    = 138  ,
            Sply       = 139  ,
            Stck       = 140  ,
            CalPos     = 141
    
        }
    
        //Motor Speed ID.
        //===========================================================================
            //Motor Velocity ID.
        public enum  EN_MOTR_VEL  : int 
        {
            Normal = -1,
            Home       ,  //0
            Work       ,  //1
            Dry        ,  //2
            HJog       ,  //3  (5 ~ 9 UserDefined)
            LJog       ,  //4  (5 ~ 9 UserDefined)
            User1      ,
            User2      ,
            User3      ,
            User4      ,
            User5
    
        }
/*    
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
            UserPitch = 72   , //User SET PITCH
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
*/
        public enum EN_MOTR_DELAY : int
        {
            TimeOut, //0                                                                        
            Stop     //1  (6 ~ 9 UserDefined)
        }


        //Direction.
        //===========================================================================
        public enum  EN_SERVO_DIR  : int 
        {
            CW  ,
            CCW
        }

        //Motor FindStep Type
        //===========================================================================
        public enum EN_FSTEP_INDEX : int
        {
            NONE = -100,
            StepA = 0,
            Step1 = 1, //FSP1_x
            Step2 = 2, //FSP2_x
            Step3 = 3, //FSP3_x
            Step4 = 4  //FSP4_x
        }

        public enum  EN_FPOSN_INDEX  : int 
        {
            NONE   = -100 ,
            Index1 =    0 , //FSPx_1
            Index2        , //FSPx_2
            Index3        , //FSPx_3
            Index4        , //FSPx_4
            Index5        , //FSPx_5
            Index6        , //FSPx_6
            Index7        , //FSPx_7
            Index8        , //FSPx_8
            Index9          //FSPx_9
        }
        //Motor Speed Mode
        //===========================================================================
        public enum EN_SPED_MODE : int
        {
            Work,
            Dry
        }
        
        //MOTOR SPEED RATIO ID.
        //===========================================================================
        public enum EN_SPED_RATIO : int
        {
            sr100,
            sr90,
            sr80,
            sr70,
            sr60,
            sr50,
            sr40,
            sr30,
            sr20,
            sr10
        }
        public enum  EN_SEQ_ID  : int 
        {
           None = -1, // All.
           SPINDLE ,
           POLISH  , //  
		   CLEAN   ,
           STORAGE ,
           TRANSFER, // 
           SYSTEM  , // SYSTEM
           ALL     
        }

        public enum EN_UTIL_STATE
        {
            Unknown,
            Empty,
            Exist
        }
        public enum EN_UTIL_KIND
        {
            none       = -1,
            Silica01   =  0,
            Silica02     ,
            Silica03     ,
            Soap         ,
            DIWater      ,
            SilicaDI     ,
            SoapDI       ,
            ALL          ,
                       
            EndofId
        }
        //AUTO SUPPLY MAP.
        //===========================================================================
        public enum EN_ATSUPPLY_UNINT
        {
            NONE        = -1,
            POLISHING       ,
            CLEANING        ,
            MAX_UNIT
        }
        public enum EN_ATSUPPLY_MEMORY
        {
            en_NONE = -1,
            en_E0Area = 0,
            en_E2Area
        }
        public enum EN_ATSUPPLY_SEND_AREA : int
        {
            _000_POU_SLURRY_REQUEST = 0,
            _001_POU_CLEANING_REQUEST,
            _002_POU_COMMUNICATION_SIGNAL,
            _003_POU_EQUIP_ID,
            _004_SUPPLY_SLURRY_ID
        }
        public enum EN_ATSUPPLY_READ_AREA : int
        {
            _000_POU_SLURRY_REQUEST = 0,
            _001_POU_CLEANING_REQUEST,
            _002_POU_COMMUNICATION_SIGNAL,
            _003_POU_EQUIP_ID,
            _004_SUPPLY_SLURRY_ID,
            _014_EMPTY,
            _015_EMPTY,
            _016_AUTO,
            _017_SOFT_MANUAL,
            _018_HARD_MANUAL,
            _019_RUN,
            _020_STOP,
            _021_EMPTY,
            _022_EMPTY,
            _023_EMPTY,
            _024_EMPTY,
            _025_EMPTY,
            _026_EMPTY,
            _027_EMPTY,
            _028_EMPTY,
            _029_EMPTY,
            _030_EMPTY,
            _031_EMPTY,
            _032_Red,
            _033_Yellow,
            _034_Green,
            _035_Buzzer,
            _036_EMPTY,
            _037_EMPTY,
            _038_EMPTY,
            _039_EMPTY,
            _040_EMPTY,
            _041_EMPTY,
            _042_EMPTY,
            _043_EMPTY,
            _044_EMPTY,
            _045_EMPTY,
            _046_EMPTY,
            _047_EMPTY,
            _048_PUMP_A_LEAK,
            _049_UNIT_LEAK_DOWN,
            _050_EMERGENCY_STOP,
            _051_AUTO_STOP,
            _052_SLURRY_NOT_READY,
            _053_SLURRY_SUPPLY_FLUX_LOW_DOWN,
            _054_SUPPLY_TANK_LEVEL_EMPTY_DOWN,
            _055_RETURN_PRESSURE_LOW_DOWN,
            _056_UNIT_LEAK__ROOM_BOTTOM,
            _057_UNIT_LEAK__TANK_ROOM,
            _058_UNIT_LEAK__VALVE_BOX,
            _059_UNIT_LEAK__LEAK_BOX_,
            _060_SLURRY_SUPPLY_TIME_OVER,
            _061_POU_LINE_CLEAN_TIME_OVER,
            _062_EMPTY,
            _063_EMPTY,
            _064_OVER_USING,
            _065_SUP_TK_A_LOAD_CELL_ALARM,
            _066_SUP_TK_A_ERROR,
            _067_SUP_TK_A_CHARGE_TIMEOUT,
            _068_SUP_TK_A_LEVEL_HH,
            _069_SUP_TK_A_LEVEL_L_OFF,
            _070_SUP_TK_A_LEVEL_EMPTY,
            _071_SUPPLY__FLUX_HIGH,
            _072_SUPPLY__FLUX_LOW,
            _073_RETURN__FLUX_HIGH,
            _074_RETURN__FLUX_LOW,
            _075_RETURN__PRESSURE_HIGH,
            _076_RETURN__PRESSURE_LOW,
            _077_PUMP__1E_RPM_HIGH,
            _078_PUMP__1E_RPM_LOW,
            _079_PUMP__1E_POWER_CEMPTYP_TRIP,
            _080_PUMP__1E_ERROR,
            _081_PUMP_A_STROKE_HIGH,
            _082_PUMP_A_STROKE_LOW,
            _083_POWER_SUPPLY__1_FAIL,
            _084_POWER_SUPPLY__2_FAIL,
            _085_POWER_SUPPLY__3_FAIL,
            _086_POWER_SUPPLY__4_FAIL,
            _087_PLC_BATTERY_LOW,
            _088_PLC_MEMORY_ERROR,
            _089_SUP_TK_A_WATER_SEALING_OVERFLOW,
            _090_PUMP_CONTROLLER_ROOM_COOLING_FAN_FAIL,
            _091_ELEC_ROOM_COOLING_FAN_FAIL,
            _092_SUP_TK_A_LEVEL_SENSOR_ERROR,
            _093_EMPTY,
            _094_EMPTY,
            _095_EMPTY,
            _096_EMPTY,
            _097_EXHAUST_PRESSURE_LOW,
            _098_OPTION_CHANGE,
            _099_PARTS_LIFE_TIME_OVER,
            _100_DOOR_OPEN__ELEC_ROOM,
            _101_DOOR_OPEN__REGULATOR_ROOM,
            _102_DOOR_OPEN__PUMP_CONTROLLER_ROOM,
            _103_DOOR_OPEN__SUP_TK_A_ROOM,
            _104_DOOR_OPEN__SOL_ROOM,
            _105_PUMP_1E_WANRING,
            _106_BOTTLE_A_EMPTY,
            _107_DOOR_OPEN__N2_ROOM,
            _108_DOOR_OPEN__BOTTLE_ROOM,
            _109_DOOR_OPEN__SAMPLING_ROOM,
            _110_DOOR_OPEN__DRAIN_UT_ROOM,
            _111_DOOR_OPEN__SAMPLE_VALVE_ROOM,
            _112_SUP_TK_A_CHARGE_REQUEST,
            _113_PUMP_1E_COMMUNICATION_ERROR,
            _114_EMPTY,
            _115_EMPTY,
            _116_EMPTY,
            _117_EMPTY,
            _118_EMPTY,
            _119_EMPTY,
            _120_EMPTY,
            _121_EMPTY,
            _122_EMPTY,
            _123_EMPTY,
            _124_EMPTY,
            _125_EMPTY,
            _126_EMPTY,
            _127_EMPTY,
            _128_SYSTEM_READY,
            _129_SYSTEM_NOT_READY,
            _130_SYSTEM_SUPPLY,
            _131_BOTTLE_A_FLOW_SENSOR,
            _132_AGITATOR__SUP_TK_A__WATER_SEALING_OVER_FLOW_SENSOR,
            _133_UNIT_LEAK__VALVE_BOX,
            _134_UNIT_LEAK__BOTTOM,
            _135_UNIT_LEAK__TANK_ROOM,
            _136_UNIT_LEAK__LEAK_BOX,
            _137_SUP_TK_A_LEVEL_LL,
            _138_SUP_TK_A_LEVEL_L,
            _139_SUP_TK_A_LEVEL_M,
            _140_SUP_TK_A_LEVEL_H,
            _141_SUP_TK_A_LEVEL_HH,
            _142_SUP_TK_A_LEVEL_HHH,
            _143_BOTTLE_A_READY,
            _144_BOTTLE_A_FULL,
            _145_EMPTY,
            _146_EMPTY,
            _147_EMPTY,
            _148_EMPTY,
            _149_EMPTY,
            _150_EMPTY,
            _151_EMPTY,
            _152_EMPTY,
            _153_EMPTY,
            _154_EMPTY,
            _155_EMPTY,
            _156_EMPTY,
            _157_EMPTY,
            _158_EMPTY,
            _159_EMPTY,
            _160_PUMP_A,
            _161_PUMP_1E,
            _162_AGITATOR_RUN__SUP_TK_A,
            _163_SUPPLY_PUMP_SPEED_MODE,
            _164_SUPPLY_PUMP_PROCESS_MODE
        }
        // PMC Port ID
        public enum EN_PORT_ID : int
        {
            LoadPort = 0,
            UnloadPort,
            PolishingBath,
            CleaningBath
        }
        public enum EN_PROCESS_STATE : int
        {
            en_Loading = 0,
            en_Preparing,
            en_ProcessingReady,
            en_Processing,
            en_ProcessDone,
            en_UnloadReady,
            en_Unloading,
            en_Unloaded
        }


        #endregion

    }


    /************************************************************************/
    /* STRUCTURE                                                            */
    /************************************************************************/
    //---------------------------------------------------------------------------
    public struct ST_MILL
    {
        public int    nTotalCycle;

        public int   [] nPath      ;
        public int   [] nRPM       ;
        public double[] dForce     ;
        public double[] dXSpeed    ;
        public double[] dXDistance ;
        public double[] dYDistance ;

        public ST_MILL(int n)
        {
            nTotalCycle = n;

            nPath       = new int   [10];
            nRPM        = new int   [10];
            dForce      = new double[10];
            dXSpeed     = new double[10];
            dXDistance  = new double[10];
            dYDistance  = new double[10];
            
            for (int i = 0; i < 10; i++)
            {
                nPath     [i] = new int   ();
                nRPM      [i] = new int   ();
                dForce    [i] = new double();
                dXSpeed   [i] = new double();
                dXDistance[i] = new double();
                dYDistance[i] = new double();

                nPath     [i] = 0  ;
                nRPM      [i] = 0  ;
                dForce    [i] = 0.0;
                dXSpeed   [i] = 0.0;
                dXDistance[i] = 0.0;
                dYDistance[i] = 0.0;
            }
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public struct ST_PIN_POS
    {
        public double dXPos;
        public double dYPos;

        public ST_PIN_POS(double a)
        {
            dXPos = a;
            dYPos = a;

        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public struct ST_LOG_INFO
    {
        public EN_LOG_TYPE eType;
        public string      sMsg ;
        public string      sdt  ;
        public EN_SEQ_ID   iPart;
        public string      sFileName ;

        public ST_LOG_INFO(string set)
        {
            eType     = EN_LOG_TYPE.ltTrace;
            sMsg      = set;
            sdt       = set;
            iPart     = EN_SEQ_ID.None;
            sFileName = string.Empty; 
        }
    }

    //---------------------------------------------------------------------------
    public struct ST_USERSET
    {
        //Access Set
        public bool bMotion ;
        public bool bSetting;
        public bool bRecipe ;
        public bool bLog    ;
        public bool bMaster ;
        public bool bExit   ;

        public ST_USERSET(bool set)
        {
            bMotion  = set;
            bSetting = set;
            bRecipe  = set;
            bLog     = set;
            bMaster  = set;
            bExit    = set;
        }

    };

//---------------------------------------------------------------------------
    public struct ST_PASS_WORD
    {

        public string strEngr ;
        public string strTech ;
        public string strOper ;
        public string strMstr ;
        public string strMake ;
        public string strAdmin;

        public ST_PASS_WORD(string m)
        {
            strEngr  = m;
            strTech  = m;
            strOper  = m;
            strMstr  = m;
            strMake  = m;
            strAdmin = m;
        }

    }
    //---------------------------------------------------------------------------
    public struct ST_MASTER_OPTION
    {
        //
        public string   sLoadCellSN     ; //Load Cell Serial No.
        public int      nOffsetDefault  ;
        public double   dTareValue      ;
        public double   dFullScaleLoaded;
        public double   dUtilOffset     ;
        public double   dPickOffset     ;
        public double   dPlaceOffset    ;
        public double   dYIntercept     ;
        public double   dYSlope         ;
        public double   dforceOffset    ;
        public double   dYInterceptBT   ;
        public double   dYSlopeBT       ;
        public double   dSpdOffset      ;
        public double   dTopLDCellOffset;
        public double   dStartDCOM      ;

        public double[] dHomeOffset     ;
        public double[] dMinPos         ;
        public double[] dMaxPos         ;
        public double[] dMinVel         ;
        public double[] dMaxVel         ;
        public double[] dMinAcc         ;
        public double[] dMaxAcc         ;

        

        //
        public int     nUseSkipDoor     ;
        public int     nUseSkipLeak     ;
        public int     nUseSkipGrid     ;
        public int     nUseSkipFan      ;
        public int     nUseSkipAir      ;
        public int     nUseSkipWaterLeak;
        public int     nUseSkipWaterLvl ;
        public int     nUseSkipAccura   ;
        public int     nUseSkipDP       ;

        public EN_RUN_MODE nRunMode  ; //AUTO, MAN, TEST
        public int     nRunSpeed        ;
                       
        public int     nDrainTime       ;
        public int     nSepBlowTime     ;
        public int     nSuckBackTime    ;
        public int     nUtilMaxTime     ;
                       
        public int     nMagaSkip        ;
        public int     nPlateSkip       ;
        public int     nToolSkip        ;
        public int     nStorageSkip     ;
        public int     nVisionSkip      ;
        public int     nForceSkip       ;
        public int     nUseVision       ;
        public int     nUseDirPos       ; // 1: Vision Align Position
        public int     nUseCalForce     ; // Use Default Force Calibration method
        public int     nUsePMC          ;
        //public int     nUseAutoSlury    ;
        public int     nUseDCOMReset    ;
        public int     nUseRESTApi      ;
        public int     nUseDI           ; //use DI for polishing instead of Slurry 
        public double  dDCOMRatio       ;
        public int     nDCOMCnt         ;
        public double  dLDCBtmOffset    ; //Bottom Load cell Offset (%)
        public int     nEPDOnlyMeasure  ; // EPD Only Measure (EPD Always return false)
        public int     nUseMOC          ; //Model 별 광학 조건
        public int     nUseCleanPos     ; //Polishing Vision Calibration Position 사용

        public bool[] bAutoOff;

        public ST_MASTER_OPTION(int n)
        {
            //
            sLoadCellSN       = string.Empty;
            nOffsetDefault    = 0;
            dTareValue        = 0.0;
            dFullScaleLoaded  = 0.0;
            dUtilOffset       = 0.0;
            dPickOffset       = 0.0;
            dPlaceOffset      = 0.0;
            dYIntercept       = 0.0;
            dYSlope           = 0.0;
            dforceOffset      = 0.0;
            dYInterceptBT     = 0.0;
            dYSlopeBT         = 0.0;
            dSpdOffset        = 0.0;
            dTopLDCellOffset  = 0.0;
            dStartDCOM        = 2.5;

            //
            nUseSkipDoor      = n;
            nUseSkipLeak      = n;
            nUseSkipGrid      = n;
            nUseSkipWaterLeak = n;
            nUseSkipFan       = n;
            nUseSkipAir       = n;
            nUseSkipWaterLvl  = n;
            nUseSkipAccura    = n;
            nUseSkipDP        = 0;


            nRunMode          = EN_RUN_MODE.AUTO_MODE;
            nRunSpeed         = n;

            nPlateSkip        = n;
            nToolSkip         = n;
            nVisionSkip       = n;
            nForceSkip        = n;
            nStorageSkip      = n;
            nMagaSkip         = n;

            nDrainTime        = 1;
            nSepBlowTime      = 1;
            nSuckBackTime     = 1;
            nUtilMaxTime      = 5;

            nUseVision        = 0;
            nUseDirPos        = 0;
            nUseCalForce      = 0;
            nUsePMC           = 0;
            //nUseAutoSlury     = 0;
            nUseRESTApi       = 0;
            nUseDI            = 0;
            nEPDOnlyMeasure   = 0;
            nUseDCOMReset     = 0;
            nDCOMCnt          = 0;
            nUseMOC           = 0;
            nUseCleanPos      = 0 ;
            dDCOMRatio        = 0.0;

            dLDCBtmOffset     = 0.0;

            bAutoOff = new bool[MAX_SEQ_PART];
            for (int i = 0; i < MAX_SEQ_PART; i++)
            {
                bAutoOff[i] = new bool();
            }

            dHomeOffset = new double[MAX_MOTOR];
            dMinPos     = new double[MAX_MOTOR];
            dMaxPos     = new double[MAX_MOTOR];
            dMinVel     = new double[MAX_MOTOR];
            dMaxVel     = new double[MAX_MOTOR];
            dMinAcc     = new double[MAX_MOTOR];
            dMaxAcc     = new double[MAX_MOTOR];

            for (int i = 0; i < MAX_MOTOR; i++)
            {
                dHomeOffset[i] = new double();
                dHomeOffset[i] = 0.0;

                dMinPos[i] = new double();
                dMinPos[i] = 0.0;
                dMaxPos[i] = new double();
                dMaxPos[i] = 0.0;
                dMinVel[i] = new double();
                dMinVel[i] = 0.0;
                dMaxVel[i] = new double();
                dMaxVel[i] = 0.0;
                dMinAcc[i] = new double();
                dMinAcc[i] = 0.0;
                dMaxAcc[i] = new double();
                dMaxAcc[i] = 0.0;
            }
        }

    }
    //---------------------------------------------------------------------------
    public struct ST_SYSTEM_OPTION
    {
        public string strMachineName;
        public string strMachineNo  ;
        public string strLogPath    ;
        
        public string sSupplyIp     ;
        public int    nSupplyPort   ;
        public string sSupplyEqpId  ;
        public int    nSupplyAddress;

        public string sSupplyIp1     ;
        public int    nSupplyPort1   ;
        public string sSupplyEqpId1  ;
        public int    nSupplyAddress1;

        public string sRestApiUrl    ;


        public int    nRunMode      ;//Auto,Manual,Offline(GEM:Remote, Local, Offline)
        public int    nUseWarming   ;
        public int    nWarmInterval ;
        public int    nWarmInterval1;
        public int    nMotrRepeat   ;
        public int    nClampRepeat  ;
        public int    nUtilRepeat   ;
        public int    nWaterLvlPol  ;
        public int    nStorDir      ;
        public int    nSpindleOffCnt;
        public int    nSplyTime     ;


        // Milling 중 Motion Moving Check Time Out
        public int    nPoliMillingTime ;
        public int    nCleanMillingTime;

        //Option
        public int    nUseLightOnRun    ;
        public int    nUseAllForceChk   ;
        public int    nUseCleanAirBlow  ;
        public int    nUsePolishingCup  ;
        public int    nUseAutoSlurry    ;
        public int    nUseSoftLimit     ;
        public int    nUseSpdDirBwd     ; // Use Spindle 1st Direction - BWD
        public int    nUseSpdDirOnlyFWD ; // Use Spindle Direction - FWD
        public int    nUseSpdDirOnlyBWD ; // Use Spindle Direction - BWD
        public int    nUsePoliOneDir    ; // Use Polishing Only One Direction
        public int    nUseSkipPolAlign  ; // Use Skip Polishing Align 
        public int    nUseSkipVisError  ; // Use Skip Vision Error //LEE/200929

        public SolidColorBrush[] brPinColor;

        //PMC Address
        public string sPMCIp           ;
        public int    nPMCPort         ;

        //RFID Address
        public string sRFIDIp          ;

        //
        public double dSoftLimitOffset ;
        public double dTargetForce     ;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = UserConst.MAX_MOTOR)]
        public bool[] bUseMotor  ;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public bool[] bUseClamp  ;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bUseUtil   ;

        public ST_USERSET[] stUserSet ;

        public string[] sToolType;

        public ST_SYSTEM_OPTION(int m)
        {
            strMachineName      = string.Empty;
            strMachineNo        = string.Empty;
            strLogPath          = string.Empty;
            sSupplyIp           = string.Empty;
            nSupplyPort         = 0;
            sSupplyEqpId        = string.Empty;
            nSupplyAddress      = -1;

            sSupplyIp1          = string.Empty;
            nSupplyPort1        = 0;
            sSupplyEqpId1       = string.Empty;
            nSupplyAddress1     = -1;

            sRestApiUrl         = string.Empty;

            nRunMode            = m;
            nUseWarming         = m;
            nWarmInterval       = m;
            nWarmInterval1      = m;
            nMotrRepeat         = m;
            nClampRepeat        = m;
            nUtilRepeat         = m;
            nWaterLvlPol        = m;

            nPoliMillingTime    = m;
            nCleanMillingTime   = m;
            
            nUseLightOnRun      = m;
            nStorDir            = m;
            nUseAllForceChk     = m;
            
            nUseCleanAirBlow    = m;
            nUsePolishingCup    = m;
            nUseAutoSlurry      = m;
            nUseSoftLimit       = m;
            nUseSpdDirBwd       = 0;
            nUseSpdDirOnlyFWD   = 0;
            nUseSpdDirOnlyBWD   = 0;
            nUsePoliOneDir      = 0; 
            nUseSkipPolAlign    = 0;
            nUseSkipVisError    = 0;

            nSpindleOffCnt      = 0;
            nSplyTime           = 0;

            dSoftLimitOffset    = 0.0;
            dTargetForce        = 0.0;

            bUseMotor = new bool[MAX_MOTOR];
            for (int i = 0; i< MAX_MOTOR; i++)
            {
                bUseMotor[i] = new bool();
            }

            bUseClamp = new bool[3];
            for (int i = 0; i < 3; i++)
            {
                bUseClamp[i] = new bool();
            }

            bUseUtil = new bool[2];
            for (int i = 0; i < 2; i++)
            {
                bUseUtil[i] = new bool();
            }

            stUserSet = new ST_USERSET[5];
            for (int i = 0; i < 5; i++)
            {
                stUserSet[i] = new ST_USERSET(false);
            }

            //
            sPMCIp   = string.Empty;
            nPMCPort = 0;

            sRFIDIp = string.Empty;

            //User Tool Type
            sToolType = new string[MAX_TOOLTYPE];
            for (int i = 0; i < MAX_TOOLTYPE; i++)
            {
                sToolType[i] = string.Empty; 
            }

            brPinColor = new SolidColorBrush[5];
            for (int i = 0; i < 5; i++)
            {
                if      (i == 0) brPinColor[i] = Brushes.CadetBlue     ;
                else if (i == 1) brPinColor[i] = Brushes.LightSteelBlue;
                else             brPinColor[i] = Brushes.White; 
            }

        }

    }


    //---------------------------------------------------------------------------
    public struct ST_SYSTEM_BASE
    {
        public int nRunMode ;
        public int nRunSpeed;

        public ST_SYSTEM_BASE(int n)
        {
            nRunMode  = n;
            nRunSpeed = n;
        }

    }

    //---------------------------------------------------------------------------
    public struct ST_ITEM_POLISHING
    {
        public int nCycleCount;
        public int nPathCount ;
        public int nForce     ;
        public int nSpindlRPM ;
        public int nCCW       ; //191031/JUNG/Spindle 방향

        public bool bUseNewTool;
        public bool bUseNewUtil; //Utility 사용 유무

        public double dSpeedX   ;
        public double dDisX1    ;
        public double dDisY1    ;
        public double dDisX2    ;
        public double dDisY2    ;
        public double dTiltAngle;
        public double dOffsetX  ;
        public double dOffsetY  ;

        public int nUtility     ;  //사용하는 Utility 종류 - 현재 4종류

        public ST_ITEM_POLISHING(int n)
        {
            nCycleCount = n;
            nPathCount  = n;
            nForce      = n;
            nSpindlRPM  = n;
            nUtility    = n;
            nCCW        = n;

            bUseNewTool = false;
            bUseNewUtil = false;

            dSpeedX    = 0.0;
            dDisX1     = 0.0;
            dDisY1     = 0.0;
            dDisX2     = 0.0;
            dDisY2     = 0.0;
            dTiltAngle = 0.0;
            dOffsetX   = 0.0;
            dOffsetY   = 0.0;

        }

    };
    public struct ST_ITEM_CLEANING
    {
        public int nCycleCount;
        public int nPathCount;
        public int nForce;
        public int nSpindlRPM;

        public int nWaterSplyTime; //Cleaning 전 Water 공급 시간
        public int nWaterSplyRPM; //Water 공급 시 RPM
        public int nDeWateringRPM; //탈수 RPM
        public int nDeWateringTime; //탈수 시간 

        public bool bUseNewTool;

        public double dSpeedX;
        public double dDisX1;
        public double dDisY1;
        public double dDisX2;
        public double dDisY2;
        public double dOffsetX;
        public double dOffsetY;

        public ST_ITEM_CLEANING(int n)
        {
            nCycleCount     = n;
            nForce          = n;
            nPathCount      = n;
            nSpindlRPM      = n;

            nWaterSplyTime  = n;
            nWaterSplyRPM   = n;
            nDeWateringRPM  = n;
            nDeWateringTime = n;

            bUseNewTool     = false;

            dSpeedX  = 0.0;
            dDisX1   = 0.0;
            dDisY1   = 0.0;
            dDisX2   = 0.0;
            dDisY2   = 0.0;
            dOffsetX = 0.0;
            dOffsetY = 0.0;
        }

    }

    public struct ST_ITEM_VISION
    {
        public double dMatchRate;
        public double dIntensity;
        public string sPatternPath;

        public ST_ITEM_VISION(double n)
        {
            dMatchRate = n;
            dIntensity = n;
            sPatternPath = string.Empty ;
        }

    }


    //---------------------------------------------------------------------------
    //Recipe Structure
    public struct ST_RECIPE
    {
        public string sRecipeName;
        public int    nWaferLoadOffset; //Wafer 가공 시 Offset Degree
        public int    nStepCountPoli;
        public int    nStepCountClen;
        public int    nStepCountVisn;

        public ST_ITEM_POLISHING[] stPOLI;
        public ST_ITEM_CLEANING [] stCLEN;
        public ST_ITEM_VISION   [] stVISN;

        public ST_RECIPE(int n = 0)
        {
            sRecipeName      = "";
            nWaferLoadOffset = n;
            nStepCountPoli   = n;
            nStepCountClen   = n;
            nStepCountVisn   = n;

            stPOLI = new ST_ITEM_POLISHING[MAX_POLI];
            stCLEN = new ST_ITEM_CLEANING [MAX_CLEN];
            stVISN = new ST_ITEM_VISION   [MAX_VISN];

            for (int i = 0; i<stPOLI.Length; i++)
            {
                stPOLI[i] = new ST_ITEM_POLISHING(0);
            }
            for (int i = 0; i < stCLEN.Length; i++)
            {
                stCLEN[i] = new ST_ITEM_CLEANING(0);
            }
            for (int i = 0; i < stVISN.Length; i++)
            {
                stVISN[i] = new ST_ITEM_VISION(0.0);
            }
        }

    }
    //---------------------------------------------------------------------------
    public struct ST_RECIPE_LIST
    {
        public int    nUtilType    ; //Util Type
        public int    nUseMilling  ; //Use Milling
        public int    nToolType    ; //Tool Type
        public int    nUseToolChg  ; //Use Tool Change
        public int    nUseUtilFill ; //Use Utility
        public int    nUseUtilDrain; //Use Drain
        public int    nUseImage    ; //Use Image
        public int    nUseEPD      ; //Use EPD

        public double dTilt        ;
        public double dTheta       ;
        public double dRPM         ;
        public double dForce       ;
        public double dSpeed       ;
        public double dPitch       ;
        public double dTiltOffset  ; //Tilt 보상

        public Point pStartPos;
        public Point pEndPos  ;

        public double dStartX ;
        public double dEndX   ;
        
        public double dStartY ;
        public double dEndY   ;
        public double dPosTH  ;
        public int    nPathCnt;
        public int    nCycle  ;

        public double dXDistance;
        public double dYDistance;

        public ST_RECIPE_LIST(int n)
        {
            nUtilType     = n;
            nUseMilling   = n;
            nToolType     = n;
            nUseToolChg   = n;
            nUseUtilFill  = n;
            nUseUtilDrain = n;
            nUseImage     = n;
            nUseEPD       = n;

            nPathCnt      = 0;
            nCycle        = 0;
                          
            dTilt         = 0.0; 
            dTheta        = 0.0;
            dRPM          = 0.0;
            dForce        = 0.0;
            dSpeed        = 0.0;
            dPitch        = 0.0;
            dTiltOffset   = 0.0;

            pStartPos = new Point(0, 0);
            pEndPos   = new Point(0, 0);

            dStartX = 0.0;
            dEndX   = 0.0;

            dStartY = 0.0;
            dEndY   = 0.0;
            dPosTH  = 0.0;

            dXDistance = 0.0; 
            dYDistance = 0.0;
        }

    }    
    public struct ST_VISION_RESULT
    {
        public bool   bResult   ; 
        public int    nTotalStep;
        public Point  pntModel  ;
        public double dTheta    ;
        public double dScore    ;

        public ST_RECIPE_LIST[] stRecipeList;

        public ST_VISION_RESULT(int n)
        {
            bResult    = false; 
            nTotalStep = n;
            pntModel   = new Point();
            dTheta     = 0.0;
            dScore     = 0.0;
            stRecipeList = new ST_RECIPE_LIST[10]; 

            for (int i =0; i<10; i++)
            {
                stRecipeList[i] = new ST_RECIPE_LIST(0);
            }
        }
    }

    public struct ST_CLEAN_RECIPE
    {
        public bool   bResult   ; 
        public int    nTotalStep;

        public ST_RECIPE_LIST[] stRecipeList;

        public ST_CLEAN_RECIPE(int n)
        {
            bResult    = false; 
            nTotalStep = n;
            stRecipeList = new ST_RECIPE_LIST[10]; 

            for (int i =0; i<10; i++)
            {
                stRecipeList[i] = new ST_RECIPE_LIST(0);
            }
        }
    }
    //---------------------------------------------------------------------------
    public struct ST_PROJECT_BASE
    {

        //Name
        public string sJobName      ;
                                    
        //Count                     
        public int    nStorage_Row  ;
        public int    nStorage_Col  ;
        public int    nMagazine_Row ;
        public int    nMagazine_Col ;

        //Pitch
        public double dStorPitch_Row;
        public double dStorPitch_Col;
        public double dMagaPitch_Row;
        public double dMagaPitch_Col;

        //Offset
        public double dPreAlignOffset_X;
        public double dPreAlignOffset_TH;

        public double dPolishOffset_X;
        public double dPolishOffset_Y;
        public double dPolishOffset_TH;
        public double dPolishOffset_TI;

        public double dCleanOffset_R;

        public double dStorOffset_Row;
        public double dStorOffset_Col;

        public int    nStotraDir     ;


        //Motor Data
        public ST_PROJECT_BASE(int n)
        {
            sJobName = "";

            nStorage_Col       = n  ;
            nStorage_Row       = n  ;
            nMagazine_Row      = n  ;
            nMagazine_Col      = n  ;
            nStotraDir         = n  ;

            dStorPitch_Row     = 0.0;
            dStorPitch_Col     = 0.0;
            dMagaPitch_Row     = 0.0;
            dMagaPitch_Col     = 0.0;

            dPreAlignOffset_X  = 0.0;
            dPreAlignOffset_TH = 0.0;
            dPolishOffset_X    = 0.0;
            dPolishOffset_Y    = 0.0;
            dPolishOffset_TH   = 0.0;
            dPolishOffset_TI   = 0.0;
            dCleanOffset_R     = 0.0;
            dStorOffset_Row    = 0.0;
            dStorOffset_Col    = 0.0;
        }
    }
    //---------------------------------------------------------------------------
    //Lamp & Buzzer Informations.
    public struct LAMP_INFO
    {
        public int nInitRed , nWarningRed , nErrorRed , nRunRed , nStopRed , nMaintRed , nRunWarnRed ;
        public int nInitYel , nWarningYel , nErrorYel , nRunYel , nStopYel , nMaintYel , nRunWarnYel ;
        public int nInitGrn , nWarningGrn , nErrorGrn , nRunGrn , nStopGrn , nMaintGrn , nRunWarnGrn ;
        public int nInitBuzz, nWarningBuzz, nErrorBuzz, nRunBuzz, nStopBuzz, nMaintBuzz, nRunWarnBuzz;

        public LAMP_INFO(int n)
        {
            nInitRed    = n;
            nWarningRed = n;
            nErrorRed   = n;
            nRunRed     = n;
            nStopRed    = n;
            nMaintRed   = n;
            nRunWarnRed = n;

            nInitYel    = n;
            nWarningYel = n;
            nErrorYel   = n;
            nRunYel     = n;
            nStopYel    = n;
            nMaintYel   = n;
            nRunWarnYel = n;

            nInitGrn    = n;
            nWarningGrn = n;
            nErrorGrn   = n;
            nRunGrn     = n;
            nStopGrn    = n;
            nMaintGrn   = n;
            nRunWarnGrn = n;

            nInitBuzz    = n;
            nWarningBuzz = n;
            nErrorBuzz   = n;
            nRunBuzz     = n;
            nStopBuzz    = n;
            nMaintBuzz   = n;
            nRunWarnBuzz = n;
        }
    }

    //---------------------------------------------------------------------------
    public struct POSN_DATA
    {
    
    	public string sPartName ;

        public int    nPart     ;
        public int    nPosnCnt  ;

        public double dMinVal   ;
        public double dMaxVal   ;


        public POSITION[] POSN ;

        public POSN_DATA(double val)
    	{
            POSN = new POSITION[MAX_POSN];
            for (int n = 0; n < MAX_POSN; n++)
            {
                POSN[n] = new POSITION(0);
            }

            sPartName  = "";
    		nPart      = -1;        
    		nPosnCnt   = 0 ;
    		
    		dMinVal    = val;    
    		dMaxVal    = val;    
    		
    	}
    
    }
    //---------------------------------------------------------------------------
    public struct POSITION
    {
        public string sPosnName  ;
        public string sUnit      ;
        public int    nPosnId    ;
        public int    nManNo     ;

        public POSITION(int n)
    	{
    		sPosnName  = "";
    		sUnit      = "mm";
    		nPosnId    = n ;
    		nManNo     = n ;
    	}
    }

    
    //Master Motor Option
    //===========================================================================
    public struct MSTR_MOTR_OPTION 
    {

        public int   [] nMotrType;
    	public int   [] nMotrKind;
        public int   [] nSpdRatio; //Speed Ratio
        public int   [] nNotUse  ;
        public int   [] nCWLevel ;
        public int   [] nCCWLevel;

        public double[] dMinPosn ;
        public double[] dMaxPosn ;
        public double[] dMinVel  ;
        public double[] dMaxVel  ;
        public double[] dMinAcc  ;
        public double[] dMinDec  ;
    
    
    	//double dDP_SPDLZ_MOVEX_CHECKPOS; //Spindle X-Axis 이동 시 Check Z Position
    
    
    	public MSTR_MOTR_OPTION(int data)
    	{
            nMotrType = new int   [MAX_MOTOR];
            nMotrKind = new int   [MAX_MOTOR];
            nSpdRatio = new int   [MAX_MOTOR];
            nNotUse   = new int   [MAX_MOTOR];
            nCWLevel  = new int   [MAX_MOTOR];
            nCCWLevel = new int   [MAX_MOTOR];

            dMinPosn  = new double[MAX_MOTOR];
            dMaxPosn  = new double[MAX_MOTOR];
            dMinVel   = new double[MAX_MOTOR];
            dMaxVel   = new double[MAX_MOTOR];
            dMinAcc   = new double[MAX_MOTOR];
            dMinDec   = new double[MAX_MOTOR];

    		//Default Init
    		for (int Axis = 0; Axis < MAX_MOTOR; Axis++)
    		{
                nMotrType[Axis] = new int   ();
                nMotrKind[Axis] = new int   ();
                nSpdRatio[Axis] = new int   ();
                nNotUse  [Axis] = new int   ();
                nCWLevel [Axis] = new int   ();
                nCCWLevel[Axis] = new int   ();

                dMinPosn [Axis] = new double();
                dMaxPosn [Axis] = new double();
                dMinVel  [Axis] = new double();
                dMaxVel  [Axis] = new double();
                dMinAcc  [Axis] = new double();
                dMinDec  [Axis] = new double();


                nMotrType[Axis] = (int)EN_MOTOR_TYPE.mtServo;
    			nSpdRatio[Axis] = (int)EN_SPEED_RATIO.sr100 ;
    
    			nNotUse  [Axis] = 0;
    			nMotrKind[Axis] = 0;
    			dMinPosn [Axis] = 0;
    			dMaxPosn [Axis] = 0;
    			dMinVel  [Axis] = 0;
    			dMaxVel  [Axis] = 0;
    			dMinAcc  [Axis] = 0;
    			dMinDec  [Axis] = 0;
    			nCWLevel [Axis] = 0;
    			nCCWLevel[Axis] = 0;
    		}
    
    		//dDP_SPDLZ_MOVEX_CHECKPOS = 0.0; 
    	}
    }

    //Motor Limit.
    //===========================================================================
    public struct MOTR_LIMT 
    {
        public double MaxPos;
        public double MaxVel;
        public double MaxAcc;
        public double MinPos;
        public double MinVel;
        public double MinAcc;

        public MOTR_LIMT(double d)
    	{
    		MaxPos = d;
    		MaxVel = d;
    		MaxAcc = d;
    		MinPos = d;
    		MinVel = d;
    		MinAcc = d;
    	}
    
    }
    //---------------------------------------------------------------------------
    public struct DSTB_POSN
    {
        public double dPos ;
        public string sName;
        public string sDesc;
        public string sUnit;

        public DSTB_POSN(double d)
        {
            dPos  = d  ;
            sName = string.Empty;
            sDesc = string.Empty;
            sUnit = string.Empty;

        }
    }

    public struct MOTN_PARA
    {
        public int      iType      ;
        public bool     bRing      ;
        public double   dPulse     ;
        public double   dPitch     ;

        public double[] dPosn      ;
        public double[] dVel       ;
        public double[] dAcc       ;
        public double[] dDec       ;
        public double[] dTime      ;
        public string[] sPosn_Desc ;
        public int   [] iPosnKind  ;

        public double   MaxPos     ;
        public double   MaxVel     ;
        public double   MaxAcc     ;
        public double   MinPos     ;
        public double   MinVel     ;
        public double   MinAcc     ;

        public double[] dArea1     ;
        public double[] dArea2     ;

        public MOTN_PARA(double d)
        {
            iType   = 0;
            bRing   = false;
            dPulse  = d;
            dPitch  = d;

            MaxPos  = d;
            MaxVel  = d;
            MaxAcc  = d;
            MinPos  = d;
            MinVel  = d;
            MinAcc  = d;

            dPosn      = new double[MAX_POSN];
            sPosn_Desc = new string[MAX_POSN];
            iPosnKind  = new int   [MAX_POSN];
            dVel       = new double[MAX_SPED];
            dAcc       = new double[MAX_SPED];
            dDec       = new double[MAX_SPED];
            dTime      = new double[MAX_DLAY];

            dArea1     = new double[20];
            dArea2     = new double[20];

            for (int n = 0; n < MAX_POSN; n++)
            {
                dPosn      [n] = new double();
                sPosn_Desc [n] = string.Empty;
                iPosnKind  [n] = new int   ();
            }
            for (int n = 0; n < MAX_SPED; n++)
            {

                dVel[n] = new double();
                dAcc[n] = new double();
                dDec[n] = new double();
            }
            for (int n = 0; n < MAX_DLAY; n++)
            {
                dTime[n] = new double();
            }

            dArea1 = new double[20];
            dArea2 = new double[20];

        }
    }

    //---------------------------------------------------------------------------
    public struct VISN_INSP_INFO
    {
        public bool   bFind;
                      
        public int    nFindMode;
        public int    nBtmWhere;

        public double dXpos;
        public double dYpos;

        public VISN_INSP_INFO(bool find)
        {
            bFind     = find;
            nFindMode = -1;
            nBtmWhere = -1;
            dXpos     = 0.0;
            dYpos     = 0.0;
        }
    }

}