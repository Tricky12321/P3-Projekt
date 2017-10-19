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
            StorageRoom testStorage = new StorageRoom("3", "medium lager");
            KeyValuePair<StorageRoom, int> testPair = new KeyValuePair<StorageRoom, int>(testStorage, 10);
             
            Group testGroup = new Group("drikkevarer", "wuhuu drikke");

            storageController.CreateProduct("mælk", "arla", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(7), Convert.ToDecimal(10), testPair);


            Product testProduct = new Product("Sodavand", "harbor", Convert.ToDecimal(5), testGroup, false, Convert.ToDecimal(10), Convert.ToDecimal(7));
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


    }
}