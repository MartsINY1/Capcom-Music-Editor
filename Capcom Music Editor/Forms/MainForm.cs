using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mega_Music_Editor.Reusable;
using Mega_Music_Editor.Unique;

namespace Mega_Music_Editor
{
    public struct SongLocalisationDatas
    {
        public Hex _soundArrayROM_Address;
        public int _soundID;

        public SongLocalisationDatas(Hex soundArrayROM_Address, int soundID)
        {
            _soundArrayROM_Address = soundArrayROM_Address;
            _soundID = soundID;
        }
    }

    public partial class FrmMain : Form
    {
        // Constants
        private const int _hexCodeQtyByAddress = 2;
        private int _channelQty = 4;

        // Variables
        private string _path = "Please select a ROM file";
        private readonly string _pathFileReading = @"\_File_Reading";
        private List<SongLocalisationDatas> _soundLocalisationDatas = null;
        private RAM_ROM_Bank_Association _RAM_ROM_Bank_Association = null;
        DataGridViewsHandler dataGridViewsHandler = null;
        private int counter = 0;
        private bool keepMusicSheetInstructionsOrder = false;
        private bool ensureJumpedInstructionAreAloneOnTheirLine = false;
        private bool writeConnectInMusicSheet = true;
        private bool doNotWriteChannelEnd = false;
        private bool stupidBool = false; // Use to sol a stupid windows but that changes scroll bar position of sheets
        private bool usePianoKeysForNotes = false;

        #region Functions
        private void UnknownExceptionHandling(Exception ex)
        {
            var s = new System.Diagnostics.StackTrace(ex);
            var thisasm = System.Reflection.Assembly.GetExecutingAssembly();
            var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == thisasm).Name;

            ExceptionsCatcher.ShowErrorWithMethodName(methodName);
        }

        private void CloseFile(ref StreamReader file)
        {
            if (file != null)
            {
                file.Dispose();
                file.Close();
            }
        }

        private void ChangeGameTypeInForm(GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                rdNes.Checked = true;
            }
            else if (gameType == GameType.SnesA)
            {
                rdSnes.Checked = true;
            }
        }

        private int LoadSongDatas(string filePath, out string errorMsg)
        {
            bool stayIn = true;
            int y = 0;
            int qtySongInBank = 0;
            double tempDbl = 0;
            string lineRead = "";
            string tempStr = "";
            string RAM_StartReadStr = "", RAM_EndReadStr = "", ROM_ReadStr = "", ROM_SongListStartReadStr = "", ROM_SongListEndReadStr = "";
            Hex RAM_StartReadHex, RAM_EndReadHex, ROM_ReadHex, ROM_SongListStartReadHex, ROM_SongListEndReadHex;
            StreamReader fileOfGameToLoad = null;

            errorMsg = "";
            _RAM_ROM_Bank_Association = new RAM_ROM_Bank_Association();

            // We build a new sound list
            _soundLocalisationDatas = new List<SongLocalisationDatas>();
            cbxSongList.Items.Clear();
            cbxSongList.ResetText();
            cbxSongList.Enabled = false;

            try
            {
                // Open file for reading
                if (FileReader.OpenFile(out fileOfGameToLoad, filePath))
                {
                    while (stayIn)
                    {
                        lineRead = fileOfGameToLoad.ReadLine();

                        if (lineRead == null)
                        {
                            errorMsg = "File content is invalid.";
                            CloseFile(ref fileOfGameToLoad);
                            return 11;
                        }

                        if (lineRead.Length > 0)
                        {
                            if (lineRead[0] == '=')
                            {
                                lineRead = fileOfGameToLoad.ReadLine();

                                if (lineRead == "Song_Names")
                                {
                                    // Skip a "==" line
                                    fileOfGameToLoad.ReadLine();

                                    while ((lineRead = fileOfGameToLoad.ReadLine()) != null)
                                    {
                                        cbxSongList.Items.Add(lineRead);

                                        if (cbxSongList.Items.Count > _soundLocalisationDatas.Count)
                                        {
                                            errorMsg = "There are more song names than there are songs read from ROM.";
                                            CloseFile(ref fileOfGameToLoad);
                                            return 10;
                                        }
                                    }

                                    // Reading finished
                                    stayIn = false;
                                }
                                else
                                {
                                    tempStr = lineRead.Substring(0, lineRead.IndexOf(":"));

                                    if (tempStr == "ROM_Song_List_Start")
                                    {
                                        ROM_SongListStartReadStr = lineRead;
                                        ROM_SongListEndReadStr = fileOfGameToLoad.ReadLine();

                                        if (ROM_SongListStartReadStr == null || ROM_SongListEndReadStr == null)
                                        {
                                            // Song pointers array datas is missing
                                            errorMsg = "Song pointers array datas is missing.";
                                            CloseFile(ref fileOfGameToLoad);
                                            return 2;
                                        }

                                        ROM_SongListStartReadStr = General.ReturnStringAfterColon(ROM_SongListStartReadStr);
                                        ROM_SongListEndReadStr = General.ReturnStringAfterColon(ROM_SongListEndReadStr);

                                        if (!Hex.IsHex(ROM_SongListStartReadStr))
                                        {
                                            errorMsg = "Following Song List Start is not a valid format: " + ROM_SongListStartReadStr;
                                            CloseFile(ref fileOfGameToLoad);
                                            return 5;
                                        }
                                        if (!Hex.IsHex(ROM_SongListEndReadStr))
                                        {
                                            errorMsg = "Following Song List End is not a valid format: " + ROM_SongListEndReadStr;
                                            CloseFile(ref fileOfGameToLoad);
                                            return 6;
                                        }
                                        
                                        // Here hex are known to be valid values
                                        ROM_SongListStartReadHex = new Hex(ROM_SongListStartReadStr);
                                        ROM_SongListEndReadHex = new Hex(ROM_SongListEndReadStr);
                                        
                                        if (ROM_SongListEndReadHex < ROM_SongListStartReadHex)
                                        {
                                            errorMsg = "Following ROM Song Start is bigger than ROM Song End: " + System.Environment.NewLine + ROM_SongListStartReadHex.GetValueAsString(0) + System.Environment.NewLine + ROM_SongListEndReadHex.GetValueAsString(0);
                                            CloseFile(ref fileOfGameToLoad);
                                            return 7;
                                        }

                                        // Calculate quantity of songs in current bank
                                        qtySongInBank = (ROM_SongListEndReadHex - ROM_SongListStartReadHex).GetValueAsInt();

                                        tempDbl = qtySongInBank;

                                        if ((tempDbl / 2) == 0)
                                        {
                                            errorMsg = "ROM_Song_List_Start and ROM_Song_List_End values are incorrect. The quantity of hex code between them is supposed to be divisible by 2. Here is the Start and End that are incorrect" +
                                                System.Environment.NewLine + ROM_SongListStartReadHex.GetValueAsString(0) + System.Environment.NewLine + ROM_SongListEndReadHex.GetValueAsString(0);
                                            CloseFile(ref fileOfGameToLoad);
                                            return 9;
                                        }

                                        qtySongInBank /= 2;

                                        // Add datas to list to defineeach song
                                        for (int x = 0; x < qtySongInBank; x++)
                                        {
                                            _soundLocalisationDatas.Add(new SongLocalisationDatas(ROM_SongListStartReadHex, y));
                                            y += 1;
                                        }
                                    }
                                    else if (tempStr == "ROM_Start") // Snes games
                                    {
                                        ROM_ReadStr = General.ReturnStringAfterColon(lineRead);

                                        if (ROM_ReadStr == null)
                                        {
                                            // ROM value is missing
                                            errorMsg = "ROM start value is incomplete.";
                                            CloseFile(ref fileOfGameToLoad);
                                            return 2;
                                        }
                                        if (!Hex.IsHex(ROM_ReadStr))
                                        {
                                            errorMsg = "Following ROM is not a valid format: " + ROM_ReadStr;
                                            CloseFile(ref fileOfGameToLoad);
                                            return 4;
                                        }

                                        // Here hex is known to be a valid values
                                        ROM_ReadHex = new Hex(ROM_ReadStr);

                                        // Format is mostly for nes, add 3 times the same ROM address
                                        _RAM_ROM_Bank_Association.AddDatas(ROM_ReadHex, ROM_ReadHex, ROM_ReadHex);
                                    }
                                    else // Nes games
                                    {
                                        RAM_StartReadStr = lineRead;
                                        RAM_EndReadStr = fileOfGameToLoad.ReadLine();
                                        ROM_ReadStr = fileOfGameToLoad.ReadLine();

                                        if (RAM_StartReadStr == null || RAM_EndReadStr == null || ROM_ReadStr == null)
                                        {
                                            // One RAM ROM association is missing
                                            errorMsg = "One RAM ROM association is incomplete.";
                                            CloseFile(ref fileOfGameToLoad);
                                            return 2;
                                        }

                                        RAM_StartReadStr = General.ReturnStringAfterColon(RAM_StartReadStr);
                                        RAM_EndReadStr = General.ReturnStringAfterColon(RAM_EndReadStr);
                                        ROM_ReadStr = General.ReturnStringAfterColon(ROM_ReadStr);
                                        
                                        if (!Hex.IsHex(RAM_StartReadStr))
                                        {
                                            errorMsg = "Following RAM is not a valid format: " + RAM_StartReadStr;
                                            CloseFile(ref fileOfGameToLoad);
                                            return 3;
                                        }
                                        if (!Hex.IsHex(RAM_EndReadStr))
                                        {
                                            errorMsg = "Following RAM is not a valid format: " + RAM_EndReadStr;
                                            CloseFile(ref fileOfGameToLoad);
                                            return 35;
                                        }
                                        if (!Hex.IsHex(ROM_ReadStr))
                                        {
                                            errorMsg = "Following ROM is not a valid format: " + ROM_ReadStr;
                                            CloseFile(ref fileOfGameToLoad);
                                            return 4;
                                        }
                                        
                                        // Here hex are known to be valid values
                                        RAM_StartReadHex = new Hex(RAM_StartReadStr);
                                        RAM_EndReadHex = new Hex(RAM_EndReadStr);
                                        ROM_ReadHex = new Hex(ROM_ReadStr);

                                        _RAM_ROM_Bank_Association.AddDatas(ROM_ReadHex, RAM_StartReadHex, RAM_EndReadHex);
                                    }                                    
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorMsg = "Cannot open file";
                    CloseFile(ref fileOfGameToLoad);
                    return 1;
                }

                // No problem encountered
                cbxSongList.Enabled = true;
            }
            catch (Exception)
            {
                // Unknown error
                CloseFile(ref fileOfGameToLoad);
                return 999;
            }

            CloseFile(ref fileOfGameToLoad);
            return 0;
        }
        #endregion

        #region Form

        // Override code to catch when a window is unminimized
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                // Check your window state here
                if (m.WParam == new IntPtr(0xF020)) // This is minimising event)
                {
                    dataGridViewsHandler.StoreScrollBarsPositions();
                }
                else  if (m.WParam == new IntPtr(0xF120)) // This is unminimized event
                {
                    stupidBool = true;
                }
            }
            base.WndProc(ref m);
        }
        public FrmMain()
        {
            InitializeComponent();
            UpdateDebugMenuChecked();
        }

        private void LoadGameFile(string fileLoadingPath, out string gameType)
        {
            string fileToLoadPath = ""; // Path from exe file
            string fileToLoadPathCompletePath = ""; // Complete path (not only from exe file)
            string defaultSongID = "";  // Number of first song datas to load on startup
            string errorMsg = ""; // Received by functions
            string temp = "";
            gameType = "";
            int defaultSongID_Int = 0;
            StreamReader fileDefaultLoading = null;
            StreamReader fileOfGameToLoad = null;

            try
            {
                try
                {
                    fileDefaultLoading = new StreamReader(fileLoadingPath);
                }
                catch (Exception)
                {
                    throw new KnownException("File for loading not found: " + fileLoadingPath);
                }

                // What is read from files
                fileToLoadPath = fileDefaultLoading.ReadLine();
                temp = fileDefaultLoading.ReadLine();
                _path = Application.StartupPath + _pathFileReading + @"\" + temp;
                defaultSongID = fileDefaultLoading.ReadLine();
                defaultSongID = General.ReturnStringAfterColon(defaultSongID);

                if (!(fileToLoadPath == null || defaultSongID == null || temp == null))
                {
                    defaultSongID = General.ReturnStringAfterColon(defaultSongID);

                    try
                    {
                        defaultSongID_Int = Int32.Parse(defaultSongID);
                    }
                    catch (Exception)
                    {
                        // The song ID read is invalid
                        throw new KnownException("Default song ID to load is invalid.");
                    }

                    try
                    {
                        fileToLoadPathCompletePath = Application.StartupPath + _pathFileReading + @"\" + fileToLoadPath;
                        fileOfGameToLoad = new StreamReader(fileToLoadPathCompletePath);

                        gameType = fileOfGameToLoad.ReadLine();
                    }
                    catch (Exception)
                    {
                        throw new KnownException("The first line in the default file is supposed to be the type (NesA, SnesA). The first line read is invalid: " + System.Environment.NewLine +
                            fileToLoadPath + System.Environment.NewLine +
                            "and the full loading path is: " + fileToLoadPathCompletePath);
                    }

                    // Here, we know path is valid, so call function that reads from said file
                    if (LoadSongDatas(fileToLoadPathCompletePath, out errorMsg) == 0)
                    {
                        // Position cursor in combobox
                        if (defaultSongID_Int <= cbxSongList.Items.Count)
                        {
                            cbxSongList.SelectedIndex = defaultSongID_Int;
                        }
                        else
                        {
                            ExceptionsCatcher.ShowError("Default song to select cannot be selected, it is out of bound");
                        }
                    }
                    else
                    {
                        // There was a known error and throw it
                        throw new KnownException(errorMsg);
                    }
                }
                else
                {
                    // If there is not 2 lines in the text file at least
                    ExceptionsCatcher.ShowError("Default loading value text file must have 3 lines.");
                }
            }
            catch (Exception)
            {
                txtRAM_Bank.Text = "";
                txtROM_Bank.Text = "";
                txtSongRAM_Address.Text = "";
            }

            // Close files
            if (fileOfGameToLoad != null)
            {
                fileOfGameToLoad.Dispose();
                fileOfGameToLoad.Close();
            }
            if (fileDefaultLoading != null)
            {
                fileDefaultLoading.Dispose();
                fileDefaultLoading.Close();
            }
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            string defaultLoadingFilePath = Application.StartupPath; // File that indicate information of automatic loading
            string gameTypeStr = "";
            string currentLoadingFilePath = ""; // Path from default txt file
            StreamReader defaultLoadingFileStreamReader = null;
            GameType gameType = GameType.NesA;

            // Create a datagridview object, gameType will be changed later if needed
            dataGridViewsHandler = new DataGridViewsHandler(this, GameType.NesA, ref gbxSheetSelection, ref gbxConsoleSelection);

            defaultLoadingFilePath += _pathFileReading + @"\Default_Loading.txt";

            try
            {
                try
                {
                    defaultLoadingFileStreamReader = new StreamReader(defaultLoadingFilePath);

                    try
                    {
                        // What is read from files
                        currentLoadingFilePath = Application.StartupPath + _pathFileReading + @"\" + defaultLoadingFileStreamReader.ReadLine();
                    }
                    catch (Exception)
                    {
                        throw new KnownException("File for default loading first line is wrong: " + defaultLoadingFilePath);
                    }
                }
                catch (KnownException ex)
                {
                    // This catches the second error (if this section wouldn't exist, the next catch would catch it)
                    throw ex;
                }
                catch (Exception)
                {
                    throw new KnownException("File for default loading not found: " + defaultLoadingFilePath);
                }
            }
            catch (KnownException ex)
            {
                ExceptionsCatcher.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                UnknownExceptionHandling(ex);
            }

            // Set the default opening path for the select file dialog
            ofdSelect.InitialDirectory = Application.StartupPath;
            ofdSelect.InitialDirectory += _pathFileReading;

            LoadGameFile(currentLoadingFilePath, out gameTypeStr);

            gameType = GameTypeFunction.ReturnGameTypeTypeFromString(gameTypeStr);

            ChangeGameTypeInForm(gameType);

            // We load the address of the song selected
            LoadSongDatasFromSongInComboBox(gameType);

            // Hide column like Connect, Triplets, Octave Plus
            dataGridViewsHandler.ChangeDisplayOfAdvancedColumns(false);

            this.Width = 1172;
        }

        private void RdMusicSheet_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewsHandler.ShowOneSheet(RdButtonSheetType.Music);
        }

        private void RdLoopSheet_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewsHandler.ShowOneSheet(RdButtonSheetType.Loop);
        }

        private void RdBreakSheet_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewsHandler.ShowOneSheet(RdButtonSheetType.Break);
        }
        #endregion

        #region Buttons
        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                // Set the default opening path
                ofdSelect.InitialDirectory = Application.StartupPath;
                ofdSelect.InitialDirectory += _pathFileReading;

                if (ofdSelect.ShowDialog() == DialogResult.OK)
                {
                    // Take selected path
                    _path = ofdSelect.FileName;
                }
            }
            catch (Exception ex)
            {
                UnknownExceptionHandling(ex);
            }
        }

        private void LoadSongDatasFromSongInComboBox(GameType gameType)
        {
            string hexRead = "";
            string songRAM_Address = "";
            Hex addressOfFirstByteReadForSong = new Hex();
            int readPosInt = 0;
            int i = 0;
            FileStream fs = null;
            Hex tempHex;

            try
            {
                if (cbxSongList.Enabled == true)
                {
                    if (_soundLocalisationDatas.Count > 0)
                    {
                        readPosInt = _soundLocalisationDatas[cbxSongList.SelectedIndex]._soundArrayROM_Address.GetValueAsInt();
                        // The game type affect the number of value to skip
                        if (gameType == GameType.NesA) readPosInt += (_soundLocalisationDatas[cbxSongList.SelectedIndex]._soundID * 2);
                        else readPosInt += (_soundLocalisationDatas[cbxSongList.SelectedIndex]._soundID * 3);

                        if (HexFileReader.HexFileOpener(ref fs, readPosInt, _path))
                        {
                            hexRead = HexFileReader.ReadOneHexCode(fs);
                            if (hexRead == HexFileReader._endOfFileReachedFlag)
                            {
                                throw new KnownException("Error when reading first song RAM address byte.");
                            }

                            songRAM_Address = hexRead;

                            hexRead = HexFileReader.ReadOneHexCode(fs);
                            if (hexRead == HexFileReader._endOfFileReachedFlag)
                            {
                                throw new KnownException("Error when reading second song RAM address byte.");
                            }

                            // Depending on game type, RAM address differs - and so do other parameters
                            if (gameType == GameType.NesA)
                            {
                                songRAM_Address += hexRead;

                                // According to RAM pointer, we can find the values desired
                                for (i = 0; i < _RAM_ROM_Bank_Association._List_RAM_StartBank.Count; i++)
                                {
                                    tempHex = new Hex(songRAM_Address);
                                    if (tempHex >= _RAM_ROM_Bank_Association._List_RAM_StartBank[i] && tempHex <= _RAM_ROM_Bank_Association._List_RAM_EndBank[i])
                                    {
                                        break;
                                    }
                                }

                                // Place all found values
                                txtRAM_Bank.Text = _RAM_ROM_Bank_Association._List_RAM_StartBank[i].GetValueAsString(0);
                                txtROM_Bank.Text = _RAM_ROM_Bank_Association._List_ROM_Bank[i].GetValueAsString(0);
                                
                                addressOfFirstByteReadForSong = Hex.ConvertStringHexToHex(songRAM_Address) - Hex.ConvertStringHexToHex(txtRAM_Bank.Text) + Hex.ConvertStringHexToHex(txtROM_Bank.Text);
                            }
                            else
                            {
                                songRAM_Address = hexRead + songRAM_Address;

                                // Read bank
                                hexRead = HexFileReader.ReadOneHexCode(fs);
                                if (hexRead == HexFileReader._endOfFileReachedFlag)
                                {
                                    throw new KnownException("Error when reading Bank Byte.");
                                }

                                // Place all found values
                                //      ROM Bank is one value for all songs
                                txtRAM_Bank.Text = hexRead;
                                txtROM_Bank.Text = _RAM_ROM_Bank_Association._List_ROM_Bank[0].GetValueAsString(0);

                                addressOfFirstByteReadForSong = Hex.ConvertStringHexToHex(txtROM_Bank.Text) + Hex.ConvertStringHexToHex(songRAM_Address) + (Hex.ConvertStringHexToHex(txtRAM_Bank.Text) * Hex.ConvertStringHexToHex("8000"));
                            }


                            // Place RAM Address
                            txtSongRAM_Address.Text = songRAM_Address;

                            txtFirstByteRead.Text = addressOfFirstByteReadForSong.GetValueAsString(8);
                        }
                        else
                        {
                            throw new KnownException("ROM file cannot be opened at following path: " + _path);
                        }
                    }
                    else
                    {
                        throw new KnownException("There is no song in the list right now.");
                    }
                }
                else
                {
                    throw new KnownException("There is no song in the list right now.");
                }
            }
            catch (KnownException ex)
            {
                ExceptionsCatcher.ShowError(ex.Message);
                HexFileReader.HexFileCloser(ref fs);
            }
            catch (Exception ex)
            {
                UnknownExceptionHandling(ex);
                HexFileReader.HexFileCloser(ref fs);
            }

            HexFileReader.HexFileCloser(ref fs);
        }

        private void BtnLoadAddressesForA_Song_Click(object sender, EventArgs e)
        {
            if (rdNes.Checked) LoadSongDatasFromSongInComboBox(GameType.NesA);
            else LoadSongDatasFromSongInComboBox(GameType.SnesA);
        }
        
        private void BtnLoadAnotherSongList_Click(object sender, EventArgs e)
        {
            try
            {
                GameType gameType = GameType.NesA;
                string gameTypeStr = "";

                // Set the default opening path
                ofdSelect.InitialDirectory = Application.StartupPath;
                ofdSelect.InitialDirectory += _pathFileReading;

                if (ofdSelect.ShowDialog() == DialogResult.OK)
                {
                    LoadGameFile(ofdSelect.FileName, out gameTypeStr);

                    gameType = GameTypeFunction.ReturnGameTypeTypeFromString(gameTypeStr);

                    // Update Game Type
                    ChangeGameTypeInForm(gameType);
                }
            }
            catch (KnownException ex)
            {
                ExceptionsCatcher.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                UnknownExceptionHandling(ex);
            }
        }

        private void DataValidation(string RAM_BankStr, string ROM_BankStr, string songRAM_AddressStr)
        {
            if (!Hex.IsHex(RAM_BankStr))
            {
                throw new KnownException("RAM bank is not in a correct format.");
            }
            if (!Hex.IsHex(ROM_BankStr))
            {
                throw new KnownException("ROM bank is not in a correct format.");
            }
            if (!Hex.IsHex(songRAM_AddressStr))
            {
                throw new KnownException("Song RAM address is not in a correct format.");
            }


            if ((new Hex(songRAM_AddressStr)) < (new Hex(RAM_BankStr)))
            {
                throw new KnownException("Song RAM address must be bigger than RAM bank.");
            }
        }

        private void DataValidation(string RAM_BankStr, string ROM_BankStr, string songRAM_AddressStr, string snesMemoryLoadPointerHexStr)
        {
            if (!Hex.IsHex(snesMemoryLoadPointerHexStr))
            {
                throw new KnownException("Memory Load Pointer is not in a correct format.");
            }

            DataValidation(RAM_BankStr, ROM_BankStr, songRAM_AddressStr);
        }

        private void UpdateDebugMenuChecked()
        {
            tsmKeepInstructionsOrder.Checked = keepMusicSheetInstructionsOrder;
            tsmEnsureJumpedOnInstructionsAreAloneOnTheirLine.Checked = ensureJumpedInstructionAreAloneOnTheirLine;
            tsmWriteConnectToggleInMusicSheet.Checked = writeConnectInMusicSheet;
            tmsDoNotWriteChannelEnd.Checked = doNotWriteChannelEnd;
        }
        private void DebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateDebugMenuChecked();
        }

        private void TsmKeepInstructionsOrder_Click(object sender, EventArgs e)
        {
            keepMusicSheetInstructionsOrder = !keepMusicSheetInstructionsOrder;
            UpdateDebugMenuChecked();
        }
        

        private void TsmEnsureJumpedOnInstructionsAreAloneOnTheirLine_Click(object sender, EventArgs e)
        {
            ensureJumpedInstructionAreAloneOnTheirLine = !ensureJumpedInstructionAreAloneOnTheirLine;
            UpdateDebugMenuChecked();
        }

        private void TsmWriteConnectToggleInMusicSheet_Click(object sender, EventArgs e)
        {
            writeConnectInMusicSheet = !writeConnectInMusicSheet;
            UpdateDebugMenuChecked();
        }

        private void TmsDoNotWriteChannelEnd_Click(object sender, EventArgs e)
        {
            doNotWriteChannelEnd = !doNotWriteChannelEnd;
            UpdateDebugMenuChecked();
        }

        private void BtnReadSong_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            int readPos;
            bool tripletMetInChannel = false;
            bool tripletMet = false;                // True if a triplet was met in any channel
            string temp = "";
            string tempStr;
            Hex[] channelAddress; // Channels RAM address
            string RAM_BankStr = txtRAM_Bank.Text;
            string ROM_BankStr = txtROM_Bank.Text;
            string songRAM_AddressStr = txtSongRAM_Address.Text;
            string oneChannelStr = ""; // Will contain one channel content
            Hex RAM_BankHex, ROM_BankHex, songRAM_AddressHex;
            Hex songROM_AddressHex;
            List<Hex> listRAM_AddressJumpedOn = new List<Hex>();
            List<string> oneChannelLstStr = null;
            ChannelInterpreterFromHex channel1 = null;
            ChannelInterpreterFromHex channel2 = null;
            ChannelInterpreterFromHex channel3 = null;
            ChannelInterpreterFromHex channel4 = null;
            ChannelInterpreterFromHex channel5 = null;
            ChannelInterpreterFromHex channel6 = null;
            ChannelInterpreterFromHex channel7 = null;
            ChannelInterpreterFromHex channel8 = null;

            // Exclusive for Nes
            string songFirstByte = "";

            // Exclusive for Snes
            string snesSongBGM_FileSize = "";
            string snesMemoryLoadPointer = "";

            try
            {
                if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                {
                    channelAddress = new Hex[4];
                }
                else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                {
                    channelAddress = new Hex[8];
                }
                else
                {
                    channelAddress = new Hex[4];
                }
                
                // Function can make a throw
                DataValidation(RAM_BankStr, ROM_BankStr, songRAM_AddressStr);
                
                // Here if data was validated
                RAM_BankHex = new Hex(RAM_BankStr);
                ROM_BankHex = new Hex(ROM_BankStr);
                songRAM_AddressHex = new Hex(songRAM_AddressStr);

                // Calculate song ROM address
                songROM_AddressHex = ChannelReader.CalculateSongROM_Address(dataGridViewsHandler.GetGameType(), RAM_BankHex, ROM_BankHex, songRAM_AddressHex);

                readPos = songROM_AddressHex.GetValueAsInt();
                if (HexFileReader.HexFileOpener(ref fs, readPos, _path))
                {
                    // If first byte already out of bound, throw an exception
                    if ((temp = HexFileReader.ReadOneHexCode(fs)) == HexFileReader._endOfFileReachedFlag)
                    {
                        throw new KnownException("End of file met while reading channels addresses.");
                    }

                    // If a nes game
                    if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                    {
                        songFirstByte = temp;
                    }
                    // If a snes game
                    else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                    {
                        // Build BGM File Size
                        snesSongBGM_FileSize = temp;
                        
                        temp = HexFileReader.ReadOneHexCode(fs);
                        if (temp == HexFileReader._endOfFileReachedFlag)
                        {
                            throw new KnownException("End of file met while reading BGM File Size.");
                        }

                        snesSongBGM_FileSize = string.Concat(snesSongBGM_FileSize, temp);
                        
                        // Read Memory Load Pointer
                        for (int i = 0; i < 2; i++)
                        {
                            temp = HexFileReader.ReadOneHexCode(fs);
                            if (temp == HexFileReader._endOfFileReachedFlag)
                            {
                                throw new KnownException("End of file met while reading Memory Load Pointer.");
                            }

                            // Snes Memory Load pointer defines, on the bytes after it, what is the RAM Address.
                            // For example, if it's B0 0D in the file, reverse it, 0DB0, the the next byte after it has RAM
                            // address 0DB0.
                            // So let's say first channel pointer is 0D C0, will need to substract 0D B0 from it, meaning
                            // channel starts 10 bytes farther.
                            snesMemoryLoadPointer = string.Concat(temp, snesMemoryLoadPointer);
                            
                            txtSnesMemoryLoadPointer.Text = snesMemoryLoadPointer;
                        }
                    }

                    // Read all address
                    for (int i = 0; i < _channelQty; i++)
                    {
                        tempStr = "";

                        for (int j = 0; j < _hexCodeQtyByAddress; j++)
                        {
                            temp = HexFileReader.ReadOneHexCode(fs);
                            if (temp == HexFileReader._endOfFileReachedFlag)
                            {
                                throw new KnownException("End of file met while reading channels addresses.");
                            }

                            tempStr = string.Concat(tempStr, temp);
                        }

                        channelAddress[i] = new Hex(tempStr);
                    }

                    for (int i = 0; i < _channelQty; i++)
                    {
                        HexFileReader.HexFileCloser(ref fs);
                        
                        // Address to start reading is different between Nes and Snes
                        if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                        {
                            HexFileReader.HexFileOpener(ref fs, Misc.ReturnAbsoluteROM_PositionFromRAM_Int(RAM_BankHex, ROM_BankHex, channelAddress[i]), _path);
                        }
                        else // Snes
                        {
                            // Need to add 4 to skip the 4 first byte before the song. Pointing starts after
                            Hex tempss = songROM_AddressHex + Hex.ConvertStringHexToHex("4") + channelAddress[i] - Hex.ConvertStringHexToHex(snesMemoryLoadPointer);
                            HexFileReader.HexFileOpener(ref fs, tempss.GetValueAsInt(), _path);
                        }


                        ChannelReader.ReadOneChannel(
                            fs,
                            dataGridViewsHandler.GetGameType(),
                            out oneChannelStr,
                            out oneChannelLstStr,
                            out listRAM_AddressJumpedOn,
                            out tripletMetInChannel
                                            );

                        tripletMet = tripletMet || tripletMetInChannel;

                        switch (i)
                        {
                            case 0:
                                txtChannel1.Text = oneChannelStr;
                                channel1 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                break;
                            case 1:
                                txtChannel2.Text = oneChannelStr;
                                channel2 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                break;
                            case 2:
                                txtChannel3.Text = oneChannelStr;
                                channel3 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                break;
                            case 3:
                                txtChannel4.Text = oneChannelStr;
                                if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                                {
                                    channel4 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, true, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                }
                                else // don't consider it's a noise channel
                                {
                                    channel4 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                }
                                break;
                            case 4:
                                channel5 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                break;
                            case 5:
                                channel6 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                break;
                            case 6:
                                channel7 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                break;
                            case 7:
                                channel8 = new ChannelInterpreterFromHex(oneChannelLstStr, channelAddress[i], i, listRAM_AddressJumpedOn, false, dataGridViewsHandler.GetGameType(), keepMusicSheetInstructionsOrder, writeConnectInMusicSheet, ensureJumpedInstructionAreAloneOnTheirLine);
                                break;
                            default:
                                break;
                        }
                    }

                    HexFileReader.HexFileCloser(ref fs);

                    // Write in every DataGridView
                    dataGridViewsHandler.WriteChannelInDataGridView(0, channel1);
                    dataGridViewsHandler.WriteChannelInDataGridView(1, channel2);
                    dataGridViewsHandler.WriteChannelInDataGridView(2, channel3);
                    
                    // Some channels are different depending on game type
                    if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                    {
                        dataGridViewsHandler.WriteChannelInDataGridView(3, channel4);
                    }
                    else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                    {
                        dataGridViewsHandler.WriteChannelInDataGridView(3, channel4);
                        dataGridViewsHandler.WriteChannelInDataGridView(4, channel5);
                        dataGridViewsHandler.WriteChannelInDataGridView(5, channel6);
                        dataGridViewsHandler.WriteChannelInDataGridView(6, channel7);
                        dataGridViewsHandler.WriteChannelInDataGridView(7, channel8);
                    }

                    lblStatus.Text = "Song read, operation number " + counter;
                    counter += 1;
                }
                else
                {
                    throw new KnownException("File cannot be opened, path is invalid? Reselect file using the 'Select File' button.");
                }
            }
            catch (KnownException ex)
            {
                ExceptionsCatcher.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                UnknownExceptionHandling(ex);
            }

            // Ensure file is closed
            if (fs != null)
            {
                fs.Dispose();
                fs.Close();
            }
        }

        private void BtnCreateSong_Click(object sender, EventArgs e)
        {
            try
            {
                string RAM_BankStr = txtRAM_Bank.Text;
                string ROM_BankStr = txtROM_Bank.Text;
                string songRAM_AddressStr = txtSongRAM_Address.Text;
                string snesMemoryLoadPointerStr = txtSnesMemoryLoadPointer.Text;
                string sizeData = "";
                Hex RAM_BankHex, ROM_BankHex, songRAM_AddressHex, snesMemoryLoadPointerHex;
                Hex songROM_AddressHex;
                List<List<MusicLine>> _channelLinesInMemory = null;

                // This is a temporary way to add first byte for mega man on nes
                if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                {
                    // Function can make a throw
                    DataValidation(RAM_BankStr, ROM_BankStr, songRAM_AddressStr);
                }
                else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                {
                    // Function can make a throw
                    DataValidation(RAM_BankStr, ROM_BankStr, songRAM_AddressStr, snesMemoryLoadPointerStr);
                }

                // Here if data was validated
                RAM_BankHex = new Hex(RAM_BankStr);
                ROM_BankHex = new Hex(ROM_BankStr);
                songRAM_AddressHex = new Hex(songRAM_AddressStr);
                snesMemoryLoadPointerHex = new Hex(snesMemoryLoadPointerStr);

                // Read in every DataGridView
                dataGridViewsHandler.ReadChannelInDataGridView(out _channelLinesInMemory, dataGridViewsHandler.GetGameType());
                
                ChannelInstructionsToHex songStringToWrite = new ChannelInstructionsToHex(_channelLinesInMemory, songRAM_AddressHex, dataGridViewsHandler.GetGameType(), doNotWriteChannelEnd, snesMemoryLoadPointerHex);

                List<string> dataLayersToWrite = songStringToWrite.Get_soundDatasToWrite();

                string toWrite = "";

                // This is a temporary way to add first byte for mega man on nes
                if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                {
                    toWrite = "00";
                }
                else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                {
                    toWrite = "AAAAAAAA"; // Place holder value until it is changed later
                }
                
                foreach (string temp in dataLayersToWrite)
                {
                    toWrite += temp;
                }

                // Those are bytes to write for snes
                if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                {
                    // We need to remove since obviously 2 bytes are the start of song are no song data and remove 1 because 0 counts for 1
                    sizeData = (Hex.ConvertIntToHex(((toWrite.Length) / 2) - 2 - 1)).GetValueAsString(4);

                    // We write the size of the song where it goes - and the Snes Memory Pointer
                    toWrite = sizeData.Substring(2, 2) + sizeData.Substring(0, 2) + snesMemoryLoadPointerStr.Substring(2, 2) + snesMemoryLoadPointerStr.Substring(0, 2) + toWrite.Substring(8);
                }
                

                FileStream fs = null;
                int writePos;

                // Calculate song ROM address
                songROM_AddressHex = ChannelReader.CalculateSongROM_Address(dataGridViewsHandler.GetGameType(), RAM_BankHex, ROM_BankHex, songRAM_AddressHex);
                
                List<Hex> toWriteListHex;

                toWriteListHex = Hex.ConvertStringHexToListOf2DigitsHexs(toWrite);

                writePos = songROM_AddressHex.GetValueAsInt();
                if (HexFileWriter.HexFileOpener(ref fs, writePos, _path))
                {
                    HexFileWriter.WriteListHexCodes(fs, toWriteListHex);
                }


                HexFileWriter.HexFileCloser(ref fs);

                lblStatus.Text = "Song created, operation number " + counter;
                counter += 1;
            }
            catch (KnownException ex)
            {
                ExceptionsCatcher.ShowError(ex.Message);
            }
        }
        #endregion

        private void ReadSongToTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            int readPos, qtyOfByteToSkip, currentChannelQtyHexCodes, songSize;
            string temp = "";
            string tempStr;
            Hex[] channelAddress; // Channels RAM address
            string RAM_BankStr = txtRAM_Bank.Text;
            string ROM_BankStr = txtROM_Bank.Text;
            string songRAM_AddressStr = txtSongRAM_Address.Text;
            string snesMemoryLoadPointerStr = txtSnesMemoryLoadPointer.Text;
            Hex RAM_BankHex, ROM_BankHex, songRAM_AddressHex, snesMemoryLoadPointerHex;
            Hex songROM_AddressHex;
            List<Hex> listRAM_AddressJumpedOn = new List<Hex>();
            List<string> oneChannelLstStr = null;
            List<string> musicLst = new List<string>();
            string songName = txtSongName.Text;

            try
            {
                // Need to initialise value of some variables sothey don't cause an error
                qtyOfByteToSkip = 0;
                songSize = 0;
                tempStr = "";
                channelAddress = null;

                if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                {
                    channelAddress = new Hex[4];

                    // Function can make a throw
                    DataValidation(RAM_BankStr, ROM_BankStr, songRAM_AddressStr);
                }
                else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                {
                    channelAddress = new Hex[8];

                    // Function can make a throw
                    DataValidation(RAM_BankStr, ROM_BankStr, songRAM_AddressStr);
                }

                // Here if data was validated
                RAM_BankHex = new Hex(RAM_BankStr);
                ROM_BankHex = new Hex(ROM_BankStr);
                songRAM_AddressHex = new Hex(songRAM_AddressStr);
                snesMemoryLoadPointerHex = new Hex(snesMemoryLoadPointerStr);

                // Calculate song ROM address
                songROM_AddressHex = ChannelReader.CalculateSongROM_Address(dataGridViewsHandler.GetGameType(), RAM_BankHex, ROM_BankHex, songRAM_AddressHex);

                readPos = songROM_AddressHex.GetValueAsInt();
                if (HexFileReader.HexFileOpener(ref fs, readPos, _path))
                {
                    if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                    {
                        qtyOfByteToSkip = 1;
                        songSize = 8 - 1; // 8 bytes for channel addresses (but 0 count for 1)
                    }
                    else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                    {
                        qtyOfByteToSkip = 4;
                        songSize = 18 - 1; // 16 bytes for channel addresses + 2 byte for Snes Memory Load Pointer (but 0 count for 1)
                    }

                    // Skip qty of bytes that need to be skipped
                    for (int i = 0; i < qtyOfByteToSkip; i++)
                    {
                        // First bytes in the song to skip
                        tempStr = HexFileReader.ReadOneHexCode(fs);

                        // Skips the useless byte at the start of the channel address location (not needed to read music)
                        if (tempStr == HexFileReader._endOfFileReachedFlag)
                        {
                            throw new KnownException("End of file met while reading channels addresses.");
                        }
                    }


                    musicLst.Add("MUSIC__" + songName + ":");

                    // Add the bytes depending on the game type
                    if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                    {
                        musicLst.Add("HEX " + tempStr);
                    }
                    else if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                    {
                        musicLst.Add("DL " + songName + "__DATA_SIZE");
                        musicLst.Add("DH " + songName + "__DATA_SIZE");
                        musicLst.Add("DL " + songName + "__SNES_MEMORY_LOAD_POINTER");
                        musicLst.Add("DH " + songName + "__SNES_MEMORY_LOAD_POINTER");
                        musicLst.Add(songName + "__SNES_MEMORY_LOAD_POINTER:");
                    }

                    // Read all address
                    for (int i = 0; i < _channelQty; i++)
                    {
                        tempStr = "";

                        for (int j = 0; j < _hexCodeQtyByAddress; j++)
                        {
                            temp = HexFileReader.ReadOneHexCode(fs);
                            if (temp == HexFileReader._endOfFileReachedFlag)
                            {
                                throw new KnownException("End of file met while reading channels addresses.");
                            }

                            tempStr = string.Concat(tempStr, temp);
                        }
                        channelAddress[i] = new Hex(tempStr);

                        // Add channels pointers

                        musicLst.Add("DH " + songName + "__CHANNEL_" + (i + 1));
                        musicLst.Add("DL " + songName + "__CHANNEL_" + (i + 1));
                    }

                    for (int i = 0; i < _channelQty; i++)
                    {
                        ChannelReader.ReadOneChannelReturnOneInstructionPerLineAndListAddressesJumpedOn(
                            channelAddress[i],
                            fs,
                            songName,
                            i,
                            out oneChannelLstStr,
                            out currentChannelQtyHexCodes
                                            );

                        songSize += currentChannelQtyHexCodes;

                        for (int j = 0; j < oneChannelLstStr.Count; j++)
                        {
                            musicLst.Add(oneChannelLstStr[j]);
                        }
                    }

                    HexFileReader.HexFileCloser(ref fs);

                    // Define what is the data size
                    // We need to remove since obviously 2 bytes are the start of song are no song data and remove 1 because 0 counts for 1
                    if (dataGridViewsHandler.GetGameType() == GameType.SnesA)
                    {
                        musicLst.Add(songName + "__DATA_SIZE = $" + Hex.ConvertIntToHex(songSize).GetValueAsString(4));
                    }

                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter("MUSIC__" + songName + ".asm"))
                    {
                        for (int i = 0; i < musicLst.Count; i++)
                        {
                            file.WriteLine(musicLst[i]);
                        }
                    }
                }
                else
                {
                    throw new KnownException("File cannot be opened, path is invalid? Reselect file using the 'Select File' button.");
                }
            }
            catch (KnownException ex)
            {
                ExceptionsCatcher.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                UnknownExceptionHandling(ex);
            }

            // Ensure file is closed
            if (fs != null)
            {
                fs.Dispose();
                fs.Close();
            }
        }
        
        private void ShowAdvancedColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAdvancedColumnsToolStripMenuItem.Checked = !showAdvancedColumnsToolStripMenuItem.Checked;
            dataGridViewsHandler.ChangeDisplayOfAdvancedColumns(showAdvancedColumnsToolStripMenuItem.Checked);
        }

        private void UsePianoKeysForNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            usePianoKeysForNotesToolStripMenuItem.Checked = !usePianoKeysForNotesToolStripMenuItem.Checked;
            usePianoKeysForNotes = !usePianoKeysForNotes;

            dataGridViewsHandler.SetUsePianoKeysForNotes(usePianoKeysForNotes);
        }

        private void RdNes_CheckedChanged(object sender, EventArgs e)
        {
            if (rdNes.Checked == true && dataGridViewsHandler.GetGameType() != GameType.NesA)
            {
                DialogResult msgButton = MessageBox.Show("Content of Triangle and Noise channel will need to be checked by user to ensure everything is valid for Nes." + Environment.NewLine + "Proceed?", "Switch to Nes Music Engine", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (msgButton == DialogResult.Yes)
                {
                    lblFirstPointer.Text = "RAM Bank";

                    // Create the groupbox to pick a channel
                    dataGridViewsHandler.ChangeGameType(GameType.NesA, ref gbxSheetSelection, ref gbxConsoleSelection);
                    lblSnesMemoryLoadPointer.Visible = false;
                    txtSnesMemoryLoadPointer.Visible = false;

                    _channelQty = 4;
                }
                else
                {
                    rdSnes.Checked = true;
                }
                    
            }
        }

        private void RdSnes_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSnes.Checked == true && dataGridViewsHandler.GetGameType() != GameType.SnesA)
            {
                    // If was a nes song, convert every line of the noise grid to normal music notes
                if (dataGridViewsHandler.GetGameType() == GameType.NesA)
                {
                    lblFirstPointer.Text = "Bank ID from Start Point";

                    dataGridViewsHandler.ConvertNoiseChannelToNormalChannel();
                }

                // Create the groupbox to pick a channel
                dataGridViewsHandler.ChangeGameType(GameType.SnesA, ref gbxSheetSelection, ref gbxConsoleSelection);
                lblSnesMemoryLoadPointer.Visible = true;
                txtSnesMemoryLoadPointer.Visible = true;

                _channelQty = 8;
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            // On form initialisation, dataGridView is not yet created, so end code
            if (dataGridViewsHandler == null) return;
            dataGridViewsHandler.ChangeDataGridViewSizes(this.Width, this.Height);

            if (stupidBool)
            {
                stupidBool = false;
                dataGridViewsHandler.RecuperateScrollBarsPositions();
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new FrmAbout();

            frmAbout.ShowDialog();
        }
    }


    public class RAM_ROM_Bank_Association
    {
        public List<Hex> _List_ROM_Bank;
        public List<Hex> _List_RAM_StartBank;
        public List<Hex> _List_RAM_EndBank;


        public RAM_ROM_Bank_Association()
        {
            _List_ROM_Bank = new List<Hex>();
            _List_RAM_StartBank = new List<Hex>();
            _List_RAM_EndBank = new List<Hex>();
        }

        public void AddDatas(Hex ROM_Bank, Hex RAM_StartBank, Hex RAM_EndBank)
        {
            _List_ROM_Bank.Add(ROM_Bank);
            _List_RAM_StartBank.Add(RAM_StartBank);
            _List_RAM_EndBank.Add(RAM_EndBank);
        }
    }
}
