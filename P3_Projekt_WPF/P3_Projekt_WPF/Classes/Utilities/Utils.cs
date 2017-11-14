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

namespace P3_Projekt_WPF.Classes.Utilities
{
    public static class Utils
    {
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

        #region SearchAlgorithm

        static List<SearchProduct> weigthedSearchList;
        public static ConcurrentDictionary<int,Product> SearchForProduct(string searchString, ConcurrentDictionary<int, Product> productDictionary, ConcurrentDictionary<int, Group> groupDictionary)
        {
            weigthedSearchList = new List<SearchProduct>();
            searchString.ToLower();
            ConcurrentDictionary<int,Product> productsToReturn = new ConcurrentDictionary<int, Product>();
            int isNumber;

            if (int.TryParse(searchString, out isNumber))
            {
                productsToReturn.TryAdd(isNumber,productDictionary[isNumber]);
            }

            foreach (Product product in productDictionary.Values)
            {
                ProductSearch(searchString, product);
                GroupSearch(searchString, productDictionary, groupDictionary);
                BrandSearch(searchString, productDictionary);
            }
            SortWeightedSearchList(ref productsToReturn);

            return productsToReturn;
        }

        private static void SortWeightedSearchList(ref ConcurrentDictionary<int,Product> productsToReturn)
        {
            weigthedSearchList.Sort();
            foreach(SearchProduct searchproduct in weigthedSearchList.TakeWhile(x => (x.BrandMatch + x.GroupMatch + x.NameMatch) > 0))
            {
                productsToReturn.TryAdd(searchproduct.CurrentProduct.ID, searchproduct.CurrentProduct);
            }
        }

        private static void ProductSearch(string searchStringElement, Product productToConvert)
        {
            SearchProduct productToAdd = new SearchProduct(productToConvert);

            string[] searchSplit = searchStringElement.Split(' ');
            string[] productSplit = productToConvert.Name.ToLower().Split(' ');

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

        private static bool LevenshteinsGroupAndProductSearch(string[] searchedString, string stringToCompare, out int charDifference)//tested
        {//setup for levenshteins
            string[] compareSplit = stringToCompare.Split(' ');

            foreach (string sString in searchedString)
            {
                foreach (string compareString in compareSplit)
                {
                    //getting the chardifference between the searchedstring and the productname
                    charDifference = ComputeLevenshteinsDistance(sString, compareString);
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


        private static void GroupSearch(string searchString, ConcurrentDictionary<int, Product> productDictionary, ConcurrentDictionary<int, Group> groupDictionary)//tested
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchString.ToLower().Split(' ');
            //checking all groups to to match with the searched string elements
            foreach (Group group in groupDictionary.Values)
            {
                //matching on each element in the string
                int MatchedValue;
                //if the string contains a name of a group, or the string is matched, each product with the same group 
                //is added to the list of products to show.
                if (LevenshteinsGroupAndProductSearch(dividedString, group.Name, out MatchedValue))
                {
                    foreach (Product product in productDictionary.Values.Where(x => x.ProductGroupID == group.ID))
                    {
                        if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).Count() > 0)
                        {
                            if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First() != null)
                            {
                                weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().GroupMatch = MatchedValue;
                            }
                        }
                    }
                }
            }
        }

        private static void BrandSearch(string searchString, ConcurrentDictionary<int, Product> productDictionary)//tested
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchString.ToLower().Split(' ');
            //checking all products to to match the brands with the searched string elements
            foreach (Product product in productDictionary.Values)
            {
                int MatchedValues;
                //matching on each element in the string
                //if the string contains a product brand, or the string is matched, each product with the same brand
                //is added to the list of products to show.
                if (LevenshteinsGroupAndProductSearch(dividedString, product.Brand, out MatchedValues))
                {
                    if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).Count() > 0)
                    {
                        if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First() != null) ;
                        {
                            weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().BrandMatch = MatchedValues;
                        }
                    }
                }
            }
        }

        private static bool LevenstheinProductSearch(string searchEle, string ProductEle)
        {
            int charDifference = ComputeLevenshteinsDistance(searchEle, ProductEle);
            return EvaluateStringLimit(searchEle.Length, charDifference);
        }

        private static bool EvaluateStringLimit(int searchedStringLength, int charDiff)//tested
        {
            int limitOfChanges;

            // Determines how many changes is allowed, depending on how long the string is
            if (searchedStringLength < 5)
                limitOfChanges = 1;
            else if (searchedStringLength < 10 && searchedStringLength >= 5)
                limitOfChanges = 3;
            else if (searchedStringLength >= 10 && searchedStringLength < 20)
                limitOfChanges = 6;
            else
                limitOfChanges = 9;

            return limitOfChanges >= charDiff;
        }

        private static int ComputeLevenshteinsDistance(string searchedString, string productToCompare)//tested
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
            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

            #endregion
        }
    }
}
