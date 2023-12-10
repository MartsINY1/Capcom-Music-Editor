namespace Mega_Music_Editor
{
    partial class FrmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAbout));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMatrixz = new System.Windows.Forms.Label();
            this.lblDurfarC = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblGoldenShades = new System.Windows.Forms.Label();
            this.txtMatrixz = new System.Windows.Forms.RichTextBox();
            this.txtDurfarC = new System.Windows.Forms.RichTextBox();
            this.txtGoldenShades = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Copyrights";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Special Thanks";
            // 
            // lblMatrixz
            // 
            this.lblMatrixz.AutoSize = true;
            this.lblMatrixz.Location = new System.Drawing.Point(15, 150);
            this.lblMatrixz.Name = "lblMatrixz";
            this.lblMatrixz.Size = new System.Drawing.Size(48, 16);
            this.lblMatrixz.TabIndex = 2;
            this.lblMatrixz.Text = "Matrixz";
            // 
            // lblDurfarC
            // 
            this.lblDurfarC.AutoSize = true;
            this.lblDurfarC.Location = new System.Drawing.Point(15, 250);
            this.lblDurfarC.Name = "lblDurfarC";
            this.lblDurfarC.Size = new System.Drawing.Size(52, 16);
            this.lblDurfarC.TabIndex = 3;
            this.lblDurfarC.Text = "DurfarC";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(18, 445);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(255, 36);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(68, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(148, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "(Martin Bédard)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(78, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 31);
            this.label6.TabIndex = 6;
            this.label6.Text = "MartsINY";
            // 
            // lblGoldenShades
            // 
            this.lblGoldenShades.AutoSize = true;
            this.lblGoldenShades.Location = new System.Drawing.Point(15, 350);
            this.lblGoldenShades.Name = "lblGoldenShades";
            this.lblGoldenShades.Size = new System.Drawing.Size(105, 16);
            this.lblGoldenShades.TabIndex = 7;
            this.lblGoldenShades.Text = "Golden_Shades";
            // 
            // txtMatrixz
            // 
            this.txtMatrixz.Enabled = false;
            this.txtMatrixz.Location = new System.Drawing.Point(18, 180);
            this.txtMatrixz.Name = "txtMatrixz";
            this.txtMatrixz.Size = new System.Drawing.Size(236, 57);
            this.txtMatrixz.TabIndex = 8;
            this.txtMatrixz.Text = "Based from his amazing documentation of Sound Engine.";
            // 
            // txtDurfarC
            // 
            this.txtDurfarC.Enabled = false;
            this.txtDurfarC.Location = new System.Drawing.Point(18, 280);
            this.txtDurfarC.Name = "txtDurfarC";
            this.txtDurfarC.Size = new System.Drawing.Size(236, 57);
            this.txtDurfarC.TabIndex = 9;
            this.txtDurfarC.Text = "Helped to identify some instructions.";
            // 
            // txtGoldenShades
            // 
            this.txtGoldenShades.Enabled = false;
            this.txtGoldenShades.Location = new System.Drawing.Point(18, 380);
            this.txtGoldenShades.Name = "txtGoldenShades";
            this.txtGoldenShades.Size = new System.Drawing.Size(236, 57);
            this.txtGoldenShades.TabIndex = 10;
            this.txtGoldenShades.Text = "Helped to identify some instructions.";
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 487);
            this.Controls.Add(this.txtGoldenShades);
            this.Controls.Add(this.txtDurfarC);
            this.Controls.Add(this.txtMatrixz);
            this.Controls.Add(this.lblGoldenShades);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblDurfarC);
            this.Controls.Add(this.lblMatrixz);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAbout";
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMatrixz;
        private System.Windows.Forms.Label lblDurfarC;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblGoldenShades;
        private System.Windows.Forms.RichTextBox txtMatrixz;
        private System.Windows.Forms.RichTextBox txtDurfarC;
        private System.Windows.Forms.RichTextBox txtGoldenShades;
    }
}