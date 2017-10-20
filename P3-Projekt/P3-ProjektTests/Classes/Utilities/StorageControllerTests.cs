using NUnit.Framework;
using P3_Projekt.Classes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace P3_Projekt.Classes.Utilities.Tests
{
    [TestFixture()]
    public class StorageControllerTests
    {
        [Test()]
        public void StorageControllerTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DeleteProductTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DeleteGroupTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CreateProductTest()
        {
            StorageController storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            StorageRoom testStorage1 = new StorageRoom("3", "medium lager");
            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage1, 10);

            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            Assert.IsTrue(storageController.ProductDictionary.ContainsKey(0));
        }

        [TestCase(10, 1, ExpectedResult = 10)]
        [TestCase(15, 2, ExpectedResult = 15)]
        [TestCase(356, 3, ExpectedResult = 356)]
        public int CreateProductTest2(int testInput, int testRunTimes)
        {
            int test = 0;
            StorageController storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            StorageRoom testStorage = new StorageRoom("medium lager", "medium lager");


            storageController.StorageRoomDictionary.Add(3, testStorage);

            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage, testInput);


            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            return test = storageController.ProductDictionary[testRunTimes].StorageWithAmount[testStorage];

        }

        [Test()]
        public void EditProductTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CreateStorageRoomTest()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());



            Assert.Fail();
        }

        [TestCase("shir with banas", 3, ExpectedResult = true)]
        [TestCase("bok", 1, ExpectedResult = true)]
        [TestCase("smal blu bid with gren head", 4, ExpectedResult = true)]
        [TestCase("bana", 2, ExpectedResult = false)]
        public bool EvaluateStringLimitTest(string searched, int charDiff)
        {
            StorageController strContr = new StorageController(new BoerglumAbbeyStorageandSale());
            return strContr.EvaluateStringLimit(searched, charDiff);
        }

        [Test()]
        public void ComputeLevenshteinsDistanceTest()
        {
            StorageController strContr = new StorageController(new BoerglumAbbeyStorageandSale());
            Group testGroup = new Group("shirts", "shirts and dresses");
            Product productToBeCompared = new Product("Running shoes", "Adidas", 100, testGroup, false, 20, 50, null);
            string searchedString = "Runin shos";
            int charDifference = strContr.ComputeLevenshteinsDistance(searchedString, productToBeCompared);
            Assert.IsTrue(charDifference == 3);
        }

        [Test()]
        public void LevenshteinsSearchTest()
        {
            StorageController strContr = new StorageController(new BoerglumAbbeyStorageandSale());
            Group testGroup = new Group("shirts", "shirts and dresses");
            Product productToBeCompared = new Product("Running shoes", "Adidas", 100, testGroup, false, 20, 50, null);
            string searchedString = "runin shos";
            List<Product> productList = new List<Product>();

            strContr.LevenshteinsSearch(searchedString, productToBeCompared, ref productList);

            Assert.IsTrue(productList.Contains(productToBeCompared));
        }
    }
}