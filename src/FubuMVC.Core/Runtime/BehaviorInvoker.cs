using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class BehaviorInvoker : IBehaviorInvoker
    {
        private readonly IServiceFactory _factory;
        private readonly BehaviorChain _chain;

        public BehaviorInvoker(IServiceFactory factory, BehaviorChain chain)
        {
            _factory = factory;
            _chain = chain;
        }

        public void Invoke(ServiceArguments arguments, IDictionary<string, object> routeValues, IRequestCompletion requestCompletion)
        {
            var currentChain = new CurrentChain(_chain, routeValues);
            arguments.Set(typeof(ICurrentChain), currentChain);
            arguments.Set(typeof(IRequestCompletion), requestCompletion);

            if (_chain.Filters.Any(filter => filter.Filter(arguments) == DoNext.Stop))
            {
                return;
            }
            var behavior = _factory.BuildBehavior(arguments, _chain.UniqueId);
            requestCompletion.WhenCompleteDo(x =>
            {
                var disposable = behavior as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            });

            behavior.Invoke();
        }

        public void Invoke(ServiceArguments arguments)
        {
            var completion = new RequestCompletion();

            completion.SafeStart(() => {
                Invoke(arguments, new Dictionary<string, object>(), completion);
            });
        }
    }
}