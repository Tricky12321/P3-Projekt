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
            this.ResizeMode = ResizeMode.NoResize;

            tempProducts = _storageController.TempProductList.Where(x => x.Value.Resolved == false).ToList();
            button_Merge.IsEnabled = false;
            listview_ProductsToMerge.SelectionMode = SelectionMode.Single;
            foreach (var tempProductsToListView in tempProducts)
            {
                ItemList.Add(new TempListItem { Description = tempProductsToListView.Value.Description, Price = tempProductsToListView.Value.SalePrice });
            }
            listview_ProductsToMerge.ItemsSource = ItemList;
            Show();
            Activate();
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
            if (int.TryParse(textBox_IDToMerge.Text, out validInput))
            {
                try
                {
                    var productToMerge = _storageController.ProductDictionary[int.Parse(textBox_IDToMerge.Text)];
                    Label_MergeInfo.Content = productToMerge.Name;
                    button_Merge.IsEnabled = true;
                    return productToMerge;
                }
                catch (KeyNotFoundException)
                {
                    Label_MergeInfo.Content = "Ugyldigt Produkt ID";
                    button_Merge.IsEnabled = false;
                    button_Merge.IsEnabled = false;
                }
            }
            else
            {
                Label_MergeInfo.Content = "Forkert Input";
                button_Merge.IsEnabled = false;
            }
            return null;
        }

        private void textBox_IDToMerge_KeyUp(object sender, KeyEventArgs e)
        {
            IDToMerge();
        }

        private void button_Merge_Click(object sender, RoutedEventArgs e)
        {

            if (!(IDToMerge().StorageWithAmount.Where(x => x.Value == 0).Count() == IDToMerge().StorageWithAmount.Count))
            {
                _storageController.MergeTempProduct(tempProducts[index].Value, IDToMerge().ID);
                Close();
                MessageBox.Show($"Midlertidigt produkt: {tempProducts[index].Value.Description}\nEr rettet til at være et produktet: {IDToMerge().Name}");
            }
            else
            {
                Label_MergeInfo.Content = "Produktet ikke på lager,\nkan ikke rette midlertidigt produkt!";

            }
        }

        private void listview_ProductsToMerge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index = listview_ProductsToMerge.SelectedIndex;
            if (index <= tempProducts.Count() && index >= 0)
            {
                textBox_TempProductInfo.Text = tempProducts[index].Value.Description;
            }
        }
    }



}
