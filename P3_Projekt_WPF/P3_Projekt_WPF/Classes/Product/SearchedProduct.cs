using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes
{
    public class SearchProduct
    {
        public Product CurrentProduct;
        public int NameMatch = 0;
        public int GroupMatch = 0;
        public int BrandMatch = 0;


        public SearchProduct(Product product)
        {
            CurrentProduct = product;
        }



    }
}
