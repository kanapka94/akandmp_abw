//autor: Tomasz P.Zieliński
//oraz zamiana kodu na C#: Michał Paduch i Adam Konopka
//źródło: T.Zieliński - Cyfrowe Przetwarzanie Sygnałów
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
    /// Klasa CZT - zawarte są w niej algorytmy metody CZT na spróbkowanym sygnale
    /// </summary>
    class CZT : Algorytm
    {
        public override int PrzeliczDokladnosc(double dokladnosc, int czestotliwoscProbkowania, double dolnaCzestosc = 0, double gornaCzestosc = 0)
        {
            int iloscPrazkow = (int)Math.Round((gornaCzestosc - dolnaCzestosc) / dokladnosc);
            return iloscPrazkow;
        }

        public override void SprawdzDokladnosc(double dokladnosc)
        {

        }

        /// <summary>
        /// Metoda wydzielająca wartość przydźwięku sieciowego co sekundę
        /// </summary>
        /// <param name="plik">Obiekt klasy PlikWave</param>
        /// <param name="stan">Zmienna referencyjna wskazująca ilość procent wykonania algorytmu</param>
        /// <param name="okno">Obiekt, który obliczy okno sygnału</param>
        /// <param name="plikPrzydzwieku">Plik, do którego zostanie zapisany znaleziony przydźwięk</param>
        /// <param name="dolnaCzestosc">Dolna częstotliwość (graniczna, badana)</param>
        /// <param name="gornaCzestosc">Górna częstotliwość (graniczna, badana)</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca obiekt klasy Wynik</returns>
        public override Wynik WydzielPrzydzwiek(PlikWave plik, ref int stan, Okno okno, string plikPrzydzwieku, double dolnaCzestosc = 40, double gornaCzestosc = 60, double dokladnosc = 1)
        {
            SprawdzDokladnosc(dokladnosc);
            if (dolnaCzestosc < 0) throw new Exception("Dolna częstotliwość poniżej 0");
            if (gornaCzestosc < 0) throw new Exception("Gorna częstotliwość poniżej 0");
            if (dolnaCzestosc > gornaCzestosc) throw new Exception("Górna częstotliwość jest mniejsza niż dolna częstotliwość");

            int iloscPrazkow = PrzeliczDokladnosc(dokladnosc, plik.czestotliwoscProbkowania,dolnaCzestosc,gornaCzestosc);

            Wynik wynik = new Wynik();
            wynik.czestotliwoscSygnalu = new double[(int)plik.dlugoscWSekundach];   // Tworzy tablicę wynik, której długość wynosi tyle
                                                                                    // ile sekund ma nagranie. Jest to spowodowane tym
                                                                                    // aby dla każdej sekundy wybrać najlepszy przydźwięk
            double[] widmo;
            StreamWriter sw;

            try
            {
                sw = new StreamWriter(plikPrzydzwieku); // Tymczasowe tylko do zapisu wyników widma
            }
            catch (Exception e)
            {
                throw e;
            }

            int rozmiarWidma;
            for (int sekunda = 0; sekunda < wynik.czestotliwoscSygnalu.Length; sekunda++)
            {
                widmo = ObliczWidmo(plik.PobierzProbki(), okno, iloscPrazkow,dolnaCzestosc,gornaCzestosc);
                rozmiarWidma = widmo.Length;
                double przydzwiek = ZnajdzPrzydzwiekWWidmie2(widmo);
                przydzwiek = dolnaCzestosc + (double)przydzwiek * dokladnosc;



                try
                {
                    sw.WriteLine("sekunda");
                    for (int i = 0; i < widmo.Length; i++)
                    {
                        
                        sw.WriteLine("{0} {1}", i, widmo[i]);
                    }
                    //sw.WriteLine("{0} {1}", sekunda, przydzwiek);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                wynik.czestotliwoscSygnalu[sekunda] = przydzwiek;
                stan = (int)((double)(sekunda + 1) / (double)wynik.czestotliwoscSygnalu.Length * 100.0);
            }

            try
            {
                sw.Close();  // Tymczasowe tylko do zapisu wyników widma
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return wynik;
        }

        // Metoda odnajdująca przydźwięk w widmie wybierając element maksymalny. 
        public int ZnajdzPrzydzwiekWWidmie2(double[] widmo)
        {
            double max = widmo[0];
            int indeksMax = 0;
            for (int index = 1; index < widmo.Length; index++)
            {
                if (widmo[index] > max)
                {
                    max = widmo[index];
                    indeksMax = index;
                }
            }

            return indeksMax;
        }

        /// <summary>
        /// CZT
        /// </summary>
        /// <param name="probki">Sygnał spróbkowany</param>
        /// <param name="czestoscProbkowania">Częstotliwość próbkowania</param>
        /// <param name="iloscPrazkow">Ilość prążków</param>
        /// <param name="czestoscDolna">Częstotliwość dolna</param>
        /// <param name="czestoscGorna">Częstotliwość górna</param>
        /// <returns>Zwraca wartości danej częstotliwości</returns>
        public static Complex[] czt(double[] probki, int czestoscProbkowania, int iloscPrazkow, double czestoscDolna, double czestoscGorna)
        {
             int N = probki.Length;
             int NM1 = N + iloscPrazkow - 1;
             Complex A = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * czestoscDolna / czestoscProbkowania);
             Complex W = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * ((czestoscGorna - czestoscDolna) / (2 * (iloscPrazkow - 1)) / czestoscProbkowania));

             Complex[] y1 = new Complex[NM1];
             Complex[] y2 = new Complex[NM1];

             int k;

             for (k = 0; k < NM1; k++)
             {
                 if (k < N) y1[k] = Complex.Pow(A * Complex.Pow(W, k), k) * probki[k]; else y1[k] = 0;
                 if (k < iloscPrazkow) y2[k] = Complex.Pow(W, -Math.Pow(k, 2)); else y2[k] = Complex.Pow(W, -Math.Pow((NM1 - k), 2));
             }

           /* int N = probki.Length;
            int NM1 = N + iloscPrazkow - 1;
            Complex A = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * czestoscDolna / czestoscProbkowania);
            Complex W = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * ((czestoscGorna - czestoscDolna) / (2 * (iloscPrazkow - 1)) / czestoscProbkowania));

            Complex[] y1 = new Complex[NM1];
            Complex[] y2 = new Complex[NM1];

            int k;

            for (k = 0; k < N; k++)
            {
                y1[k] = Complex.Pow(A * Complex.Pow(W, k), k) * probki[k];
            }

            for (k = 0; k < iloscPrazkow; k++)
            {
                y2[k] = Complex.Pow(W, -Math.Pow(k, 2));
            }

            for (k = iloscPrazkow; k < NM1; k++)
            {
                y2[k] = Complex.Pow(W, -Math.Pow((NM1 - k), 2));
            }*/



            y1 = FFT.WypelnijZerami(y1,y1.Length);
            y2 = FFT.WypelnijZerami(y2,y2.Length);

            Complex[] Y1 = FFT.fftFor(y1);
            Complex[] Y2 = FFT.fftFor(y2);
            Complex[] Y = new Complex[Y2.Length];

            for (int i = 0; i < Y2.Length; i++)
            {
                 Y[i] = Complex.Multiply(Y1[i], Y2[i]);
            }

            Complex[] y = new Complex[Y.Length];
            Complex[] pom = FFT.ifft(Y);

            for (int i = 0; i < pom.Length; i++)
            {
                y[i] = Complex.Divide(pom[i], N / 2);
            }

            Complex[] XcztN = new Complex[iloscPrazkow];

            for (k = 0; k < iloscPrazkow; k++)
            {
                XcztN[k] = Complex.Multiply(y[k],Complex.Pow(W, Math.Pow(k, 2)));
            }

            return XcztN;
        }

        /// <summary>
        /// Metoda "okienkuje" sygnał
        /// </summary>
        /// <param name="probki">Sygnał spróbkowany</param>
        /// <param name="OknoT">Numer okna przez które przemnożymy sygnał</param>
        public void PrzygotujDaneDoCZT(double[] probki, Okno okno)
        {
            okno.Funkcja(probki);
        }

        /// <summary>
        /// Metoda licząca widmo
        /// </summary>
        /// <param name="probki">Sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca widmo sygnału</returns>
        public override double[] ObliczWidmo(double[] probki, Okno okno, int iloscPrazkow, double dolnaCzestosc, double gornaCzestosc)
        {
            PrzygotujDaneDoCZT(probki, okno);

            double[] wynik = new double[iloscPrazkow];
            Complex[] sygnal = czt(probki, probki.Length, iloscPrazkow, dolnaCzestosc, gornaCzestosc);   //uruchamianie CZT ze stało określonymia parametrami

            for (int i = 0; i < sygnal.Length; i++)
            {
                wynik[i] = Complex.Abs(sygnal[i]);
            }
            
            return wynik;
        }
    }
}
