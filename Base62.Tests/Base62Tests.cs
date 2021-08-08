using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Base62.Tests
{
    public class Base62Tests
    {
        [Fact]
        public void Encoded_CanBe_Decoded()
        {
            var input = "120";
            var converter = new Base62Converter();
            var encoded = converter.Encode(input);

            var decoded = converter.Decode(encoded);

            Assert.Equal(input, decoded);
        }

        [Fact]
        public void Encoded_Inverted_CanBe_Decoded()
        {
            var input = "Whatup";
            var converter = new Base62Converter(Base62Converter.CharacterSet.INVERTED);
            var encoded = converter.Encode(input);

            var decoded = converter.Decode(encoded);

            Assert.Equal(input, decoded);
        }

        [Fact]
        public void NonAscii_CanBe_Decoded()
        {
            var input = "love爱";
            var converter = new Base62Converter(Base62Converter.CharacterSet.DEFAULT);
            var encoded = converter.Encode(input);

            var decoded = converter.Decode(encoded);

            Assert.Equal(input, decoded);
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void ASCII_AND_UTF8_Can_RoundTrip(string input, string expected)
        {
            var converter = new Base62Converter(Base62Converter.CharacterSet.DEFAULT);
            var encoded = converter.Encode(input);
            var decoded = converter.Decode(encoded);

            Assert.Equal(expected, encoded);
            Assert.Equal(input, decoded);
        }

        public static IEnumerable<object[]> GetData()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "validation_data.txt");
            using (var fileReader = new StreamReader(filePath))
            {
                string row = null;
                while ((row = fileReader.ReadLine()) != null)
                {
                    yield return row.Split('\t');
                }
            }
        }
    }
}
