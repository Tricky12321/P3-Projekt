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
        public int CashOrCard;
        public DateTime Date;

        
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
            if (Transactions.Any(x => x.Product == transaction.Product))
            {
                Transaction placeholderTransaction = Transactions.First(x => x.Product == transaction.Product);
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

        //Returns a list of the products present in the receipt
        public List<Product> Products()
        {
            var products = new List<Product>();

            foreach(Transaction transaction in Transactions)
            {
                products.Add(transaction.Product as Product);
            }
            return products;
        }

        private SaleTransaction FindTransactionFromProductID(int productID)
        {
            return Transactions.First(x => x.Product.ID == productID);
        }

        public void Delete()
        {
            // TODO: Delete receipt skal laves? 
            throw new NotImplementedException();
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
            string sql = $"SELECT * FROM `receipt` WHERE `id` = '{ID}'";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]);
            NumberOfProducts = Convert.ToInt32(Table.Values[1]);
            TotalPrice = Convert.ToDecimal(Table.Values[2]);
            PaidPrice = Convert.ToDecimal(Table.Values[3]);
            CashOrCard = Convert.ToInt32(Table.Values[4]);
            string sql = $"SELECT * FROM `sale_transactions` WHERE `receipt_id` = '{ID}' AND `amount` != 0";
            try
            {
                TableDecode Results = Mysql.RunQueryWithReturn(sql);
                Transactions = new List<SaleTransaction>();
                foreach (var item in Results.RowData)
                {
                    SaleTransaction newSaleTransaction = new SaleTransaction(item);
                    Transactions.Add(newSaleTransaction);
                }
            }
            catch (EmptyTableException)
            {

            }

            Date = Convert.ToDateTime(Table.Values[5]);
        }

        public void UploadToDatabase()
        {
            string sql = "INSERT INTO `receipt` (`id`, `number_of_products`, `total_price`, `paid_price`, `payment_method`, `datetime`)"+
                $" VALUES (NULL, '{NumberOfProducts}', '{TotalPrice}', '{PaidPrice}', '{CashOrCard}', FROM_UNIXTIME('{Utils.GetUnixTime(Date)}'));";
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `receipt` SET " +
                $"`number_of_products` = '{NumberOfProducts}'," +
                $"`total_price` = '{TotalPrice}'," +
                $"`paid_price` = '{PaidPrice}'," +
                $"`payment_method` = '{CashOrCard}'," +
                $"`datetime` = FROM_UNIXTIME('{Utils.GetUnixTime(Date)}') "+
                $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
        }
        
    }
}
