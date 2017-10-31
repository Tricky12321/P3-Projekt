using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace P3_Projekt_WPF.Classes.Utilities
{
    class SettingsController
    {
        public List<Button> quickButtonList = new List<Button>();

        public void AddNewQuickButton(string buttonText, int productID, double gridWidth, double gridHeight)
        {
            Button button = new Button();
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
