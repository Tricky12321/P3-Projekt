﻿using System;
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
        public static List<MySqlConnection> Connection = new List<MySqlConnection>();
        private static string _connectionString = $"Server={_ip};Port={_port};Database={_database};Uid={_username};Pwd={_password};";
        private static bool _internetConnection = false;
        private static int _connectionCounter => Connection.Count;
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

        public static MySqlConnection Connect()
        {
            CheckInternet();
            MySqlConnection connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                Debug.WriteLine("Database connection: OK!");
                lock (Connection)
                {
                    Connection.Add(connection);
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

        public static void RunQuery_thread(object Query)
        {

            int fails = 0;
            MySqlConnection connection = Connect();
            while (connection == null)
            {
                if (fails < 5)
                {
                    Thread.Sleep(250);
                    fails++;
                    connection = Connect();
                }
                else
                {
                    throw new NotConnectedException();
                }
            }
            try
            {
                string sql = (Query as string);
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    Debug.Print("Running: " + Query);
                    if (Connection == null)
                    {
                        throw new NotConnectedException("Der er ikke forbindelse til databasen");
                    }

                    cmd.ExecuteScalarAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Disconnect(connection);
            }

        }

        public static void RunQuery(string Query)
        {
            Thread SqlThread = new Thread(new ParameterizedThreadStart(RunQuery_thread));
            SqlThread.Start(Query);
        }

        public static TableDecode RunQueryWithReturn(string Query)
        {
            int fails = 0;
            MySqlConnection connection = Connect();
            while (connection == null)
            {
                if (fails < 5)
                {
                    Thread.Sleep(250);
                    fails++;
                    connection = Connect();
                }
                else
                {
                    throw new NotConnectedException();
                }
            }
            TableDecode TableContent;
            try
            {
                // Hvilken commando skal der køres (Query)
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = Query;
                    // Åbner forbindelsen til databasen (OPEN)
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        Debug.Print("Running: " + Query);
                        // Sikre sig at der er noget at hente i databasen.
                        if (!Reader.HasRows)
                        {
                            throw new EmptyTableException("Tabellen man forsøger at forbinde til i databasen er tom?");
                        }
                        TableContent = new TableDecode(Reader);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Disconnect(connection);
            }
            return TableContent;
        }
    }
}
