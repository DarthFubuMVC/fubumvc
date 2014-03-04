using System;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly specifies an additional route for the chain containing this method as its ActionCall.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class UrlAliasAttribute : ModifyChainAttribute
    {
        private readonly string _pattern;

        public UrlAliasAttribute(string pattern)
        {
            _pattern = pattern;
        }

        public override void Alter(ActionCall call)
        {
            var chain = call.ParentChain();
            var alias = call.BuildRouteForPattern(_pattern);

            chain.As<RoutedChain>().AddRouteAlias(alias);
        }
    }
}