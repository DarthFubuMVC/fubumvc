using FubuMVC.Core;
using FubuMVC.Core.Registration.Conventions;
using FubuTransportation.Async;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Monitoring;
using FubuTransportation.Polling;
using FubuTransportation.Registration.Nodes;
using FubuTransportation.Sagas;
using FubuTransportation.ScheduledJobs.Configuration;

namespace FubuTransportation
{
    public class FubuTransportationExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Global.Add<ApplyScheduledJobRouting>();
            registry.Services<ScheduledJobServicesRegistry>();
            registry.Services<MonitoringServiceRegistry>();
            registry.Policies.Global.Add<RegisterScheduledJobs>();
            registry.Policies.ChainSource<ImportHandlers>();
            registry.Services<FubuTransportServiceRegistry>();
            registry.Services<PollingServicesRegistry>();
            registry.Policies.Global.Add<RegisterPollingJobs>();
            registry.Policies.Global.Add<StatefulSagaConvention>();
            registry.Policies.Global.Add<AsyncHandlingConvention>();

            if (FubuTransport.AllQueuesInMemory)
            {
                registry.Policies.Global.Add<AllQueuesInMemoryPolicy>();
            }

            registry.Policies.Global.Add<InMemoryQueueRegistration>();

            registry.Policies.Global.Add<ReorderBehaviorsPolicy>(x =>
            {
                x.ThisNodeMustBeBefore<StatefulSagaNode>();
                x.ThisNodeMustBeAfter<HandlerCall>();
            });
        }
    }
}