/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    class DFT : Algorytm
    {
        public override double[] ObliczWidmo(double[] sygnal,int iloscProbek = -1)
        {
            double[] wynik = new double[2000];

            int N = sygnal.Length;
            if (iloscProbek == -1) iloscProbek = N;
            double wartoscProbki = 0;

            // Wzór na DFT //

            Complex suma = new Complex(0, 0);

            decimal k = 49.8M;

            int index = 0;

            while (k <= 50.2M)
            {
                suma = 0;

                for (int j = 0; j < iloscProbek; j++)
                {
                    if (j >= N)
                        wartoscProbki = 0;
                    else
                        wartoscProbki = sygnal[j];

                    suma += wartoscProbki * Complex.Exp(Complex.ImaginaryOne * (double)(- 2 * (decimal)Math.PI * (decimal)j * (decimal)(k) / (decimal)N));
                }
                // wynik[k] =10 * Math.Log10(Math.Pow(Complex.Abs(suma),2));
                wynik[index++] = (double)1 / (double)N * Complex.Abs(suma);
                k += 0.001M;
            }

            return wynik;
        }

    }
}*/
