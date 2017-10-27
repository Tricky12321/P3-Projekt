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

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class ReceiptItem
    {
        Button btn_down = new Button();
        public string string_Product { get; set; }

    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var gridView = new GridView();
            listView_Receipt.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                DisplayMemberBinding = new Binding("btn_down")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                DisplayMemberBinding = new Binding("btn_up")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                DisplayMemberBinding = new Binding("string_Product")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                DisplayMemberBinding = new Binding("price")
            });

            listView_Receipt.Items.Add(new ReceiptItem { string_Product = "Dette er en test" });
            listView_Receipt.Items.Add(new ReceiptItem { string_Product = "Dette er en test" });
            listView_Receipt.Items.Add(new ReceiptItem { string_Product = "Dette er en test" });
            listView_Receipt.Items.Add(new ReceiptItem { string_Product = "Dette er en test" });

            listView_Receipt.Items.Add(new ReceiptItem { string_Product = "Dette er en test" });

        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void but_MobilePay_Click(object sender, RoutedEventArgs e)
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
