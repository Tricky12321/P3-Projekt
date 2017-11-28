using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes.Database;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace P3_Projekt_WPF.Classes.Utilities
{
    public static class Utils
    {
        public static ImageSource NoImage = Utils.ImageSourceForBitmap(Properties.Resources.questionmark_png);

        public static void ShowErrorWarning(string text)
        {
            MessageBox.Show(text);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static int GetUnixTime(DateTime Tid)
        {
            int unixTimestamp = (int)(Tid.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static bool CheckInternetConnection()
        {
            Ping ping = new Ping();
            PingReply pingStatus = ping.Send(IPAddress.Parse("172.217.19.195"));
            if (pingStatus.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void _FixReceiptInDatabase()
        {
            string sql = "SELECT * FROM `receipt`";
            TableDecode receipts = Mysql.RunQueryWithReturn(sql);
            foreach (var receipt in receipts.RowData)
            {
                Receipt NewReceipt = new Receipt(receipt);
                decimal price_of_all_transactions = 0;
                int TotalProductCount = 0;
                foreach (var transaction in NewReceipt.Transactions)
                {
                    price_of_all_transactions += transaction.TotalPrice;
                    TotalProductCount += transaction.Amount;
                }
                NewReceipt.TotalPrice = price_of_all_transactions;
                //NewReceipt.PaidPrice = price_of_all_transactions;
                NewReceipt.NumberOfProducts = TotalProductCount;
                NewReceipt.UpdateInDatabase();
            }
        }

        public static void FixReceiptInDatabase()
        {
            Thread FixReceiptThread = new Thread(new ThreadStart(_FixReceiptInDatabase));
            FixReceiptThread.Name = "FixReceiptThread";
        }

        public static void GenerateSaleTransactions()
        {
            StorageController storageController = new StorageController();
            Random rng = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 4; i <= 15; i++)
            {
                Receipt TestReceipt = new Receipt(i);
                int max_prod = rng.Next(4, 20);
                for (int k = 2; k < max_prod; k++)
                {
                    int range_min = 2;
                    int range_max = 30;
                    int count_min = 1;
                    int count_max = 10;
                    Product TestProduct;
                    do
                    {
                        int random_num = rng.Next(range_min, range_max);
                        if (storageController.ProductDictionary.ContainsKey(random_num))
                        {
                            TestProduct = storageController.ProductDictionary[random_num];
                        }
                        else
                        {
                            TestProduct = null;
                        }
                    } while (TestProduct == null);
                    SaleTransaction NewSale = new SaleTransaction(TestProduct, rng.Next(count_min, count_max), i);
                    NewSale.UploadToDatabase();
                    TestReceipt.Transactions.Add(NewSale);
                }
                TestReceipt.UpdateInDatabase();
            }
            FixReceiptInDatabase();
        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static void FixStorageStatus()
        {
            string sql = "DELETE FROM `storage_status` WHERE `amount` <= '0'";
            Mysql.RunQuery(sql);
        }

        public static void GetIceCreameID()
        {
            if (Mysql.ConnectionWorking)
            {
                string sql = "SELECT `id` FROM `groups` WHERE `name` LIKE 'is'";
                TableDecode Results = Mysql.RunQueryWithReturn(sql);
                if (Results.RowCounter == 0)
                {
                    Properties.Settings.Default.IcecreamID = -1;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    int IsID = Convert.ToInt32(Results.RowData[0].Values[0]);
                    Properties.Settings.Default.IcecreamID = IsID;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public static bool RegexCheckDecimal(string input)
        {
            Regex reg = new Regex(@"^((\d+)(,{0,1})(\d{0,2}))$");
            return !reg.IsMatch(input);
        }
        public static bool RegexCheckNumber(string input)
        {
            Regex reg = new Regex(@"^(\d+)$");
            return !reg.IsMatch(input);
        }

        public static void LoadDatabaseSettings(MainWindow MainWin)
        {
            var sett = Properties.Settings.Default;
            bool Local = false;
            bool Remote = false;
            if (sett.local_or_remote == true)
            {
                MainWin.btn_RmtLcl.Content = "Remote";
                Local = true;
            }
            else
            {
                MainWin.btn_RmtLcl.Content = "Local";
                Remote = true;
            }
            UpdateDisabledFields(Local, Remote, MainWin);
            LoadDBSettingsData(MainWin);
        }

        public static void UpdateDisabledFields(bool Local, bool Remote, MainWindow MainWin)
        {
            SolidColorBrush BaseColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(229), Convert.ToByte(229), Convert.ToByte(229)));
            SolidColorBrush DiabledColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(119), Convert.ToByte(119), Convert.ToByte(119)));
            // LOCAL STARTS HERE
            MainWin.cmb_lcl_db.IsEnabled = Local;
            MainWin.cmb_lcl_ip.IsEnabled = Local;
            MainWin.cmb_lcl_port.IsEnabled = Local;
            MainWin.txt_lcl_password.IsEnabled = Local;
            MainWin.txt_lcl_username.IsEnabled = Local;
            MainWin.btn_lcl_saveDBSettings.IsEnabled = Local;

            if (Local)
            {
                MainWin.GroupLocal.Background = BaseColor;
            }
            else
            {
                MainWin.GroupLocal.Background = DiabledColor;
            }
            // REMOTE STARTS HERE
            MainWin.cmb_rmt_db.IsEnabled = Remote;
            MainWin.cmb_rmt_ip.IsEnabled = Remote;
            MainWin.cmb_rmt_port.IsEnabled = Remote;
            MainWin.txt_rmt_password.IsEnabled = Remote;
            MainWin.txt_rmt_username.IsEnabled = Remote;
            MainWin.btn_rmt_saveDBSettings.IsEnabled = Remote;
            if (Remote)
            {
                MainWin.GroupRemote.Background = BaseColor;
            }
            else
            {
                MainWin.GroupRemote.Background = DiabledColor;
            }
        }

        public static void FlipRemoteLocal(MainWindow MainWin)
        {
            var sett = Properties.Settings.Default;
            sett.local_or_remote = !sett.local_or_remote;
            sett.Save();
            LoadDatabaseSettings(MainWin);
        }


        public static void SaveDBData(MainWindow MainWin)
        {
            var sett = Properties.Settings.Default;
            sett.lcl_db = MainWin.cmb_lcl_db.Text;
            sett.lcl_ip = MainWin.cmb_lcl_ip.Text;
            sett.lcl_port = Convert.ToInt32(MainWin.cmb_lcl_port.Text);
            sett.lcl_password = MainWin.txt_lcl_password.Password;
            sett.lcl_username = MainWin.txt_lcl_username.Text;
            sett.rmt_db = MainWin.cmb_rmt_db.Text;
            sett.rmt_ip = MainWin.cmb_rmt_ip.Text;
            sett.rmt_port = Convert.ToInt32(MainWin.cmb_rmt_port.Text);
            sett.rmt_password = MainWin.txt_rmt_password.Password;
            sett.rmt_username = MainWin.txt_rmt_username.Text;
            sett.Save();
        }

        public static void LoadDBSettingsData(MainWindow MainWin)
        {
            var sett = Properties.Settings.Default;

            MainWin.cmb_rmt_db.Text = sett.rmt_db;
            MainWin.cmb_rmt_ip.Text = sett.rmt_ip;
            MainWin.cmb_rmt_port.Text = sett.rmt_port.ToString();
            MainWin.txt_rmt_password.Password = sett.rmt_password;
            MainWin.txt_rmt_username.Text = sett.rmt_username;
            MainWin.cmb_lcl_db.Text = sett.lcl_db;
            MainWin.cmb_lcl_ip.Text = sett.lcl_ip;
            MainWin.cmb_lcl_port.Text = sett.lcl_port.ToString();
            MainWin.txt_lcl_password.Password = sett.lcl_password;
            MainWin.txt_lcl_username.Text = sett.lcl_username;
        }

        #region SearchAlgorithm

        static List<SearchProduct> weigthedSearchList;
        public static ConcurrentDictionary<int, SearchProduct> SearchForProduct(string searchString, ConcurrentDictionary<int, Product> productDictionary, ConcurrentDictionary<int, Group> groupDictionary)
        {
            weigthedSearchList = new List<SearchProduct>();
            ConcurrentDictionary<int, SearchProduct> productsToReturn = new ConcurrentDictionary<int, SearchProduct>();
            int isNumber;

            if (int.TryParse(searchString, out isNumber))
            {
                if (productDictionary.Keys.Contains(isNumber))
                {
                    SearchProduct matchedProduct = new SearchProduct(productDictionary[isNumber]);
                    matchedProduct.NameMatch = 100;
                    productsToReturn.TryAdd(isNumber, matchedProduct);
                }
            }

            foreach (Product product in productDictionary.Values)
            {
                ProductSearch(searchString, product);
                GroupSearch(searchString, product, groupDictionary);
                BrandSearch(searchString, product);
            }
            TakeProductsToReturnFromWeightedList(ref productsToReturn);

            return productsToReturn;
        }

        private static void TakeProductsToReturnFromWeightedList(ref ConcurrentDictionary<int, SearchProduct> productsToReturn)
        {
            var searchList = weigthedSearchList.Where(x => (x.BrandMatch + x.GroupMatch + x.NameMatch) > 0);
            foreach (SearchProduct searchproduct in searchList)
            {
                productsToReturn.TryAdd(searchproduct.CurrentProduct.ID, searchproduct);
            }
        }

        private static void ProductSearch(string searchStringElement, Product productToConvert)
        {
            SearchProduct productToAdd = new SearchProduct(productToConvert);

            string[] searchSplit = searchStringElement.Split(' ');
            string[] productSplit = productToConvert.Name.Split(' ');

            foreach (string s in searchSplit)
            {
                foreach (string t in productSplit)
                {
                    if (LevenstheinProductSearch(s, t))
                    {
                        productToAdd.NameMatch += 1;
                    }
                }
            }
            weigthedSearchList.Add(productToAdd);
        }

        private static bool LevenshteinsGroupAndProductSearch(string[] searchedString, string stringToCompare, out int charDifference)
        {//setup for levenshteins
            string[] compareSplit = stringToCompare.Split(' ');

            foreach (string sString in searchedString)
            {
                foreach (string compareString in compareSplit)
                {
                    //getting the chardifference between the searchedstring and the productname
                    charDifference = ComputeLevenshteinsDistance(sString.ToLower(), compareString.ToLower());
                    //Evaluate if the chardifference is in between the changelimit of the string
                    if (EvaluateStringLimit(sString.Length, charDifference))
                    {
                        //only returns true of it is matching
                        return true;
                    }
                }

            }
            charDifference = -1;
            return false;
        }


        private static void GroupSearch(string searchString, Product product, ConcurrentDictionary<int, Group> groupDictionary)
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchString.Split(' ');
            //matching on each element in the string
            int MatchedValue;
            //if the string contains a name of a group, or the string is matched, each product with the same group 
            //is added to the list of products to show.
            if (LevenshteinsGroupAndProductSearch(dividedString, groupDictionary[product.ProductGroupID].Name, out MatchedValue))
            {
                if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).Count() > 0)
                {
                    if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First() != null)
                    {
                        weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().GroupMatch += 1;
                    }
                }
            }

        }

        private static void BrandSearch(string searchString, Product product)
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchString.Split(' ');
            //checking all products to to match the brands with the searched string elements
            int MatchedValues;
            //matching on each element in the string
            //if the string contains a product brand, or the string is matched, each product with the same brand
            //is added to the list of products to show.
            if (LevenshteinsGroupAndProductSearch(dividedString, product.Brand, out MatchedValues))
            {
                if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).Count() > 0)
                {
                    if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First() != null)
                    {
                        weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().BrandMatch += 1;
                    }
                }
            }

        }

        private static bool LevenstheinProductSearch(string searchEle, string ProductEle)
        {
            int charDifference = ComputeLevenshteinsDistance(searchEle.ToLower(), ProductEle.ToLower());
            return EvaluateStringLimit(searchEle.Length, charDifference);
        }

        private static bool EvaluateStringLimit(int searchedStringLength, int charDiff)
        {
            int limitOfChanges;

            // Determines how many changes is allowed, depending on how long the string is
            if (searchedStringLength < 3)
                limitOfChanges = 0;
            else if (searchedStringLength < 5)
                limitOfChanges = 1;
            else if (searchedStringLength < 8)
                limitOfChanges = 2;
            else if (searchedStringLength < 12)
                limitOfChanges = 3;
            else
            {
                limitOfChanges = 4;
            }

            return limitOfChanges >= charDiff;
        }

        private static int ComputeLevenshteinsDistance(string searchedString, string productToCompare)
        {
            //searchString Length
            int searchStringLength = searchedString.Length;
            //productname Length       
            int productNameLength = productToCompare.Length;
            int cost;
            int minimum1, minimum2, minimum3;
            //size of int array
            int[,] d = new int[searchStringLength + 1, productNameLength + 1];
            //--------------
            //Checks if the strings are empty
            if (string.IsNullOrEmpty(searchedString))
            {
                if (!string.IsNullOrEmpty(productToCompare))
                {
                    return searchStringLength;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(productToCompare))
            {
                if (!string.IsNullOrEmpty(searchedString))
                {
                    return productNameLength;
                }
                return 0;
            }
            //-------------
            //GetUpperBound gets the index of the last element in the array.
            for (int i = 0; i <= d.GetUpperBound(0); ++i)
            {
                d[i, 0] = i;
            }
            //(0) and (1) is to differentiate between the first and second element in the array.
            for (int i = 0; i <= d.GetUpperBound(1); ++i)
            {
                d[0, i] = i;
            }

            for (int i = 1; i <= d.GetUpperBound(0); ++i)
            {
                for (int j = 1; j <= d.GetUpperBound(1); ++j)
                {
                    //will convert a boolean to int, depending if a char is different different between the two strings.
                    cost = Convert.ToInt32(!(searchedString[i - 1] == productToCompare[j - 1]));


                    minimum1 = d[i - 1, j] + 1;          //takes the element in the previous row i
                    minimum2 = d[i, j - 1] + 1;          //takes the element in the previous column j
                    minimum3 = d[i - 1, j - 1] + cost;   //takes the element in the previous column j and previos row i, and adds the cost of changing a char, +1 og or +0
                                                         //the minmum of the 3 will be put into the 2-dimensial array at row i column j
                    d[i, j] = Math.Min(Math.Min(minimum1, minimum2), minimum3);

                    //for a array example, see step 1-7 https://people.cs.pitt.edu/~kirk/cs1501/Pruhs/Spring2006/assignments/editdistance/Levenshtein%20Distance.htm
                }
            }
            //returns the value of the last coloumn and last row of the array, which is the amount that is needed to change between the words.
            var test = d[d.GetUpperBound(0), d.GetUpperBound(1)];

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

            #endregion
        }

    }
}
