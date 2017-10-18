using NUnit.Framework;
using P3_Projekt.Classes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Utilities.Tests
{
    [TestFixture()]
    public class POSControllerTests
    {
        [Test()]
        public void AddSaleTransactionTest()
        {
            Product product = new Product();
            POSController pos = new POSController();
            pos.StartPurchase();
            pos.AddSaleTransaction(product, 1);


            Assert.IsTrue(pos.PlacerholderReceipt.Transactions.First().Product == product);
        }

        [Test()]
        public void StartPurchaseTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void RemoveProductTest()
        {
            Assert.Fail();
        }
    }
}