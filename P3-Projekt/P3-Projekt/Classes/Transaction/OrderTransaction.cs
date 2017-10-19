using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    public class OrderTransaction : Transaction
    {
        private int _purchasePrice;
        private string _supplier;

        public OrderTransaction(Product product, int amount) : base(product, amount)
        {
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
