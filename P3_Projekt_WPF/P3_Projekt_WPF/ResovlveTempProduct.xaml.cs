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
using P3_Projekt_WPF.Classes;
using P3_Projekt_WPF.Classes.Utilities;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for ResovleTempProduct.xaml
    /// </summary>
    public partial class ResovleTempProduct : Window
    {
        private StorageController _storageController;
        private List<TempListItem> ItemList = new List<TempListItem>();
        private List<KeyValuePair<int, TempProduct>> tempProducts = new List<KeyValuePair<int, TempProduct>>();
        private int index = 0;


        public ResovleTempProduct(StorageController storageController)
        {
            _storageController = storageController;
            InitializeComponent();
            InitWindow();
        }

        private void InitWindow()
        {
            tempProducts = _storageController.TempProductList.Where(x => x.Value.Resolved == false).ToList();

            foreach (var tempProductsToListView in tempProducts)
            {
                ItemList.Add(new TempListItem { Description = tempProductsToListView.Value.Description, Price = tempProductsToListView.Value.SalePrice });
            }
            this.listview_ProductsToMerge.ItemsSource = ItemList;
            this.Show();
            this.Activate();
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


        private Product IDToMerge()
        {
            int validInput = 0;
            bool input = true;
            if (int.TryParse(this.textBox_IDToMerge.Text, out validInput))
            {
                try
                {
                    var productToMerge = _storageController.ProductDictionary[int.Parse(this.textBox_IDToMerge.Text)];
                    this.Label_MergeInfo.Content = productToMerge.Name;
                    this.button_Merge.IsEnabled = true;
                    return productToMerge;
                }
                catch (KeyNotFoundException)
                {
                    this.Label_MergeInfo.Content = "Ugyldigt Produkt ID";
                    this.button_Merge.IsEnabled = false;
                }
            }
            else
            {
                this.Label_MergeInfo.Content = "Forkert Input";
                this.button_Merge.IsEnabled = false;
            }
            return null;
        }

        private void textBox_IDToMerge_KeyUp(object sender, KeyEventArgs e)
        {
            IDToMerge();
        }

        private void button_Merge_Click(object sender, RoutedEventArgs e)
        {
            _storageController.MergeTempProduct(tempProducts[index].Value, int.Parse(this.textBox_IDToMerge.Text));
            this.Close();
            MessageBox.Show($"Midlertidigt produkt: {tempProducts[index].Value.Description}\nEr rettet til at være et produktet: {this.textBox_IDToMerge.Text}");
        }
        
        private void listview_ProductsToMerge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index = this.listview_ProductsToMerge.SelectedIndex;
            if (index <= tempProducts.Count() && index >= 0)
            {
                this.textBox_TempProductInfo.Text = tempProducts[index].Value.Description;
            }
        }
    }



}
