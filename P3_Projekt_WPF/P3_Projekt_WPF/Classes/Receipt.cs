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
        public int NumberOfProducts;
        public decimal TotalPrice;
        public decimal PaidPrice;
        public bool CashOrCard;
        public DateTime Date;

        
        public Receipt()
        {
            ID = _idCounter++;
            Date = DateTime.Now;
        }

        public void AddTransaction(SaleTransaction transaction)
        {
            //Checks if product is already in reciept//
            if (IsProductInReceipt(transaction.Product))
            {
                Transaction placeholderTransaction = GetTransactionWithProduct(transaction.Product);
                placeholderTransaction.Edit(placeholderTransaction.Amount += transaction.Amount);
            }
            else
            {
                Transactions.Add(transaction);
            }
            TotalPrice += FindTransactionPrice(transaction);
            UpdateNumberOfProducts();
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = 0;
            foreach (SaleTransaction transaction in Transactions)
            {
                TotalPrice += FindTransactionPrice(transaction);
            }
        }

        private bool IsProductInReceipt(BaseProduct product)
        {
            return Transactions.Any(x => x.Product == product);
        }

        private SaleTransaction GetTransactionWithProduct(BaseProduct product)
        {
            return Transactions.First(x => x.Product == product);
        }

        private decimal FindTransactionPrice(SaleTransaction transaction)
        {
            decimal priceTotal = 0;

            if (transaction.Product is Product)
            {
                if ((transaction.Product as Product).DiscountBool)
                {
                    priceTotal = transaction.Amount * (transaction.Product as Product).DiscountPrice;
                }
                else
                {
                    priceTotal = transaction.Amount * (transaction.Product as Product).SalePrice;
                }
            }
            else if (transaction.Product is TempProduct)
            {
                priceTotal = transaction.Amount * (transaction.Product as TempProduct).SalePrice;
            }
            else if (transaction.Product is ServiceProduct)
            {
                if ((transaction.Product as ServiceProduct).GroupLimit > transaction.Amount)
                {
                    priceTotal += (transaction.Product as ServiceProduct).SalePrice;
                }
                else
                {
                    priceTotal += (transaction.Product as ServiceProduct).GroupPrice;
                }
            }
            else
            {
                throw new WrongProductTypeException("Transaktionens produkt har ikke en valid type!");
            }

            return priceTotal;
        }

        public void RemoveTransaction(int productID)
        {
            SaleTransaction placeholderTransaction = FindTransactionFromProductID(productID);
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

        private SaleTransaction FindTransactionFromProductID(int productID)
        {
            return Transactions.First(x => x.Product.ID == productID);
        }

        public void Delete()
        {
            foreach (Transaction transaction in Transactions)
            {
                /**/
            }
        }

        public void Execute()
        {
            foreach (Transaction transaction in Transactions)
            {
                transaction.Execute();
            }
           //ReceiptPrinter printReceipt = new ReceiptPrinter(this);
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM receipts WHERE id = {ID}";
            Mysql Connection = new Mysql();
            CreateFromRow(Connection.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]);
            NumberOfProducts = Convert.ToInt32(Table.Values[1]);
            TotalPrice = Convert.ToInt32(Table.Values[2]);
            PaidPrice = Convert.ToInt32(Table.Values[3]);
            CashOrCard = Convert.ToBoolean(Table.Values[4]);
            string sql = $"SELECT * FROM `sale_transaction` WHERE `receipt_id` = '{ID}'";
            Mysql Connection = new Mysql();
            TableDecode Results = Connection.RunQueryWithReturn(sql);
            Transactions = new List<SaleTransaction>();
            foreach (var item in Results.RowData)
            {
                SaleTransaction newSaleTransaction = new SaleTransaction(item);
                Transactions.Add(newSaleTransaction);
            }

            //TODO: Datetime skal lige implementeres korrekt (Lasse?)
            //Date = Convert.ToInt32(Table.Values[5]);
        }

        public void UploadToDatabase()
        {

            string sql = "INSERT INTO `receipt` (`id`, `number_of_products`, `total_price`, `paid_price`, `payment_method`, `datetime`)"+
                $" VALUES (NULL, '{NumberOfProducts}', '{TotalPrice}', '{PaidPrice}', '{Utils.ConvertBoolToInt(CashOrCard)}', '2017-10-25 00:00:00');";
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `receipt` SET" +
                $"`number_of_products` = '{NumberOfProducts}'," +
                $"`total_price` = '{TotalPrice}'," +
                $"`paid_price` = '{PaidPrice}'," +
                $"`payment_method` = '{Utils.ConvertBoolToInt(CashOrCard)}'," +
                $"WHERE `id` = {ID};";
            Mysql Connection = new Mysql();
            Connection.RunQuery(sql);
        }
    }
}
