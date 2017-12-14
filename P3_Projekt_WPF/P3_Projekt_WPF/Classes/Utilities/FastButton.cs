using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace P3_Projekt_WPF.Classes.Utilities
{
    public class FastButton : Button
    {
        public int ProductID { get; set; }
        public string Button_Name { get; set; }

        public FastButton()
        {

        }

        public FastButton(string CreateFromString)
        {
            string[] CreateStringSplit = CreateFromString.Split('|');
            ProductID = Convert.ToInt32(CreateStringSplit[0]);
            Button_Name = CreateStringSplit[1];
            this.Content = Button_Name;
        }

        public FastButton(int productID, string button_name)
        {
            ProductID = productID;
            Button_Name = button_name;
        }

        public string ToString()
        {
            return ProductID + "|" + Button_Name;
        }
    }
}
