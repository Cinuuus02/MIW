using System;
using System.IO;
using System.Windows.Forms;

namespace Zadanie2._1
{
    public partial class Form1 : Form
    {
        double[][] x = {
            new double[] {0, 0},
            new double[] {0, 1},
            new double[] {1, 0},
            new double[] {1, 1}
        };
        double[] y = { 0, 1, 1, 0 };

        double[] wagi1 = new double[3];
        double[] wagi2 = new double[3];
        double[] wagi3 = new double[3];
        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int epoki = (int)numericUpDown1.Value;
            double u = Convert.ToDouble(textBox1.Text);
            double b = 1;

            for (int i = 0; i < wagi1.Length; i++)
                wagi1[i] = rand.NextDouble() * 10 - 5;
            for (int i = 0; i < wagi2.Length; i++)
                wagi2[i] = rand.NextDouble() * 10 - 5;
            for (int i = 0; i < wagi3.Length; i++)
                wagi3[i] = rand.NextDouble() * 10 - 5;

            richTextBox1.Clear();

            for (int ep = 0; ep < epoki; ep++)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    double[] we = x[i];
                    double oczekiwana = y[i];

                    double n1 = Sigma(Skalar(we, wagi1));
                    double n2 = Sigma(Skalar(we, wagi2));
                    double wy = Sigma(Skalar(new double[] { n1, n2 }, wagi3));

                    double blad = oczekiwana - wy;
                    double delta_wy = u * blad;

                    if (ep == 0 || ep == epoki - 1)
                    {
                        richTextBox1.AppendText($"Epoka {ep+1}, wejście: {we[0]}, {we[1]}, błąd: {blad}\n");
                    }

                    double delta_s = delta_wy * b * wy * (1 - wy);

                    wagi3[0] += delta_s * n1;
                    wagi3[1] += delta_s * n2;
                    wagi3[2] += delta_s * 1;

                    double blad_n1 = delta_s * wagi3[0];
                    double blad_n2 = delta_s * wagi3[1];

                    double delta_n1 = blad_n1 * n1 * (1 - n1);
                    double delta_n2 = blad_n2 * n2 * (1 - n2);

                    wagi1[0] += delta_n1 * we[0];
                    wagi1[1] += delta_n1 * we[1];
                    wagi1[2] += delta_n1 * 1;

                    wagi2[0] += delta_n2 * we[0];
                    wagi2[1] += delta_n2 * we[1];
                    wagi2[2] += delta_n2 * 1;
                }
            }

            string w1 = string.Join(", ", wagi1);
            string w2 = string.Join(", ", wagi2);
            string w3 = string.Join(", ", wagi3);
            string content = $"wagi1:{w1}\nwagi2:{w2}\nwagi3:{w3}\n";
            File.WriteAllText("../../../../wagi.txt", content);
            richTextBox1.AppendText("");
            richTextBox1.AppendText("\nZapisane w wagi.txt\n");
            richTextBox1.AppendText($"wagi1: {w1}\n");
            richTextBox1.AppendText($"wagi2: {w2}\n");
            richTextBox1.AppendText($"wagi3: {w3}\n");
        }

        double Sigma(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        double Skalar(double[] x, double[] wagi)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * wagi[i];
            sum += wagi[wagi.Length - 1];
            return sum;
        }
    }
}
