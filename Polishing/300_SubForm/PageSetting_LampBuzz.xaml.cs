using System;
using System.Collections.Generic;
using System.Data;
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
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_LampBuzz : Page
    {
        //Var
        bool m_bLampOn;
        DataTable m_DataTable = new DataTable();

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_LampBuzz()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            m_bLampOn = false;


        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            if(LAMP.IsSetLamp() && LAMP.IsSetBuzz())
            { 
                lbTestRed.Background = IO.YV[(int)LAMP._iYLempRed] ? Brushes.Red    : Brushes.LightGray;
                lbTestYel.Background = IO.YV[(int)LAMP._iYLempYel] ? Brushes.Yellow : Brushes.LightGray;
                lbTestGrn.Background = IO.YV[(int)LAMP._iYLempGrn] ? Brushes.Green  : Brushes.LightGray;

                m_bLampOn = IO.YV[(int)LAMP._iYBuzz1]; //|| IO.YV[(int)LAMP._iYBuzz2] || IO.YV[(int)LAMP._iYBuzz3];
            }

            lbTestBuz.Background = m_bLampOn ? Brushes.CornflowerBlue : Brushes.LightGray;
            lbTestBuz.Content    = m_bLampOn ? "BUZZ ON" : "OFF";

            lbTesting.Background = (LAMP._bTest && SEQ._bFlick1)? Brushes.Lime : Brushes.LightGray;
            lbTesting.Content    = (LAMP._bTest && SEQ._bFlick1)? "Now Testing..." : "Normal";


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
        private void fn_DisplayLampBuzz()
        {

            //Lamp & Buzzer Value Display
            try
            {
                m_DataTable.Clear();
                m_DataTable.Columns.Clear();
                
                m_DataTable.Columns.Add("KIND"  );
                m_DataTable.Columns.Add("RED"   );
                m_DataTable.Columns.Add("GREEN" );
                m_DataTable.Columns.Add("YELLOW");
                m_DataTable.Columns.Add("BUZZER");
                m_DataTable.Columns.Add("TEST"  );

                int nCnt = (int)EN_SEQ_STATE.EndofId;
                string sKind = string.Empty;
                
                //MAX_SEQ_STATE
                for (int i = 0; i < nCnt; i++)
                {
                    sKind = ((EN_SEQ_STATE)i).ToString();

                    m_DataTable.Rows.Add(sKind,
                                         LAMP[i].iRed ,
                                         LAMP[i].iGrn ,
                                         LAMP[i].iYel ,
                                         LAMP[i].iBuzz,
                                         "TEST");
                }

                //
                dg_LampBuzzData.ItemsSource = m_DataTable.DefaultView;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fn_DisplayLampBuzz();

            fn_SetTimer(true); //Timer On
        }
        //---------------------------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Test Button 
            int nSelKind = dg_LampBuzzData.SelectedIndex;
            if (nSelKind < 0) return;

            LAMP._bTest     = true;
            LAMP._iTestStat = nSelKind;

        }
        //---------------------------------------------------------------------------
        public void fn_GridToBuff()
        {
            //copy Grid Data to Buffer
            int nRow = m_DataTable.Rows.Count;

            for (int n = 0; n < nRow; n++)
            {
                LAMP.m_LampInfo[n].iRed  = Convert.ToInt32(m_DataTable.Rows[n]["RED"   ]);
                LAMP.m_LampInfo[n].iGrn  = Convert.ToInt32(m_DataTable.Rows[n]["GREEN" ]);
                LAMP.m_LampInfo[n].iYel  = Convert.ToInt32(m_DataTable.Rows[n]["YELLOW"]);
                LAMP.m_LampInfo[n].iBuzz = Convert.ToInt32(m_DataTable.Rows[n]["BUZZER"]);
            }

        }
        //---------------------------------------------------------------------------
        private void btTestEnd_Click(object sender, RoutedEventArgs e)
        {
            //TEST End
            LAMP._bTest = false; 
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            fn_SetTimer(false);
        }
    }
}
