using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Utilities
{
    public class StorageController
    {
        BoerglumKlosterLagerogSalg _boerglumKlosterLagerogSalg;

        private int _idGroupCounter = 0;

        public Dictionary<int, Product> ProductDictionary = new Dictionary<int, Product>();
        public Dictionary<int, Group> GroupDictionary = new Dictionary<int, Group>();

        public StorageController(BoerglumKlosterLagerogSalg boerglumKlosterLagerogSalg)
        {
            _boerglumKlosterLagerogSalg = boerglumKlosterLagerogSalg;

        }

        public void DeleteProduct(int ProductID)
        {
            ProductDictionary.Remove(ProductID);
        }

        public void DeleteGroup(int GroupID)
        {
            GroupDictionary.Remove(GroupID);
        }
    }
}
