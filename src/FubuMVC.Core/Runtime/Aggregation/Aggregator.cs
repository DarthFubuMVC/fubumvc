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
            return _invoker.InvokeFast(chain, null);
        }

        public object ForQuery<T>(T query)
        {
            var chain = _resolver.FindUniqueByType(typeof(T));
            return _invoker.InvokeFast(chain, query);
        }

        public object ForResource(Type resourceType)
        {
            var chain =
                _resolver.Find(new ChainSearch {Type = resourceType, TypeMode = TypeSearchMode.ResourceModelOnly});

            return _invoker.InvokeFast(chain, null);
        }

        public object ForAction<T>(Expression<Func<T, object>> expression)
        {
            var chain = _resolver.Find(typeof (T), ReflectionHelper.GetMethod(expression));
            return _invoker.InvokeFast(chain, null);
        }
    }
}