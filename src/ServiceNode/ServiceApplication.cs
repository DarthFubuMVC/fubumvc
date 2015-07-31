using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;

namespace ServiceNode
{
    public class ServiceRegistry : FubuTransportRegistry<TestBusSettings>
    {
        public ServiceRegistry()
        {
            ServiceBus.Enable(true);
            Channel(x => x.Service).ReadIncoming();
            ServiceBus.HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);
        }
    }
}