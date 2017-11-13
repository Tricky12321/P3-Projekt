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
using System.Drawing;
using System.Collections.Concurrent;

namespace P3_Projekt_WPF
{
    public delegate void ImageChosen(string ImageName);
    /// <summary>
    /// Interaction logic for CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        public event ImageChosen ImageChosenEvent;
        public string ChosenFilePath;
        public string FileName;
        public Dictionary<int, int> StorageWithAmount = new Dictionary<int, int>();
        public Dictionary<int, string> StorageRooms = new Dictionary<int, string>();

        public CreateProduct()
        {
            InitializeComponent();
            btn_AddPicture.Click += PickImage;
            ImageChosenEvent += (FilePath) => { image_Product.Source = new BitmapImage(new Uri(FilePath)); };
            ImageChosenEvent += (FilePath) => { ChosenFilePath = FilePath; };
            btn_AddStorageRoomWithAmount.Click += AddStorageWithAmount;
            btn_JustQuit.Click += delegate { this.Close(); };
        }

        public void PickImage(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (dialog.FileName != null && dialog.FileName != "")
                {
                    ImageChosenEvent(dialog.FileName);
                    FileName = dialog.SafeFileName;
                }
            }
        }

        public void AddStorageWithAmount(object sender, RoutedEventArgs e)
        {
            listview_AddedStorageRooms.Items.Clear();
            if (comboBox_StorageRoom.Text != "")
            {
                int addedStorageRoomID = Int32.Parse(comboBox_StorageRoom.Text.Substring(0, comboBox_StorageRoom.Text.IndexOf(' ')));
                StorageWithAmount[addedStorageRoomID] += textbox_Amount.Text != "" ? Int32.Parse(textbox_Amount.Text) : 0;
            }
            // TODO: Validation of input to disallow non-numbers
            foreach (KeyValuePair<int, string> storageRoom in StorageRooms)
            {
                if (StorageWithAmount[storageRoom.Key] > 0)
                {
                    listview_AddedStorageRooms.Items.Add($"{storageRoom.Value}: {StorageWithAmount[storageRoom.Key]}");
                }
            }
            textbox_Amount.Text = "";
        }

        private void AmountInputOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            // Only allows number in textfield
            if (e.Text.Length > 0)
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                    e.Handled = true;
            }
        }

        public bool IsInputValid()
        {
            if (textbox_DiscountPrice.Text == "")
            {
                textbox_DiscountPrice.Text = "0";
            }

            if (textbox_Name.Text == "" || comboBox_Brand.Text == "" || comboBox_Group.Text == "" || textbox_SalePrice.Text == "" || textbox_PurchasePrice.Text == "")
            {
                if (textbox_Name.Text == "")
                {
                    textbox_Name.BorderBrush = System.Windows.Media.Brushes.Red;
                    textbox_Name.BorderThickness = new Thickness(2, 2, 2, 2);
                }
                else
                {
                    textbox_Name.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    textbox_Name.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                if (comboBox_Brand.Text == "")
                {
                    comboBox_Brand.BorderBrush = System.Windows.Media.Brushes.Red;
                    comboBox_Brand.BorderThickness = new Thickness(2, 2, 2, 2);
                }
                else
                {
                    comboBox_Brand.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    comboBox_Brand.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                if (comboBox_Group.Text == "")
                {
                    comboBox_Group.BorderBrush = System.Windows.Media.Brushes.Red;
                    comboBox_Group.BorderThickness = new Thickness(2, 2, 2, 2);
                }
                else
                {
                    comboBox_Group.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    comboBox_Group.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                if (textbox_SalePrice.Text == "")
                {
                    textbox_SalePrice.BorderBrush = System.Windows.Media.Brushes.Red;
                    textbox_SalePrice.BorderThickness = new Thickness(2, 2, 2, 2);
                }
                else
                {
                    textbox_SalePrice.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    textbox_SalePrice.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                if (textbox_PurchasePrice.Text == "")
                {
                    textbox_PurchasePrice.BorderBrush = System.Windows.Media.Brushes.Red;
                    textbox_PurchasePrice.BorderThickness = new Thickness(2, 2, 2, 2);
                }
                else
                {
                    textbox_PurchasePrice.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    textbox_PurchasePrice.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                label_InputNotValid.Visibility = Visibility.Visible;
                return false;
            }
            else
                return true;
            
        }
    }
}
