using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    class Algorytm
    {
        public Wynik WydzielPrzydzwiek(PlikWave plik,ref int stan)
        {
            Wynik wynik = new Wynik();
            wynik.czestotliwoscSzumow = new double[(int)plik.dlugoscWSekundach];

            for (int sekundy = 0; sekundy < (int)plik.dlugoscWSekundach; sekundy++)
            {
                double[] widmo = ObliczWidmo(plik.PobierzProbki());

                double max = 0;
                decimal maxi = 0;
                decimal index = 49.8M;

                foreach (double item in widmo)
                {
                    if (item > max)
                    {
                        max = item;
                        maxi = index;
                    }

                    index += 0.001M;
                }

                stan = (int)((double)(sekundy+1) / plik.dlugoscWSekundach * 100);

                wynik.czestotliwoscSzumow[sekundy] = (double)maxi;
            }
            stan = 100;

            return wynik;
        }

        public virtual double[] ObliczWidmo(double[] sygnal)
        {
            return null;
        }
    }
}
