#pragma once
#include <windows.h>
#include <algorithm>
#include "opencv2/opencv.hpp"
#include "mil.h"
#ifdef _WIN64
#pragma comment(lib, "MIL/mil.lib")
#pragma comment(lib, "MIL/milmod.lib")
#pragma comment(lib, "MIL/milpat.lib")
#ifdef _DEBUG
#pragma comment(lib, "opencv_world420d.lib")
#else
#pragma comment(lib, "opencv_world420.lib")
#endif
#else
#endif