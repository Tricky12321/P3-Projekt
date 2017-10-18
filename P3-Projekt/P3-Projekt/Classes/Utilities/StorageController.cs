using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace P3_Projekt.Classes.Utilities
{
    public class StorageController
    {
        BoerglumAbbeyStorageandSale _boerglumAbbeyStorageandSale;

        private int _idGroupCounter = 0;

        public Dictionary<int, Product> ProductDictionary = new Dictionary<int, Product>();
        public Dictionary<int, Group> GroupDictionary = new Dictionary<int, Group>();

        public StorageController(BoerglumAbbeyStorageandSale boerglumAbbeyStorageandSale)
        {
            _boerglumAbbeyStorageandSale = boerglumAbbeyStorageandSale;

        }

        public void DeleteProduct(int ProductID)
        {
            ProductDictionary.Remove(ProductID);
        }

        public void DeleteGroup(int GroupID)
        {
            GroupDictionary.Remove(GroupID);
        }

        public void CreateProduct(string name, string brand, decimal purchasePrice, string group, bool discount, decimal discountPrice, Image image)
        {
            Product product = new Product(name, brand, purchasePrice, group, discount, discountPrice, image);
            ProductDictionary.Add(product.ID, product);
        }
    }
}
