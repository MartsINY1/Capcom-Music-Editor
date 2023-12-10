using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicSheetsInstructionsDatas
{
    // They need to be in order they are considered
    public enum PageName
    {
        NoPage = 0,
        Breaks = 1,
        Loops = 2,
        Notes = 3
    }

    public static class Page
    {
        public static class NesA
        {
            public static readonly PageName Triplet = PageName.Notes;
            public static readonly PageName Connect = PageName.Notes; 
            public static readonly PageName PlusHalf = PageName.Notes;
            public static readonly PageName OctavePlus = PageName.Notes;
            public static readonly PageName SetFlags = PageName.Notes;
            public static readonly PageName Speed = PageName.Notes;
            public static readonly PageName ToneLenght = PageName.Notes;
            public static readonly PageName Volume = PageName.Notes;
            public static readonly PageName Instrument = PageName.Notes;
            public static readonly PageName Octave = PageName.Notes;
            public static readonly PageName GlobalTranspose = PageName.Notes;
            public static readonly PageName Transpose = PageName.Notes;
            public static readonly PageName TunePitch = PageName.Notes;
            public static readonly PageName PitchSlide = PageName.Notes;
            public static readonly PageName Loop1 = PageName.Loops;
            public static readonly PageName Loop2 = PageName.Loops;
            public static readonly PageName Loop3 = PageName.Loops;
            public static readonly PageName Loop4 = PageName.Loops;
            public static readonly PageName Break1 = PageName.Breaks;
            public static readonly PageName Break2 = PageName.Breaks;
            public static readonly PageName Break3 = PageName.Breaks;
            public static readonly PageName Break4 = PageName.Breaks;
            public static readonly PageName Jump = PageName.Loops;
            public static readonly PageName End = PageName.NoPage;
            public static readonly PageName ToneType = PageName.Notes;
            public static readonly PageName Note = PageName.Notes;

            // Snes commands
            public static readonly PageName Panning = PageName.Notes;
            public static readonly PageName VibratoDepth = PageName.Notes;
            public static readonly PageName TremoloVolume = PageName.Notes;
            public static readonly PageName VibratoOrTremolFrequency = PageName.Notes;
            public static readonly PageName ins1A03 = PageName.Notes;
            public static readonly PageName PitchSlideUnk = PageName.Notes;
            public static readonly PageName ToneLenght2 = PageName.Notes;
            public static readonly PageName SfxToggle = PageName.Notes;
            public static readonly PageName Sfx = PageName.Notes;
            public static readonly PageName GlobalVolume = PageName.Notes;
            public static readonly PageName PannMovementSpeed = PageName.Notes;
            public static readonly PageName Unknown1F = PageName.Notes;
        }
    }
}
