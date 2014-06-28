using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Visualization;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticsRegistry : FubuPackageRegistry
    {
        // default policies are good enough

        public DiagnosticsRegistry()
        {
            Actions.IncludeType<FubuDiagnosticsEndpoint>();
            Policies.ChainSource<DiagnosticChainsSource>();

            Policies.Local.Add<DiagnosticAuthorizationPolicy>();

            Policies.Global.Add<DefaultHome>();
        }
    }

    public class DefaultHome : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            if (!graph.Behaviors.OfType<RoutedChain>().Any(x => x.GetRoutePattern().IsEmpty()))
            {
                var action = ActionCall.For<DefaultHome>(x => x.GoToDiagnostics());
                var continuer = new ContinuationNode();

                var chain = new RoutedChain("");
                chain.Route.AddHttpMethodConstraint("GET");
                chain.AddToEnd(action);
                chain.AddToEnd(continuer);

                graph.AddChain(chain);
            }
        }

        public FubuContinuation GoToDiagnostics()
        {
            return FubuContinuation.RedirectTo<FubuDiagnosticsEndpoint>(x => x.get__fubu());
        }
    }

    public class DiagnosticServiceRegistry : ServiceRegistry
    {
        public DiagnosticServiceRegistry()
        {
            SetServiceIfNone<IVisualizer, Visualizer>();

            
        }
    }

    public class DiagnosticAuthorizationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<DiagnosticsSettings>();

            graph.Behaviors.Each(x => x.Authorization.AddPolicies(settings.AuthorizationRights));
        }
    }

    
}