using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes;
using P3_Projekt_WPF.Classes.Database;
using System.Diagnostics;
namespace PerformanceTest
{
    class Program
    {

        public static void Main(string[] args)
        {
            Product NewProd = new Product(0, "mmmmmmm", "Brand", 1m, 1, false, 1m, 1m);
            StorageController SC_1000 = new StorageController();
            for (int i = 0; i < 1000; i++)
            {
                SC_1000.AllProductsDictionary.TryAdd(i, NewProd);
            }
            StorageController SC_10000 = new StorageController();
            for (int i = 0; i < 10000; i++)
            {
                SC_10000.AllProductsDictionary.TryAdd(i, NewProd);
            }
            StorageController SC_100000 = new StorageController();
            for (int i = 0; i < 100000; i++)
            {
                SC_100000.AllProductsDictionary.TryAdd(i, NewProd);
            }
            StorageController SC_1000000 = new StorageController();
            for (int i = 0; i < 1000000; i++)
            {
                SC_1000000.AllProductsDictionary.TryAdd(i, NewProd);
            }
            StorageController SC_10000000 = new StorageController();
            for (int i = 0; i < 10000000; i++)
            {
                SC_10000000.AllProductsDictionary.TryAdd(i, NewProd);
            }
            Group NewGroup = new Group("HAHAHAHAHAHAHAHAHA", "Test Gruppe");
            SC_1000.GroupDictionary.TryAdd(1, NewGroup);
            SC_10000.GroupDictionary.TryAdd(1, NewGroup);
            SC_100000.GroupDictionary.TryAdd(1, NewGroup);
            SC_1000000.GroupDictionary.TryAdd(1, NewGroup);
            SC_10000000.GroupDictionary.TryAdd(1, NewGroup);
            TestPerformance(SC_1000, "TEST");
            TestPerformance(SC_1000, "TEST TEST");
            TestPerformance(SC_1000, "TEST TEST TEST");
            TestPerformance(SC_1000, "TEST TEST TEST TEST");
            TestPerformance(SC_10000, "TEST");
            TestPerformance(SC_10000, "TEST TEST");
            TestPerformance(SC_10000, "TEST TEST TEST");
            TestPerformance(SC_10000, "TEST TEST TEST TEST");
            TestPerformance(SC_100000, "TEST");
            TestPerformance(SC_100000, "TEST TEST");
            TestPerformance(SC_100000, "TEST TEST TEST");
            TestPerformance(SC_100000, "TEST TEST TEST TEST");
            TestPerformance(SC_1000000, "TEST");
            TestPerformance(SC_1000000, "TEST TEST");
            TestPerformance(SC_1000000, "TEST TEST TEST");
            TestPerformance(SC_1000000, "TEST TEST TEST TEST");
            Console.WriteLine("DONE");
            Console.ReadKey();
        }
        private static decimal _average = 0m;

        public static void TestPerformance(StorageController SC, string SearchString, int time = 1)
        {
            if (time <= 3)
            {
                Stopwatch Timer = new Stopwatch();
                Timer.Start();
                SC.SearchForProduct(SearchString);
                Timer.Stop();
                Console.WriteLine($"Time: {Timer.ElapsedMilliseconds}ms");
                _average += Timer.ElapsedMilliseconds;
                TestPerformance(SC, SearchString, time + 1);
            } else
            {
                _average = _average / 3;
                Console.WriteLine($"C:{SC.AllProductsDictionary.Count()},A:{_average},WC:{SearchString.Split(' ').Count()},SL:{SearchString.Length}\n");
                _average = 0m;
            }


        }
    }
}
