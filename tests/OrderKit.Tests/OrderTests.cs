using System;
using System.Linq;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class OrderTests
    {
        private static Order Sample()
        {
            Order o = new Order("A-1", "gold");
            o.AddLine(new OrderLine("SKU-1", 2, 500L, "general"));
            o.AddLine(new OrderLine("SKU-2", 1, 1500L, "fragile"));
            return o;
        }

        [Test]
        public void NullIdIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(null, "gold"));
        }

        [Test]
        public void BlankCustomerTierFallsBackToStandard()
        {
            Assert.AreEqual("standard", new Order("A-3", "  ").CustomerTier);
            Order o = new Order("A-4", "gold");
            o.CustomerTier = "";
            Assert.AreEqual("standard", o.CustomerTier);
        }

        [Test]
        public void CustomerTierIsTrimmedAndLowerCased()
        {
            Assert.AreEqual("gold", new Order("A-5", "  GOLD ").CustomerTier);
        }

        [Test]
        public void NewOrderIsADraftWithNoLines()
        {
            Order o = new Order("A-2", null);
            Assert.AreEqual(OrderStatus.Draft, o.Status);
            Assert.IsTrue(o.IsEmpty());
            Assert.AreEqual(0, o.LineCount());
            Assert.IsNull(o.FirstSku());
        }

        [Test]
        public void SubtotalAndQuantitySumLines()
        {
            Order o = Sample();
            Assert.AreEqual(2500L, o.SubtotalCents());
            Assert.AreEqual(3, o.TotalQuantity());
            Assert.AreEqual(2, o.LineCount());
            Assert.AreEqual("SKU-1", o.FirstSku());
        }

        [Test]
        public void AddingANullLineIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => Sample().AddLine(null));
        }

        [Test]
        public void CategoriesAreDistinctAndSorted()
        {
            Order o = Sample();
            o.AddLine(new OrderLine("SKU-3", 1, 100L, "fragile"));
            var cats = o.DistinctCategories().ToList();
            Assert.AreEqual(new[] { "fragile", "general" }, cats);
            Assert.IsTrue(o.HasCategory("fragile"));
            Assert.IsFalse(o.HasCategory("hazmat"));
        }

        [Test]
        public void StatusFollowsTheAllowedTransitions()
        {
            Order o = Sample();
            Assert.IsFalse(o.TryMoveTo(OrderStatus.Fulfilled));
            Assert.AreEqual(OrderStatus.Draft, o.Status);
            Assert.IsTrue(o.TryMoveTo(OrderStatus.Submitted));
            Assert.IsTrue(o.TryMoveTo(OrderStatus.Priced));
            Assert.IsTrue(o.TryMoveTo(OrderStatus.Fulfilled));
            Assert.IsFalse(o.TryMoveTo(OrderStatus.Cancelled));
            Assert.AreEqual(OrderStatus.Fulfilled, o.Status);
        }

        [Test]
        public void DescribeSummarisesTheOrder()
        {
            Assert.AreEqual("A-1: 2 lines, 2500c, draft", Sample().Describe());
        }
    }
}
