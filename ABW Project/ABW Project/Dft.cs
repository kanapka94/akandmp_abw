using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    class Dft : Algorytm
    {
        public override double[] ObliczWidmo(double[] sygnal)
        {
            int N = sygnal.Length;

            int iloscCzestotliwosci = N;

            double[] wynik = new double[iloscCzestotliwosci];

            // Wzór na DFT //

            Complex suma = new Complex(0, 0);
            
            for (int k = 0; k < iloscCzestotliwosci; k++)
            {
                suma = 0;
                
                for (int j = 0; j < N; j++)
                {
                    suma += sygnal[j] * Complex.Exp((double)-2 * Complex.ImaginaryOne * Math.PI * (double)j * (double)k / (double)N);
                }
                // wynik[k] =10 * Math.Log10(Math.Pow(1/N * Complex.Abs(suma),2));
                wynik[k] = Complex.Abs(suma);
            }

            return wynik;
        }
    }
}
