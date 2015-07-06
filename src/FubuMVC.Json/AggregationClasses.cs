using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Aggregation;
using FubuMVC.Core.UI;

namespace FubuMVC.Json
{
    public class ClientRequest
    {
        public string type { get; set; }
        public object query { get; set; }
    }

    public class ClientResponse
    {
        public string type { get; set; }
        public string request { get; set; }
        public object result { get; set; }
    }

    public class AggregationRequest
    {
        public ClientRequest[] requests { get; set; }
    }

    public class AggregationResponse
    {
        public ClientResponse[] responses { get; set; }
    }

    public class AggregatorEndpoint
    {
        private readonly IPartialInvoker _invoker;
        private readonly IClientMessageCache _messageTypes;

        public AggregatorEndpoint(IPartialInvoker invoker, IClientMessageCache messageTypes)
        {
            _invoker = invoker;
            _messageTypes = messageTypes;
        }

        public AggregationResponse get__aggregated__query(AggregationRequest request)
        {
            return new AggregationResponse
            {
                responses = request.requests.Select(executeQuery).ToArray()
            };
        }

        private ClientResponse executeQuery(ClientRequest request)
        {
            // TODO -- blow up w/ message type missing
            var chain = _messageTypes.FindChain(request.type);
            var output = _invoker.InvokeFast(chain, request.query);

            return new ClientResponse
            {
                request = request.type,
                type = output.GetType().GetMessageName(),
                result = output
            };
        }
    }
}