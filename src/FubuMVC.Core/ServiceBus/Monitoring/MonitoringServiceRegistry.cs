using FubuCore.Logging;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class MonitoringServiceRegistry : ServiceRegistry
    {
        public MonitoringServiceRegistry()
        {
            AddService<ILogModifier, PersistentTaskMessageModifier>();
            SetServiceIfNone<IPersistentTaskController, PersistentTaskController>().Singleton();
            SetServiceIfNone<ITaskMonitoringSource, TaskMonitoringSource>();
        }
    }
}