using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mbrot
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(800, 800);

            var watch = Stopwatch.StartNew();
            Mandel(800, 800);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            this.Text += " - " + elapsedMs + " milliseconds";
        }

        private const int MaxIterations = 500;
        private const double MinReal = -2.0;
        private const double MaxReal = 2.0;
        private const double MinImag = -2.0;
        private const double MaxImag = 2.0;
        private void Mandel(int imageWidth, int imageHeight)
        {
            double XScale = (MaxReal - MinReal) / (imageWidth);
            double YScale = (MaxImag - MinImag) / (imageHeight);

            Bitmap bitmap = new Bitmap(imageWidth, imageHeight);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            IntPtr ptr = bitmapData.Scan0;
            int pixels = bitmap.Width * bitmap.Height;

            Int32[] rgbValues = new Int32[pixels];

            Parallel.For(0, imageWidth, (x) =>
              {
                  int index = x;

                  for (int y = 0; y < imageHeight; y++)
                  {
                      double c_im = MaxImag - y * YScale;
                      double c_re = MinReal + x * XScale;
                      double Z_re = c_re, Z_im = c_im;

                      bool isInside = true;
                      int iterations = 0;

                      for (int n = 0; n < MaxIterations; ++n)
                      {
                          double Z_re2 = Z_re * Z_re, Z_im2 = Z_im * Z_im;

                          if (Z_re2 + Z_im2 > 4)
                          {
                              isInside = false;
                              break;
                          }

                          iterations++;

                          Z_im = 2 * Z_re * Z_im + c_im;
                          Z_re = Z_re2 - Z_im2 + c_re;
                      }

                      if (isInside)
                          unchecked
                          {
                              rgbValues[index] = (int)0xff000000;
                          }
                      else
                          rgbValues[index] = MapColor(iterations).ToArgb();

                      index += imageWidth;
                  }
              });

            Marshal.Copy(rgbValues, 0, ptr, pixels);
            bitmap.UnlockBits(bitmapData);

            this.BackgroundImage = bitmap;
            
            try
            {
                bitmap.Save("fractal.png", ImageFormat.Png);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Color MapColor(int iteration)
        {
            int R = iteration % 255;
            int G = iteration * 7919 % 255;
            int B = 210;
            return Color.FromArgb(R, G, B);
        }
    }
}