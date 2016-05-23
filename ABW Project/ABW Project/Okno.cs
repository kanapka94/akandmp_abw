// @autor Robert Sedgewick
// @autor Kevin Wayne

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

        public static double[] Funkcja(double[] sygnal, int okno)
        {
            if (okno == Prostokatne)
            {
                
            }
            else if (okno == Barletta)
            {
                sygnal = OknoBartletta(sygnal);
            }
            else if (okno == Hanninga)
            {
                sygnal = OknoHamminga(sygnal);
            }
            else if (okno == Hamminga)
            {
                sygnal = OknoHamminga(sygnal);
            }
            else if (okno == Blackmana)
            {
                sygnal = OknoBlackmana(sygnal);
            }
            else
            {
                // Błąd, złe okno
            }

            return sygnal;
        }
    }
}
