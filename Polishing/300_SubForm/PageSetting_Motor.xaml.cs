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
using WaferPolishingSystem.BaseUnit;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Motor : Page
    {
        //Var 
        DataTable m_DataTable  = new DataTable();
        DataTable m_DataTable1 = new DataTable();

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_Motor()
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

            fn_DisplayMotorData();

            
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
        private void fn_DisplayMotorData()
        {
            //Motor Value Display
            try
            {
                m_DataTable.Clear();
                m_DataTable.Columns.Clear();

                m_DataTable.Columns.Add("NO"       );
                m_DataTable.Columns.Add("NAME"     );
                m_DataTable.Columns.Add("AUTOSPEED");
                m_DataTable.Columns.Add("MANSPEED" );
                m_DataTable.Columns.Add("HOMESPEED");
                m_DataTable.Columns.Add("JOGSPEEDH");
                m_DataTable.Columns.Add("JOGSPEEDL");
                m_DataTable.Columns.Add("ACC"      );
                m_DataTable.Columns.Add("DEC"      );
                m_DataTable.Columns.Add("INPOS"    );
                m_DataTable.Columns.Add("STOPDLY"  );
                m_DataTable.Columns.Add("USERSPD1" );
                m_DataTable.Columns.Add("USERACC1" );
                m_DataTable.Columns.Add("USERSPD2" );
                m_DataTable.Columns.Add("USERACC2" );

                int    nCnt  = MOTR._iNumOfMotr;
                string sName = string.Empty;
                
                //MAX_MOTOR
                for (int i = 0; i < nCnt; i++)
                {
                    sName = string.Format($"#{i:D2}-{MOTR[i].m_sNameAxis} [{MOTR[i].m_sName}]");
                    m_DataTable.Rows.Add(i.ToString() ,
                                         sName        ,
                                         MOTR[i].MP.dVel [(int)EN_MOTR_VEL.Work],
                                         MOTR[i].MP.dVel [(int)EN_MOTR_VEL.Dry ],
                                         MOTR[i].MP.dVel [(int)EN_MOTR_VEL.Home],
                                         MOTR[i].MP.dVel [(int)EN_MOTR_VEL.HJog],
                                         MOTR[i].MP.dVel [(int)EN_MOTR_VEL.LJog],
                                         MOTR[i].MP.dAcc [(int)EN_MOTR_VEL.Work],
                                         MOTR[i].MP.dDec [(int)EN_MOTR_VEL.Work],
                                         MOTR[i].MP.dPosn[(int)EN_POSN_ID.InPos],
                                         MOTR[i].MP.dTime[(int)EN_MOTR_DELAY.Stop],
                                         MOTR[i].MP.dVel [(int)EN_MOTR_VEL.User1],
                                         MOTR[i].MP.dAcc [(int)EN_MOTR_VEL.User1],
                                         MOTR[i].MP.dVel [(int)EN_MOTR_VEL.User2],
                                         MOTR[i].MP.dAcc [(int)EN_MOTR_VEL.User2]);
                }

                //
                dg_MotrData.ItemsSource = m_DataTable.DefaultView;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                /*
                 * GetMinPos
                 * GetMaxPos
                 * GetMinVel
                 * GetMaxVel
                 * GetMinAcc
                 * GetMaxAcc
                 */
                m_DataTable1.Clear();
                m_DataTable1.Columns.Clear();

                m_DataTable1.Columns.Add("NO"    );
                m_DataTable1.Columns.Add("NAME"  );
                m_DataTable1.Columns.Add("MINPOS");
                m_DataTable1.Columns.Add("MAXPOS");
                m_DataTable1.Columns.Add("MINVEL");
                m_DataTable1.Columns.Add("MAXVEL");
                m_DataTable1.Columns.Add("MINACC");
                m_DataTable1.Columns.Add("MAXACC");

                //MAX_MOTOR
                for (int i = 0; i < nCnt; i++)
                {
                    sName = string.Format($"#{i:D2}-{MOTR[i].m_sNameAxis} [{MOTR[i].m_sName}]");
                    m_DataTable1.Rows.Add(i.ToString()      ,
                                         sName              ,
                                         MOTR[i].GetMinPos(),
                                         MOTR[i].GetMaxPos(),
                                         MOTR[i].GetMinVel(),
                                         MOTR[i].GetMaxVel(),
                                         MOTR[i].GetMinAcc(),
                                         MOTR[i].GetMaxAcc());
                }

                //
                dg_MotrData1.ItemsSource = m_DataTable1.DefaultView;
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
                //MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .Work ] = Convert.ToDouble(m_DataTable.Rows[i]["AUTOSPEED"]);
                //MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .Dry  ] = Convert.ToDouble(m_DataTable.Rows[i]["MANSPEED" ]);
                //MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .Home ] = Convert.ToDouble(m_DataTable.Rows[i]["HOMESPEED"]);
                //MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .HJog ] = Convert.ToDouble(m_DataTable.Rows[i]["JOGSPEEDH"]);
                //MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .LJog ] = Convert.ToDouble(m_DataTable.Rows[i]["JOGSPEEDL"]);
                //MOTR[i].MP.dAcc [(int)EN_MOTR_VEL  .Work ] = Convert.ToDouble(m_DataTable.Rows[i]["ACC"      ]);
                //MOTR[i].MP.dDec [(int)EN_MOTR_VEL  .Work ] = Convert.ToDouble(m_DataTable.Rows[i]["DEC"      ]);
                //MOTR[i].MP.dPosn[(int)EN_POSN_ID   .InPos] = Convert.ToDouble(m_DataTable.Rows[i]["INPOS"    ]);
                //MOTR[i].MP.dTime[(int)EN_MOTR_DELAY.Stop ] = Convert.ToDouble(m_DataTable.Rows[i]["STOPDLY"  ]);
                //MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .User1] = Convert.ToDouble(m_DataTable.Rows[i]["USERSPD1" ]);
                //MOTR[i].MP.dAcc [(int)EN_MOTR_VEL  .User1] = Convert.ToDouble(m_DataTable.Rows[i]["USERACC1" ]);
                //MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .User2] = Convert.ToDouble(m_DataTable.Rows[i]["USERSPD2" ]);
                //MOTR[i].MP.dAcc [(int)EN_MOTR_VEL  .User2] = Convert.ToDouble(m_DataTable.Rows[i]["USERACC2" ]);
                
                double.TryParse((string)m_DataTable.Rows[i]["AUTOSPEED"], out MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .Work ]);
                double.TryParse((string)m_DataTable.Rows[i]["MANSPEED" ], out MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .Dry  ]);
                double.TryParse((string)m_DataTable.Rows[i]["HOMESPEED"], out MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .Home ]);
                double.TryParse((string)m_DataTable.Rows[i]["JOGSPEEDH"], out MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .HJog ]);
                double.TryParse((string)m_DataTable.Rows[i]["JOGSPEEDL"], out MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .LJog ]);
                double.TryParse((string)m_DataTable.Rows[i]["ACC"      ], out MOTR[i].MP.dAcc [(int)EN_MOTR_VEL  .Work ]);
                double.TryParse((string)m_DataTable.Rows[i]["DEC"      ], out MOTR[i].MP.dDec [(int)EN_MOTR_VEL  .Work ]);
                double.TryParse((string)m_DataTable.Rows[i]["INPOS"    ], out MOTR[i].MP.dPosn[(int)EN_POSN_ID   .InPos]);
                double.TryParse((string)m_DataTable.Rows[i]["STOPDLY"  ], out MOTR[i].MP.dTime[(int)EN_MOTR_DELAY.Stop ]);
                double.TryParse((string)m_DataTable.Rows[i]["USERSPD1" ], out MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .User1]);
                double.TryParse((string)m_DataTable.Rows[i]["USERACC1" ], out MOTR[i].MP.dAcc [(int)EN_MOTR_VEL  .User1]);
                double.TryParse((string)m_DataTable.Rows[i]["USERSPD2" ], out MOTR[i].MP.dVel [(int)EN_MOTR_VEL  .User2]);
                double.TryParse((string)m_DataTable.Rows[i]["USERACC2" ], out MOTR[i].MP.dAcc [(int)EN_MOTR_VEL  .User2]);
            }

            //
            double dTemp = 0.0;
            nRow = m_DataTable1.Rows.Count;
            for (int i = 0; i < nRow; i++)
            {
                double.TryParse((string)m_DataTable1.Rows[i]["MINPOS"], out dTemp); MOTR[i].SetMinPos(dTemp); FM.m_stMasterOpt.dMinPos[i] = dTemp; 
                double.TryParse((string)m_DataTable1.Rows[i]["MAXPOS"], out dTemp); MOTR[i].SetMaxPos(dTemp); FM.m_stMasterOpt.dMaxPos[i] = dTemp;
            }
            

        }


    }
}
