using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class ReportBuilderTests
    {
        private static readonly string Nl = Environment.NewLine;

        private static IReadOnlyList<Order> Orders()
        {
            Order a = new Order("A-1", null);
            a.AddLine(new OrderLine("S-1", 2, 500L, "general"));
            a.AddLine(new OrderLine("S-2", 1, 1500L, "general"));
            Order b = new Order("A-2", null);
            b.AddLine(new OrderLine("S-1", 1, 100L, "general"));
            return new List<Order> { a, b, null, new Order("A-3", null) };
        }

        private const string Expected =
            "# Sales\n" + "A-1,draft,2500\n" + "A-2,draft,100\n" + "orders=2,total=2600\n";

        [Test]
        public void DailyReportListsEachNonEmptyOrder()
        {
            Assert.AreEqual(Expected.Replace("\n", Nl), new ReportBuilder().BuildDaily(Orders(), "Sales"));
        }

        [Test]
        public void WeeklyReportListsEachNonEmptyOrder()
        {
            Assert.AreEqual(Expected.Replace("\n", Nl), new ReportBuilder().BuildWeekly(Orders(), "Sales"));
        }

        [Test]
        public void WeeklyReportIncludesTheLastOrder()
        {
            Order only = new Order("W-1", null);
            only.AddLine(new OrderLine("S-1", 1, 700L, "general"));
            string report = new ReportBuilder().BuildWeekly(new List<Order> { only }, "Week");
            Assert.AreEqual(("# Week\nW-1,draft,700\norders=1,total=700\n").Replace("\n", Nl), report);
        }

        [Test]
        public void EmptyListGivesOnlyTheHeaderAndTotals()
        {
            Assert.AreEqual("# T\norders=0,total=0\n".Replace("\n", Nl), new ReportBuilder().BuildDaily(new List<Order>(), "T"));
        }

        [Test]
        public void NullListsAreRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new ReportBuilder().BuildDaily(null, "x"));
            Assert.Throws<ArgumentNullException>(() => new ReportBuilder().BuildWeekly(null, "x"));
        }
    }
}
