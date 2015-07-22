using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using NUnit.Framework;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    [TestFixture]
    public class PollingServicesRegistry_spec
    {
        [Test]
        public void service_registrations()
        {
            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                runtime.Container.DefaultRegistrationIs<ITimer, DefaultTimer>();
                runtime.Container.DefaultRegistrationIs<IPollingJobLogger, PollingJobLogger>();

                runtime.Container.ShouldHaveRegistration<IDeactivator, PollingJobDeactivator>();

                runtime.Container.DefaultSingletonIs<IPollingJobs, PollingJobs>();
            }
        }


    }
}