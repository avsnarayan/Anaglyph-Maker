using System.Diagnostics;

namespace MAIN.Static
{
    /// <summary>
    /// Static class that manages the Stopwatch object
    /// </summary>
    static class TimerClock
    {
        /// <summary>
        /// A Stopwatch class object that measures time.
        /// </summary>
        private static Stopwatch timer;

        /// <summary>
        /// A parameterless constructor creating a Stopwatch class object.
        /// </summary>
        static TimerClock()
        {
            timer = new Stopwatch();
        }

        /// <summary>
        /// The method calling time measurement.
        /// </summary>
        public static void StartTimer()
        {
            timer.Start(); // start timing
        }

        /// <summary>
        /// The method stops time measurement.
        /// </summary>
        public static void StopTimer()
        {
            timer.Stop(); // stopping time measurement
        }

        /// <summary>
        /// A method that resets objects that measure time.
        /// </summary>
        public static void ResetTimer()
        {
            timer.Reset(); // reset time measurement
        }

        /// <summary>
        /// A method that returns the measured time in milliseconds.
        /// </summary>
        /// <returns> timer.ElapsedMilliseconds - measured time in milliseconds </returns>
        public static long GetTime()
        {
            return timer.ElapsedMilliseconds; // time in milliseconds
        }
    }
}
