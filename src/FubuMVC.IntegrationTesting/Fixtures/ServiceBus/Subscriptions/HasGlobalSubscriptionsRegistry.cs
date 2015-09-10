using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Subscriptions
{
    public class HasGlobalSubscriptionsRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public HasGlobalSubscriptionsRegistry()
        {
            NodeName = "GlobalSubscriber";
            Channel(x => x.Subscriber1).ReadIncoming();

            SubscribeAt(x => x.Subscriber1)
                .ToSource(x => x.Publisher1)
                .ToMessage<OneMessage>();
        }
    }
}