using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.BaseUnit
{
	class ManualId
	{
		//List of Manual
		public enum EN_MAN_LIST
		{
			MAN_NON = 0,
			MAN_0001,
			MAN_0002,
			MAN_0003,
			MAN_0004,
			MAN_0005,
			MAN_0006,
			MAN_0007,
			MAN_0008,
			MAN_0009,
			MAN_0010,
			MAN_0011,
			MAN_0012,
			MAN_0013,
			MAN_0014,
			MAN_0015,
			MAN_0016,
			MAN_0017,
			MAN_0018,
			MAN_0019,
			MAN_0020,
			MAN_0021,
			MAN_0022,
			MAN_0023,
			MAN_0024,

			MAN_0025, //Spindle X Axis - MOTOR SOTP
			MAN_0026, //Spindle X Axis - MOVE JOG
			MAN_0027, //Spindle X Axis - MOVE USER PITCH
			MAN_0028, //Spindle X Axis - SERVO On/Off
			MAN_0029, //Spindle X Axis - MOTOR RESET
			MAN_0030, //Spindle X Axis - DIRECT MOVE
			MAN_0031, //Spindle X Axis - HOME
			MAN_0032, //Spindle X Axis - Wait Pos."                
			MAN_0033, //Spindle X Axis - Polishing Pos."           
			MAN_0034, //Spindle X Axis - Cleaning Pos"             
			MAN_0035, //Spindle X Axis - Tool Pick Pos"            
			MAN_0036, //Spindle X Axis - Tool Dispose Pos."        
			MAN_0037, //Spindle X Axis - Force Check Pos."         
			MAN_0038, //Spindle X Axis - Plate Polishing P/P Pos." 
			MAN_0039, //Spindle X Axis - Plate Cleaning P/P Pos."  
			MAN_0040, //Spindle X Axis - Plate Load1 P/P Pos."     
			MAN_0041, //Spindle X Axis - Plate Load2 P/P Pos."     
			MAN_0042, //Spindle X Axis - Util Supply Check Pos."   
			MAN_0043, //Spindle X Axis - Loading Zone1 Vision Pos."
			MAN_0044, //Spindle X Axis - Loading Zone2 Vision Pos."
			MAN_0045, //Spindle X Axis - Polishing Vision Pos."    
			MAN_0046, //Spindle X Axis - Tool Align1 Pos."         
			MAN_0047, //Spindle X Axis - Tool Align2 Pos."         
			MAN_0048, //Spindle X Axis - 
			MAN_0049, //Spindle X Axis - 

			MAN_0050, //Polishing Y Axis - MOTOR SOTP
			MAN_0051, //Polishing Y Axis - MOVE JOG
			MAN_0052, //Polishing Y Axis - MOVE USER PITCH
			MAN_0053, //Polishing Y Axis - SERVO On/Off
			MAN_0054, //Polishing Y Axis - MOTOR RESET
			MAN_0055, //Polishing Y Axis - DIRECT MOVE
			MAN_0056, //Polishing Y Axis - HOME
			MAN_0057, //Polishing Y Axis - "Plate P/P Pos.(Wait Pos.)"
			MAN_0058, //Polishing Y Axis - "Work Pos."                
			MAN_0059, //Polishing Y Axis - "Vision Inspect Pos."      
			MAN_0060, //Polishing Y Axis - "Cup Storage In/Out Pos."  
			MAN_0061, //Polishing Y Axis - "Polishing Cup In/Out Pos."
			MAN_0062, //Polishing Y Axis - "Utility Check Pos."       
			MAN_0063, //Polishing Y Axis - 
			MAN_0064, //Polishing Y Axis -
			MAN_0065, //Polishing Y Axis -
			MAN_0066, //Polishing Y Axis -
			MAN_0067, //Polishing Y Axis -
			MAN_0068, //Polishing Y Axis -
			MAN_0069, //Polishing Y Axis -
			MAN_0070, //Polishing Y Axis -
			MAN_0071,
			MAN_0072,
			MAN_0073,
			MAN_0074,
			MAN_0075, //Spindle Z Axis - MOTOR SOTP
			MAN_0076, //Spindle Z Axis - MOVE JOG
			MAN_0077, //Spindle Z Axis - MOVE USER PITCH
			MAN_0078, //Spindle Z Axis - SERVO On/Off
			MAN_0079, //Spindle Z Axis - MOTOR RESET
			MAN_0080, //Spindle Z Axis - DIRECT MOVE
			MAN_0081, //Spindle Z Axis - HOME
			MAN_0082, //Spindle Z Axis - "Wait Pos."               
			MAN_0083, //Spindle Z Axis - "Polishing Start Pos."    
			MAN_0084, //Spindle Z Axis - "Cleaning Start Pos."     
			MAN_0085, //Spindle Z Axis - "Polishing Tool Pick Pos."
			MAN_0086, //Spindle Z Axis - "Cleaning Tool Pick Pos." 
			MAN_0087, //Spindle Z Axis - "Tool Dispose Pos."       
			MAN_0088, //Spindle Z Axis - "Force Check Pos."        
			MAN_0089, //Spindle Z Axis -
			MAN_0090, //Spindle Z Axis -
			MAN_0091, //Spindle Z Axis -
			MAN_0092, //Spindle Z Axis -
			MAN_0093, //Spindle Z Axis -
			MAN_0094, //Spindle Z Axis -
			MAN_0095, //Spindle Z Axis -
			MAN_0096,
			MAN_0097,
			MAN_0098,
			MAN_0099,
			MAN_0100, //Cleaning TH Axis - MOTOR SOTP
			MAN_0101, //Cleaning TH Axis - MOVE JOG
			MAN_0102, //Cleaning TH Axis - MOVE USER PITCH
			MAN_0103, //Cleaning TH Axis - SERVO On/Off
			MAN_0104, //Cleaning TH Axis - MOTOR RESET
			MAN_0105, //Cleaning TH Axis - DIRECT MOVE
			MAN_0106, //Cleaning TH Axis - HOME
			MAN_0107, //Cleaning TH Axis - 
			MAN_0108, //Cleaning TH Axis - 
			MAN_0109, //Cleaning TH Axis - 
			MAN_0110, //Cleaning TH Axis - 
			MAN_0111, //Cleaning TH Axis - 
			MAN_0112, //Cleaning TH Axis - 
			MAN_0113, //Cleaning TH Axis -
			MAN_0114, //Cleaning TH Axis -
			MAN_0115, //Cleaning TH Axis -
			MAN_0116, //Cleaning TH Axis -
			MAN_0117, //Cleaning TH Axis -
			MAN_0118, //Cleaning TH Axis -
			MAN_0119, //Cleaning TH Axis -
			MAN_0120, //Cleaning TH Axis -
			MAN_0121,
			MAN_0122,
			MAN_0123,
			MAN_0124,
			MAN_0125, //Polishing TH Axis - MOTOR SOTP
			MAN_0126, //Polishing TH Axis - MOVE JOG
			MAN_0127, //Polishing TH Axis - MOVE USER PITCH
			MAN_0128, //Polishing TH Axis - SERVO On/Off
			MAN_0129, //Polishing TH Axis - MOTOR RESET
			MAN_0130, //Polishing TH Axis - DIRECT MOVE
			MAN_0131, //Polishing TH Axis - HOME
			MAN_0132, //Polishing TH Axis - "Wait Pos."      
			MAN_0133, //Polishing TH Axis - "Cup In/Out Pos."
			MAN_0134, //Polishing TH Axis - 
			MAN_0135, //Polishing TH Axis - 
			MAN_0136, //Polishing TH Axis - 
			MAN_0137, //Polishing TH Axis - 
			MAN_0138, //Polishing TH Axis -
			MAN_0139, //Polishing TH Axis -
			MAN_0140, //Polishing TH Axis -
			MAN_0141, //Polishing TH Axis -
			MAN_0142, //Polishing TH Axis -
			MAN_0143, //Polishing TH Axis -
			MAN_0144, //Polishing TH Axis -
			MAN_0145, //Polishing TH Axis -
			MAN_0146,
			MAN_0147,
			MAN_0148,
			MAN_0149,
			MAN_0150, //Polishing TI Axis - MOTOR SOTP
			MAN_0151, //Polishing TI Axis - MOVE JOG
			MAN_0152, //Polishing TI Axis - MOVE USER PITCH
			MAN_0153, //Polishing TI Axis - SERVO On/Off
			MAN_0154, //Polishing TI Axis - MOTOR RESET
			MAN_0155, //Polishing TI Axis - DIRECT MOVE
			MAN_0156, //Polishing TI Axis - HOME
			MAN_0157, //Polishing TI Axis - "Wait Pos."
			MAN_0158, //Polishing TI Axis - "Work Pos."
			MAN_0159, //Polishing TI Axis - 
			MAN_0160, //Polishing TI Axis - 
			MAN_0161, //Polishing TI Axis - 
			MAN_0162, //Polishing TI Axis - 
			MAN_0163, //Polishing TI Axis -
			MAN_0164, //Polishing TI Axis -
			MAN_0165, //Polishing TI Axis -
			MAN_0166, //Polishing TI Axis -
			MAN_0167, //Polishing TI Axis -
			MAN_0168, //Polishing TI Axis -
			MAN_0169, //Polishing TI Axis -
			MAN_0170, //Polishing TI Axis -
			MAN_0171,
			MAN_0172,
			MAN_0173,
			MAN_0174,
			MAN_0175, //Storage Y Axis - MOTOR SOTP
			MAN_0176, //Storage Y Axis - MOVE JOG
			MAN_0177, //Storage Y Axis - MOVE USER PITCH
			MAN_0178, //Storage Y Axis - SERVO On/Off
			MAN_0179, //Storage Y Axis - MOTOR RESET
			MAN_0180, //Storage Y Axis - DIRECT MOVE
			MAN_0181, //Storage Y Axis - HOME
			MAN_0182, //Storage Y Axis - "Wait Pos."               
			MAN_0183, //Storage Y Axis - "Polishing Tool Pick Pos."
			MAN_0184, //Storage Y Axis - "Cleaning Tool Pick Pos." 
			MAN_0185, //Storage Y Axis - "Tool Discard Pos."       
			MAN_0186, //Storage Y Axis - "Storage Out Pos."        
			MAN_0187, //Storage Y Axis - "Tool Storage Check Pos." 
			MAN_0188, //Storage Y Axis - "Vision Align1 Pos."      
			MAN_0189, //Storage Y Axis - "Vision Align2 Pos."      
			MAN_0190, //Storage Y Axis -
			MAN_0191, //Storage Y Axis -
			MAN_0192, //Storage Y Axis -
			MAN_0193, //Storage Y Axis -
			MAN_0194, //Storage Y Axis -
			MAN_0195, //Storage Y Axis -
			MAN_0196,
			MAN_0197,
			MAN_0198,
			MAN_0199,
			MAN_0200, //Cleaning Y Axis - MOTOR SOTP
			MAN_0201, //Cleaning Y Axis - MOVE JOG
			MAN_0202, //Cleaning Y Axis - MOVE USER PITCH
			MAN_0203, //Cleaning Y Axis - SERVO On/Off
			MAN_0204, //Cleaning Y Axis - MOTOR RESET
			MAN_0205, //Cleaning Y Axis - DIRECT MOVE
			MAN_0206, //Cleaning Y Axis - HOME
			MAN_0207, //Cleaning Y Axis - "Plate P/P Pos.(Wait Pos.)"
			MAN_0208, //Cleaning Y Axis - "Work Pos."                
			MAN_0209, //Cleaning Y Axis - "Transfer turn safety Pos."
			MAN_0210, //Cleaning Y Axis - 
			MAN_0211, //Cleaning Y Axis - 
			MAN_0212, //Cleaning Y Axis - 
			MAN_0213, //Cleaning Y Axis -
			MAN_0214, //Cleaning Y Axis -
			MAN_0215, //Cleaning Y Axis -
			MAN_0216, //Cleaning Y Axis -
			MAN_0217, //Cleaning Y Axis -
			MAN_0218, //Cleaning Y Axis -
			MAN_0219, //Cleaning Y Axis -
			MAN_0220, //Cleaning Y Axis -
			MAN_0221,
			MAN_0222,
			MAN_0223,
			MAN_0224,
			MAN_0225, //Spindle Z1 Axis - MOTOR SOTP
			MAN_0226, //Spindle Z1 Axis - MOVE JOG
			MAN_0227, //Spindle Z1 Axis - MOVE USER PITCH
			MAN_0228, //Spindle Z1 Axis - SERVO On/Off
			MAN_0229, //Spindle Z1 Axis - MOTOR RESET
			MAN_0230, //Spindle Z1 Axis - DIRECT MOVE
			MAN_0231, //Spindle Z1 Axis - HOME
			MAN_0232, //Spindle Z1 Axis - "Wait Pos."               
			MAN_0233, //Spindle Z1 Axis - "Load Zone Pick Pos."     
			MAN_0234, //Spindle Z1 Axis - "Polishing Pick Pos."     
			MAN_0235, //Spindle Z1 Axis - "Cleaning Pick Pos."      
			MAN_0236, //Spindle Z1 Axis - "Load Zone Place Pos."    
			MAN_0237, //Spindle Z1 Axis - "Polishing Place Pos."    
			MAN_0238, //Spindle Z1 Axis - "Cleaning Place Pos."     
			MAN_0239, //Spindle Z1 Axis - "Cup Storage P/P Pos."    
			MAN_0240, //Spindle Z1 Axis - "Polishing Cup Pick Pos." 
			MAN_0241, //Spindle Z1 Axis - "Polishing Cup Place Pos."
			MAN_0242, //Spindle Z1 Axis -
			MAN_0243, //Spindle Z1 Axis -
			MAN_0244, //Spindle Z1 Axis -
			MAN_0245, //Spindle Z1 Axis -
			MAN_0246,
			MAN_0247,
			MAN_0248,
			MAN_0249,
			MAN_0250, //Transfer T Axis - MOTOR SOTP
            MAN_0251, //Transfer T Axis - MOVE JOG
            MAN_0252, //Transfer T Axis - MOVE USER PITCH
            MAN_0253, //Transfer T Axis - SERVO On/Off
            MAN_0254, //Transfer T Axis - MOTOR RESET
            MAN_0255, //Transfer T Axis - DIRECT MOVE
            MAN_0256, //Transfer T Axis - HOME
            MAN_0257, //Transfer T Axis - "Wait Pos.(Load/Unload)"
			MAN_0258, //Transfer T Axis - "Pre-Align Pos."        
            MAN_0259, //Transfer T Axis - "Align Calibration Pos."
            MAN_0260, //Transfer T Axis - 
            MAN_0261, //Transfer T Axis - 
            MAN_0262, //Transfer T Axis - 
            MAN_0263, //Transfer T Axis -
            MAN_0264, //Transfer T Axis -
            MAN_0265, //Transfer T Axis -
            MAN_0266, //Transfer T Axis -
            MAN_0267, //Transfer T Axis -
            MAN_0268, //Transfer T Axis -
            MAN_0269, //Transfer T Axis -
            MAN_0270, //Transfer T Axis -
            MAN_0271,
			MAN_0272,
			MAN_0273,
			MAN_0274,
			MAN_0275, //Transfer Z Axis - MOTOR SOTP 
			MAN_0276, //Transfer Z Axis - MOVE JOG
			MAN_0277, //Transfer Z Axis - MOVE USER PITCH
			MAN_0278, //Transfer Z Axis - SERVO On/Off
			MAN_0279, //Transfer Z Axis - MOTOR RESET
			MAN_0280, //Transfer Z Axis - DIRECT MOVE
			MAN_0281, //Transfer Z Axis - HOME
			MAN_0282, //Transfer Z Axis - "Wait Pos."       
			MAN_0283, //Transfer Z Axis - "Work Start Pos." 
			MAN_0284, //Transfer Z Axis - 
			MAN_0285, //Transfer Z Axis - 
			MAN_0286, //Transfer Z Axis - 
			MAN_0287, //Transfer Z Axis - 
			MAN_0288, //Transfer Z Axis -
			MAN_0289, //Transfer Z Axis -
			MAN_0290, //Transfer Z Axis -
			MAN_0291, //Transfer Z Axis -
			MAN_0292, //Transfer Z Axis -
			MAN_0293, //Transfer Z Axis -
			MAN_0294, //Transfer Z Axis -
			MAN_0295, //Transfer Z Axis -
			MAN_0296,
			MAN_0297,
			MAN_0298,
			MAN_0299,
			MAN_0300, //
			MAN_0301, //
			MAN_0302, //
			MAN_0303, //
			MAN_0304, //
			MAN_0305, //
			MAN_0306, //
			MAN_0307, //
			MAN_0308, //
			MAN_0309, //
			MAN_0310, //
			MAN_0311, //
			MAN_0312, //
			MAN_0313, //
			MAN_0314, //
			MAN_0315, //
			MAN_0316, //
			MAN_0317, //
			MAN_0318, //
			MAN_0319, //
			MAN_0320, //
			MAN_0321,
			MAN_0322,
			MAN_0323,
			MAN_0324,
			MAN_0325, //
			MAN_0326, //
			MAN_0327, //
			MAN_0328, //
			MAN_0329, //
			MAN_0330, //
			MAN_0331, //
			MAN_0332, // 
			MAN_0333, //
			MAN_0334, //
			MAN_0335, //
			MAN_0336, //
			MAN_0337, //
			MAN_0338, //
			MAN_0339, //
			MAN_0340, //
			MAN_0341, //
			MAN_0342, //
			MAN_0343, //
			MAN_0344, //
			MAN_0345, //
			MAN_0346,
			MAN_0347,
			MAN_0348,
			MAN_0349,
			MAN_0350, //
			MAN_0351, //
			MAN_0352, //
			MAN_0353, //
			MAN_0354, //
			MAN_0355, //
			MAN_0356, //
			MAN_0357, //
			MAN_0358, //
			MAN_0359, //
			MAN_0360, //
			MAN_0361, //
			MAN_0362, //
			MAN_0363, //
			MAN_0364, //
			MAN_0365, //
			MAN_0366, //
			MAN_0367, //
			MAN_0368, //
			MAN_0369, //
			MAN_0370, //
			MAN_0371,
			MAN_0372,
			MAN_0373,
			MAN_0374,
			MAN_0375,
			MAN_0376,
			MAN_0377,
			MAN_0378,
			MAN_0379,
			MAN_0380,
			MAN_0381,
			MAN_0382,
			MAN_0383,
			MAN_0384,
			MAN_0385,
			MAN_0386,
			MAN_0387,
			MAN_0388,
			MAN_0389,
			MAN_0390,
			MAN_0391,
			MAN_0392,
			MAN_0393,
			MAN_0394,
			MAN_0395,
			MAN_0396,
			MAN_0397,
			
			MAN_0398, //SPINDEL - X AXIS STEP MOVE +
			MAN_0399, //SPINDEL - X AXIS STEP MOVE -
			MAN_0400, //SPINDEL - Tool pick One Cycle
			MAN_0401, //SPINDEL - Tool Discard One Cycle
			MAN_0402, //SPINDLE - Plate Plate One Cycle : Unload
			MAN_0403, //SPINDLE - Plate Plate One Cycle : Polishing Bath
			MAN_0404, //SPINDLE - Plate Plate One Cycle : Cleaning Bath
			MAN_0405, //SPINDLE - Plate Pick One Cycle : Load
			MAN_0406, //SPINDLE - Plate Pick One Cycle : Polishing Bath
			MAN_0407, //SPINDLE - Plate Pick One Cycle : Cleaning Bath
			MAN_0408, //SPINDLE - Tool Exist Check One Cycle
			MAN_0409, //SPINDLE - [CALIBRATION] Force Check One Cycle
			MAN_0410, //SPINDLE - [POLISHING] Force Check One Cycle
			MAN_0411, //SPINDLE - Utility Check One Cycle
			MAN_0412, //SPINDLE - fn_Vision Test OneCycle : Pre-Align
			MAN_0413, //SPINDLE - fn_Vision Test OneCycle : Polishing
			MAN_0414, //SPINDLE - fn_CupPPOneCycle   
			MAN_0415, //SPINDLE - fn_CupPPOneCycle   
			MAN_0416, //SPINDLE - fn_CupPPOneCycle   
			MAN_0417, //SPINDLE - fn_CupPPOneCycle   
			MAN_0418, //SPINDLE - fn_ForceTestCycle  
			MAN_0419, 
			MAN_0420, //POLISHING - Utility Check Cycle
			MAN_0421, //POLISHING - Drain Cycle
			MAN_0422, //POLISHING - 
			MAN_0423, //POLISHING - 
			MAN_0424, //POLISHING - 
			MAN_0425, //POLISHING - 
			MAN_0426,
			MAN_0427,
			MAN_0428,
			MAN_0429,
			MAN_0430, //CLEAN - Cleaning One Cycle
			MAN_0431, //CLEAN - Drain Cycle
			MAN_0432, //CLEAN - 
			MAN_0433, //CLEAN - 
			MAN_0434, //CLEAN - 
			MAN_0435, //CLEAN - 
			MAN_0436,
			MAN_0437,
			MAN_0438,
			MAN_0439,
			MAN_0440, //STORAGE - Step One Cycle 
			MAN_0441, //STORAGE - Align One Cycle
			MAN_0442, //STORAGE - Y AXIS STEP MOVE +
			MAN_0443, //STORAGE - Y AXIS STEP MOVE -
			MAN_0444, //STORAGE - Unlock Cycle
			MAN_0445, //STORAGE - Lock Cycle
			MAN_0446,
			MAN_0447,
			MAN_0448,
			MAN_0449,
			MAN_0450, //TRANSFER - Load One Cycle
			MAN_0451, //TRANSFER - Unload One Cycle
			MAN_0452, //TRANSFER - Pick One Cycle 
			MAN_0453, //TRANSFER - Place One Cycle
			MAN_0454, //TRANSFER - Z AXIS STEP MOVE +
			MAN_0455, //TRANSFER - Z AXIS STEP MOVE -
			MAN_0456, //TRANSFER - 
			MAN_0457,
			MAN_0458,
			MAN_0459,
			MAN_0460,
			MAN_0461,
			MAN_0462,
			MAN_0463,
			MAN_0464,
			MAN_0465,
			MAN_0466,
			MAN_0467,
			MAN_0468,
			MAN_0469,
			MAN_0470, //SPINDLE - Tool Camp
			MAN_0471, //SPINDLE - Plate Clamp
			MAN_0472, //SPINDLE - IR Shutter
			MAN_0473, //SPINDLE - Lens Cover
			MAN_0474,
			MAN_0475, //Polishing - Plate Clamp
			MAN_0476, //Polishing - Cap Fwd/Bwd
			MAN_0477,
			MAN_0478,
			MAN_0479,
			MAN_0480, //Cleaning - Plate Clamp
			MAN_0481, 
			MAN_0482, 
			MAN_0483, 
			MAN_0484,
			MAN_0485, //Storage - Lock Clamp 
			MAN_0486, //Storage - Panel Clamp 
			MAN_0487, //Storage - Total Lock   
			MAN_0488,
			MAN_0489,
			MAN_0490, //Transfer - Top Load Fwd/Bwd
			MAN_0491, //Transfer - Top Load Turn(0, 180)
			MAN_0492, //Transfer - Bottom Load Fwd/Bwd
			MAN_0493, //Transfer - Load Port Up/Down
			MAN_0494, //Transfer - Magazine Move Left/Right 
			MAN_0495, //Transfer - Load Protect Cover 
			MAN_0496, 
			MAN_0497,
			MAN_0498,
			MAN_0499,
			MAN_0500,

			EndofId
		}
	}
}
