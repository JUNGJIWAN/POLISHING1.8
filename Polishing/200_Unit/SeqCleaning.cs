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
using static WaferPolishingSystem.BaseUnit.ERRID;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using static WaferPolishingSystem.BaseUnit.IOMap;
using WaferPolishingSystem.BaseUnit;
using static WaferPolishingSystem.Define.UserClass;

namespace WaferPolishingSystem.Unit
{
    public class SeqCleaning
    {
        //Timer
        TOnDelayTimer m_tMainCycle      = new TOnDelayTimer();
        TOnDelayTimer m_tToStart        = new TOnDelayTimer();
        TOnDelayTimer m_tToStop         = new TOnDelayTimer();

        TOnDelayTimer m_tCleanCycle     = new TOnDelayTimer();
        TOnDelayTimer m_tInspectCycle   = new TOnDelayTimer();
        TOnDelayTimer m_tUtilityCycle   = new TOnDelayTimer();
		TOnDelayTimer m_tHomeCycle      = new TOnDelayTimer();
		
		TOnDelayTimer m_tDelayTimer     = new TOnDelayTimer();
		TOnDelayTimer m_tDrainTime      = new TOnDelayTimer();
		

		TOnDelayTimer m_tHome           = new TOnDelayTimer();

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Vars.
		bool          m_bToStart        ; //To... Flag.
		bool          m_bToStop         ;
		//bool          m_bWorkEnd        ;
								    
		bool          m_bDrngInspect    ; //Inspect Check
		bool          m_bDrngCleaning   ; //Cleaning Check
		bool          m_bDrngUtility    ; //Utility Check
		bool          m_bDrngWait       ; //Wait 
		bool          m_bDrngSeqDrain   ; 
		bool          m_bRotatewithPos  ; //Rotation Method
		bool          m_bTESTMODE       ; //TEST Mode Flag
		bool          m_bDrngRotateHome ;
		bool          m_bReqHomeCycle   ; //Request Home 
								    
		int           m_nSeqStep        ; //Step.
		int           m_nManStep        ;
		int           m_nHomeStep       ;
		
		int           m_nHomCycleStep   ;
		int           m_nInspectStep    ; //Inspect Step
		int           m_nCleaningStep   ; //Cleaning Step
		int           m_nUtilityStep    ; //Utility Step
		int           m_nReqDrainStep   ; //Req Drain

		double        m_dDirectPosn     ; //Direct Moving Position.
		//double        m_dMPos           ; //Command Motor Position
		//double        m_dEPos           ; //Enc. Motor Position
		double        m_dYPos           ;
		double        m_dAlignPosTH     ; //Align Theta Position for Cleaning

		//
		string        sTemp, sLogMsg    ; //for Log
		string        m_sSeqMsg     = string.Empty; 

		int           m_nPartId   ;
		EN_MOTR_ID    m_iMotrYId  ;
		EN_MOTR_ID    m_iMotrTHId ;

		bool   m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        int    m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;
        double m_dSpare1, m_dSpare2, m_dSpare3, m_dSpare4, m_dSpare5;

		
		//---------------------------------------------------------------------------
		//Property
		public int _nManStep       { get { return m_nManStep      ; } set { m_nManStep       = value; } }
		public int _nHomeStep      { get { return m_nHomeStep     ; } set { m_nHomeStep      = value; } }
		public int _nSeqStep       { get { return m_nSeqStep;       } }
		public bool _bDrngInspect  { get { return m_bDrngInspect  ; } }
        public bool _bDrngCleaning { get { return m_bDrngCleaning ; } }
        public bool _bDrngUtility  { get { return m_bDrngUtility  ; } }
        public bool _bDrngWait     { get { return m_bDrngWait     ; } }
		public bool _bDrngSeqDrain { get { return m_bDrngSeqDrain ; } }
		public bool _bDrngRotateHome { get { return m_bDrngRotateHome; } }
		

		/************************************************************************/
		/* 생성자.                                                               */
		/************************************************************************/
		public SeqCleaning()
        {

            //
            Init();

            //
            m_nPartId   = (int)EN_SEQ_ID.CLEAN;
					    
            m_iMotrYId  = EN_MOTR_ID.miCLN_Y ;
            m_iMotrTHId = EN_MOTR_ID.miCLN_R ;

			m_dAlignPosTH = 0.0;


		}
		//---------------------------------------------------------------------------
        private void Init()
        {
			m_bToStart        = false;
			m_bToStop         = false;
			//m_bWorkEnd        = false;
							  
			m_bDrngInspect    = false;
			m_bDrngCleaning   = false;
			m_bDrngUtility    = false;
			m_bDrngWait       = false;
			m_bDrngSeqDrain   = false;
			m_bReqHomeCycle   = false; 

			m_bRotatewithPos  = false; //Method

			sTemp             = string.Empty; 
			sLogMsg           = string.Empty;
							  
			m_nSeqStep        = 0;
			m_nManStep        = 0;
			m_nManStep        = 0;
			m_nHomeStep       = 0;
							  
			m_nInspectStep    = 0;
			m_nCleaningStep   = 0;
			m_nUtilityStep    = 0;
			m_nHomCycleStep   = 0;
							  
			m_dDirectPosn     = 0;
			//m_dMPos           = 0;
			//m_dEPos           = 0;
			m_dYPos           = 0;
							  
			m_nReqDrainStep   = 0;

			//Timer Clear
			m_tMainCycle   .Clear();
			m_tToStart     .Clear();
			m_tToStop      .Clear();
			m_tCleanCycle  .Clear();
			m_tInspectCycle.Clear();
			m_tUtilityCycle.Clear();
			m_tHomeCycle   .Clear();

			m_tHome        .Clear();

			m_tDelayTimer  .Clear();

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
			m_bTESTMODE       = false;
			m_bDrngRotateHome = false;

			m_bDrngInspect    = false;
			m_bDrngCleaning   = false;
			m_bDrngUtility    = false;
			m_bDrngWait       = false;


			m_nSeqStep        = 0; 
			m_nManStep        = 0; 
			m_nHomeStep       = 0;

			m_nInspectStep    = 0;
			m_nCleaningStep   = 0;
			m_nUtilityStep    = 0;
			m_nHomCycleStep   = 0;


			m_dDirectPosn     = 0.0; 
			//m_dEPos           = 0.0;
			//m_dMPos           = 0.0;
			m_dYPos           = 0.0;

			m_sSeqMsg         = string.Empty; 

			//Timer Clear
			m_tMainCycle .Clear();
			m_tToStart   .Clear();
			m_tToStop    .Clear();
			m_tDelayTimer.Clear();
            m_tHome      .Clear();


            //
            fn_SetDrainValve   (); 
			fn_SetDIWaterValve ();
			fn_SetSeperator    ();
			fn_SetSeperatorBlow();

			//
			if(EPU.ERR[(int)EN_ERR_LIST.ERR_0436].fn_GetErrOn() && !m_bDrngSeqDrain)
			{
				fn_SetDrain();
			}


		}
		//---------------------------------------------------------------------------
	    public double GetEncPos_Y () { return MOTR.GetEncPos(m_iMotrYId ); }
		public double GetEncPos_TH() { return MOTR.GetEncPos(m_iMotrTHId); }

		public double GetCmdPos_Y () { return MOTR.GetCmdPos(m_iMotrYId ); }
		public double GetCmdPos_TH() { return MOTR.GetCmdPos(m_iMotrTHId); }

		public double GetTrgPos_Y () { return MOTR.GetTrgPos(m_iMotrYId ); }
		public double GetTrgPos_TH() { return MOTR.GetTrgPos(m_iMotrTHId); }
		//---------------------------------------------------------------------------
		private bool CheckDstb(EN_MOTR_ID Motr, EN_COMD_ID Cmd = EN_COMD_ID.NoneCmd,
                                    int Step = NONE_STEP, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE, double DirPosn = 0.0)
        {
			//Var.
			bool isNoRun        = SEQ.fn_IsNoRun() && m_nManStep == 0 ;
			bool isOpenDoor     = SEQ.fn_IsAnyDoorOpen(); 
					    
			double dEnc_Y       = GetEncPos_Y ();
			double dEnc_TH      = GetEncPos_TH();
					    
			double dCmd_Y       = GetCmdPos_Y ();
			double dCmd_TH      = GetCmdPos_TH();
					    
			double dTrg_Y       = GetTrgPos_Y ();
			double dTrg_TH      = GetTrgPos_TH();

			double dEnc_SPDX    = MOTR.GetEncPos(EN_MOTR_ID.miSPD_X );
			double dEnc_SPDZ    = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z );
			double dEnc_SPDZ1   = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z1);

			double dMaxXAxisPos  = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLX_POS_DSTB_POLIZ ].dPos;
			double dMinZAxisPos  = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ_POS_MOVE_BTMY  ].dPos;
			double dMinZ1AxisPos = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ1_POS_MOVE_BTMY ].dPos;
			double dNextPosn     = 0;							

			if (Motr != EN_MOTR_ID.miCLN_Y && Motr != EN_MOTR_ID.miCLN_R) return false;

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
			
            bool bHomeEndZ  = MOTR[(int)EN_MOTR_ID.miSPD_Z ].GetHomeEnd();
            bool bHomeEndZ1 = MOTR[(int)EN_MOTR_ID.miSPD_Z1].GetHomeEnd();

			//Check Interfere Condition
			if (Motr == EN_MOTR_ID.miCLN_Y)
			{
				//Z-Axis Home End Check
				if (!bHomeEndZ || !bHomeEndZ1) //if ((!bHomeEndZ || !bHomeEndZ1) && Cmd != EN_COMD_ID.Home)
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

			}

			//
			if (Motr == EN_MOTR_ID.miCLN_R)
			{
				//Z-Axis Home End Check
				if (!bHomeEndZ || !bHomeEndZ1) //if ((!bHomeEndZ || !bHomeEndZ1) && Cmd != EN_COMD_ID.Home)
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

				
				//Run 중에는 Fwd 시에만 동작
				if(SEQ._bRun || m_nManStep != 0)
				{
// 					if(!ACTR.GetCylStat((int)EN_ACTR_LIST.aClen_Clamp, ccBwd))
// 					{
// 						if (isNoRun) fn_UserMsg("Check Cleaning Clamp!!!!.");
// 						return false; 
// 					}
				}

			}

            //
            return true; 

		}

        //---------------------------------------------------------------------------
        public bool fn_MoveMotr(EN_MOTR_ID Motr, EN_COMD_ID Cmd, EN_MOTR_VEL iSPD = EN_MOTR_VEL.Normal, 
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
			bool r1, r2;
			int iFHomeErr = MOTR._iFHomeErr;
			if (m_tHome.OnDelay(m_nHomeStep > 10, 120 * 1000)) 
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
                    //Spindle Z, Z1 Home End 시까지 대기...
                    if (!CheckDstb(EN_MOTR_ID.miCLN_Y)) return false;

					IO.fn_RunBuffer((int)EN_MOTR_ID.miCLN_R, false);
					IO.fn_RunBuffer((int)EN_MOTR_ID.miCLN_Y, false);

					//Clear Home end Flag
					MOTR.ClearHomeEnd(EN_MOTR_ID.miCLN_R);
                    MOTR.ClearHomeEnd(EN_MOTR_ID.miCLN_Y);

                    m_nHomeStep++;
					return false;

                case 11: //

                    EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miCLN_R , true);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miCLN_Y , true);

					m_nHomeStep++;
                    return false;

                case 12: //Buffer Run
					r1 = IO.fn_RunBuffer(BFNo_03_HOME_CLN_R, true);
                    r2 = IO.fn_RunBuffer(BFNo_07_HOME_CLN_Y, true);
                    if (!r1 || !r2)
                    {
                        fn_UserMsg(string.Format($"ACS RUN Buffer Error - {BFNo_03_HOME_CLN_R:D2}:{r1.ToString()} / {BFNo_07_HOME_CLN_Y:D2}:{r2.ToString()}"));
                        m_nHomeStep = 0;
                        return true;
                    }

                    m_nHomeStep++;
                    return false;

                case 13: //Check Buffer Run 
                    r1 = IO.fn_IsBuffRun(BFNo_03_HOME_CLN_R);
                    r2 = IO.fn_IsBuffRun(BFNo_07_HOME_CLN_Y);
                    if (!r1 || !r2 ) return false;

                    m_nHomeStep++;
                    return false;

                case 14:
                    //Check Error
                    if (IO.DATA_ACS_TO_EQ[BFNo_03_HOME_CLN_R] == 1)
                    {
						fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_03_HOME_CLN_R:D2}");
                        m_nHomeStep = 0;
                        return true;
                    }
                    
					if (IO.DATA_ACS_TO_EQ[BFNo_07_HOME_CLN_Y] == 1)
                    {
						fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_07_HOME_CLN_Y:D2}");
                        m_nHomeStep = 0;
                        return true;
                    }

                    //Check Buffer End
                    r1 = IO.fn_IsBuffRun(BFNo_03_HOME_CLN_R);
                    r2 = IO.fn_IsBuffRun(BFNo_07_HOME_CLN_Y);

					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miCLN_R, r1);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miCLN_Y, r2);
					
					if (r1 || r2) return false;

					MOTR[(int)EN_MOTR_ID.miCLN_R].SetHomeEndDone(true);
                    MOTR[(int)EN_MOTR_ID.miCLN_Y].SetHomeEndDone(true);

					m_bReqHomeCycle = false; //JUNG/201015

                    m_nHomeStep++;
                    return false;

                case 15:
					r1 = fn_MoveMotr(m_iMotrYId, EN_COMD_ID.Wait1);
					if (!r1) return false; 

					m_nHomeStep = 0;
                    return true;

            }

            return false; 
		}
		//-------------------------------------------------------------------------------------------------
		public bool fn_MoveHomeRotation()
		{
			//
			bool r1 = false;
			
			//Move Home.
			switch (m_nHomeStep)
			{
				case 0:
					m_nHomeStep = 0;
					return true;

                case 10:
					m_bDrngRotateHome = true; 

					//Spindle Z, Z1 Home End 시까지 대기...
					if (!CheckDstb(EN_MOTR_ID.miCLN_Y)) return false;

                    m_nHomeStep++;
					return false;

                case 11: //
					r1 = IO.fn_RunBuffer(BFNo_03_HOME_CLN_R, true);
					if (!r1) return false;

					m_tDelayTimer.Clear();
					m_nHomeStep++;
                    return false;

                case 12: //Check Buffer Run 
					if (!m_tDelayTimer.OnDelay(true, 300)) return false; 
					
					r1 = IO.fn_IsBuffRun(BFNo_03_HOME_CLN_R);
                    if (!r1 ) return false;

                    m_nHomeStep++;
                    return false;

                case 13:
                    //Check Error
                    if (IO.DATA_ACS_TO_EQ[BFNo_03_HOME_CLN_R] == 1)
                    {
						fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_03_HOME_CLN_R:D2}");
                        m_nHomeStep = 0;
                        return false;
                    }

                    //Check Buffer End
                    r1 = IO.fn_IsBuffRun(BFNo_03_HOME_CLN_R);
					if (r1) return false;

					//
					MOTR[(int)EN_MOTR_ID.miCLN_R].SetHomeEndDone(true);


					//Clear Error
					EPU.ERR[(int)EN_ERR_LIST.ERR_0018].fn_Reset();

					m_bDrngRotateHome = false;

					m_nHomeStep = 0;
                    return true;
            }

            return true; 
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

			if (m_nHomCycleStep   != 0) return false;
			if (m_nCleaningStep   != 0) return false;
			if (m_nUtilityStep    != 0) return false;

			//Check During Flag
			if (m_bDrngInspect        ) return false; 
			if (m_bDrngCleaning       ) return false; 
			if (m_bDrngUtility        ) return false; 
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
			
			if (m_tToStart.OnDelay(!m_bToStart && !bIsInitState, 20000))
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0150 + m_nPartId, true);
				return false;
			}


			//Timer Clear
			m_tMainCycle   .Clear();

			m_tCleanCycle  .Clear();
			m_tInspectCycle.Clear();
			m_tUtilityCycle.Clear();
			m_tHomeCycle   .Clear();


			//Flag On
			m_bToStart = true ; 
			m_bToStop  = false;


			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStop()
		{
			//
			if (m_bToStop) return true; 

			//Check Stop Time Out
			if (m_tToStop.OnDelay(!m_bToStop, 20 * 1000))
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0160 + m_nPartId, true);
				IO.fn_GroupDisable(true);

				return false;
			}

			//Clear Step Index
			m_nSeqStep       = 0; 

			m_nInspectStep   = 0;
			m_nCleaningStep  = 0;
			m_nUtilityStep   = 0;
			m_nHomCycleStep  = 0;

			m_bTESTMODE       = false;
			m_bDrngRotateHome = false; 


			//
			fn_SetDrainValve   (); 
			fn_SetDIWaterValve ();
			fn_SetSeperator    ();
			fn_SetSeperatorBlow();

			m_bToStop = true;

			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ReqBathWaitPos()
		{
			bool r1 = fn_MoveMotr(m_iMotrYId  , EN_COMD_ID.Wait1);
			bool r2 = fn_MoveMotr(m_iMotrTHId , EN_COMD_ID.Wait1);

			return (r1 && r2);

		}
		//---------------------------------------------------------------------------
		public bool fn_ReqCleanPos()
		{
			bool r1 = fn_MoveMotr(m_iMotrYId  , EN_COMD_ID.User1);
			return (r1);
		}
        //---------------------------------------------------------------------------
        public bool fn_MoveCylClamp(int act)
        {
            //
            bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aClen_Clamp, act);

            return r1;
        }
        //---------------------------------------------------------------------------
		public bool fn_AutoRun()
		{
			bool r1; 
			m_dYPos = MOTR[(int)m_iMotrYId].GetCmdPos();

			bool bErr        = EPU._bIsErr;
			bool bManRun     = FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE );
			     m_bTESTMODE = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);

			//Time Check
			m_tMainCycle   .OnDelay((m_nSeqStep        != 0 && !bErr && !bManRun),  60 * 1000 * 15); //15min

			m_tCleanCycle  .OnDelay((m_nCleaningStep   != 0 && !bErr && !bManRun),  60 * 1000 * 10); //
			m_tHomeCycle   .OnDelay((m_nHomCycleStep   != 0 && !bErr && !bManRun),  60 * 1000 * 10); //
			m_tInspectCycle.OnDelay((m_nInspectStep    != 0 && !bErr && !bManRun),  60 * 1000     ); //
			m_tUtilityCycle.OnDelay((m_nUtilityStep    != 0 && !bErr && !bManRun), 100 * 1000     ); //

			//
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0170 + m_nPartId, m_tMainCycle.Out))
			{
				sLogMsg = string.Format($"[CLEAN] Main Cycle Time Out : m_iSeqStep = {m_nSeqStep}"); 
			}

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0194, m_tCleanCycle.Out))
            {
                sLogMsg = string.Format($"[CLEAN] Cleaning Cycle Time Out : m_nCleaningStep = {m_nCleaningStep}");
            }
            
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0195, m_tUtilityCycle.Out))
            {
                sLogMsg = string.Format($"[CLEAN] Utility Cycle Time Out : m_nUtilityStep = {m_nUtilityStep}");
            }
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0179, m_tHomeCycle.Out))
            {
                sLogMsg = string.Format($"[CLEAN] Home Cycle Time Out : m_nHomCycleStep = {m_nHomCycleStep}");
            }


			//
			if (m_tMainCycle     .Out ||
				m_tCleanCycle    .Out ||
				m_tHomeCycle     .Out ||
				m_tUtilityCycle  .Out )
			{
				fn_WriteLog(sLogMsg);
				LOG.fn_CrntStateTrace(EN_SEQ_ID.CLEAN, sLogMsg);
				fn_Reset();

				m_bReqHomeCycle = true; 

				return false; 
			}


			//Emergency Error Check
			bool bEMOErr = EPU.fn_IsEMOError();  // Emergency Error
			if (m_nSeqStep != 0 && bEMOErr)
			{
				//
				sLogMsg = string.Format($"[EMO][SEQ_CLEN] Force Cycle End m_nSeqStep = {m_nSeqStep}");
				fn_WriteLog(sLogMsg);
				
				fn_Reset();
				
				m_nSeqStep = 0;
				return false; 
			}

			//Decide Step
			if (m_nSeqStep == 0)
			{
				//Var
				bool bYWaitPos	      = MOTR.CmprPosByCmd(m_iMotrYId , EN_COMD_ID.Wait1);
				bool isDrngPlatePlace = SEQ_SPIND._bDrngPlatePlce && SEQ_SPIND._bDrngPlatePlceC;
				bool isDrngCleaning   = SEQ_SPIND._bDrngCleaning;
				bool isReadyDehydrate = DM.MAGA[(int)EN_MAGA_ID.CLEAN].IsAllStat((int)EN_PLATE_STAT.ptsDeHydrate); //
				//

				//Step Condition
				bool isConCleaning    = !isDrngPlatePlace && isReadyDehydrate && !isDrngCleaning;
				bool isConHomeCycle   = isConCleaning     && m_bReqHomeCycle ;
				bool isConUtility     = false;  
				bool isConWait        = false; //!isConInspect && !isConCleaning && !isConUtility && !bYWaitPos;

				//Clear Var.
				m_bDrngInspect        = false;
				m_bDrngCleaning       = false;
				m_bDrngUtility        = false;
				m_bDrngWait           = false;


				//Step Clear
				m_nInspectStep        = 0;
				m_nCleaningStep       = 0;
				m_nUtilityStep	      = 0;
				m_nHomCycleStep       = 0;
				
				m_sSeqMsg             = string.Empty; 

				//Check Sequence Stop
				if ( SEQ._bStop                                          ) return false; 
				if ( EPU.fn_GetHasErr()                                  ) return false; 
				if (!SEQ._bRun && !FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE)) return false; 

				//
			  //if (isConInspect     ) { m_bDrngInspect    = true; m_nSeqStep = 100 ; m_nInspectStep    = 10; m_sSeqMsg = ""           ; goto __GOTO_CYCLE__; }
				if (isConHomeCycle   ) { m_bDrngCleaning   = true; m_nSeqStep = 100 ; m_nHomCycleStep   = 10; m_sSeqMsg = "Home Cycle" ; goto __GOTO_CYCLE__; }
				if (isConCleaning    ) { m_bDrngCleaning   = true; m_nSeqStep = 200 ; m_nCleaningStep   = 10; m_sSeqMsg = "Cleaning"   ; goto __GOTO_CYCLE__; }
				if (isConUtility     ) { m_bDrngUtility    = true; m_nSeqStep = 300 ; m_nUtilityStep	= 10; m_sSeqMsg = "Utility"    ; goto __GOTO_CYCLE__; }
				if (isConWait        ) { m_bDrngWait       = true; m_nSeqStep = 1300;                         m_sSeqMsg = "Wait"       ; goto __GOTO_CYCLE__; }

			}

			//Cycle Start
			__GOTO_CYCLE__:

			//Cycle
			switch (m_nSeqStep)
			{
				case 100:
					if (fn_HomeCycle(ref m_bDrngCleaning)) m_nSeqStep = 0;
					return false;

				case 200:
					if (fn_CleaningCycle(ref m_bDrngCleaning)) m_nSeqStep = 0;
					return false;

				case 300:
					if (fn_UtilityCycle(ref m_bDrngUtility)) m_nSeqStep = 0;
					return false;

				//Wait Position 
				case 1300:
					//
					r1 = fn_MoveMotr(m_iMotrYId, EN_COMD_ID.Wait1);
					if (!r1) return false;

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
		private bool fn_HomeCycle(ref bool Flag)
		{
			bool r1 = false;
			
			if (m_nHomCycleStep < 0) m_nHomCycleStep = 0;

			switch (m_nHomCycleStep)
			{
				default:
					Flag = false;
					m_nHomCycleStep = 0;
					return true;

				case 10:
					Flag = true;

					//
					fn_WriteSeqLog("[START] Home");

					fn_WriteLog("[START] Home Cycle before Cleaning", EN_LOG_TYPE.ltLot);

					fn_MoveCylClamp(ccFwd);

					m_nHomeStep = 10;

					m_tDelayTimer.Clear();
					m_nHomCycleStep++;
					return false;

				case 11:

					if (!m_tDelayTimer.OnDelay(true, 300)) return false;

					m_bDrngRotateHome = true;

					//Home
					r1 = fn_MoveHomeRotation();
					if (!r1) return false;

					m_bDrngRotateHome = false;

					m_bReqHomeCycle = false; //JUNG/201015

					m_nHomCycleStep = 0;
					
					fn_WriteSeqLog("[START] End");

					fn_WriteLog("[END] Home Cycle", EN_LOG_TYPE.ltLot);

					return true;
			}

        }
		//---------------------------------------------------------------------------
		double dPos  = 0.0;
		double dTime = 0.0;
		double dSpd  = 0.0;
        private bool fn_CleaningCycle(ref bool Flag)
        {
			bool r1;
			double dPitchR = 360.0;
			
			//
			m_bRotatewithPos = true; 

			if (m_nCleaningStep < 0) m_nCleaningStep = 0;

			switch (m_nCleaningStep)
			{
				default:
					m_nCleaningStep = 0;
					return true;

				case 10:
					Flag = true;

                    //
					fn_WriteSeqLog("[START] DEHYDRATION");

					fn_WriteLog("[START] DEHYDRATION", EN_LOG_TYPE.ltLot) ;

					fn_MoveCylClamp(ccFwd);
					
					SEQ_TRANS.fn_MoveCylLoadCover(ccFwd);

					m_nCleaningStep++;
                    return false;
                
				case 11:

					if (!fn_MoveCylClamp(ccFwd)) return false;

					//Test Mode
					if (m_bTESTMODE)
					{
                        m_tDelayTimer.Clear();
                        m_nCleaningStep ++;
                        return false;
                    }

					fn_SetDrain();

					fn_SetDIWaterValve(vvOpen);

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
					return false;

                case 12:
					if (!m_tDelayTimer.OnDelay(true, 300)) return false;

					//Test Mode
					if (m_bTESTMODE)
					{
						Console.WriteLine("DEHYDRATINO_TEST");
						m_nCleaningStep = 17;
                        return false;
					}

                    //Check Rotation Method with Position
                    if (m_bRotatewithPos)
                    {
                        m_tDelayTimer.Clear();
                        m_nCleaningStep = 20;
                        return false;
                    }

					//
					fn_SetDIWaterValve(vvOpen);
					fn_SetDrain();

					//with Degree
					//dPos = MOTR[(int)m_iMotrTHId].MP.dPosn[(int)EN_POSN_ID.User1];
					//r1 = MOTR.MoveMotrR(m_iMotrTHId, dPos, MOTR[(int)m_iMotrTHId].MP.dVel[(int)EN_MOTR_VEL.Work]);

					dTime =   g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingTime;
					dPos  = ((g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM / 60.0) * dPitchR) * dTime; //
					dSpd  =   fn_GetRPMtoSPD(g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM);

					if(dTime < 1 || dPos < 1 || dSpd < 1)
					{
						fn_SetDIWaterValve(vvClose);

						EPU.fn_SetErr(EN_ERR_LIST.ERR_0439);

						//
						fn_WriteLog($"DEHYDRATE - Cleaning Data Error : dTime({dTime}) / dPos({dPos}) / dSpd({dSpd})");

						Flag = false;

                        m_nCleaningStep = 0;
                        return true;

                    }

                    r1 = MOTR.MoveMotrR(m_iMotrTHId, dPos, dSpd);
					if (!r1) return false;

					sLogMsg = string.Format($"DEHYDRATION 01 - {dPos} / RPM = {g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM} / SPD = {dSpd}");
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);
					Console.WriteLine(sLogMsg);

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

				case 13:
                    
					if (SEQ._bStop)
                    {
						m_nCleaningStep = 0;
                        Flag = false;
                        return true;
                    }

					fn_SetDIWaterValve(vvOpen);
					fn_SetDrain();

					if (!m_tDelayTimer.OnDelay(true, 1000)) return false;

                    if (!MOTR[(int)m_iMotrTHId].GetStop()) return false;
					if ( MOTR[(int)m_iMotrTHId].GetBusy()) return false;

					fn_SetDIWaterValve(vvClose);

					//
					//IO.fn_ClearPos(m_iMotrTHId);

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

				case 14:

					fn_SetDrain();

					if (!m_tDelayTimer.OnDelay(true, 1000)) return false;

					//Low Speed
					dTime =   g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingTime;
					dPos  = ((g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM / 60.0) * dPitchR) * dTime; //
					dSpd  =   fn_GetRPMtoSPD(g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM);

					r1 = MOTR.MoveMotrR(m_iMotrTHId, dPos, dSpd);
					if (!r1) return false;

					sLogMsg = string.Format($"DEHYDRATION 02 - {dPos} / SPD = {dSpd}");
					Console.WriteLine(sLogMsg);
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

                case 15:

					fn_SetDrain();

					if (!m_tDelayTimer.OnDelay(true, 5000)) return false;

					if (!MOTR[(int)m_iMotrTHId].GetStop()) return false;
					if ( MOTR[(int)m_iMotrTHId].GetBusy()) return false;

					//
					//IO.fn_ClearPos(m_iMotrTHId);
					
					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

				case 16:

					fn_SetDrain();

					//
					if (!m_tDelayTimer.OnDelay(true, 3000)) return false;
				
					//High Speed
     				dTime =   g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationTime;
					dPos  = ((g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationRPM / 60.0) * dPitchR) * dTime; //
					dSpd  =   fn_GetRPMtoSPD(g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationRPM);

					r1 = MOTR.MoveMotrR(m_iMotrTHId, dPos, dSpd);
					if (!r1) return false;

					sLogMsg = string.Format($"DEHYDRATION 03 - {dPos} / RPM = {g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationRPM} / SPD = {dSpd}");
					Console.WriteLine(sLogMsg);
					fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

					//JUNG/201015
					MOTR[(int)m_iMotrTHId].Stop();

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

                case 17:
					fn_SetDrain();

					if (!m_tDelayTimer.OnDelay(true, 5000)) return false;

					if (!MOTR[(int)m_iMotrTHId].GetStop()) return false;
					if ( MOTR[(int)m_iMotrTHId].GetBusy()) return false;

					m_nHomeStep = 10;

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

				case 18:
					fn_SetDrain();

					if (!m_tDelayTimer.OnDelay(true, 300)) return false;

					m_bDrngRotateHome = true; 

					//Home
					r1 = fn_MoveHomeRotation();
					if (!r1) return false;

					m_bDrngRotateHome = false; 
					m_nCleaningStep++;
                    return false;

                case 19:

                    //Map Change
                    DM.MAGA[(int)EN_MAGA_ID.CLEAN].SetTo((int)EN_PLATE_STAT.ptsFinish);

					fn_ReqBathWaitPos();

					//
					fn_WriteSeqLog("[END] DEHYDRATION (MAP:FINISH)");
					fn_WriteLog("[END] DEHYDRATION", EN_LOG_TYPE.ltLot);

					Flag = false; 

                    m_nCleaningStep = 0;
                    return true;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Rotation move as position
                case 20:
                    if (SEQ._bStop)
                    {
                        if (!MOTR[(int)m_iMotrTHId].Stop()) return false;
                        
						//fn_SetDrainValve  (vvClose);
                        fn_SetDIWaterValve(vvClose);
						
						Flag = false;

                        m_nCleaningStep = 0;
                        return true;
                    }

					//
					fn_SetDrain();

					//Low Speed with DI
					dTime =    g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingTime;
                    dPos  = (((g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM / 60.0) * dPitchR) * dTime) + GetEncPos_TH(); //
                    dSpd  =    fn_GetRPMtoSPD(g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM);

					//
					MOTR[(int)m_iMotrTHId].MP.dPosn[(int)EN_POSN_ID.User11] = dPos;
					MOTR[(int)m_iMotrTHId].MP.dVel[(int)EN_MOTR_VEL.User1 ] = dSpd;

					//MOTR[(int)m_iMotrTHId].MP.dAcc[(int)EN_MOTR_VEL.User1 ] = dSpd;
                    //MOTR[(int)m_iMotrTHId].MP.dDec[(int)EN_MOTR_VEL.User1 ] = dSpd;

					//
					if (dTime < 1 || dPos < 1 || dSpd < 1)
					{
						fn_SetDIWaterValve(vvClose);

						EPU.fn_SetErr(EN_ERR_LIST.ERR_0439);

                        //
                        fn_WriteLog($"DEHYDRATE - Cleaning Data Error : dTime({dTime}) / dPos({dPos}) / dSpd({dSpd})");

                        Flag = false;

                        m_nCleaningStep = 0;
                        return true;

                    }

                    sLogMsg = string.Format($"DEHYDRATION 01 - {dPos} / RPM = {g_VisionManager.CurrentRecipe.Cleaning[0].PreWashingRPM} / SPD = {dSpd}");
                    Console.WriteLine(sLogMsg);
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    m_nCleaningStep++;
                    return false;

				case 21:
                    if (SEQ._bStop)
                    {
                        //fn_SetDrainValve  (vvClose);
                        fn_SetDIWaterValve(vvClose);

                        if (!MOTR[(int)m_iMotrTHId].Stop()) return false;

                        Flag = false;

                        m_nCleaningStep = 0;
                        return true;
                    }

                    //
                    fn_SetDrain();

					//Cleaning - 1 : Low Speed with DI
					r1 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.User11, EN_MOTR_VEL.User1);
					if (!r1) return false;

					fn_SetDIWaterValve(vvClose);

					//
					MOTR[(int)m_iMotrTHId].MP.dPosn[(int)EN_POSN_ID.User11] = dPos + GetEncPos_TH();  //JUNG/200915

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

                case 22:
                    if (SEQ._bStop)
                    {
                        if (!MOTR[(int)m_iMotrTHId].Stop()) return false;

                        //fn_SetDrainValve  (vvClose);
                        fn_SetDIWaterValve(vvClose);

                        Flag = false;

                        m_nCleaningStep = 0;
                        return true;
                    }

                    //
                    fn_SetDrain();

					if (!m_tDelayTimer.OnDelay(true, 1000)) return false;

					//Cleaning - 2 : Low Speed without DI
					r1 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.User11, EN_MOTR_VEL.User1);
                    if (!r1) return false;

					sLogMsg = string.Format("DEHYDRATION 02");
                    Console.WriteLine(sLogMsg);
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    m_tDelayTimer.Clear();
                    m_nCleaningStep++;
                    return false;

                case 23:
                    if (SEQ._bStop)
                    {
                        if (!MOTR[(int)m_iMotrTHId].Stop()) return false;

                        //fn_SetDrainValve  (vvClose);
                        fn_SetDIWaterValve(vvClose);

                        Flag = false;

                        m_nCleaningStep = 0;
                        return true;
                    }

                    fn_SetDrain();

                    if (!m_tDelayTimer.OnDelay(true, 3000)) return false;

                    //High Speed
                    dTime =    g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationTime;
                    dPos  = (((g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationRPM / 60.0) * dPitchR) * dTime) + GetEncPos_TH(); //
                    dSpd  =   fn_GetRPMtoSPD(g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationRPM);

                    //
                    MOTR[(int)m_iMotrTHId].MP.dPosn[(int)EN_POSN_ID.User12] = dPos; //JUNG/200915/현재 위치 +
					MOTR[(int)m_iMotrTHId].MP.dVel[(int)EN_MOTR_VEL.User2 ] = dSpd;
					//MOTR[(int)m_iMotrTHId].MP.dAcc[(int)EN_MOTR_VEL.User2 ] = dSpd;
                    //MOTR[(int)m_iMotrTHId].MP.dDec[(int)EN_MOTR_VEL.User2 ] = dSpd;

                    sLogMsg = string.Format($"DEHYDRATION 03 - {dPos} / RPM = {g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationRPM} / SPD = {dSpd}");
                    Console.WriteLine(sLogMsg);
                    fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot);

                    m_tDelayTimer.Clear();
                    m_nCleaningStep++;
                    return false;

                case 24:
                    if (SEQ._bStop)
                    {
                        if (!MOTR[(int)m_iMotrTHId].Stop()) return false;

                        //fn_SetDrainValve  (vvClose);
                        fn_SetDIWaterValve(vvClose);

                        Flag = false;

                        m_nCleaningStep = 0;
                        return true;
                    }

                    fn_SetDrain();

					//Cleaning - 3 : High Speed without DI
					r1 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.User12, EN_MOTR_VEL.User2);
                    if (!r1) return false;
                    
					m_tDelayTimer.Clear();
                    m_nCleaningStep++;
                    return false;
				
				case 25:
                    if (SEQ._bStop)
                    {
                        if (!MOTR[(int)m_iMotrTHId].Stop()) return false;

                        //fn_SetDrainValve  (vvClose);
                        fn_SetDIWaterValve(vvClose);

                        Flag = false;

                        m_nCleaningStep = 0;
                        return true;
                    }

                    fn_SetDrain();

					if (!m_tDelayTimer.OnDelay(true, 3000)) return false;

					//Home
					m_nHomeStep = 10;

                    m_tDelayTimer.Clear();
                    m_nCleaningStep++;
                    return false;

                case 26:
                    
                    r1 = fn_MoveHomeRotation(); //Home
                    if (!r1) return false;

					m_tDelayTimer.Clear();
					m_nCleaningStep++;
                    return false;

                case 27:

					if (!m_tDelayTimer.OnDelay(true, 500)) return false;

					//Map Change
					DM.MAGA[(int)EN_MAGA_ID.CLEAN].SetTo((int)EN_PLATE_STAT.ptsFinish);

                    fn_ReqBathWaitPos();

                    //
                    fn_WriteSeqLog("[END] DEHYDRATION (MAP:FINISH)");
                    fn_WriteLog   ("[END] DEHYDRATION", EN_LOG_TYPE.ltLot);

                    Flag = false;

                    m_nCleaningStep = 0;
                    return true;


                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //case 50:
				//	if(SEQ._bStop)
				//	{
				//		if (!MOTR[(int)m_iMotrTHId].Stop()) return false;
				//
				//
                //        fn_SetDrainValve  (vvClose);
                //        fn_SetDIWaterValve(vvClose);
				//
                //        Flag = false;
				//
                //        m_nCleaningStep = 0;
                //        return true;
				//
                //    }
				//
                //    dTime = MOTR[(int)m_iMotrTHId].MP.dPosn[(int)EN_POSN_ID.User2];
				//
				//	//with Time
				//	dPos = 360 * 10000;
				//	r1 = fn_MoveDirect(m_iMotrTHId, dPos);
				//	//if (!r1) return false;
				//
				//	if (!m_tDelayTimer.OnDelay(true, dTime)) return false;
				//
				//	MOTR[(int)m_iMotrTHId].Stop();
				//	m_nCleaningStep++;
                //    return false;
				//
                //case 51:
				//	if (!MOTR[(int)m_iMotrTHId].Stop()) return false;
                //    
				//	m_nCleaningStep = 13;
                //    return false;

            }
        }
		//---------------------------------------------------------------------------
		private double fn_GetRPMtoSPD(double rpm)
		{
			//g_VisionManager.CurrentRecipe.Cleaning[0].DehydrationRPM
			double dMin = 60.0;

			double spd = (rpm / dMin) * 360.0;

			return spd; 
		}
		//---------------------------------------------------------------------------
        private bool fn_UtilityCycle(ref bool Flag)
        {
            return false;
        }
        //---------------------------------------------------------------------------
        public bool fn_CleaningOneCycle()
        {
			bool   r1;
			double dPos = 0.0; 

            if (m_nManStep < 0) m_nManStep = 0;

			switch (m_nManStep)
			{
                default:
					m_nManStep = 0;
                    return true;

                case 10:
                    fn_SetDrainValve  (vvOpen);
					fn_SetDIWaterValve(vvOpen);

					m_tDelayTimer.Clear(); 

					m_nManStep++;
					return false;

                case 11:

					if (!m_tDelayTimer.OnDelay(true, 300)) return false; 

					//Rotation ON while time or rotation count
					//dPos = MOTR.fn_GetPosbyRad();


					
					r1 = fn_MoveDirect(m_iMotrTHId,dPos);
					if (!r1) return false;

					m_nHomeStep = 10;

					m_tDelayTimer.Clear();
					m_nManStep++;
                    return false;

				case 12:
					if (!m_tDelayTimer.OnDelay(true, 300)) return false;

					//Home
					r1 = fn_MoveHome();
					if (!r1) return false;

					m_nManStep++;
                    return false;

                case 13:

					fn_UserMsg("CLEANING CYCLE END");

                    m_nManStep = 0;
                    return true;
			}
        }

        //---------------------------------------------------------------------------
        public bool fn_UtilityOneCycle()
		{
			//bool r1;
			bool isMainZWait  = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
			bool isMainZ1Wait = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);

			//Pick Cycle
			if (m_nManStep < 0) m_nManStep = 0;

// 			switch (m_nManStep)
// 			{
// 				case 10:
//                     if(!isMainZWait || !isMainZ1Wait)
// 					{
//                         fn_UserMsg("Please Check Spindle Z or Z1 Axis Position");
//                         m_nManStep = 0;
//                         return true;
//                     }
// 
//                     m_nManStep ++;
//                     return false;
// 
//                 case 11:
//                     r1 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X , EN_COMD_ID.User11);
// 					if (!r1) return false;
//                     m_nManStep++;
//                     return false;
// 
//                 case 12: //Utility Supply and Check Water Level Sensor status.
// 					r1 = true;
// 					r2 = true;
// 					if (!r1 || !r2) return false;
// 
//                     m_nManStep++;
//                     return false;
// 
// 				case 13:
// 					r1 = true;
//                     if (!r1) return false;
//                     m_nManStep++;
//                     return false;
// 
//                 case 14:
// 					r1 = true;
//                     if (!r1) return false;
//                     m_nManStep++;
//                     return false;
// 
//                 case 15:
// 					r1 = true;
//                     if (!r1) return false;
//                     m_nManStep++;
//                     return false;
// 
//                 case 16:
// 					
// 					fn_UserMsg("Utility One Cycle OK.");
// 
//                     m_nManStep=0;
//                     return true;
// 			}

			return false; 
		}
        //---------------------------------------------------------------------------
        public bool fn_DoDrainOneCycle()
        {
            // 1) Cleaning Drain Valve Open_Y0055
            // 2) 3sec Delay(??)
            // 3) VACUUM_SEPARATOR ON

            if (m_nManStep < 0) m_nManStep = 0;

            switch (m_nManStep)
            {
                case 10:
					fn_SetSeperator    (vvClose);
                    fn_SetSeperatorBlow(vvOpen);
                    fn_SetDrainValve   (vvOpen);

                    m_tDrainTime.Clear();
                    m_nManStep++;
                    return false;

                case 11:
                    if (!m_tDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nDrainTime)) return false; //10 sec

                    fn_SetDrainValve(vvClose); // 
                    fn_SetSeperatorBlow(vvClose); // 
                    m_nManStep++;
                    return false;

                case 12:

                    fn_SetSeperator(vvOpen);

                    m_tDrainTime.Clear();
                    m_nManStep++;
                    return false;

                case 13:
                    if (!m_tDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nSepBlowTime)) return false; //5 sec

                    fn_SetSeperator(vvClose);

                    fn_UserMsg("Cleaning Drain One Cycle OK.");

                    m_nManStep = 0;
                    return true;

            }


            return false;

        }

		//---------------------------------------------------------------------------
		public void fn_SetDrain(bool forceset = false)
        {
			if (m_bDrngSeqDrain && !forceset) return;

			//JUNG/200717/Test Mode
			if (m_bTESTMODE && SEQ._bRun) return;
			if (SEQ_POLIS._bDrngSeqDrain) return;

            m_nReqDrainStep = 10;
			m_bDrngSeqDrain = true;

			fn_WriteLog("[Cleaning] Set Drain");
        }
        //---------------------------------------------------------------------------
        public bool fn_SeqDrain()
        {
			if (m_nReqDrainStep < 0) m_nReqDrainStep = 0;
			
			//Drain Sequence
			switch (m_nReqDrainStep)
			{
				case 10:
					m_bDrngSeqDrain = true;

					fn_SetSeperator     (vvClose);
					fn_SetSeperatorBlow	(vvOpen ); //LEE//200422
					fn_SetDrainValve	(vvOpen ); //LEE//200422

					m_tDrainTime.Clear();
					m_nReqDrainStep++;
					return false;

                case 11:
					if (!m_tDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nDrainTime)) return false; //10 sec

                    fn_SetDrainValve	(vvClose); // 
                    fn_SetSeperatorBlow	(vvClose); // 

                    m_nReqDrainStep++;
                    return false;

                case 12:

					fn_SetSeperator(vvOpen);

					m_tDrainTime.Clear();
					m_nReqDrainStep++;
                    return false;
				
				case 13:
                    if (!m_tDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nSepBlowTime)) return false; //5 sec

                    fn_SetSeperator(vvClose);

                    m_bDrngSeqDrain = false;

					m_tDrainTime.Clear();
					m_nReqDrainStep = 0;
					return true ;

                default:
					m_bDrngSeqDrain = false;
					m_nReqDrainStep = 0;
                    return true;

			}
			
        }

        //---------------------------------------------------------------------------
        //public bool fn_SeqDrain()
        //{
		//	if (m_nReqDrainStep < 0) m_nReqDrainStep = 0;
		//	
		//	//Drain Sequence
		//	switch (m_nReqDrainStep)
		//	{
		//		case 10:
		//			m_bDrngSeqDrain = true;
		//
		//			fn_SetSeperator     (vvClose);
		//			fn_SetSeperatorBlow	(vvClose); 
		//			
		//			fn_SetDrainValve	(vvOpen ); //JUNG/200914
		//
		//			m_tDrainTime.Clear();
		//			m_nReqDrainStep++;
		//			return false;
		//		
		//		case 11:
		//			if (!m_tDrainTime.OnDelay(true, 2000)) return false; //2 sec
		//
		//			fn_SetSeperator    (vvClose);
        //            fn_SetSeperatorBlow(vvOpen ); //LEE//200422
        //            fn_SetDrainValve   (vvOpen ); //LEE//200422
		//
        //            m_tDrainTime.Clear();
        //            m_nReqDrainStep++;
        //            return false;
		//
        //        case 12:
		//			if (!m_tDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nDrainTime)) return false; //10 sec
		//
        //            fn_SetDrainValve	(vvClose); // 
        //            fn_SetSeperatorBlow	(vvClose); // 
		//
		//			m_tDrainTime.Clear();
		//			m_nReqDrainStep++;
        //            return false;
		//
        //        case 13:
		//
		//			fn_SetDrainOut(vvOpen);
		//
		//			if (!m_tDrainTime.OnDelay(true, 1000)) return false; //1 sec
		//
		//			fn_SetSeperator(vvOpen);
		//
		//			m_tDrainTime.Clear();
		//			m_nReqDrainStep++;
        //            return false;
		//		
		//		case 14:
        //            if (!m_tDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nSepBlowTime)) return false; //5 sec
		//
        //            fn_SetSeperator(vvClose);
		//
        //            m_tDrainTime.Clear();
        //            m_nReqDrainStep++;
        //            return false;
		//
        //        case 15:
		//			if (!m_tDrainTime.OnDelay(true, 1000)) return false; //1 sec
		//
		//			fn_SetDrainOut(vvClose);
		//
		//			m_bDrngSeqDrain = false;
		//
		//			m_tDrainTime.Clear();
		//			m_nReqDrainStep = 0;
		//			return true ;
		//
        //        default:
		//			m_bDrngSeqDrain = false;
		//			m_nReqDrainStep = 0;
        //            return true;
		//
		//	}
		//	
        //}
        //---------------------------------------------------------------------------
        public bool fn_SetSeperator(bool set = false)
        {
			IO.YV[(int)EN_OUTPUT_ID.yValue_SperatorDrain] = false; //미사용//pump drain과 둘중 하나만....
			IO.YV[(int)EN_OUTPUT_ID.yPump_Drain         ] = m_bTESTMODE ? false : set; //

			IO.YV[(int)EN_OUTPUT_ID.yValue_backflow     ] = IO.YV[(int)EN_OUTPUT_ID.yPump_Drain];

			return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_SetDrainOut(bool set)
        {
            IO.YV[(int)EN_OUTPUT_ID.yValue_backflow] = set; //JUNG/200523/역류방지...

            return true;
        }

        //---------------------------------------------------------------------------
        public bool fn_SetSeperatorBlow(bool set = false)
        {
            IO.YV[(int)EN_OUTPUT_ID.yValue_SperatorBlower] = set;

            return true;
        }

        //---------------------------------------------------------------------------
        public void fn_SetDIWaterValve(bool set = false)
		{
			//Check Water level 
			if(SEQ.fn_IsCLNLvlErr())
			{
				IO.YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater] = false;
				IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = false;
				IO.YV[(int)EN_OUTPUT_ID.yCLN_AirBlow      ] = false; 

				return; 
			}

            //JUNG/200717/Test Mode
            if (m_bTESTMODE && SEQ._bRun) return;

            bool bUsePump = FM.m_stSystemOpt.nUseAutoSlurry == 0 && !m_bTESTMODE;

			IO.YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater] = set;
			IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = bUsePump? set : false;


			//Console.WriteLine("fn_SetDIWaterValve : " + set);

			if (FM.m_stSystemOpt.nUseCleanAirBlow == 1) //user Option
			{
				IO.YV[(int)EN_OUTPUT_ID.yCLN_AirBlow] = set;
			}
			else
			{
				IO.YV[(int)EN_OUTPUT_ID.yCLN_AirBlow] = vvClose;
			}

		}
		//---------------------------------------------------------------------------
		public void fn_SetDrainValve(bool set = false)
		{
			//
			IO.YV[(int)EN_OUTPUT_ID.yCLN_Valve_Drain] = set;
		}
        //---------------------------------------------------------------------------
        private void fn_WriteSeqLog(string log)
        {
			string stemp = string.Format($"[{EN_SEQ_ID.CLEAN.ToString()}] ");
            fn_WriteLog(stemp + log, EN_LOG_TYPE.ltEvent, EN_SEQ_ID.CLEAN);
        }
        //---------------------------------------------------------------------------
        public void fn_SetAlignPos(double pos)
        {
			if (m_dAlignPosTH > 10 || m_dAlignPosTH < -10) return; 

			//
			double dRWaitPos = MOTR[(int)m_iMotrTHId].GetPosToCmdId(EN_COMD_ID.Wait1);

			m_dAlignPosTH = pos;

            MOTR[(int)m_iMotrTHId].MP.dPosn[(int)EN_POSN_ID.CalPos] = m_dAlignPosTH + dRWaitPos;

			string sTemp = string.Format($"Set Cleaning TH-Align pos(polishing) : Offset = {pos} / Position = {m_dAlignPosTH + dRWaitPos}");

			Console.WriteLine(sTemp);
			fn_WriteLog(sTemp);

        }
		//---------------------------------------------------------------------------
		public void fn_SaveLog(ref string Msg)
        {
			string sTemp = string.Empty;

			Msg = string.Empty;

			//
			Msg += "[SeqCleaning]\r\n"; 
			Msg += sTemp = string.Format($"m_bToStart      = {m_bToStart       }\r\n");
            Msg += sTemp = string.Format($"m_bToStop       = {m_bToStop        }\r\n");
            Msg += sTemp = string.Format($"m_bDrngInspect  = {m_bDrngInspect   }\r\n");
            Msg += sTemp = string.Format($"m_bDrngCleaning = {m_bDrngCleaning  }\r\n");
            Msg += sTemp = string.Format($"m_bDrngUtility  = {m_bDrngUtility   }\r\n");
            Msg += sTemp = string.Format($"m_bDrngWait     = {m_bDrngWait      }\r\n");
            Msg += sTemp = string.Format($"m_nSeqStep      = {m_nSeqStep       }\r\n");
            Msg += sTemp = string.Format($"m_nManStep      = {m_nManStep       }\r\n");
            Msg += sTemp = string.Format($"m_nHomeStep     = {m_nHomeStep      }\r\n");
            Msg += sTemp = string.Format($"m_nInspectStep  = {m_nInspectStep   }\r\n");
            Msg += sTemp = string.Format($"m_nCleaningStep = {m_nCleaningStep  }\r\n");
            Msg += sTemp = string.Format($"m_nUtilityStep  = {m_nUtilityStep   }\r\n");
            Msg += sTemp = string.Format($"m_dAlignPosTH   = {m_dAlignPosTH    }\r\n");

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
			Items[nRow, 0].Content = "_";				Items[nRow++, 1].Content = string.Format($"");			
			Items[nRow, 0].Content = "bDrngInspect   "; Items[nRow++, 1].Content = string.Format($"{m_bDrngInspect   }");
            Items[nRow, 0].Content = "bDrngCleaning  "; Items[nRow++, 1].Content = string.Format($"{m_bDrngCleaning  }");
            Items[nRow, 0].Content = "bDrngUtility   "; Items[nRow++, 1].Content = string.Format($"{m_bDrngUtility   }");
			Items[nRow, 0].Content = "bDrngWait      "; Items[nRow++, 1].Content = string.Format($"{m_bDrngWait      }");
			Items[nRow, 0].Content = "";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nSeqStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nSeqStep       }");
			Items[nRow, 0].Content = "nManStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nManStep       }");
			Items[nRow, 0].Content = "nHomeStep"      ; Items[nRow++, 1].Content = string.Format($"{m_nHomeStep      }");
			Items[nRow, 0].Content = "_";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nInspectStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nInspectStep   }");
			Items[nRow, 0].Content = "nCleaningStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nCleaningStep  }");
			Items[nRow, 0].Content = "nUtilityStep"	  ; Items[nRow++, 1].Content = string.Format($"{m_nUtilityStep	  }");
			Items[nRow, 0].Content = "nReqDrainStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nReqDrainStep  }");
			Items[nRow, 0].Content = "_";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "m_dAlignPosTH"  ; Items[nRow++, 1].Content = string.Format($"{m_dAlignPosTH    }");
			Items[nRow, 0].Content = "bReqHomeCycle"  ; Items[nRow++, 1].Content = string.Format($"{m_bReqHomeCycle  }"); 
			Items[nRow, 0].Content = "";				Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "Seq Msg"        ; Items[nRow++, 1].Content = string.Format($"{m_sSeqMsg        }");
			
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

			Items[nRow, 0].Content = "iSeqStep"        ; Items[nRow++, 1].Content = string.Format($"{m_nSeqStep       }");
			Items[nRow, 0].Content = "iManStep"        ; Items[nRow++, 1].Content = string.Format($"{m_nManStep       }");
			Items[nRow, 0].Content = "iHomeStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nHomeStep      }");
			Items[nRow, 0].Content = "_";                Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nInspectStep"    ; Items[nRow++, 1].Content = string.Format($"{m_nInspectStep   }");
			Items[nRow, 0].Content = "nCleaningStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nCleaningStep  }");
			Items[nRow, 0].Content = "nUtilityStep"    ; Items[nRow++, 1].Content = string.Format($"{m_nUtilityStep   }");
			Items[nRow, 0].Content = "_";                Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nReqDrainStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nReqDrainStep  }");
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
				m_bReqHomeCycle = br.ReadBoolean();

				m_bSpare1       = br.ReadBoolean();
                m_bSpare2       = br.ReadBoolean();
                m_bSpare3       = br.ReadBoolean();
                m_bSpare4       = br.ReadBoolean();

				//
				m_dAlignPosTH   = br.ReadDouble ();
				m_dSpare1       = br.ReadDouble ();
                m_dSpare2       = br.ReadDouble ();
                m_dSpare3       = br.ReadDouble ();
                m_dSpare4       = br.ReadDouble ();
                m_dSpare5       = br.ReadDouble ();

            }
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);

				bw.Write(m_nSpare1      ); //Spare
                bw.Write(m_nSpare2      );
                bw.Write(m_nSpare3      );
                bw.Write(m_nSpare4      );
                bw.Write(m_nSpare5      );

				//
				bw.Write(m_bReqHomeCycle);

				bw.Write(m_bSpare1      ); //Spare
                bw.Write(m_bSpare2      );
                bw.Write(m_bSpare3      );
                bw.Write(m_bSpare4      );

				//
				bw.Write(m_dAlignPosTH  );
									    
                bw.Write(m_dSpare1      ); //Spare
                bw.Write(m_dSpare2      );
                bw.Write(m_dSpare3      );
                bw.Write(m_dSpare4      );
                bw.Write(m_dSpare5      );


                bw.Flush();

            }
        }







    }
}
