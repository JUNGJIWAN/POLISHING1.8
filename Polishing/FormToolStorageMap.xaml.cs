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
using System.Windows.Threading;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;

namespace WaferPolishingSystem
{
    
    /// <summary>
    /// FormMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormStorageMap : Window
    {
        //bool m_bClose  = false;
        bool m_bResult = false;

        int m_nRowCnt;
        int m_nColCnt;

        public int    m_nKind;
        public string m_sTitle;
        public string m_sMsg;

        Label[,] lbStr01 = new Label[50, 50];
        Label[,] lbStr02 = new Label[50, 50];

        bool[,] m_bSelStr01 = new bool[50, 50];
        bool[,] m_bSelStr02 = new bool[50, 50];

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //---------------------------------------------------------------------------
        public FormStorageMap()
        {
            InitializeComponent();

            m_nRowCnt = 0;
            m_nColCnt = 0; 

            m_bResult = false;

            m_nKind  = 0;
            m_sTitle = string.Empty;
            m_sMsg   = string.Empty;

            for (int r = 0; r < 50; r++)
            {
                for (int c = 0; c < 50; c++)
                {
                    //
                    lbStr01[r, c] = new Label();
                    lbStr02[r, c] = new Label();

                    //
                    m_bSelStr01[r,c] = new bool();
                    m_bSelStr02[r,c] = new bool();
                }
            }

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick    += new EventHandler(fn_tmUpdate);



        }
        //---------------------------------------------------------------------------
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            //m_UpdateTimer.Stop();

            int nCol = DM.STOR[0]._nMaxCol;
            int nRow = DM.STOR[0]._nMaxRow;
            
            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < nCol; c++)
                {
                    //
                    if (m_bSelStr01[r, c])
                    {
                        lbStr01[r, c].BorderThickness = new Thickness(5);
                        lbStr01[r, c].BorderBrush     = System.Windows.Media.Brushes.LimeGreen;
                    }
                    else
                    {
                        lbStr01[r, c].BorderThickness = new Thickness(1);
                        lbStr01[r, c].BorderBrush     = System.Windows.Media.Brushes.Black;
                    }

                    //
                    if (m_bSelStr02[r, c])
                    {
                        lbStr02[r, c].BorderThickness = new Thickness(5);
                        lbStr02[r, c].BorderBrush     = System.Windows.Media.Brushes.LimeGreen;
                    }
                    else
                    {
                        lbStr02[r, c].BorderThickness = new Thickness(1);
                        lbStr02[r, c].BorderBrush     = System.Windows.Media.Brushes.Black;
                    }
                }
            }


            DM.STOR[(int)EN_STOR_ID.POLISH].fn_UpdateMap(ref lbStr01, false, true) ;
            DM.STOR[(int)EN_STOR_ID.CLEAN ].fn_UpdateMap(ref lbStr02) ;
            
            //m_UpdateTimer.Start();

        }
        //---------------------------------------------------------------------------
        private void fn_SelArry(int type = -1)
        {
            for (int r = 0; r < m_nRowCnt; r++)
            {
                for (int c = 0; c < m_nColCnt; c++)
                {
                    //
                    if (type < 0)
                    {
                        m_bSelStr01[r, c] = true;
                        m_bSelStr02[r, c] = true;
                    }
                    else if (type == siPolish)
                    {
                        m_bSelStr01[r, c] = true;
                    }
                    else if (type == siClean)
                    {
                        m_bSelStr02[r, c] = true;
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        private void fn_ClearArry(int type = -1)
        {
            for (int r = 0; r < m_nRowCnt; r++)
            {
                for (int c = 0; c < m_nColCnt; c++)
                {
                    //
                    if (type < 0)
                    {
                        m_bSelStr01[r, c] = false;
                        m_bSelStr02[r, c] = false;
                    }
                    else if (type == siPolish)
                    {
                        m_bSelStr01[r, c] = false;
                    }
                    else if (type == siClean)
                    {
                        m_bSelStr02[r, c] = false;
                    }

                }
            }

        }
        //---------------------------------------------------------------------------
        public bool fn_ShowMap()
        {
            if (this.IsVisible) return false;

            m_bResult = false;

            m_nRowCnt = DM.STOR[0]._nMaxRow;
            m_nColCnt = DM.STOR[0]._nMaxCol;

            fn_ClearArry();

            fn_MapSet();

            //
            btSel01.Content = FM.m_stSystemOpt.sToolType[0];
            btSel02.Content = FM.m_stSystemOpt.sToolType[1];
            //btSel03.Content = FM.m_stSystemOpt.sToolType[2];
            btSel03.Visibility = Visibility.Hidden; 

            m_UpdateTimer.Start();

            //Modal
            this.ShowDialog();

            return m_bResult ;

        }
        //---------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //
//             if (!m_bClose)
//             {
//                 e.Cancel = true;
//                 this.Hide();
//             }

        }
        //---------------------------------------------------------------------------
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.Stop();

            this.Hide();
        }
        //---------------------------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //


        }
        //---------------------------------------------------------------------------
        public void fn_MapSet()
        {
            //
            fn_MapGridSet(ref gdStr01, ref lbStr01, (int)EN_STOR_ID.POLISH);
            fn_MapGridSet(ref gdStr02, ref lbStr02, (int)EN_STOR_ID.CLEAN );
        }
        //---------------------------------------------------------------------------
        private void fn_MapGridSet(ref Grid grd, ref Label[,] lbStorg, int sel)
        {
            //
            int nX = DM.STOR[sel]._nMaxCol; 
            int nY = DM.STOR[sel]._nMaxRow;

            
            grd.Children.Clear();
            grd.Background = Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            int idx  = 0, idxNo = 0;
            int nCol = nX;
            int nRow = nY;
            int iR   = 0, iC = 0;

            lbStorg = (Label[,]) UserFunction.fn_ResizeArray(lbStorg, new int[] { nY, nX});

            for (int C = 0; C < nCol; C++)
            {
                grd.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int R = 0; R < nRow; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < nCol; c++)
                {
                    switch ((EN_DIS_DIR)DM.STOR[sel]._nDir)
                    {
                     
                        case EN_DIS_DIR.ddTopLeft  : iR = r;              iC = c;              break; //
                        case EN_DIS_DIR.ddTopRight : iR = r             ; iC = (nCol - 1) - c; break; //
                        case EN_DIS_DIR.ddBtmRight : iR = (nRow - 1) - r; iC = (nCol - 1) - c; break; //
						case EN_DIS_DIR.ddBtmLeft  : iR = (nRow - 1) - r; iC = c;              break; //
                        default: return; 
                    }


                    idx   = r * nCol + c  ;
                    idxNo = iR * nCol + iC;

                    grd.Children.Add(new Label());
                    Label lb                      = (grd.Children[idx] as Label);
                    lb.Content                    = "No." + (idxNo + 1);
                    lb.BorderThickness            = new Thickness(1);
                    lb.BorderBrush                = System.Windows.Media.Brushes.Black;
                    lb.Tag                        = new Point(iR, iC);
                    lb.FontSize                   = 12; 
                    lb.HorizontalContentAlignment = HorizontalAlignment.Left;
                    lb.VerticalContentAlignment   = VerticalAlignment.Top;
                    lb.MouseDown                 += mouseevent;
                    lb.Margin = new Thickness(1);

                    Grid.SetColumn((grd.Children[idx] as Label), c);
                    Grid.SetRow   ((grd.Children[idx] as Label), r);

                    lbStorg[iR, iC] = lb;

                }
            }

            Border boder = new Border();
            boder.BorderBrush = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin = new Thickness(-2);
            Grid.SetRowSpan   (boder, 100);
            Grid.SetColumnSpan(boder, 100);
            grd.Children.Add  (boder);
        }


        //---------------------------------------------------------------------------
        private void mouseevent(object sender, MouseEventArgs e)
        {
            //Storage Map Setting
            //bool  bShow        = false;
            int   m_nStorage   = 0;
            Point m_ptStoPoint = new Point(0, 0);

            Label sellbl = sender as Label;
                 if ((sellbl.Parent as Grid).Name == "gdStr01") m_nStorage = (int)EN_STOR_ID.POLISH;
            else if ((sellbl.Parent as Grid).Name == "gdStr02") m_nStorage = (int)EN_STOR_ID.CLEAN ; 


            //Point
            m_ptStoPoint = (Point)sellbl.Tag; //(Point)(sender as Label).Tag;

            if(m_nStorage == (int)EN_STOR_ID.POLISH)
            {
                m_bSelStr01[(int)m_ptStoPoint.X, (int)m_ptStoPoint.Y] = !m_bSelStr01[(int)m_ptStoPoint.X, (int)m_ptStoPoint.Y]; 
            }
            else
            {
                m_bSelStr02[(int)m_ptStoPoint.X, (int)m_ptStoPoint.Y] = !m_bSelStr02[(int)m_ptStoPoint.X, (int)m_ptStoPoint.Y];
            }
        }
        //---------------------------------------------------------------------------
        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            fn_ClearArry();
        }
        //---------------------------------------------------------------------------
        private void btSel01_Click(object sender, RoutedEventArgs e)
        {//Polishing Button
         //
            Button selBtn = sender as Button;
            string sName  = selBtn.Name;

            switch (sName)
            {
                case "btSel01"  :
                    fn_SetPolishing(EN_PIN_STAT.psNewPol, (int)EN_PIN_KIND.pkUser1);
                    break;

                case "btSel02"  :
                    fn_SetPolishing(EN_PIN_STAT.psNewPol, (int)EN_PIN_KIND.pkUser2);
                    break;

                case "btSel03"  :
                    fn_SetPolishing(EN_PIN_STAT.psNewPol, (int)EN_PIN_KIND.pkUser3);
                    break;

                case "btEmptyP" :
                    fn_SetPolishing(EN_PIN_STAT.psEmpty);
                    break;

                case "btSelAllP": fn_SelArry  (siPolish); break;
                case "btCleAllP": fn_ClearArry(siPolish); break;
                default:
                    fn_ClearArry(siPolish);
                    break;
            }

            

        }
        //---------------------------------------------------------------------------
        private void fn_SetPolishing(EN_PIN_STAT stat, int type = -1)
        {
            
            for (int r = 0; r < m_nRowCnt; r++)
            {
                for (int c = 0; c < m_nColCnt; c++)
                {
                    if(m_bSelStr01[r, c])
                    {
                        DM.STOR[siPolish].SetTo(r, c, (int)stat, type);
                    }

                }
            }

            fn_ClearArry(siPolish);
        }
        
        //---------------------------------------------------------------------------
        private void btReadyC_Click(object sender, RoutedEventArgs e)
        {//Clear Button

            Button selBtn = sender as Button;
            string sName  = selBtn.Name;

            switch (sName)
            {
                case "btReadyC":
                    fn_SetCleaning(EN_PIN_STAT.psNewCln);
                    break;

                case "btEmptyC":
                    fn_SetCleaning(EN_PIN_STAT.psEmpty);
                    break;

                case "btSelAllC": fn_SelArry  (siClean); break;
                case "btCleAllC": fn_ClearArry(siClean); break;
                default:
                    fn_ClearArry(siClean);
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void fn_SetCleaning(EN_PIN_STAT stat)
        {
            
            for (int r = 0; r < m_nRowCnt; r++)
            {
                for (int c = 0; c < m_nColCnt; c++)
                {
                    if(m_bSelStr02[r, c])
                    {
                        DM.STOR[siClean].SetTo(r, c, (int)stat);
                    }

                }
            }

            fn_ClearArry(siClean);
        }

    }
}



