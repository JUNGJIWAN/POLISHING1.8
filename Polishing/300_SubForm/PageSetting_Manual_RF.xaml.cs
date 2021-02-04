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

namespace WaferPolishingSystem.Form
{
    public partial class PageSetting_Manual_RF : Page
    {
        //Var


        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        

        //---------------------------------------------------------------------------
        public PageSetting_Manual_RF()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;


        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();


            lbConnect.Background = RFID._bConnect ? Brushes.Lime : Brushes.Gray;
            
            lbState.Content = RFID._sState;


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
            {
                m_UpdateTimer.Stop();
            }
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Timer On
            fn_SetTimer(true);

            tbRFIDIP.Text = FM.m_stSystemOpt.sRFIDIp;
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            fn_SetTimer(false);

        }
        //---------------------------------------------------------------------------
        private void bnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (RFID.fn_Connect())
            {
                lbConnect.Background = Brushes.Lime;
            }
            else
            {
                lbConnect.Background = Brushes.Red;
            }

        }
        //---------------------------------------------------------------------------
        private void bnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            //client.Close();
            RFID.fn_DisConnect();
            lbConnect.Background = Brushes.Red;
        }
        //---------------------------------------------------------------------------
        private void btRead_Click(object sender, RoutedEventArgs e)
        {
            string temp = string.Empty;
            //
            if (RFID.fn_Read())
            {
                temp = string.Format($"[{DateTime.Now:HH:mm:ss}] {RFID._sReadData}");
            }
            else
            {
                temp = string.Format($"[{DateTime.Now:HH:mm:ss}] {RFID._sState}");
            }

            list_Read.Items.Insert(0, temp);


        }
        //---------------------------------------------------------------------------
        private void btWrite_Click(object sender, RoutedEventArgs e)
        {
            if(!RFID.fn_Write(tbWriteData.Text))
            {
                lbState.Content = RFID._sState; 
            }
        }
        //---------------------------------------------------------------------------
        private void btRFSave_Click(object sender, RoutedEventArgs e)
        {
            //
            FM.m_stSystemOpt.sRFIDIp = tbRFIDIP.Text.Trim();
            RFID.fn_SetIP(FM.m_stSystemOpt.sRFIDIp);
            FM.fn_LoadSysOptn(false);

            fn_WriteLog("RFID IP Address Change");
        }
        //---------------------------------------------------------------------------
        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            list_Read.Items.Clear();
        }
    }
}
