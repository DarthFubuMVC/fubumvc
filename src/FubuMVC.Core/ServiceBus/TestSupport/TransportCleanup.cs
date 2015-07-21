using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.Core.Services.Messaging;

namespace FubuMVC.Core.ServiceBus.TestSupport
{
    public class TransportCleanup : IListener<ClearAllTransports>
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