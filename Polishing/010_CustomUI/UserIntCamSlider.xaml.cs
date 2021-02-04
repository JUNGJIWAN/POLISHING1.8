using Basler.Pylon;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserFunction;

namespace UserInterface
{
    /// <summary>
    /// UserIntCamSlider.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserIntCamSlider : UserControl
    {
        #region Properties

        #region LayOut
        
        public static readonly DependencyProperty UISTitleWidthProperty
            = DependencyProperty.Register(
                  "UISTitleWidth",
                  typeof(string),
                  typeof(UserControl),
                  new PropertyMetadata("1*")
              );

        public string UISTitleWidth
        {
            get { return (string)GetValue(UISTitleWidthProperty); }
            set { SetValue(UISTitleWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISSliderWidthProperty
            = DependencyProperty.Register(
                  "UISSliderWidth",
                  typeof(string),
                  typeof(UserControl),
                  new PropertyMetadata("1*")
              );

        public string UISSliderWidth
        {
            get { return (string)GetValue(UISSliderWidthProperty); }
            set { SetValue(UISSliderWidthProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISValueWidthProperty
            = DependencyProperty.Register(
                  "UISValueWidth",
                  typeof(string),
                  typeof(UserControl),
                  new PropertyMetadata("40")
              );

        public string UISValueWidth
        {
            get { return (string)GetValue(UISValueWidthProperty); }
            set { SetValue(UISValueWidthProperty, value); }
        }
        #endregion

        #region Alignment
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISTitleHorizontalAlignProperty
            = DependencyProperty.Register(
                "UISTitleHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Left)
                );

        public HorizontalAlignment UISTitleHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(UISTitleHorizontalAlignProperty); }
            set { SetValue(UISTitleHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISTitleVerticalAlignProperty
            = DependencyProperty.Register(
                "UISTitleVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment UISTitleVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(UISTitleVerticalAlignProperty); }
            set { SetValue(UISTitleVerticalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISValueHorizontalAlignProperty
            = DependencyProperty.Register(
                "UISValueHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Center)
                );

        public HorizontalAlignment UISValueHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(UISValueHorizontalAlignProperty); }
            set { SetValue(UISValueHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISValueVerticalAlignProperty
            = DependencyProperty.Register(
                "UISValueVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment UISValueVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(UISValueVerticalAlignProperty); }
            set { SetValue(UISValueVerticalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISSliderHorizontalAlignProperty
            = DependencyProperty.Register(
                "UISSliderHorizontalAlign",
                typeof(HorizontalAlignment),
                typeof(UserControl),
                new PropertyMetadata(HorizontalAlignment.Left)
                );

        public HorizontalAlignment UISSliderHorizontalAlign
        {
            get { return (HorizontalAlignment)GetValue(UISSliderHorizontalAlignProperty); }
            set { SetValue(UISSliderHorizontalAlignProperty, value); }
        }
        //---------------------------------------------------------------------------
        public static readonly DependencyProperty UISSliderVerticalAlignProperty
            = DependencyProperty.Register(
                "UISSliderVerticalAlign",
                typeof(VerticalAlignment),
                typeof(UserControl),
                new PropertyMetadata(VerticalAlignment.Center)
                );

        public VerticalAlignment UISSliderVerticalAlign
        {
            get { return (VerticalAlignment)GetValue(UISSliderVerticalAlignProperty); }
            set { SetValue(UISSliderVerticalAlignProperty, value); }
        }
        #endregion
        //---------------------------------------------------------------------------
        #endregion

        string Title = "UserIntCamSlider";
        public UserIntCamSlider()
        {
            InitializeComponent();

            Reset();
        }

        private IIntegerParameter parameter = null; // The interface of the integer parameter.
        private string defaultName = "N/A";


        // Sets the parameter displayed by the user control.
        public IIntegerParameter Parameter
        {
            set
            {
                // Remove the old parameter.
                if (parameter != null)
                {
                    parameter.ParameterChanged -= ParameterChanged;
                }

                // Set the new parameter.
                parameter = value;
                if (parameter != null)
                {
                    parameter.ParameterChanged += ParameterChanged;
                    lb_ValueName.Content = parameter.Advanced.GetPropertyOrDefault(AdvancedParameterAccessKey.DisplayName, parameter.Name);
                    UpdateValues();
                }
                else
                {
                    lb_ValueName.Content = defaultName;
                    Reset();
                }
            }
        }


        // Sets the default name of the control.
        public string DefaultName
        {
            set
            {
                defaultName = value;
                if (parameter == null)
                {
                    lb_ValueName.Content = defaultName;
                }
            }
            get
            {
                return defaultName;
            }
        }


        // The parameter state changed. Update the control.
        private void ParameterChanged(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<ParameterChangedEventArgs>(ParameterChanged), sender, e);
                return;
            }
            try
            {
                UpdateValues();
            }
            catch
            {
                // If errors occurred disable the control.
                Reset();
            }
        }


        // Deactivate the control.
        private void Reset()
        {
            slider.IsEnabled = false;
            lb_ValueName.IsEnabled = false;
            tb_Value.IsEnabled = false;
        }


        // Get the current values from the parameter and display them.
        private void UpdateValues()
        {
            try
            {
                if (parameter != null)
                {
                    if (parameter.IsReadable)  // Check if parameter is accessible.
                    {
                        // Get values.
                        int min = checked((int)parameter.GetMinimum());
                        int max = checked((int)parameter.GetMaximum());
                        int val = checked((int)parameter.GetValue());
                        int inc = checked((int)parameter.GetIncrement());

                        // Update the slider.
                        slider.Minimum = min;
                        slider.Maximum = max > 20000 ? 20000 : max;
                        slider.Value = val;
                        //slider.SmallChange = inc;
                        //slider.TickFrequency = (max - min + 5) / 10.0;
                        slider.SmallChange = 1;
                        slider.TickFrequency = 10.0;

                        // Update the displayed values.
                        tb_Value.Text = "" + val;

                        // Update accessibility.
                        slider.IsEnabled = parameter.IsWritable;
                        lb_ValueName.IsEnabled = true;
                        tb_Value.IsEnabled = true;

                        return;
                    }
                }
            }
            catch
            {
                // If errors occurred disable the control.
            }
            Reset();
        }



        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                UpdateToSlider();
            }
        }

        private void UpdateToSlider()
        {
            int nValue;
            if (parameter != null)
            {

                if (int.TryParse(tb_Value.Text, out nValue))
                {
                    try
                    {
                        parameter.TrySetValue(nValue);

                    }
                    catch (Exception ex)
                    {
                        fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                    }
                }
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (parameter != null)
            {
                try
                {
                    // Break any recursion if the value does not exactly match the slider value.
                    //sliderMoving = true;

                    // Set the value.
                    parameter.TrySetValue((int)slider.Value, IntegerValueCorrection.Nearest);
                }
                catch
                {
                    // Ignore any errors here.
                }
            }
        }
    }

}

