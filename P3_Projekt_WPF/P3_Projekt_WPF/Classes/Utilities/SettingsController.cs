using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using P3_Projekt_WPF;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
namespace P3_Projekt_WPF.Classes.Utilities
{
    class SettingsController
    {
        public List<FastButton> quickButtonList = new List<FastButton>();
        public Dictionary<Key, FastButton> quickButtonKeyList = new Dictionary<Key, FastButton>();

        public void AddNewQuickButton(string buttonText, int productID, double gridWidth, double gridHeight, RoutedEventHandler btn_FastButton_click)
        {
            FastButton button = new FastButton();
            button.ProductID = productID;
            button.Content = buttonText; ;
            button.Height = gridHeight / 7;
            button.Width = gridWidth / 2;

            button.FontSize = 35;
            button.Background = Brushes.LightGray;
            button.Click += btn_FastButton_click;

            button.SetValue(Grid.ColumnProperty, quickButtonList.Count % 2);
            button.SetValue(Grid.RowProperty, quickButtonList.Count / 2);

            quickButtonList.Add(button);
        }

        public void SpecifyPictureFilePath()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                Properties.Settings.Default.PictureFilePath = dialog.SelectedPath;
                Properties.Settings.Default.Save();


                
            }
        }

        public void SpecifyIcecreamID(int ID)
        {
            Properties.Settings.Default.IcecreamID = ID;
        }
    }
}
