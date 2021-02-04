using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFile;
using static WaferPolishingSystem.FormMain;
using System.IO;
using static WaferPolishingSystem.BaseUnit.Magazine;
using System.Windows.Media;

namespace WaferPolishingSystem.BaseUnit
{
    /**
    @class     Data Manager Class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/02/04 10:14
    */
    public class DataManager
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Const
        public const int MAX_STORAGE_COUNT  = 2; //Polishing, Cleaning
        public const int MAX_MAGAZINE_COUNT = 6; //Polishing, Cleaning, PreAlign, Transfer, MAGA01, MAGA02

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Variable
        #region [VARIABLE]
        int m_nMaxStorage_Row ; //Storage Row Count
        int m_nMaxStorage_Col ; //Storage Col Count

        int m_nMaxMagazine_Row;
        int m_nMaxMagazine_Col;
        int m_nStotrage_Dir   ;

        int m_nMaxTool_Cnt    ; //Tool Count
        int m_nMaxPlate_Cnt   ; //Plate Count


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Storage[]  STOR;
        public Tool       TOOL;
        public Magazine[] MAGA;

//         public Storage[]  STOR_BF;
//         public Tool       TOOL_BF;
//         public Magazine[] MAGA_BF;


        //
        //static public DataManager DM = null;

        #region [Property]

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public int _nMaxStorage_Row
        {
            get { return m_nMaxStorage_Row ; }
            set { m_nMaxStorage_Row = value; }

        }
        public int _nMaxStorage_Col
        {
            get { return m_nMaxStorage_Col; }
            set { m_nMaxStorage_Col = value; }

        }
        public int _nMaxMagazine_Row
        {
            get { return m_nMaxMagazine_Row; }
            set { m_nMaxMagazine_Row = value; }

        }
        public int _nMaxMagazine_Col
        {
            get { return m_nMaxMagazine_Col; }
            set { m_nMaxMagazine_Col = value; }
        }
        
        public int _nMaxTool_Cnt
        {
            get { return m_nMaxTool_Cnt; }
            set { m_nMaxTool_Cnt = value; }

        }
        public int _nMaxPlate_Cnt
        {
            get { return m_nMaxPlate_Cnt; }
            set { m_nMaxPlate_Cnt = value; }

        }
        #endregion

        #endregion


        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public DataManager()
        {
            //
            //DM = this;

            //
            STOR    = new Storage[MAX_STORAGE_COUNT];
            //STOR_BF = new Storage[MAX_STORAGE_COUNT];

            for (int i = 0; i<STOR.Length;i++)
            {
                STOR   [i] = new Storage(i);
                //STOR_BF[i] = new Storage();
            }
            
            //
            TOOL    = new Tool();
            //TOOL_BF = new Tool();

            //
            MAGA    = new Magazine[MAX_MAGAZINE_COUNT];
            //MAGA_BF = new Magazine[MAX_MAGAZINE_COUNT];
            for (int i = 0; i < MAGA.Length; i++)
            {
                MAGA   [i] = new Magazine(i);
                //MAGA_BF[i] = new Magazine();
            }

            Init();
        }

        //---------------------------------------------------------------------------
        void Init()
        {
        	//Storage
        	m_nMaxStorage_Row  = MAX_STORAGE_R;
        	m_nMaxStorage_Col  = MAX_STORAGE_C;

            //Magazine
            m_nMaxMagazine_Row = MAX_MAGAZINE_R;
            m_nMaxMagazine_Col = MAX_MAGAZINE_C;

            //Tool
            m_nMaxTool_Cnt     = MAX_TOOL_COUNT; 
        	m_nMaxPlate_Cnt    = MAX_TOOL_PLATE;
        
        	//Init Storage
        	for (int s = 0; s < MAX_STORAGE_COUNT; s++)
        	{
        		STOR[s].Init();
        		STOR[s].SetMaxRowCol(m_nMaxStorage_Row, m_nMaxStorage_Col);
        	}
        
        	//Init Magazine
        	for (int m = 0; m < MAX_MAGAZINE_COUNT; m++)
        	{
        		MAGA[m].Init();
        		MAGA[m].SetMaxRowCol(m_nMaxMagazine_Row, m_nMaxMagazine_Col);
                MAGA[m].SetMagaName(m);


            }

        	TOOL.Init();
        	TOOL.SetMaxTool (m_nMaxTool_Cnt );
        	TOOL.SetMaxPlate(m_nMaxPlate_Cnt);
        		
        	//
        	fn_ClearMap();
        }

        //---------------------------------------------------------------------------
        /**    
        @brief     Map Clear 
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/18  12:21
        */
        public void fn_ClearMap()
        {
            fn_ClearMap_Stor();
            fn_ClearMap_Maga();
            fn_ClearMap_Tool();
        }
        //---------------------------------------------------------------------------
        public void fn_ClearMap_Stor()
        {
            for (int s = 0; s < MAX_STORAGE_COUNT; s++)
            {
                STOR[s].ClearMap();
            }
        }
        public void fn_ClearMap_Maga()
        {
            for (int m = 0; m < MAX_MAGAZINE_COUNT; m++)
            {
                MAGA[m].ClearMap();
            }
        }
        public void fn_ClearMap_Tool()
        {
            // 	for (int t = 0; t < MAX_TOOL_COUNT; t++)
            // 	{
            // 		TOOL[t].ClearMap();
            // 	}

            TOOL.ClearMap();
        }
        
        //---------------------------------------------------------------------------
        /**    
        @brief     Buffer Map All Clear
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/10/8  18:32
        */
        public void fn_ClearMapBuff()
        {
            //
            for (int s = 0; s < MAX_STORAGE_COUNT; s++)
            {
                //STOR_BF[s].ClearMap();
            }
            //
            for (int m = 0; m < MAX_MAGAZINE_COUNT; m++)
            {
                //MAGA_BF[m].ClearMap();
            }
            
            //
            //TOOL_BF.ClearMap();

        }

        //---------------------------------------------------------------------------
        /**    
        @brief     Row/Col Setting
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/18  12:25
        */
        public void fn_SetRowColInfo()
        {

            //Row, Col Setting
            if (FM.m_stProjectBase.nStorage_Row < 1) FM.m_stProjectBase.nStorage_Row = 1;
            if (FM.m_stProjectBase.nStorage_Col < 1) FM.m_stProjectBase.nStorage_Col = 1;
            m_nMaxStorage_Row  = FM.m_stProjectBase.nStorage_Row;
            m_nMaxStorage_Col  = FM.m_stProjectBase.nStorage_Col;

            if (FM.m_stProjectBase.nMagazine_Row < 1) FM.m_stProjectBase.nMagazine_Row = 1;
            if (FM.m_stProjectBase.nMagazine_Col < 1) FM.m_stProjectBase.nMagazine_Col = 1;
            m_nMaxMagazine_Row = FM.m_stProjectBase.nMagazine_Row;
            m_nMaxMagazine_Col = FM.m_stProjectBase.nMagazine_Col;

            m_nMaxTool_Cnt = 1 ;

            
            //Tool Color 
            //FM.m_stSystemOpt.brPinColor[0] = Brushes.CadetBlue;
            //FM.m_stSystemOpt.brPinColor[1] = Brushes.LightSteelBlue;
            //FM.m_stSystemOpt.brPinColor[4] = Brushes.CornflowerBlue;

            //Storage Info
            for (int s = 0; s < MAX_STORAGE_COUNT; s++)
            {
                STOR[s].SetMaxRowCol(m_nMaxStorage_Row, m_nMaxStorage_Col);
                STOR[s].SetDirection(FM.m_stSystemOpt.nStorDir           );  //STOR[s].SetDirection((int)EN_DIS_DIR.ddBtmLeft); //
                STOR[s].SetColor    (FM.m_stSystemOpt.brPinColor[0].Color, FM.m_stSystemOpt.brPinColor[1].Color, FM.m_stSystemOpt.brPinColor[4].Color); //JUNG/200625
            }
            STOR[(int)EN_STOR_ID.POLISH].SetType((int)EN_STOR_TYPE.stPOLISH);
            STOR[(int)EN_STOR_ID.CLEAN ].SetType((int)EN_STOR_TYPE.stCLEAN );

            //Magazine
            for (int m = 0; m < MAX_MAGAZINE_COUNT; m++)
            {
                if (m == (int)EN_MAGA_ID.MAGA01 || m == (int)EN_MAGA_ID.MAGA02)
                {
                    MAGA[m].SetMaxRowCol(m_nMaxMagazine_Row, m_nMaxMagazine_Col);
                }
                else MAGA[m].SetMaxRowCol(1, 1);
            }

            TOOL.SetMaxTool (MAX_TOOL_COUNT);
            TOOL.SetMaxPlate(MAX_TOOL_COUNT);

            //
            fn_WriteLog("Set Row/Col Info");
        }
        //---------------------------------------------------------------------------
        public void ShiftPinData(ref Pin From, ref Pin To)
        {
            //
            To = From.Copy();
            From.ClearMap();
        }
        public void CopyPinData(ref Pin From, ref Pin To)
        {
            //
            To = From.Copy();
        }
        //---------------------------------------------------------------------------
        public void ShiftMagaData(EN_MAGA_ID From, EN_MAGA_ID To)
        {
            ShiftMagaData((int) From, (int) To);
        }
        public void ShiftMagaData(int From, int To)
        {
            this.MAGA[To].Copy(this.MAGA[From]);
            this.MAGA[From].ClearMap();

        }
        public void CopyMagaData(int From, int To)
        {
            this.MAGA[To].Copy(this.MAGA[From]);
        }
        //---------------------------------------------------------------------------
        public void ShiftPlateData(ref Plate From, ref Plate To)
        {
            //
            To = From.Copy();
            From.ClearMap();
        }
        public void CopyPlateData(ref Plate From, ref Plate To)
        {
            //
            To = From.Copy();
        }
        //---------------------------------------------------------------------------
        public void ShiftPlateData(EN_MAGA_ID From, EN_MAGA_ID To)
        {
            if (From == EN_MAGA_ID.MAGA01 || From == EN_MAGA_ID.MAGA02) return;
            if (To   == EN_MAGA_ID.MAGA01 || To   == EN_MAGA_ID.MAGA02) return;

            ShiftPlateData(ref this.MAGA[(int)From].PLATE[0, 0], ref this.MAGA[(int)To].PLATE[0, 0]);
        }
        //---------------------------------------------------------------------------
        public void ShiftPlateData(EN_PLATE_ID From, EN_PLATE_ID To)
        {
            //
            int mMagaIdFr = (int)EN_MAGA_ID.miNone;
            int mMagaIdTo = (int)EN_MAGA_ID.miNone;

            if      (From == EN_PLATE_ID.ptiLoad  ) mMagaIdFr = (int)EN_MAGA_ID.LOAD  ;
            else if (From == EN_PLATE_ID.ptiPolish) mMagaIdFr = (int)EN_MAGA_ID.POLISH;
            else if (From == EN_PLATE_ID.ptiClean ) mMagaIdFr = (int)EN_MAGA_ID.CLEAN ;
            else if (From == EN_PLATE_ID.ptiMaga1 ) mMagaIdFr = (int)EN_MAGA_ID.MAGA01;
            else if (From == EN_PLATE_ID.ptiMaga2 ) mMagaIdFr = (int)EN_MAGA_ID.MAGA02;


            //if      (To   == EN_PLATE_ID.ptiLoad  ) mMagaIdTo = (int)EN_MAGA_ID.LOAD  ;
            if      (To   == EN_PLATE_ID.ptiPolish) mMagaIdTo = (int)EN_MAGA_ID.POLISH;
            else if (To   == EN_PLATE_ID.ptiClean ) mMagaIdTo = (int)EN_MAGA_ID.CLEAN ;
            else if (To   == EN_PLATE_ID.ptiLoad  ) mMagaIdTo = (int)EN_MAGA_ID.LOAD  ;
            else if (To   == EN_PLATE_ID.ptiMaga1 ) mMagaIdTo = (int)EN_MAGA_ID.MAGA01;
            else if (To   == EN_PLATE_ID.ptiMaga2 ) mMagaIdTo = (int)EN_MAGA_ID.MAGA02;


            if (To == EN_PLATE_ID.ptiSpindl)
            {
                ShiftPlateData(ref this.MAGA[mMagaIdFr].PLATE[0,0], ref this.TOOL.PLATES[0]);
                
            }
            else if (From == EN_PLATE_ID.ptiSpindl)
            {
                ShiftPlateData(ref this.TOOL.PLATES[0], ref this.MAGA[mMagaIdTo].PLATE[0, 0]);
            }
            else
            {
                ShiftPlateData(ref this.MAGA[mMagaIdFr].PLATE[0,0], ref this.MAGA[mMagaIdTo].PLATE[0, 0]);
            }

        }
        //---------------------------------------------------------------------------
        public void ShiftPickPlateData(EN_PLATE_ID From)
        {
            int mMagaIdFr = (int)EN_MAGA_ID.miNone;
        
            if      (From == EN_PLATE_ID.ptiLoad  ) mMagaIdFr = (int)EN_MAGA_ID.LOAD  ;
            else if (From == EN_PLATE_ID.ptiPolish) mMagaIdFr = (int)EN_MAGA_ID.POLISH;
            else if (From == EN_PLATE_ID.ptiClean ) mMagaIdFr = (int)EN_MAGA_ID.CLEAN ;
          //else if (From == EN_PLATE_ID.ptiUnLoad) mMagaIdFr = (int)EN_MAGA_ID.UNLOAD;
            else return;

            //
            ShiftPlateData(ref this.MAGA[mMagaIdFr].PLATE[0, 0], ref this.TOOL.PLATES[0]);

            //fn_WriteLog($"[MAP] Shift Plate : {mMagaIdFr.ToString()}  -> SPINDLE", EN_LOG_TYPE.ltEvent);

        }
        //---------------------------------------------------------------------------
        public void ShiftPlacePlateData(EN_PLATE_ID To)
        {
            //
            int mMagaIdTo = (int)EN_MAGA_ID.miNone;

          //if      (To   == EN_PLATE_ID.ptiLoad  ) mMagaIdTo = (int)EN_MAGA_ID.LOAD  ;
            if      (To   == EN_PLATE_ID.ptiPolish) mMagaIdTo = (int)EN_MAGA_ID.POLISH;
            else if (To   == EN_PLATE_ID.ptiClean ) mMagaIdTo = (int)EN_MAGA_ID.CLEAN ;
            else if (To   == EN_PLATE_ID.ptiLoad  ) mMagaIdTo = (int)EN_MAGA_ID.LOAD  ;
            else return;

            //
            ShiftPlateData(ref this.TOOL.PLATES[0], ref this.MAGA[mMagaIdTo].PLATE[0, 0]);

            //fn_WriteLog($"[MAP] Shift Plate : SPINDLE -> {mMagaIdTo.ToString()}", EN_LOG_TYPE.ltEvent);

            //Auto Data Change
            if (SEQ._bRun)
            {
                if (To == EN_PLATE_ID.ptiPolish) //Align --> Polishing : Load --> Align
                {
                    DM.MAGA[mMagaIdTo].SetTo((int)EN_PLATE_STAT.ptsAlign);
                }
            }
        }
        //---------------------------------------------------------------------------
        public void fn_SetPlateData(EN_MAGA_ID maga, PLATE_MOVE_INFO info, double pos = 0.0)
        {
            MAGA[(int)maga].PLATE[0, 0]._nFromRow  = info.nFindRow;
            MAGA[(int)maga].PLATE[0, 0]._nFromCol  = info.nFindCol;
            MAGA[(int)maga].PLATE[0, 0]._nFromMaga = info.nMagaId ;
            MAGA[(int)maga].PLATE[0, 0]._dPosition = pos          ;


        }
        //---------------------------------------------------------------------------
        public bool fn_IsMagaAllEmpty()
        {
            for (int i = 0; i < MAX_MAGAZINE_COUNT; i++)
            {
                if (!MAGA[i].IsAllEmpty()) return false; 
            }

            return true; 
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Map Data Save
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/18  13:33
        */
        public void fn_LoadMap(bool bLoad)
        {
            //Local Var.
            bool   bExist = false; 
            string sPath = fn_GetExePath();

            sPath += "SeqData";
            if (fn_CheckDir(sPath))
            {
                sPath += "\\DataMap.dat";

                bExist = fn_CheckFileExist(sPath);
                if (bLoad && !bExist) return; 
            }
            else return;

            try
            {
                FileStream fs = null;

                if (bLoad)
                {
                    fs = new FileStream(sPath, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    if (bExist) fs = new FileStream(sPath, FileMode.Open     , FileAccess.Write);
                    else        fs = new FileStream(sPath, FileMode.CreateNew, FileAccess.Write);

                }

                
                //Storage
                for (int s = 0; s < MAX_STORAGE_COUNT; s++)
                {
                    STOR[s].fn_Load(bLoad, fs);
                }

                //Magazine
                for (int m = 0; m < MAX_MAGAZINE_COUNT; m++)
                {
                    MAGA[m].fn_Load(bLoad, fs);

                }

                //Tool
                TOOL.fn_Load(bLoad, fs);


                //Buffer Data Update
                //if (bLoad) fn_BuffDataSet();

                fs.Close();

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        //---------------------------------------------------------------------------
        public void fn_BuffDataSet()
        {
/*            
        	for (int s = 0; s < MAX_STORAGE_COUNT; s++)
        	{
                STOR_BF[s] = STOR[s];

            }
        
        	//Init Magazine
        	for (int m = 0; m < MAX_MAGAZINE_COUNT; m++)
        	{
                MAGA_BF[m] = MAGA[m];
            }

            TOOL_BF = TOOL;
*/
        }
        //---------------------------------------------------------------------------




    }
}
