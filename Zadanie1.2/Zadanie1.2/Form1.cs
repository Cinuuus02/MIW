using System;
using System.Windows.Forms;

namespace Zadanie1._2
{
    public partial class Form1 : Form
    {
        private Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private class Osobnik
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

        private double[] x = {-1.00000, -0.80000, -0.60000, -0.40000, -0.20000, 0.00000, 0.20000, 0.40000, 0.60000, 0.80000, 1.00000,
            1.20000, 1.40000, 1.60000, 1.80000, 2.00000, 2.20000, 2.40000, 2.60000, 2.80000, 3.00000, 3.20000, 3.40000, 3.60000,
            3.80000, 4.00000, 4.20000, 4.40000, 4.60000, 4.80000, 5.00000, 5.20000, 5.40000, 5.60000, 5.80000, 6.00000};
        private double[] y = {0.59554, 0.58813, 0.64181, 0.68587, 0.44783, 0.40836, 0.38241, -0.05933, -0.12478, -0.36847, -0.39935,
            -0.50881, -0.63435, -0.59979, -0.64107, -0.51808, -0.38127, -0.12349, -0.09624, 0.27893, 0.48965, 0.33089, 0.70615,
            0.53342, 0.43321, 0.64790, 0.48834, 0.18440, -0.02389, -0.10261, -0.33594, -0.35101, -0.62027, -0.55719, -0.66377, -0.62740};

        private double Dekodowanie(string cb, int LBnP, double ZDMin, double ZD)
        {
            int ctmp = 0;
            int b = 0;
            for (int p = LBnP - 1; p >= 0; p--)
            {
                ctmp += int.Parse(cb[p].ToString()) * (int)Math.Pow(2, b);
                b++;
            }
            return ZDMin + ctmp / (Math.Pow(2, LBnP) - 1) * ZD;
        }

        private Osobnik Turniej(Osobnik[] osobniki, int TurRozm)
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
                if (sklad[i].Ocena < os_wybr.Ocena)
                {
                    os_wybr = sklad[i];
                }
            }
            return new Osobnik(os_wybr.Cb, (double[])os_wybr.Parametr.Clone(), os_wybr.Ocena);
        }

        private Osobnik[] Mutacja(Osobnik[] nowa_pula, int LBnP, int populacja, int parametry)
        {
            for (int i = 4; i < populacja - 1; i++)
            {
                int b_punkt = rand.Next(0, LBnP * parametry);
                char[] mutacja_cb = nowa_pula[i].Cb.ToCharArray();
                mutacja_cb[b_punkt] = mutacja_cb[b_punkt] == '0' ? '1' : '0';
                nowa_pula[i].Cb = new string(mutacja_cb);
            }
            return nowa_pula;
        }

        private Osobnik[] Krzyzowanie(Osobnik[] nowa_pula, int LBnP, int parametry)
        {
            int[] ktore = { 0, 1, 2, 3, 8, 9, nowa_pula.Length - 3, nowa_pula.Length - 2 };
            for (int k = 0; k < ktore.Length; k += 2)
            {
                int a = ktore[k];
                int b = ktore[k + 1];
                if (nowa_pula[a] == null || nowa_pula[b] == null) continue;

                int b_ciecie = rand.Next(0, LBnP * parametry - 1);
                char[] cba = nowa_pula[a].Cb.ToCharArray();
                char[] cbb = nowa_pula[b].Cb.ToCharArray();

                for (int i = b_ciecie; i < LBnP * parametry; i++)
                {
                    (cba[i], cbb[i]) = (cbb[i], cba[i]);
                }

                nowa_pula[a].Cb = new string(cba);
                nowa_pula[b].Cb = new string(cbb);
            }
            return nowa_pula;
        }

        private double srednia(Osobnik[] osobniki)
        {
            double suma = 0;
            foreach (var o in osobniki)
            {
                suma += o.Ocena;
            }
            return suma / osobniki.Length;
        }

        private Osobnik najlepszy(Osobnik[] osobniki)
        {
            var najlepszy = osobniki[0];
            foreach (var o in osobniki)
            {
                if (o.Ocena < najlepszy.Ocena) najlepszy = o;
            }
            return najlepszy;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int LBnP = (int)numericUpDown1.Value;
            int TurRozm = (int)numericUpDown2.Value;
            int populacja = (int)numericUpDown3.Value;
            int parametry = (int)numericUpDown4.Value;
            int generacje = (int)numericUpDown5.Value;
            double ZDMin = 0;
            double ZDMax = 3;
            double ZD = ZDMax - ZDMin;

            richTextBox1.Clear();
            richTextBox1.AppendText("Rozpoczęcie algorytmu...\n");

            Osobnik[] osobniki = new Osobnik[populacja];
            for (int i = 0; i < populacja; i++)
            {
                string cb = "";
                for (int j = 0; j < LBnP * parametry; j++)
                {
                    cb += rand.Next(0, 2);
                }
                osobniki[i] = new Osobnik(cb, new double[parametry], 0);
            }

            for (int i = 0; i < populacja; i++)
            {
                for (int j = 0; j < parametry; j++)
                {
                    osobniki[i].Parametr[j] = Dekodowanie(osobniki[i].Cb.Substring(j * LBnP, LBnP), LBnP, ZDMin, ZD);
                }

                osobniki[i].Ocena = 0;
                for (int j = 0; j < x.Length; j++)
                {
                    double f_x = osobniki[i].Parametr[0] * Math.Sin(osobniki[i].Parametr[1] * x[j] + osobniki[i].Parametr[2]);
                    osobniki[i].Ocena += Math.Pow(y[j] - f_x, 2);
                }
            }

            richTextBox1.AppendText("\nGeneracja 0:\n");
            richTextBox1.AppendText($"Najlepszy: {Math.Round(najlepszy(osobniki).Ocena, 2)}\n");
            richTextBox1.AppendText($"Średnia: {Math.Round(srednia(osobniki), 2)}\n");

            for (int g = 0; g < generacje; g++)
            {
                Osobnik[] nowa_pula = new Osobnik[populacja];

                for (int i = 0; i < populacja - 1; i++)
                {
                    nowa_pula[i] = Turniej(osobniki, TurRozm);
                }

                nowa_pula = Krzyzowanie(nowa_pula, LBnP, parametry);
                nowa_pula = Mutacja(nowa_pula, LBnP, populacja, parametry);

                nowa_pula[populacja - 1] = najlepszy(osobniki);

                osobniki = nowa_pula;

                for (int i = 0; i < populacja; i++)
                {
                    for (int j = 0; j < parametry; j++)
                    {
                        osobniki[i].Parametr[j] = Dekodowanie(osobniki[i].Cb.Substring(j * LBnP, LBnP), LBnP, ZDMin, ZD);
                    }

                    osobniki[i].Ocena = 0;
                    for (int j = 0; j < x.Length; j++)
                    {
                        double f_x = osobniki[i].Parametr[0] * Math.Sin(osobniki[i].Parametr[1] * x[j] + osobniki[i].Parametr[2]);
                        osobniki[i].Ocena += Math.Pow(y[j] - f_x, 2);
                    }
                }

                richTextBox1.AppendText($"\nGeneracja {g + 1}:\n");
                richTextBox1.AppendText($"Najlepszy: {Math.Round(najlepszy(osobniki).Ocena, 2)}\n");
                richTextBox1.AppendText($"Średnia: {Math.Round(srednia(osobniki), 2)}\n");
            }
        }
    }
}
