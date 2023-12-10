using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicSheetsInstructionsDatas
{
    public static class ColumnsWidth
    {
        static private readonly string noWidth = "-1000";

        public static class NesA
        {
            public static readonly string Triplet = "60";
            public static readonly string Connect = "60";
            public static readonly string PlusHalf = noWidth;      // Value is to indicate it doesn't have a column and is unique
            public static readonly string OctavePlus = "60";
            public static readonly string SetFlags = "40";
            public static readonly string Speed = "40|40";
            public static readonly string ToneLenght = "40";
            public static readonly string Volume = "40";
            public static readonly string Instrument = "40";
            public static readonly string Octave = "40";
            public static readonly string GlobalTranspose = "40";
            public static readonly string Transpose = "40";
            public static readonly string TunePitch = "40";
            public static readonly string PitchSlide = "40";
            public static readonly string Loop1 = "120|120";
            public static readonly string Loop2 = "120|120";
            public static readonly string Loop3 = "120|120";
            public static readonly string Loop4 = "120|120";
            public static readonly string Break1 = "120|120";
            public static readonly string Break2 = "120|120";
            public static readonly string Break3 = "120|120";
            public static readonly string Break4 = "120|120";
            public static readonly string Jump = "120";
            public static readonly string End = noWidth;           // Value is to indicate it doesn't have a column and is unique
            public static readonly string ToneType = "70";
            public static readonly string Note = "50";

            // Snes commands
            public static readonly string Panning = "40";
            public static readonly string VibratoDepth = "40";
            public static readonly string TremoloVolume = "40";
            public static readonly string VibratoOrTremolFrequency = "40";
            public static readonly string ins1A03 = "40";
            public static readonly string PitchSlideUnk = "40";
            public static readonly string ToneLenght2 = "40";
            public static readonly string SfxToggle = "40|40";
            public static readonly string Sfx = "40";
            public static readonly string GlobalVolume = "40";
            public static readonly string PannMovementSpeed = "40";
            public static readonly string Unknown1F = "40";
        }
    }
}
