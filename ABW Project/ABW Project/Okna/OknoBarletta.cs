﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    /// <summary>
    /// Klasa zawierająca metody okienkujące spróbkowany sygnał za pomocą okna Barletta
    /// </summary>
    class OknoBarletta : Okno
    {
        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał za pomocą okna Barletta</returns>
        public override double[] Funkcja(double[] x)
        {
            AnalizaLog.Postep("Obliczam okno korzystając z okna Barletta");

            int i;
            double N;
            double wartosc;
            double n;

            N = (double)x.Length;

            for (i = 0; i < x.Length; i++)
            {

                wartosc = x[i];
                n = (double)i;

                x[i] = (1.0 - (Math.Abs((n - ((N - 1.0) / 2.0))) / (N - 1.0))) * wartosc;
            }

            return x;
        }
    }
}
