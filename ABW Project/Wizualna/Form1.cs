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
        ABW_Project.WaveFile wv;

        public Form1()
        {
            wv = new ABW_Project.WaveFile();
            wv.WczytajZPliku("plik.wav");
            InitializeComponent();

            chart1.Series[0].Points.Clear();

            double srednia = 0;

            double coIleRysowac = 2;
            int i;
            for (i = 0; i < wv.iloscProbek/100; i++)
            {
                srednia += wv.NastepnaProbka();
                if (i % coIleRysowac == 0)
                {
                    chart1.Series[0].Points.Add(srednia / coIleRysowac - wv.czestotliwoscProbkowania / 2);
                    srednia = 0;
                }
            }

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
