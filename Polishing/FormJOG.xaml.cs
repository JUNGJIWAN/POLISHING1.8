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
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;


namespace WaferPolishingSystem
{
    public static class MyBtn
    {
        public static void PerformClick(this Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }

    /// <summary>
    /// FormMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormJog : Window
    {

        //Local var.
        int    m_nSelMotr;
        bool   m_bDrngDown;
        Button DownBtn  = new Button() ; 

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public FormJog()
        {
            InitializeComponent();

            //
            m_nSelMotr = -1;
            m_bDrngDown = false; 


            //
            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);


        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Message Window Show
        </summary>
        <param name="msg"> Display Message </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 10:08
        */
        public void fn_ShowJog(int type = 0)
        {
            if (this.IsVisible) return ;

            //Modal
            this.ShowDialog();
        }
        //---------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //
        }
        //---------------------------------------------------------------------------
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            if (m_bDrngDown)
            {
                DownBtn.PerformClick();
            }

            m_nSelMotr = cbMotr.SelectedIndex; 
            if (m_nSelMotr >= 0)
            {
                lbCmdPos.Content = MOTR.GetCmdPos((EN_MOTR_ID)m_nSelMotr);
                lbEncPos.Content = MOTR.GetEncPos((EN_MOTR_ID)m_nSelMotr);
            }

            if(MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nSelMotr)) //JUNG/200608
            {
                btMoveN01.IsEnabled = false; 
                btMoveN10.IsEnabled = false;
                btMoveP01.IsEnabled = false;
                btMoveP10.IsEnabled = false;
            }
            else
            {
                btMoveN01.IsEnabled = true;
                btMoveN10.IsEnabled = true;
                btMoveP01.IsEnabled = true;
                btMoveP10.IsEnabled = true;
            }



            m_UpdateTimer.Start();

        }
        //---------------------------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
            m_UpdateTimer.Start();

            for (int n=0; n < MOTR._iNumOfMotr; n++)
            {
                cbMotr.Items.Add(string.Format($"{MOTR[n].m_sNameAxis}_{MOTR[n].m_sName}"));
            }

            this.Topmost = true;
        }
        //---------------------------------------------------------------------------
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            m_UpdateTimer.Stop();
        }
        //---------------------------------------------------------------------------
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            //
            //this.Close();
            UserFunction.fn_UserJogClose(); //JUNG/200622
        }
        //---------------------------------------------------------------------------
        private void btJOGN_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //
            if (m_nSelMotr < 0) return;

            DownBtn = sender as Button;
            m_bDrngDown = true; 

            //int nManNo = FormMain.MOTR.ManNoJog((EN_MOTR_ID)m_nSelMotr);
            //FormMain.MAN.fn_ManProcOff(nManNo, false, true);
        }
        //---------------------------------------------------------------------------
        private void btJOGN_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_nSelMotr < 0) return;

            m_bDrngDown = false;
            DownBtn = null;
            

            //
            MAN._bJog = false;
            MOTR.Stop((EN_MOTR_ID)m_nSelMotr);
        }   
        //---------------------------------------------------------------------------
        private void btJOGP_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //
            if (m_nSelMotr < 0) return;

            DownBtn = sender as Button;
            m_bDrngDown = true;


            //int nManNo = MOTR.ManNoJog((EN_MOTR_ID)m_nSelMotr);
            //MAN.fn_ManProcOn(nManNo, true, false);
        }
        //---------------------------------------------------------------------------
        private void btJOGP_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_nSelMotr < 0) return;

            //
            m_bDrngDown = false;
            DownBtn = null;

            MAN._bJog = false;
            MOTR.Stop((EN_MOTR_ID)m_nSelMotr);
        }
        //---------------------------------------------------------------------------
        private void btMove_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (!double.TryParse(tbSetPos.Text, out double dPos)) return; 

            if (dPos < 0) return;

            MAN._dDirectPos = dPos;

            int nManNo = (int)MOTR.ManNoDirect(EN_MOTR_ID.miSPD_X) + (25 * m_nSelMotr); 

            MAN.fn_ManProcOn( nManNo, true, false);
            
        }
        //---------------------------------------------------------------------------
        private void btJOGN_Click(object sender, RoutedEventArgs e)
        {

            if (m_nSelMotr < 0) return;

            if (DownBtn == null)
            {
                MOTR.Stop((EN_MOTR_ID)m_nSelMotr);
                return;
            }
            //
            switch (DownBtn.Name)
            {
                case "btJOGN":
                    MOTR.MoveJog((EN_MOTR_ID)m_nSelMotr, false);
                    break;
                case "btJOGP":
                    MOTR.MoveJog((EN_MOTR_ID)m_nSelMotr, true);
                    break;
                default:
                    MOTR.Stop((EN_MOTR_ID)m_nSelMotr);
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void btMoveN01_Click(object sender, RoutedEventArgs e)
        {
            //
            if (m_nSelMotr < 0) return;
            if (MOTR.fn_IsSMCMotor((EN_MOTR_ID)m_nSelMotr)) return;  //JUNG/200608

            Button selbtn = sender as Button;

            switch (selbtn.Name)
            {
                default:
                    break;
                case "btMoveN01": IO.fn_MovePTP_R(m_nSelMotr, -0.1); break; 
                case "btMoveN10": IO.fn_MovePTP_R(m_nSelMotr, -1.0); break; 
                case "btMoveP01": IO.fn_MovePTP_R(m_nSelMotr, +0.1); break; 
                case "btMoveP10": IO.fn_MovePTP_R(m_nSelMotr, +1.0); break; 
            }         
        }
        //---------------------------------------------------------------------------
        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
