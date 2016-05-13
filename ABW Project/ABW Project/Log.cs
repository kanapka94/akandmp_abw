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

        public static void Dodaj(string txt, bool date = true)
        {
            if(sw == null) sw = new StreamWriter("log.txt");

            if (date)
            {
                DateTime date1 = DateTime.Now;
                sw.Write("{0} ",date1);
            }
            sw.WriteLine(txt);
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
