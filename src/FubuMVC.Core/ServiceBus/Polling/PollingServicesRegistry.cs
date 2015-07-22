using FubuMVC.Core.Registration;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingServicesRegistry : ServiceRegistry
    {
        public PollingServicesRegistry()
        {
            SetServiceIfNone<ITimer, DefaultTimer>();
            AddService<IDeactivator, PollingJobDeactivator>();
            SetServiceIfNone<IPollingJobLogger, PollingJobLogger>();

            SetServiceIfNone<IPollingJobs, PollingJobs>();

            SetServiceIfNone(typeof(PollingJobLatch), new ObjectInstance(new PollingJobLatch()));
        }
    }
}