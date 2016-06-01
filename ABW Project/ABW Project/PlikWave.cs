﻿//Autorzy: Michał Paduch i Adam Konopka
//Wyżej wymienieni autorzy udostępniają cały powyższy/poniższy kod zawarty w niniejszym pliku 
//na zasadach licencji GNU GPLv2.http://www.gnu.org/licenses/old-licenses/gpl-2.0.html
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic;

namespace ABW_Project
{
    /// <summary>
    /// Klasa przechowująca dane o pliku .wav
    /// </summary>
    class PlikWave
    {
        public string idRiff;
        public string idFormatu;
        public int rozmiarOpisu;
        public int rozmiarPliku;
        public string idOpisu;
        public int rodzajKompresji;
        public int iloscKanalow;
        public int czestotliwoscProbkowania;
        public int czestotliwoscBajtow;
        public int rozmiarProbki;
        public int rozdzielczosc;
        public string daneID;
        public int rozmiarDanych;

        int bitowNaKanal;
        public int iloscProbek;
        public double dlugoscWSekundach;

        private BinaryReader plik;

        /// <summary>
        /// Destruktor
        /// </summary>
        ~PlikWave()
        {
           //destruktor działa poprawnie
            ZamknijPlik();
        }

        /// <summary>
        /// Metoda zamykająca plik
        /// </summary>
        public void ZamknijPlik()
        {
            if (plik != null)
            {
                plik.Close();
            }
        }
        
        /// <summary>
        /// Metoda wczytująca dane nagłówkowe pliku .wav
        /// </summary>
        /// <param name="adresPliku">Ścieżka dostępu do pliku .wav</param>
        public void WczytajZPliku(string adresPliku)
        {
            plik = new BinaryReader(new FileStream(adresPliku,FileMode.Open));
            byte[] readedBytes = plik.ReadBytes(4);
            idRiff = Convert.ToString(((char)readedBytes[0])) + Convert.ToString(((char)readedBytes[1])) + Convert.ToString(((char)readedBytes[2])) + Convert.ToString(((char)readedBytes[3]));

            readedBytes = plik.ReadBytes(4);
            rozmiarPliku = readedBytes[0] + readedBytes[1] * (int)Math.Pow(256, 1) + readedBytes[2] * (int)Math.Pow(256, 2) + readedBytes[3] * (int)Math.Pow(256, 3);

            readedBytes = plik.ReadBytes(4);
            idFormatu = Convert.ToString(((char)readedBytes[0])) + Convert.ToString(((char)readedBytes[1])) + Convert.ToString(((char)readedBytes[2])) + Convert.ToString(((char)readedBytes[3]));

            readedBytes = plik.ReadBytes(4);
            idOpisu = Convert.ToString(((char)readedBytes[0])) + Convert.ToString(((char)readedBytes[1])) + Convert.ToString(((char)readedBytes[2])) + Convert.ToString(((char)readedBytes[3]));

            readedBytes = plik.ReadBytes(4);
            rozmiarOpisu = readedBytes[0] + readedBytes[1] * (int)Math.Pow(256, 1) + readedBytes[2] * (int)Math.Pow(256, 2) + readedBytes[3] * (int)Math.Pow(256, 3);

            readedBytes = plik.ReadBytes(2);
            rodzajKompresji = readedBytes[1] * 10 + readedBytes[0];

            readedBytes = plik.ReadBytes(2);
            iloscKanalow = readedBytes[0] + readedBytes[1] * (int)Math.Pow(256, 1);

            readedBytes = plik.ReadBytes(4);
            czestotliwoscProbkowania = readedBytes[0] + readedBytes[1] * (int)Math.Pow(256, 1) + readedBytes[2] * (int)Math.Pow(256, 2) + readedBytes[3] * (int)Math.Pow(256, 3);

            readedBytes = plik.ReadBytes(4);
            czestotliwoscBajtow = readedBytes[0] + readedBytes[1] * (int)Math.Pow(256, 1) + readedBytes[2] * (int)Math.Pow(256, 2) + readedBytes[3] * (int)Math.Pow(256, 3);

            readedBytes = plik.ReadBytes(2);
            rozmiarProbki = readedBytes[0] + readedBytes[1] * 256;

            readedBytes = plik.ReadBytes(2);
            rozdzielczosc = readedBytes[0] + readedBytes[1] * 256;

            bitowNaKanal = (rozmiarProbki / iloscKanalow);

            //byte readedBytes1 = plik.ReadBytes(1)[0];
            //byte readedBytes2 = plik.ReadBytes(1)[0];
            //byte readedBytes3 = plik.ReadBytes(1)[0];
            //byte readedBytes4 = plik.ReadBytes(1)[0];

            //while (readedBytes1 == 98 && readedBytes2 == 121 && readedBytes3 == 116 && readedBytes4 == 101)
            //{
            //    readedBytes3 = readedBytes4;
            //    readedBytes2 = readedBytes3;
            //    readedBytes1 = readedBytes2;
            //    readedBytes4 = plik.ReadBytes(1)[0];
            //}

            int ilePominac = 0;
            ilePominac = rozmiarOpisu - 16;

            if (ilePominac > 0)
            {
                plik.ReadBytes(ilePominac+4);     //pomija Dodatkowe parametry, ktore sa nam niepotrzebne
            }

            readedBytes = plik.ReadBytes(4);
            daneID = Convert.ToString(((char)readedBytes[0])) + Convert.ToString(((char)readedBytes[1])) + Convert.ToString(((char)readedBytes[2])) + Convert.ToString(((char)readedBytes[3]));

            readedBytes = plik.ReadBytes(4);
            rozmiarDanych = readedBytes[0] + readedBytes[1] * (int)Math.Pow(256, 1) + readedBytes[2] * (int)Math.Pow(256, 2) + readedBytes[3] * (int)Math.Pow(256, 3);

            iloscProbek = (rozmiarDanych) / rozmiarProbki;
            dlugoscWSekundach = (double)iloscProbek / czestotliwoscProbkowania;
            
        }

        /// <summary>
        /// Metoda wczytująca następną próbkę
        /// </summary>
        /// <param name="kanal">Kanał dźwiękowy wyrażany w wartości liczbowej (0 to lewy, 1 prawy, ...)</param>
        /// <returns>Zwraca wartość próbki</returns>
        public int NastepnaProbka(byte kanal = 0)
        {
            byte[] readedBytes = plik.ReadBytes(rozmiarProbki);
            int dane = 0;
            int pozycjaKanalu = kanal * bitowNaKanal;

            for (int i = pozycjaKanalu; i < pozycjaKanalu + bitowNaKanal; i++)
                dane += readedBytes[i] << i; //(int)Math.Pow(256, i);

            return dane;
        }

        /// <summary>
        /// Metoda pobierająca wyznaczoną ilość próbek
        /// </summary>
        /// <param name="kanal">Kanał dźwiękowy wyrażany w wartości liczbowej (0 to lewy, 1 prawy, ...)</param>
        /// <param name="probkiDoOdczytania">Ilość próbek</param>
        /// <returns>Zwraca tablicę próbek (sygnał spróbkowany)</returns>
        public int[] PobierzProbki(byte kanal = 0, int probkiDoOdczytania = -1)
        {
            if (probkiDoOdczytania == -1) probkiDoOdczytania = czestotliwoscProbkowania;

            int[] probki = new int[probkiDoOdczytania];

            for (int i = 0; i < probkiDoOdczytania; i++)
            {
                probki[i] = NastepnaProbka(kanal);
            }

            return probki;
        }

        /// <summary>
        /// Atrybut zwracający całkowity rozmiar pliku
        /// </summary>
        public int CalkowityRozmiar
        {
            get
            {
                return rozmiarPliku + 8;
            }
        }

        /// <summary>
        /// Atrybut zwracający rodzaj kompresji pliku
        /// </summary>
        public string RodzajKompresjiOpis
        {
            get
            {
                switch (rodzajKompresji)
                {
                    case 1:
                        return "Bez kompresji, modulacja PCM";
                    default:
                        return "Nie rozpoznano...";
                }
            }
        }
    }
}
