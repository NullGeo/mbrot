using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mbrot
{
    class Fractal
    {

        public const double MinReal = -2.0;
        public const double MaxReal = 2.0;
        public const double MinImag = -2.0;
        public const double MaxImag = 2.0;

        public readonly double XScale;
        public readonly double YScale;
        public readonly int MaxIterations;
        public Fractal(int imageWidth, int imageHeight, int maxIterations)
        {
            XScale = (MaxReal - MinReal) / (imageWidth);
            YScale = (MaxImag - MinImag) / (imageHeight);
            MaxIterations = maxIterations;

            Increment = new Complex(MinReal, MaxImag);
            Seed = new Complex(MinReal, MaxImag);
        }

        public Complex Seed
        {
            get;
            set;
        }

        public Complex Increment
        {
            get;
            set;
        }
    }
}
