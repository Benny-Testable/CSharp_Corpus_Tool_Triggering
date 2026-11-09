using System;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class PricingServiceTests
    {
        private readonly PricingService _svc = new PricingService();

        private static Order Order10K(string tier = "standard")
        {
            Order o = new Order("P-1", tier);
            o.AddLine(new OrderLine("SKU-1", 10, 1000L, "general"));
            return o;
        }

        private static Order OrderOf(int lines)
        {
            Order o = new Order("P-N", "standard");
            for (int i = 0; i < lines; i++)
            {
                o.AddLine(new OrderLine("SKU-" + i, 1, 100L, "general"));
            }

            return o;
        }

        private static Order Single(string category, long unitCents)
        {
            Order o = new Order("P-S", "standard");
            o.AddLine(new OrderLine("SKU-1", 1, unitCents, category));
            return o;
        }

        [Test]
        public void NullOrderIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => _svc.PriceCents(null, "eu", false, 0));
        }

        [Test]
        public void EmptyOrderPricesZero()
        {
            Assert.AreEqual(0L, _svc.PriceCents(new Order("P-0", null), "domestic", false, 0));
        }

        [Test]
        public void CancelledOrderPricesZero()
        {
            Order o = Order10K();
            Assert.IsTrue(o.TryMoveTo(OrderStatus.Cancelled));
            Assert.AreEqual(0L, _svc.PriceCents(o, "domestic", false, 0));
        }

        [TestCase("standard", 10_250L)]
        [TestCase("silver", 10_050L)]
        [TestCase("gold", 9_750L)]
        [TestCase("platinum", 9_250L)]
        public void TierDiscountIsAppliedToTheSubtotal(string tier, long expected)
        {
            Assert.AreEqual(expected, _svc.PriceCents(Order10K(tier), "domestic", false, 0));
        }

        [TestCase(null, 10_500L)]
        [TestCase("", 10_500L)]
        [TestCase("   ", 10_500L)]
        [TestCase("domestic", 10_250L)]
        [TestCase(" eu ", 11_200L)]
        [TestCase("apac", 11_800L)]
        [TestCase("moon", 12_400L)]
        public void RegionSurchargeDependsOnTheTrimmedRegion(string region, long expected)
        {
            Assert.AreEqual(expected, _svc.PriceCents(Order10K(), region, false, 0));
        }

        [Test]
        public void LargeOrdersGetARegionalRebate()
        {
            Order big = new Order("P-B", "standard");
            big.AddLine(new OrderLine("SKU-1", 100, 1000L, "general"));
            Assert.AreEqual(100_700L, _svc.PriceCents(big, "eu", false, 0));
            Assert.AreEqual(101_100L, _svc.PriceCents(big, "apac", false, 0));
        }

        [Test]
        public void TheRebateStartsJustAboveTheLargeOrderLimit()
        {
            Assert.AreEqual(100_000L, _svc.PriceCents(Single("general", 98_800L), "eu", false, 0));
            Assert.AreEqual(99_501L, _svc.PriceCents(Single("general", 98_801L), "eu", false, 0));
            Assert.AreEqual(100_000L, _svc.PriceCents(Single("general", 98_200L), "apac", false, 0));
            Assert.AreEqual(99_301L, _svc.PriceCents(Single("general", 98_201L), "apac", false, 0));
        }

        [TestCase(10, 2_750L)]
        [TestCase(11, 3_850L)]
        [TestCase(20, 4_750L)]
        [TestCase(21, 6_350L)]
        public void ExpediteFeeGrowsWithTheLineCount(int lines, long expected)
        {
            Assert.AreEqual(expected, _svc.PriceCents(OrderOf(lines), "domestic", true, 0));
        }

        [TestCase(0, 10_250L)]
        [TestCase(1, 10_250L)]
        [TestCase(2, 10_050L)]
        [TestCase(4, 10_050L)]
        [TestCase(9, 9_750L)]
        [TestCase(10, 9_250L)]
        [TestCase(-3, 10_250L)]
        public void LoyaltyYearsEarnACredit(int years, long expected)
        {
            Assert.AreEqual(expected, _svc.PriceCents(Order10K(), "domestic", false, years));
        }

        [Test]
        public void FiveYearsOfLoyaltyEarnTheFiveYearCredit()
        {
            Assert.AreEqual(9_750L, _svc.PriceCents(Order10K(), "domestic", false, 5));
        }

        [TestCase("hazmat", 1_000L, 2_750L)]
        [TestCase("fragile", 500L, 1_150L)]
        [TestCase("bulk", 40L, 350L)]
        [TestCase("bulk", 100L, 350L)]
        [TestCase("general", 10L, 260L)]
        [TestCase("weird", 1_000L, 1_300L)]
        public void CategoryFloorsTopUpCheapLines(string category, long unit, long expected)
        {
            Assert.AreEqual(expected, _svc.PriceCents(Single(category, unit), "domestic", false, 0));
        }

        [Test]
        public void PriceIsNeverNegative()
        {
            Order o = new Order("P-2", "platinum");
            o.AddLine(new OrderLine("SKU-1", 1, 1L, "general"));
            Assert.AreEqual(0L, _svc.PriceCents(o, "domestic", false, 20));
        }

        [Test]
        public void PromoCodes()
        {
            Assert.AreEqual(900L, _svc.ApplyPromo(1000L, "WELCOME10"));
            Assert.AreEqual(1000L, _svc.ApplyPromo(1000L, null));
            Assert.AreEqual(1000L, _svc.ApplyPromo(1000L, "UNKNOWN"));
            Assert.AreEqual(190_000L, _svc.ApplyPromo(200_000L, "BULK5"));
            Assert.AreEqual(100_000L, _svc.ApplyPromo(100_000L, "BULK5"));
        }

        [Test]
        public void FloorsByCategory()
        {
            Assert.AreEqual(2500L, _svc.FloorFor("hazmat"));
            Assert.AreEqual(900L, _svc.FloorFor("fragile"));
            Assert.AreEqual(100L, _svc.FloorFor("bulk"));
            Assert.AreEqual(0L, _svc.FloorFor("general"));
            Assert.AreEqual(0L, _svc.FloorFor("nope"));
            Assert.AreEqual(0L, _svc.FloorFor(null));
            Assert.AreEqual(0L, _svc.FloorFor(""));
        }
    }
}
