using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeExtractor.Exceptions
{
    class XmlParsingException : Exception
    {
        internal XmlParsingException()
        {
        }

        internal XmlParsingException(string message) : base(message)
        {
        }

        internal XmlParsingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
