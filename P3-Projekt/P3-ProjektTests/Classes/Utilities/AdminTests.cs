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
    public class AdminTests
    {
        [Test()]
        public void SaltAndHashPasswordTest()
        {
            /* Website used to generate pwd: http://passwordsgenerator.net/sha512-hash-generator/
             * The password used is 'testpassword' + salt */

            string manuallyHashedAndSaltedPwd = "B20ABA938CE60A9959080D92A32FD4424A2BB1ECA0493C5D9B7B5D1CD08BD5B879F133C2EF0317624733CF7125414720ACBEE7BDB19AE6B3EC2D5301A7BE62BF";

            string hashedAndSaltedpwd = P3_Projekt_WPF.Classes.Utilities.Admin.SaltAndHashPassword("testpassword");

            Assert.AreEqual(manuallyHashedAndSaltedPwd, hashedAndSaltedpwd);
        }
    }
}