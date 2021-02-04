using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using WaferPolishingSystem;
using WaferPolishingSystem.Define;
using System.Windows.Controls;
using System.Windows.Media;
using static WaferPolishingSystem.FormMain;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using static WaferPolishingSystem.Define.UserEnum;
using System.Windows.Threading;
using System.Windows;

namespace WaferPolishingSystem.Define
{
    /**
    @class     User Function Class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/1/30  20:33
    */
    class UserFunction
    {

        //static bool   m_bRqShowMsg;
        //
        //static bool   m_bRqHideMsg;
        //static string m_sWarnMsg  ;
        //static string m_sErrMsg   ;

        //
        static public FormJog UserJog;

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Sleep Function
        </summary>
        <param name="millisecond"> millisecond Setting </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/01/30 20:33
        */
        static public void fn_Sleep(int millisecond)
        {
            Task.Delay(TimeSpan.FromMilliseconds(millisecond)).Wait();
            return; 
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	User Message Display
        </summary>
        <param name="caption"> Caption Message </param>
        <param name="msg"> Message </param>
        <param name="type"> Message </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/01/31 20:14
        */
        static public bool fn_UserMsg(string msg, EN_MSG_TYPE type, string caption = "")
        {
            return fn_UserMsg(msg, type, caption, EN_SHOW_MODE.Normal);
        }

        static public bool fn_UserMsg(string msg)
        {
            return fn_UserMsg(msg, EN_MSG_TYPE.Info, "CHECK", EN_SHOW_MODE.Normal);
        }

        static public bool fn_UserMsg(string msg, EN_MSG_TYPE type, string caption , EN_SHOW_MODE modal)
        {
            bool rtn = false;

            if (SEQ._bRun && (SEQ._iStep > 13 && SEQ._iStep < 16)) return false; //JUNG/200418/Check Run

            if (UserMsg.IsVisible)
            {
                //UserMsg.Topmost = true;
                //UserMsg.Show();
                fn_UserMsgClose();
                //return false; 
            }

            //
            UserMsg.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate()
            {
                UserMsg.fn_SetMsg(msg, caption, type);

                fn_WriteLog(msg); //JUNG/200922

                //사용할 메서드 및 동작
                if (modal == EN_SHOW_MODE.Modal || type == EN_MSG_TYPE.Check) UserMsg.ShowDialog();
                else
                {
                    //UserMsg.Topmost = true; 
                    UserMsg.Show();
                }

                rtn = UserMsg._bResult || (UserMsg.DialogResult.HasValue && UserMsg.DialogResult.Value);

            }));

            return rtn;
        }
        //---------------------------------------------------------------------------
        static public bool fn_UserMsgClose()
        {

            if (!UserMsg.IsVisible)
            {
                return false; 
            }

            //
            UserMsg.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate()
            {
                UserMsg.Hide();
            }));

            return true;
        }

//         //---------------------------------------------------------------------------
// 		internal static void fn_ShowWarn(bool bShow, string sMsg = "") 
//         {
//             m_sWarnMsg = sMsg;
//             if(bShow) m_bRqShowMsg   = true;
//             else      
//             {
//                 m_bRqHideMsg = true ;
//                 m_bRqShowMsg = false;
//                 m_sWarnMsg   = ""   ;
//             }
//         }
//         //--------------------------------------------------------------------------
// 		internal static void fn_UpdateMsg () 
//         {
//             if(m_bRqHideMsg) {
//                 m_bRqHideMsg = false;
//                 fn_ShowMsg(false);
//             }
// 
//             if(m_bRqShowMsg) 
//             {
//                 m_bRqShowMsg = false;
//                 if (m_sWarnMsg != "") fn_ShowMsg(true, "Warning", m_sWarnMsg);
//             }
// 
//         }
//         //--------------------------------------------------------------------------
//         public static bool fn_ShowMsg(bool bShow, String sTitle = "", String sMsg = "", int iKind = (int)EN_SHOW_MODE.Normal)
//         {
//             System.Windows.Forms.DialogResult dr = new System.Windows.Forms.DialogResult();
// 
//             if (UserMsg != null) 
//             {
//                 UserMsg.Close();
//                 UserMsg = null;
//             }
// 
//             if (!bShow) return true;
// 
// 
//             UserMsg = new FormMessage();
//             UserMsg.m_nKind  = iKind;
//             UserMsg.m_sTitle = sTitle;
//             UserMsg.m_sMsg   = sMsg;
//             UserMsg.Topmost  = true; 
// 
//             if (iKind == 0) { UserMsg.Show(); return false; }
// 
//             UserMsg.ShowDialog();
//             
//             return dr == System.Windows.Forms.DialogResult.Yes;
// 
//         }
        //---------------------------------------------------------------------------
        static public void fn_UserJog()
        {
            if (UserJog != null) return;

            UserJog = new FormJog();

            UserJog.Topmost = true; 
            UserJog.Show();
        }
        //---------------------------------------------------------------------------
        static public void fn_UserJogClose()
        {
            if (UserJog == null) return;

            UserJog.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                UserJog.Close();
                UserJog = null; 
            }));
           
        }
        //---------------------------------------------------------------------------
        static public SolidColorBrush fn_ColorPick()
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();

            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                return brush;
            }
            else return Brushes.Green;
        }


        //---------------------------------------------------------------------------
        /**    
        <summary>
        	LOG Write Function
        </summary>
        <param name="msg"> Log Message </param>
        <param name="type"> Log Type </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/06 08:46
        */
        static public void fn_WriteLog(string msg, EN_LOG_TYPE type = EN_LOG_TYPE.ltTrace, EN_SEQ_ID part = EN_SEQ_ID.None)
        {
            LOG.fn_WriteLog(type, msg, part);
        }
        static public void fn_WriteTestLog(string msg, string filename = "TEST")
        {
            LOG.fn_WriteTestLog(msg, filename);
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Label Color Setting
        </summary>
            <param name="lavel"> Lavel </param>
            <param name="item"> condition </param>
            <param name="on"> On Color </param>
            <param name="off"> Off Color </param>
            <param name="flick"> Use Flick  </param>
            <param name="flickColor"> Flick Color  </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/12 16:45
        */
        public static void fn_SetLabelColor(ref Label lavel, bool item, Brush on, Brush off, bool flick, Brush flickColor)
        {
            if(flick) lavel.Background = item ? on :  SEQ._bFlick1? flickColor : off;
            else      lavel.Background = item ? on : off;
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Label Color Setting
        </summary>
            <param name="lavel"> Lavel </param>
            <param name="item"> condition </param>
            <param name="on"> On Color </param>
            <param name="off"> Off Color </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/12 16:45
        */
        public static void fn_SetLabelColor(ref Label lavel, bool item, Brush on, Brush off)
        {
            lavel.Background = item ? on : off;
        }
        //---------------------------------------------------------------------------
        public static void fn_SetLabelColor(ref Label lavel, bool item, Brush on)
        {
            lavel.Background = on;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
            Label Color Setting
        </summary>
            <param name="lavel"> Lavel </param>
            <param name="item"> condition </param>
            <param name="ontxt"> On Text </param>
            <param name="offtxt"> Off Text </param>
            <param name="oncolor"> On Color </param>
            <param name="offcolor"> Off Color </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/12 16:45
        */
        public static void fn_SetLabelText(ref Label lavel, bool item, string ontxt, string offtxt, Brush oncolor, Brush offcolor)
        {
            lavel.Foreground = item ? oncolor : offcolor;
            lavel.Content    = item ? ontxt   : offtxt;
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Label Text & Color Setting
        </summary>
            <param name="lavel"> Lavel </param>
            <param name="txt"> Text </param>
            <param name="backcolor"> Lavel Backgraund Color </param>
            <param name="txtcolor"> Lavel Text Color </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/12 17:02
        */
        public static void fn_SetLabel(ref Label lavel, string txt, Brush backcolor, Brush txtcolor = null)
        {
            lavel.Background = backcolor;
            lavel.Content    = txt  ;

            if(txtcolor != null) lavel.Foreground = txtcolor     ;
            else                 lavel.Foreground = Brushes.Black;
        }
        public static void fn_SetLabelTC(ref Label lavel, Brush txtcolor, string txt="")
        {
            lavel.Foreground = txtcolor;

            if(txt != "") lavel.Content = txt;

        }
        public static void fn_SetLabelBC(ref Label lavel, Brush backcolor)
        {
            lavel.Background = backcolor;
        }

        /**    
        <summary>
            Text Box 숫자만 입력해야 하는 경우
        </summary>
        <param name="source"></param>
        @author    이준호(LEEJOONHO)
        @date      2020/02/14 14:12
        */
        public static bool fn_IsNumeric(string source)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return regex.IsMatch(source);
        }
        //-------------------------------------------------------------------------------------------------
        /**    
        <summary>
        	다차원 배열 Resize Function
        </summary>
        <param name="arr"></param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/17 20:25
        */
        public static Array fn_ResizeArray(Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }
        //---------------------------------------------------------------------------
        public static int fn_BinaryByteToInt(Byte[] bytes, int start = 0,  bool reverse = false)
        {
            Byte[] newbyte = new Byte[16];
            Array.ConstrainedCopy(bytes, start, newbyte, 0, newbyte.Length);
            if (reverse) Array.Reverse(newbyte);
            string value = "";
            foreach (var byt in newbyte)
                value += String.Format("{0}", byt);
            int rtn = Convert.ToInt32(value, 2);
            return rtn;
        }

        //---------------------------------------------------------------------------
        public static BitmapImage fn_Convert(System.Drawing.Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        //---------------------------------------------------------------------------


		// 1. Deep Clone 구현
        public static T DeepClone<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null.");

            return (T)Process(obj, new Dictionary<object, object>() { });
        }

		private static object Process(object obj, Dictionary<object, object> circular)
        {
            if (obj == null)
                return null; 

            Type type = obj.GetType(); 

			if (type == null) return null;

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            } 

            if (type.IsArray)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj];

				int iPos = type.FullName.IndexOf("[");
				string typeNoArray = type.FullName.Substring(0, iPos);
                //string typeNoArray = type.FullName.Replace("[]", string.Empty);	
                Type elementType = Type.GetType(typeNoArray + ", " + type.Assembly.FullName);
                var array = obj as Array;

                Array arrCopied;
				if      (array.Rank == 1) arrCopied = Array.CreateInstance(elementType, array.Length);
				else if (array.Rank == 2) arrCopied = Array.CreateInstance(elementType, array.GetLength(0), array.GetLength(1));
				else if (array.Rank == 3) arrCopied = Array.CreateInstance(elementType, array.GetLength(0), array.GetLength(1), array.GetLength(2));
				else return null;				

                circular[obj] = arrCopied; 

				if (array.Rank == 1) //1차원 배열
				{
					for (int i = 0; i < array.Length; i++)
					{
						object element = array.GetValue(i);
						object objCopy = null; 
					
						if (element != null && circular.ContainsKey(element))
						    objCopy = circular[element];
						else
						    objCopy = Process(element, circular); 
					
						arrCopied.SetValue(objCopy, i);
					}
				}
				else if (array.Rank == 2) //2차원 배열
				{
					for (int i = 0; i < array.GetLength(0); i++)
					{
						for (int j = 0; j < array.GetLength(1); j++)
						{
							object element = array.GetValue(i, j);
							object objCopy = null; 
						
							if (element != null && circular.ContainsKey(element))
							    objCopy = circular[element];
							else
							    objCopy = Process(element, circular); 
						
							arrCopied.SetValue(objCopy, i, j);
						}
					}
				}
				else if (array.Rank == 3) //3차원 배열까지만.
				{
					for (int i = 0; i < array.GetLength(0); i++)
					{
						for (int j = 0; j < array.GetLength(1); j++)
						{
							 for (int k = 0; k < array.GetLength(2); k++)
							 {
							 	  object element = array.GetValue(i, j, k);
							 	  object objCopy = null; 
							 	  
							 	  if (element != null && circular.ContainsKey(element))
							 	      objCopy = circular[element];
							 	  else
							 	      objCopy = Process(element, circular); 
							 	  
							 	  arrCopied.SetValue(objCopy, i, j, k);
							 }
						}
					}
				}
				else return null;

                return Convert.ChangeType(arrCopied, obj.GetType());
            } 

            if (type.IsClass)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj]; 

                object objValue = Activator.CreateInstance(obj.GetType()); //Class 생성자에 매개변수가 있을때는 안됨. 매개변수 없는 생성자 만들어야 함.
                circular[obj] = objValue;
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); 

                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);

                    if (fieldValue == null)
                        continue; 

                    object objCopy = circular.ContainsKey(fieldValue) ? circular[fieldValue] : Process(fieldValue, circular);
                    field.SetValue(objValue, objCopy);
                }

                return objValue;
            }
            else
                throw new ArgumentException("Unknown type");
        } 

        // 2. Serializable 객체에 대한  Deep Clone
        public static T SerializableDeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var bformatter = new BinaryFormatter();
                bformatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T) bformatter.Deserialize(ms);
            }
        }
        //---------------------------------------------------------------------------
        // [CONVERT] Int To Boolean
        public static bool[] fn_IntToBoolArray(int value)
        {
            int    length = 26;
            bool[] boolArray = new bool[length];

            for (int index = 0; index < length; index++)
            {
                boolArray[index] = ((value >> index) & 1) == 1;
            }

            return boolArray;
        }

        //---------------------------------------------------------------------------
        public static Point fn_LineFitting_LMS(Point[] vecPoint, int cnt)
        {
            Point stLine = new Point();
            int nStep    = 0;
            int nVecSize = 0;
            double dAvgX = 0.0;
            double dAvgY = 0.0;

            nVecSize = cnt; //vecPoint.Length;
            for (nStep = 0; nStep < nVecSize; nStep++)
            {
                dAvgX += vecPoint[nStep].X / (double)nVecSize;
                dAvgY += vecPoint[nStep].Y / (double)nVecSize;
            }
            double temp1 = 0.0;
            double temp2 = 0.0;
            for (nStep = 0; nStep < nVecSize; nStep++)
            {
                temp1 += (vecPoint[nStep].X - dAvgX) * (vecPoint[nStep].Y - dAvgY);
                temp2 += Math.Pow(vecPoint[nStep].X - dAvgX, 2);
            }
            try
            {
                stLine.X = temp1 / temp2;
                stLine.Y = dAvgY - stLine.X * dAvgX;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return stLine;
        }


    }

    //---------------------------------------------------------------------------
    /**
    @class     One-Shot Detect Class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2019/11/1  14:20
    */
    public class TOneShotDetect
    {
    	bool m_bState; 
    	bool m_bInit ; 
    	//bool isChng  ;
    
        //생성자 & 소멸자
    	public TOneShotDetect() { Init(); }
        
        //Init.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        void Init() { m_bInit = false; m_bState = false; }
        void Init(bool Crnt) { m_bState = Crnt ; m_bInit = true ;}
    
        //Functions.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private bool IsChange(bool Crnt)
        {
            if (!m_bInit) 
    		{ 
    			m_bInit  = true ; 
    			m_bState = Crnt ; 
    			return false ;
    	    }
    
            if (m_bState != Crnt) 
    		{
    			m_bState = Crnt;
                return true;
            }
            return false;
        }
        public bool IsRising (bool Crnt ) 
    	{ 
    		bool isChng = (IsChange(Crnt) &&  Crnt);
    		return isChng; 
        }
    
        public bool IsFalling(bool Crnt ) 
    	{ 
    		bool isChng = (IsChange(Crnt) && !Crnt); 
    		return isChng; 
        }
    }



}
