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
        SettingsController _settingsController = new SettingsController();

        Grid productGrid = new Grid();


        public MainWindow()
        {
            
            InitializeComponent();
            Mysql.Connect(); // Forbinder til databasen
            var gridView = new GridView();
            listView_Receipt.View = gridView;

            
            InitGridQuickButtons();
            _settingsController.AddNewQuickButton("Hej med dig", 123, grid_QuickButton.Width, grid_QuickButton.Height);
            UpdateGridQuickButtons();
            InitStorageGridProducts();
            AddProductButton();

            AddTransactionToReceipt(new SaleTransaction(new ServiceProduct(19m, 15m, 10, "kurt", default(Group)), 12, 102));
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

            productGrid.Children.Add(new ProductControl());
        }

        public void AddProductButton()
        {
            Button addProductButton = new Button();
            addProductButton.Content = "+";
            addProductButton.Height = 350;
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

        private void but_MobilePay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void but_Increment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void but_Decrement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void but_DeleteProduct_Click(object sender, RoutedEventArgs e)
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

        }

        private void btn_PlusToReciept_Click(object sender, RoutedEventArgs e)
        {
            int inputAmount = Int32.Parse(textBox.Text);
            if (inputAmount < 99)
            {
                textBox.Text = (++inputAmount).ToString();
            }
        }

        private void btn_MinusToReciept_Click(object sender, RoutedEventArgs e)
        {
            int inputAmount = Int32.Parse(textBox.Text);

            if (inputAmount > 1)
            {
                textBox.Text = (--inputAmount).ToString();
            }
        }
    }
}
