using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using static WaferPolishingSystem.FormMain;

using System.Windows.Controls;
using System.Windows.Media;


namespace WaferPolishingSystem.BaseUnit
{
    /**
    @class     STRAGE INFO.
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/02/03 18:58
    */
    public class Storage
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Variable
        #region [VARIABLE]

        string m_sStorName      ; 

	    //
        int    m_nStorId        ; // Storage Id
	    int    m_nMaxRow        ; //Row 
	    int    m_nMaxCol        ; //Col
	    int    m_nStorStat      ; //Storage State
	    int    m_nType          ;
	    int    m_nDirection     ;
        int    m_nRecipePinKind ; //

        bool m_bUseKind  ;  //사용자 지정 Tool 사용 여부

        //bool m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        //int  m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;

        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Pin[,] PINS ;


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public string _sStorName
        {
            get { return m_sStorName ; }
            //set { m_sStorName = value; }
        }
        public int _nStorId
        {
            get { return m_nStorId; }
        }
        

        public int _nRecipePinKind
        {
            get { return m_nRecipePinKind ; }
            set { m_nRecipePinKind = value; }
        }

        public int _nMaxRow { get { return m_nMaxRow; } set { m_nMaxRow = value; } }
        public int _nMaxCol { get { return m_nMaxCol; } set { m_nMaxCol = value; } }
        public int _nDir    { get { return m_nDirection; } set { m_nDirection = value; } }


        #endregion

        //---------------------------------------------------------------------------
        //생성자
        public Storage(int id)
        {
            m_sStorName      = STR_STORAGE_ID[id];

            m_nMaxRow        = 0;
            m_nMaxCol        = 0;
            m_nStorStat      = 0;
            m_nType          = 0;
            m_nDirection     = 0;
            m_nRecipePinKind = 0;
            
            m_nStorId        = id ;

            Init();
            
        }

        //---------------------------------------------------------------------------
        public void Init()
        {
            //
            PINS = new Pin[MAX_STORAGE_R, MAX_STORAGE_C];
            for (int i = 0; i< MAX_STORAGE_R; i++)
            {
                for (int j = 0; j < MAX_STORAGE_C; j++)
                {
                    PINS[i, j] = new Pin();
                }
            }

            //
            m_nMaxRow   = MAX_STORAGE_R;
            m_nMaxCol   = MAX_STORAGE_C;
            m_nStorStat = (int)EN_STOR_STAT.ssNone;

            SetTo((int)EN_PIN_STAT.psNone); //Pin Init
            ClearPos(); //Position Clear


            //Pin Kind 확인은 1.8에서만...
            m_bUseKind = (_MCTYPE == _MCTYPE_18); //Kind 사용 여부 설정


        }
        //---------------------------------------------------------------------------
        public Storage Copy()
        {
            return UserFunction.DeepClone(this) as Storage;
        }

        //---------------------------------------------------------------------------
        public void ClearMap()
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PINS[r,c].ClearMap();
                }
            }

            m_nStorStat = (int)EN_STOR_STAT.ssNone;
        }
        //---------------------------------------------------------------------------
        private bool CheckType(int stat)
        {
            //
            if (this.m_nType == (int)EN_STOR_TYPE.stPOLISH && stat == (int)EN_PIN_STAT.psNewCln) return false;
            if (this.m_nType == (int)EN_STOR_TYPE.stCLEAN  && stat == (int)EN_PIN_STAT.psNewPol) return false;

            return true; 

        }
        //---------------------------------------------------------------------------
        public void ClearPos()
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PINS[r, c].SetPosition(-1, -1);
                }
            }
        }
        //---------------------------------------------------------------------------

        //Get Pin Info
        public int GetPinStat(int r, int c) 
        {
    		if (r < 0 || r > MAX_STORAGE_R) return (int)EN_PIN_STAT.psNone;
    		if (c < 0 || c > MAX_STORAGE_C) return (int)EN_PIN_STAT.psNone;
    		return PINS[r,c].GetPinState();
    	}

        public bool IsOneExist(int r          ) 
        { 
    		for (int c = 0; c < m_nMaxCol; c++) 
    		{
    		    if (PINS[r,c].IsExist(    )) return true; 
    		} 
    		return false; 
    	}
        public bool IsOneStat (int r, int Stat) 
        { 
    		for (int c = 0; c < m_nMaxCol; c++) 
    		{ 
    			if (PINS[r,c].IsStat (Stat)) return true; 
    		} 
    		return false; 
    	}

        public bool IsOneExist(               ) 
        { 
    		for (int r = 0; r < m_nMaxRow; r++) 
    		{ 
    			if (IsOneExist(r      )) return true; 
    		} 
    		return false; 
        }
        public bool IsOneStat (int Stat       ) 
        { 
    		for (int r = 0; r < m_nMaxRow; r++) 
    		{ 
    			if (IsOneStat (r, Stat)) return true; 
    		} 
    		return false; 
        }

        //Check Pin Status
        public bool IsExist(int r, int c          ) 
    	{ 
    		if (r < 0 || r >= MAX_STORAGE_R) return false;
    		if (c < 0 || c >= MAX_STORAGE_C) return false;
    		return (PINS[r,c].IsExist(    )); 
    	}
        public bool IsStat (int r, int c, int Stat) 
    	{ 
    		if (r < 0 || r >= MAX_STORAGE_R) return false;
    		if (c < 0 || c >= MAX_STORAGE_C) return false;
    		return (PINS[r,c].IsStat (Stat)); 
    	}

        //Check All Chip Status.
        public bool IsAllExist() 
        { 
    		for (int r = 0; r < m_nMaxRow; r++) 
    		{ 
    			for (int c = 0; c < m_nMaxCol; c++) 
    			{ 
    				if (!PINS[r,c].IsExist(    )) return false; 
    			} 
    		}
    		return true; 
    	}
        public bool IsAllStat (int Stat) 
        { 
    		for (int r = 0; r < m_nMaxRow; r++) 
    		{ 
    			for (int c = 0; c < m_nMaxCol; c++) 
    			{ 
    				if (!PINS[r,c].IsStat (Stat)) return false; 
    			} 
    		} 
    		return true; 
    	}

        //Get Row Count by ChipStatus.
        public int GetCntExist(int r          ) 
    	{ 
    		int iCnt = 0; 
    		if (r < 0 || r >= MAX_STORAGE_R) r = 0;
    		for (int i = 0; i < MAX_STORAGE_C; i++)
    		{
    			if (PINS[r,i].IsExist()) iCnt++;
    		}
    		return iCnt;
    	}
        public int GetCntStat(int r, int Stat)
    	{
    		int iCnt = 0; if (r < 0 || r >= MAX_STORAGE_R) r = 0;
    		for (int i = 0; i < MAX_STORAGE_C; i++)
    		{
    			if (PINS[r,i].IsStat(Stat)) iCnt++; 
    		}
    		return iCnt;
    	}

        //Get All Count by ChipStatus.
        public int GetCntExist() 
    	{ 
    		int iCnt = 0; 
    		for (int i = 0; i < m_nMaxRow; i++)
    		{
    			iCnt = iCnt + GetCntExist(i);
    		}
    		return iCnt; 
    	}
        public int GetCntStat (int Stat) 
    	{ 
    		int iCnt = 0; 
    		for (int i = 0; i < m_nMaxRow; i++)
    		{
    			iCnt = iCnt + GetCntStat(i, Stat);
    		}
    
    		return iCnt; 
    	}
        //---------------------------------------------------------------------------
        //Get Pin Position Data
        public ST_PIN_POS GetPinPos(int r, int c) {

            ST_PIN_POS Pos = new ST_PIN_POS(-1);
    		
    		if (r < 0 || r > MAX_STORAGE_R) return Pos;
    		if (c < 0 || c > MAX_STORAGE_C) return Pos;
    		
    		Pos = PINS[r,c].GetPosition();
    		return Pos;
    	}
    

        //---------------------------------------------------------------------------
        public void SetTo(int Stat, int kind = 0)
        {
            //Check Type
            if (!CheckType(Stat)) return; 

            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PINS[r,c].SetTo(m_nType, Stat);

                    if (Stat == (int)EN_PIN_STAT.psNone)
                    {
                        PINS[r,c].SetKind(-1);
                    }
                    if (Stat == (int)EN_PIN_STAT.psNewPol) //Default
                    {
                        PINS[r, c].SetKind(kind);
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        public void SetTo(int r, int c, int Stat)
        {
            if (r >= MAX_STORAGE_R) return;
            if (c >= MAX_STORAGE_C) return;

            //Check Type
            if (!CheckType(Stat)) return;

            PINS[r,c].SetTo(m_nType, Stat);
            
            if (Stat == (int)EN_PIN_STAT.psNewPol) //Default
            {
                PINS[r, c].SetKind(0);
            }

        }
        //---------------------------------------------------------------------------
        public void SetTo(int r, int c, int Stat, int kind)
        {
            if (r >= MAX_STORAGE_R) return;
            if (c >= MAX_STORAGE_C) return;

            //Check Type
            if (!CheckType(Stat)) return;

            PINS[r, c].SetTo  (m_nType, Stat);
            PINS[r, c].SetKind(kind);

        }

        
        //---------------------------------------------------------------------------
        public void SetToRow(int c, int Stat)
        {
            if (c >= MAX_STORAGE_C) return;
            
            //Check Type
            if (!CheckType(Stat)) return;


            for (int r = 0; r < m_nMaxRow; r++)
            {
                PINS[r,c].SetTo(m_nType, Stat);
            }

        }
        public void SetToCol(int r, int Stat)
        {
            if (r >= MAX_STORAGE_R) return;

            //Check Type
            if (!CheckType(Stat)) return;


            for (int c = 0; c < m_nMaxCol; c++)
            {
                PINS[r, c].SetTo(m_nType, Stat);
            }

        }

        //---------------------------------------------------------------------------
        public void SetPos(int r, int c, double XPos, double YPos)
        {
            if (r >= MAX_STORAGE_R) return;
            if (c >= MAX_STORAGE_C) return;

            PINS[r,c].SetPosition(XPos, YPos);

        }
        //---------------------------------------------------------------------------
        public void SetColor(Color co1, Color co2, Color co3)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PINS[r, c].SetColor(co1, co2, co3);
                }
            }
        }
        //---------------------------------------------------------------------------
        public void SetMaxRowCol    (int r, int c) { m_nMaxRow = r; m_nMaxCol = c; }
        public void SetStorState    (int state   ) { m_nStorStat = state; }
        public void SetDirection    (int dir     ) { m_nDirection = dir; }
        public void SetType         (int type    ) { m_nType = type; }
        public void SetUseKind      (bool set    ) { m_bUseKind = set; }
        public void SetRecipePinKind(int kind    ) { m_nRecipePinKind = kind; } //Recipe Pin Kind Setting

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Pin Find Function
        </summary>
        <param name="FindMode"> Find Mode </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:05
        */
        public bool FindPin(int FindMode)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (FindPin(FindMode, r, c)) return true;
                }
            }
            return false;

        }
        //---------------------------------------------------------------------------
        public bool FindPin(int FindMode, int R, int C)
        {
            if (R < 0 || R >= m_nMaxRow) return false;
            if (C < 0 || C >= m_nMaxCol) return false;

            //Check Kind 
            if (m_bUseKind && m_nStorId == siPolish)
            {
                //Recipe Pin Kind
                int nSerchKind = 0; //JUNG/200612
                if (!PINS[R,C].IsPinKind(nSerchKind))
                {
                    return false;
                }
            }

            switch ((EN_PIN_FIND_MODE)FindMode)
            {
                case EN_PIN_FIND_MODE.fmExit  : return PINS[R,C].IsExist(                    );
                case EN_PIN_FIND_MODE.fmEmpty : return PINS[R,C].IsStat (EN_PIN_STAT.psEmpty );
                case EN_PIN_FIND_MODE.fmUsed  : return PINS[R,C].IsStat (EN_PIN_STAT.psUsed  );
                case EN_PIN_FIND_MODE.fmNewPol: return PINS[R,C].IsStat (EN_PIN_STAT.psNewPol);
                case EN_PIN_FIND_MODE.fmNewCln: return PINS[R,C].IsStat (EN_PIN_STAT.psNewCln);
                default:                        return false;
            }
        }
        //---------------------------------------------------------------------------
        public bool FindPin(int FindMode, int R, int C, int type)
        {
            if (R < 0 || R >= m_nMaxRow) return false;
            if (C < 0 || C >= m_nMaxCol) return false;

            //Check Kind 
            if (m_bUseKind && m_nStorId == siPolish)
            {
                //Recipe Pin Kind
                int nSerchKind = type; //JUNG/200612
                if (!PINS[R,C].IsPinKind(nSerchKind))
                {
                    return false;
                }
            }

            switch ((EN_PIN_FIND_MODE)FindMode)
            {
                case EN_PIN_FIND_MODE.fmExit  : return PINS[R,C].IsExist(                    );
                case EN_PIN_FIND_MODE.fmEmpty : return PINS[R,C].IsStat (EN_PIN_STAT.psEmpty );
                case EN_PIN_FIND_MODE.fmUsed  : return PINS[R,C].IsStat (EN_PIN_STAT.psUsed  );
                case EN_PIN_FIND_MODE.fmNewPol: return PINS[R,C].IsStat (EN_PIN_STAT.psNewPol);
                case EN_PIN_FIND_MODE.fmNewCln: return PINS[R,C].IsStat (EN_PIN_STAT.psNewCln);
                default:                        return false;
            }
        }

        //---------------------------------------------------------------------------
        public bool IsExtPolishingPin(int nToolKind)
        {
            //Check Tool Exist
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (PINS[r, c].IsPinKind(nToolKind))
                    {
                        if (PINS[r, c].IsStat(EN_PIN_STAT.psNewPol)) return true; 
                    }
                }
            }

            return false; 
        }


        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Find First Row, Col
        </summary>
        <param name="FindeMode"> Find Mode Set </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:12
        */
        public bool FindFirstRowCol(int FindMode, ref int R, ref int C)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (FindPin(FindMode, r, c))
                    {
                        R = r;
                        C = c;
                        return true;
                    }
                }
            }

            //
            R = -1;
            C = -1;
            return false;

        }
        public bool FindFirstRowCol(int FindMode, ref int R, ref int C, int type)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (FindPin(FindMode, r, c, type))
                    {
                        R = r;
                        C = c;
                        return true;
                    }
                }
            }

            //
            R = -1;
            C = -1;
            return false;

        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Find First Row, Col
        </summary>
        <param name="FindeMode"> Find Mode Set </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:12
        */
        public bool FindLastRowCol(int FindMode, ref int R, ref int C)
        {
            for (int r = m_nMaxRow - 1; r >= 0; r--)
            {
                for (int c = m_nMaxCol - 1; c >= 0; c--)
                {
                    if (FindPin(FindMode, r, c))
                    {
                        R = r;
                        C = c;
                        return true;
                    }
                }
            }

            //
            R = -1;
            C = -1;
            return false;
        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Find First Row
        </summary>
        <param name="FindMode">Find Mode Set</param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:14
        */
        public int FindFirstRow(int FindMode)
        {
            int iRow = 0;
            int iCol = 0;

            //
            FindFirstRowCol(FindMode, ref iRow, ref iCol);

            return iRow;
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Find First Col
        </summary>
        <param name="FindMode"> Find Mode Set </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:19
        */
        public int FindFirstCol(int FindMode)
        {
            int iRow = 0;
            int iCol = 0;

            //
            FindFirstRowCol(FindMode, ref iRow, ref iCol);

            return iCol;
        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	첫번재 Row의 마지막 Col 찾기
        </summary>
        <param name=""></param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:20
        */
        public bool FindFirstRowLastCol(int FindMode, ref int R, ref int C)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = m_nMaxCol - 1; c >= 0; c--)
                {
                    if (FindPin(FindMode, r, c))
                    {
                        R = r;
                        C = c;
                        return true;
                    }
                }
            }

            //
            R = -1;
            C = -1;
            return false;

        }

        //---------------------------------------------------------------------------
        /**    
        <summary>
        	마지막 Row의 첫번째 Col 위치 찾기
        </summary>
        <param name="FindMode"></param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:21
        */
        public bool FindLastRowFirstCol(int FindMode, ref int R, ref int C)
        {
            for (int r = m_nMaxRow - 1; r >= 0; r--)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (FindPin(FindMode, r, c))
                    {
                        R = r;
                        C = c;
                        return true;
                    }
                }
            }

            //
            R = -1;
            C = -1;
            return false;

        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Check Pin Kind of Recipe
        </summary>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:26
        */
        public bool CheckPinKind(int R, int C)
        {
            //Recipe Pin Kind
            
            return PINS[R,C].IsPinKind(m_nRecipePinKind);
        }
        //---------------------------------------------------------------------------
        public void fn_Load(bool bLoad, FileStream fp)
        {
            
            if (bLoad)
            {
                BinaryReader br = new BinaryReader(fp);

                m_sStorName  = br.ReadString();
                m_nStorStat  = br.ReadInt32 ();
                m_nDirection = br.ReadInt32 ();

            }
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);

                bw.Write(m_sStorName );
                bw.Write(m_nStorStat );
                bw.Write(m_nDirection);

                bw.Flush();
                
                
            }
            

            //
            fn_LoadPin(bLoad, fp);

        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	Load Pin Info
        </summary>
        <param name="bLoad"> Load Option </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/03 20:26
        */
        public void fn_LoadPin(bool bLoad, FileStream fp)
        {
            for (int r = 0; r < MAX_STORAGE_R; r++)
            {
                for (int c = 0; c < MAX_STORAGE_C; c++)
                {
                    PINS[r, c].fn_Load(bLoad, fp);
                }
            }

        }

        //---------------------------------------------------------------------------
        public void fn_UpdateMap(ref Label[,] label, bool withpos = false, bool showkind = false)
        {
            //
            if (label == null            ) return;
            if (m_nMaxRow > MAX_STORAGE_R) return;
            if (m_nMaxCol > MAX_STORAGE_C) return;

            int        iMaxX = m_nMaxCol;
            int        iMaxY = m_nMaxRow;
            int        iMinX = 0;
            int        iMinY = 0;
            ST_PIN_POS pos = new ST_PIN_POS(0);
            string     sContent = string.Empty ;

            //Label Name, Color Setting
            for (int r = iMinY; r < iMaxY; r++)
            {
                for (int c = iMinX; c < iMaxX; c++)
                {
                    label[r, c].Background = PINS[r, c].GetPinColor_B(m_nType);
                    sContent = string.Format($"{r * iMaxX + c + 1}");
                    
                    if (withpos)
                    {
                        pos = PINS[r, c].GetPosition();
                        sContent = sContent + string.Format($"\n {pos.dXPos:F4},{pos.dYPos:F4}");
                        label[r, c].ToolTip = sContent; 
                    }

                    if(showkind && PINS[r, c].IsExist())
                    {
                        //sContent = sContent + string.Format($"\n [{PINS[r, c].GetPinKind()}]"); 
                        int ntype = PINS[r, c].GetPinKind(); 
                        if (ntype >= 0)
                        {
                            sContent = sContent + string.Format($"\n [{FM.m_stSystemOpt.sToolType[ntype]}]");
                        }
                        else
                        {
                            sContent = sContent + string.Format("\n [-1]");
                        }
                    }

                    label[r, c].Content = sContent;
    }
            }

        }

    }
}