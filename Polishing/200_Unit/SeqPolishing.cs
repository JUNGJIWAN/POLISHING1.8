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
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.ActuatorId;
using static WaferPolishingSystem.Define.UserClass;
using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace WaferPolishingSystem.Unit
{
	
	public class SeqPolishing
    {

		//Timer
		TOnDelayTimer m_tMainCycle      = new TOnDelayTimer();
        TOnDelayTimer m_tToStart        = new TOnDelayTimer();
        TOnDelayTimer m_tToStop         = new TOnDelayTimer();

        TOnDelayTimer m_tPolishCycle    = new TOnDelayTimer();
        TOnDelayTimer m_tInspectCycle   = new TOnDelayTimer();
        TOnDelayTimer m_tUtilityCycle   = new TOnDelayTimer();
		TOnDelayTimer m_tDISupplyCycle  = new TOnDelayTimer();
		TOnDelayTimer m_tDrainTime      = new TOnDelayTimer();
		TOnDelayTimer m_tSeqDrainTime   = new TOnDelayTimer();

		TOnDelayTimer m_tHome           = new TOnDelayTimer();

		TOnDelayTimer m_tDelayTime      = new TOnDelayTimer();
		TOnDelayTimer m_tSuckBackDelay  = new TOnDelayTimer();
		TOnDelayTimer m_tDrainDelay     = new TOnDelayTimer();
		TOnDelayTimer m_tDelayTime1     = new TOnDelayTimer();
		TOnDelayTimer m_tDelayTime2     = new TOnDelayTimer();
		TOnDelayTimer m_tSupplyRetry	= new TOnDelayTimer();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Internal Vars.
		bool   m_bToStart             ; //To... Flag.
		bool   m_bToStop              ;
		//bool   m_bWorkEnd             ;
						    	      
		bool   m_bDrngInspect         ; //Inspect Check
		bool   m_bDrngPolishing       ; //Polishing Check
		bool   m_bDrngUtility         ; //Utility Check
		bool   m_bDrngWait            ; //Wait 
		bool   m_bDrngDrain           ;
		bool   m_bReqDrain            ;
		bool   m_bDrngSeqDrain        ;
		bool   m_bDrngSeqSuckBack     ;
		bool   m_bTESTMODE            ; //TEST Mode Flag
								      
		bool   m_bRetryPOL_TI         ;
		bool   m_bRetryPOL_TH 	      ;
		bool   m_bUtilRequested       ; //
		bool   m_bSlurryReqRetryFlag  ;
		bool   m_bChkWritedData_S     ; //Check Slurry Writed Data - Slurry

		EN_UTIL_STATE  m_enUtilSate; //Utility Exist > Drain 동작 시 OFF

		int    m_nSeqStep        ; //Step.
		int    m_nManStep        ;
		int    m_nHomeStep       ;
		int    m_nSuckBackStep   ;
		//int    m_nSeqDrainStep   ;
		//int    m_nSelSlury       ; //Selected Slurry no

		EN_UTIL_KIND m_enUtilKind; 

		int    m_nInspectStep    ; //Inspect Step
		int    m_nPolishingStep  ; //Polishing Step
		int    m_nUtilityStep    ; //Utility Step
		int    m_nDISupplyStep   ; //DI Supply Step
		int    m_nDrainStep      ; //Drain Step
		int    m_nReqDrainStep   ; //Req Drain
		int    m_nUtilRetryCnt   ; //Utility Retry Count

		double m_dDirectPosn     ; //Direct Moving Position.
		double m_dTHPos          ; //Enc. Motor Position
		double m_dTIPos          ; //
        double m_dYPos           ;

        string sTemp         = string.Empty;
		string sLogMsg       = string.Empty; //for Log
		string m_sLogMoveEvt = string.Empty; 
		string m_sSeqMsg     = string.Empty; 

		int           m_nPartId   ;
		EN_MOTR_ID    m_iMotrYId  ;
		EN_MOTR_ID    m_iMotrTHId ;
		EN_MOTR_ID    m_iMotrTIId ;

		bool   m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        int    m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;
        double m_dSpare1, m_dSpare2, m_dSpare3, m_dSpare4, m_dSpare5;

		//
		const int SLURRY1 = 0;
		const int SLURRY2 = 1;
		const int SLURRY3 = 2;


		EN_UTIL_KIND m_enLastUtil = new EN_UTIL_KIND();


		//---------------------------------------------------------------------------
		//Property
		public int _nManStep            { get { return m_nManStep        ; } set { m_nManStep       = value; } }
		public int _nHomeStep           { get { return m_nHomeStep       ; } set { m_nHomeStep      = value; } }
		public int _nSeqStep            { get { return m_nSeqStep        ; } }
								        
		public bool _bDrngInspect       { get { return m_bDrngInspect    ; } }
        public bool _bDrngPolishing     { get { return m_bDrngPolishing  ; } }
        public bool _bDrngUtility       { get { return m_bDrngUtility    ; } }
        public bool _bDrngWait          { get { return m_bDrngWait       ; } }
		public bool _bDrngDrain         { get { return m_bDrngDrain      ; } }
		public bool _bDrngSeqDrain      { get { return m_bDrngSeqDrain   ; } }
		public bool _bDrngSeqSuckBack   { get { return m_bDrngSeqSuckBack; } }
									    
									    
		public bool _bReqDrain          { get { return m_bReqDrain; }  set { m_bReqDrain = value; } }
									    
		public bool _bUitlUnkown        { get { return m_enUtilSate == EN_UTIL_STATE.Unknown; } }
		public bool _bExtUitl           { get { return m_enUtilSate == EN_UTIL_STATE.Exist  ; } }
		public bool _bEmptyUitl         { get { return m_enUtilSate == EN_UTIL_STATE.Empty  ; } }

		public EN_UTIL_KIND _enLastUtil { get { return m_enLastUtil;  } }




		/************************************************************************/
		/* 생성자.                                                               */
		/************************************************************************/
		public SeqPolishing()
        {

            //
            Init();

            //
            m_nPartId   = (int)EN_SEQ_ID.POLISH;
					    
            m_iMotrYId  = EN_MOTR_ID.miPOL_Y ;
            m_iMotrTHId = EN_MOTR_ID.miPOL_TH;
            m_iMotrTIId = EN_MOTR_ID.miPOL_TI;
        
		}


        //---------------------------------------------------------------------------
        private void Init()
        {
			m_bToStart            = false;
			m_bToStop             = false;
			//m_bWorkEnd            = false;
							      
			m_bDrngInspect        = false;
			m_bDrngPolishing      = false;
			m_bDrngUtility        = false;
			m_bDrngWait           = false;
			m_bReqDrain           = false;
			m_bDrngSeqDrain       = false;
							      
			m_bTESTMODE           = false;
							      
            m_bRetryPOL_TI        = false; 
            m_bRetryPOL_TH	      = false;
			m_bUtilRequested      = false;
			m_bSlurryReqRetryFlag = false;
			m_bChkWritedData_S    = false;

			m_nSeqStep       = 0;
			m_nManStep       = 0;
			m_nHomeStep      = 0;

			m_nSuckBackStep  = 0;
			//m_nSeqDrainStep	 = 0;
			//m_nSelSlury      = 0;

			m_nInspectStep   = 0;
			m_nPolishingStep = 0;
			m_nUtilityStep   = 0;
			m_nDISupplyStep  = 0;
			m_nDrainStep     = 0;
			m_nReqDrainStep  = 0;
			m_nUtilRetryCnt  = 0;

			m_dDirectPosn    = 0;
			m_dTHPos         = 0;
			m_dTIPos         = 0;
			m_dYPos          = 0;

            //Timer Clear
            m_tMainCycle    .Clear();
			m_tToStart      .Clear();
			m_tToStop       .Clear();
			m_tPolishCycle  .Clear();
			m_tInspectCycle .Clear();
			m_tUtilityCycle .Clear();
			m_tDISupplyCycle.Clear();
			m_tDelayTime    .Clear();
			m_tDelayTime1   .Clear();

			m_tHome         .Clear();

			m_enUtilSate = EN_UTIL_STATE.Unknown;
			m_enUtilKind = EN_UTIL_KIND.none    ;

			//
			m_bSpare1 = m_bSpare2 = m_bSpare3 = m_bSpare4 = m_bSpare5 = false;
            m_nSpare1 = m_nSpare2 = m_nSpare3 = m_nSpare4 = m_nSpare5 = 0;
            m_dSpare1 = m_dSpare2 = m_dSpare3 = m_dSpare4 = m_dSpare5 = 0.0;


        }
        //---------------------------------------------------------------------------
        public void fn_Reset()
		{
			m_bToStart            = false; 
			m_bToStop             = false;
			//m_bWorkEnd            = false;
			m_bTESTMODE           = false;
							      
			m_bDrngInspect        = false;
			m_bDrngPolishing      = false;
			m_bDrngUtility        = false;
			m_bDrngWait           = false;
							      
			m_bReqDrain           = false; //Request Drain 
							      
			m_bTESTMODE           = false;
			m_bSlurryReqRetryFlag = false;
			m_bChkWritedData_S    = false;

			m_nSeqStep            = 0; 
			m_nManStep            = 0; 
			m_nHomeStep           = 0;
							      
			m_nInspectStep        = 0;
			m_nPolishingStep      = 0;
			m_nUtilityStep        = 0;
			m_nDISupplyStep       = 0;
			m_nDrainStep          = 0;
			//m_nReqDrainStep       = 0;
			m_nUtilRetryCnt       = 0;
							      
			m_dDirectPosn         = 0.0; 
							      
			m_sSeqMsg             = string.Empty; 

			//Timer Clear
			m_tMainCycle.Clear();
			m_tToStart  .Clear();
			m_tToStop   .Clear();
			m_tHome     .Clear();


			//
			fn_StopAllUtil      ();
			fn_SetDIWaterValve  ();
			fn_SetDrainValve    ();
			fn_SetSuckBackOff   ();
			fn_SetSeperator		();
			fn_SetSeperatorBlow (); //LEE/200422/

		}
		//---------------------------------------------------------------------------
		public double GetEncPos_Y () { return MOTR.GetEncPos(m_iMotrYId ); }
		public double GetEncPos_TH() { return MOTR.GetEncPos(m_iMotrTHId); }
		public double GetEncPos_TI() { return MOTR.GetEncPos(m_iMotrTIId); }

		public double GetCmdPos_Y () { return MOTR.GetCmdPos(m_iMotrYId ); }
		public double GetCmdPos_TH() { return MOTR.GetCmdPos(m_iMotrTHId); }
		public double GetCmdPos_TI() { return MOTR.GetCmdPos(m_iMotrTIId); }

		public double GetTrgPos_Y () { return MOTR.GetTrgPos(m_iMotrYId ); }
        public double GetTrgPos_TH() { return MOTR.GetTrgPos(m_iMotrTHId); }
		public double GetTrgPos_TI() { return MOTR.GetTrgPos(m_iMotrTIId); }
		//---------------------------------------------------------------------------
		private bool CheckDstb(EN_MOTR_ID Motr, EN_COMD_ID Cmd = EN_COMD_ID.NoneCmd,
                                    int Step = NONE_STEP, EN_FPOSN_INDEX FIndex = EN_FPOSN_INDEX.NONE, double DirPosn = 0.0)
        {
			//Var.
			bool isNoRun        = SEQ.fn_IsNoRun() && m_nManStep == 0 ;
			bool isOpenDoor     = SEQ.fn_IsAnyDoorOpen(); 
					    
			double dEnc_Y       = GetEncPos_Y ();
			double dEnc_TH      = GetEncPos_TH();
			double dEnc_TI      = GetEncPos_TI();
					    
			double dCmd_Y       = GetCmdPos_Y ();
			double dCmd_TH      = GetCmdPos_TH();
			double dCmd_TI      = GetCmdPos_TI();
					    
			double dTrg_Y       = GetTrgPos_Y ();
			double dTrg_TH      = GetTrgPos_TH();
			double dTrg_TI      = GetTrgPos_TI();


			double dEnc_SPDX    = MOTR.GetEncPos(EN_MOTR_ID.miSPD_X );
			double dEnc_SPDZ    = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z );
			double dEnc_SPDZ1   = MOTR.GetEncPos(EN_MOTR_ID.miSPD_Z1);

			double dMaxXAxisPos  = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLX_POS_DSTB_POLIZ ].dPos;
			double dMinZAxisPos  = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ_POS_MOVE_BTMY  ].dPos;
			double dMinZ1AxisPos = MOTR.DstbPosn[(int)EN_DSTB_ID.DP_SPDLZ1_POS_MOVE_BTMY ].dPos;
			double dNextPosn     = 0;							

			if (Motr != EN_MOTR_ID.miPOL_TH && Motr != EN_MOTR_ID.miPOL_TI && Motr != EN_MOTR_ID.miPOL_Y) return false;

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
			//if (Motr == EN_MOTR_ID.miPOL_Y)
			if (Motr == EN_MOTR_ID.miPOL_Y || Motr == EN_MOTR_ID.miPOL_TI || Motr == EN_MOTR_ID.miPOL_TH)  //JUNG/200624
			{
				//Z-Axis Home End Check
				bool bHomeEndZ  = MOTR[(int)EN_MOTR_ID.miSPD_Z ].GetHomeEnd() && MOTR[(int)EN_MOTR_ID.miSPD_Z ].GetServo() && MOTR[(int)EN_MOTR_ID.miSPD_Z].GetHomeEndDone();
				bool bHomeEndZ1 = MOTR[(int)EN_MOTR_ID.miSPD_Z1].GetHomeEnd() && MOTR[(int)EN_MOTR_ID.miSPD_Z1].GetServo() && MOTR[(int)EN_MOTR_ID.miSPD_Z].GetHomeEndDone();

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

            }


            //
            if (Motr == EN_MOTR_ID.miPOL_TI)
            {

            }
            
			//
            if (Motr == EN_MOTR_ID.miPOL_TH)
            {

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
		public bool fn_ReqMoveUtilChkPosn()
		{
			bool r1 = fn_MoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User5);
			bool r2 = fn_ReqBathWaitPosTHTI();

			//Ok.
			return (r1 && r2);
		}
        //---------------------------------------------------------------------------
        public bool fn_ReqMoveForceTestPosn()
        {
            bool r1 = fn_MoveMotr(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.User1);
			bool r2 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.Wait1);

			//Ok.
			return (r1 && r2);
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
					//Spindle Z, Z1 Home End 시까지 대기...
					if (!CheckDstb(EN_MOTR_ID.miPOL_Y, EN_COMD_ID.Home)) return false;

					IO.fn_RunBuffer((int)EN_MOTR_ID.miPOL_Y , false);
					IO.fn_RunBuffer((int)EN_MOTR_ID.miPOL_TI, false);
					IO.fn_RunBuffer((int)EN_MOTR_ID.miPOL_TH, false);

					//Clear Home end Flag
					MOTR.ClearHomeEnd(EN_MOTR_ID.miPOL_Y );
                    //MOTR.ClearHomeEnd(EN_MOTR_ID.miPOL_TH);
                    MOTR.ClearHomeEnd(EN_MOTR_ID.miPOL_TI);

					m_bRetryPOL_TI = false;
					m_bRetryPOL_TH = false;

					m_nHomeStep++;
					return false;

                case 11: //

                    EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_Y , true);
					//EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, true);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TI, true);

					m_nHomeStep++;
                    return false;

                case 12: //Buffer Run

					r1 = IO.fn_RunBuffer(BFNo_01_HOME_POL_Y , true);
                    r2 = IO.fn_RunBuffer(BFNo_05_HOME_POL_TI, true);

                    if (!r1 || !r2 )
                    {
                        fn_UserMsg(string.Format($"ACS RUN Buffer Error - {BFNo_01_HOME_POL_Y:D2}:{r1.ToString()} / {BFNo_05_HOME_POL_TI:D2}:{r2.ToString()} "));
                        m_nHomeStep = 0;
                        return true;
                    }
                    m_nHomeStep++;
                    return false;

                case 13: //Check Buffer Run 
                    r1 = IO.fn_IsBuffRun(BFNo_01_HOME_POL_Y );
                    r2 = IO.fn_IsBuffRun(BFNo_05_HOME_POL_TI);
					if (!r1 || !r2) return false;
                    m_nHomeStep++;
                    return false;

                case 14:
					//Check Error
                    r1 = IO.DATA_ACS_TO_EQ[BFNo_01_HOME_POL_Y] == 1;
					if(r1)
					{
						fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_01_HOME_POL_Y:D2}");
						m_nHomeStep = 0;
                        return true;
                    }
                    //                     if (IO.DATA_ACS_TO_EQ[BFNo_04_HOME_POL_TH] == 1)
                    //                     {
                    //                         fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_04_HOME_POL_TH:D2}");
                    //                         m_nHomeStep = 0;
                    //                         return true;
                    //                     }
					if (IO.DATA_ACS_TO_EQ[BFNo_05_HOME_POL_TI] == 1 && !m_bRetryPOL_TI)
					{
						m_bRetryPOL_TI = true;

						//fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_05_HOME_POL_TI:D2}");
						//m_nHomeStep = 0;
						//return true;
					}

                    //Check Buffer End
                    r1 = IO.fn_IsBuffRun(BFNo_01_HOME_POL_Y);
                    r2 = IO.fn_IsBuffRun(BFNo_05_HOME_POL_TI);

					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_Y , r1);
					//EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, r3);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TI, r2);

					if (r1 || r2 ) return false;

					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_Y , false);
					//EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, false);
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TI, false);

					MOTR[(int)EN_MOTR_ID.miPOL_Y ].SetHomeEndDone(true);
					MOTR[(int)EN_MOTR_ID.miPOL_TI].SetHomeEndDone(true);

					m_nHomeStep++;
                    return false;


                case 15: //

					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, true);

					m_nHomeStep++;
					return false;

				case 16: //Buffer Run
					r1 = IO.fn_RunBuffer(BFNo_04_HOME_POL_TH, true);
					if (!r1)
					{
						fn_UserMsg(string.Format($"ACS RUN Buffer Error - {BFNo_04_HOME_POL_TH:D2}:{r1.ToString()}"));
						m_nHomeStep = 0;
						return true;
					}
					m_nHomeStep++;
					return false;

				case 17: //Check Buffer Run 
					r1 = IO.fn_IsBuffRun(BFNo_04_HOME_POL_TH);
					if (!r1) return false;
					m_nHomeStep++;
					return false;

				case 18:
					//Check Error
					if (IO.DATA_ACS_TO_EQ[BFNo_04_HOME_POL_TH] == 1 && !m_bRetryPOL_TH)
					{
						m_bRetryPOL_TH = true;

						//fn_UserMsg($"Home FAIL!!! : Motor No - {BFNo_04_HOME_POL_TH:D2}");
						//m_nHomeStep = 0;
						//return true;
					}

					//Check Buffer End
					r1 = IO.fn_IsBuffRun(BFNo_04_HOME_POL_TH) && !m_bRetryPOL_TH;
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, r1);
					if (r1) return false;
					
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, false);

					MOTR[(int)EN_MOTR_ID.miPOL_TH].SetHomeEndDone(true);

					if (m_bRetryPOL_TH || m_bRetryPOL_TI)
					{
						Console.WriteLine("if (m_bRetryPOL_TH || m_bRetryPOL_TI)");

						m_nHomeStep = 20;
						return false;
					}
                    m_nHomeStep++;
                    return false;

                case 19:
					r1 = fn_MoveMotr(m_iMotrYId , EN_COMD_ID.Wait1);
					r2 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.Wait1);
                    r3 = fn_MoveMotr(m_iMotrTIId, EN_COMD_ID.Wait1);

					if (!r1 || !r2 || !r3) return false; 
					
                    





                    m_nHomeStep = 0;
					return true;

				case 20:
					if (m_bRetryPOL_TI)
					{
						EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TI, true);
						IO.fn_RunBuffer(BFNo_05_HOME_POL_TI, true);
					}
					else
					{
                        m_nHomeStep=23;
                        return false;
                    }
                    m_nHomeStep ++;
                    return false;
				
				case 21:
                    r1 = IO.fn_IsBuffRun(BFNo_05_HOME_POL_TI);
                    if (!r1) return false;
                    m_nHomeStep++;
                    return false;
                
				case 22:
                    r1 = IO.fn_IsBuffRun(BFNo_05_HOME_POL_TI);
                    if (r1) return false;
					
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TI, false);
					
					m_nHomeStep++;
                    return false;

                case 23:
                    
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, true);
					IO.fn_RunBuffer(BFNo_04_HOME_POL_TH, true);
                    
                    m_nHomeStep++;
                    return false;

                case 24:
                    r1 = IO.fn_IsBuffRun(BFNo_04_HOME_POL_TH);
                    if (!r1) return false;
                    m_nHomeStep++;
                    return false;

                case 25:
                    r1 = IO.fn_IsBuffRun(BFNo_04_HOME_POL_TH);
                    if (r1) return false;
					EPU.fn_SetErr(iFHomeErr + (int)EN_MOTR_ID.miPOL_TH, false);
					
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

	
			return true;
		}
		//---------------------------------------------------------------------------
		public bool fn_ToStopCon()
		{
			m_bToStop = false; 
			m_tToStop.Clear();

			//Check Step
			if (m_nSeqStep        != 0) return false;

			if (m_nInspectStep    != 0) return false;
			if (m_nPolishingStep  != 0) return false;
			if (m_nUtilityStep    != 0) return false;
			if (m_nDISupplyStep   != 0) return false;
			if (m_nDrainStep      != 0) return false;

			//Check During Flag
			if (m_bDrngInspect        ) return false; 
			if (m_bDrngPolishing      ) return false; 
			if (m_bDrngUtility        ) return false; 
			if (m_bDrngWait           ) return false;
			if (m_bDrngSeqDrain       ) return false; //JUNG/200613

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
			m_tMainCycle    .Clear();
						    
			m_tPolishCycle  .Clear();
			m_tInspectCycle .Clear();
			m_tUtilityCycle .Clear();
			m_tDISupplyCycle.Clear();

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

			//Check Start Time Out
			if (m_tToStop.OnDelay(!m_bToStop, 20 * 1000))
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0160 + m_nPartId, true);
				return false;
			}

			//Clear Step Index
			m_nSeqStep       = 0; 

			m_nInspectStep   = 0;
			m_nPolishingStep = 0;
			m_nUtilityStep   = 0;
			m_nDISupplyStep  = 0;
			m_nDrainStep     = 0;

			m_bTESTMODE      = false;


			fn_StopAllUtil      ();
			fn_SetDIWaterValve  ();
			fn_SetDrainValve    ();
			fn_SetSuckBackOff   ();
			fn_SetSeperator		();
			fn_SetSeperatorBlow (); //LEE/200422/

			m_bToStop = true;

			return true;
		}
        //---------------------------------------------------------------------------
        public bool fn_MoveCylClamp(int act)
		{
			//
			bool r1 = ACTR.MoveCyl(EN_ACTR_LIST.aPoli_Clamp, act);

			return r1; 
		}
		//---------------------------------------------------------------------------
		public bool fn_AutoRun()
		{
			m_dYPos  = MOTR[(int)EN_MOTR_ID.miPOL_Y ].GetCmdPos();
            m_dTHPos = MOTR[(int)EN_MOTR_ID.miPOL_TH].GetCmdPos();
			m_dTIPos = MOTR[(int)EN_MOTR_ID.miPOL_TI].GetCmdPos();

			bool bErr        = EPU._bIsErr;
			bool bManRun     = FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE );
			     m_bTESTMODE = FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE);

			//Time Check
			m_tMainCycle    .OnDelay((m_nSeqStep        != 0 && !bErr && !bManRun), 120 * 1000); //120sec

			m_tPolishCycle  .OnDelay((m_nPolishingStep  != 0 && !bErr && !bManRun),  60 * 1000); //
			m_tInspectCycle .OnDelay((m_nInspectStep    != 0 && !bErr && !bManRun),  60 * 1000); //
			m_tUtilityCycle .OnDelay((m_nUtilityStep    != 0 && !bErr && !bManRun), (60 + FM.m_stMasterOpt.nUtilMaxTime) * 1000); //
			m_tDISupplyCycle.OnDelay((m_nDISupplyStep   != 0 && !bErr && !bManRun), (60 + FM.m_stMasterOpt.nUtilMaxTime) * 1000); //
			m_tDrainTime    .OnDelay((m_nDrainStep      != 0 && !bErr && !bManRun),  100 * 1000); //


			//
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0170 + m_nPartId, m_tMainCycle.Out))
			{
				sLogMsg = string.Format($"[Polishing] Main Cycle Time Out : m_iSeqStep = {m_nSeqStep}");
			}
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0190, m_tPolishCycle.Out))
			{
				sLogMsg = string.Format($"[Polishing] Vision Cycle Time Out : m_iToolPickStep = {m_nPolishingStep}");
			}

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0191, m_tInspectCycle.Out))
            {
                sLogMsg = string.Format($"[Polishing] Polishing Cycle Time Out : m_iToolChkStep = {m_nInspectStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0192, m_tUtilityCycle.Out))
            {
                sLogMsg = string.Format($"[Polishing] Utility Cycle Time Out : m_nUtilityStep = {m_nUtilityStep}");
            }

            if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0193, m_tDrainTime.Out))
            {
                sLogMsg = string.Format($"[Polishing] Drain Cycle Time Out : m_iForceChkStep = {m_nDrainStep}");
            }
            
			if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0192, m_tDISupplyCycle.Out))
            {
                sLogMsg = string.Format($"[Polishing] DI Suplly Cycle Time Out : m_nDISupplyStep = {m_nDISupplyStep}");
            }


            //
            if (m_tMainCycle     .Out ||
				m_tPolishCycle   .Out ||
				m_tInspectCycle  .Out ||
				m_tDrainTime     .Out ||
				m_tDISupplyCycle .Out ||
				m_tUtilityCycle  .Out )
			{
				fn_WriteLog(sLogMsg);
				LOG.fn_CrntStateTrace(EN_SEQ_ID.POLISH, sLogMsg);
				fn_Reset();
				return false; 
			}


			//Emergency Error Check
			bool bEMOErr = EPU.fn_IsEMOError();  // Emergency Error
			if (m_nSeqStep != 0 && bEMOErr)
			{
				//
				sLogMsg = string.Format($"[EMO][SEQ_POLI] Force Cycle End m_nSeqStep = {m_nSeqStep}");
				fn_WriteLog(sLogMsg);
				
				fn_Reset();
				
				m_nSeqStep = 0;
				return false; 
			}

			//Decide Step
			if (m_nSeqStep == 0)
			{
				//Var
				bool bPoli_YWaitPos	= MOTR.CmprPosByCmd(m_iMotrYId , EN_COMD_ID.Wait1);

				//
				bool bPolishEmpty     = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllEmpty();
				bool isCleanMap       = DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsStatOne(EN_PLATE_STAT.ptsClean);
				bool bPolishFinish    = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsFinish);
				bool bPolishAlign     = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsAlign );
				bool bPolishReady     = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolish);
				bool bPolishClean     = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsClean );
				bool bLoadReady       = DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllStat((int)EN_PLATE_STAT.ptsLoad  );
				bool bLoadFinish      = DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllStat((int)EN_PLATE_STAT.ptsFinish);
				bool bLoadEmpty       = DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllStat((int)EN_PLATE_STAT.ptsEmpty );
				bool bTransferEmpty   = DM.MAGA[(int)EN_MAGA_ID.TRANS ].IsAllStat((int)EN_PLATE_STAT.ptsEmpty );
				bool isLoadEmpty      = bLoadFinish || bLoadEmpty;

				bool bSpindleLoad     = DM.TOOL.IsPlateAllStat((int)EN_PLATE_STAT.ptsLoad);
				bool bToolPlateEmpty  = DM.TOOL.IsPlateEmpty(); //IsPlateStat(0, (int)EN_PLATE_STAT.ptsLoad);
				bool bToolEmpty       = DM.TOOL.IsAllEmpty  ();
				bool bToolExtPol      = DM.TOOL.IsExistPol  ();
				bool m_bNeedUtil      = m_enUtilSate == EN_UTIL_STATE.Empty ;
				bool bNeedToolChk     = DM.TOOL.IsNeedCheck();
				bool bUseUtilDI       = !m_bTESTMODE; 
				bool isDrngPolishing  = SEQ_SPIND._bDrngPolishing;
				bool bCheckForce      = SEQ_SPIND.fn_IsCheckForce();

				//Step Condition
				bool isConUtiDI       =  bPolishEmpty && bToolPlateEmpty && bToolEmpty      &&  m_bNeedUtil     && !bNeedToolChk  &&
										!isCleanMap   && isLoadEmpty     && bTransferEmpty  &&  bUseUtilDI                    ;
				bool isConUtiSlurry   =  bPolishReady && bToolExtPol     && m_bNeedUtil     && !isDrngPolishing && bCheckForce;
				
				bool isConDrain       =  m_bReqDrain   || (!m_bNeedUtil  &&  bPolishClean) || (bSpindleLoad && !m_bNeedUtil);
				bool isConDrain1      = (bPolishFinish ||  bPolishAlign) && !SEQ_POLIS._bEmptyUitl && !m_bDrngSeqDrain;

				bool isConWait        = false;// !isConInspect && !isConPolishing && !isConUtility && !bPoli_YWaitPos;

				//Clear Var.
				m_bDrngInspect        = false;
				m_bDrngPolishing      = false;
				m_bDrngUtility        = false;
				m_bDrngDrain          = false;
				m_bDrngWait           = false;

				//Step Clear
				m_nInspectStep        = 0;
				m_nPolishingStep      = 0;
				m_nUtilityStep	      = 0;
				m_nDrainStep          = 0;
				
				m_sSeqMsg             = string.Empty; 

				//Check Sequence Stop
				if ( SEQ._bStop                                          ) return false; 
				if ( EPU.fn_GetHasErr()                                  ) return false; 
				if (!SEQ._bRun && !FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE)) return false; 

				//
// 				if (isConInspect     ) { m_bDrngInspect    = true; m_nSeqStep = 100 ; m_nInspectStep    = 10; m_sSeqMsg = ""             ; goto __GOTO_CYCLE__; }
// 				if (isConPolishing   ) { m_bDrngPolishing  = true; m_nSeqStep = 200 ; m_nPolishingStep  = 10; m_sSeqMsg = ""             ; goto __GOTO_CYCLE__; }
				if (isConUtiDI       ) { m_bDrngUtility    = true; m_nSeqStep = 300 ; m_nDISupplyStep   = 10; m_sSeqMsg = "Supply DI"    ; goto __GOTO_CYCLE__; }
				if (isConUtiSlurry   ) { m_bDrngUtility    = true; m_nSeqStep = 500 ; m_nUtilityStep	= 10; m_sSeqMsg = "Supply Slurry"; goto __GOTO_CYCLE__; }
				if (isConDrain       ) { m_bDrngDrain      = true; m_nSeqStep = 400 ; m_nDrainStep      = 10; m_sSeqMsg = "Drain"        ; goto __GOTO_CYCLE__; }
				if (isConDrain1      ) {                           m_nSeqStep = 600 ;                         m_sSeqMsg = "Drain"        ; goto __GOTO_CYCLE__; }
				if (isConWait        ) { m_bDrngWait       = true; m_nSeqStep = 1300;                         m_sSeqMsg = "Wait"         ; goto __GOTO_CYCLE__; }

			}

			//Cycle Start
			__GOTO_CYCLE__:

			//Cycle
			switch (m_nSeqStep)
			{
// 				case 100:
// 					if (fn_InspectCycle(ref m_bDrngInspect)) m_nSeqStep = 0;
// 					return false;
// 
// 				case 200:
// 					if (fn_PolishingCycle(ref m_bDrngPolishing)) m_nSeqStep = 0;
// 					return false;

				case 300:
					if (fn_UtilityCycle(ref m_bDrngUtility)) m_nSeqStep = 0;
					return false;

				case 400:
					if (fn_DoDrain(ref m_bDrngDrain)) m_nSeqStep = 0;
					return false;

                case 500:
                    if (fn_UtilitySlurryCycle(ref m_bDrngUtility)) m_nSeqStep = 0;
                    return false;

                case 600:
					fn_SetDrain();
					m_nSeqStep = 0;
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
		public string fn_GetSeqMsg()
        {
			string rtn = string.Empty;

			if (m_sSeqMsg == string.Empty) return rtn; 

			rtn = string.Format($"Now {m_sSeqMsg}...");

			return rtn; 
        }

		//---------------------------------------------------------------------------
        private bool fn_UtilityCycle(ref bool Flag)
        {
			// 1) Move Main-X Util Check Position 
			// 2) Check Position
			// 3) Supply Utility while user set level

			bool r1;

			if (m_nDISupplyStep < 0) m_nDISupplyStep = 0;

			switch (m_nDISupplyStep)
			{

				default:
					m_nDISupplyStep = 0;
					return true;


				case 10:
					
					Flag = true; 
 					
					SEQ_SPIND._bReqUtil_Polish = true ;

					fn_WriteSeqLog("[START] Utility(DI) Supply");

					m_tDelayTime.Clear();
					m_nDISupplyStep++;
					return false ;

                case 11:
					if (SEQ._bStop)
					{
						m_nDISupplyStep = 0;
						Flag = false;
						return true;
					}

					if (!m_tDelayTime.OnDelay(true, 300)) return false; 

					if (!SEQ_SPIND._bDrngUtilLevel) return false;

					m_nDISupplyStep++;
                    return false;
                
				case 12:
                    if (SEQ._bStop)
                    {
						m_nDISupplyStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Check Main-X Position
                    r1 = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_X, EN_COMD_ID.User9);
					if (!r1) return false;

					//
					if(m_bTESTMODE)
					{
						m_nDISupplyStep = 14;
                        return false;
                    }


                    m_tDelayTime.Clear();
					m_nDISupplyStep++;
                    return false;

                case 13:
					if (SEQ._bStop)
                    {
						fn_SetDIWaterValve();

						m_nDISupplyStep = 0;
                        Flag = false;
                        return true;
                    }
					if (!m_tDelayTime.OnDelay(true, 1000 * 5)) return false;
                    
					m_tDelayTime.Clear();
					m_nDISupplyStep++;
                    return false;
				
				case 14:

                    //Check Supply time out
                    m_tDelayTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nUtilMaxTime);
                    if (m_tDelayTime.Out)
                    {
						fn_SetDIWaterValve();

						fn_WriteLog(string.Format("DI Supply Timeout"), EN_LOG_TYPE.ltLot);

                        EPU.fn_SetErr(EN_ERR_LIST.ERR_0417); //Utility Supply Error

						m_nDISupplyStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Supply Utility during check UT Level Sensor
                    if (IO.fn_IsUTLevelDone())
					{
						//Supply Off
						fn_SetDIWaterValve();

						m_nDISupplyStep++;
                        return false;
                    }

                    //Supply On
                    fn_SetDIWaterValve(vvOpen);

					return false;
				
				case 15:
					
					//
					fn_SetDIWaterValve();

					//
					m_enUtilSate = EN_UTIL_STATE.Exist;

					fn_WriteSeqLog("[END] Utility(DI) Supply Done");

					//
					Flag = false;
					m_nDISupplyStep = 0;
                    return true;
            }
        }
		//-------------------------------------------------------------------------------------------------
		//bool bTest; 
		private bool fn_UtilitySlurryCycle(ref bool Flag)
        {
			// 1) Move Main-X Util Check Position 
			// 2) Check Position
			// 3) Supply Utility while user set level

			bool r1, r2, r3;
			//EN_UTIL_KIND enRcpUtilKind = EN_UTIL_KIND.Silica01;  //Set Recipe Utility 
			EN_UTIL_KIND enRcpUtilKind = (EN_UTIL_KIND)SEQ_SPIND.vresult.stRecipeList[0].nUtilType; //JUNG/200526

			if (m_nUtilityStep < 0) m_nUtilityStep = 0;

			switch (m_nUtilityStep)
			{

				default:
					m_nUtilityStep = 0;
					return true;

				case 10:

					Flag = true; 
					
					fn_WriteSeqLog("[START] Supply Slurry");
					fn_WriteLog(string.Format($"[START] Supply Slurry - TYPE : {enRcpUtilKind.ToString()}"), EN_LOG_TYPE.ltLot);


                    //JUNG/200902
                    if (m_bTESTMODE)
                    {
                        m_nUtilityStep = 17;
                        return false;
                    }


                    //SuckBack
                    fn_SetSuckBackOn(enRcpUtilKind);

					m_bUtilRequested      = false;
					m_bSlurryReqRetryFlag = false;
					m_bChkWritedData_S    = false;

					m_nUtilRetryCnt = 0; 

					m_tDelayTime1.Clear(); 
					m_tDelayTime2.Clear();

					m_tDelayTime.Clear();
					m_nUtilityStep++;
					return false ;

                case 11:
					if (SEQ._bStop)
					{
						m_nUtilityStep = 0;
						Flag = false;
						return true;
					}

					if (m_bDrngSeqSuckBack && !m_bTESTMODE) return false;
                    
					m_nUtilityStep++;
                    return false;
				
				case 12: 

                    SEQ_SPIND._bReqUtil_Polish = true;

					m_tDelayTime.Clear();
					m_nUtilityStep++;
                    return false;
				
				case 13:
                    if (SEQ._bStop)
                    {
                        m_nUtilityStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 500)) return false; 

					if (!SEQ_SPIND._bDrngUtilLevel) return false; 

					m_nUtilityStep++;
                    return false;
                
				case 14:
                    if (SEQ._bStop)
                    {
                        m_nUtilityStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Check Main-X Position
                    r1 = MOTR.CmprPosByCmd(EN_MOTR_ID.miSPD_X, EN_COMD_ID.User9);
					r2 = m_bDrngSeqSuckBack && !m_bTESTMODE; //JUNG/200514/Check Suck back 
					r3 = fn_ReqMoveUtilChkPosn(); 
					if (!r1 || r2 || !r3) return false;

					//
                    IO.fn_ClearUTQue();

					m_tDelayTime.Clear();
					m_nUtilityStep++;
                    return false;

                case 15:
                    if (SEQ._bStop)
                    {
                        m_nUtilityStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime.OnDelay(true, 1000 * 3)) return false;

					fn_WriteLog(string.Format("Valve On at Utility cycle"), EN_LOG_TYPE.ltLot);
					
					m_tSupplyRetry.Clear();
					m_tDelayTime  .Clear();
                    m_nUtilityStep++;
                    return false;
				
				case 16:
                    if (SEQ._bStop)
                    {
						fn_StopAllUtil();

						m_nUtilityStep = 0;
                        Flag = false;
                        return true;
                    }

                    if (!m_tDelayTime2.OnDelay(true, 300)) return false;

					//Check Supply time out
					m_tDelayTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nUtilMaxTime); 
					if (m_tDelayTime.Out)
					{
						fn_StopAllUtil();

						sTemp = string.Format($"Slurry Supply Timeout - {FM.m_stMasterOpt.nUtilMaxTime} sec");
						fn_WriteLog(sTemp, EN_LOG_TYPE.ltLot);
						fn_WriteLog(sTemp);
						
						//JUNG/201119/Auto Supply System Error Check
						if(!m_bChkWritedData_S) EPU.fn_SetErr(EN_ERR_LIST.ERR_0418); //Auto Supply System Reply Error
						else                    EPU.fn_SetErr(EN_ERR_LIST.ERR_0416); //Utility Supply Error
                        
						m_nUtilityStep = 0;
                        Flag = false;
                        return true;
                    }

                    //Supply Utility during check UT Level Sensor
                    if (IO.fn_IsUTLevelDone())
					{
						//Supply Off
						fn_StopAllUtil();

						fn_WriteLog(string.Format("Utility Supply OK"), EN_LOG_TYPE.ltLot);

						m_nUtilityStep++;
                        return false;
                    }

					fn_ReqMoveUtilChkPosn();


					//Signal Check 
					//if (!SUPPLY[SPLY_SLURRY]._bSlurryReqState && (SUPPLY[SPLY_SLURRY]._bSlurryReq || m_bUtilRequested))
					//if ((!SUPPLY[SPLY_SLURRY]._bSlurryReqState && SUPPLY[SPLY_SLURRY]._bSlurryReq) || m_bUtilRequested)
					//{
					//	m_bUtilRequested = true;
					//	fn_SetUtilSignal(enRcpUtilKind, false);
					//
					//	//
					//	if (!m_tDelayTime1.OnDelay(true, 500)) return false ;
					//	if (SUPPLY[SPLY_SLURRY]._bSlurryReq  ) return false ;
					//
					//	m_bUtilRequested = false;
					//	m_tDelayTime2.Clear();
					//
					//	sTemp = string.Format($"Slurry Supply Retry");
					//
					//	Console.WriteLine(sTemp);
					//}

					//Check Slurry Writed Data
					if (SUPPLY[0]._ReadData[0] == 1) m_bChkWritedData_S = true; 

					if (!SUPPLY[0]._bSlurryReqState && !SUPPLY[0]._bSlurryReq)
                    {
                        m_bSlurryReqRetryFlag = false;
						m_tSupplyRetry.Clear();
					}

					m_tSupplyRetry.OnDelay(true, 1000 * 3); //5sec
					if (m_tSupplyRetry.Out)
                    {
						m_nUtilRetryCnt++;
						sLogMsg = "UtilRetryCnt =" + m_nUtilRetryCnt;
						Console.WriteLine(sLogMsg);
						fn_WriteLog(sLogMsg, EN_LOG_TYPE.ltLot); //JUNG/201027

						SUPPLY[0]._bSlurryReq = false;
                        m_bSlurryReqRetryFlag = true ;
                        m_tSupplyRetry.Clear();

                        return false;
                    }

                    if (m_bSlurryReqRetryFlag) return false;

                    fn_SupplyUtil(enRcpUtilKind);
					
					m_tDelayTime1.Clear();
					m_bUtilRequested = false;

					return false;
				
				case 17:

					//
					fn_StopAllUtil    ();
					fn_SetDIWaterValve();

					//
					m_enUtilSate = EN_UTIL_STATE.Exist;

					fn_WriteLog("[END] Supply Slurry" , EN_LOG_TYPE.ltLot);
					fn_WriteSeqLog("[END] Supply Slurry");

					//
					Flag = false; 
                    m_nUtilityStep=0;
                    return true;
            }
        }

        //---------------------------------------------------------------------------
        public bool fn_DoDrain(ref bool Flag)
        {
			if (m_nDrainStep < 0) m_nDrainStep = 0;

			switch (m_nDrainStep)
			{

				default:
					m_nDrainStep = 0;
					return true;

				case 10:
					
					Flag = true;
					m_bReqDrain = false;
					
					fn_WriteSeqLog("[START] Drain");

					fn_SetDrain();

					m_tDelayTime.Clear();
					m_nDrainStep++;
					return false ;

                case 11:
                    if (m_bDrngSeqDrain) return false; //

					m_tDelayTime.Clear();
					m_nDrainStep++;
                    return false;
                
				case 12:

					//JUNG/200902
					if(m_bTESTMODE)
					{
						m_enUtilSate = EN_UTIL_STATE.Empty;
					}

					fn_WriteSeqLog("[END] Drain");

					Flag = false;
					m_nDrainStep = 0;
                    return true;
            }


        }
		//---------------------------------------------------------------------------
		public void fn_SetDrain()
		{
            //JUNG/200717/Test Mode
            if (m_bTESTMODE && SEQ._bRun) return;
            if (m_bDrngSeqDrain         ) return; 
			if (SEQ_CLEAN._bDrngSeqDrain) return; 


			m_nReqDrainStep = 10;
			m_bDrngSeqDrain = true; 

			fn_WriteLog("[Polishing] Set Drain");
		}
		//---------------------------------------------------------------------------
		public bool fn_SeqDrain()
		{
			if (m_nReqDrainStep < 0) m_nReqDrainStep = 0;

			//Drain Sequence
			switch (m_nReqDrainStep)
			{
				case 10:

					//
					if (m_bDrngUtility) return false;

					m_bDrngSeqDrain = true;

					fn_SetSeperator    (vvClose);

					fn_SetSeperatorBlow(vvOpen);

                    m_tSeqDrainTime.Clear();
                    m_nReqDrainStep++;
                    return false;

                case 11:
                    if (!m_tSeqDrainTime.OnDelay(true, 1000)) return false; //
                    
					fn_SetDrainValve(vvOpen);

					m_tSeqDrainTime.Clear();
					m_nReqDrainStep++;
					return false;

				case 12:
					if (!m_tSeqDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nDrainTime)) return false; //10 sec

					fn_SetDrainValve   (vvClose); // 
                    
					m_tSeqDrainTime.Clear();
                    m_nReqDrainStep++;
                    return false;

                case 13:
					if (!m_tSeqDrainTime.OnDelay(true, 500)) return false; //
                    
					fn_SetSeperatorBlow(vvClose); //
					
					m_nReqDrainStep++;
					return false;

				case 14:

					fn_SetSeperator(vvOpen);

					m_tSeqDrainTime.Clear();
					m_nReqDrainStep++;
					return false;

				case 15:
					if (!m_tSeqDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nSepBlowTime)) return false; //3 sec

					fn_SetSeperator(vvClose);

					fn_WriteLog("[Polishing] Drain End");

					//
					m_enUtilSate = EN_UTIL_STATE.Empty;

					m_bDrngSeqDrain = false;

					m_tSeqDrainTime.Clear();
					m_nReqDrainStep = 0;
					return true;

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
		//
		//			//
		//			if (m_bDrngUtility) return false; 
		//
		//			m_bDrngSeqDrain = true;
		//
		//			fn_SetSeperator    (vvClose);
		//			fn_SetSeperatorBlow(vvClose); //
		//
        //          fn_SetDrainValve   (vvOpen );
		//
		//			m_tSeqDrainTime.Clear();
		//			m_nReqDrainStep++;
        //            return false;
		//
        //        case 11:
        //            if (!m_tSeqDrainTime.OnDelay(true, 2000)) return false; //2sec /제조 요청으로 수정
        //            
        //            fn_SetSeperator    (vvClose);
		//
        //            fn_SetSeperatorBlow(vvOpen );
        //            fn_SetDrainValve   (vvOpen );
		//
		//			m_tSeqDrainTime.Clear();
		//			m_nReqDrainStep++;
		//			return false;
		//
        //        case 12:
		//			if (!m_tSeqDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nDrainTime)) return false; //10 sec
		//
		//			fn_SetDrainValve	(vvClose); // 
		//			fn_SetSeperatorBlow	(vvClose); //
		//			m_tSeqDrainTime.Clear();
		//			m_nReqDrainStep++;
        //            return false;
		//
        //        case 13:
		//
		//			fn_SetDrainOut(vvOpen);
		//
		//			if (!m_tSeqDrainTime.OnDelay(true, 1000)) return false; //1 sec
		//
		//			fn_SetSeperator(vvOpen);
		//
		//			m_tSeqDrainTime.Clear();
		//			m_nReqDrainStep++;
        //            return false;
		//		
		//		case 14:
		//			if (!m_tSeqDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nSepBlowTime)) return false; //3 sec
		//
		//			fn_SetSeperator    (vvClose);
		//
        //            m_tSeqDrainTime.Clear();
        //            m_nReqDrainStep++;
        //            return false;
		//
        //        case 15:
		//			if (!m_tSeqDrainTime.OnDelay(true, 1000)) return false; //1 sec
		//
		//			fn_SetDrainOut(vvClose);
		//
		//			fn_WriteLog("[Polishing] Drain End");
		//
		//			//
		//			m_enUtilSate = EN_UTIL_STATE.Empty;
		//
		//
		//			m_bDrngSeqDrain = false;
		//
		//			m_tSeqDrainTime.Clear();
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
        public bool fn_DoDrainOneCycle()
        {
			// 1) Polishing Drain Valve Open_Y0055
			// 2) 3sec Delay(??)
			// 3) VACUUM_SEPARATOR ON

			if (m_nManStep < 0) m_nManStep = 0;

            switch (m_nManStep)
            {
                case 10:
					fn_SetSeperator    (vvClose);
                    fn_SetSeperatorBlow(vvOpen );
                    fn_SetDrainValve   (vvOpen );

                    m_tDrainTime.Clear();
                    m_nManStep++;
                    return false;

                case 11:
                    if (!m_tDrainTime.OnDelay(true, 1000 * FM.m_stMasterOpt.nDrainTime)) return false; //10 sec

                    fn_SetDrainValve   (vvClose); // 
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

                    fn_UserMsg("Polishing Drain One Cycle OK.");

                    m_nManStep = 0;
                    return true;

            }


            return false;

        }

        //---------------------------------------------------------------------------
        public bool fn_UtilityOneCycle()
		{
			bool r1;
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

					fn_SetDrainValve(); //Drain Off
				

                    m_nManStep ++;
                    return false;

                case 11:
                    r1 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X , EN_COMD_ID.User9); //Level Check Position
					if (!r1) return false;
                    m_nManStep++;
                    return false;

                case 12: 
					
					//Utility Supply and Check Water Level Sensor status.
					if(IO.fn_IsUTLevelDone())
					{
						fn_SetDIWaterValve(vvClose);

						m_nManStep++;
                        return false;
                    }

					//Valve on
					fn_SetDIWaterValve(vvOpen);

					return false;

				case 13:
					r1 = fn_SetDIWaterValve(vvClose);
					if (!r1) return false;
                    m_nManStep++;
                    return false;

                case 14:
					
					fn_UserMsg("Utility One Cycle OK.");

                    m_nManStep=0;
                    return true;
			}

			return false; 
		}
		//---------------------------------------------------------------------------
		public bool fn_SetDrainValve(bool set = false)
		{
			//
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Drain] = set;
			
			return true; 
		}
        //---------------------------------------------------------------------------
        public bool fn_SetSlurryDIValve(EN_UTIL_KIND slury, bool set = false)
        {
			if (slury < EN_UTIL_KIND.Silica01 && slury > EN_UTIL_KIND.Silica03) return false;

			//int nadd = (int)EN_OUTPUT_ID.yPLS_Valve_SluryDI1 + (int)slury; 
			int nadd = (int)EN_OUTPUT_ID.yPLS_Valve_DIWater; //JUNG/200513/Slurry DI 삭제

			IO.YV[nadd] = set;

            return true;
        }
		//---------------------------------------------------------------------------
        public bool fn_SetSuckBackValve(EN_UTIL_KIND slury, bool set = false)
        {
			if (slury < EN_UTIL_KIND.Silica01 && slury > EN_UTIL_KIND.Silica03) return false;

			int nadd = (int)EN_OUTPUT_ID.yPLS_Slury_SuckBack1 + (int)slury;

			IO.YV[nadd] = set;

            return true;
        }
        //---------------------------------------------------------------------------
        public bool fn_SetDIWaterValve(bool set = false)
		{
            //
            if (m_bTESTMODE && SEQ._bRun) return false;

            bool bUsePump = FM.m_stSystemOpt.nUseAutoSlurry == 0 && !m_bTESTMODE;

			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater ] = set;
			IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater      ] = bUsePump? set : false;
			
			return true; 
		}
		//---------------------------------------------------------------------------
		public bool fn_SetLeakDrain(bool set = false)
		{
			IO.YV[(int)EN_OUTPUT_ID.yPLS_LeakDrain       ] = set;
			IO.YV[(int)EN_OUTPUT_ID.yValue_SperatorBlower] = set;

			return true;
		}

		//---------------------------------------------------------------------------
		public bool fn_SupplyUtil(EN_UTIL_KIND util)
        {
			//
			if (m_bTESTMODE && SEQ._bRun) return false;

			bool bUseAutoSupply =  FM.m_stSystemOpt.nUseAutoSlurry == 1;
			bool bUsePump = !m_bTESTMODE; //&& !bUseAutoSupply; 
            
			fn_SetDrainValve(vvClose);

			m_enLastUtil = util;

			//Check EMS
			if (SEQ._bEMSOn)
            {
                fn_StopAllUtil();
                return false;
            }
			
			//Use DI instead of slurry
			bool bUseDI = FM.m_stMasterOpt.nUseDI == 1;
			if (bUseDI) // && SEQ._bRun
			{
                IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = vvOpen;
                IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = vvOpen;

				if (bUseAutoSupply)
				{
					SUPPLY[SPLY_SLURRY]._bSlurryReq  = false;
					SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
					SUPPLY[SPLY_SLURRY]._bCleanReq   = false;

					//
					SUPPLY[SPLY_SOAP]._bSlurryReq    = false;
					SUPPLY[SPLY_SOAP]._nSlurryType   = 0;
					SUPPLY[SPLY_SOAP]._bCleanReq     = false;
				}

				return true;
            }
			
			//
			switch (util)
			{
				case EN_UTIL_KIND.Silica01:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] = vvOpen;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI1] = bUsePump && vvOpen; 
					if(bUseAutoSupply)
					{
						SUPPLY[SPLY_SLURRY]._bSlurryReq  = true;
						//SUPPLY[SPLY_SLURRY]._nSlurryType = 1   ; 
					}
					break;

				case EN_UTIL_KIND.Silica02:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ] = vvOpen;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI2] = bUsePump && vvOpen;
					if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SLURRY]._bSlurryReq = true;
                        //SUPPLY[SPLY_SLURRY]._nSlurryType = 2;
                    }

                    break;
				
				case EN_UTIL_KIND.Silica03:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ] = vvOpen;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI3] = bUsePump && vvOpen;
					if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SLURRY]._bSlurryReq = true;
                        //SUPPLY[SPLY_SLURRY]._nSlurryType = 3;
                    }

                    break;
				case EN_UTIL_KIND.Soap:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap   ] = vvOpen;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap    ] = bUsePump && vvOpen;
					if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SOAP]._bSlurryReq = true;
                        //SUPPLY[SPLY_SOAP]._nSlurryType = 4;
                    }

                    break;
				case EN_UTIL_KIND.DIWater: //JUNG/200615/Add Recipe List
                    IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = vvOpen;
                    IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = bUsePump && vvOpen;

                    break;

				case EN_UTIL_KIND.SilicaDI:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] = vvOpen;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI1] = bUsePump && vvOpen; 
					if(bUseAutoSupply)
					{
						SUPPLY[SPLY_SLURRY]._bCleanReq  = true;
						//SUPPLY[SPLY_SLURRY]._nSlurryType = 1   ; 
					}
					
					break;

				case EN_UTIL_KIND.SoapDI:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap   ] = vvOpen;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap    ] = bUsePump && vvOpen;
					if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SOAP]._bCleanReq = true;
                        //SUPPLY[SPLY_SOAP]._nSlurryType = 4;
                    }

					break;
				default:
					fn_StopAllUtil();
					break;
			}
			
            return true;
        }
		//---------------------------------------------------------------------------
        public bool fn_StopUtil(EN_UTIL_KIND util = EN_UTIL_KIND.ALL)
        {
            bool bUseAutoSupply = FM.m_stSystemOpt.nUseAutoSlurry == 1;
			bool bUseDI         = FM.m_stMasterOpt.nUseDI         == 1; 

			//Use DI
			if(bUseDI) // && SEQ._bRun
			{
                IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = vvClose;
                IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = vvClose;
                
				if (bUseAutoSupply)
                {
                    SUPPLY[SPLY_SLURRY]._bSlurryReq = false;
                    SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
                    SUPPLY[SPLY_SLURRY]._bCleanReq = false;

                    //
                    SUPPLY[SPLY_SOAP]._bSlurryReq  = false;
                    SUPPLY[SPLY_SOAP]._nSlurryType = 0;
                    SUPPLY[SPLY_SOAP]._bCleanReq   = false;
                }

                return true; 
			}

			//
			switch (util)
            {
				case EN_UTIL_KIND.ALL:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap   ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = vvClose;

					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI1] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI2] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI3] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = vvClose;

                    if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SLURRY]._bSlurryReq  = false;
                        //SUPPLY[SPLY_SLURRY]._nSlurryType = 0;

                        SUPPLY[SPLY_SLURRY]._bCleanReq   = false;
                    }
                    break;

				case EN_UTIL_KIND.Silica01:
                    IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI1] = vvClose;
                    
					if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SLURRY]._bSlurryReq = false;
                        //SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
                    }

                    break;
                
				case EN_UTIL_KIND.Silica02:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI2] = vvClose;
                    if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SLURRY]._bSlurryReq = false;
                        //SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
                    }

                    break;
				case EN_UTIL_KIND.Silica03:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI3] = vvClose;
                    if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SLURRY]._bSlurryReq = false;
                        //SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
                    }

                    break;
				case EN_UTIL_KIND.Soap:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap   ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap    ] = vvClose;
                    if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SOAP]._bSlurryReq = false;
                        //SUPPLY[SPLY_SOAP]._nSlurryType = 0;
                    }

                    break;
				
				case EN_UTIL_KIND.DIWater:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = vvClose;

                    break;

				default:
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap   ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = vvClose;

					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI1] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI2] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI3] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap    ] = vvClose;
					IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = vvClose;

                    if (bUseAutoSupply)
                    {
                        SUPPLY[SPLY_SLURRY]._bSlurryReq  = false;
                        SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
                        SUPPLY[SPLY_SLURRY]._bCleanReq   = false;

						//
                        SUPPLY[SPLY_SOAP  ]._bSlurryReq = false;
                        SUPPLY[SPLY_SOAP  ]._nSlurryType = 0;
                        SUPPLY[SPLY_SOAP  ]._bCleanReq = false;
                    }

                    break;
            }

			//SuckBack Cycle On
			//fn_SetSuckBackOn(util); //차후 동작 상태 확인 필요...


			return true;
        }
		//---------------------------------------------------------------------------
        public bool fn_SetUtilSignal(EN_UTIL_KIND util, bool set = false)
        {
            bool bUseAutoSupply = FM.m_stSystemOpt.nUseAutoSlurry == 1;

			if (!bUseAutoSupply) return true; 

			//
			switch (util)
            {
				case EN_UTIL_KIND.ALL:

					SUPPLY[SPLY_SLURRY]._bSlurryReq  = set;
					//SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
					
					SUPPLY[SPLY_SLURRY]._bCleanReq   = set;
                    break;

				case EN_UTIL_KIND.Silica01:
                    
                    SUPPLY[SPLY_SLURRY]._bSlurryReq = set;
                    //SUPPLY[SPLY_SLURRY]._nSlurryType = 0;


                    break;
                
				case EN_UTIL_KIND.Silica02:
                    
                    SUPPLY[SPLY_SLURRY]._bSlurryReq = set;
                    //SUPPLY[SPLY_SLURRY]._nSlurryType = 0;

                    break;

				case EN_UTIL_KIND.Silica03:
                    
                    SUPPLY[SPLY_SLURRY]._bSlurryReq = set;
                    //SUPPLY[SPLY_SLURRY]._nSlurryType = 0;

                    break;
				
				case EN_UTIL_KIND.Soap:
                    
                    SUPPLY[SPLY_SOAP]._bSlurryReq = set;
                    //SUPPLY[SPLY_SOAP]._nSlurryType = 0;
                    break;
				
				case EN_UTIL_KIND.DIWater:

                    break;

				
				default:

					SUPPLY[SPLY_SLURRY]._bSlurryReq  = set;
					//SUPPLY[SPLY_SLURRY]._nSlurryType = 0;
					SUPPLY[SPLY_SLURRY]._bCleanReq   = set;
					
					//
					SUPPLY[SPLY_SOAP  ]._bSlurryReq = set;
					//SUPPLY[SPLY_SOAP  ]._nSlurryType = 0;
					SUPPLY[SPLY_SOAP  ]._bCleanReq = set;

                    break;
            }

			return true;
        }
		//---------------------------------------------------------------------------
        public bool fn_GetUtilSignal(EN_UTIL_KIND util)
        {
            bool bUseAutoSupply = FM.m_stSystemOpt.nUseAutoSlurry == 1;

			if (!bUseAutoSupply) return false; 

			//
			switch (util)
            {
				case EN_UTIL_KIND.Silica01:
                    
                    return SUPPLY[SPLY_SLURRY]._bSlurryReq;
                
				case EN_UTIL_KIND.Silica02:

					return SUPPLY[SPLY_SLURRY]._bSlurryReq;

				case EN_UTIL_KIND.Silica03:

					return SUPPLY[SPLY_SLURRY]._bSlurryReq; 
		
				case EN_UTIL_KIND.Soap:

					return SUPPLY[SPLY_SOAP]._bSlurryReq ;
                    //SUPPLY[SPLY_SOAP]._nSlurryType = 0;
            }

			return false;
        }

		//---------------------------------------------------------------------------
		public bool fn_StopAllUtil()
		{
            IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap   ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] = vvClose;

			IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI1] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI2] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI3] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap    ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater     ] = vvClose;

			//JUNG/200810/Add Auto Slurry
			bool bUseAutoSupply = FM.m_stSystemOpt.nUseAutoSlurry == 1;
			if (bUseAutoSupply)
            {
                SUPPLY[SPLY_SLURRY]._bSlurryReq  = false;
                SUPPLY[SPLY_SLURRY]._nSlurryType = 0;

				SUPPLY[SPLY_SLURRY]._bCleanReq   = false;

				//
                SUPPLY[SPLY_SOAP]._bSlurryReq    = false;
                SUPPLY[SPLY_SOAP]._nSlurryType   = 0;

                SUPPLY[SPLY_SOAP]._bCleanReq     = false;
            }

            return true; 
        }
        //---------------------------------------------------------------------------
        public bool fn_SeqSuckBack()
		{
			//1) separator blow on  Y0058
			//2) suckback v/v On - Y0056 --> 3sec

			//Sequence Suck Back
			if (m_nSuckBackStep < 0) m_nSuckBackStep = 0;
			switch (m_nSuckBackStep)
			{
				case 10:
					m_bDrngSeqSuckBack = true; 

     				m_tSuckBackDelay.Clear();
					m_nSuckBackStep++;
					return false;

				case 11:
					//fn_SetSlurryDIValve(m_enUtilKind, vvOpen); //

					m_tSuckBackDelay.Clear();
                    m_nSuckBackStep++;
					return false;

				case 12:
					if (!m_tSuckBackDelay.OnDelay(true, 1000 * 3)) return false; //3 sec

					//fn_SetSlurryDIValve(m_enUtilKind, vvClose); //

                    m_nSuckBackStep++;
					return false;

				case 13:
					//fn_SetSeperator    (vvOpen);
					fn_SetSeperatorBlow(vvOpen);
					fn_SetSuckBackValve(m_enUtilKind, vvOpen); //

					m_tSuckBackDelay.Clear();
                    m_nSuckBackStep++;
					return false;

				case 14:
                    if (!m_tSuckBackDelay.OnDelay(true, 1000 * 3)) return false; //5 sec

					fn_SetSuckBackValve(m_enUtilKind, vvClose); //
					fn_SetSeperatorBlow(vvClose);
					//fn_SetSeperator    (vvClose); //

					fn_WriteSeqLog("[Polishing] Suck Back End - " + m_enUtilKind.ToString());

					Console.WriteLine("[Polishing] Suck Back End - " + m_enUtilKind.ToString());

					m_tSuckBackDelay.Clear();

					m_bDrngSeqSuckBack = false;
					m_nSuckBackStep = 0 ;
					return true;

				default:
                    m_tSuckBackDelay.Clear();
					m_bDrngSeqSuckBack = false;
					m_nSuckBackStep = 0;
                    return true;
			}
		}
        //---------------------------------------------------------------------------
        public bool fn_SetSuckBackOn()
        {
			if (m_bDrngSeqSuckBack) return true; 
			
			m_enUtilKind = m_enLastUtil;

            m_nSuckBackStep    = 10;
			m_bDrngSeqSuckBack = true; 

			fn_WriteSeqLog("Set Suck Back : " + m_enUtilKind.ToString());

            return true;

        }
        //---------------------------------------------------------------------------
        public bool fn_SetSuckBackOn(EN_UTIL_KIND util)
		{
			
			if (m_bTESTMODE       ) return true;
			if (m_bDrngSeqSuckBack) return true;

			m_enUtilKind       = util; 

			m_nSuckBackStep    = 10  ;
			m_bDrngSeqSuckBack = true;

			fn_WriteSeqLog("Set Suck Back : " + m_enUtilKind.ToString());

			return true; 
		
		}
        //---------------------------------------------------------------------------
        public bool fn_SetSuckBackOff()
        {
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1 ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2 ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3 ] = vvClose;
			IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap] = vvClose;
			
			fn_SetSeperator(); //IO.YV[(int)EN_OUTPUT_ID.yVacSperator         ] = vvClose; 

			return true;
        }
		//---------------------------------------------------------------------------
		public bool fn_SetSeperator(bool set = false)
		{
			IO.YV[(int)EN_OUTPUT_ID.yValue_SperatorDrain] = false; // = set; //미사용//pump drain과 둘중 하나만....
			IO.YV[(int)EN_OUTPUT_ID.yPump_Drain         ] = m_bTESTMODE ? false : set; //

			IO.YV[(int)EN_OUTPUT_ID.yValue_backflow     ] = IO.YV[(int)EN_OUTPUT_ID.yPump_Drain]; //JUNG/200523/역류방지...

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
        public bool fn_ReqBathWaitPos()
		{
			bool r1 = fn_MoveMotr(m_iMotrYId , EN_COMD_ID.Wait1);
			bool r2 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.Wait1);
			bool r3 = fn_MoveMotr(m_iMotrTIId, EN_COMD_ID.Wait1);

			return (r1 && r2 && r3);
		}
		//---------------------------------------------------------------------------
		public bool fn_ReqBathWaitPosTHTI()
		{
			bool r1 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.Wait1);
			bool r2 = fn_MoveMotr(m_iMotrTIId, EN_COMD_ID.Wait1);

			return (r1 && r2);
		}
		//---------------------------------------------------------------------------
		public bool fn_ReqBathImagePos()
		{
			bool r1 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.Wait1);
			bool r2 = fn_MoveMotr(m_iMotrTIId, EN_COMD_ID.Wait1);
			bool r3 = fn_MoveMotr(m_iMotrYId , EN_COMD_ID.User2);

			return (r1 && r2 && r3);
		}

		//---------------------------------------------------------------------------
		public bool fn_ReqBathCupInOutPos()
		{//Polishing Bath In/Out Position
			
			bool r1 = fn_MoveMotr(m_iMotrYId , EN_COMD_ID.User3);
			bool r2 = fn_MoveMotr(m_iMotrTHId, EN_COMD_ID.User1);
			bool r3 = fn_MoveMotr(m_iMotrTIId, EN_COMD_ID.Wait1);

			return (r1 && r2 && r3);
		}
		//---------------------------------------------------------------------------
		public bool fn_ReqCupStorInOutPos()
		{//Cup Storage In/Out position
			
			bool r1 = fn_MoveMotr(m_iMotrYId , EN_COMD_ID.User4);
			return (r1);
		}

		//---------------------------------------------------------------------------
		public string fn_GetUtilString()
		{
			switch(m_enUtilSate)
			{
				case EN_UTIL_STATE.Exist  : return "EXT";
                case EN_UTIL_STATE.Empty  : return "Emt";
                case EN_UTIL_STATE.Unknown: return "Unk";
				default:                    return "???";
			}
        }
		//---------------------------------------------------------------------------
        public void fn_SetUtilState(EN_UTIL_STATE state)
        {
			m_enUtilSate = state; 
        }
        //---------------------------------------------------------------------------
        private void fn_WriteSeqLog(string log)
        {
			string stemp = string.Format($"[{EN_SEQ_ID.POLISH.ToString()}] ");
            fn_WriteLog(stemp + log, EN_LOG_TYPE.ltEvent, EN_SEQ_ID.POLISH);
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
		public void fn_SaveLog(ref string Msg)
        {
			string sTemp = string.Empty;

			Msg = string.Empty;

			//
			Msg += "[SeqPolishing]\r\n"; 
			Msg += sTemp = string.Format($"m_bToStart         = {m_bToStart        }\r\n"); 
			Msg += sTemp = string.Format($"m_bToStop          = {m_bToStop         }\r\n"); 
			Msg += sTemp = string.Format($"m_bDrngInspect     = {m_bDrngInspect    }\r\n"); 
			Msg += sTemp = string.Format($"m_bDrngPolishing   = {m_bDrngPolishing  }\r\n"); 
			Msg += sTemp = string.Format($"m_bDrngUtility     = {m_bDrngUtility    }\r\n"); 
			Msg += sTemp = string.Format($"m_bDrngDrain       = {m_bDrngDrain      }\r\n"); 
			Msg += sTemp = string.Format($"m_bDrngWait        = {m_bDrngWait       }\r\n"); 
			Msg += sTemp = string.Format($"m_bDrngSeqDrain    = {m_bDrngSeqDrain   }\r\n"); 
			Msg += sTemp = string.Format($"m_bDrngSeqSuckBack = {m_bDrngSeqSuckBack}\r\n"); 
			Msg += sTemp = string.Format($"m_nSeqStep         = {m_nSeqStep        }\r\n"); 
			Msg += sTemp = string.Format($"m_nManStep         = {m_nManStep        }\r\n"); 
			Msg += sTemp = string.Format($"m_nHomeStep        = {m_nHomeStep       }\r\n"); 
			Msg += sTemp = string.Format($"m_nInspectStep     = {m_nInspectStep    }\r\n"); 
			Msg += sTemp = string.Format($"m_nPolishingStep   = {m_nPolishingStep  }\r\n"); 
			Msg += sTemp = string.Format($"m_nUtilityStep     = {m_nUtilityStep    }\r\n"); 
			Msg += sTemp = string.Format($"m_nDrainStep       = {m_nDrainStep      }\r\n"); 


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

            Items[nRow, 0].Content = "bToStart       " ; Items[nRow++, 1].Content = string.Format($"{m_bToStart        }");
			Items[nRow, 0].Content = "bToStop        " ; Items[nRow++, 1].Content = string.Format($"{m_bToStop         }");
			Items[nRow, 0].Content = "_";				 Items[nRow++, 1].Content = string.Format($"");			    
			Items[nRow, 0].Content = "bDrngInspect   " ; Items[nRow++, 1].Content = string.Format($"{m_bDrngInspect    }");
            Items[nRow, 0].Content = "bDrngPolishing " ; Items[nRow++, 1].Content = string.Format($"{m_bDrngPolishing  }");
            Items[nRow, 0].Content = "bDrngUtility   " ; Items[nRow++, 1].Content = string.Format($"{m_bDrngUtility    }");
			Items[nRow, 0].Content = "bDrngDrain"      ; Items[nRow++, 1].Content = string.Format($"{m_bDrngDrain      }");
			Items[nRow, 0].Content = "bDrngWait      " ; Items[nRow++, 1].Content = string.Format($"{m_bDrngWait       }");
			Items[nRow, 0].Content = "bDrngSeqDrain"   ; Items[nRow++, 1].Content = string.Format($"{m_bDrngSeqDrain   }");
			Items[nRow, 0].Content = "bDrngSeqSuckBack"; Items[nRow++, 1].Content = string.Format($"{m_bDrngSeqSuckBack}");
            Items[nRow, 0].Content = "";				 Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "nSeqStep"        ; Items[nRow++, 1].Content = string.Format($"{m_nSeqStep        }");
			Items[nRow, 0].Content = "nManStep"        ; Items[nRow++, 1].Content = string.Format($"{m_nManStep        }");
			Items[nRow, 0].Content = "nHomeStep"       ; Items[nRow++, 1].Content = string.Format($"{m_nHomeStep       }");
			Items[nRow, 0].Content = "_";				 Items[nRow++, 1].Content = string.Format($"");			    
			Items[nRow, 0].Content = "nInspectStep"    ; Items[nRow++, 1].Content = string.Format($"{m_nInspectStep    }");
			Items[nRow, 0].Content = "nPolishingStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nPolishingStep  }");
			Items[nRow, 0].Content = "nUtilityStep"	   ; Items[nRow++, 1].Content = string.Format($"{m_nUtilityStep    }");
			Items[nRow, 0].Content = "nDrainStep"      ; Items[nRow++, 1].Content = string.Format($"{m_nDrainStep      }");
			Items[nRow, 0].Content = "nReqDrainStep"   ; Items[nRow++, 1].Content = string.Format($"{m_nReqDrainStep   }");
			Items[nRow, 0].Content = "";				 Items[nRow++, 1].Content = string.Format($"");
			Items[nRow, 0].Content = "Seq Msg"         ; Items[nRow++, 1].Content = string.Format($"{m_sSeqMsg         }");

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
			Items[nRow, 0].Content = "nPolishingStep"  ; Items[nRow++, 1].Content = string.Format($"{m_nPolishingStep }");
			Items[nRow, 0].Content = "nUtilityStep"    ; Items[nRow++, 1].Content = string.Format($"{m_nUtilityStep   }");
			Items[nRow, 0].Content = "nDrainStep"      ; Items[nRow++, 1].Content = string.Format($"{m_nDrainStep     }");
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

				m_enUtilSate  = (EN_UTIL_STATE)br.ReadInt32();

                m_nSpare1     = br.ReadInt32  ();
                m_nSpare2     = br.ReadInt32  ();
                m_nSpare3     = br.ReadInt32  ();
                m_nSpare4     = br.ReadInt32  ();
                m_nSpare5     = br.ReadInt32  ();


				//
				m_bReqDrain   = br.ReadBoolean();

				m_bSpare1     = br.ReadBoolean();
                m_bSpare2     = br.ReadBoolean();
                m_bSpare3     = br.ReadBoolean();
                m_bSpare4     = br.ReadBoolean();
                m_bSpare5     = br.ReadBoolean();

                //
                m_dSpare1     = br.ReadDouble ();
                m_dSpare2     = br.ReadDouble ();
                m_dSpare3     = br.ReadDouble ();
                m_dSpare4     = br.ReadDouble ();
                m_dSpare5     = br.ReadDouble ();

			}
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);

				bw.Write((int)m_enUtilSate);

				bw.Write(m_nSpare1    ); //Spare
                bw.Write(m_nSpare2    );
                bw.Write(m_nSpare3    );
                bw.Write(m_nSpare4    );
                bw.Write(m_nSpare5    );

				//
				bw.Write(m_bReqDrain  ); //

				bw.Write(m_bSpare1    ); //Spare
                bw.Write(m_bSpare2    );
                bw.Write(m_bSpare3    );
                bw.Write(m_bSpare4    );
                bw.Write(m_bSpare5    );

                //
                bw.Write(m_dSpare1    ); //Spare
                bw.Write(m_dSpare2    );
                bw.Write(m_dSpare3    );
                bw.Write(m_dSpare4    );
                bw.Write(m_dSpare5    );

                bw.Flush();

            }
        }



    }
}

