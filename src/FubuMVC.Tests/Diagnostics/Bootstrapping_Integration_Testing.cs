using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.StructureMap;
using FubuMVC.StructureMap.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class Bootstrapping_Integration_Testing
    {
        private FubuRuntime runtime;

        [TestFixtureSetUp]
        public void SetUp()
        {
            FubuMode.SetUpForDevelopmentMode();
            runtime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap();

            runtime.Factory.Get<FubuDiagnosticsEndpoint>().get__fubu();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            runtime.Dispose();
        }

        [Test]
        public void find_the_routes_and_chains_from_bottles()
        {
            runtime.Behaviors.BehaviorFor<StructureMapFubuDiagnostics>(x => x.get_search_options())
                .ShouldNotBeNull();

            runtime.Behaviors.BehaviorFor<ModelBindingFubuDiagnostics>(x => x.get_binding_all())
                .ShouldNotBeNull();
        }

        [Test]
        public void builds_partials_for_Visualize_methods()
        {
            var chain = runtime.Behaviors.BehaviorFor<ModelBindingFubuDiagnostics>(x => x.VisualizePartial(null));

            chain.GetType().ShouldEqual(typeof (BehaviorChain));

            chain.IsPartialOnly.ShouldBeTrue();
        }

        [Test]
        public void got_all_the_routes_in_the_diagnostic_javascript_router()
        {
            var routes = runtime.Factory.Get<DiagnosticJavascriptRoutes>();
            var names = routes.Routes().Select(x => x.Name).ToArray();

            names.ShouldContain("StructureMap:summary");
            names.ShouldContain("Chain:chain_details_Hash");
            names.ShouldContain("Requests:requests");
        }


    }
}