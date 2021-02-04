#pragma once
#include "IncludeHeader.h"
#include "CParam.h"
#include "CFunction.h"

class CMilAlign
{
public:
	CMilAlign();
	~CMilAlign();

private:
	MIL_ID				MilApplication = M_NULL;
	MIL_ID				Default_system = M_NULL;
	MIL_ID				Obj = M_NULL;
	MIL_ID				Mark = M_NULL;

	MIL_ID				Mod_Context = M_NULL;
	MIL_ID				Mod_Result = M_NULL;

	MIL_ID				Pat_Context = M_NULL;
	MIL_ID				Pat_Result = M_NULL;

	MIL_ID				Mod_CircleShape = M_NULL;
	MIL_ID				Mod_CircleResult = M_NULL;

	cv::Mat				m_matMark;
	cv::Mat				m_matImg;
	cv::Mat				m_matEdge;

	ST_PARAM_MODEL		m_stModel;
	ST_PARAM_PATTERN	m_stPattern;
	ST_ALIGN_RESULT		m_stResult;


	unsigned int		m_nLicenseType;

	const double		SEARCHOFFSET = 10.0;

private:
	cv::Mat		m_fn_GetMatImage(char* data, int width, int height, int channel);

public:
	char*		m_strLotName;
	void 		m_fn_InitMIL();
	int			m_fn_CheckLicense();
	void		m_fn_SetMark(char* data, int width, int height, int channel, bool bPreProc = false);
	void		m_fn_SetImage(char* data, int width, int height, int channel, bool bPreProc = false);
	void		m_fn_GetMarkPosition(void* param);

	void		m_fn_SetParamPatternMatching(void* paramPat);
	void		m_fn_PreparePatternMatching();
	void		m_fn_RunProcPatternMatching();
	void		m_fn_GetResultPatternMatching();
	void		m_fn_DestroyPatternMatching();

	void		m_fn_SetParamModelFinder(void* paramMod);
	void		m_fn_PrepareModelFinder();
	void		m_fn_RunProcModelFinder();
	void		m_fn_GetResultModelFinder();
	void		m_fn_DestroyModelFinder();

	double		m_fn_SetNextSearchScore(bool bAlgModel, bool bIncrease = false);

	void		m_fn_DestroyMIL();

	void        m_fn_PinSearch(double dTargetRadius, double dSmoothness);

	double		m_fn_GetTwoLineAngle(PointD pnt1, PointD pnt2);

	void		m_fn_cvPatternMatching(char* Img, int ImgWidth, int ImgHeight, char* Mark, int MarkWidth, int MarkHeight, PointD* RetPnt, double* RetValue);
	void*		m_fn_GetModelEdge(char* data, int width, int height, void* param, bool bPreProc = false);
	void		m_fn_ReleaseEdge();
	void		m_fn_PreProcessing(cv::Mat* mat);

	bool		m_fn_CheckSearchROI(double x, double y, double sizex, double sizey, bool bModel = true);

	static bool		m_fn_Cmp(const ST_RESULT& st1, const ST_RESULT& st2, bool bAscending = false);
};