using NUnit.Framework;
using P3_Projekt_WPF.Classes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
using System.Collections.Concurrent;
namespace P3_Projekt_WPF.Classes.Utilities.Tests
{
    [TestFixture()]
    public class StatisticsControllerTests
    {
        StorageController SC;
        POSController POS;
        StatisticsController StatCon;
        [SetUp]
        public void Init()
        {
            Mysql.UseMockDatabase();
            Mysql.CheckDatabaseConnection();
            SC = new StorageController();
            SC.GetAll();
            SC.LoadAllProductsDictionary();
            SaleTransaction.SetStorageController(SC);
            SaleTransaction.HideMessageBox = true;
            POS = new POSController(SC);
            Utils.GetIceCreameID();
            StatCon = new StatisticsController(SC);

        }

        [TestCase(3, 10, PaymentMethod_Enum.Card, 100)]
        [TestCase(3, 100, PaymentMethod_Enum.Cash, 1337)]
        [TestCase(3, 1000, PaymentMethod_Enum.MobilePay, 4000)]
        public void TestStatistics(int ProductID, int Amount, PaymentMethod_Enum PaymentMethod, decimal PaidAmount)
        {
            DateTime Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime Stop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            StatCon.RequestTodayReceipts();
            StatCon.CalculatePayments();
            StatCon.RequestStatistics(false, 0, false, 0, false, "", Start, Stop);
            StatCon.GenerateProductSalesAndTotalRevenue();
            decimal Kontant_old = Math.Round(StatCon.Payments[0], 2);
            decimal Dankort_old = Math.Round(StatCon.Payments[1], 2);
            decimal MobilePay_old = Math.Round(StatCon.Payments[2], 2);
            BaseProduct TestProd = SC.AllProductsDictionary[ProductID];
            POS.AddSaleTransaction(TestProd, Amount);
            Payment TestPay = new Payment(Receipt.GetNextID(), PaidAmount, PaymentMethod);
            POS.PlacerholderReceipt.Payments.Add(TestPay);
            POS.ExecuteReceipt(false);
            StatCon.RequestTodayReceipts();
            StatCon.CalculatePayments();
            StatCon.GenerateProductSalesAndTotalRevenue();
            decimal Kontant_new = Math.Round(StatCon.Payments[0], 2);
            decimal Dankort_new = Math.Round(StatCon.Payments[1], 2);
            decimal MobilePay_new = Math.Round(StatCon.Payments[2], 2);

            if (PaymentMethod == PaymentMethod_Enum.Cash)
            {
                Assert.IsTrue(Kontant_new == Kontant_old + TestPay.Amount && Dankort_old == Dankort_new && MobilePay_old == MobilePay_new);
            }
            else if (PaymentMethod == PaymentMethod_Enum.Card)
            {
                Assert.IsTrue((Kontant_new + (PaidAmount - (TestProd.SalePrice * Amount))) == Kontant_old && Dankort_new == Dankort_old + TestPay.Amount && MobilePay_old == MobilePay_new);
            }
            else if (PaymentMethod == PaymentMethod_Enum.MobilePay)
            {
                Assert.IsTrue(Kontant_new == Kontant_old && Dankort_old == Dankort_new && MobilePay_new == MobilePay_old + TestPay.Amount);
            }
        }

        [TestCase(3, 10, PaymentMethod_Enum.Card, 100)]
        [TestCase(100,2,PaymentMethod_Enum.Cash,30)]
        [TestCase(1000,2,PaymentMethod_Enum.MobilePay,100)]
        [TestCase(10,10,PaymentMethod_Enum.Card,100)]
        [TestCase(10,1000,PaymentMethod_Enum.Cash,100)]
        [TestCase(100, 1000,PaymentMethod_Enum.MobilePay,100)]
        public void TestStatisticsTempProduct(decimal price, int Amount, PaymentMethod_Enum PaymentMethod, decimal PaidAmount)
        {
            DateTime Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime Stop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); ;
            StatCon.RequestTodayReceipts();
            StatCon.CalculatePayments();
            StatCon.RequestStatistics(false, 0, false, 0, false, "", Start, Stop);
            StatCon.GenerateProductSalesAndTotalRevenue();
            decimal Kontant_old = Math.Round(StatCon.Payments[0], 2);
            decimal Dankort_old = Math.Round(StatCon.Payments[1], 2);
            decimal MobilePay_old = Math.Round(StatCon.Payments[2], 2);
            int TestProdID = TempProduct.GetNextID();
            SC.CreateTempProduct("Test", price, TestProdID);
            BaseProduct TestProd = SC.TempProductDictionary[TestProdID];
            POS.AddSaleTransaction(TestProd, Amount);
            Payment TestPay = new Payment(Receipt.GetNextID(), PaidAmount, PaymentMethod);
            POS.PlacerholderReceipt.Payments.Add(TestPay);
            POS.ExecuteReceipt(false);
            StatCon.RequestTodayReceipts();
            StatCon.CalculatePayments();
            StatCon.GenerateProductSalesAndTotalRevenue();
            decimal Kontant_new = Math.Round(StatCon.Payments[0], 2);
            decimal Dankort_new = Math.Round(StatCon.Payments[1], 2);
            decimal MobilePay_new = Math.Round(StatCon.Payments[2], 2);

            if (PaymentMethod == PaymentMethod_Enum.Cash)
            {
                Assert.IsTrue(Kontant_new == Kontant_old + TestPay.Amount && Dankort_old == Dankort_new && MobilePay_old == MobilePay_new);
            }
            else if (PaymentMethod == PaymentMethod_Enum.Card)
            {
                Assert.IsTrue((Kontant_new + (PaidAmount - (TestProd.SalePrice * Amount))) == Kontant_old && Dankort_new == Dankort_old + TestPay.Amount && MobilePay_old == MobilePay_new);
            }
            else if (PaymentMethod == PaymentMethod_Enum.MobilePay)
            {
                Assert.IsTrue(Kontant_new == Kontant_old && Dankort_old == Dankort_new && MobilePay_new == MobilePay_old + TestPay.Amount);
            }
        }
    }
}