using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mbrot
{
    public partial class mbrotsmooth : Form
    {
        public mbrotsmooth()
        {
            InitializeComponent();
            Load += mbrotsmooth_Load;
        }

        private List<Color> Colors;
        private int ColorCount = 256;
        private static readonly Random _Random = new Random();
        private void mbrotsmooth_Load(object sender, EventArgs e)
        {
            // Setup Colors

            Colors = new List<Color>(ColorCount);

            for (int i = 0; i < ColorCount; i++)
            {
                Colors.Add(Color.FromArgb(_Random.Next() % 0xffffff));
            }


            this.ClientSize = new Size(1000, 1000);

            var watch = Stopwatch.StartNew();
            Mandel(1000, 1000);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            this.Text += " - " + elapsedMs + " milliseconds";
        }

        // Fields needed for the faster generator
        private const int MaxIterations = 1000;
        private const double MinReal = -2.0;
        private const double MaxReal = 2.0;
        private const double MinImag = -2.0;
        private const double MaxImag = 2.0;

        private void Mandel(int imageWidth, int imageHeight)
        {
            double XScale = (MaxReal - MinReal) / (imageWidth - 1);
            double YScale = (MaxImag - MinImag) / (imageHeight - 1);

            Bitmap bitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppRgb);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            IntPtr ptr = bitmapData.Scan0;
            int pixels = bitmap.Width * bitmap.Height;

            Int32[] rgbValues = new Int32[pixels];

            double ReaC = MinReal;

            for (int x = 0; x < imageWidth; x++)
            {
                int index = x;
                double ImaC = MinImag;

                for (int y = 0; y < imageHeight; y++)
                {
                    Complex Z = Complex.Zero;
                    Complex C = new Complex(ReaC, ImaC);

                    int iterations = 0;

                    while (iterations <= MaxIterations && Z.Magnitude < 2.0)
                    {
                        Z = Z * Z + C;
                        iterations++;
                    }

                    if (iterations >= MaxIterations)
                    {
                        rgbValues[index] = 0x000000;
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Z = Z * Z + C;
                            iterations++;
                        }

                        double mu = iterations + 1 - Math.Log(Math.Log(Z.Magnitude)) / Math.Log(2);
                        rgbValues[index] = MapColor(mu).ToArgb();
                    }

                    ImaC += YScale;
                    index += imageWidth;
                }

                ReaC += XScale;
            }

            Marshal.Copy(rgbValues, 0, ptr, pixels);
            bitmap.UnlockBits(bitmapData);

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.BackgroundImage = bitmap;

            try
            {
                bitmap.Save("fractal.png", ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private Color MapColor(double mu)
        {
            // Linear interporlate the color

            int color1 = (int)mu % ColorCount;
            int color2 = (color1 + 1) % ColorCount;

            double t2 = mu - color1;
            double t1 = 1 - t2;

            byte r = (byte)(Colors[color1].R * t1 + Colors[color2].R * t2);
            byte g = (byte)(Colors[color1].G * t1 + Colors[color2].G * t2);
            byte b = (byte)(Colors[color1].B * t1 + Colors[color2].B * t2);
            return Color.FromArgb(255, r, g, b);
        }
    }
}