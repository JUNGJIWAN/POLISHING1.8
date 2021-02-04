using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using WaferPolishingSystem.Define;
using System.Windows.Threading;

namespace WaferPolishingSystem
{
    /// <summary>
    /// FormAlarm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormAlarmDlg : Window
    {
        
        //
        private bool m_bClose     ;
        private int  m_nSelSubPart; 

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        
        public bool _bClose     { set { m_bClose      = value; } }
        public int _nSelSubPart { set { m_nSelSubPart = value; } }


        //---------------------------------------------------------------------------
        public FormAlarmDlg()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            m_nSelSubPart = 0;

            //tab Visibility
            tab01.Visibility = Visibility.Hidden;
            tab02.Visibility = Visibility.Hidden;



        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.IsEnabled = false;

            this.Background = SEQ._bFlick1 ? Brushes.Yellow : Brushes.Red;

            m_UpdateTimer.IsEnabled = true;
        }

        //---------------------------------------------------------------------------
        //Alarm Info Setting
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //fn_DisplayErr();
        }
        //---------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!m_bClose)
            {
                e.Cancel = true;
                this.Hide();
            }

            m_UpdateTimer.IsEnabled = false;
        }
        
        //---------------------------------------------------------------------------
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            //
            UserFunction.fn_WriteLog("ALARM - Close Click");
            this.Hide();

            m_UpdateTimer.IsEnabled = false;

        }
        //---------------------------------------------------------------------------
        private void btBZoff_Click(object sender, RoutedEventArgs e)
        {
            //Buzzer Off
            LAMP.fn_BuzzOff();

            UserFunction.fn_WriteLog("ALARM - BuzzerOff Click");

        }
        //---------------------------------------------------------------------------
        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            //Reset Click
            UserFunction.fn_WriteLog("ALARM - Reset Click");

            SEQ.fn_Reset();
            this.Hide();
        }
        //---------------------------------------------------------------------------
        private void fn_DisplayErr()
        {
            //Var.
            string sTemp = string.Empty;
            int    nErrNo = EPU.fn_GetLastErrNo(false);
            
            tbSubAlarm.Visibility = Visibility.Hidden;

            //Check
            if (nErrNo < 0 || nErrNo > MAX_ERROR) return; 

            lbErrNo      .Content = string.Format($"{nErrNo+1:D4}");
            lbErrName    .Content = EPU.ERR[nErrNo]._sName         ;
            lbErrCause   .Content = EPU.ERR[nErrNo]._sCause        ;
            //lbErrSolution.Content = EPU.ERR[nErrNo]._sSolution     ;

            //JUNG/210126
            sTemp = EPU.ERR[nErrNo]._sSolution;
            sTemp = sTemp.Replace('$', '\r'); //sTemp.Replace('\r', '$');
            sTemp = sTemp.Replace('^', '\n'); //sTemp.Replace('\n', '#');
            lbErrSolution.Content = sTemp;

            //List Setting
            lbErrList.Items.Clear();
            

            for (int i = 0; i < MAX_ERROR; i++)
            {
                if (i == nErrNo) continue; 
                if (EPU.ERR[i]._bOn && EPU.ERR[i]._nGrade == (int)EN_ERR_GRADE.egError)
                {
                    sTemp = string.Format($"[ERR_{i+1:D4}] ");
                    sTemp += EPU.ERR[i]._sName;
                    lbErrList.Items.Insert(0, sTemp);
                }
            }

            //Check Sub Alarm
            if(m_nSelSubPart> 0)
            {
                tbSubAlarm.SelectedIndex = m_nSelSubPart;
                tbSubAlarm.Visibility = Visibility.Visible;

                btReset.IsEnabled = false;
                btClose.IsEnabled = false; 
            }
            else
            {
                btReset.IsEnabled = true;
                btClose.IsEnabled = true;
            }

            //
            m_UpdateTimer.IsEnabled = true;
        }
        //---------------------------------------------------------------------------
        private void Window_Activated(object sender, EventArgs e)
        {
            fn_DisplayErr();
        }
    }
}
