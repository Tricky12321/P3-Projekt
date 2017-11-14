﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
namespace P3_Projekt_WPF.Classes
{
    public class ServiceProduct : BaseProduct
    {
        public string Name;
        public decimal GroupPrice;
        public int GroupLimit;
        public int ServiceProductGroupID;

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
            ServiceProductGroupID = Convert.ToInt32(results.Values[2]);
            SalePrice = Convert.ToInt32(results.Values[3]);                  // price
            GroupPrice = Convert.ToInt32(results.Values[4]);                 // price
            GroupLimit = Convert.ToInt32(results.Values[5]);                 // price
        }

        public override void UploadToDatabase()
        {
            string sql = $"INSERT INTO `service_products` (`id`, `name`, `price`, `group_price`, `group_limit`, `groups`)" +
            $"VALUES (NULL, '{Name}', '{SalePrice}','{GroupPrice}','{GroupLimit}','{ServiceProductGroupID}');";
            Mysql.RunQuery(sql);
        }

        public override void UpdateInDatabase()
        {
            string sql = $"UPDATE `service_products` SET " +
                $"`name` = '{Name}'," +
                $"`price` = '{SalePrice}'," +
                $"`group_price` = '{GroupPrice}'," +
                $"`group_limit` = '{GroupLimit}'," +
                $"`groups` = '{ServiceProductGroupID}' "+
                $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
        }
    }
}
