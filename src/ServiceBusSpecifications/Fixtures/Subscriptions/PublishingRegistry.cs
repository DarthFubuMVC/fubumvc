using FubuMVC.Core.ServiceBus.Configuration;

namespace ServiceBusSpecifications.Fixtures.Subscriptions
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