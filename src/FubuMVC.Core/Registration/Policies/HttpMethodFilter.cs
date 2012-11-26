using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

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
            if (chain.Route == null) return false;
            if (chain.IsPartialOnly) return false;

            return chain.Route.RespondsToMethod(_method);
        }

        public void Describe(Description description)
        {
            description.Title = "Responds to Http {0}'s".ToFormat(_method);
        }
    }
}