using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Exceptions
{
    class ProductAlreadyActivated : Exception
    {
        public ProductAlreadyActivated()
        {
        }

        public ProductAlreadyActivated(string message) : base(message)
        {
        }

        public ProductAlreadyActivated(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
