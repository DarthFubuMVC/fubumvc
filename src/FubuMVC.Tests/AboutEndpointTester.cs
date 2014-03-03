using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class AboutEndpointTester
    {
        [TearDown]
        public void TearDown()
        {
            FubuMode.Reset();
        }

        [Test]
        public void no_about_endpoint_if_not_in_development_mode()
        {
            FubuMode.Reset();

            var graph = BehaviorGraph.BuildEmptyGraph();
            graph.BehaviorFor<AboutDiagnostics>(x => x.get__about())
                .ShouldBeNull();
        }

        [Test]
        public void adds_the_about_endpoint_if_in_development_mode()
        {
            FubuMode.Mode(FubuMode.Development);

            var graph = BehaviorGraph.BuildEmptyGraph();
            graph.BehaviorFor<AboutDiagnostics>(x => x.get__about())
                .ShouldNotBeNull();
        }
    }
}