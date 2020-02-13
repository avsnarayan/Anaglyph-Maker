using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using MAIN.Model;
using MAIN.Enums;
using MAIN.Static;
using System.Threading;
using System.Windows.Input;

namespace MAIN
{
    /// <summary>
    /// Controller class.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The minimum possible number of threads to be set by the user.
        /// </summary>
        const short MIN_THREADS = 1;

        /// <summary>
        /// The maximum possible number of threads to be set by the user.
        /// </summary>
        const short MAX_THREADS = 64;

        /// <summary>
        /// ImageFileManagement instances.
        /// </summary>
        private ImageFileManagement leftImage, rightImage;

        /// <summary>
        /// ImageCreation instance.
        /// </summary>
        private AnaglyphCreation anaglyph;

        /// <summary>
        /// Enumeration type informing about anaglyph version.
        /// </summary>
        private AnaglyphVersion anaglyphVersion = AnaglyphVersion.VOID;

        /// <summary>
        /// Enumerated type informing about the dll version.
        /// </summary>
        private DllVersion dllVersion = DllVersion.VOID;

        /// <summary>
        /// Boolean variables indicating whether the left and right images are set.
        /// </summary>
        private bool leftImageSet, rightImageSet;

        /// <summary>
        /// Parameterless MainWindow class constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.InitWindow(); // application window setting
            this.InitAnaglyph(); // creation of an object responsible for creating anaglyph
            this.InitThreadsBar(); // initialize the thread number slider
        }

        /// <summary>
        /// The method that creates the object responsible for creating anaglyph.
        /// </summary>
        private void InitAnaglyph()
        {
            this.anaglyph = new AnaglyphCreation(); // creation of an object responsible for creating anaglyphs
            this.anaglyph.AnaglyphCreated += this.EventImageFinished; // creating an event informing about the completion of anaglyph creation
        }

        /// <summary>
        /// The method of setting the application window.
        /// </summary>
        private void InitWindow()
        {
            this.ResizeMode = ResizeMode.NoResize; // blocking the window's scalability

            this.buttonGenerateAnaglyph.IsEnabled = false; // blocking the button issuing the command to generate anaglyph
        }

        /// <summary>
        /// The method of setting the bar responsible for setting the number of threads.
        /// </summary>
        private void InitThreadsBar()
        {
            this.threadsBar.Minimum = MIN_THREADS; // assigning a minimum value
            this.threadsBar.Maximum = MAX_THREADS; // assigning a maximum value

            this.textThreads.Text = this.threadsBar.Value.ToString(); // rewriting the value from the bar to the variable

            int threads = Environment.ProcessorCount - 1; // determining the optimal number of threads

            this.threadsBar.Value = threads; // setting the bar to the optimal number of threads
            this.threadsLabel.Content = "Recommended number of threads: " + threads.ToString(); // setting information about the optimal number of threads for the user
        }

        /// <summary>
        /// Event supported after creating anaglyph.
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="e"> anaglyph created event </param>
        public void EventImageFinished(object sender, BitmapEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                this.SetImageOnForm(ImageVersion.ANAGLYPH_IMAGE, e.anaglyphImage); // set anaglyph image on form
                this.historyLabel.Content = TimerClock.GetTime().ToString() + " [ms]"; // set time measured on form
                e.anaglyphImage.Save("anaglyph.bmp"); // save anaglyph on project folder
                this.buttonGenerateAnaglyph.IsEnabled = true;
            });
        }

        /// <summary>
        /// A method that unlocks the anaglyph generation button when the left and right images are set.
        /// </summary>
        private void CheckImagesOnForms()
        {
            if (this.leftImageSet && this.rightImageSet) // unlock button when left and right images are set
            {
                this.buttonGenerateAnaglyph.IsEnabled = true;
            }
        }

        /// <summary>
        /// The method of setting the right image on the form.
        /// </summary>
        /// <param name="sender"> object sender </param>
        /// <param name="e"> RoutedEventArgs </param>
        private void OnClickButtonLoadRightImage(object sender, RoutedEventArgs e)
        {
            rightImage = new ImageFileManagement(); // create right image 
            Bitmap rightImageBmp = rightImage.ReadImageFromFile();

            if (rightImageBmp.Width != 1)
            {
                this.rightImageSet = true;
                this.SetImageOnForm(ImageVersion.RIGHT_IMAGE, rightImageBmp); // set right image on form
            }

            this.CheckImagesOnForms(); // check if left and right images are set
        }

        /// <summary>
        /// The method of setting the left image on the form.
        /// </summary>
        /// <param name="sender"> object sender </param>
        /// <param name="e"> RoutedEventArgs </param>
        private void OnClickButtonLoadLeftImage(object sender, RoutedEventArgs e)
        {
            leftImage = new ImageFileManagement(); // create left image instance
            Bitmap leftImageBmp = leftImage.ReadImageFromFile();

            if (leftImageBmp.Width != 1)
            {
                this.leftImageSet = true;
                this.SetImageOnForm(ImageVersion.LEFT_IMAGE, leftImageBmp); // set left image on form
            }

            this.CheckImagesOnForms(); // check if right and left images are set
        }

        /// <summary>
        /// The method of setting pictures on the form.
        /// </summary>
        /// <param name="imageVersion"> Version of image. </param>
        /// <param name="bitmapImage"> Bitmap to be set on form. </param>
        private void SetImageOnForm(ImageVersion imageVersion, Bitmap bitmapImage)
        {
            switch (imageVersion) // version of image
            {
                case ImageVersion.LEFT_IMAGE: // set left image on form
                    this.pictureBoxLeft.Source = this.BitmapToImageSource(bitmapImage);
                    this.leftImageSet = true;
                    break;
                case ImageVersion.RIGHT_IMAGE: // set right image on form
                    this.pictureBoxRight.Source = this.BitmapToImageSource(bitmapImage);
                    this.rightImageSet = true;
                    break;
                case ImageVersion.ANAGLYPH_IMAGE: // set anaglyph image on form
                    this.pictureBoxAnaglyph.Source = this.BitmapToImageSource(bitmapImage);
                    break;
            }
        }

        /// <summary>
        /// A method that converts a bitmap into an appropriate object for the form that can be displayed to the user.
        /// </summary>
        /// <param name="bitmap"> Bitmap to be converted. </param>
        /// <returns> converted bitmap </returns>
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp); // save to memory
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage; // bitmap for form
            }
        }

        /// <summary>
        /// The method updating the field informing about the number of selected threads after changing the value of the thread number slider.
        /// </summary>
        /// <param name="sender"> object sender </param>
        /// <param name="e"> , RoutedPropertyChangedEventArgs<double> </param>
        private void threadsBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int tmpThreadsAmount = 0;

            if (Int32.TryParse(textThreads.Text, out tmpThreadsAmount))
            {
                if (threadsBar.Value != tmpThreadsAmount)
                {
                    int threadsBarValue = (int)threadsBar.Value;
                    this.textThreads.Text = threadsBarValue.ToString(); // change value
                }
            }
            else
            {
                textThreads.Text = "1";
            }
        }

        /// <summary>
        /// he method updating the bar informing about the number of selected threads, after changing the value in the appropriate field.
        /// </summary>
        /// <param name="sender"> object sender </param>
        /// <param name="e"> TextChangedEventArgs </param>
        private void textThreads_TextChanged(object sender, TextChangedEventArgs e)
        {
            int tmpThreadsAmount = 0; // temporary amount of threads variable

            // set threads amount from user to text box
            if (!Int32.TryParse(textThreads.Text, out tmpThreadsAmount))
            {
                threadsBar.Value = MIN_THREADS;
            }
            else
            {
                if (tmpThreadsAmount < MIN_THREADS || tmpThreadsAmount > MAX_THREADS)
                {
                    tmpThreadsAmount = MIN_THREADS;
                }

                threadsBar.Value = tmpThreadsAmount; // set new value
            }
        }

        /// <summary>
        /// Called after pressing the button responsible for creating anaglyph. Runs the method from the object instance responsible for creating anaglyph.
        /// </summary>
        /// <param name="sender"> object sender </param>
        /// <param name="e"> RoutedEventArgs </param>
        private void OnClickButtonGenerateAnaglyph(object sender, RoutedEventArgs e)
        {
            if (radioAsm.IsChecked.Value) // assembler dll selected
            {
                this.dllVersion = DllVersion.ASM_DLL;
            }
            else if (radioC.IsChecked.Value) // C++ dll selected
            {
                this.dllVersion = DllVersion.CPP_DLL;
            }
            else // no dll selected
            {
                this.dllVersion = DllVersion.VOID;
            }

            if (radioTrue.IsChecked.Value) // true anaglyph filter selected
            {
                this.anaglyphVersion = AnaglyphVersion.TRUE_ANAGLYPH;
            }
            else if (radioGray.IsChecked.Value) // gray anaglyph filter selected
            {
                this.anaglyphVersion = AnaglyphVersion.GRAY_ANAGLYPH;
            }
            else if (radioColor.IsChecked.Value) // color anaglyph filter selected
            {
                this.anaglyphVersion = AnaglyphVersion.COLOR_ANAGLYPH;
            }
            else if (radioHalfColor.IsChecked.Value) // half color anaglyph filter selected
            {
                this.anaglyphVersion = AnaglyphVersion.HALF_COLOR_ANAGLYPH;
            }
            else if (radioOptimized.IsChecked.Value) // optimized anaglyph filter selected
            {
                this.anaglyphVersion = AnaglyphVersion.OPTIMIZED_ANAGLYPH;
            }
            else // no anaglyph filter selected 
            {
                this.anaglyphVersion = AnaglyphVersion.VOID;
            }

            if (this.anaglyphVersion == AnaglyphVersion.VOID || this.dllVersion == DllVersion.VOID) // if anaglyph filter is no selected or dll is no selected display message to user
            {
                MessageBox.Show("You must select the DLL and anaglyph type choice!", "Anaglyph Creator Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // checking if the images are the same size
            if (this.leftImage.GetImageInformation().imageSizeX != this.rightImage.GetImageInformation().imageSizeX
                || this.leftImage.GetImageInformation().imageSizeY != this.rightImage.GetImageInformation().imageSizeY)
            {
                MessageBox.Show("Left and right images must be the same size!", "Wrong image sizes", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // checking if there are dll files
            if (this.dllVersion == DllVersion.CPP_DLL && !File.Exists("DLL_C.dll"))
            {
                MessageBox.Show("No DLL found for the C++ algorithm!", "C++ DLL missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (this.dllVersion == DllVersion.ASM_DLL && !File.Exists("DLL_ASM.dll"))
            {
                MessageBox.Show("No DLL found for the assembler algorithm!", "Asm DLL missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TimerClock.ResetTimer(); // reset timer that measures algorithm time

            int threadsAmount = (int)this.threadsBar.Value; // getting the number of threads
            this.buttonGenerateAnaglyph.IsEnabled = false; // turning off the generate button while creating anaglyph

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                anaglyph.CreateAnaglyph(this.dllVersion, this.anaglyphVersion, rightImage.GetImageInformation(), leftImage.GetImageInformation(), threadsAmount);
            }).Start(); // invoke method that creates anaglyph
        }
    }
}
