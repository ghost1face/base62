using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Base62.Tests
{
    [TestClass]
    public class Base62Tests
    {
        [TestMethod]
        public void Encoded_CanBe_Decoded()
        {
            var input = "120";
            var converter = new Base62Converter();
            var encoded = converter.Encode(input);

            var decoded = converter.Decode(encoded);

            Assert.AreEqual(input, decoded);
        }

        [TestMethod]
        public void Encoded_Inverted_CanBe_Decoded()
        {
            var input = "Whatup";
            var converter = new Base62Converter(Base62Converter.CharacterSet.INVERTED);
            var encoded = converter.Encode(input);

            var decoded = converter.Decode(encoded);

            Assert.AreEqual(input, decoded);
        }

        [TestMethod]
        public void NonAscii_CanBe_Decoded()
        {
            var input = "love爱";
            var converter = new Base62Converter(Base62Converter.CharacterSet.DEFAULT);
            var encoded = converter.Encode(input);

            var decoded = converter.Decode(encoded);

            Assert.AreEqual(input, decoded);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
        public void ASCII_AND_UTF8_Can_RoundTrip(string input, string expected)
        {
            var converter = new Base62Converter(Base62Converter.CharacterSet.DEFAULT);
            var encoded = converter.Encode(input);
            var decoded = converter.Decode(encoded);

            Assert.AreEqual(expected, encoded);
            Assert.AreEqual(input, decoded);
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
