//Autorzy: Michał Paduch i Adam Konopka
//Wyżej wymienieni autorzy udostępniają cały powyższy/poniższy kod zawarty w niniejszym pliku 
//na zasadach licencji GNU GPLv2.http://www.gnu.org/licenses/old-licenses/gpl-2.0.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ABW_Project
{
    /// <summary>
    /// Klasa zapisująca logi w pliku tekstowym
    /// </summary>
    class Log
    {
        static StreamWriter sw = null;

        /// <summary>
        /// Metoda dodająca wpis do pliku
        /// </summary>
        /// <param name="tekst">Tekst, który ma zostać wpisany</param>
        /// <param name="czyWypisywacDate">Jeśli 'true' zapisuje też datę obok wpisu</param>
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

        /// <summary>
        /// Metoda zamykająca plik
        /// </summary>
        public static void Zamknij()
        {
            if(sw != null)
            { 
                sw.Close();
            }
        }
    }
}
