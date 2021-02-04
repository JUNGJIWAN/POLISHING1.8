using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserEnum;
using static WaferPolishingSystem.Define.UserConst;


namespace WaferPolishingSystem.BaseUnit
{
	/**
	@class     PIN INFO.
	@brief     
	@remark    
	-    
	@author    정지완(JUNGJIWAN)
	@date      2020/02/03 16:23
	*/
	public class Pin
	{

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//Variable
		#region [VARIABLE]

		string m_sPinId;

		bool   m_bMask    ;
		bool   m_bExistChk;
		bool   m_bForceChk;

		//EN_PIN_STAT m_iStat; 
		//EN_PIN_TYPE m_iType;
		int    m_iStat ;
		int    m_iType ; //Polishing, Washing
		int    m_iKind ; //Tool 종류(사용자 지정)
		int    m_nPinNo;  //ID 식별용.

		double m_dXPos ;
		double m_dYPos ;

		Color m_cr0, m_cr1, m_cr4;

		//Spare
		bool m_bSpare1, m_bSpare2, m_bSpare3, m_bSpare4, m_bSpare5;
        int  m_nSpare1, m_nSpare2, m_nSpare3, m_nSpare4, m_nSpare5;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Property
        public string _sPinId
		{
			get { return m_sPinId ; }
			set { m_sPinId = value; }
		}
		public int _nPinNo
		{
            get { return m_nPinNo ; }
            set { m_nPinNo = value; }
        }
        public int _iKind
		{
            get { return m_iKind ; }
            set { m_iKind = value; }
        }
        public bool _bMask
		{
            get { return m_bMask ; }
            set { m_bMask = value; }
        }

        public int _nStat { get { return m_iStat; } }

// 		static public string[] STR_PIN_STAT = new string[MAX_PIN_STAT]
// 		{
// 			"Empty"       ,
// 			"New"         ,
// 			"Used"        ,
// 			"Mask"        ,
// 			"Need Chk"    ,
// 		};


        #endregion

        //---------------------------------------------------------------------------
        //생성자
        public Pin()
		{
			Init();
		}
		//---------------------------------------------------------------------------
        public Pin Copy()
        {
            return UserFunction.DeepClone(this) as Pin;
        }

        //---------------------------------------------------------------------------
        private void Init()
		{
			m_iStat  = (int)EN_PIN_STAT.psNone;
			m_iType  = (int)EN_PIN_TYPE.ptNone;
			m_iKind  = -1;
			m_nPinNo =  0;

			m_bMask  = false;
			m_sPinId = string.Empty;

			m_dXPos  = 0.0;
            m_dYPos  = 0.0;

			m_bExistChk = false;
			m_bForceChk = false;

			m_cr0 = Colors.LightSteelBlue;
			m_cr1 = Colors.CadetBlue     ;

			m_cr4 = Colors.CornflowerBlue;

		}
		//---------------------------------------------------------------------------
		public void ClearMap()
		{
			m_iStat  = (int)EN_PIN_STAT.psEmpty;
			m_iType  = (int)EN_PIN_TYPE.ptNone ;
			
			m_iKind  = -1;
			m_nPinNo = 0;

			m_bMask = false;
            
			//m_dXPos = 0.0;
            //m_dYPos = 0.0;

            m_sPinId = string.Empty;

			m_bExistChk = false;
			m_bForceChk = false;


		}
		//---------------------------------------------------------------------------
		//GET Function 
		public bool IsExistCheck()
		{
			return m_bExistChk;
		}
        public bool IsForceCheck()
        {
            return m_bForceChk;
        }

        public bool IsExist    () 
		{ 
			return (m_iStat != (int)EN_PIN_STAT.psEmpty  ) && (m_iStat != (int)EN_PIN_STAT.psUsed   ) &&
				  ((m_iStat == (int)EN_PIN_STAT.psNewPol ) || (m_iStat == (int)EN_PIN_STAT.psNewCln ) || 
				   (m_iStat == (int)EN_PIN_STAT.psUsedCln) || (m_iStat == (int)EN_PIN_STAT.psUsedPol) ); //|| (m_iStat == (int)EN_PIN_STAT.psNeedChk)) ;
		}
		public bool IsEmpty()
		{
			return (m_iStat == (int)EN_PIN_STAT.psEmpty) || (m_iStat == (int)EN_PIN_STAT.psNone);
		}

        public bool IsExistPol () 
		{ 
			return (m_iStat != (int)EN_PIN_STAT.psEmpty  ) &&  
				   (m_iStat == (int)EN_PIN_STAT.psNewPol ) && 
				   (m_iStat != (int)EN_PIN_STAT.psUsed   )  ; 
				   //(m_iStat != (int)EN_PIN_STAT.psNeedChk); 
	    }
		public bool IsExistCln () 
		{ 
			return (m_iStat != (int)EN_PIN_STAT.psEmpty  ) && 
				   (m_iStat == (int)EN_PIN_STAT.psNewCln ) && 
				   (m_iStat != (int)EN_PIN_STAT.psUsed   ) ; 
				   //(m_iStat != (int)EN_PIN_STAT.psNeedChk); 
		}
		public bool IsStat(int state        ) 
		{ 
			return (m_iStat == state ); 
		}
		public bool IsStat(EN_PIN_STAT state) 
		{
			return (m_iStat == (int)state ); 
		}

		public bool IsType     (int type         ) 
		{ 
			return  m_iType == type        ; 
		}
		public bool IsType     (EN_PIN_TYPE type ) 
		{ 
			return  m_iType == (int)type   ; 
		}
		public bool IsTypePolis() 
		{ 
			return  m_iType == (int)EN_PIN_TYPE.ptPolis; 
		}
		public bool IsTypeClean() 
		{ 
			return  m_iType == (int)EN_PIN_TYPE.ptClean; 
		}
		public int GetPinState () 
		{ 
			return  m_iStat           ; 
		}
		public int GetPinType  () 
		{ 
			return  m_iType           ; 
		}
		public bool IsPinKind  (int kind) 
		{ 
			return  m_iKind == kind   ; 
		}
		public int GetPinKind  () 
		{ 
			return  m_iKind; 
		}

		public ST_PIN_POS GetPosition() 
		{
			ST_PIN_POS PinPos = new ST_PIN_POS(0);

			PinPos.dXPos = m_dXPos; 
			PinPos.dYPos = m_dYPos;
			
			return  PinPos;
		}
		//---------------------------------------------------------------------------
		public void SetColor(Color co1, Color co2, Color co5)
        {
            m_cr0 = co1;
            m_cr1 = co2;
			
			m_cr4 = co5;
		}
		//---------------------------------------------------------------------------
        //SET FUNCTION
        public void SetPinNo(int No          ) { m_nPinNo = No ; }
		public void SetKind (int Kind        ) { m_iKind = Kind; }
		public void SetKind (EN_PIN_KIND Kind) { m_iKind = (int)Kind; }
		public void SetType (EN_PIN_TYPE Type) { m_iType = (int)Type; }

        public void SetPosition(double XPos, double YPos) 
		{ 
			m_dXPos = XPos; 
			m_dYPos = YPos; 
		}
		public void SetPosition(ST_PIN_POS Pos) 
		{ 
			m_dXPos = Pos.dXPos; 
			m_dYPos = Pos.dYPos; 
		}
		//---------------------------------------------------------------------------
		public void SetExistCheck(bool set)
		{
			m_bExistChk = set; 
		}
        public void SetForceCheck(bool set)
        {
            m_bForceChk = set;
        }

        //---------------------------------------------------------------------------
        public void SetTo(int Stat)
		{

			SetTo((EN_PIN_STAT)Stat);

		}
		public void SetTo(EN_PIN_STAT Stat)
		{
			m_iStat = (int)Stat;

            if (Stat == EN_PIN_STAT.psEmpty)
            {
				m_bForceChk = false;
				m_bExistChk = false; 
            }

        }

        //---------------------------------------------------------------------------
        public void SetTo(int Stat, string PinId)
		{
			m_iStat = Stat;
			m_sPinId = PinId;
		}
		public void SetTo(EN_PIN_STAT Stat, string PinId)
		{
			m_iStat = (int)Stat;
			m_sPinId = PinId;
		}

		public void SetTo(int Type, int Stat)
		{
			m_iType = Type;
			m_iStat = Stat;

		}
		public void SetTo(EN_PIN_TYPE Type, EN_PIN_STAT Stat)
		{
			m_iType = (int)Type;
			m_iStat = (int)Stat;

		}

		public void SetTo(int Type, int Stat, int Kind)
		{
			m_iType = Type;
			m_iStat = Stat;
			m_iKind = Kind;

		}
		public void SetTo(EN_PIN_TYPE Type, EN_PIN_STAT Stat, int Kind)
		{
			m_iType = (int)Type;
			m_iStat = (int)Stat;
			m_iKind = Kind;

		}
		//---------------------------------------------------------------------------
		public Color GetPinColor()
		{
			if      (m_iStat == (int)EN_PIN_STAT.psEmpty  ) return Colors.White;
			else if (m_iStat == (int)EN_PIN_STAT.psNewPol ) return Colors.Green;
			else if (m_iStat == (int)EN_PIN_STAT.psNewCln ) return Colors.Green;
			else if (m_iStat == (int)EN_PIN_STAT.psUsed   ) return Colors.Black;
			else if (m_iStat == (int)EN_PIN_STAT.psMask   ) return Color.FromRgb(102, 051, 0);
			//else if (m_iStat == (int)EN_PIN_STAT.psNeedChk) return Color.FromRgb(153, 153, 153);
			else                                            return Colors.Black;

		}
		//---------------------------------------------------------------------------
		public SolidColorBrush GetPinColor_B(int type = 0)
		{
			//var pcMask    = new SolidColorBrush(Color.FromRgb(102, 051, 0));
			//var pcNeedChk = new SolidColorBrush(Color.FromRgb(102, 051, 0));

			SolidColorBrush rtnColor0 = new SolidColorBrush(m_cr0);
			SolidColorBrush rtnColor1 = new SolidColorBrush(m_cr1);
			SolidColorBrush rtnColor4 = new SolidColorBrush(m_cr4);

			if      (m_iStat == (int)EN_PIN_STAT.psEmpty  ) return type==0? Brushes.LightGray : Brushes.DarkGray;
            else if (m_iStat == (int)EN_PIN_STAT.psUsed	  ) return Brushes.Magenta       ;
			
			else if (m_iStat == (int)EN_PIN_STAT.psNewPol ) return m_iKind == 1 ? rtnColor1 : rtnColor0;
			else if (m_iStat == (int)EN_PIN_STAT.psNewCln ) return rtnColor4;
		    //else if (m_iStat == (int)EN_PIN_STAT.psNewPol ) return m_iKind == 1 ? Brushes.CadetBlue : Brushes.LightSteelBlue;   //Brushes.Green    ;
		    //else if (m_iStat == (int)EN_PIN_STAT.psNewCln ) return Brushes.CornflowerBlue;
			
			else if (m_iStat == (int)EN_PIN_STAT.psUsedPol) return Brushes.Magenta       ;
			else if (m_iStat == (int)EN_PIN_STAT.psUsedCln) return Brushes.Magenta       ;
            //else if (m_iStat == (int)EN_PIN_STAT.psMask   ) return pcMask                ;
            else                                            return Brushes.Black         ;

		}
		//---------------------------------------------------------------------------
        public SolidColorBrush GetPinColor(EN_PIN_STAT Stat)
		{
			var pcMask    = new SolidColorBrush(Color.FromRgb(102, 051, 0));
			var pcNeedChk = new SolidColorBrush(Color.FromRgb(102, 051, 0));

			if      (Stat == EN_PIN_STAT.psEmpty  ) return Brushes.LightGray;
            else if (Stat == EN_PIN_STAT.psUsed	  ) return Brushes.Magenta  ;
	      //else if (Stat == EN_PIN_STAT.psNeedChk) return Brushes.CadetBlue;
			else if (Stat == EN_PIN_STAT.psNewPol ) return Brushes.Green    ;
			else if (Stat == EN_PIN_STAT.psNewCln ) return Brushes.LimeGreen;
			else if (Stat == EN_PIN_STAT.psUsedPol) return Brushes.Magenta  ;
			else if (Stat == EN_PIN_STAT.psUsedCln) return Brushes.Magenta  ;
            else if (Stat == EN_PIN_STAT.psMask   ) return pcMask           ;
            else                                    return Brushes.Black    ;

		}
        //---------------------------------------------------------------------------
        public string GetPinStateName()
		{
			string sRtn = string.Empty;


			return sRtn; 
		}
		
		//---------------------------------------------------------------------------
		public void fn_Load(bool bLoad, FileStream fp)
		{
			
			if (bLoad)
			{
				BinaryReader br = new BinaryReader(fp);

				m_sPinId    = br.ReadString();

                m_bExistChk = br.ReadBoolean();
                m_bForceChk = br.ReadBoolean();
				m_bMask     = br.ReadBoolean();

				m_bSpare1   = br.ReadBoolean();
				m_bSpare2   = br.ReadBoolean();
				m_bSpare3   = br.ReadBoolean();
				m_bSpare4   = br.ReadBoolean();
				m_bSpare5   = br.ReadBoolean();

				m_dXPos     = br.ReadDouble();
				m_dYPos     = br.ReadDouble();

                m_nPinNo    = br.ReadInt32();
				m_iType     = br.ReadInt32();
				m_iStat     = br.ReadInt32();
				m_iKind     = br.ReadInt32();

                m_nSpare1   = br.ReadInt32();
                m_nSpare2   = br.ReadInt32();
                m_nSpare3   = br.ReadInt32();
                m_nSpare4   = br.ReadInt32();
                m_nSpare5   = br.ReadInt32();

			}
			else
			{
				BinaryWriter bw = new BinaryWriter(fp);

				bw.Write(m_sPinId    );

				bw.Write(m_bExistChk );
				bw.Write(m_bForceChk );
				bw.Write(m_bMask     );

				bw.Write(m_bSpare1   );
				bw.Write(m_bSpare2   );
				bw.Write(m_bSpare3   );
				bw.Write(m_bSpare4   );
				bw.Write(m_bSpare5   );

				bw.Write(m_dXPos     );
				bw.Write(m_dYPos     );

				bw.Write(m_nPinNo    );
				bw.Write(m_iType     );
				bw.Write(m_iStat     );
				bw.Write(m_iKind     );

				bw.Write(m_nSpare1   );
				bw.Write(m_nSpare2   );
				bw.Write(m_nSpare3   );
				bw.Write(m_nSpare4	 );
				bw.Write(m_nSpare5   );   

				bw.Flush(); 

			}
		}
	}

}
