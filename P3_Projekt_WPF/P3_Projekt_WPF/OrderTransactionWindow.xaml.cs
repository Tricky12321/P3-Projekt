﻿using System;
using System.Collections.Concurrent;
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
    /// Interaction logic for OrderTransactionWindow.xaml
    /// </summary>
    public partial class OrderTransactionWindow : Window
    {
        Product product;
        private int parsevalue;
        private StorageController _storageController;
        private POSController _posController;
        private int _amount = 1;
        private string supplier;
        public OrderTransactionWindow(StorageController storageController, POSController posController)
        {
            InitializeComponent();
            _storageController = storageController;
            _posController = posController;
            this.ResizeMode = ResizeMode.NoResize;
            combobox_Supplier.ItemsSource = _storageController.GetSuppliers();
        }


        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            ProductSearch();
        }

        private void txtBox_SearchField_LostFocus(object sender, RoutedEventArgs e)
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Collapsed;
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


        public void ProductSearch()
        {
            Int32.TryParse(txtBox_SearchField.Text, out parsevalue);
            listBox_SearchResultsSaleTab.Visibility = Visibility.Visible;
            ConcurrentDictionary<int, SearchProduct> productSearchResults = _storageController.SearchForProduct(txtBox_SearchField.Text);
            listBox_SearchResultsSaleTab.Items.Clear();
            var searchResults = productSearchResults.Values.OrderByDescending(x => x.BrandMatch + x.GroupMatch + x.NameMatch);
            if (productSearchResults.Count > 0)
            {
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

            else if (_storageController.DisabledProducts.TryGetValue(parsevalue, out product))
            {
                if (_storageController.DisabledProducts[parsevalue].ID.ToString() == txtBox_SearchField.Text)
                {
                    MessageBox.Show(($"Produkt med ID: {txtBox_SearchField.Text} er ikke aktiveret!\nGå til indstillger -> produkter for at genaktivere dette produkt"), "Produkt ikke fundet", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Topmost = true;
                    Keyboard.ClearFocus();
                    txtBox_SearchField.Text = "";
                    label_ProduktID.Content = "";
                    label_ProduktProdukt.Content = "";
                    combobox_Supplier.Text = "";
                    product = null;
                }
            }


            else
            {
                MessageBox.Show(($"Produkt med ID: {txtBox_SearchField.Text} findes ikke!"), "Produkt ikke fundet", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Topmost = true;
                Keyboard.ClearFocus();
                txtBox_SearchField.Text = "";
                label_ProduktID.Content = "";
                label_ProduktProdukt.Content = "";
                combobox_Supplier.Text = "";
                product = null;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
 
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;

        }


        private void FillItemDetails(object sender)
        {
            comboBox_StorageRooms.Items.Clear();
            product = _posController.GetProductFromID(int.Parse((sender as ListBoxItem).Tag.ToString())) as Product;
            label_ProduktID.Content = product.ID.ToString();
            label_ProduktProdukt.Content = product.Name.ToString();
            if (product.Active)
            {
                foreach (KeyValuePair<int, StorageRoom> storagerooms in _storageController.StorageRoomDictionary.Where(x => x.Value.ID != 0))
                {
                    comboBox_StorageRooms.Items.Add(_storageController.StorageRoomDictionary[storagerooms.Key].Name);
                    comboBox_StorageRooms.SelectedIndex = 0;
                    button_OrderTransaction.Background = Brushes.White;
                    comboBox_StorageRooms.IsEnabled = true;
                    button_OrderTransaction.IsEnabled = true;
                }
            }
            else
            {
                comboBox_StorageRooms.Items.Add("Produktet er ikke Aktivt");
                comboBox_StorageRooms.SelectedIndex = 0;
                comboBox_StorageRooms.IsEnabled = false;
                button_OrderTransaction.IsEnabled = false;
                button_OrderTransaction.Background = Brushes.Red;
            }

            listBox_SearchResultsSaleTab.Visibility = Visibility.Collapsed;
        }

        private void btn_PlusAmount_Click(object sender, RoutedEventArgs e)
        {
            Int32.TryParse(textBox_ProductAmount.Text, out _amount);

            if (_amount < 9999)
            {
                textBox_ProductAmount.Text = (++_amount).ToString();
            }
        }

        private void btn_MinusAmount_Click(object sender, RoutedEventArgs e)
        {
            Int32.TryParse(textBox_ProductAmount.Text, out _amount);

            if (_amount > 1)
            {
                textBox_ProductAmount.Text = (--_amount).ToString();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;
        }

        private void button_OrderTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (product == null)
            {
                textblock_Search.Text = "Vælg et Produkt";
                textblock_Search.Foreground = Brushes.Red;
                txtBox_SearchField.BorderBrush = Brushes.Red;
            }
            else if (combobox_Supplier.Text != "")
            {
                int parsevalue = 0;
                Int32.TryParse(textBox_ProductAmount.Text, out parsevalue);
                if (parsevalue > 0)
                {
                    if (!(product as Product).StorageWithAmount.Keys.Contains(_storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_StorageRooms.Text).Select(x => x.Key).First()))
                    {
                        product.StorageWithAmount.TryAdd(_storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_StorageRooms.Text).Select(x => x.Key).First(), 0);
                        product.UploadToDatabase();
                    }
                    OrderTransaction orderTransaction = new OrderTransaction(product, parsevalue, combobox_Supplier.Text, _storageController.StorageRoomDictionary.Where(x => x.Value.Name == comboBox_StorageRooms.Text).Select(x => x.Key).First());
                    orderTransaction.Execute();
                    orderTransaction.UploadToDatabase();
                    this.Close();
                }
                else
                {
                    combobox_Supplier.BorderBrush = Brushes.DarkGray;
                    textBox_ProductAmount.BorderBrush = Brushes.Red;
                }

            }
            else
            {
                label_SupplierLayer.Visibility = Visibility.Visible;
                txtBox_SearchField.BorderBrush = Brushes.DarkGray;
                label_SupplierLayer.Text = "Vælg en distributør";
                label_SupplierLayer.Foreground = Brushes.Red;
                combobox_Supplier.BorderBrush = Brushes.Red;
            }
        }

        private void combobox_Supplier_LostFocus(object sender, RoutedEventArgs e)
        {
            if (combobox_Supplier.Text == "")
            {
                label_SupplierLayer.Visibility = Visibility.Visible;
            }
        }

        private void combobox_Supplier_KeyUp(object sender, KeyEventArgs e)
        {
            int length = combobox_Supplier.Text.Count();
            if (length == 0)
            {
                label_SupplierLayer.Visibility = Visibility.Visible;
            }
            else if (length >= 1)
            {
                label_SupplierLayer.Visibility = Visibility.Hidden;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_OrderTransaction_Click(sender, e);
            }
        }

        private void combobox_Supplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            label_SupplierLayer.Text = "";
        }
    }
}
