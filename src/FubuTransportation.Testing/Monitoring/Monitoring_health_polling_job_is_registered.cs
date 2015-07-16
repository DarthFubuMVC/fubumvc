using System.Linq;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Monitoring;
using FubuTransportation.Polling;
using FubuTransportation.Runtime.Delayed;
using NUnit.Framework;

namespace FubuTransportation.Testing.Monitoring
{
    [TestFixture]
    public class Monitoring_health_polling_job_is_registered
    {
        [Test]
        public void the_job_is_registered()
        {
            using (var runtime = FubuTransport.DefaultPolicies().StructureMap().Bootstrap())
            {
                runtime.Factory.Get<IPollingJobs>().Any(x => x is PollingJob<HealthMonitorPollingJob, HealthMonitoringSettings>)
                    .ShouldBeTrue();
            }
        }
    }
}