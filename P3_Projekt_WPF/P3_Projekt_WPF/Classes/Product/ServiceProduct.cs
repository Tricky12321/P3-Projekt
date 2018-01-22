using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using P3_Projekt_WPF.Classes.Exceptions;

namespace P3_Projekt_WPF.Classes
{
    public class ServiceProduct : BaseProduct
    {
        public string Name;
        public decimal GroupPrice;
        public int GroupLimit;
        public int ServiceProductGroupID;
        public Image Image;
        private bool _active = true;
        public bool Active => _active;
        public ServiceProduct(int id, decimal salePrice, decimal groupPrice, int groupLimit, string name, int serviceProductGroupID) : base(salePrice)
        {
            ID = id;
            GroupPrice = groupPrice;
            GroupLimit = groupLimit;
            Name = name;
            ServiceProductGroupID = serviceProductGroupID;
        }

        public ServiceProduct(Row Data) : base(0)
        {
            CreateFromRow(Data);
        }

        public override string ToString()
        {
            return ID + " - " + Name;
        }

        public static int GetNextID()
        {
            if (Mysql.ConnectionWorking == false)
            {
                return 0;
            }
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
                    return_AC = service_AC;
                    sql = $"ALTER TABLE products AUTO_INCREMENT={service_AC};";
                    Mysql.RunQuery(sql);
                }
                else
                {
                    return_AC = product_AC;
                    sql = $"ALTER TABLE service_products AUTO_INCREMENT={product_AC};";
                    Mysql.RunQuery(sql);
                }
            }
            else
            {
                return_AC = service_AC;
            }
            return return_AC;
        }

        public ServiceProduct(int id) : base(0)
        {
            ID = id;
            GetFromDatabase();
        }
        public override string GetName()
        {
            return Name;
        }

        public override void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `service_products` WHERE `id` = {ID}";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public override void CreateFromRow(Row results)
        {
            ID = Convert.ToInt32(results.Values[0]);                         // id
            Name = results.Values[1];                                        // name
            ServiceProductGroupID = Convert.ToInt32(results.Values[2]);         // group id
            SalePrice = Math.Round(Convert.ToDecimal(results.Values[3]), 2);                  // price
            GroupPrice = Math.Round(Convert.ToDecimal(results.Values[4]), 2);                 // group price
            GroupLimit = Convert.ToInt32(results.Values[5]);                 // grouplimit
            _active = Convert.ToBoolean(results.Values[6]);                 // active
            CreatedTime = Convert.ToDateTime(results.Values[7]);
        }

        public override void UploadToDatabase()
        {
            string sql = $"INSERT INTO `service_products` (`id`, `name`, `price`, `group_price`, `group_limit`, `groups`)" +
            $"VALUES (NULL, '{Name}', '{SalePrice.ToString().Replace(',', '.')}','{GroupPrice.ToString().Replace(',', '.')}','{GroupLimit}','{ServiceProductGroupID}');";
            Mysql.RunQuery(sql);
        }

        public override void UpdateInDatabase()
        {
            string sql = $"UPDATE `service_products` SET " +
                $"`name` = '{Name}'," +
                $"`price` = '{SalePrice.ToString().Replace(',', '.')}'," +
                $"`group_price` = '{GroupPrice.ToString().Replace(',', '.')}'," +
                $"`group_limit` = '{GroupLimit}'," +
                $"`active` = '{Convert.ToInt32(_active)}'," +
                $"`groups` = '{ServiceProductGroupID}' " +
                $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
        }

        public void DeactivateProduct()
        {
            if (_active)
            {
                string sql = $"UPDATE `service_products` SET `active` = '0' WHERE `id` = '{ID}'";
                Mysql.RunQuery(sql);
                _active = false;

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
                string sql = $"UPDATE `service_products` SET `active` = '1' WHERE `id` = '{ID}'";
                Mysql.RunQuery(sql);
                _active = true;
            }
            else
            {
                throw new ProductAlreadyActivated("Dette produkt er allerede aktiveret");
            }
        }
    }
}
