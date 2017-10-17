using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    class Product : BaseProduct
    {
        private string _name;
        private string _brand;
        private int _purchasePrice;
        private string _group;
        private bool _discount;
        private int _discountPrice;
        private int _inStock;
        private StorageRoom _storageRoom;
    }
}
