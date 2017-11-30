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
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for CreateTemporaryProduct.xaml
    /// </summary>
    public partial class CreateTemporaryProduct : Window
    {
        private int amount = 1;
        private StorageController _storageController;
        private POSController _posController;
        private decimal price;
        private int _tempID;
        public EventHandler UpdateReceiptEventHandler;

        public CreateTemporaryProduct(StorageController storageController, POSController posController, int tempID)
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            _storageController = storageController;
            _posController = posController;
            _tempID = tempID;
        }

        private void btn_PlusToReciept_Click(object sender, RoutedEventArgs e)
        {
            ++amount;
            UpdateBox();
        }

        private void btn_MinusToReciept_Click(object sender, RoutedEventArgs e)
        {
            --amount;
            UpdateBox();
        }

        private void UpdateBox()
        {
            textBox_ProductAmount.Text = amount.ToString();
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

        private void btn_AddTempProduct_Click(object sender, RoutedEventArgs e)
        {

            if (decimal.TryParse(textbox_Price.Text, out price) && price > 0 && textbox_Description != null)
            {
                string description = textbox_Description.Text;
                price = decimal.Parse(textbox_Price.Text);
                int amount = int.Parse(textBox_ProductAmount.Text);
                TempProduct NewTemp = _storageController.CreateTempProduct(description, price, _tempID);
                _storageController.TempTempProductList.Add(NewTemp);
                _posController.AddSaleTransaction(NewTemp, amount);

                UpdateReceiptEventHandler?.Invoke(this, null);
                this.Close();
                ++_tempID;
            }
            else
            {
                textbox_Description.BorderBrush = Brushes.Red;
                textbox_Price.BorderBrush = Brushes.Red;
            }
        }
    }
}
