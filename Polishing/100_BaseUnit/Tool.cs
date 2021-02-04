using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain;
using System.Windows.Controls;
using System.Windows.Media;

namespace WaferPolishingSystem.BaseUnit
{
    public struct TOOL_PICK_INFO
    {
        public bool bFind   ;

        public int nFindMode;
        public int nBtmStor ;
        public int nFindRow ;
        public int nFindCol ;

        public double dXpos ;
        public double dYpos ;

        public TOOL_PICK_INFO(bool find)
        {
            this.bFind     = find;
            this.nFindMode =  0;
            this.nBtmStor  = -1;
            this.nFindRow  = -1;
            this.nFindCol  = -1;
            this.dXpos     =  0;
            this.dYpos     =  0;
        }

    }

    public class Tool
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Variable
        #region [VARIABLE]
        //
        string m_sToolName  ;
                            
        int    m_nMaxPin    ;
        int    m_nMaxPlate  ;

        //bool   m_bExitCheck ;  //Tool Exist Check Flag
        //bool   m_bForceCheck;  //Force Calibration 
        bool   m_bNeedCheck ; //Tool Exist Check

        //bool   m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        //int    m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;

        //
        public Pin[]   PINS  ;
        public Plate[] PLATES;

        public TOOL_PICK_INFO PICK_INFO;



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public string _sToolName
        {
            get { return m_sToolName ; }
            set { m_sToolName = value; }
        }
        public int _nMaxPin
        {
            get { return m_nMaxPin ; }
            set { m_nMaxPin = value; }
        }
        public int _nMaxPlate
        {
            get { return m_nMaxPlate; }
            set { m_nMaxPlate = value; }
        }
        #endregion

        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public Tool()
        {
            //
            PINS   = new Pin  [MAX_TOOL_PIN  ];
            for(int i = 0; i < PINS.Length; i++)
            {
                PINS[i] = new Pin();
            }
            
            PLATES = new Plate[MAX_TOOL_PLATE];
            for (int i = 0; i < PLATES.Length; i++)
            {
                PLATES[i] = new Plate();
            }

            //Structure
            PICK_INFO = new TOOL_PICK_INFO(false);


            Init();
        }
        //---------------------------------------------------------------------------
        public void Copy(Tool Obj)
        {
            for (int i = 0; i < PINS.Length; i++)
            {
                PINS[i] = Obj.PINS[i].Copy();
            }

            for (int i = 0; i < PLATES.Length; i++)
            {
                PLATES[i] = Obj.PLATES[i].Copy();
            }
        }

        //---------------------------------------------------------------------------
        public void Init()
        {
            m_nMaxPin   = MAX_TOOL_PIN  ;
            m_nMaxPlate = MAX_TOOL_PLATE; 
	        
	        SetTo     ((int)EN_PIN_TYPE.ptNone, (int)EN_PIN_STAT.psNone); //State Init
	        SetToPlate((int)EN_PLATE_STAT.ptsNone); //Plate Init

            m_sToolName  = string.Empty;
            m_bNeedCheck = false;   

        }
        //---------------------------------------------------------------------------
        public void SetMaxTool (int Cnt) { m_nMaxPin = Cnt;   }
        public void SetMaxPlate(int Cnt) { m_nMaxPlate = Cnt; }
        //---------------------------------------------------------------------------
        //Pick Info Clear
        public void InitPickInfo(ref TOOL_PICK_INFO Info)
        {
            Info.bFind      = false;
            Info.nBtmStor   = -1;
            Info.nFindRow   = -1;
            Info.nFindCol   = -1;
            Info.nFindMode  = -1;
            Info.dXpos      = 0.0;
            Info.dYpos      = 0.0;

        }
        //---------------------------------------------------------------------------
        //Get Pin Info
        public int GetPinStat(int c = 0)
        {
            if (c < 0 || c > MAX_TOOL_PIN) return (int)EN_PIN_STAT.psNone;
            return PINS[c].GetPinState();
        }

        public bool IsOneExist()
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                if (PINS[c].IsExist()) return true;
            }
            return false;
        }
        public bool IsOneStat(int Stat)
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                if (PINS[c].IsStat(Stat)) return true;
            }
            return false;
        }

        //Check Pin Status
        public bool IsExist(int c = 0)
        {
            if (c < 0 || c >= m_nMaxPin) return false;
            return (PINS[c].IsExist());
        }
        public bool IsExistPol(int c = 0)
        {
            if (c < 0 || c >= m_nMaxPin) return false;
            return (PINS[c].IsExistPol());
        }

        public bool IsExistCln(int c = 0)
        {
            if (c < 0 || c >= m_nMaxPin) return false;
            return (PINS[c].IsExistCln());
        }

        public bool IsStat(int c, int Stat)
        {
            if (c < 0 || c >= m_nMaxPin) return false;
            return (PINS[c].IsStat(Stat));
        }

        //Check All Chip Status.
        public bool IsAllExist()
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                if (!PINS[c].IsExist()) return false;
            }

            return true;
        }
        public bool IsAllEmpty()
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                if (PINS[c].IsExist()) return false;
            }

            return true;
        }
        public bool IsEmpty()
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                if (!PINS[c].IsEmpty()) return false;
            }

            return true;
        }


        public bool IsAllStat(int Stat)
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                if (!PINS[c].IsStat(Stat)) return false;
            }
            return true;
        }

        public bool IsCheckForce() 
        {
            for (int i = 0; i < m_nMaxPin; i++)
            {
                if (!PINS[i].IsForceCheck()) return false;
            }

            return true; 
        }
        public bool IsCheckExist() 
        { 
            for (int i = 0; i< m_nMaxPin; i++)
            {
                if (!PINS[i].IsExistCheck()) return false;
            }

            return true; 
        }
        public bool IsNeedCheck()
        {
            return m_bNeedCheck;
        }

        //Get All Count by ChipStatus.
        public int GetCntExist()
        {
            int iCnt = 0;

            for (int i = 0; i < m_nMaxPin; i++)
            {
                if (PINS[i].IsExist()) iCnt++;
            }
            return iCnt;
        }
        public int GetCntStat(int Stat)
        {
            int iCnt = 0;

            for (int i = 0; i < m_nMaxPin; i++)
            {
                if (PINS[i].IsStat(Stat)) iCnt++;
            }
            return iCnt;
        }

        //---------------------------------------------------------------------------
        //Get Plate Info
        public int GetPlateStat(int c = 0)
        {
            if (c < 0 || c > MAX_TOOL_PLATE) return (int)EN_PLATE_STAT.ptsNone;
            return PLATES[c].GetPlateState();
        }
        //---------------------------------------------------------------------------
        public string GetPlateRecipeName()
        {
            return PLATES[0]._sRecipeName;
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Plate Status Setting
        </summary>
        <param name="Stat"> Set Status </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 09:10
        */
        public void SetToPlate(int Stat)
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                PLATES[c].SetTo(Stat);
            }
        }
        //---------------------------------------------------------------------------
        //Get Plate Info
        public bool IsPlateExist()
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                if (PLATES[c].IsExist()) return true;
            }
            return false;
        }
        public bool IsPlateEmpty()
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                if (!PLATES[c].IsEmpty()) return false;
            }
            return true;
        }
        
        public bool IsPlateOneStat(int Stat)
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                if (PLATES[c].IsStat(Stat)) return true;
            }
            return false;
        }
        //Check Plate Status
        public bool IsPlateExist(int c = 0)
        {
            if (c < 0 || c >= m_nMaxPlate) return false;
            return (PLATES[c].IsExist());
        }
        public bool IsPlateStat(int Stat, int c = 0)
        {
            if (c < 0 || c >= m_nMaxPlate) return false;
            return (PLATES[c].IsStat(Stat));
        }

        //Check All Status.
        public bool IsPlateAllExist()
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                if (!PLATES[c].IsExist()) return false;
            }
            return true;
        }
        public bool IsPlateAllStat(int Stat)
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                if (!PLATES[c].IsStat(Stat)) return false;
            }
            return true;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	PIN Status Setting
        </summary>
        <param name="c"> Col </param>
        <param name="Type"> Set Type </param>
        <param name="Stat"> Set State </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 09:17
        */
        public void SetTo(int c, int Type, int Stat)
        {
            if (c >= MAX_TOOL_PIN) return;

            PINS[c].SetTo(Type, Stat);

        }
        //---------------------------------------------------------------------------
        public void SetTo(int Type, int Stat)
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                PINS[c].SetTo(Type, Stat);
            }
        }
        //---------------------------------------------------------------------------
        public void SetTo(EN_PIN_TYPE Stat)
        {
            SetTo((int)Stat);
        }
        public void SetTo(int Stat)
        {
            int iType = (int)EN_PIN_TYPE.ptNone;
            
            if      (Stat == (int)EN_PIN_STAT.psNewPol || Stat == (int)EN_PIN_STAT.psUsedPol) iType = (int)EN_PIN_TYPE.ptPolis;
            else if (Stat == (int)EN_PIN_STAT.psNewCln || Stat == (int)EN_PIN_STAT.psUsedCln) iType = (int)EN_PIN_TYPE.ptClean;

            for (int c = 0; c < m_nMaxPin; c++)
            {
                PINS[c].SetTo(iType, Stat);
            }
        }
        public void SetExistCheck(bool set)
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                PINS[c].SetExistCheck(set);
            }
        }
        public void SetForceCheck(bool set)
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                PINS[c].SetForceCheck(set);
            }
        }
        public void SetNeedCheck(bool set)
        {
            m_bNeedCheck = set;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Clear Map Function
        </summary>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 09:22
        */
        public void ClearMap()
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                PINS[c].ClearMap();

            }

            for (int c = 0; c < m_nMaxPlate; c++)
            {
                PLATES[c].ClearMap();

            }

            m_sToolName = string.Empty;

            
            m_bNeedCheck = false;
        }

        public void ClearMapPlate()
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                PLATES[c].ClearMap();

            }
        }
        public void ClearMapPin()
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                PINS[c].ClearMap();
            }


            m_bNeedCheck = false;

        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Pin Find
        </summary>
        <param name="FindMode"> Find Mode Setting </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 09:24
        */
        public bool FindPin(int FindMode)
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                if (FindPin(FindMode, c)) return true;
            }
            return false;

        }
        //---------------------------------------------------------------------------
        public bool FindPin(int FindMode, int c)
        {
            if (c < 0 || c >= MAX_TOOL_PIN) return false;

            switch ((EN_PIN_FIND_MODE)FindMode)
            {                  
                case EN_PIN_FIND_MODE.fmExit    : return PINS[c].IsExist();
                case EN_PIN_FIND_MODE.fmEmpty   : return PINS[c].IsStat(EN_PIN_STAT.psEmpty  );
                case EN_PIN_FIND_MODE.fmUsed    : return PINS[c].IsStat(EN_PIN_STAT.psUsed   );
                case EN_PIN_FIND_MODE.fmNewPol  : return PINS[c].IsStat(EN_PIN_STAT.psNewPol );
                case EN_PIN_FIND_MODE.fmNewCln  : return PINS[c].IsStat(EN_PIN_STAT.psNewCln );
                //case EN_PIN_FIND_MODE.fmNeedChk : return PINS[c].IsStat(EN_PIN_STAT.psNeedChk);
                default:                          return false;
            }
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Find Pin from Tool
        </summary>
        <param name="FindMode"> Set Find Mode</param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 09:31
        */
        public bool FindFirst(EN_PIN_FIND_MODE FindMode, ref int C)
        {
            for (int i = 0; i < m_nMaxPin; i++)
            {
                if (FindPin((int)FindMode, i))
                {
                    C = i;
                    return true;
                }
            }
            C = -1;
            return false;
        }

        //---------------------------------------------------------------------------
        public TOOL_PICK_INFO GetPickInfoStorage(int Where, EN_PIN_FIND_MODE FindMode)
        {
            return GetPickInfoStorage(Where, (int)FindMode);
        }
        public TOOL_PICK_INFO GetPickInfoStorage(int Where, int FindMode)
        {
            TOOL_PICK_INFO Pick = new TOOL_PICK_INFO(false);

            //Init
            InitPickInfo(ref Pick);


            Pick.nFindMode = FindMode;
            Pick.nBtmStor  = Where;

            //Find Pin - First Row/Col
            if (DM.STOR[Where].FindFirstRowCol(Pick.nFindMode, ref Pick.nFindRow, ref Pick.nFindCol)) Pick.bFind = true;

            if (Pick.bFind)
            {

            }

            return Pick;
        }
        //---------------------------------------------------------------------------
        public TOOL_PICK_INFO GetPickInfoStorage(EN_PIN_FIND_MODE FindMode)
        {
            return GetPickInfoStorage((int)FindMode);
        }
        //---------------------------------------------------------------------------
        public TOOL_PICK_INFO GetPickInfoStorage(EN_PIN_FIND_MODE FindMode, int type)
        {
            return GetPickInfoStoragebyType((int)FindMode, type);
        }
        //---------------------------------------------------------------------------
        public TOOL_PICK_INFO GetPickInfoStorage(int FindMode)
        {
            int Where = 0;

            TOOL_PICK_INFO Pick = new TOOL_PICK_INFO(false);

            //Init
            InitPickInfo(ref Pick);

            if(_MCTYPE == _MCTYPE_18)
            {
                Where = -1;
                if      (FindMode == (int)EN_PIN_FIND_MODE.fmNewPol) Where = (int)EN_STOR_ID.POLISH;
                else if (FindMode == (int)EN_PIN_FIND_MODE.fmNewCln) Where = (int)EN_STOR_ID.CLEAN ;

                if (Where < 0) return Pick;
            }


            Pick.nFindMode = FindMode;
            Pick.nBtmStor  = Where   ;

            //Find Pin - First Row/Col <-- Option 처리 필요...
            if (DM.STOR[Where].FindFirstRowCol(Pick.nFindMode, ref Pick.nFindRow, ref Pick.nFindCol)) Pick.bFind = true;

            if (Pick.bFind)
            {
                //Tool Kind Check


            }

            return Pick;
        }
        //---------------------------------------------------------------------------
        public TOOL_PICK_INFO GetPickInfoStoragebyType(int FindMode, int type)
        {
            int Where = (int)EN_STOR_ID.POLISH;

            TOOL_PICK_INFO Pick = new TOOL_PICK_INFO(false);

            //Init
            InitPickInfo(ref Pick);

            Pick.nFindMode = FindMode;
            Pick.nBtmStor  = Where   ;

            //Find Pin
            if (DM.STOR[Where].FindFirstRowCol(Pick.nFindMode, ref Pick.nFindRow, ref Pick.nFindCol, type)) Pick.bFind = true;

            if (Pick.bFind)
            {
                //Tool Kind Check


            }

            return Pick;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Load Data 
        </summary>
        <param name="bLoad"> Load/Unload </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 09:54
        */
        private void fn_LoadPin(bool bLoad, FileStream fp)
        {
            for (int c = 0; c < m_nMaxPin; c++)
            {
                PINS[c].fn_Load(bLoad, fp);
            }
        }
        private void fn_LoadPlate(bool bLoad, FileStream fp)
        {
            for (int c = 0; c < m_nMaxPlate; c++)
            {
                PLATES[c].fn_Load(bLoad, fp);
            }
        }

        public void fn_Load(bool bLoad, FileStream fp)
        {
            if (bLoad)
            {

                BinaryReader br = new BinaryReader(fp);

                m_sToolName   = br.ReadString();

                m_nMaxPin     = br.ReadInt32();
                m_nMaxPlate   = br.ReadInt32();

                m_bNeedCheck  = br.ReadBoolean();
                

            }
            else
            {

                BinaryWriter bw = new BinaryWriter(fp);

                bw.Write(m_sToolName  );

                bw.Write(m_nMaxPin    );
                bw.Write(m_nMaxPlate  );

                bw.Write(m_bNeedCheck );


                bw.Flush();

            }
            

            //
            fn_LoadPin  (bLoad, fp);
            fn_LoadPlate(bLoad, fp);

        }
        //---------------------------------------------------------------------------
        public void fn_UpdateMap(ref Label lbPlate, ref Label lbPin, ref Label lbForce, ref Label lbNeed )
        {
            int iMaxPin   = m_nMaxPin  ;
            int iMaxPlate = m_nMaxPlate;
            int iMinPin   = 0;
            int iMinPlate = 0;
            //
            if (lbPlate == null) return;
            if (lbPin   == null) return;
            if (lbForce == null) return;
            if (lbNeed  == null) return;

            //Label Name, Color Setting

            lbPlate.Background = PLATES[0].GetPlateColor_B();
            lbPlate.Content    = STR_PLATE_STAT[PLATES[0]._nStat];
            
            lbPin.Background   = PINS[0].GetPinColor_B();
            lbPin.Content      = STR_PIN_STAT[PINS[0]._nStat];

            lbForce.Background = PINS[0].IsForceCheck() ? Brushes.Lime : Brushes.LightGray;
            lbNeed.Background  = IsNeedCheck() ? Brushes.Lime : Brushes.LightGray;

        }

    }
}
