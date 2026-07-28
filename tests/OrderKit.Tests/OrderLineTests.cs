using System;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class OrderLineTests
    {
        [Test]
        public void ExtendedCentsMultipliesQuantityByUnitPrice()
        {
            Assert.AreEqual(1500L, new OrderLine("SKU-1", 3, 500L, "general").ExtendedCents());
        }

        [Test]
        public void BlankCategoryFallsBackToGeneral()
        {
            Assert.AreEqual("general", new OrderLine("SKU-9", 1, 100L, "  ").Category);
            Assert.AreEqual("general", new OrderLine("SKU-9", 1, 100L, null).Category);
            Assert.AreEqual("hazmat", new OrderLine("SKU-9", 1, 100L, "hazmat").Category);
        }

        [Test]
        public void InvalidArgumentsAreRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderLine(null, 1, 100L, "x"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderLine("S", -1, 100L, "x"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderLine("S", 1, -1L, "x"));
        }

        [Test]
        public void ZeroQuantityAndPriceAreAllowed()
        {
            OrderLine line = new OrderLine("S", 0, 0L, "x");
            Assert.AreEqual(0L, line.ExtendedCents());
        }

        [Test]
        public void WithQuantityKeepsTheOtherFields()
        {
            OrderLine line = new OrderLine("S", 1, 250L, "fragile").WithQuantity(4);
            Assert.AreEqual(4, line.Quantity);
            Assert.AreEqual("S", line.Sku);
            Assert.AreEqual(250L, line.UnitCents);
            Assert.AreEqual("fragile", line.Category);
            Assert.AreEqual("S x4 @ 250", line.ToString());
        }
    }
}
