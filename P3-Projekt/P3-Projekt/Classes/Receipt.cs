using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class Receipt
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

        public decimal FindTransactionPrice(Transaction transaction)
        {
            decimal priceTotal = 0;

            if (transaction.Product.)

            return 
        }

        public void RemoveTransaction(int productID)
        {
            Transactions.RemoveAll(x => x.Product.ID == productID);
        }

        public void Execute()
        {
            foreach (Transaction transaction in Transactions)
            {
                transaction.Execute();
            }
        }
    }
}
