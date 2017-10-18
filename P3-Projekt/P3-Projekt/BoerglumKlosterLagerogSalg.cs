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
    public partial class BoerglumKlosterLagerogSalg : Form
    {
        public BoerglumKlosterLagerogSalg()
        {
            InitializeComponent();
        }

        public void UpdateReceipt(Receipt receipt)
        {
            foreach (Transaction transaction in receipt.Transactions)
            {
                dataGridView_Receipt.Rows.Add(Text = (transaction.Product as Product).Name);
            }
        }
    }
}
