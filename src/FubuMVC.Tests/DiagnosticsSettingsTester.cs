using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class DiagnosticsSettingsTester
    {
        [Test]
        public void default_request_count_is_200()
        {
            new DiagnosticsSettings()
                .MaxRequests.ShouldEqual(200);
        }

        [Test]
        public void the_default_trace_level_is_verbose()
        {
            new DiagnosticsSettings()
                .TraceLevel.ShouldEqual(TraceLevel.Verbose);
        }
    }
}