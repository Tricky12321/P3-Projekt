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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Concurrent;
using P3_Projekt_WPF.Classes;

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
        public Dictionary<int, int> StorageWithAmount = new Dictionary<int, int>();
        public Dictionary<int, StorageRoom> StorageRooms;

        public CreateProduct(Dictionary<int, StorageRoom> storageRooms)
        {
            InitializeComponent();
            StorageRooms = storageRooms;

            foreach(KeyValuePair<int, StorageRoom> storageRoom in StorageRooms)
            {
                comboBox_StorageRoom.Items.Add($"{storageRoom.Key.ToString()} {storageRoom.Value.Name}");
                StorageWithAmount.Add(storageRoom.Key, 0);
            }
            output_ProductID.Text = Product.GetNextID().ToString();

            btn_AddPicture.Click += PickImage;
            ImageChosenEvent += (FilePath) => { image_Product.Source = new BitmapImage(new Uri(FilePath));
                                                ChosenFilePath = FilePath; };
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
                }
            }
        }

        public void AddStorageWithAmount(object sender, RoutedEventArgs e)
        {
            if (comboBox_StorageRoom.Text != "")
            {
                int addedStorageRoomID = Int32.Parse(comboBox_StorageRoom.Text.Substring(0, comboBox_StorageRoom.Text.IndexOf(' ')));
                StorageWithAmount[addedStorageRoomID] += textbox_Amount.Text != "" ? Int32.Parse(textbox_Amount.Text) : 0;
            }
            ReloadAddedStorageRooms();
            
            textbox_Amount.Text = "";
        }

        private void ReloadAddedStorageRooms()
        {
            listview_AddedStorageRooms.Items.Clear();

            foreach (KeyValuePair<int, StorageRoom> storageRoom in StorageRooms)
            {
                if (StorageWithAmount[storageRoom.Key] > 0)
                {
                    listview_AddedStorageRooms.Items.Add(new Classes.Utilities.StorageListItem(storageRoom.Value.Name, StorageWithAmount[storageRoom.Key], storageRoom.Key));
                }
            }
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
                textbox_DiscountPrice.Text = "0";

            TextBox[] textboxes = new TextBox[] { textbox_Name, textbox_SalePrice, textbox_PurchasePrice};
            ComboBox[] comboboxes = new ComboBox[] { comboBox_Brand, comboBox_Group };

            if (textbox_Name.Text == "" || comboBox_Brand.Text == "" || comboBox_Group.Text == "" || textbox_SalePrice.Text == "" || textbox_PurchasePrice.Text == "")
            {
                foreach (TextBox textbox in textboxes)
                {
                    if (textbox.Text == "")
                    {
                        textbox.BorderBrush = System.Windows.Media.Brushes.Red;
                        textbox.BorderThickness = new Thickness(2, 2, 2, 2);
                    }
                    else
                    {
                        textbox.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                        textbox.BorderThickness = new Thickness(1, 1, 1, 1);
                    }
                }
                foreach (ComboBox combobox in comboboxes)
                {
                    if (combobox.Text == "")
                    {
                        combobox.BorderBrush = System.Windows.Media.Brushes.Red;
                        combobox.BorderThickness = new Thickness(2, 2, 2, 2);
                    }
                    else
                    {
                        combobox.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                        combobox.BorderThickness = new Thickness(1, 1, 1, 1);
                    }
                }
                label_InputNotValid.Visibility = Visibility.Visible;
                return false;
            }
            else
                return true;
            
        }

        private void btn_DeleteStorage_Click(object sender, RoutedEventArgs e)
        {
            int IDToRemove = Convert.ToInt32((sender as Button).Tag);
            StorageWithAmount[IDToRemove] = 0;
            ReloadAddedStorageRooms();
        }
    }
}
