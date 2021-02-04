using Basler.Pylon;
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
using System.Windows.Threading;

namespace UserInterface
{
    /// <summary>
    /// UserSlider.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserSlider : UserControl
    {
        #region Content
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleProperty
            = DependencyProperty.Register(
                  "USTitle",
                  typeof(string),
                  typeof(UserControl),
                  new PropertyMetadata("param")
              );

        public string USTitle
        {
            get { return (string)GetValue(USTitleProperty); }
            set { SetValue(USTitleProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueProperty
            = DependencyProperty.Register(
                  "USValue",
                  typeof(double),
                  typeof(UserControl),
                  new PropertyMetadata(0.0)
              );

        public double USValue
        {
            get 
            {
                return (double)GetValue(USValueProperty); 
            }
            set
            {
                SetValue(USValueProperty, value);
            }
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Layout
        //---------------------------------------------------------------------------
        // Width
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleWidthProperty
            = DependencyProperty.Register(
                  "USTitleWidth",
                  typeof(string),
                  typeof(UserControl),
                  new PropertyMetadata("1*")
              );

        public string USTitleWidth
        {
            get { return (string)GetValue(USTitleWidthProperty); }
            set { SetValue(USTitleWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USSliderWidthProperty
            = DependencyProperty.Register(
                  "USSliderWidth",
                  typeof(string),
                  typeof(UserControl),
                  new PropertyMetadata("1*")
              );

        public string USSliderWidth
        {
            get { return (string)GetValue(USSliderWidthProperty); }
            set { SetValue(USSliderWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueWidthProperty
            = DependencyProperty.Register(
                  "USValueWidth",
                  typeof(string),
                  typeof(UserControl),
                  new PropertyMetadata("1*")
              );

        public string USValueWidth
        {
            get { return (string)GetValue(USValueWidthProperty); }
            set { SetValue(USValueWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        // Content Alignment
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleHorizontalAlignProperty
            = DependencyProperty.Register(
                "USTitleHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Center)
                );

        public HorizontalAlignment USTitleHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(USTitleHorizontalAlignProperty); }
            set { SetValue(USTitleHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleVerticalAlignProperty
            = DependencyProperty.Register(
                "USTitleVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment USTitleVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(USTitleVerticalAlignProperty); }
            set { SetValue(USTitleVerticalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueHorizontalAlignProperty
            = DependencyProperty.Register(
                "USValueHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Center)
                );

        public HorizontalAlignment USValueHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(USValueHorizontalAlignProperty); }
            set { SetValue(USValueHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueVerticalAlignProperty
            = DependencyProperty.Register(
                "USValueVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment USValueVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(USValueVerticalAlignProperty); }
            set { SetValue(USValueVerticalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        // Border Thickness
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleBorderThicknessProperty
            = DependencyProperty.Register(
                "USTitleBorderThickness",
                typeof(Thickness),
                typeof(UserControl),
                new PropertyMetadata(new Thickness(0))
                );

        public Thickness USTitleBorderThickness
        {
            get { return (Thickness)GetValue(USTitleBorderThicknessProperty); }
            set { SetValue(USTitleBorderThicknessProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueBorderThicknessProperty
            = DependencyProperty.Register(
                "USValueBorderThickness",
                typeof(Thickness),
                typeof(UserControl),
                new PropertyMetadata(new Thickness(1))
                );

        public Thickness USValueBorderThickness
        {
            get { return (Thickness)GetValue(USValueBorderThicknessProperty); }
            set { SetValue(USValueBorderThicknessProperty, value); }
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Color
        //---------------------------------------------------------------------------
        // Border Brush
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleBorderBrushProperty
            = DependencyProperty.Register(
                "USTitleBorderBrush",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush USTitleBorderBrush
        {
            get { return (Brush)GetValue(USTitleBorderBrushProperty); }
            set { SetValue(USTitleBorderBrushProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueBorderBrushProperty
            = DependencyProperty.Register(
                "USValueBorderBrush",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Black)
                );

        public Brush USValueBorderBrush
        {
            get { return (Brush)GetValue(USValueBorderBrushProperty); }
            set { SetValue(USValueBorderBrushProperty, value); }
        }
        //---------------------------------------------------------------------------
        // BackGround
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleBackgroundProperty
            = DependencyProperty.Register(
                "USTitleBackground",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush USTitleBackground
        {
            get { return (Brush)GetValue(USTitleBackgroundProperty); }
            set { SetValue(USTitleBackgroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueBackgroundProperty
            = DependencyProperty.Register(
                "USValueBackground",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.White)
                );

        public Brush USValueBackground
        {
            get { return (Brush)GetValue(USValueBackgroundProperty); }
            set { SetValue(USValueBackgroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        // Foreground
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTitleForegroundProperty
           = DependencyProperty.Register(
               "USTitleForeground",
               typeof(Brush),
               typeof(UserControl),
               new PropertyMetadata(Brushes.Black)
               );

        public Brush USTitleForeground
        {
            get { return (Brush)GetValue(USTitleForegroundProperty); }
            set { SetValue(USTitleForegroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USValueForegroundProperty
           = DependencyProperty.Register(
               "USValueForeground",
               typeof(Brush),
               typeof(UserControl),
               new PropertyMetadata(Brushes.Black)
               );

        public Brush USValueForeground
        {
            get { return (Brush)GetValue(USValueForegroundProperty); }
            set { SetValue(USValueForegroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USUnitForegroundProperty
           = DependencyProperty.Register(
               "USUnitForeground",
               typeof(Brush),
               typeof(UserControl),
               new PropertyMetadata(Brushes.Black)
               );

        public Brush USUnitForeground
        {
            get { return (Brush)GetValue(USUnitForegroundProperty); }
            set { SetValue(USUnitForegroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Data
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USMaximumProperty
            = DependencyProperty.Register(
                  "USMaximum",
                  typeof(double),
                  typeof(UserControl),
                  new PropertyMetadata(0.0)
              );

        public double USMaximum
        {
            get { return (double)GetValue(USMaximumProperty); }
            set { SetValue(USMaximumProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USMinimumProperty
            = DependencyProperty.Register(
                  "USMinimum",
                  typeof(double),
                  typeof(UserControl),
                  new PropertyMetadata(0.0)
              );

        public double USMinimum
        {
            get { return (double)GetValue(USMinimumProperty); }
            set { SetValue(USMinimumProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty USTickFrequencyProperty
            = DependencyProperty.Register(
                  "USTickFrequency",
                  typeof(double),
                  typeof(UserControl),
                  new PropertyMetadata(1.0)
              );

        public double USTickFrequency
        {
            get { return (double)GetValue(USTickFrequencyProperty); }
            set { SetValue(USTickFrequencyProperty, value); }
        }
        //---------------------------------------------------------------------------
        #endregion

        public UserSlider()
        {
            InitializeComponent();
        }
    }
}
