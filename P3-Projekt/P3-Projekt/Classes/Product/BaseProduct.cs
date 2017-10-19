using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace P3_Projekt.Classes
{
    public abstract class BaseProduct
    {
        public int ID;
        public decimal SalePrice;
        protected static int _idCounter = 0;

        public BaseProduct(decimal salePrice)
        {
            ID = _idCounter++;
            SalePrice = salePrice;
        }
        
        public abstract void Withdraw();
        public abstract void Deposit();
        public abstract void Move();
    }
}
