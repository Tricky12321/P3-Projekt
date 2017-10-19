using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class SaleTransaction : Transaction
    {
        public int ReceiptID;
        private bool _isTemp;

        public SaleTransaction(BaseProduct product, int amount, int receiptID) : base(product, amount)
        {
            ReceiptID = receiptID;
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Edit()
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            if (Product is Product)
            {
                /* Finder første storage room, som altid er butikken. Butikken har ID 0.
                 * Derefter bruger den StorageRoom delen som index,
                 * så man kan ændre Amount */
                var StoreStorage = (Product as Product).StorageWithAmount.Where( x => x.Key.ID == 0).First();

                (Product as Product).StorageWithAmount[StoreStorage.Key] -= Amount;
            }
            else
            {
                /*Gør ingenting, fordi temp produkt ikke ved hvilket lager den er på.
                 * Det eneste der sker ved temp produkt er resolve eller merge */
            }
                
        }
    }
}
