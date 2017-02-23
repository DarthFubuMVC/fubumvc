using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    
    public class Monitoring_health_polling_job_is_registered
    {
        [Fact]
        public void the_job_is_registered()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                runtime.Get<IPollingJobs>().Any(x => x is PollingJob<HealthMonitorPollingJob, HealthMonitoringSettings>)
                    .ShouldBeTrue();
            }
        }
    }
}