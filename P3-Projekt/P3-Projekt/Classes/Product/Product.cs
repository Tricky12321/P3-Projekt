using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Utilities;
using System.Drawing;
using P3_Projekt.Classes.Database;

namespace P3_Projekt.Classes
{
    public class Product : BaseProduct
    {
        public string Name;
        public string Brand;
        private decimal _purchasePrice;
        public Group ProductGroup;
        public bool DiscountBool;
        public decimal DiscountPrice;
        private Image _image;
        public Dictionary<StorageRoom, int> StorageWithAmount = new Dictionary<StorageRoom, int>();
 

        public Product(string name, string brand, decimal purchasePrice, Group group, bool discount, decimal salePrice, decimal discountPrice, Image image) : base(salePrice)
        {
            Name = name;
            Brand = brand;
            _purchasePrice = purchasePrice;
            ProductGroup = group;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
            _image = image;

            
        }

        /* No delete method */ 

        //Regular edit without admin commands toggled
        public void Edit(string name, string brand, Group group, Image image)
        {
            Name = name;
            Brand = brand;
            ProductGroup = group;
            _image = image;

        }

        //Admin edit with admin command toggled
        public void AdminEdit(string name, string brand, decimal purchasePrice, decimal salePrice, Group group, bool discount, decimal discountPrice, Image image)
        {
            Name = name;
            Brand = brand;
            _purchasePrice = purchasePrice;
            ProductGroup = group;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
            _image = image;
            SalePrice = salePrice;
        }

        //modtage storage transaction?
        /*public void Deposit(StorageRoom depositRoom, int numberDeposited)
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
        }*/


        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM products WHERE id = {ID}";
            Mysql Connection = new Mysql();
            CreateFromTableDecode(Connection.RunQueryWithReturn(sql));
        }

        private void CreateFromTableDecode(TableDecode results)
        {

        }
    }
}
