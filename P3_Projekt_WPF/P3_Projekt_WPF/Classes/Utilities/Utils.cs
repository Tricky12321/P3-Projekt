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
        public static int NumberOfCores => Convert.ToInt32(System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
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
                pingStatus = ping.Send(IPAddress.Parse(Properties.Settings.Default.lcl_ip));
                if (pingStatus.Status == IPStatus.Success)
                {
                    return true;
                }
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

        //TODO
        //Wtf
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

        public static void GetIceCreamID()
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
            else
            {
                Properties.Settings.Default.IcecreamID = -1;
                Properties.Settings.Default.Save();
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

        /*public static void UpdateDisabledFields(bool Local, bool Remote, MainWindow MainWin)
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
            //MainWin.btn_rmt_saveDBSettings.IsEnabled = Remote;
            if (Remote)
            {
                MainWin.GroupRemote.Background = BaseColor;
            }
            else
            {
                MainWin.GroupRemote.Background = DiabledColor;
            }
        }*/      

    }
}
