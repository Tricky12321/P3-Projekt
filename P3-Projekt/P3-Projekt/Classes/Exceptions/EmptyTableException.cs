using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Exceptions
{
    public class EmptyTableException : Exception
    {

        public EmptyTableException()
        {

        }

        public EmptyTableException(string message) : base(message)
        {

        }

        public EmptyTableException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
