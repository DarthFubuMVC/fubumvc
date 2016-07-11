using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public async Task<AggregationResponse> QueryAggregate(AggregatedQuery request)
        {
            var responses = new ClientResponse[request.queries.Length];
            for (int i = 0; i < request.queries.Length; i++)
            {
                responses[i] = await ExecuteQuery(request.queries[i]).ConfigureAwait(false);
            }

            return new AggregationResponse
            {
                responses = responses
            };
        }

        public async Task<ClientResponse> ExecuteQuery(ClientQuery query)
        {
            var chain = _messageTypes.FindChain(query.type);
            var output = await _invoker.InvokeFast(chain, query.query).ConfigureAwait(false);

            return new ClientResponse
            {
                request = query.type,
                type = chain.ResourceType().GetMessageName(),
                result = output,
                correlationId = query.correlationId
            };
        }

        public Task<IEnumerable<object>> Fetch(AggregateRequest request)
        {
            return request.Aggregate(this);
        }

        public Task<IEnumerable<object>> Fetch(Action<AggregateRequest> configure)
        {
            var request = new AggregateRequest();
            configure(request);

            return Fetch(request);
        }

        public Task<object> ForInputType(Type inputType)
        {
            var chain = _resolver.FindUniqueByType(inputType);
            return _invoker.InvokeFast(chain, null);
        }

        public Task<object> ForQuery<T>(T query)
        {
            var chain = _resolver.FindUniqueByType(typeof (T));
            return _invoker.InvokeFast(chain, query);
        }

        public Task<object> ForResource(Type resourceType)
        {
            var chain =
                _resolver.Find(new ChainSearch {Type = resourceType, TypeMode = TypeSearchMode.ResourceModelOnly});

            return _invoker.InvokeFast(chain, null);
        }

        public Task<object> ForAction<T>(Expression<Func<T, object>> expression)
        {
            var chain = _resolver.Find(typeof (T), ReflectionHelper.GetMethod(expression));
            return _invoker.InvokeFast(chain, null);
        }
    }
}
