using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using P3_Projekt_WPF.Classes.Database;
namespace P3_Projekt_WPF.Classes
{
    public abstract class BaseProduct : MysqlObject
    {
        public int ID;
        public decimal SalePrice;
        protected static int _idCounter = 0;
        public static int IDCounter { get { return _idCounter; } set { _idCounter = value; } }

        public BaseProduct(decimal salePrice)
        {
            ID = _idCounter++;
            SalePrice = salePrice;
        }

        public abstract string GetName();
        public abstract void GetFromDatabase();
        public abstract void CreateFromRow(Row Table);
        public abstract void UploadToDatabase();
        public abstract void UpdateInDatabase();
    }
}
