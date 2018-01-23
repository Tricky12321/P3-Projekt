using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
using System.Collections.Concurrent;

namespace P3_Projekt_WPF.Classes
{
    public class StorageTransaction : Transaction
    {
        public StorageRoom Source;
        public StorageRoom Destination;

        public int ID => _id;

        public StorageTransaction(Product product, int amount, int sourceInt, int destinationInt, ConcurrentDictionary<int, StorageRoom> storageWithAmountDictionary) : base(product, amount)
        {
            Source = storageWithAmountDictionary[sourceInt];
            Destination = storageWithAmountDictionary[destinationInt];
        }

        public StorageTransaction(int id) : base(null, 0)
        {
            _id = id;
            GetFromDatabase();
        }

        public StorageTransaction(Row Data, bool reference = false) : base(null, 0)
        {
            if (!reference)
            {
                CreateFromRow(Data);
            }
            else
            {
                CreateFromRowReference(Data);
            }
        }

        public static int GetNextID()
        {
            if (Mysql.ConnectionWorking == false)
            {
                return 0;
            }
            string sql = "SHOW TABLE STATUS LIKE 'storage_transaction'";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            return Convert.ToInt32(Results.RowData[0].Values[10]);
        }

        public override void Execute()
        {
            Product Prod = (Product as Product);
            if (!Prod.StorageWithAmount.ContainsKey(Destination.ID))
            {
                Prod.StorageWithAmount.TryAdd(Destination.ID, 0);
            }
            if (!Prod.StorageWithAmount.ContainsKey(Source.ID))
            {
                Prod.StorageWithAmount.TryAdd(Source.ID, 0);
            }
            Prod.StorageWithAmount[Source.ID] -= Amount;
            Prod.StorageWithAmount[Destination.ID] += Amount;
        }

        public override void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `storage_transaction` WHERE `id` = '{_id}'";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public void SetInformation(StorageRoom Source, StorageRoom Destination, BaseProduct Prod)
        {
            Product = Prod;
            Source = Source;
            Destination = Destination;
        }

        public void CreateFromRowReference(Row Table)
        {
            _id = Convert.ToInt32(Table.Values[0]);
            //Product = new Product(Convert.ToInt32(Table.Values[1]));
            Amount = Convert.ToInt32(Table.Values[2]);
            Date = Convert.ToDateTime(Table.Values[3]);
            //_source = new StorageRoom(Convert.ToInt32(Table.Values[4]));
            //_destination = new StorageRoom(Convert.ToInt32(Table.Values[5]));
        }

        public override void CreateFromRow(Row Table)
        {
            _id = Convert.ToInt32(Table.Values[0]);
            Product = new Product(Convert.ToInt32(Table.Values[1]));
            Amount = Convert.ToInt32(Table.Values[2]);
            Date = Convert.ToDateTime(Table.Values[3]);
            Source = new StorageRoom(Convert.ToInt32(Table.Values[4]));
            Destination = new StorageRoom(Convert.ToInt32(Table.Values[5]));
        }

        public override void UploadToDatabase()
        {
            string sql = "INSERT INTO `storage_transaction` (`id`, `product_id`, `amount`, `source_storageroom_id`, `destination_storageroom_id`)" +
                $" VALUES (NULL, '{Product.ID}', '{Amount}', '{Source.ID}', '{Destination.ID}');";
            Mysql.RunQuery(sql);
            Product.UpdateInDatabase();
        }


        public override void UpdateInDatabase()
        {
            string sql = $"UPDATE `storage_transaction` SET " +
               $"`product_id` = '{Product.ID}'," +
               $"`amount` = '{Amount}'," +
               $"`source_storageroom_id` = '{Source.ID}'," +
               $"`destination_storageroom_id` = '{Destination.ID}' " +
               $"WHERE `id` = {_id};";
            Mysql.RunQuery(sql);
        }
    }
}
