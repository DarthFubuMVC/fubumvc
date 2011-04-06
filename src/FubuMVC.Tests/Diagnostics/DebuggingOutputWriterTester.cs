using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DebuggingOutputWriterTester : InteractionContext<DebuggingOutputWriter>
    {
        [Test]
        public void the_inner_should_be_the_http_writer_when_the_debug_flag_is_not_detected()
        {
            MockFor<IDebugDetector>().Stub(x => x.IsDebugCall()).Return(false);
            ClassUnderTest.Inner.ShouldBeOfType<HttpResponseOutputWriter>();
        }

        [Test]
        public void the_inner_should_be_the_recording_option_when_the_debug_flag_is_detected()
        {
            MockFor<IDebugDetector>().Stub(x => x.IsDebugCall()).Return(true);
            ClassUnderTest.Inner.ShouldBeOfType<RecordingOutputWriter>();
        }
    }
}