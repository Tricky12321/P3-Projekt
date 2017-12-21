using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public class StatisticsController
    {
        public List<SaleTransaction> TransactionsForStatistics = new List<SaleTransaction>();
        public List<Receipt> TodayReceipts = new List<Receipt>();
        public Dictionary<int, decimal> SalesPerGroup;
        public Dictionary<string, decimal> SalesPerBrand;
        public int ReceiptTotalCount;
        public decimal AverageProductPerReceipt;
        public decimal ReceiptTotalPrice;
        public decimal TotalRevenueToday;
        public Dictionary<int, StatisticsProduct> SalesPerProduct;
        public decimal[] Payments = { 0, 0, 0 };

        private bool _allStatisticsDone = false;
        private ConcurrentQueue<SaleTransaction> _saleTransactions;
        private List<Thread> _saleTransactionThreads = new List<Thread>();
        private int _threadCount = Utils.NumberOfCores;
        private int _saleTransactionsCreated = 0;
        private ConcurrentQueue<Row> _dataQueue;
        private StorageController _storageController = null;

        public StatisticsController(StorageController storageController)
        {
            _storageController = storageController;
        }

        private void CreateThreads()
        {
            for (int i = 0; i < _threadCount - 1; i++)
            {
                Thread NewThread = new Thread(new ThreadStart(ThreadWork));
                NewThread.Name = "Statistics Controller Thread";
                NewThread.Start();
                _saleTransactionThreads.Add(NewThread);
            }
        }

        private void ThreadWork()
        {
            Row data;
            while (_dataQueue.TryDequeue(out data))
            {
                int productID = Convert.ToInt32(data.Values[1]);
                if (_storageController.AllProductsDictionary.ContainsKey(productID))
                {
                    SaleTransaction NewTransaction = new SaleTransaction(data, _storageController);
                    _saleTransactions.Enqueue(NewTransaction);
                }
                Interlocked.Increment(ref _saleTransactionsCreated);
            }
        }

        public void RequestStatistics(bool searchProduct, int productToSearch, bool searchGroup, int groupToSearch, bool searchBrand, string brandToSearch, DateTime from, DateTime to)
        {
            TransactionsForStatistics = new List<SaleTransaction>();
            string productString = GetProductsQueryString(searchProduct, productToSearch, searchGroup, groupToSearch, searchBrand, brandToSearch, from, to);
            RequestTransactionsDatabase(productString);
            if (!searchBrand)
            {
                string serviceProductString = GetServiceProductsQueryString(searchProduct, productToSearch, searchGroup, groupToSearch, from, to);
                RequestTransactionsDatabase(serviceProductString);
            }
            string tempProductString;
            if (searchGroup || searchBrand)
            {
                tempProductString = GetTempProductsQueryString(searchGroup, groupToSearch, searchBrand, brandToSearch, from, to);
            }
            else
            {
                tempProductString = GetTempProductsQueryString(searchProduct, productToSearch, from, to);
            }
            RequestTransactionsDatabase(tempProductString);
        }

        public string GetProductsQueryString(bool searchProduct, int productToSearch, bool searchGroup, int groupToSearch, bool searchBrand, string brandToSearch, DateTime from, DateTime to)
        {
            StringBuilder NewString = new StringBuilder("SELECT `sale_transactions`.* " +
                "FROM `products`, `sale_transactions` WHERE `sale_transactions`.`product_id` = `products`.`id`" +
                " AND `sale_transactions`.`product_type` = 'product'" +
                $" AND UNIX_TIMESTAMP(`datetime`) >= '{Utils.GetUnixTime(from)}' AND UNIX_TIMESTAMP(`datetime`) <= '{Utils.GetUnixTime(EndDate(to))}'");
            if (searchProduct)
            {
                NewString.Append($" AND `products`.`id` = '{productToSearch}'");
            }
            else
            {
                if (searchGroup)
                {
                    NewString.Append($" AND `products`.`groups` = '{groupToSearch}'");
                }

                if (searchBrand)
                {
                    NewString.Append($" AND `products`.`brand` = '{brandToSearch}'");
                }
            }
            return NewString.ToString();
        }

        public string GetServiceProductsQueryString(bool searchID, int idToSearch, bool searchGroup, int groupToSearch, DateTime from, DateTime to)
        {
            StringBuilder NewString = new StringBuilder("SELECT `sale_transactions`.* " +
                "FROM `service_products`, `sale_transactions` WHERE `sale_transactions`.`product_id` = `service_products`.`id`" +
                " AND `sale_transactions`.`product_type` = 'service_product'" +
                $" AND UNIX_TIMESTAMP(`datetime`) >= '{Utils.GetUnixTime(from)}' AND UNIX_TIMESTAMP(`datetime`) <= '{Utils.GetUnixTime(EndDate(to))}'");
            if (searchID)
            {
                NewString.Append($" AND `service_products`.`id` = '{idToSearch}'");
            }
            else
            {
                if (searchGroup)
                {
                    NewString.Append($" AND `service_products`.`groups` = '{groupToSearch}'");
                }
            }
            return NewString.ToString();
        }

        public string GetTempProductsQueryString(bool searchGroup, int groupToSearch, bool searchBrand, string brandToSearch, DateTime from, DateTime to)
        {
            StringBuilder NewString = new StringBuilder();
            NewString.Append("SELECT `sale_transactions`.*, `products`.`id` as 'PRODID',`products`.`groups`, `products`.`brand`, `temp_products`.`id` as 'TEMPID' " +
                "FROM `sale_transactions`, `temp_products`, `products` " +
                " WHERE " +
                "(`sale_transactions`.`product_type` = 'temp_product') AND " +
                "(`products`.`id` = `temp_products`.`resolved_product_id`)" +
                $" AND UNIX_TIMESTAMP(`datetime`) >= '{Utils.GetUnixTime(from)}'" +
                $" AND UNIX_TIMESTAMP(`datetime`) <= '{Utils.GetUnixTime(EndDate(to))}'");
            if (searchGroup)
            {
                NewString.Append($" AND `products`.`groups` = '{groupToSearch}'");
            }

            if (searchBrand)
            {
                NewString.Append($" AND `products`.`brand` = '{brandToSearch}'");
            }
            return NewString.ToString();
        }

        public string GetTempProductsQueryString(bool searchProduct, int productToSearch, DateTime from, DateTime to)
        {
            StringBuilder NewString = new StringBuilder("SELECT `sale_transactions`.* " +
                "FROM `temp_products`, `sale_transactions` WHERE" +
                " `sale_transactions`.`product_type` = 'temp_product'" +
                $" AND UNIX_TIMESTAMP(`datetime`) >= '{Utils.GetUnixTime(from)}' AND UNIX_TIMESTAMP(`datetime`) <= '{Utils.GetUnixTime(EndDate(to))}'");
            if (searchProduct)
            {
                NewString.Append($" AND `temp_products`.`resolved_product_id` = '{productToSearch}'");
            }
            else
            {
                NewString.Append($" AND `sale_transactions`.`product_id` = `temp_products`.`id`");
            }
            return NewString.ToString();
        }

        public DateTime EndDate(DateTime date)
        {
            TimeSpan dayEnd = new TimeSpan(23, 59, 59);
            return date + dayEnd;
        }

        public void RequestTransactionsDatabase(string queryString)
        {
            _saleTransactionsCreated = 0;
            _saleTransactions = new ConcurrentQueue<SaleTransaction>();
            _dataQueue = Mysql.RunQueryWithReturnQueue(queryString).RowData;
            int TransCount = _dataQueue.Count;
            CreateThreads();
            ThreadWork();
            while (!_dataQueue.IsEmpty && _saleTransactionsCreated == TransCount)
            {
                ThreadWork();
                Thread.Sleep(1);
            }

            _allStatisticsDone = true;
            TransactionsForStatistics = TransactionsForStatistics.Concat(_saleTransactions).ToList<SaleTransaction>();
        }

        public void AverageItemsPerReceipt(DateTime Start, DateTime End)
        {
            Int64 from = Utils.GetUnixTime(Start);
            Int64 to = Utils.GetUnixTime(EndDate(End));
            string sql = $"SELECT AVG(`number_of_products`) FROM `receipt` WHERE UNIX_TIMESTAMP(`datetime`) >= '{from}' AND UNIX_TIMESTAMP(`datetime`) <= '{to}'";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            AverageProductPerReceipt =  Convert.ToDecimal(Results.RowData[0].Values[0]);
        }

        public void RequestTodayReceipts()
        {
            TodayReceipts = new List<Receipt>();
            Int64 from = Utils.GetUnixTime(DateTime.Today);
            Int64 to = Utils.GetUnixTime(EndDate(DateTime.Today));
            string queryString = $"SELECT* FROM `receipt` WHERE UNIX_TIMESTAMP(`datetime`) >= '{from}' AND UNIX_TIMESTAMP(`datetime`) <= '{to}'";
            _dataQueue = Mysql.RunQueryWithReturnQueue(queryString).RowData;
            foreach (Row row in _dataQueue)
            {
                TodayReceipts.Add(new Receipt(row));
            }
        }

        public void CalculatePayments()
        {
            Payments = new decimal[] { 0, 0, 0 };
            foreach (Receipt receipt in TodayReceipts)
            {
                foreach (Payment payment in receipt.Payments)
                {
                    if ((int)payment.PaymentMethod == 1)
                    {
                        Payments[0] += payment.Amount;
                    }
                    else if ((int)payment.PaymentMethod == 2)
                    {
                        Payments[1] += payment.Amount;
                    }
                    else
                    {
                        Payments[2] += payment.Amount;
                    }
                }
                if (receipt.PaidPrice > receipt.TotalPrice)
                {
                    Payments[0] -= (receipt.PaidPrice - receipt.TotalPrice);
                }
            }
        }

        public void GetReceiptTotalPrice(DateTime From, DateTime To)
        {
            Int64 fromUnix = Utils.GetUnixTime(From);
            Int64 toUnix = Utils.GetUnixTime(EndDate(To));
            string sql = $"SELECT sum(`total_price`) as atotal FROM `receipt` WHERE UNIX_TIMESTAMP(`datetime`) >= '{fromUnix}' AND UNIX_TIMESTAMP(`datetime`) <= '{toUnix}'";
            if (!decimal.TryParse(Mysql.RunQueryWithReturn(sql).RowData[0].Values[0], out ReceiptTotalPrice))
            {
                ReceiptTotalPrice = 0;
            }
        }

        public void GetReceiptTotalCount(DateTime From, DateTime To)
        {
            Int64 fromUnix = Utils.GetUnixTime(From);
            Int64 toUnix = Utils.GetUnixTime(EndDate(To));
            string sql = $"SELECT `id` FROM `receipt` WHERE UNIX_TIMESTAMP(`datetime`) >= '{fromUnix}' AND UNIX_TIMESTAMP(`datetime`) <= '{toUnix}'";
            ReceiptTotalCount = Mysql.RunQueryWithReturn(sql).RowCounter;
        }

        public StatisticsListItem ReceiptStatisticsString()
        {
            if (ReceiptTotalCount > 0)
            {
                return new StatisticsListItem("", "Gennemsnitlig kvitterings størrelse", $"{ Math.Round(AverageProductPerReceipt, 2)}", $"{Math.Round(ReceiptTotalPrice / ReceiptTotalCount, 2)}");
            }
            return new StatisticsListItem("", "Gennemsnitlig kvitterings størrelse", "0", "0");
        }

        public void GenerateProductSalesAndTotalRevenue()
        {
            SalesPerProduct = new Dictionary<int, StatisticsProduct>();
            TotalRevenueToday = 0;
            foreach (SaleTransaction transaction in TransactionsForStatistics)
            {
                if (SalesPerProduct.ContainsKey(transaction.GetProductID()))
                {
                    SalesPerProduct[transaction.GetProductID()].Amount += transaction.Amount;
                    SalesPerProduct[transaction.GetProductID()].Revenue += transaction.TotalPrice;
                }
                else
                {
                    SalesPerProduct.Add(transaction.GetProductID(), new StatisticsProduct());
                    SalesPerProduct[transaction.GetProductID()].Amount += transaction.Amount;
                    SalesPerProduct[transaction.GetProductID()].Revenue += transaction.TotalPrice;
                }
                TotalRevenueToday += transaction.TotalPrice;
            }
        }

        public void GenerateGroupAndBrandSales()
        {
            SalesPerGroup = new Dictionary<int, decimal>();
            SalesPerBrand = new Dictionary<string, decimal>();
            foreach (Group group in _storageController.GroupDictionary.Values)
            {
                SalesPerGroup.Add(group.ID, 0m);
            }
            foreach (string brand in _storageController.ProductDictionary.Values.Select(x => x.Brand).Distinct())
            {
                SalesPerBrand.Add(brand, 0m);
            }
            GetGroupAndBrandSales();
        }

        private void GetGroupAndBrandSales()
        {
            foreach (SaleTransaction transaction in TransactionsForStatistics)
            {
                if (transaction.GetGroupID() >= 0)
                {
                    SalesPerGroup[transaction.GetGroupID()] += transaction.TotalPrice;
                }
                if (transaction.GetBrand() != "")
                {
                    SalesPerBrand[transaction.GetBrand()] += transaction.TotalPrice;
                }
            }
        }

        public StatisticsListItem GroupSalesStrings(int id, decimal totalPrice)
        {
            if (totalPrice > 0)
            {
                return new StatisticsListItem("", $"{_storageController.GroupDictionary[id].Name}", $"{Math.Round((SalesPerGroup[id] / totalPrice) * 100m, 1)}%", $"{Math.Round(SalesPerGroup[id], 2)}");
            }
            return new StatisticsListItem("", $"{_storageController.GroupDictionary[id].Name}", "0%", $"{Math.Round(SalesPerGroup[id], 2)}");
        }

        public StatisticsListItem BrandSalesStrings(string brand, decimal totalPrice)
        {
            if (totalPrice > 0)
            {
                return new StatisticsListItem("", $"{brand}", $"{Math.Round((SalesPerBrand[brand] / totalPrice) * 100m, 1)}%", $"{Math.Round(SalesPerBrand[brand], 2)}");
            }
            return new StatisticsListItem("", $"{brand}", "0%", $"{Math.Round(SalesPerBrand[brand], 2)}");
        }

        public StatisticsListItem ProductSalesStrings(int id, StatisticsProduct productInfo)
        {
            if (id == -1)
            {
                return new StatisticsListItem("", $"Midlertidigt produkt", $"{productInfo.Amount}", $"{Math.Round(productInfo.Revenue, 2)}");
            }
            return new StatisticsListItem("", $"{_storageController.AllProductsDictionary[id].GetName()}", $"{productInfo.Amount}", $"{Math.Round(productInfo.Revenue, 2)}");
        }
    }
}