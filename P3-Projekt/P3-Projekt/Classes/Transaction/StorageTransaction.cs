using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class StorageTransaction : Transaction
    {
        private StorageRoom _source;
        private StorageRoom _destination;
        private int _amountMove;
        Product _productMove = null;

        public StorageTransaction(Product product, int amount) : base(product, amount)
        {
            
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
