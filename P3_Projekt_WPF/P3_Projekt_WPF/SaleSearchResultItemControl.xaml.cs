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
    /// Interaction logic for SaleSearchResultItemControl.xaml
    /// </summary>
    public partial class SaleSearchResultItemControl : UserControl
    {
        public SaleSearchResultItemControl(Image image, string text)
        {

            InitializeComponent();
            label_SaleProductSearchItemText.Content = text;
            Image imagePlaceholder = new Image();
            if (image != null)
            {
                imagePlaceholder.Source = image.Source;
            }
            image_SaleProductSearchItemImage.Source = imagePlaceholder.Source;
        }
    }
}
