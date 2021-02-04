using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaferPolishingSystem.Define;
using WaferPolishingSystem.BaseUnit;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFile;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;
using System.Windows;
using System.Windows.Media;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.BaseUnit.ActuatorId;

namespace WaferPolishingSystem.BaseUnit
{
	/**
    @class     Main Sequence Class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/02/08 10:16
    */
	public class SequenceUnit
	{
		//Var.
		bool m_bRun         ;
		bool m_bStop        ;
		bool m_bBtnStart    ;
		bool m_bBtnStop     ;
		bool m_bBtnReset    ;
		bool m_bBtnWinStart ;
		bool m_bBtnWinStop  ;
		bool m_bBtnWinReset ;
		bool m_bBtnManReset ;
		bool m_bBtnManStart ;
		bool m_bBtnManStop  ;
		bool m_bResetCon    ;
		bool m_bRunCon      ; 
		bool m_bStopCon     ;
		bool m_bLtStop      ;
		bool m_bLtRun       ;
		bool m_bScreenLock  ;
		bool m_bAuto        ;
		bool m_bNoSafety    ;

		bool m_bFlick1      ;
		bool m_bFlick2      ;
		bool m_bFlick3      ;
		bool m_bStopErrAtRun;
		bool m_bLightOn     ;
		bool m_bEMSOn       ;

		bool m_bRecipeOpen  ; //for only one plate processing
		

		bool m_bOneShotFlick1;
		//bool    m_bOneShotFlick2;
		//bool    m_bOneShotFlick3;
		bool m_bEdgSTRLock      ;
		bool m_bEdgSTRUnlock    ;

		bool m_bEdgDoorOpen     ;
		bool m_bEdgDoorClose    ;

	    public bool FLG_RUN_WAIT  ;

		bool[] TS_RSLT = new bool[20];

		public int _nMCTYPE { get; }

        //
        private int m_iSeqStat; //Sequence Status
		private int m_iStep;
		//private int m_iIniStep; //Init Step

        //
        Label[,] Items = new Label[50, 2];


        //Timer
        private TOnDelayTimer m_tDelayTimer     = new TOnDelayTimer();
		private TOnDelayTimer m_tToStopTimer    = new TOnDelayTimer();
		private TOnDelayTimer m_tToStartTimer   = new TOnDelayTimer();
		private TOnDelayTimer m_tOnFlick1Timer  = new TOnDelayTimer();
		private TOnDelayTimer m_tOffFlick1Timer = new TOnDelayTimer();

		private TOnDelayTimer m_tOnFlick2Timer  = new TOnDelayTimer();
		private TOnDelayTimer m_tOffFlick2Timer = new TOnDelayTimer();
		private TOnDelayTimer m_tOnFlick3Timer  = new TOnDelayTimer();
		private TOnDelayTimer m_tOffFlick3Timer = new TOnDelayTimer();

		private TOnDelayTimer m_tStartDelay     = new TOnDelayTimer();
		private TOnDelayTimer m_tStopDelay      = new TOnDelayTimer();

		private TOnDelayTimer m_tAutoDelay      = new TOnDelayTimer();

		private TOnDelayTimer m_tHoldTimer      = new TOnDelayTimer();
		private TOnDelayTimer m_tRunWaitTimer   = new TOnDelayTimer();

		private TOnDelayTimer m_tEMOTimer1      = new TOnDelayTimer();
		private TOnDelayTimer m_tEMOTimer2      = new TOnDelayTimer();
		private TOnDelayTimer m_tEMOTimer3      = new TOnDelayTimer();
		private TOnDelayTimer m_tSpdlTimer      = new TOnDelayTimer();

		private TOnDelayTimer[] m_tPowerTimer   = new TOnDelayTimer[40];
		private TOnDelayTimer[] m_tFanChkTimer  = new TOnDelayTimer[10];
		private TOnDelayTimer[] m_tLeakChkTimer = new TOnDelayTimer[10];
		private TOnDelayTimer[] m_tAccuChkTimer = new TOnDelayTimer[10];
		private TOnDelayTimer[] m_tDPTimer      = new TOnDelayTimer[10];


		//OneShot
		private TOneShotDetect m_OneShotFlick1  = new TOneShotDetect();
		private TOneShotDetect m_OnshotReset    = new TOneShotDetect();
		private TOneShotDetect m_OneShotSTR     = new TOneShotDetect();

		private TOneShotDetect m_EdgDoorOpen     = new TOneShotDetect();
		private TOneShotDetect m_EdgDoorClose    = new TOneShotDetect();

		//Time
		private int[] nUD_Start = new int[20]; //Update Scan Time of Sequence
		private int[] nUD_Scan  = new int[20];
		private int[] nAR_Start = new int[20]; //Update Scan Time of Auto Run
		private int[] nAR_Scan  = new int[20];

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Property
		public bool _bRun           { get { return m_bRun     ; } }
		public bool _bAuto          { get { return m_bAuto    ; } }
		public bool _bNoSafety      { get { return m_bNoSafety; } }
		
		public bool _bStop          { get { return m_bStop; } }
		public bool _bBtnStart      { get { return m_bBtnStart; } set { m_bBtnStart = value; } }
		public bool _bBtnStop       { get { return m_bBtnStop; } set { m_bBtnStop = value; } }
		public bool _bBtnReset      { get { return m_bBtnReset; } set { m_bBtnReset = value; } }
		public bool _bBtnWinStart   { get { return m_bBtnWinStart; } set { m_bBtnWinStart = value; } }
		public bool _bBtnWinStop    { get { return m_bBtnWinStop; } set { m_bBtnWinStop = value; } }
		public bool _bBtnWinReset   { get { return m_bBtnWinReset; } set { m_bBtnWinReset = value; } }
		public bool _bBtnManReset   { get { return m_bBtnManReset; } set { m_bBtnManReset = value; } }
		public bool _bBtnManStart   { get { return m_bBtnManStart; } set { m_bBtnManStart = value; } }
		public bool _bBtnManStop    { get { return m_bBtnManStop; } set { m_bBtnManStop = value; } }
		public bool _bResetCon      { get { return m_bResetCon; } set { m_bResetCon = value; } }
		public bool _bRunCon        { get { return m_bRunCon; } set { m_bRunCon = value; } }
		public bool _bStopCon       { get { return m_bStopCon; } set { m_bStopCon = value; } }
		public bool _bLtStop        { get { return m_bLtStop; } set { m_bLtStop = value; } }
		public bool _bLtRun         { get { return m_bLtRun; } set { m_bLtRun = value; } }
		public bool _bScreenLock    { get { return m_bScreenLock; } set { m_bScreenLock = value; } }
								    
		public bool _bFlick1        { get { return m_bFlick1; } set { m_bFlick1 = value; } }
		public bool _bFlick2        { get { return m_bFlick2; } set { m_bFlick2 = value; } }
		public bool _bFlick3        { get { return m_bFlick3; } set { m_bFlick3 = value; } }
		public bool _bStopErrAtRun  { get { return m_bStopErrAtRun; } set { m_bStopErrAtRun = value; } }


		public int _iSeqStat        { get { return m_iSeqStat   ; } set { m_iSeqStat = value; } }     //Sequence Status
		public int _iStep           { get { return m_iStep      ; } set { m_iStep = value; } }

		public bool _bRecipeOpen    { get { return m_bRecipeOpen; } set { m_bRecipeOpen = value; } }

		public bool _bLightOn       { get { return m_bLightOn; } set { m_bLightOn = value; } }
		
		public bool _bEMSOn         { get { return m_bEMSOn; } }


		/************************************************************************/
		/* 생성자                                                                */
		/************************************************************************/
		public SequenceUnit()
		{
			//			    
			_nMCTYPE        = _MCTYPE_18; //VER 1.8
						    
			//			    
			m_bRun          = false;
			m_bAuto         = false; 
						    
			m_bBtnReset     = false; //Button Input.
			m_bBtnStart     = false;
			m_bBtnStop      = false;
			m_bBtnWinReset  = false;
			m_bBtnWinStart  = false;
			m_bBtnWinStop   = false;
						    
			m_bBtnWinStop   = false;
			m_bBtnManReset  = false;
			m_bBtnManStart  = false;
			m_bBtnManStop   = false;
			m_bResetCon     = false;
			m_bRunCon       = false;
			m_bStopCon      = false;
			m_bLtStop       = false;
			m_bLtRun        = false;
			m_bNoSafety     = false;
						    
			m_bFlick1       = false; //Flicking Flag
			m_bFlick2       = false;
			m_bFlick3       = false;

			m_bStopErrAtRun = false;

			m_bRecipeOpen   = false;

			FLG_RUN_WAIT    = false;

			m_bLightOn      = false;
			m_bEMSOn        = false;

            m_bEdgSTRLock   = false; 
			m_bEdgSTRUnlock = false;
            m_bEdgDoorOpen  = false;
			m_bEdgDoorClose	= false;


            for (int i = 0; i < 20; i++)
			{
				TS_RSLT  [i] = false;
				nUD_Start[i] = 0;
				nUD_Scan [i] = 0;
				nAR_Start[i] = 0;
				nAR_Scan [i] = 0;
			}

			m_iSeqStat = (int)EN_SEQ_STATE.STOP; //Current Sequence Status.
			m_iStep = 0; //Sequence Step.

			//
			for (int r = 0; r < 50; r++)
			{
				for (int c = 0; c < 2; c++)
				{
					Items[r,c] = new Label();

					Items[r,c].BorderThickness = new Thickness(1);
					Items[r,c].BorderBrush = System.Windows.Media.Brushes.LightGray;
					Items[r,c].FontSize = 11;
					Items[r,c].HorizontalContentAlignment = HorizontalAlignment.Left;
					Items[r,c].VerticalContentAlignment = VerticalAlignment.Center;
					//Items[r,c].Margin = new Thickness(1);
				}
			}

			//Timer Init
			m_tEMOTimer1.Clear();
            m_tEMOTimer2.Clear();
			m_tEMOTimer3.Clear();
			m_tSpdlTimer.Clear();

			for (int n = 0; n < 40; n++)
			{
				//
				if (n < 10)
				{
					m_tFanChkTimer [n] = new TOnDelayTimer();
					m_tLeakChkTimer[n] = new TOnDelayTimer();
					m_tAccuChkTimer[n] = new TOnDelayTimer();
					m_tDPTimer     [n] = new TOnDelayTimer();
				}


				//
				m_tPowerTimer[n] = new TOnDelayTimer();
			}

        }	
		
		//-------------------------------------------------------------------------------------------------

		//
		public int  fn_GetSeqStatus() { return m_iSeqStat; }
		public bool fn_IsSeqStatus(int status) { return m_iSeqStat == status; }
		public bool fn_IsSeqStatus(EN_SEQ_STATE status) { return m_iSeqStat == (int)status; }
		public int  fn_GetSeqStep() { return m_iStep; }

		public void fn_SetBtnStart() { m_bBtnWinStart = true; }
		public void fn_SetBtnStop () { m_bBtnWinStop  = true; }
		public void fn_SetBtnReset() { m_bBtnWinReset = true; }


		public bool fn_IsFlickOn1() { return m_bFlick1; }
		public bool fn_IsFlickOn2() { return m_bFlick2; }
		public bool fn_IsFlickOn3() { return m_bFlick3; }

		public bool fn_IsNoRun()
		{
			if (m_bRun                              ) return false;
			if (m_iSeqStat == (int)EN_SEQ_STATE.INIT) return false;
			if (m_iStep  > 10                       )  return false;
			
			return true; 
		}


		//---------------------------------------------------------------------------
		/**    
		@brief     Main Update Function
		@return    
		@param     
		@remark    장비 Main Sequence 동작 호출
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/9/3  11:45
		*/
		public void fn_Update()
		{
			if (IO._bDrngReboot) return;

			nUD_Start[0] = Environment.TickCount; // & Int32.MaxValue; //Update Time Check

			//Interlock Check
			fn_InspectEmergency    ();
			fn_InspectMainAir      ();
			fn_InspectSafety       ();
			fn_InspectMotor        ();
			fn_InspectSysFan       ();
			fn_InspectLeak         ();
			fn_InspectWaterLvl     ();
			fn_InspectCBoxEmergency();
			fn_InspectConnection   ();
			fn_InspectE3000        ();
			fn_InspectHomeEnd      ();
            fn_InspectActuator     ();
            fn_InspectHold         ();
			fn_InspectPower        ();
			fn_InspectAccura       ();
			fn_InspectDP           ();

			//Error Check
			EPU.fn_IsErrState      ();
			

			//Manual Run 
			MAN.fn_ManRunCycle();

			//Actuator
			ACTR.fn_Update();

            //Sequence Status 
            SEQ.fn_UpdateSeqState();

            //Auto Run Check - start Check // Check Button Push
            fn_CheckRunCon();
			fn_CheckMaualButton(); //Panel Button Push Check


			//Total Scan Time
			nUD_Scan[0] = Environment.TickCount - nUD_Start[0];

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Main Update Function
		@return    
		@param     
		@remark    장비 Main Sequence 동작 호출 - 01
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/9/3  11:45
		*/
		public void fn_Update1()
		{

			//

		}

		//---------------------------------------------------------------------------
		/**    
		@brief     Check Emergency Status
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  9:19
		*/
		public bool fn_InspectEmergency()
		{
			bool bErr = false;
			m_tEMOTimer1.OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_EMO_Front], 300);
            m_tEMOTimer2.OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_EMO_Rear ], 300);
            m_tEMOTimer3.OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_EMO_CBox ], 300);

			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0101, m_tEMOTimer1.Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0102, m_tEMOTimer2.Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0103, m_tEMOTimer3.Out)) bErr = true;

            if (bErr)
            {
				IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI1 ] = false;
				IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI2 ] = false;
				IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_SluryDI3 ] = false;
				IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap     ] = false;
				IO.YV[(int)EN_OUTPUT_ID.yPump_DIWater      ] = false;

				IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1     ] = false;
				IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2	   ] = false;
                IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3	   ] = false;
                IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater ] = false;
                IO.YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater ] = false;
				IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap    ] = false;

			}

			if (bErr) m_bEMSOn = true; 
										

            return bErr;
		}
		//---------------------------------------------------------------------------
		public bool fn_InspectCBoxEmergency()
		{
            bool bErr = false;

			if ( FM.m_stMasterOpt.nUseSkipAccura == 1) return bErr;
			if (!IO._bConnect                        ) return bErr;

            m_tAccuChkTimer[0].OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_CBox_Temp_Alarm], 500);
            m_tAccuChkTimer[1].OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_CBox_Gas_Alarm ], 500);

            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0107, m_tAccuChkTimer[0].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0108, m_tAccuChkTimer[1].Out)) bErr = true;

			//if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0107, !IO.XV[(int)EN_INPUT_ID.xSYS_CBox_Temp_Alarm])) bErr = true; //Temp Alarm
			//if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0108, !IO.XV[(int)EN_INPUT_ID.xSYS_CBox_Gas_Alarm ])) bErr = true; //Gas Alarm

			return bErr;
		}
		//---------------------------------------------------------------------------
		public bool fn_InspectConnection()
		{
			bool bErr = false;
			bool bUsePMC   = FM.m_stMasterOpt.nUsePMC        == 1;
			bool bUseSlruy = FM.m_stSystemOpt.nUseAutoSlurry == 1;

			//Check Connection
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0031, !IO._bConnect                          )) bErr = true; //
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0032, !MOTR._bConnect                        )) bErr = true; //
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0033, !PMC._bConnect       && bUsePMC        )) bErr = true; //
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0034, !LDCBTM._bConect                       )) bErr = true; //
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0035, !SUPPLY[0]._bConnect && bUseSlruy      )) bErr = true; //
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0036, !SUPPLY[1]._bConnect && bUseSlruy      )) bErr = true; //
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0037, !g_VisionManager._CamManager._bConnect )) bErr = true; //
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0038, !g_VisionManager._LightManger._bConnect)) bErr = true; //

			//
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0104, IO._bACSNetErr                         )) bErr = true; //
			


			return bErr;
		}
		//---------------------------------------------------------------------------
		public bool fn_InspectE3000()
		{
            bool bErr      = false;
			bool bTestMode = FM.fn_GetRunMode() == EN_RUN_MODE.TEST_MODE;
			bool xE3000_OK = IO.XV[(int)EN_INPUT_ID.xSPD_E3000_State];

			m_tSpdlTimer.OnDelay(!xE3000_OK, 2000);

			if (m_bRun)
			{
				if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0340, m_tSpdlTimer.Out && !bTestMode)) bErr = true; 
			}
			else
			{
				if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0050, !xE3000_OK)) bErr = true;
			}
			
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0051, IO.XV[(int)EN_INPUT_ID.xSPD_E3000_Warn])) bErr = true;

            return bErr;
        }
		//---------------------------------------------------------------------------
		public bool fn_InspectHomeEnd()
		{

			//EPU.fn_SetErr(EN_ERR_LIST.ERR_0015, !MOTR[(int)EN_MOTR_ID.miSPD_X].GetHomeEnd());
			for (int n = 0; n < MOTR._iNumOfMotr; n++)
			{
				EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0015 + n, !MOTR[n].GetHomeEnd());
			}
            
			return true; 
		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Main Air Condition
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  9:20
		*/
		public bool fn_InspectMainAir()
		{
			bool bErr = false;

			if (FM.m_stMasterOpt.nUseSkipAir == 1) return false;

			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0105, !IO.XV[(int)EN_INPUT_ID.xSYS_MainAir_TOP])) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0106, !IO.XV[(int)EN_INPUT_ID.xSYS_MainAir_BTM])) bErr = true;


            return bErr;

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Safety Status
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  9:17
		*/
		public bool fn_InspectSafety()
		{
			bool isOk = false;

			if (!IO._bConnect) return isOk; 

			isOk = !fn_IsAnySafetyErr();

			if (!isOk) m_bNoSafety = true; 

			if (isOk && m_bNoSafety && !fn_IsAnySafetyErr()) m_bNoSafety = false;

			if(!isOk)
			{
				MOTR.EmrgStop();
			}


			return isOk;
		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Any Safety Error Status
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  9:30
		*/
		public bool fn_IsAnySafetyErr()
		{
			if ( fn_IsAnyDoorOpen  ()) return true;
			if ( fn_InspectLeak    ()) return true;
			if ( fn_InspectSysFan  ()) return true;
			if ( fn_InspectWaterLvl()) return true;
			if ( fn_InspectAccura  ()) return true;

			return false;

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Door Open Condition
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  9:23
		*/
		public bool fn_IsAnyDoorOpen(bool check = false)
		{
			bool bOpen = false;

			if (fn_IsMainDoorOpen(check)) bOpen = true;
			if (fn_IsSideDoorOpen(check)) bOpen = true;

			if (bOpen) //&& !check
			{
				fn_SetLight(bOpen); //JUNG/200414
			}

			//Safe PLC Door Signal
			EPU.fn_SetErr(EN_ERR_LIST.ERR_0070, IO.fn_GetDoorSignalErr());

			return bOpen;
		}

		//---------------------------------------------------------------------------
		public bool fn_IsMainDoorOpen(bool check = false)
		{
			//
			if (FM.m_stMasterOpt.nUseSkipDoor == 1) return false;

			bool bCheckStatus = m_bRun || m_iSeqStat == (int)EN_SEQ_STATE.INIT;

			bool bKeyIn_R  = IO.XV[(int)EN_INPUT_ID.xSYS_DR_Right_KeyIn];
			bool bKeyIn_L  = IO.XV[(int)EN_INPUT_ID.xSYS_DR_Left_KeyIn ];
            bool bActrOk_R = IO.XV[(int)EN_INPUT_ID.xSYS_DR_MainClose_R];
            bool bActrOk_L = IO.XV[(int)EN_INPUT_ID.xSYS_DR_MainClose_L];
			bool bClose_R  = bKeyIn_R && bActrOk_R;
			bool bClose_L  = bKeyIn_L && bActrOk_L;


			//EPU.fn_SetErr(EN_ERR_LIST.ERR_0081, !IO.XV[(int)EN_INPUT_ID.xLOD_DoorClose]);

			if (check)
			{
				bool bOpen = false;
				if (!bKeyIn_R) bOpen = true;
				if (!bKeyIn_L) bOpen = true;

				return bOpen;
			}

			if (bCheckStatus)
			{
                if (!bClose_R) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0130); return true; }
                if (!bClose_L) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0131); return true; }
                
			}
			else //Warning Error 
			{
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0080, !bKeyIn_R);
                EPU.fn_SetErr(EN_ERR_LIST.ERR_0081, !bKeyIn_L);

                if (!bClose_R) { return true; }
                if (!bClose_L) { return true; }
            }

			return false;

		}
		//---------------------------------------------------------------------------
		public bool fn_IsSideDoorOpen(bool check = false)
		{
			//
			if (FM.m_stMasterOpt.nUseSkipDoor == 1) return false;

			bool bCheckStatus = m_bRun || m_iSeqStat == (int)EN_SEQ_STATE.INIT;
            bool bKeyIn  = IO.XV[(int)EN_INPUT_ID.xSYS_DR_SideDoor_KeyIn];
            bool bActrOk = IO.XV[(int)EN_INPUT_ID.xSYS_DR_SideDoorClose ];
			bool bClose  = bKeyIn && bActrOk;

			if (check)
			{
				bool bOpen = false;
				if (!bKeyIn) bOpen = true;

				return bOpen;
			}

			if (bCheckStatus)
			{
				if (!bClose) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0132); return true; }

			}
			else //Warning Error 
			{

				EPU.fn_SetErr(EN_ERR_LIST.ERR_0082, !bKeyIn);
				if (!bClose) return true; 

			}

			return false;

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Grid Sensor status
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  9:25
		*/
		public bool fn_IsGridChecked()
		{
			bool bOk = true;



			return bOk;

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Motor Error Condition Check
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/4  10:52
		*/

		public bool fn_InspectMotor()
		{
            bool bOk    = true;
			int  nErrNo = -1  ;
			bool isNoRun = SEQ._iStep == 0 && MAN._nManNo == 0;


			if (!MOTR._bConnect) return bOk;

			for (int m = 0; m < MOTR._iNumOfMotr; m++ )
			{
				//Motor Alarm.
				nErrNo = MOTR.ErrNoAlarm((EN_MOTR_ID)m);
				if (EPU.fn_SetErr(nErrNo, MOTR[m].GetAlarm())) bOk = false;
				
                //CW Limit
                nErrNo = MOTR.ErrNoCW((EN_MOTR_ID)m);
                if (EPU.fn_SetErr(nErrNo, isNoRun && MOTR[m].GetCW() && MOTR[m].GetHomeEnd())) bOk = false;

                //CCW Limit
                nErrNo = MOTR.ErrNoCCW((EN_MOTR_ID)m);
                if (EPU.fn_SetErr(nErrNo, isNoRun && MOTR[m].GetCCW() && MOTR[m].GetHomeEnd())) bOk = false;

            }

            return bOk;

		}
		//---------------------------------------------------------------------------
		public bool fn_InspectActuator()
		{
            //Local Var.
            bool bOk = true;

			if (!IO._bConnect) return bOk;

            //Inspect.
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0460, ACTR.Err((int)EN_ACTR_LIST.aSpdl_LensCovr   ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0461, ACTR.Err((int)EN_ACTR_LIST.aSpdl_PlateClamp ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0462, ACTR.Err((int)EN_ACTR_LIST.aspdl_IR         ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0463, ACTR.Err((int)EN_ACTR_LIST.aPoli_Clamp      ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0464, ACTR.Err((int)EN_ACTR_LIST.aClen_Clamp      ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0465, ACTR.Err((int)EN_ACTR_LIST.aStrg_LockBtm    ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0466, ACTR.Err((int)EN_ACTR_LIST.aStrg_LockTop    ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0467, ACTR.Err((int)EN_ACTR_LIST.aTran_TopLoadFB  ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0468, ACTR.Err((int)EN_ACTR_LIST.aTran_TopLoadTurn) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0469, ACTR.Err((int)EN_ACTR_LIST.aTran_BtmLoadFB  ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0470, ACTR.Err((int)EN_ACTR_LIST.aTran_LoadPortUD ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0471, ACTR.Err((int)EN_ACTR_LIST.aTran_MagaMoveLR ) != 0)) bOk = false;
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0472, ACTR.Err((int)EN_ACTR_LIST.aTran_LoadCover  ) != 0)) bOk = false;
																			   
																			   
			return bOk;

        }
		//---------------------------------------------------------------------------
		public void fn_InspectHold()
		{
			bool r1 = SEQ_SPIND._nSeqStep == 0;
			bool r2 = SEQ_POLIS._nSeqStep == 0;
			bool r3 = SEQ_CLEAN._nSeqStep == 0;
			bool r4 = SEQ_STORG._nSeqStep == 0;
			bool r5 = SEQ_TRANS._nSeqStep == 0;

			bool bHold = m_bRun && !m_bStop &&  r1 && r2 && r3 && r4 && r5;

			bool isExtLoad  = !DM.MAGA[(int)EN_MAGA_ID.LOAD  ].IsAllEmpty();
			bool isExtPOL   = !DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllEmpty();
			bool isExtCNL   = !DM.MAGA[(int)EN_MAGA_ID.CLEAN ].IsAllEmpty();
			bool isExtTrans = !DM.MAGA[(int)EN_MAGA_ID.TRANS ].IsAllEmpty();

			bool isExtPlate = isExtLoad || isExtPOL || isExtCNL || isExtTrans;
			if (m_tHoldTimer.OnDelay(bHold && isExtPlate, 300 * 1000))
			{
				//fn_UserMsg("MACHINE HOLD!!!! [5min]");
				EPU.fn_SetErr(EN_ERR_LIST.ERR_0119, true); //Machine Hold Error
				m_bStop = true;
			}

            //Check Run Wait
			//if (m_tHoldTimer.OnDelay(bHold && !isExtPlate, 300 * 1000)) FLG_RUN_WAIT = true;
			//if (FLG_RUN_WAIT && !bHold) FLG_RUN_WAIT = false;
			//
			//EPU.fn_SetErr(EN_ERR_LIST.ERR_0060, FLG_RUN_WAIT); //Machine Run Wait...
		}
        //---------------------------------------------------------------------------
        /**    
		@brief     Check System Fan
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2020/1/2  9:51
		*/
        public bool fn_InspectSysFan()
		{
			bool bErr = false;

			if (FM.m_stMasterOpt.nUseSkipFan == 1) return bErr;
			
			if (!IO.XV[(int)EN_INPUT_ID.xSW_Fan_Limit])
			{

				m_tFanChkTimer[0].OnDelay(IO.XV[(int)EN_INPUT_ID.xSYS_In_FanAlarm_01 ], 5000);
                m_tFanChkTimer[1].OnDelay(IO.XV[(int)EN_INPUT_ID.xSYS_In_FanAlarm_02 ], 5000);
                m_tFanChkTimer[2].OnDelay(IO.XV[(int)EN_INPUT_ID.xSYS_Out_FanAlarm_01], 5000);
                m_tFanChkTimer[3].OnDelay(IO.XV[(int)EN_INPUT_ID.xSYS_Out_FanAlarm_02], 5000);

                if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0135, m_tFanChkTimer[0].Out)) bErr = true;
                if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0136, m_tFanChkTimer[1].Out)) bErr = true;
                if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0137, m_tFanChkTimer[2].Out)) bErr = true;
                if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0138, m_tFanChkTimer[3].Out)) bErr = true;

			}
			else
			{
				m_tFanChkTimer[0].Clear();
                m_tFanChkTimer[1].Clear();
				m_tFanChkTimer[2].Clear();
				m_tFanChkTimer[3].Clear();
			}

			//if (!IO.XV[(int)EN_INPUT_ID.xSYS_PC_FanAlarm01]) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0139); bErr = true; }

			m_tFanChkTimer[4].OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_PC_FanAlarm01], 5000);
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0139, m_tFanChkTimer[4].Out)) bErr = true;



			return bErr;
		}
		//---------------------------------------------------------------------------
		public bool fn_InspectLeak()
		{
			bool bErr = false;

			if (!IO._bConnect) return bErr;

			//ACS Valve State
			EPU.fn_SetErr(EN_ERR_LIST.ERR_0099, IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_49_ACSValveState] == 1);

            if ( FM.m_stMasterOpt.nUseSkipLeak == 1) return bErr;
			

			//IO --> SICK DATA
			m_tLeakChkTimer[(int)EN_PLC_IN.Leak_Polishing    ].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_Polishing    ), 300);
            m_tLeakChkTimer[(int)EN_PLC_IN.Leak_CleanBottom  ].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_CleanBottom  ), 300);
            m_tLeakChkTimer[(int)EN_PLC_IN.Leak_CleanTop     ].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_CleanTop     ), 300);
            m_tLeakChkTimer[(int)EN_PLC_IN.Leak_LocalBtmPlate].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_LocalBtmPlate), 300);
            m_tLeakChkTimer[(int)EN_PLC_IN.Leak_BtmSolBox    ].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_BtmSolBox    ), 300);
            m_tLeakChkTimer[(int)EN_PLC_IN.Leak_Settling     ].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_Settling     ), 300);
            m_tLeakChkTimer[(int)EN_PLC_IN.Leak_UtilInlet    ].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_UtilInlet    ), 300);
            m_tLeakChkTimer[(int)EN_PLC_IN.Leak_LocalFloor   ].OnDelay(IO.fn_GetPLCIN((int)EN_PLC_IN.Leak_LocalFloor   ), 300);
			
			if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0140, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_Polishing    ].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0141, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_CleanBottom  ].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0142, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_CleanTop     ].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0143, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_LocalBtmPlate].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0144, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_BtmSolBox    ].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0145, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_Settling     ].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0146, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_UtilInlet    ].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0147, m_tLeakChkTimer[(int)EN_PLC_IN.Leak_LocalFloor   ].Out)) bErr = true;


			return bErr; 
		}
		//---------------------------------------------------------------------------
		public bool fn_InspectAccura()
		{
			bool bErr = false;

			if ( FM.m_stMasterOpt.nUseSkipAccura == 1) return bErr;
			if (!IO._bConnect                        ) return bErr;

			//SICK DATA
			m_tAccuChkTimer[(int)EN_PLC_IN.Accura_Gas ].OnDelay(!IO.fn_GetPLCIN((int)EN_PLC_IN.Accura_Gas ), 300);
			m_tAccuChkTimer[(int)EN_PLC_IN.Accura_Temp].OnDelay(!IO.fn_GetPLCIN((int)EN_PLC_IN.Accura_Temp), 300);
			
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0133, m_tAccuChkTimer[(int)EN_PLC_IN.Accura_Gas ].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0134, m_tAccuChkTimer[(int)EN_PLC_IN.Accura_Temp].Out)) bErr = true;

			return bErr; 
		}
        //---------------------------------------------------------------------------
        public bool fn_InspectDP() //JUNG/210114
        {
            bool bErr = false;

            if (FM.m_stMasterOpt.nUseSkipDP == 1) return bErr;
            if (!IO._bConnect                   ) return bErr;

			//SICK DATA
			m_tDPTimer[0].OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_DP_IN ], 1000);
			m_tDPTimer[1].OnDelay(!IO.XV[(int)EN_INPUT_ID.xSYS_DP_OUT], 1000);

            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0155, m_tDPTimer[0].Out)) bErr = true;
            if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0156, m_tDPTimer[1].Out)) bErr = true;

            return bErr;
        }

        //---------------------------------------------------------------------------
        public bool fn_InspectWaterLvl()
		{
            bool bErr = false;

			if (FM.m_stMasterOpt.nUseSkipWaterLvl == 1) return false ;
            
		  //if ( IO.XV[(int)EN_INPUT_ID.xSYS_BathWaterLimit    ]) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0148); bErr = true; } //B
            if (!IO.XV[(int)EN_INPUT_ID.xSYS_SettlingWaterLimit]) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0149); bErr = true; } //
			if (!IO.XV[(int)EN_INPUT_ID.xCLN_UtilLevelChk      ]) { EPU.fn_SetErr(EN_ERR_LIST.ERR_0436); bErr = true; }
			

			return bErr;

		}
		//---------------------------------------------------------------------------
		public bool fn_IsCLNLvlErr()
		{
			if (FM.m_stMasterOpt.nUseSkipWaterLvl == 1) return false ;

			if (!IO.XV[(int)EN_INPUT_ID.xCLN_UtilLevelChk]) return true;

			return false; 

		}

		//-------------------------------------------------------------------------------------------------
		public bool fn_InspectPower()
		{
			bool bErr = false;

			for (int n = 0; n< 38; n++)
			{
				if (n == 21) continue;
				if (n == 15) continue;
				if (n == 27) continue;
				if (n == 35) continue;

				m_tPowerTimer[n].OnDelay(!IO.XV[(int)n], 500);
			}

            for (int n = 0; n < 38; n++)
            {
                if (n == 21) continue;
				if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0501 + n, m_tPowerTimer[n].Out)) bErr = true;
			}

			return bErr;


		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Reset Function
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  14:04
		*/
		public void fn_Reset()
		{
			if (m_bRun) return;
			if (m_iSeqStat == (int)EN_SEQ_STATE.INIT) return;

            m_tEMOTimer1.Clear();
            m_tEMOTimer2.Clear();
            m_tEMOTimer3.Clear();

            for (int n=0; n<40; n++)
            {
				m_tPowerTimer[n].Clear();
			}
            for (int n = 0; n < 10; n++)
            {
				m_tFanChkTimer[n].Clear();
            }


            //Unit Reset
            SEQ_SPIND.fn_Reset();
			SEQ_POLIS.fn_Reset();
			SEQ_CLEAN.fn_Reset();
			SEQ_STORG.fn_Reset();
			SEQ_TRANS.fn_Reset();

			//Motor(ACS) Error Clear
			//MOTOR.fn_ClearACSError();

			//Manual Reset
			MAN.fn_Reset();

			//Error Reset
			EPU.fn_Clear();
			ACTR.fn_Reset();

			//
			LAMP.fn_BuzzOff(false);

			//Load Cell 
			LDCBTM.fn_Reset();

			//Auto Supply
			SUPPLY[SPLY_SLURRY].fn_Reset();
			SUPPLY[SPLY_SOAP  ].fn_Reset();

			//IO
			IO.fn_Reset();

			//
			MOTR.fn_Reset();

			//
			if (fn_InspectE3000()) SEQ_SPIND.fn_SetSpindleReset();

			MAIN.fn_CameraReset();


			//Step Clear
			m_iSeqStat = 0;
			m_iStep = 0;


			//
			if (FormPass != null && FormPass.IsVisible)
			{
				FormPass.Hide();
			}

			//JUNG/200519
			m_bEMSOn = false;

			//JUNG/200605/form visible Check
			if (UserJog != null && UserJog.Visibility == Visibility.Visible) fn_UserJogClose() ;
			if (UserMsg != null && UserMsg.Visibility == Visibility.Visible) fn_UserMsgClose() ;

			




		}

		//---------------------------------------------------------------------------
		/**    
		@brief     Update Sequence Status
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/10/18  10:39
		*/
		public void fn_UpdateSeqState()
		{
			//Local Var.
			bool IsErr     = EPU.fn_GetHasErr ();
			bool IsWarn    = EPU.fn_GetHasWarn();
			bool IsHoming  = MAN.fn_IsHoming  ();
			bool IsRunWarn = m_bRun && IsWarn;


			//500ms
			if (m_bFlick1) { m_tOnFlick1Timer .Clear(); if (m_tOffFlick1Timer.OnDelay( m_bFlick1, 500)) m_bFlick1 = false;}
			else           { m_tOffFlick1Timer.Clear(); if (m_tOnFlick1Timer .OnDelay(!m_bFlick1, 500)) m_bFlick1 = true; }

			//1sec
			if (m_bFlick2) { m_tOnFlick2Timer .Clear(); if (m_tOffFlick2Timer.OnDelay( m_bFlick2, 1000)) m_bFlick2 = false;}
			else           { m_tOffFlick2Timer.Clear(); if (m_tOnFlick2Timer .OnDelay(!m_bFlick2, 1000)) m_bFlick2 = true; }

			//2sec
			if (m_bFlick3) { m_tOnFlick3Timer .Clear(); if (m_tOffFlick3Timer.OnDelay( m_bFlick3, 2000)) m_bFlick3 = false;}
			else           { m_tOffFlick3Timer.Clear(); if (m_tOnFlick3Timer .OnDelay(!m_bFlick3, 2000)) m_bFlick3 = true; }


			m_bOneShotFlick1 = m_OneShotFlick1.IsRising(m_bFlick1);

			//Set Sequence Status
			if      (IsHoming ) m_iSeqStat = (int)EN_SEQ_STATE.INIT   ;
			else if (IsErr    ) m_iSeqStat = (int)EN_SEQ_STATE.ERROR  ;
			else if (IsRunWarn) m_iSeqStat = (int)EN_SEQ_STATE.RUNWARN;
			else if (m_bRun   ) m_iSeqStat = (int)EN_SEQ_STATE.RUNNING;
			else if (IsWarn   ) m_iSeqStat = (int)EN_SEQ_STATE.WARNING;
			else                m_iSeqStat = (int)EN_SEQ_STATE.STOP   ;


			//Machine State Update - Lamp, Buzzer
			LAMP.fn_Update(m_iSeqStat);

			//
			SPC.fn_Update((EN_SEQ_STATE)m_iSeqStat);


			//Update Switch Lamp
			//IO.YV[(int)EN_OUTPUT_ID.ySW_Start] = (m_iStep != 0); //Start
			//IO.YV[(int)EN_OUTPUT_ID.ySW_Stop ] = !m_bRun; //Stop
			
			//IO.YV[(int)EN_OUTPUT_ID.ySW_Reset] =  IsErr ? m_bFlick1 : (IO.XV[(int)EN_INPUT_ID.xSW_Reset] || m_bBtnWinReset); //Reset

		}
		//--------------------------------------------------------------------------
		/**    
		<summary>
			Save Current MC Data	
		</summary>
		@author    정지완(JUNGJIWAN)
		@date      2020/03/06 10:45
		*/
		public void fn_SaveWorkInfo(bool TO = false)
		{
			//Save Data
			DM.fn_LoadMap     (flSave); 
			   
			   fn_LoadWorkInfo(flSave);
			FM.fn_LoadLastInfo(flSave);

			//
			if(TO) fn_WriteLog("Save Work Info while Stop TimeOut");
			else   fn_WriteLog("Save Work Info");

		}
		//---------------------------------------------------------------------------
		public void fn_SaveWorkMap()
		{
			//Save Data
			DM.fn_LoadMap  (flSave); 
			fn_LoadWorkInfo(flSave);

			System.Console.WriteLine("fn_SaveWorkMap()");
		}
		//---------------------------------------------------------------------------
		/**    
		<summary>
			Map Data Load
		</summary>
		<param name="bLoad"> Load Option </param>
		@author    정지완(JUNGJIWAN)
		@date      2020/02/08 15:13
		*/
		public void fn_Load(bool bLoad, FileStream fp)
		{
			if (bLoad)
			{

				BinaryReader br = new BinaryReader(fp);

				m_bRecipeOpen = br.ReadBoolean();

			}
			else
			{

				BinaryWriter bw = new BinaryWriter(fp);

				bw.Write(m_bRecipeOpen);


				bw.Flush();

			}
		}
		//---------------------------------------------------------------------------
		/**    
		@brief     m_fn_SaveWorkInfo(bool bLoad)
		@return    
		@param     
		@remark    각 Unit 별 DATA Load/Save | 설비 정지 시 함수 호출 진행
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/9/9  9:52
		*/
		public void fn_LoadWorkInfo(bool bLoad)
		{
			//Local Var.
			string sPath = UserFile.fn_GetExePath();
			sPath += "SeqData";

			bool bExist = false ;
			if (UserFile.fn_CheckDir(sPath))
			{
				sPath += "\\SeqData.dat";

				bExist = UserFile.fn_CheckFileExist(sPath);
				if (bLoad && !bExist) return;
			}
			else return;

			try
			{
				FileStream fs = null;

				if (bLoad)
				{
					fs = new FileStream(sPath, FileMode.Open, FileAccess.Read);
				}
				else
				{
					if (bExist) fs = new FileStream(sPath, FileMode.Open     , FileAccess.Write);
					else        fs = new FileStream(sPath, FileMode.CreateNew, FileAccess.Write);
				}

				//SequenceUnit 별 Data
				SEQ      .fn_Load(bLoad, fs);
				SEQ_SPIND.fn_Load(bLoad, fs);
				SEQ_POLIS.fn_Load(bLoad, fs);
				SEQ_CLEAN.fn_Load(bLoad, fs);
				SEQ_STORG.fn_Load(bLoad, fs);
				SEQ_TRANS.fn_Load(bLoad, fs);
				
				fs.Close();

				//
				SEQ_SPIND.fn_LoadVisnResult(bLoad);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
			}

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Auto Run Cycle Function
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/4  13:48
		*/
		private bool fn_AutoRun1()
		{

			nAR_Start[0] = Environment.TickCount;
			if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.SPINDLE]) { SEQ_SPIND.fn_AutoRun(); }
			nAR_Scan[0] = Environment.TickCount - nAR_Start[0];

			nAR_Start[1] = Environment.TickCount;
			if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.POLISH]) { SEQ_POLIS.fn_AutoRun(); }
			nAR_Scan[1] = Environment.TickCount - nAR_Start[1];

			nAR_Start[2] = Environment.TickCount;
			if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.CLEAN]) { SEQ_CLEAN.fn_AutoRun(); }
			nAR_Scan[2] = Environment.TickCount - nAR_Start[2];

			nAR_Start[3] = Environment.TickCount;
			if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.STORAGE]) { SEQ_STORG.fn_AutoRun(); }
			nAR_Scan[3] = Environment.TickCount - nAR_Start[3];

			nAR_Start[4] = Environment.TickCount;
			if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.TRANSFER]) { SEQ_TRANS.fn_AutoRun(); }
			nAR_Scan[4] = Environment.TickCount - nAR_Start[4];

			return true;
		}
		//---------------------------------------------------------------------------
		private bool fn_AutoRun2()
		{
			return true;

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Panel Button Check
		@return	
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2020/1/2  10:17
		*/
		private void fn_CheckMaualButton()
		{
			//Local Var.
			bool bDoorOpen = fn_IsAnyDoorOpen(true);

			//Check Auto Key Enable Condition
			IO.fn_SetAutoKey();

			if (!IO.XV[(int)EN_INPUT_ID.xSW_Auto]) m_bAuto = false;
			if (!m_bAuto)
			{
				m_tAutoDelay.OnDelay(IO.XV[(int)EN_INPUT_ID.xSW_Auto], 2000);
				m_bAuto = m_tAutoDelay.Out;
			}

			m_bEdgSTRLock = m_OneShotSTR.IsRising(IO.XV[(int)EN_INPUT_ID.xSW_STOR_Unlock]);
			if (m_bEdgSTRLock)
			{
				if (bDoorOpen)
				{
					fn_UserMsg("Please Door Close.");
					return; 
				}
				//1) Storage move front
				//2) door open
				MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0444, true, false);
			}

			m_bEdgSTRUnlock = m_OneShotSTR.IsRising(IO.XV[(int)EN_INPUT_ID.xSW_STOR_Lock]);
			if (m_bEdgSTRUnlock)
			{
                if (bDoorOpen)
                {
                    fn_UserMsg("Please Door Close.");
                    return;
                }
				MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0445, true, false);  //SEQ_STORG.fn_ReqMoveToolChkPos();

			}


            //Door Lock
            m_bEdgDoorClose = m_EdgDoorOpen.IsRising(IO.XV[(int)EN_INPUT_ID.xSW_DoorLock]);
			if (m_bEdgDoorClose) 
			{ 
				if(bDoorOpen)
				{
                    fn_UserMsg("Please Door Close.");
                    return;
                }
                fn_DoorLock  (); 
			}

			//Door Unlock
			m_bEdgDoorOpen = m_EdgDoorClose.IsRising(IO.XV[(int)EN_INPUT_ID.xSW_DoorUnlock]);
			if (m_bEdgDoorOpen && !m_bRun && !m_bAuto) 
			{
				if (fn_IsAllMotorStop())
				{
					fn_DoorUnLock();
				}
			}	
									   

		}
		//---------------------------------------------------------------------------
		public bool fn_IsAllMotorStop()
		{
			for (int m= 0; m<MAX_MOTOR; m++)
			{
				if (!MOTR[m].GetStop()) return false;  
			}

			return true ; 

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Start Condition of each unit
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/4  11:09
		*/
		private bool fn_ToStartCon()
		{
			bool r1 = TS_RSLT[(int)EN_PART_ID.piSPDL] = SEQ_SPIND.fn_ToStartCon();
			bool r2 = TS_RSLT[(int)EN_PART_ID.piPOLI] = SEQ_POLIS.fn_ToStartCon();
			bool r3 = TS_RSLT[(int)EN_PART_ID.piCLEN] = SEQ_CLEAN.fn_ToStartCon();
			bool r4 = TS_RSLT[(int)EN_PART_ID.piSTRG] = SEQ_STORG.fn_ToStartCon();
			bool r5 = TS_RSLT[(int)EN_PART_ID.piLOAD] = SEQ_TRANS.fn_ToStartCon();

			return r1 && r2 && r3 && r4 && r5;
		}
		//---------------------------------------------------------------------------
		private bool fn_ToStart()
		{

			bool r1 = SEQ_SPIND.fn_ToStart();
            bool r2 = SEQ_POLIS.fn_ToStart();
            bool r3 = SEQ_CLEAN.fn_ToStart();
            bool r4 = SEQ_STORG.fn_ToStart();
			bool r5 = SEQ_TRANS.fn_ToStart();

			return r1 && r2 && r3 && r4 && r5;
		}
		//---------------------------------------------------------------------------
		private bool fn_ToStopCon()
		{

			bool r1 = TS_RSLT[(int)EN_PART_ID.piSPDL] = SEQ_SPIND.fn_ToStopCon();
            bool r2 = TS_RSLT[(int)EN_PART_ID.piPOLI] = SEQ_POLIS.fn_ToStopCon();
            bool r3 = TS_RSLT[(int)EN_PART_ID.piCLEN] = SEQ_CLEAN.fn_ToStopCon();
            bool r4 = TS_RSLT[(int)EN_PART_ID.piSTRG] = SEQ_STORG.fn_ToStopCon();
			bool r5 = TS_RSLT[(int)EN_PART_ID.piLOAD] = SEQ_TRANS.fn_ToStopCon();

			return r1 && r2 && r3 && r4 && r5;

		}
		//---------------------------------------------------------------------------
		private bool fn_ToStop()
		{
			bool r1 = SEQ_SPIND.fn_ToStop();
            bool r2 = SEQ_POLIS.fn_ToStop();
            bool r3 = SEQ_CLEAN.fn_ToStop();
            bool r4 = SEQ_STORG.fn_ToStop();
			bool r5 = SEQ_TRANS.fn_ToStop();

			if(r1 && r2 && r3 && r4 && r5)
            {
				fn_SetLight(true);

				return true; 
            }

			return false;
		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Start
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/4  10:57
		*/
		private bool fn_CheckStart()
		{


			return true;
		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Start Condition
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/4  10:57
		*/
		private bool fn_CheckStartCon()
		{
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			//Motor Check
			if (!MOTR.IsAllHomeEnd())
			{
				fn_UserMsg("Check Motor All Home.", EN_MSG_TYPE.Warning, "Warning");
				return false;
			}

            if (!MOTR.IsAllServoOn())
            {
                fn_UserMsg("Check Motor All Servo ON.", EN_MSG_TYPE.Warning, "Warning");
                return false;
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //Man 동작 중이면...
            if (MAN._nManNo > 0)
			{
                fn_UserMsg("Manual ", EN_MSG_TYPE.Warning, "Warning");
                //fn_ShowWarn(true, "Check Motor All Home.");
                return false;

            }

			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //Storage Sensor Check
            if (!SEQ_STORG.fn_IsLockPos())
            {
                fn_UserMsg(string.Format($"Check Storage Position.[ADD:{(int)EN_INPUT_ID.xSTR_PosCheck:D4}]"));
                return false;
            }
			else
			{
				SEQ_STORG.fn_MoveCylLock1(ccFwd);
			}

			//Storage && Tool Exist Check
			if (DM.MAGA[(int)EN_MAGA_ID.MAGA01].IsExistReady() || DM.MAGA[(int)EN_MAGA_ID.MAGA02].IsExistReady())
			{
				if (DM.STOR[siPolish].IsAllStat((int)EN_PIN_STAT.psEmpty) && DM.TOOL.IsAllEmpty())
				{
					fn_UserMsg(string.Format("Check Polishing Tool Exist. All Empty now"));
					return false;
				}

				if (DM.STOR[siClean].IsAllStat((int)EN_PIN_STAT.psEmpty) && DM.TOOL.IsAllEmpty())
				{
					fn_UserMsg(string.Format("Check Cleaning Tool Exist. All Empty now"));
					return false;
				}
			}
			//JUNG/210119
			bool bExtPolish = DM.MAGA[(int)EN_MAGA_ID.POLISH].IsAllStat((int)EN_PLATE_STAT.ptsPolish);
			if (bExtPolish && DM.STOR[siPolish].IsAllStat((int)EN_PIN_STAT.psEmpty) && DM.TOOL.IsAllEmpty())
			{
                fn_UserMsg(string.Format("Check Polishing Tool Exist. All Empty now"));
                return false;
			}

			bool bExtClean = DM.MAGA[(int)EN_MAGA_ID.CLEAN].IsAllStat((int)EN_PLATE_STAT.ptsClean);
			if (bExtClean && DM.STOR[siClean].IsAllStat((int)EN_PIN_STAT.psEmpty) && DM.TOOL.IsAllEmpty())
            {
                fn_UserMsg(string.Format("Check Cleaning Tool Exist. All Empty now"));
                return false;
            }

            //Plate Map Check and Recipe Open Flag Off
            if (DM.fn_IsMagaAllEmpty() && m_bRecipeOpen)
			{
				m_bRecipeOpen = false;
				fn_WriteLog("[Check Start] Recipe Open - OFF");
			}
			
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			//Vision/CAM Check
			if (!g_VisionManager._CamManager._bConnect && !FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE))
			{
                fn_UserMsg(string.Format("Check Camera Condition."));
                return false;
            }
            if (!g_VisionManager._LightManger._bConnect && !FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE))
            {
                fn_UserMsg(string.Format("Check Light Condition."));
                return false;
            }
     		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if (fn_InspectE3000()) 
			{
                fn_UserMsg(string.Format("Check Spindle Error!!!"));
                return false;
            }

			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			//Check Magazine Exist
			bool xMaga01Exist = SEQ_TRANS.fn_IsExistMagazine(EN_MAGA_ID.MAGA01, true); 
            bool xMaga02Exist = SEQ_TRANS.fn_IsExistMagazine(EN_MAGA_ID.MAGA02, true); 
			if (!xMaga01Exist && !xMaga02Exist)
			{
                fn_UserMsg(string.Format("Check Magazine!!!"));
                return false;
            }

			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			//Check Load Cell Connection
			if (!LDCBTM._bConect)
			{
                fn_UserMsg(string.Format("Check Bottom Load Cell Connection!!!"));
                return false;
            }

			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			//Check Cup position
			//if(FM.m_stSystemOpt.nUsePolishingCup == 1 && !SEQ_SPIND.fn_IsExistCup(true))
            //{
            //    fn_UserMsg(string.Format("Check Cup position."));
            //    return false;
            //}

			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if(FM.m_stSystemOpt.nUseAutoSlurry == 1)
			{
				if(!SUPPLY[0]._bConnect || !SUPPLY[1]._bConnect)
				{
                    fn_UserMsg(string.Format("Check Auto Supply System."));
                    return false;
                }
            }

			//JUNG/201210/Check Tool Position
			if (!SEQ_STORG.fn_CheckPos())
            {
                fn_UserMsg(string.Format("Check Storage Tool Position Data. Use Auto Calibration Method."));
                return false;
            }

            return true;

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Start Time Out
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  16:48
		*/
		private void fn_CheckToStartTO()
		{

			int nTime = 20; //Timeout time

			if (m_iStep == 14)
			{
				if (!m_tToStartTimer.OnDelay(true, nTime * 1000)) return; //20sec 동안 Start 못하면...
			}
			else
			{
				m_tToStartTimer.Clear();
				return;
			}

			//Start Unit Check
			string sMsg = string.Format($"To Start Timeout : m_iStep : {m_iStep}");
			fn_WriteLog(sMsg);

			LOG.fn_CrntStateTrace(EN_SEQ_ID.ALL, "Title : TO START TIME OUT");

			//
			EPU.fn_SetErr(EN_ERR_LIST.ERR_0400);

			//Clear Step
			m_iStep = 0;
			m_bRun = false;

		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Stop Time Out
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  16:49
		*/
		private void fn_CheckToStopTO()
		{
			int nTime = 40; //Timeout time
			
			//JUNG/200827/Polshing Cycle 제외
			bool bExCase = SEQ_SPIND._bDrngPolishing || SEQ_SPIND._bDrngCleaning; 
			if (m_bStop || m_iStep == 16 && !bExCase)
			{
				if (!m_tToStopTimer.OnDelay(true, nTime * 1000)) return; //설정 시간 동안 Stop 못하면...
			}
			else
			{
				m_tToStopTimer.Clear();
				return;
			}

			//Start Unit Check
			string sMsg;
			sMsg = string.Format($"To Stop Timeout : m_bStop = {m_bStop}");
			fn_WriteLog(sMsg);

			LOG.fn_CrntStateTrace(EN_SEQ_ID.ALL, "Title : TO STOP TIME OUT");

			//
			EPU.fn_SetErr(EN_ERR_LIST.ERR_0165); 

			//Clear Step
			m_iStep = 0;
			m_bRun  = false;
			m_bStop = false;

            //Data Save
            fn_SaveWorkInfo(true);

        }
		//---------------------------------------------------------------------------
		/**    
		@brief     Door Lock Setting
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  16:28
		*/
		public void fn_DoorLock()
		{
			fn_SetDoorLock     (ccClose);
			fn_SetDoorLock_Side(ccClose);
		}
		//---------------------------------------------------------------------------
		public void fn_DoorUnLock()
		{
			fn_SetDoorLock     (ccOpen);
			fn_SetDoorLock_Side(ccOpen);
		}
		//---------------------------------------------------------------------------
		public void fn_SetDoorLock(int nLock)
		{
			//Door Lock Setting
			if (nLock == ccOpen)
			{
                IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontLeft ] = true;
                IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontRight] = true;
            }
			else
			{
                IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontLeft ] = false;
                IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_FrontRight] = false;
            }

        }
		//---------------------------------------------------------------------------
		public void fn_SetDoorLock_Side(int nLock)
		{
			if (nLock == ccOpen)
			{
				IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_Side] = true;
			}
			else
			{
				IO.YV[(int)EN_OUTPUT_ID.ySYS_Door_Side] = false;
			}
		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Check Button Push Condition
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/1  15:15
		*/
		public void fn_CheckRunCon()
		{
			bool bErr       =  EPU.fn_GetHasErr();
			bool bErrDisp   =  EPU.fn_GetHasDisp();
			bool bDoorClose = !fn_IsAnyDoorOpen(true);

			bool IsbtnStart = m_bBtnWinStart;
			bool IsbtnStop  = m_bBtnWinStop;
			bool IsbtnReset = m_bBtnWinReset || IO.XV[(int)EN_INPUT_ID.xSW_Reset];

			m_bBtnWinStart = false;
			m_bBtnWinStop  = false;
			m_bBtnWinReset = false;

			IsbtnReset = m_OnshotReset.IsRising(IsbtnReset);

			if (!IsbtnStart) m_bBtnStart = false;
			if (!IsbtnStop ) m_bBtnStop  = false;
			if (!IsbtnReset) m_bBtnReset = false;

			if (!m_bBtnStart && IsbtnStart)
			{
				fn_WriteLog("----- STOP -----");
				m_bBtnStart = true;
			}
			if (!m_bBtnStop  && IsbtnStop ) m_bBtnStop = true;
			if (!m_bBtnReset && IsbtnReset)
			{
				m_bBtnReset = true;
				fn_WriteLog(">> RESET <<");
			}

			if (m_bScreenLock) return; //Screen Lock 상태 check

			if (m_bBtnStop ) { m_bBtnWinStop  = false; }
			if (m_bBtnReset) { m_bBtnWinReset = false; }
			if (m_bBtnStart)
			{
				m_bBtnWinStart       = false;
				EPU._bDisplayErrForm = false;

				//Door Lock
				fn_SetDoorLock(ccClose);

				bool bLvlOper = (FM._nCrntLevel == (int)EN_USER_LEVEL.lvOperator);
				bool bLvlMast = FM.fn_IsLvlMaster(); //(FM._nCrntLevel == (int)EN_USER_LEVEL.lvMaster  ); //Admin

				//Mode 자동 변경 - 
				if (!bLvlMast && bDoorClose)
				{
					//Default Setting
					FM.fn_DefaultSystemRunOpt();
					FM.fn_DefaultSystemChkOpt();
					
					MAIN._bReqOperLevel = true;

				}

			}

			//Step Condition
			m_bStopCon  = m_bBtnStop  ||  bErr;
			m_bRunCon   = m_bBtnStart && !bErr && !m_bRun && !m_bStopCon && (m_bAuto || FM.m_stMasterOpt.nRunMode == EN_RUN_MODE.TEST_MODE); //JUNG/Auto Check
			//m_bRunCon   = m_bBtnStart && !bErr && !m_bRun && !m_bStopCon ; //For TEST
			m_bResetCon = m_bBtnReset && !m_bRun;

			//Buzzer Off
			if (bErr && m_bBtnStop)
			{
				//Set Off
				LAMP.fn_BuzzOff();
			}

			//
			if      (m_bStopCon && (m_iStep == 0)) m_bRun  = false;
			else if (m_bStopCon && (m_iStep != 0)) m_bStop = true ;

			//Run Condition
			if (m_bRunCon && (m_iStep == 0))
			{
				m_bLtStop  = false;
				m_iStep    = 10;
				//m_iIniStep = 0;

				m_tToStartTimer.Clear();
				m_tStartDelay  .Clear();

				m_bRun = true; 
			}

			//Reset
			if (m_bResetCon            ) fn_Reset();
			if (m_bBtnReset && bErrDisp) EPU.fn_Clear();

			//Timeout Check
			fn_CheckToStartTO(); //Start
			fn_CheckToStopTO (); //Stop

			//Decide Step
			switch (m_iStep)
			{

				default: m_bLtStop = false;
					     m_bStop   = false; 
					     m_bRun    = false;
					     m_iStep   = 0;
					     break;

				case 10:

					if (!m_tStartDelay.OnDelay(true, 300)) return; // Start Delay for Door Close

					//Check Start Condition
					if (!fn_CheckStartCon       ()) { m_iStep = 0; return; }
											    
					//Inspect				    
					if ( fn_InspectEmergency    ()) { m_iStep = 0; return; }
					if ( fn_InspectMainAir      ()) { m_iStep = 0; return; }
					if (!fn_InspectSafety       ()) { m_iStep = 0; return; }
					if ( fn_InspectLeak         ()) { m_iStep = 0; return; }
					if ( fn_InspectSysFan       ()) { m_iStep = 0; return; }
					if ( fn_InspectCBoxEmergency()) { m_iStep = 0; return; }
					if ( fn_InspectAccura       ()) { m_iStep = 0; return; }

					//Check Motor Error
					if (!fn_InspectMotor        ()) { m_iStep = 0; return; }

					m_tStartDelay.Clear();
					m_iStep++;
					return;

				case 11:
					if (!m_tStartDelay.OnDelay(true, 500)) return; //

					//Check Motion Value
					if (!MOTR.InspectMinMax())
					{
						fn_UserMsg("Position or Velocity data Error.");
						m_iStep = 0;
						return;
					}

					//Check Manual Step
					if (MAN.fn_GetManStep() > (int)EN_MAN_LIST.MAN_NON)
					{
						fn_UserMsg("Manual Processing Now...", EN_MSG_TYPE.Warning);
						m_iStep = 0;
						return;
					}

					if (bErr) { m_iStep = 0; return; } //Check Error

					m_tStartDelay.Clear();
					m_iStep++;
					return;

				case 12:
					if (!fn_ToStartCon()) return;

                    //JUNG/201221/Add Data Save
                    fn_SaveWorkInfo();

                    m_iStep++;
					return;

				case 13:
					if (bErr) { m_iStep = 0; return; } //Check Error
					
					if (!fn_ToStart())  return;

					if(!SEQ_STORG.fn_IsExtToolBasket())
					{
						EPU.fn_SetErr(EN_ERR_LIST.ERR_0371); //fn_UserMsg(string.Format("Check Tool Discard Basket!!!"));
						m_iStep = 0;
						return;
                    }

                    //Motor Speed Set???
                    //cMainSeq->mc_Motor->m_fn_SetSpeed();

                    //Light
                    fn_SetLight(FM.m_stSystemOpt.nUseLightOnRun == 1);


                    m_tStartDelay.Clear();
					m_iStep++;
					return;

				case 14:
					m_bRun = true;
					if (!m_tDelayTimer.OnDelay(true, 500)) return;

					//Log
					fn_WriteLog("[MACHINE]START");

					EPU.fn_SetLastErr(-1);

					m_bStopErrAtRun = false;

					m_iStep++;
					return;

				case 15:

					m_tStopDelay.OnDelay(m_bStop, 300);
					//Auto Run Cycle
					if (m_bStop && m_tStopDelay.Out)
					{
						if (fn_ToStopCon())
						{
							m_bStop = false;
							m_iStep++;
						}
						else fn_AutoRun1();
					}
					else
					{
						if (m_bRun) fn_AutoRun1();
						else m_iStep++;
					}
					return;

				case 16:
					if (!fn_ToStop()) return;

					//Data Save
					fn_SaveWorkInfo();

					m_bRun = false;
					
					if (EPU.fn_GetHasErr()) m_bStopErrAtRun = true;

					//Log
					fn_WriteLog("[MACHINE] STOP");

					m_iStep = 0;
					return;

			}


		}
		//---------------------------------------------------------------------------
		/**    
		<summary>
			Actuator 간섭 Check	
		</summary>
		@author    정지완(JUNGJIWAN)
		@date      2020/02/14 19:39
		*/
		public bool fn_CheckDstbActr(int aNum, int Act)
		{
			//
			return true; 
		}

		//---------------------------------------------------------------------------
		/**    
		@brief     Update Scan Time
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/4  9:41
		*/
		public void fn_UpdateScanTime(DataGrid Grid)
		{
			//Local Var.
			if (Grid == null) return;
			//int n = 1;
	
			string sTemp = string.Empty; 


			//




			
			
		}
		//---------------------------------------------------------------------------
		/**
		@brief     Update Flag
		@return
		@param
		@remark
		-
		@author    정지완(JUNGJIWAN)
		@date      2019/10/11  15:21
		*/
		public void fn_UpdateFlag(ref Grid grd)
		{
			//Local Var.
			if (grd == null) return;

			//
			int nRow = 0;
			
			Items[nRow, 0].Content = "_nMCTYPE     ";  Items[nRow++, 1].Content = string.Format($"{_nMCTYPE       }");
			Items[nRow, 0].Content = "";               Items[nRow++, 1].Content = "";
			Items[nRow, 0].Content = "bRun         ";  Items[nRow++, 1].Content = string.Format($"{m_bRun         }");
			Items[nRow, 0].Content = "bStop        ";  Items[nRow++, 1].Content = string.Format($"{m_bStop        }");
			Items[nRow, 0].Content = "bBtnStart    ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnStart    }");
			Items[nRow, 0].Content = "bBtnStop     ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnStop     }");
			Items[nRow, 0].Content = "bBtnReset    ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnReset    }");
			Items[nRow, 0].Content = "bBtnWinStart ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnWinStart }");
			Items[nRow, 0].Content = "bBtnWinStop  ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnWinStop  }");
			Items[nRow, 0].Content = "bBtnWinReset ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnWinReset }");
			Items[nRow, 0].Content = "bBtnManReset ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnManReset }");
			Items[nRow, 0].Content = "bBtnManStart ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnManStart }");
			Items[nRow, 0].Content = "bBtnManStop  ";  Items[nRow++, 1].Content = string.Format($"{m_bBtnManStop  }");
			Items[nRow, 0].Content = "bResetCon    ";  Items[nRow++, 1].Content = string.Format($"{m_bResetCon    }");
			Items[nRow, 0].Content = "bRunCon      ";  Items[nRow++, 1].Content = string.Format($"{m_bRunCon      }");
			Items[nRow, 0].Content = "bStopCon     ";  Items[nRow++, 1].Content = string.Format($"{m_bStopCon     }");
			Items[nRow, 0].Content = "bLtStop      ";  Items[nRow++, 1].Content = string.Format($"{m_bLtStop      }");
			Items[nRow, 0].Content = "bLtRun       ";  Items[nRow++, 1].Content = string.Format($"{m_bLtRun       }");
			Items[nRow, 0].Content = "bScreenLock  ";  Items[nRow++, 1].Content = string.Format($"{m_bScreenLock  }");
			Items[nRow, 0].Content = "bAuto        ";  Items[nRow++, 1].Content = string.Format($"{m_bAuto        }");
			Items[nRow, 0].Content = "bNoSafety    ";  Items[nRow++, 1].Content = string.Format($"{m_bNoSafety    }");
			Items[nRow, 0].Content = ""             ;  Items[nRow++, 1].Content = string.Format("");
			Items[nRow, 0].Content = "bFlick1      ";  Items[nRow++, 1].Content = string.Format($"{m_bFlick1      }");
			Items[nRow, 0].Content = "bFlick2      ";  Items[nRow++, 1].Content = string.Format($"{m_bFlick2      }");
			Items[nRow, 0].Content = "bFlick3      ";  Items[nRow++, 1].Content = string.Format($"{m_bFlick3      }");
			Items[nRow, 0].Content = "bStopErrAtRun";  Items[nRow++, 1].Content = string.Format($"{m_bStopErrAtRun}");
			Items[nRow, 0].Content = ""             ;  Items[nRow++, 1].Content = string.Format("");
			Items[nRow, 0].Content = "nUD_Scan[0]"  ;  Items[nRow++, 1].Content = string.Format($"{nUD_Scan[0]    }");
			

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
		public void fn_SetLight(bool on)
		{
			IO.YV[(int)EN_OUTPUT_ID.ySYS_Light] = on || m_bLightOn;

		}
    }
}
