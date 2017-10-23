using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using P3_Projekt.Classes.Database;

namespace P3_Projekt.Classes.Utilities
{
    public class POSController
    {
        /*
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * fiks metoden til at loade ID til transaction 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
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

        public void EditReceipt(int receiptID)
        {
            PlacerholderReceipt = ReceiptList.First(x => x.ID == receiptID);
            PlacerholderReceipt.Delete();
        }

        public void AddSaleTransaction(Product product, int amount)
        {
            PlacerholderReceipt.AddTransaction(new SaleTransaction(product, amount, PlacerholderReceipt.ID));
        }

        public void RemoveTransactionFromReceipt(int productID)
        {
            PlacerholderReceipt.RemoveTransaction(productID);
        }

        public void DeleteTransaction(int transactionID)
        {
            /* TODO: Vær sikker på hvad transactions tabellen skal hedde!! */
            string deleteQuery = $"DELETE FROM transactions WHERE ID = {transactionID}";
            Mysql Connection = new Mysql();
            Connection.RunQuery(deleteQuery);
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
            PlacerholderReceipt = null;
        }
    }
}