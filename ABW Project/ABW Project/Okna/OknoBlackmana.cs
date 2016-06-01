using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    /// <summary>
    /// Klasa zawierająca metody okienkujące spróbkowany sygnał za pomocą okna Blackmana
    /// </summary>
    class OknoBlackmana : Okno
    {
        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał za pomocą Okna Blackmana</returns>
        public override double[] Funkcja(int[] x)
        {

            double[] result;
            int i;
            double N;
            double value;
            double n;

            double[] parts;

            parts = new double[2];
            result = new double[x.Length];
            N = (double)x.Length;

            for (i = 0; i < x.Length; i++)
            {

                value = (double)x[i];
                n = (double)i;

                parts[0] = 0.42 - (0.50 * Math.Cos(2.0 * Math.PI * n / (N - 1.0)));
                parts[1] = 0.08 * Math.Cos(4 * Math.PI * n / (N - 1.0));

                result[i] = (parts[0] + parts[1]) * value;
            }

            return result;

        }
    }
}
