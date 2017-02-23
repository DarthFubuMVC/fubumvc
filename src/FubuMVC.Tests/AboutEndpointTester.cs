using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests
{
    
    public class AboutEndpointTester
    {
        [Fact]
        public void no_about_endpoint_if_not_in_development_mode()
        {
            var graph = BehaviorGraph.BuildEmptyGraph();

            graph.ChainFor<AboutFubuDiagnostics>(x => x.get_about())
                .ShouldBeNull();
        }

        [Fact]
        public void adds_the_about_endpoint_if_in_development_mode()
        {
            var graph = BehaviorGraph.BuildFrom(x => x.Mode = "development");
            graph.ChainFor<AboutFubuDiagnostics>(x => x.get_about())
                .ShouldNotBeNull();
        }
    }
}