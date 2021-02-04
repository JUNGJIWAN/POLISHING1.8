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
using static WaferPolishingSystem.Define.UserClass;

namespace WaferPolishingSystem.Define
{
    /// <summary>
    /// VisionControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VisionControl : Window
    {
        public VisionControl()
        {
            InitializeComponent();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            g_VisionManager._LightManger.Serial_Conn("COM5", 0);
            g_VisionManager._LightManger.GetValue(1);
            
            us_Light1.USValue = g_VisionManager._LightManger.Bright;

            g_VisionManager._LightManger.GetValue(2);

            us_Light2.USValue = g_VisionManager._LightManger.Bright;
        }

        private void us_Light_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            g_VisionManager._LightManger.Channel = 1;
            g_VisionManager._LightManger.Bright = (int)us_Light1.USValue;
        }

        private void us_Light_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                g_VisionManager._LightManger.Channel = 1;
                g_VisionManager._LightManger.Bright = (int)us_Light1.USValue;
            }
        }

        private void UserButton2_Click(object sender, RoutedEventArgs e)
        {
            g_VisionManager._LightManger.Serial_DisConn();
        }

        private void us_Light2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            g_VisionManager._LightManger.Channel = 2;
            g_VisionManager._LightManger.Bright = (int)us_Light2.USValue;
        }

        private void us_Light2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                g_VisionManager._LightManger.Channel = 2;
                g_VisionManager._LightManger.Bright = (int)us_Light2.USValue;
            }
        }

        private void bn_Hide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
