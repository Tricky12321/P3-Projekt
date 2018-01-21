using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Database;

namespace P3_Projekt.Classes
{
    public class OrderTransaction : Transaction
    {
        private int _purchasePrice;
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
            string getQuery = $"SELECT * FROM order_transactions WHERE ID = {_id}";
            Mysql Connection = new Mysql();
            Connection.RunQuery(getQuery);

        }

        public override void CreateFromRow(Row Table)
        {
            throw new NotImplementedException();
        }

        public override void UpdateInDatabase()
        {
            throw new NotImplementedException();
        }

        public override void UploadToDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
