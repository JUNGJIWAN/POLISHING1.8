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
using WaferPolishingSystem;
using WaferPolishingSystem.BaseUnit;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.Define.UserConstVision;
using static WaferPolishingSystem.Define.UserConst;
using System.Security.Permissions;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageMotion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageOperMain : Page
    {
        //Var
        Label[,] lbStorg  = new Label[50, 50];
        Label[,] lbStorg1 = new Label[50, 50];

        Label[,] lbMagz01 = new Label[20, 2];
        Label[,] lbMagz02 = new Label[20, 2];


        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        public PageOperation_MainVision pageVision = new PageOperation_MainVision();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public PageOperMain()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;
            frame_Vision.Content = pageVision;
            ug_LoadCellTop.AddLine();

            //Init
            for (int r = 0; r < 50; r++)
            {
                for (int c = 0; c < 50; c++)
                {
                    lbStorg [r, c] = new Label();
                    lbStorg1[r, c] = new Label();
                }
            }

            //
            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    lbMagz01[r, c] = new Label();
                    lbMagz02[r, c] = new Label();
                }
            }

        }
        //---------------------------------------------------------------------------
        //Update Timer
        int nGraphTick = 0;
        private void fn_tmUpdate(object sender, EventArgs e)
        {

            //Timer
            //m_UpdateTimer.Stop();
            m_UpdateTimer.IsEnabled = false;

            //Update Tool Info
            DM.STOR[(int)EN_STOR_ID.POLISH].fn_UpdateMap(ref lbStorg );
            DM.STOR[(int)EN_STOR_ID.CLEAN ].fn_UpdateMap(ref lbStorg1);

            //Plate
            DM.MAGA[(int)EN_MAGA_ID.POLISH].fn_UpdateMap(ref lbPoliPlate  );
            DM.MAGA[(int)EN_MAGA_ID.CLEAN ].fn_UpdateMap(ref lbClenPlate  );
            DM.MAGA[(int)EN_MAGA_ID.LOAD  ].fn_UpdateMap(ref lbLoadPlate  );
            DM.MAGA[(int)EN_MAGA_ID.TRANS ].fn_UpdateMap(ref lbTransPlate );

            //Magazine
            DM.MAGA[(int)EN_MAGA_ID.MAGA01].fn_UpdateMap(ref lbMagz01);
            DM.MAGA[(int)EN_MAGA_ID.MAGA02].fn_UpdateMap(ref lbMagz02);

            //
            DM.TOOL.fn_UpdateMap(ref lbSpdlPlate, ref lbSpdlTool, ref lbSpdlToolFce, ref lbNeedCheck);

            lbCrntRcpName.Text        = string.Format("[{0}] {1}", LOT._bLotOpen ? "OPEN" : "", FM._sRecipeName);
          //lbCrntRcpName.Background  = LOT._bLotOpen? Brushes.Lime : Brushes.WhiteSmoke; //SEQ._bRecipeOpen 
            lbRcp.Background          = LOT._bLotOpen ? Brushes.LimeGreen : Brushes.WhiteSmoke;
            
            lbReqPoli.Background  = SEQ_SPIND._bReqUtil_Polish ? Brushes.Lime : Brushes.LightGray;

            lbReqPoli    .Visibility = FM.fn_IsLvlMaster() ? Visibility.Visible : Visibility.Hidden; //JUNG/210113
            lbSpdlToolFce.Visibility = FM.fn_IsLvlMaster() ? Visibility.Visible : Visibility.Hidden;
            lbNeedCheck  .Visibility = FM.fn_IsLvlMaster() ? Visibility.Visible : Visibility.Hidden;

            //LDCBTM._dLoadCellValue * ONEGRAM_TO_NEWTON
            upLoadCell.UPValue = string.Format($"{LDCBTM.fn_GetBtmLoadCell(true)} N [{LDCBTM.fn_GetBtmLoadCell()} g]") ;

            lbRPM    .Content = string.Format( "SPINDLE RPM : {0} / {1}" , SEQ_SPIND.fn_GetSpindleSpeed(), SEQ_SPIND.fn_GetSpindleDir() ? "DIR : FWD" : "DIR : BWD");
            
            //lbCrntFos.Content = string.Format($"[LOAD CELL] TOP : {IO.fn_GetTopLoadCell(true)} N");
            lbCrntFos.Content = string.Format($"[LOAD CELL] TOP : {IO.fn_GetTopLoadCellAsBTM(true)} N"); //JUNG/200910/

            lbUTLevel.Content = string.Format($"UTIL LEVEL : {IO.fn_GetUTAvgValue():F1}"); //JUNG/200423/Avg Value

            //JUNG/201119/Min,Max Reset
            if (SEQ_SPIND._bReqResetGraph) 
            {
                SEQ_SPIND._bReqResetGraph = false; 
                fn_SetGraphRange();
            }

            // AddGraph
            if (nGraphTick > ug_LoadCellTop.UGDataCount)
            {
                ug_LoadCellTop.RemoveFirst();
            }
            double dTopLoadCell = IO.fn_GetTopLoadCellAsBTM(true); //IO.fn_GetTopLoadCell(true);
            ug_LoadCellTop.UGValue = $"{dTopLoadCell} N";
            //ug_LoadCellTop.UGMaxValue = 10;
            ug_LoadCellTop.AddPoint(new Point(nGraphTick++, dTopLoadCell));
            ug_LoadCellTop.Refresh();

            lbDCOM        .Content = string.Format($"[DCOM] SET: {SEQ_SPIND._dForceRatio:F2} / READ: {IO.fn_GetDCOMValue()}");
            
            if (SEQ_SPIND._bDrngPolishing)
            {
                lbPoliCnt.Content = string.Format($"POLISHING COUNT : {SEQ_SPIND.fn_GetCurrMillCnt(true)} / CYCLE: {SEQ_SPIND._nPolCycle+1} / STEP : {SEQ_SPIND._nPoliCnt+1}");
            }
            else if (SEQ_SPIND._bDrngCleaning)
            {
                lbPoliCnt.Content = string.Format($"CLEANING COUNT : {SEQ_SPIND.fn_GetCurrMillCnt(true)} / CYCLE : {SEQ_SPIND._nClnCycle+1} / STEP : {SEQ_SPIND._nPoliCnt+1}");
            }
            
            //lbFlowMT1.Content = string.Format($"[FLOW] POLISHING SLURY :  {IO.fn_GetFlowMeter(EN_AINPUT_ID.aiPOL_SlurryFlow ):F1}");
            //lbFlowMT2.Content = string.Format($"[FLOW] POLISHING DI    :  {IO.fn_GetFlowMeter(EN_AINPUT_ID.aiPOL_DIWaterFlow):F1}");
            //lbFlowMT3.Content = string.Format($"[FLOW] CLEANING  DI    :  {IO.fn_GetFlowMeter(EN_AINPUT_ID.aiCLN_DIWaterFlow):F1}");
            
            lbLotNo        .Text    = string.Format($"LOT No : {LOT._sLotNo}");
            
            lbStartTime    .Content = string.Format($"[TOTAL] START TIME : {SEQ_TRANS._sStartTime}");
            lbEndTime      .Content = string.Format($"[TOTAL] END TIME : { SEQ_TRANS._sEndTime   }");
            
            lbMillStartTime.Content = (SEQ._bFlick3) ? string.Format($"[POL]START TIME : {SEQ_SPIND._sMillStartTime}") : string.Format($"[CLN]START TIME : {SEQ_SPIND._sCleanStartTime}");
            lbMillEndTime  .Content = (SEQ._bFlick3) ? string.Format($"[POL]END TIME : {SEQ_SPIND._sMillEndTime}")     : string.Format($"[CLN]END TIME : { SEQ_SPIND._sCleanEndTime   }");
            
            
            if (SEQ_SPIND._bDrngPolishing)
            {
                pbPoli   .Visibility = Visibility.Visible;
                tbPolibar.Visibility = Visibility.Visible;

                pbPoli.Value = SEQ_SPIND.fn_GetMillPercent();
            }
            else 
            {
                pbPoli   .Visibility = Visibility.Hidden;
                tbPolibar.Visibility = Visibility.Hidden;
            }
            
            if (SEQ_SPIND._bDrngCleaning)
            {
                //pbClen   .Visibility = Visibility.Hidden; //삭제
                //tbClenbar.Visibility = Visibility.Hidden;
                //pbClen.Value = ((IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_Milling_Cnt] + (100 * SEQ_SPIND._nCleanCnt)) / 400.0) * 100.0;
            }
            else 
            {
                pbClen   .Visibility = Visibility.Hidden;
                tbClenbar.Visibility = Visibility.Hidden;
            }

            //
            lbSpdlState.Content = SEQ_SPIND.fn_GetSeqMsg();
            lbPoliState.Content = SEQ_POLIS.fn_GetSeqMsg();
            lbClenState.Content = SEQ_CLEAN.fn_GetSeqMsg();
            lbTranState.Content = SEQ_TRANS.fn_GetSeqMsg();

            //
            //m_UpdateTimer.Start();
            m_UpdateTimer.IsEnabled = true; 
        }
        //-------------------------------------------------------------------------------------------------
        public void fn_SetGraphRange(int Max = 5, int Min = -1)
        {
            ug_LoadCellTop.UGMaxValue = Max;
            ug_LoadCellTop.UGMinValue = Min;
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
            //List Set
            EPU.m_fn_SetList(lbAlarm, lbWarn);

            fn_SetTimer(true);

            //
            fn_MapGridSet(ref gdStorage , ref lbStorg , (int)EN_STOR_ID.POLISH);
            fn_MapGridSet(ref gdStorage1, ref lbStorg1, (int)EN_STOR_ID.CLEAN );

            //
            fn_MapGridSet_MAGA(ref Magz01, ref lbMagz01, (int)EN_MAGA_ID.MAGA01);
            fn_MapGridSet_MAGA(ref Magz02, ref lbMagz02, (int)EN_MAGA_ID.MAGA02);

        }
        //---------------------------------------------------------------------------
        private void fn_MapGridSet(ref Grid grd, ref Label[,] lbStorg, int sel = 0)
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
                    lb.Content                    = "No." + idxNo + 1;
                    lb.BorderThickness            = new Thickness(1);
                    lb.BorderBrush                = System.Windows.Media.Brushes.Black;
                    lb.Tag                        = new Point(iR, iC);
                    lb.FontSize                   = 8; 
                    lb.HorizontalContentAlignment = HorizontalAlignment.Left;
                    lb.VerticalContentAlignment   = VerticalAlignment.Top;
                    lb.MouseDown                 += Storage_Click;
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
        private void fn_MapGridSet_MAGA(ref Grid grd, ref Label[,] lbDisplay, int sel = 0, bool oper = false)
        {
            //
            int nX = DM.MAGA[sel]._nMaxCol;
            int nY = DM.MAGA[sel]._nMaxRow;
            
            grd.Children.Clear();
            grd.Background = Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            int idx  = 0, idxNo = 0;
            int nCol = nX;
            int nRow = nY;
            int iR   = 0, iC = 0; 

            lbDisplay = (Label[,]) fn_ResizeArray(lbDisplay, new int[] { nY, nX});

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

                    switch ((EN_DIS_DIR)DM.MAGA[sel]._nDirection)
                    {
                     
                        case EN_DIS_DIR.ddTopLeft  : iR = r;              iC = c;              break; 
                        case EN_DIS_DIR.ddTopRight : iR = r             ; iC = (nCol - 1) - c; break; 
                        case EN_DIS_DIR.ddBtmRight : iR = (nRow - 1) - r; iC = (nCol - 1) - c; break; 
						case EN_DIS_DIR.ddBtmLeft  : iR = (nRow - 1) - r; iC = c;              break; 

                        default: return; 
                    }

                    idx   = r  * nCol + c ;
                    idxNo = iR * nCol + iC;

                    grd.Children.Add(new Label());
                    Label lb           = (grd.Children[idx] as Label);
                    lb.Content         = "NO." + idxNo;
                    lb.BorderThickness = new Thickness(1);
                    lb.BorderBrush     = System.Windows.Media.Brushes.Black;
                    lb.Tag             = new Point(iR, iC);
                    lb.MouseDown      += Magazine_Click;
                    lb.Margin          = new Thickness(1);
                    lb.FontSize        = oper? 8 : 11;
                    lb.Padding         = new Thickness(0);

                    Grid.SetColumn((grd.Children[idx] as Label), c);
                    Grid.SetRow   ((grd.Children[idx] as Label), r);

                    lbDisplay[iR, iC] = lb;

                }
            }

            Border boder          = new Border();
            boder.BorderBrush     = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin          = new Thickness(-2);
            Grid.SetRowSpan   (boder, 100);
            Grid.SetColumnSpan(boder, 100);
            grd.Children.Add  (boder     );
        }

        //---------------------------------------------------------------------------
        private void fn_GridOrderSet(ref Grid grd)
        {
            //
            int nX = DM.MAGA[4]._nMaxCol; 
            int nY = DM.MAGA[4]._nMaxRow;
            
            Label[,] lbOrder = new Label[40, 2];

            grd.Children.Clear();
            grd.Background = Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            int nCol = nX;
            int nRow = nY;

            lbOrder = (Label[,]) fn_ResizeArray(lbOrder, new int[] { nY, nX});

            for (int R = 0; R < nRow; R++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < nRow; r++)
            {
                grd.Children.Add(new Label());
                Label lb            = (grd.Children[r] as Label);
                lb.Content          =  r + 1;
                lb.BorderThickness  = new Thickness(1);
                lb.BorderBrush      = System.Windows.Media.Brushes.Black;
                lb.FontSize         = 5;

                //lb.HorizontalContentAlignment = HorizontalAlignment.Left;
                //lb.VerticalContentAlignment   = VerticalAlignment.Top;
                //lb.MouseDown += mouseevent;
                lb.Margin = new Thickness(1);

                Grid.SetRow   ((grd.Children[r] as Label), r);

            }

            Border boder = new Border();
            boder.BorderBrush     = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin = new Thickness(-2);
            Grid.SetRowSpan   (boder, 100);
            Grid.SetColumnSpan(boder, 100);
            grd.Children.Add  (boder);
        }

        //---------------------------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //UserMsg("TEST","Confirm",0);
            //EPU.fn_LoadLampData(false);


        }
        //---------------------------------------------------------------------------
        private void Storage_Click(object sender, MouseEventArgs e)
        {
            //
            if (SEQ._bRun) return;

            //
            FormMapStrg.fn_ShowMap();

        }
        //---------------------------------------------------------------------------
        private void Magazine_Click(object sender, MouseEventArgs e)
        {
            //
            if (SEQ._bRun) return; 

            //
            FormMapMAGZ.fn_ShowMap();
        }
        //---------------------------------------------------------------------------
        private void frame_Vision_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
                e.Cancel = true;
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Stop();

        }
    }
}
