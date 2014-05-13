using System.Linq;
using FubuCore;
using FubuCore.Binding.InMemory;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Diagnostics.Dashboard;
using FubuMVC.Diagnostics.Model;
using FubuMVC.Diagnostics.Visualization;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticsRegistration : IFubuRegistryExtension
    {
        public const string DIAGNOSTICS_URL_ROOT = "_fubu";

        public void Configure(FubuRegistry registry)
        {
            registry.Policies.ChainSource<DiagnosticChainsSource>();
            registry.Services<DiagnosticServiceRegistry>();

            registry.AlterSettings<DiagnosticsSettings>(x => {
                x.TraceLevel = TraceLevel.Verbose;
            });

            registry.Configure(graph => {
                var settings = graph.Settings.Get<DiagnosticsSettings>();

                if (settings.TraceLevel == TraceLevel.Verbose)
                {
                    graph.Services.Clear(typeof(IBindingLogger));
                    graph.Services.AddService<IBindingLogger, RecordingBindingLogger>();

                    graph.Services.Clear(typeof(IBindingHistory));
                    graph.Services.AddService<IBindingHistory, BindingHistory>();

                    graph.Services.AddService<ILogListener, RequestTraceListener>();
                }

                if (settings.TraceLevel == TraceLevel.Production)
                {
                    graph.Services.AddService<ILogListener, ProductionModeTraceListener>();
                }
            });

            registry.Policies.Global.Add<DefaultHome>();
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

                var chain = new RoutedChain ("");
                chain.Route.AddHttpMethodConstraint("GET");
                chain.AddToEnd(action);
                chain.AddToEnd(continuer);

                graph.AddChain(chain);
            }
        }

        public FubuContinuation GoToDiagnostics()
        {
            return FubuContinuation.RedirectTo<DashboardFubuDiagnostics>(x => x.Index(null));
        }
    }
}