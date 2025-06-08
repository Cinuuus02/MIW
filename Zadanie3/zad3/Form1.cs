using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace zad3
{
    public partial class Form1 : Form
    {
        public delegate double Metryka(double[] a, double[] b);
        public class Probka
        {
            public double[] Atrybut;
        }
        public static class MetrykaStatyczna
        {
            public static double Manhattan(double[] a, double[] b)
            {
                double suma = 0;
                for (int i = 0; i < a.Length - 1; i++)
                    suma += Math.Abs(a[i] - b[i]);
                return suma;
            }
            public static double Euklidesowa(double[] a, double[] b)
            {
                double suma = 0;
                for (int i = 0; i < a.Length - 1; i++)
                    suma += Math.Pow(a[i] - b[i], 2);
                return Math.Sqrt(suma);
            }
            public static double Czebyszewa(double[] a, double[] b)
            {
                double max = 0;
                for (int i = 0; i < a.Length - 1; i++)
                {
                    double x = Math.Abs(a[i] - b[i]);
                    if (x > max)
                        max = x;
                }
                return max;
            }
            public static double Minkowskiego(double[] a, double[] b)
            {
                double p = 2;
                double suma = 0;
                for (int i = 0; i < a.Length - 1; i++)
                    suma += Math.Pow(Math.Abs(a[i] - b[i]), p);
                return Math.Pow(suma, 1.0 / p);
            }
            public static double Logarytmiczna(double[] a, double[] b)
            {
                double suma = 0;
                for (int i = 0; i < a.Length - 1; i++)
                {
                    double ai;
                    if (a[i] > 0)
                        ai = a[i];
                    else
                        ai = 1e-10;
                    double bi;
                    if (b[i] > 0)
                        bi = b[i];
                    else
                        bi = 1e-10;
                    suma += Math.Abs(Math.Log(ai) - Math.Log(bi));
                }
                return suma;
            }
        }

        static double Normalizuj(double oryginalna, double min, double max)
        {
            return (oryginalna - min) / (max - min);
        }

        public static int Knn(Probka testowana, Probka[] probki, int k, Metryka metoda, int liczbaAtrybutow)
        {
            double[] odleglosci = new double[probki.Length - 1];
            double[] klasy = new double[probki.Length - 1];
            int i = 0;
            foreach (Probka p in probki)
            {
                if (p == testowana) continue;
                double odleglosc = metoda(testowana.Atrybut, p.Atrybut);
                odleglosci[i] = odleglosc;
                klasy[i] = p.Atrybut[liczbaAtrybutow];
                i++;
            }
            int sl = odleglosci.Length;
            for (int j = 0; j < sl - 1; j++)
            {
                for (int l = 0; l < sl - 1 - j; l++)
                {
                    if (odleglosci[l] > odleglosci[l + 1])
                    {
                        double tempDystans = odleglosci[l];
                        odleglosci[l] = odleglosci[l + 1];
                        odleglosci[l + 1] = tempDystans;
                        double tempKlasa = klasy[l];
                        klasy[l] = klasy[l + 1];
                        klasy[l + 1] = tempKlasa;
                    }
                }
            }
            double[] sasiedzi = new double[k];
            for (int j = 0; j < k; j++)
                sasiedzi[j] = klasy[j];
            Dictionary<double, int> licznik = new Dictionary<double, int>();
            foreach (var s in sasiedzi)
            {
                if (licznik.ContainsKey(s))
                    licznik[s]++;
                else
                    licznik[s] = 1;
            }
            double podobny = sasiedzi[0];
            int glosy = 0;
            bool remis = false;
            foreach (var l in licznik)
            {
                if (l.Value > glosy)
                {
                    glosy = l.Value;
                    podobny = l.Key;
                    remis = false;
                }
                else if (l.Value == glosy)
                {
                    remis = true;
                }
            }
            int wynik = (int)podobny;
            if (remis)
                return -1;
            else
                return wynik;
        }

        private Probka[] probki;
        private int liczbaAtrybutow;
        private MethodInfo[] metody;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            metody = typeof(MetrykaStatyczna)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.ReturnType == typeof(double)
                            && m.GetParameters().Length == 2
                            && m.GetParameters()[0].ParameterType == typeof(double[])
                            && m.GetParameters()[1].ParameterType == typeof(double[]))
                .ToArray();
            comboBox1.DataSource = metody;
            comboBox1.DisplayMember = "Name";
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (probki == null)
            {
                MessageBox.Show("Brak danych");
                return;
            }
            int kValue = (int)k.Value;

            MethodInfo metodaInfo = (MethodInfo)comboBox1.SelectedItem;

            Metryka m = (Metryka)Delegate.CreateDelegate(typeof(Metryka), metodaInfo);

            double licznik = 0;
            foreach (Probka p in probki)
            {
                if (p.Atrybut[liczbaAtrybutow] == Knn(p, probki, kValue, m, liczbaAtrybutow))
                    licznik++;
            }
            label1.Text = $"Skuteczność: {licznik / probki.Length * 100} %";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] wiersze = File.ReadAllLines("probki.txt");
            int liczbaProbek = wiersze.Length;
            probki = new Probka[liczbaProbek];

            for (int i = 0; i < liczbaProbek; i++)
            {
                string[] elementy = wiersze[i].Trim().Replace("   ", " ").Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                Probka p = new Probka();
                p.Atrybut = new double[elementy.Length];
                for (int j = 0; j < elementy.Length; j++)
                {
                    double.TryParse(elementy[j], out p.Atrybut[j]);
                }
                probki[i] = p;
            }
            liczbaAtrybutow = probki[0].Atrybut.Length - 1;

            double[] min = new double[liczbaAtrybutow];
            double[] max = new double[liczbaAtrybutow];
            for (int i = 0; i < liczbaAtrybutow; i++)
            {
                min[i] = probki[0].Atrybut[i];
                max[i] = probki[0].Atrybut[i];
            }
            foreach (Probka p in probki)
            {
                for (int i = 0; i < liczbaAtrybutow; i++)
                {
                    if (p.Atrybut[i] < min[i])
                        min[i] = p.Atrybut[i];
                    if (p.Atrybut[i] > max[i])
                        max[i] = p.Atrybut[i];
                }
            }
            foreach (Probka p in probki)
            {
                for (int i = 0; i < liczbaAtrybutow; i++)
                {
                    p.Atrybut[i] = Normalizuj(p.Atrybut[i], min[i], max[i]);
                }
            }
            label1.Text = "Dane wczytane";
        }
    }
}
