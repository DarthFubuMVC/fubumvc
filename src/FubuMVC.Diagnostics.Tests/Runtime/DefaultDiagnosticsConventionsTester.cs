using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class DefaultDiagnosticsConventionsTester
    {
        private FubuRegistry _registry;

        [SetUp]
        public void setup()
        {
            _registry = new FubuRegistry(x => x.Import<DiagnosticsRegistration>());
        }

        [Test]
        public void should_exclude_diagnostics_requests()
        {
            _registry.BuildGraph().Services.ServicesFor(typeof (IRequestHistoryCacheFilter))
                .Any(x => x.Type == typeof (DiagnosticRequestHistoryCacheFilter));
        }

        [Test]
        public void default_filter_excludes_diagnostics_requests()
        {
            var filter = new DiagnosticRequestHistoryCacheFilter();
            var request = new CurrentRequest { Path = "/{0}/requests".ToFormat(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT) };
            filter.Exclude(request).ShouldBeTrue();
        }
    }
}