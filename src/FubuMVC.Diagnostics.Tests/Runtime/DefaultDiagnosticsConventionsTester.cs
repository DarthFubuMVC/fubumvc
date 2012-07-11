using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class DefaultDiagnosticsConventionsTester
    {
        #region Setup/Teardown

        [SetUp]
        public void setup()
        {
            _registry = new FubuRegistry(x => x.Import<DiagnosticsRegistration>());

            Assert.Fail("NOW");
        }

        #endregion

        private FubuRegistry _registry;

        [Test]
        public void default_filter_excludes_diagnostics_requests()
        {
            //var filter = new DiagnosticRequestHistoryCacheFilter();
            //var request = new CurrentRequest { Path = "/{0}/requests".ToFormat(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT) };
            //filter.Exclude(request).ShouldBeTrue();
        }

        [Test]
        public void should_exclude_diagnostics_requests()
        {
            //BehaviorGraph.BuildFrom(_registry).Services.ServicesFor(typeof (IRequestHistoryCacheFilter))
            //    .Any(x => x.Type == typeof (DiagnosticRequestHistoryCacheFilter));
        }
    }
}