//autorzy: Michał Paduch i Adam Konopka
// @autor Robert Sedgewick
// @autor Kevin Wayne
//licencja: GPLv2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

namespace ABW_Project
{
    class FFT : Algorytm
    {
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

        public int IndeksCzestosciWWidmie(double czestosc, int czestotliwoscProbkowania, int rozmiarWidma)
        {
            return (int)((double)czestosc * (double)rozmiarWidma / (double)czestotliwoscProbkowania);
        }

        private static Complex[] fft(Complex[] x)
        {
            int N = x.Length;

            if (N == 1) return new Complex[] { x[0] };

            // fft of even terms
            Complex[] even = new Complex[N / 2];
            for (int k = 0; k < N / 2; k++)
            {
                even[k] = x[2 * k];
            }
            Complex[] q = fft(even);

            // fft of odd terms
            Complex[] odd = even;  // reuse the array
            for (int k = 0; k < N / 2; k++)
            {
                odd[k] = x[2 * k + 1];
            }
            Complex[] r = fft(odd);

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

        public static Complex[] ifft(Complex[] x)
        {
            int N = x.Length;
            Complex[] y = new Complex[N];

            for (int i = 0; i < N; i++)
            {
                y[i] = new Complex(x[i].Real, -x[i].Imaginary);
            }

            y = fft(y);

            for (int i = 0; i < N; i++)
            {
                y[i] = y[i] = new Complex(y[i].Real, -y[i].Imaginary); ;
            }

            // podziel przez N
            for (int i = 0; i < N; i++)
            {
                y[i] = y[i] * (1.0 / N);
            }

            return y;

        }


        //metoda uzupełnia sygnał zerami, tak aby jego długość była potęgą dwójki
        private double[] DostosujSygnal(double[] sygnal)    
        {
            int n = sygnal.Length;

            if ((n & (n - 1)) == 0)     //sprawdza czy długość sygnału jest potęgą dwójki
                return sygnal;

            int potega = 0;
            for (int i = 1; i <= 24; i++)   //sprawdza do jakiej potęgi dwójki ma rozszerzyć tablicę z próbkami
            {
                if (sygnal.Length <= Math.Pow(2, i))
                {
                    potega = i;
                    break;
                }
            }

            int ileCykliPotrzeba = (int)Math.Pow(2, potega);

            double[] sygnalRozszerzony = new double[ileCykliPotrzeba];

            for (int i = 0; i < sygnal.Length; i++)  //kopiuję wartości sygnału starego
            {
                sygnalRozszerzony[i] = sygnal[i];
            }

            for (int i = sygnal.Length - 1; i < ileCykliPotrzeba; i++)
            {
                sygnalRozszerzony[i] = 0;
            }

            return sygnalRozszerzony;
        }
    }
}

