using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorFactory
    {
        IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId);
    }

    public static class BehaviorFactoryExtensions
    {
        public static IActionBehavior BuildBehavior(this IBehaviorFactory factory, ServiceArguments arguments, BehaviorChain chain, IDictionary<string, object> routeValues)
        {
            var currentChain = new CurrentChain(chain, routeValues);

            arguments.Set(typeof(ICurrentChain), currentChain);

            return factory.BuildBehavior(arguments, chain.UniqueId);
        }
    }
}