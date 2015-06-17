using System.Collections.Generic;
using System.Diagnostics;
using FubuTransportation.Runtime;
using FubuTransportation.Subscriptions;

namespace FubuTransportation.TestSupport
{
    public class TransportCleanup : Bottles.Services.Messaging.IListener<ClearAllTransports>
    {
        private readonly IEnumerable<ITransport> _transports;
        private readonly ISubscriptionCache _subscriptions;

        public TransportCleanup(IEnumerable<ITransport> transports, ISubscriptionCache subscriptions)
        {
            _transports = transports;
            _subscriptions = subscriptions;
        }

        public void Receive(ClearAllTransports message)
        {
            // Debug tracing will get written out into the Storyteller 
            // output
            ClearAll();
        }

        public void ClearAll()
        {
            _transports.Each(x => {
                x.ClearAll();
            });
            _subscriptions.ClearAll();
        }
    }
}