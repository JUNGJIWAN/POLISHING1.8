using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFile;
using static WaferPolishingSystem.Define.UserClass;

using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserINI;
using static WaferPolishingSystem.Define.UserEnum;

namespace WaferPolishingSystem.BaseUnit
{
    public class LotUnit
    {
        //
        bool     m_bLotOpen            ;
        bool     m_bLotEnded           ;
        bool     m_bReqLotEnd          ;
        bool     m_bLotCancel          ;
                 
        int      m_nWorkMode           ;
        int      m_nJamQty             ;
                 
        string   m_sRecipeName         ;
        string   m_sLotNo              ;
        string   m_sPartNo             ;
        string   m_sOperId             ;
        string   m_sLotType            ;
        string   m_sStartTime          ;
        string   m_sEndTime            ;
        string   m_sLogMsg             ;


        DateTime m_tStartTime          ;
        DateTime m_tEndTime            ;
        DateTime m_tJamTime            ;

        double   m_dLotStartTime, m_dLotEndTime;



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public bool _bLotOpen      { get { return m_bLotOpen   ; } }
        public string _sRecipeName { get { return m_sRecipeName; } }
        public string _sLotNo      { get { return m_sLotNo     ; } }

        //
        public LotUnit()
        {
            m_bLotOpen   = false;
            m_bLotEnded  = false;
            m_bReqLotEnd = false;
            m_bLotCancel = false;

            m_nWorkMode  = 0 ;
            m_nJamQty    = 0 ;

            m_tStartTime = DateTime.Now;
            m_tEndTime   = DateTime.Now;

            m_sStartTime = string.Empty; 
            m_sEndTime   = string.Empty;

            m_sLogMsg    = string.Empty;

            m_dLotStartTime = 0.0;
            m_dLotEndTime   = 0.0;

        }
        //---------------------------------------------------------------------------
        public void fn_LotOpenTest()
        {
            m_sRecipeName = "TEST_RECIPE";
            m_sLotNo      = "TEST_LOT";

            m_bLotOpen    = true;

            m_tStartTime = DateTime.Now;

            fn_WriteLog(string.Format($"[Lot Open] Recipe : {m_sRecipeName} / TEST RUN"));
        }

        //---------------------------------------------------------------------------
        public bool fn_LotOpen(string recipe, string lotno = "") //Lot No = RFID No
        {
            //Vision Recipe Open
            if (g_VisionManager.fn_LoadRecipe(recipe))
            {
                m_sRecipeName   = recipe;
                FM._sRecipeName = recipe; 

                m_bLotOpen = true;

                //
                SEQ_SPIND.fn_StepDataClear();


                if (lotno != "")
                {
                    m_sLotNo = string.Format($"[{DateTime.Now:yyMMdd}]LOT_{lotno}");
                }
                else
                {
                    m_sLotNo = string.Format($"[{DateTime.Now:yyMMdd}]LOT_{DateTime.Now:yyMMdd_HHmmss}") ;
                }

                m_tStartTime = DateTime.Now;

                fn_WriteLog(string.Format($"[Lot Open] Recipe : {m_sRecipeName} / LOT : {m_sLotNo}"));
                fn_WriteLog(string.Format($"[Lot Open] Recipe : {m_sRecipeName} / LOT : {m_sLotNo}"), EN_LOG_TYPE.ltLot);
                               
                m_dLotStartTime = TICK._GetTickTime();

                return true; 
            }

            m_bLotOpen = false;
            m_sLotNo   = string.Empty;

            return false;
        }
        //---------------------------------------------------------------------------
        public void fn_LotEnd()
        {
            m_tEndTime = DateTime.Now;

            //Lot Data Save...


            fn_WriteLog(string.Format($"[Lot End] Recipe : {m_sRecipeName} / LOT : {m_sLotNo}"), EN_LOG_TYPE.ltLot);

            m_dLotEndTime = TICK._GetTickTime();

            double dRunTotal = (double)TICK.ConvTimetoSec(m_dLotEndTime - m_dLotStartTime);

            m_sLogMsg = string.Format($"[Lot Run Time] {TICK.ConvTimeTickToStr(m_dLotEndTime - m_dLotStartTime)} / {dRunTotal} sec");
            fn_WriteLog(m_sLogMsg, EN_LOG_TYPE.ltLot);
            Console.WriteLine(m_sLogMsg);
            
            m_sLogMsg = string.Format($" -------------- UPH : {SPC.CalUPH(dRunTotal, 1).ToString("0.0000")} -------------- ");
            fn_WriteLog(m_sLogMsg, EN_LOG_TYPE.ltLot);
            Console.WriteLine(m_sLogMsg);

            //Clear Lot info
            m_bLotOpen    = false;
            m_sRecipeName = string.Empty;
            //m_sLotNo      = string.Empty;

        }
        //---------------------------------------------------------------------------
        public void fn_ForceLotEnd()
        {
            m_tEndTime = DateTime.Now;


            //Lot Data Save...








            fn_WriteLog(string.Format($"[Force Lot End] Recipe : {m_sRecipeName} / LOT : {m_sLotNo}"), EN_LOG_TYPE.ltLot);

            //Clear Lot info
            m_bLotOpen    = false;
            m_sRecipeName = string.Empty;
            //m_sLotNo      = string.Empty;


        }
        //---------------------------------------------------------------------------
        public bool fn_IsLotOpen() { return m_bLotOpen;  }
        //---------------------------------------------------------------------------
        public void fn_LoadLotInfo(bool bLoad)
        {
            string sIniPath = fn_GetExePath() + "SYSTEM\\LotInfo.ini";

	        if (bLoad)
	        {
	        	//Read
                m_bLotOpen       = fn_Load("LOT"  ,"bLotOpen   ", false  , sIniPath);
                m_bLotEnded      = fn_Load("LOT"  ,"bLotEnded  ", false  , sIniPath);
                m_bReqLotEnd     = fn_Load("LOT"  ,"bReqLotEnd ", false  , sIniPath);
                m_bLotCancel     = fn_Load("LOT"  ,"bLotCancel ", false  , sIniPath);
                
                m_nWorkMode      = fn_Load("LOT"  ,"nWorkMode  ", 0      , sIniPath);
                m_nJamQty        = fn_Load("LOT"  ,"nJamQty    ", 0      , sIniPath);
                
                m_sRecipeName    = fn_Load("LOT"  ,"sRecipeName", ""     , sIniPath);
                m_sLotNo         = fn_Load("LOT"  ,"sLotNo     ", ""     , sIniPath);
                m_sPartNo        = fn_Load("LOT"  ,"sPartNo    ", ""     , sIniPath);
                m_sOperId        = fn_Load("LOT"  ,"sOperId    ", ""     , sIniPath);
                m_sLotType       = fn_Load("LOT"  ,"sLotType   ", ""     , sIniPath);
                m_sStartTime     = fn_Load("LOT"  ,"sStartTime ", ""     , sIniPath);
                m_sEndTime       = fn_Load("LOT"  ,"sEndTime   ", ""     , sIniPath);

                m_dLotStartTime  = fn_Load("LOT"  ,"tStartTime ", 0.0    , sIniPath);
                m_dLotEndTime    = fn_Load("LOT"  ,"tEndTime   ", 0.0    , sIniPath);


            }
	        else
	        {
	        	//Write
                fn_Save("LOT"  ,"bLotOpen   ", m_bLotOpen      , sIniPath);
                fn_Save("LOT"  ,"bLotEnded  ", m_bLotEnded     , sIniPath);
                fn_Save("LOT"  ,"bReqLotEnd ", m_bReqLotEnd    , sIniPath);
                fn_Save("LOT"  ,"bLotCancel ", m_bLotCancel    , sIniPath);
                                                               
                fn_Save("LOT"  ,"nWorkMode  ", m_nWorkMode     , sIniPath);
                fn_Save("LOT"  ,"nJamQty    ", m_nJamQty       , sIniPath);
                                                               
                fn_Save("LOT"  ,"sRecipeName", m_sRecipeName   , sIniPath);
                fn_Save("LOT"  ,"sLotNo     ", m_sLotNo        , sIniPath);
                fn_Save("LOT"  ,"sPartNo    ", m_sPartNo       , sIniPath);
                fn_Save("LOT"  ,"sOperId    ", m_sOperId       , sIniPath);
                fn_Save("LOT"  ,"sLotType   ", m_sLotType      , sIniPath);
                fn_Save("LOT"  ,"sStartTime ", m_sStartTime    , sIniPath);
                fn_Save("LOT"  ,"sEndTime   ", m_sEndTime      , sIniPath);

                fn_Save("LOT"  ,"tStartTime ", m_dLotStartTime , sIniPath);
                fn_Save("LOT"  ,"tEndTime   ", m_dLotEndTime   , sIniPath);

	        }

        }


















    }
}
