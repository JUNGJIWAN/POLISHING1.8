using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Samsung.PMC.Packet;
using Samsung.PMC.Packet.Body;

namespace WaferPolishingSystem.BaseUnit
{
    public class PMCUnit
    {
        public TcpClient client;
        public Thread threadPMC;
        public Thread threadRead;
        public Thread threadSend;
        SocketServerUnit Server;

        public static string Version { get {return strVersion; } set { strVersion = value; } }
        public static string Recipename { get {return strRecipename; } set {strRecipename = value; } }
        public static string RecipeType { get { return strRecipetype; } set {strRecipetype = value; } }
        public static string RecipePath { get {return strRecipepath; } set {strRecipepath = value; } }
        public static string IOName { get { return strIOName; } set { strIOName = value; } }
        public static int IOValue { get { return nIOValue; } set { nIOValue = value; } }

        private static string strVersion;
        private static string strRecipename;
        private static string strRecipetype;
        private static string strRecipepath;
        private static string strIOName;
        private static int    nIOValue;

        // PMC Body
        public Samsung.PMC.Packet.Body.TimeSync stTime = new Samsung.PMC.Packet.Body.TimeSync(DateTime.Now);
        public Samsung.PMC.Packet.Body.Mode stMode = new Samsung.PMC.Packet.Body.Mode(MODE.Offline);
        public Samsung.PMC.Packet.Body.Alarm stAlarm = new Samsung.PMC.Packet.Body.Alarm((int)ALARM_STATE.ALARM);
        public Samsung.PMC.Packet.Body.Version stVersion = new Samsung.PMC.Packet.Body.Version();
        public Samsung.PMC.Packet.Body.Recipe stRecipe = new Samsung.PMC.Packet.Body.Recipe();
        public Samsung.PMC.Packet.Body.RecipeList stRecipeList = new Samsung.PMC.Packet.Body.RecipeList();
        public Samsung.PMC.Packet.Body.State stState = new Samsung.PMC.Packet.Body.State(COMM_STATE.DISCONNECTED, CONTROL_STATE.OFFLINE, PORT_STATE.LOAD_READY, PROCESS_STATE.IDLE, ALARM_STATE.CLEAR);
        public Samsung.PMC.Packet.Body.WaferInfo stWaferInfo = new Samsung.PMC.Packet.Body.WaferInfo();
        public Samsung.PMC.Packet.Body.SpecimenInfo stSpecimenInfo = new Samsung.PMC.Packet.Body.SpecimenInfo();
        public Samsung.PMC.Packet.Body.SpecimenInfoList stSpecimenInfolist = new Samsung.PMC.Packet.Body.SpecimenInfoList();
        public Samsung.PMC.Packet.Body.IO stIO = new Samsung.PMC.Packet.Body.IO();
        public Samsung.PMC.Packet.Body.IOList stIOList = new Samsung.PMC.Packet.Body.IOList();
        public Samsung.PMC.Packet.Body.WJob stWjob = new Samsung.PMC.Packet.Body.WJob();
        public Samsung.PMC.Packet.Body.WJobList stWjobList = new Samsung.PMC.Packet.Body.WJobList();
        public Samsung.PMC.Packet.Body.SJob stSjob = new Samsung.PMC.Packet.Body.SJob();
        public Samsung.PMC.Packet.Body.SJobList stSjobList = new Samsung.PMC.Packet.Body.SJobList();


        //public Queue<pmcPacket> pollingQue  = new Queue<pmcPacket>();
        //public Queue<pmcPacket> procQue     = new Queue<pmcPacket>();

        // Test
        public IO io = new IO("TESTIO_1", 12);
        public IO io2 = new IO("TESTIO_2", 123);
        public IO io3 = new IO("TESTIO_3", 1234);

        public bool IsServerConnect { get { return Server.ServerConnect; } set { Server.ServerConnect = value; } }
        public bool IsClientConnect { get { return Server.ClientConnect; } }
        // 생성자
        public PMCUnit()
        {
            threadPMC = null;
            threadRead = null;
            threadSend = null;
            client = null;

            Server = new SocketServerUnit();
        }
        ~PMCUnit()
        {
            fn_ClosePMC();
        }
        public void fn_OpenPMC()
        {
            if (threadPMC != null) return;
            else
            {
                threadPMC = new Thread(new ThreadStart(fn_RunServer));
                threadPMC.Start();
            }
        }
        public void fn_ClosePMC()
        {
            IsServerConnect = false;

            if (threadRead != null)
            {
                if (!threadRead.Join(1000))
                    threadRead.Abort();
            }
            if (threadSend != null)
            {
                if (!threadSend.Join(1000))
                    threadSend.Abort();
            }

            if (threadPMC != null)
            {
                if (!threadPMC.Join(1000))
                    threadPMC.Abort();
            }

            if (Server.ClientConnect)
            {
                Server.fn_CloseServer();
            }
        }
        public void fn_RunServer()
        {
            try
            {
                string bind_ip = "127.0.0.1";
                int bind_port = 21000;

                Server.Address = bind_ip;
                Server.Port = bind_port;

                if (Server.fn_OpenServer(Server.Address, Server.Port)) IsServerConnect = true;
                else IsServerConnect = false;

                while (true)
                {
                    if (!IsServerConnect) break;

                    client = Server.AcceptClient;

                    Server.Stream = client.GetStream();

                    if (threadRead != null)
                    {
                        if (!threadRead.Join(1000))
                            threadRead.Abort();
                    }
                    if (threadSend != null)
                    {
                        if (!threadSend.Join(1000))
                            threadSend.Abort();
                    }

                    threadRead = new Thread(new ThreadStart(fn_UpdateRead));
                    threadRead.Start();
                    threadSend = new Thread(new ThreadStart(fn_UpdateSend));
                    threadSend.Start();

                    fn_SetState(COMM_STATE.CONNECTED, CONTROL_STATE.AUTO, PORT_STATE.DISABLED, PROCESS_STATE.READY, ALARM_STATE.CLEAR);

                    Server.fn_Receive(); // PMC Data Receive loop

                    fn_SetState(COMM_STATE.DISCONNECTED, CONTROL_STATE.OFFLINE, PORT_STATE.DISABLED, PROCESS_STATE.READY, ALARM_STATE.CLEAR);
                }

                if (!IsServerConnect) fn_ClosePMC();
            }
            catch (Exception e)
            {
                printf(e.Message);
            }
        }
        private void fn_SetState(COMM_STATE nComState, CONTROL_STATE nConState, PORT_STATE nPortState, PROCESS_STATE nProcState, ALARM_STATE nAlarmState)
        {
            stState.CommunicationState = (int)nComState;
            stState.ControlState = (int)nConState;
            stState.PortState = (int)nPortState;
            stState.ProcessState = (int)nProcState;
            stState.AlarmState = (int)nAlarmState;

            Server.SendPacket.DataClear();
            Server.SendPacket.Reply = (int)REPLY.ack_Success;
            Server.SendPacket.PushData<State>(stState);
            Server.fn_PushSendQueue(Server.SendPacket);
        }

        public void fn_UpdateRead()
        {
            Server.fn_UpdateReceive();
        }
        public void fn_UpdateSend()
        {
            Server.fn_UpdateSend();
        }
        public void fn_SetSendMsg()
        {
            pmcPacket packet = new pmcPacket();
            packet.DataClear();
            packet.Src = 6;
            packet.Dest = 0;
            packet.UnitID = 1;
            packet.Signal = (int)COMMAND.cmdVersion;
            packet.Reply = (int)REPLY.ack_Success;
            packet.SeqNo = 0;
            packet.Size = (int)stVersion.datasize;
            packet.PushData<Samsung.PMC.Packet.Body.Version>(stVersion);
            Server.fn_PushSendQueue(packet);
        }
        public Byte[] fn_GetReadMsg()
        {
            return pmcConverter.StructureToByte(Server.ReadPacket);
        }
        public Byte[] fn_GetSendMsg()
        {
            return pmcConverter.StructureToByte(Server.SendPacket);
        }
        #region Sample
        public void printf(string str)
        {
            Console.WriteLine(str);
        }

        //public void SendPacket(pmcPacket stPacket)
        //{
        //    client.GetStream().Write(pmcConverter.StructureToByte(stPacket), 0, pmcPacket.PacketSize);
        //}
        ///**    
        //<summary>
        //    From PMC Receive Data
        //</summary>
        //<param name=""></param>
        //@author    이준호(LEEJOONHO)
        //@date      2020/03/09 12:26
        //*/
        //public void WorkThread(object obj)
        //{
        //    TcpClient sock = (TcpClient)obj;
        //    int length = 0;
        //    NetworkStream stream = sock.GetStream();
        //    byte[] bytes = new byte[pmcPacket.PacketSize];

        //    while (true)
        //    {
        //        if ((length = stream.Read(bytes, 0, bytes.Length)) > 0)
        //        {
        //            pmcPacket stPacket = pmcConverter.ByteToStructure<pmcPacket>(bytes);
        //            //printf("[Packet Recv] " + (COMMAND)stPacket.Signal);

        //            if (stPacket.Signal == (int)COMMAND.cmdCurrentData || stPacket.Signal == (int)COMMAND.cmdCurrentState)
        //                pollingQue.Enqueue(stPacket);
        //            else
        //                procQue.Enqueue(stPacket);
        //        }
        //        Thread.Sleep(10);
        //    }
        //}
        ///**    
        //<summary>

        //</summary>
        //<param name=""></param>
        //@author    이준호(LEEJOONHO)
        //@date      2020/03/09 13:42
        //*/
        //public void PollingThread(object obj)
        //{
        //    Queue<pmcPacket> que = obj as Queue<pmcPacket>;
        //    while (true)
        //    {
        //        Thread.Sleep(100);
        //        if (que.Count == 0) continue;

        //        pmcPacket stPacket = que.Dequeue();

        //        switch ((COMMAND)stPacket.Signal)
        //        {
        //            case COMMAND.cmdCurrentState:
        //                stPacket.DataClear();
        //                stPacket.Reply = (int)REPLY.ack_Success;
        //                stPacket.PushData<State>(curState);
        //                SendPacket(stPacket);
        //                break;
        //            case COMMAND.cmdCurrentData:
        //                io.io_val++; io2.io_val++; io3.io_val++;
        //                IOList curIOList = new IOList();
        //                curIOList.Add(io); curIOList.Add(io2); curIOList.Add(io3);
        //                stPacket.DataClear();
        //                stPacket.Reply = (int)REPLY.ack_Success;
        //                stPacket.PushData<IOList>(curIOList);
        //                SendPacket(stPacket);
        //                break;
        //        }
        //    }
        //}

        //public void ProcThread(object obj)
        //{
        //    Queue<pmcPacket> que = obj as Queue<pmcPacket>;
        //    while (true)
        //    {
        //        Thread.Sleep(100);
        //        if (que.Count == 0) continue;

        //        pmcPacket stPacket = que.Dequeue();
        //        printf("[Proc. Packet Recv] " + (COMMAND)stPacket.Signal);

        //        switch ((COMMAND)stPacket.Signal)
        //        {
        //            case COMMAND.cmdPrepareProc:
        //                List<SJob> job = stPacket.GetDataList<SJob>(); //List로 올수도있음. 설비별로 다름.
        //                curState.ProcessState = (int)PROCESS_STATE.PREPARING;

        //                printf("[Prepare Proc] count : " + job.Count + " job id : " + job[0].sjob_id);
        //                for (int i = 0; i < 5; i++)
        //                {
        //                    printf("\tPreparing : " + (i + 1) + " / " + 5);
        //                    Thread.Sleep(1000);
        //                }
        //                printf("\tPrepare End");
        //                curState.ProcessState = (int)PROCESS_STATE.READY;

        //                stPacket.Reply = (int)REPLY.ack_Success;
        //                SendPacket(stPacket);
        //                break;
        //            case COMMAND.cmdLoadStart:
        //                List<SpecimenInfo> specimen_info = stPacket.GetDataList<SpecimenInfo>(); //List로 올수도 있음. 설비별로 다름.
        //                curState.PortState = (int)PORT_STATE.LOADEDING;
        //                curState.ProcessState = (int)PROCESS_STATE.READY;
        //                printf("[Loading Proc] count : " + specimen_info.Count + " specimen id : " + specimen_info[0].specimen_id);
        //                for (int i = 0; i < 10; i++)
        //                {
        //                    printf("\tLoading : " + (i + 1) + " / " + 10);
        //                    Thread.Sleep(1000);
        //                }
        //                printf("\tLoading End");
        //                curState.PortState = (int)PORT_STATE.LOADED;

        //                stPacket.Reply = (int)REPLY.ack_Success;
        //                SendPacket(stPacket);
        //                break;
        //            case COMMAND.cmdRunProc:
        //                //Body Data 없음.
        //                curState.ProcessState = (int)PROCESS_STATE.PROCESSING;
        //                printf("[Processing Proc]");
        //                for (int i = 0; i < 10; i++)
        //                {
        //                    printf("\tProcessing : " + (i + 1) + " / " + 10);
        //                    Thread.Sleep(1000);
        //                }
        //                printf("\tProcessing End");
        //                curState.ProcessState = (int)PROCESS_STATE.PROCESSING_DONE;

        //                stPacket.Reply = (int)REPLY.ack_Success;
        //                SendPacket(stPacket);

        //                //Unload Ready
        //                printf("[Unload Ready Proc]");
        //                for (int i = 0; i < 5; i++)
        //                {
        //                    printf("\tUnload Ready : " + (i + 1) + " / " + 5);
        //                    Thread.Sleep(1000);
        //                }
        //                printf("\tUnload Ready End");
        //                curState.PortState = (int)PORT_STATE.UNLOAD_READY;

        //                break;
        //            case COMMAND.cmdUnloadStart:
        //                SpecimenInfo specimen_info_unload = stPacket.GetData<SpecimenInfo>(); //List로 올수도 있음. 설비별로 다름.
        //                curState.PortState = (int)PORT_STATE.UNLOADING;
        //                printf("[UnLoading Proc] specimen id : " + specimen_info_unload.specimen_id);
        //                for (int i = 0; i < 5; i++)
        //                {
        //                    printf("\tUnLoading : " + (i + 1) + " / " + 5);
        //                    Thread.Sleep(1000);
        //                }
        //                printf("\tUnLoading End");

        //                stPacket.Reply = (int)REPLY.ack_Success;
        //                SendPacket(stPacket);

        //                //TheEnd
        //                curState.PortState = (int)PORT_STATE.LOAD_READY;
        //                curState.ProcessState = (int)PROCESS_STATE.IDLE;
        //                break;
        //        }
        //    }
        //}
        #endregion
    }
}
