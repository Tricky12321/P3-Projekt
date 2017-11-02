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
            _settingsController = new SettingsController();
            _storageController = new StorageController();
            _POSController = new POSController(_storageController);

            InitializeComponent();
            Mysql.Connect(); // Forbinder til databasen

            var gridView = new GridView();
            listView_Receipt.View = gridView;

            InitGridQuickButtons();

            //UpdateGridQuickButtons();
            InitStorageGridProducts();
            AddProductButton();




            // Kun til testing
            //_storageController.GetAllProductsFromDatabase();

            _settingsController.AddNewQuickButton("Hej med dig", 123, grid_QuickButton.Width, grid_QuickButton.Height);
            _settingsController.AddNewQuickButton("Hej med dig2", 123, grid_QuickButton.Width, grid_QuickButton.Height);

            AddTransactionToReceipt(new SaleTransaction(new ServiceProduct(19m, 15m, 10, "kurt", default(Group)), 12, 102));
            //UpdateGridQuickButtons();
            //UpdateReceiptList();
            //
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

        private void UpdateGridQuickButtons()
        {
            foreach (Button button in _settingsController.quickButtonList)
            {
                button.Style = FindResource("Flat_Button") as Style;
                grid_QuickButton.Children.Add(button);
            }
        }

        private void UpdateReceiptList()
        {
            foreach(SaleTransaction transaction in _POSController.PlacerholderReceipt.Transactions)
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

            productGrid.RowDefinitions.Add(new RowDefinition());


            scroll_StorageProduct.Content = productGrid;

            _storageController.GetAll();

            productGrid.Children.Add(new ProductControl(_storageController.ProductDictionary[12]));


            foreach (Product produkter in _storageController.ProductDictionary.Values)
            {
                int i = 1;
                productGrid.Children.Add(new ProductControl(_storageController.ProductDictionary[i]));
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
            // tilføj produkt addProductButton.Click
            productGrid.Children.Add(addProductButton);
        }

        public void AddTransactionToReceipt(SaleTransaction transaction)
        {
            listView_Receipt.Items.Add(new ReceiptListItem { String_Product = transaction.GetProductName(), Amount = transaction.Amount, Price = $"{transaction.GetProductPrice()},-" });

        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }



        public void Start()
        {
            StorageController StorageControl = new StorageController();
            POSController POSControl = new POSController(StorageControl);
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






        private void TextInputNoNumber(object sender, TextCompositionEventArgs e)
        {
            // xaml.cs code
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void TextInputNoNumberWithComma(object sender, TextCompositionEventArgs e)
        {
            // xaml.cs code
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && !(e.Text[e.Text.Length - 1] == ','))
                e.Handled = true;
        }
    }
}
