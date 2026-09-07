using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class OrderQueriesTests
    {
        [Test]
        public void LookupCommandCarriesTheCustomerId()
        {
            using (var cmd = OrderQueries.BuildLookup("C-1"))
            {
                Assert.AreEqual("SELECT Id, Status FROM Orders WHERE CustomerId = 'C-1'", cmd.CommandText);
            }
        }

        [Test]
        public void SearchQueryCarriesLimitAndStatus()
        {
            Assert.AreEqual("SELECT TOP 5 Id FROM Orders WHERE Status = 'open'", OrderQueries.BuildSearch("open", 5));
        }

        [TestCase(1, 10, 0)]
        [TestCase(3, 10, 20)]
        [TestCase(0, 10, 0)]
        public void PagedListComputesTheOffset(int page, int size, int offset)
        {
            string expected = $"SELECT Id FROM Orders ORDER BY Id OFFSET {offset} ROWS FETCH NEXT {size} ROWS ONLY";
            Assert.AreEqual(expected, OrderQueries.BuildPagedList(page, size));
        }
    }
}
