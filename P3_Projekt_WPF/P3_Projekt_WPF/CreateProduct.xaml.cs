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
        public ConcurrentDictionary<int, int> StorageWithAmount = new ConcurrentDictionary<int, int>();
        private ConcurrentDictionary<int, StorageRoom> _storageRooms;
        private ConcurrentDictionary<int, StorageRoom> _groups;
        private StorageController _storageController;
        private bool UpdateProductSec = false;
        private int UpdateProductID = 0;
        private MainWindow MainWin = null;
        public CreateProduct(Product prod, StorageController storageController, MainWindow MainWin)
        {
            this.MainWin = MainWin;
            UpdateProductSec = true;
            InitializeComponent();
            _storageRooms = storageController.StorageRoomDictionary;
            _storageController = storageController;
            LoadStorageRooms(_storageRooms);
            UpdateProductID = prod.ID;
            comboBox_Group.ItemsSource = (storageController.GroupDictionary.Values.Select(x => x.Name));
            comboBox_ServiceGroup.ItemsSource = (storageController.GroupDictionary.Values.Select(x => x.Name));
            comboBox_Brand.ItemsSource = (storageController.GetProductBrands());
            if (prod.Image != null)
            {
                image_Product.Source = prod.Image.Source;
            }
            output_ProductID.Text = prod.ID.ToString();
            textbox_Name.Text = prod.Name;
            textbox_PurchasePrice.Text = prod.PurchasePrice.ToString();
            textbox_SalePrice.Text = prod.SalePrice.ToString();
            textbox_DiscountPrice.Text = prod.DiscountPrice.ToString();
            comboBox_Brand.Text = prod.Brand;
            comboBox_Group.Text = storageController.GroupDictionary[prod.ProductGroupID].Name;
            StorageWithAmount = prod.StorageWithAmount;
            ReloadAddedStorageRooms();
            btn_AddPicture.Click += PickImage;
            btn_ServiceAddPicture.Click += PickImage;
            ImageChosenEvent += (FilePath) =>
            {
                image_Product.Source = new BitmapImage(new Uri(FilePath));
                image_ServiceProduct.Source = new BitmapImage(new Uri(FilePath));
                ChosenFilePath = FilePath;
            };
            btn_AddStorageRoomWithAmount.Click += AddStorageWithAmount;
            btn_JustQuit.Click += delegate { this.Close(); };
            btn_ServiceJustQuit.Click += delegate { this.Close(); };
        }

        public CreateProduct(StorageController storageController, MainWindow MainWin)
        {
            this.MainWin = MainWin;
            InitializeComponent();
            comboBox_Group.ItemsSource = (storageController.GroupDictionary.Values.Select(x => x.Name));
            comboBox_ServiceGroup.ItemsSource = (storageController.GroupDictionary.Values.Select(x => x.Name));
            comboBox_Brand.ItemsSource = (storageController.GetProductBrands());
            _storageRooms = storageController.StorageRoomDictionary;

            LoadStorageRooms(_storageRooms);
            output_ProductID.Text = Product.GetNextID().ToString();

            btn_AddPicture.Click += PickImage;
            btn_ServiceAddPicture.Click += PickImage;
            ImageChosenEvent += (FilePath) =>
            {
                image_Product.Source = new BitmapImage(new Uri(FilePath));
                image_ServiceProduct.Source = new BitmapImage(new Uri(FilePath));
                ChosenFilePath = FilePath;
            };
            btn_AddStorageRoomWithAmount.Click += AddStorageWithAmount;
            btn_JustQuit.Click += delegate { this.Close(); };
            btn_ServiceJustQuit.Click += delegate { this.Close(); };
        }

        private void LoadStorageRooms(ConcurrentDictionary<int, StorageRoom> storageRooms)
        {
            foreach (KeyValuePair<int, StorageRoom> StorageRoom in storageRooms)
            {
                comboBox_StorageRoom.Items.Add($"{StorageRoom.Key.ToString()} {StorageRoom.Value.Name}");
                StorageWithAmount.TryAdd(StorageRoom.Key, 0);
            }
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

            foreach (KeyValuePair<int, StorageRoom> storageRoom in _storageRooms)
            {
                if (StorageWithAmount.ContainsKey(storageRoom.Key) && StorageWithAmount[storageRoom.Key] > 0)
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

        public bool IsProductInputValid()
        {
            if (textbox_DiscountPrice.Text == "")
                textbox_DiscountPrice.Text = "0";

            TextBox[] textboxes = new TextBox[] { textbox_Name, textbox_SalePrice, textbox_PurchasePrice };
            ComboBox[] comboboxes = new ComboBox[] { comboBox_Brand, comboBox_Group };
            bool ReturnVal = true;
            foreach (TextBox textbox in textboxes)
            {
                if (textbox.Text == "")
                {
                    textbox.BorderBrush = System.Windows.Media.Brushes.Red;
                    textbox.BorderThickness = new Thickness(2, 2, 2, 2);
                    ReturnVal = false;
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
                    ReturnVal = false;
                }
                else
                {
                    //combobox.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    combobox.BorderThickness = new Thickness(1, 1, 1, 1);
                }
            }
            if (!ReturnVal)
            {
                label_InputNotValid.Visibility = Visibility.Visible;

            }
            return ReturnVal;

        }

        public bool IsServiceProductInputValid()
        {
            TextBox[] textboxes = new TextBox[] { textbox_ServiceName, textbox_ServiceSalePrice, textbox_ServiceGroupLimit, textbox_ServiceGroupPrice };

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
            return returnVal;

        }

        private void btn_DeleteStorage_Click(object sender, RoutedEventArgs e)
        {
            int IDToRemove = Convert.ToInt32((sender as Button).Tag);
            StorageWithAmount[IDToRemove] = 0;
            ReloadAddedStorageRooms();
        }

        private void btn_SaveAndQuit_Click(object sender, RoutedEventArgs e)
        {
            if (IsProductInputValid())
            {

                // TODO: Give ability to make a service product
                if (UpdateProductSec)
                {
                    UpdateProduct(this);
                    AddProductImage(this, UpdateProductID);
                    MainWin.ReloadProducts();
                }
                else
                {
                    AddProduct(this);
                    AddProductImage(this, Product.GetNextID());
                    MainWin.ReloadProducts();

                }

                this.Close();
            }
        }

        private void AddProductImage(CreateProduct addProductWindow, int id)
        {
            if (addProductWindow.ChosenFilePath != null)
            {
                System.IO.File.Copy(addProductWindow.ChosenFilePath, Properties.Settings.Default.PictureFilePath + "\\" + id + ".jpg", true);
            }
        }

        private void UpdateProduct(CreateProduct addProductWindow)
        {
            _storageController.UpdateProduct(UpdateProductID,
                                             addProductWindow.textbox_Name.Text,
                                             addProductWindow.comboBox_Brand.Text,
                                             Decimal.Parse(addProductWindow.textbox_PurchasePrice.Text),
                                             _storageController.GroupDictionary.First(x => x.Value.Name == addProductWindow.comboBox_Group.Text).Key,
                                             (addProductWindow.textbox_DiscountPrice.Text != "0") ? true : false,
                                             Decimal.Parse(addProductWindow.textbox_DiscountPrice.Text),
                                             Decimal.Parse(addProductWindow.textbox_SalePrice.Text),
                                             addProductWindow.StorageWithAmount);
        }

        private void AddProduct(CreateProduct addProductWindow)
        {
            _storageController.CreateProduct(Product.GetNextID(),
                                             addProductWindow.textbox_Name.Text,
                                             addProductWindow.comboBox_Brand.Text,
                                             Decimal.Parse(addProductWindow.textbox_PurchasePrice.Text),
                                             _storageController.GroupDictionary.First(x => x.Value.Name == addProductWindow.comboBox_Group.Text).Key,
                                             (addProductWindow.textbox_DiscountPrice.Text != "0") ? true : false,
                                             Decimal.Parse(addProductWindow.textbox_DiscountPrice.Text),
                                             Decimal.Parse(addProductWindow.textbox_SalePrice.Text),
                                             addProductWindow.StorageWithAmount);
        }

        private void AddServiceProduct(CreateProduct addProductWindow)
        {
            _storageController.CreateServiceProduct(ServiceProduct.GetNextID(),
                                                    Decimal.Parse(addProductWindow.textbox_ServiceSalePrice.Text),
                                                    Decimal.Parse(addProductWindow.textbox_ServiceGroupPrice.Text),
                                                    Int32.Parse(addProductWindow.textbox_ServiceGroupLimit.Text),
                                                    addProductWindow.textbox_ServiceName.Text,
                                                    _storageController.GroupDictionary.First(X => X.Value.Name == addProductWindow.comboBox_ServiceGroup.Text).Key);
        }

        private void btn_ServiceSaveAndQuit_Click(object sender, RoutedEventArgs e)
        {
            if (IsServiceProductInputValid())
            {
                // TODO: Skal lige laves så det virker med service produkter.
                //AddProductImage(this);
                // TODO: Give ability to make a service product
                AddProduct(this);
                this.Close();
            }
        }
    }
}
