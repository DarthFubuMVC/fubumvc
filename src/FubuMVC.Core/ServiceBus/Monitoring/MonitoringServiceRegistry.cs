using FubuCore.Logging;
using FubuMVC.Core.Registration;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class MonitoringServiceRegistry : ServiceRegistry
    {
        public MonitoringServiceRegistry()
        {
            AddService<ILogModifier, PersistentTaskMessageModifier>();
            SetServiceIfNone<IPersistentTaskController, PersistentTaskController>(def => def.SetLifecycleTo<SingletonLifecycle>());
            SetServiceIfNone<ITaskMonitoringSource, TaskMonitoringSource>();
        }
    }
}