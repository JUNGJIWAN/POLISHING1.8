#pragma once
#define DLLEXPORT
#include "WpfDll.h"
#include <process.h>
#include "CMilAlign.h"
#include "IncludeHeader.h"
#include "CParam.h"

CMilAlign                 gc_MilAlign;
cv::Mat	                  g_matDst;
cv::Mat                   g_matROI;
cv::Mat                   g_matROIOrigin;
cv::Rect                  g_rectROI;
std::vector<MatLabelInfo> g_vecSection;
char					  g_chLotName[1024];

DLLTYPE void libSetImage(char* data, double width, double height, double channel)
{
	// 폭, 높이 반올림 추가. [2020/5/26_10:11:58 Falcon]
	gc_MilAlign.m_fn_SetImage(data, (int)std::round(width), (int)std::round(height), (int)channel);
}

DLLTYPE void libSetMark(char* data, double width, double height, double channel)
{
	// 폭, 높이 반올림 추가. [2020/5/26_10:11:58 Falcon]
	gc_MilAlign.m_fn_SetMark(data, (int)std::round(width), (int)std::round(height), (int)channel);
}

DLLTYPE void libInit()
{
	gc_MilAlign.m_fn_InitMIL();
}

DLLTYPE int libCheckLicense()
{
	return gc_MilAlign.m_fn_CheckLicense();
}

DLLTYPE void libSetParam(void* paramModel, void* paramPattern)
{
	gc_MilAlign.m_fn_SetParamModelFinder(paramModel);
	gc_MilAlign.m_fn_SetParamPatternMatching(paramPattern);
}

DLLTYPE void libRunProcModel()
{
	gc_MilAlign.m_fn_PrepareModelFinder();
	gc_MilAlign.m_fn_RunProcModelFinder();
	gc_MilAlign.m_fn_GetResultModelFinder();
}

DLLTYPE double libRunProcModelNext(bool bIncrease)
{
	bool bModel = true;
	double dNowAcceptance = 0.0;

	dNowAcceptance = gc_MilAlign.m_fn_SetNextSearchScore(bModel, bIncrease);
	if (dNowAcceptance > 0 && dNowAcceptance < 100)
	{
		gc_MilAlign.m_fn_RunProcModelFinder();
		gc_MilAlign.m_fn_GetResultModelFinder();
	}

	return dNowAcceptance;
}

DLLTYPE void libRunProcPattern()
{
	gc_MilAlign.m_fn_PreparePatternMatching();
	gc_MilAlign.m_fn_RunProcPatternMatching();
	gc_MilAlign.m_fn_GetResultPatternMatching();
}

DLLTYPE double libRunProcPatternNext(bool bIncrease)
{
	bool bModel = false;
	double dNowAcceptance = 0.0;

	dNowAcceptance = gc_MilAlign.m_fn_SetNextSearchScore(bModel, bIncrease);
	if (dNowAcceptance > 0 && dNowAcceptance < 100)
	{
		gc_MilAlign.m_fn_RunProcPatternMatching();
		gc_MilAlign.m_fn_GetResultPatternMatching();
	}

	return dNowAcceptance;
}

DLLTYPE void libGetResult(void* paramResult)
{
	gc_MilAlign.m_fn_GetMarkPosition(paramResult);
}

DLLTYPE void libDestroyModel()
{
	gc_MilAlign.m_fn_DestroyModelFinder();
}

DLLTYPE void libDestroyPattern()
{
	gc_MilAlign.m_fn_DestroyPatternMatching();
}

DLLTYPE void libDestroy()
{
	gc_MilAlign.m_fn_DestroyMIL();

	if (g_matDst.data != nullptr)
		g_matDst.release();
	if (g_matROI.data != nullptr)
		g_matROI.release();
	if (g_matROIOrigin.data != nullptr)
		g_matROIOrigin.release();
	g_vecSection.clear();
}

DLLTYPE void  libPinSearch(double dRadius, double dSmooth)
{
	gc_MilAlign.m_fn_PinSearch(dRadius, dSmooth);
}

DLLTYPE void* libThreshold(void* ptr, double width, double height, int channel, double threshold, double sigma)
{
	int Ch = 0;
	switch (channel)
	{
	case 1:
		Ch = CV_8UC1;
		break;
	case 3:
		Ch = CV_8UC3;
		break;
	default:
		Ch = CV_8UC1;
		break;
	}
	cv::Mat matSrc = cv::Mat::zeros(cv::Size(static_cast<int>(width), static_cast<int>(height)), Ch);
	int stride = static_cast<int>(width * channel + 3) & ~3;
	matSrc.step = stride;
	int nSize = static_cast<int>(width * height * channel);
	memcpy(matSrc.data, ptr, nSize);
	
	if (g_rectROI.width == 0)
	{
		g_rectROI.x = 0;
		g_rectROI.width = matSrc.cols;
	}
	if (g_rectROI.height == 0)
	{
		g_rectROI.y = 0;
		g_rectROI.height = matSrc.rows;
	}
	cv::Mat matROI = matSrc(g_rectROI);
	if (!g_matROIOrigin.empty())
		g_matROIOrigin.release();
	g_matROIOrigin = matROI.clone();
	try
	{
		cv::GaussianBlur(matROI, matROI, cv::Size(9, 9), sigma);
		cv::threshold(matROI, matROI, threshold, 255, cv::ThresholdTypes::THRESH_BINARY);
		if (!g_matROI.empty())
			g_matROI.release();
		g_matROI = matROI.clone();
	}
	catch (cv::Exception ex)
	{
		OutputDebugString(ex.what());
	}
	if (!g_matDst.empty())
		g_matDst.release();
	g_matDst = matSrc.clone();
	matSrc.release();
	return g_matDst.data;
}

DLLTYPE void* libCanny(void* ptr, double width, double height, int channel, double sigma, double lowth, double highth)
{
	int Ch = 0;
	switch (channel)
	{
	case 1:
		Ch = CV_8UC1;
		break;
	case 3:
		Ch = CV_8UC3;
		break;
	default:
		Ch = CV_8UC1;
		break;
	}
	cv::Mat matSrc = cv::Mat::zeros(cv::Size(static_cast<int>(width), static_cast<int>(height)), Ch);
	int stride = static_cast<int>(width * channel + 3) & ~3;
	int nSize = static_cast<int>(width * height * channel);
	matSrc.step = stride;
	memcpy(matSrc.data, ptr, nSize);

	if (g_rectROI.width == 0) 
	{ 
		g_rectROI.x			= 0;
		g_rectROI.width		= matSrc.cols; 
	}
	if (g_rectROI.height == 0)
	{
		g_rectROI.y			= 0;
		g_rectROI.height	= matSrc.rows;
	}

	cv::Mat matROI = matSrc(g_rectROI);
	if (!g_matROIOrigin.empty())
		g_matROIOrigin.release();
	g_matROIOrigin = matROI.clone();
	
	try
	{
		cv::GaussianBlur(matROI, matROI, cv::Size(9, 9), sigma);
		cv::Canny(matROI, matROI, lowth, highth);
		if (!g_matROI.empty())
			g_matROI.release();
		g_matROI = matROI.clone();
	}
	catch (cv::Exception ex)
	{
		OutputDebugString(ex.what());
	}
	if(!g_matDst.empty())
		g_matDst.release();
	g_matDst = matSrc.clone();
	matSrc.release();
	return g_matDst.data;
}

DLLTYPE void libSetROI(int x, int y, int width, int height)
{
	g_rectROI.x = x;
	g_rectROI.y = y;
	g_rectROI.width = width;
	g_rectROI.height = height;
}

DLLTYPE void libGetLineProfile(void* data, int len)
{
	if (g_matROIOrigin.data != nullptr && data != nullptr)
	{
		double* pData = (double*)data;
		double dTemp = 0.0;
		int width = g_matROIOrigin.cols;
		
		assert(len == g_matROIOrigin.rows);
		//g_matROI
		for (int stepY = 0; stepY < g_matROIOrigin.rows; stepY++)
		{
			dTemp = 0.0;
			for (int stepX = 0; stepX < g_matROIOrigin.cols; stepX++)
			{
				dTemp += g_matROIOrigin.data[stepY * width + stepX];
			}
			// Average
			pData[stepY] = dTemp / width;
		}
	}
}
DLLTYPE int libProcMeasureLen(void* ptrParam, bool bAuto)
{
	int nRtn = 0;
	if (g_matROI.data != nullptr)
	{
		fn_getSection(g_matROI, *(ST_INSPECTION*)ptrParam, bAuto);
		nRtn = static_cast<int>(g_vecSection.size());
	}
	else
		nRtn = -1;
	return nRtn;
}
DLLTYPE void libGetMeasureLen(int* maxstartY, int* maxendY, int* cY, int idx)
{
	int nVecCount = static_cast<int>(g_vecSection.size());
	if (nVecCount > 0)
	{
		if (nVecCount > idx)
		{
			*maxstartY = g_vecSection[idx].miny;
			*maxendY   = g_vecSection[idx].maxy;
			*cY		   = g_vecSection[idx].cy;
		}
		else
		{
			*maxstartY = -2;
			*maxendY   = -2;
			*cY		   = -2;
		}
	}
	else
	{
		*maxstartY = -1;
		*maxendY   = -1;
		*cY		   = -1;
	}
}

DLLTYPE void libcvPatternMatching(char* Img, int ImgWidth, int ImgHeight, char* Mark, int MarkWidth, int MarkHeight, PointD* RetPnt, double* RetValue)
{
	gc_MilAlign.m_fn_cvPatternMatching(Img, ImgWidth, ImgHeight, Mark, MarkWidth, MarkHeight, RetPnt, RetValue);
}

DLLTYPE void* libGetModelEdge(char* data, int width, int height, void* param)
{
	return gc_MilAlign.m_fn_GetModelEdge(data, width, height, param);
}

DLLTYPE void libReleaseEdge()
{
	gc_MilAlign.m_fn_ReleaseEdge();
}

DLLTYPE void  libSetLotName(char* strLotName, int Length)
{
	if (strLotName != nullptr)
	{
		memcpy(g_chLotName		       , strLotName, sizeof(strLotName[0]) * Length);
		gc_MilAlign.m_strLotName = g_chLotName;
	}
}
void fn_getSection(cv::Mat matSrc, ST_INSPECTION stParam, bool bAuto)
{
	int width = matSrc.cols;
	int height = matSrc.rows;
	char strName[1024] = { 0, };
	char strText[1024] = { 0, };
	cv::Point pntText;
	SYSTEMTIME systime;
	GetLocalTime(&systime);
	
	cv::Mat matDst;
	if (matSrc.channels() == 1)
	{
		std::vector<MatLabelInfo> vecObj;
		std::vector<MatLabelInfo> vecMerge;
		MatLabelInfo stLabel;
		
		cv::Mat mask = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3), cv::Point(1, 1));
		
		for (int i = 0; i < stParam.DLLDiliteCnt; i++)
		{
			cv::dilate(matSrc, matDst, mask, cv::Point(-1, -1), 3);
		}

		int nObjCnt = fn_Labeling(matDst, vecObj);
		
		for (int i = 0; i < nObjCnt; i++)
		{
			if (vecObj[i].maxx - vecObj[i].minx > matSrc.cols * stParam.DLLLenRate)
			{
				vecMerge.push_back(vecObj[i]);
			}
		}

		int nWidth = 0;
		int nHeight = 0;
		int nPixelCount = 0;
		int nGap = 0;
		double dParamAreaRate = 0.8;
		for (int i = 0; i < vecMerge.size(); i++)
		{
			nWidth = vecMerge[i].maxx - vecMerge[i].minx;
			nHeight = vecMerge[i].maxy - vecMerge[i].miny;
			nPixelCount = (int)vecMerge[i].pixels.size();

			if ((nWidth / (double)nHeight <= stParam.DLLVHRate) && (nPixelCount / (double)(nWidth * nHeight) <= dParamAreaRate))
			{
				vecMerge.erase(vecMerge.begin() + i);
			}
		}

		//cvtColor(matSrc, matSrc, cv::ColorConversionCodes::COLOR_GRAY2BGR);
		cvtColor(g_matDst, matSrc, cv::ColorConversionCodes::COLOR_GRAY2BGR);

		// Draw Blob
		for (int i = 0; i < vecObj.size(); i++)
		{
			//rectangle(matSrc, cv::Rect(cv::Point(vecObj[i].minx, vecObj[i].miny), cv::Point(vecObj[i].maxx, vecObj[i].maxy)), cv::Scalar(255, 0, 0));
			rectangle(matSrc, cv::Rect(cv::Point(g_rectROI.x + vecObj[i].minx, g_rectROI.y + vecObj[i].miny), cv::Point(g_rectROI.x + vecObj[i].maxx, g_rectROI.y + vecObj[i].maxy)), cv::Scalar(255, 0, 0));
		}
		// Draw Filtered Blob
		for (int i = 0; i < vecMerge.size(); i++)
		{
			//rectangle(matSrc, cv::Rect(cv::Point(vecMerge[i].minx, vecMerge[i].miny), cv::Point(vecMerge[i].maxx, vecMerge[i].maxy)), cv::Scalar(0, 255, 0));
			rectangle(matSrc, cv::Rect(cv::Point(g_rectROI.x + vecMerge[i].minx, g_rectROI.y + vecMerge[i].miny), cv::Point(g_rectROI.x + vecMerge[i].maxx, g_rectROI.y + vecMerge[i].maxy)), cv::Scalar(0, 255, 0));
		}


		for (int i = 0; i < vecMerge.size(); i++)
		{
			//line(matSrc, cv::Point(vecMerge[i].minx, vecMerge[i].cy), cv::Point(vecMerge[i].maxx, vecMerge[i].cy), cv::Scalar(0, 0, 255));
			line(matSrc, cv::Point(g_rectROI.x + vecMerge[i].minx, g_rectROI.y + vecMerge[i].cy), cv::Point(g_rectROI.x + vecMerge[i].maxx, g_rectROI.y + vecMerge[i].cy), cv::Scalar(0, 0, 255));

			if (i > 0)
			{
				nGap = vecMerge[i].cy - vecMerge[i - 1].cy;
				pntText.x = g_rectROI.x + vecMerge[i].minx;
				pntText.y = g_rectROI.y + vecMerge[i].cy - (int)(nGap / 2.0) - 10;
				sprintf_s(strText, "Index:%d", i);
				cv::putText(matSrc, strText, pntText, cv::HersheyFonts::FONT_HERSHEY_SIMPLEX, 0.5, cv::Scalar(0, 0, 255));
				pntText.y = g_rectROI.y + vecMerge[i].cy - (int)(nGap / 2.0) + 10;
				sprintf_s(strText, "YPos:%dpx", nGap);
				cv::putText(matSrc, strText, pntText, cv::HersheyFonts::FONT_HERSHEY_SIMPLEX, 0.5, cv::Scalar(0, 0, 255));
			}
		}

		if(bAuto)
			sprintf_s(strName, "D:\\Image\\%s\\%02d%02d%02d\\%02d%02d%02d_%02d%02d%02d_EPDResult.jpg", g_chLotName, systime.wYear, systime.wMonth, systime.wDay, systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, systime.wSecond);
		else
			sprintf_s(strName, "D:\\Image\\%02d%02d%02d\\%02d%02d%02d_%02d%02d%02d_EPDResult.jpg", systime.wYear, systime.wMonth, systime.wDay, systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, systime.wSecond);

		//imshow("Debug", matSrc);
		//g_matDst
		//cv::imwrite(strName, matSrc);
		cv::imwrite(strName, matSrc);
		g_vecSection = vecMerge;
	}
}

int fn_Labeling(cv::Mat matSrc, std::vector<MatLabelInfo>& labels)
{
	int nW = matSrc.cols;
	int nH = matSrc.rows;

	int** pMap = new int* [nH];
	for (int y = 0; y < nH; y++)
	{
		pMap[y] = new int[nW];
		memset(pMap[y], 0, sizeof(pMap[y][0]) * nW);
	}

	const int MAX_LABEL = 10000;
	int eq_tbl[MAX_LABEL][2] = { {0,}, };

	register int nStepX, nStepY;
	int label = 0, maxl, minl, min_eq, max_eq;

	for (nStepY = 1; nStepY < nH; nStepY++)
	{
		for (nStepX = 1; nStepX < nW; nStepX++)
		{
			if (matSrc.data[nStepY * nW + nStepX] == 255)
			{
				if ((pMap[nStepY - 1][nStepX] != 0) && (pMap[nStepY][nStepX - 1] != 0))
				{
					if (pMap[nStepY - 1][nStepX] == pMap[nStepY][nStepX - 1])
					{
						pMap[nStepY][nStepX] = pMap[nStepY - 1][nStepX];
					}
					else
					{
						maxl = __max(pMap[nStepY - 1][nStepX], pMap[nStepY][nStepX - 1]);
						minl = __min(pMap[nStepY - 1][nStepX], pMap[nStepY][nStepX - 1]);

						pMap[nStepY][nStepX] = minl;

						min_eq = __min(eq_tbl[maxl][1], eq_tbl[minl][1]);
						max_eq = __max(eq_tbl[maxl][1], eq_tbl[minl][1]);

						eq_tbl[eq_tbl[max_eq][1]][1] = min_eq;
					}
				}
				else if (pMap[nStepY - 1][nStepX] != 0)
				{
					pMap[nStepY][nStepX] = pMap[nStepY - 1][nStepX];
				}
				else if (pMap[nStepY][nStepX - 1] != 0)
				{
					pMap[nStepY][nStepX] = pMap[nStepY][nStepX - 1];
				}
				else
				{
					label++;
					pMap[nStepY][nStepX] = label;
					eq_tbl[label][0] = label;
					eq_tbl[label][1] = label;
				}
			}
		}
	}
	int temp;
	for (nStepX = 1; nStepX < label; nStepX++)
	{
		temp = eq_tbl[nStepX][1];
		if (temp != eq_tbl[nStepX][0])
		{
			eq_tbl[nStepX][1] = eq_tbl[temp][1];
		}
	}

	int* hash = new int[label + 1];
	memset(hash, 0, sizeof(int) * (label + 1));

	for (nStepX = 1; nStepX <= label; nStepX++)
	{
		hash[eq_tbl[nStepX][1]] = eq_tbl[nStepX][1];
	}

	int label_cnt = 1;
	for (nStepX = 1; nStepX <= label; nStepX++)
	{
		if (hash[nStepX] != 0)
		{
			hash[nStepX] = label_cnt++;
		}
	}

	for (nStepX = 1; nStepX <= label; nStepX++)
	{
		eq_tbl[nStepX][1] = hash[eq_tbl[nStepX][1]];
	}

	delete[] hash;

	int** pDst = new int* [nH];
	for (nStepY = 0; nStepY < nH; nStepY++)
	{
		pDst[nStepY] = new int[nW];
		memset(pDst[nStepY], 0, sizeof(pDst[nStepY][0]) * nW);
	}
	
	int nIdx;
	for (nStepY = 1; nStepY < nH; nStepY++)
	{
		for (nStepX = 1; nStepX < nW; nStepX++)
		{
			if (pMap[nStepY][nStepX] != 0)
			{
				nIdx = pMap[nStepY][nStepX];
				pDst[nStepY][nStepX] = eq_tbl[nIdx][1];
			}
		}
	}

	labels.resize(label_cnt - 1);

	MatLabelInfo* pLabel;
	for (nStepY = 1; nStepY < nH; nStepY++)
	{
		for (nStepX = 1; nStepX < nW; nStepX++)
		{
			if (pDst[nStepY][nStepX] != 0)
			{
				pLabel = &labels.at(pDst[nStepY][nStepX] - 1);

				pLabel->pixels.push_back(cv::Point(nStepX, nStepY));
				pLabel->cx += nStepX;
				pLabel->cy += nStepY;

				if (nStepX < pLabel->minx) pLabel->minx = nStepX;
				if (nStepX > pLabel->maxx) pLabel->maxx = nStepX;
				if (nStepY < pLabel->miny) pLabel->miny = nStepY;
				if (nStepY > pLabel->maxy) pLabel->maxy = nStepY;
				//ASSERT(pLabel->pixels.size() != 0);
			}
		}
	}

	for (MatLabelInfo& label : labels)
	{
		//ASSERT(label.pixels.size() != 0);
		label.cx /= static_cast<int>(label.pixels.size());
		label.cy /= static_cast<int>(label.pixels.size());
	}

	for (nStepY = 0; nStepY < nH; nStepY++)
		delete[] pMap[nStepY];
	delete[] pMap;

	for (nStepY = 0; nStepY < nH; nStepY++)
		delete[] pDst[nStepY];
	delete[] pDst;

	return (label_cnt - 1);
}