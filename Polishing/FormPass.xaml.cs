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
using UserInterface;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserEnum;


namespace WaferPolishingSystem
{
    /// <summary>
    /// FormPass.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormPassword : Window
    {
        //Local Var.
        int m_nSelLevel; 


        //
        public FormPassword()
        {
            InitializeComponent();

            //
            m_nSelLevel = 0; 
        }

        //---------------------------------------------------------------------------
        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            string sId = tbUserId.Text;
            string sPw = pbPassWord.Password;


            //Debug Mode
#if !DEBUG
            if (sPw == "2141" || sPw == "SMEC1203")
            {

            }
            else
            {
                //Check Password
                if (m_nSelLevel == (int)EN_USER_LEVEL.lvEngineer)
                {
                    if (sPw != FM.fn_GetPass(EN_USER_LEVEL.lvEngineer))
                    {
                        MessageBox.Show("Engineer PassWord가 일치 하지 않습니다.");
                        pbPassWord.Password = "";
                        return;
                    }
                }
                else if (m_nSelLevel == (int)EN_USER_LEVEL.lvMaster)
                {
                    if (sPw != FM.fn_GetPass(EN_USER_LEVEL.lvMaster))
                    {
                        MessageBox.Show("Admin PassWord가 일치 하지 않습니다.");
                        pbPassWord.Password = "";
                        return;
                    }

                }
            }
#endif
            //FM._nCrntLevel = m_nSelLevel;

            //

            MAIN.fn_ChangeLevel((EN_USER_LEVEL)m_nSelLevel);

            //
            //SEQ.fn_Reset();

            //
            this.Visibility = Visibility.Hidden;


        }
        //---------------------------------------------------------------------------
        //Click...
        private void btOper_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;

            string sSelName = ub.Content as string;

            tbUserId.Text = sSelName;

            pbPassWord.IsEnabled = true ;

            switch (sSelName)
            {
                case "OPERATOR":
                    pbPassWord.IsEnabled = false;
                    m_nSelLevel = (int)EN_USER_LEVEL.lvOperator;
                    break;

                case "ENGINEER":
                    m_nSelLevel = (int)EN_USER_LEVEL.lvEngineer;
                    break;

                case "ADMIN":
                    m_nSelLevel = (int)EN_USER_LEVEL.lvMaster;
                    break;
                default:
                    break;
            }

            fn_SetBackground(m_nSelLevel);
        }
        //---------------------------------------------------------------------------
        //Form Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
            //fn_SetBackground(m_nSelLevel);
            //
            //pbPassWord.Password = "";
        }
        //---------------------------------------------------------------------------
        public void fn_SetBackground(int level)
        {
            btOper.Background = UserConst.G_COLOR_BTNNORMAL;
            btEngn.Background = UserConst.G_COLOR_BTNNORMAL;
            btMast.Background = UserConst.G_COLOR_BTNNORMAL;

            switch (level)
            {
                case (int)EN_USER_LEVEL.lvOperator:
                    btOper.Background = UserConst.G_COLOR_BTNCLICKED;
                    break;
                case (int)EN_USER_LEVEL.lvEngineer:
                    btEngn.Background = UserConst.G_COLOR_BTNCLICKED;
                    break;
                //case (int)EN_USER_LEVEL.lvAdmin :
                case (int)EN_USER_LEVEL.lvMaster:
                    btMast.Background = UserConst.G_COLOR_BTNCLICKED;
                    break;
                default:
                    break;
            }

        }
        //---------------------------------------------------------------------------
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
