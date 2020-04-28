using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
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
        
        public Form1()
        {
            InitializeComponent();
        }

        // процедура чтения данных из формы
        private void GetDataFromForm(double[] wR, double[] wV, double[] wVwind, ref  double wr, ref  double wm)
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
            return;
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
                v[0] = v[0] - dt*(k*va[0] / m);
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
            listBox1.Items.Add("Время полета = " + String.Format("{0:0.00}",t) + " c");
            listBox1.Items.Add("Дальность полета = " + String.Format("{0:0.00}",R[0]) +" м");
            listBox1.Items.Add("Максимальная высота полета = " + String.Format("{0:0.00}",hmax) + " м");
            listBox1.Items.Add("Коодинаты точки падения:");
            listBox1.Items.Add("Rx = " + String.Format("{0:0.00}", R[0]) + " м");
            listBox1.Items.Add("Ry = " + String.Format("{0:0.00}", R[1]) + " м");
            listBox1.Items.Add("Скорость в точке падения:");
            listBox1.Items.Add("Vx = " + String.Format("{0:0.00}",v[0]) + " м/c");
            listBox1.Items.Add("Vy = " + String.Format("{0:0.00}",v[1]) + " м/c");
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
                timer1.Enabled = false;
                return;
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
            timer1.Interval = 20;
            timer1.Enabled = true;
        }
    }
}
