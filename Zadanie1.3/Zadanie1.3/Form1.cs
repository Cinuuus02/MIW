using System;
using System.Windows.Forms;

namespace Zadanie1._3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class Osobnik
        {
            public string Cb;
            public double[] Parametr;
            public double Ocena;

            public Osobnik(string cb, int parametry)
            {
                Cb = cb;
                Parametr = new double[parametry];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int LBnP = (int)numericUpDown1.Value;
            int TurRozm = (int)numericUpDown2.Value;
            int populacja = (int)numericUpDown3.Value;
            int parametry = 9;
            int generacje = (int)numericUpDown5.Value;
            double ZDMin = -10;
            double ZDMax = 10;
            double ZD = ZDMax - ZDMin;

            double[][] x = {
                new double[] {0, 0},
                new double[] {0, 1},
                new double[] {1, 0},
                new double[] {1, 1}
            };
            double[] y = { 0, 1, 1, 0 };

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
                    if (sklad[i].Ocena < os_wybr.Ocena)
                    {
                        os_wybr = sklad[i];
                    }
                }
                return new Osobnik(os_wybr.Cb, parametry);
            }

            Osobnik[] Mutacja(Osobnik[] nowa_pula)
            {
                for (int i = 4; i < populacja - 1; i++)
                {
                    int b_punkt = rand.Next(0, LBnP * parametry);
                    string cb = nowa_pula[i].Cb;
                    char[] mutacja_cb = cb.ToCharArray();
                    mutacja_cb[b_punkt] = mutacja_cb[b_punkt] == '0' ? '1' : '0';
                    nowa_pula[i].Cb = new string(mutacja_cb);
                }
                return nowa_pula;
            }

            Osobnik[] Krzyzowanie(Osobnik[] nowa_pula)
            {
                int[] ktore = { 0, 1, 2, 3, 8, 9, nowa_pula.Length - 3, nowa_pula.Length - 2 };
                for (int k = 0; k < ktore.Length; k = k + 2)
                {
                    int a = ktore[k];
                    int b = ktore[k + 1];
                    if (nowa_pula[a] == null || nowa_pula[b] == null)
                        continue;

                    int b_ciecie = rand.Next(0, LBnP * parametry - 1);
                    string cba = nowa_pula[a].Cb;
                    string cbb = nowa_pula[b].Cb;
                    char[] krzyzowanie_cba = cba.ToCharArray();
                    char[] krzyzowanie_cbb = cbb.ToCharArray();
                    char[] temp_a = new char[LBnP * parametry];
                    char[] temp_b = new char[LBnP * parametry];
                    for (int i = 0; i < b_ciecie; i++)
                    {
                        temp_a[i] = krzyzowanie_cba[i];
                        temp_b[i] = krzyzowanie_cbb[i];
                    }
                    for (int i = b_ciecie; i < LBnP * parametry; i++)
                    {
                        temp_a[i] = krzyzowanie_cbb[i];
                        temp_b[i] = krzyzowanie_cba[i];
                    }
                    nowa_pula[a].Cb = new string(temp_a);
                    nowa_pula[b].Cb = new string(temp_b);
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
                    if (osobniki[i].Ocena < najlepszy.Ocena)
                    {
                        najlepszy = osobniki[i];
                    }
                }
                return najlepszy;
            }

            double Sigma(double x)
            {
                double e = Math.E;
                double wynik = 1 / (1 + Math.Pow(e, -x));
                return wynik;
            }

            double Skalar(double[] we, double[] parametryW)
            {
                double sum = 0;
                for (int i = 0; i < we.Length; i++)
                {
                    sum += we[i] * parametryW[i];
                }
                sum += parametryW[parametryW.Length - 1];
                return sum;
            }

            double Siec(double[] parametryW, double[] wejscie)
            {
                double[] neuron1 = { wejscie[0], wejscie[1] };
                double[] n1_parametry = { parametryW[1], parametryW[2], parametryW[0] };
                double n1 = Sigma(Skalar(neuron1, n1_parametry));

                double[] neuron2 = { wejscie[0], wejscie[1] };
                double[] n2_parametry = { parametryW[4], parametryW[5], parametryW[3] };
                double n2 = Sigma(Skalar(neuron2, n2_parametry));

                double[] we = { n1, n2 };
                double[] we_parametry = { parametryW[7], parametryW[8], parametryW[6] };
                double wy = Sigma(Skalar(we, we_parametry));

                return wy;
            }

            Osobnik[] osobniki = new Osobnik[populacja];
            for (int i = 0; i < populacja; i++)
            {
                string cb = "";
                for (int j = 0; j < LBnP * parametry; j++)
                {
                    cb += rand.Next(0, 2);
                }
                osobniki[i] = new Osobnik(cb, parametry);
            }

            for (int i = 0; i < populacja; i++)
            {
                osobniki[i].Ocena = 0;
                for (int j = 0; j < parametry; j++)
                {
                    osobniki[i].Parametr[j] = Dekodowanie(osobniki[i].Cb.Substring(j * LBnP, LBnP));
                }
                for (int j = 0; j < x.Length; j++)
                {
                    double f_x = Siec(osobniki[i].Parametr, x[j]);
                    osobniki[i].Ocena += Math.Pow(y[j] - f_x, 2);
                }
            }

            richTextBox1.Clear();
            richTextBox1.AppendText("Generacja 0:\n");
            richTextBox1.AppendText("Najlepszy osobnik: " + Math.Round(najlepszy(osobniki).Ocena, 5) + "\n");
            richTextBox1.AppendText("Srednia ocena: " + Math.Round(srednia(osobniki), 5) + "\n");

            for (int g = 0; g < generacje; g++)
            {
                if (g % 100 == 0 || g == generacje - 1)
                {
                    richTextBox1.AppendText($"\nGeneracja {g + 1}:\n");
                }

                Osobnik[] nowa_pula = new Osobnik[populacja];

                for (int i = 0; i < populacja - 1; i++)
                {
                    nowa_pula[i] = Turniej(osobniki);
                }

                nowa_pula = Krzyzowanie(nowa_pula);
                nowa_pula = Mutacja(nowa_pula);

                nowa_pula[populacja - 1] = najlepszy(osobniki);

                osobniki = nowa_pula;

                for (int i = 0; i < populacja; i++)
                {
                    osobniki[i].Ocena = 0;
                    for (int j = 0; j < parametry; j++)
                    {
                        osobniki[i].Parametr[j] = Dekodowanie(osobniki[i].Cb.Substring(j * LBnP, LBnP));
                    }
                    for (int j = 0; j < x.Length; j++)
                    {
                        double f_x = Siec(osobniki[i].Parametr, x[j]);
                        osobniki[i].Ocena += Math.Pow(y[j] - f_x, 2);
                    }
                }

                if (g % 100 == 0 || g == generacje - 1)
                {
                    richTextBox1.AppendText("Najlepszy osobnik: " + Math.Round(najlepszy(osobniki).Ocena, 5) + "\n");
                    richTextBox1.AppendText("Srednia ocena: " + Math.Round(srednia(osobniki), 5) + "\n");
                }
            }
        }
    }
}
