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
using WaferPolishingSystem.Unit;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;


namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Manual_REST : Page
    {
        //Var

        //int m_nIndex = 0;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_Manual_REST()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;


        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            lbConnect.Content = REST._bConnect ? "Connect OK" : "disconnect";
            fn_SetLabelColor(ref lbConnect, REST._bConnect, Brushes.Lime, Brushes.Gray);

            lbVersion.Content = REST._sVersion;

            btResdRFID.IsEnabled = RFID._bConnect ? true : false;

            //Wafer Info
            if (REST._bUpdateData)
            {
                //
                //lbPlateId.Content = REST.RcvPlateRFIDInfo._sPlateId;

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
                
                lbState.Content = "DATA OK";
                lbState.Background = Brushes.Lime;

            }
            else if(REST._bDataError)
            {
                lbState.Content = "DATA ERROR!!";
                lbState.Background = Brushes.Yellow;
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
        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            Button selbtn = sender as Button; 

            switch(selbtn.Name)
            {
                case "btConnect" :
                    REST.fn_ReqConnection();//REST.fn_Connect();
                    break;
                case "btVersion" :
                    REST.fn_ReqVersion(); //REST.fn_GetVersion();
                    break;
                case "btRFIDInfo":
                    REST.fn_ReqRFInfo(tbPlatNo.Text);//REST.fn_GetRFIDInfo(tbPlatNo.Text);
                    break;
                    

            }


        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Timer On
            fn_SetTimer(true);
        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //Timer On
            fn_SetTimer(false);
        }
    }
}
