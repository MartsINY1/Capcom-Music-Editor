using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mega_Music_Editor.Reusable;
using Mega_Music_Editor.Unique;
using Mega_Music_Editor.Unique.MusicEngineFixedInstructionsDatas;
using Mega_Music_Editor.Unique.MusicSheetsInstructionsDatas;

namespace Mega_Music_Editor.Unique
{
    public enum GameType
    {
        NesA = 0,   // Mega Man 3-4-5-6
        SnesA = 1   // Mega Man X 3
    }

    public static class GameTypeFunction
    {
        public static GameType ReturnGameTypeTypeFromString(string gameType)
        {
            if (gameType == "NesA") return GameType.NesA;
            if (gameType == "SnesA") return GameType.SnesA;

            return GameType.NesA;
        }
    }

    public struct FixedMusicEngineDataStruct
    {
        public FixedMusicEngineDataStruct(string Hexcode, string Name, int QuantityParameters, int ColumnPosition, string ColumnTitle, string ColumnWidth, string ColumnDefaultValue, PageName Page)
        {
            _HexCode = Hexcode;
            _Name = Name;
            _QuantityParameters = QuantityParameters;
            _ColumnPosition = ColumnPosition;
            _ColumnTitle = ColumnTitle;
            _Page = Page;
            _ColumnWidth = ColumnWidth;
            _ColumnDefaultValue = ColumnDefaultValue;
        }

        public string _HexCode;
        public string _Name;
        public int _QuantityParameters;
        public int _ColumnPosition;
        public string _ColumnTitle;
        public string _ColumnWidth;
        public string _ColumnDefaultValue;
        public PageName _Page;
    }

    public static class GetMusicEngineInstructionsDatas
    {
        public static readonly List<FixedMusicEngineDataStruct> fixedMusicEngineDataArrayNesA = new List<FixedMusicEngineDataStruct>
        {
            new FixedMusicEngineDataStruct(HexCodes.NesA.Triplet, Names.Triplet, QuantityParameters.NesA.Triplet, ColumnsPosition.NesA.Triplet, ColumnsTitles.NesA.Triplet, ColumnsWidth.NesA.Triplet, ColumnsEmptyValue.NesA.Triplet, Page.NesA.Triplet),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Connect, Names.Connect, QuantityParameters.NesA.Connect, ColumnsPosition.NesA.Connect, ColumnsTitles.NesA.Connect, ColumnsWidth.NesA.Connect, ColumnsEmptyValue.NesA.Connect, Page.NesA.Connect),
            new FixedMusicEngineDataStruct(HexCodes.NesA.PlusHalf, Names.PlusHalf, QuantityParameters.NesA.PlusHalf, ColumnsPosition.NesA.PlusHalf, ColumnsTitles.NesA.PlusHalf, ColumnsWidth.NesA.PlusHalf, ColumnsEmptyValue.NesA.PlusHalf, Page.NesA.PlusHalf),
            new FixedMusicEngineDataStruct(HexCodes.NesA.OctavePlus, Names.OctavePlus, QuantityParameters.NesA.OctavePlus, ColumnsPosition.NesA.OctavePlus, ColumnsTitles.NesA.OctavePlus, ColumnsWidth.NesA.OctavePlus, ColumnsEmptyValue.NesA.OctavePlus, Page.NesA.OctavePlus),
            new FixedMusicEngineDataStruct(HexCodes.NesA.SetFlags, Names.Flags, QuantityParameters.NesA.SetFlags, ColumnsPosition.NesA.SetFlags, ColumnsTitles.NesA.SetFlags, ColumnsWidth.NesA.SetFlags, ColumnsEmptyValue.NesA.SetFlags, Page.NesA.SetFlags),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Speed, Names.Speed, QuantityParameters.NesA.Speed, ColumnsPosition.NesA.Speed, ColumnsTitles.NesA.Speed, ColumnsWidth.NesA.Speed, ColumnsEmptyValue.NesA.Speed, Page.NesA.Speed),
            new FixedMusicEngineDataStruct(HexCodes.NesA.ToneLenght, Names.ToneLenght, QuantityParameters.NesA.ToneLenght, ColumnsPosition.NesA.ToneLenght, ColumnsTitles.NesA.ToneLenght, ColumnsWidth.NesA.ToneLenght, ColumnsEmptyValue.NesA.ToneLenght, Page.NesA.ToneLenght),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Volume, Names.Volume, QuantityParameters.NesA.Volume, ColumnsPosition.NesA.Volume, ColumnsTitles.NesA.Volume, ColumnsWidth.NesA.Volume, ColumnsEmptyValue.NesA.Volume, Page.NesA.Volume),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Instrument, Names.Instrument, QuantityParameters.NesA.Instrument, ColumnsPosition.NesA.Instrument, ColumnsTitles.NesA.Instrument, ColumnsWidth.NesA.Instrument, ColumnsEmptyValue.NesA.Instrument, Page.NesA.Instrument),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Octave, Names.Octave, QuantityParameters.NesA.Octave, ColumnsPosition.NesA.Octave, ColumnsTitles.NesA.Octave, ColumnsWidth.NesA.Octave, ColumnsEmptyValue.NesA.Octave, Page.NesA.Octave),
            new FixedMusicEngineDataStruct(HexCodes.NesA.GlobalTranspose, Names.GlobalTranspose, QuantityParameters.NesA.GlobalTranspose, ColumnsPosition.NesA.GlobalTranspose, ColumnsTitles.NesA.GlobalTranspose, ColumnsWidth.NesA.GlobalTranspose, ColumnsEmptyValue.NesA.GlobalTranspose, Page.NesA.GlobalTranspose),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Transpose, Names.Transpose, QuantityParameters.NesA.Transpose, ColumnsPosition.NesA.Transpose, ColumnsTitles.NesA.Transpose, ColumnsWidth.NesA.Transpose, ColumnsEmptyValue.NesA.Transpose, Page.NesA.Transpose),
            new FixedMusicEngineDataStruct(HexCodes.NesA.TunePitch, Names.TunePitch, QuantityParameters.NesA.TunePitch, ColumnsPosition.NesA.TunePitch, ColumnsTitles.NesA.TunePitch, ColumnsWidth.NesA.TunePitch, ColumnsEmptyValue.NesA.TunePitch, Page.NesA.TunePitch),
            new FixedMusicEngineDataStruct(HexCodes.NesA.PitchSlide, Names.PitchSlide, QuantityParameters.NesA.PitchSlide, ColumnsPosition.NesA.PitchSlide, ColumnsTitles.NesA.PitchSlide, ColumnsWidth.NesA.PitchSlide, ColumnsEmptyValue.NesA.PitchSlide, Page.NesA.PitchSlide),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Loop1, Names.Loop1, QuantityParameters.NesA.Loop1, ColumnsPosition.NesA.Loop1, ColumnsTitles.NesA.Loop1, ColumnsWidth.NesA.Loop1, ColumnsEmptyValue.NesA.Loop1, Page.NesA.Loop1),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Loop2, Names.Loop2, QuantityParameters.NesA.Loop2, ColumnsPosition.NesA.Loop2, ColumnsTitles.NesA.Loop2, ColumnsWidth.NesA.Loop2, ColumnsEmptyValue.NesA.Loop2, Page.NesA.Loop2),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Loop3, Names.Loop3, QuantityParameters.NesA.Loop3, ColumnsPosition.NesA.Loop3, ColumnsTitles.NesA.Loop3, ColumnsWidth.NesA.Loop3, ColumnsEmptyValue.NesA.Loop3, Page.NesA.Loop3),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Loop4, Names.Loop4, QuantityParameters.NesA.Loop4, ColumnsPosition.NesA.Loop4, ColumnsTitles.NesA.Loop4, ColumnsWidth.NesA.Loop4, ColumnsEmptyValue.NesA.Loop4, Page.NesA.Loop4),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Break1, Names.Break1, QuantityParameters.NesA.Break1, ColumnsPosition.NesA.Break1, ColumnsTitles.NesA.Break1, ColumnsWidth.NesA.Break1, ColumnsEmptyValue.NesA.Break1, Page.NesA.Break1),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Break2, Names.Break2, QuantityParameters.NesA.Break2, ColumnsPosition.NesA.Break2, ColumnsTitles.NesA.Break2, ColumnsWidth.NesA.Break2, ColumnsEmptyValue.NesA.Break2, Page.NesA.Break2),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Break3, Names.Break3, QuantityParameters.NesA.Break3, ColumnsPosition.NesA.Break3, ColumnsTitles.NesA.Break3, ColumnsWidth.NesA.Break3, ColumnsEmptyValue.NesA.Break3, Page.NesA.Break3),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Break4, Names.Break4, QuantityParameters.NesA.Break4, ColumnsPosition.NesA.Break4, ColumnsTitles.NesA.Break4, ColumnsWidth.NesA.Break4, ColumnsEmptyValue.NesA.Break4, Page.NesA.Break4),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Jump, Names.Jump, QuantityParameters.NesA.Jump, ColumnsPosition.NesA.Jump, ColumnsTitles.NesA.Jump, ColumnsWidth.NesA.Jump, ColumnsEmptyValue.NesA.Jump, Page.NesA.Jump),
            new FixedMusicEngineDataStruct(HexCodes.NesA.End, Names.End, QuantityParameters.NesA.End, ColumnsPosition.NesA.End, ColumnsTitles.NesA.End, ColumnsWidth.NesA.End, ColumnsEmptyValue.NesA.End, Page.NesA.End),
            new FixedMusicEngineDataStruct(HexCodes.NesA.ToneType, Names.ToneType, QuantityParameters.NesA.ToneType, ColumnsPosition.NesA.ToneType, ColumnsTitles.NesA.ToneType, ColumnsWidth.NesA.ToneType, ColumnsEmptyValue.NesA.ToneType, Page.NesA.ToneType),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Note, Names.Note, QuantityParameters.NesA.Note, ColumnsPosition.NesA.Note, ColumnsTitles.NesA.Note, ColumnsWidth.NesA.Note, ColumnsEmptyValue.NesA.Note, Page.NesA.Note),

            // Snes exclusives
            new FixedMusicEngineDataStruct(HexCodes.NesA.Panning, Names.Panning, QuantityParameters.NesA.Panning, ColumnsPosition.NesA.Panning, ColumnsTitles.NesA.Panning, ColumnsWidth.NesA.Panning, ColumnsEmptyValue.NesA.Panning, Page.NesA.Panning),
            new FixedMusicEngineDataStruct(HexCodes.NesA.VibratoDepth, Names.VibratoDepth, QuantityParameters.NesA.VibratoDepth, ColumnsPosition.NesA.VibratoDepth, ColumnsTitles.NesA.VibratoDepth, ColumnsWidth.NesA.VibratoDepth, ColumnsEmptyValue.NesA.VibratoDepth, Page.NesA.VibratoDepth),
            new FixedMusicEngineDataStruct(HexCodes.NesA.TremoloVolume, Names.TremoloVolume, QuantityParameters.NesA.TremoloVolume, ColumnsPosition.NesA.TremoloVolume, ColumnsTitles.NesA.TremoloVolume, ColumnsWidth.NesA.TremoloVolume, ColumnsEmptyValue.NesA.TremoloVolume, Page.NesA.TremoloVolume),
            new FixedMusicEngineDataStruct(HexCodes.NesA.VibratoOrTremolFrequency, Names.VibratoOrTremolFrequency, QuantityParameters.NesA.VibratoOrTremolFrequency, ColumnsPosition.NesA.VibratoOrTremolFrequency, ColumnsTitles.NesA.VibratoOrTremolFrequency, ColumnsWidth.NesA.VibratoOrTremolFrequency, ColumnsEmptyValue.NesA.VibratoOrTremolFrequency, Page.NesA.VibratoOrTremolFrequency),
            new FixedMusicEngineDataStruct(HexCodes.NesA.ins1A03, Names.ins1A03, QuantityParameters.NesA.ins1A03, ColumnsPosition.NesA.ins1A03, ColumnsTitles.NesA.ins1A03, ColumnsWidth.NesA.ins1A03, ColumnsEmptyValue.NesA.ins1A03, Page.NesA.ins1A03),
            new FixedMusicEngineDataStruct(HexCodes.NesA.PitchSlideUnk, Names.PitchSlideUnk, QuantityParameters.NesA.PitchSlideUnk, ColumnsPosition.NesA.PitchSlideUnk, ColumnsTitles.NesA.PitchSlideUnk, ColumnsWidth.NesA.PitchSlideUnk, ColumnsEmptyValue.NesA.PitchSlideUnk, Page.NesA.PitchSlideUnk),
            new FixedMusicEngineDataStruct(HexCodes.NesA.SfxToggle, Names.SfxToggle, QuantityParameters.NesA.SfxToggle, ColumnsPosition.NesA.SfxToggle, ColumnsTitles.NesA.SfxToggle, ColumnsWidth.NesA.SfxToggle, ColumnsEmptyValue.NesA.SfxToggle, Page.NesA.SfxToggle),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Sfx, Names.Sfx, QuantityParameters.NesA.Sfx, ColumnsPosition.NesA.Sfx, ColumnsTitles.NesA.Sfx, ColumnsWidth.NesA.Sfx, ColumnsEmptyValue.NesA.Sfx, Page.NesA.Sfx),
            new FixedMusicEngineDataStruct(HexCodes.NesA.ToneLenght2, Names.ToneLenght2, QuantityParameters.NesA.ToneLenght2, ColumnsPosition.NesA.ToneLenght2, ColumnsTitles.NesA.ToneLenght2, ColumnsWidth.NesA.ToneLenght2, ColumnsEmptyValue.NesA.ToneLenght2, Page.NesA.ToneLenght2),
            new FixedMusicEngineDataStruct(HexCodes.NesA.PannMovementSpeed, Names.PannMovementSpeed, QuantityParameters.NesA.PannMovementSpeed, ColumnsPosition.NesA.PannMovementSpeed, ColumnsTitles.NesA.PannMovementSpeed, ColumnsWidth.NesA.PannMovementSpeed, ColumnsEmptyValue.NesA.PannMovementSpeed, Page.NesA.PannMovementSpeed),
            new FixedMusicEngineDataStruct(HexCodes.NesA.Unknown1F, Names.Unknown1F, QuantityParameters.NesA.Unknown1F, ColumnsPosition.NesA.Unknown1F, ColumnsTitles.NesA.Unknown1F, ColumnsWidth.NesA.Unknown1F, ColumnsEmptyValue.NesA.Unknown1F, Page.NesA.Unknown1F),
            new FixedMusicEngineDataStruct(HexCodes.NesA.GlobalVolume, Names.GlobalVolume, QuantityParameters.NesA.GlobalVolume, ColumnsPosition.NesA.GlobalVolume, ColumnsTitles.NesA.GlobalVolume, ColumnsWidth.NesA.GlobalVolume, ColumnsEmptyValue.NesA.GlobalVolume, Page.NesA.GlobalVolume)
        };

        public static FixedMusicEngineDataStruct GetDatasByColumnPositionAndPage(int columnPosition, PageName pageName)
        {
            int arraySize = fixedMusicEngineDataArrayNesA.Count;

            for (int i = 0; i < arraySize; i++)
            {
                if (GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[i]._ColumnPosition == columnPosition && GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[i]._Page == pageName)
                {
                    return GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[i];
                }
            }
            return new FixedMusicEngineDataStruct("", "", 0, 0, "", "20", ColumnsEmptyValue.symbolRequiredForEmptyCells, PageName.NoPage);
        }

        public static FixedMusicEngineDataStruct GetDatasByHexCodes(string HexCode)
        {
            int arraySize = fixedMusicEngineDataArrayNesA.Count;

            for (int i = 0; i < arraySize; i++)
            {
                if (GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[i]._HexCode == HexCode)
                {
                    return GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[i];
                }
            }
            
            return new FixedMusicEngineDataStruct("", "", 0, 0, "", "20", ColumnsEmptyValue.symbolRequiredForEmptyCells, PageName.NoPage);
        }

        public static FixedMusicEngineDataStruct GetDatasByInstructionName(string instructionName)
        {
            int arraySize = fixedMusicEngineDataArrayNesA.Count;

            for (int i = 0; i < arraySize; i++)
            {
                if (GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[i]._Name == instructionName)
                {
                    return GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[i];
                }
            }
            return new FixedMusicEngineDataStruct("", "", 0, 0, "", "20", ColumnsEmptyValue.symbolRequiredForEmptyCells, PageName.NoPage);
        }

        public static int GetPriorityByHexCode(string HexCode)
        {
            FixedMusicEngineDataStruct data = GetDatasByHexCodes(HexCode);

            return (data._ColumnPosition + ((int)(data._Page) * 100));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HexCode"></param>
        /// <returns>Returns true if instruction order is higher than priority received</returns>
        public static bool CheckIfCurrentInstructionPriorityGreaterThan_ByHexCode(string HexCode, int priority)
        {
            FixedMusicEngineDataStruct data = GetDatasByHexCodes(HexCode);

            return (GetPriorityByHexCode(HexCode) > priority);
        }
    }
}