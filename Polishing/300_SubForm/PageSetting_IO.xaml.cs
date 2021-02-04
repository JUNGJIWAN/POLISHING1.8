using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using UserInterface;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;
using System.Windows.Threading;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_IO : Page
    {
        DataTable  _dtX        = new DataTable();
        DataTable  _dtY        = new DataTable();

        DataView   _dvX        = new DataView();
        DataView   _dvY        = new DataView();
        DataView   _dvXDisplay = new DataView();
        DataView   _dvYDisplay = new DataView();


        DataTable  _dtAI       = new DataTable();
        DataTable  _dtAO       = new DataTable();

        DataView   _dvAI       = new DataView();
        DataView   _dvAO       = new DataView();

        DataView   _dvAIDis    = new DataView();
        DataView   _dvAODis    = new DataView();

        UserButton _bnPrevX, _bnPrevY;
        UserButton _bnPrevXDisplay, _bnPrevYDisplay;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();


        public PageSetting_IO()
        {
            InitializeComponent();
            System.Windows.Interop.ComponentDispatcher.ThreadIdle += m_fn_Init;

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

        }
        //---------------------------------------------------------------------------
        void m_fn_Init(object sender, EventArgs e)
        {
            System.Windows.Interop.ComponentDispatcher.ThreadIdle -= m_fn_Init;
            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            ti_Display.Visibility = Visibility.Hidden;
            ti_Edit   .Visibility = Visibility.Hidden;
            ti_Analog .Visibility = Visibility.Hidden;

            _dtX.TableName = "table";
            _dtX.Columns.Add("NO"    );
            _dtX.Columns.Add("ADDR"  );
            _dtX.Columns.Add("IONAME");
            _dtX.Columns.Add("STAT"  );
            _dtX.Columns.Add("INV"   );
            _dtX.Columns.Add("BACK"  );
            

            _dtY.TableName = "table2";
            _dtY.Columns.Add("NO"    );
            _dtY.Columns.Add("ADDR"  );
            _dtY.Columns.Add("IONAME");
            _dtY.Columns.Add("STAT"  );
            _dtY.Columns.Add("INV"   );
            _dtY.Columns.Add("BACK"  );
            _dtY.Columns.Add("YV"    );


            _dtAI.TableName = "table_AI";
            _dtAI.Columns.Add("NO"    );
            _dtAI.Columns.Add("ADDR"  );
            _dtAI.Columns.Add("IONAME");
            _dtAI.Columns.Add("VALUE" );
            _dtAI.Columns.Add("VOLT"  );

            _dtAO.TableName = "table_AO";
            _dtAO.Columns.Add("NO"    );
            _dtAO.Columns.Add("ADDR"  );
            _dtAO.Columns.Add("IONAME");
            _dtAO.Columns.Add("VALUE" );
            _dtAO.Columns.Add("VOLT"  );

        }

        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fn_GetListX(0, MAX_INPUT_COUNT_D ); //Max of IO
            fn_GetListY(0, MAX_OUTPUT_COUNT_D); //

            fn_GetListAI(0, MAX_INPUT_COUNT_A ); //
            fn_GetListAO(0, MAX_OUTPUT_COUNT_A); //

            _dvX.Table = _dtX;
            dg_IOX.ItemsSource = _dvX;

            _dvXDisplay.Table = _dtX;
            ic_xIO.ItemsSource = _dvXDisplay;

            _dvY.Table = _dtY;
            dg_IOY.ItemsSource = _dvY;

            _dvYDisplay.Table = _dtY;
            ic_yIO.ItemsSource = _dvYDisplay;

            //
            _dvAI.Table = _dtAI;
            dg_AI.ItemsSource = _dvAI;

            _dvAO.Table = _dtAO;
            dg_AO.ItemsSource = _dvAO;


            bt_xio_Click(bt_xio1, null);
            bt_yio_Click(bt_yio1, null);

            bt_xioDisplay_Click(bt_xio1Display, null);
            bt_yioDisplay_Click(bt_yio1Display, null);

            bt_Display_Click(null, null);


            m_UpdateTimer.IsEnabled = true;
            m_UpdateTimer.Start();
        }
        //---------------------------------------------------------------------------
        void fn_GetListX(int start, int end)
        {
            _dtX.Clear();
            for(int i = start; i < end; i++)
            {
                _dtX.Rows.Add(i, IO.XA[i], IO.XName[i], IO.XV[i] ? "ON" : "OFF", IO.XInv[i], IO.XV[i] ? "Lime" : "LightGray");
            }
        }
        //---------------------------------------------------------------------------
        void fn_GetListY(int start, int end)
        {
            _dtY.Clear();
            for (int i = start; i < end; i++)
            {
                _dtY.Rows.Add(i, IO.YA[i], IO.YName[i], IO.YR[i] ? "ON" : "OFF", IO.YInv[i], IO.YR[i] ? "Lime" : "LightGray", IO.YV[i] ? "Lime" : "LightGray");
            }
        }
        //---------------------------------------------------------------------------
        void fn_GetListAI(int start, int end)
        {
            _dtAI.Clear();
            for (int i = start; i < end; i++)
            {
                _dtAI.Rows.Add(i, $"AI{i:D4}", IO.AIName[i], IO.AI[i], IO.fn_GetVoltage(true, i));
            }
        }
        //---------------------------------------------------------------------------
        void fn_GetListAO(int start, int end)
        {
            _dtAO.Clear();
            for (int i = start; i < end; i++)
            {
                _dtAO.Rows.Add(i, $"AO{i:D4}", IO.AOName[i], IO.AO[i], IO.fn_GetVoltage(false, i));
            }
        }

        //---------------------------------------------------------------------------
        private void bt_Display_Click(object sender, RoutedEventArgs e)
        {
            tab_IO.SelectedIndex = 0;
            bt_Edit.Background    = G_COLOR_BTNNORMAL;
            bt_Display.Background = G_COLOR_BTNCLICKED;
            bt_Analog.Background  = G_COLOR_BTNNORMAL;
        }
        //---------------------------------------------------------------------------
        private void bt_Edit_Click(object sender, RoutedEventArgs e)
        {
            tab_IO.SelectedIndex = 1;
            bt_Display.Background = G_COLOR_BTNNORMAL;
            bt_Edit.Background    = G_COLOR_BTNCLICKED;
            bt_Analog.Background  = G_COLOR_BTNNORMAL;
        }
        //---------------------------------------------------------------------------
        private void fn_GetStartIndex(UserButton bn, out int nStart, out int nEnd)
        {
            string strStart, strEnd;
            if (bn != null)
            {
                strStart = bn.Content.ToString().Substring(0, bn.Content.ToString().IndexOf('~'));
                strEnd   = bn.Content.ToString().Substring(bn.Content.ToString().LastIndexOf('~') + 1);
                int.TryParse(strStart, out nStart);
                int.TryParse(strEnd  , out nEnd  ); 
                if (nStart <    0) nStart = 0;
                if (nEnd   >= 256) nEnd   = 256;

                if((nEnd - nStart) > 32)
                {
                    nEnd = nStart + 32;
                }
            }
            else
            {
                nStart = 0;
                nEnd   = 31;
            }
        }
        //---------------------------------------------------------------------------
        private void bt_xio_Click(object sender, RoutedEventArgs e)
        {
            UserButton bn = sender as UserButton;
            if (_bnPrevX != null)
                _bnPrevX.Background = G_COLOR_BTNNORMAL;
            bn.Background = G_COLOR_BTNCLICKED;
            int nStart = 0, nEnd = 31;
            fn_GetStartIndex(bn, out nStart, out nEnd);
            _dvX.RowFilter = "NO >= " + nStart + " AND NO <= " + nEnd;
            _bnPrevX = bn;
        }
        //---------------------------------------------------------------------------
        private void bt_yio_Click(object sender, RoutedEventArgs e)
        {
            UserButton bn = sender as UserButton;
            if (_bnPrevY != null)
                _bnPrevY.Background = G_COLOR_BTNNORMAL;
            bn.Background = G_COLOR_BTNCLICKED;
            int nStart = 1, nEnd = 32;
            fn_GetStartIndex(bn, out nStart, out nEnd);
            _dvY.RowFilter = "NO >= " + nStart + " AND NO <= " + nEnd;
            _bnPrevY = bn;
        }
        //---------------------------------------------------------------------------
        private void bt_xioDisplay_Click(object sender, RoutedEventArgs e)
        {
            UserButton bn = sender as UserButton;
            if (_bnPrevXDisplay != null) _bnPrevXDisplay.Background = G_COLOR_BTNNORMAL;
            
            bn.Background = G_COLOR_BTNCLICKED;
            
            int nStart = 0, nEnd = 31;
            
            fn_GetStartIndex(bn, out nStart, out nEnd);
            _dvXDisplay.RowFilter = "NO >= " + nStart + " AND NO <= " + nEnd;
            _bnPrevXDisplay = bn;
        }
        //---------------------------------------------------------------------------
        private void bt_yioDisplay_Click(object sender, RoutedEventArgs e)
        {
            UserButton bn = sender as UserButton;
            if (_bnPrevYDisplay != null)
                _bnPrevYDisplay.Background = G_COLOR_BTNNORMAL;
            bn.Background = G_COLOR_BTNCLICKED;
            int nStart = 1, nEnd = 32;
            fn_GetStartIndex(bn, out nStart, out nEnd);
            _dvYDisplay.RowFilter = "NO >= " + nStart + " AND NO <= " + nEnd;
            _bnPrevYDisplay = bn;
        }
        //---------------------------------------------------------------------------
        //Inverse Setting
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int nStart = 0, nEnd = 32;
                fn_GetStartIndex(_bnPrevY, out nStart, out nEnd);
                //int idx = dg_IOY.SelectedIndex + (nStart - 1);
                int idx = dg_IOY.SelectedIndex + (nStart);

                if (idx >= 0)
                {
                    IO.YInv[idx] ^= IO.YInv[idx];

                    TextBox tb = sender as TextBox;
                    if(tb.Text == "0")
                    {
                        //_dtY.Rows[idx]["STAT"] = "OFF";
                        _dtY.Rows[idx]["INV"] = 0;
                        
                        //_dtY.Rows[idx]["BACK"] = "LightGray";
                    }
                    else
                    {
                        //_dtY.Rows[idx]["STAT"] = "ON";
                        _dtY.Rows[idx]["INV"] = 1;
                        //_dtY.Rows[idx]["BACK"] = "Lime";
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        //Digital Output Button Click 시....
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nStart = 0, nEnd = 32;
                fn_GetStartIndex(_bnPrevYDisplay, out nStart, out nEnd);

                Button btSel = sender as Button;
                string sAdd = btSel.Content.ToString();

                int nAddNo = Array.FindIndex(IO.YA, n => n == sAdd);

                IO.YV[nAddNo] = !IO.YV[nAddNo]; 

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }
        //---------------------------------------------------------------------------
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                int nStart = 0, nEnd = 32;
                fn_GetStartIndex(_bnPrevX, out nStart, out nEnd);
                //int idx = dg_IOX.SelectedIndex + (nStart - 1);
                int idx = dg_IOX.SelectedIndex + (nStart);

                if (idx >= 0)
                {
                    TextBox tb = sender as TextBox;
                    if (tb.Text != "0" && tb.Text != "1") return; 
                    if (tb.Text == "0")
                    {
                        _dtX.Rows[idx]["INV"] = 0;
                        
                        IO.XInv  [idx] = 0;
                    }
                    else
                    {
                        _dtX.Rows[idx]["INV"] = 1;
                        
                        IO.XInv  [idx] = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        
        //---------------------------------------------------------------------------
        private void bt_DataGridClickY(object sender, RoutedEventArgs e)
        {
            try
            {
                int nStart = 0, nEnd = 32;
                fn_GetStartIndex(_bnPrevY, out nStart, out nEnd);
                //int idx = dg_IOY.SelectedIndex + (nStart - 1);
                int idx = dg_IOY.SelectedIndex + (nStart);

                if (idx >= 0)
                {
                    //
                    IO.YV[idx] = !IO.YV[idx];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //Timer Off
            m_UpdateTimer.Stop();
        }
        //---------------------------------------------------------------------------
        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            //IO Name Change
            try
            {
                int nStart = 0, nEnd = 32;
                fn_GetStartIndex(_bnPrevY, out nStart, out nEnd);
                //int idx = dg_IOY.SelectedIndex + (nStart - 1);
                int idx = dg_IOY.SelectedIndex + (nStart);

                if (idx >= 0)
                {
                    TextBox tb = sender as TextBox;
                    string sIOName = tb.Text;

                    //
                    IO.YName[idx] = sIOName;

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }
        //---------------------------------------------------------------------------
        private void TextBox_XIONameChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int nStart = 0, nEnd = 32;
                fn_GetStartIndex(_bnPrevX, out nStart, out nEnd);
                //int idx = dg_IOX.SelectedIndex + (nStart - 1);
                int idx = dg_IOX.SelectedIndex + (nStart);

                if (idx >= 0)
                {
                    TextBox tb = sender as TextBox;
                    string sIOName = tb.Text;

                    //
                    IO.XName[idx] = sIOName;

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }


        }
        //---------------------------------------------------------------------------
        private void bt_Analog_Click(object sender, RoutedEventArgs e)
        {
            tab_IO.SelectedIndex = 2;
            bt_Display.Background = G_COLOR_BTNNORMAL;
            bt_Edit.Background    = G_COLOR_BTNNORMAL;
            bt_Analog.Background  = G_COLOR_BTNCLICKED;
        }
        //---------------------------------------------------------------------------
        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {//Analog Input
            try
            {
                int idx = dg_AI.SelectedIndex;

                if (idx >= 0)
                {
                    TextBox tb = sender as TextBox;
                    string sIOName = tb.Text;

                    //
                    IO.AIName[idx] = sIOName;

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }
        //---------------------------------------------------------------------------
        private void TextBox_TextChanged_4(object sender, TextChangedEventArgs e)
        {//Analog Out
            try
            {
                int idx = dg_AO.SelectedIndex;

                if (idx >= 0)
                {
                    TextBox tb = sender as TextBox;
                    string sIOName = tb.Text;

                    //
                    IO.AOName[idx] = sIOName;

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }

        //---------------------------------------------------------------------------
        public void fn_DataUpdate()
        {
            //Data Table 만 갱신
            for (int i = 0; i < MAX_INPUT_COUNT_D; i++)
            {
                _dtX.Rows[i]["STAT"] = IO.XV[i] ? "ON"   : "OFF"      ;
                _dtX.Rows[i]["BACK"] = IO.XV[i] ? "Lime" : "LightGray";
            }

            for (int i = 0; i < MAX_OUTPUT_COUNT_D; i++)
            {
                _dtY.Rows[i]["STAT"] = IO.YR[i] ? "ON"   : "OFF"      ;
                _dtY.Rows[i]["BACK"] = IO.YR[i] ? "Lime" : "LightGray";
                _dtY.Rows[i]["YV"  ] = IO.YV[i] ? "Lime" : "LightGray";
            }

            //
            for (int i = 0; i < MAX_INPUT_COUNT_A; i++)
            {
                _dtAI.Rows[i]["VALUE"] = IO.AI[i];
                _dtAI.Rows[i]["VOLT" ] = IO.fn_GetVoltage(true, i);
            }
            for (int i = 0; i < MAX_OUTPUT_COUNT_A; i++)
            {
                _dtAO.Rows[i]["VALUE"] = IO.AO[i];
                _dtAO.Rows[i]["VOLT" ] = IO.fn_GetVoltage(false, i);
            }


        }

        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            //m_UpdateTimer.Stop();

            fn_DataUpdate();




            //
            //m_UpdateTimer.Start();
        }

    }
}
