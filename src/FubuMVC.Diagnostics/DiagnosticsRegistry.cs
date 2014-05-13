using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Model;
using FubuMVC.Diagnostics.Visualization;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticsRegistry : FubuPackageRegistry
    {
        // default policies are good enough

        public DiagnosticsRegistry()
        {
            Policies.Local.Add<DiagnosticAuthorizationPolicy>();
        }
    }

    public class DiagnosticServiceRegistry : ServiceRegistry
    {
        public DiagnosticServiceRegistry()
        {
            SetServiceIfNone<IVisualizer, Visualizer>();
            SetServiceIfNone<IDiagnosticContext, DiagnosticContext>();

            
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