using MAIN.Enums;
using MAIN.Static;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace MAIN.Model
{
    /// <summary>
    /// An event returning a generated anaglyph.
    /// </summary>
    public class BitmapEventArgs : EventArgs
    {
        public Bitmap anaglyphImage { get; set; }
    }

    /// <summary>
    /// Class responsible for generating anaglyph from two provided images.
    /// </summary>
    unsafe class AnaglyphCreation
    {
        /// <summary>
        /// List of start and beginning points of the image from which and to which the algorithm is to create anaglyph.
        /// </summary>
        private List<int> coords;

        /// <summary>
        /// An array that holds the filter for creating anaglyph.
        /// </summary>
        private float[] anaglyphFilters;

        /// <summary>
        /// A temporary array that stores the left image pixels.
        /// </summary>
        private float[] tmpLeftImagePixelArray;

        /// <summary>
        /// A list of threads that execute the algorithm.
        /// </summary>
        private List<Thread> threads;

        /// <summary>
        /// The number of pixels per one thread.
        /// </summary>
        private int pixelsForThread;

        /// <summary>
        /// A function calling the dll algorithm written in C ++.
        /// </summary>
        /// <param name="leftImage"> Left image pixel array. </param>
        /// <param name="startStop"> The array has two values. Start - what index the algorithm should work from, End - which index the algorithm should work from. </param>
        /// <param name="filters"> An array that holds the filter for creating anaglyph. </param>
        /// <param name="rightImage"> Right image pixel array. </param>
        [DllImport("DLL_C.dll", EntryPoint = "AnaglyphAlgorithm")]
        public static extern void AnaglyphAlgorithm(float[] leftImage, int[] startStop, float[] filters, float[] rightImage);

        /// <summary>
        /// A function calling the dll algorithm written in assembler.
        /// </summary>
        /// <param name="leftImage"> Left image pixel array. </param>
        /// <param name="startStop"> The array has two values. Start - what index the algorithm should work from, End - which index the algorithm should work from. </param>
        /// <param name="filters"> An array that holds the filter for creating anaglyph. </param>
        /// <param name="rightImage"> Right image pixel array. </param>
        [DllImport("DLL_ASM.dll", EntryPoint = "AnaglyphAlgorithmAsm")]
        public static extern void AnaglyphAlgorithmAsm(float[] leftImage, int[] startStop, float[] filters, float[] rightImage);

        /// <summary>
        /// Parameterless AnaglyphCreation constructor. Creates a list of threads and a list of start and end indexes.
        /// </summary>
        public AnaglyphCreation()
        {
            this.threads = new List<Thread>(); // create threads list
            this.coords = new List<int>(); // create list that stores start and end indexes
        }

        /// <summary>
        /// A method that sets start and end indexes for threads that will support the anaglyph generation algorithm.
        /// </summary>
        /// <param name="threadAmount"> Number of threads selected by the user. </param>
        private void PrepareThreads(int threadAmount)
        {
            int[] coords = { 0, pixelsForThread }; // coordinates for the first thread
            for (int i = 0; i < threadAmount; ++i) // loop that fills the index table with the appropriate start and end values
            {
                this.coords.Add(coords[0]); // adding the start index value to the list of indexes
                coords[0] += pixelsForThread; // setting the start index for the next thread
                this.coords.Add(coords[1]); // adding the end index value to the list of indexes
                coords[1] += pixelsForThread; // setting the end index for the next thread
            }
        }


        /// <summary>
        /// A method that creates anaglyph from the left and right image.
        /// </summary>
        /// <param name="dllVersion"> The dll version selected by the user. </param>
        /// <param name="anaglyphVersion"> User selection of anaglyph version. </param>
        /// <param name="leftImageInformation"> An instance of the class that stores information about the left image. </param>
        /// <param name="rightImageInformation"> An instance of the class that stores information about the right image. </param>
        /// <param name="threadAmount"> Number of threads selected by the user. </param>
        public void CreateAnaglyph(DllVersion dllVersion, AnaglyphVersion anaglyphVersion, ImageInformation leftImageInformation, ImageInformation rightImageInformation, int threadAmount)
        {
            int amountOfPixels = leftImageInformation.imagePixelArray.Length / 4; // the number of pixels that will be modified
            pixelsForThread = amountOfPixels / threadAmount; // the number of pixels to modify per thread

            if (pixelsForThread * threadAmount != leftImageInformation.imagePixelArray.Length / 4) // condition checking if the same number of pixels can be processed in each thread (different number of pixels)
            {
                int delta = amountOfPixels - (pixelsForThread * threadAmount); // number of pixels remaining
                PrepareThreads(threadAmount - 1); // prepare tables for the number of threads selected by the user minus one

                int start = pixelsForThread * (threadAmount - 1); // start index for a thread that modifies the rest of the pixels

                this.coords.Add(start); // start index for the last thread
                this.coords.Add(leftImageInformation.imagePixelArray.Length / 4); // end index for the last thread
            }
            else // the same number of pixels in each thread
            {
                PrepareThreads(threadAmount); // prepare a table of coordinates
            }

            anaglyphFilters = new float[12]; // creating an array that stores an anaglyph filter
            switch (anaglyphVersion) // choosing the right anaglyph filter depending on the user's choice
            {
                case AnaglyphVersion.TRUE_ANAGLYPH:
                    anaglyphFilters = AnaglyphOptions._true;
                    break;
                case AnaglyphVersion.GRAY_ANAGLYPH:
                    anaglyphFilters = AnaglyphOptions._gray;
                    break;
                case AnaglyphVersion.COLOR_ANAGLYPH:
                    anaglyphFilters = AnaglyphOptions._color;
                    break;
                case AnaglyphVersion.HALF_COLOR_ANAGLYPH:
                    anaglyphFilters = AnaglyphOptions._halfColor;
                    break;
                case AnaglyphVersion.OPTIMIZED_ANAGLYPH:
                    anaglyphFilters = AnaglyphOptions._optimized;
                    break;
            }

            tmpLeftImagePixelArray = new float[leftImageInformation.imagePixelArray.Length]; // creating a temporary array storing the pixels of the left image
            for (int i = 0; i < tmpLeftImagePixelArray.Length; i++) // loop that copies the pixel values of the left image into a temporary table
            {
                tmpLeftImagePixelArray[i] = leftImageInformation.imagePixelArray[i]; // copy the pixel color value from the left image to a temporary table
            }

            switch (dllVersion) // choice of dll depending on the user's choice
            {
                case DllVersion.ASM_DLL: // assembler dll choice
                    for (int i = 0; i < this.coords.Count; i += 2) // loop calling as many times as there are coordinates (start and end) for each thread
                    {
                        int[] imageCoords = { this.coords[i] * 4, this.coords[i + 1] * 4 }; // get start and start index for the current thread
                        var th = new Thread(() => // thread creation
                        {
                            AnaglyphAlgorithmAsm(leftImageInformation.imagePixelArray, imageCoords, anaglyphFilters, rightImageInformation.imagePixelArray);
                        });
                        this.threads.Add(th); // adding a newly created thread to the list of threads
                    }
                    break;
                case DllVersion.CPP_DLL: // C++ dll choice
                    for (int i = 0; i < this.coords.Count; i += 2) // loop calling as many times as there are coordinates(start and end) for each thread
                    {
                        int[] imageCoords = { this.coords[i] * 4, this.coords[i + 1] * 4 }; // get start and start index for the current thread
                        var th = new Thread(() => // thread creation
                        {
                            AnaglyphAlgorithm(leftImageInformation.imagePixelArray, imageCoords, anaglyphFilters, rightImageInformation.imagePixelArray);
                        });
                        this.threads.Add(th); // adding a newly created thread to the list of threads
                    }
                    break;
            }

            TimerClock.StartTimer(); // counting the time it takes for threads to finish
            foreach (Thread th in threads) // start all threads
            {
                th.Start();
            }

            foreach (Thread th in threads) // waiting for the end of each thread
            {
                th.Join();
            }
            TimerClock.StopTimer(); // stop counting time

            threads.Clear(); // clear the list of threads
            coords.Clear(); // clearing the list of start and end indexes
            leftImageInformation.finishConversion();  // creating an image based on returned pixels

            for (int i = 0; i < tmpLeftImagePixelArray.Length; i++) // copying the pixel color values from the previously created table to the table storing the pixel values of the left image
            {
                leftImageInformation.imagePixelArray[i] = tmpLeftImagePixelArray[i];
            }

            EventImageFinished(leftImageInformation.imageBitmap); // reporting an event about the end of anaglyph creation
        }

        public event EventHandler<BitmapEventArgs> AnaglyphCreated; // anaglyph creation event

        /// <summary>
        /// Method reporting completion of anaglyph creation.
        /// </summary>
        /// <param name="anaglyphImage"> Anaglyph image. </param>
        protected virtual void EventImageFinished(Bitmap anaglyphImage)
        {
            AnaglyphCreated?.Invoke(this, new BitmapEventArgs() { anaglyphImage = anaglyphImage }); // anaglyph created
        }
    }
}
