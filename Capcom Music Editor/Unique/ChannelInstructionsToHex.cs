using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mega_Music_Editor.Reusable;

namespace Mega_Music_Editor.Unique
{
    public class ChannelInstructionsToHex
    {
        private List<string> soundDatasToWrite;

        public List<string> Get_soundDatasToWrite()
        {
            return soundDatasToWrite;
        }

        private void CheckIfNoteEndAfterLoopBreakOrJump(int y, List<MusicLine> listMusicLine, ref bool noteEnd)
        {
            while (y < listMusicLine.Count)
            {
                // We skip all skipper line, and then maybe we will realise note ends
                if (listMusicLine[y + 1]._Note.GetNote() != MusicEngineFixedNotesDatas.Names.NesA.Skip)
                {
                    // There is a note right after the skippers
                    if (listMusicLine[y + 1]._Note.IsActive())
                    {
                        noteEnd = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                if ((y + 2) >= listMusicLine.Count)
                {
                    noteEnd = true;
                    break;
                }
                y++;
            }
        }

        private void PrepareParametersForNotesFunctionCall(int currentLineIndex, List<MusicLine> listMusicLine, out bool noteStart, out bool noteEnd, out int lineQty)
        {
            // Reset everything
            noteStart = false;
            noteEnd = false;
            lineQty = 0;

            // Check for note start
            if (listMusicLine[currentLineIndex]._Note.IsActive() && listMusicLine[currentLineIndex]._Note.GetNote() != MusicEngineFixedNotesDatas.Names.NesA.Skip)
            {
                noteStart = true;
            }

            // Because of the skipping instruction, it's possible to reach many skipping line one after the others. On one of those,
            // there could be a loop for example, which would cut note. However, since we wouldn't have met another note or the end of the
            // sheet, we wouldn't consider the note ending. Though, the note could end after the skipping instruction and we would just
            // be cutting the note. This is why we need to check the note ending and the number of lines separately.
            // With bool and such we could use only one loop but it would be complicated to understand. This way code is more easy to
            // understand.

            // Check for note ending
            for (int y = currentLineIndex; y < listMusicLine.Count; y++)
            {
                if ((y + 1) < listMusicLine.Count)
                {
                    // Note is ending only if on next line there is another note
                    // This condition must be first because it ends note
                    if (listMusicLine[y + 1]._Note.IsActive())
                    {
                        if (listMusicLine[y + 1]._Note.GetNote() != MusicEngineFixedNotesDatas.Names.NesA.Skip)
                        {
                            noteEnd = true;
                            break;
                        }
                    }
                    if (listMusicLine[y + 1].LineHasMusicSheetsInstructions())
                    {
                        CheckIfNoteEndAfterLoopBreakOrJump(y, listMusicLine, ref noteEnd);
                        break;
                    }
                    if (listMusicLine[y + 1].LineHasLoopBreakInstructions())
                    {
                        CheckIfNoteEndAfterLoopBreakOrJump(y, listMusicLine, ref noteEnd);
                        break;
                    }
                }
                else
                {
                    noteEnd = true;
                    break;
                }
            }

            // Line is at least one line long if it's not a skipper
            if (listMusicLine[currentLineIndex]._Note.GetNote() != MusicEngineFixedNotesDatas.Names.NesA.Skip)
            {
                lineQty++;
            }

            for (int y = currentLineIndex; y < listMusicLine.Count; y++)
            {   
                if ((y + 1) < listMusicLine.Count)
                {
                    if (listMusicLine[y + 1].LineHasMusicSheetsInstructions())
                    {
                        break;
                    }
                    if (listMusicLine[y + 1].LineHasLoopBreakInstructions())
                    {
                        break;
                    }
                    if (!listMusicLine[y + 1]._Note.IsActive())
                    {
                        lineQty++;
                    }
                    else
                    {
                        if (listMusicLine[y + 1]._Note.GetNote() != MusicEngineFixedNotesDatas.Names.NesA.Skip)
                        {
                            break;
                        }
                        // Else it's a line skipper so it doesn't add to the quantity
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void GenerateCodeArraysWithoutAddresses(string RAM_Start, List<List<MusicLine>> _channelLinesInMemory, GameType gameType, bool doNotWriteChannelEnd, out List<List<HexAddressOfALine>> arraysHexAddressesOfLines, Hex snesMemoryLoadPointerHex)
        {
            int currentLineIndex = 0;
            List<string> builder;
            int qtyHexCodeOnLine = 0;
            soundDatasToWrite = new List<string>();
            List<HexAddressOfALine> hexAddressesOfLines;
            arraysHexAddressesOfLines = new List<List<HexAddressOfALine>>();
            bool noteStart = false;
            bool noteEnd = false;
            int currentChannel = 0;
            bool connectAlreadyOn = false;
            bool connectStartedManually = false; // Started by a command in the music grid
            int lineQty = 0;
            int nextLineWithNote = -1;
            string flagValue = "";

            Hex currentRAM_Address = new Hex(RAM_Start);

            if (gameType == GameType.NesA)
            {
                currentRAM_Address = new Hex(currentRAM_Address.GetValueAsInt() + 9);
            }
            else if (gameType == GameType.SnesA)
            {
                // This is Memory Load Pointer for the SNES + Number of pointers (this is the RAM Address for the first channel)
                currentRAM_Address = snesMemoryLoadPointerHex + Hex.ConvertStringHexToHex("10");
            }

            foreach (List<MusicLine> listMusicLine in _channelLinesInMemory)
            {
                currentLineIndex = 0;
                builder = new List<string>();
                hexAddressesOfLines = new List<HexAddressOfALine>();

                currentChannel += 1;

                // In case there is a channel without anything, we must make sure the quantity of hex code on line is reset
                qtyHexCodeOnLine = 0;

                foreach (MusicLine musicLine in listMusicLine)
                {
                    qtyHexCodeOnLine = 0;
                    
                    
                    hexAddressesOfLines.Add(new HexAddressOfALine(currentRAM_Address + new Hex(qtyHexCodeOnLine), currentLineIndex));


                    qtyHexCodeOnLine += musicLine._Break1.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);
                    qtyHexCodeOnLine += musicLine._Break2.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);
                    qtyHexCodeOnLine += musicLine._Break3.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);
                    qtyHexCodeOnLine += musicLine._Break4.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);


                    qtyHexCodeOnLine += musicLine._Loop1.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);
                    qtyHexCodeOnLine += musicLine._Loop2.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);
                    qtyHexCodeOnLine += musicLine._Loop3.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);
                    qtyHexCodeOnLine += musicLine._Loop4.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);

                    qtyHexCodeOnLine += musicLine._Jump.GetNumberOfHexCodesOfCommandWithoutAddress(gameType);






                    // Get qty for instructions
                    qtyHexCodeOnLine += musicLine._Volume.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._Instrument.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._Octave.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._ToneLenght.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._TunePitch.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._PitchSlide.GetNumberOfHexCodesOfCommandAsHex(gameType);

                    if (gameType == GameType.NesA) qtyHexCodeOnLine += musicLine._ToneType.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._Speed.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._Transpose.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._GlobalTranspose.GetNumberOfHexCodesOfCommandAsHex(gameType);

                    // Snes only instructions
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._Panning.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._VibratoDept.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._TremoloVolume.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._VibratoOrTremoloFrequency.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._ToneLength2.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._SfxToggle.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._Sfx.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._GlobalVolume.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._PanningMovementSpeed.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) qtyHexCodeOnLine += musicLine._unknown1F.GetNumberOfHexCodesOfCommandAsHex(gameType);

                    flagValue = musicLine._Flag.GetValue();

                    if (flagValue != "")
                    {
                        if ((new Hex(flagValue) & new Hex("40")).GetValueAsString(2) == "40")
                        {
                            connectStartedManually = true;
                        }
                        else
                        {
                            connectStartedManually = false;
                        }
                    }
                    if (musicLine._Connect.IsActive())
                    {
                        connectStartedManually = true;
                    }
                    else
                    {
                        connectStartedManually = false;
                    }


                    qtyHexCodeOnLine += musicLine._Flag.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._Triplet.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._Connect.GetNumberOfHexCodesOfCommandAsHex(gameType);
                    qtyHexCodeOnLine += musicLine._OctavePlus.GetNumberOfHexCodesOfCommandAsHex(gameType);


                    if (musicLine._Note.IsActive() && musicLine._Note.GetNote() != MusicEngineFixedNotesDatas.Names.NesA.Skip || nextLineWithNote == currentLineIndex)
                        {
                            PrepareParametersForNotesFunctionCall(currentLineIndex, listMusicLine, out noteStart, out noteEnd, out lineQty);

                        if (currentChannel == 3)
                        {
                            qtyHexCodeOnLine += musicLine._Note.GetNumberOfHexCodesOfNoteString(gameType, lineQty, noteStart, noteEnd, connectAlreadyOn, connectStartedManually, true);
                        }
                        else
                        {
                            qtyHexCodeOnLine += musicLine._Note.GetNumberOfHexCodesOfNoteString(gameType, lineQty, noteStart, noteEnd, connectAlreadyOn, connectStartedManually, false);
                        }


                        if (noteEnd == false)
                        {
                            nextLineWithNote = currentLineIndex + lineQty;
                            connectAlreadyOn = true;
                        }
                        else
                        {
                            // So it's never a line we reach
                            nextLineWithNote = -1;
                            connectAlreadyOn = false;
                        }
                    }

                        



                    currentLineIndex++;
                    currentRAM_Address += new Hex(qtyHexCodeOnLine);
                }

                // If there is nothing on the channel
                if (hexAddressesOfLines.Count == 0)
                {
                    // We set at least address where this channel is for channel ptr
                    hexAddressesOfLines.Add(new HexAddressOfALine(currentRAM_Address + new Hex(qtyHexCodeOnLine), currentLineIndex));
                }
                // For the end of channel, 17 code
                if (!doNotWriteChannelEnd) currentRAM_Address += new Hex(1);

                arraysHexAddressesOfLines.Add(hexAddressesOfLines);
            }
        }

        private List<string> GenerateCodeArrays(List<List<MusicLine>> _channelLinesInMemory, GameType gameType, List<List<HexAddressOfALine>> arraysHexAddressesOfLines, bool doNotWriteChannelEnd)
        {
            int currentLineIndex = 0;
            List<string> dataLayers = new List<string>();
            string tempStr = "";
            bool noteStart = false;
            bool noteEnd = false;
            bool connectAldreadyOn = false;
            bool connectStartedManually = false; // Started by a command in the music grid
            int lineQty = 0;
            int currentChannel = -1;
            int nextLineWithNote = -1;
            string flagValue = "";

            if (gameType == GameType.NesA)
            {
                tempStr = "";

                foreach (List<HexAddressOfALine> arraysHexRAM_AddressByChannelForEachLine in arraysHexAddressesOfLines)
                {
                    tempStr += arraysHexRAM_AddressByChannelForEachLine[0]._hexAddress.GetValueAsString(4);
                }
                dataLayers.Add(tempStr);
            }
            else if (gameType == GameType.SnesA)
            {
                tempStr = "";

                foreach (List<HexAddressOfALine> arraysHexRAM_AddressByChannelForEachLine in arraysHexAddressesOfLines)
                {
                    if (arraysHexRAM_AddressByChannelForEachLine.Count != 0)
                    {
                        tempStr += arraysHexRAM_AddressByChannelForEachLine[0]._hexAddress.GetValueAsString(4);
                    }
                    
                }
                dataLayers.Add(tempStr);
            }

            foreach (List<MusicLine> listMusicLine in _channelLinesInMemory)
            {
                currentLineIndex = 0;
                tempStr = "";
                currentChannel += 1;

                foreach (MusicLine musicLine in listMusicLine)
                {












                        tempStr += musicLine._Break1.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);
                        tempStr += musicLine._Break2.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);
                        tempStr += musicLine._Break3.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);
                        tempStr += musicLine._Break4.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);



                        tempStr += musicLine._Loop1.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);
                        tempStr += musicLine._Loop2.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);
                        tempStr += musicLine._Loop3.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);
                        tempStr += musicLine._Loop4.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);

                        tempStr += musicLine._Jump.GetCommandAsHex(gameType, arraysHexAddressesOfLines[currentChannel]);





                    tempStr += musicLine._Volume.GetCommandAsHex(gameType);
                    tempStr += musicLine._Instrument.GetCommandAsHex(gameType);
                    tempStr += musicLine._Octave.GetCommandAsHex(gameType);
                    tempStr += musicLine._ToneLenght.GetCommandAsHex(gameType);
                    tempStr += musicLine._TunePitch.GetCommandAsHex(gameType);
                    tempStr += musicLine._PitchSlide.GetCommandAsHex(gameType);

                    if (gameType == GameType.NesA)
                    {
                        tempStr += musicLine._ToneType.GetCommandAsHex(gameType);
                    }
                    
                    tempStr += musicLine._Speed.GetCommandAsHex(gameType);
                    tempStr += musicLine._Transpose.GetCommandAsHex(gameType);
                    tempStr += musicLine._GlobalTranspose.GetCommandAsHex(gameType);
                    

                    if (gameType == GameType.SnesA) tempStr += musicLine._Panning.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._VibratoDept.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._TremoloVolume.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._VibratoOrTremoloFrequency.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._ToneLength2.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._SfxToggle.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._Sfx.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._GlobalVolume.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._PanningMovementSpeed.GetCommandAsHex(gameType);
                    if (gameType == GameType.SnesA) tempStr += musicLine._unknown1F.GetCommandAsHex(gameType);




                    flagValue = musicLine._Flag.GetValue();

                    if (flagValue != "")
                    {
                        if ((new Hex(flagValue) & new Hex("40")).GetValueAsString(2) == "40")
                        {
                            connectStartedManually = true;
                        }
                        else
                        {
                            connectStartedManually = false;
                        }
                    }
                    if (musicLine._Connect.IsActive())
                    {
                        connectStartedManually = true;
                    }
                    else
                    {
                        connectStartedManually = false;
                    }


                    
                    tempStr += musicLine._Flag.GetCommandAsHex(gameType);
                    tempStr += musicLine._Triplet.GetCommandAsHex(gameType);
                    tempStr += musicLine._Connect.GetCommandAsHex(gameType);
                    tempStr += musicLine._OctavePlus.GetCommandAsHex(gameType);

                    if (musicLine._Note.IsActive() && musicLine._Note.GetNote() != MusicEngineFixedNotesDatas.Names.NesA.Skip || nextLineWithNote == currentLineIndex)
                    {
                        PrepareParametersForNotesFunctionCall(currentLineIndex, listMusicLine, out noteStart, out noteEnd, out lineQty);

                        // Noise channel is different for nes
                        if (currentChannel == 3 && gameType == GameType.NesA)
                        {
                            tempStr += musicLine._Note.GetNoteStringHex(gameType, lineQty, noteStart, noteEnd, connectAldreadyOn, connectStartedManually, true);
                        }
                        else
                        {
                            tempStr += musicLine._Note.GetNoteStringHex(gameType, lineQty, noteStart, noteEnd, connectAldreadyOn, connectStartedManually, false);
                        }




                        if (noteEnd == false)
                        {
                            nextLineWithNote = currentLineIndex + lineQty;
                            connectAldreadyOn = true;
                        }
                        else
                        {
                            // So it's never a line we reach
                            nextLineWithNote = -1;
                            connectAldreadyOn = false;
                        }
                    }






                        



                    currentLineIndex++;
                }

                // If we write 17 hex code on lines aaa
                if (!doNotWriteChannelEnd) tempStr += "17";

                dataLayers.Add(tempStr);
            }
            
            return dataLayers;
        }

        private void GenerateSongString(string RAM_Start, List<List<MusicLine>> _channelLinesInMemory, GameType gameType, bool doNotWriteChannelEnd, Hex snesMemoryLoadPointerHex)
        {
            List<List<HexAddressOfALine>> arraysHexAddressesOfLines = new List<List<HexAddressOfALine>>();

            soundDatasToWrite = new List<string>();

            GenerateCodeArraysWithoutAddresses(RAM_Start, _channelLinesInMemory, gameType, doNotWriteChannelEnd, out arraysHexAddressesOfLines, snesMemoryLoadPointerHex);
            soundDatasToWrite = GenerateCodeArrays(_channelLinesInMemory, gameType, arraysHexAddressesOfLines, doNotWriteChannelEnd);
        }

        public ChannelInstructionsToHex(List<List<MusicLine>> _channelLinesInMemory, Hex RAM_Start, GameType gameType, bool doNotWriteChannelEnd, Hex snesMemoryLoadPointerHex)
        {
            int channelQty = _channelLinesInMemory.Count;
            string RAM_StartStr = "";

            List<string> soundDatasToWrite = new List<string>();
            
            RAM_StartStr = RAM_Start.GetValueAsString(4);

            GenerateSongString(RAM_StartStr, _channelLinesInMemory, gameType, doNotWriteChannelEnd, snesMemoryLoadPointerHex);
        }
    }
}
