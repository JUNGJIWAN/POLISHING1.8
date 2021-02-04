#pragma once

//---------------------------------------------------------------------------
// A(x1,y1) B(x2,y2)
// y = ax + b
// a = (y2-y1)/(x2-x1)
// y = (y2-y1)/(x2-x1)x+b
// b = y-(y2-y1)/(x2-x1)x
//   = y1-(y1-y1)/(x2-x1)x1
// y = (y2-y1)/(x2-x1)x + (y1-(y2-y1)/(x2-x1)x1)
inline LINE_PARAM g_fn_GetLineParam(cv::Point2f pnt1, cv::Point2f pnt2)
{
	LINE_PARAM stLineParam;
	// 기울기가 무한대일때 처리. x절편 설정.
	if (abs(pnt1.x - pnt2.x) == 0)
	{
		stLineParam.a = 0;
		stLineParam.b = 0;
		stLineParam.x = pnt1.x;
	}
	else
	{
		stLineParam.a = (pnt2.y - pnt1.y) / (double)(pnt2.x - pnt1.x);
		stLineParam.b = pnt1.y - stLineParam.a * pnt1.x;
		stLineParam.x = 0;
		// x절편 = -(b/a)
	}

	return stLineParam;
}
//---------------------------------------------------------------------------
// 방정식 1 : y = a1x + b1
// 방정식 2 : y = a2x + b2
// 교점은 두방정식 연립.
// 교점은 (-(b1 - b2)/(a1-a2), a1(-(1-b2)/(a1-a2))+b1)
inline cv::Point2f g_fn_GetLineIntersectionPoint(LINE_PARAM param1, LINE_PARAM param2)
{
	cv::Point2f pnt;
	if (param1.x == 0)
		if (param2.x == 0)
			pnt.x = static_cast<float>((param2.b - param1.b) / (param1.a - param2.a));
		else
			pnt.x = static_cast<float>(param2.x);
	else
		pnt.x = static_cast<float>(param1.x);

	//pnt.y = param1.a * ((param2.b - 1) / (param1.a - param2.a)) + param2.b;
	pnt.y = static_cast<float>(param1.a * pnt.x + param1.b);
	return pnt;
}
//---------------------------------------------------------------------------
/**
@brief	CheckCrossLine
@return	사각형 내부 교점
@param	사각형 4점
@remark
- 사각형 4점간의 교점을 구한다.
- 비교 17번 수행
@author	선경규(Kyeong Kyu - Seon)
@date	2019/6/10  13:29
*/
inline cv::Point2f g_fn_CheckCrossLine(cv::Point2f pnt1, cv::Point2f pnt2, cv::Point2f pnt3, cv::Point2f pnt4)
{
	LINE_PARAM stLine1, stLine2;
	cv::Point2f pntReturn;
	double nMinX = 99999, nMaxX = 0, nMinY = 99999, nMaxY = 0;
	if (pnt1.x < nMinX) nMinX = pnt1.x;
	if (pnt2.x < nMinX) nMinX = pnt2.x;
	if (pnt3.x < nMinX) nMinX = pnt3.x;
	if (pnt4.x < nMinX) nMinX = pnt4.x;

	if (pnt1.x > nMaxX) nMaxX = pnt1.x;
	if (pnt2.x > nMaxX) nMaxX = pnt2.x;
	if (pnt3.x > nMaxX) nMaxX = pnt3.x;
	if (pnt4.x > nMaxX) nMaxX = pnt4.x;

	if (pnt1.y < nMinY) nMinY = pnt1.y;
	if (pnt2.y < nMinY) nMinY = pnt2.y;
	if (pnt3.y < nMinY) nMinY = pnt3.y;
	if (pnt4.y < nMinY) nMinY = pnt4.y;

	if (pnt1.y > nMaxY) nMaxY = pnt1.y;
	if (pnt2.y > nMaxY) nMaxY = pnt2.y;
	if (pnt3.y > nMaxY) nMaxY = pnt3.y;
	if (pnt4.y > nMaxY) nMaxY = pnt4.y;

	stLine1 = g_fn_GetLineParam(pnt1, pnt2);
	stLine2 = g_fn_GetLineParam(pnt2, pnt3);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt3, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt1, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt2, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt1, pnt3);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine1 = g_fn_GetLineParam(pnt2, pnt3);
	stLine2 = g_fn_GetLineParam(pnt3, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt1, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt1, pnt3);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt2, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;


	stLine1 = g_fn_GetLineParam(pnt3, pnt4);
	stLine2 = g_fn_GetLineParam(pnt1, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt2, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt1, pnt3);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine1 = g_fn_GetLineParam(pnt1, pnt4);
	stLine2 = g_fn_GetLineParam(pnt2, pnt4);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine2 = g_fn_GetLineParam(pnt1, pnt3);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	stLine1 = g_fn_GetLineParam(pnt2, pnt4);
	stLine2 = g_fn_GetLineParam(pnt1, pnt3);
	pntReturn = g_fn_GetLineIntersectionPoint(stLine1, stLine2);
	if (pntReturn.x > nMinX&& pntReturn.x < nMaxX && pntReturn.y > nMinY&& pntReturn.y < nMaxY)
		if (pntReturn != pnt1 && pntReturn != pnt2 && pntReturn != pnt3 && pntReturn != pnt4)
			return pntReturn;

	pntReturn.x = -1.0;
	pntReturn.y = -1.0;
	return pntReturn;
}

inline cv::Point2f g_fn_Get3PointAngle(cv::Point2f pnt1, cv::Point2f pnt2, cv::Point2f pnt3, cv::Point2f pnt4)
{
	LINE_PARAM stLine1, stLine2, stLine3, stLine4;
	cv::Point2f pntReturn;

	stLine1 = g_fn_GetLineParam(pnt1, pnt2);
	stLine2 = g_fn_GetLineParam(pnt3, pnt4);
	stLine3 = g_fn_GetLineParam(pnt1, pnt3);
	stLine4 = g_fn_GetLineParam(pnt2, pnt4);

	double theta1 = std::atan(stLine1.a) * 180.0 / CV_PI;
	double theta2 = std::atan(stLine2.a) * 180.0 / CV_PI;
	double theta3 = std::atan(stLine3.a) * 180.0 / CV_PI;
	double theta4 = std::atan(stLine4.a) * 180.0 / CV_PI;

	double AvgTheta = 0.0;

	if (theta1 >= 0)
		theta3 += 90;
	else
		theta3 -= 90;

	if (theta2 >= 0)
		theta4 += 90;
	else
		theta4 -= 90;

	AvgTheta += theta1;
	AvgTheta += theta2;
	AvgTheta += theta3;
	AvgTheta += theta4;

	AvgTheta /= 4.0;

	double posX = (pnt1.x + pnt2.x + pnt3.x + pnt4.x) / 4.0;
	double posY = (pnt1.y + pnt2.y + pnt3.y + pnt4.y) / 4.0;

	return cv::Point2f(0, 0);
}