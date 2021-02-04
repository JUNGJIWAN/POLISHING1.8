using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.BaseUnit
{
    static public class IOMap
    {
        public enum EN_INPUT_ID : int
        {
			xNone                  = -1,
            xSYS_CP01               ,//X0000//CP01 AX UPS PILOT LAMP 
            xSYS_CP02               ,//X0001//CP02 AX UPS PANEL LAMP 
            xSYS_CP03               ,//X0002//CP03 AX ACCURA 01 POWER
            xSYS_CP04               ,//X0003//CP04 AX SMPS01 AC POWER
            xSYS_CP05               ,//X0004//CP05 AX SMPS02 AC POWER
            xSYS_CP06               ,//X0005//CP06 AX SMPS01 DC POWER
            xSYS_CP07               ,//X0006//CP07 AX SMPS02 DC POWER
            xSYS_CP08               ,//X0007//CP08 AX IO POWER
            xSYS_CP09               ,//X0008//CP09 AX BECKHOFF POWER
            xSYS_MCCB01             ,//X0009//MCCB01 AX ON/OFF 감지
            xSYS_MC01               ,//X0010//MC 01 AX (SERVO_1ST_MC)
            xSYS_MC02               ,//X0011//MC 02 AX (SERVO_2ND_MC)
            xSYS_CP10               ,//X0012//CP10 AX Z1,X,Y1 SERVO AMP POWER
            xSYS_CP11               ,//X0013//CP11 AX Z2 SERVO AMP POWER
            xSYS_CP12               ,//X0014//CP12 AX C3,C4 SERVO AMP POWER
            xSYS_CP13               ,//X0015//CP13 AX Z3 SERVO AMP POWER
            xSYS_CP14               ,//X0016//CP14 AX FAN POWER
            xSYS_CP15               ,//X0017//CP15 AX Z1 SERVO MOTOR POWER
            xSYS_CP16               ,//X0018//CP16 AX Z2 SERVO MOTOR POWER
            xSYS_CP17               ,//X0019//CP17 AX C3 SERVO MOTOR POWER
            xSYS_CP18               ,//X0020//CP18 AX C4 SERVO MOTOR POWER
            X0021                   ,//X0021//SPARE
            xSYS_CP20               ,//X0022//CP20 AX Y2 SERVO AMP POWER
            xSYS_CP21               ,//X0023//CP21 AX Y3 SERVO AMP POWER
            xSYS_CP22               ,//X0024//CP22 AX A SERVO AMP POWER
            xSYS_CP23               ,//X0025//CP23 AX C1 SERVO AMP POWER
            xSYS_CP24               ,//X0026//CP24 AX C2 SERVO AMP POWER
            xSYS_CP25               ,//X0027//CP25 AX Y4 SERVO AMP POWER
            xSYS_CP26               ,//X0028//CP26 AX E3000C POWER
            xSYS_CP40               ,//X0029//CP40 AX X,Y1 axis (Main,Polishing Stage)
            xSYS_CP41               ,//X0030//CP41 AX Y2-axis(Tool Storage Stage)
            xSYS_CP42               ,//X0031//CP42 AX Y3-axis (Cleaning Stage)
            xSYS_CP43               ,//X0032//CP43 AX A-axis(Polishing Tilting)
            xSYS_CP44               ,//X0033//CP44 AX C1-axis(Polishing Rotation)
            xSYS_CP45               ,//X0034//CP45 AX C2-axis(Cleaning Rotation)
            xSYS_CP46               ,//X0035//CP46 AX Y4-axis (Loading/Unloading )
            xSYS_CP47               ,//X0036//CP47 AX DC MOTOR SMPS POWER
            xSYS_CP48               ,//X0037//CP48 AX DC MOTOR DRIVE POWER

            xSW_DoorLock            ,//X0038//DOOR LOCK S/W
            xSW_DoorUnlock          ,//X0039//DOOR UNLOCK S/W
            X0040                   ,//X0040//SPARE
            xSYS_CBox_Temp_Alarm    ,//X0041//C-BOX TEMP ALARM
            xSYS_CBox_Gas_Alarm     ,//X0042//C-BOX GAS ALARM
            xSW_Auto                ,//X0043//AUTO SELECT KEY
            xSW_Manual              ,//X0044//MANUAL SELECT KEY
            xSW_Reset               ,//X0045//RESET S/W
            xSW_STOR_Lock           ,//X0046//Tool Storage Lock S/W
            xSW_STOR_Unlock         ,//X0047//Tool Storage UnLock S/W
            xSW_Fan_Limit           ,//X0048//FAN LIMIT S/W LS02
            xSYS_In_FanAlarm_01     ,//X0049//흡기 FAN#1 ALARM
            xSYS_In_FanAlarm_02     ,//X0050//흡기 FAN#2 ALARM
            xSYS_Out_FanAlarm_01    ,//X0051//배기 FAN#1 ALARM
            xSYS_Out_FanAlarm_02    ,//X0052//배기 FAN#2 ALARM
            xSYS_EMO_Front          ,//X0053//EQ_EMS01 FRONT
            xSYS_EMO_Rear           ,//X0054//EQ_EMS02 REAR
            xSYS_EMO_CBox           ,//X0055//EQ_EMS03 CONTROL BOX
            xSYS_DR_Right_KeyIn     ,//X0056//EQ_DOOR01 FRONT RIGHT  KEY check
            xSYS_DR_Left_KeyIn      ,//X0057//EQ_DOOR02 FRONT LEFT  KEY check
            xSYS_DR_SideDoor_KeyIn  ,//X0058//EQ_DOOR03 Side Door Key check
            xSYS_DR_MainClose_R     ,//X0059//EQ_DOOR01 FRONT RIGHT Actuator check
            xSYS_DR_MainClose_L     ,//X0060//EQ_DOOR02 FRONT LEFT Actuator check
            xSYS_DR_SideDoorClose   ,//X0061//EQ_DOOR03 Side Door Actuator check
            xSYS_RelayON1_2         ,//X0062//SAFETY RELAY 01,02 ON
            xSYS_RelayON3           ,//X0063//SAFETY RELAY 03 ON
            xSYS_Safety01           ,//X0064//SAFETY PLC->Beckhoff PLC 01 IN
            xSYS_Safety02           ,//X0065//SAFETY PLC->Beckhoff PLC 02 IN
            xSYS_Safety03           ,//X0066//SAFETY PLC->Beckhoff PLC 03 IN
            xSYS_Safety04           ,//X0067//SAFETY PLC->Beckhoff PLC 04 IN
            xSYS_LEAK_CleanBase1    ,//X0068//Leak_Sensor_1-Cleaning Base01
            xSYS_LEAK_CleanBase2    ,//X0069//Leak_Sensor_2-Cleaning Base02
            xSYS_LEAK_PoliCover     ,//X0070//Leak_Sensor_3-Polishing Cover
            xSYS_LEAK_PoliBottom    ,//X0071//Leak_Sensor_4-Polishing Bottom
            xSYS_LEAK_BtmFrame      ,//X0072//Leak_Sensor_5-Bottom Frame
            xSYS_LEAK_MainPlate     ,//X0073//Leak_Sensor_6-Main Plate
            xSYS_LEAK_SettlingTank  ,//X0074//Leak_Sensor_7-침전조
            xSYS_LEAK_UTInlet       ,//X0075//Leak_Sensor_8-UT Inlet
            X0076                   ,//X0076//
            xSYS_PC_FanAlarm01      ,//X0077//PC BOX Exhaust FAN#1 ALARM
            xSYS_MainAir_TOP        ,//X0078//TOP SOL BOX MAIN AIR
            xSYS_MainAir_BTM        ,//X0079//BOTTOM SOL BOX MAIN AIR
            xTRS_MagaLeftExist      ,//X0080//Magazine Left 제품감지_(B접)
            xTRS_MagaLeftPos        ,//X0081//Magazine Left Sens
            xTRS_MagaRightExist     ,//X0082//Magazine Right 제품감지_(B접)
            xTRS_MagaRightPos       ,//X0083//Magazine Right Sens
            X0084                   ,//X0084//
            X0085                   ,//X0085//
            X0086                   ,//X0086//
            X0087                   ,//X0087//
            xSYS_BathWaterLimit     ,//X0088//Cleaning-수위 SENSOR #1
            xSYS_SettlingWaterLimit ,//X0089//침전조 수위 SENSOR #2
            X0090                   ,//X0090//
            X0091                   ,//X0091//
            X0092                   ,//X0092//
            X0093                   ,//X0093//
            X0094                   ,//X0094//
            X0095                   ,//X0095//
            X0096                   ,//X0096//
            X0097                   ,//X0097//
            X0098                   ,//X0098//DOOR SHUTTER Cylinder OPEN
            X0099                   ,//X0099//DOOR SHUTTER Cylinder CLOSE
            X0100                   ,//X0100//DOOR SHUTTER  제품감지
            xSYS_DP_OUT             ,//X0101//배기 차압계 OUT
            xSYS_DP_IN              ,//X0102//배기 차압계 IN
            X0103                   ,//X0103//
            X0104                   ,//X0104//Polishing Clamp Cylinder FWD
            X0105                   ,//X0105//Polishing Clamp Cylinder BWD
            X0106                   ,//X0106//
            X0107                   ,//X0107//
            X0108                   ,//X0108//
            X0109                   ,//X0109//
            X0110                   ,//X0110//
            X0111                   ,//X0111//
            xCLN_UtilLevelChk       ,//X0112//Cleaning Util Level Limit
            xTRS_MagaPlateExtChk    ,//X0113//Magazine Plate Exist Sensor
            xTRS_MagaPitchChk       ,//X0114//Magazine Pitch Sensor
            xTRS_LoadPlateExist     ,//X0115//Load Port Plate Exist Sensor
            xTRS_TopTRFwd           ,//X0116//Top Load Cylinder FWD
            xTRS_TopTRBwd           ,//X0117//Top Load Cylinder BWD
            xTRS_TopTR0             ,//X0118//Top Load Cylinder 0(to Magazine)
            xTRS_TopTR180           ,//X0119//Top Load Cylinder 180
            xTRS_BtmTRFwd           ,//X0120//Bottom Load Cylinder FWD
            xTRS_BtmTRBwd           ,//X0121//Bottom Load Cylinder BWD
            xTRS_LoadCoverBwd       ,//X0122//Load Port Cover Cylinder BWD
            xTRS_LoadCoverFwd       ,//X0123//Load Port Cover Cylinder BWD
            X0124                   ,//X0124//Load Port Cylinder UP
            X0125                   ,//X0125//Load Port Cylinder DOWN
            xTRS_TransPlateExist    ,//X0126//Transfer Plate Exist Sensor
            X0127                   ,//X0127//
            xSTR_PosCheck           ,//X0128//Storage Position Check (PT)- (B)N.C(Exist -> N.O)
            X0129                   ,//X0129//Tool Storage LOCK left Front Cylinder FWD
            X0130                   ,//X0130//Tool Storage LOCK left Front Cylinder BWD
            X0131                   ,//X0131//Tool Storage LOCK left Rear Cylinder FWD
            X0132                   ,//X0132//Tool Storage LOCK left Rear Cylinder BWD
            X0133                   ,//X0133//Tool Storage LOCK Right Front Cylinder FWD
            X0134                   ,//X0134//Tool Storage LOCK Right Front Cylinder BWD
            X0135                   ,//X0135//Tool Storage LOCK Right Rear Cylinder FWD
            X0136                   ,//X0136//Tool Storage LOCK Right Rear Cylinder BWD
            xSTR_ExitCheck          ,//X0137//Tool Storage Stage exist sensor (L/S) - N.C
            X0138                   ,//X0138//
            xSTR_ExtToolBasket      ,//X0139//Tool Storage Basket Exist Sensor
            X0140                   ,//X0140//
            X0141                   ,//X0141//
            X0142                   ,//X0142//
            X0143                   ,//X0143//
            X0144                   ,//X0144//Vision PROTECT Cylinder FWD
            X0145                   ,//X0145//Vision PROTECT Cylinder BWD
            X0146                   ,//X0146//Vision FILTER Cylinder FWD
            X0147                   ,//X0147//Vision FILTER Cylinder BWD
            X0148                   ,//X0148//Spindle Plate Gripper Cylinder FWD
            X0149                   ,//X0149//Spindle Plate Gripper Cylinder BWD
            xSPD_ToolExist          ,//X0150//Spindle Tool Exist Check
            X0151                   ,//X0151//
            xSPD_PlateExistChk      ,//X0152//Spindle Plate Exist Check
            xPOL_UtilLevelChk1      ,//X0153//Spindle 초음파 Level 1
            xPOL_UtilLevelChk2      ,//X0154//Spindle 초음파 Level 2
            xPOL_CupExistChk        ,//X0155//
            X0156                   ,//X0156//
            X0157                   ,//X0157//
            X0158                   ,//X0158//
            X0159                   ,//X0159//
            X0160                   ,//X0160//Y2-axis (Cleaning Stage) ALARM
            X0161                   ,//X0161//Y3-axis(Tool Storage Stage) ALARM
            X0162                   ,//X0162//A-axis(Polishing Tilting) ALARM
            X0163                   ,//X0163//C1-axis(Polishing Rotation) ALARM
            X0164                   ,//X0164//C2-axis(Cleaning Rotation) ALARM
            X0165                   ,//X0165//
            X0166                   ,//X0166//
            X0167                   ,//X0167//
            X0168                   ,//X0168//
            X0169                   ,//X0169//
            xSPD_E3000_RUN          ,//X0170//SPINDLE_START
            xSPD_E3000_Direction    ,//X0171//SPINDLE_DIRECTION_IN
            xSPD_E3000_State        ,//X0172//SPINDLE_ERR(B)
            xSPD_E3000_Warn         ,//X0173//SPINDLE_WARNING
            xSPD_E3000_SpeedOK      ,//X0174//SPINDLE_COIN
            X0175                   ,//X0175//SPINDLE_SAFE
            X0176                   ,//X0176//
            X0177                   ,//X0177//
            X0178                   ,//X0178//
            X0179                   ,//X0179//
            X0180                   ,//X0180//
            X0181                   ,//X0181//
            X0182                   ,//X0182//
            X0183                   ,//X0183//
            X0184                   ,//X0184//
            X0185                   ,//X0185//
            X0186                   ,//X0186//
            X0187                   ,//X0187//
            X0188                   ,//X0188//
            X0189                   ,//X0189//
            X0190                   ,//X0190//
            X0191                   ,//X0191//

            EndofList

        }

        public enum EN_OUTPUT_ID : int
        {
            yNone               = -1,
            yMC_PowerOn         = 0 ,  //Y0000//MC01,MC02 POWER ON
            ySW_AutoUse             ,  //Y0001//MANUAL -> AUTO CHANGE KEY
            ySW_Reset               ,  //Y0002//SAFETY RESET LAMP
            ySYS_Light              ,  //Y0003//EQ LED Light ON
            Y0004                   ,  //Y0004//SPARE
            yMOTR_Z2_Unlock         ,  //Y0005//Z2-axis(Plate Pick/Place) Motor Unlocking Signal
            yMOTR_TRZ_Unlock        ,  //Y0006//Magazine Z-axis(Plate Pick/Place) Motor Unlocking Signal
            ySYS_Door_FrontRight    ,  //Y0007//EQ_DOOR Door Right
            ySYS_Door_FrontLeft     ,  //Y0008//EQ_DOOR Door Left
            ySYS_Door_Side          ,  //Y0009//EQ_DOOR Side Lock/Unlock
            ySYS_Safety_PLC01       ,  //Y0010//Beckhoff PLC->SAFETY PLC 01 IN
            ySYS_Safety_PLC02       ,  //Y0011//Beckhoff PLC->SAFETY PLC 02 IN
            yLP_Red                 ,  //Y0012//TOWER LAMP RED
            yLP_Yellow              ,  //Y0013//TOWER LAMP YELLOW
            yLP_Green               ,  //Y0014//TOWER LAMP GREEN
            yLP_Buzz01              ,  //Y0015//TOWER LAMP BUZZER
            Y0016                   ,  //Y0016//LINEAR SCALE BLOWER (이물제거용)
            Y0017                   ,  //Y0017//
            Y0018                   ,  //Y0018//Storage LOCK UP
            Y0019                   ,  //Y0019//Storage LOCK DN
            Y0020                   ,  //Y0020//MAGAZINE TRANSFER_RIGHT
            Y0021                   ,  //Y0021//MAGAZINE TRANSFER_LEFT
            Y0022                   ,  //Y0022//TRANSFER Bottom Load_FWD
            Y0023                   ,  //Y0023//TRANSFER Bottom Load_BWD
            Y0024                   ,  //Y0024//WAFER TRANSFER_TURN
            Y0025                   ,  //Y0025//WAFER TRANSFER_RETURN
            Y0026                   ,  //Y0026//TRANSFER Top Load_FWD
            Y0027                   ,  //Y0027//TRANSFER Top Load_BWD
            Y0028                   ,  //Y0028//WAFER POSITION_UP
            Y0029                   ,  //Y0029//WAFER POSITION_DOWN
            Y0030                   ,  //Y0030//WAFER COVER_FWD
            Y0031                   ,  //Y0031//WAFER COVER_BWD
            Y0032                   ,  //Y0032//LIGHT COVER_TURN
            Y0033                   ,  //Y0033//LIGHT COVER_RETURN
            Y0034                   ,  //Y0034//FILTER COVER_TURN
            Y0035                   ,  //Y0035//FILTER COVER_RETURN
            Y0036                   ,  //Y0036//Z-AXIS WAFER_GRIP
            Y0037                   ,  //Y0037//POLISHING WAFER_GRIP
            Y0038                   ,  //Y0038//CLEANING CLAMP
            Y0039                   ,  //Y0039//X-AXIS 리니어 BLOWER
            Y0040                   ,  //Y0040//SPARE_SOL
            Y0041                   ,  //Y0041//
            Y0042                   ,  //Y0042//
            Y0043                   ,  //Y0043//
            Y0044                   ,  //Y0044//
            yCLN_AirBlow            ,  //Y0045//
            Y0046                   ,  //Y0046//
            Y0047                   ,  //Y0047//
            yPLS_Valve_SluryDI1     ,  //Y0048//SLURRY_1 DI WATER
            yPLS_Valve_SluryDI2     ,  //Y0049//SLURRY_2 DI WATER
            yPLS_Valve_SluryDI3     ,  //Y0050//SLURRY_3 DI WATER
            yPLS_Slury_SuckBack1    ,  //Y0051//SLURRY_1 DI SUCK BACK
            yPLS_Slury_SuckBack2    ,  //Y0052//SLURRY_2 DI SUCK BACK
            yPLS_Slury_SuckBack3    ,  //Y0053//SLURRY_3 DI SUCK BACK
            yPLS_LeakDrain          ,  //Y0054//POLISHING LEAK DRAIN
            Y0055                   ,  //Y0055//
            yPLS_Valve_Si1          ,  //Y0056//SLURRY_1 VALVE
            yPLS_Valve_Si2          ,  //Y0057//SLURRY_2 VALVE
            yPLS_Valve_Si3          ,  //Y0058//SLURRY_3 VALVE
            yPLS_Valve_DIWater      ,  //Y0059//POLISHING DI WATER VALVE
            yCLN_Valve_DIWater      ,  //Y0060//WASHING DI WATER VALVE
            yPLS_Valve_Drain        ,  //Y0061//POLISHING DRAIN VALVE
            yCLN_Valve_Drain        ,  //Y0062//WASHING DRAIN VALVE
            yPLS_Valve_Soap         ,  //Y0063//LIQUID SOAP VALVE
            Y0064                   ,  //Y0064//TOOL STORAGE LOCK VAVLE
            yValue_SperatorDrain    ,  //Y0065//SEPARATOR DRAIN VALVE
            yValue_backflow         ,  //Y0066//SPARE VALVE_1
            Y0067                   ,  //Y0067//
            yPLS_Pump_SluryDI1      ,  //Y0068//SLURRY_1 PUMP
            yPLS_Pump_SluryDI2      ,  //Y0069//SLURRY_2 PUMP
            yPLS_Pump_SluryDI3      ,  //Y0070//SLURRY_3 PUMP
            yPLS_Pump_Soap          ,  //Y0071//LIQUID SOAP PUMP
            yPump_DIWater           ,  //Y0072//DI WATER PUMP
            yPump_Drain             ,  //Y0073//DRAIN PUMP
            yValue_SperatorBlower   ,  //Y0074//SEPARATOR BLOWER
            ySPD_E3000_ToolClamp    ,  //Y0075//SPINDLE TOOL CHANGE
            Y0076                   ,  //Y0076//SPARE PUMP_1
            Y0077                   ,  //Y0077//SPARE PUMP_2
            Y0078                   ,  //Y0078//SPARE PUMP_3
            Y0079                   ,  //Y0079//
            ySPD_E3000_Run          ,  //Y0080//SPINDLE_START
            ySPD_E3000_DirIn        ,  //Y0081//SPINDLE_DIRECTION_IN
            ySPD_E3000_Reset        ,  //Y0082//SPINDLE_RESET
            ySPD_E3000_Signal       ,  //Y0083//SPINDLE_SIGNAL(500min-1, rpm)
            ySPD_E3000_SpeedUp      ,  //Y0084//SPINDLE_UPDOWN_SPEED(UD_IN)
            ySPD_E3000_CNTIn        ,  //Y0085//SPINDLE_CENTERING_SPEED(CNT_IN)
            Y0086                   ,  //Y0086//
            Y0087                   ,  //Y0087//
            Y0088                   ,  //Y0088//인터페이스용_1
            Y0089                   ,  //Y0089//인터페이스용_2
            Y0090                   ,  //Y0090//인터페이스용_3
            Y0091                   ,  //Y0091//인터페이스용_4
            Y0092                   ,  //Y0092//인터페이스용_5
            Y0093                   ,  //Y0093//인터페이스용_6
            Y0094                   ,  //Y0094//인터페이스용_7
            Y0095                   ,  //Y0095//인터페이스용_8
            EndofList
	
        }

        //---------------------------------------------------------------------------
        public enum EN_AINPUT_ID : int
        {
            AINone           = -1,
            aiSPD_LoadCell   =  0, //AI0000//LoadCell Top
            aiPOL_UtilLevel      , //AI0001//UltraSonic Sensor Value (Polishing 수위 )
            aiSPD_Pressure       , //AI0002//OP PRESSURE GAUGE
            aiSPD_E3000_Curnt    , //AI0003//SPINDLE_Motor Current Monitor(MOTOR_1 )
            aiSPD_E3000_LOAD     , //AI0004//SPINDLE_Torque Monitor(LOAD )
            aiSPD_E3000_Speed    , //AI0005//SPINDLE_Rotating Speed Analog Monitor(SPEED_V )
            aiPOL_SlurryFlow     , //AI0006//Polishing Slurry Flow Sensor
            aiPOL_DIWaterFlow    , //AI0007//Polishing DI Water Flow Sensor
            aiCLN_DIWaterFlow    , //AI0008//WASHING DI Water Flow Sensor
            AI0009               ,
            AI0010               ,
            AI0011               ,

            EndofList

        }
        public enum EN_AOUTPUT_ID : int
        {
            AONone             = -1,
            aoSDL_E3000_Speed  =  0, //SPINDLE_Motor_Speed Control (MOTOR_VR)
            AO0002                 ,
            AO0003                 ,
            AO0004                 ,

            EndofList

        }

    }
}
