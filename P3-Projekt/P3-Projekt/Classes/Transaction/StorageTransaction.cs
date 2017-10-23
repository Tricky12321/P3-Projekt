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

        public StorageTransaction(Product product, int amount, StorageRoom source, StorageRoom destination) : base(product, amount)
        {
            _source = source;
            _destination = destination;
            Amount = amount;
            Product = product;
        }

        public override void Execute()
        {
            (Product as Product).StorageWithAmount[_source] -= Amount;
            (Product as Product).StorageWithAmount[_destination] += Amount;
        }
    }
}
