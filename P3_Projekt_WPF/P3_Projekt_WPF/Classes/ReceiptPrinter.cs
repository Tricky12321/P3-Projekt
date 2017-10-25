using System;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using System.Drawing.Printing;

namespace P3_Projekt_WPF.Classes
{
    /*
    public class ReceiptPrinter
    {
        private FontStyle printFont;
        private string PrintThisText => Properties.Resources.PrintTest;
        Receipt ReceiptToPrint;


        public ReceiptPrinter(Receipt receipt)
        {
            ReceiptToPrint = receipt;
            setup();

        }

        // The Click event is raised when the user clicks the Print button.
        public void printbutton_Click(object sender, EventArgs e)
        {
            try
            {
                printFont = new Font("Courier New", 10);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler
                   (pd_PrintPage);

                // Asks for which printer to use
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = pd;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    //Print the page
                    pd.Print();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void setup()
        {
            try
            {
                printFont = new Font("Courier New", 10);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler
                   (pd_PrintPage);

                // Asks for which printer to use
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = pd;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    //Print the page
                    pd.Print();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // The PrintPage event is raised for each page to be printed.
        public void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;

            List<SaleTransaction> transactionList = ReceiptToPrint.Transactions;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            List<string> TextToPrint = new List<string>();
            string[] standardText = PrintThisText.Split('\n');
            foreach (string s in standardText)
            {
                TextToPrint.Add(s);
            }
            TextToPrint.Add($"|   #10903      { DateTime.Now.ToString()}  |");
            TextToPrint.Add($"|   01 Børglum kloster        000000 |");
            TextToPrint.Add($"|                                    |");
            foreach (SaleTransaction t in transactionList)
            {
                TextToPrint.Add($"|   {t.Amount.ToString()}x{t.Product.SalePrice.ToString().PadRight(10)}        {("*"+t.Amount * t.Product.SalePrice).ToString().PadLeft(10)}   |\n");
                TextToPrint.Add($"|   {t.GetProductNameString().PadRight(30)}   |\n");
            }
            TextToPrint.Add($"|    SUBTOTAL           {ReceiptToPrint.TotalPrice}    |");

            foreach (var SingleLine in TextToPrint)
            {
                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(SingleLine, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }
        }
    }
*/
}