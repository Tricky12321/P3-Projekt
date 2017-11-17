using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Threading;
namespace P3_Projekt_WPF.Classes.Database
{
    public class TableDecode
    {
        public List<Row> RowData = new List<Row>();
        public int RowCounter = 0;
        public TableDecode(Task<DbDataReader> Reader)
        {
            while (Reader.IsCompleted == false)
            {
                Thread.Sleep(1);
            }
            if (Reader.Result != null)
            {
                
                while (Reader.Result.Read())
                {
                    RowData.Add(new Row());
                    int fieldCount = Reader.Result.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        RowData[RowCounter].Colums.Add(Reader.Result[i].Equals(DBNull.Value) ? String.Empty : Reader.Result.GetName(i));     // Navnet på den kolonne man henter
                        RowData[RowCounter].Values.Add(Reader.Result[i].Equals(DBNull.Value) ? String.Empty : Reader.Result.GetString(i));   // Værdien på den kolonne man henter
                    }
                    RowCounter++;
                }
            }

        }
    }
}
