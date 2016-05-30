using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    class CZT : Algorytm
    {
        public Complex[] czt(int[] probki, int czestoscProbkowania, int iloscPrazkow, double czestoscDolna, double czestoscGorna)
        {

            int N = probki.Length;
            int NM1 = N + iloscPrazkow - 1;
            Complex A = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * czestoscDolna / czestoscProbkowania);
            Complex W = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * ((czestoscGorna - czestoscDolna) / (2 * (iloscPrazkow - 1)) / czestoscProbkowania));

            Complex[] y1 = new Complex[N];
            Complex[] y2 = new Complex[NM1];

            int k;

            for (k = 0; k < N; k++)
            {
                y1[k] = Complex.Pow(A * Complex.Pow(W, k), k) * probki[k];
            }

            for (k = 0; k < iloscPrazkow; k++)
            {
                y2[k] = Complex.Pow(W, Math.Pow(-k, 2));
            }

            for (k = N; k < NM1; k++)
            {
                y2[k] = Complex.Pow(W, Math.Pow(-(NM1 - k), 2));
            }

            Complex[] Y1 = FFT.fft(y1);
            Complex[] Y2 = FFT.fft(y2);
            Complex[] Y = new Complex[Y2.Length];

            for (int i = 0; i < Y2.Length; i++)
            {
                if (i < Y1.Length)
                {
                    Y[i] = Complex.Multiply(Y1[i], Y2[i]);
                }
                else
                {
                    Y[i] = Y2[i];
                }
            }

            Complex[] y = new Complex[Y.Length];

            Complex[] pom = new Complex[Y.Length];

            pom = FFT.ifft(Y);

            for (int i = 0; i < pom.Length; i++)
            {
                y[i] = Complex.Divide(pom[i], N / 2);
            }

            Complex[] XcztN = new Complex[iloscPrazkow];

            for (k = 0; k < iloscPrazkow; k++)
            {
                XcztN[k] = y[k] * Complex.Pow(W, Math.Pow(k, 2));
            }

            return XcztN;
        }

        public void PrzygotujDaneDoCZT(int[] probki, int OknoT)
        {
            Okno.Funkcja(probki, OknoT); 
        }

        public override double[] ObliczWidmo(int[] probki, int dokladnosc = 1)
        {
            PrzygotujDaneDoCZT(probki, Okno.Blackmana);

            double[] wynik = new double[probki.Length];

            Complex[] sygnal = new Complex[probki.Length];

            sygnal = czt(probki, 44100, 256, 48, 52);   //uruchamianie CZT ze stało określonymia parametrami

            for (int i = 0; i < sygnal.Length; i++)
            {
                wynik[i] = Complex.Abs(sygnal[i]);
            }
            return wynik;
        }
    }
}
