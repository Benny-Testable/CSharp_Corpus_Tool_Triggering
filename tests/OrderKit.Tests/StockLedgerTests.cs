using System;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class StockLedgerTests
    {
        private StockLedger _ledger;

        [SetUp]
        public void SetUp()
        {
            _ledger = new StockLedger();
            _ledger.Receive("ABC-0001", 10);
        }

        [Test]
        public void ReceivedStockIsOnHandIgnoringCase()
        {
            _ledger.Receive("abc-0001", 5);
            Assert.AreEqual(15, _ledger.OnHand("ABC-0001"));
            Assert.AreEqual(0, _ledger.OnHand("ZZZ-0000"));
            Assert.AreEqual(0, _ledger.OnHand(null));
        }

        [Test]
        public void ReceiveRejectsBadInput()
        {
            Assert.Throws<ArgumentException>(() => _ledger.Receive(" ", 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ledger.Receive("ABC-0002", 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ledger.Receive("ABC-0002", -3));
        }

        [Test]
        public void AllocationReducesStock()
        {
            Assert.IsTrue(_ledger.TryAllocate("ABC-0001", 4));
            Assert.AreEqual(6, _ledger.OnHand("ABC-0001"));
        }

        [Test]
        public void AllocatingExactlyWhatIsOnHandSucceeds()
        {
            Assert.IsTrue(_ledger.TryAllocate("ABC-0001", 10));
            Assert.AreEqual(0, _ledger.OnHand("ABC-0001"));
            Assert.IsFalse(_ledger.TryAllocate("ABC-0001", 1));
        }

        [Test]
        public void AllocationBeyondStockFailsWithoutChangingIt()
        {
            Assert.IsFalse(_ledger.TryAllocate("ABC-0001", 11));
            Assert.AreEqual(10, _ledger.OnHand("ABC-0001"));
        }

        [Test]
        public void ReleasePutsStockBack()
        {
            _ledger.TryAllocate("ABC-0001", 6);
            _ledger.Release("ABC-0001", 6);
            Assert.AreEqual(10, _ledger.OnHand("ABC-0001"));
        }

        [Test]
        public void AllocateOrderReportsShortLines()
        {
            _ledger.Receive("ABC-0002", 1);
            Order order = new Order("S-1", null);
            order.AddLine(new OrderLine("ABC-0001", 3, 100L, "general"));
            order.AddLine(new OrderLine("ABC-0002", 2, 100L, "general"));
            var shortages = _ledger.AllocateOrder(order);
            Assert.AreEqual(new[] { "ABC-0002" }, shortages);
            Assert.AreEqual(7, _ledger.OnHand("ABC-0001"));
            Assert.AreEqual(1, _ledger.TotalShortfalls);
        }

        [Test]
        public void AllocateOrderRejectsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ledger.AllocateOrder(null));
        }
    }
}
