using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class Aggregator : IAggregatorSource, IAggregator
    {
        private readonly IPartialInvoker _invoker;
        private readonly IChainResolver _resolver;
        private readonly IClientMessageCache _messageTypes;

        public Aggregator(IPartialInvoker invoker, IChainResolver resolver, IClientMessageCache messageTypes)
        {
            _invoker = invoker;
            _resolver = resolver;
            _messageTypes = messageTypes;
        }

        public AggregationResponse QueryAggregate(AggregatedQuery request)
        {
            return new AggregationResponse
            {
                responses = request.queries.Select(ExecuteQuery).ToArray()
            };
        }

        public ClientResponse ExecuteQuery(ClientQuery query)
        {
            var chain = _messageTypes.FindChain(query.type);
            var output = _invoker.InvokeFast(chain, query.query);

            return new ClientResponse
            {
                request = query.type,
                type = chain.ResourceType().GetMessageName(),
                result = output,
                correlationId = query.correlationId
            };
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
            var chain = _resolver.FindUniqueByType(typeof (T));
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
