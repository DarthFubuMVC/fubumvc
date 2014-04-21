using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class DiagnosticsSettingsTester
    {
        [Test]
        public void the_default_max_requests_is_200()
        {
            new DiagnosticsSettings().MaxRequests.ShouldEqual(200);
        }
    }
}