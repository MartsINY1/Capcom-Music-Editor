using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Unique.MusicEngineFixedNotesDatas
{
    public static class Lenghts
    {
        public class NoteLenght
        {
            private readonly int _LineQty;
            private readonly int _LineQtyPlusHalf;
            private readonly string _HexCode;

            public NoteLenght(int LineQty, int LineQtyPlusHalf, string HexCode)
            {
                _LineQty = LineQty;
                _LineQtyPlusHalf = LineQtyPlusHalf;
                _HexCode = HexCode;
            }
            public int GetLineQty() { return _LineQty; }
            public int GetLineQtyPlusHalf() { return _LineQtyPlusHalf; }
            public string GetHexCode() { return _HexCode; }
        }


        public static class NesA
        {
            public static readonly List<NoteLenght> noteLenghtList = new List<NoteLenght>
            {
                new NoteLenght(1, 1, "20"),
                new NoteLenght(2, 3, "40"),
                new NoteLenght(4, 6, "60"),
                new NoteLenght(8, 12, "80"),
                new NoteLenght(16, 24, "A0"),
                new NoteLenght(32, 48, "C0"),
                new NoteLenght(64, 64, "E0")
            };

            public static int GetNoteLineQtyByHexCode(string noteLenght)
            {
                for (int i = 0; i < noteLenghtList.Count; i++)
                {
                    if (noteLenghtList[i].GetHexCode() == noteLenght)
                    {
                        return noteLenghtList[i].GetLineQty();
                    }
                }
                return 1;
            }
            
            public static NoteLenght GetNoteLenghtDatasOfNoteWithClosestQtyOfLine(int lineQty, out bool isPlusHalf)
            {
                NoteLenght noteLenghtToReturn = null;
                int biggestLineAmount = 0;

                isPlusHalf = false;

                // Pick desired note (not considering plus half value)
                for (int i = 0; i < noteLenghtList.Count; i++)
                {
                    if (noteLenghtList[i].GetLineQty() <= lineQty)
                    {
                        noteLenghtToReturn = noteLenghtList[i];
                        biggestLineAmount = noteLenghtList[i].GetLineQty();
                    }
                }

                // Now loops through all line amount with plus half, and only return this value if it's bigger than biggest one found so far
                // (this is done because whole note lenght is the same with/without a plus half
                for (int i = 0; i < noteLenghtList.Count; i++)
                {
                    // If quantity of line of current note is smaller or equal than the desired quantity
                    // and the new amount found is bigger than the one found within the notes not using plushalf
                    if ((noteLenghtList[i].GetLineQtyPlusHalf() <= lineQty) && (noteLenghtList[i].GetLineQtyPlusHalf() > biggestLineAmount))
                    {
                        noteLenghtToReturn = noteLenghtList[i];
                        biggestLineAmount = noteLenghtList[i].GetLineQty();

                        isPlusHalf = true;
                    }
                }

                return noteLenghtToReturn;
            }
        }
    }
}
