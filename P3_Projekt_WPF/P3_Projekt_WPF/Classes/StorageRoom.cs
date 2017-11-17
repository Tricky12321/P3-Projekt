using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
namespace P3_Projekt_WPF.Classes
{
    public class StorageRoom : MysqlObject
    {
        private static int _idCounter = 0;
        public static int IDCounter { get { return _idCounter; } set { _idCounter = value; } }

        public int ID;
        public string Name;
        public string Description;

        public StorageRoom(string name, string description)
        {
            Name = name;
            Description = description;
            ID = _idCounter++;
        }

        public StorageRoom(int id)
        {
            ID = id;
            GetFromDatabase();
        }

        public StorageRoom(Row row)
        {
            CreateFromRow(row);
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `storagerooms` WHERE `id` = '{ID}'";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]);
            Name = Table.Values[1];
            Description = Table.Values[2];
        }

        public void UploadToDatabase()
        {
            string sql = "INSERT INTO `storagerooms` (`id`, `name`, `description`)"+
                $" VALUES (NULL, '{Name}', '{Description}');";
            Mysql.RunQuery(sql);
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `storagerooms` SET" +
               $"`name` = '{Name}'," +
               $"`description` = '{Description}' " +
               $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
        }
    }
}
