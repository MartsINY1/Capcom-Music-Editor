using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Reusable
{
    public class KnownException : Exception
    {
        public KnownException(string message)
            : base(message)
        {
        }
    }
}
