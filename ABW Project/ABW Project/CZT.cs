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
        /// <summary>
        /// Metoda obliczacąca potrzebną ilość prążków do uzyskania podanej dokładności
        /// </summary>
        /// <param name="dokladnosc">Dokładność w Hz (np. 0.02)</param>
        /// <param name="czestotliwoscProbkowania">Częstotliwość próbkowania</param>
        /// <param name="dolnaCzestosc">Graniczna, dolna częstotliwość</param>
        /// <param name="gornaCzestosc">Graniczna, górna częstotliwość</param>
        /// <returns></returns>
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
        public override Wynik WydzielPrzydzwiek(PlikWave plik, ref int stan, Okno okno, string plikPrzydzwieku, double dolnaCzestosc = 40, double gornaCzestosc = 60, double dokladnosc = 1, int poczatkowaSekunda = 0, int koncowaSekunda = -1)
        {
            if (koncowaSekunda == -1) koncowaSekunda = (int)plik.dlugoscWSekundach;
            SprawdzDlugosc((int)plik.dlugoscWSekundach, poczatkowaSekunda, koncowaSekunda);

            AnalizaLog.Postep("Rozpoczynam metodę 'WydzielPrzydzwiek'");
            AnalizaLog.Dodaj("Parametry:",false);

            AnalizaLog.Dodaj("  Długość pliku: "+plik.dlugoscWSekundach.ToString(),false);
            AnalizaLog.Dodaj("  Wybrane okno: " + okno.ToString(), false);
            AnalizaLog.Dodaj("  Nazwa pliku przydźwięku: " + plikPrzydzwieku, false);
            AnalizaLog.Dodaj("  Dolna częstotliwość: " + dolnaCzestosc.ToString(), false);
            AnalizaLog.Dodaj("  Górna częstotliwość: " + gornaCzestosc.ToString(), false);
            AnalizaLog.Dodaj("  Dokładność: " + dokladnosc.ToString(), false);
            AnalizaLog.Dodaj("  Analiza nagania od : " + poczatkowaSekunda.ToString() + " do " + koncowaSekunda.ToString(), false);


            SprawdzDokladnosc(dokladnosc);  // ?
            if (dolnaCzestosc < 0)
            {
                AnalizaLog.Blad("Dolna częstotliwość poniżej 0");
                throw new Exception("Dolna częstotliwość poniżej 0");
            }
            if (gornaCzestosc < 0)
            {
                AnalizaLog.Blad("Gorna częstotliwość poniżej 0");
                throw new Exception("Gorna częstotliwość poniżej 0");
            }
            if (dolnaCzestosc > gornaCzestosc)
            {
                AnalizaLog.Blad("Górna częstotliwość jest mniejsza niż dolna częstotliwość");
                throw new Exception("Górna częstotliwość jest mniejsza niż dolna częstotliwość");
            }

            int iloscPrazkow = PrzeliczDokladnosc(dokladnosc, plik.czestotliwoscProbkowania,dolnaCzestosc,gornaCzestosc);
            AnalizaLog.Postep("Ilość prążków: "+iloscPrazkow.ToString());


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
                AnalizaLog.Blad(e.Message);
                throw e;
            }

            AnalizaLog.Postep("Obliczam widmo");
            int rozmiarWidma;
            for (int i = 0; i < poczatkowaSekunda; i++)
            {
                plik.PobierzProbki();
            }
            for (int sekunda = poczatkowaSekunda; sekunda < koncowaSekunda; sekunda++)
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
                        
                        sw.WriteLine("{0} {1}", i*dokladnosc + dolnaCzestosc, widmo[i]);
                    }
                    //sw.WriteLine("{0} {1}", sekunda, przydzwiek);
                }
                catch (Exception ex)
                {
                    AnalizaLog.Blad(ex.Message);
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
                AnalizaLog.Blad(ex.Message);
                throw ex;
            }


            AnalizaLog.Postep("Wydzielanie przydźwięku zakończone.");
            return wynik;
        }

        /// <summary>
        /// Metoda odnajdująca przydźwięk w widmie wybierając element maksymalny.
        /// </summary>
        /// <param name="widmo">Widmo sygnału dźwiękowego</param>
        /// <returns>Zwraca maksymalny wartość przydźwięku sieciowego</returns>
        public int ZnajdzPrzydzwiekWWidmie2(double[] widmo)
        {
            AnalizaLog.Postep("Uruchamiam metodę 'ZnajdzPrzydzwiekWWidmie2'");
            AnalizaLog.Postep("Znajduję przydźwięk w widmie");
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

            AnalizaLog.Postep("Znajdowanie przydźwięku zakończone");
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
            AnalizaLog.Postep("Rozpoczynam metodę 'czt'");
            AnalizaLog.Dodaj("Parametry:", false);

            AnalizaLog.Dodaj("  Ilość próbek: " + probki.Length.ToString(), false);
            AnalizaLog.Dodaj("  Częstotliwość próbkowania: " + czestoscProbkowania.ToString(), false);
            AnalizaLog.Dodaj("  Ilość prążków: " + iloscPrazkow.ToString(), false);
            AnalizaLog.Dodaj("  Dolna częstotliwość: " + czestoscDolna.ToString(), false);
            AnalizaLog.Dodaj("  Górna częstotliwość: " + czestoscGorna.ToString(), false);

            int N = probki.Length;
             int NM1 = N + iloscPrazkow - 1;
             //Complex A = Complex.Exp(Complex.ImaginaryOne * 2 * Math.PI * czestoscDolna / czestoscProbkowania);
             //Complex W = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * (czestoscGorna - czestoscDolna) / (iloscPrazkow * czestoscProbkowania));

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
            */

            /*
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

            FFT.PrzestawienieProbek(y1);
            FFT.PrzestawienieProbek(y2);

            Complex[] Y1 = FFT.fftFor(y1);
            Complex[] Y2 = FFT.fftFor(y2);
            Complex[] Y = new Complex[Y2.Length];

            for (int i = 0; i < Y2.Length; i++)
            {
                Y[i] = Complex.Multiply(Y1[i], Y2[i]);
                //Y[i] = Y1[i] * Y2[i];
            }

            Complex[] y = new Complex[Y.Length];
            Complex[] pom = FFT.ifft(Y);

            for (int i = 0; i < pom.Length; i++)
            {
                y[i] = pom[i] / (N / 2);
            }

            Complex[] XcztN = new Complex[iloscPrazkow];

            for (k = 0; k < iloscPrazkow; k++)
            {
                XcztN[k] = y[k] * Complex.Pow(W, Math.Pow(k, 2));
            }

            AnalizaLog.Postep("CZT zakończone.");
            return XcztN;
        }

        /// <summary>
        /// Metoda "okienkuje" sygnał
        /// </summary>
        /// <param name="probki">Sygnał spróbkowany</param>
        /// <param name="OknoT">Numer okna przez które przemnożymy sygnał</param>
        public void PrzygotujDaneDoCZT(double[] probki, Okno okno)
        {
            AnalizaLog.Postep("Uruchamiam metodę 'PrzygotujDaneDoCZT'");
            okno.Funkcja(probki);
            AnalizaLog.Postep("Zakończone przygotowywanie danych.");
        }

        /// <summary>
        /// Metoda licząca widmo
        /// </summary>
        /// <param name="probki">Sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca widmo sygnału</returns>
        public override double[] ObliczWidmo(double[] probki, Okno okno, int iloscPrazkow, double dolnaCzestosc, double gornaCzestosc)
        {
      

            AnalizaLog.Postep("Uruchamiam metodę 'ObliczWidmo'");
            PrzygotujDaneDoCZT(probki, okno);

            double[] wynik = new double[iloscPrazkow];
            Complex[] sygnal = czt(probki, probki.Length, iloscPrazkow, dolnaCzestosc, gornaCzestosc);

            for (int i = 0; i < sygnal.Length; i++)
            {
                wynik[i] = Complex.Abs(sygnal[i]);
            }

            AnalizaLog.Postep("Widmo obliczone - koniec metody");
            return wynik;
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
            Spektrogram wynik = new Spektrogram(skokCzestotliwosc, skokCzas);

            /* ======================================== DOPISAC GDY U FFT BĘDZIE DZIAŁAC !!!!!!!!!!!!!!!!!!!!!!!1
            bool czyZmniejszycWynik = false;
            double oIleZmniejszyc = 0;

            if (skokCzestotliwosc > 1)
            {
                czyZmniejszycWynik = true;
                oIleZmniejszyc = skokCzestotliwosc;
                skokCzestotliwosc = 1;
            }
            
            

            plik.Przeladuj();

            int iloscWartosciWCzasie = (int)(Math.Floor(plik.dlugoscWSekundach) / skokCzas);
            int iloscPrazkow = PrzeliczDokladnosc(skokCzestotliwosc, plik.czestotliwoscProbkowania, 0, plik.czestotliwoscProbkowania / 2);

            wynik.modulWidma = new double[iloscWartosciWCzasie][];

            for (int sekunda = 0; sekunda < iloscWartosciWCzasie; sekunda++)
            {
                double[] widmo = ObliczWidmo(plik.PobierzProbki(), okno, iloscPrazkow, 0,plik.czestotliwoscProbkowania / 2);
                if(czyZmniejszycWynik)
                {
                    wynik.modulWidma[sekunda] = new double[(int)(widmo.Length / oIleZmniejszyc)];
                    for (int i = 0; i < ((double)widmo.Length)/ oIleZmniejszyc; i++)
                    {
                        wynik.modulWidma[sekunda][i] = widmo[(int)(i * oIleZmniejszyc)];
                    }
                }
                else
                    wynik.modulWidma[sekunda] = (double[])widmo.Clone();
            }*/

            return wynik;
        }
    }
}
