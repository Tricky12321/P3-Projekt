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
using P3_Projekt_WPF.Classes;
using P3_Projekt_WPF.Classes.Utilities;
using System.Collections.Concurrent;
using P3_Projekt_WPF.Classes.Exceptions;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for ProductControl.xaml
    /// </summary>
    public partial class ProductControl : UserControl
    {
        private BaseProduct _displayProduct;

        public Image txtboxImage
        {
            get
            {
                if (_displayProduct is Product)
                {
                    return (_displayProduct as Product).Image;

                }
                else if(_displayProduct is ServiceProduct)
                {
                    return (_displayProduct as ServiceProduct).Image;
                }
                else
                {
                    throw new WrongProductTypeException("Fejl i indsættelse af billede");
                }
            }
            set
            {
                if (value != null)
                {
                    img_ProductImage.Source = value.Source;
                }
                else
                {
                    img_ProductImage.Source = Utils.NoImage;
                    img_ProductImage.VerticalAlignment = VerticalAlignment.Center;
                    img_ProductImage.HorizontalAlignment = HorizontalAlignment.Center;
                    img_ProductImage.Stretch = Stretch.Uniform;
                }
            }
        } 

        public ProductControl(BaseProduct productForDisplay, ConcurrentDictionary<int, Group> groupDict)
        {
            InitializeComponent();
            _displayProduct = productForDisplay;
            txtboxImage = txtboxImage;

            if (productForDisplay is ServiceProduct)
            {
                btn_ShowMoreInformation.Background = Brushes.LightBlue;  
            }

            ShowProductInfo(groupDict);
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public void ShowProductInfo(ConcurrentDictionary<int, Group> groupDict)
        {
            if (_displayProduct is Product)
            {
                txtbox_Product.Text = $"ID: {_displayProduct.ID.ToString()}\nNavn: { (_displayProduct as Product).Name}\nGruppe: {groupDict[(_displayProduct as Product).ProductGroupID].Name}\nPris { _displayProduct.SalePrice.ToString()}DKK";

            }
            else if (_displayProduct is ServiceProduct)
            {
                txtbox_Product.Text = $"ID: {_displayProduct.ID.ToString()}\nNavn: { (_displayProduct as ServiceProduct).Name}\nGruppe: {groupDict[(_displayProduct as ServiceProduct).ServiceProductGroupID].Name}\nPris { _displayProduct.SalePrice.ToString()}DKK";
            }
            else
            {
                throw new WrongProductTypeException("Fejl i forsøg på at vise info om produkt");
            }

        }
    }
}
