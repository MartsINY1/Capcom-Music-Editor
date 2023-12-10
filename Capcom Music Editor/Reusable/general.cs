using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Reusable
{
    class General
    {
        /// <summary>
        /// Return content of string after first ":"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string ReturnStringAfterColon(string str)
        {
            return (str.Substring(str.IndexOf(":") + 1));
        }
    }
}
