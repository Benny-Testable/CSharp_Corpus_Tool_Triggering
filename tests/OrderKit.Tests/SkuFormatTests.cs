using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class SkuFormatTests
    {
        [TestCase("ABC-1234")]
        [TestCase("ZZZ-0000")]
        public void WellFormedSkusAreValid(string sku)
        {
            Assert.IsTrue(SkuFormat.IsValid(sku));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("abc-1234")]
        [TestCase("ABC-123")]
        [TestCase("AB-1234")]
        [TestCase("ABC1234")]
        public void MalformedSkusAreInvalid(string sku)
        {
            Assert.IsFalse(SkuFormat.IsValid(sku));
        }

        [Test]
        public void NormalizeCleansUpRawInput()
        {
            Assert.AreEqual("ABC-1234", SkuFormat.Normalize("  abc 1234 "));
            Assert.AreEqual("ABC-1234", SkuFormat.Normalize("abc-1234"));
            Assert.AreEqual(string.Empty, SkuFormat.Normalize(null));
            Assert.AreEqual(string.Empty, SkuFormat.Normalize("   "));
        }
    }
}
