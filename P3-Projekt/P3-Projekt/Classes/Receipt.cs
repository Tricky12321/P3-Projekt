using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    class Receipt
    {
        private static int _idCounter = 0;

        public int ID;
        public List<Transaction> Transactions = new List<Transaction>();
        private int _numberOfProducts;
        private decimal _totalPrice;
        private decimal _paidPrice;
        private string _paymentMethod;
        private DateTime _date;

        public Receipt()
        {
            ID = _idCounter++;
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
        }
    }
}
