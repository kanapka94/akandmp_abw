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
        public Complex[] PrzygotujDaneDoFFT(double[] probki, int dokladnosc,Okno okno)
        {
            Complex[] wynik;

            okno.Funkcja(probki);
            probki = WypelnijZerami(probki,dokladnosc);
            PrzestawienieProbek(probki);
            wynik = new Complex[probki.Length];

            for (int i = 0; i < probki.Length; i++)
            {
                wynik[i] = new Complex(probki[i] / (double)probki.Length, 0.0);
            }

            return wynik;
        }

        /// <summary>
        /// Metoda uzupełnia sygnał zerami, tak aby jego długość była potęgą dwójki
        /// </summary>
        /// <param name="dane">spróbkowany sygnał</param>
        /// <returns>Zwraca rozszerzony sygnał</returns>
        public static double[] WypelnijZerami(double[] dane,int iloscProbek)
        {

            if ((dane.Length & (dane.Length - 1)) == 0) return dane;

            double log2;
            int log2_int;
            int i;
            double[] wynik;
            int k;

            //log2 = Math.Log((double)dane.Length) / Math.Log(2);
            log2 = Math.Log((double)iloscProbek) / Math.Log(2);
            log2_int = (int)Math.Round(log2);

            wynik = null;

            k = (int)Math.Pow(2.0, (double)log2_int);

            if (k < iloscProbek) log2_int++; //in case when the k is too small

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
                }
            }

            return wynik;

        }//end of WypelnijZerami

        /// <summary>
        /// Metoda licząca widmo
        /// </summary>
        /// <param name="sygnal">sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca widmo sygnału</returns>
        public override double[] ObliczWidmo(double[] sygnal, Okno okno, int dokladnosc = -1)
        {
            Complex[] y = PrzygotujDaneDoFFT(sygnal,dokladnosc, okno);
            double[] wynik = new double[y.Length];
            y = fftFor(y);
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

        // ================== FFT nierekurencyjne ======================

        /// <summary>
        /// Metoda przestawiająca próbki w sygnal spróbkowanym (przygotowuje dane do FFT)
        /// </summary>
        /// <param name="probki">Spróbkowany sygnał</param>
        /// <returns></returns>
        public double[] PrzestawienieProbek(double[] probki)
        {
            int a = 1;
            int N = probki.Length; //ilość próbek w sygnale
            int c;

            double T;  //zmienna pomocnicza przechowująca wartość próbki

            for (int b = 1; b < N; b++)
            {
                if (b < a)
                {
                    T = probki[a-1];
                    probki[a-1] = probki[b-1];
                    probki[b-1] = T;
                }
                c = N / 2;
                while (c < a)
                {
                    a = a - c;
                    c = c / 2;
                }
                a = a + c;          
            }

            return probki;
        }

        /// <summary>
        ///    FFT tylko, że obliczanie bez rekurencji
        /// </summary>
        /// <param name="probki">Sygnał spróbkowany</param>
        /// <returns></returns>
        public static Complex[] fftFor(Complex[] probki)
        {
            int N = probki.Length;  //ilość próbek w sygnale
            int L,M;
            Complex T;        

            int d;

            Complex W, Wi = 1;

            for (int e = 1; e <= Math.Log(N,2); e++)
            {
                L = (int)Math.Pow(2, e); //długość bloków DFT
                M = (int)Math.Pow(2, (e - 1));   //liczba motylków w bloku, szerokość każdego motylka
                Wi = 1;

                W = Complex.Cos(2 * Math.PI / L) - Complex.ImaginaryOne * Complex.Sin(2 * Math.PI / L);  //mnożnik bazy Fouriera

                for (int m = 1; m <= M; m++) //Kolejne motylki
                {
                    for (int g = m; g <= N; g += L) //w kolejnych blokach
                    {
                        d = g + M;          //d - dolny indeks próbki motylka, g - górny indeks próbki motylka 
                        T = probki[d-1] * Wi; //"serce" FFT
                        probki[d-1] = probki[g-1] - T;
                        probki[g-1] = probki[g-1] + T;
                    }
                    Wi = Wi * W;
                }
            }

            return probki;  // probki to inaczej wynik FFT
        }
    }
}

