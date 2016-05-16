using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    class Sygnal
    {
        public static double Energia(double[] sygnal)
        {
            double result;
            int i;

            result = 0;

            for (i = 0; i < sygnal.Length; i++)
            {
                result += (sygnal[i] * sygnal[i]);
            }

            return result;
        }

        public static double Moc(double[] sygnal)
        {
            double result;

            result = Energia(sygnal) / ((double)sygnal.Length);

            return result;
        }
    }
}
