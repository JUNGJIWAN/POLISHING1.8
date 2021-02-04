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
using System.IO;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserConstVision;

namespace WaferPolishingSystem
{

    /// <summary>
    /// FormMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormMagazineMap : Window
    {
        //bool m_bClose = false;
        bool m_bResult = false;

        int m_nRowCnt;
        int m_nColCnt;

        public int m_nKind;
        public string m_sTitle;
        public string m_sMsg;

        Label[,] lbMagz01 = new Label[20, 2];
        Label[,] lbMagz02 = new Label[20, 2];
        ComboBox[,] cbRecipe;
        Point m_ptStoPoint = new Point();
        int m_nSelMaga;

        List<string> m_listRecipe = new List<string>();

        int nX = 0;
        int nY = 0;

        ContextMenu cmMenu = new ContextMenu();

        TextBox m_tbRFNo = new TextBox();

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //---------------------------------------------------------------------------
        public FormMagazineMap()
        {
            InitializeComponent();

            m_nRowCnt = 0;
            m_nColCnt = 0;

            m_bResult = false;

            m_nKind  = 0;
            m_sTitle = string.Empty;
            m_sMsg   = string.Empty;

            cbRecipe = new ComboBox[2, 20];

            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    //
                    lbMagz01[r, c] = new Label();
                    lbMagz02[r, c] = new Label();

                }
            }

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            m_nSelMaga = 0;

        }
        //---------------------------------------------------------------------------
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            //m_UpdateTimer.Stop();


            //Magazine
            DM.MAGA[(int)EN_MAGA_ID.MAGA01].fn_UpdateMap(ref lbMagz01);
            DM.MAGA[(int)EN_MAGA_ID.MAGA02].fn_UpdateMap(ref lbMagz02);

            //fn_UpdateRecipeList();

            //
            btSave.IsEnabled = SEQ._bRun ? false : true;
            //btRead.IsEnabled = SEQ._bRun ? false : RFID._bConnect ? true : false ;

            //
            if (FM.m_stMasterOpt.nUseRESTApi == 1)
            {

                if (REST._bDrngConnect)
                {
                    lbCon.Content = "Try to connection...";
                    UserFunction.fn_SetLabelColor(ref lbCon, REST._bDrngConnect && SEQ._bFlick1, Brushes.Brown, Brushes.Lime);
                }
                else
                {
                    lbCon.Content = REST._bConnect ? "Connect" : "Disconnected";
                    UserFunction.fn_SetLabelColor(ref lbCon, REST._bConnect, Brushes.Lime, Brushes.Red);
                }
            }
            else
            {
                lbCon.Content = "Not Use REST API"; ;
                UserFunction.fn_SetLabelColor(ref lbCon, false, Brushes.Lime, Brushes.Red);
            }

            //
            lbVer.Content = REST._sVersion;

            //RFID Data Display
            if (m_tbRFNo != null && !RFID._bDrngRead && RFID._bUpdateData)
            {
                RFID._bUpdateData = false; 
                m_tbRFNo.Text = RFID._sReadRFNo;
            }

            //Wafer Info
            if (REST._bUpdateData)
            {
                //
                lbPlateId.Content = "ID : " + REST.RcvPlateRFIDInfo._sPlateId;
                lbPlateId.Foreground = Brushes.Black;
                lbRcvTime.Content = "Received Time : " + REST.RcvPlateRFIDInfo.sRcvTime;

                //Specimen Info
                tbItem01.Text = REST.RcvPlateRFIDInfo.specimeninfo._nSizeX.ToString();
                tbItem02.Text = REST.RcvPlateRFIDInfo.specimeninfo._nSizeY.ToString();
                tbItem03.Text = REST.RcvPlateRFIDInfo.specimeninfo._sShotPos         ;
                tbItem04.Text = REST.RcvPlateRFIDInfo.specimeninfo._sChipPos         ;
                tbItem05.Text = REST.RcvPlateRFIDInfo.specimeninfo._sMatPos          ;
                tbItem06.Text = REST.RcvPlateRFIDInfo.specimeninfo._sMatLoc          ;
                tbItem07.Text = REST.RcvPlateRFIDInfo.specimeninfo._sType            ;

                //Wafer Info
                tbItem11.Text = REST.RcvPlateRFIDInfo.specimeninfo.waferInfo._sDevice;
                tbItem12.Text = REST.RcvPlateRFIDInfo.specimeninfo.waferInfo._sProcessStep;
                tbItem13.Text = REST.RcvPlateRFIDInfo.specimeninfo.waferInfo._sVersion;
                tbItem14.Text = REST.RcvPlateRFIDInfo.specimeninfo.waferInfo._sLotID;
                tbItem15.Text = REST.RcvPlateRFIDInfo.specimeninfo.waferInfo._nWafer_num.ToString();
                tbItem16.Text = REST.RcvPlateRFIDInfo.specimeninfo.waferInfo._nAngle.ToString();

                REST._bUpdateData = false;

            }

            //JUNG/201215
            if(RFID._bDataError)
            {
                lbPlateId.Content = "RFID DATA READ ERROR!!!";
                lbPlateId.Foreground = Brushes.Red; 
            }


            lbRFIDErr.Content = RFID._sState;
        }
        //-------------------------------------------------------------------------------------------------
        private void fn_ResetDataClear()
        {
            string sTemp = "-"; 
            //
            lbPlateId.Content = sTemp;

            //Specimen Info
            tbItem01.Text = sTemp;
            tbItem02.Text = sTemp;
            tbItem03.Text = sTemp;
            tbItem04.Text = sTemp;
            tbItem05.Text = sTemp;
            tbItem06.Text = sTemp;
            tbItem07.Text = sTemp;
                            
            //Wafer Info    
            tbItem11.Text = sTemp;
            tbItem12.Text = sTemp;
            tbItem13.Text = sTemp;
            tbItem14.Text = sTemp;
            tbItem15.Text = sTemp;
            tbItem16.Text = sTemp;

        }
        //---------------------------------------------------------------------------
        private void fn_UpdateRecipeList()
        {
             //cbRecipe
             fn_GetRecipeList();

             //int nRow = DM.MAGA[0]._nMaxRow;
             int nRow = (int)(cbRecipe.Length / 2.0);
             for (int r = 0; r < nRow; r++)
             {
                 if (cbRecipe[0, r] != null)  cbRecipe[0, r] = null;
                 if (cbRecipe[1, r] != null)  cbRecipe[1, r] = null;

                 cbRecipe[0, r] = new ComboBox();
                 cbRecipe[1, r] = new ComboBox();
                 cbRecipe[0, r].Items.Clear();
                 cbRecipe[1, r].Items.Clear();

                cbRecipe[0, r].Items.Add("");
                cbRecipe[1, r].Items.Add("");

                for (int i = 0; i < m_listRecipe.Count; i++)
                {
                    cbRecipe[0, r].Items.Add(m_listRecipe[i].ToString());
                    cbRecipe[1, r].Items.Add(m_listRecipe[i].ToString());
                }
             }

             for (int r = 0; r < 20; r++)
             {
                 cbRecipe[0, r].SelectedItem = DM.MAGA[(int)EN_MAGA_ID.MAGA01].PLATE[r, 0]._sRecipeName;
                 cbRecipe[1, r].SelectedItem = DM.MAGA[(int)EN_MAGA_ID.MAGA02].PLATE[r, 0]._sRecipeName;
             }
        }
        //---------------------------------------------------------------------------
        public bool fn_ShowMap()
        {
            if (this.IsVisible) return false;

            m_bResult = false;

            m_nRowCnt = DM.MAGA[0]._nMaxRow;
            m_nColCnt = DM.MAGA[0]._nMaxCol;

            m_UpdateTimer.Start();

            fn_LoadData();

            //
            if (!SEQ._bRun && RFID._bConnect && FM.m_stMasterOpt.nUseRESTApi == 1)
            {
                //REST.fn_Connect();
                //if(REST._bConnect) REST.fn_GetVersion();
                REST.fn_ReqConnection();
                REST.fn_ReqVersion();
            }

            //Modal
            this.ShowDialog();

            return m_bResult;

        }
        //---------------------------------------------------------------------------
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.Stop();

            this.Hide();
        }
        //---------------------------------------------------------------------------
        public void fn_LoadData()
        {
            fn_UpdateRecipeList();

            fn_MapGridSet_Maga(Magz01, (int)EN_MAGA_ID.MAGA01, ref lbMagz01);
            fn_MapGridSet_Maga(Magz02, (int)EN_MAGA_ID.MAGA02, ref lbMagz02);
        }
        //---------------------------------------------------------------------------
        public void MagaMouseEvent(object sender, MouseEventArgs e)
        {
            //Magazine Map Setting
            bool bShow = false;

            Label sellbl = sender as Label;
            if (((sellbl.Parent as Grid).Parent as Grid).Name == "Magz01") m_nSelMaga = (int)EN_MAGA_ID.MAGA01;
            if (((sellbl.Parent as Grid).Parent as Grid).Name == "Magz02") m_nSelMaga = (int)EN_MAGA_ID.MAGA02;

            //Point
            m_ptStoPoint = (Point)sellbl.Tag; //(Point)(sender as Label).Tag;

            if  (Keyboard.IsKeyDown(Key.LeftCtrl )) bShow = fn_SetMenuItem(1); //All
            else                                    bShow = fn_SetMenuItem( ); //One

            if (bShow) cmMenu.IsOpen = true;
            else       cmMenu.IsOpen = false;
        }
        //---------------------------------------------------------------------------
        public void RFMouseEvent(object sender, MouseEventArgs e)
        {
            TextBox seltb = sender as TextBox;

            seltb.SelectAll();

            //m_tbRFNo = seltb; 
        }
        //---------------------------------------------------------------------------
        private void fn_MapGridSet_Maga(Grid grd, int id, ref Label[,] arraylb)
        {
            //
            nX = DM.MAGA[id]._nMaxCol;
            nY = DM.MAGA[id]._nMaxRow;

            grd.Children.Clear();
            grd.Background = Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

            int idx = 0, idxNo = 0;
            int nCol = nX;
            int nRow = nY;
            int iR = 0, iC = 0;

            arraylb = (Label[,])fn_ResizeArray(arraylb, new int[] { nY, nX });

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
                    switch ((EN_DIS_DIR)DM.MAGA[id]._nDirection)
                    {
                        case EN_DIS_DIR.ddTopLeft : iR = r; iC = c; break;
                        case EN_DIS_DIR.ddTopRight: iR = r; iC = (nCol - 1) - c; break;
                        case EN_DIS_DIR.ddBtmRight: iR = (nRow - 1) - r; iC = (nCol - 1) - c; break;
                        case EN_DIS_DIR.ddBtmLeft : iR = (nRow - 1) - r; iC = c; break;

                        default: return;
                    }

                    idx = r * nCol + c;
                    idxNo = iR * nCol + iC;

                    grd.Children.Add(new Grid());
                    Grid grid = (grd.Children[idx] as Grid);
                    grid.Tag = new Point(iR, iC);
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions[0].Width = new GridLength(80);
                    grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                    grid.ColumnDefinitions[2].Width = new GridLength(100);

                    Label lb = new Label();
                    lb.Content = "NO." + idxNo;
                    lb.BorderThickness = new Thickness(1);
                    lb.BorderBrush = Brushes.Black;
                    lb.MouseDown += MagaMouseEvent;
                    lb.Margin = new Thickness(1);
                    lb.FontSize = 11;
                    lb.Tag = new Point(iR, iC);
                    
                    //
                    cbRecipe[id - (int)EN_MAGA_ID.MAGA01, r].Tag = new Point(iR, iC);
              
                    TextBox tb = new TextBox();
                    tb.Text = "-";
                    tb.Tag = new Point(iR, iC);
                    tb.Margin = new Thickness(1);
                    tb.FontSize = 11;
                    tb.GotFocus  += fn_SelectionTextbox;
                    tb.LostFocus += fn_UnSelectionTextbox;

                    tb.PreviewMouseUp += RFMouseEvent;


                    grid.Children.Add(lb);
                    Grid.SetColumn(lb, 0);
                    Grid.SetRow(lb, 0);
                    //                     grid.Children.Add(lb2);
                    //                     Grid.SetColumn(lb2, 1);
                    //                     Grid.SetRow(lb2, 0);
                    grid.Children.Add(cbRecipe[id - (int)EN_MAGA_ID.MAGA01, r]   );
                    Grid.SetColumn   (cbRecipe[id - (int)EN_MAGA_ID.MAGA01, r], 1);
                    Grid.SetRow      (cbRecipe[id - (int)EN_MAGA_ID.MAGA01, r], 0);

                    grid.Children.Add(tb);
                    Grid.SetColumn(tb, 2);
                    Grid.SetRow(tb, 0);

                    Grid.SetColumn((grd.Children[idx] as Grid), c);
                    Grid.SetRow((grd.Children[idx] as Grid), r);

                    arraylb[iR, iC] = lb;
                }
            }

            Border boder = new Border();
            boder.BorderBrush = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin = new Thickness(-2);
            Grid.SetRowSpan(boder, 100);
            Grid.SetColumnSpan(boder, 100);
            grd.Children.Add(boder);
        }
        //---------------------------------------------------------------------------
        public static Array fn_ResizeArray(Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }
        //---------------------------------------------------------------------------
        private bool fn_SetMenuItem(int kind = 0)
        {//kind - 0 : One, 1: All, 2:Row, 3:Col
            //
            string sPre = string.Empty;
            string sItem = string.Empty;
            int nIdex = 0;

            cmMenu.Items.Clear();
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));

            for (int i = 0; i < STR_PLATE_STAT.Length; i++)
            {
                MenuItem item = new MenuItem();
                if (kind == 1)
                {
                    sPre = "ALL - ";
                    item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
                }
                else
                {
                    sPre = "ONE - ";
                    item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));
                }

                sItem = STR_PLATE_STAT[i];
                if (sItem != "EMPTY" && sItem != "READY" && sItem != "FINISH") continue;

                item.Header = sPre + STR_PLATE_STAT[i];
                item.Tag = nIdex++;
                cmMenu.Items.Add(item);
            }

            cmMenu.IsOpen = true;


            return true;
        }
        //---------------------------------------------------------------------------
        private void menuItemOne_Click(object sender, RoutedEventArgs e)
        {//m_nSelPart >> 0:Storage, 1:Plate, 2:Tool, 3: Magazine

            if (SEQ._bRun)
            {
                UserFunction.fn_UserMsg("Can not be Change while the Machine is running.");
            }

            //Local Var.
            EN_PLATE_STAT enPlateState = EN_PLATE_STAT.ptsNone;

            MenuItem mu = sender as MenuItem;


            //int nTag = Array.IndexOf(UserConst.STR_PIN_STAT, mu.Header.ToString().Substring(6));
            int nTag = Convert.ToInt32(mu.Tag);

            switch (nTag)
            {
                case 0:
                    enPlateState = EN_PLATE_STAT.ptsEmpty;
                    break;
                case 1:
                    enPlateState = EN_PLATE_STAT.ptsReady;
                    break;
                case 2:
                    enPlateState = EN_PLATE_STAT.ptsFinish;
                    break;
                
                default:
                    break;
            }

            DM.MAGA[m_nSelMaga].SetTo((int)m_ptStoPoint.X, (int)m_ptStoPoint.Y, (int)enPlateState);


        }
        //---------------------------------------------------------------------------
        private void menuItemAll_Click(object sender, RoutedEventArgs e)
        {
            if (SEQ._bRun)
            {
                UserFunction.fn_UserMsg("Can not Change while the Machine is running.");
            }

            //Local Var.
            EN_PLATE_STAT enPlateState = EN_PLATE_STAT.ptsNone;

            MenuItem mu = sender as MenuItem;

            //int nTag = Array.IndexOf(UserConst.STR_PIN_STAT, mu.Header.ToString().Substring(6));
            int nTag = Convert.ToInt32(mu.Tag);

            switch (nTag)
            {
                case 0:
                    enPlateState = EN_PLATE_STAT.ptsEmpty;
                    break;
                case 1:
                    enPlateState = EN_PLATE_STAT.ptsReady;
                    break;
                case 2:
                    enPlateState = EN_PLATE_STAT.ptsFinish;
                    break;

                default:
                    break;
            }

            //
            DM.MAGA[m_nSelMaga].SetTo((int)enPlateState);


        }
        //---------------------------------------------------------------------------
        private void fn_GetRecipeList()
        {

            string[] strRecipeList = Directory.GetDirectories(STRRECIPEPATH);
            m_listRecipe.Clear();
            for (int i = 0; i < strRecipeList.Length; i++)
            {
                DirectoryInfo di = new DirectoryInfo(strRecipeList[i]);
                FileInfo fi = new FileInfo(di.FullName + "\\" + di.Name + ".ini");

                m_listRecipe.Add(di.Name);
            }
        }

        //---------------------------------------------------------------------------
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            // Save Code
            try
            {
                int nCol = 2;
                int nRow = (int)(cbRecipe.Length / nCol);

                string sName01 = string.Empty;
                string sName02 = string.Empty;

                for (int r = 0; r < nRow; r++)
                {
                    sName01 = cbRecipe[0, r].SelectedItem as string;
                    sName02 = cbRecipe[1, r].SelectedItem as string;

                    if (sName01 == null) sName01 = string.Empty;
                    if (sName02 == null) sName02 = string.Empty;

                    if (DM.MAGA[(int)EN_MAGA_ID.MAGA01].IsStat(r, 0, (int)EN_PLATE_STAT.ptsReady)) //JUNG/200527
                    {
                        DM.MAGA[(int)EN_MAGA_ID.MAGA01].PLATE[r, 0]._sRecipeName = sName01;
                    }
                    
                    if (DM.MAGA[(int)EN_MAGA_ID.MAGA02].IsStat(r, 0, (int)EN_PLATE_STAT.ptsReady)) //JUNG/200527
                    {
                        DM.MAGA[(int)EN_MAGA_ID.MAGA02].PLATE[r, 0]._sRecipeName = sName02;
                    }


                    //DM.MAGA[(int)EN_MAGA_ID.MAGA01].PLATE[r, 0]._sPlateId    = sName01;
                    //DM.MAGA[(int)EN_MAGA_ID.MAGA02].PLATE[r, 0]._sPlateId    = sName02;
                    this.Hide();

                }

                UserFunction.fn_WriteLog("Magazine Recipe / Map Saved");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        private void Recipe_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            fn_LoadData();
        }
        //---------------------------------------------------------------------------
        private void fn_SelectionTextbox(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if(tb != null)
            {
                tb.BorderThickness = new Thickness(3);
                tb.SelectAll();

                m_tbRFNo = tb;

                btRead.IsEnabled = !SEQ._bRun && RFID._bConnect;
            }
            
        }
        //---------------------------------------------------------------------------
        private void fn_UnSelectionTextbox(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.BorderThickness = new Thickness(1);
            }

        }
        //---------------------------------------------------------------------------
        private void btRead_Click(object sender, RoutedEventArgs e)
        {
            string sPlateID = m_tbRFNo.Text.ToUpper();

            fn_ResetDataClear();

            //JUNG/201215
            if (sPlateID.Substring(0, 1) == "P" && sPlateID.Length == 5)
            {
                UserFunction.fn_WriteLog("[Manual] Request RF Info.: " + sPlateID);

                //P,5글자의 경우 RFID Read Skip 후 REST API(요청:이은지)
                REST.fn_ReqRFInfo(sPlateID);
            }
            else
            {
                UserFunction.fn_WriteLog("[Manual] RFID Read Click");

                fn_ResetDataClear();
                RFID.fn_ReqRead();
            }
            
            btRead.IsEnabled = false; 

        }
        //---------------------------------------------------------------------------
        private void btAllClear_Click(object sender, RoutedEventArgs e)
        {//All Clear

            DM.MAGA[(int)EN_MAGA_ID.MAGA01].SetTo((int)EN_PLATE_STAT.ptsEmpty);
            DM.MAGA[(int)EN_MAGA_ID.MAGA02].SetTo((int)EN_PLATE_STAT.ptsEmpty);

            for (int r = 0; r < 20; r++)
            {
                cbRecipe[0, r].SelectedIndex = 0;
                cbRecipe[1, r].SelectedIndex = 0;
            }




        }
    }
}