using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Mega_Music_Editor.Reusable
{
    class HexFileWriter
    {
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
                fs = new FileStream(path, FileMode.Open, FileAccess.Write);

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
        /// Write list of hex codes
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="listHexCodesToWrite"></param>
        /// <returns></returns>
        static public void WriteListHexCodes(FileStream fs, List<Hex> listHexCodesToWrite)
        {
            int hexToWriteIntValue = 0;
            byte[] byteToWrite;

            foreach (Hex hexToWrite in listHexCodesToWrite)
            {
                hexToWriteIntValue  = hexToWrite.GetValueAsInt();
                byteToWrite = BitConverter.GetBytes(hexToWriteIntValue);
                fs.WriteByte(byteToWrite[0]);
            }
        }
    }
}
