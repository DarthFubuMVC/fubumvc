using FubuMVC.Core.ServiceBus.Configuration;
using TestMessages.ScenarioSupport;

namespace Specifications.Fixtures.ServiceBus.Subscriptions
{
    public class PublishingRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public PublishingRegistry()
        {
            NodeName = "Publishing";
            Channel(x => x.Service1).ReadIncoming();
        }
    }
}