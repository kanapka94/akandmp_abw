﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    /// <summary>
    /// Klasa zawierająca metody okienkujące spróbkowany sygnał za pomocą okna Hamminga
    /// </summary>
    class OknoHamminga : Okno
    {
        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał za pomocą okna Hamminga</returns>
        public override double[] Funkcja(double[] x)
        {
            AnalizaLog.Postep("Obliczam okno korzystając z okna Hamminga");

            int i;
            double N;
            double wartosc;
            double n;

            N = (double)x.Length;

            for (i = 0; i < x.Length; i++)
            {

                wartosc = x[i];
                n = (double)i;

                x[i] = (0.54 - (0.46 * Math.Cos(2.0 * Math.PI * n / (N - 1.0)))) * wartosc;
            }

            return x;

        }
    }
}
