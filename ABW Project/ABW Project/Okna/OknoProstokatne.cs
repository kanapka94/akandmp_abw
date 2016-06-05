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

        public override double[] Funkcja(double[] x)
        {
            AnalizaLog.Postep("Obliczam okno korzystając z okna Prostokątnego");
            return x;
        }
    }
}
