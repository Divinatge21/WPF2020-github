
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Programm2//лекция 2
{
    public class GradiateTheBrush : Window
    {    //атрибут
        [STAThread]
        //создание окошечка
        public static void Main()
        {
            Application app = new Application();
            app.Run(new GradiateTheBrush());
        }
        //Функция которая выполняет заливку фона
        public GradiateTheBrush()
        {
            //название окошка
            Title = "Gradiate the Brush";
            //функция создающая переход из одного цвета в другой(градиент), и задает начало координат,окуда начинается заливка цвета
            LinearGradientBrush brush = new LinearGradientBrush(Colors.Yellow, Colors.Green, new Point(0, 0), new Point(1, 1));
            //Закрашивает задний фон
            Background = brush;
        }
    }
}
