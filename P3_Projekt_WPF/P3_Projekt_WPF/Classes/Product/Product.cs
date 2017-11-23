using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Exceptions;
using System.Collections.Concurrent;
namespace P3_Projekt_WPF.Classes
{
    public class Product : BaseProduct
    {
        public string Name;
        public string Brand;
        public decimal PurchasePrice;
        public int ProductGroupID;
        public bool DiscountBool;
        public decimal DiscountPrice;
        public Image Image;
        private bool _active = true;
        public bool Active => _active;
        public DateTime CreatedTime;
        public ConcurrentDictionary<int, int> StorageWithAmount = new ConcurrentDictionary<int, int>();


        public Product(int id, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal salePrice, decimal discountPrice) : base(salePrice)
        {
            ID = id;
            Name = name;
            Brand = brand;
            PurchasePrice = purchasePrice;
            ProductGroupID = groupID;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
        }

        public Product(int id) : base(0)
        {
            ID = id;
            GetFromDatabase();
        }

        public Product(Row information) : base(0m)
        {
            CreateFromRow(information);
        }

        public override string GetName()
        {
            return Name;
        }

        /* No delete method */

        //Regular edit without admin commands toggled
        public void Edit(string name, string brand, int groupID)
        {
            Name = name;
            Brand = brand;
            ProductGroupID = groupID;
        }
        
        //Admin edit with admin command toggled
        public void AdminEdit(string name, string brand, decimal purchasePrice, decimal salePrice, int groupID, bool discount, decimal discountPrice)
        {
            Name = name;
            Brand = brand;
            PurchasePrice = purchasePrice;
            ProductGroupID = groupID;
            DiscountBool = discount;
            DiscountPrice = discountPrice;
            SalePrice = salePrice;
        }

        public static int GetNextID()
        {
            string sql = "SHOW TABLE STATUS LIKE 'products'";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            int product_AC = Convert.ToInt32(Results.RowData[0].Values[10]);
            sql = "SHOW TABLE STATUS LIKE 'service_products'";
            Results = Mysql.RunQueryWithReturn(sql);
            int service_AC = Convert.ToInt32(Results.RowData[0].Values[10]);
            int return_AC = 0;
            if (service_AC != product_AC)
            {
                if (service_AC > product_AC)
                {
                    sql = $"ALTER TABLE products AUTO_INCREMENT={service_AC};";
                    Mysql.RunQuery(sql);
                    return_AC = service_AC;
                }
                else
                {
                    sql = $"ALTER TABLE service_products AUTO_INCREMENT={product_AC};";
                    Mysql.RunQuery(sql);
                    return_AC = product_AC;
                }
            }
            return return_AC;
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
            string sql = $"SELECT * FROM `products` WHERE `id` = '{ID}'";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public override void CreateFromRow(Row results)
        {
            ID = Convert.ToInt32(results.Values[0]);                        // id
            Name = results.Values[1];                                       // name
            Brand = results.Values[2];                                      // brand
            ProductGroupID = Convert.ToInt32(results.Values[3]);            // groups
            SalePrice = Convert.ToDecimal(results.Values[4]);               // price
            PurchasePrice = Convert.ToDecimal(results.Values[5]);
            DiscountBool = Convert.ToBoolean(results.Values[6]);            // discount
            DiscountPrice = Convert.ToDecimal(results.Values[7]);           // discount_price
            _active = Convert.ToBoolean(results.Values[8]);                 // active
            CreatedTime = Convert.ToDateTime(results.Values[9]);            // CreatedTime
        }
        // Henter storage status fra databasen om hvilke lagere der har hvilket antal af produkter
        private void GetStorageStatus()
        {
            string sql = $"SELECT * FROM `storage_status` WHERE `product_id` = '{ID}'";
            try
            {
                TableDecode Results = Mysql.RunQueryWithReturn(sql);
                foreach (var row in Results.RowData)
                {
                    int StorageRoomID = Convert.ToInt32(row.Values[2]);
                    int Amount = Convert.ToInt32(row.Values[3]);
                    StorageRoom storgeRoom = new StorageRoom(StorageRoomID);
                    StorageWithAmount.TryAdd(storgeRoom.ID, Amount);
                }
            }
            catch (EmptyTableException)
            {
                //Ignore EmptyTableException
            }
        }
        // Sletter lige alt storage information i datbasen inden der bliver uploadet noget nyt. 
        private void DeleteAllStorageData()
        {
            string sql = $"DELETE FROM `storage_status` WHERE `product_id` = '{ID}'";
            Mysql.RunQuery(sql);
        }
        // Uploader alt information fra databasen om hvilke lagere der har hvilket antal af produkterne.
        private void UpdateStorageStatus()
        {
            DeleteAllStorageData();
            // Sørger for at der ikke bliver indsat nogle storage rooms som er Void eller der ikke er nogen produkter i. 
            var StorageRoomsTotal = StorageWithAmount.Where(x => x.Key >= 1).Where(x => x.Value != 0);
            foreach (var Storage_Room in StorageRoomsTotal)
            {
                string sql = "INSERT INTO `storage_status` (`id`, `product_id`, `storageroom`, `amount`)" +
                        $" VALUES (NULL, '{ID}', '{Storage_Room.Key}', '{Storage_Room.Value}');";
                Mysql.RunQuery(sql);
            }
        }

        public override void UploadToDatabase()
        {
            string sql = $"INSERT INTO `products` (`id`, `name`, `brand`, `groups`, `price`, `purchase_price` ,`discount`, `discount_price`)" +
            $" VALUES (NULL, '{Name}', '{Brand}', '{ProductGroupID}', '{SalePrice}','{PurchasePrice}', '{Convert.ToInt32(DiscountBool)}', '{DiscountPrice}');";
            Mysql.RunQuery(sql);
            UpdateStorageStatus();
            CreatedTime = DateTime.Now;
        }

        public override void UpdateInDatabase()
        {
            string sql = $"UPDATE `products` SET " +
                $"`name` = '{GetName()}'," +
                $"`brand` = '{Brand}'," +
                $"`groups` = '{ProductGroupID}'," +
                $"`price` = '{SalePrice}'," +
                $"`discount` = '{Convert.ToInt32(DiscountBool)}'," +
                $"`discount_price` = '{DiscountPrice}'," +
                $"`purchase_price` = '{PurchasePrice}' " +
                $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
            UpdateStorageStatus();
        }

        public void DeactivateProduct()
        {
            if (_active)
            {
                string sql = $"UPDATE `products` SET `active` = '0' WHERE `id` = '{ID}'";
                Mysql.RunQuery(sql);
            }
            else
            {
                throw new ProductAlreadyDeActivated("Dette produkt er allerede deaktiveret");
            }
        }

        public void ActivateProduct()
        {
            if (!_active)
            {
                string sql = $"UPDATE `products` SET `active` = '1' WHERE `id` = '{ID}'";
                Mysql.RunQuery(sql);
            } else
            {
                throw new ProductAlreadyActivated("Dette produkt er allerede aktiveret");
            }
            
        }

    }
}
