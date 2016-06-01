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
        /// Metoda wydzielająca wartość przydźwięku sieciowego co sekundę
        /// </summary>
        /// <param name="plik">Obiekt klasy PlikWave</param>
        //todo: Padi uzupełnij tutaj:
        /// <param name="stan">...</param>
        /// <param name="dolnaCzestosc">Dolna częstotliwość (graniczna, badana)</param>
        /// <param name="gornaCzestosc">Górna częstotliwość (graniczna, badana)</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca obiekt klasy Wynik</returns>
        public virtual Wynik WydzielPrzydzwiek(PlikWave plik,ref int stan, Okno okno,double dolnaCzestosc = 40, double gornaCzestosc = 60, int dokladnosc = -1)
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

            if (dokladnosc == -1) dokladnosc = plik.czestotliwoscProbkowania;       // Jeżeli dokladnosc == -1 to wynosi inaczej ilosci probek w pliku

            Wynik wynik = new Wynik();
            wynik.czestotliwoscSygnalu = new double[(int)plik.dlugoscWSekundach];   // Tworzy tablicę wynik, której długość wynosi tyle
                                                                                    // ile sekund ma nagranie. Jest to spowodowane tym
                                                                                    // aby dla każdej sekundy wybrać najlepszy przydźwięk
            StreamWriter sw = new StreamWriter("plik.txt"); // Tymczasowe tylko do zapisu wyników widma

            int rozmiarWidma;
            for (int i = 0; i < wynik.czestotliwoscSygnalu.Length; i++)
            {
                double[] widmo = ObliczWidmo(plik.PobierzProbki(), okno, dokladnosc);
                rozmiarWidma = widmo.Length;

                double przydzwiek = ZnajdzPrzydzwiekWWidmie(widmo,plik.czestotliwoscProbkowania,rozmiarWidma,dolnaCzestosc,gornaCzestosc);

                //  =========================================================================================================
                sw.WriteLine();
                sw.WriteLine("================================================================================");
                sw.WriteLine("=> Widmo dla "+Convert.ToString(i+1)+" sekundy");          // Tymczasowe tylko do zapisu widma|
                sw.WriteLine("> Dokładność = " + Convert.ToString((decimal)plik.czestotliwoscProbkowania / (decimal)rozmiarWidma));
                sw.WriteLine("50 Hz na pozycji: " + hzNaIndeksWTablicy(50, plik.czestotliwoscProbkowania, rozmiarWidma));
                sw.WriteLine("================================================================================");
                sw.WriteLine();
                for (int j = 0; j < widmo.Length; j++)                                   //                                 |
                {   
                    if(hzNaIndeksWTablicy(dolnaCzestosc,plik.czestotliwoscProbkowania,rozmiarWidma) < j)//
                        if (hzNaIndeksWTablicy(gornaCzestosc, plik.czestotliwoscProbkowania, rozmiarWidma) > j)
                            sw.WriteLine(Convert.ToString(j + " " + Math.Round((decimal)j * (decimal)plik.czestotliwoscProbkowania / (decimal)rozmiarWidma,3) + "Hz -> " + widmo[j] + dolnaCzestosc));                                   //                                 | 
                }                                                                        //                                 |       
                sw.WriteLine("Znaleziony Przydźwięk: " + Convert.ToString(przydzwiek));  //                                 |          
                //  ========================================================================================================= 
                wynik.czestotliwoscSygnalu[i] = przydzwiek;
                stan = (int)((double)(i+1) / (double)wynik.czestotliwoscSygnalu.Length * 100.0);
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
        public abstract double[] ObliczWidmo(int[] sygnal, Okno okno, int dokladnosc);

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
