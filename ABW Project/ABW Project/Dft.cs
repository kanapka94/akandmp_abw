using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    class DFT : Algorytm
    {
        public override double[] ObliczWidmo(double[] sygnal)
        {
            double[] wynik = new double[2000];

            int N = sygnal.Length;

            // Wzór na DFT //
            
            Complex suma = new Complex(0, 0);
            
            for (int k = -999; k < 1000; k++)
            {
                suma = 0;
                
                for (int j = 0; j < N; j++)
                {
                    suma += sygnal[j] * Complex.Exp((double)-2 * Complex.ImaginaryOne * Math.PI * (double)j * (double)(((double)k / 1000) + 50) / (double)N);
                }
                // wynik[k] =10 * Math.Log10(Math.Pow(Complex.Abs(suma),2));
                wynik[k+999] = (double)2 / N * Complex.Abs(suma);
            }

            return wynik;
        }

    }
}
