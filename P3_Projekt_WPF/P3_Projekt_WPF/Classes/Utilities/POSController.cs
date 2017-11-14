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
         */
        public Receipt PlacerholderReceipt;
        private StorageController _storageController;

        public List<Receipt> ReceiptList = new List<Receipt>();

        public POSController(StorageController storageController)
        {
            _storageController = storageController;
            StartPurchase();
        }

        public void StartPurchase()
        {
            PlacerholderReceipt = new Receipt();
        }

        public void EditReceipt(int receiptID)
        {
            PlacerholderReceipt = ReceiptList.First(x => x.ID == receiptID);
            //PlacerholderReceipt.Delete();
        }

        public Product GetProductFromID(int id)
        {
            if (_storageController.ProductDictionary.Keys.Contains(id))
            {
                return _storageController.ProductDictionary[id];
            }
            return null;
        }

        public void AddSaleTransaction(BaseProduct product, int amount = 1)
        {
            PlacerholderReceipt.AddTransaction(new SaleTransaction(product, amount, PlacerholderReceipt.ID));
        }

        public void AddIcecreamTransaction(decimal price)
        {
            //PlacerholderReceipt.AddTransaction(
                //new SaleTransaction(
                //new ServiceProduct(price, price, 0, "Is", _storageController.GroupDictionary.Where(x => x.Value.Name == "Is").First().Value), 1, PlacerholderReceipt.ID));
        }

        public void AddFreeSaleTransaction(BaseProduct product, int amount)
        {
            SaleTransaction transaction = new SaleTransaction(product, amount, PlacerholderReceipt.ID);
            transaction.Price = 0;
            PlacerholderReceipt.AddTransaction(transaction);

        }

        public void RemoveTransactionFromReceipt(int productID)
        {
            PlacerholderReceipt.RemoveTransaction(productID);
        }

        public void ExecuteReceipt()
        {
            try
            {
                PlacerholderReceipt.Execute();
                CheckStorageLevel();
            }
            catch (NullReferenceException e)
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