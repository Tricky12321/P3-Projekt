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
using P3_Projekt_WPF.Classes.Utilities;
using System.IO;
using System.Diagnostics;

namespace P3_Projekt_WPF
{
    public delegate void ImageChosen(string ImageName);
    public delegate void SaveAndQuitClick(int ID);
    /// <summary>
    /// Interaction logic for CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        
        public event ImageChosen ImageChosenEvent;
        public event SaveAndQuitClick SaveAndQuitEvent;
        public string ChosenFilePath;
        private ConcurrentDictionary<int, int> _storageWithAmount = new ConcurrentDictionary<int, int>();
        private ConcurrentDictionary<int, StorageRoom> _storageRooms;
        private ConcurrentDictionary<int, Group> _groups;
        private StorageController _storageController;
        private bool UpdateProductSec = false;
        private bool UpdateServiceProductSec = false;
        private int UpdateProductID = 0;
        private bool _isAdmin = false;
        public CreateProduct() { }
        private bool _deactivatedProd = false;
        //Creating new product
        public CreateProduct(StorageController storageController)
        {
            Initialize(storageController);
            output_ProductID.Text = Product.GetNextID().ToString();
            output_ServiceProductID.Text = ServiceProduct.GetNextID().ToString();
            btn_disableProduct.Visibility = Visibility.Hidden;
            btn_disableServiceProduct.Visibility = Visibility.Hidden;
            this.Title = "Opret Produkt";
        }

        //Editing normal product
        public CreateProduct(Product prod, StorageController storageController, bool adminEdit, bool deactivated = false)
        {
            Initialize(storageController);
            UpdateProductSec = true;
            FillBoxesWithExistingProduct(prod);
            UpdateProductID = prod.ID;
            if (prod.Image != null)
            {
                image_Product.Source = prod.Image.Source;
            }
            _isAdmin = adminEdit;
            tabControl.Items.RemoveAt(1);
            HideIfNotAdmin();

            ReloadAddedStorageRooms();

            this.Title = "Rediger Produkt";
            if (deactivated)
            {
                _deactivatedProd = true;
                btn_disableProduct.Click -= btn_disableProduct_Click;
                btn_disableProduct.Click += btn_EnableProduct_click;
                btn_disableProduct.Content = "Aktiver produkt";
            }

        }

        //Editing service product
        public CreateProduct(ServiceProduct prod, StorageController storageController, bool adminEdit, bool deactivated = false)
        {
            Initialize(storageController);
            UpdateServiceProductSec = true;
            output_ProductID.Text = Product.GetNextID().ToString();
            output_ServiceProductID.Text = prod.ID.ToString();
            UpdateProductID = prod.ID;
            FillBoxesWithExistingServiceProduct(prod);
            tabControl.SelectedIndex = 1;
            tabControl.Items.RemoveAt(0);

            if (prod.Image != null)
            {
                image_ServiceProduct.Source = prod.Image.Source;
            }
            _isAdmin = adminEdit;
            HideIfNotAdmin();

            this.Title = "Rediger Service Produkt";
            if (deactivated)
            {
                _deactivatedProd = true;
                btn_disableServiceProduct.Click -= btn_disableServiceProduct_Click;
                btn_disableServiceProduct.Click += btn_EnableServiceProduct_click;
                btn_disableServiceProduct.Content = "Aktiver produkt";
            }
        }


        private void Initialize(StorageController storageController)
        {

            InitializeComponent();

            _storageRooms = storageController.StorageRoomDictionary;
            _storageController = storageController;
            LoadStorageRooms(_storageRooms);


            comboBox_Group.ItemsSource = (storageController.GroupDictionary.Values.Select(x => x.Name));
            comboBox_ServiceGroup.ItemsSource = (storageController.GroupDictionary.Values.Select(x => x.Name));
            comboBox_Brand.ItemsSource = (storageController.GetProductBrands());

            btn_AddPicture.Click += PickImage;
            btn_ServiceAddPicture.Click += PickImage;
            string[] allowedImageExtensions = new string[] { ".jpg", ".bmp", ".png", ".jpeg", ".tiff", ".gif" };
            ImageChosenEvent += (FilePath) =>
            {
                if (allowedImageExtensions.Any(x => FilePath.ToLower().Contains(x)))
                {
                    image_Product.Source = new BitmapImage(new Uri(FilePath));
                    image_ServiceProduct.Source = new BitmapImage(new Uri(FilePath));
                    ChosenFilePath = FilePath;
                }
                else
                    MessageBox.Show("Ugyldig filformat!");
                
            };
            btn_AddStorageRoomWithAmount.Click += AddStorageWithAmount;
            btn_JustQuit.Click += delegate { this.Close(); };
            btn_ServiceJustQuit.Click += delegate { this.Close(); };
        }

        private void HideIfNotAdmin()
        {
            if (!_isAdmin)
            {
                border_NotAdmin.Visibility = Visibility.Visible;
                border_ServiceNotAdmin.Visibility = Visibility.Visible;

                textbox_SalePrice.IsEnabled = false;
                textbox_PurchasePrice.IsEnabled = false;
                textbox_DiscountPrice.IsEnabled = false;

                textbox_ServiceSalePrice.IsEnabled = false;
                textbox_ServiceGroupLimit.IsEnabled = false;
                textbox_ServiceGroupPrice.IsEnabled = false;
            }
        }

        private void FillBoxesWithExistingProduct(Product prod)
        {
            output_ProductID.Text = prod.ID.ToString();
            textbox_Name.Text = prod.Name;
            textbox_PurchasePrice.Text = prod.PurchasePrice.ToString();
            textbox_SalePrice.Text = prod.SalePrice.ToString();
            textbox_DiscountPrice.Text = prod.DiscountPrice.ToString();
            comboBox_Brand.Text = prod.Brand;
            comboBox_Group.Text = _storageController.GroupDictionary[prod.ProductGroupID].Name;
            _storageWithAmount = prod.StorageWithAmount;
        }

        private void FillBoxesWithExistingServiceProduct(ServiceProduct prod)
        {
            textbox_ServiceName.Text = prod.Name;
            comboBox_ServiceGroup.Text = _storageController.GroupDictionary[prod.ServiceProductGroupID].Name;
            textbox_ServiceSalePrice.Text = prod.SalePrice.ToString();
            textbox_ServiceGroupLimit.Text = prod.GroupLimit.ToString();
            textbox_ServiceGroupPrice.Text = prod.GroupPrice.ToString();
            if (prod.Image != null)
            {
                image_ServiceProduct.Source = prod.Image.Source;
            }
        }

        private void LoadStorageRooms(ConcurrentDictionary<int, StorageRoom> storageRooms)
        {
            foreach (KeyValuePair<int, StorageRoom> StorageRoom in storageRooms.Where(x => x.Value.ID != 0))
            {
                comboBox_StorageRoom.Items.Add($"{StorageRoom.Key.ToString()} {StorageRoom.Value.Name}");
                _storageWithAmount.TryAdd(StorageRoom.Key, 0);
            }
        }

        private void PickImage(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (dialog.FileName != null && dialog.FileName.Length > 0)
                {
                    ImageChosenEvent(dialog.FileName);
                }
            }
        }

        private void AddStorageWithAmount(object sender, RoutedEventArgs e)
        {
            if (comboBox_StorageRoom.Text.Length > 0)
            {
                int addedStorageRoomID = Int32.Parse(comboBox_StorageRoom.Text.Substring(0, comboBox_StorageRoom.Text.IndexOf(' ')));
                if (textbox_Amount.Text.Length > 0)
                {
                    if (!_storageWithAmount.ContainsKey(addedStorageRoomID))
                    {
                        _storageWithAmount.TryAdd(addedStorageRoomID, Convert.ToInt32(textbox_Amount.Text));
                    }
                    else
                    {
                        _storageWithAmount[addedStorageRoomID] += Convert.ToInt32(textbox_Amount.Text);
                    }
                }
            }
            ReloadAddedStorageRooms();
            textbox_Amount.Text = "";
        }

        private void ReloadAddedStorageRooms()
        {
            listview_AddedStorageRooms.Items.Clear();

            foreach (KeyValuePair<int, StorageRoom> storageRoom in _storageRooms)
            {
                if (_storageWithAmount.ContainsKey(storageRoom.Key) && _storageWithAmount[storageRoom.Key] != 0)
                {
                    listview_AddedStorageRooms.Items.Add(new Classes.Utilities.StorageListItem(storageRoom.Value.Name, _storageWithAmount[storageRoom.Key], storageRoom.Key));
                }
            }
        }

        private void AmountInputOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            // Only allows numbers in textfield except a single comma
            if (e.Text.Length > 0)
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                {
                    e.Handled = true;
                    //Handled = true means that the input will not be inputted
                }
            }
        }

        private void AmountInputOnlyNumbersButAllowOneComma(object sender, TextCompositionEventArgs e)
        {
            TextBox input = (sender as TextBox);
            //The input string has the format: An unlimited amount of numbers, then 0-1 commas, then 0-2 numbers
            var re = new System.Text.RegularExpressions.Regex(@"^((\d+)(,{0,1})(\d{0,2}))$");

            e.Handled = !re.IsMatch(input.Text.Insert(input.CaretIndex, e.Text));
        }

        private bool IsProductInputValid()
        {
            if (textbox_DiscountPrice.Text == "")
                textbox_DiscountPrice.Text = "0,00";

            TextBox[] textboxes = new TextBox[] { textbox_Name, textbox_SalePrice, textbox_PurchasePrice };
            ComboBox[] comboboxes = new ComboBox[] { comboBox_Brand, comboBox_Group };
            bool returnVal = true;
            foreach (TextBox textbox in textboxes)
            {
                if (textbox.Text == "")
                {
                    textbox.BorderBrush = System.Windows.Media.Brushes.Red;
                    textbox.BorderThickness = new Thickness(2, 2, 2, 2);
                    returnVal = false;
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
                    //combobox.BorderBrush = System.Windows.Media.Brushes.Red;
                    combobox.BorderThickness = new Thickness(2, 2, 2, 2);
                    returnVal = false;
                }
                else
                {
                    //combobox.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    combobox.BorderThickness = new Thickness(1, 1, 1, 1);
                }
            }
            if (!returnVal)
            {
                label_InputNotValid.Visibility = Visibility.Visible;
            }
            return returnVal;
        }

        private bool IsServiceProductInputValid()
        {
            TextBox[] textboxes = new TextBox[] { textbox_ServiceName, textbox_ServiceSalePrice };

            if (textbox_ServiceGroupLimit.Text == "")
                textbox_ServiceGroupLimit.Text = "0";

            if (textbox_ServiceGroupPrice.Text == "")
                textbox_ServiceGroupPrice.Text = textbox_ServiceSalePrice.Text;

            bool returnVal = true;
            foreach (TextBox textbox in textboxes)
            {
                if (textbox.Text == "")
                {
                    textbox.BorderBrush = System.Windows.Media.Brushes.Red;
                    textbox.BorderThickness = new Thickness(2, 2, 2, 2);
                    returnVal = false;
                }
                else
                {
                    textbox.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    textbox.BorderThickness = new Thickness(1, 1, 1, 1);
                }
            }

            if (comboBox_ServiceGroup.Text == "")
            {
                comboBox_ServiceGroup.BorderThickness = new Thickness(2, 2, 2, 2);
                returnVal = false;
            }
            else
            {
                comboBox_ServiceGroup.BorderThickness = new Thickness(1, 1, 1, 1);
            }

            if (!returnVal)
            {
                label_ServiceInputNotValid.Visibility = Visibility.Visible;
            }
            return returnVal;

        }

        private void btn_DeleteStorage_Click(object sender, RoutedEventArgs e)
        {
            int IDToRemove = Convert.ToInt32((sender as Button).Tag);
            _storageWithAmount[IDToRemove] = 0;
            ReloadAddedStorageRooms();
        }


        private void btn_SaveAndQuit_Click(object sender, RoutedEventArgs e)
        {
            if (IsProductInputValid())
            {
                if (UpdateProductSec)
                {
                    UpdateProduct();
                    AddProductImage(this, UpdateProductID);
                    SaveAndQuitEvent?.Invoke(UpdateProductID);
                }
                else
                {
                    AddProductImage(this, Product.GetNextID());
                    AddProduct();
                }
                this.Close();
            }
        }

        private void AddProductImage(CreateProduct addProductWindow, int id)
        {
            /*
                        DirectoryInfo directory = new DirectoryInfo($@"{ Properties.Settings.Default.PictureFilePath }");
                        string[] allowedExtensions = new string[] { ".jpg", ".bmp", ".png", ".jpeg", ".tiff", ".gif" };

                        IEnumerable<FileInfo> imageFiles = from file in directory.EnumerateFiles("*", SearchOption.AllDirectories)
                                                           where allowedExtensions.Contains(file.Extension.ToLower())
                                                           select file; 
            */
            if (addProductWindow.ChosenFilePath != null)
            {
                Debug.Print(addProductWindow.ChosenFilePath);
                try
                {
                    File.Copy(addProductWindow.ChosenFilePath, Properties.Settings.Default.PictureFilePath + "\\" + id + ".jpg", true);
                }
                catch (IOException e)
                {

                }
            }
        }

        private void UpdateProduct()
        {
            if (_deactivatedProd)
            {
                _storageController.UpdateDeactivatedProduct(UpdateProductID,
                                             textbox_Name.Text,
                                             comboBox_Brand.Text,
                                             Decimal.Parse(textbox_PurchasePrice.Text),
                                             _storageController.GroupDictionary.First(x => x.Value.Name == comboBox_Group.Text).Key,
                                             (textbox_DiscountPrice.Text != "0") ? true : false,
                                             Decimal.Parse(textbox_DiscountPrice.Text),
                                             Decimal.Parse(textbox_SalePrice.Text),
                                             _storageWithAmount);
            }
            else
            {
                _storageController.UpdateProduct(UpdateProductID,
                                                             textbox_Name.Text,
                                                             comboBox_Brand.Text,
                                                             Decimal.Parse(textbox_PurchasePrice.Text),
                                                             _storageController.GroupDictionary.First(x => x.Value.Name == comboBox_Group.Text).Key,
                                                             (textbox_DiscountPrice.Text != "0") ? true : false,
                                                             Decimal.Parse(textbox_DiscountPrice.Text),
                                                             Decimal.Parse(textbox_SalePrice.Text),
                                                             _storageWithAmount);
            }
            
        }

        private void AddProduct()
        {
            _storageController.CreateProduct(Product.GetNextID(),
                                             textbox_Name.Text,
                                             comboBox_Brand.Text,
                                             Decimal.Parse(textbox_PurchasePrice.Text),
                                             _storageController.GroupDictionary.First(x => x.Value.Name == comboBox_Group.Text).Key,
                                             (textbox_DiscountPrice.Text != "0") ? true : false,
                                             Decimal.Parse(textbox_DiscountPrice.Text),
                                             Decimal.Parse(textbox_SalePrice.Text),
                                             _storageWithAmount);
        }

        private void AddServiceProduct()
        {
            _storageController.CreateServiceProduct(ServiceProduct.GetNextID(),
                                                    Decimal.Parse(textbox_ServiceSalePrice.Text),
                                                    Decimal.Parse(textbox_ServiceGroupPrice.Text),
                                                    Int32.Parse(textbox_ServiceGroupLimit.Text),
                                                    textbox_ServiceName.Text,
                                                    _storageController.GroupDictionary.First(X => X.Value.Name == comboBox_ServiceGroup.Text).Key);
        }

        private void UpdateServiceProduct()
        {
            if (_deactivatedProd)
            {
                _storageController.UpdateDeactivatedServiceProduct(UpdateProductID,
                                                                    Decimal.Parse(textbox_ServiceSalePrice.Text),
                                                                    Decimal.Parse(textbox_ServiceGroupPrice.Text),
                                                                    Int32.Parse(textbox_ServiceGroupLimit.Text),
                                                                    textbox_ServiceName.Text,
                                                                    _storageController.GroupDictionary.First(X => X.Value.Name == comboBox_ServiceGroup.Text).Key);

            }
            else
            {
            _storageController.UpdateServiceProduct(UpdateProductID,
                                                    Decimal.Parse(textbox_ServiceSalePrice.Text),
                                                    Decimal.Parse(textbox_ServiceGroupPrice.Text),
                                                    Int32.Parse(textbox_ServiceGroupLimit.Text),
                                                    textbox_ServiceName.Text,
                                                    _storageController.GroupDictionary.First(X => X.Value.Name == comboBox_ServiceGroup.Text).Key);
            }
        }

        private void btn_ServiceSaveAndQuit_Click(object sender, RoutedEventArgs e)
        {
            if (IsServiceProductInputValid())
            {

                if (UpdateServiceProductSec)
                {
                    UpdateServiceProduct();
                }
                else
                {
                    AddServiceProduct();
                }

                this.Close();
            }
        }

        private void btn_disableProduct_Click(object sender, RoutedEventArgs e)
        {
            
            if (!_isAdmin)
            {
                MessageBox.Show("Du skal være admin for at deaktiverer produkter");
            }
            else
            {
                if (IsProductInputValid())
                {
                    _deactivatedProd = true;
                    BaseProduct ProductToDisable;
                    _storageController.AllProductsDictionary.TryRemove(UpdateProductID, out ProductToDisable);
                    Product ProductToDisable_Product;
                    _storageController.ProductDictionary.TryRemove(UpdateProductID, out ProductToDisable_Product);
                    _storageController.DisabledProducts.TryAdd(ProductToDisable_Product.ID, ProductToDisable_Product);
                    ProductToDisable_Product.DeactivateProduct();
                    if (UpdateProductSec)
                    {
                        UpdateProduct();
                        AddProductImage(this, UpdateProductID);
                    }
                    else
                    {
                        AddProductImage(this, Product.GetNextID());
                    }
                    MessageBox.Show("Du har deaktiveret produkt " + ProductToDisable_Product.ToString());
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Du kan ikke deaktivere et produkt som ikke er udfyldt korrekt!");
                }
            }
        }

        private void btn_disableServiceProduct_Click(object sender, RoutedEventArgs e)
        {

            if (!_isAdmin)
            {
                MessageBox.Show("Du skal være admin for at deaktiverer produkter");
            }
            else
            {
                if (IsServiceProductInputValid())
                {
                    _deactivatedProd = true;
                    BaseProduct ProductToDisable;
                    _storageController.AllProductsDictionary.TryRemove(UpdateProductID, out ProductToDisable);
                    ServiceProduct ProductToDisable_Product;
                    _storageController.ServiceProductDictionary.TryRemove(UpdateProductID, out ProductToDisable_Product);
                    _storageController.DisabledServiceProducts.TryAdd(ProductToDisable_Product.ID, ProductToDisable_Product);
                    ProductToDisable_Product.DeactivateProduct();
                    if (UpdateProductSec)
                    {
                        UpdateProduct();
                        AddProductImage(this, UpdateProductID);
                    }
                    else
                    {
                        AddProductImage(this, Product.GetNextID());
                    }
                    MessageBox.Show("Du har deaktiveret produkt " + ProductToDisable_Product.ToString());
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Du kan ikke deaktivere et produkt som ikke er udfyldt korrekt!");
                }
            }
        }

        private void btn_EnableServiceProduct_click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Du skal være admin for at aktiverer produkter");
            }
            else
            {
                if (IsServiceProductInputValid())
                {
                    _deactivatedProd = true;
                    ServiceProduct Product_To_Activate;
                    _storageController.DisabledServiceProducts.TryRemove(UpdateProductID, out Product_To_Activate);
                    Product_To_Activate.ActivateProduct();
                    _storageController.ServiceProductDictionary.TryAdd(Product_To_Activate.ID, Product_To_Activate);
                    _storageController.AllProductsDictionary.TryAdd(Product_To_Activate.ID, Product_To_Activate);
                    if (UpdateProductSec)
                    {
                        UpdateProduct();
                        AddProductImage(this, UpdateProductID);
                    }
                    else
                    {
                        AddProductImage(this, Product.GetNextID());
                    }
                    MessageBox.Show("Du har aktiveret produkt " + Product_To_Activate.ToString());
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Du kan ikke aktivere et produkt som ikke er udfyldt korrekt!");
                }
            }
        }

        private void btn_EnableProduct_click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Du skal være admin for at aktiverer produkter");
            }
            else
            {
                if (IsProductInputValid())
                {
                    _deactivatedProd = true;
                    Product Product_To_Activate;
                    _storageController.DisabledProducts.TryRemove(UpdateProductID, out Product_To_Activate);
                    Product_To_Activate.ActivateProduct();
                    _storageController.ProductDictionary.TryAdd(Product_To_Activate.ID,Product_To_Activate);
                    _storageController.AllProductsDictionary.TryAdd(Product_To_Activate.ID,Product_To_Activate);
                    if (UpdateProductSec)
                    {
                        UpdateProduct();
                        AddProductImage(this, UpdateProductID);
                    }
                    else
                    {
                        AddProductImage(this, Product.GetNextID());
                    }
                    MessageBox.Show("Du har aktiveret produkt " + Product_To_Activate.ToString());
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Du kan ikke aktivere et produkt som ikke er udfyldt korrekt!");
                }
            }
        }
    }
}
