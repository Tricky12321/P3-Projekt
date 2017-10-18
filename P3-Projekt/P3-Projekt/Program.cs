using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using P3_Projekt.Classes.Utilities;

namespace P3_Projekt
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
  

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var BoerglumKlosterLagerogSalg = new BoerglumKlosterLagerogSalg();
            var POSController = new POSController(BoerglumKlosterLagerogSalg);
            var StorageController = new StorageController(BoerglumKlosterLagerogSalg);
            Application.Run(BoerglumKlosterLagerogSalg);
        }
    }
}
