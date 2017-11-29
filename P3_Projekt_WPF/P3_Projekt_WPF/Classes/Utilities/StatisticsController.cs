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
        public decimal ReceiptTotalPrice;
        public Dictionary<int, KeyValuePair<int, decimal>> SalesPerProduct;
        public decimal[] Payments = { 0, 0, 0 };

        public StatisticsController(StorageController storageController)
        {
            _storageController = storageController;
        }

        /* Man anmoder altid om dato først, men kun én af dato-metoderne af gangen.
         * Hver dato-metode nulstiller TransactionsForStatistics ved at hente transaktioner ned fra databasen alt efter datoen
         * Derefter skal formen kalde RequestStatisticsWithParameters */



        private bool _allStatisticsDone = false;
        private ConcurrentQueue<SaleTransaction> _saleTransactions;
        private List<Thread> _saleTransactionThreads = new List<Thread>();
        private int _threadCount = 4;
        private int _saleTransactionsCreated = 0;
        private ConcurrentQueue<Row> _dataQueue;
        private StorageController _storageController = null;

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
                SaleTransaction NewTransaction = new SaleTransaction(data, _storageController);
                _saleTransactions.Enqueue(NewTransaction);
            }
        }

        public string GetProductsQueryString(bool searchID, int idToSearch, bool searchGroup, int groupToSearch, bool searchBrand, string brandToSearch, DateTime from, DateTime to)
        {
            StringBuilder NewString = new StringBuilder("SELECT `sale_transactions`.* " +
                "FROM `products`, `sale_transactions` WHERE `sale_transactions`.`product_id` = `products`.`id`" +
                $" AND UNIX_TIMESTAMP(`datetime`) >= '{Utils.GetUnixTime(from)}' AND UNIX_TIMESTAMP(`datetime`) <= '{Utils.GetUnixTime(EndDate(to))}'");
            if (searchID)
            {
                NewString.Append($" AND `products`.`id` = '{idToSearch}'");
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

        public DateTime EndDate(DateTime date)
        {
            TimeSpan dayEnd = new TimeSpan(23, 59, 59);
            return date + dayEnd;
        }

        public void RequestStatisticsDate(string queryString)
        {
            Stopwatch Timer1 = new Stopwatch();
            Timer1.Start();
            _saleTransactionsCreated = 0;
            _saleTransactions = new ConcurrentQueue<SaleTransaction>();
            _dataQueue = Mysql.RunQueryWithReturnQueue(queryString).RowData;
            int TransCount = _dataQueue.Count;
            CreateThreads();
            ThreadWork();
            while (!_dataQueue.IsEmpty && _saleTransactions.Count() == TransCount)
            {
                Thread.Sleep(1);
            }

            _allStatisticsDone = true;
            TransactionsForStatistics = TransactionsForStatistics.Concat(_saleTransactions).ToList<SaleTransaction>();
            Timer1.Stop();
            Debug.WriteLine("[StatisticsController] took " + Timer1.ElapsedMilliseconds + "ms to fetch");
        }

        public void RequestTodayReceipts()
        {
            TodayReceipts = new List<Receipt>();
            Int64 from = Utils.GetUnixTime(DateTime.Today);
            Int64 to = Utils.GetUnixTime(EndDate(DateTime.Today));
            string queryString = $"SELECT* FROM `receipt` WHERE UNIX_TIMESTAMP(`datetime`) >= '{from}' AND UNIX_TIMESTAMP(`datetime`) <= '{to}'";
            _dataQueue = Mysql.RunQueryWithReturnQueue(queryString).RowData;
            foreach(Row row in _dataQueue)
            {
                TodayReceipts.Add(new Receipt(row));
            }
        }

        public void CalculatePayments()
        {
            Payments = new decimal[] { 0, 0, 0 };
            foreach (Receipt receipt in TodayReceipts)
            {
                foreach(Payment payment in receipt.Payments)
                {
                    if ((int)payment.PaymentMethod == 2)
                    {
                        Payments[0] += payment.Amount;
                    }
                    else if ((int)payment.PaymentMethod == 3)
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
            if(!decimal.TryParse(Mysql.RunQueryWithReturn(sql).RowData[0].Values[0], out ReceiptTotalPrice))
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
            if(ReceiptTotalCount > 0)
            {
                return new StatisticsListItem("", "Gennemsnitlig kvitteringspris", $"{ ReceiptTotalCount}", $"{Math.Round(ReceiptTotalPrice / ReceiptTotalCount, 2)}");
            }
            return new StatisticsListItem("", "Gennemsnitlig kvitteringspris", "0", "0");
        }

        /*public void GenerateProductSales()
        {
        SalesPerProduct = new Dictionary<int, KeyValuePair<int, decimal>>();
        foreach (SaleTransaction transaction in TransactionsForStatistics)
            {
                if (SalesPerProduct.ContainsKey(transaction.GetProductID()))
                {
                }
            }
        
        }*/

        public void GenerateGroupAndBrandSales()
        {
            SalesPerGroup = new Dictionary<int, decimal>();
            SalesPerBrand = new Dictionary<string, decimal>();
            foreach (Group group in _storageController.GroupDictionary.Values)
            {
                SalesPerGroup.Add(group.ID, 0m);
            }
            foreach(string brand in _storageController.ProductDictionary.Values.Select(x => x.Brand).Distinct())
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
                if(transaction.GetBrand() != "")
                {
                    SalesPerBrand[transaction.GetBrand()] += transaction.TotalPrice;
                }
            }
        }

        public StatisticsListItem GroupSalesStrings(int id, decimal totalPrice)
        {
            if(totalPrice > 0)
            {
                return new StatisticsListItem("", $"{_storageController.GroupDictionary[id].Name}", $"{Math.Round((SalesPerGroup[id] / totalPrice) * 100m, 1)}%", $"{SalesPerGroup[id]}");
            }
            return new StatisticsListItem("", $"{_storageController.GroupDictionary[id].Name}", "0%", $"{SalesPerGroup[id]}");
        }

        public StatisticsListItem BrandSalesStrings(string brand, decimal totalPrice)
        {
            if (totalPrice > 0)
            {
                return new StatisticsListItem("", $"{brand}", $"{Math.Round((SalesPerBrand[brand] / totalPrice) * 100m, 1)}%", $"{SalesPerBrand[brand]}");
            }
            return new StatisticsListItem("", $"{brand}", "0%", $"{SalesPerBrand[brand]}");
        }
    }
}