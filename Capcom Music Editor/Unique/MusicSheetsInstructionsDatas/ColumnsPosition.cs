using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicSheetsInstructionsDatas
{
    public static class ColumnsPosition
    {
        public static class NesA
        {
            public static readonly int Triplet = 13;
            public static readonly int Connect = 14;
            public static readonly int PlusHalf = -2;      // Value is to indicate it doesn't have a column and is unique
            public static readonly int OctavePlus = 15;
            public static readonly int SetFlags = 12;
            public static readonly int Speed = 8;
            public static readonly int ToneLenght = 4;
            public static readonly int Volume = 1;
            public static readonly int Instrument = 2;
            public static readonly int Octave = 3;
            public static readonly int GlobalTranspose = 11;
            public static readonly int Transpose = 10;
            public static readonly int TunePitch = 5;
            public static readonly int PitchSlide = 6;
            public static readonly int Loop1 = 0;
            public static readonly int Loop2 = 2;
            public static readonly int Loop3 = 4;
            public static readonly int Loop4 = 6;
            public static readonly int Break1 = 0;
            public static readonly int Break2 = 2;
            public static readonly int Break3 = 4;
            public static readonly int Break4 = 6;
            public static readonly int Jump = 8;
            public static readonly int End = -4;           // Value is to indicate it doesn't have a column and is unique
            public static readonly int ToneType = 7;
            public static readonly int Note = 0;

            // Snes commands
            public static readonly int Panning = 16;
            public static readonly int VibratoDepth = 17;
            public static readonly int TremoloVolume = 18;
            public static readonly int VibratoOrTremolFrequency = 19;
            public static readonly int ins1A03 = 20;
            public static readonly int PitchSlideUnk = 21;
            public static readonly int ToneLenght2 = 22;
            public static readonly int SfxToggle = 23;
            public static readonly int Sfx = 25;
            public static readonly int GlobalVolume = 26;
            public static readonly int PannMovementSpeed = 27;
            public static readonly int Unknown1F = 28;
        }
    }
}
