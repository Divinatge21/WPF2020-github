using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace WPFATask01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer1;

        double t;  // текущее время
        double dt; // шаг интегрирования
        double[] vw = new double[2]; // v wind
        double[] v = new double[2];  // v object
        double v_mod = 0;
        double[] R = new double[2];  // координаты 
        double dg;
        double[] Q = new double[2];  // силы сопротивления воздуха
        double Q_mod; // модуль силы сопротивления воздуха
        double Psi;   // направление силы сопротивления воздуха в гор. плоскости
        double Tet;   // направление силы сопротивления воздуха в вертикальной плоскости

        double[] va = new double[2];  // вектор скорости объекта относительно потока
        double va_mod, vaero2;        // модуль и квадрат вектора скорости объекта относительно потока

        double ro = 1.29; //[kg/m3]
        double q;       // [kg/(m*c2)]
        double r = 0;   // Радиус [m2]
        double S;       // Площад сечения [m2]
        const double cx = 0.4;  //[-]
        double g = 9.8; // [m/s2]
        double m = 0;  // [kg]
        double hmax = 0;
        double xmax = 0;
        double tmax = 0;
        double k;
        public MainWindow()
        {
            InitializeComponent();

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += timer1_Tick;
            timer1.Start();

            ChartArea chartArea1 = new ChartArea();
            Legend legend1 = new Legend();
            Series series1 = new Series();
            ChartArea chartArea2 = new ChartArea();
            Legend legend2 = new Legend();
            Series series2 = new Series();
            Series series3 = new Series();
            ChartArea chartArea3 = new ChartArea();
            Legend legend3 = new Legend();
            Series series4 = new Series();
            Series series5 = new Series();
            Series series6 = new Series();

            // 
            // chart3
            // 
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.Title = "x, [м] - Дальность в горизонтальной плоскости";
            chartArea1.AxisY.TextOrientation = TextOrientation.Rotated270;
            chartArea1.AxisY.Title = "y, [м] - Высота";
            chart3.ChartAreas.Add(chartArea1);
            chart3.Legends.Add(legend1);
            series1.ChartType = SeriesChartType.Line;
            series1.LegendText = "H(x)";
            chart3.Series.Add(series1);
            chart3.Update();

            //// 
            //// chart1
            //// 
            chartArea2.AxisX.Title = "t, [c]";
            chart1.ChartAreas.Add(chartArea2);
            chart1.Legends.Add(legend2);
            series2.ChartType = SeriesChartType.FastLine;
            series2.LegendText = "h(t) - высота";
            series2.MarkerSize = 8;
            series3.ChartType = SeriesChartType.FastLine;
            series3.LegendText = "x(t)";
            chart1.Series.Add(series2);
            chart1.Series.Add(series3);
            chart1.Update();

            //// 
            //// chart2
            //// 
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.AxisX.Title = "t, [c]";
            chart2.ChartAreas.Add(chartArea3);
            chart2.Legends.Add(legend3);
            series4.ChartType = SeriesChartType.Line;
            series4.LegendText = "Vy(t)";
            series5.ChartType = SeriesChartType.Line;
            series5.LegendText = "Vx(t)";
            series6.ChartType = SeriesChartType.FastLine;
            series6.LegendText = "v_mod(t)";
            chart2.Series.Add(series4);
            chart2.Series.Add(series5);
            chart2.Series.Add(series6);
            chart2.Update();
        }

        // процедура чтения данных из формы
        private void GetDataFromForm(double[] wR, double[] wV, double[] wVwind, ref double wr, ref double wm)
        {
            wR[0] = 0;
            wR[1] = Convert.ToDouble(HTextBox.Text);

            wV[0] = Convert.ToDouble(V_xTextBox.Text);
            wV[1] = Convert.ToDouble(V_yTextBox.Text);


            wVwind[0] = Convert.ToDouble(Vw_xTextBox.Text);
            wVwind[1] = Convert.ToDouble(Vw_yTextBox.Text);

            wr = Convert.ToDouble(rTextBox.Text);
            wm = Convert.ToDouble(mTextBox.Text);
        }

        // Очистка графиков
        private void ClearGrafics()
        {
            // Очистка графиков
            for (int i = 0; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].BorderWidth = 2;
                chart1.Series[i].Points.Clear();
            }

            for (int i = 0; i < chart2.Series.Count; i++)
            {
                chart2.Series[i].BorderWidth = 2;
                chart2.Series[i].Points.Clear();
            }

            for (int i = 0; i < chart3.Series.Count; i++)
            {
                chart3.Series[i].BorderWidth = 2;
                chart3.Series[i].Points.Clear();
            }
        }

        // Обработка нажатия кнопки Расчет
        private void CalcBtn_Click(object sender, EventArgs e)
        {
            // Очистка графиков
            ClearGrafics();

            t = 0;
            dt = 0.05;

            GetDataFromForm(R, v, vw, ref r, ref m);
            hmax = R[1];
            S = Math.PI * Math.Pow(r, 2); // [m2]

            while (R[1] > 0) // пока высота больше 0
            {
                t = t + dt;

                // вектор скорости объекта относительно потока 
                for (int i = 0; i < 2; i++)
                    va[i] = v[i] - vw[i];

                // модуль вектора скорости объекта относительно потока 
                va_mod = Math.Sqrt(va[0] * va[0] + va[1] * va[1]);

                // ориентация вектора скорости объекта относительно набегающего потока
                // сила сопротивления - против скорости объекта относительно потока

                // скоростной напор - q
                q = ro * Math.Pow(va_mod, 2) / 2;

                // Сила сопротивления Q
                Q_mod = cx * q * S;

                k = cx * ro * va_mod / 2;

                // ----------------------------------------------------------------------------------
                // решение уравнения движения (2 закон ньютона) методом эйлера с шагом dt
                // ----------------------------------------------------------------------------------
                v[0] = v[0] - dt * (k * va[0] / m);
                v[1] = v[1] - dt * (g + k * va[1] / m);

                // определение координат
                R[0] = v[0] * dt + R[0];
                R[1] = v[1] * dt + R[1];

                if (R[1] > hmax) hmax = R[1];
                if (R[0] > xmax) xmax = R[0];
                v_mod = Math.Sqrt(v[0] * v[0] + v[1] * v[1]);

                // построение графиков
                if (R[1] > 0)
                {
                    chart1.Series[0].Points.AddXY(t, R[1]);
                    chart1.Series[1].Points.AddXY(t, R[0]);

                    chart2.Series[0].Points.AddXY(t, v[1]);
                    chart2.Series[1].Points.AddXY(t, v[0]);
                    chart2.Series[2].Points.AddXY(t, v_mod);

                    chart3.Series[0].Points.AddXY(R[0], R[1]);
                }
            }
            tmax = t;
            listBox1.Items.Clear();
            listBox1.Items.Add("Время полета = " + String.Format("{0:0.00}", t) + " c");
            listBox1.Items.Add("Дальность полета = " + String.Format("{0:0.00}", R[0]) + " м");
            listBox1.Items.Add("Максимальная высота полета = " + String.Format("{0:0.00}", hmax) + " м");
            listBox1.Items.Add("Коодинаты точки падения:");
            listBox1.Items.Add("Rx = " + String.Format("{0:0.00}", R[0]) + " м");
            listBox1.Items.Add("Ry = " + String.Format("{0:0.00}", R[1]) + " м");
            listBox1.Items.Add("Скорость в точке падения:");
            listBox1.Items.Add("Vx = " + String.Format("{0:0.00}", v[0]) + " м/c");
            listBox1.Items.Add("Vy = " + String.Format("{0:0.00}", v[1]) + " м/c");
            listBox1.Items.Add("Модуль скорости V = " + String.Format("{0:0.00}", v_mod) + " м/c");
        }

        // обработка таймера, расчет одного такта 
        private void timer1_Tick(object sender, EventArgs e)
        {
            t = t + dt;

            // вектор скорости объекта относительно потока 
            for (int i = 0; i < 2; i++)
                va[i] = v[i] - vw[i];

            // модуль вектора скорости объекта относительно потока 
            va_mod = Math.Sqrt(va[0] * va[0] + va[1] * va[1]);

            // ориентация вектора скорости объекта относительно набегающего потока
            // сила сопротивления - против скорости объекта относительно потока

            // скоростной напор - q
            q = ro * Math.Pow(va_mod, 2) / 2;

            // Сила сопротивления Q
            Q_mod = cx * q * S;
            k = cx * ro * va_mod / 2;

            // ----------------------------------------------------------------------------------
            // решение уравнения движения (2 закон ньютона) методом эйлера с шагом dt
            // ----------------------------------------------------------------------------------
            v[0] = v[0] - dt * (k * va[0] / m);
            v[1] = v[1] - dt * (g + k * va[1] / m);

            // определение координат
            R[0] = v[0] * dt + R[0];
            R[1] = v[1] * dt + R[1];

            if (R[1] > hmax) hmax = R[1];
            v_mod = Math.Sqrt(v[0] * v[0] + v[1] * v[1]);

            if (R[1] > 0)
            {
                // построение графиков
                chart1.Series[0].Points.AddXY(t, R[1]);
                chart1.Series[1].Points.AddXY(t, R[0]);

                chart2.Series[0].Points.AddXY(t, v[1]);
                chart2.Series[1].Points.AddXY(t, v[0]);
                chart2.Series[2].Points.AddXY(t, v_mod);

                chart3.Series[0].Points.AddXY(R[0], R[1]);
            }
            else
            {
                // завершить работу таймера
                timer1.Stop();
            }
        }

        // обработка нажатия кнопки Анимация
        private void AnimationBtn_Click(object sender, EventArgs e)
        {
            // просчет для определения пределов и выдачи результатов
            CalcBtn_Click(sender, e);

            // Задание масимальных значений по осям
            chart3.ChartAreas[0].AxisX.Maximum = (Math.Floor(xmax / 50) + 1) * 50;
            chart3.ChartAreas[0].AxisY.Maximum = (Math.Floor(hmax / 50) + 1) * 50;
            chart1.ChartAreas[0].AxisX.Maximum = (Math.Floor(tmax / 2) + 1) * 2;
            chart2.ChartAreas[0].AxisX.Maximum = (Math.Floor(tmax / 2) + 1) * 2;


            // Очистка графиков
            ClearGrafics();

            // Начальные данные
            t = 0;
            dt = 0.05;

            // данные из формы
            GetDataFromForm(R, v, vw, ref r, ref m);

            hmax = R[1];
            xmax = R[0];
            S = Math.PI * Math.Pow(r, 2); // [m2]

            // запуск таймера
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            timer1.Start();
        }
    }
}