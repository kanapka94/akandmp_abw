using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Wizualna
{
    class DFT : Algorytm
    {
        public override double[] ObliczWidmo(double[] sygnal)
        {
            int N = sygnal.Length;
            double[] wynik = new double[N];

            // Wzór na DFT //

            Complex suma = new Complex(0, 0);

            for (int k = 0; k < N; k++)
            {
                suma = 0;

                for (int j = 0; j < N; j++)
                {
                    suma += sygnal[j] * Complex.Exp((double)-2 * Complex.ImaginaryOne * Math.PI * (double)j * (double)k / (double)N);
                }
                // wynik[k] =10 * Math.Log10(Math.Pow(Complex.Abs(suma),2));
                wynik[k] = (double)2 / N * Complex.Abs(suma);
            }
            return wynik;
        }
    }
}
