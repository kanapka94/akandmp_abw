using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizualna
{
    class Algorytm
    {
        public Wynik WydzielPrzydzwiek(PlikWave plik)
        {
            Wynik wynik = new Wynik();
            wynik.czestotliwoscSzumow = ObliczWidmo(plik.PobierzProbki());
            return wynik;
        }

        public virtual decimal[] ObliczWidmo(decimal[] sygnal)
        {
            return null;
        }
    }
}
