using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes.Utilities;
namespace P3_Projekt_WPF.Classes
{
    public class SaleTransaction : Transaction
    {

        public int ReceiptID;
        public decimal Price;
        public bool Discount;
        public decimal TotalPrice => Price * Amount;
        public SaleTransaction(BaseProduct product, int amount, int receiptID) : base(product, amount)
        {
            ReceiptID = receiptID;
            Price = GetProductPrice();
        }

        public SaleTransaction(Row RowData) : base(null, 0)
        {
            _id = Convert.ToInt32(RowData.Values[0]);
            CreateFromRow(RowData);
        }

        public SaleTransaction(int id) : base(null, 1)
        {
            _id = id;
            GetFromDatabase();
        }

        //If transaction contains Product, decrements the shop storage room(ID 0) by amount
        //If transaction contains Temporary- or ServiceProduct, does nothing, since these do not have storage amounts
        public override void Execute()
        {
            if (Product is Product)
            {
                (Product as Product).StorageWithAmount[0] -= Amount;
            }
        }

        public int GetID()
        {
            return _id;
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
                throw new WrongProductTypeException("Dette er ikke en godkendt produkt type");
            }
        }

        public string GetProductName()
        {
            if (Product is Product)
            {
                    return $"{(Product as Product).Name}";
            }
            else if (Product is ServiceProduct)
            {
                    return $"{(Product as ServiceProduct).Name}";
            }
            else
            {
                return $"{(Product as TempProduct).Description}";
            }
        }

        private BaseProduct _getProduct(int id, string Type)
        {
            if (Type == "product")
            {
                return new Product(id);
            }
            else if (Type == "temp_product")
            {
                return new TempProduct(id);
            }
            else if (Type == "service_product")
            {
                return new ServiceProduct(id);
            }
            else
            {
                throw new UnknownProductTypeException("Dette produkt eksisetere ikke");
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

        public void EditSaleTransactionFromTempProduct(Product productToResolve)
        {
            this.Product = productToResolve;
            UpdateInDatabase();
        }

        public override void GetFromDatabase()
        {
            string getQuery = $"SELECT * FROM `sale_transactions` WHERE `id` = '{_id}'";
            CreateFromRow(Mysql.RunQueryWithReturn(getQuery).RowData[0]);
        }

        public override void CreateFromRow(Row Table)
        {
            _id = Convert.ToInt32(Table.Values[0]);
            Product = _getProduct(Convert.ToInt32(Table.Values[1]), Table.Values[2]);
            Amount = Convert.ToInt32(Table.Values[3]);
            Date = Convert.ToDateTime(Table.Values[4]);
            ReceiptID = Convert.ToInt32(Table.Values[5]);
            Price = Convert.ToDecimal(Table.Values[6]);
            Discount = Convert.ToBoolean(Table.Values[8]);
        }

        public override void UploadToDatabase()
        {
            string sql = "INSERT INTO `sale_transactions` (`id`, `product_id`, `product_type`,`amount`, `datetime`, `receipt_id`, `price`, `total_price`, `discount`)" +
                $" VALUES (NULL, '{Product.ID}', '{_getProductType()}','{Amount}', FROM_UNIXTIME('{Utils.GetUnixTime(Date)}'), '{ReceiptID}', '{Price}', '{TotalPrice}', '{Discount}');";
            Mysql.RunQuery(sql);
        }

        public override void UpdateInDatabase()
        {
            string sql = $"UPDATE `sale_transactions` SET " +
                $"`product_id` = '{Product.ID}'," +
                $"`product_type` = '{_getProductType()}'," +
                $"`amount` = '{Amount}'," +
                $"`datetime` = FROM_UNIXTIME('{Utils.GetUnixTime(Date)}')," +
                $"`receipt_id` = '{ReceiptID}'," +
                $"`price` = '{Price}'," +
                $"`total_price` = '{TotalPrice}'," +
                $"`discount` = '{Convert.ToInt32(Discount)}' " +
                $"WHERE `id` = {_id};";
            Mysql.RunQuery(sql);
        }
    }
}
