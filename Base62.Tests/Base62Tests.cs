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

        [Fact]
        public void FirstZeroBytesAreConvertedCorrectly()
        {
            var sourceBytes = new byte[] { 0, 0, 1, 2, 0, 0 };
            var converter = new Base62Converter(Base62Converter.CharacterSet.DEFAULT);
            var encoded = converter.Encode(sourceBytes);
            var decoded = converter.Decode(encoded);
            
            Assert.Equal(sourceBytes, decoded);
        }

        [Fact]
        public void EmptyString_EncodesAndDecodesToEmpty()
        {
            var converter = new Base62Converter();

            var encoded = converter.Encode(string.Empty);
            var decoded = converter.Decode(encoded);

            Assert.Equal(string.Empty, encoded);
            Assert.Equal(string.Empty, decoded);
        }

        [Fact]
        public void EmptyByteArray_EncodesAndDecodesToEmpty()
        {
            var converter = new Base62Converter();
            var empty = Array.Empty<byte>();

            var encoded = converter.Encode(empty);
            var decoded = converter.Decode(encoded);

            Assert.Empty(encoded);
            Assert.Empty(decoded);
        }

        [Fact]
        public void AllZeroBytes_ArePreservedOnRoundTrip()
        {
            var sourceBytes = new byte[] { 0, 0, 0 };
            var converter = new Base62Converter();
            var encodedBytes = converter.Encode(sourceBytes);
            var decoded = converter.Decode(encodedBytes);

            Assert.Equal(sourceBytes, decoded);
            Assert.Equal("000", converter.Encode("\0\0\0"));
        }

        [Fact]
        public void StringWithLeadingNullBytes_RoundTrips()
        {
            var input = "\0\0\0";
            var converter = new Base62Converter();
            var encoded = converter.Encode(input);
            var decoded = converter.Decode(encoded);

            Assert.Equal(input, decoded);
            Assert.StartsWith("0", encoded);
        }

        [Fact]
        public void SingleZeroByte_RoundTrips()
        {
            var sourceBytes = new byte[] { 0 };
            var converter = new Base62Converter();

            Assert.Equal(sourceBytes, converter.Decode(converter.Encode(sourceBytes)));
        }

        [Fact]
        public void Decode_WithCharacterOutsideCharset_DoesNotThrow()
        {
            var converter = new Base62Converter();
            var validEncoded = converter.Encode("120");

            var decoded = converter.Decode(validEncoded + "@");

            Assert.NotEqual("120", decoded);
        }

        [Fact]
        public void Decode_WithCharacterAboveAsciiRange_DoesNotThrow()
        {
            var converter = new Base62Converter();
            var validEncoded = converter.Encode("120");

            var decoded = converter.Decode(validEncoded + "\u0080");

            Assert.NotEqual("120", decoded);
        }

        [Fact]
        public void InvertedCharacterSet_AllZeroBytes_RoundTrip()
        {
            var sourceBytes = new byte[] { 0, 0 };
            var converter = new Base62Converter(Base62Converter.CharacterSet.INVERTED);

            Assert.Equal(sourceBytes, converter.Decode(converter.Encode(sourceBytes)));
        }

        [Fact]
        public void DefaultConstructor_MatchesExplicitDefaultCharacterSet()
        {
            var input = "120";
            var implicitConverter = new Base62Converter();
            var explicitConverter = new Base62Converter(Base62Converter.CharacterSet.DEFAULT);

            Assert.Equal(explicitConverter.Encode(input), implicitConverter.Encode(input));
            Assert.Equal(explicitConverter.Decode("DWjo"), implicitConverter.Decode("DWjo"));
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
