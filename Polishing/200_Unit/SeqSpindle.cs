using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaferPolishingSystem;
using WaferPolishingSystem.BaseUnit;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.BaseUnit.Magazine;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.BaseUnit.ERRID;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using static WaferPolishingSystem.BaseUnit.ActuatorId;
using static WaferPolishingSystem.BaseUnit.IOMap;
using System.IO;
using static WaferPolishingSystem.Define.UserEnumVision;


namespace WaferPolishingSystem.Unit
{
    public class SeqSpindle
    {
		//Timer
		TOnDelayTimer m_tMainCycle      = new TOnDelayTimer();
		TOnDelayTimer m_tToStart        = new TOnDelayTimer();
		TOnDelayTimer m_tToStop         = new TOnDelayTimer();

		TOnDelayTimer m_tToolPickCycle  = new TOnDelayTimer();
		TOnDelayTimer m_tToolChkCycle   = new TOnDelayTimer();
		TOnDelayTimer m_tForceChkCycle  = new TOnDelayTimer();
		TOnDelayTimer m_tPolishCycle    = new TOnDelayTimer();
		TOnDelayTimer m_tCleanCycle     = new TOnDelayTimer();
		TOnDelayTimer m_tToolPlaceCycle = new TOnDelayTimer();
		TOnDelayTimer m_tVisnInspCycle  = new TOnDelayTimer();
		TOnDelayTimer m_tUtilChkCycle   = new TOnDelayTimer();
		TOnDelayTimer m_tMagzPickCycle  = new TOnDelayTimer();
		TOnDelayTimer m_tMagzPlaceCycle = new TOnDelayTimer();
		TOnDelayTimer m_tCupInPosCycle  = new TOnDelayTimer();

		TOnDelayTimer m_tHome           = new TOnDelayTimer();
		TOnDelayTimer m_tDelayTime      = new TOnDelayTimer();
		

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Internal Vars.
		bool          m_bToStart        ; //To... Flag.
		bool          m_bToStop         ;
		bool          m_bWorkEnd        ;
								    
		bool          m_bDrngToolPick   ; //Tool Pick
		bool          m_bDrngToolCheck  ; //Tool Exist Check
		bool          m_bDrngForceChk   ; //Force Check
		bool          m_bDrngPolishing  ; //Polishing
		bool          m_bDrngCleaning   ; //Cleaning 
		bool          m_bDrngToolPlce   ; //Tool Discard
		bool          m_bDrngVisnInspA  ; //vision inspect
		bool          m_bDrngVisnInspL  ; //vision inspect : PreAlign
		bool          m_bDrngUtilLevel  ; //수위 Level Check 
		bool          m_bDrngPlatePick  ; //Magazine Pick
		bool          m_bDrngPlatePlce  ; //Magazine Place
		bool          m_bDrngCupInPos   ; //Move Cup In Position
		bool          m_bDrngPlatePickP ; //Magazine Pick at Polishing
		bool          m_bDrngPlatePickC ; //Magazine Pick at Cleaning
		bool          m_bDrngPlatePlceP ; //Magazine Place at Polishing
		bool          m_bDrngPlatePlceC ; //Magazine Place at Cleaning
		bool          m_bDrngWait       ; //Wait 

		bool          m_bReqUtil_Polish ; //Request Move Polish Utility Check Position
	  //bool          m_bReqUtil_Clean  ; //Request Move Cleaning Utility Check Position

		bool          m_bTESTMODE       ; //TEST Mode Flag
		bool          m_bForceStop      ;
		bool          m_bOffDone        ;
		bool          m_bDCOMReset      ;
		bool          m_bReqResetGraph  ; //Main Form Graph Reset 

		int           m_nSeqStep        ; //Step.
		int           m_nManStep        ;
		int           m_nHomeStep       ;
	
		int           m_nToolPickStep   ; //Pick Step
		int           m_nToolChkStep    ; //Tool Check Step
		int           m_nForceChkStep   ; //Tool Check Step
		int           m_nPolishStep     ; //Polish Step
		int           m_nCleanStep      ; //Clean Step
		int           m_nToolPlaceStep  ; //Place Step
		int           m_nVisnInspStep   ; //Vision Inspect Step
		int           m_nUtilChkStep    ; //Utility Level Check Step
		int           m_nMagzPickStep   ; 
		int           m_nMagzPlaceStep  ; 
		int           m_nPoliBackStep   ; //Back Step
		int           m_nAutoCalStep    ; //Auto Calibration Step
		int           m_nVisnBackStep   ;
		int           m_nCylRetryCnt    ;
		int           m_nCupMoveStep    ;

		int           m_nCalCount       ; //Calibration Count
		int           m_nCalCycle       ; //Cycle Count
		int           m_nTotalCalCycle  ;
		int           m_nTopForceValue  ; //Monitoring Load Cell Value
		int           m_nBtmForceValue  ; //Calibration Load Cell Value
		int           m_nForceOffset    ; //m_nBtmForceValue-m_nTopForceValue
		double        m_dForceRatio     ; //Ration Value of Top Load Cell 
		int           m_nToolClampRetry ;
		int           m_nTestIndex      ;
		int           m_nPoliCnt        ; //Polishing Step No.
		int			  m_nCleanCnt       ; //Cleaning Step No.
		int           m_nPolCycle       ; //Polishing Cycle No.
		int           m_nClnCycle       ; //Cleaning Cycle No.
		int           m_nTotalCycle     ;
		int           m_nWaterClenCnt   ; //Polishing 후에 Cleaning 동작 횟수 Count
		int           m_nVisnRetry      ; //Vision retry count
		int           m_nPreCycleNo     ; //Log 시 이전 Cycle No Check


		int           m_iPartId         ;
		EN_MOTR_ID    m_iMotrZId        ;
		EN_MOTR_ID    m_iMotrZ1Id       ;
		EN_MOTR_ID    m_iMotrXId        ;
		EN_MOTR_ID    m_iBtmMotr        ;
		

		double        m_dDirectPosn     ; //Direct Moving Position.
		double        m_dMPos           ; //Command Motor Position
		double        m_dEPos           ; //Enc. Motor Position

		double        m_dDisX_PCtoPS    ; //X-Position of Polishing Center to Polishing Start
		double        m_dDisY_PCtoPS	; //Y-Position of Polishing Center to Polishing Start
		double        m_dCleanOffsetX   ; //Cleaning Start Offset - X
		double        m_dCleanOffsetY   ; //Cleaning Start Offset - Y

        int           nTestCnt          ;
        double        m_dTestDCOM       ;

		int           m_nCalTotalCnt    ;
		int			  m_nTotalPathCnt   ; // Total Milling Path Count


		//double		  m_dCycleTimeStart ; // One Cycle Time Start
		//double		  m_dCycleTimeEnd   ; // One Cycle Time Start

		string        sTemp         = string.Empty;
		string        sLogMsg       = string.Empty; //for Log
		string        m_sLogMoveEvt = string.Empty; 
		string        m_sSeqMsg     = string.Empty; 
			          
		bool          m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        int           m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;
        double        m_dSpare1, m_dSpare2, m_dSpare3, m_dSpare4, m_dSpare5;

		TOOL_PICK_INFO  ToolPickInfo  = new TOOL_PICK_INFO (false);
		PLATE_MOVE_INFO PlatePickInfo = new PLATE_MOVE_INFO(false);
		PLATE_MOVE_INFO PlatePlceInfo = new PLATE_MOVE_INFO(false);
		PLATE_MOVE_INFO PlateMoveInfo = new PLATE_MOVE_INFO(false);
		VISN_INSP_INFO  VisnInspInfo  = new VISN_INSP_INFO (false);


		EN_COMD_ID m_iCmdX ;
		EN_COMD_ID m_iCmdY ;
		EN_COMD_ID m_iCmdZ ;
		EN_COMD_ID m_iCmdZ1;

		public  ST_VISION_RESULT vresult     ;
		private ST_CLEAN_RECIPE  m_stCleanRcp; 

		DateTime m_tStartPolishing;
		DateTime m_tEndPolishing  ;

		EN_PLATE_ID enPlateId = EN_PLATE_ID.ptiNone;

		string m_sStartTime, m_sEndTime;
		string m_sMillStartTime, m_sMillEndTime;
		string m_sCleanStartTime, m_sCleanEndTime;

		public ST_MILL enMillINFO = new ST_MILL(0);

		ST_VISION_RESULT stOneCycleResult = new ST_VISION_RESULT(0);
		ST_VISION_RESULT stvisionResult   = new ST_VISION_RESULT(0);

		//
		//Label[,] Items  = new Label[50, 2];
		//Label[,] Items1 = new Label[50, 2];

		double[,] m_dCalVal = new double[20, 3];
		Point[] m_pTopBtm  = new Point[20];
		Point[] m_pBtmDCOM = new Point[20];

		Point m_pLMSTopBtm  = new Point();
		Point m_pLMSBtmDCOM = new Point();

		//
		int    m_nAlingRetry, m_nBackStep;
		Point  m_pMotrMov = new Point();
		double m_dAlignOffsetX, m_dAlignOffsetY;
		int    m_nAlignTestCount, m_nTestCount;

		//---------------------------------------------------------------------------
		//Property
		public int _nManStep          { get { return m_nManStep;       } set { m_nManStep       = value; } }
		public int _nSeqStep          { get { return m_nSeqStep;       } }
								      
		public int _nTopForceValue    { get { return m_nTopForceValue ; } set { m_nTopForceValue = value; } }
		public int _nBtmForceValue    { get { return m_nBtmForceValue ; } set { m_nBtmForceValue = value; } }
		public int _nForceOffset      { get { return m_nForceOffset   ; } set { m_nForceOffset   = value; } }

		public double _dForceRatio    { get { return m_dForceRatio    ; } set { m_dForceRatio = value; } }
		public double _dDisX_PCtoPS   { get { return m_dDisX_PCtoPS   ; } }
        public double _dDisY_PCtoPS   { get { return m_dDisY_PCtoPS   ; } }
        public double _dCleanOffsetX  { get { return m_dCleanOffsetX  ; } }
        public double _dCleanOffsetY  { get { return m_dCleanOffsetY  ; } }
		public double _dTestDCOM      { get { return m_dTestDCOM      ; } set { m_dTestDCOM = value; } }
		

		public int _nHomeStep         { get { return m_nHomeStep     ; } set { m_nHomeStep = value; } }
		public int _nPoliCnt          { get { return m_nPoliCnt      ; }}
		public int _nCleanCnt         { get { return m_nCleanCnt     ; }}
		public int _nPolCycle         { get { return m_nPolCycle     ; }}
        public int _nClnCycle         { get { return m_nClnCycle     ; }}
        public int _nTotalCycle       { get { return m_nTotalCycle   ; }}
		public int _nVisnInspStep     { get { return m_nVisnInspStep ; }}

		public int _nAutoCalCount     { get { return m_nCalCount     ; }}
		public int _nAutoCalTotalCnt  { get { return m_nCalTotalCnt  ; }}
		public int _nTotalCalCycle    { get { return m_nTotalCalCycle; } set { m_nTotalCalCycle = value; } }
        public int _nCalCycle         { get { return m_nCalCycle     ; } }
		public int _nVisnRetry        { get { return m_nVisnRetry    ; } }
		public int _nTotalPathCnt     { get { return m_nTotalPathCnt ; } }

        public int _nAlignTestCount { get { return m_nAlignTestCount; } set { m_nAlignTestCount = value; } }
        


		public bool _bDrngToolPick    { get { return m_bDrngToolPick ; } }
        public bool _bDrngToolCheck   { get { return m_bDrngToolCheck; } }
        public bool _bDrngForceChk    { get { return m_bDrngForceChk ; } }
        public bool _bDrngPolishing   { get { return m_bDrngPolishing; } }
        public bool _bDrngCleaning    { get { return m_bDrngCleaning ; } }
        public bool _bDrngToolPlce    { get { return m_bDrngToolPlce ; } }
        public bool _bDrngVisnInspA   { get { return m_bDrngVisnInspA; } }
		public bool _bDrngVisnInspL   { get { return m_bDrngVisnInspL; } }
									  
		public bool _bDrngUtilLevel   { get { return m_bDrngUtilLevel; } }
        public bool _bDrngPlatePick   { get { return m_bDrngPlatePick; } }
        public bool _bDrngPlatePlce   { get { return m_bDrngPlatePlce; } }
        public bool _bDrngPlatePickP  { get { return m_bDrngPlatePickP; } }
        public bool _bDrngPlatePickC  { get { return m_bDrngPlatePickC; } }
        public bool _bDrngPlatePlceP  { get { return m_bDrngPlatePlceP; } }
        public bool _bDrngPlatePlceC  { get { return m_bDrngPlatePlceC; } }

        public bool _bDrngWait        { get { return m_bDrngWait; } }
		
		public bool _bPlatePlacePoli  { get { return PlatePlceInfo.nBtmPlate == (int)EN_MAGA_ID.POLISH; } }
		public bool _bPlatePlaceClen  { get { return PlatePlceInfo.nBtmPlate == (int)EN_MAGA_ID.CLEAN ; } }
		public bool _bReqUtil_Polish  { get { return m_bReqUtil_Polish; } set { m_bReqUtil_Polish = value; } }

        public bool _bReqResetGraph   { get { return m_bReqResetGraph; } set { m_bReqResetGraph = value; } }
        

        public DateTime _tStartPolishing { get { return m_tStartPolishing; } }
		public DateTime _tEndPolishing	 { get { return m_tEndPolishing  ; } }
									   
        public string _sStartTime      { get { return m_sStartTime; }}
		public string _sEndTime	       { get { return m_sEndTime; }}
        public string _sMillStartTime  { get { return m_sMillStartTime; } }
        public string _sMillEndTime    { get { return m_sMillEndTime  ; } }
        public string _sCleanStartTime { get { return m_sCleanStartTime; } }
        public string _sCleanEndTime   { get { return m_sCleanEndTime; } }

		

		/************************************************************************/
		/* 생성자.                                                               */
		/************************************************************************/
		public SeqSpindle()
		{

			//
			Init();

			//
			m_iPartId   = (int)EN_SEQ_ID.SPINDLE;

			m_iMotrXId  = EN_MOTR_ID.miSPD_X ;
			m_iMotrZId  = EN_MOTR_ID.miSPD_Z ;
            m_iMotrZ1Id = EN_MOTR_ID.miSPD_Z1;
			m_iBtmMotr  = EN_MOTR_ID.miSTR_Y ;
			
			m_tStartPolishing = DateTime.Now;
			m_tEndPolishing   = DateTime.Now;

			m_sStartTime      = string.Empty; 
			m_sEndTime        = string.Empty;
			m_sMillStartTime  = string.Empty;
			m_sMillEndTime	  = string.Empty;

			m_sCleanStartTime = string.Empty;
			m_sCleanEndTime	  = string.Empty;

		}
        //---------------------------------------------------------------------------
        private void Init()
		{
			m_bToStart        = false; 
			m_bToStop         = false;
			m_bWorkEnd        = false;
			m_bDrngWait       = false;
	
			m_bDrngToolPick   = false;
			m_bDrngToolCheck  = false;
			m_bDrngForceChk   = false;
			m_bDrngPolishing  = false;
			m_bDrngCleaning   = false;
			m_bDrngToolPlce   = false;
			m_bDrngVisnInspA  = false;
			m_bDrngVisnInspL  = false;
			m_bDrngUtilLevel  = false;
			m_bDrngPlatePick  = false;
			m_bDrngPlatePlce  = false;
			m_bDrngPlatePlceP = false;
			m_bDrngPlatePlceC = false;
            m_bDrngPlatePickP = false;
			m_bDrngPlatePickC = false;
            m_bDrngWait       = false;
			m_bDrngCupInPos   = false;

			m_bTESTMODE       = false;
			m_bForceStop      = false;
            m_bOffDone        = false;
            m_bDCOMReset	  = false;
			m_bReqResetGraph  = false;

			m_nSeqStep        = 0; 
			m_nManStep        = 0; 
			m_nHomeStep       = 0;
					  
			m_nToolPickStep   = 0;
			m_nToolChkStep    = 0;
			m_nForceChkStep   = 0;
			m_nPolishStep     = 0;
			m_nCleanStep      = 0;
			m_nToolPlaceStep  = 0;
			m_nVisnInspStep   = 0;
			m_nUtilChkStep    = 0;
			m_nMagzPickStep   = 0;
			m_nMagzPlaceStep  = 0;
			m_nPoliBackStep   = 0;
			m_nAutoCalStep    = 0;
			m_nVisnBackStep   = 0;
			m_nCupMoveStep    = 0;

			m_nCalCycle       = 0;
			m_nTotalCalCycle  = 0;
			m_nCalCount       = 0; //Save
			m_nTopForceValue  = 0; 
			m_nBtmForceValue  = 0; //Display
			m_nForceOffset    = 0;
			m_dForceRatio     = 3.65;
			m_nToolClampRetry = 0;
			m_nTestIndex      = 0;
			m_nPoliCnt        = 0;
			m_nCleanCnt       = 0;
			m_nPolCycle       = 0;
			m_nClnCycle       = 0;
			m_nTotalCycle     = 0;
			m_nWaterClenCnt   = 0;
			m_nVisnRetry      = 0;
			m_nPreCycleNo     = -1;
			m_nCalTotalCnt    = 0;
			m_nCylRetryCnt    = 0;
			m_nTotalPathCnt   = 0;

			//m_dCycleTimeStart = 0.0;
			//m_dCycleTimeEnd   = 0.0;
			m_dDirectPosn     = 0.0; 
			m_dMPos           = 0.0;
			m_dEPos           = 0.0;

            m_dDisX_PCtoPS    = 0.0;
            m_dDisY_PCtoPS	  = 0.0;
            m_dCleanOffsetX   = 0.0;
            m_dCleanOffsetY   = 0.0;

            nTestCnt          = 0;
            m_dTestDCOM       = 2;

			//Timer Clear
			m_tMainCycle.Clear();
			m_tHome     .Clear();

			//
			fn_InitToolPickInfo (ref ToolPickInfo );
			fn_InitPlateInfo    (ref PlatePickInfo);
			fn_InitPlateInfo    (ref PlatePlceInfo); 
			fn_InitVisnInspInfo (ref VisnInspInfo );

			fn_InitPlateInfo    (ref PlateMoveInfo);

			//
			m_iCmdX  = EN_COMD_ID.NoneCmd; 
            m_iCmdY  = EN_COMD_ID.NoneCmd;
			m_iCmdZ  = EN_COMD_ID.NoneCmd;
			m_iCmdZ1 = EN_COMD_ID.NoneCmd;

			//
			vresult      = new ST_VISION_RESULT(0);
			m_stCleanRcp = new ST_CLEAN_RECIPE (0);

			//
			m_bSpare1 = m_bSpare2 = m_bSpare3 = m_bSpare4 = m_bSpare5 = false;
			m_nSpare1 = m_nSpare2 = m_nSpare3 = m_nSpare4 = m_nSpare5 = 0;
			m_dSpare1 = m_dSpare2 = m_dSpare3 = m_dSpare4 = m_dSpare5 = 0.0;

			for (int n1 = 0; n1 < 20; n1++)
			{
				for (int n2 = 0; n2 < 3; n2++)
                {
					m_dCalVal[n1, n2] = new double();

					m_dCalVal[n1, n2] = 0.0;

				}
			}

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Reset
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/10/10  17:06
		*/
		public void fn_Reset()
		{
			m_bToStart        = false; 
			m_bToStop         = false;
			m_bWorkEnd        = false;
		
			m_bDrngToolPick   = false;
			m_bDrngToolCheck  = false;
			m_bDrngForceChk   = false;
			m_bDrngPolishing  = false;
			m_bDrngCleaning   = false;
			m_bDrngToolPlce   = false;
			m_bDrngVisnInspA  = false;
			m_bDrngVisnInspL  = false;
			m_bDrngUtilLevel  = false;
			m_bDrngPlatePick  = false;
			m_bDrngPlatePlce  = false;
			m_bDrngPlatePlceP = false;
			m_bDrngPlatePlceC = false;
			m_bDrngPlatePickP = false;
			m_bDrngPlatePickC = false;
            m_bDrngWait       = false;
			m_bDrngCupInPos   = false;

			m_bTESTMODE       = false;
			m_bForceStop      = false;

			m_nSeqStep        = 0; 
			m_nManStep        = 0; 
			m_nHomeStep       = 0;

			m_nToolPickStep   = 0;
			m_nToolChkStep    = 0;
			m_nForceChkStep   = 0;
			m_nPolishStep     = 0;
			m_nCleanStep      = 0;
			m_nToolPlaceStep  = 0;
			m_nVisnInspStep   = 0;
			m_nUtilChkStep    = 0;
			m_nMagzPickStep   = 0;
			m_nMagzPlaceStep  = 0;
			m_nCalCount       = 0;
			m_nCalCycle       = 0;
			m_nTotalCalCycle  = 0;
			m_nToolClampRetry = 0;
			m_nCalTotalCnt    = 0;
			m_nCupMoveStep    = 0;

			m_dDirectPosn     = 0.0; 
			m_dEPos           = 0.0;
			m_dMPos           = 0.0;

			m_nPoliBackStep   = 0;
			m_nAutoCalStep    = 0;
			m_nVisnBackStep   = 0;
			m_nCylRetryCnt    = 0;

			m_sSeqMsg         = string.Empty; 

			//Timer Clear
			m_tMainCycle.Clear();
			m_tToStart  .Clear();
			m_tToStop   .Clear();
            m_tHome     .Clear();

            //
            fn_InitToolPickInfo (ref ToolPickInfo );
			fn_InitPlateInfo    (ref PlatePickInfo);
			fn_InitPlateInfo    (ref PlatePlceInfo); 
			fn_InitVisnInspInfo (ref VisnInspInfo );

			fn_InitPlateInfo    (ref PlateMoveInfo);

			fn_MoveToolClamp(ccFwd);
			fn_SetSpindleRun(0);

			g_VisionManager.fn_SetLightValue(swOff); //Light Off

            //Z-Axis Soft limit disable
            IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false, false);
			fn_MillBuffEnd();

			//JUNG/200921
			double dMinZAxisPos = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ_POS_MOVE_MAINX].dPos;
			if (!SEQ.fn_IsAnyDoorOpen())
			{
				if (GetEncPos_Z() > dMinZAxisPos)
				{
					fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);
				}
			}


        }
		//---------------------------------------------------------------------------
		public double GetEncPos_X () { return MOTR.GetEncPos(m_iMotrXId ); }
		public double GetEncPos_Z () { return MOTR.GetEncPos(m_iMotrZId ); }
		public double GetEncPos_Z1() { return MOTR.GetEncPos(m_iMotrZ1Id); }
		public double GetCmdPos_X () { return MOTR.GetCmdPos(m_iMotrXId ); }
		public double GetCmdPos_Z () { return MOTR.GetCmdPos(m_iMotrZId ); }
		public double GetCmdPos_Z1() { return MOTR.GetCmdPos(m_iMotrZ1Id); }
		public double GetTrgPos_X () { return MOTR.GetTrgPos(m_iMotrXId ); }
		public double GetTrgPos_Z () { return MOTR.GetTrgPos(m_iMotrZId ); }
		public double GetTrgPos_Z1() { return MOTR.GetTrgPos(m_iMotrZ1Id); }
		
		//---------------------------------------------------------------------------
		private bool CheckDstb(EN_MOTR_ID Motr, EN_COMD_ID Cmd = EN_COMD_ID.NoneCmd,
                                    int Step = NONE_STEP, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE, double DirPosn = 0.0)
        {
			//Var.
			bool isNoRun        = SEQ.fn_IsNoRun() && m_nManStep == 0 ;
			bool isOpenDoor     = SEQ.fn_IsAnyDoorOpen(); 
			bool isPolishInPos  = true; //Check Move Position of X
			bool isDrngPolish   = m_bDrngPolishing && isPolishInPos;
			bool isDrngClean    = m_bDrngCleaning  ;
			
					    
			double dEnc_X       = GetEncPos_X ();
			double dEnc_Z       = GetEncPos_Z ();
			double dEnc_Z1      = GetEncPos_Z1();
					    
			double dCmd_X       = GetCmdPos_X ();
			double dCmd_Z       = GetCmdPos_Z ();
			double dCmd_Z1      = GetCmdPos_Z1();
					    
			double dTrg_X       = GetTrgPos_X ();
			double dTrg_Z       = GetTrgPos_Z ();
			double dTrg_Z1      = GetTrgPos_Z1();

			double dMinZAxisPos  = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ_POS_MOVE_MAINX ].dPos;
			double dMinZ1AxisPos = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ1_POS_MOVE_MAINX].dPos;
			double dNextPosn     = 0;

			if (Motr != EN_MOTR_ID.miSPD_X && Motr != EN_MOTR_ID.miSPD_Z && Motr != EN_MOTR_ID.miSPD_Z1) return false;

            if (Cmd == EN_COMD_ID.Direct) dNextPosn = DirPosn;
            else                          dNextPosn = MOTR.GetNextCmdTrg(Motr, Cmd, Step, FIndex);


            if (isOpenDoor)
			{
				MOTR.Stop(Motr);
				if (isNoRun) fn_UserMsg("Door is Opened.");
				return false;
			}

			if (SEQ._bNoSafety)
			{
				MOTR.Stop(Motr);
				if (isNoRun) fn_UserMsg("Any Safety Checked. Check it!!!");
				return false;
			}

			if (!MOTR[(int)Motr].GetReady())
			{
                MOTR.Stop(Motr);
                if (isNoRun) fn_UserMsg("Motor is not ready now");
                return false;
            }

            //Check Interfere Condition
            
			if (Motr == EN_MOTR_ID.miSPD_X)
			{
				//Z-Axis Home End Check
				bool bHomeEndZ  = MOTR[(int)EN_MOTR_ID.miSPD_Z ].GetHomeEnd();
				bool bHomeEndZ1 = MOTR[(int)EN_MOTR_ID.miSPD_Z1].GetHomeEnd();

				if (!bHomeEndZ || !bHomeEndZ1)
				{
					MOTR.Stop(Motr);
					if (isNoRun) fn_UserMsg("Spindle Z or Z1-Axis is not Home end. so, it can't move");
					return false; 
				}

				//Z-Axis Position Check
				if (((dEnc_Z > dMinZAxisPos + 1.0) ||
					 (dCmd_Z > dMinZAxisPos + 1.0) ||
					 (dTrg_Z > dMinZAxisPos + 1.0)) && !isDrngPolish && !isDrngClean) 
				{
					if (isNoRun) fn_UserMsg("Spindle Z-Axis Position Value Error. Check Z-Axis Position.(with X");
					return false;
				}

				if ((dEnc_Z1 > dMinZ1AxisPos + 1.0) ||
					(dCmd_Z1 > dMinZ1AxisPos + 1.0) ||
					(dTrg_Z1 > dMinZ1AxisPos + 1.0))
				{
					if (isNoRun) fn_UserMsg("Spindle Z1-Axis Position Value Error. Check Z1-Axis Position.(with X)");
					return false;
				}

			}

			//
			if (Motr == EN_MOTR_ID.miSPD_Z)
			{
				if(Cmd != EN_COMD_ID.Wait1 && dNextPosn != 0 && !MAN._bDrngWarm) //JUNG/200602/Add Warm up
				{
					//X-Axis : Polishing, Cleaning, Tool Pick(Storage), Tool Check 
					if(MOTR.fn_WhrerSPD() != EN_WHERE_SPD.wpPOLI      && MOTR.fn_WhrerSPD() != EN_WHERE_SPD.wpCLEN     && MOTR.fn_WhrerSPD() != EN_WHERE_SPD.wpTOOLOUT &&
					   MOTR.fn_WhrerSPD() != EN_WHERE_SPD.wpPTOOLPICK && MOTR.fn_WhrerSPD() != EN_WHERE_SPD.wpFORCECHK && Cmd != EN_COMD_ID.Wait1)
					{
						if (isNoRun) fn_UserMsg("[Z] Spindle X-Axis is wrong position. Check X-Axis Position.(with Z)");
						return false; 
					}
				}

				//JUNG/200624/Check Z1-Axis Wait
				if (((dEnc_Z1 > dMinZ1AxisPos + 1.0) ||
                     (dCmd_Z1 > dMinZ1AxisPos + 1.0) ||
                     (dTrg_Z1 > dMinZ1AxisPos + 1.0)) && !MAN._bDrngWarm)
                {
                    if (isNoRun) fn_UserMsg("Spindle Z1-Axis Position Value Error. Check Z1-Axis Position.(with Z)");
                    return false;
                }
            }

            //
            if (Motr == EN_MOTR_ID.miSPD_Z1)
			{
				if (Cmd != EN_COMD_ID.Wait1 && dNextPosn != 0 && !MAN._bDrngWarm) //JUNG/200602/Add Warm up
				{
					//X-Axis : Polishing, Cleaning, Load
					if (MOTR.fn_WhrerSPD(true) != EN_WHERE_SPD.wpPLATEPOLI  && MOTR.fn_WhrerSPD(true) != EN_WHERE_SPD.wpPLATECLEN  && MOTR.fn_WhrerSPD(true) != EN_WHERE_SPD.wpCUPPOLISH &&
						MOTR.fn_WhrerSPD(true) != EN_WHERE_SPD.wpPLATELOAD1 && MOTR.fn_WhrerSPD(true) != EN_WHERE_SPD.wpCUPSTORAGE && Cmd != EN_COMD_ID.Wait1)
					{
						if (isNoRun) fn_UserMsg("[Z1] Spindle X-Axis is wrong position. Check X-Axis Position.(with Z1)");
						return false;
					}
				}

				//JUNG/200624/Z-Axis Position Check
				if (((dEnc_Z > dMinZAxisPos + 1.0) ||
					 (dCmd_Z > dMinZAxisPos + 1.0) ||
					 (dTrg_Z > dMinZAxisPos + 1.0)) && !MAN._bDrngWarm)
				{
					if (isNoRun) fn_UserMsg("Spindle Z-Axis Position Value Error. Check Z-Axis Position.(with Z1)");
					return false;
				}


				//JUNG/200428/Camera Cover
				if(!fn_MoveCylLensCvr(ccBwd))
				{
                    if (isNoRun) fn_UserMsg("[Z1] Lens cover Open!!");
                    return false;
                }
            }

			//
            return true; 

		}
		//---------------------------------------------------------------------------
        private bool fn_MoveMotr(EN_MOTR_ID Motr, EN_COMD_ID Cmd, EN_MOTR_VEL iSPD = EN_MOTR_VEL.Normal, 
                                              int Step = NONE_STEP, EN_FPOSN_INDEX Index = EN_FPOSN_INDEX.NONE)
        {
            bool   bRet   = false;
            double dPosn  = 0.0  ;
           
            //Stop Command. 
            if (Cmd == EN_COMD_ID.Stop || Cmd == EN_COMD_ID.EStop) return MOTR.MoveAsComd(Motr , Cmd , iSPD, Step , Index);

            //Check Disturb.
            if (!CheckDstb(Motr , Cmd , Step, Index)) return false;

            //Jog Command
            if (Cmd == EN_COMD_ID.JogP) return MOTR.MoveAsComd(Motr , Cmd , iSPD, Step , Index);
            if (Cmd == EN_COMD_ID.JogN) return MOTR.MoveAsComd(Motr , Cmd , iSPD, Step , Index);

	        //Find Step.
            if (((Cmd == EN_COMD_ID.FindStep1) ||
                 (Cmd == EN_COMD_ID.FindStep2) || 
                 (Cmd == EN_COMD_ID.FindStep3) ||
                 (Cmd == EN_COMD_ID.FindStep4))) 
			{
		        return MOTR.MoveAsComd(Motr , Cmd , iSPD, Step  , Index);
		    }

            //Command.
	        bRet = MOTR.MoveAsComd(Motr , Cmd , iSPD , Step , (EN_FPOSN_INDEX)Index);
            if(bRet) 
			{
                dPosn  = MOTR[(int)Motr].GetPosToCmdId(Cmd);
                //if (m_sLogMoveEvt == "" || m_sLogMoveEvt == null) m_sLogMoveEvt = "MANUAL"; 
                //LogTP.FunctionMove ((int)m_iPartId, m_sLogMoveEvt, (int)Motr, dPosn);
            } 

            return bRet;
        }
		//---------------------------------------------------------------------------
        public bool fn_MoveDirect(EN_MOTR_ID Motr, double Posn)
        {
            //Set Direct Position.
            m_dDirectPosn = Posn;

            //Move.
	        if (!CheckDstb      (Motr , EN_COMD_ID.Direct ,                       NONE_STEP , EN_FPOSN_INDEX.NONE , Posn)) return false;
            if (!MOTR.MoveAsComd(Motr , EN_COMD_ID.Direct , EN_MOTR_VEL.Normal  , NONE_STEP , EN_FPOSN_INDEX.NONE , Posn)) return false;

            //if (m_sLogMoveEvt == "" || m_sLogMoveEvt == null) m_sLogMoveEvt = "MANUAL"; 
            //LogTP.FunctionMove ((int)m_iPartId, m_sLogMoveEvt, (int)Motr, Posn);

            //Reset Direct Position.
            m_dDirectPosn = 0.0;

            //Ok.
            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_ReqMoveMotr(EN_MOTR_ID Motr , EN_COMD_ID Cmd)
        {
            //Check During.
            if (m_nSeqStep != 0) return false;

            //Move.
            return fn_MoveMotr(Motr , Cmd);
        }
		//---------------------------------------------------------------------------
        public bool fn_ReqMoveDirect(EN_MOTR_ID Motr, double Posn)
        {
            //Check During.
            if (m_nSeqStep != 0) return false;
            
			//Move.
            return fn_MoveDirect(Motr , Posn);
        }
        //---------------------------------------------------------------------------
        public bool fn_MoveToLastWorkPosn(EN_MOTR_ID Motr)
        {
            //
            double dPosn = 0.0;

            //
            fn_MoveDirect(Motr, dPosn);

            //Ok.
            return true;
        }
		//---------------------------------------------------------------------------
		public void fn_ClearHomeTime()
		{
			m_tHome.Clear();
		}
		//---------------------------------------------------------------------------
		public bool fn_MoveHome()
		{
			//
			bool r1, r2, r3;
			int iFHomeErr = MOTR._iFHomeErr;
			if (m_tHome.OnDelay(m_nHomeStep >= 10, 60 * 1000 * 2)) 
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0111 + (int)m_iPartId); //
				m_nHomeStep = 0;
				return true;
			}
			
			//Move Home.
			switch (m_nHomeStep)
			{
				case 0:
					m_nHomeStep = 0;
					return true;

				case 10: 
					//Clear Home end Flag
                    MOTR.ClearHomeEnd(EN_MOTR_ID.miSPD_X );
                    MOTR.ClearHomeEnd(EN_MOTR_ID.miSPD_Z );
                    MOTR.ClearHomeEnd(EN_MOTR_ID.miSPD_Z1);
					
					//Soft Limit Off
                    IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false, false);

                    //
                    fn_MoveCylLensCvr(ccBwd);
					fn_MoveCylIR     (ccBwd);

					if(fn_IsExistTool() && !FM.fn_IsLvlMaster()) //JUNG/200527
					{
                        fn_UserMsg("Please remove tool");
                        m_nHomeStep = 0;
                        return true;
                    }

					IO.fn_RunBuffer((int)EN_MOTR_ID.miSPD_X , false);
					IO.fn_RunBuffer((int)EN_MOTR_ID.miSPD_Z , false);
                    
					m_nHomeStep++;
					return false;

                case 11: //
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSPD_Z , true);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSPD_Z1, true);

					r1 = fn_MoveCylLensCvr(ccBwd); //JUNG/200424
					r2 = fn_MoveCylIR(ccBwd);
					if (!r1 || !r2) return false; 

                    m_nHomeStep++;
                    return false;

                case 12: //Buffer Run
					r1 = IO.fn_RunBuffer(BFNo_02_HOME_SPD_Z , true);
					r2 = IO.fn_SMCHome  (EN_MOTR_ID.miSPD_Z1, true);

					if (!r1)
					{
						fn_UserMsg(string.Format($"ACS RUN Buffer Error - {BFNo_02_HOME_SPD_Z:D2}:{r2.ToString()}"));
                        m_nHomeStep = 0;
                        return true;
                    }

                    m_nHomeStep++;
                    return false;

                case 13: 
					//Check Buffer Run 
                    r1 = IO.fn_IsBuffRun(BFNo_02_HOME_SPD_Z);
					if (!r1) return false;

					m_nHomeStep++;
                    return false;
                
				case 14:
					//Check Error
					if (IO.DATA_ACS_TO_EQ[BFNo_02_HOME_SPD_Z] == 1)
                    {
						fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_02_HOME_SPD_Z:D2}");
						m_nHomeStep = 0;
                        return true;
                    }


                    //Check Buffer End
                    r1 = IO.fn_IsBuffRun(BFNo_02_HOME_SPD_Z);
					r2 = MOTR[(int)EN_MOTR_ID.miSPD_Z1].GetHomeEnd() && MOTR[(int)EN_MOTR_ID.miSPD_Z1].GetHomeEndDone();
                    
                    EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSPD_Z ,  r1);
                    EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSPD_Z1, !r2);

					if (r1 || !r2) return false;

					MOTR[(int)EN_MOTR_ID.miSPD_Z].SetHomeEndDone(true);

					m_nHomeStep++;
                    return false;


                case 15: //
                    EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSPD_X , true);

                    m_nHomeStep++;
                    return false;

                case 16: 
					//Buffer Run
					r1 = IO.fn_RunBuffer(BFNo_00_HOME_SPD_X , true);
					if (!r1)
					{
						fn_UserMsg(string.Format($"ACS RUN Buffer Error - {BFNo_00_HOME_SPD_X:D2}:{r1.ToString()} "));
                        m_nHomeStep = 0;
                        return true;
                    }

                    m_nHomeStep++;
                    return false;

                case 17: //Check Buffer Run 
                    r1 = IO.fn_IsBuffRun(BFNo_00_HOME_SPD_X);
					if (!r1) return false;

					m_nHomeStep++;
                    return false;
                
				case 18:
					//Check Error
					if (IO.DATA_ACS_TO_EQ[BFNo_00_HOME_SPD_X] == 1)
					{
						fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_00_HOME_SPD_X:D2}");
						m_nHomeStep = 0;
                        return true;
                    }

                    //Check Buffer End
                    r1 = IO.fn_IsBuffRun(BFNo_00_HOME_SPD_X);
                    
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSPD_X ,  r1);

					if (r1) return false;

					MOTR[(int)EN_MOTR_ID.miSPD_X].SetHomeEndDone(true);

					m_nHomeStep++;
                    return false;

                case 19:
					r1 = fn_MoveMotr(m_iMotrXId , EN_COMD_ID.Wait1); //move X -Axis Wait position
					r2 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1); //JUNG/201022
					r3 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2 || !r3) return false; 

					m_nHomeStep =0;
                    return true;

				default:
					m_nHomeStep = 0;
                    return true;
            }
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStartCon()
		{
			m_bToStart = false;
			m_tToStart.Clear();

			//Start 시 마다 Tool Check 
			DM.TOOL.SetNeedCheck(true);

			//
			IO.fn_ForceBufferStop(); //JUNG/200418
			IO.fn_ClearDuringFlag(); //JUNG/200519

			//JUNG/200422/Check Tool condition
			bool bExtPolish  = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolish);
			bool bExtClean   = DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllStat((int)EN_PLATE_STAT.ptsClean );
			bool bExtToolPol = DM.TOOL.IsExistPol() ;
			bool bExtToolCln = DM.TOOL.IsExistCln() ;
			if (!bExtPolish && bExtToolPol)
			{
				DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);
				fn_WriteLog("Tool map change : POLISH -> USED");
			}
			if (!bExtClean && bExtToolCln)
			{
				DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);
				fn_WriteLog("Tool map change : CLEAN -> USED");
			}

			////Soft Limit disable
			//IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false, false);
			//
			////Buffer Run Clear
			//IO.fn_StopBuffer(BFNo_14_FORCECHECK);
			//IO.fn_StopBuffer(BFNo_13_MILLING   );

			fn_MillBuffEnd();//JUNG/201020

			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStopCon()
		{
			m_bToStop = false; 
			m_tToStop.Clear();

			//Check Step
			if (m_nSeqStep        != 0) return false;

			if (m_nToolPickStep   != 0) return false;
			if (m_nToolChkStep    != 0) return false;
			if (m_nForceChkStep   != 0) return false;
			if (m_nPolishStep     != 0) return false;
			if (m_nCleanStep      != 0) return false;
			if (m_nToolPlaceStep  != 0) return false;
			if (m_nVisnInspStep   != 0) return false;
			if (m_nUtilChkStep    != 0) return false;
			if (m_nMagzPickStep   != 0) return false;
			if (m_nMagzPlaceStep  != 0) return false;
			if (m_nCupMoveStep    != 0) return false;

			//Check During Flag
			if (m_bDrngToolPick       ) return false; 
			if (m_bDrngToolCheck      ) return false; 
			if (m_bDrngForceChk       ) return false; 
			if (m_bDrngPolishing      ) return false; 
			if (m_bDrngCleaning       ) return false; 
			if (m_bDrngToolPlce       ) return false; 
			if (m_bDrngVisnInspA      ) return false;
			if (m_bDrngVisnInspL      ) return false;
			if (m_bDrngUtilLevel      ) return false;
			if (m_bDrngPlatePick      ) return false;
			if (m_bDrngPlatePlce      ) return false;
			if (m_bDrngWait           ) return false;
			if (m_bDrngCupInPos       ) return false;

			m_tToStop.Clear();

			return true; 

		}
		//---------------------------------------------------------------------------
		public bool fn_ToStart()
		{
			//
			if (m_bToStart) return true;
			
			bool r1, r2, r3, r4;

			//Check Start Time Out
			bool bIsInitState = SEQ.fn_IsSeqStatus(EN_SEQ_STATE.INIT);
			
			if (m_tToStart.OnDelay(!m_bToStart && !bIsInitState, 20000))
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0150 + m_iPartId, true);
				return false;
			}

     		//Check Lens Cover Close 
			r1 = fn_MoveCylLensCvr(ccClose); 
			r2 = fn_MoveCylIR     (ccClose);
            r3 = fn_MoveMotr      (m_iMotrZId , EN_COMD_ID.Wait1);
            r4 = fn_MoveMotr      (m_iMotrZ1Id, EN_COMD_ID.Wait1);
			if (!r1 || !r2 || !r3 || !r4) return false;

			if (!fn_MoveMotr(m_iMotrXId, EN_COMD_ID.Wait1)) return false;

			//Timer Clear
			m_tMainCycle     .Clear();
			m_tToolPickCycle .Clear();
			m_tToolChkCycle  .Clear();
			m_tForceChkCycle .Clear();
			m_tPolishCycle   .Clear();
			m_tCleanCycle    .Clear();
			m_tToolPlaceCycle.Clear();
			m_tVisnInspCycle .Clear();
			m_tUtilChkCycle  .Clear();
			m_tMagzPickCycle .Clear();
			m_tMagzPlaceCycle.Clear();
			m_tCupInPosCycle .Clear();


			//Flag On
			m_bToStart = true ; 
			m_bToStop  = false;

			m_nPoliBackStep = 0;

			if(DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolishWait)) DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolish);

			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStop()
		{
			//
			if (m_bToStop) return true; 

			//Check Start Time Out
			if (m_tToStop.OnDelay(!m_bToStop, 20 * 1000))
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0160 + m_iPartId, true);
				
				fn_MillBuffEnd(); //JUNG/200602

				fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);

				return false;
			}

			//Spindle Clamp Air Off
			fn_MoveToolClamp (ccFwd);
			fn_SetSpindleRun (0    );
			fn_MoveCylIR     (ccBwd);
			fn_MoveCylLensCvr(ccBwd);

			//JUNG/201028/삭제
			//IO.fn_SetOpenLoopOff();
			//IO.fn_GroupDisable  ();

            bool b1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1);
			bool b2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1);
			if (!b1 || !b2) return false;
	
			//Force Flag , During Flag Off
			IO.fn_ForceBufferStop();
			IO.fn_ClearDuringFlag(); //JUNG/200519

			g_VisionManager.fn_SetLightValue(swOff); //Light Off

			//Z-Axis Soft limit disable
			IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false, false);

			//Buffer Run Clear
			if(IO.fn_IsBuffRun(BFNo_14_FORCECHECK)) IO.fn_StopBuffer(BFNo_14_FORCECHECK);
			if(IO.fn_IsBuffRun(BFNo_13_MILLING   )) IO.fn_StopBuffer(BFNo_13_MILLING   );

			//Clear Step Index
			m_nSeqStep = 0; 

			m_nToolPickStep  = 0;
			m_nToolChkStep   = 0;
			m_nForceChkStep  = 0;
			m_nPolishStep    = 0;
			m_nCleanStep     = 0;
			m_nToolPlaceStep = 0;
			m_nVisnInspStep  = 0;
			m_nUtilChkStep   = 0;
			m_nMagzPickStep  = 0;
			m_nMagzPlaceStep = 0;
			m_nCupMoveStep   = 0;

			if (DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolishWait)) DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolish);

            MOTR.Stop(m_iMotrXId);
            MOTR.Stop(EN_MOTR_ID.miPOL_Y);
			MOTR.Stop(EN_MOTR_ID.miCLN_Y);

			m_bToStop = true; 
			
			return true;
		}
		//---------------------------------------------------------------------------
		void fn_InitToolPickInfo(ref TOOL_PICK_INFO info)
		{
			info.bFind    = false; 
			info.nBtmStor = -1   ; 
			info.nFindCol = -1   ;
			info.nFindRow = -1   ; 
			info.dXpos    = 0.0  ;
			info.dYpos    = 0.0  ;
		}
		//---------------------------------------------------------------------------
		void fn_InitPlateInfo(ref PLATE_MOVE_INFO info)
		{
			info.bFind     = false;
			info.nBtmPlate = -1   ;
			info.nFindMode = -1   ;
			info.nRcpNo    = -1   ;
			info.dXpos     = 0.0  ;
			info.dYpos     = 0.0  ;
		}
		//---------------------------------------------------------------------------
		void fn_InitVisnInspInfo(ref VISN_INSP_INFO info)
		{
			info.bFind     = false;
			info.nBtmWhere = -1   ;
			info.nFindMode = -1   ;
			info.dXpos     = 0.0  ;
			info.dYpos     = 0.0  ;
		}
		//---------------------------------------------------------------------------
		private void fn_InitVisionResult()
		{
			vresult.nTotalStep = 0; 

			for (int i = 0; i < 10; i++)
			{
				vresult.stRecipeList[i].nUtilType     = 0; 
				vresult.stRecipeList[i].nUseMilling   = 0; 
				vresult.stRecipeList[i].nToolType     = 0; 
				vresult.stRecipeList[i].nUseToolChg   = 0; 
				vresult.stRecipeList[i].nUseUtilFill  = 0; 
				vresult.stRecipeList[i].nUseUtilDrain = 0; 
				vresult.stRecipeList[i].nUseImage     = 0; 
				vresult.stRecipeList[i].nUseEPD       = 0; 

				vresult.stRecipeList[i].dForce        = 0.0;
				vresult.stRecipeList[i].dForce        = 0.0;
				vresult.stRecipeList[i].dTilt         = 0.0;
				vresult.stRecipeList[i].dTheta        = 0.0;
                vresult.stRecipeList[i].dRPM          = 0.0;
				vresult.stRecipeList[i].dForce        = 0.0;
                vresult.stRecipeList[i].dSpeed        = 0.0;
				vresult.stRecipeList[i].dPitch        = 0.0;

				vresult.stRecipeList[i].pStartPos     = new Point(0, 0);
                vresult.stRecipeList[i].pEndPos       = new Point(0, 0);
			}

			fn_LoadVisnResult(flSave);

		}		
		//---------------------------------------------------------------------------
		public bool fn_AutoRun()
		{
			//
			bool r1, r2; 
			m_dMPos = MOTR[(int)EN_MOTR_ID.miSPD_X].GetCmdPos();
			m_dEPos = MOTR[(int)EN_MOTR_ID.miSPD_X].GetEncPos();

			bool bErr        = EPU._bIsErr;
			bool bManRun     = FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE );
			     m_bTESTMODE = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);

			//Time Check
			m_tMainCycle     .OnDelay((m_nSeqStep        != 0 && !bErr && !bManRun && !m_bDrngPolishing),  60 * 1000 * 25); //Except Polishing Time
																							        
			m_tToolPickCycle .OnDelay((m_nToolPickStep   != 0 && !bErr && !bManRun ),  60 * 1000                                     ); //
			m_tToolChkCycle  .OnDelay((m_nToolChkStep    != 0 && !bErr && !bManRun ),  60 * 1000                                     ); //
			m_tForceChkCycle .OnDelay((m_nForceChkStep   != 0 && !bErr && !bManRun ), 200 * 1000                                     ); //JUNG/201119
			m_tPolishCycle   .OnDelay((m_nPolishStep     != 0 && !bErr && !bManRun ),  60 * 1000 * FM.m_stSystemOpt.nPoliMillingTime ); //ACS Polishing Step은 따로 Check 필
			m_tCleanCycle    .OnDelay((m_nCleanStep      != 0 && !bErr && !bManRun ),  60 * 1000 * FM.m_stSystemOpt.nCleanMillingTime); //ACS Cleaning Step은 따로 Check 필요
			m_tToolPlaceCycle.OnDelay((m_nToolPlaceStep  != 0 && !bErr && !bManRun ),  60 * 1000									 ); //
			m_tVisnInspCycle .OnDelay((m_nVisnInspStep   != 0 && !bErr && !bManRun ), 300 * 1000									 ); //
			m_tUtilChkCycle  .OnDelay((m_nUtilChkStep    != 0 && !bErr && !bManRun ),  60 * 1000									 ); //
			m_tMagzPickCycle .OnDelay((m_nMagzPickStep   != 0 && !bErr && !bManRun ),  60 * 1000									 ); //
			m_tMagzPlaceCycle.OnDelay((m_nMagzPlaceStep  != 0 && !bErr && !bManRun ),  60 * 1000									 ); //
			m_tCupInPosCycle .OnDelay((m_nCupMoveStep    != 0 && !bErr && !bManRun ),  60 * 1000 * 3                                 ); //


			//
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0170 + m_iPartId, m_tMainCycle.Out))
			{
				sLogMsg = string.Format($"[Spindle] Main Cycle Time Out : m_iSeqStep = {m_nSeqStep}");
			}
            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0180, m_tToolPickCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Pick Cycle Time Out : m_iToolPickStep = {m_nToolPickStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0181, m_tToolChkCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Tool Check Cycle Time Out : m_iToolChkStep = {m_nToolChkStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0182, m_tForceChkCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Force Check Cycle Time Out : m_iForceChkStep = {m_nForceChkStep}");
            }

            //if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0183, m_tPolishCycle.Out))
            //{
            //    sLogMsg = string.Format($"[Spindle] Polishing Cycle Time Out : m_iPolishStep = {m_nPolishStep}");
            //}
			//
            //if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0184, m_tCleanCycle.Out))
            //{
            //    sLogMsg = string.Format($"[Spindle] Cleaning Cycle Time Out : m_iCleanStep = {m_nCleanStep}");
			//	MOTR[(int)EN_MOTR_ID.miCLN_R].Stop(); //JUNG/200601
            //}

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0185, m_tToolPlaceCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Tool Place Cycle Time Out : m_iToolPlaceStep = {m_nToolPlaceStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0186, m_tVisnInspCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Vision Inspect Cycle Time Out : m_iVisnInspStep = {m_nVisnInspStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0187, m_tUtilChkCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Util Check Cycle Time Out : m_iUtilChkStep = {m_nUtilChkStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0188, m_tMagzPickCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Magazine Pick Cycle Time Out : m_iMagzPickStep = {m_nMagzPickStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0189, m_tMagzPlaceCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Magazine Pick Cycle Time Out : m_iMagzPickStep = {m_nMagzPlaceStep}");
            }
            
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0189, m_tCupInPosCycle.Out))
            {
                sLogMsg = string.Format($"[Spindle] Cup InPos Cycle Time Out : m_nCupMoveStep = {m_nCupMoveStep}");
            }

			//
			if (m_tMainCycle     .Out ||
				m_tToolPickCycle .Out ||
				m_tToolChkCycle  .Out ||
				m_tForceChkCycle .Out ||
  			  //m_tPolishCycle   .Out ||
  			  //m_tCleanCycle    .Out ||
				m_tToolPlaceCycle.Out ||
				m_tVisnInspCycle .Out ||
				m_tUtilChkCycle  .Out ||
				m_tMagzPickCycle .Out ||
				m_tCupInPosCycle .Out ||
				m_tMagzPlaceCycle.Out) 
			{
				fn_WriteLog(sLogMsg);
				LOG.fn_CrntStateTrace(EN_SEQ_ID.SPINDLE, sLogMsg);
				fn_Reset();
				return false; 
			}

			if (m_tPolishCycle.Out)
			{
				EPU.fn_SetErr(EN_ERR_LIST.ERR_0183);

				fn_WriteLog("POLISHING TIME OUE" + sLogMsg);
                LOG.fn_CrntStateTrace(EN_SEQ_ID.SPINDLE, sLogMsg);

                fn_Reset();

				fn_MillBuffEnd(); //JUNG/201026
                
                return false;
            }
            if (m_tCleanCycle.Out)
            {
				EPU.fn_SetErr(EN_ERR_LIST.ERR_0184);

				fn_WriteLog("CLEANING TIME OUE" + sLogMsg);
                LOG.fn_CrntStateTrace(EN_SEQ_ID.SPINDLE, sLogMsg);

                fn_Reset();

                fn_MillBuffEnd(); //JUNG/201026

				return false;
            }

            //Emergency Error Check
            bool bEMOErr = EPU.fn_IsEMOError();  // Emergency Error
			if (m_nSeqStep != 0 && bEMOErr)
			{
				//
				sLogMsg = string.Format($"[EMO][SEQ_SPDL] Force Cycle End m_nSeqStep = {m_nSeqStep}");
				fn_WriteLog(sLogMsg);
				
				fn_Reset();
				
				m_nSeqStep = 0;
				return false; 
			}

			//Decide Step
			if (m_nSeqStep == 0)
			{
				//Local Var.
				bool bSPD_XWaitPos	= MOTR.CmprPosByCmd(m_iMotrXId , EN_COMD_ID.Wait1);
				bool bSPD_ZWaitPos	= MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1); 
                bool bSPD_Z1WaitPos = MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1); 

				bool bCheckForce    = fn_IsCheckForce();
				bool bNeedToolChk   = DM.TOOL.IsNeedCheck();
				bool xExistTool     = fn_IsExistTool(true);
				bool xExitCupInPos  = fn_IsExistCup (true);


				bool isToolUsed     = DM.TOOL.IsAllStat((int)EN_PIN_STAT.psUsed  );
				bool isToolPolish   = DM.TOOL.IsAllStat((int)EN_PIN_STAT.psNewPol);
				bool isToolClean    = DM.TOOL.IsAllStat((int)EN_PIN_STAT.psNewCln);
				

				bool isReadyPolish  = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolish);
				bool isReadyClean   = DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllStat((int)EN_PLATE_STAT.ptsClean );
				bool isPoliToolWait = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolishWait);
				bool bNeedUitlChk   = SEQ_POLIS._bUitlUnkown && !m_bTESTMODE           ;
				bool bReqLevel_Pol  = m_bReqUtil_Polish                                ; 
				bool bRecipeOpen    = LOT._bLotOpen          ||  m_bTESTMODE           ; 
				bool bPoliUtilEmt   = SEQ_POLIS._bEmptyUitl  ||  SEQ_POLIS._bUitlUnkown;
				bool bPoliSplyUtil  = SEQ_POLIS._bDrngUtility                          ;
				bool bPoliDrngDrain = SEQ_POLIS._bDrngDrain                            ;
				bool isDrngTRPick   = SEQ_TRANS._bDrngPickPlate                        ; //JUNG/200515/
				bool bDrngPoli      = (m_nPoliBackStep == 23) && (m_nPoliCnt > 0)      ;


				//Set Info
				ToolPickInfo	= fn_GetToolPickInfo ();
				PlatePickInfo	= fn_GetPlatePickInfo(); 
				PlatePlceInfo	= fn_GetPlatePlceInfo();
				VisnInspInfo	= fn_GetInspInfo     ();
		
				//Step Condition 
				bool isConPlatePick    =  PlatePickInfo.bFind; 
				bool isConPlatePlace   =  PlatePlceInfo.bFind;
				//bool isConCupInPlace   = !isReadyPolish && !xExitCupInPos && FM.m_stSystemOpt.nUsePolishingCup == 1 && DM.TOOL.IsPlateEmpty(); 
				bool isConCupInPlace   = !isReadyPolish && !xExitCupInPos && FM.m_stSystemOpt.nUsePolishingCup == 1 && DM.TOOL.IsPlateEmpty() && !bDrngPoli; 

				bool isConToolCheck    =  bNeedToolChk && !isToolUsed                                  ;
				bool isConToolPick     =  ToolPickInfo.bFind                                           ; 
				bool isConForceCheck   = !bNeedToolChk && !bCheckForce && (isToolPolish || isToolClean);
				bool isConToolPlace    =  isToolUsed   ; //&& xExistTool                               ;

				bool isConPolishing    =  bRecipeOpen  && isToolPolish && !bNeedToolChk && bCheckForce && isReadyPolish  && !bPoliUtilEmt;
				bool isConCleaning     =  bRecipeOpen  && isToolClean  && !bNeedToolChk && bCheckForce && isReadyClean                   ;
				bool isConPolishing2   =  bRecipeOpen  && isToolPolish && !bNeedToolChk && bCheckForce && isPoliToolWait && !bPoliUtilEmt && bDrngPoli;

				bool isConVisnInspLoad =  VisnInspInfo.bFind && (VisnInspInfo.nBtmWhere == (int)EN_MAGA_ID.LOAD  )                ;
     			bool isConVisnInspP    =  VisnInspInfo.bFind && (VisnInspInfo.nBtmWhere == (int)EN_MAGA_ID.POLISH) && bPoliUtilEmt;

				bool isConPolUtilChk   =  bNeedUitlChk   && !isConPlatePick && !isConPlatePlace && !isDrngTRPick   && 
										 !isConToolCheck && !isConToolPick  && !isConToolPlace  && !isConPolishing && !isConPolishing2; //m_enUtilSate == EN_UTIL_STATE.Unknown

				bool isConUtilLevelChk =  bReqLevel_Pol && !m_bTESTMODE; //for control Slurry solidification

				bool isConWait         = !isConToolPick     && !isConToolCheck    && !isConForceCheck   && !isConPolishing  && 
					                     !isConCleaning     && !isConToolPlace    && !isConVisnInspLoad && !isConVisnInspP  && 
										 !isConUtilLevelChk && !isConPlatePick    && !isConPlatePlace   && !isConPolishing2 &&
										 !isConPolUtilChk   && !bSPD_XWaitPos     && !bPoliSplyUtil  ;

				//Clear Var.
				m_bDrngToolPick        = false;
				m_bDrngToolCheck       = false;
				m_bDrngForceChk        = false;
				m_bDrngPolishing       = false;
				m_bDrngCleaning        = false;
				m_bDrngToolPlce        = false;
				m_bDrngVisnInspA       = false;
				m_bDrngVisnInspL       = false;
				m_bDrngUtilLevel       = false;
				m_bDrngPlatePick       = false;
				m_bDrngPlatePlce       = false;
				m_bDrngPlatePickP      = false;
				m_bDrngPlatePickC      = false;
				m_bDrngPlatePlceP      = false;
				m_bDrngPlatePlceC      = false;
				m_bDrngWait            = false;
				m_bDrngCupInPos        = false;


				//Step Clear
				m_nToolPickStep        = 0;
				m_nToolChkStep         = 0;
				m_nForceChkStep        = 0;
				m_nPolishStep          = 0;
				m_nCleanStep           = 0;
				m_nToolPlaceStep       = 0;
				m_nVisnInspStep        = 0;
				m_nUtilChkStep         = 0;
				m_nMagzPickStep        = 0;
				m_nMagzPlaceStep       = 0;
				m_nCupMoveStep         = 0;

				m_sSeqMsg              = string.Empty; 


				//Check Sequence Stop
				if ( SEQ._bStop            ) return false; 
				if ( EPU.fn_GetHasErr()    ) return false; 
				if (!SEQ._bRun && !bManRun ) return false;
				if (!fn_CheckPlateError()  ) return false;
				if (!fn_CheckToolError ()  ) return false;

				//
				if (isConPlatePlace  ) { m_bDrngPlatePlce  = true; m_nSeqStep = 1200; m_nMagzPlaceStep  = 10;  m_sSeqMsg = "Place Plate"        ; goto __GOTO_CYCLE__; }
				if (isConToolPlace   ) { m_bDrngToolPlce   = true; m_nSeqStep = 600 ; m_nToolPlaceStep  = 10;  m_sSeqMsg = "Place Tool"         ; goto __GOTO_CYCLE__; }
				if (isConCupInPlace  ) { m_bDrngCupInPos   = true; m_nSeqStep = 2000; m_nCupMoveStep    = 10;  m_sSeqMsg = "Cup In-Place"       ; goto __GOTO_CYCLE__; }
				if (isConToolCheck   ) { m_bDrngToolPick   = true; m_nSeqStep = 200 ; m_nToolChkStep    = 10;  m_sSeqMsg = "Check Tool"         ; goto __GOTO_CYCLE__; }
    			if (isConPlatePick   ) { m_bDrngPlatePick  = true; m_nSeqStep = 1100; m_nMagzPickStep   = 10;  m_sSeqMsg = "Pick Plate"         ; goto __GOTO_CYCLE__; }
				if (isConVisnInspLoad) { m_bDrngVisnInspL  = true; m_nSeqStep = 800 ; m_nVisnInspStep   = 10;  m_sSeqMsg = "Visn Insp.(Load)"   ; goto __GOTO_CYCLE__; }
				if (isConVisnInspP   ) { m_bDrngVisnInspA  = true; m_nSeqStep = 900 ; m_nVisnInspStep   = 10;  m_sSeqMsg = "Visn Insp.(Poli)"   ; goto __GOTO_CYCLE__; }
				if (isConToolPick    ) { m_bDrngToolCheck  = true; m_nSeqStep = 100 ; m_nToolPickStep   = 10;  m_sSeqMsg = "Pick Tool"          ; goto __GOTO_CYCLE__; }
				if (isConForceCheck  ) { m_bDrngForceChk   = true; m_nSeqStep = 300 ; m_nForceChkStep   = 10;  m_sSeqMsg = "Check Force"        ; goto __GOTO_CYCLE__; }
				if (isConPolishing   ) { m_bDrngPolishing  = true; m_nSeqStep = 400 ; m_nPolishStep     = 10;  m_sSeqMsg = "Polishing"          ; goto __GOTO_CYCLE__; }
				if (isConPolishing2  ) { m_bDrngPolishing  = true; m_nSeqStep = 400 ; m_nPolishStep     =  5;  m_sSeqMsg = "Polishing"          ; goto __GOTO_CYCLE__; }
																																			    
				if (isConCleaning    ) { m_bDrngCleaning   = true; m_nSeqStep = 500 ; m_nCleanStep      = 10;  m_sSeqMsg = "Cleaning"           ; goto __GOTO_CYCLE__; }
				if (isConPolUtilChk  ) { m_bDrngUtilLevel  = true; m_nSeqStep = 1300; m_nUtilChkStep    = 10;  m_sSeqMsg = "Check Utililty(POL)"; goto __GOTO_CYCLE__; }
				if (isConUtilLevelChk) { m_bDrngUtilLevel  = true; m_nSeqStep = 1000; m_nUtilChkStep    = 10;  m_sSeqMsg = "Check Util Level"   ; goto __GOTO_CYCLE__; }
				if (isConWait        ) { m_bDrngWait       = true; m_nSeqStep = 5000;                          m_sSeqMsg = "Wait"               ; goto __GOTO_CYCLE__; }

			}

			//Cycle Start
			__GOTO_CYCLE__:

			//Cycle
			switch (m_nSeqStep)
			{
				case 100 :
					if (fn_ToolPickCycle(ref m_bDrngToolPick, ref ToolPickInfo)) m_nSeqStep = 0;
					return false; 

				case 200 :
					if (fn_ToolCheckCycle(ref m_bDrngToolCheck)) m_nSeqStep = 0;
					return false;

				case 300:
					if (fn_ForceCheckCycle(ref m_bDrngForceChk)) m_nSeqStep = 0;
					return false;

				case 400:
					if (fn_PolishingCycle(ref m_bDrngPolishing)) m_nSeqStep = 0;
					return false;

				case 500:
					if (fn_CleaningCycle(ref m_bDrngCleaning)) m_nSeqStep = 0;
					return false;

				case 600:
					if (fn_ToolPlaceCycle(ref m_bDrngToolPlce)) m_nSeqStep = 0;
					return false;
	
				case 800: //Pre-Align
					
					if (FM.m_stMasterOpt.nUseMOC == 1)
					{
						if (fn_VisnInspCycle2(ref m_bDrngVisnInspL, VisnInspInfo)) m_nSeqStep = 0;
					}
					else
					{
						if (fn_VisnInspCycle(ref m_bDrngVisnInspL, VisnInspInfo)) m_nSeqStep = 0;
					}
					return false;

				case 900: //Polishing
                    if (FM.m_stMasterOpt.nUseMOC == 1)
                    {
                        if (fn_VisnInspCycle2(ref m_bDrngVisnInspA, VisnInspInfo)) m_nSeqStep = 0;
                    }
                    else
                    {
						if (fn_VisnInspCycle(ref m_bDrngVisnInspA, VisnInspInfo)) m_nSeqStep = 0;
					}

                    
					return false;

				case 1000:
					if (fn_UtilCheckCycle(ref m_bDrngUtilLevel)) m_nSeqStep = 0;
					return false;

				case 1100:
					if (fn_PlatePickCycle(ref m_bDrngPlatePick, PlatePickInfo)) m_nSeqStep = 0;
					return false;

				case 1200: 
					if (fn_PlatePlaceCycle(ref m_bDrngPlatePlce, PlatePlceInfo)) m_nSeqStep = 0;
					return false;

				case 1300: //Polishing Utility Exist Check Cycle
                    if (fn_UtilExtCheckCycle(ref m_bDrngUtilLevel)) m_nSeqStep = 0;
                    return false;
                
				case 2000: //Cup In Position Cycle
                    if (fn_CupInPosCycle(ref m_bDrngCupInPos)) m_nSeqStep = 0;
                    return false;


                //Wait Position 
                case 5000: //Z-Axis, Z1-Axis Wait Position 
					m_bDrngWait = true; 

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    m_nSeqStep++;
					return false; 

				case 5001: //X-Wait Position
					r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrXId, EN_COMD_ID.Wait1);
					if (!r1 ) return false;

					m_bDrngWait = false;
					m_nSeqStep = 0;
					return true;

				default:
					m_nSeqStep = 0;
					break;

			}

			return true;
		}
		//---------------------------------------------------------------------------
		public string fn_GetSeqMsg()
        {
			string rtn = string.Empty;

			if (m_sSeqMsg == string.Empty) return rtn; 

			rtn = string.Format($"Now {m_sSeqMsg}...");

			return rtn; 
        }
		//---------------------------------------------------------------------------
		public bool fn_CheckPlateError()
		{
			//
			bool bOk = true;

			if (m_bTESTMODE) return bOk;
			bool bExtMainZ1 = !DM.TOOL.IsPlateEmpty(); //PLATES[0].IsEmpty();
			bool xExtMainZ1 = fn_GetPltExtMI(EN_MAGA_ID.SPINDLE, bExtMainZ1);
			

			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0352,  bExtMainZ1 && !xExtMainZ1)) bOk = false; //Main-Z Plate Missing.
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0357, !bExtMainZ1 &&  xExtMainZ1)) bOk = false; //Main-Z Unknown Plate .

            ////Check Recipe Name
            //if (xExtMainZ1)
			//{
			//	string sPlateRcp = DM.TOOL.GetPlateRecipeName(); //PLATES[0]._sRecipeName; 
			//	if(sPlateRcp != FM._sRecipeName)
			//	{
			//		if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0370, FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE))) bOk = false;
			//	}
			//}

			return bOk; 
		}

		//---------------------------------------------------------------------------
		private TOOL_PICK_INFO fn_GetToolPickInfo()
		{
			TOOL_PICK_INFO pickinfo = new TOOL_PICK_INFO(false);

			fn_InitToolPickInfo(ref pickinfo);

			//Polish Map : O, Spindle Tool : X, Need Check : X
			bool bExtPolish    = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolish    );
			bool bExtClean     = DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllStat((int)EN_PLATE_STAT.ptsClean     );
			bool bExtToolWait  = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolishWait);
			bool isExtToolPol  = DM.TOOL.IsExistPol()         ;
			bool isExtToolCln  = DM.TOOL.IsExistCln()         ;
			bool xExtToolPol   = fn_IsExistTool(isExtToolPol) ;
			bool xExtToolCln   = fn_IsExistTool(isExtToolCln) ;
			bool bExtToolPol   = isExtToolPol && xExtToolPol  ;
            bool bExtToolCln   = isExtToolCln && xExtToolCln  ;
            bool bNeedCheck    = DM.TOOL.IsNeedCheck() ;

			if (!DM.TOOL.IsEmpty()) return pickinfo; //JUNG/200526

			if (bExtClean && !bExtToolCln && !bNeedCheck) //JUNG/200422
            {
                pickinfo = DM.TOOL.GetPickInfoStorage(EN_PIN_FIND_MODE.fmNewCln);

				if (FM.m_stMasterOpt.nUseDirPos == 1)
				{
					//
					ST_PIN_POS Pos = new ST_PIN_POS(0);
					Pos = DM.STOR[siClean].PINS[pickinfo.nFindRow, pickinfo.nFindCol].GetPosition();

					pickinfo.dXpos = Pos.dXPos;
					pickinfo.dYpos = Pos.dYPos;
				}
                
				pickinfo.nBtmStor = siClean ;

				//JUNG/210119/Check Tool Exist
                if (!pickinfo.bFind)
                {
                    EPU.fn_SetErr(EN_ERR_LIST.ERR_0346);
                }


            }
            else if (bExtPolish && !bExtToolPol && !bNeedCheck)
			{
				int nToolType = vresult.stRecipeList[0].nToolType;
				pickinfo = DM.TOOL.GetPickInfoStorage(EN_PIN_FIND_MODE.fmNewPol, nToolType);

				if (FM.m_stMasterOpt.nUseDirPos == 1)
				{
					//
					ST_PIN_POS Pos = new ST_PIN_POS(0);
					Pos = DM.STOR[siPolish].PINS[pickinfo.nFindRow, pickinfo.nFindCol].GetPosition();

					pickinfo.dXpos = Pos.dXPos;
					pickinfo.dYpos = Pos.dYPos;
				}

				pickinfo.nBtmStor = siPolish ;

                //JUNG/210119/Check Tool Exist
                if (!pickinfo.bFind)
                {
                    EPU.fn_SetErr(EN_ERR_LIST.ERR_0345);
                }


            }
            else if(bExtToolWait && !bExtToolPol && !bNeedCheck) //JUNG/200612
            {
				if (m_nPoliCnt < 1) return pickinfo; 

				int nToolType = vresult.stRecipeList[m_nPoliCnt].nToolType;

				pickinfo = DM.TOOL.GetPickInfoStorage(EN_PIN_FIND_MODE.fmNewPol, nToolType);

                if (FM.m_stMasterOpt.nUseDirPos == 1)
                {
                    //
                    ST_PIN_POS Pos = new ST_PIN_POS(0);
                    Pos = DM.STOR[siPolish].PINS[pickinfo.nFindRow, pickinfo.nFindCol].GetPosition();

                    pickinfo.dXpos = Pos.dXPos;
                    pickinfo.dYpos = Pos.dYPos;
                }

				//JUNG/200616/Tool Exist Error
                if (!pickinfo.bFind)
                {
					EPU.fn_SetErr(EN_ERR_LIST.ERR_0343);
                }

                pickinfo.nBtmStor = siPolish;

            }

            return pickinfo; 

		}
		//---------------------------------------------------------------------------
		private PLATE_MOVE_INFO fn_GetPlatePickInfo()
		{
            //Local Var.
			PLATE_MOVE_INFO stPickInfo = new PLATE_MOVE_INFO(false);
            fn_InitPlateInfo(ref stPickInfo);

			bool IsTestMove     = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE  );
			bool bExistAlign    = fn_GetPltExtMI(EN_MAGA_ID.LOAD   , true );
			bool bExistMainZ    = fn_GetPltExtMI(EN_MAGA_ID.SPINDLE, false);

			//
			if (!DM.TOOL.IsPlateEmpty()    ) return stPickInfo; 
			if ( bExistMainZ && !IsTestMove) return stPickInfo;

			//
			bool bReqCleaning    = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsClean );
			bool bReqFnshPolBath = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsFinish);
			bool bReqFnshClnBath = DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllStat((int)EN_PLATE_STAT.ptsFinish);
			bool bReqAlign       = DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllStat((int)EN_PLATE_STAT.ptsAlign ); //for moving polishing
			bool isDrngUnload    = SEQ_TRANS._bDrngUnload || SEQ_TRANS._bDrngLoad ;
			bool bPoliDrngDrain  = SEQ_POLIS._bDrngDrain  || SEQ_POLIS._bDrngSeqDrain;
			bool bClenDrngDrain  = SEQ_CLEAN._bDrngSeqDrain;
			bool bPoliUtilEmt    = SEQ_POLIS._bEmptyUitl;

			//
			if (bReqFnshClnBath)
			{
				if (SEQ_CLEAN._bDrngCleaning) return stPickInfo;
				if (SEQ_CLEAN._bDrngSeqDrain) return stPickInfo;

				stPickInfo.bFind     = true;
				stPickInfo.nBtmPlate = (int)EN_MAGA_ID.CLEAN;
			}
            else if (bReqFnshPolBath)
			{
				if (!bPoliUtilEmt  ) return stPickInfo;
				if ( bPoliDrngDrain) return stPickInfo;

				stPickInfo.bFind     = true;
				stPickInfo.nBtmPlate = (int)EN_MAGA_ID.POLISH;
			}
			else if(bReqCleaning)
			{
				//if (!bPoliUtilEmt) return stPickInfo;  //Pick하러 가서 있으면 darin 

				if (!bPoliUtilEmt  ) return stPickInfo;
				if ( bPoliDrngDrain) return stPickInfo;

				stPickInfo.bFind     = true;
				stPickInfo.nBtmPlate = (int)EN_MAGA_ID.POLISH;
			}
			else if(bReqAlign)
			{
				//if (!bExistAlign) EPU.fn_SetErr(EN_ERR_LIST.ERR_0350, true);

				if (isDrngUnload) return stPickInfo; //Loading 동작 중인지...

				stPickInfo.bFind     = true;
				stPickInfo.nBtmPlate = (int)EN_MAGA_ID.LOAD;
			}

			//
			return stPickInfo; 
		}
        //---------------------------------------------------------------------------
        private PLATE_MOVE_INFO fn_GetPlatePlceInfo()
		{
			//Local Var.
			PLATE_MOVE_INFO stPlaceInfo = new PLATE_MOVE_INFO(false);
			fn_InitPlateInfo(ref stPlaceInfo);
			
			bool IsTestMode   = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE    );
			bool xExistMainZ  = fn_GetPltExtMI  (EN_MAGA_ID.SPINDLE, true );
			bool xExistLoad   = fn_GetPltExtMI  (EN_MAGA_ID.LOAD   , false);
			bool xExistPolish = fn_GetPltExtMI  (EN_MAGA_ID.POLISH , false);
			bool xExistClean  = fn_GetPltExtMI  (EN_MAGA_ID.CLEAN         );

			//
			if ( DM.TOOL.IsPlateEmpty()    ) return stPlaceInfo;
			if (!xExistMainZ && !IsTestMode) return stPlaceInfo;

			bool bMainZ1Align  = DM.TOOL.IsPlateStat((int)EN_PLATE_STAT.ptsAlign ); //Align
			bool bMainZ1Finish = DM.TOOL.IsPlateStat((int)EN_PLATE_STAT.ptsFinish); //Finish
			bool bMainZ1Clean  = DM.TOOL.IsPlateStat((int)EN_PLATE_STAT.ptsClean ); //Clean
			
			bool isDrngUnload   = SEQ_TRANS._bDrngUnload || SEQ_TRANS._bDrngLoad      ;
			bool bPoliUtilDrain = SEQ_POLIS._bDrngDrain  || SEQ_POLIS._bDrngSeqDrain  ; //LJH/200422 Polishing Drain Flag Add
			bool bPoliDrainReq  = SEQ_POLIS._bReqDrain ;
			bool bPoliUtilEmt   = SEQ_POLIS._bEmptyUitl;

			if (bMainZ1Finish) 
			{
				if (xExistLoad   && !IsTestMode) EPU.fn_SetErr(EN_ERR_LIST.ERR_0356);
				
				if (xExistLoad   ) return stPlaceInfo; 
				if (isDrngUnload ) return stPlaceInfo;

				stPlaceInfo.bFind     = true;
				stPlaceInfo.nBtmPlate = (int)EN_MAGA_ID.LOAD; 
			} 
			else if(bMainZ1Clean)
			{
				//if (xExistClean) return stPlaceInfo;
				if (xExistClean) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0359); return stPlaceInfo; }

				stPlaceInfo.bFind     = true;
				stPlaceInfo.nBtmPlate = (int)EN_MAGA_ID.CLEAN; 
			}
			else if(bMainZ1Align)
			{
				//if (xExistPolish) return stPlaceInfo;
				if ( xExistPolish  ) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0358); return stPlaceInfo; }
				if ( bPoliUtilDrain) return stPlaceInfo; //JUNG/200423
				if (!bPoliUtilEmt  ) return stPlaceInfo;  
				if ( bPoliDrainReq ) return stPlaceInfo;

				stPlaceInfo.bFind     = true;
				stPlaceInfo.nBtmPlate = (int)EN_MAGA_ID.POLISH; 

			}

			return stPlaceInfo;

        }
		//---------------------------------------------------------------------------
		public bool fn_GetPltExtMI(EN_MAGA_ID where, bool userset = false)
		{
			bool bManMode  = FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE );
			bool bTestMode = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);
			bool bAutoMode = FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
			bool bExist    = false; 

			switch(where)
			{
				
                case EN_MAGA_ID.TRANS   : bExist = IO.XV[(int)EN_INPUT_ID.xTRS_TransPlateExist];  break;
				case EN_MAGA_ID.SPINDLE : bExist = IO.XV[(int)EN_INPUT_ID.xSPD_PlateExistChk  ];  break;
			    case EN_MAGA_ID.LOAD    : bExist = IO.XV[(int)EN_INPUT_ID.xTRS_LoadPlateExist ];  break;
				//case EN_MAGA_ID.LOAD    : bExist = !DM.MAGA[(int)EN_MAGA_ID.LOAD].IsAllEmpty(); break; //JUNG/200527/임시로...
				
				case EN_MAGA_ID.POLISH  : bExist = !DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllEmpty(); break;
                case EN_MAGA_ID.CLEAN   : bExist = !DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllEmpty(); break;

			}

            //Auto
            if (!bAutoMode && FM.m_stMasterOpt.nPlateSkip == 1)
            {
                bExist = userset;
            }


            return bExist; 
		}
		//---------------------------------------------------------------------------
		public bool fn_IsExistPlatebyMI (EN_MAGA_ID where, bool userset = false)
		{
			bool bAutoMode = FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
			bool bExist    = false;

			switch (where)
			{

				case EN_MAGA_ID.TRANS   : bExist = IO.XV[(int)EN_INPUT_ID.xTRS_TransPlateExist]; break;
				case EN_MAGA_ID.SPINDLE : bExist = IO.XV[(int)EN_INPUT_ID.xSPD_PlateExistChk  ]; break;
				case EN_MAGA_ID.LOAD    : bExist = IO.XV[(int)EN_INPUT_ID.xTRS_LoadPlateExist ]; break;

				case EN_MAGA_ID.POLISH  : bExist = !DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllEmpty(); break;
				case EN_MAGA_ID.CLEAN   : bExist = !DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllEmpty(); break;
			}

			if (!bAutoMode && FM.m_stMasterOpt.nPlateSkip == 1)
			{
				bExist = userset;
			}

			return bExist;
		}

		//---------------------------------------------------------------------------
		private VISN_INSP_INFO fn_GetInspInfo()
		{
			VISN_INSP_INFO stVisn = new VISN_INSP_INFO(false);

			bool isTestMove     = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);
			bool xExsPlateLoad  = fn_GetPltExtMI(EN_MAGA_ID.LOAD, true);

			bool isExtAlign     = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsAlign   );
			bool isExtPreAlign  = DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllStat((int)EN_PLATE_STAT.ptsPreAlign);
			bool bRecipeOpen    = true; // SEQ._bRecipeOpen || isTestMove; 


			//
			if (bRecipeOpen)
			{
                if (isExtPreAlign)
                {
					if (!xExsPlateLoad) return stVisn;

					stVisn.bFind     = true;
                    stVisn.nBtmWhere = (int)EN_MAGA_ID.LOAD;
                }
                else if (isExtAlign)
				{
					stVisn.bFind     = true;
					stVisn.nBtmWhere = (int)EN_MAGA_ID.POLISH;
				}
			}

			return stVisn; 
		}
		//---------------------------------------------------------------------------
		private bool fn_CheckToolError()
		{
			bool bOk        =  true;
			bool isExtTool  = !DM.TOOL.IsEmpty();
			bool bToolExt   =  fn_IsExistTool(isExtTool);
			bool bRun       =  SEQ._bRun ;
			bool isToolUsed =  DM.TOOL.IsAllStat((int)EN_PIN_STAT.psUsed);

			if (m_bTESTMODE) return bOk;

			//Check Tool Info with Sensor (Call after Tool Exist Check )
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0360, bRun &&  isExtTool && !bToolExt && !isToolUsed)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0361, bRun && !isExtTool &&  bToolExt               )) bOk = false;


			return bOk; 
		}
		//---------------------------------------------------------------------------
		private bool fn_ToolPickCycle (ref bool Flag, ref TOOL_PICK_INFO pickinfo)
		{
            //"Polishing Tool Pick Pos" , EN_POSN_ID.FSP1_1
            //"Cleaning Tool Pick Pos"  , EN_POSN_ID.FSP2_1

            //"Polishing Tool Pick Pos.", EN_POSN_ID.User3, 3, EN_MOTR_ID.miSPD_Z);
            //"Cleaning Tool Pick Pos." , EN_POSN_ID.User6, 3, EN_MOTR_ID.miSPD_Z);

            bool r1, r2, r3;
			int    nFindCol, nFindRow;
			double dXPos, dYPos;

			//Use Direct Position or Set Position
			bool bUseDirectPos = FM.m_stMasterOpt.nUseDirPos == 1;
			bool isPolTool     = pickinfo.nBtmStor == siPolish;
			bool isClnTool     = pickinfo.nBtmStor == siClean ;

			m_iCmdX = isPolTool ? EN_COMD_ID.FindStep1 : EN_COMD_ID.FindStep2;
            m_iCmdZ = isPolTool ? EN_COMD_ID.User3     : EN_COMD_ID.User6    ;
			m_iCmdY = isPolTool ? EN_COMD_ID.FindStep1 : EN_COMD_ID.FindStep2;

            if (m_bTESTMODE)
            {
                MOTR[(int)EN_MOTR_ID.miSPD_Z].MP.dPosn[(int)EN_POSN_ID.Pick2] = 25;
				m_iCmdZ = EN_COMD_ID.Pick2;
            }


            //
            nFindCol = pickinfo.nFindCol;
			nFindRow = pickinfo.nFindRow;

			//
			dXPos = pickinfo.dXpos;
			dYPos = pickinfo.dYpos;

			//Tool Exist Check Cycle
			if (m_nToolPickStep < 0) m_nToolPickStep = 0;

            switch (m_nToolPickStep)
			{
				default:
					m_nToolPickStep = 0;
                    Flag = false;
                    return true;

                case 10:
                    if (SEQ._bStop)
                    {
                        m_nToolChkStep = 0;
                        Flag = false;
                        return true;
                    }
					
					Flag = true;
					m_nToolClampRetry = 0;

					fn_WriteSeqLog($"[START] PICK TOOL (R:{nFindRow},C:{nFindCol})");

					//
					SEQ_STORG.fn_SetToolPickInfo(pickinfo);

                    m_nToolPickStep++;
                    return false;

                case 11:
                    r1 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;
					m_nToolPickStep++;
                    return false;

                case 12:
                    if (SEQ._bStop)
                    {
                        m_nToolChkStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (bUseDirectPos)
					{
                        r1 = fn_MoveDirect (m_iMotrXId, dXPos);
						r2 = MOTR.CmprPos  (m_iBtmMotr, dYPos); //r2 = SEQ_STORG.fn_ReqMoveDirect(m_iBtmMotr, dYPos);
					}
                    else
					{
                        r1 = fn_MoveMotr   (m_iMotrXId, m_iCmdX, EN_MOTR_VEL.Normal, nFindCol, EN_FPOSN_INDEX.Index1);
						r2 = MOTR.CmprStep (m_iBtmMotr, nFindRow);
					}
					r3 = fn_MoveToolClamp(ccBwd);

					if (!r1 || !r2 || !r3) return false;

					m_nToolPickStep++;
                    return false;

				case 13:
                    if (SEQ._bStop)
                    {
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nToolChkStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrZId, m_iCmdZ); //Tool Pick Pos.
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nToolPickStep++;
                    return false;

                case 14:
                    if (SEQ._bStop)
                    {
                        m_nToolChkStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 200)) return false; 
                    
					r1 = fn_MoveToolClamp(ccFwd);
                    if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nToolPickStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;
					
					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1); 
                    if (!r1) return false;

					//
					if (fn_IsExistTool(true) || m_bTESTMODE)
					{
                        //Set Tool Check
                        DM.TOOL.SetNeedCheck(true);

                        //Set Force Check
                        DM.TOOL.SetForceCheck(false);

                        //Map Shift
                        DM.ShiftPinData(ref DM.STOR[pickinfo.nBtmStor].PINS[pickinfo.nFindRow, pickinfo.nFindCol], ref DM.TOOL.PINS[0]);

						//
						SEQ.fn_SaveWorkMap();

						fn_WriteSeqLog("[END] PICK TOOL");

						//
						if (m_bTESTMODE)
						{
							if(DM.STOR[pickinfo.nBtmStor].IsAllStat((int)EN_PIN_STAT.psEmpty))
							{
								if (isPolTool) DM.STOR[pickinfo.nBtmStor].SetTo((int)EN_PIN_STAT.psNewPol);
								if (isClnTool) DM.STOR[pickinfo.nBtmStor].SetTo((int)EN_PIN_STAT.psNewCln);
							}
						}
					}
					else
					{
						//Error
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0330); //Tool Pick Error
					}

                    Flag = false; 
					m_nToolPickStep = 0;
                    return true;
			}
		}
		//---------------------------------------------------------------------------
		private bool fn_ToolCheckCycle(ref bool Flag)
        {
			bool r1, r2;

			//Tool Exist Check Cycle
			if (m_nToolChkStep < 0) m_nToolChkStep = 0;

			switch (m_nToolChkStep)
			{
				default:
					m_nToolChkStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;
					
					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nToolChkStep ++; 
					return false;

                case 11:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					//Check Tool Map with sensor
					if(fn_CheckToolError())
				    {
						DM.TOOL.SetExistCheck(true ); 
						DM.TOOL.SetNeedCheck (false); 
					}
                    
					fn_WriteSeqLog("Tool Exist Check OK");

					Flag = false;
					m_nToolChkStep =0;
                    return true;

            }
        }
		//---------------------------------------------------------------------------
        private bool fn_ForceCheckCycle(ref bool Flag)
        {
			bool   r1, r2;
			double dPos, dSetForceValue;
			bool   bUseSysOption = false; //Force data = system option data
			int    nTotal = 7; //5회 Test

			//
			m_iCmdX = EN_COMD_ID.User5 ;
			m_iCmdZ = EN_COMD_ID.User5 ;

			//Tool Exist Check Cycle
			if (m_nForceChkStep < 0) m_nForceChkStep = 0;
			switch (m_nForceChkStep)
			{
				default:
					m_nForceChkStep = 0;
					Flag = false;
					return true;

				case 10:
                    if (SEQ._bStop)
                    {
						m_nForceChkStep = 0;
                        Flag = false;
                        return true;
                    }
					
					Flag = true;

					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					fn_WriteSeqLog("[START] Tool Force Check");

					System.Console.WriteLine("LDCBTM.fn_SetZero()");

					//
					fn_LoadcellInit();

					System.Console.WriteLine("LDCBTM.fn_SetZero() - 1");

					fn_WriteSeqLog("Reset Load Cell");

					//Buffer Check
					if (IO.fn_IsBuffRun(BFNo_14_FORCECHECK)) IO.fn_StopBuffer(BFNo_14_FORCECHECK);

					m_nCalCount = 0;

					m_tDelayTime.Clear();
					m_nForceChkStep++;
                    return false;

                case 11:
                    if (SEQ._bStop)
                    {
                        m_nForceChkStep = 0;
                        Flag = false;
                        return true;
                    }
					
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //X-Axis Tool Check Position
					r2 = SEQ_STORG.fn_IsForceOkPos() ? true : SEQ_STORG.fn_ReqMoveToolPickPos(); //JUNG/200826
                    if (!r1 || !r2) return false;

					m_nForceChkStep++;
                    return false;
				

				case 12:
                    if (SEQ._bStop)
                    {
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nForceChkStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrZId, m_iCmdZ); //Z-Axis Force Check Position
                    if (!r1) return false;

					//Test Mode
                    if (m_bTESTMODE)
                    {
						m_tDelayTime.Clear();
						m_nForceChkStep = 30;
                        return false;
                    }

                    m_tDelayTime.Clear();
					m_nForceChkStep++;
                    return false;
                
				case 13:
                    if (!m_tDelayTime.OnDelay(true, 200)) return false;

					//first load cell calculate
					if(m_nCalCount == 0 && FM.m_stMasterOpt.nUseCalForce == 1)
					{
						if (bUseSysOption)
						{
							//Use System Option
							dSetForceValue = FM.m_stSystemOpt.dTargetForce;
						}
						else
						{
							//JUNG/200716/Use Recipe Data 
							if (m_nPoliCnt < 0 || m_nPoliCnt > 9) m_nPoliCnt = 0; 
							dSetForceValue = vresult.stRecipeList[m_nPoliCnt].dForce;
						}

                        if (dSetForceValue >= 500) dSetForceValue = 250;
                        fn_SetDefaultForce(dSetForceValue); //

                        Console.WriteLine("Set Default DCOM Value : " + m_dForceRatio) ;
						fn_WriteLog("[UseCalForce]Set DCOM Value : " + m_dForceRatio, EN_LOG_TYPE.ltLot); //JUNG/201008

					}

					//
					dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(m_iCmdZ);
					r1 = IO.fn_ForceBufferRun(dPos);
                    if (!r1)
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0341); // Force Check ACS Buffer Error

						//
						Flag = false;
						m_nForceChkStep = 0;
                        return true;
                    }
                    
					m_tDelayTime.Clear();
                    m_nForceChkStep++;
                    return false;

                case 14:

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					if (!IO.fn_IsForceBuffRun()) return false;

					m_tDelayTime.Clear();
					m_nForceChkStep++;
                    return false;
				
				case 15:
                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun()) return false;

					m_tDelayTime.Clear();
					m_nForceChkStep++;
                    return false;

                case 16:

					if (!m_tDelayTime.OnDelay(true, 3000)) return false; //

					//Calibrate Force Value
					if (!fn_CalLoadCell())
                    {
                        if (m_nCalCount++ < nTotal)
                        {
                            fn_WriteLog(string.Format($"Force Check retry.[Count = {m_nCalCount}]"));
							Console.WriteLine (string.Format($"Force Check retry.[Count = {m_nCalCount}]"));

							IO.fn_SetOpenLoopOff();

							m_tDelayTime.Clear();
							m_nForceChkStep = 20;
							return false;
                        }
						else
                        {
							IO.fn_SetOpenLoopOff();

							//Error
							EPU.fn_SetErr(EN_ERR_LIST.ERR_0336);

							m_tDelayTime.Clear();
							
							m_nForceChkStep++;
                            return false;
                        }
                    }
					else
					{
						Console.WriteLine("Set DCOM Value : " + m_dForceRatio);
						fn_WriteLog(string.Format($"[Force Check] Set DCOM Value : {m_dForceRatio:F4} / Z-Pos:{GetEncPos_Z()}"),EN_LOG_TYPE.ltLot);

						//
						DM.TOOL.SetForceCheck(true);

                    }

                    IO.fn_SetOpenLoopOff();

					m_tDelayTime.Clear();
					m_nForceChkStep++;
                    return false;
				
				case 17:
					if (!m_tDelayTime.OnDelay(true, 500)) return false; //

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;


                    fn_WriteSeqLog("[END] Tool Force Check");

					Flag = false;
					m_nForceChkStep = 0;
                    return true;


				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Retry Force Check
                case 20:
					if(SEQ._bStop)
					{
						IO.fn_SetOpenLoopOff();
                        
						Flag = false;
                        m_nForceChkStep = 0;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 500)) return false; //

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;
                    
					m_nForceChkStep=12;
                    return false;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case 30:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false; //1sec Delay

                    MOTR[(int)m_iMotrZId].MP.dPosn[(int)EN_POSN_ID.Pick2] = 10;
                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Pick2);
                    if (!r1) return false;
                    
					m_tDelayTime.Clear();
                    m_nForceChkStep++;
                    return false;

                case 31:
                    if (!m_tDelayTime.OnDelay(true, 5 * 1000)) return false; //5sec Delay

                    //
                    DM.TOOL.SetForceCheck(true);

                    fn_WriteSeqLog("[END] Tool Force Check - TEST");

                    m_bDrngForceChk = false;
                    m_nForceChkStep = 0;
                    return true;
            }
        }
		//---------------------------------------------------------------------------
		private bool fn_PolishingCycle(ref bool Flag)
        {
			//"Cup Storage P/P Pos."   ,EN_POSN_ID.User15 , 3, EN_MOTR_ID.miSPD_X);
			//"Polishing Cup P/P Pos." ,EN_POSN_ID.User16 , 3, EN_MOTR_ID.miSPD_X);

			//"Polishing Pos."        , EN_POSN_ID.User1, 3, EN_MOTR_ID.miSPD_X);

			//"Polishing Start Pos.  ", EN_POSN_ID.User1, 3, EN_MOTR_ID.miSPD_Z );
			//"Polishing Cup P/P Pos.", EN_POSN_ID.User2, 3, EN_MOTR_ID.miSPD_Z1);

			//"Polishing Cup In/Out Pos.",EN_POSN_ID.User3, 3, EN_MOTR_ID.miPOL_Y); //Cup In/Out Position
			//"Cup Storage In/Out Pos."  ,EN_POSN_ID.User4, 3, EN_MOTR_ID.miPOL_Y); //Storage Cup In/Out Position

			//"Cup Storage P/P Pos."    , EN_POSN_ID.User5, 3, EN_MOTR_ID.miSPD_Z1);
			//"Polishing Cup Pick Pos." , EN_POSN_ID.User6, 3, EN_MOTR_ID.miSPD_Z1);
			//"Polishing Cup Place Pos.", EN_POSN_ID.User7, 3, EN_MOTR_ID.miSPD_Z1);

			//Recipe 항목 ACS Buffer에 Loading.
			//Recipe Step 별로 진행
			//

			/* -> ACS Buffer Data
            VEL(AXIS1) = X_SPEED
			ACC(AXIS1) = 1000
			DEC(AXIS1) = ACC(AXIS1)
			JERK(AXIS1) = 10000
			KDEC(AXIS1) = JERK(AXIS1)
			
			VEL(AXIS2) = 20
			ACC(AXIS2) = 200
			DEC(AXIS2) = ACC(AXIS2)
			JERK(AXIS2) = 8000
			KDEC(AXIS2) = JERK(AXIS2)
			*/

			EN_VISION_MODE visionmode = EN_VISION_MODE.Polishing;
			bool r1, r2, r3, r4;
			bool   IsTeseMode  = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);

			double dTotalCnt   = IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_POLISH_TotalCnt];
			double dXCnt       = IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_POLISH_XCnt    ];
			double dYCnt       = IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_POLISH_YCnt    ];

			double dStartXPos, dEndXPos, dStartYPos, dEndYPos, dPos, dYDistance, dXSpeed, dTOffset, dStartTH, dStartTI;
			int    nRPM, nPath, nPolCnt;
			int    nTotalStep    = vresult.nTotalStep ;
			bool   bUseCup       = FM.m_stSystemOpt.nUsePolishingCup == 1;
			int    nTotalClnCnt  = 4 ; //JUNG/200618/2-> 3 -> 4 변경(이동민프로/210114)

			//
			m_iCmdX = EN_COMD_ID.User1;
			m_iCmdZ = EN_COMD_ID.User1;

            if (m_nPolishStep < 0) m_nPolishStep = 0; 

			switch (m_nPolishStep)
			{
				default:
					Flag = false;
					m_nPolishStep   = 0;
					m_nPoliBackStep = 0;
					return true;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				case 5: //Return Tool Change Cycle
					Flag = true;

					//
					DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolish);

					//
					sLogMsg = string.Format($"Tool Change OK - STEP:{m_nPoliCnt}");
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

					m_nPoliBackStep = 0; 

					m_nPolishStep = 23;
                    return false;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case 10:
                    if (SEQ._bStop)
                    {
						m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    Flag = true; 

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					fn_WriteSeqLog("[START] POLISHING");
					fn_WriteLog   ("[START] POLISHING", EN_LOG_TYPE.ltLot);

					//Soft Limit Off
					IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false, false);

                    //Milling Start Time
                    m_sMillStartTime  = DateTime.Now.ToString();
                    m_sMillEndTime    = string.Empty;

                    //
					m_nPoliCnt       = 0;
					m_nPolCycle      = 0;
                    m_nTotalCycle    = 0;
					m_nTotalPathCnt  = 0;
					m_bOffDone       = false;
					m_bDCOMReset     = false;
					m_bReqResetGraph = true ;
					m_nPreCycleNo    = -1; 

					SEQ_TRANS.fn_MoveCylLoadCover(ccFwd);

					m_nPolishStep ++;
					return false;

				case 11: 
                    if (SEQ._bStop)
                    {
						m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

					//Move Polishing-Y Axis Work Position
					r1 = true; //SEQ_POLIS.fn_ReqBathWaitPos();
					r2 = SEQ_POLIS.fn_MoveCylClamp  (ccFwd); //JUNG/200515
					if (!r1 || !r2) return false; 

					m_tDelayTime.Clear();
                    
					//
                    if (m_bTESTMODE)
                    {
                        m_nPolishStep = 900; //PTP Mode
                        return false;
                    }

                    if (bUseCup) m_nPolishStep++;
					else         m_nPolishStep = 20;
					return false;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Cup In Cycle
				case 12:
                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

					//Check Cup Exist
					if(!SEQ_POLIS.fn_IsExistCup(true))
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0353); //Polishing Cup Loss Error

						m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr    (m_iMotrXId, EN_COMD_ID.User15); //Cup Position - X Axis
					r2 = fn_MoveCylClamp(ccBwd);
					r3 = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User4); //Cup Storage Position - Y Axis
					if (!r1 || !r2 || !r3) return false; 

					m_tDelayTime.Clear();
					m_nPolishStep++;
                    return false;

                case 13:
                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.User5)) return false; //Cup Storage P/P Pos

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 14:
                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 300)) return false;
					if (!fn_MoveCylClamp(ccFwd)) return false ;
                    
					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;
					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1)) return false; //wait Position - Z Axis

                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 16:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//JUNG/200612/Y-Axis Move --> X-Axis Move
					//JUNG/200807/X-Axis Move --> Y-Axis Move 
					if (!fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User16)) return false; //Polishing Cup P/P Position.
					if (!SEQ_POLIS.fn_ReqBathCupInOutPos()         ) return false; //Polishing Cup In/Out Position

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 17:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

                    //if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.User6)) return false; //Cup Place Position - Z Axis
					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.User7)) return false; //Cup Place Position - Z Axis

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 18:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

                    if (!fn_MoveCylClamp(ccBwd)) return false;

					//Set Polishing Cup Exist Flag???

                    
					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 19:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;
                    if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1)) return false; //wait Position - Z Axis

					//Check Exist Cup
					//if (fn_IsExistPlate(false))
					//{
					//    EPU.fn_SetErr(EN_ERR_LIST.ERR_0354); //Polishing Cup Unknown Error
					//    m_nPolishStep = 0;
					//    Flag = false;
					//    return true;
					//}

					fn_WriteLog("Cup In", EN_LOG_TYPE.ltLot);

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Main Cycle
				case 20:
                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

					if (!m_tDelayTime.OnDelay(true, 500)) return false; //

					//Clear Cycle No.
					m_nPolCycle   = 0;
					m_nTotalCycle = 0; 

					//Move polishing bath to ready position
					//r1 = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User1);
					//r2 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //Main-X Polishing Position
					//if (!r1 || !r2) return false;
                    
                    m_nPolishStep++;
                    return false;

				//Step Start 
				case 21:
                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Clear Cycle No.
                    m_nPolCycle   = 0;
                    m_nTotalCycle = 0;

                    //
                    if (m_nPoliCnt > 0)
                    {
                        // 1) Check Utility Fill condition
                        if (vresult.stRecipeList[m_nPoliCnt].nUseUtilFill == 1)
                        {
                            m_nPoliBackStep = 22;

                            m_nPolishStep = 2000;
                            return false;
                        }
                    }

                    m_nPolishStep++;
                    return false;

                case 22:
                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

					m_nPoliBackStep = 0;

					// 
					if (m_nPoliCnt > 0)
                    {
                        // 2) Milling Check
                        if (vresult.stRecipeList[m_nPoliCnt].nUseMilling == 1)
                        {
                            // 3) check tool change
                            if (vresult.stRecipeList[m_nPoliCnt].nUseToolChg == 1)
                            {
                                m_nPoliBackStep = 23;
                                
								m_nPolishStep = 3000;
                                return false;
                            }
                        }
                        else
                        {
							sLogMsg = string.Format($"{m_nPoliCnt} STEP - Milling Skip");
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

							m_nPolishStep = 36; //Drain Check Cycle
                            return false;
                        }
                    }
                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                //Step Start
                case 23: 
					if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

					m_nPoliBackStep = 0;

					//
					Console.WriteLine($"      dStartX  POS - {vresult.stRecipeList[m_nPoliCnt].dStartX}");
                    Console.WriteLine($"      dStartY  POS - {vresult.stRecipeList[m_nPoliCnt].dStartY}");
                    Console.WriteLine($"      dPosTH   POS - {vresult.stRecipeList[m_nPoliCnt].dPosTH }");
                    Console.WriteLine($"      dTilt    POS - {vresult.stRecipeList[m_nPoliCnt].dTilt  }");

                    
					//
					m_nTotalCycle = vresult.stRecipeList[m_nPoliCnt].nCycle   ;

					//Polishing Direction Option
					if(FM.m_stSystemOpt.nUsePoliOneDir == 1) //JUNG/200609
					{
						dStartXPos = vresult.stRecipeList[m_nPoliCnt].dStartX ; 
						dEndXPos   = vresult.stRecipeList[m_nPoliCnt].dEndX   ;
					}
					else
					{
						dStartXPos = m_nPolCycle % 2 == 1 ? vresult.stRecipeList[m_nPoliCnt].dEndX   : vresult.stRecipeList[m_nPoliCnt].dStartX ; //JUNG/200601
						dEndXPos   = m_nPolCycle % 2 == 1 ? vresult.stRecipeList[m_nPoliCnt].dStartX : vresult.stRecipeList[m_nPoliCnt].dEndX   ;
					}
								   									      
					dYDistance    = vresult.stRecipeList[m_nPoliCnt].dPitch      ;
					nPath         = vresult.stRecipeList[m_nPoliCnt].nPathCnt    ;
								   									      
					dStartYPos    = vresult.stRecipeList[m_nPoliCnt].dStartY     ;
					dEndYPos      = dStartYPos + (dYDistance * nPath * 2)        ; //vresult.stRecipeList[m_nCnt].dEndY ;
								   									      
					dXSpeed       = vresult.stRecipeList[m_nPoliCnt].dSpeed      ;
					dTOffset      = vresult.stRecipeList[m_nPoliCnt].dTiltOffset ;

					dStartTH      = vresult.stRecipeList[m_nPoliCnt].dPosTH      ;
					dStartTI	  = vresult.stRecipeList[m_nPoliCnt].dTilt       ;

					Console.WriteLine($"---------- MILL : {m_nPoliCnt + 1} / {nTotalStep} ---------- ");
					Console.WriteLine($"      dStartXPos  :{dStartXPos}");
                    Console.WriteLine($"      dEndXPos    :{dEndXPos  }");
                    Console.WriteLine($"      dStartYPos  :{dStartYPos}");
                    Console.WriteLine($"      dEndYPos    :{dEndYPos  }");
                    Console.WriteLine($"      dYDistance  :{dYDistance}");
                    Console.WriteLine($"      dXSpeed     :{dXSpeed   }");
					Console.WriteLine($"      Total Cycle :{m_nTotalCycle}");

					//Log
					fn_WriteLog($"---------- MILL : {m_nPoliCnt + 1} / {nTotalStep} ---------- ", EN_LOG_TYPE.ltLot);
					fn_WriteLog($"      StartXPos : {dStartXPos}"                         , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"      EndXPos   : {dEndXPos  }"                         , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"      StartYPos : {dStartYPos}"                         , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"      EndYPos   : {dEndYPos  }"                         , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"      YDistance : {dYDistance}"                         , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"      XSpeed    : {dXSpeed   }"                         , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"      StartTH   : {dStartTH  }"                         , EN_LOG_TYPE.ltLot);
                    fn_WriteLog($"      StartTI   : {dStartTI  }"                         , EN_LOG_TYPE.ltLot);
                    fn_WriteLog($"      Cycle Cnt : {m_nPolCycle+1}/{m_nTotalCycle}"      , EN_LOG_TYPE.ltLot);

					//Check Min/Max Position
					if (!fn_CheckPolishingPos(dStartXPos, dEndXPos, dStartYPos, dEndYPos, dYDistance, dXSpeed))
					{
						//Error
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0449); //Polishing Data Error

						m_nPolishStep = 0;
                        Flag = false;
                        return true;

                    }

                    //
					IO.DATA_EQ_TO_ACS[20] = dStartXPos;
                    IO.DATA_EQ_TO_ACS[21] = dEndXPos  ;
                    IO.DATA_EQ_TO_ACS[22] = dStartYPos;
                    IO.DATA_EQ_TO_ACS[23] = dEndYPos  ;
                    IO.DATA_EQ_TO_ACS[24] = dYDistance;  // Y Step
                    IO.DATA_EQ_TO_ACS[25] = 1         ;  // Direction
					IO.DATA_EQ_TO_ACS[26] = dXSpeed   ;  // X Speed

					IO.DATA_EQ_TO_ACS[27] = (int)EN_MOTR_ID.miPOL_Y;  //Axis-2

					IO.fn_DataEqToAcs();

					//Cleaning Data Setting
					//m_dDisX_PCtoPS =  dStartXPos            - MOTR[(int)EN_MOTR_ID.miSPD_X].MP.dPosn[(int)m_iCmdX         ] ; //Distance Polishing Center to Polishing Start
					//m_dDisY_PCtoPS = (dStartYPos - dTOffset)- MOTR[(int)EN_MOTR_ID.miPOL_Y].MP.dPosn[(int)EN_COMD_ID.User1] ;
                    //
					//Console.WriteLine($" dStartYPos = {dStartYPos } / dTOffset = {dTOffset} / POL POS = {MOTR[(int)EN_MOTR_ID.miPOL_Y].MP.dPosn[(int)EN_COMD_ID.User1]}");
					//Console.WriteLine($" m_dDisX_PCtoPS = {m_dDisX_PCtoPS } / m_dDisY_PCtoPS = {m_dDisY_PCtoPS}");

					//Data Setting
					MOTR[(int)EN_MOTR_ID.miSPD_X ].MP.dPosn[(int)EN_POSN_ID.CalPos] = dStartXPos;
					MOTR[(int)EN_MOTR_ID.miPOL_Y ].MP.dPosn[(int)EN_POSN_ID.CalPos] = dStartYPos;
					MOTR[(int)EN_MOTR_ID.miPOL_TH].MP.dPosn[(int)EN_POSN_ID.CalPos] = dStartTH  ;
                    MOTR[(int)EN_MOTR_ID.miPOL_TI].MP.dPosn[(int)EN_POSN_ID.CalPos] = dStartTI  ;

					m_tDelayTime.Clear();
					m_nPolishStep++;
                    return false;

				//Cycle Start
				case 24:
                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 500)) return false; //

					//Move to Vision Cal. Position
					r1 =           fn_MoveMotr   (m_iMotrXId         , EN_COMD_ID.CalPos);
					r2 = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TH, EN_COMD_ID.CalPos);
                    r3 = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y , EN_COMD_ID.CalPos);
					r4 = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TI, EN_COMD_ID.CalPos);
					if (!r1 || !r2 || !r3 || !r4) return false;
					
					//
					sLogMsg = string.Format($"[STEP:{m_nPoliCnt + 1}] Cycle Start : {m_nPolCycle+1} / {m_nTotalCycle}");
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

					Console.WriteLine(sLogMsg);

					m_bDCOMReset = false;

					m_nPolishStep++;
                    return false;
                
				case 25: //Main-Z Polishing Start Position
                    if (SEQ._bStop)
                    {
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.User1);
                    if (!r1) return false;

                    m_nPolishStep++;
                    return false;

				//Load Cell Calibration
				case 26:
                    
					//if (m_nPolCycle == 1 || FM.m_stSystemOpt.nUseAllForceChk == 1) //
                    //{
					//	//
					//	IO.fn_ForceBufferRun(MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z));  //IO.fn_RunBuffer(BFNo_14_FORCECHECK, true);
					//
                    //    //
					//	fn_WriteSeqLog("Start Load cell Calibration");
					//
					//	m_nPolishStep = 30;
                    //    return false;
                    //}

                    m_nPolishStep++;
                    return false;


				case 27:
					//Run Force Buffer
					dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(EN_COMD_ID.User1);
                    
					r1 = IO.fn_ForceBufferRun(dPos); 
					if (!r1)
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0341); // Force Check ACS Buffer Error

                        //
                        Flag = false;
                        m_nPolishStep = 0;
                        return true;
                    }
					sLogMsg = string.Format($"Force Check Start Pos: {dPos}");
					Console.WriteLine(sLogMsg);
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

					MOTR[(int)EN_MOTR_ID.miSPD_Z].MP.dPosn[(int)EN_POSN_ID.CalPos] = dPos; //JUNG/200605

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 28:
                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    if (!IO.fn_IsForceBuffRun()) return false;

					Console.WriteLine("Force Buff Start");

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 29:
                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun()) return false;

                    Console.WriteLine("Force Buff End");

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 30:
                    if (SEQ._bStop) //JUNG/200827/
                    {
						IO.fn_SetOpenLoopOff();

						m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 3000)) return false;

                    //Check Buffer State
                    if (IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_45_CheckForceBuffer] == 1)
                    {
                        //EPU.fn_SetErr(EN_ERR_LIST.ERR_0344); // Force Check ACS Buffer Error
                        //
                        ////
                        //Flag = false;
                        //m_nPolishStep = 0;
                        //return true;
                        
						sLogMsg = string.Format($"Check Buffer State : ERR_0344ERR_0344");
                        fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    }

                    sLogMsg = string.Format($"Milling Start Pos(Z): {GetEncPos_Z()}");
                    Console.WriteLine(sLogMsg);
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    //Set soft limit
                    if (FM.m_stSystemOpt.nUseSoftLimit == 1)
                    {
                        IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, true);

                    }
                    else
                    {
						IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z);
					}
					
					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                case 31:
                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					nPolCnt = m_nPolCycle;

					//Spindle Run
					nRPM = (int)vresult.stRecipeList[m_nPoliCnt].dRPM;
					if (FM.m_stSystemOpt.nUseSpdDirBwd == 1 )
					{
						fn_SetSpindleRun(nRPM, nPolCnt % 2 == 1 ? false : true); //JUNG/200603/
						sLogMsg = string.Format("Spindle Run : {1} / DIR = {0}", nPolCnt % 2 == 1 ? "FWD" : "BWD", nRPM);
					}
                    else if (FM.m_stSystemOpt.nUseSpdDirOnlyFWD == 1)
                    {
                        fn_SetSpindleRun(nRPM, false); //JUNG/200604/Only FWD
                        sLogMsg = string.Format("Spindle Run : {1} / DIR = {0}", "FWD", nRPM);
                    }
                    else if (FM.m_stSystemOpt.nUseSpdDirOnlyBWD == 1)
                    {
                        fn_SetSpindleRun(nRPM, true); //JUNG/200604/Only BWD
                        sLogMsg = string.Format("Spindle Run : {1} / DIR = {0}", "BWD", nRPM);
                    }
                    else
                    {
						fn_SetSpindleRun(nRPM, nPolCnt % 2 == 1 ? true : false); //JUNG/200601/Check Odd number
						sLogMsg = string.Format("Spindle Run : {1} / DIR = {0}", nPolCnt % 2 == 1 ? "BWD" : "FWD", nRPM);
					}
					
					fn_WriteLog(sLogMsg);
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

					Console.WriteLine(sLogMsg);

					if (!IO.fn_RunBuffer(BFNo_13_MILLING, true))
                    {

                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0342); //

                        //
                        Flag = false;
                        m_nPolishStep = 0;
                        return true;
                    }
					
                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;


                case 32:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					if (!IO.fn_IsBuffRun(BFNo_13_MILLING)) return false;

					m_bOffDone = false; 
					m_tDelayTime.Clear();
					m_nPolishStep++;
                    return false;

                case 33:
					//if (!m_tDelayTime.OnDelay(true, 1000)) return false;
                   
					//Check Soft limit
                    if (FM.m_stSystemOpt.nUseSoftLimit == 1 && MOTR[(int)m_iMotrZId].GetSRL())
                    {
                        //Milling Buffer End
						fn_MillBuffEnd();

                        sLogMsg = "[POLISHING] Soft Limit Error!!";
                        fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                        Console.WriteLine(sLogMsg);

						m_tDelayTime.Clear();

						EPU.fn_SetErr(EN_ERR_LIST.ERR_0335); //SPINDLE - Soft Limit Error

						//
						//Flag = false;
						m_nPolishStep = 1000; //JUNG/200907
                        return false;
                    }

                    //Spindle Off timing Check
                    if (!m_bOffDone && fn_IsSpindleOffCnt())
					{
						m_bOffDone = true; 
						fn_SetSpindleRun(swOff);

						sLogMsg = string.Format($"Spindle Off - Mill Cycle : {fn_GetCurrMillCnt()} / Total Cycle : {fn_GetTotalMillCnt()}");
						fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

						Console.WriteLine(sLogMsg);
					}

					//JUNG/200610/DCOM Reset
					if(FM.m_stMasterOpt.nUseDCOMReset == 1 && FM.m_stMasterOpt.dDCOMRatio > 0 && !m_bDCOMReset && fn_IsDCOMReSetCnt())
					{
						m_bDCOMReset = true;
						
						IO.fn_ResetDCOMValue();
					
						Console.WriteLine("DCOM Reset");
					}

					//JUNG/200610/LOG
					fn_MillingLog(m_nPolCycle, m_nPoliCnt);

                    if (IO.fn_IsBuffRun(BFNo_13_MILLING)) return false;

                    //
					IO.fn_ForceBufferStop();
                    IO.fn_SetOpenLoopOff ();
                    fn_SetSpindleRun     (swOff);

                    //
                    fn_WriteLog(string.Format($"[MILL END] [OpenLoop On] Z-Pos = {GetEncPos_Z()}"));
					Console.WriteLine("[MILL END] [OpenLoop On] Z-Pos = " + GetEncPos_Z());

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

				case 34:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//JUNG/200605/
					if (!fn_MoveMotr(m_iMotrZId, EN_COMD_ID.CalPos)) return false;

					fn_WriteLog(string.Format($"[MILL END][OpenLoop Off] Z-Pos = {GetEncPos_Z()}"), EN_LOG_TYPE.ltLot);
					Console.WriteLine("Buff Stop & OpenLoop Off / Z-Pos = " + GetEncPos_Z());

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

				//Check Cycle End 
				case 35:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_SetSpindleRun(swOff); //JUNG/200605/추가
                    r2 = fn_MoveMotr     (m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    //
					if (FM.m_stSystemOpt.nUseSoftLimit == 1)
                    {
                        //Limit Off
                        IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false);

                        fn_WriteLog("[POL] SoftLimit Off", EN_LOG_TYPE.ltLot);
                        Console.WriteLine("[POL] SoftLimit Off");
                    }

                    sLogMsg = string.Format($"[STEP:{m_nPoliCnt + 1}] Cycle End : {m_nPolCycle + 1} / {m_nTotalCycle}");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    Console.WriteLine(sLogMsg);

                    //Check Cycle
                    if (m_nPolCycle+1 < m_nTotalCycle)
					{
						m_nPolCycle++; //JUNG/200909/

						sLogMsg = string.Format($"Next Cycle: {m_nPolCycle+1}");
                        fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                        m_bDCOMReset = false; 

						m_tDelayTime.Clear();
						m_nPolishStep = 23; //JUNG/200615
						return false;
					}

                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

				// 4) Drain Check
				case 36:
					
					//
					if(m_nPoliCnt >= 0)
                    {
						if (vresult.stRecipeList[m_nPoliCnt].nUseUtilDrain == 1)
                        {
							m_nPoliBackStep = 37;

							m_nPolishStep = 4000;
                            return false;
                        }
                    }
                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                // 5) Image Save Check
                case 37:
					m_nPoliBackStep = 0;

					if (m_nPoliCnt >= 0)
                    {
						if (vresult.stRecipeList[m_nPoliCnt].nUseImage == 1)
						{
                            m_nPoliBackStep = 38;

                            m_nPolishStep = 5000;
                            return false;
                        }
                    }
                    
					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

                //Check Step End 
                case 38:

					m_nPoliBackStep = 0;

					//Check Step
					if (m_nPoliCnt+1 < nTotalStep) //Check total cycle count
                    {
						m_nPoliCnt++;

						sLogMsg = string.Format($"Next Step : {m_nPoliCnt + 1}");
						fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
						
						Console.WriteLine(sLogMsg);

						m_bDCOMReset = false;

						m_tDelayTime.Clear();

						m_nPolishStep = 21;
                        return false;
                    }

					//SEQ_POLIS.fn_ReqBathWaitPos(); //for plate pick

					//SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y , bUseCup ? EN_COMD_ID.User3 : EN_COMD_ID.Wait1);
					
					SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TH, EN_COMD_ID.Wait1);
					SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TI, EN_COMD_ID.Wait1);

					SEQ_POLIS.fn_ReqMoveUtilChkPosn(); //Y-Axis

					m_nPolishStep++;
					return false;

                case 39:

					//Set Drain
					SEQ_POLIS.fn_SetDrain();

                    m_tEndPolishing = DateTime.Now;
                    m_sMillEndTime  = DateTime.Now.ToString();

					fn_WriteLog("Milling End", EN_LOG_TYPE.ltLot);

					//
					if (m_bTESTMODE)
					{
                        m_nPolishStep = 46;
                        return false;
                    }

                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;
				
				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//JUNG/200503/DI Supply & Drain
				case 40:
					if (SEQ_POLIS._bDrngSeqDrain) return false;

					m_nWaterClenCnt = 0; 

					m_nPolishStep++;
                    return false;

                case 41:
					//X, Y Util Check Position
					r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User9);
					r2 = SEQ_POLIS.fn_ReqMoveUtilChkPosn();
					if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nPolishStep++;
                    return false;

				case 42:
                    if (!m_tDelayTime.OnDelay(true, 1000 * 5)) return false;

					fn_WriteLog("Polishing Clean DI ON", EN_LOG_TYPE.ltLot);

					//
					SEQ_POLIS.fn_SetDIWaterValve(vvOpen);

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;
				
				case 43:
                    if (SEQ._bStop)
                    {
						SEQ_POLIS.fn_SetDrain();

						m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    m_tDelayTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nUtilMaxTime); //TimeOut
					if (m_tDelayTime.Out)
					{
						SEQ_POLIS.fn_SetDIWaterValve();
						SEQ_POLIS.fn_SetDrain();

						fn_WriteLog("Polishing Clean DI Time Out", EN_LOG_TYPE.ltLot);

						EPU.fn_SetErr(EN_ERR_LIST.ERR_0416); //Polishing DI Water Supply Error

                        m_nPolishStep = 60;
                        return false;
                    }

					//Supply Utility during check UT Level Sensor
					if (IO.fn_IsUTLevelDone())
                    {
                        //Supply Off
                        SEQ_POLIS.fn_SetDIWaterValve();

						fn_WriteLog("Polishing Clean DI OFF", EN_LOG_TYPE.ltLot);

						m_tDelayTime.Clear();
						m_nPolishStep++;
                        return false;
                    }

					//Supply On
					SEQ_POLIS.fn_SetDIWaterValve(vvOpen);

					SEQ_POLIS.fn_SetUtilState(EN_UTIL_STATE.Exist);

                    return false;

                case 44:
					//
					SEQ_POLIS.fn_SetDIWaterValve();
					if (!m_tDelayTime.OnDelay(true, 1000 * 3)) return false;

                    SEQ_POLIS.fn_SetDrain();

                    m_nPolishStep++;
                    return false;
				
				case 45:
					if (SEQ_POLIS._bDrngSeqDrain) return false;

					//JUNG/200528/Water Clean 2회 요청으로...
					if (++m_nWaterClenCnt < nTotalClnCnt)
					{
                        m_nPolishStep = 41;
                        return false;
                    }

                    m_nPolishStep++;
                    return false;
				
				case 46:
                    //
                    if (bUseCup)
                    {
                        m_nPolishStep = 50;
                        return false;
                    }

                    //Map Change
                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsClean);

                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

					//
					SEQ_POLIS.fn_SetUtilState(EN_UTIL_STATE.Empty);

					fn_WriteLog("[END] POLISHING", EN_LOG_TYPE.ltLot);

					Flag = false; 
                    m_nPolishStep = 0;
                    return true;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Cup Out Cycle
				case 50:
                    
					r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User16); //Polishing Cup P/P Position.
                    r2 = SEQ_POLIS.fn_ReqBathCupInOutPos(); //Polishing Cup In/Out Position
					r3 = fn_MoveCylClamp(ccBwd); 
					if (!r1 || !r2 || !r3) return false;

                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

				case 51:
                    
					if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.User6)) return false; //Polishing Cup Storage Pick Pos

					m_tDelayTime.Clear();
					m_nPolishStep++;
					return false;

				case 52:

					if (!m_tDelayTime.OnDelay(true, 300)) return false;
					if (!fn_MoveCylClamp(ccFwd)) return false;

					m_tDelayTime.Clear();
					m_nPolishStep++;
					return false;

				case 53:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;
					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1)) return false; //wait Position - Z Axis

					m_tDelayTime.Clear();
					m_nPolishStep++;
					return false;

				case 54:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//JUNG/200612/Y-Axis move --> X-Axis move
					if (!SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User4)) return false; //Cup Storage Position - Y Axis
					if (!fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User15)                    ) return false; //Cup Position - X Axis

					m_tDelayTime.Clear();
					m_nPolishStep++;
					return false;

				case 55:

					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.User5)) return false; //Storage Cup P/P Position - Z Axis

					m_tDelayTime.Clear();
					m_nPolishStep++;
					return false;

				case 56:
					if (!m_tDelayTime.OnDelay(true, 100)) return false;

					if (!fn_MoveCylClamp(ccBwd)) return false;

					m_tDelayTime.Clear();
					m_nPolishStep++;
					return false;

				case 57:
					if (!m_tDelayTime.OnDelay(true, 100)) return false;
					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1)) return false; //wait Position - Z Axis

					fn_WriteLog("Cup Out", EN_LOG_TYPE.ltLot);

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;

				case 58:

                    //Map Change
                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsClean);

                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

                    //
                    SEQ_POLIS.fn_SetUtilState(EN_UTIL_STATE.Empty);


                    if (FM.m_stSystemOpt.nUseSoftLimit == 1)
					{
						//Limit Off
						IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false);
					}

					//
					if (bUseCup)
                    {
						if(!SEQ_POLIS.fn_IsExistCup(true))
                        {
							EPU.fn_SetErr(EN_ERR_LIST.ERR_0353); //Polishing Cup Loss Error
						}
                    }

                    Flag = false;
                    m_nPolishStep = 0;
                    return true;
				
				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				case 60:

                    //Map Change
                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsClean);

                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

                    if (FM.m_stSystemOpt.nUseSoftLimit == 1)
					{
						//Limit Off
						IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false);
					}

					//
					if (bUseCup)
                    {
						if(!SEQ_POLIS.fn_IsExistCup(true))
                        {
							EPU.fn_SetErr(EN_ERR_LIST.ERR_0353); //Polishing Cup Loss Error
						}
                    }

                    Flag = false;
                    m_nPolishStep = 0;
                    return true;



                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Force Check Cycle
                case 150:
					if (!IO.fn_IsBuffRun(BFNo_14_FORCECHECK)) return false;

					m_tDelayTime.Clear();
					m_nPolishStep++;
                    return false;

                case 151:
					if (!m_tDelayTime.OnDelay(true, 3000)) return false; //wait 3sec


					//Check Force
					if(fn_CalLoadCell(true))
					{
                        m_nPolishStep++;
                        return false;

                    }
					else
					{
						//force calibration - Value Change


						IO.fn_ForceBufferStop();
						m_tDelayTime.Clear();
						m_nPolishStep = 40;
                        return false;

                    }

                case 152:
					
					//
					fn_WriteSeqLog("Load cell Calibration End");

					m_nPolishStep = 19;
                    return false;
				
				case 153:
					//Wait Position
					if (!fn_MoveMotr(EN_MOTR_ID.miSPD_Z, EN_COMD_ID.Wait1)) return false;
                    m_nPolishStep = 17;
                    return false;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Utility fill cycle
                case 2000:
					//X, Y Util Check Position
					r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User9);
					r2 = SEQ_POLIS.fn_ReqMoveUtilChkPosn();
					if (!r1 || !r2) return false;

					fn_WriteLog(string.Format($"Start Utility supply (TYPE : {vresult.stRecipeList[m_nPoliCnt].nUtilType})"), EN_LOG_TYPE.ltLot);

					m_tDelayTime.Clear();
					m_nPolishStep++;
                    return false;

				case 2001:
                    if (!m_tDelayTime.OnDelay(true, 1000 * 3)) return false; //Wait

					fn_WriteLog(string.Format("Valve On at Polishing cycle"), EN_LOG_TYPE.ltLot);

					m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;
				
				case 2002:
					m_tDelayTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nUtilMaxTime); //
					if (m_tDelayTime.Out)
					{
						fn_WriteLog(string.Format("Valve TimeOut at Polishing cycle"), EN_LOG_TYPE.ltLot);

						SEQ_POLIS.fn_StopAllUtil();

						EPU.fn_SetErr(EN_ERR_LIST.ERR_0416); //Utility Supply Error

						m_nPolishStep = 0;
                        return true;
                    }

                    //Supply Utility during check UT Level Sensor
                    if (IO.fn_IsUTLevelDone())
					{
						fn_WriteLog(string.Format("Valve Off at Polishing cycle"), EN_LOG_TYPE.ltLot);

						//Supply Off
						SEQ_POLIS.fn_StopAllUtil();

						m_nPolishStep++;
                        return false;
                    }

					SEQ_POLIS.fn_SupplyUtil((EN_UTIL_KIND)vresult.stRecipeList[m_nPoliCnt].nUtilType);

					SEQ_POLIS.fn_SetUtilState(EN_UTIL_STATE.Exist);

					return false;
				
				case 2003:

					SEQ_POLIS.fn_StopAllUtil();

					fn_WriteLog("End Utility Supply", EN_LOG_TYPE.ltLot);

					m_nPolishStep = m_nPoliBackStep;
                    return false;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Tool Change Cycle
                case 3000:

                    //Discard tool & Pick new tool
                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolishWait);

                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

					//
					fn_WriteLog("Start Tool Change", EN_LOG_TYPE.ltLot);
					
                    Flag = false;
                    m_nPolishStep = 0;
                    return true;


				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Drain Cycle
				case 4000:

                    if (!SEQ_POLIS.fn_ReqBathWaitPosTHTI()) return false;

                    //
                    SEQ_POLIS.fn_SetDrain();

                    sLogMsg = string.Format("Drain Start");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    m_nPolishStep++;
                    return false;
				
				case 4001:
					
					if (SEQ_POLIS._bDrngSeqDrain) return false;

                    sLogMsg = string.Format("Drain End");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

					//
					SEQ_POLIS.fn_SetUtilState(EN_UTIL_STATE.Empty);

					m_nPolishStep = m_nPoliBackStep ;
                    return false;


                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Image Save
                case 5000:
				
                    r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User12);
					r2 = SEQ_POLIS.fn_ReqBathImagePos();

					if (!r1 || !r2 ) return false;
					
					sLogMsg = string.Format("Start Image Save");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    m_nPolishStep++;
                    return false;
				
				case 5001:
					visionmode = EN_VISION_MODE.Polishing;
					if (vresult.stRecipeList[m_nPoliCnt].nUseEPD == 1)
						visionmode = EN_VISION_MODE.Inspection;

					r1 = fn_MoveCylLensCvr(ccOpen);
					r2 = fn_MoveCylIR     (ccOpen, (int)visionmode);

					if (!r1 || !r2) return false;
					
					// Camera Parameter Set 추가 할것.
					g_VisionManager._CamManager.fn_SetExposure(g_VisionManager.CurrentRecipe.CameraExposureTime[(int)visionmode]);
					g_VisionManager._CamManager.fn_SetGain    (g_VisionManager.CurrentRecipe.CameraGain		   [(int)visionmode]);
					
					g_VisionManager.fn_SetLightValue(swOn, visionmode); //201118/Light On

					m_tDelayTime.Clear();
					m_nPolishStep++;
                    return false;

                case 5002:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false; //Delay for stable
					g_VisionManager._AlginMainCtrl.fn_ClearResult();
					//g_VisionManager.delUpdateMainresultClear?.Invoke();
					// Vision Grab & Image Save
					if (!g_VisionManager.fn_GrabImageSave(LOT._sLotNo)) //
					{
						//EPU.fn_SetErr(EN_ERR_LIST.ERR_0453); //Vision Grab & Save Error
						//Flag = false;
						//m_nPolishStep = 0;
						//return true;

						fn_WriteLog("Polishing Step Image Save Fail!!");
					}

                    m_tDelayTime.Clear();
                    m_nPolishStep++;
                    return false;
				
				case 5003:
					if (!m_tDelayTime.OnDelay(true, 500)) return false; //Delay

					r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR     (ccClose);
					if (!r1 || !r2 ) return false;

					r3 = g_VisionManager.fn_SetLightValue(swOff); //Light Off
					if (!r3) return false;

                    sLogMsg = string.Format("End Image Save");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);


					//Check EPD
					if (vresult.stRecipeList[m_nPoliCnt].nUseEPD == 1)
					{
						if (m_nPoliCnt + 1 < nTotalStep)
						{
							//Check Result
							// fn_EPDResult 파라메터가 true일 경우 EPD 결과값은 항상 false.
							if (g_VisionManager.fn_EPDResult(stvisionResult.pntModel, FM.m_stMasterOpt.nEPDOnlyMeasure == 1 ? true : false))
							{
								fn_WriteLog($"EPD OK. CurrCnt : {m_nPoliCnt}", EN_LOG_TYPE.ltLot);
								//End Polishing
								m_nPolishStep = 39;
								return false;
							}
							else
								fn_WriteLog($"EPD NG, Continue Work. CurrCnt :{m_nPoliCnt}", EN_LOG_TYPE.ltLot);
						}
					}

					m_nPolishStep = m_nPoliBackStep;
					return false;


                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //TEST Mode 
                case 900:
					if (SEQ._bStop)
					{
						m_nPolishStep = 0;
						Flag = false;
						return true;
					}

					m_nTestIndex = 0;
					
					m_nPolishStep++;
                    return false;

                case 901:
                    r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User1);
                    r2 = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User1);
					if (!r1 || !r2) return false;

					m_nPolishStep++;
                    return false;

                case 902:
                    if (SEQ._bStop)
                    {
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Check Soft limit
                    if (FM.m_stSystemOpt.nUseSoftLimit == 1 && MOTR[(int)m_iMotrZId].GetSRL())
                    {
                        //Milling Buffer End
                        fn_MillBuffEnd();

                        sLogMsg = "[POLISHING] Soft Limit Error!!";
                        fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                        Console.WriteLine(sLogMsg);
                      
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0335); //SPINDLE - Soft Limit Error

						//
						m_tDelayTime.Clear();

						m_nPolishStep = 1000; //JUNG/200907
                        return false;
                    }
                    
					//
					MOTR[(int)m_iMotrZId].MP.dPosn[(int)EN_POSN_ID.CalPos] = 20;

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.CalPos);
					if (!r1) return false;

					m_nPolishStep++;
					return false;

				case 903:

                    if (SEQ._bStop)
                    {
                        m_nPolishStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!fn_SetSpindleRun(3000)) return false;

					MOTR[(int)m_iMotrXId].MP.dPosn[(int)EN_POSN_ID.User20] = MOTR[(int)m_iMotrXId].GetEncPos() - 3;
					MOTR[(int)m_iMotrXId].MP.dPosn[(int)EN_POSN_ID.User21] = MOTR[(int)m_iMotrXId].GetEncPos() + 3;

					m_nPolishStep++;
					return false;

				case 904:
					r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User20);
					if (!r1) return false;
					m_nPolishStep++;
					return false;

				case 905:
					r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User21);
					if (!r1) return false;

                    if (m_nTestIndex++ < 20)
                    {
                        m_nPolishStep = 904;
                        return false;
                    }

                    m_nPolishStep++;
					return false;

				case 906:

                    r1 = fn_SetSpindleRun(swOff);
					r2 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

                    m_nPolishStep++;
					return false;

				case 907:

					//Map Change
					DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsClean);

					DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

					fn_WriteSeqLog("[END] POLISHING (PLATE:CLEAN/TOOL:USED) - TEST");

					Flag = false;
					m_nPolishStep = 0;
					return true;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Soft Limit 발생 시 Recovery
                case 1000:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false; //Delay

					//Map Change
					DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsFinish);
					DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

					fn_WriteLog("Soft Limit Recovery - Move Wait Position / Map Change");

					fn_SetSpindleRun(swOff);
                    fn_MoveMotr     (m_iMotrZId, EN_COMD_ID.Wait1) ;

                    m_nPolishStep ++;
                    return false;
                
				case 1001:
					if (!m_tDelayTime.OnDelay(true, 5000)) return false; //Delay

					r1 = fn_SetSpindleRun(swOff);
                    r2 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    EPU.fn_SetErr(EN_ERR_LIST.ERR_0335); //SPINDLE - Soft Limit Error

					//
					fn_MillBuffEnd();

					//JUNG/201021
					MOTR.Stop(m_iMotrXId);
					MOTR.Stop(EN_MOTR_ID.miPOL_Y);


					Flag = false;
                    m_nPolishStep = 0; //JUNG/200907
                    return true;

            }

        }
		//---------------------------------------------------------------------------
		private bool fn_CleaningCycle (ref bool Flag)
        {

			//"Cleaning Pos"          , EN_POSN_ID.User2, 3, EN_MOTR_ID.miSPD_X);
			//"Cleaning Start Pos."   , EN_POSN_ID.User2, 3, EN_MOTR_ID.miSPD_Z);
			//"Work Pos."             , EN_POSN_ID.User1, 3, EN_MOTR_ID.miCLN_Y);

			//
			bool   r1, r2, r3, r4;
			bool   IsTeseMode  = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);

			double dTotalCnt   = IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_POLISH_TotalCnt];
			double dXCnt       = IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_POLISH_XCnt    ];
			double dYCnt       = IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_POLISH_YCnt    ];

			double dStartXPos, dEndXPos, dStartYPos, dEndYPos, dPos, dYDistance, dXSpeed, dXDis, dYDis, dEncX, dEncY;
			int    nRPM, nPath  ;
			int    nTotalStep = g_VisionManager.CurrentRecipe.CleaningCount;
			bool   bTESTData  = false;

            //
            m_iCmdX = EN_COMD_ID.User2;
            m_iCmdZ = EN_COMD_ID.User2;


            if (m_nCleanStep < 0) m_nCleanStep = 0;

			switch (m_nCleanStep)
			{
				default:
					Flag = false;
					m_nCleanStep = 0;
					return true;
				
				case 10:
                    if (SEQ._bStop)
                    {
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    Flag = true; 

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					fn_WriteSeqLog("[START] CLEANING");
					fn_WriteLog("[START] CLEANING", EN_LOG_TYPE.ltLot);

					//Cleaning Start Time
					m_sCleanStartTime = DateTime.Now.ToString();
                    m_sCleanEndTime   = string.Empty;

					m_nCleanCnt = 0;

					SEQ_TRANS.fn_MoveCylLoadCover(ccFwd);

					m_nCleanStep++;
					return false;

				case 11: 
                    if (SEQ._bStop)
                    {
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Move Cleaning-Y Axis Work Position
                    SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y , EN_COMD_ID.User1);

					//Move Cleaning - TH Axis Align Position
					SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_R, EN_COMD_ID.CalPos);

					m_tDelayTime.Clear();

					m_nCleanStep++;
					return false;

				case 12:
                    if (SEQ._bStop)
                    {
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

					if (!m_tDelayTime.OnDelay(true, 500)) return false; //3sec

                    //Move Cleaning bath to ready position
                    r1 = SEQ_CLEAN.fn_ReqMoveMotr (EN_MOTR_ID.miCLN_Y, EN_COMD_ID.User1 );
					r2 =           fn_MoveMotr    (m_iMotrXId, m_iCmdX                  ); //Main-X Cleaning Position
					r3 = SEQ_CLEAN.fn_ReqMoveMotr (EN_MOTR_ID.miCLN_R, EN_COMD_ID.CalPos);
					r4 = SEQ_CLEAN.fn_MoveCylClamp(ccFwd); //JUNG/200515
					if (!r1 || !r2 || !r3 || !r4) return false;

                    //Test Mode
                    if (m_bTESTMODE)
                    {
                        m_nCleanStep = 50; //PTP Mode 
                        return false;
                    }

                    m_nCleanStep++;
                    return false;

                case 13: 
                    if (SEQ._bStop)
                    {
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

					//Center 기준으로 Polishing Gap 설정
					dEncX = MOTR[(int)m_iMotrXId        ].GetEncPos();
					dEncY = MOTR[(int)EN_MOTR_ID.miCLN_Y].GetEncPos();

					//Cleaning Data Setting
					//X
					dXDis = g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].XDistance / 2.0;

					dStartXPos = dEncX - dXDis + g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].XOffset;
					dEndXPos   = dEncX + dXDis + g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].XOffset;

					dYDistance = g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].YPitch   ; 
					nPath      = g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].PathCount;

					//Y
					dYDis = (dYDistance * nPath * 2) / 2.0;

					dStartYPos = dEncY + dYDis + g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].YOffset;
					dEndYPos   = dEncY - dYDis + g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].YOffset;

					dXSpeed    = g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].XSpeed;


					//Check position
					if (!fn_CheckCleanPos(dStartXPos, dEndXPos, dStartYPos, dEndYPos))
					{
						//
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0437);
                        
						//
                        Flag = false;
                        m_nCleanStep = 0;
                        return true;
                    }

					//JUNG/200527/Check Data
					if(nPath  < 1 || dXSpeed < 1 || dYDistance == 0)
					{
                        //
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0438);

                        //
                        Flag = false;
                        m_nCleanStep = 0;
                        return true;

                    }
                    

					IO.DATA_EQ_TO_ACS[20] = dStartXPos;
                    IO.DATA_EQ_TO_ACS[21] = dEndXPos  ;
                    IO.DATA_EQ_TO_ACS[22] = dStartYPos;
                    IO.DATA_EQ_TO_ACS[23] = dEndYPos  ;
                    IO.DATA_EQ_TO_ACS[24] = dYDistance;  // Y Step
                    IO.DATA_EQ_TO_ACS[25] = 1         ;  // Direction
					IO.DATA_EQ_TO_ACS[26] = dXSpeed   ;  // X Speed
					
					IO.DATA_EQ_TO_ACS[27] = (int)EN_MOTR_ID.miCLN_Y;  //Axis

					IO.fn_DataEqToAcs();

					Console.WriteLine($"---------- CLEAN : {m_nCleanCnt+1} / {nTotalStep} ---------- ");
					Console.WriteLine($"       StartXPos : {dStartXPos}");
                    Console.WriteLine($"       EndXPos   : {dEndXPos  }");
                    Console.WriteLine($"       StartYPos : {dStartYPos}");
                    Console.WriteLine($"       EndYPos   : {dEndYPos  }");
                    Console.WriteLine($"       YDistance : {dYDistance}");
                    Console.WriteLine($"       XSpeed    : {dXSpeed   }");

					//Log
					fn_WriteLog($"---------- CLEAN : {m_nCleanCnt+1} / {nTotalStep} ---------- ", EN_LOG_TYPE.ltLot);
					fn_WriteLog($"       StartXPos : {dStartXPos}"             , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"       EndXPos   : {dEndXPos  }"             , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"       StartYPos : {dStartYPos}"             , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"       EndYPos   : {dEndYPos  }"             , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"       YDistance : {dYDistance}"             , EN_LOG_TYPE.ltLot);
					fn_WriteLog($"       XSpeed    : {dXSpeed   }"             , EN_LOG_TYPE.ltLot);

					//
					MOTR[(int)EN_MOTR_ID.miSPD_X].MP.dPosn[(int)EN_POSN_ID.CalPos] = dStartXPos;
					MOTR[(int)EN_MOTR_ID.miCLN_Y].MP.dPosn[(int)EN_POSN_ID.CalPos] = dStartYPos;

					m_nCleanStep++;
                    return false;

				case 14:
                    if (SEQ._bStop)
                    {
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.CalPos);
                    r2 =           fn_MoveMotr(m_iMotrXId, EN_COMD_ID.CalPos); //Main-X Cleaning Position
					if (!r1 || !r2) return false;

                    m_nCleanStep++;
                    return false;

				case 15:
                    if (SEQ._bStop)
                    {
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrZId, m_iCmdZ);
                    if (!r1) return false;

					m_nCleanStep++;
                    return false;

				case 16:

                    //Run ACS Buffer for Load cell
                    dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(m_iCmdZ);
                    r1 = IO.fn_ForceBufferRun(dPos); 
					if (!r1)
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0341); // Force Check ACS Buffer Error

                        //
                        Flag = false;
						m_nCleanStep = 0;
                        return true;
                    }

                    m_tDelayTime.Clear();
					m_nCleanStep++;
                    return false;

                case 17:
                    if (SEQ._bStop)
                    {
						IO.fn_SetOpenLoopOff();

                        m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    if (!IO.fn_IsForceBuffRun()) return false;

                    m_tDelayTime.Clear();
					m_nCleanStep++;
                    return false;

                case 18:
                    if (SEQ._bStop)
                    {
						IO.fn_SetOpenLoopOff();

						SEQ_CLEAN.fn_SetDIWaterValve();
						
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun()) return false;

                    m_tDelayTime.Clear();
                    m_nCleanStep++;
                    return false;

                case 19:
					if (!m_tDelayTime.OnDelay(true, 3000)) return false;

					//Check Buffer State
					if (IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_45_CheckForceBuffer] == 1)
                    {
                        //EPU.fn_SetErr(EN_ERR_LIST.ERR_0344); // Force Check ACS Buffer Error
						//
                        ////
                        //Flag = false;
                        //m_nCleanStep = 0;
                        //return true;

                        sLogMsg = string.Format($"Check Buffer State : ERR_0344ERR_0344");
                        fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    }


                    //
                    SEQ_CLEAN.fn_SetDrain();
					SEQ_CLEAN.fn_SetDIWaterValve(vvOpen);
					
					Console.WriteLine("[Clean] Drain On / DI Water On");

					m_tDelayTime.Clear();
					m_nCleanStep++;
                    return false;

                case 20:
					SEQ_CLEAN.fn_SetDrain();
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;
                    
					//Spindle Run
                    nRPM = bTESTData? 1000 : (int)g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].SpindleRPM;
					fn_SetSpindleRun(nRPM);

					fn_WriteSeqLog("Spindle Start");

                    if (!IO.fn_RunBuffer(BFNo_13_MILLING, true))
                    {
						IO.fn_SetOpenLoopOff();

						EPU.fn_SetErr(EN_ERR_LIST.ERR_0342); //

                        //
                        Flag = false;
						m_nCleanStep = 0;
                        return true;
                    }

					m_tDelayTime.Clear();
                    m_nCleanStep++;
                    return false;

                case 21:
                    if (SEQ._bStop)
                    {
						IO.fn_SetOpenLoopOff();

						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    SEQ_CLEAN.fn_SetDrain();

					////Spindle Run
					nRPM = bTESTData ? 1000 : (int)g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].SpindleRPM;
					//if (!fn_SetSpindleRun(nRPM)) return false ;

					Console.WriteLine("[Clean] Spindle Run : " + nRPM);

                    m_tDelayTime.Clear();
					m_nCleanStep++;
                    return false;

                case 22:
                    if (SEQ._bStop)
                    {
						IO.fn_SetOpenLoopOff();

						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    //
                    SEQ_CLEAN.fn_SetDrain();

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					if (!IO.fn_IsBuffRun(BFNo_13_MILLING)) return false;

					Console.WriteLine("[Clean] BFNo_13_MILLING Start");

					m_tDelayTime.Clear();
					m_nCleanStep++;
                    return false;

                case 23:
                    if (SEQ._bStop)
                    {
						IO.fn_SetOpenLoopOff();

						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    //
                    SEQ_CLEAN.fn_SetDrain();
					SEQ_CLEAN.fn_SetDIWaterValve(vvOpen); //JUNG/200616

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Spindle Off timing Check
                    //if(fn_IsSpindleOffCnt(true)) fn_SetSpindleRun(swOff);

                    if (IO.fn_IsBuffRun(BFNo_13_MILLING)) return false;

					fn_SetSpindleRun(swOff);

					//
					SEQ_CLEAN.fn_SetDIWaterValve(vvClose);
					Console.WriteLine("[Clean] DI Water OFF"); 

					//IO.fn_ForceBufferStop();
                    //IO.fn_SetOpenLoopOff ();

					m_tDelayTime.Clear();
					m_nCleanStep++;
                    return false;

				case 24:
					//
					SEQ_CLEAN.fn_SetDrain();
					SEQ_CLEAN.fn_SetDIWaterValve(vvClose);

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					IO.fn_ForceBufferStop();
                    IO.fn_SetOpenLoopOff ();
					fn_SetSpindleRun     (swOff);

					Console.WriteLine("[Clean] Buff Stop & OpenLoop Off");

					m_tDelayTime.Clear();
					m_nCleanStep++;
                    return false;

                case 25:
					SEQ_CLEAN.fn_SetDrain();
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    r1 = fn_SetSpindleRun(swOff);
                    r2 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    //
                    if (++m_nCleanCnt < nTotalStep) //Check total cycle count
                    {
						fn_WriteSeqLog(string.Format($"Cleaning Count : {m_nCleanCnt} / {nTotalStep}"));
						Console.WriteLine(string.Format($"Cleaning Count : {m_nCleanCnt} / {nTotalStep}"));

						m_nCleanStep = 12;
                        return false;
                    }

					SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait1); //SEQ_CLEAN.fn_ReqBathWaitPos();

					m_nCleanStep++;
					return false;

                case 26:

					//Map Change
					DM.MAGA[(int)EN_MAGA_ID.CLEAN].SetTo((int)EN_PLATE_STAT.ptsDeHydrate); //

                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

                    m_sCleanEndTime = DateTime.Now.ToString();

                    fn_WriteSeqLog("[END] CLEANING (PLATE:FINISH/TOOL:USED) - Cleaning Buffer End");

					fn_WriteLog("[END] CLEANING", EN_LOG_TYPE.ltLot);

					Flag = false;
					m_nCleanStep = 0;
                    return true;


                //---------------------------------------------------------------------------
                //TEST Mode 
                case 50:
                    if (SEQ._bStop)
                    {
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

					MOTR[(int)m_iMotrZId].MP.dPosn[(int)EN_POSN_ID.CalPos] = 20;
					
					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.CalPos); //r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.User1);
					if (!r1) return false;

					m_nCleanStep++;
                    return false;

				case 51:
					if (!fn_SetSpindleRun(3000)) return false;

					MOTR[(int)m_iMotrXId].MP.dPosn[(int)EN_POSN_ID.User20] = MOTR[(int)m_iMotrXId].GetEncPos() - 3;
					MOTR[(int)m_iMotrXId].MP.dPosn[(int)EN_POSN_ID.User21] = MOTR[(int)m_iMotrXId].GetEncPos() + 3;

					m_nTestIndex = 0;

					m_nCleanStep++;
                    return false;

				case 52:
                    if (SEQ._bStop)
                    {
                        m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User20);
                    if (!r1) return false;
					m_nCleanStep++;
                    return false;

                case 53:
                    if (SEQ._bStop)
                    {
                        m_nCleanStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User21);
                    if (!r1) return false;

					if(m_nTestIndex++ < 20)
					{
						m_nCleanStep = 52;
                        return false;
                    }

					m_nCleanStep++;
                    return false;

				case 54:
					r1 = fn_SetSpindleRun(swOff);
					r2 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					m_nCleanStep++;
                    return false;

				case 55:

                    //Map Change
                    DM.MAGA[(int)EN_MAGA_ID.CLEAN].SetTo((int)EN_PLATE_STAT.ptsDeHydrate); //

                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);

                    m_sCleanEndTime = DateTime.Now.ToString();

                    fn_WriteSeqLog("[END] CLEANING (PLATE:FINISH/TOOL:USED) - Cleaning Buffer End");

                    Flag = false; 
					m_nCleanStep = 0;
                    return true;


			}

        }
		//---------------------------------------------------------------------------
        private bool fn_ToolPlaceCycle(ref bool Flag)
        {
			//Col 중에 Tool이 가장 없는 위치로 이동 후 버림...
			bool r1, r2;
			bool isCleanTool = DM.MAGA[(int)EN_MAGA_ID.CLEAN].IsStatOne(EN_PLATE_STAT.ptsDeHydrate);

			//
			m_iCmdX = isCleanTool? EN_COMD_ID.Plce1 : EN_COMD_ID.User4; //가장 없는 줄에.... CalPos
            m_iCmdZ = EN_COMD_ID.User4;
            m_iCmdY = EN_COMD_ID.User3; //Storage Y-Axis

			//Place Cycle
			if (m_nToolPlaceStep < 0) m_nToolPlaceStep = 0;

			switch (m_nToolPlaceStep)
			{
				default:
					m_nToolPlaceStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;
					m_nToolClampRetry = 0; 

					fn_MoveCylClamp(ccFwd);

					m_nToolPlaceStep++;
					return false;

				case 11: 
					if(SEQ._bStop)
					{
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nToolPlaceStep = 0;
                        Flag = false;
                        return false;
					}
					
					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

                    m_nToolPlaceStep++;
                    return false;

                case 12:
                    if (SEQ._bStop)
                    {
						m_nToolPlaceStep = 0;
                        Flag = false;
                        return false;
                    }

					MOTR[(int)m_iMotrXId].MP.dPosn[(int)EN_POSN_ID.Plce1] = MOTR[(int)m_iMotrXId].GetPosToCmdId(EN_COMD_ID.User4) - 30;

					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					if (!r1) return false;

					m_nToolPlaceStep++;
                    return false;

                case 13:
                    if (SEQ._bStop)
                    {
						m_nToolPlaceStep = 0;
                        Flag = false;
                        return false;
                    }

					r1 = MOTR.CmprPosByCmd(m_iBtmMotr, m_iCmdY); //SEQ_STORG.fn_ReqMoveMotr(m_iBtmMotr, m_iCmdY);
					if (!r1) return false;

					//Check Basket
                    if(!SEQ_STORG.fn_IsExtToolBasket())
                    {
                    	//
                    	EPU.fn_SetErr(EN_ERR_LIST.ERR_0371, true); //STORAGE - Tool Storage Discard Box Check Error
                    	
                    	m_nToolPlaceStep = 0;
                        Flag = false;
                        return false;
                    }

                    m_nToolPlaceStep++;
                    return false;

                case 14:
                    if (SEQ._bStop)
                    {
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nToolPlaceStep = 0;
                        Flag = false;
                        return false;
                    }

					r1 = fn_MoveMotr(m_iMotrZId, m_iCmdZ);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nToolPlaceStep++;
                    return false;

                case 15:
					r1 = fn_MoveToolClamp(ccBwd);
                    if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay
					m_tDelayTime.Clear();

					m_nToolPlaceStep++;
                    return false;

                case 16:

                    r1 = fn_MoveToolClamp(ccFwd);
                    if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay
					
					m_tDelayTime.Clear();
					m_nToolPlaceStep++;
                    return false;

                case 17:
                    r1 = fn_MoveToolClamp(ccBwd);
                    if (!r1) return false;

                    if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay
                    m_tDelayTime.Clear();

					if(m_nToolClampRetry++ < 1) //Clamp Retry 
					{
                        m_nToolPlaceStep = 16;
                        return false;
                    }

					m_nToolPlaceStep++;
                    return false;

				case 18:

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

					if (!fn_IsExistTool())
					{
						//Data Shift
						DM.TOOL.ClearMapPin();

						DM.TOOL.SetNeedCheck(false);
					}
					else
					{
						//Error
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0331); //Tool Place Error
					}

					fn_MoveToolClamp(ccFwd);

					m_tDelayTime.Clear();

					Flag = false;
					m_nToolPlaceStep = 0;
                    return true;

            }

        }
        //---------------------------------------------------------------------------
        private bool fn_VisnInspCycle(ref bool Flag, VISN_INSP_INFO visninfo)
        {
            //
            bool r1 = false;
            bool r2 = false;
            bool r3 = false;
            bool r4 = false;
            int nTotal = 0;
            double dXpos, dYpos, dTHpos;
            double dPlateCenterX = MOTR[(int)EN_MOTR_ID.miSPD_X].GetPosToCmdId(EN_COMD_ID.User1); // 502.3230; //Polishing Center position
            double dPlateCenterY = MOTR[(int)EN_MOTR_ID.miPOL_Y].GetPosToCmdId(EN_COMD_ID.User1); //-31.5443;
            double dXOffset = 7; //Max X position 
            double dYOffset = 7; //Max Y position 

            //double dPlateCenterX = MOTR[(int)EN_MOTR_ID.miSPD_X].MP.dPosn[(int)EN_POSN_ID.User12]
            //	+ g_VisionManager._RecipeVision.SpindleOffsetX;
            //double dPlateCenterY = MOTR[(int)EN_MOTR_ID.miPOL_Y].MP.dPosn[(int)EN_POSN_ID.User2]
            //	- g_VisionManager._RecipeVision.SpindleOffsetY;

            // Code 관리 차원 X 추가. X는 같은 값.
            double dPlateInterpolationPosX = dPlateCenterX;
            double dPlateInterpolationPosY = 0.0;

            bool IsLoadVisn = visninfo.nBtmWhere == (int)EN_MAGA_ID.LOAD;
            bool IsPoliVisn = visninfo.nBtmWhere == (int)EN_MAGA_ID.POLISH;

            bool bErr = false;

            bool bSkipAlign = FM.m_stSystemOpt.nUseSkipPolAlign == 1; //JUNG/200914/

            //Vision Inspection Cycle
            if (m_nVisnInspStep < 0) m_nVisnInspStep = 0;

            switch (m_nVisnInspStep)
            {
                default:
                    Flag = false;
                    m_nVisnInspStep = 0;
                    return true;

                case 10:
                    if (SEQ._bStop)
                    {
                        m_nVisnInspStep = 0;
                        Flag = false;
                        return true;
                    }

                    Flag = true;

                    m_nVisnRetry = 0;
                    m_nCylRetryCnt = 0;

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1 ) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    r3 = fn_MoveCylLensCvr(ccClose);
                    r4 = fn_MoveCylIR(ccClose);
                    if (!r1 || !r2 || !r3 || !r4) return false;

                    if (IsLoadVisn)
                    {
                        m_iCmdX = EN_COMD_ID.User11;
                        SEQ_TRANS.fn_MoveCylLoadUpDown(ccFwd);
						SEQ_TRANS.fn_ReqMoveVisnPos();

					}
                    
					if (IsPoliVisn)
                    {
                        m_iCmdX = EN_COMD_ID.User12;
                        SEQ_POLIS.fn_MoveCylClamp(ccFwd);
						SEQ_POLIS.fn_ReqBathWaitPosTHTI();

					}

                    m_nVisnInspStep++;
                    return false;

                case 11:
                    if (SEQ._bStop)
                    {
                        m_nVisnInspStep = 0;
                        Flag = false;
                        return true;
                    }

                    //1.8 - Inspection Position : Align, Polishing(Align), Polishing(Inspection) or Cleaning(Inspection)
                    r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
                    r2 = IsPoliVisn ? SEQ_POLIS.fn_ReqMoveMotr (EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User2) : true;
                    r3 = IsPoliVisn ? SEQ_POLIS.fn_MoveCylClamp(ccFwd) : SEQ_TRANS.fn_MoveCylLoadUpDown(ccFwd); //JUNG/200525/추가
					r4 = IsPoliVisn ? SEQ_POLIS.fn_ReqBathWaitPosTHTI(): SEQ_TRANS.fn_ReqMoveVisnPos();
					if (!r1 || !r2 || !r3 || !r4) return false;

                    m_nCylRetryCnt = 0;
                    m_nVisnInspStep++;
                    return false;

                case 12:
                    if (SEQ._bStop)
                    {
                        m_nVisnInspStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Check Retry Condition
                    if (fn_GetLensCvrRetryFwd() || fn_GetIRRetryFwd())
                    {
                        if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
                        {
                            fn_MoveCylforCam(ccClose);
                            m_tDelayTime.Clear();

                            m_nVisnBackStep = 12;
                            m_nVisnInspStep = 30;

                            return false;
                        }

                    }

                    r1 = fn_MoveCylLensCvr(ccOpen);
                    r2 = fn_MoveCylIR(ccOpen, IsPoliVisn ? (int)EN_VISION_MODE.Polishing : (int)EN_VISION_MODE.Loading);
                    if (!r1 || !r2) return false;

                    m_nVisnInspStep++;
                    return false;

                case 13:

                    m_nCylRetryCnt = 0;

                    r3 = g_VisionManager.fn_SetLightValue(swOn, IsPoliVisn ? EN_VISION_MODE.Polishing : EN_VISION_MODE.Loading); //Light On
                    if (!r3) return false;

                    //
                    if (IsLoadVisn)
                    {
                        m_tDelayTime.Clear();
                        m_nVisnInspStep = 50;
                        return false;
                    }

                    //
                    if (m_bTESTMODE)
                    {
                        m_tDelayTime.Clear();
                        m_nVisnInspStep = 19;
                        return false;
                    }

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;

                case 14:
                    if (!m_tDelayTime.OnDelay(true, 3000)) return false; //Delay for stable

                    g_VisionManager._AlginMainCtrl.fn_ClearResult();
                    //g_VisionManager.delUpdateMainresultClear?.Invoke();
                    fn_WriteLog($"---------------------------------Theta Align({m_nVisnRetry})-----------------------------", EN_LOG_TYPE.ltVision);
                    if (!g_VisionManager.fn_PolishingAlign(ref stvisionResult, bSkipAlign))
                    {
                        //JUNG/Retry 2
                        if (++m_nVisnRetry < 5)
                        {
                            g_VisionManager.fn_SetLightValue(swOn, EN_VISION_MODE.Polishing); //Light On

                            m_tDelayTime.Clear();
                            m_nVisnInspStep = 14;
                            return false;
                        }

                        //LEE/200929 [Add] : Vision Error Skip
                        if (FM.m_stSystemOpt.nUseSkipVisError == 1)
                        {
                            sLogMsg = string.Format($"FAIL - [Theta Polishing] Vision Error Skip - MAP FINISH");
                            Console.WriteLine(sLogMsg);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                            //Map - Polishing
                            DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo  ((int)EN_PLATE_STAT.ptsFinish);
                            DM.MAGA[(int)EN_MAGA_ID.POLISH].SetInfo(EN_PLATE_INFO.ifVisnErr);

                            Flag = false;
                            m_nVisnInspStep = 0;
                            return true;
                        }
                        else
                        {
                            EPU.fn_SetErr(EN_ERR_LIST.ERR_0560); //Polishing Bath Theta Align Error
                            sLogMsg = string.Format("FAIL - Spindle Seq, PolishingAlign(Theta Align)");
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);
                            Flag = false;
                            m_nVisnInspStep = 0;
                            return true;
                        }
                    }

                    sLogMsg = string.Format($"Theta Align Result {stvisionResult.dTheta}/ Theta Enc Pos : {SEQ_POLIS.GetEncPos_TH()}");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    Console.WriteLine($"Theta Align Result : {stvisionResult.dTheta}");
                    
					//JUNG/210115/Check Enc Pos.
                    if (!MOTR.CheckMinMaxP(EN_MOTR_ID.miPOL_TH, SEQ_POLIS.GetEncPos_TH()))
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0561); // Polishing Bath Theta Align Error - Out Of Range : Theta

                        sLogMsg = string.Format($"FAIL - Error Theta Enc. Position");
                        Console.WriteLine(sLogMsg);
                        fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                        Flag = false;
                        m_nVisnInspStep = 0;
                        return true;
                    }


                    //JUNG/200429/Check Position Error
                    if (!MOTR.CheckMinMaxP(EN_MOTR_ID.miPOL_TH, SEQ_POLIS.GetEncPos_TH() + (stvisionResult.dTheta * -1)))
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0561); // Polishing Bath Theta Align Error - Out Of Range : Theta

                        sLogMsg = string.Format($"FAIL - Theta Align | Value : {stvisionResult.dTheta}");
                        Console.WriteLine(sLogMsg);
                        fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                        Flag = false;
                        m_nVisnInspStep = 0;
                        return true;
                    }

                    m_nVisnInspStep++;
                    return false;

                // Move Theta
                case 15:

                    r1 = MOTR.MoveMotrR(EN_MOTR_ID.miPOL_TH, stvisionResult.dTheta * -1, 100);
                    if (!r1) return false;

                    //Cleaning Align Data
                    SEQ_CLEAN.fn_SetAlignPos(stvisionResult.dTheta * -1);

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;

                case 16:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    r1 = MOTR[(int)EN_MOTR_ID.miPOL_TH].GetStop();
                    if (!r1) return false;

                    sLogMsg = string.Format($"[ Align Result ] TH : {SEQ_POLIS.GetEncPos_TH()}mm");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
                    Console.WriteLine(sLogMsg);

                    m_nVisnRetry = 0;

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;

                case 17:
                    if (!m_tDelayTime.OnDelay(true, 1000)) return false; //Delay for stable // X,Y Align
                    fn_WriteLog($"---------------------------------X,Y Align({m_nVisnRetry})-----------------------------", EN_LOG_TYPE.ltVision);
                    g_VisionManager._AlginMainCtrl.fn_ClearResult();
                    //g_VisionManager.delUpdateMainresultClear?.Invoke();
                    if (!g_VisionManager.fn_PolishingAlign(ref stvisionResult, bSkipAlign))
                    {
                        //JUNG/Retry 2
                        if (++m_nVisnRetry < 5)
                        {
                            g_VisionManager.fn_SetLightValue(swOn, EN_VISION_MODE.Polishing); //Light On

                            m_tDelayTime.Clear();
                            m_nVisnInspStep = 17;
                            return false;
                        }

                        //LEE/200929 [Add] : Vision Error Skip
                        if (FM.m_stSystemOpt.nUseSkipVisError == 1)
                        {
                            sLogMsg = string.Format($"FAIL - [X,Y Align Polishing] Vision Error Skip - MAP FINISH");
                            Console.WriteLine(sLogMsg);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

                            //Map - Polishing
                            DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsFinish);
                            DM.MAGA[(int)EN_MAGA_ID.POLISH].SetInfo(EN_PLATE_INFO.ifVisnErr);

                            m_nVisnInspStep = 100;
                            return false;
                        }
                        else
                        {
                            EPU.fn_SetErr(EN_ERR_LIST.ERR_0562); // Polishing Bath X,Y Align Error

                            sLogMsg = string.Format("X,Y Align Fail");
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                            Flag = false;
                            m_nVisnInspStep = 0;
                            return true;
                        }

                    }

                    // 좌표계 변환. => Cam 기준 상대좌표. (3사분면 좌표계)
                    stvisionResult.pntModel.X *= g_VisionManager._RecipeVision.ResolutionX / 1000.0;
                    stvisionResult.pntModel.Y *= g_VisionManager._RecipeVision.ResolutionY / 1000.0;

                    fn_WriteLog($"[ X,Y Align Result ] X : {stvisionResult.pntModel.X:F3} mm , Y : {stvisionResult.pntModel.Y:F3} mm", EN_LOG_TYPE.ltVision);

                    fn_WriteLog($"   X-Start : {stvisionResult.stRecipeList[0].pStartPos.X:F4}", EN_LOG_TYPE.ltVision);
                    fn_WriteLog($"   X-End :   {stvisionResult.stRecipeList[0].pEndPos.X:F4}", EN_LOG_TYPE.ltVision);
                    fn_WriteLog($"   Y-Start : {stvisionResult.stRecipeList[0].pStartPos.Y:F4}", EN_LOG_TYPE.ltVision);
                    fn_WriteLog($"   Y-End :   {stvisionResult.stRecipeList[0].pEndPos.Y:F4}", EN_LOG_TYPE.ltVision);

                    m_nVisnInspStep++;
                    return false;

                case 18:

                    //stvisionResult.stRecipeList[0].pStartPos.X + 엔코더 X 현재 값.
                    //stvisionResult.stRecipeList[0].pStartPos.Y + 엔코더 Y 현재 값.

                    //stvisionResult.stRecipeList[0].pEndPos.X + 엔코더 X 현재 값.
                    //stvisionResult.stRecipeList[0].pEndPos.Y + 엔코더 Y 현재 값.

                    nTotal = stvisionResult.nTotalStep;
                    dXpos  = GetEncPos_X();
                    dYpos  = SEQ_POLIS.GetEncPos_Y();
                    dTHpos = SEQ_POLIS.GetEncPos_TH();

                    //Save X,Y Value
                    for (int n = 0; n < nTotal; n++)
                    {
                        if (stvisionResult.stRecipeList[n].nUseMilling != 1) continue;
                        //		+ (Spindle)
                        //	□ (Cam)
                        // Spindle X Pos Value > Cam X Pos Value
                        // Spindle이 더가야하므로 Offset + 부호

                        //	+ (Spindle)
                        //		□ (Cam)
                        // Spindle X Pos Value < Cam X Pos Value
                        // Spindle이 덜가야하므로 Offset - 부호

                        stvisionResult.stRecipeList[n].dStartX = dXpos - stvisionResult.stRecipeList[n].pStartPos.X + g_VisionManager._RecipeVision.SpindleOffsetX;
                        stvisionResult.stRecipeList[n].dEndX   = dXpos - stvisionResult.stRecipeList[n].pEndPos.X + g_VisionManager._RecipeVision.SpindleOffsetX;

                        stvisionResult.stRecipeList[n].dStartY = dYpos + stvisionResult.stRecipeList[n].pStartPos.Y - g_VisionManager._RecipeVision.SpindleOffsetY;
                        stvisionResult.stRecipeList[n].dEndY   = dYpos + stvisionResult.stRecipeList[n].pEndPos.Y - g_VisionManager._RecipeVision.SpindleOffsetY;

                        fn_WriteLog($"-----------------------------Result Process------------------------", EN_LOG_TYPE.ltVision);
                        fn_WriteLog($"{n} - Start Pos : {stvisionResult.stRecipeList[n].dStartX}, {stvisionResult.stRecipeList[n].dStartY}", EN_LOG_TYPE.ltVision);
                        fn_WriteLog($"{n} - End   Pos : {stvisionResult.stRecipeList[n].dEndX  }, {stvisionResult.stRecipeList[n].dEndY  }", EN_LOG_TYPE.ltVision);
                        stvisionResult.stRecipeList[n].dPosTH = dTHpos;

                        //---------------------------------------------------------------------------
                        // System Offset
                        //---------------------------------------------------------------------------
                        stvisionResult.stRecipeList[n].dStartX += (FM.m_stProjectBase.dPolishOffset_X * -1); //JUNG/200908
                        stvisionResult.stRecipeList[n].dEndX   += (FM.m_stProjectBase.dPolishOffset_X * -1);

                        stvisionResult.stRecipeList[n].dStartY += FM.m_stProjectBase.dPolishOffset_Y;
                        stvisionResult.stRecipeList[n].dEndY   += FM.m_stProjectBase.dPolishOffset_Y;

                        stvisionResult.stRecipeList[n].dTilt   += FM.m_stProjectBase.dPolishOffset_TI;
                        //---------------------------------------------------------------------------
                        // Theta Offset 은 추가 스탭 작업 요함.
                        // => Theta 가공은 Align 이후 특정 각도를 돌려서 가공을 하기위함 이므로.
                        //stvisionResult.stRecipeList[n].dPosTH += FM.m_stProjectBase.dPolishOffset_TH;
                        //---------------------------------------------------------------------------

                        //---------------------------------------------------------------------------
                        //	Vision Align InPosition Error
                        //---------------------------------------------------------------------------
                        // Interpolation Tilt.
                        dPlateInterpolationPosY = dPlateCenterY + g_VisionManager.fn_GetTiltInterpolation(stvisionResult.stRecipeList[n].dTilt);

                        //
                        if (stvisionResult.stRecipeList[n].dStartX < dPlateInterpolationPosX - dXOffset || stvisionResult.stRecipeList[n].dStartX > dPlateInterpolationPosX + dXOffset)
                        {
                            fn_WriteLog($"Start X InPosition Error.{n}", EN_LOG_TYPE.ltVision);
                            bErr = true;
                        }
                        if (stvisionResult.stRecipeList[n].dEndX < dPlateInterpolationPosX - dXOffset || stvisionResult.stRecipeList[n].dEndX > dPlateInterpolationPosX + dXOffset)
                        {
                            fn_WriteLog($"End X InPosition Error.{n}", EN_LOG_TYPE.ltVision);
                            bErr = true;
                        }
                        if (stvisionResult.stRecipeList[n].dStartY < dPlateInterpolationPosY - dYOffset || stvisionResult.stRecipeList[n].dStartY > dPlateInterpolationPosY + dYOffset)
                        {
                            fn_WriteLog($"Start Y InPosition Error.{n}", EN_LOG_TYPE.ltVision);
                            bErr = true;
                        }
                        if (stvisionResult.stRecipeList[n].dEndY < dPlateInterpolationPosY - dYOffset || stvisionResult.stRecipeList[n].dEndY > dPlateInterpolationPosY + dYOffset)
                        {
                            fn_WriteLog($"End Y InPosition Error.{n}", EN_LOG_TYPE.ltVision);
                            bErr = true;
                        }
                    }


                    //
                    Console.WriteLine($"[ BEFORR ] ENC POS - X:{dXpos}/Y:{dYpos}");

                    Console.WriteLine($"[ Align Result ] START POS - X:{stvisionResult.stRecipeList[0].dStartX}/Y:{stvisionResult.stRecipeList[0].dStartY}");
                    Console.WriteLine($"[ Align Result ] END   POS - X:{stvisionResult.stRecipeList[0].dEndX  }/Y:{stvisionResult.stRecipeList[0].dEndY  }");
                    Console.WriteLine($"[ Align Result ] TH    POS - X:{stvisionResult.stRecipeList[0].dPosTH}");
                    Console.WriteLine($"[ Align Result ] TiltOffset- T:{stvisionResult.stRecipeList[0].dTiltOffset}");


                    //Check Vision Align InPosition Error
                    if (bErr)
                    {
                        fn_WriteLog($"Polishing Bath X,Y Align Error -> In-position Error.", EN_LOG_TYPE.ltVision);
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0563); // Polishing Bath X,Y Align Error - In-position Error

                        Flag = false;
                        m_nVisnInspStep = 0;
                        return true;
                    }

                    //
                    vresult = stvisionResult;

                    //File Save
                    fn_LoadVisnResult(flSave);

                    fn_WriteSeqLog("VISION Inspect OK");

                    Console.WriteLine("VISION Inspect OK");

                    m_nCylRetryCnt = 0;
                    m_nVisnInspStep++;
                    return false;

                case 19:
                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Retry Condition
                    if (fn_GetLensCvrRetryBwd() || fn_GetIRRetryBwd())
                    {
                        if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
                        {
                            fn_MoveCylforCam(ccOpen);
                            m_tDelayTime.Clear();

                            m_nVisnBackStep = 19;
                            m_nVisnInspStep = 31;
                            return false;
                        }
                    }

                    r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR(ccClose);
                    if (!r1 || !r2) return false;

                    m_nVisnInspStep++;
                    return false;

                case 20:

                    r3 = g_VisionManager.fn_SetLightValue(swOff); //Light Off
                    if (!r3) return false;

                    //Map - Polishing
                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolish);

                    Flag = false;
                    m_nVisnInspStep = 0;
                    return true;

                //-------------------------------------------------------------------------------------------------
                //Cylinder Retry
                case 30:
                    r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR(ccClose);
                    if (!r1 || !r2) return false;

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    sTemp = string.Format($"[VISN CYCLE] Cylinder Retry : Step = {m_nVisnBackStep} / Count = {m_nCylRetryCnt}");
                    fn_WriteLog(sTemp);

                    m_nVisnInspStep = m_nVisnBackStep;
                    return false;

                case 31:
                    r1 = fn_MoveCylLensCvr(ccOpen);
                    r2 = fn_MoveCylIR(ccOpen, IsPoliVisn ? (int)EN_VISION_MODE.Polishing : (int)EN_VISION_MODE.Loading);
                    if (!r1 || !r2) return false;

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    sTemp = string.Format($"[VISN CYCLE] Cylinder Retry : Step = {m_nVisnBackStep} / Count = {m_nCylRetryCnt}");
                    fn_WriteLog(sTemp);

                    m_nVisnInspStep = m_nVisnBackStep;
                    return false;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Load Part : Pre - Align
                case 50:
                    //
                    r1 = SEQ_TRANS.fn_MoveCylTopTurn(ccDeg0);
                    r2 = SEQ_TRANS.fn_ReqMoveVisnPos();
                    if (!r1 || !r2) return false;

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
					return false;

                case 51:

                    //
                    if (m_bTESTMODE)
                    {
                        m_nVisnInspStep = 54;
                        return false;
                    }

                    if (!m_tDelayTime.OnDelay(true, 3000)) return false; //Delay for stable
                    fn_WriteLog($"---------------------------------PreAlign------------------------------", EN_LOG_TYPE.ltVision);
                    if (!g_VisionManager.fn_PreAlign(ref stvisionResult, bSkipAlign))
                    {
                        //JUNG/Retry 2
                        if (++m_nVisnRetry < 2)
                        {
                            g_VisionManager.fn_SetLightValue(swOn, EN_VISION_MODE.Loading); //Light On

                            Console.WriteLine("PreAlign Retry : " + m_nVisnRetry);
                            m_tDelayTime.Clear();
                            m_nVisnInspStep = 51;
							return false;
                        }

                        //LEE/200929 [Add] : Vision Error Skip
                        if (FM.m_stSystemOpt.nUseSkipVisError == 1)
                        {
                            sLogMsg = string.Format($"FAIL - [Theta Load] Vision Error Skip - MAP FINISH");
                            Console.WriteLine(sLogMsg);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

                            fn_MoveCylLensCvr(ccClose);
                            fn_MoveCylIR(ccClose);

                            //Map - Load
                            DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo((int)EN_PLATE_STAT.ptsFinish);
                            DM.MAGA[(int)EN_MAGA_ID.LOAD].SetInfo(EN_PLATE_INFO.ifVisnErr);

                            m_nVisnInspStep = 100;
                            return false;
                        }
                        else
                        {
                            EPU.fn_SetErr(EN_ERR_LIST.ERR_0550); // Load Theta Align Error

                            sLogMsg = string.Format($"FAIL - Load Theta Align {stvisionResult.dTheta}");
                            Console.WriteLine(sLogMsg);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

                            Flag = false;
                            m_nVisnInspStep = 0;
                            return true;
                        }
                    }

                    Console.WriteLine($"Load Theta Align Result : {stvisionResult.dTheta}");

                    //JUNG/200429/Check Position Error
                    if (!MOTR.CheckMinMaxP(EN_MOTR_ID.miTRF_T, SEQ_TRANS.GetEncPos_TH() + (stvisionResult.dTheta * -1)))
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0551); // Load Theta Align In-position Error - Out Of Range : Theta

                        Console.WriteLine("FAIL - Theta Align | Value : " + stvisionResult.dTheta);

                        Flag = false;
                        m_nVisnInspStep = 0;
                        return true;
                    }

                    //
                    MOTR[(int)EN_MOTR_ID.miTRF_T].MP.dPosn[(int)EN_POSN_ID.CalPos] = SEQ_TRANS.GetEncPos_TH() + (stvisionResult.dTheta * -1);

                    fn_MoveCylLensCvr(ccClose);
                    fn_MoveCylIR(ccClose);

                    m_nVisnInspStep++;
                    return false;

                // Move Theta
                case 52:

                    //r1 = MOTR.MoveMotrR(EN_MOTR_ID.miTRF_T, stvisionResult.dTheta * -1, 100);
                    r1 = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_T, EN_COMD_ID.CalPos);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;

                case 53:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    //r1 = MOTR[(int)EN_MOTR_ID.miTRF_T].GetStop();
                    //if (!r1) return false;

                    Console.WriteLine($"[ Peei-Align Result ] TH : {SEQ_TRANS.GetEncPos_TH()} mm");

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;

                case 54:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    //File Save
                    SEQ_TRANS.fn_SetPreAlignPos(SEQ_TRANS.GetEncPos_TH());

                    fn_WriteSeqLog("Pre-Align Vision Inspect OK : " + SEQ_TRANS.GetEncPos_TH());

                    fn_WriteLog("[Pre-Align] Vision Inspect OK : " + SEQ_TRANS.GetEncPos_TH(), EN_LOG_TYPE.ltLot);
                    Console.WriteLine("Pre-Align Vision Inspect OK");

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;

                case 55:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR(ccClose);
                    if (!r1 || !r2) return false;

                    r3 = g_VisionManager.fn_SetLightValue(swOff); //Light Off
                    if (!r3) return false;

                    //Map - Align
                    DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo((int)EN_PLATE_STAT.ptsAlign);

                    Flag = false;
                    m_nVisnInspStep = 0;
                    return true;

                //Vision Error Skip
                case 100:
                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Retry Condition
                    if (fn_GetLensCvrRetryBwd() || fn_GetIRRetryBwd())
                    {
                        if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
                        {
                            fn_MoveCylforCam(ccOpen);
                            m_tDelayTime.Clear();

                            m_nVisnBackStep = 100;
                            m_nVisnInspStep = 31;
                            return false;
                        }
                    }

                    r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR(ccClose);
                    if (!r1 || !r2) return false;

                    m_nVisnInspStep++;
                    return false;

                case 101:

                    r3 = g_VisionManager.fn_SetLightValue(swOff); //Light Off
                    if (!r3) return false;

                    Flag = false;
                    m_nVisnInspStep = 0;
                    return true;

            }
        }
		int m_nNowModelIndex = -1;
		//---------------------------------------------------------------------------
		private bool fn_VisnInspCycle2(ref bool Flag, VISN_INSP_INFO visninfo)
        {
			//
			bool r1 = false;
			bool r2 = false;
			bool r3 = false;
			bool r4 = false;
			int  nTotal = 0;
			double dXpos, dYpos, dTHpos;
			double dPlateCenterX = MOTR[(int)EN_MOTR_ID.miSPD_X].GetPosToCmdId(EN_COMD_ID.User1); // 502.3230; //Polishing Center position
			double dPlateCenterY = MOTR[(int)EN_MOTR_ID.miPOL_Y].GetPosToCmdId(EN_COMD_ID.User1); //-31.5443;
			double dXOffset = 7; //Max X position 
			double dYOffset = 7; //Max Y position 

            //double dPlateCenterX = MOTR[(int)EN_MOTR_ID.miSPD_X].MP.dPosn[(int)EN_POSN_ID.User12]
            //	+ g_VisionManager._RecipeVision.SpindleOffsetX;
            //double dPlateCenterY = MOTR[(int)EN_MOTR_ID.miPOL_Y].MP.dPosn[(int)EN_POSN_ID.User2]
            //	- g_VisionManager._RecipeVision.SpindleOffsetY;

            // Code 관리 차원 X 추가. X는 같은 값.
            double dPlateInterpolationPosX = dPlateCenterX;
			double dPlateInterpolationPosY = 0.0;

			bool IsLoadVisn = visninfo.nBtmWhere == (int)EN_MAGA_ID.LOAD  ;
			bool IsPoliVisn = visninfo.nBtmWhere == (int)EN_MAGA_ID.POLISH;

			bool bErr = false;

			bool bSkipAlign = FM.m_stSystemOpt.nUseSkipPolAlign == 1; //JUNG/200914/

            // Optic Condition Temporary Value
            int nIRFilter        = 0;
            int nLightIR         = 0;
            int nLightWhite      = 0;
            int nCameraExposure  = 0;
            int nCameraGain      = 0;

            //Vision Inspection Cycle
            if (m_nVisnInspStep < 0) m_nVisnInspStep = 0;

			switch (m_nVisnInspStep)
			{
				default:
					Flag = false;
					m_nVisnInspStep  = 0;
					m_nNowModelIndex = 0;
					return true;

				case 10:
                    if (SEQ._bStop)
                    {
						m_nVisnInspStep = 0;
                        Flag = false;
                        return true;
                    }

					Flag = true;

					m_nVisnRetry     = 0;
					m_nCylRetryCnt   = 0;
					m_nNowModelIndex = 0;

					r1 = fn_MoveMotr      (m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr      (m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    r3 = fn_MoveCylLensCvr(ccClose);
                    r4 = fn_MoveCylIR     (ccClose);
                    if (!r1 || !r2 || !r3 || !r4) return false;

					if (IsLoadVisn)
					{
						m_iCmdX = EN_COMD_ID.User11;
						SEQ_TRANS.fn_MoveCylLoadUpDown(ccFwd);
						SEQ_TRANS.fn_ReqMoveVisnPos();
					}
					else if (IsPoliVisn)
					{
						m_iCmdX = EN_COMD_ID.User12;
						SEQ_POLIS.fn_MoveCylClamp(ccFwd);
						SEQ_POLIS.fn_ReqBathWaitPosTHTI();
					}

                    m_nVisnInspStep++;
					return false;

                case 11:
                    if (SEQ._bStop)
                    {
                        m_nVisnInspStep = 0;
                        Flag = false;
                        return true;
                    }

					//1.8 - Inspection Position : Align, Polishing(Align), Polishing(Inspection) or Cleaning(Inspection)
					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					r2 = IsPoliVisn ? SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User2) : true;
					r3 = IsPoliVisn ? SEQ_POLIS.fn_MoveCylClamp(ccFwd)  : SEQ_TRANS.fn_MoveCylLoadUpDown(ccFwd); //JUNG/200525/추가
                    r4 = IsPoliVisn ? SEQ_POLIS.fn_ReqBathWaitPosTHTI() : SEQ_TRANS.fn_ReqMoveVisnPos();
                    if (!r1 || !r2 || !r3 || !r4) return false;

                    //
                    m_nCylRetryCnt = 0;

					g_VisionManager.delUpdateMainresultClear?.Invoke();

					//Check Load Part
					if (IsLoadVisn)
                    {
                        m_tDelayTime.Clear();
                        m_nVisnInspStep = 50;
                        return false;
                    }

					//JUNG/210118/Check Utility Exist
					if(!SEQ_POLIS._bEmptyUitl)
					{
						SEQ_POLIS.fn_SetDrain();
					}
                    m_nVisnInspStep++;
                    return false;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Polishing Part
				case 12: 
                    if (SEQ._bStop)
                    {
                        m_nVisnInspStep = 0;
                        Flag = false;
                        return true;
                    }

					if (SEQ_POLIS._bDrngSeqDrain) return false;

                    //Check Enable
                    if (g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].Enable == 0)
                    {
                        if (++m_nNowModelIndex < 10) return false;
                        else
                        {
							if (m_nVisnRetry > 0)
							{
                                //
                                if (FM.m_stSystemOpt.nUseSkipVisError == 1)
                                {
                                    sLogMsg = string.Format($"FAIL - [Theta Polishing] Vision Error Skip - MAP FINISH");
                                    Console.WriteLine(sLogMsg);
                                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                                    //Map - Polishing
                                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsFinish);
                                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetInfo(EN_PLATE_INFO.ifVisnErr);

                                    Flag = false;
                                    m_nVisnInspStep = 0;
                                    return true;
                                }
                                else
                                {
                                    EPU.fn_SetErr(EN_ERR_LIST.ERR_0560); //Polishing Bath Theta Align Error
                                    sLogMsg = string.Format("FAIL - Spindle Seq, PolishingAlign(Theta Align)");
                                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);
                                    Flag = false;
                                    m_nVisnInspStep = 0;
                                    return true;
                                }
                            }
                            else
							{
								EPU.fn_SetErr(EN_ERR_LIST.ERR_0564); //Vision Recipe Error - Enable Error
                                m_nVisnInspStep = 0;
                                Flag = false;
                                return true;
                            }
                        }
                    }
					else
					{
						//
						m_nVisnRetry = 0;
					}


                    //Check Cylinder Retry Condition
                    if (fn_GetLensCvrRetryFwd() || fn_GetIRRetryFwd())
                    {
						if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
						{
							fn_MoveCylforCam(ccClose);
							m_tDelayTime.Clear();

							m_nVisnBackStep = 12;
							m_nVisnInspStep = 30;

							return false;
						}

                    }

                    r1 = fn_MoveCylLensCvr (ccOpen );

					// Global Optic
					if (g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].UseGlobalPolishing == 1)
						nIRFilter = g_VisionManager.CurrentRecipe.LightIRFilter[(int)EN_VISION_MODE.Polishing];
					else
						nIRFilter = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].PolishingLightIRFilter;
					

					r2 = fn_MoveCylIR(nIRFilter == 1 ? ccClose : ccOpen);

					if (!r1 || !r2) return false;

                    //
                    if (m_bTESTMODE)
                    {
                        m_tDelayTime.Clear();
                        m_nVisnInspStep = 19;
                        return false;
                    }

                    m_nVisnInspStep++;
                    return false;
				
				case 13:

					m_nCylRetryCnt = 0;

					// Global Optic
					if (g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].UseGlobalPolishing == 1)
					{
						nLightIR        = g_VisionManager.CurrentRecipe.LightIR           [(int)EN_VISION_MODE.Polishing];
						nLightWhite     = g_VisionManager.CurrentRecipe.LightWhite        [(int)EN_VISION_MODE.Polishing];
						nCameraExposure = g_VisionManager.CurrentRecipe.CameraExposureTime[(int)EN_VISION_MODE.Polishing];
						nCameraGain     = g_VisionManager.CurrentRecipe.CameraGain	      [(int)EN_VISION_MODE.Polishing];
					}
					else
					{
						nLightIR        = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].PolishingLightIR;
						nLightWhite     = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].PolishingLightWhite;
						nCameraExposure = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].PolishingCameraExposureTime;
						nCameraGain     = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].PolishingCameraGain;
					}
					
					g_VisionManager.fn_SetLightValue(swOn, nLightWhite, nLightIR); //Light On

					// Camera Parameter Set 추가 할것.
					g_VisionManager._CamManager.fn_SetExposure(nCameraExposure);
					g_VisionManager._CamManager.fn_SetGain    (nCameraGain    );

					m_tDelayTime.Clear();
					m_nVisnInspStep++;

					return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 3000)) return false; //Delay for stable

					g_VisionManager._AlginMainCtrl.fn_ClearResult();
					//g_VisionManager.delUpdateMainresultClear?.Invoke();
					fn_WriteLog($"---------------------------------Theta Align({m_nVisnRetry})-----------------------------", EN_LOG_TYPE.ltVision);
					fn_WriteLog($"Polishing Theta Align :  Model Index : {m_nNowModelIndex + 1} / Count : {m_nVisnRetry+1}", EN_LOG_TYPE.ltLot);

					//
					if (!g_VisionManager.fn_PolishingAlign(m_nNowModelIndex, ref stvisionResult, EN_ALIGNSTEP.ThetaAlign, bSkipAlign))
                    {
						//JUNG/Retry 2
						if (++m_nVisnRetry < 2)
						{
							m_tDelayTime.Clear();
							m_nVisnInspStep = 13;
							return false;
						}
						else
						{
							if (++m_nNowModelIndex < 10)
							{
								//m_nVisnRetry = 0;

								m_tDelayTime.Clear();
								m_nVisnInspStep = 12;
								return false;
							}
							else
							{
								//LEE/200929 [Add] : Vision Error Skip
								if (FM.m_stSystemOpt.nUseSkipVisError == 1)
								{
									sLogMsg = string.Format($"FAIL - [Theta Polishing] Vision Error Skip - MAP FINISH");
									Console.WriteLine(sLogMsg);
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

									//Map - Polishing
									DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo  ((int)EN_PLATE_STAT.ptsFinish);
									DM.MAGA[(int)EN_MAGA_ID.POLISH].SetInfo(EN_PLATE_INFO.ifVisnErr);

									Flag = false;
									m_nVisnInspStep = 0;
									return true;
								}
								else
								{
									EPU.fn_SetErr(EN_ERR_LIST.ERR_0560); //Polishing Bath Theta Align Error
									sLogMsg = string.Format("FAIL - Spindle Seq, PolishingAlign(Theta Align)");
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);
									Flag = false;
									m_nVisnInspStep = 0;
									return true;
								}
							}
						}
                    }
					

					sLogMsg = string.Format($"Theta Align Result {stvisionResult.dTheta} / Model Index : {m_nNowModelIndex}");
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot   );

					Console.WriteLine($"Theta Align Result : {stvisionResult.dTheta} / Model Index : {m_nNowModelIndex}");

					//JUNG/200429/Check Position Error
					if (!MOTR.CheckMinMaxP(EN_MOTR_ID.miPOL_TH, SEQ_POLIS.GetEncPos_TH() + (stvisionResult.dTheta * -1)))
					{
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0561); // Polishing Bath Theta Align Error - Out Of Range : Theta

						sLogMsg = string.Format($"FAIL - Theta Align | Value : {stvisionResult.dTheta}" );
						Console.WriteLine(sLogMsg);
						fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

						Flag = false;
                        m_nVisnInspStep = 0;
                        return true;
                    }

                    m_nVisnInspStep++;
                    return false;

                // Move Theta
                case 15:

                    r1 = MOTR.MoveMotrR(EN_MOTR_ID.miPOL_TH, stvisionResult.dTheta * -1, 100);
                    if (!r1) return false;

					//Cleaning Align Data
					SEQ_CLEAN.fn_SetAlignPos(stvisionResult.dTheta * -1);

					m_tDelayTime.Clear();
					m_nVisnInspStep++;
                    return false;

                case 16:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = MOTR[(int)EN_MOTR_ID.miPOL_TH].GetStop();
                    if (!r1) return false;

					sLogMsg = string.Format($"[ Align Result ] TH : {SEQ_POLIS.GetEncPos_TH()}mm");
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
					Console.WriteLine(sLogMsg);

					m_nVisnRetry = 0; 

					m_tDelayTime.Clear();
					m_nVisnInspStep++;
					return false;

                case 17:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false; //Delay for stable // X,Y Align
					g_VisionManager._AlginMainCtrl.fn_ClearResult();
					//g_VisionManager.delUpdateMainresultClear?.Invoke();
					
					fn_WriteLog($"---------------------------------X,Y Align({m_nVisnRetry})-----------------------------", EN_LOG_TYPE.ltVision);
					fn_WriteLog($"Polishing : X,Y Align Count : {m_nVisnRetry}", EN_LOG_TYPE.ltLot);

					if (!g_VisionManager.fn_PolishingAlign(m_nNowModelIndex, ref stvisionResult, EN_ALIGNSTEP.XYAlign, bSkipAlign))
                    {
						//JUNG/Retry 2
						if (++m_nVisnRetry < 3)
						{
						    m_tDelayTime.Clear();
						    m_nVisnInspStep = 17;
							
							return false;
						}

						//LEE/200929 [Add] : Vision Error Skip
						if (FM.m_stSystemOpt.nUseSkipVisError == 1)
                        {
                            sLogMsg = string.Format($"FAIL - [X,Y Align Polishing] Vision Error Skip - MAP FINISH");
                            Console.WriteLine(sLogMsg);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot   );
							fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

							//Map - Polishing
							DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo  ((int)EN_PLATE_STAT.ptsFinish);
                            DM.MAGA[(int)EN_MAGA_ID.POLISH].SetInfo(EN_PLATE_INFO.ifVisnErr     );

                            m_nVisnInspStep = 100;
                            return false;
                        }
                        else
                        {
						    EPU.fn_SetErr(EN_ERR_LIST.ERR_0562); // Polishing Bath X,Y Align Error

                            sLogMsg = string.Format("X,Y Align Fail");
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);
                            fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                            Flag = false;
                            m_nVisnInspStep = 0;
                            return true;
                        }

                    }

                    sLogMsg = string.Format($"X,Y Align Result X : {stvisionResult.pntModel.X:F3} mm , Y : {stvisionResult.pntModel.Y:F3} mm / Model Index : {m_nNowModelIndex}");
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    Console.WriteLine(sLogMsg);


                    // 좌표계 변환. => Cam 기준 상대좌표. (3사분면 좌표계)
                    stvisionResult.pntModel.X *= g_VisionManager._RecipeVision.ResolutionX / 1000.0;
					stvisionResult.pntModel.Y *= g_VisionManager._RecipeVision.ResolutionY / 1000.0;

					fn_WriteLog($"[ X,Y Align Result ] X : {stvisionResult.pntModel.X:F3} mm , Y : {stvisionResult.pntModel.Y:F3} mm / Model Index : {m_nNowModelIndex}", EN_LOG_TYPE.ltVision);

					fn_WriteLog($"   X-Start : {stvisionResult.stRecipeList[0].pStartPos.X:F4}"	,EN_LOG_TYPE.ltVision);
					fn_WriteLog($"   X-End :   {stvisionResult.stRecipeList[0].pEndPos.X:F4}"	,EN_LOG_TYPE.ltVision);
                    fn_WriteLog($"   Y-Start : {stvisionResult.stRecipeList[0].pStartPos.Y:F4}"	,EN_LOG_TYPE.ltVision);
					fn_WriteLog($"   Y-End :   {stvisionResult.stRecipeList[0].pEndPos.Y:F4}"	,EN_LOG_TYPE.ltVision);

                    m_nVisnInspStep++;
                    return false;

                case 18:

					//stvisionResult.stRecipeList[0].pStartPos.X + 엔코더 X 현재 값.
					//stvisionResult.stRecipeList[0].pStartPos.Y + 엔코더 Y 현재 값.

					//stvisionResult.stRecipeList[0].pEndPos.X + 엔코더 X 현재 값.
					//stvisionResult.stRecipeList[0].pEndPos.Y + 엔코더 Y 현재 값.

					nTotal = stvisionResult.nTotalStep;
					dXpos  = GetEncPos_X();
					dYpos  = SEQ_POLIS.GetEncPos_Y ();
					dTHpos = SEQ_POLIS.GetEncPos_TH();

					//Save X,Y Value
					for (int n = 0; n< nTotal; n++)
					{
						if (stvisionResult.stRecipeList[n].nUseMilling != 1) continue;
						//		+ (Spindle)
						//	□ (Cam)
						// Spindle X Pos Value > Cam X Pos Value
						// Spindle이 더가야하므로 Offset + 부호

						//	+ (Spindle)
						//		□ (Cam)
						// Spindle X Pos Value < Cam X Pos Value
						// Spindle이 덜가야하므로 Offset - 부호

						stvisionResult.stRecipeList[n].dStartX = dXpos - stvisionResult.stRecipeList[n].pStartPos.X + g_VisionManager._RecipeVision.SpindleOffsetX;
						stvisionResult.stRecipeList[n].dEndX   = dXpos - stvisionResult.stRecipeList[n].pEndPos.X   + g_VisionManager._RecipeVision.SpindleOffsetX;
						
						stvisionResult.stRecipeList[n].dStartY = dYpos + stvisionResult.stRecipeList[n].pStartPos.Y - g_VisionManager._RecipeVision.SpindleOffsetY;
						stvisionResult.stRecipeList[n].dEndY   = dYpos + stvisionResult.stRecipeList[n].pEndPos.Y   - g_VisionManager._RecipeVision.SpindleOffsetY;

						fn_WriteLog($"-----------------------------Result Process------------------------", EN_LOG_TYPE.ltVision);
						fn_WriteLog($"{n} - Start Pos : {stvisionResult.stRecipeList[n].dStartX}, {stvisionResult.stRecipeList[n].dStartY}", EN_LOG_TYPE.ltVision);
						fn_WriteLog($"{n} - End   Pos : {stvisionResult.stRecipeList[n].dEndX  }, {stvisionResult.stRecipeList[n].dEndY  }", EN_LOG_TYPE.ltVision);
						stvisionResult.stRecipeList[n].dPosTH  = dTHpos ;

						//---------------------------------------------------------------------------
						// System Offset
						//---------------------------------------------------------------------------
						stvisionResult.stRecipeList[n].dStartX	+= (FM.m_stProjectBase.dPolishOffset_X * -1); //JUNG/200908
						stvisionResult.stRecipeList[n].dEndX	+= (FM.m_stProjectBase.dPolishOffset_X * -1); 

						stvisionResult.stRecipeList[n].dStartY	+= FM.m_stProjectBase.dPolishOffset_Y;
						stvisionResult.stRecipeList[n].dEndY	+= FM.m_stProjectBase.dPolishOffset_Y;

						stvisionResult.stRecipeList[n].dTilt	+= FM.m_stProjectBase.dPolishOffset_TI;
						//---------------------------------------------------------------------------
						// Theta Offset 은 추가 스탭 작업 요함.
						// => Theta 가공은 Align 이후 특정 각도를 돌려서 가공을 하기위함 이므로.
						//stvisionResult.stRecipeList[n].dPosTH += FM.m_stProjectBase.dPolishOffset_TH;
						//---------------------------------------------------------------------------

                        //---------------------------------------------------------------------------
                        //	Vision Align InPosition Error
                        //---------------------------------------------------------------------------
                        // Interpolation Tilt.
                        dPlateInterpolationPosY = dPlateCenterY + g_VisionManager.fn_GetTiltInterpolation(stvisionResult.stRecipeList[n].dTilt);
						
						//
						if (stvisionResult.stRecipeList[n].dStartX < dPlateInterpolationPosX - dXOffset || stvisionResult.stRecipeList[n].dStartX > dPlateInterpolationPosX + dXOffset) 
						{
                            fn_WriteLog($"Start X InPosition Error.{n}", EN_LOG_TYPE.ltVision);
                            bErr = true;
						}
						if (stvisionResult.stRecipeList[n].dEndX < dPlateInterpolationPosX - dXOffset || stvisionResult.stRecipeList[n].dEndX > dPlateInterpolationPosX + dXOffset)
						{
							fn_WriteLog($"End X InPosition Error.{n}", EN_LOG_TYPE.ltVision);
							bErr = true;
						}
						if (stvisionResult.stRecipeList[n].dStartY < dPlateInterpolationPosY - dYOffset || stvisionResult.stRecipeList[n].dStartY > dPlateInterpolationPosY + dYOffset)
						{
							fn_WriteLog($"Start Y InPosition Error.{n}", EN_LOG_TYPE.ltVision);
							bErr = true;
						}
						if (stvisionResult.stRecipeList[n].dEndY < dPlateInterpolationPosY - dYOffset || stvisionResult.stRecipeList[n].dEndY > dPlateInterpolationPosY + dYOffset)
						{
							fn_WriteLog($"End Y InPosition Error.{n}", EN_LOG_TYPE.ltVision);
							bErr = true;
						}
					}


                    //
					Console.WriteLine($"[ BEFORE ] ENC POS - X:{dXpos}/Y:{dYpos}");

                    Console.WriteLine($"[ Align Result ] START POS - X:{stvisionResult.stRecipeList[0].dStartX}/Y:{stvisionResult.stRecipeList[0].dStartY}");
                    Console.WriteLine($"[ Align Result ] END   POS - X:{stvisionResult.stRecipeList[0].dEndX  }/Y:{stvisionResult.stRecipeList[0].dEndY  }");
                    Console.WriteLine($"[ Align Result ] TH    POS - X:{stvisionResult.stRecipeList[0].dPosTH}");
                    Console.WriteLine($"[ Align Result ] TiltOffset- T:{stvisionResult.stRecipeList[0].dTiltOffset}");


					//Check Vision Align InPosition Error
					if (bErr)
					{
                        fn_WriteLog($"Polishing Bath X,Y Align Error - In-position Error.", EN_LOG_TYPE.ltVision);
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0563); // Polishing Bath X,Y Align Error - In-position Error

						Flag = false;
                        m_nVisnInspStep = 0;
                        return true;
                    }
					
					//
					vresult = stvisionResult;

					//File Save
					fn_LoadVisnResult(flSave);

					fn_WriteSeqLog("VISION Inspect OK");

					Console.WriteLine("VISION Inspect OK");
					fn_WriteLog($"VISION Inspect OK", EN_LOG_TYPE.ltLot);

					m_nCylRetryCnt = 0;
					m_nVisnInspStep++;
                    return false;

                case 19:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false; 

					//Check Retry Condition
					if(fn_GetLensCvrRetryBwd() || fn_GetIRRetryBwd())
					{
						if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
						{
							fn_MoveCylforCam(ccOpen);
							m_tDelayTime.Clear();

							m_nVisnBackStep = 19;
							m_nVisnInspStep = 31;
							return false;
						}
                    }

                    r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR     (ccClose);
					if (!r1 || !r2 ) return false;

                    m_nVisnInspStep++;
                    return false;

                case 20:

                    r3 = g_VisionManager.fn_SetLightValue(swOff); //Light Off
					if (!r3) return false;

					//Map - Polishing
					DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolish);

					Flag = false;
					m_nVisnInspStep = 0;
                    return true;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Cylinder Retry
				case 30:
					r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR     (ccClose);
					if (!r1 || !r2) return false;

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					sTemp = string.Format($"[VISN CYCLE] Cylinder Retry : Step = {m_nVisnBackStep} / Count = {m_nCylRetryCnt}");
					fn_WriteLog(sTemp);

					m_nVisnInspStep = m_nVisnBackStep;
                    return false;
                
				case 31:
                    r1 = fn_MoveCylLensCvr(ccOpen);
					// Global Optic
					if (IsPoliVisn)
					{
						if (g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].UseGlobalPolishing == 1)
							nIRFilter = g_VisionManager.CurrentRecipe.LightIRFilter[(int)EN_VISION_MODE.Polishing];
						else
							nIRFilter = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].PolishingLightIRFilter;
					}
					else
					{
						if (g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].UseGlobalLoading == 1)
							nIRFilter = g_VisionManager.CurrentRecipe.LightIRFilter[(int)EN_VISION_MODE.Loading];
						else
							nIRFilter = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].LoadingLightIRFilter;
					}

					r2 = fn_MoveCylIR(nIRFilter == 1 ? ccClose : ccOpen);

					if (!r1 || !r2) return false;

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;
					
					sTemp = string.Format($"[VISN CYCLE] Cylinder Retry : Step = {m_nVisnBackStep} / Count = {m_nCylRetryCnt}");
					fn_WriteLog(sTemp);

					m_nVisnInspStep = m_nVisnBackStep;
					return false;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Load Part : Pre - Align
                case 50:
                    if (SEQ._bStop)
                    {
                        m_nVisnInspStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Check Enable
                    if (g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].Enable == 0)
                    {
                        if (++m_nNowModelIndex < 10) return false;
                        else
                        {
							if (m_nVisnRetry > 0)
                            {
                                //
								if (FM.m_stSystemOpt.nUseSkipVisError == 1)
								{
									sLogMsg = string.Format($"FAIL - [Theta Load] Vision Error Skip - MAP FINISH");
									Console.WriteLine(sLogMsg);
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot   );
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

									fn_MoveCylLensCvr(ccClose);
									fn_MoveCylIR     (ccClose);

									//Map - Load
									DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo  ((int)EN_PLATE_STAT.ptsFinish);
									DM.MAGA[(int)EN_MAGA_ID.LOAD].SetInfo(EN_PLATE_INFO.ifVisnErr     );

									m_nVisnInspStep = 100;
									return false;
								}
								else
								{
                        			EPU.fn_SetErr(EN_ERR_LIST.ERR_0550); // Load Theta Align Error

									sLogMsg = string.Format($"FAIL - Load Theta Align {stvisionResult.dTheta}");
									Console.WriteLine(sLogMsg);
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot   );
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

									Flag = false;
									m_nVisnInspStep = 0;
									return true;
								}
                            }
                            else
                            {
                                EPU.fn_SetErr(EN_ERR_LIST.ERR_0564); //Vision Recipe Error - Enable Error
                                m_nVisnInspStep = 0;
                                Flag = false;
                                return true;
                            }
                        }
                    }
					else
					{
						//
						m_nVisnRetry = 0;
					}

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;


                case 51:

					//Check Cylinder Retry Condition
					if (fn_GetLensCvrRetryFwd() || fn_GetIRRetryFwd())
                    {
                        if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
                        {
                            fn_MoveCylforCam(ccClose);
                            m_tDelayTime.Clear();

                            m_nVisnBackStep = 51;
                            m_nVisnInspStep = 30;

                            return false;
                        }

                    }

					//
					r1 = fn_MoveCylLensCvr(ccOpen);
					r2 = SEQ_TRANS.fn_MoveCylTopTurn(ccDeg0);
					r3 = SEQ_TRANS.fn_ReqMoveVisnPos();

                    if (g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].UseGlobalLoading == 1)
                    {
                        nLightIR        = g_VisionManager.CurrentRecipe.LightIR           [(int)EN_VISION_MODE.Loading];
                        nLightWhite     = g_VisionManager.CurrentRecipe.LightWhite        [(int)EN_VISION_MODE.Loading];
                        nCameraExposure = g_VisionManager.CurrentRecipe.CameraExposureTime[(int)EN_VISION_MODE.Loading];
                        nCameraGain     = g_VisionManager.CurrentRecipe.CameraGain        [(int)EN_VISION_MODE.Loading];
						nIRFilter       = g_VisionManager.CurrentRecipe.LightIRFilter     [(int)EN_VISION_MODE.Loading];
					}
                    else
                    {
                        nLightIR        = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].LoadingLightIR;
                        nLightWhite     = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].LoadingLightWhite;
                        nCameraExposure = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].LoadingCameraExposureTime;
                        nCameraGain     = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].LoadingCameraGain;
						nIRFilter       = g_VisionManager.CurrentRecipe.Model[m_nNowModelIndex].LoadingLightIRFilter;
                    }
                    
					r4 = fn_MoveCylIR(nIRFilter == 1 ? ccClose : ccOpen);
                    if (!r1 || !r2 || !r3 || !r4) return false;

					//
                    g_VisionManager.fn_SetLightValue(swOn, nLightWhite, nLightIR); //Light On

                    // Camera Parameter Set 추가 할것.
                    g_VisionManager._CamManager.fn_SetExposure(nCameraExposure);
                    g_VisionManager._CamManager.fn_SetGain    (nCameraGain    );

                    m_tDelayTime.Clear();
					m_nVisnInspStep++;
                    return false;
				
				case 52:

					//
					if (m_bTESTMODE)
					{
                        m_nVisnInspStep = 55;
                        return false;
                    }

                    if (!m_tDelayTime.OnDelay(true, 3000)) return false; //Delay for stable

					fn_WriteLog($"---------------------------------PreAlign {m_nVisnRetry}------------------------------", EN_LOG_TYPE.ltVision);
					fn_WriteLog($"Pre-Align - Model Index : {m_nNowModelIndex+1} / Count : {m_nVisnRetry+1}", EN_LOG_TYPE.ltLot);

					if (!g_VisionManager.fn_PreAlign(m_nNowModelIndex, ref stvisionResult, bSkipAlign))
                    {
						//JUNG/Retry 2
						if(++m_nVisnRetry < 2)
						{
							Console.WriteLine( "PreAlign Retry : " + m_nVisnRetry);
							m_tDelayTime.Clear();
                            m_nVisnInspStep = 51;
                            return false;
                        }
						else
                        {
							if(++m_nNowModelIndex < 10)
                            {
                                //m_nVisnRetry = 0;

                                m_tDelayTime.Clear();
                                m_nVisnInspStep = 50;
                                return false;
                            }
                            else
                            {
								//LEE/200929 [Add] : Vision Error Skip
								if (FM.m_stSystemOpt.nUseSkipVisError == 1)
								{
									sLogMsg = string.Format($"FAIL - [Theta Load] Vision Error Skip - MAP FINISH");
									Console.WriteLine(sLogMsg);
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot   );
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

									fn_MoveCylLensCvr(ccClose);
									fn_MoveCylIR     (ccClose);

									//Map - Load
									DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo  ((int)EN_PLATE_STAT.ptsFinish);
									DM.MAGA[(int)EN_MAGA_ID.LOAD].SetInfo(EN_PLATE_INFO.ifVisnErr     );

									m_nVisnInspStep = 100;
									return false;
								}
								else
								{
                        			EPU.fn_SetErr(EN_ERR_LIST.ERR_0550); // Load Theta Align Error

									sLogMsg = string.Format($"FAIL - Load Theta Align {stvisionResult.dTheta}");
									Console.WriteLine(sLogMsg);
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot   );
									fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltVision);

									Flag = false;
									m_nVisnInspStep = 0;
									return true;
								}

                            }
                        }

                    }
                    
					Console.WriteLine($"Load Theta Align Result : {stvisionResult.dTheta}");
					fn_WriteLog($"Pre-Align Result : {stvisionResult.dTheta}", EN_LOG_TYPE.ltLot);


					//JUNG/200429/Check Position Error
					if (!MOTR.CheckMinMaxP(EN_MOTR_ID.miTRF_T, SEQ_TRANS.GetEncPos_TH() + (stvisionResult.dTheta * -1)))
					{
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0551); // Load Theta Align In-position Error - Out Of Range : Theta

                        Console.WriteLine("FAIL - Theta Align | Value : " + stvisionResult.dTheta);

                        Flag = false;
                        m_nVisnInspStep = 0;
                        return true;
                    }

					//
					MOTR[(int)EN_MOTR_ID.miTRF_T].MP.dPosn[(int)EN_POSN_ID.CalPos] = SEQ_TRANS.GetEncPos_TH() + (stvisionResult.dTheta * -1);

                    fn_MoveCylLensCvr(ccClose);
                    fn_MoveCylIR     (ccClose);

                    m_nVisnInspStep++;
                    return false;

                // Move Theta
                case 53:

					//r1 = MOTR.MoveMotrR(EN_MOTR_ID.miTRF_T, stvisionResult.dTheta * -1, 100);
					r1 = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_T, EN_COMD_ID.CalPos);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nVisnInspStep++;
                    return false;

                case 54:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					//r1 = MOTR[(int)EN_MOTR_ID.miTRF_T].GetStop();
                    //if (!r1) return false;

					Console.WriteLine($"[ Pre-Align Result ] TH : {SEQ_TRANS.GetEncPos_TH()} mm");

					m_tDelayTime.Clear();
					m_nVisnInspStep++;
                    return false;
				
				case 55:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					//File Save
					SEQ_TRANS.fn_SetPreAlignPos(SEQ_TRANS.GetEncPos_TH());

					fn_WriteSeqLog("Pre-Align Vision Inspect OK : " + SEQ_TRANS.GetEncPos_TH());

					fn_WriteLog("[Pre-Align] Vision Inspect OK : " + SEQ_TRANS.GetEncPos_TH(), EN_LOG_TYPE.ltLot);
					Console.WriteLine("Pre-Align Vision Inspect OK");

					g_VisionManager.fn_SetLightValue(swOff); //Light Off

					m_tDelayTime.Clear();
					m_nVisnInspStep++;
                    return false;

                case 56:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    //Check Retry Condition
                    if (fn_GetLensCvrRetryBwd() || fn_GetIRRetryBwd())
                    {
                        if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
                        {
                            fn_MoveCylforCam(ccOpen);
                            m_tDelayTime.Clear();

                            m_nVisnBackStep = 56;
                            m_nVisnInspStep = 31;
                            return false;
                        }
                    }

                    r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR     (ccClose);
                    if (!r1 || !r2) return false;

                    m_tDelayTime.Clear();
                    m_nVisnInspStep++;
                    return false;

				case 57:
					r3 = g_VisionManager.fn_SetLightValue(swOff); //Light Off
                    if (!r3) return false;

                    //Map - Align
                    DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo((int)EN_PLATE_STAT.ptsAlign);

                    Flag = false;
                    m_nVisnInspStep = 0;
                    return true;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Vision Error Skip
				case 100:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false; 

					//Check Retry Condition
					if(fn_GetLensCvrRetryBwd() || fn_GetIRRetryBwd())
					{
						if (++m_nCylRetryCnt < fn_GetLensCvrRetryCnt())
						{
							fn_MoveCylforCam(ccOpen);
							m_tDelayTime.Clear();

							m_nVisnBackStep = 100;
							m_nVisnInspStep = 31;
							return false;
						}
                    }

                    r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR     (ccClose);
					if (!r1 || !r2 ) return false;

                    m_nVisnInspStep++;
                    return false;

                case 101:

                    r3 = g_VisionManager.fn_SetLightValue(swOff); //Light Off
					if (!r3) return false;

					Flag = false;
					m_nVisnInspStep = 0;
                    return true;

            }
        }
		//---------------------------------------------------------------------------
        private bool fn_UtilCheckCycle(ref bool Flag)
        {
			//Check overflow of Polishing Bath while Utility supply
			bool   r1, r2;
			//double dUtilValue = 0.0;
			//double dOffset    = 0.0; 

			//
			m_iCmdX = EN_COMD_ID.User9;

            //Utility Exist Check Cycle
            if (m_nUtilChkStep < 0) m_nUtilChkStep = 0;

			switch (m_nUtilChkStep)
			{
				default:
					m_nUtilChkStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;

					m_bReqUtil_Polish = false;

					m_nUtilChkStep++;
					return false;

				case 11: 
					if(SEQ._bStop)
					{
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nUtilChkStep = 0;
                        Flag = false;
                        return true;
					}
					
					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					m_nUtilChkStep++;
                    return false;

                case 12:
                    if (SEQ._bStop)
                    {
						m_nUtilChkStep = 0;
                        Flag = false;
                        return true;
                    }
					
					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					r2 = SEQ_POLIS.fn_ReqMoveUtilChkPosn(); //fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					m_nUtilChkStep++;
                    return false;

                case 13:

                    //Wait during supply Polishing Utility 
                    if (SEQ_POLIS._bDrngUtility)
					{
                        return false;
                    }

                    m_nUtilChkStep++;
                    return false;

                case 14:
                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
					
					Flag = false;
					m_nUtilChkStep = 0;
                    return true;

            }

        }
		//---------------------------------------------------------------------------
        private bool fn_UtilExtCheckCycle(ref bool Flag)
        {
			//Check Utility of Polishing Bath
			bool   r1, r2;
			//double dUtilValue = 0.0, dOffset = 0.0; 
			
			//
			m_iCmdX = EN_COMD_ID.User9;

            //Utility Exist Check Cycle
            if (m_nUtilChkStep < 0) m_nUtilChkStep = 0;

			switch (m_nUtilChkStep)
			{
				default:
					m_nUtilChkStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;

					m_nUtilChkStep++;
					return false;

				case 11: 
					if(SEQ._bStop)
					{
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nUtilChkStep = 0;
                        Flag = false;
                        return true;
					}
					
					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					m_nUtilChkStep++;
                    return false;

                case 12:
                    if (SEQ._bStop)
                    {
						m_nUtilChkStep = 0;
                        Flag = false;
                        return true;
                    }
					
					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					r2 = SEQ_POLIS.fn_ReqMoveUtilChkPosn(); //fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					//IO.fn_ClearUTQue();

					m_tDelayTime.Clear();

					m_nUtilChkStep++;
                    return false;

                case 13:
					if (!m_tDelayTime.OnDelay(true, 1000 * 5)) return false;

					//초음파센서 확인
					if (IO.fn_IsUTLevelDone())
					{
						SEQ_POLIS.fn_SetUtilState(EN_UTIL_STATE.Exist);
					}
					else
					{
						SEQ_POLIS.fn_SetUtilState(EN_UTIL_STATE.Empty);
					}

					m_tDelayTime.Clear();
					
					Flag = false;
					m_nUtilChkStep = 0;
                    return true;

            }
        }
		//-------------------------------------------------------------------------------------------------
        private bool fn_CupInPosCycle(ref bool Flag)
        {
            //"Cup Storage P/P Pos."      ,EN_POSN_ID.User15 , 3, EN_MOTR_ID.miSPD_X);
            //"Polishing Cup P/P Pos."    ,EN_POSN_ID.User16 , 3, EN_MOTR_ID.miSPD_X);
										  
            //"Polishing Cup In/Out Pos." ,EN_POSN_ID.User3, 3, EN_MOTR_ID.miPOL_Y); //Cup In/Out Position
            //"Cup Storage In/Out Pos."   ,EN_POSN_ID.User4, 3, EN_MOTR_ID.miPOL_Y); //Storage Cup In/Out Position
										  
            //"Cup Storage P/P Pos."      , EN_POSN_ID.User5, 3, EN_MOTR_ID.miSPD_Z1);
            //"Polishing Cup Pick Pos."   , EN_POSN_ID.User6, 3, EN_MOTR_ID.miSPD_Z1);
            //"Polishing Cup Place Pos."  , EN_POSN_ID.User7, 3, EN_MOTR_ID.miSPD_Z1);
										  
            bool r1, r2, r3;
			
			//
			m_iCmdX = EN_COMD_ID.User9;

            //
            if (m_nCupMoveStep < 0) m_nCupMoveStep = 0;

			switch (m_nCupMoveStep)
			{
				default:
					m_nCupMoveStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;
                  
					r1 = fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User16); //Polishing Cup P/P Position.
                    r2 = SEQ_POLIS.fn_ReqBathCupInOutPos(); //Polishing Cup In/Out Position
					r3 = fn_MoveCylClamp(ccBwd); 
					if (!r1 || !r2 || !r3) return false;

					fn_WriteLog("[START] Move Cup In-position", EN_LOG_TYPE.ltLot);

					m_tDelayTime.Clear();
                    
					m_nCupMoveStep++;
                    return false;

                case 11:
                    
					if (SEQ._bStop)
                    {
						m_nCupMoveStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.User6)) return false; //Polishing Cup Storage Pick Pos

					m_tDelayTime.Clear();
					m_nCupMoveStep++;
					return false;

				case 12:

					if (!m_tDelayTime.OnDelay(true, 300)) return false;
					if (!fn_MoveCylClamp(ccFwd)) return false;

					m_tDelayTime.Clear();
					m_nCupMoveStep++;
					return false;

				case 13:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;
					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1)) return false; //wait Position - Z Axis

					m_tDelayTime.Clear();
					m_nCupMoveStep++;
					return false;

				case 14:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//JUNG/200612/Y-Axis move --> X-Axis move
					if (!SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User4)) return false; //Cup Storage Position - Y Axis
					if (!fn_MoveMotr(m_iMotrXId, EN_COMD_ID.User15)                    ) return false; //Cup Position - X Axis

					m_tDelayTime.Clear();
					m_nCupMoveStep++;
					return false;

				case 15:

					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.User5)) return false; //Storage Cup P/P Position - Z Axis

					m_tDelayTime.Clear();
					m_nCupMoveStep++;
					return false;

				case 16:
					if (!m_tDelayTime.OnDelay(true, 100)) return false;

					if (!fn_MoveCylClamp(ccBwd)) return false;

					m_tDelayTime.Clear();
					m_nCupMoveStep++;
					return false;

				case 17:
					if (!m_tDelayTime.OnDelay(true, 100)) return false;
					if (!fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1)) return false; //wait Position - Z Axis

					//Check Cup Exist
                    if (!SEQ_POLIS.fn_IsExistCup(true))
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0353); //Polishing Cup Loss Error
                    }

                    fn_WriteLog("[END] Move Cup In-position", EN_LOG_TYPE.ltLot);

					m_tDelayTime.Clear();
					m_nCupMoveStep = 0;
                    return true;


            }
        }

		//---------------------------------------------------------------------------
		private bool fn_PlatePickCycle(ref bool Flag, PLATE_MOVE_INFO pickinfo)
        {
			//
			bool r1, r2, r3; 
			bool isLoadPort   =  pickinfo.nBtmPlate == (int)EN_MAGA_ID.LOAD  ;
			bool isPolishBath =  pickinfo.nBtmPlate == (int)EN_MAGA_ID.POLISH;
			bool isCleanBath  =  pickinfo.nBtmPlate == (int)EN_MAGA_ID.CLEAN ;
			bool bPoliUtilExt = !SEQ_POLIS._bEmptyUitl ;

			if      (isLoadPort  ) enPlateId = EN_PLATE_ID.ptiLoad   ;
			else if (isPolishBath) enPlateId = EN_PLATE_ID.ptiPolish ;
			else if (isCleanBath ) enPlateId = EN_PLATE_ID.ptiClean  ;

			//Place Cycle
			if (m_nMagzPickStep < 0) m_nMagzPickStep = 0;

			switch (m_nMagzPickStep)
			{
				default:
					m_nMagzPickStep = 0;
					Flag = false;
                    m_bDrngPlatePickP = false;
                    m_bDrngPlatePickC = false;
					return true;

				case 10:

					Flag = true;

					if      (isPolishBath) { m_iCmdX = EN_COMD_ID.User6; m_iCmdZ1 = EN_COMD_ID.User3; m_bDrngPlatePickP = true; }
					else if (isCleanBath ) { m_iCmdX = EN_COMD_ID.User7; m_iCmdZ1 = EN_COMD_ID.User4; m_bDrngPlatePickC = true; }
					else if (isLoadPort  ) { m_iCmdX = EN_COMD_ID.User8; m_iCmdZ1 = EN_COMD_ID.User1; }
					else
					{
						m_nMagzPickStep = 0;
						Flag = false;
						return true;
					}

					if(m_bTESTMODE)
                    {
						MOTR[(int)EN_MOTR_ID.miSPD_Z1].MP.dPosn[(int)EN_POSN_ID.Pick1] = 10;
						m_iCmdZ1 = EN_COMD_ID.Pick1;
					}

                    //
                    fn_WriteSeqLog($"[START] Place pick from {enPlateId.ToString()}");

					//
					if(isLoadPort)
					{
						//Polishing Drain
						SEQ_POLIS._bReqDrain = true;
						
						fn_WriteSeqLog("Request Polishing Drain");
					}
					else if(isPolishBath) //
                    {
						if (bPoliUtilExt) SEQ_POLIS.fn_SetDrain();
                    }

                    m_nMagzPickStep++;
					return false;

				case 11:
					if (SEQ._bStop)
					{
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nMagzPickStep = 0;
						Flag = false;
						m_bDrngPlatePickP = false;
						m_bDrngPlatePickC = false;
						return true;
					}

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					if (fn_IsExistPlate(false) && !m_bTESTMODE)
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0357);

						m_nMagzPickStep = 0;
						Flag = false;
						m_bDrngPlatePickP = false;
						m_bDrngPlatePickC = false;
						return true;
					}

					fn_MoveCylClamp(ccBwd);

					m_nMagzPickStep++;
					return false;

				case 12:
					if (SEQ._bStop)
					{
						m_nMagzPickStep = 0;
						Flag = false;
						m_bDrngPlatePickP = false;
						m_bDrngPlatePickC = false;
						return true;
					}

					if (isPolishBath)
					{
						//SEQ_POLIS.fn_MoveCylClamp(ccBwd); //JUNG/200613
						r3 = SEQ_POLIS.fn_ReqBathWaitPos(); 

					}
					else if (isCleanBath)
					{
						//SEQ_CLEAN.fn_MoveCylClamp(ccBwd); //JUNG/200613
						r3 = SEQ_CLEAN.fn_ReqBathWaitPos();
					}
					else
					{
						//SEQ_TRANS.fn_ReqMoveAlignPos();
						SEQ_TRANS.fn_MoveCylLoadUpDown(ccUp);
						r3 = SEQ_TRANS.fn_MoveCylTRTurnPos();
					}

					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					r2 = fn_MoveCylClamp(ccBwd);
					if (!r1 || !r2 || !r3) return false;

					m_nMagzPickStep++;
					return false;

				case 13:
					if (SEQ._bStop)
					{
						m_nMagzPickStep = 0;
						Flag = false;
                        m_bDrngPlatePickP = false;
                        m_bDrngPlatePickC = false;
                        return true;
					}

					if (isPolishBath)
					{
						r1 =  SEQ_POLIS.fn_MoveCylClamp  (ccBwd);
						r2 =  SEQ_POLIS.fn_ReqBathWaitPos();
						r3 = !SEQ_POLIS._bDrngSeqDrain; //JUNG/200915/Check During Drains
					}
					else if (isCleanBath)
					{
						r1 = SEQ_CLEAN.fn_MoveCylClamp(ccBwd);
						r2 = SEQ_CLEAN.fn_ReqBathWaitPos();
						r3 = true; 
					}
					else
					{
						r1 = SEQ_TRANS.fn_ReqMoveAlignPos(); //T-Axis Position
						r2 = SEQ_TRANS.fn_MoveCylLoadUpDown(ccUp );
						r3 = SEQ_TRANS.fn_MoveCylLoadCover (ccBwd);
					}
					if (!r1 || !r2 || !r3) return false;

					m_nMagzPickStep++;
					return false;

				case 14:
					if (SEQ._bStop)
					{
						Flag = false;
                        m_bDrngPlatePickP = false;
                        m_bDrngPlatePickC = false;

                        m_nMagzPickStep = 0;
                        return true;
					}

					r1 = fn_MoveMotr(m_iMotrZ1Id, m_iCmdZ1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nMagzPickStep++;
					return false;

				case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveCylClamp(ccFwd);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nMagzPickStep++;
					return false;


				case 16:
					if (!m_tDelayTime.OnDelay(true, 500)) return false; //500ms Delay

					r1 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1) return false;


					if (fn_IsExistPlate(true) || m_bTESTMODE)
					{
						//Data Shift
						DM.ShiftPickPlateData(enPlateId);
						
						//
						SEQ._bRecipeOpen = true; 
					}
					else
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0332);
					}

                    //
                    fn_WriteSeqLog($"[END] Place pick from {enPlateId.ToString()}");


                    //
                    Flag              = false;
					m_nMagzPickStep   = 0;
                    m_bDrngPlatePickP = false;
                    m_bDrngPlatePickC = false;
                    return true;
			}

        }
		//---------------------------------------------------------------------------
		/**    
		<summary>
			Plate Plate Cycle
		</summary>
		<param name="Flag"> During Flag </param>
		<param name="pickinfo"> Plate Move Info </param>
		@author    정지완(JUNGJIWAN)
		@date      2020/03/10 17:20
		*/
		
		private bool fn_PlatePlaceCycle(ref bool Flag, PLATE_MOVE_INFO pickinfo)
        {
			//
			bool r1, r2, r3; 
			bool isLoadPort   = pickinfo.nBtmPlate == (int)EN_MAGA_ID.LOAD  ;
			bool isPolishBath = pickinfo.nBtmPlate == (int)EN_MAGA_ID.POLISH;
			bool isCleanBath  = pickinfo.nBtmPlate == (int)EN_MAGA_ID.CLEAN ;
		

			if      (isPolishBath) enPlateId = EN_PLATE_ID.ptiPolish;
			else if (isCleanBath ) enPlateId = EN_PLATE_ID.ptiClean ;
            else if (isLoadPort  ) enPlateId = EN_PLATE_ID.ptiLoad  ;

            //Place Cycle
            if (m_nMagzPlaceStep < 0) m_nMagzPlaceStep = 0;

			switch (m_nMagzPlaceStep)
			{
				default: 
					m_nMagzPlaceStep = 0;
					Flag = false;
					return true;

				case 10:
					
					Flag = true ;
					
					if      (isPolishBath) { m_iCmdX = EN_COMD_ID.User6; m_iCmdZ1 = EN_COMD_ID.User13;  m_bDrngPlatePlceP = true  ;  }
					else if (isCleanBath ) { m_iCmdX = EN_COMD_ID.User7; m_iCmdZ1 = EN_COMD_ID.User14;  m_bDrngPlatePlceC = true  ;  }
					else if (isLoadPort  ) { m_iCmdX = EN_COMD_ID.User8; m_iCmdZ1 = EN_COMD_ID.User11;  }
					else
					{
                        m_nMagzPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (m_bTESTMODE)
                    {
                        MOTR[(int)EN_MOTR_ID.miSPD_Z1].MP.dPosn[(int)EN_POSN_ID.Plce1] = 9;
                        m_iCmdZ1 = EN_COMD_ID.Plce1;
                    }

                    fn_MoveCylClamp(ccFwd);

					//g_VisionManager._AlginMainCtrl.fn_ClearResult(); //JUNG/210127
					g_VisionManager.delUpdateMainresultClear?.Invoke();
					//
					fn_WriteSeqLog($"[START] Place Plate to {enPlateId.ToString()}");


                    m_nMagzPlaceStep++;
					return false;

				case 11: 
					if(SEQ._bStop)
					{
						MOTR.Stop(m_iMotrZId); //JUNG/201120
						m_nMagzPlaceStep = 0;
                        Flag = false;
                        m_bDrngPlatePlceP = false;
                        m_bDrngPlatePlceC = false;
                        return true;
					}
					
					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					if(!fn_IsExistPlate(true) && !m_bTESTMODE)
					{
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0352);

                        m_nMagzPlaceStep = 0;
                        Flag = false;
                        m_bDrngPlatePlceP = false;
                        m_bDrngPlatePlceC = false;
                        return true;

                    }

                    m_nMagzPlaceStep++;
                    return false;

                case 12:
                    if (SEQ._bStop)
                    {
                        m_nMagzPlaceStep = 0;
                        Flag = false;
                        m_bDrngPlatePlceP = false;
                        m_bDrngPlatePlceC = false;
                        return true;
                    }

					if (isPolishBath)
					{
						SEQ_POLIS.fn_MoveCylClamp(ccBwd);
						SEQ_POLIS.fn_ReqBathWaitPos();
						r2 = true; 

					}
					else if (isCleanBath)
					{
						SEQ_CLEAN.fn_MoveCylClamp(ccBwd);
						SEQ_CLEAN.fn_ReqBathWaitPos();
						r2 = true; 
					}
					else
					{
						//SEQ_TRANS.fn_ReqMoveAlignPos();
						SEQ_TRANS.fn_MoveCylLoadCover (ccBwd);
						r2 = SEQ_TRANS.fn_MoveCylTRTurnPos();
					}

					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					if (!r1 || !r2) return false; 

                    m_nMagzPlaceStep++;
                    return false;

                case 13:
                    if (SEQ._bStop)
                    {
                        m_nMagzPlaceStep = 0;
                        Flag = false;
                        m_bDrngPlatePlceP = false;
                        m_bDrngPlatePlceC = false;
                        return true;
                    }

					if (isPolishBath)
					{
						r1 = SEQ_POLIS.fn_MoveCylClamp(ccBwd);
						r2 = SEQ_POLIS.fn_ReqBathWaitPos();
						r3 = true;
					}
					else if (isCleanBath)
					{
						r1 = SEQ_CLEAN.fn_MoveCylClamp(ccBwd);
						r2 = SEQ_CLEAN.fn_ReqBathWaitPos();
						r3 = true;
					}
					else
					{
						r1 = SEQ_TRANS.fn_ReqMoveAlignPos();
						r2 = SEQ_TRANS.fn_MoveCylLoadUpDown(ccUp);
						r3 = SEQ_TRANS.fn_MoveCylLoadCover(ccBwd);
					}
					if (!r1 || !r2 || !r3) return false;

					m_nMagzPlaceStep++;
                    return false;

                case 14:
                    if (SEQ._bStop)
                    {
                        m_nMagzPlaceStep = 0;
                        Flag = false;
                        m_bDrngPlatePlceP = false;
                        m_bDrngPlatePlceC = false;
                        return true;
                    }

					r1 = fn_MoveMotr(m_iMotrZ1Id, m_iCmdZ1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nMagzPlaceStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveCylClamp(ccBwd);
                    if (!r1) return false;

					m_nMagzPlaceStep++;
                    return false;


                case 16:

                    r1 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1) return false;


                    if (!fn_IsExistPlate(false) || m_bTESTMODE)
                    {
                        //Data Shift
						DM.ShiftPlacePlateData(enPlateId); // Map Change to Align at Polishing
					}
                    else
                    {
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0333);
                    }

					//
// 					if(m_bTESTMODE)
// 					{
// 						if(DM.MAGA[(int)EN_MAGA_ID.UNLOAD].IsAllStat((int)EN_PLATE_STAT.ptsFinish))
// 						{
// 							DM.MAGA[(int)EN_MAGA_ID.UNLOAD].ClearMap();
// 							DM.MAGA[(int)EN_MAGA_ID.LOAD  ].SetTo((int)EN_PLATE_STAT.ptsLoad);
// 						}
// 					}

					//Clamp Fwd
					if (!EPU.fn_GetHasErr())
					{
						if      (isPolishBath) SEQ_POLIS.fn_MoveCylClamp(ccFwd);
						else if (isCleanBath ) SEQ_CLEAN.fn_MoveCylClamp(ccFwd);
						else
						{

						}
					}

                    //
                    fn_WriteSeqLog($"[END] Place Plate to {enPlateId.ToString()}");
					
					m_tDelayTime.Clear();
					
					Flag = false;
					m_nMagzPlaceStep = 0;
                    m_bDrngPlatePlceP = false;
                    m_bDrngPlatePlceC = false;

                    return true;

            }
        }
		//---------------------------------------------------------------------------
		public bool fn_RecipeSetToACS(int step)
		{
			//Send step Data to ACS Buffer













			return true; 
		}
		//---------------------------------------------------------------------------
		/**    
		<summary>
			Check tool force 
		</summary>
		<param name="UserSet"></param>
		@author    정지완(JUNGJIWAN)
		@date      2020/03/09 11:58
		*/
		public bool fn_IsCheckForce(bool UserSet = false)
		{
			bool xPinExist  =  DM.TOOL.IsCheckForce(); //DM.TOOL.IsCheckExist() && 
			bool isTestMode = (FM.m_stMasterOpt.nToolSkip == 1) && !FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
			//bool bExist     =  isTestMode ? UserSet : xPinExist;
			bool bExist = xPinExist;

			return bExist;
		}
		//---------------------------------------------------------------------------
		/**    
		<summary>
			Check Tool Exist Sensor at Exist Check Position
		</summary>
		<param name=""></param>
		@author    정지완(JUNGJIWAN)
		@date      2020/03/09 11:57
		*/
		public bool fn_IsExistTool(bool UserSet = false)
		{
			bool xPinExist  =  IO.XV[(int)EN_INPUT_ID.xSPD_ToolExist];
			bool isTestMode = (FM.m_stMasterOpt.nToolSkip == 1) && !FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
			bool bExist     =  isTestMode ? UserSet : xPinExist;

			return bExist;
		}
		//---------------------------------------------------------------------------
		public bool fn_IsExistPlate(bool UserSet = false)
		{
			bool xStrExist = IO.XV[(int)EN_INPUT_ID.xSPD_PlateExistChk];

            bool isTestMode = (FM.m_stMasterOpt.nPlateSkip == 1) && !FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
            bool bExist = isTestMode ? UserSet : xStrExist;

            return bExist;
			
		}
		//---------------------------------------------------------------------------
		public bool fn_IsExistCup(bool UserSet = false)
		{
			bool xStrExist = IO.XV[(int)EN_INPUT_ID.xPOL_CupExistChk];

            bool isTestMode = (FM.m_stMasterOpt.nPlateSkip == 1) && !FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
            bool bExist = isTestMode ? UserSet : xStrExist;

            return bExist;
			
		}

        //---------------------------------------------------------------------------
        public bool fn_IsExistStrage(bool UserSet = false)
        {
            bool xStrExist  = !IO.XV[(int)EN_INPUT_ID.xSTR_PosCheck];
			bool isTestMode = (FM.m_stMasterOpt.nToolSkip == 1) && !FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
			bool bExist     = isTestMode ? UserSet : xStrExist;

            return bExist;
        }

		//---------------------------------------------------------------------------
		public bool fn_ToolPickOneCycle()
		{
			bool r1, r2, r3;
			bool xPinExist  = IO.XV[(int)EN_INPUT_ID.xSPD_ToolExist];
			bool xStorExist = IO.XV[(int)EN_INPUT_ID.xSTR_PosCheck ]; //Storage Check
			bool bUseDirPos = FM.m_stMasterOpt.nUseDirPos == 1;

			ST_PIN_POS Pos1 = new ST_PIN_POS(0);

			//Pick Cycle
			if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				case 10:
					if (fn_IsExistTool())
					{
						fn_UserMsg("Spindle already has Tool. Please Check.");
						m_nManStep = 0;
						return true;
					}

					// 					if(!fn_UserMsg("Did you check tool is empty?", EN_MSG_TYPE.Check))
					// 					{
					//                         m_nManStep = 0;
					//                         return true;
					//                     }

					if (!fn_IsExistStrage(true))
					{
						fn_UserMsg("The Storage is not detected.");
						m_nManStep = 0;
						return true;

					}
					m_nManStep++;
					return false;

				case 11:
					r1 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);
					r3 = fn_MoveToolClamp(ccBwd);
					if (!r1 || !r2) return false;

					m_nManStep++;
					return false;

				case 12:
					if (bUseDirPos)
					{
						Pos1 = DM.STOR[(int)EN_STOR_ID.POLISH].GetPinPos(0, 0);
						r1 = fn_MoveDirect(m_iMotrXId, Pos1.dXPos);
                        r2 = SEQ_STORG.fn_ReqMoveDirect(m_iBtmMotr, Pos1.dYPos);
						
					}
					else
					{
                        r1 = fn_MoveMotr(EN_MOTR_ID.miSPD_X, EN_COMD_ID.FindStep1);
                        r2 = SEQ_STORG.fn_ReqMoveMotr(EN_MOTR_ID.miSTR_Y, EN_COMD_ID.FindStep1);
                    }
					r3 = fn_MoveToolClamp(ccBwd);
					if (!r1 || !r2 || !r3) return false;

					m_nManStep++;
                    return false;

				case 13:
                    r1 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z, EN_COMD_ID.User3, EN_MOTR_VEL.Dry); //Tool Pick Pos.
                    if (!r1) return false;
					m_nManStep++;
                    return false;

                case 14:
                    r1 = fn_MoveToolClamp(ccFwd);
                    if (!r1) return false;
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 500)) return false; 
                    r1 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z, EN_COMD_ID.Wait1); 
                    if (!r1) return false;
					m_nManStep++;
                    return false;

                case 16:
 					if (!fn_IsExistTool(true))
 					{
 					    fn_UserMsg("Tool is not detected!!! Please Check.");
 					}
 					else
 					{
 						fn_UserMsg("Tool Pick One Cycle OK.");
 					}

					m_nManStep = 0;
                    return true;
			}


			return false; 
		}
		//---------------------------------------------------------------------------
		public bool fn_ToolCheckOneCycle()
        {
			bool r1, r2;
            
			//Tool Exist Check One Cycle
            if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:

					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nManStep = 13;
                    return false;

//                 case 11:
// 					m_iCmdX = EN_COMD_ID.User15; 
// 					m_iCmdZ = EN_COMD_ID.User6 ;
                    
// 					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //X-Axis Tool Check Position
//                     if (!r1) return false;
// 
// 					m_nManStep++;
//                     return false;
// 
//                 case 12:
// 
//                     r1 = fn_MoveMotr(m_iMotrZId, m_iCmdZ); //Z-Axis Tool Check Position
//                     if (!r1) return false;
// 
// 					m_tDelayTime.Clear();
// 					m_nManStep++;
//                     return false;

                case 13:
					if (!m_tDelayTime.OnDelay(true, 200)) return false;

					//Check Tool
					if (fn_IsExistTool())
					{
						fn_UserMsg("[Tool Check] Tool Exist!!!");
					}
					else
					{
						fn_UserMsg("[Tool Check] Tool Empty~~");
					}

                    m_nManStep++;
                    return false;

                case 14:
					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

                    m_nManStep = 0;
                    return true;
            }
            
        }
		//---------------------------------------------------------------------------
		public bool fn_ForceCheckOneCycle(bool poli = false) //409 ~ 410
        {
            bool   r1, r2;
			double dPos     = 0.0;
			double dSetDCOM = 3.0;
			
			m_iCmdX = poli ? EN_COMD_ID.User1 : EN_COMD_ID.User5;
			m_iCmdZ = poli ? EN_COMD_ID.User1 : EN_COMD_ID.User5; //Force Check Pos.

			//Tool Force Check One Cycle
			if (m_nManStep < 0) m_nManStep = 0;
            switch (m_nManStep)
            {
                default:
                    m_nManStep = 0;
                    return true;

                case 10:

					//
					m_nCalCount = 0;

					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    m_nManStep++;
                    return false;

                case 11:
                    r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //X-Axis Tool Check Position
					r2 = poli ? SEQ_POLIS.fn_ReqMoveForceTestPosn() : SEQ_TRANS.fn_ReqMoveVisnPos() ; 

					if (!r1 || !r2) return false;

                    m_nManStep++;
                    return false;

                case 12:

					dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(m_iCmdZ);

					//ACS Buffer run for Force check
					if(!IO.fn_ForceBufferRunforCal(dPos, dSetDCOM))
					{
						fn_UserMsg("[FORCE] Force Check One Cycle Error!!! <Buffer>");
						m_nManStep=0;
                        return true;
                    }

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 13:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//Check Buffer Run State
					if (!IO.fn_IsForceBuffRun()) return false; 

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun()) return false;
                    m_nManStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 3000)) return false; //

                    //Check Buffer State
                    if (IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_45_CheckForceBuffer] == 1)
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0344); // Force Check Error
                    }

                    IO.fn_SetOpenLoopOff();

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 16:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

                    m_nManStep = 0;
                    return true;

            }
        }
		//---------------------------------------------------------------------------
		public bool fn_ForceCheckOneCycle(int where, double dcom) //
        {
            bool   r1, r2;
			double dPos     = 0.0;
			double dSetDCOM = dcom;
			
			//where : 0=Polishing, 1=Cleaning, 2=Loadcell
			m_iCmdX = where == 0 ? EN_COMD_ID.User1 : (where == 1 ? EN_COMD_ID.User2 : EN_COMD_ID.User5);
			m_iCmdZ = where == 0 ? EN_COMD_ID.User1 : (where == 1 ? EN_COMD_ID.User2 : EN_COMD_ID.User5);

			//Tool Force Check One Cycle
			if (m_nManStep < 0) m_nManStep = 0;
            switch (m_nManStep)
            {
                default:
                    m_nManStep = 0;
                    return true;

                case 10:

					//
					m_nCalCount = 0;

					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    m_nManStep++;
                    return false;

                case 11:

                    //Check Tool Exist
                    if (!fn_IsExistTool(true))
                    {
						fn_UserMsg("[FORCE] Please Check Tool.");

						m_nManStep = 0;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //X-Axis Tool Check Position
					r2 = where==0 ? SEQ_POLIS.fn_ReqMoveForceTestPosn() : (where == 1 ? SEQ_CLEAN.fn_ReqCleanPos() : SEQ_TRANS.fn_ReqMoveVisnPos()) ; 

					if (!r1 || !r2) return false;

                    m_nManStep++;
                    return false;

                case 12:

					dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(m_iCmdZ);

					//ACS Buffer run for Force check
					if(!IO.fn_ForceBufferRunforCal(dPos, dSetDCOM))
					{
						fn_UserMsg("[FORCE] Force Check One Cycle Error!!! <Buffer>");
						m_nManStep=0;
                        return true;
                    }

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 13:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//Check Buffer Run State
					if (!IO.fn_IsForceBuffRun()) return false; 

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun()) return false;

                    m_nManStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 3000)) return false; //

                    //Check Buffer State
                    if (IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_45_CheckForceBuffer] == 1)
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0344); // Force Check Error
                    }

                    IO.fn_SetOpenLoopOff();

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 16:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

                    m_nManStep = 0;
                    return true;

            }
        }

		//---------------------------------------------------------------------------
		public bool fn_ForceOneCycle(bool poli)
        {//
            bool   r1, r2;
			double dPos = 0.0;

			//Force Check One Cycle
			if (m_nManStep < 0) m_nManStep = 0;
            switch (m_nManStep)
            {
                default:
                    m_nManStep = 0;
                    return true;

                case 10:
					
					m_iCmdX = poli ? EN_COMD_ID.User1 : EN_COMD_ID.User5;
                    m_iCmdZ = poli ? EN_COMD_ID.User1 : EN_COMD_ID.User5 ; //Force Check Pos.

					//
					m_nCalCount = 0;

					fn_LoadcellInit();

					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    m_nManStep++;
                    return false;

                case 11:
                    r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //X-Axis Tool Check Position
                    if (!r1) return false;

                    m_nManStep++;
                    return false;

                case 12:

					dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(m_iCmdZ);

					//ACS Buffer run for Force check
					if(!IO.fn_ForceBufferRun(dPos))
					{
						fn_UserMsg("[FORCE] Force Check One Cycle Error!!! <Buffer>");
						m_nManStep=0;
                        return true;
                    }

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 13:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//Check Buffer Run State
					if (!IO.fn_IsForceBuffRun()) return false; 

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun()) return false;
                    m_nManStep++;
                    return false;

                case 15:

                    if (m_bForceStop)
					{
						IO.fn_ForceBufferStop();
						IO.fn_SetOpenLoopOff();
						m_tDelayTime.Clear();
						m_nManStep++;
                        return false;
                    }

					//Calibrate Force Value
					if (!fn_CalLoadCell(poli))
					{
						if (m_nCalCount++ < 5)
						{
							fn_WriteLog(string.Format($"Force Check retry.[Count = {m_nCalCount}]"));
							
							IO.fn_SetOpenLoopOff();
							m_tDelayTime.Clear();
							m_nManStep = 50;
							return false;
						}
					}

                    IO.fn_SetOpenLoopOff();

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 16:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

                    m_nManStep = 0;
                    return true;
                

				//Do Calibration again.
				case 50:
                    
					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

                    m_nManStep = 12;
                    return false;

            }
        }

		//---------------------------------------------------------------------------
		public bool fn_CalLoadCell(bool polibath = false)
		{
			bool   bOk          = false;
			bool   bAutoMode    = SEQ._bAuto;
			bool   isCleanForce = DM.MAGA[(int)EN_MAGA_ID.CLEAN].IsStatOne(EN_PLATE_STAT.ptsClean);
			double dSetForceSys = FM.m_stSystemOpt.dTargetForce;
			double dPoliForce   = vresult.stRecipeList[m_nPoliCnt].dForce ;
			double dCleanForce  = g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].Force;
			double dSetForce    = bAutoMode? (isCleanForce ? dCleanForce : dPoliForce) : dSetForceSys; //JUNG/200824
			int    nInValue     = 10  ; //offset 10->20
			double dCompVale    = 0   ;
			double dOffset      = 0.0 ;
			string sTemp        = string.Empty;
			//int m_nTopForceValue = 0;
			//int m_nForceOffset   = 0;

			//Bottom Load Cell Value가 사용자가 설정한 값이랑 맞는가.
			//OK   : Top Value 저장 & Offset(btm - top) 저장->for Display & Force Check OK
			//FAIL : m_nForceRatio Value 수정 <Btm value 대비 Ratio 수정> & Need Force Check true
			
			if (polibath)
			{
				dCompVale = m_nTopForceValue;

				if (Math.Abs(dCompVale - dSetForce) < nInValue) bOk = true;

				if (!bOk)
				{
					if(dCompVale > dSetForce) m_dForceRatio = m_dForceRatio - 0.2; // 계산된 특정 값만큼 +/-
					else                      m_dForceRatio = m_dForceRatio + 0.2; 
				}
			}
			else
			{
				dCompVale = LDCBTM._dLoadCellValue; //m_nBtmForceValue; //Bottom Load cell Value
				double dGap = Math.Abs(dCompVale - dSetForce); 
				if (dGap < nInValue) bOk = true;
				if (bOk)
                {
                    m_nTopForceValue = 0; //현재 Top Load Cell Value 
                    m_nForceOffset   = m_nBtmForceValue - m_nTopForceValue;

					//
					if(isCleanForce)
					{
						sTemp = string.Format($"CLEN STEP : {m_nCleanCnt}");
					}
					else
					{
						sTemp = string.Format($"POLISHING STEP : {m_nPoliCnt}");
					}
					

					fn_WriteLog(string.Format($"Force : {dSetForce:F2} / {sTemp} [TOP:{IO.fn_GetTopLoadCell(true):F3}N]"), EN_LOG_TYPE.ltLot);

                }
                else
                {
					dOffset = 0.1;

					//if (dCompVale > dSetForce) m_dForceRatio = m_dForceRatio - 0.2; // 계산된 특정 값만큼 +/-
					//else                       m_dForceRatio = m_dForceRatio + 0.2; 
					if      (dGap > 70) dOffset = 0.8;
					else if (dGap > 60) dOffset = 0.6 ;
					else if (dGap > 50) dOffset = 0.4 ;
                    else if (dGap > 40) dOffset = 0.3 ;
					else if (dGap > 30) dOffset = 0.15;
					else if (dGap > 20) dOffset = 0.1 ;
					else if (dGap > 10) dOffset = 0.05;

					if (dCompVale > dSetForce)
					{
						m_dForceRatio = m_dForceRatio - dOffset; // 계산된 특정 값만큼 +/-
					}
					else
					{
						m_dForceRatio = m_dForceRatio + dOffset;
					}

                    //Init
                    m_nTopForceValue = -1;
                    m_nForceOffset   = 0;
                }
            }

			return bOk; 
		}
		//---------------------------------------------------------------------------
		public bool fn_PolishingOneCycle()
        {
            return false;
        }
		//---------------------------------------------------------------------------
        public bool fn_CleaningOneCycle()
        {
            return false;
        }
		//---------------------------------------------------------------------------
        public bool fn_ToolPlaceOneCycle()
        {
			bool r1, r2; 
            
			//Place Cycle
            if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:

					m_iCmdX = EN_COMD_ID.User4; //가장 없는 줄에.... CalPos
					m_iCmdZ = EN_COMD_ID.User4; //Z Position
					m_iCmdY = EN_COMD_ID.User3; //Storage Y-Axis

					fn_MoveCylClamp(ccFwd);

					m_nManStep++;
					return false;

				case 11:

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					m_nManStep++;
					return false;

				case 12:
					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					r2 = SEQ_STORG.fn_ReqMoveMotr(EN_MOTR_ID.miSTR_Y, m_iCmdY);
					if (!r1) return false;

					m_nManStep++;
					return false;

				case 13:
					r1 = MOTR.CmprPosByCmd(EN_MOTR_ID.miSTR_Y, m_iCmdY);
					if (!r1) return false;

					m_nManStep++;
					return false;

				case 14:
					r1 = fn_MoveMotr(m_iMotrZId, m_iCmdZ);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 15:
					r1 = fn_MoveToolClamp(ccBwd);
					if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 200)) return false; //
					m_tDelayTime.Clear();

					m_nManStep++;
					return false;

				case 16:

					r1 = fn_MoveToolClamp(ccFwd);
					if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 17:
					r1 = fn_MoveToolClamp(ccBwd);
					if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay
					m_tDelayTime.Clear();

					m_nManStep++;
					return false;

				case 18:
					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);
					if (!r1) return false;

					if (!fn_IsExistTool())
					{
						fn_UserMsg("Place Tool One Cycle OK");
					}
					else
					{
						//Error
						fn_UserMsg("Place Tool One Cycle - FAIL!!!");
					}

					fn_MoveToolClamp(ccFwd);

					m_tDelayTime.Clear();
					m_nManStep = 0;
					return true;
			}
        }
		//-------------------------------------------------------------------------------------------------
		public bool fn_VisnTestOneCycle(bool prealign = false)
        {
			bool r1, r2;
			
			//
			m_iCmdX  = prealign ? EN_COMD_ID.User11 : EN_COMD_ID.User12;  
			m_iCmdY  = EN_COMD_ID.User2;
			
			
			if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;


				case 10: //Wait

					r1 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;
                    m_nManStep ++;
                    return false;

                case 11: //MoveVisionPosition

					r1 = fn_MoveMotr(EN_MOTR_ID.miSPD_X, m_iCmdX);
					r2 = prealign ? SEQ_TRANS.fn_MoveCylLoadUpDown(ccFwd) : SEQ_POLIS.fn_ReqBathImagePos();
					if (!r1 || !r2) return false;
                    m_nManStep++;
                    return false;

				case 12:

                    r1 = fn_MoveCylLensCvr(ccOpen);
                    r2 = fn_MoveCylIR     (ccOpen);
                    if (!r1 || !r2 ) return false;

					//JUNG/201026/동작만 확인하는걸로....
					m_nManStep = 20; //m_nManStep++; 
					return false;

				case 13:
                    if (!g_VisionManager.fn_PolishingAlign(ref stOneCycleResult))
                    {
						Console.WriteLine("FAIL - g_VisionManager.fn_PolishingAlign(ref stOneCycleResult))");
                        m_nManStep =0;
                        return true;

                    }
					Console.WriteLine($"Theta Align Result : {stOneCycleResult.dTheta}");
                    m_nManStep++;
                    return false;

				// Move Theta
				case 14:

					//MOTR[(int)EN_MOTR_ID.miPOL_TH].MP.dPosn[(int)EN_POSN_ID.CalPos] = stOneCycleResult.dTheta ; 
					r1 = MOTR.MoveMotrR(EN_MOTR_ID.miPOL_TH, stOneCycleResult.dTheta * -1, 100);
                    if (!r1) return false;
                    m_nManStep++;
                    return false;

 
				case 15:
					r1 = MOTR[(int)EN_MOTR_ID.miPOL_TH].GetStop();
                    if (!r1) return false;
                    m_nManStep++;
                    return false;
				
				case 16:
					// X,Y Align Start
					if (!g_VisionManager.fn_PolishingAlign(ref stOneCycleResult))
                    {
						Console.WriteLine("X,Y Align Fail");
                        m_nManStep = 0;
                        return true;

                    }
                    // 좌표계 변환. => Cam 기준 상대좌표. (3사분면 좌표계)
                    stOneCycleResult.pntModel.X *= g_VisionManager._RecipeVision.ResolutionX / 1000.0;
                    stOneCycleResult.pntModel.Y *= g_VisionManager._RecipeVision.ResolutionY / 1000.0;

					Console.WriteLine($"[ X,Y Align Result ] X : {stOneCycleResult.pntModel.X:F3} mm , Y : {stOneCycleResult.pntModel.Y:F3} mm");

                    m_nManStep++;
                    return false;
				
				case 17:

					r1 = MOTR.MoveMotrR(EN_MOTR_ID.miPOL_Y, (stOneCycleResult.pntModel.Y * -1), 100);
					r2 = MOTR.MoveMotrR(EN_MOTR_ID.miSPD_X, (stOneCycleResult.pntModel.X * -1), 100);
					if (!r1 || !r2) return false;

                    m_nManStep =0;
                    return true;

				case 20: 
					r1 = fn_MoveCylLensCvr(ccClose);
                    r2 = fn_MoveCylIR     (ccClose);
                    if (!r1 || !r2 ) return false;

                    m_nManStep = 0;
                    return true;
            }
        }

        //---------------------------------------------------------------------------
        public bool fn_UtilCheckOneCycle()
        {
            //Check Utility of Polishing Bath
			bool   r1, r2;
			//double dUtilValue = 0.0;
			//double dOffset    = 0.0; 
			
			//
			m_iCmdX = EN_COMD_ID.User9;

            //Utility Exist Check Cycle
            if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:

					m_nManStep++;
					return false;

				case 11: 
					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					m_nManStep++;
                    return false;

                case 12:
					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					if (!r1) return false;

					m_nManStep++;
                    return false;

                case 13:

					//초음파센서 확인
					if (IO.fn_IsUTLevelDone())
					{
						fn_UserMsg("Utility Exist");
					}
					else
					{
						fn_UserMsg("Utility None!!!");
					}

                    m_tDelayTime.Clear();
					m_nManStep = 0;
                    return true;
            }

        }
		//---------------------------------------------------------------------------
        public bool fn_PlatePickOneCycle(EN_MAGA_ID whre)
        {
			//
			bool r1, r2, r3; 
			bool isLoadPort   = whre == EN_MAGA_ID.LOAD  ;
			bool isPolishBath = whre == EN_MAGA_ID.POLISH;
			bool isCleanBath  = whre == EN_MAGA_ID.CLEAN ;

			//Place one Cycle
			if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:

					if      (isPolishBath) { m_iCmdX = EN_COMD_ID.User6; m_iCmdZ1 = EN_COMD_ID.User3; }
					else if (isCleanBath ) { m_iCmdX = EN_COMD_ID.User7; m_iCmdZ1 = EN_COMD_ID.User4; }
					else if (isLoadPort  ) { m_iCmdX = EN_COMD_ID.User8; m_iCmdZ1 = EN_COMD_ID.User1; }
					else
					{
						m_nManStep = 0;
						return true;
					}

					m_nManStep++;
					return false;

				case 11:

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					//Check Plate Exist
					if (fn_IsExistPlate())
					{
						fn_UserMsg("[ERROR] Check Plate at spindle");
						m_nManStep = 0;
						return true;
					}

					fn_MoveCylClamp(ccBwd);

					m_nManStep++;
					return false;

				case 12:

					if (isPolishBath)
					{
						SEQ_POLIS.fn_ReqBathWaitPos();
						SEQ_POLIS.fn_MoveCylClamp(ccBwd);
						r3 = true;
					}
					else if (isCleanBath)
					{
						SEQ_CLEAN.fn_ReqBathWaitPos();
						SEQ_CLEAN.fn_MoveCylClamp(ccBwd);
						r3 = true; 
					}
					else
					{
						r3 = SEQ_TRANS.fn_MoveCylTRTurnPos();
					}
					
					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					r2 = fn_MoveCylClamp(ccBwd);
					if (!r1 || !r2 || !r3) return false;

					m_nManStep++;
					return false;

				case 13:

					if (isPolishBath)
					{
						r1 = SEQ_POLIS.fn_MoveCylClamp(ccBwd);
						r2 = SEQ_POLIS.fn_ReqBathWaitPos();
						r3 = true; 
					}
					else if (isCleanBath)
					{
						r1 = SEQ_CLEAN.fn_MoveCylClamp(ccBwd);
						r2 = SEQ_CLEAN.fn_ReqBathWaitPos();
						r3 = true; 
					}
					else
					{
                        r1 = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_T, EN_COMD_ID.User1);//fn_ReqMoveAlignPos(); //T-Axis Position
                        r2 = SEQ_TRANS.fn_MoveCylLoadUpDown(ccUp);
                        r3 = SEQ_TRANS.fn_MoveCylLoadCover(ccBwd);
					}
					if (!r1 || !r2 || !r3) return false;

					m_nManStep++;
					return false;

				case 14:
					r1 = fn_MoveMotr(m_iMotrZ1Id, m_iCmdZ1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveCylClamp(ccFwd);
					if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 16:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1) return false;

					if(fn_IsExistPlate(true))
					{
						fn_UserMsg("Pick Plate One Cycle -- OK");
					}
					else
					{
						fn_UserMsg("Pick Plate One Cycle --> FAIL!!!!!");
					}

					m_tDelayTime.Clear();
					m_nManStep = 0;
					return true;
			}

        }
		//---------------------------------------------------------------------------
		public bool fn_PlatePlaceOneCycle(EN_MAGA_ID whre)
		{
			//
			bool r1, r2, r3; 
			bool isLoadPort   = whre == EN_MAGA_ID.LOAD;
			bool isPolishBath = whre == EN_MAGA_ID.POLISH;
			bool isCleanBath  = whre == EN_MAGA_ID.CLEAN ;

			//Place one Cycle
			if (m_nManStep < 0) m_nManStep = 0;
			
			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:

					if      (isPolishBath) { m_iCmdX = EN_COMD_ID.User6; m_iCmdZ1 = EN_COMD_ID.User13; } //JUNG/200602
					else if (isCleanBath ) { m_iCmdX = EN_COMD_ID.User7; m_iCmdZ1 = EN_COMD_ID.User14; }
					else if (isLoadPort  ) { m_iCmdX = EN_COMD_ID.User8; m_iCmdZ1 = EN_COMD_ID.User11; }
					else
					{
						m_nManStep = 0;
						return true;
					}

					fn_MoveCylClamp(ccFwd);

					m_nManStep++;
					return false;

				case 11:

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1 );
					r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					if(!fn_IsExistPlate(true))
					{
						fn_UserMsg("[ERROR] Check Plate at spindle");
						m_nManStep = 0;
                        return true;
                    }

                    m_nManStep++;
					return false;

				case 12:

					if (isPolishBath)
					{
						SEQ_POLIS.fn_MoveCylClamp(ccBwd);
						SEQ_POLIS.fn_ReqBathWaitPos();
						r2 = true;
					}
					else if (isCleanBath)
					{
						SEQ_CLEAN.fn_MoveCylClamp(ccBwd);
						SEQ_CLEAN.fn_ReqBathWaitPos();
						r2 = true; 
					}
					else
					{
                        SEQ_TRANS.fn_MoveCylLoadCover(ccBwd);
                        r2 = SEQ_TRANS.fn_MoveCylTRTurnPos();
					}

					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX);
					if (!r1 || !r2) return false;

					m_nManStep++;
					return false;

				case 13:

					if (isPolishBath)
					{
						r1 = SEQ_POLIS.fn_MoveCylClamp(ccBwd);
						r2 = SEQ_POLIS.fn_ReqBathWaitPos();
						r3 = true; 
					}
					else if (isCleanBath)
					{
						r1 = SEQ_CLEAN.fn_MoveCylClamp(ccBwd);
						r2 = SEQ_CLEAN.fn_ReqBathWaitPos();
						r3 = true; 
					}
					else
					{
						r1 = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_T, EN_COMD_ID.User1);
                        r2 = SEQ_TRANS.fn_MoveCylLoadUpDown(ccUp);
                        r3 = SEQ_TRANS.fn_MoveCylLoadCover(ccBwd);
                    }
					if (!r1 || !r2 || !r3) return false;

					m_nManStep++;
					return false;

				case 14:
					r1 = fn_MoveMotr(m_iMotrZ1Id, m_iCmdZ1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveCylClamp(ccBwd);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 16:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1) return false;


                    if (!fn_IsExistPlate(false))
                    {
                        fn_UserMsg("Place Plate One Cycle -- OK");
                    }
                    else
                    {
                        fn_UserMsg("Place Plate One Cycle --> FAIL!!!!!");
                    }

                    m_tDelayTime.Clear();
					m_nManStep = 0;
					return true;
			}

		}
		//---------------------------------------------------------------------------
		public bool fn_CupPPOneCycle(bool pick, bool str)
		{
			//
			bool r1, r2; 

			//
			if (m_nManStep < 0) m_nManStep = 0;
			
			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:

					m_iCmdX  = str ? EN_COMD_ID.User15 : EN_COMD_ID.User16;  
					m_iCmdZ1 = str ? EN_COMD_ID.User5  : (pick? EN_COMD_ID.User6 : EN_COMD_ID.User7); //JUNG/200915
					m_iCmdY  = str ? EN_COMD_ID.User4  : EN_COMD_ID.User3 ;

					fn_MoveCylClamp(pick? ccBwd : ccFwd);

					m_nManStep++;
					return false;

				case 11:

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1 );
					r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					if (pick)
					{
						if (!fn_IsExistCup(true) && str)
						{
							fn_UserMsg("[ERROR] Check Cup Exist Sensor at storage");
							m_nManStep = 0;
							return true;
						}
					}
					else
					{
                        if (fn_IsExistCup(false) && str)
                        {
                            fn_UserMsg("[ERROR] Check Cup at storage");
                            m_nManStep = 0;
                            return true;
                        }

                    }

                    m_nManStep++;
					return false;

				case 12:
					if (!fn_MoveMotr(m_iMotrXId, m_iCmdX)) return false; //JUNG/200807/cup 충돌로 인해...

					r1 = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, m_iCmdY); //fn_ReqCupStorInOutPos();
					r2 = str ? true : SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TH, EN_COMD_ID.User1);
					if (!r1 || !r2) return false;

					m_nManStep++;
					return false;

				case 13:

					r1 = fn_MoveMotr(m_iMotrZ1Id, m_iCmdZ1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 14:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveCylClamp(pick? ccFwd : ccBwd);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false;

				case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //300ms Delay

					r1 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					if (!r1) return false;

					if (pick)
					{
                        //
                        if (fn_IsExistCup(true))
                        {
                            fn_UserMsg("pick cup One Cycle -- OK");
                        }
                        else
                        {
                            fn_UserMsg("pick cup One Cycle --> FAIL!!!!!");
                        }

                    }
                    else
					{
						//
						if (!fn_IsExistCup(false))
						{
							fn_UserMsg("Place cup One Cycle -- OK");
						}
						else
						{
							fn_UserMsg("Place cup One Cycle --> FAIL!!!!!");
						}
					}

                    m_tDelayTime.Clear();
					m_nManStep = 0;
					return true;
			}

		}
		
        //---------------------------------------------------------------------------
        public bool fn_SetSpindleRun(int act, bool cw = false)
		{
			//
			bool bRun     = IO.XV[(int)EN_INPUT_ID.xSPD_E3000_RUN    ];
			bool bSpeedOk = IO.XV[(int)EN_INPUT_ID.xSPD_E3000_SpeedOK];
			bool bRunOk   = bRun && bSpeedOk ;

			if (act > 0)
			{
				if (act > 1) fn_SetSpindleSpeed(act);

				IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_DirIn] = cw;
				IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_Run  ] = true;
				return bRunOk; 
			}
            else
            {
                IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_Run] = false;
                return !bRunOk;
            }

		}
		//---------------------------------------------------------------------------
		public void fn_SetSpindleSpeed(int speed)
		{
			//Sets rotating speed of the motor. 
			//Speed(min - 1(rpm)) = Motor Speed Control Voltage(VR) x 10000 / 1.5 (0V =< VR =< 10V)
			//VR = Speed * 1.5 / 10000; 
			
			//rpm : 1,000 ~ 50,000 => 0.15 ~ 7.5V

			double dOffset   = FM.m_stMasterOpt.dSpdOffset; //JUNG/200914/Spindle Offset

			//double dVolt     = speed * 1.5 / 10000.0;
			double dVolt     = (speed + dOffset) * 1.5 / 10000.0;
            int    nMaxVolt  = 10   ;
			int    nMaxValue = MAX_VALUE_10V;
            int    nSetValue = (int)(dVolt / (double)nMaxVolt * nMaxValue);

			if (nSetValue < 0 || nSetValue > nMaxValue) nSetValue = 0; 
			if (speed < 1000                          ) nSetValue = 0;

			//
			IO.AO[(int)EN_AOUTPUT_ID.aoSDL_E3000_Speed] = nSetValue; 
		}
		//---------------------------------------------------------------------------
		public void fn_SetSpindleReset()
		{
			IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_Reset] = true; //Auto Off
		}
		//---------------------------------------------------------------------------
		public int fn_GetSpindleSpeed()
		{
			//Output the rotation speed of rotating motor with Analog Monitor Voltage. 
			//10,000min-1 (rpm) / V
			double dOffset    = FM.m_stMasterOpt.dSpdOffset;
			int    nValue     = IO.AI[(int)EN_AINPUT_ID.aiSPD_E3000_Speed];
			int    nMaxValue  = MAX_VALUE_10V;
			int    nMaxVolt   = 10   ;

			if (nValue < 1) return 0;

			//0~65535 -> 0~10V
			double dVolt  = (nValue / (double)nMaxValue) * nMaxVolt;
			//double dSpeed = 10000 * dVolt;
			double dSpeed = (10000 * dVolt) + (dOffset * -2); //JUNG/200914

			double digitCal = Math.Round(dSpeed, 3); //Math.Pow(10, dSpeed) / 10;

			return (int)dSpeed;  //Math.Round(dSpeed,1); 
		}
		//---------------------------------------------------------------------------
		public double fn_GetSpindleTorque()
		{
            //Shows that the torque being applied to the analog motor.
            //회전 중의 작업량의 부하율을 전압으로 나타내는 아날로그 모니터입니다.
            //부하율 100 % (DC + 5V)까지가 연속사용 영역입니다.
            //부하율(%) = 부하율 모니터 전압×20
            //부하율: 0 - 200 % (DC0V - DC + 10V)

            int nValue    = IO.AI[(int)EN_AINPUT_ID.aiSPD_E3000_LOAD];
            int nMaxValue = MAX_VALUE_10V;
            int nMaxVolt  = 10;

            if (nValue < 1) return 0.0;

            //0~32767 -> 0~10V
            double dVolt   = nValue / (double)nMaxValue * nMaxVolt;
			double dTorque = dVolt * 20;
			
			return Math.Round(dTorque,1);

		}
        //---------------------------------------------------------------------------
        public bool fn_GetSpindleDir()
		{
            //Controls the rotational direction of the motor spindle.
			//Setting parameter, can start with reverse	rotation.
            //OFF(Open) : FWD.
            //ON(Closed) : REV.

            if (!IO.XV[(int)EN_INPUT_ID.xSPD_E3000_Direction]) return true;

			return false; 
		}
        //---------------------------------------------------------------------------
        public bool fn_GetSpindleSpeedOK()
        {
            //Shows that the Motor has achieved more than 90 % of the set speed.
            if (!IO.XV[(int)EN_INPUT_ID.xSPD_E3000_SpeedOK]) return true;

            return false;
        }
		//---------------------------------------------------------------------------
		public bool fn_GetSpindleError()
		{
            //shows that error has occurred. 
			//Error code will be displayed on Digital Speed Indicator When setting parameter, Error Output Mode can be changed.
            if (!IO.XV[(int)EN_INPUT_ID.xSPD_E3000_State]) return true;

			return false;
		}
		//---------------------------------------------------------------------------
		public double fn_GetLoadCellTop()
		{
			return 0.0;
		}

		//---------------------------------------------------------------------------
		public bool fn_MoveCylforCam(int act)
		{
			return fn_MoveCylLensCvr(act) && fn_MoveCylIR(act);
		}
		//-------------------------------------------------------------------------------------------------
		public bool fn_MoveCylLensCvr(int act)
        {
			bool   isNoRun   = SEQ.fn_IsNoRun() && m_nManStep == 0;
			double dEnc_SPDZ = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z);

            //
            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aSpdl_LensCovr, act);
            return r1;
        }
		//---------------------------------------------------------------------------
        public bool fn_MoveCylIR(int act)
        {
            bool   isNoRun   = SEQ.fn_IsNoRun() && m_nManStep == 0;
            double dEnc_SPDZ = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z);

            //
            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aspdl_IR, act);
            return r1;
        }

        public bool fn_MoveCylIR(int act, int mode = -1)
        {
            bool isNoRun = SEQ.fn_IsNoRun() && m_nManStep == 0;
            double dEnc_SPDZ = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z);

            //recipe Data 
            if (mode > -1)
            {
                act = g_VisionManager.CurrentRecipe.LightIRFilter[mode] == 1 ? ccClose : ccOpen;
            }
            //
            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aspdl_IR, act);
            return r1;
        }
        //-------------------------------------------------------------------------------------------------
        public int fn_GetLensCvrRetryCnt()
		{
			return ACTR.fn_GetRetryCnt((int)EN_ACTR_LIST.aSpdl_LensCovr);
		}
		public bool fn_GetLensCvrRetryFwd()
		{
			return ACTR.fn_IsRetryStateFwd((int)EN_ACTR_LIST.aSpdl_LensCovr);
		}
        public bool fn_GetLensCvrRetryBwd()
        {
            return ACTR.fn_IsRetryStateBwd((int)EN_ACTR_LIST.aSpdl_LensCovr);
        }
		//-------------------------------------------------------------------------------------------------
        public bool fn_GetIRRetryFwd()
        {
            return ACTR.fn_IsRetryStateFwd((int)EN_ACTR_LIST.aspdl_IR);
        }
        public bool fn_GetIRRetryBwd()
        {
            return ACTR.fn_IsRetryStateBwd((int)EN_ACTR_LIST.aspdl_IR);
        }
        //---------------------------------------------------------------------------
        public bool fn_MoveCylClamp(int act)
		{
			//Plate Clamp
			bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aSpdl_PlateClamp, act);

			return r1; 
		}
		//---------------------------------------------------------------------------
        public bool fn_MoveToolClamp(int act)
        {
			//Check Un-clamp Position
			if(SEQ._bRun) //Auto
			{
			
			}

			//Tool Clamp
			IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_ToolClamp] = (act == 0)? true : false ;

    		return true; 
        }
		//---------------------------------------------------------------------------
		private void fn_WriteSeqLog(string log)
		{
            string stemp = string.Format($"[{EN_SEQ_ID.SPINDLE.ToString()}] ");
            fn_WriteLog(stemp + log, EN_LOG_TYPE.ltEvent, EN_SEQ_ID.SPINDLE);
		}
		//---------------------------------------------------------------------------
		public void fn_SaveLog(ref string Msg)
        {
			string sTemp = string.Empty;

			Msg = string.Empty;

			//
			Msg += "[SeqSpindle]\r\n"; 
			Msg += sTemp = string.Format($"m_bToStart        = {m_bToStart       }\r\n");
            Msg += sTemp = string.Format($"m_bToStop         = {m_bToStop        }\r\n");
            Msg += sTemp = string.Format($"m_bWorkEnd        = {m_bWorkEnd       }\r\n");
            Msg += sTemp = string.Format($"m_bDrngToolPick   = {m_bDrngToolPick  }\r\n");
            Msg += sTemp = string.Format($"m_bDrngToolCheck  = {m_bDrngToolCheck }\r\n");
            Msg += sTemp = string.Format($"m_bDrngForceChk   = {m_bDrngForceChk  }\r\n");
            Msg += sTemp = string.Format($"m_bDrngPolishing  = {m_bDrngPolishing }\r\n");
            Msg += sTemp = string.Format($"m_bDrngCleaning   = {m_bDrngCleaning  }\r\n");
            Msg += sTemp = string.Format($"m_bDrngToolPlce   = {m_bDrngToolPlce  }\r\n");
            Msg += sTemp = string.Format($"m_bDrngVisnInspL  = {m_bDrngVisnInspL }\r\n");
            Msg += sTemp = string.Format($"m_bDrngVisnInspA  = {m_bDrngVisnInspA }\r\n");
            Msg += sTemp = string.Format($"m_bDrngUtilLevel  = {m_bDrngUtilLevel }\r\n");
            Msg += sTemp = string.Format($"m_bDrngPlatePick  = {m_bDrngPlatePick }\r\n");
            Msg += sTemp = string.Format($"m_bDrngPlatePlce  = {m_bDrngPlatePlce }\r\n");
            Msg += sTemp = string.Format($"m_bDrngPlatePlceP = {m_bDrngPlatePlceP}\r\n");
            Msg += sTemp = string.Format($"m_bDrngPlatePlceC = {m_bDrngPlatePlceC}\r\n");
            Msg += sTemp = string.Format($"m_bDrngWait       = {m_bDrngWait      }\r\n");
			Msg += sTemp = string.Format($"m_bDrngCupInPos   = {m_bDrngCupInPos  }\r\n");
			
			Msg += sTemp = string.Format($"m_nSeqStep        = {m_nSeqStep       }\r\n");
            Msg += sTemp = string.Format($"m_nManStep        = {m_nManStep       }\r\n");
            Msg += sTemp = string.Format($"m_nHomeStep       = {m_nHomeStep      }\r\n");
            Msg += sTemp = string.Format($"m_nToolPickStep   = {m_nToolPickStep  }\r\n");
            Msg += sTemp = string.Format($"m_nToolChkStep    = {m_nToolChkStep   }\r\n");
            Msg += sTemp = string.Format($"m_nForceChkStep   = {m_nForceChkStep  }\r\n");
            Msg += sTemp = string.Format($"m_nPolishStep     = {m_nPolishStep    }\r\n");
            Msg += sTemp = string.Format($"m_nPoliBackStep   = {m_nPoliBackStep  }\r\n");
            Msg += sTemp = string.Format($"m_nCleanStep      = {m_nCleanStep     }\r\n");
            Msg += sTemp = string.Format($"m_nToolPlaceStep  = {m_nToolPlaceStep }\r\n");
            Msg += sTemp = string.Format($"m_nVisnInspStep   = {m_nVisnInspStep  }\r\n");
            Msg += sTemp = string.Format($"m_nUtilChkStep    = {m_nUtilChkStep   }\r\n");
            Msg += sTemp = string.Format($"m_nMagzPickStep   = {m_nMagzPickStep  }\r\n");
            Msg += sTemp = string.Format($"m_nMagzPlaceStep  = {m_nMagzPlaceStep }\r\n");
			Msg += sTemp = string.Format($"m_nCupMoveStep    = {m_nCupMoveStep   }\r\n");
			


		}
        //---------------------------------------------------------------------------
        public void fn_UpdateFlag(ref Grid grd)
        {
			if (grd == null) return;
            
			Label[,] Items = new Label[50, 2];
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
            int nRow = 0; 

            Items[nRow, 0].Content = "bToStart       "; Items[nRow++, 1].Content = string.Format($"{m_bToStart       }");
			Items[nRow, 0].Content = "bToStop        "; Items[nRow++, 1].Content = string.Format($"{m_bToStop        }");
			Items[nRow, 0].Content = "bWorkEnd       "; Items[nRow++, 1].Content = string.Format($"{m_bWorkEnd       }");
			Items[nRow, 0].Content = "_";				Items[nRow++, 1].Content = string.Format($"");			
			Items[nRow, 0].Content = "bDrngToolPick  "; Items[nRow++, 1].Content = string.Format($"{m_bDrngToolPick  }");
			Items[nRow, 0].Content = "bDrngToolCheck "; Items[nRow++, 1].Content = string.Format($"{m_bDrngToolCheck }");
			Items[nRow, 0].Content = "bDrngForceChk  "; Items[nRow++, 1].Content = string.Format($"{m_bDrngForceChk  }");
			Items[nRow, 0].Content = "bDrngPolishing "; Items[nRow++, 1].Content = string.Format($"{m_bDrngPolishing }");
			Items[nRow, 0].Content = "bDrngCleaning  "; Items[nRow++, 1].Content = string.Format($"{m_bDrngCleaning  }");
			Items[nRow, 0].Content = "bDrngToolPlce  "; Items[nRow++, 1].Content = string.Format($"{m_bDrngToolPlce  }");
			Items[nRow, 0].Content = "bDrngVisnInspL "; Items[nRow++, 1].Content = string.Format($"{m_bDrngVisnInspL }");
			Items[nRow, 0].Content = "bDrngVisnInspA "; Items[nRow++, 1].Content = string.Format($"{m_bDrngVisnInspA }");
			Items[nRow, 0].Content = "bDrngUtilLevel "; Items[nRow++, 1].Content = string.Format($"{m_bDrngUtilLevel }");
			Items[nRow, 0].Content = "bDrngPlatePick "; Items[nRow++, 1].Content = string.Format($"{m_bDrngPlatePick }");
			Items[nRow, 0].Content = "bDrngPlatePlce "; Items[nRow++, 1].Content = string.Format($"{m_bDrngPlatePlce }");
			Items[nRow, 0].Content = "bDrngPlatePlceP"; Items[nRow++, 1].Content = string.Format($"{m_bDrngPlatePlceP}");
			Items[nRow, 0].Content = "bDrngPlatePlceC"; Items[nRow++, 1].Content = string.Format($"{m_bDrngPlatePlceC}");
			Items[nRow, 0].Content = "bDrngWait      "; Items[nRow++, 1].Content = string.Format($"{m_bDrngWait      }");
			Items[nRow, 0].Content = "bDrngCupInPos"  ; Items[nRow++, 1].Content = string.Format($"{m_bDrngCupInPos  }");
			
			Items[nRow, 0].Content = "";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nSeqStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nSeqStep       }");
			Items[nRow, 0].Content = "nManStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nManStep       }");
			Items[nRow, 0].Content = "nHomeStep"      ; Items[nRow++, 1].Content = string.Format($"{m_nHomeStep      }");
			Items[nRow, 0].Content = "_";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nToolPickStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nToolPickStep  }");
			Items[nRow, 0].Content = "nToolChkStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nToolChkStep   }");
			Items[nRow, 0].Content = "nForceChkStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nForceChkStep  }");
			Items[nRow, 0].Content = "nPolishStep"    ; Items[nRow++, 1].Content = string.Format($"{m_nPolishStep    }");
			Items[nRow, 0].Content = "nPolishBackStep"; Items[nRow++, 1].Content = string.Format($"{m_nPoliBackStep  }");
			Items[nRow, 0].Content = "nCleanStep"     ; Items[nRow++, 1].Content = string.Format($"{m_nCleanStep     }");
			Items[nRow, 0].Content = "nToolPlaceStep" ; Items[nRow++, 1].Content = string.Format($"{m_nToolPlaceStep }");
			Items[nRow, 0].Content = "nVisnInspStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nVisnInspStep  }");
			Items[nRow, 0].Content = "nUtilChkStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nUtilChkStep   }");
			Items[nRow, 0].Content = "nMagzPickStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nMagzPickStep  }");
			Items[nRow, 0].Content = "nMagzPlaceStep" ; Items[nRow++, 1].Content = string.Format($"{m_nMagzPlaceStep }");
			Items[nRow, 0].Content = "nCupMoveStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nCupMoveStep   }");
			Items[nRow, 0].Content = "";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "Seq Msg"        ; Items[nRow++, 1].Content = string.Format($"{m_sSeqMsg        }");

			//
			grd.Children.Clear();
            grd.Background = Brushes.White;
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

			for (int r = 0; r < nRow; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r,c]);
					Grid.SetRow   (Items[r, c], r);
					Grid.SetColumn(Items[r, c], c);
				}
            }


            //             Border boder = new Border();
            //             boder.BorderBrush = System.Windows.Media.Brushes.Black;
            //             boder.BorderThickness = new System.Windows.Thickness(1);
            //             boder.Margin = new Thickness(-2);
            //             Grid.SetRowSpan(boder, 100);
            //             Grid.SetColumnSpan(boder, 100);
            //             grd.Children.Add(boder);
        }

        //---------------------------------------------------------------------------
        public void fn_UpdateOneShot(ref Grid grd)
        {
			if (grd == null) return;

			Label[,] Items = new Label[20,2];
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
            int nRow = 0; 

			Items[nRow, 0].Content = "iSeqStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nSeqStep       }");
			Items[nRow, 0].Content = "iManStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nManStep       }");
			Items[nRow, 0].Content = "iHomeStep"      ; Items[nRow++, 1].Content = string.Format($"{m_nHomeStep      }");
			Items[nRow, 0].Content = "_";               Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nToolPickStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nToolPickStep  }");
			Items[nRow, 0].Content = "nToolChkStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nToolChkStep   }");
			Items[nRow, 0].Content = "nForceChkStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nForceChkStep  }");
			Items[nRow, 0].Content = "nPolishStep"    ; Items[nRow++, 1].Content = string.Format($"{m_nPolishStep    }");
			Items[nRow, 0].Content = "nPolishBackStep"; Items[nRow++, 1].Content = string.Format($"{m_nPoliBackStep  }");
			Items[nRow, 0].Content = "nCleanStep"     ; Items[nRow++, 1].Content = string.Format($"{m_nCleanStep     }");
			Items[nRow, 0].Content = "nToolPlaceStep" ; Items[nRow++, 1].Content = string.Format($"{m_nToolPlaceStep }");
			Items[nRow, 0].Content = "nVisnInspStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nVisnInspStep  }");
			Items[nRow, 0].Content = "nUtilChkStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nUtilChkStep   }");
			Items[nRow, 0].Content = "nMagzPickStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nMagzPickStep  }");
			Items[nRow, 0].Content = "nMagzPlaceStep" ; Items[nRow++, 1].Content = string.Format($"{m_nMagzPlaceStep }");
			Items[nRow, 0].Content = "nCupMoveStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nCupMoveStep   }");
			Items[nRow, 0].Content = "";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "Seq Msg"        ; Items[nRow++, 1].Content = string.Format($"{m_sSeqMsg        }");
			

			grd.Children.Clear();
            grd.Background = Brushes.White;
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

			for (int r = 0; r < nRow; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					grd.Children.Add(Items[r,c]);
					Grid.SetRow   (Items[r, c], r);
					Grid.SetColumn(Items[r, c], c);
				}
            }

            //             Border boder = new Border();
            //             boder.BorderBrush = System.Windows.Media.Brushes.Black;
            //             boder.BorderThickness = new System.Windows.Thickness(1);
            //             boder.Margin = new Thickness(-2);
            //             Grid.SetRowSpan(boder, 100);
            //             Grid.SetColumnSpan(boder, 100);
            //             grd.Children.Add(boder);
        }
		//---------------------------------------------------------------------------
		public void fn_Load(bool bLoad, FileStream fp)
        {
            if (bLoad)
            {
                BinaryReader br = new BinaryReader(fp);
                
				m_nTopForceValue = br.ReadInt32  ();
				m_nForceOffset	 = br.ReadInt32  ();
				m_dForceRatio    = br.ReadDouble ();

				m_nSpare1		 = br.ReadInt32  ();
                m_nSpare2		 = br.ReadInt32  ();
                m_nSpare3		 = br.ReadInt32  ();
                m_nSpare4		 = br.ReadInt32  ();
                m_nSpare5		 = br.ReadInt32  ();
								 
                //				 
                m_bSpare1		 = br.ReadBoolean();
                m_bSpare2		 = br.ReadBoolean();
                m_bSpare3		 = br.ReadBoolean();
                m_bSpare4		 = br.ReadBoolean();
                m_bSpare5		 = br.ReadBoolean();

                //				 
                m_dDisX_PCtoPS   = br.ReadDouble ();
				m_dDisY_PCtoPS   = br.ReadDouble ();

                m_dSpare1		 = br.ReadDouble ();
                m_dSpare2		 = br.ReadDouble ();
                m_dSpare3		 = br.ReadDouble ();
                m_dSpare4		 = br.ReadDouble ();
                m_dSpare5		 = br.ReadDouble ();

				enMillINFO.nTotalCycle = br.ReadInt32();

				for (int n = 0; n < 10; n++)
				{
					enMillINFO.nPath     [n] = br.ReadInt32 ();
					enMillINFO.nRPM      [n] = br.ReadInt32 ();
					enMillINFO.dForce    [n] = br.ReadDouble();
					enMillINFO.dXSpeed   [n] = br.ReadDouble();
					enMillINFO.dXDistance[n] = br.ReadDouble();
					enMillINFO.dYDistance[n] = br.ReadDouble();
				}

     		}
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);

				bw.Write(m_nTopForceValue);
				bw.Write(m_nForceOffset	 );
				bw.Write(m_dForceRatio   );

				bw.Write(m_nSpare1       ); //Spare
                bw.Write(m_nSpare2       );
                bw.Write(m_nSpare3       );
                bw.Write(m_nSpare4       );
                bw.Write(m_nSpare5       );
									     
                //					     
                bw.Write(m_bSpare1       ); //Spare
                bw.Write(m_bSpare2       );
                bw.Write(m_bSpare3       );
                bw.Write(m_bSpare4       );
                bw.Write(m_bSpare5       );

                //					     
                bw.Write(m_dDisX_PCtoPS  );
                bw.Write(m_dDisY_PCtoPS  );

                bw.Write(m_dSpare1       ); //Spare
                bw.Write(m_dSpare2       );
                bw.Write(m_dSpare3       );
                bw.Write(m_dSpare4       );
                bw.Write(m_dSpare5       );

				bw.Write(enMillINFO.nTotalCycle);

				for (int n = 0; n < 10; n++)
				{
					bw.Write(enMillINFO.nPath     [n]);
					bw.Write(enMillINFO.nRPM      [n]);
					bw.Write(enMillINFO.dForce    [n]);
					bw.Write(enMillINFO.dXSpeed   [n]);
					bw.Write(enMillINFO.dXDistance[n]);
					bw.Write(enMillINFO.dYDistance[n]);
				}


				bw.Flush();

            }        
		}
		//---------------------------------------------------------------------------
		public void fn_LoadVisnResult(bool load)
		{
			FM.fn_LoadVisionResult(load, ref vresult);
		}
        //-------------------------------------------------------------------------------------------------
        public double fn_SetDefaultForce(double Target)
        {
			//No1> DCOM = (BotLoadCell + 115)/122.8571429 : (SetValue + yintercept) / slope + forceOffset
			double dSetRatio  = 0.0 ;
		
			double dIntercept = FM.m_stMasterOpt.dYIntercept ;
			double dYSlope    = FM.m_stMasterOpt.dYSlope     ;
			double dOffset    = FM.m_stMasterOpt.dforceOffset;

			//dSetRatio = ((Target + dIntercept) / dYSlope) + dOffset;
			dSetRatio = (Target * dYSlope) + dIntercept + dOffset;

			m_dForceRatio = dSetRatio;

			return dSetRatio;
        }
		//---------------------------------------------------------------------------
		public int fn_GetCurrMillCnt(bool visible = false)
		{
			int nCurrCnt = (int)IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_Milling_Cnt];
			int nTotal   = visible? (nCurrCnt / 2) : nCurrCnt; //

			return nTotal; 
		}
		//---------------------------------------------------------------------------
        public int fn_GetTotalMillCnt(bool cln = false)
        {
			//
			int nTotal     = 0;
			int nTotalStep = cln ? g_VisionManager.CurrentRecipe.CleaningCount : vresult.nTotalStep; 

			if(cln)
			{
				nTotal = g_VisionManager.CurrentRecipe.Cleaning[m_nCleanCnt].PathCount * 2; //JUNG/200603
			}
            else
			{
				nTotal = vresult.stRecipeList[m_nPoliCnt].nPathCnt * 2; //JUNG/200603
            }

            return nTotal;
        }
		//-------------------------------------------------------------------------------------------------
		public int fn_GetTotalMillPathCnt()
		{//Recipe Total Path Co
			int nTotal = 0;
			int nTotalStep = vresult.nTotalStep;

			for (int i = 0; i < nTotalStep; i++)
			{
				nTotal += vresult.stRecipeList[i].nPathCnt * vresult.stRecipeList[i].nCycle * 2;
			}
			return nTotal;
		}
		//-------------------------------------------------------------------------------------------------
		public int fn_GetMillPercent()
		{
			int nRtn = (int)(((double)m_nTotalPathCnt / (double)fn_GetTotalMillPathCnt()) * 100.0);

			if (nRtn < 0) nRtn = 0;

			return nRtn;
		}

		//---------------------------------------------------------------------------
		public bool fn_IsSpindleOffCnt(bool cln = false)
        {
			//
			bool rtn     = false; 
            int  nUseSet = FM.m_stSystemOpt.nSpindleOffCnt * 2; //

			int  nTotal = fn_GetTotalMillCnt(cln);
			int  nCurr  = fn_GetCurrMillCnt ();

			if (nCurr >= nTotal - nUseSet)
			{
				rtn = true; 
			}

			return rtn;
        }
		//---------------------------------------------------------------------------
		public bool fn_IsDCOMReSetCnt()
        {
			//
			bool rtn     = false; 
            int  nUseSet = FM.m_stMasterOpt.nDCOMCnt; //

			int  nTotal = fn_GetTotalMillCnt();
			int  nCurr  = fn_GetCurrMillCnt ();

			if (nCurr >= nTotal - nUseSet)
			{
				rtn = true; 
			}

			return rtn;
        }

		//---------------------------------------------------------------------------
		private bool fn_CheckCleanPos(double start_x, double end_x, double start_y, double end_y)
		{
			bool rtn = true;

			double dClenCenterX = MOTR[(int)EN_MOTR_ID.miSPD_X].GetPosToCmdId(EN_COMD_ID.User2);
			double dClenCenterY = MOTR[(int)EN_MOTR_ID.miCLN_Y].GetPosToCmdId(EN_COMD_ID.User1);
			double dMax_X = 8;
			double dMax_Y = 8;

			if (Math.Abs(start_x - dClenCenterX) > dMax_X)
			{
				rtn = false; 
			}

            if (Math.Abs(end_x - dClenCenterX) > dMax_X)
            {
                rtn = false;
            }

            if (Math.Abs(start_y - dClenCenterY) > dMax_Y)
            {
                rtn = false;
            }

            if (Math.Abs(end_y - dClenCenterY) > dMax_Y)
            {
                rtn = false;
            }

            return rtn; 
		}
		//---------------------------------------------------------------------------
		public void fn_MillBuffEnd()
		{
			IO.fn_ForceBufferStop();
	    	//IO.fn_RunBuffer      (BFNo_13_MILLING   , false);
	    	//IO.fn_RunBuffer      (BFNo_14_FORCECHECK, false);
            IO.fn_StopBuffer     (BFNo_14_FORCECHECK);
            IO.fn_StopBuffer     (BFNo_13_MILLING);

            fn_SetSpindleRun     (swOff);
			IO.fn_SetSoftLimit   ((int)EN_MOTR_ID.miSPD_Z, false, false);

			IO.fn_SetOpenLoopOff ();
			IO.fn_GroupDisable   ();

			//
			MOTR.Stop(m_iMotrXId);
			MOTR.Stop(EN_MOTR_ID.miPOL_Y);
			MOTR.Stop(EN_MOTR_ID.miCLN_Y);

		}
		//---------------------------------------------------------------------------
		private void fn_MillingLog(int cycle, int step)
        {
			if (m_nPreCycleNo == fn_GetCurrMillCnt()) return;
			
			m_nPreCycleNo = fn_GetCurrMillCnt();

			//string sLog = string.Format($" STEP:{step+1} , CYCLE:{cycle+1} , COUNT:{m_nPreCycleNo+1} , X:{GetEncPos_X()} , Z:{GetEncPos_Z()} , Y:{SEQ_POLIS.GetEncPos_Y()} , RPM:{fn_GetSpindleSpeed()} , TOP LOADCELL:{IO.fn_GetTopLoadCell(true)}({IO.fn_GetTopLoadCell()}g)");
			//string sLog = string.Format($" STEP:{step + 1} , CYCLE:{cycle + 1} , COUNT:{m_nPreCycleNo + 1} , X:{GetEncPos_X()} , Z:{GetEncPos_Z()} , Y:{SEQ_POLIS.GetEncPos_Y()} , RPM:{fn_GetSpindleSpeed()} , TOP LOADCELL:{IO.fn_GetTopLoadCellAsBTM(true)}N / {IO.fn_GetTopLoadCellAsBTM()}g");
			string sLog = string.Format($" STEP:{step + 1} , CYCLE:{cycle + 1} , COUNT:{m_nPreCycleNo + 1} , X:{GetEncPos_X()} , Z:{GetEncPos_Z()} , Y:{SEQ_POLIS.GetEncPos_Y()} , RPM:{fn_GetSpindleSpeed()} , " +
										$"TOP LOADCELL:{IO.fn_GetTopLoadCellAsBTM(true)}N / {IO.fn_GetTopLoadCellAsBTM()}g , Low: {IO.fn_GetTopLoadCell()}g");


			fn_WriteLog(sLog, EN_LOG_TYPE.ltMill);

			m_nTotalPathCnt++;

			if (m_nPreCycleNo == fn_GetTotalMillCnt())
            {
				fn_WriteLog("---------------------------------------------------------------------------", EN_LOG_TYPE.ltMill);
			}
		}
		//---------------------------------------------------------------------------
		public void fn_SetVisnCamOffset()
        {
            double m_SpindleOffsetX = MOTR[(int)EN_MOTR_ID.miSPD_X].GetPosToCmdId(EN_COMD_ID.User1) - MOTR[(int)EN_MOTR_ID.miSPD_X].GetPosToCmdId(EN_COMD_ID.User12);
            double m_SpindleOffsetY = MOTR[(int)EN_MOTR_ID.miPOL_Y].GetPosToCmdId(EN_COMD_ID.User2) - MOTR[(int)EN_MOTR_ID.miPOL_Y].GetPosToCmdId(EN_COMD_ID.User1);

            g_VisionManager._RecipeVision.SpindleOffsetX = m_SpindleOffsetX ;
			g_VisionManager._RecipeVision.SpindleOffsetY = m_SpindleOffsetY ;

			fn_WriteLog($"Set Vision Camera Offset [m_SpindleOffsetX : {m_SpindleOffsetX:F4} / m_SpindleOffsetY : {m_SpindleOffsetY:F4}]");
        }											
		//---------------------------------------------------------------------------
		public void fn_StepDataClear()
		{//JUNG/200824
		
			//
			m_nPoliCnt    = 0;
			m_nPolCycle   = 0;
            m_nTotalCycle = 0; 

			m_bOffDone    = false;
			m_bDCOMReset  = false;
			m_nPreCycleNo = -1;

			//
			m_nCleanCnt = 0; 
		}
		//---------------------------------------------------------------------------
		public void fn_LoadcellInit(bool All = false)
		{
            if(All) IO.fn_SetTopOffset();
            LDCBTM.fn_SetZero(); //JUNG/200708
        }
		//---------------------------------------------------------------------------
		public bool fn_ForceTestCycle()
        {//고객사 요청 임의 TEST
            bool   r1, r2;
			string sLog = string.Empty;
			double ONEGRAM_TO_NEWTON = 0.00980665;
			int    nTotalCnt = 3000;

			//Tool Force Check One Cycle
			if (m_nManStep < 0) m_nManStep = 0;

            switch (m_nManStep)
            {
                default:
                    m_nManStep = 0;
                    return true;

                case 10:
					
					//Check Tool
					if (!fn_IsExistTool())
					{
						fn_UserMsg("Check Tool.");

                        m_nManStep = 0;
                        return true;
                    }

					//

					m_iCmdX = EN_COMD_ID.User5;
                    m_iCmdZ = EN_COMD_ID.User5 ; //Force Check Pos.

					//
					nTestCnt = 0;

					IO.fn_SetTestDCOMValue(SEQ_SPIND._dTestDCOM); //IO.fn_SetTestDCOMValue(3.0);
					IO.fn_SetOpenLoopOff();

					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
				
				case 11:
					if (!m_tDelayTime.OnDelay(true, 1500)) return false;

					r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

                    m_nManStep++;
                    return false;

                case 12:
                    r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //X-Axis Tool Check Position
					r2 = SEQ_STORG.fn_ReqMoveToolPickPos();
                    if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 13:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					if (!fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1)) return false ;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;
				
				case 14:
					if (!m_tDelayTime.OnDelay(true, 1500)) return false;

					//
					fn_LoadcellInit();

					//Test Buffer 실행
					IO.fn_RunBuffer(12, true);
					
					nTestCnt++;

					m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 15:
                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					//Check Buffer Run State
					if (!IO.fn_IsForceBuffRun(true)) return false; 

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 16:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun(true)) return false;
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 17:

					if (!m_tDelayTime.OnDelay(true, 1500)) return false;

					//Log
					sLog = string.Format($"CNT = {nTestCnt} / {nTotalCnt}, BTM = {LDCBTM._dLoadCellValue * ONEGRAM_TO_NEWTON:F3}N , TOP = {IO.fn_GetTopLoadCell(true):F3}");
					fn_WriteLog(sLog, EN_LOG_TYPE.ltTest);

					//
					IO.fn_SetOpenLoopOff();

					//Check Test Count
					if (nTestCnt < nTotalCnt)
					{
						m_tDelayTime.Clear();
						m_nManStep = 13;
                        return false;
                    }
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 18:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId, EN_COMD_ID.Wait1);
                    if (!r1) return false;

                    m_nManStep = 0;
                    return true;

            }
        }
		//---------------------------------------------------------------------------
		//Auto Calibration Cycle : DCOM, Top/Bottom Load Cell
		//---------------------------------------------------------------------------
        public bool fn_AutoCalCycle()
        {
			bool   r1, r2;
			double dPos, dStartDCOM;
			double dTop, dBtm, dDCOM;

			m_nCalTotalCnt  = 10 ;
			dStartDCOM      = FM.m_stMasterOpt.dStartDCOM; // 2.5;
			int nTotalCycle = m_nTotalCalCycle;

			bool xPinExist = IO.XV[(int)EN_INPUT_ID.xSPD_ToolExist];

			//
			m_iCmdX = EN_COMD_ID.User5 ;
			m_iCmdZ = EN_COMD_ID.User5 ;

			//Tool Exist Check Cycle
			if (m_nManStep < 0) m_nManStep = 0;
			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:
					
					//Tool Clamp Fwd
					fn_MoveToolClamp(ccFwd);

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
                    if (!r1 || !r2) return false;

					if(!xPinExist)
                    {
						fn_UserMsg("Check Tool Exist!!!!");

						m_nManStep = 0;
                        return true;

                    }

                    fn_WriteLog("[START] Auto Calibration");

					//
					fn_LoadcellInit(true);

                    m_nCalCount = 0;
					m_nCalCycle = 0; 

					//Buffer Check
					if (IO.fn_IsBuffRun(BFNo_14_FORCECHECK)) IO.fn_StopBuffer(BFNo_14_FORCECHECK);

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 11:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					r1 = fn_MoveMotr(m_iMotrXId, m_iCmdX); //X-Axis Tool Check Position
					r2 = SEQ_STORG.fn_IsForceOkPos() ? true : SEQ_STORG.fn_ReqMoveToolPickPos(); //JUNG/200826
                    if (!r1 || !r2) return false;

					m_nManStep++;
                    return false;
				
				//Force Start
				case 12:

                    r1 = fn_MoveMotr(m_iMotrZId, m_iCmdZ); //Z-Axis Force Check Position
                    if (!r1) return false;

					//dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(m_iCmdZ);
					//IO.fn_SetForceBuffData(dPos);
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 13:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    //Calibrate Force Value
                    dStartDCOM = dStartDCOM + (0.4 * m_nCalCount);
                    //IO.fn_SetDCOM(dStartDCOM);

                    //
                    dPos = MOTR[(int)m_iMotrZId].GetPosToCmdId(m_iCmdZ);
					r1 = IO.fn_ForceBufferRunforCal(dPos, dStartDCOM);
                    if (!r1)
                    {
						//
						fn_UserMsg("Buffer Run Error!!!");

						//
						m_nManStep = 0;
                        return true;
                    }
                    
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 14:

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					if (!IO.fn_IsForceBuffRun()) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
				
				case 15:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    //Check Buffer End
                    if (IO.fn_IsForceBuffRun()) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 16:

					if (!m_tDelayTime.OnDelay(true, 1000 * 5)) return false; //10sec

					//Data Record - TOP, Bottom, DCOM
					dTop  = IO    .fn_GetTopLoadCell(    ); //g으로 계산
					dBtm  = LDCBTM.fn_GetBtmLoadCell(    ); 
					dDCOM = IO    .fn_GetDCOMValue  (    );

					m_dCalVal[m_nCalCount, 0] = dTop  ;
					m_dCalVal[m_nCalCount, 1] = dBtm  ;
					m_dCalVal[m_nCalCount, 2] = dDCOM ;

					sTemp = string.Format($" CalCount : [({m_nCalCycle}/{nTotalCycle}) {m_nCalCount}/{m_nCalTotalCnt}] 1.TOP : {dTop:F4} 2.BTM : {dBtm:F4} 3.DCOM : {dDCOM:F2}");
					Console.WriteLine(sTemp);
					fn_WriteLog(sTemp);


					//Top & Bottom
					m_pTopBtm [m_nCalCount].X = dTop ;
					m_pTopBtm [m_nCalCount].Y = dBtm ;

					//Bottom & DCOM
					m_pBtmDCOM[m_nCalCount].X = dBtm ;
					m_pBtmDCOM[m_nCalCount].Y = dDCOM;


					if (++m_nCalCount < m_pTopBtm.Length)
                    {
						//sTemp = string.Format($" Cal Count : {m_nCalCount}/{m_nCalTotalCnt}");
						//fn_WriteLog(sTemp);

						if (m_nCalCount < m_nCalTotalCnt)
						{
							IO.fn_SetOpenLoopOff();

							m_tDelayTime.Clear();
							m_nManStep++ ;
							return false;
						}

                    }
					else
                    {
						IO.fn_SetOpenLoopOff();

						fn_UserMsg("Total Test Count Error!!!");

						//
						m_nManStep = 19;
                        return false;
                    }

                    IO.fn_SetOpenLoopOff();

					m_tDelayTime.Clear();
					m_nManStep = 18;
                    return false;
				
				case 17:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; //
					if (!SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z, EN_COMD_ID.Wait1)) return false;

					m_tDelayTime.Clear();
                    
					m_nManStep = 12;
                    return false;

                case 18:

					if(m_nCalCycle == 0)
					{ 
						//LMS
						m_pLMSTopBtm  = fn_LineFitting_LMS(m_pTopBtm , m_nCalTotalCnt); //Get BOTTOM Value
						m_pLMSBtmDCOM = fn_LineFitting_LMS(m_pBtmDCOM, m_nCalTotalCnt); //Get DCOM Value

						FM.m_stMasterOpt.dYSlopeBT     = Math.Round(m_pLMSTopBtm .X, 7);
						FM.m_stMasterOpt.dYInterceptBT = Math.Round(m_pLMSTopBtm .Y, 7);

						FM.m_stMasterOpt.dYSlope       = Math.Round(m_pLMSBtmDCOM.X, 7);
						FM.m_stMasterOpt.dYIntercept   = Math.Round(m_pLMSBtmDCOM.Y, 7);

						sTemp = string.Format($"[LMS] BOTTOM/DCOM - f(DCOM) = {FM.m_stMasterOpt.dYSlope:F5} X + {FM.m_stMasterOpt.dYIntercept:F5}");
						Console.WriteLine(sTemp);
						fn_WriteLog(sTemp);
						
						sTemp = string.Format($"[LMS] TOP/BOTTOM - f(BTM) = {FM.m_stMasterOpt.dYSlopeBT:F5} X + {FM.m_stMasterOpt.dYInterceptBT:F5}");
						Console.WriteLine(sTemp);
						fn_WriteLog(sTemp);
					}


                    if (++m_nCalCycle < nTotalCycle)
                    {
                        //
                        m_nCalCount = 0;

                        IO.fn_SetOpenLoopOff();

                        m_tDelayTime.Clear();
                        m_nManStep = 17;
                        return false;
                    }
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 19:
					
					if (!m_tDelayTime.OnDelay(true, 500)) return false; //

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1) ;
                    if (!r1) return false;

					PgMaster.fn_DisplayOffset();

					fn_WriteLog("[END] Auto Calibration");
					
					fn_UserMsg("[END] Auto Calibration");

					m_nManStep = 0;
                    return true;

            }
        }
		//---------------------------------------------------------------------------
		public bool fn_AutoTeachingCycle()
        {
			//
			//"Auto Teaching Pos - Left"   , "mm", EN_POSN_ID.User20 , 3, EN_MOTR_ID.miSPD_X);
			//"Auto Teaching Pos - Right"  , "mm", EN_POSN_ID.User21 , 3, EN_MOTR_ID.miSPD_X);

			//"Vision Align Pos - Bottom"  , "mm", EN_POSN_ID.User20, 3, EN_MOTR_ID.miSTR_Y);
			//"Vision Align Pos - Top"     , "mm", EN_POSN_ID.User21, 3, EN_MOTR_ID.miSTR_Y);


			//
			bool r1, r2, r3, r4, r5;
			double dXpos = GetEncPos_X();
            double dYpos = SEQ_STORG.GetEncPos_Y();

			if (m_nManStep < 0) m_nManStep = 0;
			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:

                    r1 = fn_MoveMotr(m_iMotrZId , EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);
                    r2 = fn_MoveMotr(m_iMotrZ1Id, EN_COMD_ID.Wait1) || MOTR.CmprPosByCmd(m_iMotrZ1Id, EN_COMD_ID.Wait1);
					r3 = SEQ_STORG.fn_MoveCylLock(ccFwd);
					if (!r1 || !r2 || !r3) return false;


                    fn_WriteLog("[START] Auto Storage Teaching");
					fn_WriteTestLog("[START] Auto Storage Teaching", EN_TEST_LOG_NAME.ToolAutoCal.ToString());

					m_nAlingRetry   = 0;
					m_pMotrMov.X    = 0.0;
					m_pMotrMov.Y    = 0.0;

					m_dAlignOffsetX = 0.0;
					m_dAlignOffsetY = 0.0;

					m_nTestCount    = 0 ;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 11:
					
					// 1) Move Left Bottom Position
					r1 = fn_MoveMotr             (m_iMotrXId, EN_COMD_ID.User20); //
					r2 = SEQ_STORG.fn_ReqMoveMotr(m_iBtmMotr, EN_COMD_ID.User20); //
					r3 = fn_MoveCylLensCvr(ccOpen);
					r4 = fn_MoveCylIR     (ccOpen);
					r5 = g_VisionManager.fn_SetLight_Tool(swOn);
					if (!r1 || !r2 || !r3 || !r4 || !r5) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
				
				case 12:

                    //Call Vision Function
                    dXpos = GetEncPos_X();
                    dYpos = SEQ_STORG.GetEncPos_Y();
                    
					g_VisionManager._ToolAlign.fn_PinSearch(dXpos, dYpos, 0);
                    
					// In-position 체크 및 이동량 계산.
                    r1 = g_VisionManager._ToolAlign.fn_CheckSearchResultInPosition(out m_pMotrMov);
                    if (!r1)
                    {
						m_dAlignOffsetX = dXpos; 
                        m_dAlignOffsetY = dYpos;

						m_nBackStep = m_nManStep; 
                        m_nManStep = 22;
                        return false;
                    }

					sTemp = string.Format($"[0] Pos : X = {dXpos:F5} / Y = {dYpos:F5}");
					fn_WriteTestLog(sTemp, EN_TEST_LOG_NAME.ToolAutoCal.ToString());

					//
					m_nAlingRetry = 0;
					m_nBackStep   = 0; 

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 13:

					// 2) Move Left Top Position
					r1 = fn_MoveMotr             (m_iMotrXId, EN_COMD_ID.User20); //
					r2 = SEQ_STORG.fn_ReqMoveMotr(m_iBtmMotr, EN_COMD_ID.User21); //
					if (!r1 || !r2 ) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 14:

                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    dXpos = GetEncPos_X();
                    dYpos = SEQ_STORG.GetEncPos_Y();
                    
					g_VisionManager._ToolAlign.fn_PinSearch(dXpos, dYpos, 1);

                    // In-position 체크 및 이동량 계산.
                    r1 = g_VisionManager._ToolAlign.fn_CheckSearchResultInPosition(out m_pMotrMov);
                    if (!r1)
                    {
                        m_dAlignOffsetX = dXpos;
                        m_dAlignOffsetY = dYpos;

                        m_nBackStep = m_nManStep;
                        m_nManStep = 22;
                        return false;
                    }

                    sTemp = string.Format($"[1] Pos : X = {dXpos:F5} / Y = {dYpos:F5}");
                    fn_WriteTestLog(sTemp, EN_TEST_LOG_NAME.ToolAutoCal.ToString());


                    //
                    m_nAlingRetry = 0;
                    m_nBackStep = 0;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 15:
					// 3) Move Right Top Position
					r1 = fn_MoveMotr             (m_iMotrXId, EN_COMD_ID.User21); //
					r2 = SEQ_STORG.fn_ReqMoveMotr(m_iBtmMotr, EN_COMD_ID.User21); //
					if (!r1 || !r2 ) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 16:

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    dXpos = GetEncPos_X();
                    dYpos = SEQ_STORG.GetEncPos_Y();
                    g_VisionManager._ToolAlign.fn_PinSearch(dXpos, dYpos, 2);

                    // In-position 체크 및 이동량 계산.
                    r1 = g_VisionManager._ToolAlign.fn_CheckSearchResultInPosition(out m_pMotrMov);
                    if (!r1)
                    {
                        m_dAlignOffsetX = dXpos;
                        m_dAlignOffsetY = dYpos;

                        m_nBackStep = m_nManStep;
                        m_nManStep = 22;
                        return false;
                    }

                    sTemp = string.Format($"[2] Pos : X = {dXpos:F5} / Y = {dYpos:F5}");
                    fn_WriteTestLog(sTemp, EN_TEST_LOG_NAME.ToolAutoCal.ToString());

                    //
                    m_nAlingRetry = 0;
                    m_nBackStep = 0;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;


                case 17:
					// 4) Move Right Bottom Position
					r1 =  fn_MoveMotr            (m_iMotrXId, EN_COMD_ID.User21); //
					r2 = SEQ_STORG.fn_ReqMoveMotr(m_iBtmMotr, EN_COMD_ID.User20); //
					if (!r1 || !r2 ) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 18:

                    if (!m_tDelayTime.OnDelay(true, 1000)) return false;

                    dXpos = GetEncPos_X();
                    dYpos = SEQ_STORG.GetEncPos_Y();
                    g_VisionManager._ToolAlign.fn_PinSearch(dXpos, dYpos, 3);

                    // In-position 체크 및 이동량 계산.
                    r1 = g_VisionManager._ToolAlign.fn_CheckSearchResultInPosition(out m_pMotrMov);
                    if (!r1)
                    {
                        m_dAlignOffsetX = dXpos;
                        m_dAlignOffsetY = dYpos;

                        m_nBackStep = m_nManStep;
                        m_nManStep = 22;
                        return false;
                    }

                    sTemp = string.Format($"[3] Pos : X = {dXpos:F5} / Y = {dYpos:F5}");
                    fn_WriteTestLog(sTemp, EN_TEST_LOG_NAME.ToolAutoCal.ToString());

                    //
                    m_nAlingRetry = 0;
                    m_nBackStep = 0;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 19:

					//
                    g_VisionManager._ToolAlign.fn_ProcessOffset(out Point ColOffset, out Point RowOffset, out Point RefPos);

					//Cal. Pos.
					SEQ_STORG.fn_SetAlignOffset(RowOffset, ColOffset, RefPos);

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 20:

					if (!m_tDelayTime.OnDelay(true, 500)) return false; //

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);

					r2 = fn_MoveCylLensCvr(ccClose);
					r3 = fn_MoveCylIR(ccClose);
					r4 = g_VisionManager.fn_SetLight_Tool(swOff);

					if (!r1 || !r2 || !r3 || !r4) return false;


					fn_WriteLog("[END] Auto Storage Teaching");
					fn_WriteTestLog("[END] Auto Storage Teaching", EN_TEST_LOG_NAME.ToolAutoCal.ToString());

                    if (++m_nTestCount < m_nAlignTestCount)
                    {
                        m_nManStep = 11;
                        return false;
                    }
                    
					fn_UserMsg("[END] Auto Storage Teaching");

					m_nManStep = 0;
                    return true;
                
				case 21:

                    if (!m_tDelayTime.OnDelay(true, 500)) return false; //

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1);
                    r2 = fn_MoveCylLensCvr(ccClose);
                    r3 = fn_MoveCylIR     (ccClose);
                    r4 = g_VisionManager.fn_SetLight_Tool(swOff);

                    if (!r1 || !r2 || !r3 || !r4) return false;

                    fn_WriteLog("[END] Auto Storage Teaching");
                    fn_UserMsg("[END] Auto Storage Teaching - FAIL");


                    m_nManStep = 0;
                    return true;
				
				case 22: //ABS Move

                    r1 = fn_MoveDirect          (m_iMotrXId, m_dAlignOffsetX + m_pMotrMov.X);
                    r2 = SEQ_STORG.fn_MoveDirect(m_iBtmMotr, m_dAlignOffsetY + m_pMotrMov.Y);
                    if (!r1 || !r2) return false;

					if(m_nAlingRetry++ > 5)
					{
						m_nManStep = 21;
                        return false;
                    }

                    m_tDelayTime.Clear();
					m_nManStep = m_nBackStep ;
                    return false;
            }
        }

		//---------------------------------------------------------------------------
		private bool fn_CheckPolishingPos(double StartXPos, double EndXPos, double StartYPos, double EndYPos, double YDistance, double XSpeed)
        {



			return true; 
        }
	}

}

