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
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.FormMain;


namespace WaferPolishingSystem
{
    /// <summary>
    /// FormMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormUpdateList : Window
    {
        public FormUpdateList()
        {
            InitializeComponent();
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Window Show
        </summary>
        <param name="msg"> Display Message </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 10:08
        */
        public void fn_ShowInfo()
        {
            if (this.IsVisible) return ;

            //Modal
            this.ShowDialog();
        }
        //---------------------------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Ver
            lbVer.Content = FM._sVersion;

            lbUpdate.Items.Clear();
            
            //Display Update Info
            int n = FM._nHisCnt;
            for (int i = 0; i < n; i++)
            {
                lbUpdate.Items.Add(FM[i]);
            }
        }
        //---------------------------------------------------------------------------
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            //
            this.Hide();
        }
    }
}
