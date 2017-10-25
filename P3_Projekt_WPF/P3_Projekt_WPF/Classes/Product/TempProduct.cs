using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
namespace P3_Projekt_WPF.Classes
{
    public class TempProduct : BaseProduct, MysqlObject
    {
        public string Description;
        private bool _resolved;


        public TempProduct (string description, decimal salePrice) : base(salePrice)
        {
            Description = description;
            _resolved = false;
        }

        public override string GetName()
        {
            return Description;
        }

        public void Edit(string newDescription, decimal newSalePrice)
        {
            Description = newDescription;
            SalePrice = newSalePrice;
        }

        

        public void Resolve()
        {
            _resolved = true;
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `temp_product` where `id` = '{ID}'";
            Mysql Connection = new Mysql();
            CreateFromRow(Connection.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]);
            SalePrice = Convert.ToInt32(Table.Values[1]);
            Description = Table.Values[2];
            _resolved = Utils.ConvertIntToBool(Convert.ToInt32(Table.Values[3]));
        }

        public void UploadToDatabase()
        {
            string sql = "INSERT INTO `temp_products` (`id`, `sale_price`, `description`, `resolved`)"+
                $"VALUES (NULL, '{SalePrice}', '{GetName()}', '{Utils.ConvertBoolToInt(_resolved)}');";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `temp_products` SET" +
                $"`sale_price` = '{SalePrice}'," +
                $"`description` = '{GetName()}'," +
                $"`resolved` = '{Convert.ToInt32(_resolved)}'" +
                $"WHERE `id` = {ID};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }

    }
}
