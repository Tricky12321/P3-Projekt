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
        public void EditReceiptTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void AddIcecreamTransactionTest()
        {
            StorageController SC = new StorageController();
            POSController POSC = new POSController(SC);

            POSC.StartPurchase();

            POSC.AddIcecreamTransaction(25);

            Assert.IsTrue((POSC.PlacerholderReceipt.Transactions.First().Product as ServiceProduct).ServiceProductGroup.Name == "Is");
        }

        [Test()]
        public void RemoveTransactionFromReceiptTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DeleteTransactionTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ExecuteReceiptTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void AddFreeSaleTransactionTest()
        {
            StorageController SC = new StorageController();
            POSController POSC = new POSController(SC);

            POSC.StartPurchase();

            POSC.AddFreeSaleTransaction(new TempProduct("Hello", 3.6M),7);

            Assert.IsTrue((POSC.PlacerholderReceipt.Transactions.First().TotalPrice == 0));
        }
    }
}