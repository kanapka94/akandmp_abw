using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wizualna
{
    public partial class Form1 : Form
    {
        PlikWave wv;

        public Form1()
        {
            wv = new PlikWave();
            wv.WczytajZPliku("plik.wav");
            InitializeComponent();

            chart1.Series[0].Points.Clear();

            DFT dft = new DFT();
            FFT fft = new FFT();

            /*
            decimal index = 49.001;

            foreach (decimal item in dft.WydzielPrzydzwiek(wv).czestotliwoscSzumow)
            {
                chart1.Series[0].Points.Add(item);
            }*/
            /*decimal[] S = new decimal[100];

            for (int k = 0; k < 100; k++)
            {
                S[k] = (decimal) Math.Sin((decimal)2 * Math.PI * 3 * k / 100) + Math.Cos(2*Math.PI * 10 * k / 100);
                //chart1.Series[0].Points.Add(S[k]);
            }*/

            decimal[] S;

            S = wv.PobierzProbki(0, 200);

            //decimal[] wynik = dft.ObliczWidmo(S);
            decimal[] wynik2 = fft.ObliczWidmo(S);

            for (int k = 0; k < 200; k++)
            {
                chart1.Series[0].Points.Add(wynik2[k]);
            }


            /*decimal srednia = 0;

            decimal coIleRysowac = 2;
            int i;
            for (i = 0; i < wv.iloscProbek/100; i++)
            {
                srednia += wv.NastepnaProbka();
                if (i % coIleRysowac == 0)
                {
                    chart1.Series[0].Points.Add(srednia / coIleRysowac - wv.czestotliwoscProbkowania / 2);
                    srednia = 0;
                }
            }*/

            // MessageBox.Show(Convert.ToString(wv.czestotliwoscProbkowania));

            //MessageBox.Show(Convert.ToString(coIleRysowac * chart1.Series[0].Points.Count));
        }

        private void chart1_Click(object sender, EventArgs e)
        {
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
            {
                MessageBox.Show(openFileDialog1.FileNames[i]);
            }
        }

        private void listBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
