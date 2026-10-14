using System.Collections.Generic;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class HistoryAdditionsTests
    {
        private static Order Sample()
        {
            Order o = new Order("H-1", "gold");
            o.AddLine(new OrderLine("A", 3, 100L, "general"));
            o.AddLine(new OrderLine("B", 2, 500L, "fragile"));
            return o;
        }

        [Test]
        public void TotalQuantityAndCategory()
        {
            Order o = Sample();
            Assert.AreEqual(5, o.TotalQuantity());
            Assert.IsTrue(o.HasCategory("fragile"));
            Assert.IsFalse(o.HasCategory("hazmat"));
        }

        [Test]
        public void TagsAcceptNullExtra()
        {
            Assert.AreEqual(2, Sample().Tags(null).Count);
        }

        [Test]
        public void PromoCodes()
        {
            PricingService p = new PricingService();
            Assert.AreEqual(900L, p.ApplyPromo(1000L, "WELCOME10"));
            Assert.AreEqual(1000L, p.ApplyPromo(1000L, null));
        }

        [Test]
        public void NegativeLoyaltyEqualsZero()
        {
            PricingService p = new PricingService();
            Assert.AreEqual(p.PriceCents(Sample(), "eu", false, 0), p.PriceCents(Sample(), " eu ", false, -3));
        }

        [Test]
        public void SanitizerHelpers()
        {
            Assert.AreEqual("abc", InputSanitizer.Truncate("abcdef", 3));
            Assert.AreEqual("root/", InputSanitizer.BuildReportPath("root", null));
        }

        [Test]
        public void RiskHelpers()
        {
            RiskScorer s = new RiskScorer();
            Order o = new Order("R-2", null);
            o.AddLine(new OrderLine("H", 20, 100L, "hazmat"));
            Assert.IsTrue(s.IsHighRisk(o, "Sanctioned", new List<string> { "hard-x" }));
        }
    }
}
