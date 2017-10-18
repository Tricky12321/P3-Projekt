using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public abstract class Transaction
    {
        protected static int _idCounter = 0;

        private int _id;
        public BaseProduct Product;
        private int _amount;
        private DateTime _date;

        public Transaction(Product product, int amount)
        {
            Product = product;
            _amount = amount;
            _id = _idCounter++;
            _date = DateTime.Now;
        }
        

        public abstract void Delete();
        public abstract void Edit();
        public abstract void Execute();
    }
}
