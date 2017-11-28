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
    /// Interaction logic for AddIcecream.xaml
    /// </summary>
    public partial class AddIcecream : Window
    {
        public AddIcecream()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(KeyboardHook);
            textbox_Price.Focus();
        }

        private void AmountInputOnlyNumbersButAllowOneComma(object sender, TextCompositionEventArgs e)
        {
            TextBox input = (sender as TextBox);
            //The input string has the format: An unlimited amount of numbers, then 0-1 commas, then 0-2 numbers
            var re = new System.Text.RegularExpressions.Regex(@"^((\d+)(,{0,1})(\d{0,2}))$");

            e.Handled = !re.IsMatch(input.Text.Insert(input.CaretIndex, e.Text));
        }

        private void btn_AddIcecream_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void KeyboardHook(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && textbox_Price.Text != "")
            {
                this.Close();
            }
        }
    }
}
