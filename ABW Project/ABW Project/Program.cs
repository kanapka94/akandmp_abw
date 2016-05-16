using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

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
            Console.WriteLine(" Postęp: [+++++] 100%       ");
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
            wv.WczytajZPliku("plik.wav");

            Log.Dodaj("-------------------- Nowa analiza -----------------------");                        

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

            
            
            Console.WriteLine(" Analiza...\n");
            Stan stan = new Stan();
            //DFT dft = new DFT();
            FFT fft = new FFT();

            stan.Init(Console.CursorLeft, Console.CursorTop);
            stan.Rozpocznij();
            int index = 0;

            Log.Dodaj("Rozpoczęcie analizy");
            //double[] wynik = dft.WydzielPrzydzwiek(wv, ref stan.stan,wv.czestotliwoscProbkowania*16).czestotliwoscSygnalu;
            double[] wynik = fft.WydzielPrzydzwiek(wv, ref stan.stan).czestotliwoscSygnalu;

            Log.Dodaj("Zakończenie analizy");
            stan.Zakoncz();

            /*Console.WriteLine("> Wynik DFT");
            Console.WriteLine();
            Log.Dodaj("Wyniki:", false);
            foreach (double item in wynik)
            {
                Console.WriteLine(" W {0} sekundzie {1} hz", ++index, item);
                Log.Dodaj(" w "+index+" sekundzie "+item+" hz");
            }*/

            Console.WriteLine("> Wynik FFT");
            Console.WriteLine();
            Log.Dodaj("Wyniki:", false);
            /*foreach (double item in wynik)
            {
                Console.WriteLine(" W {0} sekundzie {1} hz", ++index, item);
                Log.Dodaj(" w " + index + " sekundzie " + item + " hz");
            }*/
            int potegaDwojki = (int)Math.Log(800000, 2) + 1;

            Console.WriteLine(wv.czestotliwoscProbkowania / (Math.Pow(2, potegaDwojki)));

            StreamWriter sw = new StreamWriter("plik.txt");

            for (int i = 0; i < wynik.Length; i++)
            {
                sw.WriteLine(Convert.ToString((decimal)(i * wv.czestotliwoscProbkowania / (decimal)(Math.Pow(2, potegaDwojki)))));
            }
            sw.Close();
            /*int indexP = (int)(40 * (double)(Math.Pow(2,potegaDwojki) / wv.czestotliwoscProbkowania));
            int indexK = (int)(60 * (double)(Math.Pow(2,potegaDwojki) / wv.czestotliwoscProbkowania));
            Console.WriteLine(wynik.Length);
            index = 40;

            for (int i = indexP; i < indexK; i++)
            {
                Console.WriteLine(" W {0} sekundzie {1} hz", index, wynik[i]/100);
                Log.Dodaj(" w " + index + " sekundzie " + wynik[i] + " hz");
                index++;
            }*/

            Log.Zamknij();

            Console.WriteLine();
            Console.WriteLine(" Czas: {0} s", stan.Sekundy);

            Console.CursorVisible = true;

            Console.WriteLine();
            Console.WriteLine(" === Koniec ===");
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
