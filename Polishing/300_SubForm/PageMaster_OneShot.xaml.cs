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
    public partial class PageMaster_OneShot : Page
    {
        //Var
        int m_nOneShotTag; 

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();
        private DispatcherTimer m_OnShotTimer = new DispatcherTimer();

        public PageMaster_OneShot()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick    += new EventHandler(fn_tmUpdate);

            //
            m_OnShotTimer.IsEnabled = false;
            m_OnShotTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_OnShotTimer.Tick    += new EventHandler(fn_tmOnShot);


            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;

            m_nOneShotTag = -1;

            btOnShot1.Tag = (int)EN_SEQ_ID.SPINDLE ;
            btOnShot2.Tag = (int)EN_SEQ_ID.POLISH  ;
            btOnShot3.Tag = (int)EN_SEQ_ID.CLEAN   ;
            btOnShot4.Tag = (int)EN_SEQ_ID.STORAGE ;
            btOnShot5.Tag = (int)EN_SEQ_ID.TRANSFER;


            btReset1.Tag  = (int)EN_SEQ_ID.SPINDLE ;
            btReset2.Tag  = (int)EN_SEQ_ID.POLISH  ;
            btReset3.Tag  = (int)EN_SEQ_ID.CLEAN   ;
            btReset4.Tag  = (int)EN_SEQ_ID.STORAGE ;
            btReset5.Tag  = (int)EN_SEQ_ID.TRANSFER;

        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            SEQ_SPIND.fn_UpdateOneShot(ref gdOneShot01);
            SEQ_POLIS.fn_UpdateOneShot(ref gdOneShot02);
            SEQ_CLEAN.fn_UpdateOneShot(ref gdOneShot03);
            SEQ_STORG.fn_UpdateOneShot(ref gdOneShot04);
            SEQ_TRANS.fn_UpdateOneShot(ref gdOneShot05);
            
            //SEQ      .fn_UpdateOneShot(ref gdFlag06);

            //
            m_UpdateTimer.Start();
        }
        //---------------------------------------------------------------------------
        private void fn_tmOnShot(object sender, EventArgs e)
        {
            //
            if(SEQ._bRun)
            {
                m_nOneShotTag = -1;
                m_OnShotTimer.IsEnabled = false;
                return;
            }

            //
            switch ((EN_SEQ_ID)m_nOneShotTag)
            {
                case EN_SEQ_ID.SPINDLE:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.SPINDLE]) { SEQ_SPIND.fn_AutoRun(); } 
                    break;
                case EN_SEQ_ID.POLISH:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.POLISH]) { SEQ_POLIS.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.CLEAN:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.CLEAN]) { SEQ_CLEAN.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.STORAGE:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.STORAGE]) { SEQ_STORG.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.TRANSFER:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.TRANSFER]) { SEQ_TRANS.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.SYSTEM:
                    break;
                case EN_SEQ_ID.ALL:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.SPINDLE ]) { SEQ_SPIND.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.POLISH  ]) { SEQ_POLIS.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.CLEAN   ]) { SEQ_CLEAN.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.STORAGE ]) { SEQ_STORG.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.TRANSFER]) { SEQ_TRANS.fn_AutoRun(); }

                    break;

                default:
                    break;
            }

            m_nOneShotTag = -1;
            m_OnShotTimer.IsEnabled = false;


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
        //---------------------------------------------------------------------------
        private void btReset1_Click(object sender, RoutedEventArgs e)
        {
            //
            if (SEQ._bRun)
            {
                MessageBox.Show("Can not use during run.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            m_OnShotTimer.IsEnabled = false;

            //Reset 
            Button selbtn = sender as Button;
            int nTag = Convert.ToInt32(selbtn.Tag);

            if (nTag < 0) return;

            switch ((EN_SEQ_ID)nTag)
            {
                case EN_SEQ_ID.SPINDLE: //Spindle
                    SEQ_SPIND.fn_Reset();
                    break;
                case EN_SEQ_ID.POLISH: 
                    SEQ_POLIS.fn_Reset();
                    break;
                case EN_SEQ_ID.CLEAN: 
                    SEQ_CLEAN.fn_Reset();
                    break;
                case EN_SEQ_ID.STORAGE: 
                    SEQ_STORG.fn_Reset();
                    break;
                case EN_SEQ_ID.TRANSFER: 
                    SEQ_TRANS.fn_Reset();
                    break;
                case EN_SEQ_ID.ALL:
                    SEQ_SPIND.fn_Reset();
                    SEQ_POLIS.fn_Reset();
                    SEQ_CLEAN.fn_Reset();
                    SEQ_STORG.fn_Reset();
                    SEQ_TRANS.fn_Reset();
                    break;

                default:
                    break;
            }
        }
        //---------------------------------------------------------------------------
        private void btOnShot1_Click(object sender, MouseButtonEventArgs e)
        {
            //
            if (SEQ._bRun)
            {
                MessageBox.Show("Can not use during run.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            //One Shot
            Button selbtn = sender as Button;
            int nTag = Convert.ToInt32(selbtn.Tag);

            if (nTag < 0) return;

            //m_nOneShotTag = nTag;
            //m_OnShotTimer.IsEnabled = true;
           
            //
            switch ((EN_SEQ_ID)nTag)
            {
                case EN_SEQ_ID.SPINDLE:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.SPINDLE]) { SEQ_SPIND.fn_AutoRun(); } 
                    break;
                case EN_SEQ_ID.POLISH:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.POLISH]) { SEQ_POLIS.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.CLEAN:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.CLEAN]) { SEQ_CLEAN.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.STORAGE:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.STORAGE]) { SEQ_STORG.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.TRANSFER:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.TRANSFER]) { SEQ_TRANS.fn_AutoRun(); }
                    break;
                case EN_SEQ_ID.SYSTEM:
                    SEQ.fn_CheckRunCon();
                    break;
                case EN_SEQ_ID.ALL:
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.SPINDLE ]) { SEQ_SPIND.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.POLISH  ]) { SEQ_POLIS.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.CLEAN   ]) { SEQ_CLEAN.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.STORAGE ]) { SEQ_STORG.fn_AutoRun(); }
                    if (!FM.m_stMasterOpt.bAutoOff[(int)EN_SEQ_ID.TRANSFER]) { SEQ_TRANS.fn_AutoRun(); }
                    break;

                default:
                    break;
            }

           
           

        }
        //---------------------------------------------------------------------------
        private void btOnShot1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            //
            m_nOneShotTag = -1;
            m_OnShotTimer.IsEnabled = false;
        }

    }
}
