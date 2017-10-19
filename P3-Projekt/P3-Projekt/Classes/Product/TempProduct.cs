using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class TempProduct : BaseProduct
    {
        private string _description;
        private bool _resolved;


        public TempProduct (string description, decimal salePrice) : base(salePrice)
        {
            _description = description;
            _resolved = false;
        }

        public void Edit(string newDescription, decimal newSalePrice)
        {
            _description = newDescription;
            SalePrice = newSalePrice;
        }

        public void Resolve()
        {
            _resolved = true;
        }



    }
}
