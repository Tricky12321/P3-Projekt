using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;

namespace P3_Projekt_WPF.Classes
{
    public class OrderTransaction : Transaction
    {
        private decimal _purchasePrice;
        private string _supplier;
        private int _storageRoomID;

        public OrderTransaction(Product product, int amount, string supplier, int storageRoomID) : base(product, amount)
        {
            _supplier = supplier;
            _storageRoomID = storageRoomID;
        }

        public override void Execute()
        {
            if (Product is Product)
            {
                /* Finder først storage room ved ID.
                 * Derefter bruger den StorageRoom delen som index,
                 * så man kan ændre Amount */
                var StoreStorage = (Product as Product).StorageWithAmount.Where(x => x.Key.ID == _storageRoomID).First();

                (Product as Product).StorageWithAmount[StoreStorage.Key] += Amount;
            }
            else
            {
                /*Gør ingenting, fordi temp og service produkt ved ikke, hvilket lager de er på.
                 * Derfor kan der ikke ændres lagerstatus */
            }
        }

        public override void GetFromDatabase()
        {
            string getQuery = $"SELECT * FROM `order_transactions` WHERE `id` = {_id}";
            Mysql Connection = new Mysql();
            Connection.RunQuery(getQuery);

        }

        public BaseProduct CreateProduct(int ProductID, int type)
        {
            //TODO: SKal lige laves færdig til at tage alle slags produkter.
            return new Product(ProductID);
        }

        public override void CreateFromRow(Row Table)
        {
            //TODO: SKal lige kedes sammen med CreateProduct ^^ 
            //TODO: Datetime skal lige implementeres korrekt
            _id = Convert.ToInt32(Table.Values[0]);
            //Product = new Product(Convert.ToInt(Table.Values[1]));
            Amount = Convert.ToInt32(Table.Values[2]);
            //Datetime = ???Table.Values[3]
            _purchasePrice = Convert.ToDecimal(Table.Values[4]);
            _supplier = Table.Values[5];
            _storageRoomID = Convert.ToInt32(Table.Values[5]);

        }

        public override void UploadToDatabase()
        {
            //TODO: Datetime skal lige implementeres korrekt her
            string sql = "INSERT INTO `order_transactions` (`id`, `product_id`, `amount`, `datetime`, `purchase_price`, `supplier`, `storageroom_id`)"+
                $" VALUES (NULL, '{Product.ID}', '{Amount}', CURRENT_TIMESTAMP, '{_purchasePrice}', '{_supplier}', '{_storageRoomID}');";
        }

        public override void UpdateInDatabase()
        {
            //TODO: Datetime skal implementeres rigtigt her. 
            string sql = $"UPDATE `order_transaction` SET" +
               $"`product_id` = '{Product.ID}'," +
               $"`amount` = '{Amount}'," +
               //$"`datetime` = '{DateTime??}'," +
               $"`purchase_price` = '{_purchasePrice}'," +
               $"`supplier` = '{_supplier}'," +
               $"`storageroom_id` = '{_storageRoomID}'" +
               $"WHERE `id` = {_id};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }


    }
}
