using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using ServiceNode;

namespace WebsiteNode
{
    public class WebsiteRegistry : FubuTransportRegistry<TestBusSettings>
    {
        public WebsiteRegistry()
        {
            Channel(x => x.Website).ReadIncoming();
            Channel(x => x.Service).AcceptsMessagesInAssemblyContainingType<ServiceRegistry>();

            ServiceBus.HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);
        }
    }

    public class MessageRecorder
    {
        public IList<string> Messages = new List<string>();
    }
}