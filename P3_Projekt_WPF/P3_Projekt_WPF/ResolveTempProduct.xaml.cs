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
            button_Match.IsEnabled = false;
            
            foreach (KeyValuePair<int,TempProduct> unresolvedTempProduct in _unresolvedTempProducts)
            {
                _unresolvedItemList.Add(new TempListItem { Description = unresolvedTempProduct.Value.Description, Price = unresolvedTempProduct.Value.SalePrice });
            }
            foreach(KeyValuePair<int,TempProduct> resolvedTempProduct in _resolvedTempProducts)
            {
                _resolvedItemList.Add(new TempListItem { Description = resolvedTempProduct.Value.Description, Price = resolvedTempProduct.Value.SalePrice });
            }
            listview_ProductsToMatch.ItemsSource = _unresolvedItemList;
            listview_resolvedProductsToMatch.ItemsSource = _resolvedItemList;
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

        private Product IDToMatch()
        {
            int validInput = 0;
            bool input = true;
            if (int.TryParse(textBox_IDToMatch.Text, out validInput))
            {
                try
                {
                    var productToMatch = _storageController.ProductDictionary[int.Parse(textBox_IDToMatch.Text)];
                    Label_MatchInfo.Text = productToMatch.Name;
                    button_Match.IsEnabled = true;
                    return productToMatch;
                }
                catch (KeyNotFoundException)
                {
                    Label_MatchInfo.Text = "Ugyldigt Produkt ID";
                    button_Match.IsEnabled = false;
                }
            }
            else
            {
                Label_MatchInfo.Text = "Forkert Input";
                button_Match.IsEnabled = false;
            }
            return null;
        }

        private Product resolvedIDToMatch()
        {
            int validInput = 0;
            bool input = true;
            if (int.TryParse(textBox_resolvedIDToMatch.Text, out validInput))
            {
                try
                {
                    var productToMatch = _storageController.ProductDictionary[int.Parse(textBox_resolvedIDToMatch.Text)];
                    Label_resolvedMatchInfo.Text = productToMatch.Name;
                    button_resolvedMatch.IsEnabled = true;
                    return productToMatch;
                }
                catch (KeyNotFoundException)
                {
                    Label_resolvedMatchInfo.Text = "Ugyldigt Produkt ID";
                    button_resolvedMatch.IsEnabled = false;
                }
            }
            else
            {
                Label_resolvedMatchInfo.Text = "Forkert Input";
                button_resolvedMatch.IsEnabled = false;
            }
            return null;
        }

        private void textBox_IDToMatch_KeyUp(object sender, KeyEventArgs e)
        {
            Label_MatchInfo.Foreground = Brushes.Black;
            IDToMatch();
        }

        private void textBox_resolvedIDToMatch_KeyUp(object sender, KeyEventArgs e)
        {
            Label_resolvedMatchInfo.Foreground = Brushes.Black;
            resolvedIDToMatch();
        }

        private void button_Match_Click(object sender, RoutedEventArgs e)
        {
            _storageController.MatchTempProduct(_unresolvedTempProducts[_index].Value, IDToMatch().ID);
            Close();
            MessageBox.Show($"Midlertidigt produkt: {_unresolvedTempProducts[_index].Value.Description}\nEr rettet til at være produktet: {IDToMatch().Name}");
        }

        private void button_resolvedMatch_Click(object sender, RoutedEventArgs e)
        {
            _storageController.RematchTempProduct(_resolvedTempProducts[_index].Value, resolvedIDToMatch().ID);
            Close();
            MessageBox.Show($"Midlertidigt produkt: {_resolvedTempProducts[_index].Value.Description}\nEr rettet til at være produktet: {resolvedIDToMatch().Name}");
        }

        private void listview_ProductsToMatch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _index = listview_ProductsToMatch.SelectedIndex;
            if (_index <= _unresolvedTempProducts.Count() && _index >= 0)
            {
                textBox_TempProductInfo.Text = _unresolvedTempProducts[_index].Value.Description;
            }
        }

        private void listview_resolvedProductsToMatch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _index = listview_resolvedProductsToMatch.SelectedIndex;
            if (_index <= _resolvedTempProducts.Count() && _index >= 0)
            {
                textBox_resolvedTempProductInfo.Text = _resolvedTempProducts[_index].Value.Description;
                Label_resolvedPreviouslyResolvedproduct.Text = _storageController.ProductDictionary[_resolvedTempProducts[_index].Value.ResolvedProductID].GetName();
            }
        }
    }
}
