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

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for ResovleTempProduct.xaml
    /// </summary>
    public partial class ResovleTempProduct : Window
    {
        public ResovleTempProduct()
        {
            InitializeComponent();
        }

        private void listview_ProductsToMerge_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ok = this.listview_ProductsToMerge.SelectedValue;
            

        }
    }
}
