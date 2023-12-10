using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicSheetsInstructionsDatas
{
    public static class ColumnsEmptyValue
    {
        public static string symbolRequiredForEmptyCells = "--";

        public static class NesA
        {
            public static readonly string Triplet = symbolRequiredForEmptyCells + "-";
            public static readonly string Connect = symbolRequiredForEmptyCells + "-";
            public static readonly string PlusHalf = symbolRequiredForEmptyCells + "-";      // Value is to indicate it doesn't have a column and is unique
            public static readonly string OctavePlus = symbolRequiredForEmptyCells + "-";
            public static readonly string SetFlags = symbolRequiredForEmptyCells;
            public static readonly string Speed = symbolRequiredForEmptyCells + "|" + symbolRequiredForEmptyCells;
            public static readonly string ToneLenght = symbolRequiredForEmptyCells;
            public static readonly string Volume = symbolRequiredForEmptyCells;
            public static readonly string Instrument = symbolRequiredForEmptyCells;
            public static readonly string Octave = symbolRequiredForEmptyCells;
            public static readonly string GlobalTranspose = symbolRequiredForEmptyCells;
            public static readonly string Transpose = symbolRequiredForEmptyCells;
            public static readonly string TunePitch = symbolRequiredForEmptyCells;
            public static readonly string PitchSlide = symbolRequiredForEmptyCells;
            public static readonly string Loop1 = symbolRequiredForEmptyCells + "|-----";
            public static readonly string Loop2 = Loop1;
            public static readonly string Loop3 = Loop1;
            public static readonly string Loop4 = Loop1;
            public static readonly string Break1 = Loop1;
            public static readonly string Break2 = Break1;
            public static readonly string Break3 = Break1;
            public static readonly string Break4 = Break1;
            public static readonly string Jump =  "-----";
            public static readonly string End = symbolRequiredForEmptyCells;           // Value is to indicate it doesn't have a column and is unique
            public static readonly string ToneType = symbolRequiredForEmptyCells + "---";
            public static readonly string Note = symbolRequiredForEmptyCells + "-";

            // Snes commands
            public static readonly string Panning = symbolRequiredForEmptyCells;
            public static readonly string VibratoDepth = symbolRequiredForEmptyCells;
            public static readonly string TremoloVolume = symbolRequiredForEmptyCells;
            public static readonly string VibratoOrTremolFrequency = symbolRequiredForEmptyCells;
            public static readonly string ins1A03 = symbolRequiredForEmptyCells;
            public static readonly string PitchSlideUnk = symbolRequiredForEmptyCells;
            public static readonly string ToneLenght2 = symbolRequiredForEmptyCells;
            public static readonly string SfxToggle = symbolRequiredForEmptyCells + "|" + symbolRequiredForEmptyCells;
            public static readonly string Sfx = symbolRequiredForEmptyCells;
            public static readonly string GlobalVolume = symbolRequiredForEmptyCells;
            public static readonly string PannMovementSpeed = symbolRequiredForEmptyCells;
            public static readonly string Unknown1F = symbolRequiredForEmptyCells;
        }
    }
}
