using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Database;
namespace P3_Projekt.Classes
{
    public class ServiceProduct : BaseProduct, MysqlObject
    {
        public string Name;
        public decimal GroupPrice;
        public int GroupLimit;

        public ServiceProduct(decimal salePrice, decimal groupPrice, int groupLimit, string name) : base(salePrice)
        {
            GroupPrice = groupPrice;
            GroupLimit = groupLimit;
            Name = name;
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM service_products WHERE id = {ID}";
            Mysql Connection = new Mysql();
            CreateFromRow(Connection.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row results)
        {
            ID = Convert.ToInt32(results.Values[0]);                         // id
            Name = results.Values[1];                                        // name
            SalePrice = Convert.ToInt32(results.Values[2]);                  // price
            GroupPrice = Convert.ToInt32(results.Values[3]);                 // price
            GroupLimit = Convert.ToInt32(results.Values[4]);                 // price
        }

        public void UploadToDatabase()
        {
            string sql = $"INSERT INTO `service_products` (`id`, `name`, `price`, `group_price`, `group_limit`)" +
            $"VALUES (NULL, '{Name}', '{SalePrice}','{GroupPrice}','{GroupLimit}');";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `service_products` SET" +
                $"`name` = '{Name}'," +
                $"`price` = '{SalePrice}'," +
                $"`group_price` = '{GroupPrice}'," +
                $"`group_limit` = '{GroupLimit}'," +
                $"WHERE `id` = {ID};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }
    }
}
