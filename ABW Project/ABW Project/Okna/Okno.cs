using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    /// <summary>
    /// Klasa abstrakcyjna zawierająca metody okienkujące spróbkowany sygnał
    /// </summary>
    abstract class Okno
    {
        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał</returns>
        public abstract double[] Funkcja(int[] x);
    }
}
