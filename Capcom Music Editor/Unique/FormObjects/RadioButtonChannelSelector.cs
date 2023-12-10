using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Mega_Music_Editor.Unique
{
    partial class DataGridViewsHandler
    {
        private GroupBox _groupBox = null;

        /// <summary>
        /// Events associated to all radio buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdChannelSelection_checkedchanged(object sender, EventArgs e)
        {
            RadioButton radiobutton = sender as RadioButton;
            string rdText = radiobutton.Text;
            int index = 0;

            if (radiobutton.Checked)
            {
            }

            if (_GameType == GameType.NesA)
            {
                if (rdText == "Square 1") index = 0;
                if (rdText == "Square 2") index = 1;
                if (rdText == "Triangle") index = 2;
                if (rdText == "Noise") index = 3;
                ShowOneSheet(index);
            }
            else if (_GameType == GameType.SnesA)
            {
                if (rdText == "Channel 1") index = 0;
                if (rdText == "Channel 2") index = 1;
                if (rdText == "Channel 3") index = 2;
                if (rdText == "Channel 4") index = 3;
                if (rdText == "Channel 5") index = 4;
                if (rdText == "Channel 6") index = 5;
                if (rdText == "Channel 7") index = 6;
                if (rdText == "Channel 8") index = 7;
                ShowOneSheet(index);
            }
        }

        /// <summary>
        /// Creates group box with a number of radio button allowing to pick every channels (4 or 8)
        /// </summary>
        /// <param name="gameType"></param>
        public void CreateGroupBoxForChannelSelection(ref GroupBox gbxSheetChoice, ref GroupBox gbxConsoleChoice)
        {
            RadioButton rdTemp;
            int channelQty = 4;

            // If a Snes games there are 8 channels
            if (_GameType == GameType.SnesA)
            {
                channelQty = 8;
            }

            // If group box already exist, remove it
            if (_groupBox != null)
            {
                _currentForm.Controls.Remove(_groupBox);
                _groupBox = null;
            }

            _groupBox = new GroupBox();

            // Group box properties
            _groupBox.Name = "gbxDataGridViewSelecter";
            _groupBox.Text = "Channel";
            _groupBox.Top = 30;
            _groupBox.Left = 615;
            _groupBox.Height = 20;
            _groupBox.Width = 100;
            _groupBox.Visible = true;

            if (_GameType == GameType.SnesA)
            {
                _groupBox.Width =  200;
            }

            // Add the radio buttons
            for (int i = 0; i < channelQty; i++)
            {
                rdTemp = new RadioButton();

                // First sheet is checked
                if (i == 0)
                {
                    rdTemp.Checked = true;
                }

                rdTemp.Name = "rdSheet" + i.ToString();

                if (_GameType == GameType.NesA)
                {
                    switch (i)
                    {
                        case 0: rdTemp.Text = "Square 1"; break;
                        case 1: rdTemp.Text = "Square 2"; break;
                        case 2: rdTemp.Text = "Triangle"; break;
                        case 3: rdTemp.Text = "Noise"; break;
                        default: rdTemp.Text = "Error"; break;
                    }
                }
                else if (_GameType == GameType.SnesA)
                {
                    switch (i)
                    {
                        case 0: rdTemp.Text = "Channel 1"; break;
                        case 1: rdTemp.Text = "Channel 2"; break;
                        case 2: rdTemp.Text = "Channel 3"; break;
                        case 3: rdTemp.Text = "Channel 4"; break;
                        case 4: rdTemp.Text = "Channel 5"; break;
                        case 5: rdTemp.Text = "Channel 6"; break;
                        case 6: rdTemp.Text = "Channel 7"; break;
                        case 7: rdTemp.Text = "Channel 8"; break;
                        default: rdTemp.Text = "Error"; break;
                    }
                }

                // For snes channels
                if (i >= 4)
                {
                    rdTemp.Top = 20 + ((i - 4) * 30);     // 30 is a distance between each radio button, minus is because we have 2 column
                    rdTemp.Left = 110;
                }
                else
                {
                    rdTemp.Top = 20 + (i * 30);     // 30 is a distance between each radio button
                    rdTemp.Left = 10;

                    // Height of controls is increased
                    _groupBox.Height += 30;
                }

                rdTemp.CheckedChanged += RdChannelSelection_checkedchanged;

                // Width of radio button
                rdTemp.Width = 80;

                // Add to group box
                _groupBox.Controls.Add(rdTemp);

                // We position the 2 other group boxes accordingly
                    // The top value in the priority isn't the same as in code, so set it in code
                gbxSheetChoice.Top = 30;
                gbxConsoleChoice.Top = 30;

                gbxSheetChoice.Left = _groupBox.Left + _groupBox.Width + 10;
                gbxConsoleChoice.Left = gbxSheetChoice.Left + gbxSheetChoice.Width + 10;
            }

            _currentForm.Controls.Add(_groupBox);
        }
    }
}
