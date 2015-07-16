using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuTransportation.Polling
{
    public class PollingServicesRegistry : ServiceRegistry
    {
        public PollingServicesRegistry()
        {
            // NEED MORE.
            SetServiceIfNone<ITimer, DefaultTimer>();
            AddService<IDeactivator, PollingJobDeactivator>();
            SetServiceIfNone<IPollingJobLogger, PollingJobLogger>();

            var def = ObjectDef.ForType<PollingJobs>();
            def.IsSingleton = true;
            SetServiceIfNone(typeof (IPollingJobs), def);

            SetServiceIfNone(typeof (PollingJobLatch), ObjectDef.ForValue(new PollingJobLatch()).AsSingleton());
        }
    }
}