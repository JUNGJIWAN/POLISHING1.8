using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.BaseUnit
{
    /**
    @class     Define Error ID
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/2/3  12:04
    */
    public class ERRID
    {
        public enum EN_ERR_LIST : int
        {
            ERR_NONE =  -1,
            ERR_0001 =   0, //All Home....
            ERR_0002, //Spindle Part Home...
            ERR_0003, //Polishing Part Home...
            ERR_0004, //Cleaning Part Home...
            ERR_0005, //Storage Part Home...
            ERR_0006, //Loading Part Home...
            ERR_0007,
            ERR_0008,
            ERR_0009,
            ERR_0010,
            ERR_0011,
            ERR_0012,
            ERR_0013,
            ERR_0014,
            ERR_0015, //Main Spindle X Axis INIT Error
            ERR_0016, //Polishing Y Axis INIT Error
            ERR_0017, //Main Spindle Z Axis INIT Error
            ERR_0018, //Cleaning R Axis INIT Error
            ERR_0019, //Polishing Theta Axis INIT Error
            ERR_0020, //Polishing Tilt Axis INIT Error
            ERR_0021, //Transfer Y-Axis INIT Error
            ERR_0022, //Storage Y Axis INIT Error
            ERR_0023, //Cleaning Y Axis INIT Error
            ERR_0024, //Polishing Cup Z Axis INIT Error
            ERR_0025, //Main Plate Z Axis INIT Error
            ERR_0026, //Transfer TH1 Axis INIT Error
            ERR_0027, //Transfer TH2 Axis INIT Error
            ERR_0028, 
            ERR_0029,
            ERR_0030,
            ERR_0031, //IO Connection Error
            ERR_0032, //MOTOR Connection Error
            ERR_0033, //PMC Connection Error
            ERR_0034, //LOAD CELL(BTM) Connection Error                  
            ERR_0035, //AUTO SUPPLY[SLURRY] Connection Error
            ERR_0036, //AUTO SUPPLY[SOAP] Connection Error
            ERR_0037, //VISION CAM Connection Error 
            ERR_0038, //VISION LIGHT Connection Error
            ERR_0039,
            ERR_0040,
            ERR_0041,
            ERR_0042,
            ERR_0043,
            ERR_0044,
            ERR_0045,
            ERR_0046,
            ERR_0047,
            ERR_0048,
            ERR_0049,
            ERR_0050, //Spindle - E3000 Error
            ERR_0051, //Spindle - E3000 Warning
            ERR_0052,
            ERR_0053,
            ERR_0054,
            ERR_0055,
            ERR_0056,
            ERR_0057,
            ERR_0058,
            ERR_0059,
            ERR_0060, //Machine Run Wait...
            ERR_0061,
            ERR_0062,
            ERR_0063,
            ERR_0064,
            ERR_0065,
            ERR_0066,
            ERR_0067,
            ERR_0068,
            ERR_0069,
            ERR_0070, //SICK Door Signal - Open
            ERR_0071,
            ERR_0072,
            ERR_0073,
            ERR_0074,
            ERR_0075,
            ERR_0076,
            ERR_0077,
            ERR_0078,
            ERR_0079,
            ERR_0080, //Right Door Open
            ERR_0081, //Left Door Open
            ERR_0082, //Side Door Open
            ERR_0083,
            ERR_0084,
            ERR_0085,
            ERR_0086,
            ERR_0087,
            ERR_0088,
            ERR_0089,
            ERR_0090,
            ERR_0091,
            ERR_0092,
            ERR_0093,
            ERR_0094,
            ERR_0095,
            ERR_0096,
            ERR_0097,
            ERR_0098,
            ERR_0099,//[ACS] Supply Valve Close
            ERR_0100,
            //---------------------------------------------------------------------------
            ERR_0101, //EMO FRONT
            ERR_0102, //EMO REAR
            ERR_0103, //EMO C-BOX
            ERR_0104, //ACS Network Error
            ERR_0105, //TOP SOL BOX MAIN AIR Error
            ERR_0106, //BOTTOM SOL BOX MAIN AIR Error
            ERR_0107, //C-Box Temp Alarm
            ERR_0108, //C-Box Gas Alarm
            ERR_0109,
            ERR_0110, //All Home Error
            ERR_0111, //Spindle Part Home Error
            ERR_0112, //Polishing Part Home Error
            ERR_0113, //Cleaning Part Home Error
            ERR_0114, //Storage Part Home Error
            ERR_0115, //Transfer Part Home Error
            ERR_0116,
            ERR_0117, 
            ERR_0118,
            ERR_0119, //SYSTEM-Machine Hold Error
            ERR_0120, //SYSTEM-THREAD_00 ERROR(Reboot Now!!!)
            ERR_0121, //SYSTEM-THREAD_01 ERROR(Reboot Now!!!)
            ERR_0122, //SYSTEM-THREAD_02 ERROR(Reboot Now!!!)
            ERR_0123, //SYSTEM-THREAD_03 ERROR(Reboot Now!!!)
            ERR_0124, //SYSTEM-THREAD_04 ERROR(Reboot Now!!!)
            ERR_0125, //SYSTEM-THREAD_05 ERROR(Reboot Now!!!)
            ERR_0126, //SYSTEM-THREAD_06 ERROR(Reboot Now!!!)
            ERR_0127, //SYSTEM-THREAD_07 ERROR(Reboot Now!!!)
            ERR_0128, //SYSTEM-THREAD_08 ERROR(Reboot Now!!!)
            ERR_0129, //SYSTEM-THREAD_09 ERROR(Reboot Now!!!)
            ERR_0130, //Door Error - Right
            ERR_0131, //Door Error - Left    
            ERR_0132, //Door Error - Side
            ERR_0133,
            ERR_0134,
            ERR_0135, //흡기 FAN#1 ALARM
            ERR_0136, //흡기 FAN#2 ALARM
            ERR_0137, //배기 FAN#1 ALARM
            ERR_0138, //배기 FAN#2 ALARM
            ERR_0139, //PC BOX 배기 FAN#1 ALARM
            ERR_0140, //Leak-Polishing
            ERR_0141, //Leak-Cleaning Bottom
            ERR_0142, //Leak-Cleaning Top
            ERR_0143, //Leak-Local Bottom Plate
            ERR_0144, //Leak-Right Top Base
            ERR_0145, //Leak-Bottom Sol Box
            ERR_0146, //Leak-Utility Inlet or Local Floor
            ERR_0147, //Leak-Settling
            ERR_0148, //Water Level - Bath Drain
            ERR_0149, //Water Level - Settling
            ERR_0150, //Spindle Part ToStart TimeOut 
            ERR_0151, //Polishing Part ToStart TimeOut
            ERR_0152, //Cleaning Part ToStart TimeOut
            ERR_0153, //Storage Part ToStart TimoOut
            ERR_0154, //Transfer Part ToStart TimeOut
            ERR_0155, //DP - 배기 In 이상
            ERR_0156, //DP - 배기 Out 이상
            ERR_0157,
            ERR_0158,
            ERR_0159,
            ERR_0160, //Spindle Part ToStop TimeOut 
            ERR_0161, //Polishing Part ToStop TimeOut
            ERR_0162, //Cleaning Part ToStop TimeOut
            ERR_0163, //Storage Part ToStop TimoOut
            ERR_0164, //Transfer Part ToStop TimeOut
            ERR_0165, //Sequence ToStop TimeOut
            ERR_0166,
            ERR_0167,
            ERR_0168,
            ERR_0169,
            ERR_0170, //Spindle Main Cycle Time Out
            ERR_0171, //Polishing Main Cycle Time Out
            ERR_0172, //Cleaning Main Cycle Time Out
            ERR_0173, //Storage Main Cycle Time Out
            ERR_0174, //Transfer Main Cycle Time Out
            ERR_0175,
            ERR_0176,
            ERR_0177,
            ERR_0178,
            ERR_0179, //[Cleaning] Home Cycle Time Out  
            ERR_0180, //[Spindle] ToolPickCycle  Time Out
            ERR_0181, //[Spindle] ToolChkCycle   Time Out
            ERR_0182, //[Spindle] ForceChkCycle  Time Out
            ERR_0183, //[Spindle] PolishCycle    Time Out
            ERR_0184, //[Spindle] CleanCycle     Time Out
            ERR_0185, //[Spindle] ToolPlaceCycle Time Out
            ERR_0186, //[Spindle] VisnInspCycle  Time Out
            ERR_0187, //[Spindle] UtilChkCycle   Time Out
            ERR_0188, //[Spindle] MagzPickCycle  Time Out
            ERR_0189, //[Spindle] MagzPlaceCycle Time Out
            ERR_0190, //[Polishing] Inspect Cycle Time Out
            ERR_0191, //[Polishing] Polishing Cycle Time Out
            ERR_0192, //[Polishing] Utility Cycle Time Out
            ERR_0193, //[Polishing] Drain Cycle Time Out
            ERR_0194, //[Cleaning] Cleaning Cycle Time Out
            ERR_0195, //[Cleaning] Utility Cycle Time Out
            ERR_0196, //[Storage] Step Cycle Time Out
            ERR_0197, //[Storage] Tool Out Cycle Time Out
            ERR_0198, //[Transfer] Loading Cycle Time Out
            ERR_0199, //[Transfer] Unloading Cycle Time Out
            //---------------------------------------------------------------------------
            ERR_0200, //SPINDLE X AXIS ALARM
            ERR_0201, //SPINDLE X AXIS +LIMIT
            ERR_0202, //SPINDLE X AXIS -LIMIT
            ERR_0203, //SPINDLE X AXIS Initial
            ERR_0204, //SPINDLE X AXIS CONTROL ERROR
            ERR_0205, //SPINDLE X AXIS HOLDING (SAFETY Sensor)
            ERR_0206, //SPINDLE X AXIS Position PARAMETER LIMIT
            ERR_0207, //SPINDLE X AXIS Speed PARAMETER LIMIT
            ERR_0208, //SPINDLE X AXIS Acc/Dec PARAMETER LIMIT
            ERR_0209, //
            ERR_0210, //POLISHING Y AXIS ALARM
            ERR_0211, //POLISHING Y AXIS +LIMIT
            ERR_0212, //POLISHING Y AXIS -LIMIT
            ERR_0213, //POLISHING Y AXIS Initial
            ERR_0214, //POLISHING Y AXIS CONTROL ERROR
            ERR_0215, //POLISHING Y AXIS HOLDING (SAFETY Sensor)
            ERR_0216, //POLISHING Y AXIS Position PARAMETER LIMIT 
            ERR_0217, //POLISHING Y AXIS Speed PARAMETER LIMIT
            ERR_0218, //POLISHING Y AXIS Acc/Dec PARAMETER LIMIT
            ERR_0219, //
            ERR_0220, //SPINDLE Z AXIS ALARM
            ERR_0221, //SPINDLE Z AXIS +LIMIT
            ERR_0222, //SPINDLE Z AXIS -LIMIT
            ERR_0223, //SPINDLE Z AXIS Initial
            ERR_0224, //SPINDLE Z AXIS CONTROL ERROR
            ERR_0225, //SPINDLE Z AXIS HOLDING (SAFETY Sensor)
            ERR_0226, //SPINDLE Z AXIS Position PARAMETER LIMIT 
            ERR_0227, //SPINDLE Z AXIS Speed PARAMETER LIMIT 
            ERR_0228, //SPINDLE Z AXIS Acc/Dec PARAMETER LIMIT 
            ERR_0229, //
            ERR_0230, //CLEANING R AXIS ALARM
            ERR_0231, //CLEANING R AXIS +LIMIT
            ERR_0232, //CLEANING R AXIS -LIMIT
            ERR_0233, //CLEANING R AXIS Initial
            ERR_0234, //CLEANING R AXIS CONTROL ERROR
            ERR_0235, //CLEANING R AXIS HOLDING (SAFETY Sensor)
            ERR_0236, //CLEANING R AXIS Position PARAMETER LIMIT
            ERR_0237, //CLEANING R AXIS Speed PARAMETER LIMIT
            ERR_0238, //CLEANING R AXIS Acc/Dec PARAMETER LIMIT
            ERR_0239, //
            ERR_0240, //POLISHING TH AXIS ALARM
            ERR_0241, //POLISHING TH AXIS +LIMIT
            ERR_0242, //POLISHING TH AXIS -LIMIT
            ERR_0243, //POLISHING TH AXIS Initial
            ERR_0244, //POLISHING TH AXIS CONTROL ERROR
            ERR_0245, //POLISHING TH AXIS HOLDING (SAFETY Sensor)
            ERR_0246, //POLISHING TH AXIS Position PARAMETER LIMIT
            ERR_0247, //POLISHING TH AXIS Speed PARAMETER LIMIT
            ERR_0248, //POLISHING TH AXIS Acc/Dec PARAMETER LIMIT
            ERR_0249, //
            ERR_0250, //POLISHING TI AXIS ALARM
            ERR_0251, //POLISHING TI AXIS +LIMIT
            ERR_0252, //POLISHING TI AXIS -LIMIT
            ERR_0253, //POLISHING TI AXIS Initial
            ERR_0254, //POLISHING TI AXIS CONTROL ERROR
            ERR_0255, //POLISHING TI AXIS HOLDING (SAFETY Sensor)
            ERR_0256, //POLISHING TI AXIS Position PARAMETER LIMIT
            ERR_0257, //POLISHING TI AXIS Speed PARAMETER LIMIT
            ERR_0258, //POLISHING TI AXIS Acc/Dec PARAMETER LIMIT
            ERR_0259, //
            ERR_0260, //STORAGE Y AXIS ALARM
            ERR_0261, //STORAGE Y AXIS +LIMIT
            ERR_0262, //STORAGE Y AXIS -LIMIT
            ERR_0263, //STORAGE Y AXIS Initial
            ERR_0264, //STORAGE Y AXIS CONTROL ERROR
            ERR_0265, //STORAGE Y AXIS HOLDING (SAFETY Sensor)
            ERR_0266, //STORAGE Y AXIS Position PARAMETER LIMIT
            ERR_0267, //STORAGE Y AXIS Speed PARAMETER LIMIT
            ERR_0268, //STORAGE Y AXIS Acc/Dec PARAMETER LIMIT
            ERR_0269, //
            ERR_0270, //CLEANING Y AXIS ALARM
            ERR_0271, //CLEANING Y AXIS +LIMIT
            ERR_0272, //CLEANING Y AXIS -LIMIT
            ERR_0273, //CLEANING Y AXIS Initial
            ERR_0274, //CLEANING Y AXIS CONTROL ERROR
            ERR_0275, //CLEANING Y AXIS HOLDING (SAFETY Sensor)
            ERR_0276, //CLEANING Y AXIS Position PARAMETER LIMIT
            ERR_0277, //CLEANING Y AXIS Speed PARAMETER LIMIT
            ERR_0278, //CLEANING Y AXIS Acc/Dec PARAMETER LIMIT
            ERR_0279, //
            ERR_0280, //SPINDLE Z1 AXIS ALARM
            ERR_0281, //SPINDLE Z1 AXIS +LIMIT
            ERR_0282, //SPINDLE Z1 AXIS -LIMIT
            ERR_0283, //SPINDLE Z1 AXIS Initial
            ERR_0284, //SPINDLE Z1 AXIS CONTROL ERROR
            ERR_0285, //SPINDLE Z1 AXIS HOLDING (SAFETY Sensor)
            ERR_0286, //SPINDLE Z1 AXIS Position PARAMETER LIMIT
            ERR_0287, //SPINDLE Z1 AXIS Speed PARAMETER LIMIT 
            ERR_0288, //SPINDLE Z1 AXIS Acc/Dec PARAMETER LIMIT 
            ERR_0289, //
            ERR_0290, //TRANSFER Z AXIS ALARM
            ERR_0291, //TRANSFER Z AXIS +LIMIT
            ERR_0292, //TRANSFER Z AXIS -LIMIT
            ERR_0293, //TRANSFER Z AXIS Initial
            ERR_0294, //TRANSFER Z AXIS CONTROL ERROR
            ERR_0295, //TRANSFER Z AXIS HOLDING (SAFETY Sensor)
            ERR_0296, //TRANSFER Z AXIS Position PARAMETER LIMIT
            ERR_0297, //TRANSFER Z AXIS Speed PARAMETER LIMIT
            ERR_0298, //TRANSFER Z AXIS Acc/Dec PARAMETER LIMIT
            ERR_0299, //
            ERR_0300, //LOAD T AXIS ALARM
            ERR_0301, //LOAD T AXIS +LIMIT
            ERR_0302, //LOAD T AXIS -LIMIT
            ERR_0303, //LOAD T AXIS Initial
            ERR_0304, //LOAD T AXIS CONTROL ERROR
            ERR_0305, //LOAD T AXIS HOLDING (SAFETY Sensor)
            ERR_0306, //LOAD T AXIS Position PARAMETER LIMIT
            ERR_0307, //LOAD T AXIS Speed PARAMETER LIMIT
            ERR_0308, //LOAD T AXIS Acc/Dec PARAMETER LIMIT
            ERR_0309, //
            ERR_0310, //
            ERR_0311, //
            ERR_0312, //
            ERR_0313, //
            ERR_0314, //
            ERR_0315, //
            ERR_0316, //
            ERR_0317, //
            ERR_0318, //
            ERR_0319, //
            ERR_0320, //
            ERR_0321, //
            ERR_0322, //
            ERR_0323, //
            ERR_0324, //
            ERR_0325, //
            ERR_0326, //
            ERR_0327, //
            ERR_0328, //
            ERR_0329, //
            ERR_0330, //Tool Pick Error
            ERR_0331, //Tool Place Error
            ERR_0332, //Plate Pick Error
            ERR_0333, //Plate Place Error
            ERR_0334,
            ERR_0335, //SPINDLE - Z-Axis Soft Limit Error
            ERR_0336, //Spindle - Force Check Time Error
            ERR_0337,
            ERR_0338,
            ERR_0339,
            ERR_0340, //Spindle - E3000 Error
            ERR_0341, //Spindle - Force Check ACS Buffer Error
            ERR_0342, //Spindle - Polishing ACS Buffer Error
            ERR_0343, //Spindle - Tool Exist Error while tool change
            ERR_0344, //Spindle - Force Check Error
            ERR_0345, //Spindle - Polishing Tool Exist Error
            ERR_0346, //Spindle - Cleaning Tool Exist Error
            ERR_0347,
            ERR_0348,
            ERR_0349,
            ERR_0350, //Load Port Plate Loss Error
            ERR_0351, //Transfer Port Plate Loss Error
            ERR_0352, //Main-X Plate Loss Error
            ERR_0353, //Polishing Cup Loss Error
            ERR_0354, //Polishing Cup Unknown Error
            ERR_0355, //Load Plate Unknown Error
            ERR_0356, //Transfer Port Plate Unknown Error
            ERR_0357, //Main-X Plate Unknown Error
            ERR_0358, //Polishing Plate Unknown Error
            ERR_0359, //Cleaning Plate Unknown Error
            ERR_0360, //[Main-Z] Tool Loss Error
            ERR_0361, //[Main-Z] Tool Unknown Error
            ERR_0362,
            ERR_0363,
            ERR_0364,
            ERR_0365,
            ERR_0366,
            ERR_0367,
            ERR_0368,
            ERR_0369,
            ERR_0370, //Main-Z Plate Recipe Error
            ERR_0371, //STORAGE - Tool Storage Discard Box Check Error
            ERR_0372,
            ERR_0373,
            ERR_0374,
            ERR_0375,
            ERR_0376,
            ERR_0377,
            ERR_0378,
            ERR_0379,
            ERR_0380,
            ERR_0381,
            ERR_0382,
            ERR_0383,
            ERR_0384,
            ERR_0385,
            ERR_0386,
            ERR_0387,
            ERR_0388,
            ERR_0389,
            ERR_0390,
            ERR_0391,
            ERR_0392,
            ERR_0393,
            ERR_0394,
            ERR_0395,
            ERR_0396,
            ERR_0397,
            ERR_0398,
            ERR_0399,
            ERR_0400, //POLISHING_UNIT_LEAK_DOWN
            ERR_0401, //POLISHING_UNIT_LEAK_ROOM_BOTTOM
            ERR_0402, //POLISHING_UNIT_LEAK_TANK_ROOM     
            ERR_0403, //POLISHING_UNIT_LEAK_VALVE_BOX
            ERR_0404, //POLISHING_UNIT_LEAK_LEAK_BOX
            ERR_0405, //POLISHING_SUP_TK_A_교반기_이상
            ERR_0406, //POLISHING_SUP_TK_A_LEVEL_HH
            ERR_0407, //POLISHING_SUP_TK_A_LEVEL_L_OFF
            ERR_0408, //POLISHING_SUP_TK_A_LEVEL_EMPTY
            ERR_0409, //POLISHING_SUPPLY_유량_HIGH
            ERR_0410, //POLISHING_SUPPLY_유량_LOW
            ERR_0411, //POLISHING_RETURN_유량_HIGH
            ERR_0412, //POLISHING_RETURN_유량_LOW
            ERR_0413, //POLISHING_RETURN_압력_HIGH
            ERR_0414, //POLISHING_RETURN_압력_LOW
            ERR_0415, //POLISHING_SUP_TK_A_CHARGE_REQUEST
            ERR_0416, //Utility Supply TimeOut Error
            ERR_0417, //DI Supply Time Out Error
            ERR_0418, //Auto Supply System Reply Error
            ERR_0419, //
            ERR_0420, //SOAP_UNIT_LEAK_DOWN
            ERR_0421, //SOAP_UNIT_LEAK_ROOM_BOTTOM
            ERR_0422, //SOAP_UNIT_LEAK_TANK_ROOM    
            ERR_0423, //SOAP_UNIT_LEAK_VALVE_BOX
            ERR_0424, //SOAP_UNIT_LEAK_LEAK_BOX
            ERR_0425, //SOAP_SUP_TK_A_교반기_이상
            ERR_0426, //SOAP_SUP_TK_A_LEVEL_HH
            ERR_0427, //SOAP_SUP_TK_A_LEVEL_L_OFF
            ERR_0428, //SOAP_SUP_TK_A_LEVEL_EMPTY
            ERR_0429, //SOAP_SUPPLY_유량_HIGH
            ERR_0430, //SOAP_SUPPLY_유량_LOW
            ERR_0431, //SOAP_RETURN_유량_HIGH
            ERR_0432, //SOAP_RETURN_유량_LOW
            ERR_0433, //SOAP_RETURN_압력_HIGH
            ERR_0434, //SOAP_RETURN_압력_LOW
            ERR_0435, //SOAP_SUP_TK_A_CHARGE_REQUEST
            ERR_0436, //CLEANING_Util Level Limit Error
            ERR_0437, //CLEANING - Cleaning Position Error
            ERR_0438, //CLEANING - Cleaning Data Error
            ERR_0439, //DEHYDRATE - Cleaning Data Error
            ERR_0440,
            ERR_0441,
            ERR_0442,
            ERR_0443,
            ERR_0444,
            ERR_0445,
            ERR_0446,
            ERR_0447,
            ERR_0448,
            ERR_0449,
            ERR_0450, //Vision Inspection Error
            ERR_0451, //Pre Align Vision Inspection Error
            ERR_0452, //Auto Supply EQP ID Error - SLURRY
            ERR_0453, //Auto Supply EQP ID Error - SOAP
            ERR_0454,
            ERR_0455,
            ERR_0456,
            ERR_0457,
            ERR_0458,
            ERR_0459,
            ERR_0460, //Cylinder TimeOut - Spindle - Lens Cover           //aSpdl_LensCovr    
            ERR_0461, //Cylinder TimeOut - Spindle - Plate Clamp          //aSpdl_PlateClamp  
            ERR_0462, //Cylinder TimeOut - Spindle - IR Shutter           //aspdl_IR          
            ERR_0463, //Cylinder TimeOut - Polishing - Plate Clamp        //aPoli_Clamp       
            ERR_0464, //Cylinder TimeOut - Cleaning - Plate Clamp         //aClen_Clamp       
            ERR_0465, //Cylinder TimeOut - Storage - Lock Bottom Up/Down  //aStor_Lock_BtmUD
            ERR_0466, //Cylinder TimeOut - Storage - Lock Top Up/Down     //aStor_Lock_TopUD
            ERR_0467, //Cylinder TimeOut - Transfer - Top Load Fwd/Bwd    //aTran_TopLoadFB  
            ERR_0468, //Cylinder TimeOut - Transfer - Top Load 0/180      //aTran_TopLoadTurn
            ERR_0469, //Cylinder TimeOut - Transfer - Bottom Load Fwd/Bwd //aTran_BtmLoadFB  
            ERR_0470, //Cylinder TimeOut - Transfer - Load Port Up/Down   //aTran_LoadPortUD 
            ERR_0471, //Cylinder TimeOut - Transfer - Magazine Move LF/RT //aTran_MagaMoveLR 
            ERR_0472, //Cylinder TimeOut - Transfer - Load Cover Fwd/Bwd  //aTran_LoadCover  
            ERR_0473, //Cylinder TimeOut - 
            ERR_0474, //Cylinder TimeOut - 
            ERR_0475,
            ERR_0476,
            ERR_0477,
            ERR_0478,
            ERR_0479,
            ERR_0480,
            ERR_0481,
            ERR_0482,
            ERR_0483,
            ERR_0484,
            ERR_0485,
            ERR_0486,
            ERR_0487,
            ERR_0488, //Magazine - Empty Error : Polishing Tool 
            ERR_0489, //Magazine - Empty Error : Cleaning Tool 
            ERR_0490, //Magazine Empty Slot Error 
            ERR_0491, //Magazine Empty Error - 작업 가능한 Plate가 없습니다. 
            ERR_0492, //Magazine - Plate Loss Error 
            ERR_0493, //Magazine - Plate Unknown Error 
            ERR_0494, //Magazine - Pitch Sensor Error 
            ERR_0495, //Magazine - Left Exist Error
            ERR_0496, //Magazine - Right Exist Error
            ERR_0497, //Magazine - Tool Type Error (Check tool type of storage)
            ERR_0498, 
            ERR_0499, //LOT - Transfer Recipe Open Error
            ERR_0500, //LOT - Transfer Recipe Name Error

            ERR_0501, //[ERROR] CP01 AX UPS PILOT 
            ERR_0502, //[ERROR] CP02 AX UPS PANEL 
            ERR_0503, //[ERROR] CP03 AX ACCURA 01 POWER
            ERR_0504, //[ERROR] CP04 AX SMPS01 AC POWER
            ERR_0505, //[ERROR] CP05 AX SMPS02 AC POWER
            ERR_0506, //[ERROR] CP06 AX SMPS01 DC POWER
            ERR_0507, //[ERROR] CP07 AX SMPS02 DC POWER
            ERR_0508, //[ERROR] CP08 AX IO POWER
            ERR_0509, //[ERROR] CP09 AX BECKHOFF POWER
            ERR_0510, //[ERROR] MCCB01 AX ON/OFF
            ERR_0511, //[ERROR] MC 01 AX (SERVO_1ST_MC)
            ERR_0512, //[ERROR] MC 02 AX (SERVO_2ND_MC)
            ERR_0513, //[ERROR] CP10 AX Z1,X,Y1 SERVO AMP POWER
            ERR_0514, //[ERROR] CP11 AX Z2 SERVO AMP POWER
            ERR_0515, //[ERROR] CP12 AX C3,C4 SERVO AMP POWER
            ERR_0516, //[ERROR] CP13 AX Z3 SERVO AMP POWER
            ERR_0517, //[ERROR] CP14 AX FAN POWER
            ERR_0518, //[ERROR] CP15 AX Z1 SERVO MOTOR POWER
            ERR_0519, //[ERROR] CP16 AX Z2 SERVO MOTOR POWER
            ERR_0520, //[ERROR] CP17 AX C3 SERVO MOTOR POWER
            ERR_0521, //[ERROR] CP18 AX C4 SERVO MOTOR POWER
            ERR_0522, //
            ERR_0523, //[ERROR] CP20 AX Y2 SERVO AMP POWER
            ERR_0524, //[ERROR] CP21 AX Y3 SERVO AMP POWER
            ERR_0525, //[ERROR] CP22 AX A SERVO AMP POWER
            ERR_0526, //[ERROR] CP23 AX C1 SERVO AMP POWER
            ERR_0527, //[ERROR] CP24 AX C2 SERVO AMP POWER
            ERR_0528, //[ERROR] CP25 AX Y4 SERVO AMP POWER
            ERR_0529, //[ERROR] CP26 AX E3000C POWER
            ERR_0530, //[ERROR] CP40 AX X,Y1 axis (Main,Polishing Stage)
            ERR_0531, //[ERROR] CP41 AX Y2-axis(Tool Storage Stage)
            ERR_0532, //[ERROR] CP42 AX Y3-axis (Cleaning Stage)
            ERR_0533, //[ERROR] CP43 AX A-axis(Polishing Tilting)
            ERR_0534, //[ERROR] CP44 AX C1-axis(Polishing Rotation)
            ERR_0535, //[ERROR] CP45 AX C2-axis(Cleaning Rotation)
            ERR_0536, //[ERROR] CP46 AX Y4-axis (Loading/Unloading )
            ERR_0537, //[ERROR] CP47 AX DC MOTOR SMPS POWER
            ERR_0538, //[ERROR] CP48 AX DC MOTOR DRIVE POWER
            ERR_0539, //
            ERR_0540, //
            ERR_0541, //
            ERR_0542, //
            ERR_0543, //
            ERR_0544, //
            ERR_0545, //
            ERR_0546, //
            ERR_0547, //
            ERR_0548, //
            ERR_0549, //
            ERR_0550, //
            ERR_0551, //
            ERR_0552, //
            ERR_0553, //
            ERR_0554, //
            ERR_0555, //
            ERR_0556, //
            ERR_0557, //
            ERR_0558, //
            ERR_0559, //
            ERR_0560, //Polishing Bath Theta Align Error
            ERR_0561, //Polishing Bath Theta Align Error - Out Of Range : Theta
            ERR_0562, //Polishing Bath X,Y Align Error
            ERR_0563, //Polishing Bath X,Y Align Error - Inposition Error
            ERR_0564, //Vision Recipe Error - Enable Error
            ERR_0565, //
            ERR_0566, //
            ERR_0567, //
            ERR_0568, //
            ERR_0569, //
            ERR_0570, //
            ERR_0571, //
            ERR_0572, //
            ERR_0573, //
            ERR_0574, //
            ERR_0575, //
            ERR_0576, //
            ERR_0577, //
            ERR_0578, //
            ERR_0579, //
            ERR_0580, //
            ERR_0581, //
            ERR_0582, //
            ERR_0583, //
            ERR_0584, //
            ERR_0585, //
            ERR_0586, //
            ERR_0587, //
            ERR_0588, //
            ERR_0589, //
            ERR_0590, //
            ERR_0591, //
            ERR_0592, //
            ERR_0593, //
            ERR_0594, //
            ERR_0595, //
            ERR_0596, //
            ERR_0597, //
            ERR_0598, //
            ERR_0599, //

            EndofId
        }
        //---------------------------------------------------------------------------
        static public string GETNAME(int n)
        {
            return STR_ERRNAME[n];
        }

        //
        public static string[] STR_ERRNAME =
        {
            "" , //ERR_0001 =
            "" , //ERR_0002, //
            "" , //ERR_0003, //
            "" , //ERR_0004, //
            "" , //ERR_0005, //
            "" , //ERR_0006, //
            "" , //ERR_0007,
            "" , //ERR_0008,
            "" , //ERR_0009,
            "" , //ERR_0010,
            "" , //ERR_0011,
            "" , //ERR_0012,
            "" , //ERR_0013,
            "" , //ERR_0014,
            "" , //ERR_0015, //
            "" , //ERR_0016, //
            "" , //ERR_0017, //
            "" , //ERR_0018, //
            "" , //ERR_0019, //
            "" , //ERR_0020, //
            "" , //ERR_0021, //
            "" , //ERR_0022, //
            "" , //ERR_0023, //
            "" , //ERR_0024, //
            "" , //ERR_0025, //
            "" , //ERR_0026, //
            "" , //ERR_0027, //
            "" , //ERR_0028,
            "" , //ERR_0029,
            "" , //ERR_0030,
            "" , //ERR_0031, //
            "" , //ERR_0032, //
            "" , //ERR_0033, //
            "" , //ERR_0034, //
            "" , //ERR_0035, //
            "" , //ERR_0036, //
            "" , //ERR_0037, //
            "" , //ERR_0038, //
            "" , //ERR_0039,
            "" , //ERR_0040,
            "" , //ERR_0041,
            "" , //ERR_0042,
            "" , //ERR_0043,
            "" , //ERR_0044,
            "" , //ERR_0045,
            "" , //ERR_0046,
            "" , //ERR_0047,
            "" , //ERR_0048,
            "" , //ERR_0049,
            "" , //ERR_0050, //
            "" , //ERR_0051, //
            "" , //ERR_0052,
            "" , //ERR_0053,
            "" , //ERR_0054,
            "" , //ERR_0055,
            "" , //ERR_0056,
            "" , //ERR_0057,
            "" , //ERR_0058,
            "" , //ERR_0059,
            "" , //ERR_0060, //
            "" , //ERR_0061,
            "" , //ERR_0062,
            "" , //ERR_0063,
            "" , //ERR_0064,
            "" , //ERR_0065,
            "" , //ERR_0066,
            "" , //ERR_0067,
            "" , //ERR_0068,
            "" , //ERR_0069,
            "" , //ERR_0070, //
            "" , //ERR_0071,
            "" , //ERR_0072,
            "" , //ERR_0073,
            "" , //ERR_0074,
            "" , //ERR_0075,
            "" , //ERR_0076,
            "" , //ERR_0077,
            "" , //ERR_0078,
            "" , //ERR_0079,
            "" , //ERR_0080, //
            "" , //ERR_0081, //
            "" , //ERR_0082, //
            "" , //ERR_0083,
            "" , //ERR_0084,
            "" , //ERR_0085,
            "" , //ERR_0086,
            "" , //ERR_0087,
            "" , //ERR_0088,
            "" , //ERR_0089,
            "" , //ERR_0090,
            "" , //ERR_0091,
            "" , //ERR_0092,
            "" , //ERR_0093,
            "" , //ERR_0094,
            "" , //ERR_0095,
            "" , //ERR_0096,
            "" , //ERR_0097,
            "" , //ERR_0098,
            "" , //ERR_0099,//[
            "" , //ERR_0100,
            "" , //ERR_0101, //
            "" , //ERR_0102, //
            "" , //ERR_0103, //
            "ACS Network Error" , //ERR_0104, //
            "" , //ERR_0105, //
            "" , //ERR_0106, //
            "" , //ERR_0107, //
            "" , //ERR_0108, //
            "" , //ERR_0109,
            "" , //ERR_0110, //
            "" , //ERR_0111, //
            "" , //ERR_0112, //
            "" , //ERR_0113, //
            "" , //ERR_0114, //
            "" , //ERR_0115, //
            "" , //ERR_0116,
            "" , //ERR_0117,
            "" , //ERR_0118,
            "" , //ERR_0119, //
            "" , //ERR_0120, //
            "" , //ERR_0121, //
            "" , //ERR_0122, //
            "" , //ERR_0123, //
            "" , //ERR_0124, //
            "" , //ERR_0125, //
            "" , //ERR_0126, //
            "" , //ERR_0127, //
            "" , //ERR_0128, //
            "" , //ERR_0129, //
            "" , //ERR_0130, //
            "" , //ERR_0131, //
            "" , //ERR_0132, //
            "" , //ERR_0133,
            "" , //ERR_0134,
            "" , //ERR_0135, //
            "" , //ERR_0136, //
            "" , //ERR_0137, //
            "" , //ERR_0138, //
            "" , //ERR_0139, //
            "" , //ERR_0140, //
            "" , //ERR_0141, //
            "" , //ERR_0142, //
            "" , //ERR_0143, //
            "" , //ERR_0144, //
            "" , //ERR_0145, //
            "" , //ERR_0146, //
            "" , //ERR_0147, //
            "" , //ERR_0148, //
            "" , //ERR_0149, //
            "" , //ERR_0150, //
            "" , //ERR_0151, //
            "" , //ERR_0152, //
            "" , //ERR_0153, //
            "" , //ERR_0154, //
            "DP - 배기 In 이상" , //ERR_0155,
            "DP - 배기 Out 이상" , //ERR_0156,
            "" , //ERR_0157,
            "" , //ERR_0158,
            "" , //ERR_0159,
            "" , //ERR_0160, //
            "" , //ERR_0161, //
            "" , //ERR_0162, //
            "" , //ERR_0163, //
            "" , //ERR_0164, //
            "" , //ERR_0165, //
            "" , //ERR_0166,
            "" , //ERR_0167,
            "" , //ERR_0168,
            "" , //ERR_0169,
            "" , //ERR_0170, //
            "" , //ERR_0171, //
            "" , //ERR_0172, //
            "" , //ERR_0173, //
            "" , //ERR_0174, //
            "" , //ERR_0175,
            "" , //ERR_0176,
            "" , //ERR_0177,
            "" , //ERR_0178,
            "[Cleaning] Home Cycle Time Out" , //ERR_0179,
            "" , //ERR_0180, //
            "" , //ERR_0181, //
            "" , //ERR_0182, //
            "[Spindle] PolishCycle Time Out" , //ERR_0183, //
            "[Spindle] CleanCycle  Time Out" , //ERR_0184, //
            "" , //ERR_0185, //
            "" , //ERR_0186, //
            "" , //ERR_0187, //
            "" , //ERR_0188, //
            "" , //ERR_0189, //
            "" , //ERR_0190, //
            "" , //ERR_0191, //
            "" , //ERR_0192, //
            "" , //ERR_0193, //
            "" , //ERR_0194, //
            "" , //ERR_0195, //
            "" , //ERR_0196, //
            "" , //ERR_0197, //
            "" , //ERR_0198, //
            "" , //ERR_0199, //
            "" , //ERR_0200, //
            "" , //ERR_0201, //
            "" , //ERR_0202, //
            "" , //ERR_0203, //
            "" , //ERR_0204, //
            "" , //ERR_0205, //
            "" , //ERR_0206, //
            "" , //ERR_0207, //
            "" , //ERR_0208, //
            "" , //ERR_0209, //
            "" , //ERR_0210, //
            "" , //ERR_0211, //
            "" , //ERR_0212, //
            "" , //ERR_0213, //
            "" , //ERR_0214, //
            "" , //ERR_0215, //
            "" , //ERR_0216, //
            "" , //ERR_0217, //
            "" , //ERR_0218, //
            "" , //ERR_0219, //
            "" , //ERR_0220, //
            "" , //ERR_0221, //
            "" , //ERR_0222, //
            "" , //ERR_0223, //
            "" , //ERR_0224, //
            "" , //ERR_0225, //
            "" , //ERR_0226, //
            "" , //ERR_0227, //
            "" , //ERR_0228, //
            "" , //ERR_0229, //
            "" , //ERR_0230, //
            "" , //ERR_0231, //
            "" , //ERR_0232, //
            "" , //ERR_0233, //
            "" , //ERR_0234, //
            "" , //ERR_0235, //
            "" , //ERR_0236, //
            "" , //ERR_0237, //
            "" , //ERR_0238, //
            "" , //ERR_0239, //
            "" , //ERR_0240, //
            "" , //ERR_0241, //
            "" , //ERR_0242, //
            "" , //ERR_0243, //
            "" , //ERR_0244, //
            "" , //ERR_0245, //
            "" , //ERR_0246, //
            "" , //ERR_0247, //
            "" , //ERR_0248, //
            "" , //ERR_0249, //
            "" , //ERR_0250, //
            "" , //ERR_0251, //
            "" , //ERR_0252, //
            "" , //ERR_0253, //
            "" , //ERR_0254, //
            "" , //ERR_0255, //
            "" , //ERR_0256, //
            "" , //ERR_0257, //
            "" , //ERR_0258, //
            "" , //ERR_0259, //
            "" , //ERR_0260, //
            "" , //ERR_0261, //
            "" , //ERR_0262, //
            "" , //ERR_0263, //
            "" , //ERR_0264, //
            "" , //ERR_0265, //
            "" , //ERR_0266, //
            "" , //ERR_0267, //
            "" , //ERR_0268, //
            "" , //ERR_0269, //
            "" , //ERR_0270, //
            "" , //ERR_0271, //
            "" , //ERR_0272, //
            "" , //ERR_0273, //
            "" , //ERR_0274, //
            "" , //ERR_0275, //
            "" , //ERR_0276, //
            "" , //ERR_0277, //
            "" , //ERR_0278, //
            "" , //ERR_0279, //
            "" , //ERR_0280, //
            "" , //ERR_0281, //
            "" , //ERR_0282, //
            "" , //ERR_0283, //
            "" , //ERR_0284, //
            "" , //ERR_0285, //
            "" , //ERR_0286, //
            "" , //ERR_0287, //
            "" , //ERR_0288, //
            "" , //ERR_0289, //
            "" , //ERR_0290, //
            "" , //ERR_0291, //
            "" , //ERR_0292, //
            "" , //ERR_0293, //
            "" , //ERR_0294, //
            "" , //ERR_0295, //
            "" , //ERR_0296, //
            "" , //ERR_0297, //
            "" , //ERR_0298, //
            "" , //ERR_0299, //
            "" , //ERR_0300, //
            "" , //ERR_0301, //
            "" , //ERR_0302, //
            "" , //ERR_0303, //
            "" , //ERR_0304, //
            "" , //ERR_0305, //
            "" , //ERR_0306, //
            "" , //ERR_0307, //
            "" , //ERR_0308, //
            "" , //ERR_0309, //
            "" , //ERR_0310, //
            "" , //ERR_0311, //
            "" , //ERR_0312, //
            "" , //ERR_0313, //
            "" , //ERR_0314, //
            "" , //ERR_0315, //
            "" , //ERR_0316, //
            "" , //ERR_0317, //
            "" , //ERR_0318, //
            "" , //ERR_0319, //
            "" , //ERR_0320, //
            "" , //ERR_0321, //
            "" , //ERR_0322, //
            "" , //ERR_0323, //
            "" , //ERR_0324, //
            "" , //ERR_0325, //
            "" , //ERR_0326, //
            "" , //ERR_0327, //
            "" , //ERR_0328, //
            "" , //ERR_0329, //
            "" , //ERR_0330, //
            "" , //ERR_0331, //
            "" , //ERR_0332, //
            "" , //ERR_0333, //
            "" , //ERR_0334,
            "" , //ERR_0335, //
            "" , //ERR_0336, //
            "" , //ERR_0337,
            "" , //ERR_0338,
            "" , //ERR_0339,
            "" , //ERR_0340, //
            "" , //ERR_0341, //
            "" , //ERR_0342, //
            "" , //ERR_0343, //
            "Force Check Error" , //ERR_0344,
            "Spindle - Polishing Tool Exist Error" , //ERR_0345,
            "Spindle - Cleaning Tool Exist Error" , //ERR_0346,
            "" , //ERR_0347,
            "" , //ERR_0348,
            "" , //ERR_0349,
            "" , //ERR_0350, //
            "" , //ERR_0351, //
            "" , //ERR_0352, //
            "" , //ERR_0353, //
            "" , //ERR_0354, //
            "" , //ERR_0355, //
            "" , //ERR_0356, //
            "" , //ERR_0357, //
            "" , //ERR_0358, //
            "" , //ERR_0359, //
            "" , //ERR_0360, //
            "" , //ERR_0361, //
            "" , //ERR_0362,
            "" , //ERR_0363,
            "" , //ERR_0364,
            "" , //ERR_0365,
            "" , //ERR_0366,
            "" , //ERR_0367,
            "" , //ERR_0368,
            "" , //ERR_0369,
            "" , //ERR_0370, //
            "" , //ERR_0371, //
            "" , //ERR_0372,
            "" , //ERR_0373,
            "" , //ERR_0374,
            "" , //ERR_0375,
            "" , //ERR_0376,
            "" , //ERR_0377,
            "" , //ERR_0378,
            "" , //ERR_0379,
            "" , //ERR_0380,
            "" , //ERR_0381,
            "" , //ERR_0382,
            "" , //ERR_0383,
            "" , //ERR_0384,
            "" , //ERR_0385,
            "" , //ERR_0386,
            "" , //ERR_0387,
            "" , //ERR_0388,
            "" , //ERR_0389,
            "" , //ERR_0390,
            "" , //ERR_0391,
            "" , //ERR_0392,
            "" , //ERR_0393,
            "" , //ERR_0394,
            "" , //ERR_0395,
            "" , //ERR_0396,
            "" , //ERR_0397,
            "" , //ERR_0398,
            "" , //ERR_0399,
            "" , //ERR_0400, //
            "" , //ERR_0401, //
            "" , //ERR_0402, //
            "" , //ERR_0403, //
            "" , //ERR_0404, //
            "" , //ERR_0405, //
            "" , //ERR_0406, //
            "" , //ERR_0407, //
            "" , //ERR_0408, //
            "" , //ERR_0409, //
            "" , //ERR_0410, //
            "" , //ERR_0411, //
            "" , //ERR_0412, //
            "" , //ERR_0413, //
            "" , //ERR_0414, //
            "" , //ERR_0415, //
            "" , //ERR_0416, //
            "" , //ERR_0417, //
            "Auto Supply System Reply Error" , //ERR_0418, //
            "" , //ERR_0419, //
            "" , //ERR_0420, //
            "" , //ERR_0421, //
            "" , //ERR_0422, //
            "" , //ERR_0423, //
            "" , //ERR_0424, //
            "" , //ERR_0425, //
            "" , //ERR_0426, //
            "" , //ERR_0427, //
            "" , //ERR_0428, //
            "" , //ERR_0429, //
            "" , //ERR_0430, //
            "" , //ERR_0431, //
            "" , //ERR_0432, //
            "" , //ERR_0433, //
            "" , //ERR_0434, //
            "" , //ERR_0435, //
            "" , //ERR_0436, //
            "" , //ERR_0437, //
            "" , //ERR_0438, //
            "" , //ERR_0439, //
            "" , //ERR_0440,
            "" , //ERR_0441,
            "" , //ERR_0442,
            "" , //ERR_0443,
            "" , //ERR_0444,
            "" , //ERR_0445,
            "" , //ERR_0446,
            "" , //ERR_0447,
            "" , //ERR_0448,
            "Polishing Position Data Error(check vision data)" , //ERR_0449,
            "" , //ERR_0450, //
            "" , //ERR_0451, //
            "" , //ERR_0452, //
            "" , //ERR_0453, //
            "" , //ERR_0454,
            "" , //ERR_0455,
            "" , //ERR_0456,
            "" , //ERR_0457,
            "" , //ERR_0458,
            "" , //ERR_0459,
            "" , //ERR_0460, //
            "" , //ERR_0461, //
            "" , //ERR_0462, //
            "" , //ERR_0463, //
            "" , //ERR_0464, //
            "" , //ERR_0465, //
            "" , //ERR_0466, //
            "" , //ERR_0467, //
            "" , //ERR_0468, //
            "" , //ERR_0469, //
            "" , //ERR_0470, //
            "" , //ERR_0471, //
            "" , //ERR_0472, //
            "" , //ERR_0473, //
            "" , //ERR_0474, //
            "" , //ERR_0475,
            "" , //ERR_0476,
            "" , //ERR_0477,
            "" , //ERR_0478,
            "" , //ERR_0479,
            "" , //ERR_0480,
            "" , //ERR_0481,
            "" , //ERR_0482,
            "" , //ERR_0483,
            "" , //ERR_0484,
            "" , //ERR_0485,
            "" , //ERR_0486,
            "" , //ERR_0487,
            "Magazine - Empty Error : Polishing Tool" , //ERR_0488,
            "Magazine - Empty Error : Cleaning Tool " , //ERR_0489,
            "" , //ERR_0490, //
            "" , //ERR_0491, //
            "" , //ERR_0492, //
            "" , //ERR_0493, //
            "" , //ERR_0494, //
            "" , //ERR_0495, //
            "" , //ERR_0496, //
            "" , //ERR_0497, //
            "" , //ERR_0498,
            "" , //ERR_0499, //
            "" , //ERR_0500, //
            "" , //ERR_0501, //
            "" , //ERR_0502, //
            "" , //ERR_0503, //
            "" , //ERR_0504, //
            "" , //ERR_0505, //
            "" , //ERR_0506, //
            "" , //ERR_0507, //
            "" , //ERR_0508, //
            "" , //ERR_0509, //
            "" , //ERR_0510, //
            "" , //ERR_0511, //
            "" , //ERR_0512, //
            "" , //ERR_0513, //
            "" , //ERR_0514, //
            "" , //ERR_0515, //
            "" , //ERR_0516, //
            "" , //ERR_0517, //
            "" , //ERR_0518, //
            "" , //ERR_0519, //
            "" , //ERR_0520, //
            "" , //ERR_0521, //
            "" , //ERR_0522, //
            "" , //ERR_0523, //
            "" , //ERR_0524, //
            "" , //ERR_0525, //
            "" , //ERR_0526, //
            "" , //ERR_0527, //
            "" , //ERR_0528, //
            "" , //ERR_0529, //
            "" , //ERR_0530, //
            "" , //ERR_0531, //
            "" , //ERR_0532, //
            "" , //ERR_0533, //
            "" , //ERR_0534, //
            "" , //ERR_0535, //
            "" , //ERR_0536, //
            "" , //ERR_0537, //
            "" , //ERR_0538, //
            "" , //ERR_0539, //
            "" , //ERR_0540, //
            "" , //ERR_0541, //
            "" , //ERR_0542, //
            "" , //ERR_0543, //
            "" , //ERR_0544, //
            "" , //ERR_0545, //
            "" , //ERR_0546, //
            "" , //ERR_0547, //
            "" , //ERR_0548, //
            "" , //ERR_0549, //
            "Load Theta Align Error" , //ERR_0550, //
            "Load Theta Align Inposition Error - Out Of Range : Theta" , //ERR_0551, //
            "" , //ERR_0552, //
            "" , //ERR_0553, //
            "" , //ERR_0554, //
            "" , //ERR_0555, //
            "" , //ERR_0556, //
            "" , //ERR_0557, //
            "" , //ERR_0558, //
            "" , //ERR_0559, //
            "Polishing Bath Theta Align Error" , //ERR_0560, //
            "Polishing Bath Theta Align Error - Out Of Range : Theta" , //ERR_0561, //
            "Polishing Bath X,Y Align Error" , //ERR_0562, //
            "Polishing Bath X,Y Align Error - Inposition Error" , //ERR_0563, //
            "Vision Recipe Error - Enable Error" , //ERR_0564, //
            "" , //ERR_0565, //
            "" , //ERR_0566, //
            "" , //ERR_0567, //
            "" , //ERR_0568, //
            "" , //ERR_0569, //
            "" , //ERR_0570, //
            "" , //ERR_0571, //
            "" , //ERR_0572, //
            "" , //ERR_0573, //
            "" , //ERR_0574, //
            "" , //ERR_0575, //
            "" , //ERR_0576, //
            "" , //ERR_0577, //
            "" , //ERR_0578, //
            "" , //ERR_0579, //
            "" , //ERR_0580, //
            "" , //ERR_0581, //
            "" , //ERR_0582, //
            "" , //ERR_0583, //
            "" , //ERR_0584, //
            "" , //ERR_0585, //
            "" , //ERR_0586, //
            "" , //ERR_0587, //
            "" , //ERR_0588, //
            "" , //ERR_0589, //
            "" , //ERR_0590, //
            "" , //ERR_0591, //
            "" , //ERR_0592, //
            "" , //ERR_0593, //
            "" , //ERR_0594, //
            "" , //ERR_0595, //
            "" , //ERR_0596, //
            "" , //ERR_0597, //
            "" , //ERR_0598, //
            "" , //ERR_0599, //
            ""   //ERR_0600, //

        };

    }
}
