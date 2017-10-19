using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class ServiceProduct : BaseProduct
    {
        public string Name;
        public decimal GroupPrice;
        public int GroupLimit;

        public ServiceProduct(decimal salePrice, decimal groupPrice, int groupLimit, string name) : base(salePrice)
        {
            GroupPrice = groupPrice;
            GroupLimit = groupLimit;
            Name = name;
        }
    }
}
