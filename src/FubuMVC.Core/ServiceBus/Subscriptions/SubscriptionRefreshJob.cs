using System.Threading;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.Core.ServiceBus.Subscriptions
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