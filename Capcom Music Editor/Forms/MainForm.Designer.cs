namespace Mega_Music_Editor
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.txtSongRAM_Address = new System.Windows.Forms.TextBox();
            this.txtROM_Bank = new System.Windows.Forms.TextBox();
            this.txtRAM_Bank = new System.Windows.Forms.TextBox();
            this.lblSongRAM_Address = new System.Windows.Forms.Label();
            this.lblFirstPointer = new System.Windows.Forms.Label();
            this.lblROM_Bank = new System.Windows.Forms.Label();
            this.btnReadSong = new System.Windows.Forms.Button();
            this.ofdSelect = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtChannel1 = new System.Windows.Forms.TextBox();
            this.txtChannel4 = new System.Windows.Forms.TextBox();
            this.txtChannel2 = new System.Windows.Forms.TextBox();
            this.txtChannel3 = new System.Windows.Forms.TextBox();
            this.btnLoadAddressesForA_Song = new System.Windows.Forms.Button();
            this.cbxSongList = new System.Windows.Forms.ComboBox();
            this.btnLoadAnotherSongList = new System.Windows.Forms.Button();
            this.gbxSheetSelection = new System.Windows.Forms.GroupBox();
            this.rdBreakSheet = new System.Windows.Forms.RadioButton();
            this.rdLoopSheet = new System.Windows.Forms.RadioButton();
            this.rdMusicSheet = new System.Windows.Forms.RadioButton();
            this.btnCreateSong = new System.Windows.Forms.Button();
            this.menDebug = new System.Windows.Forms.MenuStrip();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmKeepInstructionsOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmWriteConnectToggleInMusicSheet = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsDoNotWriteChannelEnd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.readSongToTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAdvancedColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usePianoKeysForNotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSongName = new System.Windows.Forms.TextBox();
            this.gbxConsoleSelection = new System.Windows.Forms.GroupBox();
            this.rdSnes = new System.Windows.Forms.RadioButton();
            this.rdNes = new System.Windows.Forms.RadioButton();
            this.lblNameOfSongForTextFileExportation = new System.Windows.Forms.Label();
            this.lblNameOfSongForTextFileExportation_Warning = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFirstByteRead = new System.Windows.Forms.Label();
            this.txtFirstByteRead = new System.Windows.Forms.TextBox();
            this.txtSnesMemoryLoadPointer = new System.Windows.Forms.TextBox();
            this.lblSnesMemoryLoadPointer = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.gbxSheetSelection.SuspendLayout();
            this.menDebug.SuspendLayout();
            this.gbxConsoleSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSongRAM_Address
            // 
            this.txtSongRAM_Address.Location = new System.Drawing.Point(183, 134);
            this.txtSongRAM_Address.Margin = new System.Windows.Forms.Padding(4);
            this.txtSongRAM_Address.Name = "txtSongRAM_Address";
            this.txtSongRAM_Address.Size = new System.Drawing.Size(132, 22);
            this.txtSongRAM_Address.TabIndex = 23;
            // 
            // txtROM_Bank
            // 
            this.txtROM_Bank.Location = new System.Drawing.Point(183, 107);
            this.txtROM_Bank.Margin = new System.Windows.Forms.Padding(4);
            this.txtROM_Bank.Name = "txtROM_Bank";
            this.txtROM_Bank.Size = new System.Drawing.Size(132, 22);
            this.txtROM_Bank.TabIndex = 22;
            // 
            // txtRAM_Bank
            // 
            this.txtRAM_Bank.Location = new System.Drawing.Point(183, 80);
            this.txtRAM_Bank.Margin = new System.Windows.Forms.Padding(4);
            this.txtRAM_Bank.Name = "txtRAM_Bank";
            this.txtRAM_Bank.Size = new System.Drawing.Size(132, 22);
            this.txtRAM_Bank.TabIndex = 21;
            // 
            // lblSongRAM_Address
            // 
            this.lblSongRAM_Address.AutoSize = true;
            this.lblSongRAM_Address.Location = new System.Drawing.Point(15, 134);
            this.lblSongRAM_Address.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSongRAM_Address.Name = "lblSongRAM_Address";
            this.lblSongRAM_Address.Size = new System.Drawing.Size(141, 17);
            this.lblSongRAM_Address.TabIndex = 20;
            this.lblSongRAM_Address.Text = "RAM Address (Song)";
            // 
            // lblFirstPointer
            // 
            this.lblFirstPointer.AutoSize = true;
            this.lblFirstPointer.Location = new System.Drawing.Point(16, 80);
            this.lblFirstPointer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFirstPointer.Name = "lblFirstPointer";
            this.lblFirstPointer.Size = new System.Drawing.Size(74, 17);
            this.lblFirstPointer.TabIndex = 19;
            this.lblFirstPointer.Text = "RAM Bank";
            // 
            // lblROM_Bank
            // 
            this.lblROM_Bank.AutoSize = true;
            this.lblROM_Bank.Location = new System.Drawing.Point(15, 107);
            this.lblROM_Bank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblROM_Bank.Name = "lblROM_Bank";
            this.lblROM_Bank.Size = new System.Drawing.Size(129, 17);
            this.lblROM_Bank.TabIndex = 18;
            this.lblROM_Bank.Text = "ROM Starting Point";
            // 
            // btnReadSong
            // 
            this.btnReadSong.Location = new System.Drawing.Point(16, 189);
            this.btnReadSong.Margin = new System.Windows.Forms.Padding(4);
            this.btnReadSong.Name = "btnReadSong";
            this.btnReadSong.Size = new System.Drawing.Size(380, 28);
            this.btnReadSong.TabIndex = 17;
            this.btnReadSong.Text = "Read Song";
            this.btnReadSong.UseVisualStyleBackColor = true;
            this.btnReadSong.Click += new System.EventHandler(this.BtnReadSong_Click);
            // 
            // ofdSelect
            // 
            this.ofdSelect.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtChannel1);
            this.groupBox1.Controls.Add(this.txtChannel4);
            this.groupBox1.Controls.Add(this.txtChannel2);
            this.groupBox1.Controls.Add(this.txtChannel3);
            this.groupBox1.Location = new System.Drawing.Point(56, 236);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(927, 364);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chan Hex Codes";
            this.groupBox1.Visible = false;
            // 
            // txtChannel1
            // 
            this.txtChannel1.Location = new System.Drawing.Point(19, 55);
            this.txtChannel1.Margin = new System.Windows.Forms.Padding(4);
            this.txtChannel1.Multiline = true;
            this.txtChannel1.Name = "txtChannel1";
            this.txtChannel1.Size = new System.Drawing.Size(896, 61);
            this.txtChannel1.TabIndex = 28;
            // 
            // txtChannel4
            // 
            this.txtChannel4.Location = new System.Drawing.Point(19, 306);
            this.txtChannel4.Margin = new System.Windows.Forms.Padding(4);
            this.txtChannel4.Multiline = true;
            this.txtChannel4.Name = "txtChannel4";
            this.txtChannel4.Size = new System.Drawing.Size(896, 64);
            this.txtChannel4.TabIndex = 31;
            // 
            // txtChannel2
            // 
            this.txtChannel2.Location = new System.Drawing.Point(19, 137);
            this.txtChannel2.Margin = new System.Windows.Forms.Padding(4);
            this.txtChannel2.Multiline = true;
            this.txtChannel2.Name = "txtChannel2";
            this.txtChannel2.Size = new System.Drawing.Size(896, 57);
            this.txtChannel2.TabIndex = 29;
            // 
            // txtChannel3
            // 
            this.txtChannel3.Location = new System.Drawing.Point(19, 220);
            this.txtChannel3.Margin = new System.Windows.Forms.Padding(4);
            this.txtChannel3.Multiline = true;
            this.txtChannel3.Name = "txtChannel3";
            this.txtChannel3.Size = new System.Drawing.Size(896, 58);
            this.txtChannel3.TabIndex = 30;
            // 
            // btnLoadAddressesForA_Song
            // 
            this.btnLoadAddressesForA_Song.Location = new System.Drawing.Point(324, 78);
            this.btnLoadAddressesForA_Song.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadAddressesForA_Song.Name = "btnLoadAddressesForA_Song";
            this.btnLoadAddressesForA_Song.Size = new System.Drawing.Size(256, 51);
            this.btnLoadAddressesForA_Song.TabIndex = 29;
            this.btnLoadAddressesForA_Song.Text = "Load Addresses For Selected Song";
            this.btnLoadAddressesForA_Song.UseVisualStyleBackColor = true;
            this.btnLoadAddressesForA_Song.Click += new System.EventHandler(this.BtnLoadAddressesForA_Song_Click);
            // 
            // cbxSongList
            // 
            this.cbxSongList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSongList.Enabled = false;
            this.cbxSongList.FormattingEnabled = true;
            this.cbxSongList.Location = new System.Drawing.Point(183, 44);
            this.cbxSongList.Margin = new System.Windows.Forms.Padding(4);
            this.cbxSongList.Name = "cbxSongList";
            this.cbxSongList.Size = new System.Drawing.Size(625, 24);
            this.cbxSongList.TabIndex = 30;
            // 
            // btnLoadAnotherSongList
            // 
            this.btnLoadAnotherSongList.Location = new System.Drawing.Point(16, 42);
            this.btnLoadAnotherSongList.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadAnotherSongList.Name = "btnLoadAnotherSongList";
            this.btnLoadAnotherSongList.Size = new System.Drawing.Size(131, 28);
            this.btnLoadAnotherSongList.TabIndex = 31;
            this.btnLoadAnotherSongList.Text = "Load Game File";
            this.btnLoadAnotherSongList.UseVisualStyleBackColor = true;
            this.btnLoadAnotherSongList.Click += new System.EventHandler(this.BtnLoadAnotherSongList_Click);
            // 
            // gbxSheetSelection
            // 
            this.gbxSheetSelection.Controls.Add(this.rdBreakSheet);
            this.gbxSheetSelection.Controls.Add(this.rdLoopSheet);
            this.gbxSheetSelection.Controls.Add(this.rdMusicSheet);
            this.gbxSheetSelection.Location = new System.Drawing.Point(1135, 44);
            this.gbxSheetSelection.Margin = new System.Windows.Forms.Padding(4);
            this.gbxSheetSelection.Name = "gbxSheetSelection";
            this.gbxSheetSelection.Padding = new System.Windows.Forms.Padding(4);
            this.gbxSheetSelection.Size = new System.Drawing.Size(148, 161);
            this.gbxSheetSelection.TabIndex = 32;
            this.gbxSheetSelection.TabStop = false;
            this.gbxSheetSelection.Text = "Sheet";
            // 
            // rdBreakSheet
            // 
            this.rdBreakSheet.AutoSize = true;
            this.rdBreakSheet.Location = new System.Drawing.Point(13, 111);
            this.rdBreakSheet.Margin = new System.Windows.Forms.Padding(4);
            this.rdBreakSheet.Name = "rdBreakSheet";
            this.rdBreakSheet.Size = new System.Drawing.Size(107, 21);
            this.rdBreakSheet.TabIndex = 2;
            this.rdBreakSheet.Text = "Break Sheet";
            this.rdBreakSheet.UseVisualStyleBackColor = true;
            this.rdBreakSheet.CheckedChanged += new System.EventHandler(this.RdBreakSheet_CheckedChanged);
            // 
            // rdLoopSheet
            // 
            this.rdLoopSheet.AutoSize = true;
            this.rdLoopSheet.Location = new System.Drawing.Point(13, 69);
            this.rdLoopSheet.Margin = new System.Windows.Forms.Padding(4);
            this.rdLoopSheet.Name = "rdLoopSheet";
            this.rdLoopSheet.Size = new System.Drawing.Size(102, 21);
            this.rdLoopSheet.TabIndex = 1;
            this.rdLoopSheet.Text = "Loop Sheet";
            this.rdLoopSheet.UseVisualStyleBackColor = true;
            this.rdLoopSheet.CheckedChanged += new System.EventHandler(this.RdLoopSheet_CheckedChanged);
            // 
            // rdMusicSheet
            // 
            this.rdMusicSheet.AutoSize = true;
            this.rdMusicSheet.Checked = true;
            this.rdMusicSheet.Location = new System.Drawing.Point(13, 27);
            this.rdMusicSheet.Margin = new System.Windows.Forms.Padding(4);
            this.rdMusicSheet.Name = "rdMusicSheet";
            this.rdMusicSheet.Size = new System.Drawing.Size(106, 21);
            this.rdMusicSheet.TabIndex = 0;
            this.rdMusicSheet.TabStop = true;
            this.rdMusicSheet.Text = "Music Sheet";
            this.rdMusicSheet.UseVisualStyleBackColor = true;
            this.rdMusicSheet.CheckedChanged += new System.EventHandler(this.RdMusicSheet_CheckedChanged);
            // 
            // btnCreateSong
            // 
            this.btnCreateSong.Location = new System.Drawing.Point(428, 189);
            this.btnCreateSong.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateSong.Name = "btnCreateSong";
            this.btnCreateSong.Size = new System.Drawing.Size(380, 28);
            this.btnCreateSong.TabIndex = 33;
            this.btnCreateSong.Text = "Create Song";
            this.btnCreateSong.UseVisualStyleBackColor = true;
            this.btnCreateSong.Click += new System.EventHandler(this.BtnCreateSong_Click);
            // 
            // menDebug
            // 
            this.menDebug.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menDebug.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem,
            this.tsmAdvanced,
            this.tmsAbout});
            this.menDebug.Location = new System.Drawing.Point(0, 0);
            this.menDebug.Name = "menDebug";
            this.menDebug.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menDebug.Size = new System.Drawing.Size(1924, 28);
            this.menDebug.TabIndex = 37;
            this.menDebug.Text = "menuStrip1";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmKeepInstructionsOrder,
            this.tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine,
            this.tsmWriteConnectToggleInMusicSheet,
            this.tmsDoNotWriteChannelEnd});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.debugToolStripMenuItem.Text = "&Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.DebugToolStripMenuItem_Click);
            // 
            // tsmKeepInstructionsOrder
            // 
            this.tsmKeepInstructionsOrder.Name = "tsmKeepInstructionsOrder";
            this.tsmKeepInstructionsOrder.Size = new System.Drawing.Size(446, 26);
            this.tsmKeepInstructionsOrder.Text = "&Keep Instructions Order";
            this.tsmKeepInstructionsOrder.Click += new System.EventHandler(this.TsmKeepInstructionsOrder_Click);
            // 
            // tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine
            // 
            this.tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine.Name = "tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine";
            this.tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine.Size = new System.Drawing.Size(446, 26);
            this.tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine.Text = "&Ensure Jumped On Instructions Are Alone On Their Line";
            this.tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine.Click += new System.EventHandler(this.TsmEnsureJumpedOnInstructionsAreAloneOnTheirLine_Click);
            // 
            // tsmWriteConnectToggleInMusicSheet
            // 
            this.tsmWriteConnectToggleInMusicSheet.Name = "tsmWriteConnectToggleInMusicSheet";
            this.tsmWriteConnectToggleInMusicSheet.Size = new System.Drawing.Size(446, 26);
            this.tsmWriteConnectToggleInMusicSheet.Text = "&Write Connect Toggles In Music Sheet";
            this.tsmWriteConnectToggleInMusicSheet.Click += new System.EventHandler(this.TsmWriteConnectToggleInMusicSheet_Click);
            // 
            // tmsDoNotWriteChannelEnd
            // 
            this.tmsDoNotWriteChannelEnd.Name = "tmsDoNotWriteChannelEnd";
            this.tmsDoNotWriteChannelEnd.Size = new System.Drawing.Size(446, 26);
            this.tmsDoNotWriteChannelEnd.Text = "&Do Not Write Channel End";
            this.tmsDoNotWriteChannelEnd.Click += new System.EventHandler(this.TmsDoNotWriteChannelEnd_Click);
            // 
            // tsmAdvanced
            // 
            this.tsmAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readSongToTextToolStripMenuItem,
            this.showAdvancedColumnsToolStripMenuItem,
            this.usePianoKeysForNotesToolStripMenuItem});
            this.tsmAdvanced.Name = "tsmAdvanced";
            this.tsmAdvanced.Size = new System.Drawing.Size(87, 24);
            this.tsmAdvanced.Text = "&Advanced";
            // 
            // readSongToTextToolStripMenuItem
            // 
            this.readSongToTextToolStripMenuItem.Name = "readSongToTextToolStripMenuItem";
            this.readSongToTextToolStripMenuItem.Size = new System.Drawing.Size(251, 26);
            this.readSongToTextToolStripMenuItem.Text = "&Read Song - To Text";
            this.readSongToTextToolStripMenuItem.Click += new System.EventHandler(this.ReadSongToTextToolStripMenuItem_Click);
            // 
            // showAdvancedColumnsToolStripMenuItem
            // 
            this.showAdvancedColumnsToolStripMenuItem.Name = "showAdvancedColumnsToolStripMenuItem";
            this.showAdvancedColumnsToolStripMenuItem.Size = new System.Drawing.Size(251, 26);
            this.showAdvancedColumnsToolStripMenuItem.Text = "&Show Advanced Columns";
            this.showAdvancedColumnsToolStripMenuItem.Click += new System.EventHandler(this.ShowAdvancedColumnsToolStripMenuItem_Click);
            // 
            // usePianoKeysForNotesToolStripMenuItem
            // 
            this.usePianoKeysForNotesToolStripMenuItem.Name = "usePianoKeysForNotesToolStripMenuItem";
            this.usePianoKeysForNotesToolStripMenuItem.Size = new System.Drawing.Size(251, 26);
            this.usePianoKeysForNotesToolStripMenuItem.Text = "&Use Piano Keys for notes";
            this.usePianoKeysForNotesToolStripMenuItem.Click += new System.EventHandler(this.UsePianoKeysForNotesToolStripMenuItem_Click);
            // 
            // tmsAbout
            // 
            this.tmsAbout.Name = "tmsAbout";
            this.tmsAbout.Size = new System.Drawing.Size(62, 24);
            this.tmsAbout.Text = "A&bout";
            this.tmsAbout.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // txtSongName
            // 
            this.txtSongName.Location = new System.Drawing.Point(588, 134);
            this.txtSongName.Margin = new System.Windows.Forms.Padding(4);
            this.txtSongName.Name = "txtSongName";
            this.txtSongName.Size = new System.Drawing.Size(220, 22);
            this.txtSongName.TabIndex = 39;
            // 
            // gbxConsoleSelection
            // 
            this.gbxConsoleSelection.Controls.Add(this.rdSnes);
            this.gbxConsoleSelection.Controls.Add(this.rdNes);
            this.gbxConsoleSelection.Location = new System.Drawing.Point(1401, 44);
            this.gbxConsoleSelection.Margin = new System.Windows.Forms.Padding(4);
            this.gbxConsoleSelection.Name = "gbxConsoleSelection";
            this.gbxConsoleSelection.Padding = new System.Windows.Forms.Padding(4);
            this.gbxConsoleSelection.Size = new System.Drawing.Size(91, 112);
            this.gbxConsoleSelection.TabIndex = 42;
            this.gbxConsoleSelection.TabStop = false;
            this.gbxConsoleSelection.Text = "Console";
            // 
            // rdSnes
            // 
            this.rdSnes.AutoSize = true;
            this.rdSnes.Location = new System.Drawing.Point(13, 69);
            this.rdSnes.Margin = new System.Windows.Forms.Padding(4);
            this.rdSnes.Name = "rdSnes";
            this.rdSnes.Size = new System.Drawing.Size(61, 21);
            this.rdSnes.TabIndex = 1;
            this.rdSnes.Text = "Snes";
            this.rdSnes.UseVisualStyleBackColor = true;
            this.rdSnes.CheckedChanged += new System.EventHandler(this.RdSnes_CheckedChanged);
            // 
            // rdNes
            // 
            this.rdNes.AutoSize = true;
            this.rdNes.Location = new System.Drawing.Point(13, 27);
            this.rdNes.Margin = new System.Windows.Forms.Padding(4);
            this.rdNes.Name = "rdNes";
            this.rdNes.Size = new System.Drawing.Size(54, 21);
            this.rdNes.TabIndex = 0;
            this.rdNes.Text = "Nes";
            this.rdNes.UseVisualStyleBackColor = true;
            this.rdNes.CheckedChanged += new System.EventHandler(this.RdNes_CheckedChanged);
            // 
            // lblNameOfSongForTextFileExportation
            // 
            this.lblNameOfSongForTextFileExportation.AutoSize = true;
            this.lblNameOfSongForTextFileExportation.Location = new System.Drawing.Point(588, 85);
            this.lblNameOfSongForTextFileExportation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNameOfSongForTextFileExportation.Name = "lblNameOfSongForTextFileExportation";
            this.lblNameOfSongForTextFileExportation.Size = new System.Drawing.Size(96, 17);
            this.lblNameOfSongForTextFileExportation.TabIndex = 43;
            this.lblNameOfSongForTextFileExportation.Text = "Name of song";
            // 
            // lblNameOfSongForTextFileExportation_Warning
            // 
            this.lblNameOfSongForTextFileExportation_Warning.AutoSize = true;
            this.lblNameOfSongForTextFileExportation_Warning.Location = new System.Drawing.Point(588, 107);
            this.lblNameOfSongForTextFileExportation_Warning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNameOfSongForTextFileExportation_Warning.Name = "lblNameOfSongForTextFileExportation_Warning";
            this.lblNameOfSongForTextFileExportation_Warning.Size = new System.Drawing.Size(222, 17);
            this.lblNameOfSongForTextFileExportation_Warning.TabIndex = 44;
            this.lblNameOfSongForTextFileExportation_Warning.Text = "(only used for text file exportation)";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(1016, 317);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            this.lblStatus.TabIndex = 46;
            this.lblStatus.Visible = false;
            // 
            // lblFirstByteRead
            // 
            this.lblFirstByteRead.AutoSize = true;
            this.lblFirstByteRead.Location = new System.Drawing.Point(324, 134);
            this.lblFirstByteRead.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFirstByteRead.Name = "lblFirstByteRead";
            this.lblFirstByteRead.Size = new System.Drawing.Size(105, 17);
            this.lblFirstByteRead.TabIndex = 49;
            this.lblFirstByteRead.Text = "First Byte Read";
            // 
            // txtFirstByteRead
            // 
            this.txtFirstByteRead.Enabled = false;
            this.txtFirstByteRead.Location = new System.Drawing.Point(450, 134);
            this.txtFirstByteRead.Name = "txtFirstByteRead";
            this.txtFirstByteRead.Size = new System.Drawing.Size(130, 22);
            this.txtFirstByteRead.TabIndex = 50;
            // 
            // txtSnesMemoryLoadPointer
            // 
            this.txtSnesMemoryLoadPointer.Location = new System.Drawing.Point(183, 161);
            this.txtSnesMemoryLoadPointer.Margin = new System.Windows.Forms.Padding(4);
            this.txtSnesMemoryLoadPointer.Name = "txtSnesMemoryLoadPointer";
            this.txtSnesMemoryLoadPointer.Size = new System.Drawing.Size(132, 22);
            this.txtSnesMemoryLoadPointer.TabIndex = 52;
            this.txtSnesMemoryLoadPointer.Visible = false;
            // 
            // lblSnesMemoryLoadPointer
            // 
            this.lblSnesMemoryLoadPointer.AutoSize = true;
            this.lblSnesMemoryLoadPointer.Location = new System.Drawing.Point(15, 161);
            this.lblSnesMemoryLoadPointer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSnesMemoryLoadPointer.Name = "lblSnesMemoryLoadPointer";
            this.lblSnesMemoryLoadPointer.Size = new System.Drawing.Size(143, 17);
            this.lblSnesMemoryLoadPointer.TabIndex = 51;
            this.lblSnesMemoryLoadPointer.Text = "Memory Load Pointer";
            this.lblSnesMemoryLoadPointer.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 846);
            this.Controls.Add(this.txtSnesMemoryLoadPointer);
            this.Controls.Add(this.lblSnesMemoryLoadPointer);
            this.Controls.Add(this.txtFirstByteRead);
            this.Controls.Add(this.lblFirstByteRead);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblNameOfSongForTextFileExportation_Warning);
            this.Controls.Add(this.lblNameOfSongForTextFileExportation);
            this.Controls.Add(this.gbxConsoleSelection);
            this.Controls.Add(this.txtSongName);
            this.Controls.Add(this.btnCreateSong);
            this.Controls.Add(this.gbxSheetSelection);
            this.Controls.Add(this.btnLoadAnotherSongList);
            this.Controls.Add(this.cbxSongList);
            this.Controls.Add(this.btnLoadAddressesForA_Song);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtSongRAM_Address);
            this.Controls.Add(this.txtROM_Bank);
            this.Controls.Add(this.txtRAM_Bank);
            this.Controls.Add(this.lblSongRAM_Address);
            this.Controls.Add(this.lblFirstPointer);
            this.Controls.Add(this.lblROM_Bank);
            this.Controls.Add(this.btnReadSong);
            this.Controls.Add(this.menDebug);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menDebug;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Capcom Music Editor";
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbxSheetSelection.ResumeLayout(false);
            this.gbxSheetSelection.PerformLayout();
            this.menDebug.ResumeLayout(false);
            this.menDebug.PerformLayout();
            this.gbxConsoleSelection.ResumeLayout(false);
            this.gbxConsoleSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSongRAM_Address;
        private System.Windows.Forms.TextBox txtROM_Bank;
        private System.Windows.Forms.TextBox txtRAM_Bank;
        private System.Windows.Forms.Label lblSongRAM_Address;
        private System.Windows.Forms.Label lblFirstPointer;
        private System.Windows.Forms.Label lblROM_Bank;
        private System.Windows.Forms.Button btnReadSong;
        private System.Windows.Forms.OpenFileDialog ofdSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtChannel1;
        private System.Windows.Forms.TextBox txtChannel4;
        private System.Windows.Forms.TextBox txtChannel2;
        private System.Windows.Forms.TextBox txtChannel3;
        private System.Windows.Forms.Button btnLoadAddressesForA_Song;
        private System.Windows.Forms.ComboBox cbxSongList;
        private System.Windows.Forms.Button btnLoadAnotherSongList;
        private System.Windows.Forms.GroupBox gbxSheetSelection;
        private System.Windows.Forms.RadioButton rdBreakSheet;
        private System.Windows.Forms.RadioButton rdLoopSheet;
        private System.Windows.Forms.RadioButton rdMusicSheet;
        private System.Windows.Forms.Button btnCreateSong;
        private System.Windows.Forms.MenuStrip menDebug;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmKeepInstructionsOrder;
        private System.Windows.Forms.ToolStripMenuItem tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine;
        private System.Windows.Forms.TextBox txtSongName;
        private System.Windows.Forms.ToolStripMenuItem tsmAdvanced;
        private System.Windows.Forms.ToolStripMenuItem readSongToTextToolStripMenuItem;
        private System.Windows.Forms.GroupBox gbxConsoleSelection;
        private System.Windows.Forms.RadioButton rdSnes;
        private System.Windows.Forms.RadioButton rdNes;
        private System.Windows.Forms.ToolStripMenuItem showAdvancedColumnsToolStripMenuItem;
        private System.Windows.Forms.Label lblNameOfSongForTextFileExportation;
        private System.Windows.Forms.Label lblNameOfSongForTextFileExportation_Warning;
        private System.Windows.Forms.ToolStripMenuItem tsmWriteConnectToggleInMusicSheet;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolStripMenuItem tmsDoNotWriteChannelEnd;
        private System.Windows.Forms.ToolStripMenuItem tmsAbout;
        private System.Windows.Forms.ToolStripMenuItem usePianoKeysForNotesToolStripMenuItem;
        private System.Windows.Forms.Label lblFirstByteRead;
        private System.Windows.Forms.TextBox txtFirstByteRead;
        private System.Windows.Forms.TextBox txtSnesMemoryLoadPointer;
        private System.Windows.Forms.Label lblSnesMemoryLoadPointer;
    }
}

