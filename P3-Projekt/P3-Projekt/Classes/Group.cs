using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Database;
namespace P3_Projekt.Classes
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

        public Group(int id)
        {
            ID = id;
            GetFromDatabase();
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM groups WHERE id = {ID}";
            Mysql Connection = new Mysql();
            CreateFromRow(Connection.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]); // id
            Name = Table.Values[1]; // name
            Description = Table.Values[2]; // description
        }

        public void UploadToDatabase()
        {
            throw new NotImplementedException();
        }

        public void UpdateInDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
