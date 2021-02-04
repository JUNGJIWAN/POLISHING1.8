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
using WaferPolishingSystem.Define;

namespace WaferPolishingSystem.BaseUnit
{
    public class SocketServerUnit
    {
        TcpListener tcpListener;

        public string   Address { get {return strAddress; } set {strAddress = value; } }
        public int      Port    { get {return nPort; } set { nPort = value; } }
        public NetworkStream Stream {set {m_stream = value; } }
        public TcpClient AcceptClient { get { return fn_AcceptClient(); } }
        public bool ServerConnect { get { return m_bServerConnect; } set { m_bServerConnect = value; } }
        public bool ClientConnect { get { return m_bClientConnect; } }
        public pmcPacket ReadPacket { get { return m_ReadPacket; } }
        public pmcPacket SendPacket { get { return m_SendPacket; } set { m_SendPacket = value; } }





        private string  strAddress = IPAddress.Any.ToString()   ;
        private int     nPort = 21000                           ;
        private bool    m_bClientConnect = false                ;
        private bool    m_bServerConnect = false                ;
        private bool    bRet = false                            ;
        private int     m_nStepReceive  = 0                     ;
        private int     m_nStepSend     = 0                     ;
        private pmcPacket m_ReadTemp                            ;
        private pmcPacket m_SendTemp                            ;
        private pmcPacket m_ReadPacket                          ;
        private pmcPacket m_SendPacket                          ;
        private NetworkStream m_stream                          ;

        TOnDelayTimer m_tReceiveWait = new TOnDelayTimer();

        public Queue<pmcPacket> m_qReceive  = new Queue<pmcPacket>();
        public Queue<pmcPacket> m_qSend     = new Queue<pmcPacket>();

        public SocketServerUnit()
        {
            fn_InitVar();
        }
        ~SocketServerUnit()
        {
            fn_CloseServer();
        }
        private void fn_InitVar()
        {
            m_bClientConnect    = false;
            m_bServerConnect    = false;
            m_nStepReceive      = 0;
            m_nStepSend         = 0;
            m_ReadTemp          = new pmcPacket();
            m_SendTemp          = new pmcPacket();
            m_ReadPacket        = new pmcPacket();
            m_SendPacket        = new pmcPacket();
            m_stream            = null;
        }
        public bool fn_OpenServer(string strAddress, int nPort)
        {
            bRet = false;

            try
            {
                if (tcpListener != null) tcpListener.Stop();

                tcpListener = new TcpListener(IPAddress.Parse(strAddress), nPort);

                tcpListener.Start();

                bRet     = true;

                return bRet;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return bRet;
            }
        }
        public bool fn_CloseServer()
        {
            bRet = false;
            if (tcpListener != null)
            {
                tcpListener.Stop();
                bRet = true;
                return bRet;
            }
            return bRet;
        }
        private TcpClient fn_AcceptClient()
        {
            return tcpListener.AcceptTcpClient();
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_Receive()
        {
            int length = 0;
            int nReceiveErrorCount = 0;
            byte[] bytes = new byte[pmcPacket.PacketSize];

            m_bClientConnect = true;

            while (true)
            {
                Thread.Sleep(10);

                if (!ServerConnect)           break;
                if (nReceiveErrorCount > 500) break;

                if ((length = m_stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    pmcPacket stPacket = pmcConverter.ByteToStructure<pmcPacket>(bytes);

                    fn_PushReadQueue(stPacket);
                }
                else
                {
                    nReceiveErrorCount++;
                }
            }
            m_bClientConnect = false;
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_UpdateReceive()
        {
            while (true)
            {
                Thread.Sleep(100);
                fn_ReceiveQueueParsing();
            }
        }
        public void fn_UpdateSend()
        {
            while (true)
            {
                Thread.Sleep(100);
                fn_SendQueueParsing();
            }
        }
        
        private void fn_ReceiveQueueParsing()
        {
            switch (m_nStepReceive)
            {
                case 0:
                    if (m_qReceive.Count > 0)
                    {
                        m_nStepReceive++;
                    }
                    return;
                case 1:
                    m_ReadTemp = m_qReceive.Dequeue();
                    fn_ReadProcQueue(m_ReadTemp);
                    m_nStepReceive = 0;
                    return;

                default:
                    break;
            }
        }
        private void fn_SendQueueParsing()
        {
            switch (m_nStepSend)
            {
                case 0:
                    if (m_qSend.Count > 0)
                    {
                        m_nStepSend++;
                    }
                    return;
                case 1:
                    m_SendTemp = m_qSend.Dequeue();
                    fn_SendPacket(m_SendTemp);
                    m_nStepSend = 0;
                    return;
                default:
                    break;
            }
        }
        private bool fn_SendPacket(pmcPacket packet)
        {
            bRet = false;
            m_SendPacket = packet;
            try
            {
                if (m_SendPacket.Size < 0) return bRet;

                m_stream.Write(pmcConverter.StructureToByte(m_SendPacket), 0, pmcPacket.PacketSize);
                bRet = true;
                return bRet;
            }
            catch (System.Exception ex)
            {
                return bRet;
            }
        }
        public void fn_PushSendQueue(pmcPacket packet)
        {
            //
            m_SendPacket = packet;
            m_qSend.Enqueue(m_SendPacket);
        }
        public void fn_PushReadQueue(pmcPacket packet)
        {
            m_ReadPacket = packet;
            m_qReceive.Enqueue(m_ReadPacket);
        }
        private void fn_ReadProcQueue(pmcPacket packet)
        {
            try
            {
                switch ((COMMAND)packet.Signal)
                {
                    case COMMAND.cmdTimeSync:
                        break;
                    case COMMAND.cmdStop:
                        break;
                    case COMMAND.cmdCycleStop:
                        break;
                    case COMMAND.cmdOrigin:
                        break;
                    case COMMAND.cmdModeChange:
                        break;
                    case COMMAND.cmdWaferIn:
                        break;
                    case COMMAND.cmdJobStart:
                        break;
                    case COMMAND.cmdWaferOut:
                        break;
                    case COMMAND.cmdAutoOp:
                        break;
                    case COMMAND.cmdAutoOpEnd:
                        break;
                    case COMMAND.cmdRunRecipe:
                        break;
                    case COMMAND.cmdPrepareProc:
                        break;
                    case COMMAND.cmdLotInfo:
                        break;
                    case COMMAND.cmdDieIn:
                        break;
                    case COMMAND.cmdWaferInfo:
                        break;
                    case COMMAND.cmdMountInfo:
                        break;
                    case COMMAND.cmdSpecimenInfo:
                        break;
                    case COMMAND.cmdLoadStart:
                        break;
                    case COMMAND.cmdUnloadStart:
                        break;
                    case COMMAND.cmdState:
                        break;
                    case COMMAND.cmdAlarm:
                        break;
                    case COMMAND.cmdAreYouAlive:
                        break;
                    case COMMAND.cmdPMCRequest:
                        break;
                    case COMMAND.cmdRunProc:
                        break;
                    case COMMAND.cmdRunRecipeList:
                        break;
                    case COMMAND.cmdVersion:
                        break;
                    case COMMAND.cmdCurrentState:
                        fn_SendProcQueue(packet);
                        break;
                    case COMMAND.cmdCurrentData:
                        fn_SendProcQueue(packet);
                        break;
                    case COMMAND.cmdWaferExist:
                        break;
                    case COMMAND.cmdDicingInfo:
                        break;
                    case COMMAND.cmdDicingInfoList:
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
            	
            }
        }
        //------------------------------------------------------------------------------------------------- 
        private void fn_SendProcQueue(pmcPacket packet)
        {
            try
            {
                switch ((COMMAND)packet.Signal)
                {
                    case COMMAND.cmdTimeSync:
                        break;
                    case COMMAND.cmdStop:
                        break;
                    case COMMAND.cmdCycleStop:
                        break;
                    case COMMAND.cmdOrigin:
                        break;
                    case COMMAND.cmdModeChange:
                        break;
                    case COMMAND.cmdWaferIn:
                        break;
                    case COMMAND.cmdJobStart:
                        break;
                    case COMMAND.cmdWaferOut:
                        break;
                    case COMMAND.cmdAutoOp:
                        break;
                    case COMMAND.cmdAutoOpEnd:
                        break;
                    case COMMAND.cmdRunRecipe:
                        break;
                    case COMMAND.cmdPrepareProc:
                        break;
                    case COMMAND.cmdLotInfo:
                        break;
                    case COMMAND.cmdDieIn:
                        break;
                    case COMMAND.cmdWaferInfo:
                        break;
                    case COMMAND.cmdMountInfo:
                        break;
                    case COMMAND.cmdSpecimenInfo:
                        break;
                    case COMMAND.cmdLoadStart:
                        break;
                    case COMMAND.cmdUnloadStart:
                        break;
                    case COMMAND.cmdState:
                        break;
                    case COMMAND.cmdAlarm:
                        break;
                    case COMMAND.cmdAreYouAlive:
                        break;
                    case COMMAND.cmdPMCRequest:
                        break;
                    case COMMAND.cmdRunProc:
                        break;
                    case COMMAND.cmdRunRecipeList:
                        break;
                    case COMMAND.cmdVersion:
                        break;
                    case COMMAND.cmdCurrentState:
                        fn_PushSendQueue(packet);
                        break;
                    case COMMAND.cmdCurrentData:
                        fn_PushSendQueue(packet);
                        break;
                    case COMMAND.cmdWaferExist:
                        break;
                    case COMMAND.cmdDicingInfo:
                        break;
                    case COMMAND.cmdDicingInfoList:
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {

            }
        }
        //-------------------------------------------------------------------------------------------------
        #region Sample
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

        //public  void fn_Update()
        //{
        //    if (!bConnect) return;

        //    while (true) // Connect Client Check Loop
        //    {
        //        try
        //        {
        //            Thread.Sleep(10);
        //            bConnect = false;
        //            TcpClient client = tcpListener.AcceptTcpClient();
        //            bConnect = true;
        //            nReceiveCount = 0;
        //            NetworkStream stream = client.GetStream();

        //            while(true) // Receive Loop
        //            {
        //                //Console.WriteLine("Connect : {0}", client.Connected);
        //                Thread.Sleep(10);
        //                if (nReceiveCount > 500)
        //                {
        //                    break;
        //                }

        //                fn_Receive(stream);

        //                if (SendFlag)
        //                {
        //                    fn_Send(stream);
        //                    SendFlag = false;
        //                }
        //            }
        //        }
        //        catch (System.Exception ex)
        //        {

        //        }
        //    }
        //}
        //-------------------------------------------------------------------------------------------------
        //private void fn_Receive(NetworkStream stream)
        //{
        //    try
        //    {
        //        //if (stream.CanRead)
        //        if (stream.DataAvailable)
        //        {
        //            stream.Read(Receive_Buff, 0, Receive_Buff.Length);
        //            Console.WriteLine("Data : {0}", Receive_Buff.Length);
        //            Receive_Msg = Encoding.ASCII.GetString(Receive_Buff);
        //            nReceiveCount = 0;
        //        }
        //        else
        //            nReceiveCount++;
        //    }
        //    catch (System.Exception ex)
        //    {

        //    }
        //}
        #endregion
    }
}
