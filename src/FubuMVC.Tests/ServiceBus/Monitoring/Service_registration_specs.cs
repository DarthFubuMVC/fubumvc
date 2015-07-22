using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    [TestFixture]
    public class Service_registration_specs
    {
        [Test]
        public void registrations()
        {
            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                runtime.Container.ShouldHaveRegistration<ILogModifier, PersistentTaskMessageModifier>();

                runtime.Container.DefaultRegistrationIs<IPersistentTaskController, PersistentTaskController>();
                runtime.Container.DefaultRegistrationIs<ITaskMonitoringSource, TaskMonitoringSource>();
            }
        }
    }
}