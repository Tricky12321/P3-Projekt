using NUnit.Framework;
using P3_Projekt_WPF.Classes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Utilities.Tests
{
    [TestFixture()]
    public class StatisticsControllerTests
    {
        [Test()]
        public void RequestStatisticsTest()
        {
            //Assert.Fail();
        }

        /*[TestCase(2017, 1, 5, 2017, 2, 5)]
        [TestCase(2017, 1, 1, 2017, 1, 2)]
        [TestCase(500, 12, 30, 2000, 6, 30)]
        public void GetUnixTimeTest(int y1, int m1, int d1, int y2, int m2, int d2)
        {
            StatisticsController controller = new StatisticsController();
            DateTime date1 = new DateTime(y1, m1, d1);
            DateTime date2 = new DateTime(y2, m2, d2);
            int unixDate1 = Utils.GetUnixTime(date1);
            int unixDate2 = Utils.GetUnixTime(date2);

            Assert.IsTrue(unixDate1 < unixDate2);
        }*/
    }
}