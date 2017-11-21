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
//using P3_Projekt_WPF.Resources;
using System.Diagnostics;
using System.Windows.Xps;
namespace P3_Projekt_WPF.Classes
{
    public static class ReceiptPrinter
    {
        /// <param name="Text">The text to be printeted, new line using \n</param>
        public static void PrintText(string Text)
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

        /// <summary>
        /// Prints the receipt given. 
        /// </summary>
        /// <param name="receipt">The receipt you would like to print</param>
        public static void PrintReceipt(Receipt receipt)
        {
            StringBuilder textToPrint = new StringBuilder();
            textToPrint.Append($"|   {("#" + 10903).PadRight(5)}  { DateTime.Now.ToString().PadLeft(10)}  |\n");
            textToPrint.Append($"|   {("01" + "Børglum kloster").PadRight(5)} {000000.ToString().PadLeft(10)} |\n");
            textToPrint.Append($"|{" ".PadRight(35)}|\n");

            List<SaleTransaction> transactionList = receipt.Transactions;

            foreach (SaleTransaction t in transactionList)
            {
                textToPrint.Append($"|   {t.Amount}x{t.Product.SalePrice.ToString().PadRight(10)} {("*" + (t.Amount * t.Product.SalePrice)).ToString().PadLeft(10)}\n");
                textToPrint.Append($"|   {t.Product.GetName().PadRight(30)}   |\n");
            }
            textToPrint.Append($"|    {"SUBTOTAL".PadRight(10)}{receipt.TotalPrice.ToString().PadLeft(10)}\n|");
            PrintText(textToPrint.ToString());
        }

        public static void GenerateTextToPrint()
        {
            StringBuilder textToPrint = new StringBuilder();
            textToPrint.Append($"|   {("#" + 10903).PadRight(5)}  { DateTime.Now.ToString().PadLeft(10)}  |\n");
            textToPrint.Append($"|   {("01" + "Børglum kloster").PadRight(5)} {000000.ToString().PadLeft(10)} |\n");
            textToPrint.Append($"|{" ".PadRight(35)}|\n");
            PrintText(textToPrint.ToString());
        }
    }
}