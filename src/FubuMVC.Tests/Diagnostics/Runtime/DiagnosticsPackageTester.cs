using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Runtime;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class DiagnosticsPackageTester
    {
        [Test]
        public void diagnostics_should_be_enabled_in_development_mode()
        {
            FubuMode.SetUpForDevelopmentMode();
            using (var runtime = FubuApplication.DefaultPolicies().Bootstrap())
            {
                runtime.Container.ShouldHaveRegistration<ILogListener, RequestTraceListener>();
            }
        }
    }
}