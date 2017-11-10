using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes
{
    public class SearchedProduct
    {
        public Product CurrentProduct;
        public int NameMatch = 0;
        public int GroupMatch = 0;
        public int BrandMatch = 0;


        public SearchedProduct(Product product)
        {
            CurrentProduct = product;
        }

        public void SetNameMatch(int matchValue)
        {
            NameMatch = matchValue;
        }

        public void SetBrandMatch(int matchValue)
        {
            BrandMatch = matchValue;
        }

        public void SetGroupMatch(int matchValue)
        {
            GroupMatch = matchValue;
        }


    }
}
