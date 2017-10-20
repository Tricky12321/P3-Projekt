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
        public void ProductIDTest()
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


    }
}