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


        public TempProduct (decimal salePrice) : base(salePrice)
        {
        
        }
    }
}
