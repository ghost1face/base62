using System.Collections.Generic;
using System.Text;

namespace Base62
{
    public class Base62Converter
    {
        private const string DEFAULT_CHARACTER_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string INVERTED_CHARACTER_SET = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly string characterSet;

        public Base62Converter()
        {
            characterSet = DEFAULT_CHARACTER_SET;
        }

        public Base62Converter(CharacterSet charset)
        {
            if (charset == CharacterSet.DEFAULT)
                characterSet = DEFAULT_CHARACTER_SET;
            else
                characterSet = INVERTED_CHARACTER_SET;
        }

        public string Encode(string value)
        {
            var arr = Encoding.UTF8.GetBytes(value);

            return Encode(arr);
        }

        public string Decode(string value)
        {
            var arr = new byte[value.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = (byte)characterSet.IndexOf(value[i]);
            }

            return Decode(arr);
        }

        private string Encode(byte[] value)
        {
            var converted = BaseConvert(value, 256, 62);
            var builder = new StringBuilder();
            for (var i = 0; i < converted.Length; i++)
            {
                builder.Append(characterSet[converted[i]]);
            }
            return builder.ToString();
        }

        private string Decode(byte[] value)
        {
            var converted = BaseConvert(value, 62, 256);

            return Encoding.UTF8.GetString(converted, 0, converted.Length);
        }

        private static byte[] BaseConvert(byte[] source, int sourceBase, int targetBase)
        {
            var result = new List<int>();
            int count = 0;
            while ((count = source.Length) > 0)
            {
                var quotient = new List<byte>();
                int remainder = 0;
                for (var i = 0; i != count; i++)
                {
                    int accumulator = source[i] + remainder * sourceBase;
                    byte digit = System.Convert.ToByte((accumulator - (accumulator % targetBase)) / targetBase);
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

        public enum CharacterSet
        {
            DEFAULT,
            INVERTED
        }
    }
}
