using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Sagas;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration;

namespace FubuMVC.Core.ServiceBus
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