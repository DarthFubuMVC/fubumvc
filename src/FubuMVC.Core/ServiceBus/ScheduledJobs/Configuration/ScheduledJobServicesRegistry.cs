using FubuCore.Logging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    public class ScheduledJobServicesRegistry : ServiceRegistry
    {
        public ScheduledJobServicesRegistry()
        {
            SetServiceIfNone<IScheduledJobController, ScheduledJobController>().Singleton();
            SetServiceIfNone<IJobTimer, JobTimer>().Singleton();

            SetServiceIfNone<ISchedulePersistence>(new InMemorySchedulePersistence());

            SetServiceIfNone<IScheduleStatusMonitor, ScheduleStatusMonitor>();

            AddService<IPersistentTaskSource, ScheduledJobPersistentTask>();
            AddService<ILogModifier, ScheduledJobRecordModifier>();
        }
    }
}