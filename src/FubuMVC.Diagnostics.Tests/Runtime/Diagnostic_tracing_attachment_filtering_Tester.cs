using System.Linq;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Endpoints;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class Diagnostic_tracing_attachment_filtering_Tester
    {
        #region Setup/Teardown

        [SetUp]
        public void setup()
        {
            theGraph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<SomeEndpoints>();
                x.Actions.IncludeType<OtherEndpoints>();
                x.Actions.IncludeType<EndpointExplorerFubuDiagnostics>();

                //x.Actions.IncludeType<EndpointExplorerFubuDiagnostics>();
                x.Import<DiagnosticsRegistration>();
            });
        }

        #endregion

        private BehaviorGraph theGraph;


        [Test]
        public void should_be_attached_to_normal_endpoints()
        {
            theGraph.BehaviorFor<SomeEndpoints>(x => x.M1(null)).First().ShouldBeOfType<DiagnosticNode>();
            theGraph.BehaviorFor<SomeEndpoints>(x => x.M2(null)).First().ShouldBeOfType<DiagnosticNode>();
        }


        [Test]
        public void should_not_attach_to_endpoints_marked_by_No_diagnostics_on_method()
        {
            theGraph.BehaviorFor<SomeEndpoints>(x => x.M3(null)).First().ShouldNotBeOfType<DiagnosticNode>();
        }

        [Test]
        public void should_not_attach_to_endpoint_classes_marked_by_NoDiagnostics_on_class()
        {
            theGraph.BehaviorFor<OtherEndpoints>(x => x.M1(null)).First().ShouldNotBeOfType<DiagnosticNode>();
            theGraph.BehaviorFor<OtherEndpoints>(x => x.M2(null)).First().ShouldNotBeOfType<DiagnosticNode>();
        }


        public class SomeEndpoints
        {
            public void M1(Input1 input){}
            public void M2(Input2 input){}

            [NoDiagnostics]
            public void M3(Input3 input){}
        }

        [NoDiagnostics]
        public class OtherEndpoints
        {
            public Output1 M1(Input1 input)
            {
                return null;
            }

            public Output2 M2(Input2 input)
            {
                return null;
            }
        }

        public class Input1{}
        public class Input2{}
        public class Input3{}

        public class Output1{}
        public class Output2{}
    }
}