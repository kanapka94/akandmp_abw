//autorzy: Michał Paduch i Adam Konopka
//Wyżej wymienieni autorzy udostępniają cały powyższy/poniższy kod zawarty w niniejszym pliku 
//na zasadach licencji GNU GPLv2.http://www.gnu.org/licenses/old-licenses/gpl-2.0.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

namespace ABW_Project
{
    /// <summary>
    /// Klasa, którą dziedziczą klasy FFT, CZT oraz ESPRIT
    /// </summary>
    abstract class Algorytm
    {
        /// <summary>
        /// Metoda zamieniająca dokładność z postaci liczby zmiennoprzecinkowej (0,002) na ilość próbek potrzebnych do osiągnięcia takiej dokładności.
        /// </summary>
        /// <param name="dokladnosc">Dokładnośc podana przez użytkownika</param>
        /// <param name="czestotliwoscProbkowania">Częstotliwość próbkowania nagrania</param>
        /// <returns>Zwraca ilość potrzebnych próbek</returns>
        public abstract int PrzeliczDokladnosc(double dokladnosc, int czestotliwoscProbkowania, double dolnaCzestosc = 0, double gornaCzestosc = 0);

        /// <summary>
        /// Metoda sprawdzająca czy podana dokładność jest prawidłowa dla poszczególnego algorytmu
        /// </summary>
        /// <param name="dokladnosc"></param>
        public abstract void SprawdzDokladnosc(double dokladnosc);

        /// <summary>
        /// Metoda wydzielająca wartość przydźwięku sieciowego co sekundę
        /// </summary>
        /// <param name="plik">Obiekt klasy PlikWave</param>
        /// <param name="stan">Zmienna referencyjna wskazująca ilość procent wykonania algorytmu</param>
        /// <param name="okno">Obiekt, który obliczy okno sygnału</param>
        /// <param name="plikPrzydzwieku">Plik, do którego zostanie zapisany znaleziony przydźwięk</param>
        /// <param name="dolnaCzestosc">Dolna częstotliwość (graniczna, badana)</param>
        /// <param name="gornaCzestosc">Górna częstotliwość (graniczna, badana)</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <returns>Zwraca obiekt klasy Wynik</returns>
        public abstract Wynik WydzielPrzydzwiek(PlikWave plik, ref int stan, Okno okno, string plikPrzydzwieku, double dolnaCzestosc = 40, double gornaCzestosc = 60, double dokladnosc = 1, int poczatkowaSekunda = 0, int koncowaSekunda = -1);

        /// <summary>
        /// Metoda wirtualna. Jej zadaniem jest obliczanie widma sygnału dźwiękowego
        /// </summary>
        /// <param name="sygnal">Sygnał spróbkowany</param>
        /// <param name="dokladnosc">dokładność badanych częstotliwości (wyrażona w ilości próbek)</param>
        /// <param name="dolnyZakres">Dolny zakres liczenia widma</param>
        /// <param name="gornyZakres">Górny zakres obliczanego widma</param>
        /// <returns>Zwraca tablicę wartości widma</returns>
        public abstract double[] ObliczWidmo(double[] sygnal, Okno okno, int dokladnosc, double dolnaCzestosc = -1, double gornaCzestosc = -1);

        /// <summary>
        /// Metoda zwracająca Spektrogram nagrania
        /// </summary>
        /// <param name="plik">Obiekt klasy PlikWabe z nagraniem</param>
        /// <param name="okno">Zastosowane okno</param>
        /// <param name="skokCzestotliwosc">Co ile ma liczyć częstotliwość (FFT może zmniejszyć wartość)</param>
        /// <param name="skokCzas">Co jaką wartość sekundy zmienia się indeks tablicy</param>
        /// <returns>Spektrogram nagrania</returns>
        public abstract Spektrogram ObliczSpektrogram(PlikWave plik, Okno okno, double skokCzestotliwosc, double skokCzas);

        /// <summary>
        /// Metoda sprawdza czy podane przedział czasu mieści się w długości nagrania. W razie błędu zwraca wyjątek
        /// </summary>
        /// <param name="dlugoscPlikuWave">Długość pliku Wav</param>
        /// <param name="poczatkowaSekunda">Sekunda, od której ma zacząć analizę</param>
        /// <param name="koncowaSekunda">Sekunda do której ma skończyć analizę</param>
        public virtual void SprawdzDlugosc(int dlugoscPlikuWave, int poczatkowaSekunda, int koncowaSekunda)
        {
            if (poczatkowaSekunda < 0)
            {
                AnalizaLog.Blad("Początkowa sekunda nie może być mniejsza od 0");
                throw new Exception("Początkowa sekunda nie może być mniejsza od 0");
            }
            if (koncowaSekunda > dlugoscPlikuWave)
            {
                AnalizaLog.Blad("Końcowa sekunda musi być mniejsza niż długość pliku");
                throw new Exception("Końcowa sekunda musi być mniejsza niż długość pliku");
            }
            if (koncowaSekunda < poczatkowaSekunda)
            {
                AnalizaLog.Blad("Końcowa sekunda musi być większa niż początkowa sekunda");
                throw new Exception("Końcowa sekunda musi być większa niż początkowa sekunda");
            }
        }
    }
 }   
