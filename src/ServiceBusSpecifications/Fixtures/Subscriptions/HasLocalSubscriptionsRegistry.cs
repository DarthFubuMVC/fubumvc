using FubuMVC.Core.ServiceBus.Configuration;
using ServiceBusSpecifications.Support;

namespace ServiceBusSpecifications.Fixtures.Subscriptions
{
    public class HasLocalSubscriptionsRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public HasLocalSubscriptionsRegistry()
        {
            NodeName = "LocalSubscriber";
            Channel(x => x.Subscriber1).ReadIncoming();

            SubscribeLocally()
                .ToSource(x => x.Publisher1)
                .ToMessage<OneMessage>()
                .ToMessage<TwoMessage>();
        }
    }
}