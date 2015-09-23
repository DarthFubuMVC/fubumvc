using FubuMVC.Core.ServiceBus.Configuration;
using TestMessages.ScenarioSupport;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Subscriptions
{
    public class HasGlobalSubscriptionsRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public HasGlobalSubscriptionsRegistry()
        {
            NodeName = "GlobalSubscriber";
            Channel(x => x.Website1).ReadIncoming();

            SubscribeAt(x => x.Website1)
                .ToSource(x => x.Service1)
                .ToMessage<OneMessage>();
        }
    }
}