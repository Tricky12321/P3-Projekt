using NUnit.Framework;
using P3_Projekt_WPF.Classes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Utilities.Tests
{
    [TestFixture()]
    public class POSControllerTests
    {
        [TearDown]
        public void ResetStatic()
        {
        }

        [Test()]
        public void AddSaleTransactionTest()
        {
            StorageController SC = new StorageController();
            POSController POSC = new POSController(SC);

            POSC.StartPurchase();

            POSC.AddSaleTransaction(new TempProduct("Hello", 3.6M), 7);

            Assert.IsTrue((POSC.PlacerholderReceipt.Transactions.First().TotalPrice == 3.6M*7));
        }

        [Test()]
        public void StartPurchaseTest()
        {
            StorageController SC = new StorageController();
            POSController POSC = new POSController(SC);

            POSC.StartPurchase();

            Assert.IsTrue(POSC.PlacerholderReceipt != null);
        }

        [Test()]
        public void RemoveTransactionFromReceiptTestOneTransaction()
        {
            StorageController SC = new StorageController();
            POSController POSC = new POSController(SC);
            POSC.StartPurchase();
            SC.ProductDictionary.TryAdd(0, new Product(1,"test1", "blabla", 1.25m, 0, false, 5.0m, 3.0m));
            SaleTransaction transaction = new SaleTransaction(SC.ProductDictionary[0], 5, 0);
            POSC.PlacerholderReceipt.Transactions.Add(transaction);

            bool b1 = POSC.PlacerholderReceipt.Transactions.Contains(transaction);

            POSC.RemoveTransactionFromReceipt(1);

            bool b2 = !POSC.PlacerholderReceipt.Transactions.Contains(transaction);

            Assert.IsTrue(b1 && b2);
        }

        [Test()]
        public void RemoveTransactionFromReceiptTestSeveralTransactions()
        {
            StorageController SC = new StorageController();
            POSController POSC = new POSController(SC);
            POSC.StartPurchase();
            SC.ProductDictionary.TryAdd(0, new Product(1,"test1", "blabla", 1.25m, 0, false, 5.0m, 3.0m));
            SC.ProductDictionary.TryAdd(1, new Product(2,"test1", "blabla", 1.25m, 0, false, 5.0m, 3.0m));
            SC.ProductDictionary.TryAdd(2, new Product(3,"test1", "blabla", 1.25m, 0, false, 5.0m, 3.0m));
            SaleTransaction transaction1 = new SaleTransaction(SC.ProductDictionary[0], 5, 0);
            SaleTransaction transaction2 = new SaleTransaction(SC.ProductDictionary[1], 1, 0);
            SaleTransaction transaction3 = new SaleTransaction(SC.ProductDictionary[2], 1, 0);
            POSC.PlacerholderReceipt.Transactions.Add(transaction1);
            POSC.PlacerholderReceipt.Transactions.Add(transaction2);
            POSC.PlacerholderReceipt.Transactions.Add(transaction3);

            bool b1 = POSC.PlacerholderReceipt.Transactions.Contains(transaction1);
            bool b2 = POSC.PlacerholderReceipt.Transactions.Contains(transaction2);
            bool b3 = POSC.PlacerholderReceipt.Transactions.Contains(transaction3);

            POSC.RemoveTransactionFromReceipt(2);

            bool b4 = POSC.PlacerholderReceipt.Transactions.Contains(transaction1);
            bool b5 = !POSC.PlacerholderReceipt.Transactions.Contains(transaction2);
            bool b6 = POSC.PlacerholderReceipt.Transactions.Contains(transaction3);

            Assert.IsTrue(b1 && b2 && b3 && b4 && b5 && b6);
        }
    }
}