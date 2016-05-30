// @autor Robert Sedgewick
// @autor Kevin Wayne
//oraz zamiana kodu na C#: Michał Paduch i Adam Konopka
//licencja: GNU GPLv.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    /// <summary>
    /// Klasa dotycząca właściwości samego sygnału
    /// </summary>
    class Sygnal
    {
        /// <summary>
        /// Metoda obliczająca energię sygnału
        /// </summary>
        /// <param name="sygnal">Sygnał dźwiękowy</param>
        /// <returns>Zwraca energię sygnału</returns>
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

        /// <summary>
        /// Metoda obliczająca moc sygnału
        /// </summary>
        /// <param name="sygnal">Sygnał</param>
        /// <returns>Zwraca moc sygnału</returns>
        public static double Moc(double[] sygnal)
        {
            double wynik;

            wynik = Energia(sygnal) / ((double)sygnal.Length);

            return wynik;
        }
    }
}
