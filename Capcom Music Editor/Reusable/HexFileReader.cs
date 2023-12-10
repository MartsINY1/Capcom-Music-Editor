using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Mega_Music_Editor.Reusable
{
    class HexFileReader
    {
        static public readonly string _endOfFileReachedFlag = "Eof";

        /// <summary>
        /// Open a FileStream while managing in specific way the FileNotFoundException error</summary>
        /// <param name="fs">Reference to the file</param>
        /// <param name="position">Position to read</param>
        /// <param name="path"></param>
        /// <returns>True if operation was successful</returns>
        /// If path is invalid</exception>
        static public bool HexFileOpener(ref FileStream fs, int position, string path)
        {
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                fs.Seek(position, SeekOrigin.Begin);

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Close a FileStream and dispose of it
        /// </summary>
        /// <param name="fs">Reference to the file</param>
        static public void HexFileCloser(ref FileStream fs)
        {
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>
        /// Return hex code read. 
        /// Advantages : it handles the file closing if it makes a throw.
        /// </summary>
        /// <param name="fs">Reference to the file</param>
        /// <returns>Hex code read, and a specific value if eof reached</returns>
        static public string ReadOneHexCode(FileStream fs)
        {
            int hexIn = 0;                      //used to read the hex code as int

            if ((hexIn = fs.ReadByte()) == -1)  //reached end of file
            {
                HexFileCloser(ref fs);
                return _endOfFileReachedFlag;
            }

            return Hex.ConvertIntToHexString(hexIn);
        }
    }
}