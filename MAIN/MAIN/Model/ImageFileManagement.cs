using Microsoft.Win32;
using System;
using System.Drawing;

namespace MAIN.Model
{
    /// <summary>
    /// The class that reads the image from the file.
    /// </summary>
    class ImageFileManagement
    {
        /// <summary>
        /// instance of the ImageInformation class
        /// </summary>
        private ImageInformation imageInfotmation;

        /// <summary>
        /// The method of reading the image from the file.
        /// </summary>
        /// <returns> Loaded image as a bitmap. </returns>
        public Bitmap ReadImageFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // creating a class object from the C# library OpenFileDialog for reading image
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG"; // setting the filter, which images can be loaded into the application
            openFileDialog.RestoreDirectory = true; // setting whether after closing the window and re-opening it, it should be open on the same path as last time

            if (openFileDialog.ShowDialog() == true) // if the file selection window opens
            {
                String fileDirectory = openFileDialog.InitialDirectory + openFileDialog.FileName; // getting the path to the file

                imageInfotmation = new ImageInformation(new Bitmap(fileDirectory)); // create bitmap instance

                return new Bitmap(fileDirectory); // returning the loaded image as a bitmap
            }

            return new Bitmap(1, 1); // returning bitmap 1x1 if error
        }

        /// <summary>
        /// Method returning the ImageInformation class object
        /// </summary>
        /// <returns> ImageInformation object </returns>
        public ImageInformation GetImageInformation()
        {
            if (this.imageInfotmation != null) // if instance is created
            {
                return this.imageInfotmation; // return ImageInformation object
            }

            return new ImageInformation(); // return new ImageInformaiton object
        }
    }
}
