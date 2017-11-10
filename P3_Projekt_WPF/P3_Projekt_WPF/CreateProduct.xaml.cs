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
                ImageChosenEvent(dialog.FileName);
                FileName = dialog.SafeFileName;
            }
        }

        public void AddStorageWithAmount(object sender, RoutedEventArgs e)
        {
            int addedStorageRoomID = Int32.Parse(comboBox_StorageRoom.Text.Substring(0, comboBox_StorageRoom.Text.IndexOf(' ')));
            // TODO: Validation of input to disallow non-numbers
            StorageWithAmount[addedStorageRoomID] = Int32.Parse(textbox_Amount.Text);
            listview_AddedStorageRooms.Items.Add($"{comboBox_StorageRoom.Text}: {textbox_Amount.Text}");

            textbox_Amount.Text = "";
        }
    }
}
