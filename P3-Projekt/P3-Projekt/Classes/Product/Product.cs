using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt.Classes.Utilities;
using System.Drawing;

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
        private Image _image;
        private Dictionary<StorageRoom, int> _storageWithAmount = new Dictionary<StorageRoom, int>();

        public Product()
        {

        }

        /* No delete method */ 


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
