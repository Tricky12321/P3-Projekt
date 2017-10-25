using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Utilities
{
    public static class Utils
    {
        public static int ConvertBoolToInt(bool booleanValue)
        {
            if (booleanValue)
            {
                return 1;
            } else
            {
                return 0;
            }
        }

        public static bool ConvertIntToBool(int intVal)
        {
            return (intVal != 0);
        }
    }
}
