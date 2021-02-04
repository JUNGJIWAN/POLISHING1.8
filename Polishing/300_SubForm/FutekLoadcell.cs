using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.Unit
{
    enum EN_LOADCELL
    {
        Bottom = 0,
        Top,
    }

    public struct ST_Loadcell
    {
        public FUTEK_USB_DLL.USB_DLL _Loadcell;

        public IntPtr m_ptrHandle;
        public string m_strSerialNo;
        public string m_strTemp;
        public int    m_nOffsetDefault;
        public int    m_nOffset;
        public int    m_nFullScaleValue;
        public int    m_nFullScaleLoadValue;
        public int    m_nDecimalPoint;
        public int    m_nUnitCode;
        public string m_strUnitCode;
        public int    m_nNormalData;
        public bool   m_bConnect;
        public double m_dTareValue;
        public double m_dCalculatedValue;
        public double m_dFullScaleLoaded;

        public ST_Loadcell(FUTEK_USB_DLL.USB_DLL _Loadcell, IntPtr ptrHandle, string strSerialNo, string strTemp, int nOffsetDefault,
             int nOffset, int nFullScaleValue, int nFullScaleLoadValue, int nDecimalPoint, int nUnitCode, string strUnitCode,
             int nNormalData, bool bConnect, double dTareValue, double dCalculatedValue, double dFullScaleLoaded)
        {
            this._Loadcell = _Loadcell;
            this.m_ptrHandle = ptrHandle;
            this.m_strSerialNo = strSerialNo;
            this.m_strTemp = strTemp;
            this.m_nOffsetDefault = nOffsetDefault;
            this.m_nOffset = nOffset;
            this.m_nFullScaleValue = nFullScaleValue;
            this.m_nFullScaleLoadValue = nFullScaleLoadValue;
            this.m_nDecimalPoint = nDecimalPoint;
            this.m_nUnitCode = nUnitCode;
            this.m_strUnitCode = strUnitCode;
            this.m_nNormalData = nNormalData;
            this.m_bConnect = bConnect;
            this.m_dTareValue = dTareValue;
            this.m_dCalculatedValue = dCalculatedValue;
            this.m_dFullScaleLoaded = dFullScaleLoaded;
        }
    };
    //------------------------------------------------------------------------------------------------
    public class LOADCELL
    {
        #region Futek Loadcell Parameter

        //public FUTEK_USB_DLL.USB_DLL Loadcell_Bottom = new FUTEK_USB_DLL.USB_DLL();
        //public FUTEK_USB_DLL.USB_DLL Loadcell_Top = new FUTEK_USB_DLL.USB_DLL();

        //public string m_strSerialNumber_Bottom      = "792541";
        //public string m_strSerialNumber_Top         = "802150";

        //public IntPtr DeviceHandle_Bottom;
        //public IntPtr DeviceHandle_Top;

        //public string m_strTemp_Bottom;
        //public string m_strTemp_Top;

        //public Int32 OffsetValue_Bottom_textBox = 8373643;
        //public Int32 OffsetValue_Top_textBox = 8389960;

        //private double InputOffset = 0;

        //public Int32 m_nOffsetValue_Bottom;
        //public Int32 m_nOffsetValue_Top;

        //public Int32 FullscaleValue_Bottom;
        //public Int32 FullscaleValue_Top;

        //public Int32 FullScaleLoad_Bottom;
        //public Int32 FullScaleLoad_Top;

        //public Int32 DecimalPoint;
        //public Int32 UnitCode_Bottom;
        //public Int32 UnitCode_Top;
        //public double Tare_Bottom = 10.871366211572395;
        //public double Tare_Top = -9.21936819266264;

        //public Int32 NormalData_Bottom;
        //public Int32 NormalData_Top;

        //public Boolean m_bConnect_Bottom    = false;
        //public Boolean m_bConnect_Top       = false;

        //private double calculatedValue_Top = 0;
        //private double calculatedValue_Bottom = 0;

        //private double m_dfullscaledload_Bottom = 2.34;
        //private double m_dfullscaledload_Top = 10.7;
        #endregion

        // return value
        private bool m_bRet = false;
        public ST_Loadcell[] st_Loadcell;

        public bool _Connect
        {
            get { return fn_IsConnected(); }
        }
        public double _TareValue { get; set; }
        public double _CalcValue { get; set; }

        public LOADCELL()
        {
            st_Loadcell = new ST_Loadcell[]
            {
                // Bottom Load cell Model : LSB201 S/N : 809849
                new ST_Loadcell(new FUTEK_USB_DLL.USB_DLL(), new IntPtr(), "834592", null, 8373643, 0, 0, 0, 0, 0, "", 0, false, 10.871366211572395, 0.0, 2.34),

                // Top    Load cell Model : LSB201 S/N : 844186
                new ST_Loadcell(new FUTEK_USB_DLL.USB_DLL(), new IntPtr(), "841888", null, 8389960, 0, 0, 0, 0, 0, "", 0, false, -9.21936819266264, 0.0, 10.7)
           
            };
        }

        public bool Open(int nIndex = 0)
        {
            if (fn_OpenDevice(nIndex))  return true;
            else                        return false;
        }
        private bool fn_IsConnected()
        {
            return st_Loadcell[0].m_bConnect;
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
        public bool fn_OpenDevice(int nIndex)
        {
            m_bRet = false;

            if (st_Loadcell[nIndex].m_bConnect) return false;
            
            try
            {
                st_Loadcell[nIndex]._Loadcell.Open_Device_Connection(st_Loadcell[nIndex].m_strSerialNo);

                if (st_Loadcell[nIndex]._Loadcell.DeviceStatus != 0) return m_bRet;
                else
                {
                    st_Loadcell[nIndex].m_ptrHandle = st_Loadcell[nIndex]._Loadcell.DeviceHandle;
                    //m_bOpenedConnection_Bottom = true;

                    if (!fn_GetOffsetValue(ref      st_Loadcell[nIndex]))  return m_bRet;
                    if (!fn_GetFullScaleValue(ref   st_Loadcell[nIndex]))  return m_bRet;
                    if (!fn_GetFullScaleLoad(ref    st_Loadcell[nIndex]))  return m_bRet;
                    if (!fn_GetDecimalPoint(ref     st_Loadcell[nIndex]))  return m_bRet;
                    if (!fn_GetUnitCode(ref         st_Loadcell[nIndex]))  return m_bRet;

                    fn_FindUnits(ref st_Loadcell[nIndex]);

                    st_Loadcell[nIndex].m_bConnect = true;
                    m_bRet = true;
                    return m_bRet;
                }
            }
            catch (System.Exception ex)
            {
                return m_bRet;
            }
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
        public void fn_CloseDevice(int nIndex = 0)
        {
            st_Loadcell[nIndex]._Loadcell.Close_Device_Connection(st_Loadcell[nIndex].m_ptrHandle);
            st_Loadcell[nIndex].m_bConnect = false;
        }
        /**    
        <summary>
            CalculatedReading = (double)(NormalData - OffsetValue) / (FullscaleValue - OffsetValue) * FullScaleLoad / Math.Pow(10, DecimalPoint);
            TextBoxCalculatedReading.Text = String.Format("{0:0.000}", (CalculatedReading - Tare));
        </summary>
        <param name=""></param>
        @author    이준호(LEEJOONHO)
        @date      2020/03/04 10:42
        */
        public double fn_GetValue(int nIndex = 0)
        {
            if (st_Loadcell[nIndex].m_bConnect) return 0;

            do 
            {
                st_Loadcell[nIndex].m_strTemp = st_Loadcell[nIndex]._Loadcell.Normal_Data_Request(st_Loadcell[nIndex].m_ptrHandle);
            } 
            while (!fn_IsNumeric(st_Loadcell[nIndex].m_strTemp));

            try
            {
                st_Loadcell[nIndex].m_nNormalData = Convert.ToInt32(st_Loadcell[nIndex].m_strTemp);
            }
            catch (System.Exception ex)
            {
            	
            }
            //double fullscaledload = 2.34;

            //calculatedValue_Calibration = (double)(NormalData_Calibration - OffsetValue_Calibration_textBox) / (FullscaleValue_Calibration - OffsetValue_Calibration) * fullscaledload * 1000;

            //double calibrationValue = Math.Abs(Math.Round(calculatedValue_Calibration - Tare_Calibration));

            //return calibrationValue;

            st_Loadcell[nIndex].m_dCalculatedValue = (double)(st_Loadcell[nIndex].m_nNormalData - st_Loadcell[nIndex].m_nOffsetDefault) / (st_Loadcell[nIndex].m_nFullScaleValue - st_Loadcell[nIndex].m_nOffset) * st_Loadcell[nIndex].m_dFullScaleLoaded * 1000;

            return Math.Abs(Math.Round(st_Loadcell[nIndex].m_dCalculatedValue - st_Loadcell[nIndex].m_dTareValue));

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
        public void fn_TareCalibration(int nIndex = 0)
        {
            if (st_Loadcell[nIndex].m_bConnect) return;

            st_Loadcell[nIndex].m_dTareValue = st_Loadcell[nIndex].m_dCalculatedValue;
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
        public void fn_ResetTareValue(int nIndex = 0)
        {
            if (st_Loadcell[nIndex].m_bConnect) return;

            st_Loadcell[nIndex].m_dTareValue = 0;
        }
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
            m_bRet = false;

            do 
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Offset_Value(loadcell.m_ptrHandle);
            } 
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                //loadcell.m_nOffset = Int32.Parse(loadcell.m_strTemp);
                loadcell.m_nOffset = Convert.ToInt32(loadcell.m_strTemp);
                m_bRet = true;
                return m_bRet;
            }
            catch (System.Exception ex)
            {
                return m_bRet;
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
            m_bRet = false;

            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Fullscale_Value(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nFullScaleValue = Int32.Parse(loadcell.m_strTemp);
                m_bRet = true;
                return m_bRet;
            }
            catch (System.Exception ex)
            {
                return m_bRet;
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
            m_bRet = false;

            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Fullscale_Load(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nFullScaleLoadValue = Int32.Parse(loadcell.m_strTemp);
                m_bRet = true;
                return m_bRet;
            }
            catch (System.Exception ex)
            {
                return m_bRet;
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
            m_bRet = false;

            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Decimal_Point(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nDecimalPoint = Int32.Parse(loadcell.m_strTemp);
                m_bRet = true;

                if (loadcell.m_nDecimalPoint > 3)
                {
                    loadcell.m_nDecimalPoint = 3;
                }

                return m_bRet;
            }
            catch (System.Exception ex)
            { 
                return m_bRet;
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
            m_bRet = false;

            do
            {
                loadcell.m_strTemp = loadcell._Loadcell.Get_Unit_Code(loadcell.m_ptrHandle);
            }
            while (!fn_IsNumeric(loadcell.m_strTemp));

            try
            {
                loadcell.m_nUnitCode = Int32.Parse(loadcell.m_strTemp);
                m_bRet = true;
                return m_bRet;
            }
            catch (System.Exception ex)
            {
                return m_bRet;
            }
        }
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
        private static System.Boolean fn_IsNumeric(System.Object Value)
        {
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
                return false;
            }
        }
    }
}
