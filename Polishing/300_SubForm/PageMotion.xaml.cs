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
using static WaferPolishingSystem.FormMain;
using UserInterface;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserFunction;
using System.Data;
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.BaseUnit.IOMap;
using static WaferPolishingSystem.BaseUnit.ActuatorId;

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageMotion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageMotion : Page
    {
        //Var
        private UserButton[] ubbtn;
        private UserButton   ubPreBtn      = new UserButton();
        private DataTable    m_DataTable   = new DataTable ();
        private Label[]      lbMotr        = new Label[10];
        private int          m_nSelMotorNo = -1;
        private int          m_nSelPart    = -1;
        private int          m_nSelIndex   = 0 ;

        private bool         m_bDrngDown;
        private bool         Key_LeftCtrl;
        private Button       DownBtn  = new Button();

        EN_SEQ_ID enSelPart;


        private BitmapImage  imgLeft  = new BitmapImage(new Uri("pack://application:,,,/030_Image/Left.ico"  ));
        private BitmapImage  imgRight = new BitmapImage(new Uri("pack://application:,,,/030_Image/Right.ico" ));
        private BitmapImage  imgUp    = new BitmapImage(new Uri("pack://application:,,,/030_Image/Up.ico"    ));
        private BitmapImage  imgDown  = new BitmapImage(new Uri("pack://application:,,,/030_Image/Down.ico"  ));
        private BitmapImage  imgTiltP = new BitmapImage(new Uri("pack://application:,,,/030_Image/tilt_P.png"));
        private BitmapImage  imgTiltN = new BitmapImage(new Uri("pack://application:,,,/030_Image/tilt_N.png"));

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public PageMotion()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background         = UserConst.G_COLOR_PAGEBACK;

#if !DEBUG
            this.GridSub.Background = UserConst.G_COLOR_SUBMENU ;
#endif

            //Motor Setting Button 
            int nMotor = (int)EN_MOTR_ID.EndOfId;
            ubbtn = new UserButton[nMotor];
            for (int i = 0; i < nMotor; i++)
            {
                ubbtn[i] = new UserButton();
                ubbtn[i].Tag = i;
            }

            ubbtn[0]  = ubMotr01;
            ubbtn[1]  = ubMotr02;
            ubbtn[2]  = ubMotr03;
            ubbtn[3]  = ubMotr04;
            ubbtn[4]  = ubMotr05;
            ubbtn[5]  = ubMotr06;
            ubbtn[6]  = ubMotr07;
            ubbtn[7]  = ubMotr08;
            ubbtn[8]  = ubMotr09;
            ubbtn[9]  = ubMotr10;
            ubbtn[10] = ubMotr11;

            for (int i = 0; i < nMotor; i++)
            {
                ubbtn[i].Tag = i;
            }

            //Label Init
            for (int n1 = 0; n1 <= lbMotr.GetUpperBound(0); n1++)
            {
                lbMotr[n1] = new Label();
            }

            //
            m_nSelIndex = 0; 

            //
            m_bDrngDown  = false;
            Key_LeftCtrl = false;

            //
            enSelPart = EN_SEQ_ID.None;
        }
        //---------------------------------------------------------------------------
        private void lbMotor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (m_nSelMotorNo < 0         ) return;
            if (m_nSelMotorNo >= MAX_MOTOR) return;


            Label selabel = sender as Label;
            int nTag = Convert.ToInt32(selabel.Tag);

            if(nTag == 1) //Servo On
            {
                bool bOn = MOTR[m_nSelMotorNo].GetServo();
                if (!UserFunction.fn_UserMsg(string.Format("Servo {0}?", bOn ? "OFF":"ON"), EN_MSG_TYPE.Check)) return ;

                SEQ.fn_Reset();

                MOTR.SetServo((EN_MOTR_ID)m_nSelMotorNo, bOn? false : true);
            }
            else if(nTag == 2) //Alarm
            {
                if (!UserFunction.fn_UserMsg("Motor Alarm Reset?", EN_MSG_TYPE.Check)) return;
                
                SEQ.fn_Reset();
                
                //
                int nResetNo = MOTR.ManNoAlarm((EN_MOTR_ID)m_nSelMotorNo);
                MAN.fn_ManProcOn(nResetNo, true, false);
            }
            else if(nTag == 3) //Home
            {
                int nHomeNo = MOTR.ManNoHome((EN_MOTR_ID)m_nSelMotorNo);

                if (!UserFunction.fn_UserMsg($"{STRING_MOTOR_ID[m_nSelMotorNo]} Motor Home?", EN_MSG_TYPE.Check)) return;

                SEQ.fn_Reset();

                if(nHomeNo == 32) //Z-Axis
                {
                    if(SEQ_SPIND.fn_IsExistTool() && !FM.fn_IsLvlMaster())
                    {
                        UserFunction.fn_UserMsg("Please remove tool");
                        return ;
                    }
                }
                MAN.fn_ManProcOn(nHomeNo, true, false);
            }
            else if (nTag == 9) //Part Home
            {
                if (m_nSelPart < 0) return;
                
                if (!UserFunction.fn_UserMsg("Part Home?", EN_MSG_TYPE.Check)) return;

                SEQ.fn_Reset();

                //Part Home
                int nHomeNo = m_nSelPart + (int)EN_MAN_LIST.MAN_0002; 
                MAN.fn_ManProcOn(nHomeNo, true, false);
            }
        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            //Manual Jog
            if (m_bDrngDown)
            {
                DownBtn.PerformClick();
            }

            if(m_nSelMotorNo>=0)
            { 
                //Motor Status Display
                lbMotr[0].Content    = string.Format($"{MOTR[m_nSelMotorNo].GetCmdPos():F4}") ;         //(0) "Command";
                lbMotr[1].Background = MOTR[m_nSelMotorNo].GetServo  () ? Brushes.Lime : Brushes.Gray;  //(1) "Servo";
                lbMotr[2].Background = MOTR[m_nSelMotorNo].GetAlarm  () ? Brushes.Red  : Brushes.Gray;  //(2) "Alarm";
                lbMotr[3].Background = MOTR[m_nSelMotorNo].GetHomeEnd() ? Brushes.Lime : Brushes.Gray;  //(3) "Home End";
                if(!MOTR[m_nSelMotorNo].GetHomeEnd())
                {
                    if(MOTR[m_nSelMotorNo].GetHomeEndDone())
                    {
                        lbMotr[3].Background = Brushes.LightYellow; //설비 Data off시....test
                    }
                }
                lbMotr[4].Background = Brushes.LightGray;     //
                lbMotr[5].Content    = string.Format($"{MOTR[m_nSelMotorNo].GetEncPos():F4}");            //(5) "Encoder";
                lbMotr[6].Background = (MOTR[m_nSelMotorNo].GetCCW() || MOTR[m_nSelMotorNo].GetSLL()) ? Brushes.Lime : Brushes.Gray; //(6) "Limit-";
                lbMotr[7].Background = IO.DATA_ACS_TO_EQ[(int)EN_ACS_TO_EQ.ATE_10_SPD_X_Home + m_nSelMotorNo] == 1 ? Brushes.Lime : Brushes.Gray; //(4) HOME Sensor
                lbMotr[8].Background = (MOTR[m_nSelMotorNo].GetCW () || MOTR[m_nSelMotorNo].GetSRL()) ? Brushes.Lime : Brushes.Gray; //(7) "Limit+";
                lbMotr[9].Background = Brushes.Gray;     //
            }

            if(m_nSelPart == (int)EN_SEQ_ID.SPINDLE)
            {
                lbToolExist .Background = IO.XV[(int)EN_INPUT_ID.xSPD_ToolExist    ] ? Brushes.Lime : Brushes.LightGray;
                lbPlateExist.Background = IO.XV[(int)EN_INPUT_ID.xSPD_PlateExistChk] ? Brushes.Lime : Brushes.LightGray;
            }

            //Display Cylinder State
            switch (enSelPart)
            {
                case EN_SEQ_ID.SPINDLE:
                    //gbCyl01.Header = "Plate Clamp CYL'";
                    //gbCyl02.Header = "IR Shutter CYL'";
                    //gbCyl05.Header = "Lens Cover CYL'";
                    //gbCyl06.Header = "TOOL CLAMP";
                    
                    btCyl01_B.Background = ACTR[(int)EN_ACTR_LIST.aSpdl_PlateClamp].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl01_F.Background = ACTR[(int)EN_ACTR_LIST.aSpdl_PlateClamp].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    //
                    btCyl02_B.Background = ACTR[(int)EN_ACTR_LIST.aspdl_IR        ].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl02_F.Background = ACTR[(int)EN_ACTR_LIST.aspdl_IR        ].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    //                                                            
                    btCyl05_B.Background = ACTR[(int)EN_ACTR_LIST.aSpdl_LensCovr  ].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl05_F.Background = ACTR[(int)EN_ACTR_LIST.aSpdl_LensCovr  ].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    //
                    btCyl06_B.Background =  IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_ToolClamp] ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl06_F.Background = !IO.YV[(int)EN_OUTPUT_ID.ySPD_E3000_ToolClamp] ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    break;

                case EN_SEQ_ID.POLISH:

                    //gbCyl01.Header = "Plate Clamp CYL'";
                    btCyl01_B.Background = ACTR[(int)EN_ACTR_LIST.aPoli_Clamp].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl01_F.Background = ACTR[(int)EN_ACTR_LIST.aPoli_Clamp].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    break;

                case EN_SEQ_ID.CLEAN:
                    //gbCyl01.Header = "Plate Clamp CYL'";
                    btCyl01_B.Background = ACTR[(int)EN_ACTR_LIST.aClen_Clamp].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl01_F.Background = ACTR[(int)EN_ACTR_LIST.aClen_Clamp].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    break;

                case EN_SEQ_ID.STORAGE:
                    //gbCyl01.Header = "Storage Lock1 CYL'";
                    //gbCyl02.Header = "Storage Lock2 CYL'";
                    btCyl01_B.Background = ACTR[(int)EN_ACTR_LIST.aStrg_LockBtm].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl01_F.Background = ACTR[(int)EN_ACTR_LIST.aStrg_LockBtm].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    btCyl02_B.Background = ACTR[(int)EN_ACTR_LIST.aStrg_LockTop].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl02_F.Background = ACTR[(int)EN_ACTR_LIST.aStrg_LockTop].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    break;

                case EN_SEQ_ID.TRANSFER:

                    //gbCyl01.Header = "Transfer TOP CYL'";           //MAN_0490, //Transfer - Top Load Fwd/Bwd
                    //gbCyl02.Header = "Transfer Top Turn CYL'";      //MAN_0491, //Transfer - Top Load Turn(0, 180)
                    //gbCyl03.Header = "Transfer BTM CYL'";           //MAN_0492, //Transfer - Bottom Load Fwd/Bwd
                    //gbCyl04.Header = "Load Port Up/Down CYL'";      //MAN_0493, //Transfer - Load Port Up/Down
                    //gbCyl05.Header = "Magazine Move L/R CYL'";      //MAN_0494, //Transfer - Magazine Move Left/Right
                    //gbCyl06.Header = "Load Cover CYL'";             //MAN_0495, //Transfer - Load Protect Cover 
                    btCyl01_B.Background = ACTR[(int)EN_ACTR_LIST.aTran_TopLoadFB  ].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl01_F.Background = ACTR[(int)EN_ACTR_LIST.aTran_TopLoadFB  ].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    btCyl02_B.Background = ACTR[(int)EN_ACTR_LIST.aTran_TopLoadTurn].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl02_F.Background = ACTR[(int)EN_ACTR_LIST.aTran_TopLoadTurn].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    btCyl03_B.Background = ACTR[(int)EN_ACTR_LIST.aTran_BtmLoadFB  ].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl03_F.Background = ACTR[(int)EN_ACTR_LIST.aTran_BtmLoadFB  ].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    btCyl04_B.Background = ACTR[(int)EN_ACTR_LIST.aTran_LoadPortUD ].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl04_F.Background = ACTR[(int)EN_ACTR_LIST.aTran_LoadPortUD ].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    btCyl05_B.Background = ACTR[(int)EN_ACTR_LIST.aTran_MagaMoveLR ].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl05_F.Background = ACTR[(int)EN_ACTR_LIST.aTran_MagaMoveLR ].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    btCyl06_B.Background = ACTR[(int)EN_ACTR_LIST.aTran_LoadCover  ].Complete(ccBwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;
                    btCyl06_F.Background = ACTR[(int)EN_ACTR_LIST.aTran_LoadCover  ].Complete(ccFwd) ? Brushes.Lime : G_COLOR_BTNNORMAL;

                    break;

                default:
                    break;
            }

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

            //
            fn_DisplayMotorPart();

            //
            ubMotr01_Click(m_nSelMotorNo < 0 ? ubbtn[0] : ubbtn[m_nSelMotorNo], null);

            //
            fn_MotorGridSet(ref gdMotorState);

            //
            btSetSoftLimit.Visibility = FM.fn_GetLevel() == (int)EN_USER_LEVEL.lvMaster  ? Visibility.Visible : Visibility.Hidden;
            btSetCurrPos  .Visibility = FM.fn_GetLevel() >  (int)EN_USER_LEVEL.lvOperator? Visibility.Visible : Visibility.Hidden;


        }
        //---------------------------------------------------------------------------
        private void fn_DisplayMotorPos(int nMotor)
        {

            //lbSelMotor.Content = MOTR[nMotor].m_sName + "[ " + MOTR[nMotor].m_sNameAxis + " ]";

            //Motor Position Value Display
            try
            {
                m_DataTable.Clear();
                m_DataTable.Columns.Clear();
                
                m_DataTable.Columns.Add("NAME"    );
                m_DataTable.Columns.Add("POSITION");
                m_DataTable.Columns.Add("UNIT"    );
                m_DataTable.Columns.Add("MOVE"    );
                m_DataTable.Columns.Add("JOGP"    );
                m_DataTable.Columns.Add("JOGN"    );
                m_DataTable.Columns.Add("POSNID"  );
                m_DataTable.Columns.Add("POSNDATA");
                m_DataTable.Columns.Add("MANNO"   );
                m_DataTable.Columns.Add("ENABLE"  );

                if (nMotor < 0 || nMotor >= MOTR._iNumOfMotr) return;

                //
                int    nPosnId   = -1;
                int    nMotorId  = -1;
                int    nManNo    = 0 ;
                double dPosnData = 0.0;
                string sPosnName = string.Empty, sUnit = string.Empty, sPosnVal = string.Empty;
                string sManNo    = string.Empty;
                string sEnable   = string.Empty;


                int nPart = MOTR.fn_GetPart(nMotor);
                int nCnt  = MOTR.Dat[nPart].m_iItemCnt;

                //MAX_SEQ_PART
                for (int i = 0; i < nCnt; i++)
                {
                    if (i >= UserConst.MAX_ITEM) continue;

                    nPosnId = MOTR.Dat[nPart].Set[i].m_iPosnId;
                    if (nPosnId < 0 || nPosnId >= UserConst.MAX_POSN) continue;

                    nMotorId = MOTR.Dat[nPart].Set[i].m_iMotor;
                    if (nMotorId < 0 || nMotorId >= UserConst.MAX_POSN) continue;
                    if (nMotorId != nMotor) continue; 

                    if (MOTR.Dat[nPart].Set[i].m_bHomeOffset)
                    {
                        dPosnData = MOTR[nMotorId].MP.dPosn[nPosnId] + MOTR[nMotorId].m_dHomeOff;
                    }
                    else
                    {
                        dPosnData = MOTR[nMotorId].MP.dPosn[nPosnId];
                    }

                    MOTR.Dat[nPart].Set[i]._Val = dPosnData;

                    sPosnName = MOTR.Dat[nPart].Set[i].m_sName;
                    sPosnVal  = MOTR.Dat[nPart].Set[i]._gVal  ;
                    sUnit     = MOTR.Dat[nPart].Set[i].m_sUnit;
                    nManNo    = MOTR.Dat[nPart].Set[i].m_iManNo;
                    sManNo    = string.Format($"GO[M{nManNo:D4}]");
                    sEnable   = MOTR.Dat[nPart].Set[i].m_iPosnKind == (int)EN_POS_KIND.VIEW ? "False" : "True";

                    m_DataTable.Rows.Add(sPosnName ,// MOTR.Dat[nPart].Set[i].m_sName, 
                                         sPosnVal  ,// MOTR.Dat[nPart].Set[i]._gVal  , 
                                         sUnit     ,// MOTR.Dat[nPart].Set[i].m_sUnit, 
                                         sManNo ,
                                         "+",
                                         "-", 
                                         nPosnId, 
                                         dPosnData,
                                         nManNo, 
                                         sEnable);

                    //
                    if(MOTR.Dat[nPart].Set[i].m_iPosnKind == (int)EN_POS_KIND.VIEW)
                    {
                        //set row disable

                    }


                }

                m_nSelPart = nPart;

                //
                dg_MotrPosData.ItemsSource = m_DataTable.DefaultView;

                //Jog Icon

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            fn_SetTimer(false);
        }
        //---------------------------------------------------------------------------
        public void fn_DisplayMotorPart()
        {
            //Button Name Display
            string sMotrName = string.Empty; 
            for (int i=0; i<MOTR._iNumOfMotr;i++) {

                sMotrName = string.Format($"#{i+1:D2}-{MOTR[i].m_sNameAxis}\r\n  {MOTR[i].m_sName}");
                
                if (ubbtn[i] != null        ) ubbtn[i].Content   = sMotrName;
                if (MOTR[i]._iNoUseMotr == 1) ubbtn[i].IsEnabled = false; 
		    }

        }
        //---------------------------------------------------------------------------
        private void ubMotr01_Click(object sender, RoutedEventArgs e)
        {
            UserButton selbtn = sender as UserButton;
            int nTag = Convert.ToInt32(selbtn.Tag);

            if (nTag < 0 || nTag >= MOTR._iNumOfMotr) return; 

            //Motor Position Value Display
            fn_DisplayMotorPos(nTag);
            fn_DisplayPartInfo(nTag);

            //
            lbSelMotor.Content = selbtn.Content;

            if(ubPreBtn != null) ubPreBtn.Background = UserConst.G_COLOR_BTNNORMAL ;
            selbtn.Background = UserConst.G_COLOR_BTNCLICKED;

            ubPreBtn = selbtn;

            //
            m_nSelMotorNo = nTag;

            //
            btMoveP.IsEnabled  = MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nSelMotorNo) ? false : true; 
            btMoveN .IsEnabled = MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nSelMotorNo) ? false : true;
            btMoveP1.IsEnabled = MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nSelMotorNo) ? false : true;
            btMoveN1.IsEnabled = MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nSelMotorNo) ? false : true;


            /*
                        //
                        switch ((EN_MOTR_ID)nTag)
                        {
                            case EN_MOTR_ID.miSPD_X:
                                imgJogP.Source = imgLeft ;
                                imgJogN.Source = imgRight;
                                break;

                            case EN_MOTR_ID.miSPD_Z:
                            case EN_MOTR_ID.miSPD_Z1:
                            case EN_MOTR_ID.miPOL_Z:
                                imgJogP.Source = imgDown;
                                imgJogN.Source = imgUp  ;
                                break;
                            case EN_MOTR_ID.miPOL_Y:
                            case EN_MOTR_ID.miCLN_Y:
                            case EN_MOTR_ID.miSTR_Y:
                            case EN_MOTR_ID.miTRF_Y:
                                imgJogP.Source = imgDown;
                                imgJogN.Source = imgUp;

                                break;

                            case EN_MOTR_ID.miPOL_TH:
                                break;

                            case EN_MOTR_ID.miPOL_TI:
                                imgJogP.Source = imgTiltP;
                                imgJogN.Source = imgTiltN;


                                break;

                            case EN_MOTR_ID.miCLN_R:
                            case EN_MOTR_ID.miTRF_T1:
                            case EN_MOTR_ID.miTRF_T2:
                                break;

                            default:
                                break;
                        }
            */
        }
        //---------------------------------------------------------------------------
        private void fn_DisplayPartInfo(int nMotor)
        {
            EN_SEQ_ID nPart = (EN_SEQ_ID)MOTR.fn_GetPart(nMotor);

            fn_ResetPart(nPart);

            enSelPart = nPart; //JUNG/200807

            //Button Setting
            switch (nPart)
            {
                case EN_SEQ_ID.SPINDLE:
                    btOneCyle01.Tag = EN_MAN_LIST.MAN_0400; btOneCyle01.Content = string.Format($"Tool pick Cyle [M{(int)btOneCyle01.Tag:D4}]");          
                    btOneCyle02.Tag = EN_MAN_LIST.MAN_0401; btOneCyle02.Content = string.Format($"Tool place Cyle [M{(int)btOneCyle02.Tag:D4}]");         
                                                                                                                                                          
                    btOneCyle03.Tag = EN_MAN_LIST.MAN_0405; btOneCyle03.Content = string.Format($"Plate Pick Cycle [M{(int)btOneCyle03.Tag:D4}~5]");
                    btOneCyle04.Tag = EN_MAN_LIST.MAN_0402; btOneCyle04.Content = string.Format($"Plate Place Cycle [M{(int)btOneCyle04.Tag:D4}~7]");

                    //btOneCyle05.Tag = EN_MAN_LIST.MAN_0408; btOneCyle05.Content = string.Format($"Tool Exist Cycle [M{(int)btOneCyle05.Tag:D4}]");        
                    btOneCyle06.Tag = EN_MAN_LIST.MAN_0409; btOneCyle06.Content = string.Format($"Force Check Cycle [M{(int)btOneCyle06.Tag:D4}]");
                    //btOneCyle07.Tag = EN_MAN_LIST.MAN_0410; btOneCyle07.Content = string.Format($"Force Check Cycle (POL) [M{(int)btOneCyle07.Tag:D4}]");

                    cbForceWhere.Visibility = Visibility.Visible; //7
                    cbForceDCOM .Visibility = Visibility.Visible; //7
                    cbForceWhere.SelectedIndex = 0;
                    cbForceDCOM .SelectedIndex = 0;


                    btOneCyle15.Tag = EN_MAN_LIST.MAN_0413; btOneCyle15.Content = string.Format($"Vision Test Cycle (POL) [M{(int)btOneCyle15.Tag:D4}]");

                    btOneCyle05.Tag = EN_MAN_LIST.MAN_0398; btOneCyle05.Content = string.Format($"X AXIS STEP MOVE + [M{(int)btOneCyle05.Tag:D4}]");  //JUNG/200902
                    btOneCyle10.Tag = EN_MAN_LIST.MAN_0399; btOneCyle10.Content = string.Format($" AXIS STEP MOVE - [M{(int)btOneCyle10.Tag:D4}]");

                    cbPlatePlce.Visibility = Visibility.Visible; //8
                    cbPlatePick.Visibility = Visibility.Visible; //9
                    cbPlatePlce.SelectedIndex = 0;
                    cbPlatePick.SelectedIndex = 0;

                    cbCupPlce.Visibility = Visibility.Visible; //
                    cbCupPick.Visibility = Visibility.Visible; //
                    cbCupPlce.SelectedIndex = 0;
                    cbCupPick.SelectedIndex = 0;

                    btOneCyle11.Tag = EN_MAN_LIST.MAN_0411; btOneCyle11.Content = string.Format($"Util Check Cycle [M{(int)btOneCyle11.Tag:D4}]");      
                    btOneCyle12.Tag = EN_MAN_LIST.MAN_0412; btOneCyle12.Content = string.Format($"Vision Cycle(Pre) [M{(int)btOneCyle12.Tag:D4}]");

                    btOneCyle13.Tag = EN_MAN_LIST.MAN_0414; btOneCyle13.Content = string.Format($"Cup pick Cycle [M{(int)btOneCyle13.Tag:D4}]");
                    btOneCyle14.Tag = EN_MAN_LIST.MAN_0416; btOneCyle14.Content = string.Format($"Cup place Cycle [M{(int)btOneCyle14.Tag:D4}]");

                    gbCyl01.Visibility = Visibility.Visible;
                    gbCyl02.Visibility = Visibility.Visible;

                    gbCyl05.Visibility = Visibility.Visible;
                    gbCyl06.Visibility = Visibility.Visible;

                    gbCyl01.Header = "Plate Clamp CYL'";
                    gbCyl02.Header = "IR Shutter CYL'";
                    gbCyl05.Header = "Lens Cover CYL'";
                    gbCyl06.Header = "TOOL CLAMP";

                    btCyl01_B.Tag = EN_MAN_LIST.MAN_0471; btCyl01_B.Content = "RELEASE"; 
                    btCyl01_F.Tag = EN_MAN_LIST.MAN_0471; btCyl01_F.Content = "HOLD";
                    
                    btCyl02_B.Tag = EN_MAN_LIST.MAN_0472; btCyl02_B.Content = "CLOSE";
                    btCyl02_F.Tag = EN_MAN_LIST.MAN_0472; btCyl02_F.Content = "OPEN" ;

                    btCyl05_B.Tag = EN_MAN_LIST.MAN_0473; btCyl05_B.Content = "CLOSE";
                    btCyl05_F.Tag = EN_MAN_LIST.MAN_0473; btCyl05_F.Content = "OPEN" ;

                    btCyl06_B.Tag = EN_MAN_LIST.MAN_0470; btCyl06_B.Content = "RELEASE";
                    btCyl06_F.Tag = EN_MAN_LIST.MAN_0470; btCyl06_F.Content = "HOLD";

                    lbToolExist .Visibility = Visibility.Visible; 
                    lbPlateExist.Visibility = Visibility.Visible;

                    break;

                case EN_SEQ_ID.POLISH:
                    btOneCyle01.Tag = EN_MAN_LIST.MAN_0420; btOneCyle01.Content = string.Format($"Utility Check Cycle [M{(int)btOneCyle01.Tag:D4}]");    
                    btOneCyle02.Tag = EN_MAN_LIST.MAN_0421; btOneCyle02.Content = string.Format($"Drain Cycle [M{(int)btOneCyle02.Tag:D4}]");
                    
                    gbCyl01.Visibility = Visibility.Visible;

                    gbCyl01.Header = "Plate Clamp CYL'";

                    btCyl01_B.Tag = EN_MAN_LIST.MAN_0475; btCyl01_B.Content = "RELEASE";
                    btCyl01_F.Tag = EN_MAN_LIST.MAN_0475; btCyl01_F.Content = "HOLD";

                    break;

                case EN_SEQ_ID.CLEAN:
                    btOneCyle01.Tag = EN_MAN_LIST.MAN_0430; btOneCyle01.Content = string.Format($"Cleaning Cycle [M{(int)btOneCyle01.Tag:D4}]");
                    btOneCyle02.Tag = EN_MAN_LIST.MAN_0431; btOneCyle02.Content = string.Format($"Drain Cycle [M{(int)btOneCyle02.Tag:D4}]");

                    gbCyl01.Visibility = Visibility.Visible;

                    gbCyl01.Header = "Plate Clamp CYL'";

                    btCyl01_B.Tag = EN_MAN_LIST.MAN_0480; btCyl01_B.Content = "RELEASE";
                    btCyl01_F.Tag = EN_MAN_LIST.MAN_0480; btCyl01_F.Content = "HOLD";

                    break;

                case EN_SEQ_ID.STORAGE:
                    btOneCyle01.Tag = EN_MAN_LIST.MAN_0440; btOneCyle01.Content = string.Format($"Step One Cyle [M{(int)btOneCyle01.Tag:D4}]");              
                    btOneCyle02.Tag = EN_MAN_LIST.MAN_0441; btOneCyle02.Content = string.Format($"Storage Align Cycle [M{(int)btOneCyle02.Tag:D4}]");

                    btOneCyle03.Tag = EN_MAN_LIST.MAN_0442; btOneCyle03.Content = string.Format($"Y AXIS STEP MOVE + [M{(int)btOneCyle03.Tag:D4}]");
                    btOneCyle04.Tag = EN_MAN_LIST.MAN_0443; btOneCyle04.Content = string.Format($"Y AXIS STEP MOVE - [M{(int)btOneCyle04.Tag:D4}]");


                    gbCyl01.Visibility = Visibility.Visible;
                    gbCyl02.Visibility = Visibility.Visible;

                    gbCyl01.Header = "Storage Lock1 CYL'";
                    gbCyl02.Header = "Storage Lock2 CYL'";

                    btCyl01_B.Tag = EN_MAN_LIST.MAN_0485; btCyl01_B.Content = "UNLOCK"; 
                    btCyl01_F.Tag = EN_MAN_LIST.MAN_0485; btCyl01_F.Content = "LOCK"  ;

                    btCyl02_B.Tag = EN_MAN_LIST.MAN_0486; btCyl02_B.Content = "UNLOCK" ;
                    btCyl02_F.Tag = EN_MAN_LIST.MAN_0486; btCyl02_F.Content = "LOCK";

                    break;

                case EN_SEQ_ID.TRANSFER:
                    btOneCyle01.Tag = EN_MAN_LIST.MAN_0450; btOneCyle01.Content = string.Format($"Load One Cyle [M{(int)btOneCyle01.Tag:D4}]");      
                    btOneCyle02.Tag = EN_MAN_LIST.MAN_0451; btOneCyle02.Content = string.Format($"Unload One Cycle [M{(int)btOneCyle02.Tag:D4}]");
                    btOneCyle03.Tag = EN_MAN_LIST.MAN_0452; btOneCyle03.Content = string.Format($"Pick One Cycle [M{(int)btOneCyle03.Tag:D4}]");      
                    btOneCyle04.Tag = EN_MAN_LIST.MAN_0453; btOneCyle04.Content = string.Format($"Place One Cycle [M{(int)btOneCyle04.Tag:D4}]");

                    btOneCyle06.Tag = EN_MAN_LIST.MAN_0454; btOneCyle06.Content = string.Format($"Z AXIS STEP MOVE + [M{(int)btOneCyle06.Tag:D4}]");
                    btOneCyle07.Tag = EN_MAN_LIST.MAN_0455; btOneCyle07.Content = string.Format($"Z AXIS STEP MOVE - [M{(int)btOneCyle07.Tag:D4}]");

                    cbPickMagaNo.Visibility = Visibility.Visible; //8
                    cbPlceMagaNo.Visibility = Visibility.Visible; //9
                    cbPickMagaNo.SelectedIndex = 0;
                    cbPlceMagaNo.SelectedIndex = 0;

                    gbCyl01.Visibility = Visibility.Visible;
                    gbCyl02.Visibility = Visibility.Visible;
                    gbCyl03.Visibility = Visibility.Visible;
                    gbCyl04.Visibility = Visibility.Visible;
                    gbCyl05.Visibility = Visibility.Visible;
                    gbCyl06.Visibility = Visibility.Visible;

                    gbCyl01.Header = "Transfer TOP CYL'";           //MAN_0490, //Transfer - Top Load Fwd/Bwd
                    gbCyl02.Header = "Transfer Top Turn CYL'";      //MAN_0491, //Transfer - Top Load Turn(0, 180)
                    gbCyl03.Header = "Transfer BTM CYL'";           //MAN_0492, //Transfer - Bottom Load Fwd/Bwd
                    gbCyl04.Header = "Load Port Up/Down CYL'";      //MAN_0493, //Transfer - Load Port Up/Down
                    gbCyl05.Header = "Magazine Move L/R CYL'";      //MAN_0494, //Transfer - Magazine Move Left/Right
                    gbCyl06.Header = "Load Cover CYL'";             //MAN_0495, //Transfer - Load Protect Cover 


                    btCyl01_B.Tag = EN_MAN_LIST.MAN_0490; btCyl01_B.Content = "BACKWARD";
                    btCyl01_F.Tag = EN_MAN_LIST.MAN_0490; btCyl01_F.Content = "FORWARD" ;

                    btCyl02_B.Tag = EN_MAN_LIST.MAN_0491; btCyl02_B.Content = "0";
                    btCyl02_F.Tag = EN_MAN_LIST.MAN_0491; btCyl02_F.Content = "180";

                    btCyl03_B.Tag = EN_MAN_LIST.MAN_0492; btCyl03_B.Content = "BACKWARD";
                    btCyl03_F.Tag = EN_MAN_LIST.MAN_0492; btCyl03_F.Content = "FORWARD";

                    btCyl04_F.Tag = EN_MAN_LIST.MAN_0493; btCyl04_B.Content = "DOWN";
                    btCyl04_B.Tag = EN_MAN_LIST.MAN_0493; btCyl04_F.Content = "UP"  ;
                    
                    btCyl05_B.Tag = EN_MAN_LIST.MAN_0494; btCyl05_B.Content = "RIGHT";
                    btCyl05_F.Tag = EN_MAN_LIST.MAN_0494; btCyl05_F.Content = "LEFT" ;
                    
                    btCyl06_B.Tag = EN_MAN_LIST.MAN_0495; btCyl06_B.Content = "BACKWARD";
                    btCyl06_F.Tag = EN_MAN_LIST.MAN_0495; btCyl06_F.Content = "FORWARD";

                    break;

                default:
                    break;
            }

            //
            if ((EN_MAN_LIST)btOneCyle01.Tag > EN_MAN_LIST.MAN_NON) btOneCyle01.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle02.Tag > EN_MAN_LIST.MAN_NON) btOneCyle02.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle03.Tag > EN_MAN_LIST.MAN_NON) btOneCyle03.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle04.Tag > EN_MAN_LIST.MAN_NON) btOneCyle04.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle05.Tag > EN_MAN_LIST.MAN_NON) btOneCyle05.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle06.Tag > EN_MAN_LIST.MAN_NON) btOneCyle06.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle07.Tag > EN_MAN_LIST.MAN_NON) btOneCyle07.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle08.Tag > EN_MAN_LIST.MAN_NON) btOneCyle08.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle09.Tag > EN_MAN_LIST.MAN_NON) btOneCyle09.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle10.Tag > EN_MAN_LIST.MAN_NON) btOneCyle10.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle11.Tag > EN_MAN_LIST.MAN_NON) btOneCyle11.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle12.Tag > EN_MAN_LIST.MAN_NON) btOneCyle12.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle13.Tag > EN_MAN_LIST.MAN_NON) btOneCyle13.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle14.Tag > EN_MAN_LIST.MAN_NON) btOneCyle14.Visibility = Visibility.Visible;
            if ((EN_MAN_LIST)btOneCyle15.Tag > EN_MAN_LIST.MAN_NON) btOneCyle15.Visibility = Visibility.Visible;

        }
        //---------------------------------------------------------------------------
        private void fn_ResetPart(EN_SEQ_ID part)
        {
            lbSelPart1.Background = Brushes.WhiteSmoke; lbSelPart1.FontWeight = FontWeights.Normal; lbSelPart1.IsEnabled = false;
            lbSelPart2.Background = Brushes.WhiteSmoke; lbSelPart2.FontWeight = FontWeights.Normal; lbSelPart2.IsEnabled = false;
            lbSelPart3.Background = Brushes.WhiteSmoke; lbSelPart3.FontWeight = FontWeights.Normal; lbSelPart3.IsEnabled = false;
            lbSelPart4.Background = Brushes.WhiteSmoke; lbSelPart4.FontWeight = FontWeights.Normal; lbSelPart4.IsEnabled = false;
            lbSelPart5.Background = Brushes.WhiteSmoke; lbSelPart5.FontWeight = FontWeights.Normal; lbSelPart5.IsEnabled = false;

            if (part == EN_SEQ_ID.SPINDLE ) { lbSelPart1.Background = Brushes.CornflowerBlue; lbSelPart1.FontWeight = FontWeights.Bold; lbSelPart1.IsEnabled = true;}
            if (part == EN_SEQ_ID.POLISH  ) { lbSelPart2.Background = Brushes.CornflowerBlue; lbSelPart2.FontWeight = FontWeights.Bold; lbSelPart2.IsEnabled = true;}
            if (part == EN_SEQ_ID.CLEAN   ) { lbSelPart3.Background = Brushes.CornflowerBlue; lbSelPart3.FontWeight = FontWeights.Bold; lbSelPart3.IsEnabled = true;}
            if (part == EN_SEQ_ID.STORAGE ) { lbSelPart4.Background = Brushes.CornflowerBlue; lbSelPart4.FontWeight = FontWeights.Bold; lbSelPart4.IsEnabled = true;}
            if (part == EN_SEQ_ID.TRANSFER) { lbSelPart5.Background = Brushes.CornflowerBlue; lbSelPart5.FontWeight = FontWeights.Bold; lbSelPart5.IsEnabled = true;}

            //
            btOneCyle01.Visibility = Visibility.Hidden; btOneCyle01.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle02.Visibility = Visibility.Hidden; btOneCyle02.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle03.Visibility = Visibility.Hidden; btOneCyle03.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle04.Visibility = Visibility.Hidden; btOneCyle04.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle05.Visibility = Visibility.Hidden; btOneCyle05.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle06.Visibility = Visibility.Hidden; btOneCyle06.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle07.Visibility = Visibility.Hidden; btOneCyle07.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle08.Visibility = Visibility.Hidden; btOneCyle08.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle09.Visibility = Visibility.Hidden; btOneCyle09.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle10.Visibility = Visibility.Hidden; btOneCyle10.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle11.Visibility = Visibility.Hidden; btOneCyle11.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle12.Visibility = Visibility.Hidden; btOneCyle12.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle13.Visibility = Visibility.Hidden; btOneCyle13.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle14.Visibility = Visibility.Hidden; btOneCyle14.Tag = EN_MAN_LIST.MAN_NON;
            btOneCyle15.Visibility = Visibility.Hidden; btOneCyle15.Tag = EN_MAN_LIST.MAN_NON;

            cbPlatePlce.Visibility = Visibility.Hidden;
            cbPlatePick.Visibility = Visibility.Hidden;
            
            cbPickMagaNo.Visibility = Visibility.Hidden;
            cbPlceMagaNo.Visibility = Visibility.Hidden;

            cbCupPlce   .Visibility = Visibility.Hidden;
            cbCupPick   .Visibility = Visibility.Hidden;

            cbForceWhere.Visibility = Visibility.Hidden;
            cbForceDCOM .Visibility = Visibility.Hidden;


            //Cylinder
            gbCyl01.Visibility = Visibility.Hidden;
            gbCyl02.Visibility = Visibility.Hidden;
            gbCyl03.Visibility = Visibility.Hidden;
            gbCyl04.Visibility = Visibility.Hidden;
            gbCyl05.Visibility = Visibility.Hidden;
            gbCyl06.Visibility = Visibility.Hidden;

            gbCyl10.Visibility = Visibility.Hidden;


            lbToolExist .Visibility = Visibility.Hidden;
            lbPlateExist.Visibility = Visibility.Hidden;

            //
            //btMoveP.Visibility  = Visibility.Visible;
            //btMoveN.Visibility  = Visibility.Visible;
            //btMoveP1.Visibility = Visibility.Visible;
            //btMoveN1.Visibility = Visibility.Visible;

            //



        }
        //---------------------------------------------------------------------------
        private void dg_MotrPos_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }
        //---------------------------------------------------------------------------
        private void fn_MotorGridSet(ref Grid grd)
        {
            //
            int nCol = 5;
            int nRow = 2;
            int idx  = 0; 

            grd.Children.Clear();
            grd.Background = Brushes.White;
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();

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
                    idx   = r * nCol + c  ;

                    grd.Children.Add(new Label());
                    Label lb = (grd.Children[idx] as Label);

                    lb.FontSize = 12; 

                         if (idx == 0) { lb.Content = "Command"  ; lb.FontSize = 13; }
                    else if (idx == 1) { lb.Content = "Servo"    ; }
                    else if (idx == 2) { lb.Content = "Alarm"    ; }
                    else if (idx == 3) { lb.Content = "Home End" ; }
                    else if (idx == 4) { lb.Content = ""         ; }
                    else if (idx == 5) { lb.Content = "Encoder"  ; lb.FontSize = 13; }
                    else if (idx == 6) { lb.Content = "Limit-"   ; }
                    else if (idx == 7) { lb.Content = "HOME"     ; }
                    else if (idx == 8) { lb.Content = "Limit+"   ; }
                    else if (idx == 9) { lb.Content = "Part Home"; }

                    lb.BorderThickness = new Thickness(1);
                    lb.BorderBrush     = System.Windows.Media.Brushes.Black;
                    lb.Tag             = idx;
                    //lb.FontSize        = 12; 
                    lb.HorizontalContentAlignment = HorizontalAlignment.Center;
                    lb.VerticalContentAlignment   = VerticalAlignment.Center;
                    lb.Margin = new Thickness(1);
                    lb.PreviewMouseDoubleClick += new MouseButtonEventHandler(lbMotor_MouseDoubleClick);
                    
                    Grid.SetColumn(lb, c);  //Grid.SetColumn((grd.Children[idx] as Label), c);
                    Grid.SetRow   (lb, r);  //Grid.SetRow   ((grd.Children[idx] as Label), r);

                    lbMotr[idx] = lb;

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
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            //LEE/201127 [Add] : Position Compare Check
            fn_PosCompare();
            //
            fn_GridToBuff();

            //SAVE
            MOTR.Load(false, FM._sCurrJob, (EN_SEQ_ID)m_nSelPart); //

            SEQ_SPIND.fn_SetVisnCamOffset(); //JUNG/200703
        }
        //---------------------------------------------------------------------------
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //
            
            int nSelectedIndex = dg_MotrPosData.SelectedIndex;
            if (nSelectedIndex < 0) return; 

            string sTemp = m_DataTable.Rows[nSelectedIndex]["POSITION"].ToString();

        }
        //---------------------------------------------------------------------------
        private void fn_GridToBuff()
        {
            if (m_nSelPart < 0) return;
            if (m_nSelMotorNo < 0) return;

            //copy Grid Data to Buffer
            int nRow = m_DataTable.Rows.Count;
            int nPosnId = 0;
            double dPos = 0.0;

            for (int n = 0; n < nRow; n++)
            {
                nPosnId = Convert.ToInt32(m_DataTable.Rows[n]["POSNID"]);
                try
                {
                    dPos = Convert.ToDouble(m_DataTable.Rows[n]["POSITION"]);
                }
                catch (Exception)
                {
                    dPos = 0.0;
                    m_DataTable.Rows[n]["POSITION"] = dPos.ToString("0.000");
                }
                MOTR[m_nSelMotorNo].MP.dPosn[nPosnId] = dPos;

            }
        }
        //---------------------------------------------------------------------------
        private void fn_GridToBuff(ref MOTN_PARA para)
        {
            if (m_nSelPart < 0) return;
            if (m_nSelMotorNo < 0) return;

            //copy Grid Data to Buffer
            int nRow = m_DataTable.Rows.Count;
            int nPosnId = 0;
            double dPos = 0.0;


            for (int n = 0; n < nRow; n++)
            {
                nPosnId = Convert.ToInt32(m_DataTable.Rows[n]["POSNID"]);
                try
                {
                    dPos = Convert.ToDouble(m_DataTable.Rows[n]["POSITION"]);
                }
                catch (Exception)
                {
                    dPos = 0.0;
                    m_DataTable.Rows[n]["POSITION"] = dPos.ToString("0.000");
                }
                para.dPosn[nPosnId] = dPos;
            }
        }
        //---------------------------------------------------------------------------
        private void Move_Click(object sender, RoutedEventArgs e)
        {
            //Move Position 

            int nSelIndex = dg_MotrPosData.SelectedIndex;
            if (nSelIndex < 0) return;

            double dPos = Convert.ToDouble(m_DataTable.Rows[nSelIndex]["POSITION"]);

            //MOTR.MoveMotr((EN_MOTR_ID)m_nSelMotorNo, dPos);
            
            int nManNo = Convert.ToInt32(m_DataTable.Rows[nSelIndex]["MANNO"]);
            MAN.fn_ManProcOn(nManNo, true, false);

        }
        //---------------------------------------------------------------------------
        private void JOGP_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Jog Mouse Down.
            if (m_nSelMotorNo < 0) return; 

            DownBtn = sender as Button;
            m_bDrngDown = true;

            //MOTR.MoveJog((EN_MOTR_ID)m_nSelMotorNo, true);

            //MAN._bJog = true; 
            //int nManNo = MOTR.ManNoJog((EN_MOTR_ID)m_nSelMotorNo);
            //MAN.fn_ManProcOn(nManNo, true, false);

        }
        //---------------------------------------------------------------------------
        private void JOGButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_bDrngDown = false;
            DownBtn = null;

            //
            MAN._bJog = false;
            MOTR.Stop((EN_MOTR_ID)m_nSelMotorNo);

        }
        //---------------------------------------------------------------------------
        private void JOGN_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //
            //Jog Mouse Down.
            if (m_nSelMotorNo < 0) return;

            DownBtn = sender as Button;
            m_bDrngDown = true;

            //MOTR.MoveJog((EN_MOTR_ID)m_nSelMotorNo, false);

            //MAN._bJog = true;
            //int nManNo = MOTR.ManNoJog((EN_MOTR_ID)m_nSelMotorNo);
            //MAN.fn_ManProcOff(nManNo, false, true);


        }
        //---------------------------------------------------------------------------
        private void btStop_Click(object sender, RoutedEventArgs e)
        {

            MAN._bJog = false;
            MOTR.Stop();

        }
        //---------------------------------------------------------------------------
        private void btOneCyle01_Click(object sender, RoutedEventArgs e)
        {

            //
            int nIndex = 0;
            Button selbtn = sender as Button;

            int nTag = Convert.ToInt32(selbtn.Tag);

            if (nTag == (int)EN_MAN_LIST.MAN_0402) //SPINDLE - Plate Place One Cycle
            {
                nIndex = cbPlatePlce.SelectedIndex;
                if (nIndex >= 0) nTag += nIndex;
            }
            else if (nTag == (int)EN_MAN_LIST.MAN_0405) //SPINDLE - Plate Pick One Cycle
            {
                nIndex = cbPlatePick.SelectedIndex;
                if (nIndex >= 0) nTag += nIndex;
            }
            else if (nTag == (int)EN_MAN_LIST.MAN_0452) //Transfer - Plate Pick One Cycle
            {
                nIndex = cbPickMagaNo.SelectedIndex;
                MAN._nSelMaga = 0;
                if (nIndex >= 15) { nIndex -= 15; MAN._nSelMaga = 1; } //JUNG/200602
                if (nIndex >=  0) MAN._nSelMagaSlot = nIndex;
            }
            else if (nTag == (int)EN_MAN_LIST.MAN_0453) //Transfer - Plate place One Cycle
            {
                nIndex = cbPlceMagaNo.SelectedIndex;
                MAN._nSelMaga = 0;
                if (nIndex >= 15) { nIndex -= 15; MAN._nSelMaga = 1; } //JUNG/200602
                if (nIndex >=  0) MAN._nSelMagaSlot = nIndex;
            }
            else if (nTag == (int)EN_MAN_LIST.MAN_0414) //
            {
                nIndex = cbCupPick.SelectedIndex;
                if (nIndex >= 0) nTag += nIndex;
            }
            else if (nTag == (int)EN_MAN_LIST.MAN_0416) //
            {
                nIndex = cbCupPlce.SelectedIndex;
                if (nIndex >= 0) nTag += nIndex;
            }
            else if (nTag == (int)EN_MAN_LIST.MAN_0409) //
            {
                MAN._nWhere = cbForceWhere.SelectedIndex;
                MAN._dDCOM  = cbForceDCOM .SelectedIndex + 1;
            }


            //
            MAN.fn_ManProcOn(nTag, true, false);

            UserFunction.fn_WriteLog(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));
            
            Console.WriteLine(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));

        }
        //---------------------------------------------------------------------------
        private void btJogP_Click(object sender, RoutedEventArgs e)
        {
            //
            if (m_nSelMotorNo < 0) return;

            if (DownBtn == null)
            {
                MOTR.Stop((EN_MOTR_ID)m_nSelMotorNo);
                return;
            }
            //
            switch (DownBtn.Name)
            {
                case "btJogN":
                    MOTR.MoveJog((EN_MOTR_ID)m_nSelMotorNo, false);
                    break;
                case "btJogP":
                    MOTR.MoveJog((EN_MOTR_ID)m_nSelMotorNo, true);
                    break;

                case "btMoveP": //JUNG/200506
                    IO.fn_MovePTP_R(m_nSelMotorNo, 0.1);
                    break;
                
                case "btMoveN":
                    IO.fn_MovePTP_R(m_nSelMotorNo, -0.1);
                    break;
                
                case "btMoveP1": //JUNG/200506
                    IO.fn_MovePTP_R(m_nSelMotorNo, 1.0);
                    break;

                case "btMoveN1":
                    IO.fn_MovePTP_R(m_nSelMotorNo, -1.0);
                    break;

                default:
                    MOTR.Stop((EN_MOTR_ID)m_nSelMotorNo);
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void btCyl01_B_Click(object sender, RoutedEventArgs e)
        {
            //BWD
            Button selbtn = sender as Button;
            int nTag = Convert.ToInt32(selbtn.Tag);
            
            //
            MAN.fn_ManProcOff(nTag, false, true);

            UserFunction.fn_WriteLog(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));
        }
        //---------------------------------------------------------------------------
        private void btCyl01_F_Click(object sender, RoutedEventArgs e)
        {
            //FWD
            Button selbtn = sender as Button;
            int nTag = Convert.ToInt32(selbtn.Tag);

            //
            MAN.fn_ManProcOn(nTag, true, false);

            UserFunction.fn_WriteLog(string.Format($"Manual Button Click Number : {nTag + 1:D4}"));

        }
        //---------------------------------------------------------------------------
        private void btSetSoftLimit_Click(object sender, RoutedEventArgs e)
        {
            //
            if (m_nSelMotorNo < 0) return;
            IO.fn_SetSoftLimit(m_nSelMotorNo, true);
        }
        //-------------------------------------------------------------------------------------------------
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl + Mouse Double Click
            if (e.Key == Key.LeftCtrl) Key_LeftCtrl = true;
            else                       Key_LeftCtrl = false;

        }
        //-------------------------------------------------------------------------------------------------
        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            Key_LeftCtrl = false;
        }
        //-------------------------------------------------------------------------------------------------
        private void dg_MotrPosData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            m_nSelIndex = dg_MotrPosData.SelectedIndex;
        }
        //-------------------------------------------------------------------------------------------------
        private void btSetCurrPos_Click(object sender, RoutedEventArgs e)
        {
            //Ctrl + Mouse Double Click
            if (Key_LeftCtrl)
            {
                if (m_nSelIndex < 0) return;

                double dPos = MOTR[m_nSelMotorNo].GetEncPos();

                m_DataTable.Rows[m_nSelIndex]["POSITION"] = dPos.ToString();

                Key_LeftCtrl = false;
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void btToolUnClamp_Click(object sender, RoutedEventArgs e)
        {
            //
            Button selBtn = sender as Button;
            string sName = selBtn.Name as string;

            switch (sName)
            {
                case "btToolUnClamp":
                    SEQ_SPIND.fn_MoveToolClamp(ccBwd);
                    break;
                
                case "btToolClamp":
                    SEQ_SPIND.fn_MoveToolClamp(ccFwd);
                    break;
                default:
                    break;
            }
        }
        //---------------------------------------------------------------------------
        private void btMoveP_Click(object sender, RoutedEventArgs e)
        {
            //
            Button DownBtn = sender as Button; 

            if (m_nSelMotorNo < 0) return;

            if (DownBtn == null)
            {
                MOTR.Stop((EN_MOTR_ID)m_nSelMotorNo);
                return;
            }

            //
            switch (DownBtn.Name)
            {
                case "btMoveP": //JUNG/200506
                    
                    if((MOTR[m_nSelMotorNo].GetCW() || MOTR[m_nSelMotorNo].GetSRL())) //Limit Check
                    {
                        return; 
                    }
                    IO.fn_MovePTP_R(m_nSelMotorNo, 0.1);
                    break;
                
                case "btMoveN":
                    if ((MOTR[m_nSelMotorNo].GetCCW() || MOTR[m_nSelMotorNo].GetSLL())) //Limit Check
                    {
                        return;
                    }
                    IO.fn_MovePTP_R(m_nSelMotorNo, -0.1);
                    break;
                
                case "btMoveP1": //JUNG/200506
                    if ((MOTR[m_nSelMotorNo].GetCW() || MOTR[m_nSelMotorNo].GetSRL())) //Limit Check
                    {
                        return;
                    }

                    IO.fn_MovePTP_R(m_nSelMotorNo, 1.0);
                    break;

                case "btMoveN1":
                    if ((MOTR[m_nSelMotorNo].GetCCW() || MOTR[m_nSelMotorNo].GetSLL())) //Limit Check
                    {
                        return;
                    }

                    IO.fn_MovePTP_R(m_nSelMotorNo, -1.0);
                    break;

                default:
                    MOTR.Stop((EN_MOTR_ID)m_nSelMotorNo);
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void fn_PosCompare()
        {
            MOTN_PARA para = new MOTN_PARA(0.0);
            MOTN_PARA buff = new MOTN_PARA(0.0);
            //LOAD
            MOTR.Load(ref para, m_nSelMotorNo, FM._sCurrJob, (EN_SEQ_ID)m_nSelPart); //

            fn_GridToBuff(ref buff);

            for (int i = 0; i < para.dPosn.Length; i++)
            {
                if (para.dPosn[i] != buff.dPosn[i])
                {
                    if (para.sPosn_Desc[i] != null && para.sPosn_Desc[i] != "")
                    {
                        fn_WriteLog($"{para.sPosn_Desc[i]} / Before = {para.dPosn[i]:F3} / Final = {buff.dPosn[i]:F3}", EN_LOG_TYPE.ltTrace);
                    }
                }
            }
        }
    }
}
