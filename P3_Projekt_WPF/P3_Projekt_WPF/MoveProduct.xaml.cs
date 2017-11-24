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
using System.Collections.Concurrent;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for MoveProduct.xaml
    /// </summary>
    public partial class MoveProduct : Window
    {
        private StorageController _storageController;
        private POSController _posController;
        private int _amount = 1;

        public MoveProduct(StorageController storageController, POSController posController)
        {
            _storageController = storageController;
            _posController = posController;
            InitializeComponent();
            initLayout();

        }

        public void initLayout()
        {
            //comboBox_StorageRooms.IsEnabled = false;
            comboBox_Destination.ItemsSource = _storageController.StorageRoomDictionary.Where(x => x.Key > 0).Select(x => x.Value.Name);

        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            ProductSearch();
        }

        private void txtBox_SearchField_LostFocus(object sender, RoutedEventArgs e)
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FillItemDetails(sender);
        }

        private void txtBox_SearchField_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ProductSearch();
            }
        }


        private void ProductSearch()
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Visible;
            ConcurrentDictionary<int, SearchProduct> productSearchResults = _storageController.SearchForProduct(txtBox_SearchField.Text);
            listBox_SearchResultsSaleTab.Items.Clear();
            var searchResults = productSearchResults.Values.OrderByDescending(x => x.BrandMatch + x.GroupMatch + x.NameMatch);
            foreach (SearchProduct product in searchResults)
            {
                if (product.CurrentProduct is Product)
                {
                    var item = new ListBoxItem();
                    item.Tag = product.CurrentProduct.ID;
                    item.Content = new SaleSearchResultItemControl((product.CurrentProduct as Product).Image, $"{(product.CurrentProduct as Product).Name}\n{product.CurrentProduct.ID}");
                    listBox_SearchResultsSaleTab.Items.Add(item);
                    if (searchResults.Count() == 1)
                    {
                        FillItemDetails(item);
                    }
                }
            }
        }

        private void FillItemDetails(object sender)
        {
            comboBox_StorageRooms.Items.Clear();
            var product = _posController.GetProductFromID(int.Parse((sender as ListBoxItem).Tag.ToString())) as Product;
            label_ProduktID.Content = product.ID.ToString();
            label_produktProdukt.Content = product.Name.ToString();
            if (product.StorageWithAmount.Count > 0)
            {
                foreach (KeyValuePair<int, int> storagerooms in product.StorageWithAmount)
                {
                    comboBox_StorageRooms.Items.Add(_storageController.StorageRoomDictionary[storagerooms.Key].Name);
                    comboBox_StorageRooms.SelectedIndex = 0;
                    comboBox_StorageRooms.IsEnabled = true;
                    comboBox_Destination.IsEnabled = true;
                }
            }
            else
            {
                comboBox_StorageRooms.Items.Add("Produktet er ikke på lager");
                comboBox_StorageRooms.SelectedIndex = 0;
                comboBox_StorageRooms.IsEnabled = false;
                comboBox_Destination.IsEnabled = false;
            }
            listBox_SearchResultsSaleTab.Visibility = Visibility.Collapsed;
        }

        private void btn_PlusAmount_Click(object sender, RoutedEventArgs e)
        {
            ++_amount;
            textBox_ProductAmount.Text = _amount.ToString();
        }

        private void btn_MinusAmount_Click(object sender, RoutedEventArgs e)
        {
            if(_amount >= 0)
            {
                --_amount;
                textBox_ProductAmount.Text = _amount.ToString();
            }
        }

        private void button_MoveProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = _posController.GetProductFromID(int.Parse(label_ProduktID.Content.ToString())) as Product;
            int sourceRoom = _storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_StorageRooms.Text).Select(x => x.Key).First();
            int destinationRoom = _storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_Destination.Text).Select(x => x.Key).First();
            StorageTransaction storageTransaction = new StorageTransaction(product, int.Parse(textBox_ProductAmount.Text), sourceRoom, destinationRoom, _storageController.StorageRoomDictionary);
            storageTransaction.Execute();
            storageTransaction.UploadToDatabase();
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
    }
}
