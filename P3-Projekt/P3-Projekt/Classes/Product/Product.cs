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
    public class Product : BaseProduct, MysqlObject
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
            CreateFromRow(Connection.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row results)
        {
            ID = Convert.ToInt32(results.Values[0]);                        // id
            Name = results.Values[1];                                       // name
            Brand = results.Values[2];                                      // brand
            ProductGroup = new Group(Convert.ToInt32(results.Values[3]));   // groups
            SalePrice = Convert.ToInt32(results.Values[4]);                 // price
            DiscountBool = Convert.ToBoolean(results.Values[5]);            // discount
            DiscountPrice = Convert.ToInt32(results.Values[6]);             // discount_price
        }

        public void UploadToDatabase()
        {
            string sql = $"INSERT INTO `products` (`id`, `name`, `brand`, `groups`, `price`, `discount`, `discount_price`)"+
            $"VALUES (NULL, '{Name}', '{Brand}', '{ProductGroup.ID}', '{SalePrice}', '{Convert.ToInt32(DiscountBool)}', '{DiscountPrice}');";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `products` SET"+
                $"`name` = '{Name}',"+
                $"`brand` = '{Brand}'," +
                $"`groups` = '{ProductGroup.ID}'," +
                $"`price` = '{SalePrice}'," +
                $"`discount` = '{Convert.ToInt32(DiscountBool)}'," +
                $"`discount_price` = '{DiscountPrice}'" +
                $"WHERE `id` = {ID};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }
    }
}
