using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Database;

namespace P3_Projekt_WPF.Classes
{
    public abstract class Transaction : MysqlObject
    {
        protected int _id;
        public BaseProduct Product;
        public int Amount;
        public DateTime Date;

        public Transaction(BaseProduct product, int amount)
        {
            Product = product;
            Amount = amount;
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
