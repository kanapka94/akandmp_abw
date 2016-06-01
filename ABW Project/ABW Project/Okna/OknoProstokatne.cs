using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    class OknoProstokatne : Okno
    {
        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał oknem prostokątnym</returns>

        public override double[] Funkcja(int[] x)
        {
            double[] result = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                result[i] = (double)x[i];
            }

            return result;
        }
    }
}
