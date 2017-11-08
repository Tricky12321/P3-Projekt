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

namespace P3_Projekt_WPF
{
    public delegate void ImageChosen(String ImageName);
    /// <summary>
    /// Interaction logic for CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        public CreateProduct()
        {
            InitializeComponent();
            btn_AddPicture.Click += PickImage;
            ImageChosenEvent += ShowChosenImage;

        }

        public event ImageChosen ImageChosenEvent;

        public void PickImage(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                ImageChosenEvent(dialog.FileName);
            }
        }

        public void ShowChosenImage(string ImageName)
        {
            image_Product.Source = new BitmapImage(new Uri(ImageName));
        }
    }
}
