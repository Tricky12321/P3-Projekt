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
        private int productID = 0;
        private ConcurrentDictionary<int, SearchProduct> productSearchResults = new ConcurrentDictionary<int, SearchProduct>();
        private bool firstSearch = false;
        private int sourceRoom = 0;

        public MoveProduct(StorageController storageController, POSController posController)
        {
            _storageController = storageController;
            _posController = posController;
            InitializeComponent();
            InitWindow();
        }

        public void InitWindow()
        {
            button_MoveProduct.IsEnabled = false;
            comboBox_Destination.ItemsSource = _storageController.StorageRoomDictionary.Where(x => x.Key > 0).Select(x => x.Value.Name);

        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            ProductSearch();
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FillItemDetails(sender);
        }

        private void txtBox_SearchField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProductSearch();
            }
        }

        private void ProductSearch()
        {
            listBox_SearchMoveProduct.Visibility = Visibility.Visible;
            ConcurrentDictionary<int, SearchProduct> productSearchResults = _storageController.SearchForProduct(txtBox_SearchField.Text);
            listBox_SearchMoveProduct.Items.Clear();
            var searchResults = productSearchResults.Values.OrderByDescending(x => x.BrandMatch + x.GroupMatch + x.NameMatch);
            foreach (SearchProduct product in searchResults)
            {
                if (product.CurrentProduct is Product)
                {
                    var item = new ListBoxItem();
                    item.Tag = product.CurrentProduct.ID;
                    item.Content = new SaleSearchResultItemControl((product.CurrentProduct as Product).Image, $"{(product.CurrentProduct as Product).Name}\n{product.CurrentProduct.ID}");
                    listBox_SearchMoveProduct.Items.Add(item);
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
            var product =  _posController.GetProductFromID(int.Parse((sender as ListBoxItem).Tag.ToString())) as Product;
            productID = product.ID;
            label_ProduktID.Content = product.ID.ToString();
            label_produktProdukt.Content = product.Name.ToString();
            string storageroomName = "room";
            if (product.StorageWithAmount.Count > 0)
            {
                foreach (KeyValuePair<int, int> storagerooms in product.StorageWithAmount)
                {
                    storageroomName = _storageController.StorageRoomDictionary[storagerooms.Key].Name;
                    comboBox_StorageRooms.Items.Add($"{storageroomName}");
                }
                comboBox_StorageRooms.IsEnabled = true;
                comboBox_Destination.IsEnabled = true;
                comboBox_StorageRooms.SelectedIndex = 0;
                label_ActualAmountInStorage.Content = product.StorageWithAmount.Where(x => x.Key == _storageController.StorageRoomDictionary.Where(z => z.Value.Name == comboBox_StorageRooms.Text).Select(y => y.Value.ID).First()).Select(x => x.Value).First();
            }
            else
            {
                comboBox_StorageRooms.Items.Add("Produktet er ikke på lager");
                comboBox_StorageRooms.SelectedIndex = 0;
                comboBox_StorageRooms.IsEnabled = false;
                comboBox_Destination.IsEnabled = false;
            }
            listBox_SearchMoveProduct.Visibility = Visibility.Collapsed;
            firstSearch = true;
        }

        private void btn_PlusAmount_Click(object sender, RoutedEventArgs e)
        {
            ++_amount;
            textBox_ProductAmount.Text = _amount.ToString();
        }

        private void btn_MinusAmount_Click(object sender, RoutedEventArgs e)
        {
            if (_amount >= 0)
            {
                --_amount;
                textBox_ProductAmount.Text = _amount.ToString();
            }
        }

        private void button_MoveProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = _posController.GetProductFromID(int.Parse(label_ProduktID.Content.ToString())) as Product;
            int destinationRoom = _storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_Destination.Text).Select(x => x.Key).First();

            if (!product.StorageWithAmount.Keys.Contains(_storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_Destination.Text).Select(x => x.Key).First()))
            {
                product.StorageWithAmount.TryAdd(_storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_Destination.Text).Select(x => x.Key).First(), 0);
                product.UpdateInDatabase();
            }
            StorageTransaction storageTransaction = new StorageTransaction(product, int.Parse(textBox_ProductAmount.Text), sourceRoom, destinationRoom, _storageController.StorageRoomDictionary);
            storageTransaction.Execute();
            storageTransaction.UploadToDatabase();
            this.Close();
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {

                this.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listBox_SearchMoveProduct.Visibility = Visibility.Hidden;
        }

        /*private void comboBox_StorageRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayNumberInStorage();
        }*/

        private void DisplayNumberInStorage()
        {
            if (firstSearch)
            {
                sourceRoom = _storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_StorageRooms.Text).Select(x => x.Key).First();
                label_ActualAmountInStorage.Content = _storageController.ProductDictionary[productID].StorageWithAmount[sourceRoom].ToString();
            }
        }

        private void comboBox_StorageRooms_DropDownClosed(object sender, EventArgs e)
        {
            DisplayNumberInStorage();
        }
    }
}
