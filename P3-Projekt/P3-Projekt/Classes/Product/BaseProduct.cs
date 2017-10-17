using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    abstract class BaseProduct
    {
        private int _id;
        private int _salePrice;

        public abstract void Create();
        public abstract void Delete();
        public abstract void Edit();
//      public abstract void Withdraw();
        public abstract void Deposit();
        public abstract void Sold();
        public abstract void Move();
    }
}
