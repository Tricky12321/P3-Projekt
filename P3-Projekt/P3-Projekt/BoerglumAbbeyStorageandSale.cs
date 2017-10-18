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

namespace P3_Projekt
{
    public partial class BoerglumAbbeyStorageandSale : Form
    {
        public BoerglumAbbeyStorageandSale()
        {
            InitializeComponent();
            var pro1 = new Product();
            pro1.Name = "TEST1";

            var pro2 = new Product();
            pro2.Name = "TEST2";

            var pro3 = new Product();
            pro3.Name = "TEST3";

            var receipt = new Receipt();
            receipt.AddTransaction(new SaleTransaction(pro1, 1, receipt.ID));
            receipt.AddTransaction(new SaleTransaction(pro2, 2, receipt.ID));
            receipt.AddTransaction(new SaleTransaction(pro3, 3, receipt.ID));

            UpdateReceipt(receipt);
        }

        public void UpdateReceipt(Receipt receipt)
        {
            foreach (Transaction transaction in receipt.Transactions)
            {
                dataGridView_Receipt.Rows.Add(Text = $"{transaction.Amount}  {(transaction.Product as Product).Name} {receipt.ID}");
            }
        }
    }
}
