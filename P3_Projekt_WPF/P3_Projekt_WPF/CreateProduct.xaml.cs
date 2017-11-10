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
            int addedStorageRoomID = Int32.Parse(comboBox_StorageRoom.Text.Substring(0, comboBox_StorageRoom.Text.IndexOf(' ')));
            // TODO: Validation of input to disallow non-numbers
            StorageWithAmount[addedStorageRoomID] += textbox_Amount.Text != "" ? Int32.Parse(textbox_Amount.Text) : 0;
            foreach(KeyValuePair<int, string> storageRoom in StorageRooms)
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
    }
}
