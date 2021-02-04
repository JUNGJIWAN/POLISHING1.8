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
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.Define.UserEnum;


namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Manual_Valve : Page
    {
        //Var
        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_Manual_Valve()
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

            btUtil01.Background = IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1    ] ? Brushes.LimeGreen : Brushes.LightGray;
            btUtil02.Background = IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2    ] ? Brushes.LimeGreen : Brushes.LightGray;
            btUtil03.Background = IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3    ] ? Brushes.LimeGreen : Brushes.LightGray;
            btSoap  .Background = IO.YV[(int)EN_OUTPUT_ID.yPLS_Pump_Soap    ] ? Brushes.LimeGreen : Brushes.LightGray;


            btDIPol .Background = IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater] ? Brushes.LimeGreen : Brushes.LightGray;

            btDICln .Background = IO.YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater] ? Brushes.LimeGreen : Brushes.LightGray;


            lbUseAutoSupply.Foreground = FM.m_stSystemOpt.nUseAutoSlurry == 1 ? Brushes.Yellow : Brushes.Black;



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
        //---------------------------------------------------------------------------
        private void btUtil01_Click(object sender, RoutedEventArgs e)
        {
            //
            Button selBtn = sender as Button;
            string sName = selBtn.Name;
            int    nTag = 0; 

            switch (sName)
            {
                case "btUtil01":
                    if(IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1]) SEQ_POLIS.fn_StopUtil();
                    else                                        SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.Silica01);
                    break;

                case "btUtil02":
                    if (IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si2]) SEQ_POLIS.fn_StopUtil();
                    else                                         SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.Silica02);
                    break;

                case "btUtil03":
                    if (IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si3]) SEQ_POLIS.fn_StopUtil();
                    else                                         SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.Silica03);
                    break;

                case "btSoap":
                    if (IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap]) SEQ_POLIS.fn_StopUtil();
                    else                                          SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.Soap);
                    break;

                case "btDIPol" :
                    if(IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_DIWater]) SEQ_POLIS.fn_SetDIWaterValve(false);
                    else                                            SEQ_POLIS.fn_SetDIWaterValve(true );
                    break;
                    
                case "btUtilStop":
                    SEQ_POLIS.fn_StopUtil();
                    break;

                case "btDICln":
                    if (IO.YV[(int)EN_OUTPUT_ID.yCLN_Valve_DIWater]) SEQ_CLEAN.fn_SetDIWaterValve(false);
                    else                                             SEQ_CLEAN.fn_SetDIWaterValve(true);
                    
                    break;

                case "btDrainPol":
                    //nTag = (int)EN_MAN_LIST.MAN_0421;
                    //MAN.fn_ManProcOn(nTag, true, false);

                    SEQ_POLIS.fn_SetDrain();

                    fn_WriteLog(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));
                    Console.WriteLine(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));
                    break;

                case "btDrainCln":
                    //nTag = (int)EN_MAN_LIST.MAN_0431;
                    //MAN.fn_ManProcOn(nTag, true, false);

                    SEQ_CLEAN.fn_SetDrain();

                    fn_WriteLog(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));
                    Console.WriteLine(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));
                    break;

                case "btLeakDrain":
                    if (IO.YV[(int)EN_OUTPUT_ID.yPLS_LeakDrain])  SEQ_POLIS.fn_SetLeakDrain(false);
                    else                                          SEQ_POLIS.fn_SetLeakDrain(true );

                    break;
                case "btSuckBack":
                    SEQ_POLIS.fn_SetSuckBackOn();
                    break;

                case "btSlurryDI":
                    if(IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Si1]) SEQ_POLIS.fn_StopUtil();
                    else                                        SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.SilicaDI);

                    break;
                
                case "btSoapDI":
                    if (IO.YV[(int)EN_OUTPUT_ID.yPLS_Valve_Soap]) SEQ_POLIS.fn_StopUtil();
                    else                                          SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.SoapDI);

                    break;

                default:
                    break;
            }

        }
    }
}
