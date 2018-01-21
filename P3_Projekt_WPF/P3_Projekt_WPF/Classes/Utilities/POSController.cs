using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public delegate void LowStorageNotification(Product product);
    public delegate void ReceiptExecuteDoneDelegate();

    public class POSController
    {
        public Receipt PlacerholderReceipt;
        private StorageController _storageController;
        public List<Receipt> ReceiptList = new List<Receipt>();
        public int ReceiptID = 0;
        public decimal TotalPriceToPay = -1m;
        private const int _storageWarningLimit = 5;

        public POSController(StorageController storageController)
        {
            _storageController = storageController;
            StartPurchase();
        }

        public void StartPurchase()
        {
            PlacerholderReceipt = new Receipt();
            TotalPriceToPay = -1m;
            ReceiptID = 0;
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
            //TODO
            if (Properties.Settings.Default.IcecreamID != -1)
            {
                var Icecream = new SaleTransaction(_storageController.ServiceProductDictionary[Properties.Settings.Default.IcecreamProductID], 1, PlacerholderReceipt.ID);
                Icecream.Price = price;
                PlacerholderReceipt.AddTransaction(Icecream);
            }
            else
            {
                Utils.GetIceCreamID();
                if (Properties.Settings.Default.IcecreamID == -1)
                {
                    MessageBox.Show("Du skal oprette en gruppen med navnet \"is\", for at kunne sælge is.");
                }
                else
                {
                    AddIcecreamTransaction(price);
                }
            }
        }

        public void ChangeTransactionAmount(object sender, EventArgs e, int amount)
        {
            string IDTag = (sender as ReceiptListItem).IDTag;
            SaleTransaction placeholderTransaction;
            if (IDTag.Contains("t"))
            {
                int productID = Convert.ToInt32(IDTag.Replace("t", string.Empty));
                placeholderTransaction = PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First();
                placeholderTransaction.Amount += amount;
                placeholderTransaction.CheckIfGroupPrice();
                PlacerholderReceipt.UpdateTotalPrice();
            }
            else if (Convert.ToInt32(IDTag) == Properties.Settings.Default.IcecreamProductID)
            {
                int productID = Convert.ToInt32(IDTag);
                placeholderTransaction = PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID && (x.TotalPrice == (sender as ReceiptListItem).Price)).First();
                placeholderTransaction.Amount += amount;
                PlacerholderReceipt.UpdateTotalPrice();
            }
            else
            {
                int productID = Convert.ToInt32(IDTag);
                placeholderTransaction = PlacerholderReceipt.Transactions.Where(x => x.Product.ID == productID).First();
                placeholderTransaction.Amount += amount;
                placeholderTransaction.CheckIfGroupPrice();
                PlacerholderReceipt.UpdateTotalPrice();
            }
            PlacerholderReceipt.RemoveDiscountFromDiscount(placeholderTransaction);
            PlacerholderReceipt.UpdateTotalPrice();
        }

        public void RemoveTransactionFromReceipt(int productID)
        {
            PlacerholderReceipt.RemoveTransaction(productID);
        }

        public void ExecuteReceipt()
        {
            ExecuteReceipt(true);
        }

        public event ReceiptExecuteDoneDelegate ReceiptExecutingThreadDone;

        public void ExecuteReceipt(bool PrintReceipt = true)
        {
            PlacerholderReceipt.Execute(PrintReceipt);
            CheckStorageLevel();
            ReceiptList.Add(PlacerholderReceipt);
            ReceiptExecutingThreadDone?.Invoke();
            StartPurchase();
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
                    if ((product as Product).StorageWithAmount.Values.Sum() < _storageWarningLimit)
                    {
                        if (LowStorageWarning != null)
                        {
                            LowStorageWarning.Invoke(product as Product);
                        }
                    }
                }
            }
        }
        
        public string CompletePurchase(PaymentMethod_Enum PaymentMethod, TextBox PayWithAmount, ListView ReceiptListView, out bool CompletedPurchase)
        {
            CompletedPurchase = false;
            if (ReceiptListView.HasItems)
            {
                //TODO
                if (TotalPriceToPay == -1m)
                {
                    TotalPriceToPay = PlacerholderReceipt.TotalPrice;
                }

                decimal PaymentAmount;
                if (PayWithAmount.Text.Length == 0)
                {
                    PaymentAmount = TotalPriceToPay;
                }
                else
                {
                    PaymentAmount = Convert.ToDecimal(PayWithAmount.Text);
                }

                if (ReceiptID == 0)
                {
                    ReceiptID = Receipt.GetNextID();
                }

                Payment NewPayment = new Payment(ReceiptID, PaymentAmount, PaymentMethod);
                PlacerholderReceipt.Payments.Add(NewPayment);

                PayWithAmount.Text = string.Empty;
                TotalPriceToPay -= NewPayment.Amount;
                if (PlacerholderReceipt.PaidPrice >= PlacerholderReceipt.TotalPrice)
                {
                    CompletedPurchase = true;
                    //TODO
                    SaleTransaction.SetStorageController(_storageController);

                    Thread NewThread = new Thread(new ThreadStart(ExecuteReceipt));
                    NewThread.Name = "ExecuteReceipt Thread";
                    NewThread.Start();
                    _storageController.TempProductToDictionary();
                    ReceiptListView.Items.Clear();

                    TotalPriceToPay = -1m;
                    ReceiptID = 0;
                    if (PlacerholderReceipt.PaidPrice > PlacerholderReceipt.TotalPrice)
                    {
                        return "Retur: " + Math.Round((PlacerholderReceipt.PaidPrice - PlacerholderReceipt.TotalPrice), 2).ToString().Replace('.', ',');
                    }
                    else
                    {
                        return "";
                    }
                }
                if (TotalPriceToPay != -1m) 
                {
                    if (TotalPriceToPay == 0)
                    {
                        PlacerholderReceipt.Payments.Clear();
                        PlacerholderReceipt.TotalPrice = 0;
                        CompletedPurchase = true;
                        return string.Empty;
                    }
                    return Math.Round(TotalPriceToPay, 2).ToString().Replace('.', ',');
                }
            }
            return string.Empty;
        }
    }
}