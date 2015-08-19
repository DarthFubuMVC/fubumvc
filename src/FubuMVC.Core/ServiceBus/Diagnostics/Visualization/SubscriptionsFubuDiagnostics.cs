using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Subscriptions;
using HtmlTags;

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
        public HtmlTag get_subscriptions()
        {
            var div = new HtmlTag("div");

            div.Add("h3").Text("Subscription Persistence");
            div.Append( new DescriptionBodyTag(Description.For(_persistence)));

            div.Add("h3").Text("Nodes");
            div.Append(new TransportNodeTableTag(_persistence.AllNodes()));

            div.Add("h3").Text("Subscriptions");
            div.Append(new SubscriptionStorageTableTag(_persistence.AllSubscriptions()));

            return div;
        }
    }

    public class SubscriptionsViewModel
    {
        public DescriptionBodyTag Persistence { get; set; }
        public TransportNodeTableTag Nodes { get; set; }
        public SubscriptionStorageTableTag Subscriptions { get; set; }
    }
}