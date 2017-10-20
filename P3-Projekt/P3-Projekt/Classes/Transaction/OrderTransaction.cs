using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class OrderTransaction : Transaction
    {
        private int _purchasePrice;
        private string _supplier;
        private int _storageRoomid;

        public OrderTransaction(Product product, int amount, string supplier, int storageRoomID) : base(product, amount)
        {
            _supplier = supplier;
            _storageRoomid = storageRoomID;
        }

        public override void Execute()
        {
            if (Product is Product)
            {
                /* Finder først storage room ved ID.
                 * Derefter bruger den StorageRoom delen som index,
                 * så man kan ændre Amount */
                var StoreStorage = (Product as Product).StorageWithAmount.Where(x => x.Key.ID == _storageRoomid).First();

                (Product as Product).StorageWithAmount[StoreStorage.Key] -= Amount;
            }
            else
            {
                /*Gør ingenting, fordi temp og service produkt ved ikke, hvilket lager de er på.
                 * Derfor kan der ikke ændres lagerstatus */
            }
        }
    }
}
