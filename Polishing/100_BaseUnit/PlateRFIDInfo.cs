using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.BaseUnit
{
    public class PlateRFIDInfo
    {
        //
        public string plate_id = string.Empty;
        public string sRcvTime = string.Empty;
        public RestSpecimenInfo specimeninfo { get; set; }

        //Property
        public string _sPlateId { get { return plate_id; } set { plate_id = value; } }


        //---------------------------------------------------------------------------
        public PlateRFIDInfo()
        {
            specimeninfo = new RestSpecimenInfo();

            Init();
        }

        //---------------------------------------------------------------------------
        public void Init()
        {
            this.specimeninfo.Init();

            plate_id = string.Empty;
            sRcvTime = string.Empty;
        }

    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public class RestSpecimenInfo
    {
        //
        public int specimen_id = 0;

        public RestWaferInfo waferInfo;

        public int    SizeX         = 10 ;
        public int SizeY         = 10 ;
        
        public string ShotPos       = "shoptPos";
        public string ChipPos       = "chipPos";
        public string MatPos        = "matPos";
        public string MatLoc        = "matLoc";
        public string Type          = "type";
        
        //Property
        public int    _nSizeX      { get { return SizeX     ; } set { SizeX      = value; } }
        public int    _nSizeY      { get { return SizeY     ; } set { SizeY      = value; } }
        public int    _nSpecimenId { get { return specimen_id; } set { specimen_id = value; } }
        public string _sShotPos    { get { return ShotPos   ; } set { ShotPos    = value; } }
        public string _sChipPos    { get { return ChipPos   ; } set { ChipPos    = value; } }
        public string _sMatPos     { get { return MatPos    ; } set { MatPos     = value; } }
        public string _sMatLoc     { get { return MatLoc    ; } set { MatLoc     = value; } }
        public string _sType       { get { return Type      ; } set { Type       = value; } }
        
        //---------------------------------------------------------------------------
        public RestSpecimenInfo()
        {
            waferInfo = new RestWaferInfo();

            Init();
        }
        //---------------------------------------------------------------------------
        public void Init()
        {
            this.waferInfo.Init();

            this.SizeX       = 0;
            this.SizeY       = 0;
            this.specimen_id = 0;

            this.ShotPos     = "";
            this.ChipPos     = "";
            this.MatPos      = "";
            this.MatLoc      = "";
            this.Type        = "";
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public class RestWaferInfo
    {
        //
        public string device      = string.Empty;
        public string processStep = string.Empty ;
        public string version     = string.Empty ;
        public string lotID       = string.Empty ;
        public int    wafer_num   = 0;
        public int    n_angle     = 0;

        //
        public int    _nWafer_num   { get { return wafer_num; } set { wafer_num = value  ; } }
        public int    _nAngle       { get { return n_angle; } set { n_angle = value      ; } }

        public string _sDevice      { get { return device; } set { device = value; } }
        public string _sProcessStep { get { return processStep; } set { processStep = value; } }
        public string _sVersion     { get { return version; } set { version = value; } }
        public string _sLotID       { get { return lotID  ; } set { lotID = value; } }

        //---------------------------------------------------------------------------
        public RestWaferInfo()
        {
            Init();
        }
        //---------------------------------------------------------------------------
        public void Init()
        {
            this.device      = "";
            this.processStep = "";
            this.version     = "";
            this.lotID       = "";
            this.wafer_num   = 0;
            this.n_angle     = 0;
        }
    }
}
