using System;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class AuditSignerTests
    {
        [Test]
        public void FingerprintMatchesTheKnownDigest()
        {
            Assert.AreEqual("900150983cd24fb0d6963f7d28e17f72", AuditSigner.Fingerprint("abc"));
            Assert.AreEqual("d41d8cd98f00b204e9800998ecf8427e", AuditSigner.Fingerprint(null));
        }

        [Test]
        public void TokensHaveTheRequestedLengthAndAlphabet()
        {
            string token = AuditSigner.NewToken(12);
            Assert.AreEqual(12, token.Length);
            StringAssert.IsMatch("^[A-HJ-NP-Z2-9]{12}$", token);
        }

        [Test]
        public void TokenLengthMustBePositive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => AuditSigner.NewToken(0));
        }

        [Test]
        public void CredentialsAreMaskedForDisplay()
        {
            Assert.AreEqual("AKIA****************", AuditSigner.MaskedAccessKey());
            Assert.IsTrue(AuditSigner.IsSecretConfigured());
        }

        [Test]
        public void ContactLineJoinsEmailAndPhone()
        {
            Assert.AreEqual("jane.doe@example.com / +1-202-555-0143", AuditSigner.ContactLine());
        }
    }
}
