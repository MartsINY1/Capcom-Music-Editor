using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Mega_Music_Editor.Reusable
{
    /// <summary>
    /// Class which displays error. Permits standardisation of errors display.
    /// </summary>
    class ExceptionsCatcher
    {
        private const string _header = "Music Editor";   //Message on title bar

        /// <summary>
        /// Show error message received asstring
        /// </summary>
        /// <param name="error"></param>
        static public void ShowError(string error)
        {
            MessageBox.Show(error, _header, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// Show error message with name of function as parameter
        /// </summary>
        /// <param name="methodName"></param>
        static public void ShowErrorWithMethodName(string methodName)
        {
            string msg =  "PE: Following Method caused an error: " + methodName;

            ShowError(msg);
        }
    }
}
