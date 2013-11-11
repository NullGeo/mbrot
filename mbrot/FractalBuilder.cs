using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace mbrot
{
    class FractalBuilder
    {
        private Bitmap bitmap;
        private FractalOptions fractalOptions;
        private readonly int COLOR_COUNT = 256;
        private List<Color> _colors;
        private static readonly Random _Random = new Random();

        public FractalBuilder(int imageWidth, int imageHeight, FractalOptions fractalOptions, List<Color> colors = null)
        {
            this.bitmap = new Bitmap(imageWidth, imageHeight);

            this.fractalOptions = fractalOptions;

            FixAspectRatio();

            if (colors == null)
            {
                _colors = new List<Color>(COLOR_COUNT);

                for (int i = 0; i < COLOR_COUNT; i++)
                {
                    _colors.Add(Color.FromArgb(_Random.Next() % 0xffffff));
                }
            }
            else
            {
                COLOR_COUNT = colors.Count;
            }
        }

        public Bitmap Build()
        {
            int imageWidth = bitmap.Width, imageHeight = bitmap.Height;

            double XScale = (fractalOptions.MaxReal - fractalOptions.MinReal) / (imageWidth - 1);
            double YScale = (fractalOptions.MaxImag - fractalOptions.MinImag) / (imageHeight - 1);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            IntPtr ptr = bitmapData.Scan0;
            int pixels = bitmap.Width * bitmap.Height;

            Int32[] rgbValues = new Int32[pixels];

            double ReaC = fractalOptions.MinReal;

            for (int x = 0; x < imageWidth; x++)
            {
                int index = x;
                double ImaC = fractalOptions.MinImag;

                for (int y = 0; y < imageHeight; y++)
                {
                    Complex Z = Complex.Zero;
                    Complex C = new Complex(ReaC, ImaC);

                    int iterations = 0;

                    while (iterations <= fractalOptions.MaxIterations && Z.Magnitude < 2.0)
                    {
                        Z = Z * Z + C;
                        iterations++;
                    }

                    if (iterations >= fractalOptions.MaxIterations)
                    {
                        rgbValues[index] = Color.Black.ToArgb();
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
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

            return bitmap;
        }

        private Color MapColor(double mu)
        {
            // Linear interporlate the color

            int color1 = (int)mu % COLOR_COUNT;
            int color2 = (color1 + 1) % COLOR_COUNT;

            double t2 = mu - color1;
            double t1 = 1 - t2;

            byte r = (byte)(_colors[color1].R * t1 + _colors[color2].R * t2);
            byte g = (byte)(_colors[color1].G * t1 + _colors[color2].G * t2);
            byte b = (byte)(_colors[color1].B * t1 + _colors[color2].B * t2);
            return Color.FromArgb(255, r, g, b);
        }

        private void FixAspectRatio()
        {
            double tempHeight, tempWidth, midpoint;

            double desiredAR = (fractalOptions.MaxImag - fractalOptions.MinImag) / (fractalOptions.MaxReal - fractalOptions.MinReal);
            double bitmapAR = bitmap.Height / (double)bitmap.Width;

            if (desiredAR > bitmapAR)
            {
                tempWidth = (fractalOptions.MaxImag - fractalOptions.MinImag) / bitmapAR;
                midpoint = (fractalOptions.MaxReal + fractalOptions.MinReal) / 2;
                fractalOptions.MinReal = midpoint - tempWidth / 2;
                fractalOptions.MaxReal = midpoint + tempWidth / 2;
            }
            else
            {
                tempHeight = (fractalOptions.MaxReal - fractalOptions.MinReal) * bitmapAR;
                midpoint = (fractalOptions.MaxImag + fractalOptions.MinImag) / 2;
                fractalOptions.MinImag = midpoint - tempHeight / 2;
                fractalOptions.MaxImag = midpoint + tempHeight / 2;
            }
        }
    }



    public class FractalOptions
    {
        public int MaxIterations = 100;
        public double MinReal = -2.0;
        public double MaxReal = 2.0;
        public double MinImag = -2.0;
        public double MaxImag = 2.0;
    }
}
