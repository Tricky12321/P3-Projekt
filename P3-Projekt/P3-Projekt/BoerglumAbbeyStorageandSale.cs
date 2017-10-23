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
using P3_Projekt.Classes.Database;
using System.Diagnostics;
namespace P3_Projekt
{
    public partial class BoerglumAbbeyStorageandSale : Form
    {
        public BoerglumAbbeyStorageandSale()
        {
            InitializeComponent();
        }

        public void UpdateListReceipt(Receipt receipt)
        {
            foreach (SaleTransaction transaction in receipt.Transactions)
            {
                dataGridView_Receipt.Rows.Add("+", "-", transaction.GetTransactionString());
            }
        }


        private void but_addProduct_Click(object sender, EventArgs e)
        {

        }

        // The Click event is raised when the user clicks the Print button.

        private void btn_cash_Click(object sender, EventArgs e)
        {

        }

        private void BoerglumAbbeyStorageandSale_Load(object sender, EventArgs e)
        {
            Mysql DatabaseConnection = new Mysql();
            TableDecode Results = DatabaseConnection.RunQueryWithReturn("SELECT * FROM test");
            Debug.WriteLine("test");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
