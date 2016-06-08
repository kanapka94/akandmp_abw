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
        /// Metoda sprawdzająca czy podana dokładność jest z zakresu 0-1
        /// </summary>
        /// <param name="dokladnosc"></param>
        public override void SprawdzDokladnosc(double dokladnosc)
        {
            if (dokladnosc <= 0 && dokladnosc > 1)
                throw new Exception("Dokładność musi być liczbą z zakresu 0-1.");
        }

        /// <summary>
        /// Metoda przeliczająca podaną dokładność
        /// </summary>
        /// <param name="dokladnosc"></param>
        public override int PrzeliczDokladnosc(double dokladnosc, int czestotliwoscProbkowania, double dolnaCzestosc = 0, double gornaCzestosc = 0)
        {
            double dokladnosc2 = 1 / dokladnosc; // Ilość wartości zwracanych dla jednej sekundy
            return czestotliwoscProbkowania* (int)dokladnosc2;
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
        public override Wynik WydzielPrzydzwiek(PlikWave plik, ref int stan, Okno okno, string plikPrzydzwieku, double dolnaCzestosc = 40, double gornaCzestosc = 60, double dokladnosc = 1, int poczatkowaSekunda = 0, int koncowaSekunda = -1)
        {

            if (koncowaSekunda == -1) koncowaSekunda = (int)plik.dlugoscWSekundach;
            SprawdzDlugosc((int)plik.dlugoscWSekundach, poczatkowaSekunda, koncowaSekunda);

            SprawdzDokladnosc(dokladnosc);
            if (dolnaCzestosc < 0) throw new Exception("Dolna częstotliwość poniżej 0");
            if (gornaCzestosc < 0) throw new Exception("Gorna częstotliwość poniżej 0");
            if (dolnaCzestosc > gornaCzestosc) throw new Exception("Górna częstotliwość jest mniejsza niż dolna częstotliwość");
        
            int iloscProbekDoRozszerzenia = PrzeliczDokladnosc(dokladnosc, plik.czestotliwoscProbkowania);

            Wynik wynik = new Wynik();
            wynik.czestotliwoscSygnalu = new double[(int)plik.dlugoscWSekundach];   // Tworzy tablicę wynik, której długość wynosi tyle
                                                                                    // ile sekund ma nagranie. Jest to spowodowane tym
                                                                                    // aby dla każdej sekundy wybrać najlepszy przydźwięk
            AnalizaLog.Dodaj("Parametry:", false);

            AnalizaLog.Dodaj("  Długość pliku: " + plik.dlugoscWSekundach.ToString(), false);
            AnalizaLog.Dodaj("  Wybrane okno: " + okno.ToString(), false);
            AnalizaLog.Dodaj("  Nazwa pliku przydźwięku: " + plikPrzydzwieku, false);
            AnalizaLog.Dodaj("  Dolna częstotliwość: " + dolnaCzestosc.ToString(), false);
            AnalizaLog.Dodaj("  Górna częstotliwość: " + gornaCzestosc.ToString(), false);
            AnalizaLog.Dodaj("  Dokładność: " + dokladnosc.ToString(), false);
            AnalizaLog.Dodaj("  Analiza nagania od : " + poczatkowaSekunda.ToString() + " do " + koncowaSekunda.ToString(), false);

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
            for (int i = 0; i < poczatkowaSekunda; i++)
            {
                plik.PobierzProbki();
            }
            for (int sekunda = poczatkowaSekunda; sekunda < koncowaSekunda; sekunda++)
            {
                widmo = ObliczWidmo(plik.PobierzProbki(), okno, iloscProbekDoRozszerzenia);
                rozmiarWidma = widmo.Length;
                double przydzwiek = ZnajdzPrzydzwiekWWidmie(widmo, plik.czestotliwoscProbkowania, rozmiarWidma, dolnaCzestosc, gornaCzestosc);

                try
                {
                    sw.WriteLine(przydzwiek);
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

        /// <summary>
        /// Metoda przygotowująca sygnał spróbkowany do FFT
        /// </summary>
        /// <param name="probki">sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <param name="OknoT">numer okna przez które przemnożymy sygnał</param>
        /// <returns>Zwraca zmieniony sygnał</returns>
        public static Complex[] PrzygotujDaneDoFFT(double[] probki, int dokladnosc,Okno okno)
        {
            AnalizaLog.Postep("Przygotowuję dane do FFT");
            Complex[] wynik;

            okno.Funkcja(probki);
            probki = WypelnijZerami(probki,dokladnosc);
            AnalizaLog.Postep("Przestawiam próbki");
            PrzestawienieProbek(probki);
            wynik = new Complex[probki.Length];

            AnalizaLog.Postep("Normalizuję próbki");
            for (int i = 0; i < probki.Length; i++)
            {
                wynik[i] = new Complex(probki[i] / (double)probki.Length, 0.0);//
            }

            return wynik;
        }

        /// <summary>
        /// Metoda uzupełnia sygnał zerami, tak aby jego długość była potęgą dwójki
        /// </summary>
        /// <param name="dane">spróbkowany sygnał</param>
        /// <returns>Zwraca rozszerzony sygnał</returns>
        public static Complex[] WypelnijZerami(Complex[] dane, int iloscProbek)
        {
            AnalizaLog.Postep("Dopełniam zerami");
            if ((dane.Length & (dane.Length - 1)) == 0) return dane;

            double log2;
            int log2_int;
            int i;
            Complex[] wynik;
            int k;

            //log2 = Math.Log((double)dane.Length) / Math.Log(2);
            log2 = Math.Log((double)iloscProbek) / Math.Log(2);
            log2_int = (int)Math.Round(log2);

            wynik = null;

            k = (int)Math.Pow(2.0, (double)log2_int);

            if (k < iloscProbek) log2_int++; //in case when the k is too small

            k = (int)Math.Pow(2.0, (double)log2_int);

            wynik = new Complex[k];

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
            AnalizaLog.Postep("Dopelniam zerami do potegi:");
            AnalizaLog.Dodaj(Convert.ToString(wynik.Length), false);

            return wynik;

        }//end of WypelnijZerami


        /// <summary>
        /// Metoda uzupełnia sygnał zerami, tak aby jego długość była potęgą dwójki
        /// </summary>
        /// <param name="dane">spróbkowany sygnał</param>
        /// <returns>Zwraca rozszerzony sygnał</returns>
        public static double[] WypelnijZerami(double[] dane, int iloscProbek)
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
        /// <param name="dolnyZakres">Nie dotyczy FFT</param>
        /// <param name="gornyZakres">Nie dotyczy FFT</param>
        /// <returns>Zwraca widmo sygnału</returns>
        public override double[] ObliczWidmo(double[] sygnal, Okno okno, int dokladnosc = -1, double dolnaCzestosc = -1, double gornaCzestosc = -1)
        {
            AnalizaLog.Postep("Obliczam widmo metodą FFt");
            Complex[] y = PrzygotujDaneDoFFT(sygnal,dokladnosc, okno);
            double[] wynik = new double[y.Length];
            AnalizaLog.Postep("Obliczam FFT");
            y = fftFor(y);
            AnalizaLog.Postep("Obliczam wynik");
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

            PrzestawienieProbek(x);

            y = fftFor(y);

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
        public static void PrzestawienieProbek(double[] probki)
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
          
        }

        /// <summary>
        /// Metoda przestawiająca próbki w sygnal spróbkowanym (przygotowuje dane do FFT)
        /// </summary>
        /// <param name="sygnal">Spróbkowany sygnał</param>
        /// <returns></returns>
        public static void PrzestawienieProbek(Complex[] sygnal)
        {
            int a = 1;
            int N = sygnal.Length; //ilość próbek w sygnale
            int c;

            Complex T;  //zmienna pomocnicza przechowująca wartość próbki

            for (int b = 1; b < N; b++)
            {
                if (b < a)
                {
                    T = sygnal[a - 1];
                    sygnal[a - 1] = sygnal[b - 1];
                    sygnal[b - 1] = T;
                }
                c = N / 2;
                while (c < a)
                {
                    a = a - c;
                    c = c / 2;
                }
                a = a + c;
            }

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

        /// <summary>
        /// Metoda odnajdująca przydźwięk w widmie wybierając element maksymalny.
        /// </summary>
        /// <param name="widmo">Widmo dźwięku</param>
        /// <param name="czestotliwoscProbkowania">Częstotliwość próbkowania</param>
        /// <param name="rozmiarWidma">Rozmiar widma</param>
        /// <param name="hzZakresDolny">Dolna granica badanych częstotliwości</param>
        /// <param name="hzZakresGorny">Górna granica badanych częstotliwości</param>
        /// <returns>Zwraca wartość przydźwięku sieciowego znalezionego w widmie</returns>
        public virtual double ZnajdzPrzydzwiekWWidmie(double[] widmo, int czestotliwoscProbkowania, int rozmiarWidma, double hzZakresDolny, double hzZakresGorny)
        {

            int indeksZakresDolny = hzNaIndeksWTablicy(hzZakresDolny, czestotliwoscProbkowania, rozmiarWidma);
            int indeksZakresGorny = hzNaIndeksWTablicy(hzZakresGorny, czestotliwoscProbkowania, rozmiarWidma);

            if (indeksZakresDolny >= widmo.Length || indeksZakresDolny < 0)
                throw new Exception("Dolny zakres widma w szukaniu przydźwięku nie mieści się w widmie");
            if (indeksZakresGorny >= widmo.Length || indeksZakresGorny < 0)
                throw new Exception("Górny zakres widma w szukaniu przydźwięku nie mieści się w widmie");

            double max = widmo[(int)indeksZakresDolny];
            int indeksMax = (int)indeksZakresDolny;

            AnalizaLog.Dodaj("--- częstotliwość   |   wartości widma ---------------");

            for (int index = (int)indeksZakresDolny + 1; index <= indeksZakresGorny; index++)
            {
                if (widmo[index] > max)
                {
                    max = widmo[index];
                    indeksMax = index;
                }
                AnalizaLog.Dodaj(indeksTablicyNaHz(indeksMax, czestotliwoscProbkowania, rozmiarWidma).ToString() + " " + widmo[index].ToString(), false);
            }
            return indeksTablicyNaHz(indeksMax, czestotliwoscProbkowania, rozmiarWidma);
        }

        /// <summary>
        /// Metoda zwracająca indeks częstotliwości
        /// </summary>
        /// <param name="hz">Częstotliwość, której chcemy znaleźć indeks</param>
        /// <param name="czestotliwoscProbkowania">Częstotliwość próbkowania</param>
        /// <param name="rozmiarWidma">Rozmiar widma</param>
        /// <returns>Zwraca indeks danej częstotliwości w tablicy</returns>
        public int hzNaIndeksWTablicy(double hz, double czestotliwoscProbkowania, double rozmiarWidma)
        {
            return (int)Math.Round(((decimal)hz * (decimal)rozmiarWidma / (decimal)czestotliwoscProbkowania));
        }

        /// <summary>
        /// Metoda zwracająca wartość częstotliwości pod podanym indeksem
        /// </summary>
        /// <param name="indeks">Indeks elementu tablicy</param>
        /// <param name="czestotliwoscProbkowania">Częstotliwość próbkowania</param>
        /// <param name="rozmiarWidma">Rozmiar widma</param>
        /// <returns>Zwraca wartość częstotliwości ukrytej pod danym indeksem w tablicy</returns>
        public double indeksTablicyNaHz(int indeks, double czestotliwoscProbkowania, decimal rozmiarWidma)
        {
            return (double)((decimal)indeks * (decimal)czestotliwoscProbkowania / (decimal)rozmiarWidma);
        }

        /// <summary>
        /// Metoda zwracająca Spektrogram nagrania
        /// </summary>
        /// <param name="plik">Obiekt klasy PlikWabe z nagraniem</param>
        /// <param name="okno">Zastosowane okno</param>
        /// <param name="skokCzestotliwosc">Co ile ma liczyć częstotliwość (FFT może zmniejszyć wartość)</param>
        /// <param name="skokCzas">Co jaką wartość sekundy zmienia się indeks tablicy</param>
        /// <returns>Spektrogram nagrania</returns>
        public override Spektrogram ObliczSpektrogram(PlikWave plik, Okno okno, double skokCzestotliwosc, double skokCzas)
        {
            bool czyZmniejszycWynik = false;
            double oIleZmniejszyc = 0;

            if (skokCzestotliwosc > 1)
            {
                czyZmniejszycWynik = true;
                oIleZmniejszyc = skokCzestotliwosc;
                skokCzestotliwosc = 1;
            }

            Spektrogram wynik = new Spektrogram(skokCzestotliwosc, skokCzas);

            plik.Przeladuj();

            int iloscWartosciWCzasie = (int)(Math.Floor(plik.dlugoscWSekundach) / skokCzas);
            int iloscProbekNaJednostkeCzasu = (int)(plik.czestotliwoscProbkowania * skokCzas);
            int iloscProbekDoRozszerzenia = (int)((double)plik.czestotliwoscProbkowania / skokCzestotliwosc);

            wynik.modulWidma = new double[iloscWartosciWCzasie][];

            for (int sekunda = 0; sekunda < iloscWartosciWCzasie; sekunda++)
            {
                double[] widmo = ObliczWidmo(plik.PobierzProbki(0, iloscProbekNaJednostkeCzasu), okno, iloscProbekDoRozszerzenia);
                iloscProbekDoRozszerzenia = widmo.Length;
                if (czyZmniejszycWynik)
                {
                    wynik.modulWidma[sekunda] = new double[(int)(widmo.Length / oIleZmniejszyc)];
                    for (int i = 0; i < ((double)widmo.Length) / oIleZmniejszyc; i++)
                    {
                        wynik.modulWidma[sekunda][i] = widmo[(int)(i * oIleZmniejszyc)];
                    }
                }
                else
                    wynik.modulWidma[sekunda] = (double[])widmo.Clone();
            }

            return wynik;
        }
    }
}

