using System.Threading;
using FubuTransportation.Polling;

namespace FubuTransportation.Subscriptions
{
    public class SubscriptionRefreshJob : IJob
    {
        private readonly SubscriptionsHandler _handler;

        public SubscriptionRefreshJob(SubscriptionsHandler handler)
        {
            _handler = handler;
        }

        public void Execute(CancellationToken cancellation)
        {
            _handler.ReloadSubscriptions();
        }
    }
}