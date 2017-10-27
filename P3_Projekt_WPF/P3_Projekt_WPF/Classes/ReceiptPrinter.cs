using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Media;
using System.Drawing.Printing;
using System.Windows.Controls;
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

        public ReceiptPrinter()
        {

        }

        public void PrintText(string Text)
        {
            Paragraph flowParagraph = new Paragraph();
            flowParagraph.Inlines.Add(Text); //courier new 
            flowParagraph.FontFamily = new FontFamily("Courier New");
            FlowDocument flowDoc = new FlowDocument(flowParagraph);
            IDocumentPaginatorSource idpSource = flowDoc;
            DocumentPaginator docPaginator = idpSource.DocumentPaginator;
            PrintDialog printDialog = new PrintDialog();
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            printDialog.UserPageRangeEnabled = true;
            Nullable<Boolean> print = printDialog.ShowDialog();
            if (print == true)
            {
                printDialog.PrintDocument(docPaginator, "Print Kvittering");
            }
        }

        public void GenerateTextToPrint()
        {
            StringBuilder textToPrint = new StringBuilder();
            textToPrint.Append($"|   {("#" + 10903).PadRight(5)}  { DateTime.Now.ToString().PadLeft(10)}  |\n");
            textToPrint.Append($"|   {("01" + "Børglum kloster").PadRight(5)} {000000.ToString().PadLeft(10)} |\n");
            textToPrint.Append($"|{" ".PadRight(35)}|\n");

            /*List<SaleTransaction> transactionList = ReceiptToPrint.Transactions;

            foreach (SaleTransaction t in transactionList)
            {
                textToPrint.Append($"|   {t.Amount}x{t.Product.SalePrice.ToString().PadRight(10)} {("*" + (t.Amount * t.Product.SalePrice)).ToString().PadLeft(10)}\n");
                textToPrint.Append($"|   {t.Product.GetName().PadRight(30)}   |\n");
            }
            textToPrint.Append($"|    {"SUBTOTAL".PadRight(10)}{ReceiptToPrint.TotalPrice.ToString().PadLeft(10)}\n|");
            */

            PrintText(textToPrint.ToString());
        }
    }
}