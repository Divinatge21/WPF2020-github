using System;

using static System.Math;

namespace CATask5
{
    /// <summary>
    /// Класс для рассчета координат снаряда
    /// </summary>
    public static class ParabolicMovement
    {
        /// <summary>
        /// Количество временных отсчетов
        /// </summary>
        private static int count = 0;

        /// <summary>
        /// Начальный угол бросания
        /// </summary>
        private static float alfa = 0;

        /// <summary>
        /// Начальная скорость
        /// </summary>
        private static float speed = 0;

        /// <summary>
        /// Временные отсчеты
        /// </summary>
        private static float[] timeArray;

        /// <summary>
        /// Координаты Х
        /// </summary>
        private static double[] xCoord;

        /// <summary>
        /// Координаты У
        /// </summary>
        private static double[] yCoord;

        /// <summary>
        /// Ускорение свободного падения
        /// </summary>
        private static float g = 9.81f;

        /// <summary>
        /// Резервирование памяти под данные
        /// </summary>
        /// <param name="_count">количество отсчетов</param>
        /// <param name="_alfa">начальный угол броснаия</param>
        /// <param name="_speed">начальная скорость</param>
        public static void MemoryAlloc(int _count, float _alfa, float _speed)
        {
            alfa = _alfa;
            speed = _speed;
            count = _count;

            timeArray = new float[count];
            xCoord = new double[count];
            yCoord = new double[count];
        }

        /// <summary>
        /// Ввод временного отсчета под индексом _index
        /// </summary>
        /// <param name="_index">индекс вводимого временного отсчета</param>
        /// <param name="_value">значение вводимого временного отсчета</param>
        public static void InputItem(int _index, float _value)
        {
            timeArray[_index] = _value;
        }

        /// <summary>
        /// Расчет координат
        /// </summary>
        public static void Calculate()
        {
            for (int i = 0; i < count; i++)
            {
                xCoord[i] = speed * Cos(alfa) * timeArray[i];
                yCoord[i] = speed * Sin(alfa) - g * Pow((timeArray[i]), 2) / 2 ;
            }
        }

        /// <summary>
        /// Печать координат
        /// </summary>
        public static void PrintCoordinates()
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("Coordinates at {0} are next:", timeArray[i]);
                Console.WriteLine("X: {0}", xCoord[i]);
                Console.WriteLine("Y: {0}", yCoord[i]);
            }
        }
    }

    public static class Program
    {
        static void Main()
        {
            /// Вводим начальный угол
            float alfa = 0;
            Console.Write("Введите начальный угол (в радианах): ");
            alfa = Single.Parse(Console.ReadLine());

            /// Вводим начальную скорость
            float speed = 0;
            Console.Write("Введите начальную скорость: ");
            speed = Single.Parse(Console.ReadLine());

            /// Вводим количество временных отсчетов
            int count = 0;
            Console.Write("Введите количество временных отсчетов: ");
            count = Int32.Parse(Console.ReadLine());

            /// Инициализируем класс рассчета координат
            ParabolicMovement.MemoryAlloc(count, alfa, speed);

            for (int i = 0; i < count; i++)
            {
                /// Ввод i-го временного отсчета
                Console.Write("Введите {0} временной отсчет: ", i);
                float j = Single.Parse(Console.ReadLine());

                /// Запись этого значения в класс рассчета
                ParabolicMovement.InputItem(i, j);
            }

            /// Расчет координат
            ParabolicMovement.Calculate();

            /// Печать координат
            ParabolicMovement.PrintCoordinates();

            /// Пауза консоли, чтобы не закрывалось окно
            Console.Read();
        }
    }
}
