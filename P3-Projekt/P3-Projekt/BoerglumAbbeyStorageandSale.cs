using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using P3_Projekt.Classes;
using System.Drawing.Printing;

namespace P3_Projekt
{
    public partial class BoerglumAbbeyStorageandSale : Form
    {
        P3_Projekt.Classes.ReceiptPrinter Printer = new ReceiptPrinter();


        public BoerglumAbbeyStorageandSale()
        {
            InitializeComponent();
            var test = new Receipt();
            test.AddTransaction(new SaleTransaction(new Product("ged", "ged", 100m, new Group("GEd", "ged"), true, 180m, 150m, default(Image)), 10, 10));
            test.AddTransaction(new SaleTransaction(new Product("ged", "ged", 100m, new Group("GEd", "ged"), false, 180m, 150m, default(Image)), 10, 10));
            test.AddTransaction(new SaleTransaction(new Product("ged", "ged", 100m, new Group("GEd", "ged"), true, 180m, 120m, default(Image)), 10, 10));
            test.AddTransaction(new SaleTransaction(new Product("ged", "ged", 100m, new Group("GEd", "ged"), true, 180m, 150m, default(Image)), 10, 10));
            test.AddTransaction(new SaleTransaction(new ServiceProduct(85m, 75m, 10,"Voksne"), 12, 10));
            test.AddTransaction(new SaleTransaction(new ServiceProduct(85m, 75m, 10, "Voksne"), 8, 10));
            UpdateListReceipt(test);
        }

        public void UpdateListReceipt(Receipt receipt)
        {
            foreach (SaleTransaction transaction in receipt.Transactions)
            {
                dataGridView_Receipt.Rows.Add(Text = transaction.GetTransactionString());
            }
        }


        private void but_addProduct_Click(object sender, EventArgs e)
        {

        }

        // The Click event is raised when the user clicks the Print button.
        private void printbutton_Click(object sender, EventArgs e)
        {
            Printer.printbutton_Click(sender, e);
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Printer.pd_PrintPage(sender, ev);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
