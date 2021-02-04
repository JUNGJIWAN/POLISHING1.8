using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace WaferPolishingSystem
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
       {
            //JUNG/200110
            Process[] procs = Process.GetProcessesByName("WaferPolishingSystem");
            if (procs.Length > 1)
            {
                MessageBox.Show("[WaferPolishingSystem] 프로그램이 이미 실행되고 있습니다.\n기존 실행 프로그램을 작업관리자에서 확인해 주시기 바랍니다.", "ERROR");
                Application.Current.Shutdown();
                return;
            }
            

        }
    }
}
