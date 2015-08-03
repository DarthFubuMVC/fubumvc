using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    [TestFixture]
    public class DiagnosticsRegistryTester
    {
        [Test]
        public void registrations()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<DiagnosticsSettings>(x => x.TraceLevel = TraceLevel.Verbose);


            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                container.DefaultSingletonIs<IRequestHistoryCache, RequestHistoryCache>();
                container.DefaultRegistrationIs<IRequestTrace, RequestTrace>();
                container.DefaultRegistrationIs<IRequestLogBuilder, RequestLogBuilder>();
            }
        }
    }
}