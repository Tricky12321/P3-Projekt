using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    class Group
    {
        private static int _idCounter = 0;


        public int IDGroupCounter;
        public int IDProductCounter = 0;
        public string Name;

        public Group(string name)
        {
            Name = name;
            IDGroupCounter = _idCounter++;
        }
    }
}
