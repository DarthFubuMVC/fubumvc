using FubuMVC.Core.ServiceBus.Configuration;
using TestMessages.ScenarioSupport;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Subscriptions
{
    public class HasLocalSubscriptionsRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public HasLocalSubscriptionsRegistry()
        {
            NodeName = "LocalSubscriber";
            Channel(x => x.Website1).ReadIncoming();

            SubscribeLocally()
                .ToSource(x => x.Service1)
                .ToMessage<OneMessage>()
                .ToMessage<TwoMessage>();
        }
    }
}