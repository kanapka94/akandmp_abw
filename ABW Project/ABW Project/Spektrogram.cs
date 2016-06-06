using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABW_Project
{
    class Spektrogram
    {
        public double[][] modulWidma;      // Pierwsza tablica przechowuje wartość w i-tej sekundzie, druga dla n-tej częstości
        public double skokCzestotliwosci;  // Co ile zmieniają się częstotliwości
        public double skokCzasu;           // Co ile zmienia się czas

        public Spektrogram(double nowySkokCzestotliwosci, double nowySkokCzasu)
        {
            skokCzestotliwosci = nowySkokCzestotliwosci;
            skokCzasu = nowySkokCzasu;
        }
    }
}
