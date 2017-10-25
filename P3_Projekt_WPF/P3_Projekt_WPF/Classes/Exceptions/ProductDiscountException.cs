using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Exceptions
{
    public class ProductDiscountException : Exception
    {
        public ProductDiscountException()
        {
        }

        public ProductDiscountException(string message) : base(message)
        {
        }

        public ProductDiscountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProductDiscountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
