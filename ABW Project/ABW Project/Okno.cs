// @autor Robert Sedgewick
// @autor Kevin Wayne
//oraz zamiana kodu na C#: Michał Paduch i Adam Konopka
//Wyżej wymienieni autorzy udostępniają cały powyższy/poniższy kod zawarty w niniejszym pliku 
//na zasadach licencji GNU GPLv2.http://www.gnu.org/licenses/old-licenses/gpl-2.0.html
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    
    class Okno
    {
        public readonly static int Prostokatne = 1;
        public readonly static int Barletta = 2;
        public readonly static int Hanninga = 3;
        public readonly static int Hamminga = 4;
        public readonly static int Blackmana = 5;





        

        

        /// <summary>
        /// Metoda obsługująca wybór okna
        /// </summary>
        /// <param name="sygnal">Sygnał spróbkowany</param>
        /// <param name="okno">Numer okna </param>
        /// <returns>Zwraca zokienkowany sygnał</returns>
        public static double[] Funkcja(int[] sygnal, int okno)
        {
            double[] result = new double[sygnal.Length];
            for (int i = 0; i < sygnal.Length; i++)
            {
                result[i] = (double)sygnal[i];
            }

            if (okno == Prostokatne)
            {
                
            }
            else if (okno == Barletta)
            {
                result = OknoBartletta(result);
            }
            else if (okno == Hanninga)
            {
                result = OknoHanninga(result);
            }
            else if (okno == Hamminga)
            {
                result = OknoHamminga(result);
            }
            else if (okno == Blackmana)
            {
                result = OknoBlackmana(result);
            }
            else
            {
                // Błąd, złe okno
            }

            return result;
        }
    }
}
*/