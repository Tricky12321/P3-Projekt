using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace P3_Projekt.Classes.Utilities
{
    public class POSController
    {
        private BoerglumAbbeyStorageandSale _boerglumAbbeyStorageandSale;
        public Receipt PlacerholderReceipt;

        private List<Receipt> ReceiptList = new List<Receipt>();

        public POSController(BoerglumAbbeyStorageandSale boerglumAbbeyStorageandSale)
        {
            _boerglumAbbeyStorageandSale = boerglumAbbeyStorageandSale;
        }

        public void StartPurchase()
        {
            PlacerholderReceipt = new Receipt();
        }

        public void AddSaleTransaction(Product product, int amount)
        {
            PlacerholderReceipt.AddTransaction(new SaleTransaction(product, amount, PlacerholderReceipt.ID));
        }

        public void RemoveTransaction(int productID)
        {
            PlacerholderReceipt.RemoveTransaction(productID);
        }



        public void ExecuteReceipt()
        {
            try
            {
                PlacerholderReceipt.Execute();
            }
            catch(NullReferenceException e)
            {
                Debug.Print(e.Message);
            }
            ReceiptList.Add(PlacerholderReceipt);
        }
    }
}