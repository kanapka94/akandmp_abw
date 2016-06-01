using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    /*class DFT : Algorytm
    {

        /* Aby algorytm działał poprawnie należy podać jako dokładność liczbę całkowitą */
        /*(public override double[] ObliczWidmo(int[] sygnal, int dokladnosc)
        {
            /*double[] wynik = new double[(int)((gornaCzestosc - dolnaCzestosc)/dokladnosc + 1)];
            int N = sygnal.Length;

            // Wzór na DFT //

            Complex suma = new Complex(0, 0);

            double k = dolnaCzestosc;

            int index = 0;

            while (k <= gornaCzestosc)
            {
                suma = 0;

                for (int j = 0; j < N; j++)
                {
                    suma += sygnal[j] * Complex.Exp(Complex.ImaginaryOne * (double)(- 2 * (decimal)Math.PI * (decimal)j * (decimal)(k) / (decimal)N));
                }
                // wynik[k] =10 * Math.Log10(Math.Pow(Complex.Abs(suma),2));
                wynik[index++] = (double)1 / (double)N * Complex.Abs(suma);
                k += dokladnosc;
            }

            return wynik;*/
          /*  return null;
        }

    }*/
}
