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
using WaferPolishingSystem.Define;
using WaferPolishingSystem.Unit;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;


namespace WaferPolishingSystem.Form
{
    /// <summary>
    /// PageSetting_IO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageSetting_Manual_Loadcell : Page
    {
        //Var
        
        int m_nIndex = 0;

        //Timer
        private DispatcherTimer m_UpdateTimer = new DispatcherTimer();

        public PageSetting_Manual_Loadcell()
        {
            InitializeComponent();

            //Timer 생성
            m_UpdateTimer.IsEnabled = false;
            m_UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_UpdateTimer.Tick += new EventHandler(fn_tmUpdate);

            //Back Color Set
            this.Background = UserConst.G_COLOR_PAGEBACK;


        }
        //---------------------------------------------------------------------------
        //Update Timer
        private void fn_tmUpdate(object sender, EventArgs e)
        {
            //
            m_UpdateTimer.Stop();

            //Top
            //textSerialNo.Text            = "";//Convert.ToString(LDCBTM._sSerialNo       );
            //textFullScaleLoaded.Text     = "";//Convert.ToString(LDCBTM._dFullScaleLoaded);
            //textOffsetDefault.Text       = "";//Convert.ToString(LDCBTM._nOffsetDefault  );
            //textTareValue.Text           = "";//Convert.ToString(LDCBTM._dTareValue      );

            //Bottom
            //_textSerialNo.Text           = Convert.ToString(LDCBTM._sSerialNo       );
            //_textFullScaleLoaded.Text    = Convert.ToString(LDCBTM._dFullScaleLoaded);
            //_textOffsetDefault.Text      = Convert.ToString(LDCBTM._nOffsetDefault  );
            //_textTareValue.Text          = Convert.ToString(LDCBTM._dTareValue      );

            _textValue.Text              = Convert.ToString(LDCBTM._dLoadCellValue);
            _textNormalData.Text         = Convert.ToString(LDCBTM._nNormalData);
            _textCalculatedValue.Text    = Convert.ToString(LDCBTM._dCalculatedValue);
            //_textFullScaleLoaded.Text    = Convert.ToString(LDCBTM._dFullScaleLoaded);
            _textTareValue.Text          = Convert.ToString(LDCBTM._dTareValue);


            _textOffset.Text             = LDCBTM._nOffset.ToString();
            _textFullScaleValue.Text     = LDCBTM._nFullScaleValue.ToString();
            _textFullScaleLoadValue.Text = LDCBTM._nFullScaleLoadValue.ToString();
            _textDecimalPoint.Text       = LDCBTM._nDecimalPoint.ToString();
            _textUnitCode.Text           = LDCBTM._sUnit ;








            //
            m_UpdateTimer.Start();
        }
        //---------------------------------------------------------------------------
        //Timer On/Off
        public void fn_SetTimer(bool on)
        {
            if (on)
            {
                m_UpdateTimer.IsEnabled = true;
                m_UpdateTimer.Start();
            }
            else
                m_UpdateTimer.Stop();

        }
        //---------------------------------------------------------------------------
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = fn_IsNumeric(e.Text);
        }
        //---------------------------------------------------------------------------
        private void bnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (m_nIndex < 0) return;

            if (m_nIndex == 0) // TOP
            {

            }
            else if(m_nIndex == 1) // Bottom
            {
                //if (!LDCBTM.fn_Open(FM.m_stMasterOpt.sLoadCellSN)) return;
                if(LDCBTM.fn_GetSN() == "" || LDCBTM.fn_GetSN() == "Error")
                {
                    fn_UserMsg("[LOADCELL] Serial No Error!");
                    return; 
                }

                if (!LDCBTM.fn_Open())
                {
                    fn_UserMsg("[LOADCELL] Connection Error!");
                    return;

                }

                _textOffset.Text             = LDCBTM._nOffset.ToString();
                _textFullScaleValue.Text     = LDCBTM._nFullScaleValue.ToString();
                _textFullScaleLoadValue.Text = LDCBTM._nFullScaleLoadValue.ToString();
                _textDecimalPoint.Text       = LDCBTM._nDecimalPoint.ToString();
                _textUnitCode.Text           = LDCBTM._sUnit ;

            }
        }
        //---------------------------------------------------------------------------
        private void bnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (m_nIndex < 0) return;

            if (m_nIndex == 0)
            {

            }
            else if (m_nIndex == 1)
            {
                //
                LDCBTM.fn_Close();
            }
        }
        //---------------------------------------------------------------------------
        private void bnGetValue_Click(object sender, RoutedEventArgs e)
        {
            if (m_nIndex < 0) return;
            
            if(m_nIndex == 0) // Top
            {
                textValue.Text            = "";
                textNormalData.Text       = "";
                textCalculatedValue.Text  = "";
                textFullScaleLoaded.Text  = "";
                textTareValue.Text        = "";
            }
            else if(m_nIndex == 1) // Bottom
            {
                _textValue.Text           = Convert.ToString(LDCBTM._dLoadCellValue);
                _textNormalData.Text      = Convert.ToString(LDCBTM._nNormalData);
                _textCalculatedValue.Text = Convert.ToString(LDCBTM._dCalculatedValue);
                _textFullScaleLoaded.Text = Convert.ToString(LDCBTM._dFullScaleLoaded);
                _textTareValue.Text       = Convert.ToString(LDCBTM._dTareValue);
            }
        }
        //---------------------------------------------------------------------------
        private void bnSave_Click(object sender, RoutedEventArgs e)
        {
            if (m_nIndex < 0) return;
            
            if (m_nIndex == 0)
            {
            
            }
            else if (m_nIndex == 1)
            {
                LDCBTM._sSerialNo            = _textSerialNo.Text;
                //LDCBTM._nOffsetDefault       = Convert.ToInt32 (textOffsetDefault.Text  );
                //LDCBTM._dTareValue           = Convert.ToDouble(textTareValue.Text      );
                //LDCBTM._dFullScaleLoaded     = Convert.ToInt32 (textFullScaleLoaded.Text);

                int   .TryParse(_textOffsetDefault.Text  , out int    n1);
                double.TryParse(_textTareValue.Text      , out double d1);
                double.TryParse(_textFullScaleLoaded.Text, out double d2);

                LDCBTM._nOffsetDefault   = n1;
                LDCBTM._dTareValue       = d1;
                LDCBTM._dFullScaleLoaded = d2;


                FM.fn_LoadMastOptn(false);

            }

            
        }
        //---------------------------------------------------------------------------
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            cbTop   .IsChecked = false;
            cbBottom.IsChecked = true ;
            m_nIndex = 1;

            //Top
            textSerialNo.Text            = "";//Convert.ToString(LDCBTM._sSerialNo       );
            textFullScaleLoaded.Text     = "";//Convert.ToString(LDCBTM._dFullScaleLoaded);
            textOffsetDefault.Text       = "";//Convert.ToString(LDCBTM._nOffsetDefault  );
            textTareValue.Text           = "";//Convert.ToString(LDCBTM._dTareValue      );

            //Bottom
            _textSerialNo.Text           = Convert.ToString(LDCBTM._sSerialNo       );
            _textFullScaleLoaded.Text    = Convert.ToString(LDCBTM._dFullScaleLoaded);
            _textOffsetDefault.Text      = Convert.ToString(LDCBTM._nOffsetDefault  );
            _textTareValue.Text          = Convert.ToString(LDCBTM._dTareValue      );


            _textOffset.Text             = LDCBTM._nOffset.ToString();
            _textFullScaleValue.Text     = LDCBTM._nFullScaleValue.ToString();
            _textFullScaleLoadValue.Text = LDCBTM._nFullScaleLoadValue.ToString();
            _textDecimalPoint.Text       = LDCBTM._nDecimalPoint.ToString();
            _textUnitCode.Text           = LDCBTM._sUnit ;

            fn_SetTimer(true);


        }
        //---------------------------------------------------------------------------
        private void bnSetzero_Click(object sender, RoutedEventArgs e)
        {
            if (m_nIndex < 0) return;

            if (m_nIndex == 1) LDCBTM._dTareValue = 0.0; 
            //else if (m_nIndex == 1) //LOADCELL.fn_ResetTareValue(m_nIndex);
        }
        //---------------------------------------------------------------------------
        private void bnSetCalibration_Click(object sender, RoutedEventArgs e)
        {
            if (m_nIndex < 0) return;
            
            if (m_nIndex == 1) LDCBTM.fn_TareCalibration();
        }
        //---------------------------------------------------------------------------
        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            //if      (cbTop.IsChecked.Value   ) m_nIndex = 0;
            //else if (cbBottom.IsChecked.Value) m_nIndex = 1;
            //else if (cbBoth.IsChecked.Value  ) m_nIndex = 2;
            //else                               m_nIndex = -1;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            fn_SetTimer(false);
        }
        //---------------------------------------------------------------------------
        private void bnGetSerial_Click(object sender, RoutedEventArgs e)
        {
            _textSerialNo.Text = LDCBTM.fn_GetSN();
        }
    }
}

