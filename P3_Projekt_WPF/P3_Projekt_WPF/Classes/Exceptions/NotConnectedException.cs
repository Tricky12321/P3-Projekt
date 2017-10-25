using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Exceptions
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException()
        {
        }

        public NotConnectedException(string message) : base(message)
        {
        }

        public NotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotConnectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
