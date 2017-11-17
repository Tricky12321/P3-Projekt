using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Data.Common;
using System.Threading;
namespace P3_Projekt_WPF.Classes.Database
{
    public class TableDecodeQueue
    {
        public int RowCounter = 0;
        public ConcurrentQueue<Row> RowData = new ConcurrentQueue<Row>();
        public TableDecodeQueue(Task<DbDataReader> Reader)
        {
            if (Reader != null)
            {
                while (Reader.IsCompleted == false)
                {
                    Thread.Sleep(1);
                }
                Stopwatch Timer = new Stopwatch();
                Timer.Start();
                while (Reader.Result.Read())
                {
                    int fieldCount = Reader.Result.FieldCount;
                    Row NewRow = new Database.Row();

                    for (int i = 0; i < fieldCount; i++)
                    {
                        //NewRow.Colums.Add(Reader.Result[i].Equals(DBNull.Value) ? String.Empty : Reader.GetName(i));
                        NewRow.Values.Add(Reader.Result[i].Equals(DBNull.Value) ? String.Empty : Reader.Result.GetString(i));
                    }
                    RowData.Enqueue(NewRow);
                    RowCounter++;
                }
                Timer.Stop();
                Debug.WriteLine("Database call took " + Timer.ElapsedMilliseconds + "ms to process");
            }

        }
    }
}
