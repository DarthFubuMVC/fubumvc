using System.Collections.Generic;
using FubuMVC.Core;
using FubuTransportation.Configuration;
using FubuTransportation.Polling;
using ServiceNode;
using StructureMap;

namespace WebsiteNode
{
    public class WebsiteApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            var container = new Container(x => {
                x.For<MessageRecorder>().Singleton();
            });

            return FubuTransport.For<WebsiteRegistry>(container);
        }
    }

    public class WebsiteRegistry : FubuTransportRegistry<TestBusSettings>
    {
        public WebsiteRegistry()
        {
            Channel(x => x.Website).ReadIncoming();
            Channel(x => x.Service).AcceptsMessagesInAssemblyContainingType<ServiceApplication>();

            HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);
        }
    }

    public class MessageRecorder
    {
        public IList<string> Messages = new List<string>(); 
    }
}