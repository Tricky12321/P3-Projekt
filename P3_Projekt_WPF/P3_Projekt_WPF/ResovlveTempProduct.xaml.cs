using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for ResovleTempProduct.xaml
    /// </summary>
    public partial class ResovleTempProduct : Window
    {
        public ResovleTempProduct()
        {
            InitializeComponent();
        }

        private void TextInputNoNumber(object sender, TextCompositionEventArgs e)
        {
            // Only allows number in textfield
            if (e.Text.Length > 0)
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                    e.Handled = true;
            }

        }

        private void listview_ProductsToMerge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }



}
