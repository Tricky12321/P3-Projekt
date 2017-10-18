using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    class SaleTransaction : Transaction
    {
        private int _receiptID;
        private bool _isTemp;

        public SaleTransaction(Product product, int amount, int receiptID) : base(product, amount)
        {
            _receiptID = receiptID;
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Edit()
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
