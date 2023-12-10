using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mega_Music_Editor.Reusable;
using Mega_Music_Editor.Unique.MusicEngineFixedNotesDatas;

namespace Mega_Music_Editor.Unique
{
    public class ConnectState
    {
        // There are 2 bits controlling the connect. Hex 40 and Hex 80 (from Flag command)
        // Bit 40   Bit 80
        // 0        0       Nothing
        // 1        0       Looking for first note, once met, Bit 40 turns to 0 and Bit 80 turns to 1
        // 0        1       On (every next note is part of the connect)
        // 1        1       Looking for last note, once met, Bit 40 turns to 0 and Bit 80 turn to 0
        public enum ConnectStateEnum
        {
            Inactive = 0,
            LookForFirstNote = 1,
            On = 2,
            LookForLastNote = 3
        }

        private ConnectStateEnum _ConnectState;
        private List<int> connectLine = new List<int>(); // Used to know the two lines with the 01 command in case we need to delete it
        private List<int> noteConnectLine = new List<int>(); // Used to know the lines with the notes in case they need t to be deleted
        private Hex initialNote = new Hex();
        
        public ConnectState()
        {
            _ConnectState = ConnectStateEnum.Inactive;
        }
        private bool ConnectCommandNeedToBeOnLine()
        {
            if (_ConnectState == ConnectStateEnum.LookForFirstNote)
            {
                return true;
            }
            else if (_ConnectState == ConnectStateEnum.LookForLastNote)
            {
                return true;
            }
            return false;
        }
        private void CheckIfNoteMetIsSameAsFirstNoteMetInConnect(string currentNote, int line)
        {
            if ((initialNote & new Hex("1F")) == (new Hex(currentNote) & new Hex("1F")))
            {
                noteConnectLine.Add(line);
            }
            else
            {
            }
        }
        public bool IsConnectInactive() { return ConnectStateEnum.Inactive == _ConnectState; }
        public bool IsConnectLookingForFirstNote() { return ConnectStateEnum.LookForFirstNote == _ConnectState; }
        public bool IsConnectOn() { return ConnectStateEnum.On == _ConnectState; }
        public bool IsConnectLookingForLastNote() { return ConnectStateEnum.LookForLastNote == _ConnectState; }
        public void FirstNoteMet() { _ConnectState = ConnectStateEnum.On; }
        public void LastNoteMet()
        {
            _ConnectState = ConnectStateEnum.Inactive;
        }
        public void NoteMet(string noteHexCode, int currentLine)
        {
            // Update connect state
            if (IsConnectLookingForFirstNote())
            {
                FirstNoteMet();
                initialNote = new Hex(noteHexCode);
            }
            else if (IsConnectLookingForLastNote())
            {
                LastNoteMet();
                CheckIfNoteMetIsSameAsFirstNoteMetInConnect(noteHexCode, currentLine);       
            }
            else
            {
                CheckIfNoteMetIsSameAsFirstNoteMetInConnect(noteHexCode, currentLine);
            }
        }
        /// <summary>
        /// Determine what is connect state now that a connect command was met
        /// </summary>
        /// <returns>True if a connect command needs to be on the line</returns>
        public bool ConnectCommandMet(int currentLine)
        {
            if (_ConnectState == ConnectStateEnum.Inactive)
            {
                _ConnectState = ConnectStateEnum.LookForFirstNote;
                connectLine = new List<int>();  // We only reset it there because unless we come here anyway it cannot be used
                noteConnectLine = new List<int>(); // We only reset it there because unless we come here anyway it cannot be used

                connectLine.Add(currentLine);
            }
            else if (_ConnectState == ConnectStateEnum.LookForFirstNote)
            {
                _ConnectState = ConnectStateEnum.Inactive;
                connectLine.Add(currentLine);
            }
            else if (_ConnectState == ConnectStateEnum.On) _ConnectState = ConnectStateEnum.LookForLastNote;
            else if (_ConnectState == ConnectStateEnum.LookForLastNote) _ConnectState = ConnectStateEnum.On;
            
            return ConnectCommandNeedToBeOnLine();
        }
        /// <summary>
        /// Determine what is connect state now that a flag command was met
        /// </summary>
        /// <param name="flag"></param>
        /// <returns>True if a connect command needs to be on the line</returns>
        public bool FlagCommandMet(Hex flag)
        {
            string flagConnectBits = (new Hex("C0") & flag).GetValueAsString(2);

            if (flagConnectBits == "00")_ConnectState = ConnectStateEnum.Inactive;
            if (flagConnectBits == "40") _ConnectState = ConnectStateEnum.LookForFirstNote;
            else if (flagConnectBits == "80")
            {
                _ConnectState = ConnectStateEnum.LookForLastNote;
                throw new KnownException("A flag with 80 on");
            }
            else if (flagConnectBits == "C0")
            {
                _ConnectState = ConnectStateEnum.On;
                throw new KnownException("A flag with C0 on");
            }

            return ConnectCommandNeedToBeOnLine();
        }
    }


    public class ChannelInterpreterFromHex
    {
        // Contains all the line of the song, 64th note takes 3 lines because of the possibility of Triplets
        private readonly List<MusicLine> _channelLines = null;
        private readonly List<MusicLinePreFiltered> _channelLinesPreFiltered = null;
        private readonly List<int> _instructionLine = null;
        private readonly Hex _firstInstructionRAM_Hex = null;

        // Get methods
        public List<MusicLine> GetChannelLines() { return _channelLines; }

        /// <summary>
        /// Add octage to note
        /// </summary>
        /// <param name="octave"></param>
        /// <param name="note"></param>
        private void AddOctaveToNote(int octave, ref string note)
        {
            string temp = "";
            int currentOct = 0;

            if (note != MusicEngineFixedNotesDatas.Names.NesA.Delay)
            {
                temp = note.Substring(0, 2);
                currentOct = Convert.ToInt32(note.Substring(2, 1));

                currentOct += octave;

                note = temp + currentOct.ToString();
            }
        }

        /// <summary>
        /// Return lenght in lines (64th note is 1 line)
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private int ReturnNoteLineQuantityByNoteHex(string hex)
        {
            string noteLenght = (new Hex(hex) & new Hex("E0")).GetValueAsString(0);

            // Return number of lines according to hex code
            return Lenghts.NesA.GetNoteLineQtyByHexCode(noteLenght);
        }

        /// <summary>
        /// Every time an instruction is added, indicate its Line and RAM position
        /// </summary>
        private void SetLocationForNewInstruction(int currentLine)
        {
            _instructionLine.Add(currentLine);
        }

        private void ConditionalFakeLineMaker(ref int currentLine)
        {
            _channelLinesPreFiltered[currentLine].SetNote(MusicEngineFixedNotesDatas.Names.NesA.Skip);
            _channelLinesPreFiltered.Add(new MusicLinePreFiltered());
            currentLine++;
        }

        private void ForceFakeLineMaker(ref int currentLine)
        {
            _channelLinesPreFiltered[currentLine].SetNote(MusicEngineFixedNotesDatas.Names.NesA.Skip);
            _channelLinesPreFiltered.Add(new MusicLinePreFiltered());
            currentLine++;
        }

        public ChannelInterpreterFromHex(List<string> channelDatas, Hex firstInstructionRAM_Hex, int currentChannelID, List<Hex> listRAM_AddressJumpedOn, bool isNoiseChannel, GameType gameType, bool keepMusicSheetInstructionsOrder, bool writeConnectInMusicSheet, bool ensureJumpedInstructionAreAloneOnLines = true)
        {
            ConnectState connectState = new ConnectState();
            bool plusHalf = false;
            int currentLine = 0;
            int currentOctave = 0;
            int noteLenght = 0;
            int tempInt = 0;
            int lastInstructionPriority = 0;
            string tempStr = "";
            string currentHexCode = "";
            Hex tempHex1;
            FixedMusicEngineNoteStruct noteDatas;
            bool instructionIsJumpedOn = false;

            _instructionLine = new List<int>();
            _channelLinesPreFiltered = new List<MusicLinePreFiltered> { new MusicLinePreFiltered() };
            _firstInstructionRAM_Hex = firstInstructionRAM_Hex;

            int i = 0;

            #region Declaration of functions used within current zone
            void setInstructionTriplet(string hexCode)
            {
                if (keepMusicSheetInstructionsOrder)
                {
                    if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                    {
                        lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetTriplet();
                    }
                    else // There are some instructions on current line that would be played after current instruction
                         // if it would be set on same line
                    {
                        // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                        // that were on line we were analysing
                        _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                        if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                        {
                            ForceFakeLineMaker(ref currentLine);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetTriplet();

                        ForceFakeLineMaker(ref currentLine);

                        lastInstructionPriority = 0;    // No instruction yet
                    }
                }
                else
                {
                    // Keep the highest instruction since it's the one we always need to compare too
                    if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                    {
                        lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                    }

                    SetLocationForNewInstruction(currentLine);
                    _channelLinesPreFiltered[currentLine].SetTriplet();
                }
            }
            void setInstructionConnect(string hexCode)
            {
                if (keepMusicSheetInstructionsOrder)
                {
                    if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                    {
                        lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetConnect(writeConnectInMusicSheet);
                    }
                    else // There are some instructions on current line that would be played after current instruction
                         // if it would be set on same line
                    {
                        // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                        // that were on line we were analysing
                        _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                        if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                        {
                            ForceFakeLineMaker(ref currentLine);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetConnect(writeConnectInMusicSheet);

                        ForceFakeLineMaker(ref currentLine);

                        lastInstructionPriority = 0;    // No instruction yet
                    }
                }
                else
                {
                    // Keep the highest instruction since it's the one we always need to compare too
                    if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                    {
                        lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                    }

                    SetLocationForNewInstruction(currentLine);
                    _channelLinesPreFiltered[currentLine].SetConnect(writeConnectInMusicSheet);
                }
            }
            void setInstructionOctavePlus(string hexCode)
            {
                if (keepMusicSheetInstructionsOrder)
                {
                    if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                    {
                        lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetOctavePlus();
                    }
                    else // There are some instructions on current line that would be played after current instruction
                         // if it would be set on same line
                    {
                        // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                        // that were on line we were analysing
                        _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                        if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                        {
                            ForceFakeLineMaker(ref currentLine);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetOctavePlus();

                        ForceFakeLineMaker(ref currentLine);

                        lastInstructionPriority = 0;    // No instruction yet
                    }
                }
                else
                {
                    // Keep the highest instruction since it's the one we always need to compare too
                    if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                    {
                        lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                    }

                    SetLocationForNewInstruction(currentLine);
                    _channelLinesPreFiltered[currentLine].SetOctavePlus();
                }
            }

            void setInstructionToneLenght(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetToneLenght(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetToneLenght(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetToneLenght(channelDatas[i]);
                    }
                }
            }

            void setInstructionVolume(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetVolume(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetVolume(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetVolume(channelDatas[i]);
                    }
                }
            }

            void setInstructionInstrument(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetInstrument(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetInstrument(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetInstrument(channelDatas[i]);
                    }
                }
            }

            void setInstructionOctave(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetOctave(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetOctave(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetOctave(channelDatas[i]);
                    }
                }
            }

            void setInstructionGlobalTranspose(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetGlobalTranspose(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetGlobalTranspose(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetGlobalTranspose(channelDatas[i]);
                    }
                }
            }

            void setInstructionTranspose(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetTranspose(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetTranspose(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetTranspose(channelDatas[i]);
                    }
                }
            }

            void setInstructionTunePitch(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetTunePitch(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetTunePitch(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetTunePitch(channelDatas[i]);
                    }
                }
            }

            void setInstructionPitchSlide(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetPitchSlide(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetPitchSlide(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetPitchSlide(channelDatas[i]);
                    }
                }
            }

            void setInstructionToneType(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetToneType(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetToneType(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetToneType(channelDatas[i]);
                    }
                }
            }
            
            void setInstructionSnesPanning(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetPanning(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetPanning(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetPanning(channelDatas[i]);
                    }
                }
            }
            
            void setInstructionGlobalVolume(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetGlobalVolume(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetGlobalVolume(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetGlobalVolume(channelDatas[i]);
                    }
                }
            }
            
            void setInstruction1A()
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to second byte of 1A (second byte defines instruction)
                    i++;
                    
                    if (channelDatas[i] == "00") { setInstructionVibratoDepth("1A00"); } // Snes Command
                    else if (channelDatas[i] == "01") { setInstructionTremoloVolume("1A01"); } // Snes Command
                    else if (channelDatas[i] == "02") { setInstructionVibratoTremoloSpeed("1A02"); } // Snes Command
                    else if (channelDatas[i] == "03") { setInstruction1A03("1A03"); } // Snes Command
                    else if (channelDatas[i] == "04") { setInstructionPitchSlideUnk(); } // Snes Command
                    else System.Windows.Forms.MessageBox.Show("Unmanaged instruction met:" + Environment.NewLine + "1A" + channelDatas[i] + Environment.NewLine + "Warn programmer");
                }
            }
            
            void setInstructionVibratoDepth(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    // 1A command has 2 parameters, so need to set line 2 times
                    SetLocationForNewInstruction(currentLine);
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetVibratoDepth(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetVibratoDepth(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetVibratoDepth(channelDatas[i]);
                    }
                }
            }

            void setInstructionTremoloVolume(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    // 1A command has 2 parameters, so need to set line 2 times
                    SetLocationForNewInstruction(currentLine);
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetTremoloVolume(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetTremoloVolume(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetTremoloVolume(channelDatas[i]);
                    }
                }
            }

            void setInstructionVibratoTremoloSpeed(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    // 1A command has 2 parameters, so need to set line 2 times
                    SetLocationForNewInstruction(currentLine);
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetVibratoOrTremolFrequency(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetVibratoOrTremolFrequency(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetVibratoOrTremolFrequency(channelDatas[i]);
                    }
                }
            }

            void setInstruction1A03(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    // 1A command has 2 parameters, so need to set line 2 times
                    SetLocationForNewInstruction(currentLine);
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].Set1A03(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].Set1A03(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].Set1A03(channelDatas[i]);
                    }
                }
            }
            
            void setInstructionPitchSlideUnk()
            {
                throw new KnownException("setInstructionPitchSlideUnk was called!! warn programmer, instruction 1A04XX exists!!");
                // Ensure not to go out of bound
                //if ((i + 1) < channelDatas.Count())
                //{
                //    // Go to parameter
                //    i++;
                //    // 1A command has 2 parameters, so need to set line 2 times
                //    setLocationForNewInstruction(currentLine);
                //    setLocationForNewInstruction(currentLine);

                //    if (keepMusicSheetInstructionsOrder)
                //    {
                //        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                //        {
                //            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                //            // Set instruction
                //            setLocationForNewInstruction(currentLine);
                //            _channelLinesPreFiltered[currentLine].SetPitchSlideUnk(channelDatas[i]);
                //        }
                //        else // There are some instructions on current line that would be played after current instruction
                //             // if it would be set on same line
                //        {
                //            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                //            // that were on line we were analysing
                //            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                //            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                //            {
                //                forceFakeLineMaker(ref currentLine);
                //            }

                //            // Set instruction
                //            setLocationForNewInstruction(currentLine);
                //            _channelLinesPreFiltered[currentLine].SetPitchSlideUnk(channelDatas[i]);

                //            forceFakeLineMaker(ref currentLine);

                //            lastInstructionPriority = 0;    // No instruction yet
                //        }
                //    }
                //    else
                //    {
                //        // Keep the highest instruction since it's the one we always need to compare too
                //        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                //        {
                //            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                //        }

                //        // Set instruction
                //        setLocationForNewInstruction(currentLine);
                //        _channelLinesPreFiltered[currentLine].SetPitchSlideUnk(channelDatas[i]);
                //    }
                //}
            }

            void setInstructionSfxToggle(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 2) < channelDatas.Count())
                {
                    // Go to parameter
                    i += 2;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetSfxToggle(channelDatas[i - 1], channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetSfxToggle(channelDatas[i - 1], channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetSfxToggle(channelDatas[i - 1], channelDatas[i]);
                    }
                }
            }

            void setInstructionSfx(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetSfx(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetSfx(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetSfx(channelDatas[i]);
                    }
                }
            }
            
            void setInstructionFullNoteLenght(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetToggleFullNoteLenght(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetToggleFullNoteLenght(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetToggleFullNoteLenght(channelDatas[i]);
                    }
                }
            }
            
            void setInstructionPanningMovementSpeed(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetPanningMovementSpeed(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetPanningMovementSpeed(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetPanningMovementSpeed(channelDatas[i]);
                    }
                }
            }
            
            void setInstructionUnknown1F(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetUnknown1F(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetUnknown1F(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetUnknown1F(channelDatas[i]);
                    }
                }
            }

            void setInstructionFlag(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 1) < channelDatas.Count())
                {
                    // Go to parameter
                    i++;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetFlags(channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetFlags(channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetFlags(channelDatas[i]);
                    }
                    
                    tempHex1 = new Hex(channelDatas[i]);
                    connectState.FlagCommandMet(tempHex1);
                }
            }

            void setInstructionSpeed(string hexCode)
            {
                // Ensure not to go out of bound
                if ((i + 2) < channelDatas.Count())
                {
                    // Go to parameter
                    i += 2;
                    SetLocationForNewInstruction(currentLine);

                    if (keepMusicSheetInstructionsOrder)
                    {
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetSpeed(channelDatas[i - 1], channelDatas[i]);
                        }
                        else // There are some instructions on current line that would be played after current instruction
                             // if it would be set on same line
                        {
                            // We skip a line, so we ensure that current instruction is gonna be executed after instructions
                            // that were on line we were analysing
                            _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                            if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                            {
                                ForceFakeLineMaker(ref currentLine);
                            }

                            // Set instruction
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            _channelLinesPreFiltered[currentLine].SetSpeed(channelDatas[i - 1], channelDatas[i]);

                            ForceFakeLineMaker(ref currentLine);

                            lastInstructionPriority = 0;    // No instruction yet
                        }
                    }
                    else
                    {
                        // Keep the highest instruction since it's the one we always need to compare too
                        if (GetMusicEngineInstructionsDatas.CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(hexCode, lastInstructionPriority))
                        {
                            lastInstructionPriority = GetMusicEngineInstructionsDatas.GetPriorityByHexCode(hexCode);
                        }

                        // Set instruction
                        SetLocationForNewInstruction(currentLine);
                        SetLocationForNewInstruction(currentLine);
                        _channelLinesPreFiltered[currentLine].SetSpeed(channelDatas[i - 1], channelDatas[i]);
                    }
                }
            }
            #endregion

            try
            {
                // Loop through all hex codes
                for (i = 0; i < channelDatas.Count(); i++)
                {
                    // We need to be sure instruction is alone (a jumped on instruction) on the line so the jump is on the instruction directly
                    // instructionIsJumpedOn is true if the instruction that needs to be alone was set on the line and...
                    if (instructionIsJumpedOn)
                    {
                        // ... now it's time to skip a line to be sure instruction is alone
                        instructionIsJumpedOn = false;

                        ConditionalFakeLineMaker(ref currentLine);

                        lastInstructionPriority = 0;    // No instruction yet
                    }

                    currentHexCode = channelDatas[i];
                    
                    // Here we check if the instruction we are handling now is one that is jumped on
                    if (ensureJumpedInstructionAreAloneOnLines)
                    {
                        foreach (Hex hexJumpedon in listRAM_AddressJumpedOn)
                        {
                            if ((_firstInstructionRAM_Hex + new Hex(i)) == hexJumpedon)
                            {
                                // We skip a line
                                _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);

                                if (_channelLinesPreFiltered[currentLine].LineHasAnyInstruction())
                                {
                                    ConditionalFakeLineMaker(ref currentLine);
                                }

                                // This is so next line is skipped
                                instructionIsJumpedOn = true;
                                break;
                            }
                        }
                    }
                    
                    if (currentHexCode == "00")
                    {
                        setInstructionTriplet("00");
                    }
                    else if (currentHexCode == "01")
                    {
                        setInstructionConnect("01");
                        connectState.ConnectCommandMet(currentLine);
                    }
                    else if (currentHexCode == "02")
                    {
                        SetLocationForNewInstruction(currentLine);
                        plusHalf = true;
                    }
                    else if (currentHexCode == "03")
                    {
                        setInstructionOctavePlus("03");
                    }
                    else if (currentHexCode == "04")
                    {
                        setInstructionFlag("04");
                    }
                    else if (currentHexCode == "05")
                    {
                        setInstructionSpeed("05");
                    }
                    else if (currentHexCode == "06")
                    {
                        setInstructionToneLenght("06");
                    }
                    else if (currentHexCode == "07")
                    {
                        setInstructionVolume("07");
                    }
                    else if (currentHexCode == "08")
                    {
                        setInstructionInstrument("08");
                    }
                    else if (currentHexCode == "09")
                    {
                        setInstructionOctave("09");

                        // Set current octave
                        currentOctave = Hex.ConvertStringHexToInt(channelDatas[i]);
                    }
                    else if (currentHexCode == "0A")
                    {
                        setInstructionGlobalTranspose("0A");
                    }
                    else if (currentHexCode == "0B")
                    {
                        setInstructionTranspose("0B");
                    }
                    else if (currentHexCode == "0C")
                    {
                        setInstructionTunePitch("0C");
                    }
                    else if (currentHexCode == "0D")
                    {
                        setInstructionPitchSlide("0D");
                    }
                    else if (currentHexCode == "0E")
                    {

                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            
                            _channelLinesPreFiltered[currentLine].SetLoop1(Hex.ConvertStringHexToInt(channelDatas[i + 1]), channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "0F")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetLoop2(Hex.ConvertStringHexToInt(channelDatas[i + 1]), channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "10")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetLoop3(Hex.ConvertStringHexToInt(channelDatas[i + 1]), channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "11")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetLoop4(Hex.ConvertStringHexToInt(channelDatas[i + 1]), channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "12")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetBreak1(channelDatas[i + 1], channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "13")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetBreak2(channelDatas[i + 1], channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "14")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetBreak3(channelDatas[i + 1], channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "15")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 3) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetBreak4(channelDatas[i + 1], channelDatas[i + 2], channelDatas[i + 3], gameType);

                            // To skip the parameters read
                            i += 3;
                        }
                    }
                    else if (currentHexCode == "16")
                    {
                        // Since Loops, Jump and Breaks are always done first on a line, if there is a music sheet instruction that must
                        // be done before, go on a new line
                        ConditionalFakeLineMaker(ref currentLine);

                        SetLocationForNewInstruction(currentLine);

                        // Ensure not to go out of bound
                        if ((i + 2) < channelDatas.Count())
                        {
                            SetLocationForNewInstruction(currentLine);
                            SetLocationForNewInstruction(currentLine);

                            _channelLinesPreFiltered[currentLine].SetJump(channelDatas[i + 1], channelDatas[i + 2]);

                            // To skip the parameters read
                            i += 2;
                        }
                    }
                    else if (currentHexCode == "17")
                    {
                        // If ending line
                        SetLocationForNewInstruction(currentLine);
                    }
                    else if (currentHexCode == "18")
                    {
                        setInstructionToneType("18");
                    }
                    else if (currentHexCode == "SS") { setInstructionSnesPanning("SS"); } // Snes Command (we pretend panning is SS hex code)
                    else if (currentHexCode == "19") { setInstructionGlobalVolume("19"); } // Snes Command
                    else if (currentHexCode == "1A") // Snes Command
                    {
                        setInstruction1A();
                    }
                    else if (currentHexCode == "1B") { setInstructionSfxToggle("1B"); } // Snes Command
                    else if (currentHexCode == "1C") { setInstructionSfx("1C"); } // Snes Command
                    else if (currentHexCode == "1D") { setInstructionFullNoteLenght("1D"); } // Snes Command
                    else if (currentHexCode == "1E") { setInstructionPanningMovementSpeed("1E"); } // Snes Command
                    else if (currentHexCode == "1F") { setInstructionUnknown1F("1F"); } // Snes Command
                    else // A note
                    {
                        // Note code is only stored to make some checks for connect
                        _channelLinesPreFiltered[currentLine].SetNoteCode(currentHexCode);

                        SetLocationForNewInstruction(currentLine);

                        if (isNoiseChannel)
                        {
                            noteDatas = GetMusicEngineNotesDatas.GetNoiseDatasByHexCodes(currentHexCode);
                        }
                        else
                        {
                            noteDatas = GetMusicEngineNotesDatas.GetDatasByHexCodes(currentHexCode);
                            AddOctaveToNote(currentOctave, ref noteDatas._Name);
                        }

                        // Write note name on line
                        _channelLinesPreFiltered[currentLine].SetNote(noteDatas._Name);

                        // If not writing connect on lines, then we don,t write the note in the connect
                        if (!writeConnectInMusicSheet)
                        {
                            if (!connectState.IsConnectInactive()  && !connectState.IsConnectLookingForFirstNote())
                            {
                                _channelLinesPreFiltered[currentLine].SetNote(MusicSheetsInstructionsDatas.ColumnsEmptyValue.NesA.Note);
                            }
                        }

                        // Call function that handles connect state and that checks note to take a decision
                        connectState.NoteMet(currentHexCode, currentLine);

                        noteLenght = ReturnNoteLineQuantityByNoteHex(currentHexCode);

                        // If plushalf mode
                        if (plusHalf)
                        {
                            plusHalf = false;
                            noteLenght += noteLenght / 2;
                        }

                        for (int j = noteLenght; j > 0; j--)
                        {
                            _channelLinesPreFiltered.Add(new MusicLinePreFiltered());
                            currentLine++;
                        }
                    }
                }
                
                // For loops and break, set lines positions
                for (i = 0; i < _channelLinesPreFiltered.Count; i++)
                {
                    if (_channelLinesPreFiltered[i]._Loop1.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Loop1.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Loop1.SetLine(_instructionLine[tempInt]);
                    }
                    if (_channelLinesPreFiltered[i]._Loop2.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Loop2.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Loop2.SetLine(_instructionLine[tempInt]);
                    }
                    if (_channelLinesPreFiltered[i]._Loop3.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Loop3.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Loop3.SetLine(_instructionLine[tempInt]);
                    }
                    if (_channelLinesPreFiltered[i]._Loop4.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Loop4.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Loop4.SetLine(_instructionLine[tempInt]);
                    }
                    if (_channelLinesPreFiltered[i]._Jump.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Jump.GetAddress();
                        
                        if (new Hex(tempStr) < _firstInstructionRAM_Hex)
                        {
                            // Here it means it jumps in another channel, chhanel 8 of Get Weapon in Mega Man X 3 jump in Channel 1.
                            // So we just jump somewhere else
                            tempInt = 0;

                            System.Windows.Forms.MessageBox.Show("Jump on channel " + ChannelName.ReturnChannelNameByChannelIndex(currentChannelID, gameType) + " goes to another channel. It was changed to jump on current channel first line.", "Jump to another channel", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                        else
                        {
                            tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();
                        }

                        _channelLinesPreFiltered[i]._Jump.SetLine(_instructionLine[tempInt]);
                    }

                    if (_channelLinesPreFiltered[i]._Break1.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Break1.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Break1.SetLine(_instructionLine[tempInt]);
                    }
                    if (_channelLinesPreFiltered[i]._Break2.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Break2.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Break2.SetLine(_instructionLine[tempInt]);
                    }
                    if (_channelLinesPreFiltered[i]._Break3.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Break3.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Break3.SetLine(_instructionLine[tempInt]);
                    }
                    if (_channelLinesPreFiltered[i]._Break4.IsActive())
                    {
                        // Get integer position of instruction in the channel
                        tempStr = _channelLinesPreFiltered[i]._Break4.GetAddress();
                        tempInt = (new Hex(tempStr) - _firstInstructionRAM_Hex).GetValueAsInt();

                        _channelLinesPreFiltered[i]._Break4.SetLine(_instructionLine[tempInt]);
                    }
                }
                _channelLinesPreFiltered[currentLine].SetNote(Names.NesA.Skip);
                
                // Build the correct list of music lines (the list we built had too much datas that were used to make decisions)
                _channelLines = new List<MusicLine>();
                for (int j = 0; j < _channelLinesPreFiltered.Count; j++)
                {
                    _channelLines.Add(new MusicLine());
                    _channelLines[j] = (MusicLine)_channelLinesPreFiltered[j];
                }
            }
            catch (KnownException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new KnownException("ChannelInterpreterFromHex caused an error when i value was " + i);
            }
        }
    }
}