using FubuMVC.Core;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class DiagnosticsSettingsTester
    {
        [Test]
        public void the_default_max_requests_is_50()
        {
            new DiagnosticsSettings().MaxRequests.ShouldEqual(50);
        }
    }
}