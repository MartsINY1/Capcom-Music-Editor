using System;
using System.IO;
using System.Windows.Forms;
using static Mega_Music_Editor.Reusable.KeysMapped;

namespace Mega_Music_Editor.Reusable
{
    static class KeysMapped
    {
        internal class KeyMappedValues
        {
            private string _KeyString = "";
            private string _EditedValue = "";

            public KeyMappedValues(string defaultValue)
            {
                _KeyString = defaultValue;
            }
            public string GetKeyString()
            {
                return _KeyString;
            }
            public string GetEditedValue()
            {
                if (_EditedValue == "") return _KeyString;
                return _EditedValue;
            }
            public void SetEditedValue(string EditedValue)
            {
                _EditedValue = EditedValue;
            }
        }

        static public KeyMappedValues[] KeyMappedValuesArray = new KeyMappedValues[] { };
        static private FrmKeysMapping _FrmKeysMapping;
        static private string KeysMappedFileName = "KeysMapped.txt";

        static public void AddMappedKey(string KeyString)
        {
            Array.Resize(ref KeyMappedValuesArray, KeyMappedValuesArray.Length + 1);
            KeyMappedValuesArray[KeyMappedValuesArray.Length - 1] = new KeyMappedValues(KeyString);
        }

        static public string GetEditedValueByOriginalValue(string OriginalKeyString)
        {
            foreach (KeyMappedValues keyMappedValue in KeyMappedValuesArray)
            {
                if (keyMappedValue.GetKeyString() == OriginalKeyString)
                {
                    return keyMappedValue.GetEditedValue();
                }
            }

            return OriginalKeyString; // No match found, return the original string
        }

        static public bool SetEditedValueByOriginalValue(string OriginalKeyString, string EditedKeyString)
        {
            foreach (KeyMappedValues keyMappedValue in KeyMappedValuesArray)
            {
                if (keyMappedValue.GetKeyString() == OriginalKeyString)
                {
                    keyMappedValue.SetEditedValue(EditedKeyString);
                    return true;
                }
            }

            return false;
        }

        static public void WriteKeysMappedToFile()
        {
            try
            {
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(directoryPath, KeysMappedFileName);

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (KeyMappedValues keyMappedValue in KeyMappedValuesArray)
                    {
                        string line = keyMappedValue.GetKeyString() + "," + keyMappedValue.GetEditedValue();
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Couldn't Save File - Error:" + Environment.NewLine + Ex.Message, "Save Keys Mapped", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        static public void ReadKeysMappedFromFile()
        {
            try
            {
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(directoryPath, KeysMappedFileName);
                string line;
                string[] values;

                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            values = line.Split(',');
                            KeysMapped.SetEditedValueByOriginalValue(values[0], values[1]);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Couldn't Read File - Error:" + Environment.NewLine + Ex.Message, "Read Keys Mapped", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        static public void ShowEditForm()
        {
            _FrmKeysMapping = new FrmKeysMapping();
            _FrmKeysMapping.ShowDialog();
        }
    }

    public class FrmKeysMapping : Form
    {
        private bool changed = false;
        private ComboBox CbxKeysList;
        private Label LblSwappedKeyTitle;
        private TextBox TxtSwappedKeyValue;
        private Button BtnSaveChanges;
        private Button BtnCancelChanges;

        private DialogResult CloseFrmWithoutSaving()
        {
            return MessageBox.Show("Some values where changed. Quit without saving?", "Key Mapping Form - Close", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        public FrmKeysMapping()
        {
            InitializeComponents();
        }

        private void FrmKeysMapping_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changed)
            {
                DialogResult msgButton = CloseFrmWithoutSaving();

                if (msgButton == DialogResult.Yes)
                {
                    KeysMapped.ReadKeysMappedFromFile();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void CbxKeysList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxKeysList.Items.Count > 0)
            {
                TxtSwappedKeyValue.Text = KeysMapped.GetEditedValueByOriginalValue(CbxKeysList.SelectedItem.ToString());
            }
        }

        private void TxtSwappedKeyValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (CbxKeysList.SelectedIndex != -1)
            {
                // To declare a change occured, ensure new value is different
                if (e.KeyCode.ToString() != KeysMapped.GetEditedValueByOriginalValue(CbxKeysList.SelectedItem.ToString()))
                {
                    changed = true;

                    KeysMapped.SetEditedValueByOriginalValue(CbxKeysList.SelectedItem.ToString(), e.KeyCode.ToString());
                    TxtSwappedKeyValue.Text = KeysMapped.GetEditedValueByOriginalValue(CbxKeysList.SelectedItem.ToString());
                }
            }
            else
            {
                MessageBox.Show("No Key was changed - value written in list does not exist in that list.", "Key Mapping Form - Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtSwappedKeyValue_TextChanged(object sender, EventArgs e)
        {
            if (TxtSwappedKeyValue.Text != "" && TxtSwappedKeyValue.Text != KeysMapped.GetEditedValueByOriginalValue(CbxKeysList.SelectedItem.ToString()))
            {
                TxtSwappedKeyValue.Text = "";

                if (CbxKeysList.Items.Count > 0)
                {
                    TxtSwappedKeyValue.Text = KeysMapped.GetEditedValueByOriginalValue(CbxKeysList.SelectedItem.ToString());
                }
            }
        }

        private void BtnSaveChanges_Click(object sender, EventArgs e)
        {
            DialogResult msgButton = MessageBox.Show("Save edits?", "Key Mapping Form - Save", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (msgButton == DialogResult.Yes)
            {
                KeysMapped.WriteKeysMappedToFile();
                changed = false;
            }
        }

        private void BtnCancelChanges_Click(object sender, EventArgs e)
        {
            if (changed)
            {
                DialogResult msgButton = CloseFrmWithoutSaving();

                if (msgButton == DialogResult.Yes)
                {
                    changed = false;
                    KeysMapped.ReadKeysMappedFromFile();
                    Form.ActiveForm.Close();
                }
            }
            else
            {
                Form.ActiveForm.Close();
            }
        }

        private void InitializeComponents()
        {
            this.FormClosing += FrmKeysMapping_FormClosing;

            // Create and configure the ComboBox
            CbxKeysList = new ComboBox();
            CbxKeysList.Location = new System.Drawing.Point(20, 20);
            CbxKeysList.Size = new System.Drawing.Size(150, 21);
            CbxKeysList.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxKeysList.SelectedIndexChanged += CbxKeysList_SelectedIndexChanged;

            // Create LblSwappedKeyTitle label
            LblSwappedKeyTitle = new Label();
            LblSwappedKeyTitle.Location = new System.Drawing.Point(180, 20);
            LblSwappedKeyTitle.Size = new System.Drawing.Size(100, 20);
            LblSwappedKeyTitle.Text = "Swapped Key Value";

            // Create LblSwappedKeyValue label
            TxtSwappedKeyValue = new TextBox();
            TxtSwappedKeyValue.Location = new System.Drawing.Point(180, 40);
            TxtSwappedKeyValue.Size = new System.Drawing.Size(130, 20);
            TxtSwappedKeyValue.Text = "";
            TxtSwappedKeyValue.KeyDown += TxtSwappedKeyValue_KeyDown;
            TxtSwappedKeyValue.TextChanged += TxtSwappedKeyValue_TextChanged;

            BtnSaveChanges = new Button();
            BtnSaveChanges.Location = new System.Drawing.Point(20, 70);
            BtnSaveChanges.Size = new System.Drawing.Size(90, 23);
            BtnSaveChanges.Text = "Save Changes";
            BtnSaveChanges.Click += BtnSaveChanges_Click;

            BtnCancelChanges = new Button();
            BtnCancelChanges.Location = new System.Drawing.Point(120, 70);
            BtnCancelChanges.Size = new System.Drawing.Size(90, 23);
            BtnCancelChanges.Text = "Cancel Changes";
            BtnCancelChanges.Click += BtnCancelChanges_Click;

            // Add controls to the form
            Controls.Add(CbxKeysList);
            Controls.Add(LblSwappedKeyTitle);
            Controls.Add(TxtSwappedKeyValue);
            Controls.Add(BtnSaveChanges);
            Controls.Add(BtnCancelChanges);

            // Set form properties
            Text = "Keys Mapping";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new System.Drawing.Size(TxtSwappedKeyValue.Left + TxtSwappedKeyValue.Size.Width + 30, BtnCancelChanges.Top + BtnCancelChanges.Size.Height + 50);

            // Fill combo box
            foreach (KeyMappedValues keyMappedValue in KeyMappedValuesArray)
            {
                CbxKeysList.Items.Add(keyMappedValue.GetKeyString());
            }
            if (CbxKeysList.Items.Count > 0)
            {
                CbxKeysList.SelectedIndex = 0;
                TxtSwappedKeyValue.Text = CbxKeysList.SelectedItem.ToString();
            }
        }
    }
}
