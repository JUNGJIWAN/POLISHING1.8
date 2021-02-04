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
using WaferPolishingSystem.Vision;
using WaferPolishingSystem.Define;

namespace WaferPolishingSystem
{
    public class ChangeResource : IPropertyChanged
    {
        private string strTitle = "";
        public string Title
        {
            get { return strTitle; }
            set
            {
                strTitle = value;
                OnPropertyChanged("Title");
            }
        }

        private string strMessage = "";
        public string Message
        {
            get { return strMessage; }
            set
            {
                strMessage = value;
                OnPropertyChanged("Message");
            }
        }
        private List<string> listchange = new List<string>();
        public List<string> ListChange
        {
            get { return listchange; }
            set
            {
                listchange = value;
                OnPropertyChanged("ListChange");
            }
        }
    }
    /// <summary>
    /// FormMessage_Change.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormMessage_Change : Window
    {
        public ChangeResource chagneResource = new ChangeResource();
        public FormMessage_Change()
        {
            InitializeComponent();
            this.DataContext = chagneResource;
        }

        static public bool ShowChange(string strMsg, string strTitle = "Message", List<string> listChange = null, UserEnumVision.EN_MESSAGETYPE msgtype = UserEnumVision.EN_MESSAGETYPE.MESSAGE )
        {
            FormMessage_Change change = new FormMessage_Change();

            switch(msgtype)
            {
                case UserEnumVision.EN_MESSAGETYPE.MESSAGE:
                    change.Background = Brushes.LightGray;
                    change.grid_Title.Background = Brushes.Gray;
                    break;
                case UserEnumVision.EN_MESSAGETYPE.WARNNING:
                    change.Background = Brushes.Khaki;
                    change.grid_Title.Background = Brushes.DarkKhaki;
                    break;
                case UserEnumVision.EN_MESSAGETYPE.ERROR:
                    change.Background = Brushes.Red;
                    change.grid_Title.Background = Brushes.DarkRed;
                    break;
            }

            change.chagneResource.Title = strTitle;
            change.chagneResource.Message = strMsg;
            change.chagneResource.ListChange = listChange;

            change.ShowDialog();
            bool bRtn = change.DialogResult == true ? true : false;
            return bRtn;
        }

        private void bn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void bn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
