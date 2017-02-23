using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using Xunit;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    
    public class PollingServicesRegistry_spec
    {
        [Fact]
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