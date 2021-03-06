﻿//autorzy: Michał Paduch i Adam Konopka
//Wyżej wymienieni autorzy udostępniają cały powyższy/poniższy kod zawarty w niniejszym pliku 
//na zasadach licencji GNU GPLv2.http://www.gnu.org/licenses/old-licenses/gpl-2.0.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Numerics;

namespace ABW_Project
{
    public class Stan
    {

        int cX; // Pozycja kursora w consoli
        int cY;
        public int stan; // Stan wykonanej roboty

        int kX; // Pozycja animowanego znaku pasek
        int kY;

        int animacja; // Poziom Animacji
        Thread animWatek;

        Stopwatch sw = new Stopwatch();

        public void Init(int x, int y)
        {
            cX = x;
            cY = y;
            kX = 10;
            kY = cY + 1;
            stan = 0;
            animacja = 0;
        }

        public void AnimacjaKursora()
        {
            while (true)
            {
                kX = 10 + stan / 20;
                rysujStan();
                if (stan >= 100)
                {
                    animWatek.Abort();
                }
                
                if (stan < 100)
                {
                    Console.SetCursorPosition(kX, kY);
                    switch (animacja)
                    {
                        case 0:
                            Console.Write("|");
                            break;
                        case 1:
                            Console.Write("/");
                            break;
                        case 2:
                            Console.Write("-");
                            break;
                        case 3:
                            Console.Write("\\");
                            break;
                    }
                    animacja++;
                    if (animacja == 4) animacja = 0;
                }
                Thread.Sleep(200);
            }
        }
        public void rysujStan()
        {
            Console.SetCursorPosition(cX, cY);
            Console.WriteLine(" Czas: {0} s        ", (int)(sw.ElapsedMilliseconds/1000));
            Console.Write(" Postęp: [");
            for (int i = 0; i < stan / 20; i++)
            {
                Console.Write("+");
            }
            if(stan < 100)
            Console.Write(" ");
            for (int i = stan / 20 + 1; i < 5; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine("] {0}%           ", stan);
        }

        public void Rozpocznij()
        {
            animWatek = new Thread(new ThreadStart(AnimacjaKursora));
            animWatek.Start();
            while (!animWatek.IsAlive) ;

            Console.CursorVisible = false;
            sw.Reset();
            sw.Start();

        }

        public void Zakoncz()
        {
            animWatek.Abort();
            Console.SetCursorPosition(1, kY);
            Console.WriteLine("Postęp: [+++++] 100%       ");
            Console.WriteLine();
            Console.WriteLine();
            Console.SetCursorPosition(cX, cY + 3);
            Console.CursorVisible = true;

            sw.Stop();
            
        }

        public double Sekundy
        {
            get
            {
                return (double)sw.ElapsedMilliseconds / 1000;
            }
        }
    };

    class Program
    {
        static void Main(string[] args)
        {
            PlikWave wv = new PlikWave();

            Console.WriteLine("Podaj ścieżkę do pliku .wav");

            string ścieżka = Console.ReadLine();

            //wv.WczytajZPliku("plik.wav");
            wv.WczytajZPliku(ścieżka);



            //AnalizaLog.Dodaj("-------------------- Nowa analiza -----------------------");                    

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
            
            Stan stan;
            //DFT dft;
            //FFT fft;
            int index = 0;
            double[] wynik;

            /* DFT działa choć ze względu na dokładność na razie jest wyłączone z używania
             * 
            Console.WriteLine("\n ========== DFT ========== \n");
            Console.WriteLine(" Analiza...\n");
            stan = new Stan();
            dft = new DFT();

            stan.Init(Console.CursorLeft, Console.CursorTop);
            stan.Rozpocznij();
            index = 0;

            AnalizaLog.Dodaj("Rozpoczęcie analizy DFT");
            wynik = dft.WydzielPrzydzwiek(wv, ref stan.stan).czestotliwoscSygnalu;

            AnalizaLog.Dodaj("Zakończenie analizy");
            stan.Zakoncz();

            Console.WriteLine("> Wynik FFT");
            Console.WriteLine();
            AnalizaLog.Dodaj("Wyniki:", false);
            foreach (double item in wynik)
            {
                Console.WriteLine(" W {0} sekundzie {1} hz", ++index, item);
                AnalizaLog.Dodaj(" w " + index + " sekundzie " + item + " hz");
            }
            */
            Console.WriteLine("\n ========== FFT ========== \n");

            wv.ZamknijPlik();
            wv = new PlikWave();
            //wv.WczytajZPliku("plik.wav");
            wv.WczytajZPliku(ścieżka);

            Console.WriteLine(" Analiza...\n");
            stan = new Stan();
            //fft = new FFT();

            CZT czt = new CZT();

            
            index = 0;

            AnalizaLog.Dodaj("Rozpoczęcie analizy FFT");

            //wynik = fft.WydzielPrzydzwiek(wv, ref stan.stan, new OknoBlackmana(), "widmo.txt", 49.8, 50.2, 0.01).czestotliwoscSygnalu;

            Console.WriteLine("Wybierz okno: ");
            Console.WriteLine("- 1: Okno Prostokątne");
            Console.WriteLine("- 2: Okno Barletta");
            Console.WriteLine("- 3: Okno Hanninga");
            Console.WriteLine("- 4: Okno Hamminga");
            Console.WriteLine("- 5: Okno Blackmana");

            int indeksOkna;

            try
            {
                indeksOkna = Convert.ToInt32(Console.ReadLine());    
            }
            catch (Exception e)
            {

                throw e;
            }

            Okno okno;

            switch (indeksOkna)
            {
                case 1:
                    okno = new OknoProstokatne();
                    break;
                case 2:
                    okno = new OknoBarletta();
                    break;
                case 3:
                    okno = new OknoHanninga();
                    break;
                case 4:
                    okno = new OknoHamminga();
                    break;
                case 5:
                    okno = new OknoBlackmana();
                    break;
                default:
                    throw new Exception("Niepoprawny indeks okna!");
            }

            double dolnyZakres, gornyZakres, dokladnosc;

            try
            {
                Console.WriteLine("\nTeraz podaj dolny zakres szukanego przydźwięku (np. 48,9):");
                dolnyZakres = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("\nTeraz podaj górny zakres szukanego przydźwięku (np. 51,9):");
                gornyZakres = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("\nTeraz podaj dokładność (np. 0,001):");
                dokladnosc = Convert.ToDouble(Console.ReadLine());
            }
            catch (Exception ex)
            {

                throw ex;
            }

            stan.Init(Console.CursorLeft, Console.CursorTop);
            stan.Rozpocznij();

            //Spektrogram spektrogram = fft.ObliczSpektrogram(wv, new OknoBlackmana(), 0.1, 0.1);

            //wynik = fft.WydzielPrzydzwiek(wv, ref stan.stan, okno, "widmo.txt" ,dolnyZakres, gornyZakres, dokladnosc).czestotliwoscSygnalu;
            //wynik = czt.WydzielPrzydzwiek(wv, ref stan.stan, new OknoBlackmana(), "widmo2.txt", 45, 55, 0.001).czestotliwoscSygnalu;
            wynik = czt.WydzielPrzydzwiek(wv, ref stan.stan, okno, "widmo2.txt", dolnyZakres, gornyZakres, dokladnosc).czestotliwoscSygnalu;
            //wynik = czt.WydzielPrzydzwiek(wv, ref stan.stan, 48, 52).czestotliwoscSygnalu;

            int potegaDwojki = (int)Math.Log(czt.PrzeliczDokladnosc(dokladnosc,wv.czestotliwoscProbkowania), 2) + 1;

            wynik = new double[5];

           // AnalizaLog.Dodaj("Zakończenie analizy");
           // stan.Zakoncz();
            Console.WriteLine("> Wynik FFT");
            Console.WriteLine();

            Console.WriteLine(" Dokładność: {0}", wv.czestotliwoscProbkowania / (Math.Pow(2, potegaDwojki)));
            Console.WriteLine(" Potęga dwójki: {0}", potegaDwojki);
            Console.WriteLine(" Rozmiar widma: {0}", Math.Pow(2, potegaDwojki));

            Console.WriteLine();
          //  AnalizaLog.Dodaj("Wyniki:", false);
            foreach (double item in wynik)
            {
                Console.WriteLine(" W {0} sekundzie {1} hz", ++index, item);
               // AnalizaLog.Dodaj(" w " + index + " sekundzie " + item + " hz");
            }

            /*Console.WriteLine("> Wynik DFT");
            Console.WriteLine();
            AnalizaLog.Dodaj("Wyniki:", false);
            foreach (double item in wynik)
            {
                Console.WriteLine(" W {0} sekundzie {1} hz", ++index, item);
                AnalizaLog.Dodaj(" w "+index+" sekundzie "+item+" hz");
            }*/


            AnalizaLog.Zamknij();
            /*int potegaDwojki = (int)Math.Log(20000000, 2) + 1;
            Console.WriteLine(wv.czestotliwoscProbkowania / (Math.Pow(2, potegaDwojki)));*/

            
            /*int indexP = (int)(40 * (double)(Math.Pow(2,potegaDwojki) / wv.czestotliwoscProbkowania));
            int indexK = (int)(60 * (double)(Math.Pow(2,potegaDwojki) / wv.czestotliwoscProbkowania));
            Console.WriteLine(wynik.Length);
            index = 40;

            for (int i = indexP; i < indexK; i++)
            {
                Console.WriteLine(" W {0} sekundzie {1} hz", index, wynik[i]/100);
                AnalizaLog.Dodaj(" w " + index + " sekundzie " + wynik[i] + " hz");
                index++;
            }*/

            GuiLog.Zamknij();

            Console.WriteLine();
            Console.WriteLine(" Czas: {0} s", stan.Sekundy);

            Console.CursorVisible = true;

            Console.WriteLine();
            Console.WriteLine(" === Koniec ===");

            Console.WriteLine("\n\nDane widma zapisane w pliku: AnalizaLog.txt");

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
