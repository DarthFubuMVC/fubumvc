using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
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
            graph = new FubuRegistry(x => new DiagnosticsPackage().Configure(x)).BuildGraph();
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

        [Test]
        public void object_resolver_is_overridden()
        {
            graph.Services.DefaultServiceFor<IObjectResolver>().Type.ShouldEqual(typeof (RecordingObjectResolver));
        }

        [Test]
        public void output_writer_is_overridden()
        {
            graph.Services.DefaultServiceFor<IOutputWriter>().Type.ShouldEqual(typeof (DebuggingOutputWriter));
        }

        [Test]
        public void request_data_is_overriden()
        {
            graph.Services.DefaultServiceFor<IRequestData>().Type.ShouldEqual(typeof (RecordingRequestData));
        }
    }
}