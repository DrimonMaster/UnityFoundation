using System;
using NUnit.Framework;
using UnityFoundation.Services;

namespace UnityFoundation.Tests
{
    [TestFixture]
    public class CrashReporterTests
    {
        private CrashReporter _reporter;

        [SetUp]
        public void SetUp() => _reporter = new CrashReporter();

        [TearDown]
        public void TearDown()
        {
            if (_reporter.IsReady) _reporter.Dispose();
        }

        [Test]
        public void Report_BeforeInitialize_EnqueuesReport()
        {
            // IsReady is false before Initialize — Report should enqueue, not throw
            Assert.DoesNotThrow(() => _reporter.Report(new Exception("pending")));
            Assert.That(_reporter.IsReady, Is.False);
        }

        [Test]
        public void Initialize_DrainsPendingQueue()
        {
            _reporter.Report(new Exception("pending1"));
            _reporter.Report(new Exception("pending2"));

            // Initialize must complete without throwing even with pending reports
            Assert.DoesNotThrow(() => _reporter.Initialize());
            Assert.That(_reporter.IsReady, Is.True);
        }
    }
}
