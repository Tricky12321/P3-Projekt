using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Utilities;

namespace P3_Projekt.Classes
{
    public class Product : BaseProduct
    {
        public string Name;
        private string _brand;
        private decimal _purchasePrice;
        private string _group;
        private bool _discount;
        private decimal _discountPrice;
        private int _inStock;
        private StorageRoom _storageRoom;


        public Product()
        {

        }



        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Edit()
        {
            throw new NotImplementedException();
        }

        public override void Deposit()
        {
            throw new NotImplementedException();
        }

        public override void Withdraw()
        {
            throw new NotImplementedException();
        }

        public override void Move()
        {
            throw new NotImplementedException();
        }
    }
}
