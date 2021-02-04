using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserConst;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserClass;
using static WaferPolishingSystem.BaseUnit.ERRID;
using static WaferPolishingSystem.Define.UserFile;
using static WaferPolishingSystem.Define.UserINI;
using System.IO;

namespace WaferPolishingSystem.BaseUnit
{

    /***************************************************************************/
    /* Class: TSpcManger                                                       */
    /* Create:                                                                 */
    /* Developer:                                                              */
    /* Note:                                                                   */
    /***************************************************************************/

    //SPC Shift
    //===========================================================================
    public enum EN_DAY_SHIFT {
        All    ,
        ShiftGY ,
        ShiftDY ,
        ShiftSW
    };


    /***************************************************************************/
    /* Structures                                                              */
    /***************************************************************************/
    //Machine Efficiency.
    //===========================================================================
    public class TSPC_EFF 
    {
        public double dtInit      ;
        public double dtWarning   ;
        public double dtMCError   ;
        public double dtHMError   ;
        public double dtMLError   ;
        public double dtMDError   ;
        public double dtError     ;
        public double dtRunning   ;
        public double dtRunWarn   ;
        public double dtStop      ;
        public double dtMaint     ;
        public double dtIdle      ;
        public double dtPM        ;

        public int    iJamCnt     ;

        public TSPC_EFF()
        {
            ResetData();
        }
        public void ResetData()
        {
            dtInit         = 0.0; 
            dtWarning      = 0.0;
            dtMCError      = 0.0;
            dtHMError      = 0.0;
            dtMLError      = 0.0;
            dtMDError      = 0.0;
            dtError        = 0.0;
            dtRunning      = 0.0;
            dtRunWarn      = 0.0;
            dtStop         = 0.0;
            dtMaint        = 0.0;
            dtIdle         = 0.0;
            dtPM           = 0.0;

            iJamCnt        = 0;

        }
    };


    //Daily Data.
    //UserSet - ADD DAILY DATA
    //===========================================================================
    public class TDAILY_DATA {
        public int       iLoadQty    ;
        public int       iGoodQty    ;
        public int       iFailQty    ;
        public int       iJamQty     ;

        public double    dTotalTime  ;
        public double    dRunTime    ;
        public double    dErrorTime  ;
        public double    dDownTime   ;
        public double    dIdleTime   ;
        public double    dPMTime     ;

        public TDAILY_DATA()
        {
            ResetData();
        }
        public void ResetData()
        {
            iLoadQty    = 0  ;
            iGoodQty    = 0  ;
            iFailQty    = 0  ;
            iJamQty     = 0  ;
            dRunTime    = 0.0;
            dErrorTime  = 0.0;
            dDownTime   = 0.0;
            dIdleTime   = 0.0;
            dPMTime     = 0.0;
            dTotalTime  = 0.0; 
        }

    };

    public class SpcManager
    {
        //Vars.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //private:   /* Member Var.             */
        public  double   m_dSeqSrtTime       ;
        public  double   m_dDrngJamTime      ;
        public  double   m_dSeqEndTime       ;
        public  double   m_dDrngSeqTime      ;
        public  DateTime m_tDayChangeTime    ; //하루는 전날 pm10:00 ~ 금일 pm10:00
        public  DateTime m_tClearJamTime     ; //Jam All Clear Time.
        public  int      m_iLastJamNo        ;
        public  bool     m_bDayChanged       ;

        //private int      m_iDBSavePeriod     ;
        //private double   m_dJamCountInterval ;
        //private String   m_sDBForder         ;
        //private int      m_iDBClearPeriod    ;
        //private String   m_sDBClsDate        ;

        //Property.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Member Class 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public TSPC_EFF    SPC_EFF     = new TSPC_EFF   ();
        public TDAILY_DATA DAILY_DATA  = new TDAILY_DATA();


        //Method
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public SpcManager()
        {
            m_tDayChangeTime = DateTime.Parse("23:00:00");
            m_bDayChanged    = false;
            
            //
            m_dSeqSrtTime    = TICK._GetTickTime();
            m_dSeqEndTime    = TICK._GetTickTime();
            m_dDrngJamTime   = m_dDrngSeqTime = 0.0;
        }
        //---------------------------------------------------------------------------
        public void InitDB() 
        {//UserSet - Database Field 정의  
        
        }


        /***************************************************************************/
        /* Time Cal.                                                               */
        /***************************************************************************/
        public double ConvTimeDateTimeToSec(DateTime Time)
        {//Time.Ticks use
            return (Time.Ticks / 1000.0); //날짜를 초단위로 변경한다.
        }

        //---------------------------------------------------------------------------
        public string ConvTimeSecToStr(double second)
        {
            int Csec1, Csec2;
            string temp;

            Csec1   =  (int)second / 3600 ;
            second -= (Csec1  * 3600);
            Csec2   =  (int)second / 60   ;
            second -= (Csec2  * 60  );

            temp = string.Format("{0:00}:{1:00}:{2:00}" , Csec1 , Csec2 , (int)second);
            return temp;
        }

        //---------------------------------------------------------------------------
        public string ConvTimeTickToStr(double tickCnt)
        {
            int Hour, Minute , Sec;
            string temp;

  
            Hour     = (int)tickCnt / 3600000 ;
            tickCnt -= (Hour        * 3600000);
            Minute   = (int)tickCnt / 60000   ;
            tickCnt -= (Minute      * 60000  );
            Sec      = (int)tickCnt / 1000    ;
            tickCnt -= (Sec         * 1000   );

            temp = string.Format("{0:00}:{1:00}:{2:00}" , Hour , Minute , Sec);
            return temp;
        }
        //---------------------------------------------------------------------------
        public string  GetAvrScanTime(int iCnt, double dTotTime)
        {
            double tAvrScanTime;
            if(iCnt>0)   tAvrScanTime = dTotTime/iCnt;
            else         tAvrScanTime = dTotTime;

            return ConvTimeTickToStr(tAvrScanTime);
        }
        //---------------------------------------------------------------------------
        public  DateTime ChangeDateTime (DateTime SetDate, bool First , bool Section)
        {
            //Local Var.
            DateTime  tDate     = DateTime.Now;
            
            tDate = SetDate.Date;
            

  	        if (First) 
            { //시작 날짜 시간
                if (Section) tDate = tDate.AddDays(1);  
                else 
                {
                    if (DateTime.Now < m_tDayChangeTime) tDate = tDate.AddDays(-1); //오늘 22:00 이전은 어제 22:00 부터
                }
            }
            else 
            { //종료 날짜 시간
                if(!Section && DateTime.Now > m_tDayChangeTime)  tDate = tDate.AddDays(1);  
            }
            

            tDate = tDate.AddHours(m_tDayChangeTime.Hour);
            return tDate;

        }

        //---------------------------------------------------------------------------
        public DateTime ChangeDate(DateTime SetDate)
        {
            DateTime tDate = DateTime.Now;
            
            if (DateTime.Now < m_tDayChangeTime) 
            {
               return tDate = DateTime.Now;    //설정 시간 이전은 오늘
            }
            else 
            {
               return tDate.AddDays(1);
            }
            
            //return tDate;
        }
        //----------------------------------------------------------------------------
        public EN_DAY_SHIFT GetShift(DateTime tDateTime)
        {
            //Local Var.
 
            DateTime CTime ; CTime  = DateTime.Now;
            DateTime Time22; Time22 = DateTime.  Parse("22:00:00");
            DateTime Time06; Time06 = DateTime.  Parse("06:00:00");
            DateTime Time14; Time14 = DateTime.  Parse("14:00:00");

            //Cal. Shift.
            if      ((CTime >= Time22) || (CTime < Time06)) return EN_DAY_SHIFT.ShiftGY; //GY
            else if ((CTime >= Time06) && (CTime < Time14)) return EN_DAY_SHIFT.ShiftDY; //DY
            else if ((CTime >= Time14) && (CTime < Time22)) return EN_DAY_SHIFT.ShiftSW; //SW
            
            return EN_DAY_SHIFT.All;
        }
        //---------------------------------------------------------------------------
        public int CalUPH(string LotRunTime , int WorkCnt)
        {
            //Local Var.R
            double WorkTime =  DateTime.Parse(LotRunTime).Ticks;
            double Hour     =  WorkTime / 60.0 / 60.0 / 1000.0; //시 / 분 / 초 /

            //Check Error.
            if (WorkCnt <= 0 || Hour <= 0) return 0;

            //Cal UPH.
   	        return (int)((int)WorkCnt / Hour);
        }
        //---------------------------------------------------------------------------
        public int CalUPEH(double LotStrtTime , double LotEndTime , int WorkCnt)
        {
            //Local Var.
            string Temp = string.Empty;
                       

            double WorkTime = LotEndTime - LotStrtTime;
            double Hour     = WorkTime / 60.0 / 60.0 / 1000.0; //시 / 분 / 초 /

            //Check Error.
            if (WorkCnt <= 0 || Hour <= 0) return 0;
            

            //Cal UPH.
   	        return (int)(WorkCnt / Hour);
        }
        //---------------------------------------------------------------------------
        public double CalUPH(double dtSecScan, int iCnt)
        {
            //Local Var.
            if (dtSecScan <= 0) return 0.0;
            if (iCnt      <= 0) return 0.0;

            return (3600.0 * (double)iCnt) / ((double)dtSecScan);
        }

        //---------------------------------------------------------------------------
        public string CalMTBI(double RunTime , int JamCnt)
        {
            //Local Var.
            double dMTBI;
            string strMTBI = string.Empty;
            
            //Check Error.
            if (RunTime <= 0) return "0.0";
            if (JamCnt  <= 0) JamCnt = 1  ;

            //Cal UPH.
   	        dMTBI = RunTime / JamCnt;

            return ConvTimeSecToStr(dMTBI);            
        }


        //---------------------------------------------------------------------------
        public bool IsChangedDay(bool IsChkOnly = false)
        {
            //Local Var.
            int Hour1;
            int Hour2;

            //Set Time.
            Hour1 = m_tDayChangeTime.Hour;
            Hour2 = DateTime.Now.Hour;

            //Check Changing Day.
	        if (Hour1 <= Hour2) 
            {
                if (!IsChkOnly) 
                {
    	            if (!m_bDayChanged) 
                    {
             	        m_bDayChanged = true;
                        return true;
                    }
                }
                else return true;
            }
            else 
            {
    	        m_bDayChanged = false;
    	        return false;
            }
        
            return false;
        }
        //---------------------------------------------------------------------------
        public DateTime ChangeDay(DateTime CurrDate)
        {
            
            if(CurrDate.TimeOfDay > m_tDayChangeTime.TimeOfDay) 
            {//현재 시간이 설정 시간보다 큰 경우
               if(!m_bDayChanged) return  CurrDate.AddDays(1); //설정시간 이후는 내일
            }
            else 
            {//현재 시간이 설정 시간보다 작은 경우
               if(m_bDayChanged)  return CurrDate.AddDays(-1);  //설정시간 이전은 어제
            }
           
           return CurrDate;
        }
        //---------------------------------------------------------------------------
        public void InsDbJam(int iErrNo) 
        {
//             if(iErrNo < 0 || iErrNo >= MAX_ERROR) return;
// 
//             //Check Skip Error
//             if (!EPU.CheckWriteErr(iErrNo)) return;
//             
//             //Check Error No.
//             //if ( IsChkSameErr     (ErrNo)) return;
//             //if ( cDEF.FM.SysOptn .iSkipErrLog == 1) return; //iSkipErrLog 추가c
// 
//             //Server 전송하지 않는 Error는 DB 저장되지 않음.
//             if (!EPU[iErrNo].m_bOnAtRun                          ) return;
//             if ( EPU[iErrNo].m_iGrade != (int)EN_ERR_GRADE.Error ) return;
// 
// 
//             DateTime  STime   = cDEF.EPU.GetSetTimeDB(iErrNo);
// 
//             string    sDate    = string.Format("{0:yy/MM/dd}", STime       );
//             string    sMonth   = string.Format("{0:yy/MM}"   , STime       );
//             string    sStart   = string.Format("{0:HH:mm:ss}", STime       );
//             string    sEnd     = string.Format("{0:HH:mm:ss}", DateTime.Now);
//             string    sErrNo   = string.Format("E{0:0000}"   , iErrNo      );
//             string    sErrName = EPU.fn_GetName(iErrNo);
//             string    sLotNo   = "";//cDEF.LOT.Info.sLotNo;


            SPC_EFF   .iJamCnt ++;
            DAILY_DATA.iJamQty ++;

        }
        //---------------------------------------------------------------------------
        public void InsDbProd() 
        {
            string    sDate   = string.Format("{0:yy/MM/dd}", DateTime.Now.AddDays(-1));
            string    sMonth  = string.Format("{0:yy/MM}"   , DateTime.Now.AddDays(-1));
            string    sStart  = string.Format("{0:HH:mm:ss}", DateTime.Now.AddDays(-1));
            string    sEnd    = string.Format("{0:HH:mm:ss}", DateTime.Now.AddDays(-1));

        }                                           
        //---------------------------------------------------------------------------
        public void fn_Update(EN_SEQ_STATE iSeqStat) 
        {
            //if(SEQ._bRun) return;

            bool bLotOpen  = LOT._bLotOpen;
            bool bDoorOpen = SEQ.fn_IsAnyDoorOpen(true);

            //Set Start.
            m_dSeqSrtTime = TICK._GetTickTime();

            //Get During Seq Time.
            m_dDrngSeqTime = m_dSeqSrtTime - m_dSeqEndTime;


            EN_ERR_KIND iLastErrKind = (EN_ERR_KIND)EPU.fn_GetKind(EPU._nLastErr);


            //Clear. //86400000 == 24:00:00

            //Day Sequence 별 시간 증가.
            //dPMTime : PM Time / dDownTime : 비가동 / dRunTime : 가동
            
            
            switch (iSeqStat)
            {
                case EN_SEQ_STATE.RUNNING:
                case EN_SEQ_STATE.RUNWARN:
                                           
                    DAILY_DATA.dRunTime   += m_dDrngSeqTime;
                    DAILY_DATA.dTotalTime += m_dDrngSeqTime;
                    break;

                case EN_SEQ_STATE.INIT   :
                case EN_SEQ_STATE.WARNING:
                case EN_SEQ_STATE.STOP   :
                case EN_SEQ_STATE.IDLE   :
                    if (bLotOpen)
                    {
                        DAILY_DATA.dPMTime += m_dDrngSeqTime;
                    }
                    else
                    {
                        if (bDoorOpen)
                        {
                            DAILY_DATA.dPMTime += m_dDrngSeqTime;
                        }
                        else
                        {
                            DAILY_DATA.dDownTime += m_dDrngSeqTime;
                        }
                    }

                    DAILY_DATA.dTotalTime += m_dDrngSeqTime;
                    break;

                case EN_SEQ_STATE.ERROR  :
                    if (bLotOpen)
                    {
                        DAILY_DATA.dPMTime += m_dDrngSeqTime;
                    }
                    else
                    {
                        if (bDoorOpen)
                        {
                            DAILY_DATA.dPMTime += m_dDrngSeqTime;
                        }
                        else
                        {
                            DAILY_DATA.dDownTime += m_dDrngSeqTime;
                        }
                    }

                    DAILY_DATA.dErrorTime += m_dDrngSeqTime;
                    DAILY_DATA.dTotalTime += m_dDrngSeqTime;
                    break;
                
                default:
                    if (bLotOpen)
                    {
                        DAILY_DATA.dPMTime += m_dDrngSeqTime;
                    }
                    else
                    {
                        if (bDoorOpen)
                        {
                            DAILY_DATA.dPMTime += m_dDrngSeqTime;
                        }
                        else
                        {
                            DAILY_DATA.dDownTime += m_dDrngSeqTime;
                        }
                    }

                    DAILY_DATA.dTotalTime += m_dDrngSeqTime;
                    break;

            }

            //
            //DAILY_DATA.dTotalTime += m_dDrngSeqTime;


            //Set End Time.
            m_dSeqEndTime = TICK._GetTickTime();

            //Check Changing Time.
            if (!IsChangedDay()) return;

            string sLog = string.Empty;

            //string sLog = string.Format($"Day Change : MTBI= {CalMTBI(DAILY_DATA.dRunTime, DAILY_DATA.iJamQty)})");
            //fn_WriteLog(sLog);

            sLog = string.Format($"TOTAL TIME : {TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dTotalTime)} / RUN TIME : {TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dRunTime)} / DOWN TIME : {TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dDownTime)} / PM TIME : {TICK.ConvTimeTickToStr(SPC.DAILY_DATA.dPMTime)}");
            fn_WriteLog(sLog);

            //Kill Past Log
            LOG.fn_DeleteOldFile();

            SPC_EFF   .ResetData();
            DAILY_DATA.ResetData();

        }
        //---------------------------------------------------------------------------
        public void Load(bool isLoad) 
        {
            LoadDayData(isLoad);
            LoadTime   (isLoad);
        }
        //---------------------------------------------------------------------------
        public void LoadDayData(bool isLoad) 
        {
            string sFile = "DailyData";
            string sSection = sFile;
            string sRoot = fn_GetExePath() + "\\System\\SPC";

            if (!Directory.Exists(sRoot)) Directory.CreateDirectory(sRoot);

            string sIniPath = sRoot + "\\" + sFile + ".INI";

            //Load Work Quantity.
            if (isLoad) 
            {
                m_bDayChanged          = fn_Load(sSection, "DayChanged", false, sIniPath);
                DAILY_DATA.iLoadQty    = fn_Load(sSection, "LoadQty   ", 0    , sIniPath);
                DAILY_DATA.iGoodQty    = fn_Load(sSection, "GoodQty   ", 0    , sIniPath);
                DAILY_DATA.iFailQty    = fn_Load(sSection, "FailQty   ", 0    , sIniPath);
                DAILY_DATA.iJamQty     = fn_Load(sSection, "JamQty    ", 0    , sIniPath);
                
                DAILY_DATA.dRunTime    = fn_Load(sSection, "RunTime   ", 0.0  , sIniPath);
                DAILY_DATA.dErrorTime  = fn_Load(sSection, "ErrorTime ", 0.0  , sIniPath);
                DAILY_DATA.dDownTime   = fn_Load(sSection, "DownTime  ", 0.0  , sIniPath);
                DAILY_DATA.dIdleTime   = fn_Load(sSection, "IdleTime  ", 0.0  , sIniPath);
                DAILY_DATA.dPMTime     = fn_Load(sSection, "IdleTime  ", 0.0  , sIniPath);
                DAILY_DATA.dTotalTime  = fn_Load(sSection, "TotalTime ", 0.0  , sIniPath);
                
            }
            else 
            {
                fn_Save(sSection, "DayChanged", m_bDayChanged         , sIniPath);
                fn_Save(sSection, "LoadQty   ", DAILY_DATA.iLoadQty   , sIniPath);
                fn_Save(sSection, "GoodQty   ", DAILY_DATA.iGoodQty   , sIniPath);
                fn_Save(sSection, "FailQty   ", DAILY_DATA.iFailQty   , sIniPath);
                fn_Save(sSection, "JamQty    ", DAILY_DATA.iJamQty    , sIniPath);
                fn_Save(sSection, "RunTime   ", DAILY_DATA.dRunTime   , sIniPath);
                fn_Save(sSection, "ErrorTime ", DAILY_DATA.dErrorTime , sIniPath);
                fn_Save(sSection, "DownTime  ", DAILY_DATA.dDownTime  , sIniPath);
                fn_Save(sSection, "IdleTime  ", DAILY_DATA.dIdleTime  , sIniPath);
                fn_Save(sSection, "IdleTime  ", DAILY_DATA.dPMTime    , sIniPath);
                fn_Save(sSection, "TotalTime ", DAILY_DATA.dTotalTime , sIniPath);
            }
        }
        //---------------------------------------------------------------------------
        public void LoadTime(bool isLoad) 
        {

            string sFile = "EffTime";
            string sSection = sFile;
            string sRoot = fn_GetExePath() + "\\System\\SPC";

            if (!Directory.Exists(sRoot)) Directory.CreateDirectory(sRoot);

            string sIniPath = sRoot + "\\" + sFile + ".INI";


            //Load Work Quantity.
            if (isLoad) 
            {
                SPC_EFF.dtInit    = fn_Load(sSection, "Init      ", 0.0, sIniPath);
                SPC_EFF.dtWarning = fn_Load(sSection, "Warning   ", 0.0, sIniPath);
                SPC_EFF.dtMCError = fn_Load(sSection, "MCError   ", 0.0, sIniPath);
                SPC_EFF.dtHMError = fn_Load(sSection, "HMError   ", 0.0, sIniPath);
                SPC_EFF.dtMLError = fn_Load(sSection, "MLError   ", 0.0, sIniPath);
                SPC_EFF.dtMDError = fn_Load(sSection, "MDError   ", 0.0, sIniPath);
                SPC_EFF.dtError   = fn_Load(sSection, "Error     ", 0.0, sIniPath);
                SPC_EFF.dtRunning = fn_Load(sSection, "Running   ", 0.0, sIniPath);
                SPC_EFF.dtRunWarn = fn_Load(sSection, "RunWarn   ", 0.0, sIniPath);
                SPC_EFF.dtStop    = fn_Load(sSection, "Stop      ", 0.0, sIniPath);
                SPC_EFF.dtMaint   = fn_Load(sSection, "Maint     ", 0.0, sIniPath);
                SPC_EFF.dtIdle    = fn_Load(sSection, "Idle      ", 0.0, sIniPath);
                SPC_EFF.dtPM      = fn_Load(sSection, "PM        ", 0.0, sIniPath);
                
                SPC_EFF.iJamCnt   = fn_Load(sSection, "JamCnt    ", 0  , sIniPath);
            }
            else 
            {
                fn_Save(sSection, "Init      ", SPC_EFF.dtInit   , sIniPath);
                fn_Save(sSection, "Warning   ", SPC_EFF.dtWarning, sIniPath);
                fn_Save(sSection, "MCError   ", SPC_EFF.dtMCError, sIniPath);
                fn_Save(sSection, "HMError   ", SPC_EFF.dtHMError, sIniPath);
                fn_Save(sSection, "MLError   ", SPC_EFF.dtMLError, sIniPath);
                fn_Save(sSection, "MDError   ", SPC_EFF.dtMDError, sIniPath);
                fn_Save(sSection, "Error     ", SPC_EFF.dtError  , sIniPath);
                fn_Save(sSection, "Running   ", SPC_EFF.dtRunning, sIniPath);
                fn_Save(sSection, "RunWarn   ", SPC_EFF.dtRunWarn, sIniPath);
                fn_Save(sSection, "Stop      ", SPC_EFF.dtStop   , sIniPath);
                fn_Save(sSection, "Maint     ", SPC_EFF.dtMaint  , sIniPath);
                fn_Save(sSection, "Idle      ", SPC_EFF.dtIdle   , sIniPath);
                fn_Save(sSection, "PM        ", SPC_EFF.dtPM     , sIniPath);
                
                fn_Save(sSection, "JamCnt    ", SPC_EFF.iJamCnt  , sIniPath);
            }
        }

    }
}
