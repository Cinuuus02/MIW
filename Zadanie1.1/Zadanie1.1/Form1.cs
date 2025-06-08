using System;
using System.Windows.Forms;

namespace Zadanie1._1
{
    public partial class Form1 : Form
    {
        class Osobnik
        {
            public string Cb;
            public double[] Parametr;
            public double Ocena;

            public Osobnik(string cb, double[] parametr, double ocena)
            {
                Cb = cb;
                Parametr = parametr;
                Ocena = ocena;
            }
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int LBnP = (int)numericUpDown1.Value;
            int TurRozm = (int)numericUpDown2.Value;
            int populacja = (int)numericUpDown3.Value;
            int parametry = (int)numericUpDown4.Value;
            int generacje = (int)numericUpDown5.Value;
            double ZDMin = 0;
            double ZDMax = 100;
            double ZD = ZDMax - ZDMin;
            Random rand = new Random();

            double Dekodowanie(string cb)
            {
                int ctmp = 0;
                int b = 0;
                for (int p = LBnP - 1; p >= 0; p--)
                {
                    ctmp += int.Parse(cb[p].ToString()) * (int)Math.Pow(2, b);
                    b++;
                }
                double pm = ZDMin + ctmp / (Math.Pow(2, LBnP) - 1) * ZD;
                return pm;
            }

            Osobnik Turniej(Osobnik[] osobniki)
            {
                Osobnik[] sklad = new Osobnik[TurRozm];
                for (int i = 0; i < TurRozm; i++)
                {
                    int index = rand.Next(osobniki.Length);
                    sklad[i] = osobniki[index];
                }
                var os_wybr = sklad[0];
                for (int i = 1; i < TurRozm; i++)
                {
                    if (sklad[i].Ocena > os_wybr.Ocena)
                    {
                        os_wybr = sklad[i];
                    }
                }
                return new Osobnik(os_wybr.Cb, (double[])os_wybr.Parametr.Clone(), os_wybr.Ocena);
            }

            Osobnik[] Mutacja(Osobnik[] nowa_pula)
            {
                for (int i = 0; i < populacja - 1; i++)
                {
                    int b_punkt = rand.Next(0, LBnP * parametry);
                    string cb = nowa_pula[i].Cb;
                    char[] mutacja_cb = cb.ToCharArray();
                    mutacja_cb[b_punkt] = mutacja_cb[b_punkt] == '0' ? '1' : '0';
                    nowa_pula[i].Cb = new string(mutacja_cb);
                }
                return nowa_pula;
            }

            double srednia(Osobnik[] osobniki)
            {
                double suma = 0;
                for (int i = 0; i < populacja; i++)
                {
                    suma += osobniki[i].Ocena;
                }
                return suma / populacja;
            }

            Osobnik najlepszy(Osobnik[] osobniki)
            {
                var najlepszy = osobniki[0];
                for (int i = 1; i < populacja; i++)
                {
                    if (osobniki[i].Ocena > najlepszy.Ocena)
                    {
                        najlepszy = osobniki[i];
                    }
                }
                return najlepszy;
            }

            Osobnik[] osobniki = new Osobnik[populacja];
            for (int i = 0; i < populacja; i++)
            {
                string cb = "";
                for (int j = 0; j < LBnP * parametry; j++)
                {
                    cb += rand.Next(0, 2);
                }
                osobniki[i] = new Osobnik(cb, new double[parametry], 0.0);
            }

            for (int i = 0; i < populacja; i++)
            {
                for (int j = 0; j < parametry; j++)
                {
                    osobniki[i].Parametr[j] = Math.Round(Dekodowanie(osobniki[i].Cb.Substring(j * LBnP, LBnP)), 2);
                }
                osobniki[i].Ocena = Math.Round(Math.Sin(osobniki[i].Parametr[0] * 0.05) + Math.Sin(osobniki[i].Parametr[1] * 0.05) + 0.4 * Math.Sin(osobniki[i].Parametr[0] * 0.15) * Math.Sin(osobniki[i].Parametr[1] * 0.15), 2);
            }

            richTextBox1.Clear();
            richTextBox1.AppendText("Generacja 0:\n");
            richTextBox1.AppendText("Najlepszy osobnik: " + najlepszy(osobniki).Ocena + "\n");
            richTextBox1.AppendText("Srednia ocena: " + Math.Round(srednia(osobniki), 2) + "\n");

            for (int g = 0; g < generacje; g++)
            {
                richTextBox1.AppendText($"\nGeneracja {g + 1}:\n");

                Osobnik[] nowa_pula = new Osobnik[populacja];

                for (int i = 0; i < populacja - 1; i++)
                {
                    nowa_pula[i] = Turniej(osobniki);
                }

                nowa_pula = Mutacja(nowa_pula);

                nowa_pula[populacja - 1] = najlepszy(osobniki);

                osobniki = nowa_pula;

                for (int i = 0; i < populacja; i++)
                {
                    for (int j = 0; j < parametry; j++)
                    {
                        osobniki[i].Parametr[j] = Math.Round(Dekodowanie(osobniki[i].Cb.Substring(j * LBnP, LBnP)), 2);
                    }
                    osobniki[i].Ocena = Math.Round(Math.Sin(osobniki[i].Parametr[0] * 0.05) + Math.Sin(osobniki[i].Parametr[1] * 0.05) + 0.4 * Math.Sin(osobniki[i].Parametr[0] * 0.15) * Math.Sin(osobniki[i].Parametr[1] * 0.15), 2);
                }
                richTextBox1.AppendText("Najlepszy osobnik: " + najlepszy(osobniki).Ocena + "\n");
                richTextBox1.AppendText("Srednia ocena: " + Math.Round(srednia(osobniki), 2) + "\n");
            }
        }
    }
}
