using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class DiagnosticsPackageTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x => x.Import<DiagnosticsRegistration>());
        }

        #endregion

        private BehaviorGraph graph;


        [Test]
        public void adds_RequestTraceListener()
        {
            graph.Services.ServicesFor(typeof(ILogListener)).Any(x => x.Type == typeof(RequestTraceListener))
                .ShouldBeTrue();
        }
    }
}