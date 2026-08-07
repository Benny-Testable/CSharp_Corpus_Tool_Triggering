using System.Collections.Generic;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class RiskScorerTests
    {
        private readonly RiskScorer _scorer = new RiskScorer();

        private static Order One(string category, int quantity)
        {
            Order o = new Order("R-1", null);
            o.AddLine(new OrderLine("L-1", quantity, 100L, category));
            return o;
        }

        [Test]
        public void NullOrderScoresFlagsOnly()
        {
            Assert.AreEqual(20, _scorer.Score(null, null, new List<string> { "hard-x" }));
            Assert.AreEqual(3, _scorer.Score(null, null, new List<string> { "soft-y" }));
            Assert.AreEqual(23, _scorer.Score(null, null, new List<string> { "hard-a", "soft-b", null, "other" }));
            Assert.AreEqual(0, _scorer.Score(null, null, null));
        }

        [Test]
        public void EmptyOrderScoresZero()
        {
            Assert.AreEqual(0, _scorer.Score(new Order("R-0", null), "sanctioned", null));
        }

        [TestCase("sanctioned", 40)]
        [TestCase("Embargoed", 40)]
        [TestCase("domestic", 15)]
        [TestCase(null, 25)]
        public void LargeHazmatLinesScoreByRegion(string region, int expected)
        {
            Assert.AreEqual(expected, _scorer.Score(One("hazmat", 20), region, null));
        }

        [TestCase(10, 5)]
        [TestCase(11, 15)]
        public void HazmatThresholdIsExclusive(int quantity, int expected)
        {
            Assert.AreEqual(expected, _scorer.Score(One("hazmat", quantity), "domestic", null));
        }

        [TestCase(50, 0)]
        [TestCase(51, 8)]
        [TestCase(1000, 8)]
        [TestCase(1001, 12)]
        public void FragileLinesScoreByQuantity(int quantity, int expected)
        {
            Assert.AreEqual(expected, _scorer.Score(One("fragile", quantity), null, null));
        }

        [Test]
        public void OtherCategoriesAddNothing()
        {
            Assert.AreEqual(0, _scorer.Score(One("general", 5000), "sanctioned", null));
        }

        [Test]
        public void HighRiskStartsAtSixty()
        {
            Order o = One("hazmat", 20);
            Assert.IsTrue(_scorer.IsHighRisk(o, "sanctioned", new List<string> { "hard-x" }));
            Assert.IsFalse(_scorer.IsHighRisk(o, "sanctioned", new List<string> { "soft-a", "soft-b", "soft-c", "soft-d", "soft-e", "soft-f" }));
            Assert.IsTrue(_scorer.IsHighRisk(o, "sanctioned", new List<string> { "soft-a", "soft-b", "soft-c", "soft-d", "soft-e", "soft-f", "soft-g" }));
        }

        [Test]
        public void ScoreOrDefaultMatchesScore()
        {
            Assert.AreEqual(20, _scorer.ScoreOrDefault(null, null, new List<string> { "hard-x" }));
        }

        [Test]
        public void ScoreAnyHandlesMixedItems()
        {
            Assert.AreEqual(32, _scorer.ScoreAny(new object[] { One("hazmat", 20), 7, "text" }));
            Assert.AreEqual(0, _scorer.ScoreAny(null));
        }
    }
}
