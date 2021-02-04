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
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.IOMap;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using static WaferPolishingSystem.BaseUnit.ActuatorId;
using static WaferPolishingSystem.BaseUnit.Magazine;
using WaferPolishingSystem.BaseUnit;
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.Define.UserClass;


namespace WaferPolishingSystem.Unit
{
	public class SeqTransfer
	{

		//Timer
		TOnDelayTimer m_tMainCycle   = new TOnDelayTimer();
		TOnDelayTimer m_tToStart     = new TOnDelayTimer();
		TOnDelayTimer m_tToStop      = new TOnDelayTimer();

		TOnDelayTimer m_tLoadCycle   = new TOnDelayTimer();
		TOnDelayTimer m_tUnloadCycle = new TOnDelayTimer();

		TOnDelayTimer m_tPickCycle   = new TOnDelayTimer();
		TOnDelayTimer m_tPlaceCycle  = new TOnDelayTimer();
		TOnDelayTimer m_tLotCycle    = new TOnDelayTimer();
		TOnDelayTimer m_tDelayTime   = new TOnDelayTimer();
		TOnDelayTimer m_tHomeDelay   = new TOnDelayTimer();

		TOnDelayTimer m_tHome        = new TOnDelayTimer();

		TOnDelayTimer m_tDelaly      = new TOnDelayTimer();

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Vars.
		bool m_bToStart       ; //To... Flag.
		bool m_bToStop        ;
		//bool m_bWorkEnd       ;
							  
		bool m_bDrngLoad      ; //Load Cycle
		bool m_bDrngUnload    ; //Unload Cycle
		bool m_bDrngWait      ; //Wait 
		bool m_bDrngPickPlate ;
		bool m_bDrngPlacePlate;
		bool m_bDrngLotOpen   ;


		bool m_bTESTMODE      ; //TEST Mode Flag

		int m_nSeqStep        ; //Step.
		int m_nManStep        ;
		int m_nHomeStep       ;

		int m_nLoadStep       ; //Load Step
		int m_nUnloadStep     ; //Unload Step
		int m_nPickStep       ;
		int m_nPlaceStep      ;
		int m_nLotStep        ;
		
		
		double m_dPreAlignPos ; //Pre-Align Theta Position
		double m_dDirectPosn  ; //Direct Moving Position.
		//double m_dMPos        ; //Command Motor Position
		//double m_dEPos        ; //Enc. Motor Position
		double m_dZPos        ;
		double m_dLastPickPos ;
		double m_dStartFwdPos ; //Pick or Place start position

		string m_sStartTime, m_sEndTime;
		string m_sShowMsg;
		string m_sSeqMsg = string.Empty;

		double m_dCycleTimeStart, m_dCycleTimeEnd;
		//
		string sLogMsg; //for Log
		

		int        m_nPartId ;
		EN_MOTR_ID m_iMotrTId;
		EN_MOTR_ID m_iMotrZId;

		PLATE_MOVE_INFO PlatePickInfo = new PLATE_MOVE_INFO(false);
		PLATE_MOVE_INFO PlatePlceInfo = new PLATE_MOVE_INFO(false);

		bool   m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
		int    m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;
		double m_dSpare1, m_dSpare2, m_dSpare3, m_dSpare4, m_dSpare5;


		//---------------------------------------------------------------------------
		//Property
		public int _nManStep           { get { return m_nManStep; } set { m_nManStep = value; } }
		public int _nHomeStep          { get { return m_nHomeStep; } set { m_nHomeStep = value; } }
		public int _nSeqStep           { get { return m_nSeqStep; } }
								       
		public bool _bDrngLoad         { get { return m_bDrngLoad      ; } }
		public bool _bDrngUnload       { get { return m_bDrngUnload    ; } }
		public bool _bDrngWait         { get { return m_bDrngWait      ; } }
		public bool _bDrngPickPlate    { get { return m_bDrngPickPlate ; } }
		public bool _bDrngPlacePlate   { get { return m_bDrngPlacePlate; } }
		public bool _bDrngLotOpen      { get { return m_bDrngLotOpen   ; } }
									   
		public string _sStartTime      { get { return m_sStartTime     ; } }
		public string _sEndTime        { get { return m_sEndTime       ; } }


		public double _dCycleTimeStart { get { return m_dCycleTimeStart; } }



        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public SeqTransfer()
		{

			//
			Init();

			//
			m_nPartId = (int)EN_SEQ_ID.TRANSFER;

			m_iMotrTId = EN_MOTR_ID.miTRF_T;
			m_iMotrZId = EN_MOTR_ID.miTRF_Z;

			m_dPreAlignPos    = 0.0;
			m_dLastPickPos    = -1 ;
			m_dStartFwdPos    = -1 ;
			m_dCycleTimeStart = 0.0;
			m_dCycleTimeEnd   = 0.0; 

		}

		//---------------------------------------------------------------------------
		private void Init()
		{
			m_bToStart        = false;
			m_bToStop         = false;
			//m_bWorkEnd        = false;
						      
			m_bDrngLoad       = false;
			m_bDrngUnload     = false;
			m_bDrngWait       = false;
			m_bTESTMODE       = false;

			m_bDrngPickPlate  = false;
			m_bDrngPlacePlate = false;
			m_bDrngLotOpen    = false;

			m_sShowMsg        = string.Empty; 


			m_nSeqStep    = 0;
			m_nManStep    = 0;
			m_nHomeStep   = 0;

			m_nUnloadStep = 0;
			m_nLoadStep   = 0;
			m_nPickStep   = 0;
			m_nPlaceStep  = 0;
			m_nLotStep    = 0;

			m_dDirectPosn = 0;
			//m_dMPos       = 0;
			//m_dEPos       = 0;

			m_sSeqMsg     = string.Empty;

			//Timer Clear
			m_tMainCycle  .Clear();
			m_tToStart    .Clear();
			m_tToStop     .Clear();
			m_tLoadCycle  .Clear();
			m_tUnloadCycle.Clear();

			m_tHome       .Clear();

			m_tDelayTime  .Clear();
			m_tHomeDelay  .Clear();

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

			m_bDrngLoad       = false;
			m_bDrngUnload     = false;
			m_bDrngWait       = false;

			m_bDrngPickPlate  = false;
			m_bDrngPlacePlate = false;
			m_bDrngLotOpen    = false;
						      
			m_bTESTMODE       = false;
						   
			m_nSeqStep        = 0;
			m_nManStep        = 0;
			m_nHomeStep       = 0;
						      
			m_nLoadStep       = 0;
			m_nUnloadStep     = 0;
			m_nPickStep       = 0;
			m_nPlaceStep      = 0;
			m_nLotStep        = 0;
						      
			m_dDirectPosn     = 0.0;
			//m_dEPos           = 0.0;
			//m_dMPos           = 0.0;

			m_dLastPickPos    = -1;
			m_dStartFwdPos    = -1;

			m_sSeqMsg         = string.Empty; 

			//Timer Clear
			m_tMainCycle.Clear();
			m_tToStart  .Clear();
			m_tToStop   .Clear();
			m_tHome     .Clear();
			m_tDelayTime.Clear();
			m_tHomeDelay.Clear();

			fn_InitPlateInfo(ref PlatePickInfo);
			fn_InitPlateInfo(ref PlatePlceInfo);

			m_sShowMsg = string.Empty;


		}

		//---------------------------------------------------------------------------
		public double GetEncPos_Z () { return MOTR.GetEncPos(m_iMotrZId); }
		public double GetCmdPos_Z () { return MOTR.GetCmdPos(m_iMotrZId); }
		public double GetTrgPos_Z () { return MOTR.GetTrgPos(m_iMotrZId); }
        public double GetEncPos_TH() { return MOTR.GetEncPos(m_iMotrTId); }
        public double GetCmdPos_TH() { return MOTR.GetCmdPos(m_iMotrTId); }
        public double GetTrgPos_TH() { return MOTR.GetTrgPos(m_iMotrTId); }


        //---------------------------------------------------------------------------
        private bool CheckDstb(EN_MOTR_ID Motr, EN_COMD_ID Cmd = EN_COMD_ID.NoneCmd,
									int Step = NONE_STEP, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE, double DirPosn = 0.0)
		{
			//Var.
			bool isNoRun    = SEQ.fn_IsNoRun() && m_nManStep == 0;
			bool isOpenDoor = SEQ.fn_IsAnyDoorOpen();

			double dEnc_Z = GetEncPos_Z();
			double dCmd_Z = GetCmdPos_Z();
			double dTrg_Z = GetTrgPos_Z();

			double dNextPosn = 0;


			if (Motr != EN_MOTR_ID.miTRF_Z && Motr != EN_MOTR_ID.miTRF_T) return false;

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

			//Check Disturbance Condition
			if (Motr == EN_MOTR_ID.miTRF_Z)
			{
				bool xTopTRBwd = IO.XV[(int)EN_INPUT_ID.xTRS_TopTRBwd];
				bool xBtmTRBwd = IO.XV[(int)EN_INPUT_ID.xTRS_BtmTRBwd];
				bool isManual  = MAN._nManNo == (int)EN_MAN_LIST.MAN_0452 || MAN._nManNo == (int)EN_MAN_LIST.MAN_0453;

				if (m_nSeqStep == 0 && (!xTopTRBwd || !xBtmTRBwd) && !isManual)
				{
                    MOTR.Stop(Motr);
                    if (isNoRun) fn_UserMsg("Check transfer backward position.");
                    return false;
                }

                if (!m_bDrngPickPlate && !m_bDrngPlacePlate && (!xTopTRBwd || !xBtmTRBwd) && !isManual)
                {
                    MOTR.Stop(Motr);
                    if (isNoRun) fn_UserMsg("Check transfer backward position.");
                    return false;
                }

				//Transfer Bottom Cylinder Fwd 상태에서는 이동 거리 제한 < 10mm
				double dMax = 13.0; 
				if (!xTopTRBwd && !xBtmTRBwd && (m_bDrngPickPlate || m_bDrngPlacePlate))
				{
					if (m_dStartFwdPos > 0)
					{
						if (dNextPosn > m_dStartFwdPos)
						{
							if (Math.Abs(dNextPosn - m_dStartFwdPos) > dMax)
							{
								MOTR.Stop(Motr);
								if (isNoRun) fn_UserMsg("Check transfer Z-Axis moving position. (> 13mm)");
								return false;
							}
						}
						else
						{
							if (Math.Abs(m_dStartFwdPos - dNextPosn) > dMax)
							{ 
								MOTR.Stop(Motr);
								if (isNoRun) fn_UserMsg("Check transfer Z-Axis moving position. (> 13mm)");
								return false;
							}
						}
					}
                }
			}

			//T-Axis
			if (Motr == EN_MOTR_ID.miTRF_T)
			{


			}

			//
			return true;
		}
    	//---------------------------------------------------------------------------
		public bool fn_MoveMotr(EN_MOTR_ID Motr, EN_COMD_ID Cmd, EN_MOTR_VEL iSPD = EN_MOTR_VEL.Normal,int Step = NONE_STEP, EN_FPOSN_INDEX Index = EN_FPOSN_INDEX.NONE)
		{
			bool bRet = false;
			double dPosn = 0.0;

			//Stop Command. 
			if (Cmd == EN_COMD_ID.Stop || Cmd == EN_COMD_ID.EStop) return MOTR.MoveAsComd(Motr, Cmd, iSPD, Step, Index);

			//Check Disturb.
			if (!CheckDstb(Motr, Cmd, Step, Index)) return false;

			//Jog Command
			if (Cmd == EN_COMD_ID.JogP) return MOTR.MoveAsComd(Motr, Cmd, iSPD, Step, Index);
			if (Cmd == EN_COMD_ID.JogN) return MOTR.MoveAsComd(Motr, Cmd, iSPD, Step, Index);

			//Find Step.
			if (((Cmd == EN_COMD_ID.FindStep1) ||
				 (Cmd == EN_COMD_ID.FindStep2) ||
				 (Cmd == EN_COMD_ID.FindStep3) ||
				 (Cmd == EN_COMD_ID.FindStep4)))
			{
				return MOTR.MoveAsComd(Motr, Cmd, iSPD, Step, Index);
			}

			//Command.
			bRet = MOTR.MoveAsComd(Motr, Cmd, iSPD, Step, (EN_FPOSN_INDEX)Index);
			if (bRet)
			{
				dPosn = MOTR[(int)Motr].GetPosToCmdId(Cmd);
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
			if (!CheckDstb(Motr, EN_COMD_ID.Direct, NONE_STEP, EN_FPOSN_INDEX.NONE, Posn)) return false;
			if (!MOTR.MoveAsComd(Motr, EN_COMD_ID.Direct, EN_MOTR_VEL.Normal, NONE_STEP, EN_FPOSN_INDEX.NONE, Posn)) return false;

			//if (m_sLogMoveEvt == "" || m_sLogMoveEvt == null) m_sLogMoveEvt = "MANUAL"; 
			//LogTP.FunctionMove ((int)m_iPartId, m_sLogMoveEvt, (int)Motr, Posn);

			//Reset Direct Position.
			m_dDirectPosn = 0.0;

			//Ok.
			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ReqMoveMotr(EN_MOTR_ID Motr, EN_COMD_ID Cmd)
		{
			//Check During.
			if (m_nSeqStep != 0) return false;

			//Move.
			return fn_MoveMotr(Motr, Cmd);
		}
		//---------------------------------------------------------------------------
		public bool fn_ReqMoveDirect(EN_MOTR_ID Motr, double Posn)
		{
			//Check During.
			if (m_nSeqStep != 0) return false;

			//Move.
			return fn_MoveDirect(Motr, Posn);
		}
		//---------------------------------------------------------------------------
        public bool fn_ReqMoveAlignPos()
        {
            //Move Calibration Position
            return fn_MoveMotr(m_iMotrTId, EN_COMD_ID.CalPos);
        }
		//---------------------------------------------------------------------------
        public bool fn_ReqMoveVisnPos()
        {
            //Move Vision Pre Inspection Position
            return fn_MoveMotr(m_iMotrTId, EN_COMD_ID.User1);
        }
		//---------------------------------------------------------------------------
        public bool fn_ReqMoveLoadPos()
        {
            //Move Load/Unload Position
            return fn_MoveMotr(m_iMotrTId, EN_COMD_ID.Wait1);
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
			if (m_tHome.OnDelay(m_nHomeStep >= 10, 90 * 1000))
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

				case 10:

					r1 = fn_MoveCylTopTR  (ccBwd );
					r2 = fn_MoveCylBtmTR  (ccBwd );
					r3 = fn_MoveCylTopTurn(ccDeg0);
					if (!r1 || !r2 || !r3) return false;

					m_tHomeDelay.Clear();
					m_nHomeStep++;
                    return false;

                case 11: //
					if (!m_tHomeDelay.OnDelay(true, 1000)) return false; 

					if (!CheckDstb(EN_MOTR_ID.miTRF_Z, EN_COMD_ID.Home)) return false;

					//Clear Home end Flag
					MOTR.ClearHomeEnd(EN_MOTR_ID.miTRF_T);
					MOTR.ClearHomeEnd(EN_MOTR_ID.miTRF_Z);

					m_nHomeStep++;
					return false;

				case 12: //
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miTRF_T, true);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miTRF_Z, true);

					m_nHomeStep++;
					return false;

				case 13: //Buffer Run

					r1 = IO.fn_SMCHome(EN_MOTR_ID.miTRF_T, true);
					r2 = IO.fn_SMCHome(EN_MOTR_ID.miTRF_Z, true);
					if (!r1 || !r2) return false;

					m_tHomeDelay.Clear();
					m_nHomeStep++;
					return false;

				case 14:
					if (!m_tHomeDelay.OnDelay(true, 500)) return false;

					//Check Buffer End
					r1 = MOTR[(int)EN_MOTR_ID.miTRF_T].GetHomeEnd(); EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miTRF_T, r1);
					r2 = MOTR[(int)EN_MOTR_ID.miTRF_Z].GetHomeEnd(); EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miTRF_Z, r2);

					if (!r1 || !r2) return false;

					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miTRF_T, false);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miTRF_Z, false);
                    
					m_tHomeDelay.Clear();
                    m_nHomeStep++;
                    return false;

                case 15:
                    if (!m_tHomeDelay.OnDelay(true, 500)) return false;

					r1 = fn_MoveMotr(EN_MOTR_ID.miTRF_Z, EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(EN_MOTR_ID.miTRF_T, EN_COMD_ID.Wait1);

					if (!r1 || !r2) return false; 

					m_nHomeStep = 0;
					return true;

			}

			return false;
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStartCon()
		{
			m_bToStart = false;
			m_tToStart.Clear();

			fn_SetPreAlignPos(m_dPreAlignPos);

			m_dLastPickPos = -1;
			m_dStartFwdPos = -1;

			m_sShowMsg = string.Empty;

			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStopCon()
		{
			m_bToStop = false;
			m_tToStop.Clear();

			//Check Step
			if (m_nSeqStep    != 0) return false;
							  
			if (m_nLoadStep   != 0) return false;
			if (m_nUnloadStep != 0) return false;
			if (m_nPickStep   != 0) return false;
			if (m_nPlaceStep  != 0) return false;
			if (m_nLotStep    != 0) return false;

			//Check During Flag
			if (m_bDrngLoad       ) return false;
			if (m_bDrngUnload     ) return false;
			if (m_bDrngWait       ) return false;
								  
			if (m_bDrngPickPlate  ) return false;
			if (m_bDrngPlacePlate ) return false;
			if (m_bDrngLotOpen    ) return false;

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

			//Check Cylinder Bwd
			bool r1 = fn_MoveCylBtmTR(ccBwd);
			bool r2 = fn_MoveCylTopTR(ccBwd);
			if (!r1 || !r2) return false;

			//if (fn_MoveDirect(m_iMotrZId, m_dZPos)) return false; //Last Position

			//Timer Clear
			m_tMainCycle  .Clear();

			m_tLoadCycle  .Clear();
			m_tUnloadCycle.Clear();

			//Flag On
			m_bToStart = true;
			m_bToStop  = false;

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

			fn_MoveCylTRWait(); //JUNG/200518/

			//Clear Step Index
			m_nSeqStep    = 0;
						  
			m_nLoadStep   = 0;
			m_nUnloadStep = 0;
			m_nPickStep   = 0;
			m_nPlaceStep  = 0;
			m_nLotStep    = 0;


			//Show Message
			if(m_sShowMsg != string.Empty) fn_UserMsg(m_sShowMsg);

			m_bToStop = true;

			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_AutoRun()
		{
			m_dZPos = MOTR[(int)m_iMotrZId].GetCmdPos();

			bool bErr       = EPU._bIsErr;
			bool bManRun    = FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE );
		        m_bTESTMODE = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);

			//Time Check
			m_tMainCycle  .OnDelay((m_nSeqStep    != 0 && !bErr && !bManRun), 120 * 1000); //120sec

			m_tLoadCycle  .OnDelay((m_nLoadStep   != 0 && !bErr && !bManRun), 60 * 1000); //
			m_tUnloadCycle.OnDelay((m_nUnloadStep != 0 && !bErr && !bManRun), 60 * 1000); //

			m_tPickCycle  .OnDelay((m_nPickStep   != 0 && !bErr && !bManRun), 60 * 1000); //
			m_tPlaceCycle .OnDelay((m_nPlaceStep  != 0 && !bErr && !bManRun), 60 * 1000); //
			m_tLotCycle   .OnDelay((m_nLotStep    != 0 && !bErr && !bManRun), 60 * 1000); //


			//
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0170 + m_nPartId, m_tMainCycle.Out))
			{
				sLogMsg = string.Format($"[TRASFER] Main Cycle Time Out : m_iSeqStep = {m_nSeqStep}");
			}
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0198, m_tLoadCycle.Out))
			{
				sLogMsg = string.Format($"[STORAGE] Step Cycle Time Out : m_iToolPickStep = {m_nLoadStep}");
			}
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0199, m_tUnloadCycle.Out))
			{
				sLogMsg = string.Format($"[STORAGE] Tool Out Cycle Time Out : m_iToolChkStep = {m_nUnloadStep}");
			}

			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0179, m_tPickCycle.Out))
			{
				sLogMsg = string.Format($"[STORAGE] Pick Cycle Time Out : m_nPickStep = {m_nPickStep}");
			}
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0178, m_tPlaceCycle.Out))
			{
				sLogMsg = string.Format($"[STORAGE] Place Cycle Time Out : m_nPlaceStep = {m_nPlaceStep}");
			}
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0177, m_tLotCycle.Out))
			{
				sLogMsg = string.Format($"[STORAGE] LOT Cycle Time Out : m_nLotStep = {m_nLotStep}");
			}


			//
			if (m_tMainCycle .Out || m_tPickCycle.Out  || m_tLoadCycle  .Out ||
				m_tPlaceCycle.Out || m_tLotCycle .Out  || m_tUnloadCycle.Out )
			{
				fn_WriteLog(sLogMsg);
				LOG.fn_CrntStateTrace(EN_SEQ_ID.TRANSFER, sLogMsg);
				fn_Reset();
				return false; 
			}

			//Emergency Error Check
			bool bEMOErr = EPU.fn_IsEMOError();  // Emergency Error
			if (m_nSeqStep != 0 && bEMOErr)
			{
				//
				sLogMsg = string.Format($"[EMO][SEQ_TRAN] Force Cycle End m_nSeqStep = {m_nSeqStep}");
				fn_WriteLog(sLogMsg);
				
				fn_Reset();
				
				m_nSeqStep = 0;
				return false; 
			}

			//Decide Step
			if (m_nSeqStep == 0)
			{
				//Var
				bool bZWaitPos	      = MOTR.CmprPosByCmd(m_iMotrZId , EN_COMD_ID.Wait1);

				//
				bool xTrasnExist      =  IO.XV[(int)EN_INPUT_ID.xTRS_TransPlateExist]; //Transfer Exist
				bool xMaga01Exist     = !IO.XV[(int)EN_INPUT_ID.xTRS_MagaLeftExist  ]; //
				bool xMaga02Exist     = !IO.XV[(int)EN_INPUT_ID.xTRS_MagaRightExist ]; //
									  
				bool isMaga1AllEmpty  = DM.MAGA[(int)EN_MAGA_ID.MAGA01].IsAllEmpty();
				bool isMaga2AllEmpty  = DM.MAGA[(int)EN_MAGA_ID.MAGA02].IsAllEmpty();
				
				bool isTransEmpty     = DM.MAGA[(int)EN_MAGA_ID.TRANS ].IsAllEmpty();
				bool isLoadEmpty      = DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllEmpty();
				bool isPoliEmpty      = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllEmpty();
				bool isClenEmpty      = DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllEmpty();
				bool isToolEmpty      = DM.TOOL.IsPlateEmpty();
				bool isMagaAllEmpty   = isLoadEmpty && isPoliEmpty && isClenEmpty && isToolEmpty && isTransEmpty;

				bool isLoadFinish     = DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllStat((int)EN_PLATE_STAT.ptsFinish);
				bool isTrasferFinish  = DM.MAGA[(int)EN_MAGA_ID.TRANS ].IsAllStat((int)EN_PLATE_STAT.ptsFinish );
				bool isTrasferLoad    = DM.MAGA[(int)EN_MAGA_ID.TRANS ].IsAllStat((int)EN_PLATE_STAT.ptsLoad   );
				bool isTrasferReady   = DM.MAGA[(int)EN_MAGA_ID.TRANS ].IsAllStat((int)EN_PLATE_STAT.ptsReady  );
				bool isLotOpen        = LOT.fn_IsLotOpen(); //SEQ._bRecipeOpen ;
				bool isDrngPoliUtil   = SEQ_SPIND._bDrngUtilLevel || SEQ_POLIS._bDrngUtility; //JUNG/200515/

				PlatePickInfo         = fn_GetPickInfoMaga ();
				PlatePlceInfo         = fn_GetPlaceInfoMaga();

				//Step Condition
				bool isConPickPlate   = PlatePickInfo.bFind && isMagaAllEmpty  && !isDrngPoliUtil; //&& !isLotOpen  ; //Maga -> Trans
				bool isConPlacePlate  = PlatePlceInfo.bFind && isTrasferFinish                   ; //Trans -> Maga 
																							     
				bool isConLoad        =  isLoadEmpty        && isTrasferLoad                     ; //Trans -> Load 
				bool isConUnload      =  isLoadFinish       && isTransEmpty                      ; //Load  -> Trans
													        
				bool isConLotOpen     =  isTrasferReady     && fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS, true); 

				bool isConWait        = false; //!isConStep   && !isConToolOut   && !bYWaitPos;

				//Clear Var.
				m_bDrngLoad       = false;
				m_bDrngUnload     = false;
				m_bDrngWait       = false;
				m_bDrngPickPlate  = false;
				m_bDrngPlacePlate = false;
				m_bDrngLotOpen    = false;

				//Step Clear
				m_nLoadStep       = 0;
				m_nUnloadStep     = 0;
				m_nPickStep       = 0;
				m_nPlaceStep      =	0;
				m_nLotStep        =	0;

				m_sSeqMsg         = string.Empty;

				//Check Sequence Stop
				if ( SEQ._bStop                                          ) return false; 
				if ( EPU.fn_GetHasErr()                                  ) return false; 
				if (!SEQ._bRun && !FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE)) return false; 
				
				//Check Error
				if (!fn_CheckPlateError()                                ) return false;

				//Magazine Check
				if(!PlatePlceInfo.bFind && isTrasferFinish) //Magazine Slot Error
				{
					EPU.fn_SetErr(EN_ERR_LIST.ERR_0490);
					return false; 
				}

                if (!PlatePickInfo.bFind && isMagaAllEmpty && !m_bTESTMODE) //Magazine Empty Error
                {
					//EPU.fn_SetErr(EN_ERR_LIST.ERR_0491);
					SEQ._bBtnWinStop = true;

					m_sShowMsg = "[Magazine Empty]- 작업이 종료 되었습니다.";

					fn_MoveMotr(m_iMotrZId, EN_COMD_ID.Wait1); //JUNG/200922

					return false;
                }


				//
				if (isConPickPlate   ) { m_bDrngPickPlate  = true; m_nSeqStep = 500 ; m_nPickStep   = 10; m_sSeqMsg = "Plate Pick" ; goto __GOTO_CYCLE__; }
				if (isConPlacePlate  ) { m_bDrngPlacePlate = true; m_nSeqStep = 600 ; m_nPlaceStep  = 10; m_sSeqMsg = "Plate Place"; goto __GOTO_CYCLE__; }
				if ( isConLotOpen    ) { m_bDrngLotOpen    = true; m_nSeqStep = 700 ; m_nLotStep    = 10; m_sSeqMsg = "Lot Open"   ; goto __GOTO_CYCLE__; }
				if (isConLoad        ) { m_bDrngLoad       = true; m_nSeqStep = 100 ; m_nLoadStep   = 10; m_sSeqMsg = "Loading"    ; goto __GOTO_CYCLE__; }
				if (isConUnload      ) { m_bDrngUnload     = true; m_nSeqStep = 200 ; m_nUnloadStep = 10; m_sSeqMsg = "Unloading"  ; goto __GOTO_CYCLE__; }
				if (isConWait        ) { m_bDrngWait       = true; m_nSeqStep = 1300;                     m_sSeqMsg = "Wait"       ; goto __GOTO_CYCLE__; }

			}

			//Cycle Start
			__GOTO_CYCLE__:

			//Cycle
			switch (m_nSeqStep)
			{
				case 100:
					if (fn_LoadCycle(ref m_bDrngLoad)) m_nSeqStep = 0;
					return false;

				case 200:
					if (fn_UnloadCycle(ref m_bDrngUnload)) m_nSeqStep = 0;
					return false;

                case 500:
                    if (fn_PickCycle(ref m_bDrngPickPlate)) m_nSeqStep = 0;
                    return false;

                case 600:
                    if (fn_PlaceCycle(ref m_bDrngPlacePlate)) m_nSeqStep = 0;
                    return false;

                case 700:
                    if (fn_LotCycle(ref m_bDrngLotOpen)) m_nSeqStep = 0;
                    return false;


                //Wait Position 
                case 1300:
					//

					m_nSeqStep++;
					return false;

				case 1301:
					//Move Wait Position

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
        private bool fn_LoadCycle(ref bool Flag)
        {//Transfer -> Load Port

			//1) Top/Btm Back && Cover Bwd 
			//2) Cleaning Bath Wait 
			//3) Turn
			//4) Load Down && Load Wait pos
			//5) Top Fwd
			//6) Load Up
			//7) Top Bwd
			
			//Local Var.
			bool r1, r2, r3;

            if (m_nLoadStep < 0) m_nLoadStep = 0;

			switch (m_nLoadStep)
			{
				default:
					m_nLoadStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;
					
					fn_WriteSeqLog("[START] Load Cycle");

					//Clear Align position
					//m_dPreAlignPos = 0 ;
					fn_SetPreAlignPos(0.0);
					SEQ_CLEAN.fn_SetAlignPos(0.0);

					m_tDelayTime.Clear();
					m_nLoadStep++;
                    return false;
				
				case 11:
					r1 = fn_MoveCylTopTR     (ccBwd );
					r2 = fn_MoveCylLoadUpDown(ccDown);
					r3 = fn_MoveCylLoadCover (ccBwd );
					if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nLoadStep ++;
					return false ;

				case 12:
					if(SEQ._bStop)
					{
						m_nLoadStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r2 = fn_MoveMotr(m_iMotrTId, EN_COMD_ID.Wait1);
					if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nLoadStep++;
                    return false;
                
				case 13:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					fn_MoveCylLoadUpDown(ccDown);
                    
					r1 = fn_MoveCylTopTurn(ccDeg180);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nLoadStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylLoadUpDown(ccDown);
					r2 = fn_MoveMotr(m_iMotrTId, EN_COMD_ID.Wait1); //Zero
					r3 = fn_MoveCylTRWait();
                    if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nLoadStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTR(ccFwd);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nLoadStep++;
                    return false;

                case 16:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylLoadUpDown(ccUp);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nLoadStep++;
                    return false;

                case 17:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTR(ccBwd);
                    if (!r1) return false;

					if(fn_IsExistPlatebyMI(EN_MAGA_ID.LOAD, true))
					{
                        //Data Shift
                        DM.ShiftMagaData(EN_MAGA_ID.TRANS, EN_MAGA_ID.LOAD);

                        DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo((int)EN_PLATE_STAT.ptsPreAlign);

                        fn_WriteSeqLog("[END] Load Cycle");

						fn_MoveCylTopTurn(ccDeg0); //Turn 시 간섭 발생으로...

						fn_ReqMoveVisnPos(); //Pre-Align Position

					}
					else
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0350);
					}
                 
					Flag = false; 
					m_nLoadStep=0;
                    return true;
            }


        }
		//---------------------------------------------------------------------------
        private bool fn_UnloadCycle(ref bool Flag)
        {//Load -> Transfer

			/*
			 1) Load Port Up && Wait Pos
			 2) Top/Btm Bwd
			 3) Top Turn 180
			 4) Top Fwd
			 5) Load Down
			 6) Top Bwd
			 7) Top Turn 0
			 */
			
			//Local Var.
			bool r1, r2, r3;

            if (m_nUnloadStep < 0) m_nUnloadStep = 0;

			switch (m_nUnloadStep)
			{
				default:
					m_nUnloadStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;
					
					fn_WriteSeqLog("[START] Unload Cycle");

					m_nUnloadStep++;
                    return false;
				
				case 11:

					SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);

                    r1 = fn_MoveCylTopTR    (ccBwd);
					r2 = fn_MoveCylBtmTR    (ccFwd);
                    r3 = fn_MoveCylLoadCover(ccBwd);
                    if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nUnloadStep++;
					return false ;

                case 12:
                    if (SEQ._bStop)
                    {
						m_nUnloadStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r2 = fn_MoveMotr(m_iMotrTId, EN_COMD_ID.Wait1);
                    r3 = fn_MoveCylLoadUpDown(ccUp);
					if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nUnloadStep++;
                    return false;
                
				case 13:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTurn(ccDeg180);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nUnloadStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylTopTR(ccFwd);
					r2 = fn_MoveCylBtmTR(ccBwd);
					if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nUnloadStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylLoadUpDown(ccDown);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nUnloadStep++;
                    return false;

                case 16:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTR(ccBwd);
                    if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nUnloadStep++;
                    return false;

                case 17:

					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTurn(ccDeg0);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nUnloadStep++;
                    return false;

				case 18:

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					if (fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS, true)) //JUNG/200523
					{
                        //Data Shift
                        DM.ShiftMagaData(EN_MAGA_ID.LOAD, EN_MAGA_ID.TRANS);

                        fn_WriteSeqLog("[END] Unload Cycle");
                    }
                    else
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0351);
					}

					Flag = false;
					m_nUnloadStep = 0;
                    return true;
            }
        }
		//---------------------------------------------------------------------------
        private bool fn_PickCycle(ref bool Flag)
        {//Magazine -> Transfer
			/*
			 * 1) Top/ Btm Bwd
			 * 2) Magazine Move L/R
			 * 3) Ready Slot Check (1pitch Down)
			 * 4) Magazine Move Up + pick Offset
			 * 5) Btm Fwd
			 * 6) X0114 Check
			 * 7) Top Fwd
			 * 8) Magazine Down 
			 * 9) Top/Btm  Bwd
			 
			 * * ** Btm Fwd 상태에서 Magazine move X
			 * 
			 */

			//Local Var.
			bool r1, r2, r3, r4;
			int nFindRow = PlatePickInfo.nFindRow;
			int nFindCol = PlatePickInfo.nFindCol;
			int nMagaId  = PlatePickInfo.nMagaId ; 
			//int nIdex    = 4 + nMagaId ;

			if (m_nPickStep < 0) m_nPickStep = 0;

			switch (m_nPickStep)
			{
				default:
					m_nPickStep = 0;
					Flag = false;
					return true;

				case 10:

					sLogMsg = string.Format($"[START] Pick plate from magazine - {STR_MAGAZINE_NAME[nMagaId]} / Slot : {nFindRow}");
					fn_WriteSeqLog(sLogMsg);

					//Check Magazine Exist
					if(!fn_IsExistMagazine((EN_MAGA_ID)nMagaId, true))
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0495 + (nMagaId- (int)EN_MAGA_ID.MAGA01));
						m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    //JUNG/201118/Check tool exist
                    if (DM.STOR[siPolish].IsAllStat((int)EN_PIN_STAT.psEmpty) && DM.TOOL.IsAllEmpty())
                    {
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0488);
                        m_nPickStep = 0;					
                        Flag = false;
                        return false;
                    }

                    if (DM.STOR[siClean].IsAllStat((int)EN_PIN_STAT.psEmpty) && DM.TOOL.IsAllEmpty())
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0489);
                        m_nPickStep = 0;
                        Flag = false;
                        return false;
                    }


                    //Check tool exist of recipe
                    if (!fn_CheckToolExist(DM.MAGA[nMagaId].GetPlateRecipeName(nFindRow)))
					{
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0497);
                        m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    Flag = true;

					//
					m_sStartTime = DateTime.Now.ToString();
                    m_sEndTime        = "";

                    m_dCycleTimeStart = TICK._GetTickTime();

                    m_nPickStep++;
                    return false;
				
				case 11:
                    if (SEQ._bStop)
                    {
						m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveCylTopTR  (ccBwd);
                    r2 = fn_MoveCylBtmTR  (ccBwd);
					r3 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r4 = true; // SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Wait2);
					if (!r1 || !r2 || !r3 || !r4) return false;

					m_tDelayTime.Clear();
					m_nPickStep++;
					return false ;

                case 12:
                    if (SEQ._bStop)
                    {
                        m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    r1 = fn_MoveCylMaga(nMagaId == (int)EN_MAGA_ID.MAGA01 ? ccBwd : ccFwd);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nPickStep++;
                    return false;

                case 13:
                    if (SEQ._bStop)
                    {
                        m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTurn(ccDeg0);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nPickStep++;
                    return false;

                case 14:
                    if (SEQ._bStop)
                    {
                        m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylBtmTR(ccFwd); //JUNG/200515/Fwd시에만 Sensor 감지
					if (!r1) return false;
                    
					m_tDelayTime.Clear();
                    m_nPickStep++;
                    return false;
				
				case 15:
                    if (SEQ._bStop)
                    {
                        m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

					//한 Pitch 밑에서 확인...
					r1 = fn_MoveMotr    (m_iMotrZId, EN_COMD_ID.FindStepB1, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

					m_tDelayTime.Clear();
                    m_nPickStep++;
                    return false;

                case 16:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

					if(!fn_CheckPlateExist() && !m_bTESTMODE)
					{
						//Error or map Clear
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0492);
                        
						m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    m_tDelayTime.Clear();
                    m_nPickStep++;
                    return false;

                case 17:
                    if (SEQ._bStop)
                    {
                        m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PickInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

					//
					m_dStartFwdPos = GetEncPos_Z(); 

                    m_tDelayTime.Clear();
                    m_nPickStep++;
                    return false;

                case 18:
					r1 = fn_MoveCylBtmTR(ccFwd);
                    if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					//Check Safety Sensor
					if (!fn_CheckMagaPitch() && !m_bTESTMODE)
                    {
                        //Error or map Clear
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0494);

						m_nPickStep = 0;
                        Flag = false;
                        return true;
                    }

                    r2 = fn_MoveCylTopTR(ccFwd);
                    if (!r2) return false;

                    m_tDelayTime.Clear();
					m_nPickStep++;
                    return false;

                case 19:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PlceInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
					m_nPickStep++;
                    return false;

                case 20:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTR(ccBwd); //fn_MoveCylTRWait();
                    if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nPickStep++;
                    return false;

                case 21:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

                    //Save Last Work Position
                    m_dLastPickPos = GetEncPos_Z();
					
					if (fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS, true))
					{
						//Data Shift
						DM.ShiftPlateData (ref DM.MAGA[PlatePickInfo.nMagaId].PLATE[PlatePickInfo.nFindRow, PlatePickInfo.nFindCol], ref DM.MAGA[(int)EN_MAGA_ID.TRANS].PLATE[0, 0]);
						DM.fn_SetPlateData(EN_MAGA_ID.TRANS, PlatePickInfo, m_dLastPickPos);

						sLogMsg = string.Format($"[END] Pick plate from magazine - {STR_MAGAZINE_NAME[nMagaId]} / Pos: {m_dLastPickPos}");
						fn_WriteSeqLog(sLogMsg);
						Console.WriteLine(sLogMsg);

                        //Drain
                        SEQ_POLIS.fn_SetDrain(); //JUNG/200824

                    }
                    else
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0351);
					}
                    
					Flag = false;
					m_nPickStep = 0;
                    return true;
            }
        }
        //---------------------------------------------------------------------------
        private bool fn_PlaceCycle(ref bool Flag)
        {//Transfer -> Magazine
			/*
			 * 1) Top/Btm Bwd
			 * 2) Top Turn 0
			 * 3) Magazine Move L/R
			 * 4) Empty Slot Check(1pitch down)
			 * 5) Magazine move Place Position (Place Offset)
			 * 6) Btm Fwd
			 * 7) X0114 Check
			 * 8) Top Fwd
			 * 9) Magazine move Up
			 * 10) Top/Btm Bwd
			 */

			//Local Var.
			bool r1, r2, r3, r4;
			int nFindRow = PlatePlceInfo.nFindRow;
			int nFindCol = PlatePlceInfo.nFindCol;
			int nMagaId  = PlatePlceInfo.nMagaId ;

			if (m_nPlaceStep < 0) m_nPlaceStep = 0;

			switch (m_nPlaceStep)
			{
				default:
					m_nPlaceStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;
					sLogMsg = string.Format($"[START] Place plate to magazine - {STR_MAGAZINE_NAME[nMagaId]}"); 
					fn_WriteSeqLog(sLogMsg);
                    
					//Check Magazine Exist
                    if (!fn_IsExistMagazine((EN_MAGA_ID)nMagaId, true))
                    {
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0495 + (nMagaId - (int)EN_MAGA_ID.MAGA01));
						m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    m_nPlaceStep++;
                    return false;
				
				case 11:
					if(SEQ._bStop)
					{
                        m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveCylTopTR  (ccBwd);
                    r2 = fn_MoveCylBtmTR  (ccBwd);
					r3 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r4 = true; //SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Wait2);
					if (!r1 || !r2 || !r3 || !r4) return false;

					m_tDelayTime.Clear();
					m_nPlaceStep++;
					return false ;

                case 12:
                    if (SEQ._bStop)
                    {
                        m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylMaga(nMagaId == (int)EN_MAGA_ID.MAGA01 ? ccBwd : ccFwd);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nPlaceStep++;
                    return false;

                case 13:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylTopTurn(ccDeg0);
					if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nPlaceStep++;
                    return false;
                
                case 14:
                    if (SEQ._bStop)
                    {
                        m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylBtmTR(ccFwd);
					if (!r1) return false;
                    m_tDelayTime.Clear();
                    m_nPlaceStep++;
                    return false;
				
				case 15:

                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

					//현재 위치가 Pick 후에 위치랑 동일하면 그대로 진행 --> 차후 확인 후...
					if(fn_CheckPickPos())
					{
						Console.WriteLine("현재 위치가 Pick 후에 위치랑 동일");
						fn_WriteLog("[Transfer] place 위치가 Pick 후에 위치랑 동일");

						m_tDelayTime.Clear();
						m_nPlaceStep = 18;
                        return false;
                    }

                    //1) PlateInfo 위치 확인 후 자재가 없다면 그대로 In
                    //2) 감지되면 Empty Slot 찾아서 In

                    //한 Pitch 밑에서 확인...
                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.FindStepB1, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nPlaceStep++;
                    return false;

                case 16:
                    if (SEQ._bStop)
                    {
                        m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 300)) return false;

					if(fn_CheckPlateExist() && !m_bTESTMODE)
					{
						//Error or map Clear
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0493);

						m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    m_tDelayTime.Clear();
					m_nPlaceStep++;
                    return false;

                case 17:
                    if (SEQ._bStop)
                    {
                        m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PlceInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

					m_dStartFwdPos = GetEncPos_Z();

					m_tDelayTime.Clear();
					m_nPlaceStep++;
                    return false;

                case 18:
                    r1 = fn_MoveCylBtmTR(ccFwd);
                    if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					//Check Safety Sensor
					if (!fn_CheckMagaPitch() && !m_bTESTMODE)
					{
                        //Error or map Clear
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0494);

                        m_nPlaceStep = 0;
                        Flag = false;
                        return true;
                    }

                    r2 = fn_MoveCylTopTR(ccFwd);
                    if (!r2) return false;

					m_tDelayTime.Clear();
					m_nPlaceStep++;
                    return false;

                case 19:
					if (!m_tDelayTime.OnDelay(true, m_bTESTMODE? 3000 : 500)) return false; //Test mode - 3sec 

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PickInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
					m_nPlaceStep++;
                    return false;

                case 20:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTRWait();
                    if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nPlaceStep++;
                    return false;

                case 21:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					if (!fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS, false))
					{
						//Data Shift
						DM.ShiftPlateData(ref DM.MAGA[(int)EN_MAGA_ID.TRANS].PLATE[0, 0], ref DM.MAGA[PlatePlceInfo.nMagaId].PLATE[PlatePlceInfo.nFindRow, PlatePlceInfo.nFindCol]);

                        sLogMsg = string.Format($"Place plate to magazine - {STR_MAGAZINE_NAME[nMagaId]} / Slot : {nFindRow+1}");
                        fn_WriteSeqLog(sLogMsg);
                        Console.WriteLine(sLogMsg);
						fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

						LOT.fn_LotEnd();

					}
                    else
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0356);
					}

					//
					if(m_bTESTMODE)
					{
						if(!DM.MAGA[PlatePlceInfo.nMagaId].IsExistReady())
						{
							DM.MAGA[PlatePlceInfo.nMagaId].SetTo((int)EN_PLATE_STAT.ptsReady);

							DM.MAGA[PlatePlceInfo.nMagaId].SetTo(0, (int)EN_PLATE_STAT.ptsFinish);

							Console.WriteLine("SetTo --> EN_PLATE_STAT.ptsReady");
						}
					}

					//
					m_sEndTime = DateTime.Now.ToString();
					m_dCycleTimeEnd = TICK._GetTickTime() - m_dCycleTimeStart;
                    
					fn_WriteSeqLog(string.Format($"[CycleTimeEnd]_{TICK.ConvTimeTickToStr(m_dCycleTimeEnd)}"));

					//UPH


					Flag = false;
					m_nPlaceStep = 0;
                    return true;
            }

        }
		//---------------------------------------------------------------------------
		private bool fn_CheckPickPos()
		{
			bool rtn = false;

			//
			double dEncPosZ = GetEncPos_Z();

			Console.WriteLine(string.Format($"m_dLastPickPos : {m_dLastPickPos} / GetEncPos_Z : {dEncPosZ}"));

			if (m_dLastPickPos > dEncPosZ)
			{
                if (Math.Abs(m_dLastPickPos - dEncPosZ) < 0.1)
                {
                    rtn = true;
                }
            }
			else
			{
                if (Math.Abs(dEncPosZ - m_dLastPickPos) < 0.1)
                {
                    rtn = true;
                }
            }

            return rtn; 

		}
		//---------------------------------------------------------------------------
		private bool fn_LotCycle(ref bool Flag)
        {
			//Lot Open Cycle
			string sRecipe  = string.Empty;
			string sPlateId = string.Empty;
			int nFromMaga, nFormSlot; 

			if (m_nLotStep < 0) m_nLotStep = 0;

			switch (m_nLotStep)
			{
				default:
					m_nLotStep = 0;
					Flag = false;
					return true;

				case 10:
					Flag = true;

					//
					if (m_bTESTMODE) //if(m_bTESTMODE || FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE))
					{
						LOT.fn_LotOpenTest();
						m_nLotStep++;
                        return false;
                    }

                    sRecipe = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetPlateRecipeName();

					if (sRecipe == "" || sRecipe == string.Empty)
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0500);
                        
						m_nLotStep = 0;
                        Flag = false;
                        return true;

                    }

					//
					sPlateId = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetPlateId();
					if (LOT.fn_LotOpen(sRecipe, sPlateId))
					{
						//libSetLotName
						UserClass.g_VisionManager.fn_SetLotName(LOT._sLotNo);
						nFromMaga = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromMaga();
						nFormSlot = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromRow();

						if (nFromMaga >= 0 && nFromMaga <= 5)
						{
							sLogMsg = string.Format($"MAGA No : {STR_MAGAZINE_NAME[nFromMaga]} / Slot No : {nFormSlot+1}");
							fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
						}
					}
					else
					{
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0499);

                        m_nLotStep = 0;
                        Flag = false;
                        return true;
                    }

                    m_nLotStep++;
                    return false;
                
				case 11:
					
					//Map 
					DM.MAGA[(int)EN_MAGA_ID.TRANS].SetTo((int)EN_PLATE_STAT.ptsLoad);

					fn_WriteLog("LOT Open Cycle OK");

					m_nLotStep = 0;
                    Flag = false;
                    return true;

            }
        }

        //---------------------------------------------------------------------------
        public bool fn_LoadOneCycle()
        {//Transfer -> Load 

			//Local Var.
			bool r1, r2, r3;

            if (m_nManStep < 0) m_nManStep = 0;
			switch (m_nManStep)
			{
				default:
					m_nManStep = 0;
					return true;

				case 10:
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
				
				case 11:
					r1 = fn_MoveCylTopTR    (ccBwd);
					r2 = fn_MoveCylBtmTR    (ccBwd);
					r3 = fn_MoveCylLoadCover(ccBwd);
					if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false ;

				case 12:
					if (!m_tDelayTime.OnDelay(true, 300)) return false; 

					r1 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r2 = true;// SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Wait2);
					if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 13:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					fn_MoveCylLoadUpDown(ccDown);
					fn_MoveMotr(m_iMotrTId, EN_COMD_ID.Wait1); //Zero

					r1 = fn_MoveCylTopTurn(ccDeg180);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylLoadUpDown(ccDown);
					r2 = fn_MoveCylTRWait();
					r3 = fn_MoveMotr(m_iMotrTId, EN_COMD_ID.Wait1); //Zero
                    if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTR(ccFwd);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 16:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylLoadUpDown(ccUp);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 17:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTR(ccBwd);
                    if (!r1) return false;

					fn_MoveCylTopTurn(ccDeg0);

					if (fn_IsExistPlatebyMI(EN_MAGA_ID.LOAD))
					{
						fn_UserMsg("Load Cycle Done");
					}
					else
					{
						fn_UserMsg("Load Cycle Error!!!!");
					}

					m_nManStep = 0;
                    return true;
            }
            
        }
        //---------------------------------------------------------------------------
        public bool fn_UnloadOneCycle()
		{//Load -> Transfer 
			//Local Var.
			bool r1, r2, r3;

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
                    r1 = fn_MoveCylTopTR    (ccBwd);
                    r2 = fn_MoveCylBtmTR    (ccFwd);
                    r3 = fn_MoveCylLoadCover(ccBwd);
                    if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false ;

                case 12:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r2 = fn_MoveMotr(m_iMotrTId, EN_COMD_ID.Wait1);
					r3 = fn_MoveCylLoadUpDown(ccUp);
					if (!r1 || !r2 || !r3) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;
                
				case 13:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTurn(ccDeg180);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylTopTR(ccFwd);
					r2 = fn_MoveCylBtmTR(ccBwd);
					if (!r1 || !r2) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 15:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylLoadUpDown(ccDown);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 16:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTR(ccBwd);
                    if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 17:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;
                    
					r1 = fn_MoveCylTopTurn(ccDeg0);
					if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

				case 18:
					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					if (fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS))
					{
						fn_UserMsg("Unload Cycle Done");
					}
					else
					{
						fn_UserMsg("Unload Cycle Error!!!");
					}

					m_nManStep = 0;
                    return true;
            }
        }
        //---------------------------------------------------------------------------
        public bool fn_PickOneCycle(int maga = 0, int row = 0)
        { //Magazine -> Transfer

			//Local Var.
			bool r1, r2, r3, r4;
			int nFindRow = row < 0? 0 : row;
			int nMagaId  = maga < 0 ? 0 : (maga > 1? 1 : maga);

			//nMagaId = 1; 
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
                    r1 = fn_MoveCylTopTR  (ccBwd);
                    r2 = fn_MoveCylBtmTR  (ccBwd);
					r3 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r4 = true; // SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Wait2);
					if (!r1 || !r2 || !r3 || !r4) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false ;

                case 12:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    r1 = fn_MoveCylMaga(nMagaId);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                case 13:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTopTurn(ccDeg0);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                
				case 14:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylBtmTR(ccFwd);
					if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

                    
				case 15:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					//한 Pitch 밑에서 확인...
					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.FindStepB1, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 16:
                    if (!m_tDelayTime.OnDelay(true, 500)) return false;

					if(!fn_CheckPlateExist())
					{
						Console.WriteLine("fn_CheckPlateExist() = false");
						fn_UserMsg("fn_CheckPlateExist() = false [X0113]");

						// m_nManStep = 0;
                        // return false;
                    }
					//else
					//{
					//	Console.WriteLine("fn_CheckPlateExist() = true");
					//}

                    m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 17:

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PickInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 18:
					r1 = fn_MoveCylBtmTR(ccFwd);
                    if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					//Check Safety Sensor
					if (!fn_CheckMagaPitch())
                    {
                        //Error or map Clear
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0494);

						m_nManStep = 0;
                        return true;
                    }

                    r2 = fn_MoveCylTopTR(ccFwd);
                    if (!r2) return false;

                    m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 19:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PlceInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 20:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTRWait();
                    if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 21:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

                    //Save Last Work Position
                    m_dLastPickPos = GetEncPos_Z();

					if (fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS, true))
					{
						sLogMsg = string.Format($"[END] Pick plate from magazine - {STR_MAGAZINE_NAME[4 + nMagaId]} / Pos: {m_dLastPickPos} / Row : {nFindRow}");
					}
					else
					{
						sLogMsg = string.Format($"[END] Pick plate Error!!! - {STR_MAGAZINE_NAME[4 + nMagaId]} / Pos: {m_dLastPickPos} / Row : {nFindRow}");
					}
					fn_UserMsg(sLogMsg);

					m_nManStep = 0;
                    return true;
            }
        }
		//---------------------------------------------------------------------------
		public bool fn_PlaceOneCycle(int maga = -1 , int row = -1)
		{//Transfer -> Magazine 
		
			//Local Var.
			bool r1, r2, r3, r4;
			int nFindRow = row  < 0 ? 0 : row ;
			int nMagaId  = maga < 0 ? 0 : (maga > 1 ? 1 : maga);
			//nMagaId = 1;

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
                    r1 = fn_MoveCylTopTR  (ccBwd);
                    r2 = fn_MoveCylBtmTR  (ccBwd);
					r3 = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y, EN_COMD_ID.Wait2);
					r4 = true; // SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Wait2);
					if (!r1 || !r2 || !r3 || !r4) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
					return false ;

                case 12:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

                    r1 = fn_MoveCylMaga(nMagaId);
                    if (!r1) return false;
                    
					m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;

				case 13:

					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylTopTurn(ccDeg0);
                    if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 14:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					r1 = fn_MoveCylBtmTR(ccFwd);
					if (!r1) return false;

                    m_tDelayTime.Clear();
                    m_nManStep++;
                    return false;


                case 15:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

					//한 Pitch 밑에서 확인...
					r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.FindStepB1, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
					if (!r1) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 16:
                    if (!m_tDelayTime.OnDelay(true, 100)) return false;

					if(fn_CheckPlateExist())
					{
						//Error or map Clear
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0493);

						m_nManStep = 0;
                        return true;
                    }

                    m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 17:

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PlceInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;
                    
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 18:
                    r1 = fn_MoveCylBtmTR(ccFwd);
                    if (!r1) return false;

					if (!m_tDelayTime.OnDelay(true, 1000)) return false;

					//Check Safety Sensor
					if (!fn_CheckMagaPitch())
					{
                        //Error or map Clear
                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0494);

						m_nManStep = 0;
                        return true;
                    }

                    r2 = fn_MoveCylTopTR(ccFwd);
                    if (!r2) return false;

					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 19:
					if (!m_tDelayTime.OnDelay(true, 500)) return false;

                    r1 = fn_MoveMotr(m_iMotrZId, EN_COMD_ID.PickInc, EN_MOTR_VEL.Normal, nFindRow, EN_FPOSN_INDEX.Index1);
                    if (!r1) return false;

                    m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 20:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					r1 = fn_MoveCylTRWait();
                    if (!r1) return false;
					
					m_tDelayTime.Clear();
					m_nManStep++;
                    return false;

                case 21:
					if (!m_tDelayTime.OnDelay(true, 300)) return false;

					if(!fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS, false))
					{
						sLogMsg = string.Format($"[END] Place plate to magazine - {STR_MAGAZINE_NAME[4 + nMagaId]}");
					}
					else
					{
						sLogMsg = string.Format($"[END] Place Error!!!! - {STR_MAGAZINE_NAME[4 + nMagaId]} / Row : {nFindRow}");
					}
					
					fn_UserMsg(sLogMsg);

					m_nManStep = 0;
                    return true;
            }
        }
		//---------------------------------------------------------------------------
		private bool fn_CheckToolExist(string recipe)
		{//Check Polishing Tool of tool storage

			bool rtn      = true;
			int nToolKind = 0;

			if (m_bTESTMODE) return true; 

			g_VisionManager._RecipeManager.fn_ReadRecipe("", recipe);

			int nStep     = g_VisionManager._RecipeModel.Model[0].MillingCount; 
		
			for (int n = 0; n< nStep; n++)
			{
				if (g_VisionManager._RecipeModel.Model[0].Milling[n].ToolChange != 1) continue;  //JUNG/200918

				nToolKind = g_VisionManager._RecipeModel.Model[0].Milling[n].ToolType;

				if (!DM.STOR[siPolish].IsExtPolishingPin(nToolKind)) return false;
			}

			return rtn; 
		}

		//---------------------------------------------------------------------------
		public bool fn_CheckPlateExist()
		{//Magazine Plate Exist Check

			return IO.XV[(int)EN_INPUT_ID.xTRS_MagaPlateExtChk]; 
		}
		//---------------------------------------------------------------------------
        public bool fn_CheckMagaPitch()
        {//Magazine Plate Exist Check

            return IO.XV[(int)EN_INPUT_ID.xTRS_MagaPitchChk];
        }
        //---------------------------------------------------------------------------
        private bool fn_CheckPlateError()
		{
			//
			bool bOk = true;

			if (m_bTESTMODE) return bOk;

			bool bExtLoad   = !DM.MAGA[(int)EN_MAGA_ID.LOAD ].IsAllEmpty();
			bool bExtTrans  = !DM.MAGA[(int)EN_MAGA_ID.TRANS].IsAllEmpty();

			bool xExtLoad   = fn_IsExistPlatebyMI(EN_MAGA_ID.LOAD , bExtLoad );
			bool xExtTrans  = fn_IsExistPlatebyMI(EN_MAGA_ID.TRANS, bExtTrans);
			
			bool bDrngPP    = SEQ_SPIND._bDrngPlatePick || SEQ_SPIND._bDrngPlatePlce; //JUNG/200528/
			bool bDrngAlign = SEQ_SPIND._bDrngVisnInspL;

			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0350, !xExtLoad  && bExtLoad  && !bDrngPP && !bDrngAlign)) bOk = false; 
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0355,  xExtLoad  && !bExtLoad && !bDrngPP && !bDrngAlign)) bOk = false; 

			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0351, !xExtTrans &&  bExtTrans)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0356,  xExtTrans && !bExtTrans)) bOk = false;

			bool isTrasferLoad = DM.MAGA[(int)EN_MAGA_ID.TRANS].IsAllStat((int)EN_PLATE_STAT.ptsLoad);
			//Check Recipe Name
			if (xExtTrans && isTrasferLoad)
			{
				string sPlateRcp = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetPlateRecipeName(); //
				if(sPlateRcp != FM._sRecipeName) //
				{
					if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0370, FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE))) bOk = false;
				}
			}

			return bOk; 
		}
		//---------------------------------------------------------------------------
		public bool fn_MoveCylTRWait()
		{
			bool r1 = fn_MoveCylTopTR(ccBwd);
			bool r2 = fn_MoveCylBtmTR(ccBwd);

			return ( r1 && r2) ;
        }
		//---------------------------------------------------------------------------
        public bool fn_MoveCylTRTurnPos()
        {
            bool r1 = fn_MoveCylTopTR(ccBwd);
            bool r2 = fn_MoveCylBtmTR(ccFwd);

            return (r1 && r2);
        }
        //---------------------------------------------------------------------------
        public bool fn_MoveCylTopTR(int act)
		{
			//Turn 180 인 경우에 Cover 간섭
			if (!IO.XV[(int)EN_INPUT_ID.xTRS_TopTR0] && act == ccFwd)
			{
				if (!IO.XV[(int)EN_INPUT_ID.xTRS_LoadCoverBwd]) return false; 
			}
            
			//Turn 0 인 경우에 Bottom 먼저
            if (act == ccFwd && !IO.XV[(int)EN_INPUT_ID.xTRS_TopTR180])
            {
                if (!IO.XV[(int)EN_INPUT_ID.xTRS_BtmTRFwd]) return false;
            }


            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aTran_TopLoadFB, act);
            return r1;

        }
		//---------------------------------------------------------------------------
        public bool fn_MoveCylTopTurn(int act)
        {
			//Check Cleaning Bath disturb position
			double dMinYAxisPos = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_CLEANY_POS_MOVE_LOADTURN].dPos;
			double dEnc_CLEANY  = MOTR.GetEncPos(EN_MOTR_ID.miCLN_Y);

			//JUNG/200515/
			if(act == ccDeg0)
			{
				if (IO.XV[(int)EN_INPUT_ID.xTRS_TopTR0]) return true; 
			}
            if (act == ccDeg180)

            {
                if (IO.XV[(int)EN_INPUT_ID.xTRS_TopTR180]) return true;
            }

			
			if (dEnc_CLEANY > dMinYAxisPos) //JUNG/200506
			{
				return false;
			}

			if (!fn_MoveCylTRTurnPos()) return false; //JUNG/200515/Bottom TR Fwd Position

            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aTran_TopLoadTurn, act);
            return r1;

        }
		//---------------------------------------------------------------------------
        public bool fn_MoveCylBtmTR(int act)
        {
			//Check Magazine Left/Right
			if(act == ccFwd)
			{
				if (!IO.XV[(int)EN_INPUT_ID.xTRS_MagaLeftPos] && !IO.XV[(int)EN_INPUT_ID.xTRS_MagaRightPos]) return false; 
				//if (fn_IsExistMagazine(EN_MAGA_ID.MAGA01, false) && !IO.XV[(int)EN_INPUT_ID.xTRS_MagaRightPos])
				//{
				//	return false; 
				//}
				//else if (fn_IsExistMagazine(EN_MAGA_ID.MAGA02, false) && )
				//{
				//	return false;
				//}
			}

			bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aTran_BtmLoadFB, act);
            return r1;

        }
        //---------------------------------------------------------------------------
        public bool fn_MoveCylLoadUpDown(int act)
        {
            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aTran_LoadPortUD, act);
            return r1;

        }
        //---------------------------------------------------------------------------
        public bool fn_MoveCylLoadCover(int act)
        {
			//Map 상태 확인 후 Fwd.
			if(!DM.MAGA[(int)EN_MAGA_ID.LOAD].IsAllEmpty())
			{
				if (act == ccFwd)
				{
					return false; 
				}
			}

            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aTran_LoadCover, act);
            return r1;

        }
		//---------------------------------------------------------------------------
		public bool fn_MoveCylMaga(int act)
		{
			//Transfer Bwd 아니면 이동 불가
			if (!IO.XV[(int)EN_INPUT_ID.xTRS_BtmTRBwd]) return false;
			if (!IO.XV[(int)EN_INPUT_ID.xTRS_TopTRBwd]) return false;

			bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aTran_MagaMoveLR, act);
			return r1;
		}
		//---------------------------------------------------------------------------
		public bool fn_MoveCylDoor(int act)
		{
			//bool isNoRun   = SEQ.fn_IsNoRun() && m_nManStep == 0;
			//bool xArmCheck = fn_IsArmChk();
			//bool isDrngPIO = m_bDrngLoad || m_bDrngUnload; 
			//bool bLockCon  = xArmCheck   || isDrngPIO;
			//
			////Arm Check
			//if (bLockCon && (act == ccFwd))
			//{
			//	if (isNoRun) fn_UserMsg(string.Format("Check Arm Detect Sensor."));
			//	return false; 
			//}
			//
			//bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aTRAS_Door, act);
			//return r1;

			return true; 

        }
		//-------------------------------------------------------------------------------------------------
		public bool fn_IsExistMagazine(EN_MAGA_ID maga, bool userset)
		{
            bool bAutoMode = FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
            bool bExist    = false;

            bool xMaga01Exist = !IO.XV[(int)EN_INPUT_ID.xTRS_MagaLeftExist ]; //
			bool xMaga02Exist = !IO.XV[(int)EN_INPUT_ID.xTRS_MagaRightExist]; //

			switch (maga)
			{
				case EN_MAGA_ID.MAGA01:
					bExist = xMaga01Exist; 
					break;
				case EN_MAGA_ID.MAGA02:
					bExist = xMaga02Exist;
					break;
				default:
					break;
			}
            
			if (!bAutoMode && FM.m_stMasterOpt.nMagaSkip == 1)
            {
                bExist = userset;
            }

            return bExist;



        }
        //---------------------------------------------------------------------------
        private bool fn_IsExistPlatebyMI (EN_MAGA_ID where, bool userset = false)
		{
			bool bAutoMode = FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE);
			bool bExist    = false;

			switch (where)
			{
				
				case EN_MAGA_ID.TRANS   : bExist = IO.XV[(int)EN_INPUT_ID.xTRS_TransPlateExist]; break;
				case EN_MAGA_ID.SPINDLE : bExist = IO.XV[(int)EN_INPUT_ID.xSPD_PlateExistChk  ]; break;
			    case EN_MAGA_ID.LOAD    : bExist = IO.XV[(int)EN_INPUT_ID.xTRS_LoadPlateExist ]; break;
				//case EN_MAGA_ID.LOAD    : bExist = !DM.MAGA[(int)EN_MAGA_ID.LOAD].IsAllEmpty();  break; //JUNG/200527/임시로...

				case EN_MAGA_ID.MAGA01  : bExist = !IO.XV[(int)EN_INPUT_ID.xTRS_MagaLeftExist ]; break;
				case EN_MAGA_ID.MAGA02  : bExist = !IO.XV[(int)EN_INPUT_ID.xTRS_MagaRightExist]; break;
			}

			if (!bAutoMode && FM.m_stMasterOpt.nPlateSkip == 1)
			{
				bExist = userset;
			}

			return bExist;
		}

        //---------------------------------------------------------------------------
        private void fn_WriteSeqLog(string log)
        {
            fn_WriteLog(log, EN_LOG_TYPE.ltEvent, EN_SEQ_ID.TRANSFER);
        }
		//---------------------------------------------------------------------------
		private void fn_InitPlateInfo(ref PLATE_MOVE_INFO info)
		{
			info.bFind     = false;
			info.nBtmPlate = -1;
			info.nFindMode = -1;
			info.nRcpNo    = -1;
			info.dXpos     = 0.0;
			info.dYpos     = 0.0;
		}


		//---------------------------------------------------------------------------
		public PLATE_MOVE_INFO fn_GetPickInfoMaga()
		{
			PLATE_MOVE_INFO stMoveInfo = new PLATE_MOVE_INFO(false);
			
			for (int n = (int)EN_MAGA_ID.MAGA01; n <= (int)EN_MAGA_ID.MAGA02; n++)
			{
				stMoveInfo.bFind = DM.MAGA[n].FindLeftTopDown((int)EN_PLATE_FIND_MODE.fmpReady, ref stMoveInfo.nFindRow, ref stMoveInfo.nFindCol, ref stMoveInfo.nMagaId);
				if (stMoveInfo.bFind)
				{
					return stMoveInfo; 
				}
			}

			return stMoveInfo; 

		}
		//---------------------------------------------------------------------------
		public PLATE_MOVE_INFO fn_GetPlaceInfoMaga()
		{

			bool bUseSamePos = true;

			PLATE_MOVE_INFO stMoveInfo = new PLATE_MOVE_INFO(false);
			bool isTrasferFinish = DM.MAGA[(int)EN_MAGA_ID.TRANS].IsAllStat((int)EN_PLATE_STAT.ptsFinish);


			//Pick Position Check
			if (bUseSamePos)
            {
				if (!isTrasferFinish) return stMoveInfo; 

				int nRow  = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromRow ();
				int nCol  = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromCol ();
				int nMaga = DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromMaga();

				if (nRow >= 0 && nCol >= 0 && nMaga >= 0)
				{
					if (DM.MAGA[nMaga].IsStat(nRow, nCol, (int)EN_PLATE_STAT.ptsEmpty))
					{
						stMoveInfo.bFind    = true;
						stMoveInfo.nFindRow = nRow;
						stMoveInfo.nFindCol = nCol;
						stMoveInfo.nMagaId  = nMaga;

						return stMoveInfo;
					}
					else
					{
						//return stMoveInfo;
						string stemp = string.Format($"Search new place position [ plate form = R:{nRow}, C:{nCol}, Maga : {nMaga} ]");
						Console.WriteLine(stemp);
					}
				}
			}

            for (int n = (int)EN_MAGA_ID.MAGA01; n <= (int)EN_MAGA_ID.MAGA02; n++)
			{
				stMoveInfo.bFind = DM.MAGA[n].FindLeftTopDown((int)EN_PLATE_FIND_MODE.fmpEmpty, ref stMoveInfo.nFindRow, ref stMoveInfo.nFindCol, ref stMoveInfo.nMagaId);

				if (stMoveInfo.bFind)
				{
					return stMoveInfo;
				}

			}

			return stMoveInfo; 

		}
		//---------------------------------------------------------------------------
		public void fn_SetPreAlignPos(double pos, bool polish = false)
		{
			//
			MOTR[(int)m_iMotrTId].MP.dPosn[(int)EN_POSN_ID.CalPos] = pos;

			m_dPreAlignPos = pos;

			Console.WriteLine("Set Transfer T-Align pos : " + pos);

		}
		//---------------------------------------------------------------------------
		public void fn_SaveLog(ref string Msg)
        {
			string sTemp = string.Empty;

			Msg = string.Empty;

			//
			Msg += "[SeqTransfer]\r\n"; 
			Msg += sTemp = string.Format($"m_bToStart        = {m_bToStart       }\r\n");
			Msg += sTemp = string.Format($"m_bToStop         = {m_bToStop        }\r\n");
			Msg += sTemp = string.Format($"m_bDrngLoad       = {m_bDrngLoad      }\r\n");
			Msg += sTemp = string.Format($"m_bDrngUnload     = {m_bDrngUnload    }\r\n");
			Msg += sTemp = string.Format($"m_bDrngWait       = {m_bDrngWait      }\r\n");
			Msg += sTemp = string.Format($"m_bDrngPickPlate  = {m_bDrngPickPlate }\r\n");
			Msg += sTemp = string.Format($"m_bDrngPlacePlate = {m_bDrngPlacePlate}\r\n");
			Msg += sTemp = string.Format($"m_bDrngLotOpen    = {m_bDrngLotOpen   }\r\n");
			Msg += sTemp = string.Format($"m_nSeqStep        = {m_nSeqStep       }\r\n");
			Msg += sTemp = string.Format($"m_nManStep        = {m_nManStep       }\r\n");
			Msg += sTemp = string.Format($"m_nHomeStep       = {m_nHomeStep      }\r\n");
			Msg += sTemp = string.Format($"m_nLoadStep       = {m_nLoadStep      }\r\n");
			Msg += sTemp = string.Format($"m_nUnloadStep     = {m_nUnloadStep    }\r\n");
			Msg += sTemp = string.Format($"m_nPickStep       = {m_nPickStep      }\r\n");
			Msg += sTemp = string.Format($"m_nPlaceStep      = {m_nPlaceStep     }\r\n");
			Msg += sTemp = string.Format($"m_nLotStep        = {m_nLotStep       }\r\n");
			Msg += sTemp = string.Format($"m_dPreAlignPos    = {m_dPreAlignPos   }\r\n");
			Msg += sTemp = string.Format($"m_dLastPickPos    = {m_dLastPickPos   }\r\n");
			

		}

        //---------------------------------------------------------------------------
        public void fn_UpdateFlag(ref Grid grd)
        {
			if (grd == null) return;
            
			Label[,] Items = new Label[30, 2];
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

            Items[nRow, 0].Content = "bToStart       ";  Items[nRow++, 1].Content = string.Format($"{m_bToStart       }");
			Items[nRow, 0].Content = "bToStop        ";  Items[nRow++, 1].Content = string.Format($"{m_bToStop        }");
			Items[nRow, 0].Content = "_";			     Items[nRow++, 1].Content = string.Format($"");			
			Items[nRow, 0].Content = "bDrngLoad      ";  Items[nRow++, 1].Content = string.Format($"{m_bDrngLoad      }");
            Items[nRow, 0].Content = "bDrngUnload    ";  Items[nRow++, 1].Content = string.Format($"{m_bDrngUnload    }");
			Items[nRow, 0].Content = "bDrngWait      ";  Items[nRow++, 1].Content = string.Format($"{m_bDrngWait      }");
			Items[nRow, 0].Content = "bDrngPickPlate" ;  Items[nRow++, 1].Content = string.Format($"{m_bDrngPickPlate }");
            Items[nRow, 0].Content = "bDrngPlacePlate";  Items[nRow++, 1].Content = string.Format($"{m_bDrngPlacePlate}");
            Items[nRow, 0].Content = "bDrngLotOpen"   ;  Items[nRow++, 1].Content = string.Format($"{m_bDrngLotOpen   }");

            Items[nRow, 0].Content = "";				 Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nSeqStep"       ;  Items[nRow++, 1].Content = string.Format($"{m_nSeqStep       }");
			Items[nRow, 0].Content = "nManStep"       ;  Items[nRow++, 1].Content = string.Format($"{m_nManStep       }");
			Items[nRow, 0].Content = "nHomeStep"      ;  Items[nRow++, 1].Content = string.Format($"{m_nHomeStep      }");
			Items[nRow, 0].Content = "_";				 Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nLoadStep  "    ;  Items[nRow++, 1].Content = string.Format($"{m_nLoadStep      }");
			Items[nRow, 0].Content = "nUnloadStep"    ;  Items[nRow++, 1].Content = string.Format($"{m_nUnloadStep    }");
			Items[nRow, 0].Content = "nPickStep"      ;  Items[nRow++, 1].Content = string.Format($"{m_nPickStep      }");
			Items[nRow, 0].Content = "nPlaceStep"     ;  Items[nRow++, 1].Content = string.Format($"{m_nPlaceStep     }");
			Items[nRow, 0].Content = "nLotStep"       ;  Items[nRow++, 1].Content = string.Format($"{m_nLotStep       }");
			Items[nRow, 0].Content = "";				 Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "dPreAlignPos"   ;  Items[nRow++, 1].Content = string.Format($"{m_dPreAlignPos   }");
			Items[nRow, 0].Content = "dLastPickPos"   ;  Items[nRow++, 1].Content = string.Format($"{m_dLastPickPos   }");
			Items[nRow, 0].Content = "";				 Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "Seq Msg"        ;  Items[nRow++, 1].Content = string.Format($"{m_sSeqMsg        }");

			//
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

			Label[,] Items = new Label[30,2];
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

			Items[nRow, 0].Content = "iSeqStep"        ; Items[nRow++, 1].Content = string.Format($"{m_nSeqStep       }");
			Items[nRow, 0].Content = "iManStep"        ; Items[nRow++, 1].Content = string.Format($"{m_nManStep       }");
			Items[nRow, 0].Content = "iHomeStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nHomeStep      }");
			Items[nRow, 0].Content = "_";                Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nLoadStep  "     ; Items[nRow++, 1].Content = string.Format($"{m_nLoadStep      }");
			Items[nRow, 0].Content = "nUnloadStep"     ; Items[nRow++, 1].Content = string.Format($"{m_nUnloadStep    }");
			Items[nRow, 0].Content = "nPickStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nPickStep      }");
			Items[nRow, 0].Content = "nPlaceStep"      ; Items[nRow++, 1].Content = string.Format($"{m_nPlaceStep     }");
			Items[nRow, 0].Content = "nLotStep"        ; Items[nRow++, 1].Content = string.Format($"{m_nLotStep       }");
			Items[nRow, 0].Content = "_";                Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "bDrngLoad"       ; Items[nRow++, 1].Content = string.Format($"{m_bDrngLoad      }");
            Items[nRow, 0].Content = "bDrngUnload"     ; Items[nRow++, 1].Content = string.Format($"{m_bDrngUnload    }");
			Items[nRow, 0].Content = "bDrngWait"       ; Items[nRow++, 1].Content = string.Format($"{m_bDrngWait      }");
			Items[nRow, 0].Content = "bDrngPickPlate"  ; Items[nRow++, 1].Content = string.Format($"{m_bDrngPickPlate }");
            Items[nRow, 0].Content = "bDrngPlacePlate" ; Items[nRow++, 1].Content = string.Format($"{m_bDrngPlacePlate}");
            Items[nRow, 0].Content = "bDrngLotOpen"    ; Items[nRow++, 1].Content = string.Format($"{m_bDrngLotOpen   }");
			Items[nRow, 0].Content = "dPreAlignPos"    ; Items[nRow++, 1].Content = string.Format($"{m_dPreAlignPos   }");
			Items[nRow, 0].Content = "dLastPickPos"    ; Items[nRow++, 1].Content = string.Format($"{m_dLastPickPos   }");
			Items[nRow, 0].Content = "";				 Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "Seq Msg"         ; Items[nRow++, 1].Content = string.Format($"{m_sSeqMsg        }");

			//
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

				m_nSpare1       = br.ReadInt32  ();
                m_nSpare2       = br.ReadInt32  ();
                m_nSpare3       = br.ReadInt32  ();
                m_nSpare4       = br.ReadInt32  ();
                m_nSpare5       = br.ReadInt32  ();

				//			    
				m_bSpare1       = br.ReadBoolean();
                m_bSpare2       = br.ReadBoolean();
                m_bSpare3       = br.ReadBoolean();
                m_bSpare4       = br.ReadBoolean();
                m_bSpare5       = br.ReadBoolean();

				//
				m_dPreAlignPos  = br.ReadDouble ();
				m_dZPos         = br.ReadDouble ();
				m_dLastPickPos  = br.ReadDouble ();
				m_dSpare1       = br.ReadDouble ();
                m_dSpare2       = br.ReadDouble ();
                m_dSpare3       = br.ReadDouble ();
                m_dSpare4       = br.ReadDouble ();
                m_dSpare5       = br.ReadDouble ();

            }
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);
				
				bw.Write(m_nSpare1     ); //Spare
                bw.Write(m_nSpare2     );
                bw.Write(m_nSpare3     );
                bw.Write(m_nSpare4     );
                bw.Write(m_nSpare5     );
									   
                //					   
                bw.Write(m_bSpare1     ); //Spare
                bw.Write(m_bSpare2     );
                bw.Write(m_bSpare3     );
                bw.Write(m_bSpare4     );
                bw.Write(m_bSpare5     );

				//
				bw.Write(m_dPreAlignPos); //
				bw.Write(m_dZPos       ); //
				bw.Write(m_dLastPickPos); //

				bw.Write(m_dSpare1     ); //Spare
                bw.Write(m_dSpare2     );
                bw.Write(m_dSpare3     );
                bw.Write(m_dSpare4     );
                bw.Write(m_dSpare5     );
                bw.Flush();			   

            }
        }





    }
}
