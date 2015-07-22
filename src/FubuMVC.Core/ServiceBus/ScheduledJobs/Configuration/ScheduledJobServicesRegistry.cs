using FubuCore.Logging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    public class ScheduledJobServicesRegistry : ServiceRegistry
    {
        public ScheduledJobServicesRegistry()
        {
            SetServiceIfNone<IScheduledJobController, ScheduledJobController>(x => x.SetLifecycleTo<SingletonLifecycle>());
            SetServiceIfNone<IJobTimer, JobTimer>(x => x.SetLifecycleTo<SingletonLifecycle>());

            SetServiceIfNone<ISchedulePersistence>(new InMemorySchedulePersistence());

            SetServiceIfNone<IScheduleStatusMonitor, ScheduleStatusMonitor>();

            AddService<IPersistentTaskSource, ScheduledJobPersistentTask>();
            AddService<ILogModifier, ScheduledJobRecordModifier>();
        }
    }
}