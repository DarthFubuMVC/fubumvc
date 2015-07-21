using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class ChannelVisualization
    {
        public ChannelGraph Graph { get; set; }
        public TransportsTag Transports { get; set; }
        public ChannelsTableTag Channels { get; set; }
        public SerializersTag Serializers { get; set; }
        public SubscriptionsTableTag Subscriptions { get; set; }
    }
}
