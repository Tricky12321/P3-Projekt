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
        private static string _username = sett.local_or_remote ? sett.lcl_username : sett.rmt_username;
        private static string _password = sett.local_or_remote ? sett.lcl_password : sett.rmt_password;
        private static string _ip = sett.local_or_remote ? sett.lcl_ip : sett.rmt_ip;
        private static int _port = sett.local_or_remote ? sett.lcl_port : sett.rmt_port;
        private static string _database = sett.local_or_remote ? sett.lcl_db : sett.rmt_db;
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
            } else
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
            Stopwatch ConnectionTimer = new Stopwatch();
            ConnectionTimer.Start();
            MySqlConnection connection = null;
            connection = new MySqlConnection(_connectionString);
            connection.Open();
            ////ConnectionTimer.Stop();
            Debug.WriteLine("[DATABASE] Database ConnectionTimer = " + ConnectionTimer.ElapsedMilliseconds + "ms");
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

        public static TableDecodeQueue RunQueryWithReturnQueue(string Query)
        {
            if (!ConnectionWorking)
            {
                return null;
            }
            Stopwatch DatabaseTimer = new Stopwatch();
            DatabaseTimer.Start();
            MySqlConnection connection = Connect();
            DatabaseTimer.Stop();
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

            Debug.WriteLine("[DATABASE] Det tog " + DatabaseTimer.ElapsedMilliseconds + "ms at oprette mysql forbindelse");
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
