using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Utilities
{
    class StorageListItem
    {
        public StorageListItem (string name, int numberStored, int id)
        {
            Name = name;
            NumberStored = numberStored;
            ID = id;
        }

        public string Name { get; set; }
        public int NumberStored { get; set; }
        public int ID { get; set; }
    }
}
