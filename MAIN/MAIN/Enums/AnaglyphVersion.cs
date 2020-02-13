using System;

namespace MAIN.Enums
{
    /// <summary>
    /// Enumerated type informing about anaglyph type.
    /// </summary>
    [Flags]
    public enum AnaglyphVersion
    {
        VOID = 0, // no anaglyph type selected
        TRUE_ANAGLYPH = 1, // true anaglyph
        GRAY_ANAGLYPH = 2, // gray anaglyph
        COLOR_ANAGLYPH = 3, // anaglyph in color
        HALF_COLOR_ANAGLYPH = 4, // anaglyph in half color
        OPTIMIZED_ANAGLYPH = 5 // optimized anaglyph
    }
}
