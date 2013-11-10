using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mbrot
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(800, 800);
            var watch = Stopwatch.StartNew();
            Mandel(2000, 2000);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            this.Text += " - " + elapsedMs + " milliseconds";
        }

        private const int MaxIterations = 1000;
        private const double MinRe = -2.0;
        private const double MaxRe = 2.0;
        private const double MinIm = -2.0;
        private const double MaxIm = 2.0;
        private void Mandel(int imageWidth, int imageHeight)
        {

            double XScale = (MaxRe - MinRe) / (imageWidth);
            double YScale = (MaxIm - MinIm) / (imageHeight);

            Bitmap bitmap = new Bitmap(imageWidth, imageHeight);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            IntPtr ptr = bitmapData.Scan0;
            int pixels = bitmap.Width * bitmap.Height;

            Int32[] rgbValues = new Int32[pixels];

            for (int x = 0; x < imageWidth; x++)
            {
                int index = x;

                for (int y = 0; y < imageHeight; y++)
                {
                    double c_im = MaxIm - y * YScale;
                    double c_re = MinRe + x * XScale;
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
            }

            Marshal.Copy(rgbValues, 0, ptr, pixels);
            bitmap.UnlockBits(bitmapData);

            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.BackgroundImage = bitmap;

            bitmap.Save("fractal.png", ImageFormat.Png);
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