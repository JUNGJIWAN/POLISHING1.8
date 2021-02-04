using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserFile;
using static WaferPolishingSystem.Define.UserEnum;

namespace WaferPolishingSystem.BaseUnit
{
    public enum EN_LAMP_OPER  : int 
    {
        LampOff   ,
        LampOn    ,
        LampFlick
    };

    //Buzzer Operations.
    //===========================================================================
    public enum  EN_BUZZ_OPER  : int 
    {
        BuzzOff ,
        Buzz1   ,
        Buzz2   ,
        Buzz3   ,
        Buzz4   ,
        Buzz5
    };


    /***************************************************************************/
    /* Structures & Variables                                                  */
    /***************************************************************************/
    //Lamp & Buzzer Informations.
    //===========================================================================
    public struct _TLampInfo {
        public int iRed  ;
        public int iYel  ;
        public int iGrn  ;
        public int iBuzz ;
        public string sKindStr;
    };


    /***************************************************************************/
    /* Class: TLampBuzz                                                        */
    /* Create:                                                                 */
    /* Developer:                                                              */
    /* Note:                                                                   */
    /***************************************************************************/
    public class LampBuzz
    {
        //Timer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        TOnDelayTimer m_FlickOnTimer   = new TOnDelayTimer();
        TOnDelayTimer m_FlickOffTimer  = new TOnDelayTimer();
        TOnDelayTimer m_FlickOnTimer2  = new TOnDelayTimer();
        TOnDelayTimer m_FlickOffTimer2 = new TOnDelayTimer();

        //Vars.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        
        public _TLampInfo[] m_LampInfo = new _TLampInfo[UserConst.MAX_LAMP_KIND];

        int  m_iTestStat ;
        bool m_bTest     ;
        bool m_bBuzzOff  ;
        bool m_bMute     ;
        bool m_bFlickLamp;
        bool m_bFlickBuzz;

        bool m_bDrngSave ;

        //Set Output
        EN_OUTPUT_ID m_iYLempRed ;
        EN_OUTPUT_ID m_iYLempYel ;
        EN_OUTPUT_ID m_iYLempGrn ;

        EN_OUTPUT_ID m_iYBuzz1   ;
        EN_OUTPUT_ID m_iYBuzz2   ;
        EN_OUTPUT_ID m_iYBuzz3   ;
        

        string[] m_sKindStr = new string[UserConst.MAX_LAMP_KIND];

        //Property.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public _TLampInfo this[int index]
        {
            get
            {
                if (index < 0 || index >= (int)EN_SEQ_STATE.EndofId) index = 0;
                return this.m_LampInfo[index];
            }
            set
            {
                if (index < 0 || index >= (int)EN_SEQ_STATE.EndofId) return ;
                this.m_LampInfo[index] = value;
            }

        }

        public bool _bDrngSave
        {
            get { return m_bDrngSave; }
        }
        public bool _bBuzzOff
        {
            get { return m_bBuzzOff; }
            set { m_bBuzzOff = value; }
        }
        public bool _bTest
        {
            get { return m_bTest; }
            set { m_bTest = value; }
        }
        public int _iTestStat
        {
            get { return m_iTestStat; }
            set { m_iTestStat = value; }
        }
        public EN_OUTPUT_ID _iYLempRed
        {
            set { m_iYLempRed = value; }
            get { return m_iYLempRed; }
        }
        public EN_OUTPUT_ID _iYLempYel
        {
            set { m_iYLempYel = value; }
            get { return m_iYLempYel; }
        }
        public EN_OUTPUT_ID _iYLempGrn
        {
            set { m_iYLempGrn = value; }
            get { return m_iYLempGrn; }
        }

        public EN_OUTPUT_ID _iYBuzz1
        {
            set { m_iYBuzz1 = value; }
            get { return m_iYBuzz1; }
        }
        public EN_OUTPUT_ID _iYBuzz2
        {
            set { m_iYBuzz2 = value; }
            get { return m_iYBuzz2; }
        }
        public EN_OUTPUT_ID _iYBuzz3
        {
            set { m_iYBuzz3 = value; }
            get { return m_iYBuzz3; }
        }

        //Member Class 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public LampBuzz()
        {
            for (int i = 0; i < UserConst.MAX_LAMP_KIND; i++)
            {
                m_LampInfo[i] = new _TLampInfo();
            }

            //
            Init();
        }

        //Init.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void  Init   ()
        {
            m_iTestStat = 5;
            m_bTest     = false;

            //Lamp IO Setting
            m_iYLempRed = EN_OUTPUT_ID.yLP_Red;
            m_iYLempYel = EN_OUTPUT_ID.yLP_Yellow;
            m_iYLempGrn = EN_OUTPUT_ID.yLP_Green;

            m_iYBuzz1   = EN_OUTPUT_ID.yLP_Buzz01;
            m_iYBuzz2   = EN_OUTPUT_ID.yNone;
            m_iYBuzz3   = EN_OUTPUT_ID.yNone;

            m_bBuzzOff = false;
            m_bMute    = false;

            for (int i = 0; i < UserConst.MAX_LAMP_KIND; i++) m_sKindStr[i] = "";

            string[] sKIND = {"Init"       ,
                              "Warning"    ,
                              "Error"      ,
                              "Run Warning",
                              "Run"        ,
                              "Stop"       ,
                              "",
                              "",
                              "",
                              ""
                              };
            
            SetKindStr(sKIND);
        }
        //--------------------------------------------------------------------------
        public void  Reset  ()
        {

        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Conversion - 화면 Display 시...

        public int     ConvStrToStat (string sLampBuzz, bool IsBuzz = false)      
        {
            int iStat;

            if (!IsBuzz)
            {
                iStat = (int)EN_LAMP_OPER.LampOff;

                if (sLampBuzz == "ON"      ) iStat = (int)EN_LAMP_OPER.LampOn   ;
                if (sLampBuzz == "FLICKING") iStat = (int)EN_LAMP_OPER.LampFlick;
            }
            else
            {
                iStat = (int)EN_BUZZ_OPER.BuzzOff;

                if (sLampBuzz == "Buzzer 1") iStat = (int)EN_BUZZ_OPER.Buzz1;
                if (sLampBuzz == "Buzzer 2") iStat = (int)EN_BUZZ_OPER.Buzz2;
                if (sLampBuzz == "Buzzer 3") iStat = (int)EN_BUZZ_OPER.Buzz3;
                if (sLampBuzz == "Buzzer 4") iStat = (int)EN_BUZZ_OPER.Buzz4;
                if (sLampBuzz == "Buzzer 5") iStat = (int)EN_BUZZ_OPER.Buzz5;

            }
            return iStat;
        }
        //--------------------------------------------------------------------------
        public string  ConvStatToStr (int iStat, bool IsBuzz = false)
        {
            String sTemp;
            EN_LAMP_OPER iLampStat;
            EN_BUZZ_OPER iBuzzStat;
            if (!IsBuzz)
            {
                iLampStat = (EN_LAMP_OPER)iStat;
                sTemp = "OFF";
                switch (iLampStat)
                {
                    default: sTemp = "OFF"; break;
                    case EN_LAMP_OPER.LampOff  : sTemp = "OFF";      break;
                    case EN_LAMP_OPER.LampOn   : sTemp = "ON";       break;
                    case EN_LAMP_OPER.LampFlick: sTemp = "FLICKING"; break;
                }
            }
            else
            {
                iBuzzStat = (EN_BUZZ_OPER)iStat;
                sTemp = "Buzzer Off";
                switch (iBuzzStat)
                {
                    default: sTemp = "Buzzer Off"; break;
                    case EN_BUZZ_OPER.BuzzOff: sTemp = "Buzzer Off"; break;
                    case EN_BUZZ_OPER.Buzz1  : sTemp = "Buzzer 1";   break;
                    case EN_BUZZ_OPER.Buzz2  : sTemp = "Buzzer 2";   break;
                    case EN_BUZZ_OPER.Buzz3  : sTemp = "Buzzer 3";   break;
                }

            }
            return sTemp;

        }
        //--------------------------------------------------------------------------
        //Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Set
        public void  SetKindStr(string[] sKIND )
        {
            for (int i = 0; i < UserConst.MAX_LAMP_KIND; i++)
            {
                if (sKIND[i] != "") m_sKindStr[i] = sKIND[i];

            }
        }
        //--------------------------------------------------------------------------
        //Status
        public bool  IsSetLamp  ()
        {
            if(m_iYLempRed < 0) return false;
            if(m_iYLempYel < 0) return false;
            if(m_iYLempGrn < 0) return false;
            return true;;
        }
        //--------------------------------------------------------------------------
        public bool  IsSetBuzz  ()
        {
            if(m_iYBuzz1 < 0 &&
               m_iYBuzz2 < 0 &&
               m_iYBuzz3 < 0) return false;
            return true;
        }
        //--------------------------------------------------------------------------
        public void  fn_BuzzOff    (bool Off = true)
        {
            m_bBuzzOff = Off;
        }
        //--------------------------------------------------------------------------
        public void fn_Mute(bool set = true)
        {
            m_bMute = set;
        }
        //--------------------------------------------------------------------------
        //Update
        public void  fn_Update(int iSeqStat)
        {
            //Local Var.
            int  iStat = m_bTest ? m_iTestStat : iSeqStat;

            if (m_bFlickLamp)   { m_FlickOnTimer  .Clear(); if (m_FlickOffTimer.OnDelay( m_bFlickLamp,  500 )) m_bFlickLamp = false; }
            else                { m_FlickOffTimer .Clear(); if (m_FlickOnTimer .OnDelay(!m_bFlickLamp,  500 )) m_bFlickLamp = true ; }

            if (m_bFlickBuzz)  { m_FlickOnTimer2 .Clear(); if (m_FlickOffTimer2.OnDelay( m_bFlickBuzz,  200 )) m_bFlickBuzz = false; }
            else               { m_FlickOffTimer2.Clear(); if (m_FlickOnTimer2 .OnDelay(!m_bFlickBuzz,  200 )) m_bFlickBuzz = true ; }


            if(iStat<0 || iStat>=UserConst.MAX_LAMP_KIND) return;

            UpdateLamp(m_iYLempRed, (EN_LAMP_OPER)m_LampInfo[iStat].iRed );
            UpdateLamp(m_iYLempYel, (EN_LAMP_OPER)m_LampInfo[iStat].iYel );
            UpdateLamp(m_iYLempGrn, (EN_LAMP_OPER)m_LampInfo[iStat].iGrn );
            UpdateBuzz(             (EN_BUZZ_OPER)m_LampInfo[iStat].iBuzz);
        }
        //--------------------------------------------------------------------------
        public void  UpdateLamp (EN_OUTPUT_ID iLamp, EN_LAMP_OPER iLampStat)
        {
            if(!IsSetLamp()) return;

            switch (iLampStat) 
            {
                case EN_LAMP_OPER.LampOff  : IO.YV[(int)iLamp] = false       ; break;
                case EN_LAMP_OPER.LampOn   : IO.YV[(int)iLamp] = true        ; break;
                case EN_LAMP_OPER.LampFlick: IO.YV[(int)iLamp] = m_bFlickLamp; break;
                default                    : IO.YV[(int)iLamp] = false       ; break;
            }
        }
        //--------------------------------------------------------------------------
        public void  UpdateBuzz (EN_BUZZ_OPER iBuzzStat)
        {
            bool isBuzzOn1  = false;
            bool isBuzzOn2  = false;
            bool isBuzzOn3  = false;

            if(!IsSetBuzz()) return;

            switch (iBuzzStat) 
            {
                 default                  : isBuzzOn1 = false   ; isBuzzOn2 = false   ; isBuzzOn3 = false; break;
                 case EN_BUZZ_OPER.BuzzOff: isBuzzOn1 = false   ; isBuzzOn2 = false   ; isBuzzOn3 = false; break;
                 case EN_BUZZ_OPER.Buzz1  : isBuzzOn1 = true    ; isBuzzOn2 = false   ; isBuzzOn3 = false; break;
                 case EN_BUZZ_OPER.Buzz2  : isBuzzOn1 = false   ; isBuzzOn2 = true    ; isBuzzOn3 = false; break;
                 case EN_BUZZ_OPER.Buzz3  : isBuzzOn1 = false   ; isBuzzOn2 = false   ; isBuzzOn3 = true ; break;
                 case EN_BUZZ_OPER.Buzz4  : if(m_bFlickBuzz)  { isBuzzOn1 = true    ; isBuzzOn2 = false   ; isBuzzOn3 = false; }
                                             else             { isBuzzOn1 = false   ; isBuzzOn2 = true    ; isBuzzOn3 = false; } break;
                 case EN_BUZZ_OPER.Buzz5  : if(m_bFlickBuzz)  { isBuzzOn1 = false   ; isBuzzOn2 = true    ; isBuzzOn3 = false; }
                                             else             { isBuzzOn1 = false   ; isBuzzOn2 = false   ; isBuzzOn3 = true ; } break;
            }

            //Buzzer Control.
            if(m_iYBuzz1>=0) IO.YV[(int)m_iYBuzz1] = !m_bMute && isBuzzOn1 && (!m_bBuzzOff || m_bTest); //TEST 인 경우에는 Buzz Off 무시
            if(m_iYBuzz2>=0) IO.YV[(int)m_iYBuzz2] = !m_bMute && isBuzzOn2 && (!m_bBuzzOff || m_bTest);
            if(m_iYBuzz3>=0) IO.YV[(int)m_iYBuzz3] = !m_bMute && isBuzzOn3 && (!m_bBuzzOff || m_bTest);
        }
        //--------------------------------------------------------------------------
        //Loading Para.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void fn_Load(bool bload)
        {
			//Local Var
			string sIniPath, sSection, m_strPath;

			m_strPath = fn_GetExePath();

			//File Path
			sIniPath = m_strPath + "SYSTEM\\LampBuzz.ini";

            m_bDrngSave = true;
            if (bload)
            {
                if (!fn_CheckFileExist(sIniPath))
                {
                    m_bDrngSave = false;
                    return;
                }

                for (int i = 0; i < UserConst.MAX_LAMP_KIND; i++)
                {
                    sSection = string.Format($"LAMP_BUZZ_{i:D2}");
                    m_LampInfo[i].iRed  = UserINI.fn_Load(sSection, "Red"   , m_LampInfo[i].iRed , sIniPath);
                    m_LampInfo[i].iYel  = UserINI.fn_Load(sSection, "Yellow", m_LampInfo[i].iYel , sIniPath);
                    m_LampInfo[i].iGrn  = UserINI.fn_Load(sSection, "Green" , m_LampInfo[i].iGrn , sIniPath);
                    m_LampInfo[i].iBuzz = UserINI.fn_Load(sSection, "Buzzer", m_LampInfo[i].iBuzz, sIniPath);
                }

            }
            else
            {
                for (int i = 0; i < UserConst.MAX_LAMP_KIND; i++)
                {
                    sSection = string.Format($"LAMP_BUZZ_{i:D2}");
                    UserINI.fn_Save(sSection, "KIND"  , ((EN_SEQ_STATE)i).ToString(), sIniPath);
                    UserINI.fn_Save(sSection, "Red"   , m_LampInfo[i].iRed , sIniPath);
                    UserINI.fn_Save(sSection, "Yellow", m_LampInfo[i].iYel , sIniPath);
                    UserINI.fn_Save(sSection, "Green" , m_LampInfo[i].iGrn , sIniPath);
                    UserINI.fn_Save(sSection, "Buzzer", m_LampInfo[i].iBuzz, sIniPath);
                }

            }
        }
    }
}
