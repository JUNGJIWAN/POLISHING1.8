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
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;



namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageMaster_Flag : Page
    {
        //Var

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageMaster_Flag()
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

            SEQ_SPIND.fn_UpdateFlag(ref gdFlag01);
            SEQ_POLIS.fn_UpdateFlag(ref gdFlag02);
            SEQ_CLEAN.fn_UpdateFlag(ref gdFlag03);
            SEQ_STORG.fn_UpdateFlag(ref gdFlag04);
            SEQ_TRANS.fn_UpdateFlag(ref gdFlag05);

            SEQ      .fn_UpdateFlag(ref gdFlag06);

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

            lbFlagName01.Content    = EN_SEQ_ID.SPINDLE .ToString();
            lbFlagName02.Content    = EN_SEQ_ID.POLISH  .ToString();
            lbFlagName03.Content    = EN_SEQ_ID.CLEAN   .ToString();
            lbFlagName04.Content    = EN_SEQ_ID.STORAGE .ToString();
            lbFlagName05.Content    = EN_SEQ_ID.TRANSFER.ToString();
            lbFlagName06.Content    = EN_SEQ_ID.SYSTEM  .ToString();

            lbFlagName07.Visibility = Visibility.Hidden;
            lbFlagName08.Visibility = Visibility.Hidden;

        }
        //---------------------------------------------------------------------------
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //
            fn_SetTimer(false);
        }
    }
}
