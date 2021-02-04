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

namespace UserInterface
{
    /// <summary>
    /// UserGraph.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserGraph : UserControl
    {
        #region Content
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UGTitleXProperty
           = DependencyProperty.Register(
                 "UGTitleX",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("x")
             );

        public string UGTitleX
        {
            get { return (string)GetValue(UGTitleXProperty); }
            set { SetValue(UGTitleXProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UGTitleYProperty
           = DependencyProperty.Register(
                 "UGTitleY",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("y")
             );

        public string UGTitleY
        {
            get { return (string)GetValue(UGTitleYProperty); }
            set { SetValue(UGTitleYProperty, value); }
        }

        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UGTitleZeroProperty
           = DependencyProperty.Register(
                 "UGTitleZero",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("y")
             );

        public string UGTitleZero
        {
            get { return (string)GetValue(UGTitleZeroProperty); }
            set { SetValue(UGTitleZeroProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UGTitleProperty
           = DependencyProperty.Register(
                 "UGTitle",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("y")
             );

        public string UGTitle
        {
            get { return (string)GetValue(UGTitleProperty); }
            set { SetValue(UGTitleProperty, value); }
        }
        #endregion

        public static readonly DependencyProperty UGMaxValueProperty
            = DependencyProperty.Register(
                 "UGMaxValue",
                 typeof(int),
                 typeof(UserControl),
                 new PropertyMetadata(10)
                );
        public int UGMaxValue
        {
            get { return (int)GetValue(UGMaxValueProperty); }
            set { SetValue(UGMaxValueProperty, value); }
        }

        public static readonly DependencyProperty UGMinValueProperty
           = DependencyProperty.Register(
                "UGMinValue",
                typeof(int),
                typeof(UserControl),
                new PropertyMetadata(0)
               );
        public int UGMinValue
        {
            get { return (int)GetValue(UGMinValueProperty); }
            set { SetValue(UGMinValueProperty, value); }
        }

        public static readonly DependencyProperty UGDataCountProperty
           = DependencyProperty.Register(
                "UGDataCount",
                typeof(int),
                typeof(UserControl),
                new PropertyMetadata(100)
               );

        public int UGDataCount
        {
            get { return (int)GetValue(UGDataCountProperty); }
            set { SetValue(UGDataCountProperty, value); }
        }

        public static readonly DependencyProperty UGLineColorProperty
           = DependencyProperty.Register(
                "UGLineColor",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Gray)
               );

        public Brush UGLineColor
        {
            get { return (Brush)GetValue(UGLineColorProperty); }
            set { SetValue(UGLineColorProperty, value); }
        }

        public static readonly DependencyProperty UGValueProperty
            = DependencyProperty.Register(
                "UGValue",
                typeof(string),
                typeof(UserControl),
                new PropertyMetadata("0")
                );
        public string UGValue
        {
            get { return (string)GetValue(UGValueProperty); }
            set { SetValue(UGValueProperty, value); }
        }

        double dMaxValue = 0.0;

        public UserGraph()
        {
            InitializeComponent();
        }

        public void AddLine()
        {
            Polyline pl = new Polyline();
            pl.Stroke = UGLineColor;
            pl.StrokeThickness = 1;
            Plotter.Children.Add(pl);
        }

        public void AddPoint(Point pnt, int idx = 0)
        {
            if(Plotter.Children.Count > 0)
            {
                Polyline polyline = Plotter.Children[idx] as Polyline;
                Point pnt2;
                if(polyline != null)
                {

                    pnt.Y = Plotter.ActualHeight - ((pnt.Y - UGMinValue) * (Plotter.ActualHeight / (double)(UGMaxValue - UGMinValue)));
                    //if( dMaxValue < pnt.Y)
                    //{
                        //dMaxValue = pnt.Y;
                        if((pnt.Y * 1.3 > UGMaxValue) && (pnt.Y * 1.3 < 20)) //JUNG/201118
                        {
                            UGMaxValue = (int)(pnt.Y * 1.3 + 0.5);
                        }
                    //}
                    polyline.Points.Add(pnt);
                    for (int i = 0; i < polyline.Points.Count; i++)
                    {
                        pnt2 = polyline.Points[i];
                        pnt2.X = i * Plotter.ActualWidth / UGDataCount;
                        polyline.Points.RemoveAt(i);
                        polyline.Points.Insert(i,pnt2);
                    }
                    //UGTitle = pnt.Y.ToString("0.00");
                }
            }
        }

        public void RemoveFirst()
        {
            for (int i = 0; i < Plotter.Children.Count; i++)
            {
                Polyline polyline = Plotter.Children[i] as Polyline;
                polyline.Points.RemoveAt(0);
            }   
        }

        private void Plotter_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {

            for (int i = 0; i < Plotter.Children.Count; i++)
            {
                Polyline polyline = Plotter.Children[i] as Polyline;
                if(polyline.Width != Plotter.ActualWidth)
                    polyline.Width = Plotter.ActualWidth;
            }
        }
    }
}
