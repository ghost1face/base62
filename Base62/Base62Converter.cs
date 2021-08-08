using System.Collections.Generic;
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
        private readonly string characterSet;

        /// <summary>
        /// Initializes a new instance of <see cref="Base62Converter"/>.
        /// </summary>
        public Base62Converter()
        {
            characterSet = DEFAULT_CHARACTER_SET;
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
            var builder = new StringBuilder();
            foreach (var c in converted)
            {
                builder.Append(characterSet[c]);
            }
            return builder.ToString();
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
                arr[i] = (byte)characterSet.IndexOf(value[i]);
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

        private static byte[] BaseConvert(byte[] source, int sourceBase, int targetBase)
        {
            var result = new List<int>();
            int count;
            while ((count = source.Length) > 0)
            {
                var quotient = new List<byte>();
                int remainder = 0;
                for (var i = 0; i != count; i++)
                {
                    int accumulator = source[i] + remainder * sourceBase;
                    byte digit = (byte)((accumulator - (accumulator % targetBase)) / targetBase);
                    remainder = accumulator % targetBase;
                    if (quotient.Count > 0 || digit != 0)
                    {
                        quotient.Add(digit);
                    }
                }

                result.Insert(0, remainder);
                source = quotient.ToArray();
            }

            var output = new byte[result.Count];
            for (int i = 0; i < result.Count; i++)
                output[i] = (byte)result[i];

            return output;
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
