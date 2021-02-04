using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows.Media;
using static WaferPolishingSystem.FormMain;
using System.Windows.Forms;

namespace WaferPolishingSystem.BaseUnit
{
    public class RFIDUnit
    {
        //Var
        int  m_nSeqStep;
        bool m_bDrngConnect, m_bDrngRead, m_bDrngWrite;
        bool m_bReqConnect , m_bReqRead , m_bReqWrite ;
        bool m_bUpdateData;
        bool m_bDataError ;

        //const string RF_ADDRESS = "192.168.0.79";
        string RF_ADDRESS;
        const int RF_PORT = 33000;
        const string RF_CMD_PORTOPEN = "CI_01_11_0000_004_028_01_01_00\r\n";
        const string RF_CMD_READ     = "RD_01_00010_0032\r\n";
        const string RF_CMD_WRITE    = "WR_01_00010_0032_";

        string   data_dte104 ;
        string   error_dte104;
        string[] diag_dte104 = new string[4];
        string[] split_data  ;

        string   m_sState    ;
        string   m_sReadData ;

        bool     m_bConnect  ;


        TcpClient     client;
        NetworkStream clientStream;

        //declare separator to parse answer strings
        char[] delimiterChars = { '_', '\r', '\n' };

        //
        public bool   _bConnect     { get { return m_bConnect      ; } }
        public bool   _bDrngConnect { get { return m_bDrngConnect  ; } }
        public bool   _bDrngRead    { get { return m_bDrngRead     ; } }
        public bool   _bDrngWrite   { get { return m_bDrngWrite    ; } }
        public bool   _bUpdateData  { get { return m_bUpdateData   ; } set { m_bUpdateData = value; } }
        public bool   _bDataError   { get { return m_bDataError    ; } }


        public string _sState       { get { return m_sState        ; } }
        public string _sReadData    { get { return m_sReadData     ; } }
        public string _sReadRFNo    { get { return fn_GetRFNo()    ; } }
                                    
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자
        public RFIDUnit()
        {
            //
            m_nSeqStep     = 0;
            m_bDrngConnect = false;
            m_bDrngRead    = false;
            m_bDrngWrite   = false;

            m_bReqConnect  = false; 
            m_bReqRead     = false;
            m_bReqWrite    = false;
            m_bUpdateData  = false;
            m_bDataError   = false;


            //
            m_sState    = string.Empty;
            m_sReadData = string.Empty;

            m_bConnect  = false;

            RF_ADDRESS  = string.Empty;

            for (int n = 0; n<4; n++)
            {
                diag_dte104[n] = string.Empty; 
            }

        }

        //---------------------------------------------------------------------------
        public void fn_SetIP(string ip)
        {
            RF_ADDRESS = ip;
        }
        //---------------------------------------------------------------------------
        public bool fn_Connect()
        {
            //
            if (m_bConnect) return true; 

            //
            try
            {
                if (RF_ADDRESS == string.Empty) RF_ADDRESS = "192.168.0.79";

                //Open connection to DTE104
                client = new TcpClient();
                client.Connect(IPAddress.Parse(RF_ADDRESS), RF_PORT); //content textBox1 = IP address of the DTE104, Port number for ASCii protocol fixed to 33000

                m_bConnect = fn_PortOpen();
            
            }
            catch(Exception ex)
            {
                m_bConnect = false;
                Console.WriteLine("RF Connection Error = " + ex.Message);
            }

            return m_bConnect; 

        }
        //---------------------------------------------------------------------------
        public bool fn_DisConnect()
        {
            client.Close();

            m_bConnect = false;

            return m_bConnect; 
        }
        //---------------------------------------------------------------------------
        private bool fn_PortOpen()
        {
            try
            {
                fn_ClearMsg();

                //set command string for port configuration IO-1
                data_dte104 = SendMessage(RF_CMD_PORTOPEN);

                //split answer by seperator : "_"
                split_data   = data_dte104.Split(delimiterChars);
                error_dte104 = split_data[2];
                data_dte104  = split_data[4];

                //check if a diagnosis event happen
                if (error_dte104 == "01")
                {
                    diag_dte104[0] = split_data[4].Substring(0, 4);
                    diag_dte104[1] = split_data[4].Substring(4, 4);
                    diag_dte104[2] = split_data[4].Substring(8, 4);
                    diag_dte104[3] = split_data[4].Substring(12, 4);
                    
                    m_sState = "State: " + diag_dte104[0] + "\t" + diag_dte104[1] + "\t" + diag_dte104[2] + "\t" + diag_dte104[4];
                    
                    Console.WriteLine("RF Port Open Error = " + m_sState);
                    return false;

                }
            }
            catch (Exception ex)
            {
                m_sState = "RF Port Open Error = " + ex.Message;
                Console.WriteLine(m_sState);
                return false;
            }

            return true;
        }

        //---------------------------------------------------------------------------
        private string SendMessage(string msg)
        {
            //NetworkStream 
            clientStream = client.GetStream();

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(msg);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            // Receive the TcpServer.response. 

            // Buffer to store the response bytes. 
            Byte[] data = new Byte[2048];

            // String to store the response ASCII representation. 
            string responseData = string.Empty;

            // Read the first batch of the TcpServer response bytes. 
            Int32 bytes = clientStream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            return responseData;
        }
        //---------------------------------------------------------------------------
        public bool fn_Read()
        {
            //
            if (!m_bConnect) return false; 

            fn_ClearMsg();
            m_sReadData = string.Empty;

            try
            {
                //prepare reading string
                data_dte104 = SendMessage(RF_CMD_READ);
                split_data  = data_dte104.Split(delimiterChars);

                error_dte104 = split_data[2];

                //check if a diagnosis event happen
                if (error_dte104 == "01")
                {
                    //read out diagnosis if error happen
                    data_dte104 = SendMessage("DI_01\r\n");
                    split_data = data_dte104.Split(delimiterChars);

                    m_sState  = "[Read Error] Message: " + split_data[4];
                    return false; 
                }
                else
                {
                    m_sReadData = split_data[5];
                    return true; 
                }

            }
            catch (System.Exception ex)
            {
                m_sState = "RF Read Error = " + ex.Message;
                Console.WriteLine(m_sState);
                return false;
            }
          
        }
        //---------------------------------------------------------------------------
        public bool fn_Write(string data)
        {
            //
            if (!m_bConnect) return false;

            fn_ClearMsg();

            //prepare writing string
            string sMsg = RF_CMD_WRITE + data.PadRight(32, '-') + "\r\n";
            data_dte104 = SendMessage(sMsg);
            split_data = data_dte104.Split(delimiterChars);

            error_dte104 = split_data[2];

            //check if a diagnosis event happen
            if (error_dte104 == "01")
            {
                //read out diagnosis if error happen
                data_dte104 = SendMessage("DI_01\r\n");
                split_data = data_dte104.Split(delimiterChars);

                m_sState = "[ERROR] State: " + split_data[4];

                return false; 
            }
            else
            {
                return true;
            }

        }
        //---------------------------------------------------------------------------
        private void fn_ClearMsg()
        {
            m_sState = string.Empty;
        }
        //---------------------------------------------------------------------------
        private string fn_GetRFNo()
        {
            //RF No Split
            string sNo = string.Empty;

            if(m_sReadData != "")
            {
                //자리수 Split
                sNo = m_sReadData.Substring(0,5);
            }

            return sNo;
        }
        //---------------------------------------------------------------------------
        public void fn_ReqConnect()
        {
            m_bReqConnect = true; 
        }
        //---------------------------------------------------------------------------
        public void fn_ReqRead()
        {
            //JUNG/201215
            REST.fn_Init();

            m_bReqRead    = true;
            m_bUpdateData = false;

        }
        //---------------------------------------------------------------------------
        public void fn_ReqWrite()
        {
            m_bReqWrite = true;
        }
        //---------------------------------------------------------------------------
        public bool fn_Upate()
        {
            //
            if (SEQ._bRun) return true;

            //Decide Step
            if (m_nSeqStep == 0)
            {
				//Step Condition
				bool isConConnect  = m_bReqConnect; 
				bool isConRead     = m_bReqRead   ; 
				bool isConWrite    = m_bReqWrite  ;  
				
				//Clear Var.
				m_bDrngConnect  = false;
				m_bDrngRead     = false;
				m_bDrngWrite    = false;

                //Check Sequence Stop
                if (SEQ._bRun)
                {
                    m_bReqConnect = false;
                    m_bReqRead    = false;
                    m_bReqWrite   = false;

                    m_bReqConnect = false;
                    m_bReqRead    = false;
                    m_bReqWrite   = false;

                    return false;
                }

				//
				if (isConConnect) { m_bDrngConnect   = true; m_nSeqStep = 100 ; goto __GOTO_CYCLE__; }
				if (isConRead   ) { m_bDrngRead      = true; m_nSeqStep = 200 ; goto __GOTO_CYCLE__; }
				if (isConWrite  ) { m_bDrngWrite     = true; m_nSeqStep = 300 ; goto __GOTO_CYCLE__; }

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

                //RFID Read
                case 200:
                    m_bReqRead    = false;
                    m_bUpdateData = false;
                    m_bDataError  = false;

                    //Read
                    if (fn_Read())
                    {
                        m_bUpdateData = true; 

                        if (FM.m_stMasterOpt.nUseRESTApi == 1)
                        {
                            m_nSeqStep++;
                            return false;
                        }
                    }
                    else
                    {
                        m_bDataError = true;
                    }

                    m_bDrngRead = false;

                    m_nSeqStep = 0;
                    return true;

                //REST API
                case 201:
                    if (!REST._bConnect) 
                    {
                        REST.fn_Connect();
                    }

                    if(REST._bConnect)
                    {
                        //REST.fn_GetRFIDInfo(_sReadRFNo);
                        REST.fn_ReqRFInfo(_sReadRFNo);
                    }

                    m_bDrngRead = false;

                    m_nSeqStep = 0;
                    return true;

                //Write
                case 300:
                    m_bReqWrite = false;

                    //

                    m_bDrngWrite = false;

                    m_nSeqStep = 0;
                    return true;

            }


            return true; 
        }
    }




}
