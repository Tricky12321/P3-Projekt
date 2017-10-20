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
        public static int IDCounter { get { return _idCounter; } set { _idCounter = value; } }
        
        public int ID;
        public List<Transaction> Transactions = new List<Transaction>();
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

        public void AddTransaction(Transaction transaction)
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
            decimal priceTotal = 0;
            foreach(Transaction transaction in Transactions)
            {
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
            }
        }

        private bool IsProductInReceipt(BaseProduct product)
        {
            return Transactions.Any(x => x.Product == product);
        }

        private Transaction GetTransactionWithProduct(BaseProduct product)
        {
            return Transactions.First(x => x.Product == product);
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
