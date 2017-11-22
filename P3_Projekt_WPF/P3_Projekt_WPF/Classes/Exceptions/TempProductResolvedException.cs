using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Exceptions
{
    public class TempProductResolvedException : Exception
    {

        public TempProductResolvedException()
        {

        }

        public TempProductResolvedException(string message) : base(message)
        {

        }

        public TempProductResolvedException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
