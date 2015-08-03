using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class AboutEndpointTester
    {
        [Test]
        public void no_about_endpoint_if_not_in_development_mode()
        {
            var graph = BehaviorGraph.BuildEmptyGraph();

            graph.ChainFor<AboutFubuDiagnostics>(x => x.get_about())
                .ShouldBeNull();
        }

        [Test]
        public void adds_the_about_endpoint_if_in_development_mode()
        {
            var graph = BehaviorGraph.BuildFrom(x => x.Mode = "development");
            graph.ChainFor<AboutFubuDiagnostics>(x => x.get_about())
                .ShouldNotBeNull();
        }
    }
}