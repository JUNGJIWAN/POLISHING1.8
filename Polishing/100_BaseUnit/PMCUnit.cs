using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Samsung.PMC.Packet;
using Samsung.PMC.Packet.Body;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;

namespace WaferPolishingSystem.BaseUnit
{
    public class PMCUnit : ConsoleEQP
    {
        //Var
        //bool    m_bConnect;
        int     m_nModeTest;
        bool    m_bDuringProcess;

        //---------------------------------------------------------------------------
        //Property
        public bool _bConnect 
        { 
            get { return IsConnect(); } 
        }
        public bool _bUpdate
        {
            get { return m_bUpdate; }
            set { m_bUpdate = value; }
        }
        public bool _bUsePollLog
        {
            get { return m_bUsePollLog; }
            set { m_bUsePollLog = value; }
        }
        public bool _bDryRunMode
        {
            get { return m_bDryRunMode; }
            set { m_bDryRunMode = value; }
        }

        public bool _Prepare { get { return m_bPrepare; } set { m_bPrepare = value; } }
        public bool _LoadStart { get { return m_bLoadStart; } set { m_bLoadStart = value; } }
        public bool _ProcessStart { get { return m_bProcessStart; } set { m_bProcessStart = value; } }
        public bool _UnloadStart { get { return m_bUnloadStart; } set { m_bUnloadStart = value; } }
        public bool _DuringProcess { get { return m_bDuringProcess; } }



        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public PMCUnit()
        {
            //
            
            //m_bConnect = false;
            m_bUpdate        = false;
            m_bDuringProcess = false;

            m_nModeTest = 0; 


        }
        //---------------------------------------------------------------------------
        public void fn_SetListBox(ListBox send, ListBox recv)
        {
            lbsnd = send;
            lbrcv = recv;

            Console.WriteLine("List Box Setting...");
        }
        //---------------------------------------------------------------------------
        public void fn_AlarmSet(int no, int grade)
        {
            //
            SetAlarm(no);

            switch ((EN_ERR_GRADE)grade)
            {
                case EN_ERR_GRADE.egError  : fn_SetAlarmState(ALARM_STATE.ALARM  ); break;
                case EN_ERR_GRADE.egWarning: fn_SetAlarmState(ALARM_STATE.WARNING); break;
                case EN_ERR_GRADE.egDisplay: fn_SetAlarmState(ALARM_STATE.WARNING); break;
                default:                     fn_SetAlarmState(ALARM_STATE.CLEAR  ); break;
                    
            }

        }
        //---------------------------------------------------------------------------
        public void fn_SendCommand(COMMAND cmd)
        {
            //Local Var.
            pmcPacket stPacket = new pmcPacket();
            stPacket.DataClear();
            stPacket.Signal = (int)cmd;

            switch (cmd)
            {
                case COMMAND.cmdStop:
                    
                    stPacket.Reply  = (int)REPLY.req_AckUse;
                    break;

                case COMMAND.cmdCycleStop:

                    stPacket.Reply  = (int)REPLY.req_AckUse;
                    break;

                case COMMAND.cmdOrigin:

                    stPacket.Reply  = (int)REPLY.req_AckUse;
                    break;

                case COMMAND.cmdModeChange:
                    //if (m_nModeTest++ == 0) pmcMode = new Mode(MODE.Manual);
                    //else
                    //{
                    //    pmcMode = new Mode(MODE.Offline);
                    //    m_nModeTest = 0; 
                    //}
                    //stPacket.Reply  = (int)REPLY.req_AckUse;
                    //stPacket.PushData<Mode>(pmcMode);
                    break;

                case COMMAND.cmdPrepareProc:
                    stPacket.Reply = (int)REPLY.ack_Success;
                    break;

                case COMMAND.cmdLoadStart:
                    stPacket.Reply = (int)REPLY.ack_Success;
                    break;
                case COMMAND.cmdUnloadStart:
                    stPacket.Reply = (int)REPLY.ack_Success;
                    break;
                case COMMAND.cmdAlarm:
                    break;
                case COMMAND.cmdAreYouAlive:
                    break;
                case COMMAND.cmdRunProc:
                    break;
                case COMMAND.cmdVersion:
                    break;
                case COMMAND.cmdCurrentState:
                    break;
                case COMMAND.cmdCurrentData:
                    break;
                
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case COMMAND.cmdTimeSync:
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
                case COMMAND.cmdState:
                    break;
                case COMMAND.cmdPMCRequest:
                    break;
                case COMMAND.cmdRunRecipeList:
                    break;
                case COMMAND.cmdDicingInfoList:
                    break;

                default:
                    return; 
            }

            fn_EnqueuePacket(stPacket);

            printf("[MANUAL]" + (COMMAND)stPacket.Signal, true);
        }
        //---------------------------------------------------------------------------
        public State fn_GetState()
        {
            return curState; 
        }
        //------------------------------------------------------------------------------------------------- 
        public void fn_SetState(EN_PROCESS_STATE nState)
        {
            switch (nState)
            {
                case EN_PROCESS_STATE.en_Loading:
                    SetPortState((int)EN_PORT_ID.LoadPort, PORT_STATE.LOADING);
                    SetPortState((int)EN_PORT_ID.UnloadPort, PORT_STATE.IDLE);
                    SetProcState((int)EN_PORT_ID.LoadPort, PROCESS_STATE.READY);
                    break;

                case EN_PROCESS_STATE.en_Preparing:
                    SetPortState((int)EN_PORT_ID.LoadPort, PORT_STATE.LOAD_READY);
                    SetPortState((int)EN_PORT_ID.UnloadPort, PORT_STATE.IDLE);
                    SetProcState((int)EN_PORT_ID.LoadPort, PROCESS_STATE.READY);
                    fn_SendCommand(COMMAND.cmdPrepareProc);
                    break;

                case EN_PROCESS_STATE.en_ProcessingReady:
                    SetPortState((int)EN_PORT_ID.LoadPort, PORT_STATE.LOADED);
                    SetProcState((int)EN_PORT_ID.LoadPort, PROCESS_STATE.READY);
                    fn_SendCommand(COMMAND.cmdLoadStart);
                    break;

                case EN_PROCESS_STATE.en_Processing:
                    //if (FM.m_stMasterOpt.nUsePMCStateSkip != 1)
                    //{
                    //    SetPortState((int)EN_PORT_ID.LoadPort, PORT_STATE.IDLE);
                    //    m_bDuringProcess = true;
                    //}
                    SetProcState(0, PROCESS_STATE.PROCESSING);
                    break;

                case EN_PROCESS_STATE.en_ProcessDone:
                    SetPortState((int)EN_PORT_ID.UnloadPort, PORT_STATE.IDLE);
                    SetProcState((int)EN_PORT_ID.LoadPort, PROCESS_STATE.PROCESSING_DONE);
                    m_bDuringProcess = false;
                    fn_SendCommand(COMMAND.cmdRunProc);
                    break;

                case EN_PROCESS_STATE.en_UnloadReady:
                    SetPortState((int)EN_PORT_ID.UnloadPort, PORT_STATE.UNLOAD_READY);
                    SetProcState((int)EN_PORT_ID.LoadPort, PROCESS_STATE.PROCESSING_DONE);
                    break;

                case EN_PROCESS_STATE.en_Unloading:
                    SetPortState((int)EN_PORT_ID.UnloadPort, PORT_STATE.UNLOADING);
                    break;

                case EN_PROCESS_STATE.en_Unloaded:
                    //if (FM.m_stMasterOpt.nUsePMCStateSkip == 1)
                    //{
                    //    SetPortState((int)EN_PORT_ID.LoadPort, PORT_STATE.IDLE);
                    //}
                    SetPortState((int)EN_PORT_ID.UnloadPort, PORT_STATE.IDLE);

                    fn_SendCommand(COMMAND.cmdUnloadStart);
                    break;
                default:
                    break;
            }

            //Log Add
        }
    }
}
