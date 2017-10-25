using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Exceptions
{
    public class UnknownProductTypeException : Exception
    {
        public UnknownProductTypeException()
        {
        }

        public UnknownProductTypeException(string message) : base(message)
        {
        }

        public UnknownProductTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownProductTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
