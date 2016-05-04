using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    class Algorytm
    {
        public Wynik WydzielPrzydzwiek(PlikWave plik)
        {
            Wynik wynik = new Wynik();
            wynik.czestotliwoscSzumow = ObliczWidmo(plik.PobierzProbki());
            return wynik;
        }

        public virtual double[] ObliczWidmo(double[] sygnal)
        {
            return null;
        }
    }
}
