using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Exceptions
{
    class ProductAlreadyDeActivated : Exception
    {
        public ProductAlreadyDeActivated()
        {
        }

        public ProductAlreadyDeActivated(string message) : base(message)
        {
        }

        public ProductAlreadyDeActivated(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
