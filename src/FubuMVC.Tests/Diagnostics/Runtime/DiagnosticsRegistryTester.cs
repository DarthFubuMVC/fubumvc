using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using TraceLevel = FubuMVC.Core.TraceLevel;

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


            using (var runtime = FubuApplication.For(registry).Bootstrap())
            {
                var container = runtime.Factory.Get<IContainer>();

                container.DefaultSingletonIs<IRequestHistoryCache, RequestHistoryCache>();
                container.DefaultRegistrationIs<IRequestTrace, RequestTrace>();
                container.DefaultRegistrationIs<IRequestLogBuilder, RequestLogBuilder>();
            }
        }

    }


}