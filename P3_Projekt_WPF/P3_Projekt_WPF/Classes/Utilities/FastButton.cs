using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace P3_Projekt_WPF.Classes.Utilities
{
    class FastButton : Button
    {
        public int ProductID { get; set; }
        public string Button_Name { get; set; }
        public FastButton()
        {

        }

        public FastButton(int productID, string button_name)
        {
            ProductID = productID;
            Button_Name = button_name;
        }
    }
}
