using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaferPolishingSystem;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.ActuatorId;

namespace WaferPolishingSystem.BaseUnit
{

    public class ManProcess
    {
	    int    m_nManNo            ; //Selected Manual No.
        int    m_nPrevManNo        ;
        int    m_nManStep          ;
	  //int    m_nRptMotrID        ;
	  //int    m_nRptActrID        ;
	  //int    m_nChngMotrDlay     ;
        int    m_nChngActrDlay     ;
        int    m_nHomeStep         ;
        int    m_nSelHomeAxis      ;
        int    m_iRptCmd           ;
        int    m_nBuffNo           ;
        int    m_nSelMagaSlot      ;
        int    m_nSelMaga          ;
        int    m_nWarmStep         ;
        int    m_iRMotrID          ;
        int    m_iChngMotrDlay     ;
        int    m_nWhere            ;


        bool   m_bJog              ; //JOG Flag.
        bool   m_bHoming           ; //For Homing.
      //bool   m_bOneShot          ;
	    bool   m_bRptMotr          ;
        bool   m_bRptActrIng       ;
	    bool[] m_bRptActr          ;
	    bool   m_bLtCycleRun       ;
        bool   m_bDrngWarm         ;
        bool   m_bWarmErr          ;
        bool   m_bStartWarm        ;

        bool   m_bDirMotr          ;
        

        double m_dDirectPos        ;
        double m_dP1, m_dV1, m_dA1 ;
        double m_dP2, m_dV2, m_dA2 ;
        double m_dDCOM             ;


        string m_sTemp; //For Log

		TOnDelayTimer m_HomeTimer         = new TOnDelayTimer();
	    TOnDelayTimer m_ManCycleTimer     = new TOnDelayTimer();
        TOnDelayTimer m_ManDelayTimer     = new TOnDelayTimer();
                                          
		TOnDelayTimer m_MotrOnTimer       = new TOnDelayTimer();
	    TOnDelayTimer m_MotrOffTimer      = new TOnDelayTimer();
		TOnDelayTimer m_ActrTimer         = new TOnDelayTimer();
                                          
        TOnDelayTimer m_tWarmDelay        = new TOnDelayTimer();
        TOnDelayTimer m_tCheckWarmCycle   = new TOnDelayTimer();
        TOnDelayTimer m_tWarmCycleTimeOut = new TOnDelayTimer();
        


        int          m_nWarmCnt ;
        bool      [] m_bRtn     = new bool[MAX_MOTOR];
        EN_COMD_ID[] m_cmdPos01 = new EN_COMD_ID[MAX_MOTOR];
        EN_COMD_ID[] m_cmdPos02 = new EN_COMD_ID[MAX_MOTOR];

        //Property
        public int _nManNo        { get { return m_nManNo      ;} } //set { m_nManNo = value  ; } }
        public int _nSelHomeAxis  { get { return m_nSelHomeAxis;} set { m_nSelHomeAxis = value  ; } }
        public int _nSelMagaSlot  { get { return m_nSelMagaSlot;} set { m_nSelMagaSlot = value  ; } }
        public int _nSelMaga      { get { return m_nSelMaga    ;} set { m_nSelMaga     = value  ; } }
        public int _nWhere        { get { return m_nWhere      ;} set { m_nWhere       = value  ; } }

        
        public int _nManStep      { get { return m_nManStep;  } set { m_nManStep = value; } }
        public bool _bHoming      { get { return m_bHoming; } }
        public bool _bJog         { get { return m_bJog;      } set { m_bJog = value    ; } }

        public bool _bRptActrIng  { get { return m_bRptActrIng;}  }

        public double _dDirectPos { get { return m_dDirectPos; } set { m_dDirectPos = value; }  }
        public double _dDCOM      { get { return m_dDCOM     ; } set { m_dDCOM      = value; }  }
        

        public bool _bDrngWarm    { get { return m_bDrngWarm; } }
        public bool _bWarmErr     { get { return m_bWarmErr ; } set { m_bWarmErr = value; } }
        public bool _bStartWarm   { get { return m_bStartWarm; } }
        
        public int _nWarmStep     { get { return m_nWarmStep; } }

        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public ManProcess()
        {

            m_nManNo        = (int)EN_MAN_LIST.MAN_NON;
            m_nPrevManNo    = (int)EN_MAN_LIST.MAN_NON;
            m_nManStep      = 0;
            m_nHomeStep     = 0;
            m_nSelHomeAxis  = 0;
            m_nWarmStep     = 0; 
          //m_nChngMotrDlay = 0;
            m_nChngActrDlay = 0;
            m_iRptCmd       = 0;
            m_nBuffNo       = 0;
            m_nWarmCnt      = 0;
            m_iRMotrID      = 0;
            m_iChngMotrDlay = 0;
            m_nWhere        = 0;

            m_nSelMagaSlot  = 0;
            m_nSelMaga      = 0;

            m_bJog          = false;
            m_bHoming       = false;
          //m_bOneShot      = false;
            m_bRptMotr      = false;
            m_bLtCycleRun   = false;
            m_bDrngWarm     = false;
            m_bWarmErr      = false;
            m_bStartWarm    = false;

            m_dDirectPos    = 0.0;

            m_dP1 = m_dV1 = m_dA1 = 0.0;
            m_dP2 = m_dV2 = m_dA2 = 0.0;
            m_dDCOM = 0;

            m_bRptActr     = new bool[ACTR._iNumOfACT];

            for (int n = 0; n< ACTR._iNumOfACT; n++)
            {
                m_bRptActr[n] = new bool();
            }

            //
            m_MotrOnTimer .Clear();
            m_MotrOffTimer.Clear();
            m_ActrTimer   .Clear();

            for (int m = 0; m<MAX_MOTOR; m++)
            {
                m_cmdPos01[m] = new EN_COMD_ID();
                m_cmdPos02[m] = new EN_COMD_ID();

                m_bRtn    [m] = new bool();
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
        @date      2019/11/21  14:41
        */
        public void fn_Reset()
        {                  
        	m_nManNo       = (int)EN_MAN_LIST.MAN_NON;
            m_nPrevManNo   = (int)EN_MAN_LIST.MAN_NON;
        	m_nManStep     = 0;
            m_nHomeStep    = 0;
            m_nSelHomeAxis = -1;

            m_bJog         = false;
        	m_bHoming      = false;
          //m_bOneShot     = false;
        	m_bRptMotr     = false;
        	m_bLtCycleRun  = false;

            //
            m_nWarmStep    = 0;
            m_bDrngWarm    = false;
            m_bWarmErr     = false;
            m_bStartWarm   = false;

            for (int n = 0; n < ACTR._iNumOfACT; n++)
            {
                m_bRptActr[n] = new bool();
            }


        }
        //---------------------------------------------------------------------------
        public bool fn_IsHoming()
        {
            return m_bHoming; 
        }
        //---------------------------------------------------------------------------
        public int fn_GetManStep()
        {
            return m_nManStep; 
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     All Home
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/11/21  14:41
        */
        public void fn_MoveAllHome()
        {
            //Door lock
            SEQ.fn_DoorLock();

            bool bErr = false; 
        
        	//Time Out.
        	m_HomeTimer.OnDelay(m_bHoming, 180 * 1000); //180sec
        	if (EPU.fn_SetErr(EN_ERR_LIST.ERR_0110, m_HomeTimer.Out))
        	{
        		m_nManNo  = (int)EN_ERR_LIST.ERR_NONE; 
        		m_bHoming = false;

                int nStepSpndle  = SEQ_SPIND._nHomeStep;
                int nStepPolish  = SEQ_POLIS._nHomeStep;
                int nStepClean   = SEQ_CLEAN._nHomeStep;
                int nStepStorage = SEQ_STORG._nHomeStep;
                int nStepLoad    = SEQ_TRANS._nHomeStep;

                m_sTemp = string.Format($"All Home TimeOut : Spindle = {nStepSpndle:D3}, Polish = {nStepPolish:D3}, " +
                    $"Clean = {nStepClean:D3}, Storage = {nStepStorage:D3}, Load = {nStepLoad:D3}");
        
        		fn_WriteLog(m_sTemp);

                EPU.fn_SetErr(EN_ERR_LIST.ERR_0001, m_bHoming);

                return; 
        	}
        
        	//Inspect Safety
        	if ( SEQ.fn_InspectEmergency    ()) { bErr = true; }
        	if ( SEQ.fn_InspectMainAir      ()) { bErr = true; }
        	if (!SEQ.fn_InspectSafety       ()) { bErr = true; }
        	if (!SEQ.fn_InspectMotor        ()) { bErr = true; } //Check Motor Min/Max Value
            if ( SEQ.fn_InspectCBoxEmergency()) { bErr = true; }

            //JUNG/200527/Tool Exist Check
            if(SEQ_SPIND.fn_IsExistTool() && !FM.fn_IsLvlMaster())
            {
                fn_UserMsg("Please remove tool.");
                bErr = true;
            }


            if (bErr)
        	{
                EPU._bDisplayErrForm = false; 
        		m_nManNo  = (int)EN_MAN_LIST.MAN_NON;  
        		m_bHoming = false; 
        		EPU.fn_SetErr(EN_ERR_LIST.ERR_0001, m_bHoming);
        		return;
        	}

			bool b1 = SEQ_SPIND.fn_MoveHome();
			bool b2 = SEQ_POLIS.fn_MoveHome();
			bool b3 = SEQ_CLEAN.fn_MoveHome();
			bool b4 = SEQ_STORG.fn_MoveHome();
			bool b5 = SEQ_TRANS.fn_MoveHome();

			m_bHoming = !b1 || !b2 || !b3 || !b4 || !b5;

        	if (!m_bHoming) m_nManNo = (int)EN_MAN_LIST.MAN_NON; 
        
        	EPU.fn_SetErr(EN_ERR_LIST.ERR_0001, m_bHoming);
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Selected Part Motor Home
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/11/21  14:45
        */
        public void fn_MovePartHome(EN_PART_ID Where)
        {
            bool bErr   = false;
            int  nErrNo = (int)EN_ERR_LIST.ERR_0002 + (int)Where;

            //Time Out.
            m_HomeTimer.OnDelay(m_bHoming, 120 * 1000); //120sec
        	if (EPU.fn_SetErr((int)EN_ERR_LIST.ERR_0111 + (int)Where, m_HomeTimer.Out))
        	{
        		m_nManNo  = (int)EN_ERR_LIST.ERR_NONE; 
        		m_bHoming = false;

                int nStep = -1;
                switch (Where)
                {
                    case EN_PART_ID.piSPDL: nStep = SEQ_SPIND._nHomeStep; break;
                    case EN_PART_ID.piPOLI: nStep = SEQ_POLIS._nHomeStep; break;
                    case EN_PART_ID.piCLEN: nStep = SEQ_CLEAN._nHomeStep; break;
                    case EN_PART_ID.piSTRG: nStep = SEQ_STORG._nHomeStep; break;
                    case EN_PART_ID.piLOAD: nStep = SEQ_TRANS._nHomeStep; break;
                    default: break;
                }

                m_sTemp = string.Format($"[{Where.ToString()}] Part Home TimeOut : Step = {nStep:D3}");
        		fn_WriteLog(m_sTemp);

                EPU.fn_SetErr(nErrNo, m_bHoming);

                return; 
        	}
        
        	//Inspect Safety
        	if ( SEQ.fn_InspectEmergency    ()) { bErr = true; }
        	if ( SEQ.fn_InspectMainAir      ()) { bErr = true; }
        	if (!SEQ.fn_InspectSafety       ()) { bErr = true; }
        	if (!SEQ.fn_InspectMotor        ()) { bErr = true; } //Check Motor Min/Max Value
            if ( SEQ.fn_InspectCBoxEmergency()) { bErr = true; }
            
            if (bErr)
        	{
                EPU._bDisplayErrForm = false; 
        		m_nManNo  = (int)EN_MAN_LIST.MAN_NON;  
        		m_bHoming = false; 
        		EPU.fn_SetErr(nErrNo, m_bHoming);
        		return;
        	}

            switch (Where)
            {
                case EN_PART_ID.piSPDL: m_bHoming = !SEQ_SPIND.fn_MoveHome(); break;
                case EN_PART_ID.piPOLI: m_bHoming = !SEQ_POLIS.fn_MoveHome(); break;
                case EN_PART_ID.piCLEN: m_bHoming = !SEQ_CLEAN.fn_MoveHome(); break;
                case EN_PART_ID.piSTRG: m_bHoming = !SEQ_STORG.fn_MoveHome(); break;
                case EN_PART_ID.piLOAD: m_bHoming = !SEQ_TRANS.fn_MoveHome(); break;
                default: break;
            }

        	//
            if (!m_bHoming) m_nManNo = (int)EN_MAN_LIST.MAN_NON; 
        
        	//
            EPU.fn_SetErr(nErrNo, m_bHoming);
      
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_SetHomeMotr(EN_MOTR_ID motr)
        {
            m_nSelHomeAxis = (int)motr;
            m_nHomeStep = 10;
            return true;
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Selected Motor Home
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/11/21  14:47
        */
        public bool fn_MoveSelHome(EN_MOTR_ID motr)
        {
            //Axis Home
            switch (m_nHomeStep)
            {
                case 10:
                    //Clear Home End
                    MOTR.ClearHomeEnd(motr);
                    
                    if (MOTR.fn_IsSMCMotor(motr))
                    {
                        IO.fn_SMCHome(motr, true);
                        m_ManDelayTimer.Clear();
                        m_nHomeStep = 20;
                        return false;
                    }
                    else
                    {
                        //Write Axis No to ACS Var
                        m_nBuffNo = fn_GetHomeBuffNo(motr);

                        //JUNG/200717/Check Home buffer 
                        if(IO.fn_IsBuffRun(m_nBuffNo)) IO.fn_RunBuffer(m_nBuffNo, false);

                        IO.fn_RunBuffer(m_nBuffNo, true);
                        m_ManDelayTimer.Clear();
                    }

                    m_nHomeStep++;
                    return false;

                case 11:
                    if (!m_ManDelayTimer.OnDelay(true, 300)) return false;

                    //Wait buffer Start
                    if (!IO.fn_IsBuffRun(m_nBuffNo)) return false;
                    m_nHomeStep++;
                    return false;

                case 12:
                    //Check Home Error
                    if(IO.DATA_ACS_TO_EQ[(int)motr] == 1)
                    {
                        fn_WriteLog($"Home FAIL!!! : {motr.ToString()}");
                        m_nHomeStep = 0;
                        return true;
                    }

                    //Wait buffer End
                    if (IO.fn_IsBuffRun(m_nBuffNo)) return false;

                    MOTR[(int)motr].SetHomeEndDone(true);

                    m_nHomeStep++;
                    return false;

                case 13:

                    if (!MOTR.MoveAsComd(motr, EN_COMD_ID.Wait1, EN_MOTR_VEL.Normal)) return false; 

                    //
                    fn_WriteLog($"Home End : {motr.ToString()}");
                    m_nHomeStep = 0;
                    return true;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //SMC Motor
                case 20:
                    
                    //Check SMC Step


                    m_nHomeStep++;
                    return false;

                case 21:
                    //
                    if (!MOTR[(int)motr].GetHomeEnd()) return false;

                    m_nHomeStep=13;
                    return false;



                default:
                    m_nHomeStep = 0;
                    return true;
            }
        }
        //---------------------------------------------------------------------------
        public int fn_GetHomeBuffNo(EN_MOTR_ID motr)
        {
            switch (motr)
            {
                case EN_MOTR_ID.miSPD_X : return BFNo_00_HOME_SPD_X ;
                case EN_MOTR_ID.miPOL_Y : return BFNo_01_HOME_POL_Y ;
                case EN_MOTR_ID.miSPD_Z : return BFNo_02_HOME_SPD_Z ;
                case EN_MOTR_ID.miCLN_R : return BFNo_03_HOME_CLN_R ;
                case EN_MOTR_ID.miPOL_TH: return BFNo_04_HOME_POL_TH;
                case EN_MOTR_ID.miPOL_TI: return BFNo_05_HOME_POL_TI;
                case EN_MOTR_ID.miSTR_Y : return BFNo_06_HOME_STR_Y ;
                case EN_MOTR_ID.miCLN_Y : return BFNo_07_HOME_CLN_Y ;

                case EN_MOTR_ID.miSPD_Z1: return 6; 
                case EN_MOTR_ID.miTRF_T : return 16;
                case EN_MOTR_ID.miTRF_Z : return 26;
            }

            return -1; 

        }
        //---------------------------------------------------------------------------
        public void fn_SetMotrRpt(int motor, int delay, double p1, double p2, double v1, double a1, double v2 = 0.0, double a2 = 0.0)
        {

            m_iRMotrID = motor;

            m_dP1 = p1;
            m_dP2 = p2;
            m_dV1 = v1;
            m_dA1 = a1;
            
            if (v2 == 0) m_dV2 = m_dV1;
            if (a2 == 0) m_dA2 = m_dA1;


            m_bRptMotr = true; 

        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Motor Repeat 
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/11/22  14:34
        */
        private bool fn_RepeatMotor()
        {
            if ( MOTR[m_iRMotrID].GetAlarm()) m_bRptMotr = false;
            if ( MOTR[m_iRMotrID].GetCW   ()) m_bRptMotr = false;
            if ( MOTR[m_iRMotrID].GetCCW  ()) m_bRptMotr = false;
            if (!MOTR[m_iRMotrID].GetHome ()) m_bRptMotr = false;
            if (!MOTR[m_iRMotrID].GetServo()) m_bRptMotr = false;
        
            //Changing Timer.
            m_MotrOnTimer .OnDelay(MOTR.MotnDone((EN_MOTR_ID)m_iRMotrID) && !m_bDirMotr , m_iChngMotrDlay);
            m_MotrOffTimer.OnDelay(MOTR.MotnDone((EN_MOTR_ID)m_iRMotrID) &&  m_bDirMotr , m_iChngMotrDlay);
            if (m_MotrOnTimer .Out) { MOTR.MoveMotr((EN_MOTR_ID)m_iRMotrID , m_dP1 , m_dV1 , m_dA1); m_bDirMotr = true ; }
            if (m_MotrOffTimer.Out) { MOTR.MoveMotr((EN_MOTR_ID)m_iRMotrID , m_dP2 , m_dV2 , m_dA2); m_bDirMotr = false; }
        	    
        	
        	return true;
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Actuator Repeat
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/11/22  14:35
        */
        private bool fn_RepeatActr()
        {
            bool isOk = true;

            for (int n=0; n<ACTR._iNumOfACT; n++)
            {
                if (ACTR.Err(n) != 0) m_bRptActr[n] = false;
                
                //Changing Timer.
                if (m_bRptActr[n])
                {
                    ACTR.MoveCyl(n, m_iRptCmd);
                    if (!ACTR.Complete(n, m_iRptCmd)) isOk = false;
                }
                m_ActrTimer.OnDelay(isOk, m_nChngActrDlay);
                if (m_ActrTimer.Out)
                {
                    m_ActrTimer.Clear();
                    if (m_iRptCmd == (int)EN_ACTR_CMD.Fwd) m_iRptCmd = (int)EN_ACTR_CMD.Bwd;
                    else                                   m_iRptCmd = (int)EN_ACTR_CMD.Fwd;
                }

            }

            return true; 
        }
        //---------------------------------------------------------------------------

        public void fn_SetRptAct(int aNum, bool Flag, int nDelay = 0)
        {
            m_nChngActrDlay = nDelay;
            if (!Flag) m_bRptActrIng = false;
            
            //
            if (aNum == -1)
            {
                for (int n = 0; n < ACTR._iNumOfACT; n++) m_bRptActr[n] = Flag;
            }
            else
            {
                if (aNum < 0 || aNum >= ACTR._iNumOfACT) return;
                m_bRptActr[aNum] = Flag;
                m_iRptCmd = 1;
                m_bRptActrIng = true;
            }
        }

        //---------------------------------------------------------------------------
        /**    
        @brief     Manual Cycle Run
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/11/21  14:48
        */
        public bool fn_ManRunCycle()
        {
	        //Check Run
	        if (SEQ._bRun || SEQ._bAuto) return false;

            //Check Repeating.
            m_bRptActrIng = false;
            for (int n = 0; n < ACTR._iNumOfACT; n++)
            {
                if (m_bRptActr[n]) { m_bRptActrIng = true; break; }
            }


            //Master Level에서만 Repeat 동작
            if (FM._nCrntLevel == (int)EN_USER_LEVEL.lvMaster)
	        {
                //강제 Manual 작업시 다른 Manual은 무시.
                if (m_bRptMotr || m_bRptActrIng)
                {
                    m_nManNo = (int)EN_MAN_LIST.MAN_NON;
                }

                //Motor
                if (m_bRptMotr)
	        	{
	        		fn_RepeatMotor();
	        	}

                //Actuator
                if (m_bRptActrIng)
	        	{
	        		fn_RepeatActr();
	        	}
	        }
            else
            {
                for (int n = 0; n < ACTR._iNumOfACT; n++)
                {
                    m_bRptActr[n] = false; 
                }

                m_bRptMotr = false;

            }

            //JUNG/200812/Drain Cycle
            bool isDrainCycle = m_nManNo == (int)EN_MAN_LIST.MAN_0421 || m_nManNo == (int)EN_MAN_LIST.MAN_0431;
            //Error 상태 Check
            if (EPU._bIsErr && !isDrainCycle)
	        {
	        	fn_Reset();
	        	return false; 
	        }

            //Check Time Out
            bool bSkipManNum = m_nManNo == (int)EN_MAN_LIST.MAN_0418 || m_nManNo == (int)EN_MAN_LIST.MAN_0419 ||
                               m_nManNo == (int)EN_MAN_LIST.MAN_0425;

            bool bChkTO = !m_bHoming && m_nManNo != (int)EN_MAN_LIST.MAN_NON && !bSkipManNum;
            if (m_ManCycleTimer.OnDelay( bChkTO, 90 * 1000))
	        {
	        	m_sTemp = string.Format($"[MAN:{m_nManNo:D4}] Cycle Time Out");
	        	fn_WriteLog(m_sTemp);

	        	fn_UserMsg(m_sTemp, EN_MSG_TYPE.Warning );

                if(m_nManNo == (int)EN_MAN_LIST.MAN_0007)
                {
                    //

                }

	        	m_nManNo = (int)EN_MAN_LIST.MAN_NON; 
	        	m_nManStep = 0; 

	        	return false;
	        }

            //JUNG/200601/Check Warming up
            bool bConWarm = FM.m_stSystemOpt.nUseWarming == 1 && m_bStartWarm && !m_bDrngWarm && !m_bWarmErr && MOTR.IsAllHomeEnd() && !SEQ.fn_IsAnyDoorOpen(true);
            m_tCheckWarmCycle.OnDelay(bConWarm, 1000 * 60 * FM.m_stSystemOpt.nWarmInterval); 
            if (m_tCheckWarmCycle.Out)
            {
                m_tCheckWarmCycle.Clear();
                fn_SetWarmFun();
            }

            fn_WarmFun();

            //
            if (m_nManNo == (int)EN_MAN_LIST.MAN_NON)
	        {
	        	m_bHoming = false; 
	        	return true; 
	        }

	        //Check Flag
	        if (!m_bLtCycleRun) return true;


	        //Manual
	        switch ((EN_MAN_LIST)m_nManNo)
	        {
		        case EN_MAN_LIST.MAN_0001:
		        	fn_MoveAllHome();
		        	m_nManStep = 0; 
		        	break;
	
		        case EN_MAN_LIST.MAN_0002: //Spindle Part Home
                    fn_MovePartHome(EN_PART_ID.piSPDL);
                    m_nManStep = 0;
                    break;
		        case EN_MAN_LIST.MAN_0003: //Polishing Part Home
                    fn_MovePartHome(EN_PART_ID.piPOLI);
                    m_nManStep = 0;
                    break;
		        case EN_MAN_LIST.MAN_0004: //Cleaning Part Home
                    fn_MovePartHome(EN_PART_ID.piCLEN);
                    m_nManStep = 0;
                    break;
		        case EN_MAN_LIST.MAN_0005: //Storage Part Home
                    fn_MovePartHome(EN_PART_ID.piSTRG);
                    m_nManStep = 0;
                    break;
		        case EN_MAN_LIST.MAN_0006: //Transfer Part Home
                    fn_MovePartHome(EN_PART_ID.piLOAD);
                    m_nManStep = 0;
                    break;

		        case EN_MAN_LIST.MAN_0007: //Axis Home
                    if(fn_MoveSelHome((EN_MOTR_ID)m_nSelHomeAxis))
                    {
                        m_nManStep = 0; 
                        m_nManNo = 0;
                    }
                    
                    break;

		        case EN_MAN_LIST.MAN_0008:
		        	break;
		        case EN_MAN_LIST.MAN_0009:
		        	break;
		        case EN_MAN_LIST.MAN_0010:
		        	break;
		        case EN_MAN_LIST.MAN_0011:
		        	break;
		        case EN_MAN_LIST.MAN_0012:
		        	break;
		        case EN_MAN_LIST.MAN_0013:
		        	break;
		        case EN_MAN_LIST.MAN_0014:
		        	break;
		        case EN_MAN_LIST.MAN_0015:
		        	break;
		        case EN_MAN_LIST.MAN_0016:
		        	break;
		        case EN_MAN_LIST.MAN_0017:
		        	break;


                //SPINDLE ONE CYCLE
                case EN_MAN_LIST.MAN_0400: if (SEQ_SPIND.fn_ToolPickOneCycle  (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0401: if (SEQ_SPIND.fn_ToolPlaceOneCycle (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0402: if (SEQ_SPIND.fn_PlatePlaceOneCycle(EN_MAGA_ID.LOAD  )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0403: if (SEQ_SPIND.fn_PlatePlaceOneCycle(EN_MAGA_ID.POLISH)) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0404: if (SEQ_SPIND.fn_PlatePlaceOneCycle(EN_MAGA_ID.CLEAN )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0405: if (SEQ_SPIND.fn_PlatePickOneCycle (EN_MAGA_ID.LOAD  )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0406: if (SEQ_SPIND.fn_PlatePickOneCycle (EN_MAGA_ID.POLISH)) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0407: if (SEQ_SPIND.fn_PlatePickOneCycle (EN_MAGA_ID.CLEAN )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0408: if (SEQ_SPIND.fn_ToolCheckOneCycle (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                
              //case EN_MAN_LIST.MAN_0409: if (SEQ_SPIND.fn_ForceCheckOneCycle(                 )) { m_nManStep = 0; m_nManNo = 0; } break;
              //case EN_MAN_LIST.MAN_0410: if (SEQ_SPIND.fn_ForceCheckOneCycle(true             )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0409: if (SEQ_SPIND.fn_ForceCheckOneCycle(m_nWhere, m_dDCOM)) { m_nManStep = 0; m_nManNo = 0; } break; //JUNG/201103
                
                case EN_MAN_LIST.MAN_0411: if (SEQ_SPIND.fn_UtilCheckOneCycle (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0412: if (SEQ_SPIND.fn_VisnTestOneCycle  (true             )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0413: if (SEQ_SPIND.fn_VisnTestOneCycle  (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0414: if (SEQ_SPIND.fn_CupPPOneCycle     (true   , true    )) { m_nManStep = 0; m_nManNo = 0; } break; //Storage   - pick
                case EN_MAN_LIST.MAN_0415: if (SEQ_SPIND.fn_CupPPOneCycle     (true   , false   )) { m_nManStep = 0; m_nManNo = 0; } break; //Polishing - Pick
                case EN_MAN_LIST.MAN_0416: if (SEQ_SPIND.fn_CupPPOneCycle     (false  , true    )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0417: if (SEQ_SPIND.fn_CupPPOneCycle     (false  , false   )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0418: if (SEQ_SPIND.fn_ForceTestCycle    (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0419: if (SEQ_SPIND.fn_AutoCalCycle      (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0425: if (SEQ_SPIND.fn_AutoTeachingCycle (                 )) { m_nManStep = 0; m_nManNo = 0; } break;



                //Polishing
                case EN_MAN_LIST.MAN_0420: if (SEQ_POLIS.fn_UtilityOneCycle   (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
              //case EN_MAN_LIST.MAN_0421: if (SEQ_POLIS.fn_DoDrainOneCycle   (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0421:     SEQ_POLIS.fn_SetDrain          (                 );   m_nManStep = 0; m_nManNo = 0;   break;


                //Cleaning
                case EN_MAN_LIST.MAN_0430: if (SEQ_CLEAN.fn_CleaningOneCycle  (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
              //case EN_MAN_LIST.MAN_0431: if (SEQ_CLEAN.fn_DoDrainOneCycle   (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0431:     SEQ_CLEAN.fn_SetDrain          (                 );   m_nManStep = 0; m_nManNo = 0;   break;


                //Storage
                case EN_MAN_LIST.MAN_0440: if (SEQ_STORG.fn_StepOneCycle      (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0441: if (SEQ_STORG.fn_AlignOneCycle     (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                
                case EN_MAN_LIST.MAN_0444: if (SEQ_STORG.fn_StorageOutOneCycle(                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0445: if (SEQ_STORG.fn_StoragInOneCycle  (                 )) { m_nManStep = 0; m_nManNo = 0; } break;


                //Transfer
                case EN_MAN_LIST.MAN_0450: if (SEQ_TRANS.fn_LoadOneCycle      (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0451: if (SEQ_TRANS.fn_UnloadOneCycle    (                 )) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0452: if (SEQ_TRANS.fn_PickOneCycle      (m_nSelMaga, m_nSelMagaSlot)) { m_nManStep = 0; m_nManNo = 0; } break;
                case EN_MAN_LIST.MAN_0453: if (SEQ_TRANS.fn_PlaceOneCycle     (m_nSelMaga, m_nSelMagaSlot)) { m_nManStep = 0; m_nManNo = 0; } break;

                


                default:
		        	m_nManNo = (int)EN_MAN_LIST.MAN_NON;
		        	break;
	        }



	        //Reset Flag.                                             
	        if (m_nManNo == (int)EN_MAN_LIST.MAN_NON) m_bLtCycleRun = false;

			return true; 
        }
        //---------------------------------------------------------------------------
        void fn_ManMoveMotr(EN_SEQ_ID iPart, EN_MOTR_ID iMotr, EN_COMD_ID iCmd)
        {

            switch (iPart)
            {   
                case EN_SEQ_ID.SPINDLE : SEQ_SPIND.fn_ReqMoveMotr(iMotr, iCmd); break;
                case EN_SEQ_ID.POLISH  : SEQ_POLIS.fn_ReqMoveMotr(iMotr, iCmd); break;
                case EN_SEQ_ID.CLEAN   : SEQ_CLEAN.fn_ReqMoveMotr(iMotr, iCmd); break;
                case EN_SEQ_ID.STORAGE : SEQ_STORG.fn_ReqMoveMotr(iMotr, iCmd); break;
                case EN_SEQ_ID.TRANSFER: SEQ_TRANS.fn_ReqMoveMotr(iMotr, iCmd); break;
                default: break;
            }
        }
        //---------------------------------------------------------------------------
        void fn_ManMoveDirect(EN_SEQ_ID iPart, EN_MOTR_ID iMotr, double dPos)
        {
            switch (iPart)
            {   
                case EN_SEQ_ID.SPINDLE : SEQ_SPIND.fn_ReqMoveDirect(iMotr, dPos); break;
                case EN_SEQ_ID.POLISH  : SEQ_POLIS.fn_ReqMoveDirect(iMotr, dPos); break;
                case EN_SEQ_ID.CLEAN   : SEQ_CLEAN.fn_ReqMoveDirect(iMotr, dPos); break;
                case EN_SEQ_ID.STORAGE : SEQ_STORG.fn_ReqMoveDirect(iMotr, dPos); break;
                case EN_SEQ_ID.TRANSFER: SEQ_TRANS.fn_ReqMoveDirect(iMotr, dPos); break;
                default: break;
            }
        }
        
        //---------------------------------------------------------------------------
        //MoveMotr
        public void fn_ManMoveMotr(EN_MOTR_ID iMotr, EN_COMD_ID Cmd)
        {
            int iPart = 0;
            int iItem = 0;
            if (!MOTR.GetMotorPart(ref iPart, ref iItem, (int)iMotr)) return;

            fn_ManMoveMotr((EN_SEQ_ID)iPart, iMotr, Cmd);
        }
        //---------------------------------------------------------------------------
        public void fn_ManMoveDirect(EN_MOTR_ID iMotr, double dPosn)
        {
            int iPart = 0;
            int iItem = 0;
            if (!MOTR.GetMotorPart(ref iPart, ref iItem, (int)iMotr)) return;

            fn_ManMoveDirect((EN_SEQ_ID)iPart, iMotr, dPosn);
        }
        //---------------------------------------------------------------------------
        public void fn_SetAllHomeStep()
        {
            m_HomeTimer.Clear();

            fn_ClearAllHomeTime();

            EPU.fn_Clear();

            //JUNG/200716
            IO.fn_StopAllHomeBuffer();
            
            SEQ_SPIND._nHomeStep = 10;
            SEQ_POLIS._nHomeStep = 10;
            SEQ_CLEAN._nHomeStep = 10;
            SEQ_STORG._nHomeStep = 10;
            SEQ_TRANS._nHomeStep = 10;

            SEQ.fn_UpdateSeqState();

        }
        //---------------------------------------------------------------------------
        public void fn_SetPartHomeStep(EN_PART_ID part)
        {
            //
            m_HomeTimer.Clear();
            EPU.fn_Clear();

            switch (part)
            {
                case EN_PART_ID.piSPDL:
                    SEQ_SPIND.fn_ClearHomeTime();
                    SEQ_SPIND._nHomeStep = 10;
                    break;
                case EN_PART_ID.piPOLI:
                    SEQ_POLIS.fn_ClearHomeTime();
                    SEQ_POLIS._nHomeStep = 10;
                    break;
                case EN_PART_ID.piCLEN:
                    SEQ_CLEAN.fn_ClearHomeTime();
                    SEQ_CLEAN._nHomeStep = 10;
                    break;
                case EN_PART_ID.piSTRG:
                    SEQ_STORG.fn_ClearHomeTime();
                    SEQ_STORG._nHomeStep = 10;
                    break;
                case EN_PART_ID.piLOAD:
                    SEQ_TRANS.fn_ClearHomeTime();
                    SEQ_TRANS._nHomeStep = 10;
                    break;
                default:
                    break;
            }

            SEQ.fn_UpdateSeqState();
        }

        //---------------------------------------------------------------------------
        /**    
		@brief     Manual Process On
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/25  10:55
		*/
        public void fn_ManProcOn(int No, bool OnKey, bool OffKey)
		{
            //Local Var.
            double dPosn = 0.0;

            //Check Min/Max
            if (No < 0             ) return; 
			if (No > MAX_MANUAL    ) return;

            //JOG STOP
            if (!OnKey)
            {
                for (int i = 0; i < MOTR._iNumOfMotr; i++)
                {
                    if (No == MOTR.ManNoJog((EN_MOTR_ID)i)) MOTR.Stop((EN_MOTR_ID)i);
                }
            }


            for(int i=0; i< MOTR._iNumOfMotr; i++)
            {
                if (!OnKey || OffKey) continue;
                if (No == MOTR.ManNoAlarm ((EN_MOTR_ID)i)) MOTR.SetAlarm((EN_MOTR_ID)i, true);
            }

            //Run Check
            if (SEQ._bRun || SEQ._bAuto)
			{
                fn_UserMsg("Machine is Run or Auto Mode!!");
				return;
			}

            if ( m_nManNo != (int)EN_MAN_LIST.MAN_NON) return;

            bool bSkipDoor = (No == (int)EN_MAN_LIST.MAN_0421) || (No == (int)EN_MAN_LIST.MAN_0431); //JUNG/200513/Door Skip 
            if ( SEQ.fn_IsAnyDoorOpen() && !bSkipDoor)
			{
                fn_UserMsg("Door Opened !!");
				return; 
			}
            

            //Clear
            for (int i = 0; i < MOTR._iNumOfMotr; i++)
            {
				// HomeEnd Flag.
				//if (No == MOTR.ManNoHome((EN_MOTR_ID)i)) MOTR.ClearHomeEnd((EN_MOTR_ID)i); //확인 필요...
				
				//Jog
				if (!OnKey && No == MOTR.ManNoJog ((EN_MOTR_ID)i)) MOTR.Stop((EN_MOTR_ID)i);
				if (          No == MOTR.ManNoStop((EN_MOTR_ID)i)) MOTR.Stop((EN_MOTR_ID)i);
			}

            //
            for(int i=0; i< MOTR._iNumOfMotr; i++)
            {
                if (!OnKey || OffKey) continue;
                     if (No == MOTR.ManNoStop  ((EN_MOTR_ID)i    )) fn_ManMoveMotr    ((EN_MOTR_ID)i, EN_COMD_ID.Stop);
                else if (No == MOTR.ManNoJog   ((EN_MOTR_ID)i    )) fn_ManMoveMotr    ((EN_MOTR_ID)i, EN_COMD_ID.JogP);
                else if (No == MOTR.ManNoServo ((EN_MOTR_ID)i    )) MOTR.SetServo     ((EN_MOTR_ID)i, true           );
                else if (No == MOTR.ManNoAlarm ((EN_MOTR_ID)i    )) MOTR.SetAlarm     ((EN_MOTR_ID)i, true           );
                else if (No == MOTR.ManNoHome  ((EN_MOTR_ID)i    ))
                {
                    //
                    fn_SetHomeMotr((EN_MOTR_ID)i);
                    No = (int)EN_MAN_LIST.MAN_0007;
                    //fn_MoveSelHome    ((EN_MOTR_ID)i                 );
                }
                else if (MOTR.GetPosnByManNo   (i, No, out dPosn )) fn_ManMoveDirect  ((EN_MOTR_ID)i, dPosn          );
            }

            //Log
            if (m_nPrevManNo != m_nManNo) fn_WriteLog(string.Format($"MANUAL ON [MAN_{No:D4}]"));
            m_nPrevManNo = m_nManNo;

            //Set Manual No & Flag
            m_nManNo = No; 
			m_bLtCycleRun = true;

            if (!OnKey || OffKey) No = 0; 

            //Selection 
            switch ((EN_MAN_LIST)No)
			{
			    case EN_MAN_LIST.MAN_0001: 
                    m_bHoming = true;
                    fn_SetAllHomeStep();                                return; 
                case EN_MAN_LIST.MAN_0002:
                    m_bHoming = true;
                    fn_SetPartHomeStep(EN_PART_ID.piSPDL);              return;
				case EN_MAN_LIST.MAN_0003:
                    m_bHoming = true;
                    fn_SetPartHomeStep(EN_PART_ID.piPOLI);              return;
                case EN_MAN_LIST.MAN_0004:
                    m_bHoming = true;
                    fn_SetPartHomeStep(EN_PART_ID.piCLEN);              return;
                case EN_MAN_LIST.MAN_0005:
                    m_bHoming = true;
                    fn_SetPartHomeStep(EN_PART_ID.piSTRG);              return;
                case EN_MAN_LIST.MAN_0006:
                    m_bHoming = true;
                    fn_SetPartHomeStep(EN_PART_ID.piLOAD);              return;
				
                case EN_MAN_LIST.MAN_0007:                              return; //Home

				case EN_MAN_LIST.MAN_0008:                              break;
				case EN_MAN_LIST.MAN_0009:                              break;
				case EN_MAN_LIST.MAN_0010:                              break;
				case EN_MAN_LIST.MAN_0011:                              break;
				case EN_MAN_LIST.MAN_0012:                              break;
				case EN_MAN_LIST.MAN_0013:                              break;
				case EN_MAN_LIST.MAN_0014:                              break;
				case EN_MAN_LIST.MAN_0015:                              break;
				case EN_MAN_LIST.MAN_0016:                              break;
				case EN_MAN_LIST.MAN_0017:                              break;
				case EN_MAN_LIST.MAN_0018:                              break;
				case EN_MAN_LIST.MAN_0019:                              break;
				case EN_MAN_LIST.MAN_0020: m_bJog = true;               break;
				case EN_MAN_LIST.MAN_0021:                              break;
				case EN_MAN_LIST.MAN_0022:                              break;
				case EN_MAN_LIST.MAN_0023:                              break;
				case EN_MAN_LIST.MAN_0024:                              break;

                //MAN_0025 ~ 0349
                case EN_MAN_LIST.MAN_0281:                              return;

                case EN_MAN_LIST.MAN_0350:                              break;
                case EN_MAN_LIST.MAN_0351:                              break;
                case EN_MAN_LIST.MAN_0352:                              break;
                case EN_MAN_LIST.MAN_0353:                              break;
                case EN_MAN_LIST.MAN_0354:                              break;
                case EN_MAN_LIST.MAN_0355:                              break;
                case EN_MAN_LIST.MAN_0356:                              break;
                case EN_MAN_LIST.MAN_0357:                              break;
                case EN_MAN_LIST.MAN_0358:                              break;
                case EN_MAN_LIST.MAN_0359:                              break;
                case EN_MAN_LIST.MAN_0360:                              break;
                case EN_MAN_LIST.MAN_0361:                              break;
                case EN_MAN_LIST.MAN_0362:                              break;
                case EN_MAN_LIST.MAN_0363:                              break;
                case EN_MAN_LIST.MAN_0364:                              break;
                case EN_MAN_LIST.MAN_0365:                              break;
                case EN_MAN_LIST.MAN_0366:                              break;
                case EN_MAN_LIST.MAN_0367:                              break;
                case EN_MAN_LIST.MAN_0368:                              break;
                case EN_MAN_LIST.MAN_0369:                              break;
                case EN_MAN_LIST.MAN_0370:                              break;
                case EN_MAN_LIST.MAN_0371:                              break;
                case EN_MAN_LIST.MAN_0372:                              break;
                case EN_MAN_LIST.MAN_0373:                              break;
                case EN_MAN_LIST.MAN_0374:                              break;
                case EN_MAN_LIST.MAN_0375:                              break;
                case EN_MAN_LIST.MAN_0376:                              break;
                case EN_MAN_LIST.MAN_0377:                              break;
                case EN_MAN_LIST.MAN_0378:                              break;
                case EN_MAN_LIST.MAN_0379:                              break;
                case EN_MAN_LIST.MAN_0380:                              break;
                case EN_MAN_LIST.MAN_0381:                              break;
                case EN_MAN_LIST.MAN_0382:                              break;
                case EN_MAN_LIST.MAN_0383:                              break;
                case EN_MAN_LIST.MAN_0384:                              break;
                case EN_MAN_LIST.MAN_0385:                              break;
                case EN_MAN_LIST.MAN_0386:                              break;
                case EN_MAN_LIST.MAN_0387:                              break;
                case EN_MAN_LIST.MAN_0388:                              break;
                case EN_MAN_LIST.MAN_0389:                              break;
                case EN_MAN_LIST.MAN_0390:                              break;
                case EN_MAN_LIST.MAN_0391:                              break;
                case EN_MAN_LIST.MAN_0392:                              break;
                case EN_MAN_LIST.MAN_0393:                              break;
                case EN_MAN_LIST.MAN_0394:                              break;
                case EN_MAN_LIST.MAN_0395:                              break;
                case EN_MAN_LIST.MAN_0396:                              break;
                case EN_MAN_LIST.MAN_0397:                              break;

                //Spindle 
                case EN_MAN_LIST.MAN_0398:   SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X, EN_COMD_ID.OneStepF1); return; //SPINDLE X-Axis STEP MOVE +
                case EN_MAN_LIST.MAN_0399:   SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X, EN_COMD_ID.OneStepB1); return; //SPINDLE X-Axis STEP MOVE -
                case EN_MAN_LIST.MAN_0400:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Tool Pick One Cycle
                case EN_MAN_LIST.MAN_0401:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Tool Discard One Cycle
                case EN_MAN_LIST.MAN_0402:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Plate Place One Cycle : Unload
                case EN_MAN_LIST.MAN_0403:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Plate Place One Cycle : Polishing Bath
                case EN_MAN_LIST.MAN_0404:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Plate Place One Cycle : Cleaning Bath
                case EN_MAN_LIST.MAN_0405:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Plate Pick One Cycle : Load
                case EN_MAN_LIST.MAN_0406:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Plate Pick One Cycle : Polishing Bath
                case EN_MAN_LIST.MAN_0407:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Plate Pick One Cycle : Cleaning Bath
                case EN_MAN_LIST.MAN_0408:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Tool Exist Check One Cycle
                case EN_MAN_LIST.MAN_0409:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Force Check of Calibration zone
                case EN_MAN_LIST.MAN_0410:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Force Check of Polishing Bath 
                case EN_MAN_LIST.MAN_0411:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Utility Check One Cycle
                case EN_MAN_LIST.MAN_0412:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Vision One Cycle 
                case EN_MAN_LIST.MAN_0413:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Vision TEST Cycle 
                case EN_MAN_LIST.MAN_0414:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Cup Pick one Cycle : Storage
                case EN_MAN_LIST.MAN_0415:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Cup Pick one Cycle : Polishing
                case EN_MAN_LIST.MAN_0416:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Cup Place one Cycle : Storage
                case EN_MAN_LIST.MAN_0417:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Cup Place one Cycle : Polishing
                case EN_MAN_LIST.MAN_0418:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Force Test Cycle
                case EN_MAN_LIST.MAN_0419:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Auto Calibration Cycle
                case EN_MAN_LIST.MAN_0425:   SEQ_SPIND._nManStep = 10;  return; //SPINDLE - Auto Tool Storage Teaching Cycle

                //
                case EN_MAN_LIST.MAN_0420:   SEQ_POLIS._nManStep = 10;  return; //POLISHING - Utility Check Cycle
                case EN_MAN_LIST.MAN_0421: /*SEQ_POLIS._nManStep = 10;*/return; //POLISHING - Drain Cycle


                //
                case EN_MAN_LIST.MAN_0430:   SEQ_CLEAN._nManStep = 10;  return; //CLEANING - Cleaning One Cycle
                case EN_MAN_LIST.MAN_0431: /*SEQ_CLEAN._nManStep = 10;*/return; //CLEANING - Drain Cycle


                //
                case EN_MAN_LIST.MAN_0440:   SEQ_STORG._nManStep = 10;  return; //STORAGE - Step One Cycle
                case EN_MAN_LIST.MAN_0441:   SEQ_STORG._nManStep = 10;  return; //STORAGE - Storage Align Cycle
                
                case EN_MAN_LIST.MAN_0442:   SEQ_STORG.fn_ReqMoveMotr(EN_MOTR_ID.miSTR_Y, EN_COMD_ID.OneStepF1); return; //STORAGE - Y Axis STEP MOVE +
                case EN_MAN_LIST.MAN_0443:   SEQ_STORG.fn_ReqMoveMotr(EN_MOTR_ID.miSTR_Y, EN_COMD_ID.OneStepB1); return; //STORAGE - Y Axis STEP MOVE -
                case EN_MAN_LIST.MAN_0444:   SEQ_STORG._nManStep = 10;  return; //STORAGE - Unlock Cycle
                case EN_MAN_LIST.MAN_0445:   SEQ_STORG._nManStep = 10;  return; //STORAGE - Lock Cycle


                //
                case EN_MAN_LIST.MAN_0450:   SEQ_TRANS._nManStep = 10;  return; //TRANSFER - Load One Cycle
                case EN_MAN_LIST.MAN_0451:   SEQ_TRANS._nManStep = 10;  return; //TRANSFER - Unload One Cycle
                case EN_MAN_LIST.MAN_0452:   SEQ_TRANS._nManStep = 10;  return; //TRANSFER - Pick One Cycle
                case EN_MAN_LIST.MAN_0453:   SEQ_TRANS._nManStep = 10;  return; //TRANSFER - Place One Cycle
                case EN_MAN_LIST.MAN_0454:   SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_Z, EN_COMD_ID.OneStepF1); return; //TARANSFER - Z Axis STEP MOVE +
                case EN_MAN_LIST.MAN_0455:   SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_Z, EN_COMD_ID.OneStepB1); return; //TARANSFER - Z Axis STEP MOVE -


                //Actuator                 
                case EN_MAN_LIST.MAN_0470: SEQ_SPIND.fn_MoveToolClamp    (ccFwd); break; //SPINDLE - Tool Camp
                case EN_MAN_LIST.MAN_0471: SEQ_SPIND.fn_MoveCylClamp     (ccFwd); break; //SPINDLE - Plate Clamp
                case EN_MAN_LIST.MAN_0472: SEQ_SPIND.fn_MoveCylIR        (ccFwd); break; //SPINDLE - IR Shutter
                case EN_MAN_LIST.MAN_0473: SEQ_SPIND.fn_MoveCylLensCvr   (ccFwd); break; //SPINDLE - Lens Cover

                case EN_MAN_LIST.MAN_0475: SEQ_POLIS.fn_MoveCylClamp     (ccFwd); break; //Polishing - Plate Clamp
                //case EN_MAN_LIST.MAN_0476: SEQ_POLIS.fn_MoveCylCup       (ccFwd); break; //Polishing - Cap Fwd/Bwd
                                                                         
                case EN_MAN_LIST.MAN_0480: SEQ_CLEAN.fn_MoveCylClamp     (ccFwd); break; //Cleaning - Plate Clamp
                                                                         
                                                                         
                case EN_MAN_LIST.MAN_0485: SEQ_STORG.fn_MoveCylLock1     (ccFwd, true); break; //Storage - Bottom Lock Clamp 
                case EN_MAN_LIST.MAN_0486: SEQ_STORG.fn_MoveCylLock2     (ccFwd, true); break; //Storage - Top Lock Clamp
                case EN_MAN_LIST.MAN_0487: SEQ_STORG.fn_MoveCylLock      (ccFwd      ); break; //Storage - Lock Clamp 
                                                                         
                case EN_MAN_LIST.MAN_0490: SEQ_TRANS.fn_MoveCylTopTR     (ccFwd); break; //Transfer - Top Load Fwd/Bwd
                case EN_MAN_LIST.MAN_0491: SEQ_TRANS.fn_MoveCylTopTurn   (ccFwd); break; //Transfer - Top Load Turn(0, 180)
                case EN_MAN_LIST.MAN_0492: SEQ_TRANS.fn_MoveCylBtmTR     (ccFwd); break; //Transfer - Bottom Load Fwd/Bwd
                case EN_MAN_LIST.MAN_0493: SEQ_TRANS.fn_MoveCylLoadUpDown(ccFwd); break; //Transfer - Load Port Up/Down
                case EN_MAN_LIST.MAN_0494: SEQ_TRANS.fn_MoveCylMaga      (ccFwd); break; //Transfer - Magazine Move Left/Right
                case EN_MAN_LIST.MAN_0495: SEQ_TRANS.fn_MoveCylLoadCover (ccFwd); break; //Transfer - Load Protect Cover 


                default:
					break;

			}

			//Reset Manual No & Flag
			m_nManNo      = (int)EN_MAN_LIST.MAN_NON;
			m_bLtCycleRun = false;
            m_dDirectPos  = 0.0;
            m_bJog        = false;

        }

		//---------------------------------------------------------------------------
		/**    
		@brief     Manual Process Off
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/11/25  10:55
		*/
		public void fn_ManProcOff(int No, bool OnKey, bool OffKey)
		{
			//Check Min/Max
			if (No < 0             ) return; 
			if (No > MAX_MANUAL    ) return;

            //JOG STOP
            if (!OnKey)
            {
                for (int i = 0; i < MOTR._iNumOfMotr; i++)
                {
                    if (No == MOTR.ManNoJog((EN_MOTR_ID)i)) MOTR.Stop((EN_MOTR_ID)i);
                }
            }

            //Run Check
            if (SEQ._bRun || SEQ._bAuto)
			{
				fn_UserMsg("Machine is Run or Auto Mode!!", EN_MSG_TYPE.Check);
				return;
			}

			if (m_nManNo != (int)EN_MAN_LIST.MAN_NON) return;
			if( SEQ.fn_IsAnyDoorOpen())
			{
				fn_UserMsg("Door Opened !!", EN_MSG_TYPE.Check);
				return; 
			}

            //Clear
            for (int i = 0; i < MOTR._iNumOfMotr; i++)
            {
				// HomeEnd Flag.
				//if (No == MOTR.ManNoHome((EN_MOTR_ID)i)) MOTR.ClearHomeEnd((EN_MOTR_ID)i); //확인 필요...
				
				//Jog
				if (!OffKey && No == MOTR.ManNoJog ((EN_MOTR_ID)i)) MOTR.Stop((EN_MOTR_ID)i);
				if (           No == MOTR.ManNoStop((EN_MOTR_ID)i)) MOTR.Stop((EN_MOTR_ID)i);
			}

            //Log
            if (m_nPrevManNo != m_nManNo) fn_WriteLog(string.Format($"MANUAL OFF [MAN_{No:D4}]"));
            m_nPrevManNo = m_nManNo;

            //Set Manual No & Flag
            m_nManNo = No; 
			m_bLtCycleRun = true;


            //
            for(int i=0; i<MOTR._iNumOfMotr;i++)
            {
                 if(!OffKey || OnKey) continue;
                      if (No == MOTR.ManNoStop  ((EN_MOTR_ID)i)) fn_ManMoveMotr ((EN_MOTR_ID)i, EN_COMD_ID.Stop);
                 else if (No == MOTR.ManNoJog   ((EN_MOTR_ID)i)) fn_ManMoveMotr ((EN_MOTR_ID)i, EN_COMD_ID.JogN);
                 else if (No == MOTR.ManNoServo ((EN_MOTR_ID)i)) MOTR.SetServo  ((EN_MOTR_ID)i, true           );
                 else if (No == MOTR.ManNoAlarm ((EN_MOTR_ID)i)) MOTR.SetAlarm  ((EN_MOTR_ID)i, true           );
            }

            if (OnKey || !OffKey) No = 0;

            //Selection 
			switch ((EN_MAN_LIST)No)
			{
				case EN_MAN_LIST.MAN_0001: fn_ClearAllHomeTime(); m_bHoming = true; return; 
                case EN_MAN_LIST.MAN_0002:                              break;  
				case EN_MAN_LIST.MAN_0003:                              break;
                case EN_MAN_LIST.MAN_0004:                              break;
                case EN_MAN_LIST.MAN_0005:                              break;
                case EN_MAN_LIST.MAN_0006:                              break;
                case EN_MAN_LIST.MAN_0007:                              break;
				case EN_MAN_LIST.MAN_0008:                              break;
				case EN_MAN_LIST.MAN_0009:                              break;
				case EN_MAN_LIST.MAN_0010:                              break;
				case EN_MAN_LIST.MAN_0011:                              break;
				case EN_MAN_LIST.MAN_0012:                              break;
				case EN_MAN_LIST.MAN_0013:                              break;
				case EN_MAN_LIST.MAN_0014:                              break;
				case EN_MAN_LIST.MAN_0015:                              break;
				case EN_MAN_LIST.MAN_0016:                              break;
				case EN_MAN_LIST.MAN_0017:                              break;
				case EN_MAN_LIST.MAN_0018:                              break;
				case EN_MAN_LIST.MAN_0019:                              break;
				case EN_MAN_LIST.MAN_0020: m_bJog = false;              break;
				case EN_MAN_LIST.MAN_0021:                              break;
				case EN_MAN_LIST.MAN_0022:                              break;
				case EN_MAN_LIST.MAN_0023:                              break;
				case EN_MAN_LIST.MAN_0024:                              break;
				
                //MAN_0025~0349
				
                case EN_MAN_LIST.MAN_0350: break;
                case EN_MAN_LIST.MAN_0351: break;
                case EN_MAN_LIST.MAN_0352: break;
                case EN_MAN_LIST.MAN_0353: break;
                case EN_MAN_LIST.MAN_0354: break;
                case EN_MAN_LIST.MAN_0355: break;
                case EN_MAN_LIST.MAN_0356: break;
                case EN_MAN_LIST.MAN_0357: break;
                case EN_MAN_LIST.MAN_0358: break;
                case EN_MAN_LIST.MAN_0359: break;
                case EN_MAN_LIST.MAN_0360: break;
                case EN_MAN_LIST.MAN_0361: break;
                case EN_MAN_LIST.MAN_0362: break;
                case EN_MAN_LIST.MAN_0363: break;
                case EN_MAN_LIST.MAN_0364: break;
                case EN_MAN_LIST.MAN_0365: break;
                case EN_MAN_LIST.MAN_0366: break;
                case EN_MAN_LIST.MAN_0367: break;
                case EN_MAN_LIST.MAN_0368: break;
                case EN_MAN_LIST.MAN_0369: break;
                case EN_MAN_LIST.MAN_0370: break;
                case EN_MAN_LIST.MAN_0371: break;
                case EN_MAN_LIST.MAN_0372: break;
                case EN_MAN_LIST.MAN_0373: break;
                case EN_MAN_LIST.MAN_0374: break;
                case EN_MAN_LIST.MAN_0375: break;
                case EN_MAN_LIST.MAN_0376: break;
                case EN_MAN_LIST.MAN_0377: break;
                case EN_MAN_LIST.MAN_0378: break;
                case EN_MAN_LIST.MAN_0379: break;
                case EN_MAN_LIST.MAN_0380: break;
                case EN_MAN_LIST.MAN_0381: break;
                case EN_MAN_LIST.MAN_0382: break;
                case EN_MAN_LIST.MAN_0383: break;
                case EN_MAN_LIST.MAN_0384: break;
                case EN_MAN_LIST.MAN_0385: break;
                case EN_MAN_LIST.MAN_0386: break;
                case EN_MAN_LIST.MAN_0387: break;
                case EN_MAN_LIST.MAN_0388: break;
                case EN_MAN_LIST.MAN_0389: break;
                case EN_MAN_LIST.MAN_0390: break;
                case EN_MAN_LIST.MAN_0391: break;
                case EN_MAN_LIST.MAN_0392: break;
                case EN_MAN_LIST.MAN_0393: break;
                case EN_MAN_LIST.MAN_0394: break;
                case EN_MAN_LIST.MAN_0395: break;
                case EN_MAN_LIST.MAN_0396: break;
                case EN_MAN_LIST.MAN_0397: break;
                case EN_MAN_LIST.MAN_0398: break;
                case EN_MAN_LIST.MAN_0399: break;
                case EN_MAN_LIST.MAN_0400: break;


                //Actuator
                case EN_MAN_LIST.MAN_0470: SEQ_SPIND.fn_MoveToolClamp    (ccBwd); break; //SPINDLE - Tool Camp
                case EN_MAN_LIST.MAN_0471: SEQ_SPIND.fn_MoveCylClamp     (ccBwd); break; //SPINDLE - Plate Clamp
                case EN_MAN_LIST.MAN_0472: SEQ_SPIND.fn_MoveCylIR        (ccBwd); break; //SPINDLE - IR Shutter
                case EN_MAN_LIST.MAN_0473: SEQ_SPIND.fn_MoveCylLensCvr   (ccBwd); break; //SPINDLE - Lens Cover
                                                                         
                case EN_MAN_LIST.MAN_0475: SEQ_POLIS.fn_MoveCylClamp     (ccBwd); break; //Polishing - Plate Clamp
                //case EN_MAN_LIST.MAN_0476: SEQ_POLIS.fn_MoveCylCup       (ccBwd); break; //Polishing - Cap Fwd/Bwd
                                                                            
                case EN_MAN_LIST.MAN_0480: SEQ_CLEAN.fn_MoveCylClamp     (ccBwd); break; //Cleaning - Plate Clamp
                                                                            
                case EN_MAN_LIST.MAN_0485: SEQ_STORG.fn_MoveCylLock1     (ccBwd, true); break; //Storage - Bottom Lock Clamp 
                case EN_MAN_LIST.MAN_0486: SEQ_STORG.fn_MoveCylLock2     (ccBwd, true); break; //Storage - Top Lock Clamp 
                case EN_MAN_LIST.MAN_0487: SEQ_STORG.fn_MoveCylLock      (ccBwd      ); break; //Storage - Lock Clamp 

                case EN_MAN_LIST.MAN_0490: SEQ_TRANS.fn_MoveCylTopTR     (ccBwd); break; //Transfer - Top Load Fwd/Bwd
                case EN_MAN_LIST.MAN_0491: SEQ_TRANS.fn_MoveCylTopTurn   (ccBwd); break; //Transfer - Top Load Turn(0, 180)
                case EN_MAN_LIST.MAN_0492: SEQ_TRANS.fn_MoveCylBtmTR     (ccBwd); break; //Transfer - Bottom Load Fwd/Bwd
                case EN_MAN_LIST.MAN_0493: SEQ_TRANS.fn_MoveCylLoadUpDown(ccBwd); break; //Transfer - Load Port Up/Down
                case EN_MAN_LIST.MAN_0494: SEQ_TRANS.fn_MoveCylMaga      (ccBwd); break; //Transfer - Magazine Move Left/Right
                case EN_MAN_LIST.MAN_0495: SEQ_TRANS.fn_MoveCylLoadCover (ccBwd); break; //Transfer - Load Protect Cover 

                default:
					break;
				
			}

			//Reset Manual No & Flag
			m_nManNo      = (int)EN_MAN_LIST.MAN_NON;
			m_bLtCycleRun = false;

		}
        //---------------------------------------------------------------------------
        public void fn_ClearAllHomeTime()
        {
            SEQ_SPIND.fn_ClearHomeTime();
            SEQ_POLIS.fn_ClearHomeTime();
            SEQ_CLEAN.fn_ClearHomeTime();
            SEQ_STORG.fn_ClearHomeTime();
            SEQ_TRANS.fn_ClearHomeTime();
        }
        //-------------------------------------------------------------------------------------------------
        public int fn_GetIndexNo(EN_MOTR_ID motor)
        {
            int nRtn   = -1;
            int nMotor = (int)EN_MOTR_ID.miSPD_Z1;

            if ((int)motor < nMotor) return 0;

            nRtn = ((int)motor - (int)nMotor) * 10;

            return nRtn;
        }
        //---------------------------------------------------------------------------
        public void fn_SetWarmFun(bool on = true)
        {
            if (FM.m_stSystemOpt.nUseWarming != 1)
            {
                m_bStartWarm = false;
                m_nWarmStep  = 0;
                m_bDrngWarm  = false;
                m_bWarmErr   = false;
                return;
            }

            if (on)
            {
                if (SEQ.fn_IsAnyDoorOpen())
                {
                    fn_UserMsg("Please Check Door.");
                    return;
                }

                m_bStartWarm = true;

                m_nWarmStep  = 10;
                m_bDrngWarm  = true;
                m_bWarmErr   = false;

                m_tCheckWarmCycle.Clear();

                fn_WriteLog("[ON] Warming Up Function");
            }
            else
            {
                m_bStartWarm = false;

                m_nWarmStep  = 0;
                m_bDrngWarm  = false;
                m_bWarmErr   = false;

                m_tCheckWarmCycle.Clear();

                SEQ.fn_Reset(); //JUNG/201210/Add

                fn_WriteLog("[OFF] Warming Up Function");
            }
        }
        //---------------------------------------------------------------------------
        public bool fn_WarmFun()
        {
            //Time out Check
            m_tWarmCycleTimeOut.OnDelay(FM.m_stSystemOpt.nUseWarming == 1 && m_nWarmStep != 0, 60 * 1000 * 10); //
            if (m_tWarmCycleTimeOut.Out)
            {
                m_bWarmErr = true;

                fn_WriteLog($"Warming Up Cycle Error / Step : {m_nWarmStep}");

                m_tWarmCycleTimeOut.Clear();
                
                m_bDrngWarm = false;
                m_nWarmStep = 0;
                return true;
            }

            //Auto, Run Check
            if (SEQ._bRun || SEQ._bAuto || FM.m_stSystemOpt.nUseWarming != 1 || EPU.fn_GetHasErr())
            {
                m_bStartWarm = false;
                m_bWarmErr   = true;
                m_bDrngWarm  = false;
                m_nWarmStep  = 0;
                return true;
            }

            bool r1, r2, r3;

            //Warming Up 
            switch (m_nWarmStep)
            {
                default:
                    m_bWarmErr  = false;
                    m_bDrngWarm = false;
                    m_nWarmStep = 0;
                    return true;

                case 10:

                    //Check Door Close
                    if (SEQ.fn_IsAnyDoorOpen())
                    {
                        fn_UserMsg("Door is Open. Warming Stop.");
                        m_bWarmErr = true;
                        m_nWarmStep = 0;
                        return true;
                    }

                    //Set Position 
                    m_cmdPos01[(int)EN_MOTR_ID.miSPD_X ] = EN_COMD_ID.User1; m_cmdPos02[(int)EN_MOTR_ID.miSPD_X ] = EN_COMD_ID.User2;
                    m_cmdPos01[(int)EN_MOTR_ID.miPOL_Y ] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miPOL_Y ] = EN_COMD_ID.User4;
                    m_cmdPos01[(int)EN_MOTR_ID.miSPD_Z ] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miSPD_Z ] = EN_COMD_ID.User4;
                    m_cmdPos01[(int)EN_MOTR_ID.miCLN_R ] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miCLN_R ] = EN_COMD_ID.CalPos;
                    m_cmdPos01[(int)EN_MOTR_ID.miPOL_TH] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miPOL_TH] = EN_COMD_ID.User1;
                    m_cmdPos01[(int)EN_MOTR_ID.miPOL_TI] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miPOL_TI] = EN_COMD_ID.User1;
                    m_cmdPos01[(int)EN_MOTR_ID.miSTR_Y ] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miSTR_Y ] = EN_COMD_ID.User3;
                    m_cmdPos01[(int)EN_MOTR_ID.miCLN_Y ] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miCLN_Y ] = EN_COMD_ID.User1;
                    m_cmdPos01[(int)EN_MOTR_ID.miSPD_Z1] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miSPD_Z1] = EN_COMD_ID.User14;
                    m_cmdPos01[(int)EN_MOTR_ID.miTRF_Z ] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miTRF_Z ] = EN_COMD_ID.FSP1_1;
                    m_cmdPos01[(int)EN_MOTR_ID.miTRF_T ] = EN_COMD_ID.Wait1; m_cmdPos02[(int)EN_MOTR_ID.miTRF_T ] = EN_COMD_ID.User1;


                    MOTR[(int)EN_MOTR_ID.miCLN_R].MP.dPosn[(int)EN_POSN_ID.CalPos] = 180;


                    //Count Clear
                    m_nWarmCnt = 0;

                    for (int n = 0; n < MAX_MOTOR; n++)
                    {
                        m_bRtn[n] = false;
                    }

                    SEQ.fn_SetLight(true);

                    m_bDrngWarm = true;

                    fn_WriteLog("Start Warming Up");

                    //Drain
                    SEQ_POLIS.fn_SetDrain();
                    SEQ_CLEAN.fn_SetDrain();

                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;

                case 11:
                    //
                    r1 =  SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
                    r2 =  SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);
                    r3 = !SEQ_POLIS._bDrngSeqDrain && !SEQ_CLEAN._bDrngSeqDrain;
                    
                    if (!r1 || !r2 || !r3) return false;

                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;

                case 12:

                    //if (SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X, m_cmdPos02[(int)EN_MOTR_ID.miSPD_X])) return false; //JUNG/200602/move Cleaning position for Z-Axis DSTB
                    r1 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X, m_cmdPos02[(int)EN_MOTR_ID.miSPD_X]);
                    r2 = SEQ_TRANS.fn_MoveCylTRWait(); //JUNG/210114
                    if (!r1 || !r2) return false;

                    fn_WriteLog("Warming Up Ready Done");

                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;
                
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //Z-Axis
                case 13:
                    if (!m_tWarmDelay.OnDelay(true, 3000)) return false;
                    
                    //Z-Axis Move Motor Pos-01
                    if (FM.m_stSystemOpt.bUseMotor[ 2]) r1 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z , m_cmdPos01[ 2]); else r1 = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 8]) r2 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z1, m_cmdPos01[ 8]); else r2 = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 9]) r3 = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_Z , m_cmdPos01[ 9]); else r3 = true;

                    //
                    if (!r1 || !r2 || !r3) return false; 

                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;
                
                case 14:
                    if (!m_tWarmDelay.OnDelay(true, 3000)) return false; 
                    
                    //Z-Axis Move Motor Pos-02
                    if (FM.m_stSystemOpt.bUseMotor[ 2]) r1 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z , m_cmdPos02[ 2]); else r1 = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 8]) r2 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z1, m_cmdPos02[ 8]); else r2 = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 9]) r3 = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_Z , m_cmdPos02[ 9]); else r3 = true;

                    //
                    if (!r1 || !r2 || !r3) return false;

                    if (++m_nWarmCnt < FM.m_stSystemOpt.nMotrRepeat)
                    {
                        m_tWarmDelay.Clear();
                        m_nWarmStep--;
                        return false;
                    }

                    fn_WriteLog(string.Format($"[Warming Up] Z-Axis Motor Done / Count : {m_nWarmCnt}"));

                    m_nWarmCnt = 0;
                    
                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;

                case 15:
                    if (!m_tWarmDelay.OnDelay(true, 1000)) return false;

                    r1 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z , EN_COMD_ID.Wait1);
                    r2 = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z1, EN_COMD_ID.Wait1);
                    r3 = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_Z , EN_COMD_ID.Wait1);
                    if (!r1 || !r2 || !r3) return false;
                    
                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;
                
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //With out Z-Axis
                case 16:
                    if (!m_tWarmDelay.OnDelay(true, 3000)) return false;

                    //Move Motor Pos-01
                    if (FM.m_stSystemOpt.bUseMotor[ 0]) m_bRtn[ 0] = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X , m_cmdPos01[ 0]); else m_bRtn[ 0] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 1]) m_bRtn[ 1] = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y , m_cmdPos01[ 1]); else m_bRtn[ 1] = true;
                                                                                                                                         m_bRtn[ 2] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 3]) m_bRtn[ 3] = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_R , m_cmdPos01[ 3]); else m_bRtn[ 3] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 4]) m_bRtn[ 4] = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TH, m_cmdPos01[ 4]); else m_bRtn[ 4] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 5]) m_bRtn[ 5] = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TI, m_cmdPos01[ 5]); else m_bRtn[ 5] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 6]) m_bRtn[ 6] = SEQ_STORG.fn_ReqMoveMotr(EN_MOTR_ID.miSTR_Y , m_cmdPos01[ 6]); else m_bRtn[ 6] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 7]) m_bRtn[ 7] = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y , m_cmdPos01[ 7]); else m_bRtn[ 7] = true;
                                                                                                                                         m_bRtn[ 8] = true;
                                                                                                                                         m_bRtn[ 9] = true;
                    if (FM.m_stSystemOpt.bUseMotor[10]) m_bRtn[10] = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_T , m_cmdPos01[10]); else m_bRtn[10] = true;

                    //
                    for (int n = 0; n<MAX_MOTOR; n++)
                    {
                        if(!m_bRtn[n]) return false;
                    }

                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;
                
                case 17:
                    if (!m_tWarmDelay.OnDelay(true, 3000)) return false; 
                    
                    //Move Motor Pos-02
                    if (FM.m_stSystemOpt.bUseMotor[ 0]) m_bRtn[ 0] = SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_X , m_cmdPos02[ 0]); else m_bRtn[ 0] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 1]) m_bRtn[ 1] = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_Y , m_cmdPos02[ 1]); else m_bRtn[ 1] = true;
                                                                                                                                         m_bRtn[ 2] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 3]) m_bRtn[ 3] = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_R , m_cmdPos02[ 3]); else m_bRtn[ 3] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 4]) m_bRtn[ 4] = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TH, m_cmdPos02[ 4]); else m_bRtn[ 4] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 5]) m_bRtn[ 5] = SEQ_POLIS.fn_ReqMoveMotr(EN_MOTR_ID.miPOL_TI, m_cmdPos02[ 5]); else m_bRtn[ 5] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 6]) m_bRtn[ 6] = SEQ_STORG.fn_ReqMoveMotr(EN_MOTR_ID.miSTR_Y , m_cmdPos02[ 6]); else m_bRtn[ 6] = true;
                    if (FM.m_stSystemOpt.bUseMotor[ 7]) m_bRtn[ 7] = SEQ_CLEAN.fn_ReqMoveMotr(EN_MOTR_ID.miCLN_Y , m_cmdPos02[ 7]); else m_bRtn[ 7] = true;
                                                                                                                                         m_bRtn[ 8] = true;
                                                                                                                                         m_bRtn[ 9] = true;                    
                    if (FM.m_stSystemOpt.bUseMotor[10]) m_bRtn[10] = SEQ_TRANS.fn_ReqMoveMotr(EN_MOTR_ID.miTRF_T , m_cmdPos02[10]); else m_bRtn[10] = true;

                    //
                    for (int n = 0; n < MAX_MOTOR; n++)
                    {
                        if (!m_bRtn[n]) return false;
                    }

                    if(++m_nWarmCnt < FM.m_stSystemOpt.nMotrRepeat)
                    {
                        m_tWarmDelay.Clear();
                        m_nWarmStep--;
                        return false;
                    }

                    fn_WriteLog(string.Format($"[Warming Up] Motor Done Without Z-Axis / Count : {m_nWarmCnt}"));

                    m_nWarmCnt = 0;
                    
                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;

                case 18:
                    if (!m_tWarmDelay.OnDelay(true, 3000)) return false;

                    //Clamp 
                    if (FM.m_stSystemOpt.bUseClamp[0]) r1 = SEQ_SPIND.fn_MoveCylClamp(ccFwd); else r1 = true;
                    if (FM.m_stSystemOpt.bUseClamp[1]) r2 = SEQ_POLIS.fn_MoveCylClamp(ccFwd); else r2 = true;
                    if (FM.m_stSystemOpt.bUseClamp[2]) r3 = SEQ_CLEAN.fn_MoveCylClamp(ccFwd); else r3 = true;

                    if (!r1 || !r2 || !r3) return false;

                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;

                case 19:
                    if (!m_tWarmDelay.OnDelay(true, 3000)) return false;

                    //Clamp 
                    if (FM.m_stSystemOpt.bUseClamp[0]) r1 = SEQ_SPIND.fn_MoveCylClamp(ccBwd); else r1 = true;
                    if (FM.m_stSystemOpt.bUseClamp[1]) r2 = SEQ_POLIS.fn_MoveCylClamp(ccBwd); else r2 = true;
                    if (FM.m_stSystemOpt.bUseClamp[2]) r3 = SEQ_CLEAN.fn_MoveCylClamp(ccBwd); else r3 = true;

                    if (!r1 || !r2 || !r3) return false;

                    if (++m_nWarmCnt < FM.m_stSystemOpt.nClampRepeat)
                    {
                        m_tWarmDelay.Clear();
                        m_nWarmStep--;
                        return false;
                    }

                    fn_WriteLog(string.Format($"[Warming Up] Clamp Done / Count : {m_nWarmCnt}"));

                    m_nWarmCnt = 0; 

                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //DI Water
                case 20:
                    
                    //DI Supply
                    if (FM.m_stSystemOpt.bUseUtil[0]) SEQ_POLIS.fn_SetDIWaterValve(true);
                    if (FM.m_stSystemOpt.bUseUtil[1]) SEQ_CLEAN.fn_SetDIWaterValve(true);

                    if (!m_tWarmDelay.OnDelay(true, 1000 * FM.m_stSystemOpt.nSplyTime)) return false; //

                    SEQ_POLIS.fn_SetDIWaterValve(false);
                    SEQ_CLEAN.fn_SetDIWaterValve(false);
                    
                    m_tWarmDelay.Clear();
                    m_nWarmStep++;
                    return false;

                case 21:
                    if (!m_tWarmDelay.OnDelay(true, 3000)) return false; //3sec

                    //
                    if (m_nWarmCnt++ < FM.m_stSystemOpt.nUtilRepeat)
                    {
                        SEQ_POLIS.fn_SetDrain();
                        SEQ_CLEAN.fn_SetDrain();

                        m_tWarmDelay.Clear();
                        m_nWarmStep++;
                        return false;
                    }

                    fn_WriteLog(string.Format($"[Warming Up] Utility Done / Count : {m_nWarmCnt}"));
                    
                    m_nWarmCnt = 0;

                    m_tWarmDelay.Clear();
                    m_nWarmStep = 23;
                    return false;
                
                case 22:
                    if (SEQ_POLIS._bDrngSeqDrain || SEQ_CLEAN._bDrngSeqDrain) return false;

                    m_tWarmDelay.Clear();
                    m_nWarmStep = 20;
                    return false;
                
                case 23:

                    fn_WriteLog("End Warming Up");

                    SEQ.fn_SetLight(true); //SEQ._bLightOn = false; 

                    m_bDrngWarm = false;
                    m_nWarmStep =0;
                    return true;
            }
        }

    }
}
