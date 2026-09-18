using System;
using System.IO;
using NUnit.Framework;

namespace OrderKit.Tests
{
    [TestFixture]
    public class ReportStoreTests
    {
        private string _dir;

        [SetUp]
        public void SetUp()
        {
            _dir = Path.Combine(Path.GetTempPath(), "orderkit-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_dir, true);
        }

        [Test]
        public void SavedContentCanBeLoaded()
        {
            string path = Path.Combine(_dir, "daily.csv");
            ReportStore.Save(path, "a,b\nc,d\n");
            Assert.AreEqual("a,b\nc,d\n", ReportStore.Load(path));
            Assert.AreEqual(2, ReportStore.CountLines(path));
        }

        [Test]
        public void MissingFilesLoadAsNull()
        {
            Assert.IsNull(ReportStore.Load(Path.Combine(_dir, "missing.csv")));
            Assert.AreEqual(0, ReportStore.CountLines(Path.Combine(_dir, "missing.csv")));
        }

        [Test]
        public void NullContentIsSavedAsEmpty()
        {
            string path = Path.Combine(_dir, "empty.csv");
            ReportStore.Save(path, null);
            Assert.AreEqual(string.Empty, ReportStore.Load(path));
            Assert.AreEqual(0, ReportStore.CountLines(path));
        }

        [Test]
        public void LoadFromRootCombinesRootAndName()
        {
            ReportStore.Save(Path.Combine(_dir, "weekly.csv"), "x\n");
            Assert.AreEqual("x\n", ReportStore.LoadFromRoot(_dir, "weekly.csv"));
        }
    }
}
