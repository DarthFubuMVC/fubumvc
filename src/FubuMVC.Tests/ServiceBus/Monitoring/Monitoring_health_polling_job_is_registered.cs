using System.Linq;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    [TestFixture]
    public class Monitoring_health_polling_job_is_registered
    {
        [Test]
        public void the_job_is_registered()
        {
            using (var runtime = FubuTransport.DefaultPolicies())
            {
                runtime.Get<IPollingJobs>().Any(x => x is PollingJob<HealthMonitorPollingJob, HealthMonitoringSettings>)
                    .ShouldBeTrue();
            }
        }
    }
}