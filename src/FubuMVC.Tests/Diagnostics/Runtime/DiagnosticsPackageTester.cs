using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Runtime;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class DiagnosticsPackageTester
    {
        [Test]
        public void diagnostics_should_be_enabled_in_development_mode()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Mode = "development"))
            {
                var container = runtime.Get<IContainer>();
                container.ShouldHaveRegistration<ILogListener, RequestTraceListener>();
            }
        }
    }
}