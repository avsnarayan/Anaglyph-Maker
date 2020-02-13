
namespace MAIN.Model
{
    /// <summary>
    /// A static class that stores anaglyph filters.
    /// </summary>
    static class AnaglyphOptions
    {
        /// <summary>
        /// True anaglyph filter.
        /// </summary>
        public static float[] _true = { 0.114f, 0.587f, 0.299f, 0.0f,
            0.114f, 0.587f, 0.299f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f };

        /// <summary>
        /// Gray anagliph filter.
        /// </summary>
        public static float[] _gray = { 0.114f, 0.587f, 0.299f, 0.0f,
            0.114f, 0.587f, 0.299f, 0.0f,
             0.114f, 0.587f, 0.299f, 0.0f };

        /// <summary>
        /// Color anaglif filter.
        /// </summary>
        public static float[] _color = {  0.0f, 0.0f, 1.0f, 0.0f,
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f };

        /// <summary>
        /// Half color anaglif filter.
        /// </summary>
        public static float[] _halfColor = {  0.114f, 0.587f, 0.299f, 0.0f,
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f };

        /// <summary>
        /// Optimized anaglif filter.
        /// </summary>
        public static float[] _optimized = { 0.3f, 0.7f, 0.0f, 0.0f,
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f };
    }
}
