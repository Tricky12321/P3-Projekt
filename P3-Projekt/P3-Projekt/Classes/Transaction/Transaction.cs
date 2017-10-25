using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Database;

namespace P3_Projekt.Classes
{
    public abstract class Transaction : MysqlObject
    {
        protected static int _idCounter = 0;
        public static int IDCounter { get { return _idCounter; } set { _idCounter = value; } }
        
        protected int _id;
        public BaseProduct Product;
        public int Amount;
        public DateTime Date;

        public Transaction(BaseProduct product, int amount)
        {
            Product = product;
            Amount = amount;
            _id = _idCounter++;
            Date = DateTime.Now;
        }

        public virtual void Edit(int newAmount)
        {
            Amount = newAmount;
        }

        public abstract void Execute();

        public void LoadIDValue()
        {
            /* Should load the latest transaction ID from database.
             * Should be called at start of program */
        }

        public abstract void GetFromDatabase();
        public abstract void CreateFromRow(Row Table);
        public abstract void UploadToDatabase();
        public abstract void UpdateInDatabase();
    }
}
