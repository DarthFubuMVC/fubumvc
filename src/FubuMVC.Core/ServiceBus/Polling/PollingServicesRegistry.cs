using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingServicesRegistry : ServiceRegistry
    {
        public PollingServicesRegistry()
        {
            SetServiceIfNone<ITimer, DefaultTimer>();
            AddService<IDeactivator, PollingJobDeactivator>();
            SetServiceIfNone<IPollingJobLogger, PollingJobLogger>();

            SetServiceIfNone<IPollingJobs, PollingJobs>().Singleton();


            For<PollingJobLatch>().Singleton();
        }
    }
}