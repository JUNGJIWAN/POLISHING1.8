using System;
using System.Collections;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserFunction;

namespace WaferPolishingSystem.Vision
{
    /**
    @class	LightControllerManager
    @brief	RS232 통신 프로토콜의 조명 컨트롤러 관리자
    @remark	
     - 
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/10/6  12:05
    */
    public class LightControllerManager
    {
        SerialPort sPort;
        byte[] _CRLF = new byte[2];
        const byte _ACK = 0x06;
        const byte _NAK = 0x15;
        public int _BrightValue = 0;
        public int Channel = 1;
        public bool _bConnect = false;

        private int _SendTick = 0;
        private int _RecvTick = 0;

        string _Port = "";
        const int _Baudrate1 = 9600;
        const int _Baudrate2 = 19200;
        const int _Databit = 8;

        const int _MaxSerialBufferSize = 2048;

        byte[] _Buffer = new byte[_MaxSerialBufferSize];
        int _TotalReciveCount = 0;
        
        Parity _Parity = Parity.None;
        StopBits _Stopbit = StopBits.One;

        Queue _queue = new Queue();

        DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        public int Bright
        {
            get
            {
                //GetValue(Channel);
                return _BrightValue;
            }
            set
            {   
                _BrightValue = value;
                SetValue(Channel, _BrightValue);
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     puiblic LightControllerManager()
        @brief	클래스 초기화.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:06
        */
        public LightControllerManager()
        {
            _CRLF[0] = 0x0d;
            _CRLF[1] = 0x0a;
            _DispatcherTimer.Tick += Timer_CheckRecive;
            _DispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
        }
        
        //---------------------------------------------------------------------------
        /**
        @fn     public bool Serial_Conn(string port, int ConnMode = 0)
        @brief	Serial 연결.
        @return	bool 연결 성공 여부.
        @param	string port     : Serial Port Name
        @param  string ConnMode : baudrate Index 0 - 9600, 1 - 19200
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:31
        */
        public bool Serial_Conn(string port, int ConnMode = 0)
        {
            _Port = port;
            if (ConnMode == 0)
                fn_Serial_Init(_Port, _Baudrate1, _Databit, _Parity, _Stopbit);
            else if (ConnMode == 1)
                fn_Serial_Init(_Port, _Baudrate2, _Databit, _Parity, _Stopbit);
            else
            {
                fn_WriteLog("SerialConn : Conn Fail. (Invalid Conn Mode.)", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }

            if (sPort == null)
            {
                fn_WriteLog("SerialConn : Conn Fail. (Serial Port is Null.)", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }
            else
            {
                _bConnect = true;
                fn_WriteLog("SerialConn : Conn Succ.", UserEnum.EN_LOG_TYPE.ltVision);
                return true;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void fn_Serial_Init(string port, int baudrate, int Databit, Parity parity, StopBits stopbit)
        @brief	Serial 통신 초기화.
        @return	void
        @param	string   port     : Serial Port
        @param  int      baudrate : Serial Baudrate
        @param  int      Databit  : Serial Databit
        @param  Parity   parity   : Serial parity
        @param  StopBits stopbit  : Serial Stopbit
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:33
        */
        private void fn_Serial_Init(string port, int baudrate, int Databit, Parity parity, StopBits stopbit)
        {
            try
            {
                if (sPort == null)
                {
                    sPort = new SerialPort();
                    sPort.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived);
                    sPort.PortName = port;
                    sPort.BaudRate = baudrate;
                    sPort.DataBits = Databit;
                    sPort.Parity = parity;
                    sPort.StopBits = stopbit;

                    sPort.Open();
                    _DispatcherTimer.Start();
                    fn_WriteLog("SerialConn_Init : Init Succ.", UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                fn_WriteLog("SerialConn_Init : Init Fail. (" + ex.Message + ".)", UserEnum.EN_LOG_TYPE.ltVision);
                sPort.Close();
                sPort = null;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     public bool Serial_DisConn()
        @brief	Serial DisConnection
        @return	bool : DisConnection 성공 여부.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:39
        */
        public bool Serial_DisConn()
        {
            try
            {
                if (sPort != null)
                {
                    if (sPort.IsOpen)
                    {
                        sPort.DataReceived -= Serial_DataReceived;
                        sPort.Close();
                        sPort = null;
                        fn_WriteLog("SerialDisConn : Succ.", UserEnum.EN_LOG_TYPE.ltVision);
                        _DispatcherTimer.Stop();
                        _bConnect = false;
                        return true;
                    }
                }
                fn_WriteLog("SerialDisConn : Fail. (SerialPort Is Null.)", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }
            catch (System.Exception ex)
            {
                fn_WriteLog("SerialDisConn : Fail. (" + ex.Message + ".)", UserEnum.EN_LOG_TYPE.ltVision);
                return false;
            }
        }

        //---------------------------------------------------------------------------
        /**
        @fn     private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        @brief	Serial Data Receive Callback
        @return	void
        @param	object                      sender
        @param  SerialDataReceivedEventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/5/7  16:39
        */
        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            int nReciveLength = sPort.BytesToRead;
            if (nReciveLength != 0)
            {
                byte[] buff = new byte[nReciveLength];
                sPort.Read(buff, 0, buff.Length);
                _RecvTick = Environment.TickCount;
                // Data 수신시 연결됨으로 간주.
                _bConnect = true;
                
                // Write to _BrightValue
                if (buff.Length == 1)
                {
                    if (buff[0] == _ACK)
                    {
                        // OK
                        //Console.WriteLine("Ack");
                        //fn_WriteLog("SerialDataRecive : Ack", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                    else if (buff[0] == _NAK)
                    {
                        // NG
                        Console.WriteLine("Nck");
                        //fn_WriteLog("SerialDataRecive : Nck", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                }
                else
                {
                    // 정상 수신시
                    // 데이터 복사 하고, Receive Count 초기화.
                    if(buff[buff.Length-2] == _CRLF[0] && buff[buff.Length - 1] == _CRLF[1])
                    {
                        fn_WriteLog("SerialDataRecive : Receive Data.", UserEnum.EN_LOG_TYPE.ltVision);
                        Array.Copy(buff, 0, _Buffer, _TotalReciveCount, buff.Length);
                        _TotalReciveCount = 0;
                        string strValue = Encoding.ASCII.GetString(_Buffer);
                        if (int.TryParse(strValue, out _BrightValue))
                        {
                            Bright = _BrightValue;
                        }
                    }
                    // 종료 바이트가 없을 때에는
                    // 2048바이트의 버퍼에 복사 하고, Receive Count 증가.
                    else
                    {
                        fn_WriteLog("SerialDataRecive : Appand Data.", UserEnum.EN_LOG_TYPE.ltVision);
                        if (_TotalReciveCount < _MaxSerialBufferSize)
                        {
                            Array.Copy(buff, 0, _Buffer, _TotalReciveCount, buff.Length);
                            _TotalReciveCount += _Buffer.Length;
                        }
                        else
                            _TotalReciveCount = 0;
                    }
                }
                _RecvTick = 0;
                _SendTick = 0;
            }

        }

        //---------------------------------------------------------------------------
        // Command
        //---------------------------------------------------------------------------
        /**
        @fn     public void SetValue(int channel, int value)
        @brief	조명값 셋팅
        @return	void
        @param	int channel : 대상 채널 (1,2)
        @param	int value   : 목표 값 (0-255)
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:08
        */
        public void SetValue(int channel, int value)
        {
            string strMsg = "N" + channel.ToString()+value.ToString("000");
            if (sPort != null)
            {
                if(sPort.IsOpen)
                    WriteValue(strMsg);
            }
        }
        //---------------------------------------------------------------------------
        /**
        @fn     public void SetOff(int channel)
        @brief	조명 off
        @return	void
        @param	int channel : 대상 채널 (1,2)
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:09
        */
        public void SetOff(int channel)
        {
            string strMsg = "E" + channel.ToString();
            if (sPort != null)
            {
                if (sPort.IsOpen)
                    WriteValue(strMsg);
            }
        }
        //---------------------------------------------------------------------------
        /**
        @fn     public void GetValue(int channel)
        @brief	조명값 읽기
        @return	void
        @param	int channel : 대상 채널 (1,2)
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:10
        */
        public void GetValue(int channel)
        {
            _BrightValue = -1;
            string strMsg = "R" + channel.ToString();
            if (sPort != null)
            {
                if (sPort.IsOpen)
                    WriteValue(strMsg);
            }
        }
        //---------------------------------------------------------------------------
        /**
        @fn     private void WriteValue(string strMsg)
        @brief	RS232 통신으로 데이터 전송.
        @return	void
        @param	string strMsg : 전송 데이터.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  12:11
        */
        private void WriteValue(string strMsg)
        {
            byte[] bufMsg = Encoding.ASCII.GetBytes(strMsg);
            byte[] buff = new byte[bufMsg.Length + _CRLF.Length];

            Array.Copy(bufMsg, 0, buff, 0, bufMsg.Length);
            Array.Copy(_CRLF, 0, buff, bufMsg.Length, _CRLF.Length);
            sPort.Write(buff, 0, buff.Length);
            _SendTick = Environment.TickCount;
        }
        //---------------------------------------------------------------------------
        /**
        @fn     private void Timer_CheckRecive(object sender, EventArgs e)
        @brief	연결 상태 확인하는 타이머
        @return	void
        @param	obejct sender
        @param	EventArgs e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  11:30
        */
        private void Timer_CheckRecive(object sender, EventArgs e)
        {
            
            if (_SendTick != 0)
            {
                if (_RecvTick == 0)
                    _bConnect = false;
            }
        }
        //---------------------------------------------------------------------------
    }
}
