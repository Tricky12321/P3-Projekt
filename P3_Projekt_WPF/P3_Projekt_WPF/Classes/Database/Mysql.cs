using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using P3_Projekt_WPF.Classes.Exceptions;
namespace P3_Projekt_WPF.Classes.Database
{
    public class Mysql
    {
        private const string _username = "P3";
        private const string _password = "frankythefish";
        private const string _ip = "v-world.dk";
        private const int _port = 3306;
        private const string _database = "P3";
        public MySqlConnection Connection = null;
        private string _connectionString => $"Server={_ip};Port={_port};Database={_database};Uid={_username};Pwd={_password};";

        public void Disconnect()
        {
            Connection = null;
        }

        public bool Connect()
        {
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

        public void RunQuery(string Query)
        {
            Connect();
            MySqlCommand cmd = Connection.CreateCommand();
            cmd.CommandText = Query;    
            if (Connection == null)
            {
                throw new NotConnectedException("There is no connection to the database");
            }
            cmd.ExecuteReader();
        }

        public TableDecode RunQueryWithReturn(string Query)
        {
            TableDecode TableContent;
            try
            {
                Connect();
                // Hvilken commando skal der køres (Query)
                MySqlCommand cmd = Connection.CreateCommand();
                cmd.CommandText = Query;
                // Åbner forbindelsen til databasen (OPEN)
                MySqlDataReader Reader = cmd.ExecuteReader();
                // Sikre sig at der er noget at hente i databasen.
                if (!Reader.HasRows)
                {
                    throw new EmptyTableException("The Table is empty, are you sure this is what you wanted?");
                }
                TableContent = new TableDecode(Reader);
                Reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Sørger for at vi lukker mysql forbindelsen
                Disconnect();
            }
            return TableContent;
        }
    }
}
