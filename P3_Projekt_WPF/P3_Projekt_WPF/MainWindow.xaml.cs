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
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes;
using System.Diagnostics;
using System.Threading;
namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        SettingsController _settingsController;
        StorageController _storageController;
        POSController _POSController;
        Grid productGrid = new Grid();

        public MainWindow()
        {
            InitializeComponent();

            InitComponents();
        }

        private void InitComponents()
        {
            _settingsController = new SettingsController();
            _storageController = new StorageController();
            _POSController = new POSController(_storageController);
            InitGridQuickButtons();
            
            InitStorageGridProducts();
            AddProductButton();
            LoadProductGrid();
        }

        private void InitGridQuickButtons()
        {
            grid_QuickButton.ColumnDefinitions.Add(new ColumnDefinition());
            grid_QuickButton.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < 7; ++i)
            {
                grid_QuickButton.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void updateGridQuickButtons(object sender, RoutedEventArgs e)
        {
            UpdateGridQuickButtons();
        }

        private void UpdateGridQuickButtons()
        {
            foreach (FastButton button in _settingsController.quickButtonList)
            {
                if (!grid_QuickButton.Children.Contains(button)){
                    button.Style = FindResource("Flat_Button") as Style;

                    button.Click += btn_FastButton_click;
                    grid_QuickButton.Children.Add(button);
                }
            }
        }

        private void UpdateReceiptList()
        {
            listView_Receipt.Items.Clear();
            foreach (SaleTransaction transaction in _POSController.PlacerholderReceipt.Transactions)
            {
                AddTransactionToReceipt(transaction);
            }
        }

        public void InitStorageGridProducts()
        {
            productGrid.VerticalAlignment = VerticalAlignment.Top;
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });
            
            scroll_StorageProduct.Content = productGrid;
            _storageController.GetAll();
            while (!_storageController.ThreadDone())
            {
                Thread.Sleep(200);
            }

            productGrid.Children.Add(new ProductControl(_storageController.ProductDictionary[12]));


            foreach (Product produkter in _storageController.ProductDictionary.Values)
            {
                int i = 1;
                productGrid.Children.Add(new ProductControl(produkter));
                if(i % 5 == 0)
                {
                    productGrid.RowDefinitions.Add(new RowDefinition());
                }
                ++i;
            }
            


        }

        public void AddProductButton()
        {
            Button addProductButton = new Button();
            addProductButton.Content = "+";
            addProductButton.Height = 360;
            addProductButton.Width = 250;
            addProductButton.SetValue(Grid.RowProperty, 0);
            addProductButton.SetValue(Grid.ColumnProperty, 0);
            addProductButton.Style = FindResource("Flat_Button") as Style;
            addProductButton.Background = Brushes.Transparent;
            // tilføj produkt addProductButton.Click
            productGrid.Children.Add(addProductButton);
        }

        public void StorageTabClick(object sender, RoutedEventArgs e)
        {
            LoadProductGrid();
        }

        public void LoadProductGrid()
        {
            int i = 1;

            foreach (Product produkter in _storageController.ProductDictionary.Values)
            {
                if (i % 5 == 0)
                {
                    productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });
                }
                ProductControl productControl = new ProductControl(produkter);
                productControl.SetValue(Grid.ColumnProperty, i % 5);
                productControl.SetValue(Grid.RowProperty, i / 5);
                productGrid.Children.Add(productControl);

                i++;
            }
        }

        public void AddTransactionToReceipt(SaleTransaction transaction)
        {
            listView_Receipt.Items.Add(new ReceiptListItem { String_Product = transaction.GetProductName(), Amount = transaction.Amount, Price = $"{transaction.GetProductPrice()},-" });
        }

        private void btn_MobilePay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Increment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Decrement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_DeleteProduct_Click(object sender, RoutedEventArgs e)
        {

        }


        private void listView_Receipt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void btn_TempProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_AddProduct_Click(object sender, RoutedEventArgs e)
        {
            _POSController.AddSaleTransaction(_POSController.GetProductFromID(int.Parse(textBox_AddProductID.Text)), int.Parse(textBox_ProductAmount.Text));
            UpdateReceiptList();
        }

        private void btn_PlusToReciept_Click(object sender, RoutedEventArgs e)
        {
            int inputAmount = Int32.Parse(textBox_ProductAmount.Text);
            if (inputAmount < 99)
            {
                textBox_ProductAmount.Text = (++inputAmount).ToString();
            }
        }

        private void btn_MinusToReciept_Click(object sender, RoutedEventArgs e)
        {
            int inputAmount = Int32.Parse(textBox_ProductAmount.Text);

            if (inputAmount > 1)
            {
                textBox_ProductAmount.Text = (--inputAmount).ToString();
            }
        }

        private void btn_SettingFastBtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _settingsController.AddNewQuickButton(textBox_CreateQuickBtnName.Text, int.Parse(textBox_CreateQuickBtnID.Text), grid_QuickButton.Width, grid_QuickButton.Height);
        }

        private void btn_FastButton_click(object sender, RoutedEventArgs e)
        {
            _POSController.AddSaleTransaction(_POSController.GetProductFromID((sender as FastButton).ProductID), 1);
            UpdateReceiptList();
        }



        private void TextInputNoNumber(object sender, TextCompositionEventArgs e)
        {
            // Only allows number in textfield
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void TextInputNoNumberWithComma(object sender, TextCompositionEventArgs e)
        {
            // Only allows number in textfield, also with comma
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && !(e.Text[e.Text.Length - 1] == ','))
                e.Handled = true;
        }
    }
}
