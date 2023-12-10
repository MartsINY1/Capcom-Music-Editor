using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mega_Music_Editor.Reusable;

namespace Mega_Music_Editor
{
    public struct FixedMusicEngineNoteStruct
    {
        public FixedMusicEngineNoteStruct(string Hexcode, string Name)
        {
            _HexCode = Hexcode;
            _Name = Name;
        }

        public string _HexCode;
        public string _Name;
    }

    public static class GetMusicEngineNotesDatas
    {
        private static readonly List<FixedMusicEngineNoteStruct> fixedMusicEngineNoteArrayNesA = new List<FixedMusicEngineNoteStruct>
        {
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Delay, Unique.MusicEngineFixedNotesDatas.Names.NesA.Delay),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.C_0, Unique.MusicEngineFixedNotesDatas.Names.NesA.C_0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Cd0, Unique.MusicEngineFixedNotesDatas.Names.NesA.Cd0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.D_0, Unique.MusicEngineFixedNotesDatas.Names.NesA.D_0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Dd0, Unique.MusicEngineFixedNotesDatas.Names.NesA.Dd0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.E_0, Unique.MusicEngineFixedNotesDatas.Names.NesA.E_0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.F_0, Unique.MusicEngineFixedNotesDatas.Names.NesA.F_0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Fd0, Unique.MusicEngineFixedNotesDatas.Names.NesA.Fd0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.G_0, Unique.MusicEngineFixedNotesDatas.Names.NesA.G_0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Gd0, Unique.MusicEngineFixedNotesDatas.Names.NesA.Gd0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.A_0, Unique.MusicEngineFixedNotesDatas.Names.NesA.A_0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Ad0, Unique.MusicEngineFixedNotesDatas.Names.NesA.Ad0),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.B_0, Unique.MusicEngineFixedNotesDatas.Names.NesA.B_0),

            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.C_1, Unique.MusicEngineFixedNotesDatas.Names.NesA.C_1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Cd1, Unique.MusicEngineFixedNotesDatas.Names.NesA.Cd1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.D_1, Unique.MusicEngineFixedNotesDatas.Names.NesA.D_1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Dd1, Unique.MusicEngineFixedNotesDatas.Names.NesA.Dd1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.E_1, Unique.MusicEngineFixedNotesDatas.Names.NesA.E_1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.F_1, Unique.MusicEngineFixedNotesDatas.Names.NesA.F_1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Fd1, Unique.MusicEngineFixedNotesDatas.Names.NesA.Fd1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.G_1, Unique.MusicEngineFixedNotesDatas.Names.NesA.G_1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Gd1, Unique.MusicEngineFixedNotesDatas.Names.NesA.Gd1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.A_1, Unique.MusicEngineFixedNotesDatas.Names.NesA.A_1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Ad1, Unique.MusicEngineFixedNotesDatas.Names.NesA.Ad1),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.B_1, Unique.MusicEngineFixedNotesDatas.Names.NesA.B_1),


            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.C_2, Unique.MusicEngineFixedNotesDatas.Names.NesA.C_2),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Cd2, Unique.MusicEngineFixedNotesDatas.Names.NesA.Cd2),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.D_2, Unique.MusicEngineFixedNotesDatas.Names.NesA.D_2),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Dd2, Unique.MusicEngineFixedNotesDatas.Names.NesA.Dd2),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.E_2, Unique.MusicEngineFixedNotesDatas.Names.NesA.E_2),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.F_2, Unique.MusicEngineFixedNotesDatas.Names.NesA.F_2),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Fd2, Unique.MusicEngineFixedNotesDatas.Names.NesA.Fd2)
        };

        private static readonly List<FixedMusicEngineNoteStruct> fixedMusicEngineNoiseNotesArrayNesA = new List<FixedMusicEngineNoteStruct>
        {
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.Delay, Unique.MusicEngineFixedNotesDatas.Names.NesA.Delay),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N0d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N0d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N1d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N1d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N2d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N2d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N3d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N3d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N4d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N4d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N5d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N5d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N6d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N6d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N7d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N7d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N8d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N8d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.N9d, Unique.MusicEngineFixedNotesDatas.Names.NesA.N9d),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.NAd, Unique.MusicEngineFixedNotesDatas.Names.NesA.NAd),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.NBd, Unique.MusicEngineFixedNotesDatas.Names.NesA.NBd),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.NCd, Unique.MusicEngineFixedNotesDatas.Names.NesA.NCd),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.NDd, Unique.MusicEngineFixedNotesDatas.Names.NesA.NDd),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.NEd, Unique.MusicEngineFixedNotesDatas.Names.NesA.NEd),
            new FixedMusicEngineNoteStruct(Unique.MusicEngineFixedNotesDatas.HexCodes.NesA.NFd, Unique.MusicEngineFixedNotesDatas.Names.NesA.NFd)
        };

        public static FixedMusicEngineNoteStruct GetDatasByHexCodes(string HexCode)
        {
            int arraySize = fixedMusicEngineNoteArrayNesA.Count;

            HexCode = (new Hex(HexCode) & new Hex("1F")).GetValueAsString(0);

            for (int i = 0; i < arraySize; i++)
            {
                if (GetMusicEngineNotesDatas.fixedMusicEngineNoteArrayNesA[i]._HexCode == HexCode)
                {
                    return GetMusicEngineNotesDatas.fixedMusicEngineNoteArrayNesA[i];
                }
            }
            return new FixedMusicEngineNoteStruct("", "");
        }

        public static FixedMusicEngineNoteStruct GetNoiseDatasByHexCodes(string HexCode)
        {
            int arraySize = fixedMusicEngineNoiseNotesArrayNesA.Count;

            HexCode = (new Hex(HexCode) & new Hex("1F")).GetValueAsString(0);

            for (int i = 0; i < arraySize; i++)
            {
                if (GetMusicEngineNotesDatas.fixedMusicEngineNoiseNotesArrayNesA[i]._HexCode == HexCode)
                {
                    return GetMusicEngineNotesDatas.fixedMusicEngineNoiseNotesArrayNesA[i];
                }
            }
            return new FixedMusicEngineNoteStruct("", "");
        }




        public static FixedMusicEngineNoteStruct GetDatasByNote(string Note)
        {
            int arraySize = fixedMusicEngineNoteArrayNesA.Count;

            for (int i = 0; i < arraySize; i++)
            {
                if (GetMusicEngineNotesDatas.fixedMusicEngineNoteArrayNesA[i]._Name == Note)
                {
                    return GetMusicEngineNotesDatas.fixedMusicEngineNoteArrayNesA[i];
                }
            }
            return new FixedMusicEngineNoteStruct("", "");
        }


        public static FixedMusicEngineNoteStruct GetDatasByNoiseNote(string Note)
        {
            int arraySize = fixedMusicEngineNoiseNotesArrayNesA.Count;

            for (int i = 0; i < arraySize; i++)
            {
                if (GetMusicEngineNotesDatas.fixedMusicEngineNoiseNotesArrayNesA[i]._Name == Note)
                {
                    return GetMusicEngineNotesDatas.fixedMusicEngineNoiseNotesArrayNesA[i];
                }
            }
            return new FixedMusicEngineNoteStruct("", "");
        }
    }
}