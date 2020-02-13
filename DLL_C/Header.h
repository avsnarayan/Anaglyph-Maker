#pragma once
#ifdef DLLC_EXPORTS
#define DLLC_API __declspec(dllexport)
#else
#define DLLC_API __declspec(dllimport)
#endif

static int pixelSize = 4;

extern "C" DLLC_API void AnaglyphAlgorithm(float* leftImage, int* coords, float* filter, float* rightImage);