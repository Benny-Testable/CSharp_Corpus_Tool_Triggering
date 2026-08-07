using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class InputSanitizerTests
    {
        [Test]
        public void SanitizeStripsSuspiciousCharacters()
        {
            Assert.AreEqual("abc", InputSanitizer.Sanitize("a<b>c"));
            Assert.AreEqual("x", InputSanitizer.Sanitize(" x "));
            Assert.AreEqual(string.Empty, InputSanitizer.Sanitize(null));
            Assert.AreEqual("plain text", InputSanitizer.Sanitize("plain text"));
        }

        [Test]
        public void SuspiciousIsDetected()
        {
            Assert.IsTrue(InputSanitizer.LooksSuspicious("a;b"));
            Assert.IsTrue(InputSanitizer.LooksSuspicious("$"));
            Assert.IsFalse(InputSanitizer.LooksSuspicious("plain"));
            Assert.IsFalse(InputSanitizer.LooksSuspicious(""));
            Assert.IsFalse(InputSanitizer.LooksSuspicious(null));
        }

        [Test]
        public void LookupKeyJoinsTenantAndSku()
        {
            Assert.AreEqual("tenant/t/sku/s", InputSanitizer.BuildLookupKey("t", "s"));
            Assert.AreEqual("tenant//sku/", InputSanitizer.BuildLookupKey(null, null));
        }

        [Test]
        public void QueryIsFormatted()
        {
            Assert.AreEqual("SELECT * FROM orders WHERE 1=1", InputSanitizer.BuildQuery("orders", "1=1"));
        }

        [Test]
        public void ReportPathJoinsRootAndName()
        {
            Assert.AreEqual("/tmp/r.csv", InputSanitizer.BuildReportPath("/tmp/", "r.csv"));
            Assert.AreEqual("root/", InputSanitizer.BuildReportPath("root", null));
            Assert.Throws<ArgumentNullException>(() => InputSanitizer.BuildReportPath(null, "x"));
        }

        [Test]
        public void CommandJoinsArguments()
        {
            Assert.AreEqual("ls -l -a", InputSanitizer.BuildCommand("ls", new List<string> { "-l", "-a" }));
            Assert.AreEqual("ls", InputSanitizer.BuildCommand("ls", null));
        }

        [Test]
        public void TruncateKeepsAtMostMaxCharacters()
        {
            Assert.AreEqual("abc", InputSanitizer.Truncate("abcdef", 3));
            Assert.AreEqual("abc", InputSanitizer.Truncate("abc", 3));
            Assert.AreEqual("ab", InputSanitizer.Truncate("ab", 3));
            Assert.AreEqual(string.Empty, InputSanitizer.Truncate(null, 3));
            Assert.AreEqual(string.Empty, InputSanitizer.Truncate("abc", 0));
        }
    }
}
