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

            //for (int k = -999; k < 1000; k++)

            decimal k = 49.8M;

            int index = 0;

            while (k <= 50.2M)
            {
                suma = 0;
                
                for (int j = 0; j < N; j++)
                {
                    suma += sygnal[j] * Complex.Exp(Complex.ImaginaryOne * (double)(- 2 * (decimal)Math.PI * (decimal)j * (decimal)(k) / (decimal)N));
                }
                // wynik[k] =10 * Math.Log10(Math.Pow(Complex.Abs(suma),2));
                wynik[index++] = (double)1 / (double)N * Complex.Abs(suma);
                k += 0.001M;
            }

            return wynik;
        }

    }
}
