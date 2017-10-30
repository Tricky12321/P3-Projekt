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
        List<Button> quickButtonList = new List<Button>();

        public MainWindow()
        {
            InitializeComponent();
            InitGridQuickButtons();

            for (int i = 0; i < 10; ++i)
            {
                AddNewQuickButton($"{i} test", 123);
            }

            AddTransactionToReceipt(default(SaleTransaction));
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

        public void AddNewQuickButton(string buttonText, int productID)
        {
            Button button = new Button();
            button.Content = buttonText;
            button.Height = 50;
            button.Width = 250;
            button.SetValue(Grid.ColumnProperty, quickButtonList.Count % 2);
            button.SetValue(Grid.RowProperty, quickButtonList.Count / 2);
            button.Style = FindResource("Flat_Button") as Style;

            grid_QuickButton.Children.Add(button);
            quickButtonList.Add(button);

        }
        public void AddTransactionToReceipt(SaleTransaction transaction)
        {
            //TODO:
            listView_Receipt.Items.Add(new ReceiptListItem { String_Product = "test", Amount = 12, Price = "123,-" });

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
    }
}
