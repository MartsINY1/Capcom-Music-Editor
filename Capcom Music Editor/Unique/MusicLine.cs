using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mega_Music_Editor.Reusable;
using Mega_Music_Editor.Unique.MusicEngineFixedInstructionsDatas;
using Mega_Music_Editor.Unique.MusicEngineFixedNotesDatas;

namespace Mega_Music_Editor.Unique
{
    public class HexAddressOfALine
    {
        public readonly Hex _hexAddress;
        public readonly int _line;

        public HexAddressOfALine(Hex hexAddress, int line)
        {
            _hexAddress = hexAddress;
            _line = line;
        }
    }
    public class StandardElement
    {
        readonly bool _Active;
        readonly string _HexValue;
        readonly string _instructionName = "";

        public StandardElement()
        {
            _Active = false;
            _HexValue = "";
        }
        public StandardElement(bool Active, string HexValue, string instructionName)
        {
            _Active = Active;
            _HexValue = HexValue;
            _instructionName = instructionName;
        }

        public bool IsActive() { return _Active; }
        public string GetValue() { return _HexValue; }
        public string GetCommandAsHex(GameType gameType)
        {
            if (!_Active) return "";

            string hexCodes = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(_instructionName)._HexCode;

            if (gameType == GameType.NesA)
            {
                return string.Concat(hexCodes, _HexValue);
            }
            else if (gameType == GameType.SnesA)
            {
                if (hexCodes == "SS") hexCodes = "18";
                return string.Concat(hexCodes, _HexValue);
            }

            return string.Concat(hexCodes, _HexValue);
        }
        public int GetNumberOfHexCodesOfCommandAsHex(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHex(gameType).Length / 2;
        }
    }
    public class ToneType
    {
        readonly bool _Active;
        readonly string _Value;
        readonly string _HexValue;
        readonly string _instructionName = "";

        public ToneType()
        {
            _Active = false;
            _Value = "";
            _HexValue = "";
        }
        public ToneType(bool Active, string Value, string instructionName)
        {
            _Active = Active;
            _instructionName = instructionName;

            if (Value.Contains('%'))
            {
                _Value = Value;

                if (_Value == "12.5%") _HexValue = "00";
                else if (_Value == "25.0%") _HexValue = "40";
                else if (_Value == "50.0%") _HexValue = "80";
                else _HexValue = "C0";
            }
            else
            {
                _HexValue = Value;

                if (_HexValue == "00") _Value = "12.5%";
                else if (_HexValue == "40") _Value = "25.0%";
                else if (_HexValue == "80") _Value = "50.0%";
                else _Value = "75%";
            }
        }

        public bool IsActive() { return _Active; }
        public string GetValue() { return _Value; }
        public string GetCommandAsHex(GameType gameType)
        {
            if (!_Active) return "";

            string hexCodes = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(_instructionName)._HexCode;

            if (gameType == GameType.NesA)
            {
                return string.Concat(hexCodes, _HexValue);
            }
            else if (gameType == GameType.SnesA)
            {
                return string.Concat(hexCodes, _HexValue);
            }

            return string.Concat(hexCodes, _HexValue);
        }
        public int GetNumberOfHexCodesOfCommandAsHex(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHex(gameType).Length / 2;
        }
    }
        public class Note
    {
        private readonly bool _Active;
        private readonly string _Note;
        private static string _LastNoteHexCodePlayed;

        public Note()
        {
            _Active = false;
            _Note = "";
        }
        public Note(bool Active, string Note)
        {
            _Active = Active;
            _Note = Note;
        }

        public bool IsActive() { return _Active; }
        public string GetNote() { return _Note; }
        public string GetNoteStringHex(GameType gameType, int lineQty, bool noteStart, bool noteEnd, bool connectAlreadyOn, bool connectStartedManually, bool isNoiseChannel)
        {
            string noteHexCode = "";
            string hexStringToPlayNote = "";
            string temp = "";
            bool connect = connectAlreadyOn;
            Lenghts.NoteLenght noteLenghtDatas = null;
            bool isPlusHalfNote = false;
            int lineQtyLeft = lineQty;
            
            if (lineQty == 0) return "";

            if (noteStart && !noteEnd)
            {
                connect = true;

                if (!connectStartedManually)
                {
                    hexStringToPlayNote += "01";
                }
            }
            
            // Notes are the same for nes/snes
            if (gameType == GameType.NesA || gameType == GameType.SnesA)
            {
                do
                {
                    isPlusHalfNote = false;
                    
                    if (_Active)
                    {
                        if (isNoiseChannel)
                        {
                            noteHexCode = GetMusicEngineNotesDatas.GetDatasByNoiseNote(_Note)._HexCode;
                        }
                        else
                        {
                            noteHexCode = GetMusicEngineNotesDatas.GetDatasByNote(_Note)._HexCode;
                        }
                        _LastNoteHexCodePlayed = noteHexCode;

                        noteLenghtDatas = Lenghts.NesA.GetNoteLenghtDatasOfNoteWithClosestQtyOfLine(lineQtyLeft, out isPlusHalfNote);
                    }
                    else
                    {
                        // In playing previous note (in a connect)
                        noteHexCode = _LastNoteHexCodePlayed;
                        noteLenghtDatas = Lenghts.NesA.GetNoteLenghtDatasOfNoteWithClosestQtyOfLine(lineQtyLeft, out isPlusHalfNote);
                    }

                    if (isPlusHalfNote)
                    {
                        lineQtyLeft -= noteLenghtDatas.GetLineQtyPlusHalf();

                        if (lineQtyLeft > 0 && connect == false)
                        {
                            connect = true;

                            if (!connectStartedManually)
                            {
                                hexStringToPlayNote += "01";
                            }
                        }
                        else if (lineQtyLeft == 0 && connect == true)
                        {
                            if (noteEnd)
                            {
                                hexStringToPlayNote += "01";
                                connect = false;
                            }
                        }

                        hexStringToPlayNote += "02";
                    }
                    else
                    {
                        lineQtyLeft -= noteLenghtDatas.GetLineQty();

                        if (lineQtyLeft > 0 && connect == false)
                        {
                            connect = true;
                            if (!connectStartedManually)
                            {
                                hexStringToPlayNote += "01";
                            }
                        }
                        else if (lineQtyLeft == 0 && connect == true)
                        {
                            if (noteEnd)
                            {
                                hexStringToPlayNote += "01";
                                connect = false;
                            }
                        }
                    }

                    temp = (new Hex(noteLenghtDatas.GetHexCode()) + new Hex(noteHexCode)).GetValueAsString(0);

                    hexStringToPlayNote += temp;
                }
                while (lineQtyLeft > 0);

                return hexStringToPlayNote;
            }

            return "";
        }
        public int GetNumberOfHexCodesOfNoteString(GameType gameType, int lineQty, bool noteStart, bool noteEnd, bool connectAldreadyOn, bool connectStartedManually, bool isNoiseChannel)
        {
            return GetNoteStringHex(gameType, lineQty, noteStart, noteEnd, connectAldreadyOn, connectStartedManually, isNoiseChannel).Length/2;
        }
    }
    public class StandardElementTwoParameters
    {
        readonly bool _Active;
        readonly string _HexValueLow;
        readonly string _HexValueHigh;
        readonly string _instructionName;

        public StandardElementTwoParameters()
        {
            _Active = false;
            _HexValueLow = "";
            _HexValueHigh = "";
            _instructionName = "";
        }
        public StandardElementTwoParameters(bool Active, string HexValueHigh, string HexValueLow, string instructionName)
        {
            _Active = Active;
            _HexValueHigh = HexValueHigh;
            _HexValueLow = HexValueLow;
            _instructionName = instructionName;
        }
        public bool IsActive() { return _Active; }
        public string GetHighValue() { return _HexValueHigh; }
        public string GetLowValue() { return _HexValueLow; }
        public string GetCommandAsHex(GameType gameType)
        {
            if (!_Active) return "";

            string hexCodes = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(_instructionName)._HexCode;
            
            if (gameType == GameType.NesA)
            {
                return string.Concat(hexCodes, _HexValueHigh, _HexValueLow);
            }
            else if (gameType == GameType.SnesA)
            {
                return string.Concat(hexCodes, _HexValueHigh, _HexValueLow);
            }

            return string.Concat(hexCodes, _HexValueHigh, _HexValueLow);
        }
        public int GetNumberOfHexCodesOfCommandAsHex(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHex(gameType).Length / 2;
        }
    }
    public class Triplet
    {
        bool _Active;

        public Triplet()
        {
            _Active = false;
        }
        public void Toggle()
        {
            _Active = !_Active;
        }
        public bool IsActive() { return _Active; }
        public string GetCommandAsHex(GameType gameType)
        {
            if (!_Active) return "";

            if (gameType == GameType.NesA)
            {
                return GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Triplet)._HexCode;
            }
            else if (gameType == GameType.SnesA)
            {
                return GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Triplet)._HexCode;
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommandAsHex(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHex(gameType).Length / 2;
        }
    }
    public class Connect
    {
        bool _Active;

        public Connect()
        {
            _Active = false;
        }
        public void Toggle()
        {
            _Active = !_Active;
        }
        public void SetConnect()
        {
            _Active = true;
        }
        public void ClearConnect()
        {
            _Active = false;
        }
        public bool IsActive() { return _Active; }
        public string GetCommandAsHex(GameType gameType)
        {
            if (!_Active) return "";

            if (gameType == GameType.NesA)
            {
                return GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Connect)._HexCode;
            }
            else if (gameType == GameType.SnesA)
            {
                return GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Connect)._HexCode;
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommandAsHex(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHex(gameType).Length / 2;
        }
    }
    public class OctPlus
    {
        bool _Active;

        public OctPlus()
        {
            _Active = false;
        }
        public void Toggle()
        {
            _Active = !_Active;
        }
        public bool IsActive() { return _Active; }
        public string GetCommandAsHex(GameType gameType)
        {
            if (!_Active) return "";

            if (gameType == GameType.NesA)
            {
                return GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.OctavePlus)._HexCode;
            }
            else if (gameType == GameType.SnesA)
            {
                return GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.OctavePlus)._HexCode;
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommandAsHex(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHex(gameType).Length / 2;
        }
    }
    public class TogglingElement
    {
        bool _Active;

        public TogglingElement()
        {
            _Active = false;
        }
        public void Toggle()
        {
            _Active = !_Active;
        }
        public bool IsActive() { return _Active; }
        public string GetCommandAsHex()
        {
            if (!_Active) return "";
            
            return GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.SfxToggle)._HexCode;
        }
        public int GetNumberOfHexCodesOfCommandAsHex()
        {
            if (!_Active) return 0;
            return GetCommandAsHex().Length / 2;
        }
    }
    public class Loop
    {
        int _Line;
        string _Address;
        readonly bool _Active;
        readonly string _InstructionHexCode;
        readonly int _NumberOfLoops;

        public Loop()
        {
            _Active = false;
            _NumberOfLoops = 0;
            _Address = "";
        }
        public Loop(bool Active, string instructionHexCode,int NumberOfLoops, string HighAddress, string LowAddress)
        {
            _Active = Active;
            _InstructionHexCode = instructionHexCode;
            _NumberOfLoops = NumberOfLoops;
            _Address = string.Concat(HighAddress, LowAddress);
        }
        public Loop(bool Active, string InstructionHexCode, int NumberOfLoops, int Line)
        {
            _Active = Active;
            _InstructionHexCode = InstructionHexCode;
            _NumberOfLoops = NumberOfLoops;
            _Line = Line;
        }
        public bool IsActive() { return _Active; }
        public int GetNumberOfLoops() { return _NumberOfLoops; }
        public int GetLine()
        {
            return _Line;
        }
        public string GetAddress() { return _Address; }

        public void SetLine(int Line) { _Line = Line; }
        public string GetCommandAsHex(GameType gameType, List<HexAddressOfALine> listHexAddressOfLines)
        {
            if (!_Active) return "";
            
            if (gameType == GameType.NesA)
            {
                _Address = listHexAddressOfLines[_Line]._hexAddress.GetValueAsString(4);
                return string.Concat(_InstructionHexCode, (new Hex(_NumberOfLoops)).GetValueAsString(2), _Address);
            }
            else if (gameType == GameType.SnesA)
            {
                _Address = listHexAddressOfLines[_Line]._hexAddress.GetValueAsString(4);
                return string.Concat(_InstructionHexCode, (new Hex(_NumberOfLoops)).GetValueAsString(2), _Address);
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommand(GameType gameType, List<HexAddressOfALine> listHexAddressOfLines)
        {
            return GetCommandAsHex(gameType, listHexAddressOfLines).Length / 2;
        }


        private string GetCommandAsHexWithoutAddress(GameType gameType)
        {
            // For NesA
            string Address;

            if (gameType == GameType.NesA)
            {
                Address = "----";
                return string.Concat(_InstructionHexCode, (new Hex(_NumberOfLoops)).GetValueAsString(2), Address);
            }
            else if (gameType == GameType.SnesA)
            {
                Address = "----";
                return string.Concat(_InstructionHexCode, (new Hex(_NumberOfLoops)).GetValueAsString(2), Address);
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommandWithoutAddress(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHexWithoutAddress(gameType).Length / 2;
        }
    }
    public class Jump
    {
        int _Line;
        string _Address;
        readonly bool _Active;

        public Jump()
        {
            _Active = false;
            _Address = "";
        }
        public Jump(bool Active, string HighAddress, string LowAddress)
        {
            _Active = Active;
            _Address = string.Concat(HighAddress, LowAddress);
        }
        public Jump(bool Active, int Line)
        {
            _Active = Active;
            _Line = Line;
        }

        public bool IsActive() { return _Active; }
        public int GetLine()
        {
            return _Line;
        }
        public string GetAddress() { return _Address; }
        public void SetLine(int Line) { _Line = Line; }

        public string GetCommandAsHex(GameType gameType, List<HexAddressOfALine> listHexAddressOfLines)
        {
            if (!_Active) return "";

            if (gameType == GameType.NesA)
            {
                _Address = listHexAddressOfLines[_Line]._hexAddress.GetValueAsString(4);
                return string.Concat(MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump, _Address);
            }
            else if (gameType == GameType.SnesA)
            {
                _Address = listHexAddressOfLines[_Line]._hexAddress.GetValueAsString(4);
                return string.Concat(MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump, _Address);
            }

            return "";
        }

        public int GetNumberOfHexCodesOfCommand(GameType gameType, List<HexAddressOfALine> listHexAddressOfLines)
        {
            return GetCommandAsHex(gameType, listHexAddressOfLines).Length / 2;
        }


        private string GetCommandAsHexWithoutAddress(GameType gameType)
        {
            // For NesA
            string Address;

            if (gameType == GameType.NesA)
            {
                Address = "----";
                return string.Concat(MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump, Address);
            }
            else if (gameType == GameType.SnesA)
            {
                Address = "----";
                return string.Concat(MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump, Address);
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommandWithoutAddress(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHexWithoutAddress(gameType).Length / 2;
        }
    }
    public class Break
    {
        readonly bool _Active;
        readonly string _Flag;
        readonly string _InstructionHexCode;
        string _Address;
        int _Line;

        public Break()
        {
            _Active = false;
            _Flag = "";
            _Address = "";
        }
        public Break(bool Active, string InstructionHexCode, string Flag, string HighAddress, string LowAddress)
        {
            _Active = Active;
            _Flag = Flag;
            _Address = string.Concat(HighAddress, LowAddress);
            _InstructionHexCode = InstructionHexCode;
        }
        public Break(bool Active, string InstructionHexCode, string Flag, int Line)
        {
            _Active = Active;
            _Flag = Flag;
            _Line = Line;
            _InstructionHexCode = InstructionHexCode;
        }
        public bool IsActive() { return _Active; }
        public int GetLine()
        {
            return _Line;
        }
        public string GetAddress() { return _Address; }
        public string GetFlag() { return _Flag; }
        public void SetLine(int Line) { _Line = Line; }

        public string GetCommandAsHex(GameType gameType, List<HexAddressOfALine> listHexAddressOfLines)
        {
            if (!_Active) return "";
            
            if (gameType == GameType.NesA)
            {
                _Address = listHexAddressOfLines[_Line]._hexAddress.GetValueAsString(4);
                return string.Concat(_InstructionHexCode, (new Hex(_Flag)).GetValueAsString(2), _Address);
            }
            else if (gameType == GameType.SnesA)
            {
                _Address = listHexAddressOfLines[_Line]._hexAddress.GetValueAsString(4);
                return string.Concat(_InstructionHexCode, (new Hex(_Flag)).GetValueAsString(2), _Address);
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommand(GameType gameType, List<HexAddressOfALine> listHexAddressOfLines)
        {
            return GetCommandAsHex(gameType, listHexAddressOfLines).Length / 2;
        }


        private string GetCommandAsHexWithoutAddress(GameType gameType)
        {
            // For NesA
            string Address;

            if (gameType == GameType.NesA)
            {
                Address = "----";
                return string.Concat(_InstructionHexCode, (new Hex(_Flag)).GetValueAsString(2), Address);
            }
            else if (gameType == GameType.SnesA)
            {
                Address = "----";
                return string.Concat(_InstructionHexCode, (new Hex(_Flag)).GetValueAsString(2), Address);
            }

            return "";
        }
        public int GetNumberOfHexCodesOfCommandWithoutAddress(GameType gameType)
        {
            if (!_Active) return 0;
            return GetCommandAsHexWithoutAddress(gameType).Length / 2;
        }
    }

    public class MusicLine
    {
        public Note _Note;
        public StandardElement _Volume;
        public StandardElement _Instrument;
        public StandardElement _Octave;
        public StandardElement _ToneLenght;
        public StandardElement _TunePitch;
        public StandardElement _PitchSlide;
        public StandardElement _Flag;
        public ToneType _ToneType;
        public StandardElement _Transpose;
        public StandardElement _GlobalTranspose;
        public StandardElementTwoParameters _Speed;
        public OctPlus _OctavePlus;
        public Triplet _Triplet;
        public Connect _Connect;
        public Loop _Loop1, _Loop2, _Loop3, _Loop4;
        public Jump _Jump;
        public Break _Break1, _Break2, _Break3, _Break4;

        // Snes
        public StandardElement _Panning;
        public StandardElement _VibratoDept;
        public StandardElement _TremoloVolume;
        public StandardElement _VibratoOrTremoloFrequency;
        public StandardElement _ins1A03;
        public StandardElement _PitchSlideUnk;
        public StandardElement _ToneLength2;
        public StandardElementTwoParameters _SfxToggle;
        public StandardElement _Sfx;
        public StandardElement _GlobalVolume;
        public StandardElement _PanningMovementSpeed;
        public StandardElement _unknown1F;

        public MusicLine()
        {
            // By default a line has nothing
            _Note = new Note();
            _Volume = new StandardElement();
            _Instrument = new StandardElement();
            _Octave = new StandardElement();
            _ToneLenght = new StandardElement();
            _TunePitch = new StandardElement();
            _PitchSlide = new StandardElement();
            _ToneType = new ToneType();
            _Speed = new StandardElementTwoParameters();
            _Transpose = new StandardElement();
            _GlobalTranspose = new StandardElement();
            _Flag = new StandardElement();
            _Triplet = new Triplet();
            _Connect = new Connect();
            _OctavePlus = new OctPlus();
            _Panning = new StandardElement();
            _VibratoDept = new StandardElement();
            _TremoloVolume = new StandardElement();
            _VibratoOrTremoloFrequency = new StandardElement();
            _ins1A03 = new StandardElement();
            _PitchSlideUnk = new StandardElement();
            _ToneLength2 = new StandardElement();
            _SfxToggle = new StandardElementTwoParameters();
            _Sfx = new StandardElement();
            _GlobalVolume = new StandardElement();
            _PanningMovementSpeed = new StandardElement();
            _unknown1F = new StandardElement();


            _Loop1 = new Loop();
            _Loop2 = new Loop();
            _Loop3 = new Loop();
            _Loop4 = new Loop();
            _Jump = new Jump();

            _Break1 = new Break();
            _Break2 = new Break();
            _Break3 = new Break();
            _Break4 = new Break();
        }

        public bool LineHasLoopBreakInstructions()
        {
            return (_Loop1.IsActive() || _Loop2.IsActive() || _Loop3.IsActive() || _Loop4.IsActive()
                || _Jump.IsActive()
                || _Break1.IsActive() || _Break2.IsActive() || _Break3.IsActive() || _Break4.IsActive());
        }


        public bool LineHasMusicSheetsInstructions(bool checkForNote = false)
        {
            if (checkForNote && _Note.IsActive()) return true;
            if (_Volume.IsActive())
            {
                return true;
            }
            if (_Instrument.IsActive())
            {
                return true;
            }
            if (_Octave.IsActive())
            {
                return true;
            }
            if (_ToneLenght.IsActive())
            {
                return true;
            }
            if (_TunePitch.IsActive())
            {
                return true;
            }
            if (_PitchSlide.IsActive())
            {
                return true;
            }
            if (_Flag.IsActive())
            {
                return true;
            }
            if (_ToneType.IsActive())
            {
                return true;
            }
            if (_Transpose.IsActive())
            {
                return true;
            }
            if (_GlobalTranspose.IsActive())
            {
                return true;
            }
            if (_Speed.IsActive())
            {
                return true;
            }
            if (_OctavePlus.IsActive())
            {
                return true;
            }
            if (_Connect.IsActive())
            {
                return true;
            }
            if (_Triplet.IsActive())
            {
                return true;
            }

            // Snes commands
            if (_Panning.IsActive())
            {
                return true;
            }
            if (_VibratoDept.IsActive())
            {
                return true;
            }
            if (_TremoloVolume.IsActive())
            {
                return true;
            }
            if (_VibratoOrTremoloFrequency.IsActive())
            {
                return true;
            }
            if (_ToneLength2.IsActive())
            {
                return true;
            }
            if (_SfxToggle.IsActive())
            {
                return true;
            }
            if (_Sfx.IsActive())
            {
                return true;
            }
            if (_GlobalVolume.IsActive())
            {
                return true;
            }
            if (_PanningMovementSpeed.IsActive())
            {
                return true;
            }
            if (_unknown1F.IsActive())
            {
                return true;
            }
            return false;
        }

        public bool LineHasAnyInstruction(bool checkForNote = false)
        {
            if (LineHasLoopBreakInstructions() || LineHasMusicSheetsInstructions(checkForNote))
            {
                return true;
            }
            return false;
        }
        
        public void SetNote(string note) { _Note = new Note(true, note); }
        public void SetVolume(string volume) { _Volume = new StandardElement(true, volume, MusicEngineFixedInstructionsDatas.Names.Volume); }
        public void SetInstrument(string instrument) { _Instrument = new StandardElement(true, instrument, MusicEngineFixedInstructionsDatas.Names.Instrument); }
        public void SetOctave(string octave) { _Octave = new StandardElement(true, octave, MusicEngineFixedInstructionsDatas.Names.Octave); }
        public void SetToneLenght(string toneLenght) { _ToneLenght = new StandardElement(true, toneLenght, MusicEngineFixedInstructionsDatas.Names.ToneLenght); }
        public void SetTunePitch(string pitch) { _TunePitch = new StandardElement(true, pitch, MusicEngineFixedInstructionsDatas.Names.TunePitch); }
        public void SetPitchSlide(string slide) { _PitchSlide = new StandardElement(true, slide, MusicEngineFixedInstructionsDatas.Names.PitchSlide); }
        public void SetFlags(string flag) { _Flag = new StandardElement(true, flag, MusicEngineFixedInstructionsDatas.Names.Flags); }
        public void SetToneType(string toneType) { _ToneType = new ToneType(true, toneType, MusicEngineFixedInstructionsDatas.Names.ToneType); }
        public void SetPanning(string panning) { _Panning = new StandardElement(true, panning, MusicEngineFixedInstructionsDatas.Names.Panning); }
        public void SetGlobalVolume(string globalVolume) { _GlobalVolume = new StandardElement(true, globalVolume, MusicEngineFixedInstructionsDatas.Names.GlobalVolume); }
        public void SetVibratoDepth(string VibratoDepth) { _VibratoDept = new StandardElement(true, VibratoDepth, MusicEngineFixedInstructionsDatas.Names.VibratoDepth); }
        public void SetTremoloVolume(string TremoloVolume) { _TremoloVolume = new StandardElement(true, TremoloVolume, MusicEngineFixedInstructionsDatas.Names.TremoloVolume); }
        public void SetVibratoOrTremolFrequency(string VibratoOrTremolFrequency) { _VibratoOrTremoloFrequency = new StandardElement(true, VibratoOrTremolFrequency, MusicEngineFixedInstructionsDatas.Names.VibratoOrTremolFrequency); }
        public void Set1A03(string ins1A03) { _ins1A03 = new StandardElement(true, ins1A03, MusicEngineFixedInstructionsDatas.Names.ins1A03); }
        public void SetPitchSlideUnk(string PitchSlideUnk) { _PitchSlideUnk = new StandardElement(true, PitchSlideUnk, MusicEngineFixedInstructionsDatas.Names.PitchSlideUnk); }
        public void SetSfxToggle(string hexValueHigh, string hexValueLow) { _SfxToggle = new StandardElementTwoParameters(true, hexValueHigh, hexValueLow, MusicEngineFixedInstructionsDatas.Names.SfxToggle); }
        public void SetSfx(string Sfx) { _Sfx = new StandardElement(true, Sfx, MusicEngineFixedInstructionsDatas.Names.Sfx); }
        public void SetToggleFullNoteLenght(string toggleFullNoteLenght) { _ToneLength2 = new StandardElement(true, toggleFullNoteLenght, MusicEngineFixedInstructionsDatas.Names.ToneLenght2); }
        public void SetPanningMovementSpeed(string PanningMovementSPeed) { _PanningMovementSpeed = new StandardElement(true, PanningMovementSPeed, MusicEngineFixedInstructionsDatas.Names.PannMovementSpeed); }
        public void SetUnknown1F(string unknown1F) { _unknown1F = new StandardElement(true, unknown1F, MusicEngineFixedInstructionsDatas.Names.Unknown1F); }
        public void SetTranspose(string transpose) { _Transpose = new StandardElement(true, transpose, MusicEngineFixedInstructionsDatas.Names.Transpose); }
        public void SetGlobalTranspose(string transposeAll) { _GlobalTranspose = new StandardElement(true, transposeAll, MusicEngineFixedInstructionsDatas.Names.GlobalTranspose); }
        public void SetSpeed(string hexValueHigh, string hexValueLow) { _Speed = new StandardElementTwoParameters(true, hexValueHigh, hexValueLow, MusicEngineFixedInstructionsDatas.Names.Speed); }
        public void SetConnect(bool writeConnectInMusicSheet) { if (writeConnectInMusicSheet) _Connect.Toggle(); }
        public void SetTriplet() { _Triplet.Toggle(); }
        public void SetOctavePlus() { _OctavePlus.Toggle(); }
        public void SetLoop1(int NumberOfLoops, string HighAddress, string LowAddress, GameType gameType)
        {
            /// bbb
            if (gameType == GameType.NesA)
            {
                _Loop1 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop1, NumberOfLoops, HighAddress, LowAddress);
            }
            else
            {
                _Loop1 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop1, NumberOfLoops, HighAddress, LowAddress);
            }
        }
        public void SetLoop1(int NumberOfLoops, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Loop1 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop1, NumberOfLoops, Line);
            }
            else
            {
                _Loop1 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop1, NumberOfLoops, Line);
            }
        }
        public void SetLoop2(int NumberOfLoops, string HighAddress, string LowAddress, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Loop2 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop2, NumberOfLoops, HighAddress, LowAddress);
            }
            else
            {
                _Loop2 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop2, NumberOfLoops, HighAddress, LowAddress);
            }
        }
        public void SetLoop2(int NumberOfLoops, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Loop2 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop2, NumberOfLoops, Line);
            }
            else
            {
                _Loop2 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop2, NumberOfLoops, Line);
            }
        }
        public void SetLoop3(int NumberOfLoops, string HighAddress, string LowAddress, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Loop3 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop3, NumberOfLoops, HighAddress, LowAddress);
            }
            else
            {
                _Loop3 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop3, NumberOfLoops, HighAddress, LowAddress);
            }
        }
        public void SetLoop3(int NumberOfLoops, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Loop3 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop3, NumberOfLoops, Line);
            }
            else
            {
                _Loop3 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop3, NumberOfLoops, Line);
            }
        }
        public void SetLoop4(int NumberOfLoops, string HighAddress, string LowAddress, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Loop4 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop4, NumberOfLoops, HighAddress, LowAddress);
            }
            else
            {
                _Loop4 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop4, NumberOfLoops, HighAddress, LowAddress);
            }
        }
        public void SetLoop4(int NumberOfLoops, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Loop4 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop4, NumberOfLoops, Line);
            }
            else
            {
                _Loop4 = new Loop(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop4, NumberOfLoops, Line);
            }
        }
        public void SetJump(string HighAddress, string LowAddress) { _Jump = new Jump(true, HighAddress, LowAddress); }
        public void SetJump(int Line) { _Jump = new Jump(true, Line); }
        public void SetBreak1(string Flag, string HighAddress, string LowAddress, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break1 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break1, Flag, HighAddress, LowAddress);
            }
            else
            {
                _Break1 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break1, Flag, HighAddress, LowAddress);
            }
        }
        public void SetBreak1(string Flag, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break1 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break1, Flag, Line);
            }
            else
            {
                _Break1 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break1, Flag, Line);
            }
        }
        public void SetBreak2(string Flag, string HighAddress, string LowAddress, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break2 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break2, Flag, HighAddress, LowAddress);
            }
            else
            {
                _Break2 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break2, Flag, HighAddress, LowAddress);
            }
        }
        public void SetBreak2(string Flag, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break2 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break2, Flag, Line);
            }
            else
            {
                _Break2 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break2, Flag, Line);
            }
        }
        public void SetBreak3(string Flag, string HighAddress, string LowAddress, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break3 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break3, Flag, HighAddress, LowAddress);
            }
            else
            {
                _Break3 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break3, Flag, HighAddress, LowAddress);
            }
        }
        public void SetBreak3(string Flag, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break3 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break3, Flag, Line);
            }
            else
            {
                _Break3 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break3, Flag, Line);
            }
        }
        public void SetBreak4(string Flag, string HighAddress, string LowAddress, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break4 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break4, Flag, HighAddress, LowAddress);
            }
            else
            {
                _Break4 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break4, Flag, HighAddress, LowAddress);
            }
        }
        public void SetBreak4(string Flag, int Line, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                _Break4 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break4, Flag, Line);
            }
            else
            {
                _Break4 = new Break(true, MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break4, Flag, Line);
            }
        }

        public void ClearConnect() { _Connect.ClearConnect(); }
    }

    /// <summary>
    /// This class has some temporary elements that will be filtered to obtain a MusicLine object.
    /// For the connect, some datas are more easily analysed once whole song is stored
    /// </summary>
    public class MusicLinePreFiltered : MusicLine
    {
        public string _NoteCode;

        public MusicLinePreFiltered() : base()
        {
            _NoteCode = "";
        }

        public void SetNoteCode(string noteCode) { _NoteCode = noteCode; }
    }
}