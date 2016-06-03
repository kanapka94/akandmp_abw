﻿//autor: Michał Paduch i Adam Konopka
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
    /// Klasa AnalizaLog - obsługuje plik log zbierający informacje o analizie sygnału
    /// </summary>
    class AnalizaLog
    {
        static StreamWriter logSW = null;

        protected static string NazwaPliku = "AnalizaLog.txt";

        /// <summary>
        /// Metoda tworząca plik log w folderze logi. Do funkcji trzeba podać tylko nazwę pliku.
        /// </summary>
        protected static void UtworzPlikLog()
        {
            if (logSW == null)
                logSW = new StreamWriter(NazwaPliku);
        }

        /// <summary>
        /// Metoda dodająca wpis do pliku
        /// </summary>
        /// <param name="tekst">Tekst, który ma zostać wpisany</param>
        /// <param name="Modul">Nazwa modułu, do którego należy plik Log (Log.MODUL_GUI, Log.MODUL_ANALIZY, Log.MODUL_BAZ_DANYCH,Log.MODUL_POROWNANIA) </param>
        /// <param name="czyWypisywacDate">Jeśli 'true' zapisuje też datę obok wpisu</param>
        public static void Postep(string tekst, int Modul, bool czyWypisywacDate = true)
        {
            Dodaj("[Postęp] " + tekst, czyWypisywacDate);
        }

        /// <summary>
        /// Metoda dodająca wpis do pliku
        /// </summary>
        /// <param name="tekst">Tekst, który ma zostać wpisany</param>
        /// <param name="Modul">Nazwa modułu, do którego należy plik Log (Log.MODUL_GUI, Log.MODUL_ANALIZY, Log.MODUL_BAZ_DANYCH,Log.MODUL_POROWNANIA) </param>
        /// <param name="czyWypisywacDate">Jeśli 'true' zapisuje też datę obok wpisu</param>
        public static void Blad(string tekst, bool czyWypisywacDate = true)
        {
            Dodaj("[Błąd] " + tekst, czyWypisywacDate);
        }

        /// <summary>
        /// Metoda dodająca wpis do pliku
        /// </summary>
        /// <param name="tekst">Tekst, który ma zostać wpisany</param>
        /// <param name="Modul">Nazwa modułu, do którego należy plik Log (Log.MODUL_GUI, Log.MODUL_ANALIZY, Log.MODUL_BAZ_DANYCH,Log.MODUL_POROWNANIA) </param>
        /// <param name="czyWypisywacDate">Jeśli 'true' zapisuje też datę obok wpisu</param>
        public static void Dodaj(string tekst, bool czyWypisywacDate = true)
        {
            if (logSW == null)
                UtworzPlikLog();

            try
            {         
                if (czyWypisywacDate)
                {
                    DateTime data1 = DateTime.Now;
                    logSW.Write("{0} ", data1);
                }
                logSW.WriteLine(tekst);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Metoda zamykająca plik
        /// </summary>
        public static void Zamknij()
        {
            if (logSW != null)
            {
                try
                {
                    logSW.Close();
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
        }
    }
}