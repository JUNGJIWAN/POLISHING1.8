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

using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Error : Page
    {
        //Var
        DataTable m_DataTable = new DataTable();
        int nSelectedIndex = -1;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //---------------------------------------------------------------------------
        public PageSetting_Error()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = true;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;
            fn_EnableControl(false);
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
                m_UpdateTimer.Start();
            else
                m_UpdateTimer.Stop();

        }
        //---------------------------------------------------------------------------
        public void fn_InitDataGrid()
        {
            try
            {
                m_DataTable.Clear();
                m_DataTable.Columns.Clear();
                
                m_DataTable.Columns.Add("NO");
                m_DataTable.Columns.Add("ERROR");
                m_DataTable.Columns.Add("GRADE");
                m_DataTable.Columns.Add("KIND");
                m_DataTable.Columns.Add("CAUSE");
                m_DataTable.Columns.Add("SOLUTION");

                for (int i = 0; i < EPU.ERR.Length; i++)
                {
                    m_DataTable.Rows.Add(i + 1, EPU.ERR[i]._sName, EPU.ERR[i]._nGrade, EPU.ERR[i]._nKind, EPU.ERR[i]._sCause, EPU.ERR[i]._sSolution);
                }
                dg_Error.ItemsSource = m_DataTable.DefaultView;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fn_InitDataGrid();
        }
        //---------------------------------------------------------------------------
        private void fn_EnableControl(bool bEnable)
        {
            bn_Update  .IsEnabled = bEnable;
            rb_Display .IsEnabled = bEnable;
            rb_Warnning.IsEnabled = bEnable;
            rb_Error   .IsEnabled = bEnable;
            rb_Method  .IsEnabled = bEnable;
            rb_Human   .IsEnabled = bEnable;
            rb_Machine .IsEnabled = bEnable;
            rb_Meterial.IsEnabled = bEnable;
        }
        //---------------------------------------------------------------------------
        private void bn_Update_Click(object sender, RoutedEventArgs e)
        {
            string sTemp = string.Empty;
            int row = 0;
            for(row = 0; row < m_DataTable.Rows.Count; row++)
            {
                EPU.ERR[row]._sName     = m_DataTable.Rows[row][1].ToString();
                EPU.ERR[row]._nGrade    = Convert.ToInt32(m_DataTable.Rows[row][2]);
                EPU.ERR[row]._nKind     = Convert.ToInt32(m_DataTable.Rows[row][3]);
                //EPU.ERR[row]._sCause    = m_DataTable.Rows[row][4].ToString();
                //EPU.ERR[row]._sSolution = m_DataTable.Rows[row][5].ToString();

                //JUNG/210119
                sTemp = m_DataTable.Rows[row][4].ToString();
                sTemp = sTemp.Replace('\r', '$');
                sTemp = sTemp.Replace('\n', '^');
                EPU.ERR[row]._sCause    = sTemp;
                
                sTemp = m_DataTable.Rows[row][5].ToString();
                sTemp = sTemp.Replace('\r', '$');
                sTemp = sTemp.Replace('\n', '^');
                EPU.ERR[row]._sSolution = sTemp;
            }
        }
        //---------------------------------------------------------------------------
        private void dg_Error_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nSelectedIndex = dg_Error.SelectedIndex;

            if (nSelectedIndex >= 0)
            {
                fn_EnableControl(true);

                int nCvt = 0;
                int.TryParse(m_DataTable.Rows[nSelectedIndex][2] as string, out nCvt);
                switch (nCvt)
                {
                    case 0:
                        rb_Display.IsChecked = true;
                        break;
                    case 1:
                        rb_Warnning.IsChecked = true;
                        break;
                    case 2:
                        rb_Error.IsChecked = true;
                        break;
                }
                int.TryParse(m_DataTable.Rows[nSelectedIndex][3] as string, out nCvt);
                switch (nCvt)
                {
                    case 0:
                        rb_Method.IsChecked = true;
                        break;
                    case 1:
                        rb_Human.IsChecked = true;
                        break;
                    case 2:
                        rb_Machine.IsChecked = true;
                        break;
                    case 3:
                        rb_Meterial.IsChecked = true;
                        break;
                }
                //tb_Cause.Text    = m_DataTable.Rows[nSelectedIndex][4].ToString();
                //tb_Solution.Text = m_DataTable.Rows[nSelectedIndex][5].ToString();
                
                //JUNG/210119
                string sTemp  = m_DataTable.Rows[nSelectedIndex][4].ToString();
                sTemp = sTemp.Replace('$', '\r'); //sTemp.Replace('\r', '$');
                sTemp = sTemp.Replace('^', '\n'); //sTemp.Replace('\n', '#');
                tb_Cause.Text = sTemp;

                sTemp = m_DataTable.Rows[nSelectedIndex][5].ToString();
                sTemp = sTemp.Replace('$', '\r');
                sTemp = sTemp.Replace('^', '\n');
                tb_Solution.Text = sTemp; 
            }
            else
            {
                fn_EnableControl(false);
            }
        }

        private void Grade_Checked(object sender, RoutedEventArgs e)
        {
            if(nSelectedIndex >= 0)
                m_DataTable.Rows[nSelectedIndex][2] = (sender as RadioButton).Tag;
        }

        private void Kind_Checked(object sender, RoutedEventArgs e)
        {
            if (nSelectedIndex >= 0)
                m_DataTable.Rows[nSelectedIndex][3] = (sender as RadioButton).Tag;
        }

        private void tb_Cause_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(nSelectedIndex >= 0)
                m_DataTable.Rows[nSelectedIndex][4] = tb_Cause.Text;
        }

        private void tb_Solution_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (nSelectedIndex >= 0)
                m_DataTable.Rows[nSelectedIndex][5] = tb_Solution.Text;
        }

        private void dg_Error_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
