
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Petzold.ShowMyFace
{
    class ShowMyFace : Window
    {
        [STAThread]
        //Создание окошечка
        public static void Main()
        {
            Application app = new Application();
            app.Run(new ShowMyFace());
        }
        public ShowMyFace()
        {
            //Заголовок окошечка
            Title = "Show My Face";
            //добовление фонового изображения
            //расположения файла с картинкой
            Uri uri = new Uri("http://www.charlespetzold.com/PetzoldTattoo.jpg");
            //базовый класс
            BitmapImage bitmap = new BitmapImage(uri);
            //абстрактный класс
            Image img = new Image();
            // преобразование классов
            img.Source = bitmap;
            Content = img;
        }
    }
}
