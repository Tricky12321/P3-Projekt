using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Database;

namespace P3_Projekt.Classes
{
    public class TempProduct : BaseProduct, MysqlObject
    {
        public string Description;
        private bool _resolved;


        public TempProduct (string description, decimal salePrice) : base(salePrice)
        {
            Description = description;
            _resolved = false;
        }

        

        public void Edit(string newDescription, decimal newSalePrice)
        {
            Description = newDescription;
            SalePrice = newSalePrice;
        }

        

        public void Resolve()
        {
            _resolved = true;
        }

        public void GetFromDatabase()
        {
            throw new NotImplementedException();
        }

        public void CreateFromRow(Row Table)
        {
            throw new NotImplementedException();
        }

        public void UploadToDatabase()
        {
            throw new NotImplementedException();
        }

        public void UpdateInDatabase()
        {
            throw new NotImplementedException();
        }

    }
}
