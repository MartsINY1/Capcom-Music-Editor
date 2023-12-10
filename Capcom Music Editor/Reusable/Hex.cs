using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Music_Editor.Reusable
{
    public class Hex
    {
        /// <summary>
        /// Contains value as a string. Methods of class handle it as hexadecimal.
        /// </summary>
        private string _value = "";

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="value"></param>
        public Hex()
        {
            _value = ConvertIntToHexString(0);
        }


        /// <summary>
        /// Value received is set
        /// </summary>
        /// <param name="value"></param>
        public Hex(int value)
        {
            if (value < 0) { value = 0; }
            _value = ConvertIntToHexString(value);
        }

        /// <summary>
        /// Value received is set if it's hexadecimal
        /// </summary>
        /// <param name="value"></param>
        public Hex(string value)
        {
            _value = "";
            if (IsHex(value))
            {
                // This is to remove 0 at the start if there are
                value = Hex.ConvertIntToHexString((Hex.ConvertHexStringToInt(value)));
                _value = value;
            }
        }

        #region Get Methods
        /// <summary>
        /// Get value in string format
        /// </summary>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        public string GetValueAsString(int numberOfDigits)
        {
            return _value.PadLeft(numberOfDigits, '0');
        }

        /// <summary>
        /// Get value in integer format
        /// </summary>
        /// <returns></returns>
        public int GetValueAsInt()
        {
            return ConvertHexStringToInt(_value);
        }
        #endregion

        #region Validation
        /// <summary>
        /// Return true if is hexadecimal
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        static public bool IsHex(string hex)
        {
            try
            {
                int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Operator +
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Hex operator +(Hex a, Hex b)
        {
            Hex temp = new Hex(ConvertIntToHexString(ConvertHexStringToInt(a._value) + ConvertHexStringToInt(b._value)));
            return temp;
        }

        /// <summary>
        /// Operator -
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Hex operator -(Hex a, Hex b)
        {
            Hex temp = new Hex(ConvertIntToHexString(ConvertHexStringToInt(a._value) - ConvertHexStringToInt(b._value)));
            return temp;
        }

        /// <summary>
        /// Operator *
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Hex operator *(Hex a, Hex b)
        {
            Hex temp = new Hex(ConvertIntToHexString(ConvertHexStringToInt(a._value) * ConvertHexStringToInt(b._value)));
            return temp;
        }

        /// <summary>
        /// Operator ==
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Hex a, Hex b)
        {
            return (ConvertHexStringToInt(a._value) == ConvertHexStringToInt(b._value));
        }


        /// <summary>
        /// Method had to be override to prevent warnings
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public override bool Equals(object b)
        {
            return base.Equals(b);
        }

        /// <summary>
        /// Operator !=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Hex a, Hex b)
        {
            return (ConvertHexStringToInt(a._value) != ConvertHexStringToInt(b._value));
        }

        /// <summary>
        /// Operator <
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <(Hex a, Hex b)
        {
            return (ConvertHexStringToInt(a._value) < ConvertHexStringToInt(b._value));
        }

        /// <summary>
        /// Operator <=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <=(Hex a, Hex b)
        {
            return (ConvertHexStringToInt(a._value) <= ConvertHexStringToInt(b._value));
        }

        /// <summary>
        /// Operator >
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(Hex a, Hex b)
        {
            return (ConvertHexStringToInt(a._value) > ConvertHexStringToInt(b._value));
        }

        /// <summary>
        /// Operator >=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >=(Hex a, Hex b)
        {
            return (ConvertHexStringToInt(a._value) >= ConvertHexStringToInt(b._value));
        }

        /// <summary>
        /// Logical AND
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Hex operator&(Hex a, Hex b)
        {
            Hex toReturn = new Hex();

            byte aByte = ConvertStringHexToByte(a._value);
            byte bByte = ConvertStringHexToByte(b._value);
            aByte &= bByte;
            toReturn._value = ConvertByteToStringHex(aByte);

            return toReturn;
        }

        /// <summary>
        /// Logical OR
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Hex operator |(Hex a, Hex b)
        {
            Hex toReturn = new Hex();

            byte aByte = ConvertStringHexToByte(a._value);
            byte bByte = ConvertStringHexToByte(b._value);
            aByte |= bByte;
            toReturn._value = ConvertByteToStringHex(aByte);

            return toReturn;
        }

        /// <summary>
        /// Need to exist to prevent warning about this
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Return byte from hex string (0 if value is invalid)
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        static public byte ConvertStringHexToByte(string hex)
        {
            try
            {
                return Convert.ToByte(ConvertHexStringToInt(hex));
            }
            catch (Exception)
            {
                return Convert.ToByte(0);
            }
        }
        
        /// <summary>
        /// Return int from hex string (0 if value is invalid)
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        static public int ConvertStringHexToInt(string hex)
        {
            Hex temp;

            try
            {
                temp = new Hex(hex);
                return Convert.ToByte(temp.GetValueAsInt());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Return hex string from byte (0 if value is invalid)
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        static public string ConvertByteToStringHex(byte hex)
        {
            try
            {
                return ConvertIntToHexString(Convert.ToInt32(hex));
            }
            catch (Exception)
            {
                return ConvertIntToHexString(0);
            }
        }

        /// <summary>
        /// Return int from hex string (0 if value is invalid)
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        static public int ConvertHexStringToInt(string hexStr)
        {
            try
            {
                return int.Parse(hexStr, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Return hex string from int (0 if value is invalid)
        /// </summary>
        /// <param name="hexInt"></param>
        /// <returns></returns>
        static public string ConvertIntToHexString(int hexInt)
        {
            try
            {
                return string.Format("{0:X2}", hexInt);
            }
            catch (Exception)
            {
                return "0";
            }
        }

        /// <summary>
        /// Return hex from int (0 if value is invalid)
        /// </summary>
        /// <param name="hexInt"></param>
        /// <returns></returns>
        static public Hex ConvertIntToHex(int hexInt)
        {
            return new Hex(ConvertIntToHexString(hexInt));
        }

        /// <summary>
        /// Return hex from hex string (0 if value is invalid)
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        static public Hex ConvertStringHexToHex(string hex)
        {
            if (IsHex(hex))
            {
                return new Hex(hex);
            }
            else
            {
                return new Hex("0");
            }
        }

        static public List<Hex> ConvertStringHexToListOf2DigitsHexs(string listHextStr)
        {
            List<Hex> listHex = new List<Hex>();

            for (int i = 0; i < listHextStr.Length; i+=2)
            {
                listHex.Add(new Hex(listHextStr.Substring(i, 2)));
            }

            return listHex;
        }
        #endregion
    }
}