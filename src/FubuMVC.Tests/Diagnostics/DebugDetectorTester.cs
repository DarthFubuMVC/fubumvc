using FubuCore.Binding;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DebugDetectorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            data = new InMemoryRequestData();
            detector = new DebugDetector(data);
        }

        #endregion

        private InMemoryRequestData data;
        private DebugDetector detector;

        [Test]
        public void negative_case_when_the_value_is_missing()
        {
            detector.IsOutputWritingLatched().ShouldBeFalse();
        }

        [Test]
        public void positive_case_when_the_value_exists()
        {
            data[DebugDetector.FLAG] = "anything";
            detector.IsOutputWritingLatched().ShouldBeTrue();
        }

        [Test]
        public void unlatch_unlocks_the_writing()
        {
            data[DebugDetector.FLAG] = "anything";
            detector.IsOutputWritingLatched().ShouldBeTrue();

            detector.UnlatchWriting();

            detector.IsOutputWritingLatched().ShouldBeFalse();
        }
    }
}