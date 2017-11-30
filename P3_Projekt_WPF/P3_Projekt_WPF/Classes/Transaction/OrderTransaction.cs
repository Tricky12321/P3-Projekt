using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
namespace P3_Projekt_WPF.Classes
{
    public class OrderTransaction : Transaction
    {
        private decimal _purchasePrice;
        private string _supplier;
        private int _storageRoomID;
        public int ID => _id;

        public OrderTransaction(Product product, int amount, string supplier, int storageRoomID) : base(product, amount)
        {
            _supplier = supplier;
            _storageRoomID = storageRoomID;
        }

        public OrderTransaction(int id) : base(null, 0)
        {
            _id = id;
            GetFromDatabase();
        }

        public OrderTransaction(Row Data, bool reference = false) : base(null, 0)
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

        public override void Execute()
        {
            if (Product is Product)
            {
                /* Finder først storage room ved ID.
                 * Derefter bruger den StorageRoom delen som index,
                 * så man kan ændre Amount */

                var StoreStorage = (Product as Product).StorageWithAmount.Where(x => x.Key == _storageRoomID).First();
                (Product as Product).StorageWithAmount[StoreStorage.Key] += Amount;
                (Product as Product).UpdateInDatabase();
            }
            else
            {
                /*Gør ingenting, fordi temp og service produkt ved ikke, hvilket lager de er på.
                 * Derfor kan der ikke ændres lagerstatus */
            }
        }

        public override void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `order_transactions` WHERE `id` = {_id}";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public void SetInformation(BaseProduct Prod)
        {
            Product = Prod;
        }

        public void CreateFromRowReference(Row Table)
        {
            _id = Convert.ToInt32(Table.Values[0]);
            //Product = new Product(Convert.ToInt32(Table.Values[1]));
            Amount = Convert.ToInt32(Table.Values[2]);
            Date = Convert.ToDateTime(Table.Values[3]);
            _purchasePrice = Convert.ToDecimal(Table.Values[4]);
            _supplier = Table.Values[5];
            _storageRoomID = Convert.ToInt32(Table.Values[6]);

        }

        public override void CreateFromRow(Row Table)
        {
            _id = Convert.ToInt32(Table.Values[0]);
            Product = new Product(Convert.ToInt32(Table.Values[1]));
            Amount = Convert.ToInt32(Table.Values[2]);
            Date = Convert.ToDateTime(Table.Values[3]);
            _purchasePrice = Convert.ToDecimal(Table.Values[4]);
            _supplier = Table.Values[5];
            _storageRoomID = Convert.ToInt32(Table.Values[6]);

        }

        public override void UploadToDatabase()
        {
            string sql = "INSERT INTO `order_transactions` (`id`, `product_id`, `amount`, `datetime`, `purchase_price`, `supplier`, `storageroom_id`)"+
                $" VALUES (NULL, '{Product.ID}', '{Amount}', FROM_UNIXTIME('{Utils.GetUnixTime(Date)}'), '{_purchasePrice.ToString().Replace(',', '.')}', '{_supplier}', '{_storageRoomID}');";
            Mysql.RunQuery(sql);
        }

        public override void UpdateInDatabase()
        {
            string sql = $"UPDATE `order_transaction` SET " +
               $"`product_id` = '{Product.ID}'," +
               $"`amount` = '{Amount}'," +
               $"`datetime` = FROM_UNIXTIME('{Utils.GetUnixTime(Date)}')," +
               $"`purchase_price` = '{_purchasePrice.ToString().Replace(',', '.')}'," +
               $"`supplier` = '{_supplier}'," +
               $"`storageroom_id` = '{_storageRoomID}' " +
               $"WHERE `id` = {_id};";
            Mysql.RunQuery(sql);
        }


    }
}
