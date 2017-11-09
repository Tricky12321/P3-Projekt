using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;

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

        public void RequestStatisticsDate(DateTime from, DateTime to)
        {
            int fromUnixTime = Utils.GetUnixTime(from);
            int toUnixTime = Utils.GetUnixTime(to);

            string requestStatisticsQuery = 
                $"SELECT * FROM `sale_transactions` WHERE FROM_UNIXTIME(`datetime`) >= '{fromUnixTime}' AND FROM_UNIXTIME(`datetime`) <= '{toUnixTime}';";

            TableDecode Return = Mysql.RunQueryWithReturn(requestStatisticsQuery);
            
            foreach(Row row in Return.RowData)
            {
                TransactionsForStatistics.Add(new SaleTransaction(row));
            }

            //TransactionsForStatistics = TransactionsForStatistics.Where(x => (Utils.GetUnixTime(x.Date) > fromUnixTime && Utils.GetUnixTime(x.Date) < toUnixTime)).ToList();
        }

        public void RequestStatisticsToday()
        {
            DateTime today = DateTime.Now;

            string requestStatisticsQuery =
                $"SELECT * FROM `sale_transactions` WHERE FROM_UNIXTIME(`datetime`) == '{today}';";

            TableDecode Return = Mysql.RunQueryWithReturn(requestStatisticsQuery);

            foreach (Row row in Return.RowData)
            {
                TransactionsForStatistics.Add(new SaleTransaction(row));
            }
            //TransactionsForStatistics = TransactionsForStatistics.Where(x => x.Date.Day == today.Day).ToList();
        }

        public void RequestStatisticsYesterday()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);

            string requestStatisticsQuery =
                $"SELECT * FROM `sale_transactions` WHERE FROM_UNIXTIME(`datetime`) == '{yesterday}';";

            TableDecode Return = Mysql.RunQueryWithReturn(requestStatisticsQuery);

            foreach (Row row in Return.RowData)
            {
                TransactionsForStatistics.Add(new SaleTransaction(row));
            }

           // TransactionsForStatistics = TransactionsForStatistics.Where(x => x.Date.Day == yesterday.Day).ToList();
        }

        public void RequestStatisticsWithParameters(string productID, string brand, Group group)
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
                TransactionsForStatistics = TransactionsForStatistics.Where(x => ((x.Product as Product).ProductGroupID == group.ID) || (x.Product as ServiceProduct).ServiceProductGroup == group).ToList();
            }
        }
    }
}
