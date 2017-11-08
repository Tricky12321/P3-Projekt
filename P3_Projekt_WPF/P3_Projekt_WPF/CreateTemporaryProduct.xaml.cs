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
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for CreateTemporaryProduct.xaml
    /// </summary>
    public partial class CreateTemporaryProduct : Window
    {

        string description;
        decimal price;

        public CreateTemporaryProduct()
        {
            InitializeComponent();
        }

        private void btn_AddTempProduct_Click(object sender, RoutedEventArgs e)
        {
            price = decimal.Parse(textbox_Price.Text);
            description = textbox_Description.Text;

        }
    }
}
