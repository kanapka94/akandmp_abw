// @autor Robert Sedgewick
// @autor Kevin Wayne
//licencja: GPLv2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    class Sygnal
    {
        public static double Energia(double[] sygnal)
        {
            double wynik;
            int i;

            wynik = 0;

            for (i = 0; i < sygnal.Length; i++)
            {
                wynik += (sygnal[i] * sygnal[i]);
            }

            return wynik;
        }

        public static double Moc(double[] sygnal)
        {
            double wynik;

            wynik = Energia(sygnal) / ((double)sygnal.Length);

            return wynik;
        }
    }
}
