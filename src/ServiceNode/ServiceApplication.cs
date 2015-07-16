using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuTransportation.Configuration;
using FubuTransportation.Polling;

namespace ServiceNode
{
    public class ServiceApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuTransport.For<ServiceRegistry>();
        }
    }

    public class ServiceRegistry : FubuTransportRegistry<TestBusSettings>
    {
        public ServiceRegistry()
        {
            Channel(x => x.Service).ReadIncoming();
            HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);
        }
    }
}