using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class BehaviorInvoker : IBehaviorInvoker
    {
        private readonly IBehaviorFactory _factory;
        private readonly BehaviorChain _chain;

        public BehaviorInvoker(IBehaviorFactory factory, BehaviorChain chain)
        {
            _factory = factory;
            _chain = chain;
        }

        public void Invoke(ServiceArguments arguments, IDictionary<string, object> routeValues, Action onComplete)
        {
            var currentChain = new CurrentChain(_chain, routeValues);
            arguments.Set(typeof(ICurrentChain), currentChain);

            if (_chain.Filters.Any(filter => filter.Filter(arguments) == DoNext.Stop))
            {
                return;
            }

            var behavior = _factory.BuildBehavior(arguments, _chain.UniqueId).As<IEntrypointActionBehavior>();
            Invoke(behavior, onComplete);
        }

        protected virtual void Invoke(IEntrypointActionBehavior behavior, Action onComplete)
        {
            using (behavior)
            {
                behavior.Invoke();
            }
            onComplete();
        }
    }
}