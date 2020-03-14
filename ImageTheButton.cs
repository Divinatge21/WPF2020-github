
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Petzold.ImageTheButton
{
    public class ImageTheButton : Window
    {
        [STAThread]
        //создание окошечка
        public static void Main()
        {
            Application app = new Application();
            app.Run(new ImageTheButton());
        }
        public ImageTheButton()
        {
            //Название окошечка
            Title = "Image the Button";

            Uri uri = new Uri("pack://application:,,/munch.png");
            BitmapImage bitmap = new BitmapImage(uri);

            //установить источник изображения
            Image img = new Image();
            img.Source = bitmap;
            img.Stretch = Stretch.None;
            //Выравнивание картинки по по центру окешечка
            Button btn = new Button();
            btn.Content = img;
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;

            Content = btn;
        }
    }
}
