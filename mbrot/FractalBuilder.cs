using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mbrot
{
    class FractalBuilder
    {
        private Bitmap bitmap;
        private FractalOptions fractalOptions;
        private Complex Z, C;
        public FractalBuilder(Bitmap bitmap, FractalOptions fractalOptions, Complex Z, Complex C)
        {
            this.bitmap = bitmap;
            this.fractalOptions = fractalOptions;
            this.Z = Z;
            this.C = C;
        }

        public Bitmap Build()
        {

        }
    }

    public struct FractalOptions
    {
        int MaxIterations = 100;
        double MinReal = -2.0;
        double MaxReal = 2.0;
        double MinImag = -2.0;
        double MaxImag = 2.0;
    }
}
