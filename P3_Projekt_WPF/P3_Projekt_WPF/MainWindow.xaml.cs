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
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.IO;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
//using System.Drawing;

namespace P3_Projekt_WPF
{
    public partial class MainWindow : Window
    {
        private SettingsController _settingsController;
        private StorageController _storageController;
        private POSController _POSController;
        private StatisticsController _statisticsController;
        private Grid productGrid = new Grid();
        private Dictionary<int, ProductControl> _productControlDictionary = new Dictionary<int, ProductControl>();
        private bool _ctrlDown = false;
        private bool _partlypaid = false;

        public MainWindow()
        {
            Mysql.CheckDatabaseConnection();
            Utils.GetIceCreamID();
            _storageController = new StorageController();
            _POSController = new POSController(_storageController);
            _settingsController = new SettingsController();
            _statisticsController = new StatisticsController(_storageController);
            InitializeComponent();
            InitComponents();
            CultureInfo ci = new CultureInfo("da-DK");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        public void UpdateProductNewAmount(Product product)
        {
            _storageController.ProductDictionary[product.ID] = product;
        }

        //TODO
        //Dispatcher?
        public void reloadProducts()
        {
            Dispatcher?.Invoke(ReloadProducts);
        }

        public void ReloadProducts()
        {
            _storageController.LoadAllProductsDictionary();
            LoadProductImages();
            LoadProductControlDictionary();
            LoadProductGrid(_storageController.AllProductsDictionary);
        }

        public void LowStorageMessage(Product prod)
        {
            int storageLeft = prod.StorageWithAmount.Values.Sum();
            MessageBox.Show("Lager antal er lavt for: " + prod.ToString() + ", der er kun " + storageLeft.ToString() + " tilbage");
        }

        //TODO
        //counter aldrig brugt
        public void SaveQuickButtons()
        {
            int[] QuickButtonsValues;
            int counter = 0;
            List<FastButton> QuickButtons = new List<FastButton>();

            foreach (var item in _settingsController.quickButtonList)
            {
                QuickButtons.Add(item);
            }

            var Settings = Properties.Settings.Default;
            int QuickButtonsCount = QuickButtons.Count();
            Settings.QuickButton1 = QuickButtonsCount >= 1 ? QuickButtons[0].ToString() : null;
            Settings.QuickButton2 = QuickButtonsCount >= 2 ? QuickButtons[1].ToString() : null;
            Settings.QuickButton3 = QuickButtonsCount >= 3 ? QuickButtons[2].ToString() : null;
            Settings.QuickButton4 = QuickButtonsCount >= 4 ? QuickButtons[3].ToString() : null;
            Settings.QuickButton5 = QuickButtonsCount >= 5 ? QuickButtons[4].ToString() : null;
            Settings.QuickButton6 = QuickButtonsCount >= 6 ? QuickButtons[5].ToString() : null;
            Settings.QuickButton7 = QuickButtonsCount >= 7 ? QuickButtons[6].ToString() : null;
            Settings.QuickButton8 = QuickButtonsCount >= 8 ? QuickButtons[7].ToString() : null;
            Settings.QuickButton9 = QuickButtonsCount >= 9 ? QuickButtons[8].ToString() : null;
            Settings.QuickButton10 = QuickButtonsCount >= 10 ? QuickButtons[9].ToString() : null;
            Settings.QuickButton11 = QuickButtonsCount >= 11 ? QuickButtons[10].ToString() : null;
            Settings.QuickButton12 = QuickButtonsCount >= 12 ? QuickButtons[11].ToString() : null;
            Settings.QuickButton13 = QuickButtonsCount >= 13 ? QuickButtons[12].ToString() : null;
            Settings.QuickButton14 = QuickButtonsCount >= 14 ? QuickButtons[13].ToString() : null;
            Settings.QuickButton15 = QuickButtonsCount >= 15 ? QuickButtons[14].ToString() : null;
            Settings.QuickButton16 = QuickButtonsCount >= 16 ? QuickButtons[15].ToString() : null;
            Settings.QuickButton17 = QuickButtonsCount >= 17 ? QuickButtons[16].ToString() : null;
            Settings.QuickButton18 = QuickButtonsCount >= 18 ? QuickButtons[17].ToString() : null;
            Settings.QuickButton19 = QuickButtonsCount >= 19 ? QuickButtons[18].ToString() : null;
            Settings.QuickButton20 = QuickButtonsCount >= 20 ? QuickButtons[19].ToString() : null;
            Settings.QuickButton21 = QuickButtonsCount >= 21 ? QuickButtons[20].ToString() : null;
            Settings.Save();
        }

        public void LoadQuickButtons()
        {
            foreach (FastButton button in _settingsController.quickButtonList)
            {
                button.Visibility = Visibility.Hidden;
            }
            _settingsController.quickButtonList.Clear();
            listView_QuickBtn.Items.Clear();
            var Settings = Properties.Settings.Default;
            AddQuickButton(Settings.QuickButton1);
            AddQuickButton(Settings.QuickButton2);
            AddQuickButton(Settings.QuickButton3);
            AddQuickButton(Settings.QuickButton4);
            AddQuickButton(Settings.QuickButton5);
            AddQuickButton(Settings.QuickButton6);
            AddQuickButton(Settings.QuickButton7);
            AddQuickButton(Settings.QuickButton8);
            AddQuickButton(Settings.QuickButton9);
            AddQuickButton(Settings.QuickButton10);
            AddQuickButton(Settings.QuickButton11);
            AddQuickButton(Settings.QuickButton12);
            AddQuickButton(Settings.QuickButton13);
            AddQuickButton(Settings.QuickButton14);
            AddQuickButton(Settings.QuickButton15);
            AddQuickButton(Settings.QuickButton16);
            AddQuickButton(Settings.QuickButton17);
            AddQuickButton(Settings.QuickButton18);
            AddQuickButton(Settings.QuickButton19);
            AddQuickButton(Settings.QuickButton20);
            AddQuickButton(Settings.QuickButton21);
            UpdateGridQuickButtons();
        }

        private void AddQuickButton(string NewButton)
        {
            var Settings = Properties.Settings.Default;
            if (NewButton != "" && NewButton != null)
            {
                string[] NewButtonInfo = NewButton.Split('|');
                int prodID = Convert.ToInt32(NewButtonInfo[0]);
                if (_storageController.AllProductsDictionary.ContainsKey(prodID))
                {
                    _settingsController.AddNewQuickButton(NewButtonInfo[1], prodID, grid_QuickButton.Width, grid_QuickButton.Height, btn_FastButton_click);
                    listView_QuickBtn.Items.Add(new FastButton() { Button_Name = NewButtonInfo[1], ProductID = prodID });
                    listView_QuickBtn.Items.Refresh();
                }
            }
        }

        private void InitComponents()
        {
            LoadDatabase();
            InitGridQuickButtons();
            InitStorageGridProducts();
            AddProductButton();
            ReloadProducts();
            LoadProductGrid(_storageController.AllProductsDictionary);
            InitStatisticsTab();
            InitAdminLogin();
            LoadDatabaseSettings();
            FillDeactivatedProductsIntoGrid();
            image_DeleteFullReceiptDiscount.Source = Utils.ImageSourceForBitmap(Properties.Resources.DeleteIcon);
            BuildInformationTable();
            LoadGroups();
            LoadQuickButtons();
            this.KeyDown += new KeyEventHandler(EnterKeyPressedSearch);
            SaleTransaction.UpdateProductEvent += UpdateProductNewAmount;
            _POSController.ReceiptExecutingThreadDone += reloadProducts;
            _POSController.LowStorageWarning += LowStorageMessage;

            _storageController.MakeSureIcecreamExists();
            if (!Mysql.ConnectionWorking)
            {
                tabControl.SelectedValue = tab_Settings;
                tab_WithSettings.SelectedValue = tab_admin;
            }
        }

        private void FlipRemoteLocal()
        {
            var settings = Properties.Settings.Default;
            settings.local_or_remote = !settings.local_or_remote;
            settings.Save();
            LoadDatabaseSettings();
        }

        private void LoadDatabaseSettings()
        {
            var settings = Properties.Settings.Default;
            bool Local = false;
            bool Remote = false;
            if (settings.local_or_remote == true)
            {
                btn_RmtLcl.Content = "Butikkens PC";
                Local = true;
            }
            else
            {
                btn_RmtLcl.Content = "Anden PC";
                Remote = true;
            }
            LoadDBSettingsData();
        }

        private void SaveDBData()
        {
            var settings = Properties.Settings.Default;
            settings.lcl_db = cmb_lcl_db.Text;
            settings.lcl_ip = cmb_lcl_ip.Text;
            settings.lcl_port = Convert.ToInt32(cmb_lcl_port.Text);
            settings.lcl_password = txt_lcl_password.Password;
            settings.lcl_username = txt_lcl_username.Text;
            settings.Save();
            Mysql.UpdateSettings(settings);
            Mysql.CheckDatabaseConnection();
        }

        private void LoadDBSettingsData()
        {
            var settings = Properties.Settings.Default;
            cmb_lcl_db.Text = settings.lcl_db;
            cmb_lcl_ip.Text = settings.lcl_ip;
            cmb_lcl_port.Text = settings.lcl_port.ToString();
            txt_lcl_password.Password = settings.lcl_password;
            txt_lcl_username.Text = settings.lcl_username;
        }

        //TODO
        //i++
        private void InitGridQuickButtons()
        {
            grid_QuickButton.ColumnDefinitions.Add(new ColumnDefinition());
            grid_QuickButton.ColumnDefinitions.Add(new ColumnDefinition());
            grid_QuickButton.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < 7; ++i)
            {
                grid_QuickButton.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void updateGridQuickButtons(object sender, RoutedEventArgs e)
        {
            UpdateGridQuickButtons();
        }

        private void InitStatisticsTab()
        {
            datePicker_StartDate.SelectedDate = DateTime.Now;
            datePicker_EndDate.SelectedDate = DateTime.Now;
            ChangeStatisticsState();
            IEnumerable products = _storageController.GetProductBrands();
            foreach (string brand in products)
            {
                comboBox_Brand.Items.Add(brand);
            }
            foreach (Group group in _storageController.GroupDictionary.Values)
            {
                comboBox_Group.Items.Add(group.Name);
            }
        }

        public void UpdateReceiptList()
        {
            listView_Receipt.Items.Clear();
            foreach (SaleTransaction transaction in _POSController.PlacerholderReceipt.Transactions)
            {
                AddTransactionToReceipt(transaction);
            }
            label_TotalPrice.Content = Math.Round(_POSController.PlacerholderReceipt.TotalPrice, 2).ToString().Replace('.', ',');

            if (_POSController.PlacerholderReceipt.Transactions.Count == 0)
            {
                btn_discount.IsEnabled = false;
            }
            else
            {
                btn_discount.IsEnabled = true;
            }

            if (_POSController.PlacerholderReceipt.DiscountOnFullReceipt == 0)
            {
                DisableDiscountOnReceipt();
            }
        }

        public void InitStorageGridProducts()
        {
            productGrid.VerticalAlignment = VerticalAlignment.Stretch;
            productGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            for (int i = 0; i < 5; i++)
            {
                productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });

            scroll_StorageProduct.Content = productGrid;
        }

        public void LoadDatabase()
        {
            if (Mysql.ConnectionWorking)
            {
                Thread GetAllThread = new Thread(new ThreadStart(_storageController.GetAll));
                GetAllThread.Name = "GetAllThread";
                GetAllThread.Start();
                while (!_storageController.ThreadsDone)
                {
                    Thread.Sleep(10);
                }
            }
        }

        private Button addProductButton = new Button();
        public void AddProductButton()
        {
            addProductButton.Content = "Tilføj nyt\nprodukt";
            addProductButton.FontSize = 30;
            addProductButton.SetValue(Grid.RowProperty, 0);
            addProductButton.SetValue(Grid.ColumnProperty, 0);
            addProductButton.Style = FindResource("Flat_Button") as Style;
            addProductButton.Margin = new System.Windows.Thickness(2);
            addProductButton.Background = System.Windows.Media.Brushes.Transparent;
            addProductButton.Click += AddProductDialogOpener;
        }

        public void AddProductDialogOpener(object sender, RoutedEventArgs e)
        {
            CreateProduct addProductWindow = new CreateProduct(_storageController);
            addProductWindow.Closed += delegate { ReloadProducts(); };
            addProductWindow.ShowDialog();
        }

        private bool _firstClick = true;
        private BaseProduct _productToEdit;
        private void EditProductClick(object sender, RoutedEventArgs e)
        {
            CreateProduct EditProductForm;
            if (_productToEdit is Product)
            {
                EditProductForm = new CreateProduct(_productToEdit as Product, _storageController, _settingsController.isAdmin);
            }
            else if (_productToEdit is ServiceProduct)
            {
                EditProductForm = new CreateProduct(_productToEdit as ServiceProduct, _storageController, _settingsController.isAdmin);
            }
            else
            {
                throw new WrongProductTypeException("Fejl i forsøg på at redigere produkt");
            }
            EditProductForm.Closed += delegate
            {
                ReloadProducts();
                LoadQuickButtons();
                ReloadDisabledProducts();
            };
            EditProductForm.ShowDialog();
        }

        //TODO
        //_selectedProductID bliver ikke brugt.
        private int _selectedProductID = -1;
        private void ShowSpecificInfoProductStorage(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag.ToString());
            _selectedProductID = id;
            ShowSpecificInfoProductStorage(id);
        }

        public void ShowSpecificInfoProductStorage(int id)
        {
            // Hvis det er første gang, skal EditProduct lige hookes op på edit product
            if (_firstClick)
            {
                _firstClick = false;
                btn_EditProduct.Visibility = Visibility.Visible;
            }
            _productToEdit = _storageController.AllProductsDictionary[id];
            image_ChosenProduct.Source = Utils.ImageSourceForBitmap(Properties.Resources.questionmark_png);

            if (_productToEdit is Product)
            {
                if ((_productToEdit as Product).Image != null)
                {
                    image_ChosenProduct.Source = (_productToEdit as Product).Image.Source;
                }

                textBlock_ChosenProduct.Text = $"ID: {_productToEdit.ID}\n" +
                                                $"{(_productToEdit as Product).Name}\n" +
                                                $"Gruppe: {_storageController.GroupDictionary[(_productToEdit as Product).ProductGroupID].Name}\n" +
                                                $"Mærke: {(_productToEdit as Product).Brand}\n" +
                                                $"Pris: {_productToEdit.SalePrice}\n" +
                                                $"Oprettet d. {(_productToEdit as Product).CreatedTime.ToShortDateString()}\n" +
                                                $"Tilbudspris: {(_productToEdit as Product).DiscountPrice}\n" +
                                                $"Indkøbspris: {(_productToEdit as Product).PurchasePrice}\n" +
                                                $"{((_productToEdit as Product).StorageWithAmount.Count == 0 ? "Produktet er ikke på lager" : "Lagerstatus:")}";
                foreach (KeyValuePair<int, int> storageWithAmount in (_productToEdit as Product).StorageWithAmount)
                {
                    textBlock_ChosenProduct.Text += $"\n  - {_storageController.StorageRoomDictionary[storageWithAmount.Key].Name} har {storageWithAmount.Value} stk.";
                }
            }
            else if (_productToEdit is ServiceProduct)
            {
                ServiceProduct product = (_productToEdit as ServiceProduct);
                if (product.Image != null)
                {
                    image_ChosenProduct.Source = (_productToEdit as ServiceProduct).Image.Source;
                }

                textBlock_ChosenProduct.Text = $"ID: {product.ID}\n" +
                                                $"Navn: {product.Name}\n" +
                                                $"Gruppe: {_storageController.GroupDictionary[product.ServiceProductGroupID].Name}\n" +
                                                $"Oprettet d. {(_productToEdit as ServiceProduct).CreatedTime.ToShortDateString()}\n" +
                                                $"{((product.SalePrice == product.GroupPrice) ? $"Pris: {product.SalePrice}\n" : $"Pris: {product.SalePrice}\nGruppepris: {product.GroupPrice}\nGruppe grænse: {product.GroupLimit}")}";
            }
            else
            {
                throw new WrongProductTypeException("Kunne ikke vise information for et produkt af denne type " + _productToEdit.GetType().ToString());
            }
        }

        public void UpdateStorageTab(object sender, RoutedEventArgs e)
        {
            LoadProductGrid(_storageController.AllProductsDictionary);
        }

        private void LoadProductControlDictionary()
        {
            _productControlDictionary.Clear();

            IOrderedEnumerable<KeyValuePair<int, BaseProduct>> ProductList = _storageController.AllProductsDictionary.OrderBy(x => x.Key);
            foreach (KeyValuePair<int, BaseProduct> product in ProductList)
            {
                ProductControl productControl = new ProductControl(product.Value, _storageController.GroupDictionary);
                productControl.btn_ShowMoreInformation.Tag = product.Value.ID;
                productControl.btn_ShowMoreInformation.Click += ShowSpecificInfoProductStorage;

                _productControlDictionary.Add(product.Value.ID, productControl);
            }
        }

        public void LoadProductGrid(ConcurrentDictionary<int, BaseProduct> productDictionary)
        {
            productGrid.RowDefinitions.Clear();
            productGrid.ColumnDefinitions.Clear();
            InitStorageGridProducts();
            productGrid.Children.Clear();
            productGrid.Children.Add(addProductButton);

            int i = 1;
            var ProductListSorted = _storageController.AllProductsDictionary.OrderBy(x => x.Key);
            foreach (KeyValuePair<int, BaseProduct> product in ProductListSorted)
            {
                if (i % 5 == 0)
                {
                    productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });
                }

                ProductControl productControl = _productControlDictionary[product.Value.ID];
                productControl.SetValue(Grid.ColumnProperty, i % 5);
                productControl.SetValue(Grid.RowProperty, i / 5);

                productGrid.Children.Add(productControl);
                i++;
            }
        }

        public void LoadProductGrid(ConcurrentDictionary<int, SearchProduct> productDictionary)
        {
            productGrid.RowDefinitions.Clear();
            productGrid.Children.Clear();
            productGrid.Children.Add(addProductButton);

            productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });
            int i = 1;
            var products = productDictionary.OrderByDescending(x => x.Value.BrandMatch + x.Value.GroupMatch + x.Value.NameMatch);
            foreach (KeyValuePair<int, SearchProduct> product in products)
            {
                if (i % 5 == 0)
                {
                    productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });
                }

                ProductControl productControl = _productControlDictionary[product.Value.CurrentProduct.ID];
                productControl.SetValue(Grid.ColumnProperty, i % 5);
                productControl.SetValue(Grid.RowProperty, i / 5);

                productGrid.Children.Add(productControl);
                i++;
            }
        }

        public void LoadProductImages()
        {
            if (Directory.Exists(Properties.Settings.Default.PictureFilePath))
            {
                DirectoryInfo directory = new DirectoryInfo($@"{ Properties.Settings.Default.PictureFilePath }");
                string[] allowedExtensions = new string[] { ".jpg", ".bmp", ".png", ".jpeg", ".tiff", ".gif" };

                IEnumerable<FileInfo> imageFiles = from file in directory.EnumerateFiles("*", SearchOption.AllDirectories)
                                                   where allowedExtensions.Contains(file.Extension.ToLower())
                                                   select file;
                try
                {
                    foreach (FileInfo productImage in imageFiles)
                    {
                        int productID;
                        int.TryParse((productImage.Name.Replace(productImage.Extension, "")), out productID);

                        BitmapImage bitMap = new BitmapImage(new Uri($"{productImage.DirectoryName}/{productImage}"));

                        Image image = new Image();
                        image.Source = bitMap;

                        _storageController.ProductDictionary[productID].Image = image;
                    }
                }
                catch (KeyNotFoundException e)
                {
                    //Missing image
                }
                catch (UnauthorizedAccessException e)
                {
                    //Can't access file
                }
            }
            LoadProductControlDictionary();
        }

        public void AddTransactionToReceipt(SaleTransaction transaction)
        {
            ReceiptListItem item;
            if (transaction.Product is TempProduct)
            {
                item = new ReceiptListItem(transaction.GetProductName(), Math.Round(transaction.TotalPrice, 2), transaction.Amount, 't' + transaction.Product.ID.ToString(), transaction.GetID());

                if (transaction.DiscountBool)
                {
                    item.canvas_Discount.Visibility = Visibility.Visible;
                    item.textBlock_DiscountAmount.Text = '-' + Math.Round((transaction.Price - transaction.DiscountPrice) * transaction.Amount, 2).ToString() + " : Ny Pris: " + Math.Round(transaction.DiscountPrice * transaction.Amount, 2).ToString();
                    item.textBlock_DiscountAmount.Foreground = Brushes.Green;
                }
                else
                {
                    item.canvas_Discount.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                item = new ReceiptListItem(transaction.GetProductName(), Math.Round(transaction.TotalPrice, 2), transaction.Amount, transaction.Product.ID.ToString(), transaction.GetID());
                if (transaction.DiscountBool)
                {
                    item.canvas_Discount.Visibility = Visibility.Visible;
                    item.textBlock_DiscountAmount.Text = '-' + Math.Round((transaction.Price - transaction.DiscountPrice) * transaction.Amount, 2).ToString() + " : Ny Pris: " + Math.Round(transaction.DiscountPrice * transaction.Amount, 2).ToString();
                    item.textBlock_DiscountAmount.Foreground = Brushes.Green;
                }
                else
                {
                    item.canvas_Discount.Visibility = Visibility.Collapsed;
                }
            }
            item.Delete_Button_Event += btn_DeleteProduct_Click;
            item.Increment_Button_Event += btn_Increment_Click;
            item.Decrement_Button_Event += btn_Decrement_Click;
            item.Delete_Receipt_Event += btn_DeleteReceiptDiscount_Click;

            listView_Receipt.Items.Add(item);
        }

        private void btn_Increment_Click(object sender, EventArgs e)
        {
            if (!_partlypaid)
            {
                _POSController.ChangeTransactionAmount(sender, e, 1);
                UpdateReceiptList();
            }
        }

        private void btn_Decrement_Click(object sender, EventArgs e)
        {
            if (!_partlypaid)
            {
                _POSController.ChangeTransactionAmount(sender, e, -1);
                UpdateReceiptList();
            }
        }

        private void btn_DeleteProduct_Click(object sender, EventArgs e)
        {
            if (!_partlypaid)
            {
                if ((sender as ReceiptListItem).IDTag.Contains("t"))
                {
                    string tempID = (sender as ReceiptListItem).IDTag;
                    _POSController.PlacerholderReceipt.RemoveTransaction(tempID);
                    _tempID--;
                }
                else
                {
                    int productID = Convert.ToInt32((sender as ReceiptListItem).IDTag);
                    _POSController.PlacerholderReceipt.RemoveTransaction(productID);
                }
                _POSController.PlacerholderReceipt.UpdateTotalPrice();
                UpdateReceiptList();
            }
        }

        private void btn_AddProduct_Click(object sender, RoutedEventArgs e)
        {
            int inputInt;
            int.TryParse(textBox_AddProductID.Text, out inputInt);
            BaseProduct ProductToAdd = _POSController.GetProductFromID(inputInt);
            if (ProductToAdd != null && ProductToAdd.GetName() != "Is")
            {
                if (_POSController.PlacerholderReceipt.Transactions.Where(x => x.Product.ID == ProductToAdd.ID).Count() > 0)
                {
                    _POSController.ChangeTransactionAmount(ProductToAdd.ID, int.Parse(textBox_ProductAmount.Text));
                }
                else
                {
                    _POSController.AddSaleTransaction(ProductToAdd, int.Parse(textBox_ProductAmount.Text));
                }
                UpdateReceiptList();

                textBox_AddProductID.Text = string.Empty;
                textBox_ProductAmount.Text = "1";
                textBox_AddProductID.Focus();
            }
            else
            {
                Utils.ShowErrorWarning($"Produkt med ID {inputInt} findes ikke på lageret");
            }
        }

        private void btn_PlusToReciept_Click(object sender, RoutedEventArgs e)
        {
            int inputAmount = Int32.Parse(textBox_ProductAmount.Text);
            if (inputAmount < 99)
            {
                textBox_ProductAmount.Text = (++inputAmount).ToString();
            }
        }

        private void btn_MinusToReciept_Click(object sender, RoutedEventArgs e)
        {
            int inputAmount = Int32.Parse(textBox_ProductAmount.Text);

            if (inputAmount > 1)
            {
                textBox_ProductAmount.Text = (--inputAmount).ToString();
            }
        }

        private void btn_SettingFastBtnAdd_Click(object sender, RoutedEventArgs e)
        {
            int inputInt;
            int.TryParse(textBox_CreateQuickBtnID.Text, out inputInt);
            int maxButtons = 21;

            if (_POSController.GetProductFromID(inputInt) != null && !_settingsController.quickButtonList.Any(x => x.ProductID == inputInt) && CheckIfTooManyQuickButtons(maxButtons))
            {
                _settingsController.AddNewQuickButton(textBox_CreateQuickBtnName.Text, inputInt, grid_QuickButton.Width, grid_QuickButton.Height, btn_FastButton_click);
                listView_QuickBtn.Items.Add(new FastButton() { Button_Name = textBox_CreateQuickBtnName.Text, ProductID = inputInt });
                listView_QuickBtn.Items.Refresh();
            }
            else if (_settingsController.quickButtonList.Any(x => x.ProductID == inputInt))
            {
                Utils.ShowErrorWarning($"Produkt med dette ID {inputInt} er allerede oprettet");
            }
            else if (!CheckIfTooManyQuickButtons(maxButtons))
            {
                Utils.ShowErrorWarning($"Der kan ikke oprettes flere hurtigknapper, der må højest være {maxButtons} hurtigknapper ");
            }
            else
            {
                Utils.ShowErrorWarning($"Produkt med ID {inputInt} findes ikke på lageret");
            }
            SaveQuickButtons();
        }

        private bool CheckIfTooManyQuickButtons(int maximumButtons)
        {
            return _settingsController.quickButtonList.Count < maximumButtons;
        }

        public void btn_FastButton_click(object sender, RoutedEventArgs e)
        {
            _POSController.AddSaleTransaction(_POSController.GetProductFromID((sender as FastButton).ProductID));
            UpdateReceiptList();
        }

        private void UpdateGridQuickButtons()
        {
            int i = 0;
            grid_QuickButton.Children.Clear();
            foreach (FastButton button in _settingsController.quickButtonList)
            {
                if (!grid_QuickButton.Children.Contains(button))
                {
                    button.Style = FindResource("Flat_Button") as Style;

                    button.SetValue(Grid.ColumnProperty, i % 3);
                    button.SetValue(Grid.RowProperty, i / 3);
                    grid_QuickButton.Children.Add(button);
                    i++;
                }
            }
        }

        private void btn_Remove_Quick_Button(object sender, RoutedEventArgs e)
        {
            int removeThis = _settingsController.quickButtonList.FindIndex(x => x.ProductID == Convert.ToUInt32((sender as Button).Tag));

            _settingsController.quickButtonList.RemoveAll(x => x.ProductID == Convert.ToUInt32((sender as Button).Tag));

            listView_QuickBtn.Items.RemoveAt(removeThis);
            listView_QuickBtn.Items.Refresh();
            UpdateGridQuickButtons();
            SaveQuickButtons();
        }

        private CreateTemporaryProduct _createTempProduct;
        private int _tempID = -1;
        private void btn_Temporary_Click(object sender, RoutedEventArgs e)
        {
            decimal price;
            if (_tempID == -1)
            {
                _tempID = TempProduct.GetNextID();
            }

            if (_createTempProduct == null)
            {
                _createTempProduct = new CreateTemporaryProduct(_storageController, _POSController, _tempID);
                _createTempProduct.UpdateReceiptEventHandler += updateReceiptList;
                _createTempProduct.Closed += delegate { _createTempProduct = null; };
                _tempID++;
            }
            _createTempProduct.Activate();
            _createTempProduct.ShowDialog();
        }

        private void updateReceiptList(object sender, EventArgs e)
        {
            UpdateReceiptList();
        }

        private void btn_PictureFilePath_Click(object sender, RoutedEventArgs e)
        {
            _settingsController.SpecifyPictureFilePath();
            LoadProductImages();
        }

        #region Statistics

        private void OnSelectedStartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] dateTime = datePicker_StartDate.ToString().Split(' ');
            label_CurrentStartDate.Text = $"{ dateTime[0]}";
        }

        private void OnSelectedEndDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] dateTime = datePicker_EndDate.ToString().Split(' ');
            label_CurrentEndDate.Text = $"{ dateTime[0]}";
        }

        private void Button_CreateStatistics_Click(object sender, RoutedEventArgs e)
        {
            ResetStatisticsView();
            ((GridView)listView_Statistics.View).Columns[0].Width = 110;

            int productID = 0;
            if (textBox_StatisticsProductID.Text.Length > 0)
            {
                productID = int.Parse(textBox_StatisticsProductID.Text);
            }
            string brand = comboBox_Brand.Text;
            int groupID = -1;
            if (comboBox_Group.Text != "")
            {
                groupID = _storageController.GroupDictionary.Values.First(x => x.Name == comboBox_Group.Text).ID;
            }
            bool filterProduct = checkBox_Product.IsChecked.Value;
            bool filterBrand = checkBox_Brand.IsChecked.Value;
            bool filterGroup = checkBox_Group.IsChecked.Value;
            _statisticsController.RequestStatistics(filterProduct, productID, filterGroup, groupID, filterBrand, brand, datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);
            _statisticsController.GetReceiptTotalCount(datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);
            _statisticsController.AverageItemsPerReceipt(datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);
            _statisticsController.GetReceiptTotalPrice(datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);
            _statisticsController.GenerateGroupAndBrandSales();

            DisplayStatistics();
            if (_statisticsController.TransactionsForStatistics.Count == 0)
            {
                label_NoTransactions.Visibility = Visibility.Visible;
            }
        }

        private void ResetStatisticsView()
        {
            listView_Statistics.Items.Clear();
            listView_GroupStatistics.Items.Clear();
            listView_BrandStatistics.Items.Clear();
            label_NoTransactions.Visibility = Visibility.Hidden;
        }

        private void DisplayStatistics()
        {
            int productAmount = 0;
            decimal totalTransactionPrice = 0;

            foreach (SaleTransaction transaction in _statisticsController.TransactionsForStatistics)
            {
                listView_Statistics.Items.Add(transaction.StatisticsStrings());
                productAmount += transaction.Amount;
                totalTransactionPrice += transaction.TotalPrice;
            }
            listView_Statistics.Items.Insert(0, new StatisticsListItem("", "Total", $"{productAmount}", $"{Math.Round(totalTransactionPrice, 2)}"));

            if (!(checkBox_Brand.IsChecked.Value || checkBox_Group.IsChecked.Value || checkBox_Product.IsChecked.Value))
            {
                listView_Statistics.Items.Insert(1, _statisticsController.ReceiptStatisticsString());
            }

            foreach (int groupID in _statisticsController.SalesPerGroup.Keys)
            {
                listView_GroupStatistics.Items.Add(_statisticsController.GroupSalesStrings(groupID, totalTransactionPrice));
            }
            foreach (string brand in _statisticsController.SalesPerBrand.Keys)
            {
                listView_BrandStatistics.Items.Add(_statisticsController.BrandSalesStrings(brand, totalTransactionPrice));
            }
        }

        private void Button_StatisticsToday_Click(object sender, RoutedEventArgs e)
        {
            int totalProductAmount = 0;
            ResetStatisticsView();
            ((GridView)listView_Statistics.View).Columns[0].Width = 0;
            _statisticsController.RequestTodayReceipts();
            _statisticsController.CalculatePayments();
            listView_Statistics.Items.Add(new StatisticsListItem("", "Kontant", "", $"{Math.Round(_statisticsController.Payments[0], 2)}"));
            listView_Statistics.Items.Add(new StatisticsListItem("", "Kort", "", $"{Math.Round(_statisticsController.Payments[1], 2)}"));
            listView_Statistics.Items.Add(new StatisticsListItem("", "MobilePay", "", $"{Math.Round(_statisticsController.Payments[2], 2)}"));
            listView_Statistics.Items.Insert(0, new StatisticsListItem("", "Kroner per betalingsmetode:", "", ""));
            _statisticsController.RequestStatistics(false, 0, false, 0, false, "", DateTime.Today, DateTime.Today);
            _statisticsController.GenerateProductSalesAndTotalRevenue();
            foreach (int id in _statisticsController.SalesPerProduct.Keys)
            {
                listView_Statistics.Items.Add(_statisticsController.ProductSalesStrings(id, _statisticsController.SalesPerProduct[id]));
                totalProductAmount += _statisticsController.SalesPerProduct[id].Amount;
            }
            _statisticsController.GenerateGroupAndBrandSales();
            foreach (int groupID in _statisticsController.SalesPerGroup.Keys)
            {
                listView_GroupStatistics.Items.Add(_statisticsController.GroupSalesStrings(groupID, _statisticsController.TotalRevenueToday));
            }

            foreach (string brand in _statisticsController.SalesPerBrand.Keys)
            {
                listView_BrandStatistics.Items.Add(_statisticsController.BrandSalesStrings(brand, _statisticsController.TotalRevenueToday));
            }

            listView_Statistics.Items.Insert(4, new StatisticsListItem("", "Total", $"{totalProductAmount}", $"{Math.Round(_statisticsController.TotalRevenueToday, 2)}"));
            listView_Statistics.Items.Insert(4, new StatisticsListItem("", "Produkter solgt i dag:", "", ""));
            listView_Statistics.Items.Insert(4, new StatisticsListItem("", "", "", ""));
        }

        private void checkBox_Product_Checked(object sender, RoutedEventArgs e)
        {
            checkBox_Brand.IsEnabled = false;
            checkBox_Group.IsEnabled = false;
            checkBox_Brand.IsChecked = false;
            checkBox_Group.IsChecked = false;
            comboBox_Brand.IsEnabled = false;
            comboBox_Group.IsEnabled = false;
        }

        private void checkBox_Product_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBox_Brand.IsEnabled = true;
            checkBox_Group.IsEnabled = true;
            comboBox_Brand.IsEnabled = true;
            comboBox_Group.IsEnabled = true;
        }

        #endregion

        private void TextInputNoNumber(object sender, TextCompositionEventArgs e)
        {
            // Only allows number in textfield
            if (e.Text.Length > 0)
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                    e.Handled = true;
            }
        }

        private void TextInputNoNumberWithComma(object sender, TextCompositionEventArgs e)
        {
            TextBox input = (sender as TextBox);
            //The input string has the format: An unlimited amount of numbers, then 0-1 commas, then 0-2 numbers
            var regex = new System.Text.RegularExpressions.Regex(@"^((\d+)(,{0,1})(\d{0,2}))$");

            e.Handled = !regex.IsMatch(input.Text.Insert(input.CaretIndex, e.Text));
        }

        private void BuildInformationTable()
        {
            //Puts the analytical information, that is in settings under product tab
            foreach (var item in _storageController.InformationGridData)
            {
                InformationGrid.Items.Add(new { title = item[0], value = item[1] });
            }
            InformationGrid.UpdateLayout();
        }

        private void btn_MatchTempProduct_Click(object sender, RoutedEventArgs e)
        {
            ResolveTempProduct ResolveTempProduct = new ResolveTempProduct(_storageController);
            ResolveTempProduct.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchResultsList();
        }

        private void ShowSearchResultsList()
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Visible;
            ConcurrentDictionary<int, SearchProduct> productSearchResults = _storageController.SearchForProduct(txtBox_SearchField.Text);
            listBox_SearchResultsSaleTab.Items.Clear();
            var searchResults = productSearchResults.Values.OrderByDescending(x => x.BrandMatch + x.GroupMatch + x.NameMatch);
            foreach (SearchProduct product in searchResults)
            {
                if (product.CurrentProduct is Product)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Tag = product.CurrentProduct.ID;
                    item.Content = new SaleSearchResultItemControl((product.CurrentProduct as Product).Image, $"{(product.CurrentProduct as Product).Name}\n{product.CurrentProduct.ID}");
                    listBox_SearchResultsSaleTab.Items.Add(item);
                }
                else if (product.CurrentProduct is ServiceProduct)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Tag = product.CurrentProduct.ID;
                    item.Content = new SaleSearchResultItemControl((product.CurrentProduct as ServiceProduct).Image, $"{(product.CurrentProduct as ServiceProduct).Name}\n{product.CurrentProduct.ID}\n{product.CurrentProduct.SalePrice}");
                    item.Background = Brushes.LightBlue;
                    listBox_SearchResultsSaleTab.Items.Add(item);
                    listBox_SearchResultsSaleTab.SelectedIndex = 0;
                }
                else
                {
                    throw new WrongProductTypeException("Du kan ikke søge efter denne type product");
                }
            }
            if (listBox_SearchResultsSaleTab.Items.Count > 0)
            {
                listBox_SearchResultsSaleTab.Focus();
                listBox_SearchResultsSaleTab.SelectedIndex = 0;
            }
            else
            {
                listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;
                txtBox_SearchField.Focus();
                txtBox_SearchField.SelectAll();
            }
        }

        public void SearchFieldLostFocus(object sender, RoutedEventArgs e)
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;
            listBox_SearchResultsSaleTab.UnselectAll();
            listView_Receipt.UnselectAll();
        }

        private void EnterKeyPressedSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtBox_SearchField_Storage.IsFocused)
            {
                btn_search_Storage_Click(sender, e);
            }
            else if (e.Key == Key.Enter && txtBox_SearchField.IsFocused)
            {
                btn_search.Focus();
                ShowSearchResultsList();
            }
            else if (e.Key == Key.Enter && (textBox_AddProductID.IsFocused || textBox_ProductAmount.IsFocused))
            {
                btn_AddProduct_Click(sender, e);
            }
            else if (e.Key == Key.Enter && textBox_discount.IsFocused)
            {
                btn_discount_Click(sender, e);
            }
        }

        private void btn_search_Storage_Click(object sender, RoutedEventArgs e)
        {
            ConcurrentDictionary<int, SearchProduct> productSearchResults = _storageController.SearchForProduct(txtBox_SearchField_Storage.Text);
            LoadProductGrid(productSearchResults);
        }

        #region StorageRoomCreateAddDelete
        CreateStorageRoom _createStorageRoom;
        private void btn_newStorageRoom_Click(object sender, RoutedEventArgs e)
        {
            _createStorageRoom = new CreateStorageRoom(_storageController);
            _createStorageRoom.Closed += delegate { LoadStorageRooms(); };
            _createStorageRoom.Activate();
            _createStorageRoom.Show();
        }

        private void btn_editStorageRoom_Click(object sender, RoutedEventArgs e)
        {
            int storageID = Convert.ToInt32((sender as Button).Tag);
            StorageRoom chosenStorage = _storageController.StorageRoomDictionary[storageID];
            _createStorageRoom = new CreateStorageRoom(_storageController, chosenStorage);
            _createStorageRoom.Closed += delegate { LoadStorageRooms(); };
            _createStorageRoom.Activate();
            _createStorageRoom.Show();
        }

        CreateGroup _createGroup;
        private void btn_editGroup_Click(object sender, RoutedEventArgs e)
        {
            int groupID = Convert.ToInt32((sender as Button).Tag);
            Group chosenGroup = _storageController.GroupDictionary[groupID];
            _createGroup = new CreateGroup(_storageController, chosenGroup);
            _createGroup.Activate();
            _createGroup.Closed += delegate { LoadGroups(); };
            _createGroup.Show();
        }

        private void btn_AddGroup_Click(object sender, RoutedEventArgs e)
        {
            _createGroup = new CreateGroup(_storageController);
            _createGroup.Closed += delegate { LoadGroups(); };
            _createGroup.Activate();
            _createGroup.Show();
        }

        //TODO
        //Hvor bliver de loadet groups og storagerooms vist?
        public void LoadGroups()
        {
            listView_Groups.Items.Clear();
            foreach (KeyValuePair<int, Group> SingleGroup in _storageController.GroupDictionary.Where(x => x.Value.ID != 0))
            {
                listView_Groups.Items.Add(new { groupID = SingleGroup.Key, groupName = SingleGroup.Value.Name, groupDescription = SingleGroup.Value.Description, groupEditWithID = SingleGroup.Key });
            }
        }

        public void LoadStorageRooms()
        {
            listView_StorageRoom.Items.Clear();
            foreach (KeyValuePair<int, StorageRoom> StorageRoom in _storageController.StorageRoomDictionary.Where(x => x.Value.ID != 0))
            {
                listView_StorageRoom.Items.Add(new { storageID = StorageRoom.Key, storageName = StorageRoom.Value.Name, storageDescription = StorageRoom.Value.Description, storageEditWithID = StorageRoom.Key });
            }
        }

        #endregion

        private void btn_OpenAdmin_Click(object sender, RoutedEventArgs e)
        {
            new AdminValidation().ShowDialog();
        }

        private void btn_ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            //Allows you to reset password without having the password, if you have a text file on your desktop with the specified name, and the content says "reset"
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//" + "resetpassword.txt";

            if (File.Exists(desktopPath) && System.IO.File.ReadAllText(desktopPath) == "reset")
            {
                new AdminNewPassword().ShowDialog();
            }
            else
            {
                AdminValidation check = new AdminValidation();
                check.Closed += delegate
                {
                    if (check.IsPasswordCorrect)
                        new AdminNewPassword().ShowDialog();
                };
                check.ShowDialog();
            }
        }

        private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_POSController.GetProductFromID(int.Parse((sender as ListBoxItem).Tag.ToString())) != null)
            {
                _POSController.AddSaleTransaction(_POSController.GetProductFromID(int.Parse((sender as ListBoxItem).Tag.ToString())), 1);
                UpdateReceiptList();
                listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;
            }
        }

        public void ResetPOSController(bool completedPurchase)
        {
            if (completedPurchase || _statisticsController.Payments.Count() == 0)
            {
                Mysql.CheckDatabaseConnection();
                button_DeleteFulReceiptDiscount.Visibility = Visibility.Hidden;
                image_DeleteFullReceiptDiscount.Visibility = Visibility.Hidden;
                text_FullReceiptDiscount.Text = string.Empty;
                _storageController.ReloadAllDictionaries();
                _partlypaid = false;
                ReloadProducts();
                ReloadDisabledProducts();
                LoadQuickButtons();
                SaveQuickButtons();
                LoadGroups();
                DisableDiscountOnReceipt();
                _POSController.StartPurchase();
            }
        }

        private void btn_Cash_Click(object sender, RoutedEventArgs e)
        {
            if (listView_Receipt.HasItems)
            {
                bool CompletedPurchase = false;
                label_TotalPrice.Content = _POSController.CompletePurchase(PaymentMethod_Enum.Cash, PayWithAmount, listView_Receipt, out CompletedPurchase);
                StartsToPay(CompletedPurchase);
                _partlypaid = !CompletedPurchase;
                ResetPOSController(CompletedPurchase);
            }
        }

        private void btn_Dankort_Click(object sender, RoutedEventArgs e)
        {
            if (listView_Receipt.HasItems)
            {
                bool CompletedPurchase = false;
                label_TotalPrice.Content = _POSController.CompletePurchase(PaymentMethod_Enum.Card, PayWithAmount, listView_Receipt, out CompletedPurchase);
                StartsToPay(CompletedPurchase);
                _partlypaid = !CompletedPurchase;
                ResetPOSController(CompletedPurchase);
            }
        }


        private void btn_MobilePay_Click(object sender, RoutedEventArgs e)
        {
            if (listView_Receipt.HasItems)
            {
                bool CompletedPurchase = false;
                label_TotalPrice.Content = _POSController.CompletePurchase(PaymentMethod_Enum.MobilePay, PayWithAmount, listView_Receipt, out CompletedPurchase);
                StartsToPay(CompletedPurchase);
                _partlypaid = !CompletedPurchase;
                ResetPOSController(CompletedPurchase);
            }
        }

        private void StartsToPay(bool HasPartlyPaid)
        {
            txtBox_SearchField.IsEnabled = HasPartlyPaid;
            btn_search.IsEnabled = HasPartlyPaid;
            textBox_discount.IsEnabled = HasPartlyPaid;
            btn_discount.IsEnabled = HasPartlyPaid;
            btn_AddProduct.IsEnabled = HasPartlyPaid;
            btn_Temporary.IsEnabled = HasPartlyPaid;
            btn_AddIcecream.IsEnabled = HasPartlyPaid;
            textBox_AddProductID.IsEnabled = HasPartlyPaid;
        }

        private void DisableDiscountOnReceipt()
        {
            button_DeleteFulReceiptDiscount.Visibility = Visibility.Hidden;
            image_DeleteFullReceiptDiscount.Visibility = Visibility.Hidden;
            text_FullReceiptDiscount.Text = string.Empty;
        }

        AdminValidation adminValid;
        private void InitAdminLogin()
        {
            ImageBrush locked = new ImageBrush();
            ImageBrush unlocked = new ImageBrush();
            locked.ImageSource = Utils.ImageSourceForBitmap(Properties.Resources.if_102_111044LOCK);
            unlocked.ImageSource = Utils.ImageSourceForBitmap(Properties.Resources.if_103_111043UNLOCK);
            image_Admin.Stretch = Stretch.Uniform;

            image_Admin.Source = locked.ImageSource;

            btn_AdminLogin.Click += delegate
            {
                if (_settingsController.isAdmin)
                {
                    _settingsController.isAdmin = false;
                    btn_AdminLogin.Content = "Log ind";
                    image_Admin.Source = locked.ImageSource;

                    label_NoAdmin.Visibility = Visibility.Visible;
                    ChangeStatisticsState();
                }
                else
                {
                    adminValid = new AdminValidation();
                    adminValid.Closed += delegate
                    {
                        if (adminValid.IsPasswordCorrect)
                        {
                            _settingsController.isAdmin = true;
                            btn_AdminLogin.Content = "Log ud";
                            image_Admin.Source = unlocked.ImageSource;
                            label_NoAdmin.Visibility = Visibility.Collapsed;
                            ChangeStatisticsState();
                        }
                    };
                    adminValid.ShowDialog();
                }
            };
        }

        private void ChangeStatisticsState()
        {
            bool state = _settingsController.isAdmin;
            checkBox_Product.IsEnabled = state;
            textBox_StatisticsProductID.IsEnabled = state;
            checkBox_Brand.IsEnabled = state;
            checkBox_Group.IsEnabled = state;
            comboBox_Brand.IsEnabled = state;
            comboBox_Group.IsEnabled = state;
            Button_CreateStatistics.IsEnabled = state;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _POSController.PlacerholderReceipt = new Receipt();
            listView_Receipt.Items.Clear();
            label_TotalPrice.Content = "Total";
            PayWithAmount.Clear();
            StartsToPay(true);
            DisableDiscountOnReceipt();
            ResetPOSController(true);
        }

        private MoveProduct _productMove;
        private void btn_MoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_productMove == null)
            {
                _productMove = new MoveProduct(_storageController, _POSController);
                _productMove.Closing += delegate
                {
                    _productMove = null;
                };
            }
            _productMove.Show();
            _productMove.Activate();
        }

        private void btn_RmtLcl_Click(object sender, RoutedEventArgs e)
        {
            FlipRemoteLocal();
        }

        private void SaveDBSettings(object sender, RoutedEventArgs e)
        {
            SaveDBData();
        }

        //TODO
        //Bliver den brugt?
        private void PortNumberControl(object sender, TextCompositionEventArgs e)
        {
            TextBox input = (e.OriginalSource as TextBox);
            e.Handled = Utils.RegexCheckNumber(input.Text.Insert(input.CaretIndex, e.Text));
        }

        private void listBox_SearchResultsSaleTab_KeyDown(object sender, KeyEventArgs e)
        {
            ListBoxItem selectedItem = ((sender as ListBox).SelectedItem as ListBoxItem);
            int selectedItemID;
            if (int.TryParse(selectedItem.Tag.ToString(), out selectedItemID))
            {
                BaseProduct product = _POSController.GetProductFromID(selectedItemID);
                if (product != null && e.Key == Key.Enter)
                {
                    _POSController.AddSaleTransaction(product);
                    UpdateReceiptList();
                }
            }
            listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;
        }
    
        //TODO
        //?????
        private void SelectAddress(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (sender as TextBox);
            if (textbox != null)
            {
                textbox.SelectAll();
            }
        }

        //TODO
        //?????
        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox textbox = (sender as TextBox);
            if (textbox != null)
            {
                if (!textbox.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    textbox.Focus();
                }
            }
        }

        private OrderTransactionWindow _orderTransactionWindow;
        private void button_OrderTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (_orderTransactionWindow == null)
            {
                _orderTransactionWindow = new OrderTransactionWindow(_storageController, _POSController);
                _orderTransactionWindow.Closing += delegate
                {
                    _orderTransactionWindow = null;
                };
            }
            _orderTransactionWindow.Show();
            _orderTransactionWindow.Activate();
        }

        private void TextInputDiscountExpression(object sender, TextCompositionEventArgs e)
        {
            TextBox input = (sender as TextBox);
            var discountPercent = new System.Text.RegularExpressions.Regex(@"^((?:100|[1-9]?[0-9](,{0,0})(\d{0,2}))%|((\d+)(,{0,1})(\d{0,2}))$)$");
            e.Handled = !discountPercent.IsMatch(input.Text.Insert(input.CaretIndex, e.Text));
        }

        private void btn_discount_Click(object sender, RoutedEventArgs e)
        {
            if (listView_Receipt.SelectedItem != null)
            {
                _POSController.PlacerholderReceipt.DiscountOnSingleTransaction((listView_Receipt.SelectedItem as ReceiptListItem).TransID, textBox_discount.Text);
            }
            else if (listView_Receipt.SelectedItem == null)
            {
                DiscountOnReceipt();
            }
            textBox_discount.Text = string.Empty;
            UpdateReceiptList();
        }

        private void btn_DeleteReceiptDiscount_Click(object sender, EventArgs e)
        {
            SaleTransaction currentSaleTransaction = _POSController.PlacerholderReceipt.Transactions.Where(x => x.GetID() == (sender as ReceiptListItem).TransID).First();
            currentSaleTransaction.DiscountBool = false;
            _POSController.PlacerholderReceipt.UpdateTotalPrice();
            UpdateReceiptList();
        }

        private void DiscountOnReceipt()
        {
            if (textBox_discount.Text.Length > 0)
            {
                _POSController.PlacerholderReceipt.DiscountOnFullReceipt = 0m;
                _POSController.PlacerholderReceipt.UpdateTotalPrice();
                if (textBox_discount.Text.Contains('%'))
                {
                    if (textBox_discount.Text == "%")
                    {
                        textBox_discount.Text = "0%";
                    }
                    decimal percentage = Convert.ToDecimal(textBox_discount.Text.Replace("%", string.Empty));
                    _POSController.PlacerholderReceipt.DiscountOnFullReceipt = _POSController.PlacerholderReceipt.TotalPrice * (percentage / 100m);
                }
                else
                {
                    _POSController.PlacerholderReceipt.DiscountOnFullReceipt = Convert.ToDecimal(textBox_discount.Text);
                }
                text_FullReceiptDiscount.Text = Math.Round(_POSController.PlacerholderReceipt.DiscountOnFullReceipt, 2).ToString() + " DKK rabat på kvittering";

                image_DeleteFullReceiptDiscount.Visibility = Visibility.Visible;
                button_DeleteFulReceiptDiscount.Visibility = Visibility.Visible;

                _POSController.PlacerholderReceipt.UpdateTotalPrice();
            }
        }


        private void btn_AddIcecream_Click(object sender, RoutedEventArgs e)
        {
            AddIcecream Icecream = new AddIcecream();
            Icecream.Closed += delegate
            {
                if (Icecream.textbox_Price.Text != "")
                {
                    _POSController.AddIcecreamTransaction(Decimal.Parse(Icecream.textbox_Price.Text));
                    UpdateReceiptList();
                };
            };
            Icecream.ShowDialog();
        }

        private void FillDeactivatedProductsIntoGrid()
        {
            datagrid_deactivated_products.Items.Clear();
            foreach (var item in _storageController.DisabledProducts)
            {
                datagrid_deactivated_products.Items.Add(new
                {
                    type = "Produkt",
                    id = item.Value.ID.ToString(),
                    name = item.Value.Name.ToString(),
                    group = _storageController.GroupDictionary[item.Value.ProductGroupID].Name,
                    brand = item.Value.Brand,
                    price = item.Value.SalePrice.ToString(),
                });
            }
            foreach (var item in _storageController.DisabledServiceProducts)
            {
                datagrid_deactivated_products.Items.Add(new
                {
                    type = "ServiceProdukt",
                    id = item.Value.ID.ToString(),
                    name = item.Value.Name.ToString(),
                    group = _storageController.GroupDictionary[item.Value.ServiceProductGroupID].Name,
                    brand = "",
                    price = item.Value.SalePrice.ToString(),
                });
            }
        }


        //TODO
        //Nice Method
        public void ReloadDisabledProducts()
        {
            FillDeactivatedProductsIntoGrid();
        }

        private void ActivateProduct(object sender, RoutedEventArgs e)
        {
            if (datagrid_deactivated_products.SelectedIndex != -1)
            {
                char[] seperator = new char[] { ',', ' ' };
                int ID = Convert.ToInt32(datagrid_deactivated_products.SelectedCells[0].Item.ToString().Split(seperator)[7]);
                string ProductString = _storageController.ActivateProduct(ID);
                MessageBox.Show("Du har genaktiveret " + ProductString);
                ReloadDisabledProducts();
                ReloadProducts();
            }
        }

        //TODO
        //Det skal vel ikke være muligt?
        private void EditDisabledProduct(object sender, RoutedEventArgs e)
        {
            if (datagrid_deactivated_products.SelectedIndex != -1)
            {
                char[] seperator = new char[] { ',', ' ' };
                int ID = Convert.ToInt32(datagrid_deactivated_products.SelectedCells[0].Item.ToString().Split(seperator)[7]);
                if (_storageController.DisabledProducts.ContainsKey(ID))
                {
                    Product ProductToActivate = _storageController.DisabledProducts[ID];
                    CreateProduct EditDisabledProduct = new CreateProduct(ProductToActivate, _storageController, _settingsController.isAdmin, true);
                    EditDisabledProduct.SaveAndQuitEvent += SaveAndQuitClick;
                    EditDisabledProduct.Show();
                }
                else
                {
                    ServiceProduct ServiceProductToActivate = _storageController.DisabledServiceProducts[ID];
                    CreateProduct EditDiabledServiceProduct = new CreateProduct(ServiceProductToActivate, _storageController, _settingsController.isAdmin, true);
                    EditDiabledServiceProduct.Show();
                }
                ReloadDisabledProducts();
                ReloadProducts();
            }
        }

        private void SaveAndQuitClick(int ID)
        {
            ShowSpecificInfoProductStorage(ID);
        }

        //TODO
        //Er det ikke både storage og order? Navn
        private void StorageTransactionsHistory()
        {
            listview_SettingsStorage.Height = 500;
            listView_StorageRoom.Height = 500;
            listview_SettingsStorageStorageTransaction.Height = 500;
            grid_StorageSettings.Height = 1865;
            Product product;
            int checkIfNewOrderTrans = 0;
            int checkIfNewStorageTrans = 0;
            checkIfNewOrderTrans = _storageController.OrderTransactionDictionary.Count;
            foreach (KeyValuePair<int, OrderTransaction> orderTrans in _storageController.OrderTransactionDictionary)
            {
                if (_storageController.ProductDictionary.ContainsKey(orderTrans.Value.Product.ID))
                {
                    product = _storageController.ProductDictionary[orderTrans.Value.Product.ID];
                }
                else
                {
                    product = _storageController.DisabledProducts[orderTrans.Value.Product.ID];
                }
                if (_storageController.StorageRoomDictionary.ContainsKey(orderTrans.Value.StorageRoomID))
                {
                    listview_SettingsStorage.Items.Add(new { Received = product.Name, Amount = orderTrans.Value.Amount, StorageRoom = _storageController.StorageRoomDictionary[orderTrans.Value.StorageRoomID].Name, Distributor = orderTrans.Value._supplier });
                }
            }

            foreach (KeyValuePair<int, StorageTransaction> storageTrans in _storageController.StorageTransactionDictionary)
            {
                if (_storageController.ProductDictionary.ContainsKey(storageTrans.Value.Product.ID))
                {
                    product = _storageController.ProductDictionary[storageTrans.Value.Product.ID];
                }
                else
                {
                    product = _storageController.DisabledProducts[storageTrans.Value.Product.ID];
                }
                if (storageTrans.Value.Source != null && storageTrans.Value.Destination != null)
                {
                    listview_SettingsStorageStorageTransaction.Items.Add(new { Received = product.Name, Amount = storageTrans.Value.Amount, StorageRoomSource = storageTrans.Value.Source.Name, StorageRoomDest = storageTrans.Value.Source.Name });
                }
            }
        }

        private void button_DeleteFulReceiptDiscount_Click(object sender, EventArgs e)
        {
            _POSController.PlacerholderReceipt.DiscountOnFullReceipt = 0m;
            _POSController.PlacerholderReceipt.UpdateTotalPrice();
            text_FullReceiptDiscount.Text = string.Empty;
            image_DeleteFullReceiptDiscount.Visibility = Visibility.Hidden;
            button_DeleteFulReceiptDiscount.Visibility = Visibility.Hidden;
            UpdateReceiptList();
        }

        private int checkIfNewOrderTrans = 0;
        private int checkIfNewStorageTrans = 0;
        private void settingsTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_storageController.StorageTransactionDictionary.Count > checkIfNewStorageTrans || _storageController.OrderTransactionDictionary.Count > checkIfNewOrderTrans)
            {
                checkIfNewStorageTrans = _storageController.StorageTransactionDictionary.Count;
                checkIfNewOrderTrans = _storageController.OrderTransactionDictionary.Count;

                StorageTransactionsHistory();
            }
        }

        private void btn_ReloadDatabase_Click(object sender, RoutedEventArgs e)
        {
            _storageController.ClearDictionaries();

            Mysql.CheckDatabaseConnection();
            _storageController.ReloadAllDictionaries();
            ReloadProducts();
            ReloadDisabledProducts();

            //TODO
            //Hvorfor skal de reloades?
            LoadQuickButtons();
            SaveQuickButtons();
        }

        private bool firstLoad = true;
        private void tab_Settings_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_settingsController.isAdmin)
            {
                if (settingsTab.IsSelected && firstLoad)
                {
                    LoadStorageRooms();
                    firstLoad = false;
                }
            }
            else
            {
                if (Mysql.ConnectionWorking)
                {
                    tab_Sale.Focus();
                    tab_Sale.IsSelected = true;
                    MessageBox.Show("Du skal være admin for at gå ind i indstillinger");
                }
            }
        }
    }
}
