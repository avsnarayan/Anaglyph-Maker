using System;

namespace MAIN.Enums
{
    /// <summary>
    /// Enumerated type informing about the type of image.
    /// </summary>
    [Flags]
    public enum ImageVersion
    {
        VOID = 0, // no image selected
        LEFT_IMAGE = 1, // left image selected
        RIGHT_IMAGE = 2, // right image selected
        ANAGLYPH_IMAGE = 3 // anaglyph image selected
    }
}
