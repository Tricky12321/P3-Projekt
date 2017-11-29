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
        public static bool runLoading = true;
        public MainWindow()
        {
            Mysql.CheckDatabaseConnection();
            List<string> OutputList = new List<string>();
            Stopwatch LoadingTimer = new Stopwatch();
            LoadingTimer.Start();
            _storageController = new StorageController();
            _POSController = new POSController(_storageController);
            _settingsController = new SettingsController();
            _statisticsController = new StatisticsController(_storageController);
            OutputList.Add("[1. TIMER] took " + LoadingTimer.ElapsedMilliseconds + "ms");
            InitializeComponent();
            OutputList.Add("[2. TIMER] took " + LoadingTimer.ElapsedMilliseconds + "ms");
            LoadDatabase();
            OutputList.Add("[3. TIMER] took " + LoadingTimer.ElapsedMilliseconds + "ms");
            InitComponents();
            OutputList.Add("[4. TIMER] took " + LoadingTimer.ElapsedMilliseconds + "ms");
            this.KeyDown += new KeyEventHandler(KeyboardHook);
            this.KeyDown += new KeyEventHandler(CtrlHookDown);
            this.KeyDown += new KeyEventHandler(EnterKeyPressedSearch);
            this.KeyUp += new KeyEventHandler(CtrlHookUp);

            foreach (var item in OutputList)
            {
                Debug.WriteLine(item);
            }
            Utils.GetIceCreameID();
            Console.WriteLine("Username: " + Environment.UserName);

            _storageController.MakeSureIcecreamExists();
            this.WindowState = WindowState.Maximized;
            LoadingTimer.Stop();
            OutputList.Add("[TOTAL TIMER] took " + LoadingTimer.ElapsedMilliseconds + "ms");
            _storageController.AddInformation("Loading timer", LoadingTimer.ElapsedMilliseconds + "ms");
            BuildInformationTable();
            LoadQuickButtons();

        }

        public void ReloadProducts()
        {
            _storageController.LoadAllProductsDictionary();
            LoadProductImages();
            LoadProductControlDictionary();
            LoadProductGrid(_storageController.AllProductsDictionary);
        }

        private void KeyboardHook(object sender, KeyEventArgs e)
        {
            if (_ctrlDown && (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl))
            {
                ClickQuickButton(e.Key);
            }
        }

        private void ClickQuickButton(Key btn)
        {
            Debug.Print("Pressed quickbutton combo:" + btn.ToString());
            if (_settingsController.quickButtonKeyList.ContainsKey(btn))
            {
                _settingsController.quickButtonKeyList[btn].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }



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
            Settings.QuickButton11 = QuickButtonsCount >= 12 ? QuickButtons[10].ToString() : null;
            Settings.QuickButton12 = QuickButtonsCount >= 13 ? QuickButtons[11].ToString() : null;
            Settings.QuickButton13 = QuickButtonsCount >= 14 ? QuickButtons[12].ToString() : null;
            Settings.QuickButton14 = QuickButtonsCount >= 15 ? QuickButtons[13].ToString() : null;
            Settings.Save();
            
        }

        public void LoadQuickButtons()
        {
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
            UpdateGridQuickButtons();
        }

        private void AddQuickButton(string NewButton)
        {
            var Settings = Properties.Settings.Default;
            if (NewButton != "")
            {
                string[] NewButtonInfo = NewButton.Split('|');
                int prodID = Convert.ToInt32(NewButtonInfo[0]);
                _settingsController.AddNewQuickButton(NewButtonInfo[1], prodID, grid_QuickButton.Width, grid_QuickButton.Height, btn_FastButton_click);
                listView_QuickBtn.Items.Add(new FastButton() { Button_Name = NewButtonInfo[1], ProductID = prodID });
                listView_QuickBtn.Items.Refresh();
            }
        }

        private void CtrlHookDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                _ctrlDown = true;
            }
        }

        private void CtrlHookUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                _ctrlDown = false;
            }
        }

        private void InitComponents()
        {
            Stopwatch GridButtonTimer = new Stopwatch();
            GridButtonTimer.Start();
            InitGridQuickButtons();
            GridButtonTimer.Stop();
            Debug.WriteLine("[InitGridQuickButtons] took " + GridButtonTimer.ElapsedMilliseconds + "ms");
            Stopwatch StorageGridTimer = new Stopwatch();
            StorageGridTimer.Start();
            InitStorageGridProducts();
            StorageGridTimer.Stop();
            Debug.WriteLine("[InitStorageGridProductsF] took " + StorageGridTimer.ElapsedMilliseconds + "ms");
            AddProductButton();
            Stopwatch Timer1 = new Stopwatch();
            Timer1.Start();
            ReloadProducts();
            Timer1.Stop();
            Debug.WriteLine("[LoadProductImages] took " + Timer1.ElapsedMilliseconds + "ms");
            LoadProductGrid(_storageController.AllProductsDictionary);
            InitStatisticsTab();
            InitAdminLogin();
            Utils.LoadDatabaseSettings(this);
            FillDeactivatedProductsIntoGrid();
        }

        private void InitGridQuickButtons()
        {
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

        private void UpdateReceiptList()
        {
            listView_Receipt.Items.Clear();
            foreach (SaleTransaction transaction in _POSController.PlacerholderReceipt.Transactions)
            {
                AddTransactionToReceipt(transaction);
            }
            label_TotalPrice.Content = _POSController.PlacerholderReceipt.TotalPrice.ToString().Replace('.', ',');
            btn_discount.IsEnabled = true;
        }

        public void InitStorageGridProducts()
        {
            productGrid.VerticalAlignment = VerticalAlignment.Stretch;
            productGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });

            scroll_StorageProduct.Content = productGrid;
        }

        public void LoadDatabase()
        {
            if (Mysql.ConnectionWorking)
            {

                Stopwatch TimeTester = new Stopwatch();
                TimeTester.Start();
                Thread GetAllThread = new Thread(new ThreadStart(_storageController.GetAll));
                GetAllThread.Start();
                while (!_storageController.ThreadsDone)
                {
                    Thread.Sleep(1);
                }
                runLoading = false;
                TimeTester.Stop();
                string Output = "";
                while (_storageController.TimerStrings.TryDequeue(out Output))
                {
                    Debug.WriteLine(Output);
                }
                Debug.WriteLine("[P3] Det tog " + TimeTester.ElapsedMilliseconds + "ms at hente alt fra databasen");
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
            CreateProduct addProductWindow = new CreateProduct(_storageController, this);
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
                EditProductForm = new CreateProduct(_productToEdit as Product, _storageController, this, _settingsController.isAdmin);
            }
            else if (_productToEdit is ServiceProduct)
            {
                EditProductForm = new CreateProduct(_productToEdit as ServiceProduct, _storageController, this, _settingsController.isAdmin);
            }
            else
            {
                throw new WrongProductTypeException("Fejl i forsøg på at redigere produkt");
            }
            EditProductForm.Closed += delegate { ReloadProducts(); };
            EditProductForm.ShowDialog();
        }

        private void ShowSpecificInfoProductStorage(object sender, RoutedEventArgs e)
        {
            ShowSpecificInfoProductStorage(int.Parse((sender as Button).Tag.ToString()));
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
            Stopwatch Timer2 = new Stopwatch();
            Timer2.Start();
            _productControlDictionary.Clear();

            IOrderedEnumerable<KeyValuePair<int, BaseProduct>> ProductList = _storageController.AllProductsDictionary.OrderBy(x => x.Key);
            foreach (KeyValuePair<int, BaseProduct> product in ProductList)
            {
                ProductControl productControl = new ProductControl(product.Value, _storageController.GroupDictionary);
                productControl.btn_ShowMoreInformation.Tag = product.Value.ID;
                productControl.btn_ShowMoreInformation.Click += ShowSpecificInfoProductStorage;

                _productControlDictionary.Add(product.Value.ID, productControl);
            }
            Timer2.Stop();
            Debug.WriteLine("[LoadProductControlDictionary] took " + Timer2.ElapsedMilliseconds + "ms");
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
            foreach (var product in ProductListSorted)
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

                }
                catch (UnauthorizedAccessException e)
                {

                }
            }

            LoadProductControlDictionary();
        }

        public void AddTransactionToReceipt(SaleTransaction transaction)
        {
            ReceiptListItem item;

            if (transaction.Product is TempProduct)
            {
                item = new ReceiptListItem(transaction.GetProductName(), Math.Round(transaction.TotalPrice + transaction.DiscountPrice, 2), transaction.Amount, 't' + transaction.Product.ID.ToString());

                if (transaction.DiscountBool)
                {
                    item.canvas_Discount.Visibility = Visibility.Visible;
                    item.textBlock_DiscountAmount.Text = '-' + Math.Round(transaction.DiscountPrice, 2).ToString() + " : Ny Pris: " + Math.Round(transaction.TotalPrice, 2).ToString();
                    item.textBlock_DiscountAmount.Foreground = Brushes.Red;
                }
            }
            else
            {
                item = new ReceiptListItem(transaction.GetProductName(), transaction.TotalPrice + transaction.DiscountPrice, transaction.Amount, transaction.Product.ID.ToString(), transaction.GetID());
                if (transaction.DiscountPrice > 0m)
                {
                    item.canvas_Discount.Visibility = Visibility.Visible;
                    item.textBlock_DiscountAmount.Text = '-' + Math.Round(transaction.DiscountPrice, 2).ToString() + " : Ny Pris: " + Math.Round(transaction.TotalPrice, 2).ToString();
                }
            }
            item.Delete_Button_Event += btn_DeleteProduct_Click;
            item.Increment_Button_Event += btn_Increment_Click;
            item.Decrement_Button_Event += btn_Decrement_Click;
            listView_Receipt.Items.Add(item);
        }

        private void btn_Increment_Click(object sender, EventArgs e)
        {
            _POSController.ChangeTransactionAmount(sender, e, 1);
            UpdateReceiptList();
        }

        private void btn_Decrement_Click(object sender, EventArgs e)
        {
            _POSController.ChangeTransactionAmount(sender, e, -1);
            UpdateReceiptList();
        }

        private void btn_DeleteProduct_Click(object sender, EventArgs e)
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

        private void btn_AddProduct_Click(object sender, RoutedEventArgs e)
        {
            int inputInt;
            int.TryParse(textBox_AddProductID.Text, out inputInt);
            BaseProduct ProductToAdd = _POSController.GetProductFromID(inputInt);
            if (ProductToAdd != null)
            {
                _POSController.AddSaleTransaction(ProductToAdd, int.Parse(textBox_ProductAmount.Text));
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
            int maxButtons = 14;

            if (_POSController.GetProductFromID(inputInt) != null && !_settingsController.quickButtonList.Any(x => x.ProductID == inputInt) && checkIfTooManyQuickButtons(maxButtons))
            {
                _settingsController.AddNewQuickButton(textBox_CreateQuickBtnName.Text, inputInt, grid_QuickButton.Width, grid_QuickButton.Height, btn_FastButton_click);
                listView_QuickBtn.Items.Add(new FastButton() { Button_Name = textBox_CreateQuickBtnName.Text, ProductID = inputInt });
                listView_QuickBtn.Items.Refresh();

            }
            else if (_settingsController.quickButtonList.Any(x => x.ProductID == inputInt))
            {
                Utils.ShowErrorWarning($"Produkt med dette ID {inputInt} er allerede oprettet");
            }
            else if (!checkIfTooManyQuickButtons(maxButtons))
            {
                Utils.ShowErrorWarning($"Der kan ikke oprettes flere hurtigknapper, der må højest være {maxButtons} hurtigknapper ");
            }
            else
            {
                Utils.ShowErrorWarning($"Produkt med ID {inputInt} findes ikke på lageret");
            }
            SaveQuickButtons();
        }

        private bool checkIfTooManyQuickButtons(int maximumButtons)
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

                    button.SetValue(Grid.ColumnProperty, i % 2);
                    button.SetValue(Grid.RowProperty, i / 2);
                    grid_QuickButton.Children.Add(button);
                    ++i;
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
                _createTempProduct = new CreateTemporaryProduct();
                _createTempProduct.Closed += delegate { _createTempProduct = null; };
                _createTempProduct.btn_AddTempProduct.Click += delegate
                {
                    if (decimal.TryParse(_createTempProduct.textbox_Price.Text, out price) && price > 0  && _createTempProduct.textbox_Description != null)
                    {
                        string description = _createTempProduct.textbox_Description.Text;
                        price = decimal.Parse(_createTempProduct.textbox_Price.Text);
                        int amount = int.Parse(_createTempProduct.textBox_ProductAmount.Text);
                        TempProduct NewTemp = _storageController.CreateTempProduct(description, price, _tempID);
                        _POSController.AddSaleTransaction(NewTemp, amount);
                        UpdateReceiptList();
                        _createTempProduct.Close();
                        MessageBox.Show($"Produkt med beskrivelsen: {description}\nPris: {price}\nAntal: {amount}\nEr oprettet!");
                        ++_tempID;
                    }
                    else
                    {
                        _createTempProduct.textbox_Description.BorderBrush = Brushes.Red;
                        _createTempProduct.textbox_Price.BorderBrush = Brushes.Red;
                    }
                };
            }
            _createTempProduct.Activate();
            _createTempProduct.ShowDialog();
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

            int productID = 0;
            if (textBox_StatisticsProductID.Text.Length > 0)
            {
                productID = int.Parse(textBox_StatisticsProductID.Text);
            }
            string brand = comboBox_Brand.Text;
            int groupID = 0;
            if (comboBox_Group.Text != "")
            {
                groupID = _storageController.GroupDictionary.Values.First(x => x.Name == comboBox_Group.Text).ID;
            }
            bool filterProduct = checkBox_Product.IsChecked.Value;
            bool filterBrand = checkBox_Brand.IsChecked.Value;
            bool filterGroup = checkBox_Group.IsChecked.Value;
            _statisticsController.TransactionsForStatistics = new List<SaleTransaction>();
            string ProductqueryString = _statisticsController.GetProductsQueryString(filterProduct, productID, filterGroup, groupID, filterBrand, brand, datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);
            string ServiceProductQueryString = _statisticsController.GetServiceProductsQueryString(filterProduct, productID, filterGroup, groupID, datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);
            _statisticsController.RequestStatisticsDate(ProductqueryString);
            _statisticsController.RequestStatisticsDate(ServiceProductQueryString);
            _statisticsController.GetReceiptTotalCount(datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);
            _statisticsController.GetReceiptTotalPrice(datePicker_StartDate.SelectedDate.Value, datePicker_EndDate.SelectedDate.Value);

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
            listView_Statistics.Items.Insert(0, new StatisticsListItem("", "Total", $"{productAmount}", $"{totalTransactionPrice}"));

            if (!(checkBox_Brand.IsChecked.Value || checkBox_Group.IsChecked.Value || checkBox_Product.IsChecked.Value))
            {
                listView_Statistics.Items.Insert(1, _statisticsController.ReceiptStatisticsString());
            }

            _statisticsController.GenerateGroupAndBrandSales();
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
            ResetStatisticsView();
            _statisticsController.RequestTodayReceipts();
            _statisticsController.CalculatePayments();
            listView_Statistics.Items.Add(new StatisticsListItem($"{DateTime.Today.ToString("dd/MM/yy")}", "Kontant", "", $"{_statisticsController.Payments[0]}"));
            listView_Statistics.Items.Add(new StatisticsListItem($"{DateTime.Today.ToString("dd/MM/yy")}", "Kort", "", $"{_statisticsController.Payments[1]}"));
            listView_Statistics.Items.Add(new StatisticsListItem($"{DateTime.Today.ToString("dd/MM/yy")}", "MobilePay", "", $"{_statisticsController.Payments[2]}"));
            /*string queryString = _statisticsController.GetQueryString(false, 0, false, 0, false, "", DateTime.Today, DateTime.Today);
            _statisticsController.RequestStatisticsDate(queryString);
            _statisticsController.GenerateGroupAndBrandSales();
            foreach (int groupID in _statisticsController.SalesPerGroup.Keys)
            {
                listView_GroupStatistics.Items.Add(_statisticsController.GroupSalesStrings(groupID, totalTransactionPrice));
            }*/
        }

        private void checkBox_Product_Checked(object sender, RoutedEventArgs e)
        {
            checkBox_Brand.IsEnabled = false;
            checkBox_Group.IsEnabled = false;
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
            var re = new System.Text.RegularExpressions.Regex(@"^((\d+)(,{0,1})(\d{0,2}))$");

            e.Handled = !re.IsMatch(input.Text.Insert(input.CaretIndex, e.Text));
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

        ResovleTempProduct _resolveTempProduct;
        private void btn_MergeTempProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_storageController.TempProductList.Where(x => x.Value.Resolved == false).Count() > 0 && _resolveTempProduct == null)
            {
                _resolveTempProduct = new ResovleTempProduct(_storageController);
                _resolveTempProduct.Show();
                _resolveTempProduct.Closed += delegate {
                    _resolveTempProduct = null;
                };
            }
            else
            {
                MessageBox.Show("Der findes ingen midlertidige produkter");
            }
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

            _createStorageRoom = new CreateStorageRoom(_storageController, this);

            _createStorageRoom.Activate();
            _createStorageRoom.Show();
        }

        private void btn_editStorageRoom_Click(object sender, RoutedEventArgs e)
        {
            //int storageID = Convert.ToInt32(comboBox_storageRoomSelect.Text.Split(' ').First());
            int storageID = Convert.ToInt32((sender as Button).Tag);
            StorageRoom chosenStorage = _storageController.StorageRoomDictionary[storageID];
            _createStorageRoom = new CreateStorageRoom(_storageController, this, chosenStorage);
            _createStorageRoom.Activate();
            _createStorageRoom.Show();
        }

        public void LoadStorageRooms()
        {
            listView_StorageRoom.Items.Clear();
            foreach (KeyValuePair<int, StorageRoom> StorageRoom in _storageController.StorageRoomDictionary.Where(x => x.Value.ID != 0))
            {
                listView_StorageRoom.Items.Add(new { storageID = StorageRoom.Key, storageName = StorageRoom.Value.Name, storageDescription = StorageRoom.Value.Description, storageEditWithID = StorageRoom.Key });
            }
        }

        private bool firstLoad = true;
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (settingsTab.IsSelected && firstLoad)
            {
                LoadStorageRooms();
                firstLoad = false;
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

        private void btn_Cash_Click(object sender, RoutedEventArgs e)
        {
            CompletePurchase(PaymentMethod_Enum.Cash);
        }

        private void btn_Dankort_Click(object sender, RoutedEventArgs e)
        {
            CompletePurchase(PaymentMethod_Enum.Card);
        }

        private void btn_MobilePay_Click(object sender, RoutedEventArgs e)
        {
            CompletePurchase(PaymentMethod_Enum.MobilePay);
        }

        public void CompletePurchase(PaymentMethod_Enum PaymentMethod)
        {
            if (listView_Receipt.HasItems)
            {
                decimal PriceToPay = Convert.ToDecimal(label_TotalPrice.Content.ToString().Replace(',', '.'));
                if (_POSController.PlacerholderReceipt.TotalPriceToPay == -1m)
                {
                    _POSController.PlacerholderReceipt.TotalPriceToPay = PriceToPay;
                }
                decimal PaymentAmount;

                if (PayWithAmount.Text.Length == 0)
                {
                    PaymentAmount = Convert.ToDecimal(label_TotalPrice.Content.ToString().Replace(',', '.'));
                }
                else
                {
                    PaymentAmount = Convert.ToDecimal(PayWithAmount.Text);
                }

                Payment NewPayment = new Payment(Receipt.GetNextID(), PaymentAmount, PaymentMethod);
                _POSController.PlacerholderReceipt.Payments.Add(NewPayment);

                PayWithAmount.Text = "";
                label_TotalPrice.Content = $"{PriceToPay - NewPayment.Amount}".Replace('.', ',');
                if (_POSController.PlacerholderReceipt.PaidPrice >= _POSController.PlacerholderReceipt.TotalPrice)
                {
                    SaleTransaction.SetStorageController(_storageController);
                    //_POSController.PlacerholderReceipt.PaymentMethod = PaymentMethod;
                    Thread NewThread = new Thread(new ThreadStart(_POSController.ExecuteReceipt));
                    NewThread.Name = "ExecuteReceipt Thread";
                    NewThread.Start();
                    listView_Receipt.Items.Clear();
                    if (_POSController.PlacerholderReceipt.PaidPrice > _POSController.PlacerholderReceipt.TotalPrice)
                    {
                        label_TotalPrice.Content = "Retur: " + (_POSController.PlacerholderReceipt.PaidPrice - _POSController.PlacerholderReceipt.TotalPrice).ToString().Replace('.', ',').Replace('-', ' ');
                    }
                }
            }
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

        private void MoveProductWindow()
        {
        }

        private void btn_RmtLcl_Click(object sender, RoutedEventArgs e)
        {
            Utils.FlipRemoteLocal(this);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            listView_Receipt.UnselectAll();
        }

        private void SaveDBSettings(object sender, RoutedEventArgs e)
        {
            Utils.SaveDBData(this);
        }

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

        private void SelectAddress(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (sender as TextBox);
            if (textbox != null)
            {
                textbox.SelectAll();
            }
        }

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
            var discountPercent = new System.Text.RegularExpressions.Regex(@"^((?:100|[1-9]?[0-9](,{0,1})(\d{1,2}))%|((\d+)(,{0,1})(\d{0,2}))$)$");
            e.Handled = !discountPercent.IsMatch(input.Text.Insert(input.CaretIndex, e.Text));
        }

        private void btn_discount_Click(object sender, RoutedEventArgs e)
        {
            if (listView_Receipt.SelectedItem != null)
            {
                DiscountSingleTransaction();
            }
            else if (listView_Receipt.SelectedItem == null)
            {
                DiscountOnReceipt();
            }
        }

        private void DiscountSingleTransaction()
        {
            _POSController.PlacerholderReceipt.DiscountOnSingleTransaction((listView_Receipt.SelectedItem as ReceiptListItem).TransID, textBox_discount.Text);            
            UpdateReceiptList();
        }

        private void DiscountOnReceipt()
        {
            if (textBox_discount.Text.Contains('%'))
            {
                decimal percentage = Convert.ToDecimal(textBox_discount.Text.Remove(textBox_discount.Text.Length - 1, 1));
                _POSController.PlacerholderReceipt.DiscountOnFullReceipt = _POSController.PlacerholderReceipt.TotalPrice * (percentage / 100m);
            }
            else
            {
                _POSController.PlacerholderReceipt.DiscountOnFullReceipt = Convert.ToDecimal(textBox_discount.Text);
            }
            text_FullReceiptDiscount.Text = "Der er givet " + Math.Round(_POSController.PlacerholderReceipt.DiscountOnFullReceipt, 2).ToString() + " DKK rabat på kvitteringen";

            _POSController.PlacerholderReceipt.UpdateTotalPrice();
            UpdateReceiptList();
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
                string ProductString = "";
                if (_storageController.DisabledProducts.ContainsKey(ID))
                {
                    Product ProductToActivate;
                    _storageController.DisabledProducts.TryRemove(ID, out ProductToActivate);
                    ProductToActivate.ActivateProduct();
                    _storageController.AllProductsDictionary.TryAdd(ID, ProductToActivate);
                    _storageController.ProductDictionary.TryAdd(ID, ProductToActivate);
                    datagrid_deactivated_products.Items.Remove(datagrid_deactivated_products.SelectedItem);
                    ProductString = ProductToActivate.ToString();
                }
                else
                {
                    ServiceProduct ServiceProductToActivate;
                    _storageController.DisabledServiceProducts.TryRemove(ID, out ServiceProductToActivate);
                    ServiceProductToActivate.ActivateProduct();
                    _storageController.AllProductsDictionary.TryAdd(ID, ServiceProductToActivate);
                    _storageController.ServiceProductDictionary.TryAdd(ID, ServiceProductToActivate);
                    datagrid_deactivated_products.Items.Remove(datagrid_deactivated_products.SelectedItem);
                    ProductString = ServiceProductToActivate.ToString();
                }

                MessageBox.Show("Du har genaktiveret " + ProductString);
                ReloadDisabledProducts();
                ReloadProducts();

            }
        }
        private void EditDisabledProduct(object sender, RoutedEventArgs e)
        {
            if (datagrid_deactivated_products.SelectedIndex != -1)
            {
                char[] seperator = new char[] { ',', ' ' };
                int ID = Convert.ToInt32(datagrid_deactivated_products.SelectedCells[0].Item.ToString().Split(seperator)[7]);
                if (_storageController.DisabledProducts.ContainsKey(ID))
                {
                    Product ProductToActivate = _storageController.DisabledProducts[ID];
                    CreateProduct EditDiabledProduct = new CreateProduct(ProductToActivate, _storageController, this, _settingsController.isAdmin, true);
                    EditDiabledProduct.Show();
                }
                else
                {
                    ServiceProduct ServiceProductToActivate = _storageController.DisabledServiceProducts[ID];
                    CreateProduct EditDiabledServiceProduct = new CreateProduct(ServiceProductToActivate, _storageController, this, _settingsController.isAdmin, true);
                    EditDiabledServiceProduct.Show();
                }
                ReloadDisabledProducts();
                ReloadProducts();

            }
        }

        private void StorageTransactionsHistory()
        {
            List<OrderTransaction> orderTransList = StorageController.GetAllOrderTransactions();
            foreach (var ordertrans in orderTransList)
            {
                //listview_SettingsStorage.Items.Add(new { Recieved = ordertrans.Product.});
            }
            List<StorageTransaction> storageTransList = StorageController.GetAllStorageTransactions();

        }

        private void settingsTab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StorageTransactionsHistory();
        }
    }
}
