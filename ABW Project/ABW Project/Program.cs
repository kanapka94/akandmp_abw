using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ABW_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            PlikWave wv = new PlikWave();
            wv.WczytajZPliku("plik.wav");

            Console.WriteLine("\n ========== Nagłówki ========== \n");

            Console.WriteLine(" Riff id: " + wv.idRiff);
            Console.WriteLine(" Rozmiar pliku (bez 8 bajtów początkowych): " + Convert.ToString(wv.rozmiarPliku) + "B");
            Console.WriteLine(" Całkowity rozmiar: " + Convert.ToString(wv.CalkowityRozmiar) + "B");
            Console.WriteLine(" Id formatu: " + wv.idFormatu);
            Console.WriteLine(" Id opisu: " + wv.idOpisu);
            Console.WriteLine(" Rozmiar opisu: " + Convert.ToString(wv.rozmiarOpisu));
            Console.WriteLine(" Rodzaj kompresji: " + wv.rodzajKompresji);
            Console.WriteLine(" Rodzaj kompresji opis: " + wv.RodzajKompresjiOpis);
            Console.WriteLine(" Ilość kanałów: " + Convert.ToString(wv.iloscKanalow));
            Console.WriteLine(" Częstotliwość próbkowania: " + Convert.ToString(wv.czestotliwoscProbkowania));
            Console.WriteLine(" Częstotliwość bajtów: " + Convert.ToString(wv.czestotliwoscBajtow));
            Console.WriteLine(" Rozmiar próbki: " + Convert.ToString(wv.rozmiarProbki));
            Console.WriteLine(" Rozdzielczość: " + Convert.ToString(wv.rozdzielczosc));
            Console.WriteLine(" Rozmiar danych: " + Convert.ToString(wv.rozmiarDanych));
            Console.WriteLine(" Ilość próbek: " + Convert.ToString(wv.iloscProbek));
            Console.WriteLine(" Długość w sekundach: " + Convert.ToString(wv.dlugoscWSekundach));
            
            Console.WriteLine("\n ========== DFT ========== \n");

            Dft dft = new Dft();

            double index = 49.001;

            StreamWriter sw = new StreamWriter("dupa.txt");

            foreach (double item in dft.WydzielPrzydzwiek(wv).czestotliwoscSzumow)
            {
                sw.Write(index);
                sw.Write(" ");
                sw.WriteLine((double)item);
                index += 0.001;
            }
            Console.WriteLine("Koniec");
            sw.Close();

            /*
             * Testujący DFT
            double[] S = new double[100];

            for (int t = 0; t < 100; t++)
            {
                S[t] = Math.Sin(2 * Math.PI * 3 * t / 100);
            }

            Dft dft = new Dft();

            int index = 0;

            foreach (double item in dft.ObliczWidmo(S))
            {
                Console.WriteLine("{0} {1} ", index++, item);
            }
            */
            Console.ReadKey();
        }
    }
}
