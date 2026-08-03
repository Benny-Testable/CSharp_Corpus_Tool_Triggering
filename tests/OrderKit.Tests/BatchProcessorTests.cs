using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class BatchProcessorTests
    {
        private static Order Line(string id, int quantity)
        {
            Order o = new Order(id, null);
            o.AddLine(new OrderLine("S", quantity, 100L, "general"));
            return o;
        }

        private static IList<Order> Orders()
        {
            Order cancelled = Line("D-4", 1);
            cancelled.TryMoveTo(OrderStatus.Cancelled);
            return new List<Order> { Line("D-2", 1000), Line("D-1", 1), new Order("D-3", null), null, cancelled };
        }

        [Test]
        public void OrderBatchProcessorAcceptsOrdersUnderTheCeiling()
        {
            OrderBatchProcessor p = new OrderBatchProcessor();
            p.Process(Orders(), 50_000L);
            Assert.AreEqual(1, p.AcceptedCount());
            Assert.AreEqual(3, p.RejectedCount());
            Assert.AreEqual(100L, p.AcceptedTotalCents());
            Assert.AreEqual(new[] { "D-1" }, p.AcceptedIds());
        }

        [Test]
        public void ArchiveBatchProcessorBehavesTheSame()
        {
            ArchiveBatchProcessor p = new ArchiveBatchProcessor();
            p.Process(Orders(), 50_000L);
            Assert.AreEqual(1, p.AcceptedCount());
            Assert.AreEqual(3, p.RejectedCount());
            Assert.AreEqual(100L, p.AcceptedTotalCents());
            Assert.AreEqual(new[] { "D-1" }, p.AcceptedIds());
        }

        [Test]
        public void AnOrderExactlyAtTheCeilingIsAccepted()
        {
            OrderBatchProcessor p = new OrderBatchProcessor();
            p.Process(Orders(), 100_000L);
            Assert.AreEqual(new[] { "D-1", "D-2" }, p.AcceptedIds());
            Assert.AreEqual(100_100L, p.AcceptedTotalCents());
        }

        [Test]
        public void ResetClearsBothLists()
        {
            OrderBatchProcessor a = new OrderBatchProcessor();
            a.Process(Orders(), 50_000L);
            a.Reset();
            Assert.AreEqual(0, a.AcceptedCount());
            Assert.AreEqual(0, a.RejectedCount());

            ArchiveBatchProcessor b = new ArchiveBatchProcessor();
            b.Process(Orders(), 50_000L);
            b.Reset();
            Assert.AreEqual(0, b.AcceptedCount());
            Assert.AreEqual(0, b.RejectedCount());
        }

        [Test]
        public void NullBatchesAreRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderBatchProcessor().Process(null, 1L));
            Assert.Throws<ArgumentNullException>(() => new ArchiveBatchProcessor().Process(null, 1L));
        }
    }
}
