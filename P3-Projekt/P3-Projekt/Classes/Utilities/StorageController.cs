using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Utilities
{
    class StorageController
    {
        private int _idGroupCounter = 0;

        Dictionary<int, Product> ProductDictionary = new Dictionary<int, Product>();
        Dictionary<int, Group> GroupDictionary = new Dictionary<int, Group>();

    }
}
