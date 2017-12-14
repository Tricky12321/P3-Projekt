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
    /// Interaction logic for ResolveTempProduct.xaml
    /// </summary>
    public partial class ResolveTempProduct : Window
    {
        private StorageController _storageController;
        private List<TempListItem> _unresolvedItemList = new List<TempListItem>();
        private List<TempListItem> _resolvedItemList = new List<TempListItem>();
        private List<KeyValuePair<int, TempProduct>> _unresolvedTempProducts = new List<KeyValuePair<int, TempProduct>>();
        private List<KeyValuePair<int, TempProduct>> _resolvedTempProducts = new List<KeyValuePair<int, TempProduct>>();
        private int _index = 0;

        public ResolveTempProduct(StorageController storageController)
        {
            _storageController = storageController;
            InitializeComponent();
            InitWindow();
        }

        private void InitWindow()
        {
            this.ResizeMode = ResizeMode.NoResize;

            _unresolvedTempProducts = _storageController.TempProductDictionary.Where(x => x.Value.Resolved == false).ToList();
            _resolvedTempProducts = _storageController.TempProductDictionary.Where(x => x.Value.Resolved == true).ToList();
            button_Merge.IsEnabled = false;
            
            foreach (KeyValuePair<int,TempProduct> unresolvedTempProduct in _unresolvedTempProducts)
            {
                _unresolvedItemList.Add(new TempListItem { Description = unresolvedTempProduct.Value.Description, Price = unresolvedTempProduct.Value.SalePrice });
            }
            foreach(KeyValuePair<int,TempProduct> resolvedTempProduct in _resolvedTempProducts)
            {
                _resolvedItemList.Add(new TempListItem { Description = resolvedTempProduct.Value.Description, Price = resolvedTempProduct.Value.SalePrice });
            }
            listview_ProductsToMerge.ItemsSource = _unresolvedItemList;
            listview_resolvedProductsToMerge.ItemsSource = _resolvedItemList;
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
                    Label_MergeInfo.Text = productToMerge.Name;
                    button_Merge.IsEnabled = true;
                    return productToMerge;
                }
                catch (KeyNotFoundException)
                {
                    Label_MergeInfo.Text = "Ugyldigt Produkt ID";
                    button_Merge.IsEnabled = false;
                }
            }
            else
            {
                Label_MergeInfo.Text = "Forkert Input";
                button_Merge.IsEnabled = false;
            }
            return null;
        }

        private Product resolvedIDToMerge()
        {
            int validInput = 0;
            bool input = true;
            if (int.TryParse(textBox_resolvedIDToMerge.Text, out validInput))
            {
                try
                {
                    var productToMerge = _storageController.ProductDictionary[int.Parse(textBox_resolvedIDToMerge.Text)];
                    Label_resolvedMergeInfo.Text = productToMerge.Name;
                    button_resolvedMerge.IsEnabled = true;
                    return productToMerge;
                }
                catch (KeyNotFoundException)
                {
                    Label_resolvedMergeInfo.Text = "Ugyldigt Produkt ID";
                    button_resolvedMerge.IsEnabled = false;
                }
            }
            else
            {
                Label_resolvedMergeInfo.Text = "Forkert Input";
                button_resolvedMerge.IsEnabled = false;
            }
            return null;
        }

        private void textBox_IDToMerge_KeyUp(object sender, KeyEventArgs e)
        {
            Label_MergeInfo.Foreground = Brushes.Black;
            IDToMerge();
        }

        private void textBox_resolvedIDToMerge_KeyUp(object sender, KeyEventArgs e)
        {
            Label_resolvedMergeInfo.Foreground = Brushes.Black;
            resolvedIDToMerge();
        }

        private void button_Merge_Click(object sender, RoutedEventArgs e)
        {
            _storageController.MergeTempProduct(_unresolvedTempProducts[_index].Value, IDToMerge().ID);
            Close();
            MessageBox.Show($"Midlertidigt produkt: {_unresolvedTempProducts[_index].Value.Description}\nEr rettet til at være produktet: {IDToMerge().Name}");
        }

        private void button_resolvedMerge_Click(object sender, RoutedEventArgs e)
        {
            _storageController.RemergeTempProduct(_resolvedTempProducts[_index].Value, resolvedIDToMerge().ID);
            Close();
            MessageBox.Show($"Midlertidigt produkt: {_resolvedTempProducts[_index].Value.Description}\nEr rettet til at være produktet: {resolvedIDToMerge().Name}");
        }

        private void listview_ProductsToMerge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _index = listview_ProductsToMerge.SelectedIndex;
            if (_index <= _unresolvedTempProducts.Count() && _index >= 0)
            {
                textBox_TempProductInfo.Text = _unresolvedTempProducts[_index].Value.Description;
            }
        }

        private void listview_resolvedProductsToMerge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _index = listview_resolvedProductsToMerge.SelectedIndex;
            if (_index <= _resolvedTempProducts.Count() && _index >= 0)
            {
                textBox_resolvedTempProductInfo.Text = _resolvedTempProducts[_index].Value.Description;
                Label_resolvedPreviouslyResolvedproduct.Text = _storageController.ProductDictionary[_resolvedTempProducts[_index].Value.ResolvedProductID].GetName();
            }
        }
    }
}
