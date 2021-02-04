using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserClass;

//

namespace WaferPolishingSystem.BaseUnit
{
    public class LogUnit
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Log 종류 설정


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Var
        Queue<ST_LOG_INFO> m_Que = new Queue<ST_LOG_INFO>();

        ST_LOG_INFO stLog = new ST_LOG_INFO("");

        int    m_nLogStep   ;
        //int    m_nLogCount  ;
        //bool   m_bUseLogCnt ;

        private string m_sLogRoot ;
        private string m_sPremsg  ;
        private int    m_nLogCnt  ;

        public LogUnit()
        {
            //Log Folder Setting
            m_sLogRoot = UserFile.fn_GetExePath() + "LOG\\";

            m_nLogStep   = 0    ;
            //m_nLogCount  = 100  ;

            //m_bUseLogCnt = false;

            m_sPremsg = string.Empty;
            m_nLogCnt = 0;


            fn_MakeLogRoot  ();
            fn_DeleteOldFile();
            
        }
        //---------------------------------------------------------------------------
        //Make Log Folder Root 
        private void fn_MakeLogRoot()
        {
            string sPath = string.Empty;

            //Log Folder
            if (!Directory.Exists(m_sLogRoot))
            {
                Directory.CreateDirectory(m_sLogRoot);
            }

            if (!Directory.Exists(m_sLogRoot + "TRACE"     )) Directory.CreateDirectory(m_sLogRoot + "TRACE"     );
            if (!Directory.Exists(m_sLogRoot + "EVENT"     )) Directory.CreateDirectory(m_sLogRoot + "EVENT"     );
            if (!Directory.Exists(m_sLogRoot + "TRANSFER"  )) Directory.CreateDirectory(m_sLogRoot + "TRANSFER"  );
            if (!Directory.Exists(m_sLogRoot + "PMC"       )) Directory.CreateDirectory(m_sLogRoot + "PMC"       );
            if (!Directory.Exists(m_sLogRoot + "LOT"       )) Directory.CreateDirectory(m_sLogRoot + "LOT"       );
            if (!Directory.Exists(m_sLogRoot + "VISION"    )) Directory.CreateDirectory(m_sLogRoot + "VISION"    );
            if (!Directory.Exists(m_sLogRoot + "Exception" )) Directory.CreateDirectory(m_sLogRoot + "Exception" );
            if (!Directory.Exists(m_sLogRoot + "IO"        )) Directory.CreateDirectory(m_sLogRoot + "IO"        );
            if (!Directory.Exists(m_sLogRoot + "ALARM"     )) Directory.CreateDirectory(m_sLogRoot + "ALARM"     );
            if (!Directory.Exists(m_sLogRoot + "CRNT_STATE")) Directory.CreateDirectory(m_sLogRoot + "CRNT_STATE");
            if (!Directory.Exists(m_sLogRoot + "TEST"      )) Directory.CreateDirectory(m_sLogRoot + "TEST"      );
            if (!Directory.Exists(m_sLogRoot + "SMC"       )) Directory.CreateDirectory(m_sLogRoot + "SMC"       );
            if (!Directory.Exists(m_sLogRoot + "REST"      )) Directory.CreateDirectory(m_sLogRoot + "REST"      );


        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
            Update Log Function
        </summary>
        @author    정지완(JUNGJIWAN)
        @date      2020/01/31 09:48
        */

        public void fn_UpdateLog()
        {
            //
            switch (m_nLogStep)
            { 
                case 0:

                    if (m_Que.Count>0)
                    {
                        m_nLogStep++;
                        return;
                    }
                    
                    return; 

                case 1:
                    
                    //
                    stLog = m_Que.Dequeue();

                    //Logging
                    fn_WriteLog(stLog);

                    //m_fn_DisplayEvent(Data.strMsg); //List Box Display

                    m_nLogStep = 0;
                    return;

                default:
                    m_nLogStep = 0;
                    break;
            }

        }

        //---------------------------------------------------------------------------
        private void fn_WriteLog(ST_LOG_INFO log)
        {

            switch (log.eType)
            {
                case EN_LOG_TYPE.ltTrace : fn_Trace    (log); break;
                case EN_LOG_TYPE.ltEvent : fn_Event    (log);
                                           fn_PartEvent(log); break;
                case EN_LOG_TYPE.ltJam   : fn_JAM      (log);
                                           fn_Trace    (log); break;
                case EN_LOG_TYPE.ltPMC   : fn_PMC      (log); break;
                                                       
                case EN_LOG_TYPE.ltRS232 :                    break;
                case EN_LOG_TYPE.ltIO    : fn_IOLog    (log); break;
                case EN_LOG_TYPE.ltError :                    break;
                case EN_LOG_TYPE.ltVision: fn_Vision   (log); break;

                case EN_LOG_TYPE.ltLot   : fn_LOT      (log); break;
                case EN_LOG_TYPE.ltMill  : fn_MillLog  (log); break;

                case EN_LOG_TYPE.ltTest  : fn_TestLog  (log); break;
                case EN_LOG_TYPE.ltSMC   : fn_SMCLog   (log); break;
                case EN_LOG_TYPE.ltREST  : fn_RESTLog  (log); break;


                default:
                    break;
            }


        }
        //---------------------------------------------------------------------------
        public void fn_Trace(string msg)
        {
            ST_LOG_INFO temp = new ST_LOG_INFO("");

            temp.sMsg = msg; 
            temp.sdt  = string.Format($"{DateTime.Now:HH:mm:ss}");

            fn_Trace(temp);
        }
        //---------------------------------------------------------------------------
        private void fn_Trace(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] Trace.log");
            string sPath     = m_sLogRoot + "TRACE\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            if (m_sPremsg == msg)
            {
                if (++m_nLogCnt > 5)
                {
                    m_nLogCnt = 5;
                    return;
                }
            }
            else
            {
                m_nLogCnt = 0;
            }
 
            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();

                    m_sPremsg = msg;

                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_Trace>", ex);
            }

        }

        //---------------------------------------------------------------------------
        private void fn_Vision(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] Vision.log");
            string sPath = m_sLogRoot + "VISION\\" + sFileName;
            string msg = log.sMsg;
            string sTime = log.sdt;

            try
            {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_Vision>", ex);
            }

        }
        //---------------------------------------------------------------------------
        public void fn_SaveasCSV(string src,  string filename)
        {
            //Local Var.
            string sFrom     = src + filename ;
            string sTo       = string.Empty   ;
            string sPath     = "D:\\LogExport";
            string sFileName = filename.Substring(0, filename.Length - 4) + ".csv";

            //Check Folder
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }

            //
            sTo = sPath + "\\" + sFileName;

            try
            {
                File.Copy(sFrom, sTo);

                UserMsgBox.Show(string.Format($"File Export Ok - Path : {sTo}"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_SaveasCSV>", ex);
                
                UserMsgBox.Show(string.Format($"File Export Error - {ex.Message}"));
            }

        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Sequence Event Log
        </summary>
        <param name="ST_LOG_INFO"> Log Type</param>
        @author    정지완(JUNGJIWAN)
        @date      2020/04/01 18:47
        */
        private void fn_Event(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] Event.log");
            string sPath     = m_sLogRoot + "EVENT\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try
            {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_Event>", ex);
            }

        }
        //---------------------------------------------------------------------------
        private void fn_PartEvent(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp = log.iPart.ToString();
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] {sTemp}.log");
            string sPath     = m_sLogRoot + "EVENT\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try
            {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_PartEvent>", ex);
            }

        }

        //---------------------------------------------------------------------------
        private void fn_JAM(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] ALARM.log");
            string sPath     = m_sLogRoot + "ALARM\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try
            {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_ALARM>", ex);
            }
        }
        //---------------------------------------------------------------------------
        private void fn_PMC(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] PMC.log");
            string sPath     = m_sLogRoot + "PMC\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_PMC>", ex);
            }

        }
        //---------------------------------------------------------------------------
        private void fn_IOLog(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] IO.log");
            string sPath     = m_sLogRoot + "IO\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_IOLog>", ex);
            }

        }
        //---------------------------------------------------------------------------
        private void fn_LOT(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"{LOT._sLotNo}.log");
            string sPath     = m_sLogRoot + "LOT\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_LOT>", ex);
            }

        }
        //---------------------------------------------------------------------------
        private void fn_MillLog(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"{LOT._sLotNo}_Status.log");
            string sPath     = m_sLogRoot + "LOT\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_MillLog>", ex);
            }

        }
        //---------------------------------------------------------------------------
        private void fn_SMCLog(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] SMC.log");
            string sPath     = m_sLogRoot + "SMC\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_SMCLog>", ex);
            }

        }
        //---------------------------------------------------------------------------
        private void fn_RESTLog(ST_LOG_INFO log)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] REST.log");
            string sPath     = m_sLogRoot + "REST\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_RESTLog>", ex);
            }
        }

        //---------------------------------------------------------------------------
        private void fn_TestLog(ST_LOG_INFO log)
        {
            //Local Var.
            string sName = log.sFileName ; 
            string sTemp;
            //string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] TEST.log");
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}] {sName}.log");
            string sPath     = m_sLogRoot + "TEST\\" + sFileName;
            string msg       = log.sMsg; 
            string sTime     = log.sdt ;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{sTime}] {msg}\r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_TestLog>", ex);
            }

        }

        //---------------------------------------------------------------------------
        public void fn_WriteLog(string msg)
        {
            //
            fn_WriteLog(EN_LOG_TYPE.ltTrace, msg);
        }
        //---------------------------------------------------------------------------
        public void fn_WriteLog(EN_LOG_TYPE type , string msg, EN_SEQ_ID part = EN_SEQ_ID.None)
        {
            ST_LOG_INFO  temp = new ST_LOG_INFO("");
            temp.eType = type; 
            temp.sMsg  = msg ;
            temp.sdt   = string.Format($"{DateTime.Now:HH:mm:ss}");
            temp.iPart = part; 

            m_Que.Enqueue(temp);

            //
            fn_DisplayLog(msg);

        }
        //---------------------------------------------------------------------------
        public void fn_WriteTestLog(string msg, string filename)
        {
            ST_LOG_INFO temp = new ST_LOG_INFO("");
            temp.eType     = EN_LOG_TYPE.ltTest;
            temp.sMsg      = msg;
            temp.sdt       = string.Format($"{DateTime.Now:HH:mm:ss}");
            temp.sFileName = filename; 

            m_Que.Enqueue(temp);

            //
            //fn_DisplayLog(msg);

        }

        //---------------------------------------------------------------------------
        //Log Display Label
        private void fn_DisplayLog(string msg)
        {
            //Check Label Set 
        }
        //---------------------------------------------------------------------------
        public void fn_DeleteOldFile()
        {
            double dBackTime      = -30;  //Option.
            double dBackTimeImage = -90;  //

            UserFile.fn_DelDirFrDate   (m_sLogRoot + "EVENT\\"          , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "ALARM\\"          , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "TRANSFER\\"       , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "PMC\\"            , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "LOT\\"            , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "VISION\\"         , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "TRACE\\"          , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "IO\\"             , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "CRNT_STATE\\"     , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "TEST\\"           , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "SMC\\"            , DateTime.Now.AddDays(dBackTime));
            UserFile.fn_DelDirFrDate   (m_sLogRoot + "REST\\"           , DateTime.Now.AddDays(dBackTime)); //JUNG/201216

            //Vision Image
            UserFile.fn_DelFolderFrDate(g_VisionManager._strImageLogPath, DateTime.Now.AddDays(dBackTimeImage)); //JUNG/201130
        }
        //---------------------------------------------------------------------------
        public void ExceptionTrace(string Msg, Exception Ex)
        {
            ExceptionTrace(string.Format($"{Msg} {Ex.Message}\r\n{Ex.StackTrace}"));
        }
        //---------------------------------------------------------------------------
		public void ExceptionTrace(string Msg)
        {
            //Local Var.
            string sTemp;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}]Exception.txt");
            string sPath     = m_sLogRoot + "Exception\\" + sFileName;

            try {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sTemp = string.Format($"[{DateTime.Now:HH:mm:ss}] {Msg} \r\n");
                    sw.Write(sTemp);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                //ExceptionTrace("TLogUnit. ExceptionTrace " + ex.ToString());
            } 

        }
        //---------------------------------------------------------------------------
        public void fn_CrntStateTrace(EN_SEQ_ID part, string title)
        {
            //Local Var.
            string sTemp     = string.Empty ;
            string sMsg      = string.Empty ;
            string sFileName = string.Format($"[{DateTime.Now:yyMMdd}]CRNT_STATE.log");
            string sPath     = m_sLogRoot + "CRNT_STATE\\" + sFileName;

            try
            {
                //File Open.
                using (Stream stream = new FileStream(sPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                   
                    sMsg += string.Format($">>> {DateTime.Now:yyyy-MM-dd HH:mm:dd:ss} <<< \r\n");
                    sMsg += title + "\r\n";
                    
                    sMsg += "----------------------------------------------------------- \r\n";
                    if (part == EN_SEQ_ID.SPINDLE  || part == EN_SEQ_ID.ALL) { SEQ_SPIND.fn_SaveLog(ref sTemp); sMsg += sTemp; }
                    if (part == EN_SEQ_ID.POLISH   || part == EN_SEQ_ID.ALL) { SEQ_POLIS.fn_SaveLog(ref sTemp); sMsg += sTemp; }
                    if (part == EN_SEQ_ID.CLEAN    || part == EN_SEQ_ID.ALL) { SEQ_CLEAN.fn_SaveLog(ref sTemp); sMsg += sTemp; }
                    if (part == EN_SEQ_ID.STORAGE  || part == EN_SEQ_ID.ALL) { SEQ_STORG.fn_SaveLog(ref sTemp); sMsg += sTemp; }
                    if (part == EN_SEQ_ID.TRANSFER || part == EN_SEQ_ID.ALL) { SEQ_TRANS.fn_SaveLog(ref sTemp); sMsg += sTemp; }
                    sMsg += "----------------------------------------------------------- \r\n\r\n";

                    sw.Write(sMsg);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                ExceptionTrace("<fn_CrntStateTrace>", ex);
            }

        }
        //---------------------------------------------------------------------------





    }
}
