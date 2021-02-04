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
    public partial class UserFloatCamSlider : UserControl
    {
        public UserFloatCamSlider()
        {
            InitializeComponent();
            Reset();
        }

        private IFloatParameter parameter = null; // The interface of the integer parameter.
        private string defaultName = "N/A";
        private double _dMax = 0.0;
        private double _dMin = 0.0;
        string Title = "UserFloatCamSlider";

        // Sets the parameter displayed by the user control.
        public IFloatParameter Parameter
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
                    tb_Value.Text = defaultName;
                }
            }
            get
            {
                return defaultName;
            }
        }


        // The parameter state changed. Update the control.
        private void ParameterChanged(object sender, ParameterChangedEventArgs e)
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
//             labelMin.Enabled = false;
//             labelMax.Enabled = false;
            lb_ValueName.IsEnabled = false;
            tb_Value.IsEnabled = false;
        }
        // Converts slider range to percent value.
        private int PercentToSliderValue(double percent)
        {
            return (int)(((_dMax) / 100.0) * percent);
        }

        // Converts percent value to slider range.
        private double SliderToPercentValue(double sliderValue)
        {
            return (sliderValue / (_dMax)) * 100.0;
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
                        _dMin = parameter.GetMinimum();
                        _dMax = parameter.GetMaximum();
                        double val = parameter.GetValue();
                        double percent = parameter.GetValuePercentOfRange();

                        // Update the slider.
                        slider.Minimum = PercentToSliderValue(0);
                        slider.Maximum = PercentToSliderValue(100);
                        slider.Value = PercentToSliderValue(percent);
                        slider.SmallChange = PercentToSliderValue(0.05);
                        slider.TickFrequency = PercentToSliderValue(10);

                        // Update the displayed values.
                        //                         labelMin.Text = "" + min;
                        //                         labelMax.Text = "" + max;
                        tb_Value.Text = "" + val;

                        // Update accessibility.
                        slider.IsEnabled = parameter.IsWritable;
//                         labelMin.Enabled = true;
//                         labelMax.Enabled = true;
                        lb_ValueName.IsEnabled = true;
                        tb_Value.IsEnabled = true;

                        return;
                    }
                }
            }
            catch
            {
                // If errors occurred disable the control.
                Reset();
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
                    parameter.SetValuePercentOfRange(SliderToPercentValue(slider.Value));
                }
                catch
                {
                    // Ignore any errors here.
                }
            }
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
            double dValue;
            if (parameter != null)
            {

                if (double.TryParse(tb_Value.Text, out dValue))
                {
                    
                    try
                    {
                        parameter.TrySetValue(dValue);

                    }
                    catch (Exception ex)
                    {
                        fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                    }
                }
            }
        }
    }
}
