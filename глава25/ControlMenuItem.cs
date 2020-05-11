//------------------------------------------------
// ControlMenuItem.cs (c) 2006 by Charles Petzold
//------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Petzold.DumpControlTemplate
{
    public class ControlMenuItem : MenuItem
    {
        public ControlMenuItem()
        {
            // Получить сборку, в которой определен класс Control.
            Assembly asbly = Assembly.GetAssembly(typeof(Control));

            // Это массив всех типов в этом классе.
            Type[] atype = asbly.GetTypes();

            // Мы собираемся сохранить потомков Control в отсортированном списке.
            SortedList<string, MenuItem> sortlst = 
                                    new SortedList<string, MenuItem>();

            Header = "Control";
            Tag = typeof(Control);
            sortlst.Add("Control", this);

            // Перечисляем все типы в массиве.
            // Для элемента управления и его потомков создаем пункты меню и
            // добавить в объект SortedList.
            // Обратите внимание на пункт меню. Свойство Tag является связанным объектом Type.
            foreach (Type typ in atype)
            {
                if (typ.IsPublic && (typ.IsSubclassOf(typeof(Control))))
                {
                    MenuItem item = new MenuItem();
                    item.Header = typ.Name;
                    item.Tag = typ;
                    sortlst.Add(typ.Name, item);
                }
            }

            // Пройдемся по отсортированному списку и установим родителей пункта меню.
            foreach (KeyValuePair<string, MenuItem> kvp in sortlst)
            {
                if (kvp.Key != "Control")
                {
                    string strParent = ((Type)kvp.Value.Tag).BaseType.Name;
                    MenuItem itemParent = sortlst[strParent];
                    itemParent.Items.Add(kvp.Value);
                }
            }

            // Сканирование еще раз:
            // Если абстрактный и выбираемый, отключить.
            // Если не абстрактный и не выбираемый, добавить новый элемент.
            foreach (KeyValuePair<string, MenuItem> kvp in sortlst)
            {
                Type typ = (Type)kvp.Value.Tag;

                if (typ.IsAbstract && kvp.Value.Items.Count == 0)
                    kvp.Value.IsEnabled = false;

                if (!typ.IsAbstract && kvp.Value.Items.Count > 0)
                {
                    MenuItem item = new MenuItem();
                    item.Header = kvp.Value.Header as string;
                    item.Tag = typ;
                    kvp.Value.Items.Insert(0, item);
                }
            }
        }
    }
}
