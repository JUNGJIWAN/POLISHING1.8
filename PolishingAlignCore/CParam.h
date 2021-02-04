#pragma once

const int NUMBER_OF_MODELS = 3;
const int MODELS_MAX_OCCURRENCES = 16;
//---------------------------------------------------------------------------
/**
@struct	ST_PARAM_MODEL
@brief	MIL 모델 파인더 파라메터
@remark	
 - 
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  8:43
*/
struct ST_PARAM_MODEL
{
	int DetailLevel;        // 엣지 추출 감도 M_HIGH, M_MEDIUM(*), M_VERY_HIGH
	double Smoothness;      // 엣지 노이즈 처리 감도 50.0(*)(0~100.0) // 후처리?
	double Acceptance;      // 모델의 1차 검색 스코어.(허용수준)
	double AcceptanceTarget; // 모델에 정의되지 않은 엣지의 허용 수준
	double Angle;           // 기본 발생으로 반환될 수 있는 각도를 제한한다.
	double AngleDeltaNeg;   // 기본 발생으로 반환될 수 있는 각도를 제한한다.
	double AngleDeltaPos;   // 기본 발생으로 반환될 수 있는 각도를 제한한다.
	int	   SearchAngleRange;// 각도 범위 탐색 M_DISABLE(*)
	double Certainty;       // 모델 결정을 위한 스코어.(확실성)
	double CertaintyTarget; // 특정 모델 target 스코어 확실성
	double Scale;           // 축척 범위 검색
	double ScaleMaxFactor;  // 축척 범위 검색
	double ScaleMinFactor;  // 축척 범위 검색
	int SearchScaleRange;   // 축척 범위 탐색 M_DISABLE(*)
	int Speed;              // 검색 속도
	int SearchOffsetX;		// 검색 ROI
	int SearchOffsetY;		// 검색 ROI
	int SearchSizeX;		// 검색 ROI
	int SearchSizeY;		// 검색 ROI
	char dummy1;
	char dummy2;
	char dummy3;
	char dummy4;
	ST_PARAM_MODEL()
	{
		memset(this, 0x00, sizeof(ST_PARAM_MODEL));
	}
};
//---------------------------------------------------------------------------
/**
@struct	ST_PARAM_PATTERN
@brief	MIL 패턴 매칭 파라메터
@remark	
 - 
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  8:44
*/

struct ST_PARAM_PATTERN
{
	double Acceptance;		// 모델 스코어 허용 수준
	double Certainty;		// 모델 스코어 모델의 확실성
	int AngleMode;			// 각도 검색 허용 여부
	double NegativeDelta;	// 회전 공차를 기준으로 하한선
	double PositiveDelta;	// 회전 공자를 기준으로 상한선
	double Angle;			// 기본 회전 각도
	double Tolerance;		// 회전공차
	double Accuracy;		// 각도 정확도, 검색을 미세조정, 회전공차보다 작아야함.
	int InterpolationMode;	// Mark 회전시 보간 모드
	int SearchOffsetX;		// 검색 ROI
	int SearchOffsetY;		// 검색 ROI
	int SearchSizeX;		// 검색 ROI
	int SearchSizeY;		// 검색 ROI

	ST_PARAM_PATTERN()
	{
		memset(this, 0x00, sizeof(ST_PARAM_PATTERN));
	}
};

struct ST_INSPECTION
{
	double ROIX;
	double ROIY;
	double ROIW;
	double ROIH;
	int    Algorithm;
	double RefDistance;
	double RefDistance2;
	double Sigma;
	double Threshold;
	double LowThreshold;
	double HighThreshold;
	int    Condition;
	int    Section1;
	int    Section2;
	double DLLVHRate;
	double DLLLenRate;
	double DLLDiliteCnt;
	double DLLReserve1;
	double DLLReserve2;
	ST_INSPECTION()
	{
		memset(this, 0x00, sizeof(ST_INSPECTION));
	}
};

struct ST_RESULT
{
	double nX;
	double nY;
	double nWidth;
	double nHeight;
	double dScore;
	double dAngle;
	ST_RESULT()
	{
		memset(this, 0x00, sizeof(ST_RESULT));
	}
};

//---------------------------------------------------------------------------
/**
@struct	ST_ALIGN_RESULT
@brief	검색 결과 구조체
@remark	
 - 
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  13:53
*/
//#pragma pack(1)
struct ST_ALIGN_RESULT
{
	__int64 NumOfFound;
	ST_RESULT stResult[MODELS_MAX_OCCURRENCES];
};



//---------------------------------------------------------------------------
/**
@struct	ST_LINE_PARAM
@brief	직선 구조체
@remark	
 - 
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/14  14:05
*/
typedef struct ST_LINE_PARAM
{
	double a;	// 1차 방정식 기울기
	double b;	// 1차 방정식 y절편
	double x;	// 기울기가 무한대 일 때 x절편
	ST_LINE_PARAM()
	{
		memset(this, 0x00, sizeof(ST_LINE_PARAM));
	}
}LINE_PARAM;

//---------------------------------------------------------------------------

typedef struct ST_POINT_DOUBLE
{
	double x;
	double y;
	ST_POINT_DOUBLE()
	{
		memset(this, 0x00, sizeof(ST_POINT_DOUBLE));
	}
	ST_POINT_DOUBLE(double dx, double dy)
	{
		x = dx;
		y = dy;
	}
}PointD;

class MatLabelInfo
{
public:
	std::vector<cv::Point> pixels;
	int cx, cy;
	int minx, miny, maxx, maxy;
public:
	MatLabelInfo() : cx(0), cy(0), minx(INT_MAX), miny(INT_MAX), maxx(0), maxy(0)
	{
		pixels.clear();
	}
};