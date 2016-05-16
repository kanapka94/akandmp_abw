using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ABW_Project
{
    class Algorytm
    {
        public Wynik WydzielPrzydzwiek(PlikWave plik,ref int stan)
        {
            Wynik wynik = new Wynik();

            double[] wynik2 = plik.PobierzProbki();
            wynik.czestotliwoscSygnalu = new double[800000];

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
        }

        public virtual double[] ObliczWidmo(double[] sygnal)
        {
            // Algorytm powinien być nadpisany

            return null;
        }
    }
}
