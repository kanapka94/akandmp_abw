//@autorzy: Michał Paduch i Adam Konopka
// @autor Robert Sedgewick
// @autor Kevin Wayne
//Wyżej wymienieni autorzy udostępniają cały powyższy/poniższy kod zawarty w niniejszym pliku 
//na zasadach licencji GNU GPLv2.http://www.gnu.org/licenses/old-licenses/gpl-2.0.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

namespace ABW_Project
{
    /// <summary>
    /// Klasa FFT - zawarte są w niej algorytmy metody FFT na spróbkowanym sygnale
    /// </summary>
    class FFT : Algorytm
    {
        /// <summary>
        /// Metoda przygotowująca sygnał spróbkowany do FFT
        /// </summary>
        /// <param name="probki">sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <param name="OknoT">numer okna przez które przemnożymy sygnał</param>
        /// <returns>Zwraca zmieniony sygnał</returns>
        public Complex[] PrzygotujDaneDoFFT(int[] probki, int dokladnosc,int OknoT)
        {
            Complex[] wynik;

            int[] w1 = (int[])probki.Clone();
            double[] x = Okno.Funkcja(probki, OknoT);

            int licznik = 0;

            for (int i = 0; i < w1.Length; i++)
            {
                if (w1[i] == x[i])
                    licznik++;
            }
            StreamWriter sww = new StreamWriter("wyniksaasd.txt");

            sww.WriteLine(Convert.ToString(licznik) + " " +Convert.ToString(w1.Length));
            sww.Close();
            double[] daneDoObliczenia = new double[dokladnosc];

            for (int i = 0; i < probki.Length; i++)
            {
                daneDoObliczenia[i] = probki[i];
            }
            for (int i = probki.Length; i < daneDoObliczenia.Length; i++)
            {
                daneDoObliczenia[i] = 0;
            }

            double[] tablicaPotegiDwa;
            
            

            tablicaPotegiDwa = WypelnijZerami(daneDoObliczenia);

            wynik = new Complex[tablicaPotegiDwa.Length];

            for (int i = 0; i < tablicaPotegiDwa.Length; i++)
            {
                wynik[i] = new Complex(tablicaPotegiDwa[i] / (double)tablicaPotegiDwa.Length, 0.0);
            }//next i

            return wynik;
        }

        /// <summary>
        /// Metoda uzupełnia sygnał zerami, tak aby jego długość była potęgą dwójki
        /// </summary>
        /// <param name="dane">spróbkowany sygnał</param>
        /// <returns>Zwraca rozszerzony sygnał</returns>
        public static double[] WypelnijZerami(double[] dane)
        {

            if ((dane.Length & (dane.Length - 1)) == 0) return dane;

            double log2;
            int log2_int;
            int i;
            double[] wynik;
            int k;

            log2 = Math.Log((double)dane.Length) / Math.Log(2);
            log2_int = (int)Math.Round(log2);

            wynik = null;

            k = (int)Math.Pow(2.0, (double)log2_int);

            if (k < dane.Length) log2_int++; //in case when the k is too small

            k = (int)Math.Pow(2.0, (double)log2_int);

            wynik = new double[k];

            for (i = 0; i < k; i++)
            {
                if (i < dane.Length)
                {
                    wynik[i] = dane[i];
                }
                else
                {
                    wynik[i] = 0;
                }//end if
            }//next i

            return wynik;

        }//end of WypelnijZerami

        /// <summary>
        /// Metoda licząca widmo
        /// </summary>
        /// <param name="sygnal">sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca widmo sygnału</returns>
        public override double[] ObliczWidmo(int[] sygnal, int dokladnosc = 1)
        {
            Complex[] y = PrzygotujDaneDoFFT(sygnal,dokladnosc, Okno.Blackmana);

            double[] wynik = new double[y.Length];

            y = fft(y);

            for (int i = 0; i < y.Length ; i++)
            {
                wynik[i] = Complex.Abs(y[i]);
            }
            return wynik;
        }

        /// <summary>
        /// Metoda zwracająca indeks danej częstotliwości w tablicy widma.
        /// </summary>
        /// <param name="czestosc">Częstotliwość, której chcemy znaleźć indeks</param>
        /// <param name="czestotliwoscProbkowania">Częstotliwość próbkowania</param>
        /// <param name="rozmiarWidma">Rozmiar widma</param>
        /// <returns>Zwraca indeks częstotliwości w tablicy widma</returns>
        public int IndeksCzestosciWWidmie(double czestosc, int czestotliwoscProbkowania, int rozmiarWidma)
        {
            return (int)((double)czestosc * (double)rozmiarWidma / (double)czestotliwoscProbkowania);
        }

        /// <summary>
        /// FFT
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca wartości danej częstotliwości</returns>
        public static Complex[] fft(Complex[] x)
        {
            int N = x.Length;

            if (N == 1) return new Complex[] { x[0] };

            // fft of even terms
            Complex[] parzyste = new Complex[N / 2];
            for (int k = 0; k < N / 2; k++)
            {
                parzyste[k] = x[2 * k];
            }
            Complex[] q = fft(parzyste);

            // fft of odd terms
            Complex[] nieparzyste = parzyste;
            for (int k = 0; k < N / 2; k++)
            {
                nieparzyste[k] = x[2 * k + 1];
            }
            Complex[] r = fft(nieparzyste);

            // combine
            Complex[] y = new Complex[N];
            for (int k = 0; k < N / 2; k++)
            {
                double kth = -2 * k * Math.PI / N;
                Complex wk = new Complex(Math.Cos(kth), Math.Sin(kth));
                y[k] = q[k] + wk * r[k];
                y[k + N / 2] = q[k] - wk * r[k];
            }

            return y;
        }

        /// <summary>
        /// Odwrotne FFT
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca wartości amplitudy w czasie</returns>
        public static Complex[] ifft(Complex[] x)
        {
            int N = x.Length;
            Complex[] y = new Complex[N];

            for (int i = 0; i < N; i++)
            {
                y[i] = Complex.Conjugate(x[i]);
            }

            y = fft(y);

            for (int i = 0; i < N; i++)
            {
                y[i] = Complex.Conjugate(y[i]);
            }

            // podziel przez N
            for (int i = 0; i < N; i++)
            {
                y[i] = y[i] * (1.0 / N);
            }

            return y;

        }

    }
}

