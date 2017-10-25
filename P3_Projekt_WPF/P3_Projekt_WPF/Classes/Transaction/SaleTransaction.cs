using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Exceptions;

namespace P3_Projekt_WPF.Classes
{
    public class SaleTransaction : Transaction
    {

        public int ReceiptID;
        public decimal Price;
        public bool Discount;
        public decimal TotalPrice => GetProductPrice() * Amount;
        public SaleTransaction(BaseProduct product, int amount, int receiptID) : base(product, amount)
        {
            ReceiptID = receiptID;
        }

        public SaleTransaction(Row RowData) : base(new Product(Convert.ToInt32(RowData.Values[1])),Convert.ToInt32(RowData.Values[2]))
        {
            CreateFromRow(RowData);
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

        public override void GetFromDatabase()
        {
            string getQuery = $"SELECT * FROM sale_transactions WHERE ID = {_id}";
            Mysql Connection = new Mysql();
            Connection.RunQuery(getQuery);

        }

        public override void CreateFromRow(Row Table)
        {
            _id = Convert.ToInt32(Table.Values[0]);
            Product = _getProduct(Convert.ToInt32(Table.Values[1]), Table.Values[2]);
            Amount = Convert.ToInt32(Table.Values[3]);
            //TODO: Datatime skal laves her Table.Values[4]
            ReceiptID = Convert.ToInt32(Table.Values[5]);
            Price = Convert.ToDecimal(Table.Values[6]);
            Discount = Convert.ToBoolean(Table.Values[8]);
        }

        private BaseProduct _getProduct(int id, string Type)
        {
            if (Type == "product")
            {
                return new Product(id);
            } else if(Type == "temp_product")
            {
                return new TempProduct(id);
            } else if(Type == "service_product")
            {
                return new ServiceProduct(id);
            } else
            {
                throw new UnknownProductTypeException("This product is not known by the program");
            }
        }

        private string _getProductType()
        {
            if (Product is ServiceProduct)
            {
                return "service_product";
            }
            if (Product is TempProduct)
            {
                return "temp_product";
            }
            return "product";
        }

        public override void UploadToDatabase()
        {
            //TODO: Der skal laves datetime i den her query
            string sql = "INSERT INTO `sale_transactions` (`id`, `product_id`, `product_type`,`amount`, `datetime`, `receipt_id`, `price`, `total_price`, `discount`)"+
                $" VALUES (NULL, '{Product.ID}', '{_getProductType()}','{Amount}', '2017-10-25 00:00:00', '{ReceiptID}', '{GetProductPrice()}', '{TotalPrice}', '{Discount}');";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }

        public override void UpdateInDatabase()
        {   
            //TODO: Der skal laves Datatime her
            string sql = $"UPDATE `sale_transactions` SET" +
                $"`product_id` = '{Product.ID}'," +
                $"`product_type` = '{_getProductType()}'," +
                $"`amount` = '{Amount}'," +
                //$"`datetime` = '{DateTime??}'," +
                $"`receipt_id` = '{ReceiptID}'," +
                $"`price` = '{GetProductPrice()}'," +
                $"`total_price` = '{TotalPrice}'," +
                $"`discount` = '{Convert.ToInt32(Discount)}'" +
                $"WHERE `id` = {_id};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }
    }
}
