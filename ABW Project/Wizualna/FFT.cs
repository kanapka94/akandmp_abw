using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Wizualna
{
    class FFT : Algorytm
    {
        public override double[] ObliczWidmo(double[] sygnal)
        {
            return PodzialFFT(sygnal);
        }
        public double[] PodzialFFT(double[] sygnal)
        {
            int N = sygnal.Length;  //długość sygnału
            double[] S = new double[N]; //tworzę nową tablicę na sygnał

            if (N == 1)     //jeżeli została mi tylko jedna próbka w sygnale to zwracam jej wartość
            {
                S[0] = sygnal[0];
                return S;
            }

            double[] probkiParzyste, probkiNieparzyste, probkiParzysteCalosc, probkiNieparzysteCalosc;

            probkiParzyste = new double[N / 2];     //
            probkiNieparzyste = new double[N / 2];  //dzielę sygnał na pół (część parzystą i nieparzystą)

            for (int n = 0; n < N/2; n++)   //uzupełniam puste szuflady (2 skrzynki z próbkami parzystymi i nieparzystymi) próbkami
            {
                probkiParzyste[n] = sygnal[2 * n];
                probkiNieparzyste[n] = sygnal[2 * n + 1];
            }

            //stosuję rekurencję
            probkiParzysteCalosc = PodzialFFT(probkiParzyste);
            probkiNieparzysteCalosc = PodzialFFT(probkiNieparzyste);

            Complex suma = new Complex(0, 0);

            for (int k = 0; k < N; k++)   //DFT
            {
                suma = 0;
                for (int n = 0; n < N; n++)
                {
                    suma += sygnal[n] * Complex.Exp((double)-2 * Complex.ImaginaryOne * Math.PI * (double)n * (double)k / (double)N);
                }
                // wynik[k] =10 * Math.Log10(Math.Pow(Complex.Abs(suma),2));
                S[k] = (double)2 / N * Complex.Abs(suma);
            }

            for (int k = 0; k < N/2; k++)
            {
                S[k] = probkiParzysteCalosc[k] + probkiNieparzysteCalosc[k];
                S[k + N / 2] = probkiParzysteCalosc[k] - probkiNieparzysteCalosc[k];
            }

            return S;   //zwracam powstały sygnał
        }
    }
}
