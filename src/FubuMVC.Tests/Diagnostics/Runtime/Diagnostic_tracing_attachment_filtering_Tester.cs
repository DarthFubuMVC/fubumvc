using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Endpoints;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Diagnostics.Runtime
{
    
    public class Diagnostic_tracing_attachment_filtering_Tester
    {
        public Diagnostic_tracing_attachment_filtering_Tester()
        {
            theGraph = BehaviorGraph.BuildFrom(x =>
            {
                x.Mode = "development";

                x.Actions.IncludeType<SomeEndpoints>();
                x.Actions.IncludeType<OtherEndpoints>();
                x.Actions.IncludeType<EndpointExplorerFubuDiagnostics>();


            });
        }

        private BehaviorGraph theGraph;


        [Fact]
        public void should_be_attached_to_normal_endpoints()
        {
            theGraph.ChainFor<SomeEndpoints>(x => x.M1(null)).First().ShouldBeOfType<BehaviorTracerNode>();
            theGraph.ChainFor<SomeEndpoints>(x => x.M2(null)).First().ShouldBeOfType<BehaviorTracerNode>();
        }


        [Fact]
        public void should_not_attach_to_endpoints_marked_by_No_diagnostics_on_method()
        {
            theGraph.ChainFor<SomeEndpoints>(x => x.M3(null)).First().ShouldNotBeOfType<BehaviorTracerNode>();
        }

        [Fact]
        public void should_not_attach_to_endpoint_classes_marked_by_NoDiagnostics_on_class()
        {
            theGraph.ChainFor<OtherEndpoints>(x => x.M1(null)).First().ShouldNotBeOfType<BehaviorTracerNode>();
            theGraph.ChainFor<OtherEndpoints>(x => x.M2(null)).First().ShouldNotBeOfType<BehaviorTracerNode>();
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