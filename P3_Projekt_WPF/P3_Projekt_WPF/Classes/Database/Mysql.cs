using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes.Utilities;
namespace P3_Projekt_WPF.Classes.Database
{
    public static class Mysql
    {
        private const string _username = "P3";
        private const string _password = "frankythefish";
        private const string _ip = "v-world.dk";
        private const int _port = 3306;
        private const string _database = "P3";
        public static MySqlConnection Connection = null;
        private static string _connectionString => $"Server={_ip};Port={_port};Database={_database};Uid={_username};Pwd={_password};";

        public static void Disconnect()
        {
            Connection.Close();
            Connection = null;
        }

        public static bool Connect()
        {
            Utils.CheckInternetConnection();
            Connection = new MySqlConnection(_connectionString);
            try
            {
                Connection.Open();
                Debug.WriteLine("Database connection: OK!");
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        //MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        //MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        public static void RunQuery(string Query)
        {
            using (MySqlCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = Query;
                Debug.Print("Running: " + Query);
                if (Connection == null)
                {
                    throw new NotConnectedException("Der er ikke forbindelse til databasen");
                }

                cmd.ExecuteScalar();
            }

        }

        public static TableDecode RunQueryWithReturn(string Query)
        {
            TableDecode TableContent;
            try
            {
                // Hvilken commando skal der køres (Query)
                using (MySqlCommand cmd = Connection.CreateCommand())
                {
                    cmd.CommandText = Query;
                    // Åbner forbindelsen til databasen (OPEN)
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        Debug.Print("Running: " + Query);
                        // Sikre sig at der er noget at hente i databasen.
                        if (!Reader.HasRows)
                        {
                            throw new EmptyTableException("Tabellen man forsøger at forbinde til í databasen er tom?");
                        }
                        TableContent = new TableDecode(Reader);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return TableContent;
        }
    }
}
