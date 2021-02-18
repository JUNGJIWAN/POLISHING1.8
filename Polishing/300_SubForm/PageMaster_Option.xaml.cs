using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
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
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;


namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageMaster_Option : Page
    {
        //Var
        private CheckBox[] cbOffAR = new CheckBox[10];
        private string sTemp; 
        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageMaster_Option()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            //Init
            fn_InitComponent();



        }
        //---------------------------------------------------------------------------
        private void fn_InitComponent()
        {
            for (int i = 0; i < 10; i++)
            {
                cbOffAR[i] = new CheckBox();
            }

            cbOffAR[0] = cbOffAR0;
            cbOffAR[1] = cbOffAR1;
            cbOffAR[2] = cbOffAR2;
            cbOffAR[3] = cbOffAR3;
            cbOffAR[4] = cbOffAR4;
            
            //cbOffAR[5] = cbOffAR5;
            //cbOffAR[6] = cbOffAR6;
            //cbOffAR[7] = cbOffAR7;
            //cbOffAR[8] = cbOffAR8;
            //cbOffAR[9] = cbOffAR9;

            for (int i = 0; i < 10; i++)
            {
                cbOffAR[i].Visibility = Visibility.Hidden;
            }


        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            lbTopForce  .Content = string.Format($"[TOP LOAD CELL] {IO.fn_GetTopLoadCell()}g / ORGING: {IO.fn_GetTopLoadCellValue()}g");
            lbLoadOffset.Content = string.Format($"[Offset] = {IO._dTopLoadCellOffSet}");

            lbAutoCalStep.Content = string.Format($"[{SEQ_SPIND._nManStep}]  {SEQ_SPIND._nCalCycle} / {SEQ_SPIND._nTotalCalCycle} | {SEQ_SPIND._nAutoCalCount} / {SEQ_SPIND._nAutoCalTotalCnt}");

            
            
            //
            m_UpdateTimer.Start();
        }
        //---------------------------------------------------------------------------
        //Timer On/Off
        public void fn_SetTimer(bool on)
        {
            if (on)
            {
                m_UpdateTimer.Start();
                m_UpdateTimer.IsEnabled = true; 
            }
            else m_UpdateTimer.Stop();

        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            for (int i=0; i< MOTR.GetPartCnt(); i++)
            {
                if (i == 0) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 1) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 2) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 3) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 4) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 5) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 6) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 7) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 8) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
                if (i == 9) { cbOffAR[i].Visibility = Visibility.Visible; cbOffAR[i].Content = MOTR.fn_GetPartName(i); cbOffAR[i].IsChecked = FM.m_stMasterOpt.bAutoOff[i]; }
            }

            if      (FM.m_stMasterOpt.nRunMode == EN_RUN_MODE.AUTO_MODE) rbAuto.IsChecked = true;
            else if (FM.m_stMasterOpt.nRunMode == EN_RUN_MODE.TEST_MODE) rbTEST.IsChecked = true;
            else if (FM.m_stMasterOpt.nRunMode == EN_RUN_MODE.MAN_MODE ) rbMAN .IsChecked = true;


            cbSkipDoor      .IsChecked  = (FM.m_stMasterOpt.nUseSkipDoor      == 1) ? true : false;
            cbSkipLeak      .IsChecked  = (FM.m_stMasterOpt.nUseSkipLeak      == 1) ? true : false;
            cbSkipFan       .IsChecked  = (FM.m_stMasterOpt.nUseSkipFan       == 1) ? true : false;
            cbSkipAir       .IsChecked  = (FM.m_stMasterOpt.nUseSkipAir       == 1) ? true : false;
            cbSkipWtLvl     .IsChecked  = (FM.m_stMasterOpt.nUseSkipWaterLvl  == 1) ? true : false;
            cbSkipWtLeak    .IsChecked  = (FM.m_stMasterOpt.nUseSkipWaterLeak == 1) ? true : false;
            cbSkipAccura    .IsChecked  = (FM.m_stMasterOpt.nUseSkipAccura    == 1) ? true : false;
            cbSkipDP        .IsChecked  = (FM.m_stMasterOpt.nUseSkipDP        == 1) ? true : false;             
                            
            cbSkipTool      .IsChecked  = (FM.m_stMasterOpt.nToolSkip         == 1) ? true : false;
            cbSkipPlate     .IsChecked  = (FM.m_stMasterOpt.nPlateSkip        == 1) ? true : false;
            cbSkipStrg      .IsChecked  = (FM.m_stMasterOpt.nStorageSkip      == 1) ? true : false;
            cbSkipMaga      .IsChecked  = (FM.m_stMasterOpt.nMagaSkip         == 1) ? true : false;
            cbUseVisn       .IsChecked  = (FM.m_stMasterOpt.nUseVision        == 1) ? true : false;
            cbUseDirPos     .IsChecked  = (FM.m_stMasterOpt.nUseDirPos        == 1) ? true : false;
            cbUseCalFoc     .IsChecked  = (FM.m_stMasterOpt.nUseCalForce      == 1) ? true : false;
            cbUseREST       .IsChecked  = (FM.m_stMasterOpt.nUseRESTApi       == 1) ? true : false;
            cbUseOnlyDI     .IsChecked  = (FM.m_stMasterOpt.nUseDI            == 1) ? true : false;
            cbEPDOnlyMeasure.IsChecked  = (FM.m_stMasterOpt.nEPDOnlyMeasure   == 1) ? true : false;
            cbUseMOC        .IsChecked  = (FM.m_stMasterOpt.nUseMOC           == 1) ? true : false;
            cbUseCleanPos   .IsChecked  = (FM.m_stMasterOpt.nUseCleanPos      == 1) ? true : false;

            cbUsePMC     .IsChecked  = (FM.m_stMasterOpt.nUsePMC           == 1) ? true : false;
            cbUseDCOMRst .IsChecked  = (FM.m_stMasterOpt.nUseDCOMReset     == 1) ? true : false;
            //cbUseAutoSlry.IsChecked  = (FM.m_stMasterOpt.nUseAutoSlury     == 1) ? true : false;


            //
            tbDrainTime    .Text = FM.m_stMasterOpt.nDrainTime    .ToString();
            tbSepBlowTime  .Text = FM.m_stMasterOpt.nSepBlowTime  .ToString();
            tbSuckBackTime .Text = FM.m_stMasterOpt.nSuckBackTime .ToString();
                                                                  
            tbTopOffset    .Text = FM.m_stMasterOpt.dUtilOffset   .ToString();
                                                                  
            tbTrPickOffset .Text = FM.m_stMasterOpt.dPickOffset   .ToString();
            tbTrPlaceOffset.Text = FM.m_stMasterOpt.dPlaceOffset  .ToString();
                                                                  
            tbUtilTimeout  .Text = FM.m_stMasterOpt.nUtilMaxTime  .ToString();
                                                                  
            tbYIntercept   .Text = FM.m_stMasterOpt.dYIntercept   .ToString();
            tbYSlope       .Text = FM.m_stMasterOpt.dYSlope       .ToString();
            tbforceOffset  .Text = FM.m_stMasterOpt.dforceOffset  .ToString();

            tbYInterceptBT .Text = FM.m_stMasterOpt.dYInterceptBT .ToString();
            tbYSlopeBT     .Text = FM.m_stMasterOpt.dYSlopeBT     .ToString();

            tbDCOMRatio    .Text = FM.m_stMasterOpt.dDCOMRatio    .ToString();
            tbDCOMCnt      .Text = FM.m_stMasterOpt.nDCOMCnt      .ToString();

            tbSpdOffset    .Text = FM.m_stMasterOpt.dSpdOffset    .ToString();

            tbCalStart     .Text = FM.m_stMasterOpt.dStartDCOM    .ToString();

            //tbLDCBtmOffset .Text = FM.m_stMasterOpt.dLDCBtmOffset.ToString();

            fn_SetTimer(true);


        }
        //---------------------------------------------------------------------------
        public void fn_SaveToBuff()
        {

            for (int i=0; i< MOTR.GetPartCnt(); i++)
            {
                FM.m_stMasterOpt.bAutoOff[i] = (bool)cbOffAR[i].IsChecked ;
            }

            if      (rbAuto.IsChecked == true) FM.m_stMasterOpt.nRunMode = EN_RUN_MODE.AUTO_MODE;
            else if (rbTEST.IsChecked == true) FM.m_stMasterOpt.nRunMode = EN_RUN_MODE.TEST_MODE;
            else if (rbMAN .IsChecked == true) FM.m_stMasterOpt.nRunMode = EN_RUN_MODE.MAN_MODE ;

            FM.m_stMasterOpt.nUseSkipDoor      = cbSkipDoor      .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseSkipLeak      = cbSkipLeak      .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseSkipFan       = cbSkipFan       .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseSkipAir       = cbSkipAir       .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseSkipWaterLvl  = cbSkipWtLvl     .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseSkipWaterLeak = cbSkipWtLeak    .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseSkipAccura    = cbSkipAccura    .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseSkipDP        = cbSkipDP        .IsChecked == true ? 1 : 0;
                                                                 
            FM.m_stMasterOpt.nToolSkip         = cbSkipTool      .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nPlateSkip        = cbSkipPlate     .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nStorageSkip      = cbSkipStrg      .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nMagaSkip         = cbSkipMaga      .IsChecked == true ? 1 : 0;
                                                                 
            FM.m_stMasterOpt.nUseVision        = cbUseVisn       .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseDirPos        = cbUseDirPos     .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseCalForce      = cbUseCalFoc     .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseRESTApi       = cbUseREST       .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseDI            = cbUseOnlyDI     .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nEPDOnlyMeasure   = cbEPDOnlyMeasure.IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseMOC           = cbUseMOC        .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseCleanPos      = cbUseCleanPos   .IsChecked == true ? 1 : 0;

            FM.m_stMasterOpt.nUsePMC           = cbUsePMC     .IsChecked == true ? 1 : 0;
            FM.m_stMasterOpt.nUseDCOMReset     = cbUseDCOMRst .IsChecked == true ? 1 : 0;
            //FM.m_stMasterOpt.nUseAutoSlury     = cbUseAutoSlry.IsChecked == true ? 1 : 0;


            //
            int   .TryParse(tbDrainTime    .Text, out FM.m_stMasterOpt.nDrainTime     );
            int   .TryParse(tbSepBlowTime  .Text, out FM.m_stMasterOpt.nSepBlowTime   );
            int   .TryParse(tbSuckBackTime .Text, out FM.m_stMasterOpt.nSuckBackTime  );
                                                                                      
            double.TryParse(tbTrPickOffset .Text, out FM.m_stMasterOpt.dPickOffset    );
            double.TryParse(tbTrPlaceOffset.Text, out FM.m_stMasterOpt.dPlaceOffset   );
                                                                                      
            int   .TryParse(tbUtilTimeout  .Text, out FM.m_stMasterOpt.nUtilMaxTime   );
            int   .TryParse(tbDCOMCnt      .Text, out FM.m_stMasterOpt.nDCOMCnt       );
                                                                                      
            double.TryParse(tbYIntercept   .Text, out FM.m_stMasterOpt.dYIntercept    );
            double.TryParse(tbYSlope       .Text, out FM.m_stMasterOpt.dYSlope        );
            double.TryParse(tbforceOffset  .Text, out FM.m_stMasterOpt.dforceOffset   );
            double.TryParse(tbYInterceptBT .Text, out FM.m_stMasterOpt.dYInterceptBT  ); //JUNG/200908
            double.TryParse(tbYSlopeBT     .Text, out FM.m_stMasterOpt.dYSlopeBT      );

            double.TryParse(tbDCOMRatio    .Text, out FM.m_stMasterOpt.dDCOMRatio     );

            double.TryParse(tbSpdOffset    .Text, out FM.m_stMasterOpt.dSpdOffset     );

            double.TryParse(tbCalStart     .Text, out FM.m_stMasterOpt.dStartDCOM     );


            //double.TryParse(tbLDCBtmOffset .Text, out FM.m_stMasterOpt.dLDCBtmOffset);



            //
            FM.fn_LoadMastOptn(false); //JUNG/200511

        }
        //---------------------------------------------------------------------------
        private void btIdleSet_Click(object sender, RoutedEventArgs e)
        {
            //
            rbMAN.IsChecked = true;

            cbSkipDoor.IsChecked  = true;
            cbSkipLeak.IsChecked  = true;
            cbSkipFan .IsChecked  = true;
            cbSkipAir .IsChecked  = true;

            cbSkipTool .IsChecked = true;
            cbSkipPlate.IsChecked = true;
            cbSkipStrg .IsChecked = true;
            cbSkipMaga .IsChecked = true;

            cbUseVisn  .IsChecked = false;

            cbSkipWtLvl .IsChecked = true;
            cbSkipWtLeak.IsChecked = true;
            cbSkipAccura.IsChecked = true;


        }
        //---------------------------------------------------------------------------
        private void btLoadCellSet_Click(object sender, RoutedEventArgs e)
        {
            //
            IO.fn_SetTopOffset();
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            fn_SetTimer(false);
        }
        //---------------------------------------------------------------------------
        private void btSafetyBypass_Click(object sender, RoutedEventArgs e)
        {
            //
            Button btnSel = sender as Button;

            string name = btnSel.Content as string;

            switch (name)
            {
                case "Safety bypass OFF":
                    IO.YV[(int)EN_OUTPUT_ID.ySYS_Safety_PLC01] = true;
                    IO.YV[(int)EN_OUTPUT_ID.ySYS_Safety_PLC02] = true;

                    btnSel.Content = "Safety bypass ON";
                    btnSel.Background = Brushes.Red; 

                    break;

                case "Safety bypass ON":
                    IO.YV[(int)EN_OUTPUT_ID.ySYS_Safety_PLC01] = false;
                    IO.YV[(int)EN_OUTPUT_ID.ySYS_Safety_PLC02] = false;
                    
                    btnSel.Content = "Safety bypass OFF";
                    btnSel.Background = Brushes.LightGray;
                    break;
                default:
                    btnSel.Content = "Safety bypass OFF";
                    IO.YV[(int)EN_OUTPUT_ID.ySYS_Safety_PLC01] = false;
                    IO.YV[(int)EN_OUTPUT_ID.ySYS_Safety_PLC02] = false;
                    btnSel.Background = Brushes.LightGray;
                    break;
            }

        }
        //-------------------------------------------------------------------------------------------------
        private void lbTopOffset_Click(object sender, RoutedEventArgs e)
        {
            //
            double.TryParse(tbTopOffset.Text, out FM.m_stMasterOpt.dUtilOffset);

            FM.fn_LoadMastOptn(false);

        }
        //-------------------------------------------------------------------------------------------------
        bool bOn = false;
        private void btMillingTest_Click(object sender, RoutedEventArgs e)
        {
            //
            //X_start
            //X_end
            //Y_start
            //Y_end
            //Y Step(mm)
            //Direction

            if (!bOn)
            {
                IO.DATA_EQ_TO_ACS[20] = 200.0;// X Start Pos
                IO.DATA_EQ_TO_ACS[21] = 190.0;// X End Pos
                IO.DATA_EQ_TO_ACS[22] = 20.0;//  Y Start Pos
                IO.DATA_EQ_TO_ACS[23] = 20.5;//  Y End Pos
                IO.DATA_EQ_TO_ACS[24] = 0.005;// Y Step
                IO.DATA_EQ_TO_ACS[25] = 1;// Direction

                fn_WriteLog($"StartX_{IO.DATA_EQ_TO_ACS[20]}EndX_{IO.DATA_EQ_TO_ACS[21]}StartY_{IO.DATA_EQ_TO_ACS[22]}EndY{IO.DATA_EQ_TO_ACS[23]}");

                IO.fn_DataEqToAcs();

                IO.fn_RunBuffer(13, true);
                bOn = true;
            }
            else
            {
                IO.fn_StopAllBuffer();
                bOn = false;
            }
        }
        //---------------------------------------------------------------------------
        private void btForceTest_Click(object sender, RoutedEventArgs e)
        {
            //TARGET_FORCE
            double.TryParse(tbTarget.Text, out double dValue);
            
            IO.fn_ForceBuffTest(dValue);
        }
        //-------------------------------------------------------------------------------------------------
        private void btWait_Click(object sender, RoutedEventArgs e)
        {
            //
            IO.fn_SetOpenLoopOff();
            
            //MOTR[(int)EN_MOTR_ID.miSPD_Z].
            SEQ_SPIND.fn_ReqMoveMotr(EN_MOTR_ID.miSPD_Z, EN_COMD_ID.Wait1);
        }
        //---------------------------------------------------------------------------
        private void btHomeOffset_Click(object sender, RoutedEventArgs e)
        {
            //
            MOTR.InitHomeOffset();
            
            fn_WriteLog("[MASTER] ACS Home Offset Clear.");

        }
        //---------------------------------------------------------------------------
        private void btACSReboot_Click(object sender, RoutedEventArgs e)
        {
            //IO.fn_ACSReboot(true);
            IO.fn_SetACSReboot();

            fn_WriteLog("[MASTER] ACS Reboot.");
        }
        //---------------------------------------------------------------------------
        private void btAutoCal_Click(object sender, RoutedEventArgs e)
        {

            int nCycle = 0; 
            int.TryParse(lbCalCycle.Text, out nCycle) ;

            if (nCycle > 0 && nCycle < 20)
            {
                SEQ_SPIND._nTotalCalCycle = nCycle;
            }
            else
            {
                fn_UserMsg("Count 숫자를 확인하세요 (Count < 20)");
                return;
            }

            //Auto Calibration 
            MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0419, true, false);

            fn_WriteLog("[MASTER] Auto Calibration.");

        }
        //---------------------------------------------------------------------------
        public void fn_DisplayOffset()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate () 
            {
                tbYIntercept.Text = FM.m_stMasterOpt.dYIntercept.ToString();
                tbYSlope.Text = FM.m_stMasterOpt.dYSlope.ToString();

                tbYInterceptBT.Text = FM.m_stMasterOpt.dYInterceptBT.ToString();
                tbYSlopeBT.Text = FM.m_stMasterOpt.dYSlopeBT.ToString();
            }));
        }
        //---------------------------------------------------------------------------
        private void btToolCal_Click(object sender, RoutedEventArgs e)
        {
            //Auto Tool Calibration 
            MAN.fn_ManProcOn((int)EN_MAN_LIST.MAN_0425, true, false);

            fn_WriteLog("[MASTER] Auto Tool Storage Calibration.");

        }
    }
}