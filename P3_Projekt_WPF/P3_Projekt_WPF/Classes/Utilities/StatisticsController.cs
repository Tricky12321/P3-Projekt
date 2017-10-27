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

        public StatisticsController()
        {

        }

        public void RequestStatistics(DateTime from, DateTime to)
        {
            int fromUnixTime = Utils.GetUnixTime(from);
            int toUnixTime = Utils.GetUnixTime(to);

            TransactionsForStatistics = DatabaseTransactions.Where(x => (Utils.GetUnixTime(x.Date) > fromUnixTime && Utils.GetUnixTime(x.Date) < toUnixTime)).ToList();
        }

        
        

    }
}
