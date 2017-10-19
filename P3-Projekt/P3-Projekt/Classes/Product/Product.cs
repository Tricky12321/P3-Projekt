using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Utilities;
using System.Drawing;

namespace P3_Projekt.Classes
{
    public class Product : BaseProduct
    {
        public string Name;
        private string _brand;
        private decimal _purchasePrice;
        private Group _group;
        public bool DiscountBool;
        public decimal DiscountPrice;
        private Image _image;
        public Dictionary<StorageRoom, int> StorageWithAmount = new Dictionary<StorageRoom, int>();

        public Product(string name, string brand, decimal purchasePrice, Group group, bool discount, decimal salePrice, decimal discountPrice, Image image) : base(salePrice)
        {
            Name = name;
            _brand = brand;
            _purchasePrice = purchasePrice;
            _group = group;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
            _image = image;
            
        }

        /* No delete method */ 


        public void Edit(string name, string brand, string group, Image image)
        {
            Name = name;
            _brand = brand;
            _group = group;
            _image = image;

        }

        public void AdminEdit(string name, string brand, decimal purchasePrice, string group, bool discount, decimal discountPrice, Image image)
        {
            Name = name;
            _brand = brand;
            _purchasePrice = purchasePrice;
            _group = group;
            _discount = discount;
            _discountPrice = discountPrice;
            _image = image;
        }

        public override void Deposit()
        {
            throw new NotImplementedException();
        }

        public override void Withdraw()
        {
            throw new NotImplementedException();
        }

        public override void Move()
        {
            throw new NotImplementedException();
        }
    }
}
