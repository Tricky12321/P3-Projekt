using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using P3_Projekt_WPF;

namespace P3_Projekt_WPF.Classes.Utilities
{
    class FastButton : Button
    {
        public int ProductID { get; set; }
        public string Button_Name { get; set; }
    }

    class FastButtonListItem
    {
        public int ProductID { get; set; }
        public string Button_Name { get; set; }
    }

    class SettingsController
    {
        public List<FastButton> quickButtonList = new List<FastButton>();
        public List<FastButtonListItem> quickButtonSettingsViewList = new List<FastButtonListItem>();

        public void AddNewQuickButton(string buttonText, int productID, double gridWidth, double gridHeight, RoutedEventHandler btn_FastButton_click)
        {
            FastButton button = new FastButton();
            button.ProductID = productID;
            button.Button_Name = buttonText;
            button.Content = buttonText;
            button.Height = gridHeight / 7;
            button.Width = gridWidth / 2;

            button.FontSize = 35;
            button.Background = Brushes.LightGray;
            button.Click += btn_FastButton_click;

            button.SetValue(Grid.ColumnProperty, quickButtonList.Count % 2);
            button.SetValue(Grid.RowProperty, quickButtonList.Count / 2);

            FastButtonListItem buttonElement = new FastButtonListItem();
            buttonElement.ProductID = productID;
            buttonElement.Button_Name = buttonText;

            quickButtonSettingsViewList.Add(buttonElement);
            quickButtonList.Add(button);

        }
    }
}
