
using System;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Petzold.DumpControlTemplate
{
    public partial class DumpControlTemplate : Window
    {
        Control ctrl;

        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new DumpControlTemplate());
        }
        public DumpControlTemplate()
        {
            InitializeComponent();
        }

        // Обработчик события для элемента, нажатого в меню «Управление».
        void ControlItemOnClick(object sender, RoutedEventArgs args)
        {
            // Удалить любого существующего потомка из первой строки таблицы.
            for (int i = 0; i < grid.Children.Count; i++)
                if (Grid.GetRow(grid.Children[i]) == 0)
                {
                    grid.Children.Remove(grid.Children[i]);
                    break;
                }

            // Очистить TextBox.
            txtbox.Text = "";

            // Получить класс Control для выбранного элемента меню.
            MenuItem item = args.Source as MenuItem;
            Type typ = (Type)item.Tag;

            // Подготовка к созданию объекта этого типа.
            ConstructorInfo info = typ.GetConstructor(System.Type.EmptyTypes);

            // Попробуйте создать объект этого типа.
            try
            {
                ctrl = (Control)info.Invoke(null);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Title);
                return;
            }

            // Попробуйте создать объект этого типа.
            try
            {
                grid.Children.Add(ctrl);
            }
            catch
            {
                if (ctrl is Window)
                    (ctrl as Window).Show();
                else
                    return;
            }
            Title = Title.Remove(Title.IndexOf('-')) + "- " + typ.Name;
        }
        // При открытии меню дампа включить элементы.
        void DumpOnOpened(object sender, RoutedEventArgs args)
        {
            itemTemplate.IsEnabled = ctrl != null;
            itemItemsPanel.IsEnabled = ctrl != null && ctrl is ItemsControl;
        }
        // Объект шаблона дампа прикреплен к свойству ControlTemplate.
        void DumpTemplateOnClick(object sender, RoutedEventArgs args)
        {
            if (ctrl != null)
                Dump(ctrl.Template);
        }
        // Дамп объекта ItemsPanelTemplate, прикрепленный к свойству ItemsPanel.
        void DumpItemsPanelOnClick(object sender, RoutedEventArgs args)
        {
            if (ctrl != null && ctrl is ItemsControl)
                Dump((ctrl as ItemsControl).ItemsPanel);
        }
        // Дамп шаблона.
        void Dump(FrameworkTemplate template)
        {
            if (template != null)
            {
                // Дамп XAML в TextBox.
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = new string(' ', 4);
                settings.NewLineOnAttributes = true;

                StringBuilder strbuild = new StringBuilder();
                XmlWriter xmlwrite = XmlWriter.Create(strbuild, settings);

                try
                {
                    XamlWriter.Save(template, xmlwrite);
                    txtbox.Text = strbuild.ToString();
                }
                catch (Exception exc)
                {
                    txtbox.Text = exc.Message;
                }
            }
            else
                txtbox.Text = "no template";
        }
    }
}
