using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class OrderExporterTests
    {
        private static Order Sample()
        {
            Order o = new Order("E-1", "gold");
            o.AddLine(new OrderLine("A", 2, 500L, "general"));
            return o;
        }

        [Test]
        public void DtoCopiesTheOrderFields()
        {
            OrderDto dto = OrderExporter.ToDto(Sample());
            Assert.AreEqual("E-1", dto.Id);
            Assert.AreEqual("gold", dto.Tier);
            Assert.AreEqual("draft", dto.Status);
            Assert.AreEqual(1, dto.Lines.Count);
            Assert.AreEqual("A", dto.Lines[0].Sku);
            Assert.AreEqual(2, dto.Lines[0].Quantity);
            Assert.AreEqual(500L, dto.Lines[0].UnitCents);
            Assert.AreEqual("general", dto.Lines[0].Category);
        }

        [Test]
        public void CompactJsonHasNoWhitespace()
        {
            Assert.AreEqual(
                "{\"Id\":\"E-1\",\"Tier\":\"gold\",\"Status\":\"draft\",\"Lines\":[{\"Sku\":\"A\",\"Quantity\":2,\"UnitCents\":500,\"Category\":\"general\"}]}",
                OrderExporter.ToCompactJson(Sample()));
        }

        [Test]
        public void JsonRoundTrips()
        {
            string json = OrderExporter.ToJson(Sample());
            StringAssert.Contains("\"Id\": \"E-1\"", json);
            OrderDto back = OrderExporter.FromJson(json);
            Assert.AreEqual("E-1", back.Id);
            Assert.AreEqual(2, back.Lines[0].Quantity);
        }

        [Test]
        public void TryFromJsonReportsBadDocuments()
        {
            Assert.IsTrue(OrderExporter.TryFromJson("{\"Id\":\"X\"}", out OrderDto dto));
            Assert.AreEqual("X", dto.Id);
            Assert.IsFalse(OrderExporter.TryFromJson("not json", out dto));
            Assert.IsNull(dto);
            Assert.IsFalse(OrderExporter.TryFromJson("null", out dto));
        }
    }
}
