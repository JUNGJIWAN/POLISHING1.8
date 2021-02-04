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
using static WaferPolishingSystem.Define.UserEnum;

namespace WaferPolishingSystem
{
    
    /// <summary>
    /// FormMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormMessage : Window
    {
        bool m_bClose  = false;
        bool m_bResult = false;

        public int    m_nKind;
        public string m_sTitle;
        public string m_sMsg;


        public bool _bClose
        {
            get { return m_bClose ; }
            set { m_bClose = value; }
        }
        public bool _bResult { get { return m_bResult; } }

        public FormMessage()
        {
            InitializeComponent();

            //
            menuOk    .Visibility = Visibility.Hidden ;
            menuCancel.Visibility = Visibility.Hidden ;
            menuOk_1  .Visibility = Visibility.Visible;

            m_bResult = false;

            m_nKind  = 0;
            m_sTitle = string.Empty;
            m_sMsg   = string.Empty;

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
        public bool fn_ShowMsg(string msg, string caption, int type = 0 )
        {
            if (this.IsVisible) return false;

            m_bResult = false;

            string sCaption = caption; 

            //Type에 따라 Button Display
            menuOk    .Visibility = Visibility.Hidden;  
            menuCancel.Visibility = Visibility.Hidden;
            menuOk_1  .Visibility = Visibility.Hidden;

            if (type == (int)EN_MSG_TYPE.Info)
            {
                menuOk_1.Visibility = Visibility.Visible;
                if (sCaption == "") sCaption = "INFORMATION";
            }
            else if (type == (int)EN_MSG_TYPE.Check)
            {
                menuOk    .Visibility = Visibility.Visible;
                menuCancel.Visibility = Visibility.Visible;

                if (sCaption == "") sCaption = "CHECK";
            }
            else if (type == (int)EN_MSG_TYPE.Warning)
            {
                menuOk_1.Visibility = Visibility.Visible;
                if (sCaption == "") sCaption = "WARNING";
            }

            this.Title = sCaption;//caption; 
            lbMsg.Content = msg;

            //Modal
            this.ShowDialog();

            return m_bResult ;

        }
        //---------------------------------------------------------------------------
        public bool fn_SetMsg(string msg, string caption, EN_MSG_TYPE type = 0 )
        {
            m_bResult = false;

            string sCaption = caption; 

            //Type에 따라 Button Display
            menuOk    .Visibility = Visibility.Hidden;  
            menuCancel.Visibility = Visibility.Hidden;
            menuOk_1  .Visibility = Visibility.Hidden;

            if (type == EN_MSG_TYPE.Info)
            {
                menuOk_1.Visibility = Visibility.Visible;
                if (sCaption == "") sCaption = "CONFIRM";
            }
            else if (type == EN_MSG_TYPE.Check)
            {
                menuOk    .Visibility = Visibility.Visible;
                menuCancel.Visibility = Visibility.Visible;

                if (sCaption == "") sCaption = "CHECK";
            }
            else if (type == EN_MSG_TYPE.Warning)
            {
                menuOk_1.Visibility = Visibility.Visible;
                if (sCaption == "") sCaption = "WARNING";
            }

            this.Title = sCaption;//caption; 
            lbMsg.Content = msg;

            return true; 
            

        }
        //---------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //
//             if (!m_bClose)
//             {
//                 e.Cancel = true;
//                 this.Hide();
//             }

        }
        //---------------------------------------------------------------------------
        private void menuCancel_Click(object sender, RoutedEventArgs e)
        {
            m_bResult = false;

            this.Hide();
            //this.Close();
        }
        //---------------------------------------------------------------------------
        private void menuOk_Click(object sender, RoutedEventArgs e)
        {
            m_bResult = true;

            this.Hide();
            //this.Close();
        }
    }
}
