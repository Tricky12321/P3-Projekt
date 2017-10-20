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
        public Group ProductGroup;
        public bool DiscountBool;
        public decimal DiscountPrice;
        private Image _image;
        public Dictionary<StorageRoom, int> StorageWithAmount = new Dictionary<StorageRoom, int>();

        
        public string GetFullID { get {
                int fullID = this.ID + ProductGroup.ID * 10000;
                string GetFullID = fullID.ToString().PadLeft(5, '0');
                return GetFullID;
            } }



        public Product(string name, string brand, decimal purchasePrice, Group group, bool discount, decimal salePrice, decimal discountPrice, Image image) : base(salePrice)
        {
            Name = name;
            _brand = brand;
            _purchasePrice = purchasePrice;
            ProductGroup = group;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
            _image = image;

            
        }

        /* No delete method */ 


        public void Edit(string name, string brand, Group group, Image image)
        {
            Name = name;
            _brand = brand;
            ProductGroup = group;
            _image = image;

        }

        public void AdminEdit(string name, string brand, decimal purchasePrice, decimal salePrice, Group group, bool discount, decimal discountPrice, Image image)
        {
            Name = name;
            _brand = brand;
            _purchasePrice = purchasePrice;
            ProductGroup = group;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
            _image = image;
            SalePrice = salePrice;
        }

        public void Deposit(StorageRoom depositRoom, int numberDeposited)
        {
            StorageWithAmount[depositRoom] =+ numberDeposited;
        }

        public void Withdraw(StorageRoom withdrawnRoom, int numberWithdrawn)
        {
            StorageWithAmount[withdrawnRoom] =- numberWithdrawn;
        }

        public void Move(StorageRoom moveFromRoom, StorageRoom moveToRoom, int numberMove)
        {
            StorageWithAmount[moveFromRoom] =- numberMove;
            StorageWithAmount[moveToRoom] =+ numberMove;
        }
    }
}
