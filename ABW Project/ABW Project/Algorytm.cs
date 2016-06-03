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
        /// <param name="dolnaCzestosc">Dolna częstotliwość (graniczna, badana)</param>
        /// <param name="gornaCzestosc">Górna częstotliwość (graniczna, badana)</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca obiekt klasy Wynik</returns>
        public virtual Wynik WydzielPrzydzwiek(PlikWave plik,ref int stan, Okno okno, string plikWidma, double dolnaCzestosc = 40, double gornaCzestosc = 60, double dokladnosc = 1)
        {


            // Kod testujący do DFT =========================================================
            // ******************************************************************************

            /********************************************************************************
            *
            *    Opis działania algorytmu
             *    
             *  Do metody wydziel przydzwiek podajemy nastepujace wartosci: 
             *  > plik - obiekt klasy PlikWave przechowywujący metody pobierania próbek i nagłówków
             *  > ref stan - referencja do zmiennej int wskazującej na aktualny stan wykonanej roboty
             *  Wartości są w procentach więc powinny być z zakresu 0-100
             *  > dolnaCzestosc - czestosc minimalna jaka ma byc wyszukiwana
             *  > gornaCzestosc - czestosc maksymalna jaka ma byc wyszukana
             *  > dokladnosc - ilosc probek, na których ma opierać się algorytm
             *  
             * Metoda na początku tworzy obiekt typu wynik z tablicą double o długości ilości sekund w nagraniu.
             * Dla każdej sekundy wydzielony przydźwięk zapisze się w tablicy double.
             * Następnie dla każdej sekundy oblicza widmo korzystajac z metody pobierzProbki, oraz podanej dokladnosci
             * 
             * 
            *
            ********************************************************************************/

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
                sw = new StreamWriter(plikWidma); // Tymczasowe tylko do zapisu wyników widma
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
                sw.WriteLine("{0} {1}", sekunda, przydzwiek);
                wynik.czestotliwoscSygnalu[sekunda] = przydzwiek;
                stan = (int)((double)(sekunda+1) / (double)wynik.czestotliwoscSygnalu.Length * 100.0);
            }
            sw.Close();  // Tymczasowe tylko do zapisu wyników widma

            return wynik;

            // ******************************************************************************
            // Kod testujący do DFT =========================================================a
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
