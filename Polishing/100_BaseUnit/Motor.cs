using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACS.SPiiPlusNET;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunction;


namespace WaferPolishingSystem.BaseUnit
{

    public class Motor
    {
        //Var.
		bool   m_bConnect      ; //Connection Status
        bool   m_bConnectAsSim ; //Connection with Simulator
        //
        int    m_nSpeedMode;
        int    m_nACSErrNo ; //ACS Error No


        double[,] m_dPitch_R  = new double[MAX_MOTOR, MAX_PITCH];
		double[,] m_dPitch_C  = new double[MAX_MOTOR, MAX_PITCH];
	
		double[,] m_dCenPitch = new double[MAX_MOTOR, MAX_PITCH];
		
		string[] m_sName      = new string[MAX_MOTOR];
		string[] m_sNameAxis  = new string[MAX_MOTOR];
	
		//
		double[] m_dVelGain   = new double[MAX_MOTOR]; //Vel. Gain
		double[] m_dAccGain   = new double[MAX_MOTOR]; //Acc. Gain
	
	
        //AXIS      Axis[MAX_MOTOR];
        POSN_DATA[]       m_stPosnData = new POSN_DATA[MAX_MOTOR];
        MSTR_MOTR_OPTION  m_MstrMotr   = new MSTR_MOTR_OPTION();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //ACS Api
        private ACSControl ACS_MOTR = new ACSControl();

        //---------------------------------------------------------------------------
        //Property
        public bool _bConnect { get { return m_bConnect; } }



        public Motor()
		{
            //
            for (int n = 0; n< MAX_MOTOR; n++)
            {
                m_stPosnData[n] = new POSN_DATA();
            }
            

            //ACS Connection
            fn_ACSConnect(ACS_CON_NOR);
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     ACS Connection
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/12/16  9:55
        */
        public bool fn_ACSConnect(int type = 0)
        {
            //Local Var
            m_bConnect      = false;
            m_bConnectAsSim = false;

            string sIp = "10.0.0.100";
            int    port = (int)EthernetCommOption.ACSC_SOCKET_STREAM_PORT;

            if (type == ACS_CON_NOR)
            {
                m_bConnect = ACS_MOTR.fn_OnConnectEthernet(sIp, port);
            }
            else
            {
                m_bConnect = ACS_MOTR.fn_OnConnectSimulator();
                if(m_bConnect) m_bConnectAsSim = true;
            }

            if (m_bConnect)
            {
                if (m_bConnectAsSim) fn_WriteLog("[ACS] MOTOR Open as Simulator");
                else                 fn_WriteLog("[ACS] MOTOR Open");
            }
            else
            {
                fn_WriteLog(ACS_MOTR._sErrMsg);
            }

            return m_bConnect;

        }
        //---------------------------------------------------------------------------
        private void Init()
        {

        }
        private void fn_InitMotorName()
        {

        }
        private void fn_InitPositionName()
        {

        }
        private void fn_InitMinMax()
        {

        }

        private void fn_InitInposition()
        {

        }
        private void fn_InitMstrMotor()
        { 
        }
        private void fn_InitMstrDSTB()
        {

        }
        private void fn_SetMotorName(EN_MOTOR_ID no, string Name, string Axis)
        {

        }
        private void fn_SetPosn(EN_MOTOR_ID no, int nPart, string sPart, int nPosnId, string sName, string sUnit, int nManNo)
        {

        }
        private void fn_SetMinMaxData(EN_MOTOR_ID no, double dMinPos, double dMaxPos, double dMinVel, double dMaxVel, double dMinAcc, double dMaxAcc)
        { 
        }

        private void fn_SetInposition(EN_MOTOR_ID no, double inpos)
        {

        }
        public void fn_Reset(EN_MOTOR_ID Axis)
        {

        }
        public void fn_Reset()
        {
            for (int Axis = 0; Axis < MAX_MOTOR; Axis++)
            {
                fn_Reset((EN_MOTOR_ID)Axis);
            }
        }

        	//Motor Check
    	public bool CheckAxis(EN_MOTOR_ID Axis) { return ((int)Axis >= 0 && (int)Axis < MAX_MOTOR); }
    
    
    	public void   fn_SetAxis        ()
        {

        }
        public void  fn_SetParameter   (EN_MOTOR_ID Axis)
        {

        }

        public void fn_SetStoragePitch (double dpitch_R , double dpitch_C, double dCenpitch = 0.0  ) //Pitch Setting
        {

        }
        public void fn_SetMagazinePitch(double dpitch_R , double dpitch_C, double dCenpitch = 0.0  ) //Pitch Setting
        {

        }
    	public void fn_SetPitch(EN_MOTOR_ID Axis, double dpitch_R, double dpitch_C, double dCenPitch = 0.0) //Pitch
        {

        }

        public double fn_GetPitch_R     (EN_MOTOR_ID Axis, int step = 0)
        {

            return 0.0;
        }
        public double fn_GetPitch_C     (EN_MOTOR_ID Axis, int step = 0)
        {

            return 0.0;
        }
        public double fn_GetCenPitch    (EN_MOTOR_ID Axis, int step = 0)
        {

            return 0.0;
        }

        public void fn_SetServo       (bool on)
        {

        }
        public void fn_SetServo       (EN_MOTOR_ID Axis, bool on)
        {

        }
        public void fn_SetServoAllOff ()
        {

        }
        public void fn_ClearHomeEnd   (EN_MOTOR_ID Axis)
        {

        }
        public void fn_ClearHomeEnd   ()
    	{
    		for (int i =0 ; i < MAX_MOTOR; i++)
    		{
    			fn_ClearHomeEnd((EN_MOTOR_ID)i);
    		}
    	}


        public void fn_ClearPosition  (EN_MOTOR_ID Axis)
        {

        }
        public void fn_ClearPosition  () //Clear Position - All Motor 
    	{
    		for (int i = 0; i < MAX_MOTOR; i++)
    		{
    			fn_ClearPosition((EN_MOTOR_ID)i);
    		}
    
    	}
        public void fn_ClearACSError  ()
        {

        }

        //---------------------------------------------------------------------------
        //Communication
        public bool fn_OpenMotor (int type = 0 )
        {

            return true; 
        }
        public void fn_CloseMotor()
        {

        }
        public bool m_fn_IsConnect () { return m_bConnect; }

        //Motion
        public bool fn_EmrgStop  (EN_MOTOR_ID Axis)
        {
            return true; 
        }
        public bool m_fn_EmrgStop  ()
        {
            
            return true; 

        }
        public bool fn_Stop      (EN_MOTOR_ID Axis, double Dec = 0.1)
        {

            return true; 
        }
        public bool fn_Stop      ()
    	{
    		for (int Axis = 0; Axis < MAX_MOTOR; Axis++)
    		{
    			fn_Stop((EN_MOTOR_ID)Axis);
    		}
    		return true;
    	}


        public bool   fn_MoveHome       (EN_MOTOR_ID Axis)
        {

        }
    	public bool   fn_MoveR          (EN_MOTOR_ID Axis, double Pos, double Vel, double Acc            )
        {

        }
    	public bool   fn_MoveA          (EN_MOTOR_ID Axis, double Pos, double Vel, double Acc            )
        {

        }
    	public bool   fn_MoveA          (EN_MOTOR_ID Axis, double Pos, double Vel, double Acc, double Dec)
        {

        }
    	public bool   fn_MoveMotr       (EN_MOTOR_ID Axis, double Pos, double Vel, double Acc, bool R    ) //상대 위치 이동
        {

        }
    	public bool   fn_MoveMotr       (EN_MOTOR_ID Axis, double Pos, double Vel, double Acc, double Dec) //절대 위치 이동
        {

        }
    	public bool   fn_MoveMotr       (EN_MOTOR_ID Axis, double Pos, int SPD = (int)EN_MOTOR_VEL.mvNormal) //Position Move
        {

        }

    	public bool   fn_MoveJog        (EN_MOTOR_ID Axis, bool Dir)
        {

        }
    	public void   fn_MotrDone       (EN_MOTOR_ID Axis, bool ChkEnc, double Inpos) //사용자 임의 In-position Check
        {

        }
    	public void   fn_MotrDone       (EN_MOTOR_ID Axis                           ) //Motor Done 상태 Check
        {

        }
    	public bool   fn_MoveInc        (EN_MOTOR_ID Axis, double Pos)
        {

        }
    	public bool   fn_MoveDec        (EN_MOTOR_ID Axis, double Pos)
        {

        }
    	public bool   fn_MoveAsComd     (EN_MOTOR_ID Axis, EN_MOTOR_COMD_ID Cmd, int Spd)
        {

        }
    	public bool   fn_ComparePosByCmd(EN_MOTOR_ID Axis, EN_MOTOR_COMD_ID Cmd, double Inpos = 0)
        {

        }
    	public bool   fn_ComparePos	   (EN_MOTOR_ID Axis, double Pos)
        {

        }

    	public int    fn_GetPosnIdByCmd (EN_MOTOR_COMD_ID Cmd)
        {

        }


        //---------------------------------------------------------------------------
    	public void   fn_SetVGain(EN_MOTOR_ID Axis, double Vel) 
    	{
    		if (!CheckAxis(Axis)) return; 
    		m_dVelGain[(int)Axis] = Vel;
    	}
    	//---------------------------------------------------------------------------
    	public void   fn_SetAGain(EN_MOTOR_ID Axis, double Acc)
    	{
    		if (!CheckAxis(Axis)) return;
    		m_dAccGain[(int)Axis] = Acc;
    	}
        //---------------------------------------------------------------------------
        public void fn_SetGain(EN_MOTOR_ID Axis, double Vel, double Acc)
    	{
    		if (!CheckAxis(Axis)) return;
    		m_dVelGain[(int)Axis] = Vel;
    		m_dAccGain[(int)Axis] = Acc;
    	}
        //---------------------------------------------------------------------------
        public void fn_SetJogHigh     (EN_MOTOR_ID Axis, bool set) 
    	{ 
    		m_Axis[(int)Axis].stJogInfo.m_bJogHigh = set;
    	}

        //---------------------------------------------------------------------------
        //Motor Status
        public bool fn_GetHomeEnd    (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetBusy       (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetJogN       (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetJogP       (EN_MOTOR_ID Axis)
        {
        }
        public bool fn_GetJogHigh    (EN_MOTOR_ID Axis)
        {

        }
        public double fn_GetJogSpd     (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetServo      (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetAlarm      (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetHome       (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetCCW        (EN_MOTOR_ID Axis)
        {

        }
        public bool fn_GetCW         (EN_MOTOR_ID Axis)
        {

        }
    	public bool   fn_GetStop       (EN_MOTOR_ID Axis)
        {

        }
        public bool   fn_GetMoving     (EN_MOTOR_ID Axis)
        {

        }
        public double fn_GetInpos      (EN_MOTOR_ID Axis)
        {

        }
        public bool   fn_GetAllServoOn ()
        {

        }
        public bool   fn_GetAllServoOff()
        {

        }
        public bool   fn_GetAllHomeEnd ()
        {

        }

        public double fn_GetCmdPos     (EN_MOTOR_ID Axis)
        {

        }
        public double fn_GetTrgPos     (EN_MOTOR_ID Axis)
        {

        }
        public double fn_GetEncPos     (EN_MOTOR_ID Axis)
        {

        }
        public double fn_GetAbsEncPos  (EN_MOTOR_ID Axis)
        {

        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Get.
        public double    m_fn_GetMaxPos(EN_MOTOR_ID Axis) { if (!CheckAxis(Axis)) return 0.0; return m_Axis[(int)Axis].stMotrLimit.MaxPos; }
    	public double    m_fn_GetMaxVel(EN_MOTOR_ID Axis) { if (!CheckAxis(Axis)) return 0.0; return m_Axis[(int)Axis].stMotrLimit.MaxVel; }
    	public double    m_fn_GetMaxAcc(EN_MOTOR_ID Axis) { if (!CheckAxis(Axis)) return 0.0; return m_Axis[(int)Axis].stMotrLimit.MaxAcc; }
    	public double    m_fn_GetMinPos(EN_MOTOR_ID Axis) { if (!CheckAxis(Axis)) return 0.0; return m_Axis[(int)Axis].stMotrLimit.MinPos; }
    	public double    m_fn_GetMinVel(EN_MOTOR_ID Axis) { if (!CheckAxis(Axis)) return 0.0; return m_Axis[(int)Axis].stMotrLimit.MinVel; }
    	public double    m_fn_GetMinAcc(EN_MOTOR_ID Axis) { if (!CheckAxis(Axis)) return 0.0; return m_Axis[(int)Axis].stMotrLimit.MinAcc; }
    	public MOTR_LIMT m_fn_GetMaxMin(EN_MOTOR_ID Axis) 
    	{ 
    		MOTR_LIMT stML = new MOTR_LIMT(); 
    		
            if (!CheckAxis(Axis)) return stML; 

    		return m_Axis[(int)Axis].stMotrLimit;
    	}
    
    	//
    	public int m_fn_GetMaxPosnCnt()
    	{
    		int n = 0; 
    		for (int i=0; i<MAX_MOTOR;i++)
    		{
    			if(n <= m_stPosnData[i].nPosnCnt) n = m_stPosnData[i].nPosnCnt;
    		}
    		
    		return n; 
    	}
        
    	//Check Motor Position
    	public int fn_WhereMotor(EN_MOTOR_ID Axis)
        {

        }
    
    	//Set.	
    	public void fn_SetMaxPos(EN_MOTOR_ID Axis, double    Data) { if (!CheckAxis(Axis)) return; m_Axis[(int)Axis].stMotrLimit.MaxPos = Data; }
    	public void fn_SetMaxVel(EN_MOTOR_ID Axis, double    Data) { if (!CheckAxis(Axis)) return; m_Axis[(int)Axis].stMotrLimit.MaxVel = Data; }
    	public void fn_SetMaxAcc(EN_MOTOR_ID Axis, double    Data) { if (!CheckAxis(Axis)) return; m_Axis[(int)Axis].stMotrLimit.MaxAcc = Data; }
    	public void fn_SetMinPos(EN_MOTOR_ID Axis, double    Data) { if (!CheckAxis(Axis)) return; m_Axis[(int)Axis].stMotrLimit.MinPos = Data; }
    	public void fn_SetMinVel(EN_MOTOR_ID Axis, double    Data) { if (!CheckAxis(Axis)) return; m_Axis[(int)Axis].stMotrLimit.MinVel = Data; }
    	public void fn_SetMinAcc(EN_MOTOR_ID Axis, double    Data) { if (!CheckAxis(Axis)) return; m_Axis[(int)Axis].stMotrLimit.MinAcc = Data; }
    	public void fn_SetMaxMin(EN_MOTOR_ID Axis, MOTR_LIMT Data) { if (!CheckAxis(Axis)) return; m_Axis[(int)Axis].stMotrLimit        = Data; }
        public void fn_SetMaxMin(EN_MOTOR_ID Axis, double MaxPos, double MaxVel, double MaxAcc, double MinPos, double MinVel, double MinAcc)
        {

        }


        //---------------------------------------------------------------------------
        //Inspect Motor
        //Tool Impact.
        public bool fn_InspectCrash   (bool bRun)
        {

        }
        public bool fn_CheckCrash     (EN_MOTOR_ID Axis, double dTrgPos)
        {

        }
        //Min-Max
        public bool fn_InitPos        ()
        {

        }
    	public bool fn_CheckMinMaxErr (bool ChkOnly = false)
        {

        }
    	public bool fn_CheckMinMax    (EN_MOTOR_ID Axis, double Pos, double Vel, double Acc)
        {

        }
    	public bool fn_CheckMinMaxPos (EN_MOTOR_ID Axis, double Pos     )
        {

        }
        public bool fn_CheckMinMaxVel (EN_MOTOR_ID Axis, double Vel     )
        {

        }
        public bool fn_CheckMinMaxAcc (EN_MOTOR_ID Axis, double Acc     )
        {

        }
        public bool fn_CheckMinMaxPos (EN_MOTOR_ID Axis, int    PosIndex)
        {

        }
        public bool fn_CheckMinMaxVel (EN_MOTOR_ID Axis, int    VelIndex)
        {

        }
        public bool fn_CheckMinMaxAcc (EN_MOTOR_ID Axis, int    AccIndex)
        {

        }

        //---------------------------------------------------------------------------
        //ACSPL+ Program Management Function(ACS Only)
        public bool fn_SetBuffer         (int nBuffNo, bool on)
        {

        }

        // 	public bool fn_RunBuff           (int BuffNo          ) //Starts up ACSPL+ program in the specified program buffer.
        // 	public bool fn_StopBuff          (int BuffNo          ) //Stops ACSPL+ program in the specified program buffer(s).
        public int  fn_GetBufferStatus   (int nBuffNo         )
        {

        }
        public int  fn_GetBufferStatusRun(int nBuffNo         )
        {

        }
        public int  fn_GetBuffCount      ()
        {

        }
        //---------------------------------------------------------------------------

        //Update
        public void fn_Update       (bool Run)
        {

        }
    
    	
    	//Load Parameter
    	public void fn_Load         (bool bLoad             )
        {

        }
    	public void fn_Load         (bool bLoad, string Job)
        {

        }
    	public void fn_Load         (bool bLoad, EN_MOTOR_ID Axis, string sPath)
        {

        }
        public void fn_LoadOption   (EN_MOTOR_ID Axis, bool bLoad               )
        {

        }
        public void fn_LoadMotorData(bool bLoad)
        {

        }
    
    	//---------------------------------------------------------------------------
    	public int  fn_GetACSErrNo() {
    		return m_nACSErrNo;
    	}
    
    	//Update Motor Data
//     	public void fn_UpdateMotionParaData(bool bLoad, CGridCtrl* Grid              );
//         public void fn_UpdateMotorPosData  (bool bLoad, CGridCtrl* Grid, int Axis = 0);
//         public void fn_UpdateMotorInfoData (bool bLoad, CGridCtrl* Grid, int Axis = 0);
//         public void fn_UpdateMasterMotor   (CGridCtrl* Grid);
    
    	public string fn_GetMotorName    (int n) { return m_sName    [n]; }
        public string fn_GetMotorAxis    (int n) { return m_sNameAxis[n]; }

        public string fn_GetMotorNameAxis(int n) { return fn_GetMotorName(n) + (" [ ") + fn_GetMotorAxis(n) + (" ]"); }

        public double fn_GetMotrPosnFrCmd(EN_MOTOR_ID Axe, EN_MOTOR_COMD_ID Comd)
        {

        }

        //void m_fn_CheckErr    (void            );
        public void fn_CheckErrList   ()
        {

        }
        public void fn_SetSoftLimit(EN_MOTOR_ID Axe, double dPos, double dNeg, bool bUse)
        {
            fn_SetSoftLimitPos(Axe, dPos, bUse);
            fn_SetSoftLimitNeg(Axe, dNeg, bUse);
        }
        public void fn_SetSoftLimitPos(EN_MOTOR_ID Axe, double dPos, bool bUse)
        {

        }
        public void fn_SetSoftLimitNeg(EN_MOTOR_ID Axe, double dPos, bool bUse)
        {

        }
    
    
    
    
    
    }   
}
