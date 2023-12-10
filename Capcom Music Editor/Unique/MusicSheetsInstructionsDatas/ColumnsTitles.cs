using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicSheetsInstructionsDatas
{
    public static class ColumnsTitles
    {
        static private readonly string noTitle = "This is not supposed to have a title";

        public static class NesA
        {
            // "|" is used as a delimiter to go to next column
            public static readonly string Triplet = "Trip.";
            public static readonly string Connect = "Con.";
            public static readonly string PlusHalf = noTitle;
            public static readonly string OctavePlus = "Oct.+2";
            public static readonly string SetFlags = "Flg";
            public static readonly string Speed = "Spd";
            public static readonly string ToneLenght = "Len";
            public static readonly string Volume = "Vol.";
            public static readonly string Instrument = "Ins.";
            public static readonly string Octave = "Oct.";
            public static readonly string GlobalTranspose = "Glb.";
            public static readonly string Transpose = "Trs.";
            public static readonly string TunePitch = "Ptc.";
            public static readonly string PitchSlide = "Sld.";
            public static readonly string Loop1 = "Loop1 [Times]|Loop1 [Line]";
            public static readonly string Loop2 = "Loop2 [Times]|Loop2 [Line]";
            public static readonly string Loop3 = "Loop3 [Times]|Loop3 [Line]";
            public static readonly string Loop4 = "Loop4 [Times]|Loop4 [Line]";
            public static readonly string Break1 = "Break1 [Flag]|Break1 [Line]";
            public static readonly string Break2 = "Break2 [Flag]|Break2 [Line]";
            public static readonly string Break3 = "Break3 [Flag]|Break3 [Line]";
            public static readonly string Break4 = "Break4 [Flag]|Break4 [Line]";
            public static readonly string Jump = "Jump [Line]";
            public static readonly string End = noTitle;
            public static readonly string ToneType = "Dty Cyc";
            public static readonly string Note = "Note";

            // Snes commands
            public static readonly string Panning = "Pan.";
            public static readonly string VibratoDepth = "Vbr.";
            public static readonly string TremoloVolume = "Trm.";
            public static readonly string VibratoOrTremolFrequency = "Frq.";
            public static readonly string ins1A03 = "A03";
            public static readonly string PitchSlideUnk = "Unk";
            public static readonly string ToneLenght2 = "Len2";
            public static readonly string SfxToggle = "Sf??";
            public static readonly string Sfx = "Sfx.";
            public static readonly string GlobalVolume = "GlbV";
            public static readonly string PannMovementSpeed = "PanS";
            public static readonly string Unknown1F = "Unk";
        }
    }
}
