using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes
{
    class SearchedProduct
    {
        public Product CurrentProduct;
        public int WordMatches;


        public SearchedProduct(Product product)
        {
            CurrentProduct = product;
        }
    }
}
