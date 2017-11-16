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
using System.Collections.Concurrent;
namespace P3_Projekt_WPF.Classes.Database
{
    public static class Mysql
    {
        private const bool _debug = true;
        private const string _username = "P3";
        private const string _password = "frankythefish";
        private const bool _remoteDB = true;
        private const string _ip = _remoteDB ? "v-world.dk" : "192.168.2.1";
        private const int _port = 3306;
        private const string _database = "P3";
        public static List<MySqlConnection> Connection = new List<MySqlConnection>();
        private static string _connectionString = $"Server={_ip};Port={_port};Database={_database};Uid={_username};Pwd={_password};";
        private static bool _internetConnection = false;
        private static int _connectionCounter => Connection.Count;
        private static List<Thread> _queryThreads = new List<Thread>();
        private static ConcurrentQueue<string> _queryTasks = new ConcurrentQueue<string>();
        private const int _queryThreadCount = 1;

        public static void Disconnect(MySqlConnection connection)
        {
            connection.Close();
            lock (Connection)
            {
                Connection.Remove(connection);
            }
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

        public static MySqlConnection Connect(int fails = 0)
        {
            CheckInternet();
            MySqlConnection connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                if (connection == null)
                {
                    if (fails < 5)
                    {
                        return Connect();
                    }
                    else
                    {
                        throw new NotConnectedException("Der er ingen forbindelse til databasen");
                    }
                }
                return connection;
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
                return null;
            }
        }

        public static void RunQuery(string Query)
        {
            using (MySqlConnection connection = Connect())
            {
                try
                {
                    using (MySqlCommand cmd = connection.CreateCommand())
                    {
                        if (_debug)
                        {
                            Debug.WriteLine("SQL: "+Query);
                        }
                        cmd.CommandText = Query;
                        cmd.ExecuteScalarAsync();
                    }
                }
                finally
                {
                    Disconnect(connection);
                }
            }
        }


        public static TableDecode RunQueryWithReturn(string Query)
        {
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
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
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
