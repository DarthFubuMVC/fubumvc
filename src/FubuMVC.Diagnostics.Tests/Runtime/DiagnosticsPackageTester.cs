using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
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
        public void debug_detector_is_registered()
        {
            graph.Services.DefaultServiceFor<IDebugDetector>().Type.ShouldEqual(typeof (DebugDetector));
        }

        [Test]
        public void debug_report_is_registered()
        {
            graph.Services.DefaultServiceFor<IDebugReport>().Type.ShouldEqual(typeof (DebugReport));
        }


    }
}