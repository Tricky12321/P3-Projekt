using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes.Utilities;
using System.Windows;
namespace P3_Projekt_WPF.Classes
{
    public class SaleTransaction : Transaction
    {
        public int ReceiptID;
        public decimal Price;
        public bool DiscountBool;
        public decimal DiscountPrice = 0m; 
        public decimal TotalPrice => Price * Amount;
        private static StorageController _storageController = null;
        public string SoldBy = "";

        const int shopID = 1;

        public SaleTransaction(BaseProduct product, int amount, int receiptID) : base(product, amount)
        {
            ReceiptID = receiptID;
            Price = GetProductPrice();
        }

        public SaleTransaction(Row RowData) : base(null, 0)
        {
            CreateFromRow(RowData);
        }

        public SaleTransaction(Row RowData, StorageController storageController) : base(null, 0)
        {
            _storageController = storageController;
            CreateFromRow(RowData);
        }

        public SaleTransaction(int id) : base(null, 1)
        {
            _id = id;
            GetFromDatabase();
        }

        public static void SetStorageController(StorageController SC)
        {
            _storageController = SC;
        }
        //If transaction contains Product, decrements the shop storage room(ID 0) by amount
        //If transaction contains Temporary- or ServiceProduct, does nothing, since these do not have storage amounts
        public override void Execute()
        {
            if (Product is Product)
            {
                Product prod = (Product as Product);
                prod.GetStorageStatus();
                if (!prod.StorageWithAmount.ContainsKey(shopID))
                {
                    prod.StorageWithAmount.TryAdd(shopID, -Amount);
                }
                else
                {
                    (Product as Product).StorageWithAmount[shopID] -= Amount;
                }
                if (prod.StorageWithAmount.Where(x => x.Value < 0).Count() > 0)
                {
                    // Hvis det er nogle storageroom med negativ værdi
                    StringBuilder Text = new StringBuilder();

                    Text.Append("Produktet: " + prod.Name + "'s lagerstatus har fejl!\n");

                    foreach (KeyValuePair<int,int> strorageWithAmount in prod.StorageWithAmount)
                    {
                        string AppendString = "";
                        if (strorageWithAmount.Value < 0)
                        {
                            AppendString = " **** - "+ _storageController.StorageRoomDictionary[strorageWithAmount.Key].Name + " har " + strorageWithAmount.Value.ToString() + " stk\n";
                        }
                        else
                        {
                            AppendString = " - " + _storageController.StorageRoomDictionary[strorageWithAmount.Key].Name + " har " + strorageWithAmount.Value.ToString() + " stk\n";
                        }
                        Text.Append(AppendString);
                    }
                    string Output = Text.ToString();
                    var NewBox = MessageBox.Show(Text.ToString());
                }
                prod.UpdateInDatabase();
            }
            else if (Product is TempProduct)
            {
                Product.ID = TempProduct.GetNextID();
                Product.UploadToDatabase();
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

        public int GetProductID()
        {
            if (Product is Product)
            {
                return (Product as Product).ID;
            }
            else if (Product is ServiceProduct)
            {
                return (Product as ServiceProduct).ID;
            }
            else if (Product is TempProduct && (Product as TempProduct).Resolved)
            {
                return (_getProduct((Product as TempProduct).ResolvedProductID, "product") as Product).ID;
            }
            //Returns -1 for unresolved temporary products
            return -1;
        }

        public int GetGroupID()
        {
            if (Product is Product)
            {
                return (Product as Product).ProductGroupID;
            }
            else if (Product is ServiceProduct)
            {
                return (Product as ServiceProduct).ServiceProductGroupID;
            }
            else if (Product is TempProduct && (Product as TempProduct).Resolved)
            {
                return (_getProduct((Product as TempProduct).ResolvedProductID, "product") as Product).ProductGroupID;
            }
            return -1;
        }

        public string GetBrand()
        {
            if(Product is Product)
            {
                return (Product as Product).Brand;
            }
            else if (Product is TempProduct && (Product as TempProduct).Resolved)
            {
                return (_getProduct((Product as TempProduct).ResolvedProductID, "product") as Product).Brand;
            }
            else
            {
                //ServiceProducts and unresolved TempProducts do not contain brands
                return "";
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
                if (_storageController != null)
                {
                    return _storageController.ProductDictionary[id];
                }
                return new Product(id);
            }
            else if (Type == "temp_product")
            {
                TempProduct ResultsProduct;
                if (_storageController != null)
                {
                    ResultsProduct = _storageController.TempProductDictionary[id];
                }
                else
                {
                    ResultsProduct = new TempProduct(id);
                }
                if (ResultsProduct.ResolvedProductID != 0)
                {
                    if (_storageController != null)
                    {
                        return _storageController.ProductDictionary[ResultsProduct.ResolvedProductID];
                    }
                    return new Product(ResultsProduct.ResolvedProductID);
                }
                else
                {
                    return ResultsProduct;
                }
            }
            else if (Type == "service_product")
            {
                if (_storageController != null)
                {
                    return _storageController.ServiceProductDictionary[id];
                }
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
            Product = productToResolve;
            UpdateInDatabase();
        }

        public StatisticsListItem StatisticsStrings()
        {
            return new StatisticsListItem(Date.ToString("dd/MM/yy"), Product.GetName(), Amount.ToString(), TotalPrice.ToString());
        }

        public void CheckIfGroupPrice()
        {
            if(Product is ServiceProduct && Product.GetName() != "Is")
            {
                Price = (Amount >= (Product as ServiceProduct).GroupLimit ? (Product as ServiceProduct).GroupPrice : (Product as ServiceProduct).SalePrice);
            }
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
            DiscountBool = Convert.ToBoolean(Table.Values[8]);
            SoldBy = Table.Values[9];
        }

        public override void UploadToDatabase()
        {
            SoldBy = Environment.UserName;
            string sql = "INSERT INTO `sale_transactions` (`id`, `product_id`, `product_type`,`amount`, `receipt_id`, `price`, `total_price`, `discount`,`sold_by`)" +
                $" VALUES (NULL, '{Product.ID}', '{_getProductType()}','{Amount.ToString().Replace(',', '.')}', '{ReceiptID}', '{Price.ToString().Replace(',', '.')}', '{TotalPrice.ToString().Replace(',', '.')}', '{DiscountBool}','{SoldBy}');";
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
                $"`price` = '{Price.ToString().Replace(',', '.')}'," +
                $"`total_price` = '{TotalPrice.ToString().Replace(',', '.')}'," +
                $"`discount` = '{Convert.ToInt32(DiscountBool)}'," +
                $"`sold_by` = '{SoldBy}' "+
                $"WHERE `id` = {_id};";
            Mysql.RunQuery(sql);
        }
    }
}
