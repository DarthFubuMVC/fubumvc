using System.Linq;
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
            RoutePattern = chain.RoutePattern;
            Actions = chain.Calls.Select(x => new ActionToken(x)).ToArray();
        }

        public string RoutePattern { get; set; }
        public ActionToken[] Actions { get; set; }
    }
}