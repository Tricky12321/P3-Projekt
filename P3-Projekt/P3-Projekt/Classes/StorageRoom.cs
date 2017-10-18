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
        private string _name;
        private string _description;

        public StorageRoom(string name, string description)
        {
            _name = name;
            _description = description;
            ID = IDCounter++;
        }
    }
}
