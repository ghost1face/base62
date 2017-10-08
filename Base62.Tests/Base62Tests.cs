using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
