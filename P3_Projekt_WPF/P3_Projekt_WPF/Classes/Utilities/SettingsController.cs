using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace P3_Projekt_WPF.Classes.Utilities
{
    class FastButton : Button
    {
        public int ProductID;
    }

    class SettingsController
    {
        public List<FastButton> quickButtonList = new List<FastButton>();

        public void AddNewQuickButton(string buttonText, int productID, double gridWidth, double gridHeight)
        {

            FastButton button = new FastButton();
            button.ProductID = productID;
            button.Content = buttonText;;
            button.Height = gridHeight / 7;
            button.Width = gridWidth / 2;

            button.FontSize = 35;
            button.Background = Brushes.LightGray;

            button.SetValue(Grid.ColumnProperty, quickButtonList.Count % 2);
            button.SetValue(Grid.RowProperty, quickButtonList.Count / 2);

            quickButtonList.Add(button);
        }
    }
}
