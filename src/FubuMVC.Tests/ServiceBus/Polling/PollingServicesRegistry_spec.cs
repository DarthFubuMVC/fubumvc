using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    [TestFixture]
    public class PollingServicesRegistry_spec
    {
        [Test]
        public void service_registrations()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                var container = runtime.Get<IContainer>();

                container.DefaultRegistrationIs<ITimer, DefaultTimer>();
                container.DefaultRegistrationIs<IPollingJobLogger, PollingJobLogger>();

                container.ShouldHaveRegistration<IDeactivator, PollingJobDeactivator>();

                container.DefaultSingletonIs<IPollingJobs, PollingJobs>();
            }
        }


    }
}