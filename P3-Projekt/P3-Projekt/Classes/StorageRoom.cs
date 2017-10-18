using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class StorageRoom
    {
        private static int IDCounter = 0;

        public int ID;
        public string Name;
        public string Description;

        public StorageRoom(string name, string description)
        {
            Name = name;
            Description = description;
            ID = IDCounter++;
        }
    }
}
