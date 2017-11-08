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
using System.Windows.Controls;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        SettingsController _settingsController;
        StorageController _storageController;
        POSController _POSController;
        Grid productGrid = new Grid();
        private bool _ctrlDown = false;
        public MainWindow()
        {
            InitializeComponent();
            InitComponents();
            this.KeyDown += new KeyEventHandler(KeyboardHook);
            this.KeyDown += new KeyEventHandler(CtrlHookDown);
            this.KeyUp += new KeyEventHandler(CtrlHookUp);
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
            _storageController = new StorageController();
            _POSController = new POSController(_storageController);
            _settingsController = new SettingsController();
            InitGridQuickButtons();

            InitStorageGridProducts();
            AddProductButton();
            LoadProductGrid();
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
            Stopwatch TimeTester = new Stopwatch();
            TimeTester.Start();
            _storageController.GetAll();
            while (!_storageController.ThreadsDone)
            {
                Thread.Sleep(100);
            }
            TimeTester.Stop();
            Debug.WriteLine("[P3] Det tog " + TimeTester.ElapsedMilliseconds + "ms at hente alt fra databasen");
        }

        public void AddProductButton()
        {
            Button addProductButton = new Button();
            addProductButton.Content = "Tilføj nyt produkt";
            addProductButton.FontSize = 30;

            addProductButton.SetValue(Grid.RowProperty, 0);
            addProductButton.SetValue(Grid.ColumnProperty, 0);
            addProductButton.Style = FindResource("Flat_Button") as Style;
            addProductButton.Margin = new System.Windows.Thickness(2);
            addProductButton.Background = System.Windows.Media.Brushes.Transparent;

            addProductButton.Click += AddProductDialogOpener;

            productGrid.Children.Add(addProductButton);
        }

        public void AddProductDialogOpener(object sender, RoutedEventArgs e)
        {
            CreateProduct addProductWindow = new CreateProduct();
            foreach (Group group in _storageController.GroupDictionary.Values)
            {
                addProductWindow.comboBox_Group.Items.Add(group.Name);
            }
            foreach (string brand in _storageController.ProductDictionary.Values.Select(x => x.Brand).Distinct())
            {
                addProductWindow.comboBox_Brand.Items.Add(brand);
            }
            addProductWindow.output_ProductID.Text = Product.GetNextID().ToString();
            addProductWindow.btn_SaveAndQuit.Click += delegate {
                AddProduct(addProductWindow.textbox_Name.Text,
                           addProductWindow.comboBox_Brand.Text,
                           addProductWindow.comboBox_Group.Text,
                           addProductWindow.textbox_PurchasePrice.Text,
                           addProductWindow.textbox_SalePrice.Text,
                           addProductWindow.textbox_DiscountPrice.Text,
                           addProductWindow.textbox_Amount.Text,
                           addProductWindow.ChosenFilePath);
                addProductWindow.Close();
            };
            addProductWindow.Show();
        }

        
        public void AddProduct(string name, string brand, string group, string purchasePrice, string salePrice, string discountPrice, string amount, string filePath)
        {
            testtesttest.Text = $"Name: {name}, brand: {brand}, group: {group}, purchaseprice: {purchasePrice}, saleprice: {salePrice}, discountprice: {discountPrice}, amount: {amount}";

            Product product = new Product(name, brand, Decimal.Parse(purchasePrice), _storageController.GroupDictionary.First(x => x.Value.Name.ToLower() == group).Key, (discountPrice != null) ? true : false, Decimal.Parse(salePrice), Decimal.Parse(discountPrice), filePath);

            _storageController.ProductDictionary.TryAdd(product.ID, product);
            product.UploadToDatabase();
        }
        

        private void ShowSpecificInfoProductStorage(object sender, RoutedEventArgs e)
        {
            Debug.Print((sender as Button).Tag.ToString());
            Product placeholder = _storageController.ProductDictionary[Convert.ToInt32((sender as Button).Tag)];

            image_ChosenProduct = placeholder.Image;
            textBlock_ChosenProduct.Text = $"ID: {placeholder.ID}\nNavn: {placeholder.Name}\nGruppe: {_storageController.GroupDictionary[placeholder.ProductGroupID].Name}\nMærke: {placeholder.Brand}\nPris: {placeholder.SalePrice}\nTilbudspris: {placeholder.DiscountPrice}\nIndkøbspris: {placeholder.PurchasePrice}\nLagerstatus:";
            foreach (KeyValuePair<int, int> storageWithAmount in placeholder.StorageWithAmount)
            {
                textBlock_ChosenProduct.Text += $"\n  - {_storageController.StorageRoomDictionary[storageWithAmount.Key].Name} har {storageWithAmount.Value} stk.";
            }
        }

        public void StorageTabClick(object sender, RoutedEventArgs e)
        {
            //LoadProductGrid();
        }

        public void LoadProductGrid()
        {
            int i = 1;

            foreach (KeyValuePair<int, Product> product in _storageController.ProductDictionary.OrderBy(x => x.Key))
            {
                if (i % 5 == 0)
                {
                    productGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(380) });
                    int hej = productGrid.RowDefinitions.Count;
                }

                Product placeProduct = product.Value;



                Debug.Print($@"{_settingsController.PictureFilePath}/{product.Value.ID}.png");
                try
                {
                    var image = new Image();
                    image.Source = new BitmapImage(new Uri($@"{_settingsController.PictureFilePath}/{product.Value.ID}.png", UriKind.RelativeOrAbsolute));
                    image.Stretch = Stretch.Uniform;
                    placeProduct.Image = image;
                }
                catch
                {
                
                }



                ProductControl productControl = new ProductControl(placeProduct, _storageController.GroupDictionary);
                productControl.SetValue(Grid.ColumnProperty, i % 5);
                productControl.SetValue(Grid.RowProperty, i / 5);
                productControl.btn_ShowMoreInformation.Tag = product.Value.ID;
                productControl.btn_ShowMoreInformation.Click += ShowSpecificInfoProductStorage;
                productGrid.Children.Add(productControl);

                i++;
            }
        }

        public void AddTransactionToReceipt(SaleTransaction transaction)
        {
            listView_Receipt.Items.Add(new ReceiptListItem { String_Product = transaction.GetProductName(), Amount = transaction.Amount, Price = $"{transaction.GetProductPrice()},-", IDTag = transaction.Product.ID });

        }

        private void btn_MobilePay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Increment_Click(object sender, RoutedEventArgs e)
        {
            int productID = Convert.ToInt32((sender as Button).Tag);
            _POSController.PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().Amount++;
            UpdateReceiptList();
        }

        private void btn_Decrement_Click(object sender, RoutedEventArgs e)
        {
            int productID = Convert.ToInt32((sender as Button).Tag);
            _POSController.PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().Amount--;
            UpdateReceiptList();

        }

        private void btn_DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            int productID = Convert.ToInt32((sender as Button).Tag);
            _POSController.PlacerholderReceipt.RemoveTransaction(productID);
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

            if (_POSController.GetProductFromID(inputInt) != null)
            {
                _POSController.AddSaleTransaction(_POSController.GetProductFromID(int.Parse(textBox_AddProductID.Text)), int.Parse(textBox_ProductAmount.Text));
                UpdateReceiptList();
            }
            else
            {
                MessageBox.Show($"Produkt med ID {inputInt} findes ikke på lageret");
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

            if (_POSController.GetProductFromID(inputInt) != null && !_settingsController.quickButtonList.Any(x => x.ProductID == inputInt))
            {
                _settingsController.AddNewQuickButton(textBox_CreateQuickBtnName.Text, inputInt, grid_QuickButton.Width, grid_QuickButton.Height, btn_FastButton_click);
                listView_QuickBtn.Items.Add(new FastButton() { Button_Name = textBox_CreateQuickBtnName.Text, ProductID = inputInt });
            }
            else if (_settingsController.quickButtonList.Any(x => x.ProductID == inputInt))
            {
                MessageBox.Show($"Produkt med dette ID {inputInt} er allerede oprettet");
            }
            else
            {
                MessageBox.Show($"Produkt med ID {inputInt} findes ikke på lageret");
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

        private void btn_Temporary_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_PictureFilePath_Click(object sender, RoutedEventArgs e)
        {
            _settingsController.SpecifyPictureFilePath();
        }

        private void TextInputNoNumber(object sender, TextCompositionEventArgs e)
        {
            // Only allows number in textfield
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void TextInputNoNumberWithComma(object sender, TextCompositionEventArgs e)
        {
            // Only allows number in textfield, also with comma
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && !(e.Text[e.Text.Length - 1] == ','))
                e.Handled = true;
        }


    }
}
