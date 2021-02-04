using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.BaseUnit.ActuatorId;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.BaseUnit.ERRID;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using WaferPolishingSystem.BaseUnit;
using System.Diagnostics.SymbolStore;

namespace WaferPolishingSystem.Unit
{
    public class SeqStorage
    {
        //Timer
        TOnDelayTimer m_tMainCycle      = new TOnDelayTimer();
        TOnDelayTimer m_tToStart        = new TOnDelayTimer();
        TOnDelayTimer m_tToStop         = new TOnDelayTimer();

        TOnDelayTimer m_tStepCycle      = new TOnDelayTimer();
        TOnDelayTimer m_tToolOutCycle   = new TOnDelayTimer();

		TOnDelayTimer m_tHome           = new TOnDelayTimer();

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Vars.
		bool          m_bToStart        ; //To... Flag.
		bool          m_bToStop         ;
		//bool          m_bWorkEnd        ;
								    
		bool          m_bDrngStep       ; //Step Cycle
		bool          m_bDrngToolOut    ; //Tool Discard Cycle
		bool          m_bDrngWait       ; //Wait 

		bool          m_bUseDirectPos   ;

		int           m_nSeqStep        ; //Step.
		int           m_nManStep        ;
		int           m_nHomeStep       ;
	
		int           m_nStepStep       ; //Tool Search Step
		int           m_nToolOutStep    ; //Tool Out Step

		//int           m_nWorkStep       ;
		int           m_AlignCnt        ;

		double        m_dDirectPosn     ; //Direct Moving Position.
		//double        m_dMPos           ; //Command Motor Position
		//double        m_dEPos           ; //Enc. Motor Position
		double        m_dYPos           ;

        double        m_dRefPosX   , m_dRefPosY   ;
        double        m_dXOffsetEnc, m_dYOffsetEnc;

		//
		string        sLogMsg; //for Log

		int           m_nPartId   ;
		EN_MOTR_ID    m_iMotrYId  ;
		EN_MOTR_ID    m_iMotrXId  ;
		EN_COMD_ID    m_iCmdY     ;

        EN_COMD_ID    Man_XCmdId  ;
        EN_COMD_ID    Man_YCmdId  ;


        TOOL_PICK_INFO ToolPickInfo = new TOOL_PICK_INFO(false);
		TOOL_PICK_INFO pickinfo     = new TOOL_PICK_INFO(false); //Manual

		//
		Point m_pColOffset;
		Point m_pRowOffset;


		bool   m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        int    m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;
        double m_dSpare1, m_dSpare2, m_dSpare3, m_dSpare4, m_dSpare5;

		
		//---------------------------------------------------------------------------
		//Property
		public int _nManStep       { get { return m_nManStep      ; } set { m_nManStep       = value; } }
		public int _nHomeStep      { get { return m_nHomeStep     ; } set { m_nHomeStep      = value; } }
		public int _nSeqStep       { get { return m_nSeqStep;       } }

		public bool _bDrngStep      { get { return m_bDrngStep    ; } }
        public bool _bDrngToolOut   { get { return m_bDrngToolOut ; } }
        public bool _bDrngWait      { get { return m_bDrngWait    ; } }




		/************************************************************************/
        /* 생성자.                                                               */
        /************************************************************************/
        public SeqStorage()
        {

            //
            Init();

            //
            m_nPartId   = (int)EN_SEQ_ID.STORAGE;
					    
            m_iMotrYId  = EN_MOTR_ID.miSTR_Y ;
			m_iMotrXId  = EN_MOTR_ID.miSPD_X ;

			m_iCmdY     = EN_COMD_ID.NoneCmd ;

			//
			m_bUseDirectPos = false;


		}
		//---------------------------------------------------------------------------
        private void Init()
        {
			m_bToStart       = false;
			m_bToStop        = false;
			//m_bWorkEnd       = false;

			m_bDrngStep      = false;
			m_bDrngToolOut   = false;
			m_bDrngWait      = false;

			m_nSeqStep       = 0;
			m_nManStep       = 0;
			m_nHomeStep      = 0;

			m_nStepStep      = 0;
			m_nToolOutStep   = 0;

			m_dDirectPosn    = 0;
			m_dYPos          = 0;

			m_AlignCnt       = 0;
							 
			m_dXOffsetEnc    = 0;
			m_dYOffsetEnc    = 0;
			m_dRefPosX       = 0;
			m_dRefPosY       = 0;

			//m_dMPos          = 0;
			//m_dEPos          = 0;

			//m_nWorkStep     = -1;


			//Timer Clear
			m_tMainCycle.Clear();
			m_tToStart     .Clear();
			m_tToStop      .Clear();
			m_tStepCycle   .Clear();
			m_tToolOutCycle.Clear();

			m_tHome        .Clear();

            //
            m_bSpare1 = m_bSpare2 = m_bSpare3 = m_bSpare4 = m_bSpare5 = false;
			m_nSpare1 = m_nSpare2 = m_nSpare3 = m_nSpare4 = m_nSpare5 = 0;
			m_dSpare1 = m_dSpare2 = m_dSpare3 = m_dSpare4 = m_dSpare5 = 0.0;

		}
		//---------------------------------------------------------------------------
		public void fn_Reset()
		{
			m_bToStart        = false; 
			m_bToStop         = false;
			//m_bWorkEnd        = false;
		
			m_bDrngStep       = false;
			m_bDrngToolOut    = false;
			m_bDrngWait       = false;
							  

			m_nSeqStep        = 0; 
			m_nManStep        = 0; 
			m_nHomeStep       = 0;

			m_nStepStep       = 0;
			m_nToolOutStep    = 0;

			//m_nWorkStep       = -1;

			m_dDirectPosn     = 0.0; 
			//m_dEPos           = 0.0;
			//m_dMPos           = 0.0;
			m_dYPos           = 0.0;

			//Timer Clear
			m_tMainCycle.Clear();
			m_tToStart  .Clear();
			m_tToStop   .Clear();
			m_tHome     .Clear();

			fn_InitToolPickInfo(ref ToolPickInfo);

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
		public double GetEncPos_Y () { return MOTR.GetEncPos(m_iMotrYId ); }
		public double GetCmdPos_Y () { return MOTR.GetCmdPos(m_iMotrYId ); }
		public double GetTrgPos_Y () { return MOTR.GetTrgPos(m_iMotrYId ); }
		//---------------------------------------------------------------------------
		private bool CheckDstb(EN_MOTR_ID Motr, EN_COMD_ID Cmd = EN_COMD_ID.NoneCmd,
                                    int Step = NONE_STEP, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE, double DirPosn = 0.0)
        {
			//Var.
			bool isNoRun        = SEQ.fn_IsNoRun() && m_nManStep == 0 ;
			bool isOpenDoor     = SEQ.fn_IsAnyDoorOpen(); 
					    
			double dEnc_Y       = GetEncPos_Y ();
			double dCmd_Y       = GetCmdPos_Y ();
			double dTrg_Y       = GetTrgPos_Y ();
			
			double dEnc_SPDX    = MOTR.GetEncPos(EN_MOTR_ID.miSPD_X );
			double dEnc_SPDZ    = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z );
			double dEnc_SPDZ1   = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z1);

			double dMaxXAxisPos  = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLX_POS_DSTB_POLIZ ].dPos;
			double dMinZAxisPos  = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ_POS_MOVE_BTMY  ].dPos;
			double dMinZ1AxisPos = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ1_POS_MOVE_BTMY ].dPos;
			double dNextPosn     = 0;							

			if (Motr != EN_MOTR_ID.miSTR_Y) return false;

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
			if (Motr == EN_MOTR_ID.miSTR_Y)
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
				if (dEnc_SPDZ > dMinZAxisPos) 
				{
					if (isNoRun) fn_UserMsg(string.Format($"Spindle Z-Axis Position Value Error. Check Z-Axis Position.[MIN:{dMinZAxisPos}]"));
					return false;
				}

				if (dEnc_SPDZ1 > dMinZ1AxisPos)
				{
					if (isNoRun) fn_UserMsg(string.Format($"Spindle Z1-Axis Position Value Error. Check Z1-Axis Position.[MIN:{dMinZ1AxisPos}]"));
					return false;
				}


				//Check Storage Position, Actuator while run
				if(SEQ._bRun || m_nManStep != 0)
				{
					if (!fn_IsLockPos())
					{
                        if (isNoRun) fn_UserMsg(string.Format($"Check Storage Position.[ADD:{(int)EN_INPUT_ID.xSTR_PosCheck:D4}]"));
                        return false;
                    }
                    
					if (!fn_IsCylLock() && FM.m_stMasterOpt.nStorageSkip != 1 ) 
					{															 
						if (isNoRun) fn_UserMsg(string.Format($"Check Storage Lock Cylinder."));
						return false; 
					}
				}

			}

            //
            return true; 

		}
		//---------------------------------------------------------------------------
		public bool fn_IsForceOkPos()
		{
			return GetEncPos_Y() >= MOTR[(int)m_iMotrYId].GetPosToCmdId(EN_COMD_ID.FSP1_1);
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
		public bool fn_ReqMoveToolChkPos()
		{
			//Move tool discard position for check basket
			fn_MoveMotr(m_iMotrYId, EN_COMD_ID.Wait2);

			//Ok.
			return true;
		}
		//---------------------------------------------------------------------------
        public bool fn_ReqMoveToolPickPos()
        {
			//
			return fn_MoveMotr(m_iMotrYId, EN_COMD_ID.FindStep1, EN_MOTR_VEL.Normal, 0, EN_FPOSN_INDEX.Index1);
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
			bool r1; 
			int iFHomeErr = MOTR._iFHomeErr;
			if (m_tHome.OnDelay(m_nHomeStep >= 10, 60 * 1000 * 2))  //2min
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0111 + m_nPartId); //
				m_nHomeStep = 0;
				return true;
			}

			
			//Move Home.
			switch (m_nHomeStep)
			{
				case 0:
					m_nHomeStep = 0;
					return true;

				case 10: //Clear Home end Flag
                    if (!CheckDstb(EN_MOTR_ID.miSTR_Y)) return false;

					IO.fn_RunBuffer((int)EN_MOTR_ID.miSTR_Y, false);

					//Clear Home end Flag
					MOTR.ClearHomeEnd(EN_MOTR_ID.miSTR_Y);

                    m_nHomeStep++;
					return false;

                case 11: //

					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSTR_Y , true);

					m_nHomeStep++;
                    return false;

                case 12: //Buffer Run
                    if (!IO.fn_RunBuffer(BFNo_06_HOME_STR_Y, true))
                    {
                        fn_UserMsg("ACS RUN Buffer Error - BUFF : " + BFNo_06_HOME_STR_Y);
                        m_nHomeStep = 0;
                        return true;
                    }

                    m_nHomeStep++;
                    return false;

                case 13: //Check Buffer Run 
                    if (!IO.fn_IsBuffRun(BFNo_06_HOME_STR_Y)) return false;

                    m_nHomeStep++;
                    return false;

                case 14:
                    //Check Error
					if (IO.DATA_ACS_TO_EQ[BFNo_06_HOME_STR_Y] == 1)
                    {
                        fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_06_HOME_STR_Y:D2}");
                        m_nHomeStep = 0;
                        return true;
                    }

                    //Check Buffer End
                    if (IO.fn_IsBuffRun(BFNo_06_HOME_STR_Y)) return false;

                    EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miSTR_Y , false);

					MOTR[(int)EN_MOTR_ID.miSTR_Y].SetHomeEndDone(true);

                    m_nHomeStep++;
                    return false;

                case 15:
					r1 = fn_MoveMotr(m_iMotrYId, EN_COMD_ID.Wait1);
					if (!r1) return false;


                    m_nHomeStep = 0 ;
                    return true;

            }

            return false; 
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStartCon()
		{
			m_bToStart = false;
			m_tToStart.Clear();

	
			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStopCon()
		{
			m_bToStop = false; 
			m_tToStop.Clear();

			//Check Step
			if (m_nSeqStep        != 0) return false;

			if (m_nStepStep       != 0) return false;
			if (m_nToolOutStep    != 0) return false;

			//Check During Flag
			if (m_bDrngStep           ) return false; 
			if (m_bDrngToolOut        ) return false; 
			if (m_bDrngWait           ) return false;

			m_tToStop.Clear();

			return true; 

		}
		//---------------------------------------------------------------------------
		public bool fn_ToStart()
		{
			//
			if (m_bToStart) return true; 

			//Check Start Time Out
			bool bIsInitState = SEQ.fn_IsSeqStatus(EN_SEQ_STATE.INIT);
			
			if (m_tToStart.OnDelay(!m_bToStart && !bIsInitState, 20 * 1000))
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0150 + m_nPartId, true);
				return false;
			}


			//Lock Cylinder
			if (!fn_MoveCylLock(ccFwd)) return false;
			if (!fn_MoveMotr(m_iMotrYId, EN_COMD_ID.Wait2)) return false; //Move tool discard position for check basket


            //Timer Clear
            m_tMainCycle.Clear();

			m_tStepCycle   .Clear();
			m_tToolOutCycle.Clear();

			//Flag On
			m_bToStart  = true ; 
			m_bToStop   = false;

			//m_nWorkStep = -1;

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
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0160 + m_nPartId, true);
				return false;
			}

			//Clear Step Index
			m_nSeqStep       = 0; 

			m_nStepStep      = 0;
			m_nToolOutStep   = 0;
			
			m_bToStop = true;

			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_IsLockPos()
		{
			if (FM.m_stMasterOpt.nStorageSkip == 1) return true;

			bool r1  =  true; // !IO.XV[(int)EN_INPUT_ID.xSTR_PosCheck ];
			bool r2  = !IO.XV[(int)EN_INPUT_ID.xSTR_PosCheck]; //!IO.XV[(int)EN_INPUT_ID.xSTR_ExitCheck];
			bool rtn =  r1 && r2; 

			return rtn;
		}
		//---------------------------------------------------------------------------
		public bool fn_IsCylLock()
		{
			bool r1 = ACTR.GetCylStat((int)EN_ACTR_LIST.aStrg_LockBtm , ccFwd);
			bool r2 = ACTR.GetCylStat((int)EN_ACTR_LIST.aStrg_LockTop , ccFwd);

			return (r1 && r2 ); 

		}

        //---------------------------------------------------------------------------
        public bool fn_MoveCylLock(int act)
        {
			//Check Position Sensor
			if(!fn_IsLockPos() && act == ccFwd)
			{
				if(!SEQ._bRun)
				{
					fn_UserMsg("Check Storage Position");
				}
				return false;
			}

			bool r1, r2;
			
			
			r1 = fn_MoveCylLock1(act); //Bottom
			r2 = fn_MoveCylLock2(act); //Top
			if (!r1 || !r2) return false;

			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_MoveCylLock1(int act, bool Man = false) //Bottom
		{
			bool isTopBwd = ACTR.GetCylStat((int)EN_ACTR_LIST.aStrg_LockTop , ccBwd);
		
			if (!isTopBwd && act == ccBwd && !Man) return false; 
            
			//
            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aStrg_LockBtm , act);
			return (r1);
        }
        //---------------------------------------------------------------------------
        public bool fn_MoveCylLock2(int act, bool Man = false) //Top
		{
			bool isBtmFwd = ACTR.GetCylStat((int)EN_ACTR_LIST.aStrg_LockBtm , ccFwd);

			//Check Lock1 Cylinder
			if (!isBtmFwd && act == ccFwd && !Man) return false;

            //
            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aStrg_LockTop , act);

            return (r1);
        }
		//---------------------------------------------------------------------------
		public bool fn_MoveCylDoor(int act)
		{
			bool isNoRun   = SEQ.fn_IsNoRun() && m_nManStep == 0;

			bool r1 = false ; // ACTR.MoveCyl(EN_ACTR_LIST.aStrg_Door, act);
            return r1;

		}
		//---------------------------------------------------------------------------
		public void fn_SetToolPickInfo(TOOL_PICK_INFO info)
		{
			ToolPickInfo = info;
		}
		//---------------------------------------------------------------------------
		public bool fn_AutoRun()
		{
			m_bUseDirectPos = FM.m_stMasterOpt.nUseDirPos == 1;

			m_dYPos = MOTR[(int)m_iMotrYId].GetCmdPos();

			bool bErr    = EPU._bIsErr;
			bool bManRun = FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE);

			//Time Check
			m_tMainCycle   .OnDelay((m_nSeqStep        != 0 && !bErr && !bManRun), 120 * 1000); //120sec

			m_tStepCycle   .OnDelay((m_nStepStep       != 0 && !bErr && !bManRun),  60 * 1000); //
			m_tToolOutCycle.OnDelay((m_nToolOutStep    != 0 && !bErr && !bManRun),  60 * 1000); //


			//
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0170 + m_nPartId, m_tMainCycle.Out))
			{
				sLogMsg = string.Format($"[STORAGE] Main Cycle Time Out : m_iSeqStep = {m_nSeqStep}");
			}
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0196, m_tStepCycle.Out))
			{
				sLogMsg = string.Format($"[STORAGE] Step Cycle Time Out : m_iToolPickStep = {m_nStepStep}");
			}
            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0197, m_tToolOutCycle.Out))
            {
                sLogMsg = string.Format($"[STORAGE] Tool Out Cycle Time Out : m_iToolChkStep = {m_nToolOutStep}");
            }


			//
			if (m_tMainCycle     .Out ||
				m_tStepCycle     .Out ||
				m_tToolOutCycle  .Out )
			{
				fn_WriteLog(sLogMsg);
				LOG.fn_CrntStateTrace(EN_SEQ_ID.STORAGE, sLogMsg);
				fn_Reset();
				return false; 
			}

			//Emergency Error Check
			bool bEMOErr = EPU.fn_IsEMOError();  // Emergency Error
			if (m_nSeqStep != 0 && bEMOErr)
			{
				//
				sLogMsg = string.Format($"[EMO][SEQ_STOR] Force Cycle End m_nSeqStep = {m_nSeqStep}");
				fn_WriteLog(sLogMsg);
				
				fn_Reset();
				
				m_nSeqStep = 0;
				return false; 
			}

			//Decide Step
			if (m_nSeqStep == 0)
			{
				//Var
				bool bYWaitPos	     = MOTR.CmprPosByCmd(m_iMotrYId , EN_COMD_ID.Wait1);

				//
				bool xStoragExist    = fn_IsLockPos(); //IO.XV[(int) EN_INPUT_ID.xSTR_PosCheck];
				bool isDrngSpdlPick  = SEQ_SPIND._bDrngToolPick ;
				bool isDrngSpdlPlace = SEQ_SPIND._bDrngToolPlce ;
				bool isStorageOutPos = MOTR.CmprPosByCmd(m_iMotrYId, EN_COMD_ID.User3);
				bool isPickFind      = ToolPickInfo.bFind ;
				int  nPysiStep       = MOTR.GetCrntStep (m_iMotrYId) ;

				//Step Condition
				bool isConStep         = xStoragExist && isDrngSpdlPick  && isPickFind       ; 
				bool isConToolOut      = xStoragExist && isDrngSpdlPlace && !isStorageOutPos ; 
				bool isConWait         = false; //!isConStep   && !isConToolOut   && !bYWaitPos;

				//Clear Var.
				m_bDrngStep            = false;
				m_bDrngToolOut         = false;
				m_bDrngWait            = false;

				//Step Clear
				m_nStepStep     = 0;
				m_nToolOutStep  = 0;

				//Check Sequence Stop
				if ( SEQ._bStop                                          ) return false; 
				if ( EPU.fn_GetHasErr()                                  ) return false; 
				if (!SEQ._bRun && !FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE)) return false; 

				//
				if (isConStep        ) { m_bDrngStep       = true; m_nSeqStep = 100 ; m_nStepStep       = 10; goto __GOTO_CYCLE__; }
				if (isConToolOut     ) { m_bDrngToolOut    = true; m_nSeqStep = 200 ; m_nToolOutStep    = 10; goto __GOTO_CYCLE__; }
				if (isConWait        ) { m_bDrngWait       = true; m_nSeqStep = 1300;                         goto __GOTO_CYCLE__; }

			}

			//Cycle Start
			__GOTO_CYCLE__:

			//Cycle
			switch (m_nSeqStep)
			{
				case 100:
					if (fn_StepCycle(ref m_bDrngStep, ref ToolPickInfo)) m_nSeqStep = 0;
					return false;

				case 200:
					if (fn_ToolOutCycle(ref m_bDrngToolOut)) m_nSeqStep = 0;
					return false;


				//Wait Position 
				case 1300:
					//Z-Axis, Z1-Axis Wait Position 

					m_nSeqStep++;
					return false;

				case 1301:
					//X-Wait Position

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
		private bool fn_StepCycle(ref bool Flag, ref TOOL_PICK_INFO PickInfo)
        {
			//
			bool   r1;
			int    nFindRow = PickInfo.nFindRow; 
			double dYPos    = PickInfo.dYpos   ;

			//
			m_iCmdY = EN_COMD_ID.FindStep1; //Storage Y-Axis

			//Step Cycle
			if (m_nStepStep < 0) m_nStepStep = 0;

			switch (m_nStepStep)
			{
				default:
					m_nStepStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;

					PickInfo.bFind = false;

					fn_WriteSeqLog("[START] STEP");

                    m_nStepStep++;
                    return false;
				
				case 11:
					if (!fn_MoveCylLock(ccFwd)) return false; 

					m_nStepStep++;
					return false;

				case 12:
					if(m_bUseDirectPos)
					{
						r1 = fn_MoveDirect(m_iMotrYId, dYPos);
					}
					else
					{
						r1 = fn_MoveMotr(m_iMotrYId, m_iCmdY, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
					}
					
					if (!r1) return false;

					fn_WriteSeqLog("[END] STEP");

					Flag = false;
					m_nToolOutStep = 0;
                    return true;
            }
        }
		//---------------------------------------------------------------------------
        private bool fn_ToolOutCycle(ref bool Flag)
        {
			//
			bool r1;

			//
			m_iCmdY = EN_COMD_ID.User3; //Storage Y-Axis

			//Tool discard Cycle
			if (m_nToolOutStep < 0) m_nToolOutStep = 0;

			switch (m_nToolOutStep)
			{
				default:
					m_nToolOutStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;
					
					fn_WriteSeqLog("[START] TOOL OUT");

					m_nToolOutStep++;
					return false;

				case 11: 
					if(SEQ._bStop)
					{
						m_nToolOutStep = 0;
                        Flag = false;
                        return false;
					}

					r1 = fn_MoveMotr(m_iMotrYId, m_iCmdY);
					if (!r1) return false;


					fn_WriteSeqLog("[END] TOOL OUT");

					Flag = false;
					m_nToolOutStep = 0;
                    return true;
            }
        }
		//---------------------------------------------------------------------------
		public bool fn_StepOneCycle()
		{
			bool r1 ;
			bool isMainZWait  = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
			bool isMainZ1Wait = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);

			//Pick Cycle
			if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				case 10:
                    if(!isMainZWait || !isMainZ1Wait)
					{
                        fn_UserMsg("Please Check Spindle Z or Z1 Axis Position");
                        m_nManStep = 0;
                        return true;
                    }

					fn_InitToolPickInfo(ref pickinfo);

					pickinfo = DM.TOOL.GetPickInfoStorage(EN_PIN_FIND_MODE.fmNewPol);
					if (!pickinfo.bFind)
					{
                        fn_UserMsg("Not exist tool in storage");
                        m_nManStep = 0;
                        return true;
                    }

                    m_nManStep ++;
                    return false;

				case 11:
					if (!fn_MoveCylLock(ccFwd)) return false;

					m_nManStep++;
					return false;

                case 12:
                    if (m_bUseDirectPos)
                    {
                        r1 = fn_MoveDirect(m_iMotrYId, pickinfo.dYpos);
                    }
                    else
                    {
						r1 = fn_MoveMotr(m_iMotrYId, EN_COMD_ID.FindStep1, EN_MOTR_VEL.Normal, pickinfo.nFindRow, EN_FPOSN_INDEX.Index1);
                    }
					if (!r1) return false; 

					m_nManStep++;
                    return false;

                case 13:
					
					fn_UserMsg("Step One Cycle OK.");

                    m_nManStep=0;
                    return true;
			}

			return false; 
		}
		//---------------------------------------------------------------------------
		public bool fn_AlignOneCycle()
		{
			bool r1, r2 ;
			bool isMainZWait  = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
			bool isMainZ1Wait = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);
			double dXPos, dYPos; 

			//Pick Cycle
			if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true; 

				case 10:
                    if(!isMainZWait || !isMainZ1Wait)
					{
                        fn_UserMsg("Please Check Spindle Z or Z1 Axis Position");
                        m_nManStep = 0;
                        return true;
                    }

					if(!fn_IsLockPos())
					{
						fn_UserMsg(string.Format($"Check Storage Position.[ADD:{(int)EN_INPUT_ID.xSTR_PosCheck:D4}]"));
                        m_nManStep = 0;
                        return true;
                    }
					
					m_AlignCnt  = 0;

                    m_nManStep ++;
                    return false;

				case 11:
					if (!fn_MoveCylLock(ccFwd)) return false;

					
					//Vision -> Position Clear
					//Init Parameter Setting(Reference Tool Position)



					m_nManStep++;
					return false;

                case 12:
					//0 : 0,0
					//1 : 1,0
					//2 : 1,1
					//3 : 0,1
					switch (m_AlignCnt)
					{
						case 0: Man_XCmdId = EN_COMD_ID.User13; Man_YCmdId = EN_COMD_ID.User1; break;
						case 1: Man_XCmdId = EN_COMD_ID.User14; Man_YCmdId = EN_COMD_ID.User1; break;
						case 2: Man_XCmdId = EN_COMD_ID.User14; Man_YCmdId = EN_COMD_ID.User2; break;
						case 3: Man_XCmdId = EN_COMD_ID.User13; Man_YCmdId = EN_COMD_ID.User2; break;
						default:
                            m_nManStep = 15;
                            return false;
					}
                    
					m_nManStep++;
                    return false;

				case 13:

					r1 = SEQ_SPIND.fn_ReqMoveMotr(m_iMotrXId, Man_XCmdId);
					r2 =           fn_MoveMotr   (m_iMotrYId, Man_YCmdId);
					if (!r1 || !r2) return false; 

					m_nManStep++;
                    return false;


                case 14: //Call Vision Align Function && Wait Processing

					//Check Position
					dXPos = MOTR[(int)m_iMotrXId].GetPosToCmdId(Man_XCmdId);
					dYPos = MOTR[(int)m_iMotrXId].GetPosToCmdId(Man_YCmdId);

					//if(         (m_AlignCnt, dXPos, dYPos))  return false;


					if (m_AlignCnt++<4)
					{
                        m_nManStep = 12;
                        return false;
                    }

                    m_nManStep++;
                    return false;

                case 15:

					//Get Result



					//Position Save





                    fn_UserMsg("Storage Align One Cycle OK.");

                    m_nManStep=0;
                    return true;
			}

		}
		//---------------------------------------------------------------------------
        public bool fn_StorageOutOneCycle()
		{
			bool r1, r2;
			bool isMainZWait  = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
			bool isMainZ1Wait = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);

			//Storage Out Cycle
			if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				case 10:
                    if(!isMainZWait || !isMainZ1Wait)
					{
                        fn_UserMsg("Please Check Spindle Z or Z1 Axis Position");
                        m_nManStep = 0;
                        return true;
                    }

					//Check Y Position
					if (MOTR.CmprPosByCmd(m_iMotrYId, EN_COMD_ID.User4))
					{
                        m_nManStep = 0;
                        return true;
                    }

					if(!fn_IsCylLock())
                    {
						fn_UserMsg("Storage Lock Cylinder is Down.");

						m_nManStep = 0;
                        return true;
                    }
					
					SEQ.fn_DoorLock();

					m_nManStep ++;
                    return false;

                case 11:
					
					r1 = fn_MoveMotr(m_iMotrYId, EN_COMD_ID.User4);
					r2 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X, EN_COMD_ID.User2); //move cleaning position
					if (!r1 || !r2) return false;

					fn_MoveCylLock(ccBwd);

					m_nManStep++;
                    return false;

                case 12:
					if (!fn_MoveCylLock(ccBwd)) return false;

					SEQ.fn_DoorUnLock();

					SEQ.fn_SetLight(true); //SEQ._bLightOn = true; //JUNG/200601

					fn_UserMsg("STORAGE UNLOCK OK");

                    m_nManStep=0;
                    return true;

                default:
                    m_nManStep = 0;
                    return true;

            }
		}
		//---------------------------------------------------------------------------
		public bool fn_StoragInOneCycle()
		{
			bool r1;
			bool isMainZWait  = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
			bool isMainZ1Wait = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);

			//Storage In Cycle
			if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
				case 10:
					if (!isMainZWait || !isMainZ1Wait)
					{
						fn_UserMsg("Please Check Spindle Z or Z1 Axis Position");
						m_nManStep = 0;
						return true;
					}

					SEQ.fn_SetLight(true);

					m_nManStep++;
					return false;

				case 11:
					if (!fn_IsLockPos())
					{
                        fn_UserMsg("Please check storage Lock position.");
                        m_nManStep = 0;
                        return true;
                    }

					SEQ.fn_DoorLock();
					
					if (!fn_MoveCylLock(ccFwd)) return false;

                    m_nManStep++;
                    return false;
				
				case 12:
					r1 = fn_MoveMotr(m_iMotrYId, EN_COMD_ID.Wait2); //storage check position
					if (!r1) return false;

					SEQ.fn_SetLight(false); //SEQ._bLightOn = false ; //JUNG/200601

					fn_UserMsg("STORAGE LOCK OK");

					m_nManStep = 0;
					return true;

				default:
                    m_nManStep = 0;
                    return true;
            }
		}
		//---------------------------------------------------------------------------
		public bool fn_CheckPos()
        {
			//Check Option
			if (FM.m_stMasterOpt.nUseDirPos == 0) return true; 

			//Check Storage Tool Pick Position 
			ST_PIN_POS Pos = new ST_PIN_POS(-1);

			for (int r = 0; r<DM.STOR[siPolish]._nMaxRow; r++)
			{
				for (int c = 0; c < DM.STOR[siPolish]._nMaxCol; c++)
				{
					Pos = DM.STOR[siPolish].GetPinPos(r, c);
					if (Pos.dXPos < 1 || Pos.dYPos < 1) return false; 
				}
			}

            return true;
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_IsExtToolBasket()
        {
            if (FM.m_stMasterOpt.nStorageSkip == 1) return true;

            return IO.XV[(int)EN_INPUT_ID.xSTR_ExtToolBasket];
        }
		//---------------------------------------------------------------------------
		public bool fn_SetAlignOffset(Point row, Point col, Point refpos)
        {//JUNG/201210/Align Offset Save
			m_pRowOffset.X = 0;

			m_pRowOffset = row;
			m_pColOffset = col;

			fn_WriteLog($"Set Storage Align - Row : X_{row.X:F5}/Y_{row.Y:F5} / Col : X_{col.X:F5}/Y_{col.Y:F5}");
			fn_WriteLog($"Set Storage Align - Ref Pos Row : {refpos.X:F5} / Col : {refpos.Y:F5}");

            fn_WriteTestLog($"Set Storage Align - Row : X_{row.X:F5}/Y_{row.Y:F5} / Col : X_{col.X:F5}/Y_{col.Y:F5}", EN_TEST_LOG_NAME.ToolAutoCal.ToString());
			fn_WriteTestLog($"Set Storage Align - Ref Pos Row : {refpos.X:F5} / Col : {refpos.Y:F5}"          , EN_TEST_LOG_NAME.ToolAutoCal.ToString());


			//
			double dXoffset = UserClass.g_VisionManager._RecipeVision.HardwareOffsetX;
			double dYoffset = UserClass.g_VisionManager._RecipeVision.HardwareOffsetY;

			m_dRefPosX = refpos.X + dXoffset;
            m_dRefPosY = refpos.Y + dYoffset;


			//
			if (FM.m_stMasterOpt.nUseDirPos == 1)
            {
				//Spindle - X
				//MOTR[(int)EN_MOTR_ID.miSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP1_1] = m_dRefPosX                       ; //Polishing Tool
				//MOTR[(int)EN_MOTR_ID.miSPD_X].MP.dPosn[(int)EN_POSN_ID.FSP2_1] = m_dRefPosX - (3*(m_pColOffset.X)); //Cleaning Tool

				//Storage - Y
				//MOTR[(int)EN_MOTR_ID.miSTR_Y].MP.dPosn[(int)EN_POSN_ID.FSP1_1] = m_dRefPosY                       ; //Polishing Tool
				//MOTR[(int)EN_MOTR_ID.miSTR_Y].MP.dPosn[(int)EN_POSN_ID.FSP2_1] = m_dRefPosY + (3*(m_pColOffset.Y)); //Cleaning Tool
				

				//Position Setting
                for (int r = 0; r < DM.STOR[siPolish]._nMaxRow; r++)
                {
                    for (int c = 0; c < DM.STOR[siPolish]._nMaxCol; c++)
                    {
						DM.STOR[(int)EN_STOR_ID.POLISH].SetPos(r, c, m_dRefPosX + ((c * m_pColOffset.X) + (r * m_pRowOffset.X)), m_dRefPosY + ((c * m_pColOffset.Y) + (r * m_pRowOffset.Y)));
					}
                }

				//Cleaning Setting
				for (int r = 0; r < DM.STOR[siClean]._nMaxRow; r++)
				{
					for (int c = 0; c < DM.STOR[siClean]._nMaxCol; c++)
					{
						DM.STOR[(int)EN_STOR_ID.CLEAN].SetPos(r, c, m_dRefPosX + (((c + 3) * m_pColOffset.X) + (r * m_pRowOffset.X)), m_dRefPosY + (((c + 3) * m_pColOffset.Y) + (r * m_pRowOffset.Y)));
					}
				}

				//SAVE
				DM.fn_LoadMap(flSave);

				//LOG
				string     sTemp = string.Empty; 
				string     sLog  = string.Empty;
				ST_PIN_POS Pos   = new ST_PIN_POS(-1);

				fn_WriteTestLog(">> [POLISHING TOOL STORAGE] <<", EN_TEST_LOG_NAME.ToolAutoCal.ToString());
				for (int r = DM.STOR[siPolish]._nMaxRow - 1; r >= 0 ; r--)
                {
					sLog = string.Empty;
					for (int c = 0; c < DM.STOR[siPolish]._nMaxCol; c++)
                    {
						Pos = DM.STOR[(int)EN_STOR_ID.POLISH].GetPinPos(r, c);
						sTemp = string.Format($"{Pos.dXPos:F5},{Pos.dYPos:F5}     "); 
						sLog += sTemp ;
					}
					fn_WriteTestLog(sLog, EN_TEST_LOG_NAME.ToolAutoCal.ToString());
                }
                
				fn_WriteTestLog(">> [CLEANING TOOL STORAGE] <<", EN_TEST_LOG_NAME.ToolAutoCal.ToString());
                for (int r = DM.STOR[siClean]._nMaxRow - 1; r >= 0; r--)
                {
					sLog = string.Empty;
                    for (int c = 0; c < DM.STOR[siClean]._nMaxCol; c++)
                    {
                        Pos = DM.STOR[(int)EN_STOR_ID.CLEAN].GetPinPos(r, c);
                        sTemp = string.Format($"{Pos.dXPos:F5},{Pos.dYPos:F5}     ");
                        sLog += sTemp;

                    }
                    fn_WriteTestLog(sLog, EN_TEST_LOG_NAME.ToolAutoCal.ToString());
                }

            }

            return true; 
        }
        //---------------------------------------------------------------------------
        private void fn_WriteSeqLog(string log)
        {
			string stemp = string.Format($"[{EN_SEQ_ID.STORAGE.ToString()}] ");
            fn_WriteLog(stemp + log, EN_LOG_TYPE.ltEvent, EN_SEQ_ID.STORAGE);
        }
		//---------------------------------------------------------------------------
		public void fn_SaveLog(ref string Msg)
        {
			string sTemp = string.Empty;

			Msg = string.Empty;

			//
			Msg += "[SeqStorage]\r\n"; 
			Msg += sTemp = string.Format($"m_bToStart            = {m_bToStart           }\r\n");
			Msg += sTemp = string.Format($"m_bToStop             = {m_bToStop            }\r\n");
			Msg += sTemp = string.Format($"m_bDrngStep           = {m_bDrngStep          }\r\n");
			Msg += sTemp = string.Format($"m_bDrngToolOut        = {m_bDrngToolOut       }\r\n");
			Msg += sTemp = string.Format($"m_bDrngWait           = {m_bDrngWait          }\r\n");
			Msg += sTemp = string.Format($"m_nSeqStep            = {m_nSeqStep           }\r\n");
			Msg += sTemp = string.Format($"m_nManStep            = {m_nManStep           }\r\n");
			Msg += sTemp = string.Format($"m_nHomeStep           = {m_nHomeStep          }\r\n");
			Msg += sTemp = string.Format($"m_nStepStep           = {m_nStepStep          }\r\n");
			Msg += sTemp = string.Format($"m_nToolOutStep        = {m_nToolOutStep       }\r\n");
			Msg += sTemp = string.Format($"ToolPickInfo.bFind    = {ToolPickInfo.bFind   }\r\n");
			Msg += sTemp = string.Format($"ToolPickInfo.nFindRow = {ToolPickInfo.nFindRow}\r\n");
			Msg += sTemp = string.Format($"ToolPickInfo.nFindCol = {ToolPickInfo.nFindCol}\r\n");
			Msg += sTemp = string.Format($"ToolPickInfo.dXpos    = {ToolPickInfo.dXpos   }\r\n");
			Msg += sTemp = string.Format($"ToolPickInfo.dYpos    = {ToolPickInfo.dYpos   }\r\n");


		}

		//---------------------------------------------------------------------------
		public void fn_UpdateFlag(ref Grid grd)
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
            int nRow = 0; 

            Items[nRow, 0].Content = "bToStart       "  ;  Items[nRow++, 1].Content = string.Format($"{m_bToStart           }");
			Items[nRow, 0].Content = "bToStop        "  ;  Items[nRow++, 1].Content = string.Format($"{m_bToStop            }");
			Items[nRow, 0].Content = "_";				   Items[nRow++, 1].Content = string.Format($"");				    
			Items[nRow, 0].Content = "bDrngStep      "  ;  Items[nRow++, 1].Content = string.Format($"{m_bDrngStep          }");
            Items[nRow, 0].Content = "bDrngToolOut   "  ;  Items[nRow++, 1].Content = string.Format($"{m_bDrngToolOut       }");
			Items[nRow, 0].Content = "bDrngWait      "  ;  Items[nRow++, 1].Content = string.Format($"{m_bDrngWait          }");
			Items[nRow, 0].Content = "";				   Items[nRow++, 1].Content = string.Format($"");				    
			Items[nRow, 0].Content = "nSeqStep"         ;  Items[nRow++, 1].Content = string.Format($"{m_nSeqStep           }");
			Items[nRow, 0].Content = "nManStep"         ;  Items[nRow++, 1].Content = string.Format($"{m_nManStep           }");
			Items[nRow, 0].Content = "nHomeStep"        ;  Items[nRow++, 1].Content = string.Format($"{m_nHomeStep          }");
			Items[nRow, 0].Content = "_";				   Items[nRow++, 1].Content = string.Format($"");				    
			Items[nRow, 0].Content = "nStepStep   "     ;  Items[nRow++, 1].Content = string.Format($"{m_nStepStep          }");
			Items[nRow, 0].Content = "nToolOutStep"     ;  Items[nRow++, 1].Content = string.Format($"{m_nToolOutStep       }");
			Items[nRow, 0].Content = "_";				   Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "PickInfo.bFind"   ;  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.bFind   }");
			Items[nRow, 0].Content = "PickInfo.nFindRow";  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.nFindRow}");
			Items[nRow, 0].Content = "PickInfo.nFindCol";  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.nFindCol}");
			Items[nRow, 0].Content = "PickInfo.dXpos"   ;  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.dXpos   }");
			Items[nRow, 0].Content = "PickInfo.dYpos"   ;  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.dYpos   }");

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

			Items[nRow, 0].Content = "iSeqStep"             ;  Items[nRow++, 1].Content = string.Format($"{m_nSeqStep           }");
			Items[nRow, 0].Content = "iManStep"             ;  Items[nRow++, 1].Content = string.Format($"{m_nManStep           }");
			Items[nRow, 0].Content = "iHomeStep"            ;  Items[nRow++, 1].Content = string.Format($"{m_nHomeStep          }");
			Items[nRow, 0].Content = "_";                      Items[nRow++, 1].Content = string.Format($"");				     
			Items[nRow, 0].Content = "nStepStep"            ;  Items[nRow++, 1].Content = string.Format($"{m_nStepStep          }");
			Items[nRow, 0].Content = "nToolOutStep"         ;  Items[nRow++, 1].Content = string.Format($"{m_nToolOutStep       }");
			Items[nRow, 0].Content = "_";                      Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "ToolPickInfo.bFind"   ;  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.bFind   }");
			Items[nRow, 0].Content = "ToolPickInfo.nFindRow";  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.nFindRow}");
			Items[nRow, 0].Content = "ToolPickInfo.nFindCol";  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.nFindCol}");
			Items[nRow, 0].Content = "ToolPickInfo.dXpos"   ;  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.dXpos   }");
			Items[nRow, 0].Content = "ToolPickInfo.dYpos"   ;  Items[nRow++, 1].Content = string.Format($"{ToolPickInfo.dYpos   }");


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

                m_nSpare1     = br.ReadInt32  ();
                m_nSpare2     = br.ReadInt32  ();
                m_nSpare3     = br.ReadInt32  ();
                m_nSpare4     = br.ReadInt32  ();
                m_nSpare5     = br.ReadInt32  ();
                                   
                //
                m_bSpare1     = br.ReadBoolean();
                m_bSpare2     = br.ReadBoolean();
                m_bSpare3     = br.ReadBoolean();
                m_bSpare4     = br.ReadBoolean();
                m_bSpare5     = br.ReadBoolean();

                //
                m_dXOffsetEnc = br.ReadDouble ();
                m_dYOffsetEnc = br.ReadDouble ();
                m_dRefPosX    = br.ReadDouble ();
                m_dRefPosY    = br.ReadDouble ();

				m_dSpare1     = br.ReadDouble ();

            }
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);

				bw.Write(m_nSpare1    ); //Spare
                bw.Write(m_nSpare2    );
                bw.Write(m_nSpare3    );
                bw.Write(m_nSpare4    );
                bw.Write(m_nSpare5    );

                //
                bw.Write(m_bSpare1    ); //Spare
                bw.Write(m_bSpare2    );
                bw.Write(m_bSpare3    );
                bw.Write(m_bSpare4    );
                bw.Write(m_bSpare5    );

                //
                bw.Write(m_dXOffsetEnc);
                bw.Write(m_dYOffsetEnc);
                bw.Write(m_dRefPosX   );
                bw.Write(m_dRefPosY   );

                bw.Write(m_dSpare1    ); //Spare

                bw.Flush();

            }
        }







    }
}
