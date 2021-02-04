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
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.BaseUnit.ActuatorId;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Actr : Page
    {
        //Var 
        DataTable m_DataTable  = new DataTable();
        
        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        private int m_nSelIndex ; 

        public PageSetting_Actr()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            m_nSelIndex = 0;
        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            
            if (m_nSelIndex >= 0 && m_nSelIndex < ACTR._iNumOfACT) // Index Check
            {
                lbFWD  .Background = ACTR[m_nSelIndex].Complete(ccFwd) ? Brushes.Lime : Brushes.LightGray;
                lbBWD  .Background = ACTR[m_nSelIndex].Complete(ccBwd) ? Brushes.Lime : Brushes.LightGray;
                lbALARM.Background = ACTR[m_nSelIndex].GetTimeOut   () ? Brushes.Red  : Brushes.LightGray;
            }

            btStart.Background = MAN._bRptActrIng ? Brushes.LimeGreen : Brushes.LightGray;

            lbRetyCnt.Content = "[Retry Count] Fwd : " + ACTR[m_nSelIndex].GetRetryCntFwd() + "/Bwd : "+ ACTR[m_nSelIndex].GetRetryCntFwd() + "/Total" + ACTR[m_nSelIndex].GetRetryCnt();


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
            fn_SetTimer(true);

            fn_DisplayActrData();
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //
            fn_SetTimer(false);

        }
        //---------------------------------------------------------------------------
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //
        }
        //---------------------------------------------------------------------------
        private void fn_DisplayActrData()
        {
            //Motor Value Display
            try
            {
                m_DataTable.Clear();
                m_DataTable.Columns.Clear();
                
                m_DataTable.Columns.Add("No"          );
                m_DataTable.Columns.Add("Name"        );
                m_DataTable.Columns.Add("Comment"     );
                m_DataTable.Columns.Add("xFwdID"      );
                m_DataTable.Columns.Add("xBwdID"      );
                m_DataTable.Columns.Add("yFwdID"      );
                m_DataTable.Columns.Add("yBwdID"      );
                m_DataTable.Columns.Add("ApplyTimeOut");
                m_DataTable.Columns.Add("FwdOnDelay"  );
                m_DataTable.Columns.Add("BwdOnDelay"  );
                m_DataTable.Columns.Add("FwdTimeOut"  );
                m_DataTable.Columns.Add("BwdTimeOut"  );


                int nCnt  = ACTR._iNumOfACT;
                string sName = string.Empty;
                
                //Actuator
                for (int i = 0; i < nCnt; i++)
                {
                    m_DataTable.Rows.Add(i.ToString() ,
                                         ACTR[i].GetName               (),
                                         ACTR[i].GetComt               (),
                                         ACTR[i].GetxfwdId             (),
                                         ACTR[i].GetxbwdId             (),
                                         ACTR[i].GetyfwdId             (),
                                         ACTR[i].GetybwdId             (),
                                         ACTR[i].GetApplyTimeout       ()? 1 : 0,
                                         ACTR[i].GetFwdOnDelayTime     (),
                                         ACTR[i].GetBwdOnDelayTime     (),
                                         ACTR[i].GetFwdTimeOutDelayTime(),
                                         ACTR[i].GetBwdTimeOutDelayTime());
                }

                //
                dg_ActrData.ItemsSource = m_DataTable.DefaultView;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        public void fn_GridToBuff()
        {
            //copy Grid Data to Buffer
            int nRow = m_DataTable.Rows.Count;

            for (int i = 0; i < nRow; i++)
            {
                //ACTR[i].SetName               (Convert.ToString (m_DataTable.Rows[i]["Name"        ]));
                ACTR[i].SetComt               (Convert.ToString (m_DataTable.Rows[i]["Comment"     ]));
                ACTR[i].SetxfwdId             (Convert.ToInt32  (m_DataTable.Rows[i]["xFwdID"      ]));
                ACTR[i].SetxbwdId             (Convert.ToInt32  (m_DataTable.Rows[i]["xBwdID"      ]));
                ACTR[i].SetyfwdId             (Convert.ToInt32  (m_DataTable.Rows[i]["yFwdID"      ]));
                ACTR[i].SetybwdId             (Convert.ToInt32  (m_DataTable.Rows[i]["yBwdID"      ]));
                ACTR[i].SetApplyTimeout       (Convert.ToInt32  (m_DataTable.Rows[i]["ApplyTimeOut"])==1? true : false );
                ACTR[i].SetFwdOnDelayTime     (Convert.ToInt32  (m_DataTable.Rows[i]["FwdOnDelay"  ]));
                ACTR[i].SetBwdOnDelayTime     (Convert.ToInt32  (m_DataTable.Rows[i]["BwdOnDelay"  ]));
                ACTR[i].SetFwdTimeOutDelayTime(Convert.ToInt32  (m_DataTable.Rows[i]["FwdTimeOut"  ]));
                ACTR[i].SetBwdTimeOutDelayTime(Convert.ToInt32  (m_DataTable.Rows[i]["BwdTimeOut"  ]));
            }
        }
        //---------------------------------------------------------------------------
        private void dg_ActrData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
            int nSel = dg_ActrData.SelectedIndex;

            if (nSel < 0 || nSel > ACTR._iNumOfACT) return; 

            m_nSelIndex = nSel;

            lbSelIdex.Content = string.Format($"[{m_nSelIndex:D2}] {ACTR[nSel].GetName()}"); 



        }
        //---------------------------------------------------------------------------
        private void btFwd_Click(object sender, RoutedEventArgs e)
        {
            if (SEQ._bRun || SEQ._bAuto)
            {
                UserFunction.fn_UserMsg("Can't Work while RUN or Auto");
                return;
            }

            //
            EN_ACTR_LIST acList = (EN_ACTR_LIST)m_nSelIndex;

            ACTR.MoveCyl(acList, ccFwd);

        }
        //---------------------------------------------------------------------------
        private void btBwd_Click(object sender, RoutedEventArgs e)
        {

            if (SEQ._bRun || SEQ._bAuto)
            {
                UserFunction.fn_UserMsg("Can't Work while RUN or Auto");
                return;
            }
            //
            EN_ACTR_LIST acList = (EN_ACTR_LIST)m_nSelIndex;

            ACTR.MoveCyl(acList, ccBwd);

        }
        //---------------------------------------------------------------------------
        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            SEQ.fn_Reset();
        }
        //---------------------------------------------------------------------------
        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            //
            if(SEQ._bRun || SEQ._bAuto)
            {
                UserFunction.fn_UserMsg("Can't Work while RUN or Auto");
                return; 
            }
            if (FM.fn_GetLevel() != (int)EN_USER_LEVEL.lvMaster)
            {
                UserFunction.fn_UserMsg("Please Change Master Level.");
                return;
            }
            int nDly = 0; 
            int.TryParse(upStopDelay.UPValue, out nDly);

            bool isOk = ACTR.MoveCyl(m_nSelIndex, (int)EN_ACTR_CMD.Fwd);

            if (isOk)
            {
                ACTR.fn_Reset();
                MAN.fn_SetRptAct(-1         , false, nDly);
                MAN.fn_SetRptAct(m_nSelIndex, true , nDly);
            }
        }
        //---------------------------------------------------------------------------
        private void btEnd_Click(object sender, RoutedEventArgs e)
        {
            MAN.fn_SetRptAct(-1, false, 0);
        }
        //---------------------------------------------------------------------------

    }
}
