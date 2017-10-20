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
        [TearDown] public void ResetStatic()
        {
            StorageRoom.IDCounter = 0;
            BaseProduct.IDCounter = 0;
            Group.IDCounter = 0;
            Transaction.IDCounter = 0;
            Receipt.IDCounter = 0;
        }

        [Test()]
        public void StorageControllerTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DeleteProductTest()
        {
            StorageController storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            StorageRoom testStorage1 = new StorageRoom("3", "medium lager");
            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage1, 10);
            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            storageController.DeleteProduct(0);

            Assert.IsTrue(!storageController.ProductDictionary.ContainsKey(0));
        }

        [Test()]
        public void EditProductTest1()
        {
            StorageController storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            StorageRoom testStorage1 = new StorageRoom("3", "medium lager");
            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage1, 10);
            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            storageController.EditProduct(true, storageController.ProductDictionary[0], "test", "test", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null);

            Assert.IsTrue(storageController.ProductDictionary[0].Name == "test");

        }

        [Test()]
        public void CreateGroupTestOneGroup()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            storageController.CreateGroup("group", "test");

            bool b1 = "group" == storageController.GroupDictionary[1].Name;
            bool b2 = "test" == storageController.GroupDictionary[1].Description;

            Assert.IsTrue(b1 && b2);
        }

        [Test()]
        public void CreateGroupTestThreeGroups()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            storageController.CreateGroup("group1", "test1");
            storageController.CreateGroup("group2", "test2");
            storageController.CreateGroup("group3", "test3");

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
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            Group newGroup = new Group("group", "test");
            storageController.GroupDictionary.Add(newGroup.ID, newGroup);

            storageController.EditGroup(1, "newgroup", "newtest");

            bool b1 = "newgroup" == storageController.GroupDictionary[1].Name;
            bool b2 = "newtest" == storageController.GroupDictionary[1].Description;

            Assert.IsTrue(b1 && b2);
        }

        [Test()]
        public void EditGroupTestThreeGroups()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            Group newGroup1 = new Group("group1", "test1");
            Group newGroup2 = new Group("group2", "test2");
            Group newGroup3 = new Group("group3", "test3");
            storageController.GroupDictionary.Add(newGroup1.ID, newGroup1);
            storageController.GroupDictionary.Add(newGroup2.ID, newGroup2);
            storageController.GroupDictionary.Add(newGroup3.ID, newGroup3);

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
        public void EditProductTest2()
        {

            StorageController storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            StorageRoom testStorage1 = new StorageRoom("3", "medium lager");
            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage1, 10);
            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            storageController.EditProduct(true, storageController.ProductDictionary[0], "test", "test", Convert.ToDecimal(5), testGroup, true, Convert.ToDecimal(2), Convert.ToDecimal(14), null);

            Assert.IsTrue(storageController.ProductDictionary[0].DiscountBool && storageController.ProductDictionary[0].SalePrice == 2 && storageController.ProductDictionary[0].DiscountPrice == 14);
        }
        
        [Test()]
        public void ProductIDTest()
        {
            StorageController storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            StorageRoom testStorage1 = new StorageRoom("3", "medium lager");
            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage1, 10);
            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            Assert.IsTrue(storageController.ProductDictionary[0].GetFullID == "000000");
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

        [TestCase(10, ExpectedResult = 10)]
        [TestCase(15, ExpectedResult = 15)]
        [TestCase(356, ExpectedResult = 356)]
        public int CreateProductTest2(int testInput)
        {
            int test = 0;
            StorageController storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            StorageRoom testStorage = new StorageRoom("medium lager", "medium lager");

            storageController.StorageRoomDictionary.Add(3, testStorage);

            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage, testInput);

            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), null, testPair);

            return test = storageController.ProductDictionary[0].StorageWithAmount[testStorage];
        }

        [Test()]
        public void CreateStorageRoomTestOneProduct()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            var testProduct = new Product("test1", "blabla", 1.25m, new Group("group1", "good group"), false, 5.0m, 3.0m, null);
            storageController.ProductDictionary.Add(testProduct.ID, testProduct);

            storageController.CreateStorageRoom("room1", "test room");

            Assert.AreEqual("room1", storageController.ProductDictionary[0].StorageWithAmount.Keys.First().Name);
        }

        [Test()]
        public void CreateStorageRoomTestThreeProducts()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            var testProduct1 = new Product("test1", "blabla", 1.25m, new Group("group1", "good group"), false, 5.0m, 3.0m, null);
            var testProduct2 = new Product("test2", "blabla", 1.25m, new Group("group2", "good group"), false, 5.0m, 3.0m, null);
            var testProduct3 = new Product("test3", "blabla", 1.25m, new Group("group3", "good group"), false, 5.0m, 3.0m, null);
            storageController.ProductDictionary.Add(testProduct1.ID, testProduct1);
            storageController.ProductDictionary.Add(testProduct2.ID, testProduct2);
            storageController.ProductDictionary.Add(testProduct3.ID, testProduct3);

            storageController.CreateStorageRoom("room1", "test room");

            bool b1 = "room1" == storageController.ProductDictionary[0].StorageWithAmount.Keys.First().Name;
            bool b2 = "room1" == storageController.ProductDictionary[1].StorageWithAmount.Keys.First().Name;
            bool b3 = "room1" == storageController.ProductDictionary[2].StorageWithAmount.Keys.First().Name;

            Assert.IsTrue(b1 && b2 && b3);
        }

        [Test()]
        public void EditStorageRoomTestOneRoom()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            storageController.StorageRoomDictionary.Add(0, new StorageRoom("name", "text"));
            storageController.EditStorageRoom(0, "newname", "newtext");

            bool b1 = "newname" == storageController.StorageRoomDictionary[0].Name;
            bool b2 = "newtext" == storageController.StorageRoomDictionary[0].Description;

            Assert.IsTrue(b1 && b2);
        }

        [Test()]
        public void EditStorageRoomTestThreeRooms()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            storageController.StorageRoomDictionary.Add(0, new StorageRoom("name1", "text1"));
            storageController.StorageRoomDictionary.Add(1, new StorageRoom("name2", "text2"));
            storageController.StorageRoomDictionary.Add(2, new StorageRoom("name3", "text3"));

            storageController.EditStorageRoom(0, "newname1", "newtext1");
            storageController.EditStorageRoom(1, "newname2", "newtext2");
            storageController.EditStorageRoom(2, "newname3", "newtext3");

            bool b1 = "newname1" == storageController.StorageRoomDictionary[0].Name;
            bool b2 = "newtext1" == storageController.StorageRoomDictionary[0].Description;
            bool b3 = "newname2" == storageController.StorageRoomDictionary[1].Name;
            bool b4 = "newtext2" == storageController.StorageRoomDictionary[1].Description;
            bool b5 = "newname3" == storageController.StorageRoomDictionary[2].Name;
            bool b6 = "newtext3" == storageController.StorageRoomDictionary[2].Description;

            Assert.IsTrue(b1 && b2 && b3 && b4 && b5 && b6);
        }

        [Test()]
        public void DeleteStorageRoomTestOneProduct()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            var testProduct = new Product("test1", "blabla", 1.25m, new Group("group1", "good group"), false, 5.0m, 3.0m, null);
            storageController.ProductDictionary.Add(testProduct.ID, testProduct);
            var room = new StorageRoom("test", "test");
            storageController.ProductDictionary[0].StorageWithAmount.Add(room, 0);
            storageController.StorageRoomDictionary.Add(0, room);

            storageController.DeleteStorageRoom(0);

            bool b1 = !storageController.ProductDictionary[0].StorageWithAmount.ContainsKey(room);
            bool b2 = !storageController.StorageRoomDictionary.ContainsKey(0);

            Assert.IsTrue(b1 && b2);
        }

        [Test()]
        public void DeleteStorageRoomTestThreeProducts()
        {
            var storageController = new StorageController(new BoerglumAbbeyStorageandSale());
            var testProduct1 = new Product("test1", "blabla", 1.25m, new Group("group1", "good group"), false, 5.0m, 3.0m, null);
            var testProduct2 = new Product("test2", "blabla", 1.25m, new Group("group2", "good group"), false, 5.0m, 3.0m, null);
            var testProduct3 = new Product("test3", "blabla", 1.25m, new Group("group3", "good group"), false, 5.0m, 3.0m, null);
            storageController.ProductDictionary.Add(testProduct1.ID, testProduct1);
            storageController.ProductDictionary.Add(testProduct2.ID, testProduct2);
            storageController.ProductDictionary.Add(testProduct3.ID, testProduct3);
            var room = new StorageRoom("test", "test");
            storageController.ProductDictionary[0].StorageWithAmount.Add(room, 0);
            storageController.ProductDictionary[1].StorageWithAmount.Add(room, 0);
            storageController.ProductDictionary[2].StorageWithAmount.Add(room, 0);
            storageController.StorageRoomDictionary.Add(0, room);

            storageController.DeleteStorageRoom(0);

            bool b1 = !storageController.ProductDictionary[0].StorageWithAmount.ContainsKey(room);
            bool b2 = !storageController.ProductDictionary[1].StorageWithAmount.ContainsKey(room);
            bool b3 = !storageController.ProductDictionary[2].StorageWithAmount.ContainsKey(room);
            bool b4 = !storageController.StorageRoomDictionary.ContainsKey(0);

            Assert.IsTrue(b1 && b2 && b3 && b4);
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