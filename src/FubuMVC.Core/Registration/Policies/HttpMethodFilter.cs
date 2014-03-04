using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class HttpMethodFilter : IChainFilter, DescribesItself
    {
        private readonly string _method;

        public HttpMethodFilter(string method)
        {
            _method = method.ToUpper();
        }

        public bool Matches(BehaviorChain chain)
        {
            var routed = chain as RoutedChain;
            return routed == null ? false : routed.Route.RespondsToMethod(_method);
        }

        public void Describe(Description description)
        {
            description.Title = "Responds to Http {0}'s".ToFormat(_method);
        }
    }
}