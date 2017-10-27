using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Media;
using System.Drawing.Printing;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using P3_Projekt_WPF.Properties;
using P3_Projekt_WPF.Resources;
using System.Diagnostics;
using System.Windows.Xps;
namespace P3_Projekt_WPF.Classes
{
    //TODO: ReceiptPrinter virker slet ikke mere, alt er kommenteret ud.

    public class ReceiptPrinter
    {
        //TODO: Skal laves om til WPF font
        //private System.Drawing.FontStyle printFont;
        //private string PrintThisText => .PrintTest;
        Receipt ReceiptToPrint;


        public ReceiptPrinter(Receipt receipt)
        {
            ReceiptToPrint = receipt;
            //setup();

        }

        // The Click event is raised when the user clicks the Print button.
        public void printbutton_Click(object sender, EventArgs e)
        {
            try
            {
                //TODO: SKal lige fixet med WPF font istedet for System.Windows Font
                //printFont = new Font("Courier New", 10);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler
                   (pd_PrintPage);

                // Asks for which printer to use
                PrintDialog printDialog = new PrintDialog();
                printDialog.PageRangeSelection = PageRangeSelection.AllPages;
                printDialog.UserPageRangeEnabled = true;
                // Display the dialog. This returns true if the user presses the Print button.
                Nullable<Boolean> print = printDialog.ShowDialog();
                if (print == true)
                {
                    XpsDocument xpsDocument = new XpsDocument("C:\\FixedDocumentSequence.xps", FileAccess.ReadWrite);
                    FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

                    printDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
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
            //TODO: Virker ikke mere skal laves efter XPS standard. 
            //linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);

            StringBuilder textToPrint = new StringBuilder();
            string[] standardText = Properties.Resources.PrintTest.Split('\n');
            int i;
            for (i = 0; i <= standardText.Length; i++)
            {
                textToPrint.Insert(i, standardText[i]);
            }
            
            textToPrint.Append($"|   {("#"+10903).PadRight(5)}  { DateTime.Now.ToString().PadLeft(10)}  |\n");
            textToPrint.Append($"|   {("01" + "Børglum kloster").PadRight(5)} {000000.ToString().PadLeft(10)} |\n");
            textToPrint.Append($"|{" ".PadRight(35)}|\n");

            foreach (SaleTransaction t in transactionList)
            {
                textToPrint.Append($"|   {t.Amount}x{t.Product.SalePrice.ToString().PadRight(10)} {("*" + (t.Amount * t.Product.SalePrice)).ToString().PadLeft(10)}\n");
                textToPrint.Append($"|   {t.Product.GetName().PadRight(30)}   |\n");
            }
            textToPrint.Append($"|    {"SUBTOTAL".PadRight(10)}{ReceiptToPrint.TotalPrice.ToString().PadLeft(10)}\n|");

            Debug.Print(textToPrint.ToString());

            /*
            foreach (var SingleLine in TextToPrint)
            {
                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(SingleLine, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }
            */
        }
    }
}