using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Monitoring;
using Xunit;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    
    public class Service_registration_specs
    {
        [Fact]
        public void registrations()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                var container = runtime.Get<IContainer>();

                container.ShouldHaveRegistration<ILogModifier, PersistentTaskMessageModifier>();

                container.DefaultRegistrationIs<IPersistentTaskController, PersistentTaskController>();
                container.DefaultRegistrationIs<ITaskMonitoringSource, TaskMonitoringSource>();
            }
        }
    }
}