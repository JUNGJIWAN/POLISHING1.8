#pragma once
// #include "opencv2/opencv.hpp"
// #include "mil.h"
// #ifdef _WIN64
// #pragma comment(lib, "MIL/mil.lib")
// #pragma comment(lib, "MIL/milmod.lib")
// #pragma comment(lib, "MIL/milpat.lib")
// #ifdef _DEBUG
// #pragma comment(lib, "opencv_world430d.lib")
// #else
// #pragma comment(lib, "opencv_world430.lib")
// #endif
// #else
// #endif
#include "IncludeHeader.h"
#include "CParam.h"
#include "CFunction.h"
#include "CMilAlign.h"

//---------------------------------------------------------------------------
/**
@fn		CMilAlign::CMilAlign
@brief	클래스 초기화
@return	void
@param	void
@remark
 -
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  12:31
*/
CMilAlign::CMilAlign()
{
	m_nLicenseType = 0;
	m_strLotName = nullptr;
	
}
//---------------------------------------------------------------------------
/**
@fn		CMilAlign::~CMilAlign()
@brief	클래스 소멸자
@return	void
@param	void
@remark
 -
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  13:52
*/
CMilAlign::~CMilAlign()
{
// 	m_matImg.release();
// 	m_matMark.release();
}
//---------------------------------------------------------------------------
/**
@fn		cv::Mat CMilAlign::m_fn_GetMatImage(char* data, int width, int height, int bpp)
@brief	byte Array to cv::Mat
@return	Mat Image
@param	char*	data	: 이미지 버퍼
@param	int		width	: 이미지 폭
@param	int		height	: 이미지 높이
@param	int		bpp		: 이미지 채널
@remark
 -
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  11:38
*/
cv::Mat CMilAlign::m_fn_GetMatImage(char* data, int width, int height, int channel)
{
	int type;

	switch (channel)
	{
	case 1:
		type = CV_8UC1;
		break;
	case 3:
		type = CV_8UC3;
		break;
	case 4:
		type = CV_8UC4;
		break;
	default:
		type = CV_8UC1;
		break;
	}
	int stride = (width * channel + 3) & ~3;
	cv::Mat matTemp = cv::Mat::zeros(cv::Size(width, height), type);

	for (int i = 0; i < height; i++)
	{
		memcpy(matTemp.data + width * i, data + stride * i, static_cast<size_t>(width * channel));
	}


	return matTemp;
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_Init()
@brief	Lib Init
@return	int  라이센스 종류
@param	void
@remark
 - Mil Lib의 Application과 System 초기화.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/6  12:28
*/
void CMilAlign::m_fn_InitMIL()
{	
	if(MilApplication == 0) MappAlloc(M_NULL, M_DEFAULT, &MilApplication);
	if(Default_system == 0) MsysAlloc(M_DEFAULT, MIL_TEXT("M_SYSTEM_HOST"), M_DEFAULT, M_DEFAULT, &Default_system);
}

int CMilAlign::m_fn_CheckLicense()
{
	if (MilApplication > 0 && Default_system > 0)
	{
		MIL_INT LicenseMoules = 0;
		MappInquire(M_LICENSE_MODULES, &LicenseMoules);
		m_nLicenseType = (unsigned int )LicenseMoules;
	}
	else
		m_nLicenseType = 0;

	return m_nLicenseType;
}

//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_SetMark(cv::Mat matImg)
@brief	ROI 이미지 세팅
@return	void
@param	char*	data	: 이미지 버퍼
@param	int		width	: 이미지 폭`
@param	int		height	: 이미지 높이
@param	int		bpp		: 이미지 채널
@remark
 - Mil 버퍼에 마크 이미지 할당.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/6  12:29
*/
void CMilAlign::m_fn_SetMark(char* data, int width, int height, int bpp, bool bPreProc)
{
	if (m_nLicenseType == 0)
		return;
	m_matMark = m_fn_GetMatImage(data, width, height, bpp);
	//imshow("Mark",m_matMark);
	if (bPreProc)
	{
		m_fn_PreProcessing(&m_matMark);
	}
	if (Mark > 0)
	{
		MbufFree(Mark);
		Mark = 0;
	}
	MbufAlloc2d(Default_system, m_matMark.cols, m_matMark.rows, 8 + M_UNSIGNED, M_IMAGE + M_PROC + M_DISP, &Mark);
	MbufClear(Mark, M_RGB888(0, 0, 0));
	MbufPut2d(Mark, 0, 0, m_matMark.cols, m_matMark.rows, m_matMark.data);
}

/**
@fn     void CMilAlign::m_fn_PreProcessing(cv::Mat* mat)
@brief	
@return	void
@param	Mat* mat : 입력 이미지.
@remark	
 - 
@author	선경규(Kyeong Kyu - Seon)
@date	2020/9/23  14:52
*/
void CMilAlign::m_fn_PreProcessing(cv::Mat* mat)
{
	if (mat != nullptr)
	{
		cv::GaussianBlur(*mat, *mat, cv::Size(3, 3), 1.4);
		cv::threshold(*mat, *mat, 0, 255, cv::ThresholdTypes::THRESH_OTSU);
		cv::Mat mask = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
		cv::morphologyEx(*mat, *mat, cv::MORPH_OPEN, mask);
	}
}

//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_SetImage(cv::Mat matImg)
@brief	검색할 이미지 세팅
@return	void
@param	char*	data	: 이미지 버퍼
@param	int		width	: 이미지 폭
@param	int		height	: 이미지 높이
@param	int		bpp		: 이미지 채널
@remark
 - Mil 버퍼에 검색할 이미지 할당.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/6  13:43
*/
void CMilAlign::m_fn_SetImage(char* data, int width, int height, int bpp, bool bPreProc)
{
	if (m_nLicenseType == 0)
		return;
	m_matImg = m_fn_GetMatImage(data, width, height, bpp);
	//imshow("Debug", m_matImg);
	if (bPreProc)
		m_fn_PreProcessing(&m_matMark);

	if (Obj > 0)
	{
		MbufFree(Obj);
		Obj = 0;
	}
	MbufAlloc2d(Default_system, m_matImg.cols, m_matImg.rows, 8 + M_UNSIGNED, M_IMAGE + M_PROC + M_DISP, &Obj);
	MbufClear(Obj, M_RGB888(0, 0, 0));
	MbufPut2d(Obj, 0, 0, m_matImg.cols, m_matImg.rows, m_matImg.data);
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_SetParamModelFinder(void* paramMod)
@brief	클래스에 ModelFinder 파라메터 세팅
@return	void
@param	void* ModelParam : ModelFinder 파라메터 구조체 포인터
@remark
 - 외부에서 클래스에 ModelFinder 파라메터 세팅.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  10:37
*/
void CMilAlign::m_fn_SetParamModelFinder(void* paramMod)
{
	if (paramMod != nullptr)
		memcpy(&m_stModel, paramMod, sizeof(ST_PARAM_MODEL));
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_PrepareModelFinder()
@brief	ModelFinder Processing 준비
@return	void
@param	void
@remark
 - ModelFinder Processing을 위한 준비 단계.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  10:43
*/
void CMilAlign::m_fn_PrepareModelFinder()
{
	if (Default_system == 0 || MilApplication == 0)
		return;
	if (m_nLicenseType == 0)
		return;
	MmodAlloc(Default_system, M_GEOMETRIC, M_DEFAULT, &Mod_Context);

	MmodDefine(Mod_Context, M_IMAGE, Mark, 0, 0, m_matMark.cols, m_matMark.rows);

	// Time Out 10sec
	MmodControl(Mod_Context, M_CONTEXT, M_TIMEOUT, 5000);
	// Control Block for Mod Context
	MmodControl(Mod_Context, M_ALL, M_ACCEPTANCE, m_stModel.Acceptance);
	MmodControl(Mod_Context, M_ALL, M_CERTAINTY, m_stModel.Certainty);

	// Control Block for Mod Context
	if (m_stModel.SearchScaleRange > -1)
	{
		MmodControl(Mod_Context, M_CONTEXT, M_SEARCH_SCALE_RANGE, m_stModel.SearchScaleRange == 1 ? M_ENABLE : M_DISABLE);
		if (m_stModel.ScaleMinFactor < m_stModel.ScaleMaxFactor)
		{
			MmodControl(Mod_Context, M_ALL, M_SCALE_MIN_FACTOR, m_stModel.ScaleMinFactor);
			MmodControl(Mod_Context, M_ALL, M_SCALE_MAX_FACTOR, m_stModel.ScaleMaxFactor);
			//if (m_stModel.ScaleMinFactor != 0.0)
			//if (m_stModel.ScaleMaxFactor != 0.0)
		}
	}

	if (m_stModel.SearchAngleRange > -1)
	{
		MmodControl(Mod_Context, M_CONTEXT, M_SEARCH_ANGLE_RANGE, m_stModel.SearchAngleRange == 1 ? M_ENABLE : M_DISABLE);
		MmodControl(Mod_Context, M_ALL, M_ANGLE_DELTA_NEG, m_stModel.AngleDeltaNeg);
		MmodControl(Mod_Context, M_ALL, M_ANGLE_DELTA_POS, m_stModel.AngleDeltaPos);
		//if (m_stModel.AngleDeltaNeg != 0.0)
		//if (m_stModel.AngleDeltaPos != 0.0)
	}

	switch (m_stModel.DetailLevel)
	{
	case 0:  MmodControl(Mod_Context, M_CONTEXT, M_DETAIL_LEVEL, M_MEDIUM); break;
	case 1:  MmodControl(Mod_Context, M_CONTEXT, M_DETAIL_LEVEL, M_HIGH); break;
	case 2:  MmodControl(Mod_Context, M_CONTEXT, M_DETAIL_LEVEL, M_VERY_HIGH); break;
	default: MmodControl(Mod_Context, M_CONTEXT, M_DETAIL_LEVEL, M_MEDIUM); break;
	}

	MmodControl(Mod_Context, M_CONTEXT, M_SMOOTHNESS, m_stModel.Smoothness);
	
	MmodControl(Mod_Context, M_ALL, M_NUMBER, M_ALL);
	//MmodControl(Mod_Context, M_CONTEXT, M_NUMBER, 1);
	//Modelfinder
	if (m_stModel.SearchSizeX > 0 && m_stModel.SearchSizeY > 0)
	{
		MmodControl(Mod_Context, M_ALL, M_POSITION_X, m_stModel.SearchOffsetX + m_stModel.SearchSizeX / 2.0);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_NEG_X, m_stModel.SearchSizeX / 2.0);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_POS_X, m_stModel.SearchSizeX / 2.0);
		MmodControl(Mod_Context, M_ALL, M_POSITION_Y, m_stModel.SearchOffsetY + m_stModel.SearchSizeY / 2.0);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_NEG_Y, m_stModel.SearchSizeY / 2.0);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_POS_Y, m_stModel.SearchSizeY / 2.0);
	}
	else
	{
		MmodControl(Mod_Context, M_ALL, M_POSITION_X			, M_ALL);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_NEG_X	, 0.0);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_POS_X	, M_INFINITE);
		MmodControl(Mod_Context, M_ALL, M_POSITION_Y			, M_ALL);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_NEG_Y	, 0.0);
		MmodControl(Mod_Context, M_ALL, M_POSITION_DELTA_POS_Y	, M_INFINITE);
	}
}


//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_RunProcModelFinder()
@brief	ModelFinder Processing
@return	void
@param	void
@remark
 - ModelFinder Processing.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  10:53
*/
void CMilAlign::m_fn_RunProcModelFinder()
{
	if (m_nLicenseType == 0)
		return;
        if (Mod_Result <= 0) // model result 객체가 비어있을때만 할당.
	        MmodAllocResult(Default_system, M_DEFAULT, &Mod_Result);
	MmodPreprocess(Mod_Context, M_DEFAULT);
	MmodFind(Mod_Context, Obj, Mod_Result);
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_GetResultModelFinder()
@brief	ModelFinder 결과 얻음.
@return	void
@param	void
@remark
(200825-Seon Todo)
 v SearchROI 벗어난 결과 처리 할 것.
 v SearchROI 벗어난 결과를 예외 처리 하고나서 검색 결과가 0개 일 경우 처리 할 것.
 v SearchROi UI상에서 ModelROI보다 작을수 없게 처리 할 것.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/4/7  16:37
*/
void CMilAlign::m_fn_GetResultModelFinder()
{
	if (m_nLicenseType == 0)
		return;
	// 구조체 초기화 추가. [2020/5/26_10:11:27 Falcon]
	memset(&m_stResult, 0x00, sizeof(ST_ALIGN_RESULT));
	SYSTEMTIME systime;
	GetLocalTime(&systime);
	char strName[1024] = { 0, };

	long NumOfFound;
	double posX[MODELS_MAX_OCCURRENCES] = { 0, };
	double posY[MODELS_MAX_OCCURRENCES] = { 0, }; 
	double score[MODELS_MAX_OCCURRENCES] = { 0, }; 
	double angle[MODELS_MAX_OCCURRENCES] = { 0, };
	double scale[MODELS_MAX_OCCURRENCES] = { 0, };
	MIL_INT		Models[MODELS_MAX_OCCURRENCES] = { 0, };
	MIL_DOUBLE SizeX, SizeY;

	MmodGetResult(Mod_Result, M_GENERAL, M_NUMBER + M_TYPE_LONG, &NumOfFound);

	m_stResult.NumOfFound = NumOfFound;
	//if ((NumOfFound >= 1) && (NumOfFound <= MODELS_MAX_OCCURRENCES))
	if (NumOfFound > 0)
	{
		// Get the result for each model
		MmodGetResult(Mod_Result, M_DEFAULT, M_INDEX + M_TYPE_MIL_INT, Models);
		MmodGetResult(Mod_Result, M_ALL, M_POSITION_X, posX);
		MmodGetResult(Mod_Result, M_ALL, M_POSITION_Y, posY);
		MmodGetResult(Mod_Result, M_ALL, M_SCORE, score);
		MmodGetResult(Mod_Result, M_ALL, M_ANGLE, angle);
		MmodGetResult(Mod_Result, M_ALL, M_SCALE, scale);

		MmodInquire(Mod_Context, 0, M_ALLOC_SIZE_X, &SizeX);
		MmodInquire(Mod_Context, 0, M_ALLOC_SIZE_Y, &SizeY);

		// Check Search ROI InPosition
		// Result가 Search ROI에서 벗어나면 Score Zero 처리.
 		for (int i = 0; i < NumOfFound; i++)
 		{
 			if (!m_fn_CheckSearchROI(posX[i], posY[i], SizeX, SizeY))
 			{
 				score[i] *= -1;
 			}
 		}

		cv::Mat matlog = m_matImg.clone();
		if(matlog.channels() == 1)
			cvtColor(matlog, matlog, cv::ColorConversionCodes::COLOR_GRAY2BGR);
		cv::RotatedRect rect;
		cv::Point2f pnt[4];
		cv::Point pntScore;
		char strScore[1024] = { 0, };
		int j = 0;
		for (int i = 0; i < NumOfFound; i++)
		{
			if (score[i] <= 0)
				continue;

			rect.angle = static_cast<float>(angle[i] * -1);
			rect.center.x = static_cast<float>(posX[i]);
			rect.center.y = static_cast<float>(posY[i]);
			rect.size.width = static_cast<float>(SizeX);
			rect.size.height = static_cast<float>(SizeY);
			rect.points(pnt);
			line(matlog, pnt[0], pnt[1], cv::Scalar(0, 0, 255));
			line(matlog, pnt[1], pnt[2], cv::Scalar(0, 0, 255));
			line(matlog, pnt[2], pnt[3], cv::Scalar(0, 0, 255));
			line(matlog, pnt[3], pnt[0], cv::Scalar(0, 0, 255));

			sprintf_s(strScore, "%.2lf Score, %.2lf Angle", score[i], angle[i] * -1);
			pntScore.x = static_cast<int>(pnt[0].x);
			pntScore.y = static_cast<int>(pnt[1].y);
			cv::putText(matlog, strScore, pntScore, cv::HersheyFonts::FONT_HERSHEY_SIMPLEX, 1.0, cv::Scalar(0, 0, 255));

			if (j < MODELS_MAX_OCCURRENCES)
			{
				m_stResult.stResult[j].nX = posX[i];
				m_stResult.stResult[j].nY = posY[i];
				m_stResult.stResult[j].nWidth = SizeX;
				m_stResult.stResult[j].nHeight = SizeY;
				m_stResult.stResult[j].dAngle = angle[i];
				m_stResult.stResult[j].dScore = score[i];
				j++;
			}
		}
		if (j == 0)
			m_stResult.NumOfFound = 0;
		//std::sort(m_stResult.stResult, m_stResult.stResult + MODELS_MAX_OCCURRENCES, m_fn_Cmp);

		//sprintf_s(strName, ("D:\\Image\\%s\\%02d%02d%02d\\%02d%02d%02d_%02d%02d%02d_AlignResult.jpg"), m_strLotName, systime.wYear, systime.wMonth, systime.wDay, systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, systime.wSecond);
		sprintf_s(strName, ("D:\\Image\\%02d%02d%02d\\%02d%02d%02d_%02d%02d%02d_AlignResult.jpg"), systime.wYear, systime.wMonth, systime.wDay, systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, systime.wSecond);
		cv::imwrite(strName, matlog);
	}
}

double	CMilAlign::m_fn_SetNextSearchScore(bool bAlgModel, bool bIncrease)
{
	double dNowAcceptance = 0.0;
	if (bAlgModel)
	{
			if (bIncrease)
				m_stModel.Acceptance += SEARCHOFFSET;
			else
				m_stModel.Acceptance -= SEARCHOFFSET;
		if (m_stModel.Acceptance > 0 && m_stModel.Acceptance < 100)
			MmodControl(Mod_Context, M_ALL, M_ACCEPTANCE, m_stModel.Acceptance);

		dNowAcceptance = m_stModel.Acceptance;
	}
	else
	{
		if (bIncrease)
			m_stPattern.Acceptance += SEARCHOFFSET;
		else
			m_stPattern.Acceptance -= SEARCHOFFSET;

		if(m_stPattern.Acceptance > 0 && m_stPattern.Acceptance < 100)
			MpatControl(Mod_Context, M_ALL, M_ACCEPTANCE, m_stPattern.Acceptance);

		dNowAcceptance = m_stPattern.Acceptance;
	}

	return dNowAcceptance;
}

bool CMilAlign::m_fn_CheckSearchROI(double x, double y, double sizex, double sizey, bool bModel)
{
	bool bRtn = false;
	double dSearchOffsetX = 0.0;
	double dSearchOffsetY = 0.0;
	double dSearchSizeX   = 0.0;
	double dSearchSizeY   = 0.0;
	if (bModel)
	{
		dSearchOffsetX = m_stModel.SearchOffsetX;
		dSearchOffsetY = m_stModel.SearchOffsetY;
		dSearchSizeX   = m_stModel.SearchSizeX;
		dSearchSizeY   = m_stModel.SearchSizeY;
	}
	else
	{
		dSearchOffsetX = m_stPattern.SearchOffsetX;
		dSearchOffsetY = m_stPattern.SearchOffsetY;
		dSearchSizeX   = m_stPattern.SearchSizeX;
		dSearchSizeY   = m_stPattern.SearchSizeY;
	}
	if (dSearchSizeX != 0 && dSearchSizeY != 0)
	{
		if (dSearchOffsetX > x - sizex / 2.0 ||
			dSearchOffsetY > y - sizey / 2.0 ||
			dSearchOffsetX + dSearchSizeX < x + sizex / 2.0 ||
			dSearchOffsetY + dSearchSizeY < y + sizey / 2.0)
			bRtn = false;
		else
			bRtn = true;
	}
	else
		bRtn = true;
	return bRtn;
}

//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_DestroyModelFinder()
@brief	ModelFinder 객체 해제
@return	void
@param	void
@remark
 - ModelFinder 객체를 해제한다.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  11:43
*/
void CMilAlign::m_fn_DestroyModelFinder()
{
	if (m_nLicenseType == 0)
		return;
	if (Mod_Result > 0)
	{
		MmodFree(Mod_Result);
		Mod_Result = 0;
	}
	if (Mod_Context > 0)
	{
		MmodFree(Mod_Context);
		Mod_Context = 0;
	}
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_SetParamPatternMatching(void* paramPat)
@brief	클래스에 PatternMatching 파라메터 세팅
@return	void
@param	void* paramPat : PatternMatching 파라메터 구조체 포인터
@remark
 - 외부에서 클래스에 ModelFinder 파라메터 세팅.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  10:37
*/
void CMilAlign::m_fn_SetParamPatternMatching(void* paramPat)
{
	if (paramPat != nullptr)
		memcpy(&m_stPattern, paramPat, sizeof(ST_PARAM_PATTERN));
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_PreparePatternMatching()
@brief	PatternMatching Processing 준비
@return	void
@param	void
@remark
 - PatternMatching Processing을 위한 준비 단계.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  10:43
*/
void CMilAlign::m_fn_PreparePatternMatching()
{
	if (m_nLicenseType == 0)
		return;
	MpatAlloc(Default_system, M_DEFAULT, M_DEFAULT, &Pat_Context);
	MpatDefine(Pat_Context, M_REGULAR_MODEL, Mark, 0, 0, m_matMark.cols, m_matMark.rows, M_DEFAULT);

	// Control Block for Pat Context
	MpatControl(Pat_Context, M_CONTEXT, M_SEARCH_MODE, M_FIND_ALL_MODELS);

	MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_MODE, M_ENABLE);
	MpatControl(Pat_Context, M_ALL, M_ACCEPTANCE, m_stPattern.Acceptance);
	MpatControl(Pat_Context, M_ALL, M_CERTAINTY, m_stPattern.Certainty);
	if (m_stPattern.AngleMode > -1)
	{
		MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_MODE, M_ENABLE);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE, m_stPattern.Angle);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_ACCURACY, m_stPattern.Accuracy);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_TOLERANCE, m_stPattern.Tolerance);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_DELTA_NEG, m_stPattern.NegativeDelta);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_DELTA_POS, m_stPattern.PositiveDelta);
		
		switch (m_stPattern.InterpolationMode)
		{
		case 0:  MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_INTERPOLATION_MODE, M_BICUBIC);			 break;
		case 1:  MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_INTERPOLATION_MODE, M_BILINEAR);		 break;
		case 2:  MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_INTERPOLATION_MODE, M_NEAREST_NEIGHBOR); break;
		default: MpatControl(Pat_Context, M_ALL, M_SEARCH_ANGLE_INTERPOLATION_MODE, M_BICUBIC);			 break;
		}
		
	}

	//pattern
	if (m_stPattern.SearchSizeX > 0 && m_stPattern.SearchSizeY > 0)
	{
		MpatControl(Pat_Context, M_ALL, M_SEARCH_OFFSET_X, m_stPattern.SearchOffsetX);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_OFFSET_Y, m_stPattern.SearchOffsetY);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_SIZE_X, m_stPattern.SearchSizeX);
		MpatControl(Pat_Context, M_ALL, M_SEARCH_SIZE_Y, m_stPattern.SearchSizeY);
	}
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_RunProcPatternMatching()
@brief	PatternMatching Processing
@return	void
@param	void
@remark
 - PatternMatching Processing.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  10:53
*/
void CMilAlign::m_fn_RunProcPatternMatching()
{
	if (m_nLicenseType == 0)
		return;
        if (Pat_Result <= 0) // pattern match result가 비어있을때만 할당
	        MpatAllocResult(Default_system, M_DEFAULT, &Pat_Result);
	MpatPreprocess(Pat_Context, M_DEFAULT, M_NULL);
	MpatFind(Pat_Context, Obj, Pat_Result);
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_GetResultPatternMatching()
@brief	Pattern Matching 결과 얻기.
@return	void
@param	void
@remark
 -
@author	선경규(Kyeong Kyu - Seon)
@date	2020/4/7  16:33
*/
void CMilAlign::m_fn_GetResultPatternMatching()
{
	if (m_nLicenseType == 0)
		return;
	// 구조체 초기화 추가. [2020/5/26_10:11:27 Falcon]
	memset(&m_stResult, 0x00, sizeof(ST_ALIGN_RESULT));
	SYSTEMTIME systime;
	GetLocalTime(&systime);
	char strName[1024] = { 0, };

	long NumOfFound;
	double posX[MODELS_MAX_OCCURRENCES] = { 0, };
	double posY[MODELS_MAX_OCCURRENCES] = { 0, };
	double score[MODELS_MAX_OCCURRENCES] = { 0, };
	double angle[MODELS_MAX_OCCURRENCES] = { 0, };
	MIL_DOUBLE SizeX, SizeY;

	MpatGetResult(Pat_Result, M_ALL, M_POSITION_X, posX);
	MpatGetResult(Pat_Result, M_ALL, M_POSITION_Y, posY);
	MpatGetResult(Pat_Result, M_GENERAL, M_NUMBER + M_TYPE_LONG, &NumOfFound);
	m_stResult.NumOfFound = NumOfFound;
	//if ((NumOfFound >= 1) && (NumOfFound <= MODELS_MAX_OCCURRENCES))
	if (NumOfFound > 0)
	{
		// Get the result for each model
		MpatGetResult(Pat_Result, M_ALL, M_POSITION_X, posX);
		MpatGetResult(Pat_Result, M_ALL, M_POSITION_Y, posY);
		MpatGetResult(Pat_Result, M_ALL, M_SCORE, score);
		MpatGetResult(Pat_Result, M_ALL, M_ANGLE, angle);

		MpatInquire(Pat_Context, 0, M_ALLOC_SIZE_X, &SizeX);
		MpatInquire(Pat_Context, 0, M_ALLOC_SIZE_Y, &SizeY);

		for (int i = 0; i < NumOfFound; i++)
		{
			if (!m_fn_CheckSearchROI(posX[i], posY[i], SizeX, SizeY, false))
			{
				score[i] *= -1;
			}
		}

		cv::Mat matlog = m_matImg.clone();
		if (matlog.channels() == 1)
			cvtColor(matlog, matlog, cv::ColorConversionCodes::COLOR_GRAY2BGR);
		cv::RotatedRect rect;
		cv::Point2f pnt[4];
		cv::Point pntScore;
		char strScore[1024] = { 0, };
		int j = 0;
		for (int i = 0; i < NumOfFound; i++)
		{
			if (score[i] <= 0)
				continue;

			rect.angle = static_cast<float>(angle[i] * -1);
			rect.center.x = static_cast<float>(posX[i]);
			rect.center.y = static_cast<float>(posY[i]);
			rect.size.width = static_cast<float>(SizeX);
			rect.size.height = static_cast<float>(SizeY);
			rect.points(pnt);
			line(matlog, pnt[0], pnt[1], cv::Scalar(0, 0, 255));
			line(matlog, pnt[1], pnt[2], cv::Scalar(0, 0, 255));
			line(matlog, pnt[2], pnt[3], cv::Scalar(0, 0, 255));
			line(matlog, pnt[3], pnt[0], cv::Scalar(0, 0, 255));

			sprintf_s(strScore, "%.2lf Score, %.2lf Angle", score[i], angle[i] * -1);
			pntScore.x = static_cast<int>(pnt[0].x);
			pntScore.y = static_cast<int>(pnt[1].y);
			cv::putText(matlog, strScore, pntScore, cv::HersheyFonts::FONT_HERSHEY_SIMPLEX, 1.0, cv::Scalar(0, 0, 255));

			if (j < MODELS_MAX_OCCURRENCES)
			{
				m_stResult.stResult[j].nX = posX[i];
				m_stResult.stResult[j].nY = posY[i];
				m_stResult.stResult[j].nWidth = SizeX;
				m_stResult.stResult[j].nHeight = SizeY;
				m_stResult.stResult[j].dAngle = angle[i];
				m_stResult.stResult[j].dScore = score[i];
				j++;
			}
		}
		if (j == 0)
			m_stResult.NumOfFound = 0;
		//std::sort(m_stResult.stResult, m_stResult.stResult + MODELS_MAX_OCCURRENCES, m_fn_Cmp);
		
		//sprintf_s(strName, ("D:\\Image\\%s\\%02d%02d%02d\\%02d%02d%02d_%02d%02d%02d_AlignResult_P.jpg"), m_strLotName,systime.wYear, systime.wMonth, systime.wDay, systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, systime.wSecond);
		sprintf_s(strName, ("D:\\Image\\%02d%02d%02d\\%02d%02d%02d_%02d%02d%02d_AlignResult_P.jpg"), systime.wYear, systime.wMonth, systime.wDay, systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, systime.wSecond);
		cv::imwrite(strName, matlog);
	}
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_DestroyPatternMatching()
@brief	PatternMatching 객체 해제
@return	void
@param	void
@remark
 - PatternMatching 객체 해제
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  12:28
*/
void CMilAlign::m_fn_DestroyPatternMatching()
{
	if (m_nLicenseType == 0)
		return;
	if (Pat_Result > 0)
	{
		MpatFree(Pat_Result);
		Pat_Result = 0;
	}
	if (Pat_Context > 0)
	{
		MpatFree(Pat_Context);
		Pat_Context = 0;
	}
}
//---------------------------------------------------------------------------
/**
@fn		void CMilAlign::m_fn_DestroyMIL()
@brief	MIL 객체 해제
@return	void
@param	void
@remark
 - MIL 객체를 해제한다.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/7  12:29
*/
void CMilAlign::m_fn_DestroyMIL()
{
// 	if (m_nLicenseType == 0)
// 		return;
	if (Obj > 0)
	{
		MbufFree(Obj);
		Obj = 0;
	}
	if (Mark > 0)
	{
		MbufFree(Mark);
		Mark = 0;
	}
	if (Default_system > 0)
	{
		MsysFree(Default_system);
		Default_system = 0;
	}
	if (MilApplication > 0)
	{
		MappFree(MilApplication);
		MilApplication = 0;
	}
}

//---------------------------------------------------------------------------
/**
@fn 	 void CMilAlign::m_fn_GetMarkPosition(void* param)
@brief	 Mark 검색 결과 출력.
@return void
@param	 void* param : result structure pointer
@remark
 - Mark 검색 결과를 결과 파라메터에 반환.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/2/10  13:39
*/
void CMilAlign::m_fn_GetMarkPosition(void* param)
{
	if (param != nullptr)
	{
		memcpy(param, &m_stResult, sizeof(ST_ALIGN_RESULT));
	}
}

//---------------------------------------------------------------------------
/**
@fn		double CMilAlign::m_fn_GetTwoLineAngle(PointD pnt1, PointD pnt2)
@brief	두 직선 사이의 각도
@return	Degree Angle
@param	PointD pnt1 : 1번 포인트
@param	PointD pnt2 : 2번 포인트
@remark
 - 벡터 내적으로 구함.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/3/3  16:47
*/
double CMilAlign::m_fn_GetTwoLineAngle(PointD pnt1, PointD pnt2)
{
	double dC = (pnt1.x * pnt2.y - pnt1.y * pnt2.x);
	double dA = std::sqrt(std::pow(pnt1.x, 2) + std::pow(pnt1.y, 2));
	double dB = std::sqrt(std::pow(pnt2.x, 2) + std::pow(pnt2.y, 2));

	double dAngle = std::asin(dC / (dA * dB));

	return dAngle * 180.0 / CV_PI;
}

//---------------------------------------------------------------------------
/**
@fn     void CMilAlign::m_fn_cvPatternMatching(char* Img, int ImgWidth, int ImgHeight, char* Mark, int MarkWidth, int MarkHeight, PointD* RetPnt, double* RetValue)
@brief	OpenCV Pattern Matching
@return	void
@param	char* Img		 : 대상 이미지
@param  int ImgWidth     : 대상 이미지 너비
@param  int ImgHeight    : 대상 이미지 폭
@param  char* Mark       : 마크 이미지
@param  int MarkWidth    : 마크 이미지 너비
@param  int MarkHeight   : 마크 이미지 폭
@param  PointD* RetPnt   : 매칭 결과 중심 위치
@param  double* RetValue : 매칭 결과 퍼센트
@remark	
 - Gray Image 한정.
 - OpenCV 패턴 매칭.
 - 검색 결과 이미지 중심 위치 및 매칭 퍼센트 반환함.
@author	선경규(Kyeong Kyu - Seon)
@date	2020/5/6  19:06
*/
void CMilAlign::m_fn_cvPatternMatching(char* Img, int ImgWidth, int ImgHeight, char* Mark, int MarkWidth, int MarkHeight, PointD* RetPnt, double* RetValue)
{

	if (RetPnt != nullptr)
	{
		cv::Mat matImg = m_fn_GetMatImage(Img, ImgWidth, ImgHeight, 1);
		cv::Mat matMark = m_fn_GetMatImage(Mark, MarkWidth, MarkHeight, 1);
		cv::Mat matTemplate;
		int nMatchMethod = cv::TemplateMatchModes::TM_CCOEFF_NORMED;
		matchTemplate(matImg, matMark, matTemplate, nMatchMethod);

		*RetValue = 0.0;
		int x, y;
		float value;
		for (y = 0; y < matTemplate.rows; y++)
		{
			for (x = 0; x < matTemplate.cols; x++)
			{
				value = matTemplate.at<float>(y, x);

				if (*RetValue <= (double)value)
				{
					RetPnt->x = x;
					RetPnt->y = y;
					*RetValue = value;
				}
			}
		}
	}
}

void CMilAlign::m_fn_PinSearch(double dTargetRadius, double dSmoothness)
{
	if (Default_system == 0 || MilApplication == 0)
		return;
	if (m_nLicenseType == 0)
		return;

	//---------------------------------------------------------------------------
	MmodAlloc(Default_system, M_SHAPE_CIRCLE, M_DEFAULT, &Mod_CircleShape);
	// Post-Alloc Block for Mod Circle Shape Context
	MmodDefine(Mod_CircleShape, M_CIRCLE, M_FOREGROUND_WHITE, dTargetRadius, M_DEFAULT, M_DEFAULT, M_DEFAULT);

	// Control Block for Mod Circle Shape Context
	MmodControl(Mod_CircleShape, M_CONTEXT, M_SMOOTHNESS, dSmoothness);

	MmodAllocResult(Default_system, M_SHAPE_CIRCLE, &Mod_CircleResult);

	MmodPreprocess(Mod_CircleShape, M_DEFAULT);
	MmodFind(Mod_CircleShape, Obj, Mod_CircleResult);

	//---------------------------------------------------------------------------
	// Result
	long NumOfFound = 0;
	MmodGetResult(Mod_CircleResult, M_GENERAL, M_NUMBER + M_TYPE_LONG, &NumOfFound);

	double posX  [MODELS_MAX_OCCURRENCES] = { 0, };
	double posY  [MODELS_MAX_OCCURRENCES] = { 0, };
	double radius[MODELS_MAX_OCCURRENCES] = { 0, };
	double score [MODELS_MAX_OCCURRENCES] = { 0, };
	if (NumOfFound > 0)
	{
		MmodGetResult(Mod_CircleResult, M_ALL, M_CENTER_X, posX);
		MmodGetResult(Mod_CircleResult, M_ALL, M_CENTER_Y, posY);
		MmodGetResult(Mod_CircleResult, M_ALL, M_RADIUS  , radius);
		MmodGetResult(Mod_CircleResult, M_ALL, M_SCORE   , score);
	}
	//---------------------------------------------------------------------------
	// Free
	MmodFree(Mod_CircleResult);
	Mod_CircleResult = 0;
	
	MmodFree(Mod_CircleShape);
	Mod_CircleShape = 0;

	memset(&m_stResult, 0x00, sizeof(m_stResult));
	m_stResult.NumOfFound = NumOfFound;
	int j = 0; 
	for (int i = 0; i < NumOfFound; i++)
	{
		if(j < MODELS_MAX_OCCURRENCES)
		{
			m_stResult.stResult[j].nX      = posX  [i];
			m_stResult.stResult[j].nY      = posY  [i];
			m_stResult.stResult[j].nWidth  = radius[i];
			m_stResult.stResult[j].nHeight = radius[i];
			m_stResult.stResult[j].dScore  = score [i];
			j++;
		}
	}
	
}

void* CMilAlign::m_fn_GetModelEdge(char* data, int width, int height, void* param, bool bPreProc)
{
	if (m_nLicenseType == 0)
		return nullptr;

	m_fn_SetMark(data, width, height, 1, bPreProc);
	m_fn_SetParamModelFinder(param);
	m_fn_PrepareModelFinder();

	MIL_ID MarkResult = M_NULL;
	MbufAlloc2d(Default_system, m_matMark.cols, m_matMark.rows, 8 + M_UNSIGNED, M_IMAGE + M_PROC + M_DISP, &MarkResult);
	MbufClear(MarkResult, M_RGB888(0,0,0));

	cv::Mat matTemp = cv::Mat::zeros(cv::Size(m_matMark.cols, m_matMark.rows), CV_8UC1);
	if (!m_matEdge.empty())
		m_matEdge.release();
	m_matEdge = cv::Mat::zeros(cv::Size(m_matMark.cols, m_matMark.rows), CV_8UC3);

	MmodDraw(M_DEFAULT, Mod_Context, MarkResult, M_DRAW_EDGES, M_ALL, M_DEFAULT);
	MbufGet2d(MarkResult, 0, 0, matTemp.cols, matTemp.rows, matTemp.data);
	MbufFree(MarkResult);
	MarkResult = 0;

	//imshow("Edge_FromMIL", matTemp);

	m_fn_DestroyModelFinder();
 	//m_fn_DestroyMIL();

	cvtColor(m_matMark, m_matEdge, cv::ColorConversionCodes::COLOR_GRAY2BGR);

	for (int j = 0; j < m_matEdge.rows; j++)
	{
		for (int i = 0; i < m_matEdge.cols; i++)
		{
			if (matTemp.data[j * matTemp.cols + i] == 255)
			{
				m_matEdge.at<cv::Vec3b>(j, i)[0] = 0;
				m_matEdge.at<cv::Vec3b>(j, i)[1] = 0;
				m_matEdge.at<cv::Vec3b>(j, i)[2] = 255;
			}
		}
	}

	matTemp.release();
	return m_matEdge.data;
}

void CMilAlign::m_fn_ReleaseEdge()
{
	m_matEdge.release();
}

 bool CMilAlign::m_fn_Cmp(const ST_RESULT& st1, const ST_RESULT& st2, bool bAscending)
{
	 bool bRtn = false;
	 
	 if(bAscending)
		bRtn = st1.dScore < st2.dScore; // 오름차순
	 else
		bRtn = st1.dScore > st2.dScore; // 내림차순

	 return bRtn;
}
