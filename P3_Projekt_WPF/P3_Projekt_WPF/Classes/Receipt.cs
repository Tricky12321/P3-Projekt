using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
namespace P3_Projekt_WPF.Classes
{
    public class Receipt : MysqlObject
    {
        private static int _idCounter = 0;
        public static int IDCounter { get { return _idCounter; } set { _idCounter = value; } }
        public int ID;
        public List<SaleTransaction> Transactions = new List<SaleTransaction>();
        public List<Payment> Payments = new List<Payment>();
        public int NumberOfProducts;
        public decimal TotalPrice;
        public DateTime Date;
        public decimal PaidPrice => Payments.Sum(x => x.Amount);
        public decimal TotalPriceToPay = -1m;
        public decimal DiscountOnFullReceipt = 0m;

        public Receipt()
        {
            ID = _idCounter++;
            Date = DateTime.Now;
        }


        public Receipt(int ID)
        {
            this.ID = ID;
            GetFromDatabase();
        }

        public Receipt(Row row)
        {
            CreateFromRow(row);
        }
        public void AddTransaction(SaleTransaction transaction)
        {
            //Checks if product is already in receipt//
            if (Transactions.Any(x => x.Product == transaction.Product) && transaction.Product.GetName() != "Is")
            {
                Transaction placeholderTransaction = Transactions.First(x => x.Product == transaction.Product);
                placeholderTransaction.Edit(placeholderTransaction.Amount += transaction.Amount);
            }
            else
            {
                Transactions.Add(transaction);
            }
            TotalPrice += transaction.TotalPrice;
            UpdateNumberOfProducts();
        }

        public void UpdateTotalPrice()
        {
            TotalPrice = 0;
            foreach (SaleTransaction transaction in Transactions)
            {
                if (transaction.DiscountBool)
                {

                }
                TotalPrice += transaction.TotalPrice;
            }
            TotalPrice -= DiscountOnFullReceipt;
        }

        public void RemoveTransaction(int productID)
        {
            SaleTransaction placeholderTransaction = FindTransactionFromProductID(productID);
            Transactions.Remove(placeholderTransaction);
            UpdateTotalPrice();
            UpdateNumberOfProducts();
        }

        public void RemoveTransaction(string tempID)
        {
            int ID = int.Parse(tempID.Remove(0, 1));
            SaleTransaction placeholderTransaction = Transactions.Where(x => x.Product is TempProduct && x.Product.ID == ID).First();
            Transactions.Remove(placeholderTransaction);
            UpdateTotalPrice();
            UpdateNumberOfProducts();
        }

        private void UpdateNumberOfProducts()
        {
            NumberOfProducts = 0;
            foreach (Transaction transaction in Transactions)
            {
                NumberOfProducts += transaction.Amount;
            }
        }

        //Returns a list of the products present in the receipt
        public List<BaseProduct> GetProducts()
        {
            List<BaseProduct> products = new List<BaseProduct>();

            foreach (SaleTransaction transaction in Transactions)
            {
                products.Add(transaction.Product);
            }
            return products;
        }

        private SaleTransaction FindTransactionFromProductID(int productID)
        {
            return Transactions.First(x => x.Product.ID == productID);
        }

        public void DiscountOnSingleTransaction(int transID, string inputDiscount)
        {
            SaleTransaction currentSaleTransaction = Transactions.Where(x => x.GetID() == transID).First();

            if (inputDiscount.Contains('%'))
            {
                decimal percentage = Convert.ToDecimal(inputDiscount.Remove(inputDiscount.Length - 1, 1));
                decimal priceInDiscountPrProduct = currentSaleTransaction.Product.SalePrice * (percentage / 100m);
                currentSaleTransaction.DiscountPrice = currentSaleTransaction.Price - currentSaleTransaction.Price * (percentage / 100m);
                
                //currentSaleTransaction.Product.DiscountPrice = currentSaleTransaction.Product.SalePrice - priceInDiscountPrProduct;
            }
            else
            {
                decimal customDiscount = Convert.ToDecimal(inputDiscount);
                currentSaleTransaction.DiscountPrice = currentSaleTransaction.Price - (customDiscount / currentSaleTransaction.Amount);
            }

            currentSaleTransaction.DiscountBool = true;
            UpdateTotalPrice();
        }

        public void Execute()
        {
            UploadToDatabase();
            UpdateNumberOfProducts();

            if (DiscountOnFullReceipt > 0m)
            {
                foreach (SaleTransaction transaction in Transactions)
                {
                    transaction.DiscountBool = true;
                    transaction.DiscountPrice = transaction.Product.DiscountPrice * transaction.Amount;
                    transaction.Product.DiscountBool = true;
                    transaction.Product.DiscountPrice = DiscountOnFullReceipt / NumberOfProducts;
                }
            }
            foreach (SaleTransaction transaction in Transactions)
            {
                transaction.Execute();
            }
            //ReceiptPrinter printReceipt = new ReceiptPrinter(this);
        }

        public static int GetNextID()
        {
            string sql = "SHOW TABLE STATUS LIKE 'receipt'";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            return Convert.ToInt32(Results.RowData[0].Values[10]);
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `receipt` WHERE `id` = '{ID}'";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow_NORECUR(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]);
            NumberOfProducts = Convert.ToInt32(Table.Values[1]);
            TotalPrice = Convert.ToDecimal(Table.Values[2]);
            Date = Convert.ToDateTime(Table.Values[5]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]);
            NumberOfProducts = Convert.ToInt32(Table.Values[1]);
            TotalPrice = Convert.ToDecimal(Table.Values[2]);
            //PaidPrice = Convert.ToDecimal(Table.Values[3]);
            string sql = $"SELECT * FROM `sale_transactions` WHERE `receipt_id` = '{ID}' AND `amount` != 0";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            Transactions = new List<SaleTransaction>();
            foreach (var item in Results.RowData)
            {
                SaleTransaction newSaleTransaction = new SaleTransaction(item);
                Transactions.Add(newSaleTransaction);
            }
            sql = $"SELECT * FROM `payments` WHERE `receipt_id` = '{ID}'";
            TableDecode Pays = Mysql.RunQueryWithReturn(sql);
            Payments = new List<Payment>();
            foreach (var item in Pays.RowData)
            {
                Payment NewPayment = new Payment(item);
                Payments.Add(NewPayment);
            }
            Date = Convert.ToDateTime(Table.Values[3]);
        }

        public void UploadToDatabase()
        {
            int ID = GetNextID();
            this.ID = ID;
            string sql = "INSERT INTO `receipt` (`id`, `number_of_products`, `total_price`)" +
                $" VALUES (NULL, '{NumberOfProducts}', '{TotalPrice}')";
            Mysql.RunQuery(sql);
            foreach (var item in Transactions)
            {
                item.ReceiptID = ID;
                item.UploadToDatabase();
            }
            foreach (var payment in Payments)
            {
                payment.UploadToDatabase();
            }
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `receipt` SET " +
                $"`number_of_products` = '{NumberOfProducts}'," +
                $"`total_price` = '{TotalPrice}'," +
                $"`datetime` = FROM_UNIXTIME('{Utils.GetUnixTime(Date)}') " +
                $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
        }

    }
}
