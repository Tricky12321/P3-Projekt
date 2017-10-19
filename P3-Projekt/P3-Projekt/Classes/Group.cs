using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class Group
    {
        private static int _idCounter = 0;


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
    }
}
