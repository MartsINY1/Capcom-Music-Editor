using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mega_Music_Editor.Reusable;

namespace Mega_Music_Editor.Unique
{
    public static class ChannelName
    {
        static public string ReturnChannelNameByChannelIndex(int channelId, GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                switch (channelId)
                {
                    case 0:
                        {
                            return "Square 1";
                        }
                    case 1:
                        {
                            return "Square 2";
                        }
                    case 2:
                        {
                            return "Triangle";
                        }
                    case 3:
                        {
                            return "Noise";
                        }
                    default:
                        {
                            return "No Channel with that index";
                        }
                }
            }
            else if (gameType == GameType.SnesA)
            {
                switch (channelId)
                {
                    case 0:
                        {
                            return "Channel 1";
                        }
                    case 1:
                        {
                            return "Channel 2";
                        }
                    case 2:
                        {
                            return "Channel 3";
                        }
                    case 3:
                        {
                            return "Channel 4";
                        }
                    case 4:
                        {
                            return "Channel 5";
                        }
                    case 5:
                        {
                            return "Channel 6";
                        }
                    case 6:
                        {
                            return "Channel 7";
                        }
                    case 7:
                        {
                            return "Channel 8";
                        }
                    default:
                        {
                            return "No Channel with that index";
                        }
                }
            }
            return "Game Type unknown";
        }
    }

    class Misc
    {
        

        /// <summary>
        /// Return integer position where to given a RAM address
        /// </summary>
        /// <param name="RAM_Bank"></param>
        /// <param name="ROM_Bank"></param>
        /// <param name="RAM_Address_Start"></param>
        static public int ReturnAbsoluteROM_PositionFromRAM_Int(Hex RAM_BankHex, Hex ROM_BankHex, Hex RAM_Address_StartHex)
        {
            try
            {
                return (ROM_BankHex + RAM_Address_StartHex - RAM_BankHex).GetValueAsInt();
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
