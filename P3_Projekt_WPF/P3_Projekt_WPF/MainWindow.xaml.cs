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

            LoadingTimer.Stop();
            OutputList.Add("[TOTAL TIMER] took " + LoadingTimer.ElapsedMilliseconds + "ms");
            foreach (var item in OutputList)
            {
                Debug.WriteLine(item);
            }
            Utils.GetIceCreameID();
            Console.WriteLine("Username: " + Environment.UserName);
        }

        public void ReloadProducts()
        {
            _storageController.LoadAllProductsDictionary();
            LoadProductImages();
            LoadProductControlDictionary();
            LoadProductGrid(_storageController.AllProductsDictionary);
        }

        private void showloadform()
        {
            LoadingScreen load = new LoadingScreen();
            load.ShowDialog();
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
            Debug.WriteLine("[InitStorageGridProducts] took " + StorageGridTimer.ElapsedMilliseconds + "ms");
            AddProductButton();
            Stopwatch Timer1 = new Stopwatch();
            Timer1.Start();
            ReloadProducts();
            Timer1.Stop();
            Debug.WriteLine("[LoadProductImages] took " + Timer1.ElapsedMilliseconds + "ms");
            LoadProductGrid(_storageController.AllProductsDictionary);
            BuildInformationTable();
            InitStatisticsTab();
            InitAdminLogin();
            Utils.LoadDatabaseSettings(this);
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
            var products = _storageController.ProductDictionary.Values.Select(x => x.Brand).Distinct();
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
            label_TotalPrice.Content = _POSController.PlacerholderReceipt.TotalPrice;
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
            addProductWindow.Show();
        }

        private bool _firstClick = true;
        private BaseProduct _productToEdit;
        private void EditProductClick(object sender, RoutedEventArgs e)
        {
            CreateProduct EditProductForm = new CreateProduct();
            if (_productToEdit is Product)
            {
                EditProductForm = new CreateProduct(_productToEdit as Product, _storageController, this);
            }
            else if (_productToEdit is ServiceProduct)
            {
                EditProductForm = new CreateProduct(_productToEdit as ServiceProduct, _storageController, this);
            }
            else
            {
                throw new WrongProductTypeException("Fejl i forsøg på at redigere produkt");
            }


            EditProductForm.Show();

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

                textBlock_ChosenProduct.Text = $"ID: {_productToEdit.ID}\nNavn: {(_productToEdit as Product).Name}\nGruppe: {_storageController.GroupDictionary[(_productToEdit as Product).ProductGroupID].Name}\nMærke: {(_productToEdit as Product).Brand}\nPris: {_productToEdit.SalePrice}\nTilbudspris: {(_productToEdit as Product).DiscountPrice}\nIndkøbspris: {(_productToEdit as Product).PurchasePrice}\nLagerstatus:";
                foreach (KeyValuePair<int, int> storageWithAmount in (_productToEdit as Product).StorageWithAmount)
                {
                    textBlock_ChosenProduct.Text += $"\n  - {_storageController.StorageRoomDictionary[storageWithAmount.Key].Name} har {storageWithAmount.Value} stk.";
                }

            }
            else if (_productToEdit is ServiceProduct)
            {
                if ((_productToEdit as ServiceProduct).Image != null)
                {
                    image_ChosenProduct.Source = (_productToEdit as ServiceProduct).Image.Source;
                }

                textBlock_ChosenProduct.Text = $"ID: {_productToEdit.ID}\nNavn: {(_productToEdit as ServiceProduct).Name}\nGruppe: {_storageController.GroupDictionary[(_productToEdit as ServiceProduct).ServiceProductGroupID].Name}\nPris: {_productToEdit.SalePrice}\nGruppepris: {(_productToEdit as ServiceProduct).GroupPrice}";
            }
            else
            {
                throw new WrongProductTypeException("Kunne ikke vise information for et produkt af denne type " + _productToEdit.GetType().ToString());
            }
        }

        public void StorageTabClick(object sender, RoutedEventArgs e)
        {
            //LoadProductGrid();
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
            /*
            Thread productControlThread = new Thread(LoadProductControlDictionary);
            productControlThread.Name = "Product Control Load Thread";
            productControlThread.SetApartmentState(ApartmentState.STA);
            productControlThread.Start();
            */
            LoadProductControlDictionary();
        }

        public void AddTransactionToReceipt(SaleTransaction transaction)
        {
            if (transaction.Product is TempProduct)
            {
                listView_Receipt.Items.Add(new ReceiptListItem { String_Product = (transaction.Product as TempProduct).Description, Amount = transaction.Amount, Price = $"{transaction.GetProductPrice()}", IDTag = $"t{transaction.Product.ID}" });
            }
            else
            {
                listView_Receipt.Items.Add(new ReceiptListItem { String_Product = transaction.GetProductName(), Amount = transaction.Amount, Price = $"{transaction.GetProductPrice()}", IDTag = transaction.Product.ID.ToString() });
            }
        }



        private void btn_Increment_Click(object sender, RoutedEventArgs e)
        {
            int productID = Convert.ToInt32((sender as Button).Tag);
            _POSController.PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().Amount++;
            _POSController.PlacerholderReceipt.UpdateTotalPrice();
            UpdateReceiptList();
        }

        private void btn_Decrement_Click(object sender, RoutedEventArgs e)
        {
            int productID = Convert.ToInt32((sender as Button).Tag);
            _POSController.PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().Amount--;
            _POSController.PlacerholderReceipt.UpdateTotalPrice();
            UpdateReceiptList();

        }

        private void btn_DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag.ToString().Contains("t"))
            {
                string tempID = Convert.ToString((sender as Button).Tag);
                _POSController.PlacerholderReceipt.RemoveTransaction(tempID);
            }
            else
            {
                int productID = Convert.ToInt32((sender as Button).Tag);
                _POSController.PlacerholderReceipt.RemoveTransaction(productID);
                _POSController.PlacerholderReceipt.UpdateTotalPrice();
            }
            UpdateReceiptList();
        }

        private void listView_Receipt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {

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
                textBox_AddProductID.SelectAll();
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
        }

        private bool checkIfTooManyQuickButtons(int maximumButtons)
        {
            if (_settingsController.quickButtonList.Count < maximumButtons)
            {
                return true;
            }
            else
            {
                return false;
            }
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

                    grid_QuickButton.Children.Add(button);
                    button.SetValue(Grid.ColumnProperty, i % 2);
                    button.SetValue(Grid.RowProperty, i / 2);
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
                    if (decimal.TryParse(_createTempProduct.textbox_Price.Text, out price) && _createTempProduct.textbox_Description != null)
                    {
                        string description = _createTempProduct.textbox_Description.Text;
                        price = decimal.Parse(_createTempProduct.textbox_Price.Text);
                        int amount = int.Parse(_createTempProduct.textBox_ProductAmount.Text);
                        TempProduct NewTemp = _storageController.CreateTempProduct(description, price);
                        _POSController.AddSaleTransaction(NewTemp, amount);
                        NewTemp.ID = _tempID;
                        UpdateReceiptList();
                        _createTempProduct.Close();
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
            _createTempProduct.Show();
        }

        private void btn_PictureFilePath_Click(object sender, RoutedEventArgs e)
        {
            _settingsController.SpecifyPictureFilePath();
            LoadProductImages();
        }

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

        //Today?? Yesterday??
        private void Button_CreateStatistics_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = datePicker_StartDate.SelectedDate.Value;
            DateTime endDate = datePicker_EndDate.SelectedDate.Value;

            if (_settingsController.isAdmin)
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
                bool filterID = false;
                bool filterBrand = false;
                bool filterGroup = false;

                CheckboxChecker(ref filterID, ref filterBrand, ref filterGroup);

                string queryString = _statisticsController.GetQueryString(filterID, productID, filterGroup, groupID, filterBrand, brand);
                _statisticsController.RequestStatisticsDate(startDate, endDate, queryString);

                DisplayStatistics();
                if (_statisticsController.TransactionsForStatistics.Count == 0)
                {
                    label_NoTransactions.Visibility = Visibility.Visible;
                }
            }
            else
            {
                string queryString = _statisticsController.GetQueryString(false, 0, false, 0, false, "");
                _statisticsController.RequestStatisticsDate(DateTime.Today, DateTime.Today, queryString);
                ResetStatisticsView();
                DisplayStatistics();
            }
        }

        private void ResetStatisticsView()
        {
            listView_Statistics.Items.Clear();
            label_NoTransactions.Visibility = Visibility.Hidden;
        }

        private void CheckboxChecker(ref bool id, ref bool brand, ref bool group)
        {
            if (!checkBox_Product.IsChecked.Value)
            {
                id = true;
            }
            if (!checkBox_Brand.IsChecked.Value)
            {
                brand = true;
            }
            if (!checkBox_Group.IsChecked.Value)
            {
                group = true;
            }
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

            /*listView_Statistics.Items.Insert(1, _statisticsController.GetReceiptStatistics());

            _statisticsController.GenerateGroupSales();
            foreach(int groupID in _statisticsController.SalesPerGroup.Keys)
            {
                listView_GroupStatistics.Items.Add(_statisticsController.GroupSalesStrings(groupID, totalTransactionPrice));
            }*/
        }

        private void Button_DateToday_Click(object sender, RoutedEventArgs e)
        {
            datePicker_StartDate.SelectedDate = DateTime.Today;
            datePicker_EndDate.SelectedDate = DateTime.Today;
            Button_CreateStatistics_Click(sender, e);
        }

        private void Button_DateYesterday_Click(object sender, RoutedEventArgs e)
        {
            datePicker_StartDate.SelectedDate = DateTime.Today.AddDays(-1);
            datePicker_EndDate.SelectedDate = DateTime.Today.AddDays(-1);
            Button_CreateStatistics_Click(sender, e);
        }


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
            InformationGrid.CanUserAddRows = false;
            foreach (var item in _storageController.InformationGridData)
            {
                InformationGrid.Items.Add(new { title = item[0], value = item[1] });
            }
            InformationGrid.UpdateLayout();
        }

        ResovleTempProduct _resolveTempProduct;
        private void btn_MergeTempProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_storageController.TempProductList.Where(x => x.Value.Resolved == false).Count() > 0)
            {
                InitMergeWindow();
            }
        }

        private void InitMergeWindow()
        {
            int index = 0;
            List<TempListItem> ItemList = new List<TempListItem>();
            var tempProducts = _storageController.TempProductList.Where(x => x.Value.Resolved == false).ToList();

            if (_resolveTempProduct == null)
            {
                _resolveTempProduct = new ResovleTempProduct();
                _resolveTempProduct.button_Merge.IsEnabled = false;
                _resolveTempProduct.Closed += delegate { _resolveTempProduct = null; };
                _resolveTempProduct.MouseLeftButtonUp += delegate
                {
                    index = _resolveTempProduct.listview_ProductsToMerge.SelectedIndex;
                    if (index <= tempProducts.Count() && index >= 0)
                    {
                        _resolveTempProduct.textBox_TempProductInfo.Text = tempProducts[index].Value.Description;
                    }
                };
                _resolveTempProduct.textBox_IDToMerge.KeyUp += delegate { IDToMerge(); };
                _resolveTempProduct.button_Merge.Click += delegate
                {
                    _storageController.MergeTempProduct(tempProducts[index].Value, int.Parse(_resolveTempProduct.textBox_IDToMerge.Text));

                };
            }

            foreach (var tempProductsToListView in tempProducts)
            {
                ItemList.Add(new TempListItem { Description = tempProductsToListView.Value.Description, Price = tempProductsToListView.Value.SalePrice });
            }
            _resolveTempProduct.listview_ProductsToMerge.ItemsSource = ItemList;
            _resolveTempProduct.Show();
            _resolveTempProduct.Activate();
        }

        private Product IDToMerge()
        {
            int validInput = 0;
            bool input = true;
            if (int.TryParse(_resolveTempProduct.textBox_IDToMerge.Text, out validInput))
            {
                try
                {
                    var productToMerge = _storageController.ProductDictionary[int.Parse(_resolveTempProduct.textBox_IDToMerge.Text)];
                    _resolveTempProduct.Label_MergeInfo.Content = productToMerge.Name;
                    _resolveTempProduct.button_Merge.IsEnabled = true;
                    return productToMerge;
                }
                catch (KeyNotFoundException)
                {
                    _resolveTempProduct.Label_MergeInfo.Content = "Ugyldigt Produkt ID";
                    _resolveTempProduct.button_Merge.IsEnabled = false;
                }
            }
            else
            {
                _resolveTempProduct.Label_MergeInfo.Content = "Forkert Input";
                _resolveTempProduct.button_Merge.IsEnabled = false;
            }
            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            listBox_SearchResultsSaleTab.Visibility = Visibility.Visible;
            ConcurrentDictionary<int, SearchProduct> productSearchResults = _storageController.SearchForProduct(txtBox_SearchField.Text);
            listBox_SearchResultsSaleTab.Items.Clear();
            var searchResults = productSearchResults.Values.OrderByDescending(x => x.BrandMatch + x.GroupMatch + x.NameMatch);
            foreach (SearchProduct product in searchResults)
            {
                if (product.CurrentProduct is Product)
                {
                    var item = new ListBoxItem();
                    item.Tag = product.CurrentProduct.ID;
                    item.Content = new SaleSearchResultItemControl((product.CurrentProduct as Product).Image, $"{(product.CurrentProduct as Product).Name}\n{product.CurrentProduct.ID}");
                    listBox_SearchResultsSaleTab.Items.Add(item);
                }
                else if (product.CurrentProduct is ServiceProduct)
                {
                    var item = new ListBoxItem();
                    item.Tag = product.CurrentProduct.ID;
                    item.Content = new SaleSearchResultItemControl((product.CurrentProduct as ServiceProduct).Image, $"{(product.CurrentProduct as ServiceProduct).Name}\n{product.CurrentProduct.ID}");
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
        }

        public void SearchFieldLostFocus(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(5);
            if (!listBox_SearchResultsSaleTab.IsFocused)
            {
                listBox_SearchResultsSaleTab.Visibility = Visibility.Hidden;
            }
        }

        private void EnterKeyPressedSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtBox_SearchField_Storage.IsFocused)
            {
                btn_search_Storage_Click(sender, e);
            }
            else if (e.Key == Key.Enter && txtBox_SearchField.IsFocused)
            {
                ///////////
            }
            else if (e.Key == Key.Enter && (textBox_AddProductID.IsFocused || textBox_ProductAmount.IsFocused))
            {
                btn_AddProduct_Click(sender, e);
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

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//" + "resetpassword.txt";

            if (File.Exists(desktopPath) && System.IO.File.ReadAllText(desktopPath) == "reset")
            {
                new AdminNewPassword().ShowDialog();
            }
            else
            {
                var check = new AdminValidation();
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
                decimal PriceToPay = Convert.ToDecimal(label_TotalPrice.Content);
                decimal PaymentAmount;
                if (PayWithAmount.Text.Length == 0)
                {
                    PaymentAmount = Convert.ToDecimal(label_TotalPrice.Content);
                }
                else
                {
                    PaymentAmount = Convert.ToDecimal(PayWithAmount.Text);
                }

                if (PriceToPay > PaymentAmount)
                {
                    MessageBox.Show("Det betalte beløb er ikke højere end prisen for varene.");
                }
                else
                {
                    SaleTransaction.SetStorageController(_storageController);
                    _POSController.PlacerholderReceipt.PaymentMethod = PaymentMethod;
                    Thread NewThread = new Thread(new ThreadStart(_POSController.ExecuteReceipt));
                    NewThread.Name = "ExecuteReceipt Thread";
                    NewThread.Start();
                    listView_Receipt.Items.Clear();
                    label_TotalPrice.Content = "Retur: " + (PriceToPay - PaymentAmount).ToString();
                    PayWithAmount.Text = "";
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
                }
                else
                {
                    adminValid = new AdminValidation();
                    adminValid.ShowDialog();
                    _settingsController.isAdmin = true;
                    btn_AdminLogin.Content = "Log ud";
                    image_Admin.Source = unlocked.ImageSource;
                    label_NoAdmin.Visibility = Visibility.Collapsed;

                }
            };
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _POSController.PlacerholderReceipt = new Receipt();
            listView_Receipt.Items.Clear();
            label_TotalPrice.Content = "Total";
            PayWithAmount.Clear();
        }

        private MoveProduct productMove;
        private void btn_MoveProduct_Click(object sender, RoutedEventArgs e)
        {
            productMove = new MoveProduct(_storageController, _POSController);

            productMove.Show();
            productMove.Activate();
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

        private void Receipt_Click(object sender, RoutedEventArgs e)
        {
            listView_Receipt.SelectionChanged += delegate { Mouse.Capture(listView_Receipt); };
            listView_Receipt.LostFocus += delegate { ReleaseMouseCapture(); listView_Receipt.UnselectAll(); };
        }

        private void PortNumberControl(object sender, TextCompositionEventArgs e)
        {
            TextBox input = (e.OriginalSource as TextBox);
            e.Handled = Utils.RegexCheckNumber(input.Text.Insert(input.CaretIndex, e.Text));
        }

    }
}
