//Autorzy: Michał Paduch i Adam Konopka

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ABW_Project
{
    class Log
    {
        static StreamWriter sw = null;

        public static void Dodaj(string tekst, bool czyWypisywacDate = true)
        {
            if(sw == null) sw = new StreamWriter("log.txt");

            if (czyWypisywacDate)
            {
                DateTime data1 = DateTime.Now;
                sw.Write("{0} ",data1);
            }
            sw.WriteLine(tekst);
        }

        public static void Zamknij()
        {
            if(sw != null)
            { 
                sw.Close();
            }
        }
    }
}
