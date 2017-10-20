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

        //Laver ID om til streng hvori gruppens nummer er indkluderet som de to første cifre, padding med 0'er for syns
        public string GetFullID { get {
                int fullID = this.ID + _group.ID * 10000;
                string GetFullID = fullID.ToString().PadLeft(6, '0');
                return GetFullID;
            } }



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

        //Regular edit without admin commands toggled
        public void Edit(string name, string brand, Group group, Image image)
        {
            Name = name;
            _brand = brand;
            _group = group;
            _image = image;

        }

        //Admin edit with admin command toggled
        public void AdminEdit(string name, string brand, decimal purchasePrice, decimal salePrice, Group group, bool discount, decimal discountPrice, Image image)
        {
            Name = name;
            _brand = brand;
            _purchasePrice = purchasePrice;
            _group = group;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
            _image = image;
            SalePrice = salePrice;
        }

        //modtage storage transaction?
        public void Deposit(StorageRoom depositRoom, int numberDeposited)
        {
            StorageWithAmount[depositRoom] =+ numberDeposited;
        }

        //modtage sale transaction???
        public void Withdraw(StorageRoom withdrawnRoom, int numberWithdrawn)
        {
            StorageWithAmount[withdrawnRoom] =- numberWithdrawn;
        }

        //modtage storage transaction?
        public void Move(StorageRoom moveFromRoom, StorageRoom moveToRoom, int numberMove)
        {
            StorageWithAmount[moveFromRoom] =- numberMove;
            StorageWithAmount[moveToRoom] =+ numberMove;
        }
    }
}
