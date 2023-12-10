using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicEngineFixedInstructionsDatas
{
    public static class QuantityParameters
    {
        public static class NesA
        {
            public static readonly int Triplet = 0;     // Quantity varies, so it's just a "filler" value
            public static readonly int Connect = 1;
            public static readonly int PlusHalf = 0;    // Quantity varies, so it's just a "filler" value
            public static readonly int OctavePlus = 0;
            public static readonly int SetFlags = 1;
            public static readonly int Speed = 2;
            public static readonly int ToneLenght = 1;
            public static readonly int Volume = 1;
            public static readonly int Instrument = 1;
            public static readonly int Octave = 1;
            public static readonly int GlobalTranspose = 1;
            public static readonly int PitchSlide = 1;
            public static readonly int Transpose = 1;
            public static readonly int TunePitch = 1;
            public static readonly int Loop1 = 3;
            public static readonly int Loop2 = 3;
            public static readonly int Loop3 = 3;
            public static readonly int Loop4 = 3;
            public static readonly int Break1 = 3;
            public static readonly int Break2 = 3;
            public static readonly int Break3 = 3;
            public static readonly int Break4 = 3;
            public static readonly int Jump = 2;
            public static readonly int End = 0;
            public static readonly int ToneType = 1;
            public static readonly int Note = 0;

            // Snes commands
            public static readonly int Panning = 1;
            public static readonly int VibratoDepth = 1;
            public static readonly int TremoloVolume = 1;
            public static readonly int VibratoOrTremolFrequency = 1;
            public static readonly int ins1A03 = 1;
            public static readonly int PitchSlideUnk = 1;
            public static readonly int ToneLenght2 = 1;
            public static readonly int SfxToggle = 2;
            public static readonly int Sfx = 1;
            public static readonly int GlobalVolume = 1;
            public static readonly int PannMovementSpeed = 1;
            public static readonly int Unknown1F = 1;
        }
    }
}
