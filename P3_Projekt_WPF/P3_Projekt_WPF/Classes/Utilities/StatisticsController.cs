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

        public string GetQueryString(bool searchID, int idToSearch, bool searchGroup, int groupToSearch, bool searchBrand, string brandToSearch, DateTime from, DateTime to)
        {
            StringBuilder NewString = new StringBuilder("SELECT `sale_transactions`.* " +
                "FROM `products`, `sale_transactions` WHERE `products`.`id` = `sale_transactions`.`product_id`"+
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
            while (!_dataQueue.IsEmpty && (_saleTransactionThreads.Where(x => x.ThreadState == System.Threading.ThreadState.Running).Count() == 0))
            {
                Thread.Sleep(1);
            }
            
            _allStatisticsDone = true;
            TransactionsForStatistics = new List<SaleTransaction>(_saleTransactions);
            Timer1.Stop();
            Debug.WriteLine("[StatisticsController] took " + Timer1.ElapsedMilliseconds + "ms to fetch");
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
        }*/

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
                if(transaction.GetGroupID() >= 0)
                {
                    SalesPerGroup[transaction.GetGroupID()] += transaction.TotalPrice;
                }
            }
        }

        public StatisticsListItem GroupSalesStrings(int id, decimal totalPrice)
        {
            return new StatisticsListItem("", $"{_storageController.GroupDictionary[id].Name}", $"{Math.Round((SalesPerGroup[id] / totalPrice) * 100m, 1)}%", $"{SalesPerGroup[id]}");
        }
    }
}
