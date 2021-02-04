using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Samsung.PMC.Packet;
using Samsung.PMC.Packet.Body;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;
using WaferPolishingSystem.Define;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using static WaferPolishingSystem.Define.UserEnum;

namespace WaferPolishingSystem.BaseUnit
{

    public class ConsoleEQP
    {
        
        public struct SYSTEMTIME
        {
            public short Year;
            public short Month;
            public short DayOfWeek;
            public short Day;
            public short Hour;
            public short Minute;
            public short Second;
            public short Milliseconds;
        }

        [DllImport("kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);

        private   bool   m_bAlive;
        private   string pmcip;
        private   int    pmcport;
        private   TOnDelayTimer m_tAliveTimer = new TOnDelayTimer();
        private   string stime = string.Empty;

        private Thread thread         = null;
        private Thread polling_thread = null;
        private Thread proc_thread    = null;
        private Thread proc_Send      = null;

        private int timeScale = 1;
        private TcpClient   client = null;
        private TcpListener server = null;

        protected bool m_bPrepare     ;
        protected bool m_bLoadStart   ;
        protected bool m_bProcessStart;
        protected bool m_bUnloadStart ;

        //
        private Queue<pmcPacket> m_SendQue = new Queue<pmcPacket>();

        protected bool m_bUpdate;
        protected bool m_bUsePollLog;
        protected bool m_bDryRunMode;
        protected ListBox lbsnd = new ListBox();
        protected ListBox lbrcv = new ListBox();

        public Queue<pmcPacket> pollingQue = new Queue<pmcPacket>();
        public Queue<pmcPacket> procQue = new Queue<pmcPacket>();
        public double delayFactor = 1.0;

        public int m_nLoadPortState    = 0;
        public int m_nUnloadPortState  = 0;
        public int m_nProcessState     = 0;

        protected State curState;

        protected Data data  = new Data("job_progress", DATA_GROUP.Monitoring, DATA_TYPE.Integer, 0);
        protected Data data2 = new Data("TEST_data", DATA_GROUP.Monitoring, DATA_TYPE.String, "EQP.Simulator.TEST_DATA");
        protected Data data3 = new Data("TEST_data2", DATA_GROUP.Monitoring, DATA_TYPE.Double, 1234.1235);
        protected Data data4 = new Data("TEST_data3", DATA_GROUP.Monitoring, DATA_TYPE.Boolean, true);

        public int _LoadPortState    { get { return m_nLoadPortState; } }
        public int _UnloadPortState  { get { return m_nUnloadPortState; } }
        public int _ProcessState     { get { return m_nProcessState; } }



        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public ConsoleEQP()
        {
            m_bAlive = false;
            m_tAliveTimer.Clear();

            m_bUpdate     = false;
            m_bAlive      = false;
            pmcip         = string.Empty;
            pmcport       = 0;
            m_bUsePollLog = true ;
            m_bDryRunMode = false;

            lbsnd = null;
            lbrcv = null;

            curState = new State();

            m_bPrepare       = false;
            m_bLoadStart     = false;
            m_bProcessStart  = false;
            m_bUnloadStart   = false;

            InitState();
        }
        

        protected void printf(string str)
        {
            Console.WriteLine(str);
        }
        protected void WriteOperationLog(string op, bool isSync = false)
        {
            double localFactor = delayFactor;
            if (isSync) localFactor = 0.25;

            if (op == "Process")
            {
                int progress = 0;
                for (int i = 0; i < 50; i++)
                {
                    printf($"\t{op} : " + (i + 1) + " / " + 50);
                    Thread.Sleep((int)(500.0 * localFactor));
                    progress += 2;
                    data.setValue(progress);
                }
                data.setValue((int)100);
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    printf($"\t{op} : " + (i + 1) + " / " + 5);
                    Thread.Sleep((int)(500.0 * localFactor));
                }
            }
            printf($"\t{op} End");
        }
        //---------------------------------------------------------------------------
        protected void printf(string str, bool send = false)
        {
            Console.WriteLine(str);

            fn_WriteLog(str, Define.UserEnum.EN_LOG_TYPE.ltPMC);

            //Display Window
            fn_Display(send, str);
        }
        //---------------------------------------------------------------------------
        protected void SetPollingLog(bool set)
        {
            m_bUsePollLog = set; 
        }
        //---------------------------------------------------------------------------
        private void fn_Display(bool send, string str)
        {
            stime = DateTime.Now.ToString("[HH:mm:ss] ");

            try
            {
                if (m_bUpdate)
                {
                    if (send)
                    {
                        if (lbsnd == null) return;

                        lbsnd.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate ()
                        {
                            //lbsnd.Items.Add(str);
                            lbsnd.Items.Insert(0, stime + str);

                        }));
                    }
                    else
                    {
                        if (lbrcv == null) return;

                        lbrcv.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate ()
                        {
                            //lbrcv.Items.Add(str);
                            lbrcv.Items.Insert(0, stime + str);

                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                printf("[fn_Display]" + ex.Message);
            }


        }
        //---------------------------------------------------------------------------
        private void SendPacket(pmcPacket stPacket)
        {
            try
            {
                client.GetStream().Write(pmcConverter.StructureToByte(stPacket), 0, pmcPacket.PacketSize);

                if (IsPollingPacket(stPacket) && !m_bUsePollLog) return;

                printf("[Packet Send] " + (COMMAND)stPacket.Signal, true);

            }
            catch (System.Exception ex)
            {
                printf("[SendPacket]" + ex.Message);
                fn_StopThread();
            }

        }
        //---------------------------------------------------------------------------
        protected void SendAckPacket(pmcPacket stPacket)
        {
            stPacket.Reply = (int)REPLY.ack_Success;
            SendPacket(stPacket);
        }
        //---------------------------------------------------------------------------
        private bool IsPollingPacket(pmcPacket pck)
        {
            if (pck.Signal == (int)COMMAND.cmdCurrentState) return true;
            if (pck.Signal == (int)COMMAND.cmdCurrentData ) return true;
            return false; 

        }
        //---------------------------------------------------------------------------
        private void WorkThread(object obj)
        {
            TcpClient sock = (TcpClient)obj;

            while (true)
            {
                if (!sock.Connected) break;

                int length = 0;
                NetworkStream stream = sock.GetStream();
                byte[] bytes = new byte[pmcPacket.PacketSize];

                try
                {
                    if ((length = stream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        pmcPacket stPacket = pmcConverter.ByteToStructure<pmcPacket>(bytes);
                        //printf("[Packet Recv.] " + (COMMAND)stPacket.Signal);

                        if (IsPollingPacket(stPacket))
                        {
                            pollingQue.Enqueue(stPacket);
                        }
                        else
                        {
                            procQue.Enqueue(stPacket);
                        }

                        m_bAlive = false;
                    }
                    else m_bAlive = true;
                }
                catch (System.Exception ex)
                {
                    printf("[SendPacket]" + ex.Message);
                    m_bAlive = true;
                }

                Thread.Sleep(10);
            }
        }
        //---------------------------------------------------------------------------
        private void PollingThread(object obj)
        {
            Queue<pmcPacket> que = obj as Queue<pmcPacket>;

            while (true)
            {
                Thread.Sleep(100);
                if (que.Count < 1) continue;

                pmcPacket stPacket = que.Dequeue();
                if(m_bUsePollLog) printf("[Polling. Packet Recv] " + (COMMAND)stPacket.Signal);

                switch ((COMMAND)stPacket.Signal)
                {
                    case COMMAND.cmdCurrentState:
                        stPacket.DataClear();
                        stPacket.Reply = (int)REPLY.ack_Success;
                        stPacket.PushData<State>(curState);
                        SendPacket(stPacket);
                        break;
                    case COMMAND.cmdCurrentData:
                        DataList curDataList = new DataList();
                        curDataList.Add(data); curDataList.Add(data2); curDataList.Add(data3); curDataList.Add(data4);
                        stPacket.DataClear();
                        stPacket.Reply = (int)REPLY.ack_Success;
                        stPacket.PushData<DataList>(curDataList);
                        SendPacket(stPacket);
                        break;
                }
            }
        }
        //---------------------------------------------------------------------------
        private void ProcThread(object obj)
        {
            Queue<pmcPacket> que = obj as Queue<pmcPacket>;
            SJob job;
            SJobStruct struct_data;
            while (true)
            {
                Thread.Sleep(100);
                if (que.Count < 1) continue;

                pmcPacket stPacket = que.Dequeue();
                printf("[Proc. Packet Recv] " + (COMMAND)stPacket.Signal);

                switch ((COMMAND)stPacket.Signal)
                {
                    case COMMAND.cmdTimeSync:
                        TimeSync pmcTime = new TimeSync();
                        pmcTime = stPacket.GetData<TimeSync>();
                        //
                        SetTime(pmcTime);
                        break;

                    case COMMAND.cmdVersion:
                        break;

                    case COMMAND.cmdAreYouAlive:
                        stPacket.DataClear();
                        stPacket.Reply = (int)REPLY.ack_Success;
                        SendPacket(stPacket);

                        m_bAlive = !m_bAlive; //Connection Check

                        break;

                    case COMMAND.cmdPrepareProc:
                        job = stPacket.GetData<SJob>();
                        struct_data = job.getStructData();

                        //WriteOperationLog("Prepare", true);

                        m_bPrepare = true;

                        //SetProcState(0, PROCESS_STATE.IDLE);

                        //printf("[Prepare Proc] count : " + job.Count + " job id : " + job[0].sjob_id);
                        //stPacket.Reply = (int)REPLY.ack_Success;
                        //SendPacket(stPacket);
                        break;
                    case COMMAND.cmdLoadStart:
                        //List<SpecimenInfo> specimen_info = stPacket.GetDataList<SpecimenInfo>(); //List로 올수도 있음. 설비별로 다름.
                        job = stPacket.GetData<SJob>();
                        struct_data = job.getStructData();
                        //SetPortState(0, PORT_STATE.LOADING);
                        //SetPortState(1, PORT_STATE.IDLE);

                        //curState.ProcessState.count = 0; //clear
                        //curState.ProcessState.Add(PROCESS_STATE.READY);
                        //printf("[Loading Proc] count : " + specimen_info.Count + " specimen id : " + specimen_info[0].specimen_id);
                        //WriteOperationLog("Load", true);

                        m_bLoadStart = true;

                        Thread.Sleep(1500); // 3초간 Loaded상태 유지(Odin - Dashboard)
                        //SetPortState(0, PORT_STATE.LOADED);
                        //SetPortState(1, PORT_STATE.IDLE);
                        //stPacket.Reply = (int)REPLY.ack_Success;
                        //SendPacket(stPacket);
                        Thread.Sleep(3000); // 3초간 Loaded상태 유지(Odin - Dashboard)
                        break;
                    case COMMAND.cmdRunProc:
                        //Body Data 없음.
                        job = stPacket.GetData<SJob>();
                        struct_data = job.getStructData();
                        //SetProcState(0, PROCESS_STATE.PROCESSING);
                        //printf("[Processing Proc]");
                        //WriteOperationLog("Process");

                        m_bProcessStart = true;
                        //SetProcState(0, PROCESS_STATE.PROCESSING_DONE);
                        //stPacket.Reply = (int)REPLY.ack_Success;

                        //Unload Ready
                        //printf("[Unload Ready Proc]");
                        //WriteOperationLog("Unload Ready");
                        //data.setValue((int)0);

                        //SendPacket(stPacket);

                        //SetPortState(0, PORT_STATE.LOADED);
                        //SetPortState(1, PORT_STATE.UNLOAD_READY);
                        Thread.Sleep(3000); // 3초간 UNLOAD_READY 상태 유지(Odin - Dashboard)
                        break;
                    case COMMAND.cmdUnloadStart:
                        //SpecimenInfo specimen_info_unload = stPacket.GetData<SpecimenInfo>(); //List로 올수도 있음. 설비별로 다름.
                        job = stPacket.GetData<SJob>();
                        struct_data = job.getStructData();
                        //SetPortState(0, PORT_STATE.LOADED);
                        //SetPortState(1, PORT_STATE.UNLOADING);
                        //printf("[UnLoading Proc] specimen id : " + specimen_info_unload.specimen_id);
                        //WriteOperationLog("Unload", true);

                        m_bUnloadStart = true;

                        //stPacket.Reply = (int)REPLY.ack_Success;
                        //SendPacket(stPacket);

                        ////TheEnd
                        //SetPortState(0, PORT_STATE.IDLE);
                        //SetPortState(1, PORT_STATE.IDLE);
                        //SetProcState(0, PROCESS_STATE.IDLE);
                        break;
                    default:
                        COMMAND command = (COMMAND)stPacket.Signal;
                        string str = command.ToString() + ":";
                        if (command == COMMAND.cmdTimeSync)
                        {
                            TimeSync timesync = stPacket.GetData<TimeSync>();
                            str += timesync.Year + "_" + timesync.Month + "_" + timesync.Day + "_" + timesync.Hour + "_" + timesync.Minute + "_" + timesync.Second;
                        }
                        else if (command == (COMMAND.cmdModeChange))
                        {
                            int Mode = stPacket.Data[0];
                            str += Mode;
                        }

                        //WriteOperationLog(str);

                        stPacket.Reply = (int)REPLY.ack_Success;
                        SendPacket(stPacket);
                        break;
                }
            }
        }
        //-------------------------------------------------------------------------------------------------
        private byte[] ConvertIntToByte(int val)
        {
            byte[] b = new byte[4];
            b[0] = (byte)(val % (1 << 8));
            val /= (1 << 8);
            b[1] = (byte)(val % (1 << 8));
            val /= (1 << 8);
            b[2] = (byte)(val % (1 << 8));
            val /= (1 << 8);
            b[3] = (byte)(val % (1 << 8));

            return b;
        }
        //-------------------------------------------------------------------------------------------------
        public void SetPortState(int portNo, PORT_STATE portState)
        {
            if (portNo == (int)EN_PORT_ID.LoadPort)
            {
                m_nLoadPortState = (int)portState;
            }
            else if (portNo == (int)EN_PORT_ID.UnloadPort)
            {
                m_nUnloadPortState = (int)portState;
            }

            var b = ConvertIntToByte((int)portState);

            Array.Copy(b, 0, curState.PortState.portState, portNo * 4, 4);
        }
        //-------------------------------------------------------------------------------------------------
        public void SetProcState(int procNo, PROCESS_STATE procState)
        {
            m_nProcessState = (int)procState;

            var b = ConvertIntToByte((int)procState);

            Array.Copy(b, 0, curState.ProcessState.processState, procNo * 4, 4);
        }
        //-------------------------------------------------------------------------------------------------
        protected virtual void InitState()
        {
            curState.ProcessState.Add(PROCESS_STATE.IDLE);
            curState.PortState.Add(PORT_STATE.IDLE);
            curState.PortState.Add(PORT_STATE.IDLE);
        }
        //-------------------------------------------------------------------------------------------------
        protected virtual void InitReadyState()
        {
            SetProcState(0, PROCESS_STATE.IDLE);
            SetPortState(0, PORT_STATE.IDLE);
            SetPortState(1, PORT_STATE.IDLE);
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetAlarmState(ALARM_STATE state)
        {
            curState.AlarmState = (int)state;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetVersion(string ver)
        {
            Samsung.PMC.Packet.Body.Version pmcVer = new Samsung.PMC.Packet.Body.Version(ver);
        }
        //-------------------------------------------------------------------------------------------------
        public void SetAlarm(int no)
        {
            Alarm pmcAlarm = new Alarm(no);

            pmcPacket packet = new pmcPacket();

            packet.DataClear();
            packet.Signal = (int)COMMAND.cmdAlarm;
            packet.Reply = (int)REPLY.ack_Success;
            packet.PushData<Alarm>(pmcAlarm);

            fn_EnqueuePacket(packet);

        }
        //-------------------------------------------------------------------------------------------------
        private void SendThread()
        {
            while (true)
            {
                Thread.Sleep(100);

                if (m_SendQue.Count < 1) continue;

                pmcPacket stPacket = m_SendQue.Dequeue();
                //printf("[Data Packet Send] " + (COMMAND)stPacket.Signal, true);

                SendPacket(stPacket);

            }
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetIpAddress(string ip, int port)
        {
            pmcip   = ip;
            pmcport = port;
        }
        //---------------------------------------------------------------------------
        public void fn_Update() 
        {
            //
            if (pmcip   == string.Empty) return;
            if (pmcport  < 1           ) return;

            printf("[OPEN PMC] ip : " + pmcip + ", port : " + pmcport);

            IPEndPoint localAddress = new IPEndPoint(IPAddress.Parse(pmcip), pmcport);
            server = new TcpListener(localAddress);
            server.Start();
            printf("server start...");

            while (true)
            {
                //
                if (client != null) //if (client != null && client.Connected)
                {
                    m_tAliveTimer.OnDelay(m_bAlive, 5000);
                    if (m_tAliveTimer.Out)
                    {
                        //
                        fn_StopThread();
                        client.Close();
                        client = null;
                        m_bAlive = false;
                    }

                    continue;
                }

                try
                {
                    client = server.AcceptTcpClient();
                    Console.WriteLine("Accept");

                    fn_SetCommState   (COMM_STATE.CONNECTED); //curState.CommunicationState = (int)COMM_STATE.CONNECTED;
                    //fn_SetControlState(CONTROL_STATE.AUTO  ); //curState.ControlState = (int)CONTROL_STATE.AUTO;
                    fn_SetControlState();

                    printf("Accept Client : " + client.Client.RemoteEndPoint.ToString());

                    if (thread != null)
                    {
                        thread.Abort();
                        thread = null;
                    }

                    thread = new Thread(new ParameterizedThreadStart(WorkThread));
                    thread.Start(client);

                    polling_thread = new Thread(new ParameterizedThreadStart(PollingThread));
                    polling_thread.Start(pollingQue);

                    proc_thread = new Thread(new ParameterizedThreadStart(ProcThread));
                    proc_thread.Start(procQue);

                    proc_Send = new Thread(new ThreadStart(SendThread));
                    proc_Send.Start();
                }
                catch (Exception ex)
                {
                    printf("[fn_OpenPMC] " + ex.Message);
                    m_bAlive = true;
                }

            }
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
            PMC Thread Stop	
        </summary>
        @author    정지완(JUNGJIWAN)
        @date      2020/03/17 08:38
        */
        public void fn_StopThread()
        {
            if (thread         != null) thread        .Abort();
            if (polling_thread != null) polling_thread.Abort();
            if (proc_thread    != null) proc_thread   .Abort();
            if (proc_Send      != null) proc_Send     .Abort();

            //Que Clear
            m_SendQue.Clear();

            m_bAlive = false;

            printf("[Client] Stop Thread and Clear Que");

        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Send Packet to Queue
        </summary>
        <param name="stPacket"> User Set Packet </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/03/16 15:10
        */
        
        public void fn_EnqueuePacket(pmcPacket stPacket)
        {
            //
            if (client == null || !client.Connected) return;
            if (stPacket.Signal == 0               ) return; 
            
            m_SendQue.Enqueue(stPacket);

            printf("[Push Que Packet] " + (COMMAND)stPacket.Signal, true);
        }
        //---------------------------------------------------------------------------
        public void fn_SetCommState(COMM_STATE state)
        {
            //curState = State(COMM_STATE.DISCONNECTED, CONTROL_STATE.OFFLINE, PORT_STATE.LOAD_READY, PROCESS_STATE.IDLE, ALARM_STATE.CLEAR);
            curState.CommunicationState = (int)state;
        }
        //---------------------------------------------------------------------------
        public void fn_SetControlState(CONTROL_STATE state)
        {
            curState.ControlState = (int)state;
        }
        //---------------------------------------------------------------------------
        public void fn_SetControlState()
        {
            CONTROL_STATE state = new CONTROL_STATE();
                 if (FM.fn_GetRunMode(EN_RUN_MODE.AUTO_MODE)) state = CONTROL_STATE.AUTO   ;
            else if (FM.fn_GetRunMode(EN_RUN_MODE.MAN_MODE )) state = CONTROL_STATE.MANUAL ;
            else if (FM.fn_GetRunMode(EN_RUN_MODE.TEST_MODE)) state = CONTROL_STATE.OFFLINE;

            curState.ControlState = (int)state;
        }
        //---------------------------------------------------------------------------
        public void SetTime(TimeSync time)
        {
            //Time Setting
            SYSTEMTIME systemTime = new SYSTEMTIME();
            systemTime.Year   = (short)time.Year  ;
            systemTime.Month  = (short)time.Month ;
            systemTime.Day    = (short)time.Day   ;
            systemTime.Hour   = (short)time.Hour  ;
            systemTime.Minute = (short)time.Minute;
            systemTime.Second = (short)time.Second;

            SetLocalTime(ref systemTime);

            printf("[Time Sync] DONE");

        }
        //---------------------------------------------------------------------------
        protected bool IsConnect()
        {
            return (client != null && client.Connected);
        }
        //---------------------------------------------------------------------------
        public void fn_Close()
        {
            try
            {
                if (server != null) server.Stop ();
                if (client != null) client.Close();

                printf("[Close] OK");

            }
            catch (System.Exception ex)
            {
                printf("[Close] " + ex.Message);
            }

        }




    }
}
