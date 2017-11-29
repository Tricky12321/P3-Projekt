using NUnit.Framework;
using P3_Projekt_WPF.Classes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
namespace P3_Projekt_WPF.Classes.Utilities.Tests
{

    [TestFixture()]
    public class StorageControllerTests
    {
        [Test()]
        public void CreateGroupTestOneGroup()
        {
            var storageController = new StorageController();
            storageController.CreateGroup(1,"group", "test");

            bool b1 = "group" == storageController.GroupDictionary[1].Name;
            bool b2 = "test" == storageController.GroupDictionary[1].Description;

            Assert.IsTrue(b1 && b2);
        }

        [Test()]
        public void CreateGroupTestThreeGroups()
        {
            var storageController = new StorageController();
            storageController.CreateGroup(1,"group1", "test1");
            storageController.CreateGroup(2,"group2", "test2");
            storageController.CreateGroup(3,"group3", "test3");

            bool b1 = "group1" == storageController.GroupDictionary[1].Name;
            bool b2 = "test1" == storageController.GroupDictionary[1].Description;
            bool b3 = "group2" == storageController.GroupDictionary[2].Name;
            bool b4 = "test2" == storageController.GroupDictionary[2].Description;
            bool b5 = "group3" == storageController.GroupDictionary[3].Name;
            bool b6 = "test3" == storageController.GroupDictionary[3].Description;

            Assert.IsTrue(b1 && b2 && b3 && b4 && b5 && b6);
        }

        [Test()]
        public void EditGroupTestOneGroup()
        {
            var storageController = new StorageController();
            Group newGroup = new Group("group", "test");
            newGroup.ID = 1;
            storageController.GroupDictionary.TryAdd(newGroup.ID, newGroup);

            storageController.EditGroup(1, "newgroup", "newtest");

            bool b1 = "newgroup" == storageController.GroupDictionary[1].Name;
            bool b2 = "newtest" == storageController.GroupDictionary[1].Description;

            Assert.IsTrue(b1 && b2);
        }

        [Test()]
        public void EditGroupTestThreeGroups()
        {
            var storageController = new StorageController();
            Group newGroup1 = new Group("group1", "test1");
            Group newGroup2 = new Group("group2", "test2");
            Group newGroup3 = new Group("group3", "test3");
            newGroup1.ID = 1;
            newGroup2.ID = 2;
            newGroup3.ID = 3;
            storageController.GroupDictionary.TryAdd(newGroup1.ID, newGroup1);
            storageController.GroupDictionary.TryAdd(newGroup2.ID, newGroup2);
            storageController.GroupDictionary.TryAdd(newGroup3.ID, newGroup3);

            storageController.EditGroup(1, "newgroup1", "newtest1");
            storageController.EditGroup(2, "newgroup2", "newtest2");
            storageController.EditGroup(3, "newgroup3", "newtest3");

            bool b1 = "newgroup1" == storageController.GroupDictionary[1].Name;
            bool b2 = "newtest1" == storageController.GroupDictionary[1].Description;
            bool b3 = "newgroup2" == storageController.GroupDictionary[2].Name;
            bool b4 = "newtest2" == storageController.GroupDictionary[2].Description;
            bool b5 = "newgroup3" == storageController.GroupDictionary[3].Name;
            bool b6 = "newtest3" == storageController.GroupDictionary[3].Description;

            Assert.IsTrue(b1 && b2 && b3 && b4 && b5 && b6);
        }

        [Test()]
        public void DeleteGroupTestOneGroup()
        {
            var storageController = new StorageController();
            storageController.CreateGroup(1, "group", "test");
            storageController.CreateGroup(0, "Diverse", "test");
            var testProduct1 = new Product(1,"test1", "blabla", 1.25m, 1, false, 5.0m, 3.0m);
            var testProduct2 = new Product(2,"test2", "blabla", 1.25m, 1, false, 5.0m, 3.0m);
            var testProduct3 = new Product(3,"test3", "blabla", 1.25m, 1, false, 5.0m, 3.0m);
            storageController.ProductDictionary.TryAdd(testProduct1.ID, testProduct1);
            storageController.ProductDictionary.TryAdd(testProduct2.ID, testProduct2);
            storageController.ProductDictionary.TryAdd(testProduct3.ID, testProduct3);

            storageController.DeleteGroup(1);

            bool b1 = storageController.GroupDictionary.ContainsKey(1);

            Assert.IsTrue(!b1);
        }

        [Test()]
        public void DeleteGroupTestTwoGroups()
        {
            var storageController = new StorageController();
            Group newGroup1 = new Group("group", "test");
            newGroup1.ID = 1;
            Group newGroup2 = new Group("group2", "test");
            newGroup2.ID = 2;
            storageController.GroupDictionary.TryAdd(newGroup1.ID, newGroup1);
            storageController.GroupDictionary.TryAdd(newGroup2.ID, newGroup2);
            var testProduct1 = new Product(1,"test1", "blabla", 1.25m, newGroup1.ID, false, 5.0m, 3.0m);
            var testProduct2 = new Product(2,"test2", "blabla", 1.25m, newGroup2.ID, false, 5.0m, 3.0m);
            var testProduct3 = new Product(3,"test3", "blabla", 1.25m, newGroup2.ID, false, 5.0m, 3.0m);
            storageController.ProductDictionary.TryAdd(testProduct1.ID, testProduct1);
            storageController.ProductDictionary.TryAdd(testProduct2.ID, testProduct2);
            storageController.ProductDictionary.TryAdd(testProduct3.ID, testProduct3);

            storageController.DeleteGroup(3);

            bool b1 = storageController.ProductDictionary[1].ProductGroupID == storageController.GroupDictionary[1].ID;
            bool b2 = storageController.ProductDictionary[2].ProductGroupID == storageController.GroupDictionary[2].ID;
            bool b3 = storageController.ProductDictionary[3].ProductGroupID == storageController.GroupDictionary[2].ID;

            Assert.IsTrue(b1 && b2 && b3);
        }

        [Test()]
        public void DeleteGroupAndMoveTest()
        {
            var storageController = new StorageController();
            Group newGroup1 = new Group("group", "test");
            newGroup1.ID = 1;
            Group newGroup2 = new Group("group2", "test");
            newGroup2.ID = 2;
            storageController.GroupDictionary.TryAdd(newGroup1.ID, newGroup1);
            storageController.GroupDictionary.TryAdd(newGroup2.ID, newGroup2);
            var testProduct1 = new Product(1,"test1", "blabla", 1.25m, newGroup1.ID, false, 5.0m, 3.0m);
            var testProduct2 = new Product(2,"test2", "blabla", 1.25m, newGroup1.ID, false, 5.0m, 3.0m);
            var testProduct3 = new Product(3,"test3", "blabla", 1.25m, newGroup1.ID, false, 5.0m, 3.0m);
            storageController.ProductDictionary.TryAdd(testProduct1.ID, testProduct1);
            storageController.ProductDictionary.TryAdd(testProduct2.ID, testProduct2);
            storageController.ProductDictionary.TryAdd(testProduct3.ID, testProduct3);

            storageController.DeleteGroupAndMove(1, 2);

            bool b1 = storageController.ProductDictionary[1].ProductGroupID == storageController.GroupDictionary[2].ID;
            bool b2 = storageController.ProductDictionary[2].ProductGroupID == storageController.GroupDictionary[2].ID;
            bool b3 = storageController.ProductDictionary[3].ProductGroupID == storageController.GroupDictionary[2].ID;

            Assert.IsTrue(b1 && b2 && b3);
        }

        /*[Test()]
        public void ProductIDTest()
        {
            StorageController storageController = new StorageController();
            StorageRoom testStorage1 = new StorageRoom("3", "medium lager");
            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage1, 10);

            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            Assert.IsTrue(storageController.ProductDictionary[0].GetFullID == "010000");
        }*/


        /*
        [TestCase("shir with banas", 3, ExpectedResult = true)]
        [TestCase("bok", 1, ExpectedResult = true)]
        [TestCase("smal blu bid with gren head", 4, ExpectedResult = true)]
        [TestCase("bana", 2, ExpectedResult = false)]
        public bool EvaluateStringLimitTest(string searched, int charDiff)
        {
            StorageController strContr = new StorageController();
            return strContr.EvaluateStringLimit(searched, charDiff);
        }

        [Test()]
        public void ComputeLevenshteinsDistanceTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("shirts", "shirts and dresses");
            Product productToBeCompared = new Product(1, "Running shoes", "Adidas", 100, testGroup.ID, false, 20, 50);
            string searchedString = "Runin shos";
            int charDifference = strContr.ComputeLevenshteinsDistance(searchedString, productToBeCompared.Name);
            Assert.IsTrue(charDifference == 3);
        }
        */
        /*
        [Test()]
        public void LevenshteinsProductSearchTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("shirts", "shirts and dresses");
            Product productToBeCompared = new Product("Running shoes", "Adidas", 100, testGroup.ID, false, 20, 50);
            string searchedString = "runin shos";
            ConcurrentQueue<Product> productList = new ConcurrentQueue<Product>();

            strContr.LevenshteinsProductSearch(searchedString, productToBeCompared, ref productList);

            Assert.IsTrue(productList.Contains(productToBeCompared));
        }

        [Test()]
        public void LevenshteinsGroupSearchTrueTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("shirts", "shirts and dresses");
            string[] searchedString = { "sirts", "random" };
            bool groupWasMatched = strContr.LevenshteinsGroupSearch(searchedString, testGroup);

            Assert.IsTrue(groupWasMatched == true);
        }
        [Test()]
        public void LevenshteinsGroupSearchFalseTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("shirts", "shirts and dresses");
            string[] searchedString = { "dresses" };
            bool groupWasMatched = strContr.LevenshteinsGroupSearch(searchedString, testGroup);

            Assert.IsTrue(groupWasMatched == false);
        }

        [Test()]
        public void LevenshteinsBrandSearchTrueTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("shirts", "shirts and dresses");
            Product productToBeCompared = new Product("Running shoes", "Adidas", 100, testGroup.ID, false, 20, 50, null);
            string[] searchedString = { "shoes", "affidas" };
            bool brandWasMatched = strContr.LevenshteinsBrandSearch(searchedString, productToBeCompared.Brand);

            Assert.IsTrue(brandWasMatched == true);
        }

        [Test()]
        public void LevenshteinsBrandSearchFalseTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("shirts", "shirts and dresses");
            Product productToBeCompared = new Product("Running shoes", "Adidas", 100, testGroup.ID, false, 20, 50, null);
            string[] searchedString = { "shoes", "nike" };
            bool brandWasMatched = strContr.LevenshteinsBrandSearch(searchedString, productToBeCompared.Brand);

            Assert.IsTrue(brandWasMatched == false);
        }
        */

        /*
        [Test()]
        public void GroupSearchTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("Adidas", "shoes and clothes");
            strContr.GroupDictionary.Add(101, testGroup);
            Product productToBeCompared = new Product("Running trousers", "Adidas", 100, testGroup.ID, false, 20, 50, null);
            strContr.ProductDictionary.Add(productToBeCompared.ID, productToBeCompared);
            string searchedString = "trousers Affidas";
            List<Product> productList = new List<Product>();

            strContr.GroupSearch(searchedString, ref productList);

            Assert.IsTrue(productList.Contains(productToBeCompared));
        }
        */
        /*
        [Test()]
        public void BrandSearchTest()
        {
            StorageController strContr = new StorageController();
            Group testGroup = new Group("Adidas", "shoes and clothes");
            Product productToBeCompared = new Product("Running shoes", "Adidas", 100, testGroup.ID, false, 20, 50, null);
            strContr.ProductDictionary.Add(100, productToBeCompared);
            string searchedString = "shoes Affidas";
            List<Product> productList = new List<Product>();
            strContr.BrandSearch(searchedString, ref productList);
            Assert.IsTrue(productList.Contains(productToBeCompared));
        }
        */
        /*
        [Test()]
        public void CreateTempProductTest()
        {
            StorageController strContr = new StorageController();
            string productDescription = "A blue shirt with yellow bananas";
            decimal sellPrice = 100;
            strContr.CreateTempProduct(productDescription, sellPrice);
            //TODO: Exists does not exist
            //Assert.IsTrue(strContr.TempProductList.Exists(x => x.Description == "A blue shirt with yellow bananas"));
        }
        */
        [Test()]
        public void ContainsSearchTest()
        {
            StorageController SC = new StorageController();
            BaseProduct TestProduct = new Product(1, "TestProductMedSlagI", "Aner det ikke", 10m, 1, false, 0m, 0m);
            if (SC.ContainsSearch("sLaG", TestProduct))
            {
                Assert.Pass();
            } else
            {
                Assert.Fail();
            }
        }
        [Test()]
        public void ContainsSearchTest5()
        {
            StorageController SC = new StorageController();
            BaseProduct TestProduct = new Product(1, "TestProductMedSlagI", "Aner det ikke", 10m, 1, false, 0m, 0m);
            if (SC.ContainsSearch("asdf", TestProduct))
            {
                Assert.Fail();
            }
            else
            {
                Assert.Pass();
            }
        }
        [Test()]
        public void ContainsSearchTest2()
        {
            StorageController SC = new StorageController();
            BaseProduct TestProduct = new Product(1, "TestProductMedSlagI", "Aner det ikke", 10m, 1, false, 0m, 0m);
            if (SC.ContainsSearch("Product", TestProduct))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test()]
        public void ContainsSearchTest3()
        {
            StorageController SC = new StorageController();
            BaseProduct TestProduct = new Product(1, "asdf", "Aner det ikke", 10m, 1, false, 0m, 0m);
            if (SC.ContainsSearch("Product", TestProduct))
            {
                Assert.Fail();
            }
            else
            {
                Assert.Pass();
            }
        }
        [Test()]
        public void ContainsSearchTest4()
        {
            StorageController SC = new StorageController();
            BaseProduct TestProduct = new Product(1, "TestProductTing", "Aner det ikke", 10m, 1, false, 0m, 0m);
            if (SC.ContainsSearch("", TestProduct))
            {
                Assert.Fail();
            }
            else
            {
                Assert.Pass();
            }
        }

    }
}