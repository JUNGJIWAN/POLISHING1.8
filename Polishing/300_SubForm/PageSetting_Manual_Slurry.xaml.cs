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
using UserInterface;

using WaferPolishingSystem.Define;
using static WaferPolishingSystem.BaseUnit.ManualId;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;



namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Manual_Slurry : Page
    {
        //Var

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_Manual_Slurry()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick    += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;
        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            SUPPLY[SPLY_SLURRY].fn_UpdateState();
            SUPPLY[SPLY_SOAP  ].fn_UpdateState();

            btSlury01.Background     = SUPPLY[SPLY_SLURRY]._bSlurryReqState   ? Brushes.Lime : Brushes.LightGray;
            btSlury02.Background     = SUPPLY[SPLY_SLURRY]._bCleaningReqState ? Brushes.Lime : Brushes.LightGray;
            lbSReqlurry  .Background = SUPPLY[SPLY_SLURRY]._bSlurryReq        ? Brushes.Lime : Brushes.LightGray;
            lbReqSlurryDI.Background = SUPPLY[SPLY_SLURRY]._bCleanReq         ? Brushes.Lime : Brushes.LightGray;

            //btSlury04.Background = SUPPLY[1]._bSlurryReq   ? Brushes.Lime : Brushes.LightGray;
            //btSlury05.Background = SUPPLY[1]._bCleanReq    ? Brushes.Lime : Brushes.LightGray;
            //btSlury06.Background = SUPPLY[1]._bDrainvValve ? Brushes.Lime : Brushes.LightGray;

            btSlury51.Background   = SUPPLY[SPLY_SOAP]._bSlurryReqState   ? Brushes.Lime : Brushes.LightGray;
            btSlury52.Background   = SUPPLY[SPLY_SOAP]._bCleaningReqState ? Brushes.Lime : Brushes.LightGray;
            lbReqSoap  .Background = SUPPLY[SPLY_SOAP]._bSlurryReq        ? Brushes.Lime : Brushes.LightGray;
            lbReqSoapDI.Background = SUPPLY[SPLY_SOAP]._bCleanReq         ? Brushes.Lime : Brushes.LightGray;
            

            //
            m_UpdateTimer.Start();
        }
        //---------------------------------------------------------------------------
        //Timer On/Off
        public void fn_SetTimer(bool on)
        {
            if (on) m_UpdateTimer.Start();
            else    m_UpdateTimer.Stop ();
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //
            fn_SetTimer(true);

            SUPPLY[SPLY_SLURRY].fn_SetReadGrid(ref gdFlag01);
            SUPPLY[SPLY_SLURRY].fn_SetSendGrid(ref gdFlag02);

            SUPPLY[SPLY_SOAP  ].fn_SetReadGrid(ref gdFlag11);
            SUPPLY[SPLY_SOAP  ].fn_SetSendGrid(ref gdFlag12);

        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //
            fn_SetTimer(false);
        }
        //---------------------------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //
            Button selbtn  = sender as Button;
            int    nSelTag = Convert.ToInt32(selbtn.Tag);
            int    nTag = 0;

            switch (nSelTag)
            {
                case 0:
                    SUPPLY[0]._bSlurryReq = !SUPPLY[0]._bSlurryReq;
                    //if (SUPPLY[0]._bSlurryReq)
                    //{
                    //    SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.Silica01);
                    //}
                    //else
                    //{
                    //    SEQ_POLIS.fn_StopAllUtil();
                    //}
                    break;
                case 1:
                    SUPPLY[0]._bCleanReq = !SUPPLY[0]._bCleanReq;
                    //if (SUPPLY[0]._bCleanReq)
                    //{
                    //    SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.Silica01);  //SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.DIWater);
                    //}
                    //else
                    //{
                    //    SEQ_POLIS.fn_StopAllUtil();
                    //}
                    break;
                case 2:
                    nTag = (int)EN_MAN_LIST.MAN_0421;
                    MAN.fn_ManProcOn(nTag, true, false);
                    break;

                //case 3:
                //    SUPPLY[1]._bSlurryReq = !SUPPLY[1]._bSlurryReq;
                //    break;
                //case 4:
                //    SUPPLY[1]._bCleanReq = !SUPPLY[1]._bCleanReq;
                //    break;
                //case 5:
                //    SUPPLY[1]._bDrainvValve = !SUPPLY[1]._bDrainvValve;
                //    break;
                case 10:
                    SUPPLY[1]._bSlurryReq = !SUPPLY[1]._bSlurryReq;
                    //if (SUPPLY[1]._bSlurryReq)
                    //{
                    //    SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.Soap);
                    //}
                    //else
                    //{
                    //    SEQ_POLIS.fn_StopAllUtil();
                    //}
                    break;
                case 11:
                    SUPPLY[1]._bCleanReq = !SUPPLY[1]._bCleanReq;
                    //if (SUPPLY[0]._bCleanReq)
                    //{
                    //    SEQ_POLIS.fn_SupplyUtil(EN_UTIL_KIND.DIWater);
                    //}
                    //else
                    //{
                    //    SEQ_POLIS.fn_StopAllUtil();
                    //}
                    break;

                default:
                    break;
            }


        }
    }
}
