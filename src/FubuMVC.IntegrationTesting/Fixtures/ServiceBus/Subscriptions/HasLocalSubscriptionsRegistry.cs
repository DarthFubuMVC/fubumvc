using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Subscriptions
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