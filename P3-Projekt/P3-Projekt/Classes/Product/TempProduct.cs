using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class TempProduct : BaseProduct
    {
        public string Description;
        private bool _resolved;


        public TempProduct (string description, decimal salePrice) : base(salePrice)
        {
            Description = description;
            _resolved = false;
        }

        public void Edit(string newDescription, decimal newSalePrice)
        {
            Description = newDescription;
            SalePrice = newSalePrice;
        }

        public void Resolve()
        {
            _resolved = true;
        }



    }
}
