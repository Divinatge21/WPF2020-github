
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Petzold.PopupContextMenu
{
    public class PopupContextMenu : Window
    {
        ContextMenu menu;
        MenuItem itemBold, itemItalic;
        MenuItem[] itemDecor;
        Inline inlClicked;

        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new PopupContextMenu());
        }
        public PopupContextMenu()
        {
            Title = "Popup Context Menu";

            // Создать контекстное меню.
            menu = new ContextMenu();

            //Добавьте элемент для "Bold".
            itemBold = new MenuItem();
            itemBold.Header = "Bold";
            menu.Items.Add(itemBold);

            // Добавьте пункт для «Курсив».
            itemItalic = new MenuItem();
            itemItalic.Header = "Italic";
            menu.Items.Add(itemItalic);

            //Получить всех членов текста украшения места.
            TextDecorationLocation[] locs = 
                (TextDecorationLocation[]) 
                    Enum.GetValues(typeof(TextDecorationLocation));

            // Create an array of MenuItem objects and fill them up.
            itemDecor = new MenuItem[locs.Length];

            for (int i = 0; i < locs.Length; i++)
            {
                TextDecoration decor = new TextDecoration();
                decor.Location = locs[i];

                itemDecor[i] = new MenuItem();
                itemDecor[i].Header = locs[i].ToString();
                itemDecor[i].Tag = decor;
                menu.Items.Add(itemDecor[i]);
            }

            // Используйте один обработчик для всего контекстного меню.
            menu.AddHandler(MenuItem.ClickEvent, 
                            new RoutedEventHandler(MenuOnClick));

            //Создайте TextBlock как содержимое окна.
            TextBlock text = new TextBlock();
            text.FontSize = 32;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            Content = text;

            // Разбейте известную цитату на слова.
            string strQuote = "To be, or not to be, that is the question";
            string[] strWords = strQuote.Split();

            // Сделайте каждое слово выполненным и добавьте его в TextBlock.
            foreach (string str in strWords)
            {
                Run run = new Run(str);

                //Убедитесь, что TextDecorations является актуальной коллекцией!
                run.TextDecorations = new TextDecorationCollection();
                text.Inlines.Add(run);
                text.Inlines.Add(" ");
            }
        }
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs args)
        {
            base.OnMouseRightButtonUp(args);

            if ((inlClicked = args.Source as Inline) != null)
            {
                // Проверьте пункты меню в соответствии со свойствами InLine.
                itemBold.IsChecked = (inlClicked.FontWeight == FontWeights.Bold);
                itemItalic.IsChecked = (inlClicked.FontStyle == FontStyles.Italic);

                foreach (MenuItem item in itemDecor)
                    item.IsChecked = (inlClicked.TextDecorations.Contains
                            (item.Tag as TextDecoration));

                // Отображение контекстного меню.
                menu.IsOpen = true;
                args.Handled = true;
            }
        }
        void MenuOnClick(object sender, RoutedEventArgs args)
        {
            MenuItem item = args.Source as MenuItem;

            item.IsChecked ^= true;

            // Измените Inline на основе отмеченного или непроверенного элемента.
            if (item == itemBold)
                inlClicked.FontWeight = 
                    (item.IsChecked ? FontWeights.Bold : FontWeights.Normal);

            else if (item == itemItalic)
                inlClicked.FontStyle = 
                    (item.IsChecked ? FontStyles.Italic : FontStyles.Normal);

            else
            {
                if (item.IsChecked)
                    inlClicked.TextDecorations.Add(item.Tag as TextDecoration);
                else
                    inlClicked.TextDecorations.Remove(item.Tag as TextDecoration);
            }
        }
    }
}
