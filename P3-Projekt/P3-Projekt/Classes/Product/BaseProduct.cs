using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    abstract class BaseProduct
    {
        public int ID;
        private decimal _salePrice;

        public BaseProduct()
        {

        }

        public abstract void Delete();
        public abstract void Edit();
        public abstract void Withdraw();
        public abstract void Deposit();
        public abstract void Move();
    }
}
