using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;

namespace P3_Projekt_WPF.Classes
{
    public class StorageTransaction : Transaction
    {
        private StorageRoom _source;
        private StorageRoom _destination;

        public StorageTransaction(Product product, int amount, StorageRoom source, StorageRoom destination) : base(product, amount)
        {
            _source = source;
            _destination = destination;
            Amount = amount;
            Product = product;
        }

        public override void Execute()
        {
            (Product as Product).StorageWithAmount[_source] -= Amount;
            (Product as Product).StorageWithAmount[_destination] += Amount;
        }

        public override void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `storage_transactions` WHERE `id` = {_id}";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);

        }

        public override void CreateFromRow(Row Table)
        {
            _id = Convert.ToInt32(Table.Values[0]);
            Product = new Product(Convert.ToInt32(Table.Values[1]));
            Amount = Convert.ToInt32(Table.Values[2]);
            //TODO: Der skal implementeres en korrekt version af datetime her.
            //Datetime = Convert.ToInt32(Table.Values[3]);
            _source = new StorageRoom(Convert.ToInt32(Table.Values[4]));
            _destination = new StorageRoom(Convert.ToInt32(Table.Values[5]));
        }

        public override void UploadToDatabase()
        {
            //TODO: Implementere Datetime korrekt
            string sql = "INSERT INTO `storage_transaction` (`id`, `product_id`, `amount`, `datetime`, `source_storageroom_id`, `destination_storageroom_id`)"+
                $" VALUES (NULL, '{Product.ID}', '{Amount}', 'DATETIME', '{_source.ID}', '{_destination.ID}');";
        }

        public override void UpdateInDatabase()
        {
            //TODO: Implementere Datetime korrekt
            string sql = $"UPDATE `storage_transaction` SET" +
               $"`product_id` = '{Product.ID}'," +
               $"`amount` = '{Amount}'," +
               $"`datetime` = 'DATETIME'," +
               $"`source_storageroom_id` = '{_source.ID}'," +
               $"`destination_storageroom_id` = '{_destination.ID}'," +
               $"WHERE `id` = {_id};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        } 
    }
}
