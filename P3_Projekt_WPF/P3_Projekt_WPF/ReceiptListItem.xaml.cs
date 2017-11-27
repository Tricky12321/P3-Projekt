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
        public int IDTag { get; set; }
        public int TransID { get; set; }

        public ReceiptListItem(string product, decimal price, int amount, int idTag)
        {
            InitializeComponent();
            String_Product = product;
            Price = price;
            Amount = amount;
            IDTag = idTag;
        }

        public ReceiptListItem(string product, decimal price, int amount, int idTag, int idTransaction) : this(product, price, amount, idTag)
        {
            TransID = idTransaction;
        }

    }
}
