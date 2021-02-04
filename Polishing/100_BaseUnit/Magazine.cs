using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;
using System.IO;
using System.Windows.Controls;

namespace WaferPolishingSystem.BaseUnit
{
    /**
    @class     Magazine Class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/02/04 10:50
    */
    public class Magazine
    {
        public struct PLATE_MOVE_INFO
        {
            public bool bFind    ;

            public int nFindMode ;
            public int nBtmPlate ;
            public int nRcpNo    ;

            public double dXpos  ;
            public double dYpos  ;
            
            public int nFindRow  ;
            public int nFindCol  ;
            public int nMagaId   ;


            public PLATE_MOVE_INFO(bool find)
            {
                this.bFind     = find;

                this.nFindMode = 0;
                this.nBtmPlate = 0;
                this.nRcpNo    = 0;

                this.nFindRow  = -1;
                this.nFindCol  = -1;
                this.nMagaId   = -1;

                this.dXpos     = 0.0;
                this.dYpos     = 0.0;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Variable
        #region [VARIABLE]

        string m_sMagaName    ;

        //
        int    m_nMagaId      ;
        //
        int    m_nMaxRow      ;   //Row 
        int    m_nMaxCol      ;   //Col
        int    m_nMagazineStat;   //State
        int    m_nDirection   ;

        bool   m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        int    m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;
        double m_dSpare1, m_dSpare2, m_dSpare3, m_dSpare4, m_dSpare5;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Plate[,] PLATE;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public int _nMaxRow
        {
            get { return m_nMaxRow; }
            set { m_nMaxRow = value; }
        }
        public int _nMaxCol
        {
            get { return m_nMaxCol; }
            set { m_nMaxCol = value; }
        }
        public int _nMagazineStat
        {
            get { return m_nMagazineStat; }
            set { m_nMagazineStat = value; }
        }
        public int _nDirection
        {
            get { return m_nDirection; }
            set { m_nDirection = value; }
        }


        #endregion

        /************************************************************************/
        /* 생성자                                                                */
        /************************************************************************/
        public Magazine(int id)
        {
            //
            m_nMagaId = id; 

            //
            PLATE = new Plate[MAX_MAGAZINE_R, MAX_MAGAZINE_C];
            for (int i =0; i< MAX_MAGAZINE_R; i++)
            {
                for (int j = 0; j < MAX_MAGAZINE_C; j++)
                {
                    PLATE[i,j] = new Plate();
                }
            }

            //
            Init();
        }
        //---------------------------------------------------------------------------
        public void Copy(Magazine Obj)
        {
            for (int i = 0; i < MAX_MAGAZINE_R; i++)
            {
                for (int j = 0; j < MAX_MAGAZINE_C; j++)
                {
                    PLATE[i, j] = Obj.PLATE[i,j].Copy();
                }
            }

        }

        //---------------------------------------------------------------------------
        public void Init()
        {
            m_nMaxRow       = MAX_MAGAZINE_R;
            m_nMaxCol       = MAX_MAGAZINE_C;
            m_nMagazineStat = (int)EN_MAGA_STAT.mzsNone;

            m_sMagaName     = string.Empty;
            m_nDirection    = 0;

            SetTo((int)EN_PLATE_STAT.ptsEmpty); //State Init
            ClearPos(); //Position Clear

            //
            m_bSpare1 = m_bSpare2 = m_bSpare3 = m_bSpare4 = m_bSpare5 = false;
            m_nSpare1 = m_nSpare2 = m_nSpare3 = m_nSpare4 = m_nSpare5 = 0;
            m_dSpare1 = m_dSpare2 = m_dSpare3 = m_dSpare4 = m_dSpare5 = 0.0;
        }
        //---------------------------------------------------------------------------
        /**    
        @brief     Storage, Pin 상태 Clear
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  12:05
        */
        public void ClearMap()
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PLATE[r, c].ClearMap();
                }
            }
            
            m_sMagaName     = string.Empty;
            m_nMagazineStat = (int)EN_MAGA_STAT.mzsNone;

            ClearPos(); //Position Clear
        }
        //---------------------------------------------------------------------------
        public void SetMaxRowCol(int r, int c) 
        {
            m_nMaxRow = r; 
            m_nMaxCol = c; 
        }
        //---------------------------------------------------------------------------
        public void SetMagaName(int m)
        {
            m_sMagaName = STR_MAGAZINE_NAME[m];
        }

        public void SetMagazineState(int state) 
        { 
            m_nMagazineStat = state; 
        }
        //---------------------------------------------------------------------------
        public int GetFromMaga()
        {
            return PLATE[0, 0]._nFromMaga;
        }
        public int GetFromRow()
        {
            return PLATE[0, 0]._nFromRow;
        }
        public int GetFromCol()
        {
            return PLATE[0, 0]._nFromCol;
        }
        public double GetFromPos()
        {
            return PLATE[0, 0]._dPosition;
        }

        //---------------------------------------------------------------------------
        /**
        @brief     Pin 상태 Setting
        @return
        @param
        @remark
        -
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  12:05
        */
        public void SetTo(int r, int c, int Stat)
        {
            if (r >= m_nMaxRow) return;
            if (c >= m_nMaxCol) return;

            PLATE[r,c].SetTo(Stat);

        }
        public void SetTo(int r, int Stat)
        {
            if (r >= MAX_MAGAZINE_R) return;

            for (int c = 0; c < m_nMaxCol; c++)
            {
                PLATE[r, c].SetTo(Stat);
            }

        }
        //---------------------------------------------------------------------------
        public void SetTo(int Stat)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PLATE[r,c].SetTo(Stat);
                }
            }
        }
        //---------------------------------------------------------------------------
        public void ClearPos()
        {

            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PLATE[r,c].SetPosition(0);
                }
            }
        }
        //---------------------------------------------------------------------------
        public void SetInfo(EN_PLATE_INFO info)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    PLATE[r, c].SetInfo(info);
                }
            }
        }


        //---------------------------------------------------------------------------
        //Get Pin Info
        public int GetPlateStat(int r, int c)
        {
            if (r < 0 || r > MAX_MAGAZINE_R) return (int)EN_PLATE_STAT.ptsNone;
            if (c < 0 || c > MAX_MAGAZINE_C) return (int)EN_PLATE_STAT.ptsNone;

            return PLATE[r,c].GetPlateState();
        }

        public bool IsOneExist(int r)
        {
            for (int c = 0; c < m_nMaxCol; c++)
            {
                if (PLATE[r,c].IsExist()) return true;
            }
            return false;
        }
        public bool IsOneStat(int r, int Stat)
        {
            for (int c = 0; c < m_nMaxCol; c++)
            {
                if (PLATE[r,c].IsStat(Stat)) return true;
            }
            return false;
        }

        public bool IsOneExist()
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                if (IsOneExist(r)) return true;
            }
            return false;
        }
        public bool IsOneStat(int Stat)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                if (IsOneStat(r, Stat)) return true;
            }
            return false;
        }

        //Check Plate Status
        public bool IsExist(int r, int c)
        {
            if (r < 0 || r >= MAX_MAGAZINE_R) return false;
            if (c < 0 || c >= MAX_MAGAZINE_C) return false;
            return (PLATE[r,c].IsExist());
        }
        //---------------------------------------------------------------------------
        public bool IsStatOne(EN_PLATE_STAT Stat)
        {
            return IsStatOne((int)Stat);
        }
        public bool IsStatOne(int Stat)
        {
            return (PLATE[0, 0].IsStat(Stat));
        }
        //---------------------------------------------------------------------------
        public bool IsStat(int r, int c, int Stat)
        {
            if (r < 0 || r >= MAX_MAGAZINE_R) return false;
            if (c < 0 || c >= MAX_MAGAZINE_C) return false;
            return (PLATE[r,c].IsStat(Stat));
        }

        //Check All Chip Status.
        public bool IsAllExist()
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (!PLATE[r,c].IsExist()) return false;
                }
            }
            return true;
        }
        public bool IsAllEmpty()
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (!PLATE[r,c].IsEmpty()) return false;
                }
            }
            return true;
        }
        public bool IsExistReady()
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (PLATE[r, c].IsExistReady()) return true;
                }
            }
            return false;
        }
        
        public bool IsAllStat(int Stat)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (!PLATE[r,c].IsStat(Stat)) return false;
                }
            }
            return true;
        }

        //Get Row Count by plate Status.
        public int GetCntExist(int r)
        {
            int iCnt = 0;
            if (r < 0 || r >= m_nMaxRow) r = 0;
            for (int i = 0; i < m_nMaxCol; i++)
            {
                if (PLATE[r,i].IsExist()) iCnt++;
            }
            return iCnt;
        }
        public int GetCntStat(int r, int Stat)
        {
            int iCnt = 0;
            if (r < 0 || r >= MAX_MAGAZINE_R) r = 0;
            for (int i = 0; i < MAX_MAGAZINE_C; i++)
            {
                if (PLATE[r,i].IsStat(Stat)) iCnt++;

            }
            return iCnt;
        }

        //Get All Count by ChipStatus.
        public int GetCntExist()
        {
            int iCnt = 0;
            for (int i = 0; i < m_nMaxRow; i++) iCnt = iCnt + GetCntExist(i);
            return iCnt;
        }
        public int GetCntStat(int Stat)
        {
            int iCnt = 0;
            for (int i = 0; i < m_nMaxRow; i++)
            {
                iCnt = iCnt + GetCntStat(i, Stat);
            }
            return iCnt;
        }

        //Get Pin Position Data
        public double GetPlatePos(int r, int c)
        {
            if (r < 0 || r > MAX_MAGAZINE_R) return -1;
            if (c < 0 || c > MAX_MAGAZINE_C) return -1;
            return PLATE[r,c].GetPosition();
        }
        //---------------------------------------------------------------------------
        public string GetPlateRecipeName()
        {
            return PLATE[0, 0]._sRecipeName;
        }
        //-------------------------------------------------------------------------------------------------
        public string GetPlateRecipeName(int row)
        {
            return PLATE[row, 0]._sRecipeName;
        }
        //---------------------------------------------------------------------------
        public string GetPlateId()
        {
            return PLATE[0, 0]._sPlateId;
        }


        //Search Pin.
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool FindPlate(int FindMode)
        {
        	for (int r = 0; r < m_nMaxRow; r++)
        	{
        		for (int c = 0; c < m_nMaxCol; c++)
        		{
        			if (FindPlate(FindMode, r, c)) return true;
        		}
        	}
        	return false; 
        
        }
        //---------------------------------------------------------------------------
        public bool FindPlate(int FindMode, int R, int C)
        {
        	if (R < 0 || R >= m_nMaxRow) return false; 
        	if (C < 0 || C >= m_nMaxCol) return false;
        
        	switch ((EN_PLATE_FIND_MODE)FindMode)
        	{
        	    case EN_PLATE_FIND_MODE.fmpReady  : return PLATE[R,C].IsExist();
        	    case EN_PLATE_FIND_MODE.fmpEmpty  : return PLATE[R,C].IsStat((int)EN_PLATE_STAT.ptsEmpty );
        		case EN_PLATE_FIND_MODE.fmpFinish : return PLATE[R,C].IsStat((int)EN_PLATE_STAT.ptsFinish);
        		case EN_PLATE_FIND_MODE.fmpAlign  : return PLATE[R,C].IsStat((int)EN_PLATE_STAT.ptsAlign );
        		case EN_PLATE_FIND_MODE.fmpPolish : return PLATE[R,C].IsStat((int)EN_PLATE_STAT.ptsPolish);
        		case EN_PLATE_FIND_MODE.fmpWashing: return PLATE[R,C].IsStat((int)EN_PLATE_STAT.ptsClean );
        		case EN_PLATE_FIND_MODE.fmpSkip   : return false; 
        		default                           : return false;
        			
        	}
        }
        
        //---------------------------------------------------------------------------
        /**    
        @brief     첫번째 Row, Col Search
        @return    first Row, Col
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  12:03
        */
        public bool FindFirstRowCol(int FindMode, ref int R, ref int C)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (FindPlate(FindMode, r, c))
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
        @brief     마지막 Row, Col Search
        @return    Last Row, Last Col
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  12:02
        */
        public bool FindLastRowCol(int FindMode, ref int R, ref int C)
        {
            for (int r = m_nMaxRow - 1; r >= 0; r--)
            {
                for (int c = m_nMaxCol - 1; c >= 0; c--)
                {
                    if (FindPlate(FindMode, r, c))
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
        @brief     첫번째 Row Search  
        @return    Row No.
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  11:59
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
        @brief     First Col Search
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  12:01
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
        @brief     첫번째 Row의 마지막 Col 위치 찾기
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  12:07
        */
        public bool FindFirstRowLastCol(int FindMode, ref int R, ref int C)
        {
            for (int r = 0; r < m_nMaxRow; r++)
            {
                for (int c = m_nMaxCol - 1; c >= 0; c--)
                {
                    if (FindPlate(FindMode, r, c))
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
        @brief     마지막 Row의 첫번째 Col 위치 찾기
        @return
        @param
        @remark
        -
        @author    정지완(JUNGJIWAN)
        @date      2019/8/26  12:07
        */
        public bool FindLastRowFirstCol(int FindMode, ref int R, ref int C)
        {
            for (int r = m_nMaxRow - 1; r >= 0; r--)
            {
                for (int c = 0; c < m_nMaxCol; c++)
                {
                    if (FindPlate(FindMode, r, c))
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
        @brief     Left Top 부터 순차적으로 진행
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/18  10:51
        */
        public bool FindLeftTopToRightDown(int FindMode, ref int R, ref int C)
        {
            //홀수는 Right to Left
            for (int r = 0; r < m_nMaxRow; r++)
            {
                if (r % 2 == 1)
                {
                    for (int c = m_nMaxCol - 1; c >= 0; c--)
                    {
                        if (FindPlate(FindMode, r, c))
                        {
                            R = r;
                            C = c;
                            return true;
                        }
                    }

                }
                else
                {
                    for (int c = 0; c < m_nMaxCol; c++)
                    {
                        if (FindPlate(FindMode, r, c))
                        {
                            R = r;
                            C = c;
                            return true;
                        }
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
        @brief     Right Top 부터 순차적으로 진행
        @return
        @param
        @remark
        -
        @author    정지완(JUNGJIWAN)
        @date      2019/9/18  10:51
        */
        public bool FindRightTopToLeftDown(int FindMode, ref int R, ref int C)
        {
            //홀수는 Left to Right
            for (int r = 0; r < m_nMaxRow; r++)
            {
                if (r % 2 == 1)
                {
                    for (int c = 0; c < m_nMaxCol; c++)
                    {
                        if (FindPlate(FindMode, r, c))
                        {
                            R = r;
                            C = c;
                            return true;
                        }
                    }
                }
                else
                {
                    for (int c = m_nMaxCol - 1; c >= 0; c--)
                    {
                        if (FindPlate(FindMode, r, c))
                        {
                            R = r;
                            C = c;
                            return true;
                        }
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
        @brief     Left Top 부터 하단으로 Search(Col 우선)
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/19  10:38
        */
        public bool FindLeftTopDown(int FindMode, ref int R, ref int C, ref int id)
        {
            for (int c = 0; c < m_nMaxCol; c++)
            {
                for (int r = 0; r < m_nMaxRow; r++)
                {
                    if (FindPlate(FindMode, r, c))
                    {
                        R  = r;
                        C  = c;
                        id = m_nMagaId; 
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
        @brief     Right Top 부터 하단으로 Search(Col 우선)
        @return
        @param
        @remark
        -
        @author    정지완(JUNGJIWAN)
        @date      2019/9/19  10:38
        */
        public bool FindRightTopDown(int FindMode, ref int R, ref int C)
        {
            for (int c = m_nMaxCol - 1; c >= 0; c--)
            {
                for (int r = 0; r < m_nMaxRow; r++)
                {
                    if (FindPlate(FindMode, r, c))
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
        @brief     Load   (bool IsLoad, FILE* fp)
        @return    
        @param     
        @remark    
        -        
        @author    정지완(JUNGJIWAN)
        @date      2019/9/9  15:39
        */
        public void fn_LoadPlate(bool bLoad, FileStream fp)
        {
            for (int r = 0; r < m_nMaxRow; r++) //for (int r = 0; r < MAX_MAGAZINE_R; r++)
            {
                for (int c = 0; c < m_nMaxCol; c++) //for (int r = 0; r < MAX_MAGAZINE_C; r++)
                {
                    PLATE[r,c].fn_Load(bLoad, fp);
                }
            }
        }
        public void fn_Load(bool bLoad, FileStream fp)
        {

            
            if (bLoad)
            {
                BinaryReader br = new BinaryReader(fp);

                m_sMagaName     = br.ReadString();

                m_nMaxRow       = br.ReadInt32(); 
                m_nMaxCol       = br.ReadInt32();
                m_nMagazineStat = br.ReadInt32();
                m_nDirection    = br.ReadInt32();
            }
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);

                bw.Write(m_sMagaName    );

                bw.Write(m_nMaxRow      );
                bw.Write(m_nMaxCol      );
                bw.Write(m_nMagazineStat);
                bw.Write(m_nDirection   );
                
                bw.Flush(); 


                //bw.Close();
            }
            

            //
            fn_LoadPlate(bLoad, fp);

        }
        //---------------------------------------------------------------------------
        public void fn_UpdateMap(ref Label[,] label, bool rcp = false)
        {
            //
            if (label == null             ) return;
            if (m_nMaxRow > MAX_MAGAZINE_R) return;
            if (m_nMaxCol > MAX_MAGAZINE_C) return;

            int iMaxX = m_nMaxCol; 
            int iMaxY = m_nMaxRow; 
            int iMinX = 0; 
            int iMinY = 0;

            //Label Name, Color Setting
            for (int r = iMinY; r < iMaxY; r++)
            {
                for (int c = iMinX; c < iMaxX; c++)
                {
                    label[r, c].Background = PLATE[r, c].GetPlateColor_B();
                    if(rcp)
                    {
                        label[r, c].Content = string.Format($"{r * iMaxX + c + 1} : {PLATE[r, c]._sRecipeName}");
                    }
                    else
                    {
                        label[r, c].Content = string.Format($"{r * iMaxX + c + 1}");
                    }
                    //label[r, c].FontSize   = 5;

                }
            }

        }
        //---------------------------------------------------------------------------
        public void fn_UpdateMap(ref Label label)
        {
            //
            if (label == null) return;

            int iMaxX = m_nMaxCol;
            int iMaxY = m_nMaxRow;
            int iMinX = 0;
            int iMinY = 0;

            //Label Name, Color Setting
            for (int r = iMinY; r < iMaxY; r++)
            {
                for (int c = iMinX; c < iMaxX; c++)
                {
                    label.Background = PLATE[r, c].GetPlateColor_B();
                    //label.Content = string.Format($"{r * iMaxX + c + 1}");
                    
                    label.Content = STR_PLATE_STAT[PLATE[r, c]._nStat];
                }
            }
        }
    }
}
