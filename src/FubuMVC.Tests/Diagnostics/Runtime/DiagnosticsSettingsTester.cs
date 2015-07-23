using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class DiagnosticsSettingsTester
    {
        [Test]
        public void the_default_max_requests_is_200()
        {
            new DiagnosticsSettings().MaxRequests.ShouldBe(200);
        }
    }
}