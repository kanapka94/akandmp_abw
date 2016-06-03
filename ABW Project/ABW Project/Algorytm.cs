//autorzy: Michał Paduch i Adam Konopka
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
    /// Klasa, którą dziedziczą klasy FFT, CZT oraz ESPRIT
    /// </summary>
    abstract class Algorytm
    {
        /// <summary>
        /// Metoda zamieniająca dokładność z postaci liczby zmiennoprzecinkowej (0,002) na ilość próbek potrzebnych do osiągnięcia takiej dokładności.
        /// </summary>
        /// <param name="dokladnosc">Dokładnośc podana przez użytkownika</param>
        /// <param name="czestotliwoscProbkowania">Częstotliwość próbkowania nagrania</param>
        /// <returns>Zwraca ilość potrzebnych próbek</returns>
        public int PrzeliczDokladnosc(double dokladnosc,int czestotliwoscProbkowania)
        {
            double dokladnosc2 = 1 / dokladnosc; // Ilość wartości zwracanych dla jednej sekundy
            return czestotliwoscProbkowania * (int)dokladnosc2;
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
        public virtual Wynik WydzielPrzydzwiek(PlikWave plik,ref int stan, Okno okno, string plikPrzydzwieku, double dolnaCzestosc = 40, double gornaCzestosc = 60, double dokladnosc = 1)
        { 
            if (dokladnosc < 0 || dokladnosc > 1)
                throw new Exception("Dokładność musi być liczbą z zakresu 0-1.");
            if (dolnaCzestosc < 0) throw new Exception("Dolna częstotliwość poniżej 0");
            if (gornaCzestosc < 0) throw new Exception("Gorna częstotliwość poniżej 0");
            if (dolnaCzestosc > gornaCzestosc) throw new Exception("Górna częstotliwość jest mniejsza niż dolna częstotliwość");

            int iloscProbekDoRozszerzenia = PrzeliczDokladnosc(dokladnosc, plik.czestotliwoscProbkowania);

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
                widmo = ObliczWidmo(plik.PobierzProbki(), okno, iloscProbekDoRozszerzenia);
                rozmiarWidma = widmo.Length;
                double przydzwiek = ZnajdzPrzydzwiekWWidmie(widmo,plik.czestotliwoscProbkowania,rozmiarWidma,dolnaCzestosc,gornaCzestosc);

                try
                {
                    sw.WriteLine("{0} {1}", sekunda, przydzwiek);
                }
                catch(Exception ex)
                {
                    throw ex;
                }

                wynik.czestotliwoscSygnalu[sekunda] = przydzwiek;
                stan = (int)((double)(sekunda+1) / (double)wynik.czestotliwoscSygnalu.Length * 100.0);
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
        /// Metoda wirtualna. Jej zadaniem jest obliczanie widma sygnału dźwiękowego
        /// </summary>
        /// <param name="sygnal">Sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca tablicę wartości widma</returns>
        public abstract double[] ObliczWidmo(double[] sygnal, Okno okno, int dokladnosc);

        // Metoda odnajdująca przydźwięk w widmie wybierając element maksymalny. 
        /*public virtual int ZnajdzPrzydzwiekWWidmie(double[] widmo, int indeksZakresDolny, int indeksZakresGorny, int czestotliwoscProbkowania, int rozmiarWidma)
        {
            if (indeksZakresDolny == -1) indeksZakresDolny = 0;
            if (indeksZakresGorny == -1) indeksZakresGorny = widmo.Length - 1;

            double max = widmo[indeksZakresDolny];
            int indeksMax = indeksZakresDolny;
            for (int index = indeksZakresDolny+1; index <= indeksZakresGorny; index++)
            {
                if (widmo[index] > max)
                {
                    max = widmo[index];
                    indeksMax = index;
                }
            }
            return 
        }*/

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

            int indeksZakresDolny = hzNaIndeksWTablicy(hzZakresDolny,czestotliwoscProbkowania,rozmiarWidma);
            int indeksZakresGorny = hzNaIndeksWTablicy(hzZakresGorny, czestotliwoscProbkowania, rozmiarWidma);

            if (indeksZakresDolny >= widmo.Length || indeksZakresDolny < 0)
                throw new Exception("Dolny zakres widma w szukaniu przydźwięku nie mieści się w widmie");
            if (indeksZakresGorny >= widmo.Length || indeksZakresGorny < 0)
                throw new Exception("Górny zakres widma w szukaniu przydźwięku nie mieści się w widmie");

            double max = widmo[(int)indeksZakresDolny];
            int indeksMax = (int)indeksZakresDolny;
            for (int index = (int)indeksZakresDolny + 1; index <= indeksZakresGorny; index++)
            {
                if (widmo[index] > max)
                {
                    max = widmo[index];
                    indeksMax = index;
                }
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
        public int hzNaIndeksWTablicy(double hz,double czestotliwoscProbkowania, double rozmiarWidma)
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
    }
}
