using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using UserInterface;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain; 

namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageMaster.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageLog : Page
    {
        //
        string     _strLogType   ;
        string     m_sSelFileName;
        string     m_sSelFilePath;

        UserButton _prevButton   ;
        bool m_bInit = false;

        private bool Key_LeftCtrl;

        public PageLog()
        {
            InitializeComponent();
            System.Windows.Interop.ComponentDispatcher.ThreadIdle += MenuInit;

            //Back Color Set
            this.Background         = UserConst.G_COLOR_PAGEBACK;
            this.GridSub.Background = UserConst.G_COLOR_SUBMENU ;

            m_sSelFileName = string.Empty;
            m_sSelFilePath = string.Empty;

            Key_LeftCtrl = false; 
        }
        //---------------------------------------------------------------------------
        private void m_fn_SetPage(UserInterface.UserButton bn)
        {
            string strName = bn.Content as string;
            string sPath;
            sPath = UserFile.fn_GetExePath();
            _strLogType = "LOG\\";
           

            if (_prevButton != null) _prevButton.Background = G_COLOR_BTNNORMAL;
            
            bn.Background = G_COLOR_BTNCLICKED;

            if      (strName == "EVENT"     ) _strLogType += "EVENT"   ;
            else if (strName == "ALARM"     ) _strLogType += "ALARM"   ;
            else if (strName == "TRANSFER"  ) _strLogType += "TRANSFER";
            else if (strName == "PMC"       ) _strLogType += "PMC"     ;
            else if (strName == "LOT"       ) _strLogType += "LOT"     ;
            else if (strName == "VISION"    ) _strLogType += "VISION"  ;
            else if (strName == "TRACE"     ) _strLogType += "TRACE"   ;
            
            _strLogType += "\\";
            string[] strPaths = UserFile.fn_GetFileList(sPath + _strLogType);
            
            lvLogList.ItemsSource = strPaths;
            
            lbLogInfo.Items.Clear();
            
            _prevButton = bn;

            fn_LogItemDisplay(0);

            //for csv
            m_sSelFilePath = sPath + _strLogType;
        }
        //---------------------------------------------------------------------------
        private void MenuInit(object sender, EventArgs e)
        {
            System.Windows.Interop.ComponentDispatcher.ThreadIdle -= MenuInit;
            //m_fn_SetPage(bn_Event);
        }
        //---------------------------------------------------------------------------
        private void MenuChange(object sender, RoutedEventArgs e)
        {
            UserInterface.UserButton menubn = (sender as UserInterface.UserButton);
            m_fn_SetPage(menubn);
        }
        //---------------------------------------------------------------------------
        private void dtLogList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Log Item Selected
            int n = lvLogList.SelectedIndex;
            
            if (n < 0) return;

            fn_LogItemDisplay(n);

            /*
            string sLogName = lvLogList.SelectedItem as string;
            string strLog = "";
            
            lbLogInfo.Items.Clear();

            //이름으로 파일 찾기
            try
            {
                StreamReader sr = new StreamReader(UserFile.fn_GetExePath() + _strLogType + sLogName);
                while ((strLog = sr.ReadLine()) != null) 
                {
                    lbLogInfo.Items.Add(strLog);
                }
            }
            catch (System.Exception ex)
            {
            	
            }
            */
        }
        //---------------------------------------------------------------------------
        private void fn_LogItemDisplay(int nlist)
        {
            if (nlist < 0) return;

            //Log Item Selected
            lvLogList.SelectedIndex = nlist;

            string sLogName = lvLogList.SelectedItem as string;
            string strLog = "";

            lbLogInfo.Items.Clear();

            //이름으로 파일 찾기
            try
            {
                StreamReader sr = new StreamReader(UserFile.fn_GetExePath() + _strLogType + sLogName);
                while ((strLog = sr.ReadLine()) != null)
                {
                    lbLogInfo.Items.Add(strLog);
                }
                //LEE/201127 [Add] : Scroll 하단으로 변경
                lbLogInfo.ScrollIntoView(lbLogInfo.Items[lbLogInfo.Items.Count - 1]);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            //Selected File Name
            m_sSelFileName = sLogName;


        }

        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!m_bInit)
            {
                m_fn_SetPage(bn_Trace); //Loading시
                
                m_bInit = true;
            }

            bn_FileOpen.IsEnabled = SEQ._bRun ? false : true; 
            bn_CSVFile .IsEnabled = SEQ._bRun ? false : true;

        }
        //---------------------------------------------------------------------------
        private void bn_CSVFile_Click(object sender, RoutedEventArgs e)
        {
            //
            if (SEQ._bRun) return; 

            //CSV File로 변환...
            if (m_sSelFileName == string.Empty) return ;
            if (m_sSelFilePath == string.Empty) return ;

            LOG.fn_SaveasCSV(m_sSelFilePath, m_sSelFileName);

        }
        //---------------------------------------------------------------------------
        private void bn_FileOpen_Click(object sender, RoutedEventArgs e)
        {
            //
            if (SEQ._bRun) return; 

            //CSV File로 변환...
            if (m_sSelFileName == string.Empty) return ;
            if (m_sSelFilePath == string.Empty) return ;

            try
            {
                if (Key_LeftCtrl)
                {
                    Process.Start(m_sSelFilePath);
                    Key_LeftCtrl = false; 
                }
                else
                {
                    Process.Start(m_sSelFilePath + m_sSelFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        //---------------------------------------------------------------------------
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl) Key_LeftCtrl = true ;
            else                       Key_LeftCtrl = false;

        }
        //---------------------------------------------------------------------------
        private void Page_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key_LeftCtrl = false;
        }
    }
}
