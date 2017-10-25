using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Exceptions;

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

        public override void Execute()
        {
            if (Product is Product)
            {
                /* Finder første storage room, som altid er butikken. Butikken har ID 0.
                 * Derefter bruger den StorageRoom delen som index,
                 * så man kan ændre Amount */
                var StoreStorage = (Product as Product).StorageWithAmount.Where(x => x.Key.ID == 0).First();

                (Product as Product).StorageWithAmount[StoreStorage.Key] -= Amount;
            }
            else
            {
                /*Gør ingenting, fordi temp og service produkt ved ikke, hvilket lager de er på.
                 * Derfor kan der ikke ændres lagerstatus */
            }

        }

        //Returns the correct price according to discount, groups etc.
        public decimal GetProductPrice()
        {
            if (Product is Product)
            {
                if ((Product as Product).DiscountBool)
                {
                    return (Product as Product).DiscountPrice;
                }
                else
                {
                    return (Product as Product).SalePrice;
                }
            }
            else if (Product is TempProduct)
            {
                return (Product as TempProduct).SalePrice;
            }
            else if (Product is ServiceProduct)
            {
                if (Amount >= (Product as ServiceProduct).GroupLimit)
                {
                    return (Product as ServiceProduct).GroupPrice;
                }
                else
                {
                    return (Product as ServiceProduct).SalePrice;
                }
            }
            else
            {
                throw new WrongProductTypeException();
            }
        }

        public string GetTransactionString()
        {
            if (Product is Product)
            {
                if ((Product as Product).DiscountBool)
                {
                    return $"{(Product as Product).Name} {(Product as Product).DiscountPrice}";
                }
                else
                {
                    return $"{(Product as Product).Name} {(Product as Product).SalePrice}";
                }
            }
            else if (Product is ServiceProduct)
            {
                if ((Product as ServiceProduct).GroupLimit <= Amount)
                {
                    return $"{(Product as ServiceProduct).Name} {(Product as ServiceProduct).GroupPrice}";
                }
                else
                {
                    return $"{(Product as ServiceProduct).Name} {(Product as ServiceProduct).SalePrice}";
                }
            }
            else
            {
                return $"{(Product as TempProduct).Description} {(Product as TempProduct).SalePrice}";
            }
        }

        public string GetProductNameString()
        {
            if (Product is Product)
            {
                if ((Product as Product).DiscountBool)
                {
                    return $"{(Product as Product).Name}";
                }
                else
                {
                    return $"{(Product as Product).Name}";
                }
            }
            else if (Product is ServiceProduct)
            {
                if ((Product as ServiceProduct).GroupLimit <= Amount)
                {
                    return $"{(Product as ServiceProduct).Name}";
                }
                else
                {
                    return $"{(Product as ServiceProduct).Name}";
                }
            }
            else
            {
                return $"{(Product as TempProduct).Description}";
            }
        }
    }
}
