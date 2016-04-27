using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic;

namespace ABW_Project
{
    class WaveFile
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

        public int iloscProbek;
        public double dlugoscWSekundach;

        private BinaryReader plik;

        ~WaveFile()
        {
           //destruktor działa poprawnie
            if (plik != null)
            {
                plik.Close();
            }
        }
        
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
            // riffId = ((string)readedBytes[0]) + ((char)readedBytes[1]) + ((char)readedBytes[2]) + ((char)readedBytes[3]);

            //riffId = 1000 * readedBytes[3] + 100 * readedBytes[2] + 10 * readedBytes[1] + readedBytes[0];
            
        }

        public double NastepnaProbka(byte kanal = 0)
        {
            byte[] readedBytes = plik.ReadBytes(rozmiarProbki);
            double dane = 0;

            for (int i = kanal * (rozmiarProbki / iloscKanalow); i < (kanal+1) * (rozmiarProbki / iloscKanalow); i++)
                dane += readedBytes[i] * (double)Math.Pow(256, i);

            return dane;
        }

        /*public double NastepnaProbka(bool czyLewy = true)
        {
            byte[] readedBytes = plik.ReadBytes(rozmiarProbki);
            double dane = 0;
          
            if (stereo)
            {
                if (czyLewy)
                {
                    for (int i = 0; i < rozmiarProbki / 2; i++)
                        dane += readedBytes[i] * (double)Math.Pow(256, i);
                    plik.ReadBytes(rozmiarProbki / 2);  //gdy mono to odrzuca drugą połowę bajtów
                }
                else
                {
                    plik.ReadBytes(rozmiarProbki / 2);  //gdy mono to odrzuca drugą połowę bajtów
                    for (int i = 0; i < rozmiarProbki / 2; i++)
                        dane += readedBytes[i] * (double)Math.Pow(256, i);                    
                }
            }
            else
            {
                for (int i = 0; i < rozmiarProbki; i++)
                {
                    dane += readedBytes[i] * (double)Math.Pow(256, i);
                }
            }
            return dane;
        }*/

        public int CalkowityRozmiar
        {
            get
            {
                return rozmiarPliku + 8;
            }
        }

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
