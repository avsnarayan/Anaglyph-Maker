#include "stdafx.h"
#include "Header.h"
#include <xmmintrin.h>
#include <emmintrin.h>
#include <dvec.h>

#if 1	// SIMD anaglyph algorithm

void AnaglyphAlgorithm(float* leftImage, int* coords, float* filters, float* rightImage)
{
	leftImage += coords[0]; // set the pointer to start for the leftImage array of pixels
	rightImage += coords[0]; // set the pointer to start for the rightImage array of pixels

	__m128* imageLeft = (__m128*)leftImage; // convert float pointer to __m128 pointer (leftImage array of pixels)
	__m128* imageRight = (__m128*)rightImage; // convert float pointer to __m128 pointer (rightImage array of pixels)
	__m128* filter = (__m128*)filters; // convert float pointer to __m128 pointer (array of filter values)

	float maskFirstImage[] = { 0.0f, 0.0f, 1.0f, 1.0f }; // mask for left image
	float maskSecondImage[] = { 1.0f, 1.0f, 0.0f, 0.0f }; // mask for right image
	__m128* mask0011ptr = (__m128*)maskFirstImage; // convert float values to __m128* value
	__m128* mask1100ptr = (__m128*)maskSecondImage; // convert float values to __m128* value
	__m128 mask0011 = *mask0011ptr; // get value
	__m128 mask1100 = *mask1100ptr; // get value

	__m128 filterRed = *filter; // set red filter
	filter++;
	__m128 filterBlue = *filter; // set blue filter
	filter++;
	__m128 filterGreen = *filter; // set green filter

	for (int i = coords[0]; i < coords[1]; i += pixelSize) // anaglyph conversion loop from start (coords[0]) to end (coords[1]) by 4 (pixel size rgba)
	{
		// LEFT IMAGE - RED COLOR
		__m128 RGBA_leftImage = _mm_mul_ps(*imageLeft, filterRed); // multiple left image pixel by red filter

		__m128 RRRR_R = _mm_shuffle_ps(RGBA_leftImage, RGBA_leftImage, _MM_SHUFFLE(1, 1, 1, 1)); // [r g b a] -> [r r r r]
		__m128 GGGG_R = _mm_shuffle_ps(RGBA_leftImage, RGBA_leftImage, _MM_SHUFFLE(2, 2, 2, 2)); // [r g b a] -> [g g g g]
		__m128 BBBB_R = _mm_shuffle_ps(RGBA_leftImage, RGBA_leftImage, _MM_SHUFFLE(0, 0, 0, 0)); // [r g b a] -> [b b b b]

		__m128 ANAGLYPH_R = _mm_add_ps(BBBB_R, _mm_add_ps(RRRR_R, GGGG_R)); // add [r r r r], [g g g g], [b b b b]
		ANAGLYPH_R = _mm_mul_ps(ANAGLYPH_R, mask0011); // multiple by mask [0 0 rA rA]

		// RIGHT IMAGE - BLUE COLOR
		__m128 RGBA_rightImage = _mm_mul_ps(*imageRight, filterBlue); // multiple right image pixel by blue filter

		__m128 RRRR_B = _mm_shuffle_ps(RGBA_rightImage, RGBA_rightImage, _MM_SHUFFLE(1, 1, 1, 1)); // [r g b a] -> [r r r r]
		__m128 GGGG_B = _mm_shuffle_ps(RGBA_rightImage, RGBA_rightImage, _MM_SHUFFLE(2, 2, 2, 2)); // [r g b a] -> [g g g g]
		__m128 BBBB_B = _mm_shuffle_ps(RGBA_rightImage, RGBA_rightImage, _MM_SHUFFLE(0, 0, 0, 0)); // [r g b a] -> [b b b b]

		__m128 ANAGLYPH_B = _mm_add_ps(BBBB_B, _mm_add_ps(RRRR_B, GGGG_B)); // add [r r r r], [g g g g], [b b b b]

		// RIGHT IMAGE - GREEN COLOR
		RGBA_rightImage = _mm_mul_ps(*imageRight, filterGreen); // multiple right image pixel by green filter

		__m128 RRRR_G = _mm_shuffle_ps(RGBA_rightImage, RGBA_rightImage, _MM_SHUFFLE(1, 1, 1, 1)); // [r g b a] -> [r r r r]
		__m128 GGGG_G = _mm_shuffle_ps(RGBA_rightImage, RGBA_rightImage, _MM_SHUFFLE(2, 2, 2, 2)); // [r g b a] -> [g g g g]
		__m128 BBBB_G = _mm_shuffle_ps(RGBA_rightImage, RGBA_rightImage, _MM_SHUFFLE(0, 0, 0, 0)); // [r g b a] -> [b b b b]

		__m128 ANAGLYPH_G = _mm_add_ps(BBBB_G, _mm_add_ps(RRRR_G, GGGG_G)); // add [r r r r], [g g g g], [b b b b]

		__m128 GBGB = _mm_unpackhi_ps(ANAGLYPH_B, ANAGLYPH_G);  // [b g b g]
		GBGB = _mm_mul_ps(GBGB, mask1100); // [b g b g] -> [b g 0 0]

		_mm_store_ps(leftImage, _mm_add_ps(ANAGLYPH_R, GBGB)); // [bA gA 0 0] + [0 0 rA rA] -> [bA gA rA rA] anaglyph pixel

		imageLeft++; // move pointer to next pixel (left image [*__m128])
		imageRight++; // move pointer to next pixel (right image [*__m128])
		leftImage += 4; // move pointer to next pixel (left image [*float])
		rightImage += 4; // move pointer to next pixel (right image [*float])
	}
}

#else	// sequential anaglyph algorithm

void AnaglyphAlgorithm(float* leftImage, int* coords, float* filters, float* rightImage)
{
	for (int i = coords[0]; i < coords[1]; i += pixelSize) // anaglyph conversion loop from start index (coords[0]) to end index (coords[1]) steb by pixel size (4)
	{
		float anaglyphR = leftImage[i] * filters[0] + // calculate R anaglyph value
			leftImage[i + 1] * filters[1] +
			leftImage[i + 2] * filters[2];
		float anaglyphG = rightImage[i] * filters[4] + // calculate G anaglyph value
			rightImage[i + 1] * filters[5] +
			rightImage[i + 2] * filters[6];
		float anaglyphB = rightImage[i] * filters[8] + // calculate B anaglyph value
			rightImage[i + 1] * filters[9] +
			rightImage[i + 2] * filters[10];

		leftImage[i + 2] = anaglyphR; // set anaglyph Red value to leftImage R value
		leftImage[i] = anaglyphG; // set anaglyph Green value to leftImage G value
		leftImage[i + 1] = anaglyphB; // set anaglyph Blue value to leftImage B value
	}
}
#endif