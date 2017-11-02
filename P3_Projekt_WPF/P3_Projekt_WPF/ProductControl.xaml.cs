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
using System.Windows.Navigation;
using System.Windows.Shapes;
using P3_Projekt.Classes;
namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for ProductControl.xaml
    /// </summary>
    public partial class ProductControl : UserControl
    {
        private Product _displayProduct;
        public ProductControl(Product productForDisplay)
        {
            InitializeComponent();
            _displayProduct = productForDisplay;
            ShowProductInfo();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ShowProductInfo()
        {
            txtbox_ID.Text = _displayProduct.ID.ToString();
            txtbox_Navn.Text = _displayProduct.Name;
            txtbox_Price.Text = _displayProduct.SalePrice.ToString();
            txtbox_Stock.Text = _displayProduct.StorageWithAmount.Values.ToString();
        }
    }
}
