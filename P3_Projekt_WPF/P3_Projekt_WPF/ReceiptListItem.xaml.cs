using P3_Projekt_WPF.Classes.Utilities;
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

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for ReceiptListItem.xaml
    /// </summary>
    public partial class ReceiptListItem : UserControl
    {
        public string String_Product { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public string IDTag { get; set; }
        public int TransID { get; set; }

        public event EventHandler Delete_Button_Event;
        public event EventHandler Increment_Button_Event;
        public event EventHandler Decrement_Button_Event;

        public ReceiptListItem(string product, decimal price, int amount, string idTag)
        {
            InitializeComponent();
            String_Product = product;
            Price = price;
            Amount = amount;
            IDTag = idTag;

            text_productName.Text = String_Product;
            text_price.Text = Price.ToString().Replace('.', ',');
            text_amount.Text = Amount.ToString();

            image_Delete.Source = Utils.ImageSourceForBitmap(Properties.Resources.DeleteIcon);
            image_Delete_Discount.Source = Utils.ImageSourceForBitmap(Properties.Resources.DeleteIcon);
        }


        public ReceiptListItem(string product, decimal price, int amount, string idTag, int idTransaction) : this(product, price, amount, idTag)
        {
            TransID = idTransaction;
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Delete_Button_Event?.Invoke(this, null);
        }

        private void Increment_Button_Click(object sender, RoutedEventArgs e)
        {
            Increment_Button_Event?.Invoke(this, null);
        }

        private void Decrement_Button_Click(object sender, RoutedEventArgs e)
        {
            Decrement_Button_Event?.Invoke(this, null);
        }
    }
}
