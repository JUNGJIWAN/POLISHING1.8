using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WaferPolishingSystem.BaseUnit;
using WaferPolishingSystem.Define;
using WaferPolishingSystem.Unit;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserFunction;
using UserInterface;
using Samsung.PMC.Packet;
using Samsung.PMC.Packet.Body;
using static WaferPolishingSystem.BaseUnit.PMCUnit;
using static WaferPolishingSystem.Define.UserEnum;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Manual_PMC : Page
    {
        //Var
        

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        


        public PageSetting_Manual_PMC()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            ubComm1 .Tag = COMMAND.cmdStop; 
            ubComm2 .Tag = COMMAND.cmdCycleStop;
            ubComm3 .Tag = COMMAND.cmdOrigin;
            ubComm4 .Tag = COMMAND.cmdModeChange;
            ubComm5 .Tag = COMMAND.cmdPrepareProc;
            ubComm6 .Tag = COMMAND.cmdLoadStart;
            ubComm7 .Tag = COMMAND.cmdUnloadStart;
            ubComm8 .Tag = COMMAND.cmdAlarm;
            ubComm9 .Tag = COMMAND.cmdAreYouAlive;
            ubComm10.Tag = COMMAND.cmdRunProc;
            ubComm11.Tag = COMMAND.cmdVersion;
            ubComm12.Tag = COMMAND.cmdCurrentState;
            ubComm13.Tag = COMMAND.cmdCurrentData ;

            ubComm1 .Content = COMMAND.cmdStop;
            ubComm2 .Content = COMMAND.cmdCycleStop;
            ubComm3 .Content = COMMAND.cmdOrigin;
            ubComm4 .Content = COMMAND.cmdModeChange;
            ubComm5 .Content = COMMAND.cmdPrepareProc;
            ubComm6 .Content = COMMAND.cmdLoadStart;
            ubComm7 .Content = COMMAND.cmdUnloadStart;
            ubComm8 .Content = COMMAND.cmdAlarm;
            ubComm9 .Content = COMMAND.cmdAreYouAlive;
            ubComm10.Content = COMMAND.cmdRunProc;
            ubComm11.Content = COMMAND.cmdVersion;
            ubComm12.Content = COMMAND.cmdCurrentState;
            ubComm13.Content = COMMAND.cmdCurrentData;

            
            EN_PORT_ID portId = new EN_PORT_ID();
            for (int i = 0; i < Enum.GetNames(typeof(EN_PORT_ID)).Length; i++)
            {
                cbPortNo.Items.Add(portId++);
            }
            EN_PORT_ID procId = new EN_PORT_ID();
            for (int i = 0; i < Enum.GetNames(typeof(EN_PORT_ID)).Length; i++)
            {
                cbProcNo.Items.Add(procId++);
            }
            PORT_STATE temp = new PORT_STATE();
            for (int i = 0; i < Enum.GetNames(typeof(PORT_STATE)).Length; i++)
            {
                cbPortState.Items.Add(temp++);
            }
            PROCESS_STATE tempProc = new PROCESS_STATE();
            for (int i = 0; i < Enum.GetNames(typeof(PROCESS_STATE)).Length; i++)
            {
                cbProcState.Items.Add(tempProc++);
            }
            CONTROL_STATE tempCont = new CONTROL_STATE();
            for (int i = 0; i < Enum.GetNames(typeof(CONTROL_STATE)).Length; i++)
            {
                cbContState.Items.Add(tempCont++);
            }

        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();
            string str = string.Empty;


            if (PMC._bConnect)
            {
                lbConnect.Background = Brushes.Lime;

                lbPortState.Content = string.Format($"PORT :    {PMC.fn_GetState().PortState   .ToString()}");
                lbPorce    .Content = string.Format($"PROCESS : {PMC.fn_GetState().ProcessState.ToString()}");
                lbControl  .Content = string.Format($"CONTROL : {PMC.fn_GetState().ControlState.ToString()}");

                if (PMC._bDryRunMode)
                {
                    var vPort    = PMC.fn_GetState().PortState;
                    var vProcess = PMC.fn_GetState().ProcessState;

                    cbPortNo.SelectedIndex = vPort.GetStateList().Count;
                    cbPortState.SelectedIndex = vPort.GetStateListInt().Count;
                    cbProcNo.SelectedIndex = vProcess.GetStateList().Count;
                    cbProcState.SelectedIndex = vProcess.GetStateListInt().Count; ;
                    cbContState.SelectedIndex = (int)PMC.fn_GetState().ControlState;
                }

            }
            else lbConnect.Background = Brushes.Gray;


            //
            m_UpdateTimer.Start();
        }
        //---------------------------------------------------------------------------
        //Timer On/Off
        public void fn_SetTimer(bool on)
        {
            if (on)
            {
                m_UpdateTimer.IsEnabled = true;
                m_UpdateTimer.Start();
            }
            else
                m_UpdateTimer.Stop();

        }
        //---------------------------------------------------------------------------
        private bool fn_DataParsing(byte[] receive, ref string str)
        {
            //object ob;
            int nSrc        = BitConverter.ToInt32(receive, 1   );
            int nDest       = BitConverter.ToInt32(receive, 5   );
            int nUnitId     = BitConverter.ToInt32(receive, 9   );
            int nSignal     = BitConverter.ToInt32(receive, 13  );
            int nReply      = BitConverter.ToInt32(receive, 17  );
            int nSeqNo      = BitConverter.ToInt32(receive, 21  );
            int nSize       = BitConverter.ToInt32(receive, 25  );

            str = "[Src] = " + nSrc.ToString() + " [Dest] = " + nDest.ToString() + " [UnitID] = " + nUnitId.ToString()
                + " [Signal] = " + nSignal.ToString() + " [Reply] = " + nReply.ToString() + " [Seq No] = " + nSeqNo.ToString()
                + " [Size] = " + nSize.ToString();
            return true;
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bnConnect.IsEnabled = false;
            bnDisconnect.IsEnabled = false;
            bnHidePollDate.Visibility = Visibility.Hidden;

            PMC.fn_SetListBox(list_Send, list_Receive);
            PMC._bUpdate = true;

            bnSendTeset.Content = string.Format("Update - {0}", PMC._bUpdate ? "ON" : "OFF");
            bnHidePollDate.Content = string.Format("Use Polling Log - {0}", PMC._bUsePollLog ? "ON" : "OFF");
            cbUsePollLog.IsChecked = PMC._bUsePollLog;
            cbDryRun.IsChecked = PMC._bDryRunMode;

            //Timer On
            fn_SetTimer(true);
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            fn_SetTimer(false);
            PMC._bUpdate = false;
        }
        //---------------------------------------------------------------------------
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            //
            UserButton selbtn = sender as UserButton;
            COMMAND nTag = (COMMAND)selbtn.Tag ;

            PMC.fn_SendCommand(nTag);
        }
        //---------------------------------------------------------------------------
        private void bnConnect_Click(object sender, RoutedEventArgs e)
        {
            //PMC.fn_OpenPMC();
            //fn_SetTimer(true);
        }
        //---------------------------------------------------------------------------
        private void bnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            //PMC.fn_ClosePMC();
            //fn_SetTimer(false);
        }
        //---------------------------------------------------------------------------
        private void bnRecReset_Click(object sender, RoutedEventArgs e)
        {
            list_Receive.Items.Clear();
        }
        //---------------------------------------------------------------------------
        
        private void bnSendTeset_Click(object sender, RoutedEventArgs e)
        {
            PMC._bUpdate = !PMC._bUpdate;
            bnSendTeset.Content = string.Format("Update - {0}", PMC._bUpdate ? "ON" : "OFF");

        }
        //---------------------------------------------------------------------------
        private void bnSendReset_Click(object sender, RoutedEventArgs e)
        {
            list_Send.Items.Clear();
        }
        //---------------------------------------------------------------------------
        private void bnHidePollDate_Click(object sender, RoutedEventArgs e)
        {
            PMC._bUsePollLog = !PMC._bUsePollLog;

            bnHidePollDate.Content = string.Format("Use Polling Log - {0}", PMC._bUsePollLog ? "ON" : "OFF");
        }
        //---------------------------------------------------------------------------
        private void cbUsePollLog_Click(object sender, RoutedEventArgs e)
        {
            PMC._bUsePollLog = (bool)cbUsePollLog.IsChecked;
        }
        //---------------------------------------------------------------------------
        private void cbDryRun_Click(object sender, RoutedEventArgs e)
        {
            //Dry Run Mode Selection
            PMC._bDryRunMode = (bool)cbDryRun.IsChecked;
        }
        //---------------------------------------------------------------------------
        private void btPortSet_Click(object sender, RoutedEventArgs e)
        {
            //
            if (cbPortState.SelectedIndex < 0) return;
            PMC.SetPortState(cbPortNo.SelectedIndex,(PORT_STATE)cbPortState.SelectedIndex);
        }
        //---------------------------------------------------------------------------
        private void btPrecSet_Click(object sender, RoutedEventArgs e)
        {
            //
            if (cbProcState.SelectedIndex < 0) return;
            PMC.SetProcState(cbProcNo.SelectedIndex,(PROCESS_STATE)cbProcState.SelectedIndex);
        }
        //---------------------------------------------------------------------------
        private void btContSet_Click(object sender, RoutedEventArgs e)
        {
            //
            if (cbContState.SelectedIndex < 0) return;
            PMC.fn_SetControlState((CONTROL_STATE)cbContState.SelectedIndex);
        }
    }
}
