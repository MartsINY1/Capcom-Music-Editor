using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mega_Music_Editor.Reusable;


namespace Mega_Music_Editor.Unique
{
    public static class ChannelReader
    {
        private const string endByte = "17";

        private static void HandleHexCommandReadWithDifferentUsageBetweenNesAndSnes(GameType gameType, ref string hexCode)
        {
            if (hexCode == "18")
            {
                if (gameType == GameType.SnesA)
                {
                    hexCode = "SS";
                }
            }
        }

        /// <summary>
        /// Calculate ROM Address of song's first instruction - Considering game type
        /// </summary>
        /// <param name="gameType">Game Type (Video Game Console, NES for example)</param>
        /// <param name="RAM_BankHex">As name implies</param>
        /// <param name="ROM_BankHex">As name implies</param>
        /// <param name="songRAM_AddressHex">As name implies</param>
        /// 
        public static Hex CalculateSongROM_Address(GameType gameType, Hex RAM_BankHex, Hex ROM_BankHex, Hex songRAM_AddressHex)
        {
            if (gameType == GameType.NesA) return(ROM_BankHex + songRAM_AddressHex - RAM_BankHex);
            else return(ROM_BankHex + songRAM_AddressHex + (RAM_BankHex* Hex.ConvertStringHexToHex("8000")));
        }

        /// <summary>
        /// Receive a channel location and returns it's content via a parameter
        /// </summary>
        /// <param name="position">Position to start reading</param>
        /// <param name="fs">Opened file to read</param>
        /// <param name="hexChannel">Out : Channel content in hex codes</param>
        /// <exception cref="SystemException">Indirect : If read out of file</exception>
        public static void ReadOneChannel(FileStream fs, GameType gameType, out string hexChannel, out List<string> oneChannelLstStr, out List<Hex> lstRAM_With_Jump, out bool tripletMet)
        {
            string temp = "";
            string hex = "";            // File reading variable
            bool stayIn = true;         // Stay in the loop while true
            bool jumpMet = false;       // Need to know when we are meeting a jump
            int paramQty = 0;           // Number of parameters for a given hex code
                                        // Example, command 07, volume, takes 1
            tripletMet = false;         // By default, suppose no triplet was met
            int positionOfAddressToFetch = 9999;
            string tempStr = "";

            oneChannelLstStr = new List<string>();
            lstRAM_With_Jump = new List<Hex>();

            // Ensure variable to return content is empty
            hexChannel = "";

            // Add all hex codes to channel string until end
            while (stayIn)
            {
                // Read a hex code from the file
                if ((hex = HexFileReader.ReadOneHexCode(fs)) == HexFileReader._endOfFileReachedFlag)
                {
                    throw new KnownException("End of file met early when interpreting channel.");
                }

                // Adjust hex code command if necessary
                //      This is in case it's Panning instruction
                HandleHexCommandReadWithDifferentUsageBetweenNesAndSnes(gameType, ref hex);

                hexChannel += hex;
                oneChannelLstStr.Add(hex);

                if (hex == endByte)
                {
                    stayIn = false;
                }
                else
                {
                    if (hex == "00") tripletMet = true;

                    paramQty = GetMusicEngineInstructionsDatas.GetDatasByHexCodes(hex)._QuantityParameters;

                    if (hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop1
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop2
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop3
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop4
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break1
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break2
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break3
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break4)
                    {
                        positionOfAddressToFetch = 1;

                        if (hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump)
                        {
                            positionOfAddressToFetch = 0;
                            jumpMet = true;
                        }
                    }

                    // For the followings, it's not a constant value (we pretend 0 instruction follow those).
                    // When reading channel like this and not interpreting them, it doesn't cause a problem. See how the instruction
                    // Hex : 00 = Triplet
                    // Hex : 01 = Connect
                    // Hex : 02 = PlusHalf

                    tempStr = "";

                    // Loop through all parameters
                    for (int i = 0; i < paramQty; i++)
                    {
                        //read a hex code from the file
                        temp = HexFileReader.ReadOneHexCode(fs);
                        if (temp == HexFileReader._endOfFileReachedFlag)
                        {
                            throw new KnownException("End of file met when reading parameters for an instruction.");
                        }
                        hexChannel += temp;
                        oneChannelLstStr.Add(temp);

                        if (i >= positionOfAddressToFetch)
                        {
                            tempStr += temp;
                        }
                    }

                    if (tempStr != "")
                    {
                        lstRAM_With_Jump.Add(new Reusable.Hex(tempStr));
                    }

                    if (jumpMet)
                    {
                        jumpMet = false;

                        // Jump means the end
                        stayIn = false;

                        // Read a hex code from the file
                        if ((hex = HexFileReader.ReadOneHexCode(fs)) == HexFileReader._endOfFileReachedFlag)
                        {
                            throw new KnownException("End of file met early when interpreting channel.");
                        }
                        
                        // If there was an end byte after the jump
                        if (hex == endByte)
                        {
                            hexChannel += hex;
                            oneChannelLstStr.Add(hex);
                        }
                        else
                        {
                            // We "unread" the byte but this remains the end since we had a jump
                            // In Snes, channels end with a jump
                            fs.Seek(fs.Position-1, SeekOrigin.Begin);
                        }
                    }

                    positionOfAddressToFetch = 9999;
                }
            }
            bool tempBool = true;
            bool duplicateFound = false;
            int elementID_ToDelete = 0;

            while (tempBool)
            {
                duplicateFound = false;

                for (int x = 0; x < lstRAM_With_Jump.Count; x++)
                {
                    for (int y = 0; y < lstRAM_With_Jump.Count; y++)
                    {
                        if (y != x && lstRAM_With_Jump[x] == lstRAM_With_Jump[y])
                        {
                            elementID_ToDelete = y;
                            duplicateFound = true;
                            break;
                        }
                    }
                    if (duplicateFound) break;
                }

                if (duplicateFound)
                {
                    lstRAM_With_Jump.RemoveAt(elementID_ToDelete);
                }
                else
                {
                    tempBool = false;
                }
            }
        }

        /// <summary>
        /// Receive a channel location and returns it's content via a list string for ASM6 style output
        /// </summary>
        /// <param name="startAddress">Address of first instruction</param>
        /// <param name="fs">Opened file to read</param>
        /// <param name="oneChannelLstStr">Channel output</param>
        /// <param name="currentChannelQtyHexCodes">Channel quantity of hex code</param>
        /// <exception cref="SystemException">Indirect : If read out of file</exception>
        public static void ReadOneChannelReturnOneInstructionPerLineAndListAddressesJumpedOn(Hex startAddress, FileStream fs, string songName, int channelId, out List<string> oneChannelLstStr, out int currentChannelQtyHexCodes)
        {
            string temp = "";
            string hex = "";            // File reading variable
            bool stayIn = true;         // Stay in the loop while true
            int paramQty = 0;           // Number of parameters for a given hex code
                                        // Example, command 07, volume, takes 1
            int positionOfAddressToFetch = 9999;

            List<Hex> oneChannelLstAddresses = new List<Hex>();
            List<Hex> lstRAM_With_Jump = new List<Hex>();
            List<string> lstInstructions = new List<string>();

            oneChannelLstStr = new List<string>();
            currentChannelQtyHexCodes = 0; // Initialise quantity of hex code for current channel



            // Add all hex codes to channel string until end
            // We build a list of every insructions
            while (stayIn)
            {
                // Read a hex code from the file
                if ((hex = HexFileReader.ReadOneHexCode(fs)) == HexFileReader._endOfFileReachedFlag)
                {
                    throw new KnownException("End of file met early when interpreting channel.");
                }
                lstInstructions.Add(hex);
                currentChannelQtyHexCodes += 1;

                if (hex == endByte)
                {
                    stayIn = false;
                }
                else
                {
                    paramQty = GetMusicEngineInstructionsDatas.GetDatasByHexCodes(hex)._QuantityParameters;

                    if (hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop1
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop2
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop3
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Loop4
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break1
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break2
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break3
                        || hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Break4)
                    {
                        positionOfAddressToFetch = 1;

                        if (hex == MusicEngineFixedInstructionsDatas.HexCodes.NesA.Jump) positionOfAddressToFetch = 0;
                    }

                    // For the followings, it's not a constant value (we pretend 0 instruction follow those).
                    // When reading channel like this and not interpreting them, it doesn't cause a problem. See how the instruction
                    // Hex : 00 = Triplet
                    // Hex : 01 = Connect
                    // Hex : 02 = PlusHalf

                    hex = "";

                    // Loop through all parameters
                    for (int i = 0; i < paramQty; i++)
                    {
                        //read a hex code from the file
                        temp = HexFileReader.ReadOneHexCode(fs);
                        currentChannelQtyHexCodes += 1;

                        if (temp == HexFileReader._endOfFileReachedFlag)
                        {
                            throw new KnownException("End of file met when reading parameters for an instruction.");
                        }
                        
                        if (i >= positionOfAddressToFetch)
                        {
                            hex += temp;
                        }
                        else
                        {
                            lstInstructions.Add(temp);
                        }
                    }


                    if (hex != "")
                    {
                        lstRAM_With_Jump.Add(new Hex(hex));

                        lstInstructions[lstInstructions.Count - 1] += "+" + (new Hex(hex) - startAddress).GetValueAsInt() + "=";
                    }

                    positionOfAddressToFetch = 9999;
                }
            }









            bool tempBool = true;
            bool duplicateFound = false;
            int elementID_ToDelete = 0;

            // Purge list of instruction we jump on
            while (tempBool)
            {
                duplicateFound = false;

                for (int x = 0; x < lstRAM_With_Jump.Count; x++)
                {
                    for (int y = 0; y < lstRAM_With_Jump.Count; y++)
                    {
                        if (y != x && lstRAM_With_Jump[x] == lstRAM_With_Jump[y])
                        {
                            elementID_ToDelete = y;
                            duplicateFound = true;
                            break;
                        }
                    }
                    if (duplicateFound) break;
                }

                if (duplicateFound)
                {
                    lstRAM_With_Jump.RemoveAt(elementID_ToDelete);
                }
                else
                {
                    tempBool = false;
                }
            }














            int endLoop2;
            int position = 0;
            string tempStr2 = "";

            // Calculate address of each instruction
            for (int x = 0; x < lstInstructions.Count; x++)
            {
                tempStr2 = lstInstructions[x];

                if (tempStr2.IndexOf("+") != -1)
                {
                    tempStr2 = tempStr2.Substring(0, tempStr2.IndexOf("+"));
                    endLoop2 = tempStr2.Length / 2 + 2;
                }
                else
                {
                    endLoop2 = tempStr2.Length / 2;
                }
                
                oneChannelLstAddresses.Add(startAddress + Hex.ConvertIntToHex(position));

                for (int y = 0; y < endLoop2; y++)
                {
                    position++;
                }
            }








            // Check every line we jump on and add addresse to it
            for (int x = 0; x < lstRAM_With_Jump.Count; x++)
            {
                for (int y = 0; y < oneChannelLstAddresses.Count; y++)
                {
                    if (lstRAM_With_Jump[x] == oneChannelLstAddresses[y])
                    {
                        lstInstructions[y] = "_" + (lstRAM_With_Jump[x] - startAddress).GetValueAsInt() + "-" + lstInstructions[y];
                    }
                }
            }




            string tempStr = "";
            string tempStr3 = "";
            int tempInt = 0;
            bool needHEX_Instruction = true;

            // Write pointer to channel i + 1 start
            oneChannelLstStr.Add(songName + "__CHANNEL_" + (channelId + 1) + ":");

            // Start looping through instructions
            for (int x = 0; x < oneChannelLstAddresses.Count; x++)
            {
                while (lstInstructions[x].IndexOf("_") != -1)
                {
                    tempStr = lstInstructions[x].Substring(lstInstructions[x].IndexOf("_") + 1, lstInstructions[x].IndexOf("-") - lstInstructions[x].IndexOf("_") - 1);
                    lstInstructions[x] = lstInstructions[x].Substring(lstInstructions[x].IndexOf("-") + 1);

                    int.TryParse(tempStr, out tempInt);
                    oneChannelLstStr.Add(songName + "__CHANNEL_" + (channelId + 1) + "__INSTRUCTION_" + (tempInt) + ":");

                    needHEX_Instruction = true;
                }

                if (needHEX_Instruction)
                {
                    oneChannelLstStr.Add("HEX");
                    needHEX_Instruction = false;
                }

                //oneChannelLstStr[oneChannelLstStr.Count - 1] += " " + lstInstructions[x].Substring(0, 2);
                oneChannelLstStr[oneChannelLstStr.Count - 1] += " " + lstInstructions[x].Substring(0,2);

                tempStr3 = lstInstructions[x].Substring(2);

                if (tempStr3 != "")
                {
                    tempStr3 = tempStr3.Substring(1, tempStr3.Length - 2);
                    int.TryParse(tempStr3, out tempInt);

                    oneChannelLstStr.Add("DH " + songName + "__CHANNEL_" + (channelId + 1) + "__INSTRUCTION_" + (tempInt));
                    oneChannelLstStr.Add("DL " + songName + "__CHANNEL_" + (channelId + 1) + "__INSTRUCTION_" + (tempInt));

                    needHEX_Instruction = true;
                }
            }
        }
    }
}
