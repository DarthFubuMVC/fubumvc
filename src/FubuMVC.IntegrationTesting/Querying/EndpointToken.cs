using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.IntegrationTesting.Querying
{
    public class EndpointToken
    {
        public EndpointToken()
        {
        }

        public EndpointToken(BehaviorChain chain)
        {
            RoutePattern = chain is RoutedChain ? chain.As<RoutedChain>().GetRoutePattern() : string.Empty;
            Actions = chain.Calls.Select(x => new ActionToken(x)).ToArray();
        }

        public string RoutePattern { get; set; }
        public ActionToken[] Actions { get; set; }

        public string FirstActionDescription
        {
            get { return HasActions() ? Actions.First().Description : "None"; }
        }

        public bool HasActions()
        {
            return Actions.Any();
        }

        public bool IsFromAssembly(string assemblyName)
        {
            if (!Actions.Any()) return false;

            return Actions.First().HandlerType.Assembly.Name == assemblyName;
        }
    }
}