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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.BaseUnit.IOMap;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Manual_Spindle : Page
    {
        //Var
        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_Manual_Spindle()
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


            lbRun  .Background =  IO.XV[(int)EN_INPUT_ID.xSPD_E3000_RUN    ] ? Brushes.LimeGreen : Brushes.LightGray;
            lbRunOk.Background =  IO.XV[(int)EN_INPUT_ID.xSPD_E3000_SpeedOK] ? Brushes.LightGreen: Brushes.LightGray;
            lbWarn .Background =  IO.XV[(int)EN_INPUT_ID.xSPD_E3000_Warn   ] ? Brushes.Yellow    : Brushes.LightGray;
            lbError.Background = !IO.XV[(int)EN_INPUT_ID.xSPD_E3000_State  ] ? Brushes.Red       : Brushes.LightGray;


            lbRPM   .Content = "RPM : " + SEQ_SPIND.fn_GetSpindleSpeed().ToString();
            lbDir   .Content = SEQ_SPIND.fn_GetSpindleDir() ? "DIRECTION : FWD" : "DIRECTION : BWD";
            lbTorque.Content = "TORQUE : " + SEQ_SPIND.fn_GetSpindleTorque();



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
        private void TextBox_Cmd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = fn_IsNumeric(e.Text);
        }
        //---------------------------------------------------------------------------
        private void bnStart_Click(object sender, RoutedEventArgs e)
        {
            //
            Button selBtn = sender as Button;
            string sName  = selBtn.Name;
            bool   bCCW   = rbDirCCW.IsChecked == true;
            int    nRPM   = 0;
            
            int.TryParse(tbRPMSet.Text, out nRPM);

            switch (sName)
            {
                case "bnStart":
                    //SEQ_SPIND.fn_SetSpindleSpeed(nRPM);
                    //SEQ_SPIND.fn_SetSpindleRun  (swOn, bCW);
                    SEQ_SPIND.fn_SetSpindleRun(nRPM, bCCW);
                    break;
                case "bnStop":
                    SEQ_SPIND.fn_SetSpindleRun(swOff);
                    break;
                case "bnClamp":
                    SEQ_SPIND.fn_MoveToolClamp(ccFwd);
                    break;
                case "bnUnclamp":
                    SEQ_SPIND.fn_MoveToolClamp(ccBwd);
                    break;
                case "bnReset":
                    SEQ_SPIND.fn_SetSpindleReset();
                    break;

                default:
                    break;
            }

            fn_WriteLog($"Spindle Manual Button Click - [{sName}]");

        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //
            m_UpdateTimer.IsEnabled = true;


        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.IsEnabled = false;
        }
    }
}
