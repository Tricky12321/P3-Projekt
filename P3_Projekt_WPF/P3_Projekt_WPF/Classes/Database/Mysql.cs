using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes.Utilities;
using System.Collections.Concurrent;
using System.Windows;
namespace P3_Projekt_WPF.Classes.Database
{
    public static class Mysql
    {
        private static Properties.Settings sett = Properties.Settings.Default;
        private const bool _debug = true;
        private static string _username = sett.lcl_username;
        private static string _password = sett.lcl_password;
        private static string _ip = sett.lcl_ip;
        private static int _port = sett.lcl_port;
        private static string _database = sett.lcl_db;

        public static List<MySqlConnection> Connection = new List<MySqlConnection>();
        private static string _connectionString = $"Server={_ip};Port={_port};Database={_database};Uid={_username};Pwd={_password};";
        private static bool _internetConnection = false;
        private static int _connectionCounter => Connection.Count;
        private static ConcurrentQueue<string> _queryTasks = new ConcurrentQueue<string>();
        private const int _queryThreadCount = 1;
        private static MySqlConnection _connection = null;
        public static bool ConnectionWorking = false;

        public static void Disconnect(MySqlConnection connection)
        {
            connection.Close();
            lock (Connection)
            {
                Connection.Remove(connection);
            }
        }

        public static void UpdateSettings(Properties.Settings lcl_sett)
        {
            _username = lcl_sett.lcl_username;
            _password = lcl_sett.lcl_password;
            _ip = lcl_sett.lcl_ip;
            _port = lcl_sett.lcl_port;
            _database = lcl_sett.lcl_db;
            _connectionString = $"Server={_ip};Port={_port};Database={_database};Uid={_username};Pwd={_password};";
        }

        public static void UseMockDatabase()
        {
            _database = "P3_mock";
            _username = "P3";
            _password = "frankythefish";
            _port = 40001;
            _ip = "nobelnet.dk";
            _connectionString = $"Server={_ip};Port={_port};Database={_database};Uid={_username};Pwd={_password};";
        }

        private static void CheckInternet()
        {
            if (!_internetConnection)
            {
                _internetConnection = Utils.CheckInternetConnection();
                if (_internetConnection == false)
                {
                    throw new NoInternetConnectionException("Der er ingen forbindelse til serveren");
                }
            }
        }

        public static void CheckDatabaseConnection()
        {
            CheckInternet();
            if (_internetConnection == false)
            {
                MessageBox.Show("Der er ikke forbindelse til internettet!\nDette betyder at programmet ikke kan forbinde til fjern databaser");
            }
            else
            {
                try
                {
                    MySqlConnection Test = Connect();
                    if (Test.State == System.Data.ConnectionState.Open)
                    {
                        Test.Close();
                        ConnectionWorking = true;
                    }
                    else
                    {
                        MessageBox.Show("Der er ingen forbindelse til Databasen, og der vil derfor ikke blive hentet noget data\nSørg for at dine database indstillinger er sat rigtigt op, og genstart programmet");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Der er ingen forbindelse til Databasen, og der vil derfor ikke blive hentet noget data\nSørg for at dine database indstillinger er sat rigtigt op, og genstart programmet");

                }

            }
        }

        public static MySqlConnection Connect(int fails = 0)
        {
            //CheckInternet();
            MySqlConnection connection = null;
            connection = new MySqlConnection(_connectionString);
            connection.Open();
            if (connection == null)
            {
                if (fails < 5)
                {
                    return Connect(fails + 1);
                }
                else
                {
                    throw new NotConnectedException("Der er ingen forbindelse til databasen");
                }
            }
            return connection;
        }

        public static void RunQuery(string Query)
        {
            if (ConnectionWorking)
            {
                using (MySqlConnection connection = Connect())
                {
                    using (MySqlCommand cmd = connection.CreateCommand())
                    {
                        if (_debug)
                        {
                            Debug.WriteLine("SQL: " + Query);
                        }
                        cmd.CommandText = Query;
                        cmd.ExecuteScalarAsync();
                    }
                }
            }
        }

        public static TableDecodeQueue RunQueryWithReturnQueue(string Query)
        {
            if (!ConnectionWorking)
            {
                return null;
            }
            MySqlConnection connection = Connect();
            TableDecodeQueue TableContent;
            lock (connection)
            {
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    if (_debug)
                    {
                        Debug.WriteLine("SQL: " + Query);
                    }

                    cmd.CommandText = Query;

                    using (var Reader = cmd.ExecuteReaderAsync())
                    {
                        TableContent = new TableDecodeQueue(Reader);
                    }
                }
            }
            return TableContent;
        }

        public static TableDecode RunQueryWithReturn(string Query)
        {
            if (!ConnectionWorking)
            {
                return null;
            }
            MySqlConnection connection = Connect();
            TableDecode TableContent;
            try
            {
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    if (_debug)
                    {
                        Debug.WriteLine("SQL: " + Query);
                    }
                    cmd.CommandText = Query;
                    using (var Reader = cmd.ExecuteReaderAsync())
                    {
                        TableContent = new TableDecode(Reader);
                    }
                }
            }
            finally
            {
                Disconnect(connection);
            }
            return TableContent;
        }
    }
}
