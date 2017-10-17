using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    abstract class Transaction
    {
        private int _id;
        private Product _product;
        private int _amount;
        private DateTime _date;

        public abstract void Create();
        public abstract void Delete();
        public abstract void Edit();
        public abstract void Execute();
    }
}
