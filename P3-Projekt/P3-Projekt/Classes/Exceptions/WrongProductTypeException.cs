using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Exceptions
{
    class WrongProductTypeException : Exception
    {
        public WrongProductTypeException()
        {
        }

        public WrongProductTypeException(string message) : base(message)
        {
        }

        public WrongProductTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongProductTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
