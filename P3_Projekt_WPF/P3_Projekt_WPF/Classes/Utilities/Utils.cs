using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes.Database;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public static class Utils
    {
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

        public static void CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("https://google.dk/"))
                    {

                    }
                }
            }
            catch
            {
                throw new NoInternetConnectionException("Der er ingen forbindelse til internettet!");
            }
        }

        public static void FixReceiptInDatabase()
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
                NewReceipt.PaidPrice = price_of_all_transactions;
                NewReceipt.NumberOfProducts = TotalProductCount;
                NewReceipt.UpdateInDatabase();
            }
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
                        } else
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
    }
}
