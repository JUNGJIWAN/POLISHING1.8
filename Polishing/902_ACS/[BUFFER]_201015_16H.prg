#/ Controller version = 2.70
#/ Date = 10/15/2020 3:32 PM
#/ User remarks = 
#0
! - Single X Axis  Home for NT controller 
! - Homing Direction : TO LEFT(NEGATIVE) LIMIT
! - User Unit : mm
! - Revision History 
! - 2012.12.22  - Created  
! - Version 2.01

GLOBAL REAL UDCOM
GLOBAL INT AXIS_HOME
REAL Save(32)(5), TimeOut
REAL Search_Vel, Commut_current
INT AXIS

! 1 *************************************************************************
! Set user variable initialize 
AXIS = 0				           ! axis number
TimeOut = 1000                     ! [1sec], TimeOut depends on Homing time: serch vel and travel Time out 
Search_Vel = 20			           ! home search velrocity
Commut_current = XRMS(0) * 0.6  ! Commut Current < XRMS
HomeFlag(0) = -1			       ! home flag reset
DATA_ACS_TO_EQ(0) = 0
!*****************************************************************************
SAFETYGROUP(0)				! safetygroup disable
SAFETYGROUP(1)				! safetygroup disable
! 2 **************************************************************************
! Set parameters for motor flags
DISABLE(0)
WAIT 1000
TILL ^MST(0).#ENABLED

Save(0)(0)=VEL (0)			! operating motion profile backup
Save(0)(1)=ACC (0)
Save(0)(2)=DEC (0)
Save(0)(3)=JERK(0)
Save(0)(4)=KDEC(0)

MFLAGS(0).#DEFCON = 1			! Connect function disable[error mapping or inputshaping]
MFLAGS(0).#OPEN = 0			! close loop enable
!*****************************************************************************
CERRI(0) = 1
CERRA(0) = 1
CERRV(0) = 1
! 3 **************************************************************************
!Set Dynamic brake ON
VELBRK(0)=XVEL(0)
MFLAGS(0).11=0				! Dynamic brake mode disable
!*****************************************************************************

! 4 *************************************************************************
! Set Motion parameters for homing 
VEL (0) = Search_Vel
ACC (0)	= VEL(0)*10
DEC (0)	= ACC(0)
JERK(0) = ACC(0)*3
KDEC(0) = JERK(0)
!*****************************************************************************

! 5***************************************************************************
! Disable the default response of the hardware and software limits
FMASK(0).#ENCNC=1 
FMASK(0).#SRL=0
FMASK(0).#SLL=0
FDEF(0).#SRL=0
FDEF(0).#SLL=0 
FDEF(0).#LL=0
FDEF(0).#RL=0 
FDEF(0).#CPE=1
WAIT 10
!*****************************************************************************

! 6 **************************************************************************
! Commutation for each motor
IF MFLAGS(0).#HALL = 0
	IF MFLAGS(0).#BRUSHOK=0
		MFLAGS(0).9=0				! Commutation reset
		ENABLE (0)
		TILL MST(0).#ENABLED
		WAIT 500
		COMMUT (0), Commut_current		! Encoder commutation command
		TILL MFLAGS(0).9, (TimeOut*10)		
		IF ^MFLAGS(0).9
  			DISP "X-Axis commutation fault"
  			GOTO Time_Out
		END
	DISABLE (0)
	WAIT 500
	DISP" X-Axis commutation comp"
	END
END
!*****************************************************************************
! 7 **************************************************************************
! Start homing - look for left limit switch
DISP" X-Axis Home Start, Move to search limit"
ENABLE (0) 
TILL MST(0).#ENABLED 
JOG (0), -                                           ! Move to left limit  at low speed  
TILL FAULT(0).#LL | IN(0).0 = 1, (TimeOut * 60)       ! TimeOut 60sec 
IF ^FAULT(0).#LL & IN(0).0 = 0
  DISP" X-Axis move to search limits fault "
  GOTO Time_Out
END

HALT (0)
WAIT 100

DISP" X-search limit or Home" !JUNG

!Check Limit Condition
IF FAULT(0).#LL
	JOG (0), +
	TILL ^FAULT(0).#LL & IN(0).0 = 1, (TimeOut * 60)       ! TimeOut 60sec 
	
	IF FAULT(0).#LL | IN(0).0 = 0
  		DISP" Axis move to search home fault "
  		GOTO Time_Out
	END
	
	HALT (0)
	WAIT 100
END

JOG (0), +
TILL ^FAULT(0).#LL & IN(0).0 = 0, (TimeOut * 60)       ! TimeOut 60sec 
IF FAULT(0).#LL & IN(0).0 = 1
  DISP" X-Axis move to search limits fault "
  GOTO Time_Out
END

DISP "X-Search Home End Off" !JUNG

HALT (0)
WAIT 100

TILL ^MST(0).#MOVE
WAIT 500

DISP" X-Axis move to search limit comp, Axis move to search index "

!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
IST(0).#IND=1      ! Prepare for finding indexes 
WAIT 100
IST(0).#IND=0      ! Prepare for finding indexes 
JOG/v (0), Search_Vel/2 ! Move to index  at low speed  
TILL IST(0).#IND,(TimeOut * 40) 
IF ^ IST(0).#IND
  DISP" X-Axis move to search index fault " 
  GOTO Time_Out
END
HALT (0)
TILL ^MST(0).#MOVE; WAIT 500
DISP "X-Axis move to search index comp "
SET FPOS(0) = FPOS(0) - IND(0)
!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
!SET FPOS(AXIS) = FPOS(AXIS) - FPOS(AXIS) !Delete
!*****************************************************************************

! 8 **************************************************************************
! Move to zero position
PTP/E 0, 0
TILL ^MST(0).#MOVE
WAIT 500
!*****************************************************************************

! 9 **************************************************************************
! Move to pin up position
PTP/E 0, Home_Offset(Main_X)
TILL ^MST(0).#MOVE
WAIT 1000
SET FPOS(0) = FPOS(0) - FPOS(0)
!*****************************************************************************

! 10 *************************************************************************
! Set software limits and unmask left/right limit faults
FMASK(0).#RL=1
FMASK(0).#LL=1 
FMASK(0).#SRL=0
FMASK(0).#SLL=0
FDEF(0).#SRL=0
FDEF(0).#SLL=0; 
FDEF(0).#LL=1
FDEF(0).#RL=1      
!*****************************************************************************

!  11 ************************************************************************
! Set home done flags and encoder filter 
HomeFlag(0)=1
DISP "X-Axis Home Done = %d, Axis number=%d" ,HomeFlag(0), 0
!*****************************************************************************

! 12 **************************************************************************
! Restore previous motion parameters
Restore:
VEL(0)  = Save(0)(0)
ACC(0)  = Save(0)(1)
DEC(0)  = Save(0)(2)
JERK(0) = Save(0)(3)
KDEC(0) = Save(0)(4)
!*****************************************************************************
STOP

! 15 *************************************************************************
Time_Out:
DISP "X-Home fault time_out, Axis number=",0
HALT (0)
TILL ^MST(0).#MOVE, TimeOut
DISABLE (0)
GOTO Restore
STOP
!*****************************************************************************

! 16 *************************************************************************
! Set software limits and unmask left/right limit faults
ON ^PST(0).#RUN
FMASK(0).#RL=1
FMASK(0).#LL=1 
FMASK(0).#SRL=0
FMASK(0).#SLL=0
FDEF(0).#SRL=0
FDEF(0).#SLL=0;
FDEF(0).#LL=1
FDEF(0).#RL=1  
FDEF(0).#CPE=1
DISP" X-Axis safety all enable, Axis Nomber = ", 0
RET
!*****************************************************************************
ON PST(0).#RUN = 0 & MST(0).#ENABLED = 0
	HomeFlag(0)=0
RET

#1
! - Single Y Axis  Home for NT controller 
! - Homing Direction : TO LEFT(NEGATIVE) LIMIT
! - User Unit : mm
! - Revision History 
! - 2012.12.22  - Created  
! - Version 2.01

GLOBAL REAL UDCOM
GLOBAL INT AXIS_HOME
REAL Save(32)(5), TimeOut
REAL Search_Vel, Commut_current
INT AXIS

! 1 *************************************************************************
! Set user variable initialize 
AXIS = 1				           ! axis number
TimeOut = 1000                     ! [1sec], TimeOut depends on Homing time: serch vel and travel Time out 
Search_Vel = 10			           ! home search velrocity
Commut_current = XRMS(1) * 0.6  ! Commut Current < XRMS
HomeFlag(1) = -1			       ! home flag reset
DATA_ACS_TO_EQ(1) = 0
!*****************************************************************************
SAFETYGROUP(0)				! safetygroup disable
SAFETYGROUP(1)				! safetygroup disable
! 2 **************************************************************************
! Set parameters for motor flags
DISABLE(1)
WAIT 100
TILL ^MST(1).#ENABLED


Save(AXIS)(0)=VEL(1)			! operating motion profile backup
Save(AXIS)(1)=ACC(1)
Save(AXIS)(2)=DEC(1)
Save(AXIS)(3)=JERK(1)
Save(AXIS)(4)=KDEC(1)

MFLAGS(1).#DEFCON = 1			! Connect function disable[error mapping or inputshaping]
MFLAGS(1).#OPEN = 0			! close loop enable
!*****************************************************************************
CERRI(1) = 1
CERRA(1) = 1
CERRV(1) = 1
! 3 **************************************************************************
!Set Dynamic brake ON
VELBRK(1)=XVEL(1)
MFLAGS(1).11=0				! Dynamic brake mode disable
!*****************************************************************************

! 4 *************************************************************************
! Set Motion parameters for homing 
VEL(1) 	= Search_Vel
ACC(1)	= VEL(1)*10
DEC(1)	= ACC(1)
JERK(1) = ACC(1)*3
KDEC(1) = JERK(1)
!*****************************************************************************

! 5***************************************************************************
! Disable the default response of the hardware and software limits
FMASK(1).#ENCNC=1 
FMASK(1).#SRL=0
FMASK(1).#SLL=0
FDEF(1).#SRL=0
FDEF(1).#SLL=0 
FDEF(1).#LL=0
FDEF(1).#RL=0 
FDEF(1).#CPE=1
WAIT 10
!*****************************************************************************

! 6 **************************************************************************
! Commutation for each motor
IF MFLAGS(1).#HALL = 0
	IF MFLAGS(1).#BRUSHOK=0
		MFLAGS(1).9=0				! Commutation reset
		ENABLE (1)
		TILL MST(1).#ENABLED
		WAIT 500
		COMMUT (1), Commut_current		! Encoder commutation command
		TILL MFLAGS(1).9, (TimeOut*10)		
		IF ^MFLAGS(1).9
  			DISP"Axis commutation fault"
  			GOTO Time_Out
		END
	DISABLE (1)
	WAIT 500
	DISP" Axis commutation comp"
	END
END
!*****************************************************************************
! 7 **************************************************************************
! Start homing - look for left limit switch
DISP" Axis Home Start, Move to search limit"
ENABLE (1) 
TILL MST(1).#ENABLED 
JOG (1), -                                           ! Move to left limit  at low speed  
TILL FAULT(1).#LL | IN(0).1 = 1, (TimeOut * 60)       ! TimeOut 60sec 
IF ^FAULT(1).#LL &  IN(0).1 = 0
  DISP" Axis move to search limits fault "
  GOTO Time_Out
END

DISP" search limit or Home" !JUNG
HALT (1)
WAIT 100

!Check Limit Condition
IF FAULT(1).#LL
	JOG (1), +
	TILL ^FAULT(1).#LL & IN(0).1 = 1, (TimeOut * 60)       ! TimeOut 60sec 
	
	IF FAULT(1).#LL | IN(0).1 = 0
  		DISP" Axis move to search home fault "
  		GOTO Time_Out
	END
	
	HALT (1)
	WAIT 100
END

JOG (1), +
TILL ^FAULT(1).#LL & IN(0).1 = 0, (TimeOut * 60)       ! TimeOut 60sec 
IF FAULT(1).#LL & IN(0).1 = 1
  DISP" Axis move to search limits fault "
  GOTO Time_Out
END

DISP "Search Home End" !JUNG

HALT (1)
WAIT 100


TILL ^MST(1).#MOVE
WAIT 500

DISP" Axis move to search limit comp, Axis move to search index "

!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
IST(1).#IND=1      ! Prepare for finding indexes 
WAIT 100
IST(1).#IND=0      ! Prepare for finding indexes 
JOG/v (1), Search_Vel/2 ! Move to index  at low speed  
TILL IST(1).#IND,(TimeOut * 40) 
IF ^ IST(1).#IND
  DISP" Axis move to search index fault " 
  GOTO Time_Out
END
HALT (1)
TILL ^MST(1).#MOVE; WAIT 500
DISP" Axis move to search index comp "
SET FPOS(1) = FPOS(1) - IND(1)
!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
!SET FPOS(AXIS) = FPOS(AXIS) - FPOS(AXIS) !Delete
!*****************************************************************************

! 8 **************************************************************************
! Move to zero position
PTP/E 1, 0
TILL ^MST(1).#MOVE
WAIT 500
!*****************************************************************************

! 9 **************************************************************************
! Move to pin up position
PTP/E 1, Home_Offset(Polishing_Y)
TILL ^MST(1).#MOVE
WAIT 1000
SET FPOS(1) = FPOS(1) - FPOS(1)
!*****************************************************************************

! 10 *************************************************************************
! Set software limits and unmask left/right limit faults
FMASK(1).#RL=1
FMASK(1).#LL=1 
FMASK(1).#SRL=0
FMASK(1).#SLL=0
FDEF(1).#SRL=0
FDEF(1).#SLL=0; 
FDEF(1).#LL=1
FDEF(1).#RL=1      
!*****************************************************************************

!  11 ************************************************************************
! Set home done flags and encoder filter 
HomeFlag(1)=1
DISP" Axis Home Done = , Axis number=",HomeFlag(1), AXIS
!*****************************************************************************

! 12 **************************************************************************
! Restore previous motion parameters
Restore:
VEL(1)  = Save(1)(0)
ACC(1)  = Save(1)(1)
DEC(1)  = Save(1)(2)
JERK(1) = Save(1)(3)
KDEC(1) = Save(1)(4)
!*****************************************************************************
STOP

! 15 *************************************************************************
Time_Out:
DISP" Home fault time_out, Axis number=",1
HALT (1)
TILL ^MST(1).#MOVE, TimeOut
DISABLE (1)
GOTO Restore
STOP
!*****************************************************************************

! 16 *************************************************************************
! Set software limits and unmask left/right limit faults
ON ^PST(1).#RUN
FMASK(1).#RL=1
FMASK(1).#LL=1; 
FMASK(1).#SRL=0
FMASK(1).#SLL=0
FDEF(1).#SRL=0
FDEF(1).#SLL=0; 
FDEF(1).#LL=1
FDEF(1).#RL=1  
FDEF(1).#CPE=1
DISP" Axis safety all enable, Axis Nomber = ", 1
RET
!*****************************************************************************
ON PST(1).#RUN = 0 & MST(1).#ENABLED = 0
	HomeFlag(1)=0
RET

#2
! - Single Z Axis  Home for NT controller 
! - Homing Direction : TO LEFT(NEGATIVE) LIMIT
! - User Unit : mm
! - Revision History 
! - 2012.12.22  - Created  
! - Version 2.01

GLOBAL REAL UDCOM
GLOBAL INT AXIS_HOME
REAL Save(32)(5), TimeOut
REAL Search_Vel, Commut_current
INT AXIS

! 1 *************************************************************************
! Set user variable initialize 
AXIS = 2				           ! axis number
TimeOut = 1000                     ! [1sec], TimeOut depends on Homing time: serch vel and travel Time out 
Search_Vel = 5			           ! home search velrocity
Commut_current = XRMS(2) * 0.6  ! Commut Current < XRMS
HomeFlag(2) = -1		           ! home flag reset
DATA_ACS_TO_EQ(2) = 0
!*****************************************************************************

! 2 **************************************************************************
! Set parameters for motor flags
DISABLEON AXIS
WAIT 100
DISABLE(2)
WAIT 2000
TILL ^MST(2).#ENABLED & FVEL(2) < 3

SAFETYGROUP(2)				! safetygroup disable
Save(2)(0)=VEL(2)			! operating motion profile backup
Save(2)(1)=ACC(2)
Save(2)(2)=DEC(2)
Save(2)(3)=JERK(2)
Save(2)(4)=KDEC(2)

MFLAGS(2).#DEFCON = 1			! Connect function disable[error mapping or inputshaping]
MFLAGS(2).#OPEN = 0			! close loop enable
!*****************************************************************************
CERRI(2) = 1
CERRA(2) = 1
CERRV(2) = 1
! 3 **************************************************************************
!Set Dynamic brake ON
VELBRK(2)=XVEL(2)
MFLAGS(2).11=0				! Dynamic brake mode disable
!*****************************************************************************

! 4 *************************************************************************
! Set Motion parameters for homing 
VEL(2) 	= Search_Vel
ACC(2)	= VEL(2)*10
DEC(2)	= ACC(2)
JERK(2) = ACC(2)*3
KDEC(2) = JERK(2)
!*****************************************************************************

! 5***************************************************************************
! Disable the default response of the hardware and software limits
FMASK(2).#ENCNC=1 
FMASK(2).#SRL=0
FMASK(2).#SLL=0
FDEF(2).#SRL=0
FDEF(2).#SLL=0 
FDEF(2).#LL=0
FDEF(2).#RL=0 
FDEF(2).#CPE=1
WAIT 10
!*****************************************************************************

! 6 **************************************************************************
! Commutation for each motor
IF MFLAGS(2).#HALL = 0
	!IF MFLAGS(2).#BRUSHOK=0
		MFLAGS(2).9=0				! Commutation reset
		ENABLE (2)
		TILL MST(2).#ENABLED
		WAIT 500
		COMMUT (2), Commut_current, 2000		! Encoder commutation command
		TILL MFLAGS(2).9, (TimeOut*10)		
		IF ^MFLAGS(2).9
  			DISP"Axis commutation fault"
  			GOTO Time_Out
		END
	DISABLE (2)
	WAIT 500
	DISP" Axis commutation comp"
	!END
END
!*****************************************************************************
ENABLEON AXIS
WAIT 100
! 7 **************************************************************************
! Start homing - look for left limit switch
DISP" Axis Home Start, Move to search limit"
ENABLE (2) 
TILL MST(2).#ENABLED 
JOG (2), -                                            ! Move to left limit  at low speed  
TILL FAULT(2).#LL | IN(1).0 = 1, (TimeOut * 60)       ! TimeOut 60sec 
IF ^FAULT(2).#LL & IN(1).0 = 0
  DISP" Axis move to search limits fault "
  GOTO Time_Out
END

HALT (2)
WAIT 100

!Check Limit Condition
IF FAULT(2).#LL
	JOG (2), +
	TILL ^FAULT(2).#LL & IN(1).0 = 1, (TimeOut * 60)       ! TimeOut 60sec 
	
	IF FAULT(2).#LL | IN(1).0 = 0
  		DISP" Axis move to search home fault "
  		GOTO Time_Out
	END
	
	HALT (2)
	WAIT 100

END

DISP" search limit or Home" !JUNG

JOG (2), +
TILL ^FAULT(2).#LL & IN(1).0 = 0, (TimeOut * 60)       ! TimeOut 60sec 
IF FAULT(2).#LL & IN(1).0 = 1
  DISP" Axis move to search limits fault - 2 Axis"
  GOTO Time_Out
END

DISP "Search Home End Off" !JUNG

HALT (2)
WAIT 100

TILL ^MST(2).#MOVE
WAIT 500

DISP" Axis move to search home comp, Axis move to search index "

!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
IST(2).#IND=1      ! Prepare for finding indexes 
WAIT 100
IST(2).#IND=0      ! Prepare for finding indexes 
JOG/v (2), Search_Vel/2 ! Move to index  at low speed  
TILL IST(2).#IND,(TimeOut * 40) 
IF ^ IST(2).#IND
  DISP" Axis move to search index fault " 
  GOTO Time_Out
END
HALT (2)
TILL ^MST(2).#MOVE; WAIT 500
DISP" Axis move to search index comp "
SET FPOS(2) = FPOS(2) - IND(2)
!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
!SET FPOS(2) = FPOS(2) - FPOS(2) !Delete
!*****************************************************************************

! 8 **************************************************************************
! Move to zero position
PTP/E AXIS, 0
TILL ^MST(2).#MOVE
WAIT 500
!*****************************************************************************

! 9 **************************************************************************
! Move to pin up position
PTP/E AXIS, Home_Offset(Main_Z)
TILL ^MST(2).#MOVE
WAIT 500
SET FPOS(2) = FPOS(2) - FPOS(2)
!*****************************************************************************

! 10 *************************************************************************
! Set software limits and unmask left/right limit faults
FMASK(2).#RL=1
FMASK(2).#LL=1 
FMASK(2).#SRL=0
FMASK(2).#SLL=0
FDEF(2).#SRL=0
FDEF(2).#SLL=0 
FDEF(2).#LL=1
FDEF(2).#RL=1      
!*****************************************************************************

!  11 ************************************************************************
! Set home done flags and encoder filter 
HomeFlag(2)=1
DISP" Axis Home Done = , Axis number=",HomeFlag(2), AXIS
!*****************************************************************************

! 12 **************************************************************************
! Restore previous motion parameters
Restore:
VEL(2)=Save(2)(0)
ACC(2)=Save(2)(1)
DEC(2)=Save(2)(2)
JERK(2)=Save(2)(3)
KDEC(2) = Save(2)(4)
!*****************************************************************************
STOP

! 15 *************************************************************************
Time_Out:
DISP" Home fault time_out, Axis number=",AXIS
HALT (2)
TILL ^MST(2).#MOVE, TimeOut
DISABLE (2)
GOTO Restore
STOP
!*****************************************************************************

! 16 *************************************************************************
! Set software limits and unmask left/right limit faults
ON ^PST(2).#RUN
FMASK(2).#RL=1
FMASK(2).#LL=1; 
FMASK(2).#SRL=0
FMASK(2).#SLL=0
FDEF(2).#SRL=0
FDEF(2).#SLL=0; 
FDEF(2).#LL=1
FDEF(2).#RL=1  
FDEF(2).#CPE=1
DISP" Axis safety all enable, Axis Nomber = ", AXIS
RET
!*****************************************************************************

#3
! Homing program, Axis 3 name : Cleaning Theta
!*****************************************************************************
! Description : 3rd party drive homing program
! Last Update : 2016.11.02
!*****************************************************************************
!*****************************************************************************
GLOBAL INT Home_Dir(64)
INT  AXIS, Timeout
INT  ECAT_NODE_NO, E_ADDR_CONTROL_WORD, E_ADDR_STATUS, E_ADDR_ACTUAL_POS
INT  VelMoveToSwitch, VelMoveToZero, AccHoming, HomeOffset, VelMoveNormal

!*****************************************************************************
! User define
!*****************************************************************************
AXIS = 3				! Axis number
ECAT_NODE_NO = 25		! EtherCAT node number

HomeFlag(3) = -1		! Reset homeflag
DATA_ACS_TO_EQ(3) = 0

CERRI(3) = 500
CERRA(3) = 500
CERRV(3) = 500

Timeout = 1000 * 120	! Time-out (unit : msec)
Home_Dir(3) = 0      ! 0: left(-) limit, 1: right(+) limit

VelMoveToSwitch = 15 !5	! Home speed : find to limit
VelMoveToZero =  VelMoveToSwitch / 2		! Home speed : find to index
AccHoming = VelMoveToSwitch * 5			! Home acceration
!*****************************************************************************

DISP "-----------------------------------------------------------------------"
DISP "%d Axis, Preparing homing parameters", AXIS
!*****************************************************************************
! Disable motor
DISABLE (3); TILL ^MST(3).#ENABLED
! Find network address of variables 
E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)

! Release mapping - prepare
ControlWord(ECAT_NODE_NO)    = 0; ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
StatusWord(ECAT_NODE_NO)     = 0; ECUNMAPIN (E_ADDR_STATUS)			! Status word
ActualPosition(ECAT_NODE_NO) = 0; ECUNMAPIN (E_ADDR_ACTUAL_POS)		! Actual Position
WAIT 10
!*****************************************************************************
! PDO MAPPING - EtherCAT IN/OUT Adress
ECOUT(E_ADDR_CONTROL_WORD, ControlWord(ECAT_NODE_NO))	! Control word
ECIN (E_ADDR_STATUS, StatusWord(ECAT_NODE_NO))			! Status word
ECIN (E_ADDR_ACTUAL_POS, ActualPosition(ECAT_NODE_NO))	! Actual Position
!*****************************************************************************
! Fault reset
ControlWord(ECAT_NODE_NO).7 = 1
TILL StatusWord(ECAT_NODE_NO).3 = 0

FMASK(3).#SRL=0
FMASK(3).#SLL=0

DISP "%d Axis, Clear all faults!", AXIS
WAIT 500
!*****************************************************************************
! Homing parameter setting
!*****************************************************************************
! Operation mode change : Homing (0x06)
COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x06)
TILL (0x06 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
DISP "%d Axis, Change mode to homing", AXIS

! Homing method
IF     Home_Dir(3) = 0; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 11) ! 1 => Move to LEFT  limit then index
ELSEIF Home_Dir(3) = 1; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 12) ! 2 => Move to RIGHT limit then index
ELSE   GOTO L_ERROR
END

! Homing Speed & Offset
! Calculate to count for EtherCAT parameters
VelMoveNormal    = VelMoveToSwitch
VelMoveToSwitch  = VelMoveToSwitch / EFAC(3)
VelMoveToZero    = VelMoveToZero   / EFAC(3)
AccHoming        = AccHoming       / EFAC(3)
Home_Offset(Cleaning_T)		 = Home_Offset(Cleaning_T)	   / EFAC(3) * (-1)

COEWRITE/4 (ECAT_NODE_NO, 0x6099, 1, VelMoveToSwitch)	! Speed during a search for switch
COEWRITE/4 (ECAT_NODE_NO, 0x6099, 2, VelMoveToZero)	! Speed during a search for zero (index)
COEWRITE/4 (ECAT_NODE_NO, 0x609A, 0, AccHoming )	! Homing acceleration
COEWRITE/4 (ECAT_NODE_NO, 0x607C, 0, HomeOffset)	! Home offset
COEWRITE/4 (ECAT_NODE_NO, 0x6067, 0, 1000      )        !JUNG/200608

WAIT 100
!*****************************************************************************

!*****************************************************************************
! Start homing procedure
!*****************************************************************************
ControlWord(ECAT_NODE_NO) = 0
WAIT 500

! Ready to enable
ControlWord(ECAT_NODE_NO) = 0x06
TILL StatusWord(ECAT_NODE_NO).0 = 1
ControlWord(ECAT_NODE_NO) = 0x07
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1)
WAIT 100

! Enable motor
ControlWord(ECAT_NODE_NO) = 0x0F
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1) & (StatusWord(ECAT_NODE_NO).2 = 1), 5000	! Wait 1sec until enabled
IF   (StatusWord(ECAT_NODE_NO).0 = 0) | (StatusWord(ECAT_NODE_NO).1 = 0) | (StatusWord(ECAT_NODE_NO).2 = 0)
	DISP "%d Axis, Enable failed!", AXIS
	GOTO L_ERROR
END
DISP "%d Axis, Enabled!", AXIS
WAIT 500

! Before homing, FPOS has to make the same position from drive's actual position
SET FPOS(3) = ActualPosition(ECAT_NODE_NO) * EFAC(3)
! Homing start
DISP "%d Axis, Homing start!", AXIS
ControlWord(ECAT_NODE_NO) = 0x1F
WAIT 2000

! Check homing state : Complete & Move Stopped
TILL ((StatusWord(ECAT_NODE_NO).10 = 1) & (StatusWord(ECAT_NODE_NO).12 = 1)) | (StatusWord(ECAT_NODE_NO).13 = 1), Timeout
IF StatusWord(ECAT_NODE_NO).13 = 1
	DISP "%d Axis, Homing error has been occurred. Program will be stop.", AXIS
	GOTO L_ERROR
END

IF (StatusWord(ECAT_NODE_NO).10 = 0) | (StatusWord(ECAT_NODE_NO).12 = 0)
	DISP"StatusWord.10: %d, StatusWord.12: %d, ", StatusWord(ECAT_NODE_NO).10, StatusWord(ECAT_NODE_NO).12
	DISP "%d Axis, Timeout!", AXIS
	GOTO L_ERROR;
END
WAIT 500

! Homing command off
ControlWord(ECAT_NODE_NO) = 0x0F
!*****************************************************************************
! Restore control mode
CALL L_RESTORE

! Position Setting
ENABLE AXIS
TILL MST(3).#ENABLED
WAIT 200

! Move to zero position
PTP/V AXIS, 0, VelMoveNormal
TILL ^MST(3).#MOVE
WAIT 500

!Home Offset
PTP/E AXIS, Home_Offset(3)
TILL ^MST(3).#MOVE
WAIT 500
SET FPOS(3) = FPOS(3) - FPOS(3)


HomeFlag(3) = 1
DISP "%d Axis, Homing complete!", AXIS

STOP

!*****************************************************************************
! Restore normal operation mode (Release mapping & Return CSP mode)
!*****************************************************************************
L_RESTORE:
	! release mapping
	ControlWord(ECAT_NODE_NO) = 0
	DISABLE (3); TILL ^MST(3).#ENABLED
	! Un-map EtherCAT adress
	ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
	ECUNMAPIN (E_ADDR_STATUS)		! Status word
	ECUNMAPIN (E_ADDR_ACTUAL_POS)	! Actual Position
	WAIT 200
	! Return CSP mode : 0x08
	COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x08)
	TILL (0x08 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
    WAIT 100
	DISP "%d Axis, Return Normal Operation Mode (CSP)", AXIS
RET

!*****************************************************************************
! Error 
!*****************************************************************************
L_ERROR:
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore values
	CALL L_RESTORE
	! Stop move and disable
	HALT 3;    TILL ^MST(3).#MOVE
	DISABLE 3; TILL ^MST(3).#ENABLED 
	HomeFlag(3) = -1
	DATA_ACS_TO_EQ(3) = 1
STOP

!*****************************************************************************
! Auto routine for external stop command
!*****************************************************************************
ON ^PST(3).#RUN & HomeFlag(3) <> 1
	! Axis Number
	AXIS = 3; ECAT_NODE_NO = 25
	! EtherCAT Address 
	E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
	E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
	E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore	
	CALL L_RESTORE
	! Halt and Disable motor
	HALT    3; TILL ^MST(3).#MOVE
	DISABLE 3; TILL ^MST(3).#ENABLED 
	
	HomeFlag(3) = -1
	DISP "%d Axis, Homing stop!", AXIS
RET

ON IN(2).0 = 1; SAFINI(3).#LL = 1; RET 	!4 AXIS Left Limit ON
ON IN(2).0 = 0; SAFINI(3).#LL = 0; RET 	!4 AXIS Left Limit OFF
ON IN(2).1 = 1; SAFINI(3).#RL = 1; RET	!4 AXIS Right Limit ON
ON IN(2).1 = 0; SAFINI(3).#RL = 0; RET	!4 AXIS Right Limit OFF

#4
! Homing program, Axis 4 name : Polishing Theta
!*****************************************************************************
! Description : 3rd party drive homing program
! Last Update : 2016.11.02
!*****************************************************************************
!*****************************************************************************
GLOBAL INT Home_Dir(64)
INT  AXIS, Timeout
INT  ECAT_NODE_NO, E_ADDR_CONTROL_WORD, E_ADDR_STATUS, E_ADDR_ACTUAL_POS
INT  VelMoveToSwitch, VelMoveToZero, AccHoming, HomeOffset, VelMoveNormal

!*****************************************************************************
! User define
!*****************************************************************************
AXIS = 4				! Axis number
ECAT_NODE_NO = 26		! EtherCAT node number

Timeout = 1000 * 120	! Time-out (unit : msec)
Home_Dir(4) = 0      ! 0: left(-) limit, 1: right(+) limit

HomeFlag(4) = -1		! Reset homeflag
DATA_ACS_TO_EQ(4) = 0

VelMoveToSwitch = 1	    ! Home speed : find to limit
VelMoveToZero   = 1	    ! Home speed : find to index
AccHoming = VelMoveToSwitch * 5			! Home acceration
!*****************************************************************************

DISP "-----------------------------------------------------------------------"
DISP "%d Axis, Preparing homing parameters", AXIS
!*****************************************************************************
! Disable motor
DISABLE (4); TILL ^MST(4).#ENABLED
! Find network address of variables 
E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)

! Release mapping - prepare
ControlWord(ECAT_NODE_NO)    = 0; ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
StatusWord(ECAT_NODE_NO)     = 0; ECUNMAPIN (E_ADDR_STATUS)			! Status word
ActualPosition(ECAT_NODE_NO) = 0; ECUNMAPIN (E_ADDR_ACTUAL_POS)		! Actual Position
WAIT 10
!*****************************************************************************
! PDO MAPPING - EtherCAT IN/OUT Adress
ECOUT(E_ADDR_CONTROL_WORD, ControlWord(ECAT_NODE_NO))	! Control word
ECIN (E_ADDR_STATUS, StatusWord(ECAT_NODE_NO))			! Status word
ECIN (E_ADDR_ACTUAL_POS, ActualPosition(ECAT_NODE_NO))	! Actual Position
!*****************************************************************************
! Fault reset
ControlWord(ECAT_NODE_NO).7 = 1
TILL StatusWord(ECAT_NODE_NO).3 = 0
FMASK(4).#SRL=0
FMASK(4).#SLL=0
DISP "%d Axis, Clear all faults!", AXIS
WAIT 500
!*****************************************************************************
! Homing parameter setting
!*****************************************************************************
! Operation mode change : Homing (0x06)
COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x06)
TILL (0x06 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
DISP "%d Axis, Change mode to homing", AXIS

! Homing method
IF     Home_Dir(4) = 0; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 11) ! 1 => Move to LEFT  limit then index
ELSEIF Home_Dir(4) = 1; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 12) ! 2 => Move to RIGHT limit then index
ELSE   GOTO L_ERROR
END

! Homing Speed & Offset
! Calculate to count for EtherCAT parameters
VelMoveNormal                = VelMoveToSwitch
VelMoveToSwitch              = VelMoveToSwitch / EFAC(4)
VelMoveToZero                = VelMoveToZero   / EFAC(4)
AccHoming                    = AccHoming       / EFAC(4)
Home_Offset(Polishing_Theta) = Home_Offset(Polishing_Theta)	/ EFAC(4) * (-1)

COEWRITE/4 (ECAT_NODE_NO, 0x6099, 1, VelMoveToSwitch)	! Speed during a search for switch
COEWRITE/4 (ECAT_NODE_NO, 0x6099, 2, VelMoveToZero)	    ! Speed during a search for zero (index)
COEWRITE/4 (ECAT_NODE_NO, 0x609A, 0, AccHoming )		! Homing acceleration
COEWRITE/4 (ECAT_NODE_NO, 0x607C, 0, HomeOffset)		! Home offset
WAIT 100
!*****************************************************************************

!*****************************************************************************
! Start homing procedure
!*****************************************************************************
ControlWord(ECAT_NODE_NO) = 0
WAIT 500

! Ready to enable
ControlWord(ECAT_NODE_NO) = 0x06
TILL StatusWord(ECAT_NODE_NO).0 = 1
ControlWord(ECAT_NODE_NO) = 0x07
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1)
WAIT 100

! Enable motor
ControlWord(ECAT_NODE_NO) = 0x0F
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1) & (StatusWord(ECAT_NODE_NO).2 = 1), 5000	! Wait 1sec until enabled
IF   (StatusWord(ECAT_NODE_NO).0 = 0) | (StatusWord(ECAT_NODE_NO).1 = 0) | (StatusWord(ECAT_NODE_NO).2 = 0)
	DISP "%d Axis, Enable failed!", AXIS
	GOTO L_ERROR
END
DISP "%d Axis, Enabled!", AXIS
WAIT 500

! Before homing, FPOS has to make the same position from drive's actual position
SET FPOS(4) = ActualPosition(ECAT_NODE_NO) * EFAC(4)
! Homing start
DISP "%d Axis, Homing start!", AXIS
ControlWord(ECAT_NODE_NO) = 0x1F
WAIT 5000

! Check homing state : Complete & Move Stopped
TILL ((StatusWord(ECAT_NODE_NO).10 = 1) & (StatusWord(ECAT_NODE_NO).12 = 1)) | (StatusWord(ECAT_NODE_NO).13 = 1), Timeout
IF StatusWord(ECAT_NODE_NO).13 = 1
	DISP "%d Axis, Homing error has been occurred. Program will be stop.", AXIS
	GOTO L_ERROR
END

IF (StatusWord(ECAT_NODE_NO).10 = 0) | (StatusWord(ECAT_NODE_NO).12 = 0)
	DISP "%d Axis, Timeout!", AXIS
	GOTO L_ERROR;
END
WAIT 500

! Homing command off
ControlWord(ECAT_NODE_NO) = 0x0F
!*****************************************************************************
! Restore control mode
CALL L_RESTORE

! Position Setting
ENABLE AXIS
TILL MST(4).#ENABLED
WAIT 200

! Move to zero position
PTP/V AXIS, 0, VelMoveNormal
TILL ^MST(4).#MOVE
WAIT 500

!Home Offset
PTP/E AXIS, Home_Offset(4)
TILL ^MST(4).#MOVE
WAIT 500
SET FPOS(4) = FPOS(4) - FPOS(4)

HomeFlag(4) = 1
DATA_ACS_TO_EQ(4) = 0
DISP "%d Axis, Homing complete!", AXIS

STOP

!*****************************************************************************
! Restore normal operation mode (Release mapping & Return CSP mode)
!*****************************************************************************
L_RESTORE:
	! release mapping
	ControlWord(ECAT_NODE_NO) = 0
	DISABLE (4); TILL ^MST(4).#ENABLED
	! Un-map EtherCAT adress
	ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
	ECUNMAPIN (E_ADDR_STATUS)		! Status word
	ECUNMAPIN (E_ADDR_ACTUAL_POS)	! Actual Position
	WAIT 200
	! Return CSP mode : 0x08
	COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x08)
	TILL (0x08 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
    WAIT 100
	DISP "%d Axis, Return Normal Operation Mode (CSP)", AXIS
RET

!*****************************************************************************
! Error 
!*****************************************************************************
L_ERROR:
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore values
	CALL L_RESTORE
	! Stop move and disable
	HALT 4;    TILL ^MST(4).#MOVE
	DISABLE 4; TILL ^MST(4).#ENABLED 
	HomeFlag(4) = -1
	DATA_ACS_TO_EQ(4) = 1
STOP

!*****************************************************************************
! Auto routine for external stop command
!*****************************************************************************
ON ^PST(4).#RUN & HomeFlag(4) <> 1
	! Axis Number
	AXIS = 4; ECAT_NODE_NO = 26
	! EtherCAT Address 
	E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
	E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
	E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore	
	CALL L_RESTORE
	! Halt and Disable motor
	HALT    4; TILL ^MST(4).#MOVE
	DISABLE 4; TILL ^MST(4).#ENABLED 
	
	HomeFlag(4) = -1
	DATA_ACS_TO_EQ(4) = 1
	DISP "%d Axis, Homing stop!", AXIS
RET
ON IN(3).0 = 1; SAFINI(4).#LL = 1; RET 	!4 AXIS Left Limit ON
ON IN(3).0 = 0; SAFINI(4).#LL = 0; RET 	!4 AXIS Left Limit OFF
ON IN(3).1 = 1; SAFINI(4).#RL = 1; RET	!4 AXIS Right Limit ON
ON IN(3).1 = 0; SAFINI(4).#RL = 0; RET	!4 AXIS Right Limit OFF

#5
! Homing program, Axis 5 name : Polishing Tilt
!*****************************************************************************
! Description : 3rd party drive homing program
! Last Update : 2016.11.02
!*****************************************************************************
!*****************************************************************************
GLOBAL INT Home_Dir(64)
INT  AXIS, Timeout
INT  ECAT_NODE_NO, E_ADDR_CONTROL_WORD, E_ADDR_STATUS, E_ADDR_ACTUAL_POS
INT  VelMoveToSwitch, VelMoveToZero, AccHoming, HomeOffset, VelMoveNormal

!*****************************************************************************
! User define
!*****************************************************************************
AXIS = 5				! Axis number
ECAT_NODE_NO = 27		! EtherCAT node number

Timeout = 1000 * 120	! Time-out (unit : msec)
Home_Dir(5) = 0      ! 0: left(-) limit, 1: right(+) limit

HomeFlag(5) = -1		! Reset homeflag
DATA_ACS_TO_EQ(5) = 0

VelMoveToSwitch = 1	! Home speed : find to limit
VelMoveToZero =  1		! Home speed : find to index
AccHoming = VelMoveToSwitch * 5			! Home acceration
!*****************************************************************************

DISP "-----------------------------------------------------------------------"
DISP "%d Axis, Preparing homing parameters", AXIS
!*****************************************************************************
! Disable motor
DISABLE (5); TILL ^MST(5).#ENABLED
! Find network address of variables 
E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)

! Release mapping - prepare
ControlWord(ECAT_NODE_NO)    = 0; ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
StatusWord(ECAT_NODE_NO)     = 0; ECUNMAPIN (E_ADDR_STATUS)			! Status word
ActualPosition(ECAT_NODE_NO) = 0; ECUNMAPIN (E_ADDR_ACTUAL_POS)		! Actual Position
WAIT 10
!*****************************************************************************
! PDO MAPPING - EtherCAT IN/OUT Adress
ECOUT(E_ADDR_CONTROL_WORD, ControlWord(ECAT_NODE_NO))	! Control word
ECIN (E_ADDR_STATUS, StatusWord(ECAT_NODE_NO))			! Status word
ECIN (E_ADDR_ACTUAL_POS, ActualPosition(ECAT_NODE_NO))	! Actual Position
!*****************************************************************************
! Fault reset
ControlWord(ECAT_NODE_NO).7 = 1
TILL StatusWord(ECAT_NODE_NO).3 = 0
FMASK(5).#SRL=0
FMASK(5).#SLL=0
DISP "%d Axis, Clear all faults!", AXIS
WAIT 500
!*****************************************************************************
! Homing parameter setting
!*****************************************************************************
! Operation mode change : Homing (0x06)
COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x06)
TILL (0x06 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
DISP "%d Axis, Change mode to homing", AXIS

! Homing method
IF     Home_Dir(5) = 0; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 11) ! 1 => Move to LEFT  limit then index
ELSEIF Home_Dir(5) = 1; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 12) ! 2 => Move to RIGHT limit then index
ELSE   GOTO L_ERROR
END

! Homing Speed & Offset
! Calculate to count for EtherCAT parameters
VelMoveNormal    = VelMoveToSwitch
VelMoveToSwitch  = VelMoveToSwitch / EFAC(5)
VelMoveToZero    = VelMoveToZero   / EFAC(5)
AccHoming        = AccHoming       / EFAC(5)
Home_Offset(Polishing_Tilt)		 = Home_Offset(Polishing_Tilt)	   / EFAC(5) * (-1)

COEWRITE/4 (ECAT_NODE_NO, 0x6099, 1, VelMoveToSwitch)	! Speed during a search for switch
COEWRITE/4 (ECAT_NODE_NO, 0x6099, 2, VelMoveToZero)	    ! Speed during a search for zero (index)
COEWRITE/4 (ECAT_NODE_NO, 0x609A, 0, AccHoming )		! Homing acceleration
COEWRITE/4 (ECAT_NODE_NO, 0x607C, 0, HomeOffset)		! Home offset
WAIT 100
!*****************************************************************************

!*****************************************************************************
! Start homing procedure
!*****************************************************************************
ControlWord(ECAT_NODE_NO) = 0
WAIT 500

! Ready to enable
ControlWord(ECAT_NODE_NO) = 0x06
TILL StatusWord(ECAT_NODE_NO).0 = 1
ControlWord(ECAT_NODE_NO) = 0x07
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1)
WAIT 100

! Enable motor
ControlWord(ECAT_NODE_NO) = 0x0F
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1) & (StatusWord(ECAT_NODE_NO).2 = 1), 10000	! Wait 1sec until enabled
IF   (StatusWord(ECAT_NODE_NO).0 = 0) | (StatusWord(ECAT_NODE_NO).1 = 0) | (StatusWord(ECAT_NODE_NO).2 = 0)
	DISP "%d Axis, Enable failed!", AXIS
	GOTO L_ERROR
END
DISP "%d Axis, Enabled!", AXIS
WAIT 500

! Before homing, FPOS has to make the same position from drive's actual position
SET FPOS(5) = ActualPosition(ECAT_NODE_NO) * EFAC(5)
! Homing start
DISP "%d Axis, Homing start!", AXIS
ControlWord(ECAT_NODE_NO) = 0x1F
WAIT 2000

! Check homing state : Complete & Move Stopped
TILL ((StatusWord(ECAT_NODE_NO).10 = 1) & (StatusWord(ECAT_NODE_NO).12 = 1)) | (StatusWord(ECAT_NODE_NO).13 = 1), Timeout
IF StatusWord(ECAT_NODE_NO).13 = 1
	DISP "%d Axis, Homing error has been occurred. Program will be stop.", AXIS
	GOTO L_ERROR
END

IF (StatusWord(ECAT_NODE_NO).10 = 0) | (StatusWord(ECAT_NODE_NO).12 = 0)
	DISP "%d Axis, Timeout!", AXIS
	GOTO L_ERROR;
END
WAIT 500

! Homing command off
ControlWord(ECAT_NODE_NO) = 0x0F
!*****************************************************************************
! Restore control mode
CALL L_RESTORE

! Position Setting
ENABLE 5
TILL MST(5).#ENABLED
WAIT 200

! Move to zero position
PTP/V AXIS, 0, VelMoveNormal
TILL ^MST(5).#MOVE
WAIT 500

!Home Offset
PTP/E 5, Home_Offset(5)
TILL ^MST(5).#MOVE
WAIT 500
SET FPOS(5) = FPOS(5) - FPOS(5)

HomeFlag(5) = 1
DISP "%d Axis, Homing complete!", AXIS

STOP

!*****************************************************************************
! Restore normal operation mode (Release mapping & Return CSP mode)
!*****************************************************************************
L_RESTORE:
	! release mapping
	ControlWord(ECAT_NODE_NO) = 0
	DISABLE (5); TILL ^MST(5).#ENABLED
	! Un-map EtherCAT adress
	ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
	ECUNMAPIN (E_ADDR_STATUS)		! Status word
	ECUNMAPIN (E_ADDR_ACTUAL_POS)	! Actual Position
	WAIT 200
	! Return CSP mode : 0x08
	COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x08)
	TILL (0x08 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
    WAIT 100
	DISP "%d Axis, Return Normal Operation Mode (CSP)", AXIS
RET

!*****************************************************************************
! Error 
!*****************************************************************************
L_ERROR:
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore values
	CALL L_RESTORE
	! Stop move and disable
	HALT 5;    TILL ^MST(5).#MOVE
	DISABLE 5; TILL ^MST(5).#ENABLED 
	HomeFlag(5) = -1
	DATA_ACS_TO_EQ(5) = 1
STOP

!*****************************************************************************
! Auto routine for external stop command
!*****************************************************************************
ON ^PST(5).#RUN & HomeFlag(5) <> 1
	! Axis Number
	AXIS = 5; ECAT_NODE_NO = 27
	! EtherCAT Address 
	E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
	E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
	E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore	
	CALL L_RESTORE
	! Halt and Disable motor
	HALT    5; TILL ^MST(5).#MOVE
	DISABLE 5; TILL ^MST(5).#ENABLED 
	
	HomeFlag(5) = -1
	DISP "%d Axis, Homing stop!", AXIS
RET
ON IN(4).0 = 1; SAFINI(5).#LL = 1; RET 	!4 AXIS Left Limit ON
ON IN(4).0 = 0; SAFINI(5).#LL = 0; RET 	!4 AXIS Left Limit OFF
ON IN(4).1 = 1; SAFINI(5).#RL = 1; RET	!4 AXIS Right Limit ON
ON IN(4).1 = 0; SAFINI(5).#RL = 0; RET	!4 AXIS Right Limit OFF

#6
! Homing program, Axis 6 name : Storage Y
!*****************************************************************************
! Description : 3rd party drive homing program
! Last Update : 2016.11.02
!*****************************************************************************
!*****************************************************************************
GLOBAL INT Home_Dir(64)
INT  AXIS, Timeout
INT  ECAT_NODE_NO, E_ADDR_CONTROL_WORD, E_ADDR_STATUS, E_ADDR_ACTUAL_POS
INT  VelMoveToSwitch, VelMoveToZero, AccHoming, HomeOffset, VelMoveNormal

!*****************************************************************************
! User define
!*****************************************************************************
AXIS = 6				! Axis number
ECAT_NODE_NO = 28		! EtherCAT node number

Timeout = 1000 * 120	! Time-out (unit : msec)
Home_Dir(6) = 0      ! 0: left(-) limit, 1: right(+) limit

HomeFlag(6) = -1		! Reset homeflag
DATA_ACS_TO_EQ(6) = 0

VelMoveToSwitch = 15	!5! Home speed : find to limit
VelMoveToZero =  VelMoveToSwitch / 2		! Home speed : find to index
AccHoming = VelMoveToSwitch * 5			! Home acceration
!*****************************************************************************

DISP "-----------------------------------------------------------------------"
DISP "%d Axis, Preparing homing parameters", AXIS
!*****************************************************************************
! Disable motor
DISABLE (6); TILL ^MST(6).#ENABLED
! Find network address of variables 
E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)

! Release mapping - prepare
ControlWord(ECAT_NODE_NO)    = 0; ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
StatusWord(ECAT_NODE_NO)     = 0; ECUNMAPIN (E_ADDR_STATUS)			! Status word
ActualPosition(ECAT_NODE_NO) = 0; ECUNMAPIN (E_ADDR_ACTUAL_POS)		! Actual Position
WAIT 10
!*****************************************************************************
! PDO MAPPING - EtherCAT IN/OUT Adress
ECOUT(E_ADDR_CONTROL_WORD, ControlWord(ECAT_NODE_NO))	! Control word
ECIN (E_ADDR_STATUS, StatusWord(ECAT_NODE_NO))			! Status word
ECIN (E_ADDR_ACTUAL_POS, ActualPosition(ECAT_NODE_NO))	! Actual Position
!*****************************************************************************
! Fault reset
ControlWord(ECAT_NODE_NO).7 = 1
TILL StatusWord(ECAT_NODE_NO).3 = 0
DISP "%d Axis, Clear all faults!", AXIS

FMASK(6).#SRL=0
FMASK(6).#SLL=0
WAIT 500
!*****************************************************************************
! Homing parameter setting
!*****************************************************************************
! Operation mode change : Homing (0x06)
COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x06)
TILL (0x06 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
DISP "%d Axis, Change mode to homing", AXIS

! Homing method
IF     Home_Dir(6) = 0; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 11) ! 1 => Move to LEFT  limit then index
ELSEIF Home_Dir(6) = 1; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 12) ! 2 => Move to RIGHT limit then index
ELSE   GOTO L_ERROR
END

! Homing Speed & Offset
! Calculate to count for EtherCAT parameters
VelMoveNormal           = VelMoveToSwitch
VelMoveToSwitch         = VelMoveToSwitch / EFAC(6)
VelMoveToZero           = VelMoveToZero   / EFAC(6)
AccHoming               = AccHoming       / EFAC(6)
Home_Offset(Storage_Y)	= Home_Offset(Storage_Y)	   / EFAC(6) * (-1)

COEWRITE/4 (ECAT_NODE_NO, 0x6099, 1, VelMoveToSwitch)	! Speed during a search for switch
COEWRITE/4 (ECAT_NODE_NO, 0x6099, 2, VelMoveToZero)	    ! Speed during a search for zero (index)
COEWRITE/4 (ECAT_NODE_NO, 0x609A, 0, AccHoming )		! Homing acceleration
COEWRITE/4 (ECAT_NODE_NO, 0x607C, 0, HomeOffset)		! Home offset
WAIT 100
!*****************************************************************************

!*****************************************************************************
! Start homing procedure
!*****************************************************************************
ControlWord(ECAT_NODE_NO) = 0
WAIT 500

! Ready to enable
ControlWord(ECAT_NODE_NO) = 0x06
TILL StatusWord(ECAT_NODE_NO).0 = 1
ControlWord(ECAT_NODE_NO) = 0x07
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1)
WAIT 100

! Enable motor
ControlWord(ECAT_NODE_NO) = 0x0F
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1) & (StatusWord(ECAT_NODE_NO).2 = 1), 5000	! Wait 1sec until enabled
IF   (StatusWord(ECAT_NODE_NO).0 = 0) | (StatusWord(ECAT_NODE_NO).1 = 0) | (StatusWord(ECAT_NODE_NO).2 = 0)
	DISP "%d Axis, Enable failed!", AXIS
	GOTO L_ERROR
END
DISP "%d Axis, Enabled!", AXIS
WAIT 500

! Before homing, FPOS has to make the same position from drive's actual position
SET FPOS(6) = ActualPosition(ECAT_NODE_NO) * EFAC(6)
! Homing start
DISP "%d Axis, Homing start!", AXIS
ControlWord(ECAT_NODE_NO) = 0x1F
WAIT 2000

! Check homing state : Complete & Move Stopped
TILL ((StatusWord(ECAT_NODE_NO).10 = 1) & (StatusWord(ECAT_NODE_NO).12 = 1)) | (StatusWord(ECAT_NODE_NO).13 = 1), Timeout
IF StatusWord(ECAT_NODE_NO).13 = 1
	DISP "%d Axis, Homing error has been occurred. Program will be stop.", AXIS
	GOTO L_ERROR
END

IF (StatusWord(ECAT_NODE_NO).10 = 0) | (StatusWord(ECAT_NODE_NO).12 = 0)
	DISP "%d Axis, Timeout!", AXIS
	GOTO L_ERROR;
END
WAIT 500

! Homing command off
ControlWord(ECAT_NODE_NO) = 0x0F
!*****************************************************************************
! Restore control mode
CALL L_RESTORE

! Position Setting
ENABLE AXIS
TILL MST(6).#ENABLED
WAIT 200

! Move to zero position
PTP/V 6, 0, VelMoveNormal
TILL ^MST(6).#MOVE
WAIT 500

!Home Offset
PTP/E 6, Home_Offset(6)
TILL ^MST(6).#MOVE
WAIT 500
SET FPOS(6) = FPOS(6) - FPOS(6)

HomeFlag(6) = 1
DISP "%d Axis, Homing complete!", AXIS

STOP

!*****************************************************************************
! Restore normal operation mode (Release mapping & Return CSP mode)
!*****************************************************************************
L_RESTORE:
	! release mapping
	ControlWord(ECAT_NODE_NO) = 0
	DISABLE (6); TILL ^MST(6).#ENABLED
	! Un-map EtherCAT adress
	ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
	ECUNMAPIN (E_ADDR_STATUS)		! Status word
	ECUNMAPIN (E_ADDR_ACTUAL_POS)	! Actual Position
	WAIT 200
	! Return CSP mode : 0x08
	COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x08)
	TILL (0x08 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
    WAIT 100
	DISP "%d Axis, Return Normal Operation Mode (CSP)", AXIS
RET

!*****************************************************************************
! Error 
!*****************************************************************************
L_ERROR:
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore values
	CALL L_RESTORE
	! Stop move and disable
	HALT 6;    TILL ^MST(6).#MOVE
	DISABLE 6; TILL ^MST(6).#ENABLED 
	HomeFlag(6) = -1
	DATA_ACS_TO_EQ(6) = 1
STOP

!*****************************************************************************
! Auto routine for external stop command
!*****************************************************************************
ON ^PST(6).#RUN & HomeFlag(6) <> 1
	! Axis Number
	AXIS = 6; ECAT_NODE_NO = 28
	! EtherCAT Address 
	E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
	E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
	E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore	
	CALL L_RESTORE
	! Halt and Disable motor
	HALT    6; TILL ^MST(6).#MOVE
	DISABLE 6; TILL ^MST(6).#ENABLED 
	
	HomeFlag(6) = -1
	DISP "%d Axis, Homing stop!", AXIS
RET
ON IN(5).0 = 1; SAFINI(6).#LL = 1; RET 	!4 AXIS Left Limit ON
ON IN(5).0 = 0; SAFINI(6).#LL = 0; RET 	!4 AXIS Left Limit OFF
ON IN(5).1 = 1; SAFINI(6).#RL = 1; RET	!4 AXIS Right Limit ON
ON IN(5).1 = 0; SAFINI(6).#RL = 0; RET	!4 AXIS Right Limit OFF

#7
! Homing program, Axis 7 name : Cleaning Y
!*****************************************************************************
! Description : 3rd party drive homing program
! Last Update : 2016.11.02
!*****************************************************************************
!*****************************************************************************
GLOBAL INT Home_Dir(64)
INT  AXIS, Timeout
INT  ECAT_NODE_NO, E_ADDR_CONTROL_WORD, E_ADDR_STATUS, E_ADDR_ACTUAL_POS
INT  VelMoveToSwitch, VelMoveToZero, AccHoming, HomeOffset, VelMoveNormal

!*****************************************************************************
! User define
!*****************************************************************************
AXIS = 7				! Axis number
ECAT_NODE_NO = 29		! EtherCAT node number

Timeout = 1000 * 120	! Time-out (unit : msec)
Home_Dir(7) = 0      ! 0: left(-) limit, 1: right(+) limit

HomeFlag(7) = -1		! Reset homeflag
DATA_ACS_TO_EQ(7) = 0

VelMoveToSwitch = 5	! Home speed : find to limit
VelMoveToZero =  VelMoveToSwitch / 2		! Home speed : find to index
AccHoming = VelMoveToSwitch * 2			! Home acceration
!*****************************************************************************

DISP "-----------------------------------------------------------------------"
DISP "%d Axis, Preparing homing parameters", AXIS
!*****************************************************************************
! Disable motor
DISABLE (7); TILL ^MST(7).#ENABLED
! Find network address of variables 
E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)

! Release mapping - prepare
ControlWord(ECAT_NODE_NO)    = 0; ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
StatusWord(ECAT_NODE_NO)     = 0; ECUNMAPIN (E_ADDR_STATUS)			! Status word
ActualPosition(ECAT_NODE_NO) = 0; ECUNMAPIN (E_ADDR_ACTUAL_POS)		! Actual Position
WAIT 10
!*****************************************************************************
! PDO MAPPING - EtherCAT IN/OUT Adress
ECOUT(E_ADDR_CONTROL_WORD, ControlWord(ECAT_NODE_NO))	! Control word
ECIN (E_ADDR_STATUS, StatusWord(ECAT_NODE_NO))			! Status word
ECIN (E_ADDR_ACTUAL_POS, ActualPosition(ECAT_NODE_NO))	! Actual Position
!*****************************************************************************
! Fault reset
ControlWord(ECAT_NODE_NO).7 = 1
TILL StatusWord(ECAT_NODE_NO).3 = 0
FMASK(7).#SRL=0
FMASK(7).#SLL=0
DISP "%d Axis, Clear all faults!", AXIS
WAIT 500
!*****************************************************************************
! Homing parameter setting
!*****************************************************************************
! Operation mode change : Homing (0x06)
COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x06)
TILL (0x06 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
DISP "%d Axis, Change mode to homing", AXIS

! Homing method
IF     Home_Dir(7) = 0; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 11) ! 1 => Move to LEFT  limit then index
ELSEIF Home_Dir(7) = 1; COEWRITE/1 (ECAT_NODE_NO, 0x6098, 0, 12) ! 2 => Move to RIGHT limit then index
ELSE   GOTO L_ERROR
END

! Homing Speed & Offset
! Calculate to count for EtherCAT parameters
VelMoveNormal           = VelMoveToSwitch
VelMoveToSwitch         = VelMoveToSwitch / EFAC(7)
VelMoveToZero           = VelMoveToZero   / EFAC(7)
AccHoming               = AccHoming       / EFAC(7)
Home_Offset(Cleaning_Y)	= Home_Offset(Cleaning_Y) / EFAC(7) * (-1)

COEWRITE/4 (ECAT_NODE_NO, 0x6099, 1, VelMoveToSwitch)	! Speed during a search for switch
COEWRITE/4 (ECAT_NODE_NO, 0x6099, 2, VelMoveToZero)	    ! Speed during a search for zero (index)
COEWRITE/4 (ECAT_NODE_NO, 0x609A, 0, AccHoming )		! Homing acceleration
COEWRITE/4 (ECAT_NODE_NO, 0x607C, 0, HomeOffset)		! Home offset
WAIT 100
!*****************************************************************************

!*****************************************************************************
! Start homing procedure
!*****************************************************************************
ControlWord(ECAT_NODE_NO) = 0
WAIT 500

! Ready to enable
ControlWord(ECAT_NODE_NO) = 0x06
TILL StatusWord(ECAT_NODE_NO).0 = 1
ControlWord(ECAT_NODE_NO) = 0x07
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1)
WAIT 100

! Enable motor
ControlWord(ECAT_NODE_NO) = 0x0F
TILL (StatusWord(ECAT_NODE_NO).0 = 1) & (StatusWord(ECAT_NODE_NO).1 = 1) & (StatusWord(ECAT_NODE_NO).2 = 1), 5000	! Wait 1sec until enabled
IF   (StatusWord(ECAT_NODE_NO).0 = 0) | (StatusWord(ECAT_NODE_NO).1 = 0) | (StatusWord(ECAT_NODE_NO).2 = 0)
	DISP "%d Axis, Enable failed!", AXIS
	GOTO L_ERROR
END
DISP "%d Axis, Enabled!", AXIS
WAIT 500

! Before homing, FPOS has to make the same position from drive's actual position
SET FPOS(7) = ActualPosition(ECAT_NODE_NO) * EFAC(7)
! Homing start
DISP "%d Axis, Homing start!", AXIS
ControlWord(ECAT_NODE_NO) = 0x1F
WAIT 2000

! Check homing state : Complete & Move Stopped
TILL ((StatusWord(ECAT_NODE_NO).10 = 1) & (StatusWord(ECAT_NODE_NO).12 = 1)) | (StatusWord(ECAT_NODE_NO).13 = 1), Timeout
IF StatusWord(ECAT_NODE_NO).13 = 1
	DISP "%d Axis, Homing error has been occurred. Program will be stop.", AXIS
	GOTO L_ERROR
END

! Homing command off
ControlWord(ECAT_NODE_NO) = 0x0F
!*****************************************************************************
! Restore control mode
CALL L_RESTORE

! Position Setting
ENABLE 7
TILL MST(7).#ENABLED
WAIT 200

! Move to zero position
PTP/V 7, 0, VelMoveNormal
TILL ^MST(7).#MOVE
WAIT 500

!Home Offset
PTP/E 7, Home_Offset(7)
TILL ^MST(7).#MOVE
WAIT 500
SET FPOS(7) = FPOS(7) - FPOS(7)

HomeFlag(7) = 1
DISP "%d Axis, Homing complete!", AXIS

STOP

!*****************************************************************************
! Restore normal operation mode (Release mapping & Return CSP mode)
!*****************************************************************************
L_RESTORE:
	! release mapping
	ControlWord(ECAT_NODE_NO) = 0
	DISABLE (7); TILL ^MST(7).#ENABLED
	! Un-map EtherCAT adress
	ECUNMAPOUT(E_ADDR_CONTROL_WORD)	! Control word
	ECUNMAPIN (E_ADDR_STATUS)		! Status word
	ECUNMAPIN (E_ADDR_ACTUAL_POS)	! Actual Position
	WAIT 200
	! Return CSP mode : 0x08
	COEWRITE/1 (ECAT_NODE_NO, 0x6060, 0, 0x08)
	TILL (0x08 = (COEREAD/1 (ECAT_NODE_NO, 0x6061, 0)))
    WAIT 100
	DISP "%d Axis, Return Normal Operation Mode (CSP)", AXIS
RET

!*****************************************************************************
! Error 
!*****************************************************************************
L_ERROR:
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore values
	CALL L_RESTORE
	! Stop move and disable
	HALT 7;    TILL ^MST(7).#MOVE
	DISABLE 7; TILL ^MST(7).#ENABLED 
	HomeFlag(7) = -1
	DATA_ACS_TO_EQ(7) = 1
STOP

!*****************************************************************************
! Auto routine for external stop command
!*****************************************************************************
ON ^PST(7).#RUN & HomeFlag(7) <> 1
	! Axis Number
	AXIS = 7; ECAT_NODE_NO = 29
	! EtherCAT Address 
	E_ADDR_CONTROL_WORD = ECGETOFFSET("Control word", ECAT_NODE_NO)
	E_ADDR_STATUS  		= ECGETOFFSET("Status word",  ECAT_NODE_NO)
	E_ADDR_ACTUAL_POS	= ECGETOFFSET("Position actual value", ECAT_NODE_NO)
	! Quick Stop
	ControlWord(ECAT_NODE_NO) = 0x0B		
	TILL StatusWord(ECAT_NODE_NO).5 = 1, 500
	! Restore	
	CALL L_RESTORE
	! Halt and Disable motor
	HALT    7; TILL ^MST(7).#MOVE
	DISABLE 7; TILL ^MST(7).#ENABLED 
	
	HomeFlag(7) = -1
	DISP "%d Axis, Homing stop!", AXIS
RET
ON IN(6).0 = 1; SAFINI(7).#LL = 1; RET 	!4 AXIS Left Limit ON
ON IN(6).0 = 0; SAFINI(7).#LL = 0; RET 	!4 AXIS Left Limit OFF
ON IN(6).1 = 1; SAFINI(7).#RL = 1; RET	!4 AXIS Right Limit ON
ON IN(6).1 = 0; SAFINI(7).#RL = 0; RET	!4 AXIS Right Limit OFF

#8
! Motion profile generator (Setting Profile Value)

INT AXIS
REAL JERK_RATIO, JERK_TIME, ADEC_TIME, GET_VEL
WAIT 100
BLOCK
	AXIS = 0
	
	GET_VEL = VEL(0)
	
	JERK_RATIO = (ACC(AXIS) / JERK(AXIS)) * 1000 / 2
	
	ADEC_TIME = ABS(GET_VEL) / ACC(AXIS) * 1000
	
	JERK_TIME	= ACC(AXIS) / JERK(AXIS) * 1000

	DISP"Total Acc Time: %.2f msec(Tj: %.2f , Ta: %.2f )", (ADEC_TIME + JERK_TIME), ADEC_TIME, JERK_TIME
END

STOP

#10
! Motion profile generator (Given acc_time, jerk_ratio)

!INT AXIS
!GLOBAL REAL ACC_TIME(64), VELOCITY(64), JERK_RATIO(64)
!GLOBAL REAL JERK_TIME(64), ADEC_TIME(64), T_ACC(64), T_JERK(64)
!REAL GET_VEL, GET_ACC, GET_DEC, GET_JERK
!WAIT 100
!BLOCK
!	AXIS = 0
!	VELOCITY(AXIS) = 20	! User unit/sec
!	ACC_TIME(AXIS) = 50	! msec
!	JERK_RATIO(AXIS) = 100	! percentage(%)
!
!	JERK_TIME(AXIS)	= ACC_TIME(AXIS) / 1000 / (1/JERK_RATIO(AXIS)*100*2)
!	ADEC_TIME(AXIS)	= ACC_TIME(AXIS) / 1000 - JERK_TIME(AXIS)
!	T_ACC(AXIS)		= ABS(VELOCITY(AXIS)) / ADEC_TIME(AXIS)
!	T_JERK(AXIS)	= T_ACC(AXIS) / JERK_TIME(AXIS)
!	GET_VEL			= ABS(VELOCITY(AXIS))
!	GET_ACC			= T_ACC(AXIS)
!	GET_DEC			= GET_ACC
!	GET_JERK		= T_JERK(AXIS)
!
!	DISP"Vel.:%5f , Acc.:%5f , Dec: %5f, Jerk:%5f ",GET_VEL, GET_ACC, GET_DEC, GET_JERK
!END
!
!STOP

GLOBAL INT XSEG_Output(2)
STOP

ON XSEG_Output(0) <> 0
	XSEG_Output(0) = 0;
	XSEG_CNT = XSEG_CNT + 1;
	!DISP "XSEG Cnt: %d / X Pos :%f / Y Pos : %f", XSEG_CNT, FPOS(0), FPOS(1);
RET

#11
!TEST
!Z axis Force Control Buffer
global real CurrentRatio
global real DynamicCurrentRatio

GLOBAL REAL TARGET_FORCE

INT iDetected;

iDetected=0;
!DynamicCurrentRatio = 2

COUNT = 1					!Loop Count
START_POS = 40				!Waiting Position(mm)
SOFT_START_POS = 50			!Softlanding Start Position(mm)
Detect_POS = 10;				!Detect Forcemode Switching Width(mm)
Detect_VEL = 1;				!Softlanding Velocity(mm/s)
Detect_Time = 4500			!Softlanding Start Time + Forcemode Switching Time(ms)
DCOM(2)=0

!if Force_Flag = 1
	LOOP COUNT
		VEL(2)=20; ACC(2)=1000; DEC(2)=1000; JERK(2)=10000
		PTP/E 2, START_POS									!Start Position
		WAIT 500
		!PTP/f 2, SOFT_START_POS, 5							!Move to Softlanding Position
		PTP/v 2, (SOFT_START_POS + Detect_POS), Detect_VEL	!Detect Force Index
	!	WAIT Detect_Time
		WAIT 10
		TILL GMQU(2) = 1 & GPHASE(2) = 4; WAIT 10			! Motion Queue & Constant Velocity
		TILL 0.05 < ABS(getsp(1,getspa(1,"axes[0].command")))!Force Mode Current Limit
	KILL 2; PTP/ER 2, -0.1
	
		DCOM(2)=DynamicCurrentRatio ; MFLAGS(2).#OPEN=1 ! DCOM
		WAIT 2000
	END
	
!	Force_Flag = 0
!END

STOP

!ON ^PST(2).#RUN &  MST(2).#OPEN=1 & Force_Flag; KILL (2); set PE(2)=0; disp"force mode on"; RET
!ON ^PST(2).#RUN & ^MST(2).#OPEN   & Force_Flag; WAIT 100; PTP/e 2,START_POS; XCURV(2)=100; WAIT 100; disp"force mode off"; iDetected=1;Ret

#12
!TEST - Z axis Force Control Buffer
REAL START_CHECK_POS
!DynamicCurrentRatio = 2

COUNT = 1					!Loop Count
START_POS = 0				!Waiting Position(mm)
START_CHECK_POS = 48		!Softlanding Start Position(mm)
Detect_POS = 15;			!Detect Forcemode Switching Width(mm)
Detect_VEL = 1;				!Softlanding Velocity(mm/s)
Detect_Time = 4500			!Softlanding Start Time + Forcemode Switching Time(ms)
DCOM(2)=  2.5 !DCOM_TEST_VALUE
Force_Flag = 0
Drng_Flag = 0
!Wait Pos Move
!MFLAGS(2).#OPEN=0
!WAIT 100

!if Force_Flag = 1
	LOOP COUNT
		VEL(2)=20; ACC(2)=1000; DEC(2)=1000; JERK(2)=10000
		PTP/E 2, START_POS									!Start Position
		WAIT 500
		PTP/f 2, START_CHECK_POS, 5							!Move to Softlanding Position
		PTP/v 2, (START_CHECK_POS + Detect_POS), Detect_VEL	!Detect Force Index
	!	WAIT Detect_Time
		WAIT 10
		TILL GMQU(2) = 1 & GPHASE(2) = 4; 
		
		WAIT 10			! Motion Queue & Constant Velocity
		TILL 0.05 < ABS(getsp(1,getspa(1,"axes[0].command")))!Force Mode Current Limit
	
	KILL 2; PTP/ER 2, -0.1
	
		!DCOM(2)=Z_OFFSET+Dy4namicCurrentRatio; 
		MFLAGS(2).#OPEN=1 ! DCOM
		WAIT 2000
	END
!END
STOP

!ON ^PST(2).#RUN &  MST(2).#OPEN=1 & Force_Flag; KILL (2); set PE(2)=0; DISP"force mode on"; RET
!ON ^PST(2).#RUN & ^MST(2).#OPEN   & Force_Flag; WAIT 100; PTP/e 2,START_POS; XCURV(2)=100; WAIT 100; disp"force mode off"; iDetected=1;Ret

#13
!Milling Buffer
INT AXIS1,AXIS2, SLAVE_AXIS, Direction
REAL Save(64)(5)
REAL R
!int CNT
int Y_LOOP_COUNT!,X_LOOP_COUNT
REAL X_LOOP_COUNT
REAL Search_vel
INT DELAY_T, TimeOut
Global real X_Ditance, Y_Ditance
!Global real X_START_POS, X_END_POS, Y_START_POS, Y_END_POS, Y_STEP_POS
!Global int X_cnt, Y_cnt

! Variable declaration for Segment execution synchronization count 
GLOBAL INT XSEG_VALUE(1)
GLOBAL INT XSEG_Output(2), XSEG_MASK(1)

X_START_POS = DATA_EQ_TO_ACS(20)
X_END_POS   = DATA_EQ_TO_ACS(21)
Y_START_POS = DATA_EQ_TO_ACS(22)
Y_END_POS   = DATA_EQ_TO_ACS(23)
Y_STEP_POS  = DATA_EQ_TO_ACS(24)

X_SPEED     = DATA_EQ_TO_ACS(26)

!!!!!user define value!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
AXIS1 = 0
AXIS2 = DATA_EQ_TO_ACS(27) !JUNG/200515

TimeOut = 1000               		! [1sec], TimeOut depends on Homing time: serch vel and travel Time out 
DELAY_T = 50						! [1sec], TimeOut depends on Homing time: serch vel and travel Time out 

!SIMULATOR Porfile!!
!X_START_POS = 207
!X_END_POS = 200
!Y_START_POS = 29.657
!Y_END_POS = 30.657
!Y_STEP_POS = 0.05
!SIMULATOR Porfile!!

! Variable initialize
XSEG_VALUE(0) = 1;
XSEG_MASK(0)  = 0xFF;
FILL (0, XSEG_Output);
XSEG_CNT = 0;

R = 1 
CNT = 0
X_Ditance = X_END_POS - X_START_POS 
Y_Ditance = Y_END_POS - Y_START_POS 

Call MotionProfile

PTP/E (AXIS1,AXIS2),(X_START_POS),(Y_START_POS)
WAIT DELAY_T	
DISP "START Miling Process"
REAL Excute_Count
DISP "FPOS(AXIS1) = %f,", FPOS(AXIS1), "FPOS(AXIS2) = %f", FPOS(AXIS2);
IF Y_Ditance >= 0	Y_STEP_POS = Y_STEP_POS; ELSE	Y_STEP_POS=-Y_STEP_POS; END
Excute_Count = ABS(Y_Ditance/Y_STEP_POS) 
Excute_Count = FLOOR(Excute_Count)

XSEG/uy (AXIS1, AXIS2), X_START_POS, Y_START_POS, R
	LOOP Excute_Count
		LINE/o (AXIS1, AXIS2), X_START_POS + X_Ditance * ^CNT.0, Y_START_POS + Y_STEP_POS * CNT, XSEG_VALUE, XSEG_Output 
		!DISP "Count: %d. AXIS0= %f, AXIS1=%f", CNT, (X_START_POS + X_Ditance * ^CNT.0), (Y_START_POS + Y_STEP_POS * CNT); 	
		CNT = CNT + 1
		LINE/o (AXIS1, AXIS2), X_START_POS + X_Ditance * CNT.0,  Y_START_POS + Y_STEP_POS * CNT, XSEG_VALUE, XSEG_Output	
		TILL GSFREE(AXIS1) >= 25
		!DISP "Count: %d. AXIS0= %f, AXIS1=%f", CNT,(X_START_POS + X_Ditance * CNT.0), (Y_START_POS + Y_STEP_POS * CNT)
	END
	IF CNT.0
		LINE/o (AXIS1, AXIS2), X_START_POS + (X_Ditance * ^CNT.0),  Y_START_POS + Y_STEP_POS * CNT, XSEG_VALUE, XSEG_Output
		!DISP "Count: %d. AXIS0= %f, AXIS1=%f", CNT,(X_START_POS + X_Ditance * ^CNT.0), (Y_START_POS + Y_STEP_POS * CNT)
	END
ENDS (AXIS1, AXIS2)

TILL GSEG(AXIS1) = -1

STOP

Time_Out:
DISP" Miling fault time_out, Axis number=",AXIS1, "Axis2 number=", AXIS2
HALT (AXIS1, AXIS2)
TILL ^MST(AXIS1).#MOVE & ^MST(AXIS2).#MOVE, TimeOut
DISABLE (AXIS1, AXIS2)

call Restore

STOP
Restore:
VEL (AXIS1) = Save(AXIS1)(0)
ACC (AXIS1) = Save(AXIS1)(1)
DEC (AXIS1) = Save(AXIS1)(2)
JERK(AXIS1) = Save(AXIS1)(3)
KDEC(AXIS1) = Save(AXIS1)(4)
DISP "RESTOR"
RET

MotionProfile:					!! calculate and save motion profile
Save(AXIS1)(0)=VEL(AXIS1)			! operating motion profile backup
Save(AXIS1)(1)=ACC(AXIS1)
Save(AXIS1)(2)=DEC(AXIS1)
Save(AXIS1)(3)=JERK(AXIS1)
Save(AXIS1)(4)=KDEC(AXIS1)

Save(AXIS2)(0)=VEL(AXIS2)			! operating motion profile backup
Save(AXIS2)(1)=ACC(AXIS2)
Save(AXIS2)(2)=DEC(AXIS2)
Save(AXIS2)(3)=JERK(AXIS2)
Save(AXIS2)(4)=KDEC(AXIS2)

VEL(AXIS1) = X_SPEED     !10  !200
ACC(AXIS1) = 1000         !3000 ! VEL(AXIS1)*5
DEC(AXIS1) = ACC(AXIS1)
JERK(AXIS1)= 10000        ! ACC(AXIS1)*5
KDEC(AXIS1)= JERK(AXIS1)

VEL (AXIS2) = 20
ACC (AXIS2) = 200 !3000 !VEL(AXIS2)*5
DEC (AXIS2) = ACC(AXIS2)
JERK(AXIS2)= 8000 !ACC(AXIS2)*5
KDEC(AXIS2)= JERK(AXIS2)

ENABLE (AXIS1,AXIS2)
TILL MST(AXIS1).#ENABLED & MST(AXIS2).#ENABLED
SAFETYGROUP(AXIS1, AXIS2)	! safetygroup disable
RET

#14
!Z axis Force Control Buffer
global real CurrentRatio
global real DynamicCurrentRatio
INT iDetected;

iDetected=0;
!DynamicCurrentRatio = 2

COUNT = 1					!Loop Count
START_POS = 0				!Waiting Position(mm)
!SOFT_START_POS = 49		!Softlanding Start Position(mm)
Detect_POS = 10;			!Detect Forcemode Switching Width(mm)
Detect_VEL = 1;				!Softlanding Velocity(mm/s)
Detect_Time = 4500			!Softlanding Start Time + Forcemode Switching Time(ms)
DCOM(2)=0
Drng_Flag = 0

if Force_Flag = 1
    LOOP COUNT
		VEL(2)=20; ACC(2)=1000; DEC(2)=1000; JERK(2)=10000
		WAIT 500
		PTP/f 2, SOFT_START_POS, 5							!Move to Softlanding Position
		PTP/v 2, (SOFT_START_POS + Detect_POS), Detect_VEL	!Detect Force Index
	!	WAIT Detect_Time
		WAIT 10
		TILL GMQU(2) = 1 & GPHASE(2) = 4; 
		WAIT 10			 ! Motion Queue & Constant Velocity
		TILL 0.05 < ABS(getsp(1,getspa(1,"axes[0].command")))!Force Mode Current Limit
	KILL 2; PTP/ER 2, -0.1
	
		DCOM(2)=Z_OFFSET+DynamicCurrentRatio;  !DCOM
		MFLAGS(2).#OPEN=1 
		WAIT 2000
	END
	Drng_Flag = 1
END
STOP

ON ^PST(2).#RUN &  MST(2).#OPEN=1 & Force_Flag; 
	KILL (2); 
	set PE(2)=0; 
	disp"force mode on"; 
	Force_Flag = 0
	RET
	
!ON ^PST(2).#RUN & ^MST(2).#OPEN   & Drng_Flag & Force_Flag; WAIT 100; PTP/e 2,0; XCURV(2)=100; WAIT 100; disp"force mode off"; iDetected=1;Ret
ON ^PST(2).#RUN & ^MST(2).#OPEN & Drng_Flag !& Force_END; 
	WAIT 100; 
	XCURV(2)=100; 
	WAIT 100; 
	disp"force mode off"; 
	iDetected=1;
	Drng_Flag = 0
	Ret

#15
INT m_nEMS   !EMS
INT m_nLeak  !Leak
INT IO_NODE_NO, EC_ADDR, PRODUCT_ID, NODE_Q
INT PLC_GATE_NODE, II

AUTOEXEC:
ECUNMAP
WAIT 500
!---- Ethercat Slave Node Initialize
PLC_GATE_NODE = 34
IO_NODE_NO = 2


!SICK SAFETY PLC UNIT
PRODUCT_ID = ECGETPID(PLC_GATE_NODE)
DISP "EtherCAT PLC PRODUCT_ID : %d" , PRODUCT_ID

EC_ADDR = ECGETOFFSET("Diag", PLC_GATE_NODE)
DISP "EtherCAT PLC PRODUCT_ID : %d, ADDR : %d", PRODUCT_ID, EC_ADDR
ECIN(EC_ADDR, Diag(0))
EC_ADDR = ECGETOFFSET("In_Dataset1", PLC_GATE_NODE)
DISP "EtherCAT PLC PRODUCT_ID : %d, ADDR : %d", PRODUCT_ID, EC_ADDR
ECIN(EC_ADDR, SICK_DATA(0)) 
EC_ADDR = ECGETOFFSET("In_Dataset2", PLC_GATE_NODE)
DISP "EtherCAT PLC PRODUCT_ID : %d, ADDR : %d", PRODUCT_ID, EC_ADDR
ECIN(EC_ADDR, SICK_DATA(1))
EC_ADDR = ECGETOFFSET("Out_Dataset1", PLC_GATE_NODE)
DISP "EtherCAT PLC PRODUCT_ID : %d, ADDR : %d", PRODUCT_ID, EC_ADDR
ECOUT(EC_ADDR, EC_PLCOUT(0))

WAIT 100

!--Digital Input Beckhoff Digital Input Card EL1889
!--Node Number 2 ~ 13
II = 0
LOOP 12
	PRODUCT_ID = ECGETPID(IO_NODE_NO)
	EC_ADDR = ECGETOFFSET("Input", IO_NODE_NO)
	ECIN(EC_ADDR,EC_IN(II))
	DISP " EtherCAT_Input_ Mapping EC_IN(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+1
	ECIN(EC_ADDR,EC_IN(II))
	DISP " EtherCAT_Input_ Mapping EC_IN(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1
	IO_NODE_NO = IO_NODE_NO+1
END
!--Digital Output Beckhoff Digital Output Card EL2889
!--Node Number 15 ~ 20
II = 0
IO_NODE_NO = 15
LOOP 6
	PRODUCT_ID = ECGETPID(IO_NODE_NO)
	EC_ADDR = ECGETOFFSET("Output", IO_NODE_NO)
	ECOUT(EC_ADDR,EC_OUT(II))
	DISP " EtherCAT_Input_ Mapping EC_OUT(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+1
	ECOUT(EC_ADDR,EC_OUT(II))
	DISP " EtherCAT_Input_ Mapping EC_OUT(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1
	IO_NODE_NO = IO_NODE_NO+1
END

!Beckhoff Analog Input Card EL3164, 12Bit
!--Node Number 21 ~ 23
II = 0
IO_NODE_NO = 21
LOOP 3
	PRODUCT_ID = ECGETPID(IO_NODE_NO)
	EC_ADDR = ECGETOFFSET("Value", IO_NODE_NO)
	ECIN(EC_ADDR,EC_AIN(II))
	DISP " EtherCAT_Input_ Mapping EC_AIN(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+4
	ECIN(EC_ADDR,EC_AIN(II))
	DISP " EtherCAT_Input_ Mapping EC_AIN(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+4
	ECIN(EC_ADDR,EC_AIN(II))
	DISP " EtherCAT_Input_ Mapping EC_AIN(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+4
	ECIN(EC_ADDR,EC_AIN(II))
	DISP " EtherCAT_Input_ Mapping EC_AIN(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+4
	IO_NODE_NO = IO_NODE_NO+1
END
!Beckhoff Analog Output Card EL4104, 12Bit
!--Node Number 24
II = 0
IO_NODE_NO = 24
LOOP 1
	PRODUCT_ID = ECGETPID(IO_NODE_NO)
	EC_ADDR = ECGETOFFSET("Analog output", IO_NODE_NO)
	ECOUT(EC_ADDR,EC_AOUT(II))
	DISP " EtherCAT_OUT_ Mapping EC_AOUT(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+2
	ECOUT(EC_ADDR,EC_AOUT(II))
	DISP " EtherCAT_OUT_ Mapping EC_AOUT(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+2
	ECOUT(EC_ADDR,EC_AOUT(II))
	DISP " EtherCAT_OUT_ Mapping EC_AOUT(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+2
	ECOUT(EC_ADDR,EC_AOUT(II))
	DISP " EtherCAT_OUT_ Mapping EC_AOUT(%d) = ADDR : %d" , II, EC_ADDR
	II = II + 1 
	EC_ADDR = EC_ADDR+2
	IO_NODE_NO = IO_NODE_NO+1
END

!SMC JXCE1 Drive PDO Data, Node 30
ECIN(208, SMC_INPUT(0))
ECIN(210, SMC_INFO_FLAG(0))
ECIN(212, SMC_CURRENT_POS(0))
ECIN(216, SMC_CURRENT_VEL(0))
ECIN(218, SMC_READ_PUSH_FORCE(0))
ECIN(220, SMC_READ_TARGET_POS(0))
ECIN(224, SMC_ALRAM(0))

ECOUT(208, SMC_OUTPUT(0))			!Object 7010
ECOUT(210, SMC_CONTROL_FLAG(0))		!		7011
ECOUT(212, SMC_START_FLAG(0))		!		7012
ECOUT(213, SMC_MOVEMODE(0))			!		7020
ECOUT(214, SMC_TARGET_VEL(0))		!		7021
ECOUT(216, SMC_TARGET_POS(0))		!		7022
ECOUT(220, SMC_ACC_VEL(0))			!		7023
ECOUT(222, SMC_DEC_VEL(0))			!		7024
ECOUT(224, SMC_PUSH_FORCE(0))		!		7025
ECOUT(226, SMC_TRIGGER_LV(0))		!		7026
ECOUT(228, SMC_PUSH_SPEED(0))		!		7027
ECOUT(230, SMC_MOVE_FORCE(0))		!		7028
ECOUT(240, SMC_IN_POSISIONT(0))		!		702B

!SMC JXCE1 Drive PDO Data, Node 31
ECIN(244, SMC_INPUT(1))
ECIN(246, SMC_INFO_FLAG(1))
ECIN(248, SMC_CURRENT_POS(1))
ECIN(252, SMC_CURRENT_VEL(1))
ECIN(254, SMC_READ_PUSH_FORCE(1))
ECIN(256, SMC_READ_TARGET_POS(1))
ECIN(260, SMC_ALRAM(1))

ECOUT(244, SMC_OUTPUT(1))
ECOUT(246, SMC_CONTROL_FLAG(1))
ECOUT(248, SMC_START_FLAG(1))
ECOUT(250, SMC_TARGET_VEL(1))
ECOUT(252, SMC_TARGET_POS(1))
ECOUT(256, SMC_ACC_VEL(1))			!		7023
ECOUT(258, SMC_DEC_VEL(1))			!		7024
ECOUT(260, SMC_PUSH_FORCE(1))		!		7025
ECOUT(262, SMC_TRIGGER_LV(1))		!		7026
ECOUT(264, SMC_PUSH_SPEED(1))		!		7027
ECOUT(266, SMC_MOVE_FORCE(1))		!		7028
ECOUT(276, SMC_IN_POSISIONT(1))		!		702B

!SMC JXCE1 Drive PDO Data, Node 32
ECIN(280, SMC_INPUT(2))
ECIN(282, SMC_INFO_FLAG(2))
ECIN(284, SMC_CURRENT_POS(2))
ECIN(288, SMC_CURRENT_VEL(2))
ECIN(290, SMC_READ_PUSH_FORCE(2))
ECIN(292, SMC_READ_TARGET_POS(2))
ECIN(296, SMC_ALRAM(2))

ECOUT(280, SMC_OUTPUT(2))
ECOUT(282, SMC_CONTROL_FLAG(2))
ECOUT(284, SMC_START_FLAG(2))
ECOUT(286, SMC_TARGET_VEL(2))
ECOUT(288, SMC_TARGET_POS(2))
ECOUT(292, SMC_ACC_VEL(2))			!		7023
ECOUT(294, SMC_DEC_VEL(2))			!		7024
ECOUT(296, SMC_PUSH_FORCE(2))		!		7025
ECOUT(298, SMC_TRIGGER_LV(2))		!		7026
ECOUT(300, SMC_PUSH_SPEED(2))		!		7027
ECOUT(302, SMC_MOVE_FORCE(2))		!		7028
ECOUT(312, SMC_IN_POSISIONT(2))		!		702B
WAIT 100

!--SGD7S Safety IO Mapping
!--Node Number 25 ~ 29
IF ^IN(2).0; SAFINI(3).#LL = 0; ELSEIF IN(2).0; SAFINI(3).#LL = 1; END;
IF ^IN(2).1; SAFINI(3).#RL = 0; ELSEIF IN(2).1; SAFINI(3).#RL = 1; END;
IF ^IN(3).0; SAFINI(4).#LL = 0; ELSEIF IN(3).0; SAFINI(4).#LL = 1; END;
IF ^IN(3).1; SAFINI(4).#RL = 0; ELSEIF IN(3).1; SAFINI(4).#RL = 1; END;
IF ^IN(4).0; SAFINI(5).#LL = 0; ELSEIF IN(4).0; SAFINI(5).#LL = 1; END;
IF ^IN(4).1; SAFINI(5).#RL = 0; ELSEIF IN(4).1; SAFINI(5).#RL = 1; END;
IF ^IN(5).0; SAFINI(6).#LL = 0; ELSEIF IN(5).0; SAFINI(6).#LL = 1; END;
IF ^IN(5).1; SAFINI(6).#RL = 0; ELSEIF IN(5).1; SAFINI(6).#RL = 1; END;
IF ^IN(6).0; SAFINI(7).#LL = 0; ELSEIF IN(6).0; SAFINI(7).#LL = 1; END;
IF ^IN(6).1; SAFINI(7).#RL = 0; ELSEIF IN(6).1; SAFINI(7).#RL = 1; END;
	   
WHILE 1
	!A = ((getsp(0,getspa(0,"axes[0].command"))))*100; wait 10;
	!Actual_Current = 10 * A;
	BLOCK
		!DATA_EQ_TO_SMC(EQ --> SMC)			
		SMC_START_FLAG(0).0	  	= DATA_EQ_TO_SMC(0)				!GO
		SMC_OUTPUT(0).11		= DATA_EQ_TO_SMC(1)	  			!Fault Reset
		SMC_OUTPUT(0).9			= DATA_EQ_TO_SMC(2)	  			!Servo On
		SMC_OUTPUT(0).13		= DATA_EQ_TO_SMC(3)	  			!JOG -
		SMC_OUTPUT(0).14	  	= DATA_EQ_TO_SMC(4)	  			!JOG +
		SMC_OUTPUT(0).15		= DATA_EQ_TO_SMC(5)	  			!1mm Step Move
		SMC_OUTPUT(0).12		= DATA_EQ_TO_SMC(6)	  			!Home
		SMC_OUTPUT(0).0			= DATA_EQ_TO_SMC(7)	  			!Default Parameter Set
		SMC_TARGET_VEL(0)		= DATA_EQ_TO_SMC(8)	  			!TARGET VELOCITY
		SMC_TARGET_POS(0) 		= DATA_EQ_TO_SMC(9)! * 100		!TARGET POSITION
		SMC_OUTPUT(0).8			= DATA_EQ_TO_SMC(10)			!HOLD
		SMC_CONTROL_FLAG(0).5	= DATA_EQ_TO_SMC(11)			!ACC Default Set on
		SMC_CONTROL_FLAG(0).6	= DATA_EQ_TO_SMC(12)			!DEC Default Set on
		SMC_CONTROL_FLAG(0).7	= DATA_EQ_TO_SMC(13)			!Acc Set on
		SMC_CONTROL_FLAG(0).8	= DATA_EQ_TO_SMC(14)			!Dec Set on
		SMC_MOVEMODE(0).0		= DATA_EQ_TO_SMC(15)			!MOVEMODE
		SMC_MOVEMODE(0).1		= DATA_EQ_TO_SMC(16)			!MOVEMODE
		SMC_ACC_VEL(0)			= DATA_EQ_TO_SMC(17)			!ACC_VEL
		SMC_DEC_VEL(0)			= DATA_EQ_TO_SMC(18)			!DEC_VEL
		SMC_PUSH_FORCE(0)		= DATA_EQ_TO_SMC(19)			!PUSH_FORCE
		SMC_TRIGGER_LV(0)		= DATA_EQ_TO_SMC(20)			!TRIGGER_LV
		SMC_PUSH_SPEED(0)		= DATA_EQ_TO_SMC(21)			!PUSH_SPEED
		SMC_MOVE_FORCE(0)		= DATA_EQ_TO_SMC(22)			!MOVE_FORCE
		SMC_IN_POSISIONT(0)		= DATA_EQ_TO_SMC(23)			!TARGRAD
		
		SMC_START_FLAG(2).0	  	= DATA_EQ_TO_SMC(30)			!GO
		SMC_OUTPUT(2).11		= DATA_EQ_TO_SMC(31)	  		!Fault Reset
		SMC_OUTPUT(2).9			= DATA_EQ_TO_SMC(32)	  		!Servo On
		SMC_OUTPUT(2).13		= DATA_EQ_TO_SMC(33)	  		!JOG -
		SMC_OUTPUT(2).14	  	= DATA_EQ_TO_SMC(34)	  		!JOG +
		SMC_OUTPUT(2).15		= DATA_EQ_TO_SMC(35)	  		!1mm Step Move
		SMC_OUTPUT(2).12		= DATA_EQ_TO_SMC(36)	  		!Home
		SMC_OUTPUT(2).0			= DATA_EQ_TO_SMC(37)	  		!Default Parameter Set
		SMC_TARGET_VEL(2)		= DATA_EQ_TO_SMC(38)	  		!TARGET VELOCITY
		SMC_TARGET_POS(2) 		= DATA_EQ_TO_SMC(39)! * 100		!TARGET POSITION
		SMC_OUTPUT(2).8			= DATA_EQ_TO_SMC(40)			!HOLD
		SMC_CONTROL_FLAG(2).5	= DATA_EQ_TO_SMC(41)			!ACC Default Set on
		SMC_CONTROL_FLAG(2).6	= DATA_EQ_TO_SMC(42)			!DEC Default Set on
		SMC_CONTROL_FLAG(2).7	= DATA_EQ_TO_SMC(43)			!Acc Set on
		SMC_CONTROL_FLAG(2).8	= DATA_EQ_TO_SMC(44)			!Dec Set on
		SMC_MOVEMODE(2).0		= DATA_EQ_TO_SMC(45)			!MOVEMODE
		SMC_MOVEMODE(2).1		= DATA_EQ_TO_SMC(46)			!MOVEMODE
		SMC_ACC_VEL(2)			= DATA_EQ_TO_SMC(47)			!ACC_VEL
		SMC_DEC_VEL(2)			= DATA_EQ_TO_SMC(48)			!DEC_VEL
		SMC_PUSH_FORCE(2)		= DATA_EQ_TO_SMC(49)			!PUSH_FORCE
		SMC_TRIGGER_LV(2)		= DATA_EQ_TO_SMC(50)			!TRIGGER_LV
		SMC_PUSH_SPEED(2)		= DATA_EQ_TO_SMC(51)			!PUSH_SPEED
		SMC_MOVE_FORCE(2)		= DATA_EQ_TO_SMC(52)			!MOVE_FORCE
		SMC_IN_POSISIONT(2)		= DATA_EQ_TO_SMC(53)			!TARGRAD
		
		SMC_START_FLAG(1).0	  	= DATA_EQ_TO_SMC(60)			!GO
		SMC_OUTPUT(1).11		= DATA_EQ_TO_SMC(61)	  		!Fault Reset
		SMC_OUTPUT(1).9			= DATA_EQ_TO_SMC(62)	  		!Servo On
		SMC_OUTPUT(1).13		= DATA_EQ_TO_SMC(63)	  		!JOG -
		SMC_OUTPUT(1).14	  	= DATA_EQ_TO_SMC(64)	  		!JOG +
		SMC_OUTPUT(1).15		= DATA_EQ_TO_SMC(65)	  		!1mm Step Move
		SMC_OUTPUT(1).12		= DATA_EQ_TO_SMC(66)	  		!Home
		SMC_OUTPUT(1).0			= DATA_EQ_TO_SMC(67)	  		!Default Parameter Set
		SMC_TARGET_VEL(1)		= DATA_EQ_TO_SMC(68)	  		!TARGET VELOCITY
		SMC_TARGET_POS(1) 		= DATA_EQ_TO_SMC(69)! * 100		!TARGET POSITION
		SMC_OUTPUT(1).8			= DATA_EQ_TO_SMC(70)			!HOLD
		SMC_CONTROL_FLAG(1).5	= DATA_EQ_TO_SMC(71)			!ACC Default Set on
		SMC_CONTROL_FLAG(1).6	= DATA_EQ_TO_SMC(72)			!DEC Default Set on
		SMC_CONTROL_FLAG(1).7	= DATA_EQ_TO_SMC(73)			!Acc Set on
		SMC_CONTROL_FLAG(1).8	= DATA_EQ_TO_SMC(74)			!Dec Set on
		SMC_MOVEMODE(1).0		= DATA_EQ_TO_SMC(75)			!MOVEMODE
		SMC_MOVEMODE(1).1		= DATA_EQ_TO_SMC(76)			!MOVEMODE
		SMC_ACC_VEL(1)			= DATA_EQ_TO_SMC(77)			!ACC_VEL
		SMC_DEC_VEL(1)			= DATA_EQ_TO_SMC(78)			!DEC_VEL
		SMC_PUSH_FORCE(1)		= DATA_EQ_TO_SMC(79)			!PUSH_FORCE
		SMC_TRIGGER_LV(1)		= DATA_EQ_TO_SMC(80)			!TRIGGER_LV
		SMC_PUSH_SPEED(1)		= DATA_EQ_TO_SMC(81)			!PUSH_SPEED
		SMC_MOVE_FORCE(1)		= DATA_EQ_TO_SMC(82)			!MOVE_FORCE
		SMC_IN_POSISIONT(1)		= DATA_EQ_TO_SMC(83)			!TARGRAD
		
		!DATA_SMC_TO_EQ(EQ --> ACS)			
		DATA_SMC_TO_EQ(0) = SMC_CURRENT_POS(0)
		DATA_SMC_TO_EQ(1) = SMC_CURRENT_VEL(0)
		DATA_SMC_TO_EQ(2) = SIGN(SMC_ALRAM(0))			!Fault State, SIGN(SMC_ALRAM(0))
		DATA_SMC_TO_EQ(3) = SMC_ALRAM(0)		!Alram Code
		DATA_SMC_TO_EQ(4) =	SMC_INPUT(0).10			!Home Flag
		DATA_SMC_TO_EQ(5) = SMC_INPUT(0).11			!Inposition Signal
		DATA_SMC_TO_EQ(6) = SMC_INPUT(0).8			!Busy Signal
		DATA_SMC_TO_EQ(7) = SMC_INPUT(0).9			!Servo On Signal
		DATA_SMC_TO_EQ(8) = SMC_INPUT(0).12 		!AREA
		DATA_SMC_TO_EQ(9) = SMC_INPUT(0).14			!ESTOP
		DATA_SMC_TO_EQ(10) = SMC_READ_PUSH_FORCE(0)	!PUSH FORCE
		DATA_SMC_TO_EQ(11) = SMC_READ_TARGET_POS(0)	!READING TARGET POSITION
		DATA_SMC_TO_EQ(12) = SMC_INFO_FLAG(0).4		!READY
		
		DATA_SMC_TO_EQ(60) = SMC_CURRENT_POS(1)
		DATA_SMC_TO_EQ(61) = SMC_CURRENT_VEL(1)
		DATA_SMC_TO_EQ(62) = SIGN(SMC_ALRAM(1))		!Fault State
		DATA_SMC_TO_EQ(63) = SMC_ALRAM(1)		!Alram Code
		DATA_SMC_TO_EQ(64) = SMC_INPUT(1).10		!Home Flag
		DATA_SMC_TO_EQ(65) = SMC_INPUT(1).11		!Inposition Signal
		DATA_SMC_TO_EQ(66) = SMC_INPUT(1).8			!Busy Signal
		DATA_SMC_TO_EQ(67) = SMC_INPUT(1).9			!Servo On Signal
		DATA_SMC_TO_EQ(68) = SMC_INPUT(1).12 		!AREA
		DATA_SMC_TO_EQ(69) = SMC_INPUT(1).14		!ESTOP
		DATA_SMC_TO_EQ(70) = SMC_READ_PUSH_FORCE(1)	!PUSH FORCE
		DATA_SMC_TO_EQ(71) = SMC_READ_TARGET_POS(1)	!READING TARGET POSITION
		DATA_SMC_TO_EQ(72) = SMC_INFO_FLAG(1).4		!READY

		DATA_SMC_TO_EQ(30) = SMC_CURRENT_POS(2)
		DATA_SMC_TO_EQ(31) = SMC_CURRENT_VEL(2)
		DATA_SMC_TO_EQ(32) = SIGN(SMC_ALRAM(2))		!Fault State
		DATA_SMC_TO_EQ(33) = SMC_ALRAM(2)		!Alram Code
		DATA_SMC_TO_EQ(34) = SMC_INPUT(2).10		!Home Flag
		DATA_SMC_TO_EQ(35) = SMC_INPUT(2).11		!Inposition Signal
		DATA_SMC_TO_EQ(36) = SMC_INPUT(2).8			!Busy Signal
		DATA_SMC_TO_EQ(37) = SMC_INPUT(2).9			!Servo On Signal
		DATA_SMC_TO_EQ(38) = SMC_INPUT(2).12 		!AREA
		DATA_SMC_TO_EQ(39) = SMC_INPUT(2).14		!ESTOP
		DATA_SMC_TO_EQ(40) = SMC_READ_PUSH_FORCE(2)		!PUSH FORCE
		DATA_SMC_TO_EQ(41) = SMC_READ_TARGET_POS(2)	!READING TARGET POSITION
		DATA_SMC_TO_EQ(42) = SMC_INFO_FLAG(2).4		!READY

		!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		!EQ TO ACS
		Home_Offset(Main_X)           = DATA_EQ_TO_ACS(0)
		Home_Offset(Polishing_Y)      = DATA_EQ_TO_ACS(1)
		Home_Offset(Main_Z)           = DATA_EQ_TO_ACS(2)
		Home_Offset(Cleaning_T)       = DATA_EQ_TO_ACS(3)
		Home_Offset(Polishing_Theta)  = DATA_EQ_TO_ACS(4)
		Home_Offset(Polishing_Tilt)   = DATA_EQ_TO_ACS(5)
		Home_Offset(Storage_Y)        = DATA_EQ_TO_ACS(6)
		Home_Offset(Cleaning_Y)       = DATA_EQ_TO_ACS(7)

		!Force_Flag                    = DATA_EQ_TO_ACS(31)
		DCOM_TEST_VALUE               = DATA_EQ_TO_ACS(31)
		
		!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		!ACS TO EQ
		DATA_ACS_TO_EQ(20) = T_cnt
		DATA_ACS_TO_EQ(21) = X_cnt
		DATA_ACS_TO_EQ(22) = Y_cnt
		
		DATA_ACS_TO_EQ(23) = XSEG_CNT/2 !CNT 
		
		!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		!SICK Data
		EC_PLCIN(0) = SICK_DATA(0).8  !Leak-Polishing
		EC_PLCIN(1) = SICK_DATA(0).9  !Leak-Cleaning Bottom
		EC_PLCIN(2) = SICK_DATA(0).10 !Leak-Cleaning Top
		EC_PLCIN(3) = SICK_DATA(0).11 !Leak-Local Bottom Plate
		EC_PLCIN(4) = SICK_DATA(0).12 !Leak-Bottom Sol Box
		EC_PLCIN(5) = SICK_DATA(0).13 !Leak-Settling
		EC_PLCIN(6) = SICK_DATA(0).14 !Leak-Utility Inlet
		EC_PLCIN(7) = SICK_DATA(0).15 !Leak-Local Floor
		EC_PLCIN(8) = SICK_DATA(0).16 !Accura Gas
		EC_PLCIN(9) = SICK_DATA(0).17 !Accura Temp
		
		EC_PLCIN(10)= SICK_DATA(0).2  !DOOR-1
		EC_PLCIN(11)= SICK_DATA(0).3  !DOOR-2
		
		!EMS -> Valve/Pump Off
		m_nEMS  = EC_IN(6).5<>1 | EC_IN(6).6<>1 | EC_IN(6).7<>1
		m_nLeak = EC_PLCIN(0) | EC_PLCIN(1) | EC_PLCIN(2) | EC_PLCIN(3) | EC_PLCIN(4) | EC_PLCIN(5) | EC_PLCIN(6) | EC_PLCIN(7)
		IF m_nLeak | m_nEMS
			EC_OUT(7).0 = 0
			EC_OUT(7).1 = 0
			EC_OUT(7).2 = 0
			EC_OUT(7).3 = 0
			EC_OUT(7).4 = 0
			
			EC_OUT(7).7 = 0
			
			EC_OUT(8).4 = 0
			EC_OUT(8).5 = 0
			EC_OUT(8).6 = 0
			EC_OUT(8).7 = 0
			
			EC_OUT(9).0 = 0
	
			DATA_ACS_TO_EQ(49) = 1 !PC : Valve Control State
		ELSE 
			DATA_ACS_TO_EQ(49) = 0 
		END
		
		!MC ON
		EC_OUT(0).0 = 1
		
		!Home Sensor Display
		IF IN(0).0; DATA_ACS_TO_EQ(10) = 1; ELSE DATA_ACS_TO_EQ(10) = 0; END;
		IF IN(0).1; DATA_ACS_TO_EQ(11) = 1; ELSE DATA_ACS_TO_EQ(11) = 0; END;
		IF IN(1).0; DATA_ACS_TO_EQ(12) = 1; ELSE DATA_ACS_TO_EQ(12) = 0; END; !Z-Axis
		IF IN(2).2; DATA_ACS_TO_EQ(13) = 1; ELSE DATA_ACS_TO_EQ(13) = 0; END;
		IF IN(3).2; DATA_ACS_TO_EQ(14) = 1; ELSE DATA_ACS_TO_EQ(14) = 0; END;
		IF IN(4).2; DATA_ACS_TO_EQ(15) = 1; ELSE DATA_ACS_TO_EQ(15) = 0; END;
		IF IN(5).2; DATA_ACS_TO_EQ(16) = 1; ELSE DATA_ACS_TO_EQ(16) = 0; END;
		IF IN(6).2; DATA_ACS_TO_EQ(17) = 1; ELSE DATA_ACS_TO_EQ(17) = 0; END;
	
		
    END
END
STOP
ON ^PST(15).#RUN;EXEC "#15X"; RET

#A
!axisdef X=0,Y=1,Z=2,T=3,A=4,B=5,C=6,D=7
!axisdef x=0,y=1,z=2,t=3,a=4,b=5,c=6,d=7
global int I(100),I0,I1,I2,I3,I4,I5,I6,I7,I8,I9,I90,I91,I92,I93,I94,I95,I96,I97,I98,I99
global real V(100),V0,V1,V2,V3,V4,V5,V6,V7,V8,V9,V90,V91,V92,V93,V94,V95,V96,V97,V98,V99

AXISDEF Main_X = 0, Polishing_Y = 1, Main_Z = 2 !ACS Axis Define
AXISDEF Cleaning_T = 3, Polishing_Theta = 4, Polishing_Tilt = 5, Storage_Y = 6, Cleaning_Y = 7	!Yaskawa Axis Define

GLOBAL INT HomeFlag(64)
GLOBAL INT Addr_DampFactor, COUNT
GLOBAL REAL DampFactor, SOFT_START_POS, Detect_VEL, Z_OFFSET
GLOBAL REAL Detect_POS, START_POS, Detect_Time, Actual_Current, ContactPoint, ReadyPosition
GLOBAL INT ControlWord(64), StatusWord(64), ActualPosition(64), TP_FUNCTION(64), TP_STATUS(64), TP_POS(64), EACT_ACTUAL_POS(64)
GLOBAL REAL Home_Offset(64)

!MILLING DATA
GLOBAL INT  X_cnt, Y_cnt, T_cnt, CNT
GLOBAL REAL Y_START_POS,Y_END_POS,Y_STEP_POS
GLOBAL REAL X_START_POS,X_END_POS,X_STEP_POS, X_SPEED

!SMEC DATA
GLOBAL INT EC_IN(26), EC_OUT(26), EC_AIN(12), EC_AOUT(12)
GLOBAL REAL DATA_EQ_TO_ACS(50), DATA_ACS_TO_EQ(50)
GLOBAL INT  DATA_EQ_TO_SMC(90), DATA_SMC_TO_EQ(90)

!SMC READ DATA
GLOBAL INT SMC_INPUT(3), SMC_INFO_FLAG(3), SMC_CURRENT_VEL(3), SMC_CURRENT_POS(3), SMC_ALRAM(3), SMC_READ_PUSH_FORCE(3), SMC_READ_TARGET_POS(3), SMC_READY(3)
!SMC WRITE DATA
GLOBAL INT SMC_OUTPUT(3), SMC_START_FLAG(3), SMC_TARGET_VEL(3), SMC_TARGET_POS(3), SMC_CONTROL_FLAG(3), SMC_MOVEMODE(3), SMC_ACC_VEL(3), SMC_DEC_VEL(3)
GLOBAL INT SMC_TRIGGER_LV(3), SMC_PUSH_FORCE(3), SMC_PUSH_SPEED(3), SMC_MOVE_FORCE(3), SMC_IN_POSISIONT(3)
!SAFETYPLC DATA
GLOBAL INT EC_PLCOUT(20), EC_PLCIN(20), Diag(10)
GLOBAL INT SICK_DATA(2)

GLOBAL INT INPOS_COUNT(3), Force_Flag, Force_END
GLOBAL INT Drng_Flag

GLOBAL INT XSEG_CNT

!Force Test
GLOBAL REAL DCOM_TEST_VALUE

