using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MAIN.Model
{
    /// <summary>
    /// The class that stores the structure of the loaded bitmap.
    /// </summary>
    class ImageInformation
    {
        /// <summary>
        /// Loaded image in the form of a bitmap by the user.
        /// </summary>
        public Bitmap imageBitmap;

        /// <summary>
        /// Image width.
        /// </summary>
        public int imageSizeX;

        /// <summary>
        /// Picture height.
        /// </summary>
        public int imageSizeY;

        /// <summary>
        /// Image in the form of a byte array.
        /// </summary>
        public byte[] imageByteArray;

        /// <summary>
        /// An array that stores image pixel values.
        /// </summary>
        public float[] imagePixelArray;

        /// <summary>
        /// Argumentless constructor.
        /// </summary>
        public ImageInformation()
        {
            this.imageSizeX = 0; // set imageSizeX on 0
            this.imageSizeY = 0; // set imageSizeY on 0
        }

        /// <summary>
        /// One parameter constructor.
        /// </summary>
        /// <param name="bmp"> Image loaded by the user in the form of a bitmap. </param>
        public ImageInformation(Bitmap bmp)
        {
            imageBitmap = new Bitmap(bmp); // create an instance of the Bitmap class

            this.imageSizeX = imageBitmap.Width; // assigning image width in pixels
            this.imageSizeY = imageBitmap.Height; // assigning image height in pixels

            this.ConvertBitmapTo32bit(); // bitmap conversion to 32-bit
            this.CreateImageByteArray(); // convert bitmap to byte array
            this.CreateImagePixelArray(); // create an array containing bitmap pixel colors
        }

        /// <summary>
        /// The method converts the bitmap to a 32-bit version.
        /// </summary>
        private void ConvertBitmapTo32bit()
        {
            Bitmap tmpBmp = new Bitmap(this.imageBitmap.Width, this.imageBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb); // create a temporary empty bitmap of the given size and width in 32-bit mode
            Graphics gr = Graphics.FromImage(tmpBmp); // transforming a temporarily created bitmap into a Graphics object
            gr.DrawImage(this.imageBitmap, new Rectangle(0, 0, this.imageBitmap.Width, this.imageBitmap.Height)); // copy the original image into the created bitmap
            this.imageBitmap = tmpBmp; // replace the original bitmap with a 32-bit bitmap
        }

        /// <summary>
        /// A method that converts an image into bytes to an array
        /// </summary>
        private void CreateImageByteArray()
        {
            MemoryStream memoryStream = new MemoryStream(); // create a MemoryStream object
            imageBitmap.Save(memoryStream, ImageFormat.Bmp); // saving the bitmap to memorySteam
            this.imageByteArray = memoryStream.ToArray(); // replace bitmap with byte array and assign to array
        }

        /// <summary>
        /// A method that creates an array containing bitmap pixel values.
        /// </summary>
        public void CreateImagePixelArray()
        {
            imagePixelArray = new float[imageByteArray.Length - 54]; // creating an array to store pixel values
            for (int i = 0; i < imagePixelArray.Length; i++) // copying the corresponding values from the byte array to the pixel array
            {
                imagePixelArray[i] = imageByteArray[i + 54];
            }
        }

        /// <summary>
        /// A method that transfers processed pixel values to the byte array, and then creates a bitmap from the newly received array.
        /// </summary>
        public void finishConversion()
        {
            for (int i = 0; i < imagePixelArray.Length; i++) // copy pixel values to the byte array
            {
                imageByteArray[i + 54] = (byte)imagePixelArray[i];
            }

            // create a new bitmap from a new byte array
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            this.imageBitmap = (Bitmap)tc.ConvertFrom((byte[])imageByteArray);
        }
    }
}
