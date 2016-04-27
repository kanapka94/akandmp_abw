using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ABW_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            WaveFile wv = new WaveFile();
            wv.WczytajZPliku("plik.wav");
            Console.WriteLine("Riff id: " + wv.idRiff);
            Console.WriteLine("Rozmiar pliku (bez 8 bajtów początkowych): " + Convert.ToString(wv.rozmiarPliku) + "B");
            Console.WriteLine("Całkowity rozmiar: " + Convert.ToString(wv.CalkowityRozmiar) + "B");
            Console.WriteLine("Id formatu: " + wv.idFormatu);
            Console.WriteLine("Id opisu: " + wv.idOpisu);
            Console.WriteLine("Rozmiar opisu: " + Convert.ToString(wv.rozmiarOpisu));
            Console.WriteLine("Rodzaj kompresji: " + wv.rodzajKompresji);
            Console.WriteLine("Rodzaj kompresji opis: " + wv.RodzajKompresjiOpis);
            Console.WriteLine("Ilość kanałów: " + Convert.ToString(wv.iloscKanalow));
            Console.WriteLine("Częstotliwość próbkowania: " + Convert.ToString(wv.czestotliwoscProbkowania));
            Console.WriteLine("Częstotliwość bajtów: " + Convert.ToString(wv.czestotliwoscBajtow));
            Console.WriteLine("Rozmiar próbki: " + Convert.ToString(wv.rozmiarProbki));
            Console.WriteLine("Rozdzielczość: " + Convert.ToString(wv.rozdzielczosc));
            Console.WriteLine("Rozmiar danych: " + Convert.ToString(wv.rozmiarDanych));
            Console.WriteLine("Ilość próbek: " + Convert.ToString(wv.iloscProbek));
            Console.WriteLine("Długość w sekundach: " + Convert.ToString(wv.dlugoscWSekundach));

            /*for (int i = 0; i < 10000; i++)
            {
                Console.Write("{0} ", wv.NastepnaProbka());
                Thread.Sleep(10);
            }*/
            

            Console.ReadKey();
        }
    }
}
