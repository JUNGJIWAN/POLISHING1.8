using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaferPolishingSystem;
using static WaferPolishingSystem.Define.UserEnum    ;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserINI;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserFile;
using static WaferPolishingSystem.FormMain;
using System.Windows.Media;
using WaferPolishingSystem.Define;

namespace WaferPolishingSystem.BaseUnit
{
    public class FileManager
    {
        //Var
        int      m_nCrntLevel   ; //현재 설비 적용 Level

        String   m_sVersion     ; //PGM Version
        string   m_strPath      ;
        string   m_strCurrJob   ;
        string   m_strRecipeName;
        string   m_sOperId      ;
        //string   m_sUserID      ;

        int      m_nHisCnt ;

        string[] sHistory;

        //
        public ST_MASTER_OPTION   m_stMasterOpt   = new ST_MASTER_OPTION(0);
        public ST_SYSTEM_OPTION   m_stSystemOpt   = new ST_SYSTEM_OPTION(0);
        //public ST_SYSTEM_BASE     m_stSystemBase  = new ST_SYSTEM_BASE  (0);
        public ST_PROJECT_BASE    m_stProjectBase = new ST_PROJECT_BASE (0);
        public ST_PASS_WORD       m_stPassWord    = new ST_PASS_WORD    ("");
        
        public ST_RECIPE          m_stRecipe      = new ST_RECIPE       (0); 
        public ST_RECIPE          m_stRecipe_Buff = new ST_RECIPE       (0); //for Modify

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public string _sVersion { get { return m_sVersion; } }
        public int    _nHisCnt { get { return m_nHisCnt ; } }
        public string this[int a]  { get { return sHistory[a]; } }

        public string _strPath
        {
            get { return m_strPath; }
            set { m_strPath = value; }
        }
        public string _sCurrJob
        {
            get { return m_strCurrJob; }
            set { m_strCurrJob = value; }
        }
        public string _sRecipeName
        {
            get { return m_strRecipeName;  }
            set { m_strRecipeName = value; }
        }
        public string _sOperId
        {
            get { return m_sOperId; }
            set { m_sOperId = value; }
        }
        public int _nCrntLevel
        {
            get { return m_nCrntLevel; }
            set { m_nCrntLevel = value; }
        }

        //
        //static public FileManager FM = null; 


        //---------------------------------------------------------------------------
        //생성자
        public FileManager()
        {
            //
            //FM = this;

            Init();

            //
            fn_DefaultSet();

            //
            fn_SetVersion();

        }
        //---------------------------------------------------------------------------
        public void Init()
        {
            m_sVersion      = string.Empty;
            m_strPath       = string.Empty;
            m_strCurrJob    = string.Empty;
            m_strRecipeName = string.Empty;
            
            m_nHisCnt       = 0 ;

            sHistory        = new string[1000];

            //Set Default
            //m_sUserID       = "SMEC";
            m_nCrntLevel    = 0;

            //directory
            m_strPath       = fn_GetExePath();
            m_strCurrJob    = "Default";
        }
        //---------------------------------------------------------------------------
        public void fn_DefaultSet()
        {
            //
            fn_DefaultMasterOpt  ();
            fn_DefaultSystemOpt  ();
            fn_DefaultSystemBase ();
            fn_DefaultProjectBase();
            fn_DefaultRecipe     ();
            fn_DefaultPassWord   ();
            
        }
        //---------------------------------------------------------------------------
        public void fn_DefaultPassWord()
        {
            m_stPassWord.strMake = "SMEC1203";
        }
        //---------------------------------------------------------------------------
        public void fn_DefaultMasterOpt(bool reset = false)
        {
            m_stMasterOpt.nUseSkipDoor      = 0;
            m_stMasterOpt.nUseSkipLeak      = 0;
            m_stMasterOpt.nUseSkipGrid      = 0;
            m_stMasterOpt.nUseSkipWaterLeak = 0;
            m_stMasterOpt.nUseSkipFan       = 0;
            m_stMasterOpt.nUseSkipAir       = 0;
            m_stMasterOpt.nUseSkipWaterLvl  = 0;
            m_stMasterOpt.nUseSkipAccura    = 0;
            m_stMasterOpt.nUseSkipDP        = 0;

            m_stMasterOpt.nToolSkip         = 0;
            m_stMasterOpt.nPlateSkip        = 0;
            m_stMasterOpt.nStorageSkip      = 0;
            m_stMasterOpt.nMagaSkip         = 0;

            if(!reset) m_stMasterOpt.nEPDOnlyMeasure = 0;

            m_stMasterOpt.nRunMode          = EN_RUN_MODE.AUTO_MODE;
            
            for (int i = 0; i < m_stMasterOpt.bAutoOff.Length; i++)
            {
                m_stMasterOpt.bAutoOff[i] = false; 
            }
        }
        //---------------------------------------------------------------------------
        public void fn_DefaultSystemOpt()
        {
            //m_stSystemOpt.strMachineName    = "POLISHING 2.0";
            //m_stSystemOpt.strMachineNo      = "";
            //m_stSystemOpt.strLogPath        = "";

            m_stSystemOpt.nRunMode          = (int)EN_MC_MODE.REMOTE;

            //m_stSystemOpt.nUseWarming       = 0;
            //m_stSystemOpt.nWarmInterval     = 0;
            //m_stSystemOpt.nWarmInterval1    = 0;
            //m_stSystemOpt.nMotrRepeat       = 0;
            //m_stSystemOpt.nClampRepeat      = 0;
            //m_stSystemOpt.nUtilRepeat       = 0;
            //m_stSystemOpt.nPoliMillingTime  = 0;
            //m_stSystemOpt.nCleanMillingTime = 0;


            //for (int i = 0; i< MAX_MOTOR; i++)
            //{
            //    m_stSystemOpt.bUseMotor[i] = false;
            //}

            
            //for (int i = 0; i < 3; i++)
            //{
            //    m_stSystemOpt.bUseClamp[i] = false;
            //}

            
            //for (int i = 0; i < 2; i++)
            //{
            //    m_stSystemOpt.bUseUtil[i] = false;
            //}

            //for (int i = 0; i < 5; i++)
            //{
            //    m_stSystemOpt.stUserSet[i].bMotion  = false;
            //    m_stSystemOpt.stUserSet[i].bSetting = false;
            //    m_stSystemOpt.stUserSet[i].bRecipe  = false;
            //    m_stSystemOpt.stUserSet[i].bLog     = false;
            //    m_stSystemOpt.stUserSet[i].bMaster  = false;
            //    m_stSystemOpt.stUserSet[i].bExit    = false;
            //
            //}

        }
        //---------------------------------------------------------------------------
        public void fn_DefaultSystemRunOpt()
        {
            m_stSystemOpt.nRunMode       = (int)EN_MC_MODE.REMOTE;
            
            m_stMasterOpt.nRunSpeed      = 0;

            m_stMasterOpt.nPlateSkip     = 0;
            m_stMasterOpt.nToolSkip      = 0;
            m_stMasterOpt.nVisionSkip    = 0;
            m_stMasterOpt.nForceSkip     = 0;
            m_stMasterOpt.nMagaSkip      = 0;


        }
        //---------------------------------------------------------------------------
        public void fn_DefaultSystemChkOpt()
        {
            m_stMasterOpt.nUseSkipDoor      = 0;
            m_stMasterOpt.nUseSkipLeak      = 0;
            m_stMasterOpt.nUseSkipGrid      = 0;
            m_stMasterOpt.nUseSkipWaterLeak = 0;
            m_stMasterOpt.nUseSkipWaterLvl  = 0;
            m_stMasterOpt.nUseSkipAccura    = 0;
            m_stMasterOpt.nUseSkipDP        = 0;


            m_stMasterOpt.nToolSkip         = 0;
            m_stMasterOpt.nPlateSkip        = 0;
            m_stMasterOpt.nStorageSkip      = 0;
            m_stMasterOpt.nMagaSkip         = 0;

            m_stMasterOpt.nRunMode          = (int)EN_RUN_MODE.AUTO_MODE;

            for (int i = 0; i < MAX_SEQ_PART; i++)
            {
                m_stMasterOpt.bAutoOff[i] = false;
            }

        }

        //---------------------------------------------------------------------------
        public void fn_DefaultSystemBase()
        {
            //m_stSystemBase.nRunMode  = 0;
            //m_stSystemBase.nRunSpeed = 0; 
        }
        //---------------------------------------------------------------------------
        public void fn_DefaultProjectBase()
        {
            m_stProjectBase.sJobName = "";

            m_stProjectBase.nStorage_Col       = 0  ;
            m_stProjectBase.nStorage_Row       = 0  ;
            m_stProjectBase.nMagazine_Row      = 0  ;
            m_stProjectBase.nMagazine_Col      = 0  ;

            m_stProjectBase.dStorPitch_Row     = 0.0;
            m_stProjectBase.dStorPitch_Col     = 0.0;
            m_stProjectBase.dMagaPitch_Row     = 0.0;
            m_stProjectBase.dMagaPitch_Col     = 0.0;

            m_stProjectBase.dPreAlignOffset_X  = 0.0;
            m_stProjectBase.dPreAlignOffset_TH = 0.0;
            m_stProjectBase.dPolishOffset_X    = 0.0;
            m_stProjectBase.dPolishOffset_Y    = 0.0;
            m_stProjectBase.dPolishOffset_TH   = 0.0;
            m_stProjectBase.dPolishOffset_TI   = 0.0;
            m_stProjectBase.dCleanOffset_R     = 0.0;
            m_stProjectBase.dStorOffset_Row    = 0.0;
            m_stProjectBase.dStorOffset_Col    = 0.0;

        }
        //---------------------------------------------------------------------------
        private void fn_DefaultRecipe()
        {

        }


        //---------------------------------------------------------------------------
        public string fn_GetPass(int Level)
        {
            if      (Level == (int)EN_USER_LEVEL.lvEngineer) return m_stPassWord.strEngr;
            //else if (Level == (int)EN_USER_LEVEL.lvTechnis ) return m_stPassWord.strTech;
            else if (Level == (int)EN_USER_LEVEL.lvMaster  ) return m_stPassWord.strMstr;
            else if (Level == (int)EN_USER_LEVEL.lvMaker   ) return m_stPassWord.strMake;
            else                                             return "SMEC_12345";

        }
        //---------------------------------------------------------------------------
        public string fn_GetPass(EN_USER_LEVEL Level)
        {
            if      (Level == EN_USER_LEVEL.lvEngineer) return m_stPassWord.strEngr;
            //else if (Level == EN_USER_LEVEL.lvTechnis ) return m_stPassWord.strTech;
            else if (Level == EN_USER_LEVEL.lvMaster  ) return m_stPassWord.strMstr;
            else if (Level == EN_USER_LEVEL.lvMaker   ) return m_stPassWord.strMake;
            else                                        return "SMEC_12345";

        }
        //---------------------------------------------------------------------------
        public int fn_GetLevel    () { return m_nCrntLevel; }
        public bool fn_IsLvlMaster() { return m_nCrntLevel == (int)EN_USER_LEVEL.lvMaster; }
        //---------------------------------------------------------------------------
        public string fn_GetLevelText() 
        {
            string sLevel = string.Empty ;

            switch (m_nCrntLevel)
            {
                case (int)EN_USER_LEVEL.lvOperator:
                    sLevel = "OPERATOR";
                    break;
                case (int)EN_USER_LEVEL.lvEngineer:
                    sLevel = "ENGINEER";
                    break;
                case (int)EN_USER_LEVEL.lvMaster:
                    sLevel = "ADMIN";  //sLevel = "MASTER";
                    break;
                case (int)EN_USER_LEVEL.lvMaker:
                    sLevel = "MAKER";
                    break;
//                 case (int)EN_USER_LEVEL.lvAdmin:
//                     sLevel = "ADMIN";
//                     break;
                default:
                    sLevel = "OPERATOR";
                    break;
            }

            return sLevel; 
        }

        //---------------------------------------------------------------------------
        //Apply Device.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void ApplyProject(string JobName)
        {
            //
            MOTR.Load            (true, JobName);
            MOTR.SetAxis_AsDevice(             );

            //
            DM.fn_SetRowColInfo();

            //
            for (int m = 0; m < MOTR._iNumOfMotr; m++)
            {
                MOTR.SetEachPitchSlot((EN_MOTR_ID)m);
            }

            MOTR.SetStragePitch  (m_stProjectBase.dStorPitch_Row, m_stProjectBase.dStorPitch_Col);
            MOTR.SetMagazinePitch(m_stProjectBase.dMagaPitch_Row, m_stProjectBase.dMagaPitch_Col);  //JUNG/200506

            m_strCurrJob = JobName;

            fn_LoadLastInfo(false);

        }
        //---------------------------------------------------------------------------
        public void ApplySystem()
        {

        }

        //---------------------------------------------------------------------------
        //Get Mode 
        //public bool fn_GetRunMode(int mode        ) { return m_stMasterOpt.nRunMode == mode     ; }
        
        public bool fn_GetRunMode(EN_RUN_MODE mode) { return m_stMasterOpt.nRunMode ==      mode; }
        public bool fn_GetMCMode (EN_MC_MODE  mode) { return m_stSystemOpt.nRunMode == (int)mode; }

        public EN_RUN_MODE fn_GetRunMode(         ) { return m_stMasterOpt.nRunMode            ; }
        public EN_MC_MODE  fn_GetMCMode (         ) { return (EN_MC_MODE)m_stSystemOpt.nRunMode; }
        //---------------------------------------------------------------------------
        public void fn_LoadProject(bool bload, string sJobName = "") //현재는 JOB이 1개 뿐인 상태라...
        {
	        //Local Var.
            bool    brtn    ; 
	        string  sIniPath;
	        sIniPath = m_strPath + "PROJECT"; 
	        brtn = fn_CheckDir(sIniPath);	

	        if (sJobName == "") sJobName = m_strCurrJob; //Default 기본 JOB
	        sIniPath = sIniPath + "\\" + sJobName;
	        brtn = fn_CheckDir(sIniPath);

	        sIniPath = sIniPath + "\\Project.ini";

	        if (bload)
	        {
	        	m_stProjectBase.sJobName           = fn_Load("NAME"  , "JOB_NAME"   , "", sIniPath);
	        								     								  
	        	m_stProjectBase.nStorage_Row       = fn_Load("COUNT" ,"STORAGE_ROW" ,   0, sIniPath);
	        	m_stProjectBase.nStorage_Col       = fn_Load("COUNT" ,"STORAGE_COL" ,   0, sIniPath);
	        	m_stProjectBase.nMagazine_Row      = fn_Load("COUNT" ,"MAGAZINE_ROW",   0, sIniPath);
	        	m_stProjectBase.nMagazine_Col      = fn_Load("COUNT" ,"MAGAZINE_COL",   0, sIniPath);
	        				      			     					
	        	m_stProjectBase.dStorPitch_Row     = fn_Load("PITCH" ,"STORAGE_ROW" , 0.0, sIniPath);
	        	m_stProjectBase.dStorPitch_Col     = fn_Load("PITCH" ,"STORAGE_COL" , 0.0, sIniPath);
	        	m_stProjectBase.dMagaPitch_Row     = fn_Load("PITCH" ,"MAGAZINE_ROW", 0.0, sIniPath);
	        	m_stProjectBase.dMagaPitch_Col     = fn_Load("PITCH" ,"MAGAZINE_COL", 0.0, sIniPath);
	        														
	        	m_stProjectBase.dPreAlignOffset_X  = fn_Load("OFFSET","PREALIGN_X"  , 0.0, sIniPath);
	        	m_stProjectBase.dPreAlignOffset_TH = fn_Load("OFFSET","PREALIGN_TH" , 0.0, sIniPath);
	        	m_stProjectBase.dPolishOffset_X    = fn_Load("OFFSET","POLISHING_X" , 0.0, sIniPath);
	        	m_stProjectBase.dPolishOffset_Y    = fn_Load("OFFSET","POLISHING_Y" , 0.0, sIniPath);
	        	m_stProjectBase.dPolishOffset_TH   = fn_Load("OFFSET","POLISHING_TH", 0.0, sIniPath);
	        	m_stProjectBase.dPolishOffset_TI   = fn_Load("OFFSET","POLISHING_TI", 0.0, sIniPath);
                m_stProjectBase.dCleanOffset_R     = fn_Load("OFFSET","CLEANING_R"  , 0.0, sIniPath);
                m_stProjectBase.dStorOffset_Row    = fn_Load("OFFSET","STORAGE_ROW" , 0.0, sIniPath);
	        	m_stProjectBase.dStorOffset_Col    = fn_Load("OFFSET","STORAGE_COL" , 0.0, sIniPath);


                if (m_stProjectBase.nStorage_Row  < 1) m_stProjectBase.nStorage_Row  = 1;
                if (m_stProjectBase.nStorage_Col  < 1) m_stProjectBase.nStorage_Col  = 1;
                if (m_stProjectBase.nMagazine_Row < 1) m_stProjectBase.nMagazine_Row = 1;
                if (m_stProjectBase.nMagazine_Col < 1) m_stProjectBase.nMagazine_Col = 1;


            }
	        else 
	        {
	        	fn_Save("NAME"  , "JOB_NAME"   , m_stProjectBase.sJobName          , sIniPath);

	        	fn_Save("COUNT" ,"STORAGE_ROW" , m_stProjectBase.nStorage_Row      , sIniPath);
	        	fn_Save("COUNT" ,"STORAGE_COL" , m_stProjectBase.nStorage_Col      , sIniPath);
	        	fn_Save("COUNT" ,"MAGAZINE_ROW", m_stProjectBase.nMagazine_Row     , sIniPath);
	        	fn_Save("COUNT" ,"MAGAZINE_COL", m_stProjectBase.nMagazine_Col     , sIniPath);

	        	fn_Save("PITCH" ,"STORAGE_ROW" , m_stProjectBase.dStorPitch_Row    , sIniPath);
	        	fn_Save("PITCH" ,"STORAGE_COL" , m_stProjectBase.dStorPitch_Col    , sIniPath);
	        	fn_Save("PITCH" ,"MAGAZINE_ROW", m_stProjectBase.dMagaPitch_Row    , sIniPath);
	        	fn_Save("PITCH" ,"MAGAZINE_COL", m_stProjectBase.dMagaPitch_Col    , sIniPath);

	        	fn_Save("OFFSET","PREALIGN_X"  , m_stProjectBase.dPreAlignOffset_X , sIniPath);
	        	fn_Save("OFFSET","PREALIGN_TH" , m_stProjectBase.dPreAlignOffset_TH, sIniPath);
	        	fn_Save("OFFSET","POLISHING_X" , m_stProjectBase.dPolishOffset_X   , sIniPath);
	        	fn_Save("OFFSET","POLISHING_Y" , m_stProjectBase.dPolishOffset_Y   , sIniPath);
	        	fn_Save("OFFSET","POLISHING_TH", m_stProjectBase.dPolishOffset_TH  , sIniPath);
	        	fn_Save("OFFSET","POLISHING_TI", m_stProjectBase.dPolishOffset_TI  , sIniPath);
                fn_Save("OFFSET","CLEANING_R"  , m_stProjectBase.dCleanOffset_R    , sIniPath);
                fn_Save("OFFSET","STORAGE_ROW" , m_stProjectBase.dStorOffset_Row   , sIniPath);
	        	fn_Save("OFFSET","STORAGE_COL" , m_stProjectBase.dStorOffset_Col   , sIniPath);

	        }

        }
        //---------------------------------------------------------------------------
        public void fn_LoadEngrOptn(bool bload)
        {
        
            /*
            string sIniPath = string.Empty;
            sIniPath = m_strPath + "SYSTEM\\Engr.ini";
            
            if (bload)
            {

                m_stMasterOpt.nRunMode                         = fn_Load("SYSTEM", "RUNMODE"  , 0, sIniPath);
                m_stMasterOpt.nUseSkipDoor                     = fn_Load("SYSTEM", "SKIPDOOR" , 0, sIniPath);
                m_stMasterOpt.nUseSkipLeak                     = fn_Load("SYSTEM", "SKIPLEAK" , 0, sIniPath);
                m_stMasterOpt.nUseSkipGrid                     = fn_Load("SYSTEM", "SKIPGRID" , 0, sIniPath);
                m_stMasterOpt.nUseSkipWaterLeak                = fn_Load("SYSTEM", "SKIPWATER", 0, sIniPath);
                

                //
                m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piSPDL] = fn_Load("AUTO_OFF", "SPDL", false, sIniPath);
                m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piPOLI] = fn_Load("AUTO_OFF", "POLI", false, sIniPath);
                m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piCLEN] = fn_Load("AUTO_OFF", "CLEN", false, sIniPath);
                m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piSTRG] = fn_Load("AUTO_OFF", "STRG", false, sIniPath);
                m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piLOAD] = fn_Load("AUTO_OFF", "LOAD", false, sIniPath);


            } 
            else
            {
                fn_Save("SYSTEM"  , "RUNMODE"  , m_stMasterOpt.nRunMode         , sIniPath);
                fn_Save("SYSTEM"  , "SKIPDOOR" , m_stMasterOpt.nUseSkipDoor     , sIniPath);
                fn_Save("SYSTEM"  , "SKIPLEAK" , m_stMasterOpt.nUseSkipLeak     , sIniPath);
                fn_Save("SYSTEM"  , "SKIPGRID" , m_stMasterOpt.nUseSkipGrid     , sIniPath);
                fn_Save("SYSTEM"  , "SKIPWATER", m_stMasterOpt.nUseSkipWaterLeak, sIniPath);

                fn_Save("AUTO_OFF", "SPDL"     , m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piSPDL], sIniPath);
                fn_Save("AUTO_OFF", "POLI"     , m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piPOLI], sIniPath);
                fn_Save("AUTO_OFF", "CLEN"     , m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piCLEN], sIniPath);
                fn_Save("AUTO_OFF", "STRG"     , m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piSTRG], sIniPath);
                fn_Save("AUTO_OFF", "LOAD"     , m_stMasterOpt.bAutoOff[(int)EN_PART_ID.piLOAD], sIniPath);

            }
            */


        }
        //---------------------------------------------------------------------------
        public void fn_LoadPassWord(bool bload)
        {
            string sIniPath = string.Empty;
            sIniPath = m_strPath + "SYSTEM\\PassWord.ini";
            
            if (bload)
            {
                m_stPassWord.strTech       = fn_Load("TECH"    , "PW" , "", sIniPath);
                m_stPassWord.strEngr       = fn_Load("ENGINEER", "PW" , "", sIniPath);
                m_stPassWord.strMstr       = fn_Load("MASTER"  , "PW" , "", sIniPath);
                m_stPassWord.strMake       = fn_Load("MAKER"   , "PW" , "", sIniPath);
                m_stPassWord.strAdmin      = fn_Load("ADMIN"   , "PW" , "", sIniPath);
                

            } 
            else
            {
                fn_Save("TECH"    , "PW" , m_stPassWord.strTech , sIniPath);
                fn_Save("ENGINEER", "PW" , m_stPassWord.strEngr , sIniPath);
                fn_Save("MASTER"  , "PW" , m_stPassWord.strMstr , sIniPath);
                fn_Save("MAKER"   , "PW" , m_stPassWord.strMake , sIniPath);
                fn_Save("ADMIN"   , "PW" , m_stPassWord.strAdmin, sIniPath);
            }
        }
        //---------------------------------------------------------------------------

        public void fn_LoadMastOptn(bool bload)
        {
            string sIniPath = string.Empty;
            sIniPath = m_strPath + "SYSTEM\\Master.ini";
            

            if (bload)
            {
                m_stMasterOpt.sLoadCellSN       = fn_Load("LOADCELL", "Serial_No", "834592", sIniPath);
                m_stMasterOpt.nOffsetDefault    = fn_Load("LOADCELL", "Offset"   , 0       , sIniPath);
                m_stMasterOpt.dTareValue        = fn_Load("LOADCELL", "TareValue", 0.0     , sIniPath);
                m_stMasterOpt.dFullScaleLoaded  = fn_Load("LOADCELL", "FullScale", 0.0     , sIniPath);
                                                
                m_stMasterOpt.dUtilOffset       = fn_Load("UTIL"    , "Offset", 0.0     , sIniPath);
                                                
                                                
                LDCBTM._nOffsetDefault          = m_stMasterOpt.nOffsetDefault   ;
                LDCBTM._dTareValue              = m_stMasterOpt.dTareValue       ;
                LDCBTM._dFullScaleLoaded        = m_stMasterOpt.dFullScaleLoaded ;
                                                
                                                
                m_stMasterOpt.nDrainTime        = fn_Load("VALVE"   , "DrainTime   "     , 0    , sIniPath);
                m_stMasterOpt.nSepBlowTime      = fn_Load("VALVE"   , "SepBlowTime "     , 0    , sIniPath);
                m_stMasterOpt.nSuckBackTime     = fn_Load("VALVE"   , "SuckBackTime"     , 0    , sIniPath);
                                                                                         
                m_stMasterOpt.nUseVision        = fn_Load("OPTION"  , "UseVision"        , 0    , sIniPath);
                m_stMasterOpt.nUseDirPos        = fn_Load("OPTION"  , "UseDirPos"        , 0    , sIniPath);
                m_stMasterOpt.nUseCalForce      = fn_Load("OPTION"  , "UseCalForce"      , 0    , sIniPath);
                m_stMasterOpt.nUseRESTApi       = fn_Load("OPTION"  , "UseRESTApi"       , 0    , sIniPath);
                m_stMasterOpt.nEPDOnlyMeasure   = fn_Load("OPTION"  , "EPDOnlyMeasure"   , 0    , sIniPath);
                m_stMasterOpt.nUseMOC           = fn_Load("OPTION"  , "UseMOC"           , 0    , sIniPath);

                m_stMasterOpt.nUsePMC           = fn_Load("OPTION"  , "UsePMC"           , 0    , sIniPath);
                m_stMasterOpt.nUseDCOMReset     = fn_Load("OPTION"  , "UseDCOMReset"     , 0    , sIniPath);
                m_stMasterOpt.dDCOMRatio        = fn_Load("OPTION"  , "DCOMRatio"        , 0.0  , sIniPath);
                m_stMasterOpt.nDCOMCnt          = fn_Load("OPTION"  , "DCOMCnt"          , 0    , sIniPath);
                //m_stMasterOpt.nUseAutoSlury     = fn_Load("OPTION"  , "UseAutoSlury"     , 0    , sIniPath);
                m_stMasterOpt.dLDCBtmOffset     = fn_Load("OPTION"  , "BTMOffset"        , 0.0  , sIniPath);
                m_stMasterOpt.dSpdOffset        = fn_Load("OPTION"  , "SpdOffset"        , 0.0  , sIniPath);
                
                m_stMasterOpt.dTopLDCellOffset  = fn_Load("OPTION"  , "TopLDCOffset"     , 0.0  , sIniPath);
                
                //JUNG/200922
                if(m_stMasterOpt.dTopLDCellOffset < 1) IO.fn_SetTopOffset();
                else IO.fn_SetTopLoadCellOffset(m_stMasterOpt.dTopLDCellOffset);

                if (m_stMasterOpt.nDrainTime    < 1) m_stMasterOpt.nDrainTime    = 1;   
                if (m_stMasterOpt.nSepBlowTime  < 1) m_stMasterOpt.nSepBlowTime  = 1; 
                if (m_stMasterOpt.nSuckBackTime < 1) m_stMasterOpt.nSuckBackTime = 1;


                m_stMasterOpt.dPickOffset      = fn_Load("OPTION"  , "TRPickOffset"  , 0    , sIniPath);
                m_stMasterOpt.dPlaceOffset     = fn_Load("OPTION"  , "TRPlaceOffset" , 0    , sIniPath);

                MOTR.SetTransferPPOffset(m_stMasterOpt.dPickOffset, m_stMasterOpt.dPlaceOffset);

                m_stMasterOpt.nUtilMaxTime     = fn_Load("OPTION"  , "UtilMaxTime"   , 0    , sIniPath); //Utility Supply Max Time
                if (m_stMasterOpt.nUtilMaxTime < 5) m_stMasterOpt.nUtilMaxTime = 5;


                m_stMasterOpt.dYIntercept      = fn_Load("LOADCELL"  , "YIntercept"     , 0.0  , sIniPath);
                m_stMasterOpt.dYSlope          = fn_Load("LOADCELL"  , "YSlope"         , 0.0  , sIniPath);
                m_stMasterOpt.dforceOffset     = fn_Load("LOADCELL"  , "forceOffset"    , 0.0  , sIniPath);
                                                                                        
                m_stMasterOpt.dYInterceptBT    = fn_Load("LOADCELL"  , "YInterceptBT"   , 0.0  , sIniPath);
                m_stMasterOpt.dYSlopeBT        = fn_Load("LOADCELL"  , "YSlopeBT"       , 0.0  , sIniPath);

                m_stMasterOpt.dStartDCOM       = fn_Load("AUTOCAL"   , "StartDCOM"      , 2.5  , sIniPath); //JUNG/210127

                //Min/Max Position
                string sKey = string.Empty; 
                for (int n = 0; n < MAX_MOTOR; n++)
                {
                    sKey = string.Format($"HomeOffset{n:D2}_({STR_MOTOR_ID[n]})");
                    m_stMasterOpt.dHomeOffset[n] = fn_Load("MOTOR", sKey, 0.0, sIniPath);

                    sKey = string.Format($"MinPos{n:D2}_({STR_MOTOR_ID[n]})"); m_stMasterOpt.dMinPos[n] = fn_Load("MOTOR_MINMAX", sKey, 0.0, sIniPath);
                    sKey = string.Format($"MaxPos{n:D2}_({STR_MOTOR_ID[n]})"); m_stMasterOpt.dMaxPos[n] = fn_Load("MOTOR_MINMAX", sKey, 0.0, sIniPath);
                    sKey = string.Format($"MinVel{n:D2}_({STR_MOTOR_ID[n]})"); m_stMasterOpt.dMinVel[n] = fn_Load("MOTOR_MINMAX", sKey, 0.0, sIniPath);
                    sKey = string.Format($"MaxVel{n:D2}_({STR_MOTOR_ID[n]})"); m_stMasterOpt.dMaxVel[n] = fn_Load("MOTOR_MINMAX", sKey, 0.0, sIniPath);
                    sKey = string.Format($"MinAcc{n:D2}_({STR_MOTOR_ID[n]})"); m_stMasterOpt.dMinAcc[n] = fn_Load("MOTOR_MINMAX", sKey, 0.0, sIniPath);
                    sKey = string.Format($"MaxAcc{n:D2}_({STR_MOTOR_ID[n]})"); m_stMasterOpt.dMaxAcc[n] = fn_Load("MOTOR_MINMAX", sKey, 0.0, sIniPath);

                }
                MOTR.SetHomeOffset();
                //MOTR.InitMinMaxData(ref m_stMasterOpt.dMinPos, ref m_stMasterOpt.dMaxPos, ref m_stMasterOpt.dMinVel, ref m_stMasterOpt.dMaxVel, ref m_stMasterOpt.dMinAcc, ref m_stMasterOpt.dMaxAcc);


                /*
                m_stMasterOpt.nUseSkipDoor         = fn_Load("SYSTEM", "SKIPDOOR"    , 0, sIniPath);
                m_stMasterOpt.nUseSkipLeak         = fn_Load("SYSTEM", "SKIPLEAK"    , 0, sIniPath);
                m_stMasterOpt.nUseSkipGrid         = fn_Load("SYSTEM", "SKIPGRID"    , 0, sIniPath);
                m_stMasterOpt.nUseSkipWaterLeak    = fn_Load("SYSTEM", "SKIPWATER"   , 0, sIniPath);
                m_stMasterOpt.nRunMode             = fn_Load("SYSTEM", "RUNMODE"     , 0, sIniPath);
                m_stMasterOpt.nRunSpeed            = fn_Load("SYSTEM", "RUNSPEED"    , 0, sIniPath);
                m_stMasterOpt.nPlateSkip           = fn_Load("SYSTEM", "PLATE_SKIP"  , 0, sIniPath);
                m_stMasterOpt.nToolSkip            = fn_Load("SYSTEM", "TOOL_SKIP"   , 0, sIniPath);
                m_stMasterOpt.nVisionSkip          = fn_Load("SYSTEM", "VISN_SKIP"   , 0, sIniPath);
                m_stMasterOpt.nForceSkip           = fn_Load("SYSTEM", "FORCE_SKIP"  , 0, sIniPath);
                */

            }
            else
            {

                m_stMasterOpt.nOffsetDefault   = LDCBTM._nOffsetDefault   ;
                m_stMasterOpt.dTareValue       = LDCBTM._dTareValue       ;
                m_stMasterOpt.dFullScaleLoaded = LDCBTM._dFullScaleLoaded ;

                fn_Save("LOADCELL", "Serial_No"        , m_stMasterOpt.sLoadCellSN       , sIniPath);
                fn_Save("LOADCELL", "Offset"           , m_stMasterOpt.nOffsetDefault    , sIniPath);
                fn_Save("LOADCELL", "TareValue"        , m_stMasterOpt.dTareValue        , sIniPath);
                fn_Save("LOADCELL", "FullScale"        , m_stMasterOpt.dFullScaleLoaded  , sIniPath);
                                                       
                fn_Save("UTIL"    , "Offset"           , m_stMasterOpt.dUtilOffset       , sIniPath);
                                                       
                fn_Save("VALVE"   , "DrainTime"        , m_stMasterOpt.nDrainTime        , sIniPath);
                fn_Save("VALVE"   , "SepBlowTime"      , m_stMasterOpt.nSepBlowTime      , sIniPath);
                fn_Save("VALVE"   , "SuckBackTime"     , m_stMasterOpt.nSuckBackTime     , sIniPath);
                                                                                        
                fn_Save("OPTION"  , "UseVision"        , m_stMasterOpt.nUseVision        , sIniPath);
                fn_Save("OPTION"  , "UseDirPos"        , m_stMasterOpt.nUseDirPos        , sIniPath);
                fn_Save("OPTION"  , "UseCalForce"      , m_stMasterOpt.nUseCalForce      , sIniPath);
                fn_Save("OPTION"  , "UsePMC"           , m_stMasterOpt.nUsePMC           , sIniPath);
                fn_Save("OPTION"  , "UseDCOMReset"     , m_stMasterOpt.nUseDCOMReset     , sIniPath);
                fn_Save("OPTION"  , "DCOMRatio"        , m_stMasterOpt.dDCOMRatio        , sIniPath);
                fn_Save("OPTION"  , "DCOMCnt"          , m_stMasterOpt.nDCOMCnt          , sIniPath);
                fn_Save("OPTION"  , "BTMOffset"        , m_stMasterOpt.dLDCBtmOffset     , sIniPath);
                fn_Save("OPTION"  , "UseRESTApi"       , m_stMasterOpt.nUseRESTApi       , sIniPath);
                fn_Save("OPTION"  , "EPDOnlyMeasure"   , m_stMasterOpt.nEPDOnlyMeasure   , sIniPath);
                //fn_Save("OPTION"  , "UseAutoSlury"     , m_stMasterOpt.nUseAutoSlury     , sIniPath);
                fn_Save("OPTION"  , "SpdOffset"        , m_stMasterOpt.dSpdOffset        , sIniPath);
                fn_Save("OPTION"  , "TopLDCOffset"     , m_stMasterOpt.dTopLDCellOffset  , sIniPath);

                fn_Save("OPTION"  , "TRPickOffset"     , m_stMasterOpt.dPickOffset       , sIniPath);
                fn_Save("OPTION"  , "TRPlaceOffset"    , m_stMasterOpt.dPlaceOffset      , sIniPath);
                                                       
                fn_Save("OPTION"  , "UtilMaxTime"      , m_stMasterOpt.nUtilMaxTime      , sIniPath);
                fn_Save("OPTION"  , "UseMOC"           , m_stMasterOpt.nUseMOC           , sIniPath);

                MOTR.SetTransferPPOffset(m_stMasterOpt.dPickOffset, m_stMasterOpt.dPlaceOffset);

                fn_Save("LOADCELL", "YIntercept"       , m_stMasterOpt.dYIntercept       , sIniPath); //JUNG/200523
                fn_Save("LOADCELL", "YSlope"           , m_stMasterOpt.dYSlope           , sIniPath);
                fn_Save("LOADCELL", "forceOffset"      , m_stMasterOpt.dforceOffset      , sIniPath); 
                                                       
                fn_Save("LOADCELL", "YInterceptBT"     , m_stMasterOpt.dYInterceptBT     , sIniPath); //JUNG/200908
                fn_Save("LOADCELL", "YSlopeBT"         , m_stMasterOpt.dYSlopeBT         , sIniPath);
                                                       
                fn_Save("AUTOCAL" , "StartDCOM"        , m_stMasterOpt.dStartDCOM        , sIniPath); //JUNG/210127


                string sKey = string.Empty; 
                for (int n = 0; n < MAX_MOTOR; n++)
                {
                    sKey = string.Format($"HomeOffset{n:D2}_({STR_MOTOR_ID[n]})"); fn_Save("MOTOR", sKey, m_stMasterOpt.dHomeOffset[n], sIniPath);

                    sKey = string.Format($"MinPos{n:D2}_({STR_MOTOR_ID[n]})");     fn_Save("MOTOR_MINMAX", sKey, m_stMasterOpt.dMinPos[n], sIniPath);
                    sKey = string.Format($"MaxPos{n:D2}_({STR_MOTOR_ID[n]})");     fn_Save("MOTOR_MINMAX", sKey, m_stMasterOpt.dMaxPos[n], sIniPath);
                    sKey = string.Format($"MinVel{n:D2}_({STR_MOTOR_ID[n]})");     fn_Save("MOTOR_MINMAX", sKey, m_stMasterOpt.dMinVel[n], sIniPath);
                    sKey = string.Format($"MaxVel{n:D2}_({STR_MOTOR_ID[n]})");     fn_Save("MOTOR_MINMAX", sKey, m_stMasterOpt.dMaxVel[n], sIniPath);
                    sKey = string.Format($"MinAcc{n:D2}_({STR_MOTOR_ID[n]})");     fn_Save("MOTOR_MINMAX", sKey, m_stMasterOpt.dMinAcc[n], sIniPath);
                    sKey = string.Format($"MaxAcc{n:D2}_({STR_MOTOR_ID[n]})");     fn_Save("MOTOR_MINMAX", sKey, m_stMasterOpt.dMaxAcc[n], sIniPath);

                }

                /*
                fn_Save("SYSTEM", "SKIPDOOR"   , m_stMasterOpt.nUseSkipDoor     , sIniPath);
                fn_Save("SYSTEM", "SKIPLEAK"   , m_stMasterOpt.nUseSkipLeak     , sIniPath);
                fn_Save("SYSTEM", "SKIPGRID"   , m_stMasterOpt.nUseSkipGrid     , sIniPath);
                fn_Save("SYSTEM", "SKIPWATER"  , m_stMasterOpt.nUseSkipWaterLeak, sIniPath);
                fn_Save("SYSTEM", "RUNMODE"    , m_stMasterOpt.nRunMode         , sIniPath);
                fn_Save("SYSTEM", "RUNSPEED"   , m_stMasterOpt.nRunSpeed        , sIniPath);
                fn_Save("SYSTEM", "PLATE_SKIP" , m_stMasterOpt.nPlateSkip       , sIniPath);
                fn_Save("SYSTEM", "TOOL_SKIP"  , m_stMasterOpt.nToolSkip        , sIniPath);
                fn_Save("SYSTEM", "VISN_SKIP"  , m_stMasterOpt.nVisionSkip      , sIniPath);
                fn_Save("SYSTEM", "FORCE_SKIP" , m_stMasterOpt.nForceSkip       , sIniPath);
                */

            }           
                        
        }
        //---------------------------------------------------------------------------
        public void fn_LoadSysOptn(bool bload)
        {
            string sIniPath = string.Empty;
            sIniPath = m_strPath + "SYSTEM\\SystemInfo.ini";
            
            if (bload)
            {

                m_stSystemOpt.strMachineName    = fn_Load("SYSTEM", "MACHINE_NAME"          , ""   , sIniPath);
                m_stSystemOpt.strMachineNo      = fn_Load("SYSTEM", "MACHINE_NO"            , ""   , sIniPath);
                m_stSystemOpt.strLogPath        = fn_Load("SYSTEM", "LOG_PATH"              , ""   , sIniPath);
               
                m_stSystemOpt.nRunMode          = fn_Load("SYSTEM", "RUNMODE"               , 0    , sIniPath);

                m_stSystemOpt.nPoliMillingTime  = fn_Load("SYSTEM", "POLISHING_MILLING_TIME", 0    , sIniPath);
                m_stSystemOpt.nCleanMillingTime = fn_Load("SYSTEM", "CLEANING_MILLING_TIME" , 0    , sIniPath);

                m_stSystemOpt.dSoftLimitOffset  = fn_Load("SYSTEM", "SOFT_LIMIT_OFFSET"     , 0.0  , sIniPath);
                m_stSystemOpt.nWaterLvlPol      = fn_Load("SYSTEM", "WATER_LEVEL_POLISH"    , 0    , sIniPath);
                m_stSystemOpt.dTargetForce      = fn_Load("SYSTEM", "TARGET_FORCE"          , 0.0  , sIniPath);
                m_stSystemOpt.nStorDir          = fn_Load("SYSTEM", "STORAGE_DIRECTION"     , 0    , sIniPath);

                m_stSystemOpt.nUseCleanAirBlow  = fn_Load("SYSTEM", "CLEAN_AIR_BLOW"        , 0    , sIniPath);
                m_stSystemOpt.nUseLightOnRun    = fn_Load("SYSTEM", "LIGHT_AT_RUN"          , 0    , sIniPath);
                m_stSystemOpt.nUseAllForceChk   = fn_Load("SYSTEM", "FORCE_CHECK_ALL"       , 0    , sIniPath);
                m_stSystemOpt.nUsePolishingCup  = fn_Load("SYSTEM", "USE_POLISH_CUP"        , 0    , sIniPath);
                m_stSystemOpt.nUseAutoSlurry    = fn_Load("SYSTEM", "USE_Auto_Slurry"       , 0    , sIniPath);
                m_stSystemOpt.nSpindleOffCnt    = fn_Load("SYSTEM", "SPINDLE_OFFCUNT"       , 0    , sIniPath);
                m_stSystemOpt.nUseSoftLimit     = fn_Load("SYSTEM", "UseSoftLimit"          , 0    , sIniPath);

                m_stSystemOpt.nUseSpdDirBwd     = fn_Load("SYSTEM"  , "UseSpdDirBwd"        , 0    , sIniPath);
                m_stSystemOpt.nUseSpdDirOnlyFWD = fn_Load("SYSTEM"  , "UseSpdDirOnlyFWD"    , 0    , sIniPath);
                m_stSystemOpt.nUseSpdDirOnlyBWD = fn_Load("SYSTEM"  , "UseSpdDirOnlyBWD"    , 0    , sIniPath);
                m_stSystemOpt.nUsePoliOneDir    = fn_Load("SYSTEM"  , "UsePoliOneDir"       , 0    , sIniPath);
                m_stSystemOpt.nUseSkipPolAlign  = fn_Load("SYSTEM"  , "UseSkipPolAlign"     , 0    , sIniPath);
                m_stSystemOpt.nUseSkipVisError  = fn_Load("SYSTEM"  , "UseSkipVisionError"  , 0    , sIniPath);
                

                if (m_stSystemOpt.dSoftLimitOffset < 0.1) m_stSystemOpt.dSoftLimitOffset = 0.1; 

                m_stSystemOpt.sPMCIp            = fn_Load("PMC"   , "IP"                    , "127.0.0.1", sIniPath);
                m_stSystemOpt.nPMCPort          = fn_Load("PMC"   , "Port"                  ,1550        , sIniPath);

                //
                m_stSystemOpt.nUseWarming       = fn_Load("WARMUP", "USE_WARM"              , 0    , sIniPath);
                m_stSystemOpt.nWarmInterval     = fn_Load("WARMUP", "INTERVAL"              , 0    , sIniPath);
                m_stSystemOpt.nMotrRepeat       = fn_Load("WARMUP", "MOTR_REPEAT"           , 0    , sIniPath);
                m_stSystemOpt.nClampRepeat      = fn_Load("WARMUP", "CLAMP_REPEAT"          , 0    , sIniPath);
                m_stSystemOpt.nUtilRepeat       = fn_Load("WARMUP", "UTIL_REPEAT"           , 0    , sIniPath);
                m_stSystemOpt.nSplyTime         = fn_Load("WARMUP", "SPLY_TIME"             , 0    , sIniPath);

                //RFID
                m_stSystemOpt.sRFIDIp           = fn_Load("RFID"   , "IP"                   , "192.168.0.79", sIniPath);
                RFID.fn_SetIP(m_stSystemOpt.sRFIDIp);

                //Auto Supply
                m_stSystemOpt.sSupplyIp         = fn_Load("SUPPLY", "IP"      , "192.168.0.0", sIniPath);
                m_stSystemOpt.nSupplyPort       = fn_Load("SUPPLY", "PORT"    , 0            , sIniPath);
                m_stSystemOpt.sSupplyEqpId      = fn_Load("SUPPLY", "EQPID"   , "0000"       , sIniPath);
                m_stSystemOpt.nSupplyAddress    = fn_Load("SUPPLY", "ADDRESS" , 0            , sIniPath);

                m_stSystemOpt.sSupplyIp1        = fn_Load("SUPPLY", "IP1"     , "192.168.0.0", sIniPath);
                m_stSystemOpt.nSupplyPort1      = fn_Load("SUPPLY", "PORT1"   , 0            , sIniPath);
                m_stSystemOpt.sSupplyEqpId1     = fn_Load("SUPPLY", "EQPID1"  , "0000"       , sIniPath);
                m_stSystemOpt.nSupplyAddress1   = fn_Load("SUPPLY", "ADDRESS1", 0            , sIniPath);

                //REST
                m_stSystemOpt.sRestApiUrl       = fn_Load("REST", "URL", "http://localhost:55000/", sIniPath);

                string sNo = string.Empty; 
                for (int i = 0; i < MAX_MOTOR; i++)
                {
                    sNo = string.Format($"MOTOR{i+1}");
                    m_stSystemOpt.bUseMotor[i] = fn_Load("WARMUP", sNo, false, sIniPath);
                }
                for (int i = 0; i < 3; i++)
                {
                    sNo = string.Format($"CLAMP{i + 1}");
                    m_stSystemOpt.bUseClamp[i] = fn_Load("WARMUP", sNo, false, sIniPath);
                }
                for (int i = 0; i < 2; i++)
                {
                    sNo = string.Format($"UTIL{i + 1}");
                    m_stSystemOpt.bUseUtil[i] = fn_Load("WARMUP", sNo, false, sIniPath);
                }
                for (int i = 0; i < 5; i++)
                {
                    sNo = string.Format($"USERSET{i + 1}");
                    m_stSystemOpt.stUserSet[i].bMotion  = fn_Load(sNo, "MOTION" , false, sIniPath);
                    m_stSystemOpt.stUserSet[i].bSetting = fn_Load(sNo, "SETTING", false, sIniPath);
                    m_stSystemOpt.stUserSet[i].bRecipe  = fn_Load(sNo, "RECIPE" , false, sIniPath);
                    m_stSystemOpt.stUserSet[i].bLog     = fn_Load(sNo, "LOG"    , false, sIniPath);
                    m_stSystemOpt.stUserSet[i].bMaster  = fn_Load(sNo, "MASTER" , false, sIniPath);
                    m_stSystemOpt.stUserSet[i].bExit    = fn_Load(sNo, "EXIT"   , false, sIniPath);
                }
                
                //Tool Type
                for (int i = 0; i < MAX_TOOLTYPE; i++)
                {
                    sNo = string.Format($"TOOLTYPE{i + 1}");
                    m_stSystemOpt.sToolType[i] = fn_Load("USERSET", sNo, string.Format($"TYPE{i+1:D2}"), sIniPath);
                }
                
                //Tool Color
                //Pin Color Default
                FM.m_stSystemOpt.brPinColor[0] = Brushes.CadetBlue;
                FM.m_stSystemOpt.brPinColor[1] = Brushes.LightSteelBlue;
                FM.m_stSystemOpt.brPinColor[4] = Brushes.CornflowerBlue;

                string sPinColor = string.Empty;
                var bc = new BrushConverter();

                for (int i = 0; i < MAX_TOOLCOLOR; i++)
                {
                    sNo       = string.Format($"TOOLCOLOR{i + 1}");
                    sPinColor = fn_Load("USERSET", sNo, "", sIniPath);
                    if (sPinColor != string.Empty)
                    {
                        m_stSystemOpt.brPinColor[i] = (SolidColorBrush)bc.ConvertFrom(sPinColor);
                    }
                }

            }
            else
            {

                fn_Save("SYSTEM", "MACHINE_NAME"            , m_stSystemOpt.strMachineName      , sIniPath);
                fn_Save("SYSTEM", "MACHINE_NO"              , m_stSystemOpt.strMachineNo        , sIniPath);
                fn_Save("SYSTEM", "LOG_PATH"                , m_stSystemOpt.strLogPath          , sIniPath);
                                                
                fn_Save("SYSTEM", "RUNMODE"                 , m_stSystemOpt.nRunMode            , sIniPath);
                
                fn_Save("SYSTEM", "POLISHING_MILLING_TIME"  , m_stSystemOpt.nPoliMillingTime    , sIniPath);
                fn_Save("SYSTEM", "CLEANING_MILLING_TIME"   , m_stSystemOpt.nCleanMillingTime   , sIniPath);

                fn_Save("SYSTEM", "SOFT_LIMIT_OFFSET"       , m_stSystemOpt.dSoftLimitOffset    , sIniPath);
                fn_Save("SYSTEM", "WATER_LEVEL_POLISH"      , m_stSystemOpt.nWaterLvlPol        , sIniPath);

                fn_Save("SYSTEM", "TARGET_FORCE"            , m_stSystemOpt.dTargetForce        , sIniPath);
                fn_Save("SYSTEM", "STORAGE_DIRECTION"       , m_stSystemOpt.nStorDir            , sIniPath);

                fn_Save("SYSTEM", "CLEAN_AIR_BLOW"          , m_stSystemOpt.nUseCleanAirBlow    , sIniPath);
                fn_Save("SYSTEM", "LIGHT_AT_RUN"            , m_stSystemOpt.nUseLightOnRun      , sIniPath);
                fn_Save("SYSTEM", "FORCE_CHECK_ALL"         , m_stSystemOpt.nUseAllForceChk     , sIniPath);
                fn_Save("SYSTEM", "USE_POLISH_CUP"          , m_stSystemOpt.nUsePolishingCup    , sIniPath);
                fn_Save("SYSTEM", "USE_Auto_Slurry"         , m_stSystemOpt.nUseAutoSlurry      , sIniPath);
                fn_Save("SYSTEM", "SPINDLE_OFFCUNT"         , m_stSystemOpt.nSpindleOffCnt      , sIniPath);
                fn_Save("SYSTEM", "UseSoftLimit"            , m_stSystemOpt.nUseSoftLimit       , sIniPath);

                fn_Save("SYSTEM", "UseSpdDirBwd"            , m_stSystemOpt.nUseSpdDirBwd       , sIniPath);
                fn_Save("SYSTEM", "UseSpdDirOnlyFWD"        , m_stSystemOpt.nUseSpdDirOnlyFWD   , sIniPath);
                fn_Save("SYSTEM", "UseSpdDirOnlyBWD"        , m_stSystemOpt.nUseSpdDirOnlyBWD   , sIniPath);
                fn_Save("SYSTEM", "UsePoliOneDir"           , m_stSystemOpt.nUsePoliOneDir      , sIniPath);
                fn_Save("SYSTEM", "UseSkipPolAlign"         , m_stSystemOpt.nUseSkipPolAlign    , sIniPath);
                fn_Save("SYSTEM", "UseSkipVisionError"      , m_stSystemOpt.nUseSkipVisError    , sIniPath);

                fn_Save("PMC"   , "IP"                      , m_stSystemOpt.sPMCIp              , sIniPath);
                fn_Save("PMC"   , "Port"                    , m_stSystemOpt.nPMCPort            , sIniPath);

                fn_Save("WARMUP", "USE_WARM"                , m_stSystemOpt.nUseWarming         , sIniPath);
                fn_Save("WARMUP", "INTERVAL"                , m_stSystemOpt.nWarmInterval       , sIniPath);
                fn_Save("WARMUP", "MOTR_REPEAT"             , m_stSystemOpt.nMotrRepeat         , sIniPath);
                fn_Save("WARMUP", "CLAMP_REPEAT"            , m_stSystemOpt.nClampRepeat        , sIniPath);
                fn_Save("WARMUP", "UTIL_REPEAT"             , m_stSystemOpt.nUtilRepeat         , sIniPath);
                fn_Save("WARMUP", "SPLY_TIME"               , m_stSystemOpt.nSplyTime           , sIniPath);

                fn_Save("RFID"  , "IP"                      , m_stSystemOpt.sRFIDIp             , sIniPath);

                //Auto Supply
                fn_Save("SUPPLY", "IP"                      , m_stSystemOpt.sSupplyIp           , sIniPath);
                fn_Save("SUPPLY", "PORT"                    , m_stSystemOpt.nSupplyPort         , sIniPath);
                fn_Save("SUPPLY", "EQPID"                   , m_stSystemOpt.sSupplyEqpId        , sIniPath);
                fn_Save("SUPPLY", "ADDRESS"                 , m_stSystemOpt.nSupplyAddress      , sIniPath);

                fn_Save("SUPPLY", "IP1"                     , m_stSystemOpt.sSupplyIp1          , sIniPath);
                fn_Save("SUPPLY", "PORT1"                   , m_stSystemOpt.nSupplyPort1        , sIniPath);
                fn_Save("SUPPLY", "EQPID1"                  , m_stSystemOpt.sSupplyEqpId1       , sIniPath);
                fn_Save("SUPPLY", "ADDRESS1"                , m_stSystemOpt.nSupplyAddress1     , sIniPath);

                //REST
                fn_Save("REST"  , "URL"                     , m_stSystemOpt.sRestApiUrl         , sIniPath);

                string sNo = string.Empty; 
                for (int i = 0; i < MAX_MOTOR; i++)
                {
                    sNo = string.Format($"MOTOR{i+1}");
                    fn_Save("WARMUP", sNo, m_stSystemOpt.bUseMotor[i], sIniPath);
                }
                for (int i = 0; i < 3; i++)
                {
                    sNo = string.Format($"CLAMP{i + 1}");
                    fn_Save("WARMUP", sNo, m_stSystemOpt.bUseClamp[i], sIniPath);
                }
                for (int i = 0; i < 2; i++)
                {
                    sNo = string.Format($"UTIL{i + 1}");
                    fn_Save("WARMUP", sNo, m_stSystemOpt.bUseUtil[i], sIniPath);
                }
                for (int i = 0; i < 5; i++)
                {
                    sNo = string.Format($"USERSET{i + 1}");
                    //m_stSystemOpt.stUserSet[i].bMotion  = fn_Load(sNo, "MOTION" , false, sIniPath);
                    //m_stSystemOpt.stUserSet[i].bSetting = fn_Load(sNo, "SETTING", false, sIniPath);
                    //m_stSystemOpt.stUserSet[i].bRecipe  = fn_Load(sNo, "RECIPE" , false, sIniPath);
                    //m_stSystemOpt.stUserSet[i].bLog     = fn_Load(sNo, "LOG"    , false, sIniPath);
                    //m_stSystemOpt.stUserSet[i].bMaster  = fn_Load(sNo, "MASTER" , false, sIniPath);
                    //m_stSystemOpt.stUserSet[i].bExit    = fn_Load(sNo, "EXIT"   , false, sIniPath);

                    fn_Save(sNo, "MOTION" , m_stSystemOpt.stUserSet[i].bMotion  , sIniPath);
                    fn_Save(sNo, "SETTING", m_stSystemOpt.stUserSet[i].bSetting , sIniPath);
                    fn_Save(sNo, "RECIPE" , m_stSystemOpt.stUserSet[i].bRecipe  , sIniPath);
                    fn_Save(sNo, "LOG"    , m_stSystemOpt.stUserSet[i].bLog     , sIniPath);
                    fn_Save(sNo, "MASTER" , m_stSystemOpt.stUserSet[i].bMaster  , sIniPath);
                    fn_Save(sNo, "EXIT"   , m_stSystemOpt.stUserSet[i].bExit    , sIniPath);
                }

                //Tool Type
                for (int i = 0; i < MAX_TOOLTYPE; i++)
                {
                    sNo = string.Format($"TOOLTYPE{i + 1}");
                    fn_Save("USERSET", sNo, m_stSystemOpt.sToolType[i], sIniPath);
                }
                
                //Tool Color
                for (int i = 0; i < MAX_TOOLCOLOR; i++)
                {
                    sNo = string.Format($"TOOLCOLOR{i + 1}");
                    fn_Save("USERSET", sNo, m_stSystemOpt.brPinColor[i].ToString(), sIniPath);
                }


            }

        }
        //---------------------------------------------------------------------------
        public void fn_LoadLastInfo(bool bLoad)
        {
            string sIniPath = string.Empty;
            sIniPath = m_strPath + "SYSTEM\\SystemInfo.ini";

	        if (bLoad)
	        {
	        	//Read
	        	m_strCurrJob    = fn_Load("LAST_WORK"   ,"JOB_NAME"   , "", sIniPath);
	        	m_strRecipeName = fn_Load("RECIPE"      ,"RECIPE_NAME", "", sIniPath);


	        	if (m_strCurrJob == "")
	        	{
	        		m_strCurrJob = "Default";
	        		//stProjectBase.sJobName = m_strCurrJob;
	        	}
	        }
	        else
	        {
	        	//Write
	        	fn_Save("LAST_WORK"    ,"JOB_NAME"   , m_strCurrJob   , sIniPath);
	        	fn_Save("RECIPE"       ,"RECIPE_NAME", m_strRecipeName, sIniPath);
	        }


            //Lot Info
            LOT.fn_LoadLotInfo(bLoad);


        }

        //---------------------------------------------------------------------------
        public void fn_LoadRecipeInfo(bool bLoad, string sRecipeName)
        {
            string sIniPath, sAppName;
            if (sRecipeName == "") sRecipeName = "Default";
            sIniPath = m_strPath + "RECIPE\\" + sRecipeName + ".ini";

        	if (bLoad)
        	{
         		//Read
        	    m_stRecipe.nWaferLoadOffset = fn_Load("General","WaferLoadDirection", 0     , sIniPath);
        		m_stRecipe.sRecipeName      = fn_Load("General","ModelName"         , ""    , sIniPath);
        
        		//Polishing Data
        		for (int i = 0 ; i<MAX_POLI; i++)
        		{
        			sAppName = string.Format($"Polishing{i}");
        		    m_stRecipe.stPOLI[i].nCycleCount = fn_Load(sAppName, "NumberofCycle"      , 0      , sIniPath);
        		    m_stRecipe.stPOLI[i].nPathCount  = fn_Load(sAppName, "NumberofPath"       , 0      , sIniPath);
        		    m_stRecipe.stPOLI[i].nForce      = fn_Load(sAppName, "TargetForce"        , 0      , sIniPath);
        		    m_stRecipe.stPOLI[i].nSpindlRPM  = fn_Load(sAppName, "SpindleRPM"         , 0      , sIniPath);
        		    m_stRecipe.stPOLI[i].bUseNewTool = fn_Load(sAppName, "NeedNewTool"        , false  , sIniPath);
        		    m_stRecipe.stPOLI[i].bUseNewUtil = fn_Load(sAppName, "NeedNewUtil"        , false  , sIniPath);
        		    m_stRecipe.stPOLI[i].nUtility    = fn_Load(sAppName, "Util"               , 0      , sIniPath);
        			m_stRecipe.stPOLI[i].dSpeedX     = fn_Load(sAppName, "Speed"              , 0.0    , sIniPath);
        		    m_stRecipe.stPOLI[i].dDisX1      = fn_Load(sAppName, "PolishingXdistance1", 0.0    , sIniPath);
        		    m_stRecipe.stPOLI[i].dDisY1      = fn_Load(sAppName, "PolishingYdistance1", 0.0    , sIniPath);
        		    m_stRecipe.stPOLI[i].dDisX2      = fn_Load(sAppName, "PolishingXdistance2", 0.0    , sIniPath);
        		    m_stRecipe.stPOLI[i].dDisY2      = fn_Load(sAppName, "PolishingYdistance2", 0.0    , sIniPath);
        		    m_stRecipe.stPOLI[i].dTiltAngle  = fn_Load(sAppName, "TiltAngle"          , 0.0    , sIniPath);
        		    m_stRecipe.stPOLI[i].dOffsetX    = fn_Load(sAppName, "OffsetX"            , 0.0    , sIniPath);
        		    m_stRecipe.stPOLI[i].dOffsetY    = fn_Load(sAppName, "OffsetY"            , 0.0    , sIniPath);
        			m_stRecipe.stPOLI[i].nCCW        = fn_Load(sAppName, "SpindleDirection"   ,   0    , sIniPath);
        		}				
        
        		//Cleaning Data
        		for (int i = 0 ; i<MAX_CLEN; i++)
        		{
                    sAppName = string.Format($"Cleaning{i}");
                    m_stRecipe.stCLEN[i].nCycleCount     = fn_Load(sAppName, "NumberofCycle"      , 0    , sIniPath);
        		    m_stRecipe.stCLEN[i].nPathCount      = fn_Load(sAppName, "NumberofPath"       , 0    , sIniPath);
        		    m_stRecipe.stCLEN[i].nForce          = fn_Load(sAppName, "TargetForce"        , 0    , sIniPath);
        		    m_stRecipe.stCLEN[i].nSpindlRPM      = fn_Load(sAppName, "SpindleRPM"         , 0    , sIniPath);
        		    m_stRecipe.stCLEN[i].bUseNewTool     = fn_Load(sAppName, "NeedNewTool"        , false, sIniPath);
        			m_stRecipe.stCLEN[i].dSpeedX         = fn_Load(sAppName, "Speed"              , 0.0  , sIniPath);
        		    m_stRecipe.stCLEN[i].dDisX1          = fn_Load(sAppName, "CleaningXdistance1" , 0.0  , sIniPath);
        		    m_stRecipe.stCLEN[i].dDisY1          = fn_Load(sAppName, "CleaningYdistance1" , 0.0  , sIniPath);
        		    m_stRecipe.stCLEN[i].dDisX2          = fn_Load(sAppName, "CleaningXdistance2" , 0.0  , sIniPath);
        		    m_stRecipe.stCLEN[i].dDisY2          = fn_Load(sAppName, "CleaningYdistance2" , 0.0  , sIniPath);
        		    m_stRecipe.stCLEN[i].dOffsetX        = fn_Load(sAppName, "OffsetX"            , 0.0  , sIniPath);
        		    m_stRecipe.stCLEN[i].dOffsetY        = fn_Load(sAppName, "OffsetY"            , 0.0  , sIniPath);
        			m_stRecipe.stCLEN[i].nWaterSplyTime  = fn_Load(sAppName, "PrewashingRPM"      ,   0  , sIniPath);
        			m_stRecipe.stCLEN[i].nWaterSplyRPM   = fn_Load(sAppName, "PrewashingTime"     ,   0  , sIniPath);
        			m_stRecipe.stCLEN[i].nDeWateringRPM  = fn_Load(sAppName, "DewateringRPM"      ,   0  , sIniPath);
        			m_stRecipe.stCLEN[i].nDeWateringTime = fn_Load(sAppName, "DewateringTimeinSec",   0  , sIniPath);
        		}			
        
        
        	}
        	else
        	{
         		//Write
        		fn_Save("General","WaferLoadDirection", m_stRecipe.nWaferLoadOffset, sIniPath);
        		fn_Save("General","ModelName"         , m_stRecipe.sRecipeName     , sIniPath);
        		
        		for (int i = 0; i < MAX_POLI; i++)
        		{
        			sAppName = string.Format($"Polishing{i}");
        			fn_Save(sAppName, "NumberofCycle"      , m_stRecipe.stPOLI[i].nCycleCount, sIniPath);
        			fn_Save(sAppName, "NumberofPath"       , m_stRecipe.stPOLI[i].nPathCount , sIniPath);
        			fn_Save(sAppName, "TargetForce"        , m_stRecipe.stPOLI[i].nForce     , sIniPath);
        			fn_Save(sAppName, "SpindleRPM"         , m_stRecipe.stPOLI[i].nSpindlRPM , sIniPath);
        			fn_Save(sAppName, "NeedNewTool"        , m_stRecipe.stPOLI[i].bUseNewTool, sIniPath);
        			fn_Save(sAppName, "NeedNewUtil"        , m_stRecipe.stPOLI[i].bUseNewUtil, sIniPath);
        			fn_Save(sAppName, "Util"               , m_stRecipe.stPOLI[i].nUtility   , sIniPath);
        			fn_Save(sAppName, "Speed"              , m_stRecipe.stPOLI[i].dSpeedX    , sIniPath);
        			fn_Save(sAppName, "PolishingXdistance1", m_stRecipe.stPOLI[i].dDisX1     , sIniPath);
        			fn_Save(sAppName, "PolishingYdistance1", m_stRecipe.stPOLI[i].dDisY1     , sIniPath);
        			fn_Save(sAppName, "PolishingXdistance2", m_stRecipe.stPOLI[i].dDisX2     , sIniPath);
        			fn_Save(sAppName, "PolishingYdistance2", m_stRecipe.stPOLI[i].dDisY2     , sIniPath);
        			fn_Save(sAppName, "TiltAngle"          , m_stRecipe.stPOLI[i].dTiltAngle , sIniPath);
        			fn_Save(sAppName, "OffsetX"            , m_stRecipe.stPOLI[i].dOffsetX   , sIniPath);
        			fn_Save(sAppName, "OffsetY"            , m_stRecipe.stPOLI[i].dOffsetY   , sIniPath);
        			fn_Save(sAppName, "SpindleDirection"   , m_stRecipe.stPOLI[i].nCCW       , sIniPath);
        
        		}
        
        		//Cleaning Data
        		for (int i = 0; i < MAX_CLEN; i++)
        		{
                    sAppName = string.Format($"Cleaning{i}");
                    fn_Save(sAppName, "NumberofCycle"      , m_stRecipe.stCLEN[i].nCycleCount    , sIniPath);
        			fn_Save(sAppName, "NumberofPath"       , m_stRecipe.stCLEN[i].nPathCount     , sIniPath);
        			fn_Save(sAppName, "TargetForce"        , m_stRecipe.stCLEN[i].nForce         , sIniPath);
        			fn_Save(sAppName, "SpindleRPM"         , m_stRecipe.stCLEN[i].nSpindlRPM     , sIniPath);
        			fn_Save(sAppName, "NeedNewTool"        , m_stRecipe.stCLEN[i].bUseNewTool    , sIniPath);
        			fn_Save(sAppName, "Speed"              , m_stRecipe.stCLEN[i].dSpeedX        , sIniPath);
        			fn_Save(sAppName, "CleaningXdistance1" , m_stRecipe.stCLEN[i].dDisX1         , sIniPath);
        			fn_Save(sAppName, "CleaningYdistance1" , m_stRecipe.stCLEN[i].dDisY1         , sIniPath);
        			fn_Save(sAppName, "CleaningXdistance2" , m_stRecipe.stCLEN[i].dDisX2         , sIniPath);
        			fn_Save(sAppName, "CleaningYdistance2" , m_stRecipe.stCLEN[i].dDisY2         , sIniPath);
        			fn_Save(sAppName, "OffsetX"            , m_stRecipe.stCLEN[i].dOffsetX       , sIniPath);
        			fn_Save(sAppName, "OffsetY"            , m_stRecipe.stCLEN[i].dOffsetY       , sIniPath);
        			fn_Save(sAppName, "PrewashingRPM"      , m_stRecipe.stCLEN[i].nWaterSplyTime , sIniPath);
        			fn_Save(sAppName, "PrewashingTime"     , m_stRecipe.stCLEN[i].nWaterSplyRPM  , sIniPath);
        			fn_Save(sAppName, "DewateringRPM"      , m_stRecipe.stCLEN[i].nDeWateringRPM , sIniPath);
        			fn_Save(sAppName, "DewateringTimeinSec", m_stRecipe.stCLEN[i].nDeWateringTime, sIniPath);
        		}									  
        
        	}

        }
        //---------------------------------------------------------------------------
        public void fn_LoadVisionResult(bool bLoad, ref ST_VISION_RESULT vs)
        {
            string sTemp    = string.Empty; 
            string sIniPath = string.Empty;
            sIniPath = m_strPath + "SYSTEM\\VisionResult.ini";

            if (bLoad)
            {
                //Read
                vs.nTotalStep   = fn_Load("VS_RESULT", "TotalStep"   , 0, sIniPath);
                
                for (int n =0; n<10; n++)
                {
                    sTemp = string.Format($"LIST_{n:D2}");
                    vs.stRecipeList[n].nUtilType     = fn_Load(sTemp, "UtilType"    , 0  , sIniPath);
                    vs.stRecipeList[n].nUseMilling   = fn_Load(sTemp, "UseMilling"  , 0  , sIniPath);
                    vs.stRecipeList[n].nToolType     = fn_Load(sTemp, "ToolType"    , 0  , sIniPath);
                    vs.stRecipeList[n].nUseToolChg   = fn_Load(sTemp, "UseToolChg"  , 0  , sIniPath);
                    vs.stRecipeList[n].nUseUtilFill  = fn_Load(sTemp, "UseUtilFill" , 0  , sIniPath);
                    vs.stRecipeList[n].nUseUtilDrain = fn_Load(sTemp, "UseUtilDrain", 0  , sIniPath);
                    vs.stRecipeList[n].nUseImage     = fn_Load(sTemp, "UseImage"    , 0  , sIniPath);
                    vs.stRecipeList[n].nUseEPD       = fn_Load(sTemp, "UseEPD"      , 0  , sIniPath);
                    vs.stRecipeList[n].dTilt         = fn_Load(sTemp, "Tilt"        , 0.0, sIniPath);
                    vs.stRecipeList[n].dTheta        = fn_Load(sTemp, "Theta"       , 0.0, sIniPath);
                    vs.stRecipeList[n].dRPM          = fn_Load(sTemp, "RPM"         , 0.0, sIniPath);
                    vs.stRecipeList[n].dForce        = fn_Load(sTemp, "Force"       , 0.0, sIniPath);
                    vs.stRecipeList[n].dSpeed        = fn_Load(sTemp, "Speed"       , 0.0, sIniPath);
                    vs.stRecipeList[n].dPitch        = fn_Load(sTemp, "Pitch"       , 0.0, sIniPath);
                                                                                    
                    vs.stRecipeList[n].pStartPos.X   = fn_Load(sTemp, "StartPos.X"  , 0.0, sIniPath);
                    vs.stRecipeList[n].pStartPos.Y   = fn_Load(sTemp, "StartPos.Y"  , 0.0, sIniPath);
                    vs.stRecipeList[n].pEndPos  .X   = fn_Load(sTemp, "EndPosX"     , 0.0, sIniPath);
                    vs.stRecipeList[n].pEndPos  .Y   = fn_Load(sTemp, "EndPosY"     , 0.0, sIniPath);
                                                                                    
                    vs.stRecipeList[n].dStartX       = fn_Load(sTemp, "StartX"      , 0.0, sIniPath);
                    vs.stRecipeList[n].dEndX         = fn_Load(sTemp, "EndX"        , 0.0, sIniPath);
                    vs.stRecipeList[n].dStartY       = fn_Load(sTemp, "StartY"      , 0.0, sIniPath);
                    vs.stRecipeList[n].dEndY         = fn_Load(sTemp, "EndY"        , 0.0, sIniPath);
                                                                                    
                    vs.stRecipeList[n].dPosTH        = fn_Load(sTemp, "PosTH"       , 0.0, sIniPath);
                    vs.stRecipeList[n].nPathCnt      = fn_Load(sTemp, "Path"        , 0  , sIniPath);
                    vs.stRecipeList[n].nCycle        = fn_Load(sTemp, "Cycle"       , 0  , sIniPath);
                }
            }
            else
            {
                //Write
                fn_Save("VS_RESULT", "TotalStep"   , vs.nTotalStep, sIniPath);
                
                for (int n =0; n<10; n++)
                {
                    sTemp = string.Format($"LIST_{n:D2}");
                    fn_Save(sTemp, "UtilType"    , vs.stRecipeList[n].nUtilType      , sIniPath);
                    fn_Save(sTemp, "UseMilling"  , vs.stRecipeList[n].nUseMilling    , sIniPath);
                    fn_Save(sTemp, "ToolType"    , vs.stRecipeList[n].nToolType      , sIniPath);
                    fn_Save(sTemp, "UseToolChg"  , vs.stRecipeList[n].nUseToolChg    , sIniPath);
                    fn_Save(sTemp, "UseUtilFill" , vs.stRecipeList[n].nUseUtilFill   , sIniPath);
                    fn_Save(sTemp, "UseUtilDrain", vs.stRecipeList[n].nUseUtilDrain  , sIniPath);
                    fn_Save(sTemp, "UseImage"    , vs.stRecipeList[n].nUseImage      , sIniPath);
                    fn_Save(sTemp, "UseEPD"      , vs.stRecipeList[n].nUseEPD        , sIniPath);
                    fn_Save(sTemp, "Tilt"        , vs.stRecipeList[n].dTilt          , sIniPath);
                    fn_Save(sTemp, "Theta"       , vs.stRecipeList[n].dTheta         , sIniPath);
                    fn_Save(sTemp, "RPM"         , vs.stRecipeList[n].dRPM           , sIniPath);
                    fn_Save(sTemp, "Force"       , vs.stRecipeList[n].dForce         , sIniPath);
                    fn_Save(sTemp, "Speed"       , vs.stRecipeList[n].dSpeed         , sIniPath);
                    fn_Save(sTemp, "Pitch"       , vs.stRecipeList[n].dPitch         , sIniPath);
                                                                                     
                    fn_Save(sTemp, "StartPos.X"  , vs.stRecipeList[n].pStartPos.X    , sIniPath);
                    fn_Save(sTemp, "StartPos.Y"  , vs.stRecipeList[n].pStartPos.Y    , sIniPath);
                    fn_Save(sTemp, "EndPos.X"    , vs.stRecipeList[n].pEndPos  .X    , sIniPath);
                    fn_Save(sTemp, "EndPos.Y"    , vs.stRecipeList[n].pEndPos  .Y    , sIniPath);
                                                                                     
                    fn_Save(sTemp, "StartX"      , vs.stRecipeList[n].dStartX        , sIniPath);
                    fn_Save(sTemp, "EndX"        , vs.stRecipeList[n].dEndX          , sIniPath);
                    fn_Save(sTemp, "StartY"      , vs.stRecipeList[n].dStartY        , sIniPath);
                    fn_Save(sTemp, "EndY"        , vs.stRecipeList[n].dEndY          , sIniPath);
                                                                                     
                    fn_Save(sTemp, "PosTH"       , vs.stRecipeList[n].dPosTH         , sIniPath);
                                                                                     
                    fn_Save(sTemp, "Path"        , vs.stRecipeList[n].nPathCnt       , sIniPath);
                    fn_Save(sTemp, "Cycle"       , vs.stRecipeList[n].nCycle         , sIniPath);

                }
            }
        }
        //---------------------------------------------------------------------------
        public void fn_LoadRecipeInfoBuff(bool bLoad, string sRecipeName)
        {

        }
        //---------------------------------------------------------------------------
        //Version
        public void fn_SetVersion()
        {
            //Local var
            int n = 0;

            //
            sHistory[n++] = "[DATE  ][요청자][담당자][LIST      ]";
//             sHistory[n++] = "[200420][정지완][정지완] 1.8 PGM START";
//             sHistory[n++] = "[200506][정지완][정지완] Polishing End 시 DI / Suck back 추가";
//             sHistory[n++] = "[200506][정지완][정지완] 0.1 mm/1.0 이동 Function";
//             sHistory[n++] = "[200508][정지완][정지완] 1차 완료";
//             sHistory[n++] = "[200510][정지완][정지완] Dry Run Ok";
//             sHistory[n++] = "[200512][정지완][정지완] Cleaning Align Position Save";
//             sHistory[n++] = "[200513][정지완][정지완] Storage Map Form 추가";
//             sHistory[n++] = "[200513][정지완][정지완] Home Offset 추가(ACS Buffer 수정)";
//             sHistory[n++] = "[200514][정지완][정지완] 1차완료";
//             sHistory[n++] = "[200519][정지완][정지완] Load/Unload시 Loading T-Axis Zero Position";
//             sHistory[n++] = "[200519][정지완][정지완] Spindle Z-Axis Home Offset 적용";
//             sHistory[n++] = "[200519][정지완][정지완] Tool Name 저장";
//             sHistory[n++] = "[200519][정지완][정지완] EMS 동작 시 Valve/Pump OFF (ACS 적용)";
//             sHistory[n++] = "[200519][정지완][정지완] Motor Min/Max Data Save(Master.ini)";
//             sHistory[n++] = "[200523][정지완][정지완] Transfer Exist Sensor/ Load Exist Sensor 추가";
//             sHistory[n++] = "[200523][정지완][정지완] Load Pre-Align Position 추가";
//             sHistory[n++] = "[200523][정지완][정지완] Force Default Set Option";
//             sHistory[n++] = "[200525][정지완][정지완] CSV File Save 추가";
//             sHistory[n++] = "[200525][이소연][정지완] Magazine Empty Alarm 삭제 -> Message변경";
//             sHistory[n++] = "[200526][정지완][정지완] Cup pick/place one cycle 추가";
//             sHistory[n++] = "[200527][이종문][정지완] Spindle Tool 있는 경우 home 동작시 Message";
//             sHistory[n++] = "[200527][고정진][정지완] Cleaning Position Setting Method 변경(Center 기준으로 변경)";
//             sHistory[n++] = "[200527][정지완][정지완] Polishing Cup 사용 수정";
//             sHistory[n++] = "[200528][정지완][정지완] Load Plate Sensor 감지 조건 추가";
//             sHistory[n++] = "[200528][정지완][정지완] Plate Pick/Place Position 분리";
//             sHistory[n++] = "[200529][정지완][정지완] Soft Limit Option 추가";
//             sHistory[n++] = "[200601][정지완][정지완] Warming up 기능 추가";
//             sHistory[n++] = "[200601][정지완][정지완] Polishing Cycle Direction 수정";
//             sHistory[n++] = "[200601][정지완][정지완] Cleaning RPM 및 Time으로 동작하게 수정";
//             sHistory[n++] = "[200602][정지완][정지완] Map에서 magazine form show 수정";
//             sHistory[n++] = "[200602][정지완][정지완] Soft Limit 추가 및 Alarm 추가";
//             sHistory[n++] = "[200602][정지완][정지완] Valve On Time - 3sec 제한";
//             sHistory[n++] = "[200603][정지완][정지완] LOT log 수정";
//             sHistory[n++] = "[200603][정지완][정지완] Cleaning Time 적용";
//             sHistory[n++] = "[200604][정지완][정지완] Run/PM/Down Time 추가";
//             sHistory[n++] = "[200605][정지완][정지완] vision position 이동 button 추가";
//             sHistory[n++] = "[200605][정지완][정지완] Reset click 시 Jog,Message window close 추가";
//             sHistory[n++] = "[200605][정지완][정지완] Cylinder 명칭 변경";
//             sHistory[n++] = "[200609][정지완][정지완] Polishing only one direction 추가";
//             sHistory[n++] = "[200610][정지완][정지완] Milling State Log 추가";
//             sHistory[n++] = "[200610][정지완][정지완] DCOM Value Setting / Option 추가";
//             sHistory[n++] = "[200610][정지완][정지완] Start 시 Cup 정위치 Check 추가 / Force Lot End 추가";
//             sHistory[n++] = "[200613][정지완][정지완] Polishing/Clean bath에서 Plate Pick 동작 시 unclamp 수정";
//             sHistory[n++] = "[200615][정지완][정지완] Recipe List-DIWater 추가/";
//             sHistory[n++] = "[200617][정지완][정지완] Home End Done 추가(home 동작 순서)";
//             sHistory[n++] = "[200617][정지완][정지완] Password init 추가";
//             sHistory[n++] = "[200617][정지완][정지완] RFID Read 추가";
//             sHistory[n++] = "[200626][정지완][정지완] Tool Color 추가";
//             sHistory[n++] = "[200703][정지완][정지완] Vision Camera Offset 추가";
//             sHistory[n++] = "[200703][정지완][정지완] Default Jerk Setting 추가";
//             sHistory[n++] = "[200704][정지완][정지완] Futek S/N Auto Reading 추가";
//             sHistory[n++] = "[200708][정지완][정지완] Force Check Cycle Load Cell Set Zero 추가";

            //
            sHistory[n++] = "[200713][정지완][정지완] Leak Sensor Alarm Read from ACS";
            sHistory[n++] = "[200713][정지완][정지완] Current State Log 추가";
            sHistory[n++] = "[200713][정지완][정지완] EMO 시 Cycle Out 추가";
            sHistory[n++] = "[200715][정지완][정지완] Top Load cell value Average 추가";
            sHistory[n++] = "[200716][정지완][정지완] Use Force Data of recipe while force check";
            sHistory[n++] = "[200717][정지완][정지완] Cleaning 동작 시 Position 이동으로 변경 - 검증 필요";
            sHistory[n++] = "[200720][정지완][정지완] REST API 추가";
            sHistory[n++] = "[200721][정지완][정지완] Vision PGM Merge";
            sHistory[n++] = "[200722][정지완][정지완] REST Manual 추가";
            sHistory[n++] = "[200723][정지완][정지완] Position Min/Max 수정 추가 / ACS Home Offset Clear 추가";
            sHistory[n++] = "[200804][정지완][정지완] Home 동작시 Message 추가";
            sHistory[n++] = "[200804][정지완][정지완] BIN 경로 변경";
            sHistory[n++] = "[200805][정지완][정지완] RFID,REST Thread 변경";
            sHistory[n++] = "[200806][정지완][정지완] RFID Display 변경";
            sHistory[n++] = "[200807][정지완][정지완] Bin Path Change";
            sHistory[n++] = "[200807][정지완][정지완] Water Leak Address change / ACS Valve Close Warning Add";
            sHistory[n++] = "[200807][고정진][정지완] JOG 동작 시 stop 삭제";
            sHistory[n++] = "[200807][정지완][정지완] Motion Cylinder State display";
            sHistory[n++] = "[200807][정지완][정지완] Spindle Cup Place 동작 시 X-Axis 우선 이동 수정";
            sHistory[n++] = "[200811][정지완][정지완] Auto Slurry 사용 시 Pump 사용 X";
            sHistory[n++] = "[200811][정지완][정지완] Auto Slurry Sequence, Error, Option 추가";
            sHistory[n++] = "[200813][정지완][정지완] Auto Soap Add";
            sHistory[n++] = "[200813][정지완][정지완] REST API Option Add";
            sHistory[n++] = "[200818][정지완][정지완] REST Wafer Info Display 수정";
            sHistory[n++] = "[200818][정지완][정지완] RUN 중 INIT Display 삭제";
            sHistory[n++] = "[200819][정지완][정지완] Tool Discard Position 추가-Cleaning Tool 위치 +30";
            sHistory[n++] = "[200819][정지완][정지완] Master Motor 수정";
            sHistory[n++] = "[200819][정지완][정지완] Top Load cell Offset 추가";
            sHistory[n++] = "[200820][정지완][정지완] UserMessage Show 상태 시 Hide 추가";
            sHistory[n++] = "[200821][정지완][정지완] Polishing Count +1 수정/REST API 검증완료";
            sHistory[n++] = "[200824][정지완][정지완] Polishing Force Data Recipe/System option 처리 (AUTO 상태에따라)";
            sHistory[n++] = "[200824][정지완][정지완] Lot Open 시 Polishing Count/Cleaning Count Reset";
            sHistory[n++] = "[200824][정지완][정지완] Magazine Plate Pick 동작 시 Polishing Drain 추가";
            sHistory[n++] = "[200825][정지완][정지완] Force Test Manual 추가";
            sHistory[n++] = "[200826][정지완][정지완] Force Check 시 Storage Y-axis 위치 이동";
            sHistory[n++] = "[200827][정지완][정지완] TimeOut 수정/Bottom Load cell offset 삭제";
            sHistory[n++] = "[200901][정지완][정지완] Force Check Time 7 수정";
            sHistory[n++] = "[200901][정지완][정지완] Top Load cell Display 수정 / Alarm 시 Home Flag 삭제";
            sHistory[n++] = "[200903][정지완][정지완] Analog Name, Display Save 추가";
            sHistory[n++] = "[200907][정지완][정지완] Drain Sequence 수정";
            sHistory[n++] = "[200907][정지완][정지완] Plate Display Item 수정 / Alarm Log 수정";
            sHistory[n++] = "[200908][정지완][정지완] Auto Calibration 추가";
            sHistory[n++] = "[200909][신수진][정지완] Cycle/Step Count Display 수정";
            sHistory[n++] = "[200910][정지완][정지완] HOME Sensor Display 추가";
            sHistory[n++] = "[200911][권혁상][정지완] Drain Pump IO 삭제";
            sHistory[n++] = "[200914][선경규][정지완] Vision Skip Option 추가";
            sHistory[n++] = "[200914][이종문][정지완] Spindle Offset 추가";
            sHistory[n++] = "[200914][정지완][정지완] Auto Calibration Seq 수정";
            sHistory[n++] = "[200915][정지완][정지완] Cleaning Position 수정";
            sHistory[n++] = "[200915][정지완][정지완] ACS Reboot TEST";
            sHistory[n++] = "[200915][이종문][정지완] Polishing Cup Place Position 추가";
            sHistory[n++] = "[200916][정지완][정지완] Polishing/Cleaning Time Out 수정";
            sHistory[n++] = "[200916][정지완][정지완] Drain Sequence 원복";
            sHistory[n++] = "[200919][정지완][정지완] modify Vision Position Check/Tool Check ";
            sHistory[n++] = "[200921][신수진][정지완] Recipe All Clear 추가";
            sHistory[n++] = "[200922][선경규][정지완] Vision Retry 추가";
            sHistory[n++] = "[200922][신수진][정지완] 작업종료 시 Magazine Wait Pos 이동";
            sHistory[n++] = "[200929][신수진][이준호] Run 동작 전 Storage Lock Check 후 Lock 동작 수행, Vision Error Option"; //200929
            sHistory[n++] = "[201006][이소연][선경규] EPD Test";
            sHistory[n++] = "[201006][선경규][선경규] Recipe ROI 개선, Mark-Milling ROI Lock, Main Vision ViewUpdate";
            sHistory[n++] = "[201007][정지완][정지완] Load Port Vision Error 시 Map 수정";
            sHistory[n++] = "[201007][정지완][정지완] Run 중 Recipe Confirm Button 미사용 수정";
            sHistory[n++] = "[201008][정지완][정지완] Vision Retry Count Display 추가";
            sHistory[n++] = "[201008][정지완][정지완] Fan Alarm Time 5sec 변경";
            sHistory[n++] = "[201008][정지완][정지완] Force 측정 후 DCOM Value Log 추가";
            sHistory[n++] = "[201008][정지완][정지완] Top Load cell Data Log와 Display 일치";
            sHistory[n++] = "[201012][정지완][정지완] Actuator Retry 적용";
            sHistory[n++] = "[201013][정지완][정지완] DCOM Value Lot Log 추가/Lot Log에 Top Low Data 추가";
            sHistory[n++] = "[201014][정지완][정지완] Load-cell Open Retry 추가 / close try catch 적용";
            sHistory[n++] = "[201015][신수진][정지완] Soft Limit Error 발생 시 Map Finish 변경";
            sHistory[n++] = "[201015][정지완][정지완] Soft Limit 발생 시 Error 발생 부분 수정";
            sHistory[n++] = "[201015][정지완][정지완] Cleaning Time Error 발생 시 Home 동작 추가";
            sHistory[n++] = "[201015][정지완][정지완] Polishing Cup 원위치 이동 Sequence 추가";
            sHistory[n++] = "[201016][신수진][선경규] Main 화면이 아닐 때, Main Vision 영상 업데이트 수정.";
            sHistory[n++] = "[201019][이소연][선경규] EPD Ref Distance 파라메터 double 형으로 Update.";
            sHistory[n++] = "[201020][이소연][선경규] EPD Edit 화면에 Mark 표기.";
            sHistory[n++] = "[201020][이소연][선경규] EPD Run 중 Align Offset 적용.";
            sHistory[n++] = "[201021][정지완][정지완] Soft Limit 후 STOP 추가";
            sHistory[n++] = "[201022][정지완][정지완] Home 동작 후 Wait 위치 이동 추가";
            sHistory[n++] = "[201022][선경규][선경규] Loading Setting Page UI 정리.";
            sHistory[n++] = "[201026][이소연][정지완] Force Test Cycle 수정: 일정DCOM(3.0)으로 Force 동작만.";
            sHistory[n++] = "[201026][선경규][선경규] Milling ROI 표기 방식 수정.";
            sHistory[n++] = "[201026][정지완][정지완] Force Buffer Error Check 추가 / ACS Force Buffer 수정";
            sHistory[n++] = "[201026][정지완][정지완] Vision Manual 동작 수정";
            sHistory[n++] = "[201027][선경규][선경규] Camera Live 시 기타 동작 비활성화.";
            sHistory[n++] = "[201028][정지완][정지완] Force Check Error 삭제(임시)";
            sHistory[n++] = "[201029][이소연][선경규] Search Lib - Scale 범위 추가.(0.9~1.1)(테스트)";
            sHistory[n++] = "[201029][선경규][선경규] Search Lib - Angle 범위 추가.(±45)(테스트)";
            sHistory[n++] = "[201029][선경규][선경규] Recipe UtilFill & Milling Enable 종속성 해제.";
			sHistory[n++] = "[201103][선경규][선경규] Align Angle Range & Scale Range Parameter 추가.";
            sHistory[n++] = "[201103][선경규][선경규] PreAlign Rectangle Control 추가.";
            sHistory[n++] = "[201103][선경규][선경규] Vision Log - Align Result From Lib 항목 추가.";
            sHistory[n++] = "[201103][정지완][정지완] MUTE 추가";
            sHistory[n++] = "[201103][신수진][정지완] Vision Error 배출 시 Map Color 변경";
            sHistory[n++] = "[201103][이소연][정지완] Force Check Cycle 변경";
            sHistory[n++] = "[201104][정지완][정지완] Sequence Message 추가";
            sHistory[n++] = "[201106][조민철][선경규] EPD Error, Mark 이미지 버그 수정.";
            sHistory[n++] = "[201109][신수진][정지완] Vision Log 추가(Searched Mark No)";
            sHistory[n++] = "[201110][선경규][선경규] Image Save 할 때 Grab 실패시 저장 안함.";
            sHistory[n++] = "[201110][선경규][선경규] Recipe Copy Image Path 버그 수정.";
            sHistory[n++] = "[201112][선경규][선경규] Recipe Milling Rectangle 수정.";
            sHistory[n++] = "[201113][정지완][정지완] Milling % 추가, Warming 수정";
            sHistory[n++] = "[201117][정지완][정지완] Vision Result Cycle Count Save 추가";
            sHistory[n++] = "[201118][정지완][정지완] Milling Image Save 동작 시 Inspection(EPD) 조명 값 사용 수정 ";
            sHistory[n++] = "[201118][정지완][정지완] Magazine 에서 Plate Pick 시 Tool Empty Check 추가";
            sHistory[n++] = "[201119][정지완][정지완] Auto Supply System Reply Error 추가";
            sHistory[n++] = "[201119][정지완][정지완] Force Check Time 100sec -> 200sec 증가";
            sHistory[n++] = "[201119][정지완][정지완] Top force Graph Reset 추가";
            sHistory[n++] = "[201120][정지완][정지완] Tool Pick 시 Stop Error 수정";
            sHistory[n++] = "[201120][신수진][선경규] Recipe 목록 작업 시 Recipe UnLoad 추가.";
            sHistory[n++] = "[201123][선경규][선경규] Recipe 상에서 One Shot, Live 시 조명 재설정 코드 추가.";
            sHistory[n++] = "[201123][선경규][선경규] 조명 및 카메라 Parameter Step Gap 조정.";
            sHistory[n++] = "[201125][선경규][선경규] Recipe Model Compare 항목 업데이트.";
            sHistory[n++] = "[201126][이준호][선경규] 조명이 Load 된 Recipe 로 동작하는 버그 수정.";
            sHistory[n++] = "[201127][이준호][이준호] Position 변경 시 Trace Log 추가, Log 스크롤 하단으로 변경";
            sHistory[n++] = "[201130][정지완][정지완] Vision Error Map 수정";
            sHistory[n++] = "[201130][이소연][선경규] Vision IR Shutter Option 추가.";
            sHistory[n++] = "[201202][선경규][선경규] IR Filter Bug 수정.";
            sHistory[n++] = "[201202][신수진][이준호] Tool Change Option 사용 시 Cup 사용 유지되도록 변경";
            sHistory[n++] = "[201208][강기훈][선경규] Vision ROI Lock.";
            sHistory[n++] = "[201208][강기훈][선경규] Polishing Recipe Vision ROI 기준 위치 변경.(Left Top -> Center)";
            sHistory[n++] = "[201210][정지완][정지완] Tool Auto Calibration 관련 추가";
            sHistory[n++] = "[201215][선경규][선경규] Tool Auto Cal 계산 오류 수정.";
            sHistory[n++] = "[201221][선경규][선경규] Tool Auto Cal 계산 로직 수정.";
            sHistory[n++] = "[201221][정지완][정지완] Start 시 Map Save 추가";
            sHistory[n++] = "[201222][정지완][정지완] Tool Auto Calibration 후 Data Save / Log 추가 ";
            sHistory[n++] = "[201222][정지완][정지완] TEST Log File 별로 사용하도록 수정";
            sHistory[n++] = "[201223][선경규][선경규] Tool Align Offset 적용.";
            sHistory[n++] = "[201223][정지완][정지완] Spindle Z-Axis DSTB Align Option 적용";
            sHistory[n++] = "[201228][선경규][선경규] DLL Bug Fix.";
			sHistory[n++] = "[210112][선경규][선경규] Optic 조건 분리, Model Scale, Angle Default 작업, Under Score 작업 적용.";
            sHistory[n++] = "[210113][정지완][정지완] Warming up 조건 추가 / Operation 화면 수정";
            sHistory[n++] = "[210114][정지완][정지완] Warming up 시 Transfer Bwd 추가 / DP Error 추가";
            
            sHistory[n++] = "[210115][선경규][선경규] EPD Invalid Index 예외 처리 추가.";
            sHistory[n++] = "[210115][정지완][정지완] Vision Inspection 전 Wait 위치 추가";

            sHistory[n++] = "[210118][선경규][선경규] Recipe Bug 수정.";
            sHistory[n++] = "[210118][선경규][선경규] Main Result 수정.";
            sHistory[n++] = "[210119][정지완][정지완] Tool Exist Error 추가";
            sHistory[n++] = "[210119][정지완][정지완] Error Solution/Cause Save 시 Enter 추가";
            sHistory[n++] = "[210119][정지완][정지완] 현장 적용";

            sHistory[n++] = "[210119][선경규][선경규] DLL 객체 해제 버그 수정.";
            sHistory[n++] = "[210119][선경규][선경규] Recipe 이미지 Open 수정.";

            sHistory[n++] = "[210127][정지완][정지완] Vision Result Clear 시점 변경";
            sHistory[n++] = "[210127][정지완][정지완] Auto Calibration Start DCOM Option 처리";
            sHistory[n++] = "[210127][정지완][정지완] Vision Enable Fail Error 처리 및 Alarm 추가";
            sHistory[n++] = "[210128][정지완][정지완] Pre-Align Retry 수정";

            sHistory[n++] = "[210203][정지완][정지완] Alarm Sub Page 추가";


            //
            m_nHisCnt = n;
            //
            m_sVersion = "VER : 1.0.00.210203_15H";
        }


    }
}


/* 1) Critical Error 발생 시 Cycle out(ex. IO/Motor Connection) ??
 * 2) 
 * 3) 
 * 4) 
 * 5) 
 * 
 * 
 * 
 * 
 * > Warming up 기능 Check - Magazine X && Door Close 시
 * > Polishing End Cycle에서 DCOM Value 단계적으로 줄이기? (XSEG value 사용) --> ACS Buffer에서...
 * > ACS 연결 끊김시 처리?
 * > 
 */
