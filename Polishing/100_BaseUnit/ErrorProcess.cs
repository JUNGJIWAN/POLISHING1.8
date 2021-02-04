using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaferPolishingSystem.Define;
using WaferPolishingSystem.Unit;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserINI;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserFile;
using System.Windows.Forms.VisualStyles;

namespace WaferPolishingSystem.BaseUnit
{
    
    public class Error
    {
	    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Var
	    bool     m_bOn        ; //Alarm On
		bool     m_bOnAtRun   ; //Run 중 Error
		bool     m_bUpdate    ; 
		
		int      m_nGrade     ; //등급
		int      m_nPart      ; //발생 Part
		int      m_nKind      ; //종류
			     
		string   m_strName    ;  //Error Name
		string   m_strCause   ; //발생 원인
		string   m_strSolution; //해결 방법
	
		DateTime m_tSetTime   ;
		DateTime m_tResetTime ;

		//---------------------------------------------------------------------------
		//Property		   
		public bool   _bOn         { get { return m_bOn        ; } set { m_bOn         = value; } } 
		public bool   _bOnAtRun    { get { return m_bOnAtRun   ; } set { m_bOnAtRun    = value; } }
		public bool   _bUpdate     { get { return m_bUpdate    ; } set { m_bUpdate     = value; } }
						    						   			  		   
		public int    _nGrade      { get { return m_nGrade     ; } set { m_nGrade      = value; } }
		public int    _nPart       { get { return m_nPart      ; } set { m_nPart       = value; } }
		public int    _nKind       { get { return m_nKind      ; } set { m_nKind       = value; } }
						    	   					   			  		   
		public string _sName       { get { return m_strName    ; } set { m_strName     = value; } }
		public string _sCause      { get { return m_strCause   ; } set { m_strCause    = value; } }
		public string _sSolution   { get { return m_strSolution; } set { m_strSolution = value; } }


		/************************************************************************/
		/* 생성자                                                                     */
		/************************************************************************/
		public Error(bool set)
		{
			//
			m_bOn         = set;
			m_bOnAtRun    = set;
			m_bUpdate     = set;
					      
			m_nGrade      = -1;
			m_nPart       = 0;
			m_nKind       = 0;

			m_strName     = string.Empty;
			m_strCause    = string.Empty;
			m_strSolution = string.Empty;

		}


		//Set Function
		public void fn_SetErrOn     (bool On      ) { m_bOn         = On;    }
		public void fn_SetErrOnAtRun(bool On      ) { m_bOnAtRun    = On;    }
		public void fn_SetUpdate    (bool On      ) { m_bUpdate     = On;    }

		public void fn_SetGrade     (int grade    ) { m_nGrade      = grade; }
		public void fn_SetPart      (int part     ) { m_nPart       = part ; }
		public void fn_SetKind      (int kind     ) { m_nKind       = kind ; }

		public void fn_SetName      (string name  ) { m_strName     = name ; }
		public void fn_SetCause     (string cause ) { m_strCause    = cause; }
		public void fn_SetSolution  (string solu  ) { m_strSolution = solu ; }

		public void fn_SetErrTime   (DateTime time) { m_tSetTime    = time ; }
		public void fn_SetRstErrTime(DateTime time) { m_tResetTime  = time ; }

									   
		//Get Function				 					    
		public bool   fn_GetErrOn      () { return m_bOn         ; }
		public bool   fn_GetErrOnAtRun () { return m_bOnAtRun    ; }
		public bool   fn_GetUpdate     () { return m_bUpdate     ; }
		public int    fn_GetGrade      () { return m_nGrade      ; }
		public int    fn_GetPart       () { return m_nPart       ; }
		public int    fn_GetKind       () { return m_nKind       ; }
	
		public string fn_GetName       () { return m_strName     ; }
		public string fn_GetCause      () { return m_strCause    ; }
		public string fn_GetSolution   () { return m_strSolution ; }

		//Init.
		public void fn_Init () { m_bOn = false; m_nGrade = (int)EN_ERR_GRADE.egInit; }
		public void fn_Reset() { m_bOn = false; }

    }

	public class ErrorProcess
	{
		//List Box
		ListBox m_lErrList  = null;
		ListBox m_lWarnList = null;

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Var
		bool m_bIsErr         ;
		bool m_bIsWarn        ;
		bool m_bIsDisp        ;
		bool m_bUpdate        ; //Display 진행여부
		bool m_bDisplayErrForm;
		bool m_bDrngSave      ;
		bool m_bLampTest      ;
		bool m_bNeedSave      ; //

		int m_nSeqState       ;
		int m_nLastErr        ;
		int m_nLastWarn       ;
		int m_nTestState      ;

		//
		public LAMP_INFO m_LampInfo = new LAMP_INFO(0);

		//
		public Error[] ERR = new Error[MAX_ERROR];

		static public ErrorProcess m_Epu = null;

		//Property
		public bool _bIsErr          { get { return m_bIsErr ; } }
		public bool _bIsWarn         { get { return m_bIsWarn; } }
		public bool _bIsDisp         { get { return m_bIsDisp; } }
		public bool _bDisplayErrForm { get { return m_bDisplayErrForm; } set { m_bDisplayErrForm = value; } }

		public bool _bNeedSave       { get { return m_bNeedSave; } }

		public int _nLastWarn        { get {return m_nLastWarn;} }
		public int _nLastErr         { get { return m_nLastErr;} }

		/************************************************************************/
		/* 생성자                                                                */
		/************************************************************************/
		public ErrorProcess()
		{
			
			//
			m_Epu = this;

			//
			for (int i = 0; i<MAX_ERROR; i++)
			{
				ERR[i] = new Error(false);
			}
			
			//
			fn_Init();

		}

		//---------------------------------------------------------------------------
		//Init
		private void fn_Init()
		{
			string sTemp;
	
			//Error Init
			for (int i = 0 ; i < MAX_ERROR; i++)
			{
				sTemp = string.Format($"ERROR_{i+1:D4}");
				ERR[i].fn_Init       (     );
				ERR[i].fn_SetName    (""   );
				ERR[i].fn_SetCause   (sTemp);
				ERR[i].fn_SetSolution(sTemp);
			}
		
			m_lErrList        = null;
			m_lWarnList       = null;
			
			m_bDisplayErrForm = false; 
	
			m_bIsErr          = false; 
			m_bIsWarn         = false; 
			m_bIsDisp         = false; 
			m_bDrngSave       = false;
			m_bLampTest       = false; 
					      
			m_nLastErr        = -1;
			m_nLastWarn       = -1;
			m_nTestState      = (int)EN_SEQ_STATE.STOP;

			m_bNeedSave       = false; 
		}
		//---------------------------------------------------------------------------
		public void fn_Init(int no)
		{
            if (no < 0 || no >= MAX_ERROR) return;

            ERR[no].fn_Init();
        }
        //---------------------------------------------------------------------------
        //Get Function
        public bool fn_GetHasErr   () 
		{ 
			return m_bIsErr   ;
		}
		public bool fn_GetHasWarn  () 
		{ 
			return m_bIsWarn  ;
		}
		public bool fn_GetHasDisp  () 
		{ 
			return m_bIsDisp  ;
		}
		public bool fn_GetUpdate   () 
		{ 
			return m_bUpdate  ;
		}
		public bool fn_IsDrngSave  () 
		{ 
			return m_bDrngSave;
		}

		public int fn_GetLastErr  () 
		{ 
			return m_nLastErr ;
		}
		public int fn_GetSeqState () 
		{ 
			return m_nSeqState;
		}
		//---------------------------------------------------------------------------
		public int fn_GetLastErrNo (bool bChkOnAtRun)
		{
            int No = -1;
            for (int i = (int)EN_ERR_LIST.ERR_0001; i < MAX_ERROR; i++)
            {
                if (bChkOnAtRun) 
				{ 
					if (ERR[i].fn_GetErrOn     () && 
				       (ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egError) && ERR[i].fn_GetErrOnAtRun()) No = i; 
				}
                else 
				{ 
					if (ERR[i].fn_GetErrOn() && (ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egError)) No = i; 
				}
            }

            return No;

        }
		//---------------------------------------------------------------------------
		//Set Function		   
		public void fn_SetHasErr     (bool set) { m_bIsErr    = set; }
		public void fn_SetHasWarn    (bool set) { m_bIsWarn   = set; }
		public void fn_SetHasDisp    (bool set) { m_bIsDisp   = set; }
		public void fn_SetUpdate     (bool set) { m_bUpdate   = set; }
		public void fn_SetLastErr    (int  set) { m_nLastErr  = set; }
		public void fn_SetSeqState   (int  set) { m_nSeqState = set; }

		//---------------------------------------------------------------------------
        public void m_fn_SetList(ListBox List, ListBox List1)
        {
            m_lErrList  = List ;
            m_lWarnList = List1;
        }
	    //---------------------------------------------------------------------------
		public void fn_SetGrade      (int No, int Grade       ) { if (No < 0 || No >= MAX_ERROR) return; ERR[No].fn_SetGrade   (Grade   ); }
		public void fn_SetPart       (int No, int Part        ) { if (No < 0 || No >= MAX_ERROR) return; ERR[No].fn_SetPart    (Part    ); }
		public void fn_SetKind       (int No, int Kind        ) { if (No < 0 || No >= MAX_ERROR) return; ERR[No].fn_SetKind    (Kind    ); }
			  	 			       
		public void fn_SetName       (int No, string Name     ) { if (No < 0 || No >= MAX_ERROR) return; ERR[No].fn_SetName    (Name    ); }
		public void fn_SetCause      (int No, string Cause    ) { if (No < 0 || No >= MAX_ERROR) return; ERR[No].fn_SetCause   (Cause   ); }
		public void fn_SetSolution   (int No, string Solution ) { if (No < 0 || No >= MAX_ERROR) return; ERR[No].fn_SetSolution(Solution); }
		//---------------------------------------------------------------------------	  	 			       							  
		public int  fn_GetGrade      (int No                  ) { if (No < 0 || No >= MAX_ERROR) return 0; return ERR[No].fn_GetGrade    (); }
		public int  fn_GetPart       (int No                  ) { if (No < 0 || No >= MAX_ERROR) return 0; return ERR[No].fn_GetPart     (); }
		public int  fn_GetKind       (int No                  ) { if (No < 0 || No >= MAX_ERROR) return 0; return ERR[No].fn_GetKind     (); }

		public string fn_GetName     (int No                  ) { if (No < 0 || No >= MAX_ERROR) return ""; return ERR[No].fn_GetName    (); }
		public string fn_GetCause    (int No                  ) { if (No < 0 || No >= MAX_ERROR) return ""; return ERR[No].fn_GetCause   (); }
		public string fn_GetSolution (int No                  ) { if (No < 0 || No >= MAX_ERROR) return ""; return ERR[No].fn_GetSolution(); }
	

		//---------------------------------------------------------------------------
		/**    
		@brief     Error Clear
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/8/28  13:43
		*/
		public void fn_Clear(int no)
		{
			if (no < 0 || no >= MAX_ERROR) return; 

			if (ERR[no]._bOn) //Error 상태이면...
			{
				//
				ERR[no].fn_SetErrTime(DateTime.Now);
				ERR[no].fn_Reset();
				//ERR[no].fn_SetUpdate(false);
			}
		}

		//---------------------------------------------------------------------------
		public void fn_Clear()
		{
			//
			int nLastErrNo = fn_GetLastErrNo(true);

			//Log
			if (nLastErrNo > 0 && nLastErrNo < MAX_ERROR) {
				string sTemp; 
				sTemp = string.Format($"Clear Error [{m_nLastErr + 1:D4}] ");
				sTemp = sTemp + fn_GetName(m_nLastErr);
				fn_WriteLog(sTemp);

				//PMC 
				PMC.fn_AlarmSet(0, -1);
			}

			//
			if (nLastErrNo > 0 && ERR[nLastErrNo].fn_GetGrade() == (int)EN_ERR_GRADE.egError) m_nLastErr = nLastErrNo;
			else                                                                              m_nLastErr = -1; 

			//Error Clear
			m_bIsErr  = false;
			m_bIsWarn = false;
			m_bIsDisp = false;

			m_bDisplayErrForm = false;
	
			for (int i = 0; i < MAX_ERROR; i++) { fn_Clear(i); }

			//Lamp or Buzzer Clear
			m_bLampTest = false;
			
		}

		//---------------------------------------------------------------------------
		/**    
		@brief     Error Setting Function
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/8/28  16:38
		*/
		public bool fn_SetErr(EN_ERR_LIST No, bool Con)
		{
			return fn_SetErr((int)No, Con);
		}
		public bool fn_SetErr(int No, bool Con)
		{
			//Check Error No
			if (No < 0         ) return false;
			if (No >= MAX_ERROR) return false;

			int enGrade = ERR[No].fn_GetGrade();

			//Check Grade
			if (enGrade == (int)EN_ERR_GRADE.egDisplay || enGrade == (int)EN_ERR_GRADE.egWarning)
			{
				if (Con) fn_SetErr(No);
				else     fn_Clear (No);
			}
			else if (enGrade == (int)EN_ERR_GRADE.egError) //한번 Error 발생 후에는 Error 발생하지 않도록...
			{
				if (!ERR[No].fn_GetErrOn() && Con) fn_SetErr(No);
			}
			
			//Return Error Status.
			if (ERR[No]._bOn && Con) Con = true;
			return Con; 
		}
		//---------------------------------------------------------------------------
		/**    
		<summary>
			Set Error Function
		</summary>
		<param name="no"> ERROR List </param>
		@author    정지완(JUNGJIWAN)
		@date      2020/02/08 14:01
		*/
		public void fn_SetErr(EN_ERR_LIST No)
		{
			fn_SetErr((int)No);
		}
        public void fn_SetErr(int No)
		{
			//Check Error No
			if (No < 0         ) return;
			if (No >= MAX_ERROR) return; 

			if (ERR[No].fn_GetErrOn()) return; 
	
			string sGrade = string.Empty;
			bool   bRun   = SEQ._bRun; //&& !SEQ._bStop; //Run 중 Error 인지 확인

			if (No < (int)EN_ERR_LIST.ERR_0100) ERR[No]._nGrade = (int)EN_ERR_GRADE.egWarning;
			else
			{
				if (ERR[No]._sName != "" && ERR[No]._nGrade == (int)EN_ERR_GRADE.egInit) ERR[No]._nGrade = (int)EN_ERR_GRADE.egError;
			}

			//
			ERR[No].fn_SetErrOn     (true              );
			ERR[No].fn_SetErrTime   (DateTime.Now      );
			ERR[No].fn_SetErrOnAtRun(bRun? true : false);

			//
			PMC.fn_AlarmSet(No, ERR[No].fn_GetGrade());

			//
			if      (ERR[No].fn_GetGrade() == (int)EN_ERR_GRADE.egError  ) { m_bIsErr  = true; sGrade = "[ERR ]"; }
            else if (ERR[No].fn_GetGrade() == (int)EN_ERR_GRADE.egWarning) { m_bIsWarn = true; sGrade = "[WARN]"; }
            else if (ERR[No].fn_GetGrade() == (int)EN_ERR_GRADE.egDisplay) { m_bIsDisp = true; sGrade = "[DISP]"; }
			else                                                           { m_bIsDisp = true; sGrade = "[DISP]"; }
																												                  
			//Water Level Error
			if(No == (int)EN_ERR_LIST.ERR_0436)
			{
				IO.fn_CloseCLNValve();
			}
			
			//JUNG/200612
			if (!m_bIsErr                  ) return;
			if (!ERR[No].fn_GetErrOnAtRun()) return;

			//JUNG/200907
            if (No > 100)
			{
				//Log
				string sTemp; //, sErrNo;
				sTemp = string.Format($"{sGrade} Error No : {No + 1:D4} / Name : {ERR[No].fn_GetName()}");

				fn_WriteLog(sTemp, EN_LOG_TYPE.ltJam);

				//Last Error Setting
				m_nLastErr = No;
			}
            
        }

        //---------------------------------------------------------------------------
        /**    
		@brief     실시간 Error 상태 Check
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/9/17  13:49
		*/
        public bool fn_IsErrState()
		{
			//Local Var.
			bool isErr = false; 
			bool isWar = false; 
			bool isDis = false; 

			//
			for (int i = (int)EN_ERR_LIST.ERR_0001; i<MAX_ERROR; i++)
			{
				if (ERR[i].fn_GetErrOn())
				{
						 if(ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egError  ) isErr = true;
					else if(ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egWarning) isWar = true;
					else if(ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egDisplay) isDis = true;
				}
			}

			m_bIsErr  = isErr; 
			m_bIsWarn = isWar; 
			m_bIsDisp = isDis; 

			return m_bIsErr; 
		}

		//---------------------------------------------------------------------------
		/**    
		@brief     Error Display at List Box
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/8/28  17:12
		*/
		public void fn_DisplayErrList()
		{
			string  Str;
			bool    isUpdateErr = false;
			bool    isUpdateWrn = false; 

			if (m_lErrList  == null) return;
			if (m_lWarnList == null) return;

			//Check Update List.
			for (int i = 0; i < MAX_ERROR; i++) {
				if ((ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egWarning) || (ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egDisplay)) 
				{
					if ( ERR[i].fn_GetErrOn() && !ERR[i].fn_GetUpdate()) {isUpdateWrn = true; m_nLastWarn = i; }
					if (!ERR[i].fn_GetErrOn() &&  ERR[i].fn_GetUpdate()) {isUpdateWrn = true; m_nLastWarn = i; }
				}
				if ((ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egError))
				{
					if ( ERR[i].fn_GetErrOn() && !ERR[i].fn_GetUpdate()) isUpdateErr = true;
					if (!ERR[i].fn_GetErrOn() &&  ERR[i].fn_GetUpdate()) isUpdateErr = true;
				}

			}

			if (isUpdateWrn) 
			{ 
				if (m_lWarnList != null) m_lWarnList.Items.Clear(); 
			}

			if (isUpdateErr) 
			{ 
				if (m_lErrList  != null) m_lErrList .Items.Clear(); 
			}

			if (isUpdateErr || isUpdateWrn) { for (int i = 0; i < MAX_ERROR; i++) ERR[i].fn_SetUpdate(false); }

			//Display to ListBox.
			for (int i = 0; i < MAX_ERROR; i++) {
				if (ERR[i].fn_GetErrOn() && !ERR[i].fn_GetUpdate()) {
					ERR[i].fn_SetUpdate(true);

					if (ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egError && isUpdateErr) 
					{
						//Str = string.Format("[ERR{0:D4}] ", i+1);
						Str = string.Format($"[{DateTime.Now:HH:mm:ss}]") + string.Format($"[ERR{i + 1:D4}] ");

						Str += ERR[i].fn_GetName();
						if (m_lErrList != null) m_lErrList.Items.Insert(0, Str);
					}
					
					if (ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egWarning && isUpdateWrn)
					{
						Str = string.Format($"[{DateTime.Now:HH:mm:ss}]") + string.Format($"[ERR{i + 1 :D4}] ");

						Str += ERR[i].fn_GetName();
						if (m_lWarnList != null) m_lWarnList.Items.Insert(0, Str);
					}
					
					if (ERR[i].fn_GetGrade() == (int)EN_ERR_GRADE.egDisplay && isUpdateWrn)
					{
						Str = string.Format($"[{DateTime.Now:HH:mm:ss}]") + string.Format($"[ERR{ i + 1:D4}] ");

						Str += ERR[i].fn_GetName();
						if (m_lWarnList != null) m_lWarnList.Items.Insert(0, Str);
					}
				}
			}

			//List Clear
			int iCnt = m_lErrList.Items.Count;
			if (!fn_GetHasErr() && iCnt > 0)
			{
				if (m_lErrList!= null) m_lErrList.Items.Clear();
			}
			
			iCnt = m_lWarnList.Items.Count;
			if (!fn_GetHasDisp() && !fn_GetHasWarn() && iCnt > 0)
			{
				if (m_lWarnList != null) m_lWarnList.Items.Clear();
			}

		}

		//---------------------------------------------------------------------------
		/**    
		@brief     Error 실시간 Check
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/9/11  9:21
		*/
		int iSubFormSel, iSubFormKind;
		public void fn_ErrorDisplay()
		{
			//
			fn_DisplayErrList();

			//
			bool isNeedSubFrm = fn_IsNeedSubForm(out iSubFormSel, out iSubFormKind);


			if (isNeedSubFrm && m_bDisplayErrForm) m_bDisplayErrForm = false; 

			//Check Form Show
			if (m_bDisplayErrForm  ) return; //Start 시 Clear or Reset 시 Clear???
			if (FormAlarm   == null) return; 

			//Last Error Check
			int nLastErr = fn_GetLastErrNo(false);
			if (nLastErr > (int)EN_ERR_LIST.ERR_0100 && nLastErr < MAX_ERROR)
			{
				if (ERR[nLastErr].fn_GetGrade() == (int)EN_ERR_GRADE.egError) //Error Grade
				{
					if (!FormAlarm.IsVisible  & !SEQ._bRun)
					{
						m_bDisplayErrForm = true;

                        if (isNeedSubFrm)
                        {
							FormAlarm._nSelSubPart = iSubFormSel;
						}
						else FormAlarm._nSelSubPart = 0;


						//Form Show
						FormAlarm.ShowDialog();
					}
				}
			}

			//Warning
			if (!m_bDisplayErrForm)
			{
				if(m_nLastWarn > (int)EN_ERR_LIST.ERR_0100 && m_nLastWarn < MAX_ERROR)
				{
                    if (!FormAlarm.IsVisible  && !SEQ._bRun )
                    {
                        //m_bDisplayErrForm = true;

						//Form Show
						//FormAlarm.ShowDialog();
                    }

                }
            }

            //if Need a Sub Alarm Window, Write down here
            if (isNeedSubFrm)
            {

            }








        }
        //---------------------------------------------------------------------------
        private bool fn_IsNeedSubForm(out int sel, out int kind)
        {
			//Init.
			sel  = 0;
			kind = 0;





			return sel > 0 ? true : false ; 
        }

		//---------------------------------------------------------------------------
		/**    
		@brief     Get Lamp Status 
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2020/1/2  14:36
		*/
		public bool fn_GetLampStat(int iStat)
		{
			bool bOn = false;

			if      (iStat == (int)EN_LAMP_STATE.lsLampOn   ) bOn = true;
			else if (iStat == (int)EN_LAMP_STATE.lsLampFlick) bOn = SEQ._bFlick1;

			return bOn;
		}
		//---------------------------------------------------------------------------
		//Buzzer & Lamp.
		public void fn_GetBuzzStat(int iStat, ref bool bOn1, ref bool bOn2, ref bool bon3)
		{
			EN_BUZZ_STATE iState = (EN_BUZZ_STATE)iStat;

			switch (iState)
			{
				case EN_BUZZ_STATE.bsBuzzOff: bOn1 = false; bOn2 = false; bon3 = false; break;
				case EN_BUZZ_STATE.bsBuzz1  : bOn1 = true ; bOn2 = false; bon3 = false; break;
				case EN_BUZZ_STATE.bsBuzz2  : bOn1 = false; bOn2 = true ; bon3 = false; break;
				case EN_BUZZ_STATE.bsBuzz3  : bOn1 = false; bOn2 = false; bon3 = true ; break;
				default                     : bOn1 = false; bOn2 = false; bon3 = false; break;
			}
		}
		//---------------------------------------------------------------------------
		/**    
		@brief     Update Status of Lamp, Buzzer 
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2020/1/2  10:26
		*/
		public void fn_UpdateLampBuzz()
		{
			//Local Var.
			int  nState    = m_bLampTest ? m_nTestState : SEQ.fn_GetSeqStatus(); //
			bool isBuzzOn1 = false;
			bool isBuzzOn2 = false;
			bool isBuzzOn3 = false;

			int nLampRed    = (int)EN_OUTPUT_ID.yLP_Red   ; //Io Set
			int nLampYellow = (int)EN_OUTPUT_ID.yLP_Yellow;
			int nLampGreen  = (int)EN_OUTPUT_ID.yLP_Green ;

			int nYBuzz01    = (int)EN_OUTPUT_ID.yLP_Buzz01;
			int nYBuzz02    = -1;//(int)EN_OUTPUT_ID.yLP_Buzz02;
			int nYBuzz03    = -1;//(int)EN_OUTPUT_ID.yLP_Buzz03;

			bool bRedOn     = false; 
			bool bYellowOn  = false;
			bool bGreenOn   = false;

			switch ((EN_SEQ_STATE)nState)
			{
			case EN_SEQ_STATE.STOP :
					bRedOn     = fn_GetLampStat(m_LampInfo.nStopRed);
					bYellowOn  = fn_GetLampStat(m_LampInfo.nStopYel);
					bGreenOn   = fn_GetLampStat(m_LampInfo.nStopGrn);

					fn_GetBuzzStat(m_LampInfo.nStopBuzz, ref isBuzzOn1, ref isBuzzOn2, ref isBuzzOn3);

					break;

			case EN_SEQ_STATE.RUNNING :
					bRedOn     = fn_GetLampStat(m_LampInfo.nRunRed );
					bYellowOn  = fn_GetLampStat(m_LampInfo.nRunYel );
					bGreenOn   = fn_GetLampStat(m_LampInfo.nRunGrn );

					fn_GetBuzzStat(m_LampInfo.nRunBuzz, ref isBuzzOn1, ref isBuzzOn2, ref isBuzzOn3);

					break;

			case EN_SEQ_STATE.ERROR   :
					bRedOn     = fn_GetLampStat(m_LampInfo.nErrorRed);
					bYellowOn  = fn_GetLampStat(m_LampInfo.nErrorYel);
					bGreenOn   = fn_GetLampStat(m_LampInfo.nErrorGrn);

					fn_GetBuzzStat(m_LampInfo.nErrorBuzz, ref isBuzzOn1, ref isBuzzOn2, ref isBuzzOn3);

					break;

			case EN_SEQ_STATE.WARNING    :
					bRedOn     = fn_GetLampStat(m_LampInfo.nWarningRed);
					bYellowOn  = fn_GetLampStat(m_LampInfo.nWarningYel);
					bGreenOn   = fn_GetLampStat(m_LampInfo.nWarningGrn);

					fn_GetBuzzStat(m_LampInfo.nWarningBuzz, ref isBuzzOn1, ref isBuzzOn2, ref isBuzzOn3);
					break;

			case EN_SEQ_STATE.INIT    :

					bRedOn     = fn_GetLampStat(m_LampInfo.nInitRed);
					bYellowOn  = fn_GetLampStat(m_LampInfo.nInitYel);
					bGreenOn   = fn_GetLampStat(m_LampInfo.nInitGrn);

					fn_GetBuzzStat(m_LampInfo.nInitBuzz, ref isBuzzOn1, ref isBuzzOn2, ref isBuzzOn3);
					break;
	
			case EN_SEQ_STATE.RUNWARN :
	
					bRedOn     = fn_GetLampStat(m_LampInfo.nRunWarnRed);
					bYellowOn  = fn_GetLampStat(m_LampInfo.nRunWarnYel);
					bGreenOn   = fn_GetLampStat(m_LampInfo.nRunWarnGrn);

					fn_GetBuzzStat(m_LampInfo.nRunWarnBuzz, ref isBuzzOn1, ref isBuzzOn2, ref isBuzzOn3);
					break;

			}


			//Alarm 인경우, Warning 인 경우.
			IO.YV[nLampRed   ] = bRedOn   ; 
			IO.YV[nLampYellow] = bYellowOn;
			IO.YV[nLampGreen ] = bGreenOn ;
	
			IO.YV[nYBuzz01   ] = isBuzzOn1;
			IO.YV[nYBuzz02   ] = isBuzzOn2;
			IO.YV[nYBuzz03   ] = isBuzzOn3;


		}


		//---------------------------------------------------------------------------
		/**    
		@brief     Error Information Load/Save
		@return    
		@param     
		@remark    
		-        
		@author    정지완(JUNGJIWAN)
		@date      2019/9/17  11:37
		*/
		public void fn_LoadErrorData(bool bload)
		{
			//Local Var
			string sIniPath, sTemp, m_strPath, sLoadName;
			sLoadName = string.Empty;

			m_bDrngSave = true;

			m_strPath = fn_GetExePath();

			//Input
			sIniPath = m_strPath + "SYSTEM\\Error.ini";
			if (bload)
			{
				if (!fn_CheckFileExist(sIniPath))
				{
					m_bDrngSave = false; 
					return;
				}
	
				for (int i = 0; i < MAX_ERROR; i++)
				{
					sTemp = string.Format($"(ERR{i + 1:D4})");
					ERR[i]._sName     = fn_Load("NAME"    , sTemp, ERR[i]._sName    , sIniPath);
					ERR[i]._nGrade    = fn_Load("GRADE"   , sTemp, ERR[i]._nGrade   , sIniPath);
					ERR[i]._nPart     = fn_Load("PART"    , sTemp, ERR[i]._nPart    , sIniPath);
					ERR[i]._nKind     = fn_Load("KIND"    , sTemp, ERR[i]._nKind    , sIniPath);
					ERR[i]._sCause    = fn_Load("CAUSE"   , sTemp, ERR[i]._sCause   , sIniPath);
                    ERR[i]._sSolution = fn_Load("SOLUTION", sTemp, ERR[i]._sSolution, sIniPath);

					if (ERR[i]._sName == "" || ERR[i]._sName == string.Empty)
                    {
						if (GETNAME(i) != "")
                        {
                        	ERR[i]._sName = GETNAME(i);
                        	
							m_bNeedSave = true; 
                        }
                        else
                        {
                        	ERR[i].fn_SetGrade((int)EN_ERR_GRADE.egDisplay);
                        	ERR[i].fn_SetPart(-1);
                        }
                    }
				}
			}
			else
			{
				for (int i = 0; i < MAX_ERROR; i++)
				{
					sTemp = string.Format($"(ERR{i + 1:D4})");
					fn_Save("NAME"    , sTemp, ERR[i]._sName     , sIniPath);
					fn_Save("GRADE"   , sTemp, ERR[i]._nGrade    , sIniPath);
					fn_Save("PART"    , sTemp, ERR[i]._nPart     , sIniPath);
					fn_Save("KIND"    , sTemp, ERR[i]._nKind     , sIniPath);
					fn_Save("CAUSE"   , sTemp, ERR[i]._sCause    , sIniPath);
					fn_Save("SOLUTION", sTemp, ERR[i]._sSolution , sIniPath);
			
				}
		
			}

			m_bDrngSave = false;

		}
		//---------------------------------------------------------------------------
		public bool fn_IsEMOError()
        {
			if (ERR[(int)EN_ERR_LIST.ERR_0101]._bOn) return true; //EMO
			if (ERR[(int)EN_ERR_LIST.ERR_0102]._bOn) return true;
			if (ERR[(int)EN_ERR_LIST.ERR_0103]._bOn) return true;

			return false; 
        }
		//---------------------------------------------------------------------------
		public void fn_LoadLampData(bool bload)
		{
            //Local Var
            string sIniPath, m_strPath;

            m_bDrngSave = true;

            m_strPath = fn_GetExePath();

            //Input
            sIniPath = m_strPath + ("SYSTEM\\LampBuzzData.ini");
            if (bload)
            {
                if (!fn_CheckFileExist(sIniPath))
                {
                    m_bDrngSave = false;
                    return;
                }

				m_LampInfo.nInitRed     = fn_Load("LAMP_RED", "INIT"   , 0, sIniPath);
				m_LampInfo.nWarningRed  = fn_Load("LAMP_RED", "WARN"   , 0, sIniPath);
				m_LampInfo.nErrorRed    = fn_Load("LAMP_RED", "ERR"    , 0, sIniPath);
				m_LampInfo.nRunRed      = fn_Load("LAMP_RED", "RUN"    , 0, sIniPath);
				m_LampInfo.nStopRed     = fn_Load("LAMP_RED", "STOP"   , 0, sIniPath);
				m_LampInfo.nMaintRed    = fn_Load("LAMP_RED", "MAINT"  , 0, sIniPath);
				m_LampInfo.nRunWarnRed  = fn_Load("LAMP_RED", "RUNWARN", 0, sIniPath);

				m_LampInfo.nInitYel     = fn_Load("LAMP_YEL", "INIT"   , 0, sIniPath);
				m_LampInfo.nWarningYel  = fn_Load("LAMP_YEL", "WARN"   , 0, sIniPath);
				m_LampInfo.nErrorYel    = fn_Load("LAMP_YEL", "ERR"    , 0, sIniPath);
				m_LampInfo.nRunYel      = fn_Load("LAMP_YEL", "RUN"    , 0, sIniPath);
				m_LampInfo.nStopYel     = fn_Load("LAMP_YEL", "STOP"   , 0, sIniPath);
				m_LampInfo.nMaintYel    = fn_Load("LAMP_YEL", "MAINT"  , 0, sIniPath);
				m_LampInfo.nRunWarnYel  = fn_Load("LAMP_YEL", "RUNWARN", 0, sIniPath);

				m_LampInfo.nInitGrn     = fn_Load("LAMP_GRN", "INIT"   , 0, sIniPath);
				m_LampInfo.nWarningGrn  = fn_Load("LAMP_GRN", "WARN"   , 0, sIniPath);
				m_LampInfo.nErrorGrn    = fn_Load("LAMP_GRN", "ERR"    , 0, sIniPath);
				m_LampInfo.nRunGrn      = fn_Load("LAMP_GRN", "RUN"    , 0, sIniPath);
				m_LampInfo.nStopGrn     = fn_Load("LAMP_GRN", "STOP"   , 0, sIniPath);
				m_LampInfo.nMaintGrn    = fn_Load("LAMP_GRN", "MAINT"  , 0, sIniPath);
				m_LampInfo.nRunWarnGrn  = fn_Load("LAMP_GRN", "RUNWARN", 0, sIniPath);

				m_LampInfo.nInitBuzz    = fn_Load("BUZZER"  , "INIT"   , 0, sIniPath);
				m_LampInfo.nWarningBuzz = fn_Load("BUZZER"  , "WARN"   , 0, sIniPath);
				m_LampInfo.nErrorBuzz   = fn_Load("BUZZER"  , "ERR"    , 0, sIniPath);
				m_LampInfo.nRunBuzz     = fn_Load("BUZZER"  , "RUN"    , 0, sIniPath);
				m_LampInfo.nStopBuzz    = fn_Load("BUZZER"  , "STOP"   , 0, sIniPath);
				m_LampInfo.nMaintBuzz   = fn_Load("BUZZER"  , "MAINT"  , 0, sIniPath);
				m_LampInfo.nRunWarnBuzz = fn_Load("BUZZER"  , "RUNWARN", 0, sIniPath);
							    
			}
			else
			{
				fn_Save("LAMP_RED", "INIT"   , m_LampInfo.nInitRed     , sIniPath);
				fn_Save("LAMP_RED", "WARN"   , m_LampInfo.nWarningRed  , sIniPath);
				fn_Save("LAMP_RED", "ERR"    , m_LampInfo.nErrorRed    , sIniPath);
				fn_Save("LAMP_RED", "RUN"    , m_LampInfo.nRunRed      , sIniPath);
				fn_Save("LAMP_RED", "STOP"   , m_LampInfo.nStopRed     , sIniPath);
				fn_Save("LAMP_RED", "MAINT"  , m_LampInfo.nMaintRed    , sIniPath);
				fn_Save("LAMP_RED", "RUNWARN", m_LampInfo.nRunWarnRed  , sIniPath);

				fn_Save("LAMP_YEL", "INIT"   , m_LampInfo.nInitYel     , sIniPath);
				fn_Save("LAMP_YEL", "WARN"   , m_LampInfo.nWarningYel  , sIniPath);
				fn_Save("LAMP_YEL", "ERR"    , m_LampInfo.nErrorYel    , sIniPath);
				fn_Save("LAMP_YEL", "RUN"    , m_LampInfo.nRunYel      , sIniPath);
				fn_Save("LAMP_YEL", "STOP"   , m_LampInfo.nStopYel     , sIniPath);
				fn_Save("LAMP_YEL", "MAINT"  , m_LampInfo.nMaintYel    , sIniPath);
				fn_Save("LAMP_YEL", "RUNWARN", m_LampInfo.nRunWarnYel  , sIniPath);
							  							  		   	   
				fn_Save("LAMP_GRN", "INIT"   , m_LampInfo.nInitGrn     , sIniPath);
				fn_Save("LAMP_GRN", "WARN"   , m_LampInfo.nWarningGrn  , sIniPath);
				fn_Save("LAMP_GRN", "ERR"    , m_LampInfo.nErrorGrn    , sIniPath);
				fn_Save("LAMP_GRN", "RUN"    , m_LampInfo.nRunGrn      , sIniPath);
				fn_Save("LAMP_GRN", "STOP"   , m_LampInfo.nStopGrn     , sIniPath);
				fn_Save("LAMP_GRN", "MAINT"  , m_LampInfo.nMaintGrn    , sIniPath);
				fn_Save("LAMP_GRN", "RUNWARN", m_LampInfo.nRunWarnGrn  , sIniPath);

				fn_Save("BUZZER"  , "INIT"   , m_LampInfo.nInitBuzz    , sIniPath);
				fn_Save("BUZZER"  , "WARN"   , m_LampInfo.nWarningBuzz , sIniPath);
				fn_Save("BUZZER"  , "ERR"    , m_LampInfo.nErrorBuzz   , sIniPath);
				fn_Save("BUZZER"  , "RUN"    , m_LampInfo.nRunBuzz     , sIniPath);
				fn_Save("BUZZER"  , "STOP"   , m_LampInfo.nStopBuzz    , sIniPath);
				fn_Save("BUZZER"  , "MAINT"  , m_LampInfo.nMaintBuzz   , sIniPath);
				fn_Save("BUZZER"  , "RUNWARN", m_LampInfo.nRunWarnBuzz , sIniPath);
			}
			//
			m_bDrngSave = false;

		}



    }
}
