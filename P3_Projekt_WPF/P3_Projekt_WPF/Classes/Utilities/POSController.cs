using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF;
using System.Windows;
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

        public BaseProduct GetProductFromID(int id)
        {
            if (_storageController.AllProductsDictionary.Keys.Contains(id))
            {
                return _storageController.AllProductsDictionary[id];
            } 
            return null;
        }

        public void AddSaleTransaction(BaseProduct product, int amount = 1)
        {
            PlacerholderReceipt.AddTransaction(new SaleTransaction(product, amount, PlacerholderReceipt.ID));
        }

        public void AddIcecreamTransaction(decimal price)
        {
            if (Properties.Settings.Default.IcecreamID != -1)
            {
                var Icecream = new SaleTransaction(_storageController.ServiceProductDictionary[Properties.Settings.Default.IcecreamProductID], 1, PlacerholderReceipt.ID);
                Icecream.Price = price;
                PlacerholderReceipt.AddTransaction(Icecream);
            } else
            {
                Utils.GetIceCreameID();
                if (Properties.Settings.Default.IcecreamID == -1)
                {
                    MessageBox.Show("Du skal oprette en gruppen med navnet \"is\", for at kunne sælge is.");

                } else
                {
                    AddIcecreamTransaction(price);
                }
            }
            
        }

        public void AddFreeSaleTransaction(BaseProduct product, int amount)
        {
            SaleTransaction transaction = new SaleTransaction(product, amount, PlacerholderReceipt.ID);
            transaction.Price = 0;
            PlacerholderReceipt.AddTransaction(transaction);

        }

        public void ChangeTransactionAmount(object sender, EventArgs e, int amount)
        {
            string IDTag = (sender as ReceiptListItem).IDTag;
            if (IDTag.Contains("t"))
            {
                int productID = Convert.ToInt32(IDTag.Replace("t", string.Empty));
                PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().Amount += amount;
                PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().CheckIfGroupPrice();
                PlacerholderReceipt.UpdateTotalPrice();
            }
            else if (Convert.ToInt32(IDTag) == Properties.Settings.Default.IcecreamProductID)
            {
                int productID = Convert.ToInt32(IDTag);
                PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID && (x.TotalPrice == (sender as ReceiptListItem).Price)).First().Amount += amount;
                PlacerholderReceipt.UpdateTotalPrice();
            }
            else
            {
                int productID = Convert.ToInt32(IDTag);
                PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().Amount += amount;
                PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First().CheckIfGroupPrice();
                PlacerholderReceipt.UpdateTotalPrice();
            }
        }

        public void RemoveTransactionFromReceipt(int productID)
        {
            PlacerholderReceipt.RemoveTransaction(productID);
        }

        public void ExecuteReceipt()
        {
            PlacerholderReceipt.Execute();
            CheckStorageLevel();
            ReceiptList.Add(PlacerholderReceipt);
            PlacerholderReceipt = new Receipt();
        }

        //Event for when the amount of a product in storage drops below a certain limit
        public event LowStorageNotification LowStorageWarning;

        //Checks the updated products after execution of receipt, to see if storage amount drops below limit
        private void CheckStorageLevel()
        {
            foreach (BaseProduct product in PlacerholderReceipt.GetProducts())
            {
                if (product is Product)
                {
                    if ((product as Product).StorageWithAmount.Values.Sum() < 5)
                    {
                        if (LowStorageWarning != null)
                        {
                            LowStorageWarning.Invoke(product as Product);
                        }
                    }
                }
                //Limit??
            }
        }
    }
}