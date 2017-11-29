using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes
{
    public class SearchProduct : IComparable
    {
        public BaseProduct CurrentProduct;
        public int NameMatch = 0;
        public int GroupMatch = 0;
        public int BrandMatch = 0;
        public bool ContainsMatch = false;

        public SearchProduct(BaseProduct product)
        {
            CurrentProduct = product;
        }

        public int CompareTo(object obj)
        {
            int compare = 0;

            compare += (obj as SearchProduct).NameMatch - this.NameMatch;
            compare += (obj as SearchProduct).GroupMatch - this.GroupMatch;
            compare += (obj as SearchProduct).BrandMatch - this.BrandMatch;

            return compare;
        }
    }
}
