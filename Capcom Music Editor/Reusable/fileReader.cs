using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Reusable
{
    class FileReader
    {
        /// <summary>
        /// Return true if file is opened correctly
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool OpenFile(out StreamReader file, string path)
        {
            try
            {
                file = new StreamReader(path);
            }
            catch (Exception)
            {
                // Couldn't open file
                file = null;
                return false;
            }
            return true;
        }
    }
}
