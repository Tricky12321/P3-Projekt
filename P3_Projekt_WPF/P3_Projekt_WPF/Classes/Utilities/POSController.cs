using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public delegate void LowStorageNotification(Product product);

    
    public class POSController
    {
        /*
         * TODO: fiks metoden til at loade ID til transaction
         * TODO: Fix constructoren så den ikke kræver BoerglumAbbeyStorageandSale
         */
        public Receipt PlacerholderReceipt;
        public Dictionary<int, Product> ProductList;

        private List<Receipt> ReceiptList = new List<Receipt>();
        
        public POSController(Dictionary<int, Product> ProductListe)
        {
            ProductList = ProductListe;
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
            // TODO: Vær sikker på hvad transactions tabellen skal hedde!!
            string deleteQuery = $"DELETE FROM transactions WHERE ID = {transactionID}";
            Mysql Connection = new Mysql();
            Connection.RunQuery(deleteQuery);
        }

        public void ExecuteReceipt()
        {
            try
            {
                PlacerholderReceipt.Execute();
                CheckStorageLevel();
            }
            catch(NullReferenceException e)
            {
                Debug.Print(e.Message);
            }
            ReceiptList.Add(PlacerholderReceipt);
            PlacerholderReceipt = null;
        }

        //Event for when the amount of a product in storage drops below a certain limit
        public event LowStorageNotification LowStorageWarning;

        //Checks the updated products after execution of receipt, to see if storage amount drops below limit
        private void CheckStorageLevel()
        {
            foreach (Product product in PlacerholderReceipt.Products())
            {
                //Limit??
                if (product.StorageWithAmount.Values.Sum() < 5)
                {
                    LowStorageWarning(product);
                }
            }
        }
    }
}