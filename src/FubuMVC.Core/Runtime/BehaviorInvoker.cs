using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Logging;
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

        public void Invoke(TypeArguments arguments, IDictionary<string, object> routeValues, IRequestCompletion requestCompletion)
        {
            var currentChain = new CurrentChain(_chain, routeValues);
            arguments.Set(typeof(ICurrentChain), currentChain);
            arguments.Set(typeof(IRequestCompletion), requestCompletion);

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

            
            requestCompletion.WhenCompleteDo(x =>
            {
                if (x != null)
                {
                    try
                    {
                        _factory.Get<ILogger>().Error("Async route failure at " + arguments.Get<IHttpRequest>().FullUrl(), x);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                var disposable = behavior as IDisposable;
                disposable?.Dispose();
            });

            behavior.Invoke();
        }

        public void Invoke(TypeArguments arguments)
        {
            var completion = new RequestCompletion();

            completion.SafeStart(() => {
                Invoke(arguments, new Dictionary<string, object>(), completion);
            });
        }
    }
}