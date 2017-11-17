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

        public StatisticsController()
        {

        }

        /* Man anmoder altid om dato først, men kun én af dato-metoderne af gangen.
         * Hver dato-metode nulstiller TransactionsForStatistics ved at hente transaktioner ned fra databasen alt efter datoen
         * Derefter skal formen kalde RequestStatisticsWithParameters */


        // TODO: Tjek om det virker

        private bool _allStatisticsDone = false;
        private ConcurrentQueue<SaleTransaction> _saleTransactions;
        private List<Thread> _saleTransactionThreads = new List<Thread>();
        private int _threadCount = 30;
        private int _saleTransactionsCreated = 0;
        private ConcurrentQueue<Row> _dataQueue;

        private void CreateThreads()
        {
            for (int i = 0; i < _threadCount; i++)
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
                SaleTransaction NewTransaction = new SaleTransaction(data);
                _saleTransactions.Enqueue(NewTransaction);
                Interlocked.Increment(ref _saleTransactionsCreated);
            }

        }

        public void RequestStatisticsDate(DateTime from, DateTime to)
        {
            _saleTransactionsCreated = 0;
            _saleTransactions = new ConcurrentQueue<SaleTransaction>();
            TransactionsForStatistics = new List<SaleTransaction>();
            int fromUnixTime = Utils.GetUnixTime(from);
            int toUnixTime = Utils.GetUnixTime(EndDate(to));
            string requestStatisticsQuery =
            $"SELECT * FROM `sale_transactions` WHERE UNIX_TIMESTAMP(`datetime`) >= '{fromUnixTime}' AND UNIX_TIMESTAMP(`datetime`) <= '{toUnixTime}';";

            _dataQueue = new ConcurrentQueue<Row>(Mysql.RunQueryWithReturn(requestStatisticsQuery).RowData);
            int TransCount = _dataQueue.Count;
            Stopwatch Timer1 = new Stopwatch();
            CreateThreads();
            while (_saleTransactionsCreated != TransCount)
            {
                Thread.Sleep(5);
            }
            _allStatisticsDone = true;
            TransactionsForStatistics = new List<SaleTransaction>(_saleTransactions);
        }

        public void FilterByParameters(string productID, string brand, Group group)
        {
            if (productID != null)
            {
                // TODO: Lav input validering af product id
                TransactionsForStatistics = TransactionsForStatistics.Where(x => x.Product.ID == Int32.Parse(productID)).ToList();
            }
            if (brand != null)
            {
                TransactionsForStatistics = TransactionsForStatistics.Where(x => (x.Product as Product).Brand == brand).ToList();
            }
            if (group != null)
            {
                TransactionsForStatistics = TransactionsForStatistics.Where(x => x.GetGroupID() == group.ID).ToList();
            }
        }

        public DateTime EndDate(DateTime date)
        {
            TimeSpan dayEnd = new TimeSpan(23, 59, 59);
            return date + dayEnd;
        }
    }
}
