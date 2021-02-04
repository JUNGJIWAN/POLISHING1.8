using System;
using System.Collections.Generic;
using System.IO;
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
using WaferPolishingSystem.Form;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;
using Brushes = System.Windows.Media.Brushes;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Image = System.Windows.Controls.Image;
//using Point = System.Drawing.Point;
using WaferPolishingSystem.BaseUnit;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.BaseUnit.Magazine;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.BaseUnit.ManualId;

using ControlCore;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageMotion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageMapDisplay : Page
    {
        //Var
        int   nX, nY;
        Label[,] lbDisplay    = new Label[50, 50];
        Label[,] lbDisplay1   = new Label[50, 50];
        Point    m_ptStoPoint = new Point();

        Label[,] lbMagz01 = new Label[20, 2];
        Label[,] lbMagz02 = new Label[20, 2];

        bool [,] m_bUseItem = new bool[4, 9];

        int m_nSelPart   ;
        int m_nSelWhre   ;
        int m_nTestTool  ;
        int m_nTestPlate ;
        int m_nStorage   ;
        int m_nSelMaga   ;

        //
        ContextMenu cmMenu = new ContextMenu();

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public PageMapDisplay()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);


            //Back Color Set
            this.Background = Brushes.Beige; //G_COLOR_PAGEBACK;

            //Init
            for (int n1 = 0; n1 <= lbDisplay.GetUpperBound(0); n1++)
            {
                lbDisplay[n1, 0] = new Label();
                lbDisplay[n1, 1] = new Label();

                lbDisplay1[n1, 0] = new Label();
                lbDisplay1[n1, 1] = new Label();

            }

            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    lbMagz01[r, c] = new Label();
                    lbMagz02[r, c] = new Label();
                }
            }

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    m_bUseItem[r ,c] = new bool();
                    m_bUseItem[r, c] = false;
                }
            }

            //Plate Tag
            lbPoliPlate .Tag = (int)EN_MAGA_ID.POLISH;
            lbClenPlate .Tag = (int)EN_MAGA_ID.CLEAN ;
            lbLoad      .Tag = (int)EN_MAGA_ID.LOAD  ;
            lbTrans     .Tag = (int)EN_MAGA_ID.TRANS ;

            //0//"EMPTY"
            //1//"LOADED"
            //2//"PRE-ALIGN"
            //3//"READY"
            //4//"ALIGN"
            //5//"POLISHING"
            //6//"CLEANING"
            //7//"FINISH"
            //8//"DeHydrate"

            m_bUseItem[(int)EN_MAGA_ID.POLISH, 0] = true;   m_bUseItem[(int)EN_MAGA_ID.LOAD  , 0] = true;
            m_bUseItem[(int)EN_MAGA_ID.POLISH, 4] = true;   m_bUseItem[(int)EN_MAGA_ID.LOAD  , 2] = true;
            m_bUseItem[(int)EN_MAGA_ID.POLISH, 5] = true;   m_bUseItem[(int)EN_MAGA_ID.LOAD  , 4] = true;
            m_bUseItem[(int)EN_MAGA_ID.POLISH, 6] = true;   m_bUseItem[(int)EN_MAGA_ID.LOAD  , 7] = true;
            m_bUseItem[(int)EN_MAGA_ID.POLISH, 7] = true;   
            
            m_bUseItem[(int)EN_MAGA_ID.CLEAN , 0] = true;   m_bUseItem[(int)EN_MAGA_ID.TRANS , 0] = true;
            m_bUseItem[(int)EN_MAGA_ID.CLEAN , 6] = true;   m_bUseItem[(int)EN_MAGA_ID.TRANS , 1] = true;
            m_bUseItem[(int)EN_MAGA_ID.CLEAN , 7] = true;   m_bUseItem[(int)EN_MAGA_ID.TRANS , 3] = true;
            m_bUseItem[(int)EN_MAGA_ID.CLEAN , 8] = true;   m_bUseItem[(int)EN_MAGA_ID.TRANS , 7] = true;

            //lbSpdlPlate  .Tag = 0;
            //lbSpdlTool   .Tag = 1;

            //
            m_nSelPart = -1;
            m_nSelWhre = -1;
            m_nStorage =  0;
            m_nSelMaga =  0;

            m_nTestTool  = 0;
            m_nTestPlate = 0;

            //
            


        }
        //---------------------------------------------------------------------------
        //Update Timer
        string sToolTip;
        EN_MAGA_ID enMaId; 
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //m_UpdateTimer.Stop();
            m_UpdateTimer.IsEnabled = false;
            
            bool bUsePos = FM.m_stMasterOpt.nUseDirPos == 1; 
            DM.STOR[(int)EN_STOR_ID.POLISH].fn_UpdateMap(ref lbDisplay , bUsePos, true);
            DM.STOR[(int)EN_STOR_ID.CLEAN ].fn_UpdateMap(ref lbDisplay1, bUsePos);

            //Plate
            DM.MAGA[(int)EN_MAGA_ID.POLISH].fn_UpdateMap(ref lbPoliPlate  );
            DM.MAGA[(int)EN_MAGA_ID.CLEAN ].fn_UpdateMap(ref lbClenPlate  );
            DM.MAGA[(int)EN_MAGA_ID.LOAD  ].fn_UpdateMap(ref lbLoad       );
            DM.MAGA[(int)EN_MAGA_ID.TRANS ].fn_UpdateMap(ref lbTrans      );

            enMaId = (EN_MAGA_ID)DM.MAGA[(int)EN_MAGA_ID.POLISH].GetFromMaga();
            sToolTip = string.Format($"MAGA : { enMaId.ToString()} / R:{DM.MAGA[(int)EN_MAGA_ID.POLISH].GetFromRow()} / C:{DM.MAGA[(int)EN_MAGA_ID.POLISH].GetFromCol()}");
            lbPoliPlate.ToolTip = sToolTip;

            enMaId = (EN_MAGA_ID)DM.MAGA[(int)EN_MAGA_ID.CLEAN].GetFromMaga();
            sToolTip = string.Format($"MAGA : { enMaId.ToString()} / R:{DM.MAGA[(int)EN_MAGA_ID.CLEAN].GetFromRow()} / C:{DM.MAGA[(int)EN_MAGA_ID.CLEAN].GetFromCol()}");
            lbClenPlate.ToolTip = sToolTip;

            enMaId = (EN_MAGA_ID)DM.MAGA[(int)EN_MAGA_ID.LOAD].GetFromMaga();
            sToolTip = string.Format($"MAGA : { enMaId.ToString()} / R:{DM.MAGA[(int)EN_MAGA_ID.LOAD].GetFromRow()} / C:{DM.MAGA[(int)EN_MAGA_ID.LOAD].GetFromCol()}");
            lbLoad.ToolTip = sToolTip;
            
            enMaId = (EN_MAGA_ID)DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromMaga();
            sToolTip = string.Format($"MAGA : { enMaId.ToString()} / R:{DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromRow()} / C:{DM.MAGA[(int)EN_MAGA_ID.TRANS].GetFromCol()} / Rcp : {DM.MAGA[(int)EN_MAGA_ID.TRANS].GetPlateRecipeName()}");
            lbTrans.ToolTip = sToolTip;


            //Magazine
            DM.MAGA[(int)EN_MAGA_ID.MAGA01].fn_UpdateMap(ref lbMagz01, true);
            DM.MAGA[(int)EN_MAGA_ID.MAGA02].fn_UpdateMap(ref lbMagz02, true);

            //
            DM.TOOL.fn_UpdateMap(ref lbSpdlPlateMap, ref lbSpdlToolMap, ref lbToolFrc, ref lbNeedChk);
            lbReqPoli.Background = SEQ_SPIND._bReqUtil_Polish ? Brushes.Lime : Brushes.LightGray;


            fn_SetLabelBC (ref lbExtUtil , SEQ_POLIS._bExtUitl  ? Brushes.Lime : Brushes.LightGray); 
            fn_SetLabelTC (ref lbExtUtil , Brushes.Black, SEQ_POLIS.fn_GetUtilString());
            fn_SetLabelBC (ref lbReqDrain, SEQ_POLIS._bReqDrain ? Brushes.Lime : Brushes.LightGray);

            lbRPM    .Content = string.Format($"SPINDLE RPM : {SEQ_SPIND.fn_GetSpindleSpeed()}");
            //lbCrntFos.Content = string.Format($"[LOAD CELL] TOP : {IO.fn_GetTopLoadCell()}");
            lbCrntFos.Content = string.Format($"[LOAD CELL] TOP : {IO.fn_GetTopLoadCellAsBTM ()} g"); //JUNG/200910

            //lbUTLevel.Content = string.Format($"UTIL LEVEL : {IO.fn_GetUTLevelValue()}");
            lbUTLevel.Content = string.Format($"UTIL LEVEL : {IO.fn_GetUTAvgValue()}"); //JUNG/200423/Avg Value

            lbFlowMT1.Content = string.Format($"[FLOW] POLISHING SLURY :  {IO.fn_GetFlowMeter(EN_AINPUT_ID.aiPOL_SlurryFlow ):F1}");
            lbFlowMT2.Content = string.Format($"[FLOW] POLISHING DI    :  {IO.fn_GetFlowMeter(EN_AINPUT_ID.aiPOL_DIWaterFlow):F1}");
            lbFlowMT3.Content = string.Format($"[FLOW] CLEANING  DI    :  {IO.fn_GetFlowMeter(EN_AINPUT_ID.aiCLN_DIWaterFlow):F1}");

            lbLastUtil.Content = string.Format($"LAST Utility : {SEQ_POLIS._enLastUtil.ToString()}");


            lbSelStor.Content = m_nStorage == 0 ? "SEL PART : POLISHING" : m_nStorage == 1 ? "SEL PART : CLEANING" : "-";


            //
            btTest    .Visibility = FM.fn_IsLvlMaster() ? Visibility.Visible : Visibility.Hidden;
            btMapTest .Visibility = FM.fn_IsLvlMaster() ? Visibility.Visible : Visibility.Hidden;
            btMapTest1.Visibility = FM.fn_IsLvlMaster() ? Visibility.Visible : Visibility.Hidden;
            btClrTest .Visibility = FM.fn_IsLvlMaster() ? Visibility.Visible : Visibility.Hidden;

            //
            lbTotalTime.Content = " TOTAL TIME : " + TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dTotalTime);
            lbRunTime  .Content = " RUN TIME   : " + TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dRunTime  );
            lbDownTime .Content = " DOWN TIME  : " + TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dDownTime );
            lbPMTime   .Content = " PM TIME    : " + TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dPMTime   );

            btLotEndForce.IsEnabled = LOT._bLotOpen ? true  : false;
            btWarmStart  .IsEnabled = SEQ._bRun     ? false : true;
            btWarmEnd    .IsEnabled = SEQ._bRun     ? false : true;


            //
            //m_UpdateTimer.Start();
            m_UpdateTimer.IsEnabled = true;
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
        public void mouseevent(object sender, MouseEventArgs e)
        {
            if (SEQ._bRun) return; 

            //Storage Map Setting
            bool bShow = false;

            Label sellbl = sender as Label;
                 if ((sellbl.Parent as Grid).Name == "grd1") { m_nStorage = (int)EN_STOR_ID.POLISH; m_nSelPart = siPolish    ; }
            else if ((sellbl.Parent as Grid).Name == "grd2") { m_nStorage = (int)EN_STOR_ID.CLEAN ; m_nSelPart = siClean + 10; }

            //Point
            m_ptStoPoint = (Point)sellbl.Tag; //(Point)(sender as Label).Tag;

            //tbSelRow.Text = (m_ptStoPoint.X + 1).ToString();
            //tbSelCol.Text = (m_ptStoPoint.Y + 1).ToString();
            
            //Manual Motor Move Function
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (FM.m_stMasterOpt.nUseDirPos == 1)
                {
                    ST_PIN_POS Pos = new ST_PIN_POS(0);
                    Pos = DM.STOR[m_nStorage].PINS[(int)m_ptStoPoint.X, (int)m_ptStoPoint.Y].GetPosition();

                    SEQ_SPIND.fn_MoveDirect(EN_MOTR_ID.miSPD_X, Pos.dXPos);
                    SEQ_STORG.fn_MoveDirect(EN_MOTR_ID.miSTR_Y, Pos.dYPos);
                    
                }
                return;
            }

            if      (Keyboard.IsKeyDown(Key.LeftCtrl )) bShow = fn_SetMenuItem(1); //All
            else if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                if      (e.LeftButton  == MouseButtonState.Pressed) bShow = fn_SetMenuItem(2);
                else if (e.RightButton == MouseButtonState.Pressed) bShow = fn_SetMenuItem(3);
            }
            else bShow = fn_SetMenuItem(); //One

            if (bShow) cmMenu.IsOpen = true ;
            else       cmMenu.IsOpen = false;

        }
        //---------------------------------------------------------------------------
        public void MagaMouseEvent(object sender, MouseEventArgs e)
        {
            /*
            //Magazine Map Setting
            bool bShow = false;

            Label sellbl = sender as Label;
            if ((sellbl.Parent as Grid).Name == "Magz01") m_nSelMaga = (int)EN_MAGA_ID.MAGA01;
            if ((sellbl.Parent as Grid).Name == "Magz02") m_nSelMaga = (int)EN_MAGA_ID.MAGA02; 

            //Point
            m_ptStoPoint = (Point)sellbl.Tag; //(Point)(sender as Label).Tag;
            m_nSelPart   = 3;

            if      (Keyboard.IsKeyDown(Key.LeftCtrl )) bShow = fn_SetMenuItem(1); //All
            else if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                if      (e.LeftButton  == MouseButtonState.Pressed) bShow = fn_SetMenuItem(2);
                else if (e.RightButton == MouseButtonState.Pressed) bShow = fn_SetMenuItem(3);
            }
            else bShow = fn_SetMenuItem(); //One

            if (bShow) cmMenu.IsOpen = true ;
            else       cmMenu.IsOpen = false;
            */

            //
            if (SEQ._bRun) return; 

            //
            FormMapMAGZ.fn_ShowMap();

        }
        //---------------------------------------------------------------------------
        private bool fn_SetMenuItem(int kind = 0)
        {//kind - 0 : One, 1: All, 2:Row, 3:Col
            //
            string sPre  = string.Empty;
            string sItem = string.Empty; 
            int    nIdex = 0 ;

            cmMenu.Items.Clear();
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));

            if (m_nSelPart == 3) //Magazine
            {
                nIdex = 10; 
                for (int i = 0; i < STR_PLATE_STAT.Length; i++)
                {
                    MenuItem item = new MenuItem();
                    if (kind == 0)
                    {
                        sPre = "ONE - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));
                    }
                    else if (kind == 1)
                    {
                        sPre = "ALL - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
                    }
                    else if (kind == 2)
                    {
                        sPre = "ROW - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemRow_Click));
                    }
                    else if (kind == 3)
                    {
                        sPre = "COL  - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemCol_Click));
                    }
                    
                    sItem = STR_PLATE_STAT[i];
                    if (sItem != "EMPTY" && sItem != "READY" && sItem != "FINISH") continue; 

                    item.Header = sPre + STR_PLATE_STAT[i];
                    item.Tag = nIdex++;
                    cmMenu.Items.Add(item);
                }

            }
            else if (m_nSelPart == siClean+10)
            {
                for (int i = 0; i < STR_SEL_PIN_STAT.Length; i++)
                {
                    MenuItem item = new MenuItem();
                    if (kind == 0)
                    {
                        sPre = "ONE - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));
                    }
                    else if (kind == 1)
                    {
                        sPre = "ALL - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
                    }
                    else if (kind == 2)
                    {
                        sPre = "ROW - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemRow_Click));
                    }
                    else if (kind == 3)
                    {
                        sPre = "COL  - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemCol_Click));
                    }

                    
                    
                    item.Header = sPre + STR_SEL_PIN_STAT[i];
                    item.Tag = i;
                    cmMenu.Items.Add(item);
                }
            }
            else if (m_nSelPart == siPolish)
            {
                for (int i = 0; i < STR_SEL_PIN_STAT.Length + MAX_TOOLTYPE; i++)
                {
                    MenuItem item = new MenuItem();
                    if (kind == 0)
                    {
                        sPre = "ONE - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemOne_Click));
                    }
                    else if (kind == 1)
                    {
                        sPre = "ALL - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
                    }
                    else if (kind == 2)
                    {
                        sPre = "ROW - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemRow_Click));
                    }
                    else if (kind == 3)
                    {
                        sPre = "COL  - ";
                        item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemCol_Click));
                    }
                    
                    
                    if (i < STR_SEL_PIN_STAT.Length) item.Header = sPre + STR_SEL_PIN_STAT[i];
                    else
                    {
                        item.Header = sPre + FM.m_stSystemOpt.sToolType[i - STR_SEL_PIN_STAT.Length];
                    }

                    item.Tag = i;
                    cmMenu.Items.Add(item);
                }
            }

            cmMenu.IsOpen = true;


            return true; 
        }


        //---------------------------------------------------------------------------
        private void menuItemOne_Click(object sender, RoutedEventArgs e)
        {//m_nSelPart >> 0:Storage, 1:Plate, 2:Tool, 3: Magazine

            if(SEQ._bRun)
            {
                //fn_UserMsg("Can not be Change while the Machine is running.");
                return; 
            }

            //Local Var.
            //EN_MAGA_STAT  enMagaState  = EN_MAGA_STAT.mzsNone ;
            EN_PIN_STAT   enPinState   = EN_PIN_STAT.psNone   ;
            EN_PLATE_STAT enPlateState = EN_PLATE_STAT.ptsNone;

            MenuItem mu = sender as MenuItem; 


            //int nTag = Array.IndexOf(UserConst.STR_PIN_STAT, mu.Header.ToString().Substring(6));
            int nTag     = Convert.ToInt32(mu.Tag);
            int nTooType = 0; 

            switch (nTag)
            {
                case 0:
                    enPinState   = EN_PIN_STAT.psEmpty   ; 
                    //enMagaState  = EN_MAGA_STAT.mzsEmpty ;
                    enPlateState = EN_PLATE_STAT.ptsEmpty; 
                    break;
                case 1:
                    if(m_nStorage == (int)EN_STOR_ID.POLISH) enPinState = EN_PIN_STAT.psNewPol;
                    else                                     enPinState = EN_PIN_STAT.psNewCln;
                    //enMagaState  = EN_MAGA_STAT.mzsReady  ;
                    enPlateState = EN_PLATE_STAT.ptsReady ;
                    break;
                case 2:
                    enPinState   = EN_PIN_STAT.psUsed     ;
                    //enMagaState  = EN_MAGA_STAT.mzsFinish ;
                    enPlateState = EN_PLATE_STAT.ptsFinish;
                    break;
                case 3:
                    enPinState   = EN_PIN_STAT.psNewPol   ; nTooType = 0; 
                    //enMagaState  = EN_MAGA_STAT.mzsWork   ;
                    enPlateState = EN_PLATE_STAT.ptsAlign ;
                    break;
                case 4:
                    enPinState   = EN_PIN_STAT.psNewPol   ; nTooType = 1;
                    enPlateState = EN_PLATE_STAT.ptsPolish;
                    break;
                case 5:
                    enPinState   = EN_PIN_STAT.psNewPol   ; nTooType = 2;
                    enPlateState = EN_PLATE_STAT.ptsClean ;
                    break;

                case 6:
                    enPinState   = EN_PIN_STAT.psNewPol  ; nTooType = 3;
                    break;

                case 7:
                    enPinState = EN_PIN_STAT.psUsedCln;
                    break;

                case 8:
                    enPinState = EN_PIN_STAT.psMask;
                    break;
                
                case 10: enPlateState = EN_PLATE_STAT.ptsEmpty ; break;
                case 11: enPlateState = EN_PLATE_STAT.ptsReady ; break;
                case 12: enPlateState = EN_PLATE_STAT.ptsFinish; break;

                default:
                    break;
            }


            //
            if (m_nSelPart == 0) //Polishing Storage
            {
                DM.STOR[m_nStorage].SetTo((int)m_ptStoPoint.X, (int)m_ptStoPoint.Y, (int)enPinState, nTooType);

            }
            else if (m_nSelPart == siClean + 10) // Plate
            {
                DM.STOR[m_nStorage].SetTo((int)m_ptStoPoint.X, (int)m_ptStoPoint.Y, (int)enPinState);
            }
            else if (m_nSelPart == 1) // Plate
            {

            }
            else if (m_nSelPart == 2) //Tool
            {

            }
            else if (m_nSelPart == 3) //Magazine
            {
                DM.MAGA[m_nSelMaga].SetTo((int)m_ptStoPoint.X, (int)m_ptStoPoint.Y, (int)enPlateState);
            }


        }
        //---------------------------------------------------------------------------
        private void menuItemAll_Click(object sender, RoutedEventArgs e)
        {
            if(SEQ._bRun)
            {
                //fn_UserMsg("Can not Change while the Machine is running.");
                return; 
            }

            //Local Var.
            //EN_MAGA_STAT  enMagaState  = EN_MAGA_STAT.mzsNone ;
            EN_PIN_STAT   enPinState   = EN_PIN_STAT.psNone   ;
            EN_PLATE_STAT enPlateState = EN_PLATE_STAT.ptsNone;

            MenuItem mu = sender as MenuItem; 

            //int nTag = Array.IndexOf(UserConst.STR_PIN_STAT, mu.Header.ToString().Substring(6));
            int nTag     = Convert.ToInt32(mu.Tag);
            int nTooType = 0;

            switch (nTag)
            {
                case 0:
                    enPinState   = EN_PIN_STAT  .psEmpty ; 
                    //enMagaState  = EN_MAGA_STAT .mzsEmpty;
                    enPlateState = EN_PLATE_STAT.ptsEmpty; 
                    break;
                case 1:
                    if (m_nStorage == (int)EN_STOR_ID.POLISH) enPinState = EN_PIN_STAT.psNewPol; 
                    else                                      enPinState = EN_PIN_STAT.psNewCln;
                   
                    //enMagaState  = EN_MAGA_STAT.mzsReady ;
                    enPlateState = EN_PLATE_STAT.ptsLoad ;
                    break;
                case 2:
                    enPinState   = EN_PIN_STAT.psUsed     ;
                    //enMagaState  = EN_MAGA_STAT.mzsFinish ;
                    enPlateState = EN_PLATE_STAT.ptsPreAlign;
                    break;
                case 3:
                    enPinState   = EN_PIN_STAT.psNewPol  ; nTooType = 0;
                    //enMagaState  = EN_MAGA_STAT.mzsWork  ;
                    enPlateState = EN_PLATE_STAT.ptsReady;
                    break;
                case 4:
                    enPinState   = EN_PIN_STAT.psNewPol  ; nTooType = 1;
                    enPlateState = EN_PLATE_STAT.ptsAlign;
                    break;
                case 5:
                    enPinState   = EN_PIN_STAT.psNewPol  ; nTooType = 2;
                    enPlateState = EN_PLATE_STAT.ptsPolish;
                    break;

                case 6:
                    enPinState   = EN_PIN_STAT.psUsedPol;
                    enPlateState = EN_PLATE_STAT.ptsClean;
                    break;

                case 7:
                    enPinState   = EN_PIN_STAT.psUsedCln;
                    enPlateState = EN_PLATE_STAT.ptsFinish;
                    break;

                case 8:
                    enPinState   = EN_PIN_STAT.psMask;
                    enPlateState = EN_PLATE_STAT.ptsDeHydrate;
                    break;
                
                case 10: enPlateState = EN_PLATE_STAT.ptsEmpty ; break;
                case 11: enPlateState = EN_PLATE_STAT.ptsReady ; break;
                case 12: enPlateState = EN_PLATE_STAT.ptsFinish; break;

                default:
                    break;
            }


            //
            if (m_nSelPart == 0) //Storage
            {
                DM.STOR[m_nStorage].SetTo((int)enPinState, nTooType);
            }
            else if (m_nSelPart == siClean + 10) 
            {
                DM.MAGA[m_nSelWhre].SetTo((int)enPlateState);
            }
            else if (m_nSelPart == 1) // Plate
            {
                DM.MAGA[m_nSelWhre].SetTo((int)enPlateState);

            }
            else if (m_nSelPart == 2) //Tool
            {
                if      (m_nSelWhre == 0) DM.TOOL.PLATES[0].SetTo((int)enPlateState);
                else if (m_nSelWhre == 1) DM.TOOL.PINS  [0].SetTo((int)enPinState  );
            }
            else if (m_nSelPart == 3) //Magazine
            {
                DM.MAGA[m_nSelMaga].SetTo((int)enPlateState);
            }

        }
        //---------------------------------------------------------------------------
        private void menuItemRow_Click(object sender, RoutedEventArgs e)
        {
            if(SEQ._bRun)
            {
                //fn_UserMsg("Can not be Change while the Machine is running.");
                return; 
            }

            //Local Var.
            //EN_MAGA_STAT  enMagaState  = EN_MAGA_STAT.mzsNone ;
            EN_PIN_STAT   enPinState   = EN_PIN_STAT.psNone   ;
            EN_PLATE_STAT enPlateState = EN_PLATE_STAT.ptsNone;

            MenuItem mu = sender as MenuItem; 

            //int nTag = Array.IndexOf(UserConst.STR_PIN_STAT, mu.Header.ToString().Substring(6));
            int nTag     = Convert.ToInt32(mu.Tag);
            int nTooType = 0; 

            switch (nTag)
            {
                case 0:
                    enPinState   = EN_PIN_STAT.psEmpty   ; 
                    //enMagaState  = EN_MAGA_STAT.mzsEmpty ;
                    enPlateState = EN_PLATE_STAT.ptsEmpty; 
                    break;
                case 1:
                    if (m_nStorage == (int)EN_STOR_ID.POLISH) enPinState = EN_PIN_STAT.psNewPol; //enPinState   = EN_PIN_STAT.psNewPol;
                    else                                      enPinState = EN_PIN_STAT.psNewCln;
                    //enMagaState  = EN_MAGA_STAT.mzsReady ;
                    enPlateState = EN_PLATE_STAT.ptsReady;
                    break;
                case 2:
                    enPinState   = EN_PIN_STAT.psUsed     ;
                    //enMagaState  = EN_MAGA_STAT.mzsFinish ;
                    enPlateState = EN_PLATE_STAT.ptsFinish;
                    break;
                case 3:
                    enPinState   = EN_PIN_STAT.psNewPol  ; nTooType = 0; 
                    //enMagaState  = EN_MAGA_STAT.mzsWork  ;
                    enPlateState = EN_PLATE_STAT.ptsAlign;
                    break;
                case 4:
                    enPinState   = EN_PIN_STAT.psNewPol  ; nTooType = 1; 
                    enPlateState = EN_PLATE_STAT.ptsPolish;
                    break;
                case 5:
                    enPinState   = EN_PIN_STAT.psNewPol  ; nTooType = 2; 
                    enPlateState = EN_PLATE_STAT.ptsClean;
                    break;

                case 6:
                    enPinState = EN_PIN_STAT.psUsedPol;
                    break;

                case 7:
                    enPinState = EN_PIN_STAT.psUsedCln;
                    break;

                case 8:
                    enPinState = EN_PIN_STAT.psMask;
                    break;

                case 10: enPlateState = EN_PLATE_STAT.ptsEmpty ; break;
                case 11: enPlateState = EN_PLATE_STAT.ptsReady ; break;
                case 12: enPlateState = EN_PLATE_STAT.ptsFinish; break;

                default:
                    break;
            }


            //
            if (m_nSelPart == 0) //Storage
            {
                DM.STOR[m_nStorage].SetTo((int)enPinState, nTooType);
            }
            else if (m_nSelPart == siClean + 10)
            {
                DM.MAGA[m_nSelWhre].SetTo((int)enPlateState);
            }
            else if (m_nSelPart == 1) // Plate
            {

            }
            else if (m_nSelPart == 2) //Tool
            {

            }
            else if (m_nSelPart == 3) //Magazine
            {
                //DM.MAGA[m_nSelMaga].SetToRow(m_ptStoPoint.Y, (int)enPlateState);
            }

        }
        //---------------------------------------------------------------------------
        private void menuItemCol_Click(object sender, RoutedEventArgs e)
        {
            if(SEQ._bRun)
            {
                //fn_UserMsg("Can not be Change while the Machine is running.");
                return; 
            }

            //Local Var.
            //EN_MAGA_STAT  enMagaState  = EN_MAGA_STAT.mzsNone ;
            EN_PIN_STAT   enPinState   = EN_PIN_STAT.psNone   ;
            EN_PLATE_STAT enPlateState = EN_PLATE_STAT.ptsNone;

            MenuItem mu = sender as MenuItem; 

            //int nTag = Array.IndexOf(UserConst.STR_PIN_STAT, mu.Header.ToString().Substring(6));
            int nTag     = Convert.ToInt32(mu.Tag);
            int nTooType = 0; 

            switch (nTag)
            {
                case 0:
                    enPinState   = EN_PIN_STAT.psEmpty   ; 
                    //enMagaState  = EN_MAGA_STAT.mzsEmpty ;
                    enPlateState = EN_PLATE_STAT.ptsEmpty; 
                    break;
                case 1:
                    if (m_nStorage == (int)EN_STOR_ID.POLISH) enPinState = EN_PIN_STAT.psNewPol; //enPinState   = EN_PIN_STAT.psNewPol;
                    else                                      enPinState = EN_PIN_STAT.psNewCln;
                    //enMagaState  = EN_MAGA_STAT.mzsReady ;
                    enPlateState = EN_PLATE_STAT.ptsReady;
                    break;
                case 2:
                    enPinState   = EN_PIN_STAT.psUsed     ;
                    //enMagaState  = EN_MAGA_STAT.mzsFinish ;
                    enPlateState = EN_PLATE_STAT.ptsFinish;
                    break;
                case 3:
                    enPinState = EN_PIN_STAT.psNewPol    ; nTooType = 0;
                    //enMagaState  = EN_MAGA_STAT.mzsWork  ;
                    enPlateState = EN_PLATE_STAT.ptsAlign;
                    break;
                case 4:
                    enPinState   = EN_PIN_STAT.psNewPol   ; nTooType = 1;
                    enPlateState = EN_PLATE_STAT.ptsPolish;
                    break;
                case 5:
                    enPinState   = EN_PIN_STAT.psNewPol   ; nTooType = 2;
                    enPlateState = EN_PLATE_STAT.ptsClean ;
                    break;

                case 6:
                    enPinState = EN_PIN_STAT.psUsedPol;
                    break;

                case 7:
                    enPinState = EN_PIN_STAT.psUsedCln;
                    break;

                case 8:
                    enPinState = EN_PIN_STAT.psMask;
                    break;

                case 10: enPlateState = EN_PLATE_STAT.ptsEmpty ; break;
                case 11: enPlateState = EN_PLATE_STAT.ptsReady ; break;
                case 12: enPlateState = EN_PLATE_STAT.ptsFinish; break;

                default:
                    break;
            }


            //
            if (m_nSelPart == 0) //Storage
            {
                DM.STOR[m_nStorage].SetTo((int)enPinState, nTooType);
            }
            else if (m_nSelPart == siClean + 10)
            {
                DM.MAGA[m_nSelWhre].SetTo((int)enPlateState);
            }
        }

        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Timer ON
            m_UpdateTimer.IsEnabled = true;
            m_UpdateTimer.Start();

            fn_MapGridSet ();
            fn_MapGridSet1();

            fn_MapGridSet_Maga1();
            fn_MapGridSet_Maga2();

            //
            lbTag1.Background = DM.TOOL.PLATES[0].GetPlateColor(EN_PLATE_STAT.ptsLoad     );
            lbTag2.Background = DM.TOOL.PLATES[0].GetPlateColor(EN_PLATE_STAT.ptsAlign    );
            lbTag3.Background = DM.TOOL.PLATES[0].GetPlateColor(EN_PLATE_STAT.ptsPolish   );
            lbTag4.Background = DM.TOOL.PLATES[0].GetPlateColor(EN_PLATE_STAT.ptsClean    );
            lbTag5.Background = DM.TOOL.PLATES[0].GetPlateColor(EN_PLATE_STAT.ptsFinish   );
            lbTag6.Background = DM.TOOL.PLATES[0].GetPlateColor(EN_PLATE_STAT.ptsDeHydrate);
            lbTag7.Background = Brushes.Maroon; 


            lbReqPoli.ToolTip = "Request Util of Polishing";
            lbToolFrc.ToolTip = "Request Tool Force Check";
            lbNeedChk.ToolTip = "Need Tool Exist Check";

            //
#if !DEBUG
            btMapTest .Visibility = Visibility.Hidden;
            btMapTest1.Visibility = Visibility.Hidden;
            btClrTest .Visibility = Visibility.Hidden;
            btTest    .Visibility = Visibility.Hidden;
#endif


        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Stop();

        }
        //---------------------------------------------------------------------------
        private void Plate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Check Run
            if (SEQ._bRun) return; 
            
            //Plate 
            m_nSelPart = 1;

            cmMenu.Items.Clear();
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
            //
            string sPre = string.Empty;
            Label sellabel = sender as Label;

            //0//"EMPTY"
            //1//"LOADED"
            //2//"PRE-ALIGN"
            //3//"READY"
            //4//"ALIGN"
            //5//"POLISHING"
            //6//"CLEANING"
            //7//"FINISH"
            //8//"DeHydrate"

            m_nSelWhre = Convert.ToInt32(sellabel.Tag);
            if (m_nSelWhre < 0 || m_nSelWhre > 4) return;

            for (int i = 0; i < STR_PLATE_STAT.Length; i++)
            {
                if (i > 8) continue; 

                MenuItem item = new MenuItem();
                sPre = "ALL - ";
                item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
                item.Header = sPre + STR_PLATE_STAT[i];
                item.Tag = i; 

                item.IsEnabled  = m_bUseItem[m_nSelWhre, i] ? true : false;
                item.FontWeight = m_bUseItem[m_nSelWhre, i] ? FontWeights.Bold : FontWeights.Normal;

                cmMenu.Items.Add(item);
            }

            cmMenu.IsOpen = true;

        }
        //---------------------------------------------------------------------------
        private void Tool_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (SEQ._bRun) return; 

            //Tool 
            m_nSelPart = 2;

            cmMenu.Items.Clear();
            cmMenu.RemoveHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));

            //
            string sPre     = string.Empty;
            Label  sellabel = sender as Label;

            m_nSelWhre = Convert.ToInt32(sellabel.Tag);
            
            if (m_nSelWhre < 0 || m_nSelWhre > 2) return; //Pin, Plate

            if (m_nSelWhre == 0) //Plate
            {
                //0//"EMPTY"
                //1//"LOADED"
                //2//"PRE-ALIGN"
                //3//"READY"
                //4//"ALIGN"
                //5//"POLISHING"
                //6//"CLEANING"
                //7//"FINISH"
                //8//"DeHydrate"

                for (int i = 0; i < STR_PLATE_STAT.Length; i++)
                {
                    if (i > 8) continue;

                    MenuItem item = new MenuItem();
                    sPre = "ALL - ";
                    item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));

                    item.Header = sPre + STR_PLATE_STAT[i];
                    item.Tag = i;
                    item.FontSize = 12;
                    if (i == 0 || i == 4 || i == 6 || i == 7) { item.IsEnabled = true ; item.FontWeight = FontWeights.Bold  ; }
                    else                                      { item.IsEnabled = false; item.FontWeight = FontWeights.Normal; }

                    cmMenu.Items.Add(item);
                }

            }
            else if (m_nSelWhre == 1) //Pin
            {
                for (int i = 0; i < STR_SEL_PIN_STAT.Length; i++)
                {
                    MenuItem item = new MenuItem();
                    sPre = "ALL - ";
                    item.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemAll_Click));
                    item.Header = sPre + STR_SEL_PIN_STAT[i];
                    item.Tag = i;
                    item.FontSize = 12;
                    cmMenu.Items.Add(item);
                }

            }
            else return; 

            cmMenu.IsOpen = true;

        }
        //---------------------------------------------------------------------------
        private void StorageMap_Click(object sender, RoutedEventArgs e)
        {
            //Storage Map Setting
            Button SelBtn = sender as Button;

            string sItem = SelBtn.Content.ToString();

            switch (sItem)
            {
                case "ALL EMPTY":
                    DM.STOR[m_nStorage].SetTo((int)EN_PIN_STAT.psEmpty);
                    break;
                case "ALL EXIST":
                    DM.STOR[m_nStorage].SetTo(m_nStorage == 0 ? (int)EN_PIN_STAT.psNewPol : (int)EN_PIN_STAT.psNewCln);
                    break;
                default:
                    break;
            }


            /*
            Int32.TryParse(tbSelRow.Text.ToString(), out int nRow);
            Int32.TryParse(tbSelCol.Text.ToString(), out int nCol);

            if (nRow <= 0 || nRow > DM.STOR[0]._nMaxRow) bOk = false;
            if (nCol <= 0 || nCol > DM.STOR[0]._nMaxCol) bOk = false;

            nRow--;
            nCol--;

            switch (sItem)
            {
                case "ONE EXIST":
                    if (!bOk) return;
                    DM.STOR[m_nStorage].SetTo(nRow, nCol, m_nStorage == 0? (int)EN_PIN_STAT.psNewPol : (int)EN_PIN_STAT.psNewCln);
                    break;
                case "ONE EMPTY":
                    if (!bOk) return;
                    DM.STOR[m_nStorage].SetTo(nRow, nCol, (int)EN_PIN_STAT.psEmpty);
                    break;
                case "ROW EXIST":
                    if (!bOk) return;
                    DM.STOR[m_nStorage].SetToRow(nCol, m_nStorage == 0 ? (int)EN_PIN_STAT.psNewPol : (int)EN_PIN_STAT.psNewCln);
                    break;
                case "COL EXIST":
                    if (!bOk) return;
                    DM.STOR[m_nStorage].SetToCol(nRow, m_nStorage == 0 ? (int)EN_PIN_STAT.psNewPol : (int)EN_PIN_STAT.psNewCln);
                    break;
                case "ROW EMPTY":
                    if (!bOk) return;
                    DM.STOR[m_nStorage].SetToRow(nCol, (int)EN_PIN_STAT.psEmpty);
                    break;
                case "COL EMPTY":
                    if (!bOk) return;
                    DM.STOR[m_nStorage].SetToCol(nRow, (int)EN_PIN_STAT.psEmpty);
                    break;

                case "ALL EMPTY":
                    DM.STOR[m_nStorage].SetTo((int)EN_PIN_STAT.psEmpty);
                    break;
                case "ALL EXIST":
                    DM.STOR[m_nStorage].SetTo(m_nStorage == 0 ? (int)EN_PIN_STAT.psNewPol : (int)EN_PIN_STAT.psNewCln);
                    break;



                default:
                    break;
            }
            */

        }
        //---------------------------------------------------------------------------
        
        private void btMapTest_Click(object sender, RoutedEventArgs e)
        {
            //
            TOOL_PICK_INFO PickInfo = new TOOL_PICK_INFO(false);

            //DM.TOOL.InitPickInfo(ref PickInfo);

            switch (m_nTestTool)
            {
                case 0:

                    //PickInfo = DM.TOOL.GetPickInfoStorage(EN_PIN_FIND_MODE.fmExit);
                    PickInfo = DM.TOOL.GetPickInfoStorage(EN_PIN_FIND_MODE.fmNewPol);

                    if (!PickInfo.bFind)
                    {
                        fn_UserMsg("NO POLISHINGN TOOL!!!!");
                        return;
                    }

                    DM.ShiftPinData(ref DM.STOR[siPolish].PINS[PickInfo.nFindRow, PickInfo.nFindCol], ref DM.TOOL.PINS[0]);

                    m_nTestTool++;
                    break;

                case 1:
                    DM.TOOL.SetExistCheck(true);
                    m_nTestTool++;
                    break;

                case 2:
                    DM.TOOL.SetForceCheck(true);
                    m_nTestTool++;
                    break;

                case 3:
                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);
                    m_nTestTool++;
                    break;

                case 4:
                    DM.TOOL.ClearMapPin();
                    m_nTestTool++;
                    break;

                case 5:

                    PickInfo = DM.TOOL.GetPickInfoStorage(EN_PIN_FIND_MODE.fmNewCln);

                    if (!PickInfo.bFind)
                    {
                        fn_UserMsg("NO POLISHINGN TOOL!!!!");
                        m_nTestTool = 0;
                        return;
                    }

                    DM.ShiftPinData(ref DM.STOR[siClean].PINS[PickInfo.nFindRow, PickInfo.nFindCol], ref DM.TOOL.PINS[0]);
                    m_nTestTool++;
                    break;

                case 6:
                    DM.TOOL.SetExistCheck(true);
                    m_nTestTool++;
                    break;

                case 7:
                    DM.TOOL.SetForceCheck(true);
                    m_nTestTool++;
                    break;

                case 8:
                    DM.TOOL.SetTo((int)EN_PIN_STAT.psUsed);
                    m_nTestTool++;
                    break;

                case 9:
                    DM.TOOL.ClearMapPin();
                    m_nTestTool++;
                    break;

                case 10:

                    m_nTestTool = 0;
                    break;

                default:
                    m_nTestTool = 0;
                    break; 
            }

        }
        //---------------------------------------------------------------------------
        private void btMapTest_Click1(object sender, RoutedEventArgs e)
        {

            PLATE_MOVE_INFO PickInfo = new PLATE_MOVE_INFO(false);
            //PickInfo = SEQ_TRANS.fn_GetPickInfoMaga();

            //
            switch (m_nTestPlate)
            {
                case 0:

                    //
                    PickInfo = SEQ_TRANS.fn_GetPickInfoMaga();
                    if(!PickInfo.bFind)
                    {
                        return;
                    }
                    //Magazine -> Transfer
                    DM.ShiftPlateData(ref DM.MAGA[PickInfo.nMagaId].PLATE[PickInfo.nFindRow, PickInfo.nFindCol], ref DM.MAGA[(int)EN_MAGA_ID.TRANS].PLATE[0, 0]);
                    DM.fn_SetPlateData(EN_MAGA_ID.TRANS, PickInfo);
                    m_nTestPlate++;
                    break;

                case 1:
                    //Change Map : Load
                    DM.MAGA[(int)EN_MAGA_ID.TRANS].SetTo((int)EN_PLATE_STAT.ptsLoad);
                    m_nTestPlate++;
                    break;

                case 2:
                    //Move : Trans --> Load 
                    DM.ShiftPlateData(EN_MAGA_ID.TRANS, EN_MAGA_ID.LOAD);
                    m_nTestPlate++;
                    break;

                case 3:
                    DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo((int)EN_PLATE_STAT.ptsPreAlign);
                    m_nTestPlate++;
                    break;

                case 4:
                    DM.MAGA[(int)EN_MAGA_ID.LOAD].SetTo((int)EN_PLATE_STAT.ptsAlign);
                    m_nTestPlate++;
                    break;


                case 5:
                    DM.ShiftPlateData(EN_PLATE_ID.ptiLoad, EN_PLATE_ID.ptiSpindl);
                    m_nTestPlate++;
                    break;

                case 6:
                    DM.ShiftPlateData(EN_PLATE_ID.ptiSpindl, EN_PLATE_ID.ptiPolish);
                    m_nTestPlate++;
                    break;
                
                case 7:
                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsPolish);
                    m_nTestPlate++;
                    break;

                case 8:
                    DM.MAGA[(int)EN_MAGA_ID.POLISH].SetTo((int)EN_PLATE_STAT.ptsClean);
                    m_nTestPlate++;
                    break;
                
                case 9:
                    DM.ShiftPlateData(EN_PLATE_ID.ptiPolish, EN_PLATE_ID.ptiSpindl);
                    m_nTestPlate++;
                    break;

                case 10:
                    DM.ShiftPlateData(EN_PLATE_ID.ptiSpindl, EN_PLATE_ID.ptiClean);
                    m_nTestPlate++;
                    break;

                case 11:
                    DM.MAGA[(int)EN_MAGA_ID.CLEAN].SetTo((int)EN_PLATE_STAT.ptsFinish);
                    m_nTestPlate++;
                    break;

                case 12:
                    DM.ShiftPlateData(EN_PLATE_ID.ptiClean, EN_PLATE_ID.ptiSpindl);
                    m_nTestPlate++;
                    break;

                case 13:
                    DM.ShiftPlateData(EN_PLATE_ID.ptiSpindl, EN_PLATE_ID.ptiLoad);
                    m_nTestPlate++;
                    break;
                
                case 14:
                    DM.ShiftPlateData(EN_MAGA_ID.LOAD, EN_MAGA_ID.TRANS);
                    m_nTestPlate++;
                    break;

                case 15:

                    PickInfo = SEQ_TRANS.fn_GetPlaceInfoMaga();
                    if (!PickInfo.bFind)
                    {
                        fn_UserMsg("NO EMPTY MAGAZINE!!!!");
                        return;
                    }

                    //Magazine -> Transfer
                    DM.ShiftPlateData(ref DM.MAGA[(int)EN_MAGA_ID.TRANS].PLATE[0, 0], ref DM.MAGA[PickInfo.nMagaId].PLATE[PickInfo.nFindRow, PickInfo.nFindCol]);
                                            
                    m_nTestPlate = 0;
                    break;
                
                default:
                    m_nTestPlate = 0; 
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void lbToolExt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Check Run
            if (SEQ._bRun) return;

            Label sellabel = sender as Label;

            m_nSelWhre = Convert.ToInt32(sellabel.Tag);

            if (m_nSelWhre < 2 || m_nSelWhre > 4) return; //2:Exist, 3:Force, 4: Need Check
            
            string stemp = string.Empty;
            
            if (m_nSelWhre == 2) //
            {
//                 bool bToolExt = DM.TOOL.PINS[0].IsExistCheck();
// 
//                 stemp = String.Format("Change Tool - {0} ?", bToolExt ? "Empty" : "Exist");
//                 if (UserFunction.fn_UserMsg(stemp, EN_MSG_TYPE.Check))
//                 {
//                     DM.TOOL.PINS[0].SetExistCheck(!bToolExt);// = !DM.TOOL.PINS[0]._bExistChk;
// 
//                     UserFunction.fn_WriteLog("Change Tool Exist");
//                 }

                bool bReqUtilPOL = SEQ_SPIND._bReqUtil_Polish;

                stemp = string.Format("Change Polishing Util Check - {0} ?", bReqUtilPOL ? "OFF" : "ON");
                if (UserFunction.fn_UserMsg(stemp, EN_MSG_TYPE.Check))
                {
                    SEQ_SPIND._bReqUtil_Polish = !SEQ_SPIND._bReqUtil_Polish;

                    UserFunction.fn_WriteLog("Change Polishing Util Check");
                }
            }
            else if (m_nSelWhre == 3) //Force
            {
                bool bToolForce = DM.TOOL.PINS[0].IsForceCheck();

                stemp = string.Format("Change Tool Force Check - {0} ?", bToolForce ? "Need" : "Done");
                if (UserFunction.fn_UserMsg(stemp, EN_MSG_TYPE.Check))
                {
                    DM.TOOL.PINS[0].SetForceCheck(!bToolForce);

                    UserFunction.fn_WriteLog("Change Tool Force Check");
                }

            }
            else if (m_nSelWhre == 4) //Need Check
            {
                bool bNeedChk = DM.TOOL.IsNeedCheck();

                stemp = string.Format("Change Tool Check - {0} ?", bNeedChk ? "DONE" : "NEED");
                if (UserFunction.fn_UserMsg(stemp, EN_MSG_TYPE.Check))
                {
                    DM.TOOL.SetNeedCheck(!bNeedChk);

                    UserFunction.fn_WriteLog("Change Tool Need Check");
                }
            }
        }
        //---------------------------------------------------------------------------
        private void btClrTest_Click(object sender, RoutedEventArgs e)
        {
            m_nTestPlate = m_nTestTool = 0;

            //UserFunction.fn_UserJog();

        }
        //---------------------------------------------------------------------------
        private void lbExtUtil_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Check Run
            if (SEQ._bRun) return;

            //
            string stemp = string.Empty; 
            Label sellbl = sender as Label;
            int nTag = Convert.ToInt32(sellbl.Tag);

            if (nTag == 1) //lbExtUtil
            {
                bool bExtUtil = SEQ_POLIS._bExtUitl;

                stemp = string.Format("Change Polishing Utility - {0} ?", bExtUtil ? "EMPTY" : "EXIST");
                if (UserFunction.fn_UserMsg(stemp, EN_MSG_TYPE.Check))
                {
                    SEQ_POLIS.fn_SetUtilState(bExtUtil ? EN_UTIL_STATE.Empty : EN_UTIL_STATE.Exist);

                    UserFunction.fn_WriteLog("Change Polishing Utility State.");
                }
            }
            else if(nTag == 2) //lbReqDrain
            {
                bool bReqDrain = SEQ_POLIS._bReqDrain;

                stemp = string.Format("Change Polishing Drain - {0} ?", bReqDrain ? "OFF" : "ON");
                if (UserFunction.fn_UserMsg(stemp, EN_MSG_TYPE.Check))
                {
                    SEQ_POLIS._bReqDrain = !SEQ_POLIS._bReqDrain;

                    UserFunction.fn_WriteLog("Change Polishing Request Drain.");
                }

            }


        }
        //---------------------------------------------------------------------------
        private void btTest_Click(object sender, RoutedEventArgs e)
        {

            //bool   con = REST.fn_GetConnect();
            //string ver = REST.fn_GetVersion();
            //
            //if(REST.fn_GetRFIDInfo("1"))
            //{
            //    string sid   = REST.RcvPlateRFIDInfo._waferInfo.sLotID;
            //    string stype = REST.RcvPlateRFIDInfo._stype;
            //}

            //double dValue = 0.0;
            //double.TryParse(tbTestDCOM.Text, out dValue);
            //if(dValue < 1)
            //{
            //    return;
            //}
            //
            //SEQ_SPIND._dTestDCOM = dValue; 
            //

            //MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0418, true, false);


            //IO.fn_SetSoftLimit((int)EN_MOTR_ID.miSPD_Z, false, false);
            //IO.fn_SetSoftLimit_Z(true, SEQ_SPIND.GetEncPos_Z() - 5);

            //DM.MAGA[(int)EN_MAGA_ID.POLISH].SetInfo(EN_PLATE_INFO.ifVisnErr);
            //double dTemp = SEQ_POLIS.GetEncPos_TH();
            //bool rtn = MOTR.CheckMinMaxP(EN_MOTR_ID.miPOL_TH, dTemp);
            
            Class1 test = new ControlCore.Class1();

            test.fn_Test("Test");
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Form Loading 시 Grid Setting
        </summary>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/18 10:13
        */
        private void fn_MapGridSet()
        {
            int id = (int)EN_STOR_ID.POLISH; 
            //
            nX = DM.STOR[id]._nMaxCol; 
            nY = DM.STOR[id]._nMaxRow;
            
            grd1.Children.Clear();
            grd1.Background = Brushes.White;
            grd1.ColumnDefinitions.Clear();
            grd1.RowDefinitions.Clear();

            int idx  = 0, idxNo = 0;
            int nCol = nX;
            int nRow = nY;
            int iR   = 0, iC = 0; 

            lbDisplay = (Label[,]) fn_ResizeArray(lbDisplay, new int[] { nY, nX});

            for (int C = 0; C < nCol; C++)
            {
                grd1.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int R = 0; R < nRow; R++)
            {
                grd1.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < nCol; c++)
                {

                    switch ((EN_DIS_DIR)DM.STOR[id]._nDir)
                    {
                     
                        case EN_DIS_DIR.ddTopLeft  : iR = r;              iC = c;              break; 
                        case EN_DIS_DIR.ddTopRight : iR = r             ; iC = (nCol - 1) - c; break; 
                        case EN_DIS_DIR.ddBtmRight : iR = (nRow - 1) - r; iC = (nCol - 1) - c; break; 
						case EN_DIS_DIR.ddBtmLeft  : iR = (nRow - 1) - r; iC = c;              break; 

                        default: return; 
                    }

                    idx   = r  * nCol + c ;
                    idxNo = iR * nCol + iC;

                    grd1.Children.Add(new Label());
                    Label lb           = (grd1.Children[idx] as Label);
                    lb.Content         = "NO." + idxNo;
                    lb.BorderThickness = new Thickness(1);
                    lb.BorderBrush     = System.Windows.Media.Brushes.Black;
                    lb.Tag             = new Point(iR, iC);
                    lb.MouseDown      += mouseevent;
                    lb.Margin          = new Thickness(1);
                    lb.FontSize        = 11;

                    Grid.SetColumn((grd1.Children[idx] as Label), c);
                    Grid.SetRow   ((grd1.Children[idx] as Label), r);

                    lbDisplay[iR, iC] = lb;

                }
            }

            Border boder          = new Border();
            boder.BorderBrush     = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin          = new Thickness(-2);
            Grid.SetRowSpan   (boder, 100);
            Grid.SetColumnSpan(boder, 100);
            grd1.Children.Add (boder     );
        }
        //---------------------------------------------------------------------------
        private void fn_MapGridSet1()
        {
            int id = (int)EN_STOR_ID.CLEAN; 

            //
            nX = DM.STOR[id]._nMaxCol; 
            nY = DM.STOR[id]._nMaxRow;
            
            grd2.Children.Clear();
            grd2.Background = Brushes.White;
            grd2.ColumnDefinitions.Clear();
            grd2.RowDefinitions.Clear();

            int idx  = 0, idxNo = 0;
            int nCol = nX;
            int nRow = nY;
            int iR   = 0, iC = 0; 

            lbDisplay1 = (Label[,]) fn_ResizeArray(lbDisplay1, new int[] { nY, nX});

            for (int C = 0; C < nCol; C++)
            {
                grd2.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int R = 0; R < nRow; R++)
            {
                grd2.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < nCol; c++)
                {

                    switch ((EN_DIS_DIR)DM.STOR[id]._nDir)
                    {
                     
                        case EN_DIS_DIR.ddTopLeft  : iR = r;              iC = c;              break; 
                        case EN_DIS_DIR.ddTopRight : iR = r             ; iC = (nCol - 1) - c; break; 
                        case EN_DIS_DIR.ddBtmRight : iR = (nRow - 1) - r; iC = (nCol - 1) - c; break; 
						case EN_DIS_DIR.ddBtmLeft  : iR = (nRow - 1) - r; iC = c;              break; 

                        default: return; 
                    }

                    idx   = r  * nCol + c ;
                    idxNo = iR * nCol + iC;

                    grd2.Children.Add(new Label());
                    Label lb           = (grd2.Children[idx] as Label);
                    lb.Content         = "NO." + idxNo;
                    lb.BorderThickness = new Thickness(1);
                    lb.BorderBrush     = System.Windows.Media.Brushes.Black;
                    lb.Tag             = new Point(iR, iC);
                    lb.MouseDown      += mouseevent;
                    lb.Margin          = new Thickness(1);
                    lb.FontSize        = 11;


                    Grid.SetColumn((grd2.Children[idx] as Label), c);
                    Grid.SetRow   ((grd2.Children[idx] as Label), r);

                    lbDisplay1[iR, iC] = lb;

                }
            }

            Border boder          = new Border();
            boder.BorderBrush     = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin          = new Thickness(-2);
            Grid.SetRowSpan   (boder, 100);
            Grid.SetColumnSpan(boder, 100);
            grd2.Children.Add (boder     );
        }

        //---------------------------------------------------------------------------
        private void fn_MapGridSet_Maga1()
        {
            int id = (int)EN_MAGA_ID.MAGA01; 
            //
            nX = DM.MAGA[id]._nMaxCol; 
            nY = DM.MAGA[id]._nMaxRow;

            Magz01.Children.Clear();
            Magz01.Background = Brushes.White;
            Magz01.ColumnDefinitions.Clear();
            Magz01.RowDefinitions.Clear();

            int idx  = 0, idxNo = 0;
            int nCol = nX;
            int nRow = nY;
            int iR   = 0, iC = 0;

            lbMagz01 = (Label[,]) fn_ResizeArray(lbMagz01, new int[] { nY, nX});

            for (int C = 0; C < nCol; C++)
            {
                Magz01.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int R = 0; R < nRow; R++)
            {
                Magz01.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < nCol; c++)
                {
                    switch ((EN_DIS_DIR)DM.MAGA[id]._nDirection)
                    {
                        case EN_DIS_DIR.ddTopLeft  : iR = r;              iC = c;              break; 
                        case EN_DIS_DIR.ddTopRight : iR = r             ; iC = (nCol - 1) - c; break; 
                        case EN_DIS_DIR.ddBtmRight : iR = (nRow - 1) - r; iC = (nCol - 1) - c; break; 
						case EN_DIS_DIR.ddBtmLeft  : iR = (nRow - 1) - r; iC = c;              break; 

                        default: return; 
                    }

                    idx   = r  * nCol + c ;
                    idxNo = iR * nCol + iC;

                    Magz01.Children.Add(new Label());
                    Label lb           = (Magz01.Children[idx] as Label);
                    lb.Content         = "NO." + idxNo;
                    lb.BorderThickness = new Thickness(1);
                    lb.BorderBrush     = System.Windows.Media.Brushes.Black;
                    lb.Tag             = new Point(iR, iC);
                    lb.MouseDown      += MagaMouseEvent;
                    lb.Margin          = new Thickness(1);
                    lb.FontSize        = 11;
                    lb.Padding         = new Thickness(0);
                    lb.VerticalContentAlignment = VerticalAlignment.Center;

                    Grid.SetColumn((Magz01.Children[idx] as Label), c);
                    Grid.SetRow   ((Magz01.Children[idx] as Label), r);

                    lbMagz01[iR, iC] = lb;

                }
            }

            Border boder          = new Border();
            boder.BorderBrush     = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin          = new Thickness(-2);
            Grid.SetRowSpan     (boder, 100);
            Grid.SetColumnSpan  (boder, 100);
            Magz01.Children.Add (boder     );
        }
        //---------------------------------------------------------------------------
        private void btWarm_Click(object sender, RoutedEventArgs e)
        {
            Button SelBtn = sender as Button;

            string sItem = SelBtn.Content.ToString();

            switch (sItem)
            {
                case "Warming up Start":
                    if(SEQ._bAuto) //JUNG/210113
                    {
                        fn_UserMsg("Manual Mode로 변경 하세요.");
                        break;
                    }
                    MAN.fn_SetWarmFun();
                    break;

                case "Warming up End":
                    MAN.fn_SetWarmFun(false);
                    break;
                default:
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void btLotEndForce_Click(object sender, RoutedEventArgs e)
        {
            if (!LOT._bLotOpen) return;

            //Force Lot End
            LOT.fn_ForceLotEnd();
        }

        //---------------------------------------------------------------------------
        private void fn_MapGridSet_Maga2()
        {
            int id = (int)EN_MAGA_ID.MAGA02; 
            //
            nX = DM.MAGA[id]._nMaxCol; 
            nY = DM.MAGA[id]._nMaxRow;

            Magz02.Children.Clear();
            Magz02.Background = Brushes.White;
            Magz02.ColumnDefinitions.Clear();
            Magz02.RowDefinitions.Clear();

            int idx  = 0, idxNo = 0;
            int nCol = nX;
            int nRow = nY;
            int iR   = 0, iC = 0;

            lbMagz02 = (Label[,]) fn_ResizeArray(lbMagz02, new int[] { nY, nX});

            for (int C = 0; C < nCol; C++)
            {
                Magz02.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int R = 0; R < nRow; R++)
            {
                Magz02.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < nRow; r++)
            {
                for (int c = 0; c < nCol; c++)
                {
                    switch ((EN_DIS_DIR)DM.MAGA[id]._nDirection)
                    {
                        case EN_DIS_DIR.ddTopLeft  : iR = r;              iC = c;              break; 
                        case EN_DIS_DIR.ddTopRight : iR = r             ; iC = (nCol - 1) - c; break; 
                        case EN_DIS_DIR.ddBtmRight : iR = (nRow - 1) - r; iC = (nCol - 1) - c; break; 
						case EN_DIS_DIR.ddBtmLeft  : iR = (nRow - 1) - r; iC = c;              break; 

                        default: return; 
                    }

                    idx   = r  * nCol + c ;
                    idxNo = iR * nCol + iC;

                    Magz02.Children.Add(new Label());
                    Label lb           = (Magz02.Children[idx] as Label);
                    lb.Content         = "NO." + idxNo;
                    lb.BorderThickness = new Thickness(1);
                    lb.BorderBrush     = System.Windows.Media.Brushes.Black;
                    lb.Tag             = new Point(iR, iC);
                    lb.MouseDown      += MagaMouseEvent;
                    lb.Margin          = new Thickness(1);
                    lb.FontSize        = 11;
                    lb.Padding         = new Thickness(0);
                    lb.VerticalContentAlignment = VerticalAlignment.Center;


                    Grid.SetColumn((Magz02.Children[idx] as Label), c);
                    Grid.SetRow   ((Magz02.Children[idx] as Label), r);

                    lbMagz02[iR, iC] = lb;

                }
            }

            Border boder          = new Border();
            boder.BorderBrush     = System.Windows.Media.Brushes.Black;
            boder.BorderThickness = new System.Windows.Thickness(1);
            boder.Margin          = new Thickness(-2);
            Grid.SetRowSpan     (boder, 100);
            Grid.SetColumnSpan  (boder, 100);
            Magz02.Children.Add (boder     );
        }












    }
}
