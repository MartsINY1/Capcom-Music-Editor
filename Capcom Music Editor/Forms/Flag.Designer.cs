namespace Mega_Music_Editor
{
    partial class FrmFlag
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFlag));
            this.chkConModeFlag2 = new System.Windows.Forms.CheckBox();
            this.chkConModeFlag1 = new System.Windows.Forms.CheckBox();
            this.chkTripFlag = new System.Windows.Forms.CheckBox();
            this.chkDotNoteFlag = new System.Windows.Forms.CheckBox();
            this.chkOctPlusFlag = new System.Windows.Forms.CheckBox();
            this.nudOctave = new System.Windows.Forms.NumericUpDown();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudOctave)).BeginInit();
            this.SuspendLayout();
            // 
            // chkConModeFlag2
            // 
            this.chkConModeFlag2.AutoSize = true;
            this.chkConModeFlag2.Location = new System.Drawing.Point(21, 15);
            this.chkConModeFlag2.Margin = new System.Windows.Forms.Padding(4);
            this.chkConModeFlag2.Name = "chkConModeFlag2";
            this.chkConModeFlag2.Size = new System.Drawing.Size(156, 20);
            this.chkConModeFlag2.TabIndex = 0;
            this.chkConModeFlag2.Text = "Connect Mode Flag 2";
            this.chkConModeFlag2.UseVisualStyleBackColor = true;
            // 
            // chkConModeFlag1
            // 
            this.chkConModeFlag1.AutoSize = true;
            this.chkConModeFlag1.Location = new System.Drawing.Point(21, 43);
            this.chkConModeFlag1.Margin = new System.Windows.Forms.Padding(4);
            this.chkConModeFlag1.Name = "chkConModeFlag1";
            this.chkConModeFlag1.Size = new System.Drawing.Size(156, 20);
            this.chkConModeFlag1.TabIndex = 1;
            this.chkConModeFlag1.Text = "Connect Mode Flag 1";
            this.chkConModeFlag1.UseVisualStyleBackColor = true;
            // 
            // chkTripFlag
            // 
            this.chkTripFlag.AutoSize = true;
            this.chkTripFlag.Location = new System.Drawing.Point(21, 71);
            this.chkTripFlag.Margin = new System.Windows.Forms.Padding(4);
            this.chkTripFlag.Name = "chkTripFlag";
            this.chkTripFlag.Size = new System.Drawing.Size(97, 20);
            this.chkTripFlag.TabIndex = 2;
            this.chkTripFlag.Text = "Triplet Flag";
            this.chkTripFlag.UseVisualStyleBackColor = true;
            // 
            // chkDotNoteFlag
            // 
            this.chkDotNoteFlag.AutoSize = true;
            this.chkDotNoteFlag.Location = new System.Drawing.Point(21, 100);
            this.chkDotNoteFlag.Margin = new System.Windows.Forms.Padding(4);
            this.chkDotNoteFlag.Name = "chkDotNoteFlag";
            this.chkDotNoteFlag.Size = new System.Drawing.Size(131, 20);
            this.chkDotNoteFlag.TabIndex = 3;
            this.chkDotNoteFlag.Text = "Dotted Note Flag";
            this.chkDotNoteFlag.UseVisualStyleBackColor = true;
            // 
            // chkOctPlusFlag
            // 
            this.chkOctPlusFlag.AutoSize = true;
            this.chkOctPlusFlag.Location = new System.Drawing.Point(21, 128);
            this.chkOctPlusFlag.Margin = new System.Windows.Forms.Padding(4);
            this.chkOctPlusFlag.Name = "chkOctPlusFlag";
            this.chkOctPlusFlag.Size = new System.Drawing.Size(131, 20);
            this.chkOctPlusFlag.TabIndex = 4;
            this.chkOctPlusFlag.Text = "Octave Plus Flag";
            this.chkOctPlusFlag.UseVisualStyleBackColor = true;
            // 
            // nudOctave
            // 
            this.nudOctave.Location = new System.Drawing.Point(21, 158);
            this.nudOctave.Margin = new System.Windows.Forms.Padding(4);
            this.nudOctave.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudOctave.Name = "nudOctave";
            this.nudOctave.Size = new System.Drawing.Size(254, 22);
            this.nudOctave.TabIndex = 5;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(21, 206);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(254, 28);
            this.btnConfirm.TabIndex = 6;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(21, 241);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(254, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FrmFlag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 288);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.nudOctave);
            this.Controls.Add(this.chkOctPlusFlag);
            this.Controls.Add(this.chkDotNoteFlag);
            this.Controls.Add(this.chkTripFlag);
            this.Controls.Add(this.chkConModeFlag1);
            this.Controls.Add(this.chkConModeFlag2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmFlag";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Flag";
            ((System.ComponentModel.ISupportInitialize)(this.nudOctave)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkConModeFlag2;
        private System.Windows.Forms.CheckBox chkConModeFlag1;
        private System.Windows.Forms.CheckBox chkTripFlag;
        private System.Windows.Forms.CheckBox chkDotNoteFlag;
        private System.Windows.Forms.CheckBox chkOctPlusFlag;
        private System.Windows.Forms.NumericUpDown nudOctave;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
    }
}