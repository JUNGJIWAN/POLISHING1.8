using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.Define
{
    //---------------------------------------------------------------------------
    /**
    @class	INI
    @brief	INI파일 입출력 함수
    @remark	
     - 기본 String으로 읽어서 C#의 컨버트로 형변환함.
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/2/5  17:09
    */
    class UserINI
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString  (string section, string key, string val, StringBuilder retVal, int size, string filepath);
        
        const int _MaxPathLength = 256;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Load
        static public string fn_Load(string section, string key, string val, string filepath)
        {
            StringBuilder sbReturnValue = new StringBuilder(_MaxPathLength);
            GetPrivateProfileString(section, key, val, sbReturnValue, sbReturnValue.Capacity, filepath);
            return sbReturnValue.ToString();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static public int fn_Load(string section, string key, int val, string filepath)
        {
            string strValue = fn_GetINIString(section, key, val.ToString(), filepath);
            int rtnValue = 0;
            try
            {
                rtnValue = Convert.ToInt32(strValue);
            }
            catch
            {
                rtnValue = -1;
                return -1; 
            }

            return rtnValue;
        }
        //---------------------------------------------------------------------------
        static public bool fn_Load(string section, string key, bool val, string filepath)
        {
            int nTemp = val ? 1 : 0; 
            string strValue = fn_GetINIString(section, key, nTemp.ToString(), filepath);
            int rtnValue = 0;
            try
            {
                rtnValue = Convert.ToInt32(strValue);
            }
            catch
            {
                rtnValue = -1;
                return false; 
            }

            return (rtnValue == 1);

        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static public double fn_Load(string section, string key, double val, string filepath)
        {
            string strValue = fn_GetINIString(section, key, val.ToString(), filepath);
            double rtnValue = 0;
            try
            {
                rtnValue = Convert.ToDouble(strValue);
            }
            catch
            {
                rtnValue = -1;
                return 0.0;
            }
            
            return rtnValue;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static private string fn_GetINIString(string section, string key, string val, string filepath)
        {
            StringBuilder sbReturnValue = new StringBuilder(_MaxPathLength);
            GetPrivateProfileString(section, key, val, sbReturnValue, sbReturnValue.Capacity, filepath);
            return sbReturnValue.ToString();
        }
        //---------------------------------------------------------------------------
        //Save
        static public long fn_Save(string section, string key, bool val, string filepath)
        {
            string strSetValue;
            try
            {
                strSetValue = val? "1" : "0";
                return WritePrivateProfileString(section, key, strSetValue, filepath);
            }
            catch
            {
                return -1;
            }

        }
        //---------------------------------------------------------------------------
        static public long fn_Save(string section, string key, int val, string filepath)
        {
            string strSetValue;
            try
            {
                strSetValue = Convert.ToString(val);
                return WritePrivateProfileString(section, key, strSetValue, filepath);
            }
            catch
            {
                return -1;
            }

        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static public long fn_Save(string section, string key, double val, string filepath)
        {
            string strSetValue = string.Empty;

            try
            {
                strSetValue = Convert.ToString(val);
                return WritePrivateProfileString(section, key, strSetValue, filepath);
            }
            catch
            {
                return -1;
            }

        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static public long fn_Save(string section, string key, string val, string filepath)
        {
            return WritePrivateProfileString(section, key, val, filepath);
        }

    }
    //---------------------------------------------------------------------------
    class UserFile
    {
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	File Exist Check
        </summary>
        <param name="file">Check 할 파일 </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/07 13:57
        */
        static public bool fn_CheckFileExist(string file)
        {
            FileInfo fileInfo = new FileInfo(file);

            return fileInfo.Exists;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	실행 파일 경로 찾기
        </summary>
        
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 09:16
        */
        static public string fn_GetExePath()
        {
            string sRtn = string.Empty;

            sRtn = System.Reflection.Assembly.GetExecutingAssembly().Location;

            int nIndex = sRtn.LastIndexOf("\\") + 1;

            //sRtn = sRtn.Substring(nIndex, (sRtn.Length - nIndex)); //file name
            sRtn = sRtn.Substring(0, nIndex); //경로.

            return sRtn; 
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Directory Exist Check & Make Directory
        </summary>
        <param name="dir"> 찾을 Directory </param>
        <param name="onlycheck"> Check만 할지 선택 </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 09:18
        */
        static public bool fn_CheckDir(string dir, bool onlycheck = false)
        {

            bool bRtn = Directory.Exists(dir);

            if (bRtn) return true;
            else
            {
                if (onlycheck) return false;
                else
                {
                    if (dir.Length - 1 > dir.LastIndexOf('\\'))
                    {
                        dir += "\\";
                    }

                    DirectoryInfo di = Directory.CreateDirectory(dir);

                    bRtn = di.Exists;
                }
            }

            return bRtn; 

        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Check Exist File & Count
        </summary>
        <param name="dir"> Check Directory </param>
        <param name="Cnt"> Check File Count </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 10:40
        */
        public static string[] fn_GetExistFile(string dir, ref int Cnt)
        {
            //Local Var.
            int nCnt   = 0;
            int nIndex = 0;
            string[] sFileName = new string[100];

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(dir);
            //string[] fileName = Directory.GetFileSystemEntries(dir); //하위 폴더 전체 파일 가져오기...

            if (fileEntries.Length>0)
            {
                //Array.Resize<String>(ref sFileName, fileEntries.Length);
                Array.Resize(ref sFileName, fileEntries.Length);
            }
            else
            {
                Cnt = 0;
                return fileEntries; 
            }
            

            foreach (var item in fileEntries)
            {
                nIndex = item.LastIndexOf("\\") + 1;

                sFileName[nCnt] = item.Substring(nIndex, (item.Length-nIndex));

                nCnt++;
            }

            Cnt = nCnt;  

            return sFileName;

        }

        //---------------------------------------------------------------------------
        public static string[] fn_GetFileList(string path)
        {
            string[] strPaths;
            string[] strFiles;
            try
            {
                if (!Directory.Exists(path))
                {
                    //Directory.CreateDirectory(path);
//                     strFiles = new string[1];
//                     strFiles[0] = string.Empty;

                    strFiles = null; 
                    return strFiles; 
                }

                strPaths = Directory.GetFiles(path);
                strFiles = new string[strPaths.Length];
                for(int i =0; i < strPaths.Length; i++)
                {
                    strFiles[i] = strPaths[i].Substring(strPaths[i].LastIndexOf('\\') + 1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                strFiles = null;
            }

            return strFiles;
        }


        //--------------------------------------------------------------------------
        /**    
        <summary>
        	Past File Delete 
        </summary>
        <param name="sPath"> File Directory </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/20 13:58
        */
        public static bool fn_DelDirFrDate(string sPath, DateTime tDelDate)
        {
            if(sPath == null) return false;
            if(sPath == ""  ) return false;

            DirectoryInfo di = new DirectoryInfo(sPath);

            if (!di.Exists) return false;

            foreach(FileInfo file in di.GetFiles()) 
            {
                if(file.CreationTime < tDelDate)
                {
                    file.Delete();
                }
            }
            di = null;
            return true;
        }
        //-------------------------------------------------------------------------------------------------
        //Folder Delete Function
        public static bool fn_DelFolderFrDate(string sPath, DateTime tDelDate)
        {
            if(sPath == null) return false;
            if(sPath == ""  ) return false;

            DirectoryInfo di = new DirectoryInfo(sPath);

            if (!di.Exists) return false;

            foreach(DirectoryInfo dir in di.GetDirectories())
            {
                if(dir.CreationTime < tDelDate)
                {
                    dir.Delete(true);
                }
            }
            di = null;
            return true;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Delete File Function
        </summary>
        <param name="sFileName"></param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/20 14:01
        */
        public static bool fn_FileDelete(string sFileName)
        {
            if(sFileName == null) return false;
            if(sFileName == ""  ) return false;

            FileInfo fi = new FileInfo(sFileName);
            bool bExist =  fi.Exists;
            if(!bExist) return false;
            fi.Delete();
            return true; 
        }





    }
}
