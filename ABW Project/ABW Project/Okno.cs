﻿// @autor Robert Sedgewick
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
    /// Klasa zawierająca metody okienkujące spróbkowany sygnał
    /// </summary>
    class Okno
    {
        public readonly static int Prostokatne = 1;
        public readonly static int Barletta = 2;
        public readonly static int Hanninga = 3;
        public readonly static int Hamminga = 4;
        public readonly static int Blackmana = 5;

        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał</returns>
        private static double[] OknoBartletta(double[] x)
        {
            double[] result;
            int i;
            double N;
            double value;
            double n;

            result = new double[x.Length];
            N = (double)x.Length;

            for (i = 0; i < x.Length; i++)
            {

                value = x[i];
                n = (double)i;

                result[i] = (1.0 - (Math.Abs((n - ((N - 1.0) / 2.0))) / (N - 1.0))) * value;
            }

            return result;
        }

        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał</returns>
        private static double[] OknoHanninga(double[] x)
        {

            double[] result;
            int i;
            double N;
            double value;
            double n;

            result = new double[x.Length];
            N = (double)x.Length;

            for (i = 0; i < x.Length; i++)
            {

                value = x[i];
                n = (double)i;

                result[i] = (0.5 * (1.0 - Math.Cos(2.0 * Math.PI * n / (N - 1.0)))) * value;
            }

            return result;

        }

        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał</returns>
        private static double[] OknoHamminga(double[] x)
        {

            double[] result;
            int i;
            double N;
            double value;
            double n;

            result = new double[x.Length];
            N = (double)x.Length;

            for (i = 0; i < x.Length; i++)
            {

                value = x[i];
                n = (double)i;

                result[i] = (0.54 - (0.46 * Math.Cos(2.0 * Math.PI * n / (N - 1.0)))) * value;
            }

            return result;

        }

        /// <summary>
        /// Metoda modyfikująca sygnał
        /// </summary>
        /// <param name="x">Sygnał spróbkowany</param>
        /// <returns>Zwraca zokienkowany sygnał</returns>
        private static double[] OknoBlackmana(double[] x)
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

                value = x[i];
                n = (double)i;

                parts[0] = 0.42 - (0.50 * Math.Cos(2.0 * Math.PI * n / (N - 1.0)));
                parts[1] = 0.08 * Math.Cos(4 * Math.PI * n / (N - 1.0));

                result[i] = (parts[0] + parts[1]) * value;
            }

            return result;

        }

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
