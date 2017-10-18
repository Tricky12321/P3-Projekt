using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Utilities
{
    public class POSController
    {
        private Receipt _placerholderReceipt;

        private List<Receipt> ReceiptList = new List<Receipt>();

        public POSController()
        {
            
        }

        public void StartPurchase()
        {
            _placerholderReceipt = new Receipt();
        }

        public void AddProduct(Product product, int amount)
        {
            _placerholderReceipt.AddTransaction(new SaleTransaction(product, amount, _placerholderReceipt.ID));
        }

        public void RemoveProduct(int productID)
        {
            _placerholderReceipt.Transactions.RemoveAll(x => x.Product.ID == productID);
        }

    }
}