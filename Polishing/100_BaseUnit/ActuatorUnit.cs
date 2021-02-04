using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.BaseUnit.ActuatorId;
using static WaferPolishingSystem.BaseUnit.ManualId;

namespace WaferPolishingSystem.BaseUnit
{
    public class TKaBit
    {

        public bool DifuFlag;
        public bool Enabled ;
        public int  Axis    ;
        public int  Tag     ;


        public bool On ;
        public bool Off;

        public TKaBit ()
        {
            Clear();
            Axis = 0;            
        }
        //~TKaBit(){}

        public void  Clear  ()
        {
            On = false;
            Off = true;
            Enabled = true;
            Tag = 0;
        }
        //--------------------------------------------------------------------------
        public void  Out    (bool SeqCondition)
        {
            //if (!Enabled) return;
            if (SeqCondition) { On = true; Off = false; }
            else { On = false; Off = true; }
        }
        //--------------------------------------------------------------------------
        public void  Flk    () // Repeat On/Off.
        {
            if (On) Reset();
            else    Set();
        }
        //--------------------------------------------------------------------------
        public void  KeepOn ()
        {
            Enabled = true;
            Set();
            Enabled = false;
        }
        //--------------------------------------------------------------------------
        public void  KeepOff()
        {
            Enabled = true;
            Reset();
            Enabled = false;
        }
        //--------------------------------------------------------------------------
        public void  Set    ()
        {
            Out(true);
        }
        //--------------------------------------------------------------------------
        public void  Reset    ()
        {
            Out(false);
            Clear();
        }
        //--------------------------------------------------------------------------
        public void  Difu   (int Condition    )
        {
            if (!On)
            {
                if (Condition==1)
                {
                    if (!DifuFlag)
                    {
                        On = true;
                        Off = false;
                        DifuFlag = true;
                    }
                }
            }
            else
            {
                On = false;
                Off = true;
            }

            if (Condition==0) DifuFlag = false;
        }
    };

    /**
    @class    Actuator class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/02/14 19:01
    */
    public class TActuator:TKaBit
    {
        const int SKIP4  =   9999;
        const int SKIP5  =   9999;

        //Timer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`
        TOnDelayTimer tFwdTimeOutTimer = new TOnDelayTimer();
        TOnDelayTimer tBwdTimeOutTimer = new TOnDelayTimer();
        TOnDelayTimer tFwdOnDelayTimer = new TOnDelayTimer();
        TOnDelayTimer tBwdOnDelayTimer = new TOnDelayTimer();
 
        //Vars.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //private:   /* Member Var.             */
        //String.
        string Name;
        string Comt;

        //I/O Index.
        int    m_xfwdId   ;  //FWD Sensor Address.
        int    m_xbwdId   ;  //BWD Sensor Address.
        int    m_yfwdId   ;  //FWD Output Address.
        int    m_ybwdId   ;  //BWD Output Address.
        int    m_xfwdIndex;  //FWD Sensor Index No.
        int    m_xbwdIndex;  //BWD Sensor Index No.
        int    m_yfwdIndex;  //FWD Output Index No.
        int    m_ybwdIndex;  //BWD Output Index No.

        //Delay Time.
        int    nFwdOnDelayTime     ;
        int    nBwdOnDelayTime     ;
        int    nFwdTimeOutDelayTime;                                                                  
        int    nBwdTimeOutDelayTime;

        int    ThreadApplyTime;
        int    iInv;            //Inverse Output.
        int    LastOutCommand;
        int    nReCntFwd, nReCntBwd;

        //protected: /* Inheritable Vars.        */

        //public:    /* Direct Accessable Vars.  */
        public bool ApplyTimeout;
        public bool ApplyOutComplete;
        public bool ApplyKillOutput;
        public bool EnableLastOutCmdTOut;

        public bool vfy,dfy, dfx, fx, vfx, odfx, ftoe, Ltftoe;
        public bool vby,dby, dbx, bx, vbx, odbx, btoe, Ltbtoe;

        //Define Manual & No
        public int  m_iManNo;
        public int  m_iErrNo;

        public int  m_iRetry;



        //Property.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        //Member Class 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public TActuator()
        {
            
        }
        //~TActuator() { }

        public bool   Input (int n         )
        {
            if ((n < 0) || (n >= IO._iNumOfX)) return false;
            return IO.XV[n];
        }
        //--------------------------------------------------------------------------
        public bool   Output(int n         )
        {
            if ((n < 0) || (n >= IO._iNumOfY)) return false;
            return IO.YR[n];
        }
        //--------------------------------------------------------------------------
        public void Output(int n, int on)
        {
            if ((n < 0) || (n >= IO._iNumOfY)) return;
            IO.YV[n] = (on==1) ? true : false;
        }
        //--------------------------------------------------------------------------
        //Init.
        public void Init()
        {
            m_xfwdId = m_xfwdIndex = -1;
            m_xbwdId = m_xbwdIndex = -1;
            m_yfwdId = m_yfwdIndex = -1;
            m_ybwdId = m_ybwdIndex = -1;

            m_iManNo = -1;
            m_iErrNo = -1;
            m_iRetry =  0;

            nReCntFwd = 0;
            nReCntBwd = 0; 

            //options.
            ApplyTimeout     = false;
            ApplyOutComplete = false;

            nFwdTimeOutDelayTime = 0;
            nFwdTimeOutDelayTime = 0;

            Name = "NONE";

            dfy = dfx = fx = vfx = odfx = ftoe = Ltftoe = false;
            dby = dbx = bx = vbx = odbx = btoe = Ltbtoe = false;

            LastOutCommand = -1;

            nFwdOnDelayTime = 0;
            nBwdOnDelayTime = 0;

            tFwdTimeOutTimer.Clear();
            tBwdTimeOutTimer.Clear();
            tFwdOnDelayTimer.Clear();
            tBwdOnDelayTimer.Clear();
           
        }
        //--------------------------------------------------------------------------
        //basic  function
        public void Update()
        {
	        //
	        UpdateInput();
        }
        //--------------------------------------------------------------------------
        public void UpdateInput()
        {
            //Local Var.
            bool  isXFwdID  ;
            bool  isXBwdID  ;
            bool  isYFwdID  ;
            bool  isYBwdID  ;
            int   iXfwdId   ;
            int   iXbwdId   ;
            int   iYfwdId   ;
            int   iYbwdId   ;
            int   iXfwdIndex;
            int   iXbwdIndex;
            int   iYfwdIndex;
            int   iYbwdIndex;

            //Set I/O ID.
            iXfwdId    = (iInv ==1) ? m_xbwdId : m_xfwdId;
            iXbwdId    = (iInv ==1) ? m_xfwdId : m_xbwdId;
            iYfwdId    = (iInv ==1) ? m_ybwdId : m_yfwdId;
            iYbwdId    = (iInv ==1) ? m_yfwdId : m_ybwdId;

            //Set I/O Index.
            iXfwdIndex = (iInv ==1) ? m_xbwdIndex : m_xfwdIndex;
            iXbwdIndex = (iInv ==1) ? m_xfwdIndex : m_xbwdIndex;
            iYfwdIndex = (iInv ==1) ? m_ybwdIndex : m_yfwdIndex;
            iYbwdIndex = (iInv ==1) ? m_yfwdIndex : m_ybwdIndex;

            if(!IO._bConnect) return;

            //Check SKIP.
            if ((iYfwdId == SKIP4 && iYbwdId == SKIP4) ||
                (iYfwdId == SKIP5 && iYbwdId == SKIP5)   )
            {
                Rst();
                return ;
            }

            //Set exist ID.
            isXFwdID = (iXfwdId != SKIP4 && iXfwdId != SKIP5);
            isXBwdID = (iXbwdId != SKIP4 && iXbwdId != SKIP5);
            isYFwdID = (iYfwdId != SKIP4 && iYfwdId != SKIP5);
            isYBwdID = (iYbwdId != SKIP4 && iYbwdId != SKIP5);

            //I/O.
            if (isXFwdID) dfx = Input (iXfwdIndex); //Detect X/Y Val.
            if (isXBwdID) dbx = Input (iXbwdIndex);
            if (isYFwdID) dfy = Output(iYfwdIndex);
            if (isYBwdID) dby = Output(iYbwdIndex);

            //Virtual Input Sensor
                //FWD.
            if      (isXFwdID) vfx =  dfx;
            else if (isXBwdID) vfx = !dbx;
            else if (isYFwdID) vfx =  dfy;
            else if (isYBwdID) vfx = !dby;
            else               vfx = !dby;
                //BWD.
            if      (isXBwdID) vbx =  dbx;
            else if (isXFwdID) vbx = !dfx;
            else if (isYBwdID) vbx =  dby;
            else if (isYFwdID) vbx = !dfy;
            else               vbx = !dfy;

            //Virtual Output Sensor
            if (isYFwdID) vfy =  dfy;
            else          vfy = !dby;
            if (isYBwdID) vby =  dby;
            else          vby = !dfy;

            //OnDelay Timer.
            odfx = tFwdOnDelayTimer.OnDelay(vfx , nFwdOnDelayTime);
            odbx = tBwdOnDelayTimer.OnDelay(vbx , nBwdOnDelayTime);

            //Check time out by delay result.
                //Time out By Input
            bool r = true;
            if ( odfx && !odbx) r = false;
            if (!odfx &&  odbx) r = false;

            if ( odfx &&  odbx) r = true ;
            if (!odfx && !odbx) r = true ;

                //Time out By Output
            if ( dfy  &&  odbx) r = true ;
            if ( dfy  && !odfx) r = true ;
            if ( dby  &&  odfx) r = true ;
            if ( dby  && !odbx) r = true ;

            //Is timeout.
            ftoe = tFwdTimeOutTimer.OnDelay(ApplyTimeout && r , nFwdTimeOutDelayTime);
            btoe = tBwdTimeOutTimer.OnDelay(ApplyTimeout && r , nBwdTimeOutDelayTime);
            //if (ftoe) Ltftoe = true;
            //if (btoe) Ltbtoe = true;
            if (ftoe) //JUNG/201012/Retry 
            {
                if(++nReCntFwd >= m_iRetry)
                {  
                    Ltftoe    = true;
                    nReCntFwd = m_iRetry;
                }
                else
                {
                    tFwdTimeOutTimer.Clear();
                    ftoe = false;
                }
            }

            if (btoe)
            {
                if (++nReCntBwd >= m_iRetry)
                {
                    Ltbtoe    = true;
                    nReCntBwd = m_iRetry;
                }
                else
                {
                    tBwdTimeOutTimer.Clear();
                    btoe = false;
                }
            }

            //Error Action.
            if (ftoe || btoe) { fx = false; bx = false; }
            else              { fx = odfx ; bx = odbx ; }

            //Apply Out Complete.
            if (ApplyOutComplete) {
                Rst();
                fx = dfy;
                bx = dby;

                if      (isYFwdID) fx =  dfy;
                else if (isYBwdID) fx = !dby;
                else               fx =  !bx;

                if      (isYBwdID) bx =  dby;
                else if (isYFwdID) bx = !dfy;
                else               bx =  !fx;
                }
        }
        //--------------------------------------------------------------------------
        public string GetName()            { return Name; }
        public void   SetName(string data) { Name = data; }
        public string GetComt()            { return Comt; }
        public void   SetComt(string data) { Comt = data; }
        public int    GetManNo()           { return m_iManNo; }
        public void   SetManNo(int data)   { m_iManNo = data; }
        public int    GetErrNo()           { return m_iErrNo; }
        public void   SetErrNo(int data)   { m_iErrNo = data; }
        
        public int    GetRetryCntFwd()     { return nReCntFwd; }
        public int    GetRetryCntBwd()     { return nReCntBwd; }
        public int    GetRetryCnt()        { return m_iRetry; }
        public void   SetRetryCnt(int data){ m_iRetry = data; }


        //--------------------------------------------------------------------------
        //status functions.
        public int GetLastCmd() { return LastOutCommand; }
        //--------------------------------------------------------------------------
        public bool Complete(int Cmd) //1:On,Fwd,Up,Close,Rotate 0:Off,Bwd,Dw, Open.
        {
            if (Cmd == 1)
            {
                if (fx && !bx) Rst();

                return fx && !bx;
            }
            else
            {
                if (!fx && bx) Rst();

                return !fx && bx;
            }
        }
        //--------------------------------------------------------------------------
        public bool Complete(int Cmd, int Fwd, int Bwd)
        {
            if (Cmd==1) return  Input(Fwd) && !Input(Bwd);
            else        return !Input(Fwd) &&  Input(Bwd);
        }
        //--------------------------------------------------------------------------
        public bool Trg(int Cmd) //Last Command.
        {
            return (LastOutCommand == Cmd) ? true : false;
        }
        //--------------------------------------------------------------------------
        public int Err()
        {
            if (Ltftoe) return 1;
            if (Ltbtoe) return 2;
            return 0;
        }
        //--------------------------------------------------------------------------
        public void Rst()
        {
            //
            ftoe = false; Ltftoe = false;
            btoe = false; Ltbtoe = false;

            //Timer Reset.
            tFwdTimeOutTimer.Clear();
            tBwdTimeOutTimer.Clear();

            nReCntFwd = 0;
            nReCntBwd = 0;
        }
        //--------------------------------------------------------------------------
        public bool GetTimeOut()
        {
            if (IsSkip()) return false;

            //
            if (ftoe  ) return true;
            if (btoe  ) return true;
            if (Ltftoe) return true;
            if (Ltbtoe) return true;
            return false;
        }
        //--------------------------------------------------------------------------
        public bool IsSkip()
        {
            bool r1 = (m_yfwdId == SKIP4 || m_yfwdId == SKIP5);
            bool r2 = (m_ybwdId == SKIP4 || m_ybwdId == SKIP5);
            return (r1 && r2);
        }
        //--------------------------------------------------------------------------
        //run function
        public bool Out(int Cmd)  //Force.
        {
            int yf     ;
            int yb     ;
            int yfIndex;
            int ybIndex;

            //Inverse Out.
            if (iInv==1) {
                yf = GetybwdId(); yfIndex = GetybwdIndex();
                yb = GetyfwdId(); ybIndex = GetyfwdIndex();
                }
            else {
                yf = GetyfwdId(); yfIndex = GetyfwdIndex();
                yb = GetybwdId(); ybIndex = GetybwdIndex();
                }

            //Set.
            EnableLastOutCmdTOut = true;
            if (Cmd==1){
                LastOutCommand = 1;
                if (yf != SKIP4 || yf != SKIP5) Output(yfIndex , 1);
                if (yb != SKIP4 || yf != SKIP5) Output(ybIndex , 0);
                return dfy && !dby;
                }
            else {
                LastOutCommand = 0;
                if (yf != SKIP4 || yf != SKIP5) Output(yfIndex , 0);
                if (yb != SKIP4 || yb != SKIP5) Output(ybIndex , 1);
                return !dfy && dby;
                }
        }
        //--------------------------------------------------------------------------
        public bool Run(int Cmd)  //Check Time Out.
        {
            Out(Cmd);
            return (IsSkip() || Complete(Cmd));
        }
        //--------------------------------------------------------------------------
        //ID function
        //Set
        public void SetThreadApplyTime(int Time) { ThreadApplyTime = Time; }
        public void SetInv(int Inv) { iInv = Inv; }
        public void SetAllId(int xf, int xb, int yf, int yb)
        {
        }
        //--------------------------------------------------------------------------
        public void SetxfwdId(int n) 
        { 
            m_xfwdId  = n;
            for (int i = 0; i < IO._iNumOfX; i++) 
            {
                if (n == IO.XAdd[i]) { m_xfwdIndex = i; return; } 
            } 
        }
        //--------------------------------------------------------------------------
        public void SetxbwdId(int n) 
        { 
            m_xbwdId  = n; 
            for ( int i = 0 ; i < IO._iNumOfX; i++) { 
                if (n == IO.XAdd[i]) { m_xbwdIndex = i; return; } 
            } 
        }
        //--------------------------------------------------------------------------
        public void SetyfwdId(int n) 
        { 
            m_yfwdId  = n;
            for (int i = 0; i < IO._iNumOfY; i++)
            {
                if (n == IO.YAdd[i]) { m_yfwdIndex = i; return; } 
            } 
        }
        //--------------------------------------------------------------------------
        public void SetybwdId(int n) 
        { 
            m_ybwdId  = n;
            for (int i = 0; i < IO._iNumOfY; i++)
            {
                if (n == IO.YAdd[i]) { m_ybwdIndex = i; return; } 
            } 
        }
        //--------------------------------------------------------------------------
        //Get
        public int    GetThreadApplyTime() { return ThreadApplyTime; }
        public int    GetxfwdId()          { return m_xfwdId; }
        public int    GetxbwdId()          { return m_xbwdId; }
        public int    GetyfwdId()          { return m_yfwdId; }
        public int    GetybwdId()          { return m_ybwdId; }
        public int    GetxfwdIndex()       { return m_xfwdIndex; }
        public int    GetxbwdIndex()       { return m_xbwdIndex; }
        public int    GetyfwdIndex()       { return m_yfwdIndex; }
        public int    GetybwdIndex()       { return m_ybwdIndex; }
        public int    GetInv()             { return iInv       ; }
        //--------------------------------------------------------------------------
        //Set function
            //Set.
        public void   SetFwdOnDelayTime        (int data ) { nFwdOnDelayTime = data; }
        public void   SetBwdOnDelayTime        (int data ) { nBwdOnDelayTime = data; }
        public void   SetFwdTimeOutDelayTime   (int data ) { nFwdTimeOutDelayTime = data; }
        public void   SetBwdTimeOutDelayTime   (int data ) { nBwdTimeOutDelayTime = data; }
        public void   SetApplyTimeout          (bool data) { ApplyTimeout = data; }
        public void   SetApplyOutComplete      (bool data) { ApplyOutComplete = data; }
        //--------------------------------------------------------------------------
            //Get.
        public double GetFwdOnDelayTime()      { return nFwdOnDelayTime; }
        public double GetBwdOnDelayTime()      { return nBwdOnDelayTime; }
        public double GetFwdTimeOutDelayTime() { return nFwdTimeOutDelayTime; }
        public double GetBwdTimeOutDelayTime() { return nBwdTimeOutDelayTime; }
        public bool   GetApplyTimeout()        { return ApplyTimeout; }
        public bool   GetApplyOutComplete()    { return ApplyOutComplete; }
        
        //--------------------------------------------------------------------------
        //Read/Write Para.
        public void  Load(bool IsLoad , int Act , string Path)
        {
            //Local Var.
            string sName = string.Format("ACTUATOR({0,3:000})", Act + 1);
            
            if (IsLoad)
            {
                m_xfwdId = 0;
                m_xbwdId = 0;
                m_yfwdId = 0;
                m_ybwdId = 0;

                Name                 = UserINI.fn_Load("Name"               , sName, sName   , Path);
                Comt                 = UserINI.fn_Load("Comment"            , sName, Comt    , Path);
                m_xfwdId             = UserINI.fn_Load("xFwdID"             , sName, m_xfwdId, Path);
                m_xbwdId             = UserINI.fn_Load("xBwdID"             , sName, m_xbwdId, Path);
                m_yfwdId             = UserINI.fn_Load("yFwdID"             , sName, m_yfwdId, Path);
                m_ybwdId             = UserINI.fn_Load("yBwdID"             , sName, m_ybwdId, Path);
                ApplyTimeout         = UserINI.fn_Load("ApplyTimeout"       , sName, false   , Path);
                nFwdTimeOutDelayTime = UserINI.fn_Load("FwdTimeOutDelayTime", sName, 0       , Path);
                nBwdTimeOutDelayTime = UserINI.fn_Load("BwdTimeOutDelayTime", sName, 0       , Path);
                nFwdOnDelayTime      = UserINI.fn_Load("FwdOnDelayTime"     , sName, 0       , Path);
                nBwdOnDelayTime      = UserINI.fn_Load("BwdOnDelayTime"     , sName, 0       , Path);
                
                ThreadApplyTime      = UserINI.fn_Load("ThreadApplyTime"    , sName, 0       , Path);
                ApplyOutComplete     = UserINI.fn_Load("ApplyOutComplete"   , sName, false   , Path);
                iInv                 = UserINI.fn_Load("Inv"                , sName, 0       , Path);

                SetxfwdId(m_xfwdId);
                SetxbwdId(m_xbwdId);
                SetyfwdId(m_yfwdId);
                SetybwdId(m_ybwdId);
            }
            else
            {

                UserINI.fn_Save("Name"               , sName, Name                 , Path);
                UserINI.fn_Save("Comment"            , sName, Comt                 , Path);
                UserINI.fn_Save("xFwdID"             , sName, m_xfwdId             , Path);
                UserINI.fn_Save("xBwdID"             , sName, m_xbwdId             , Path);
                UserINI.fn_Save("yFwdID"             , sName, m_yfwdId             , Path);
                UserINI.fn_Save("yBwdID"             , sName, m_ybwdId             , Path);
                UserINI.fn_Save("ApplyTimeout"       , sName, ApplyTimeout         , Path);
                UserINI.fn_Save("FwdTimeOutDelayTime", sName, nFwdTimeOutDelayTime , Path);
                UserINI.fn_Save("BwdTimeOutDelayTime", sName, nBwdTimeOutDelayTime , Path);
                UserINI.fn_Save("FwdOnDelayTime"     , sName, nFwdOnDelayTime      , Path);
                UserINI.fn_Save("BwdOnDelayTime"     , sName, nBwdOnDelayTime      , Path);
                
                UserINI.fn_Save("ThreadApplyTime"    , sName, ThreadApplyTime      , Path);
                UserINI.fn_Save("ApplyOutComplete"   , sName, ApplyOutComplete     , Path);
                UserINI.fn_Save("Inv"                , sName, iInv                 , Path);

                SetxfwdId(m_xfwdId);
                SetxbwdId(m_xbwdId);
                SetyfwdId(m_yfwdId);
                SetybwdId(m_ybwdId);
            }
        }

    }


    /***************************************************************************/
    /* Class: TSysActuator                                                     */
    /* Create:                                                                 */
    /* Developer:                                                              */
    /* Note:                                                                   */
    /***************************************************************************/

    public class TSysActuator
    {
        //Timer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        TOnDelayTimer m_ChngActrOnTimer = new TOnDelayTimer();
        int           m_iNumOfACT;

        //Vars.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //private:   /* Member Var.             */
        TActuator[]  Actuator;
        //protected: /* Inheritable Vars.        */

        //public:    /* Direct Accessable Vars.  */
        public bool[] m_bRptActr;
        public string m_sFNameACT    ;
        public int    m_iChngActrDlay;
		public int    m_iErrFNo      ;
        public int    m_iManFNo      ;
        public bool   m_bRptActrIng  ;
        public int    m_iRptCmd      ;
            
        //Indexer.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public TActuator this[int iActNo]
        {
            get
            {
                if (iActNo < 0 | iActNo >= m_iNumOfACT) return null;
                return Actuator[iActNo];
            }
            set
            {
                if (iActNo < 0 | iActNo >= m_iNumOfACT) return;
                Actuator[iActNo] = value;
            }
        }
        //Property.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public string _sFNameACT
        {
            get { return m_sFNameACT; }
            set { m_sFNameACT = value; }
        }
        public int _iNumOfACT
        {
            get { return m_iNumOfACT; }
        }
        public int _iErrFNo
        {
            get { return m_iErrFNo; }
            set { m_iErrFNo = value; }
        }

        //Member Class 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public TSysActuator()
        {
            //
            Init(0, (int)EN_MAN_LIST.MAN_0200);
        }
        //~TSysActuator() { }

        //--------------------------------------------------------------------------
        //Init.
        public void  Init (int iErrNo, int iManNo)
        {
            string Name , Comt;
            m_iNumOfACT = (int)EN_ACTR_LIST.EndOfId;
            Actuator    = new TActuator[m_iNumOfACT];
            m_bRptActr  = new bool     [m_iNumOfACT];

	        for (int i = 0 ; i < m_iNumOfACT ; i++) 
            {
                Actuator[i] = new TActuator();
            }
            for (int i = 0 ; i < m_iNumOfACT ; i++) 
            {
                Name = string.Format("ACTUATOR{0,3:000}",i);   
                Comt = string.Format("COMMENT {0,3:000}",i);   
                Actuator[i].Init       (       );
                Actuator[i].SetName    (Name   );
                Actuator[i].SetComt    (Comt   );
                Actuator[i].SetxfwdId  (0      );
                Actuator[i].SetxbwdId  (0      );
                Actuator[i].SetyfwdId  (0      );
                Actuator[i].SetybwdId  (0      );
                Actuator[i].SetManNo   (iManNo + i);
                Actuator[i].SetErrNo   (iErrNo + i);
                Actuator[i].SetRetryCnt(0      );
                
                m_bRptActr[i] = false;
            }
           
            m_iErrFNo       = iErrNo   ;
            m_bRptActrIng   = false;
            m_iChngActrDlay = 0;
            
            //Load(true);
        }
        //--------------------------------------------------------------------------
        public void  fn_Reset() 
        { 
            for (int i = 0 ; i < m_iNumOfACT ; i++) Rst(i); 
        }
        //--------------------------------------------------------------------------
        //Normal Control.
        public bool  Out        (int aNum , int Act                                      ) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return false; 
            return Actuator[aNum].Out     (Act              ); 
        }
        //--------------------------------------------------------------------------
        public bool  Trg (int aNum , int Act) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return false; 
            return Actuator[aNum].Trg     (Act              ); 
        }
        //--------------------------------------------------------------------------
        public void  Rst(int aNum) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return  ;        
            Actuator[aNum].Rst     (                 ); 
        }
        //--------------------------------------------------------------------------
        public int Err (int aNum) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return 0; 
            return Actuator[aNum].Err     ( ); 
        }
        //--------------------------------------------------------------------------
        public bool   Complete   (int aNum , int Act) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return false; 
            return Actuator[aNum].Complete(Act); 
        }
        //--------------------------------------------------------------------------
        public bool   Complete   (int aNum , int Act , int xFwd , int xBwd                ) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return false; 
            return Actuator[aNum].Complete(Act , xFwd , xBwd); 
        }
        //--------------------------------------------------------------------------
        //Cylinder Control.
        public int    GetLastCmd(int aNum          ) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return 0; 
            return Actuator[aNum].GetLastCmd(   ); 
        }
        //--------------------------------------------------------------------------
        public bool   Getdfx    (int aNum          ) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return false; 
            return Actuator[aNum].dfx            ; 
        }
        //--------------------------------------------------------------------------
        public bool Getdbx(int aNum) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return false; 
            return Actuator[aNum].dbx ; 
        }
        //--------------------------------------------------------------------------
        public bool GetCylStat(int aNum , int Act) 
        { 
            if ((aNum < 0) || (aNum >= m_iNumOfACT)) return false; 
            return Actuator[aNum].Complete  (Act); 
        }
        //---------------------------------------------------------------------------
        public bool MoveCyl(EN_ACTR_LIST aNum, int Act)
        {
            return MoveCyl((int)aNum, Act);
        }
        //--------------------------------------------------------------------------
        public bool MoveCyl(int aNum , int Act)
        {
            //Run Actuator.
	        if(aNum < 0 || aNum >= m_iNumOfACT ) return false;
            if(!SEQ.fn_CheckDstbActr(aNum, Act)) return false;
            return Actuator[aNum].Run(Act);
        }
        //--------------------------------------------------------------------------
        public void SetRpt(int aNum , bool Flag, int iDelay = 0)
        {
	        m_iChngActrDlay = iDelay;
            if (!Flag)  m_bRptActrIng = false;
	        if (aNum == -1) 
            {
		        for (int n = 0 ; n < m_iNumOfACT; n++) m_bRptActr[n] = Flag;
		    }
	        else 
            {
		        if(aNum<0 || aNum>=m_iNumOfACT) return;
		        m_bRptActr[aNum] = Flag;
		        m_iRptCmd = 1;
		    }
        }
        //--------------------------------------------------------------------------
        //Update.
        public void fn_Update()
        {
	        bool isOk = true;
	        for (int i = 0 ; i < m_iNumOfACT ; i++) {
		        Actuator[i].Update();
		        
                //Check Actuator Status.
		        if (Err(i)==1) m_bRptActr[i] = false;

		        //Changing Timer.
		        if (!m_bRptActrIng) continue;
		        if (m_bRptActr[i]) {
                    if(!SEQ.fn_CheckDstbActr(i, m_iRptCmd)) { m_bRptActrIng = false; continue; }
			        MoveCyl(i , m_iRptCmd);
			        if(!Complete(i , m_iRptCmd)) isOk = false;
		        }
	        }

	        m_ChngActrOnTimer.OnDelay(isOk , m_iChngActrDlay);
	        if(m_ChngActrOnTimer.Out)
	        {
		        m_ChngActrOnTimer.Clear();
                if (m_iRptCmd == (int)EN_ACTR_CMD.Fwd) m_iRptCmd = (int)EN_ACTR_CMD.Bwd;
		        else                                   m_iRptCmd = (int)EN_ACTR_CMD.Fwd;
	        }
        }
        //--------------------------------------------------------------------------
        //Read/Write Para.
        public void fn_Load(bool IsLoad)
        {
            //Local Var.
            string m_strPath, sPath;
            
            m_strPath = UserFile.fn_GetExePath();

            //File Path
            sPath = m_strPath + "SYSTEM\\Actuator.ini";

            if (IsLoad) 
            {
                if (!UserFile.fn_CheckFileExist(sPath)) return;
            }
            for (int i = 0 ; i < m_iNumOfACT  ; i++) 
            {
                Actuator[i].Load(IsLoad , i , sPath);
            }

            //JUNG/201012
            fn_SetRetryCnt((int)EN_ACTR_LIST.aSpdl_LensCovr, 2);
            fn_SetRetryCnt((int)EN_ACTR_LIST.aspdl_IR      , 2);

        }
        //--------------------------------------------------------------------------
        public int fn_GetRetryCnt(int Actr)
        {
            return Actuator[Actr].GetRetryCnt();
        }
        public void fn_SetRetryCnt(int Actr, int Cnt)
        {
            Actuator[Actr].SetRetryCnt(Cnt);
        }
        //-------------------------------------------------------------------------------------------------
        public int fn_GetRetryCntFwd(int Actr)
        {
            return Actuator[Actr].GetRetryCntFwd();
        }
        //-------------------------------------------------------------------------------------------------
        public int fn_GetRetryCntBwd(int Actr)
        {
            return Actuator[Actr].GetRetryCntBwd();
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_IsRetryStateFwd(int Actr)
        {
            return Actuator[Actr].GetRetryCntFwd() > 0 && Actuator[Actr].GetRetryCnt() > 0;
        }
        //-------------------------------------------------------------------------------------------------
        public bool fn_IsRetryStateBwd(int Actr)
        {
            return Actuator[Actr].GetRetryCntBwd() > 0 && Actuator[Actr].GetRetryCnt() > 0;
        }

        //--------------------------------------------------------------------------
        public int  ManNoActr        (int Actr) 
        {
            if(Actr < 0           ) return -1; 
            if(Actr >= m_iNumOfACT) return -1; 
            return Actuator[Actr].m_iManNo   ;
        }
        //--------------------------------------------------------------------------
        public int ErrNoActr(int Actr) 
        { 
            if(Actr < 0           ) return -1; 
            if(Actr >= m_iNumOfACT) return -1; 
            return Actuator[Actr].m_iErrNo; 
        }

    }


}
