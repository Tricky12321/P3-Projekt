using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
namespace P3_Projekt.Classes
{

    public class ReceiptPrinter
    {
        private Font printFont;
        private string PrintThisText => Properties.Resources.PrintTest;



        // The Click event is raised when the user clicks the Print button.
        public void printbutton_Click(object sender, EventArgs e)
        {
            try
            {
                printFont = new Font("Courier New", 25);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler
                   (pd_PrintPage);

                /* Asks for which printer to use */
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

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            string[] TextToPrint = PrintThisText.Split('\n');
            
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
}