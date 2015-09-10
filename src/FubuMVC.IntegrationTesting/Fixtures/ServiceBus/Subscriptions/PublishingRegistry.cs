using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Subscriptions
{
    public class PublishingRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public PublishingRegistry()
        {
            NodeName = "Publishing";
            Channel(x => x.Publisher1).ReadIncoming();
        }
    }
}