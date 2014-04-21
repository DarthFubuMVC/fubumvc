using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Diagnostics.Model;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class RoutesRequest{}

    public class EndpointExplorerFubuDiagnostics
    {
        private static readonly string Namespace = Assembly.GetExecutingAssembly().GetName().Name;

        private readonly BehaviorGraph _graph;
        private readonly IUrlRegistry _urls;

        public EndpointExplorerFubuDiagnostics(BehaviorGraph graph, IUrlRegistry urls)
        {
            _graph = graph;
            _urls = urls;
        }

        [System.ComponentModel.Description("Endpoints:Table of all the configured routes, partials, and handlers in a FubuMVC application")]
        public EndpointExplorerModel get_endpoints(RoutesRequest request)
        {
            var reports = _graph.Behaviors.Where(IsNotDiagnosticRoute).Select(x => RouteReport.ForChain(x, _urls)).OrderBy(x => x.Title);

            return new EndpointExplorerModel
            {
                EndpointsTable = new EndpointsTable(reports)
            };
        }

        public static bool IsNotDiagnosticRoute(BehaviorChain chain)
        {
            if (chain is DiagnosticChain) return false;


            if (chain.Calls.Any(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()))
            {
                return false;
            }

            if (chain.Calls.Any(x => x.HasInput && x.InputType().Assembly == Assembly.GetExecutingAssembly()))
            {
                return false;
            }

            if (chain.HasOutput() && chain.ResourceType().Assembly == Assembly.GetExecutingAssembly())
            {
                return false;
            }

            return true;
        }
    }
}