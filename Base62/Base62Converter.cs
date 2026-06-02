using System;
using System.Text;

namespace Base62
{
    /// <summary>
    /// Encodes and decodes text to and from base62 encoding.
    /// </summary>
    public class Base62Converter
    {
        private const string DEFAULT_CHARACTER_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string INVERTED_CHARACTER_SET = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const byte InvalidCharacterValue = byte.MaxValue;
        private readonly string characterSet;
        private readonly byte[] decodeMap;

        /// <summary>
        /// Initializes a new instance of <see cref="Base62Converter"/>.
        /// </summary>
        public Base62Converter()
        {
            characterSet = DEFAULT_CHARACTER_SET;
            decodeMap = CreateDecodeMap(characterSet);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Base62Converter"/> with the provided <see cref="CharacterSet"/>.
        /// </summary>
        /// <param name="charset"></param>
        public Base62Converter(CharacterSet charset)
        {
            if (charset == CharacterSet.DEFAULT)
                characterSet = DEFAULT_CHARACTER_SET;
            else
                characterSet = INVERTED_CHARACTER_SET;

            decodeMap = CreateDecodeMap(characterSet);
        }

        /// <summary>
        /// Encodes the input text to Base62 format.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>Encoded base62 value.</returns>
        public string Encode(string value)
        {
            var arr = Encoding.UTF8.GetBytes(value);
            var converted = Encode(arr);
            var output = new char[converted.Length];
            for (var i = 0; i < converted.Length; i++)
            {
                output[i] = characterSet[converted[i]];
            }
            return new string(output);
        }

        /// <summary>
        /// Decodes the input text from Base62 format.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The decoded value.</returns>
        public string Decode(string value)
        {
            var arr = new byte[value.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                var c = value[i];
                arr[i] = c < decodeMap.Length ? decodeMap[c] : InvalidCharacterValue;
            }

            var converted = Decode(arr);
            return Encoding.UTF8.GetString(converted, 0, converted.Length);
        }

        /// <summary>
        /// Encodes the input bytes to Base62 format.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>Encoded base62 value.</returns>
        public byte[] Encode(byte[] value)
        {
            return BaseConvert(value, 256, 62);
        }

        /// <summary>
        /// Decodes the input bytes from Base62 format.
        /// </summary>
        /// <param name="value">The inpnut value.</param>
        /// <returns>The decoded value.</returns>
        public byte[] Decode(byte[] value)
        {
            return BaseConvert(value, 62, 256);
        }

        /// <summary>
        /// Converts source byte array from the source base to the destination base.
        /// </summary>
        /// <param name="source">Byte array to convert.</param>
        /// <param name="sourceBase">Source base to convert from.</param>
        /// <param name="targetBase">Target base to convert to.</param>
        /// <returns>Converted byte array.</returns>
        private static byte[] BaseConvert(byte[] source, int sourceBase, int targetBase)
        {
            if (targetBase < 2 || targetBase > 256)
                throw new ArgumentOutOfRangeException(nameof(targetBase), targetBase, "Value must be between 2 & 256 (inclusive)");
            
            if (sourceBase < 2 || sourceBase > 256)
                throw new ArgumentOutOfRangeException(nameof(sourceBase), sourceBase, "Value must be between 2 & 256 (inclusive)");

            if (source.Length == 0)
                return Array.Empty<byte>();

            var sourceBuffer = new byte[source.Length];
            Buffer.BlockCopy(source, 0, sourceBuffer, 0, source.Length);

            var leadingZeroCount = 0;
            while (leadingZeroCount < sourceBuffer.Length && sourceBuffer[leadingZeroCount] == 0)
            {
                leadingZeroCount++;
            }

            if (leadingZeroCount == sourceBuffer.Length)
                return new byte[sourceBuffer.Length];

            var maxResultDigitCount = sourceBase >= targetBase
                ? (int)Math.Ceiling(source.Length * Math.Log(sourceBase) / Math.Log(targetBase)) + 1
                : source.Length + 1;

            var quotientBuffer = new byte[source.Length];
            var resultDigits = new byte[maxResultDigitCount];
            var resultDigitCount = 0;
            var sourceLength = sourceBuffer.Length;
            var sourceStartOffset = leadingZeroCount;

            while (sourceLength > 0)
            {
                int remainder = 0;
                var quotientLength = 0;
                for (var i = sourceStartOffset; i < sourceLength; i++)
                {
                    var accumulator = sourceBuffer[i] + remainder * sourceBase;
                    var digit = (byte)(accumulator / targetBase);
                    remainder = accumulator % targetBase;
                    if (quotientLength > 0 || digit != 0)
                    {
                        quotientBuffer[quotientLength++] = digit;
                    }
                }

                resultDigits[resultDigitCount++] = (byte)remainder;
                if (quotientLength == 0)
                    break;

                (sourceBuffer, quotientBuffer) = (quotientBuffer, sourceBuffer);
                sourceLength = quotientLength;
                sourceStartOffset = 0;
            }

            var output = new byte[leadingZeroCount + resultDigitCount];
            for (var i = 0; i < resultDigitCount; i++)
            {
                output[leadingZeroCount + i] = resultDigits[resultDigitCount - 1 - i];
            }

            return output;
        }

        private static byte[] CreateDecodeMap(string characterSet)
        {
            var map = new byte[128];
            for (var i = 0; i < map.Length; i++)
            {
                map[i] = InvalidCharacterValue;
            }

            for (byte i = 0; i < characterSet.Length; i++)
            {
                map[characterSet[i]] = i;
            }

            return map;
        }

        /// <summary>
        /// Character set to use for encoding/decoding.
        /// </summary>
        public enum CharacterSet
        {
            /// <summary>
            /// Alpha numeric character set, using capital letters first before lowercase.
            /// </summary>
            DEFAULT,

            /// <summary>
            /// Alpha numeric character set, using lower case letters first before uppercase.
            /// </summary>
            INVERTED
        }
    }
}
