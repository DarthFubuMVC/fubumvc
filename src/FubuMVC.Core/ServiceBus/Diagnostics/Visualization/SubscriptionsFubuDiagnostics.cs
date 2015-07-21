using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class SubscriptionsFubuDiagnostics
    {
        private readonly ISubscriptionPersistence _persistence;

        public SubscriptionsFubuDiagnostics(ISubscriptionPersistence persistence)
        {
            _persistence = persistence;
        }

        [System.ComponentModel.Description("Subscriptions:Visualizes the persisted subscriptions in this active node")]
        public SubscriptionsViewModel get_subscriptions()
        {
            return new SubscriptionsViewModel
            {
                Persistence = new DescriptionBodyTag(Description.For(_persistence)),
                Nodes = new TransportNodeTableTag(_persistence.AllNodes()),
                Subscriptions = new SubscriptionStorageTableTag(_persistence.AllSubscriptions())
            };
        }
    }

    public class SubscriptionsViewModel
    {
        public DescriptionBodyTag Persistence { get; set; }
        public TransportNodeTableTag Nodes { get; set; }
        public SubscriptionStorageTableTag Subscriptions { get; set; }
    }
}