using System;
using System.Linq;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Querying
{
    public class EndpointToken
    {
        public EndpointToken()
        {
        }

        public EndpointToken(BehaviorChain chain)
        {
            RoutePattern = chain.GetRoutePattern();
            Actions = chain.Calls.Select(x => new ActionToken(x)).ToArray();
        }

        public string RoutePattern { get; set; }
        public ActionToken[] Actions { get; set; }

        public bool HasActions()
        {
            return Actions.Any();
        }

        public bool IsFromAssembly(string assemblyName)
        {
            if (!Actions.Any()) return false;

            return Actions.First().HandlerType.Assembly.Name == assemblyName;
        }

        public string FirstActionDescription
        {
            get
            {
                return HasActions() ? Actions.First().Description : "None";
            }
        }
    }
}