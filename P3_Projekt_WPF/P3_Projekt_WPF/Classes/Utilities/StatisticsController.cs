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
        public List<Receipt> ReceiptsForStatistics = new List<Receipt>();
        public Dictionary<int, decimal> SalesPerGroup;

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

        public string GetQueryString(bool SearchID, int IDToSearch, bool SearchGroup, int GroupToSearch, bool SearchBrand, string BrandToSearch, DateTime From, DateTime To)
        {
            StringBuilder NewString = new StringBuilder("SELECT `sale_transactions`.*, `products`.`brand`, `products`.`groups` " +
                "FROM `products`, `sale_transactions` WHERE `products`.`id` = `sale_transactions`.`product_id`"+
                $" AND UNIX_TIMESTAMP(`datetime`) >= '{From}' AND UNIX_TIMESTAMP(`datetime`) <= '{To}'");
            if (SearchID)
            {
                NewString.Append($" AND `products`.`id` = '{IDToSearch}'");
            }
            else
            {
                if (SearchGroup)
                {
                    NewString.Append($" AND `products`.`group` = '{GroupToSearch}'");
                }

                if (SearchBrand)
                {
                    NewString.Append($" AND `products`.`brand` = '{BrandToSearch}'");
                }
            }

            return NewString.ToString();

        }

        public void RequestStatisticsDate(DateTime from, DateTime to)
        {
            Stopwatch Timer1 = new Stopwatch();
            Timer1.Start();
            _saleTransactionsCreated = 0;
            _saleTransactions = new ConcurrentQueue<SaleTransaction>();
            int fromUnixTime = Utils.GetUnixTime(from);
            int toUnixTime = Utils.GetUnixTime(EndDate(to));
            string requestStatisticsQuery =
            $"SELECT * FROM `sale_transactions` WHERE UNIX_TIMESTAMP(`datetime`) >= '{fromUnixTime}' AND UNIX_TIMESTAMP(`datetime`) <= '{toUnixTime}';";
            _dataQueue = Mysql.RunQueryWithReturnQueue(requestStatisticsQuery).RowData;
            int TransCount = _dataQueue.Count;
            CreateThreads();
            ThreadWork();
            while (!_dataQueue.IsEmpty)
            {
                Thread.Sleep(1);
            }
            
            _allStatisticsDone = true;
            TransactionsForStatistics = new List<SaleTransaction>(_saleTransactions);
            Timer1.Stop();
            Debug.WriteLine("[StatisticsController] took " + Timer1.ElapsedMilliseconds + "ms to fetch");
        }

        public DateTime EndDate(DateTime date)
        {
            TimeSpan dayEnd = new TimeSpan(23, 59, 59);
            return date + dayEnd;
        }

        public void FilterByParameters(string productID, string brand, Group group)
        {
            if (productID != null)
            {
                TransactionsForStatistics = TransactionsForStatistics.Where(x => x.Product.ID == Int32.Parse(productID)).ToList();
            }
            if (brand != null)
            {
                TransactionsForStatistics = TransactionsForStatistics.Where(x => x.GetBrand() == brand).ToList();
            }
            if (group != null)
            {
                TransactionsForStatistics = TransactionsForStatistics.Where(x => x.GetGroupID() == group.ID).ToList();
            }
        }

        public int GetReceiptTotalPrice(DateTime From, DateTime To)
        {
            Int64 toUnix = Utils.GetUnixTime(To);
            Int64 fromUnix = Utils.GetUnixTime(From);
            string sql = $"SELECT sum(`total_price`) as atotal FROM `receipt` WHERE UNIX_TIMESTAMP(`datetime`) >= '{fromUnix}' AND UNIX_TIMESTAMP(`datetime`) <= '{toUnix}'";
            return Convert.ToInt32(Mysql.RunQueryWithReturn(sql).RowData[0].Values[0]);
        }

        public int GetReceiptTotalCount(DateTime From, DateTime To)
        {
            Int64 toUnix = Utils.GetUnixTime(To);
            Int64 fromUnix = Utils.GetUnixTime(From);
            string sql = $"SELECT `id` FROM `receipt` WHERE UNIX_TIMESTAMP(`datetime`) >= '{fromUnix}' AND UNIX_TIMESTAMP(`datetime`) <= '{toUnix}'";
            return Mysql.RunQueryWithReturn(sql).RowCounter;
        }

        /*public StatisticsListItem GetReceiptStatistics()
        {
            int receiptCount = 0;
            decimal totalReceiptPrice = 0;

            foreach (Receipt receipt in ReceiptsForStatistics)
            {
                receiptCount++;
                totalReceiptPrice += receipt.TotalPrice;
            }

            return new StatisticsListItem("", "Gennemsnitlig kvitteringspris", $"{ receiptCount}", $"{totalReceiptPrice / receiptCount}");
        }

        public void GenerateGroupSales()
        {
            SalesPerGroup = new Dictionary<int, decimal>();
            foreach (Group group in _storageController.GroupDictionary.Values)
            {
                SalesPerGroup.Add(group.ID, 0m);
            }
            GetGroupSales();
        }
        
        private void GetGroupSales()
        {
            foreach(SaleTransaction transaction in TransactionsForStatistics)
            {
                if(transaction.GetGroupID() > 0)
                {
                    SalesPerGroup[transaction.GetGroupID()] += transaction.TotalPrice;
                }
            }
        }

        public StatisticsListItem GroupSalesStrings(int id, decimal totalPrice)
        {
            return new StatisticsListItem("", $"{_storageController.GroupDictionary[id].Name}", $"{(SalesPerGroup[id] / totalPrice) * 100m}%", $"{SalesPerGroup[id]}");
        }*/
    }
}
