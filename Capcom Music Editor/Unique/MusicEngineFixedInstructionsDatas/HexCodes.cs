using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicEngineFixedInstructionsDatas
{
    public static class HexCodes
    {
        public static class NesA
        {
            public static readonly string Triplet = "00";
            public static readonly string Connect = "01";
            public static readonly string PlusHalf = "02";
            public static readonly string OctavePlus = "03";
            public static readonly string SetFlags = "04";
            public static readonly string Speed = "05";
            public static readonly string ToneLenght = "06";
            public static readonly string Volume = "07";
            public static readonly string Instrument = "08";
            public static readonly string Octave = "09";
            public static readonly string GlobalTranspose = "0A";
            public static readonly string Transpose = "0B";
            public static readonly string TunePitch = "0C";
            public static readonly string PitchSlide = "0D";
            public static readonly string Loop1 = "0E";
            public static readonly string Loop2 = "0F";
            public static readonly string Loop3 = "10";
            public static readonly string Loop4 = "11";
            public static readonly string Break1 = "12";
            public static readonly string Break2 = "13";
            public static readonly string Break3 = "14";
            public static readonly string Break4 = "15";
            public static readonly string Jump = "16";
            public static readonly string End = "17";
            public static readonly string ToneType = "18";
            public static readonly string Note = "NA";     // Any note has this code.

            // Snes commands
            public static readonly string Panning = "SS"; // We need to set a false value because else it has the same as tone type
                // When reading a snes song, code will replace 18 by AA as soon as met. When writing one, it will intercept hex code
                // SS and replace it by 18
            public static readonly string VibratoDepth = "1A00";
            public static readonly string TremoloVolume = "1A01";
            public static readonly string VibratoOrTremolFrequency = "1A02";
            public static readonly string ins1A03 = "1A03";
            public static readonly string PitchSlideUnk = "1A04";
            public static readonly string ToneLenght2 = "1D";
            public static readonly string SfxToggle = "1B";
            public static readonly string Sfx = "1C";
            public static readonly string GlobalVolume = "19";
            public static readonly string PannMovementSpeed = "1E";
            public static readonly string Unknown1F = "1F";
        }
    }
}
