using FubuMVC.Core.ServiceBus.Configuration;
using ServiceBusSerenitySamples.SystemUnderTest.Subscriptions;

namespace ServiceBusSerenitySamples.Setup
{
    public class ClientRegistry : FubuTransportRegistry<MultipleEndpointsSettings>
    {
        public ClientRegistry()
        {
            Channel(x => x.Client)
                .AcceptsMessage<MessageForExternalService>()
                .ReadIncoming();
        }
    }

    public class AnotherServiceRegistry : FubuTransportRegistry<MultipleEndpointsSettings>
    {
        public AnotherServiceRegistry()
        {
            Channel(x => x.AnotherService)
                .ReadIncoming();

            SubscribeLocally()
                .ToSource(x => x.SystemUnderTest)
                .ToMessage<PublishedEvent>();
        }
    }
}