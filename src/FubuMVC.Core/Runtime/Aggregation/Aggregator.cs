using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.UI;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class Aggregator : IAggregator, IAggregatorSource
    {
        private readonly IPartialInvoker _invoker;
        private readonly IFubuRequest _models;
        private readonly IChainResolver _resolver;

        public Aggregator(IPartialInvoker invoker, IFubuRequest models, IChainResolver resolver)
        {
            _invoker = invoker;
            _models = models;
            _resolver = resolver;
        }

        public object ExecuteChain(BehaviorChain chain, object input = null)
        {
            if (input != null)
            {
                _models.Set(chain.InputType(), input);
            }

            _invoker.InvokeFast(chain);

            var resourceType = chain.ResourceType();
            return _models.Has(resourceType) ? _models.Get(resourceType) : null;
        }


        public IEnumerable<object> Fetch(AggregateRequest request)
        {
            return request.Aggregate(this);
        }

        public IEnumerable<object> Fetch(Action<AggregateRequest> configure)
        {
            var request = new AggregateRequest();
            configure(request);

            return Fetch(request);
        }

        public object ForInputType(Type inputType)
        {
            var chain = _resolver.FindUniqueByType(inputType);
            return ExecuteChain(chain);
        }

        public object ForQuery<T>(T query)
        {
            var chain = _resolver.FindUniqueByType(typeof(T));
            return ExecuteChain(chain, query);
        }

        public object ForResource(Type resourceType)
        {
            var chain =
                _resolver.Find(new ChainSearch {Type = resourceType, TypeMode = TypeSearchMode.ResourceModelOnly});

            return ExecuteChain(chain);
        }

        public object ForAction<T>(Expression<Func<T, object>> expression)
        {
            var chain = _resolver.Find(typeof (T), ReflectionHelper.GetMethod(expression));
            return ExecuteChain(chain);
        }
    }
}