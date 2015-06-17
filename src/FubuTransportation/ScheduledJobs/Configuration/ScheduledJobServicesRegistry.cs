using FubuCore.Logging;
using FubuMVC.Core.Registration;
using FubuTransportation.Monitoring;
using FubuTransportation.ScheduledJobs.Execution;
using FubuTransportation.ScheduledJobs.Persistence;

namespace FubuTransportation.ScheduledJobs.Configuration
{
    public class ScheduledJobServicesRegistry : ServiceRegistry
    {
        public ScheduledJobServicesRegistry()
        {
            SetServiceIfNone<IScheduledJobController, ScheduledJobController>(x => x.AsSingleton());
            SetServiceIfNone<IJobTimer, JobTimer>(x => x.AsSingleton());

            SetServiceIfNone<ISchedulePersistence>(new InMemorySchedulePersistence());

            SetServiceIfNone<IScheduleStatusMonitor, ScheduleStatusMonitor>();

            AddService<IPersistentTaskSource, ScheduledJobPersistentTask>();
            AddService<ILogModifier, ScheduledJobRecordModifier>();
        }
    }
}