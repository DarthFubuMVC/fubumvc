using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    [TestFixture]
    public class Service_registration_specs
    {
        [Test]
        public void registrations()
        {
            using (var runtime = FubuTransport.DefaultPolicies())
            {
                var container = runtime.Get<IContainer>();

                container.ShouldHaveRegistration<ILogModifier, PersistentTaskMessageModifier>();

                container.DefaultRegistrationIs<IPersistentTaskController, PersistentTaskController>();
                container.DefaultRegistrationIs<ITaskMonitoringSource, TaskMonitoringSource>();
            }
        }
    }
}