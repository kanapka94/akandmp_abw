using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    class FFT : Algorytm
    {
        public Complex[] PrzygotujDaneDoFFT(double[] x, int WindowT)
        {

            Complex[] result;

            double[] data_for_calculation;
            double[] table_power_2;
            int i;

            data_for_calculation = x;
            Okno.Funkcja(x, WindowT);

            table_power_2 = FillZeros(data_for_calculation);
            result = new Complex[table_power_2.Length];

            for (i = 0; i < table_power_2.Length; i++)
            {
                result[i] = new Complex(table_power_2[i] / (double)table_power_2.Length, 0.0);
            }//next i

            return result;
        }//end of DataPreparingForFFT

        public static double[] FillZeros(double[] x)
        {

            if ((x.Length & (x.Length - 1)) == 0) return x;

            double log2;
            int log2_int;
            int i;
            double[] result;
            int k;

            log2 = Math.Log((double)x.Length) / Math.Log(2);
            log2_int = (int)Math.Round(log2);

            result = null;

            k = (int)Math.Pow(2.0, (double)log2_int);

            if (k < x.Length) log2_int++; //in case when the k is too small

            k = (int)Math.Pow(2.0, (double)log2_int);

            result = new double[k];

            for (i = 0; i < k; i++)
            {
                if (i < x.Length)
                {
                    result[i] = x[i];
                }
                else
                {
                    result[i] = 0;
                }//end if
            }//next i

            return result;

        }//end of FillZeros

        /*private double[] DostosujSygnal(double[] sygnal)    //metoda uzupełnia sygnał zerami, tak aby jego długość była potęgą dwójki
        {
            int n = sygnal.Length;

            if ((n & (n - 1)) == 0)     //sprawdza czy długość sygnału jest potęgą dwójki
                return sygnal;

            int potega = 0;
            for (int i = 1; i <= 16; i++)   //sprawdza do jakiej potęgi dwójki ma rozszerzyć tablicę z próbkami
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
        }*/

        public override double[] ObliczWidmo(double[] sygnal)
        {
            //sygnal = DostosujSygnal(sygnal);


            Complex[] y = PrzygotujDaneDoFFT(sygnal, Okno.Blackmana);

            /*Complex[] y = new Complex[sygnal.Length];
            for (int i = 0; i < y.Length; i++)
            {
                y[i] = new Complex(sygnal[i], 0);
            }*/
            double[] wynik = new double[y.Length];

            //

            y = fft(y);

            for (int i = 0; i < y.Length; i++)
            {
                wynik[i] = Complex.Abs(y[i]);
                //Math.Pow(Complex.Abs(y[i]),2)
            }

            return wynik;
        }



        private static Complex[] fft(Complex[] x)
        {
            int N = x.Length;

            // base case
            if (N == 1) return new Complex[] { x[0] };

            // radix 2 Cooley-Tukey FFT
            if (N % 2 != 0)
            {
               // throw new IllegalArgumentException("N is not a power of 2");
            }

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
    }
}

