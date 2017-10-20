using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class StorageRoom
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
    }
}
