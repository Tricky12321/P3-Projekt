using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Utilities
{
    public class StatisticsController
    {
        //New name please :(
        List<SaleTransaction> TransactionsForStatistics = new List<SaleTransaction>();
        List<SaleTransaction> DatabaseTransactions = new List<SaleTransaction>();

        public StatisticsController()
        {

        }

        public void RequestStatistics(DateTime from, DateTime to)
        {
            int fromUnixTime = GetUnixTime(from);
            int toUnixTime = GetUnixTime(to);

            TransactionsForStatistics = DatabaseTransactions.Where(x => (GetUnixTime(x.Date) > fromUnixTime && GetUnixTime(x.Date) < toUnixTime)).ToList();
        }

        public int GetUnixTime(DateTime Tid)
        {
            int unixTimestamp = (int)(Tid.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

    }
}
