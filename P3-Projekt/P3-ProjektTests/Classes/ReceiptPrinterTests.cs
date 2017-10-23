using NUnit.Framework;
using P3_Projekt.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Tests
{
    [TestFixture()]
    public class ReceiptPrinterTests
    {
        [Test()]
        public void pd_PrintPageTest()
        {
            Receipt receipt = new Receipt();
            Group group = new Group("testgroup", "testgroup");
            Product test1 = new Product("sko", "ok", 50, group, false, 50, 2, null);
            Product test2 = new Product("jakke", "ok", 500, group, false, 500, 2, null);
            Product test3 = new Product("halsterklæde", "ok", 50, group, false, 50, 2, null);
            Product test4 = new Product("flaske", "ok", 5, group, false, 5, 2, null);
            Product test5 = new Product("børste", "ok", 20, group, false, 20, 2, null);
            Product test6 = new Product("pude", "ok", 1000, group, false, 1000, 2, null);
            Product test7 = new Product("slik", "ok", 10, group, false, 10, 2, null);
            Product test8 = new Product("is", "ok", 10, group, false, 10, 2, null);
            SaleTransaction s1 = new SaleTransaction(test1, 2, receipt.ID);
            SaleTransaction s2 = new SaleTransaction(test2, 3, receipt.ID);
            SaleTransaction s3 = new SaleTransaction(test3, 1, receipt.ID);
            SaleTransaction s4 = new SaleTransaction(test4, 1, receipt.ID);
            SaleTransaction s6 = new SaleTransaction(test6, 11, receipt.ID);
            receipt.AddTransaction(s1);
            receipt.AddTransaction(s2);
            receipt.AddTransaction(s3);
            receipt.AddTransaction(s4);
            receipt.AddTransaction(s6);
            receipt.Execute();
            Assert.Fail();
        }
    }
}