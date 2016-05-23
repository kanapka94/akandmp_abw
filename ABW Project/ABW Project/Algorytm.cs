using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

namespace ABW_Project
{
    class Algorytm
    {
        public Wynik WydzielPrzydzwiek(PlikWave plik,ref int stan,double dolnaCzestosc = 40, double gornaCzestosc = 60, int dokladnosc = -1)
        {
            /*Wynik wynik = new Wynik();

              === FFT
             * 
             * double[] wynik2 = plik.PobierzProbki();
            wynik.czestotliwoscSygnalu = new double[2000000];

            for (int i = 0; i < wynik.czestotliwoscSygnalu.Length; i++)
            {
                wynik.czestotliwoscSygnalu[i] = 0;
            }

            for (int i = 0; i < wynik2.Length; i++)
            {
                wynik.czestotliwoscSygnalu[i] = wynik2[i];
            }

            wynik.czestotliwoscSygnalu = ObliczWidmo(wynik.czestotliwoscSygnalu);

            return wynik;
            */
            
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
                double[] widmo = ObliczWidmo(plik.PobierzProbki(),dokladnosc);
                rozmiarWidma = widmo.Length;

                double przydzwiek = ZnajdzPrzydzwiekWWidmie(widmo,plik.czestotliwoscProbkowania,rozmiarWidma,dolnaCzestosc,gornaCzestosc);

                //  =========================================================================================================
                sw.WriteLine("=> Widmo dla "+Convert.ToString(i+1)+" sekundy");          // Tymczasowe tylko do zapisu widma|
                sw.WriteLine("> Dokładność = " + Convert.ToString((decimal)plik.czestotliwoscProbkowania / (decimal)rozmiarWidma));
                for (int j = 0; j < widmo.Length; j++)                                   //                                 |
                {                                                                        //                                 |
                    sw.WriteLine(Convert.ToString((double)j * (double)plik.czestotliwoscProbkowania / (double)rozmiarWidma + "Hz -> " + widmo[j] + dolnaCzestosc));                                   //                                 | 
                }                                                                        //                                 |       
                sw.WriteLine("Znaleziony Przydźwięk: " + Convert.ToString(przydzwiek));  //                                 |          
                //  ========================================================================================================= 
                wynik.czestotliwoscSygnalu[i] = przydzwiek;
                stan = i / wynik.czestotliwoscSygnalu.Length * 100;
            }
            sw.Close();  // Tymczasowe tylko do zapisu wyników widma

            return wynik;

            // ******************************************************************************
            // Kod testujący do DFT =========================================================a
        }

        public virtual double[] ObliczWidmo(int[] sygnal, int dokladnosc)
        {
            // Algorytm powinien być nadpisany

            return null;
        }

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

        // Metoda odnajdująca przydźwięk w widmie wybierając element maksymalny. 
        public virtual double ZnajdzPrzydzwiekWWidmie(double[] widmo, int czestotliwoscProbkowania, int rozmiarWidma, double hzZakresDolny, double hzZakresGorny)
        {

            int indeksZakresDolny = indeksHzWTablicy(hzZakresDolny,czestotliwoscProbkowania,rozmiarWidma)+1;
            int indeksZakresGorny = indeksHzWTablicy(hzZakresGorny, czestotliwoscProbkowania, rozmiarWidma) + 1;

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

        public int indeksHzWTablicy(double hz,double czestotliwoscProbkowania, double rozmiarWidma)
        {
            return (int)Math.Round((hz * (double)rozmiarWidma / (double)czestotliwoscProbkowania));
        }

        public double indeksTablicyNaHz(int indeks, double czestotliwoscProbkowania, double rozmiarWidma)
        {
            return (double)(indeks * (double)czestotliwoscProbkowania) / (double)rozmiarWidma;
        }
    }
}
