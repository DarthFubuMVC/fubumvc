using FubuMVC.Core.ServiceBus.Configuration;
using FubuTransportation.Storyteller.Support;

namespace FubuTransportation.Storyteller.Fixtures.Subscriptions
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