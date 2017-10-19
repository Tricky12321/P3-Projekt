using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Exceptions;

namespace P3_Projekt.Classes
{
    public class Receipt
    {
        private static int _idCounter = 0;

        public int ID;
        public List<Transaction> Transactions = new List<Transaction>();
        public int NumberOfProducts;
        public decimal TotalPrice = 0;
        public decimal PaidPrice;
        public bool CashOrCard;
        public DateTime Date;

        public Receipt()
        {
            ID = _idCounter++;
            Date = DateTime.Now;
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            TotalPrice += FindTransactionPrice(transaction);
            UpdateNumberOfProducts();
        }

        private decimal FindTransactionPrice(Transaction transaction)
        {
            decimal priceTotal = 0;

            if (transaction.Product is Product)
            {
                if ((transaction.Product as Product).DiscountBool)
                {
                    priceTotal = transaction.Amount * (transaction.Product as Product).DiscountPrice;
                }
                else if (!(transaction.Product as Product).DiscountBool)
                {
                    priceTotal = transaction.Amount * (transaction.Product as Product).SalePrice;
                }
                throw new ProductDiscountException("Kunne ikke finde Discount Bool i Produkt!");
            }
            else if (transaction.Product is TempProduct)
            {
                priceTotal = transaction.Amount * (transaction.Product as TempProduct).SalePrice;
            }
            else if (transaction.Product is ServiceProduct)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new WrongProductTypeException("Transaktionens produkt har ikke en valid type!");
            }

            return priceTotal;
        }

        public void RemoveTransaction(int productID)
        {
            Transaction placeholderTransaction = FindTransactionFromProductID(productID);
            Transactions.Remove(placeholderTransaction);
            TotalPrice -= FindTransactionPrice(placeholderTransaction);
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

        private Transaction FindTransactionFromProductID(int productID)
        {
            return Transactions.First(x => x.Product.ID == productID);
        }

        public void Delete()
        {
            foreach (Transaction transaction in Transactions)
            {
                transaction.Delete();
            }
        }


        public void Execute()
        {
            foreach (Transaction transaction in Transactions)
            {
                transaction.Execute();
            }
        }
    }
}
