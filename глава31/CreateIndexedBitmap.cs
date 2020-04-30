
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Petzold.CreateIndexedBitmap
{
    public class CreateIndexedBitmap : Window
    {
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new CreateIndexedBitmap());
        }
        public CreateIndexedBitmap()
        {
            Title = "Create Indexed Bitmap";

            // ������� ������� � 256 �������, ����������� ������� � �����.
            List<Color> colors = new List<Color>();

            for (int r = 0; r < 256; r += 17)
            for (int b = 0; b < 256; b += 17)
               colors.Add(Color.FromRgb((byte)r, 0, (byte)b));

            BitmapPalette palette = new BitmapPalette(colors);

            // ������� ������� ������
            byte[] array = new byte[256 * 256];

            for (int x = 0; x < 256; x++)
            for (int y = 0; y < 256; y++)
                array[256 * y + x] = (byte)(((int)Math.Round(y / 17.0) << 4) |
                                             (int)Math.Round(x / 17.0));
            // ������� ��������� �����������.
            BitmapSource bitmap = 
                BitmapSource.Create(256, 256, 96, 96, PixelFormats.Indexed8,
                                    palette, array, 256);

            // ������� ������ Image � ������������� ��� Source � ��������� �����������.
            Image img = new Image();
            img.Source = bitmap;

            // ������ ������ Image ���������� ����.
            Content = img;
        }
    }
}