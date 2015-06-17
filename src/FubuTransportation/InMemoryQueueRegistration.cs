using FubuMVC.Core.Registration;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Runtime;

namespace FubuTransportation
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