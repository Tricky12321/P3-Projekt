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
using P3_Projekt_WPF.Classes;
namespace P3_ProjektTests.Classes.Utilities
{
    [TestFixture()]
    public class UtilsTest
    {
        [TestCase("1234", ExpectedResult = true)]
        [TestCase("asdf", ExpectedResult = false)]
        [TestCase("hello with you", ExpectedResult = false)]
        [TestCase("12345678901234567890", ExpectedResult = true)]
        [TestCase("h", ExpectedResult = false)]
        public bool RegexCheckNumbersTest(string StringToCheck)
        {
            return !Utils.RegexCheckNumber(StringToCheck);
        }

        [TestCase("1234", ExpectedResult = true)]
        [TestCase("asdf", ExpectedResult = false)]
        [TestCase("hello with you", ExpectedResult = false)]
        [TestCase("12345678901234567890", ExpectedResult = true)]
        [TestCase("h", ExpectedResult = false)]
        [TestCase("1234,12", ExpectedResult = true)]
        [TestCase("1234,123", ExpectedResult = false)]
        [TestCase("1234.12", ExpectedResult = false)]
        [TestCase("asdf,12", ExpectedResult = false)]
        [TestCase("123,as", ExpectedResult = false)]
        public bool RegexCheckDecimalTest(string StringToCheck)
        {
            return !Utils.RegexCheckDecimal(StringToCheck);
        }


        [TestCase("Blå", ExpectedResult = 1)]
        [TestCase("Rød", ExpectedResult = 1)]
        [TestCase("asdf", ExpectedResult = 0)]
        [TestCase("1234", ExpectedResult = 0)]
        [TestCase("1", ExpectedResult = 1)]
        [TestCase("Rød Fugl", ExpectedResult = 2)]
        [TestCase("Glas", ExpectedResult = 2)]
        [TestCase("Fugle", ExpectedResult = 2)]
        [TestCase("fuelg", ExpectedResult = 2)]
        [TestCase("Fugl", ExpectedResult = 2)]
        [TestCase("BLÅ FUGL", ExpectedResult = 2)]
        [TestCase("BLÅFUGL", ExpectedResult = 1)]
        [TestCase("blåfugl", ExpectedResult = 1)]
        [TestCase("blåaføgl", ExpectedResult = 1)]
        [TestCase("blaafuel", ExpectedResult = 1)]
        [TestCase("ød Fuel", ExpectedResult = 2)]
        [TestCase("ød  Fuel", ExpectedResult = 2)]
        [TestCase("ød     Fuel", ExpectedResult = 2)]
        [TestCase("FUGL", ExpectedResult = 2)]
        [TestCase("pind", ExpectedResult = 2)]
        [TestCase("SLIK", ExpectedResult = 2)]
        [TestCase("slIk", ExpectedResult = 2)]
        [TestCase("snacks", ExpectedResult = 3)]
        [TestCase("smag", ExpectedResult = 1)]
        [TestCase("Slikke pind", ExpectedResult = 2)]
        [TestCase("shoppen", ExpectedResult = 0)]
        [TestCase(".dk", ExpectedResult = 3)]
        [TestCase("folket", ExpectedResult = 0)]
        [TestCase("til folket", ExpectedResult = 0)]
        [TestCase("A/S", ExpectedResult = 2)]
        [TestCase("græs", ExpectedResult = 1)]

        public int SearchAlgorithm(string SearchString)
        {
            ConcurrentDictionary<int, BaseProduct> ProductList = new ConcurrentDictionary<int, BaseProduct>();
            ConcurrentDictionary<int, Group> GroupList = new ConcurrentDictionary<int, Group>();
            ProductList.TryAdd(1, new Product(1, "Blå Fugl", "Glas Fugle A/S", 0m, 1, false, 0m, 0m));
            ProductList.TryAdd(2, new Product(2, "Rød Fugl", "Glas Fugle A/S", 0m, 1, false, 0m, 0m));
            ProductList.TryAdd(3, new Product(3, "Slikkepind", "SnacksShoppen.dk", 0m, 2, false, 0m, 0m));
            ProductList.TryAdd(4, new Product(4, "Sandwich", "SnacksShoppen.dk", 0m, 2, false, 0m, 0m));
            ProductList.TryAdd(5, new Product(5, "Slikkepind m. græs smag", "SnacksShoppen.dk", 0m, 2, false, 0m, 0m));
            GroupList.TryAdd(1, new Group("Glas", "Glas varer"));
            GroupList.TryAdd(2, new Group("Snacks", "Snacks til folket"));
            StorageController SC = new StorageController();
            SC.AllProductsDictionary = ProductList;
            SC.GroupDictionary = GroupList;
            return SC.SearchForProduct(SearchString).Count();
        }


    }
}
