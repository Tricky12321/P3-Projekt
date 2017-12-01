using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public static class Admin
    {

        public static void CreateNewPassword (string passwordInput)
        {
            string setQuery = $"UPDATE `admin_password` SET `admin_password` = '{SaltAndHashPassword(passwordInput)}' WHERE `id` = '1'";
            Mysql.RunQuery(setQuery);
        }
        
        // Generates a random salt
        /*
        public static byte[] GenerateSalt (string passwordInput)
        {
            int minSaltSize = 4;
            int maxSaltSize = 8;

            Random random = new Random();
            int saltSize = random.Next(minSaltSize, maxSaltSize);

            byte[] saltBytes = new byte[saltSize];

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(saltBytes);
            }
            return saltBytes;
        }
        */
        public static bool VerifyPassword (string passwordInput)
        {
            string getQuery = $"SELECT `admin_password` FROM `admin_password` WHERE `id` = '1'";
            string storedPassword = Mysql.RunQueryWithReturn(getQuery).RowData[0].Values[0];

            return SaltAndHashPassword(passwordInput).Equals(storedPassword);
        }

        public static string SaltAndHashPassword (string passwordInput)
        {
            string pwdSaltedAndHashed;

            string _salt = "ds312e17frankythefishøæå!#salt";

            string pwdWithSalt = passwordInput + _salt;

            byte[] pwdWithSaltBytes = System.Text.Encoding.UTF8.GetBytes(pwdWithSalt);

            /* Now that the password has been prepared by being salted, we will hash it */
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                byte[] hashedInputBytes = hash.ComputeHash(pwdWithSaltBytes);

                //Converts hashed bytes to text
                //Size is 128, because 512 bits / 8 bits per byte * 2 symbols
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (byte b in hashedInputBytes)
                {
                    //X2 means, that it formats the bytes into hexidecimal
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                }
                pwdSaltedAndHashed = hashedInputStringBuilder.ToString();
            }
            return pwdSaltedAndHashed;
        }
    }
}
