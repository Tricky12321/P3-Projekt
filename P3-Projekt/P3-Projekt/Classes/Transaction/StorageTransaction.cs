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

        public StorageTransaction(Product product, int amount) : base(product, amount)
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

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
