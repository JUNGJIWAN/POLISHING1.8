using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WaferPolishingSystem.Define
{
    //---------------------------------------------------------------------------
    /**
    @class	UserFunctionVision
    @brief	사용자 정의 함수
    @remark	
     - C 라이브러리 인터페이스
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:09
    */
    class UserFunctionVision
    {
        const string STRDLLPATH = "Dll\\PolishingAlignCore.dll";
        public delegate int UserCallBack(int func, int seq, IntPtr data1, ref IntPtr data2);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libSetImage(IntPtr data, double width, double height, double channel);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libSetMark(IntPtr data, double width, double height, double channel);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libInit();

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libCheckLicense();

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libSetParam(IntPtr paramModel, IntPtr paramPattern);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libRunProcModel();

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern double libRunProcModelNext(bool bIncrease);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libRunProcPattern();

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern double libRunProcPatternNext(bool bIncrease);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libGetResult(IntPtr paramResult);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libDestroyModel();

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libDestroyPattern();

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libDestroy();

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libThreshold(IntPtr ptr, double width, double hegith, int channel, double thresh, double Smooth);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libSetROI(int x, int y, int width, int height);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libCanny(IntPtr ptr, double width, double height, int channel, double sigma, double lowth, double highth);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libPinSearch(double radius, double smooth);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libSetLotName(StringBuilder name, int length);

        //[DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //public extern static void libMeasureLength(ref int maxstarty, ref int maxendy, ref int maxidx);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int libProcMeasureLen(IntPtr ptrParam, bool bAuto = true);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void libGetMeasureLen(ref int maxstarty, ref int maxendy, ref int cy, int idx);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void libGetLineProfile(IntPtr data, int len);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void libGetToolPos(PointD RefToolPos, PointD MarkPinPos, PointD Pitch, PointD Offset, double Angle, ref PointD RetPnt);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr libGetModelEdge(IntPtr ptr, int width, int height, IntPtr parameter);

        [DllImport(STRDLLPATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void libReleaseEdge();
        // Pointer Copy
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        // Deep Copy
        public static T DeepClone<T>(T from)
        {
            using (MemoryStream s = new MemoryStream())
            {
                BinaryFormatter f = new BinaryFormatter();
                f.Serialize(s, from);
                s.Position = 0;
                object clone = f.Deserialize(s);

                return (T)clone;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public Point fn_GetPositionFromImageCenter(Point pnt)
        @brief	이미지 중심 기준 입력된 포인트 상대 좌표 변환.
        @return	Point : 이미지 중심 기준 상대 좌표.
        @param	Point pnt : 변환 하고자하는 이미지 좌표.
        @remark	
         - 좌표는 1사분면 좌표계로 환산.
         - 4사분면 -> 1사분면
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/25  17:11
        */
        public static Point fn_GetPositionFromImageCenter(Point pnt, double CamWidth, double CamHeight)
        {
            Point pntReturn = new Point();

            pntReturn.X = (pnt.X - (CamWidth / 2.0));
            pntReturn.Y = ((CamHeight / 2.0) - pnt.Y);

            return pntReturn;
        }

        public static PointD fn_GetPositionFromImageCenter(PointD pnt, double CamWidth, double CamHeight)
        {
            PointD pntReturn = new PointD();

            pntReturn.x = (pnt.x - (CamWidth / 2.0));
            pntReturn.x = ((CamHeight / 2.0) - pnt.y);

            return pntReturn;
        }

        public static void fn_SaveImage(WriteableBitmap wb, string strPath)
        {
            if (wb != null)
            {
                try
                {
                    using (FileStream stream = new FileStream(strPath, FileMode.Create))
                    {
                        BmpBitmapEncoder encoder = new BmpBitmapEncoder();

                        encoder.Frames.Add(BitmapFrame.Create(wb));
                        encoder.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    //---------------------------------------------------------------------------
    /**
    @class	INI
    @brief	INI파일 입출력 함수
    @remark	
     - 기본 String으로 읽어서 C#의 컨버트로 형변환함.
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:09
    */
    class INI
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string val, StringBuilder retVal, int size, string filepath);
        const int _MaxPathLength = 256;
        static public string fnGetINIString(string section, string key, string val, string filepath)
        {
            StringBuilder sbReturnValue = new StringBuilder(_MaxPathLength);
            GetPrivateProfileString(section, key, val, sbReturnValue, sbReturnValue.Capacity, filepath);
            return sbReturnValue.ToString();
        }

        static public int fnGetINIInt(string section, string key, string val, string filepath)
        {
            string strValue = fnGetINIString(section, key, val, filepath);
            int rtnValue = 0;
            try
            {
                rtnValue = Convert.ToInt32(strValue);
            }
            catch
            {
                rtnValue = -1;
            }
            return rtnValue;
        }

        static public long fnSetINIString(string section, string key, string val, string filepath)
        {
            return WritePrivateProfileString(section, key, val, filepath);
        }

        static public long fnSetINIInt(string section, string key, int val, string filepath)
        {
            string strSetValue;
            try
            {
                strSetValue = Convert.ToString(val);
                return fnSetINIString(section, key, strSetValue, filepath);
            }
            catch
            {
                return -1;
            }
        }
    }
}
