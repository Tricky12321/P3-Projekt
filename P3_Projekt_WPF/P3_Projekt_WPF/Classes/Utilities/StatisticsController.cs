using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Utilities
{
    public class StatisticsController
    {
        //New name please :(
        List<SaleTransaction> TransactionsForStatistics = new List<SaleTransaction>();
        List<SaleTransaction> DatabaseTransactions = new List<SaleTransaction>();

        private bool _sortByProductID;
        private bool _sortByBrand;
        private bool _sortByGroup;

        public StatisticsController()
        {
            // TODO: Hent transactions fra DB til Databasetransactions

        }

        public void RequestStatistics() { }

        public void RequestStatisticsDate(DateTime from, DateTime to)
        {
            int fromUnixTime = Utils.GetUnixTime(from);
            int toUnixTime = Utils.GetUnixTime(to);

            TransactionsForStatistics = DatabaseTransactions.Where(x => (Utils.GetUnixTime(x.Date) > fromUnixTime && Utils.GetUnixTime(x.Date) < toUnixTime)).ToList();
        }

        public void RequestStatisticsToday()
        {
            DateTime today = DateTime.Now;

            TransactionsForStatistics = DatabaseTransactions.Where(x => x.Date.Day == today.Day).ToList();
        }

        public void RequestStatisticsYesterday()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);

            TransactionsForStatistics = DatabaseTransactions.Where(x => x.Date.Day == yesterday.Day).ToList();
        }
    }
}
