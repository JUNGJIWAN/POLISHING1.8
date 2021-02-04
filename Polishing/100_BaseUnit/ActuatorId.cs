using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.BaseUnit
{
    /**
    @class     Actuator ID Setting   
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2019/12/23  13:45
    */

    public class ActuatorId
    {
        public enum EN_ACTR_LIST
        {
            aNONE = -1,

            // Part
            aSpdl_LensCovr    ,  //00//ACTR_No00//Spindle - Lens Cover
            aSpdl_PlateClamp  ,  //01//ACTR_No01//Spindle - Plate Clamp
            aspdl_IR          ,  //02//ACTR_No02//Spindle - IR Shutter
            ACTR_No03         ,  //03//ACTR_No03//
            aPoli_Clamp       ,  //04//ACTR_No04//Polishing - Plate Clamp
            ACTR_No05         ,  //05//ACTR_No05//
            aClen_Clamp       ,  //06//ACTR_No06//Cleaning - Plate Clamp 
            ACTR_No07         ,  //07//ACTR_No07//
            aStrg_LockBtm     ,  //08//ACTR_No08//Storage Lock Bottom
            aStrg_LockTop     ,  //09//ACTR_No09//Storage Lock Top
            ACTR_No10         ,  //10//ACTR_No10//
            aTran_TopLoadFB   ,  //11//ACTR_No11//Transfer - Top Load Fwd/Bwd
            aTran_TopLoadTurn ,  //12//ACTR_No12//Transfer - Top Load Turn(0, 180)
            aTran_BtmLoadFB   ,  //13//ACTR_No13//Transfer - Bottom Load Fwd/Bwd
            aTran_LoadPortUD  ,  //14//ACTR_No14//Transfer - Load Port Up/Down
            aTran_MagaMoveLR  ,  //15//ACTR_No15//Transfer - Magazine Move Left/Right
            aTran_LoadCover   ,  //16//ACTR_No16//Transfer - Load Protect Cover

            EndOfId           ,

            ACTR_No17         ,  //17//ACTR_No17//            
            ACTR_No18         ,  //18// 
            ACTR_No19         ,  //19//
            ACTR_No20         ,  //20//
            ACTR_No21         ,  //21//
            aSYS_DoorMain     ,  //22//SYSTEM - Door Main
            aSYS_DoorSTOR     ,  //23//SYSTEM - Door Storage

            //aMgzn_TRTopFwd,  //Magazine Part - 1.8
            //aMgzn_TRBtmFwd,
            //aMgzn_TRTurn,
            //aMgzn_PlateLdUp,
            //ACTR_No30,
            //ACTR_No31,
            //ACTR_No32,
            //ACTR_No33,
            //ACTR_No34,
            //ACTR_No35,
            //ACTR_No36,
            //ACTR_No37,
            //ACTR_No38,
            //ACTR_No39,
            //ACTR_No40,
            //ACTR_No41,
            //ACTR_No42,
            //ACTR_No43,
            //ACTR_No44,
            //ACTR_No45,
            
            
        }
        //---------------------------------------------------------------------------
        public enum EN_ACTR_CMD
        {
            Fwd ,
            Bwd , 
        }
    }
}
