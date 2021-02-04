using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
//
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum ;


namespace WaferPolishingSystem.BaseUnit
{
    /**
    @class     PLATE INFO.
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/02/03 16:24
    */
    public class Plate
    {
        //Const
        const int MAX_PLATE_STAT = 5 ;


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Variable
        #region [VARIABLE]


        string m_sPlateId   ;
	    string m_sPlateType ;
        string m_sRecipeName; 
	    	   
	    bool   m_bMask      ;
	    bool   m_bExitChk   ; 
               
	    int    m_nPlateNo   ; 
        int    m_nStat      ; //EN_PLATE_STAT m_nStat;
        int    m_nToolType  ; //사용할 Tool Type
        int    m_nFromRow   ;
        int    m_nFromCol   ;
        int    m_nFromMaga  ;
        int    m_nInfo      ;

	    double m_dPosition  ;


        bool   m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        int    m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;
        string m_sSpare1, m_sSpare2, m_sSpare3, m_sSpare4, m_sSpare5;
        double m_dSpare1, m_dSpare2, m_dSpare3, m_dSpare4, m_dSpare5;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public string _sPlateId
        {
            get { return m_sPlateId ; }
            set { m_sPlateId = value; }
        }
        public string _sPlateType
        {
            get { return m_sPlateType ; }
            set { m_sPlateType = value; }
        }

        public string _sRecipeName
        {
            get { return m_sRecipeName; }
            set { m_sRecipeName = value; }
        }

        public bool _bMask
        {
            get { return m_bMask ; }
            set { m_bMask = value; }
        }
        public bool _bExitChk
        {
            get { return m_bExitChk ; }
            set { m_bExitChk = value; }
        }

        public int _nPlateNo
        {
            get { return m_nPlateNo ; }
            set { m_nPlateNo = value; }
        }
        public int _nToolType
        {
            get { return m_nToolType ; }
            set { m_nToolType = value; }
        }

        public double _dPosition
        {
            get { return m_dPosition ; }
            set { m_dPosition = value; }
        }

        public int _nStat { get { return m_nStat; } }
        public int _nInfo { get { return m_nInfo; } }

        public int _nFromRow  { get { return m_nFromRow ; } set { m_nFromRow  = value; } }
        public int _nFromCol  { get { return m_nFromCol ; } set { m_nFromCol  = value; } }
        public int _nFromMaga { get { return m_nFromMaga; } set { m_nFromMaga = value; } }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static public string[] STR_PLATE_STAT = new string[MAX_PLATE_STAT]
        {
            "None"      ,
            "Empty"     ,
            "Ready"     ,
            "Finish"    ,
            "Skip"      ,
        };

        #endregion

        //---------------------------------------------------------------------------
        public Plate()
        {
            Init();
        }
        //---------------------------------------------------------------------------
        public Plate Copy()
        {
            return UserFunction.DeepClone(this) as Plate;
        }

        //---------------------------------------------------------------------------
        void Init()
        {
            m_nStat    = (int)EN_PLATE_STAT.ptsNone;
            m_bMask    = false;
            m_bExitChk = false;
            m_nPlateNo = 0;
            m_nInfo    = (int)EN_PLATE_INFO.ifNone;

            m_sPlateId   = string.Empty;
            m_sPlateType = string.Empty;
            m_sRecipeName= string.Empty;
            m_dPosition  = 0.0;

            m_nFromRow  = -1;
            m_nFromCol  = -1;
            m_nFromMaga = -1;

            m_bSpare1 = m_bSpare2 = m_bSpare3 = m_bSpare4 = m_bSpare5 = false;
            m_nSpare1 = m_nSpare2 = m_nSpare3 = m_nSpare4 = m_nSpare5 = 0;
            m_sSpare1 = m_sSpare2 = m_sSpare3 = m_sSpare4 = m_sSpare5 = string.Empty;
            m_dSpare1 = m_dSpare2 = m_dSpare3 = m_dSpare4 = m_dSpare5 = 0.0;

        }
        //---------------------------------------------------------------------------
        public void ClearMap()
        {
            m_nStat       = (int)EN_PLATE_STAT.ptsEmpty;
            m_bMask       = false;
            m_bExitChk    = false;
            m_nPlateNo    = 0;
            m_nInfo       = (int)EN_PLATE_INFO.ifNone;
                          
            m_nFromRow    = -1;
            m_nFromCol    = -1;
            m_nFromMaga   = -1;

            m_sPlateId    = string.Empty;
            m_sRecipeName = string.Empty;
            m_sPlateType  = string.Empty;

        }
        //---------------------------------------------------------------------------
        //GET FUNCTION
        public string GetPlateId  () { return m_sPlateId  ; }
        public string GetPlateType() { return m_sPlateType; }

        //---------------------------------------------------------------------------
        public bool IsExist() 
        { 
            return  (m_nStat != (int)EN_PLATE_STAT.ptsEmpty ) && 
                    (m_nStat == (int)EN_PLATE_STAT.ptsReady ) && (m_nStat != (int)EN_PLATE_STAT.ptsFinish); 
        }
//         public bool IsEmpty()
//         {
//             return (m_nStat == (int)EN_PLATE_STAT.ptsEmpty || m_nStat == (int)EN_PLATE_STAT.ptsNone) &&
//                    (m_nStat != (int)EN_PLATE_STAT.ptsReady) &&
//                    (m_nStat != (int)EN_PLATE_STAT.ptsFinish);
//         }
        public bool IsEmpty()
        {
            return (m_nStat == (int)EN_PLATE_STAT.ptsEmpty || m_nStat == (int)EN_PLATE_STAT.ptsNone);
        }

        public bool IsExistReady()
        {
            return (m_nStat == (int)EN_PLATE_STAT.ptsReady);
        }


        public bool IsStat(int state) { return (m_nStat == state); }
        public bool IsStat(EN_PLATE_STAT state) { return (m_nStat == (int)state); }
        public int GetPlateState () { return m_nStat    ; }
        public double GetPosition() { return m_dPosition; }


        //---------------------------------------------------------------------------
        //SET FUNCTION
        public void SetPlateId  (string set        ) { m_sPlateId   = set; }
        public void SetPlateType(string set        ) { m_sPlateType = set; }
        public void SetStat     (EN_PLATE_STAT Stat) { m_nStat = (int)Stat; }
        public void SetInfo     (EN_PLATE_INFO Info) { m_nInfo = (int)Info; }
        
        public void SetStat     (int           Stat) { m_nStat =      Stat; }
        public void SetPlateNo  (int No            ) { m_nPlateNo  = No ; }
        public void SetPosition (double Pos        ) { m_dPosition = Pos; }

        public void SetTo(int Stat)
        {
            m_nStat = Stat;
            
            if (Stat == (int)EN_PLATE_STAT.ptsEmpty ) ClearMap();
            //if (Stat == (int)EN_PLATE_STAT.ptsReady ) m_nInfo = 0;
            if (Stat != (int)EN_PLATE_STAT.ptsFinish) m_nInfo = 0; //JUNG/201130

        }
        public void SetTo(EN_PLATE_STAT Stat)
        {
            m_nStat = (int)Stat;
        }
        public void SetTo(int Stat, string PinId)
        {
            m_nStat    = (Stat);
            m_sPlateId = PinId; 

        }
        public void SetTo(EN_PLATE_STAT Stat, string PinId)
        {
            m_nStat    = (int)Stat;
            m_sPlateId = PinId;

        }

        //---------------------------------------------------------------------------
//         public Color GetPlateColor()
// 		{
// 			if      (m_nStat == (int)EN_PLATE_STAT.ptsEmpty  ) return Colors.White;
// 			else if (m_nStat == (int)EN_PLATE_STAT.ptsReady  ) return Colors.Green;
//             else if (m_nStat == (int)EN_PLATE_STAT.ptsFinish ) return Colors.Maroon;
//             else if (m_nStat == (int)EN_PLATE_STAT.ptsSkip   ) return Color.FromRgb(102, 051, 0);
// 			else                                               return Colors.Black;
// 		}
        //---------------------------------------------------------------------------
        public SolidColorBrush GetPlateColor_B()
		{
            //var pcSkip = new SolidColorBrush(Color.FromRgb(102, 051, 0));

			if      (m_nStat == (int)EN_PLATE_STAT.ptsEmpty     ) return Brushes.LightGray     ;
            else if (m_nStat == (int)EN_PLATE_STAT.ptsLoad      ) return Brushes.Coral         ;
            else if (m_nStat == (int)EN_PLATE_STAT.ptsPreAlign  ) return Brushes.HotPink       ;
			else if (m_nStat == (int)EN_PLATE_STAT.ptsReady     ) return Brushes.Green         ;
          //else if (m_nStat == (int)EN_PLATE_STAT.ptsFinish    ) return Brushes.Magenta       ;
            else if (m_nStat == (int)EN_PLATE_STAT.ptsFinish    )
            {
                if(m_nInfo == (int)EN_PLATE_INFO.ifVisnErr) return Brushes.Maroon ; //JUNG/201103
                else                                        return Brushes.Magenta;
            }
            else if (m_nStat == (int)EN_PLATE_STAT.ptsAlign     ) return Brushes.CornflowerBlue;
            else if (m_nStat == (int)EN_PLATE_STAT.ptsPolish    ) return Brushes.SandyBrown    ;
			else if (m_nStat == (int)EN_PLATE_STAT.ptsClean     ) return Brushes.SkyBlue       ;
            else if (m_nStat == (int)EN_PLATE_STAT.ptsDeHydrate ) return Brushes.SkyBlue       ;
          //else if (m_nStat == (int)EN_PLATE_STAT.ptsSkip      ) return pcSkip                ;
            else if (m_nStat == (int)EN_PLATE_STAT.ptsPolishWait) return Brushes.Yellow        ;
            else                                                  return Brushes.Black         ;

        }
        //---------------------------------------------------------------------------
        public SolidColorBrush GetPlateColor(EN_PLATE_STAT Stat)
		{
            //var pcSkip = new SolidColorBrush(Color.FromRgb(102, 051, 0));

			if      (Stat == EN_PLATE_STAT.ptsEmpty     ) return Brushes.LightGray     ;
            else if (Stat == EN_PLATE_STAT.ptsLoad      ) return Brushes.Coral         ;
            else if (Stat == EN_PLATE_STAT.ptsPreAlign  ) return Brushes.HotPink       ;
			else if (Stat == EN_PLATE_STAT.ptsReady     ) return Brushes.Green         ;
          //else if (Stat == EN_PLATE_STAT.ptsFinish    ) return Brushes.Magenta       ;
            else if (Stat == EN_PLATE_STAT.ptsFinish    )
            { 
                if(m_nInfo == (int)EN_PLATE_INFO.ifVisnErr) return Brushes.Maroon ; //JUNG/201103
                else                                        return Brushes.Magenta;

            }
            else if (Stat == EN_PLATE_STAT.ptsAlign     ) return Brushes.CornflowerBlue;
            else if (Stat == EN_PLATE_STAT.ptsPolish    ) return Brushes.SandyBrown    ; //
            else if (Stat == EN_PLATE_STAT.ptsClean     ) return Brushes.SkyBlue       ;
            else if (Stat == EN_PLATE_STAT.ptsDeHydrate ) return Brushes.SkyBlue       ;
            //else if (Stat == EN_PLATE_STAT.ptsSkip      ) return pcSkip                ;
            else if (Stat == EN_PLATE_STAT.ptsPolishWait) return Brushes.Yellow        ; //
			else                                          return Brushes.Black         ;

        }
        //---------------------------------------------------------------------------
        /**    
        <summary>
        	File Load
        </summary>
        <param name="bLoad"> Load/Unlaod </param>
        @author    정지완(JUNGJIWAN)
        @date      2020/02/04 09:55
        */
        public void fn_Load(bool bLoad, FileStream fp)
        {
            
            if (bLoad)
            {
                BinaryReader br = new BinaryReader(fp);

                //
                m_nPlateNo    = br.ReadInt32  ();
                m_nStat       = br.ReadInt32  ();
                m_nToolType   = br.ReadInt32  ();

                m_nFromRow    = br.ReadInt32  ();
                m_nFromCol    = br.ReadInt32  ();
                m_nFromMaga   = br.ReadInt32  ();
                m_nInfo       = br.ReadInt32  ();

                m_nSpare1     = br.ReadInt32  ();
                m_nSpare2     = br.ReadInt32  ();
                m_nSpare3     = br.ReadInt32  ();
                m_nSpare4     = br.ReadInt32  ();

                //
                m_sPlateId    = br.ReadString ();
                m_sPlateType  = br.ReadString ();
                m_sRecipeName = br.ReadString ();

                m_sSpare1     = br.ReadString ();
                m_sSpare2     = br.ReadString ();
                m_sSpare3     = br.ReadString ();
                m_sSpare4     = br.ReadString ();
                m_sSpare5     = br.ReadString ();
                                   

                //
                m_bMask       = br.ReadBoolean();
                m_bExitChk    = br.ReadBoolean();

                m_bSpare1     = br.ReadBoolean();
                m_bSpare2     = br.ReadBoolean();
                m_bSpare3     = br.ReadBoolean();
                m_bSpare4     = br.ReadBoolean();
                m_bSpare5     = br.ReadBoolean();

                //
                m_dPosition   = br.ReadDouble ();

                m_dSpare1     = br.ReadDouble ();
                m_dSpare2     = br.ReadDouble ();
                m_dSpare3     = br.ReadDouble ();
                m_dSpare4     = br.ReadDouble ();
                m_dSpare5     = br.ReadDouble ();

                //br.Close();
            }
            else
            {
                BinaryWriter bw = new BinaryWriter(fp);

                bw.Write(m_nPlateNo   );
                bw.Write(m_nStat      );
                bw.Write(m_nToolType  );

                bw.Write(m_nFromRow   );
                bw.Write(m_nFromCol   );
                bw.Write(m_nFromMaga  );
                bw.Write(m_nInfo      );

                bw.Write(m_nSpare1    ); //Spare
                bw.Write(m_nSpare2    );
                bw.Write(m_nSpare3    );
                bw.Write(m_nSpare4    );

                //
                bw.Write(m_sPlateId   );
                bw.Write(m_sPlateType );
                bw.Write(m_sRecipeName);

                bw.Write(m_sSpare1    ); //Spare
                bw.Write(m_sSpare2    );
                bw.Write(m_sSpare3    );
                bw.Write(m_sSpare4    );
                bw.Write(m_sSpare5    );

                //
                bw.Write(m_bMask      );
                bw.Write(m_bExitChk   );

                bw.Write(m_bSpare1    ); //Spare
                bw.Write(m_bSpare2    );
                bw.Write(m_bSpare3    );
                bw.Write(m_bSpare4    );
                bw.Write(m_bSpare5    );

                //
                bw.Write(m_dPosition  );

                bw.Write(m_dSpare1    ); //Spare
                bw.Write(m_dSpare2    );
                bw.Write(m_dSpare3    );
                bw.Write(m_dSpare4    );
                bw.Write(m_dSpare5    );

                bw.Flush();

                //bw.Close();
                
            }
        }
    }
}
