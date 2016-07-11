using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using StructureMap.Pipeline;

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

        public async Task Invoke(TypeArguments arguments, IDictionary<string, object> routeValues)
        {
            var currentChain = new CurrentChain(_chain, routeValues);
            arguments.Set(typeof(ICurrentChain), currentChain);

            if (arguments.Has(typeof (IChainExecutionLog)))
            {
                arguments.Get<IChainExecutionLog>().RootChain = _chain;
            }

            if (_chain.Filters.Any(filter => filter.Filter(arguments) == DoNext.Stop))
            {
                return;
            }

            IActionBehavior behavior = null;

            if (arguments.Has(typeof (IChainExecutionLog)))
            {
                arguments.Get<IChainExecutionLog>().Trace("Building the Behaviors", () =>
                {
                    behavior = _factory.BuildBehavior(arguments, _chain.UniqueId);
                });
            }
            else
            {
                behavior = _factory.BuildBehavior(arguments, _chain.UniqueId);
            }

            try
            {
                await behavior.Invoke().ConfigureAwait(false);
            }
            finally
            {
                (behavior as IDisposable)?.Dispose();
            }
        }

    }
}