using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Utilities;

namespace P3_Projekt_WPF.Classes
{
    public class Statistics
    {
        /*
        private int _id;
        private DateTime _fromDate;
        private DateTime _toDate;

        private List<SaleTransaction> _sortedTransactions = new List<SaleTransaction>();

        public Statistics()
        {
            // TODO: Hent transactions fra DB til _sortedTransactions

            // _sortedTransactions = 
        }

        public List<SaleTransaction> RequestStatisticsWithParameters(string productID, string brand, Group group)
        {
            if (productID != null)
            {
                // TODO: Lav input validering af product id
                _sortedTransactions = _sortedTransactions.Where(x => x.Product.ID == Int32.Parse(productID)).ToList();
            }
            if (brand != null)
            {
                _sortedTransactions = _sortedTransactions.Where(x => (x.Product as Product).Brand == brand).ToList();
            }
            if (group != null)
            {
                _sortedTransactions = _sortedTransactions.Where(x => ((x.Product as Product).ProductGroup == group) || (x.Product as ServiceProduct).ServiceProductGroup == group).ToList();
            }

            return _sortedTransactions;
        }

        public List<SaleTransaction> RequestStatisticsDate(DateTime from, DateTime to)
        {
            int fromUnixTime = Utils.GetUnixTime(from);
            int toUnixTime = Utils.GetUnixTime(to);

            _sortedTransactions = _sortedTransactions.Where(x => (Utils.GetUnixTime(x.Date) > fromUnixTime && Utils.GetUnixTime(x.Date) < toUnixTime)).ToList();

            return _sortedTransactions;
        }

        public List<SaleTransaction> RequestStatisticsToday()
        {
            DateTime today = DateTime.Now;

            _sortedTransactions = _sortedTransactions.Where(x => x.Date.Day == today.Day).ToList();

            return _sortedTransactions;
        }

        public List<SaleTransaction> RequestStatisticsYesterday()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);

            _sortedTransactions = _sortedTransactions.Where(x => x.Date.Day == yesterday.Day).ToList();

            return _sortedTransactions;
        }*/
    }
}
