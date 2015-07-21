using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus
{
    [System.ComponentModel.Description("Register the InMemoryTransport if enabled")]
    public class InMemoryQueueRegistration : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var enabled = FubuTransport.AllQueuesInMemory ||
                          graph.Settings.Get<TransportSettings>().EnableInMemoryTransport;

            if (enabled)
            {
                graph.Services.AddService<ITransport, InMemoryTransport>();
            }
        }
    }
}