using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mega_Music_Editor.Unique;
using Mega_Music_Editor.Reusable;

namespace Mega_Music_Editor
{
    public partial class FrmFlag : Form
    {
        public FrmFlag()
        {
            InitializeComponent();

            nudOctave.ReadOnly = true;

            Hex flagHex = new Hex(ParameterPasser.flagValue);
            int octave = 0;

            Hex bit = new Hex("80");
            chkConModeFlag2.Checked = ((flagHex & bit) == bit);
            bit = new Hex("40");
            chkConModeFlag1.Checked = ((flagHex & bit) == bit);
            bit = new Hex("20");
            chkTripFlag.Checked = ((flagHex & bit) == bit);
            bit = new Hex("10");
            chkDotNoteFlag.Checked = ((flagHex & bit) == bit);
            bit = new Hex("08");
            chkOctPlusFlag.Checked = ((flagHex & bit) == bit);

            bit = new Hex("07");
            octave = (flagHex & bit).GetValueAsInt();

            nudOctave.Value = octave;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            Hex flagHex = new Hex("00");
            Hex octave = new Hex();

            Hex bit = new Hex("80");
            flagHex = chkConModeFlag2.Checked ? (flagHex | bit) : flagHex;
            
            bit = new Hex("40");
            flagHex = chkConModeFlag1.Checked ? (flagHex | bit) : flagHex;

            bit = new Hex("20");
            flagHex = chkTripFlag.Checked ? (flagHex | bit) : flagHex;

            bit = new Hex("10");
            flagHex = chkDotNoteFlag.Checked ? (flagHex | bit) : flagHex;
            
            bit = new Hex("08");
            flagHex = chkOctPlusFlag.Checked ? (flagHex | bit) : flagHex;

            octave = new Hex(Convert.ToInt32(nudOctave.Value));
            flagHex |= octave;

            ParameterPasser.flagValue = flagHex.GetValueAsString(2);

            Close();
        }
    }
}
