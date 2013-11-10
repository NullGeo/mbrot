using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbrot
{
    public struct Complex : IEquatable<Complex>
    {
        public double Real;
        public double Imag;

        public Complex(double real, double imag)
        {
            this.Real = real;
            this.Imag = imag;
        }


        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Real + c2.Real, c1.Imag + c2.Imag);
        }

        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Real - c2.Real, c1.Imag - c2.Imag);
        }

        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Real * c2.Real - c1.Imag * c2.Imag, c1.Real * c2.Imag + c2.Real * c1.Imag);
        }

        public static Complex operator /(Complex c1, Complex c2)
        {
            double denominator = c2.Real * c2.Real + c2.Imag * c2.Imag;
            Complex numerator = c1 * Conjugate(c2);
            return new Complex(numerator.Real / denominator, numerator.Imag / denominator);
        }

        public static Complex Conjugate(Complex other)
        {
            return new Complex(other.Real, other.Imag * -1);
        }

        public override string ToString()
        {
            return (String.Format("{0} + {1}i", Real, Imag));
        }

        public static double Magnitude(Complex c1)
        {
            return Math.Sqrt(c1.Real * c1.Real + c1.Imag * c1.Imag);
        }

        public bool Equals(Complex other)
        {
            return (this.Imag == other.Imag && this.Real == other.Real);
        }
    }
}
