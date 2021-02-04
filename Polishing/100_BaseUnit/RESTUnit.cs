using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using static WaferPolishingSystem.FormMain;
using WaferPolishingSystem.Define;

namespace WaferPolishingSystem.BaseUnit
{
    public class RESTUnit
    {
        //Var
        int    m_nSeqStep;
        bool   m_bDrngConnect, m_bDrngVersion, m_bDrngRFInfo;
        bool   m_bReqConnect , m_bReqVersion , m_bReqRFInfo ;
        bool   m_bUpdateData ;
        bool   m_bDataError  ;
        string m_sReqPlateId; 

        public  static string   DISCONNECTED = "Disconnected";
        private static RESTUnit m_Instance   = (RESTUnit)null;
        
        private string apiUrl = "http://localhost:55000/";  //ADDRESS
        private HttpClient client;

        public PlateRFIDInfo RcvPlateRFIDInfo;

        private string m_sVersion = string.Empty;
        private bool   m_bConnect;


        //---------------------------------------------------------------------------
        public string _sVersion     { get { return m_sVersion    ; } }
        public bool   _bConnect     { get { return m_bConnect    ; } }
        public bool   _bDrngConnect { get { return m_bDrngConnect; } }
        public bool   _bDrngVersion { get { return m_bDrngVersion; } }
        public bool   _bDrngRFInfo  { get { return m_bDrngRFInfo ; } }
        public bool   _bUpdateData  { get { return m_bUpdateData ; } set { m_bUpdateData = value; } }
        public bool   _bDataError   { get { return m_bDataError  ; } }
        


        //---------------------------------------------------------------------------
        public RESTUnit()
        {
            //
            m_nSeqStep = 0;

            m_bDrngConnect = false; 
            m_bDrngVersion = false; 
            m_bDrngRFInfo  = false;
            m_bReqConnect  = false;  
            m_bReqVersion  = false; 
            m_bReqRFInfo   = false;
            m_bUpdateData  = false;
            m_bDataError   = false;

            m_sReqPlateId  = string.Empty;

            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(this.apiUrl);
            this.client.Timeout = TimeSpan.FromSeconds(5.0);

            RcvPlateRFIDInfo = new PlateRFIDInfo();
            RcvPlateRFIDInfo.Init();

            //
            m_Instance = this;
        }
        //---------------------------------------------------------------------------
        public static RESTUnit Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new RESTUnit();
            
                return m_Instance;
            }
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_Init()
        {
            RcvPlateRFIDInfo.Init();
        }
        //---------------------------------------------------------------------------
        public void fn_SetURL(string url, bool reset = false)
        {
            try
            {
                if (reset)
                {
                    //if (m_Instance._bConnect) return; 
                    this.client             = new HttpClient();
                    this.client.BaseAddress = new Uri(url);
                    this.client.Timeout     = TimeSpan.FromSeconds(5.0);
                }
                else
                {
                    this.client.BaseAddress = new Uri(url);
                }

                
            }
            catch (System.Exception ex)
            {
                UserMsgBox.Error("[URL ERROR] " + ex.Message);
            }
            
        }
        //---------------------------------------------------------------------------
        private string REST_GET(string requestUri)
        {
            try
            {
                return this.client.GetStringAsync(requestUri).Result;
            }
            catch
            {
                return DISCONNECTED;
            }
        }
        //---------------------------------------------------------------------------
        private string REST_POST(string requestUri, string param)
        {
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject((object)param), Encoding.UTF8, "application/json");
            return this.client.PostAsync(requestUri, (HttpContent)stringContent).Result.ReasonPhrase;
        }
        //---------------------------------------------------------------------------
        public bool fn_GetRFIDInfo(string id)
        {
            try
            {
                //
                m_bUpdateData = false;
                m_bDataError  = false;
                
                //JUNG/2012015
                fn_Init();

                string str = Instance.REST_GET("/api/PlateRFIDInfo/" + id);
                
                if (str == DISCONNECTED) return false ;
                if (str == "")
                {
                    m_bDataError = true;
                    return false;
                }

                this.RcvPlateRFIDInfo = (PlateRFIDInfo)JsonConvert.DeserializeObject(str, typeof(PlateRFIDInfo));
                this.RcvPlateRFIDInfo.sRcvTime = DateTime.Now.ToString("HH:mm:ss");
                
                m_bUpdateData = true;

                return true;
            }
            catch
            {
                return false; 
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_WriteLog(string log)
        {//JUNG/201216
            UserFunction.fn_WriteLog(log, UserEnum.EN_LOG_TYPE.ltREST);
        }
        //---------------------------------------------------------------------------
        public bool fn_Connect()
        {
            fn_WriteLog("Request Connect");

            m_bConnect = (Instance.REST_GET("api/connect") != DISCONNECTED);

            return m_bConnect; 
        }
        //---------------------------------------------------------------------------
        private string fn_GetVersion()
        {
            fn_WriteLog("Request Version");

            m_sVersion = Instance.REST_GET("api/version");

            return m_sVersion; 
        }
        //---------------------------------------------------------------------------
        public void fn_ReqConnection()
        {
            m_bReqConnect = true ;
        }
        //---------------------------------------------------------------------------
        public void fn_ReqVersion()
        {
            m_bReqVersion = true;
        }
        //---------------------------------------------------------------------------
        public void fn_ReqRFInfo(string id)
        {
            m_sReqPlateId = id; 
            m_bReqRFInfo  = true;

            fn_WriteLog("Request RF Info - Id :" + id);
        }

        //---------------------------------------------------------------------------
        public bool fn_Upate()
        {
            //
            if (FM.m_stMasterOpt.nUseRESTApi == 0) return true; 
            if (SEQ._bRun                        ) return true;

            //Decide Step
            if (m_nSeqStep == 0)
            {
				//Step Condition
				bool isConConnect  = m_bReqConnect; 
				bool isConVersion  = m_bReqVersion; 
				bool isConRFInfo   = m_bReqRFInfo ;  
				
				//Clear Var.
				m_bDrngConnect  = false;
				m_bDrngVersion  = false;
				m_bDrngRFInfo   = false;

                //Check Sequence Stop
                if (SEQ._bRun)
                {
                    m_bDrngConnect = false;
                    m_bDrngVersion = false;
                    m_bDrngRFInfo  = false;

                    m_bReqConnect  = false;
                    m_bReqVersion  = false;
                    m_bReqRFInfo   = false;

                    return false;
                }

				//
				if (isConConnect) { m_bDrngConnect   = true; m_nSeqStep = 100 ; goto __GOTO_CYCLE__; }
				if (isConVersion) { m_bDrngVersion   = true; m_nSeqStep = 200 ; goto __GOTO_CYCLE__; }
				if (isConRFInfo ) { m_bDrngRFInfo    = true; m_nSeqStep = 300 ; goto __GOTO_CYCLE__; }

			}

            //Cycle Start
            __GOTO_CYCLE__:

            //Cycle
            switch (m_nSeqStep)
            {
                default:
                    m_nSeqStep = 0;
                    break;

                //Connection
                case 100:
                    m_bReqConnect = false;

                    //Connection
                    fn_Connect();
                    
                    m_bDrngConnect = false ;
                    
                    m_nSeqStep = 0;
                    return true;

                //Get Version
                case 200:
                    m_bReqVersion = false;

                    if(m_bConnect)
                    {
                        fn_GetVersion();
                    }

                    m_bDrngVersion = false;

                    m_nSeqStep = 0;
                    return true;

                //Get RF Info
                case 300:
                    m_bReqConnect = false;
                    m_bUpdateData = false;
                    m_bDataError  = false;
                    m_bReqRFInfo  = false;

                    //
                    if (m_sReqPlateId != string.Empty)
                    { 
                        fn_GetRFIDInfo(m_sReqPlateId);
                    }

                    m_sReqPlateId = string.Empty;

                    m_bDrngConnect = false;

                    m_nSeqStep = 0;
                    return true;

            }
            return true; 
        }
    }
}
