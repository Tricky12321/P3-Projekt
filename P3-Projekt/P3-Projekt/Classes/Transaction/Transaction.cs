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
        public int Amount;
        private DateTime _date;

        public Transaction(BaseProduct product, int amount)
        {
            Product = product;
            Amount = amount;
            _id = _idCounter++;
            _date = DateTime.Now;
        }
        

        public abstract void Delete();
        public abstract void Edit();
        public abstract void Execute();
    }
}
