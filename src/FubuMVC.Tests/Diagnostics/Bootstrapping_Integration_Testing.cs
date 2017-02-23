using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.StructureMap.Diagnostics;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Diagnostics
{
    
    public class Bootstrapping_Integration_Testing : IDisposable
    {
        private FubuRuntime runtime;

        public Bootstrapping_Integration_Testing()
        {
            runtime = FubuRuntime.Basic(_ => _.Mode = "development");

            runtime.Get<FubuDiagnosticsEndpoint>().get__fubu();
        }

        public void Dispose()
        {
            runtime.Dispose();
        }

        [Fact]
        public void find_the_routes_and_chains_from_extensions()
        {
            runtime.Behaviors.ChainFor<StructureMapFubuDiagnostics>(x => x.get_search_options())
                .ShouldNotBeNull();

            runtime.Behaviors.ChainFor<ModelBindingFubuDiagnostics>(x => x.get_binding_all())
                .ShouldNotBeNull();
        }

        [Fact]
        public void got_all_the_routes_in_the_diagnostic_javascript_router()
        {
            var routes = runtime.Get<DiagnosticJavascriptRoutes>();
            var names = routes.Routes().Select(x => x.Name).ToArray();

            names.ShouldContain("StructureMap:summary");
            names.ShouldContain("Chain:chain_details_Hash");
            names.ShouldContain("Requests:requests");
        }
    }
}