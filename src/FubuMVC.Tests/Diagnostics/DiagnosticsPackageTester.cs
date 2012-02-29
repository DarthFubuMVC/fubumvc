using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsPackageTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x => x.IncludeDiagnostics(true)).BuildGraph();
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

        [Test]
        public void fubu_request_is_overriden()
        {
            graph.Services.DefaultServiceFor<IFubuRequest>().Type.ShouldEqual(typeof (RecordingFubuRequest));
        }

    }
}