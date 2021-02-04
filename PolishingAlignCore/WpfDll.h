#pragma once

#ifdef DLLEXPORT
#define DLLTYPE extern "C" __declspec(dllexport)
#else
#define DLLTYPE extern "C" __declspec(dllimport)
#endif
#include "IncludeHeader.h"
#include <windows.h>
#include "CParam.h"

DLLTYPE void   libSetImage(char* data, double width, double height, double channel);
DLLTYPE void   libSetMark(char* data, double width, double height, double channel);
DLLTYPE void   libInit();
DLLTYPE int	   libCheckLicense();
DLLTYPE void   libSetParam(void* paramModel, void* paramPattern);
DLLTYPE void   libRunProcModel();
DLLTYPE double libRunProcModelNext(bool bIncrease);
DLLTYPE void   libRunProcPattern();
DLLTYPE double libRunProcPatternNext(bool bIncrease);
DLLTYPE void   libDestroyModel();
DLLTYPE void   libDestroyPattern();
DLLTYPE void   libDestroy();
DLLTYPE void   libGetResult(void* paramResult);

DLLTYPE void   libPinSearch(double dRadius, double dSmooth);
			   
DLLTYPE void   libSetROI(int x, int y, int width, int height);
DLLTYPE void*  libThreshold(void* ptr, double width, double height, int channel, double threshold, double sigma);
DLLTYPE void*  libCanny(void* ptr, double width, double height, int channel, double sigma, double lowth, double highth);
			   
DLLTYPE void   libGetLineProfile(void* data, int len);
DLLTYPE int    libProcMeasureLen(void* ptrParam, bool bAuto);
DLLTYPE void   libGetMeasureLen(int* maxstartY, int* maxendY, int* cY, int idx = 0);
			   
DLLTYPE void*  libGetModelEdge(char* data, int width, int height, void* param);
DLLTYPE void   libReleaseEdge();
			   
DLLTYPE void   libSetLotName(char* strLotName, int Length);

int fn_Labeling(cv::Mat matSrc, std::vector<MatLabelInfo>& labels);
void fn_getSection(cv::Mat matSrc, ST_INSPECTION stParam, bool bAuto);