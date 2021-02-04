using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;


namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Option : Page
    {
        //Var
        private CheckBox[] chkWarmMot = new CheckBox[MAX_MOTOR];
        //private CheckBox[] chkWarmClamp;
        //private CheckBox[] chkWarmUtil;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_Option()
        {
            InitializeComponent();

            fn_InitCheckBox();
            
            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;


        }
        //---------------------------------------------------------------------------
        private void fn_InitCheckBox()
        {
            for (int n = 0; n < MAX_MOTOR; n++)
            {
                chkWarmMot[n] = new CheckBox();
            }

            chkWarmMot[0]  = cbWarmMOT_0 ;
            chkWarmMot[1]  = cbWarmMOT_1 ;
            chkWarmMot[2]  = cbWarmMOT_2 ;
            chkWarmMot[3]  = cbWarmMOT_3 ;
            chkWarmMot[4]  = cbWarmMOT_4 ;
            chkWarmMot[5]  = cbWarmMOT_5 ;
            chkWarmMot[6]  = cbWarmMOT_6 ;
            chkWarmMot[7]  = cbWarmMOT_7 ;
            chkWarmMot[8]  = cbWarmMOT_8 ;
            chkWarmMot[9]  = cbWarmMOT_9 ;
            chkWarmMot[10] = cbWarmMOT_10;
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
        //-------------------------------------------------------------------------------------------------

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = fn_IsNumeric(e.Text);
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            for (int n = 0; n < MAX_MOTOR; n++)
            {
                chkWarmMot[n].Content = STRING_MOTOR_ID[n];  //MOTR[n].m_sName + " " + MOTR[n].m_sNameAxis;
            }

            //fn_SetTimer(true);
            fn_DisplayOptionData();


        }
        //-------------------------------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //fn_SetTimer(false);
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Load/Save Option Data
        </summary>
        <param name="bLoad">true: Load, false: Save</param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/28 11:07
        */
        public void fn_DisplayOptionData(bool bLoad = true)
        {
            // Data Read
            if (bLoad)
            {
                // Machine Option
                text_MachineName.Text = FM.m_stSystemOpt.strMachineName;
                text_MachineNo  .Text = FM.m_stSystemOpt.strMachineNo  ;
                text_LogPath    .Text = FM.m_stSystemOpt.strLogPath    ;

                // WarmingUp
                cbWarmingup.IsChecked = FM.m_stSystemOpt.nUseWarming == 1? true : false;

                // WarmingUp Interval
                upInterval.UPValue = Convert.ToString(FM.m_stSystemOpt.nWarmInterval);


                // WarmingUp Motor
                for (int i = 0; i < chkWarmMot.Length; i++)
                {
                    chkWarmMot[i].IsChecked = FM.m_stSystemOpt.bUseMotor[i]? true : false; 
                }

                text_RepeatMOT.Text = Convert.ToString(FM.m_stSystemOpt.nMotrRepeat);

                // WarmingUp Clamp
                cbSpindleclamp  .IsChecked = FM.m_stSystemOpt.bUseClamp[0] ? true : false;
                cbPolishingclamp.IsChecked = FM.m_stSystemOpt.bUseClamp[1] ? true : false;
                cbCleaningclamp .IsChecked = FM.m_stSystemOpt.bUseClamp[2] ? true : false;

                text_RepeatCLAMP.Text = Convert.ToString(FM.m_stSystemOpt.nClampRepeat);

                // WarmingUp Util
                cbPolishing.IsChecked = FM.m_stSystemOpt.bUseUtil[0] ? true : false;
                cbCleaning .IsChecked = FM.m_stSystemOpt.bUseUtil[1] ? true : false;
                
                text_RepeatUTIL.Text = Convert.ToString(FM.m_stSystemOpt.nUtilRepeat);
                tbSplyTime     .Text = Convert.ToString(FM.m_stSystemOpt.nSplyTime  );

                //
                cbOption1    .IsChecked  = FM.m_stSystemOpt.nUseCleanAirBlow  == 1 ? true : false;
                cbOption2    .IsChecked  = FM.m_stSystemOpt.nUseLightOnRun    == 1 ? true : false;
                cbOption3    .IsChecked  = FM.m_stSystemOpt.nUsePolishingCup  == 1 ? true : false;
                cbOption4    .IsChecked  = FM.m_stSystemOpt.nUseAutoSlurry    == 1 ? true : false;
                cbUseSoftLimt.IsChecked  = FM.m_stSystemOpt.nUseSoftLimit     == 1 ? true : false;

                cbSpdFirstDir.IsChecked  = FM.m_stSystemOpt.nUseSpdDirBwd     == 1 ? true : false;
                cbSpdDir_FWD .IsChecked  = FM.m_stSystemOpt.nUseSpdDirOnlyFWD == 1 ? true : false;
                cbSpdDir_BWD .IsChecked  = FM.m_stSystemOpt.nUseSpdDirOnlyBWD == 1 ? true : false;
                cbPoilOneway .IsChecked  = FM.m_stSystemOpt.nUsePoliOneDir    == 1 ? true : false;
                cbSkipAlignP .IsChecked  = FM.m_stSystemOpt.nUseSkipPolAlign  == 1 ? true : false;
                cbSkipAlignE .IsChecked  = FM.m_stSystemOpt.nUseSkipVisError  == 1 ? true : false;
                

                //
                upWaterLvlPol  .UPValue = Convert.ToString(FM.m_stSystemOpt.nWaterLvlPol    );
                upSoftLimit    .UPValue = Convert.ToString(FM.m_stSystemOpt.dSoftLimitOffset);
                upTargetForce  .UPValue = Convert.ToString(FM.m_stSystemOpt.dTargetForce    );
                upSpindleOffCnt.UPValue = Convert.ToString(FM.m_stSystemOpt.nSpindleOffCnt  );

                //
                cbStogDir.SelectedIndex = FM.m_stSystemOpt.nStorDir;

                //PMC
                //tbPMCIp  .Text = FM.m_stSystemOpt.sPMCIp;
                //tbPMCPort.Text = Convert.ToString(FM.m_stSystemOpt.nPMCPort);


                // Operator
                cbOpMotion .IsChecked = FM.m_stSystemOpt.stUserSet[0].bMotion ? true : false;
                cbOpSetting.IsChecked = FM.m_stSystemOpt.stUserSet[0].bSetting? true : false;
                cbOpRecipe .IsChecked = FM.m_stSystemOpt.stUserSet[0].bRecipe ? true : false;
                cbOpLog    .IsChecked = FM.m_stSystemOpt.stUserSet[0].bLog    ? true : false;
                cbOpMaster .IsChecked = FM.m_stSystemOpt.stUserSet[0].bMaster ? true : false;
                cbOpExit   .IsChecked = FM.m_stSystemOpt.stUserSet[0].bExit   ? true : false;

                // Engineer
                cbEnMotion .IsChecked = FM.m_stSystemOpt.stUserSet[1].bMotion ? true : false;
                cbEnSetting.IsChecked = FM.m_stSystemOpt.stUserSet[1].bSetting? true : false;
                cbEnRecipe .IsChecked = FM.m_stSystemOpt.stUserSet[1].bRecipe ? true : false;
                cbEnLog    .IsChecked = FM.m_stSystemOpt.stUserSet[1].bLog    ? true : false;
                cbEnMaster .IsChecked = FM.m_stSystemOpt.stUserSet[1].bMaster ? true : false;
                cbEnExit   .IsChecked = FM.m_stSystemOpt.stUserSet[1].bExit   ? true : false;

                
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // Project Option
                // Storage
                text_STRowCount.Text = Convert.ToString(FM.m_stProjectBase.nStorage_Row  );
                text_STColCount.Text = Convert.ToString(FM.m_stProjectBase.nStorage_Col  );
                text_STRowPitch.Text = Convert.ToString(FM.m_stProjectBase.dStorPitch_Row);
                text_STColPitch.Text = Convert.ToString(FM.m_stProjectBase.dStorPitch_Col);

                //Magazine
                tbMAGARowCnt   .Text = Convert.ToString(FM.m_stProjectBase.nMagazine_Row );
                tbMAGAColCnt   .Text = Convert.ToString(FM.m_stProjectBase.nMagazine_Col );
                tbMAGARowPitch .Text = Convert.ToString(FM.m_stProjectBase.dMagaPitch_Row);
                tbMAGAColPitch .Text = Convert.ToString(FM.m_stProjectBase.dMagaPitch_Col);


                // System Parameter
                // Polishing
                text_POL_X_Offset    .Text = Convert.ToString(FM.m_stProjectBase.dPolishOffset_X);
                text_POL_Y_Offset    .Text = Convert.ToString(FM.m_stProjectBase.dPolishOffset_Y);
                text_POL_Theta_Offset.Text = Convert.ToString(FM.m_stProjectBase.dPolishOffset_TH);
                text_POL_Tilt_Offset .Text = Convert.ToString(FM.m_stProjectBase.dPolishOffset_TI);
                // Cleaning
                text_CLN_R_Offset.Text = Convert.ToString(FM.m_stProjectBase.dCleanOffset_R);

                // System Option
                // Polishing
                text_POL_MillingTime.Text = Convert.ToString(FM.m_stSystemOpt.nPoliMillingTime);
                // Cleaning
                text_CLN_MillingTime.Text = Convert.ToString(FM.m_stSystemOpt.nCleanMillingTime);

                //Mode
                //rbRemote .IsChecked = (FM.m_stSystemOpt.nRunMode == 0) ? true : false;
                //rbLocal  .IsChecked = (FM.m_stSystemOpt.nRunMode == 1) ? true : false;
                //rbOffline.IsChecked = (FM.m_stSystemOpt.nRunMode == 2) ? true : false;

                //Tool Type
                tbToolName1.Text = FM.m_stSystemOpt.sToolType[0];
                tbToolName2.Text = FM.m_stSystemOpt.sToolType[1];

                btToolType01.Background = FM.m_stSystemOpt.brPinColor[0];
                btToolType02.Background = FM.m_stSystemOpt.brPinColor[1];
                btCleanTool .Background = FM.m_stSystemOpt.brPinColor[4];

                //Auto Supply
                tbAutoSupplyIp   .Text = FM.m_stSystemOpt.sSupplyIp; 
                tbAutoSupplyPort .Text = Convert.ToString(FM.m_stSystemOpt.nSupplyPort);
                tbAutoSupplyAdd  .Text = Convert.ToString(FM.m_stSystemOpt.nSupplyAddress);
                tbAutoSupplyID   .Text = FM.m_stSystemOpt.sSupplyEqpId;
                
                tbAutoSupplyIp1  .Text = FM.m_stSystemOpt.sSupplyIp1; 
                tbAutoSupplyPort1.Text = Convert.ToString(FM.m_stSystemOpt.nSupplyPort1);
                tbAutoSupplyAdd1 .Text = Convert.ToString(FM.m_stSystemOpt.nSupplyAddress1);
                tbAutoSupplyID1  .Text = FM.m_stSystemOpt.sSupplyEqpId1;

                //REST API
                tbRestApiUrl     .Text = FM.m_stSystemOpt.sRestApiUrl ;

            }
            else // Data Save
            {
                // Machine Option
                FM.m_stSystemOpt.strMachineName = text_MachineName.Text;
                FM.m_stSystemOpt.strMachineNo   = text_MachineNo  .Text;
                FM.m_stSystemOpt.strLogPath     = text_LogPath    .Text;

                // WarmingUp
                FM.m_stSystemOpt.nUseWarming = cbWarmingup.IsChecked == true ? 1 : 0 ;

                // WarmingUp Interval
                int.TryParse(upInterval.UPValue, out FM.m_stSystemOpt.nWarmInterval);

                // WarmingUp Motor
                for (int i = 0; i < chkWarmMot.Length; i++)
                {
                    FM.m_stSystemOpt.bUseMotor[i] = chkWarmMot[i].IsChecked == true ? true : false;
                }

                //FM.m_stSystemOpt.nMotrRepeat = Convert.ToInt32(text_RepeatMOT.Text);
                int.TryParse(text_RepeatMOT.Text, out FM.m_stSystemOpt.nMotrRepeat);

                // WarmingUp Clamp
                FM.m_stSystemOpt .bUseClamp[0] = cbSpindleclamp  .IsChecked == true ? true : false;
                FM.m_stSystemOpt .bUseClamp[1] = cbPolishingclamp.IsChecked == true ? true : false;
                FM.m_stSystemOpt .bUseClamp[2] = cbCleaningclamp .IsChecked == true ? true : false;

                //FM.m_stSystemOpt.nClampRepeat = Convert.ToInt32(text_RepeatCLAMP.Text);
                int.TryParse(text_RepeatCLAMP.Text, out FM.m_stSystemOpt.nClampRepeat);

                // WarmingUp Util
                FM.m_stSystemOpt.bUseUtil[0] = cbPolishing.IsChecked == true ? true : false;
                FM.m_stSystemOpt.bUseUtil[1] = cbCleaning .IsChecked == true ? true : false;

                //FM.m_stSystemOpt.nUtilRepeat = Convert.ToInt32(text_RepeatUTIL.Text);
                int.TryParse(text_RepeatUTIL.Text, out FM.m_stSystemOpt.nUtilRepeat);
                int.TryParse(tbSplyTime     .Text, out FM.m_stSystemOpt.nSplyTime  );


                //
                int   .TryParse(upWaterLvlPol  .UPValue, out FM.m_stSystemOpt.nWaterLvlPol    );
                double.TryParse(upSoftLimit    .UPValue, out FM.m_stSystemOpt.dSoftLimitOffset);
                double.TryParse(upTargetForce  .UPValue, out FM.m_stSystemOpt.dTargetForce    );
                int   .TryParse(upSpindleOffCnt.UPValue, out FM.m_stSystemOpt.nSpindleOffCnt  );


                FM.m_stSystemOpt.nStorDir = cbStogDir.SelectedIndex;

                //PMC
                //FM.m_stSystemOpt.sPMCIp = tbPMCIp.Text;
                //int.TryParse(tbPMCPort.Text, out FM.m_stSystemOpt.nPMCPort);


                //
                FM.m_stSystemOpt.nUseCleanAirBlow  =  cbOption1    .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUseLightOnRun    =  cbOption2    .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUsePolishingCup  =  cbOption3    .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUseAutoSlurry    =  cbOption4    .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUseSoftLimit     =  cbUseSoftLimt.IsChecked == true ? 1 : 0;

                FM.m_stSystemOpt.nUseSpdDirBwd     =  cbSpdFirstDir.IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUseSpdDirOnlyFWD =  cbSpdDir_FWD .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUseSpdDirOnlyBWD =  cbSpdDir_BWD .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUsePoliOneDir    =  cbPoilOneway .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUseSkipPolAlign  =  cbSkipAlignP .IsChecked == true ? 1 : 0;
                FM.m_stSystemOpt.nUseSkipVisError  =  cbSkipAlignE .IsChecked == true ? 1 : 0;

                // User Level
                // Operator
                FM.m_stSystemOpt.stUserSet[0].bMotion  = cbOpMotion .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[0].bSetting = cbOpSetting.IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[0].bRecipe  = cbOpRecipe .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[0].bLog     = cbOpLog    .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[0].bMaster  = cbOpMaster .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[0].bExit    = cbOpExit   .IsChecked == true ? true : false;

                // Engineer
                FM.m_stSystemOpt.stUserSet[1].bMotion  = cbEnMotion .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[1].bSetting = cbEnSetting.IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[1].bRecipe  = cbEnRecipe .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[1].bLog     = cbEnLog    .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[1].bMaster  = cbEnMaster .IsChecked == true ? true : false;
                FM.m_stSystemOpt.stUserSet[1].bExit    = cbEnExit   .IsChecked == true ? true : false;

                // Project Option
                // Storage
                int   .TryParse(text_STRowCount.Text, out FM.m_stProjectBase.nStorage_Row  ); 
                int   .TryParse(text_STColCount.Text, out FM.m_stProjectBase.nStorage_Col  ); 
                double.TryParse(text_STRowPitch.Text, out FM.m_stProjectBase.dStorPitch_Row); 
                double.TryParse(text_STColPitch.Text, out FM.m_stProjectBase.dStorPitch_Col); 

                //Magazine
                int   .TryParse(tbMAGARowCnt   .Text, out FM.m_stProjectBase.nMagazine_Row ); 
                int   .TryParse(tbMAGAColCnt   .Text, out FM.m_stProjectBase.nMagazine_Col ); 
                double.TryParse(tbMAGARowPitch .Text, out FM.m_stProjectBase.dMagaPitch_Row); 
                double.TryParse(tbMAGAColPitch .Text, out FM.m_stProjectBase.dMagaPitch_Col); 


                // System Parameter
                // Polishing
                double.TryParse(text_POL_X_Offset    .Text, out FM.m_stProjectBase.dPolishOffset_X );
                double.TryParse(text_POL_Y_Offset    .Text, out FM.m_stProjectBase.dPolishOffset_Y );
                double.TryParse(text_POL_Theta_Offset.Text, out FM.m_stProjectBase.dPolishOffset_TH);
                double.TryParse(text_POL_Tilt_Offset .Text, out FM.m_stProjectBase.dPolishOffset_TI);

                // Cleaning
                double.TryParse(text_CLN_R_Offset    .Text, out FM.m_stProjectBase.dCleanOffset_R  );

                // System Option
                // Polishing
                int.TryParse(text_POL_MillingTime.Text, out FM.m_stSystemOpt.nPoliMillingTime);

                // Cleaning
                int.TryParse(text_CLN_MillingTime.Text, out FM.m_stSystemOpt.nCleanMillingTime);

                //Auto Supply
                bool bReqDiscon = false;
                if (tbAutoSupplyIp.Text != FM.m_stSystemOpt.sSupplyIp) bReqDiscon = true;
                FM.m_stSystemOpt.sSupplyIp = tbAutoSupplyIp.Text;
                int.TryParse(tbAutoSupplyPort.Text, out FM.m_stSystemOpt.nSupplyPort);
                int.TryParse(tbAutoSupplyAdd.Text , out FM.m_stSystemOpt.nSupplyAddress);
                FM.m_stSystemOpt.sSupplyEqpId = tbAutoSupplyID.Text;

                if (tbAutoSupplyIp1.Text != FM.m_stSystemOpt.sSupplyIp1) bReqDiscon = true;
                FM.m_stSystemOpt.sSupplyIp1 = tbAutoSupplyIp1.Text;
                int.TryParse(tbAutoSupplyPort1.Text, out FM.m_stSystemOpt.nSupplyPort1);
                int.TryParse(tbAutoSupplyAdd1.Text, out FM.m_stSystemOpt.nSupplyAddress1);
                FM.m_stSystemOpt.sSupplyEqpId1 = tbAutoSupplyID1.Text;

                //REST API
                FM.m_stSystemOpt.sRestApiUrl   = tbRestApiUrl.Text; 

                //
                //if      (rbRemote .IsChecked == true) FM.m_stSystemOpt.nRunMode = 0;
                //else if (rbLocal  .IsChecked == true) FM.m_stSystemOpt.nRunMode = 1;
                //else if (rbOffline.IsChecked == true) FM.m_stSystemOpt.nRunMode = 2;


                FM.m_stSystemOpt.sToolType[0] = tbToolName1.Text ;
                FM.m_stSystemOpt.sToolType[1] = tbToolName2.Text ;
                
                //
                FM.m_stSystemOpt.brPinColor[0] = (SolidColorBrush)btToolType01.Background;
                FM.m_stSystemOpt.brPinColor[1] = (SolidColorBrush)btToolType02.Background;

                FM.m_stSystemOpt.brPinColor[4] = (SolidColorBrush)btCleanTool .Background; //Clean Tool

                for (int s = 0; s < 2; s++)
                {
                    DM.STOR[s].SetColor(FM.m_stSystemOpt.brPinColor[0].Color, FM.m_stSystemOpt.brPinColor[1].Color, FM.m_stSystemOpt.brPinColor[4].Color); //JUNG/200625
                }

                // Save File Manager
                FM.fn_LoadProject(false);
                FM.fn_LoadSysOptn(false);

                //
                REST.fn_SetURL(FM.m_stSystemOpt.sRestApiUrl, true);

                //
                SUPPLY[SPLY_SLURRY].fn_SetAddress(FM.m_stSystemOpt.sSupplyIp , FM.m_stSystemOpt.nSupplyPort , FM.m_stSystemOpt.nSupplyAddress, FM.m_stSystemOpt.sSupplyEqpId );
                SUPPLY[SPLY_SOAP  ].fn_SetAddress(FM.m_stSystemOpt.sSupplyIp1, FM.m_stSystemOpt.nSupplyPort1, FM.m_stSystemOpt.nSupplyAddress, FM.m_stSystemOpt.sSupplyEqpId1);
                if (bReqDiscon)
                {
                    SUPPLY[SPLY_SLURRY].fn_Disconnect();
                    SUPPLY[SPLY_SOAP  ].fn_Disconnect();
                }

            }
        }
        //---------------------------------------------------------------------------
        private void btToolType01_Click(object sender, RoutedEventArgs e)
        {
            btToolType01.Background = fn_ColorPick();  //FM.m_stSystemOpt.brPinColor[0];
            
        }
        //---------------------------------------------------------------------------
        private void btToolType02_Click(object sender, RoutedEventArgs e)
        {
            btToolType02.Background = fn_ColorPick();  //FM.m_stSystemOpt.brPinColor[1];
        }
        //---------------------------------------------------------------------------
        private void btCleanTool_Click(object sender, RoutedEventArgs e)
        {
            btCleanTool.Background = fn_ColorPick();  //FM.m_stSystemOpt.brPinColor[4];
        }
        //---------------------------------------------------------------------------
        private void btDefault_Click(object sender, RoutedEventArgs e)
        {
            //
            btToolType01.Background = Brushes.LightSteelBlue;
            btToolType02.Background = Brushes.CadetBlue;

            btCleanTool.Background  = Brushes.CornflowerBlue;


        }
    }
}
