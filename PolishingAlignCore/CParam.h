#pragma once

const int NUMBER_OF_MODELS = 3;
const int MODELS_MAX_OCCURRENCES = 16;
//---------------------------------------------------------------------------
/**
@struct	ST_PARAM_MODEL
@brief	MIL �� ���δ� �Ķ����
@remark	
 - 
@author	�����(Kyeong Kyu - Seon)
@date	2020/2/7  8:43
*/
struct ST_PARAM_MODEL
{
	int DetailLevel;        // ���� ���� ���� M_HIGH, M_MEDIUM(*), M_VERY_HIGH
	double Smoothness;      // ���� ������ ó�� ���� 50.0(*)(0~100.0) // ��ó��?
	double Acceptance;      // ���� 1�� �˻� ���ھ�.(������)
	double AcceptanceTarget; // �𵨿� ���ǵ��� ���� ������ ��� ����
	double Angle;           // �⺻ �߻����� ��ȯ�� �� �ִ� ������ �����Ѵ�.
	double AngleDeltaNeg;   // �⺻ �߻����� ��ȯ�� �� �ִ� ������ �����Ѵ�.
	double AngleDeltaPos;   // �⺻ �߻����� ��ȯ�� �� �ִ� ������ �����Ѵ�.
	int	   SearchAngleRange;// ���� ���� Ž�� M_DISABLE(*)
	double Certainty;       // �� ������ ���� ���ھ�.(Ȯ�Ǽ�)
	double CertaintyTarget; // Ư�� �� target ���ھ� Ȯ�Ǽ�
	double Scale;           // ��ô ���� �˻�
	double ScaleMaxFactor;  // ��ô ���� �˻�
	double ScaleMinFactor;  // ��ô ���� �˻�
	int SearchScaleRange;   // ��ô ���� Ž�� M_DISABLE(*)
	int Speed;              // �˻� �ӵ�
	int SearchOffsetX;		// �˻� ROI
	int SearchOffsetY;		// �˻� ROI
	int SearchSizeX;		// �˻� ROI
	int SearchSizeY;		// �˻� ROI
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
@brief	MIL ���� ��Ī �Ķ����
@remark	
 - 
@author	�����(Kyeong Kyu - Seon)
@date	2020/2/7  8:44
*/

struct ST_PARAM_PATTERN
{
	double Acceptance;		// �� ���ھ� ��� ����
	double Certainty;		// �� ���ھ� ���� Ȯ�Ǽ�
	int AngleMode;			// ���� �˻� ��� ����
	double NegativeDelta;	// ȸ�� ������ �������� ���Ѽ�
	double PositiveDelta;	// ȸ�� ���ڸ� �������� ���Ѽ�
	double Angle;			// �⺻ ȸ�� ����
	double Tolerance;		// ȸ������
	double Accuracy;		// ���� ��Ȯ��, �˻��� �̼�����, ȸ���������� �۾ƾ���.
	int InterpolationMode;	// Mark ȸ���� ���� ���
	int SearchOffsetX;		// �˻� ROI
	int SearchOffsetY;		// �˻� ROI
	int SearchSizeX;		// �˻� ROI
	int SearchSizeY;		// �˻� ROI

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
@brief	�˻� ��� ����ü
@remark	
 - 
@author	�����(Kyeong Kyu - Seon)
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
@brief	���� ����ü
@remark	
 - 
@author	�����(Kyeong Kyu - Seon)
@date	2020/2/14  14:05
*/
typedef struct ST_LINE_PARAM
{
	double a;	// 1�� ������ ����
	double b;	// 1�� ������ y����
	double x;	// ���Ⱑ ���Ѵ� �� �� x����
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