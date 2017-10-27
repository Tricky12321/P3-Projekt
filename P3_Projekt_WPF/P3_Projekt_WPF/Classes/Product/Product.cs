using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Exceptions;
namespace P3_Projekt_WPF.Classes
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

        public Product(int id) : base(0)
        {
            this.ID = id;
            GetFromDatabase();
        }

        public override string GetName()
        {
            return Name;
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

        // Database stuff
        public override void GetFromDatabase()
        {
            string sql = $"SELECT * FROM products WHERE id = {ID}";
            Mysql Connection = new Mysql();
            CreateFromRow(Connection.RunQueryWithReturn(sql).RowData[0]);
        }

        public override void CreateFromRow(Row results)
        {
            ID = Convert.ToInt32(results.Values[0]);                        // id
            Name = results.Values[1];                                       // name
            Brand = results.Values[2];                                      // brand
            ProductGroup = new Group(Convert.ToInt32(results.Values[3]));   // groups
            SalePrice = Convert.ToDecimal(results.Values[4]);                 // price
            DiscountBool = Convert.ToBoolean(results.Values[5]);            // discount
            DiscountPrice = Convert.ToDecimal(results.Values[6]);             // discount_price
            GetStorageStatus();
        }
        // Henter storage status fra databasen om hvilke lagere der har hvilket antal af produkter
        private void GetStorageStatus()
        {
            string sql = $"SELECT * FROM `storage_status` WHERE `product_id` = '{ID}'";
            Mysql Connection = new Mysql();
            TableDecode Results = Connection.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                int StorageRoomID = Convert.ToInt32(row.Values[1]);
                int Amount = Convert.ToInt32(row.Values[3]);
                StorageRoom storgeRoom = new StorageRoom(StorageRoomID);
                StorageWithAmount.Add(storgeRoom, Amount);
            }
        }
        // Sletter lige alt storage information i datbasen inden der bliver uploadet noget nyt. 
        private void DeleteAllStorageData()
        {
            string sql = $"DELETE FROM `storage_status` WHERE `product_id` = '{ID}'";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }
        // Uploader alt information fra databasen om hvilke lagere der har hvilket antal af produkterne.
        private void UpdateStorageStatus()
        {
            Mysql Connection = new Mysql();
            DeleteAllStorageData();
            foreach (var Storage_Room in StorageWithAmount)
            {
                string sql = "INSERT INTO `storage_status` (`id`, `product_id`, `storageroom`, `amount`)" +
                        $" VALUES (NULL, '{ID}', '{Storage_Room.Key.ID}', '{Storage_Room.Value}');";
                Connection.RunQuery(sql);
            }
        }

        public override void UploadToDatabase()
        {
            string sql = $"INSERT INTO `products` (`id`, `name`, `brand`, `groups`, `price`, `discount`, `discount_price`)" +
            $"VALUES (NULL, '{Name}', '{Brand}', '{ProductGroup.ID}', '{SalePrice}', '{Convert.ToInt32(DiscountBool)}', '{DiscountPrice}');";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
            UpdateStorageStatus();
        }

        public override void UpdateInDatabase()
        {
            string sql = $"UPDATE `products` SET" +
                $"`name` = '{GetName()}'," +
                $"`brand` = '{Brand}'," +
                $"`groups` = '{ProductGroup.ID}'," +
                $"`price` = '{SalePrice}'," +
                $"`discount` = '{Convert.ToInt32(DiscountBool)}'," +
                $"`discount_price` = '{DiscountPrice}'" +
                $"WHERE `id` = {ID};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
            UpdateStorageStatus();


        }
    }
}
