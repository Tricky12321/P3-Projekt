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
using System.Drawing;
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
            string file = ($"Kvittering{Receipt.GetNextID()}_{DateTime.Now.ToString()}").Replace(" ", string.Empty).Replace("-", string.Empty).Replace(":", string.Empty);

            // the directory to store the output.
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            PrintDocument doc = new PrintDocument()
            {
                PrinterSettings = new PrinterSettings()
                {
                    // set the printer to 'Microsoft Print to PDF'
                    PrinterName = "Microsoft Print to PDF",

                    // tell the object this document will print to file
                    PrintToFile = true,

                    // set the filename to whatever you like (full path)
                    PrintFileName = Path.Combine(directory, file + ".pdf"),
                }
            };

            doc.PrintPage += delegate (object sender, PrintPageEventArgs e)
            {
                e.Graphics.DrawString(Text, new Font("Courier New", 12), new SolidBrush(System.Drawing.Color.Black), new RectangleF(0, 0, doc.DefaultPageSettings.PrintableArea.Width, doc.DefaultPageSettings.PrintableArea.Height));
            };

            try
            {
                doc.Print();
            }
            catch
            {
                MessageBox.Show("Printer var ikke tilgængelig");
            }
        }

        public static string GenerateTextToPrint(Receipt receipt)
        {
            StringBuilder textToPrint = new StringBuilder();

            textToPrint.Append($" _____________________________________________ \n");
            textToPrint.Append($"|                                             |\n");
            textToPrint.Append($"|               Børglum Kloster               |\n");
            textToPrint.Append($"|              Tlf. 98 99 40 11               |\n");
            textToPrint.Append($"|              Cvr. nr. 77653414              |\n");
            textToPrint.Append($"|                                             |\n");
            textToPrint.Append($"|          Info@boerglumkloster.dk            |\n");
            textToPrint.Append($"|          www.boerglumkloster.dk             |\n");
            textToPrint.Append($"|                                             |\n");
            textToPrint.Append($"|{("  #" + Receipt.GetNextID()).PadRight(6)}  {(DateTime.Now.ToString() + "  ").PadLeft(37)}|\n");
            textToPrint.Append($"|                                             |\n");

            return textToPrint.ToString();
        }

        /// <summary>
        /// Prints the receipt given. 
        /// </summary>
        /// <param name="receipt">The receipt you would like to print</param>
        public static void PrintReceipt(Receipt receipt)
        {
            StringBuilder textToPrint = new StringBuilder();

            textToPrint.Append(GenerateTextToPrint(receipt));

            foreach (SaleTransaction transaction in receipt.Transactions)
            {
                if (transaction.Product.GetName().Length > 40)
                {
                    foreach (string namePart in bestProductNameSplit(transaction.Product.GetName()))
                    {
                        textToPrint.Append($"|  {namePart.PadRight(40)}   |\n");
                    }
                }
                else
                {
                    textToPrint.Append($"|  {transaction.Product.GetName().PadRight(40)}   |\n");
                }

                if (transaction.DiscountBool)
                {
                    textToPrint.Append($"|  {transaction.Amount.ToString().PadRight(3)} x {Math.Round(transaction.Price, 2).ToString().PadRight(22)} {((Math.Round(transaction.Price * transaction.Amount, 2)).ToString().PadRight(2)).PadLeft(12)}  |\n");
                    textToPrint.Append($"|  {("- " + (Math.Round(transaction.TotalPrice - (transaction.DiscountPrice * transaction.Amount), 2)).ToString().PadRight(2)).PadLeft(41)}  |\n");
                    textToPrint.Append($"|  {("= " + (Math.Round(transaction.DiscountPrice * transaction.Amount, 2)).ToString().PadRight(2)).PadLeft(41)}  |\n");
                }
                else
                {
                    textToPrint.Append($"|  {transaction.Amount.ToString().PadRight(3)} x {Math.Round(transaction.Price, 2).ToString().PadRight(22)} {("= " + (Math.Round(transaction.TotalPrice, 2)).ToString().PadRight(2)).PadLeft(12)}  |\n");
                }

                textToPrint.Append($"|                                             |\n");
            }

            textToPrint.Append($"|                                             |\n");

            if (receipt.DiscountOnFullReceipt > 0)
            {
                textToPrint.Append($"|  {"SUBTOTAL = ".PadLeft(25)} {((Math.Round(receipt.TotalPrice + receipt.DiscountOnFullReceipt, 2).ToString())).PadLeft(15)}  |\n");
                textToPrint.Append($"|  {"RABAT = ".PadLeft(25)} {("- " + (Math.Round(receipt.DiscountOnFullReceipt, 2).ToString())).PadLeft(15)}  |\n");
            }

            textToPrint.Append($"|  {"TOTAL = ".PadLeft(25)} {((Math.Round(receipt.TotalPrice, 2).ToString())).PadLeft(15)}  |\n");
            textToPrint.Append($"|_____________________________________________|\n");
            PrintText(textToPrint.ToString());
        }

        private static List<string> bestProductNameSplit(string name)
        {
            string[] splittetName = name.Split(' ');
            List<string> nameSplitToReturn = new List<string>();
            nameSplitToReturn.Add(string.Empty);

            int j = 0;
            foreach (string startNamePart in splittetName)
            {
                if (nameSplitToReturn[j].Length + 1 + startNamePart.Length <= 40)
                {
                    nameSplitToReturn[j] += $"{startNamePart} ";
                }
                else
                {
                    nameSplitToReturn.Add(startNamePart);
                    j++;
                }

                if (j == 3)
                {
                    return nameSplitToReturn;
                }
            }

            return nameSplitToReturn;
        }
    }
}