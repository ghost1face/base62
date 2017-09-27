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
            var arr = new int[value.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = value[i];
            }

            return Encode(arr);
        }

        public string Decode(string value)
        {
            var arr = new int[value.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = characterSet.IndexOf(value[i]);
            }

            return Decode(arr);
        }

        private string Encode(int[] value)
        {
            var converted = BaseConvert(value, 256, 62);
            var builder = new StringBuilder();
            for (var i = 0; i < converted.Length; i++)
            {
                builder.Append(characterSet[converted[i]]);
            }
            return builder.ToString();
        }

        private string Decode(int[] value)
        {
            var converted = BaseConvert(value, 62, 256);
            var builder = new StringBuilder();
            for (var i = 0; i < converted.Length; i++)
            {
                builder.Append((char)converted[i]);
            }
            return builder.ToString();
        }

        private static int[] BaseConvert(int[] source, int sourceBase, int targetBase)
        {
            var result = new List<int>();
            int count = 0;
            while ((count = source.Length) > 0)
            {
                var quotient = new List<int>();
                int remainder = 0;
                for (var i = 0; i != count; i++)
                {
                    int accumulator = source[i] + remainder * sourceBase;
                    int digit = accumulator / targetBase;
                    remainder = accumulator % targetBase;
                    if (quotient.Count > 0 || digit > 0)
                    {
                        quotient.Add(digit);
                    }
                }

                result.Insert(0, remainder);
                source = quotient.ToArray();
            }

            return result.ToArray();
        }

        public enum CharacterSet
        {
            DEFAULT,
            INVERTED
        }
    }
}
