using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DefaultDiagnosticsConventionsTester
    {
        private FubuRegistry _registry;

        [SetUp]
        public void setup()
        {
            _registry = new FubuRegistry(x => x.IncludeDiagnostics(true));
        }

        [Test]
        public void should_limit_requests_to_fifty()
        {
            var config = new DiagnosticsConfiguration();
            _registry
                .Services(x =>
                              {
                                  config = x.FindAllValues<DiagnosticsConfiguration>().First();
                              });
            _registry.BuildGraph();

            config.MaxRequests.ShouldEqual(50);
        }

        [Test]
        public void should_exclude_diagnostics_requests()
        {
            IRequestHistoryCacheFilter filter = null;
            _registry
                .Services(x =>
                              {
                                  filter = x.FindAllValues<IRequestHistoryCacheFilter>().First();
                              });
            _registry.BuildGraph();

            var request = new CurrentRequest {Path = "/{0}/requests".ToFormat(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT)};
            filter.Exclude(request).ShouldBeTrue();
        }
    }
}