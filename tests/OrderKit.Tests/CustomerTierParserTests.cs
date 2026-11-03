using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class CustomerTierParserTests
    {
        [TestCase("silver", "silver")]
        [TestCase(" Gold ", "gold")]
        [TestCase("PLATINUM", "platinum")]
        [TestCase("standard", "standard")]
        [TestCase("vip", "standard")]
        [TestCase("   ", "standard")]
        public void KnownTiersAreNormalised(string raw, string expected)
        {
            Assert.AreEqual(expected, CustomerTierParser.Parse(raw));
        }

        [Test]
        public void NullParsesAsStandard()
        {
            Assert.AreEqual("standard", CustomerTierParser.Parse(null));
        }

        [TestCase("silver", 2)]
        [TestCase("gold", 5)]
        [TestCase("platinum", 10)]
        [TestCase("standard", 0)]
        [TestCase("unknown", 0)]
        public void DiscountPercentByTier(string tier, int percent)
        {
            Assert.AreEqual(percent, CustomerTierParser.DiscountPercent(tier));
        }
    }
}
