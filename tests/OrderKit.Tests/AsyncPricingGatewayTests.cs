using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class AsyncPricingGatewayTests
    {
        private static Order Sample(string id, long unit)
        {
            Order o = new Order(id, "standard");
            o.AddLine(new OrderLine("S", 1, unit, "general"));
            return o;
        }

        [Test]
        public void NullPricingServiceIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new AsyncPricingGateway(null));
        }

        [Test]
        public async Task PriceAsyncMatchesTheSynchronousPrice()
        {
            PricingService svc = new PricingService();
            AsyncPricingGateway gw = new AsyncPricingGateway(svc);
            Order o = Sample("G-1", 1000L);
            long price = await gw.PriceAsync(o, "domestic");
            Assert.AreEqual(svc.PriceCents(o, "domestic", false, 0), price);
            Assert.AreEqual(1250L, price);
        }

        [Test]
        public async Task PriceBatchAsyncSumsTheOrders()
        {
            AsyncPricingGateway gw = new AsyncPricingGateway(new PricingService());
            var orders = new List<Order> { Sample("G-1", 1000L), Sample("G-2", 2000L) };
            Assert.AreEqual(1250L + 2250L, await gw.PriceBatchAsync(orders, "domestic"));
        }
    }
}
