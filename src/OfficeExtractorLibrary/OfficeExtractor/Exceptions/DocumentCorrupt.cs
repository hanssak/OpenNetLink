using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeExtractor.Exceptions
{
    public class DocumentCorrupt : Exception
    {
        internal DocumentCorrupt()
        {
        }

        internal DocumentCorrupt(string message) : base(message)
        {
        }

        internal DocumentCorrupt(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
