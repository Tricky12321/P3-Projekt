using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Utilities
{
    public class StatisticsListItem
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Price { get; set; }

        public StatisticsListItem(string date, string name, string amount, string price)
        {
            Date = date;
            Name = name;
            Amount = amount;
            Price = price;
        }
    }
}
