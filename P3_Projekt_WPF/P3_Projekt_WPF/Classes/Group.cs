using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;
namespace P3_Projekt_WPF.Classes
{
    public class Group : MysqlObject
    {
        private static int _idCounter = 0;
        public static int IDCounter { get { return _idCounter; } set { _idCounter = value; } }
        
        public int ID;
        public int IDProductCounter = 0;
        public string Name;
        public string Description;

        public Group(string name, string description)
        {
            Name = name;
            Description = description;
            ID = _idCounter++;
        }

        public Group(Row row)
        {
            CreateFromRow(row);
        }

        public Group(int id)
        {
            ID = id;
            GetFromDatabase();
        }

        public static int GetNextID()
        {
            if (Mysql.ConnectionWorking == false)
            {
                return 0;
            }
            string sql = "SHOW TABLE STATUS LIKE 'groups'";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            return Convert.ToInt32(Results.RowData[0].Values[10]);
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `groups` WHERE `id` = '{ID}'";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]); // id
            Name = Table.Values[1]; // name
            Description = Table.Values[2]; // description
        }

        public void UploadToDatabase()
        {
            string sql = $"INSERT INTO `groups` (`id`, `name`, `description`) VALUES (NULL, '{Name}', '{Description}');";
            Mysql.RunQuery(sql);
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `groups` SET" +
                $"`name` = '{Name}'," +
                $"`description` = '{Description}' " +
                $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
        }
    }
}
