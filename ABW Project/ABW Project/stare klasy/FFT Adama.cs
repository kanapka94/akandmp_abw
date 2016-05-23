/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    class FFT : Algorytm
    {
        public override double[] ObliczWidmo(double[] sygnal,int iloscProbek = -1)
        {
            return PodzialFFT(DostosujSygnal(sygnal));
            //return PodzialRadix2(DostosujSygnal(sygnal));
        }
        private double[] PodzialFFT(double[] sygnal)
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

            for (int n = 0; n < N / 2; n++)   //uzupełniam puste szuflady (2 skrzynki z próbkami parzystymi i nieparzystymi) próbkami
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

                //S[k] = Math.Pow(Complex.Abs(suma),2)/N;   //moc sygnału
                S[k] = Math.Pow(Complex.Abs(suma), 2) / N;
            }

            for (int k = 0; k < N / 2; k++)
            {
                S[k] = probkiParzysteCalosc[k] + probkiNieparzysteCalosc[k];
                S[k + N / 2] = probkiParzysteCalosc[k] - probkiNieparzysteCalosc[k];
            }

            return S;   //zwracam powstały sygnał
        }
        private double[] DostosujSygnal(double[] sygnal)    //metoda uzupełnia sygnał zerami, tak aby jego długość była potęgą dwójki
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
        }


        //-----------------------------------------------------------------------------------------------------
        // ---------------------------------------- algorytm Nayuki -------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        public static double[] PodzialRadix2(double[] sygnal)
        {

            int n = sygnal.Length;
            int poziomy = 31 - LiczbaZerWiodacych(n);  // podłoga(log2(n))
            double[] cosTable = new double[n / 2];
            double[] sinTable = new double[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                cosTable[i] = Math.Cos(2 * Math.PI * i / n);
                sinTable[i] = Math.Sin(2 * Math.PI * i / n);
            }

            // Zamiana bitów
            for (int i = 0; i < n; i++)
            {
                int j = (int)((uint)OdwrocBity(i) >> (32 - poziomy));
                if (j > i)
                {
                    double temp = sygnal[i];
                    sygnal[i] = sygnal[j];
                    sygnal[j] = temp;
                }
            }

            // Cooley-Tukey podział w czasie radix-2 FFT
            for (int rozmiar = 2; rozmiar <= n; rozmiar *= 2)
            {
                int polowaRozmiaru = rozmiar / 2;
                int tablestep = n / rozmiar;
                for (int i = 0; i < n; i += rozmiar)
                {
                    for (int j = i, k = 0; j < i + polowaRozmiaru; j++, k += tablestep)
                    {
                        double tpre = sygnal[j + polowaRozmiaru] * cosTable[k];
                        double tpim = -sygnal[j + polowaRozmiaru] * sinTable[k];
                        sygnal[j + polowaRozmiaru] = sygnal[j] - tpre;
                        sygnal[j] += tpre;
                    }
                }
                if (rozmiar == n)  // Prevent overflow in 'size *= 2'
                    break;
            }

            double pom = 0.0;
            for (int i = 0; i < sygnal.Length; i++)
            {
                pom = Math.Pow(sygnal[i], 2);
                sygnal[i] = pom;
            }

            return sygnal;
        }


        private static int LiczbaZerWiodacych(int wartosc)
        {
            if (wartosc == 0)
                return 32;
            int wynik = 0;
            for (; wartosc >= 0; wartosc <<= 1)
                wynik++;
            return wynik;
        }


        private static int NajwiekszyBit(int wartosc)
        {
            for (int i = 1 << 31; i != 0; i = (int)((uint)i >> 1))
            {
                if ((wartosc & i) != 0)
                    return i;
            }
            return 0;
        }


        private static int OdwrocBity(int wartosc)
        {
            int wynik = 0;
            for (int i = 0; i < 32; i++, wartosc >>= 1)
                wynik = (wynik << 1) | (wartosc & 1);
            return wynik;
        }

    }
}*/
