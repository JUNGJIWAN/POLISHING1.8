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
using static WaferPolishingSystem.FormMain;

namespace WaferPolishingSystem
{
    /// <summary>
    /// KeyPad.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPad : Window
    {
        string sOperator = "";
        string sPrevInput = "";
        public KeyPad()
        {
            InitializeComponent();
        }

        private void bn_OK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            fn_Close();
        }

        public void fn_Close()
        {
            Close();
        }

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            //SendKey(sender);
            string strKey = (sender as UserButton).Content as string;
            string strInput = lb_Input.Content as string;
            fn_Update(strInput, strKey);
        }

        private void fn_Update(string input, string key)
        {
            input += key;
            lb_Input.Content = input;
        }

        private void ub_Clear_Click(object sender, RoutedEventArgs e)
        {
            lb_Input.Content = "0.000";
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                sOperator = ub.Content as string;
                sPrevInput = lb_Input.Content as string;
            }
        }

        private void bn_Equal_Click(object sender, RoutedEventArgs e)
        {
            if(sPrevInput != "")
            {
                double dLeft = 0.0, dRight = 0.0, dResult = 0.0;
                double.TryParse(sPrevInput, out dLeft);
                double.TryParse(lb_Input.Content as string, out dRight);
                
                switch (sOperator)
                {
                    case "+":
                        dResult = dLeft + dRight;
                        break;
                    case "-":
                        dResult = dLeft - dRight;
                        break;
                    case "*":
                        dResult = dLeft * dRight;
                        break;
                    case "/":
                        dResult = dLeft / dRight;
                        break;
                }

                lb_InputResult.Content = dResult.ToString("0.000");
            }
        }

        private void bn_InDeCrease_Click(object sender, RoutedEventArgs e)
        {
            UserButton ub = sender as UserButton;
            if (ub != null)
            {
                double dValue = 0.0, dCurrent = 0.0;
                double.TryParse(ub.Content as string, out dValue);
                double.TryParse(lb_InputResult.Content as string, out dCurrent);
                lb_InputResult.Content = (dCurrent + dValue).ToString("0.00");
            }
        }

        private void InverseSign(object sender, RoutedEventArgs e)
        {
            double dCurrent = 0.0;
            double.TryParse(lb_InputResult.Content as string, out dCurrent);
            lb_InputResult.Content = (dCurrent * -1).ToString("0.00");
        }

        private void ub_Back_Click(object sender, RoutedEventArgs e)
        {
            string strValue = lb_Input.Content as string;
            string strReturn = "";
            int nLength = strValue.Length;
            if (nLength > 1)
                strReturn = strValue.Substring(0, strValue.Length - 1);
            else if (nLength == 1)
                strReturn = "0.000";
            lb_Input.Content = strReturn;
        }

        private void Dot_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
