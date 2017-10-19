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
        private int _numberOfProducts;
        public decimal TotalPrice = 0;
        private decimal _paidPrice;
        private string _paymentMethod;
        private DateTime _date;

        public Receipt()
        {
            ID = _idCounter++;
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);

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
            else
            {
                throw new WrongProductTypeException("Transaktionens produkt har ikke en valid type!");
            }

            return priceTotal;
        }

        public void RemoveTransaction(int productID)
        {
            Transactions.RemoveAll(x => x.Product.ID == productID);
        }

        private void FindTransactionFromProductID(int productID)
        {
            throw new NotImplementedException();
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
