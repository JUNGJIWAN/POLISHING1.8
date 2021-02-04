using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.FormMain;

namespace WaferPolishingSystem.Unit
{
    enum EN_LOADCELL
    {
        BTM = 0,
        TOP, //Monitoring
    }

    public struct ST_Loadcell
    {
        public FUTEK_USB_DLL.USB_DLL _Loadcell;
        
        public IntPtr  m_ptrHandle;
        public string  m_strSerialNo;
        public string  m_strTemp;
        public int     m_nOffsetDefault;
        public int     m_nOffset;
        public int     m_nFullScaleValue;
        public int     m_nFullScaleLoadValue;
        public int     m_nDecimalPoint;
        public int     m_nUnitCode;
        public string  m_strUnitCode;
        public int     m_nNormalData;
        public bool    m_bConnect;
        public double  m_dTareValue;
        public double  m_dCalculatedValue;
        public double  m_dFullScaleLoaded;

        public ST_Loadcell(bool con)
        {
            this._Loadcell             = new FUTEK_USB_DLL.USB_DLL();
            this.m_ptrHandle           = new IntPtr();

            this.m_bConnect            = con;
            this.m_strSerialNo         = string.Empty;
            this.m_strTemp             = string.Empty;
            this.m_nOffsetDefault      = 0;
            this.m_nOffset             = 0;
            this.m_nFullScaleValue     = 0;
            this.m_nFullScaleLoadValue = 0;
            this.m_nDecimalPoint       = 0;
            this.m_nUnitCode           = 5;
            this.m_strUnitCode         = string.Empty;
            this.m_nNormalData         = 0;
            
            this.m_dTareValue          = 0.0;
            this.m_dCalculatedValue    = 0.0;
            this.m_dFullScaleLoaded    = 0.0;

        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public string GetSerialNo(int id)
        {
            return this._Loadcell.Get_Device_Serial_Number(id.ToString());
        }
    }
    //------------------------------------------------------------------------------------------------
    public class FutekLoadcell
    {

        //Local Var.
        private double      m_dLoadCellValue  ; //Data
        private ST_Loadcell st_Loadcell;
        bool    m_bOn ;

        //---------------------------------------------------------------------------
        //Property
        public bool _bOn
        {
            get { return m_bOn; }
            set { m_bOn = value; }
        }
        public double _dLoadCellValue
        {
            get { return m_dLoadCellValue; }
        }
        public bool _bConect
        {
            get { return fn_IsConnected(); }
        }
        public string _sUnit
        {
            get { return st_Loadcell.m_strUnitCode;  } 
        }
        public string _sSerialNo
        {
            get { return st_Loadcell.m_strSerialNo; }
            set { fn_SetSerialNo(value); }
        }
        public int _nOffset
        {
            get { return st_Loadcell.m_nOffset; }
            //set { fn_SetOffset(value); }
        }
        public int _nNormalData
        {
            get { return st_Loadcell.m_nNormalData; }
        }

        public double _dFullScaleLoaded
        {
            get { return st_Loadcell.m_dFullScaleLoaded; }
            set { fn_SetFullScaleLoaded(value); }
        }
        public double _dCalculatedValue
        {
            get { return st_Loadcell.m_dCalculatedValue; }
        }
        public int _nOffsetDefault
        {
            get { return st_Loadcell.m_nOffsetDefault; }
            set { fn_SetOffDefault(value); }
        }
        public double _dTareValue
        {
            get { return st_Loadcell.m_dTareValue; }
            set { fn_SetTareValue(value); }
        }
        public int _nFullScaleValue
        {
            get { return st_Loadcell.m_nFullScaleValue; }
            //set { fn_SetFullScale(value); }
        }
        public int _nDecimalPoint
        {
            get { return st_Loadcell.m_nDecimalPoint; }
            //set { fn_SetDecPoint(value); }
        }

        public int _nFullScaleLoadValue
        {
            get { return st_Loadcell.m_nFullScaleLoadValue; }
            //set { fn_SetFullScaleLoaded(value); }
        }
        
        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public FutekLoadcell()
        {
            //1.5 Data
            //"841888", null, 8389960, 0, 0, 0, 0, 0, "", 0, false, -9.21936819266264, 0.0, 10.7 
            //"834592", null, 8373643, 0, 0, 0, 0, 0, "", 0, false, 10.871366211572395, 0.0, 2.34

            st_Loadcell = new ST_Loadcell(false);

            //
            m_bOn = false;
            m_dLoadCellValue = 0.0;

        }
        //---------------------------------------------------------------------------
        public void fn_SetParam()
        {
            //"834592", null, 8373643, 0, 0, 0, 0, 0, "", 0, false, 10.871366211572395, 0.0, 2.34
            st_Loadcell.m_nOffsetDefault   = FormMain.FM.m_stMasterOpt.nOffsetDefault  ;
            st_Loadcell.m_dTareValue       = FormMain.FM.m_stMasterOpt.dTareValue      ;
            st_Loadcell.m_dFullScaleLoaded = FormMain.FM.m_stMasterOpt.dFullScaleLoaded;

            fn_GetSN();
        }
        //---------------------------------------------------------------------------
        public void fn_SetParam(int nOffsetDefault, double dTareValue, double dScaleLoaded)
        {
            //"834592", null, 8373643, 0, 0, 0, 0, 0, "", 0, false, 10.871366211572395, 0.0, 2.34
            //FM.m_stMasterOpt.nOffsetDefault, FM.m_stMasterOpt.dTareValue, 

            st_Loadcell.m_nOffsetDefault   = nOffsetDefault  ;
            st_Loadcell.m_dTareValue       = dTareValue      ;
            st_Loadcell.m_dFullScaleLoaded = dScaleLoaded    ;
        }
        //---------------------------------------------------------------------------
        public bool fn_Reset()
        {
            if (fn_IsConnected()) return true;

            return fn_Open();
        }
        //---------------------------------------------------------------------------
        public bool fn_Open()
        {
            //return fn_Open(FormMain.FM.m_stMasterOpt.sLoadCellSN);
            return fn_Open(_sSerialNo);
        }
        //---------------------------------------------------------------------------
        public bool fn_Open(string sn)
        {
            fn_SetSerialNo(sn);

            //fn_WriteLog(string.Format($"Load Cell Open - SN : {sn}"));

            if (fn_OpenDevice())  return true;
            else                  return false;
        }
        //---------------------------------------------------------------------------
        private bool fn_IsConnected()
        {
            return st_Loadcell.m_bConnect;
        }
        //---------------------------------------------------------------------------
        private void fn_SetSerialNo(string sn)
        {
            st_Loadcell.m_strSerialNo = sn; 
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Open Device
        </summary>
        <param name="loadcell"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/18 15:02
        */
        private bool fn_OpenDevice()
        {
            if (fn_IsConnected()) return true;
            
            if (st_Loadcell.m_strSerialNo == null        ) return false;
            if (st_Loadcell.m_strSerialNo == string.Empty) return false;

            try
            {
                st_Loadcell._Loadcell.Open_Device_Connection(st_Loadcell.m_strSerialNo);

                if (st_Loadcell._Loadcell.DeviceStatus != 0) return false ;
                else
                {
                    st_Loadcell.m_ptrHandle = st_Loadcell._Loadcell.DeviceHandle;

                    if (!fn_GetOffsetValue   (ref st_Loadcell))  return false;
                    if (!fn_GetFullScaleValue(ref st_Loadcell))  return false;
                    if (!fn_GetFullScaleLoad (ref st_Loadcell))  return false;
                    if (!fn_GetDecimalPoint  (ref st_Loadcell))  return false;
                    //if (!fn_GetUnitCode      (ref st_Loadcell))  return false;

                    fn_FindUnits             (ref st_Loadcell);

                    st_Loadcell.m_bConnect = true;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        //---------------------------------------------------------------------------
        private void fn_SetOffset(int offset)
        {
            st_Loadcell.m_nOffset = offset;
        }
        //---------------------------------------------------------------------------
        private void fn_SetOffDefault(int offset)
        {
            st_Loadcell.m_nOffsetDefault = offset;
        }
        //---------------------------------------------------------------------------
        private void fn_SetTareValue(double tarevalue)
        {
            st_Loadcell.m_dTareValue = tarevalue;
        }
        //---------------------------------------------------------------------------
        public void fn_SetZero()
        {
            fn_TareCalibration();
            fn_WriteLog("Load Cell Set Zero");
        }
        //---------------------------------------------------------------------------
        private void fn_SetFullScale(int fullvalue)
        {
            st_Loadcell.m_nFullScaleValue = fullvalue;
        }
        private void fn_SetFullScaleLoaded(double fullvalue)
        {
            st_Loadcell.m_dFullScaleLoaded = fullvalue;
        }
        private void fn_SetDecPoint(int fullvalue)
        {
            st_Loadcell.m_nDecimalPoint = fullvalue;
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Close Device
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 17:53
        */
        public void fn_Close()
        {
            //if (!fn_IsConnected()) return; 

            try //JUNG/201014
            {
                st_Loadcell._Loadcell.Close_Device_Connection(st_Loadcell.m_ptrHandle);
                st_Loadcell.m_bConnect = false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Futek Loadcell Close Exception :" + ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
            CalculatedReading = (double)(NormalData - OffsetValue) / (FullscaleValue - OffsetValue) * FullScaleLoad / Math.Pow(10, DecimalPoint);
            TextBoxCalculatedReading.Text = String.Format("{0:0.000}", (CalculatedReading - Tare));
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:42
        */
        private double fn_GetValue()
        {
            if (!fn_IsConnected()) return 0.0;

            do 
            {
                st_Loadcell.m_strTemp = st_Loadcell._Loadcell.Normal_Data_Request(st_Loadcell.m_ptrHandle);
            } 
            while (!fn_IsNumeric(st_Loadcell.m_strTemp));

            try
            {
                if (st_Loadcell.m_strTemp == "Error") return 0.0;
                st_Loadcell.m_nNormalData = Convert.ToInt32(st_Loadcell.m_strTemp);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return 0.0;
            }

            //double fullscaledload = 2.34;
            //calculatedValue_Calibration = (double)(NormalData_Calibration - OffsetValue_Calibration_textBox) / (FullscaleValue_Calibration - OffsetValue_Calibration) * fullscaledload * 1000;
            //double calibrationValue = Math.Abs(Math.Round(calculatedValue_Calibration - Tare_Calibration));
            //return calibrationValue;

            double dNormalData = (double)(st_Loadcell.m_nNormalData     - st_Loadcell.m_nOffsetDefault);
            double dFullData   = (double)(st_Loadcell.m_nFullScaleValue - st_Loadcell.m_nOffset       );

            st_Loadcell.m_dCalculatedValue = dNormalData / dFullData * st_Loadcell.m_dFullScaleLoaded * 1000;

            return Math.Abs(Math.Round(st_Loadcell.m_dCalculatedValue - st_Loadcell.m_dTareValue));
            
            //JUNG/200827/delete
            //return Math.Abs(Math.Round(st_Loadcell.m_dCalculatedValue - st_Loadcell.m_dTareValue)) * (1 + FM.m_stMasterOpt.dLDCBtmOffset/100.0);

        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Tare Value 값에 Calculated Value 값을 넣고 GetValue 값을 0 으로 Calibration 한다.
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:59
        */
        public bool fn_TareCalibration()
        {
            if (!fn_IsConnected()) return false;

            st_Loadcell.m_dTareValue = st_Loadcell.m_dCalculatedValue;
            
            return true; 
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
            Tare Value 값을 0 으로 초기화 한다.( 제로 셋팅)
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 11:03
        */
        public void fn_ResetTareValue()
        {
             st_Loadcell.m_dTareValue = 0;
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        /// Gets the offset value by using the FUTEK DLL Method and
        /// check if it's numeric and then parse it into integer
        /// then store it into the memory
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:39
        */
        private bool fn_GetOffsetValue(ref ST_Loadcell loadcell)
        {
            do 
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Offset_Value(loadcell.m_ptrHandle);
            } 
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                //loadcell.m_nOffset = Int32.Parse(loadcell.m_strTemp);
                loadcell.m_nOffset = Convert.ToInt32(loadcell.m_strTemp);
                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        /**    
        <summary>
        /// Gets the fullscale value by using the FUTEK DLL Method and
        /// check if it's numeric and then parse it into integer
        /// then store it into the memory  
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:39
        */
        private bool fn_GetFullScaleValue(ref ST_Loadcell loadcell)
        {
            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Fullscale_Value(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nFullScaleValue = Int32.Parse(loadcell.m_strTemp);
                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        /**    
        <summary>
        /// Gets the fullscale load by using the FUTEK DLL Method and
        /// check if it's numeric and then parse it into integer
        /// then store it into the memory
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:40
        */
        private bool fn_GetFullScaleLoad(ref ST_Loadcell loadcell)
        {
            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Fullscale_Load(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nFullScaleLoadValue = Int32.Parse(loadcell.m_strTemp);

                return true ;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        /**    
        <summary>
        /// Gets the number of decimal places by using the FUTEK 
        /// DLL Method and check if it's numeric and then parse
        /// it into integer then store it into the memory
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:40
        */
        private bool fn_GetDecimalPoint(ref ST_Loadcell loadcell)
        {
            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Decimal_Point(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nDecimalPoint = Int32.Parse(loadcell.m_strTemp);

                if (loadcell.m_nDecimalPoint > 3)
                {
                    loadcell.m_nDecimalPoint = 3;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        /**    
        <summary>
        /// Gets the unit code to later find unit needed for the device
        /// by using the FUTEK DLL Method and check if it's numeric and
        /// then parse it into integer and then store it into the memory
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:41
        */
        private bool fn_GetUnitCode(ref ST_Loadcell loadcell)
        {
            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Unit_Code(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nUnitCode = Int32.Parse(loadcell.m_strTemp);
                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        //---------------------------------------------------------------------------
        private void fn_FindUnits(ref ST_Loadcell loadcell)
        {
            switch (loadcell.m_nUnitCode)
            {
                case 0:
                    loadcell.m_strUnitCode = "atm";
                    break;
                case 1:
                    loadcell.m_strUnitCode = "bar";
                    break;
                case 2:
                    loadcell.m_strUnitCode = "dyn";
                    break;
                case 3:
                    loadcell.m_strUnitCode = "ft-H2O";
                    break;
                case 4:
                    loadcell.m_strUnitCode = "ft-lb";
                    break;
                case 5:
                    loadcell.m_strUnitCode = "g";
                    break;
                case 6:
                    loadcell.m_strUnitCode = "g-cm";
                    break;
                case 7:
                    loadcell.m_strUnitCode = "g-mm";
                    break;
                case 8:
                    loadcell.m_strUnitCode = "in-H2O";
                    break;
                case 9:
                    loadcell.m_strUnitCode = "in-lb";
                    break;
                case 10:
                    loadcell.m_strUnitCode = "in-oz";
                    break;
                case 11:
                    loadcell.m_strUnitCode = "kdyn";
                    break;
                case 12:
                    loadcell.m_strUnitCode = "kg";
                    break;
                case 13:
                    loadcell.m_strUnitCode = "kg-cm";
                    break;
                case 14:
                    loadcell.m_strUnitCode = "kg/cm2";
                    break;
                case 15:
                    loadcell.m_strUnitCode = "kg-m";
                    break;
                case 16:
                    loadcell.m_strUnitCode = "klbs";
                    break;
                case 17:
                    loadcell.m_strUnitCode = "kN";
                    break;
                case 18:
                    loadcell.m_strUnitCode = "kPa";
                    break;
                case 19:
                    loadcell.m_strUnitCode = "kpsi";
                    break;
                case 20:
                    loadcell.m_strUnitCode = "lbs";
                    break;
                case 21:
                    loadcell.m_strUnitCode = "Mdyn";
                    break;
                case 22:
                    loadcell.m_strUnitCode = "mmHG";
                    break;
                case 23:
                    loadcell.m_strUnitCode = "mN-m";
                    break;
                case 24:
                    loadcell.m_strUnitCode = "MPa";
                    break;
                case 25:
                    loadcell.m_strUnitCode = "MT";
                    break;
                case 26:
                    loadcell.m_strUnitCode = "N";
                    break;
                case 27:
                    loadcell.m_strUnitCode = "N-cm";
                    break;
                case 28:
                    loadcell.m_strUnitCode = "N-m";
                    break;
                case 29:
                    loadcell.m_strUnitCode = "N-mm";
                    break;
                case 30:
                    loadcell.m_strUnitCode = "oz";
                    break;
                case 31:
                    loadcell.m_strUnitCode = "psi";
                    break;
                case 32:
                    loadcell.m_strUnitCode = "Pa";
                    break;
                case 33:
                    loadcell.m_strUnitCode = "T";
                    break;
                case 34:
                    loadcell.m_strUnitCode = "mV/V";
                    break;
                case 35:
                    loadcell.m_strUnitCode = "µA";
                    break;
                case 36:
                    loadcell.m_strUnitCode = "mA";
                    break;
                case 37:
                    loadcell.m_strUnitCode = "A";
                    break;
                case 38:
                    loadcell.m_strUnitCode = "mm";
                    break;
                case 39:
                    loadcell.m_strUnitCode = "cm";
                    break;
                case 40:
                    loadcell.m_strUnitCode = "dm";
                    break;
                case 41:
                    loadcell.m_strUnitCode = "m";
                    break;
                case 42:
                    loadcell.m_strUnitCode = "km";
                    break;
                case 43:
                    loadcell.m_strUnitCode = "in";
                    break;
                case 44:
                    loadcell.m_strUnitCode = "ft";
                    break;
                case 45:
                    loadcell.m_strUnitCode = "yd";
                    break;
                case 46:
                    loadcell.m_strUnitCode = "mi";
                    break;
                case 47:
                    loadcell.m_strUnitCode = "µg";
                    break;
                case 48:
                    loadcell.m_strUnitCode = "mg";
                    break;
                case 49:
                    loadcell.m_strUnitCode = "LT";
                    break;
                case 50:
                    loadcell.m_strUnitCode = "mbar";
                    break;
                case 51:
                    loadcell.m_strUnitCode = "˚C";
                    break;
                case 52:
                    loadcell.m_strUnitCode = "˚F";
                    break;
                case 53:
                    loadcell.m_strUnitCode = "K";
                    break;
                case 54:
                    loadcell.m_strUnitCode = "˚Ra";
                    break;
                case 55:
                    loadcell.m_strUnitCode = "kN-m";
                    break;
                case 56:
                    loadcell.m_strUnitCode = "g-m";
                    break;
                case 57:
                    loadcell.m_strUnitCode = "nV";
                    break;
                case 58:
                    loadcell.m_strUnitCode = "µV";
                    break;
                case 59:
                    loadcell.m_strUnitCode = "mV";
                    break;
                case 60:
                    loadcell.m_strUnitCode = "V";
                    break;
                case 61:
                    loadcell.m_strUnitCode = "kV";
                    break;
                case 62:
                    loadcell.m_strUnitCode = "NONE";
                    break;
                default:
                    loadcell.m_strUnitCode = "Undefined";
                    break;
            }
        }
        //---------------------------------------------------------------------------
        private bool fn_IsNumeric(object Value)
        {
            if(Value as string == "Error") //JUNG/200519
            {
                return true; 

            }

            if (Value == null || Value is DateTime)
                return false;
            if (Value is Int16 || Value is Int32 || Value is Int64 || Value is Decimal || Value is Single || Value is Double || Value is Boolean)
                return true;

            try
            {
                if (Value is string)
                    double.Parse(Value as string);
                else
                    double.Parse(Value.ToString());

                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        //---------------------------------------------------------------------------
        public void fn_Update()
        {
            try
            {
                if (!fn_IsConnected()) return;

                m_dLoadCellValue = fn_GetValue();

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[LDCBTM.fn_Update()]" + ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        public double fn_GetBtmLoadCell(bool bNT = false)
        {
            //Default = g / bNT = N
            double ONEGRAM_TO_NEWTON = 0.00980665;
            double dRtn = bNT ? Math.Round(m_dLoadCellValue * ONEGRAM_TO_NEWTON, 2) : Math.Round(m_dLoadCellValue, 4);

            return dRtn;

        }
        //---------------------------------------------------------------------------
        public string fn_GetLoadCellValue()
        {
            return string.Format($"{m_dLoadCellValue } {_sUnit}");
        }
        //---------------------------------------------------------------------------
        public string fn_GetSN()
        {
            string sn  = string.Empty;
            int    nSN = -1;

            for (int i = 0; i < 16; i++)
            {
                try
                {
                    sn = this.st_Loadcell.GetSerialNo(i);
                    if (sn == "Error" || sn == "") continue;
                    nSN = Convert.ToInt32(sn);
                    break;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    return sn;
                }
            }
            st_Loadcell.m_strSerialNo = sn;
            return st_Loadcell.m_strSerialNo; 
        }
    }
}
