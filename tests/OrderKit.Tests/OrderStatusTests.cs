using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class OrderStatusTests
    {
        [TestCase(OrderStatus.Draft, "draft")]
        [TestCase(OrderStatus.Submitted, "submitted")]
        [TestCase(OrderStatus.Priced, "priced")]
        [TestCase(OrderStatus.Fulfilled, "fulfilled")]
        [TestCase(OrderStatus.Cancelled, "cancelled")]
        [TestCase(OrderStatus.Rejected, "rejected")]
        [TestCase((OrderStatus)42, "unknown")]
        public void LabelsAreLowerCase(OrderStatus status, string label)
        {
            Assert.AreEqual(label, status.Label());
        }

        [TestCase(OrderStatus.Draft, false)]
        [TestCase(OrderStatus.Submitted, false)]
        [TestCase(OrderStatus.Priced, false)]
        [TestCase(OrderStatus.Fulfilled, true)]
        [TestCase(OrderStatus.Cancelled, true)]
        [TestCase(OrderStatus.Rejected, true)]
        public void TerminalStatuses(OrderStatus status, bool terminal)
        {
            Assert.AreEqual(terminal, status.IsTerminal());
        }

        [TestCase(OrderStatus.Draft, OrderStatus.Submitted, true)]
        [TestCase(OrderStatus.Draft, OrderStatus.Cancelled, true)]
        [TestCase(OrderStatus.Draft, OrderStatus.Priced, false)]
        [TestCase(OrderStatus.Submitted, OrderStatus.Priced, true)]
        [TestCase(OrderStatus.Submitted, OrderStatus.Rejected, true)]
        [TestCase(OrderStatus.Submitted, OrderStatus.Cancelled, true)]
        [TestCase(OrderStatus.Submitted, OrderStatus.Fulfilled, false)]
        [TestCase(OrderStatus.Priced, OrderStatus.Fulfilled, true)]
        [TestCase(OrderStatus.Priced, OrderStatus.Cancelled, true)]
        [TestCase(OrderStatus.Priced, OrderStatus.Rejected, false)]
        [TestCase(OrderStatus.Fulfilled, OrderStatus.Cancelled, false)]
        [TestCase(OrderStatus.Cancelled, OrderStatus.Draft, false)]
        public void TransitionMatrix(OrderStatus from, OrderStatus to, bool allowed)
        {
            Assert.AreEqual(allowed, from.CanMoveTo(to));
        }

        [Test]
        public void LabelsParseCaseInsensitively()
        {
            Assert.IsTrue(OrderStatusExtensions.TryParseLabel("Submitted", out OrderStatus status));
            Assert.AreEqual(OrderStatus.Submitted, status);
            Assert.IsTrue(OrderStatusExtensions.TryParseLabel(" priced ", out status));
            Assert.AreEqual(OrderStatus.Priced, status);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("bogus")]
        [TestCase("99")]
        public void BadLabelsDoNotParse(string label)
        {
            Assert.IsFalse(OrderStatusExtensions.TryParseLabel(label, out OrderStatus status));
            Assert.AreEqual(OrderStatus.Draft, status);
        }
    }
}
