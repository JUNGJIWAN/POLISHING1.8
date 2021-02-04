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
    /// UserControlTEset.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserCombo : UserControl
    {

        #region Content
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleProperty
           = DependencyProperty.Register(
                 "UCTitle",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("Title :")
             );

        public string UCTitle
        {
            get { return (string)GetValue(UCTitleProperty); }
            set { SetValue(UCTitleProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueProperty
           = DependencyProperty.Register(
                 "UCValue",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("0")
             );

        public string UCValue
        {
            get { return (string)GetValue(UCValueProperty); }
            set { SetValue(UCValueProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitProperty
           = DependencyProperty.Register(
                 "UCUnit",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("mm")
             );
        public string UCUnit
        {
            get { return (string)GetValue(UCUnitProperty); }
            set { SetValue(UCUnitProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCSelectedIndexProperty
            = DependencyProperty.Register(
                "UCSelectedIndex",
                typeof(int),
                typeof(UserControl),
                new PropertyMetadata(-1)
                );

        public int UCSelectedIndex
        {
            get { return (int)GetValue(UCSelectedIndexProperty); }
            set { SetValue(UCSelectedIndexProperty, value); }
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Layout
        //---------------------------------------------------------------------------
        // Width
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleWidthProperty
           = DependencyProperty.Register(
                 "UCTitleWidth",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("1*")
             );
        public string UCTitleWidth
        {
            get { return (string)GetValue(UCTitleWidthProperty); }
            set { SetValue(UCTitleWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueWidthProperty
           = DependencyProperty.Register(
                 "UCValueWidth",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("1*")
             );

        public string UCValueWidth
        {
            get { return (string)GetValue(UCValueWidthProperty); }
            set { SetValue(UCValueWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitWidthProperty
           = DependencyProperty.Register(
                 "UCUnitWidth",
                 typeof(string),
                 typeof(UserControl),
                 new PropertyMetadata("1*")
             );

        public string UCUnitWidth
        {
            get { return (string)GetValue(UCUnitWidthProperty); }
            set { SetValue(UCUnitWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        // Content Alignment
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleHorizontalAlignProperty
            = DependencyProperty.Register(
                "UCTitleHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Center)
                );

        public HorizontalAlignment UCTitleHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(UCTitleHorizontalAlignProperty); }
            set { SetValue(UCTitleHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleVerticalAlignProperty
            = DependencyProperty.Register(
                "UCTitleVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment UCTitleVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(UCTitleVerticalAlignProperty); }
            set { SetValue(UCTitleVerticalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueHorizontalAlignProperty
            = DependencyProperty.Register(
                "UCValueHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Center)
                );

        public HorizontalAlignment UCValueHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(UCValueHorizontalAlignProperty); }
            set { SetValue(UCValueHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueVerticalAlignProperty
            = DependencyProperty.Register(
                "UCValueVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment UCValueVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(UCValueVerticalAlignProperty); }
            set { SetValue(UCValueVerticalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitHorizontalAlignProperty
            = DependencyProperty.Register(
                "UCUnitHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Center)
                );

        public HorizontalAlignment UCUnitHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(UCUnitHorizontalAlignProperty); }
            set { SetValue(UCUnitHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitVerticalAlignProperty
            = DependencyProperty.Register(
                "UCUnitVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment UCUnitVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(UCUnitVerticalAlignProperty); }
            set { SetValue(UCUnitVerticalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        // Border Thickness
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleBorderThicknessProperty
            = DependencyProperty.Register(
                "UCTitleBorderThickness",
                typeof(Thickness),
                typeof(UserControl),
                new PropertyMetadata(new Thickness(0))
                );

        public Thickness UCTitleBorderThickness
        {
            get { return (Thickness)GetValue(UCTitleBorderThicknessProperty); }
            set { SetValue(UCTitleBorderThicknessProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueBorderThicknessProperty
            = DependencyProperty.Register(
                "UCValueBorderThickness",
                typeof(Thickness),
                typeof(UserControl),
                new PropertyMetadata(new Thickness(0))
                );

        public Thickness UCValueBorderThickness
        {
            get { return (Thickness)GetValue(UCValueBorderThicknessProperty); }
            set { SetValue(UCValueBorderThicknessProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitBorderThicknessProperty
            = DependencyProperty.Register(
                "UCUnitBorderThickness",
                typeof(Thickness),
                typeof(UserControl),
                new PropertyMetadata(new Thickness(0))
                );

        public Thickness UCUnitBorderThickness
        {
            get { return (Thickness)GetValue(UCUnitBorderThicknessProperty); }
            set { SetValue(UCUnitBorderThicknessProperty, value); }
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Color
        //---------------------------------------------------------------------------
        // Border Brush
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleBorderBrushProperty
            = DependencyProperty.Register(
                "UCTitleBorderBrush",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush UCTitleBorderBrush
        {
            get { return (Brush)GetValue(UCTitleBorderBrushProperty); }
            set { SetValue(UCTitleBorderBrushProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueBorderBrushProperty
            = DependencyProperty.Register(
                "UCValueBorderBrush",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush UCValueBorderBrush
        {
            get { return (Brush)GetValue(UCValueBorderBrushProperty); }
            set { SetValue(UCValueBorderBrushProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitBorderBrushProperty
            = DependencyProperty.Register(
                "UCUnitBorderBrush",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush UCUnitBorderBrush
        {
            get { return (Brush)GetValue(UCUnitBorderBrushProperty); }
            set { SetValue(UCUnitBorderBrushProperty, value); }
        }
        //---------------------------------------------------------------------------
        // BackGround
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleBackgroundProperty
            = DependencyProperty.Register(
                "UCTitleBackground",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush UCTitleBackground
        {
            get { return (Brush)GetValue(UCTitleBackgroundProperty); }
            set { SetValue(UCTitleBackgroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueBackgroundProperty
            = DependencyProperty.Register(
                "UCValueBackground",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush UCValueBackground
        {
            get { return (Brush)GetValue(UCValueBackgroundProperty); }
            set { SetValue(UCValueBackgroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitBackgroundProperty
            = DependencyProperty.Register(
                "UCUnitBackground",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(Brushes.Transparent)
                );

        public Brush UCUnitBackground
        {
            get { return (Brush)GetValue(UCUnitBackgroundProperty); }
            set { SetValue(UCUnitBackgroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        // Foreground
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCTitleForegroundProperty
           = DependencyProperty.Register(
               "UCTitleForeground",
               typeof(Brush),
               typeof(UserControl),
               new PropertyMetadata(Brushes.Black)
               );

        public Brush UCTitleForeground
        {
            get { return (Brush)GetValue(UCTitleForegroundProperty); }
            set { SetValue(UCTitleForegroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCValueForegroundProperty
           = DependencyProperty.Register(
               "UCValueForeground",
               typeof(Brush),
               typeof(UserControl),
               new PropertyMetadata(Brushes.Black)
               );

        public Brush UCValueForeground
        {
            get { return (Brush)GetValue(UCValueForegroundProperty); }
            set { SetValue(UCValueForegroundProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UCUnitForegroundProperty
           = DependencyProperty.Register(
               "UCUnitForeground",
               typeof(Brush),
               typeof(UserControl),
               new PropertyMetadata(Brushes.Black)
               );

        public Brush UCUnitForeground
        {
            get { return (Brush)GetValue(UCUnitForegroundProperty); }
            set { SetValue(UCUnitForegroundProperty, value); }
        }
        //---------------------------------------------------------------------------

        #endregion

        public UserCombo()
        {
            InitializeComponent();
        }

        public void Add(object obj)
        {
            uc_Combo.Items.Add(obj);
        }

        public void Clear()
        {
            uc_Combo.Items.Clear();
        }
    }
}
